Imports System.Data
Imports System.Security.Policy
Imports MySql.Data.MySqlClient

Public Class Section

    Private selectedSectionID As Integer = 0


    Private Function NormalizeDept(ByVal d As String) As String
        If String.IsNullOrWhiteSpace(d) Then Return ""
        Return d.Trim().ToLower().Replace(".", "").Replace(" ", "").Replace("-", "").Replace("_", "")
    End Function

    Private Function IsJHS(ByVal d As String) As Boolean
        Select Case NormalizeDept(d)
            Case "juniorhighschool", "juniorhigh", "jrhighschool", "jrhigh", "jhs"
                Return True
        End Select
        Return False
    End Function

    Private Function IsSHS(ByVal d As String) As Boolean
        Select Case NormalizeDept(d)
            Case "seniorhighschool", "seniorhigh", "srhighschool", "srhigh", "shs"
                Return True
        End Select
        Return False
    End Function

    Private Function IsElementary(ByVal d As String) As Boolean
        Select Case NormalizeDept(d)
            Case "elementary", "elementaryschool", "gradeschool", "elem"
                Return True
        End Select
        Return False
    End Function

    Private Sub Section_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        DisablePaste_AllTextBoxes()

        TopMost = True
        Me.Refresh()

        DataGridView1.AutoSizeColumnsMode =
        DataGridViewAutoSizeColumnsMode.Fill

        DataGridView1.AllowUserToOrderColumns = False

        refreshsecs()
        PopulateFilter()

        AddHandler cbgrade.DropDown, AddressOf RefreshComboBoxes
        AddHandler cbstrand.DropDown, AddressOf RefreshComboBoxes
        AddHandler GlobalVarsModule.DatabaseUpdated, AddressOf OnDatabaseUpdated
        AddHandler cbdepartment.SelectedIndexChanged,
        AddressOf cbdepartment_SelectedIndexChanged

        GlobalVarsModule.EnableCapitalizeFirstLetterForControls(Me)

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
            AutoRefreshComboBox(cbstrand, "SELECT ID, Strand FROM strand_tbl ORDER BY Strand", "Strand", "ID")
            cbgrade.Enabled = False
        Catch ex As Exception
            Debug.WriteLine("Auto-refresh ComboBox failed: " & ex.Message)
        End Try
    End Sub


    Public Sub PopulateFilter()

        Dim con As New MySqlConnection(GlobalVarsModule.connectionString)
        Dim query As String = "SELECT DISTINCT Department FROM department_tbl ORDER BY Department"

        Try
            con.Open()

            Dim adap As New MySqlDataAdapter(query, con)
            Dim dt As New DataTable
            adap.Fill(dt)

            cbfilter.DataSource = dt
            cbfilter.DisplayMember = "Department"
            cbfilter.ValueMember = "Department"
            cbfilter.SelectedIndex = -1

        Catch ex As Exception
            Debug.WriteLine("cbfilter populate error: " & ex.Message)
        Finally
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
        End Try

    End Sub

    Private Sub cbfilter_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbfilter.SelectedIndexChanged

        Dim dt As DataTable =
            DirectCast(DataGridView1.DataSource, DataTable)

        If dt Is Nothing Then Exit Sub

        If cbfilter.SelectedIndex <> -1 Then

            Dim selectedDept As String =
                cbfilter.GetItemText(cbfilter.SelectedItem)

            dt.DefaultView.RowFilter =
                $"Department = '{selectedDept.Replace("'", "''")}'"

        Else

            dt.DefaultView.RowFilter = ""

        End If

    End Sub

    Private Async Sub OnDatabaseUpdated()
        Dim query As String = "SELECT * FROM `section_tbl`"
        Await GlobalVarsModule.LoadToGridAsync(DataGridView1, query)
        SetupGridStyle()
        cbdeptss()
        clearlahat()
        PopulateFilter()

        AutoRefreshComboBox(cbdepartment, "SELECT ID, Department FROM department_tbl ORDER BY Department", "Department", "ID")
        AutoRefreshComboBox(cbgrade, "SELECT ID, Grade FROM grade_tbl ORDER BY Grade", "Grade", "ID")
        AutoRefreshComboBox(cbstrand, "SELECT ID, Strand FROM strand_tbl ORDER BY Strand", "Strand", "ID")

    End Sub


    Private Sub PinEditDeleteColumnsToEnd()
        Try
            If DataGridView1.Columns.Contains("ID") Then
                DataGridView1.Columns("ID").Visible = False
            End If

            Dim total As Integer = DataGridView1.Columns.Count

            If DataGridView1.Columns.Contains("Edit") Then
                DataGridView1.Columns("Edit").DisplayIndex = Math.Max(0, total - 2)
            End If

            If DataGridView1.Columns.Contains("Delete") Then
                DataGridView1.Columns("Delete").DisplayIndex = Math.Max(0, total - 1)
            End If

        Catch ex As Exception
            Debug.WriteLine("Pin columns error: " & ex.Message)
        End Try
    End Sub


    Private Sub DataGridView1_ColumnAdded(sender As Object, e As DataGridViewColumnEventArgs) Handles DataGridView1.ColumnAdded
        PinEditDeleteColumnsToEnd()
    End Sub

    Private Sub DataGridView1_DataBindingComplete(
    sender As Object,
    e As DataGridViewBindingCompleteEventArgs
) Handles DataGridView1.DataBindingComplete

        Try
            PinEditDeleteColumnsToEnd()

        Catch ex As Exception
            Debug.WriteLine("Grid column layout error: " & ex.Message)
        End Try

    End Sub

    Private Sub RefreshComboBoxes(sender As Object, e As EventArgs)
        Dim cb As ComboBox = DirectCast(sender, ComboBox)

        Using con As New MySqlConnection(GlobalVarsModule.connectionString)
            Dim query As String = ""

            Select Case cb.Name.ToLower()
                Case "cbdepartment"
                    query = "SELECT Department FROM department_tbl ORDER BY Department"

                Case "cbgrade"

                    Dim selectedDept As String = ""

                    Try
                        If cbdepartment IsNot Nothing AndAlso cbdepartment.SelectedIndex <> -1 Then
                            selectedDept = cbdepartment.GetItemText(cbdepartment.SelectedItem)
                        End If
                    Catch
                        selectedDept = ""
                    End Try

                    If Not String.IsNullOrEmpty(selectedDept) Then

                        If IsJHS(selectedDept) Then
                            query = "SELECT ID, Grade FROM grade_tbl WHERE Grade BETWEEN 7 AND 10 ORDER BY CAST(Grade AS UNSIGNED)"

                        ElseIf IsSHS(selectedDept) Then
                            query = "SELECT ID, Grade FROM grade_tbl WHERE Grade BETWEEN 11 AND 12 ORDER BY CAST(Grade AS UNSIGNED)"

                        ElseIf IsElementary(selectedDept) Then
                            query = "SELECT ID, Grade FROM grade_tbl WHERE Grade BETWEEN 1 AND 6 ORDER BY CAST(Grade AS UNSIGNED)"

                        Else
                            query = "SELECT ID, Grade FROM grade_tbl ORDER BY CAST(Grade AS UNSIGNED)"
                        End If

                    Else
                        query = "SELECT ID, Grade FROM grade_tbl ORDER BY CAST(Grade AS UNSIGNED)"
                    End If

            End Select

            If query <> "" Then

                Dim dt As New DataTable()
                Dim da As New MySqlDataAdapter(query, con)
                da.Fill(dt)

                If dt.Columns.Contains("Grade") Then

                    cb.DataSource = dt
                    cb.DisplayMember = "Grade"
                    cb.ValueMember = If(dt.Columns.Contains("ID"), "ID", "Grade")
                    cb.SelectedIndex = -1

                Else

                    cb.DataSource = dt
                    cb.DisplayMember = dt.Columns(0).ColumnName
                    cb.ValueMember = dt.Columns(0).ColumnName
                    cb.SelectedIndex = -1

                End If

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

    Private Sub cbstrand_DropDown(sender As Object, e As EventArgs) Handles cbstrand.DropDown

        Try
            cbstrand.DataSource = Nothing
            cbstrandsu()

        Catch ex As Exception
            Debug.WriteLine("Error refreshing strand combo: " & ex.Message)
        End Try

    End Sub


    Private Sub SetupGridStyle()

        Try

            If DataGridView1.Columns.Contains("ID") Then
                DataGridView1.Columns("ID").Visible = False
            End If

            DataGridView1.EnableHeadersVisualStyles = False
            DataGridView1.ColumnHeadersDefaultCellStyle.BackColor =
            Color.FromArgb(207, 58, 109)

            DataGridView1.ColumnHeadersDefaultCellStyle.ForeColor =
            Color.White

            PinEditDeleteColumnsToEnd()

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


        If IsJHS(dept) OrElse IsElementary(dept) Then

            secs = txtsection.Text.Trim
            strand = ""

            If String.IsNullOrWhiteSpace(secs) Then

                MsgBox(
                    "Please fill in the required fields.",
                    vbExclamation,
                    "Missing Information")

                Exit Sub

            End If

            entryDescription =
                $"Department: {dept}, Grade Level: {grade}, Section: {secs}"


        ElseIf IsSHS(dept) Then

            secs = ""

            If cbstrand.SelectedIndex <> -1 Then
                strand = cbstrand.GetItemText(cbstrand.SelectedItem)
            End If

            If String.IsNullOrWhiteSpace(strand) Then

                MsgBox(
                    "Please fill in the required fields.",
                    vbExclamation,
                    "Missing Information")

                Exit Sub

            End If

            entryDescription =
                $"Department: {dept}, Grade Level: {grade}, Strand: {strand}"

        End If


        If String.IsNullOrWhiteSpace(dept) OrElse
           String.IsNullOrWhiteSpace(grade) Then

            MsgBox(
                "Please select Department and Grade Level.",
                vbExclamation,
                "Missing Information")

            Exit Sub

        End If


        Try


            If selectedSectionID > 0 Then

                Dim oldDept As String = ""
                Dim oldGrade As String = ""
                Dim oldSecs As String = ""
                Dim oldStrand As String = ""


                For Each row As DataGridViewRow In DataGridView1.Rows

                    If row.IsNewRow Then Continue For

                    Dim rowID As Integer

                    If Integer.TryParse(
                        If(row.Cells("ID").Value Is Nothing,
                           "",
                           row.Cells("ID").Value.ToString()),
                        rowID) Then

                        If rowID = selectedSectionID Then

                            oldDept = SafeCellValue(row, "Department").Trim()
                            oldGrade = SafeCellValue(row, "GradeLevel").Trim()
                            oldSecs = SafeCellValue(row, "Section").Trim()
                            oldStrand = SafeCellValue(row, "Strand").Trim()

                            Exit For

                        End If

                    End If

                Next


                Dim oldEntry As String = ""

                If IsSHS(oldDept) Then

                    oldEntry =
                        $"Dept: {oldDept}, Grade: {oldGrade}, Strand: {oldStrand}"

                Else

                    oldEntry =
                        $"Dept: {oldDept}, Grade: {oldGrade}, Section: {oldSecs}"

                End If


                Dim newDept = dept
                Dim newGrade = grade
                Dim newSecs = secs
                Dim newStrand = strand
                Dim newEntry = entryDescription


                If oldDept.Equals(newDept) AndAlso
                   oldGrade.Equals(newGrade) AndAlso
                   oldSecs.Equals(newSecs) AndAlso
                   oldStrand.Equals(newStrand) Then

                    MsgBox(
                        "No changes were made.",
                        vbExclamation,
                        "No Update")

                    Exit Sub

                End If


                If duplication(
                    newDept,
                    newGrade,
                    newSecs,
                    newStrand,
                    selectedSectionID) Then

                    MsgBox(
                        "The updated entry already exists in the database. Please use another Section or Strand name.",
                        vbExclamation,
                        "Duplication Error")

                    Exit Sub

                End If


                con.Open()


                Dim com As New MySqlCommand(
                    "UPDATE `section_tbl` SET " &
                    "`Department` = @newDept, " &
                    "`GradeLevel` = @newGrade, " &
                    "`Section` = @newSection, " &
                    "`Strand` = @newStrand " &
                    "WHERE `ID` = @id",
                    con)


                com.Parameters.AddWithValue("@newDept", newDept)
                com.Parameters.AddWithValue("@newGrade", newGrade)

                com.Parameters.AddWithValue(
                    "@newSection",
                    If(
                        String.IsNullOrWhiteSpace(newSecs),
                        CType(DBNull.Value, Object),
                        newSecs))

                com.Parameters.AddWithValue(
                    "@newStrand",
                    If(
                        String.IsNullOrWhiteSpace(newStrand),
                        CType(DBNull.Value, Object),
                        newStrand))

                com.Parameters.AddWithValue("@id", selectedSectionID)

                com.ExecuteNonQuery()


                Dim comsss As New MySqlCommand("", con)


                If IsSHS(oldDept) Then

                    comsss.CommandText =
                        "UPDATE `borrower_tbl` SET " &
                        "`Department` = @newDept, " &
                        "`Grade` = @newGrade, " &
                        "`Strand` = @newStrand, " &
                        "`Section` = NULL " &
                        "WHERE `Department` = @oldDept " &
                        "AND `Grade` = @oldGrade " &
                        "AND `Strand` = @oldStrand"

                    comsss.Parameters.AddWithValue(
                        "@newStrand",
                        newStrand)

                    comsss.Parameters.AddWithValue(
                        "@oldStrand",
                        oldStrand)

                Else

                    comsss.CommandText =
                        "UPDATE `borrower_tbl` SET " &
                        "`Department` = @newDept, " &
                        "`Grade` = @newGrade, " &
                        "`Section` = @newSection, " &
                        "`Strand` = NULL " &
                        "WHERE `Department` = @oldDept " &
                        "AND `Grade` = @oldGrade " &
                        "AND `Section` = @oldSection"

                    comsss.Parameters.AddWithValue(
                        "@newSection",
                        newSecs)

                    comsss.Parameters.AddWithValue(
                        "@oldSection",
                        oldSecs)

                End If


                comsss.Parameters.AddWithValue(
                    "@newDept",
                    newDept)

                comsss.Parameters.AddWithValue(
                    "@newGrade",
                    newGrade)

                comsss.Parameters.AddWithValue(
                    "@oldDept",
                    oldDept)

                comsss.Parameters.AddWithValue(
                    "@oldGrade",
                    oldGrade)

                comsss.ExecuteNonQuery()



                LogAudit(
                    actionType:="UPDATE",
                    formName:="SECTION FORM",
                    description:=$"Updated section/strand ID {selectedSectionID} from '{oldEntry}' to '{newEntry}'",
                    recordID:=selectedSectionID.ToString,
                    oldValue:=oldEntry,
                    newValue:=newEntry
                )



                For Each form In Application.OpenForms

                    If TypeOf form Is AuditTrail Then

                        DirectCast(
                            form,
                            AuditTrail).refreshaudit()

                    ElseIf TypeOf form Is Borrower Then

                        Dim borrower =
                            DirectCast(form, Borrower)

                        borrower.cbsecs()
                        borrower.refreshData()

                    ElseIf TypeOf form Is MainForm Then

                        Dim load =
                            DirectCast(form, MainForm)

                        load.loadsu()

                    End If

                Next


                MsgBox(
                    "Updated successfully!",
                    vbInformation)


            Else



                If duplication(
                    dept,
                    grade,
                    secs,
                    strand) Then

                    MsgBox(
                        "The entry already exists in the database. Please use an another Section or Strand name.",
                        vbExclamation,
                        "Duplication Error")

                    Exit Sub

                End If


                con.Open()


                Dim com As New MySqlCommand(
                    "INSERT INTO `section_tbl` " &
                    "(`Department`, `GradeLevel`, `Section`, `Strand`) " &
                    "VALUES (@dept, @grade, @section, @strand); " &
                    "SELECT LAST_INSERT_ID();",
                    con)


                com.Parameters.AddWithValue(
                    "@dept",
                    dept)

                com.Parameters.AddWithValue(
                    "@grade",
                    grade)

                com.Parameters.AddWithValue(
                    "@section",
                    If(
                        String.IsNullOrWhiteSpace(secs),
                        CType(DBNull.Value, Object),
                        secs))

                com.Parameters.AddWithValue(
                    "@strand",
                    If(
                        String.IsNullOrWhiteSpace(strand),
                        CType(DBNull.Value, Object),
                        strand))


                newID =
                    Convert.ToInt32(
                        com.ExecuteScalar())


                GlobalVarsModule.LogAudit(
                    actionType:="ADD",
                    formName:="SECTION FORM",
                    description:=$"Added new section/strand: {entryDescription}",
                    recordID:=newID.ToString()
                )


                For Each form In Application.OpenForms

                    If TypeOf form Is AuditTrail Then

                        DirectCast(
                            form,
                            AuditTrail).refreshaudit()

                    End If

                Next


                For Each form In Application.OpenForms

                    If TypeOf form Is Borrower Then

                        Dim borrower =
                            DirectCast(form, Borrower)

                        borrower.cbsecs()

                    End If

                Next


                MsgBox(
                    "Section added successfully",
                    vbInformation)

            End If


            selectedSectionID = 0

            clearlahat()

            DataGridView1.ClearSelection()
            DataGridView1.CurrentCell = Nothing

            refreshsecs()

            DataGridView1.ClearSelection()
            DataGridView1.CurrentCell = Nothing


        Catch ex As Exception

            MsgBox(
                ex.Message,
                vbCritical)

        Finally

            If con.State = ConnectionState.Open Then
                con.Close()

            End If

        End Try

    End Sub


    Private Function duplication(
        ByVal dept As String,
        ByVal grade As String,
        ByVal section As String,
        ByVal strand As String) As Boolean

        Dim con As New MySqlConnection(
            GlobalVarsModule.connectionString)

        Dim query As String = ""
        Dim count As Integer = 0
        Dim useSection As Boolean = IsJHS(dept) OrElse IsElementary(dept)
        Dim useStrand As Boolean = IsSHS(dept)


        If useSection Then

            query =
                "SELECT COUNT(*) FROM `section_tbl` " &
                "WHERE `Department` = @dept " &
                "AND `GradeLevel` = @grade " &
                "AND `Section` = @section"

        ElseIf useStrand Then

            query =
                "SELECT COUNT(*) FROM `section_tbl` " &
                "WHERE `Department` = @dept " &
                "AND `GradeLevel` = @grade " &
                "AND `Strand` = @strand"

        End If


        If String.IsNullOrWhiteSpace(query) Then
            Return False
        End If


        Try

            con.Open()

            Dim com As New MySqlCommand(
                query,
                con)

            com.Parameters.AddWithValue(
                "@dept",
                dept)

            com.Parameters.AddWithValue(
                "@grade",
                grade)


            If useSection Then

                com.Parameters.AddWithValue(
                    "@section",
                    section)

            ElseIf useStrand Then

                com.Parameters.AddWithValue(
                    "@strand",
                    strand)

            End If


            count =
                CInt(
                    com.ExecuteScalar())


        Catch ex As Exception

            MsgBox(
                ex.Message,
                vbCritical)

            Return True

        Finally

            If con.State = ConnectionState.Open Then
                con.Close()

            End If

        End Try


        Return count > 0

    End Function


    Private Function duplication(
        ByVal dept As String,
        ByVal grade As String,
        ByVal section As String,
        ByVal strand As String,
        ByVal currentID As Integer) As Boolean

        Dim con As New MySqlConnection(
            GlobalVarsModule.connectionString)

        Dim query As String = ""
        Dim count As Integer = 0
        Dim useSection As Boolean = IsJHS(dept) OrElse IsElementary(dept)
        Dim useStrand As Boolean = IsSHS(dept)


        If useSection Then

            query =
                "SELECT COUNT(*) FROM `section_tbl` " &
                "WHERE `Department` = @dept " &
                "AND `GradeLevel` = @grade " &
                "AND `Section` = @section " &
                "AND `ID` <> @currentID"

        ElseIf useStrand Then

            query =
                "SELECT COUNT(*) FROM `section_tbl` " &
                "WHERE `Department` = @dept " &
                "AND `GradeLevel` = @grade " &
                "AND `Strand` = @strand " &
                "AND `ID` <> @currentID"

        End If


        If String.IsNullOrWhiteSpace(query) Then
            Return False
        End If


        Try

            con.Open()

            Dim com As New MySqlCommand(
                query,
                con)

            com.Parameters.AddWithValue(
                "@dept",
                dept)

            com.Parameters.AddWithValue(
                "@grade",
                grade)

            com.Parameters.AddWithValue(
                "@currentID",
                currentID)


            If useSection Then

                com.Parameters.AddWithValue(
                    "@section",
                    section)

            ElseIf useStrand Then

                com.Parameters.AddWithValue(
                    "@strand",
                    strand)

            End If


            count =
                CInt(
                    com.ExecuteScalar())


        Catch ex As Exception

            MsgBox(
                ex.Message,
                vbCritical)

            Return True

        Finally

            If con.State = ConnectionState.Open Then
                con.Close()

            End If

        End Try


        Return count > 0

    End Function


    Private Sub DataGridView1_CellContentClick(
        sender As Object,
        e As DataGridViewCellEventArgs
    ) Handles DataGridView1.CellContentClick

        If e.RowIndex < 0 Then Exit Sub
        If e.ColumnIndex < 0 Then Exit Sub


        Dim columnName As String =
            DataGridView1.Columns(e.ColumnIndex).Name


        Dim row As DataGridViewRow =
            DataGridView1.Rows(e.RowIndex)


        If columnName = "Edit" Then

            Dim ID As Integer


            If row.Cells("ID").Value Is Nothing OrElse
               IsDBNull(row.Cells("ID").Value) Then

                MsgBox(
                    "Unable to get the Section ID.",
                    vbCritical,
                    "Edit Error")

                Exit Sub

            End If


            If Not Integer.TryParse(
                row.Cells("ID").Value.ToString(),
                ID) Then

                MsgBox(
                    "Invalid Section ID.",
                    vbCritical,
                    "Edit Error")

                Exit Sub

            End If


            selectedSectionID = ID


            Dim deptValue As String =
                SafeCellValue(
                    row,
                    "Department").Trim()

            Dim gradeValue As String =
                SafeCellValue(
                    row,
                    "GradeLevel").Trim()

            Dim sectionValue As String =
                SafeCellValue(
                    row,
                    "Section").Trim()

            Dim strandValue As String =
                SafeCellValue(
                    row,
                    "Strand").Trim()


            RemoveHandler cbdepartment.SelectedIndexChanged,
                AddressOf cbdepartment_SelectedIndexChanged

            RemoveHandler cbgrade.SelectedIndexChanged,
                AddressOf cbgrade_SelectedIndexChanged


            Try

                cbdepartment.Text =
                    deptValue

                cbdepartment_SelectedIndexChanged(
                    cbdepartment,
                    EventArgs.Empty)


                cbgrade.Text =
                    gradeValue


                If IsJHS(deptValue) OrElse IsElementary(deptValue) Then

                    txtsection.Visible = True
                    txtsection.Enabled = True

                    cbstrand.Visible = False
                    cbstrand.Enabled = False



                    txtsection.Text =
                        sectionValue


                ElseIf IsSHS(deptValue) Then

                    txtsection.Visible = False
                    txtsection.Enabled = False

                    cbstrand.Visible = True
                    cbstrand.Enabled = True

                    lbl_sectionandstrand.Text =
                        "Strand:"

                    cbstrand.Text =
                        strandValue

                End If


                DataGridView1.ClearSelection()

                row.Selected = True


                If DataGridView1.Columns.Contains("Edit") Then

                    DataGridView1.CurrentCell =
                        row.Cells("Edit")

                End If


            Catch ex As Exception

                MessageBox.Show(
                    "Error loading section details: " &
                    ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error)

            Finally

                AddHandler cbdepartment.SelectedIndexChanged,
                    AddressOf cbdepartment_SelectedIndexChanged

                AddHandler cbgrade.SelectedIndexChanged,
                    AddressOf cbgrade_SelectedIndexChanged

            End Try


            Exit Sub

        End If


        If columnName = "Delete" Then

            Dim ID As Integer


            If row.Cells("ID").Value Is Nothing OrElse
               IsDBNull(row.Cells("ID").Value) Then

                MsgBox(
                    "Unable to get the Section ID.",
                    vbCritical,
                    "Delete Error")

                Exit Sub

            End If


            If Not Integer.TryParse(
                row.Cells("ID").Value.ToString(),
                ID) Then

                MsgBox(
                    "Invalid Section ID.",
                    vbCritical,
                    "Delete Error")

                Exit Sub

            End If


            Dim dept As String =
                SafeCellValue(
                    row,
                    "Department").Trim()

            Dim grade As String =
                SafeCellValue(
                    row,
                    "GradeLevel").Trim()

            Dim section As String =
                SafeCellValue(
                    row,
                    "Section").Trim()

            Dim strand As String =
                SafeCellValue(
                    row,
                    "Strand").Trim()


            Dim deleteDescription As String


            If IsSHS(dept) Then

                deleteDescription =
                    $"Department: {dept}, Grade Level: {grade}, Strand: {strand}"

            Else

                deleteDescription =
                    $"Department: {dept}, Grade Level: {grade}, Section: {section}"

            End If


            Dim dialogResult =
                MessageBox.Show(
                    "Are you sure you want to delete this section?",
                    "Confirm Delete",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning)


            If dialogResult <> DialogResult.Yes Then
                Exit Sub
            End If


            Dim con As New MySqlConnection(
                GlobalVarsModule.connectionString)


            Try

                con.Open()


                Dim borrowerQuery As New MySqlCommand("", con)


                If IsJHS(dept) OrElse IsElementary(dept) Then

                    borrowerQuery.CommandText =
                        "SELECT COUNT(*) FROM `borrower_tbl` " &
                        "WHERE Department = @dept " &
                        "AND Grade = @grade " &
                        "AND Section = @section"


                    borrowerQuery.Parameters.AddWithValue(
                        "@dept",
                        dept)

                    borrowerQuery.Parameters.AddWithValue(
                        "@grade",
                        grade)

                    borrowerQuery.Parameters.AddWithValue(
                        "@section",
                        section)


                ElseIf IsSHS(dept) Then

                    borrowerQuery.CommandText =
                        "SELECT COUNT(*) FROM `borrower_tbl` " &
                        "WHERE Department = @dept " &
                        "AND Grade = @grade " &
                        "AND Strand = @strand"


                    borrowerQuery.Parameters.AddWithValue(
                        "@dept",
                        dept)

                    borrowerQuery.Parameters.AddWithValue(
                        "@grade",
                        grade)

                    borrowerQuery.Parameters.AddWithValue(
                        "@strand",
                        strand)

                End If


                Dim borrowerCount As Integer =
                    Convert.ToInt32(
                        borrowerQuery.ExecuteScalar())


                If borrowerCount > 0 Then

                    MessageBox.Show(
                        "Cannot delete this section/strand. It is currently assigned to " &
                        borrowerCount &
                        " borrower(s).",
                        "Information",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning)

                    Exit Sub

                End If


                Dim delete As New MySqlCommand(
                    "DELETE FROM `section_tbl` WHERE `ID` = @id",
                    con)


                delete.Parameters.AddWithValue(
                    "@id",
                    ID)

                delete.ExecuteNonQuery()




                LogAudit(
                    actionType:="DELETE",
                    formName:="SECTION FORM",
                    description:=$"Deleted section/strand: {deleteDescription}",
                    recordID:=ID.ToString
                )



                For Each form In Application.OpenForms

                    If TypeOf form Is AuditTrail Then

                        DirectCast(
                            form,
                            AuditTrail).refreshaudit()

                    ElseIf TypeOf form Is Borrower Then

                        DirectCast(
                            form,
                            Borrower).cbsecs()

                    End If

                Next



                selectedSectionID = 0

                clearlahat()

                DataGridView1.ClearSelection()
                DataGridView1.CurrentCell = Nothing

                refreshsecs()

                DataGridView1.ClearSelection()
                DataGridView1.CurrentCell = Nothing


                MsgBox(
                    "Section deleted successfully.",
                    vbInformation)


                Dim count As New MySqlCommand(
                    "SELECT COUNT(*) FROM `section_tbl`",
                    con)

                Dim rowCount As Long =
                    Convert.ToInt64(
                        count.ExecuteScalar())


                If rowCount = 0 Then

                    Dim reset As New MySqlCommand(
                        "ALTER TABLE `section_tbl` AUTO_INCREMENT = 1",
                        con)

                    reset.ExecuteNonQuery()

                End If


            Catch ex As Exception

                MsgBox(
                    ex.Message,
                    vbCritical)

            Finally

                If con.State = ConnectionState.Open Then
                    con.Close()

                End If

            End Try

        End If

    End Sub


    Private Sub txtsearch_TextChanged(
        sender As Object,
        e As EventArgs
    ) Handles txtsearch.TextChanged

        HandleAutoRefreshPause(
            DataGridView1,
            txtsearch)


        Dim dt As DataTable =
            DirectCast(
                DataGridView1.DataSource,
                DataTable)


        If dt IsNot Nothing Then

            If txtsearch.Text.Trim() <> "" Then

                Dim filter As String =
                    String.Format(
                        "Section LIKE '*{0}*' OR Department LIKE '*{0}*' OR Strand LIKE '*{0}*' OR GradeLevel LIKE '*{0}*'",
                        txtsearch.Text.Trim())

                dt.DefaultView.RowFilter =
                    filter

            Else

                dt.DefaultView.RowFilter = ""

            End If

        End If

    End Sub


    Private Sub txtsection_KeyDown(
        sender As Object,
        e As KeyEventArgs
    ) Handles txtsection.KeyDown

        If e.Control AndAlso
           (e.KeyCode = Keys.V Or
            e.KeyCode = Keys.C Or
            e.KeyCode = Keys.X) Then

            e.SuppressKeyPress = True

        End If


        If e.KeyCode = Keys.Enter Then

            btnadd_Click(
                sender,
                e)

            e.Handled = True

        End If

    End Sub


    Private Sub txtsection_KeyPress(
        sender As Object,
        e As KeyPressEventArgs
    ) Handles txtsection.KeyPress

        If Not Char.IsLetterOrDigit(e.KeyChar) AndAlso
           Not Char.IsControl(e.KeyChar) AndAlso
           Not Char.IsWhiteSpace(e.KeyChar) Then

            e.Handled = True

        End If

    End Sub


    Private Sub txtsearch_KeyDown(
        sender As Object,
        e As KeyEventArgs
    ) Handles txtsearch.KeyDown

        If e.Control AndAlso
           (e.KeyCode = Keys.V Or
            e.KeyCode = Keys.C Or
            e.KeyCode = Keys.X) Then

            e.SuppressKeyPress = True

        End If

    End Sub


    Private Sub btnadd_KeyDown(
        sender As Object,
        e As KeyEventArgs
    ) Handles btnadd.KeyDown

        If e.KeyCode = Keys.Enter Then

            btnadd_Click(
                sender,
                e)

            e.Handled = True

        End If

    End Sub


    Public Sub cbdeptss()

        Dim con As New MySqlConnection(
            GlobalVarsModule.connectionString)

        Dim com As String =
            "SELECT ID, Department FROM `department_tbl`"

        Dim adap As New MySqlDataAdapter(
            com,
            con)

        Dim ds As New DataTable

        adap.Fill(ds)

        cbdepartment.DataSource = ds
        cbdepartment.DisplayMember = "Department"
        cbdepartment.ValueMember = "ID"
        cbdepartment.SelectedIndex = -1

    End Sub


    Public Sub cbgradesu()

        Dim con As New MySqlConnection(
            GlobalVarsModule.connectionString)

        Dim com As String =
            "SELECT ID, Grade FROM `grade_tbl` ORDER BY CAST(Grade AS UNSIGNED)"

        Dim adap As New MySqlDataAdapter(
            com,
            con)

        Dim ds As New DataTable

        adap.Fill(ds)

        cbgrade.DataSource = ds
        cbgrade.DisplayMember = "Grade"
        cbgrade.ValueMember = "ID"
        cbgrade.SelectedIndex = -1

    End Sub


    Public Sub cbstrandsu()

        Dim con As New MySqlConnection(
            GlobalVarsModule.connectionString)

        Dim com As String =
            "SELECT ID, Strand FROM `strand_tbl`"

        Dim adap As New MySqlDataAdapter(
            com,
            con)

        Dim ds As New DataTable

        adap.Fill(ds)

        cbstrand.DataSource = ds
        cbstrand.DisplayMember = "Strand"
        cbstrand.ValueMember = "ID"
        cbstrand.SelectedIndex = -1

    End Sub



    Private Sub cbdepartment_SelectedIndexChanged(
    sender As Object,
    e As EventArgs
) Handles cbdepartment.SelectedIndexChanged

        If cbdepartment.SelectedValue Is Nothing OrElse
       cbdepartment.SelectedIndex = -1 Then

            cbgrade.Enabled = False
            txtsection.Enabled = False
            cbstrand.Enabled = False
            Exit Sub

        End If


        If cbdepartment.SelectedIndex <> -1 Then

            Dim selectedDept =
            cbdepartment.GetItemText(
                cbdepartment.SelectedItem)


            cbgrade.DataSource = Nothing
            cbgrade.Items.Clear()


            cbgrade.Enabled = False
            txtsection.Enabled = False
            cbstrand.Enabled = False


            txtsection.Visible = True
            cbstrand.Visible = True

            Dim hasStrand As Boolean = IsSHS(selectedDept)
            cbstrand.Enabled = hasStrand

            If Not hasStrand Then
                cbstrand.Text = ""
            End If


            If IsJHS(selectedDept) Then

                Dim con As New MySqlConnection(
                connectionString)

                Dim com =
                "SELECT ID, Grade FROM `grade_tbl` WHERE Grade BETWEEN 7 AND 10"

                Dim adap As New MySqlDataAdapter(
                com,
                con)

                Dim ds As New DataTable

                adap.Fill(ds)

                cbgrade.DataSource = ds
                cbgrade.DisplayMember = "Grade"
                cbgrade.ValueMember = "ID"
                cbgrade.SelectedIndex = -1

                cbgrade.Enabled = True


            ElseIf IsSHS(selectedDept) Then

                Dim con As New MySqlConnection(
                connectionString)

                Dim com =
                "SELECT ID, Grade FROM `grade_tbl` WHERE Grade BETWEEN 11 AND 12"

                Dim adap As New MySqlDataAdapter(
                com,
                con)

                Dim ds As New DataTable

                adap.Fill(ds)

                cbgrade.DataSource = ds
                cbgrade.DisplayMember = "Grade"
                cbgrade.ValueMember = "ID"
                cbgrade.SelectedIndex = -1

                cbstrandsu()

                cbgrade.Enabled = True


            ElseIf IsElementary(selectedDept) Then

                Dim con As New MySqlConnection(
                connectionString)

                Dim com =
                "SELECT ID, Grade FROM `grade_tbl` WHERE Grade BETWEEN 1 AND 6"

                Dim adap As New MySqlDataAdapter(
                com,
                con)

                Dim ds As New DataTable

                adap.Fill(ds)

                cbgrade.DataSource = ds
                cbgrade.DisplayMember = "Grade"
                cbgrade.ValueMember = "ID"
                cbgrade.SelectedIndex = -1

                cbgrade.Enabled = True

            Else


                Dim con As New MySqlConnection(
                connectionString)

                Dim com =
                "SELECT ID, Grade FROM `grade_tbl` ORDER BY CAST(Grade AS UNSIGNED)"

                Dim adap As New MySqlDataAdapter(
                com,
                con)

                Dim ds As New DataTable

                adap.Fill(ds)

                cbgrade.DataSource = ds
                cbgrade.DisplayMember = "Grade"
                cbgrade.ValueMember = "ID"
                cbgrade.SelectedIndex = -1

                cbgrade.Enabled = True

            End If

        End If

    End Sub


    Private Sub btnclear_Click(
        sender As Object,
        e As EventArgs
    ) Handles btnclear.Click

        selectedSectionID = 0

        clearlahat()

        DataGridView1.ClearSelection()
        DataGridView1.CurrentCell = Nothing

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
        cbstrand.Visible = True



        If cbfilter IsNot Nothing Then
            cbfilter.SelectedIndex = -1
        End If


        Dim dt As DataTable =
            DirectCast(
                DataGridView1.DataSource,
                DataTable)


        If dt IsNot Nothing Then

            dt.DefaultView.RowFilter = ""

        End If


        DataGridView1.ClearSelection()

    End Sub

    Private Sub cbgrade_SelectedIndexChanged(
        sender As Object,
        e As EventArgs
    ) Handles cbgrade.SelectedIndexChanged

        If cbgrade.SelectedIndex <> -1 Then

            Dim selectedDept =
                cbdepartment.GetItemText(
                    cbdepartment.SelectedItem)


            txtsection.Enabled = True

            cbstrand.Enabled = IsSHS(selectedDept)

        Else

            txtsection.Enabled = False
            cbstrand.Enabled = False

        End If

    End Sub


    Private Function SafeCellValue(
        row As DataGridViewRow,
        columnName As String) As String

        Try

            If row.Cells(columnName).Value IsNot Nothing AndAlso
               Not IsDBNull(row.Cells(columnName).Value) Then

                Return row.Cells(columnName).Value.ToString()

            End If

        Catch
        End Try

        Return ""

    End Function


    Private Sub DataGridView1_CellClick_1(
        sender As Object,
        e As DataGridViewCellEventArgs
    ) Handles DataGridView1.CellClick

        If e.RowIndex < 0 Then Exit Sub
        If e.ColumnIndex < 0 Then Exit Sub


        Dim columnName As String =
            DataGridView1.Columns(e.ColumnIndex).Name



        If columnName = "Edit" OrElse
           columnName = "Delete" Then

            Exit Sub

        End If


        Dim connected As Boolean =
            IsDatabaseConnected()


        Dim row As DataGridViewRow =
            DataGridView1.Rows(e.RowIndex)


        RemoveHandler cbdepartment.SelectedIndexChanged,
            AddressOf cbdepartment_SelectedIndexChanged

        RemoveHandler cbgrade.SelectedIndexChanged,
            AddressOf cbgrade_SelectedIndexChanged


        Try

            Dim deptValue As String =
                SafeCellValue(
                    row,
                    "Department")

            Dim gradeValue As String =
                SafeCellValue(
                    row,
                    "GradeLevel")

            Dim sectionValue As String =
                SafeCellValue(
                    row,
                    "Section")

            Dim strandValue As String =
                SafeCellValue(
                    row,
                    "Strand")


            If connected Then

                cbdepartment.Text =
                    deptValue

                cbdepartment_SelectedIndexChanged(
                    cbdepartment,
                    EventArgs.Empty)

                cbgrade.Text =
                    gradeValue


                If IsJHS(deptValue) OrElse IsElementary(deptValue) Then

                    txtsection.Visible = True
                    cbstrand.Visible = False

                    txtsection.Text =
                        sectionValue

                    txtsection.Enabled = True
                    cbstrand.Enabled = False


                ElseIf IsSHS(deptValue) Then

                    cbstrand.Visible = True
                    txtsection.Visible = False

                    lbl_sectionandstrand.Text =
                        "Strand:"

                    cbstrand.Text =
                        strandValue

                    cbstrand.Enabled = True
                    txtsection.Enabled = False

                End If


            Else

                MessageBox.Show(
                    "Database not connected. Showing local grid data only.",
                    "Offline Mode",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information)


                cbdepartment.Text =
                    deptValue

                cbgrade.Text =
                    gradeValue


                If IsJHS(deptValue) OrElse IsElementary(deptValue) Then

                    txtsection.Visible = True
                    cbstrand.Visible = False



                    txtsection.Text =
                        sectionValue

                    txtsection.Enabled = True
                    cbstrand.Enabled = False


                ElseIf IsSHS(deptValue) Then

                    cbstrand.Visible = True
                    txtsection.Visible = False

                    lbl_sectionandstrand.Text =
                        "Strand:"

                    cbstrand.Text =
                        strandValue

                    cbstrand.Enabled = True
                    txtsection.Enabled = False

                End If

            End If


        Catch ex As Exception

            MessageBox.Show(
                "Error loading section details: " &
                ex.Message,
                "Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error)

        Finally

            AddHandler cbdepartment.SelectedIndexChanged,
                AddressOf cbdepartment_SelectedIndexChanged

            AddHandler cbgrade.SelectedIndexChanged,
                AddressOf cbgrade_SelectedIndexChanged

        End Try

    End Sub


    Private Sub btnadd_MouseHover(
        sender As Object,
        e As EventArgs
    ) Handles btnadd.MouseHover

        Cursor = Cursors.Hand

    End Sub


    Private Sub btnadd_MouseLeave(
        sender As Object,
        e As EventArgs
    ) Handles btnadd.MouseLeave

        Cursor = Cursors.Default

    End Sub


    Private Sub btnedit_MouseHover(
        sender As Object,
        e As EventArgs)

        Cursor = Cursors.Hand

    End Sub


    Private Sub btnedit_MouseLeave(
        sender As Object,
        e As EventArgs)

        Cursor = Cursors.Default

    End Sub


    Private Sub btndelete_MouseHover(
        sender As Object,
        e As EventArgs)

        Cursor = Cursors.Hand

    End Sub


    Private Sub btndelete_MouseLeave(
        sender As Object,
        e As EventArgs)

        Cursor = Cursors.Default

    End Sub


    Private Sub btnclear_MouseHover(
        sender As Object,
        e As EventArgs
    ) Handles btnclear.MouseHover

        Cursor = Cursors.Hand

    End Sub


    Private Sub btnclear_MouseLeave(
        sender As Object,
        e As EventArgs
    ) Handles btnclear.MouseLeave

        Cursor = Cursors.Default

    End Sub


    Private Sub DisablePaste_AllTextBoxes()

        For Each ctrl As Control In Me.Controls

            AddHandlerToTextBoxes_NoPaste(ctrl)

        Next

    End Sub


    Private Sub AddHandlerToTextBoxes_NoPaste(
        parent As Control)

        For Each ctrl As Control In parent.Controls

            If TypeOf ctrl Is TextBox Then

                Dim tb As TextBox =
                    CType(ctrl, TextBox)

                tb.ContextMenuStrip =
                    New ContextMenuStrip()

                AddHandler tb.KeyDown,
                    AddressOf BlockPasteKey

                AddHandler tb.MouseUp,
                    AddressOf BlockRightClick

            End If


            If ctrl.HasChildren Then

                AddHandlerToTextBoxes_NoPaste(ctrl)

            End If

        Next

    End Sub


    Private Sub BlockPasteKey(
        sender As Object,
        e As KeyEventArgs)

        If (e.Control AndAlso
            e.KeyCode = Keys.V) OrElse
           (e.Shift AndAlso
            e.KeyCode = Keys.Insert) Then

            e.SuppressKeyPress = True

        End If

    End Sub


    Private Sub BlockRightClick(
        sender As Object,
        e As MouseEventArgs)

        If e.Button = MouseButtons.Right Then

            Dim tb As TextBox =
                TryCast(
                    sender,
                    TextBox)


            If tb IsNot Nothing Then

                tb.ContextMenuStrip =
                    New ContextMenuStrip()

            End If

        End If

    End Sub


    Private Sub Section_KeyDown(
        sender As Object,
        e As KeyEventArgs
    ) Handles MyBase.KeyDown

        If e.KeyCode = Keys.Escape Then

            Me.Close()

        End If

    End Sub

End Class