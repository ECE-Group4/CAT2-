Imports System
Imports System.Drawing
Imports System.Windows.Forms
Imports HBMS.Desktop.Services

Namespace HBMS.Desktop.Forms
    Public Class MaintenanceUpdateForm
        Inherits Form
        Private ReadOnly _breakdownId As Integer
        Private ReadOnly _userId As Integer
        Private ReadOnly cboStatus As New ComboBox() With {.DropDownStyle = ComboBoxStyle.DropDownList, .Width = 210}
        Private ReadOnly txtNotes As New TextBox() With {.Multiline = True, .Width = 320, .Height = 100, .ScrollBars = ScrollBars.Vertical}
        Public Sub New(breakdownId As Integer, userId As Integer)
            _breakdownId = breakdownId : _userId = userId
            Text = "Update maintenance progress" : StartPosition = FormStartPosition.CenterParent : ClientSize = New Size(480, 270)
            cboStatus.Items.AddRange({"InProgress", "Repaired", "Verified", "Closed"}) : cboStatus.SelectedIndex = 0
            Controls.Add(New Label() With {.Text = "New status", .Location = New Point(20, 30), .AutoSize = True}) : cboStatus.Location = New Point(135, 25) : Controls.Add(cboStatus)
            Controls.Add(New Label() With {.Text = "Work / diagnosis / verification notes", .Location = New Point(20, 75), .AutoSize = True}) : txtNotes.Location = New Point(135, 70) : Controls.Add(txtNotes)
            Dim save As New Button() With {.Text = "Save update", .Location = New Point(165, 195), .Width = 120} : AddHandler save.Click, AddressOf SaveUpdate : Controls.Add(save)
        End Sub
        Private Sub SaveUpdate(sender As Object, e As EventArgs)
            Try
                HbmsService.AdvanceMaintenance(_breakdownId, CStr(cboStatus.SelectedItem), txtNotes.Text.Trim(), _userId)
                MessageBox.Show("Maintenance status updated.", "Saved", MessageBoxButtons.OK, MessageBoxIcon.Information) : DialogResult = DialogResult.OK : Close()
            Catch ex As Exception
                MessageBox.Show(ex.Message, "Unable to update maintenance", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            End Try
        End Sub
    End Class
End Namespace
