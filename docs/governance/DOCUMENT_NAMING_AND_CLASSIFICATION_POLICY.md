# DOCUMENT NAMING AND CLASSIFICATION POLICY

## Enterprise Documentation Governance Standard

**Authority:** Chief Enterprise Architect, Board of Directors, CISO, DPO
**Document ID:** DNCP-001-2026
**Date Effective:** 2026-02-23
**Classification:** CONFIDENTIAL - Governance Restricted
**Status:** ✓ BINDING & MANDATORY
**Version:** 1.0
**Scope:** All documentation under `/docs` directory and all governance artifacts
**Language:** English (normative); Spanish provided for reference only

---

## EXECUTIVE SUMMARY

This policy establishes the mandatory, non-negotiable standards for naming, classifying, organizing, and maintaining all enterprise governance documentation for LCWPS. Compliance is required for all contributors, teams, and governance authorities.

**Key Principles:**

- Single source of truth (no duplicates or redirects)
- Zero ambiguity in naming and structure
- Deterministic organization for 100+ documents
- Full traceability and audit capability
- ISO 27001 Document Control alignment

---

## 1. GOVERNANCE AUTHORITY

### 1.1 Governing Bodies

| Authority | Responsibility |
|-----------|-----------------|
| **Board of Directors** | Annual policy review and approval |
| **Chief Enterprise Architect** | Architecture & ADR governance |
| **CISO** | Security classification and access control |
| **DPO** | Regulatory compliance and data governance |
| **CTO** | Technology documentation standards |
| **Chief DevSecOps Officer** | Operations and CI/CD documentation |
| **Compliance Officer** | Policy enforcement and audit trail |

### 1.2 Policy Amendment Process

| Step | Owner | Timeline | Approval |
|------|-------|----------|----------|
| **1. Proposal** | Any governance authority | — | Chief Enterprise Architect |
| **2. Stakeholder Review** | CISO, DPO, CTO | 5 business days | 2-person control |
| **3. Board Approval** | Board of Directors | 10 business days | ≥2 votes required |
| **4. Implementation** | Documentation Team | Upon approval | No delay permitted |
| **5. Enforcement** | Compliance Officer | Immediate | CI/CD gate enforcement |

---

## 2. FOLDER NAMING STANDARDS

### 2.1 Folder Naming Rules (MANDATORY)

**Format:** `lowercase-kebab-case-singular`

**Rules:**

1. All lowercase (a-z, 0-9 only)
2. Kebab-case (hyphens, NOT underscores)
3. Singular noun (not plural)
4. NO numeric prefixes
5. NO spaces
6. Semantic, human-readable name
7. Maximum 50 characters
8. NO special characters except hyphens

**Valid Examples:**

```
governance/
architecture/
security/
regulatory/
operations/
git/
ai-generated/
```

**Invalid Examples (PROHIBITED):**

```
❌ Governance/ (capitalized)
❌ governances/ (plural)
❌ 01_governance/ (numeric prefix)
❌ governance_items/ (underscore)
❌ Gov (not semantic)
❌ governance-baseline/ (should be at file level)
```

### 2.2 Nested Folder Rules

**Requirement:** Maximum **2 levels deep**

**Permitted Nesting:**

```
docs/
  domain/
    subdomain/  ← MAX 2 levels
      files.md
```

**Prohibited Nesting:**

```
❌ docs/domain/subdomain/subsubdomain/  ← Too deep
❌ docs/governance/operations/  ← Domain mixing (operations ≠ governing governance)
❌ Cross-domain nesting
```

### 2.3 Mandatory Folder Structure

**EXACT Required Structure (deterministic):**

```
docs/
├── governance/
│   ├── [normative documents]
│   └── [NEW POLICY DOCUMENTS]
├── architecture/
│   └── adr/
│       ├── [ADR files]
│       └── [ADR-TEMPLATE.md]
├── security/
│   └── [security documents]
├── regulatory/
│   └── [regulatory documents]
├── operations/
│   └── [operations documents]
├── git/
│   └── [git governance]
└── ai-generated/
    ├── prompts/
    │   └── [YYYY-MM-DD-*.md]
    └── logs/
        └── [YYYY-MM-DD-*.md]
```

**No deviations permitted.** Each domain is a peer under `/docs`. Operational subdomain (`ai-generated/prompts/`, `ai-generated/logs/`) is permitted only under `ai-generated/`.

---

## 3. FILE NAMING STANDARDS

### 3.1 Normative Document Files

**Format:** `UPPERCASE_UNDERSCORE_NAMING.md`

**Rules:**

1. UPPERCASE letters only (A-Z)
2. Underscore separator (NOT hyphens or spaces)
3. NO numeric prefixes or inline numbers
4. Semantic, complete English noun phrases
5. Maximum 80 characters
6. NO special characters except underscores
7. Always `.md` extension

**Valid Examples:**

```
GOVERNANCE_INDEX.md
ENTERPRISE_GOVERNANCE_BASELINE.md
BUSINESS_RULES.md
SECURITY_GOVERNANCE.md
FUNCTIONAL_REQUIREMENTS.md
DOCUMENT_NAMING_AND_CLASSIFICATION_POLICY.md
```

**Invalid Examples (PROHIBITED):**

```
❌ governance_index.md (lowercase)
❌ Governance-Index.md (mixed case, hyphens)
❌ 00-GOVERNANCE_INDEX.md (numeric prefix)
❌ governance index.md (spaces)
❌ GOVERNANCE_INDEX (missing .md)
❌ BZ-001_governance (numeric + wrong format)
```

### 3.2 Architecture Decision Record (ADR) Files

**Format:** `ADR-XXXX-DECISION_TITLE.md`

**Rules:**

1. Prefix: `ADR-` (always)
2. Sequence: `XXXX` (4-digit zero-padded, starting from 0001)
3. Title: `DECISION_TITLE` (UPPERCASE_UNDERSCORE)
4. Sequential numbering is **MANDATORY** for every ADR
5. No gaps in numbering
6. ADR numbering is global (not per project/service)
7. Only actual ADRs are numbered; templates are excluded
8. Each ADR is immutable (cannot be renumbered)

**Valid Examples:**

```
ADR-0001-MICROSERVICES_EVENT_DRIVEN.md
ADR-0002-SECURITY_ZERO_TRUST_ARCHITECTURE.md
ADR-0003-POSTGRESQL_DATA_PERSISTENCE.md
```

**Invalid Examples (PROHIBITED):**

```
❌ ADR-001-MICROSERVICES.md (3-digit)
❌ ADR-01-MICROSERVICES.md (2-digit)
❌ adr-0001-microservices.md (lowercase)
❌ ADR-0001-Microservices.md (mixed case)
❌ ADR-1-MICROSERVICES.md (no zero-padding)
❌ 0001-MICROSERVICES.md (ADR- prefix missing)
❌ ADR-0005-NEXT.md followed by ADR-0001-FIRST.md (non-sequential)
```

**Special Case — ADR-TEMPLATE.md:**

The template file is **NOT numbered** because it is not an actual decision record:

```
ADR-TEMPLATE.md ✓ (Correct - no numbering)
❌ ADR-0999-TEMPLATE.md (Incorrect - templates not numbered)
```

### 3.3 AI-Generated Log Files

**Format:** `YYYY-MM-DD-log-type.md`

**Rules:**

1. ISO 8601 date format (YYYY-MM-DD) at start
2. Hyphen separator only
3. Log type suffix: `-prompt-context`, `-change-summary`, `-execution-log`
4. lowercase log type
5. All lowercase filename
6. Maximum 100 characters

**Valid Examples:**

```
2026-02-23-microservices-decision-prompt-context.md
2026-02-23-security-review-change-summary.md
2026-02-21-api-design-execution-log.md
```

**Invalid Examples (PROHIBITED):**

```
❌ 2026_02_23-prompt.md (underscores in date)
❌ 02-23-2026-prompt.md (wrong date format)
❌ 2026-02-23-PROMPT_CONTEXT.md (uppercase)
❌ prompt-2026-02-23.md (date not at start)
```

### 3.4 Synthetic Examples by Policy

| Document Type | Folder | File Name | Status |
|---|---|---|---|
| Governance Policy | governance/ | DOCUMENT_NAMING_POLICY.md | ✓ Correct |
| Business Rules | governance/ | BUSINESS_RULES.md | ✓ Correct |
| Architecture | architecture/ | ARCHITECTURE.md | ✓ Correct |
| ADR #1 | architecture/adr/ | ADR-0001-MICROSERVICES_EVENT_DRIVEN.md | ✓ Correct |
| ADR Template | architecture/adr/ | ADR-TEMPLATE.md | ✓ Correct |
| Security Policy | security/ | SECURITY_GOVERNANCE.md | ✓ Correct |
| Quality Governance | operations/ | QA_GOVERNANCE.md | ✓ Correct |
| **VIOLATION** | governance/ | **00-GOVERNANCE_INDEX.md** | ✗ **REMOVED** |
| **VIOLATION** | governance/ | **governance/operations/07-QA_GOVERNANCE.md** | ✗ **REMOVED** |
| AI Prompt Log | ai-generated/prompts/ | 2026-02-23-business-logic-analysis-prompt.md | ✓ Correct |

---

## 4. DOCUMENT CLASSIFICATION

### 4.1 Classification Domains

**Every document MUST be classified into exactly one domain:**

| Domain | Purpose | Subfolder | Examples |
|--------|---------|-----------|----------|
| **governance** | Authority, business rules, policies, requirements | No subfolder; ADR-specific items belong in architecture/ | ENTERPRISE_GOVERNANCE_BASELINE.md, BUSINESS_RULES.md, AI_GOVERNANCE.md |
| **architecture** | Technical baseline, design patterns, ADR records | `adr/` subfolder for numbered decisions | ARCHITECTURE.md, ADR-0001-*.md, ADR-TEMPLATE.md |
| **security** | Security controls, threat models, cryptography | No subfolder | SECURITY_GOVERNANCE.md |
| **regulatory** | Compliance, audit, legal traceability | No subfolder | REGULATORY_TRACEABILITY_MATRIX.md |
| **operations** | SLAs, monitoring, incident response, QA, DevSecOps | No subfolder | QA_GOVERNANCE.md, DEVSECOPS_GOVERNANCE.md, OPERATIONS_GOVERNANCE.md |
| **git** | Version control, branching, release policy | No subfolder | GIT_GOVERNANCE.md |
| **ai-generated** | Archive of AI-generated prompts and logs only | `prompts/` and `logs/` subfolders | 2026-02-23-prompt-context.md |

### 4.2 Classification Rules (MANDATORY)

1. **Single Domain Assignment:** No document can belong to multiple domains
2. **No Cross-Domain Nesting:** Operations docs do NOT nest under governance
3. **Hierarchical Integrity:** ADRs belong ONLY in architecture/adr/
4. **Clear Boundaries:**
   - Governance = Business rules, policies, requirements, authority
   - Architecture = Technical decisions, patterns, baselines
   - Security = Controls and threat analysis
   - Regulatory = Compliance mapping and evidence
   - Operations = Runbooks, SLAs, QA, DevSecOps
   - Git = Source control governance
   - AI-Generated = Prompt archives and execution logs
5. **No Orphaned Documents:** Every file must be in the correct domain folder

### 4.3 Misclassification Examples

| Document | Incorrect Classification | Correct Classification | Reason |
|----------|--------------------------|----------------------|--------|
| Payment verification workflow | security/ | governance/ | Business rule, not security threat |
| RBAC policy | governance/ | security/ | Access control is security domain |
| API versioning strategy | operations/ | architecture/ | Technical decision = ADR candidate |
| Test coverage targets | security/ | operations/ | QA metric, not security control |
| SAML integration | operations/ | security/ | Authentication is security |

---

## 5. CROSS-REFERENCE POLICY

### 5.1 Internal Reference Format (MANDATORY)

**All markdown links MUST use relative paths:**

**Valid Examples:**

```markdown
[Referenced Document](../architecture/ARCHITECTURE.md)
[ADR Decision](./adr/ADR-0001-MICROSERVICES_EVENT_DRIVEN.md)
[Section Link](../operations/QA_GOVERNANCE.md#L23)
[Same Folder](./BUSINESS_RULES.md)
```

**Invalid Examples (PROHIBITED):**

```markdown
❌ [Document](/docs/architecture/ARCHITECTURE.md) [absolute path]
❌ [Document](file:///docs/...) [file URI]
❌ [Document](vscode://...) [VS Code URI]
❌ [Document](docs/architecture/ARCHITECTURE.md) [from workspace root when file is nested]
```

### 5.2 Reference Update Policy

**When a document is renamed or relocated:**

1. ALL inbound references MUST be updated within 24 hours
2. A cross-reference audit MUST be performed
3. Broken link detection tool MUST be run
4. No merged code contains broken references

**When a document is deprecated:**

1. Create NO legacy redirects (single source of truth)
2. Update all inbound references immediately
3. Document removal in commit message
4. Archive in version control (Git history preserves)

### 5.3 Forbidden Reference Patterns

**These reference patterns are PROHIBITED:**

❌ Implicit assumptions (e.g., "see the security document")
❌ Hyperlinks to external versions not in repository
❌ Hard-coded absolute paths
❌ Circular dependencies (A→B→A)
❌ References to deprecated files
❌ Comments instead of markdown links

---

## 6. DOCUMENT LIFECYCLE POLICY

### 6.1 Document Lifecycle Stages

```
DRAFT → REVIEW → APPROVED → ACTIVE → DEPRECATED → ARCHIVED
```

| Stage | Definition | Duration | Action |
|-------|----------|----------|--------|
| **DRAFT** | Initial creation, not yet reviewed | ≤ 14 days | Internal team review |
| **REVIEW** | Submitted to governance authority | ≤ 10 days | 2-person approval required |
| **APPROVED** | Authorized; ready for use | — | Merge to main branch |
| **ACTIVE** | In force; binding standard | 12 months | Quarterly review cycle |
| **DEPRECATED** | Superseded but retained for audit | 12 months | Marked in document header |
| **ARCHIVED** | Moved to historical record | Permanent | Immutable in Git history |

### 6.2 Review Cadence (MANDATORY)

**Every document MUST be reviewed on this schedule:**

| Document | Frequency | Owner | Evidence |
|----------|-----------|-------|----------|
| ENTERPRISE_GOVERNANCE_BASELINE | Annual | Board + CISO + DPO | Signed board minutes |
| ARCHITECTURE | Quarterly | Chief Architect + CTO | Review audit trail |
| ADR documents | Per decision | Architecture Review Board | ADR status field |
| BUSINESS_RULES | Quarterly | Product Owner + Domain Expert | Approval ticket |
| FUNCTIONAL_REQUIREMENTS | Quarterly | Product Owner + Analyst | Change log |
| SECURITY_GOVERNANCE | Quarterly | CISO | Vulnerability correlation |
| REGULATORY_TRACEABILITY_MATRIX | Annual | DPO + Compliance | Audit report |
| DEVSECOPS_GOVERNANCE | Monthly | Chief DevSecOps Officer | Incident correlation |
| OPERATIONS_GOVERNANCE | Monthly | COO + DevOps Lead | Metric correlation |
| QA_GOVERNANCE | Quarterly | QA Lead | Defect correlation |
| RISK_MANAGEMENT_FRAMEWORK | Monthly | Chief Risk Officer | Risk log update |
| GIT_GOVERNANCE | Quarterly | CTO + DevOps Lead | Audit log review |
| DOCUMENT_NAMING_POLICY | Annual | Chief Enterprise Architect + Board | Policy review minutes |

### 6.3 Version Control in Document Headers

**Every document MUST contain:**

```markdown
# DOCUMENT TITLE

**Authority:** [Responsible authority]
**Document ID:** [DOMAIN-NNN-YYYY] (e.g., SEC-001-2026)
**Date Effective:** YYYY-MM-DD
**Classification:** [Level]
**Status:** [DRAFT | REVIEW | APPROVED | ACTIVE | DEPRECATED]
**Version:** X.Y (Major.Minor)
**Last Review:** YYYY-MM-DD
**Next Review:** YYYY-MM-DD
```

---

## 7. VERSION CONTROL POLICY

### 7.1 Git Commit Standards for Documentation

**Commit Format:**

```
docs(<domain>): <action> <document>

<description>

Fixes: #<issue-number>
Document-ID: <ID>
Authority-Approval: <reviewer>
```

**Valid Examples:**

```
docs(governance): add BUSINESS_RULES normative document

- Defines appointment state machine
- Establishes queue FIFO rules
- Authority: Product Owner approved 2026-02-23

Fixes: #42
Document-ID: GOV-001-2026
Authority-Approval: chief-architect
```

**Invalid Examples (PROHIBITED):**

```
❌ docs: update stuff
❌ update documentation
❌ fix naming
❌ [no commit message]
```

### 7.2 Branch Protection for Documentation

**All documentation changes MUST:**

1. Be made in a feature branch: `docs/feature-name`
2. Require 2 reviewers (no self-approval)
3. Pass CI/CD checks:
   - Markdown linting
   - Broken link detection
   - Naming convention validation
4. Reference governance ticket/issue

### 7.3 Pull Request Requirements

**Every documentation PR MUST include:**

- Title: `docs(<domain>): description`
- Linked governance issue (not optional)
- List of modified files
- Cross-reference audit (if renamed)
- Approval from domain authority
- No unrelated code changes

---

## 8. CHANGE APPROVAL REQUIREMENTS

### 8.1 Approval Matrix

| Change Type | Authority | Approval Required | Timeline |
|------------|-----------|------------------|----------|
| **Normative Policy (Governance)** | Board | ≥2 votes | 10 days |
| **Architecture Decision (ADR)** | Chief Architect | Architecture Review Board | 5 days |
| **Security Policy** | CISO | CISO + Risk Officer | 3 days |
| **Regulatory Update** | DPO | DPO + Compliance | 5 days |
| **Operations Procedure** | COO/DevOps Lead | Peer review + authority | 2 days |
| **New ADR Entry** | Chief Architect | ARB + CTO | 7 days |
| **Naming Correction** | Chief Architect | Compliance Officer | 1 day |
| **Typo/Grammar Fixes** | Document Owner | Self (logged in PR) | Same day |

### 8.2 Approval Workflow

```
CHANGE REQUEST
     ↓
DOMAIN AUTHORITY REVIEW (5 days max)
     ↓
CROSS-FUNCTIONAL STAKEHOLDER CHECK (if multi-domain)
     ↓
APPROVAL OR REJECTION
     ↓
IF APPROVED: Merge to main branch (immediate)
IF REJECTED: Return for revision
```

### 8.3 Two-Person Control (MANDATORY for critical docs)

**These documents require 2-person approval before merge:**

1. ENTERPRISE_GOVERNANCE_BASELINE
2. ARCHITECTURE
3. SECURITY_GOVERNANCE
4. All ADR documents (status change)
5. REGULATORY_TRACEABILITY_MATRIX
6. This policy document itself

**Rule:** Author ≠ Approver (strict separation of duties)

---

## 9. ENFORCEMENT MECHANISM

### 9.1 Automated Enforcement (CI/CD Gates)

**All commits to `/docs` MUST pass:**

1. **Markdown Linting**
   - Tool: markdownlint
   - Config: `.markdownlintrc`
   - Failure: Blocks merge

2. **Naming Convention Validator**
   - Tool: Custom GitHub Actions script
   - Checks: File names, folder names, ADR numbering
   - Failure: Blocks merge

3. **Cross-Reference Link Validator**
   - Tool: markdown-link-check
   - Config: `.markdown-link-check.json`
   - Failure: Blocks merge

4. **Conflict Detection**
   - Check: No duplicate domain nesting
   - Check: No numeric prefixes
   - Check: No deprecated files
   - Failure: Blocks merge

**Implementation:**

```yaml
# .github/workflows/docs-enforcement.yml
name: Documentation Governance Enforcer
on: [pull_request]
jobs:
  validate:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3
      - name: Validate Naming Convention
        run: ./scripts/validate-doc-naming.sh docs/
      - name: Check Links
        run: markdown-link-check --config .markdown-link-check.json 'docs/**/*.md'
      - name: Lint Markdown
        run: markdownlint 'docs/**/*.md'
```

### 9.2 Manual Enforcement (Human Review)

**Compliance Officer responsibilities:**

1. Monthly audit of `/docs` directory
2. Verify all documents contain required headers
3. Confirm all cross-references are valid
4. Check review cadence compliance
5. Report violating documents to Board
6. Recommend remediation within 30 days

### 9.3 Violation Consequences

| Violation Severity | Consequence | Timeline |
|-------------------|----------|----------|
| **Critical** (numeric prefix, domain mixing) | Merge blocked; PR requires resubmission | Immediate |
| **High** (missing authority header, broken link) | Merge blocked until fixed | 24 hours |
| **Medium** (outdated review date, wrong classifier) | Merge allowed with warning; fix within 5 days | 5 days |
| **Low** (typos, formatting inconsistency) | Merge allowed; fix in next revision cycle | 30 days |

---

## 10. GOVERNANCE AUTHORITY ASSIGNMENT

### 10.1 Role-Based Responsibilities

| Role | Authority | Duty | Escalation |
|------|-----------|------|-----------|
| **Chief Enterprise Architect** | ADR governance, architecture baseline | Approve all structure changes, ADR numbering | Board |
| **CISO** | Security classification, threat analysis | Approve security docs, classify info levels | Board |
| **DPO** | Regulatory compliance, data protection | Approve regulatory docs, audit trails | Board |
| **CTO** | Technology standards, API documentation | Approve architecture patterns, AI governance | Chief Architect |
| **Chief DevSecOps Officer** | Operations automation, security gates | Approve DevOps/QA/CI-CD docs | CISO + CTO |
| **Compliance Officer** | Policy enforcement, violations | Monthly audit, violation reporting | Board |
| **Product Owner** | Business rules, functional requirements | Approve domain model, business logic docs | Chief Architect |
| **Documentation Team Lead** | Operational maintenance | File organization, CI/CD updates, link validation | Chief Architect |

### 10.2 Escalation Matrix

**Conflict Resolution Escalation:**

```
Document Disagreement
       ↓
Domain Authority (5 day review)
       ↓
If unresolved:
       ↓
Chief Enterprise Architect + CISO (3 day joint review)
       ↓
If unresolved:
       ↓
Board of Directors (final authority)
```

---

## 11. POLICY COMPLIANCE CHECKLIST

**Use this checklist for every documentation submission:**

- [ ] File name follows UPPERCASE_UNDERSCORE or ADR-XXXX-TITLE format
- [ ] File location is in correct domain folder
- [ ] No numeric prefixes (00-, 07-, etc.)
- [ ] No spaces in file names
- [ ] Folder structure is ≤2 levels deep
- [ ] Document header contains all required fields (Authority, Classification, Status, Date, Version)
- [ ] All cross-references use relative markdown links
- [ ] No references to deprecated documents
- [ ] Markdown passes linter (markdownlint)
- [ ] All links are valid (markdown-link-check)
- [ ] ADRs follow sequential numbering (if applicable)
- [ ] Review date is current (within review cadence)
- [ ] Authority approval is documented (PR approval + comment)
- [ ] 2-person approval obtained (if critical document)
- [ ] Commit message follows specification
- [ ] No deprecated redirect files
- [ ] No domain mixing (e.g., operations docs not under governance)

---

## 12. EFFECTIVE DATE & TRANSITION

**Effective Date:** 2026-02-23
**Retroactive Application:** Yes (all existing documents must comply)

**Transition Timeline:**

- **2026-02-23:** Policy approved; all violations removed
- **2026-03-01:** First enforcement checkpoint (CI/CD enforcement active)
- **2026-03-31:** Compliance Officer first audit
- **2026-04-30:** Board review of compliance status

**Legacy Document Handling:**

- All deprecated redirect files (00-*, 07-*) have been removed
- Orphan folders (governance/operations/) have been removed
- All remaining documents conform to this policy
- Zero legacy debt remains

---

## 13. APPENDIX: RESERVED FILE NAMES

**These file names are RESERVED and cannot be reused:**

| Reserved Name | Purpose | Domain |
|---|---|---|
| `ADR-TEMPLATE.md` | Template for new ADR documents | architecture/adr/ |
| `GOVERNANCE_INDEX.md` | Master index of all governance documents | governance/ |
| `README.md` | Repository root documentation (if applicable) | docs/ root |

**All other file names are available for new documents.**

---

## 14. POLICY REVIEW & AMENDMENT

**Next Scheduled Review:** 2027-02-23 (annual)

**Amendment Process:** See Section 1.2

**Policy Owner:** Chief Enterprise Architect
**Contact:** [Chief Architect Email]
**Questions/Clarifications:** Submit via governance ticket system

---

**END OF POLICY**

---

## SIGNATURE PAGE

| Role | Name | Signature | Date |
|------|------|-----------|------|
| Chief Enterprise Architect | — | — | 2026-02-23 |
| CISO | — | — | 2026-02-23 |
| DPO | — | — | 2026-02-23 |
| Board Approver | — | — | 2026-02-23 |

**This policy is BINDING and NON-NEGOTIABLE.**
**All contributors must acknowledge and comply with this policy before submitting documentation.**
