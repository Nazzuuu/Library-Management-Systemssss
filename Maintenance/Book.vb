Imports System.Drawing
Imports System.IO
Imports MySql.Data.MySqlClient
Imports ZXing
Imports ZXing.Rendering
Imports ZXing.Windows.Compatibility
Imports System.Drawing.Printing
Imports System.Collections.Generic
Imports System.Data

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


    Private Sub Book_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        TopMost = True
        Me.Font = New Font("Baskerville Old Face", 9)
        Me.Refresh()

        refreshbook()
        DisablePaste_AllTextBoxes()

        AddHandler cbauthor.DropDown, AddressOf RefreshComboBoxes
        AddHandler cbgenre.DropDown, AddressOf RefreshComboBoxes
        AddHandler cbpublisher.DropDown, AddressOf RefreshComboBoxes
        AddHandler cblanguage.DropDown, AddressOf RefreshComboBoxes

        AddHandler GlobalVarsModule.DatabaseUpdated, AddressOf OnDatabaseUpdated
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


    Private Async Sub OnDatabaseUpdated()
        Dim query As String = "SELECT * FROM `book_tbl` ORDER BY ID DESC"
        Await GlobalVarsModule.LoadToGridAsync(DataGridView1, query)
        SetupGridStyle()


        Try
            AutoRefreshComboBox(cbauthor, "SELECT ID, AuthorName FROM author_tbl", "AuthorName", "ID")
            AutoRefreshComboBox(cbgenre, "SELECT ID, GenreName FROM genre_tbl", "GenreName", "ID")
            AutoRefreshComboBox(cbpublisher, "SELECT ID, PublisherName FROM publisher_tbl", "PublisherName", "ID")
            AutoRefreshComboBox(cblanguage, "SELECT ID, LanguageName FROM language_tbl", "LanguageName", "ID")
        Catch ex As Exception
            Debug.WriteLine("ComboBox refresh failed: " & ex.Message)
        End Try

    End Sub


    Private Sub SetupGridStyle()
        Try
            If DataGridView1.Columns.Contains("ID") Then
                DataGridView1.Columns("ID").Visible = False
            End If

            DataGridView1.ClearSelection()
            DataGridView1.CurrentCell = Nothing
            DataGridView1.EnableHeadersVisualStyles = False
            DataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(207, 58, 109)
            DataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White
            DataGridView1.ReadOnly = True
        Catch
        End Try
    End Sub


    Public Sub cbauthorr()

        Dim con As New MySqlConnection(connectionString)
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

        Dim con As New MySqlConnection(connectionString)
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

        Dim con As New MySqlConnection(connectionString)
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

        Dim con As New MySqlConnection(connectionString)
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
    End Sub




    Private Sub PrintDocument_PrintPage(sender As Object, e As Printing.PrintPageEventArgs) Handles printDoc.PrintPage

        Dim g As Graphics = e.Graphics

        Dim TitleFont As New Font("Arial", 8, FontStyle.Bold)
        Dim TitleBrush As New SolidBrush(Color.Black)
        Dim TitleLineHeight As Integer = CInt(TitleFont.GetHeight(g)) + 2


        Dim TOTAL_ITEM_HEIGHT As Integer = BARCODE_HEIGHT_HM + TitleLineHeight
        Dim TOTAL_ROW_WIDTH As Integer = (BARCODE_WIDTH_HM * 2) + HORIZONTAL_SPACING_HM
        Dim CENTER_PADDING_X As Integer = CInt((e.MarginBounds.Width - TOTAL_ROW_WIDTH) / 2)


        Dim x_start As Integer = e.MarginBounds.Left + CENTER_PADDING_X
        Dim y_start As Integer = e.MarginBounds.Top

        Dim x_pos As Integer = x_start
        Dim y_pos As Integer = y_start

        Dim barcodesPrintedOnPage As Integer = 0


        While BarcodeIndex < BarcodeList.Count


            Dim currentBarcodeInfo As BarcodeInfo = BarcodeList(BarcodeIndex)
            Dim barcodeText As String = currentBarcodeInfo.Barcode
            Dim bookTitle As String = currentBarcodeInfo.Title


            If y_pos + TOTAL_ITEM_HEIGHT > e.MarginBounds.Bottom Then



                If x_pos = x_start Then


                    x_pos = x_start + BARCODE_WIDTH_HM + HORIZONTAL_SPACING_HM
                    y_pos = y_start

                    If y_pos + TOTAL_ITEM_HEIGHT > e.MarginBounds.Bottom Then

                        e.HasMorePages = True
                        Exit Sub
                    End If

                Else

                    e.HasMorePages = True
                    Exit Sub
                End If
            End If

            Using barcodeImage As Image = GenerateBarcodeImage(barcodeText, BARCODE_PIXEL_WIDTH, BARCODE_PIXEL_HEIGHT)

                If barcodeImage Is Nothing Then
                    BarcodeIndex += 1
                    Continue While
                End If


                g.DrawString(bookTitle, TitleFont, TitleBrush, x_pos, y_pos)
                g.DrawImage(barcodeImage, x_pos, y_pos + TitleLineHeight, BARCODE_WIDTH_HM, BARCODE_HEIGHT_HM)

            End Using


            BarcodeIndex += 1
            barcodesPrintedOnPage += 1


            y_pos += TOTAL_ITEM_HEIGHT + VERTICAL_SPACING_HM



            If barcodesPrintedOnPage Mod MAX_ROWS_PER_COLUMN = 0 AndAlso x_pos = x_start Then

                x_pos = x_start + BARCODE_WIDTH_HM + HORIZONTAL_SPACING_HM
                y_pos = y_start

            End If


        End While


        TitleFont.Dispose()
        TitleBrush.Dispose()


        If BarcodeIndex < BarcodeList.Count Then
            e.HasMorePages = True
        Else
            e.HasMorePages = False
            BarcodeIndex = 0
        End If

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

        Dim con As New MySqlConnection(GlobalVarsModule.connectionString)
        Dim isbn As Object = Nothing
        Dim barcode As Object = Nothing
        Dim booktitle As String = txtbooktitle.Text.Trim()
        Dim yearsu As String = txtyearr.Text.Trim
        Dim bookID As Integer = 0

        If rbgenerate.Checked Then
            isbn = DBNull.Value
            barcode = lblrandom.Text.Trim()


            Dim coms As New MySqlCommand("SELECT COUNT(*) FROM `book_tbl` WHERE `Barcode` = @barcode", con)
            coms.Parameters.AddWithValue("@barcode", barcode)
            Try
                con.Open()
                Dim count As Integer = CInt(coms.ExecuteScalar())
                If count > 0 Then
                    MsgBox("This book already exists.", vbExclamation, "Duplication not allowed.")
                    Exit Sub
                End If
            Catch ex As Exception
                MsgBox("Error checking Barcode: " & ex.Message, vbCritical)
                Exit Sub
            Finally
                If con.State = ConnectionState.Open Then
                    con.Close()
                End If
            End Try

        Else
            isbn = txtisbn.Text.Trim()

            If String.IsNullOrEmpty(CStr(isbn)) Then
                MsgBox("Please enter a valid ISBN.", vbExclamation, "Validation Error")
                Exit Sub
            End If

            If String.IsNullOrEmpty(CStr(isbn)) Then
                MsgBox("Please enter a valid ISBN.", vbExclamation, "Validation Error")
                Exit Sub
            End If


            Dim isbnString As String = CStr(isbn).Replace("-", "").Replace(" ", "")


            If Not System.Text.RegularExpressions.Regex.IsMatch(isbnString, "^(978|979)\d{10}$") Then

                MsgBox("ISBN must start with '978' or '979'.", vbExclamation, "ISBN Format Error")
                Exit Sub
            End If

            Dim coms As New MySqlCommand("SELECT COUNT(*) FROM `book_tbl` WHERE `ISBN` = @isbn", con)
            coms.Parameters.AddWithValue("@isbn", isbn)
            Try
                con.Open()
                Dim count As Integer = CInt(coms.ExecuteScalar())
                If count > 0 Then
                    MsgBox("The ISBN already exists. Please enter a unique ISBN.", vbExclamation, "Duplication not allowed.")
                    Exit Sub
                End If
            Catch ex As Exception
                MsgBox("Error checking ISBN: " & ex.Message, vbCritical)
                Exit Sub
            Finally
                If con.State = ConnectionState.Open Then
                    con.Close()
                End If
            End Try

            barcode = DBNull.Value
        End If

        If String.IsNullOrEmpty(booktitle) OrElse cbauthor.SelectedIndex = -1 OrElse cbgenre.SelectedIndex = -1 OrElse cbpublisher.SelectedIndex = -1 OrElse cblanguage.SelectedIndex = -1 Then
            MsgBox("Please fill in all the required fields.", vbExclamation, "Validation Error")
            Exit Sub
        End If




        Try
            con.Open()

            Dim com As New MySqlCommand("INSERT INTO `book_tbl`(`Barcode`,`ISBN`, `BookTitle`, `Author`, `Genre`, `Publisher`, `Language`, `YearPublished`) VALUES (@barcode, @isbn, @booktitle, @author, @genre, @publisher, @language, @yearpublished); SELECT LAST_INSERT_ID()", con)

            com.Parameters.AddWithValue("@barcode", If(IsDBNull(barcode), DBNull.Value, barcode))
            com.Parameters.AddWithValue("@isbn", If(IsDBNull(isbn), DBNull.Value, isbn))
            com.Parameters.AddWithValue("@booktitle", booktitle)
            com.Parameters.AddWithValue("@author", cbauthor.Text)
            com.Parameters.AddWithValue("@genre", cbgenre.Text)
            com.Parameters.AddWithValue("@publisher", cbpublisher.Text)
            com.Parameters.AddWithValue("@language", cblanguage.Text)
            com.Parameters.AddWithValue("@yearpublished", yearsu)

            bookID = Convert.ToInt32(com.ExecuteScalar())

            GlobalVarsModule.LogAudit(
            actionType:="ADD",
            formName:="BOOK FORM",
            description:=$"Added new Book: {booktitle}",
            recordID:=bookID.ToString()
        )

            For Each form In Application.OpenForms
                If TypeOf form Is AuditTrail Then
                    Dim load = DirectCast(form, AuditTrail)
                    load.refreshaudit()
                End If
            Next

            MsgBox("Book added successfully", vbInformation)
            clear()
            LoadBookData()
        Catch ex As Exception
            MsgBox(ex.Message, vbCritical)
        Finally
            If Not IsNothing(con) AndAlso con.State = ConnectionState.Open Then
                con.Close()
            End If
        End Try
    End Sub

    Private Sub btnedit_Click(sender As Object, e As EventArgs) Handles btnedit.Click

        If DataGridView1.SelectedRows.Count > 0 Then
            Dim selectedRow As DataGridViewRow = DataGridView1.SelectedRows(0)
            Dim bookID As Integer = CInt(selectedRow.Cells("ID").Value)
            Dim oldBookTitle As String = selectedRow.Cells("BookTitle").Value.ToString()
            Dim newBookTitle As String = txtbooktitle.Text.Trim()
            Dim yearsu As String = txtyearr.Text.Trim

            If String.IsNullOrEmpty(newBookTitle) OrElse cbauthor.SelectedIndex = -1 OrElse cbgenre.SelectedIndex = -1 OrElse cbpublisher.SelectedIndex = -1 OrElse cblanguage.SelectedIndex = -1 Then
                MsgBox("Please fill in all the required fields.", vbExclamation, "Validation Error")
                Exit Sub
            End If


            Dim con As New MySqlConnection(GlobalVarsModule.connectionString)


            Dim isbnValue As Object
            Dim barcodeValue As Object

            If rbgenerate.Checked Then
                isbnValue = DBNull.Value
                barcodeValue = lblrandom.Text.Trim()
                Dim coms As New MySqlCommand("SELECT COUNT(*) FROM `book_tbl` WHERE `Barcode` = @barcode AND `ID` <> @id", con)
                coms.Parameters.AddWithValue("@barcode", barcodeValue)
                coms.Parameters.AddWithValue("@id", bookID)

                Try
                    con.Open()
                    Dim count As Integer = CInt(coms.ExecuteScalar())
                    If count > 0 Then
                        MsgBox("This book already exists.", vbExclamation, "Duplication not allowed.")
                        Exit Sub
                    End If
                Catch ex As Exception
                    MsgBox("Error checking Barcode: " & ex.Message, vbCritical)
                    Exit Sub
                Finally
                    If con.State = ConnectionState.Open Then
                        con.Close()
                    End If
                End Try

            Else
                isbnValue = txtisbn.Text.Trim()
                If String.IsNullOrEmpty(CStr(isbnValue)) Then
                    MsgBox("Please enter a valid ISBN.", vbExclamation, "Validation Error")
                    Exit Sub
                End If

                If String.IsNullOrEmpty(CStr(isbnValue)) Then
                    MsgBox("Please enter a valid ISBN.", vbExclamation, "Validation Error")
                    Exit Sub
                End If


                Dim isbnString As String = CStr(isbnValue).Replace("-", "").Replace(" ", "")


                If Not System.Text.RegularExpressions.Regex.IsMatch(isbnString, "^(978|979)\d{10}$") Then

                    MsgBox("ISBN must start with '978' or '979'.", vbExclamation, "ISBN Format Error")
                    Exit Sub
                End If

                Dim coms As New MySqlCommand("SELECT COUNT(*) FROM `book_tbl` WHERE `ISBN` = @isbn AND `ID` <> @id", con)
                coms.Parameters.AddWithValue("@isbn", isbnValue)
                coms.Parameters.AddWithValue("@id", bookID)
                Try
                    con.Open()
                    Dim count As Integer = CInt(coms.ExecuteScalar())
                    If count > 0 Then
                        MsgBox("The ISBN already exists. Please enter a unique ISBN.", vbExclamation, "Duplication not allowed.")
                        Exit Sub
                    End If
                Catch ex As Exception
                    MsgBox("Error checking ISBN: " & ex.Message, vbCritical)
                    Exit Sub
                Finally
                    If con.State = ConnectionState.Open Then
                        con.Close()
                    End If
                End Try
                barcodeValue = DBNull.Value
            End If



            Try
                con.Open()

                Dim com As New MySqlCommand("UPDATE `book_tbl` SET `Barcode` = @barcode, `ISBN`=@isbn, `BookTitle`= @booktitle, `Author`= @author, `Genre`= @genre, `Publisher`= @publisher, `Language`= @language, `YearPublished`= @yearpublished WHERE `ID` = @id", con)
                com.Parameters.AddWithValue("@barcode", If(IsDBNull(barcodeValue), DBNull.Value, barcodeValue))
                com.Parameters.AddWithValue("@isbn", If(IsDBNull(isbnValue), DBNull.Value, isbnValue))
                com.Parameters.AddWithValue("@booktitle", newBookTitle)
                com.Parameters.AddWithValue("@author", cbauthor.Text)
                com.Parameters.AddWithValue("@genre", cbgenre.Text)

                com.Parameters.AddWithValue("@publisher", cbpublisher.Text)
                com.Parameters.AddWithValue("@language", cblanguage.Text)
                com.Parameters.AddWithValue("@yearpublished", yearsu)
                com.Parameters.AddWithValue("@id", bookID)
                com.ExecuteNonQuery()

                GlobalVarsModule.LogAudit(
                actionType:="UPDATE",
                formName:="BOOK FORM",
                description:=$"Updated Book Title from '{oldBookTitle}' to '{newBookTitle}' (ID: {bookID})",
                recordID:=bookID.ToString(),
                oldValue:=oldBookTitle,
                newValue:=newBookTitle
            )

                Dim comAcquisition As New MySqlCommand("UPDATE `acquisition_tbl` SET `BookTitle` = @newBookTitle WHERE `BookTitle` = @oldBookTitle", con)
                comAcquisition.Parameters.AddWithValue("@newBookTitle", newBookTitle)
                comAcquisition.Parameters.AddWithValue("@oldBookTitle", oldBookTitle)
                comAcquisition.ExecuteNonQuery()

                Dim comAcession As New MySqlCommand("UPDATE `acession_tbl` SET `BookTitle` = @newBookTitle WHERE `BookTitle` = @oldBookTitle", con)
                comAcession.Parameters.AddWithValue("@newBookTitle", newBookTitle)
                comAcession.Parameters.AddWithValue("@oldBookTitle", oldBookTitle)
                comAcession.ExecuteNonQuery()

                Dim comBorrowing As New MySqlCommand("UPDATE `borrowing_tbl` SET `BookTitle` = @newBookTitle WHERE `BookTitle` = @oldBookTitle", con)
                comBorrowing.Parameters.AddWithValue("@newBookTitle", newBookTitle)
                comBorrowing.Parameters.AddWithValue("@oldBookTitle", oldBookTitle)
                comBorrowing.ExecuteNonQuery()

                Dim comReserve As New MySqlCommand("UPDATE `reservecopiess_tbl` SET `BookTitle` = @newBookTitle WHERE `BookTitle` = @oldBookTitle", con)
                comReserve.Parameters.AddWithValue("@newBookTitle", newBookTitle)
                comReserve.Parameters.AddWithValue("@oldBookTitle", oldBookTitle)
                comReserve.ExecuteNonQuery()

                Dim availsus As New MySqlCommand("UPDATE `available_tbl` SET `BookTitle` = @newBookTitle WHERE `BookTitle` = @oldBookTitle", con)
                availsus.Parameters.AddWithValue("@newBookTitle", newBookTitle)
                availsus.Parameters.AddWithValue("@oldBookTitle", oldBookTitle)
                availsus.ExecuteNonQuery()

                For Each form In Application.OpenForms

                    If TypeOf form Is Acquisition Then
                        DirectCast(form, Acquisition).refreshData()
                    End If

                    If TypeOf form Is Accession Then
                        DirectCast(form, Accession).RefreshAccessionData()
                    End If

                    If TypeOf form Is ReserveCopies Then
                        DirectCast(form, ReserveCopies).reserveload()
                    End If

                    If TypeOf form Is Borrowing Then
                        DirectCast(form, Borrowing).refreshborrowingsu()
                    End If

                    If TypeOf form Is AuditTrail Then
                        Dim load = DirectCast(form, AuditTrail)
                        load.refreshaudit()
                    End If

                    If TypeOf form Is AvailableBooks Then
                        Dim load = DirectCast(form, AvailableBooks)
                        load.refreshavail()
                    End If
                Next

                MsgBox("Book updated successfully", vbInformation)
                clear()
                LoadBookData()
                AvailableBooks.refreshavail()

            Catch ex As Exception
                MsgBox(ex.Message, vbCritical)
            Finally
                If Not IsNothing(con) AndAlso con.State = ConnectionState.Open Then
                    con.Close()
                End If
            End Try
        Else
            MsgBox("Please select a row to edit.", vbExclamation)
        End If
    End Sub

    Private Sub btndelete_Click(sender As Object, e As EventArgs) Handles btndelete.Click

        If DataGridView1.SelectedRows.Count > 0 Then

            Dim dialogResult As DialogResult = MessageBox.Show("Are you sure you want to delete this book?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)

            If dialogResult = DialogResult.Yes Then

                Dim con As New MySqlConnection(GlobalVarsModule.connectionString)
                Dim selectedRow As DataGridViewRow = DataGridView1.SelectedRows(0)

                Dim bookID As Integer = CInt(selectedRow.Cells("ID").Value)
                Dim bookTitle As String = selectedRow.Cells("BookTitle").Value.ToString()

                Dim bookISBN As String = selectedRow.Cells("ISBN").Value.ToString()
                Dim bookBarcode As String = selectedRow.Cells("Barcode").Value.ToString()

                Try
                    con.Open()


                    Dim acs As New MySqlCommand("SELECT COUNT(*) FROM `acession_tbl` WHERE `ISBN` = @ISBN OR `Barcode` = @Barcode", con)
                    acs.Parameters.AddWithValue("@ISBN", bookISBN)
                    acs.Parameters.AddWithValue("@Barcode", bookBarcode)

                    Dim countss As Integer = CInt(acs.ExecuteScalar())

                    If countss > 0 Then
                        MsgBox("Cannot delete this book. It has existing accession records.", vbExclamation, "Deletion Blocked")
                        Exit Sub
                    End If


                    Dim acx As New MySqlCommand("SELECT COUNT(*) FROM `acquisition_tbl` WHERE `ISBN` = @ISBN OR `Barcode` = @Barcode", con)
                    acx.Parameters.AddWithValue("@ISBN", bookISBN)
                    acx.Parameters.AddWithValue("@Barcode", bookBarcode)

                    Dim countsx As Integer = CInt(acx.ExecuteScalar())

                    If countsx > 0 Then
                        MsgBox("Cannot delete this book. It has existing acquisition records.", vbExclamation, "Deletion Blocked")
                        Exit Sub
                    End If

                    Dim delete As New MySqlCommand("DELETE FROM `book_tbl` WHERE `ID` = @id", con)
                    delete.Parameters.AddWithValue("@id", bookID)
                    delete.ExecuteNonQuery()

                    GlobalVarsModule.LogAudit(
                    actionType:="DELETE",
                    formName:="BOOK FORM",
                    description:=$"Deleted Book: {bookTitle}",
                    recordID:=bookID.ToString()
                )

                    For Each form In Application.OpenForms
                        If TypeOf form Is AuditTrail Then
                            Dim load = DirectCast(form, AuditTrail)
                            load.refreshaudit()
                        End If
                    Next

                    MsgBox("Book deleted successfully.", vbInformation)


                    LoadBookData()
                    clear()

                    Dim count As New MySqlCommand("SELECT COUNT(*) FROM `book_tbl`", con)
                    Dim rowCount As Long = CLng(count.ExecuteScalar())

                    If rowCount = 0 Then
                        Dim reset As New MySqlCommand("ALTER TABLE `book_tbl` AUTO_INCREMENT = 1", con)
                        reset.ExecuteNonQuery()
                    End If

                Catch ex As Exception
                    MsgBox("An error occurred: " & ex.Message, vbCritical)
                Finally
                    If con.State = ConnectionState.Open Then
                        con.Close()
                    End If
                End Try
            End If
        Else
            MsgBox("Please select a row to delete.", vbExclamation)
        End If
    End Sub

    Private Sub Printbarcode(sender As Object, e As EventArgs) Handles btnprint.Click

        BarcodeList.Clear()
        BarcodeIndex = 0

        For Each row As DataGridViewRow In DataGridView1.Rows
            If Not row.IsNewRow Then
                Dim barcodeValue As String = If(IsDBNull(row.Cells("Barcode").Value), String.Empty, CStr(row.Cells("Barcode").Value))
                Dim bookTitleValue As String = If(IsDBNull(row.Cells("BookTitle").Value), "(No Title)", CStr(row.Cells("BookTitle").Value))

                If Not String.IsNullOrEmpty(barcodeValue) AndAlso barcodeValue <> "0000000000000" Then
                    BarcodeList.Add(New BarcodeInfo With {.Barcode = barcodeValue, .Title = bookTitleValue})
                End If
            End If
        Next

        If BarcodeList.Count = 0 Then
            MessageBox.Show("No barcodes found.", "Print Error", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Exit Sub
        End If

        Try
            Using pdlg As New PrintDialog With {.Document = printDoc}
                printDoc.DefaultPageSettings.Landscape = False

                If pdlg.ShowDialog() = DialogResult.OK Then

                    Dim selectedPrinterName As String = pdlg.PrinterSettings.PrinterName.ToLower()

                    If Not (selectedPrinterName.Contains("microsoft") OrElse selectedPrinterName.Contains("epson")) Then
                        MessageBox.Show("Printer is not compatible. Please use a Microsoft or Epson printer only.", "Printer Not Allowed", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        Exit Sub
                    End If

                    printDoc.PrinterSettings = pdlg.PrinterSettings
                    printDoc.Print()

                    MessageBox.Show($"Successfully sent {BarcodeList.Count} barcode labels to '{pdlg.PrinterSettings.PrinterName}'.", "Print Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Else
                    Exit Sub
                End If
            End Using

        Catch ex As System.Drawing.Printing.InvalidPrinterException
            Dim currentPrinterName As String = If(printDoc.PrinterSettings Is Nothing, "Selected Printer", printDoc.PrinterSettings.PrinterName)
            MessageBox.Show($"Warning: The selected printer ('{currentPrinterName}') is not connected or ready. Please check the printer connection.", "Printer Not Ready", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        Catch ex As Exception
            MessageBox.Show("An unexpected error occurred during the print job: " & ex.Message, "Print Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            BarcodeIndex = 0
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
    End Sub




    Function jinireyt() As String
        Dim random As New Random()
        Dim barcode As String = ""
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
                Dim filter As String = String.Format("BookTitle LIKE '*{0}*' OR ISBN LIKE '*{0}*'", txtsearch.Text.Trim())
                dt.DefaultView.RowFilter = filter
            Else
                dt.DefaultView.RowFilter = ""
            End If
        End If
    End Sub


    Private Sub txtisbn_KeyDown(sender As Object, e As KeyEventArgs) Handles txtisbn.KeyDown
        If e.Control AndAlso (e.KeyCode = Keys.V Or e.KeyCode = Keys.C Or e.KeyCode = Keys.X) Then
            e.SuppressKeyPress = True
        End If
    End Sub

    Private Sub txtisbn_KeyPress(sender As Object, e As KeyPressEventArgs) Handles txtisbn.KeyPress
        If Not Char.IsDigit(e.KeyChar) And Not Char.IsControl(e.KeyChar) Then
            e.Handled = True
        End If
    End Sub

    Private Sub txtbooktitle_KeyDown(sender As Object, e As KeyEventArgs) Handles txtbooktitle.KeyDown
        If e.Control AndAlso (e.KeyCode = Keys.V Or e.KeyCode = Keys.C Or e.KeyCode = Keys.X) Then
            e.SuppressKeyPress = True
        End If
    End Sub

    Private Sub txtsearch_KeyDown(sender As Object, e As KeyEventArgs) Handles txtsearch.KeyDown
        If e.Control AndAlso (e.KeyCode = Keys.V Or e.KeyCode = Keys.C Or e.KeyCode = Keys.X) Then
            e.SuppressKeyPress = True
        End If
    End Sub

    Private Sub jinreytsu()
        If rbgenerate.Checked Then
            Dim newBarcode As String = jinireyt()
            lblrandom.Text = newBarcode
            picbarcode.Image = GenerateBarcodeImage(newBarcode, picbarcode.Width, picbarcode.Height)
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
        End If
    End Sub

    Private Sub DataGridView1_CellClick_1(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellClick

        If e.RowIndex >= 0 AndAlso Not DataGridView1.Rows(e.RowIndex).IsNewRow Then

            isbarcode = True

            Dim row = DataGridView1.Rows(e.RowIndex)

            Dim barcodeValue As String = row.Cells("Barcode").Value.ToString
            lblrandom.Text = barcodeValue

            Dim isbnn = row.Cells("ISBN").Value


            Dim isbnonleh As Boolean = Not IsDBNull(isbnn) AndAlso Not String.IsNullOrEmpty(isbnn.ToString)

            If Not isbnonleh Then
                rbgenerate.Checked = True
                txtisbn.Enabled = False
                txtisbn.Text = ""
            Else
                rbgenerate.Checked = False
                txtisbn.Enabled = True
                txtisbn.Text = isbnn.ToString
            End If

            If Not String.IsNullOrEmpty(barcodeValue) AndAlso barcodeValue <> "0000000000000" Then

                picbarcode.Image = GenerateBarcodeImage(barcodeValue, picbarcode.Width, picbarcode.Height)
            ElseIf isbnonleh AndAlso (String.IsNullOrEmpty(barcodeValue) OrElse barcodeValue = "0000000000000") Then

                picbarcode.Image = GenerateBarcodeImage("0000000000000", picbarcode.Width, picbarcode.Height)

                lblrandom.Text = "0000000000000"
            Else

                picbarcode.Image = Nothing
            End If

            txtbooktitle.Text = row.Cells("BookTitle").Value.ToString
            cbauthor.Text = row.Cells("Author").Value.ToString
            cbgenre.Text = row.Cells("Genre").Value.ToString
            txtyearr.Text = row.Cells("YearPublished").Value.ToString
            cbpublisher.Text = row.Cells("Publisher").Value.ToString
            cblanguage.Text = row.Cells("Language").Value.ToString


            rbgenerate.Enabled = False

            isbarcode = False

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
            Case "."c, ","c, "'"c, "-"c, ":"c, ";"c, "("c, ")"c, "?"c, "!"c, "&"c, "/"c
                e.Handled = False
                Return
        End Select

        If e.KeyChar = Chr(34) Then
            e.Handled = False
            Return
        End If

        e.Handled = True
    End Sub

    Private Sub txtbooktitle_Validating(sender As Object, e As System.ComponentModel.CancelEventArgs) Handles txtbooktitle.Validating

        Dim BookTitle As String = txtbooktitle.Text.Trim()
        Dim TitlePattern As String = "^(?=.*[a-zA-Z])(?!.*[.,'():;?!&/-]{2,})[a-zA-Z0-9\s.,'():;?!&/-]+$"

        If String.IsNullOrEmpty(BookTitle) Then
            e.Cancel = False
            Return
        End If

        If Not System.Text.RegularExpressions.Regex.IsMatch(BookTitle, TitlePattern, System.Text.RegularExpressions.RegexOptions.IgnoreCase) Then
            MessageBox.Show("Invalid book title format.", "Validation Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            e.Cancel = True
        Else
            e.Cancel = False
        End If
    End Sub


    Private Sub btnaddauthor_Click(sender As Object, e As EventArgs) Handles btnaddauthor.Click
        Author.ShowDialog()
    End Sub

    Private Sub btnaddgenre_Click(sender As Object, e As EventArgs) Handles btnaddgenre.Click
        Genre.ShowDialog()
    End Sub

    Private Sub btnaddpublisher_Click(sender As Object, e As EventArgs) Handles btnaddpublisher.Click
        Publisher.ShowDialog()
    End Sub

    Private Sub btnaddlangauge_Click(sender As Object, e As EventArgs) Handles btnaddlangauge.Click
        Language.ShowDialog()
    End Sub

    Private Sub btnaddauthor_MouseHover(sender As Object, e As EventArgs) Handles btnaddauthor.MouseHover
        Cursor = Cursors.Hand
    End Sub

    Private Sub btnaddauthor_MouseLeave(sender As Object, e As EventArgs) Handles btnaddauthor.MouseLeave
        Cursor = Cursors.Default
    End Sub

    Private Sub btnaddgenre_MouseHover(sender As Object, e As EventArgs) Handles btnaddgenre.MouseHover
        Cursor = Cursors.Hand
    End Sub

    Private Sub btnaddgenre_MouseLeave(sender As Object, e As EventArgs) Handles btnaddgenre.MouseLeave
        Cursor = Cursors.Default
    End Sub

    Private Sub btnaddpublisher_MouseHover(sender As Object, e As EventArgs) Handles btnaddpublisher.MouseHover
        Cursor = Cursors.Hand
    End Sub

    Private Sub btnaddpublisher_MouseLeave(sender As Object, e As EventArgs) Handles btnaddpublisher.MouseLeave
        Cursor = Cursors.Default
    End Sub



    Private Sub btnaddlangauge_MouseHover(sender As Object, e As EventArgs) Handles btnaddlangauge.MouseHover
        Cursor = Cursors.Hand
    End Sub

    Private Sub btnaddlangauge_MouseLeave(sender As Object, e As EventArgs) Handles btnaddlangauge.MouseLeave
        Cursor = Cursors.Default
    End Sub

    Private Sub btnadd_MouseHover(sender As Object, e As EventArgs) Handles btnadd.MouseHover
        Cursor = Cursors.Hand
    End Sub

    Private Sub btnadd_MouseLeave(sender As Object, e As EventArgs) Handles btnadd.MouseLeave
        Cursor = Cursors.Default
    End Sub

    Private Sub btnedit_MouseHover(sender As Object, e As EventArgs) Handles btnedit.MouseHover
        Cursor = Cursors.Hand
    End Sub

    Private Sub btnedit_MouseLeave(sender As Object, e As EventArgs) Handles btnedit.MouseLeave
        Cursor = Cursors.Default
    End Sub

    Private Sub btndelete_MouseHover(sender As Object, e As EventArgs) Handles btndelete.MouseHover
        Cursor = Cursors.Hand
    End Sub

    Private Sub btndelete_MouseLeave(sender As Object, e As EventArgs) Handles btndelete.MouseLeave
        Cursor = Cursors.Default
    End Sub

    Private Sub btnclear_MouseHover(sender As Object, e As EventArgs) Handles btnclear.MouseHover
        Cursor = Cursors.Hand
    End Sub

    Private Sub btnclear_MouseLeave(sender As Object, e As EventArgs) Handles btnclear.MouseLeave
        Cursor = Cursors.Default
    End Sub

    Private Sub btnprint_MouseHover(sender As Object, e As EventArgs) Handles btnprint.MouseHover
        Cursor = Cursors.Hand
    End Sub

    Private Sub btnprint_MouseLeave(sender As Object, e As EventArgs) Handles btnprint.MouseLeave
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
                Dim tb As TextBox = CType(ctrl, TextBox)

                tb.ContextMenuStrip = New ContextMenuStrip()

                AddHandler tb.KeyDown, AddressOf BlockPasteKey
                AddHandler tb.MouseUp, AddressOf BlockRightClick

            End If

            If ctrl.HasChildren Then
                AddHandlerToTextBoxes_NoPaste(ctrl)
            End If
        Next

    End Sub


    Private Sub BlockPasteKey(sender As Object, e As KeyEventArgs)

        If (e.Control AndAlso e.KeyCode = Keys.V) OrElse (e.Shift AndAlso e.KeyCode = Keys.Insert) Then
            e.SuppressKeyPress = True
        End If

    End Sub

    Private Sub BlockRightClick(sender As Object, e As MouseEventArgs)

        If e.Button = MouseButtons.Right Then

            Dim tb As TextBox = TryCast(sender, TextBox)
            If tb IsNot Nothing Then
                tb.ContextMenuStrip = New ContextMenuStrip()
            End If
        End If

    End Sub

    Private Sub txtyearr_KeyDown(sender As Object, e As KeyEventArgs) Handles txtyearr.KeyDown
        If e.Control AndAlso (e.KeyCode = Keys.V Or e.KeyCode = Keys.C Or e.KeyCode = Keys.X) Then
            e.SuppressKeyPress = True
        End If
    End Sub

    Private Sub txtyearr_KeyPress(sender As Object, e As KeyPressEventArgs) Handles txtyearr.KeyPress
        If Not Char.IsDigit(e.KeyChar) And Not Char.IsControl(e.KeyChar) Then
            e.Handled = True
        End If
    End Sub

    Private Sub txtyearr_Validating(sender As Object, e As System.ComponentModel.CancelEventArgs) Handles txtyearr.Validating

        If String.IsNullOrWhiteSpace(txtyearr.Text) Then
            Exit Sub
        End If

        Dim yearValue As Integer
        If Not Integer.TryParse(txtyearr.Text, yearValue) Then
            MessageBox.Show("Please enter a valid year.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtyearr.Clear()
            e.Cancel = True
            Exit Sub
        End If


        Dim currentYear As Integer = Date.Now.Year


        If yearValue > currentYear Then
            MessageBox.Show("Year Published cannot be in the future. Please enter a valid year (up to " & currentYear & ").", "Invalid Year", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtyearr.Text = currentYear.ToString()
            e.Cancel = True
        End If
    End Sub

    Private Sub cbgenre_DropDown(sender As Object, e As EventArgs) Handles cbgenre.DropDown
        Try
            cbgenre.DataSource = Nothing
            cbgenree()
        Catch ex As Exception
            Debug.WriteLine("Error refreshing genre combo: " & ex.Message)
        End Try
    End Sub

    Private Sub cbauthor_DropDown(sender As Object, e As EventArgs) Handles cbauthor.DropDown
        Try
            cbauthor.DataSource = Nothing
            cbauthorr()
        Catch ex As Exception
            Debug.WriteLine("Error refreshing author combo: " & ex.Message)
        End Try
    End Sub

    Private Sub cbpublisher_DropDown(sender As Object, e As EventArgs) Handles cbpublisher.DropDown
        Try
            cbpublisher.DataSource = Nothing
            cbpublisherr()
        Catch ex As Exception
            Debug.WriteLine("Error refreshing publisher combo: " & ex.Message)
        End Try
    End Sub

    Private Sub cblanguage_DropDown(sender As Object, e As EventArgs) Handles cblanguage.DropDown
        Try
            cblanguage.DataSource = Nothing
            cblang()
        Catch ex As Exception
            Debug.WriteLine("Error refreshing language combo: " & ex.Message)
        End Try
    End Sub


End Class