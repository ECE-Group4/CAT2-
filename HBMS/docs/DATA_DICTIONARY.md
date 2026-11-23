# Data Dictionary

Access SQL type conventions: `AUTOINCREMENT` primary key, `LONG` foreign key, `TEXT(n)` bounded text, `MEMO` long text, `DATETIME` timestamp, `CURRENCY` monetary value, `YESNO` boolean, `DOUBLE` fractional hours.

| Table | Key fields | Purpose |
|---|---|---|
| Wards | WardID PK; WardCode unique | Hospital ward and department directory |
| Users | UserID PK; Username unique | Staff accounts, password hashes and roles |
| Vendors | VendorID PK; VendorCode unique | External service-provider directory |
| VendorSLAs | SLAID PK; VendorID FK | Effective-dated vendor response/resolution targets |
| Assets | AssetID PK; AssetCode unique; WardID FK; VendorID FK | Biomedical equipment register |
| Breakdowns | BreakdownID PK; BreakdownRef unique; AssetID FK; ReportedByUserID FK | Fault report, severity, timestamps and status |
| MaintenanceJobs | JobID PK; BreakdownID FK; EngineerUserID FK | Engineer assignment and repair history |
| SpareParts | PartID PK; PartCode unique | Stock catalogue and reorder thresholds |
| JobSpareParts | JobPartID PK; JobID FK; PartID FK | Parts consumed by maintenance job |
| RepairCosts | CostID PK; JobID FK; RecordedByUserID FK | Labour, parts, vendor and other costs |
| EscalationLogs | EscalationID PK; BreakdownID FK | SLA escalation audit history |
| AuditLogs | AuditID PK; UserID FK | User actions and important data changes |
| ImportedAlerts | ImportedAlertID PK; MessageID unique; BreakdownID FK | Idempotency and import tracking for Java alerts |
| NotificationLogs | NotificationID PK; BreakdownID FK | Notification delivery audit |

## Controlled vocabularies
- Severity: `CRITICAL`, `HIGH`, `MEDIUM`, `LOW`.
- Asset criticality: `CRITICAL_CARE`, `HIGH`, `STANDARD`.
- Asset status: `OPERATIONAL`, `UNDER_MAINTENANCE`, `OUT_OF_SERVICE`, `RETIRED`.
- Breakdown status: `Reported`, `Triaged`, `Assigned`, `Responded`, `InProgress`, `AwaitingParts`, `Repaired`, `Verified`, `Closed`, `Cancelled`.
- User roles: `ADMIN`, `SUPERVISOR`, `ENGINEER`, `WARD_REPORTER`, `STORE`, `EXECUTIVE`.
- Cost types: `LABOUR`, `PARTS`, `VENDOR`, `OTHER`.

## Data integrity
- All unique identifiers use unique indexes.
- Foreign keys are created by the Windows bootstrapper where supported by the installed ACE provider.
- Monetary values use Access `CURRENCY` to avoid floating-point rounding.
- Quantities must be positive for usage and non-negative in stock.
- Passwords are salted PBKDF2-SHA256 hashes; plaintext is never persisted.
- Synthetic data only; no patient-identifiable or clinical data is stored.
