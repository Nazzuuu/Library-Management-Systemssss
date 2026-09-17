Imports MySql.Data.MySqlClient

Public Class Genre

    Private Sub Genre_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        DisablePaste_AllTextBoxes()
        TopMost = True
        Me.Refresh()
        refreshGenre()
        AddHandler GlobalVarsModule.DatabaseUpdated, AddressOf OnDatabaseUpdated
        GlobalVarsModule.EnableCapitalizeFirstLetterForControls(Me)
    End Sub

    Public Sub refreshGenre()
        Dim query As String = "SELECT * FROM `genre_tbl`"
        GlobalVarsModule.AutoRefreshGrid(DataGridView1, query, 2000)
        SetupGridStyle()
        txtgenre.Clear()
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

    Private Sub DataGridView1_DataBindingComplete(sender As Object, e As DataGridViewBindingCompleteEventArgs) Handles DataGridView1.DataBindingComplete

        If DataGridView1.Columns.Contains("ID") Then
            DataGridView1.Columns("ID").Visible = False
        End If

        If DataGridView1.Columns.Contains("Genre") Then
            DataGridView1.Columns("Genre").DisplayIndex = 0
        End If

        If DataGridView1.Columns.Contains("Edit") Then
            DataGridView1.Columns("Edit").DisplayIndex = DataGridView1.Columns.Count - 2
        End If

        If DataGridView1.Columns.Contains("Delete") Then
            DataGridView1.Columns("Delete").DisplayIndex = DataGridView1.Columns.Count - 1
        End If

        DataGridView1.ClearSelection()
        DataGridView1.CurrentCell = Nothing
    End Sub

    Private Sub Genre_Shown(sender As Object, e As EventArgs) Handles MyBase.Shown
        DataGridView1.ClearSelection()
        DataGridView1.CurrentCell = Nothing
    End Sub

    Private Sub Genre_FormClosed(sender As Object, e As FormClosedEventArgs) Handles Me.FormClosed
        MainForm.MaintenanceToolStripMenuItem.ForeColor = Color.White
        txtgenre.Clear()
    End Sub

    Private Sub btnadd_Click(sender As Object, e As EventArgs) Handles btnadd.Click

        Dim genre As String = txtgenre.Text.Trim()

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

        Using con As New MySqlConnection(GlobalVarsModule.connectionString)

            Try
                con.Open()


                If DataGridView1.SelectedRows.Count > 0 Then

                    Dim selectedRow = DataGridView1.SelectedRows(0)
                    Dim ID As Integer = Convert.ToInt32(selectedRow.Cells("ID").Value)
                    Dim oldGenre As String = selectedRow.Cells("Genre").Value.ToString().Trim()

                    If String.Equals(oldGenre, genre, StringComparison.OrdinalIgnoreCase) Then
                        MsgBox("The genre name is the same as the current one.", vbInformation)
                        Exit Sub
                    End If

                    Dim check As New MySqlCommand(
                        "SELECT COUNT(*) FROM `genre_tbl` WHERE `Genre` = @genre AND `ID` <> @id", con)

                    check.Parameters.AddWithValue("@genre", genre)
                    check.Parameters.AddWithValue("@id", ID)

                    If Convert.ToInt32(check.ExecuteScalar()) > 0 Then
                        MsgBox("Genre already exists.", vbExclamation, "Duplication is not allowed.")
                        Exit Sub
                    End If

                    Dim update As New MySqlCommand(
                        "UPDATE `genre_tbl` SET `Genre` = @genre WHERE `ID` = @id", con)

                    update.Parameters.AddWithValue("@genre", genre)
                    update.Parameters.AddWithValue("@id", ID)
                    update.ExecuteNonQuery()

                    Dim updateBooks As New MySqlCommand(
                        "UPDATE `book_tbl` SET `Genre` = @newGenre WHERE `Genre` = @oldGenre", con)

                    updateBooks.Parameters.AddWithValue("@newGenre", genre)
                    updateBooks.Parameters.AddWithValue("@oldGenre", oldGenre)
                    updateBooks.ExecuteNonQuery()

                    GlobalVarsModule.LogAudit(
                        actionType:="UPDATE",
                        formName:="GENRE FORM",
                        description:=$"Updated Genre Name from '{oldGenre}' to '{genre}'.",
                        recordID:=ID.ToString(),
                        oldValue:=oldGenre,
                        newValue:=genre
                    )

                    For Each form In Application.OpenForms
                        If TypeOf form Is Book Then
                            DirectCast(form, Book).cbgenree()
                        ElseIf TypeOf form Is AuditTrail Then
                            DirectCast(form, AuditTrail).refreshaudit()
                        ElseIf TypeOf form Is MainForm Then
                            DirectCast(form, MainForm).loadsu()
                        End If
                    Next

                    MsgBox("Genre updated successfully!", vbInformation)

                Else

                    ' ADD
                    Dim check As New MySqlCommand(
                        "SELECT COUNT(*) FROM `genre_tbl` WHERE `Genre` = @genre", con)

                    check.Parameters.AddWithValue("@genre", genre)

                    If Convert.ToInt32(check.ExecuteScalar()) > 0 Then
                        MsgBox("Genre already exists.", vbExclamation, "Duplication is not allowed.")
                        Exit Sub
                    End If

                    Dim insert As New MySqlCommand(
                        "INSERT INTO `genre_tbl` (`Genre`) VALUES (@genre); SELECT LAST_INSERT_ID()", con)

                    insert.Parameters.AddWithValue("@genre", genre)

                    Dim insertedID As Integer = Convert.ToInt32(insert.ExecuteScalar())

                    GlobalVarsModule.LogAudit(
                        actionType:="ADD",
                        formName:="GENRE FORM",
                        description:=$"Added new Genre: {genre}",
                        recordID:=insertedID.ToString()
                    )

                    For Each form In Application.OpenForms
                        If TypeOf form Is Book Then
                            DirectCast(form, Book).cbgenree()
                        ElseIf TypeOf form Is AuditTrail Then
                            DirectCast(form, AuditTrail).refreshaudit()
                        End If
                    Next

                    MsgBox("Genre added successfully", vbInformation)

                End If

                txtgenre.Clear()
                DataGridView1.ClearSelection()
                DataGridView1.CurrentCell = Nothing
                refreshGenre()

            Catch ex As Exception
                MsgBox(ex.Message, vbCritical)
            End Try

        End Using

    End Sub

    Private Sub DataGridView1_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellContentClick

        If e.RowIndex < 0 Then Exit Sub

        Dim row = DataGridView1.Rows(e.RowIndex)

        If DataGridView1.Columns(e.ColumnIndex).Name = "Edit" Then

            DataGridView1.ClearSelection()
            row.Selected = True
            DataGridView1.CurrentCell = row.Cells(DataGridView1.Columns.GetFirstColumn(DataGridViewElementStates.Visible).Index)

            txtgenre.Text = row.Cells("Genre").Value.ToString()

            Exit Sub
        End If

        If DataGridView1.Columns(e.ColumnIndex).Name = "Delete" Then

            Dim result = MessageBox.Show(
        "Are you sure you want to delete this genre?",
        "Confirm Delete",
        MessageBoxButtons.YesNo,
        MessageBoxIcon.Warning
    )

            If result <> DialogResult.Yes Then Exit Sub

            Dim genreName As String = row.Cells("Genre").Value.ToString().Trim()

            Try

                Dim ID As Integer

                If row.Cells("ID").Value Is Nothing OrElse
           IsDBNull(row.Cells("ID").Value) OrElse
           Not Integer.TryParse(row.Cells("ID").Value.ToString(), ID) Then

                    MsgBox("Invalid Genre ID.", vbCritical)
                    Exit Sub
                End If

                Using con As New MySqlConnection(GlobalVarsModule.connectionString)

                    con.Open()

                    Dim bookCom As New MySqlCommand(
                "SELECT COUNT(*) FROM `book_tbl` WHERE `Genre` = @genre", con)

                    bookCom.Parameters.AddWithValue("@genre", genreName)

                    Dim bookCount As Integer = Convert.ToInt32(bookCom.ExecuteScalar())

                    If bookCount > 0 Then
                        MessageBox.Show(
                    "Cannot delete this genre. It is assigned to " &
                    bookCount & " book(s).",
                    "Information",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                )
                        Exit Sub
                    End If

                    Dim delete As New MySqlCommand(
                "DELETE FROM `genre_tbl` WHERE `ID` = @id", con)

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
                            DirectCast(form, Book).cbgenree()
                        ElseIf TypeOf form Is AuditTrail Then
                            DirectCast(form, AuditTrail).refreshaudit()
                        End If
                    Next

                    Dim count As New MySqlCommand(
                "SELECT COUNT(*) FROM `genre_tbl`", con)

                    Dim rowCount As Long = Convert.ToInt64(count.ExecuteScalar())

                    If rowCount = 0 Then
                        Dim reset As New MySqlCommand(
                    "ALTER TABLE `genre_tbl` AUTO_INCREMENT = 1", con)

                        reset.ExecuteNonQuery()
                    End If

                End Using

                MsgBox("Genre deleted successfully.", vbInformation)

                txtgenre.Clear()
                DataGridView1.ClearSelection()
                DataGridView1.CurrentCell = Nothing
                refreshGenre()

            Catch ex As MySqlException
                MsgBox("Database Error: " & ex.Message, vbCritical, "Delete Error")

            Catch ex As Exception
                MsgBox(ex.Message, vbCritical, "Delete Error")

            End Try

            Exit Sub

        End If

    End Sub

    Private Sub DataGridView1_CellClick_1(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellClick

        If e.RowIndex >= 0 Then
            Dim row = DataGridView1.Rows(e.RowIndex)
            txtgenre.Text = row.Cells("Genre").Value.ToString()
        End If

    End Sub

    Private Sub txtsearch_TextChanged(sender As Object, e As EventArgs) Handles txtsearch.TextChanged

        HandleAutoRefreshPause(DataGridView1, txtsearch)

        Dim dt As DataTable = TryCast(DataGridView1.DataSource, DataTable)

        If dt IsNot Nothing Then
            If txtsearch.Text.Trim() <> "" Then
                Dim filter As String =
                    String.Format("Genre LIKE '*{0}*'", txtsearch.Text.Trim())

                dt.DefaultView.RowFilter = filter
            Else
                dt.DefaultView.RowFilter = ""
            End If
        End If

    End Sub

    Private Sub txtgenre_KeyDown(sender As Object, e As KeyEventArgs) Handles txtgenre.KeyDown

        If e.Control AndAlso
           (e.KeyCode = Keys.V OrElse
            e.KeyCode = Keys.C OrElse
            e.KeyCode = Keys.X) Then

            e.SuppressKeyPress = True
        End If

        If e.KeyCode = Keys.Enter Then
            e.SuppressKeyPress = True
            btnadd.PerformClick()
        End If

    End Sub

    Private Sub txtgenre_KeyPress(sender As Object, e As KeyPressEventArgs) Handles txtgenre.KeyPress

        If e.KeyChar = " "c AndAlso String.IsNullOrEmpty(txtgenre.Text) Then
            e.Handled = True
            Exit Sub
        End If

        If Not Char.IsLetter(e.KeyChar) AndAlso
           Not Char.IsControl(e.KeyChar) AndAlso
           Not Char.IsWhiteSpace(e.KeyChar) AndAlso
           e.KeyChar <> "-"c Then

            e.Handled = True
        End If

    End Sub

    Private Sub txtsearch_KeyDown(sender As Object, e As KeyEventArgs) Handles txtsearch.KeyDown

        If e.Control AndAlso
           (e.KeyCode = Keys.V OrElse
            e.KeyCode = Keys.C OrElse
            e.KeyCode = Keys.X) Then

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

        If (e.Control AndAlso e.KeyCode = Keys.V) OrElse
           (e.Shift AndAlso e.KeyCode = Keys.Insert) Then

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