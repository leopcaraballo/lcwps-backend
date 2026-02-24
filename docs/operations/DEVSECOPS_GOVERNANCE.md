# DEVSECOPS GOVERNANCE – LCWPS

## CI/CD Automation + Security Gates + Container Hardening

**Authority:** Chief DevSecOps Officer, CISO
**Classification:** INFRASTRUCTURE CRITICAL
**Date:** 2026-02-23
**Status:** BINDING STANDARD

### Resumen (ES)

- Pipeline con puertas obligatorias de calidad y seguridad.
- SAST, Snyk y Trivy sin vulnerabilidades criticas.
- Imagenes firmadas y hardening de contenedores.

---

## 1. CI/CD Pipeline (Mandatory Gates)

```
Git Commit -> Build/Unit Tests -> SAST -> Dependency Scan
-> Container Build/Scan -> Integration Tests -> Architecture Compliance
-> PR Approval (2 reviewers) -> Merge -> GitOps Deployment
```

### 1.1 Gate Requirements

- Unit + Domain coverage >= 95%
- SonarQube: zero blocker issues
- Snyk: zero critical vulnerabilities
- Trivy: zero critical image vulnerabilities
- ArchUnit: no architecture violations
- Signed commits required

---

## 2. Security Gates

### 2.1 SAST (SonarQube)

```yaml
sonar.projectKey=lcwps-backend
sonar.sources=src
sonar.qualitygate.wait=true
```

### 2.2 Dependency Scanning (Snyk)

```bash
snyk test --severity-threshold=critical
snyk monitor
```

### 2.3 Container Scanning (Trivy)

```bash
trivy image --severity CRITICAL,HIGH registry.example.com/lcwps/queue-service:latest
```

---

## 3. Container Hardening

**Requirements:**

- Non-root user
- Minimal base image
- Signed images
- Read-only filesystem where feasible

---

## 4. Incident Response & Rollback

**Automated rollback** is enabled via GitOps (ArgoCD) with revision history retention.

---

## 5. Compliance Validation

Pipeline execution reports are retained for 5 years and must include:

- Commit SHA
- All stage results
- Coverage metrics
- Vulnerability summary
- Deployment timestamp

---

**Document Status:** ✓ APPROVED
**Review:** Monthly (pipeline incidents)
**Next Review:** 23-03-2026
