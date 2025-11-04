Imports System.Net
Imports MySql.Data.MySqlClient
Imports Windows.Win32.System

Public Class Author
    Private Sub Author_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        TopMost = True
        Me.Font = New Font("Baskerville Old Face", 9)
        Me.Refresh()

        refreshauthor()
        AddHandler GlobalVarsModule.DatabaseUpdated, AddressOf OnDatabaseUpdated

    End Sub


    Public Sub refreshauthor()

        Dim query As String = "SELECT * FROM `author_tbl` ORDER BY ID DESC"
        GlobalVarsModule.AutoRefreshGrid(DataGridView1, query, 2000)
        SetupGridStyle()

    End Sub


    Private Async Sub OnDatabaseUpdated()
        Try

            Dim selectedAuthor As String = ""
            If DataGridView1.SelectedRows.Count > 0 Then
                selectedAuthor = DataGridView1.SelectedRows(0).Cells("AuthorName").Value.ToString()
            End If


            Dim query As String = "SELECT * FROM `author_tbl` ORDER BY ID DESC"
            Await GlobalVarsModule.LoadToGridAsync(DataGridView1, query)
            SetupGridStyle()


            If Not String.IsNullOrWhiteSpace(selectedAuthor) Then
                For Each row As DataGridViewRow In DataGridView1.Rows
                    If row.Cells("AuthorName").Value.ToString() = selectedAuthor Then
                        row.Selected = True
                        DataGridView1.FirstDisplayedScrollingRowIndex = row.Index
                        Exit For
                    End If
                Next
            End If

        Catch ex As Exception
            MessageBox.Show("Error while refreshing author list: " & ex.Message)
        End Try
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
        Catch ex As Exception

        End Try
    End Sub


    Private Sub Guna2ControlBox1_Click(sender As Object, e As EventArgs)
        MainForm.Show()

    End Sub

    Private Sub Author_Shown(sender As Object, e As EventArgs) Handles MyBase.Shown

        DataGridView1.ClearSelection()

    End Sub

    Private Sub Author_FormClosed(sender As Object, e As FormClosedEventArgs) Handles Me.FormClosed

        For Each form In Application.OpenForms
            If TypeOf form Is MainForm Then
                Dim load = DirectCast(form, MainForm)
                load.loadsu()
            End If
        Next

        MainForm.MaintenanceToolStripMenuItem.ForeColor = Color.White
        txtauthor.Text = ""

    End Sub



    Private Sub btnadd_Click(sender As Object, e As EventArgs) Handles btnadd.Click

        Dim con As New MySqlConnection(GlobalVarsModule.connectionString)
        Dim author As String = txtauthor.Text.Trim()
        Dim insertedID As Integer = 0

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

            Dim com As New MySqlCommand("INSERT INTO `author_tbl`(`AuthorName`) VALUES (@author); SELECT LAST_INSERT_ID()", con)
            com.Parameters.AddWithValue("@author", author)

            insertedID = Convert.ToInt32(com.ExecuteScalar())

            GlobalVarsModule.LogAudit(
                actionType:="ADD",
                formName:="AUTHOR FORM",
                description:=$"Added new Author: {author}",
                recordID:=insertedID.ToString()
            )

            For Each form In Application.OpenForms
                If TypeOf form Is Book Then
                    Dim book = DirectCast(form, Book)
                    book.cbauthorr()
                    Exit For
                End If
            Next

            For Each form In Application.OpenForms
                If TypeOf form Is AuditTrail Then
                    Dim load = DirectCast(form, AuditTrail)
                    load.refreshaudit()
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

            Dim con As New MySqlConnection(GlobalVarsModule.connectionString)

            Dim selectedRow As DataGridViewRow = DataGridView1.SelectedRows(0)
            Dim ID As Integer = CInt(selectedRow.Cells("ID").Value)

            Dim author As String = txtauthor.Text.Trim()

            If String.IsNullOrWhiteSpace(author) Then
                MsgBox("Please fill in the required fields.", vbExclamation, "Missing Information")
                Exit Sub
            End If

            Dim oldawtor As String = selectedRow.Cells("AuthorName").Value.ToString().Trim()
            Dim newawtor As String = txtauthor.Text.Trim()

            If String.Equals(oldawtor, newawtor, StringComparison.OrdinalIgnoreCase) Then
                MsgBox("The author name is the same as the current one.", vbInformation)
                Exit Sub
            End If

            Try
                con.Open()

                Dim comsu As New MySqlCommand("SELECT COUNT(*) FROM `author_tbl` WHERE `AuthorName` = @author AND ID <> @id", con)
                comsu.Parameters.AddWithValue("@author", author)
                comsu.Parameters.AddWithValue("@id", ID)
                Dim count As Integer = Convert.ToInt32(comsu.ExecuteScalar())

                If count > 0 Then
                    MsgBox("This author already exists.", vbExclamation, "Duplication is not allowed.")
                    Exit Sub
                End If

                Dim com As New MySqlCommand("UPDATE `author_tbl` SET `AuthorName` = @newawtor WHERE `ID` = @id", con)
                com.Parameters.AddWithValue("@newawtor", newawtor)
                com.Parameters.AddWithValue("@id", ID)
                com.ExecuteNonQuery()


                Dim comsus As New MySqlCommand("UPDATE `book_tbl` SET `Author` = @newawtor WHERE `Author` = @oldawtor", con)
                comsus.Parameters.AddWithValue("@newawtor", newawtor)
                comsus.Parameters.AddWithValue("@oldawtor", oldawtor)
                comsus.ExecuteNonQuery()

                GlobalVarsModule.LogAudit(
                    actionType:="UPDATE",
                    formName:="AUTHOR FORM",
                    description:=$"Updated Author Name.",
                    recordID:=ID.ToString(),
                    oldValue:=oldawtor,
                    newValue:=newawtor
                )

                For Each form In Application.OpenForms
                    If TypeOf form Is Book Then
                        Dim book = DirectCast(form, Book)
                        book.cbauthorr()
                        Exit For
                    End If
                Next

                For Each form In Application.OpenForms
                    If TypeOf form Is AuditTrail Then
                        Dim load = DirectCast(form, AuditTrail)
                        load.refreshaudit()
                    End If
                Next

                For Each form In Application.OpenForms
                    If TypeOf form Is MainForm Then
                        Dim load = DirectCast(form, MainForm)
                        load.loadsu()
                    End If
                Next

                MsgBox("Updated successfully!", vbInformation)
                Author_Load(sender, e)
                txtauthor.Clear()
            Catch ex As Exception
                MsgBox(ex.Message, vbCritical)
            End Try
        Else
            MsgBox("Please select a row to edit.", vbExclamation)
        End If
    End Sub

    Private Sub btndelete_Click(sender As Object, e As EventArgs) Handles btndelete.Click

        If DataGridView1.SelectedRows.Count > 0 Then

            Dim dialogResult As DialogResult = MessageBox.Show("Are you sure you want to delete this author?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)

            If dialogResult = DialogResult.Yes Then

                Dim con As New MySqlConnection(GlobalVarsModule.connectionString)
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

                    GlobalVarsModule.LogAudit(
                        actionType:="DELETE",
                        formName:="AUTHOR FORM",
                        description:=$"Deleted Author: {authorName}",
                        recordID:=ID.ToString()
                    )

                    For Each form In Application.OpenForms
                        If TypeOf form Is Book Then
                            Dim book = DirectCast(form, Book)
                            book.cbauthorr()
                            Exit For
                        End If
                    Next

                    For Each form In Application.OpenForms
                        If TypeOf form Is AuditTrail Then
                            Dim load = DirectCast(form, AuditTrail)
                            load.refreshaudit()
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

        If Char.IsLetter(e.KeyChar) Then
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


        If e.KeyChar = "."c Then
            e.Handled = False
            Return
        End If


        e.Handled = True

    End Sub



    Private Sub txtsearch_KeyDown(sender As Object, e As KeyEventArgs) Handles txtsearch.KeyDown

        If e.Control AndAlso (e.KeyCode = Keys.V Or e.KeyCode = Keys.C Or e.KeyCode = Keys.X) Then
            e.SuppressKeyPress = True
        End If

    End Sub

    Private Sub Author_KeyDown(sender As Object, e As KeyEventArgs) Handles MyBase.KeyDown
        If e.KeyCode = Keys.Escape Then
            Me.Close()
        End If
    End Sub

    Private Sub btnadd_MouseHover(sender As Object, e As EventArgs) Handles btnadd.MouseHover

        Me.Cursor = Cursors.Hand

    End Sub

    Private Sub btnadd_Mouseleave(sender As Object, e As EventArgs) Handles btnadd.MouseLeave

        Me.Cursor = Cursors.Default

    End Sub

    Private Sub btnedit_MouseHover(sender As Object, e As EventArgs) Handles btnedit.MouseHover

        Me.Cursor = Cursors.Hand

    End Sub

    Private Sub btnedit_Mouseleave(sender As Object, e As EventArgs) Handles btnedit.MouseLeave

        Me.Cursor = Cursors.Default

    End Sub

    Private Sub btndelete_MouseHover(sender As Object, e As EventArgs) Handles btndelete.MouseHover

        Me.Cursor = Cursors.Hand

    End Sub

    Private Sub btndelete_Mouseleave(sender As Object, e As EventArgs) Handles btndelete.MouseLeave

        Me.Cursor = Cursors.Default

    End Sub

    Private Sub DataGridView1_CellClick_1(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellClick

        If e.RowIndex >= 0 Then

            Dim row = DataGridView1.Rows(e.RowIndex)
            txtauthor.Text = row.Cells("AuthorName").Value.ToString

        End If

    End Sub

    Private Sub txtauthor_TextChanged(sender As Object, e As EventArgs) Handles txtauthor.TextChanged

        Dim InputText As String = txtauthor.Text
        Dim FilteredText As New System.Text.StringBuilder()

        For Each c As Char In InputText
            If Char.IsLetter(c) OrElse Char.IsWhiteSpace(c) OrElse c = "."c Then
                FilteredText.Append(c)
            End If
        Next

        If FilteredText.ToString() <> InputText Then

            Dim CursorPosition As Integer = txtauthor.SelectionStart
            txtauthor.Text = FilteredText.ToString()


            If CursorPosition > 0 Then
                txtauthor.SelectionStart = Math.Min(CursorPosition - 1, txtauthor.Text.Length)
            Else
                txtauthor.SelectionStart = 0
            End If
        End If

    End Sub

    Private Sub txtauthor_Validating(sender As Object, e As System.ComponentModel.CancelEventArgs) Handles txtauthor.Validating

        Dim AuthorName As String = txtauthor.Text.Trim()

        Dim StrictPattern As String = "^([A-Z][a-z]*|\.?[A-Z]\.?)(\s+([A-Z][a-z]*|\.?[A-Z]\.?))*((\s(Jr|Sr|III|IV)\.?))?$"


        If String.IsNullOrEmpty(AuthorName) Then

            e.Cancel = False
            Return
        End If

        If Not System.Text.RegularExpressions.Regex.IsMatch(AuthorName, StrictPattern, System.Text.RegularExpressions.RegexOptions.IgnoreCase) Then


            MessageBox.Show("Invalid author name format. Periods (.) are only allowed for middle initials or suffixes (e.g., S. or Jr./Sr.).", "Input Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning)

            e.Cancel = True
        Else

            e.Cancel = False
        End If
    End Sub

    Private Sub txtauthor_MouseDown(sender As Object, e As MouseEventArgs) Handles txtauthor.MouseDown
        If e.Button = MouseButtons.Right Then
            e = Nothing
        End If
    End Sub
End Class