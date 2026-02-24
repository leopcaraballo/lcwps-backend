# GIT GOVERNANCE – LCWPS

## Branch Model + Commit Rules + Release Control

**Authority:** CTO, DevOps Lead
**Classification:** GOVERNANCE
**Date:** 2026-02-23
**Status:** BINDING

### Resumen (ES)

- Ramas protegidas con 2 aprobaciones y commits firmados.
- Formato de mensajes de commit estandarizado.
- Tags firmados para releases en `main`.

---

## 1. Branch Model (Mandatory)

- `main`: production releases only
- `qa`: pre-production validation
- `develop`: integration branch
- `feature/*`: feature work
- `hotfix/*`: emergency fixes

Direct commits to `main`, `qa`, or `develop` are prohibited.

---

## 2. Commit Requirements

- **Signed commits required (GPG).**
- Commit message format: `type(scope): summary`

Examples:

- `feat(queue): enforce FIFO ordering`
- `fix(notification): prevent duplicate delivery`
- `chore(ci): update security scan thresholds`

---

## 3. Pull Request Workflow

- Minimum 2 approvals from distinct reviewers
- Author cannot approve or merge
- All checks must pass before merge
- Conversation resolution required

---

## 4. Branch Protection Rules

For `main`, `qa`, `develop`:

- Require pull request reviews before merging
- Require signed commits
- Require status checks to pass
- Prevent force pushes and branch deletion
- Dismiss stale approvals on new commits

---

## 5. Release Tagging

- Releases are tagged on `main` only
- Tags must be signed: `vX.Y.Z`

Example:

```
git tag -s v1.2.3 -m "Release v1.2.3"
git push origin v1.2.3
```

---

## 6. Emergency Procedure (Hotfix)

1. Create `hotfix/*` from `main`
2. Apply fix + tests
3. PR with two-person approval
4. Merge to `main` and backport to `develop`

---

**Document Status:** ✓ APPROVED
**Review:** Quarterly (git audit)
**Next Review:** 2026-05-23
