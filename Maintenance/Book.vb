Imports System.Collections.Generic
Imports System.Data
Imports System.Diagnostics
Imports System.Drawing
Imports System.Drawing.Printing
Imports System.IO
Imports MySql.Data.MySqlClient
Imports PdfSharp.Drawing
Imports PdfSharp.Pdf
Imports PdfSharp.Fonts
Imports TheArtOfDevHtmlRenderer.Adapters.Entities
Imports ZXing
Imports ZXing.Rendering
Imports ZXing.Windows.Compatibility

'kainisus'

Public Class Book


    Private Structure BarcodeInfo
        Public Barcode As String
        Public Title As String
    End Structure

    Private Const connectionString As String = "Server=localhost;Database=laybsisu_dbs;Uid=root;Pwd=;"

    Private isbarcode As Boolean = False

    Private BarcodeList As New List(Of BarcodeInfo)
    Private BarcodeIndex As Integer = 0

    Private Const BARCODE_PIXEL_WIDTH As Integer = 300
    Private Const BARCODE_PIXEL_HEIGHT As Integer = 100

    Private Const BARCODE_WIDTH_HM As Integer = 350
    Private Const BARCODE_HEIGHT_HM As Integer = 120
    Private Const HORIZONTAL_SPACING_HM As Integer = 20
    Private Const VERTICAL_SPACING_HM As Integer = 10
    Private Const MAX_ROWS_PER_COLUMN As Integer = 7

    Private WithEvents printDoc As New System.Drawing.Printing.PrintDocument
    Private LastSelectedBookID As Integer = -1
    Private skipRestore As Boolean = False
    Private skipRestoreTimer As System.Windows.Forms.Timer
    Private autoRefreshBooksRunning As Boolean = False


    Private Sub Book_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        TopMost = True
        Me.Font = New Font("Baskerville Old Face", 9)
        Me.Refresh()

        GlobalVarsModule.EnableCapitalizeFirstLetterForControls(Me)
        If GlobalFontSettings.FontResolver Is Nothing Then
            GlobalFontSettings.FontResolver = New CustomFontResolver()
        End If

        refreshbook()
        DisablePaste_AllTextBoxes()

        skipRestoreTimer = New System.Windows.Forms.Timer()
        skipRestoreTimer.Interval = 2000
        AddHandler skipRestoreTimer.Tick, Sub()
                                              skipRestore = False
                                              skipRestoreTimer.Stop()
                                          End Sub

        AddHandler cbauthor.DropDown, AddressOf RefreshComboBoxes
        AddHandler cbgenre.DropDown, AddressOf RefreshComboBoxes
        AddHandler cbpublisher.DropDown, AddressOf RefreshComboBoxes
        AddHandler cblanguage.DropDown, AddressOf RefreshComboBoxes

        AddHandler GlobalVarsModule.DatabaseUpdated, AddressOf OnDatabaseUpdated
    End Sub

    Private Sub DataGridView1_DataBindingComplete(sender As Object, e As DataGridViewBindingCompleteEventArgs) Handles DataGridView1.DataBindingComplete

        Try

            If DataGridView1.Columns.Contains("ID") Then
                DataGridView1.Columns("ID").Visible = False
            End If

            If DataGridView1.Columns.Contains("Delete") Then
                DataGridView1.Columns("Delete").DisplayIndex = DataGridView1.Columns.Count - 1
            End If

            If DataGridView1.Columns.Contains("Edit") Then
                DataGridView1.Columns("Edit").DisplayIndex = DataGridView1.Columns.Count - 2
            End If

            For Each row As DataGridViewRow In DataGridView1.Rows

                If DataGridView1.Columns.Contains("Edit") Then
                    row.Cells("Edit").Style.Alignment =
                        DataGridViewContentAlignment.MiddleCenter
                End If

                If DataGridView1.Columns.Contains("Delete") Then
                    row.Cells("Delete").Style.Alignment =
                        DataGridViewContentAlignment.MiddleCenter
                End If

            Next

        Catch ex As Exception
            Debug.WriteLine("DataBindingComplete error: " & ex.Message)
        End Try

    End Sub

    Private Sub DataGridView1_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellContentClick

        If e.RowIndex < 0 Then Exit Sub

        If e.ColumnIndex < 0 OrElse e.ColumnIndex >= DataGridView1.Columns.Count Then Exit Sub

        Dim clickedColumn As String = DataGridView1.Columns(e.ColumnIndex).Name
        Dim selectedRow As DataGridViewRow = DataGridView1.Rows(e.RowIndex)

        If selectedRow.IsNewRow Then Exit Sub



        If clickedColumn = "Edit" Then

            Try
                DataGridView1.ClearSelection()
                selectedRow.Selected = True

                Dim idObj = selectedRow.Cells("ID").Value

                If idObj IsNot Nothing AndAlso Not IsDBNull(idObj) Then
                    LastSelectedBookID = Convert.ToInt32(idObj)
                Else
                    LastSelectedBookID = -1
                End If

                Try
                    PauseAutoRefresh(DataGridView1)
                Catch
                End Try

                skipRestore = True

                If skipRestoreTimer IsNot Nothing Then
                    skipRestoreTimer.Stop()
                    skipRestoreTimer.Start()
                End If

                ApplyRowToFields(selectedRow)

                rbgenerate.Enabled = False

            Catch ex As Exception
                MessageBox.Show(
                    "Error loading book for editing: " & ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                )
            End Try

            Exit Sub

        End If



        If clickedColumn = "Delete" Then

            Dim bookID As Integer = CInt(selectedRow.Cells("ID").Value)
            Dim bookTitle As String = selectedRow.Cells("BookTitle").Value.ToString()

            Dim bookISBN As String = ""
            Dim bookBarcode As String = ""

            If selectedRow.Cells("ISBN").Value IsNot Nothing AndAlso
           Not IsDBNull(selectedRow.Cells("ISBN").Value) Then

                bookISBN = selectedRow.Cells("ISBN").Value.ToString()
            End If

            If selectedRow.Cells("Barcode").Value IsNot Nothing AndAlso
           Not IsDBNull(selectedRow.Cells("Barcode").Value) Then

                bookBarcode = selectedRow.Cells("Barcode").Value.ToString()
            End If


            Dim dialogResult As DialogResult =
            MessageBox.Show(
                "Are you sure you want to delete this book?" &
                Environment.NewLine &
                Environment.NewLine &
                "Book: " & bookTitle,
                "Confirm Delete",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            )

            If dialogResult <> DialogResult.Yes Then
                Exit Sub
            End If


            Using con As New MySqlConnection(GlobalVarsModule.connectionString)

                Try

                    con.Open()



                    Dim acs As New MySqlCommand(
                    "SELECT COUNT(*) 
                     FROM `acession_tbl`
                     WHERE (`ISBN` = @ISBN AND @ISBN <> '')
                     OR (`Barcode` = @Barcode AND @Barcode <> '')",
                    con)

                    acs.Parameters.AddWithValue("@ISBN", bookISBN)
                    acs.Parameters.AddWithValue("@Barcode", bookBarcode)

                    Dim accessionCount As Integer =
                    CInt(acs.ExecuteScalar())


                    If accessionCount > 0 Then

                        MsgBox(
                        "Cannot delete this book. It has existing accession records.",
                        vbExclamation,
                        "Deletion Blocked"
                    )

                        Exit Sub

                    End If



                    Dim acx As New MySqlCommand(
                    "SELECT COUNT(*)
                     FROM `acquisition_tbl`
                     WHERE (`ISBN` = @ISBN AND @ISBN <> '')
                     OR (`Barcode` = @Barcode AND @Barcode <> '')",
                    con)

                    acx.Parameters.AddWithValue("@ISBN", bookISBN)
                    acx.Parameters.AddWithValue("@Barcode", bookBarcode)

                    Dim acquisitionCount As Integer =
                    CInt(acx.ExecuteScalar())


                    If acquisitionCount > 0 Then

                        MsgBox(
                        "Cannot delete this book. It has existing acquisition records.",
                        vbExclamation,
                        "Deletion Blocked"
                    )

                        Exit Sub

                    End If



                    Dim deleteCmd As New MySqlCommand(
                    "DELETE FROM `book_tbl`
                     WHERE `ID` = @id",
                    con)

                    deleteCmd.Parameters.AddWithValue("@id", bookID)

                    Dim affectedRows As Integer =
                    deleteCmd.ExecuteNonQuery()


                    If affectedRows = 0 Then

                        MsgBox(
                        "Book was not found or was already deleted.",
                        vbExclamation,
                        "Delete Failed"
                    )

                        Exit Sub

                    End If


                    GlobalVarsModule.LogAudit(
                    actionType:="DELETE",
                    formName:="BOOK FORM",
                    description:=$"Deleted Book: {bookTitle}",
                    recordID:=bookID.ToString()
                )



                    For Each form In Application.OpenForms

                        If TypeOf form Is AuditTrail Then
                            DirectCast(form, AuditTrail).refreshaudit()
                        End If

                        If TypeOf form Is AvailableBooks Then
                            DirectCast(form, AvailableBooks).refreshavail()
                        End If

                        If TypeOf form Is Acquisition2 Then
                            DirectCast(form, Acquisition2).refreshData()
                        End If

                        If TypeOf form Is Accession Then
                            DirectCast(form, Accession).RefreshAccessionData()
                        End If

                        If TypeOf form Is ReserveCopies Then
                            DirectCast(form, ReserveCopies).reserveload()
                        End If

                        If TypeOf form Is Borrowing Then
                            'DirectCast(form, Borrowing).refreshborrowingsu()
                        End If

                    Next


                    MsgBox(
                    "Book deleted successfully.",
                    vbInformation,
                    "Success"
                )


                    LastSelectedBookID = -1

                    clear()

                    LoadBookData()


                    Dim countCmd As New MySqlCommand(
                    "SELECT COUNT(*) FROM `book_tbl`",
                    con)

                    Dim rowCount As Long =
                    CLng(countCmd.ExecuteScalar())


                    If rowCount = 0 Then

                        Dim resetCmd As New MySqlCommand(
                        "ALTER TABLE `book_tbl` AUTO_INCREMENT = 1",
                        con)

                        resetCmd.ExecuteNonQuery()

                    End If


                Catch ex As Exception

                    MsgBox(
                    "An error occurred while deleting the book: " &
                    ex.Message,
                    vbCritical,
                    "Delete Error"
                )

                End Try

            End Using

        End If

    End Sub


    Public Sub refreshbook()
        If String.IsNullOrEmpty(GlobalVarsModule.connectionString) Then
            MessageBox.Show("Connection string is not set.", "Configuration Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End If

        LoadBookData()
        SetupGridStyle()
        cbauthorr()
        cbgenree()
        cbpublisherr()
        cblang()
        clear()
        picbarcode.Image = GenerateBarcodeImage(lblrandom.Text, picbarcode.Width, picbarcode.Height)
    End Sub


    Public Async Sub AutoRefreshBooks()
        If String.IsNullOrEmpty(GlobalVarsModule.connectionString) Then Return
        If IsDisposed OrElse Disposing Then Return
        If autoRefreshBooksRunning Then Return

        autoRefreshBooksRunning = True

        Try
            Dim selectedID As Integer = LastSelectedBookID
            Dim scrollIndex As Integer = -1

            Dim savedBookTitle As String = txtbooktitle.Text
            Dim savedISBN As String = txtisbn.Text
            Dim savedYear As String = txtyearr.Text
            Dim savedAuthor As String = cbauthor.Text
            Dim savedGenre As String = cbgenre.Text
            Dim savedPublisher As String = cbpublisher.Text
            Dim savedLanguage As String = cblanguage.Text
            Dim savedRandom As String = lblrandom.Text
            Dim savedGenerate As Boolean = rbgenerate.Checked
            Dim savedISBNEnabled As Boolean = txtisbn.Enabled
            Dim savedGenerateEnabled As Boolean = rbgenerate.Enabled

            Try
                If DataGridView1.RowCount > 0 Then scrollIndex = DataGridView1.FirstDisplayedScrollingRowIndex
            Catch
            End Try

            If selectedID <= 0 AndAlso DataGridView1.SelectedRows.Count > 0 Then
                Try
                    Dim idObj = DataGridView1.SelectedRows(0).Cells("ID").Value
                    If idObj IsNot Nothing AndAlso Not IsDBNull(idObj) Then selectedID = Convert.ToInt32(idObj)
                Catch
                End Try
            End If

            Dim query As String = "SELECT * FROM `book_tbl` ORDER BY ID DESC"
            Await GlobalVarsModule.LoadToGridAsync(DataGridView1, query)

            If IsDisposed OrElse Disposing Then Return

            SetupGridStyle()

            If selectedID > 0 Then
                For i As Integer = 0 To DataGridView1.Rows.Count - 1
                    Dim row As DataGridViewRow = DataGridView1.Rows(i)
                    If row.IsNewRow Then Continue For

                    Try
                        Dim idObj = row.Cells("ID").Value
                        If idObj IsNot Nothing AndAlso Not IsDBNull(idObj) AndAlso Convert.ToInt32(idObj) = selectedID Then
                            DataGridView1.ClearSelection()
                            row.Selected = True
                            Dim currentColumnIndex As Integer = 0
                            For c As Integer = 0 To DataGridView1.Columns.Count - 1
                                If DataGridView1.Columns(c).Visible Then
                                    currentColumnIndex = c
                                    Exit For
                                End If
                            Next
                            DataGridView1.CurrentCell = row.Cells(currentColumnIndex)
                            LastSelectedBookID = selectedID
                            Exit For
                        End If
                    Catch ex As Exception
                        Debug.WriteLine("AutoRefreshBooks selection restore error: " & ex.Message)
                    End Try
                Next
            Else
                DataGridView1.ClearSelection()
            End If

            If scrollIndex >= 0 AndAlso scrollIndex < DataGridView1.RowCount Then
                Try
                    DataGridView1.FirstDisplayedScrollingRowIndex = scrollIndex
                Catch
                End Try
            End If

            txtbooktitle.Text = savedBookTitle
            txtisbn.Text = savedISBN
            txtyearr.Text = savedYear
            cbauthor.Text = savedAuthor
            cbgenre.Text = savedGenre
            cbpublisher.Text = savedPublisher
            cblanguage.Text = savedLanguage
            lblrandom.Text = savedRandom
            rbgenerate.Checked = savedGenerate
            txtisbn.Enabled = savedISBNEnabled
            rbgenerate.Enabled = savedGenerateEnabled

            If Not String.IsNullOrWhiteSpace(savedRandom) AndAlso savedRandom <> "0000000000000" Then
                picbarcode.Image = GenerateBarcodeImage(savedRandom, picbarcode.Width, picbarcode.Height)
            ElseIf savedGenerate Then
                picbarcode.Image = GenerateBarcodeImage(savedRandom, picbarcode.Width, picbarcode.Height)
            Else
                picbarcode.Image = GenerateBarcodeImage("0000000000000", picbarcode.Width, picbarcode.Height)
            End If

        Catch ex As Exception
            Debug.WriteLine("AutoRefreshBooks error: " & ex.Message)
        Finally
            autoRefreshBooksRunning = False
        End Try
    End Sub

    Private Sub RefreshComboBoxes(sender As Object, e As EventArgs)
        Dim cb As ComboBox = DirectCast(sender, ComboBox)

        Using con As New MySqlConnection(GlobalVarsModule.connectionString)
            Dim query As String = ""

            Select Case cb.Name.ToLower()
                Case "cbauthor"
                    query = "SELECT AuthorName FROM author_tbl ORDER BY AuthorName"
                Case "cbgenre"
                    query = "SELECT Genre FROM genre_tbl ORDER BY Genre"
                Case "cbpublisher"
                    query = "SELECT PublisherName FROM publisher_tbl ORDER BY PublisherName"
                Case "cblanguage"
                    query = "SELECT Language FROM language_tbl ORDER BY Language"
            End Select

            If query <> "" Then
                Dim dt As New DataTable()
                Dim da As New MySqlDataAdapter(query, con)
                da.Fill(dt)

                cb.DataSource = dt
                cb.DisplayMember = dt.Columns(0).ColumnName
                cb.ValueMember = dt.Columns(0).ColumnName
                cb.SelectedIndex = -1
            End If
        End Using
    End Sub


    Private Sub LoadBookData()
        Dim query As String = "SELECT * FROM `book_tbl` ORDER BY ID DESC"
        GlobalVarsModule.AutoRefreshGrid(DataGridView1, query, 2000)
    End Sub


    Private Sub OnDatabaseUpdated()
        If IsDisposed OrElse Disposing Then Return
        AutoRefreshBooks()
    End Sub

    Private Sub SetupGridStyle()
        Try
            If DataGridView1.Columns.Contains("ID") Then
                DataGridView1.Columns("ID").Visible = False
            End If

            DataGridView1.EnableHeadersVisualStyles = False
            DataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(207, 58, 109)
            DataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White
            DataGridView1.ReadOnly = True
        Catch
        End Try
    End Sub


    Public Sub cbauthorr()

        Dim con As New MySqlConnection(GlobalVarsModule.connectionString)
        Dim com As String = "SELECT `ID`, `AuthorName` FROM `author_tbl`"
        Dim adap As New MySqlDataAdapter(com, con)
        Dim dt As New DataTable

        adap.Fill(dt)

        cbauthor.DataSource = dt
        cbauthor.DisplayMember = "AuthorName"
        cbauthor.ValueMember = "ID"
        cbauthor.SelectedIndex = -1

    End Sub

    Public Sub cbgenree()

        Dim con As New MySqlConnection(GlobalVarsModule.connectionString)
        Dim com As String = "SELECT ID, Genre FROM genre_tbl"
        Dim adap As New MySqlDataAdapter(com, con)
        Dim dt As New DataTable

        adap.Fill(dt)

        cbgenre.DataSource = dt
        cbgenre.DisplayMember = "Genre"
        cbgenre.ValueMember = "ID"
        cbgenre.SelectedIndex = -1

    End Sub

    Public Sub cbpublisherr()

        Dim con As New MySqlConnection(GlobalVarsModule.connectionString)
        Dim com As String = "SELECT ID, PublisherName FROM `publisher_tbl`"
        Dim adap As New MySqlDataAdapter(com, con)
        Dim dt As New DataTable

        adap.Fill(dt)

        cbpublisher.DataSource = dt
        cbpublisher.DisplayMember = "PublisherName"
        cbpublisher.ValueMember = "ID"
        cbpublisher.SelectedIndex = -1

    End Sub

    Public Sub cblang()

        Dim con As New MySqlConnection(GlobalVarsModule.connectionString)
        Dim com As String = "SELECT `ID`, `Language` FROM `language_tbl`"
        Dim adap As New MySqlDataAdapter(com, con)
        Dim dt As New DataTable

        adap.Fill(dt)
        cblanguage.DataSource = dt
        cblanguage.DisplayMember = "Language"
        cblanguage.ValueMember = "ID"
        cblanguage.SelectedIndex = -1
    End Sub



    Private Sub Book_Shown(sender As Object, e As EventArgs) Handles MyBase.Shown

        DataGridView1.ClearSelection()
        LastSelectedBookID = -1

    End Sub


    Private Sub ExportPDF_Click(sender As Object, e As EventArgs) Handles Guna2Button1.Click

        Try
            BarcodeList.Clear()
            Using con As New MySqlConnection(GlobalVarsModule.connectionString)
                Dim query As String = "SELECT Barcode, BookTitle FROM book_tbl WHERE Barcode IS NOT NULL AND Barcode <> ''"
                Dim cmd As New MySqlCommand(query, con)
                con.Open()
                Dim reader As MySqlDataReader = cmd.ExecuteReader()
                While reader.Read()
                    BarcodeList.Add(New BarcodeInfo With {
                .Barcode = reader("Barcode").ToString(),
                .Title = reader("BookTitle").ToString()
            })
                End While
            End Using

            If BarcodeList.Count = 0 Then
                MessageBox.Show("No barcodes found.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Return
            End If

            Dim sfd As New SaveFileDialog()
            sfd.Filter = "PDF File|*.pdf"
            sfd.FileName = "BarcodeList.pdf"
            If sfd.ShowDialog() <> DialogResult.OK Then Return

            Dim pdf As New PdfDocument()
            pdf.Info.Title = "Barcode List"

            Dim page As PdfPage = pdf.AddPage()
            Dim gfx As XGraphics = XGraphics.FromPdfPage(page)

            Dim font As XFont = New XFont("Arial", 10, XFontStyleEx.Regular,
            New XPdfFontOptions(PdfFontEncoding.Unicode, PdfFontEmbedding.EmbedCompleteFontFile))


            Dim marginX As Double = 40
            Dim currentY As Double = 40
            Dim boxWidth As Double = 270
            Dim boxHeight As Double = 100
            Dim padding As Double = 10
            Dim lineHeight As Double = 120

            For Each item In BarcodeList


                Dim writer As New ZXing.Windows.Compatibility.BarcodeWriter()
                writer.Format = ZXing.BarcodeFormat.CODE_128
                writer.Options = New ZXing.Common.EncodingOptions With {
                .Width = 250,
                .Height = 60,
                .Margin = 2
            }

                Dim barcodeBitmap As Bitmap = writer.Write(item.Barcode)
                Dim ms As New System.IO.MemoryStream()
                barcodeBitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png)
                ms.Position = 0
                Dim xImg As XImage = XImage.FromStream(ms)


                Dim pen As New XPen(XColors.Black, 1)

                gfx.DrawRectangle(pen, marginX, currentY, boxWidth, boxHeight)


                Dim displayTitle As String = item.Title
                If displayTitle.Length > 40 Then displayTitle = displayTitle.Substring(0, 37) & "..."

                gfx.DrawString(displayTitle, font, XBrushes.Black,
                           New XPoint(marginX + padding, currentY + 20))


                gfx.DrawImage(xImg, marginX + 10, currentY + 30, 250, 60)


                currentY += lineHeight

                If currentY > page.Height.Point - 150 Then
                    page = pdf.AddPage()
                    gfx = XGraphics.FromPdfPage(page)
                    currentY = 40
                End If
            Next

            pdf.Save(sfd.FileName)
            MessageBox.Show($"PDF saved successfully at {sfd.FileName}", "Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information)

        Catch ex As Exception
            MessageBox.Show("Error: " & ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub printDoc_PrintPage(sender As Object, e As PrintPageEventArgs) Handles printDoc.PrintPage

        Dim x As Integer = 25
        Dim y As Integer = 20
        Dim fontTitle As New Font("Arial", 9, FontStyle.Bold)

        While BarcodeIndex < BarcodeList.Count
            Dim info = BarcodeList(BarcodeIndex)

            Dim img As Image = GenerateBarcodeImage(info.Barcode, 280, 100)

            e.Graphics.DrawString(info.Title, fontTitle, Brushes.Black, x, y)
            e.Graphics.DrawImage(img, x, y + 20)

            y += 130
            BarcodeIndex += 1

            If y > e.PageSettings.PrintableArea.Height - 100 And BarcodeIndex < BarcodeList.Count Then
                e.HasMorePages = True
                Return
            End If
        End While

        e.HasMorePages = False

    End Sub



    Function GenerateBarcodeImage(ByVal barcodeText As String, ByVal width As Integer, ByVal height As Integer) As Image
        Try

            Dim renderWidth As Integer = BARCODE_PIXEL_WIDTH
            Dim renderHeight As Integer = 80
            Dim totalLabelHeight As Integer = BARCODE_PIXEL_HEIGHT

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
                    Using sf As New StringFormat With {
                     .Alignment = StringAlignment.Center
                 }

                        g.DrawString(barcodeText, font, Brushes.Black, New RectangleF(0, renderHeight, renderWidth, totalLabelHeight - renderHeight), sf)
                    End Using
                End Using

                Using borderPen As New Pen(Color.Black, 1)

                    g.DrawRectangle(borderPen, 0, 0, renderWidth - 1, totalLabelHeight - 1)
                End Using

            End Using

            barcodeBitmap.Dispose()

            If width = renderWidth AndAlso height = totalLabelHeight Then

                Return printImage
            Else

                Dim finalDisplayImage As New Bitmap(printImage, New Size(width, height))
                printImage.Dispose()
                Return finalDisplayImage
            End If

        Catch ex As Exception
            MessageBox.Show("Error: " & ex.Message, "Barcode Error", MessageBoxButtons.OK, MessageBoxIcon.Error)

            Dim bmp As New Bitmap(width, height)
            Using g As Graphics = Graphics.FromImage(bmp)
                g.Clear(Color.Red)
                Using font As New Font("Arial", 10)
                    g.DrawString("ERROR: " & barcodeText, font, Brushes.White, 10, 10)
                End Using
            End Using
            Return bmp
        End Try
    End Function


    Private Sub btnadd_Click(sender As Object, e As EventArgs) Handles btnadd.Click

        Dim isUpdate As Boolean = False
        Dim bookID As Integer = -1
        Dim oldBookTitle As String = ""



        If DataGridView1.SelectedRows.Count > 0 Then

            Dim selectedRow As DataGridViewRow = DataGridView1.SelectedRows(0)

            If Not selectedRow.IsNewRow Then

                Dim idObj = selectedRow.Cells("ID").Value

                If idObj IsNot Nothing AndAlso Not IsDBNull(idObj) Then

                    bookID = Convert.ToInt32(idObj)
                    oldBookTitle = selectedRow.Cells("BookTitle").Value.ToString()
                    isUpdate = True

                End If

            End If

        End If


        Dim newBookTitle As String = txtbooktitle.Text.Trim()
        Dim yearsu As String = txtyearr.Text.Trim()

        Dim currentYear As Integer = Date.Now.Year
        Dim inputYear As Integer


        If Not Integer.TryParse(yearsu, inputYear) OrElse
       inputYear > currentYear OrElse
       inputYear < 1700 Then

            MsgBox("Invalid Year. Please enter a valid year.",
               vbExclamation,
               "Validation Error")
            Exit Sub
        End If


        Dim authorValid As Boolean =
        chkauthor.Checked OrElse
        cbauthor.SelectedIndex >= 0 OrElse
        Not String.IsNullOrWhiteSpace(cbauthor.Text)

        Dim genreValid As Boolean =
        cbgenre.SelectedIndex >= 0 OrElse
        Not String.IsNullOrWhiteSpace(cbgenre.Text)

        Dim publisherValid As Boolean =
        cbpublisher.SelectedIndex >= 0 OrElse
        Not String.IsNullOrWhiteSpace(cbpublisher.Text)

        Dim languageValid As Boolean =
        cblanguage.SelectedIndex >= 0 OrElse
        Not String.IsNullOrWhiteSpace(cblanguage.Text)


        If String.IsNullOrWhiteSpace(newBookTitle) OrElse
       Not authorValid OrElse
       Not genreValid OrElse
       Not publisherValid OrElse
       Not languageValid Then

            MsgBox("Please fill in all the required fields.",
               vbExclamation,
               "Validation Error")
            Exit Sub
        End If


        Dim con As New MySqlConnection(GlobalVarsModule.connectionString)

        Dim isbnValue As Object
        Dim barcodeValue As Object


        Try


            If rbgenerate.Checked Then

                isbnValue = DBNull.Value
                barcodeValue = lblrandom.Text.Trim()

                If String.IsNullOrWhiteSpace(CStr(barcodeValue)) OrElse
                   CStr(barcodeValue) = "0000000000000" Then

                    MsgBox("Please generate a valid barcode.",
                       vbExclamation,
                       "Validation Error")
                    Exit Sub

                End If


                Dim barcodeQuery As String

                If isUpdate Then
                    barcodeQuery =
                    "SELECT COUNT(*) FROM `book_tbl`
                     WHERE `Barcode` = @barcode
                     AND `ID` <> @id"
                Else
                    barcodeQuery =
                    "SELECT COUNT(*) FROM `book_tbl`
                     WHERE `Barcode` = @barcode"
                End If

                Dim coms As New MySqlCommand(barcodeQuery, con)

                coms.Parameters.AddWithValue("@barcode", barcodeValue)

                If isUpdate Then
                    coms.Parameters.AddWithValue("@id", bookID)
                End If

                con.Open()

                Dim count As Integer = CInt(coms.ExecuteScalar())

                con.Close()


                If count > 0 Then

                    MsgBox("This barcode already exists.",
                       vbExclamation,
                       "Duplication not allowed.")

                    Exit Sub
                End If


            Else

                isbnValue = txtisbn.Text.Trim()

                If String.IsNullOrEmpty(CStr(isbnValue)) Then

                    MsgBox("Please enter a valid ISBN.",
                       vbExclamation,
                       "Validation Error")

                    Exit Sub
                End If


                Dim isbnString As String =
                CStr(isbnValue).Replace("-", "").Replace(" ", "")


                If Not System.Text.RegularExpressions.Regex.IsMatch(
                isbnString,
                "^97[1-9]\d{10}$") Then

                    MsgBox("Invalid ISBN format.",
                       vbExclamation,
                       "Validation Error")

                    Exit Sub
                End If


                Dim isbnQuery As String

                If isUpdate Then
                    isbnQuery =
                    "SELECT COUNT(*) FROM `book_tbl`
                     WHERE `ISBN` = @isbn
                     AND `ID` <> @id"
                Else
                    isbnQuery =
                    "SELECT COUNT(*) FROM `book_tbl`
                     WHERE `ISBN` = @isbn"
                End If


                Dim coms As New MySqlCommand(isbnQuery, con)

                coms.Parameters.AddWithValue("@isbn", isbnValue)

                If isUpdate Then
                    coms.Parameters.AddWithValue("@id", bookID)
                End If

                con.Open()

                Dim count As Integer = CInt(coms.ExecuteScalar())

                con.Close()


                If count > 0 Then

                    MsgBox("The ISBN already exists. Please enter a unique ISBN.",
                       vbExclamation,
                       "Duplication not allowed.")

                    Exit Sub
                End If


                barcodeValue = DBNull.Value

            End If


            If Not isUpdate Then

                con.Open()

                Dim addCmd As New MySqlCommand(
                "INSERT INTO `book_tbl`
                (`Barcode`,
                 `ISBN`,
                 `BookTitle`,
                 `Author`,
                 `Genre`,
                 `Publisher`,
                 `Language`,
                 `YearPublished`)
                 VALUES
                (@barcode,
                 @isbn,
                 @booktitle,
                 @author,
                 @genre,
                 @publisher,
                 @language,
                 @yearpublished)",
                con)


                addCmd.Parameters.AddWithValue(
                "@barcode",
                If(IsDBNull(barcodeValue),
                   DBNull.Value,
                   barcodeValue))

                addCmd.Parameters.AddWithValue(
                "@isbn",
                If(IsDBNull(isbnValue),
                   DBNull.Value,
                   isbnValue))

                addCmd.Parameters.AddWithValue(
                "@booktitle",
                newBookTitle)

                addCmd.Parameters.AddWithValue(
                "@author",
                If(chkauthor.Checked,
                   DBNull.Value,
                   cbauthor.Text))

                addCmd.Parameters.AddWithValue("@genre", cbgenre.Text)
                addCmd.Parameters.AddWithValue("@publisher", cbpublisher.Text)
                addCmd.Parameters.AddWithValue("@language", cblanguage.Text)
                addCmd.Parameters.AddWithValue("@yearpublished", yearsu)

                addCmd.ExecuteNonQuery()

                Dim newBookID As Long = addCmd.LastInsertedId

                con.Close()


                GlobalVarsModule.LogAudit(
                actionType:="ADD",
                formName:="BOOK FORM",
                description:=$"Added Book: {newBookTitle}",
                recordID:=newBookID.ToString()
            )


                For Each form In Application.OpenForms

                    If TypeOf form Is Acquisition2 Then
                        DirectCast(form, Acquisition2).refreshData()
                    End If

                    If TypeOf form Is Accession Then
                        DirectCast(form, Accession).RefreshAccessionData()
                    End If

                    If TypeOf form Is ReserveCopies Then
                        DirectCast(form, ReserveCopies).reserveload()
                    End If

                    If TypeOf form Is Borrowing Then
                        'DirectCast(form, Borrowing).refreshborrowingsu()
                    End If

                    If TypeOf form Is AvailableBooks Then
                        DirectCast(form, AvailableBooks).refreshavail()
                    End If

                    If TypeOf form Is AuditTrail Then
                        DirectCast(form, AuditTrail).refreshaudit()
                    End If

                Next


                MsgBox(
                "Book added successfully.",
                vbInformation,
                "Success"
            )


                LastSelectedBookID = -1

                clear()
                LoadBookData()

                Exit Sub

            End If


            con.Open()

            Dim com As New MySqlCommand(
            "UPDATE `book_tbl`
             SET
             `Barcode` = @barcode,
             `ISBN` = @isbn,
             `BookTitle` = @booktitle,
             `Author` = @author,
             `Genre` = @genre,
             `Publisher` = @publisher,
             `Language` = @language,
             `YearPublished` = @yearpublished
             WHERE `ID` = @id",
            con)


            com.Parameters.AddWithValue(
            "@barcode",
            If(IsDBNull(barcodeValue),
               DBNull.Value,
               barcodeValue))

            com.Parameters.AddWithValue(
            "@isbn",
            If(IsDBNull(isbnValue),
               DBNull.Value,
               isbnValue))

            com.Parameters.AddWithValue(
            "@booktitle",
            newBookTitle)

            com.Parameters.AddWithValue(
            "@author",
            If(chkauthor.Checked,
               DBNull.Value,
               cbauthor.Text))

            com.Parameters.AddWithValue("@genre", cbgenre.Text)
            com.Parameters.AddWithValue("@publisher", cbpublisher.Text)
            com.Parameters.AddWithValue("@language", cblanguage.Text)
            com.Parameters.AddWithValue("@yearpublished", yearsu)
            com.Parameters.AddWithValue("@id", bookID)

            com.ExecuteNonQuery()



            Dim comAcquisition As New MySqlCommand(
            "UPDATE `acquisition_tbl`
             SET `BookTitle` = @newBookTitle
             WHERE `BookTitle` = @oldBookTitle",
            con)

            comAcquisition.Parameters.AddWithValue("@newBookTitle", newBookTitle)
            comAcquisition.Parameters.AddWithValue("@oldBookTitle", oldBookTitle)
            comAcquisition.ExecuteNonQuery()


            Dim comAccession As New MySqlCommand(
            "UPDATE `acession_tbl`
             SET `BookTitle` = @newBookTitle
             WHERE `BookTitle` = @oldBookTitle",
            con)

            comAccession.Parameters.AddWithValue("@newBookTitle", newBookTitle)
            comAccession.Parameters.AddWithValue("@oldBookTitle", oldBookTitle)
            comAccession.ExecuteNonQuery()


            Dim comBorrowing As New MySqlCommand(
            "UPDATE `borrowing_tbl`
             SET `BookTitle` = @newBookTitle
             WHERE `BookTitle` = @oldBookTitle",
            con)

            comBorrowing.Parameters.AddWithValue("@newBookTitle", newBookTitle)
            comBorrowing.Parameters.AddWithValue("@oldBookTitle", oldBookTitle)
            comBorrowing.ExecuteNonQuery()


            Dim comReserve As New MySqlCommand(
            "UPDATE `reservecopiess_tbl`
             SET `BookTitle` = @newBookTitle
             WHERE `BookTitle` = @oldBookTitle",
            con)

            comReserve.Parameters.AddWithValue("@newBookTitle", newBookTitle)
            comReserve.Parameters.AddWithValue("@oldBookTitle", oldBookTitle)
            comReserve.ExecuteNonQuery()


            Dim comAvailable As New MySqlCommand(
            "UPDATE `available_tbl`
             SET `BookTitle` = @newBookTitle
             WHERE `BookTitle` = @oldBookTitle",
            con)

            comAvailable.Parameters.AddWithValue("@newBookTitle", newBookTitle)
            comAvailable.Parameters.AddWithValue("@oldBookTitle", oldBookTitle)
            comAvailable.ExecuteNonQuery()


            con.Close()



            GlobalVarsModule.LogAudit(
            actionType:="UPDATE",
            formName:="BOOK FORM",
            description:=$"Updated Book Title from '{oldBookTitle}' to '{newBookTitle}' (ID: {bookID})",
            recordID:=bookID.ToString(),
            oldValue:=oldBookTitle,
            newValue:=newBookTitle
        )


            For Each form In Application.OpenForms

                If TypeOf form Is Acquisition2 Then
                    DirectCast(form, Acquisition2).refreshData()
                End If

                If TypeOf form Is Accession Then
                    DirectCast(form, Accession).RefreshAccessionData()
                End If

                If TypeOf form Is ReserveCopies Then
                    DirectCast(form, ReserveCopies).reserveload()
                End If

                If TypeOf form Is Borrowing Then
                    'DirectCast(form, Borrowing).refreshborrowingsu()
                End If

                If TypeOf form Is AvailableBooks Then
                    DirectCast(form, AvailableBooks).refreshavail()
                End If

                If TypeOf form Is AuditTrail Then
                    DirectCast(form, AuditTrail).refreshaudit()
                End If

            Next


            MsgBox(
            "Book updated successfully.",
            vbInformation,
            "Success"
        )


            LastSelectedBookID = -1

            clear()
            LoadBookData()


        Catch ex As Exception

            MsgBox("An error occurred: " & ex.Message,
               vbCritical,
               "Error")

        Finally

            If con.State = ConnectionState.Open Then
                con.Close()
            End If

        End Try

    End Sub


    Private Sub btnclear_Click(sender As Object, e As EventArgs) Handles btnclear.Click
        clear()
    End Sub

    Public Sub clear()

        txtisbn.Text = ""
        txtbooktitle.Text = ""
        txtyearr.Text = ""
        lblrandom.Text = "0000000000000"
        lblrandom.Visible = False

        txtisbn.Enabled = True
        rbgenerate.Enabled = True

        cbauthor.DataSource = Nothing
        cbgenre.DataSource = Nothing
        'cbcategory.DataSource = Nothing
        cbpublisher.DataSource = Nothing
        cblanguage.DataSource = Nothing
        rbgenerate.Checked = False

        cbauthorr()
        cbgenree()
        'cbcategoryy()
        cbpublisherr()
        cblang()

        picbarcode.Image = GenerateBarcodeImage(lblrandom.Text, picbarcode.Width, picbarcode.Height)

        DataGridView1.ClearSelection()
        chkauthor.Checked = False
        cbauthor.Enabled = True
    End Sub


    Function jinireyt() As String
        Dim random As New Random()
        Dim barcode As String = ""
        HandleAutoRefreshPause(DataGridView1, txtsearch)

        For i As Integer = 0 To 12
            barcode += random.Next(0, 10).ToString()
        Next

        Return barcode
    End Function


    Private Sub txtsearch_TextChanged(sender As Object, e As EventArgs) Handles txtsearch.TextChanged

        HandleAutoRefreshPause(DataGridView1, txtsearch)

        Dim dt As DataTable = DirectCast(DataGridView1.DataSource, DataTable)

        If dt IsNot Nothing Then

            If txtsearch.Text.Trim() <> "" Then

                Dim filter As String =
                    String.Format(
                        "BookTitle LIKE '*{0}*' OR ISBN LIKE '*{0}*'",
                        txtsearch.Text.Trim()
                    )

                dt.DefaultView.RowFilter = filter

            Else

                dt.DefaultView.RowFilter = ""

            End If

        End If

    End Sub


    Private Sub txtisbn_KeyDown(sender As Object, e As KeyEventArgs) Handles txtisbn.KeyDown

        If e.Control AndAlso
           (e.KeyCode = Keys.V Or e.KeyCode = Keys.C Or e.KeyCode = Keys.X) Then

            e.SuppressKeyPress = True

        End If

    End Sub

    Private Sub txtisbn_KeyPress(sender As Object, e As KeyPressEventArgs) Handles txtisbn.KeyPress

        If Not Char.IsDigit(e.KeyChar) And Not Char.IsControl(e.KeyChar) Then
            e.Handled = True
        End If

    End Sub

    Private Sub txtbooktitle_KeyDown(sender As Object, e As KeyEventArgs) Handles txtbooktitle.KeyDown

        If e.Control AndAlso
           (e.KeyCode = Keys.V Or e.KeyCode = Keys.C Or e.KeyCode = Keys.X) Then

            e.SuppressKeyPress = True

        End If

    End Sub

    Private Sub txtsearch_KeyDown(sender As Object, e As KeyEventArgs) Handles txtsearch.KeyDown

        If e.Control AndAlso
           (e.KeyCode = Keys.V Or e.KeyCode = Keys.C Or e.KeyCode = Keys.X) Then

            e.SuppressKeyPress = True

        End If

    End Sub

    Private Sub jinreytsu()

        skipRestore = True

        If skipRestoreTimer IsNot Nothing Then
            skipRestoreTimer.Stop()
            skipRestoreTimer.Start()
        End If

        If rbgenerate.Checked Then

            Dim newBarcode As String = jinireyt()

            lblrandom.Text = newBarcode

            picbarcode.Image =
                GenerateBarcodeImage(
                    newBarcode,
                    picbarcode.Width,
                    picbarcode.Height
                )

            txtisbn.Enabled = False
            txtisbn.Text = ""

        Else

            lblrandom.Text = "0000000000000"
            picbarcode.Image = Nothing
            txtisbn.Enabled = True

        End If

    End Sub

    Private Sub rbgenerate_CheckedChanged(sender As Object, e As EventArgs) Handles rbgenerate.CheckedChanged

        If Not isbarcode Then
            jinreytsu()
            DataGridView1.ClearSelection()
        End If

    End Sub

    Private Function SafeCellValue(row As DataGridViewRow, columnName As String) As String

        Try

            If row.Cells(columnName).Value IsNot Nothing Then
                Return row.Cells(columnName).Value.ToString()
            End If

        Catch
        End Try

        Return ""

    End Function

    Private Sub ApplyRowToFields(row As DataGridViewRow)

        If row Is Nothing Then Return

        Try

            isbarcode = True

            txtbooktitle.Text =
            SafeCellValue(row, "BookTitle")


            Dim authorVal As String =
            SafeCellValue(row, "Author")

            If String.IsNullOrWhiteSpace(authorVal) Then

                chkauthor.Checked = True
                cbauthor.SelectedIndex = -1
                cbauthor.Enabled = False

            Else

                chkauthor.Checked = False
                cbauthor.Enabled = True

                If cbauthor.DataSource Is Nothing Then
                    cbauthorr()
                End If

                Dim authorIndex As Integer =
                cbauthor.FindStringExact(authorVal)

                If authorIndex >= 0 Then
                    cbauthor.SelectedIndex = authorIndex
                Else
                    cbauthor.Text = authorVal
                End If

            End If



            Dim genreVal As String =
            SafeCellValue(row, "Genre")

            If String.IsNullOrWhiteSpace(genreVal) Then

                cbgenre.SelectedIndex = -1

            Else

                If cbgenre.DataSource Is Nothing Then
                    cbgenree()
                End If

                Dim genreIndex As Integer =
                cbgenre.FindStringExact(genreVal)

                If genreIndex >= 0 Then
                    cbgenre.SelectedIndex = genreIndex
                Else
                    cbgenre.Text = genreVal
                End If

            End If



            Dim publisherVal As String =
            SafeCellValue(row, "Publisher")

            If String.IsNullOrWhiteSpace(publisherVal) Then

                cbpublisher.SelectedIndex = -1

            Else

                If cbpublisher.DataSource Is Nothing Then
                    cbpublisherr()
                End If

                Dim publisherIndex As Integer =
                cbpublisher.FindStringExact(publisherVal)

                If publisherIndex >= 0 Then
                    cbpublisher.SelectedIndex = publisherIndex
                Else
                    cbpublisher.Text = publisherVal
                End If

            End If



            Dim languageVal As String =
            SafeCellValue(row, "Language")

            If String.IsNullOrWhiteSpace(languageVal) Then

                cblanguage.SelectedIndex = -1

            Else

                If cblanguage.DataSource Is Nothing Then
                    cblang()
                End If

                Dim languageIndex As Integer =
                cblanguage.FindStringExact(languageVal)

                If languageIndex >= 0 Then
                    cblanguage.SelectedIndex = languageIndex
                Else
                    cblanguage.Text = languageVal
                End If

            End If


            txtyearr.Text =
            SafeCellValue(row, "YearPublished")


            Dim barcodeValue As String =
            SafeCellValue(row, "Barcode")

            lblrandom.Text = barcodeValue


            Dim isbnObj = row.Cells("ISBN").Value

            Dim hasISBN As Boolean =
            isbnObj IsNot Nothing AndAlso
            Not IsDBNull(isbnObj) AndAlso
            Not String.IsNullOrWhiteSpace(
                isbnObj.ToString()
            )

            If Not hasISBN Then

                rbgenerate.Checked = True
                txtisbn.Enabled = False
                txtisbn.Text = ""

            Else

                rbgenerate.Checked = False
                txtisbn.Enabled = True
                txtisbn.Text = isbnObj.ToString()

            End If


            If Not String.IsNullOrEmpty(barcodeValue) AndAlso
           barcodeValue <> "0000000000000" Then

                picbarcode.Image =
                GenerateBarcodeImage(
                    barcodeValue,
                    picbarcode.Width,
                    picbarcode.Height
                )

            ElseIf hasISBN Then

                picbarcode.Image =
                GenerateBarcodeImage(
                    "0000000000000",
                    picbarcode.Width,
                    picbarcode.Height
                )

                lblrandom.Text = "0000000000000"

            Else

                picbarcode.Image = Nothing

            End If


        Catch ex As Exception

            Debug.WriteLine(
            "ApplyRowToFields error: " &
            ex.Message
        )

        Finally

            isbarcode = False

        End Try

    End Sub


    Private Sub DataGridView1_CellClick_1(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellClick

        If e.RowIndex < 0 Then Exit Sub

        If e.ColumnIndex < 0 Then Exit Sub

        If DataGridView1.Columns(e.ColumnIndex).Name = "Edit" OrElse
           DataGridView1.Columns(e.ColumnIndex).Name = "Delete" Then

            Exit Sub

        End If

        Try
            PauseAutoRefresh(DataGridView1)
        Catch
        End Try

        Try

            If cbauthor.DataSource Is Nothing OrElse cbauthor.Items.Count = 0 Then
                AutoRefreshComboBox(
                    cbauthor,
                    "SELECT ID, AuthorName FROM author_tbl ORDER BY AuthorName",
                    "AuthorName",
                    "ID"
                )
            End If

            If cbgenre.DataSource Is Nothing OrElse cbgenre.Items.Count = 0 Then
                AutoRefreshComboBox(
                    cbgenre,
                    "SELECT ID, Genre FROM genre_tbl ORDER BY Genre",
                    "Genre",
                    "ID"
                )
            End If

            If cbpublisher.DataSource Is Nothing OrElse cbpublisher.Items.Count = 0 Then
                AutoRefreshComboBox(
                    cbpublisher,
                    "SELECT ID, PublisherName FROM publisher_tbl ORDER BY PublisherName",
                    "PublisherName",
                    "ID"
                )
            End If

            If cblanguage.DataSource Is Nothing OrElse cblanguage.Items.Count = 0 Then
                AutoRefreshComboBox(
                    cblanguage,
                    "SELECT ID, Language FROM language_tbl ORDER BY Language",
                    "Language",
                    "ID"
                )
            End If

        Catch ex As Exception

            Debug.WriteLine(
                "Error auto-refreshing combos: " &
                ex.Message
            )

        End Try

        Dim row As DataGridViewRow =
            DataGridView1.Rows(e.RowIndex)

        Try

            Try

                Dim idObj = row.Cells("ID").Value

                If idObj IsNot Nothing AndAlso
                   Not IsDBNull(idObj) Then

                    LastSelectedBookID =
                        Convert.ToInt32(idObj)

                Else

                    LastSelectedBookID = -1

                End If

            Catch

                LastSelectedBookID = -1

            End Try

            skipRestore = True

            skipRestoreTimer.Stop()
            skipRestoreTimer.Start()

            ApplyRowToFields(row)

        Catch ex As Exception

            MessageBox.Show(
                "Error loading row data: " &
                ex.Message,
                "Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error
            )

        End Try

        If e.RowIndex >= 0 AndAlso
           Not DataGridView1.Rows(e.RowIndex).IsNewRow Then

            rbgenerate.Enabled = False

        End If

    End Sub


    Private Sub txtbooktitle_KeyPress(sender As Object, e As KeyPressEventArgs) Handles txtbooktitle.KeyPress

        If Char.IsLetter(e.KeyChar) Then
            e.Handled = False
            Return
        End If

        If Char.IsDigit(e.KeyChar) Then
            e.Handled = False
            Return
        End If

        If Char.IsControl(e.KeyChar) Then
            e.Handled = False
            Return
        End If

        If Char.IsWhiteSpace(e.KeyChar) Then
            e.Handled = False
            Return
        End If

        Select Case e.KeyChar

            Case "."c, ","c, "'"c, "-"c, ":"c, ";"c,
                 "("c, ")"c, "?"c, "!"c, "&"c, "/"c

                e.Handled = False
                Return

        End Select

        If e.KeyChar = Chr(34) Then

            e.Handled = False
            Return

        End If

        e.Handled = True

    End Sub

    Private Sub txtbooktitle_Validating(
        sender As Object,
        e As System.ComponentModel.CancelEventArgs
    ) Handles txtbooktitle.Validating

        Dim BookTitle As String =
            txtbooktitle.Text.Trim()

        Dim TitlePattern As String =
            "^(?=.*[a-zA-Z])(?!.*[.,'():;?!&/-]{2,})[a-zA-Z0-9\s.,'():;?!&/-]+$"

        If String.IsNullOrEmpty(BookTitle) Then

            e.Cancel = False
            Return

        End If

        If Not System.Text.RegularExpressions.Regex.IsMatch(
            BookTitle,
            TitlePattern,
            System.Text.RegularExpressions.RegexOptions.IgnoreCase) Then

            MessageBox.Show(
                "Invalid book title format.",
                "Validation Warning",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning
            )

            e.Cancel = True

        Else

            e.Cancel = False

        End If

    End Sub


    Private Sub btnaddauthor_Click(sender As Object, e As EventArgs)
        Author.ShowDialog()
    End Sub

    Private Sub btnaddgenre_Click(sender As Object, e As EventArgs)
        Genre.ShowDialog()
    End Sub

    Private Sub btnaddpublisher_Click(sender As Object, e As EventArgs)
        Publisher.ShowDialog()
    End Sub

    Private Sub btnaddlangauge_Click(sender As Object, e As EventArgs)
        Language.ShowDialog()
    End Sub

    Private Sub btnaddauthor_MouseHover(sender As Object, e As EventArgs)
        Cursor = Cursors.Hand
    End Sub

    Private Sub btnaddauthor_MouseLeave(sender As Object, e As EventArgs)
        Cursor = Cursors.Default
    End Sub

    Private Sub btnaddgenre_MouseHover(sender As Object, e As EventArgs)
        Cursor = Cursors.Hand
    End Sub

    Private Sub btnaddgenre_MouseLeave(sender As Object, e As EventArgs)
        Cursor = Cursors.Default
    End Sub

    Private Sub btnaddpublisher_MouseHover(sender As Object, e As EventArgs)
        Cursor = Cursors.Hand
    End Sub

    Private Sub btnaddpublisher_MouseLeave(sender As Object, e As EventArgs)
        Cursor = Cursors.Default
    End Sub

    Private Sub btnaddlangauge_MouseHover(sender As Object, e As EventArgs)
        Cursor = Cursors.Hand
    End Sub

    Private Sub btnaddlangauge_MouseLeave(sender As Object, e As EventArgs)
        Cursor = Cursors.Default
    End Sub

    Private Sub btnadd_MouseHover(sender As Object, e As EventArgs) Handles btnadd.MouseHover
        Cursor = Cursors.Hand
    End Sub

    Private Sub btnadd_MouseLeave(sender As Object, e As EventArgs) Handles btnadd.MouseLeave
        Cursor = Cursors.Default
    End Sub

    Private Sub btnedit_MouseHover(sender As Object, e As EventArgs)
        Cursor = Cursors.Hand
    End Sub

    Private Sub btnedit_MouseLeave(sender As Object, e As EventArgs)
        Cursor = Cursors.Default
    End Sub

    Private Sub btndelete_MouseHover(sender As Object, e As EventArgs)
        Cursor = Cursors.Hand
    End Sub

    Private Sub btndelete_MouseLeave(sender As Object, e As EventArgs)
        Cursor = Cursors.Default
    End Sub

    Private Sub btnclear_MouseHover(sender As Object, e As EventArgs) Handles btnclear.MouseHover
        Cursor = Cursors.Hand
    End Sub

    Private Sub btnclear_MouseLeave(sender As Object, e As EventArgs) Handles btnclear.MouseLeave
        Cursor = Cursors.Default
    End Sub


    Private Sub DisablePaste_AllTextBoxes()

        For Each ctrl As Control In Me.Controls
            AddHandlerToTextBoxes_NoPaste(ctrl)
        Next

    End Sub

    Private Sub AddHandlerToTextBoxes_NoPaste(parent As Control)

        For Each ctrl As Control In parent.Controls

            If TypeOf ctrl Is TextBox Then

                Dim tb As TextBox =
                    CType(ctrl, TextBox)

                tb.ContextMenuStrip =
                    New ContextMenuStrip()

                AddHandler tb.KeyDown,
                    AddressOf BlockPasteKey

                AddHandler tb.MouseUp,
                    AddressOf BlockRightClick

            End If

            If ctrl.HasChildren Then
                AddHandlerToTextBoxes_NoPaste(ctrl)
            End If

        Next

    End Sub


    Private Sub BlockPasteKey(
        sender As Object,
        e As KeyEventArgs
    )

        If (e.Control AndAlso e.KeyCode = Keys.V) OrElse
           (e.Shift AndAlso e.KeyCode = Keys.Insert) Then

            e.SuppressKeyPress = True

        End If

    End Sub

    Private Sub BlockRightClick(
        sender As Object,
        e As MouseEventArgs
    )

        If e.Button = MouseButtons.Right Then

            Dim tb As TextBox =
                TryCast(sender, TextBox)

            If tb IsNot Nothing Then
                tb.ContextMenuStrip =
                    New ContextMenuStrip()
            End If

        End If

    End Sub

    Private Sub txtyearr_KeyDown(
        sender As Object,
        e As KeyEventArgs
    ) Handles txtyearr.KeyDown

        If e.Control AndAlso
           (e.KeyCode = Keys.V Or e.KeyCode = Keys.C Or e.KeyCode = Keys.X) Then

            e.SuppressKeyPress = True

        End If

    End Sub

    Private Sub txtyearr_KeyPress(
        sender As Object,
        e As KeyPressEventArgs
    ) Handles txtyearr.KeyPress

        If Not Char.IsDigit(e.KeyChar) And
           Not Char.IsControl(e.KeyChar) Then

            e.Handled = True

        End If

    End Sub

    Private Sub txtyearr_Validating(
        sender As Object,
        e As System.ComponentModel.CancelEventArgs
    ) Handles txtyearr.Validating

        If String.IsNullOrWhiteSpace(txtyearr.Text) Then
            Exit Sub
        End If

        Dim yearValue As Integer

        If Not Integer.TryParse(
            txtyearr.Text,
            yearValue) Then

            MessageBox.Show(
                "Please enter a valid year.",
                "Invalid Input",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning
            )

            txtyearr.Clear()
            e.Cancel = True
            Exit Sub

        End If

        Dim currentYear As Integer =
            Date.Now.Year

        If yearValue > currentYear Then

            MessageBox.Show(
                "Year Published cannot be in the future. Please enter a valid year (up to " &
                currentYear &
                ").",
                "Invalid Year",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning
            )

            txtyearr.Text =
                currentYear.ToString()

            e.Cancel = True

        End If

    End Sub

    Private Sub cbgenre_DropDown(
        sender As Object,
        e As EventArgs
    ) Handles cbgenre.DropDown

        Try

            cbgenre.DataSource = Nothing
            cbgenree()

        Catch ex As Exception

            Debug.WriteLine(
                "Error refreshing genre combo: " &
                ex.Message
            )

        End Try

    End Sub

    Private Sub cbauthor_DropDown(
        sender As Object,
        e As EventArgs
    ) Handles cbauthor.DropDown

        Try

            cbauthor.DataSource = Nothing
            cbauthorr()

        Catch ex As Exception

            Debug.WriteLine(
                "Error refreshing author combo: " &
                ex.Message
            )

        End Try

    End Sub

    Private Sub cbpublisher_DropDown(
        sender As Object,
        e As EventArgs
    ) Handles cbpublisher.DropDown

        Try

            cbpublisher.DataSource = Nothing
            cbpublisherr()

        Catch ex As Exception

            Debug.WriteLine(
                "Error refreshing publisher combo: " &
                ex.Message
            )

        End Try

    End Sub

    Private Sub cblanguage_DropDown(
        sender As Object,
        e As EventArgs
    ) Handles cblanguage.DropDown

        Try

            cblanguage.DataSource = Nothing
            cblang()

        Catch ex As Exception

            Debug.WriteLine(
                "Error refreshing language combo: " &
                ex.Message
            )

        End Try

    End Sub

    Private Sub DataGridView1_MouseHover(
        sender As Object,
        e As EventArgs
    ) Handles DataGridView1.MouseHover

        PauseAutoRefresh(DataGridView1)

    End Sub

    Private Sub datagridview1_MouseLeave(
        sender As Object,
        e As EventArgs
    ) Handles DataGridView1.MouseLeave

        ResumeAutoRefresh(DataGridView1)

    End Sub

    Private Sub chkauthor_CheckedChanged(
        sender As Object,
        e As EventArgs
    ) Handles chkauthor.CheckedChanged

        Try

            If chkauthor.Checked Then

                cbauthor.SelectedIndex = -1
                cbauthor.Enabled = False

            Else

                cbauthor.Enabled = True

            End If

        Catch ex As Exception

            Debug.WriteLine(
                "chkauthor_CheckedChanged error: " &
                ex.Message
            )

        End Try

    End Sub

End Class