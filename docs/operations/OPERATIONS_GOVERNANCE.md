# OPERATIONS GOVERNANCE – LCWPS

## SLA + Monitoring + Incident Response + Disaster Recovery

**Authority:** Chief Operations Officer, DevOps Lead
**Classification:** INFRASTRUCTURE CRITICAL
**Date:** 2026-02-23
**Status:** BINDING

### Resumen (ES)

- SLA 99.8% y tiempos de respuesta por severidad.
- Monitoreo con OpenTelemetry, Prometheus y alertas.
- Respuesta a incidentes y DR con RPO/RTO definidos.

---

## 1. Service Level Agreements (SLA)

### 1.1 Availability

**Target:** 99.8% availability (<= 8.6 hours unplanned downtime/month)

| Severity | Response Time | Resolution Time |
|----------|---------------|------------------|
| **P1 (Critical)** | 15 minutes | 1 hour |
| **P2 (High)** | 1 hour | 4 hours |
| **P3 (Medium)** | 4 hours | 24 hours |
| **P4 (Low)** | 24 hours | 1 week |

### 1.2 Performance

| Metric | Target | Measurement |
|--------|--------|-------------|
| Queue Lookup Latency | p95 < 100ms | OpenTelemetry histogram |
| Payment Verification | p95 < 500ms | Prometheus metric |
| API Gateway Latency | p95 < 200ms | YARP metrics |
| Database Query | p95 < 50ms | EF Core profiling |

### 1.3 SLA Credits

| Uptime | Credit |
|--------|--------|
| 99.8% - 99.5% | 10% monthly fee |
| 99.5% - 99.0% | 25% monthly fee |
| < 99.0% | 50% monthly fee |

---

## 2. Monitoring Architecture

### 2.1 Telemetry Stack

```
Application -> OpenTelemetry SDK -> OTLP Collector
   -> Prometheus (metrics) -> Alertmanager -> On-call (PagerDuty/Slack)
   -> Grafana Loki (logs) -> Grafana dashboards
```

### 2.2 Key Alerts (Examples)

```yaml
alerts:
  - name: "HighErrorRate"
    condition: "rate(http_requests_total{status=~\"5..\"}[5m]) > 0.05"
    severity: critical
  - name: "QueueLatencySLABreach"
    condition: "histogram_quantile(0.95, queue_latency_seconds) > 0.1"
    severity: warning
  - name: "DatabaseConnectionPoolExhausted"
    condition: "db_connection_pool_available < 5"
    severity: critical
```

---

## 3. Incident Response

### 3.1 Severity Classification

| Severity | Impact | Example | Response |
|----------|--------|---------|----------|
| **P1** | System down / data loss | Queue service unavailable | SEV-1, all-hands |
| **P2** | Partial outage / degradation | Verification latency > 500ms p95 | On-call team |
| **P3** | Minor feature broken | UI button not working | Assign to team |
| **P4** | Documentation issue | Typo | Backlog |

### 3.2 Timeline

```
T+0m  Alert fired -> on-call paged
T+5m  Triage and initial assessment
T+15m Mitigation decision
T+30m RCA start
T+60m (P1) resolution or escalation
T+4h  RCA draft
T+5d  Post-mortem review
```

---

## 4. Disaster Recovery

### 4.1 RPO/RTO Targets

| Component | RPO | RTO | Strategy |
|-----------|-----|-----|----------|
| Database | 1 hour | 4 hours | Daily backup + WAL archiving |
| Kubernetes cluster | 0 min | 10 min | Multi-AZ deployment |
| Secrets | Real-time | 5 min | Vault replication + failover |
| Git repository | Real-time | 5 min | Mirrored backup hosting |

### 4.2 Backup Strategy

```yaml
Daily: Full database backup (24h retention)
Hourly: WAL archiving
Real-time: Persistent volume snapshots (15m interval)
Verification: Weekly restore test
```

---

## 5. Capacity Planning

| Year | Clinics | Patients | Daily Queue Entries | Peak QPS |
|------|---------|----------|--------------------|---------|
| 2026 | 10 | 100K | 10K | 100 |
| 2027 | 25 | 250K | 25K | 250 |
| 2028 | 50 | 500K | 50K | 500 |

---

## 6. Maintenance Windows

**Window:** Sunday 03:00-05:00 UTC
**Activities:** Database maintenance, security patches, certificate rotation, log archival
**Communication:** Status page + email + in-app banner (24-48h notice)

---

**Document Status:** ✓ APPROVED
**Review:** Monthly (SLA monitoring)
**Next Review:** 23-03-2026
