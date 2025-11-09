Imports System.Data
Imports System.Security.Policy
Imports MySql.Data.MySqlClient

Public Class Section

    Private Sub Section_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        DisablePaste_AllTextBoxes()
        TopMost = True
        Me.Refresh()

        refreshsecs()
        AddHandler cbgrade.DropDown, AddressOf RefreshComboBoxes
        AddHandler GlobalVarsModule.DatabaseUpdated, AddressOf OnDatabaseUpdated
        AddHandler cbdepartment.SelectedIndexChanged, AddressOf cbdepartment_SelectedIndexChanged

        cbgrade.Enabled = False
    End Sub

    Public Sub refreshsecs()
        Dim query As String = "SELECT * FROM `section_tbl`"
        GlobalVarsModule.AutoRefreshGrid(DataGridView1, query, 2000)
        SetupGridStyle()
        cbdeptss()
        clearlahat()

        Try
            AutoRefreshComboBox(cbdepartment, "SELECT ID, Department FROM department_tbl ORDER BY Department", "Department", "ID")
            AutoRefreshComboBox(cbgrade, "SELECT ID, Grade FROM grade_tbl ORDER BY Grade", "Grade", "ID")
            cbgrade.Enabled = False
        Catch ex As Exception
            Debug.WriteLine("Auto-refresh ComboBox failed: " & ex.Message)
        End Try
    End Sub

    Private Async Sub OnDatabaseUpdated()
        Dim query As String = "SELECT * FROM `section_tbl`"
        Await GlobalVarsModule.LoadToGridAsync(DataGridView1, query)
        SetupGridStyle()
        cbdeptss()
        clearlahat()

        AutoRefreshComboBox(cbdepartment, "SELECT ID, Department FROM department_tbl ORDER BY Department", "Department", "ID")
        AutoRefreshComboBox(cbgrade, "SELECT ID, Grade FROM grade_tbl ORDER BY Grade", "Grade", "ID")

    End Sub

    Private Sub RefreshComboBoxes(sender As Object, e As EventArgs)
        Dim cb As ComboBox = DirectCast(sender, ComboBox)

        Using con As New MySqlConnection(GlobalVarsModule.connectionString)
            Dim query As String = ""

            Select Case cb.Name.ToLower()
                Case "cbdepartment"
                    query = "SELECT Department FROM department_tbl ORDER BY Department"
                Case "cbgrade"
                    query = "SELECT Grade FROM grade_tbl ORDER BY Grade"
            End Select

            If query <> "" Then
                Dim dt As New DataTable()
                Dim da As New MySqlDataAdapter(query, con)
                da.Fill(dt)

                cb.DataSource = dt
                cb.DisplayMember = dt.Columns(0).ColumnName
                cb.ValueMember = dt.Columns(0).ColumnName
                cb.SelectedIndex = -1
            End If
        End Using
    End Sub


    Private Sub cbdepartment_DropDown(sender As Object, e As EventArgs) Handles cbdepartment.DropDown
        Try
            cbdepartment.DataSource = Nothing
            cbdeptss()
        Catch ex As Exception
            Debug.WriteLine("Error refreshing department combo: " & ex.Message)
        End Try
    End Sub

    Private Sub cbgrade_DropDown(sender As Object, e As EventArgs) Handles cbgrade.DropDown
        Try
            cbgrade.DataSource = Nothing
            cbgradesu()
        Catch ex As Exception
            Debug.WriteLine("Error refreshing grade combo: " & ex.Message)
        End Try
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


    Private Sub Section_Shown(sender As Object, e As EventArgs) Handles MyBase.Shown

        DataGridView1.ClearSelection()

    End Sub

    Private Sub Section_FormClosed(sender As Object, e As FormClosedEventArgs) Handles Me.FormClosed

        For Each form In Application.OpenForms
            If TypeOf form Is MainForm Then
                Dim load = DirectCast(form, MainForm)
                load.loadsu()
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
        clearlahat()

    End Sub

    Private Sub btnadd_Click(sender As Object, e As EventArgs) Handles btnadd.Click

        Dim con As New MySqlConnection(GlobalVarsModule.connectionString)
        Dim newID As Integer = 0
        Dim dept = ""
        Dim grade = ""
        Dim secs = ""
        Dim strand = ""
        Dim entryDescription As String = ""

        If cbdepartment.SelectedIndex <> -1 Then
            dept = cbdepartment.GetItemText(cbdepartment.SelectedItem)
        End If

        If cbgrade.SelectedIndex <> -1 Then
            grade = cbgrade.GetItemText(cbgrade.SelectedItem)
        End If

        If dept = "Junior High School" OrElse dept = "Elementary" Then
            secs = txtsection.Text.Trim
            strand = ""
            If String.IsNullOrWhiteSpace(secs) Then
                MsgBox("Please fill in the required fields.", vbExclamation, "Missing Information")
                Exit Sub
            End If
            entryDescription = $"Department: {dept}, Grade Level: {grade}, Section: {secs}"
        ElseIf dept = "Senior High School" Then
            secs = ""
            If cbstrand.SelectedIndex <> -1 Then
                strand = cbstrand.GetItemText(cbstrand.SelectedItem)
            End If
            If String.IsNullOrWhiteSpace(strand) Then
                MsgBox("Please fill in the required fields.", vbExclamation, "Missing Information")
                Exit Sub
            End If
            entryDescription = $"Department: {dept}, Grade Level: {grade}, Strand: {strand}"
        End If

        If String.IsNullOrWhiteSpace(dept) OrElse String.IsNullOrWhiteSpace(grade) Then
            MsgBox("Please select Department and Grade Level.", vbExclamation, "Missing Information")
            Exit Sub
        End If


        If duplication(dept, grade, secs, strand) Then
            MsgBox("The entry already exists in the database. Please use an another Section or Strand name.", vbExclamation, "Duplication Error")
            Exit Sub
        End If


        Try
            con.Open()
            Dim com As New MySqlCommand("INSERT INTO `section_tbl`(`Department`, `GradeLevel`, `Section`, `Strand`) VALUES (@dept, @grade, @section, @strand); SELECT LAST_INSERT_ID();", con)
            com.Parameters.AddWithValue("@dept", dept)
            com.Parameters.AddWithValue("@grade", grade)
            com.Parameters.AddWithValue("@section", If(String.IsNullOrWhiteSpace(secs), CType(DBNull.Value, Object), secs))
            com.Parameters.AddWithValue("@strand", If(String.IsNullOrWhiteSpace(strand), CType(DBNull.Value, Object), strand))
            newID = Convert.ToInt32(com.ExecuteScalar())

            GlobalVarsModule.LogAudit(
                actionType:="ADD",
                formName:="SECTION FORM",
                description:=$"Added new section/strand: {entryDescription}",
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
                    borrower.cbsecs()
                End If
            Next

            MsgBox("Section added successfully", vbInformation)
            Section_Load(sender, e)

            clearlahat()

        Catch ex As Exception
            MsgBox(ex.Message, vbCritical)
        Finally
            If con.State = ConnectionState.Open Then con.Close()
        End Try

    End Sub



    Private Function duplication(ByVal dept As String, ByVal grade As String, ByVal section As String, ByVal strand As String) As Boolean
        Dim con As New MySqlConnection(GlobalVarsModule.connectionString)
        Dim query As String = ""
        Dim count As Integer = 0

        If dept = "Junior High School" OrElse dept = "Elementary" Then
            query = "SELECT COUNT(*) FROM `section_tbl` WHERE `Department` = @dept AND `GradeLevel` = @grade AND `Section` = @section"
        ElseIf dept = "Senior High School" Then
            query = "SELECT COUNT(*) FROM `section_tbl` WHERE `Department` = @dept AND `GradeLevel` = @grade AND `Strand` = @strand"
        End If

        If String.IsNullOrWhiteSpace(query) Then
            Return False
        End If

        Try
            con.Open()
            Dim com As New MySqlCommand(query, con)
            com.Parameters.AddWithValue("@dept", dept)
            com.Parameters.AddWithValue("@grade", grade)

            If dept = "Junior High School" OrElse dept = "Elementary" Then
                com.Parameters.AddWithValue("@section", section)
            ElseIf dept = "Senior High School" Then
                com.Parameters.AddWithValue("@strand", strand)
            End If

            count = CInt(com.ExecuteScalar())

        Catch ex As Exception
            MsgBox(ex.Message, vbCritical)
            Return True
        Finally
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
        End Try

        Return count > 0
    End Function

    Private Sub btnedit_Click(sender As Object, e As EventArgs) Handles btnedit.Click

        If DataGridView1.SelectedRows.Count > 0 Then
            Dim con As New MySqlConnection(GlobalVarsModule.connectionString)
            Dim selectedRow = DataGridView1.SelectedRows(0)
            Dim ID As Integer = CInt(selectedRow.Cells("ID").Value)

            Dim oldDept As String = selectedRow.Cells("Department").Value.ToString().Trim()
            Dim oldGrade As String = selectedRow.Cells("GradeLevel").Value.ToString().Trim()
            Dim oldSecs As String = If(IsDBNull(selectedRow.Cells("Section").Value), "", selectedRow.Cells("Section").Value.ToString().Trim())
            Dim oldStrand As String = If(IsDBNull(selectedRow.Cells("Strand").Value), "", selectedRow.Cells("Strand").Value.ToString().Trim())
            Dim oldEntry As String = If(oldDept = "Senior High School", $"Dept: {oldDept}, Grade: {oldGrade}, Strand: {oldStrand}", $"Dept: {oldDept}, Grade: {oldGrade}, Section: {oldSecs}")


            Dim newDept As String = ""
            Dim newGrade As String = ""
            Dim newSecs As String = ""
            Dim newStrand As String = ""
            Dim newEntry As String = ""

            If cbdepartment.SelectedIndex <> -1 Then
                newDept = cbdepartment.GetItemText(cbdepartment.SelectedItem)
            End If
            If cbgrade.SelectedIndex <> -1 Then
                newGrade = cbgrade.GetItemText(cbgrade.SelectedItem)
            End If

            If newDept = "Junior High School" OrElse newDept = "Elementary" Then
                newSecs = txtsection.Text.Trim
                newStrand = ""
                If String.IsNullOrWhiteSpace(newSecs) Then
                    MsgBox("Please fill in the required fields.", vbExclamation, "Missing Information")
                    Exit Sub
                End If
                newEntry = $"Dept: {newDept}, Grade: {newGrade}, Section: {newSecs}"
            ElseIf newDept = "Senior High School" Then
                newSecs = ""
                If cbstrand.SelectedIndex <> -1 Then
                    newStrand = cbstrand.GetItemText(cbstrand.SelectedItem)
                End If
                If String.IsNullOrWhiteSpace(newStrand) Then
                    MsgBox("Please fill in the required fields.", vbExclamation, "Missing Information")
                    Exit Sub
                End If
                newEntry = $"Dept: {newDept}, Grade: {newGrade}, Strand: {newStrand}"
            End If

            If String.IsNullOrWhiteSpace(newDept) OrElse String.IsNullOrWhiteSpace(newGrade) Then
                MsgBox("Please select Department and Grade Level.", vbExclamation, "Missing Information")
                Exit Sub
            End If


            If oldDept.Equals(newDept) And oldGrade.Equals(newGrade) And oldSecs.Equals(newSecs) And oldStrand.Equals(newStrand) Then
                MsgBox("No changes were made.", vbExclamation, "No Update")
                Exit Sub
            End If

            If duplication(newDept, newGrade, newSecs, newStrand, ID) Then
                MsgBox("The updated entry already exists in the database. Please use another Section or Strand name.", vbExclamation, "Duplication Error")
                Exit Sub
            End If

            Try
                con.Open()

                Dim com As New MySqlCommand("UPDATE `section_tbl` SET `Department` = @newDept, `GradeLevel` = @newGrade, `Section` = @newSection, `Strand` = @newStrand WHERE `ID` = @id", con)
                com.Parameters.AddWithValue("@newDept", newDept)
                com.Parameters.AddWithValue("@newGrade", newGrade)
                com.Parameters.AddWithValue("@newSection", If(String.IsNullOrWhiteSpace(newSecs), CType(DBNull.Value, Object), newSecs))
                com.Parameters.AddWithValue("@newStrand", If(String.IsNullOrWhiteSpace(newStrand), CType(DBNull.Value, Object), newStrand))
                com.Parameters.AddWithValue("@id", ID)
                com.ExecuteNonQuery()



                Dim comsss As New MySqlCommand("", con)
                If oldDept = "Senior High School" Then
                    comsss.CommandText = "UPDATE `borrower_tbl` SET `Department` = @newDept, `Grade` = @newGrade, `Strand` = @newStrand, `Section` = NULL WHERE `Department` = @oldDept AND `Grade` = @oldGrade AND `Strand` = @oldStrand"
                    comsss.Parameters.AddWithValue("@newStrand", newStrand)
                    comsss.Parameters.AddWithValue("@oldStrand", oldStrand)
                Else
                    comsss.CommandText = "UPDATE `borrower_tbl` SET `Department` = @newDept, `Grade` = @newGrade, `Section` = @newSection, `Strand` = NULL WHERE `Department` = @oldDept AND `Grade` = @oldGrade AND `Section` = @oldSection"
                    comsss.Parameters.AddWithValue("@newSection", newSecs)
                    comsss.Parameters.AddWithValue("@oldSection", oldSecs)
                End If
                comsss.Parameters.AddWithValue("@newDept", newDept)
                comsss.Parameters.AddWithValue("@newGrade", newGrade)
                comsss.Parameters.AddWithValue("@oldDept", oldDept)
                comsss.Parameters.AddWithValue("@oldGrade", oldGrade)
                comsss.ExecuteNonQuery()

                GlobalVarsModule.LogAudit(
                    actionType:="UPDATE",
                    formName:="SECTION FORM",
                    description:=$"Updated section/strand ID {ID} from '{oldEntry}' to '{newEntry}'",
                    recordID:=ID.ToString(),
                    oldValue:=oldEntry,
                    newValue:=newEntry
                )

                For Each form In Application.OpenForms
                    If TypeOf form Is AuditTrail Then
                        DirectCast(form, AuditTrail).refreshaudit()
                    End If
                Next

                For Each form In Application.OpenForms
                    If TypeOf form Is Borrower Then
                        Dim borrower = DirectCast(form, Borrower)
                        borrower.cbsecs()
                        borrower.refreshData()
                    End If
                Next

                For Each form In Application.OpenForms
                    If TypeOf form Is MainForm Then
                        Dim load = DirectCast(form, MainForm)
                        load.loadsu()
                    End If
                Next

                MsgBox("Updated successfully!", vbInformation)
                Section_Load(sender, e)
                clearlahat()
            Catch ex As Exception
                MsgBox(ex.Message, vbCritical)
            Finally
                If con.State = ConnectionState.Open Then con.Close()
            End Try
        Else
            MsgBox("Please select a row to edit.", vbExclamation)
        End If

    End Sub


    Private Function duplication(ByVal dept As String, ByVal grade As String, ByVal section As String, ByVal strand As String, ByVal currentID As Integer) As Boolean
        Dim con As New MySqlConnection(GlobalVarsModule.connectionString)
        Dim query As String = ""
        Dim count As Integer = 0

        If dept = "Junior High School" OrElse dept = "Elementary" Then
            query = "SELECT COUNT(*) FROM `section_tbl` WHERE `Department` = @dept AND `GradeLevel` = @grade AND `Section` = @section AND `ID` <> @currentID"
        ElseIf dept = "Senior High School" Then
            query = "SELECT COUNT(*) FROM `section_tbl` WHERE `Department` = @dept AND `GradeLevel` = @grade AND `Strand` = @strand AND `ID` <> @currentID"
        End If

        If String.IsNullOrWhiteSpace(query) Then
            Return False
        End If

        Try
            con.Open()
            Dim com As New MySqlCommand(query, con)
            com.Parameters.AddWithValue("@dept", dept)
            com.Parameters.AddWithValue("@grade", grade)
            com.Parameters.AddWithValue("@currentID", currentID)

            If dept = "Junior High School" OrElse dept = "Elementary" Then
                com.Parameters.AddWithValue("@section", section)
            ElseIf dept = "Senior High School" Then
                com.Parameters.AddWithValue("@strand", strand)
            End If

            count = CInt(com.ExecuteScalar())

        Catch ex As Exception
            MsgBox(ex.Message, vbCritical)
            Return True
        Finally
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
        End Try

        Return count > 0
    End Function


    Private Sub btndelete_Click(sender As Object, e As EventArgs) Handles btndelete.Click

        If DataGridView1.SelectedRows.Count > 0 Then

            Dim dialogResult = MessageBox.Show("Are you sure you want to delete this section?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)

            If dialogResult = DialogResult.Yes Then

                Dim con As New MySqlConnection(GlobalVarsModule.connectionString)
                Dim selectedRow = DataGridView1.SelectedRows(0)
                Dim ID As Integer = CInt(selectedRow.Cells("ID").Value)
                Dim dept = selectedRow.Cells("Department").Value.ToString.Trim
                Dim grade = selectedRow.Cells("GradeLevel").Value.ToString.Trim
                Dim section = If(IsDBNull(selectedRow.Cells("Section").Value), "", selectedRow.Cells("Section").Value.ToString.Trim())
                Dim strand = If(IsDBNull(selectedRow.Cells("Strand").Value), "", selectedRow.Cells("Strand").Value.ToString.Trim())
                Dim deleteDescription As String = If(dept = "Senior High School", $"Department: {dept}, Grade Level: {grade}, Strand: {strand}", $"Department: {dept}, Grade Level: {grade}, Section: {section}")


                Try
                    con.Open()

                    Dim borrowerQuery As New MySqlCommand("", con)

                    If dept = "Junior High School" OrElse dept = "Elementary" Then
                        borrowerQuery.CommandText = "SELECT COUNT(*) FROM `borrower_tbl` WHERE Department = @dept AND Grade = @grade AND Section = @section"
                        borrowerQuery.Parameters.AddWithValue("@dept", dept)
                        borrowerQuery.Parameters.AddWithValue("@grade", grade)
                        borrowerQuery.Parameters.AddWithValue("@section", section)

                    ElseIf dept = "Senior High School" Then
                        borrowerQuery.CommandText = "SELECT COUNT(*) FROM `borrower_tbl` WHERE Department = @dept AND Grade = @grade AND Strand = @strand"
                        borrowerQuery.Parameters.AddWithValue("@dept", dept)
                        borrowerQuery.Parameters.AddWithValue("@grade", grade)
                        borrowerQuery.Parameters.AddWithValue("@strand", strand)
                    End If

                    Dim borrowerCount As Integer = CInt(borrowerQuery.ExecuteScalar())

                    If borrowerCount > 0 Then
                        MessageBox.Show("Cannot delete this section/strand. It is currently assigned to " & borrowerCount & " borrower(s).", "Information", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        Return
                    End If


                    Dim delete As New MySqlCommand("DELETE FROM `section_tbl` WHERE `ID` = @id", con)
                    delete.Parameters.AddWithValue("@id", ID)
                    delete.ExecuteNonQuery()

                    GlobalVarsModule.LogAudit(
                        actionType:="DELETE",
                        formName:="SECTION FORM",
                        description:=$"Deleted section/strand: {deleteDescription}",
                        recordID:=ID.ToString()
                    )

                    For Each form In Application.OpenForms
                        If TypeOf form Is AuditTrail Then
                            DirectCast(form, AuditTrail).refreshaudit()
                        End If
                    Next

                    For Each form In Application.OpenForms
                        If TypeOf form Is Borrower Then
                            DirectCast(form, Borrower).cbsecs()
                        End If
                    Next

                    MsgBox("Section deleted successfully.", vbInformation)
                    Section_Load(sender, e)
                    clearlahat()

                    Dim count As New MySqlCommand("SELECT COUNT(*) FROM `section_tbl`", con)
                    Dim rowCount As Long = CLng(count.ExecuteScalar())

                    If rowCount = 0 Then
                        Dim reset As New MySqlCommand("ALTER TABLE `section_tbl` AUTO_INCREMENT = 1", con)
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

    Private Sub txtsearch_TextChanged(sender As Object, e As EventArgs) Handles txtsearch.TextChanged

        HandleAutoRefreshPause(DataGridView1, txtsearch)

        Dim dt As DataTable = DirectCast(DataGridView1.DataSource, DataTable)
        If dt IsNot Nothing Then
            If txtsearch.Text.Trim() <> "" Then
                Dim filter As String = String.Format("Section LIKE '*{0}*' OR Department LIKE '*{0}*' OR Strand LIKE '*{0}*' OR GradeLevel LIKE '*{0}*'", txtsearch.Text.Trim())
                dt.DefaultView.RowFilter = filter
            Else
                dt.DefaultView.RowFilter = ""
            End If
        End If

    End Sub

    Private Sub txtsection_KeyDown(sender As Object, e As KeyEventArgs) Handles txtsection.KeyDown

        If e.Control AndAlso (e.KeyCode = Keys.V Or e.KeyCode = Keys.C Or e.KeyCode = Keys.X) Then
            e.SuppressKeyPress = True
        End If

        If e.KeyCode = Keys.Enter Then
            btnadd_Click(sender, e)
            e.Handled = True
        End If

    End Sub

    Private Sub txtsection_KeyPress(sender As Object, e As KeyPressEventArgs) Handles txtsection.KeyPress

        If Not Char.IsLetterOrDigit(e.KeyChar) AndAlso Not Char.IsControl(e.KeyChar) AndAlso Not Char.IsWhiteSpace(e.KeyChar) Then
            e.Handled = True
        End If


    End Sub

    Private Sub txtsearch_KeyDown(sender As Object, e As KeyEventArgs) Handles txtsearch.KeyDown

        If e.Control AndAlso (e.KeyCode = Keys.V Or e.KeyCode = Keys.C Or e.KeyCode = Keys.X) Then
            e.SuppressKeyPress = True
        End If

    End Sub

    Private Sub btnadd_KeyDown(sender As Object, e As KeyEventArgs) Handles btnadd.KeyDown

        If e.KeyCode = Keys.Enter Then
            btnadd_Click(sender, e)
            e.Handled = True
        End If

    End Sub

    Public Sub cbdeptss()

        Dim con As New MySqlConnection(GlobalVarsModule.connectionString)
        Dim com As String = "SELECT ID, Department FROM `department_tbl`"
        Dim adap As New MySqlDataAdapter(com, con)
        Dim ds As New DataTable

        adap.Fill(ds)

        cbdepartment.DataSource = ds
        cbdepartment.DisplayMember = "Department"
        cbdepartment.ValueMember = "ID"
        cbdepartment.SelectedIndex = -1

    End Sub

    Public Sub cbgradesu()

        Dim con As New MySqlConnection(GlobalVarsModule.connectionString)
        Dim com As String = "SELECT ID, Grade FROM `grade_tbl`"
        Dim adap As New MySqlDataAdapter(com, con)
        Dim ds As New DataTable

        adap.Fill(ds)

        cbgrade.DataSource = ds
        cbgrade.DisplayMember = "Grade"
        cbgrade.ValueMember = "ID"
        cbgrade.SelectedIndex = -1

    End Sub

    Public Sub cbstrandsu()

        Dim con As New MySqlConnection(GlobalVarsModule.connectionString)
        Dim com As String = "SELECT ID, Strand FROM `strand_tbl`"
        Dim adap As New MySqlDataAdapter(com, con)
        Dim ds As New DataTable

        adap.Fill(ds)

        cbstrand.DataSource = ds
        cbstrand.DisplayMember = "Strand"
        cbstrand.ValueMember = "ID"
        cbstrand.SelectedIndex = -1

    End Sub

    Private Sub cbdepartment_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbdepartment.SelectedIndexChanged

        If cbdepartment.SelectedValue Is Nothing OrElse cbdepartment.SelectedIndex = -1 Then
            cbgrade.Enabled = False
            Exit Sub
        End If

        If cbdepartment.SelectedIndex <> -1 Then
            Dim selectedDept = cbdepartment.GetItemText(cbdepartment.SelectedItem)

            cbgrade.DataSource = Nothing
            cbgrade.Items.Clear()


            cbgrade.Enabled = False
            txtsection.Enabled = False
            cbstrand.Enabled = False

            If selectedDept = "Junior High School" Then
                Dim con As New MySqlConnection(GlobalVarsModule.connectionString)
                Dim com = "SELECT ID, Grade FROM `grade_tbl` WHERE Grade BETWEEN 7 AND 10"
                Dim adap As New MySqlDataAdapter(com, con)
                Dim ds As New DataTable
                adap.Fill(ds)

                cbgrade.DataSource = ds
                cbgrade.DisplayMember = "Grade"
                cbgrade.ValueMember = "ID"
                cbgrade.SelectedIndex = -1

                txtsection.Visible = True
                cbstrand.Visible = False

                lbl_sectionandstrand.Text = "Section:"


                cbgrade.Enabled = True

            ElseIf selectedDept = "Senior High School" Then
                Dim con As New MySqlConnection(GlobalVarsModule.connectionString)
                Dim com = "SELECT ID, Grade FROM `grade_tbl` WHERE Grade BETWEEN 11 AND 12"
                Dim adap As New MySqlDataAdapter(com, con)
                Dim ds As New DataTable
                adap.Fill(ds)

                cbgrade.DataSource = ds
                cbgrade.DisplayMember = "Grade"
                cbgrade.ValueMember = "ID"
                cbgrade.SelectedIndex = -1

                cbstrand.Visible = True
                txtsection.Visible = False

                cbstrandsu()

                lbl_sectionandstrand.Text = "Strand:"

                cbgrade.Enabled = True

            ElseIf selectedDept = "Elementary" Then

                Dim con As New MySqlConnection(GlobalVarsModule.connectionString)
                Dim com = "SELECT ID, Grade FROM `grade_tbl` WHERE Grade BETWEEN 1 AND 6"
                Dim adap As New MySqlDataAdapter(com, con)
                Dim ds As New DataTable
                adap.Fill(ds)

                cbgrade.DataSource = ds
                cbgrade.DisplayMember = "Grade"
                cbgrade.ValueMember = "ID"
                cbgrade.SelectedIndex = -1

                txtsection.Visible = True
                cbstrand.Visible = False

                lbl_sectionandstrand.Text = "Section:"

                cbgrade.Enabled = True
            End If
        End If
    End Sub

    Private Sub btnclear_Click(sender As Object, e As EventArgs) Handles btnclear.Click
        clearlahat()
    End Sub

    Public Sub clearlahat()

        cbdepartment.DataSource = Nothing
        cbgrade.DataSource = Nothing
        cbstrand.DataSource = Nothing

        cbdeptss()


        txtsection.Text = ""
        txtsearch.Clear()


        cbgrade.Enabled = False
        txtsection.Enabled = False
        cbstrand.Enabled = False


        txtsection.Visible = True
        cbstrand.Visible = False

        rbjhs.Checked = False
        rbshs.Checked = False
        rbelem.Checked = False

        lbl_sectionandstrand.Text = "Section:"


        Dim dt As DataTable = DirectCast(DataGridView1.DataSource, DataTable)
        If dt IsNot Nothing Then
            dt.DefaultView.RowFilter = ""
        End If

        DataGridView1.ClearSelection()

    End Sub

    Private Sub cbgrade_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbgrade.SelectedIndexChanged

        If cbgrade.SelectedIndex <> -1 Then
            Dim selectedDept = cbdepartment.GetItemText(cbdepartment.SelectedItem)

            If selectedDept = "Junior High School" Then
                txtsection.Enabled = True
                cbstrand.Enabled = False
            ElseIf selectedDept = "Senior High School" Then
                cbstrand.Enabled = True
                txtsection.Enabled = False
            ElseIf selectedDept = "Elementary" Then
                txtsection.Enabled = True
                cbstrand.Enabled = False
            End If
        Else

            txtsection.Enabled = False
            cbstrand.Enabled = False
        End If
    End Sub

    Private Sub rbelem_CheckedChanged(sender As Object, e As EventArgs) Handles rbelem.CheckedChanged
        Dim dt = DirectCast(DataGridView1.DataSource, DataTable)


        PauseAutoRefresh(DataGridView1)

        If dt IsNot Nothing AndAlso rbelem.Checked Then
            dt.DefaultView.RowFilter = "Department = 'Elementary'"
        ElseIf dt IsNot Nothing AndAlso Not rbjhs.Checked AndAlso Not rbshs.Checked AndAlso Not rbelem.Checked Then
            dt.DefaultView.RowFilter = ""
        End If


        If Not rbelem.Checked AndAlso Not rbjhs.Checked AndAlso Not rbshs.Checked Then
            ResumeAutoRefresh(DataGridView1)
        End If
    End Sub

    Private Sub rbjhs_CheckedChanged(sender As Object, e As EventArgs) Handles rbjhs.CheckedChanged
        Dim dt = DirectCast(DataGridView1.DataSource, DataTable)

        PauseAutoRefresh(DataGridView1)

        If dt IsNot Nothing AndAlso rbjhs.Checked Then
            dt.DefaultView.RowFilter = "Department = 'Junior High School'"
        ElseIf dt IsNot Nothing AndAlso Not rbjhs.Checked AndAlso Not rbshs.Checked AndAlso Not rbelem.Checked Then
            dt.DefaultView.RowFilter = ""
        End If

        If Not rbelem.Checked AndAlso Not rbjhs.Checked AndAlso Not rbshs.Checked Then
            ResumeAutoRefresh(DataGridView1)
        End If
    End Sub

    Private Sub rbshs_CheckedChanged(sender As Object, e As EventArgs) Handles rbshs.CheckedChanged
        Dim dt = DirectCast(DataGridView1.DataSource, DataTable)

        PauseAutoRefresh(DataGridView1)

        If dt IsNot Nothing AndAlso rbshs.Checked Then
            dt.DefaultView.RowFilter = "Department = 'Senior High School'"
        ElseIf dt IsNot Nothing AndAlso Not rbjhs.Checked AndAlso Not rbshs.Checked AndAlso Not rbelem.Checked Then
            dt.DefaultView.RowFilter = ""
        End If

        If Not rbelem.Checked AndAlso Not rbjhs.Checked AndAlso Not rbshs.Checked Then
            ResumeAutoRefresh(DataGridView1)
        End If
    End Sub


    Private Sub DataGridView1_CellClick_1(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellClick
        If e.RowIndex >= 0 Then
            Dim row = DataGridView1.Rows(e.RowIndex)


            RemoveHandler cbdepartment.SelectedIndexChanged, AddressOf cbdepartment_SelectedIndexChanged
            RemoveHandler cbgrade.SelectedIndexChanged, AddressOf cbgrade_SelectedIndexChanged


            Dim deptValue As String = row.Cells("Department").Value.ToString()
            Dim gradeValue As String = row.Cells("GradeLevel").Value.ToString()
            Dim sectionValue = row.Cells("Section").Value
            Dim strandValue = row.Cells("Strand").Value


            cbdepartment.Text = deptValue


            cbdepartment_SelectedIndexChanged(cbdepartment, EventArgs.Empty)

            cbgrade.Text = gradeValue

            If deptValue = "Junior High School" OrElse deptValue = "Elementary" Then
                txtsection.Visible = True
                cbstrand.Visible = False
                lbl_sectionandstrand.Text = "Section:"
                txtsection.Text = If(IsDBNull(sectionValue), "", sectionValue.ToString())
                txtsection.Enabled = True
                cbstrand.Enabled = False
            ElseIf deptValue = "Senior High School" Then
                cbstrand.Visible = True
                txtsection.Visible = False
                lbl_sectionandstrand.Text = "Strand:"
                cbstrand.Text = If(IsDBNull(strandValue), "", strandValue.ToString())
                cbstrand.Enabled = True
                txtsection.Enabled = False
            End If

            AddHandler cbdepartment.SelectedIndexChanged, AddressOf cbdepartment_SelectedIndexChanged
            AddHandler cbgrade.SelectedIndexChanged, AddressOf cbgrade_SelectedIndexChanged
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

    Private Sub btnclear_MouseHover(sender As Object, e As EventArgs) Handles btnclear.MouseHover
        Cursor = Cursors.Hand
    End Sub

    Private Sub btnclear_MouseLeave(sender As Object, e As EventArgs) Handles btnclear.MouseLeave
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