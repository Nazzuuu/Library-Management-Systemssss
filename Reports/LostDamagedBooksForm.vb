Imports Microsoft.Reporting.WinForms
Imports MySql.Data.MySqlClient
Imports System.Reflection
Imports System.Data

Public Class LostDamagedBooksForm
    Inherits Form

    Private ReadOnly reportType As String
    Private WithEvents reportViewer As New ReportViewer()

    Public Sub New(reportType As String)
        InitializeComponent()
        Me.reportType = reportType
        Me.Text = If(reportType = "LostBooks", "Lost Books Report", "Damaged Books Report")
        Me.StartPosition = FormStartPosition.CenterScreen
        Me.ClientSize = New Size(900, 700)
        reportViewer.Dock = DockStyle.Fill
        reportViewer.Name = "ReportViewer1"
        Me.Controls.Add(reportViewer)
    End Sub

    Private Async Sub LostDamagedBooksForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            Dim dt As New DataTable()

            Using con As New MySqlConnection(GlobalVarsModule.connectionString)
                Await con.OpenAsync()

                Dim query As String = "SELECT Borrower, FullName, ReturnedBook, BorrowedDate, DueDate, ReturnDate, TransactionReceipt, Status " &
                                      "FROM penalty_tbl WHERE Status LIKE @Status ORDER BY BorrowedDate DESC;"

                Using cmd As New MySqlCommand(query, con)
                    cmd.Parameters.AddWithValue("@Status", If(reportType = "LostBooks", "Lost%", "Damaged%"))

                    Using adapter As New MySqlDataAdapter(cmd)
                        adapter.Fill(dt)
                    End Using
                End Using
            End Using

            reportViewer.LocalReport.DataSources.Clear()

            Dim rds As New ReportDataSource("DataSet_Penalty", dt)
            reportViewer.LocalReport.DataSources.Add(rds)

            Dim assembly As Assembly = Assembly.GetExecutingAssembly()
            Dim rdlcResource As String = If(reportType = "LostBooks",
                                            "Library_Management_Systemssss.rptLostBooks.rdlc",
                                            "Library_Management_Systemssss.rptDamagedBooks.rdlc")

            Using stream = assembly.GetManifestResourceStream(rdlcResource)
                If stream Is Nothing Then
                    Throw New Exception($"Embedded RDLC not found: {rdlcResource}")
                End If
                reportViewer.LocalReport.LoadReportDefinition(stream)
            End Using

            reportViewer.RefreshReport()

        Catch ex As Exception
            MessageBox.Show("Error loading report: " & ex.Message, "Report Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
End Class