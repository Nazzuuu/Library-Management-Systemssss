Imports System.Data
Imports MySql.Data.MySqlClient
Imports ZXing
Imports ZXing.Rendering
Imports ZXing.Windows.Compatibility
Public Class PrintReceiptForm


    Private WithEvents printDoc As New System.Drawing.Printing.PrintDocument

    Private DataToPrintList As New List(Of DataGridViewRow)
    Private PrintIndex As Integer = 0

    Private Sub PrintReceipt_Shown(sender As Object, e As EventArgs) Handles MyBase.Shown

        DataGridView1.ClearSelection()

    End Sub

    Private Sub GenerateBarcode(ByVal codeText As String)
        If String.IsNullOrWhiteSpace(codeText) Then
            Me.picbarcode.Image = Nothing
            Exit Sub
        End If

        Try
            Dim writer As New BarcodeWriter() With {
                .Format = BarcodeFormat.CODE_128,
                .Options = New ZXing.Common.EncodingOptions With {
                    .Width = Me.picbarcode.Width,
                    .Height = Me.picbarcode.Height,
                    .Margin = 1
                }
            }


            Dim barcodeBitmap As Bitmap = writer.Write(codeText)


            Me.picbarcode.Image = barcodeBitmap

        Catch ex As Exception
            MessageBox.Show("Error generating barcode: " & ex.Message, "Barcode Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub





    Private Sub DataGridView1_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellClick

        If e.RowIndex >= 0 Then
            Dim selectedRow As DataGridViewRow = Me.DataGridView1.Rows(e.RowIndex)

            Dim transactionID As String = selectedRow.Cells("TransactionReceipt").Value.ToString()


            Me.lbltransacno.Text = transactionID


            GenerateBarcode(transactionID)


        End If
    End Sub

    Private Sub btnprint_Click(sender As Object, e As EventArgs) Handles btnprint.Click


        If Me.DataGridView1.RowCount = 0 OrElse Me.DataGridView1.SelectedRows.Count = 0 Then
            MessageBox.Show("Please select a borrowing record to print.", "Print Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
            Exit Sub
        End If


        Dim selectedRow As DataGridViewRow = Me.DataGridView1.SelectedRows(0)
        Dim transacReceiptID As String = selectedRow.Cells("TransactionReceipt").Value?.ToString()

        If selectedRow.Cells("IsPrinted").Value IsNot DBNull.Value AndAlso selectedRow.Cells("IsPrinted").Value.ToString() = "1" Then
            MessageBox.Show($"Transaction No. {transacReceiptID} is already printed and cannot be printed again.", "Print Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
            Exit Sub
        End If

        DataToPrintList.Clear()

        DataToPrintList.Add(selectedRow)

        PrintIndex = 0

        Try
            Dim pdlg As New PrintDialog With {.Document = printDoc}

            If pdlg.ShowDialog() = DialogResult.OK Then
                printDoc.PrinterSettings = pdlg.PrinterSettings
                printDoc.Print()


                UpdatePrintedStatus(transacReceiptID)


                refreshreceipt()

                MessageBox.Show($"Receipt for Transaction No. {transacReceiptID} successfully printed and marked as printed.", "Print Success", MessageBoxButtons.OK, MessageBoxIcon.Information)

            End If
        Catch ex As Exception
            MessageBox.Show("Error starting print job: " & ex.Message, "Print Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            PrintIndex = 0
        End Try
    End Sub

    Public Sub UpdatePrintedStatus(ByVal transactionID As String)

        Dim con As New MySqlConnection(GlobalVarsModule.connectionString)
        Dim com As String = "UPDATE `printreceipt_tbl` SET `IsPrinted` = 1 WHERE `TransactionReceipt` = @TID"

        Try
            con.Open()
            Using cmd As New MySqlCommand(com, con)
                cmd.Parameters.AddWithValue("@TID", transactionID)
                cmd.ExecuteNonQuery()
            End Using
        Catch ex As Exception
            MessageBox.Show("Error updating printed status: " & ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
        End Try
    End Sub


    Public Function GenerateBarcodeImage(ByVal barcodeText As String, ByVal width As Integer, ByVal height As Integer) As Image
        If String.IsNullOrWhiteSpace(barcodeText) OrElse barcodeText = "0000000000000" Then
            Return Nothing
        End If

        Try

            Dim renderWidth As Integer = 300
            Dim renderHeight As Integer = 80
            Dim totalLabelHeight As Integer = 100

            Dim options As New ZXing.Common.EncodingOptions With {
                .Width = renderWidth,
                .Height = renderHeight,
                .Margin = 10,
                .PureBarcode = True
            }

            Dim writer As New BarcodeWriter(Of Bitmap) With {
                .Format = BarcodeFormat.CODE_128,
                .Options = options,
                .Renderer = New BitmapRenderer()
            }

            Dim barcodeBitmap As Bitmap = writer.Write(barcodeText)
            Dim printImage As New Bitmap(renderWidth, totalLabelHeight)

            Using g As Graphics = Graphics.FromImage(printImage)
                g.Clear(Color.White)
                g.DrawImage(barcodeBitmap, 0, 0, renderWidth, renderHeight)

                Using font As New Font("Arial", 8)
                    Using sf As New StringFormat With {.Alignment = StringAlignment.Center}
                        g.DrawString(barcodeText, font, Brushes.Black, New RectangleF(0, renderHeight, renderWidth, totalLabelHeight - renderHeight), sf)
                    End Using
                End Using
            End Using

            barcodeBitmap.Dispose()

            Return New Bitmap(printImage, New Size(width, height))

        Catch ex As Exception

            Dim bmp As New Bitmap(width, height)
            Using g As Graphics = Graphics.FromImage(bmp)
                g.Clear(Color.Red)
                Using font As New Font("Arial", 8)
                    g.DrawString("Barcode Error: " & barcodeText, font, Brushes.White, 5, 5)
                End Using
            End Using
            Return bmp
        End Try
    End Function
    Private Sub PrintDocument_PrintPage(sender As Object, e As Printing.PrintPageEventArgs) Handles printDoc.PrintPage

        Dim g As Graphics = e.Graphics
        Dim fontHeader As New Font("Arial", 12, FontStyle.Bold)
        Dim fontBody As New Font("Arial", 10, FontStyle.Regular)
        Dim margin As Integer = 50
        Dim yPos As Integer = margin
        Dim lineHeight As Integer = 20
        Dim logoImage As Image = Nothing



        If PrintIndex < DataToPrintList.Count Then
            Dim row As DataGridViewRow = DataToPrintList(PrintIndex)


            Dim name As String = row.Cells("Name").Value?.ToString()
            Dim borrowerType As String = row.Cells("Borrower").Value?.ToString()
            Dim bookTitle As String = row.Cells("BookTitle").Value?.ToString()
            Dim isbn As String = row.Cells("ISBN").Value?.ToString()
            Dim barcode As String = row.Cells("Barcode").Value?.ToString()
            Dim borrowedDate As String = row.Cells("BorrowedDate").Value?.ToString()
            Dim dueDate As String = row.Cells("DueDate").Value?.ToString()
            Dim transacReceipt As String = row.Cells("TransactionReceipt").Value?.ToString()

            Dim codeToPrint As String = If(String.IsNullOrWhiteSpace(barcode), isbn, barcode)

            g.DrawString("Monlimar Development Academy", fontHeader, Brushes.Black, margin, yPos)
            yPos += lineHeight
            g.DrawString("Library Management System - Borrowing Receipt", fontBody, Brushes.Black, margin, yPos)
            yPos += lineHeight + 10

            g.DrawLine(Pens.Black, margin, yPos, e.PageBounds.Width - margin, yPos)
            yPos += 10


            g.DrawString($"Transaction Receipt No: {transacReceipt}", fontHeader, Brushes.Black, margin, yPos)
            yPos += lineHeight * 2

            g.DrawString($"Borrower: {name} ({borrowerType})", fontBody, Brushes.Black, margin, yPos)
            yPos += lineHeight
            g.DrawString($"Date Borrowed: {borrowedDate}", fontBody, Brushes.Black, margin, yPos)
            yPos += lineHeight
            g.DrawString($"Due Date: {dueDate}", fontBody, Brushes.Black, margin, yPos)
            yPos += lineHeight * 2

            g.DrawString("Book Details:", fontHeader, Brushes.Black, margin, yPos)
            yPos += lineHeight

            g.DrawString($"Title: {bookTitle}", fontBody, Brushes.Black, margin, yPos)
            yPos += lineHeight
            g.DrawString($"ISBN/Barcode: {codeToPrint}", fontBody, Brushes.Black, margin, yPos)
            yPos += lineHeight * 2


            If Not String.IsNullOrEmpty(transacReceipt) Then

                Dim receiptBarcode As Image = GenerateBarcodeImage(transacReceipt, 200, 50)

                If receiptBarcode IsNot Nothing Then
                    Dim xPosBarcode As Integer = margin
                    g.DrawImage(receiptBarcode, xPosBarcode, yPos, receiptBarcode.Width, receiptBarcode.Height)
                    yPos += receiptBarcode.Height + 5

                    g.DrawString(transacReceipt, fontBody, Brushes.Black, xPosBarcode + (CSng(receiptBarcode.Width) / 2), yPos, New StringFormat With {.Alignment = StringAlignment.Center})
                    yPos += lineHeight * 2
                    receiptBarcode.Dispose()
                End If
            End If


            g.DrawString("Please return the book on or before the due date to avoid penalty.", fontBody, Brushes.Black, margin, yPos)
            yPos += lineHeight * 2
            g.DrawString("Signature: __________________________", fontBody, Brushes.Black, margin, yPos)


            PrintIndex += 1

            If PrintIndex < DataToPrintList.Count Then
                e.HasMorePages = True
            Else
                e.HasMorePages = False
            End If

        End If
    End Sub

    Public Sub LoadPrintReceiptDataByTransaction(ByVal transactionID As String)

        Dim con As New MySqlConnection(GlobalVarsModule.connectionString)

        Dim com As String = "SELECT * FROM `printreceipt_tbl` WHERE `TransactionReceipt` = @TID"

        Dim adap As New MySqlDataAdapter()
        Dim dt As New DataTable

        Try
            con.Open()
            Using cmd As New MySqlCommand(com, con)
                cmd.Parameters.AddWithValue("@TID", transactionID)
                adap.SelectCommand = cmd
                adap.Fill(dt)

                DataGridView1.DataSource = dt

                If dt.Rows.Count > 0 Then
                    Me.lbltransacno.Text = transactionID
                End If


                GenerateBarcode(transactionID)

            End Using
        Catch ex As Exception
            MessageBox.Show("Error loading print receipt data: " & ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
        End Try
    End Sub


    Private Sub DataGridView1_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellContentClick
    End Sub

    Private Sub PrintReceiptForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        refreshreceipt()
    End Sub

    Public Sub refreshreceipt()

        Dim con As New MySqlConnection(GlobalVarsModule.connectionString)
        Dim com As String = "SELECT * FROM `printreceipt_tbl` WHERE `IsPrinted` = 0"

        Dim adap As New MySqlDataAdapter(com, con)
        Dim ds As New DataSet

        adap.Fill(ds, "info")
        DataGridView1.DataSource = ds.Tables("info")

        DataGridView1.Columns("ID").Visible = False
        DataGridView1.EnableHeadersVisualStyles = False
        DataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(207, 58, 109)
        DataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White

    End Sub

    Private Sub txtsearch_TextChanged(sender As Object, e As EventArgs) Handles txtsearch.TextChanged

        Dim dt As DataTable = DirectCast(DataGridView1.DataSource, DataTable)
        If dt IsNot Nothing Then
            If txtsearch.Text.Trim() <> "" Then
                Dim filter As String = String.Format("Name LIKE '*{0}*'", txtsearch.Text.Trim())
                dt.DefaultView.RowFilter = filter
            Else
                dt.DefaultView.RowFilter = ""
            End If
        End If

    End Sub
End Class