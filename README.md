# Hospital Biomedical Equipment Maintenance & SLA Management System (HBMS)

**System Name:** HBMS (Hospital Biomedical Maintenance System)  
**Target Organization:** County Referral Hospital  
**Repository:** [https://github.com/ECE-Group4/CAT2-](https://github.com/ECE-Group4/CAT2-)  

---

## 1. System Overview

A County Referral Hospital relies on critical biomedical assets to sustain life-support, critical care, and diagnostic operations. Equipment downtime directly threatens patient survival, while external medical equipment vendors frequently breach contracted Service Level Agreements (SLAs).

The **Hospital Biomedical Equipment Maintenance & SLA Management System (HBMS)** is a comprehensive, standalone three-tier maintenance management platform connecting clinical hospital wards, the central biomedical engineering workshop, hospital administration, and external service vendors into an automated, accountable operational workflow.

### Core Biomedical Equipment Categories Managed:
1. **Mechanical Ventilators:** Life-support invasive and non-invasive ventilators (e.g., Hamilton-C6, Draeger Evita V800). *Priority: Critical Care (Statutory 4-Hour Response Rule).*
2. **Patient Monitors:** Multi-parameter vital signs monitors tracking ECG, SpO2, and arterial pressures (e.g., Mindray ePM 12M, Philips IntelliVue MX40). *Priority: Critical Care / High.*
3. **Infusion Pumps:** Volumetric syringe and drip infusion pumps delivering continuous intravenous medication (e.g., Alaris GH Plus, B. Braun Infusomat Space). *Priority: General Care / Medium.*
4. **X-Ray Units:** Mobile and stationary diagnostic radiography imaging systems (e.g., GE Optima XR220amx, Shimadzu MobileDaRt). *Priority: Diagnostic / Routine.*

---

## 2. Integrated System Architecture

HBMS is engineered upon a classical, decoupled 3-tier client/server architecture:

```text
[ Ward Client Terminals ]                [ Executive Management ]
   (Java AlertClient / Forms)               (HTML Multi-Frame Portal)
            |                                           ^
   TCP 5055 |                                           | HTTP 80 / Local
            v                                           |
+-------------------------------------------------------------------+
|               HBMS Application & Business Logic Tier              |
|                                                                   |
|   • Java TCP Socket Server Daemon (BiomedicalServer / Port 5055)   |
|   • In-Memory Priority Queue Engine (Clinical Urgency Sorting)    |
|   • Automated 4-Hour SLA Escalation Surveillance Engine           |
|   • Maintenance Cost Accounting Engine (Labor + Parts + Vendor)   |
|   • Durable JSON Lines Ingestion Bridge (alerts.jsonl)            |
+-------------------------------------------------------------------+
                                |
               ADO.NET / OleDb  |  (Microsoft.ACE.OLEDB.12.0)
                                v
+-------------------------------------------------------------------+
|                   Relational Persistence Tier                     |
|                                                                   |
|   • Microsoft Access Relational Database (HBMS.accdb)             |
|   • 11 Normalized Tables in Third Normal Form (3NF)               |
|   • Primary / Foreign Key Referential Integrity                   |
|   • Pre-compiled Reporting Queries & Audit Views                  |
+-------------------------------------------------------------------+
                                ^
                                | Parameterized SQL
[ Biomedical Engineering Workstations ]
   (VB.NET Windows Forms Desktop Client - HBMS.Desktop)
```

---

## 3. Platform Modules & Component Guide

### 3.1 Biomedical Engineer Desktop Application (`HBMS/desktop/HBMS.Desktop/`)
Windows Forms application targeting .NET 8 with parameterized OleDb data access:
- **Authentication & Roles (`LoginForm.vb`):** Secure login supporting Biomedical Engineers, Ward Staff, and Administrators.
- **Operational Command Center (`MainForm.vb`):** Status cards for active breakdowns, critical alarms, live SLA countdown timer, and background poller.
- **Biomedical Asset Master (`ReferenceDataForm.vb`):** Full CRUD cataloging of medical assets, clinical departments, vendors, and contractual SLA terms.
- **Breakdown Intake & Triage (`BreakdownForm.vb`):** Immediate incident intake with automatic calculation of the 4-hour response deadline for critical-care devices.
- **Priority Queue Dispatcher (`PriorityQueueService.vb`):** In-memory priority queue sorting work orders by clinical urgency:
  $$\text{Priority 1 (Critical Care)} \rightarrow \text{Priority 2 (High)} \rightarrow \text{Priority 3 (Medium)} \rightarrow \text{Priority 4 (Low)} \rightarrow \text{FIFO on ReportedAt}$$
- **Work Order Management (`MaintenanceUpdateForm.vb`):** Work order lifecycle tracking from `Assigned` $\rightarrow$ `In Progress` $\rightarrow$ `Awaiting Parts` $\rightarrow$ `Completed`.
- **Spare Parts Inventory (`SparePartUsageForm.vb`):** Inventory deduction and consumption logging per maintenance job.
- **Cost Accounting (`RepairCostForm.vb`):** Itemizes composite maintenance expenses:
  $$\text{Total Job Cost} = (\text{Labor Hours} \times \text{Hourly Rate}) + \sum (\text{Part Qty} \times \text{Unit Cost}) + \text{External Vendor Fees}$$
- **Defensive Safety Dialogs (`ConfirmDialog.vb`):** Custom modal alerts for life-critical equipment warnings and confirmation of irreversible actions.

### 3.2 Relational Database & Automation (`database/` and `HBMS/database/`)
Structured in Third Normal Form (3NF) to eliminate anomalies:
- **Relational Entities (11 Tables):** `Wards`, `Assets`, `Vendors`, `VendorSLAs`, `Users`, `BreakdownLogs`, `MaintenanceJobs`, `SpareParts`, `JobSpareParts`, `RepairCosts`, `EscalationLogs`.
- **Automated 4-Hour SLA Escalation:** When a breakdown is logged for critical-care equipment, the system enforces:
  $$\text{ResponseDeadline} = \text{ReportedAt} + 4.0\text{ Hours}$$
  If $\text{ResponseAt IS NULL}$ and $\text{CurrentTime} > \text{ResponseDeadline}$, the system automatically appends an audit event to `EscalationLogs` and raises executive alerts while preventing duplicate entries.
- **Pre-Compiled Reporting Queries (`database/queries.sql`):** Stored queries for active breakdowns, overdue critical incidents, departmental equipment uptime %, and composite job costs.
- **Automated Windows Bootstrapper (`Create-HBMSDatabase.ps1`):** PowerShell script creating the native `.accdb` file and loading synthetic demonstration data.

### 3.3 Java Network Alert Subsystem (`HBMS/java/`)
Pure Java 17+ client-server network implementation:
- **Server Daemon (`AlertReceiver.java` / `AlertApplication.java`):** Multi-threaded `ServerSocket` listening on port 5055 with an `ExecutorService` thread pool.
- **Durable Inbox Queuing:** Incoming JSON alert packets are validated and committed to a JSON Lines inbox (`integration/inbox/alerts.jsonl`) before returning structured acknowledgments:
  ```json
  {"messageId":"<UUID>","status":"RECEIVED"}
  ```
- **Ward Client (`AlertSender.java`):** Client socket transmitting urgent equipment failure alerts directly from ward terminals.
- **Desktop Ingestion Bridge (`AlertInboxImporter.vb`):** Consumes inbox events and creates database work orders without manual data re-entry.

### 3.4 HTML Executive Management Portal (`HBMS/portal/`)
Traditional multi-frame layout providing executive availability metrics:
- **Frame Layout (`index.html`):** Header frame (`header.html`), sidebar navigation (`navigation.html`), and main content frame.
- **Interactive Hospital Ward Floor Plan (`wards.html`):** Client-side image map with clickable hotspots for ICU, Operating Theatre, Emergency, Radiology, and General Wards.
- **Departmental Equipment Uptime (`uptime.html`):** Calculates and displays availability:
  $$\text{Uptime (\%)} = \left[ \frac{\text{Total Available Hours} - \text{Total Downtime}}{\text{Total Available Hours}} \right] \times 100$$
- **Equipment & SLA Catalog (`equipment.html`, `sla.html`, `overview.html`):** Tabular inventories, status badges, and vendor performance audits.

---

## 4. Build, Verification & Run Instructions

### 4.1 Running Automated Validation Tests (Linux / Windows)
From the project root:

```bash
# 1. Run HTML Portal Integrity Checks
bash HBMS/tests/run-portal-tests.sh

# 2. Run Java Alert Protocol Validation Smoke Tests
bash HBMS/tests/run-java-smoke.sh

# 3. Verify Portal JavaScript & Uptime Data Syntax
bash HBMS/scripts/verify-portal.sh
```

### 4.2 Running the Java TCP Alert Network Service
To start the workshop socket receiver:
```bash
cd HBMS/java/src/main/java
java org/hbms/alerts/AlertApplication.java --server --host 0.0.0.0 --port 5055 --inbox ../../../integration/inbox/alerts.jsonl
```

To broadcast an urgent equipment alert from a ward terminal:
```bash
cd HBMS/java/src/main/java
java org/hbms/alerts/AlertApplication.java --send --host 127.0.0.1 --port 5055 \
  --asset EQ-VENT-001 --ward ICU --severity CRITICAL \
  --description "Ventilator inspiratory pressure drop during ventilation"
```

### 4.3 Running the Windows Desktop Application & Access Database
On a Windows workstation with .NET 8 SDK and Access Database Engine (ACE OLEDB 12.0):
```powershell
# 1. Create the Access database and seed synthetic records
powershell -ExecutionPolicy Bypass -File .\HBMS\database\Create-HBMSDatabase.ps1

# 2. Build the VB.NET desktop solution
powershell -ExecutionPolicy Bypass -File .\HBMS\scripts\build-desktop.ps1

# 3. Launch the desktop executable
.\HBMS\desktop\HBMS.Desktop\bin\Release\net8.0-windows\HBMS.Desktop.exe
```
*Default initial login:* `admin` / `ChangeMe123!`

### 4.4 Viewing the Executive Web Portal
Open `HBMS/portal/index.html` in any web browser. Use the desktop application's snapshot feature to refresh `uptime.json` with live operational data.

---

## 5. Repository Structure

```text
├── database/                         # Relational schema DDL, queries, and data dictionary
│   ├── schema.sql                    # SQL DDL defining all 11 tables
│   ├── data_dictionary.md            # Attribute specifications, types, and constraints
│   ├── normalization_analysis.md     # 1NF to 3NF formal normalization study
│   ├── erd.md                        # Entity-Relationship Diagram (Crow's Foot notation)
│   ├── queries.sql                   # Stored Access SQL queries & views
│   ├── synthetic_dataset.sql         # Synthetic demonstration dataset
│   └── README.md                     # Database execution guide
│
├── docs/                             # Engineering documentation & blueprints
│   ├── analysis/                     # Requirements, DFDs, Context Diagram, 3-Tier Architecture
│   │   ├── SRS.md                    # Detailed Software Requirements Specification
│   │   ├── CONTEXT_DIAGRAM.md        # Level-0 Context Diagram & Information Flows
│   │   ├── DFD_LEVEL_1.md            # Functional Process Decomposition (1.0 to 6.0)
│   │   └── ARCHITECTURE_3TIER.md     # 3-Tier Physical & Logical Deployment Architecture
│   └── GROUP4_HBMS_TEAM_DESIGN_BLUEPRINT.md # Master System Design Blueprint
│
└── HBMS/                             # Standalone runnable project package
    ├── database/                     # Access schema and PowerShell bootstrapper
    ├── desktop/HBMS.Desktop/         # VB.NET Windows Forms solution (.sln, .vbproj, forms)
    ├── java/                         # Java TCP socket alert server and client
    ├── portal/                       # HTML frameset portal, CSS, SVG ward map, JS
    ├── integration/                  # Alert wire protocol & inbox contract
    ├── docs/                         # Technical guides, workflows, and diagrams
    ├── scripts/                      # Build and run scripts (PowerShell & Bash)
    └── tests/                        # Automated smoke and verification test suites
```
