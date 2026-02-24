# RISK MANAGEMENT FRAMEWORK – LCWPS

## Enterprise Risk Assessment + Mitigation Strategy

**Authority:** Chief Risk Officer, CISO, DPO
**Classification:** CONFIDENTIAL - Risk Restricted
**Date:** 2026-02-23
**Status:** BINDING

### Resumen (ES)

- Matriz de riesgo por probabilidad e impacto.
- Top riesgos priorizados con mitigacion trimestral.
- Escalamiento definido por severidad.

---

## 1. Risk Assessment Matrix

```
Risk Score = Likelihood × Impact

Likelihood Scale:
  1 = Unlikely (< 5% annual probability)
  2 = Possible (5-25% annual probability)
  3 = Likely (25-50% annual probability)
  4 = Highly Likely (> 50% annual probability)

Impact Scale:
  1 = Minor (< $10K loss, zero patient impact)
  2 = Moderate ($10K-$100K loss, limited patient impact)
  3 = Major ($100K-$1M loss, significant patient harm, regulatory review)
  4 = Critical (>$1M loss, patient deaths, license revocation risk)

Risk Rating:
  1-4 = Green (acceptable, monitor)
  5-8 = Yellow (mitigate actively)
  9-12 = Orange (high priority mitigation)
  13-16 = Red (escalate immediately, board review)
```

---

## 2. Top Enterprise Risks (Summary)

1. Multi-tenant data breach (RLS bypass)
2. Manual payment verification bypass
3. Kubernetes cluster failure
4. Supply chain attack (NuGet vulnerabilities)
5. Regulatory non-compliance (Ley 1581)
6. Patient data retention violation
7. Insider threat (rogue staff)
8. Financial processing fraud
9. DDoS attack
10. Encryption key compromise

---

## 3. Risk Mitigation Roadmap

### Q1 2026 (Immediate)

- Implement and test RLS policies
- Enable CI/CD security gates (SAST, Snyk, Trivy)
- Deploy OpenTelemetry monitoring

### Q2 2026 (High Priority)

- External penetration testing
- ISO 27001 Stage 1 pre-audit
- Disaster recovery drill
- Compliance audit (Ley 1581)

### Q3 2026 (Medium Priority)

- ISO 27001 certification audit
- Supply chain risk assessment
- Insider threat training
- DDoS protection validation

### Q4 2026 (Strategic)

- Board risk review
- Enterprise risk strategy update
- Cybersecurity insurance review
- Regulatory landscape scanning

---

## 4. Risk Escalation Matrix

| Risk Level | Notification | Action | Timeline |
|-----------|---|---|---|
| **Green** | Monitor | Standard procedures | Quarterly review |
| **Yellow** | Team lead | Mitigation plan | Monthly check-in |
| **Orange** | Management | Executive briefing | Weekly review |
| **Red** | C-Suite + Board | Emergency response | Immediate |

---

## 5. Risk Register (Living Document)

The enterprise risk register is maintained in the GRC repository under compliance control.

**Owner:** Chief Risk Officer
**Review Frequency:** Monthly
**Board Review:** Quarterly

---

**Document Status:** ✓ APPROVED
**Review:** Monthly (risk assessment)
**Next Review:** 23-03-2026
