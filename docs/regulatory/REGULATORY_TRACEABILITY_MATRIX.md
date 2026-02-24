# REGULATORY TRACEABILITY MATRIX – LCWPS

## Complete Regulation → Control → Implementation → Evidence Mapping

**Authority:** DPO, CISO, Compliance Officer
**Classification:** CONFIDENTIAL - Regulatory Restricted
**Date:** 2026-02-23
**Jurisdiction Focus:** Colombia (Ley 1581/12, Decreto 1377/13, Ley 23/81, Supersalud)

---

## PART I: LEY 1581 DE 2012 (PROTECCIÓN DE DATOS PERSONALES)

### REGULATION: LEY 1581, ARTICLE 4 (PRINCIPIOS)

| Regulation | Control Name | Implementation | Evidence Artifact | Validation Test |
|-----------|:------:|:----:|:------:|:---:|
| **Art. 4.1 - Legalidad** (Lawfulness) | Legal Basis Documentation | Privacy Policy published on clinic portal + consent form in patient onboarding | Legal privacy policy (Legal repository) + form screenshots | Annual review by Legal (signed board minutes) |
| **Art. 4.2 - Finalidad** (Purpose Limitation) | Data Use Matrix | SQL views restrict column access; documented data flows | Data use matrix (Compliance repository) | Privacy audit + column-level access test |
| **Art. 4.3 - Libertad** (Consent) | Explicit Consent Capture | Consent recorded in `patients.consent_log` table with timestamp | PostgreSQL table audit | Consent audit report (monthly) |
| **Art. 4.4 - Veracidad** (Accuracy) | Data Quality Controls | Patient validation rules in domain layer; duplicate detection | `.../Domain/Patients/PatientValidation.cs` | Test coverage ≥95% for validation rules |
| **Art. 4.5 - Integridad** (Data Integrity) | ACID Transactions | PostgreSQL ACID compliance; EF Core transaction handling | EF Core DbContext.SaveChangesAsync() + logs | Integration tests with transaction rollback |
| **Art. 4.6 - Confidencialidad** (Confidentiality) | Encryption at Rest | AES-256-GCM on sensitive fields via pgcrypto | Migration scripts w/ encryption triggers | Penetration test + key audit |
| **Art. 4.7 - Seguridad** (Security) | Zero Trust Architecture | RLS policies + TLS 1.3 + RBAC | `/docs/security/SECURITY_GOVERNANCE.md` | Annual ISO 27001 audit |
| **Art. 4.8 - Actualización** (Maintenance) | Data Refresh Policy | Annual patient contact confirmation; stale data detection | Batch job scheduled in K8s CronJob | Job execution logs (CloudWatch) |

### REGULATION: LEY 1581, ARTICLE 6 (DERECHOS)

| Regulation | Control Name | Implementation | Evidence | Test |
|-----------|:------:|:----:|:------:|:---:|
| **Art. 6.1 - Acceso** (Right to Access) | ARCO API Endpoint | `[HttpGet("/arco/access")] public async Task<DataSubjectRecord>` | Controller + integration test | E2E test simulating patient request |
| **Art. 6.2 - Rectificación** (Right to Rectify) | Update Validation | Patient can edit non-clinical fields via UI; clinical updates require MD | UI + business logic validations | Functional test: Patient ↔ Doctor update flows |
| **Art. 6.3 - Cancelación** (Right to Erasure) | Soft Delete with Audit | Records marked as `deleted_at` timestamp; not physical deletion | PostgreSQL soft delete trigger | Retention test: verify 5-year immutability |
| **Art. 6.4 - Oposición** (Right to Object) | Opt-Out Mechanism | Table: `patient_communications_preferences`; flag: `do_not_contact` | Database schema + update API | Test: verify SMS not sent after opt-out |

### REGULATION: LEY 1581, ARTICLE 13 (RESPONSABLE)

| Regulation | Control Name | Implementation | Evidence | Test |
|-----------|:------:|:----:|:------:|:---:|
| **Art. 13 - Responsibility** | RACI Matrix | Defined roles: DPO → breaches, CISO → security | `/docs/governance/ENTERPRISE_GOVERNANCE_BASELINE.md` Section 9 (RACI) | Quarterly governance audit |
| **Breach Notification** | Incident Response Plan | Notify DPA within 30 days; affected subjects within 48h | SECURITY_GOVERNANCE.md (Incident Response Plan) | Tabletop exercise (annual) |
| **Data Protection Officer** | DPO Appointment | Named DPO with authority; contact: <dpo@lcwps.com> | Board resolution + contract | HR system verification |

---

## PART II: DECRETO 1377 DE 2013 (REGULACIÓN LEY 1581)

### REGULATION: DECRETO 1377, TITLE I (PRINCIPIOS)

| Regulation | Control Name | Implementation | Evidence | Test |
|-----------|:------:|:----:|:------:|:---:|
| **Art. 1 - Consentimiento** (Consent) | Informed Consent Form | Two-part consent: (1) clinic service, (2) data processing | Angular form component w/ acceptance checkbox | Manual UAT + accessibility test |
| **Art. 2 - Proporcionalidad** (Proportionality) | Data Minimization | Only collect: name, DOB, insurance, contact; NOT SSN unless required by law | Database schema constraints + RESTRICT DML trigger | Column enumeration audit |
| **Art. 3 - Transparencia** (Transparency) | Privacy Policy (Public) | Published in Spanish; accessible without login | `/var/www/public/privacy-policy.html` | Monitor with web crawler (weekly) |

### REGULATION: DECRETO 1377, TITLE III (DERECHOS)

| Regulation | Control Name | Implementation | Evidence | Test |
|-----------|:------:|:----:|:------:|:---:|
| **Art. 11 - ARCO Response** | ARCO Request Handling | API endpoint: POST `/api/arco/request`; Response SLA: 10 business days | BackgroundService task scheduled in Kubernetes | Test: submit ARCO → verify compliance report |
| **Art. 12 - Petición Formal** | Formal Petition Handler | Email handler: <dpo@lcwps.com>→ticket system→DPO review | Ticket system w/ SLA monitoring | Test: DPO acknowledges within 2 days |
| **Art. 13 - Rectificación** | Correction Workflow | Patient submits correction → DPO reviews → Clinical update (if applicable) | Workflow state machine in Application layer | Test: verify audit trail of changes |

---

## PART III: LEY 23 DE 1981 (CÓDIGO SANITARIO COLOMBIANO)

### REGULATION: LEY 23, ARTICLE 15 (SECRETO MÉDICO)

| Regulation | Control Name | Implementation | Evidence | Test |
|-----------|:------:|:----:|:------:|:---:|
| **Art. 15 - Confidencialidad** (Medical Secrecy) | RLS Policy: Clinical Notes | PostgreSQL RLS: Only treating physician + clinic admin can view | RLS policy in migration: `/Infrastructure/Persistence/Migrations/AddClinicalNotesRLS.cs` | Integration test: non-authorized user denied |
| **Access Audit** | Clinical Notes Access Log | Every access to clinical_notes logged in audit table with user_id, timestamp, purpose | Auditing trigger on clinical_notes table | Verify audit completeness (100% coverage) |
| **Breach Disclosure** | Patient Notification | If breach occurs: notify patient within 48h; notify Supersalud within 5 days | SECURITY_GOVERNANCE.md (Incident Response) | Tabletop: test notification flow |

### REGULATION: LEY 23, ARTICLE 34 (REGISTRO CLÍNICO)

| Regulation | Control Name | Implementation | Evidence | Test |
|-----------|:------:|:----:|:------:|:---:|
| **20-Year Retention** | Long-Term Archive | Clinical records stored in archive database; retention: 20+ years | Archive migration job scheduled daily; retention policy documented | Verify oldest record retention = current_date - 20 years |
| **Physical Security** | Database Encryption | PostgreSQL at-rest encryption + backup encryption (S3 SSE-S3) | AWS S3 bucket policy + PostgreSQL pgcrypto extension | Penetration test: attempt disk/backup access |
| **Destruction Policy** | Secure Deletion | After retention period: cryptographic erasure (key destruction) | Deletion scheduled job + audit log | Verify deletion: SELECT count(*) after retention ends = 0 |

---

## PART IV: SUPERINTENDENCIA DE SALUD (SUPERSALUD) REQUIREMENTS

### REGULATION: SUPERSALUD – PRESTADORES (PROVIDER VALIDATION)

| Regulation | Control Name | Implementation | Evidence | Test |
|-----------|:------:|:----:|:------:|:---:|
| **Provider Credentials** | License Verification | Clinic providers validate against REPS database (Colombian registry) | Daily REPS sync via scheduled job | Test: invalid provider rejected at queue entry |
| **Service Coverage** | Insurance Coverage Validation | Manual verification confirms coverage before queue entry | Payment verification workflow evidence + audit log | Test: uncovered service → queue blocked |
| **SLA: Availability** | System Uptime SLA | Target: 99.8% availability (8.6 hours downtime/month) | Prometheus uptime monitoring; Grafana dashboard | Monthly SLA report signed by DevOps |
| **SLA: Response Time** | Queue Processing SLA | 95th percentile queue processing ≤ 5 minutes | Prometheus histogram: `queue_processing_duration_seconds` | Weekly performance reports |

### REGULATION: SUPERSALUD – REPORTES (REPORTING OBLIGATIONS)

| Regulation | Control Name | Implementation | Evidence | Test |
|-----------|:------:|:----:|:------:|:---:|
| **Monthly REPS Report** | Automated Reporting | Extract clinic data → format XML → submit to Supersalud API | Analytics service scheduled job (1st of month) | Test: valid XML schema + successful submission |
| **Incident Reporting** | 24H Alert System | Incident detected → alert Compliance officer → report Supersalud within 24h | CloudWatch alarm → SNS → email | Test: simulate incident → verify email sent |
| **Service Denial Tracking** | Denial Log Integration | Track all queue denials (reason, timestamp, patient); monthly report to Supersalud | Analytical view: `analytics.queue_denial_summary` | Verify completeness: 100% of denials logged |

---

## PART V: INTERNATIONAL STANDARDS (ISO 27001 / NIST / OWASP)

### REGULATION: ISO/IEC 27001 (INFORMATION SECURITY)

| Regulation | Control Name | Implementation | Evidence | Test |
|-----------|:------:|:----:|:------:|:---:|
| **A.5.1 - Access Control** | RBAC Model | 5 roles defined in system; authorization checked on each API call | Authorization attribute in Controllers + middleware | Test: RBAC.cs unit tests for role combinations |
| **A.9.1 - User Authentication** | OAuth2.0 + MFA | IdentityServer4; TOTP (RFC 6238) or SMS OTP | Auth service + Identity layer | Test: MFA optional for doctors, mandatory for admins |
| **A.10.1 - Cryptography** | TLS 1.3 + AES-256 | All API traffic encrypted; at-rest encryption on sensitive columns | Nginx config + EF Core encryption converters | SSL Labs A+ rating + encryption audit |
| **A.12.4 - Logging** | Immutable Audit Logs | All changes logged to append-only table; retention 5+ years | PostgreSQL audit table + S3 backup | Verify: zero UPDATE/DELETE on audit table |

### REGULATION: NIST CYBERSECURITY FRAMEWORK

| Regulation | Control Name | Implementation | Evidence | Test |
|-----------|:------:|:----:|:------:|:---:|
| **Identify** | Asset Inventory | CMDB: all services, databases, APIs documented | CMDB (Operations repository) | Quarterly audit: verify completeness |
| **Protect** | Network Isolation | Kubernetes NetworkPolicy; service mesh mTLS | Service mesh Istio + K8s policies | Network traffic capture test (only authorized flows) |
| **Detect** | Anomaly Detection | Prometheus alerting rules for unusual patterns (spike in errors, latency) | Alert rules in Prometheus config | Test: simulate anomaly → verify alert triggered |
| **Respond** | Incident Response | Runbooks for each alert; escalation matrix defined | OPERATIONS_GOVERNANCE.md (Incident Response) | Tabletop exercise (quarterly) |
| **Recover** | Disaster Recovery** | RTO: 4 hours, RPO: 1 hour for primary DC | Backup restoration test (quarterly) | Verify: database restored within RTO |

### REGULATION: OWASP TOP 10 MAPPING

| OWASP Risk | Control | Implementation | Evidence | Test |
|-----------|:------:|:----:|:------:|:---:|
| **A01 - Broken Access Control** | RBAC + RLS | Two-layer: app-level roles + database RLS | Authorization tests + integration tests | Test: query as unauthorized tenant → denied |
| **A02 - Cryptographic Failures** | TLS 1.3 + AES-256-GCM | All sensitive fields encrypted; key rotation 90 days | Encryption configuration + KMS audit | SSL test + crypto audit |
| **A03 - Injection** | Parameterized Queries | EF Core exclusively; no string concatenation SQL | Code review + static analysis (SonarQube) | SAST scan: zero SQL injection findings |
| **A04 - Insecure Design** | Domain-Driven Design | Business rules enforced in domain layer; no logic in controller | LayerDependencies ArchUnit test | Test: invariant violations throw exceptions |
| **A05 - Security Misconfiguration** | Infrastructure as Code | Kubernetes YAML + Helm charts; all secrets in external vault (Vault or AWS Secrets Manager) | GitOps repo + Sealed Secrets | Configuration review: zero hardcoded secrets |
| **A06 - Vulnerable Components** | Dependency Scanning | Snyk scan on every PR; zero critical vulnerabilities | Branch protection gate: SAST + Snyk pass | Weekly vulnerability report |
| **A07 - Authentication Failures** | OAuth2.0 + MFA | IdentityServer4; session timeout 15 minutes | Auth service tests | Test: brute force attack → account locked after 5 attempts |
| **A08 - Software & Data Integrity Failures** | Signed Commits + Tags | All commits signed with GPG; all main tags signed | Branch protection: `require signed commits` | Verify sig: git log --show-signature |
| **A09 - Logging & Monitoring** | OpenTelemetry + Immutable Logs | Structured JSON logging; sent to ELK stack; retention 5 years | Log aggregation configuration | Verify: attempt log deletion → blocked by trigger |
| **A10 - SSRF** | Network Isolation | Pod-to-pod communication via service mesh; no egress to untrusted hosts | NetworkPolicy denying egress except allowed | Test: curl from pod to external IP → denied |

---

## PART VI: COLOMBIAN FINANCIAL REGULATIONS

### REGULATION: Monetary Transactions (Applicable if USD Processing)

| Regulation | Control | Implementation | Evidence | Test |
|-----------|:------:|:----:|:------:|:---:|
| **Transaction Audit Trail** | Payment Verification Audit Log | All payment verification actions logged: user, amount, timestamp, status | Payment verification audit table | Query: SELECT count(*) FROM payment_verification_audit = expected actions |
| **Fraud Detection** | Anomaly Rules | Transaction >$5000 USD requires manual approval | Business rule in PaymentVerificationService | Test: $6000 transaction → requires approval |
| **Segregation of Duties** | Two-Person Control | Approval ≠ Execution; both logged separately | PR approval model + merge separation | Test: attempt self-approval → blocked |

---

## PART VII: EVIDENCE ARTIFACT INVENTORY

### Mandatory Artifacts (5-Year Retention)

| Artifact Type | Location | Format | Retention | Validation |
|--------------|----------|--------|-----------|-----------|
| **Git Commit Log** | GitHub/GitLab | Native | 5 years | Hash verification (git fsck) |
| **PR Reviews** | GitHub API archive | JSON export | 5 years | PR export + approval screenshots |
| **CI/CD Pipeline Logs** | CloudWatch/ELK | Structured JSON | 5 years | Pipeline step audit |
| **Test Coverage Reports** | SonarQube | HTML + JSON | 5 years | Coverage trend analysis |
| **Security Scan Results** | Snyk/Trivy archive | CSV exports | 5 years | Zero-critical verification |
| **Audit Logs (Application)** | PostgreSQL + S3 backup | SQL dumps | 5 years | 4-week integrity verification |
| **Access Logs** | CloudWatch | CloudWatch Logs | 5 years | Monthly log review |
| **RLS Policy Deployments** | Migration scripts + audit | SQL + Git history | 5 years | Policy diff review |
| **Encryption Key Rotation** | AWS KMS API logs | JSON logs | 5 years | Rotation date verification |
| **Formal Acts** | Formal acts repository (Compliance) | PDF + signature | 5 years | Digital signature verification |
| **Board Minutes** | Company secretary | Minutes + signatures | 5 years | Secretary verification |

---

## PART VIII: COMPLIANCE VERIFICATION & AUDIT

### Annual Compliance Audit Checklist

| Control | Responsible | Q1 | Q2 | Q3 | Q4 |
|---------|------------|----|----|----|----|
| Privacy Policy reviewed | DPO | ✓ | | | |
| ARCO requests SLA monitored | DPO | ✓ | ✓ | ✓ | ✓ |
| Clinical notes RLS tested | CISO | ✓ | | | ✓ |
| Encryption key rotation | CISO | | ✓ | | ✓ |
| Data retention verified | Compliance | | | ✓ | |
| SLA metrics published | DevOps | ✓ | ✓ | ✓ | ✓ |
| Security scan results reviewed | CISO | ✓ | ✓ | ✓ | ✓ |
| Formal acts logged | Governance | ✓ | | ✓ | |

### External Audit Schedule

- **ISO 27001 Stage 1:** Q3 2026 (scoping)
- **ISO 27001 Stage 2:** Q1 2027 (full audit)
- **Supersalud Inspection:** Random (on-demand)
- **Data Protection Authority Audit:** Random (on-demand)

---

## SIGNATURE BOX

**Certification Board:**

| Role | Name | Signature | Date |
|------|------|-----------|------|
| DPO | [Name] | _____________ | 23-02-2026 |
| CISO | [Name] | _____________ | 23-02-2026 |
| Compliance Officer | [Name] | _____________ | 23-02-2026 |
| CTO | [Name] | _____________ | 23-02-2026 |

---

**Document Status:** ✓ APPROVED FOR IMPLEMENTATION
**Review Interval:** Annual (Q1)
**Next Review Date:** 23-02-2027
