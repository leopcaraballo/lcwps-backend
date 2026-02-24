# BUSINESS RULES CODIFICATION – LCWPS

## Real-Time Orchestration, Manual Payment Verification, Queue Management

**Authority:** Product Owner, Domain Expert, Chief Architect
**Classification:** BUSINESS CRITICAL
**Date:** 2026-02-23
**Version:** 2.0
**Status:** ✓ APPROVED

---

## System Redefinition (Binding)

LCWPS is a real-time orchestration and notification system for clinical appointments. Payment verification is manual and required before queue entry.

---

## 1. Queue and Priority Rules

### BR-001: FIFO Within Priority (MANDATORY)

Patients are served in strict FIFO order **within the same priority level**.

### BR-002: Automatic Priority Classification (MANDATORY)

- Minor (< 18 years) → **HIGH**
- Pregnant → **HIGH**
- Senior (>= 65 years) → **HIGH**
- Others → **NORMAL**

Priority is **automatic and immutable**.

---

## 2. State Machine Rules

### BR-003: Appointment State Machine (UPDATED)

```
CREATED
  -> PAYMENT_PENDING_VERIFICATION (automatic)
  -> PAYMENT_VERIFIED (manual) -> WAITING (automatic)
  -> PAYMENT_REJECTED (manual, terminal)
WAITING -> CALLED -> IN_SERVICE -> COMPLETED (terminal)
WAITING/CALLED -> NO_SHOW (terminal)
Any non-terminal -> CANCELLED (terminal)
```

---

## 3. Manual Payment Verification Rules

### BR-004: Manual Verification Required (CRITICAL)

Payment verification is performed **only** by authorized roles:

- `FinancialVerifier`
- `Supervisor`
- `Admin`

### BR-005: Bypass Prevention (CRITICAL)

No appointment can reach `WAITING` without a recorded manual verification event.

---

## 4. Notification Rules

### BR-006: Immediate Notification (CRITICAL)

All significant state changes must emit notifications with < 1 second latency target.

### BR-007: Non-Blocking Delivery

Notification failures **must not** block appointment flow.

### BR-008: Idempotency

Events are processed idempotently using a unique `EventId` per delivery channel.

### BR-009: Retry Policy

Retry up to 3 attempts with exponential backoff for failed notifications.

---

## 5. Tenant Isolation Rules

### BR-010: Strict Multi-Tenant Isolation

Tenant A must never access Tenant B data. Enforced through application authorization and PostgreSQL RLS.

---

## 6. Audit and Immutability Rules

### BR-011: Immutable Audit Trail

All state changes, verification actions, and sensitive operations must be recorded immutably (append-only).

**Retention:** minimum 5 years.

---

## 7. Event Catalog (Binding)

### Appointment Lifecycle

- `AppointmentCreatedEvent`
- `PaymentPendingVerificationEvent`
- `PaymentVerifiedEvent`
- `PaymentRejectedEvent`
- `AppointmentMovedToWaitingEvent`
- `AppointmentCalledEvent`
- `AppointmentInServiceEvent`
- `AppointmentCompletedEvent`
- `AppointmentCancelledEvent`
- `AppointmentNoShowEvent`

### Notification Events

- `NotificationSentEvent`
- `NotificationFailedEvent`

---

## 8. Business Rule Validation Matrix

| Rule ID | Enforcement Point | Test Type | Status |
|---------|-------------------|-----------|--------|
| BR-001 | Queue ordering query | Unit + Integration | ✓ Active |
| BR-002 | Priority classification service | Unit | ✓ Active |
| BR-003 | Appointment aggregate transitions | Unit + Integration | ✓ Active |
| BR-004 | Verification workflow | Integration + E2E | ✓ Active |
| BR-005 | Move-to-waiting guard | Unit + Integration | ✓ Active |
| BR-006 | Notification dispatcher | Integration + Performance | ✓ Active |
| BR-007 | Notification error handling | Unit + Integration | ✓ Active |
| BR-008 | Delivery log constraint | Integration | ✓ Active |
| BR-009 | Retry scheduler | Integration | ✓ Active |
| BR-010 | RLS + auth middleware | Integration + E2E | ✓ Active |
| BR-011 | Audit triggers | Database integrity test | ✓ Active |

---

## 9. Status Enums (Reference)

```csharp
public enum AppointmentStatus
{
    Created = 0,
    PaymentPendingVerification = 1,
    PaymentVerified = 2,
    PaymentRejected = 3,
    Waiting = 4,
    Called = 5,
    InService = 6,
    Completed = 7,
    Cancelled = 8,
    NoShow = 9
}

public enum Priority
{
    Normal = 0,
    High = 1
}

public enum DeliveryStatus
{
    Pending = 0,
    Delivered = 1,
    Failed = 2,
    Retry = 3
}
```

---

**Document Status:** ✓ APPROVED – Implementation Binding
**Review:** Quarterly (two-person approval required)
**Next Review:** 2026-05-23
