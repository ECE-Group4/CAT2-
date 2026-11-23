Imports System
Imports System.Data
Imports System.Data.OleDb
Imports System.Linq
Imports HBMS.Desktop.Database
Imports HBMS.Desktop.Domain

Namespace HBMS.Desktop.Services
    Public NotInheritable Class HbmsService
        Private Sub New()
        End Sub
        Public Shared Function Authenticate(username As String, password As String) As DataRow
            Dim t = Db.Query("SELECT TOP 1 UserID,FullName,Username,PasswordHash,RoleName FROM Users WHERE Username=? AND IsActive=True", Db.P("u", username))
            If t.Rows.Count = 0 Then Return Nothing
            Dim r = t.Rows(0)
            If PasswordHasher.Verify(password, CStr(r("PasswordHash"))) Then Return r
            Return Nothing
        End Function
        Public Shared Function GetBreakdowns(Optional openOnly As Boolean = True) As DataTable
            Dim sql = "SELECT B.BreakdownID,B.BreakdownRef,A.AssetCode,A.EquipmentName,W.WardName,B.Severity,B.ReportedAt,B.ResponseDeadline,B.Status FROM (Breakdowns AS B INNER JOIN Assets AS A ON B.AssetID=A.AssetID) INNER JOIN Wards AS W ON A.WardID=W.WardID"
            If openOnly Then sql &= " WHERE B.Status NOT IN ('Closed','Cancelled')"
            sql &= " ORDER BY IIF(B.Severity='CRITICAL',4,IIF(B.Severity='HIGH',3,IIF(B.Severity='MEDIUM',2,1))) DESC, B.ReportedAt ASC"
            Return Db.Query(sql)
        End Function
        Public Shared Function GetAssets() As DataTable
            Return Db.Query("SELECT A.AssetID,A.AssetCode,A.EquipmentName,W.WardName,A.Criticality,A.AssetStatus FROM Assets AS A INNER JOIN Wards AS W ON A.WardID=W.WardID ORDER BY A.AssetCode")
        End Function
        Public Shared Function GetParts() As DataTable
            Return Db.Query("SELECT PartID,PartCode,PartName,QuantityAvailable,ReorderLevel,UnitCost FROM SpareParts WHERE IsActive=True ORDER BY PartName")
        End Function
        Public Shared Function GetWards() As DataTable
            Return Db.Query("SELECT WardID,WardName,DepartmentName FROM Wards WHERE IsActive=True ORDER BY WardName")
        End Function
        Public Shared Function CreateBreakdown(assetId As Integer, reporterId As Integer, severity As String, description As String) As String
            If String.IsNullOrWhiteSpace(description) Then Throw New ArgumentException("Describe the fault before submitting.")
            Dim asset = Db.Query("SELECT VendorID,Criticality FROM Assets WHERE AssetID=?", Db.P("a", assetId, OleDbType.Integer))
            If asset.Rows.Count = 0 Then Throw New InvalidOperationException("Selected asset was not found.")
            Dim nowAt = DateTime.Now, responseHours = 4.0, resolutionHours = 24.0, selectedSlaId As Object = DBNull.Value
            If Not IsDBNull(asset.Rows(0)("VendorID")) Then
                Dim sla = Db.Query("SELECT TOP 1 S.SLAID,S.ResponseHours,S.ResolutionHours FROM VendorSLAs AS S INNER JOIN Assets AS A ON S.VendorID=A.VendorID WHERE A.AssetID=? AND S.IsActive=True AND S.EffectiveFrom<=? AND (S.EffectiveTo IS NULL OR S.EffectiveTo>=?) ORDER BY S.EffectiveFrom DESC", Db.P("a", assetId, OleDbType.Integer), Db.P("d1", nowAt, OleDbType.Date), Db.P("d2", nowAt, OleDbType.Date))
                If sla.Rows.Count > 0 Then selectedSlaId = CInt(sla.Rows(0)("SLAID")) : responseHours = CDbl(sla.Rows(0)("ResponseHours")) : resolutionHours = CDbl(sla.Rows(0)("ResolutionHours"))
            End If
            If String.Equals(CStr(asset.Rows(0)("Criticality")), "CRITICAL_CARE", StringComparison.OrdinalIgnoreCase) Then responseHours = Math.Min(responseHours, 4.0)
            Dim ref = "BRK-" & DateTime.Now.ToString("yyyyMMdd-HHmmss") & "-" & Guid.NewGuid().ToString("N").Substring(0, 4).ToUpperInvariant()
            Using c = Db.OpenConnection(), tx = c.BeginTransaction()
                Using cmd As New OleDbCommand("INSERT INTO Breakdowns (BreakdownRef,AssetID,ReportedByUserID,SLAID,ReportedAt,Severity,Description,ResponseDeadline,ResolutionDeadline,Status) VALUES (?,?,?,?,?,?,?,?,?,?)", c, tx)
                    cmd.Parameters.AddRange({Db.P("r", ref), Db.P("a", assetId, OleDbType.Integer), Db.P("u", reporterId, OleDbType.Integer), Db.P("sla", selectedSlaId, OleDbType.Integer), Db.P("t", nowAt, OleDbType.Date), Db.P("s", severity), Db.P("d", description), Db.P("rd", nowAt.AddHours(responseHours), OleDbType.Date), Db.P("xd", nowAt.AddHours(resolutionHours), OleDbType.Date), Db.P("st", "Reported")})
                    cmd.ExecuteNonQuery()
                End Using
                Using cmd As New OleDbCommand("INSERT INTO AuditLogs (UserID,EntityName,ActionName,Details,OccurredAt) VALUES (?,?,?,?,?)", c, tx)
                    cmd.Parameters.AddRange({Db.P("u", reporterId, OleDbType.Integer), Db.P("e", "Breakdown"), Db.P("a", "CREATE"), Db.P("d", ref & " reported"), Db.P("t", nowAt, OleDbType.Date)}) : cmd.ExecuteNonQuery()
                End Using
                tx.Commit()
            End Using
            Return ref
        End Function
        Public Shared Sub RecordResponse(breakdownId As Integer, userId As Integer)
            Db.Execute("UPDATE Breakdowns SET ResponseAt=?,ResponseByUserID=?,Status='Responded' WHERE BreakdownID=? AND ResponseAt IS NULL", Db.P("t", DateTime.Now, OleDbType.Date), Db.P("u", userId, OleDbType.Integer), Db.P("b", breakdownId, OleDbType.Integer))
        End Sub
        Public Shared Sub AssignEngineer(breakdownId As Integer, engineerId As Integer)
            Db.Execute("INSERT INTO MaintenanceJobs (BreakdownID,EngineerUserID,AssignedAt,JobStatus) VALUES (?,?,?,?)", Db.P("b", breakdownId, OleDbType.Integer), Db.P("u", engineerId, OleDbType.Integer), Db.P("t", DateTime.Now, OleDbType.Date), Db.P("s", "Assigned"))
            Db.Execute("UPDATE Breakdowns SET Status='Assigned' WHERE BreakdownID=?", Db.P("b", breakdownId, OleDbType.Integer))
        End Sub
        Public Shared Sub RunEscalations()
            Dim overdue = Db.Query("SELECT B.BreakdownID,B.BreakdownRef FROM Breakdowns AS B INNER JOIN Assets AS A ON B.AssetID=A.AssetID WHERE A.Criticality='CRITICAL_CARE' AND B.ResponseAt IS NULL AND B.ResponseDeadline<Now() AND B.Status NOT IN ('Closed','Cancelled') AND NOT EXISTS (SELECT E.EscalationID FROM EscalationLogs AS E WHERE E.BreakdownID=B.BreakdownID AND E.EscalationLevel='FOUR_HOUR_RESPONSE')")
            For Each r As DataRow In overdue.Rows
                Db.Execute("INSERT INTO EscalationLogs (BreakdownID,EscalatedAt,EscalationLevel,Reason,NotifiedTo,NotificationStatus) VALUES (?,?,?,?,?,?)", Db.P("b", CInt(r("BreakdownID")), OleDbType.Integer), Db.P("t", DateTime.Now, OleDbType.Date), Db.P("l", "FOUR_HOUR_RESPONSE"), Db.P("r", "Critical-care response window exceeded without recorded response."), Db.P("n", "Biomedical Supervisor"), Db.P("s", "DISPLAYED"))
            Next
        End Sub
        Public Shared Sub AddPartUsage(jobId As Integer, partId As Integer, quantity As Integer)
            If quantity <= 0 Then Throw New ArgumentOutOfRangeException(NameOf(quantity))
            Using c = Db.OpenConnection(), tx = c.BeginTransaction()
                Dim unitCost As Decimal, available As Integer
                Using cmd As New OleDbCommand("SELECT QuantityAvailable,UnitCost FROM SpareParts WHERE PartID=?", c, tx)
                    cmd.Parameters.Add(Db.P("p", partId, OleDbType.Integer))
                    Using rd = cmd.ExecuteReader()
                        If Not rd.Read() Then Throw New InvalidOperationException("Part not found.")
                        available = CInt(rd("QuantityAvailable")) : unitCost = CDec(rd("UnitCost"))
                    End Using
                End Using
                If available < quantity Then Throw New InvalidOperationException("Insufficient stock. Available: " & available)
                Using cmd As New OleDbCommand("UPDATE SpareParts SET QuantityAvailable=QuantityAvailable-? WHERE PartID=?", c, tx)
                    cmd.Parameters.Add(Db.P("q", quantity, OleDbType.Integer)) : cmd.Parameters.Add(Db.P("p", partId, OleDbType.Integer)) : cmd.ExecuteNonQuery()
                End Using
                Using cmd As New OleDbCommand("INSERT INTO JobSpareParts (JobID,PartID,QuantityUsed,UnitCostAtUse,UsedAt) VALUES (?,?,?,?,?)", c, tx)
                    cmd.Parameters.Add(Db.P("j", jobId, OleDbType.Integer)) : cmd.Parameters.Add(Db.P("p", partId, OleDbType.Integer)) : cmd.Parameters.Add(Db.P("q", quantity, OleDbType.Integer)) : cmd.Parameters.Add(Db.P("c", unitCost, OleDbType.Currency)) : cmd.Parameters.Add(Db.P("t", DateTime.Now, OleDbType.Date)) : cmd.ExecuteNonQuery()
                End Using
                tx.Commit()
            End Using
        End Sub
        Public Shared Sub AddRepairCost(jobId As Integer, costType As String, description As String, amount As Decimal, userId As Integer)
            If amount < 0 Then Throw New ArgumentOutOfRangeException(NameOf(amount))
            Db.Execute("INSERT INTO RepairCosts (JobID,CostType,Description,Amount,RecordedAt,RecordedByUserID) VALUES (?,?,?,?,?,?)", Db.P("j", jobId, OleDbType.Integer), Db.P("t", costType), Db.P("d", description), Db.P("a", amount, OleDbType.Currency), Db.P("r", DateTime.Now, OleDbType.Date), Db.P("u", userId, OleDbType.Integer))
        End Sub

        Public Shared Sub AdvanceMaintenance(breakdownId As Integer, targetStatus As String, notes As String, userId As Integer)
            Dim job = Db.Query("SELECT TOP 1 JobID FROM MaintenanceJobs WHERE BreakdownID=? ORDER BY JobID DESC", Db.P("b", breakdownId, OleDbType.Integer))
            If job.Rows.Count = 0 Then Throw New InvalidOperationException("Assign an engineer before updating maintenance progress.")
            Dim jobId = CInt(job.Rows(0)("JobID")), nowAt = DateTime.Now
            Select Case targetStatus
                Case "InProgress"
                    Db.Execute("UPDATE MaintenanceJobs SET StartedAt=?,JobStatus=?,Diagnosis=? WHERE JobID=?", Db.P("t", nowAt, OleDbType.Date), Db.P("s", targetStatus), Db.P("n", notes), Db.P("j", jobId, OleDbType.Integer))
                    Db.Execute("UPDATE Breakdowns SET Status='InProgress' WHERE BreakdownID=?", Db.P("b", breakdownId, OleDbType.Integer))
                Case "Repaired"
                    Db.Execute("UPDATE MaintenanceJobs SET CompletedAt=?,JobStatus=?,WorkPerformed=? WHERE JobID=?", Db.P("t", nowAt, OleDbType.Date), Db.P("s", targetStatus), Db.P("n", notes), Db.P("j", jobId, OleDbType.Integer))
                    Db.Execute("UPDATE Breakdowns SET Status='Repaired' WHERE BreakdownID=?", Db.P("b", breakdownId, OleDbType.Integer))
                Case "Verified"
                    If String.IsNullOrWhiteSpace(notes) Then Throw New ArgumentException("Verification notes are required.")
                    Db.Execute("UPDATE MaintenanceJobs SET JobStatus=?,VerificationNotes=? WHERE JobID=?", Db.P("s", targetStatus), Db.P("n", notes), Db.P("j", jobId, OleDbType.Integer))
                    Db.Execute("UPDATE Breakdowns SET Status='Verified' WHERE BreakdownID=?", Db.P("b", breakdownId, OleDbType.Integer))
                Case "Closed"
                    Dim verification = Db.Query("SELECT VerificationNotes FROM MaintenanceJobs WHERE JobID=?", Db.P("j", jobId, OleDbType.Integer))
                    If verification.Rows.Count = 0 OrElse IsDBNull(verification.Rows(0)("VerificationNotes")) OrElse String.IsNullOrWhiteSpace(CStr(verification.Rows(0)("VerificationNotes"))) Then Throw New InvalidOperationException("Record verification before closing the breakdown.")
                    Db.Execute("UPDATE MaintenanceJobs SET JobStatus='Closed' WHERE JobID=?", Db.P("j", jobId, OleDbType.Integer))
                    Db.Execute("UPDATE Breakdowns SET Status='Closed',ClosedAt=? WHERE BreakdownID=?", Db.P("t", nowAt, OleDbType.Date), Db.P("b", breakdownId, OleDbType.Integer))
                Case Else
                    Throw New ArgumentException("Unsupported maintenance status.")
            End Select
            Db.Execute("INSERT INTO AuditLogs (UserID,EntityName,EntityID,ActionName,Details,OccurredAt) VALUES (?,?,?,?,?,?)", Db.P("u", userId, OleDbType.Integer), Db.P("e", "Breakdown"), Db.P("id", breakdownId, OleDbType.Integer), Db.P("a", "STATUS_CHANGE"), Db.P("d", targetStatus & ": " & notes), Db.P("t", nowAt, OleDbType.Date))
        End Sub
        Public Shared Sub PublishUptimeSnapshot(outputPath As String)
            Dim rows = GetExecutiveUptime(), departments As New List(Of Object)(), total = 0, operational = 0
            For Each r As DataRow In rows.Rows
                Dim count = CInt(r("AssetCount")), active = If(IsDBNull(r("OperationalCount")), 0, CInt(r("OperationalCount")))
                total += count : operational += active
                departments.Add(New With {.department = CStr(r("DepartmentName")), .assets = count, .operational = active, .openBreakdowns = 0})
            Next
            Dim payload = New With {.generatedAt = DateTimeOffset.Now.ToString("o"), .reportingDefinition = "Current asset status snapshot; not time-weighted uptime.", .departments = departments}
            Dim directory = IO.Path.GetDirectoryName(outputPath)
            If Not String.IsNullOrWhiteSpace(directory) Then IO.Directory.CreateDirectory(directory)
            IO.File.WriteAllText(outputPath, System.Text.Json.JsonSerializer.Serialize(payload, New System.Text.Json.JsonSerializerOptions With {.WriteIndented = True}))
        End Sub


        Public Shared Sub AddUser(fullName As String, username As String, password As String, roleName As String)
            If String.IsNullOrWhiteSpace(fullName) OrElse String.IsNullOrWhiteSpace(username) OrElse password.Length < 10 Then Throw New ArgumentException("Enter a name, username and password of at least 10 characters.")
            Dim existing = Db.Query("SELECT UserID FROM Users WHERE Username=?", Db.P("u", username.Trim()))
            If existing.Rows.Count > 0 Then Throw New InvalidOperationException("That username is already in use.")
            Db.Execute("INSERT INTO Users (FullName,Username,PasswordHash,RoleName,IsActive,CreatedAt) VALUES (?,?,?,?,?,?)", Db.P("n", fullName.Trim()), Db.P("u", username.Trim()), Db.P("p", PasswordHasher.HashPassword(password)), Db.P("r", roleName), Db.P("a", True, OleDbType.Boolean), Db.P("t", DateTime.Now, OleDbType.Date))
        End Sub
        Public Shared Sub AddVendor(vendorCode As String, vendorName As String, contact As String, phone As String, email As String)
            Db.Execute("INSERT INTO Vendors (VendorCode,VendorName,ContactPerson,Phone,Email,IsActive) VALUES (?,?,?,?,?,?)", Db.P("c", vendorCode.Trim()), Db.P("n", vendorName.Trim()), Db.P("p", contact), Db.P("ph", phone), Db.P("e", email), Db.P("a", True, OleDbType.Boolean))
        End Sub
        Public Shared Sub AddAsset(assetCode As String, equipmentName As String, category As String, wardId As Integer, criticality As String, vendorId As Object)
            If String.IsNullOrWhiteSpace(assetCode) OrElse String.IsNullOrWhiteSpace(equipmentName) Then Throw New ArgumentException("Asset code and equipment name are required.")
            Db.Execute("INSERT INTO Assets (AssetCode,EquipmentName,EquipmentCategory,WardID,Criticality,AssetStatus,VendorID,DateCommissioned) VALUES (?,?,?,?,?,?,?,?)", Db.P("c", assetCode.Trim()), Db.P("n", equipmentName.Trim()), Db.P("cat", category), Db.P("w", wardId, OleDbType.Integer), Db.P("crit", criticality), Db.P("st", "OPERATIONAL"), Db.P("v", vendorId, OleDbType.Integer), Db.P("d", DateTime.Now, OleDbType.Date))
        End Sub
        Public Shared Sub AddPart(partCode As String, partName As String, quantity As Integer, reorderLevel As Integer, unitCost As Decimal)
            If String.IsNullOrWhiteSpace(partCode) OrElse String.IsNullOrWhiteSpace(partName) OrElse quantity < 0 OrElse reorderLevel < 0 OrElse unitCost < 0 Then Throw New ArgumentException("Enter valid part details, non-negative stock and cost.")
            Db.Execute("INSERT INTO SpareParts (PartCode,PartName,UnitName,QuantityAvailable,ReorderLevel,UnitCost,IsActive) VALUES (?,?,?,?,?,?,?)", Db.P("c", partCode.Trim()), Db.P("n", partName.Trim()), Db.P("u", "piece"), Db.P("q", quantity, OleDbType.Integer), Db.P("r", reorderLevel, OleDbType.Integer), Db.P("cost", unitCost, OleDbType.Currency), Db.P("a", True, OleDbType.Boolean))
        End Sub
        Public Shared Sub AddVendorSla(vendorId As Integer, slaName As String, responseHours As Double, resolutionHours As Double)
            If responseHours <= 0 OrElse resolutionHours <= 0 Then Throw New ArgumentException("SLA hours must be greater than zero.")
            Db.Execute("INSERT INTO VendorSLAs (VendorID,SLAName,ResponseHours,ResolutionHours,EffectiveFrom,IsActive) VALUES (?,?,?,?,?,?)", Db.P("v", vendorId, OleDbType.Integer), Db.P("n", slaName.Trim()), Db.P("r", responseHours, OleDbType.Double), Db.P("x", resolutionHours, OleDbType.Double), Db.P("d", DateTime.Now.Date, OleDbType.Date), Db.P("a", True, OleDbType.Boolean))
        End Sub

        Public Shared Function GetExecutiveUptime() As DataTable
            Return Db.Query("SELECT W.DepartmentName,Count(A.AssetID) AS AssetCount,Sum(IIF(A.AssetStatus='OPERATIONAL',1,0)) AS OperationalCount FROM Wards AS W INNER JOIN Assets AS A ON W.WardID=A.WardID GROUP BY W.DepartmentName ORDER BY W.DepartmentName")
        End Function
    End Class
End Namespace
