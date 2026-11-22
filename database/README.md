# Database Engineering & Demonstration Guide
## County Referral Hospital Biomedical Equipment Maintenance & SLA System (HBMS)

**Unit Code & Title:** ECE 2423 – Advanced Programming  
**Programme:** Bachelor of Technology in Electrical and Computer Engineering (Year IV, Semester I)  
**Academic Year:** 2026/2027  
**Institution:** Kirinyaga University — School of Pure and Applied Sciences  
**Lecturer:** Dr. Stephen Kahara Wanjau, PhD  
**Group:** Group 4  
**Assigned Case Study:** Group 4 — County Referral Hospital Biomedical Equipment Maintenance & SLA System  
**Database File:** `HBMS.accdb`  
**Tracking Issues:**
- [Issue #5: [TASK-2.1] Microsoft Access Database Schema & Data Dictionary Design](https://github.com/ECE-Group4/CAT2-/issues/5)
- [Issue #6: [TASK-2.2] Database Normalization Analysis (1NF to 3NF) & Entity-Relationship Diagram (ERD)](https://github.com/ECE-Group4/CAT2-/issues/6)
- [Issue #7: [TASK-2.3] MS Access SQL Queries, Data Views & Synthetic Demonstration Dataset](https://github.com/ECE-Group4/CAT2-/issues/7)

---

## 1. Directory Structure

```
database/
├── schema.sql                   # Complete DDL script creating all 11 relational tables
├── data_dictionary.md           # Detailed technical data dictionary for all entities & attributes
├── normalization_analysis.md    # Formal 1NF to 3NF normalization study & anomaly proofs
├── erd.md                       # Crow's Foot Entity-Relationship Diagram & cardinalities
├── queries.sql                  # Pre-compiled MS Access SQL reporting queries & views
├── synthetic_dataset.sql        # Comprehensive realistic demonstration dataset
└── README.md                    # Database documentation & execution guide (this file)
```

---

## 2. Summary of Database Entities

The relational database enforces 11 normalized tables in Third Normal Form (3NF):

| # | Entity Name | Primary Key | Description | Record Count in Dataset |
|---|---|---|---|---|
| 1 | **`Users`** | `UserID` | Hospital staff accounts, roles, and authentication credentials | 4 |
| 2 | **`Wards`** | `WardID` | Hospital clinical locations and departmental divisions | 5 |
| 3 | **`Assets`** | `AssetID` | Master biomedical equipment register (Ventilators, Monitors, Pumps, X-Ray) | 20 |
| 4 | **`Vendors`** | `VendorID` | Medical equipment manufacturers and service contractors | 5 |
| 5 | **`VendorSLAs`** | `SLAID` | Contractual response and resolution SLA commitments | 5 |
| 6 | **`BreakdownLogs`** | `BreakdownID` | Equipment failure events, triage, response times, and state | 5 |
| 7 | **`MaintenanceJobs`** | `JobID` | Technical work orders, diagnostic notes, and labor logs | 3 |
| 8 | **`SpareParts`** | `PartID` | Medical replacement parts catalog and stock levels | 10 |
| 9 | **`JobSpareParts`** | `JobPartID` | Parts consumed on specific work orders | 3 |
| 10 | **`RepairCosts`** | `CostID` | Itemized financial accounting (Labor, Parts, Vendor fees) | 5 |
| 11 | **`EscalationLogs`** | `EscalationID` | Audit trail of unacknowledged critical-care SLA breaches | 1 |

---

## 3. Pre-Compiled Stored Queries & Views

The queries provided in [`queries.sql`](./queries.sql) directly satisfy operational workflows and executive reporting:

1. **`qry_ActiveBreakdowns`:** Displays open equipment failure incidents, clinical severity, and a dynamic calculation of remaining hours before the statutory response deadline expires.
2. **`qry_OverdueCriticalCareBreakdowns`:** Enforces the CAT 2 statutory 4-hour rule: identifies critical care assets where $\text{ResponseAt IS NULL}$, $\text{Now()} > \text{ResponseDeadline}$, and ensures no duplicate escalation record exists.
3. **`qry_DepartmentUptime`:** Aggregates total observation hours and downtime hours across hospital wards, computing operational uptime percentage:
   $$\text{Uptime (\%)} = \left[ \frac{\text{Observation Hours} - \text{Total Downtime}}{\text{Observation Hours}} \right] \times 100$$
4. **`qry_JobCostSummary`:** Calculates the total composite expenditure for every completed work order:
   $$\text{Total Job Cost} = \text{Labor Cost} + \text{Spare Parts Total} + \text{External Vendor Fees}$$
5. **`qry_VendorSLACompliance`:** Audits vendor SLA performance by measuring the percentage of breakdown dispatches acknowledged within contractual time windows.
6. **`qry_PriorityQueueOrder`:** Ranks incoming repair orders according to clinical urgency (Priority 1: Critical Care life-support pre-empting routine equipment).

---

## 4. How to Initialize & Run in Microsoft Access

1. **Create Database:** Open Microsoft Access and create a blank desktop database named `HBMS.accdb`.
2. **Execute Schema:** Navigate to `Create` $\rightarrow$ `Query Design`, switch to `SQL View`, paste the contents of [`schema.sql`](./schema.sql), and click `Run`.
3. **Load Demonstration Data:** Paste the contents of [`synthetic_dataset.sql`](./synthetic_dataset.sql) in SQL View and click `Run`.
4. **Save Queries:** Open separate query windows, paste each query from [`queries.sql`](./queries.sql), and save them with their respective query names (`qry_ActiveBreakdowns`, `qry_OverdueCriticalCareBreakdowns`, etc.).
5. **Test Escalation Query:** Run `qry_OverdueCriticalCareBreakdowns`; verify that Incident `BRK-2026-003` (Surgical Anaesthesia Ventilator) is returned as an overdue SLA breach qualifying for immediate escalation.
