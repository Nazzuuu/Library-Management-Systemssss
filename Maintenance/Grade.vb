Imports MySql.Data.MySqlClient

Public Class Grade

    Private Sub Grade_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        DisablePaste_AllTextBoxes()
        TopMost = True
        Me.Refresh()
        refreshGrade()

        Try
            numupdown.Minimum = 0
            numupdown.Maximum = 12
            numupdown.Value = 0
        Catch
        End Try

        AddHandler GlobalVarsModule.DatabaseUpdated, AddressOf OnDatabaseUpdated
    End Sub

    Public Sub refreshGrade()
        Dim query As String = "SELECT * FROM `grade_tbl` ORDER BY CAST(Grade AS UNSIGNED)"
        GlobalVarsModule.AutoRefreshGrid(DataGridView1, query, 2000)
        SetupGridStyle()
        txtgrade.Clear()
        numupdown.Value = 0
    End Sub

    Private Async Sub OnDatabaseUpdated()
        Dim query As String = "SELECT * FROM `grade_tbl` ORDER BY CAST(Grade AS UNSIGNED)"
        Await GlobalVarsModule.LoadToGridAsync(DataGridView1, query)
        SetupGridStyle()
    End Sub

    Private Sub SetupGridStyle()
        Try
            If DataGridView1.Columns.Contains("ID") Then
                DataGridView1.Columns("ID").Visible = False
            End If

            If DataGridView1.Columns.Contains("Grade") Then
                DataGridView1.Columns("Grade").DisplayIndex = 0
            End If

            If DataGridView1.Columns.Contains("Edit") Then
                DataGridView1.Columns("Edit").DisplayIndex = DataGridView1.Columns.Count - 2
            End If

            If DataGridView1.Columns.Contains("Delete") Then
                DataGridView1.Columns("Delete").DisplayIndex = DataGridView1.Columns.Count - 1
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
        SetupGridStyle()
    End Sub

    Private Sub Grade_Shown(sender As Object, e As EventArgs) Handles MyBase.Shown
        DataGridView1.ClearSelection()
        DataGridView1.CurrentCell = Nothing
    End Sub

    Private Sub Grade_FormClosed(sender As Object, e As FormClosedEventArgs) Handles Me.FormClosed

        For Each form In Application.OpenForms
            If TypeOf form Is MainForm Then
                DirectCast(form, MainForm).loadsu()
            End If
        Next

        Dim activeMain As MainForm = GlobalVarsModule.ActiveMainForm

        If activeMain Is Nothing OrElse activeMain.IsDisposed Then
            activeMain = New MainForm()
            GlobalVarsModule.ActiveMainForm = activeMain
            activeMain.Show()
        End If

        activeMain.MaintenanceToolStripMenuItem.ShowDropDown()
        activeMain.MaintenanceToolStripMenuItem.ForeColor = Color.Gray
        txtgrade.Clear()

    End Sub

    Private Sub btnadd_Click(sender As Object, e As EventArgs) Handles btnadd.Click

        Dim gradeText As String = txtgrade.Text.Trim()

        Try


            If DataGridView1.SelectedRows.Count > 0 Then

                If String.IsNullOrWhiteSpace(gradeText) Then
                    MsgBox("Please fill in the required fields.", vbExclamation, "Missing Information")
                    Exit Sub
                End If

                Dim newGrade As Integer

                If Not Integer.TryParse(gradeText, newGrade) Then
                    MsgBox("Please enter a valid number.", vbExclamation, "Invalid Input")
                    Exit Sub
                End If

                If newGrade < 1 OrElse newGrade > 12 Then
                    MsgBox("Please enter a grade between 1 and 12.", vbExclamation, "Invalid Grade")
                    Exit Sub
                End If

                Dim selectedRow = DataGridView1.SelectedRows(0)
                Dim ID As Integer = Convert.ToInt32(selectedRow.Cells("ID").Value)
                Dim oldGrade As String = selectedRow.Cells("Grade").Value.ToString().Trim()

                If oldGrade = gradeText Then
                    MsgBox("No changes were made.", vbExclamation, "No Update")
                    Exit Sub
                End If

                Using con As New MySqlConnection(GlobalVarsModule.connectionString)

                    con.Open()

                    Dim check As New MySqlCommand(
                        "SELECT COUNT(*) FROM `grade_tbl` WHERE `Grade` = @grade AND `ID` <> @id", con)

                    check.Parameters.AddWithValue("@grade", gradeText)
                    check.Parameters.AddWithValue("@id", ID)

                    If Convert.ToInt32(check.ExecuteScalar()) > 0 Then
                        MsgBox("This grade already exists.", vbExclamation, "Duplication not allowed.")
                        Exit Sub
                    End If

                    Dim update As New MySqlCommand(
                        "UPDATE `grade_tbl` SET `Grade` = @grade WHERE `ID` = @id", con)

                    update.Parameters.AddWithValue("@grade", gradeText)
                    update.Parameters.AddWithValue("@id", ID)
                    update.ExecuteNonQuery()

                    Dim updateSection As New MySqlCommand(
                        "UPDATE `section_tbl` SET `GradeLevel` = @newGrade WHERE `GradeLevel` = @oldGrade", con)

                    updateSection.Parameters.AddWithValue("@newGrade", gradeText)
                    updateSection.Parameters.AddWithValue("@oldGrade", oldGrade)
                    updateSection.ExecuteNonQuery()

                    Dim updateBorrower As New MySqlCommand(
                        "UPDATE `borrower_tbl` SET `Grade` = @newGrade WHERE `GradeLevel` = @oldGrade", con)

                    updateBorrower.Parameters.AddWithValue("@newGrade", gradeText)
                    updateBorrower.Parameters.AddWithValue("@oldGrade", oldGrade)
                    updateBorrower.ExecuteNonQuery()

                    GlobalVarsModule.LogAudit(
                        actionType:="UPDATE",
                        formName:="GRADE FORM",
                        description:=$"Updated grade level ID {ID} from '{oldGrade}' to '{gradeText}'",
                        recordID:=ID.ToString(),
                        oldValue:=$"Grade Level: {oldGrade}",
                        newValue:=$"Grade Level: {gradeText}"
                    )

                End Using

                For Each form In Application.OpenForms

                    If TypeOf form Is AuditTrail Then
                        DirectCast(form, AuditTrail).refreshaudit()
                    End If

                    If TypeOf form Is Borrower Then
                        DirectCast(form, Borrower).cbgradee()
                        DirectCast(form, Borrower).refreshData()
                    End If

                    If TypeOf form Is Section Then
                        DirectCast(form, Section).cbgradesu()
                        DirectCast(form, Section).refreshsecs()
                    End If

                    If TypeOf form Is MainForm Then
                        DirectCast(form, MainForm).loadsu()
                    End If

                Next

                MsgBox("Grade updated successfully!", vbInformation)

            Else


                Dim quantity As Integer = 0

                Try
                    quantity = CInt(numupdown.Value)
                Catch
                    quantity = 0
                End Try

                Dim gradesToAdd As New List(Of Integer)

                If quantity > 0 Then

                    For i As Integer = 1 To quantity
                        gradesToAdd.Add(i)
                    Next

                Else

                    If String.IsNullOrWhiteSpace(gradeText) Then
                        MsgBox("Please fill in the required fields.", vbExclamation, "Missing Information")
                        Exit Sub
                    End If

                    Dim grade As Integer

                    If Not Integer.TryParse(gradeText, grade) Then
                        MsgBox("Please enter a valid number.", vbExclamation, "Invalid Input")
                        Exit Sub
                    End If

                    If grade < 1 OrElse grade > 12 Then
                        MsgBox("Please enter a grade between 1 and 12.", vbExclamation, "Invalid Grade")
                        Exit Sub
                    End If

                    gradesToAdd.Add(grade)

                End If

                Using con As New MySqlConnection(GlobalVarsModule.connectionString)

                    con.Open()

                    For Each gradeValue In gradesToAdd

                        Dim check As New MySqlCommand(
                            "SELECT COUNT(*) FROM `grade_tbl` WHERE `Grade` = @grade", con)

                        check.Parameters.AddWithValue("@grade", gradeValue.ToString())

                        If Convert.ToInt32(check.ExecuteScalar()) > 0 Then
                            Continue For
                        End If

                        Dim insert As New MySqlCommand(
                            "INSERT INTO `grade_tbl` (`Grade`) VALUES (@grade); SELECT LAST_INSERT_ID();", con)

                        insert.Parameters.AddWithValue("@grade", gradeValue.ToString())

                        Dim newID As Integer = Convert.ToInt32(insert.ExecuteScalar())

                        GlobalVarsModule.LogAudit(
                            actionType:="ADD",
                            formName:="GRADE FORM",
                            description:=$"Added new grade level: {gradeValue}",
                            recordID:=newID.ToString()
                        )

                    Next

                End Using

                For Each form In Application.OpenForms

                    If TypeOf form Is AuditTrail Then
                        DirectCast(form, AuditTrail).refreshaudit()
                    End If

                    If TypeOf form Is Borrower Then
                        DirectCast(form, Borrower).cbgradee()
                        DirectCast(form, Borrower).refreshData()
                    End If

                    If TypeOf form Is Section Then
                        DirectCast(form, Section).cbgradesu()
                        DirectCast(form, Section).refreshsecs()
                    End If

                Next

                MsgBox("Grade(s) added successfully", vbInformation)

            End If

            txtgrade.Clear()
            numupdown.Value = 0
            DataGridView1.ClearSelection()
            DataGridView1.CurrentCell = Nothing
            refreshGrade()

        Catch ex As MySqlException
            MsgBox("Database Error: " & ex.Message, vbCritical, "Grade Error")

        Catch ex As Exception
            MsgBox(ex.Message, vbCritical, "Grade Error")

        End Try

    End Sub

    Private Sub DataGridView1_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellContentClick

        If e.RowIndex < 0 Then Exit Sub

        Dim row = DataGridView1.Rows(e.RowIndex)

        If DataGridView1.Columns(e.ColumnIndex).Name = "Edit" Then

            DataGridView1.ClearSelection()
            row.Selected = True

            txtgrade.Text = row.Cells("Grade").Value.ToString()
            numupdown.Value = 0

            Exit Sub

        End If

        If DataGridView1.Columns(e.ColumnIndex).Name = "Delete" Then

            Dim result = MessageBox.Show(
        "Are you sure you want to delete this grade?",
        "Confirm Delete",
        MessageBoxButtons.YesNo,
        MessageBoxIcon.Warning
    )

            If result <> DialogResult.Yes Then Exit Sub

            Dim selectedRow = DataGridView1.Rows(e.RowIndex)
            Dim gradeLevel As String = selectedRow.Cells("Grade").Value.ToString().Trim()

            Try

                Dim ID As Integer

                If selectedRow.Cells("ID").Value Is Nothing OrElse
           IsDBNull(selectedRow.Cells("ID").Value) OrElse
           Not Integer.TryParse(selectedRow.Cells("ID").Value.ToString(), ID) Then

                    MsgBox("Invalid Grade ID.", vbCritical, "Delete Error")
                    Exit Sub

                End If

                Using con As New MySqlConnection(GlobalVarsModule.connectionString)

                    con.Open()

                    Dim sectionCom As New MySqlCommand(
                "SELECT COUNT(*) FROM `section_tbl` WHERE `GradeLevel` = @grade", con)

                    sectionCom.Parameters.AddWithValue("@grade", gradeLevel)

                    Dim sectionCount As Integer =
                Convert.ToInt32(sectionCom.ExecuteScalar())

                    If sectionCount > 0 Then

                        MessageBox.Show(
                    "Cannot delete this grade. It is currently assigned to " &
                    sectionCount & " section(s).",
                    "Information",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                )

                        Exit Sub

                    End If

                    Dim borrowerCom As New MySqlCommand(
                "SELECT COUNT(*) FROM `borrower_tbl` WHERE `Grade` = @grade", con)

                    borrowerCom.Parameters.AddWithValue("@grade", gradeLevel)

                    Dim borrowerCount As Integer =
                Convert.ToInt32(borrowerCom.ExecuteScalar())

                    If borrowerCount > 0 Then

                        MessageBox.Show(
                    "Cannot delete this grade. It is currently assigned to " &
                    borrowerCount & " borrower(s).",
                    "Information",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                )

                        Exit Sub

                    End If

                    Dim delete As New MySqlCommand(
                "DELETE FROM `grade_tbl` WHERE `ID` = @id", con)

                    delete.Parameters.AddWithValue("@id", ID)
                    delete.ExecuteNonQuery()

                    GlobalVarsModule.LogAudit(
                actionType:="DELETE",
                formName:="GRADE FORM",
                description:=$"Deleted grade level: {gradeLevel}",
                recordID:=ID.ToString()
            )

                    Dim count As New MySqlCommand(
                "SELECT COUNT(*) FROM `grade_tbl`", con)

                    Dim rowCount As Long =
                Convert.ToInt64(count.ExecuteScalar())

                    If rowCount = 0 Then

                        Dim reset As New MySqlCommand(
                    "ALTER TABLE `grade_tbl` AUTO_INCREMENT = 1", con)

                        reset.ExecuteNonQuery()

                    End If

                End Using

                For Each form In Application.OpenForms

                    If TypeOf form Is AuditTrail Then
                        DirectCast(form, AuditTrail).refreshaudit()
                    End If

                    If TypeOf form Is Borrower Then
                        DirectCast(form, Borrower).cbgradee()
                        DirectCast(form, Borrower).refreshData()
                    End If

                    If TypeOf form Is Section Then
                        DirectCast(form, Section).cbgradesu()
                        DirectCast(form, Section).refreshsecs()
                    End If

                Next

                MsgBox("Grade deleted successfully.", vbInformation)

                txtgrade.Clear()
                numupdown.Value = 0
                DataGridView1.ClearSelection()
                DataGridView1.CurrentCell = Nothing
                refreshGrade()

                Exit Sub

            Catch ex As MySqlException

                MsgBox("Database Error: " & ex.Message, vbCritical, "Delete Error")

            Catch ex As Exception

                MsgBox(ex.Message, vbCritical, "Delete Error")

            End Try

        End If

    End Sub

    Private Sub DataGridView1_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellClick

        If e.RowIndex >= 0 Then

            Dim row = DataGridView1.Rows(e.RowIndex)
            txtgrade.Text = row.Cells("Grade").Value.ToString()

        End If

    End Sub

    Private Sub numupdown_ValueChanged(sender As Object, e As EventArgs) Handles numupdown.ValueChanged

        Try
            If numupdown.Value > 0 Then
                txtgrade.Enabled = False
            Else
                txtgrade.Enabled = True
            End If

            If numupdown.Value > 12 Then
                numupdown.Value = 12
            End If

        Catch
        End Try

    End Sub

    Private Sub txtsearch_TextChanged(sender As Object, e As EventArgs) Handles txtsearch.TextChanged

        HandleAutoRefreshPause(DataGridView1, txtsearch)

        Dim dt As DataTable = TryCast(DataGridView1.DataSource, DataTable)

        If dt IsNot Nothing Then

            If txtsearch.Text.Trim() <> "" Then

                Dim filter As String =
                    String.Format("Grade LIKE '*{0}*'", txtsearch.Text.Trim())

                dt.DefaultView.RowFilter = filter

            Else
                dt.DefaultView.RowFilter = ""
            End If

        End If

    End Sub

    Private Sub txtgrade_KeyDown(sender As Object, e As KeyEventArgs) Handles txtgrade.KeyDown

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

    Private Sub txtgrade_KeyPress(sender As Object, e As KeyPressEventArgs) Handles txtgrade.KeyPress

        If Not Char.IsDigit(e.KeyChar) AndAlso Not Char.IsControl(e.KeyChar) Then
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

    Private Sub Grade_KeyDown(sender As Object, e As KeyEventArgs) Handles MyBase.KeyDown

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