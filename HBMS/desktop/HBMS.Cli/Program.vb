Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.Text
Imports System.Text.Json
Imports HBMS.Desktop.Domain
Imports HBMS.Desktop.Services

Namespace HBMS.Cli
    Public Module Program
        Public Function Main(args As String()) As Integer
            Console.WriteLine("================================================================================")
            Console.WriteLine("  HBMS CROSS-PLATFORM CLI RUNNER (VB.NET / .NET 8)")
            Console.WriteLine("  County Referral Hospital Biomedical Equipment Maintenance & SLA System")
            Console.WriteLine("================================================================================")

            Try
                ' 1. Staff Authentication Simulation
                Console.WriteLine(vbCrLf & "[Step 1] Biomedical Engineer Authentication:")
                Dim engineerUser As String = "dkiprono"
                Dim engineerPassword As String = "Biomed@2026!"
                Dim storedHash = PasswordHasher.HashPassword(engineerPassword)
                Dim authSuccess = PasswordHasher.Verify(engineerPassword, storedHash)
                Console.WriteLine($"  User: {engineerUser} (Lead Biomedical Engineer)")
                Console.WriteLine($"  PBKDF2 Password Verification: {If(authSuccess, "AUTHENTICATED (Success)", "FAILED")}")
                If Not authSuccess Then Return 1

                ' 2. Biomedical Asset Inventory Ingestion
                Console.WriteLine(vbCrLf & "[Step 2] Biomedical Equipment Asset Inventory:")
                Dim assets As New List(Of EquipmentRecord) From {
                    New EquipmentRecord With {.AssetID = 1, .AssetCode = "EQ-VENT-001", .EquipmentName = "ICU Mechanical Ventilator", .EquipmentCategory = "Ventilators", .WardID = 1, .Criticality = "Critical Care", .AssetStatus = "Operational"},
                    New EquipmentRecord With {.AssetID = 4, .AssetCode = "EQ-VENT-004", .EquipmentName = "Anaesthesia Ventilator Unit", .EquipmentCategory = "Ventilators", .WardID = 2, .Criticality = "Critical Care", .AssetStatus = "Under Maintenance"},
                    New EquipmentRecord With {.AssetID = 8, .AssetCode = "EQ-MON-003", .EquipmentName = "Emergency Trauma Bedside Monitor", .EquipmentCategory = "Patient Monitors", .WardID = 3, .Criticality = "Critical Care", .AssetStatus = "Under Maintenance"},
                    New EquipmentRecord With {.AssetID = 14, .AssetCode = "EQ-PUMP-004", .EquipmentName = "Emergency Resuscitation Syringe Driver", .EquipmentCategory = "Infusion Pumps", .WardID = 3, .Criticality = "General Care", .AssetStatus = "Under Maintenance"},
                    New EquipmentRecord With {.AssetID = 17, .AssetCode = "EQ-XRAY-002", .EquipmentName = "Operating Theatre C-Arm Fluoroscopy", .EquipmentCategory = "X-Ray Units", .WardID = 2, .Criticality = "Critical Care", .AssetStatus = "Operational"}
                }
                For Each a In assets
                    Console.WriteLine($"  [{a.AssetCode}] {a.EquipmentName,-38} | Cat: {a.EquipmentCategory,-15} | Status: {a.AssetStatus}")
                Next

                ' 3. Priority Queue Triage (Binary Max-Heap)
                Console.WriteLine(vbCrLf & "[Step 3] Priority Queue Binary Max-Heap Triage:")
                Dim pq As New PriorityQueueService()
                Dim now = DateTime.Now

                Dim b1 As New BreakdownItem With {
                    .BreakdownID = 101,
                    .BreakdownRef = "BRK-2026-003",
                    .AssetCode = "EQ-VENT-004",
                    .EquipmentName = "Anaesthesia Ventilator Unit",
                    .WardName = "Operating Theatre (OT)",
                    .Severity = Severity.Critical,
                    .ReportedAt = now.AddHours(-6),
                    .ResponseDeadline = now.AddHours(-2),
                    .Status = "ESCALATED"
                }

                Dim b2 As New BreakdownItem With {
                    .BreakdownID = 102,
                    .BreakdownRef = "BRK-2026-004",
                    .AssetCode = "EQ-MON-003",
                    .EquipmentName = "Emergency Trauma Bedside Monitor",
                    .WardName = "Emergency Department (ED)",
                    .Severity = Severity.High,
                    .ReportedAt = now.AddMinutes(-45),
                    .ResponseDeadline = now.AddMinutes(195),
                    .Status = "Pending"
                }

                Dim b3 As New BreakdownItem With {
                    .BreakdownID = 103,
                    .BreakdownRef = "BRK-2026-005",
                    .AssetCode = "EQ-PUMP-004",
                    .EquipmentName = "Emergency Resuscitation Syringe Driver",
                    .WardName = "Emergency Department (ED)",
                    .Severity = Severity.Medium,
                    .ReportedAt = now.AddHours(-5),
                    .ResponseDeadline = now.AddHours(7),
                    .Status = "InProgress"
                }

                pq.Enqueue(b2)
                pq.Enqueue(b3)
                pq.Enqueue(b1)

                Console.WriteLine($"  Enqueued 3 active incidents. Total in queue: {pq.Count}")
                Dim snapshot = pq.Snapshot()
                For i As Integer = 0 To snapshot.Count - 1
                    Dim item = snapshot(i)
                    Console.WriteLine($"  Rank #{i + 1}: [{item.Severity,-8}] Ref: {item.BreakdownRef} | {item.EquipmentName} | Deadline: {item.ResponseDeadline:yyyy-MM-dd HH:mm:ss}")
                Next

                ' 4. Automated 4-Hour SLA Breach & Escalation Engine
                Console.WriteLine(vbCrLf & "[Step 4] Automated 4-Hour Critical Care SLA Escalation Engine:")
                Dim overdueCount As Integer = 0
                For Each item In snapshot
                    If item.IsOverdue AndAlso item.Severity = Severity.Critical Then
                        overdueCount += 1
                        Dim overdueHours = (now - item.ResponseDeadline).TotalHours
                        Console.WriteLine($"  [!] SLA BREACH DETECTED: {item.BreakdownRef} ({item.AssetCode})")
                        Console.WriteLine($"      Response Window: 4.0 Hours | Overdue by: {overdueHours:F1} hrs")
                        Console.WriteLine($"      Action: Escalated to Medical Superintendent & Biomedical Director.")
                    End If
                Next
                Console.WriteLine($"  Escalation Check Completed: {overdueCount} critical breach(es) flagged.")

                ' 5. Repair Cost Calculation Rollup
                Console.WriteLine(vbCrLf & "[Step 5] Work Order Repair Cost Accounting:")
                Dim laborHours As Double = 5.75
                Dim laborRate As Decimal = 1500.0D
                Dim laborCost As Decimal = CDec(laborHours) * laborRate
                Dim sparePartCost As Decimal = 57500.0D ' Expiratory Valve + HEPA Filter
                Dim vendorFee As Decimal = 25000.0D     ' Draeger Calibration
                Dim totalCost As Decimal = laborCost + sparePartCost + vendorFee

                Console.WriteLine($"  Work Order #1 (BRK-2026-001) Final Billing:")
                Console.WriteLine($"    - Internal Labor ({laborHours} hrs @ KES {laborRate:N2}/hr): KES {laborCost:N2}")
                Console.WriteLine($"    - Replacement Spare Parts                   : KES {sparePartCost:N2}")
                Console.WriteLine($"    - External Specialist Vendor Fee           : KES {vendorFee:N2}")
                Console.WriteLine($"    -------------------------------------------------------")
                Console.WriteLine($"    TOTAL REPAIR EXPENDITURE                    : KES {totalCost:N2}")

                ' 6. Export Portal Status Snapshot (Optional)
                Try
                    Dim searchDir = New DirectoryInfo(AppContext.BaseDirectory)
                    Dim portalDir As DirectoryInfo = Nothing
                    While searchDir IsNot Nothing
                        Dim cand = Path.Combine(searchDir.FullName, "HBMS", "portal")
                        If Directory.Exists(cand) Then
                            portalDir = New DirectoryInfo(cand)
                            Exit While
                        End If
                        Dim cand2 = Path.Combine(searchDir.FullName, "portal")
                        If Directory.Exists(cand2) AndAlso File.Exists(Path.Combine(cand2, "index.html")) Then
                            portalDir = New DirectoryInfo(cand2)
                            Exit While
                        End If
                        searchDir = searchDir.Parent
                    End While

                    If portalDir IsNot Nothing Then
                        Dim snapshotPath = Path.Combine(portalDir.FullName, "live-snapshot.json")
                        Dim jsonPayload = New With {
                            .generatedAt = DateTime.UtcNow.ToString("o"),
                            .activeBreakdowns = snapshot.Count,
                            .overdueCritical = overdueCount,
                            .operationalAssets = 17,
                            .totalAssets = 20,
                            .uptimePercentage = 85.0
                        }
                        Dim options As New JsonSerializerOptions With {.WriteIndented = True}
                        Dim jsonString = JsonSerializer.Serialize(jsonPayload, options)
                        File.WriteAllText(snapshotPath, jsonString, Encoding.UTF8)
                        Console.WriteLine(vbCrLf & $"[Step 6] Web Portal Live Snapshot exported to: {snapshotPath}")
                    Else
                        Console.WriteLine(vbCrLf & "[Step 6] Notice: Portal directory not located; skipping live snapshot export.")
                    End If
                Catch ex As Exception
                    Console.WriteLine(vbCrLf & $"[Step 6] Notice: Could not write portal live-snapshot: {ex.Message}")
                End Try

                Console.WriteLine(vbCrLf & "================================================================================")
                Console.WriteLine("  HBMS CLI SIMULATION COMPLETED SUCCESSFULLY (All 5 Steps Passed)")
                Console.WriteLine("================================================================================")
                Return 0
            Catch ex As Exception
                Console.WriteLine($"FATAL CLI ERROR: {ex.Message}" & vbCrLf & ex.StackTrace)
                Return 1
            End Try
        End Function
    End Module
End Namespace
