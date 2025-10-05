Imports System.Drawing
Imports System.IO
Imports MySql.Data.MySqlClient
Imports ZXing
Imports ZXing.Rendering
Imports ZXing.Windows.Compatibility

Public Class Book

    Private isbarcode As Boolean = False
    Private BarcodeList As New List(Of String)
    Private BarcodeIndex As Integer = 0
    Private Const BarcodeWidth As Integer = 200
    Private Const BarcodeHeight As Integer = 80
    Private Const MarginX As Integer = 50
    Private Const MarginY As Integer = 50
    Private Const BarcodeSpacing As Integer = 20
    Private Sub Book_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        If MainForm.WindowState = FormWindowState.Normal Then
            panel_book.Size = New Size(930, 310)
        End If

        Dim con As New MySqlConnection(connectionString)
        Dim com As String = "SELECT * FROM `book_tbl`"
        Dim adp As New MySqlDataAdapter(com, con)
        Dim dt As New DataSet

        adp.Fill(dt, "INFO")

        DataGridView1.DataSource = dt.Tables("INFO")




        DataGridView1.Columns("ID").Visible = False
        DataGridView1.EnableHeadersVisualStyles = False
        DataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(207, 58, 109)
        DataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White

        DataGridView1.ClearSelection()
        DataGridView1.CurrentCell = Nothing


        cbauthorr()
        cbgenree()
        cbpublisherr()
        cblang()
        cbcategoryy()

        clear()
        picbarcode.Image = GenerateBarcodeImage(lblrandom.Text, picbarcode.Width, picbarcode.Height)

        DateTimePicker1.Value = DateTime.Now
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

    Public Sub cbcategoryy()

        Dim con As New MySqlConnection(connectionString)
        Dim com As String = "SELECT ID, Category FROM category_tbl"
        Dim adap As New MySqlDataAdapter(com, con)
        Dim dt As New DataTable

        adap.Fill(dt)

        cbcategory.DataSource = dt
        cbcategory.DisplayMember = "Category"
        cbcategory.ValueMember = "ID"
        cbcategory.SelectedIndex = -1

    End Sub

    Private Sub Book_Shown(sender As Object, e As EventArgs) Handles MyBase.Shown

        DataGridView1.ClearSelection()

    End Sub
    Private Sub btnadd_Click(sender As Object, e As EventArgs) Handles btnadd.Click

        Dim con As New MySqlConnection(connectionString)
        Dim isbn As Object = Nothing
        Dim barcode As Object = Nothing
        Dim booktitle As String = txtbooktitle.Text.Trim()
        Dim deyts As DateTime = DateTimePicker1.Value

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

        If String.IsNullOrEmpty(booktitle) OrElse cbauthor.SelectedIndex = -1 OrElse cbgenre.SelectedIndex = -1 OrElse cbcategory.SelectedIndex = -1 OrElse cbpublisher.SelectedIndex = -1 OrElse cblanguage.SelectedIndex = -1 Then
            MsgBox("Please fill in all the required fields.", vbExclamation, "Validation Error")
            Exit Sub
        End If

        If deyts.Date > DateTime.Today.Date Then
            MsgBox("You cannot select a future date.", vbExclamation)
            Exit Sub
        End If

        Dim purmatdeyt As String = deyts.ToString("yyyy-MM-dd")

        Try
            con.Open()

            Dim com As New MySqlCommand("INSERT INTO `book_tbl`(`Barcode`,`ISBN`, `BookTitle`, `Author`, `Genre`, `Category`, `Publisher`, `Language`, `YearPublished`) VALUES (@barcode, @isbn, @booktitle, @author, @genre, @category, @publisher, @language, @yearpublished)", con)

            com.Parameters.AddWithValue("@barcode", If(IsDBNull(barcode), DBNull.Value, barcode))
            com.Parameters.AddWithValue("@isbn", If(IsDBNull(isbn), DBNull.Value, isbn))
            com.Parameters.AddWithValue("@booktitle", booktitle)
            com.Parameters.AddWithValue("@author", cbauthor.Text)
            com.Parameters.AddWithValue("@genre", cbgenre.Text)
            com.Parameters.AddWithValue("@category", cbcategory.Text)
            com.Parameters.AddWithValue("@publisher", cbpublisher.Text)
            com.Parameters.AddWithValue("@language", cblanguage.Text)
            com.Parameters.AddWithValue("@yearpublished", purmatdeyt)
            com.ExecuteNonQuery()

            MsgBox("Book added successfully", vbInformation)
            clear()
            Book_Load(sender, e)
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

            If String.IsNullOrEmpty(newBookTitle) OrElse cbauthor.SelectedIndex = -1 OrElse cbgenre.SelectedIndex = -1 OrElse cbcategory.SelectedIndex = -1 OrElse cbpublisher.SelectedIndex = -1 OrElse cblanguage.SelectedIndex = -1 Then
                MsgBox("Please fill in all the required fields.", vbExclamation, "Validation Error")
                Exit Sub
            End If


            Dim con As New MySqlConnection(connectionString)
            Dim deyts As DateTime = DateTimePicker1.Value

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

            If deyts.Date > DateTime.Today.Date Then
                MsgBox("You cannot select a future date.", vbExclamation)
                Exit Sub
            End If
            Dim purmatdeyt As String = deyts.ToString("yyyy-MM-dd")

            Try
                con.Open()


                Dim com As New MySqlCommand("UPDATE `book_tbl` SET `Barcode` = @barcode, `ISBN`=@isbn, `BookTitle`= @booktitle, `Author`= @author, `Genre`= @genre, `Category`= @category, `Publisher`= @publisher, `Language`= @language, `YearPublished`= @yearpublished WHERE `ID` = @id", con)
                com.Parameters.AddWithValue("@barcode", If(IsDBNull(barcodeValue), DBNull.Value, barcodeValue))
                com.Parameters.AddWithValue("@isbn", If(IsDBNull(isbnValue), DBNull.Value, isbnValue))
                com.Parameters.AddWithValue("@booktitle", newBookTitle)
                com.Parameters.AddWithValue("@author", cbauthor.Text)
                com.Parameters.AddWithValue("@genre", cbgenre.Text)
                com.Parameters.AddWithValue("@category", cbcategory.Text)
                com.Parameters.AddWithValue("@publisher", cbpublisher.Text)
                com.Parameters.AddWithValue("@language", cblanguage.Text)
                com.Parameters.AddWithValue("@yearpublished", purmatdeyt)
                com.Parameters.AddWithValue("@id", bookID)
                com.ExecuteNonQuery()


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
                comReserve.Parameters.AddWithValue("oldBookTitle", oldBookTitle)
                comReserve.ExecuteNonQuery()

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



                Next

                MsgBox("Book updated successfully", vbInformation)
                clear()
                Book_Load(sender, e)

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

                Dim con As New MySqlConnection(connectionString)
                Dim selectedRow As DataGridViewRow = DataGridView1.SelectedRows(0)

                Dim bookID As Integer = CInt(selectedRow.Cells("ID").Value)

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

                    MsgBox("Book deleted successfully.", vbInformation)


                    Book_Load(sender, e)
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

    Private Sub btnclear_Click(sender As Object, e As EventArgs) Handles btnclear.Click

        clear()

    End Sub

    Public Sub clear()

        txtisbn.Text = ""
        txtbooktitle.Text = ""
        lblrandom.Text = "0000000000000"
        lblrandom.Visible = False

        txtisbn.Enabled = True
        rbgenerate.Enabled = True

        cbauthor.DataSource = Nothing
        cbgenre.DataSource = Nothing
        cbcategory.DataSource = Nothing
        cbpublisher.DataSource = Nothing
        cblanguage.DataSource = Nothing
        rbgenerate.Checked = False

        cbauthorr()
        cbgenree()
        cbcategoryy()
        cbpublisherr()
        cblang()


        picbarcode.Image = GenerateBarcodeImage(lblrandom.Text, picbarcode.Width, picbarcode.Height)

        DateTimePicker1.Value = DateTime.Today
        DataGridView1.ClearSelection()
    End Sub

    Function GenerateBarcodeImage(ByVal barcodeText As String, ByVal width As Integer, ByVal height As Integer) As Image
        Try

            Dim writer As New BarcodeWriter(Of Bitmap) With {
            .Format = BarcodeFormat.CODE_128,
            .Options = New ZXing.Common.EncodingOptions With {
                .Width = width,
                .Height = height,
                .Margin = 10
            },
            .Renderer = New BitmapRenderer()
        }

            Dim barcodeBitmap As Bitmap = writer.Write(barcodeText)

            Dim finalImage As New Bitmap(width, height + 20)
            Using g As Graphics = Graphics.FromImage(finalImage)
                g.Clear(Color.White)
                g.DrawImage(barcodeBitmap, 0, 0)

                Using font As New Font("Arial", 8)
                    Using sf As New StringFormat With {
                        .Alignment = StringAlignment.Center
                    }
                        g.DrawString(barcodeText, font, Brushes.Black, New RectangleF(0, height, width, 20), sf)
                    End Using
                End Using
            End Using

            Return finalImage

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


    Function jinireyt() As String
        Dim random As New Random()
        Dim barcode As String = ""
        For i As Integer = 0 To 12
            barcode += random.Next(0, 10).ToString()
        Next
        Return barcode
    End Function

    Private Sub txtsearch_TextChanged(sender As Object, e As EventArgs) Handles txtsearch.TextChanged


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
            cbcategory.Text = row.Cells("Category").Value.ToString
            cbpublisher.Text = row.Cells("Publisher").Value.ToString
            cblanguage.Text = row.Cells("Language").Value.ToString
            DateTimePicker1.Value = CDate(row.Cells("YearPublished").Value)

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

    Private Sub Printbarcode(sender As Object, e As EventArgs) Handles btnprint.Click

        BarcodeList.Clear()
        BarcodeIndex = 0

        For Each row As DataGridViewRow In DataGridView1.Rows
            If Not row.IsNewRow Then

                Dim barcodeValue As String = If(IsDBNull(row.Cells("Barcode").Value), String.Empty, CStr(row.Cells("Barcode").Value))


                If Not String.IsNullOrEmpty(barcodeValue) AndAlso barcodeValue <> "0000000000000" Then
                    BarcodeList.Add(barcodeValue)
                End If
            End If
        Next

        If BarcodeList.Count = 0 Then
            MessageBox.Show("Walang Barcode na makikita para i-print. Tiyakin na may Barcode value ang mga aklat.", "Print Error", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Exit Sub
        End If

        Dim pd As New Printing.PrintDocument()
        AddHandler pd.PrintPage, AddressOf Me.PrintDocument_PrintPage

        Dim ppd As New PrintPreviewDialog()
        ppd.Document = pd

        ppd.ShowDialog()

    End Sub


    Private Sub PrintDocument_PrintPage(sender As Object, e As Printing.PrintPageEventArgs)
        Dim g As Graphics = e.Graphics


        Dim currentX As Integer = MarginX
        Dim currentY As Integer = MarginY


        Dim BarcodesPerRow As Integer = CInt(Math.Floor((e.PageBounds.Width - (2 * MarginX)) / (BarcodeWidth + BarcodeSpacing)))
        If BarcodesPerRow = 0 Then BarcodesPerRow = 1


        While BarcodeIndex < BarcodeList.Count

            Dim barcodeText As String = BarcodeList(BarcodeIndex)


            Dim barcodeImage As Image = GenerateBarcodeImage(barcodeText, BarcodeWidth, BarcodeHeight)

            If currentX + BarcodeWidth > e.PageBounds.Width - MarginX Then
                currentX = MarginX
                currentY += BarcodeHeight + BarcodeSpacing
            End If


            If currentY + BarcodeHeight > e.PageBounds.Height - MarginY Then
                e.HasMorePages = True
                BarcodeIndex -= 1
                Exit Sub
            End If


            g.DrawImage(barcodeImage, currentX, currentY, BarcodeWidth, BarcodeHeight)


            barcodeImage.Dispose()


            currentX += BarcodeWidth + BarcodeSpacing


            BarcodeIndex += 1
        End While


        e.HasMorePages = False
    End Sub

End Class