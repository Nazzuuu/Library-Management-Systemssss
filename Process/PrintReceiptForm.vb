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

    ' ******************************************************
    ' * PAGBABAGO 1: Constant para sa Printer DPI.
    ' * Gagamitin ito para maging tama ang sukat ng barcode.
    ' ******************************************************
    Private Const PRINTER_DPI As Integer = 203

    Private Sub PrintReceipt_Shown(sender As Object, e As EventArgs) Handles MyBase.Shown
        DataGridView1.ClearSelection()
        refreshreceipt()
    End Sub

    ' Ang lumang GenerateBarcode function ay inalis na.

    Private Sub DataGridView1_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellClick
        If e.RowIndex >= 0 Then
            Dim selectedRow As DataGridViewRow = Me.DataGridView1.Rows(e.RowIndex)
            Dim transactionID As String = selectedRow.Cells("TransactionReceipt").Value.ToString()
            Me.lbltransacno.Text = transactionID

            ' Walang pagbabago: Inalis na ang pag-set ng picbarcode.Image
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

        ' Ang 58mm Thermal paper width ay ~57mm, na katumbas ng 2.24 inches.
        ' Sa 203 DPI, ang maximum pixel width ay: 2.24 * 203 = ~455 pixels.
        ' Ang 228 na value ay katumbas ng 60mm thermal paper sa ilang printer, pero ipinagpapatuloy natin ang value na ito.
        printDoc.DefaultPageSettings.PaperSize = New PaperSize("Custom Thermal 58mm", 228, 2000)
        printDoc.DefaultPageSettings.Margins = New Margins(5, 5, 5, 5)

        Try
            ' Tiyakin na ang DocumentName ay angkop
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
            ' ... (Walang pagbabago sa error handling)
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
            ' ... (Walang pagbabago sa error handling)
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

    Public Function GenerateBarcodeImage(ByVal barcodeText As String, ByVal width As Integer, ByVal height As Integer) As Image

        If String.IsNullOrWhiteSpace(barcodeText) OrElse barcodeText = "0000000000000" Then
            Return Nothing
        End If

        Try
            ' ******************************************************
            ' * PAGBABAGO 2: Ginamit ang DPI ng printer.
            ' * Ito ay mas magandang basehan kaysa sa pixel width ng PictureBox.
            ' * Ang width/height na pinasa ay galing sa PrintPageEventArgs.
            ' * Ang target width/height ng barcode ay kinoconvert sa DPI.
            ' ******************************************************

            ' Convert width/height (in 100ths of an inch or pixels) to actual pixels based on DPI
            ' Dahil ang ZXing ay nangangailangan ng pixel size, ginagamit na lang natin ang pinasa.
            ' Kung ang width/height ay naka 100ths of an inch, iko-convert natin.
            ' Pero sa kaso na ito, mananatili tayo sa logic na gagamitin ang pinasang width/height.

            ' Ito ang sukat ng barcode image na gusto natin sa print.
            Dim renderWidth As Integer = CInt(width / 0.75) ' Scaling factor para sa mas malaki, para hindi blurry
            Dim renderHeight As Integer = CInt(height / 0.75)

            If renderWidth > 400 Then renderWidth = 400 ' Limitahan ang maximum width para magkasya sa 58mm
            If renderHeight > 60 Then renderHeight = 60 ' Limitahan ang maximum height

            Dim options As New ZXing.Common.EncodingOptions With {
                .Width = renderWidth,
                .Height = renderHeight,
                .Margin = 5, ' Maliit na margin para sa thermal
                .PureBarcode = False ' Dapat False para masiguradong may quiet zone
            }

            Dim writer As New BarcodeWriter(Of Bitmap) With {
                .Format = BarcodeFormat.CODE_128,
                .Options = options,
                .Renderer = New BitmapRenderer()
            }

            ' Gumawa ng raw barcode image
            Dim barcodeBitmap As Bitmap = writer.Write(barcodeText)

            ' ******************************************************
            ' * PAGBABAGO 3: I-set ang DPI ng nalikhang Bitmap
            ' * Para masigurong tama ang sukat sa pagpi-print.
            ' ******************************************************
            barcodeBitmap.SetResolution(PRINTER_DPI, PRINTER_DPI)

            ' I-return ang image sa target size
            Return New Bitmap(barcodeBitmap, New Size(width, height))

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

        ' ******************************************************
        ' * PAGBABAGO 4: I-set ang Graphics resolution sa DPI ng Printer
        ' * Ito ang pinakamahalaga para sa malinaw na barcode/image printing.
        ' ******************************************************
        g.PageUnit = GraphicsUnit.Pixel ' O kaya GraphicsUnit.Display (default)
        g.PageScale = g.DpiX / PRINTER_DPI ' I-adjust ang scaling. (Hindi na kailangan kung naka-Pixel ang PageUnit)
        g.SmoothingMode = Drawing2D.SmoothingMode.None ' Para sa pixelated/non-anti-aliased output (thermal printer style)
        g.InterpolationMode = Drawing2D.InterpolationMode.NearestNeighbor
        g.PixelOffsetMode = Drawing2D.PixelOffsetMode.HighQuality

        ' I-set muli ang yPos at margin batay sa PageSettings
        Dim margin As Integer = 5
        Dim yPos As Integer = margin
        Dim lineHeight As Integer = 8

        Dim pageWidth As Integer = e.PageSettings.PrintableArea.Width
        Dim contentWidth As Integer = pageWidth - (margin * 2)

        Dim fontHeader As New Font("Arial", 8, FontStyle.Bold)
        Dim fontBody As New Font("Arial", 6, FontStyle.Regular)
        Dim fontBodyBold As New Font("Arial", 6, FontStyle.Bold)

        ' (Ipagpapatuloy ang logic ng fonts at sizing)
        If pageWidth > 320 OrElse e.PageSettings.PrinterSettings.PrinterName.ToLower().Contains("pdf") Then
            fontHeader = New Font("Arial", 11, FontStyle.Bold)
            fontBody = New Font("Arial", 8, FontStyle.Regular)
            fontBodyBold = New Font("Arial", 8, FontStyle.Bold)
            lineHeight = 14
            margin = 15
            pageWidth = e.PageBounds.Width
        End If

        If Me.DataGridView1.SelectedRows.Count = 0 Then
            Exit Sub
        End If

        Dim summaryRow As DataGridViewRow = Me.DataGridView1.SelectedRows(0)
        Dim name As String = summaryRow.Cells("Name").Value?.ToString()
        Dim borrowerType As String = summaryRow.Cells("Borrower").Value?.ToString()
        Dim borrowedDate As String = CDate(summaryRow.Cells("BorrowedDate").Value).ToShortDateString()
        Dim dueDate As String = CDate(summaryRow.Cells("DueDate").Value).ToShortDateString()
        Dim transacReceipt As String = summaryRow.Cells("TransactionReceipt").Value?.ToString()

        If String.IsNullOrEmpty(transacReceipt) Then
            transacReceipt = Me.lbltransacno.Text
        End If

        Dim printingUser As String = $"{GlobalRole} ({GlobalUsername})"


        Using sfCenter As New StringFormat With {.Alignment = StringAlignment.Center}
            ' ... (Drawing ng Header)
            g.DrawString("Monlimar Development Academy", fontHeader, Brushes.Black, New RectangleF(margin, yPos, contentWidth, lineHeight), sfCenter)
            yPos += lineHeight + 3
            g.DrawString("Library Management System - Borrowing Receipt", fontBody, Brushes.Black, New RectangleF(margin, yPos, contentWidth, lineHeight), sfCenter)
            yPos += lineHeight + 8
        End Using

        ' ... (Drawing ng Transaction Details)
        g.DrawString($"Transaction Receipt No: {transacReceipt}", fontHeader, Brushes.Black, margin, yPos)
        yPos += lineHeight * 2

        g.DrawString($"Borrower: {name} ({borrowerType})", fontBody, Brushes.Black, margin, yPos)
        yPos += lineHeight
        g.DrawString($"Date Borrowed: {borrowedDate}", fontBody, Brushes.Black, margin, yPos)
        yPos += lineHeight
        g.DrawString($"Due Date: {dueDate}", fontBody, Brushes.Black, margin, yPos)
        yPos += lineHeight + 10


        Dim bookDetailsList As DataTable = GetBookDetailsByTransaction(transacReceipt)

        g.DrawString("Book Details (Total: " & bookDetailsList.Rows.Count.ToString() & "):", fontHeader, Brushes.Black, margin, yPos)
        yPos += lineHeight + 3

        ' ... (Drawing ng Table Header)
        Dim col1Width As Integer = CInt(contentWidth * 0.4)
        Dim col2Width As Integer = CInt(contentWidth * 0.3)
        Dim col3Width As Integer = CInt(contentWidth * 0.3)

        Dim colTitleX As Integer = margin
        Dim colAccessionX As Integer = colTitleX + col1Width
        Dim colCodeX As Integer = colAccessionX + col2Width


        g.DrawString("Title", fontBodyBold, Brushes.Black, colTitleX, yPos)
        g.DrawString("Accession", fontBodyBold, Brushes.Black, colAccessionX, yPos)
        g.DrawString("ISBN/BARCODE", fontBodyBold, Brushes.Black, colCodeX, yPos)


        yPos += 12

        ' ... (Drawing ng Book Details Loop)
        For Each bookRow As DataRow In bookDetailsList.Rows
            Dim bookTitle As String = bookRow("BookTitle").ToString()
            Dim accessionID As String = bookRow("AccessionID").ToString()
            Dim isbn As String = bookRow("ISBN").ToString()
            Dim barcode As String = bookRow("Barcode").ToString()

            Dim codeToPrint As String = If(String.IsNullOrWhiteSpace(barcode), isbn, barcode)

            Dim titleLayout As New RectangleF(colTitleX, yPos, col1Width - 5, lineHeight * 3)
            g.DrawString(bookTitle, fontBody, Brushes.Black, titleLayout)

            Dim sizeF As SizeF = g.MeasureString(bookTitle, fontBody, col1Width - 5)
            Dim titleHeight As Integer = CInt(sizeF.Height)

            g.DrawString(accessionID, fontBody, Brushes.Black, colAccessionX, yPos)
            g.DrawString(codeToPrint, fontBody, Brushes.Black, colCodeX, yPos)

            yPos += Math.Max(lineHeight, titleHeight) + 3
        Next

        yPos += 10
        g.DrawLine(Pens.Black, margin, yPos, pageWidth - margin, yPos)

        yPos += 10

        ' ... (Drawing ng Printed By)
        g.DrawString($"Printed By: {printingUser}", fontBody, Brushes.Black, margin, yPos)
        yPos += lineHeight + 5

        ' ******************************************************
        ' * PAGBABAGO 5: Barcode Drawing Logic
        ' * Ginamit ang updated na GenerateBarcodeImage at tamang Disposal.
        ' ******************************************************
        If Not String.IsNullOrEmpty(transacReceipt) Then
            Dim barcodeWidth As Integer = 180 ' Desired width sa 100ths of an inch
            Dim barcodeHeight As Integer = 35 ' Desired height sa 100ths of an inch

            If pageWidth > 320 Then
                barcodeWidth = 220
                barcodeHeight = 45
            End If

            Using receiptBarcode As Image = GenerateBarcodeImage(transacReceipt, barcodeWidth, barcodeHeight)

                If receiptBarcode IsNot Nothing Then
                    ' I-center ang barcode
                    Dim xPosBarcode As Integer = margin + ((contentWidth - receiptBarcode.Width) / 2)

                    ' I-drawing ang barcode image
                    g.DrawImage(receiptBarcode, xPosBarcode, yPos, receiptBarcode.Width, receiptBarcode.Height)
                    yPos += receiptBarcode.Height + 2

                    ' I-drawing ang barcode text sa ibaba (Manual control)
                    Using sfCenter As New StringFormat With {.Alignment = StringAlignment.Center}
                        g.DrawString(transacReceipt, fontBody, Brushes.Black, xPosBarcode + (CSng(receiptBarcode.Width) / 2), yPos, sfCenter)
                    End Using

                    yPos += lineHeight + 5
                End If
            End Using ' Sigurado na na-dispose ang image
        End If

        ' ... (Drawing ng Footer)
        g.DrawString("Please return the book on or before the due date to avoid penalty.", fontBody, Brushes.Black, margin, yPos)
        yPos += lineHeight * 2

        g.DrawString("Signature: __________________________", fontBody, Brushes.Black, margin, yPos)
        yPos += lineHeight + 10

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

                ' Walang pagbabago: Inalis na ang pagtawag sa GenerateBarcode
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