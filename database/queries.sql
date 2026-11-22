-- =============================================================================
-- ECE 2423: Advanced Programming - Group 4
-- Academic Year: 2026/2027 | Kirinyaga University
-- Lecturer: Dr. Stephen Kahara Wanjau, PhD
-- Case Study: County Referral Hospital Biomedical Equipment Maintenance & SLA System
-- Database: HBMS.accdb (Pre-compiled Microsoft Access Relational Queries & Views)
-- Tracking Issue: Issue #7: [TASK-2.3] MS Access SQL Queries, Data Views & Synthetic Demonstration Dataset
-- =============================================================================

-- =============================================================================
-- 1. QUERY: qry_ActiveBreakdowns
-- Description: Retrieves all active, unclosed breakdown incidents, displaying
-- remaining time before response deadline and current triage status.
-- =============================================================================
SELECT 
    B.BreakdownID,
    B.BreakdownRef,
    A.AssetCode,
    A.EquipmentName,
    A.EquipmentCategory,
    A.Criticality,
    W.WardName,
    W.DepartmentName,
    B.Severity,
    B.ReportedAt,
    B.ResponseDeadline,
    ROUND((B.ResponseDeadline - NOW()) * 24, 2) AS HoursRemainingBeforeDeadline,
    B.ResponseAt,
    B.ResponseBy,
    B.Status AS IncidentStatus,
    U.FullName AS ReportedByStaff
FROM ((BreakdownLogs AS B
INNER JOIN Assets AS A ON B.AssetID = A.AssetID)
INNER JOIN Wards AS W ON A.WardID = W.WardID)
INNER JOIN Users AS U ON B.ReportedByUserID = U.UserID
WHERE B.Status <> 'Closed'
ORDER BY B.ResponseDeadline ASC;

-- =============================================================================
-- 2. QUERY: qry_OverdueCriticalCareBreakdowns
-- Description: Core query driving the automated 4-Hour SLA Escalation Engine.
-- Identifies critical care equipment breakdowns that have exceeded the 4-hour
-- response window without recorded acknowledgment.
-- =============================================================================
SELECT 
    B.BreakdownID,
    B.BreakdownRef,
    A.AssetCode,
    A.EquipmentName,
    A.EquipmentCategory,
    W.WardName,
    V.VendorName,
    V.ContactPerson AS VendorContact,
    V.Phone AS VendorEmergencyPhone,
    B.ReportedAt,
    B.ResponseDeadline,
    NOW() AS EscalationCheckTimestamp,
    ROUND((NOW() - B.ResponseDeadline) * 24, 2) AS OverdueHours,
    B.Status AS CurrentStatus
FROM (((BreakdownLogs AS B
INNER JOIN Assets AS A ON B.AssetID = A.AssetID)
INNER JOIN Wards AS W ON A.WardID = W.WardID)
LEFT JOIN VendorSLAs AS S ON B.SLAID = S.SLAID)
LEFT JOIN Vendors AS V ON S.VendorID = V.VendorID
WHERE (B.Severity = 'Critical' OR A.Criticality = 'Critical Care')
  AND B.ResponseAt IS NULL
  AND NOW() > B.ResponseDeadline
  AND B.Status <> 'Closed'
  AND B.BreakdownID NOT IN (SELECT BreakdownID FROM EscalationLogs);

-- =============================================================================
-- 3. QUERY: qry_DepartmentUptime
-- Description: Computes equipment availability and operational uptime percentages
-- aggregated across clinical hospital departments over a 720-hour (30-day) window.
-- Formula: Uptime (%) = [(Total Operating Hours - Total Downtime) / Total Operating Hours] * 100
-- =============================================================================
SELECT 
    W.DepartmentName,
    W.WardName,
    COUNT(A.AssetID) AS TotalRegisteredAssets,
    SUM(IIF(A.AssetStatus = 'Operational', 1, 0)) AS OperationalUnits,
    SUM(IIF(A.AssetStatus <> 'Operational', 1, 0)) AS UnitsUnderRepair,
    720 AS ObservationPeriodHours,
    NZ(SUM(ROUND((NZ(B.ClosedAt, NOW()) - B.ReportedAt) * 24, 1)), 0) AS TotalDowntimeHours,
    ROUND(
        ((COUNT(A.AssetID) * 720 - NZ(SUM(ROUND((NZ(B.ClosedAt, NOW()) - B.ReportedAt) * 24, 1)), 0)) 
        / (COUNT(A.AssetID) * 720)) * 100, 
        2
    ) AS UptimePercentage
FROM (Wards AS W
LEFT JOIN Assets AS A ON W.WardID = A.WardID)
LEFT JOIN BreakdownLogs AS B ON A.AssetID = B.AssetID
GROUP BY W.DepartmentName, W.WardName;

-- =============================================================================
-- 4. QUERY: qry_JobCostSummary
-- Description: Computes the composite financial expenditure for each work order.
-- Formula: Total Job Cost = Labor Charges + Spare Parts Total + Vendor Service Fee
-- =============================================================================
SELECT 
    J.JobID,
    B.BreakdownRef,
    A.AssetCode,
    A.EquipmentName,
    U.FullName AS AssignedEngineer,
    J.LaborHours,
    J.HourlyRate,
    ROUND(J.LaborHours * J.HourlyRate, 2) AS TotalLaborCost,
    NZ(PartsSub.TotalPartsCost, 0.00) AS TotalSparePartsCost,
    NZ(VendorSub.TotalVendorCost, 0.00) AS TotalVendorFees,
    ROUND(
        (J.LaborHours * J.HourlyRate) + NZ(PartsSub.TotalPartsCost, 0.00) + NZ(VendorSub.TotalVendorCost, 0.00), 
        2
    ) AS TotalJobCost,
    J.JobStatus
FROM (((MaintenanceJobs AS J
INNER JOIN BreakdownLogs AS B ON J.BreakdownID = B.BreakdownID)
INNER JOIN Assets AS A ON B.AssetID = A.AssetID)
INNER JOIN Users AS U ON J.EngineerUserID = U.UserID)
LEFT JOIN (
    SELECT JobID, SUM(QuantityUsed * UnitCostAtUse) AS TotalPartsCost
    FROM JobSpareParts
    GROUP BY JobID
) AS PartsSub ON J.JobID = PartsSub.JobID
LEFT JOIN (
    SELECT JobID, SUM(Amount) AS TotalVendorCost
    FROM RepairCosts
    WHERE CostType = 'External Vendor Fee'
    GROUP BY JobID
) AS VendorSub ON J.JobID = VendorSub.JobID;

-- =============================================================================
-- 5. QUERY: qry_VendorSLACompliance
-- Description: Audits vendor compliance against contracted response windows.
-- =============================================================================
SELECT 
    V.VendorName,
    V.ContactPerson,
    COUNT(B.BreakdownID) AS TotalDispatchedIncidents,
    SUM(IIF(B.ResponseAt IS NOT NULL AND B.ResponseAt <= B.ResponseDeadline, 1, 0)) AS CompliantResponses,
    SUM(IIF(B.ResponseAt IS NULL OR B.ResponseAt > B.ResponseDeadline, 1, 0)) AS BreachedResponses,
    ROUND(
        (SUM(IIF(B.ResponseAt IS NOT NULL AND B.ResponseAt <= B.ResponseDeadline, 1, 0)) / COUNT(B.BreakdownID)) * 100, 
        1
    ) AS CompliancePercentage,
    ROUND(AVG(ROUND((NZ(B.ResponseAt, NOW()) - B.ReportedAt) * 24, 2)), 2) AS AvgResponseHours
FROM (Vendors AS V
INNER JOIN VendorSLAs AS S ON V.VendorID = S.VendorID)
INNER JOIN BreakdownLogs AS B ON S.SLAID = B.SLAID
GROUP BY V.VendorName, V.ContactPerson;

-- =============================================================================
-- 6. QUERY: qry_PriorityQueueOrder
-- Description: Orders open work orders for in-memory priority queue ingestion.
-- Order: Priority Level 1 (Critical Care) -> 2 (High) -> 3 (Medium) -> 4 (Low) -> FIFO.
-- =============================================================================
SELECT 
    B.BreakdownID,
    B.BreakdownRef,
    A.AssetCode,
    A.EquipmentName,
    A.EquipmentCategory,
    W.WardName,
    B.Severity,
    SWITCH(
        B.Severity = 'Critical' OR A.Criticality = 'Critical Care', 1,
        B.Severity = 'High', 2,
        B.Severity = 'Medium', 3,
        TRUE, 4
    ) AS PriorityScore,
    B.ReportedAt,
    B.ResponseDeadline,
    B.Status
FROM (BreakdownLogs AS B
INNER JOIN Assets AS A ON B.AssetID = A.AssetID)
INNER JOIN Wards AS W ON A.WardID = W.WardID
WHERE B.Status IN ('Pending', 'Assigned', 'In Progress', 'ESCALATED')
ORDER BY 
    SWITCH(
        B.Severity = 'Critical' OR A.Criticality = 'Critical Care', 1,
        B.Severity = 'High', 2,
        B.Severity = 'Medium', 3,
        TRUE, 4
    ) ASC,
    B.ReportedAt ASC;
