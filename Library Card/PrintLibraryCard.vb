Imports Microsoft.Reporting.WinForms
Imports MySql.Data.MySqlClient
Imports System.Reflection
Imports System.Data
Imports System.Drawing
Imports System.IO
Imports ZXing
Imports ZXing.Rendering

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
        reportViewer.Name = "ReportViewer1"
        Me.Controls.Add(reportViewer)

        reportViewer.ProcessingMode = ProcessingMode.Local
        reportViewer.LocalReport.EnableExternalImages = True
    End Sub


    Private Async Sub PrintLibraryCard_Shown(sender As Object, e As EventArgs) Handles MyBase.Shown
        Try

            If _selectedIDs Is Nothing OrElse _selectedIDs.Count = 0 Then
                MessageBox.Show("No borrowers selected.", "Print Error")
                Exit Sub
            End If

            Dim dt As New DataTable

            dt.Columns.Add("FullName")
            dt.Columns.Add("Identifier")
            dt.Columns.Add("Department")
            dt.Columns.Add("LibrarianName")
            dt.Columns.Add("Barcode", GetType(Byte()))
            dt.Columns.Add("Borrower")

            Using con As New MySqlConnection(GlobalVarsModule.connectionString)

                Await con.OpenAsync()

                For Each selectedID In _selectedIDs

                    Dim query As String =
                    "SELECT b.FirstName, b.LastName, b.EmployeeNo, b.LRN, b.Department, b.Borrower, " &
                    "s.FirstName AS LibFirstName, s.LastName AS LibLastName " &
                    "FROM borrower_tbl b CROSS JOIN superadmin_tbl s " &
                    "WHERE b.ID = @id AND b.Borrower = 'Student'"

                    Using cmd As New MySqlCommand(query, con)

                        cmd.Parameters.AddWithValue("@id", selectedID)

                        Using reader As MySqlDataReader = Await cmd.ExecuteReaderAsync()

                            While Await reader.ReadAsync()

                                Dim row = dt.NewRow()

                                row("FullName") =
                                reader("FirstName").ToString() & " " &
                                reader("LastName").ToString()

                                row("Department") =
                                reader("Department").ToString()

                                row("LibrarianName") =
                                reader("LibFirstName").ToString() & " " &
                                reader("LibLastName").ToString()

                                Dim iden As String = If(Not String.IsNullOrEmpty(reader("LRN").ToString()),
                                                          reader("LRN").ToString(),
                                                          reader("EmployeeNo").ToString())

                                row("Identifier") = iden

                                Dim barcodeSource As String = iden

                                row("Barcode") = GenerateBarcodeBytes(barcodeSource)
                                row("Borrower") = If(reader.IsDBNull(reader.GetOrdinal("Borrower")), "", reader("Borrower").ToString())

                                dt.Rows.Add(row)

                            End While

                        End Using

                    End Using

                Next

            End Using



            reportViewer.Reset()

            reportViewer.LocalReport.EnableExternalImages = True
            reportViewer.ProcessingMode = ProcessingMode.Local


            reportViewer.LocalReport.DataSources.Clear()
            Dim rds As New ReportDataSource("DataSet1", dt)
            reportViewer.LocalReport.DataSources.Add(rds)


            Dim asm As Assembly = Assembly.GetExecutingAssembly()


            Dim resourceName As String = Nothing
            For Each res As String In asm.GetManifestResourceNames()
                If res.EndsWith("LibraryCard.rdlc", StringComparison.OrdinalIgnoreCase) Then
                    resourceName = res
                    Exit For
                End If
            Next

            If String.IsNullOrEmpty(resourceName) Then
                Dim available = String.Join(", ", asm.GetManifestResourceNames())
                Throw New Exception("Embedded RDLC 'LibraryCard.rdlc' not found. Available: " & available)
            End If

            Using stream = asm.GetManifestResourceStream(resourceName)
                If stream Is Nothing Then
                    Throw New Exception($"Failed to load embedded RDLC stream: {resourceName}")
                End If
                reportViewer.LocalReport.LoadReportDefinition(stream)
            End Using


            If Not reportViewer.IsHandleCreated Then
                reportViewer.CreateControl()
            End If

            reportViewer.LocalReport.Refresh()
            reportViewer.RefreshReport()

        Catch ex As Exception

            MessageBox.Show("Error loading Library Card report: " & ex.Message,
                        "Report Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error)

        End Try

    End Sub


    Private Function GenerateBarcodeBytes(text As String) As Byte()

        If String.IsNullOrWhiteSpace(text) Then
            text = "00000"
        End If

        Dim writer As New ZXing.Windows.Compatibility.BarcodeWriter()
        writer.Format = BarcodeFormat.CODE_128
        writer.Options = New ZXing.Common.EncodingOptions With {
            .Height = 50,
            .Width = 200,
            .PureBarcode = True
        }

        Using bmp = writer.Write(text)

            Using ms As New MemoryStream()

                bmp.Save(ms, Imaging.ImageFormat.Png)

                Return ms.ToArray()

            End Using

        End Using

    End Function

End Class