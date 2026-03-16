Imports Microsoft.Reporting.WinForms
Imports MySql.Data.MySqlClient
Imports System.Reflection
Imports System.Data
Imports System.Drawing
Imports System.IO
Imports ZXing

Public Class PrintLibraryCard
    Inherits Form

    Private ReadOnly _selectedIDs As List(Of Integer)
    Private WithEvents reportViewer As New ReportViewer()

    Public Sub New(selectedIDs As List(Of Integer))
        InitializeComponent()
        _selectedIDs = selectedIDs
        Me.Text = "Library Card Preview"
        Me.StartPosition = FormStartPosition.CenterScreen
        Me.ClientSize = New Size(900, 700)

        reportViewer.Dock = DockStyle.Fill
        Me.Controls.Add(reportViewer)
        reportViewer.ProcessingMode = ProcessingMode.Local
    End Sub

    Private Async Sub PrintLibraryCard_Shown(sender As Object, e As EventArgs) Handles MyBase.Shown
        Try
            If _selectedIDs Is Nothing OrElse _selectedIDs.Count = 0 Then
                MessageBox.Show("No borrowers selected.", "Print Error")
                Exit Sub
            End If

            Dim dt As New DataTable
            dt.Columns.Add("ID", GetType(Integer))
            dt.Columns.Add("FullName")
            dt.Columns.Add("Identifier")
            dt.Columns.Add("Department")
            dt.Columns.Add("LibrarianName")
            dt.Columns.Add("Barcode", GetType(Byte()))
            dt.Columns.Add("Borrower")

            Dim idList As String = String.Join(",", _selectedIDs)

            Using con As New MySqlConnection(GlobalVarsModule.connectionString)
                Await con.OpenAsync()

                Dim query As String =
                    "SELECT b.ID, b.FirstName, b.LastName, b.EmployeeNo, b.LRN, b.Department, b.Borrower, " &
                    "s.FirstName AS LibFirstName, s.LastName AS LibLastName " &
                    "FROM borrower_tbl b CROSS JOIN superadmin_tbl s " &
                    $"WHERE b.ID IN ({idList}) AND b.Borrower = 'Student'"

                Using cmd As New MySqlCommand(query, con)
                    Using reader As MySqlDataReader = Await cmd.ExecuteReaderAsync()
                        While Await reader.ReadAsync()
                            Dim row As DataRow = dt.NewRow()

                            row("ID") = Convert.ToInt32(reader("ID"))
                            row("FullName") = reader("FirstName").ToString() & " " & reader("LastName").ToString()
                            row("Department") = reader("Department").ToString()
                            row("LibrarianName") = reader("LibFirstName").ToString() & " " & reader("LibLastName").ToString()

                            Dim iden As String = If(Not String.IsNullOrEmpty(reader("LRN").ToString()), reader("LRN").ToString(), reader("EmployeeNo").ToString())
                            row("Identifier") = iden
                            row("Barcode") = GenerateBarcodeBytes(iden)
                            row("Borrower") = If(reader.IsDBNull(reader.GetOrdinal("Borrower")), "", reader("Borrower").ToString())

                            dt.Rows.Add(row)
                        End While
                    End Using
                End Using
            End Using

            reportViewer.Reset()
            reportViewer.LocalReport.EnableExternalImages = True
            reportViewer.ProcessingMode = ProcessingMode.Local
            reportViewer.LocalReport.DataSources.Clear()
            reportViewer.LocalReport.DataSources.Add(New ReportDataSource("DataSet1", dt))

            Dim asm As Assembly = Assembly.GetExecutingAssembly()
            Dim resourceName As String = asm.GetManifestResourceNames().FirstOrDefault(Function(n) n.EndsWith("LibraryCard.rdlc", StringComparison.OrdinalIgnoreCase))

            If resourceName Is Nothing Then Throw New Exception("RDLC file not found.")

            Using stream = asm.GetManifestResourceStream(resourceName)
                reportViewer.LocalReport.LoadReportDefinition(stream)
            End Using

            reportViewer.RefreshReport()

        Catch ex As Exception
            MessageBox.Show("Error loading report: " & ex.Message, "Report Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Function GenerateBarcodeBytes(text As String) As Byte()
        Dim writer As New ZXing.Windows.Compatibility.BarcodeWriter()
        writer.Format = BarcodeFormat.CODE_128
        writer.Options = New ZXing.Common.EncodingOptions With {
            .Height = 50, .Width = 200, .PureBarcode = True
        }
        Using bmp = writer.Write(If(String.IsNullOrWhiteSpace(text), "00000", text))
            Using ms As New MemoryStream()
                bmp.Save(ms, Imaging.ImageFormat.Png)
                Return ms.ToArray()
            End Using
        End Using
    End Function
End Class