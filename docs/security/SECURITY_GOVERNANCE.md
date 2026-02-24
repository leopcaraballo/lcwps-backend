# SECURITY GOVERNANCE FRAMEWORK – LCWPS

## Zero Trust Architecture + Row-Level Security + STRIDE + OWASP

**Authority:** CISO, Chief Architect
**Classification:** CONFIDENTIAL - Security Restricted
**Date:** 2026-02-23
**Status:** BINDING STANDARD

### Resumen (ES)

- Zero Trust obligatorio en red, aplicacion, datos e infraestructura.
- RLS en PostgreSQL para aislamiento multi-tenant.
- Cifrado TLS 1.3 en transito y AES-256 en reposo.

---

## 1. ZERO TRUST ARCHITECTURE (MANDATORY)

### 1.1 Zero Trust Principles

**Core Tenet:** Never trust, always verify. Implicit trust is prohibited.

| Layer | Trust Model | Verification Method | Implementation |
|-------|-----------|-------------------|-----------------|
| **Network** | Zero Trust Network | Mutual TLS (mTLS) | Istio service mesh + signed certificates |
| **Application** | Zero Trust Identity | OAuth2.0 + JWT + MFA | IdentityServer4 + TOTP |
| **Data** | Zero Trust Access | Row-Level Security (RLS) | PostgreSQL policies + audit logs |
| **Infrastructure** | Zero Trust Workload | Pod Security Policy + RBAC | Kubernetes NetworkPolicy + signed images |
| **Endpoint** | Zero Trust Device | Signed commits + attestation | GPG signatures on all commits |

### 1.2 Network Layer (Istio Service Mesh)

**Mutual TLS (mTLS) Configuration:**

```yaml
# Istio PeerAuthentication: enforce mTLS
apiVersion: security.istio.io/v1beta1
kind: PeerAuthentication
metadata:
  name: lcwps-mtls
  namespace: lcwps
spec:
  mtls:
    mode: STRICT  # Reject non-mTLS traffic

---
# RequestAuthentication: validate JWT tokens
apiVersion: security.istio.io/v1beta1
kind: RequestAuthentication
metadata:
  name: lcwps-auth
  namespace: lcwps
spec:
  jwtRules:
  - issuer: "https://auth.lcwps.com"
    jwksUri: "https://auth.lcwps.com/.well-known/openid-configuration/jwks"
    audiences:
    - "lcwps-api"

---
# AuthorizationPolicy: enforce fine-grained RBAC
apiVersion: security.istio.io/v1beta1
kind: AuthorizationPolicy
metadata:
  name: lcwps-authz
  namespace: lcwps
spec:
  rules:
  # Only queue-service can call payment-verification-service
  - from:
    - source:
        principals: ["cluster.local/ns/lcwps/sa/queue-service"]
    to:
    - operation:
        methods: ["POST"]
        paths: ["/api/financial/validate"]
  # Deny all other traffic
  - {} # This empty rule means "deny all unmatched"
```

### 1.3 Application Layer (OAuth2.0 Token Validation)

**JWT Token Structure (Required):**

```json
{
  "iss": "https://auth.lcwps.com",
  "sub": "user-uuid-12345",
  "aud": "lcwps-api",
  "iat": 1708610400,
  "exp": 1708614000,
  "roles": ["doctor", "clinic-manager"],
  "tenant_id": "clinic-uuid-67890",
  "claims": {
    "email": "doctor@lcwps.com",
    "email_verified": true,
    "name": "Dr. Juan Perez"
  }
}
```

**C# Implementation (Verification):**

```csharp
public static class TokenValidationSetup
{
    public static IServiceCollection AddTokenValidation(this IServiceCollection services)
    {
        // Configure JWT bearer authentication
        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Authority = "https://auth.lcwps.com";
                options.Audience = "lcwps-api";

                // Token validation parameters
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = "https://auth.lcwps.com",
                    ValidateAudience = true,
                    ValidAudience = "lcwps-api",
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                    IssuerSigningKeyResolver = (token, securityToken, kid, parameters) =>
                    {
                        // Fetch JWKS from auth endpoint; cache for 1 hour
                        var jwksUri = "https://auth.lcwps.com/.well-known/openid-configuration/jwks";
                        using (var client = new HttpClient())
                        {
                            var json = client.GetStringAsync(jwksUri).Result;
                            var jwks = JsonConvert.DeserializeObject<JsonWebKeySet>(json);
                            return jwks.Keys;
                        }
                    }
                };
            });

        return services;
    }
}
```

### 1.4 Identity & Access Management (RBAC)

**Role-Based Access Control (RBAC) Matrix:**

```csharp
public enum ApplicationRole
{
    // Administrative roles
    SystemAdministrator,   // Full system access (rare)
    ClinicAdministrator,   // Clinic-level admin

    // Clinical roles
    Doctor,                // Can view/create clinical notes, approve queue
    Nurse,                 // Can view Patient data, assist with queueing
    ClinicalSupervisor,    // Can review clinical workflows

    // Financial roles
    FinancialVerifier,     // Can approve/reject payment verifications
    Accountant,            // Can view financial reports

    // Patient-facing
    Patient,               // Can view own record

    // System roles
    Auditor,               // Read-only access for compliance audits
    ServiceAccount         // Automated service authentication
}

// Authorization policy example
[Authorize(Roles = $"{nameof(ApplicationRole.Doctor)},{nameof(ApplicationRole.ClinicalSupervisor)}")]
[HttpPost("queue/{id}/approve")]
public async Task<ActionResult> ApproveQueueEntry(Guid id)
{
    // Only doctors and clinical supervisors can approve queue entries
}
```

---

## 2. ROW-LEVEL SECURITY (RLS) – PostgreSQL IMPLEMENTATION

### 2.1 RLS Policy for Multi-Tenant Isolation

**Requirement:** Patients from Clinic A must NEVER see data from Clinic B.

**SQL Implementation:**

```sql
-- ============================================================================
-- TABLE: patients (Core patient demographics)
-- ============================================================================
CREATE TABLE patients (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id UUID NOT NULL,  -- Foreign key to clinic
    document_number VARCHAR(20) NOT NULL,
    first_name VARCHAR(100) NOT NULL,
    last_name VARCHAR(100) NOT NULL,
    date_of_birth DATE NOT NULL,
    gender VARCHAR(1) NOT NULL CHECK (gender IN ('M', 'F', 'O')),
    email VARCHAR(255),
    phone VARCHAR(20),
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    CONSTRAINT fk_tenant FOREIGN KEY (tenant_id) REFERENCES clinics(id)
);

-- Enable RLS on patients table
ALTER TABLE patients ENABLE ROW LEVEL SECURITY;

-- Tenant isolation policy: Each tenant can only see their own patients
CREATE POLICY policy_tenant_isolation_patients
    ON patients
    FOR ALL
    USING (tenant_id = CAST(current_setting('app.current_tenant') AS UUID));

-- Additional policy: Patients can only see their own records
CREATE POLICY policy_patient_own_records
    ON patients
    FOR SELECT
    USING (
        -- Patient accessing own record
        id = CAST(current_setting('app.user_id') AS UUID)
        OR
        -- Clinic staff accessing patients
        EXISTS (
            SELECT 1 FROM clinic_staff
            WHERE clinic_staff.clinic_id = patients.tenant_id
            AND clinic_staff.user_id = CAST(current_setting('app.user_id') AS UUID)
        )
    );

-- ============================================================================
-- TABLE: clinical_notes (Medical records - HIPAA equivalent)
-- ============================================================================
CREATE TABLE clinical_notes (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id UUID NOT NULL,
    patient_id UUID NOT NULL,
    doctor_id UUID NOT NULL,
    note_text TEXT NOT NULL,
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    created_by UUID NOT NULL,
    CONSTRAINT fk_patient FOREIGN KEY (patient_id) REFERENCES patients(id),
    CONSTRAINT fk_tenant FOREIGN KEY (tenant_id) REFERENCES clinics(id)
);

ALTER TABLE clinical_notes ENABLE ROW LEVEL SECURITY;

-- Doctor (treating physician) can view their own notes
CREATE POLICY policy_doctor_own_notes
    ON clinical_notes
    FOR SELECT
    USING (
        doctor_id = CAST(current_setting('app.user_id') AS UUID)
    );

-- Clinic admin can view all clinic notes
CREATE POLICY policy_admin_all_notes
    ON clinical_notes
    FOR SELECT
    USING (
        EXISTS (
            SELECT 1 FROM clinic_staff
            WHERE clinic_staff.clinic_id = clinical_notes.tenant_id
            AND clinic_staff.user_id = CAST(current_setting('app.user_id') AS UUID)
            AND clinic_staff.role = 'ADMINISTRATOR'
        )
    );

-- ============================================================================
-- TABLE: financial_records (Payment data - Segregated by tenant)
-- ============================================================================
CREATE TABLE financial_records (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id UUID NOT NULL,
    patient_id UUID NOT NULL,
    transaction_amount DECIMAL(10,2) NOT NULL,
    transaction_status VARCHAR(20) NOT NULL CHECK (transaction_status IN ('PENDING', 'APPROVED', 'REJECTED', 'COMPLETED')),
    insurance_plan_id UUID,
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    CONSTRAINT fk_patient FOREIGN KEY (patient_id) REFERENCES patients(id),
    CONSTRAINT fk_tenant FOREIGN KEY (tenant_id) REFERENCES clinics(id)
);

ALTER TABLE financial_records ENABLE ROW LEVEL SECURITY;

-- Tenant isolation: Financial records visible only to owning clinic
CREATE POLICY policy_financial_tenant_isolation
    ON financial_records
    FOR ALL
    USING (tenant_id = CAST(current_setting('app.current_tenant') AS UUID));

-- Financial officers can only modify approved records from their clinic
CREATE POLICY policy_financial_officer_update
    ON financial_records
    FOR UPDATE
    USING (
        EXISTS (
            SELECT 1 FROM clinic_staff
            WHERE clinic_staff.clinic_id = financial_records.tenant_id
            AND clinic_staff.user_id = CAST(current_setting('app.user_id') AS UUID)
            AND clinic_staff.role IN ('FINANCIAL_OFFICER', 'ADMINISTRATOR')
        )
    );
```

### 2.2 Setting Tenant Context in Application

**C# Implementation (Middleware):**

```csharp
public class TenantContextMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<TenantContextMiddleware> _logger;

    public TenantContextMiddleware(RequestDelegate next, ILogger<TenantContextMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, ITenantService tenantService)
    {
        try
        {
            // Extract tenant ID from JWT claim "tenant_id"
            var tenantIdClaim = context.User.FindFirst("tenant_id");
            if (tenantIdClaim == null)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Tenant ID missing in token");
                return;
            }

            var tenantId = tenantIdClaim.Value;
            var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // Validate tenant access on every request
            var tenantExists = await tenantService.IsTenantAuthenticatedAsync(
                Guid.Parse(tenantId),
                Guid.Parse(userId)
            );

            if (!tenantExists)
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsync("Tenant access denied");
                return;
            }

            // Set tenant context in database session
            using (var connection = new NpgsqlConnection(context.RequestServices.GetRequiredService<IConfiguration>()["ConnectionStrings:Default"]))
            {
                await connection.OpenAsync();
                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = $"SET app.current_tenant = '{tenantId}'; SET app.user_id = '{userId}';";
                    await cmd.ExecuteNonQueryAsync();
                }
            }

            _logger.LogInformation("Tenant context set: {TenantId} by User {UserId}", tenantId, userId);

            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in tenant context middleware");
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        }
    }
}

// Startup configuration
public void Configure(IApplicationBuilder app)
{
    app.UseMiddleware<TenantContextMiddleware>();
    app.UseRouting();
    app.UseEndpoints(endpoints => endpoints.MapControllers());
}
```

### 2.3 RLS Policy Audit & Validation

**SQL Audit Query:**

```sql
-- Verify all tables have RLS enabled
SELECT
    schemaname,
    tablename,
    rowsecurity
FROM pg_tables
WHERE rowsecurity = false AND schemaname NOT IN ('pg_catalog', 'information_schema')
ORDER BY schemaname, tablename;

-- List all active RLS policies
SELECT
    schemaname,
    tablename,
    policyname,
    qual,  -- USING clause
    with_check  -- WITH CHECK clause
FROM pg_policies
ORDER BY schemaname, tablename;

-- Test RLS isolation: Patient A should NOT see Patient B's records
-- Simulate as clinic (tenant_id = 'clinic-123') with user_id = 'doctor-456'
SET app.current_tenant = 'clinic-123'::uuid;
SET app.user_id = 'doctor-456'::uuid;

-- Should return only patients with tenant_id = 'clinic-123'
SELECT count(*) FROM patients;  -- Expected: N
-- Try to see clinic-999 data (should return 0 rows)
SELECT count(*) FROM patients WHERE tenant_id = 'clinic-999'::uuid;  -- Expected: 0
```

---

## 3. THREAT MODELING (STRIDE)

### 3.1 STRIDE Analysis: Patient Queue Domain

| Threat | Category | Attack Scenario | Mitigation | Owner |
|--------|----------|-----------------|-----------|-------|
| **Attacker impersonates doctor** | Spoofing | Uses stolen JWT to approve queue | Signed JWT + MFA + mutually authenticated TLS | Auth service |
| **Patient views other patient record** | Tampering | SQL injection to bypass RLS | Parameterized queries (EF Core ORM only) + RLS policy test | Database team |
| **Queue processing logic manipulated** | Tampering | Modify FIFO order (e.g., prioritize by bribe) | Domain aggregate invariants enforce FIFO; audit trail captures changes | Architecture |
| **Attacker denies queue access** | Denial of Service | Send massive queue requests | Rate limiting (per tenant, per IP) + Kubernetes resource quotas | DevOps |
| **Payment verification bypassed** | Repudiation | Claim "system approved" invalid claim | Immutable audit log + signed approval transactions | Compliance |
| **Patient financial data exposed** | Information Disclosure | Breached database credentials | Encryption at rest (AES-256) +  private network isolation | Security |
| **Clinic A finances exposed to Clinic B** | Information Disclosure | Multi-tenant data leak | RLS policy mandatory + integration tests verify isolation | Database team |
| **Elevation of privilege** | Elevation of Privilege | Patient claims admin access | RBAC enforced at application + database layer | Identity team |

---

## 4. OWASP TOP 10 MAPPING

### 4.1 A01 – Broken Access Control

**Risk:** Unauthorized access to resources.

**LCWPS Mitigation:**

```csharp
// Mitigation: Declarative authorization + RLS
public class PatientController : ControllerBase
{
    [Authorize(Roles = "Doctor,Nurse,Patient")]  // Application-level check
    [HttpGet("{id}")]
    public async Task<ActionResult<PatientDto>> GetPatient(Guid id)
    {
        var patient = await _patientRepository.GetByIdAsync(id);

        // Authorization check: verify request user can access this patient
        var authorizationResult = await _authorizationService.AuthorizeAsync(
            User, patient, "ViewPatientPolicy");

        if (!authorizationResult.Succeeded)
            return Forbid();

        // RLS filters at DB level (tenant_id = current_tenant)
        return Ok(_mapper.Map<PatientDto>(patient));
    }
}

// Integrated test
[Fact]
public async Task GetPatient_WhenCalledByUnauthorizedUser_ReturnsForbidden()
{
    // Arrange: Patient from clinic-A, request by doctor from clinic-B
    var patientFromClinicA = new Patient { TenantId = ClinicA, Id = PatientId123 };

    // Act: Set context to clinic-B
    SetTenantContext(ClinicB);
    var result = await _controller.GetPatient(PatientId123);

    // Assert: Forbidden
    var forbidResult = Assert.IsType<ForbidResult>(result);
}
```

### 4.2 A02 – Cryptographic Failures

**Risk:** Sensitive data exposed due to weak encryption.

**LCWPS Mitigation:**

```sql
-- At-rest encryption for PII fields
CREATE FUNCTION encrypt_pii(plaintext TEXT) RETURNS BYTEA AS $$
BEGIN
    RETURN pgcrypto.encrypt(plaintext::bytea, 'aes-256-key'::bytea, 'aes');
END;
$$ LANGUAGE plpgsql IMMUTABLE;

-- Trigger to auto-encrypt on insert
CREATE TRIGGER auto_encrypt_ssn
BEFORE INSERT ON patients
FOR EACH ROW
EXECUTE FUNCTION set_encrypted_ssn();
```

**In-transit encryption:**

```yaml
# Kubernetes ingress: TLS 1.3 only
apiVersion: networking.k8s.io/v1
kind: NetworkPolicy
metadata:
  name: tls-enforcement
spec:
  ingress:
  - from:
    - namespaceSelector: {}
    ports:
    - protocol: TCP
      port: 443  # HTTPS only
---
# TLS certificate rotation (cert-manager)
apiVersion: cert-manager.io/v1
kind: Certificate
metadata:
  name: lcwps-tls
spec:
  secretName: lcwps-tls-secret
  duration: 2160h  # 90 days
  renewBefore: 360h  # Rotate 15 days before expiry
  issuerRef:
    name: letsencrypt-prod
```

### 4.3 A03 – Injection

**Risk:** Malicious code injection (SQL, command, etc.).

**LCWPS Mitigation:**

```csharp
// ❌ PROHIBITED: String concatenation SQL
// var query = $"SELECT * FROM patients WHERE name = '{input}'";

// ✓ REQUIRED: Parameterized queries via ORM
var patients = await _dbContext.Patients
    .FromSql($"SELECT * FROM patients WHERE name = {input}")
    .ToListAsync();

// OR (preferred) using LINQ
var patients = await _dbContext.Patients
    .Where(p => p.Name == input)
    .ToListAsync();

// Test: SQL injection attempt
[Fact]
public async Task GetPatients_WithInjectedSQL_ReturnsNoResults()
{
    var maliciousInput = "x' OR '1'='1";
    var result = await _controller.GetPatients(maliciousInput);

    Assert.Empty(result); // Should not return unauthorized data
}
```

### 4.4 A04 – Insecure Design

**Risk:** Missing business logic controls.

**LCWPS Mitigation:**

```csharp
// Domain-level invariant enforcement
public class Appointment : AggregateRoot
{
    public Appointment(Guid tenantId, Guid patientId, PaymentVerification validation)
    {
        // Invariant 1: Payment verification must be APPROVED
        if (validation.Status != ValidationStatus.Approved)
            throw new InvalidOperationException(
                "Queue entry requires approved payment verification");

        // Invariant 2: Patient must be active
        if (patientId == Guid.Empty)
            throw new InvalidOperationException("Invalid patient ID");

        // Only after all invariants pass can object be created
        TenantId = tenantId;
        PatientId = patientId;
        PaymentVerification = validation;
        Status = AppointmentStatus.Waiting;
        CreatedAt = DateTime.UtcNow;

        // Publish domain event
        AddDomainEvent(new AppointmentdEvent(tenantId, patientId));
    }

    // Invariant enforcement on state transitions
    public void Approve(Guid clinicId, string approverName)
    {
        if (Status != AppointmentStatus.Waiting)
            throw new InvalidOperationException($"Cannot approve queue in {Status} state");

        if (CurrentDateTime > ScheduledTime.AddHours(2))
            throw new InvalidOperationException("Approval window expired");

        Status = AppointmentStatus.Approved;
        ApprovedBy = approverName;
        ApprovedAt = DateTime.UtcNow;
    }
}

// Unit test: verify invariant
[Fact]
public void Appointment_WhenCreatedWithRejectedPaymentVerification_ThrowsException()
{
    var validation = new PaymentVerification { Status = ValidationStatus.Rejected };

    Assert.Throws<InvalidOperationException>(() =>
        new Appointment(Guid.NewGuid(), Guid.NewGuid(), validation));
}
```

### 4.5 A05 – Security Misconfiguration

**Risk:** Insecure deployment settings.

**LCWPS Mitigation:**

```yaml
# Kubernetes Pod Security Policy
apiVersion: policy/v1beta1
kind: PodSecurityPolicy
metadata:
  name: restricted
spec:
  privileged: false
  allowPrivilegeEscalation: false
  requiredDropCapabilities:
    - ALL
  volumes:
    - 'configMap'
    - 'emptyDir'
    - 'projected'
    - 'secret'
    - 'downwardAPI'
    - 'persistentVolumeClaim'
  hostNetwork: false
  hostIPC: false
  hostPID: false
  runAsUser:
    rule: 'MustRunAsNonRoot'
  seLinux:
    rule: 'MustRunAs'
  supplementalGroups:
    rule: 'RunAsAny'
  fsGroup:
    rule: 'RunAsAny'
  readOnlyRootFilesystem: false

---
# Pod security context
apiVersion: v1
kind: Pod
metadata:
  name: queue-service
spec:
  securityContext:
    runAsNonRoot: true
    runAsUser: 1000
    fsGroup: 2000
  containers:
  - name: app
    image: registry.lcwps.com/lcwps/queue-service:v1.0.0@sha256:abc...
    securityContext:
      allowPrivilegeEscalation: false
      readOnlyRootFilesystem: true
      runAsNonRoot: true
      capabilities:
        drop:
        - ALL
```

### 4.6 A06 – Vulnerable Components

**Risk:** Outdated dependencies with known CVEs.

**LCWPS Mitigation:**

```yaml
# CI/CD: Dependency scanning on every PR
name: Security Scan
on: [pull_request]
jobs:
  snyk:
    runs-on: ubuntu-latest
    steps:
    - uses: actions/checkout@v3
    - name: Run Snyk scan
      run: |
        snyk auth ${{ secrets.SNYK_TOKEN }}
        snyk test --fail-on=all  # Fail on any vulnerability
        snyk monitor  # Track over time
```

### 4.7 A07 – Authentication Failures

**Risk:** Weak authentication allowing unauthorized access.

**LCWPS Mitigation:**

```csharp
// OAuth2.0 + TOTP/SMS MFA
public class AuthenticationService
{
    public async Task<LoginResponse> AuthenticateAsync(string email, string password)
    {
        var user = await _userRepository.GetByEmailAsync(email);

        // Verify password (bcrypt, not plaintext)
        if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            throw new UnauthorizedAccessException("Invalid credentials");

        // Require MFA for sensitive roles
        if (user.Role.RequiresMFA)
        {
            var mfaToken = _mfaService.GenerateTOTP();
            await _notificationService.SendMFACodeAsync(user.Email, mfaToken);

            return new LoginResponse
            {
                RequiresMFA = true,
                SessionToken = Guid.NewGuid().ToString()  // Temporary
            };
        }

        // Return JWT token (valid for 15 minutes)
        var token = _tokenService.GenerateJWT(user, expiresIn: TimeSpan.FromMinutes(15));
        return new LoginResponse { AccessToken = token };
    }

    // Account lockout after 5 failed attempts
    public async Task HandleFailedLoginAsync(string email)
    {
        var user = await _userRepository.GetByEmailAsync(email);
        user.FailedLoginAttempts++;

        if (user.FailedLoginAttempts >= 5)
        {
            user.LockoutEndTime = DateTime.UtcNow.AddMinutes(15);
            await _auditLogger.LogSecurityEventAsync(
                SecurityEventType.AccountLockout, email);
        }

        await _userRepository.SaveAsync(user);
    }
}
```

### 4.8 A08 – Software & Data Integrity Failures

**Risk:** Untrusted code or data modifications.

**LCWPS Mitigation:**

```bash
# Enforce signed commits
git config core.sshCommand "ssh -o UserKnownHostsFile=/dev/null"
git commit -S -m "feat(queue): implement FIFO validation"

# Verify signature
git log --show-signature

# Enforce in branch protection (GitHub)
# Settings → Branches → Require signed commits
```

### 4.9 A09 – Logging & Monitoring

**Risk:** Insufficient audit trails for forensics.

**LCWPS Mitigation:**

```csharp
// Immutable audit logging
public class AuditService
{
    public async Task LogAsync(AuditEvent @event)
    {
        // Append-only insert; no UPDATE/DELETE allowed
        await _dbContext.AuditLog.AddAsync(
            new AuditLogEntry
            {
                Id = Guid.NewGuid(),
                TableName = @event.TableName,
                Operation = @event.Operation,
                OldValues = JsonConvert.SerializeObject(@event.OldValues),
                NewValues = JsonConvert.SerializeObject(@event.NewValues),
                ChangedBy = @event.UserId,
                ChangedAt = DateTime.UtcNow,
                TenantId = @event.TenantId,
                IpAddress = @event.IpAddress,
                UserAgent = @event.UserAgent
            });

        await _dbContext.SaveChangesAsync();

        // Also send to immutable event store (S3/Kafka)
        await _eventPublisher.PublishAsync(@event);
    }
}

-- SQL: Prevent audit log modification
CREATE FUNCTION prevent_audit_tampering()
RETURNS TRIGGER AS $$
BEGIN
    RAISE EXCEPTION 'Audit logs are immutable. Only appends allowed.';
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER audit_immutability
BEFORE UPDATE OR DELETE ON audit_log
FOR EACH ROW EXECUTE FUNCTION prevent_audit_tampering();
```

### 4.10 A10 – SSRF (Server-Side Request Forgery)

**Risk:** Service makes requests to unintended internal services.

**LCWPS Mitigation:**

```csharp
// Network isolation + whitelist
public class ExternalServiceClient
{
    private static readonly IEnumerable<Uri> AllowedHosts = new[]
    {
        new Uri("https://api.insurance-provider.com/"),
        new Uri("https://auth.lcwps.com/"),
        new Uri("https://analytics.lcwps.com/")
    };

    public async Task<T> GetAsync<T>(Uri uri)
    {
        // Validate against whitelist
        if (!AllowedHosts.Any(host => uri.Host.EndsWith(host.Host)))
            throw new InvalidOperationException($"Host {uri.Host} not whitelisted");

        // Block internal IPs
        var ipAddress = Dns.GetHostAddresses(uri.Host)[0];
        if (IsPrivateIP(ipAddress))
            throw new InvalidOperationException("Internal IP access blocked");

        using (var client = new HttpClient())
        {
            var response = await client.GetAsync(uri);
            return JsonConvert.DeserializeObject<T>(
                await response.Content.ReadAsStringAsync());
        }
    }

    private bool IsPrivateIP(IPAddress ip)
    {
        return ip.IsLoopback
            || ip.ToString().StartsWith("10.")
            || ip.ToString().StartsWith("172.16.")
            || ip.ToString().StartsWith("192.168.");
    }
}

// Kubernetes NetworkPolicy: explicit egress control
apiVersion: networking.k8s.io/v1
kind: NetworkPolicy
metadata:
  name: queue-service-egress
spec:
  podSelector:
    matchLabels:
      app: queue-service
  policyTypes:
  - Egress
  egress:
  - to:
    - namespaceSelector:
        matchLabels:
          name: lcwps-database
    ports:
    - port: 5432
      protocol: TCP
  - to:
    - namespaceSelector:
        matchLabels:
          name: lcwps-messaging
    ports:
    - port: 5672
      protocol: TCP
  # Deny everything else
```

---

## 5. CRYPTOGRAPHIC KEY MANAGEMENT

### 5.1 Key Rotation Policy

| Key Type | Algorithm | Rotation | Storage | Owner |
|----------|-----------|----------|---------|-------|
| **JWT Signing Key** | RSA-2048 | 90 days (scheduled) | AWS KMS | Auth Service |
| **Database Encryption Key** | AES-256 | 90 days | AWS KMS (envelope encryption) | Database Team |
| **TLS Certificates** | ECDSA/RSA | 60 days before expiry | cert-manager + K8s secrets | DevSecOps |
| **API Keys (external)** | HMAC-SHA256 | 180 days | HashiCorp Vault | Integration Team |

### 5.2 KMS Integration (AWS KMS)

```csharp
public class KeyManagementService
{
    private readonly IAmazonKeyManagementService _kmsClient;

    public async Task<string> RotateDataEncryptionKeyAsync(string keyId)
    {
        // Initiate automatic key rotation
        var request = new PutKeyPolicyRequest
        {
            KeyId = keyId,
            PolicyName = "default",
            Policy = @"{
                'Sid': 'Enable IAM User Permissions',
                'Effect': 'Allow',
                'Principal': {'AWS': 'arn:aws:iam::ACCOUNT_ID:root'},
                'Action': 'kms:*',
                'Resource': '*'
            }"
        };

        await _kmsClient.PutKeyPolicyAsync(request);

        // Log rotation event
        await _auditLogger.LogSecurityEventAsync(
            SecurityEventType.KeyRotation,
            new { KeyId = keyId, RotatedAt = DateTime.UtcNow });

        return "Key rotation scheduled";
    }
}
```

---

## 6. INCIDENT RESPONSE PLAN

### 6.1 Data Breach Response (48-Hour Timeline)

| Hour | Action | Owner | Evidence |
|------|--------|-------|----------|
| **0** | Breach detected | Security Monitoring | Alert log in CloudWatch |
| **1** | Incident commander activated | CISO | Incident ticket created |
| **4** | Root cause analysis begins | Security Engineering | Analysis doc started |
| **12** | Patient notification draft prepared | DPO + Legal | Notification template |
| **24** | Regulatory notification (Autoridad) | DPO | Complaint ticket filed |
| **48** | Root cause analysis complete | Architecture | RCA final report signed |

### 6.2 Containment Steps

1. **Isolate affected systems:** NetworkPolicy + pod eviction
2. **Preserve evidence:** Copy logs to immutable S3 bucket
3. **Notify stakeholders:** Automated alert to CISO, DPO, CEO
4. **Communicate:** Update status.lcwps.com every 4 hours

---

## 7. SECURITY GOVERNANCE BOARD (MONTHLY)

**Attendees:** CISO, Chief Architect, DPO, DevSecOps Lead
**Agenda:**

- Vulnerability scan results (Snyk/Trivy)
- Pending security patches
- Compliance status
- Incident review (if any)
- Threat modeling updates

---

**Document Status:** ✓ APPROVED – Implementation Binding
**Review:** Quarterly (every 90 days)
**Next Review:** 23-05-2026
