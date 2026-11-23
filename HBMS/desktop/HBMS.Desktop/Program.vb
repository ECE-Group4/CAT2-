Imports System
Imports System.Windows.Forms

Namespace HBMS.Desktop
    Friend Module Program
        <STAThread>
        Public Sub Main()
            ApplicationConfiguration.Initialize()
            Dim dbPath = Environment.GetEnvironmentVariable("HBMS_DB_PATH")
            If String.IsNullOrWhiteSpace(dbPath) Then dbPath = IO.Path.GetFullPath(IO.Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "..", "database", "HBMS.accdb"))
            Database.Db.Initialize(dbPath)
            Application.Run(New Forms.LoginForm())
        End Sub
    End Module
End Namespace
