Imports MySql.Data.MySqlClient

Public Class Book
    Private Sub Panel2_Paint(sender As Object, e As PaintEventArgs)

    End Sub

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


        DateTimePicker1.MaxDate = DateTime.Today.AddMonths(-7)

        DataGridView1.Columns("ID").Visible = False
        DataGridView1.EnableHeadersVisualStyles = False
        DataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(207, 58, 109)
        DataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White


        cbauthorr()
        cbgenree()
        cbpublisherr()
        cblang()
        cbcategoryy

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
        Dim isbn As String = txtisbn.Text.Trim
        Dim booktitle As String = txtbooktitle.Text.Trim

        Dim deyts As DateTime = DateTimePicker1.Value
        Dim purmatdeyt As String = deyts.ToString("yyyy-MMMM-dd")


        Try
            con.Open()
            Dim com As New MySqlCommand("INSERT INTO `book_tbl`(`ISBN`, `BookTitle`, `Author`, `Genre`, `Category`, `Publisher`, `Language`, `YearPublished`) VALUES
                                       (@isbn, @booktitle, @author, @genre, @category, @publisher, @language, @yearpublished )", con)

            com.Parameters.AddWithValue("@isbn", isbn)
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
        End Try

    End Sub

    Private Sub btnedit_Click(sender As Object, e As EventArgs) Handles btnedit.Click

        If DataGridView1.SelectedRows.Count > 0 Then

            Dim con As New MySqlConnection(connectionString)

            Dim isbn As String = txtisbn.Text.Trim
            Dim booktitle As String = txtbooktitle.Text.Trim
            Dim deyts As DateTime = DateTimePicker1.Value
            Dim purmatdeyt As String = deyts.ToString("yyyy-MMMM-dd")


            Dim selectedRow As DataGridViewRow = DataGridView1.SelectedRows(0)
            Dim bookID As Integer = CInt(selectedRow.Cells("ID").Value)

            Try
                con.Open()


                Dim com As New MySqlCommand("UPDATE `book_tbl` SET `ISBN`=@isbn, `BookTitle`= @booktitle, `Author`= @author, `Genre`= @genre, `Category`= @category, `Publisher`= @publisher, `Language`= @language, `YearPublished`= @yearpublished WHERE `ID` = @id", con)

                com.Parameters.AddWithValue("@isbn", isbn)
                com.Parameters.AddWithValue("@booktitle", booktitle)
                com.Parameters.AddWithValue("@author", cbauthor.Text)
                com.Parameters.AddWithValue("@genre", cbgenre.Text)
                com.Parameters.AddWithValue("@category", cbcategory.Text)
                com.Parameters.AddWithValue("@publisher", cbpublisher.Text)
                com.Parameters.AddWithValue("@language", cblanguage.Text)
                com.Parameters.AddWithValue("@yearpublished", purmatdeyt)
                com.Parameters.AddWithValue("@id", bookID)

                com.ExecuteNonQuery()

                MsgBox("Book updated successfully", vbInformation)
                clear()

                Book_Load(sender, e)

            Catch ex As Exception
                MsgBox(ex.Message, vbCritical)
            End Try
        Else
            MsgBox("Please select a row to edit.", vbExclamation)
        End If
    End Sub

    Private Sub DataGridView1_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellClick

        If e.RowIndex >= 0 Then
            Dim row As DataGridViewRow = Me.DataGridView1.Rows(e.RowIndex)

            txtisbn.Text = row.Cells("ISBN").Value.ToString
            txtbooktitle.Text = row.Cells("BookTitle").Value.ToString
            cbauthor.Text = row.Cells("Author").Value.ToString()
            cbgenre.Text = row.Cells("Genre").Value.ToString
            cbcategory.Text = row.Cells("Category").Value.ToString
            cbpublisher.Text = row.Cells("Publisher").Value.ToString
            cblanguage.Text = row.Cells("Language").Value.ToString
            DateTimePicker1.Value = CDate(row.Cells("YearPublished").Value)


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

        txtisbn.Clear()
        txtbooktitle.Clear()

        cbauthor.SelectedIndex = -1
        cbgenre.SelectedIndex = -1
        cbcategory.SelectedIndex = -1
        cbpublisher.SelectedIndex = -1
        cblanguage.SelectedIndex = -1

        DateTimePicker1.Value = DateTime.Today

    End Sub
End Class