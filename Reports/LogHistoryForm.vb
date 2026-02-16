Imports Microsoft.Reporting.WinForms
Imports MySql.Data.MySqlClient
Imports System.Reflection
Imports System.Data

Public Class LogHistoryForm
    Inherits Form

    Private WithEvents reportViewer As New ReportViewer()

    Public Sub New()
        Me.Text = "ORAS Log History Report"
        Me.StartPosition = FormStartPosition.CenterScreen
        Me.ClientSize = New Size(900, 600)

        reportViewer.Dock = DockStyle.Fill
        reportViewer.Name = "ReportViewer1"
        Me.Controls.Add(reportViewer)
    End Sub

    Private Async Sub LogHistoryForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Dim dt As New DataTable()

        Using con As New MySqlConnection(GlobalVarsModule.connectionString)
            Await con.OpenAsync()

            Dim query As String =
                "SELECT
                    Borrower,
                    LRN,
                    EmployeeNo,
                    FirstName,
                    LastName,
                    MiddleInitial,
                    Department,
                    TimeIn,
                    TimeOut
                 FROM oras_tbl
                 ORDER BY TimeIn DESC;"

            Using cmd As New MySqlCommand(query, con)
                Using adapter As New MySqlDataAdapter(cmd)
                    adapter.Fill(dt)
                End Using
            End Using
        End Using

        reportViewer.LocalReport.DataSources.Clear()

        Dim rds As New ReportDataSource("DataSet_ActivityLogs", dt)
        reportViewer.LocalReport.DataSources.Add(rds)

        Dim asm As Assembly = Assembly.GetExecutingAssembly()
        Dim rdlcResource As String =
            "Library_Management_Systemssss.rptActivityLogs.rdlc"

        Using stream = asm.GetManifestResourceStream(rdlcResource)
            If stream Is Nothing Then
                Throw New Exception("Embedded RDLC not found: " & rdlcResource)
            End If
            reportViewer.LocalReport.LoadReportDefinition(stream)
        End Using

        reportViewer.RefreshReport()
    End Sub
End Class
