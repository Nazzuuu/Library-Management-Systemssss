Imports MySql.Data.MySqlClient
Imports System.Data

Public Class Publisher
    Private Sub Publisher_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        DisablePaste_AllTextBoxes()
        TopMost = True
        Me.Refresh()
        refreshPublisher()
        AddHandler GlobalVarsModule.DatabaseUpdated, AddressOf OnDatabaseUpdated
    End Sub

    Public Sub refreshPublisher()
        Dim query As String = "SELECT * FROM `publisher_tbl`"
        GlobalVarsModule.AutoRefreshGrid(DataGridView1, query, 2000)
        SetupGridStyle()
    End Sub

    Private Async Sub OnDatabaseUpdated()
        Dim query As String = "SELECT * FROM `publisher_tbl`"
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


    Private Sub Publisher_Shown(sender As Object, e As EventArgs) Handles MyBase.Shown

        DataGridView1.ClearSelection()

    End Sub

    Private Sub Publisher_FormClosed(sender As Object, e As FormClosedEventArgs) Handles Me.FormClosed



        MainForm.MaintenanceToolStripMenuItem.ForeColor = Color.White

        DataGridView1.EnableHeadersVisualStyles = False
        DataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(207, 58, 109)
        DataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White

        clear()
    End Sub

    Public Sub clear()

        txtpublisher.Text = ""

    End Sub

    Private Sub btnadd_Click(sender As Object, e As EventArgs) Handles btnadd.Click
        Dim con As New MySqlConnection(GlobalVarsModule.connectionString)
        Dim newID As Integer = 0
        Dim publisher As String = txtpublisher.Text.Trim()

        If String.IsNullOrWhiteSpace(publisher) Then
            MsgBox("Please fill in the required fields.", vbExclamation, "Missing Information")
            Exit Sub
        End If

        If publisher.Length < 2 Then
            MsgBox("Publisher Name must be 2 characters or more.", vbExclamation, "Input Error")
            Exit Sub
        End If

        Try
            con.Open()


            Dim checkCmd As New MySqlCommand("SELECT COUNT(*) FROM publisher_tbl WHERE PublisherName = @publisher", con)
            checkCmd.Parameters.AddWithValue("@publisher", publisher)
            Dim count As Integer = Convert.ToInt32(checkCmd.ExecuteScalar())

            If count > 0 Then
                MsgBox("Publisher name already exists.", vbExclamation, "Warning")
                Exit Sub
            End If


            Dim insertCmd As New MySqlCommand("
            INSERT INTO publisher_tbl (PublisherName) 
            VALUES (@publisher);
            SELECT LAST_INSERT_ID();
        ", con)
            insertCmd.Parameters.AddWithValue("@publisher", publisher)

            newID = Convert.ToInt32(insertCmd.ExecuteScalar())

            GlobalVarsModule.LogAudit(
            actionType:="ADD",
            formName:="PUBLISHER FORM",
            description:=$"Added new publisher: {publisher}",
            recordID:=newID.ToString()
        )


            For Each form In Application.OpenForms
                If TypeOf form Is AuditTrail Then
                    DirectCast(form, AuditTrail).refreshaudit()
                End If
            Next

            For Each form In Application.OpenForms
                If TypeOf form Is Book Then
                    DirectCast(form, Book).cbpublisherr()
                End If
            Next

            MsgBox("Publisher added successfully", vbInformation)
            Publisher_Load(sender, e)

        Catch ex As Exception
            MsgBox("Error adding publisher: " & ex.Message, vbCritical)
        Finally
            If con.State = ConnectionState.Open Then con.Close()
            clear()
        End Try
    End Sub


    Private Sub btnedit_Click(sender As Object, e As EventArgs) Handles btnedit.Click
        If DataGridView1.SelectedRows.Count = 0 Then
            MsgBox("Please select a row to edit.", vbExclamation)
            Exit Sub
        End If

        Dim con As New MySqlConnection(GlobalVarsModule.connectionString)
        Dim selectedRow As DataGridViewRow = DataGridView1.SelectedRows(0)

        Dim ID As Integer = CInt(selectedRow.Cells("ID").Value)
        Dim oldPub As String = selectedRow.Cells("PublisherName").Value.ToString().Trim()
        Dim newPub As String = txtpublisher.Text.Trim()

  
        If String.IsNullOrWhiteSpace(newPub) Then
            MsgBox("Please fill in the required fields.", vbExclamation, "Missing Information")
            Exit Sub
        End If

        If newPub.Length < 2 Then
            MsgBox("Publisher Name must be 2 characters or more.", vbExclamation, "Input Error")
            Exit Sub
        End If

        Try
            con.Open()


            Dim checkCmd As New MySqlCommand("
            SELECT COUNT(*) FROM publisher_tbl 
            WHERE PublisherName = @publisher AND ID <> @id
        ", con)
            checkCmd.Parameters.AddWithValue("@publisher", newPub)
            checkCmd.Parameters.AddWithValue("@id", ID)
            Dim count As Integer = Convert.ToInt32(checkCmd.ExecuteScalar())

            If count > 0 Then
                MsgBox("Publisher name already exists.", vbExclamation, "Warning")
                Exit Sub
            End If


            Dim updateCmd As New MySqlCommand("
            UPDATE publisher_tbl 
            SET PublisherName = @publisher 
            WHERE ID = @id
        ", con)
            updateCmd.Parameters.AddWithValue("@publisher", newPub)
            updateCmd.Parameters.AddWithValue("@id", ID)
            updateCmd.ExecuteNonQuery()


            Dim updateBookCmd As New MySqlCommand("
            UPDATE book_tbl 
            SET Publisher = @newPublisher 
            WHERE Publisher = @oldPublisher
        ", con)
            updateBookCmd.Parameters.AddWithValue("@newPublisher", newPub)
            updateBookCmd.Parameters.AddWithValue("@oldPublisher", oldPub)
            updateBookCmd.ExecuteNonQuery()


            GlobalVarsModule.LogAudit(
            actionType:="UPDATE",
            formName:="PUBLISHER FORM",
            description:=$"Updated publisher ID {ID} from '{oldPub}' to '{newPub}'",
            recordID:=ID.ToString(),
            oldValue:=$"Publisher: {oldPub}",
            newValue:=$"Publisher: {newPub}"
        )


            For Each form In Application.OpenForms
                If TypeOf form Is Book Then
                    DirectCast(form, Book).refreshbook()
                    DirectCast(form, Book).cbpublisherr()
                ElseIf TypeOf form Is AuditTrail Then
                    DirectCast(form, AuditTrail).refreshaudit()
                ElseIf TypeOf form Is MainForm Then
                    DirectCast(form, MainForm).loadsu()
                End If
            Next

            MsgBox("Publisher updated successfully", vbInformation)
            Publisher_Load(sender, e)

        Catch ex As Exception
            MsgBox("Error updating publisher: " & ex.Message, vbCritical)
        Finally
            If con.State = ConnectionState.Open Then con.Close()
            clear()
        End Try
    End Sub


    Private Sub btndelete_Click(sender As Object, e As EventArgs) Handles btndelete.Click

        If DataGridView1.SelectedRows.Count > 0 Then

            Dim dialogResult As DialogResult = MessageBox.Show("Are you sure you want to delete this publisher?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)

            If dialogResult = DialogResult.Yes Then

                Dim con As New MySqlConnection(GlobalVarsModule.connectionString)
                Dim selectedRow As DataGridViewRow = DataGridView1.SelectedRows(0)
                Dim ID As Integer = CInt(selectedRow.Cells("ID").Value)
                Dim publisherName As String = selectedRow.Cells("PublisherName").Value.ToString().Trim()

                Try
                    con.Open()


                    Dim bookCom As New MySqlCommand("SELECT COUNT(*) FROM `book_tbl` WHERE Publisher = @publisher", con)
                    bookCom.Parameters.AddWithValue("@publisher", publisherName)
                    Dim bookCount As Integer = CInt(bookCom.ExecuteScalar())

                    If bookCount > 0 Then
                        MessageBox.Show("Cannot delete this publisher. They are assigned to " & bookCount & " book(s).", "Information", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        Return
                    End If


                    Dim delete As New MySqlCommand("DELETE FROM `publisher_tbl` WHERE `ID` = @id", con)
                    delete.Parameters.AddWithValue("@id", ID)
                    delete.ExecuteNonQuery()

                    GlobalVarsModule.LogAudit(
                        actionType:="DELETE",
                        formName:="PUBLISHER FORM",
                        description:=$"Deleted publisher: {publisherName}",
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
                            book.cbpublisherr()
                        End If
                    Next

                    MsgBox("Publisher deleted successfully.", vbInformation)
                    Publisher_Load(sender, e)
                    clear()

                    Dim count As New MySqlCommand("SELECT COUNT(*) FROM `publisher_tbl`", con)
                    Dim rowCount As Long = CLng(count.ExecuteScalar())

                    If rowCount = 0 Then
                        Dim reset As New MySqlCommand("ALTER TABLE `publisher_tbl` AUTO_INCREMENT = 1", con)
                        reset.ExecuteNonQuery()
                    End If

                Catch ex As Exception
                    MsgBox(ex.Message, vbCritical)
                Finally
                    If con.State = ConnectionState.Open Then con.Close()
                End Try
            End If
        End If
    End Sub

    Private Sub txtsearch_TextChanged(sender As Object, e As EventArgs) Handles txtsearch.TextChanged

        HandleAutoRefreshPause(DataGridView1, txtsearch)

        Dim dt As DataTable = DirectCast(DataGridView1.DataSource, DataTable)
        If dt IsNot Nothing Then
            If txtsearch.Text.Trim() <> "" Then
                Dim filter As String = String.Format("PublisherName LIKE '*{0}*'", txtsearch.Text.Trim())
                dt.DefaultView.RowFilter = filter
            Else
                dt.DefaultView.RowFilter = ""
            End If
        End If

    End Sub


    Private Sub txtpublisher_KeyDown(sender As Object, e As KeyEventArgs) Handles txtpublisher.KeyDown

        If e.Control AndAlso (e.KeyCode = Keys.V Or e.KeyCode = Keys.C Or e.KeyCode = Keys.X) Then
            e.SuppressKeyPress = True
        End If

    End Sub



    Private Sub txtcontact_KeyPress(sender As Object, e As KeyPressEventArgs)


        If Not Char.IsDigit(e.KeyChar) And Not Char.IsControl(e.KeyChar) Then
            e.Handled = True
        End If

    End Sub

    Private Sub txtaddress_KeyDown(sender As Object, e As KeyEventArgs)

        If e.Control AndAlso (e.KeyCode = Keys.V Or e.KeyCode = Keys.C Or e.KeyCode = Keys.X) Then
            e.SuppressKeyPress = True
        End If

    End Sub

    Private Sub txtaddress_KeyPress(sender As Object, e As KeyPressEventArgs)


        If Char.IsLetterOrDigit(e.KeyChar) OrElse Char.IsControl(e.KeyChar) OrElse Char.IsWhiteSpace(e.KeyChar) Then
            e.Handled = False
            Return
        End If


        Select Case e.KeyChar
            Case "#"c, "."c, ","c, "-"c, "/"c, "'"c, ":"c, "("c, ")"c
                e.Handled = False
                Return
        End Select


        e.Handled = True

    End Sub


    Private Sub txtsearch_KeyDown(sender As Object, e As KeyEventArgs) Handles txtsearch.KeyDown

        If e.Control AndAlso (e.KeyCode = Keys.V Or e.KeyCode = Keys.C Or e.KeyCode = Keys.X) Then
            e.SuppressKeyPress = True
        End If

    End Sub


    Private Sub Publisher_KeyDown(sender As Object, e As KeyEventArgs) Handles MyBase.KeyDown
        If e.KeyCode = Keys.Escape Then
            Me.Close()
        End If
    End Sub

    Private Sub DataGridView1_CellClick_1(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellClick

        If e.RowIndex >= 0 Then

            Dim row = DataGridView1.Rows(e.RowIndex)
            txtpublisher.Text = row.Cells("PublisherName").Value.ToString

        End If


    End Sub


    Private Sub txtpublisher_KeyPress(sender As Object, e As KeyPressEventArgs) Handles txtpublisher.KeyPress

        If Char.IsLetter(e.KeyChar) Then
            e.Handled = False
        ElseIf Char.IsControl(e.KeyChar) Then
            e.Handled = False
        ElseIf Char.IsWhiteSpace(e.KeyChar) Then
            e.Handled = False
        Else
            e.Handled = True
        End If
    End Sub


    Private Sub txtpublisher_TextChanged(sender As Object, e As EventArgs) Handles txtpublisher.TextChanged

        Dim InputText As String = txtpublisher.Text
        Dim FilteredText As New System.Text.StringBuilder()

        For Each c As Char In InputText
            If Char.IsLetter(c) OrElse Char.IsWhiteSpace(c) Then
                FilteredText.Append(c)
            End If
        Next


        If FilteredText.ToString() <> InputText Then

            Dim CursorPosition As Integer = txtpublisher.SelectionStart
            txtpublisher.Text = FilteredText.ToString()

            If CursorPosition > 0 Then

                txtpublisher.SelectionStart = Math.Min(CursorPosition - 1, txtpublisher.Text.Length)
            Else
                txtpublisher.SelectionStart = 0
            End If
        End If
    End Sub


    Private Sub txtpublisher_Validating(sender As Object, e As System.ComponentModel.CancelEventArgs) Handles txtpublisher.Validating

        Dim PublisherName As String = txtpublisher.Text.Trim()

        If String.IsNullOrEmpty(PublisherName) Then
            e.Cancel = False
            Return
        End If

        Dim Pattern As String = "^[A-Za-z]+(\s[A-Za-z]+)*$"

        If Not System.Text.RegularExpressions.Regex.IsMatch(PublisherName, Pattern) Then
            MessageBox.Show("Invalid publisher name format.", "Input Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning)

            e.Cancel = True
        Else
            e.Cancel = False
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