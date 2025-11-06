Imports System.Diagnostics.Eventing.Reader
Imports System.Runtime.Intrinsics.Arm
Imports System.Security
Imports MySql.Data.MySqlClient

Public Class Department
    Private Sub Department_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        TopMost = True
        Me.Refresh()
        refreshDepartment()
        DisablePaste_AllTextBoxes()
        AddHandler GlobalVarsModule.DatabaseUpdated, AddressOf OnDatabaseUpdated
    End Sub

    Public Sub refreshDepartment()
        Dim query As String = "SELECT * FROM `department_tbl`"
        GlobalVarsModule.AutoRefreshGrid(DataGridView1, query, 2000)
        SetupGridStyle()
    End Sub

    Private Async Sub OnDatabaseUpdated()
        Dim query As String = "SELECT * FROM `department_tbl`"
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


    Private Sub Department_Shown(sender As Object, e As EventArgs) Handles MyBase.Shown

        DataGridView1.ClearSelection()

    End Sub

    Private Sub Department_FormClosed(sender As Object, e As FormClosedEventArgs) Handles Me.FormClosed



        Dim activeMain As MainForm = GlobalVarsModule.ActiveMainForm
        If activeMain Is Nothing OrElse activeMain.IsDisposed Then
            activeMain = New MainForm()
            GlobalVarsModule.ActiveMainForm = activeMain
            activeMain.Show()
        End If

        activeMain.MaintenanceToolStripMenuItem.ShowDropDown()
        activeMain.MaintenanceToolStripMenuItem.ForeColor = Color.Gray
        txtdepartment.Text = ""


    End Sub

    Private Sub btnadd_Click(sender As Object, e As EventArgs) Handles btnadd.Click

        Dim con As New MySqlConnection(GlobalVarsModule.connectionString)
        Dim deps As String = txtdepartment.Text.Trim()
        Dim newID As Integer = 0

        If String.IsNullOrWhiteSpace(deps) Then
            MsgBox("Please fill in the required fields.", vbExclamation, "Missing Information")
            Exit Sub
        End If

        If deps = "Junior High School" OrElse deps = "Senior High School" OrElse deps = "Elementary" Then

            Try
                con.Open()

                Dim command As New MySqlCommand("SELECT COUNT(*) FROM `department_tbl` WHERE `Department` = @department", con)
                command.Parameters.AddWithValue("@department", deps)
                Dim count As Integer = Convert.ToInt32(command.ExecuteScalar)

                If count > 0 Then
                    MsgBox("This department already exists.", vbExclamation, "Duplication is not allowed.")
                    Exit Sub
                End If

                Dim com As New MySqlCommand("INSERT INTO `department_tbl`(`Department`) VALUES (@department); SELECT LAST_INSERT_ID();", con)
                com.Parameters.AddWithValue("@department", deps)
                newID = Convert.ToInt32(com.ExecuteScalar())

                GlobalVarsModule.LogAudit(
                    actionType:="ADD",
                    formName:="DEPARTMENT FORM",
                    description:=$"Added new department: {deps}",
                    recordID:=newID.ToString()
                )

                For Each form In Application.OpenForms
                    If TypeOf form Is AuditTrail Then
                        DirectCast(form, AuditTrail).refreshaudit()
                    End If
                Next

                For Each form In Application.OpenForms
                    If TypeOf form Is Borrower Then
                        Dim borrower = DirectCast(form, Borrower)
                        borrower.cbdepts()
                    End If
                Next

                For Each form In Application.OpenForms
                    If TypeOf form Is Section Then
                        Dim depsu = DirectCast(form, Section)
                        depsu.cbdeptss()
                    End If
                Next

                MsgBox("Department added successfully", vbInformation)
                Department_Load(sender, e)
            Catch ex As Exception
                MsgBox(ex.Message, vbCritical)
            Finally
                txtdepartment.Clear()
                If con.State = ConnectionState.Open Then
                    con.Close()
                End If
            End Try
        Else
            MessageBox.Show("Invalid input. Please enter 'Elementary' or 'Junior High School' or 'Senior High School' only.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtdepartment.Clear()
        End If
    End Sub

    Private Sub btnedit_Click(sender As Object, e As EventArgs) Handles btnedit.Click

        If DataGridView1.SelectedRows.Count > 0 Then

            Dim con As New MySqlConnection(GlobalVarsModule.connectionString)
            Dim selectedRow As DataGridViewRow = DataGridView1.SelectedRows(0)
            Dim ID As Integer = CInt(selectedRow.Cells("ID").Value)

            Dim oldDept As String = selectedRow.Cells("Department").Value.ToString()
            Dim dept As String = txtdepartment.Text.Trim()

            If String.IsNullOrWhiteSpace(dept) Then
                MsgBox("Please fill in the required fields.", vbExclamation, "Missing Information")
                Exit Sub
            End If

            If oldDept.ToUpper() = dept.ToUpper() Then
                MsgBox("No changes were made.", vbExclamation, "No Update")
                Exit Sub
            End If

            If dept = "Junior High School" OrElse dept = "Senior High School" OrElse dept = "Elementary" Then

                Try
                    con.Open()

                    Dim command As New MySqlCommand("SELECT COUNT(*) FROM `department_tbl` WHERE `Department` = @department AND `ID` <> @id", con)
                    command.Parameters.AddWithValue("@department", dept)
                    command.Parameters.AddWithValue("@id", ID)
                    Dim count As Integer = Convert.ToInt32(command.ExecuteScalar)
                    If count > 0 Then
                        MsgBox("This department already exists.", vbExclamation, "Duplication is not allowed.")
                        Exit Sub
                    End If

                    Dim com As New MySqlCommand("UPDATE `department_tbl` SET `Department`= @department WHERE `ID` = @id", con)
                    com.Parameters.AddWithValue("@department", dept)
                    com.Parameters.AddWithValue("@id", ID)
                    com.ExecuteNonQuery()

                    Dim comss As New MySqlCommand("UPDATE `section_tbl` SET `Department` = @newDept WHERE `Department` = @oldDept", con)
                    comss.Parameters.AddWithValue("@newDept", dept)
                    comss.Parameters.AddWithValue("@oldDept", oldDept)
                    comss.ExecuteNonQuery()

                    Dim comsuss As New MySqlCommand("UPDATE `borrower_tbl` SET `Department` = @newDept WHERE `Department` = @oldDept", con)
                    comsuss.Parameters.AddWithValue("@newDept", dept)
                    comsuss.Parameters.AddWithValue("@oldDept", oldDept)
                    comsuss.ExecuteNonQuery()

                    GlobalVarsModule.LogAudit(
                        actionType:="UPDATE",
                        formName:="DEPARTMENT FORM",
                        description:=$"Updated department ID {ID} from '{oldDept}' to '{dept}'",
                        recordID:=ID.ToString(),
                        oldValue:=$"Department: {oldDept}",
                        newValue:=$"Department: {dept}"
                    )

                    For Each form In Application.OpenForms
                        If TypeOf form Is AuditTrail Then
                            DirectCast(form, AuditTrail).refreshaudit()
                        End If
                    Next

                    For Each form In Application.OpenForms
                        If TypeOf form Is Borrower Then
                            Dim borrower = DirectCast(form, Borrower)
                            borrower.cbdepts()
                        End If

                        If TypeOf form Is Section Then
                            Dim depsu = DirectCast(form, Section)
                            depsu.cbdeptss()
                        End If
                    Next

                    For Each form In Application.OpenForms
                        If TypeOf form Is MainForm Then
                            Dim load = DirectCast(form, MainForm)
                            load.loadsu()
                        End If
                    Next

                    MsgBox("Updated successfully!", vbInformation)
                    Department_Load(sender, e)

                Catch ex As Exception
                    MsgBox(ex.Message, vbCritical)
                Finally
                    txtdepartment.Clear()
                    If con.State = ConnectionState.Open Then
                        con.Close()
                    End If
                End Try
            Else
                MessageBox.Show("Invalid input. Please enter 'Elementary' or 'Junior High School' or 'Senior High School' only.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtdepartment.Text = ""
            End If
        Else
            MsgBox("Please select a row to edit.", vbExclamation)
        End If

    End Sub

    Private Sub btndelete_Click(sender As Object, e As EventArgs) Handles btndelete.Click

        If DataGridView1.SelectedRows.Count > 0 Then

            Dim dialogResult As DialogResult = MessageBox.Show("Are you sure you want to delete this department?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)

            If dialogResult = DialogResult.Yes Then

                Dim con As New MySqlConnection(GlobalVarsModule.connectionString)
                Dim selectedRow As DataGridViewRow = DataGridView1.SelectedRows(0)
                Dim ID As Integer = CInt(selectedRow.Cells("ID").Value)
                Dim departmentName As String = selectedRow.Cells("Department").Value.ToString().Trim()

                Try
                    con.Open()


                    Dim sectionCom As New MySqlCommand("SELECT COUNT(*) FROM `section_tbl` WHERE Department = @department", con)
                    sectionCom.Parameters.AddWithValue("@department", departmentName)
                    Dim sectionCount As Integer = CInt(sectionCom.ExecuteScalar())

                    If sectionCount > 0 Then
                        MessageBox.Show("Cannot delete this department. It is already assigned to a section. You must delete its sections first.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        Return
                    End If


                    Dim borrowerCom As New MySqlCommand("SELECT COUNT(*) FROM `borrower_tbl` WHERE Department = @department", con)
                    borrowerCom.Parameters.AddWithValue("@department", departmentName)
                    Dim borrowerCount As Integer = CInt(borrowerCom.ExecuteScalar())

                    If borrowerCount > 0 Then
                        MessageBox.Show("Cannot delete this department. It is assigned to " & borrowerCount & " borrower(s).", "Information", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        Return
                    End If



                    Dim delete As New MySqlCommand("DELETE FROM `department_tbl` WHERE `ID` = @id", con)
                    delete.Parameters.AddWithValue("@id", ID)
                    delete.ExecuteNonQuery()

                    GlobalVarsModule.LogAudit(
                        actionType:="DELETE",
                        formName:="DEPARTMENT FORM",
                        description:=$"Deleted department: {departmentName}",
                        recordID:=ID.ToString()
                    )

                    For Each form In Application.OpenForms
                        If TypeOf form Is AuditTrail Then
                            DirectCast(form, AuditTrail).refreshaudit()
                        End If
                    Next

                    For Each form In Application.OpenForms
                        If TypeOf form Is Borrower Then
                            DirectCast(form, Borrower).cbdepts()
                        End If
                        If TypeOf form Is Section Then
                            DirectCast(form, Section).cbdeptss()
                        End If
                    Next

                    MsgBox("Department deleted successfully.", vbInformation)
                    Department_Load(sender, e)
                    txtdepartment.Clear()

                    Dim rowCountCom As New MySqlCommand("SELECT COUNT(*) FROM `department_tbl`", con)
                    Dim rowCount As Long = CLng(rowCountCom.ExecuteScalar())

                    If rowCount = 0 Then
                        Dim reset As New MySqlCommand("ALTER TABLE `department_tbl` AUTO_INCREMENT = 1", con)
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
            txtdepartment.Text = row.Cells("Department").Value.ToString

        End If

    End Sub

    Private Sub txtsearch_TextChanged(sender As Object, e As EventArgs) Handles txtsearch.TextChanged


        Dim dt As DataTable = DirectCast(DataGridView1.DataSource, DataTable)
        If dt IsNot Nothing Then
            If txtsearch.Text.Trim() <> "" Then
                Dim filter As String = String.Format("Department LIKE '*{0}*'", txtsearch.Text.Trim())
                dt.DefaultView.RowFilter = filter
            Else
                dt.DefaultView.RowFilter = ""
            End If
        End If

    End Sub

    Private Sub txtdepartment_KeyDown(sender As Object, e As KeyEventArgs) Handles txtdepartment.KeyDown

        If e.Control AndAlso (e.KeyCode = Keys.V Or e.KeyCode = Keys.C Or e.KeyCode = Keys.X) Then
            e.SuppressKeyPress = True
        End If

        If e.KeyCode = Keys.Enter Then
            btnadd_Click(sender, e)
            e.Handled = True
        End If

    End Sub

    Private Sub txtdepartment_KeyPress(sender As Object, e As KeyPressEventArgs) Handles txtdepartment.KeyPress

        If Not Char.IsLetter(e.KeyChar) AndAlso Not Char.IsControl(e.KeyChar) AndAlso Not Char.IsWhiteSpace(e.KeyChar) Then
            e.Handled = True

        End If

    End Sub

    Private Sub txtsearch_KeyDown(sender As Object, e As KeyEventArgs) Handles txtsearch.KeyDown

        If e.Control AndAlso (e.KeyCode = Keys.V Or e.KeyCode = Keys.C Or e.KeyCode = Keys.X) Then
            e.SuppressKeyPress = True
        End If

    End Sub

    Private Sub Department_KeyDown(sender As Object, e As KeyEventArgs) Handles MyBase.KeyDown
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