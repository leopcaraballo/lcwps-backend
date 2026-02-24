# ADR TEMPLATE – LCWPS

## Architecture Decision Record Template

---

# ADR-NNNN: [Concise Architectural Decision Title]

**Date:** YYYY-MM-DD
**Status:** [Proposed | Accepted | Deprecated | Superseded by ADR-NNNN]
**Architect:** [Name + Role]
**Review Board:** [ARB approval required]

---

## 1. Context

### Problem Statement

- What is the business/technical problem?
- What constraints exist?
- Why is this decision needed now?

### Current State

- What are we replacing?
- Why is the current state insufficient?

### Stakeholders

- Product Owner
- Technical Team
- Operations
- Security
- Compliance
- Others

---

## 2. Architectural Decision

### Selected Architecture

**Decision:** [Concise statement of what was decided]

**Rationale:**

- Why this decision over alternatives?
- How does it address the problem?
- What principles guide this choice?

### Implementation Strategy

[High-level steps for implementation]

### Technology Stack

| Component | Technology | Version | Binding |
|-----------|-----------|---------|--------|
| | | | MANDATORY/RECOMMENDED |

---

## 3. Consequences

### Positive Consequences

- [Benefit 1]
- [Benefit 2]
- [Benefit 3]

### Negative Consequences

- [Trade-off 1]
- [Trade-off 2]
- [Mitigation strategy]

### Long-term Impact

- Scalability
- Maintainability
- Time-to-market
- Future decisions

---

## 4. Alternatives Considered (Rejected)

### Alternative A: [Name]

**Approach:** [Description]

**Rejection Rationale:**

- [Why not A?]

### Alternative B: [Name]

**Approach:** [Description]

**Rejection Rationale:**

- [Why not B?]

---

## 5. Regulatory and Compliance Alignment

- Ley 1581 (Data Protection): [Complies / Requires plan]
- Ley 23 (Healthcare): [Complies / Requires plan]
- ISO 27001: [Aligned / Requires controls]
- OWASP Top 10: [Mapping to applicable risks]

---

## 6. Risk Assessment

| Risk | Likelihood | Impact | Mitigation |
|------|-----------|--------|----------|
| [Risk 1] | Low/Med/High | Low/Med/High | [Approach] |

---

## 7. Validation and Testing Strategy

- Unit tests
- Integration tests
- Load/performance testing
- Security testing
- Chaos engineering

---

## 8. Rollback Strategy

- Complexity level (Low/Medium/High)
- Estimated rollback effort
- Data migration concerns🎯 ROLE (MANDATORY)

Act as:
Enterprise Documentation Governance Architect
ISO 27001 Document Control Specialist
Healthcare Regulatory Compliance Structuring Lead
Repository Standardization Authority

You are NOT a coding assistant.
You are NOT making suggestions.
You are executing a mandatory enterprise documentation normalization directive.

Zero ambiguity.
Zero partial execution.
Zero skipped files.
Zero optional behavior.

------------------------------------------------------------

📌 SCOPE

Scan and analyze ALL files inside:

/docs

Recursively.

This includes all subdirectories and markdown files.

------------------------------------------------------------

🎯 OBJECTIVES

You MUST:

1. Read every document.
2. Identify its purpose, scope, and governance domain.
3. Classify each document into one of the following domains:

   - governance
   - architecture
   - security
   - regulatory
   - operations
   - git
   - ai-generated
   - adr

4. Detect:
   - Duplicate content
   - Overlapping documents
   - Broken cross-references
   - Inconsistent terminology
   - Naming inconsistencies
   - Numeric prefixes
   - Non-enterprise naming

5. Normalize file names using a strict enterprise naming convention.
6. Update ALL internal references to match new file names.
7. Ensure no document loses traceability.
8. Preserve all content (do NOT delete content).
9. Ensure deterministic structure.
10. Ensure scalability for 100+ documents.

------------------------------------------------------------

📁 NAMING CONVENTION (MANDATORY)

Folder Naming:
- lowercase
- singular
- kebab-case
- no numeric prefixes

File Naming:

A) Normative Documents:
- UPPERCASE
- underscore separator
- no spaces
- no numeric prefix
- semantic naming only

Example:
ENTERPRISE_GOVERNANCE_BASELINE.md
BUSINESS_RULES.md
SECURITY_GOVERNANCE.md

B) ADR Files:
ADR-XXXX-TITLE.md
Sequential numbering mandatory.
4-digit zero-padded.
Example:
ADR-0001-MICROSERVICES_EVENT_DRIVEN.md

C) AI-Generated Logs:
YYYY-MM-DD-change-summary.md
YYYY-MM-DD-prompt-context.md
ISO date required.

------------------------------------------------------------

📌 STRUCTURE TARGET

/docs
    governance/
    architecture/
        adr/
    security/
    regulatory/
    operations/
    git/
    ai-generated/

No governance mixing.
No cross-domain nesting.
No orphan folders.

------------------------------------------------------------

📌 CROSS-REFERENCE ENFORCEMENT

You MUST:

- Update all markdown links.
- Update references to renamed files.
- Ensure internal references are consistent.
- Ensure no broken relative paths exist.

------------------------------------------------------------

📌 OUTPUT REQUIRED

You MUST produce:

1. Full inventory of current documents (before normalization).
2. Classification table (Document → Domain).
3. Renaming mapping table (Old Name → New Name).
4. Structural relocation mapping.
5. Updated tree structure.
6. Required git mv command list in correct execution order.
7. Confirmation that:
   - No numeric prefixes remain.
   - No duplicate domain nesting remains.
   - No broken references remain.
   - All documents conform to enterprise naming.

------------------------------------------------------------

📌 ADDITIONAL MANDATORY TASK

Create a new file:

/docs/governance/DOCUMENT_NAMING_AND_CLASSIFICATION_POLICY.md

This file MUST define:

1. Folder naming rules
2. File naming rules
3. ADR numbering policy
4. AI log naming policy
5. Cross-reference policy
6. Document lifecycle policy
7. Version control policy for documentation
8. Change approval requirement
9. Enforcement mechanism
10. Governance authority responsible

The policy must be formal, audit-ready, ISO-aligned.

------------------------------------------------------------

🚫 PROHIBITED

Do NOT:
- Ask clarifying questions.
- Suggest alternatives.
- Modify document meaning.
- Delete content.
- Skip any file.
- Produce partial output.

------------------------------------------------------------

🎯 EXECUTION MODE

This is a mandatory enterprise governance normalization directive.

Execute completely.
- Fallback approach

---

## 9. Architectural Review Schedule

| Milestone | Review | Decision |
|-----------|--------|----------|
| [Date] | [Phase/Gate] | [Approval/Rejection/Defer] |

---

## 10. Related ADRs

- ADR-NNNN: [Related decision]

---

## 11. Approval Signatures

| Role | Name | Signature | Date |
|------|------|-----------|------|
| CTO | | _____________ | |
| Chief Architect | | _____________ | |
| CISO (if security) | | _____________ | |
| DPO (if data/privacy) | | _____________ | |

---

**Document Status:** [APPROVED / PENDING / REJECTED]
**Review Interval:** [Timing for re-evaluation]
**Next Review Date:** YYYY-MM-DD
