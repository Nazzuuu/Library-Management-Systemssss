Imports System.Data
Imports MySql.Data.MySqlClient
Imports System.Drawing.Printing
Imports ZXing
Imports ZXing.Rendering
Imports ZXing.Windows.Compatibility
Imports System.Drawing.Drawing2D
Imports System.IO
Imports System.Runtime.InteropServices
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
            Dim response As DialogResult = MessageBox.Show(
            $"Transaction No. {transacReceiptID} is already marked as printed. Do you want to print a duplicate receipt?",
            "Print Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question)

            If response = DialogResult.No Then Exit Sub
        End If

        Try
            Dim printerName As String = printDoc.PrinterSettings.PrinterName
            Dim bytesToPrint As Byte() = BuildEscPosReceiptBytes(transacReceiptID)
            Dim result As Boolean = RawPrinterHelper.SendBytesToPrinter(printerName, bytesToPrint)

            If Not result Then
                MessageBox.Show($"Failed to send raw ESC/POS data to '{printerName}'. Please verify printer selection.", "Print Error", MessageBoxButtons.OK, MessageBoxIcon.Error)

                Using pdlg As New PrintDialog With {.Document = printDoc}
                    If pdlg.ShowDialog() = DialogResult.OK Then
                        printDoc.PrinterSettings = pdlg.PrinterSettings
                        bytesToPrint = BuildEscPosReceiptBytes(transacReceiptID)
                        If RawPrinterHelper.SendBytesToPrinter(printDoc.PrinterSettings.PrinterName, bytesToPrint) Then
                            UpdatePrintedStatus(transacReceiptID)
                        Else
                            MessageBox.Show("Printing failed with selected printer.", "Print Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                            Exit Sub
                        End If
                    Else
                        Exit Sub
                    End If
                End Using
            Else
                UpdatePrintedStatus(transacReceiptID)
            End If

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

            For Each row As DataGridViewRow In DataGridView1.Rows
                If row.Cells("IsPrinted").Value?.ToString() = "1" Then
                    row.DefaultCellStyle.BackColor = Color.LightGray
                    row.DefaultCellStyle.ForeColor = Color.Black
                End If
            Next


            MessageBox.Show($"Receipt for Transaction No. {transacReceiptID} successfully sent to '{printDoc.PrinterSettings.PrinterName}'.",
                        "Print Success", MessageBoxButtons.OK, MessageBoxIcon.Information)

        Catch ex As Exception
            MessageBox.Show("An unexpected error occurred during the print job. Please try a different printer. " & ex.Message,
                        "Print Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Function BuildEscPosReceiptBytes(transactionID As String) As Byte()
        Dim esc As New List(Of Byte)
        Dim enc As System.Text.Encoding = System.Text.Encoding.ASCII

        Dim SubAppendText = Sub(text As String)
                                esc.AddRange(enc.GetBytes(text & vbLf))
                            End Sub

        esc.Add(&H1B) : esc.Add(&H40)
        esc.AddRange(enc.GetBytes(vbLf))
        esc.Add(&H1B) : esc.Add(&H61) : esc.Add(&H1)
        esc.Add(&H1B) : esc.Add(&H45) : esc.Add(&H1)

        SubAppendText("Library Management System")

        'MDA-LMS'


        esc.Add(&H1B) : esc.Add(&H45) : esc.Add(&H0)
        SubAppendText("---- Borrowing Receipt ----")
        esc.AddRange(enc.GetBytes(vbLf))


        esc.Add(&H1B) : esc.Add(&H61) : esc.Add(&H0)

        SubAppendText("Transaction Receipt No:")

        esc.Add(&H1D) : esc.Add(&H21) : esc.Add(&H11)
        SubAppendText(transactionID)

        esc.Add(&H1D) : esc.Add(&H21) : esc.Add(&H0)

        Dim summaryRow As DataGridViewRow = Nothing
        For Each row As DataGridViewRow In DataGridView1.SelectedRows
            summaryRow = row
            Exit For
        Next

        Dim name As String = If(summaryRow IsNot Nothing, summaryRow.Cells("Name").Value?.ToString(), "")
        Dim borrowerType As String = If(summaryRow IsNot Nothing, summaryRow.Cells("Borrower").Value?.ToString(), "")
        Dim borrowedDate As String = ""
        Dim dueDate As String = ""
        Try
            If summaryRow IsNot Nothing AndAlso summaryRow.Cells("BorrowedDate").Value IsNot DBNull.Value Then borrowedDate = CDate(summaryRow.Cells("BorrowedDate").Value).ToShortDateString()
            If summaryRow IsNot Nothing AndAlso summaryRow.Cells("DueDate").Value IsNot DBNull.Value Then dueDate = CDate(summaryRow.Cells("DueDate").Value).ToShortDateString()
        Catch
        End Try

        SubAppendText($"Borrower: {name} ({borrowerType})")
        SubAppendText($"Date Borrowed: {borrowedDate}")
        SubAppendText($"Due Date: {dueDate}")
        esc.AddRange(enc.GetBytes(vbLf))


        Dim bookDetails As DataTable = GetBookDetailsByTransaction(transactionID)
        Dim totalBooks As Integer = If(bookDetails IsNot Nothing, bookDetails.Rows.Count, 0)
        SubAppendText($"Book Details (Total: {totalBooks}):")
        esc.AddRange(enc.GetBytes(vbLf))

        Dim maxCharsPerLine As Integer = 32

        For Each dr As DataRow In bookDetails.Rows
            Dim bookTitle As String = dr("BookTitle").ToString()
            Dim accessionID As String = dr("AccessionID").ToString()
            Dim isbn As String = dr("ISBN").ToString()
            Dim barcode As String = dr("Barcode").ToString()
            Dim codeToPrint As String = If(String.IsNullOrWhiteSpace(barcode), isbn, barcode)
            Dim titleLine As String = $"Title: {bookTitle}"
            For Each line As String In WrapText(titleLine, maxCharsPerLine)
                SubAppendText(line)
            Next

            SubAppendText($"AccessionID: {accessionID}")
            SubAppendText($"Barcode/ISBN: {codeToPrint}")
            esc.AddRange(enc.GetBytes(vbLf))
        Next


        If Not String.IsNullOrEmpty(transactionID) Then

            esc.Add(&H1B) : esc.Add(&H61) : esc.Add(&H1)
            esc.Add(&H1D) : esc.Add(&H68) : esc.Add(80)
            esc.Add(&H1D) : esc.Add(&H77) : esc.Add(3)
            esc.Add(&H1D) : esc.Add(&H48) : esc.Add(2)
            esc.Add(&H1D) : esc.Add(&H6B) : esc.Add(73)

            Dim barcodeBytes As Byte() = enc.GetBytes(transactionID)
            Dim blen As Integer = barcodeBytes.Length

            If blen > 255 Then blen = 255
            esc.Add(CByte(blen))
            esc.AddRange(barcodeBytes)
            esc.AddRange(enc.GetBytes(vbLf))

        End If


        esc.Add(&H1B) : esc.Add(&H64) : esc.Add(4)
        esc.Add(&H1D) : esc.Add(&H56) : esc.Add(&H1)

        Return esc.ToArray()

    End Function

    Private Iterator Function WrapText(text As String, maxChars As Integer) As IEnumerable(Of String)
        If String.IsNullOrEmpty(text) Then
            Yield ""
            Return
        End If

        Dim parts As String() = text.Split(" "c)
        Dim line As String = ""
        For Each word In parts
            If line.Length = 0 Then
                line = word
            ElseIf line.Length + 1 + word.Length <= maxChars Then
                line = line & " " & word
            Else
                Yield line
                line = word
            End If
        Next
        If line.Length > 0 Then Yield line
    End Function

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
            If con.State = ConnectionState.Open Then con.Close()
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
            If con.State = ConnectionState.Open Then con.Close()
        End Try
        Return dt
    End Function

    Private printedAlready As Boolean = False

    Public Sub PrintBarcodeESC_POS(ByVal barcodeText As String)

        Try
            Dim printerName As String = printDoc.PrinterSettings.PrinterName
            Dim escpos As New List(Of Byte)


            escpos.Add(&H1B)
            escpos.Add(&H40)


            escpos.Add(&H1B)
            escpos.Add(&H61)
            escpos.Add(&H1)


            escpos.Add(&H1D)
            escpos.Add(&H68)
            escpos.Add(80)


            escpos.Add(&H1D)
            escpos.Add(&H77)
            escpos.Add(3)


            escpos.Add(&H1D)
            escpos.Add(&H48)
            escpos.Add(2)


            escpos.Add(&H1D)
            escpos.Add(&H6B)
            escpos.Add(73)

            Dim barcodeBytes As Byte() = System.Text.Encoding.ASCII.GetBytes(barcodeText)
            escpos.Add(CByte(barcodeBytes.Length))
            escpos.AddRange(barcodeBytes)


            escpos.Add(&H1B)
            escpos.Add(&H64)
            escpos.Add(6)

            RawPrinterHelper.SendBytesToPrinter(printerName, escpos.ToArray())

        Catch ex As Exception
            MessageBox.Show("ESC/POS Barcode Print Error: " & ex.Message)
        End Try

    End Sub


    Private Sub PrintDocument_PrintPage(sender As Object, e As Printing.PrintPageEventArgs) Handles printDoc.PrintPage
        If printedAlready Then
            e.HasMorePages = False
            Exit Sub
        End If
        printedAlready = True

        Dim g As Graphics = e.Graphics
        g.SmoothingMode = SmoothingMode.AntiAlias
        g.TextRenderingHint = Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit

        Dim margin As Integer = 5
        Dim yPos As Integer = margin

        Dim fontHeader As New Font("Arial", 10.0F, FontStyle.Bold)
        Dim fontSubHeader As New Font("Arial", 9.0F, FontStyle.Regular)
        Dim fontBody As New Font("Arial", 8.0F, FontStyle.Regular)
        Dim fontBodyBold As New Font("Arial", 8.0F, FontStyle.Bold)
        Dim pageWidth As Integer = e.PageSettings.PrintableArea.Width
        Dim contentWidth As Integer = pageWidth - (margin * 2)
        Dim lineHeight As Integer = CInt(Math.Ceiling(fontBody.GetHeight(g)))
        Dim bodyLineSpacing As Integer = CInt(Math.Ceiling(lineHeight * 0.2F))

        If Me.DataGridView1.SelectedRows.Count = 0 Then Exit Sub

        Dim summaryRow As DataGridViewRow = Me.DataGridView1.SelectedRows(0)
        Dim name As String = summaryRow.Cells("Name").Value?.ToString()
        Dim borrowerType As String = summaryRow.Cells("Borrower").Value?.ToString()
        Dim borrowedDate As String = CDate(summaryRow.Cells("BorrowedDate").Value).ToShortDateString()
        Dim dueDate As String = CDate(summaryRow.Cells("DueDate").Value).ToShortDateString()
        Dim transacReceipt As String = summaryRow.Cells("TransactionReceipt").Value?.ToString()
        If String.IsNullOrEmpty(transacReceipt) Then transacReceipt = Me.lbltransacno.Text

        Using sfCenter As New StringFormat With {.Alignment = StringAlignment.Center}
            g.DrawString("", fontHeader, Brushes.Black, New RectangleF(margin - 8, yPos, contentWidth + 16, lineHeight * 2), sfCenter)
            yPos += CInt(lineHeight * 1.8)
            g.DrawString("Library Management System", fontSubHeader, Brushes.Black, New RectangleF(margin, yPos, contentWidth, lineHeight * 2), sfCenter)
            yPos += lineHeight
            g.DrawString("---- Borrowing Receipt ----", fontSubHeader, Brushes.Black, New RectangleF(margin, yPos, contentWidth, lineHeight * 2), sfCenter)
            yPos += CInt(lineHeight * 2.5)
        End Using

        g.DrawString("Transaction Receipt No:", fontBodyBold, Brushes.Black, margin, yPos)
        yPos += lineHeight + bodyLineSpacing
        g.DrawString(transacReceipt, fontBodyBold, Brushes.Black, margin + 2, yPos)
        yPos += lineHeight + bodyLineSpacing

        g.DrawString($"Borrower: {name} ({borrowerType})", fontBody, Brushes.Black, margin, yPos)
        yPos += lineHeight + bodyLineSpacing
        g.DrawString($"Date Borrowed: {borrowedDate}", fontBody, Brushes.Black, margin + 2, yPos)
        yPos += lineHeight + bodyLineSpacing
        g.DrawString($"Due Date: {dueDate}", fontBody, Brushes.Black, margin + 2, yPos)
        yPos += lineHeight + (bodyLineSpacing * 2)

        Dim bookDetailsList As DataTable = GetBookDetailsByTransaction(transacReceipt)
        Dim totalBooks As Integer = bookDetailsList.Rows.Count

        Dim sfLeft As New StringFormat()
        sfLeft.FormatFlags = StringFormatFlags.LineLimit
        sfLeft.Trimming = StringTrimming.Word

        Dim booksHeader As String = $"Book Details (Total: {totalBooks}):"
        Dim booksHeaderSize As SizeF = g.MeasureString(booksHeader, fontBodyBold, contentWidth)
        Dim booksHeaderHeight As Integer = CInt(Math.Ceiling(booksHeaderSize.Height))
        g.DrawString(booksHeader, fontBodyBold, Brushes.Black, New RectangleF(margin, yPos, contentWidth, booksHeaderHeight), sfLeft)
        yPos += booksHeaderHeight + bodyLineSpacing

        For Each bookRow As DataRow In bookDetailsList.Rows
            Dim bookTitle As String = bookRow("BookTitle").ToString()
            Dim accessionID As String = bookRow("AccessionID").ToString()
            Dim isbn As String = bookRow("ISBN").ToString()
            Dim barcode As String = bookRow("Barcode").ToString()
            Dim codeToPrint As String = If(String.IsNullOrWhiteSpace(barcode), isbn, barcode)

            Dim titleTextLine As String = $"Title: {bookTitle}"
            Dim titleWidth As Integer = Math.Max(20, contentWidth - 20)
            Dim titleSize As SizeF = g.MeasureString(titleTextLine, fontBody, titleWidth)
            Dim titleHeight As Integer = CInt(Math.Ceiling(titleSize.Height))


            g.DrawString(titleTextLine, fontBody, Brushes.Black, New RectangleF(margin + 10, yPos, titleWidth, titleHeight), sfLeft)
            yPos += titleHeight + bodyLineSpacing


            g.DrawString($"AccessionID: {accessionID}", fontBody, Brushes.Black, margin + 10, yPos)
            yPos += lineHeight + bodyLineSpacing
            g.DrawString($"Barcode/ISBN: {codeToPrint}", fontBody, Brushes.Black, margin + 10, yPos)
            yPos += lineHeight + (bodyLineSpacing * 2)
        Next

        yPos -= 5

        If Not String.IsNullOrEmpty(transacReceipt) Then
            System.Threading.Thread.Sleep(150)
            PrintBarcodeESC_POS(transacReceipt)
        End If

        yPos += 20
        g.DrawString(" ", fontBody, Brushes.Black, margin, yPos)

        e.HasMorePages = False
    End Sub


    Private Sub printDoc_EndPrint(sender As Object, e As Printing.PrintEventArgs) Handles printDoc.EndPrint
        printedAlready = False
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
                If dt.Rows.Count > 0 Then Me.lbltransacno.Text = transactionID
            End Using
        Catch ex As Exception
            MessageBox.Show("Error loading print receipt data: " & ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            If con.State = ConnectionState.Open Then con.Close()
        End Try
    End Sub

    Private Sub PrintReceiptForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        refreshreceipt()
        GlobalVarsModule.AutoRefreshGrid(DataGridView1, "SELECT * FROM `printreceipt_tbl`", 2000)
        AddHandler GlobalVarsModule.DatabaseUpdated, AddressOf OnDatabaseUpdated
    End Sub

    Public Sub refreshreceipt()
        Dim con As New MySqlConnection(GlobalVarsModule.connectionString)
        Dim com As String = "SELECT * FROM `printreceipt_tbl`"
        Dim adap As New MySqlDataAdapter(com, con)
        Dim ds As New DataSet
        adap.Fill(ds, "info")
        DataGridView1.DataSource = ds.Tables("info")

        DataGridView1.Columns("ID").Visible = False
        DataGridView1.EnableHeadersVisualStyles = False
        DataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(207, 58, 109)
        DataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White
        DataGridView1.ClearSelection()
    End Sub

    Private Async Sub OnDatabaseUpdated()
        Try
            Await GlobalVarsModule.LoadToGridAsync(DataGridView1, "SELECT * FROM `printreceipt_tbl` WHERE `IsPrinted` = 0")
            DataGridView1.ClearSelection()
        Catch
        End Try
    End Sub

    Private Sub txtsearch_TextChanged(sender As Object, e As EventArgs) Handles txtsearch.TextChanged

        HandleAutoRefreshPause(DataGridView1, txtsearch)

        Dim dt As DataTable = DirectCast(DataGridView1.DataSource, DataTable)
        If dt IsNot Nothing Then
            If txtsearch.Text.Trim() <> "" Then
                dt.DefaultView.RowFilter = String.Format("Name LIKE '*{0}*'", txtsearch.Text.Trim())
            Else
                dt.DefaultView.RowFilter = ""
            End If
        End If
    End Sub

    Private Sub DataGridView1_MouseHover(sender As Object, e As EventArgs) Handles DataGridView1.MouseHover
        PauseAutoRefresh(DataGridView1)
    End Sub

    Private Sub datagridview1_mouseleave(sender As Object, e As EventArgs) Handles DataGridView1.MouseLeave
        ResumeAutoRefresh(DataGridView1)
    End Sub

End Class

Public Class RawPrinterHelper

    <DllImport("winspool.Drv", EntryPoint:="OpenPrinterA", SetLastError:=True)>
    Public Shared Function OpenPrinter(szPrinter As String, ByRef hPrinter As IntPtr, pd As IntPtr) As Boolean
    End Function

    <DllImport("winspool.Drv", EntryPoint:="ClosePrinter", SetLastError:=True)>
    Public Shared Function ClosePrinter(hPrinter As IntPtr) As Boolean
    End Function

    <DllImport("winspool.Drv", EntryPoint:="StartDocPrinterA", SetLastError:=True)>
    Public Shared Function StartDocPrinter(hPrinter As IntPtr, level As Integer, di As DOCINFOA) As Boolean
    End Function

    <DllImport("winspool.Drv", EntryPoint:="EndDocPrinter", SetLastError:=True)>
    Public Shared Function EndDocPrinter(hPrinter As IntPtr) As Boolean
    End Function

    <DllImport("winspool.Drv", EntryPoint:="StartPagePrinter", SetLastError:=True)>
    Public Shared Function StartPagePrinter(hPrinter As IntPtr) As Boolean
    End Function

    <DllImport("winspool.Drv", EntryPoint:="EndPagePrinter", SetLastError:=True)>
    Public Shared Function EndPagePrinter(hPrinter As IntPtr) As Boolean
    End Function

    <DllImport("winspool.Drv", EntryPoint:="WritePrinter", SetLastError:=True)>
    Public Shared Function WritePrinter(hPrinter As IntPtr, pBytes As Byte(), dwCount As Integer, ByRef dwWritten As Integer) As Boolean
    End Function

    <StructLayout(LayoutKind.Sequential)>
    Public Structure DOCINFOA
        <MarshalAs(UnmanagedType.LPStr)>
        Public pDocName As String
        <MarshalAs(UnmanagedType.LPStr)>
        Public pOutputFile As String
        <MarshalAs(UnmanagedType.LPStr)>
        Public pDataType As String
    End Structure

    Public Shared Function SendBytesToPrinter(printerName As String, bytes As Byte()) As Boolean
        Dim hPrinter As IntPtr
        Dim docInfo As New DOCINFOA()
        docInfo.pDocName = "ESC POS Barcode"
        docInfo.pDataType = "RAW"

        If OpenPrinter(printerName, hPrinter, IntPtr.Zero) Then
            StartDocPrinter(hPrinter, 1, docInfo)
            StartPagePrinter(hPrinter)

            Dim written As Integer
            WritePrinter(hPrinter, bytes, bytes.Length, written)

            EndPagePrinter(hPrinter)
            EndDocPrinter(hPrinter)
            ClosePrinter(hPrinter)
            Return True
        End If

        Return False
    End Function

End Class