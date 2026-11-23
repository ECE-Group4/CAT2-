Imports System.Collections.Generic
Imports System.Linq
Imports HBMS.Desktop.Domain

Namespace HBMS.Desktop.Services
    ' Binary max-heap: Critical > High > Medium > Low; FIFO for equal priority.
    Public Class PriorityQueueService
        Private ReadOnly _heap As New List(Of (Item As BreakdownItem, Sequence As Long))()
        Private _sequence As Long
        Private Shared Function Higher(a As (Item As BreakdownItem, Sequence As Long), b As (Item As BreakdownItem, Sequence As Long)) As Boolean
            If a.Item.Severity <> b.Item.Severity Then Return CInt(a.Item.Severity) > CInt(b.Item.Severity)
            Return a.Sequence < b.Sequence
        End Function
        Public Sub Enqueue(item As BreakdownItem)
            _heap.Add((item, _sequence)) : _sequence += 1
            Dim i = _heap.Count - 1
            While i > 0
                Dim p = (i - 1) \ 2
                If Not Higher(_heap(i), _heap(p)) Then Exit While
                Dim swap = _heap(i) : _heap(i) = _heap(p) : _heap(p) = swap : i = p
            End While
        End Sub
        Public Function Dequeue() As BreakdownItem
            If _heap.Count = 0 Then Throw New InvalidOperationException("Priority queue is empty.")
            Dim result = _heap(0).Item, last = _heap(_heap.Count - 1)
            _heap.RemoveAt(_heap.Count - 1)
            If _heap.Count > 0 Then
                _heap(0) = last : Dim i = 0
                Do
                    Dim l = 2 * i + 1, r = l + 1, best = i
                    If l < _heap.Count AndAlso Higher(_heap(l), _heap(best)) Then best = l
                    If r < _heap.Count AndAlso Higher(_heap(r), _heap(best)) Then best = r
                    If best = i Then Exit Do
                    Dim swap = _heap(i) : _heap(i) = _heap(best) : _heap(best) = swap : i = best
                Loop
            End If
            Return result
        End Function
        Public Function Snapshot() As IReadOnlyList(Of BreakdownItem)
            Return _heap.OrderByDescending(Function(x) CInt(x.Item.Severity)).ThenBy(Function(x) x.Sequence).Select(Function(x) x.Item).ToList()
        End Function
        Public ReadOnly Property Count As Integer
            Get
                Return _heap.Count
            End Get
        End Property
    End Class
End Namespace
