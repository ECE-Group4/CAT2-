# Microsoft Access Data Dictionary Specification
## County Referral Hospital Biomedical Equipment Maintenance & SLA System (HBMS)

**Unit Code & Title:** ECE 2423 – Advanced Programming  
**Programme:** Bachelor of Technology in Electrical and Computer Engineering (Year IV, Semester I)  
**Academic Year:** 2026/2027  
**Institution:** Kirinyaga University — School of Pure and Applied Sciences  
**Lecturer:** Dr. Stephen Kahara Wanjau, PhD  
**Group:** Group 4  
**Assigned Case Study:** Group 4 — County Referral Hospital Biomedical Equipment Maintenance & SLA System  
**Database File:** `HBMS.accdb` (Microsoft Access 2016–365 Relational Format)  
**Document Version:** 1.0  
**Tracking Issue:** [Issue #5: [TASK-2.1] Microsoft Access Database Schema & Data Dictionary Design](https://github.com/ECE-Group4/CAT2-/issues/5)

---

## 1. Overview & Data Architecture

The **Hospital Biomedical Equipment Maintenance & SLA Management System (HBMS)** relational database comprises **11 normalized entities** designed to track biomedical assets, ward locations, equipment breakdown incidents, vendor contracts, work orders, spare parts stock, repair expenditures, and automated SLA escalation audits.

This data dictionary provides a complete technical reference detailing:
- Table names and primary keys.
- Attributes, data types (Microsoft Access and ANSI SQL equivalents), field sizes, nullability, default values, and relational constraints.
- Clinical and operational business rules enforced at the database level.

---

## 2. Table Data Dictionaries

### 2.1 Table: `Users`
*Stores user accounts, authentication credentials, and role-based permissions across hospital personnel.*

| Column Name | MS Access Data Type | ANSI SQL Type | Size | Nullable | Key / Constraint | Default Value | Description / Business Meaning |
|---|---|---|---|---|---|---|---|
| `UserID` | AutoNumber | INTEGER | 4 | No | **PK** | Identity | System-generated unique identifier for user account. |
| `FullName` | Short Text | VARCHAR(100) | 100 | No | None | None | Legal employee full name. |
| `Username` | Short Text | VARCHAR(30) | 30 | No | **UK** | None | Unique login handle. |
| `PasswordHash` | Short Text | VARCHAR(128) | 128 | No | None | None | Cryptographic password hash (e.g. SHA-256). |
| `Role` | Short Text | VARCHAR(30) | 30 | No | Check Constraint | None | RBAC role: `Biomedical Engineer`, `Ward Personnel`, `Hospital Administrator`. |
| `ContactEmail` | Short Text | VARCHAR(100) | 100 | Yes | None | None | Work email for alerts and dispatch notifications. |
| `ContactPhone` | Short Text | VARCHAR(20) | 20 | Yes | None | None | Mobile phone number for SMS emergency notifications. |
| `IsActive` | Yes/No | BOOLEAN | 1 | No | None | `True` | Account active flag; deactivation preserves historical audit trails. |
| `CreatedAt` | Date/Time | DATETIME | 8 | No | None | `Now()` | Account provisioning timestamp. |

---

### 2.2 Table: `Wards`
*Maintains clinical hospital locations, departments, and capacity.*

| Column Name | MS Access Data Type | ANSI SQL Type | Size | Nullable | Key / Constraint | Default Value | Description / Business Meaning |
|---|---|---|---|---|---|---|---|
| `WardID` | AutoNumber | INTEGER | 4 | No | **PK** | Identity | System-generated unique identifier for ward/department. |
| `WardName` | Short Text | VARCHAR(50) | 50 | No | **UK** | None | Department name (e.g., `Intensive Care Unit (ICU)`, `Operating Theatre`). |
| `DepartmentName` | Short Text | VARCHAR(50) | 50 | No | None | None | Clinical division (e.g., `Critical Care`, `Surgical Services`, `Emergency`). |
| `LocationDescription` | Short Text | VARCHAR(100) | 100 | Yes | None | None | Physical campus location (e.g., `Block B, Level 2, Room 204`). |
| `BedCapacity` | Number (Integer) | INTEGER | 2 | No | $\ge 0$ | `0` | Number of patient beds allocated to ward. |
| `IsActive` | Yes/No | BOOLEAN | 1 | No | None | `True` | Operational status of the ward unit. |

---

### 2.3 Table: `Assets`
*Master register of all hospital biomedical equipment.*

| Column Name | MS Access Data Type | ANSI SQL Type | Size | Nullable | Key / Constraint | Default Value | Description / Business Meaning |
|---|---|---|---|---|---|---|---|
| `AssetID` | AutoNumber | INTEGER | 4 | No | **PK** | Identity | System-generated asset surrogate key. |
| `AssetCode` | Short Text | VARCHAR(20) | 20 | No | **UK** | None | Hospital asset barcode/tag (e.g., `EQ-VENT-001`, `EQ-MON-004`). |
| `EquipmentName` | Short Text | VARCHAR(100) | 100 | No | None | None | Standard clinical device name (e.g., `Mechanical Life-Support Ventilator`). |
| `EquipmentCategory` | Short Text | VARCHAR(50) | 50 | No | Check Constraint | None | Category: `Ventilators`, `Patient Monitors`, `Infusion Pumps`, `X-Ray Units`. |
| `Manufacturer` | Short Text | VARCHAR(50) | 50 | No | None | None | OEM manufacturer (e.g., `Hamilton Medical`, `Mindray`, `GE Healthcare`). |
| `Model` | Short Text | VARCHAR(50) | 50 | No | None | None | OEM model designation (e.g., `Hamilton-C6`, `ePM 12M`). |
| `SerialNumber` | Short Text | VARCHAR(50) | 50 | No | **UK** | None | Factory stamped serial number. |
| `WardID` | Number (Long Int) | INTEGER | 4 | No | **FK** $\rightarrow$ `Wards(WardID)` | None | Current physical ward assignment. |
| `Criticality` | Short Text | VARCHAR(20) | 20 | No | Check Constraint | None | Clinical urgency: `Critical Care`, `General Care`. |
| `AssetStatus` | Short Text | VARCHAR(20) | 20 | No | Check Constraint | `Operational` | State: `Operational`, `Under Maintenance`, `Malfunctioning`, `Decommissioned`. |
| `DateCommissioned` | Date/Time | DATETIME | 8 | No | None | None | Date asset was clinically deployed into service. |
| `PurchaseCost` | Currency | DECIMAL(12,2) | 8 | No | $\ge 0.00$ | `0.00` | Initial capital acquisition cost in KES. |
| `WarrantyExpiry` | Date/Time | DATETIME | 8 | Yes | None | None | End date of OEM manufacturer warranty. |

---

### 2.4 Table: `Vendors`
*Stores contracted equipment manufacturers and biomedical engineering service providers.*

| Column Name | MS Access Data Type | ANSI SQL Type | Size | Nullable | Key / Constraint | Default Value | Description / Business Meaning |
|---|---|---|---|---|---|---|---|
| `VendorID` | AutoNumber | INTEGER | 4 | No | **PK** | Identity | Unique vendor surrogate key. |
| `VendorName` | Short Text | VARCHAR(100) | 100 | No | **UK** | None | Commercial company name (e.g., `Philips Medical Systems East Africa`). |
| `ContactPerson` | Short Text | VARCHAR(50) | 50 | No | None | None | Designated biomedical account manager or service liaison. |
| `Phone` | Short Text | VARCHAR(20) | 20 | No | None | None | 24/7 emergency dispatch telephone number. |
| `Email` | Short Text | VARCHAR(100) | 100 | No | None | None | Official service dispatch email address. |
| `PhysicalAddress` | Short Text | VARCHAR(150) | 150 | Yes | None | None | Regional workshop or office physical location. |
| `ContractRef` | Short Text | VARCHAR(50) | 50 | Yes | None | None | Hospital legal procurement contract number. |
| `IsActive` | Yes/No | BOOLEAN | 1 | No | None | `True` | Vendor active engagement status. |

---

### 2.5 Table: `VendorSLAs`
*Formal contractual Service Level Agreement parameters.*

| Column Name | MS Access Data Type | ANSI SQL Type | Size | Nullable | Key / Constraint | Default Value | Description / Business Meaning |
|---|---|---|---|---|---|---|---|
| `SLAID` | AutoNumber | INTEGER | 4 | No | **PK** | Identity | Unique SLA policy surrogate key. |
| `VendorID` | Number (Long Int) | INTEGER | 4 | No | **FK** $\rightarrow$ `Vendors(VendorID)` | None | Contracted vendor responsible for SLA. |
| `SLAName` | Short Text | VARCHAR(100) | 100 | No | None | None | SLA policy description (e.g., `Critical Care 4-Hour Response SLA`). |
| `EquipmentCategory` | Short Text | VARCHAR(50) | 50 | No | None | None | Equipment category covered by policy (`Ventilators`, `All`). |
| `ResponseHours` | Number (Double) | DOUBLE | 8 | No | $> 0.0$ | `4.0` | Contractual response window in hours (statutory 4.0h for critical care). |
| `ResolutionHours` | Number (Double) | DOUBLE | 8 | No | $> 0.0$ | `24.0` | Maximum target repair completion duration in hours. |
| `LowPowerSurchargeRate` | Number (Double) | DOUBLE | 8 | No | $\ge 0.0$ | `0.0` | Surcharge rate applied for sub-optimal environmental conditions. |
| `EffectiveFrom` | Date/Time | DATETIME | 8 | No | None | None | SLA policy commencement date. |
| `EffectiveTo` | Date/Time | DATETIME | 8 | No | None | None | SLA contract expiration date. |
| `IsActive` | Yes/No | BOOLEAN | 1 | No | None | `True` | SLA active enforcement flag. |

---

### 2.6 Table: `BreakdownLogs`
*Records reported failure incidents, severity, response times, and incident state.*

| Column Name | MS Access Data Type | ANSI SQL Type | Size | Nullable | Key / Constraint | Default Value | Description / Business Meaning |
|---|---|---|---|---|---|---|---|
| `BreakdownID` | AutoNumber | INTEGER | 4 | No | **PK** | Identity | System-generated breakdown primary key. |
| `BreakdownRef` | Short Text | VARCHAR(20) | 20 | No | **UK** | None | Human-readable tracking number (e.g., `BRK-2026-001`). |
| `AssetID` | Number (Long Int) | INTEGER | 4 | No | **FK** $\rightarrow$ `Assets(AssetID)` | None | Malfunctioning biomedical asset. |
| `ReportedByUserID` | Number (Long Int) | INTEGER | 4 | No | **FK** $\rightarrow$ `Users(UserID)` | None | Clinical staff member logging incident. |
| `SLAID` | Number (Long Int) | INTEGER | 4 | Yes | **FK** $\rightarrow$ `VendorSLAs(SLAID)` | None | Applicable contractual SLA policy. |
| `ReportedAt` | Date/Time | DATETIME | 8 | No | None | `Now()` | Exact failure logging timestamp. |
| `Severity` | Short Text | VARCHAR(20) | 20 | No | Check Constraint | None | Clinical severity: `Critical`, `High`, `Medium`, `Low`. |
| `Description` | Long Text (Memo) | LONGTEXT | - | No | None | None | Detailed narrative of symptoms and operational context. |
| `ResponseDeadline` | Date/Time | DATETIME | 8 | No | None | None | Calculated deadline: `ReportedAt + ResponseHours` (4h for critical). |
| `ResponseAt` | Date/Time | DATETIME | 8 | Yes | None | None | Official timestamp when technician acknowledged fault. |
| `ResponseBy` | Short Text | VARCHAR(50) | 50 | Yes | None | None | Name of acknowledging engineer or vendor technician. |
| `Status` | Short Text | VARCHAR(20) | 20 | No | Check Constraint | `Pending` | Incident state: `Pending`, `Assigned`, `In Progress`, `ESCALATED`, `Closed`. |
| `ClosedAt` | Date/Time | DATETIME | 8 | Yes | None | None | Final sign-off timestamp upon full repair verification. |

---

### 2.7 Table: `MaintenanceJobs`
*Technical work orders, engineer dispatches, and diagnostics.*

| Column Name | MS Access Data Type | ANSI SQL Type | Size | Nullable | Key / Constraint | Default Value | Description / Business Meaning |
|---|---|---|---|---|---|---|---|
| `JobID` | AutoNumber | INTEGER | 4 | No | **PK** | Identity | Unique work order job number. |
| `BreakdownID` | Number (Long Int) | INTEGER | 4 | No | **FK, UK** $\rightarrow$ `BreakdownLogs` | None | Associated breakdown incident (enforces 1:1 job per incident). |
| `EngineerUserID` | Number (Long Int) | INTEGER | 4 | No | **FK** $\rightarrow$ `Users(UserID)` | None | Assigned biomedical engineer. |
| `AssignedAt` | Date/Time | DATETIME | 8 | No | None | `Now()` | Timestamp of job assignment. |
| `StartedAt` | Date/Time | DATETIME | 8 | Yes | None | None | Timestamp when engineer commenced physical repair. |
| `CompletedAt` | Date/Time | DATETIME | 8 | Yes | None | None | Timestamp when repair was technically concluded. |
| `LaborHours` | Number (Double) | DOUBLE | 8 | No | $\ge 0.0$ | `0.0` | Total technician hours dedicated to repair. |
| `HourlyRate` | Currency | DECIMAL(10,2) | 8 | No | $\ge 0.00$ | `1500.00` | Standard engineer labor rate in KES/hour. |
| `Diagnosis` | Long Text (Memo) | LONGTEXT | - | Yes | None | None | Root cause analysis and technical defect description. |
| `WorkPerformed` | Long Text (Memo) | LONGTEXT | - | Yes | None | None | Actions executed (e.g., recalibrated blender, replaced O2 cell). |
| `VerificationNotes` | Long Text (Memo) | LONGTEXT | - | Yes | None | None | Calibration testing, electrical safety check results. |
| `JobStatus` | Short Text | VARCHAR(20) | 20 | No | Check Constraint | `Assigned` | Status: `Assigned`, `In Progress`, `Awaiting Parts`, `Completed`. |

---

### 2.8 Table: `SpareParts`
*Master catalog of biomedical replacement parts and stock inventory.*

| Column Name | MS Access Data Type | ANSI SQL Type | Size | Nullable | Key / Constraint | Default Value | Description / Business Meaning |
|---|---|---|---|---|---|---|---|
| `PartID` | AutoNumber | INTEGER | 4 | No | **PK** | Identity | Unique part surrogate key. |
| `PartCode` | Short Text | VARCHAR(20) | 20 | No | **UK** | None | Barcode/SKU (e.g., `PRT-VENT-O2`, `PRT-PUMP-BAT`). |
| `PartName` | Short Text | VARCHAR(100) | 100 | No | None | None | Technical component description (e.g., `Galvanic O2 Sensor Cell`). |
| `EquipmentCategory` | Short Text | VARCHAR(50) | 50 | No | None | None | Compatible equipment category (`Ventilators`, `Infusion Pumps`). |
| `Unit` | Short Text | VARCHAR(20) | 20 | No | None | `Piece` | Measurement unit: `Piece`, `Kit`, `Meter`, `Assembly`. |
| `QuantityAvailable` | Number (Integer) | INTEGER | 2 | No | $\ge 0$ | `0` | Physical stock count currently on workshop shelves. |
| `ReorderLevel` | Number (Integer) | INTEGER | 2 | No | $\ge 0$ | `2` | Minimum inventory threshold triggering automated reorder warning. |
| `UnitCost` | Currency | DECIMAL(10,2) | 8 | No | $\ge 0.00$ | `0.00` | Unit purchase acquisition price in KES. |

---

### 2.9 Table: `JobSpareParts`
*Junction table recording spare parts consumed during specific maintenance jobs.*

| Column Name | MS Access Data Type | ANSI SQL Type | Size | Nullable | Key / Constraint | Default Value | Description / Business Meaning |
|---|---|---|---|---|---|---|---|
| `JobPartID` | AutoNumber | INTEGER | 4 | No | **PK** | Identity | Unique allocation record surrogate key. |
| `JobID` | Number (Long Int) | INTEGER | 4 | No | **FK** $\rightarrow$ `MaintenanceJobs(JobID)` | None | Maintenance job consuming the component. |
| `PartID` | Number (Long Int) | INTEGER | 4 | No | **FK** $\rightarrow$ `SpareParts(PartID)` | None | Spare part consumed. |
| `QuantityUsed` | Number (Integer) | INTEGER | 2 | No | $> 0$ | `1` | Number of units issued and installed. |
| `UnitCostAtUse` | Currency | DECIMAL(10,2) | 8 | No | $\ge 0.00$ | None | Unit cost locked at time of consumption (preserves historical cost). |

---

### 2.10 Table: `RepairCosts`
*Financial accounting ledger itemizing maintenance expenditures.*

| Column Name | MS Access Data Type | ANSI SQL Type | Size | Nullable | Key / Constraint | Default Value | Description / Business Meaning |
|---|---|---|---|---|---|---|---|
| `CostID` | AutoNumber | INTEGER | 4 | No | **PK** | Identity | Unique cost line-item surrogate key. |
| `JobID` | Number (Long Int) | INTEGER | 4 | No | **FK** $\rightarrow$ `MaintenanceJobs(JobID)` | None | Associated work order. |
| `CostType` | Short Text | VARCHAR(30) | 30 | No | Check Constraint | None | Category: `Internal Labor`, `Spare Parts`, `External Vendor Fee`. |
| `Description` | Short Text | VARCHAR(100) | 100 | No | None | None | Narrative line item (e.g., `3.5 Engineering Labor Hours`). |
| `Amount` | Currency | DECIMAL(10,2) | 8 | No | $\ge 0.00$ | `0.00` | Financial amount in KES. |
| `RecordedAt` | Date/Time | DATETIME | 8 | No | None | `Now()` | Accounting entry timestamp. |
| `RecordedByUserID` | Number (Long Int) | INTEGER | 4 | No | **FK** $\rightarrow$ `Users(UserID)` | None | Biomedical accountant or engineer recording line item. |

---

### 2.11 Table: `EscalationLogs`
*Immutable audit trail of unacknowledged critical-care breakdowns breaching SLA response deadlines.*

| Column Name | MS Access Data Type | ANSI SQL Type | Size | Nullable | Key / Constraint | Default Value | Description / Business Meaning |
|---|---|---|---|---|---|---|---|
| `EscalationID` | AutoNumber | INTEGER | 4 | No | **PK** | Identity | System-generated escalation audit key. |
| `BreakdownID` | Number (Long Int) | INTEGER | 4 | No | **FK** $\rightarrow$ `BreakdownLogs(BreakdownID)` | None | Breached critical-care breakdown incident. |
| `EscalatedAt` | Date/Time | DATETIME | 8 | No | None | `Now()` | Timestamp when automated 4-hour rule triggered escalation. |
| `EscalationLevel` | Short Text | VARCHAR(30) | 30 | No | None | None | Priority level: `Level-1 Executive Alert`, `Level-2 Director Escalation`. |
| `Reason` | Short Text | VARCHAR(150) | 150 | No | None | None | Statutory breach rationale (e.g., `4-Hour Critical Response SLA Exceeded`). |
| `NotifiedTo` | Short Text | VARCHAR(100) | 100 | No | None | None | Recipients: `Biomedical Director, Hospital Administrator`. |
| `NotificationStatus` | Short Text | VARCHAR(20) | 20 | No | None | `DISPATCHED` | Delivery status: `DISPATCHED`, `ACKNOWLEDGED`. |

---

## 3. Relational Cardinality & Integrity Summary

```
Wards (1) --------< (M) Assets
Assets (1) -------< (M) BreakdownLogs
Users (1) --------< (M) BreakdownLogs [as Reporter]
Users (1) --------< (M) MaintenanceJobs [as Engineer]
Vendors (1) ------< (M) VendorSLAs
VendorSLAs (1) ---< (M) BreakdownLogs
BreakdownLogs (1) -- (1) MaintenanceJobs
BreakdownLogs (1) -< (M) EscalationLogs
MaintenanceJobs (1) < (M) JobSpareParts
SpareParts (1) ---< (M) JobSpareParts
MaintenanceJobs (1) < (M) RepairCosts
Users (1) --------< (M) RepairCosts [as Approver]
```

All foreign keys enforce referential integrity. Deletions on master reference entities (`Wards`, `Vendors`, `Assets`) are restricted if active breakdown or job history exists, preventing orphaned child records.
