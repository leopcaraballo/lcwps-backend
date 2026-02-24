# GOVERNANCE BASELINE – LCWPS

## Official Enterprise Governance Bundle (EGB)

**Documento Autoridad:** Directiva de Gobernanza Empresarial LCWPS-2026-02-23
**Clasificación:** Confidencial - Restringido
**Efectividad:** 23 de febrero de 2026
**Autoridades Responsables:** Chief Enterprise Architect, CISO, DPO, Compliance Officer
**Vigencia Normativa:** Indefinida (Revisión anual mínima)

---

## 1. AUTORIDAD Y ALCANCE

### 1.1 Autoridades Constituidas

Este documento establece autoridad vinculante ejercida por:

- **Chief Enterprise Architect**: Definición de baseline arquitectónico inmutable
- **Chief Information Security Officer (CISO)**: Zero Trust Architecture, encriptación, access control
- **Data Protection Officer (DPO)**: Cumplimiento Ley 1581/2012, Decreto 1377/13, Habeas Data
- **Chief DevSecOps Officer**: CI/CD governance, automatización de security gates, container hardening
- **Healthcare Compliance Officer**: Ley 23/1981, regulaciones Supersalud, continuidad de negocio
- **Enterprise Risk & Governance Authority**: Matriz de riesgos, control de collusion, traceability

### 1.2 Alcance Sistémico

**Sistema:** LCWPS – Luxury Clinic Waiting Panel System
**Tipo:** Sistema Crítico Multi-Tenant de Orquestación y Notificación Clínica con Verificación Manual de Pago
**Jurisdicción:** Colombia (Leyes Colombianas Primarias)
**Criticidad:** CRITICAL – Continuidad de Atención Médica
**Impacto Regulatorio:**

- Dirección Supersalud
- Autoridad Nacional de Protección de Datos (ANPD)
- Riesgo FCPA (si aplica por transacciones USD)
- ISO 27001 Scope (Información Sensible de Salud)

---

## 2. MODELO DE AUTORIDAD NO NEGOCIABLE

### 2.1 Principios Fundamentales de Gobernanza

1. **Doble Control Mandatorio**: Ninguna decisión arquitectónica, seguridad o regulatoria puede ser unilateral
2. **Inmutabilidad de Decisiones**: Las decisiones documentadas en ADR no pueden ser revertidas sin formal act
3. **Trazabilidad Quinquenal**: Toda decisión, commit, PR, aprobación debe ser rastreable por mínimo 5 años
4. **Separación de Deberes Estricta**: Autor ≠ Revisor ≠ Aprobador ≠ Mergeador
5. **Automatización de Controles**: Los controles deben ser codificados en CI/CD, no manuales
6. **Resistencia a Colusión**: Las reglas deben requerir colusión de múltiples actores para ser burladas
7. **Audit Trail Inmutable**: Los logs de auditoría no pueden ser modificados, solo añadidos
8. **Zero Trust por Defecto**: Ningún acceso se asume confiable sin verificación explícita

### 2.2 Invariantes Arquitectónicas

El sistema LCWPS **DEBE** cumplir de forma inmutable:

| Invariante | Descripción | Enforcement |
|-----------|------------|------------|
| **Microservicios Distributivos** | No monolitos, no shared databases | CI/CD + Architecture Deployment |
| **.NET 10 LTS Únicamente** | Lenguaje base no negociable | Project file validation |
| **Clean Architecture + DDD** | Boundary enforcement obligatorio | Dependency analyzer |
| **Event-Driven Async** | RabbitMQ para inter-service comms | Service contract tests |
| **PostgreSQL + RLS** | Base de datos única con RLS obligatorio | Policy deployment validation |
| **Kubernetes Native** | Orquestación obligatoria | Helm chart validation |
| **OpenTelemetry + Prometheus + Grafana** | Observability no opcional | Instrumentation tests |
| **Docker Runtime** | Containerización obligatoria | Signed image registry |
| **Encriptación TLS+AES256** | In-transit y at-rest | Secret rotation automation |
| **RLS Política Tenant** | Row-Level Security para multi-tenancy | RLS deployment verification |

---

## 3. MARCO REGULATORIO COLOMBIANO (TIER 1 – MÁXIMA EXPOSICIÓN)

### 3.1 Ley 1581 de 2012 – Protección de Datos Personales

**Aplicabilidad:** CRÍTICA
**Enforcement Point:** DPO, Legal, CISO

**Control Obligatorios:**

- Consentimiento explícito para tratamiento de datos
- Finalidad específica documentada
- Seguridad técnica y administrativa
- Derecho al acceso, rectificación, cancelación (ARCO)
- Respuesta en máximo 10 días hábiles para peticiones ARCO
- Evaluación de impacto de privacidad (PIA)
- Registro de actividades tratamiento (LRD)

### 3.2 Decreto 1377 de 2013 – Regulación Ley 1581

**Aplicabilidad:** CRÍTICA
**Enforcement Point:** DPO, CISO

**Control Obligatorios:**

- Política de privacidad clara y accesible
- Mecanismo ARCO implementado en aplicación
- Transferencia internacional bloqueada por defecto
- Incidentes de seguridad reportados en calendario regulatorio
- Autorización de terceros para procesamiento
- Almacenamiento restringido a fines legales

### 3.3 Ley 23 de 1981 – Código Sanitario Colombiano

**Aplicabilidad:** CRÍTICA (Healthcare Domain)
**Enforcement Point:** Healthcare Compliance, Chief Architect

**Control Obligatorios:**

- Secreto médico profesional preservado
- Acceso a datos clínicos solo personal autorizado
- Auditoría de acceso a historias clínicas (HIPAA-equivalent)
- Retención de registros clínicos: mínimo 20 años
- Destrucción segura de media OBSOLETA
- Continuidad operacional en caso de desastre

### 3.4 Regulaciones Supersalud

**Aplicabilidad:** CRÍTICA (Healthcare Provider Validation)
**Enforcement Point:** Healthcare Compliance Officer

**Control Obligatorios:**

- Validación de credenciales de proveedor médico
- Cobertura de servicios verificable
- Histórico de autorizaciones mantengible
- Trazabilidad de decisiones clínicas
- Reportes estadísticos Supersalud (REPS)
- Continuidad de servicio SLA: 99.8% mínimo

---

## 4. MARCO REGULATORIO INTERNACIONAL (TIER 2 + 3)

### 4.1 ISO/IEC 27001 – Information Security Management

**Aplicabilidad:** MANDATORY
**Evidence Artifact:** Estatuto de Información Sensible

- Control table en SECURITY_GOVERNANCE.md
- RLS policies (PostgreSQL)
- TLS certificate validation
- Access control matrix
- Encryption key management
- Incident response procedures

### 4.2 ISO/IEC 25010 – Software Quality

**Aplicabilidad:** MANDATORY
**Evidence Artifact:** Test Coverage Reports

- General coverage ≥ 95%
- Domain coverage ≥ 95%
- Code quality gates (SonarQube)
- Performance baselines
- Reliability metrics

### 4.3 NIST Cybersecurity Framework

**Aplicabilidad:** ALIGNMENT (Reference Architecture)

- **Identify**: Asset inventory, data classification
- **Protect**: Zero Trust, encryption, access control
- **Detect**: OpenTelemetry, Prometheus alerting
- **Respond**: Incident response procedures
- **Recover**: Database backup with RPO/RTO

### 4.4 OWASP Top 10 Mapping

**Aplicabilidad:** MANDATORY
**Evidence Artifact:** SECURITY_GOVERNANCE.md + Code scanning rules

- A01 Broken Access Control → RBAC + RLS
- A02 Cryptographic Failures → TLS + AES256 at-rest
- A03 Injection → ORM only, no raw SQL
- A04 Insecure Design → DDD + Architecture Review
- A05 Security Misconfiguration → Signed Docker images
- A06 Vulnerable Components → Dependency scanning
- A07 Authentication Failures → OAuth2 + MFA
- A08 Software Integrity Failures → Signed commits + tags
- A09 Logging & Monitoring → OpenTelemetry + immutable logs
- A10 SSRF → Network isolation + pod policies

---

## 5. MODELO DE DOS PERSONAS (TWO-PERSON CONTROL)

### 5.1 Principio Fundamental

**Ninguna decisión crítica puede ser ejecutada unilateralmente.**

Definición: Decisión Crítica

- Merge a main, qa, develop
- Cambios en políticas de seguridad
- Cambios en table schemas (migrations)
- Cambios en RLS policies
- Cambios en CI/CD pipelines
- Secretos (API keys, certificates)
- Cambios de infraestructura

### 5.2 Modelo de Aprobación

| Rol | Responsabilidad | Restricción |
|-----|-----------------|------------|
| **Author** | Crea PR, implementa cambio | ✗ No puede auto-approve |
| **Reviewer 1** | Revisión técnica, aprobación | Debe ser distinto a author |
| **Reviewer 2** | Revisión de seguridad/compliance | ✗ No puede ser author, debe aprobar en secuencia |
| **Mergeer** | Ejecuta merge en rama protegida | ✗ **DEBE SER** distinto a author |

**Flujo Obligatorio:**

```
Author creates PR
  → Reviewer-1 approves (Code quality + architecture)
  → Reviewer-2 approves (Security + compliance)
  → Mergeer (non-author) merges
  → Tag (for main/qa only)
```

### 5.3 Excepciones Formales

Las únicas excepciones a two-person control requieren:

1. Formal Act (Acta Formal) firmada por mínimo 2 autoridades
2. Justificación documentada con impacto regulatory
3. Almacenamiento en repositorio de actas formales (Compliance)
4. Reporte a Junta Directiva en sesión siguiente
5. **No override automático permitido** – requiere manual pull de rama protegida

---

## 6. MODELO DE APROBACIÓN FORMAL

### 6.1 Niveles de Aprobación por Tipo de Cambio

| Cambio | Aprobación Técnica | Aprobación Seguridad | Aprobación Legal | Aprobación Business |
|--------|-------------------|-------------------|-----------------|-------------------|
| Feature | ✓ (1 reviewer) | ✓ (SAST + check) | — | — |
| Security patch | ✓ (2 reviewers) | ✓ (CISO sign-off) | ✓ (si PII) | — |
| Regulatory change | ✓ (2 reviewers) | ✓ (CISO sign-off) | ✓ (DPO sign-off) | ✓ (CEO/Board) |
| Migration (DB) | ✓ (DB architect) | ✓ (backup confirmation) | — | — |
| RLS policy change | ✓ (2 reviewers) | ✓ (CISO sign-off) | ✓ (DPO sign-off) | — |
| Infrastructure | ✓ (DevOps) | ✓ (CISO) | — | ✓ (CTO) |

---

## 7. ENFORCEMENT MECHANISMS (NO EXCEPTIONS)

### 7.1 GitHub/GitLab Branch Protection (Mandatory)

For branches: **main**, **qa**, **develop**

```
✓ Require pull request reviews before merging
✓ Require 2 review approvals (minimum)
✓ Require signed commits
✓ Require status checks to pass
✓ Require conversation resolution
✓ Dismiss stale pull request approvals when new commits are pushed
✓ Include administrators in restrictions
✓ Restrict who can push to matching branches
✓ Prevent force pushes
✓ Prevent branch deletion
```

### 7.2 CI/CD Automated Gates (No Override)

Pipeline must pass **before** PR merge is available:

- ✓ Unit tests ≥ 95% coverage
- ✓ Domain tests ≥ 95% coverage
- ✓ SAST scan (SonarQube) with security gate
- ✓ Dependency vulnerability scan (Snyk/WhiteSource) with zero critical
- ✓ Container scan (Trivy) with zero critical
- ✓ Integration tests pass
- ✓ Architecture analyzer (ArchUnit) passes
- ✓ Commit must be signed
- ✓ PR author cannot merge (automated by system)

### 7.3 Production Deployment Controls

Only signed tags on main branch trigger production deployment:

```bash
git tag -s vX.Y.Z -m "Release vX.Y.Z - <description>"
git push origin vX.Y.Z
```

- ✓ Tag must be GPG-signed
- ✓ Tag must reference approved PR
- ✓ Tag must follow Semantic Versioning
- ✓ Deployment approval board: minimum 2 signatories on release notes

---

## 8. MODELO DE RETENCIÓN DE EVIDENCIA

### 8.1 Trazabilidad Quinquenal Obligatoria

Toda evidencia relativa a LCWPS deve ser mantenida **mínimo 5 años**:

| Artefacto | Retención | Almacenamiento | Immutabilidad |
|-----------|-----------|-----------------|---------------|
| Git commits + tags | 5 años | Git repository + backup | ✓ Verificado por hash |
| Pull requests | 5 años | GitHub/GitLab archive | ✓ API + archive export |
| CI/CD pipeline logs | 5 años | CloudWatch/ELK | ✓ Append-only |
| Audit logs (application) | 5 años | PostgreSQL audit table | ✓ No-delete triggers |
| Security scan results | 5 años | Artifact repository | ✓ Signed + hashed |
| Formal acts (decisiones) | 5 años | Repositorio de actas formales (Compliance) | ✓ PDF + signature |
| Release notes | 5 años | GitHub releases | ✓ Immutable |
| Access logs | 5 años | CloudWatch + Grafana | ✓ Append-only |
| Compliance evidence | 5 años | REGULATORY_TRACEABILITY_MATRIX.md | ✓ Timestamped |

### 8.2 Auditoría de Retención

Anualmente se debe:

1. Validar que toda evidencia requerida esté presente
2. Generar reporte de completitud (hash-based verification)
3. Firmar reporte por mínimo 2 autoridades
4. Almacenar en repositorio de reportes de auditoria anual (Compliance)

---

## 9. MATRIZ DE RESPONSABILIDAD (RACI)

| Decisión | Responsible | Accountable | Consulted | Informed |
|----------|------------|------------|----------|----------|
| Architecture drift | Architect | Architect + CISO | Tech lead | Team |
| Security vulnerability | CISO | CISO + CEO | Architect | All |
| Data breach | DPO | DPO + CEO + Legal | CISO | Regulators |
| Performance SLA breach | DevOps | DevOps + CTO | Architect | Management |
| Compliance violation | Compliance | Compliance + CEO + Legal | CISO | Board |
| RLS policy change | Architect | Architect + DPO | CISO | Team |
| Feature approval | Product | CTO + Product | Architect | Stakeholders |
| Production hotfix | DevOps | CTO + DevOps | CISO | Management |

---

## 10. VIOLACIONES Y SANCIONES

### 10.1 Violaciones Críticas (Immediate Escalation)

1. **Merge sin PR** → Immediate revert + Formal Act required
2. **Force push en rama protegida** → Incident response + Root cause analysis
3. **Commit no firmado en main** → Revert + Training required
4. **Acceso directo a base de datos en producción** → Incident investigation + Access revocation
5. **Cambio de RLS policy sin DPO approval** → Regulatory review + Board escalation
6. **Desactivar branch protection** → System lockdown + CEO approval required

### 10.2 Consecuencias

| Violación | Consecuencia |
|-----------|--------------|
| Reiterada (2x) | Warning formal + Retraining |
| Reiterada (3x) | Suspension de acceso a main branch por 30 días |
| Reiterada (4x) | Escalation a Junta Directiva |
| Maliciosa (intencional) | Terminación de relación laboral + Reporte legal |

---

## 11. VIGENCIA Y ENMIENDAS

Este documento entra en vigencia inmediatamente como **Baseline de Gobernanza Empresarial (EGB)**.

**Enmiendas:**

- Cambios menores: CTO + CISO (co-sign)
- Cambios moderados: CTO + CISO + DPO + Compliance (3/4 approval)
- Cambios mayores: Board approval required

**Revisión Obligatoria:** Anualmente (Q1)
**Próxima Revisión:** 23 de febrero de 2027

---

**Documento Clasificado: CONFIDENCIAL – Restringido a Personal Autorizado**
**Vigencia:** 23 de febrero de 2026 – [indefinida]
