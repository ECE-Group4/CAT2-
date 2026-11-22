# Entity-Relationship Diagram (ERD) & Relational Mapping
## County Referral Hospital Biomedical Equipment Maintenance & SLA System (HBMS)

**Unit Code & Title:** ECE 2423 – Advanced Programming  
**Programme:** Bachelor of Technology in Electrical and Computer Engineering (Year IV, Semester I)  
**Academic Year:** 2026/2027  
**Institution:** Kirinyaga University — School of Pure and Applied Sciences  
**Lecturer:** Dr. Stephen Kahara Wanjau, PhD  
**Group:** Group 4  
**Assigned Case Study:** Group 4 — County Referral Hospital Biomedical Equipment Maintenance & SLA System  
**Document Version:** 1.0  
**Tracking Issue:** [Issue #6: [TASK-2.2] Database Normalization Analysis (1NF to 3NF) & Entity-Relationship Diagram (ERD)](https://github.com/ECE-Group4/CAT2-/issues/6)

---

## 1. Visual Entity-Relationship Diagram (Crow's Foot Notation)

The diagram below documents the complete entity-relationship model for `HBMS.accdb` with explicit Primary Keys (`PK`), Foreign Keys (`FK`), Unique Keys (`UK`), and cardinalities:

```mermaid
erDiagram
    WARDS ||--o{ ASSETS : "houses"
    ASSETS ||--o{ BREAKDOWN_LOGS : "experiences"
    USERS ||--o{ BREAKDOWN_LOGS : "reports"
    USERS ||--o{ MAINTENANCE_JOBS : "assigned_to"
    USERS ||--o{ REPAIR_COSTS : "approves"
    VENDORS ||--o{ VENDOR_SLAS : "commits_to"
    VENDOR_SLAS ||--o{ BREAKDOWN_LOGS : "governs"
    BREAKDOWN_LOGS ||--|| MAINTENANCE_JOBS : "spawns"
    BREAKDOWN_LOGS ||--o{ ESCALATION_LOGS : "triggers"
    MAINTENANCE_JOBS ||--o{ JOB_SPARE_PARTS : "consumes"
    SPARE_PARTS ||--o{ JOB_SPARE_PARTS : "allocated_to"
    MAINTENANCE_JOBS ||--o{ REPAIR_COSTS : "incurs"

    WARDS {
        int WardID PK
        string WardName UK
        string DepartmentName
        string LocationDescription
        int BedCapacity
        boolean IsActive
    }

    ASSETS {
        int AssetID PK
        string AssetCode UK
        string EquipmentName
        string EquipmentCategory
        string Manufacturer
        string Model
        string SerialNumber UK
        int WardID FK
        string Criticality
        string AssetStatus
        datetime DateCommissioned
        currency PurchaseCost
        datetime WarrantyExpiry
    }

    USERS {
        int UserID PK
        string FullName
        string Username UK
        string PasswordHash
        string Role
        string ContactEmail
        string ContactPhone
        boolean IsActive
        datetime CreatedAt
    }

    VENDORS {
        int VendorID PK
        string VendorName UK
        string ContactPerson
        string Phone
        string Email
        string PhysicalAddress
        string ContractRef
        boolean IsActive
    }

    VENDOR_SLAS {
        int SLAID PK
        int VendorID FK
        string SLAName
        string EquipmentCategory
        double ResponseHours
        double ResolutionHours
        double LowPowerSurchargeRate
        datetime EffectiveFrom
        datetime EffectiveTo
        boolean IsActive
    }

    BREAKDOWN_LOGS {
        int BreakdownID PK
        string BreakdownRef UK
        int AssetID FK
        int ReportedByUserID FK
        int SLAID FK
        datetime ReportedAt
        string Severity
        string Description
        datetime ResponseDeadline
        datetime ResponseAt
        string ResponseBy
        string Status
        datetime ClosedAt
    }

    MAINTENANCE_JOBS {
        int JobID PK
        int BreakdownID FK,UK
        int EngineerUserID FK
        datetime AssignedAt
        datetime StartedAt
        datetime CompletedAt
        double LaborHours
        currency HourlyRate
        string Diagnosis
        string WorkPerformed
        string VerificationNotes
        string JobStatus
    }

    SPARE_PARTS {
        int PartID PK
        string PartCode UK
        string PartName
        string EquipmentCategory
        string Unit
        int QuantityAvailable
        int ReorderLevel
        currency UnitCost
    }

    JOB_SPARE_PARTS {
        int JobPartID PK
        int JobID FK
        int PartID FK
        int QuantityUsed
        currency UnitCostAtUse
    }

    REPAIR_COSTS {
        int CostID PK
        int JobID FK
        string CostType
        string Description
        currency Amount
        datetime RecordedAt
        int RecordedByUserID FK
    }

    ESCALATION_LOGS {
        int EscalationID PK
        int BreakdownID FK
        datetime EscalatedAt
        string EscalationLevel
        string Reason
        string NotifiedTo
        string NotificationStatus
    }
```

---

## 2. Cardinality & Relational Semantics

| Parent Entity | Relationship | Child Entity | Business Semantic & Rule Enforced |
|---|---|---|---|
| **`Wards`** | `1 : M` | **`Assets`** | A clinical ward accommodates zero, one, or multiple biomedical devices. An asset resides in exactly one ward at any given time. |
| **`Assets`** | `1 : M` | **`BreakdownLogs`** | An asset can experience zero or multiple breakdown events over its operational lifespan. Each breakdown refers to exactly one asset. |
| **`Vendors`** | `1 : M` | **`VendorSLAs`** | A vendor can hold multiple SLA contracts covering different equipment lines (e.g. 4-hour critical care vs 24-hour routine). Each SLA belongs to one vendor. |
| **`VendorSLAs`** | `1 : M` | **`BreakdownLogs`** | An SLA policy can govern multiple breakdown events. A breakdown log is governed by zero or one applicable vendor SLA policy. |
| **`BreakdownLogs`** | `1 : 1` | **`MaintenanceJobs`** | In this prototype design, a single breakdown event spawns exactly one active maintenance work order, enforced by a Unique Key (`UK`) on `MaintenanceJobs.BreakdownID`. |
| **`BreakdownLogs`** | `1 : M` | **`EscalationLogs`** | A breakdown incident can generate zero, one, or multiple progressive escalation audit entries if response targets continue to be breached. |
| **`MaintenanceJobs`** | `1 : M` | **`JobSpareParts`** | A maintenance job can consume multiple spare parts. Each allocation line item links to one job and one inventory part. |
| **`SpareParts`** | `1 : M` | **`JobSpareParts`** | A spare part catalog item can be allocated across multiple maintenance jobs over time. |
| **`MaintenanceJobs`** | `1 : M` | **`RepairCosts`** | A maintenance job accumulates multiple itemized cost entries (Labor, Parts, Vendor Charges). |
| **`Users`** | `1 : M` | **`BreakdownLogs`** | A clinical user can submit multiple breakdown reports. |
| **`Users`** | `1 : M` | **`MaintenanceJobs`** | A biomedical engineer can be assigned to multiple work orders. |
