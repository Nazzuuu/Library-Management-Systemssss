﻿Imports System.Data
Imports MySql.Data.MySqlClient
Imports System.Drawing.Printing
Imports ZXing
Imports ZXing.Rendering
Imports ZXing.Windows.Compatibility
Imports System.Drawing.Drawing2D

Public Class PrintReceiptForm

    Private WithEvents printDoc As New System.Drawing.Printing.PrintDocument
    Private DataToPrintList As New List(Of DataGridViewRow)
    Private PrintIndex As Integer = 0
    Private PrintingUserRole As String = String.Empty
    Private Const PRINTER_DPI As Single = 203.0F
    Private Const MAX_PIXEL_WIDTH As Integer = 460


    Private Sub PrintReceipt_Shown(sender As Object, e As EventArgs) Handles MyBase.Shown
        DataGridView1.ClearSelection()
        refreshreceipt()
    End Sub



    Private Sub DataGridView1_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellClick
        If e.RowIndex >= 0 Then
            Dim selectedRow As DataGridViewRow = Me.DataGridView1.Rows(e.RowIndex)
            Dim transactionID As String = selectedRow.Cells("TransactionReceipt").Value.ToString()
            Me.lbltransacno.Text = transactionID

        End If
    End Sub


    Private Sub btnprint_Click(sender As Object, e As EventArgs) Handles btnprint.Click

        If Me.DataGridView1.RowCount = 0 OrElse Me.DataGridView1.SelectedRows.Count = 0 Then
            MessageBox.Show("Please select a borrowing record to print.", "Print Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
            Exit Sub
        End If

        Dim selectedRow As DataGridViewRow = Me.DataGridView1.SelectedRows(0)
        Dim transacReceiptID As String = selectedRow.Cells("TransactionReceipt").Value?.ToString()
        Dim borrowerName As String = selectedRow.Cells("Name").Value?.ToString()
        Dim oldIsPrintedValue As String = selectedRow.Cells("IsPrinted").Value?.ToString()

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

        printDoc.DefaultPageSettings.PaperSize = New PaperSize("Custom Thermal 58mm", 228, 2000)
        printDoc.DefaultPageSettings.Margins = New Margins(5, 5, 5, 5)

        Try

            printDoc.DocumentName = $"Receipt {transacReceiptID}"

            printDoc.Print()

            UpdatePrintedStatus(transacReceiptID)

            GlobalVarsModule.LogAudit(
                actionType:="UPDATE",
                formName:="PRINT RECEIPT",
                description:=$"Printed receipt for transaction {transacReceiptID}.",
                recordID:=transacReceiptID,
                oldValue:=$"IsPrinted={oldIsPrintedValue} (Borrower: {borrowerName})",
                newValue:=$"IsPrinted=1 (Printed by: {GlobalUsername})"
            )
            For Each form In Application.OpenForms
                If TypeOf form Is AuditTrail Then
                    DirectCast(form, AuditTrail).refreshaudit()
                End If
            Next

            refreshreceipt()
            MessageBox.Show($"Receipt for Transaction No. {transacReceiptID} successfully sent to '{printDoc.PrinterSettings.PrinterName}'.", "Print Success", MessageBoxButtons.OK, MessageBoxIcon.Information)

        Catch ex As System.Drawing.Printing.InvalidPrinterException

            MessageBox.Show($"Warning: The default printer ('{printDoc.PrinterSettings.PrinterName}') is not connected or ready. Please choose your Thermal Printer.", "Printer Not Ready", MessageBoxButtons.OK, MessageBoxIcon.Warning)

            Using pdlg As New PrintDialog With {.Document = printDoc}
                If pdlg.ShowDialog() = DialogResult.OK Then
                    printDoc.PrinterSettings = pdlg.PrinterSettings
                    printDoc.Print()

                    UpdatePrintedStatus(transacReceiptID)

                    GlobalVarsModule.LogAudit(
                        actionType:="UPDATE",
                        formName:="PRINT RECEIPT",
                        description:=$"Printed receipt for transaction {transacReceiptID} using fallback printer.",
                        recordID:=transacReceiptID,
                        oldValue:=$"IsPrinted={oldIsPrintedValue} (Borrower: {borrowerName})",
                        newValue:=$"IsPrinted=1 (Printed by: {GlobalUsername})"
                    )
                    For Each form In Application.OpenForms
                        If TypeOf form Is AuditTrail Then
                            DirectCast(form, AuditTrail).refreshaudit()
                        End If
                    Next

                    refreshreceipt()
                    MessageBox.Show($"Receipt for Transaction No. {transacReceiptID} successfully sent to '{printDoc.PrinterSettings.PrinterName}'.", "Print Success (Fallback)", MessageBoxButtons.OK, MessageBoxIcon.Information)
                End If
            End Using

        Catch ex As Exception

            MessageBox.Show("An unexpected error occurred during the print job. You may need to choose a different printer.", "Print Error", MessageBoxButtons.OK, MessageBoxIcon.Error)


            Using pdlg As New PrintDialog With {.Document = printDoc}
                If pdlg.ShowDialog() = DialogResult.OK Then
                    printDoc.PrinterSettings = pdlg.PrinterSettings
                    printDoc.Print()

                    UpdatePrintedStatus(transacReceiptID)

                    GlobalVarsModule.LogAudit(
                        actionType:="UPDATE",
                        formName:="PRINT RECEIPT",
                        description:=$"Printed receipt for transaction {transacReceiptID} after print error.",
                        recordID:=transacReceiptID,
                        oldValue:=$"IsPrinted={oldIsPrintedValue} (Borrower: {borrowerName})",
                        newValue:=$"IsPrinted=1 (Printed by: {GlobalUsername})"
                    )
                    For Each form In Application.OpenForms
                        If TypeOf form Is AuditTrail Then
                            DirectCast(form, AuditTrail).refreshaudit()
                        End If
                    Next

                    refreshreceipt()
                    MessageBox.Show($"Receipt for Transaction No. {transacReceiptID} successfully sent to '{printDoc.PrinterSettings.PrinterName}'.", "Print Success (Fallback)", MessageBoxButtons.OK, MessageBoxIcon.Information)
                End If
            End Using
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

    Public Function GetBookDetailsByTransaction(ByVal transactionID As String) As DataTable
        Dim con As New MySqlConnection(GlobalVarsModule.connectionString)
        Dim dt As New DataTable

        Dim com As String = "SELECT BookTitle, ISBN, Barcode, AccessionID FROM `borrowinghistory_tbl` WHERE `TransactionReceipt` = @TID AND `Status` = 'Granted'"

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
        g.SmoothingMode = Drawing2D.SmoothingMode.AntiAlias
        g.TextRenderingHint = Drawing.Text.TextRenderingHint.ClearTypeGridFit

        Dim margin As Integer = 5
        Dim yPos As Integer = margin
        Dim lineHeight As Integer = 9

        Dim pageWidth As Integer = e.PageSettings.PrintableArea.Width
        Dim contentWidth As Integer = pageWidth - (margin * 2)

        Dim fontHeader As New Font("Arial", 7, FontStyle.Bold)
        Dim fontBody As New Font("Arial", 6.5, FontStyle.Regular)
        Dim fontBodyBold As New Font("Arial", 6.5, FontStyle.Bold)

        If Me.DataGridView1.SelectedRows.Count = 0 Then Exit Sub

        Dim summaryRow As DataGridViewRow = Me.DataGridView1.SelectedRows(0)
        Dim name As String = summaryRow.Cells("Name").Value?.ToString()
        Dim borrowerType As String = summaryRow.Cells("Borrower").Value?.ToString()
        Dim borrowedDate As String = CDate(summaryRow.Cells("BorrowedDate").Value).ToShortDateString()
        Dim dueDate As String = CDate(summaryRow.Cells("DueDate").Value).ToShortDateString()
        Dim transacReceipt As String = summaryRow.Cells("TransactionReceipt").Value?.ToString()
        If String.IsNullOrEmpty(transacReceipt) Then transacReceipt = Me.lbltransacno.Text


        Using sfCenter As New StringFormat With {.Alignment = StringAlignment.Center}
            g.DrawString("Monlimar Development Academy", fontHeader, Brushes.Black, New RectangleF(margin, yPos, contentWidth, lineHeight), sfCenter)
            yPos += lineHeight + 2
            g.DrawString("Library Management System", fontBody, Brushes.Black, New RectangleF(margin, yPos, contentWidth, lineHeight), sfCenter)
            yPos += lineHeight + 8
        End Using


        g.DrawString("Transaction Receipt No: " & transacReceipt, fontBody, Brushes.Black, margin, yPos) : yPos += lineHeight
        g.DrawString("Name: " & name, fontBody, Brushes.Black, margin, yPos) : yPos += lineHeight
        g.DrawString("Borrower Type: " & borrowerType, fontBody, Brushes.Black, margin, yPos) : yPos += lineHeight
        g.DrawString("Date Borrowed: " & borrowedDate, fontBody, Brushes.Black, margin, yPos) : yPos += lineHeight
        g.DrawString("Due Date: " & dueDate, fontBody, Brushes.Black, margin, yPos) : yPos += lineHeight + 6


        Dim bookDetailsList As DataTable = GetBookDetailsByTransaction(transacReceipt)
        g.DrawString("Book Details (Total: " & bookDetailsList.Rows.Count & ")", fontBodyBold, Brushes.Black, margin, yPos)
        yPos += lineHeight + 3

        For Each bookRow As DataRow In bookDetailsList.Rows

            Dim titleText As String = "Title: " & bookRow("BookTitle").ToString()
            Dim titleRect As New RectangleF(margin, yPos, contentWidth, lineHeight * 3)
            g.DrawString(titleText, fontBody, Brushes.Black, titleRect)
            yPos += CInt(g.MeasureString(titleText, fontBody, contentWidth).Height)


            Dim accessionID As String = "Accession ID: " & bookRow("AccessionID").ToString()
            g.DrawString(accessionID, fontBody, Brushes.Black, margin, yPos)
            yPos += lineHeight

            Dim codeText As String = "Barcode/ISBN: " & If(String.IsNullOrWhiteSpace(bookRow("Barcode").ToString()), bookRow("ISBN").ToString(), bookRow("Barcode").ToString())
            Dim codeRect As New RectangleF(margin, yPos, contentWidth, lineHeight * 2)
            g.DrawString(codeText, fontBody, Brushes.Black, codeRect)
            yPos += CInt(g.MeasureString(codeText, fontBody, contentWidth).Height) + 5
        Next


        If Not String.IsNullOrEmpty(transacReceipt) Then
            Dim barcodeWidth As Integer = 140
            Dim barcodeHeight As Integer = 25

            Using receiptBarcode As Image = GenerateBarcodeImage(transacReceipt, barcodeWidth, barcodeHeight)
                If receiptBarcode IsNot Nothing Then
                    Dim xPosBarcode As Integer = margin + ((contentWidth - barcodeWidth) / 2)
                    g.DrawImage(receiptBarcode, xPosBarcode, yPos, barcodeWidth, barcodeHeight)
                    yPos += barcodeHeight + 2

                    Using sfCenter As New StringFormat With {.Alignment = StringAlignment.Center}
                        g.DrawString(transacReceipt, fontBody, Brushes.Black, New RectangleF(margin, yPos, contentWidth, lineHeight), sfCenter)
                    End Using
                    yPos += lineHeight + 2
                End If
            End Using
        End If

        e.HasMorePages = False
    End Sub


    Public Function GenerateBarcodeImage(ByVal barcodeText As String, ByVal targetWidth As Integer, ByVal targetHeight As Integer) As Image
        If String.IsNullOrWhiteSpace(barcodeText) Then Return Nothing
        Try
            Dim options As New ZXing.Common.EncodingOptions With {
.Width = targetWidth,
.Height = targetHeight,
.Margin = 1,
.PureBarcode = True
}

            Dim writer As New BarcodeWriter(Of Bitmap) With {
        .Format = BarcodeFormat.CODE_128,
        .Options = options,
        .Renderer = New BitmapRenderer()
    }

            Dim barcodeBitmap As Bitmap = writer.Write(barcodeText)
            barcodeBitmap.SetResolution(96, 96)
            Return barcodeBitmap

        Catch ex As Exception
            Dim bmp As New Bitmap(targetWidth, targetHeight)
            Using g As Graphics = Graphics.FromImage(bmp)
                g.Clear(Color.White)
                Using font As New Font("Arial", 6)
                    g.DrawString("Barcode Error", font, Brushes.Red, 2, 2)
                End Using
            End Using
            Return bmp
        End Try


    End Function

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