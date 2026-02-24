# GOVERNANCE INDEX – LCWPS

## Enterprise Documentation Control Point

**Authority:** Board of Directors, Chief Enterprise Architect
**Classification:** CONFIDENTIAL - Governance Restricted
**Date:** 2026-02-23
**Status:** ✓ COMPLETE & APPROVED

---

## System Definition (Current)

LCWPS is a **real-time orchestration and notification system** for clinical appointment management with **manual payment verification** as a mandatory gate before queue entry.

---

## Documentation Structure

### Tier 1: Governance Baseline

1. [ENTERPRISE_GOVERNANCE_BASELINE.md](ENTERPRISE_GOVERNANCE_BASELINE.md)
   Authority model, two-person control, compliance framework.

2. [GOVERNANCE_INDEX.md](GOVERNANCE_INDEX.md)
   Canonical index for all governance domains.

### Tier 2: Business Domain

1. [BUSINESS_RULES.md](BUSINESS_RULES.md)
   Binding business rules, invariants, event catalog.

2. [BUSINESS_LOGIC_ANALYSIS.md](BUSINESS_LOGIC_ANALYSIS.md)
   Updated domain analysis (manual verification model).

3. [FUNCTIONAL_REQUIREMENTS.md](FUNCTIONAL_REQUIREMENTS.md)
   Functional + non-functional requirements.

4. [AI_GOVERNANCE.md](AI_GOVERNANCE.md)
   AI usage policy and prompt archival requirements.

### Tier 3: Architecture

1. [../architecture/ARCHITECTURE.md](../architecture/ARCHITECTURE.md)
   Architecture baseline (microservices + event-driven).

2. [../architecture/adr/ADR-0001-MICROSERVICES_EVENT_DRIVEN.md](../architecture/adr/ADR-0001-MICROSERVICES_EVENT_DRIVEN.md)
   Foundational ADR (historical baseline).

3. [../architecture/adr/ADR-TEMPLATE.md](../architecture/adr/ADR-TEMPLATE.md)
   ADR template for new decisions.

### Tier 4: Security and Regulatory

1. [../security/SECURITY_GOVERNANCE.md](../security/SECURITY_GOVERNANCE.md)
    Zero Trust, RLS, encryption, RBAC, OWASP mapping.

2. [../regulatory/REGULATORY_TRACEABILITY_MATRIX.md](../regulatory/REGULATORY_TRACEABILITY_MATRIX.md)
    Regulation-to-control traceability.

### Tier 5: Operations and Quality

1. [../operations/DEVSECOPS_GOVERNANCE.md](../operations/DEVSECOPS_GOVERNANCE.md)
    CI/CD governance, security gates, container hardening.

2. [../operations/QA_GOVERNANCE.md](../operations/QA_GOVERNANCE.md)
    Test strategy, coverage, execution requirements.

3. [../operations/OPERATIONS_GOVERNANCE.md](../operations/OPERATIONS_GOVERNANCE.md)
    SLA, monitoring, incident response, disaster recovery.

4. [../operations/RISK_MANAGEMENT_FRAMEWORK.md](../operations/RISK_MANAGEMENT_FRAMEWORK.md)
    Enterprise risk model and escalation matrix.

### Tier 6: Source Control Governance

1. [../git/GIT_GOVERNANCE.md](../git/GIT_GOVERNANCE.md)
    Branch model, commit rules, approvals, release policy.

---

## Review Cadence

| Document | Frequency | Owner |
|----------|-----------|-------|
| ENTERPRISE_GOVERNANCE_BASELINE | Annual | Board + CISO + DPO |
| ARCHITECTURE | Quarterly | Chief Enterprise Architect + CTO |
| BUSINESS_RULES | Quarterly | Product Owner + Domain Expert |
| BUSINESS_LOGIC_ANALYSIS | Quarterly | Product Owner + Chief Architect |
| FUNCTIONAL_REQUIREMENTS | Quarterly | Product Owner + Business Analyst |
| SECURITY_GOVERNANCE | Quarterly | CISO |
| REGULATORY_TRACEABILITY_MATRIX | Annual | DPO + Compliance |
| DEVSECOPS_GOVERNANCE | Monthly | Chief DevSecOps Officer |
| QA_GOVERNANCE | Quarterly | QA Lead |
| OPERATIONS_GOVERNANCE | Monthly | COO + DevOps Lead |
| RISK_MANAGEMENT_FRAMEWORK | Monthly | Chief Risk Officer |
| AI_GOVERNANCE | Monthly | CTO |
| GIT_GOVERNANCE | Quarterly | CTO + DevOps Lead |

---

**Document Status:** ✓ APPROVED – Canonical Index
**Next Review:** 2027-02-23 (Q1)
