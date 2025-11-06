Imports System.Globalization
Imports System.Runtime.InteropServices
Imports MySql.Data.MySqlClient

Public Class Language
    Private Sub Language_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        TopMost = True
        Me.Refresh()
        refreshLanguage()
        DisablePaste_AllTextBoxes()
        AddHandler GlobalVarsModule.DatabaseUpdated, AddressOf OnDatabaseUpdated
    End Sub

    Public Sub refreshLanguage()
        Dim query As String = "SELECT * FROM `language_tbl`"
        GlobalVarsModule.AutoRefreshGrid(DataGridView1, query, 2000)
        SetupGridStyle()
    End Sub

    Private Async Sub OnDatabaseUpdated()
        Dim query As String = "SELECT * FROM `language_tbl`"
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

    Private Function IsValidLanguage(language As String) As Boolean

        If String.IsNullOrWhiteSpace(language) Then Return False


        For Each culture As CultureInfo In CultureInfo.GetCultures(CultureTypes.AllCultures)

            If culture.EnglishName.Equals(language, StringComparison.OrdinalIgnoreCase) _
          Or culture.NativeName.Equals(language, StringComparison.OrdinalIgnoreCase) Then
                Return True
            End If
        Next

        Return False
    End Function


    Private Sub Language_Shown(sender As Object, e As EventArgs) Handles MyBase.Shown

        DataGridView1.ClearSelection()

    End Sub

    Private Sub Language_FormClosed(sender As Object, e As FormClosedEventArgs) Handles Me.FormClosed

        MainForm.MaintenanceToolStripMenuItem.ForeColor = Color.White
        txtlanguage.Text = ""


    End Sub

    Private Sub btnadd_Click(sender As Object, e As EventArgs) Handles btnadd.Click

        Dim con As New MySqlConnection(GlobalVarsModule.connectionString)
        Dim lang As String = txtlanguage.Text.Trim
        Dim newID As Integer = 0

        If String.IsNullOrWhiteSpace(lang) Then
            MsgBox("Please fill in the required fields.", vbExclamation, "Missing Information")
            Exit Sub
        End If


        If lang.Length < 3 Then
            MsgBox("Language name must be 3 characters or more.", vbExclamation, "Input Error")
            Exit Sub
        End If

        If Not IsValidLanguage(lang) Then
            MessageBox.Show("Invalid language name. Please enter a valid language.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Exit Sub
        End If


        Try
            con.Open()

            Dim coms As New MySqlCommand("SELECT COUNT(*) FROM `language_tbl` WHERE `Language` = @language", con)
            coms.Parameters.AddWithValue("@language", lang)
            Dim count As Integer = Convert.ToInt32(coms.ExecuteScalar)

            If count > 0 Then
                MsgBox("This language is already exists.", vbExclamation, "Duplication not allowed.")
                Exit Sub
            End If

            Dim com As New MySqlCommand("INSERT INTO `language_tbl`(`Language`) VALUES (@language); SELECT LAST_INSERT_ID();", con)
            com.Parameters.AddWithValue("@language", lang)
            newID = Convert.ToInt32(com.ExecuteScalar())

            GlobalVarsModule.LogAudit(
            actionType:="ADD",
            formName:="LANGUAGE FORM",
            description:=$"Added new language: {lang}",
            recordID:=newID.ToString()
        )

            For Each form In Application.OpenForms
                If TypeOf form Is AuditTrail Then
                    DirectCast(form, AuditTrail).refreshaudit()
                End If
            Next

            For Each form In Application.OpenForms
                If TypeOf form Is Book Then
                    Dim book = DirectCast(form, Book)

                    book.cblang()
                End If
            Next

            MsgBox("Language added successfully", vbInformation)
            Language_Load(sender, e)

        Catch ex As Exception
            MsgBox(ex.Message, vbCritical)
        Finally
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
            txtlanguage.Clear()
        End Try

    End Sub

    Private Sub btnedit_Click(sender As Object, e As EventArgs) Handles btnedit.Click

        If DataGridView1.SelectedRows.Count > 0 Then

            Dim con As New MySqlConnection(GlobalVarsModule.connectionString)
            Dim selectedRow As DataGridViewRow = DataGridView1.SelectedRows(0)
            Dim ID As Integer = CInt(selectedRow.Cells("ID").Value)

            Dim oldLang As String = selectedRow.Cells("Language").Value.ToString()
            Dim lang As String = txtlanguage.Text.Trim

            If String.IsNullOrWhiteSpace(lang) Then
                MsgBox("Please fill in the required fields.", vbExclamation, "Missing Information")
                Exit Sub
            End If


            If lang.Length < 3 Then
                MsgBox("Language name must be 3 characters or more.", vbExclamation, "Input Error")
                Exit Sub
            End If


            'If oldLang.ToUpper() = lang.ToUpper() Then
            '    MsgBox("No changes were made.", vbExclamation, "No Update")
            '    Exit Sub
            'End If

            If Not IsValidLanguage(lang) Then
                MessageBox.Show("Invalid language name. Please enter a valid language.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Exit Sub
            End If

            Try
                con.Open()

                Dim coms As New MySqlCommand("SELECT COUNT(*) FROM `language_tbl` WHERE `Language` = @language AND `ID` <> @id", con)
                coms.Parameters.AddWithValue("@language", lang)
                coms.Parameters.AddWithValue("@id", ID)

                Dim count As Integer = Convert.ToInt32(coms.ExecuteScalar)

                If count > 0 Then
                    MsgBox("This language already exists.", vbExclamation, "Duplication not allowed.")
                    Exit Sub
                End If

                Dim com As New MySqlCommand("UPDATE `language_tbl` SET `Language` = @language WHERE `ID` = @id", con)
                com.Parameters.AddWithValue("@language", lang)
                com.Parameters.AddWithValue("@id", ID)
                com.ExecuteNonQuery()

                Dim comss As New MySqlCommand("UPDATE `book_tbl` SET `Language` = @newLanguage WHERE `Language` = @oldLanguage", con)
                comss.Parameters.AddWithValue("@newLanguage", lang)
                comss.Parameters.AddWithValue("@oldLanguage", oldLang)
                comss.ExecuteNonQuery()

                GlobalVarsModule.LogAudit(
                actionType:="UPDATE",
                formName:="LANGUAGE FORM",
                description:=$"Updated language ID {ID} from '{oldLang}' to '{lang}'",
                recordID:=ID.ToString(),
                oldValue:=$"Language: {oldLang}",
                newValue:=$"Language: {lang}"
            )

                For Each form In Application.OpenForms
                    If TypeOf form Is AuditTrail Then
                        DirectCast(form, AuditTrail).refreshaudit()
                    End If
                Next

                For Each form In Application.OpenForms
                    If TypeOf form Is Book Then
                        Dim book = DirectCast(form, Book)
                        book.cblang()
                    End If
                Next

                For Each form In Application.OpenForms
                    If TypeOf form Is MainForm Then
                        Dim load = DirectCast(form, MainForm)
                        load.loadsu()
                    End If
                Next

                MsgBox("Updated successfully!", vbInformation)
                Language_Load(sender, e)
                txtlanguage.Clear()

            Catch ex As Exception
                MsgBox(ex.Message, vbCritical)
            Finally
                If con.State = ConnectionState.Open Then
                    con.Close()
                End If
            End Try
        Else
            MsgBox("Please select a row to edit.", vbExclamation)
        End If

    End Sub

    Private Sub btndelete_Click(sender As Object, e As EventArgs) Handles btndelete.Click

        If DataGridView1.SelectedRows.Count > 0 Then

            Dim dialogResult As DialogResult = MessageBox.Show("Are you sure you want to delete this language?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)

            If dialogResult = DialogResult.Yes Then

                Dim con As New MySqlConnection(GlobalVarsModule.connectionString)
                Dim selectedRow As DataGridViewRow = DataGridView1.SelectedRows(0)
                Dim ID As Integer = CInt(selectedRow.Cells("ID").Value)
                Dim languageName As String = selectedRow.Cells("Language").Value.ToString().Trim()

                Try
                    con.Open()


                    Dim bookCom As New MySqlCommand("SELECT COUNT(*) FROM `book_tbl` WHERE Language = @language", con)
                    bookCom.Parameters.AddWithValue("@language", languageName)
                    Dim bookCount As Integer = CInt(bookCom.ExecuteScalar())

                    If bookCount > 0 Then
                        MessageBox.Show("Cannot delete this language. It is assigned to " & bookCount & " book(s).", "Information", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        Return
                    End If


                    Dim delete As New MySqlCommand("DELETE FROM `language_tbl` WHERE `ID` = @id", con)
                    delete.Parameters.AddWithValue("@id", ID)
                    delete.ExecuteNonQuery()

                    GlobalVarsModule.LogAudit(
                        actionType:="DELETE",
                        formName:="LANGUAGE FORM",
                        description:=$"Deleted language: {languageName}",
                        recordID:=ID.ToString()
                    )

                    For Each form In Application.OpenForms
                        If TypeOf form Is AuditTrail Then
                            DirectCast(form, AuditTrail).refreshaudit()
                        End If
                    Next

                    For Each form In Application.OpenForms
                        If TypeOf form Is Book Then
                            Dim book = DirectCast(form, Book)

                            book.cblang()
                        End If
                    Next

                    MsgBox("Language deleted successfully.", vbInformation)
                    Language_Load(sender, e)
                    txtlanguage.Clear()

                    Dim count As New MySqlCommand("SELECT COUNT(*) FROM `language_tbl`", con)
                    Dim rowCount As Long = CLng(count.ExecuteScalar())

                    If rowCount = 0 Then
                        Dim reset As New MySqlCommand("ALTER TABLE `language_tbl` AUTO_INCREMENT = 1", con)
                        reset.ExecuteNonQuery()
                    End If

                Catch ex As Exception
                    MsgBox(ex.Message, vbCritical)
                Finally
                    If con.State = ConnectionState.Open Then
                        con.Close()
                    End If
                End Try
            End If
        End If
    End Sub

    Private Sub DataGridView1_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellClick

        If e.RowIndex >= 0 Then

            Dim row = DataGridView1.Rows(e.RowIndex)
            txtlanguage.Text = row.Cells("Language").Value.ToString

        End If

    End Sub

    Private Sub txtsearch_TextChanged(sender As Object, e As EventArgs) Handles txtsearch.TextChanged


        Dim dt As DataTable = DirectCast(DataGridView1.DataSource, DataTable)
        If dt IsNot Nothing Then
            If txtsearch.Text.Trim() <> "" Then
                Dim filter As String = String.Format("Language LIKE '*{0}*'", txtsearch.Text.Trim())
                dt.DefaultView.RowFilter = filter
            Else
                dt.DefaultView.RowFilter = ""
            End If
        End If

    End Sub

    Private Sub txtlanguage_KeyPress(sender As Object, e As KeyPressEventArgs) Handles txtlanguage.KeyPress

        If Not Char.IsLetter(e.KeyChar) AndAlso Not Char.IsControl(e.KeyChar) AndAlso Not Char.IsWhiteSpace(e.KeyChar) Then
            e.Handled = True
        End If

    End Sub

    Private Sub txtlanguage_KeyDown(sender As Object, e As KeyEventArgs) Handles txtlanguage.KeyDown

        If e.Control AndAlso (e.KeyCode = Keys.V Or e.KeyCode = Keys.C Or e.KeyCode = Keys.X) Then
            e.SuppressKeyPress = True
        End If



    End Sub

    Private Sub txtsearch_KeyDown(sender As Object, e As KeyEventArgs) Handles txtsearch.KeyDown

        If e.Control AndAlso (e.KeyCode = Keys.V Or e.KeyCode = Keys.C Or e.KeyCode = Keys.X) Then
            e.SuppressKeyPress = True
        End If

    End Sub

    Private Sub Language_KeyDown(sender As Object, e As KeyEventArgs) Handles MyBase.KeyDown
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