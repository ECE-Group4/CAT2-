Imports System
Imports System.Drawing
Imports System.Windows.Forms
Imports HBMS.Desktop.Services

Namespace HBMS.Desktop.Forms
    Public Class SparePartUsageForm
        Inherits Form
        Private ReadOnly cboPart As New ComboBox() With {.DropDownStyle = ComboBoxStyle.DropDownList, .Width = 280}
        Private ReadOnly txtJob As New TextBox() With {.Text = "1", .Width = 100}
        Private ReadOnly numQty As New NumericUpDown() With {.Minimum = 1, .Maximum = 10000, .Value = 1, .Width = 100}
        Public Sub New()
            Text = "Record spare-part usage" : StartPosition = FormStartPosition.CenterParent : ClientSize = New Size(430, 230)
            cboPart.DataSource = HbmsService.GetParts() : cboPart.DisplayMember = "PartName" : cboPart.ValueMember = "PartID"
            AddField("Maintenance job ID", txtJob, 35) : AddField("Spare part", cboPart, 80) : AddField("Quantity used", numQty, 125)
            Dim save As New Button() With {.Text = "Record usage", .Location = New Point(150, 175)} : AddHandler save.Click, AddressOf SaveUsage : Controls.Add(save)
        End Sub
        Private Sub AddField(label As String, c As Control, y As Integer)
            Controls.Add(New Label() With {.Text = label, .Location = New Point(20, y), .AutoSize = True}) : c.Location = New Point(145, y - 4) : Controls.Add(c)
        End Sub
        Private Sub SaveUsage(sender As Object, e As EventArgs)
            Try
                HbmsService.AddPartUsage(Integer.Parse(txtJob.Text), CInt(cboPart.SelectedValue), CInt(numQty.Value))
                MessageBox.Show("Part usage recorded and stock updated.", "Saved", MessageBoxButtons.OK, MessageBoxIcon.Information) : DialogResult = DialogResult.OK : Close()
            Catch ex As Exception
                MessageBox.Show(ex.Message, "Unable to record usage", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            End Try
        End Sub
    End Class
End Namespace
