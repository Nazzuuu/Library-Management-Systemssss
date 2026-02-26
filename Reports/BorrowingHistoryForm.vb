Imports Microsoft.Reporting.WinForms
Imports MySql.Data.MySqlClient
Imports System.Reflection
Imports System.Data

Public Class BorrowingHistoryForm
    Inherits Form

    Private WithEvents reportViewer As New ReportViewer()

    Public Sub New()
        InitializeComponent()
        Me.Text = "Borrowing History Report"
        Me.StartPosition = FormStartPosition.CenterScreen
        Me.ClientSize = New Size(900, 600)
        reportViewer.Dock = DockStyle.Fill
        reportViewer.Name = "ReportViewer1"
        Me.Controls.Add(reportViewer)
    End Sub

    Private Async Sub BorrowingHistoryForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            Dim dt As New DataTable()

            Using con As New MySqlConnection(GlobalVarsModule.connectionString)
                Await con.OpenAsync()

                Dim query As String =
                    "SELECT Borrower, ISBN, Barcode, AccessionID, BookTitle, Name, BorrowedDate, DueDate, TransactionReceipt " &
                    "FROM borrowinghistory_tbl " &
                    "WHERE Status = 'Granted' " &
                    "ORDER BY BorrowedDate DESC;"

                Using cmd As New MySqlCommand(query, con)
                    Using adapter As New MySqlDataAdapter(cmd)
                        adapter.Fill(dt)
                    End Using
                End Using
            End Using

            reportViewer.LocalReport.DataSources.Clear()

            Dim rds As New ReportDataSource("DataSet_Borrow", dt)
            reportViewer.LocalReport.DataSources.Add(rds)
            Dim assembly As Assembly = Assembly.GetExecutingAssembly()
            Dim rdlcResource As String = "Library_Management_Systemssss.rptBorrowing.rdlc"

            Using stream = assembly.GetManifestResourceStream(rdlcResource)
                If stream Is Nothing Then
                    Throw New Exception($"Embedded RDLC not found: {rdlcResource}")
                End If
                reportViewer.LocalReport.LoadReportDefinition(stream)
            End Using

            reportViewer.RefreshReport()

        Catch ex As Exception
            MessageBox.Show("Error loading Borrowing History report: " & ex.Message,
                            "Report Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
End Class