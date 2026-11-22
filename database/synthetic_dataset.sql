-- =============================================================================
-- ECE 2423: Advanced Programming - Group 4
-- Academic Year: 2026/2027 | Kirinyaga University
-- Lecturer: Dr. Stephen Kahara Wanjau, PhD
-- Case Study: County Referral Hospital Biomedical Equipment Maintenance & SLA System
-- Database: HBMS.accdb (Synthetic Demonstration Dataset)
-- Tracking Issue: Issue #7: [TASK-2.3] MS Access SQL Queries, Data Views & Synthetic Demonstration Dataset
-- =============================================================================

-- =============================================================================
-- 1. SEED DATA: Wards (5 Clinical Departments)
-- =============================================================================
INSERT INTO Wards (WardID, WardName, DepartmentName, LocationDescription, BedCapacity, IsActive) VALUES
(1, 'Intensive Care Unit (ICU)', 'Critical Care', 'Building A, 2nd Floor, Wing North', 15, TRUE),
(2, 'Operating Theatre (OT)', 'Surgical Services', 'Building A, 1st Floor, Suite 3', 8, TRUE),
(3, 'Emergency Department (ED)', 'Emergency Care', 'Ground Floor, Main Entrance East', 25, TRUE),
(4, 'Radiology & Imaging', 'Diagnostic Services', 'Building B, Ground Floor, Wing West', 10, TRUE),
(5, 'General Inpatient Ward', 'Internal Medicine', 'Building C, Floors 2 & 3', 60, TRUE);

-- =============================================================================
-- 2. SEED DATA: Users (Role-Based Personnel)
-- =============================================================================
INSERT INTO Users (UserID, FullName, Username, PasswordHash, Role, ContactEmail, ContactPhone, IsActive, CreatedAt) VALUES
(1, 'Eng. David Kiprono', 'dkiprono', 'ef92b778bafe771e89245b89ecbc08a44a4e166c06659911881f383d4473e94f', 'Biomedical Engineer', 'dkiprono@referral.health.go.ke', '+254711002233', TRUE, #2026-01-10 08:00:00#),
(2, 'Eng. Mercy Wambui', 'mwambui', 'ef92b778bafe771e89245b89ecbc08a44a4e166c06659911881f383d4473e94f', 'Biomedical Engineer', 'mwambui@referral.health.go.ke', '+254722114455', TRUE, #2026-01-12 08:30:00#),
(3, 'Sister Beatrice Achieng', 'bachieng', 'ef92b778bafe771e89245b89ecbc08a44a4e166c06659911881f383d4473e94f', 'Ward Personnel', 'bachieng@referral.health.go.ke', '+254733225566', TRUE, #2026-02-01 07:15:00#),
(4, 'Dr. Samuel K. Mutua', 'smutua', 'ef92b778bafe771e89245b89ecbc08a44a4e166c06659911881f383d4473e94f', 'Hospital Administrator', 'smutua@referral.health.go.ke', '+254700998877', TRUE, #2026-01-05 09:00:00#);

-- =============================================================================
-- 3. SEED DATA: Vendors (5 Medical Equipment Suppliers)
-- =============================================================================
INSERT INTO Vendors (VendorID, VendorName, ContactPerson, Phone, Email, PhysicalAddress, ContractRef, IsActive) VALUES
(1, 'Draeger Medical East Africa Ltd', 'Alex Kamau', '+254720100100', 'service.ke@draeger.com', 'Mombasa Road, Apex Business Park, Nairobi', 'KNY-MED-2025-01', TRUE),
(2, 'Mindray Healthcare Kenya', 'Grace Njoroge', '+254721200200', 'service@mindray.co.ke', 'Westlands Commercial Center, Nairobi', 'KNY-MED-2025-04', TRUE),
(3, 'Philips Medical Systems Kenya', 'Dennis Omondi', '+254722300300', 'dispatch@philips.co.ke', 'Upper Hill, Landmark Plaza, Nairobi', 'KNY-MED-2025-07', TRUE),
(4, 'B. Braun Medical East Africa', 'Fatuma Hassan', '+254723400400', 'support@bbraun.co.ke', 'Industrial Area, Commercial Street, Nairobi', 'KNY-MED-2025-09', TRUE),
(5, 'GE Healthcare Africa', 'Brian Rotich', '+254724500500', 'emea.service@ge.com', 'Gigiri, Courtyard Block B, Nairobi', 'KNY-MED-2025-11', TRUE);

-- =============================================================================
-- 4. SEED DATA: VendorSLAs (Contractual Service Level Agreements)
-- =============================================================================
INSERT INTO VendorSLAs (SLAID, VendorID, SLAName, EquipmentCategory, ResponseHours, ResolutionHours, LowPowerSurchargeRate, EffectiveFrom, EffectiveTo, IsActive) VALUES
(1, 1, 'Critical Care Ventilator 4-Hour Response SLA', 'Ventilators', 4.0, 24.0, 0.05, #2026-01-01 00:00:00#, #2027-12-31 23:59:59#, TRUE),
(2, 2, 'ICU & Emergency Patient Monitor 4-Hour SLA', 'Patient Monitors', 4.0, 18.0, 0.05, #2026-01-01 00:00:00#, #2027-12-31 23:59:59#, TRUE),
(3, 3, 'Cardiac Monitor & Diagnostic 8-Hour SLA', 'Patient Monitors', 8.0, 36.0, 0.00, #2026-01-01 00:00:00#, #2027-12-31 23:59:59#, TRUE),
(4, 4, 'Infusion Pump Standard 12-Hour SLA', 'Infusion Pumps', 12.0, 48.0, 0.00, #2026-01-01 00:00:00#, #2027-12-31 23:59:59#, TRUE),
(5, 5, 'Radiology & X-Ray Specialized 6-Hour SLA', 'X-Ray Units', 6.0, 30.0, 0.08, #2026-01-01 00:00:00#, #2027-12-31 23:59:59#, TRUE);

-- =============================================================================
-- 5. SEED DATA: Assets (20 Assets across 4 Required Categories)
-- Categories: Ventilators (5), Patient Monitors (5), Infusion Pumps (5), X-Ray Units (5)
-- =============================================================================
INSERT INTO Assets (AssetID, AssetCode, EquipmentName, EquipmentCategory, Manufacturer, Model, SerialNumber, WardID, Criticality, AssetStatus, DateCommissioned, PurchaseCost, WarrantyExpiry) VALUES
-- Ventilators (5 units)
(1, 'EQ-VENT-001', 'Intensive Care Mechanical Ventilator', 'Ventilators', 'Hamilton Medical', 'Hamilton-C6', 'SN-HAM-9011', 1, 'Critical Care', 'Operational', #2025-03-15#, 3200000.00, #2027-03-15#),
(2, 'EQ-VENT-002', 'High-Frequency Neonatal Ventilator', 'Ventilators', 'Draeger Medical', 'Evita V800', 'SN-DRA-4421', 1, 'Critical Care', 'Operational', #2025-04-10#, 3500000.00, #2027-04-10#),
(3, 'EQ-VENT-003', 'Emergency Transport Ventilator', 'Ventilators', 'Hamilton Medical', 'Hamilton-T1', 'SN-HAM-1088', 3, 'Critical Care', 'Operational', #2025-06-20#, 1800000.00, #2027-06-20#),
(4, 'EQ-VENT-004', 'Surgical Anaesthesia Ventilator Unit', 'Ventilators', 'Draeger Medical', 'Perseus A500', 'SN-DRA-7732', 2, 'Critical Care', 'Under Maintenance', #2025-01-18#, 4100000.00, #2027-01-18#),
(5, 'EQ-VENT-005', 'ICU Non-Invasive BiPAP Ventilator', 'Ventilators', 'Philips Healthcare', 'V60 Plus', 'SN-PHI-5519', 1, 'Critical Care', 'Operational', #2025-08-05#, 1600000.00, #2027-08-05#),

-- Patient Monitors (5 units)
(6, 'EQ-MON-001', 'Multi-Parameter ICU Patient Monitor', 'Patient Monitors', 'Mindray', 'ePM 12M', 'SN-MIN-3301', 1, 'Critical Care', 'Operational', #2025-02-14#, 650000.00, #2027-02-14#),
(7, 'EQ-MON-002', 'Anaesthesia Vital Signs Monitor', 'Patient Monitors', 'Philips Healthcare', 'IntelliVue MX40', 'SN-PHI-8820', 2, 'Critical Care', 'Operational', #2025-02-20#, 850000.00, #2027-02-20#),
(8, 'EQ-MON-003', 'Emergency Trauma Bedside Monitor', 'Patient Monitors', 'Mindray', 'BeneVision N15', 'SN-MIN-4411', 3, 'Critical Care', 'Under Maintenance', #2025-05-12#, 720000.00, #2027-05-12#),
(9, 'EQ-MON-004', 'Continuous Cardiac Telemetry Monitor', 'Patient Monitors', 'Philips Healthcare', 'IntelliVue MP50', 'SN-PHI-9014', 1, 'Critical Care', 'Operational', #2025-07-22#, 920000.00, #2027-07-22#),
(10, 'EQ-MON-005', 'General Ward Bedside Vitals Monitor', 'Patient Monitors', 'Mindray', 'VS-900', 'SN-MIN-1123', 5, 'General Care', 'Operational', #2025-09-01#, 380000.00, #2027-09-01#),

-- Infusion Pumps (5 units)
(11, 'EQ-PUMP-001', 'Volumetric Syringe Infusion Pump', 'Infusion Pumps', 'B. Braun', 'Infusomat Space', 'SN-BBR-6011', 1, 'General Care', 'Operational', #2025-03-01#, 240000.00, #2027-03-01#),
(12, 'EQ-PUMP-002', 'Dual-Channel Micro-Infusion Pump', 'Infusion Pumps', 'Becton Dickinson', 'Alaris GH Plus', 'SN-BD-3390', 1, 'General Care', 'Operational', #2025-03-05#, 280000.00, #2027-03-05#),
(13, 'EQ-PUMP-003', 'Operating Theatre Anaesthetic Infuser', 'Infusion Pumps', 'B. Braun', 'Perfusor Space', 'SN-BBR-7712', 2, 'General Care', 'Operational', #2025-04-12#, 260000.00, #2027-04-12#),
(14, 'EQ-PUMP-004', 'Emergency Resuscitation Syringe Driver', 'Infusion Pumps', 'Becton Dickinson', 'Alaris CC Plus', 'SN-BD-9021', 3, 'General Care', 'Under Maintenance', #2025-06-18#, 250000.00, #2027-06-18#),
(15, 'EQ-PUMP-005', 'General Inpatient Ward Infusion Drip', 'Infusion Pumps', 'B. Braun', 'Infusomat Compact', 'SN-BBR-1189', 5, 'General Care', 'Operational', #2025-07-30#, 180000.00, #2027-07-30#),

-- X-Ray Units (5 units)
(16, 'EQ-XRAY-001', 'Digital Mobile Ward Radiography Unit', 'X-Ray Units', 'GE Healthcare', 'Optima XR220amx', 'SN-GE-5510', 4, 'General Care', 'Operational', #2024-11-10#, 6500000.00, #2026-11-10#),
(17, 'EQ-XRAY-002', 'Operating Theatre C-Arm Fluoroscopy', 'X-Ray Units', 'GE Healthcare', 'OEC One CFD', 'SN-GE-8842', 2, 'Critical Care', 'Operational', #2025-01-25#, 8200000.00, #2027-01-25#),
(18, 'EQ-XRAY-003', 'Emergency Portable Trauma X-Ray', 'X-Ray Units', 'Shimadzu', 'MobileDaRt Evolution', 'SN-SHI-2219', 3, 'General Care', 'Operational', #2025-04-14#, 5900000.00, #2027-04-14#),
(19, 'EQ-XRAY-004', 'Stationary Ceiling-Suspended Bucky X-Ray', 'X-Ray Units', 'GE Healthcare', 'Discovery XR656', 'SN-GE-9021', 4, 'General Care', 'Operational', #2024-08-15#, 9500000.00, #2026-08-15#),
(20, 'EQ-XRAY-005', 'Orthopaedic Mobile Imaging Scanner', 'X-Ray Units', 'Shimadzu', 'MobileArt Plus', 'SN-SHI-6701', 4, 'General Care', 'Operational', #2025-02-11#, 4800000.00, #2027-02-11#);

-- =============================================================================
-- 6. SEED DATA: SpareParts (10 Master Inventory Items)
-- =============================================================================
INSERT INTO SpareParts (PartID, PartCode, PartName, EquipmentCategory, Unit, QuantityAvailable, ReorderLevel, UnitCost) VALUES
(1, 'PRT-VENT-O2', 'Galvanic O2 Sensor Cell & Fuel Module', 'Ventilators', 'Piece', 6, 2, 35000.00),
(2, 'PRT-VENT-EXP', 'Expiratory Valve Assembly & Diaphragm', 'Ventilators', 'Assembly Kit', 4, 2, 48000.00),
(3, 'PRT-VENT-FLT', 'HEPA Antibacterial Air Intake Filter Kit', 'Ventilators', 'Kit', 12, 4, 9500.00),
(4, 'PRT-MON-SPO2', 'Reusable Adult Silicon SpO2 Sensor Clip', 'Patient Monitors', 'Piece', 15, 5, 8500.00),
(5, 'PRT-MON-ECG', '5-Lead ECG Trunk Cable & Leadwires', 'Patient Monitors', 'Piece', 8, 3, 12000.00),
(6, 'PRT-MON-BAT', 'Rechargeable Lithium-Ion Battery Pack', 'Patient Monitors', 'Piece', 5, 2, 18500.00),
(7, 'PRT-PUMP-BAT', 'Internal Backup Battery 12V 2.2Ah', 'Infusion Pumps', 'Piece', 7, 3, 7500.00),
(8, 'PRT-PUMP-PRS', 'Upstream Occlusion Pressure Sensor', 'Infusion Pumps', 'Piece', 3, 2, 22000.00),
(9, 'PRT-XRAY-TUB', 'High-Voltage X-Ray Generator Anode Tube', 'X-Ray Units', 'Piece', 1, 1, 450000.00),
(10, 'PRT-XRAY-COL', 'Motorized Manual Radiographic Collimator', 'X-Ray Units', 'Piece', 2, 1, 180000.00);

-- =============================================================================
-- 7. SEED DATA: BreakdownLogs (Mix of Historical, Active, and Overdue SLA Breaches)
-- =============================================================================
INSERT INTO BreakdownLogs (BreakdownID, BreakdownRef, AssetID, ReportedByUserID, SLAID, ReportedAt, Severity, Description, ResponseDeadline, ResponseAt, ResponseBy, Status, ClosedAt) VALUES
-- Incident 1: Historical Resolved Ventilator Fault (Closed)
(1, 'BRK-2026-001', 1, 3, 1, #2026-11-10 09:00:00#, 'Critical', 'Ventilator inspiratory pressure fluctuation and flow sensor calibration error.', #2026-11-10 13:00:00#, #2026-11-10 10:45:00#, 'Alex Kamau (Draeger)', 'Closed', #2026-11-10 16:30:00#),

-- Incident 2: Historical Resolved Monitor Fault (Closed)
(2, 'BRK-2026-002', 6, 3, 2, #2026-11-12 14:00:00#, 'High', 'ECG Lead II intermittent artifact and trace dropouts on patient monitor.', #2026-11-12 18:00:00#, #2026-11-12 15:30:00#, 'Eng. David Kiprono', 'Closed', #2026-11-12 17:45:00#),

-- Incident 3: Active Under Maintenance Ventilator (CRITICAL CARE BREACH - 4-Hour Rule Exceeded)
-- Reported 6 hours ago with NO response recorded; automatically qualifies for 4-Hour SLA Escalation!
(3, 'BRK-2026-003', 4, 3, 1, DATEADD('h', -6, NOW()), 'Critical', 'Surgical Anaesthesia Ventilator low oxygen pressure delivery during preoperative setup.', DATEADD('h', -2, NOW()), NULL, NULL, 'ESCALATED', NULL),

-- Incident 4: Active Under Maintenance Emergency Monitor (Pending within SLA window)
(4, 'BRK-2026-004', 8, 3, 2, DATEADD('n', -45, NOW()), 'High', 'Touchscreen digitizer unresponsive to user touches in Trauma bay.', DATEADD('n', 195, NOW()), NULL, NULL, 'Pending', NULL),

-- Incident 5: Active Under Maintenance Infusion Pump (Awaiting Parts)
(5, 'BRK-2026-005', 14, 3, 4, DATEADD('h', -5, NOW()), 'Medium', 'Persistent downstream occlusion false alarm despite clear cannula line.', DATEADD('h', 7, NOW()), DATEADD('h', -3, NOW()), 'Eng. Mercy Wambui', 'In Progress', NULL);

-- =============================================================================
-- 8. SEED DATA: MaintenanceJobs (Work Orders for Incidents)
-- =============================================================================
INSERT INTO MaintenanceJobs (JobID, BreakdownID, EngineerUserID, AssignedAt, StartedAt, CompletedAt, LaborHours, HourlyRate, Diagnosis, WorkPerformed, VerificationNotes, JobStatus) VALUES
(1, 1, 1, #2026-11-10 09:30:00#, #2026-11-10 10:45:00#, #2026-11-10 16:30:00#, 5.75, 1500.00, 'Expiratory flow diaphragm worn out causing volume drift.', 'Replaced expiratory valve assembly, recalibrated flow sensors, passed pressure tightness tests.', 'Passed 100% calibration, tidal volume error < 2%. Ready for clinical use.', 'Completed'),
(2, 2, 2, #2026-11-12 14:15:00#, #2026-11-12 15:30:00#, #2026-11-12 17:45:00#, 2.25, 1500.00, 'Damaged internal copper shield on 5-lead ECG trunk wire.', 'Replaced ECG cable with new OEM Mindray leadwire set.', 'Electrical safety analyzer verified leakage < 10uA. Clean rhythm trace.', 'Completed'),
(3, 5, 2, DATEADD('h', -4, NOW()), DATEADD('h', -3, NOW()), NULL, 1.50, 1500.00, 'Occlusion pressure sensor drift requiring physical replacement.', 'Disassembled chassis, ordered replacement pressure sensor PRT-PUMP-PRS from workshop stock.', NULL, 'Awaiting Parts');

-- =============================================================================
-- 9. SEED DATA: JobSpareParts (Parts Consumed on Completed Jobs)
-- =============================================================================
INSERT INTO JobSpareParts (JobPartID, JobID, PartID, QuantityUsed, UnitCostAtUse) VALUES
(1, 1, 2, 1, 48000.00), -- Expiratory Valve for Ventilator
(2, 1, 3, 1, 9500.00),  -- HEPA Filter for Ventilator
(3, 2, 5, 1, 12000.00); -- ECG Cable for Patient Monitor

-- =============================================================================
-- 10. SEED DATA: RepairCosts (Financial Accounting Entries)
-- =============================================================================
INSERT INTO RepairCosts (CostID, JobID, CostType, Description, Amount, RecordedAt, RecordedByUserID) VALUES
(1, 1, 'Internal Labor', '5.75 Engineering Labor Hours @ KES 1,500/hr', 8625.00, #2026-11-10 16:35:00#, 1),
(2, 1, 'Spare Parts', 'Expiratory Valve Assembly + HEPA Filter Kit', 57500.00, #2026-11-10 16:36:00#, 1),
(3, 1, 'External Vendor Fee', 'Draeger On-Site Calibration & Safety Certification Fee', 25000.00, #2026-11-10 16:40:00#, 1),
(4, 2, 'Internal Labor', '2.25 Engineering Labor Hours @ KES 1,500/hr', 3375.00, #2026-11-12 17:50:00#, 2),
(5, 2, 'Spare Parts', '5-Lead ECG Trunk Cable Replacement', 12000.00, #2026-11-12 17:52:00#, 2);

-- =============================================================================
-- 11. SEED DATA: EscalationLogs (SLA Breach Audit for Incident 3)
-- =============================================================================
INSERT INTO EscalationLogs (EscalationID, BreakdownID, EscalatedAt, EscalationLevel, Reason, NotifiedTo, NotificationStatus) VALUES
(1, 3, DATEADD('h', -2, NOW()), 'Level-1 Executive Alert', '4-Hour Critical Response Window Exceeded: Surgical Ventilator Unattended', 'Biomedical Director, Hospital Medical Superintendent', 'DISPATCHED');
