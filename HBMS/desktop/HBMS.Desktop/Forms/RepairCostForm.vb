Imports System
Imports System.Drawing
Imports System.Windows.Forms
Imports HBMS.Desktop.Services

Namespace HBMS.Desktop.Forms
    Public Class RepairCostForm
        Inherits Form
        Private ReadOnly _userId As Integer
        Private ReadOnly txtJob As New TextBox() With {.Text = "1", .Width = 110}
        Private ReadOnly cboType As New ComboBox() With {.DropDownStyle = ComboBoxStyle.DropDownList, .Width = 150}
        Private ReadOnly txtDescription As New TextBox() With {.Width = 240}
        Private ReadOnly numAmount As New NumericUpDown() With {.DecimalPlaces = 2, .Maximum = 100000000, .Width = 150}
        Public Sub New(userId As Integer)
            _userId = userId : Text = "Record repair cost" : StartPosition = FormStartPosition.CenterParent : ClientSize = New Size(450, 270)
            cboType.Items.AddRange({"LABOUR", "PARTS", "VENDOR", "OTHER"}) : cboType.SelectedIndex = 0
            AddField("Maintenance job ID", txtJob, 30) : AddField("Cost category", cboType, 75) : AddField("Description", txtDescription, 120) : AddField("Amount", numAmount, 165)
            Dim save As New Button() With {.Text = "Save cost", .Location = New Point(155, 215)} : AddHandler save.Click, AddressOf SaveCost : Controls.Add(save)
        End Sub
        Private Sub AddField(label As String, c As Control, y As Integer)
            Controls.Add(New Label() With {.Text = label, .Location = New Point(20, y), .AutoSize = True}) : c.Location = New Point(150, y - 4) : Controls.Add(c)
        End Sub
        Private Sub SaveCost(sender As Object, e As EventArgs)
            Try
                HbmsService.AddRepairCost(Integer.Parse(txtJob.Text), CStr(cboType.SelectedItem), txtDescription.Text.Trim(), numAmount.Value, _userId)
                MessageBox.Show("Repair cost saved.", "Saved", MessageBoxButtons.OK, MessageBoxIcon.Information) : DialogResult = DialogResult.OK : Close()
            Catch ex As Exception
                MessageBox.Show(ex.Message, "Unable to save cost", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            End Try
        End Sub
    End Class
End Namespace
