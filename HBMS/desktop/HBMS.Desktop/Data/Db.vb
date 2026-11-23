Imports System
Imports System.Data
Imports System.Data.OleDb

Namespace HBMS.Desktop.Database
    Public NotInheritable Class Db
        Private Sub New()
        End Sub
        Public Shared Property ConnectionString As String
        Public Shared Sub Initialize(path As String)
            ConnectionString = $"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={path};Persist Security Info=False;"
        End Sub
        Public Shared Function OpenConnection() As OleDbConnection
            Dim c As New OleDbConnection(ConnectionString)
            c.Open()
            Return c
        End Function
        Public Shared Function Query(sql As String, ParamArray parameters As OleDbParameter()) As DataTable
            Using c = OpenConnection(), cmd As New OleDbCommand(sql, c), da As New OleDbDataAdapter(cmd)
                If parameters IsNot Nothing Then cmd.Parameters.AddRange(parameters)
                Dim t As New DataTable()
                da.Fill(t)
                Return t
            End Using
        End Function
        Public Shared Function Execute(sql As String, ParamArray parameters As OleDbParameter()) As Integer
            Using c = OpenConnection(), cmd As New OleDbCommand(sql, c)
                If parameters IsNot Nothing Then cmd.Parameters.AddRange(parameters)
                Return cmd.ExecuteNonQuery()
            End Using
        End Function
        Public Shared Function P(name As String, value As Object, Optional type As OleDbType = OleDbType.VarWChar) As OleDbParameter
            Return New OleDbParameter(name, type) With {.Value = If(value, DBNull.Value)}
        End Function
    End Class
End Namespace
