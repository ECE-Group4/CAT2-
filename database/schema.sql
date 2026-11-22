-- =============================================================================
-- ECE 2423: Advanced Programming - Group 4
-- Academic Year: 2026/2027 | Kirinyaga University
-- Lecturer: Dr. Stephen Kahara Wanjau, PhD
-- Case Study: County Referral Hospital Biomedical Equipment Maintenance & SLA System
-- Database: HBMS.accdb (Microsoft Access / ANSI SQL Relational Schema)
-- Tracking Issue: Issue #5: [TASK-2.1] Microsoft Access Database Schema & Data Dictionary Design
-- =============================================================================

-- =============================================================================
-- 1. DROP EXISTING TABLES (In reverse dependency order)
-- =============================================================================
DROP TABLE IF EXISTS EscalationLogs;
DROP TABLE IF EXISTS RepairCosts;
DROP TABLE IF EXISTS JobSpareParts;
DROP TABLE IF EXISTS SpareParts;
DROP TABLE IF EXISTS MaintenanceJobs;
DROP TABLE IF EXISTS BreakdownLogs;
DROP TABLE IF EXISTS VendorSLAs;
DROP TABLE IF EXISTS Vendors;
DROP TABLE IF EXISTS Assets;
DROP TABLE IF EXISTS Wards;
DROP TABLE IF EXISTS Users;

-- =============================================================================
-- 2. CREATE ENTITY TABLES
-- =============================================================================

-- -----------------------------------------------------------------------------
-- Entity: Users
-- Description: Stores user accounts, role-based access credentials and profiles.
-- -----------------------------------------------------------------------------
CREATE TABLE Users (
    UserID AUTOINCREMENT PRIMARY KEY,
    FullName VARCHAR(100) NOT NULL,
    Username VARCHAR(30) NOT NULL UNIQUE,
    PasswordHash VARCHAR(128) NOT NULL,
    Role VARCHAR(30) NOT NULL,              -- 'Biomedical Engineer', 'Ward Personnel', 'Hospital Administrator'
    ContactEmail VARCHAR(100) NULL,
    ContactPhone VARCHAR(20) NULL,
    IsActive YESNO DEFAULT TRUE NOT NULL,
    CreatedAt DATETIME DEFAULT NOW() NOT NULL
);

-- -----------------------------------------------------------------------------
-- Entity: Wards
-- Description: Maintains hospital clinical locations and department metadata.
-- -----------------------------------------------------------------------------
CREATE TABLE Wards (
    WardID AUTOINCREMENT PRIMARY KEY,
    WardName VARCHAR(50) NOT NULL UNIQUE,   -- e.g. 'Intensive Care Unit (ICU)'
    DepartmentName VARCHAR(50) NOT NULL,    -- e.g. 'Critical Care', 'Surgery', 'Emergency'
    LocationDescription VARCHAR(100) NULL,  -- e.g. 'Building B, 2nd Floor'
    BedCapacity INTEGER DEFAULT 0 NOT NULL,
    IsActive YESNO DEFAULT TRUE NOT NULL
);

-- -----------------------------------------------------------------------------
-- Entity: Assets
-- Description: Master register of biomedical equipment assets in the hospital.
-- -----------------------------------------------------------------------------
CREATE TABLE Assets (
    AssetID AUTOINCREMENT PRIMARY KEY,
    AssetCode VARCHAR(20) NOT NULL UNIQUE,   -- e.g. 'EQ-VENT-001'
    EquipmentName VARCHAR(100) NOT NULL,    -- e.g. 'Mechanical Life-Support Ventilator'
    EquipmentCategory VARCHAR(50) NOT NULL, -- 'Ventilators', 'Patient Monitors', 'Infusion Pumps', 'X-Ray Units'
    Manufacturer VARCHAR(50) NOT NULL,      -- e.g. 'Hamilton Medical', 'Mindray', 'GE Healthcare'
    Model VARCHAR(50) NOT NULL,             -- e.g. 'Hamilton-C6', 'ePM 12M'
    SerialNumber VARCHAR(50) NOT NULL UNIQUE,
    WardID INTEGER NOT NULL,
    Criticality VARCHAR(20) NOT NULL,       -- 'Critical Care', 'General Care'
    AssetStatus VARCHAR(20) NOT NULL,       -- 'Operational', 'Under Maintenance', 'Malfunctioning', 'Decommissioned'
    DateCommissioned DATETIME NOT NULL,
    PurchaseCost CURRENCY DEFAULT 0.00 NOT NULL,
    WarrantyExpiry DATETIME NULL,
    CONSTRAINT FK_Assets_Wards FOREIGN KEY (WardID) REFERENCES Wards (WardID)
);

-- -----------------------------------------------------------------------------
-- Entity: Vendors
-- Description: Contracted biomedical manufacturers and specialized service firms.
-- -----------------------------------------------------------------------------
CREATE TABLE Vendors (
    VendorID AUTOINCREMENT PRIMARY KEY,
    VendorName VARCHAR(100) NOT NULL UNIQUE,
    ContactPerson VARCHAR(50) NOT NULL,
    Phone VARCHAR(20) NOT NULL,
    Email VARCHAR(100) NOT NULL,
    PhysicalAddress VARCHAR(150) NULL,
    ContractRef VARCHAR(50) NULL,
    IsActive YESNO DEFAULT TRUE NOT NULL
);

-- -----------------------------------------------------------------------------
-- Entity: VendorSLAs
-- Description: Contractual Service Level Agreements defining response windows.
-- -----------------------------------------------------------------------------
CREATE TABLE VendorSLAs (
    SLAID AUTOINCREMENT PRIMARY KEY,
    VendorID INTEGER NOT NULL,
    SLAName VARCHAR(100) NOT NULL,           -- e.g. 'Critical Care 4-Hour Response SLA'
    EquipmentCategory VARCHAR(50) NOT NULL, -- Applicable category or 'All'
    ResponseHours DOUBLE DEFAULT 4.0 NOT NULL, -- Response deadline window (e.g. 4.0 Hours)
    ResolutionHours DOUBLE DEFAULT 24.0 NOT NULL,
    LowPowerSurchargeRate DOUBLE DEFAULT 0.0 NOT NULL,
    EffectiveFrom DATETIME NOT NULL,
    EffectiveTo DATETIME NOT NULL,
    IsActive YESNO DEFAULT TRUE NOT NULL,
    CONSTRAINT FK_VendorSLAs_Vendors FOREIGN KEY (VendorID) REFERENCES Vendors (VendorID)
);

-- -----------------------------------------------------------------------------
-- Entity: BreakdownLogs
-- Description: Records equipment failure incidents, triage, response times & status.
-- -----------------------------------------------------------------------------
CREATE TABLE BreakdownLogs (
    BreakdownID AUTOINCREMENT PRIMARY KEY,
    BreakdownRef VARCHAR(20) NOT NULL UNIQUE, -- e.g. 'BRK-2026-001'
    AssetID INTEGER NOT NULL,
    ReportedByUserID INTEGER NOT NULL,
    SLAID INTEGER NULL,                       -- Applicable SLA policy
    ReportedAt DATETIME DEFAULT NOW() NOT NULL,
    Severity VARCHAR(20) NOT NULL,            -- 'Critical', 'High', 'Medium', 'Low'
    Description LONGTEXT NOT NULL,
    ResponseDeadline DATETIME NOT NULL,       -- Calculated (ReportedAt + ResponseHours)
    ResponseAt DATETIME NULL,                 -- Official acknowledgment timestamp
    ResponseBy VARCHAR(50) NULL,              -- Name or ID of responder
    Status VARCHAR(20) NOT NULL,              -- 'Pending', 'Assigned', 'In Progress', 'ESCALATED', 'Closed'
    ClosedAt DATETIME NULL,
    CONSTRAINT FK_Breakdowns_Assets FOREIGN KEY (AssetID) REFERENCES Assets (AssetID),
    CONSTRAINT FK_Breakdowns_Users FOREIGN KEY (ReportedByUserID) REFERENCES Users (UserID),
    CONSTRAINT FK_Breakdowns_VendorSLAs FOREIGN KEY (SLAID) REFERENCES VendorSLAs (SLAID)
);

-- -----------------------------------------------------------------------------
-- Entity: MaintenanceJobs
-- Description: Tracks technical work orders, engineer dispatches & diagnostics.
-- -----------------------------------------------------------------------------
CREATE TABLE MaintenanceJobs (
    JobID AUTOINCREMENT PRIMARY KEY,
    BreakdownID INTEGER NOT NULL UNIQUE,      -- 1-to-1 work order per breakdown incident
    EngineerUserID INTEGER NOT NULL,
    AssignedAt DATETIME DEFAULT NOW() NOT NULL,
    StartedAt DATETIME NULL,
    CompletedAt DATETIME NULL,
    LaborHours DOUBLE DEFAULT 0.0 NOT NULL,
    HourlyRate CURRENCY DEFAULT 1500.00 NOT NULL, -- Internal biomedical engineer billing rate (KES)
    Diagnosis LONGTEXT NULL,
    WorkPerformed LONGTEXT NULL,
    VerificationNotes LONGTEXT NULL,
    JobStatus VARCHAR(20) NOT NULL,           -- 'Assigned', 'In Progress', 'Awaiting Parts', 'Completed'
    CONSTRAINT FK_Jobs_Breakdowns FOREIGN KEY (BreakdownID) REFERENCES BreakdownLogs (BreakdownID),
    CONSTRAINT FK_Jobs_Engineers FOREIGN KEY (EngineerUserID) REFERENCES Users (UserID)
);

-- -----------------------------------------------------------------------------
-- Entity: SpareParts
-- Description: Master catalog of medical replacement parts and stock inventory.
-- -----------------------------------------------------------------------------
CREATE TABLE SpareParts (
    PartID AUTOINCREMENT PRIMARY KEY,
    PartCode VARCHAR(20) NOT NULL UNIQUE,    -- e.g. 'PRT-VENT-O2'
    PartName VARCHAR(100) NOT NULL,          -- e.g. 'O2 Sensor Cell / Blender'
    EquipmentCategory VARCHAR(50) NOT NULL,
    Unit VARCHAR(20) NOT NULL,               -- 'Piece', 'Meter', 'Assembly Kit'
    QuantityAvailable INTEGER DEFAULT 0 NOT NULL,
    ReorderLevel INTEGER DEFAULT 2 NOT NULL,
    UnitCost CURRENCY DEFAULT 0.00 NOT NULL
);

-- -----------------------------------------------------------------------------
-- Entity: JobSpareParts
-- Description: Junction table tracking parts consumed during specific jobs.
-- -----------------------------------------------------------------------------
CREATE TABLE JobSpareParts (
    JobPartID AUTOINCREMENT PRIMARY KEY,
    JobID INTEGER NOT NULL,
    PartID INTEGER NOT NULL,
    QuantityUsed INTEGER DEFAULT 1 NOT NULL,
    UnitCostAtUse CURRENCY NOT NULL,         -- Historical price lock at time of usage
    CONSTRAINT FK_JobParts_Jobs FOREIGN KEY (JobID) REFERENCES MaintenanceJobs (JobID),
    CONSTRAINT FK_JobParts_Parts FOREIGN KEY (PartID) REFERENCES SpareParts (PartID)
);

-- -----------------------------------------------------------------------------
-- Entity: RepairCosts
-- Description: Comprehensive financial accounting itemizing job expenditures.
-- -----------------------------------------------------------------------------
CREATE TABLE RepairCosts (
    CostID AUTOINCREMENT PRIMARY KEY,
    JobID INTEGER NOT NULL,
    CostType VARCHAR(30) NOT NULL,           -- 'Internal Labor', 'Spare Parts', 'External Vendor Fee'
    Description VARCHAR(100) NOT NULL,
    Amount CURRENCY DEFAULT 0.00 NOT NULL,
    RecordedAt DATETIME DEFAULT NOW() NOT NULL,
    RecordedByUserID INTEGER NOT NULL,
    CONSTRAINT FK_Costs_Jobs FOREIGN KEY (JobID) REFERENCES MaintenanceJobs (JobID),
    CONSTRAINT FK_Costs_Users FOREIGN KEY (RecordedByUserID) REFERENCES Users (UserID)
);

-- -----------------------------------------------------------------------------
-- Entity: EscalationLogs
-- Description: Immutable audit trail of overdue critical-care breakdowns breaching SLA.
-- -----------------------------------------------------------------------------
CREATE TABLE EscalationLogs (
    EscalationID AUTOINCREMENT PRIMARY KEY,
    BreakdownID INTEGER NOT NULL,
    EscalatedAt DATETIME DEFAULT NOW() NOT NULL,
    EscalationLevel VARCHAR(30) NOT NULL,    -- 'Level-1 Executive Alert', 'Level-2 Director Escalation'
    Reason VARCHAR(150) NOT NULL,            -- e.g. '4-Hour Response Window Exceeded Without Acknowledgment'
    NotifiedTo VARCHAR(100) NOT NULL,        -- 'Biomedical Director, Hospital Administrator'
    NotificationStatus VARCHAR(20) NOT NULL, -- 'DISPATCHED', 'ACKNOWLEDGED'
    CONSTRAINT FK_Escalations_Breakdowns FOREIGN KEY (BreakdownID) REFERENCES BreakdownLogs (BreakdownID)
);

-- =============================================================================
-- 3. INDEXES FOR PERFORMANCE OPTIMIZATION
-- =============================================================================
CREATE INDEX IDX_Assets_Ward ON Assets (WardID);
CREATE INDEX IDX_Assets_Status ON Assets (AssetStatus);
CREATE INDEX IDX_Assets_Category ON Assets (EquipmentCategory);
CREATE INDEX IDX_Breakdowns_Asset ON BreakdownLogs (AssetID);
CREATE INDEX IDX_Breakdowns_Status ON BreakdownLogs (Status);
CREATE INDEX IDX_Breakdowns_ReportedAt ON BreakdownLogs (ReportedAt);
CREATE INDEX IDX_Breakdowns_Deadline ON BreakdownLogs (ResponseDeadline);
CREATE INDEX IDX_Jobs_Breakdown ON MaintenanceJobs (BreakdownID);
CREATE INDEX IDX_Escalations_Breakdown ON EscalationLogs (BreakdownID);
