Imports System
Imports System.IO
Imports System.Text.Json
Imports HBMS.Desktop.Database

Namespace HBMS.Desktop.Services
    Public NotInheritable Class AlertInboxImporter
        Private Sub New()
        End Sub
        Public Shared Function ImportPending(inboxPath As String, userId As Integer) As Integer
            If Not File.Exists(inboxPath) Then Return 0
            Dim imported = 0
            For Each line In File.ReadLines(inboxPath)
                If String.IsNullOrWhiteSpace(line) Then Continue For
                Dim messageId = ""
                Try
                    Using doc = JsonDocument.Parse(line)
                        Dim root = doc.RootElement
                        messageId = root.GetProperty("messageId").GetString()
                        Dim duplicate = Db.Query("SELECT ImportedAlertID FROM ImportedAlerts WHERE MessageID=?", Db.P("m", messageId))
                        If duplicate.Rows.Count > 0 Then Continue For
                        Dim assetCode = root.GetProperty("assetCode").GetString()
                        Dim severity = root.GetProperty("severity").GetString().ToUpperInvariant()
                        Dim description = root.GetProperty("description").GetString()
                        Dim asset = Db.Query("SELECT AssetID FROM Assets WHERE AssetCode=?", Db.P("a", assetCode))
                        If asset.Rows.Count = 0 Then Throw New InvalidOperationException("Unknown asset code: " & assetCode)
                        Dim ref = HbmsService.CreateBreakdown(CInt(asset.Rows(0)("AssetID")), userId, severity, "[WARD NETWORK ALERT " & messageId & "] " & description)
                        Dim b = Db.Query("SELECT BreakdownID FROM Breakdowns WHERE BreakdownRef=?", Db.P("r", ref))
                        Db.Execute("INSERT INTO ImportedAlerts (MessageID,BreakdownID,ImportedAt,ImportStatus) VALUES (?,?,?,?)", Db.P("m", messageId), Db.P("b", CInt(b.Rows(0)("BreakdownID")), System.Data.OleDb.OleDbType.Integer), Db.P("t", DateTime.Now, System.Data.OleDb.OleDbType.Date), Db.P("s", "IMPORTED"))
                        imported += 1
                    End Using
                Catch ex As Exception
                    ' Keep the source line untouched. Failed records can be corrected/retried; surface the error to the operator.
                    Throw New InvalidOperationException("Alert inbox import failed for message " & If(messageId, "(unknown)") & ": " & ex.Message, ex)
                End Try
            Next
            Return imported
        End Function
    End Class
End Namespace
