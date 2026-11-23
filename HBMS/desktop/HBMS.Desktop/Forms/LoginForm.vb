Imports System
Imports System.Drawing
Imports System.Windows.Forms
Imports HBMS.Desktop.Services

Namespace HBMS.Desktop.Forms
    Public Class LoginForm
        Inherits Form
        Private ReadOnly txtUser As New TextBox() With {.Text = "admin", .Width = 240}
        Private ReadOnly txtPassword As New TextBox() With {.PasswordChar = "●"c, .Width = 240}
        Public Sub New()
            Text = "HBMS | Sign in" : StartPosition = FormStartPosition.CenterScreen : FormBorderStyle = FormBorderStyle.FixedDialog : MaximizeBox = False : ClientSize = New Size(390, 245)
            Dim title As New Label() With {.Text = "Hospital Biomedical Maintenance System", .Font = New Font("Segoe UI", 13, FontStyle.Bold), .AutoSize = True, .Location = New Point(22, 22)}
            Dim l1 As New Label() With {.Text = "Username", .Location = New Point(25, 83), .AutoSize = True}
            Dim l2 As New Label() With {.Text = "Password", .Location = New Point(25, 125), .AutoSize = True}
            txtUser.Location = New Point(120, 78) : txtPassword.Location = New Point(120, 120)
            Dim btn As New Button() With {.Text = "Sign in", .Location = New Point(120, 168), .Width = 110}
            AddHandler btn.Click, AddressOf SignIn
            Controls.AddRange({title, l1, l2, txtUser, txtPassword, btn})
            AcceptButton = btn
        End Sub
        Private Sub SignIn(sender As Object, e As EventArgs)
            Try
                Dim user = HbmsService.Authenticate(txtUser.Text.Trim(), txtPassword.Text)
                If user Is Nothing Then
                    MessageBox.Show("The username or password is incorrect.", "Sign in failed", MessageBoxButtons.OK, MessageBoxIcon.Warning) : Return
                End If
                Hide()
                Using main As New MainForm(CInt(user("UserID")), CStr(user("FullName")), CStr(user("RoleName")))
                    main.ShowDialog()
                End Using
                Close()
            Catch ex As Exception
                MessageBox.Show("Unable to sign in. Confirm the database is configured and available." & Environment.NewLine & ex.Message, "HBMS", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End Sub
    End Class
End Namespace
