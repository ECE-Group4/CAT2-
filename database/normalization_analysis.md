# Relational Database Normalization Analysis (1NF to 3NF)
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

## 1. Executive Summary & Normalization Objectives

In compliance with the ECE 2423 curriculum on Relational Database Engineering, this document details the formal normalization process applied to the **Hospital Biomedical Equipment Maintenance & SLA Management System (HBMS)**. Starting from an unnormalized, flat clinical maintenance log containing repeating groups and composite fields, the database structure was systematically normalized through **First Normal Form (1NF)**, **Second Normal Form (2NF)**, and **Third Normal Form (3NF)**.

### Governing Engineering Objectives:
1. **Eliminate Data Redundancy:** Avoid duplicate storage of equipment specifications, ward locations, and vendor contract terms across repetitive breakdown records.
2. **Prevent Modification Anomalies:** Ensure that adding a new biomedical device, updating a vendor phone number, or deleting a completed work order causes zero unintended data corruption.
3. **Preserve Functional Dependencies & Lossless Decomposition:** Guarantee that decomposing large unnormalized entities into smaller relations retains all data integrity constraints and allows full relational reconstruction via standard SQL `JOIN` operations.

---

## 2. Unnormalized Form (UNF)

In typical legacy paper logbooks or non-relational spreadsheets, a single hospital maintenance log entry attempts to capture all operational information in one unnormalized record:

$$\text{UNF\_MaintenanceLog}(\underline{\text{BreakdownRef}}, \text{ReportedAt}, \text{AssetCode}, \text{EquipmentName}, \text{Category}, \text{Model}, \text{SerialNo}, \text{WardName},$$
$$\text{Department}, \text{Severity}, \text{VendorName}, \text{VendorPhone}, \text{SLA\_Hours}, \text{EngineerName}, \text{LaborHours},$$
$$\{\text{PartCode}, \text{PartName}, \text{PartQty}, \text{PartCost}\}, \text{JobTotalCost}, \text{EscalationReason}, \text{Status})$$

### Flaws & Vulnerabilities in UNF:
- **Repeating Groups:** Multiple spare parts used during a single maintenance job are stored as repeating, comma-separated lists or variable-length repeating groups ($\{\text{PartCode}, \text{PartName}, \dots\}$).
- **Multi-Valued Cells:** A single database field holds composite arrays, violating basic relational atomicity.
- **Extreme Data Duplication:** Every time a breakdown occurs on the same Hamilton-C6 ventilator, the manufacturer, model, serial number, ward name, vendor contact details, and contractual SLA hours are re-typed.

---

## 3. First Normal Form (1NF)

### 3.1 Definition & Transformation Rule
> **A relation is in First Normal Form (1NF) if and only if:**
> 1. All attribute values are atomic (each cell contains exactly one scalar value; no repeating groups, arrays, or comma-separated lists).
> 2. Every row is unique and identifiable by a defined Primary Key.

### 3.2 Transformation to 1NF:
To achieve 1NF, the repeating groups of spare parts are eliminated by creating separate rows for each consumed spare part, and atomic surrogate primary keys (`BreakdownID`, `AssetID`, `PartID`, etc.) are established.

$$\text{1NF\_Breakdown}(\underline{\text{BreakdownRef}}, \underline{\text{PartCode}}, \text{ReportedAt}, \text{AssetCode}, \text{EquipmentName}, \text{Category}, \text{Model}, \text{SerialNo},$$
$$\text{WardName}, \text{Department}, \text{Severity}, \text{VendorName}, \text{VendorPhone}, \text{SLA\_Hours}, \text{EngineerName},$$
$$\text{LaborHours}, \text{PartName}, \text{PartQty}, \text{PartCost}, \text{JobTotalCost}, \text{Status})$$

### 3.3 Functional Dependencies in 1NF:
1. $\text{BreakdownRef} \rightarrow \text{ReportedAt}, \text{AssetCode}, \text{EquipmentName}, \text{Category}, \text{Model}, \text{SerialNo}, \text{WardName}, \text{Department}, \text{Severity}, \text{VendorName}, \text{SLA\_Hours}$
2. $\{\text{BreakdownRef}, \text{PartCode}\} \rightarrow \text{PartQty}, \text{PartCost}$
3. $\text{PartCode} \rightarrow \text{PartName}, \text{PartCost}$
4. $\text{AssetCode} \rightarrow \text{EquipmentName}, \text{Category}, \text{Model}, \text{SerialNo}, \text{WardName}, \text{Department}$
5. $\text{VendorName} \rightarrow \text{VendorPhone}, \text{SLA\_Hours}$

### 3.4 Remaining Flaws in 1NF:
- **Partial Functional Dependencies:** Attributes like `PartName` depend only on `PartCode`, not on the entire composite key $\{\text{BreakdownRef}, \text{PartCode}\}$.
- **Massive Redundancy:** Repeating `BreakdownRef` for every part consumed duplicates all breakdown, asset, and vendor data.

---

## 4. Second Normal Form (2NF)

### 4.1 Definition & Transformation Rule
> **A relation is in Second Normal Form (2NF) if and only if:**
> 1. It is already in 1NF.
> 2. Every non-key attribute is **fully functionally dependent** on the primary key (i.e., no partial dependencies exist where an attribute depends on only a subset of a composite primary key).

### 4.2 Transformation to 2NF:
We decompose relations with composite primary keys. The composite relationship between maintenance work orders and spare parts inventory is extracted into a dedicated associative bridge entity: `JobSpareParts`.

#### Decomposed 2NF Relations:
1. **$\text{Assets}(\underline{\text{AssetID}}, \text{AssetCode}, \text{EquipmentName}, \text{Category}, \text{Model}, \text{SerialNo}, \text{WardName}, \text{Department}, \text{Criticality}, \text{Status})$**
   - *Key:* `AssetID` (All attributes depend entirely on `AssetID`).
2. **$\text{SpareParts}(\underline{\text{PartID}}, \text{PartCode}, \text{PartName}, \text{Unit}, \text{QuantityAvailable}, \text{UnitCost})$**
   - *Key:* `PartID` (All attributes depend entirely on `PartID`).
3. **$\text{MaintenanceJobs}(\underline{\text{JobID}}, \text{BreakdownRef}, \text{EngineerName}, \text{LaborHours}, \text{Diagnosis}, \text{JobStatus})$**
   - *Key:* `JobID`.
4. **$\text{JobSpareParts}(\underline{\text{JobPartID}}, \text{JobID}^*, \text{PartID}^*, \text{QuantityUsed}, \text{UnitCostAtUse})$**
   - *Key:* `JobPartID` (or composite $\{\text{JobID}, \text{PartID}\}$). `QuantityUsed` and `UnitCostAtUse` depend on the combination of the job and the part.
5. **$\text{Breakdowns}(\underline{\text{BreakdownID}}, \text{BreakdownRef}, \text{AssetID}^*, \text{ReportedAt}, \text{Severity}, \text{VendorName}, \text{VendorPhone}, \text{ResponseHours}, \text{Status})$**
   - *Key:* `BreakdownID`.

### 4.3 Remaining Flaws in 2NF:
- **Transitive Functional Dependencies:** While no partial dependencies remain, non-key attributes still depend on other non-key attributes:
  - In `Assets`: $\text{AssetID} \rightarrow \text{WardName} \rightarrow \text{Department}$ (Ward details depend on `WardName`).
  - In `Breakdowns`: $\text{BreakdownID} \rightarrow \text{VendorName} \rightarrow \text{VendorPhone}, \text{ResponseHours}$ (Vendor details depend on `VendorName`, not the breakdown itself).

---

## 5. Third Normal Form (3NF)

### 5.1 Definition & Transformation Rule
> **A relation is in Third Normal Form (3NF) if and only if:**
> 1. It is already in 2NF.
> 2. It contains **no transitive dependencies** (i.e., non-key attributes must depend solely on the primary key, directly and not through another non-key attribute: *"The key, the whole key, and nothing but the key, so help me Codd"*).

### 5.2 Transformation to 3NF:
We extract transitive relationships into independent domain entities:
1. **Ward Transitivity Extracted:** We create a separate entity `Wards` with `WardID` as PK. `Assets` now only holds a Foreign Key `WardID`.
   $$\text{Assets}(\dots, \text{WardID}^*) \quad \text{and} \quad \text{Wards}(\underline{\text{WardID}}, \text{WardName}, \text{DepartmentName}, \text{LocationDescription})$$
2. **Vendor Transitivity Extracted:** We create `Vendors` and `VendorSLAs` with `VendorID` and `SLAID` as PKs. `BreakdownLogs` now references `SLAID` directly.
   $$\text{Vendors}(\underline{\text{VendorID}}, \text{VendorName}, \text{ContactPerson}, \text{Phone}, \text{Email})$$
   $$\text{VendorSLAs}(\underline{\text{SLAID}}, \text{VendorID}^*, \text{SLAName}, \text{ResponseHours}, \text{ResolutionHours})$$
3. **User Transitivity Extracted:** Staff names are replaced by references to a centralized `Users` entity (`ReportedByUserID`, `EngineerUserID`).
4. **Audit Transitivity Extracted:** SLA breach escalations are decoupled into `EscalationLogs`.
5. **Financial Transitivity Extracted:** Itemized expenses are decoupled into `RepairCosts`.

---

## 6. Final Normalized 3NF Relational Schema

The resulting 11 entities in Third Normal Form (3NF) are completely free of redundancy and anomalies:

```
1. Users(UserID [PK], FullName, Username, PasswordHash, Role, ContactEmail, ContactPhone, IsActive, CreatedAt)
2. Wards(WardID [PK], WardName, DepartmentName, LocationDescription, BedCapacity, IsActive)
3. Assets(AssetID [PK], AssetCode, EquipmentName, EquipmentCategory, Manufacturer, Model, SerialNumber, WardID [FK], Criticality, AssetStatus, DateCommissioned, PurchaseCost, WarrantyExpiry)
4. Vendors(VendorID [PK], VendorName, ContactPerson, Phone, Email, PhysicalAddress, ContractRef, IsActive)
5. VendorSLAs(SLAID [PK], VendorID [FK], SLAName, EquipmentCategory, ResponseHours, ResolutionHours, LowPowerSurchargeRate, EffectiveFrom, EffectiveTo, IsActive)
6. BreakdownLogs(BreakdownID [PK], BreakdownRef, AssetID [FK], ReportedByUserID [FK], SLAID [FK], ReportedAt, Severity, Description, ResponseDeadline, ResponseAt, ResponseBy, Status, ClosedAt)
7. MaintenanceJobs(JobID [PK], BreakdownID [FK], EngineerUserID [FK], AssignedAt, StartedAt, CompletedAt, LaborHours, HourlyRate, Diagnosis, WorkPerformed, VerificationNotes, JobStatus)
8. SpareParts(PartID [PK], PartCode, PartName, EquipmentCategory, Unit, QuantityAvailable, ReorderLevel, UnitCost)
9. JobSpareParts(JobPartID [PK], JobID [FK], PartID [FK], QuantityUsed, UnitCostAtUse)
10. RepairCosts(CostID [PK], JobID [FK], CostType, Description, Amount, RecordedAt, RecordedByUserID [FK])
11. EscalationLogs(EscalationID [PK], BreakdownID [FK], EscalatedAt, EscalationLevel, Reason, NotifiedTo, NotificationStatus)
```

---

## 7. Anomaly Elimination Proof Matrix

| Anomaly Type | Unnormalized Form (UNF) Vulnerability | 3NF Resolution & Guarantee |
|---|---|---|
| **Insertion Anomaly** | Cannot add a new hospital ward or new biomedical vendor contract unless an active equipment breakdown exists to attach it to. | Wards, Vendors, and VendorSLAs exist as autonomous entities. New wards or vendors can be registered anytime without needing breakdown records. |
| **Update Anomaly** | If a vendor changes their emergency dispatch phone number, dozens of historical breakdown rows must be updated; missing one leaves conflicting numbers. | Vendor phone number is stored in exactly one tuple in `Vendors`. An update takes effect hospital-wide instantly with zero inconsistencies. |
| **Deletion Anomaly** | Deleting a resolved maintenance log entry inadvertently erases all historical record of the spare part used or the vendor's existence. | Work orders are isolated in `MaintenanceJobs`. Deleting or closing a job preserves the master parts catalog in `SpareParts` and vendor profile in `Vendors`. |
