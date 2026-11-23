Imports System
Imports System.Drawing
Imports System.Windows.Forms
Imports HBMS.Desktop.Services

Namespace HBMS.Desktop.Forms
    Public Class BreakdownForm
        Inherits Form
        Private ReadOnly _userId As Integer
        Private ReadOnly cboAsset As New ComboBox() With {.DropDownStyle = ComboBoxStyle.DropDownList, .Width = 330}
        Private ReadOnly cboSeverity As New ComboBox() With {.DropDownStyle = ComboBoxStyle.DropDownList, .Width = 180}
        Private ReadOnly txtDescription As New TextBox() With {.Multiline = True, .Width = 330, .Height = 105, .ScrollBars = ScrollBars.Vertical}
        Public Sub New(userId As Integer)
            _userId = userId : Text = "Report equipment breakdown" : StartPosition = FormStartPosition.CenterParent : ClientSize = New Size(510, 330)
            Dim assets = HbmsService.GetAssets() : cboAsset.DataSource = assets : cboAsset.DisplayMember = "AssetCode" : cboAsset.ValueMember = "AssetID"
            cboSeverity.Items.AddRange({"CRITICAL", "HIGH", "MEDIUM", "LOW"}) : cboSeverity.SelectedIndex = 0
            AddField("Equipment asset", cboAsset, 25) : AddField("Severity", cboSeverity, 75) : AddField("Fault description", txtDescription, 125)
            Dim save As New Button() With {.Text = "Submit breakdown", .Location = New Point(145, 260), .Width = 150}
            AddHandler save.Click, AddressOf SaveBreakdown : Controls.Add(save)
        End Sub
        Private Sub AddField(label As String, control As Control, y As Integer)
            Controls.Add(New Label() With {.Text = label, .Location = New Point(25, y), .AutoSize = True}) : control.Location = New Point(155, y - 4) : Controls.Add(control)
        End Sub
        Private Sub SaveBreakdown(sender As Object, e As EventArgs)
            Try
                Dim ref = HbmsService.CreateBreakdown(CInt(cboAsset.SelectedValue), _userId, CStr(cboSeverity.SelectedItem), txtDescription.Text.Trim())
                MessageBox.Show("Breakdown registered: " & ref, "Report saved", MessageBoxButtons.OK, MessageBoxIcon.Information) : DialogResult = DialogResult.OK : Close()
            Catch ex As Exception
                MessageBox.Show(ex.Message, "Unable to save breakdown", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            End Try
        End Sub
    End Class
End Namespace
