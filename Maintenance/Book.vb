Imports MySql.Data.MySqlClient

Public Class Book

    Private isbarcode As Boolean = False

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

    Private Sub btnadd_Click(sender As Object, e As EventArgs) Handles btnadd.Click

        Dim con As New MySqlConnection(connectionString)
        Dim isbn As Object = Nothing
        Dim barcode As Object = Nothing
        Dim booktitle As String = txtbooktitle.Text.Trim()
        Dim deyts As DateTime = DateTimePicker1.Value

        Dim bookStatus As String = ""
        If rbloanable.Checked Then
            bookStatus = "Borrowable"
        ElseIf rbforlibraryonly.Checked Then
            bookStatus = "For In-Library Use Only"
        Else
            MsgBox("Please select the book status.", vbExclamation, "Validation Error")
            Exit Sub
        End If


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
            Dim com As New MySqlCommand("INSERT INTO `book_tbl`(`Barcode`,`ISBN`, `BookTitle`, `Author`, `Genre`, `Category`, `Publisher`, `Language`, `YearPublished`, `Status`) VALUES (@barcode, @isbn, @booktitle, @author, @genre, @category, @publisher, @language, @yearpublished,@status )", con)

            com.Parameters.AddWithValue("@barcode", If(IsDBNull(barcode), DBNull.Value, barcode))
            com.Parameters.AddWithValue("@isbn", If(IsDBNull(isbn), DBNull.Value, isbn))
            com.Parameters.AddWithValue("@booktitle", booktitle)
            com.Parameters.AddWithValue("@author", cbauthor.Text)
            com.Parameters.AddWithValue("@genre", cbgenre.Text)
            com.Parameters.AddWithValue("@category", cbcategory.Text)
            com.Parameters.AddWithValue("@publisher", cbpublisher.Text)
            com.Parameters.AddWithValue("@language", cblanguage.Text)
            com.Parameters.AddWithValue("@yearpublished", purmatdeyt)
            com.Parameters.AddWithValue("@status", bookStatus)
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

            Dim booktitle As String = txtbooktitle.Text.Trim()
            If String.IsNullOrEmpty(booktitle) OrElse cbauthor.SelectedIndex = -1 OrElse cbgenre.SelectedIndex = -1 OrElse cbcategory.SelectedIndex = -1 OrElse cbpublisher.SelectedIndex = -1 OrElse cblanguage.SelectedIndex = -1 Then
                MsgBox("Please fill in all the required fields.", vbExclamation, "Validation Error")
                Exit Sub
            End If

            Dim bookStatus As String = ""
            If rbloanable.Checked Then
                bookStatus = "Borrowable"
            ElseIf rbforlibraryonly.Checked Then
                bookStatus = "For In-Library Use Only"
            Else
                MsgBox("Please select the book status.", vbExclamation, "Validation Error")
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
                Dim com As New MySqlCommand("UPDATE `book_tbl` SET `Barcode` = @barcode, `ISBN`=@isbn, `BookTitle`= @booktitle, `Author`= @author, `Genre`= @genre, `Category`= @category, `Publisher`= @publisher, `Language`= @language, `YearPublished`= @yearpublished, `Status` = @status WHERE `ID` = @id", con)

                com.Parameters.AddWithValue("@barcode", If(IsDBNull(barcodeValue), DBNull.Value, barcodeValue))
                com.Parameters.AddWithValue("@isbn", If(IsDBNull(isbnValue), DBNull.Value, isbnValue))
                com.Parameters.AddWithValue("@booktitle", booktitle)
                com.Parameters.AddWithValue("@author", cbauthor.Text)
                com.Parameters.AddWithValue("@genre", cbgenre.Text)
                com.Parameters.AddWithValue("@category", cbcategory.Text)
                com.Parameters.AddWithValue("@publisher", cbpublisher.Text)
                com.Parameters.AddWithValue("@language", cblanguage.Text)
                com.Parameters.AddWithValue("@yearpublished", purmatdeyt)
                com.Parameters.AddWithValue("@id", bookID)
                com.Parameters.AddWithValue("@status", bookStatus)
                com.ExecuteNonQuery()

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

                Try
                    con.Open()


                    Dim delete As New MySqlCommand("DELETE FROM `book_tbl` WHERE `ID` = @id", con)
                    delete.Parameters.AddWithValue("@id", bookID)
                    delete.ExecuteNonQuery()

                    MsgBox("Book deleted successfully.", vbInformation)
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


                Book_Load(sender, e)

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
        lblrandom.Text = "00000000000"
        txtisbn.Enabled = True
        rbgenerate.Enabled = True

        cbauthor.DataSource = Nothing
        cbgenre.DataSource = Nothing
        cbcategory.DataSource = Nothing
        cbpublisher.DataSource = Nothing
        cblanguage.DataSource = Nothing
        rbgenerate.Checked = False
        rbloanable.Checked = False
        rbforlibraryonly.Checked = False

        cbauthorr()
        cbgenree()
        cbcategoryy()
        cbpublisherr()
        cblang()


        DateTimePicker1.Value = DateTime.Today
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

    Private Sub Book_FormClosed(sender As Object, e As FormClosedEventArgs) Handles Me.FormClosed
        clear()
    End Sub

    Private Sub rbgenerate_CheckedChanged(sender As Object, e As EventArgs) Handles rbgenerate.CheckedChanged

        If Not isbarcode Then
            If rbgenerate.Checked Then
                lblrandom.Text = jinireyt
                txtisbn.Enabled = False
                txtisbn.Text = ""
            Else
                lblrandom.Text = "00000000000"
                txtisbn.Enabled = True
            End If
        End If
    End Sub

    Private Sub DataGridView1_CellClick_1(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellClick

        If e.RowIndex >= 0 AndAlso Not DataGridView1.Rows(e.RowIndex).IsNewRow Then

            isbarcode = True

            Dim row = DataGridView1.Rows(e.RowIndex)

            lblrandom.Text = row.Cells("Barcode").Value.ToString


            Dim isbnValue = row.Cells("ISBN").Value

            If IsDBNull(isbnValue) OrElse String.IsNullOrEmpty(isbnValue.ToString) Then

                rbgenerate.Checked = True
                txtisbn.Enabled = False
                txtisbn.Text = ""
            Else

                rbgenerate.Checked = False
                txtisbn.Enabled = True
                txtisbn.Text = isbnValue.ToString
            End If

            txtbooktitle.Text = row.Cells("BookTitle").Value.ToString
            cbauthor.Text = row.Cells("Author").Value.ToString
            cbgenre.Text = row.Cells("Genre").Value.ToString
            cbcategory.Text = row.Cells("Category").Value.ToString
            cbpublisher.Text = row.Cells("Publisher").Value.ToString
            cblanguage.Text = row.Cells("Language").Value.ToString
            DateTimePicker1.Value = CDate(row.Cells("YearPublished").Value)

            Dim bookStatus = row.Cells("Status").Value.ToString
            If bookStatus = "Borrowable" Then
                rbloanable.Checked = True
            ElseIf bookStatus = "For In-Library Use Only" Then
                rbforlibraryonly.Checked = True
            End If

            rbgenerate.Enabled = False
            isbarcode = False
        End If

    End Sub
End Class