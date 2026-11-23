Imports System
Imports System.Security.Cryptography

Namespace HBMS.Desktop.Services
    Public NotInheritable Class PasswordHasher
        Private Sub New()
        End Sub
        Public Shared Function HashPassword(password As String) As String
            Dim salt(15) As Byte
            RandomNumberGenerator.Fill(salt)
            Const iterations As Integer = 150000
            Using derive As New Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256)
                Return $"PBKDF2${iterations}${Convert.ToBase64String(salt)}${Convert.ToBase64String(derive.GetBytes(32))}"
            End Using
        End Function
        Public Shared Function Verify(password As String, encoded As String) As Boolean
            Try
                Dim parts = encoded.Split("$"c)
                If parts.Length <> 4 OrElse parts(0) <> "PBKDF2" Then Return False
                Dim iterations = Integer.Parse(parts(1))
                Dim salt = Convert.FromBase64String(parts(2))
                Dim expected = Convert.FromBase64String(parts(3))
                Using derive As New Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256)
                    Return CryptographicOperations.FixedTimeEquals(derive.GetBytes(expected.Length), expected)
                End Using
            Catch
                Return False
            End Try
        End Function
    End Class
End Namespace
