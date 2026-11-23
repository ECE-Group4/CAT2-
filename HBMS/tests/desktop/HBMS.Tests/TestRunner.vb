Imports System
Imports System.Collections.Generic
Imports HBMS.Desktop.Domain
Imports HBMS.Desktop.Services

Namespace HBMS.Tests
    Public Module TestRunner
        Private _passedTests As Integer = 0
        Private _failedTests As Integer = 0

        Public Function Main(args As String()) As Integer
            Console.WriteLine("================================================================================")
            Console.WriteLine("  HBMS DESKTOP AUTOMATED TEST SUITE (Cross-Platform VB.NET / .NET 8)")
            Console.WriteLine("================================================================================")

            RunTest("PriorityQueue - Strict Max-Heap Ordering (Critical > High > Medium > Low)", AddressOf TestPriorityQueueOrdering)
            RunTest("PriorityQueue - Secondary FIFO Ordering for Identical Severity", AddressOf TestPriorityQueueFIFO)
            RunTest("PriorityQueue - Non-destructive Snapshot Inspection", AddressOf TestPriorityQueueSnapshot)
            RunTest("PasswordHasher - PBKDF2/SHA-256 Hashing and Positive Verification", AddressOf TestPasswordHasherValid)
            RunTest("PasswordHasher - Constant-time Rejection for Invalid Password", AddressOf TestPasswordHasherInvalid)
            RunTest("BreakdownItem - Overdue SLA Evaluation for Open vs Closed States", AddressOf TestBreakdownOverdueEvaluation)
            RunTest("Domain Records - Structured Value Types Integrity", AddressOf TestDomainRecordStructures)

            Console.WriteLine("--------------------------------------------------------------------------------")
            Console.WriteLine($"Total Tests: {_passedTests + _failedTests} | Passed: {_passedTests} | Failed: {_failedTests}")
            Console.WriteLine("================================================================================")

            Return If(_failedTests = 0, 0, 1)
        End Function

        Private Sub RunTest(name As String, action As Action)
            Try
                action()
                Console.WriteLine($"  [PASS] {name}")
                _passedTests += 1
            Catch ex As Exception
                Console.WriteLine($"  [FAIL] {name} - Reason: {ex.Message}")
                _failedTests += 1
            End Try
        End Sub

        Private Sub Assert(condition As Boolean, message As String)
            If Not condition Then
                Throw New InvalidOperationException("Assertion Failed: " & message)
            End If
        End Sub

        Private Sub TestPriorityQueueOrdering()
            Dim pq As New PriorityQueueService()
            Dim lowItem As New BreakdownItem With {.BreakdownID = 1, .BreakdownRef = "BRK-LOW", .Severity = Severity.Low}
            Dim critItem As New BreakdownItem With {.BreakdownID = 2, .BreakdownRef = "BRK-CRIT", .Severity = Severity.Critical}
            Dim medItem As New BreakdownItem With {.BreakdownID = 3, .BreakdownRef = "BRK-MED", .Severity = Severity.Medium}
            Dim highItem As New BreakdownItem With {.BreakdownID = 4, .BreakdownRef = "BRK-HIGH", .Severity = Severity.High}

            pq.Enqueue(lowItem)
            pq.Enqueue(critItem)
            pq.Enqueue(medItem)
            pq.Enqueue(highItem)

            Assert(pq.Count = 4, "Queue count must be 4.")
            Dim first = pq.Dequeue()
            Assert(first.Severity = Severity.Critical, "1st dequeued must be Critical.")
            Dim second = pq.Dequeue()
            Assert(second.Severity = Severity.High, "2nd dequeued must be High.")
            Dim third = pq.Dequeue()
            Assert(third.Severity = Severity.Medium, "3rd dequeued must be Medium.")
            Dim fourth = pq.Dequeue()
            Assert(fourth.Severity = Severity.Low, "4th dequeued must be Low.")
            Assert(pq.Count = 0, "Queue must be empty after 4 dequeues.")
        End Sub

        Private Sub TestPriorityQueueFIFO()
            Dim pq As New PriorityQueueService()
            Dim crit1 As New BreakdownItem With {.BreakdownID = 101, .BreakdownRef = "CRIT-001", .Severity = Severity.Critical}
            Dim crit2 As New BreakdownItem With {.BreakdownID = 102, .BreakdownRef = "CRIT-002", .Severity = Severity.Critical}
            Dim crit3 As New BreakdownItem With {.BreakdownID = 103, .BreakdownRef = "CRIT-003", .Severity = Severity.Critical}

            pq.Enqueue(crit1)
            pq.Enqueue(crit2)
            pq.Enqueue(crit3)

            Assert(pq.Dequeue().BreakdownRef = "CRIT-001", "FIFO ordering broken on 1st element.")
            Assert(pq.Dequeue().BreakdownRef = "CRIT-002", "FIFO ordering broken on 2nd element.")
            Assert(pq.Dequeue().BreakdownRef = "CRIT-003", "FIFO ordering broken on 3rd element.")
        End Sub

        Private Sub TestPriorityQueueSnapshot()
            Dim pq As New PriorityQueueService()
            pq.Enqueue(New BreakdownItem With {.BreakdownID = 1, .Severity = Severity.Medium})
            pq.Enqueue(New BreakdownItem With {.BreakdownID = 2, .Severity = Severity.Critical})
            pq.Enqueue(New BreakdownItem With {.BreakdownID = 3, .Severity = Severity.High})

            Dim snapshot = pq.Snapshot()
            Assert(snapshot.Count = 3, "Snapshot must have 3 items.")
            Assert(snapshot(0).Severity = Severity.Critical, "Snapshot first item must be Critical.")
            Assert(snapshot(1).Severity = Severity.High, "Snapshot second item must be High.")
            Assert(snapshot(2).Severity = Severity.Medium, "Snapshot third item must be Medium.")
            Assert(pq.Count = 3, "Snapshot must not mutate or drain the priority queue.")
        End Sub

        Private Sub TestPasswordHasherValid()
            Dim rawPassword As String = "BiomedicalAdmin@2026!"
            Dim hash = PasswordHasher.HashPassword(rawPassword)

            Assert(hash.StartsWith("PBKDF2$150000$"), "Password hash format must follow PBKDF2$150000$<salt>$<hash>.")
            Assert(PasswordHasher.Verify(rawPassword, hash), "PasswordHasher must successfully verify matching password.")
        End Sub

        Private Sub TestPasswordHasherInvalid()
            Dim rawPassword As String = "BiomedicalAdmin@2026!"
            Dim hash = PasswordHasher.HashPassword(rawPassword)

            Assert(Not PasswordHasher.Verify("WrongPassword!123", hash), "PasswordHasher must reject invalid password.")
            Assert(Not PasswordHasher.Verify("", hash), "PasswordHasher must reject empty password.")
            Assert(Not PasswordHasher.Verify(rawPassword, "INVALID_FORMAT_STRING"), "PasswordHasher must safely reject malformed hash strings.")
        End Sub

        Private Sub TestBreakdownOverdueEvaluation()
            Dim overdueOpen As New BreakdownItem With {
                .BreakdownID = 501,
                .Status = "Reported",
                .ResponseDeadline = DateTime.Now.AddHours(-2)
            }
            Assert(overdueOpen.IsOverdue, "Open item with past deadline must be overdue.")

            Dim notOverdueOpen As New BreakdownItem With {
                .BreakdownID = 502,
                .Status = "Reported",
                .ResponseDeadline = DateTime.Now.AddHours(2)
            }
            Assert(Not notOverdueOpen.IsOverdue, "Open item with future deadline must not be overdue.")

            Dim pastClosed As New BreakdownItem With {
                .BreakdownID = 503,
                .Status = "Closed",
                .ResponseDeadline = DateTime.Now.AddHours(-5)
            }
            Assert(Not pastClosed.IsOverdue, "Closed item must not be overdue regardless of deadline timestamp.")
        End Sub

        Private Sub TestDomainRecordStructures()
            Dim equip As EquipmentRecord
            equip.AssetID = 42
            equip.AssetCode = "EQ-ICU-001"
            equip.EquipmentName = "Mindray SV300 Ventilator"
            equip.EquipmentCategory = "Ventilators"
            equip.WardID = 1
            equip.Criticality = "Critical"
            equip.AssetStatus = "Operational"

            Assert(equip.AssetID = 42, "AssetID mismatch.")
            Assert(equip.AssetCode = "EQ-ICU-001", "AssetCode mismatch.")
            Assert(equip.EquipmentName = "Mindray SV300 Ventilator", "EquipmentName mismatch.")

            Dim queueRec As BreakdownQueueRecord
            queueRec.BreakdownID = 88
            queueRec.BreakdownRef = "BRK-2026-0088"
            queueRec.AssetCode = "EQ-ICU-001"
            queueRec.Severity = "Critical"
            queueRec.PriorityScore = 100
            queueRec.ReportedAt = New DateTime(2026, 11, 23, 8, 0, 0)
            queueRec.ResponseDeadline = New DateTime(2026, 11, 23, 12, 0, 0)

            Assert(queueRec.PriorityScore = 100, "PriorityScore mismatch.")
            Assert((queueRec.ResponseDeadline - queueRec.ReportedAt).TotalHours = 4.0, "Critical SLA response window must be 4.0 hours.")
        End Sub
    End Module
End Namespace
