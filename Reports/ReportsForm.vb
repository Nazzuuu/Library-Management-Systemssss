Public Class ReportsForm
    Private SelectedReport As String = ""
    Private Sub btnTransactionReport_Click(sender As Object, e As EventArgs) Handles btnTransactionReport.Click
        lblReportName.Text = "Borrowing History"
        lblReportsDescription.Text = "Generate a comprehensive report of all the books borrowed."
        SelectedReport = "BorrowingHistory"
    End Sub

    Private Sub btnLogHistoryReport_Click(sender As Object, e As EventArgs) Handles btnLogHistoryReport.Click
        lblReportName.Text = "Log History Report"
        lblReportsDescription.Text = "Track library visits with check-in and check-out times."
        SelectedReport = "LogHistory"
    End Sub

    Private Sub btnLostBooksReport_Click(sender As Object, e As EventArgs) Handles btnLostBooksReport.Click
        lblReportName.Text = "Lost Books Report"
        lblReportsDescription.Text = "Displays a record of all books reported as lost."
        SelectedReport = "LostBooks"
    End Sub

    Private Sub btnDamagedBooksReport_Click(sender As Object, e As EventArgs) Handles btnDamagedBooksReport.Click
        lblReportName.Text = "Damaged Books Report"
        lblReportsDescription.Text = "Displays a record of all books reported as damaged."
        SelectedReport = "DamagedBooks"
    End Sub

    Private Sub ReportsForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        btnTransactionReport.PerformClick()
    End Sub

    Private Sub btnGenerateReport_Click(sender As Object, e As EventArgs) Handles btnGenerateReport.Click

    End Sub
End Class
