# County Referral Hospital Biomedical Equipment Maintenance & SLA System (HBMS)

**Unit Code & Title:** ECE 2423 – Advanced Programming  
**Programme:** Bachelor of Technology in Electrical and Computer Engineering (Year IV, Semester I)  
**Academic Year:** 2026/2027  
**Institution:** Kirinyaga University — School of Pure and Applied Sciences  
**Lecturer:** Dr. Stephen Kahara Wanjau, PhD  
**Group:** Group 4  
**Assigned Case Study:** Group 4 — County Referral Hospital Biomedical Equipment Maintenance & SLA System  
**Repository:** [ECE-Group4/CAT2-](https://github.com/ECE-Group4/CAT2-)  

---

## 1. Project Overview
A County Referral Hospital relies on life-critical biomedical devices (**Patient Monitors**, **Infusion Pumps**, **X-Ray Units**, and **Ventilators**). Equipment downtime directly impacts patient survival, and external biomedical vendors frequently breach Service Level Agreements (SLAs).

The **Hospital Biomedical Equipment Maintenance & SLA Management System (HBMS)** provides an integrated, multi-tier software prototype connecting clinical wards, the biomedical engineering workshop, hospital administration, and external vendors into an accountable, automated maintenance workflow.

---

## 2. Assessment Rubric Traceability (100 Marks)

| Assessment Criterion | Marks | Primary Deliverables & Locations |
|---|---|---|
| **1. Software Analysis, Design & Documentation** | **20** | SRS, Context Diagram, DFD Level-1, 3-Tier Architecture in [`docs/analysis/`](./docs/analysis/) |
| **2. Relational Database & Macro Automation** | **25** | MS Access Relational Schema, 3NF Normalization, ERD, Queries, Dataset in [`database/`](./database/) |
| **3. Visual Basic GUI & Logic Implementation** | **25** | Multi-form VB.NET application, In-memory Priority Queue, Custom Modal Dialogs, Cost accounting |
| **4. HTML Web Portal & Java Network Prototype** | **30** | Multi-frame portal, Interactive Ward Image Map, Java TCP Socket Server & Client |
| **TOTAL** | **100** | Full compliance with ECE 2423 CAT 2 requirements |

---

## 3. Project Structure

```
├── database/
│   ├── schema.sql                   # Complete MS Access DDL script (11 normalized tables)
│   ├── data_dictionary.md           # Technical data dictionary for all entities & fields
│   ├── normalization_analysis.md    # Formal 1NF to 3NF normalization study & anomaly proofs
│   ├── erd.md                       # Crow's Foot Entity-Relationship Diagram & cardinalities
│   ├── queries.sql                  # Pre-compiled MS Access SQL queries & views
│   ├── synthetic_dataset.sql        # Comprehensive realistic dataset (5 wards, 20 assets, vendors, logs)
│   └── README.md                    # Database execution & verification guide
│
├── docs/
│   └── analysis/
│       ├── SRS.md                   # Software Requirements Specification (FR-01 to FR-13, NFRs, Use Cases)
│       ├── CONTEXT_DIAGRAM.md       # Level-0 Context Diagram & External Entity Flows
│       ├── DFD_LEVEL_1.md           # DFD Level-1 & Functional Process Decomposition (1.0 to 6.0)
│       └── ARCHITECTURE_3TIER.md    # 3-Tier Client/Server Deployment Model & Network Topology
│
└── README.md                        # Project master documentation (this file)
```

---

## 4. Completed Tasks & Feature Branches

The following tasks have been completed and published to dedicated feature branches on GitHub:

| Issue | Branch Name | Task Title | Key Deliverable |
|---|---|---|---|
| [#1](https://github.com/ECE-Group4/CAT2-/issues/1) | `task-1.1-srs-use-cases` | **[TASK-1.1] Software Requirements Specification (SRS)** | [`docs/analysis/SRS.md`](./docs/analysis/SRS.md) |
| [#2](https://github.com/ECE-Group4/CAT2-/issues/2) | `task-1.2-context-diagram` | **[TASK-1.2] System Context Diagram (Level-0 DFD)** | [`docs/analysis/CONTEXT_DIAGRAM.md`](./docs/analysis/CONTEXT_DIAGRAM.md) |
| [#3](https://github.com/ECE-Group4/CAT2-/issues/3) | `task-1.3-dfd-level-1` | **[TASK-1.3] Data Flow Diagram (DFD Level-1)** | [`docs/analysis/DFD_LEVEL_1.md`](./docs/analysis/DFD_LEVEL_1.md) |
| [#4](https://github.com/ECE-Group4/CAT2-/issues/4) | `task-1.4-3tier-architecture` | **[TASK-1.4] 3-Tier Client/Server Architecture** | [`docs/analysis/ARCHITECTURE_3TIER.md`](./docs/analysis/ARCHITECTURE_3TIER.md) |
| [#5](https://github.com/ECE-Group4/CAT2-/issues/5) | `task-2.1-access-db-schema-data-dictionary` | **[TASK-2.1] MS Access Database Schema & Data Dictionary** | [`database/schema.sql`](./database/schema.sql), [`database/data_dictionary.md`](./database/data_dictionary.md) |
| [#6](https://github.com/ECE-Group4/CAT2-/issues/6) | `task-2.2-database-normalization-erd` | **[TASK-2.2] Normalization Analysis (1NF-3NF) & ERD** | [`database/normalization_analysis.md`](./database/normalization_analysis.md), [`database/erd.md`](./database/erd.md) |
| [#7](https://github.com/ECE-Group4/CAT2-/issues/7) | `task-2.3-access-sql-queries-synthetic-dataset` | **[TASK-2.3] MS Access SQL Queries & Demonstration Dataset** | [`database/queries.sql`](./database/queries.sql), [`database/synthetic_dataset.sql`](./database/synthetic_dataset.sql) |
