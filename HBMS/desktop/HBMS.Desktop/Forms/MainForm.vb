Imports System
Imports System.Data
Imports System.Drawing
Imports System.IO
Imports System.Windows.Forms
Imports HBMS.Desktop.Services

Namespace HBMS.Desktop.Forms
    Public Class MainForm
        Inherits Form
        Private ReadOnly _userId As Integer
        Private ReadOnly _fullName As String
        Private ReadOnly _role As String
        Private ReadOnly gridBreakdowns As New DataGridView()
        Private ReadOnly gridAssets As New DataGridView()
        Private ReadOnly gridParts As New DataGridView()
        Private ReadOnly gridUptime As New DataGridView()
        Private ReadOnly refreshTimer As New Timer() With {.Interval = 60000}
        Private ReadOnly status As New StatusStrip()
        Public Sub New(userId As Integer, fullName As String, role As String)
            _userId = userId : _fullName = fullName : _role = role
            Text = "HBMS | Operations Console" : WindowState = FormWindowState.Maximized : MinimumSize = New Size(1000, 650)
            Dim header As New Panel() With {.Dock = DockStyle.Top, .Height = 66, .BackColor = Color.FromArgb(25, 53, 72)}
            header.Controls.Add(New Label() With {.Text = "HOSPITAL BIOMEDICAL MAINTENANCE", .ForeColor = Color.White, .Font = New Font("Segoe UI", 15, FontStyle.Bold), .Location = New Point(18, 8), .AutoSize = True})
            header.Controls.Add(New Label() With {.Text = _fullName & "  |  " & _role, .ForeColor = Color.WhiteSmoke, .Location = New Point(20, 39), .AutoSize = True})
            Dim tabs As New TabControl() With {.Dock = DockStyle.Fill}
            tabs.TabPages.Add(BuildBreakdownTab()) : tabs.TabPages.Add(BuildGridTab("Equipment assets", gridAssets, AddressOf RefreshAssets)) : tabs.TabPages.Add(BuildGridTab("Spare parts", gridParts, AddressOf RefreshParts)) : tabs.TabPages.Add(BuildGridTab("Department uptime", gridUptime, AddressOf RefreshUptime))
            status.Items.Add(New ToolStripStatusLabel("Ready"))
            Controls.Add(tabs) : Controls.Add(header) : Controls.Add(status)
            AddHandler refreshTimer.Tick, AddressOf OnTimerTick : refreshTimer.Start()
            AddHandler Shown, Sub() RefreshAll()
        End Sub
        Private Function BuildBreakdownTab() As TabPage
            Dim page As New TabPage("Breakdown queue")
            Dim toolbar As New FlowLayoutPanel() With {.Dock = DockStyle.Top, .Height = 48, .Padding = New Padding(6)}
            AddButton(toolbar, "Log breakdown", AddressOf LogBreakdown)
            AddButton(toolbar, "Manage registers", AddressOf ManageRegisters)
            AddButton(toolbar, "Record response", AddressOf RecordResponse)
            AddButton(toolbar, "Assign engineer", AddressOf AssignEngineer)
            AddButton(toolbar, "Update maintenance", AddressOf UpdateMaintenance)
            AddButton(toolbar, "Import ward alerts", AddressOf ImportAlerts)
            AddButton(toolbar, "Publish portal data", AddressOf PublishPortalData)
            AddButton(toolbar, "Refresh", Sub() RefreshBreakdowns())
            ConfigureGrid(gridBreakdowns)
            page.Controls.Add(gridBreakdowns) : page.Controls.Add(toolbar)
            Return page
        End Function
        Private Function BuildGridTab(caption As String, grid As DataGridView, refresh As Action) As TabPage
            Dim page As New TabPage(caption), toolbar As New FlowLayoutPanel() With {.Dock = DockStyle.Top, .Height = 45, .Padding = New Padding(6)}
            AddButton(toolbar, "Refresh", Sub() refresh())
            If caption = "Spare parts" Then
                AddButton(toolbar, "Record part usage", AddressOf RecordPartUsage)
                AddButton(toolbar, "Record repair cost", AddressOf RecordRepairCost)
            End If
            ConfigureGrid(grid) : page.Controls.Add(grid) : page.Controls.Add(toolbar) : Return page
        End Function
        Private Sub ConfigureGrid(grid As DataGridView)
            grid.Dock = DockStyle.Fill : grid.ReadOnly = True : grid.AllowUserToAddRows = False : grid.AllowUserToDeleteRows = False : grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill : grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect : grid.MultiSelect = False : grid.BackgroundColor = Color.White
        End Sub
        Private Sub AddButton(parent As FlowLayoutPanel, caption As String, action As Action)
            Dim b As New Button() With {.Text = caption, .AutoSize = True, .Height = 30, .Margin = New Padding(4)}
            AddHandler b.Click, Sub() action()
            parent.Controls.Add(b)
        End Sub
        Private Function SelectedBreakdownId() As Integer?
            If gridBreakdowns.CurrentRow Is Nothing Then Return Nothing
            Return Convert.ToInt32(gridBreakdowns.CurrentRow.Cells("BreakdownID").Value)
        End Function
        Private Sub RefreshAll()
            RefreshBreakdowns() : RefreshAssets() : RefreshParts() : RefreshUptime()
        End Sub
        Private Sub RefreshBreakdowns()
            Try
                gridBreakdowns.DataSource = HbmsService.GetBreakdowns()
                If gridBreakdowns.Columns.Contains("BreakdownID") Then gridBreakdowns.Columns("BreakdownID").Visible = False
                status.Items(0).Text = "Queue refreshed at " & DateTime.Now.ToString("HH:mm:ss")
            Catch ex As Exception
                MessageBox.Show(ex.Message, "Queue refresh failed", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End Sub
        Private Sub RefreshAssets()
            gridAssets.DataSource = HbmsService.GetAssets()
            If gridAssets.Columns.Contains("AssetID") Then gridAssets.Columns("AssetID").Visible = False
        End Sub
        Private Sub RefreshParts()
            gridParts.DataSource = HbmsService.GetParts()
            If gridParts.Columns.Contains("PartID") Then gridParts.Columns("PartID").Visible = False
        End Sub
        Private Sub RefreshUptime()
            Dim t = HbmsService.GetExecutiveUptime()
            If Not t.Columns.Contains("UptimePercent") Then t.Columns.Add("UptimePercent", GetType(String))
            For Each r As DataRow In t.Rows
                Dim total = CInt(r("AssetCount")), operational = If(IsDBNull(r("OperationalCount")), 0, CInt(r("OperationalCount")))
                r("UptimePercent") = If(total = 0, "N/A", (100.0 * operational / total).ToString("0.0") & "%")
            Next
            gridUptime.DataSource = t
        End Sub
        Private Sub ManageRegisters()
            If _role <> "ADMIN" AndAlso _role <> "SUPERVISOR" Then
                MessageBox.Show("Only administrators and supervisors can manage reference data.", "Access restricted", MessageBoxButtons.OK, MessageBoxIcon.Information) : Return
            End If
            Using f As New ReferenceDataForm()
                f.ShowDialog(Me)
            End Using
            RefreshAll()
        End Sub
        Private Sub LogBreakdown()
            Using f As New BreakdownForm(_userId)
                If f.ShowDialog(Me) = DialogResult.OK Then RefreshAll()
            End Using
        End Sub
        Private Sub RecordResponse()
            Dim id = SelectedBreakdownId() : If Not id.HasValue Then MessageBox.Show("Select a breakdown first.") : Return
            If MessageBox.Show("Record that the engineering team has responded to this breakdown?", "Confirm response", MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> DialogResult.Yes Then Return
            HbmsService.RecordResponse(id.Value, _userId) : RefreshBreakdowns()
        End Sub
        Private Sub AssignEngineer()
            Dim id = SelectedBreakdownId() : If Not id.HasValue Then MessageBox.Show("Select a breakdown first.") : Return
            Dim users = Database.Db.Query("SELECT UserID,FullName FROM Users WHERE IsActive=True AND RoleName IN ('ENGINEER','SUPERVISOR','ADMIN') ORDER BY FullName")
            If users.Rows.Count = 0 Then MessageBox.Show("Create an engineer account first.") : Return
            Using f As New Form() With {.Text = "Assign maintenance job", .StartPosition = FormStartPosition.CenterParent, .ClientSize = New Size(380, 130)}
                Dim cbo As New ComboBox() With {.DataSource = users, .DisplayMember = "FullName", .ValueMember = "UserID", .DropDownStyle = ComboBoxStyle.DropDownList, .Location = New Point(20, 20), .Width = 330}
                Dim ok As New Button() With {.Text = "Assign", .DialogResult = DialogResult.OK, .Location = New Point(140, 70)}
                f.Controls.AddRange({cbo, ok}) : f.AcceptButton = ok
                If f.ShowDialog(Me) = DialogResult.OK Then HbmsService.AssignEngineer(id.Value, CInt(cbo.SelectedValue))
            End Using
            RefreshBreakdowns()
        End Sub
        Private Sub UpdateMaintenance()
            Dim id = SelectedBreakdownId() : If Not id.HasValue Then MessageBox.Show("Select a breakdown first.") : Return
            Using f As New MaintenanceUpdateForm(id.Value, _userId)
                If f.ShowDialog(Me) = DialogResult.OK Then RefreshAll()
            End Using
        End Sub
        Private Sub PublishPortalData()
            Try
                Dim output = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "..", "portal", "data", "uptime.json"))
                HbmsService.PublishUptimeSnapshot(output)
                MessageBox.Show("Executive snapshot published to:" & Environment.NewLine & output, "Portal data", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Catch ex As Exception
                MessageBox.Show(ex.Message, "Unable to publish portal data", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End Sub
        Private Sub RecordPartUsage()
            Using f As New SparePartUsageForm()
                If f.ShowDialog(Me) = DialogResult.OK Then RefreshParts()
            End Using
        End Sub
        Private Sub RecordRepairCost()
            Using f As New RepairCostForm(_userId)
                f.ShowDialog(Me)
            End Using
        End Sub
        Private Sub ImportAlerts()
            Try
                Dim inbox = Environment.GetEnvironmentVariable("HBMS_ALERT_INBOX")
                If String.IsNullOrWhiteSpace(inbox) Then inbox = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "..", "integration", "inbox", "alerts.jsonl"))
                Dim count = AlertInboxImporter.ImportPending(inbox, _userId)
                MessageBox.Show(count & " new ward alert(s) imported.", "Alert import", MessageBoxButtons.OK, MessageBoxIcon.Information) : RefreshAll()
            Catch ex As Exception
                MessageBox.Show(ex.Message, "Alert import needs attention", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            End Try
        End Sub
        Private Sub OnTimerTick(sender As Object, e As EventArgs)
            Try
                HbmsService.RunEscalations()
                Dim overdue = Database.Db.Query("SELECT Count(*) AS C FROM EscalationLogs WHERE NotificationStatus='DISPLAYED'")
                If overdue.Rows.Count > 0 AndAlso CInt(overdue.Rows(0)("C")) > 0 Then status.Items(0).Text = "Attention: response escalations are recorded. Review the queue."
            Catch ex As Exception
                status.Items(0).Text = "Escalation check failed: " & ex.Message
            End Try
        End Sub
        Protected Overrides Sub OnFormClosed(e As FormClosedEventArgs)
            refreshTimer.Stop() : MyBase.OnFormClosed(e)
        End Sub
    End Class
End Namespace
