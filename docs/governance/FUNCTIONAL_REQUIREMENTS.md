# FUNCTIONAL REQUIREMENTS – LCWPS

## Real-Time Orchestration System with Manual Payment Verification

**Authority:** Product Owner, Business Analyst, Chief Architect
**Classification:** BUSINESS CRITICAL
**Date:** 2026-02-23
**Language:** English (Technical); Spanish (Reference)
**Status:** ✓ APPROVED

---

## 1. SYSTEM PURPOSE

LCWPS is a **real-time orchestration and notification system** for clinical appointment management with manual payment verification workflow.

### Resumen (ES)

- Orquestacion y notificacion en tiempo real para turnos clinicos.
- Verificacion manual de pago antes de ingresar a la cola activa.
- Priorizacion automatica y FIFO estricto.
- Auditoria inmutable y trazabilidad de eventos.

**Primary Objectives:**

1. Organize appointments in queue under deterministic rules (FIFO + Priority)
2. Notify all stakeholders in real-time (< 1s latency)
3. Enable manual supervised payment verification
4. Guarantee traceability, equity, and automatic prioritization
5. Ensure optimal patient experience

---

## 2. FUNCTIONAL REQUIREMENTS

### FR-001: Appointment Management

**Priority:** CRITICAL
**Category:** Core Business Function

#### FR-001.1: Create Appointment

**Actor:** Patient, Receptionist, System Administrator

**Preconditions:**

- User is authenticated
- Patient exists in system
- Tenant context is established

**Main Flow:**

1. User initiates appointment creation
2. System validates patient information
3. System automatically classifies priority based on demographics:
   - Age < 18 → HIGH
   - Age ≥ 65 → HIGH
   - Is Pregnant → HIGH
   - Others → NORMAL
4. System creates appointment with status `CREATED`
5. System automatically transitions to `PAYMENT_PENDING_VERIFICATION`
6. System emits `AppointmentCreatedEvent` and `PaymentPendingVerificationEvent`
7. System displays appointment confirmation with unique ID

**Postconditions:**

- Appointment created with automatic priority assignment
- Payment verification pending
- Notifications sent to patient and financial staff

**Business Rules:**

- BR-002: Automatic Priority Classification
- BR-003: State Machine Transitions

**Acceptance Criteria:**

```gherkin
Given a patient with age 25 and not pregnant
When an appointment is created
Then priority should be NORMAL
And status should be PAYMENT_PENDING_VERIFICATION
And AppointmentCreatedEvent should be emitted
```

---

#### FR-001.2: Query Appointment Status

**Actor:** Patient, Staff

**Preconditions:**

- User is authenticated
- User has permission to view appointment

**Main Flow:**

1. User requests appointment details
2. System validates authorization (tenant isolation)
3. System returns:
   - Current status
   - Position in queue (if in WAITING state)
   - Assigned priority (with explanation)
   - Estimated wait time
   - Historical state transitions

**Postconditions:**

- User informed of current appointment status

**Business Rules:**

- BR-010: Tenant Isolation

**Acceptance Criteria:**

```gherkin
Given an appointment in WAITING state
When patient queries their appointment
Then they should see their current position in queue
And they should see estimated wait time
And they should see their priority level
```

---

#### FR-001.3: Cancel Appointment

**Actor:** Patient, Receptionist, Doctor

**Preconditions:**

- Appointment exists
- Appointment is not in terminal state (COMPLETED, NO_SHOW)

**Main Flow:**

1. User selects appointment to cancel
2. System validates current state
3. System prompts for cancellation reason (mandatory)
4. User provides reason
5. System transitions to `CANCELLED` state
6. System emits `AppointmentCancelledEvent`
7. System sends notification to patient (if not patient-initiated)

**Postconditions:**

- Appointment cancelled
- Reason recorded in audit log
- Notifications sent

**Business Rules:**

- BR-003: State Machine Transitions
- BR-011: Immutable Audit Trail

**Acceptance Criteria:**

```gherkin
Given an appointment in WAITING state
When user cancels with reason "Patient requested"
Then status should change to CANCELLED
And cancellation reason should be recorded
And AppointmentCancelledEvent should be emitted
```

---

#### FR-001.4: View Queue Position

**Actor:** Patient

**Preconditions:**

- Appointment is in WAITING state
- User is authenticated as appointment owner

**Main Flow:**

1. Patient requests queue position
2. System calculates position based on:
   - Priority (HIGH before NORMAL)
   - Created timestamp (FIFO within priority)
3. System returns:
   - Absolute position (e.g., "5th in line")
   - Relative position within priority (e.g., "3rd of 8 HIGH priority")
   - Estimated wait time

**Postconditions:**

- Patient informed of queue position

**Business Rules:**

- BR-001: FIFO Within Priority Level

**Acceptance Criteria:**

```gherkin
Given 3 HIGH priority appointments created before mine
And 2 HIGH priority appointments created after mine
And 10 NORMAL priority appointments
When I query my position
Then I should see "4th in line (4th of 5 HIGH priority)"
```

---

#### FR-001.5: View Assigned Priority

**Actor:** Patient, Staff

**Preconditions:**

- Appointment exists

**Main Flow:**

1. User views appointment details
2. System displays:
   - Priority level (HIGH or NORMAL)
   - Reason for priority (e.g., "High priority: Senior citizen")
   - Notice that priority is automatic and non-editable

**Postconditions:**

- User understands priority assignment

**Business Rules:**

- BR-002: Automatic Priority Classification
- Priority CANNOT be manually changed

**Acceptance Criteria:**

```gherkin
Given a patient aged 70
When appointment is created
Then priority should be HIGH
And reason should display "High priority: Senior citizen (age ≥ 65)"
And priority field should not be editable
```

---

### FR-002: Manual Payment Verification

**Priority:** CRITICAL
**Category:** Core Business Function

#### FR-002.1: View Pending Verifications

**Actor:** Financial Verifier, Supervisor, Admin

**Preconditions:**

- User has role: FinancialVerifier, Supervisor, or Admin
- User is authenticated

**Main Flow:**

1. User navigates to "Pending Payment Verifications" dashboard
2. System displays list of appointments in `PAYMENT_PENDING_VERIFICATION` state
3. List includes:
   - Appointment ID
   - Patient name
   - Created timestamp
   - Time pending (e.g., "15 minutes ago")
   - Priority level
4. List is sortable and filterable
5. System auto-refreshes list every 30 seconds

**Postconditions:**

- User sees all pending verifications for their tenant

**Business Rules:**

- BR-004: Manual Verification Required
- BR-010: Tenant Isolation

**Acceptance Criteria:**

```gherkin
Given 5 appointments pending verification
And user has FinancialVerifier role
When user opens pending verifications dashboard
Then they should see all 5 appointments
And list should auto-refresh every 30 seconds
```

---

#### FR-002.2: Execute Payment Verification (Approve)

**Actor:** Financial Verifier, Supervisor, Admin

**Preconditions:**

- Appointment is in `PAYMENT_PENDING_VERIFICATION` state
- User has authorization role
- User has verified payment in external system

**Main Flow:**

1. User selects appointment to verify
2. System displays appointment details
3. User clicks "Verify Payment" button
4. System prompts for confirmation
5. User confirms verification
6. User optionally adds observation notes
7. System:
   - Validates user authorization
   - Transitions appointment to `PAYMENT_VERIFIED`
   - Automatically transitions to `WAITING`
   - Creates immutable audit record
   - Emits `PaymentVerifiedEvent` and `AppointmentMovedToWaitingEvent`
   - Sends notifications to patient
8. System displays success confirmation

**Postconditions:**

- Appointment moved to WAITING queue
- Verification recorded in audit trail
- Patient notified

**Business Rules:**

- BR-004: Manual Verification Required
- BR-005: Bypass Prevention
- BR-011: Immutable Audit Trail

**Acceptance Criteria:**

```gherkin
Given an appointment pending verification
And user has FinancialVerifier role
When user verifies payment
Then appointment should move to WAITING state
And audit record should be created with verifier ID and timestamp
And patient should receive notification
```

---

#### FR-002.3: Execute Payment Verification (Reject)

**Actor:** Financial Verifier, Supervisor, Admin

**Preconditions:**

- Appointment is in `PAYMENT_PENDING_VERIFICATION` state
- User has authorization role
- Payment verification failed in external system

**Main Flow:**

1. User selects appointment to verify
2. System displays appointment details
3. User clicks "Reject Payment" button
4. System prompts for mandatory rejection reason
5. User enters rejection reason (e.g., "Payment not found in system")
6. User optionally adds observation notes
7. User confirms rejection
8. System:
   - Validates user authorization
   - Validates rejection reason is provided
   - Transitions appointment to `PAYMENT_REJECTED` (terminal state)
   - Creates immutable audit record
   - Emits `PaymentRejectedEvent`
   - Sends notifications to patient (Web, Mobile, Email)
9. System displays confirmation

**Postconditions:**

- Appointment rejected (terminal state)
- Rejection reason recorded
- Patient notified with reason

**Business Rules:**

- BR-004: Manual Verification Required
- BR-011: Immutable Audit Trail

**Acceptance Criteria:**

```gherkin
Given an appointment pending verification
And user has FinancialVerifier role
When user rejects payment with reason "Payment not found"
Then appointment should move to PAYMENT_REJECTED state
And rejection reason should be recorded
And patient should receive rejection notification
And appointment should not enter WAITING queue
```

---

#### FR-002.4: View Verification History

**Actor:** Financial Verifier (own), Supervisor (all), Admin (all)

**Preconditions:**

- User has authorization

**Main Flow:**

1. User navigates to "Verification History"
2. System displays list of past verifications:
   - Appointment ID
   - Patient name
   - Verifier name
   - Verification timestamp
   - Result (Approved / Rejected)
   - Rejection reason (if applicable)
   - Observation notes (if any)
3. List is filterable by date range, verifier, result
4. User can export to CSV for external audit

**Postconditions:**

- User views verification history

**Business Rules:**

- BR-011: Immutable Audit Trail
- Authorization matrix (see BR-004)

**Acceptance Criteria:**

```gherkin
Given 10 verifications performed today
And user has Supervisor role
When user opens verification history
Then they should see all 10 verifications
And they should be able to filter by date range
And they should be able to export to CSV
```

---

### FR-003: Real-Time Notification System

**Priority:** CRITICAL
**Category:** Core System Function

#### FR-003.1: Emit Notifications on State Change

**Actor:** System (automated)

**Preconditions:**

- Appointment state change occurs
- Event is emitted by aggregate root

**Main Flow:**

1. Appointment state changes (e.g., WAITING → CALLED)
2. Aggregate emits domain event (e.g., `AppointmentCalledEvent`)
3. Notification Service subscribes to event
4. System determines notification channels based on event type
5. System publishes notifications to:
   - WebSocket (real-time web/mobile app)
   - Mobile push notification (if configured)
   - Email (for critical events)
   - Waiting room display panel (for CALLED events)
6. System records delivery attempt in `notification_delivery_log`
7. If delivery fails, system schedules retry (3 attempts max)

**Postconditions:**

- Notifications sent to all relevant channels
- Delivery attempts logged

**Business Rules:**

- BR-006: Immediate Notification (< 1s latency)
- BR-007: Non-Blocking (failures don't block flow)
- BR-008: Idempotency

**Acceptance Criteria:**

```gherkin
Given an appointment transitions to CALLED state
When AppointmentCalledEvent is emitted
Then WebSocket notification should be sent within 1 second
And waiting room display should show appointment
And delivery log should record successful delivery
```

---

#### FR-003.2: Support Multiple Concurrent Clients

**Actor:** System (automated)

**Preconditions:**

- Multiple clients connected via WebSocket

**Main Flow:**

1. Event occurs (e.g., new appointment in queue)
2. System identifies all connected clients for tenant
3. System broadcasts event to all clients simultaneously
4. Each client receives notification independently
5. System ensures order consistency (events delivered in order)

**Postconditions:**

- All clients receive notification

**Business Rules:**

- BR-006: Immediate Notification
- BR-010: Tenant Isolation

**Performance Requirements:**

- Support 1000+ concurrent WebSocket connections per tenant
- Maintain < 1s latency under load

**Acceptance Criteria:**

```gherkin
Given 50 clients connected for tenant A
When an event is published for tenant A
Then all 50 clients should receive notification within 1 second
And clients from tenant B should not receive notification
```

---

#### FR-003.3: Maintain Event Consistency

**Actor:** System (automated)

**Preconditions:**

- Multiple events emitted in sequence

**Main Flow:**

1. Appointment state changes multiple times (e.g., CREATED → PAYMENT_PENDING → PAYMENT_VERIFIED → WAITING)
2. System emits events in order
3. System ensures clients receive events in same order
4. System prevents race conditions via event sequencing

**Postconditions:**

- Event order preserved

**Business Rules:**

- BR-008: Idempotency

**Acceptance Criteria:**

```gherkin
Given appointment transitions CREATED → PAYMENT_VERIFIED → WAITING
When events are emitted
Then client should receive events in exact order
And no events should be skipped
```

---

#### FR-003.4: Handle Notification Failures Gracefully

**Actor:** System (automated)

**Preconditions:**

- Notification channel unavailable (e.g., mobile push service down)

**Main Flow:**

1. System attempts to send notification
2. Delivery fails (e.g., timeout, service error)
3. System logs failure in `notification_delivery_log`
4. System schedules retry with exponential backoff:
   - Attempt 1: immediate
   - Attempt 2: 1 second
   - Attempt 3: 2 seconds
   - Attempt 4: 4 seconds
5. If all attempts fail, system logs permanent failure
6. **Critical:** Appointment flow continues (non-blocking)

**Postconditions:**

- Failure logged
- Retry attempted
- Appointment flow not blocked

**Business Rules:**

- BR-007: Non-Blocking
- BR-008: Idempotency

**Acceptance Criteria:**

```gherkin
Given mobile push service is unavailable
When appointment transitions to CALLED
Then WebSocket notification should still be sent
And appointment should remain in CALLED state
And failure should be logged for mobile push
And retry should be scheduled
```

---

#### FR-003.5: Prevent Duplicate Notifications

**Actor:** System (automated)

**Preconditions:**

- Event is processed multiple times (e.g., retry)

**Main Flow:**

1. Event is published with unique `EventId`
2. System checks `notification_delivery_log` for existing delivery
3. If already delivered, system skips notification
4. If not delivered, system processes notification
5. System records delivery with `(EventId, Channel)` as unique key

**Postconditions:**

- Duplicate notifications prevented

**Business Rules:**

- BR-008: Idempotency

**Acceptance Criteria:**

```gherkin
Given an event already delivered via WebSocket
When the same event is retried
Then duplicate notification should not be sent
And existing delivery log entry should prevent duplicate
```

---

### FR-004: Prioritization and FIFO

**Priority:** CRITICAL
**Category:** Core Business Function

#### FR-004.1: Calculate Priority Automatically

**Actor:** System (automated)

**Preconditions:**

- Appointment is being created

**Main Flow:**

1. System receives patient information
2. System extracts: Date of Birth, Is Pregnant
3. System calculates age
4. System applies priority rules:
   - IF age < 18 THEN Priority = HIGH (reason: "Minor")
   - ELSE IF age ≥ 65 THEN Priority = HIGH (reason: "Senior")
   - ELSE IF isPregnant = true THEN Priority = HIGH (reason: "Pregnant")
   - ELSE Priority = NORMAL
5. System assigns priority (immutable, non-editable)

**Postconditions:**

- Priority assigned automatically

**Business Rules:**

- BR-002: Automatic Priority Classification

**Acceptance Criteria:**

```gherkin
Scenario Outline: Priority calculation
  Given a patient with age <age> and pregnant status <pregnant>
  When appointment is created
  Then priority should be <priority>
  And reason should be <reason>

  Examples:
    | age | pregnant | priority | reason    |
    | 10  | false    | HIGH     | Minor     |
    | 25  | false    | NORMAL   | Standard  |
    | 25  | true     | HIGH     | Pregnant  |
    | 70  | false    | HIGH     | Senior    |
```

---

#### FR-004.2: Apply FIFO Ordering Within Priority

**Actor:** System (automated)

**Preconditions:**

- Multiple appointments in WAITING state

**Main Flow:**

1. System maintains queue ordered by:
   - Priority DESC (HIGH before NORMAL)
   - CreatedAt ASC (oldest first within priority)
2. When staff calls next appointment, system returns first in order
3. System prevents manual reordering
4. System handles concurrency with optimistic locking

**Postconditions:**

- FIFO order maintained

**Business Rules:**

- BR-001: FIFO Within Priority Level

**Acceptance Criteria:**

```gherkin
Given appointments in queue:
  | ID | Priority | CreatedAt           |
  | A1 | HIGH     | 2026-02-23 10:00:00 |
  | A2 | HIGH     | 2026-02-23 10:05:00 |
  | A3 | NORMAL   | 2026-02-23 09:50:00 |
  | A4 | NORMAL   | 2026-02-23 10:10:00 |
When next appointment is requested
Then order should be: A1, A2, A3, A4
```

---

#### FR-004.3: Prevent Manual Reordering

**Actor:** System (enforcement)

**Preconditions:**

- User attempts to manually change queue order

**Main Flow:**

1. User requests to move appointment in queue
2. System rejects request with error message
3. System logs unauthorized attempt in audit log

**Postconditions:**

- Reordering prevented
- Attempt logged

**Business Rules:**

- BR-001: FIFO Within Priority Level

**Acceptance Criteria:**

```gherkin
Given user attempts to move appointment A2 before A1
When system processes request
Then request should be rejected
And error message should state "Manual reordering not permitted"
And attempt should be logged in audit
```

---

#### FR-004.4: Handle High Concurrency

**Actor:** System (automated)

**Preconditions:**

- Multiple appointments created simultaneously

**Main Flow:**

1. System uses database timestamp with microsecond precision
2. System applies optimistic locking on queue reads
3. System resolves conflicts by timestamp
4. System maintains FIFO order under load

**Postconditions:**

- Queue order correct under concurrency

**Performance Requirements:**

- Handle 100 concurrent appointment creations
- Maintain FIFO order accuracy

**Acceptance Criteria:**

```gherkin
Given 50 appointments created within 1 second
When queue is queried
Then all appointments should be in correct FIFO order
And no appointments should be lost
```

---

### FR-005: Call Management

**Priority:** HIGH
**Category:** Operational Function

#### FR-005.1: Call Appointment from Queue

**Actor:** Doctor, Nurse, Receptionist

**Preconditions:**

- User is authenticated
- Appointment is in WAITING state

**Main Flow:**

1. User views list of appointments in WAITING state
2. User selects next appointment (system suggests first in FIFO order)
3. User clicks "Call Patient" button
4. System:
   - Validates appointment is in WAITING state
   - Transitions to CALLED state
   - Increments call attempt counter
   - Records caller ID and timestamp
   - Emits `AppointmentCalledEvent`
   - Sends notifications (WebSocket, Mobile, Waiting Panel)
5. Waiting room display shows:
   - Appointment number
   - Doctor/Room name
   - Visual and audio alert

**Postconditions:**

- Appointment in CALLED state
- Patient notified
- Display updated

**Business Rules:**

- BR-003: State Machine Transitions

**Acceptance Criteria:**

```gherkin
Given appointment in WAITING state
And user has Doctor role
When user clicks "Call Patient"
Then appointment should move to CALLED state
And waiting room display should show appointment
And patient should receive mobile notification
```

---

#### FR-005.2: Display on Waiting Room Panel

**Actor:** System (automated)

**Preconditions:**

- Appointment transitioned to CALLED state

**Main Flow:**

1. System receives `AppointmentCalledEvent`
2. System updates waiting room display:
   - Shows appointment number prominently
   - Shows doctor/room name
   - Displays for configured duration (e.g., 2 minutes)
   - Plays audio alert
3. Display cycles through recent calls if multiple

**Postconditions:**

- Display updated in real-time

**Business Rules:**

- BR-006: Immediate Notification (< 1s latency)

**Performance Requirements:**

- Display update latency < 500ms

**Acceptance Criteria:**

```gherkin
Given appointment transitioned to CALLED
When AppointmentCalledEvent is emitted
Then waiting room display should update within 500ms
And audio alert should play
And display should show for at least 2 minutes
```

---

#### FR-005.3: Record Call Attempts

**Actor:** System (automated)

**Preconditions:**

- Appointment is called

**Main Flow:**

1. System increments `CallAttempts` counter
2. System records timestamp of each call
3. System stores in audit log

**Postconditions:**

- Call attempts tracked

**Business Rules:**

- BR-011: Immutable Audit Trail

**Acceptance Criteria:**

```gherkin
Given appointment with 0 call attempts
When appointment is called
Then CallAttempts should be 1
And call timestamp should be recorded
```

---

#### FR-005.4: Mark as No-Show

**Actor:** Doctor, Nurse, Receptionist

**Preconditions:**

- Appointment is in CALLED state
- Patient has not responded after configured attempts (e.g., 3 calls)

**Main Flow:**

1. User clicks "Mark as No-Show" button
2. System validates:
   - Appointment is in CALLED state
   - CallAttempts ≥ configured maximum (e.g., 3)
3. System prompts for reason
4. User enters reason (e.g., "Patient not in waiting room")
5. System:
   - Transitions to NO_SHOW state
   - Records reason and timestamp
   - Emits `AppointmentNoShowEvent`
   - Sends notification to patient (Email + Mobile)
   - Logs in audit trail

**Postconditions:**

- Appointment marked as NO_SHOW
- Patient notified

**Business Rules:**

- BR-003: State Machine Transitions
- CallAttempts must meet minimum before NO_SHOW allowed

**Acceptance Criteria:**

```gherkin
Given appointment called 3 times
And patient has not responded
When user marks as No-Show with reason "Not in waiting room"
Then appointment should transition to NO_SHOW state
And patient should receive notification
And reason should be recorded
```

---

### FR-006: Audit and Traceability

**Priority:** CRITICAL
**Category:** Compliance & Governance

#### FR-006.1: Record State Transitions

**Actor:** System (automated)

**Preconditions:**

- Appointment state changes

**Main Flow:**

1. State transition occurs (e.g., WAITING → CALLED)
2. System creates immutable audit entry:
   - AppointmentId
   - PreviousState
   - NewState
   - ChangedBy (user ID)
   - ChangedAt (timestamp)
   - Reason (if applicable)
   - TenantId
3. System stores in `appointment_audit_log` table
4. PostgreSQL trigger prevents UPDATE/DELETE on audit records

**Postconditions:**

- State transition recorded immutably

**Business Rules:**

- BR-011: Immutable Audit Trail

**Retention:** 5 years minimum

**Acceptance Criteria:**

```gherkin
Given appointment transitions from WAITING to CALLED
When state change occurs
Then audit entry should be created
And entry should include user ID, timestamp, previous state, new state
And entry cannot be modified or deleted
```

---

#### FR-006.2: Record Payment Verifications

**Actor:** System (automated)

**Preconditions:**

- Payment verification executed

**Main Flow:**

1. Verification occurs (approve or reject)
2. System creates immutable audit entry in `payment_verification_audit`:
   - AppointmentId
   - VerifiedBy (user ID)
   - VerifiedAt (timestamp)
   - Result (Approved / Rejected)
   - RejectionReason (if rejected)
   - ObservationNotes (if provided)
   - TenantId
3. PostgreSQL trigger prevents modification

**Postconditions:**

- Verification recorded immutably

**Business Rules:**

- BR-011: Immutable Audit Trail

**Retention:** 5 years minimum

**Acceptance Criteria:**

```gherkin
Given user verifies payment
When verification is executed
Then audit entry should be created in payment_verification_audit
And entry should be immutable
```

---

#### FR-006.3: Query Audit Trail

**Actor:** Auditor, Supervisor, Admin

**Preconditions:**

- User has authorization

**Main Flow:**

1. User navigates to "Audit Log" section
2. User applies filters:
   - Date range
   - Appointment ID
   - Event type (State Change, Payment Verification, etc.)
   - User ID
3. System displays audit entries
4. User can export to CSV for external audit

**Postconditions:**

- Audit trail queried

**Business Rules:**

- BR-011: Immutable Audit Trail
- BR-010: Tenant Isolation

**Acceptance Criteria:**

```gherkin
Given 100 audit entries for tenant A
And user has Auditor role for tenant A
When user queries audit log
Then they should see all 100 entries for tenant A
And they should not see entries for tenant B
And they should be able to export to CSV
```

---

#### FR-006.4: Prevent Audit Log Modification

**Actor:** System (enforcement)

**Preconditions:**

- User attempts to modify audit log

**Main Flow:**

1. User attempts UPDATE or DELETE on audit table
2. PostgreSQL trigger rejects operation
3. System returns error: "Audit logs are immutable"
4. System logs unauthorized attempt

**Postconditions:**

- Modification prevented
- Attempt logged

**Business Rules:**

- BR-011: Immutable Audit Trail

**Acceptance Criteria:**

```gherkin
Given audit entry exists
When user attempts to delete or update entry
Then operation should be rejected
And error message should state "Audit logs are immutable"
```

---

## 3. NON-FUNCTIONAL REQUIREMENTS

### NFR-001: Real-Time Performance

| Metric | Target | Critical |
|--------|--------|----------|
| **Internal notification latency** | < 500ms | < 1s |
| **Mobile push latency** | < 2s | < 5s |
| **Waiting panel update** | < 500ms | < 1s |
| **Event throughput** | 1000 events/s | 500 events/s |
| **WebSocket connection capacity** | 1000 concurrent/tenant | 500 concurrent/tenant |

### NFR-002: Security

- OAuth 2.0 + JWT authentication (mandatory)
- Role-based authorization (RBAC)
- Multi-tenant isolation (PostgreSQL RLS)
- TLS 1.3 encryption (in-transit)
- AES-256 encryption (at-rest)
- Audit log immutability (PostgreSQL triggers)

### NFR-003: Availability

- **SLA:** ≥ 99.8% uptime (≤ 8.6 hours downtime/month)
- **High Availability:** PostgreSQL replication
- **Failover:** Kubernetes auto-healing (< 30s)
- **Disaster Recovery:** RPO < 1 hour, RTO < 4 hours

### NFR-004: Data Integrity

- No event loss (persistent message queue)
- Idempotent processing (deduplication by EventId)
- Eventual consistency (< 5 seconds)
- Optimistic locking (prevent race conditions)

### NFR-005: Scalability

- Horizontal scaling (Kubernetes pods)
- Load balancing (YARP / nginx)
- Database connection pooling
- Message queue distribution (RabbitMQ)

---

## 4. USER ROLES & PERMISSIONS

| Role | Permissions |
|------|-------------|
| **Patient** | View own appointments, cancel own appointments, receive notifications |
| **Receptionist** | Create appointments, view queue, call appointments, cancel appointments |
| **Doctor** | View queue, call appointments, start service, complete service, mark no-show |
| **Nurse** | View queue, call appointments |
| **FinancialVerifier** | View pending verifications, verify payments (approve/reject), view own verification history |
| **Supervisor** | All of above + view all verification history + manage users |
| **Admin** | All permissions + system configuration + audit access |

---

## 5. GLOSSARY

| Term | Definition |
|------|------------|
| **Appointment** | Entity representing a patient's scheduled visit |
| **Priority** | Automatic classification (HIGH or NORMAL) based on demographics |
| **FIFO** | First-In-First-Out ordering within same priority level |
| **Manual Verification** | Human-supervised payment approval/rejection process |
| **Real-Time Notification** | Event-driven message delivered within 1 second |
| **Tenant** | Isolated clinic/organization in multi-tenant system |
| **Audit Trail** | Immutable log of all state changes and verifications |
| **State Machine** | Defined transitions between appointment states |

---

**Document Status:** ✓ APPROVED – Implementation Binding
**Review:** Quarterly (functional requirements changes require two-person approval)
**Next Review:** 2026-05-23 (Q2)
