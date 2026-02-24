# ARCHITECTURE BASELINE – LCWPS

## Non-Negotiable Technical Baseline

**Document Authority:** CTO + Chief Enterprise Architect
**Classification:** Architecture Binding Standard
**Effective:** 2026-02-23
**Updated:** 2026-02-23 (Manual Verification Model)
**Status:** FINAL – No alternatives permitted
**Version:** 2.0

---

## 📢 ARCHITECTURAL UPDATE NOTICE (2026-02-23)

**System Redefinition:**

LCWPS has transitioned from an **automated financial validation system** to a **real-time orchestration and notification system with manual payment verification**.

**Key Changes:**

1. ❌ **Removed:** Financial Service (automated validation)
2. ✅ **Added:** Payment Verification Service (manual workflow)
3. ✅ **Expanded:** Notification Service (now core system)
4. ✅ **New Focus:** Real-time notifications (< 1s latency)

**Core Architecture:** Microservices + Event-Driven remains unchanged
**Technology Stack:** .NET 10 + PostgreSQL + RabbitMQ + Kubernetes remains unchanged

### Resumen (ES)

- Arquitectura de microservicios y eventos se mantiene.
- Verificacion de pago es manual y obligatoria antes de `WAITING`.
- Servicio de notificaciones es nucleo funcional con baja latencia.

---

## 1. ARCHITECTURE OVERVIEW

### 1.1 Architecture Type

**Model:** Microservices + Event-Driven Architecture
**Technology Stack:** .NET 10 LTS + PostgreSQL + RabbitMQ + Kubernetes
**Pattern Enforcement:** Clean Architecture + Domain-Driven Design
**Data Model:** Event Sourcing for clinical events
**Deployment:** Kubernetes + Docker + GitOps
**Observability:** OpenTelemetry + Prometheus + Grafana

**Alternative Models:** ❌ NOT PERMITTED

- Monoliths
- Layered architecture without domain boundaries
- Shared databases across services
- Synchronous inter-service RPC
- Manual deployment
- Print-based logging

---

## 2. TECHNOLOGY STACK (IMMUTABLE)

### 2.1 Platform

| Component | Version | Binding | Rationale |
|-----------|---------|---------|-----------|
| **.NET** | 10 LTS | MANDATORY | LTS support until 2029, modern async/await |
| **Language** | C# 13+ | MANDATORY | Type safety, non-null reference types |
| **PostgreSQL** | 15+ | MANDATORY | ACID compliance, RLS native support |
| **Container Runtime** | Docker 24+ | MANDATORY | Image signing, vulnerability scanning |
| **Orchestrator** | Kubernetes 1.28+ | MANDATORY | Multi-region, auto-healing, GitOps capable |
| **Message Broker** | RabbitMQ 12 | MANDATORY | Pub/Sub, dead-letter queues, HA support |
| **Observability** | OpenTelemetry | MANDATORY | Vendor-neutral instrumentation |
| **Metrics** | Prometheus 2.45+ | MANDATORY | Time-series DB, alerting rules |
| **Visualization** | Grafana 10+ | MANDATORY | Dashboard, SLA monitoring |

### 2.2 Development Tools

| Component | Version | Binding |
|-----------|---------|---------|
| **IDE** | Visual Studio 2024 / JetBrains Rider | RECOMMENDED |
| **Build** | dotnet CLI 10 | MANDATORY |
| **Package Manager** | NuGet (internal feed) | MANDATORY |
| **Testing Frameworks** | xUnit + Moq + FluentAssertions | MANDATORY |
| **Code Analysis** | SonarQube + Roslyn analyzers | MANDATORY |
| **Dependency Scanner** | Snyk / WhiteSource | MANDATORY |
| **Container Scanner** | Trivy | MANDATORY |
| **Git Hooks** | Husky.NET | RECOMMENDED |

---

## 3. MICROSERVICES DECOMPOSITION (BINDING)

### 3.1 Service Inventory (UPDATED 2026-02-23)

**System Redefinition:** LCWPS is now a **real-time orchestration and notification system** with manual payment verification, NOT an automated financial validation system.

| Service | Responsibility | Data Owner | Technology | Status |
|---------|-----------------|-----------|-----------|--------|
| **Queue Service** | FIFO management, priority classification, appointment orchestration | Queue DB | .NET 10 + EF Core | ✓ Active |
| **Payment Verification Service** | ✅ **NEW**: Manual payment verification workflow, verification audit | Verification DB | .NET 10 + PostgreSQL RLS | ✓ Active |
| **Notification Service** | ✅ **CORE**: Real-time notifications (WebSocket, Mobile, Email), event distribution | Events | .NET 10 + SignalR + async handlers | ✓ Active |
| **Provider Service** | Clinic/doctor credentials, availability | Provider DB | .NET 10 + PostgreSQL RLS | ✓ Active |
| **Patient Service** | Demographics, medical history (SOAP) | Patient DB | .NET 10 + PostgreSQL RLS | ✓ Active |
| **Analytics Service** | Reporting, KPI metrics, audit queries | Analytics DB | .NET 10 + analytic views | ✓ Active |
| **Auth Service** | OAuth2.0, JWT, MFA, RBAC | Auth DB | .NET 10 + IdentityServer | ✓ Active |
| **API Gateway** | Request routing, authentication, rate limiting | N/A | YARP (.NET) or nginx | ✓ Active |
| ~~**Financial Service**~~ | ~~Pre-validation, insurance coverage, payment~~ | ~~Financial DB~~ | ~~.NET 10 + PostgreSQL RLS~~ | ❌ **REMOVED** |

### 3.2 Key Architectural Changes

**What Changed:**

1. **Financial Service (REMOVED)**
   - Previously: Automated insurance validation, payment processing
   - Reason: System no longer performs automatic financial validation
   - Replacement: Manual verification via Payment Verification Service

2. **Payment Verification Service (NEW)**
   - Purpose: Workflow for manual payment verification by authorized staff
   - Core Functions:
     - Display appointments pending verification
     - Execute verification actions (approve/reject)
     - Audit trail of all verifications
     - Authorization enforcement (FinancialVerifier, Supervisor, Admin roles)

3. **Notification Service (EXPANDED - CORE SYSTEM)**
   - Promoted to **primary functional core** of the system
   - Responsibilities expanded:
     - Real-time WebSocket notifications
     - Mobile push notifications
     - Email notifications
     - Waiting room display panel updates
     - Event distribution to all subscribers
   - Performance targets:
     - < 1s latency for critical notifications
     - < 500ms for internal notifications
     - Idempotent delivery with retry logic

### 3.2 Service Isolation (MANDATORY)

Each service **MUST** have:

✓ **Independent Database** (no shared schemas)
✓ **Isolated Deployment** (separate container, pod)
✓ **Async Communication** (RabbitMQ events only)
✓ **Timeout Policies** (10s default, circuit breaker)
✓ **Health Checks** (liveness + readiness probes)
✓ **Monitoring** (OpenTelemetry instrumentation)
✓ **Rate Limiting** (Leaky bucket algorithm)
✓ **Bulkhead** (Thread pool isolation)

**Prohibited:**
❌ Synchronous service-to-service calls
❌ Shared database access
❌ Direct HTTP between services (except authorized API Gateway)
❌ Distributed transactions (use saga pattern)

---

## 4. DATABASE ARCHITECTURE

### 4.1 PostgreSQL (Single Instance per Service)

**Schema Design:**

```sql
-- Service Database Structure
CREATE SCHEMA service_name AUTHORIZATION service_user;

-- Row-Level Security (MANDATORY)
CREATE POLICY tenant_isolation ON service_name.table_name
  USING (tenant_id = current_setting('app.current_tenant')::UUID);

-- Audit Trail (IMMUTABLE)
CREATE TABLE service_name.audit_log (
  id BIGSERIAL PRIMARY KEY,
  table_name VARCHAR NOT NULL,
  operation VARCHAR NOT NULL,
  old_values JSONB,
  new_values JSONB,
  changed_by UUID NOT NULL,
  changed_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
  tenant_id UUID NOT NULL
);

-- Immutability Trigger
CREATE TRIGGER enforce_audit_log_immutability
  BEFORE UPDATE OR DELETE ON service_name.audit_log
  FOR EACH ROW EXECUTE FUNCTION deny_modification();
```

### 4.2 Row-Level Security (RLS) – NON-NEGOTIABLE

**Implementation Requirement:**

```sql
-- Enable RLS on all tables
ALTER TABLE patient_data ENABLE ROW LEVEL SECURITY;
ALTER TABLE financial_records ENABLE ROW LEVEL SECURITY;
ALTER TABLE clinical_notes ENABLE ROW LEVEL SECURITY;

-- Multi-tenant isolation
CREATE POLICY rls_tenant_isolation
  ON public.patient_data
  FOR ALL
  USING (tenant_id = CAST(current_setting('app.current_tenant') AS UUID))
  WITH CHECK (tenant_id = CAST(current_setting('app.current_tenant') AS UUID));

-- Role-based access
CREATE POLICY rls_doctor_access
  ON public.clinical_notes
  FOR SELECT
  USING (
    doctor_id = current_user_id()
    OR EXISTS (
      SELECT 1 FROM clinic_roles
      WHERE user_id = current_user_id()
      AND role = 'ADMINISTRATOR'
    )
  );
```

### 4.3 Backup & Disaster Recovery

| Requirement | Implementation | RPO | RTO |
|------------|---------------|----|-----|
| **Daily Backup** | pg_basebackup + WAL archiving | 1 hour | 4 hours |
| **Hot Standby** | Streaming replication | 0 minutes | 30 seconds |
| **Cross-Region** | S3 replication (AWS) | 8 hours | 2 hours |
| **Retention** | 30 days full backups | 30 days | Immediate |

---

## 5. EVENT-DRIVEN ARCHITECTURE

### 5.1 RabbitMQ Configuration

**Exchange Types (UPDATED):**

| Exchange | Type | Purpose | Binding | Status |
|----------|------|---------|---------|--------|
| **appointment.events** | Topic | Appointment lifecycle events | Mandatory | ✓ Active |
| **payment.events** | ✅ **NEW** | Manual payment verification events | Mandatory | ✓ Active |
| **notification.events** | Topic | Real-time notifications (expanded) | Mandatory | ✓ Active |
| **audit.events** | Topic | Immutable audit trail | Mandatory | ✓ Active |
| ~~**financial.events**~~ | ~~Topic~~ | ~~Financial transactions~~ | ~~Mandatory~~ | ❌ **REMOVED** |

**Queue Configuration:**

```csharp
public class EventBusConfiguration
{
    // Dead Letter Queue for failed events
    public const string DeadLetterQueue = "dlq.main";

    // Retry policy: exponential backoff
    // Attempt 1: immediate
    // Attempt 2: 5 seconds
    // Attempt 3: 25 seconds
    // Attempt 4: 125 seconds
    // Attempt 5+: manual intervention

    public const int MaxRetries = 5;
    public const int RetryMultiplier = 5;
}
```

### 5.2 Event Types (UPDATED 2026-02-23)

**Domain Events** (Published by aggregate roots):

**Appointment Lifecycle:**

- `AppointmentCreatedEvent` - New appointment created
- `PaymentPendingVerificationEvent` - Awaiting manual verification
- `PaymentVerifiedEvent` - Payment manually approved
- `PaymentRejectedEvent` - Payment manually rejected
- `AppointmentMovedToWaitingEvent` - Entered active queue
- `AppointmentCalledEvent` - Patient called for service
- `AppointmentInServiceEvent` - Service started
- `AppointmentCompletedEvent` - Service completed
- `AppointmentCancelledEvent` - Cancelled by patient or staff
- `AppointmentNoShowEvent` - Patient didn't respond to call

**Notifications:**

- `NotificationSentEvent` - Notification successfully delivered
- `NotificationFailedEvent` - Notification delivery failed
- `NotificationRetryScheduledEvent` - Retry scheduled

**Audit:**

- `PaymentVerificationAuditEvent` - Manual verification recorded
- `StateTransitionAuditEvent` - Appointment state changed
- `AccessAttemptAuditEvent` - Authorization check performed

**Integration Events** (Async handoffs between services):

- Serialization: JSON with schema versioning
- Headers: TraceId, TenantId, UserId, Timestamp, EventId (idempotency)
- Retention: Audit queue retention = 90 days
- Delivery: At-least-once with idempotency keys

---

## 6. CLEAN ARCHITECTURE + DDD

### 6.1 Layered Model (MANDATORY)

```
Service.{ServiceName} (root namespace)
├── Presentation/
│   ├── Controllers/
│   ├── DTOs/
│   ├── Validators/
│   └── Middleware/
├── Application/
│   ├── UseCases/
│   ├── Services/
│   ├── Mappers/
│   └── Events/
├── Domain/
│   ├── Entities/
│   ├── ValueObjects/
│   ├── Aggregates/
│   ├── Repositories/ (interfaces)
│   ├── Specifications/
│   └── DomainEvents/
├── Infrastructure/
│   ├── Persistence/
│   │   ├── EFCore/
│   │   ├── Migrations/
│   │   └── Repositories/
│   ├── ExternalServices/
│   ├── Caching/
│   └── Messaging/
└── Tests/
    ├── Unit/
    ├── Domain/
    ├── Integration/
    └── E2E/
```

### 6.2 Dependency Injection (Mandatory Pattern)

```csharp
// Services.AddApplicationLayer()
services.AddScoped<IQueueService, QueueService>();
services.AddScoped<IQueueRepository, QueueRepository>();
services.AddScoped<QueueCommandHandler>(/* ... */);

// Validate at startup
ArchUnitNet.Fluent
    .Classes()
    .That()
    .ResideInNamespace("Application")
    .Should()
    .NotDependOnAny()
    .Classes()
    .That()
    .ResideInNamespace("Presentation")
    .Because("Application layer must not reference Presentation")
    .Check(architecture);
```

### 6.3 Domain Invariants (Not Negotiable)

Every aggregate must enforce invariants in constructor:

```csharp
public class Appointment : AggregateRoot
{
    public Appointment(
        Guid tenantId,
        Guid patientId,
        Priority priority,
        PaymentVerification validation)
    {
        // Invariant 1: Payment verification must be complete
        if (validation.Status != ValidationStatus.Approved)
            throw new InvalidOperationException("Payment verification required");

        // Invariant 2: Priority must be valid
        if (priority == Priority.Unknown)
            throw new InvalidOperationException("Priority must not be Unknown");

        // Invariant 3: Tenant must be set
        if (tenantId == Guid.Empty)
            throw new InvalidOperationException("Tenant ID must not be empty");

        TenantId = tenantId;
        PatientId = patientId;
        Priority = priority;
        PaymentVerification = validation;
        CreatedAt = DateTime.UtcNow;

        AddDomainEvent(new AppointmentdEvent(tenantId, patientId));
    }
}
```

---

## 7. SECURITY ARCHITECTURE

### 7.1 Zero Trust Model (MANDATORY)

**Principle:** Never trust, always verify.

| Layer | Verification | Implementation |
|-------|--------------|-----------------|
| **Network** | Mutual TLS | Service mesh (Istio) + signed certificates |
| **Application** | OAuth2.0 + JWT | IdentityServer4 + MFA |
| **Data** | RLS policies | PostgreSQL RLS + audit logs |
| **Infrastructure** | Pod security policies | Kubernetes NetworkPolicy + RBAC |

### 7.2 Encryption Standards (MANDATORY)

**Transit (TLS 1.3):**

```nginx
ssl_protocols TLSv1.3 TLSv1.2;
ssl_ciphers ECDHE-RSA-AES256-GCM-SHA384:ECDHE-RSA-AES128-GCM-SHA256;
ssl_prefer_server_ciphers on;
```

**At-Rest (AES-256-GCM):**

```sql
-- PostgreSQL pgcrypto extension
-- Sensitive fields: encrypted in application layer before INSERT
-- Key rotation: every 90 days via KMS
```

### 7.3 RBAC Model

```csharp
public enum RoleType
{
    Administrator,      // Full system access
    ClinicManager,      // Clinic + payment verification oversight
    Doctor,             // Patient data + queue management
    Nurse,              // Patient data viewer
    FinancialVerifier,  // Payment verification only
    Patient             // Own record access only
}

// Authorization attribute
[Authorize(Roles = "Doctor,Nurse")]
[HttpGet("patients/{id}/history")]
public async Task<ActionResult<PatientHistory>> GetHistory(Guid id)
```

---

## 8. DEPLOYMENT ARCHITECTURE

### 8.1 Kubernetes Native

**Minimum Resources:**

```yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: queue-service
  namespace: lcwps
spec:
  replicas: 3  # High availability
  template:
    spec:
      serviceAccountName: queue-service
      securityContext:
        runAsNonRoot: true
        runAsUser: 1000
      containers:
      - name: queue-service
        image: registry.example.com/lcwps/queue-service:v1.0.0@sha256:abc...
        imagePullPolicy: Always
        livenessProbe:
          httpGet:
            path: /health/live
            port: 8080
          initialDelaySeconds: 10
          periodSeconds: 10
        readinessProbe:
          httpGet:
            path: /health/ready
            port: 8080
          initialDelaySeconds: 5
          periodSeconds: 5
        resources:
          requests:
            cpu: 500m
            memory: 512Mi
          limits:
            cpu: 1000m
            memory: 1Gi
```

### 8.2 Container Image Requirements

- ✓ **Base Image**: microsoft/dotnet:10-runtime-alpine (minimal attack surface)
- ✓ **Image Signing**: Signed with company GPG key
- ✓ **Vulnerability Scan**: Trivy scan with zero critical findings
- ✓ **Registry**: Private registry with access control
- ✓ **Tagging**: Semantic versioning + commit SHA reference
- ✓ **No Root User**: RunAsNonRoot: true in PodSecurityPolicy

---

## 9. OBSERVABILITY ARCHITECTURE

### 9.1 OpenTelemetry Instrumentation (Mandatory)

Every service **MUST** export:

- **Traces**: Per-request execution flow
- **Metrics**: Performance + business KPIs
- **Logs**: Structured logging (JSON)

```csharp
public class OpenTelemetrySetup
{
    public static void ConfigureOpenTelemetry(IServiceCollection services)
    {
        services
            .AddOpenTelemetry()
            .WithTracing(builder => builder
                .SetResourceBuilder(ResourceBuilder.CreateDefault()
                    .AddService("queue-service"))
                .AddAspNetCoreInstrumentation()
                .AddEntityFrameworkCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddOtlpExporter(options => options.Endpoint = new Uri("http://otel-collector:4317")))
            .WithMetrics(builder => builder
                .SetResourceBuilder(ResourceBuilder.CreateDefault()
                    .AddService("queue-service"))
                .AddAspNetCoreInstrumentation()
                .AddRuntimeInstrumentation()
                .AddOtlpExporter());
    }
}
```

### 9.2 Alerting Rules (Prometheus)

```yaml
groups:
- name: LCWPS Alerts
  rules:
  - alert: HighErrorRate
    expr: rate(http_requests_total{status=~"5.."}[5m]) > 0.05
    annotations:
      summary: "High error rate detected"
  - alert: QueueLatencySLA
    expr: histogram_quantile(0.95, http_request_duration_seconds) > 0.5
    annotations:
      summary: "Queue latency exceeds SLA (500ms)"
```

---

## 10. ARCHITECTURAL DECISION RECORDS (ADR)

All architectural decisions **MUST** be documented as ADRs in:

```
/docs/architecture/adr/
```

**ADR Template:**

```
# ADR-NNNN: [Decision Title]

Date: YYYY-MM-DD
Status: [Proposed | Accepted | Deprecated | Superseded by ADR-NNNN]
Context: [Problem statement]
Decision: [What was decided]
Consequences: [Trade-offs and impact]
Alternatives Considered: [Why not X, Y, Z]
```

---

## 11. ARCHITECTURE GOVERNANCE

### 11.1 Architecture Review Board (ARB)

- Meets bi-weekly
- Reviews all PRs touching architecture layers
- Authority to reject changes non-compliant with baseline
- Escalates conflicts to CTO

### 11.2 Architecture Validations (Automated)

```csharp
// ArchUnit tests (mandatory in CI pipeline)
[Fact]
public void LayerDependencies_ShouldNotViolateCleanArchitecture()
{
  var architecture = new ArchLoader().LoadAssemblies(
    typeof(Appointment).Assembly,
    typeof(IQueueRepository).Assembly,
    typeof(QueueController).Assembly)
    .Build();

  Classes()
    .That()
    .ResideInNamespace("Presentation")
    .Should()
    .NotDependOnAny()
    .Classes()
    .That()
    .ResideInNamespace("Domain")
    .Check(architecture);
}
```

## 12. TECHNOLOGY OBSOLESCENCE POLICY

Components entering EOL **MUST** be migrated:

| Component | Current Version | EOL Date | Replacement Path |
|-----------|-----------------|----------|------------------|
| .NET 10 | 10.0 | Nov 2029 | → .NET 15 (if released) |
| PostgreSQL 15 | 15.x | Oct 2026 | → PostgreSQL 16 |
| Kubernetes 1.28 | 1.28.x | Dec 2024 | → Kubernetes 1.30+ |

---

## 13. VALIDATION & CERTIFICATION

This architecture baseline is **FINAL and BINDING**.

**Certification Board:**

- Chief Enterprise Architect (✓ certifies)
- CISO (✓ certifies security compliance)
- CTO (✓ sign-off on technology choices)

**Next Review:** 2026-06-23 (Q2)
**Change Authority:** CTO + Chief Architect (minimum 2-person approval)

---

**Document Status:** ✓ APPROVED – No alternatives permitted
**Violation Escalation:** Immediate escalation to CTO + CISO
