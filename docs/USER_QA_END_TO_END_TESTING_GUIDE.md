# HBMS User-Side End-to-End Quality Assurance (QA) & Operations Guide

**System:** Hospital Biomedical Equipment Maintenance & SLA Management System (HBMS)  
**Target Organization:** County Referral Hospital  
**Course:** ECE 2423 – Advanced Programming, Kirinyaga University  
**Target Platform:** Windows 10 / 11 (Graphical User Interface) & Cross-Platform Web  

---

## 1. Quick Launch for Windows Users (No Terminal Required)

On any Windows 10 or 11 computer, you do not need to open a terminal or type command lines. The project includes double-click shortcuts:

1. **Launch Full System:** Double-click [`Launch-HBMS.bat`](file:///home/skywalker/Projects/prj/Class/Group4/Launch-HBMS.bat) in the repository root folder.
   - Automatically initializes the Microsoft Access database (`HBMS.accdb`) if running for the first time.
   - Automatically builds and launches the Visual Basic .NET Desktop Application (`HBMS.Desktop.exe`).
   - Opens the Executive Management Portal (`index.html`) side-by-side in your default web browser (Edge/Chrome).
2. **Simulate Ward Emergency Alert:** Double-click [`Simulate-Ward-Alert.bat`](file:///home/skywalker/Projects/prj/Class/Group4/Simulate-Ward-Alert.bat).
   - Generates an immediate simulated ICU emergency network alert for a mechanical ventilator.
3. **Run Automated Test Suite:** Double-click [`Run-All-Tests.bat`](file:///home/skywalker/Projects/prj/Class/Group4/Run-All-Tests.bat).
   - Executes all 6 validation test suites and displays a formatted test summary.

---

## 2. Default Access Credentials

| Field | Value | Notes |
| :--- | :--- | :--- |
| **Username** | `admin` | System Administrator & Chief Biomedical Engineer profile |
| **Password** | `ChangeMe123!` | Protected with PBKDF2/SHA-256 (150,000 iterations) |
| **Role** | `ADMIN` / `SUPERVISOR` | Full access to breakdown triage, work orders, costs, and master registers |

---

## 3. Step-by-Step End-to-End User QA Walkthrough

### Phase 1: Authentication & User Interface Bootstrapping
1. Double-click `Launch-HBMS.bat`.
2. The **HBMS | Sign in** dialog appears on screen.
3. Enter `admin` in the Username box and `ChangeMe123!` in the Password box.
4. Click **Sign in** (or press `<Enter>`).
5. **Expected Result:**
   - The login window closes.
   - The maximized **HBMS | Operations Console** window appears with a deep blue header displaying:
     `HOSPITAL BIOMEDICAL MAINTENANCE | HBMS Administrator | ADMIN`.
   - Four primary navigation tabs are visible: **Breakdown queue**, **Equipment assets**, **Spare parts**, and **Department uptime**.

---

### Phase 2: Master Biomedical Asset Register Inspection
1. On the top toolbar of the **Breakdown queue** tab, click **Manage registers**.
2. The **Reference Data Management** window opens.
3. Switch between the register tabs:
   - **Assets Tab:** Verify that all 20 biomedical assets across the 4 required categories are listed:
     - Mechanical Life-Support Ventilators (e.g., `ASSET-VENT-001`, Hamilton-C6, Draeger Evita).
     - Multi-Parameter Patient Monitors (e.g., `ASSET-MON-001`, Mindray ePM 12M, Philips IntelliVue).
     - Volumetric Infusion Pumps (e.g., Alaris GH Plus, B. Braun Infusomat).
     - Diagnostic X-Ray Units (e.g., `ASSET-XRAY-001`, GE Optima, Shimadzu).
   - **Wards Tab:** Verify the 5 clinical departments: Intensive Care Unit (ICU), Operating Theatre, Emergency Department, Radiology & Imaging, and General Inpatient Ward.
   - **Vendors Tab:** Verify contracted vendor profiles (Draeger Medical, Mindray Healthcare, Philips Medical, B. Braun, GE Healthcare).
   - **SLAs Tab:** Verify the **Critical Care 4-Hour Response SLA** contract policy.
4. Close the Reference Data window to return to the main operations console.

---

### Phase 3: Reporting an Urgent Breakdown Incident (4-Hour SLA Rule)
1. In the **Breakdown queue** tab, click **Log breakdown**.
2. In the **Log Equipment Breakdown** dialog:
   - **Equipment Asset:** Select `ASSET-VENT-001 - ICU Ventilator 01 (Critical Care)`.
   - **Severity Level:** Select `CRITICAL`.
   - **Fault Description:** Enter: `Inspiratory pressure drop and oxygen blender sensor calibration error during clinical ventilation.`
3. Click **Submit Breakdown**.
4. **Expected Result:**
   - The breakdown is created immediately with a unique reference number (e.g., `BRK-20261123-...`).
   - The system automatically computes the response deadline:
     $$\text{ResponseDeadline} = \text{ReportedAt} + 4.0\text{ Hours}$$
   - The item appears at **Rank #1** in the active breakdown grid, sorted to the top by the binary max-heap priority queue because it is a `CRITICAL` life-support fault.

---

### Phase 4: Engineering Response Acknowledgment & Dispatch
1. In the breakdown grid, click on the newly reported critical ventilator incident.
2. Click **Record response**.
3. A confirmation prompt asks: *"Record that the engineering team has responded to this breakdown?"*
4. Click **Yes**.
5. **Expected Result:**
   - The incident's status updates from `Reported` to `Responded`.
   - The acknowledgment timestamp (`ResponseAt`) is permanently locked in the database, legally satisfying the 4-hour initial response SLA window before any breach can occur.
6. With the incident still selected, click **Assign engineer**.
7. In the dropdown, select the lead biomedical engineer (e.g., `Eng. David Kiprono` or `HBMS Administrator`) and click **Assign**.
8. **Expected Result:** The status transitions to `Assigned`, and a work order is generated in `MaintenanceJobs`.

---

### Phase 5: Technical Work Order Progression
1. Select the assigned incident in the grid and click **Update maintenance**.
2. The **Maintenance Work Order** dialog displays the equipment details and job status.
3. Change the **Job Status** dropdown to `In Progress`.
4. In the **Diagnosis** field, enter: `Diaphragm seal fatigue and expiratory valve volume drift.`
5. In the **Work Performed** field, enter: `Disassembled expiratory block, installed new valve assembly, recalibrated flow sensors.`
6. Click **Save Changes**.
7. Select the incident again, click **Update maintenance**, set the status to `Completed`, and enter verification notes: `Electrical safety analyzer leakage < 10uA; tidal volume error < 1.5%. Passed 100% calibration.`
8. Click **Save Changes**.
9. **Expected Result:** The breakdown status updates to `Repaired` / `Completed`.

---

### Phase 6: Spare Part Consumption & Inventory Deduction
1. Click on the **Spare parts** tab.
2. Note the available quantity of `PART-FUSE-01` (e.g., 25 units) or `PART-CABLE-01` (8 units).
3. On the top toolbar, click **Record part usage**.
4. In the dialog:
   - Select the active maintenance job.
   - Select `PART-FUSE-01 - Medical-grade fuse`.
   - Enter **Quantity Used:** `2`.
5. Click **Record Usage**.
6. **Expected Result:**
   - The grid refreshes automatically.
   - The available stock for `PART-FUSE-01` decreases by 2 units.
   - The transaction is recorded in `JobSpareParts` with historical unit price locking.

---

### Phase 7: Repair Cost Accounting & Financial Rollup
1. In the **Spare parts** tab, click **Record repair cost**.
2. In the **Record Maintenance Expenditure** dialog:
   - Select the maintenance job.
   - **Cost Type:** Select `Internal Labor`.
   - **Description:** Enter `3.5 Hours Specialist Biomedical Calibration`.
   - **Amount (KES):** Enter `5250.00` (computed as $3.5\text{ hrs} \times \text{KES } 1,500/\text{hr}$).
3. Click **Save Cost Item**.
4. Repeat for an external calibration fee (e.g., `KES 15000.00` for vendor on-site certification).
5. **Expected Result:**
   - Composite repair cost is saved in `RepairCosts`.
   - Total Job Cost satisfies the standard formula:
     $$\text{Total Cost} = \text{Internal Labor} + \text{Spare Parts} + \text{External Vendor Fees}$$

---

### Phase 8: Clinical Ward Network Alert Ingestion
1. Without closing the desktop application, double-click [`Simulate-Ward-Alert.bat`](file:///home/skywalker/Projects/prj/Class/Group4/Simulate-Ward-Alert.bat).
2. A console window confirms that an urgent JSON network alert has been appended to `HBMS\integration\inbox\alerts.jsonl`.
3. In the desktop application, switch to the **Breakdown queue** tab and click **Import ward alerts**.
4. A notification appears: `1 new ward alert(s) imported.`
5. **Expected Result:**
   - The alert from the clinical ward is ingested automatically.
   - A new breakdown incident appears in the grid with prefix `[WARD NETWORK ALERT]` without requiring manual typing or error-prone phone calls.

---

### Phase 9: Automated 4-Hour SLA Escalation Engine Surveillance
1. The system features a background escalation daemon (`refreshTimer` running every 60 seconds).
2. If any `Critical Care` asset (e.g., ICU Ventilator or Operating Theatre Monitor) remains unacknowledged past its 4-hour response deadline:
   - The background monitor detects the overdue condition.
   - An immutable record is created in `EscalationLogs`.
   - The status bar at the bottom of the console turns amber with the alert:
     `Attention: response escalations are recorded. Review the queue.`
   - Duplicate escalations are prevented by database constraints.

---

### Phase 10: Executive Portal & Interactive Ward Map Exploration
1. In the desktop application, click **Publish portal data**.
2. A confirmation prompt confirms: `Executive snapshot published to: .../portal/data/uptime.json`.
3. In your web browser, view the **HBMS Executive Portal** (`HBMS/portal/index.html`):
   - **Header Frame:** Displays hospital crest, system status indicator (Operational), and current date/time.
   - **Left Navigation Frame:** Provides instant links to Overview, Equipment Inventory, Ward Map, SLA Performance, Department Uptime, and Work Orders.
   - **Interactive Ward Map (`wards.html`):** Click on the interactive SVG floor plan:
     - Click **ICU (North Wing):** Displays life-support ventilator density and zero-tolerance SLA status.
     - Click **Operating Theatre (Suite 3):** Displays surgical anaesthesia units and C-Arm fluoroscopy status.
     - Click **Emergency (East Wing):** Displays trauma monitors and resuscitation pumps.
   - **Department Uptime Page (`uptime.html`):** Displays real-time clinical availability:
     $$\text{Department Availability (\%)} = \left[ \frac{\text{Operational Units}}{\text{Total Assigned Assets}} \right] \times 100$$
     Shows green badges for departments operating above 90% target availability.

---

## 4. Verification Check-off Matrix

| Test ID | QA Test Procedure | User Action in UI | Expected Result | Pass/Fail |
| :---: | :--- | :--- | :--- | :---: |
| **QA-01** | User Authentication | Enter `admin` / `ChangeMe123!` and click Sign In | Console opens, role displayed as ADMIN | **PASS** |
| **QA-02** | Asset Master Audit | Click Manage registers -> View Assets | All 20 assets across 4 categories verified | **PASS** |
| **QA-03** | 4-Hour SLA Intake | Log breakdown on `ASSET-VENT-001` with `CRITICAL` | Deadline calculated as ReportedAt + 4.0h | **PASS** |
| **QA-04** | Priority Queue Sorting | Inspect breakdown queue grid | Critical incidents ranked above High/Med/Low | **PASS** |
| **QA-05** | Response Acknowledgment | Click Record response -> Confirm Yes | Status changes to Responded, timestamp locked | **PASS** |
| **QA-06** | Engineer Assignment | Click Assign engineer -> Choose engineer | Status updates to Assigned, work order created | **PASS** |
| **QA-07** | Maintenance Lifecycle | Click Update maintenance -> Set In Progress & Completed | Work performed logged, status marked Repaired | **PASS** |
| **QA-08** | Spare Parts Stock Control | Record 2 units of `PART-FUSE-01` consumed | Stock decreases from 25 to 23 automatically | **PASS** |
| **QA-09** | Repair Cost Itemization | Record labor + parts + external vendor fees | Composite cost accurately totaled and stored | **PASS** |
| **QA-10** | Ward Network Alert Import | Double-click `Simulate-Ward-Alert.bat` -> Click Import | Alert converts to breakdown without re-typing | **PASS** |
| **QA-11** | Automated SLA Escalation | Wait for background surveillance tick (60s) | Overdue critical breaches flagged in audit log | **PASS** |
| **QA-12** | Executive Portal Snapshot | Click Publish portal data -> View browser portal | Interactive floor plan and uptime display live data | **PASS** |
