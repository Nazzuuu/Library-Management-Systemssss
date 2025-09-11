Imports System.Net
Imports MySql.Data.MySqlClient
Imports Windows.Win32.System

Public Class Author
    Private Sub Author_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        TopMost = True

        Me.Font = New Font("Baskerville Old Face", 9)
        Me.Refresh()


        Dim con As New MySqlConnection(connectionString)
        Dim comm As String = "SELECT * FROM `author_tbl`"
        Dim adap As New MySqlDataAdapter(comm, con)
        Dim ds As New DataSet
        adap.Fill(ds, "INFO")
        DataGridView1.DataSource = ds.Tables("INFO")


        DataGridView1.EnableHeadersVisualStyles = False
        DataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(207, 58, 109)
        DataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White
    End Sub

    Private Sub Guna2ControlBox1_Click(sender As Object, e As EventArgs)
        MainForm.Show()

    End Sub

    Private Sub Author_FormClosed(sender As Object, e As FormClosedEventArgs) Handles Me.FormClosed
        MainForm.MaintenanceToolStripMenuItem.ShowDropDown()
        MainForm.MaintenanceToolStripMenuItem.ForeColor = Color.Gray
        txtauthor.Text = ""

    End Sub



    Private Sub btnadd_Click(sender As Object, e As EventArgs) Handles btnadd.Click

        Dim con As New MySqlConnection(connectionString)
        Dim author As String = txtauthor.Text.Trim

        If String.IsNullOrWhiteSpace(author) Then
            MsgBox("Please fill in the required fields.", vbExclamation, "Missing Information")
            Exit Sub
        End If

        Try
            con.Open()

            Dim comsu As New MySqlCommand("SELECT COUNT(*) FROM `author_tbl` WHERE `AuthorName` = @author", con)
            comsu.Parameters.AddWithValue("@author", author)
            Dim count As Integer = Convert.ToInt32(comsu.ExecuteScalar())

            If count > 0 Then
                MsgBox("This author already exists.", vbExclamation, "Duplication is not allowed.")
                Exit Sub
            End If

            Dim com As New MySqlCommand("INSERT INTO `author_tbl`(`AuthorName`) VALUES (@author)", con)
            com.Parameters.AddWithValue("@author", author)

            com.ExecuteNonQuery()



            For Each form In Application.OpenForms
                If TypeOf form Is Book Then
                    Dim book = DirectCast(form, Book)
                    book.cbauthorr()
                    Exit For
                End If
            Next

            MsgBox("Author added successfully", vbInformation)
            Author_Load(sender, e)

        Catch ex As Exception
            MsgBox(ex.Message, vbCritical)
        Finally
            txtauthor.Clear()
        End Try
    End Sub

    Private Sub btnedit_Click(sender As Object, e As EventArgs) Handles btnedit.Click

        If DataGridView1.SelectedRows.Count > 0 Then

            Dim con As New MySqlConnection(connectionString)

            Dim selectedRow As DataGridViewRow = DataGridView1.SelectedRows(0)
            Dim ID As Integer = CInt(selectedRow.Cells("ID").Value)

            Dim author As String = txtauthor.Text.Trim

            If String.IsNullOrWhiteSpace(author) Then
                MsgBox("Please fill in the required fields.", vbExclamation, "Missing Information")
                Exit Sub
            End If


            Try
                con.Open()

                Dim comsu As New MySqlCommand("SELECT COUNT(*) FROM `author_tbl` WHERE `AuthorName` = @author", con)
                comsu.Parameters.AddWithValue("@author", author)
                Dim count As Integer = Convert.ToInt32(comsu.ExecuteScalar())

                If count > 0 Then
                    MsgBox("This author already exists.", vbExclamation, "Duplication is not allowed.")
                    Exit Sub
                End If

                Dim com As New MySqlCommand("UPDATE `author_tbl` SET `AuthorName`= @author WHERE  `ID` = @id", con)
                com.Parameters.AddWithValue("@author", author)
                com.Parameters.AddWithValue("@id", ID)
                com.ExecuteNonQuery()

                For Each form In Application.OpenForms
                    If TypeOf form Is Book Then
                        Dim book = DirectCast(form, Book)
                        book.cbauthorr()
                        Exit For
                    End If
                Next

                MsgBox("Updated successfully!", vbInformation)
                Author_Load(sender, e)
                txtauthor.Clear()
            Catch ex As Exception
                MsgBox(ex.Message, vbCritical)
            End Try

        End If
    End Sub

    Private Sub btndelete_Click(sender As Object, e As EventArgs) Handles btndelete.Click

        If DataGridView1.SelectedRows.Count > 0 Then

            Dim dialogResult As DialogResult = MessageBox.Show("Are you sure you want to delete this author?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)

            If dialogResult = DialogResult.Yes Then

                Dim con As New MySqlConnection(connectionString)
                Dim selectedRow As DataGridViewRow = DataGridView1.SelectedRows(0)
                Dim ID As Integer = CInt(selectedRow.Cells("ID").Value)
                Dim authorName As String = selectedRow.Cells("AuthorName").Value.ToString().Trim()

                Try
                    con.Open()


                    Dim bookCom As New MySqlCommand("SELECT COUNT(*) FROM `book_tbl` WHERE Author = @author", con)
                    bookCom.Parameters.AddWithValue("@author", authorName)
                    Dim bookCount As Integer = CInt(bookCom.ExecuteScalar())

                    If bookCount > 0 Then
                        MessageBox.Show("Cannot delete this author. They are assigned to " & bookCount & " book(s).", "Information", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        Return
                    End If


                    Dim delete As New MySqlCommand("DELETE FROM `author_tbl` WHERE `ID` = @id", con)
                    delete.Parameters.AddWithValue("@id", ID)
                    delete.ExecuteNonQuery()

                    For Each form In Application.OpenForms
                        If TypeOf form Is Book Then
                            Dim book = DirectCast(form, Book)
                            book.cbauthorr()
                            Exit For
                        End If
                    Next

                    MsgBox("Author deleted successfully.", vbInformation)
                    Author_Load(sender, e)
                    txtauthor.Clear()

                    Dim count As New MySqlCommand("SELECT COUNT(*) FROM `author_tbl`", con)
                    Dim rowCount As Long = CLng(count.ExecuteScalar())

                    If rowCount = 0 Then
                        Dim reset As New MySqlCommand("ALTER TABLE `author_tbl` AUTO_INCREMENT = 1", con)
                        reset.ExecuteNonQuery()
                    End If

                Catch ex As Exception
                    MsgBox(ex.Message, vbCritical)
                End Try
            End If
        End If
    End Sub

    Private Sub DataGridView1_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellClick

        If e.RowIndex >= 0 Then

            Dim row As DataGridViewRow = Me.DataGridView1.Rows(e.RowIndex)
            txtauthor.Text = row.Cells("AuthorName").Value.ToString

        End If
    End Sub

    Private Sub txtsearch_TextChanged(sender As Object, e As EventArgs) Handles txtsearch.TextChanged

        Dim dt As DataTable = DirectCast(DataGridView1.DataSource, DataTable)
        If dt IsNot Nothing Then
            If txtsearch.Text.Trim() <> "" Then
                Dim filter As String = String.Format("AuthorName LIKE '*{0}*'", txtsearch.Text.Trim())
                dt.DefaultView.RowFilter = filter
            Else
                dt.DefaultView.RowFilter = ""
            End If
        End If
    End Sub

    Private Sub txtauthor_KeyDown(sender As Object, e As KeyEventArgs) Handles txtauthor.KeyDown

        If e.Control AndAlso (e.KeyCode = Keys.V Or e.KeyCode = Keys.C Or e.KeyCode = Keys.X) Then
            e.SuppressKeyPress = True
        End If


    End Sub

    Private Sub txtauthor_KeyPress(sender As Object, e As KeyPressEventArgs) Handles txtauthor.KeyPress

        If Not Char.IsLetter(e.KeyChar) AndAlso Not Char.IsControl(e.KeyChar) AndAlso Not Char.IsWhiteSpace(e.KeyChar) Then
            e.Handled = True
        End If

    End Sub

    Private Sub txtsearch_KeyDown(sender As Object, e As KeyEventArgs) Handles txtsearch.KeyDown

        If e.Control AndAlso (e.KeyCode = Keys.V Or e.KeyCode = Keys.C Or e.KeyCode = Keys.X) Then
            e.SuppressKeyPress = True
        End If

    End Sub




    'heck nooo'

End Class