Imports System.Windows.Forms

Public Class ReportsForm
    Private SelectedReport As String = ""
    Private ActiveButton As Guna.UI2.WinForms.Guna2Button = Nothing

    Private Sub SetActiveButton(btn As Guna.UI2.WinForms.Guna2Button)
        If ActiveButton IsNot Nothing Then
            ActiveButton.FillColor = Color.Gainsboro
        End If

        btn.FillColor = Color.White
        ActiveButton = btn
    End Sub


    Private Sub btnTransactionReport_Click(sender As Object, e As EventArgs) Handles btnTransactionReport.Click
        SetActiveButton(CType(sender, Guna.UI2.WinForms.Guna2Button))
        lblReportName.Text = "Borrowing History"
        lblReportsDescription.Text = "Generate a comprehensive report of all the books borrowed."
        SelectedReport = "BorrowingHistory"
    End Sub

    Private Sub btnLogHistoryReport_Click(sender As Object, e As EventArgs) Handles btnLogHistoryReport.Click
        SetActiveButton(CType(sender, Guna.UI2.WinForms.Guna2Button))
        lblReportName.Text = "Log History Report"
        lblReportsDescription.Text = "Track library visits with check-in and check-out times."
        SelectedReport = "LogHistory"
    End Sub

    Private Sub btnLostBooksReport_Click(sender As Object, e As EventArgs) Handles btnLostBooksReport.Click
        SetActiveButton(CType(sender, Guna.UI2.WinForms.Guna2Button))
        lblReportName.Text = "Lost Books Report"
        lblReportsDescription.Text = "Displays a record of all books reported as lost."
        SelectedReport = "LostBooks"
    End Sub

    Private Sub btnDamagedBooksReport_Click(sender As Object, e As EventArgs) Handles btnDamagedBooksReport.Click
        SetActiveButton(CType(sender, Guna.UI2.WinForms.Guna2Button))
        lblReportName.Text = "Damaged Books Report"
        lblReportsDescription.Text = "Displays a record of all books reported as damaged."
        SelectedReport = "DamagedBooks"
    End Sub


    Private Sub ReportsForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        btnTransactionReport.PerformClick()
    End Sub

    Private Sub btnGenerateReport_Click(sender As Object, e As EventArgs) Handles btnGenerateReport.Click
        Try
            Select Case SelectedReport
                Case "BorrowingHistory"
                    Dim frm As New BorrowingHistoryForm()
                    frm.ShowDialog()

                Case "LogHistory"
                    Dim frm As New LogHistoryForm()
                    frm.ShowDialog()

                Case "LostBooks"
                    Dim frm As New LostDamagedBooksForm("LostBooks")
                    frm.ShowDialog()

                Case "DamagedBooks"
                    Dim frm As New LostDamagedBooksForm("DamagedBooks")
                    frm.ShowDialog()

                Case Else
                    MessageBox.Show("Please select a report first.", "No Report Selected", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            End Select

        Catch ex As Exception
            MessageBox.Show("Error generating report: " & ex.Message, "Report Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
End Class
