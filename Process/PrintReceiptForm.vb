Imports System.Data
Imports MySql.Data.MySqlClient
Imports System.Drawing.Printing
Imports ZXing
Imports ZXing.Rendering
Imports ZXing.Windows.Compatibility
Public Class PrintReceiptForm


    Private WithEvents printDoc As New System.Drawing.Printing.PrintDocument

    Private DataToPrintList As New List(Of DataGridViewRow)
    Private PrintIndex As Integer = 0
    Private PrintingUserRole As String = String.Empty
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

        If MainForm IsNot Nothing AndAlso MainForm.lbl_currentuser IsNot Nothing Then
            PrintingUserRole = MainForm.lbl_currentuser.Text
        Else
            PrintingUserRole = "System User"
        End If

        If selectedRow.Cells("IsPrinted").Value IsNot DBNull.Value AndAlso selectedRow.Cells("IsPrinted").Value.ToString() = "1" Then
            Dim response As DialogResult = MessageBox.Show($"Transaction No. {transacReceiptID} is already marked as printed. Do you want to print a duplicate receipt?", "Print Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
            If response = DialogResult.No Then
                Exit Sub
            End If
        End If



        Dim defaultPrinterName As String = printDoc.PrinterSettings.PrinterName


        If String.IsNullOrEmpty(defaultPrinterName) OrElse defaultPrinterName.ToLower().Contains("pdf") OrElse defaultPrinterName.ToLower().Contains("xps") OrElse defaultPrinterName.ToLower().Contains("onenote") Then


            MessageBox.Show("No printer is available. Please connect a thermal printer and set it as your Windows default.", "Thermal Printer Not Ready", MessageBoxButtons.OK, MessageBoxIcon.Warning)


            Try
                Using pdlg As New PrintDialog With {.Document = printDoc}
                    If pdlg.ShowDialog() = DialogResult.OK Then
                        printDoc.PrinterSettings = pdlg.PrinterSettings
                        printDoc.Print()
                        UpdatePrintedStatus(transacReceiptID)
                        refreshreceipt()
                        MessageBox.Show($"Receipt for Transaction No. {transacReceiptID} successfully sent to '{printDoc.PrinterSettings.PrinterName}'.", "Print Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
                    End If
                End Using
            Catch ex As Exception
                MessageBox.Show("Error selecting or starting print job: " & ex.Message, "Print Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try

            Exit Sub

        End If


        Try

            printDoc.Print()


            UpdatePrintedStatus(transacReceiptID)

            refreshreceipt()

            MessageBox.Show($"Receipt for Transaction No. {transacReceiptID} successfully sent to '{defaultPrinterName}'.", "Print Success", MessageBoxButtons.OK, MessageBoxIcon.Information)

        Catch ex As System.Drawing.Printing.InvalidPrinterException

            MessageBox.Show($"Warning: The default printer ('{defaultPrinterName}') is not connected or ready. Please check the printer connection (cable/Bluetooth) and power.", "Printer Not Ready", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        Catch ex As Exception
            MessageBox.Show("An unexpected error occurred during the print job: " & ex.Message, "Print Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
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


    Public Function GetBookDetailsByTransaction(ByVal transactionID As String) As DataTable
        Dim con As New MySqlConnection(GlobalVarsModule.connectionString)
        Dim dt As New DataTable

        Dim com As String = "SELECT BookTitle, ISBN, Barcode FROM `borrowing_tbl` WHERE `TransactionReceipt` = @TID"

        Try
            con.Open()
            Using cmd As New MySqlCommand(com, con)
                cmd.Parameters.AddWithValue("@TID", transactionID)
                Dim adap As New MySqlDataAdapter(cmd)
                adap.Fill(dt)
            End Using
        Catch ex As Exception
            MessageBox.Show("Error retrieving book details for printing: " & ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
        End Try

        Return dt
    End Function

    Private Sub PrintDocument_PrintPage(sender As Object, e As Printing.PrintPageEventArgs) Handles printDoc.PrintPage

        Dim g As Graphics = e.Graphics
        Dim fontHeader As New Font("Arial", 11, FontStyle.Bold)
        Dim fontBody As New Font("Arial", 8, FontStyle.Regular)
        Dim fontBodyBold As New Font("Arial", 8, FontStyle.Bold)


        Dim margin As Integer = 10
        Dim yPos As Integer = margin
        Dim lineHeight As Integer = 14


        Dim pageWidth As Integer = e.PageBounds.Width


        Dim contentWidth As Integer = pageWidth - (margin * 2)

        Dim borderPen As New Pen(Color.Black, 1)

        If Me.DataGridView1.SelectedRows.Count = 0 Then
            Exit Sub
        End If

        Dim summaryRow As DataGridViewRow = Me.DataGridView1.SelectedRows(0)
        Dim name As String = summaryRow.Cells("Name").Value?.ToString()
        Dim borrowerType As String = summaryRow.Cells("Borrower").Value?.ToString()
        Dim borrowedDate As String = summaryRow.Cells("BorrowedDate").Value?.ToString()
        Dim dueDate As String = summaryRow.Cells("DueDate").Value?.ToString()
        Dim transacReceipt As String = summaryRow.Cells("TransactionReceipt").Value?.ToString()

        Dim printingUser As String = $"{GlobalRole} ({GlobalUsername})"


        Using sfCenter As New StringFormat With {.Alignment = StringAlignment.Center}
            g.DrawString("Monlimar Development Academy", fontHeader, Brushes.Black, New RectangleF(margin, yPos, contentWidth, lineHeight), sfCenter)

            yPos += lineHeight + 5
            g.DrawString("Library Management System - Borrowing Receipt", fontBody, Brushes.Black, New RectangleF(margin, yPos, contentWidth, lineHeight), sfCenter)
            yPos += lineHeight + 10
        End Using


        g.DrawString($"Transaction Receipt No: {transacReceipt}", fontHeader, Brushes.Black, margin, yPos)
        yPos += lineHeight * 2

        g.DrawString($"Borrower: {name} ({borrowerType})", fontBody, Brushes.Black, margin, yPos)
        yPos += lineHeight
        g.DrawString($"Date Borrowed: {borrowedDate}", fontBody, Brushes.Black, margin, yPos)
        yPos += lineHeight
        g.DrawString($"Due Date: {dueDate}", fontBody, Brushes.Black, margin, yPos)
        yPos += lineHeight + 15


        Dim bookDetailsList As DataTable = GetBookDetailsByTransaction(transacReceipt)

        g.DrawString("Book Details (Total: " & bookDetailsList.Rows.Count.ToString() & "):", fontHeader, Brushes.Black, margin, yPos)
        yPos += lineHeight + 5


        Dim col1Width As Integer = 160
        Dim col2XPos As Integer = margin + col1Width + 20

        g.DrawString("Title", fontBodyBold, Brushes.Black, margin, yPos)
        g.DrawString("ISBN/Barcode", fontBodyBold, Brushes.Black, col2XPos, yPos)


        yPos += 15

        For Each bookRow As DataRow In bookDetailsList.Rows
            Dim bookTitle As String = bookRow("BookTitle").ToString()
            Dim isbn As String = bookRow("ISBN").ToString()
            Dim barcode As String = bookRow("Barcode").ToString()

            Dim codeToPrint As String = If(String.IsNullOrWhiteSpace(barcode), isbn, barcode)


            Dim titleLayout As New RectangleF(margin, yPos, col1Width, lineHeight * 2)
            g.DrawString(bookTitle, fontBody, Brushes.Black, titleLayout)

            Dim sizeF As SizeF = g.MeasureString(bookTitle, fontBody, col1Width)
            Dim titleHeight As Integer = CInt(sizeF.Height)

            g.DrawString(codeToPrint, fontBody, Brushes.Black, col2XPos, yPos)

            yPos += Math.Max(lineHeight, titleHeight) + 5

        Next

        yPos += 15

        g.DrawString($"Printed By: {printingUser}", fontBody, Brushes.Black, margin, yPos)
        yPos += lineHeight


        If Not String.IsNullOrEmpty(transacReceipt) Then

            Dim receiptBarcode As Image = GenerateBarcodeImage(transacReceipt, 180, 35)

            If receiptBarcode IsNot Nothing Then

                Dim xPosBarcode As Integer = margin + ((contentWidth - receiptBarcode.Width) / 2)
                g.DrawImage(receiptBarcode, xPosBarcode, yPos, receiptBarcode.Width, receiptBarcode.Height)
                yPos += receiptBarcode.Height + 5

                Using sfCenter As New StringFormat With {.Alignment = StringAlignment.Center}
                    g.DrawString(transacReceipt, fontBody, Brushes.Black, xPosBarcode + (CSng(receiptBarcode.Width) / 2), yPos, sfCenter)
                End Using

                yPos += lineHeight * 2
                receiptBarcode.Dispose()
            End If
        End If

        g.DrawString("Please return the book on or before the due date to avoid penalty.", fontBody, Brushes.Black, margin, yPos)
        yPos += lineHeight * 2


        Dim sigLineLength As Integer = 200
        g.DrawString("Signature: ", fontBody, Brushes.Black, margin, yPos)
        g.DrawLine(Pens.Black, margin + g.MeasureString("Signature: ", fontBody).Width, yPos + lineHeight - 3, margin + g.MeasureString("Signature: ", fontBody).Width + sigLineLength, yPos + lineHeight - 3)
        yPos += lineHeight



        Dim frameRect As New Rectangle(margin - 5, margin - 5, contentWidth + 10, yPos - margin + 10)
        g.DrawRectangle(borderPen, frameRect)

        e.HasMorePages = False

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