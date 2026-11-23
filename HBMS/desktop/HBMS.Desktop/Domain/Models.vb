Namespace HBMS.Desktop.Domain
    Public Enum Severity
        Low = 1
        Medium = 2
        High = 3
        Critical = 4
    End Enum
    Public Enum BreakdownStatus
        Reported
        Triaged
        Assigned
        Responded
        InProgress
        AwaitingParts
        Repaired
        Verified
        Closed
        Cancelled
    End Enum
    Public Class BreakdownItem
        Public Property BreakdownID As Integer
        Public Property BreakdownRef As String = ""
        Public Property AssetCode As String = ""
        Public Property EquipmentName As String = ""
        Public Property WardName As String = ""
        Public Property Severity As Severity
        Public Property ReportedAt As DateTime
        Public Property ResponseDeadline As DateTime
        Public Property Status As String = "Reported"
        Public ReadOnly Property IsOverdue As Boolean
            Get
                Return ResponseDeadline < DateTime.Now AndAlso Status <> "Closed" AndAlso Status <> "Cancelled"
            End Get
        End Property
    End Class
End Namespace
