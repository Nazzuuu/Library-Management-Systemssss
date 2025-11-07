Imports MySql.Data.MySqlClient
Public Class Genre
    Private Sub Genre_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        DisablePaste_AllTextBoxes()
        TopMost = True
        Me.Refresh()
        refreshGenre()
        AddHandler GlobalVarsModule.DatabaseUpdated, AddressOf OnDatabaseUpdated
    End Sub

    Public Sub refreshGenre()
        Dim query As String = "SELECT * FROM `genre_tbl`"
        GlobalVarsModule.AutoRefreshGrid(DataGridView1, query, 2000)
        SetupGridStyle()
        txtgenre.Text = ""
    End Sub

    Private Async Sub OnDatabaseUpdated()
        Dim query As String = "SELECT * FROM `genre_tbl`"
        Await GlobalVarsModule.LoadToGridAsync(DataGridView1, query)
        SetupGridStyle()
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
        Catch
        End Try
    End Sub


    Private Sub Genre_Shown(sender As Object, e As EventArgs) Handles MyBase.Shown

        DataGridView1.ClearSelection()


    End Sub

    Private Sub Genre_FormClosed(sender As Object, e As FormClosedEventArgs) Handles Me.FormClosed



        MainForm.MaintenanceToolStripMenuItem.ForeColor = Color.White
        txtgenre.Text = ""


    End Sub

    Private Sub btnadd_Click(sender As Object, e As EventArgs) Handles btnadd.Click

        Dim con As New MySqlConnection(GlobalVarsModule.connectionString)
        Dim genre As String = txtgenre.Text.Trim()
        Dim insertedID As Integer = 0

        If String.IsNullOrWhiteSpace(genre) Then
            MsgBox("Please fill in the required fields.", vbExclamation, "Missing Information")
            Exit Sub
        End If


        If genre.Length < 3 Then
            MsgBox("Genre must be 3 characters or more.", vbExclamation, "Input Error")
            Exit Sub
        End If

        For Each c As Char In genre
            If Not Char.IsLetter(c) AndAlso Not Char.IsWhiteSpace(c) AndAlso c <> "-" Then
                MsgBox("Genre name can only contain letters, spaces, and hyphens (e.g., Sci-Fi).", vbExclamation, "Invalid Genre Format")
                Exit Sub
            End If
        Next


        Try
            con.Open()

            Dim coms As New MySqlCommand("SELECT COUNT(*) FROM `genre_tbl` WHERE `Genre` = @genre ", con)
            coms.Parameters.AddWithValue("@genre", genre)
            Dim count As Integer = Convert.ToInt32(coms.ExecuteScalar())

            If count > 0 Then
                MsgBox("Genre already exists.", vbExclamation, "Duplication is not allowed.")
                Exit Sub
            End If

            Dim com As New MySqlCommand("INSERT INTO `genre_tbl`(`Genre`) VALUES (@genre); SELECT LAST_INSERT_ID()", con)
            com.Parameters.AddWithValue("@genre", genre)
            insertedID = Convert.ToInt32(com.ExecuteScalar())

            GlobalVarsModule.LogAudit(
            actionType:="ADD",
            formName:="GENRE FORM",
            description:=$"Added new Genre: {genre}",
            recordID:=insertedID.ToString()
        )

            For Each form In Application.OpenForms
                If TypeOf form Is Book Then
                    Dim book = DirectCast(form, Book)
                    book.cbgenree()
                End If
            Next

            For Each form In Application.OpenForms
                If TypeOf form Is AuditTrail Then
                    Dim load = DirectCast(form, AuditTrail)
                    load.refreshaudit()
                End If
            Next

            MsgBox("Genre added successfully", vbInformation)
            Genre_Load(sender, e)

        Catch ex As Exception
            MsgBox(ex.Message, vbCritical)
        Finally
            txtgenre.Clear()
        End Try

    End Sub

    Private Sub btnedit_Click(sender As Object, e As EventArgs) Handles btnedit.Click

        If DataGridView1.SelectedRows.Count > 0 Then

            Dim selectedRow As DataGridViewRow = DataGridView1.SelectedRows(0)
            Dim ID As Integer = CInt(selectedRow.Cells("ID").Value)

            Dim oldGenre As String = selectedRow.Cells("Genre").Value.ToString().Trim()
            Dim newGenre As String = txtgenre.Text.Trim()

            If String.IsNullOrWhiteSpace(newGenre) Then
                MsgBox("Please fill in the required fields.", vbExclamation, "Missing Information")
                Exit Sub
            End If


            If newGenre.Length < 3 Then
                MsgBox("New Genre must be 3 characters or more.", vbExclamation, "Input Error")
                Exit Sub
            End If


            For Each c As Char In newGenre
                If Not Char.IsLetter(c) AndAlso Not Char.IsWhiteSpace(c) AndAlso c <> "-" Then
                    MsgBox("New Genre name can only contain letters (e.g., Sci-Fi).", vbExclamation, "Invalid Genre Format")
                    Exit Sub
                End If
            Next


            If String.Equals(oldGenre, newGenre, StringComparison.OrdinalIgnoreCase) Then
                MsgBox("The genre name is the same as the current one.", vbInformation)
                Exit Sub
            End If

            Using con As New MySqlConnection(GlobalVarsModule.connectionString)
                Try
                    con.Open()


                    Dim coms As New MySqlCommand("SELECT COUNT(*) FROM `genre_tbl` WHERE `Genre` = @newGenre AND ID <> @id", con)
                    coms.Parameters.AddWithValue("@newGenre", newGenre)
                    coms.Parameters.AddWithValue("@id", ID)
                    Dim count As Integer = Convert.ToInt32(coms.ExecuteScalar())

                    If count > 0 Then
                        MsgBox("Genre already exists.", vbExclamation, "Duplication is not allowed.")
                        Exit Sub
                    End If


                    Dim com As New MySqlCommand("UPDATE `genre_tbl` SET `Genre` = @newGenre WHERE `ID` = @id", con)
                    com.Parameters.AddWithValue("@newGenre", newGenre)
                    com.Parameters.AddWithValue("@id", ID)
                    com.ExecuteNonQuery()


                    Dim comsis As New MySqlCommand("UPDATE `book_tbl` SET `Genre` = @newGenre WHERE `Genre` = @oldGenre", con)
                    comsis.Parameters.AddWithValue("@newGenre", newGenre)
                    comsis.Parameters.AddWithValue("@oldGenre", oldGenre)
                    comsis.ExecuteNonQuery()


                    GlobalVarsModule.LogAudit(
                actionType:="UPDATE",
                formName:="GENRE FORM",
                description:=$"Updated Genre Name from '{oldGenre}' to '{newGenre}'.",
                recordID:=ID.ToString(),
                oldValue:=oldGenre,
                newValue:=newGenre
            )

                    For Each form In Application.OpenForms
                        If TypeOf form Is Book Then

                            DirectCast(form, Book).cbgenree()
                        End If
                        If TypeOf form Is AuditTrail Then
                            DirectCast(form, AuditTrail).refreshaudit()
                        End If
                        If TypeOf form Is MainForm Then
                            DirectCast(form, MainForm).loadsu()
                        End If
                    Next

                    MsgBox("Updated successfully!", vbInformation)


                    Me.Genre_Load(sender, e)
                    txtgenre.Clear()

                Catch ex As Exception
                    MsgBox(ex.Message, vbCritical)
                End Try
            End Using

        Else
            MsgBox("Please select a row to edit.", vbExclamation)
        End If
    End Sub

    Private Sub btndelete_Click(sender As Object, e As EventArgs) Handles btndelete.Click

        If DataGridView1.SelectedRows.Count > 0 Then

            Dim dialogResult As DialogResult = MessageBox.Show("Are you sure you want to delete this genre?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)

            If dialogResult = DialogResult.Yes Then

                Dim con As New MySqlConnection(GlobalVarsModule.connectionString)
                Dim selectedRow As DataGridViewRow = DataGridView1.SelectedRows(0)
                Dim ID As Integer = CInt(selectedRow.Cells("ID").Value)
                Dim genreName As String = selectedRow.Cells("Genre").Value.ToString().Trim()

                Try
                    con.Open()


                    Dim bookCom As New MySqlCommand("SELECT COUNT(*) FROM `book_tbl` WHERE Genre = @genre", con)
                    bookCom.Parameters.AddWithValue("@genre", genreName)
                    Dim bookCount As Integer = CInt(bookCom.ExecuteScalar())

                    If bookCount > 0 Then
                        MessageBox.Show("Cannot delete this genre. It is assigned to " & bookCount & " book(s).", "Information", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        Return
                    End If


                    Dim delete As New MySqlCommand("DELETE FROM `genre_tbl` WHERE `ID` = @id", con)
                    delete.Parameters.AddWithValue("@id", ID)
                    delete.ExecuteNonQuery()

                    GlobalVarsModule.LogAudit(
                        actionType:="DELETE",
                        formName:="GENRE FORM",
                        description:=$"Deleted Genre: {genreName}",
                        recordID:=ID.ToString()
                    )

                    For Each form In Application.OpenForms
                        If TypeOf form Is Book Then
                            Dim book = DirectCast(form, Book)
                            book.cbgenree()
                        End If
                    Next

                    For Each form In Application.OpenForms
                        If TypeOf form Is AuditTrail Then
                            Dim load = DirectCast(form, AuditTrail)
                            load.refreshaudit()
                        End If
                    Next

                    MsgBox("Genre deleted successfully.", vbInformation)
                    Genre_Load(sender, e)
                    txtgenre.Clear()

                    Dim count As New MySqlCommand("SELECT COUNT(*) FROM `genre_tbl`", con)
                    Dim rowCount As Long = CLng(count.ExecuteScalar())

                    If rowCount = 0 Then
                        Dim reset As New MySqlCommand("ALTER TABLE `genre_tbl` AUTO_INCREMENT = 1", con)
                        reset.ExecuteNonQuery()
                    End If

                Catch ex As Exception
                    MsgBox(ex.Message, vbCritical)
                End Try
            End If
        End If
    End Sub



    Private Sub txtsearch_TextChanged(sender As Object, e As EventArgs) Handles txtsearch.TextChanged

        HandleAutoRefreshPause(DataGridView1, txtsearch)

        Dim dt As DataTable = DirectCast(DataGridView1.DataSource, DataTable)
        If dt IsNot Nothing Then
            If txtsearch.Text.Trim() <> "" Then
                Dim filter As String = String.Format("Genre LIKE '*{0}*'", txtsearch.Text.Trim())
                dt.DefaultView.RowFilter = filter
            Else
                dt.DefaultView.RowFilter = ""
            End If
        End If

    End Sub

    Private Sub DataGridView1_CellClick_1(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellClick


        If e.RowIndex >= 0 Then

            Dim row = DataGridView1.Rows(e.RowIndex)
            txtgenre.Text = row.Cells("Genre").Value.ToString()

        End If

    End Sub

    Private Sub txtgenre_KeyDown(sender As Object, e As KeyEventArgs) Handles txtgenre.KeyDown

        If e.Control AndAlso (e.KeyCode = Keys.V Or e.KeyCode = Keys.C Or e.KeyCode = Keys.X) Then
            e.SuppressKeyPress = True
        End If

    End Sub

    Private Sub txtgenre_KeyPress(sender As Object, e As KeyPressEventArgs) Handles txtgenre.KeyPress

        If e.KeyChar = " "c AndAlso String.IsNullOrEmpty(txtgenre.Text) Then
            e.Handled = True


        End If

        If Not Char.IsLetter(e.KeyChar) AndAlso Not Char.IsControl(e.KeyChar) AndAlso Not Char.IsWhiteSpace(e.KeyChar) Then
            e.Handled = True
        End If

    End Sub

    Private Sub txtsearch_KeyDown(sender As Object, e As KeyEventArgs) Handles txtsearch.KeyDown

        If e.Control AndAlso (e.KeyCode = Keys.V Or e.KeyCode = Keys.C Or e.KeyCode = Keys.X) Then
            e.SuppressKeyPress = True
        End If

    End Sub

    Private Sub Genre_KeyDown(sender As Object, e As KeyEventArgs) Handles MyBase.KeyDown
        If e.KeyCode = Keys.Escape Then
            Me.Close()
        End If
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


End Class