Imports System.Management
Imports MySql.Data.MySqlClient
Public Class Borrower

    Private Sub Borrower_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        refreshData()

    End Sub

    Public Sub refreshData()

        Dim con As New MySqlConnection(connectionString)
        Dim com As String = "SELECT * FROM `borrower_tbl`"
        Dim adap As New MySqlDataAdapter(com, con)
        Dim dt As New DataSet

        Try
            con.Open()
            adap.Fill(dt, "INFO")
            DataGridView1.DataSource = dt.Tables("INFO")
            DataGridView1.Columns("ID").Visible = False
            DataGridView1.EnableHeadersVisualStyles = False
            DataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(207, 58, 109)
            DataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White
        Catch ex As Exception
            MessageBox.Show("Error loading data: " & ex.Message)
        Finally
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
        End Try

        DataGridView1.ClearSelection()
        DataGridView1.CurrentCell = Nothing

        cbgradee()
        cbsecs()
        cbdepts()
        cbstrandd()

        ClearFields()
        strandlocation()

    End Sub

    Public Sub strandlocation()

        cbstrand.Visible = True
        cbstrand.Location = New Point(942, 285)


        lblstrand.Visible = True
        lblstrand.Location = New Point(942, 266)

    End Sub

    Public Sub cbgradee()
        Dim con As New MySqlConnection(connectionString)
        Dim com As String = "SELECT ID, Grade FROM `grade_tbl`"
        Dim adap As New MySqlDataAdapter(com, con)
        Dim dt As New DataTable
        Try
            con.Open()
            adap.Fill(dt)
            cbgrade.DataSource = dt
            cbgrade.DisplayMember = "Grade"
            cbgrade.ValueMember = "ID"
            cbgrade.SelectedIndex = -1
        Catch ex As Exception
            MessageBox.Show("Error loading grades: " & ex.Message)
        Finally
            con.Close()
        End Try
    End Sub

    Public Sub cbsecs()
        Dim con As New MySqlConnection(connectionString)
        Dim com As String = "SELECT ID, Section FROM `section_tbl`"
        Dim adap As New MySqlDataAdapter(com, con)
        Dim dt As New DataTable
        Try
            con.Open()
            adap.Fill(dt)
            cbsection.DataSource = dt
            cbsection.DisplayMember = "Section"
            cbsection.ValueMember = "ID"
            cbsection.SelectedIndex = -1
        Catch ex As Exception
            MessageBox.Show("Error loading sections: " & ex.Message)
        Finally
            con.Close()
        End Try
    End Sub

    Public Sub cbdepts()
        Dim con As New MySqlConnection(connectionString)
        Dim com As String = "SELECT ID, Department FROM `department_tbl`"
        Dim adap As New MySqlDataAdapter(com, con)
        Dim dt As New DataTable
        Try
            con.Open()
            adap.Fill(dt)
            cbdepartment.DataSource = dt
            cbdepartment.DisplayMember = "Department"
            cbdepartment.ValueMember = "ID"
            cbdepartment.SelectedIndex = -1
        Catch ex As Exception
            MessageBox.Show("Error loading departments: " & ex.Message)
        Finally
            con.Close()
        End Try

    End Sub

    Public Sub cbstrandd()

        Dim con As New MySqlConnection(connectionString)
        Dim com As String = "SELECT ID, Strand FROM `strand_tbl`"
        Dim adap As New MySqlDataAdapter(com, con)
        Dim dt As New DataTable
        Try
            con.Open()
            adap.Fill(dt)
            cbstrand.DataSource = dt
            cbstrand.DisplayMember = "Strand"
            cbstrand.ValueMember = "ID"
            cbstrand.SelectedIndex = -1
        Catch ex As Exception
            MessageBox.Show("Error loading strands: " & ex.Message)
        Finally
            con.Close()
        End Try

    End Sub

    Private Sub Borrower_Shown(sender As Object, e As EventArgs) Handles MyBase.Shown

        DataGridView1.ClearSelection()

    End Sub
    Private Sub txtsearch_TextChanged(sender As Object, e As EventArgs) Handles txtsearch.TextChanged

        Dim dt As DataTable = DirectCast(DataGridView1.DataSource, DataTable)
        If dt IsNot Nothing Then
            Dim searchValue As String = txtsearch.Text.Trim()
            If Not String.IsNullOrEmpty(searchValue) Then
                Dim filter As String = String.Format("FirstName LIKE '%{0}%' OR Department LIKE '%{0}%' OR LastName LIKE '%{0}%' OR MiddleName LIKE '%{0}%' OR LRN LIKE '%{0}%' OR Borrower LIKE '%{0}%'", searchValue)
                dt.DefaultView.RowFilter = filter
            Else
                dt.DefaultView.RowFilter = ""
            End If
        End If

    End Sub

    Private Sub btnadd_Click(sender As Object, e As EventArgs) Handles btnadd.Click

        Dim con As New MySqlConnection(connectionString)
        Dim borrowerType As String = ""
        Dim middleName As String = txtmname.Text.Trim()
        Dim lrn As Object = DBNull.Value
        Dim employeeNo As Object = DBNull.Value
        Dim contactNumber As String = txtcontactnumber.Text.Trim()
        Dim firstName As String = txtfname.Text.Trim()
        Dim lastName As String = txtlname.Text.Trim()

        If rbstudent.Checked Then
            borrowerType = "Student"
        ElseIf rbteacher.Checked Then
            borrowerType = "Teacher"
        End If

        If rbnone.Checked Then
            middleName = "N/A"
        End If


        If String.IsNullOrWhiteSpace(firstName) OrElse String.IsNullOrWhiteSpace(lastName) OrElse String.IsNullOrWhiteSpace(contactNumber) Then
            MsgBox("Please fill in all required fields.", vbExclamation, "Missing Information")
            Exit Sub
        End If

        If borrowerType = "Student" Then
            If String.IsNullOrWhiteSpace(txtlrn.Text) Then
                MsgBox("Please enter the student's LRN.", vbExclamation, "Missing Information")
                Exit Sub
            End If
            lrn = txtlrn.Text.Trim()
        ElseIf borrowerType = "Teacher" Then
            If String.IsNullOrWhiteSpace(txtemployeeno.Text) Then
                MsgBox("Please enter the teacher's Employee Number.", vbExclamation, "Missing Information")
                Exit Sub
            End If
            employeeNo = txtemployeeno.Text.Trim()
        End If


        Try
            con.Open()


            Dim checkCom As New MySqlCommand("SELECT COUNT(*) FROM `borrower_tbl` WHERE (`LRN` = @LRN OR `EmployeeNo` = @EmployeeNo) OR `ContactNumber` = @ContactNumber", con)
            checkCom.Parameters.AddWithValue("@LRN", If(lrn Is DBNull.Value, "", lrn))
            checkCom.Parameters.AddWithValue("@EmployeeNo", If(employeeNo Is DBNull.Value, "", employeeNo))
            checkCom.Parameters.AddWithValue("@ContactNumber", contactNumber)

            Dim count As Integer = Convert.ToInt32(checkCom.ExecuteScalar())

            If count > 0 Then
                MsgBox("LRN, Employee Number, or Contact Number already exists. Please use a different one.", vbExclamation, "Duplication Not Allowed")
                con.Close()
                Exit Sub
            End If


            Dim com As New MySqlCommand("INSERT INTO `borrower_tbl`(`Borrower`, `FirstName`, `LastName`, `MiddleName`, `LRN`, `EmployeeNo`, `ContactNumber`, `Department`, `Grade`, `Section`, `Strand`) VALUES (@Borrower, @FirstName, @LastName, @MiddleName, @LRN, @EmployeeNo, @ContactNumber, @Department, @Grade, @Section, @Strand)", con)

            com.Parameters.AddWithValue("@Borrower", borrowerType)
            com.Parameters.AddWithValue("@FirstName", firstName)
            com.Parameters.AddWithValue("@LastName", lastName)
            com.Parameters.AddWithValue("@MiddleName", middleName)
            com.Parameters.AddWithValue("@LRN", lrn)
            com.Parameters.AddWithValue("@EmployeeNo", employeeNo)
            com.Parameters.AddWithValue("@ContactNumber", contactNumber)
            com.Parameters.AddWithValue("@Department", cbdepartment.Text.Trim())
            com.Parameters.AddWithValue("@Grade", cbgrade.Text.Trim())
            com.Parameters.AddWithValue("@Section", cbsection.Text.Trim())
            com.Parameters.AddWithValue("@Strand", cbstrand.Text.Trim())

            com.ExecuteNonQuery()

            MsgBox("Borrower added successfully!", vbInformation)
            Borrower_Load(sender, e)
            ClearFields()

            Dim registeredForm As RegisteredBrwr = Application.OpenForms.OfType(Of RegisteredBrwr)().FirstOrDefault()
            If registeredForm IsNot Nothing Then
                registeredForm.ludeyngborrower()
            End If

            cbstrand.Visible = True
            cbstrand.Location = New Point(942, 285)
            lblstrand.Visible = True
            lblstrand.Location = New Point(942, 266)

        Catch ex As Exception
            MessageBox.Show("Error adding borrower: " & ex.Message)
        Finally
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
        End Try
    End Sub


    Private Sub btnedit_Click(sender As Object, e As EventArgs) Handles btnedit.Click

        If DataGridView1.SelectedRows.Count = 0 Then
            MessageBox.Show("Please select a record to edit.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Dim ID As Integer = Convert.ToInt32(DataGridView1.SelectedRows(0).Cells("ID").Value)
        Dim con As New MySqlConnection(connectionString)
        Dim borrowerType As String = ""
        Dim middleName As String = txtmname.Text.Trim()
        Dim lrn As Object = DBNull.Value
        Dim employeeNo As Object = DBNull.Value
        Dim contactNumber As String = txtcontactnumber.Text.Trim()
        Dim firstName As String = txtfname.Text.Trim()
        Dim lastName As String = txtlname.Text.Trim()

        If rbstudent.Checked Then
            borrowerType = "Student"
        ElseIf rbteacher.Checked Then
            borrowerType = "Teacher"
        End If

        If rbnone.Checked Then
            middleName = "N/A"
        End If


        If String.IsNullOrWhiteSpace(firstName) OrElse String.IsNullOrWhiteSpace(lastName) OrElse String.IsNullOrWhiteSpace(contactNumber) Then
            MsgBox("Please fill in all required fields.", vbExclamation, "Missing Information")
            Exit Sub
        End If

        If borrowerType = "Student" Then
            If String.IsNullOrWhiteSpace(txtlrn.Text) Then
                MsgBox("Please enter the student's LRN.", vbExclamation, "Missing Information")
                Exit Sub
            End If
            lrn = txtlrn.Text.Trim()
        ElseIf borrowerType = "Teacher" Then
            If String.IsNullOrWhiteSpace(txtemployeeno.Text) Then
                MsgBox("Please enter the teacher's Employee Number.", vbExclamation, "Missing Information")
                Exit Sub
            End If
            employeeNo = txtemployeeno.Text.Trim()
        End If

        Try
            con.Open()


            Dim coms As New MySqlCommand("SELECT COUNT(*) FROM `borrower_tbl` WHERE ((`LRN` = @LRN AND `LRN` IS NOT NULL) OR (`EmployeeNo` = @EmployeeNo AND `EmployeeNo` IS NOT NULL) OR `ContactNumber` = @ContactNumber) AND `ID` <> @ID", con)
            coms.Parameters.AddWithValue("@LRN", If(lrn Is DBNull.Value, "", lrn))
            coms.Parameters.AddWithValue("@EmployeeNo", If(employeeNo Is DBNull.Value, "", employeeNo))
            coms.Parameters.AddWithValue("@ContactNumber", contactNumber)
            coms.Parameters.AddWithValue("@ID", Id)

            Dim count As Integer = Convert.ToInt32(coms.ExecuteScalar())

            If count > 0 Then
                MsgBox("LRN, Employee Number, or Contact Number already exists. Please use a different one.", vbExclamation, "Duplication Not Allowed")
                con.Close()
                Exit Sub
            End If

            Dim com As New MySqlCommand("UPDATE `borrower_tbl` SET `Borrower`=@Borrower, `FirstName`=@FirstName, `LastName`=@LastName, `MiddleName`=@MiddleName, `LRN`=@LRN, `EmployeeNo`=@EmployeeNo, `ContactNumber`=@ContactNumber, `Department`=@Department, `Grade`=@Grade, `Section`=@Section, `Strand`=@Strand WHERE `ID`=@ID", con)

            com.Parameters.AddWithValue("@Borrower", borrowerType)
            com.Parameters.AddWithValue("@FirstName", firstName)
            com.Parameters.AddWithValue("@LastName", lastName)
            com.Parameters.AddWithValue("@MiddleName", middleName)
            com.Parameters.AddWithValue("@LRN", lrn)
            com.Parameters.AddWithValue("@EmployeeNo", employeeNo)
            com.Parameters.AddWithValue("@ContactNumber", contactNumber)
            com.Parameters.AddWithValue("@Department", cbdepartment.Text.Trim())
            com.Parameters.AddWithValue("@Grade", cbgrade.Text.Trim())
            com.Parameters.AddWithValue("@Section", cbsection.Text.Trim())
            com.Parameters.AddWithValue("@Strand", cbstrand.Text.Trim())
            com.Parameters.AddWithValue("@ID", ID)

            com.ExecuteNonQuery()

            MsgBox("Borrower updated successfully!", vbInformation)
            Borrower_Load(sender, e)
            ClearFields()

            Dim registeredForm As RegisteredBrwr = Application.OpenForms.OfType(Of RegisteredBrwr)().FirstOrDefault()
            If registeredForm IsNot Nothing Then
                registeredForm.ludeyngborrower()
            End If


            cbstrand.Visible = True
            cbstrand.Location = New Point(942, 285)
            lblstrand.Visible = True
            lblstrand.Location = New Point(942, 266)

        Catch ex As Exception
            MessageBox.Show("Error updating borrower: " & ex.Message)
        Finally
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
        End Try
    End Sub


    Private Sub btndelete_Click(sender As Object, e As EventArgs) Handles btndelete.Click


        If DataGridView1.SelectedRows.Count > 0 Then

            Dim dialogResult As DialogResult = MessageBox.Show("Are you sure you want to delete this borrower?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)

            If dialogResult = DialogResult.Yes Then

                Dim con As New MySqlConnection(connectionString)
                Dim selectedRow As DataGridViewRow = DataGridView1.SelectedRows(0)
                Dim ID As Integer = CInt(selectedRow.Cells("ID").Value)

                Try
                    con.Open()
                    Dim delete As New MySqlCommand("DELETE FROM `borrower_tbl` WHERE `ID` = @id", con)
                    delete.Parameters.AddWithValue("@id", ID)
                    delete.ExecuteNonQuery()


                    MsgBox("Borrower deleted successfully.", vbInformation)
                    Borrower_Load(sender, e)
                    ClearFields()

                    Dim registeredForm As RegisteredBrwr = Application.OpenForms.OfType(Of RegisteredBrwr)().FirstOrDefault()
                    If registeredForm IsNot Nothing Then
                        registeredForm.ludeyngborrower()
                    End If

                    cbstrand.Visible = True
                    cbstrand.Location = New Point(942, 285)


                    lblstrand.Visible = True
                    lblstrand.Location = New Point(942, 266)

                    Dim count As New MySqlCommand("SELECT COUNT(*) FROM `borrower_tbl`", con)
                    Dim rowCount As Long = CLng(count.ExecuteScalar())

                    If rowCount = 0 Then

                        Dim reset As New MySqlCommand("ALTER TABLE `borrower_tbl` AUTO_INCREMENT = 1", con)
                        reset.ExecuteNonQuery()

                    End If

                Catch ex As Exception
                    MsgBox(ex.Message, vbCritical)
                End Try
            End If

        End If

    End Sub


    Private Sub btnclear_Click(sender As Object, e As EventArgs) Handles btnclear.Click

        ClearFields

        cbstrand.Visible = True
        cbstrand.Location = New Point(942, 285)


        lblstrand.Visible = True
        lblstrand.Location = New Point(942, 266)

    End Sub


    Private Sub DataGridView1_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellClick



        If e.RowIndex >= 0 Then

            Dim row As DataGridViewRow = DataGridView1.Rows(e.RowIndex)
            Dim borrowerType As String = row.Cells("Borrower").Value.ToString()



            If borrowerType = "Student" Then

                rbstudent.Checked = True

            ElseIf borrowerType = "Teacher" Then

                rbteacher.Checked = True

                txtemployeeno.Text = If(IsDBNull(row.Cells("EmployeeNo").Value), String.Empty, row.Cells("EmployeeNo").Value.ToString())
                txtlrn.Text = ""

                cbstrand.Visible = True
                cbstrand.Location = New Point(942, 285)


                lblstrand.Visible = True
                lblstrand.Location = New Point(942, 266)


            End If


            txtfname.Text = row.Cells("FirstName").Value.ToString()

            Dim middleName As String = row.Cells("MiddleName").Value.ToString()

            If middleName.Trim().ToUpper() = "N/A" Then

                rbnone.Checked = True
                txtmname.Text = ""

            Else

                rbnone.Checked = False
                txtmname.Text = middleName

            End If


            txtlname.Text = row.Cells("LastName").Value.ToString()
            txtlrn.Text = row.Cells("LRN").Value.ToString()
            txtcontactnumber.Text = row.Cells("ContactNumber").Value.ToString()
            cbdepartment.Text = row.Cells("Department").Value.ToString()
            cbgrade.Text = row.Cells("Grade").Value.ToString()
            cbsection.Text = row.Cells("Section").Value.ToString()
            cbstrand.Text = row.Cells("Strand").Value.ToString()


        End If

    End Sub

    Private Sub cbdepartment_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbdepartment.SelectedIndexChanged


        cbgrade.Enabled = False
        cbgrade.SelectedIndex = -1
        cbsection.Enabled = False
        cbsection.SelectedIndex = -1
        cbstrand.Enabled = False
        cbstrand.SelectedIndex = -1


        cbgrade.Visible = True
        lblgrade.Visible = True
        cbsection.Visible = True
        lblsection.Visible = True
        cbstrand.Visible = True
        lblstrand.Visible = True

        If cbdepartment.SelectedIndex <> -1 Then
            Dim selectedDept As String = cbdepartment.GetItemText(cbdepartment.SelectedItem)


            If rbteacher.Checked Then
                cbgrade.Enabled = False
                cbgrade.Visible = False
                lblgrade.Visible = False
                cbsection.Enabled = False
                cbsection.Visible = False
                lblsection.Visible = False
                cbstrand.Enabled = False
                cbstrand.Visible = False
                lblstrand.Visible = False
                Return
            End If


            Select Case selectedDept
                Case "Junior High School"
                    Dim con As New MySqlConnection(connectionString)
                    Dim dt As New DataTable
                    Try
                        con.Open()
                        Dim adap As New MySqlDataAdapter("SELECT ID, Grade FROM `grade_tbl` WHERE Grade IN ('7', '8', '9', '10')", con)
                        adap.Fill(dt)
                        cbgrade.DataSource = dt
                        cbgrade.DisplayMember = "Grade"
                        cbgrade.ValueMember = "ID"
                        cbgrade.SelectedIndex = -1

                        cbgrade.Enabled = True
                        cbsection.Visible = True
                        lblsection.Visible = True
                        cbstrand.Visible = False
                        lblstrand.Visible = False
                    Catch ex As Exception
                        MessageBox.Show("Error filtering JHS grades: " & ex.Message)
                    End Try

                Case "Senior High School"
                    Dim con As New MySqlConnection(connectionString)
                    Dim dt As New DataTable
                    Try
                        con.Open()
                        Dim adap As New MySqlDataAdapter("SELECT ID, Grade FROM `grade_tbl` WHERE Grade IN ('11', '12')", con)
                        adap.Fill(dt)
                        cbgrade.DataSource = dt
                        cbgrade.DisplayMember = "Grade"
                        cbgrade.ValueMember = "ID"
                        cbgrade.SelectedIndex = -1

                        cbgrade.Enabled = True
                        cbsection.Visible = False
                        lblsection.Visible = False
                        cbstrand.Visible = True
                        lblstrand.Visible = True
                        cbstrand.Location = New Point(942, 216)
                        lblstrand.Location = New Point(942, 197)
                    Catch ex As Exception
                        MessageBox.Show("Error filtering SHS grades: " & ex.Message)
                    End Try

                Case "Elementary"
                    Dim con As New MySqlConnection(connectionString)
                    Dim dt As New DataTable
                    Try
                        con.Open()
                        Dim adap As New MySqlDataAdapter("SELECT ID, Grade FROM `grade_tbl` WHERE Grade IN ('1', '2', '3', '4', '5', '6')", con)
                        adap.Fill(dt)
                        cbgrade.DataSource = dt
                        cbgrade.DisplayMember = "Grade"
                        cbgrade.ValueMember = "ID"
                        cbgrade.SelectedIndex = -1

                        cbgrade.Enabled = True
                        cbsection.Visible = True
                        lblsection.Visible = True
                        cbstrand.Visible = False
                        lblstrand.Visible = False
                    Catch ex As Exception
                        MessageBox.Show("Error filtering Elementary grades: " & ex.Message)
                    End Try
            End Select
        End If
    End Sub

    Private Sub cbsection_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbsection.SelectedIndexChanged

        If cbsection.SelectedIndex <> -1 AndAlso cbgrade.SelectedIndex <> -1 Then
            cbstrand.Enabled = True
        Else
            cbstrand.Enabled = False
        End If
    End Sub

    Private Sub cbgrade_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbgrade.SelectedIndexChanged

        cbsection.Enabled = False
        cbsection.SelectedIndex = -1
        cbstrand.Enabled = False
        cbstrand.SelectedIndex = -1

        If cbgrade.SelectedIndex <> -1 Then
            Dim selectedGrade As String = cbgrade.GetItemText(cbgrade.SelectedItem)
            Dim selectedDept As String = cbdepartment.GetItemText(cbdepartment.SelectedItem)

            Select Case selectedDept
                Case "Junior High School"
                    cbsection.Enabled = True
                    Dim con As New MySqlConnection(connectionString)
                    Dim dt As New DataTable
                    Try
                        con.Open()
                        Dim com As New MySqlCommand("SELECT ID, Section FROM `section_tbl` WHERE Department = 'Junior High School' AND GradeLevel = @grade", con)
                        com.Parameters.AddWithValue("@grade", selectedGrade)
                        Dim adap As New MySqlDataAdapter(com)
                        adap.Fill(dt)
                        cbsection.DataSource = dt
                        cbsection.DisplayMember = "Section"
                        cbsection.ValueMember = "ID"
                        cbsection.SelectedIndex = -1
                        cbstrand.Enabled = False
                    Catch ex As Exception
                        MessageBox.Show("Error loading sections: " & ex.Message)
                    Finally
                        con.Close()
                    End Try

                Case "Senior High School"
                    cbstrand.Enabled = True
                    Dim con As New MySqlConnection(connectionString)
                    Dim dt As New DataTable
                    Try
                        con.Open()
                        Dim com As New MySqlCommand("SELECT ID, Strand FROM `section_tbl` WHERE Department = 'Senior High School' AND GradeLevel = @grade", con)
                        com.Parameters.AddWithValue("@grade", selectedGrade)
                        Dim adap As New MySqlDataAdapter(com)
                        adap.Fill(dt)
                        cbstrand.DataSource = dt
                        cbstrand.DisplayMember = "Strand"
                        cbstrand.ValueMember = "ID"
                        cbstrand.SelectedIndex = -1
                        cbsection.Enabled = False
                    Catch ex As Exception
                        MessageBox.Show("Error loading strands: " & ex.Message)
                    Finally
                        con.Close()
                    End Try

                Case "Elementary"
                    cbsection.Enabled = True
                    Dim con As New MySqlConnection(connectionString)
                    Dim dt As New DataTable
                    Try
                        con.Open()
                        Dim com As New MySqlCommand("SELECT ID, Section FROM `section_tbl` WHERE Department = 'Elementary' AND GradeLevel = @grade", con)
                        com.Parameters.AddWithValue("@grade", selectedGrade)
                        Dim adap As New MySqlDataAdapter(com)
                        adap.Fill(dt)
                        cbsection.DataSource = dt
                        cbsection.DisplayMember = "Section"
                        cbsection.ValueMember = "ID"
                        cbsection.SelectedIndex = -1
                        cbstrand.Enabled = False
                    Catch ex As Exception
                        MessageBox.Show("Error loading sections for Elementary: " & ex.Message)
                    Finally
                        con.Close()
                    End Try
            End Select
        End If
    End Sub

    Private Sub rbstudent_CheckedChanged(sender As Object, e As EventArgs) Handles rbstudent.CheckedChanged

        If rbstudent.Checked Then


            txtlrn.Visible = True
            txtlrn.Enabled = True
            txtemployeeno.Visible = False


            lblborrowertype.Text = "LRN:"


            cbdepartment.Enabled = True
            cbdepartment.DataSource = Nothing
            cbdepts()

            cbgrade.Enabled = False
            cbgrade.Visible = True
            lblgrade.Visible = True

            cbsection.Enabled = False
            cbsection.Visible = True
            lblsection.Visible = True

            cbstrand.Enabled = False
            cbstrand.Visible = True
            lblstrand.Visible = True

            cbstrand.Location = New Point(942, 285)
            lblstrand.Location = New Point(942, 266)


            txtfname.Enabled = True
            txtlname.Enabled = True
            txtmname.Enabled = True
            txtcontactnumber.Enabled = True
            rbnone.Enabled = True
            rbnone.Checked = False

        End If

    End Sub

    Private Sub rbteacher_CheckedChanged(sender As Object, e As EventArgs) Handles rbteacher.CheckedChanged

        If rbteacher.Checked Then


            txtlrn.Visible = False
            txtlrn.Enabled = False
            txtemployeeno.Visible = True


            lblborrowertype.Text = "Employee no.:"


            cbdepartment.Enabled = True
            cbdepartment.DataSource = Nothing
            cbdepts()


            cbgrade.Enabled = False
            cbgrade.Visible = False
            lblgrade.Visible = False

            cbsection.Enabled = False
            cbsection.Visible = False
            lblsection.Visible = False

            cbstrand.Enabled = False
            cbstrand.Visible = False
            lblstrand.Visible = False


            txtfname.Enabled = True
            txtlname.Enabled = True
            txtmname.Enabled = True
            txtemployeeno.Enabled = True
            txtcontactnumber.Enabled = True
            rbnone.Enabled = True
            rbnone.Checked = False

        End If

    End Sub

    Private Sub rbnone_CheckedChanged(sender As Object, e As EventArgs) Handles rbnone.CheckedChanged

        If rbnone.Checked Then
            txtmname.Enabled = False
            txtmname.Text = ""
        Else
            txtmname.Enabled = True
        End If

    End Sub

    Private Sub ClearFields()

        txtemployeeno.Text = ""
        txtfname.Text = ""
        txtmname.Text = ""
        txtlname.Text = ""
        txtlrn.Text = ""
        txtcontactnumber.Text = ""
        DataGridView1.ClearSelection()

        txtlrn.Visible = True
        txtemployeeno.Visible = False

        cbdepartment.Visible = True
        cbgrade.Visible = True
        cbsection.Visible = True
        lblsection.Visible = True
        cbstrand.Visible = True
        lblstrand.Visible = True

        cbdepartment.Enabled = False
        cbgrade.Enabled = False
        cbsection.Enabled = False
        cbstrand.Enabled = False


        cbdepartment.DataSource = Nothing
        cbgrade.DataSource = Nothing
        cbsection.DataSource = Nothing
        cbstrand.DataSource = Nothing

        cbdepts()
        cbsecs()
        cbgradee()
        cbstrandd()


        rbstudent.Checked = False
        rbteacher.Checked = False
        rbnone.Checked = False
        txtmname.Enabled = True
        lblborrowertype.Text = "LRN:"

        txtfname.Enabled = False
        txtlname.Enabled = False
        txtmname.Enabled = False
        txtlrn.Enabled = False
        txtcontactnumber.Enabled = False
        rbnone.Enabled = False

    End Sub

    Private Sub txtfname_KeyDown(sender As Object, e As KeyEventArgs) Handles txtfname.KeyDown

        If e.Control AndAlso (e.KeyCode = Keys.V Or e.KeyCode = Keys.C Or e.KeyCode = Keys.X) Then
            e.SuppressKeyPress = True
        End If

    End Sub

    Private Sub txtfname_KeyPress(sender As Object, e As KeyPressEventArgs) Handles txtfname.KeyPress

        If Not Char.IsLetter(e.KeyChar) AndAlso Not Char.IsControl(e.KeyChar) AndAlso Not Char.IsWhiteSpace(e.KeyChar) Then
            e.Handled = True
        End If

    End Sub

    Private Sub txtlname_KeyDown(sender As Object, e As KeyEventArgs) Handles txtlname.KeyDown

        If e.Control AndAlso (e.KeyCode = Keys.V Or e.KeyCode = Keys.C Or e.KeyCode = Keys.X) Then
            e.SuppressKeyPress = True
        End If

    End Sub

    Private Sub txtlname_KeyPress(sender As Object, e As KeyPressEventArgs) Handles txtlname.KeyPress

        If Not Char.IsLetter(e.KeyChar) AndAlso Not Char.IsControl(e.KeyChar) AndAlso Not Char.IsWhiteSpace(e.KeyChar) Then
            e.Handled = True
        End If

    End Sub

    Private Sub txtmname_KeyDown(sender As Object, e As KeyEventArgs) Handles txtmname.KeyDown

        If e.Control AndAlso (e.KeyCode = Keys.V Or e.KeyCode = Keys.C Or e.KeyCode = Keys.X) Then
            e.SuppressKeyPress = True
        End If

    End Sub

    Private Sub txtmname_KeyPress(sender As Object, e As KeyPressEventArgs) Handles txtmname.KeyPress

        If Not Char.IsLetter(e.KeyChar) AndAlso Not Char.IsControl(e.KeyChar) AndAlso Not Char.IsWhiteSpace(e.KeyChar) Then
            e.Handled = True
        End If

    End Sub

    Private Sub txtlrn_KeyDown(sender As Object, e As KeyEventArgs) Handles txtlrn.KeyDown

        If e.Control AndAlso (e.KeyCode = Keys.V Or e.KeyCode = Keys.C Or e.KeyCode = Keys.X) Then
            e.SuppressKeyPress = True
        End If

    End Sub

    Private Sub txtlrn_KeyPress(sender As Object, e As KeyPressEventArgs) Handles txtlrn.KeyPress

        If Not Char.IsDigit(e.KeyChar) And Not Char.IsControl(e.KeyChar) Then
            e.Handled = True
        End If

    End Sub

    Private Sub txtcontactnumber_KeyDown(sender As Object, e As KeyEventArgs) Handles txtcontactnumber.KeyDown

        If e.Control AndAlso (e.KeyCode = Keys.V Or e.KeyCode = Keys.C Or e.KeyCode = Keys.X) Then
            e.SuppressKeyPress = True
        End If

        If e.KeyCode = Keys.Back Then
            RemoveHandler txtcontactnumber.TextChanged, AddressOf txtcontactnumber_TextChanged
        End If

    End Sub

    Private Sub txtcontactnumber_KeyPress(sender As Object, e As KeyPressEventArgs) Handles txtcontactnumber.KeyPress


        If Not Char.IsDigit(e.KeyChar) And Not Char.IsControl(e.KeyChar) Then
            e.Handled = True
        End If

    End Sub

    Private Sub txtcontactnumber_TextChanged(sender As Object, e As EventArgs) Handles txtcontactnumber.TextChanged

        Dim oreyjeynal = txtcontactnumber.Text

        If String.IsNullOrEmpty(oreyjeynal) Then
            Return
        End If

        If oreyjeynal.StartsWith("09") Then

        Else

            If oreyjeynal.Length > 0 Then
                txtcontactnumber.Clear
                txtcontactnumber.Text = "09"
                txtcontactnumber.SelectionStart = 2
            End If
        End If

    End Sub

    Private Sub txtsearch_KeyDown(sender As Object, e As KeyEventArgs) Handles txtsearch.KeyDown

        If e.Control AndAlso (e.KeyCode = Keys.V Or e.KeyCode = Keys.C Or e.KeyCode = Keys.X) Then
            e.SuppressKeyPress = True
        End If

    End Sub

    Private Sub Borrower_FormClosed(sender As Object, e As FormClosedEventArgs) Handles Me.FormClosed
        ClearFields()
    End Sub

    Private Sub txtcontactnumber_KeyUp(sender As Object, e As KeyEventArgs) Handles txtcontactnumber.KeyUp


        If e.KeyCode = Keys.Back Then
            AddHandler txtcontactnumber.TextChanged, AddressOf txtcontactnumber_TextChanged

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
End Class