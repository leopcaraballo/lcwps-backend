# ADR-0001: INITIAL ARCHITECTURE DECISION

## LCWPS Microservices + Event-Driven + PostgreSQL + Kubernetes

**Status:** ✓ ACCEPTED (Partially superseded by ARCHITECTURE.md update 2026-02-23)
**Date:** 2026-02-23
**Architect:** Chief Enterprise Architect + CTO
**Review Board:** Architecture Review Board
**Supersedes:** None (Initial decision)
**Superseded By:** None

---

## ADDENDUM (2026-02-23)

The core microservices + event-driven decision remains binding. The business model update introduced:

- Financial Service removed (no automated financial validation).
- Payment Verification Service added (manual verification workflow).
- Notification Service expanded as a core system function.

See ARCHITECTURE.md for the current baseline.

---

## 1. CONTEXT

### 1.1 Problem Statement

LCWPS is a critical healthcare infrastructure system managing clinical queue orchestration with mandatory financial pre-validation. Key constraints:

**Functional Requirements:**

- Multi-tenant clinic system (10+ clinics, 100K+ patients)
- Real-time queue management (FIFO with priority classification)
- Financial validation before queue entry (insurance verification)
- Immutable audit logs (5-year retention; healthcare compliance)
- Row-level security for patient data isolation
- Integration with external insurance providers (sync, API validation)

**Non-Functional Requirements:**

- Availability: 99.8% SLA (≤ 8.6 hours downtime/month)
- Response time: 95th percentile ≤ 500ms for queue operations
- Isolation: Multi-tenant data segregation at application + database layers
- Security: Zero Trust architecture, HIPAA-equivalent controls, NIST alignment
- Regulatory: Colombian (Ley 1581, Decreto 1377, Ley 23, Supersalud) + ISO 27001 + OWASP Top 10

### 1.2 Constraints

- **Technology Stack (Mandatory):** .NET 10 LTS (no alternatives)
- **Database:** PostgreSQL with Row-Level Security
- **Deployment:** Kubernetes (no alternative container orchestrators)
- **Architecture Pattern:** Microservices (monoliths explicitly prohibited)
- **Data Sharing:** No shared databases across services (forbidden)
- **Compliance:** Colombian regulatory framework (immutable)

---

## 2. ARCHITECTURE DECISION

### 2.1 Selected Architecture

**Microservices + Event-Driven Architecture**

```
┌────────────────┐     ┌──────────────────┐     ┌──────────────────┐
│  API Gateway   │     │  Auth Service    │     │ Patient Service  │
│  (YARP/.NET)   │────▶│  (OAuth2 + JWT)  │──▶  │  (Patient CRUD)  │
└────────────────┘     └──────────────────┘     └──────────────────┘
         │                                               │
         │                                               │ events
         │              ┌──────────────────┐            ▼
         ├─────────────▶│  Queue Service   │◀────┬──────────────────┐
         │              │  (FIFO + Priority│     │ Financial Svc    │
         │              └──────────────────┘     │ (Validation)     │
         │                    │ events           └──────────────────┘
         │                    ▼
         │              ┌──────────────────┐
         ├─────────────▶│ Provider Service │
         │              │ (Doctor/Clinic)  │
         │              └──────────────────┘
         │
         └──────────────┬──────────────────┐
                        │ events           ▼
                        │         ┌──────────────────┐
                        │         │ Notification Svc │
                        │         │ (SMS/Email/Push) │
                        │         └──────────────────┘
                        │
                        ▼
                  ┌──────────────────┐
                  │  RabbitMQ Topic  │
                  │  Exchanges:      │
                  │  - clinic.*      │
                  │  - financial.*   │
                  │  - patient.*     │
                  │  - audit.*       │
                  └──────────────────┘
```

### 2.2 Technology Stack

| Component | Selection | Binding |
|-----------|-----------|---------|
| **Language** | C# 13 + .NET 10 LTS | MANDATORY |
| **Database** | PostgreSQL 15+ | MANDATORY |
| **Message Broker** | RabbitMQ 12 | MANDATORY |
| **Container Runtime** | Docker 24+ | MANDATORY |
| **Orchestrator** | Kubernetes 1.28+ | MANDATORY |
| **Service Mesh** | Istio (mTLS + observability) | MANDATORY |
| **API Gateway** | YARP (.NET) or nginx | MANDATORY |
| **Auth Framework** | OAuth2.0 + IdentityServer4 | MANDATORY |
| **ORM** | Entity Framework Core 8 | MANDATORY |
| **Event Bus** | MassTransit (.NET abstraction over RabbitMQ) | MANDATORY |
| **Observability** | OpenTelemetry + Prometheus + Grafana | MANDATORY |
| **Secrets Management** | AWS KMS / HashiCorp Vault | MANDATORY |

---

## 3. CONSEQUENCES (TRADE-OFFS)

### 3.1 Positive Consequences

✓ **Scalability:** Independent service scaling; horizontal Pod Autoscaler for traffic spikes
✓ **Resilience:** Service isolation prevents cascade failures; circuit breaker patterns
✓ **Operational Independence:** Teams own services end-to-end (deployment, monitoring)
✓ **Technology Flexibility:** Language/technology choices per service (within .NET constraint)
✓ **Regulatory Alignment:** Event sourcing enables immutable audit trails (5-year compliance)
✓ **Zero Trust Native:** Inter-service mTLS (Istio) + database RLS combines to prevent lateral movement
✓ **Data Protection:** Each service owns data; multi-tenant isolation enforced at DB layer (RLS)

### 3.2 Negative Consequences

✗ **Operational Complexity:** 8+ services to monitor, scale, deploy; requires Kubernetes expertise
✗ **Distributed Transactions:** No ACID across multiple services; must use Saga pattern (compensating transactions)
✗ **Network Latency:** Service-to-service calls over network (vs. in-process); mitigated by async event model
✗ **Data Consistency:** Eventually consistent model; transactional boundaries within service only
✗ **Debugging Difficulty:** Request tracing required across service boundaries; mitigated by OpenTelemetry
✗ **Development Overhead:** Local development requires Docker Compose to run all services

### 3.3 Mitigation Strategies

| Challenge | Mitigation |
|-----------|-----------|
| **Complexity** | Kubernetes operators (cert-manager, sealed-secrets); Infrastructure as Code (Helm) |
| **Distributed Transactions** | Saga pattern + event sourcing; idempotent operations |
| **Network Latency** | Async event model; caching (Redis) for frequently accessed data |
| **Data Consistency** | Event-driven architecture ensures eventual consistency; tests validate invariants |
| **Debugging** | OpenTelemetry traces span services; logs correlated by trace ID |
| **Local Development** | Docker Compose file for all services + hot-reload support |

---

## 4. ALTERNATIVES CONSIDERED (REJECTED)

### 4.1 Monolithic Architecture (REJECTED)

**Option:** Single .NET application managing all domains.

**Rejection Rationale:**

- ❌ Cannot meet 99.8% SLA; single service failure = entire system down
- ❌ Multi-tenancy enforcement weaker (no database-level RLS isolation)
- ❌ Teams blocked on deployment (single CI/CD pipeline)
- ❌ Regulatory audit trail harder to enforce (no event immutability pattern)
- ❌ Violates decision constraint: "Microservices required"

### 4.2 Synchronous REST Between Services (REJECTED)

**Option:** Direct HTTP service-to-service calls instead of RabbitMQ.

**Rejection Rationale:**

- ❌ Tight coupling; service failures cascade (timeout chains)
- ❌ No guaranteed delivery; lost messages in network partition
- ❌ Harder to audit financial validation flow (no event immutability)
- ❌ Cannot scale financial processing asynchronously

**Decision:** Async event-driven (RabbitMQ) for all inter-service communication.

### 4.3 Shared Database (REJECTED)

**Option:** All services access single shared PostgreSQL schema (normalized schema).

**Rejection Rationale:**

- ❌ Tight coupling; schema changes block all services
- ❌ No service autonomy; shared migration pipeline
- ❌ Multi-tenancy enforcement only at application layer (RLS harder to enforce uniformly)
- ❌ Violates decision constraint: "No shared databases"

**Decision:** Each service owns its database schema; join data via events.

### 4.4 Different Languages Per Service (REJECTED)

**Option:** Queue service in Go, Financial service in Python, etc.

**Rejection Rationale:**

- ❌ Increases operational complexity (multiple runtimes, CI/CD tools, logging formats)
- ❌ Team context switching costs
- ❌ Regulatory audits harder to verify (different security postures)

**Decision:** All services in .NET 10 (within constraint).

---

## 5. IMPLEMENTATION STRATEGY

### 5.1 Phase 1: Service Extraction (Month 1-2)

1. **Auth Service:** Extract OAuth2.0 + JWT validation
2. **Queue Service:** Extract FIFO + priority logic
3. **Financial Service:** Extract insurance validation

### 5.2 Phase 2: Event Bus Integration (Month 3)

1. RabbitMQ cluster setup
2. MassTransit integration in services
3. Event sourcing pattern implementation

### 5.3 Phase 3: Kubernetes Deployment (Month 4)

1. Docker image creation + registry
2. Helm charts for service deployment
3. Service mesh (Istio) setup + mTLS

### 5.4 Phase 4: Observability & Compliance (Month 5)

1. OpenTelemetry agent deployment
2. Prometheus scraping + Grafana dashboards
3. Audit log immutability validation
4. RLS policy testing (multi-tenant isolation)

---

## 6. VALIDATION STRATEGY

### 6.1 Architectural Compliance Tests

```csharp
[Fact]
public void Architecture_ShouldEnforceLayerDependencies()
{
    var architecture = new ArchLoader().LoadAssemblies(...)
        .Build();

    // Presentation layer should not reference Domain layer directly
    Classes()
        .That()
        .ResideInNamespace("Presentation")
        .Should()
        .NotDependOn()
        .Classes()
        .That()
        .ResideInNamespace("Domain")
        .Check(architecture);
}

[Fact]
public void Services_ShouldNotShareDatabase()
{
    // Validate: Queue service == queue_service schema
    //           Financial service == financial_service schema

    var queueDb = GetDatabaseSchema("queue_service");
    var financialDb = GetDatabaseSchema("financial_service");

    var overlap = queueDb.Tables.Intersect(financialDb.Tables);
    Assert.Empty(overlap);  // No shared tables
}
```

### 6.2 Multi-Tenancy Isolation Tests

```sql
-- Test: Clinic B staff cannot see Clinic A patients
SET ROLE clinic_b_doctor;
SET app.current_tenant = 'clinic-b-id'::uuid;

SELECT COUNT(*) FROM patients WHERE tenant_id = 'clinic-a-id'::uuid;
-- Result should be 0 (RLS enforces this)
```

### 6.3 Event Ordering & Consistency Tests

```csharp
[Fact]
public async Task PatientApprovalAndNotification_ShouldCompleteInOrder()
{
    // Ensure: PatientApprovedEvent published
    //    →    NotificationService subscribes
    //    →    SMS sent

    var approvalEvent = new PatientApprovedEvent(...);
    await _eventBus.PublishAsync(approvalEvent);

    // Verify in RabbitMQ DLQ archive
    var dlq = GetDeadLetterQueue();
    Assert.Empty(dlq);  // No failed messages
}
```

---

## 7. ROLLBACK STRATEGY

If architecture proves unworkable:

1. **Service 1 Failure:** Isolate failed service; redirect traffic to synchronous fallback
2. **Event Bus Failure:** Database-backed queue (polling) as fallback; automatic revert to RabbitMQ when healthy
3. **Multi-Tenancy Failure:** Application-layer filtering fallback; no database-level RLS (reduced security)

**Rollback Decision Gate:** If any service has >99% error rate for >15 minutes, escalate to on-call CTO for decision.

---

## 8. ARCHITECTURAL REVIEW SCHEDULE

| Milestone | Review | Decision |
|-----------|--------|----------|
| **Week 2** | Phase 1 completion | Approve Queue service extraction |
| **Week 6** | Phase 2 completion | Approve RabbitMQ event model |
| **Week 10** | Phase 3 completion | Approve Kubernetes deployment |
| **Week 14** | Phase 4 completion | Approve observability + compliance |
| **Month 6** | Full system validation | ADR superseded or affirmed |

---

## 9. RELATED ADRs

- ADR-0002: Event Sourcing Strategy (pending)
- ADR-0003: Multi-Tenancy Enforcement Model (pending)
- ADR-0004: Saga Pattern for Financial Transactions (pending)

---

## 10. APPROVAL SIGNATURES

| Role | Name | Signature | Date |
|------|------|-----------|------|
| **CTO** | [CEO-Authorized] | _____________ | 23-02-2026 |
| **Chief Architect** | [Enterprise Arch] | _____________ | 23-02-2026 |
| **CISO** | [Security] | _____________ | 23-02-2026 |
| **DPO** | [Data Protection] | _____________ | 23-02-2026 |

---

**Document Status:** ✓ APPROVED – Architecture Binding
**Review Interval:** Quarterly (ADR review by Architecture Board)
**Next Review Date:** 23-05-2026 (Q2)
