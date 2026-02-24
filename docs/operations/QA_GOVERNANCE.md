# QA GOVERNANCE – LCWPS

## Test Strategy + Coverage Requirements + Quality Gates

**Authority:** QA Lead, Testing Architect
**Classification:** QUALITY STANDARD
**Date:** 2026-02-23
**Status:** BINDING

### Resumen (ES)

- Piramide de pruebas con enfasis en unitarias y dominio.
- Cobertura >= 95% general y dominio.
- Pruebas de rendimiento y seguridad obligatorias.

---

## 1. Testing Pyramid

```
                    ▲
                   /|\
                  / | \
                 /  |  \          E2E Tests (10%)
                /   |   \         - User workflows
               /    |    \        - Multi-service scenarios
              /     |     \       - Production-like env
             ├──────┼──────┤
            /       |       \
           /        |        \     Integration Tests (20%)
          /         |         \    - Service contracts
         /          |          \   - Database interactions
        /           |           \  - RabbitMQ events
       ├────────────┼────────────┤
      /             |             \    Unit + Domain Tests (70%)
     /              |              \   - Domain invariants
    /               |               \  - Use cases
   /                |                \ - Controllers (minimal)
  /                 |                 \- Repositories (mocked)
 ╱──────────────────┼──────────────────╲
```

**Coverage Thresholds:**

- General coverage: >= 95%
- Domain/business logic: >= 95%

---

## 2. Test Types & Scope

### 2.1 Unit Tests

**Framework:** xUnit + Moq + FluentAssertions
**Scope:** Domain entities, value objects, services, use cases

### 2.2 Domain Tests (BDD)

**Framework:** SpecFlow (Gherkin)
**Focus:** Business rules (FIFO, manual verification, state transitions)

### 2.3 Integration Tests

**Scope:**

- Database migrations and RLS policies
- Event publishing/consumption (RabbitMQ)
- API endpoints with persistence

### 2.4 Contract Tests

**Scope:** API contracts and message schemas between services

### 2.5 End-to-End Tests

**Scope:** Full appointment flow with manual payment verification and notifications

---

## 3. Performance Testing

**Tools:** k6 or JMeter
**Targets:**

- Queue lookup p95 < 100ms
- Payment verification p95 < 500ms
- Notification delivery < 1s

---

## 4. Security Testing

**OWASP Top 10 coverage required:**

- Broken Access Control
- Injection
- Authentication Failures
- Security Misconfiguration

**Minimum:** automated tests + SAST checks per PR.

---

## 5. Test Execution Requirements

| Trigger | Required Tests | SLA |
|---------|----------------|-----|
| Each commit | Unit + Domain | < 5 min |
| PR creation | Unit + Domain + Integration | < 15 min |
| Merge to develop | All tests (Unit + Integration + E2E) | < 1 hour |
| Release tag | All tests + Performance + Security | < 2 hours |

**Isolation Rules:**

- Tests must be independent
- Database state reset between integration tests
- No reliance on execution order

---

## 6. Defect Tracking

**Rule:** Bug escape rate < 1 per 1000 lines of code

---

**Document Status:** ✓ APPROVED
**Review:** Quarterly (test effectiveness)
**Next Review:** 2026-05-23
