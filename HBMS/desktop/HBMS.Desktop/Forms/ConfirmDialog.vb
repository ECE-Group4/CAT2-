Imports System.Drawing
Imports System.Windows.Forms

Namespace HBMS.Desktop.Forms
    Public Class ConfirmDialog
        Inherits Form
        Public Sub New(title As String, message As String)
            Text = title : StartPosition = FormStartPosition.CenterParent : FormBorderStyle = FormBorderStyle.FixedDialog : MinimizeBox = False : MaximizeBox = False : ClientSize = New Size(390, 155)
            Controls.Add(New Label() With {.Text = message, .Location = New Point(18, 18), .Size = New Size(350, 55)})
            Dim yes As New Button() With {.Text = "Confirm", .DialogResult = DialogResult.Yes, .Location = New Point(185, 100), .Width = 85}
            Dim no As New Button() With {.Text = "Cancel", .DialogResult = DialogResult.No, .Location = New Point(280, 100), .Width = 85}
            Controls.AddRange({yes, no}) : AcceptButton = yes : CancelButton = no
        End Sub
    End Class
End Namespace
