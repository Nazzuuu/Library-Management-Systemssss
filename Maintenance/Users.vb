Imports System.IO
Imports System.Security.Cryptography
Imports System.Text
Imports MySql.Data.MySqlClient
Imports System.Data
Imports System.Text.RegularExpressions

Public Class Users

    Private isBackspacing As Boolean = False

    Public Sub LoadStaffData()
        Dim con As New MySqlConnection(connectionString)
        Dim com As String = "SELECT * FROM `user_staff_tbl`"
        Dim adapp As New MySqlDataAdapter(com, con)
        Dim dt As New DataSet

        Try
            con.Open()
            adapp.Fill(dt, "INFO")
            DataGridView1.DataSource = dt.Tables("INFO")
            con.Close()

            DataGridView1.Columns("ID").Visible = False
            DataGridView1.Columns("Password").Visible = False

            DataGridView1.EnableHeadersVisualStyles = False
            DataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(207, 58, 109)
            DataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White

            DataGridView1.ClearSelection()
            DataGridView1.CurrentCell = Nothing
        Catch ex As Exception
            MsgBox("Error loading staff data: " & ex.Message, vbCritical, "Database Error")
        Finally
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
        End Try

        LoadUserData()
    End Sub

    Private Sub Users_Staffs_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        txtpassword.PasswordChar = "•"

        PictureBox2.Image = Image.FromFile(Application.StartupPath & "\Resources\pikit.png")

        LoadStaffData()
        LoadUserData()

        AddHandler DataGridView1.CellFormatting, AddressOf Me.DataGridView1_CellFormatting

        lblpassword.Visible = False


    End Sub

    Private Sub User_staff(sender As Object, e As EventArgs) Handles MyBase.Shown

        DataGridView1.ClearSelection()

    End Sub

    Public Sub LoadUserData()



        Dim con As New MySqlConnection(GlobalVarsModule.connectionString)
        Dim SQLQuery As String = "SELECT ID, FirstName, LastName, MiddleInitial, Username, Password, Email, Address, ContactNumber, Gender, Role FROM user_staff_tbl"


        If GlobalVarsModule.GlobalRole = "Staff" OrElse GlobalVarsModule.GlobalRole = "Assistant Librarian" Then
            SQLQuery &= " WHERE ID = @EmployeeID"
            uidisplay()
        Else
            uireset()
        End If

        Using cmd As New MySqlCommand(SQLQuery, con)
            If GlobalVarsModule.GlobalRole = "Staff" OrElse GlobalVarsModule.GlobalRole = "Assistant Librarian" Then
                cmd.Parameters.AddWithValue("@EmployeeID", GlobalVarsModule.CurrentEmployeeID)
            End If

            Try
                con.Open()


                Dim da As New MySqlDataAdapter(cmd)
                Dim dt As New DataTable()
                da.Fill(dt)

                DataGridView1.DataSource = dt


            Catch ex As Exception
                MessageBox.Show("Error loading user data: " & ex.Message)
            Finally
                If con.State = ConnectionState.Open Then con.Close()
            End Try
        End Using

    End Sub

    Public Sub uidisplay()

        btnedit.Location = New Point(30, 227)
        btnclear.Location = New Point(158, 227)
        btnadd.Visible = False
        btndelete.Visible = False

        rbassistant.Visible = False
        rbstaff.Visible = False

        lblrolesu.Visible = False

    End Sub

    Public Sub uireset()

        btnclear.Location = New Point(417, 227)
        btnedit.Location = New Point(158, 227)

        btnadd.Visible = True
        btndelete.Visible = True

        rbassistant.Visible = True
        rbstaff.Visible = True

        lblrolesu.Visible = True

    End Sub

    Private Sub DataGridView1_CellFormatting(sender As Object, e As DataGridViewCellFormattingEventArgs)

        If DataGridView1.Columns(e.ColumnIndex).Name = "Password" AndAlso e.Value IsNot Nothing Then
            Dim passText As String = e.Value.ToString()
            Dim formattedString As String = New String("*"c, passText.Length)
            e.Value = formattedString
            e.FormattingApplied = True
        End If
    End Sub


    Private Sub btnadd_Click(sender As Object, e As EventArgs) Handles btnadd.Click

        Dim firstName As String = txtfname.Text.Trim()
        Dim lastName As String = txtlname.Text.Trim()
        Dim user As String = txtusername.Text.Trim()
        Dim pass As String = txtpassword.Text.Trim()
        Dim email As String = txtemail.Text.Trim()
        Dim contact As String = txtcontactnumber.Text.Trim()
        Dim address As String = txtaddress.Text.Trim()
        Dim newID As Integer = 0

        If String.IsNullOrWhiteSpace(firstName) OrElse String.IsNullOrWhiteSpace(lastName) OrElse String.IsNullOrWhiteSpace(user) OrElse String.IsNullOrWhiteSpace(pass) OrElse String.IsNullOrWhiteSpace(email) OrElse String.IsNullOrWhiteSpace(address) Then
            MsgBox("Please fill in all the required fields (including Address).", vbExclamation, "Missing Information")
            Exit Sub
        End If

        Dim emailRegex As New System.Text.RegularExpressions.Regex("^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.com$")
        If Not emailRegex.IsMatch(email) Then
            MsgBox("Invalid email format. Please enter a valid email address (e.g., example@gmail.com).", vbExclamation, "Invalid Email")
            Exit Sub
        End If

        Dim gender As String = ""
        If rbmale.Checked Then
            gender = "Male"
        ElseIf rbfemale.Checked Then
            gender = "Female"
        Else
            MsgBox("Please select a gender.", vbExclamation, "Missing Information")
            Exit Sub
        End If

        Dim role As String = ""

        If rbassistant.Checked Then
            role = "Assistant Librarian"
        ElseIf rbstaff.Checked Then
            role = "Staff"
        Else
            MsgBox("Please select a role.", vbExclamation, "Missing Information")
            Exit Sub
        End If

        Dim middleName As String
        If CheckBox2.Checked Then
            middleName = "N/A"
        Else
            middleName = txtmname.Text.Trim()
        End If


        Dim con As New MySqlConnection(GlobalVarsModule.connectionString)
        Try
            con.Open()

            Dim Coms As New MySqlCommand("SELECT COUNT(*) FROM `user_staff_tbl` WHERE `Username` = @username OR `Email` = @email", con)
            Coms.Parameters.AddWithValue("@username", user)
            Coms.Parameters.AddWithValue("@email", email)
            Dim count As Integer = Convert.ToInt32(Coms.ExecuteScalar())

            If count > 0 Then
                MsgBox("The username or email already exists. Please use a different one.", vbExclamation, "Duplication Not Allowed")
                Exit Sub
            End If

            Dim com As New MySqlCommand("INSERT INTO `user_staff_tbl`(`FirstName`, `LastName`, `MiddleInitial`, `Username`, `Password`, `Email`, `ContactNumber`, `Address`, `Gender`, `Role`) VALUES (@firstName, @lastName, @middleName, @username, @password, @email, @contact, @address, @gender, @role); SELECT LAST_INSERT_ID();", con)
            com.Parameters.AddWithValue("@firstName", firstName)
            com.Parameters.AddWithValue("@lastName", lastName)
            com.Parameters.AddWithValue("@middleName", middleName)
            com.Parameters.AddWithValue("@username", user)
            com.Parameters.AddWithValue("@password", pass)
            com.Parameters.AddWithValue("@email", email)
            com.Parameters.AddWithValue("@contact", contact)
            com.Parameters.AddWithValue("@address", address)
            com.Parameters.AddWithValue("@gender", gender)
            com.Parameters.AddWithValue("@role", role)
            newID = Convert.ToInt32(com.ExecuteScalar())

            GlobalVarsModule.LogAudit(
            actionType:="ADD",
            formName:="USER STAFF FORM",
            description:=$"Added new staff: {lastName}, {firstName} ({role})",
            recordID:=newID.ToString()
        )

            For Each form In Application.OpenForms
                If TypeOf form Is AuditTrail Then
                    DirectCast(form, AuditTrail).refreshaudit()
                End If
            Next

            MsgBox("User added successfully!", vbInformation)
            LoadStaffData()
            clearfields()

        Catch ex As Exception
            MsgBox(ex.Message, vbCritical, "Database Error")
        Finally
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
        End Try
    End Sub

    Private Sub btnedit_Click(sender As Object, e As EventArgs) Handles btnedit.Click

        If DataGridView1.CurrentRow Is Nothing Then
            MsgBox("Please select a row before to edit.", vbExclamation)
            Exit Sub
        End If

        Dim selectedRow = DataGridView1.CurrentRow
        Dim ID As Integer = CInt(selectedRow.Cells("ID").Value)


        Dim oldFirstName As String = selectedRow.Cells("FirstName").Value.ToString.Trim()
        Dim oldLastName As String = selectedRow.Cells("LastName").Value.ToString.Trim()
        Dim oldMiddleName As String = selectedRow.Cells("MiddleInitial").Value.ToString.Trim()
        Dim oldUsername As String = selectedRow.Cells("Username").Value.ToString.Trim()
        Dim oldPassword As String = selectedRow.Cells("Password").Value.ToString.Trim()
        Dim oldEmail As String = selectedRow.Cells("Email").Value.ToString.Trim()
        Dim oldContact As String = selectedRow.Cells("ContactNumber").Value.ToString.Trim()
        Dim oldAddress As String = selectedRow.Cells("Address").Value.ToString.Trim()
        Dim oldGender As String = selectedRow.Cells("Gender").Value.ToString.Trim()
        Dim oldRole As String = selectedRow.Cells("Role").Value.ToString.Trim()


        Dim firstName As String = txtfname.Text.Trim()
        Dim lastName As String = txtlname.Text.Trim()
        Dim user As String = txtusername.Text.Trim()
        Dim pass As String = txtpassword.Text.Trim()
        Dim email As String = txtemail.Text.Trim()
        Dim contact As String = txtcontactnumber.Text.Trim()
        Dim address As String = txtaddress.Text.Trim()

        Dim gender As String = ""

        If String.IsNullOrWhiteSpace(firstName) OrElse String.IsNullOrWhiteSpace(lastName) OrElse String.IsNullOrWhiteSpace(user) OrElse String.IsNullOrWhiteSpace(pass) OrElse String.IsNullOrWhiteSpace(email) OrElse String.IsNullOrWhiteSpace(address) Then
            MsgBox("Please fill in all the required fields.", vbExclamation, "Missing Information")
            Exit Sub
        End If

        Dim emailRegex As New System.Text.RegularExpressions.Regex("^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.com$")
        If Not emailRegex.IsMatch(email) Then
            MsgBox("Invalid email format. Please enter a valid email address (e.g., example@gmail.com).", vbExclamation, "Invalid Email")
            Exit Sub
        End If

        If rbmale.Checked Then
            gender = "Male"
        ElseIf rbfemale.Checked Then
            gender = "Female"
        Else
            MsgBox("Please select a gender.", vbExclamation, "Missing Information")
            Exit Sub
        End If

        Dim role As String = ""

        If rbassistant.Checked Then
            role = "Assistant Librarian"
        ElseIf rbstaff.Checked Then
            role = "Staff"
        Else
            MsgBox("Please select a role.", vbExclamation, "Missing Information")
            Exit Sub
        End If

        Dim middleName As String
        If CheckBox2.Checked Then
            middleName = "N/A"
        Else
            middleName = txtmname.Text.Trim()
        End If

        Dim con As New MySqlConnection(GlobalVarsModule.connectionString)

        Try
            con.Open()

            Dim Com As New MySqlCommand("SELECT COUNT(*) FROM `user_staff_tbl` WHERE (`Username` = @username OR `Email` = @email) AND `ID` <> @id", con)
            Com.Parameters.AddWithValue("@username", user)
            Com.Parameters.AddWithValue("@email", email)
            Com.Parameters.AddWithValue("@id", ID)
            Dim count As Integer = Convert.ToInt32(Com.ExecuteScalar())

            If count > 0 Then
                MsgBox("The username or email already exists. Please use a different one.", vbExclamation, "Duplication Not Allowed")
                Exit Sub
            End If

            Dim Coms As New MySqlCommand("UPDATE `user_staff_tbl` SET `FirstName` = @firstName, `LastName` = @lastName, `MiddleInitial` = @middleName, `Username` = @username, `Password` = @password, `Email` = @email, `ContactNumber` = @contact, `Address` = @address, `Gender` = @gender, `Role` = @role WHERE `ID` = @id", con)
            Coms.Parameters.AddWithValue("@firstName", firstName)
            Coms.Parameters.AddWithValue("@lastName", lastName)
            Coms.Parameters.AddWithValue("@middleName", middleName)
            Coms.Parameters.AddWithValue("@username", user)
            Coms.Parameters.AddWithValue("@password", pass)
            Coms.Parameters.AddWithValue("@email", email)
            Coms.Parameters.AddWithValue("@contact", contact)
            Coms.Parameters.AddWithValue("@address", address)
            Coms.Parameters.AddWithValue("@gender", gender)
            Coms.Parameters.AddWithValue("@role", role)
            Coms.Parameters.AddWithValue("@id", ID)
            Coms.ExecuteNonQuery()

            Dim oldValueString As String = $"{oldFirstName}|{oldLastName}|{oldMiddleName}|{oldUsername}|{oldPassword}|{oldEmail}|{oldContact}|{oldAddress}|{oldGender}|{oldRole}"
            Dim newValueString As String = $"{firstName}|{lastName}|{middleName}|{user}|{pass}|{email}|{contact}|{address}|{gender}|{role}"

            GlobalVarsModule.LogAudit(
            actionType:="UPDATE",
            formName:="USER STAFF FORM",
            description:=$"Updated staff ID {ID}: {lastName}, {firstName} ({role})",
            recordID:=ID.ToString(),
            oldValue:=oldValueString,
            newValue:=newValueString
        )

            For Each form In Application.OpenForms
                If TypeOf form Is AuditTrail Then
                    DirectCast(form, AuditTrail).refreshaudit()
                End If
            Next

            If MainForm.lblgmail.Text = DataGridView1.CurrentRow.Cells("Email").Value.ToString Then
                MainForm.lblgmail.Text = txtemail.Text.Trim
            End If

            MsgBox("User updated successfully!", vbInformation)
            LoadStaffData()
            clearfields()

        Catch ex As Exception
            MsgBox("Error updating staff member: " & ex.Message, vbCritical)
        Finally
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
        End Try
    End Sub

    Private Sub btndelete_Click(sender As Object, e As EventArgs) Handles btndelete.Click

        If DataGridView1.SelectedRows.Count > 0 Then

            Dim dialogResult As DialogResult = MessageBox.Show("Are you sure you want to delete this staff member?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)

            If dialogResult = DialogResult.Yes Then

                Dim con As New MySqlConnection(GlobalVarsModule.connectionString)
                Dim selectedRow As DataGridViewRow = DataGridView1.SelectedRows(0)
                Dim ID As Integer = CInt(selectedRow.Cells("ID").Value)
                Dim staffName As String = $"{selectedRow.Cells("LastName").Value}, {selectedRow.Cells("FirstName").Value} ({selectedRow.Cells("Role").Value})"

                Try
                    con.Open()
                    Dim delete As New MySqlCommand("DELETE FROM `user_staff_tbl` WHERE `ID` = @id", con)
                    delete.Parameters.AddWithValue("@id", ID)
                    delete.ExecuteNonQuery()

                    GlobalVarsModule.LogAudit(
                    actionType:="DELETE",
                    formName:="USER STAFF FORM",
                    description:=$"Deleted staff member: {staffName}",
                    recordID:=ID.ToString()
                )

                    For Each form In Application.OpenForms
                        If TypeOf form Is AuditTrail Then
                            DirectCast(form, AuditTrail).refreshaudit()
                        End If
                    Next

                    MsgBox("User deleted successfully.", vbInformation)
                    LoadStaffData()
                    clearfields()


                    Dim count As New MySqlCommand("SELECT COUNT(*) FROM `user_staff_tbl`", con)
                    Dim rowCount As Long = CLng(count.ExecuteScalar())

                    If rowCount = 0 Then

                        Dim reset As New MySqlCommand("ALTER TABLE `user_staff_tbl` AUTO_INCREMENT = 1", con)
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
            Dim row As DataGridViewRow = DataGridView1.Rows(e.RowIndex)

            txtfname.Text = If(IsDBNull(row.Cells("FirstName").Value), String.Empty, row.Cells("FirstName").Value.ToString())
            txtlname.Text = If(IsDBNull(row.Cells("LastName").Value), String.Empty, row.Cells("LastName").Value.ToString())
            txtusername.Text = If(IsDBNull(row.Cells("Username").Value), String.Empty, row.Cells("Username").Value.ToString())
            txtemail.Text = If(IsDBNull(row.Cells("Email").Value), String.Empty, row.Cells("Email").Value.ToString())
            txtcontactnumber.Text = If(IsDBNull(row.Cells("ContactNumber").Value), String.Empty, row.Cells("ContactNumber").Value.ToString())
            txtpassword.Text = If(IsDBNull(row.Cells("Password").Value), String.Empty, row.Cells("Password").Value.ToString())

            txtaddress.Text = If(IsDBNull(row.Cells("Address").Value), String.Empty, row.Cells("Address").Value.ToString())

            Dim gender As String = If(IsDBNull(row.Cells("Gender").Value), String.Empty, row.Cells("Gender").Value.ToString())
            rbmale.Checked = (gender = "Male")
            rbfemale.Checked = (gender = "Female")


            Dim role As String = If(IsDBNull(row.Cells("Role").Value), String.Empty, row.Cells("Role").Value.ToString())

            rbassistant.Checked = (role = "Assistant Librarian")
            rbstaff.Checked = (role = "Staff")

            Dim middleName As String = If(IsDBNull(row.Cells("MiddleInitial").Value), String.Empty, row.Cells("MiddleInitial").Value.ToString())

            If middleName = "N/A" Then
                CheckBox2.Checked = True
                txtmname.Text = ""
                txtmname.Enabled = False
            Else
                CheckBox2.Checked = False
                txtmname.Text = middleName
                txtmname.Enabled = True
            End If

            txtemail_TextChanged(txtemail, EventArgs.Empty)
            txtpass(txtpassword, EventArgs.Empty)

        End If
    End Sub


    Private Sub txtsearch_TextChanged(sender As Object, e As EventArgs) Handles txtsearch.TextChanged
        Dim dt As DataTable = DirectCast(DataGridView1.DataSource, DataTable)
        If dt IsNot Nothing Then
            If txtsearch.Text.Trim() <> "" Then
                Dim filter As String = String.Format("FirstName LIKE '*{0}*' OR LastName LIKE '*{0}*' OR ContactNumber LIKE '*{0}*'", txtsearch.Text.Trim())
                dt.DefaultView.RowFilter = filter
            Else
                dt.DefaultView.RowFilter = ""
            End If
        End If
    End Sub

    Private Sub btnclear_Click(sender As Object, e As EventArgs) Handles btnclear.Click
        clearfields()
    End Sub

    Public Sub clearfields()

        txtfname.Text = ""
        txtlname.Text = ""
        txtmname.Text = ""
        txtusername.Text = ""
        txtpassword.Text = ""
        txtemail.Text = ""
        txtcontactnumber.Text = ""
        txtaddress.Text = ""

        rbmale.Checked = False
        rbfemale.Checked = False
        rbassistant.Checked = False
        rbstaff.Checked = False
        CheckBox2.Checked = False

        txtmname.Enabled = True

        DataGridView1.ClearSelection()

        lblexample.ForeColor = Color.Black
        lblexample.Text = "Name@domain.com"
        lblpassword.Visible = False

    End Sub

    Private Sub txtusername_KeyDown(sender As Object, e As KeyEventArgs) Handles txtusername.KeyDown
        If e.Control AndAlso (e.KeyCode = Keys.V Or e.KeyCode = Keys.C Or e.KeyCode = Keys.X) Then
            e.SuppressKeyPress = True
        End If
    End Sub

    Private Sub txtpassword_KeyDown(sender As Object, e As KeyEventArgs) Handles txtpassword.KeyDown
        If e.Control AndAlso (e.KeyCode = Keys.V Or e.KeyCode = Keys.C Or e.KeyCode = Keys.X) Then
            e.SuppressKeyPress = True
        End If
    End Sub

    Private Sub txtemail_KeyDown(sender As Object, e As KeyEventArgs) Handles txtemail.KeyDown
        If e.Control AndAlso (e.KeyCode = Keys.V Or e.KeyCode = Keys.C Or e.KeyCode = Keys.X) Then
            e.SuppressKeyPress = True
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

    Private Sub txtemail_KeyPress(sender As Object, e As KeyPressEventArgs) Handles txtemail.KeyPress
        If Char.IsLetterOrDigit(e.KeyChar) OrElse Char.IsControl(e.KeyChar) Then
            e.Handled = False
            Return
        End If

        Select Case e.KeyChar
            Case "@"c, "."c, "-"c, "_"c
                e.Handled = False
                Return
        End Select

        If Char.IsWhiteSpace(e.KeyChar) Then
            e.Handled = True
            Return
        End If

        e.Handled = True
    End Sub


    Private Sub txtsearch_KeyDown(sender As Object, e As KeyEventArgs) Handles txtsearch.KeyDown
        If e.Control AndAlso (e.KeyCode = Keys.V Or e.KeyCode = Keys.C Or e.KeyCode = Keys.X) Then
            e.SuppressKeyPress = True
        End If
    End Sub

    Private Sub txtcontactnumber_KeyUp(sender As Object, e As KeyEventArgs) Handles txtcontactnumber.KeyUp
        If e.KeyCode = Keys.Back Then
            AddHandler txtcontactnumber.TextChanged, AddressOf txtcontactnumber_TextChanged
        End If
    End Sub

    Private Sub txtcontactnumber_TextChanged(sender As Object, e As EventArgs) Handles txtcontactnumber.TextChanged

        Dim oreyjeynal As String = txtcontactnumber.Text

        If String.IsNullOrEmpty(oreyjeynal) Then
            Return
        End If

        If oreyjeynal.StartsWith("09") Then
            If oreyjeynal.Length > 11 Then
                txtcontactnumber.Text = oreyjeynal.Substring(0, 11)
                txtcontactnumber.SelectionStart = 11
            End If
        Else
            If oreyjeynal.Length > 0 Then
                txtcontactnumber.Clear()
                txtcontactnumber.Text = "09"
                txtcontactnumber.SelectionStart = 2
            End If
        End If

    End Sub

    Private Sub CheckBox2_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox2.CheckedChanged

        If CheckBox2.Checked Then
            txtmname.Enabled = False
            txtmname.Text = ""
        Else
            txtmname.Enabled = True
            txtmname.Text = ""
        End If

    End Sub

    Private Sub Users_Staffs_FormClosed(sender As Object, e As FormClosedEventArgs) Handles Me.FormClosed
        clearfields()
    End Sub

    Private Sub txtfname_KeyPress(sender As Object, e As KeyPressEventArgs) Handles txtfname.KeyPress

        If Not Char.IsLetter(e.KeyChar) AndAlso Not Char.IsControl(e.KeyChar) AndAlso Not Char.IsWhiteSpace(e.KeyChar) Then
            e.Handled = True
        End If

    End Sub

    Private Sub txtfname_KeyDown(sender As Object, e As KeyEventArgs) Handles txtfname.KeyDown

        If e.Control AndAlso (e.KeyCode = Keys.V Or e.KeyCode = Keys.C Or e.KeyCode = Keys.X) Then
            e.SuppressKeyPress = True
        End If

    End Sub

    Private Sub txtlname_KeyPress(sender As Object, e As KeyPressEventArgs) Handles txtlname.KeyPress

        If Not Char.IsLetter(e.KeyChar) AndAlso Not Char.IsControl(e.KeyChar) AndAlso Not Char.IsWhiteSpace(e.KeyChar) Then
            e.Handled = True
        End If

    End Sub

    Private Sub txtlname_KeyDown(sender As Object, e As KeyEventArgs) Handles txtlname.KeyDown

        If e.Control AndAlso (e.KeyCode = Keys.V Or e.KeyCode = Keys.C Or e.KeyCode = Keys.X) Then
            e.SuppressKeyPress = True
        End If

    End Sub


    Private Sub txtmname_KeyDown(sender As Object, e As KeyEventArgs) Handles txtmname.KeyDown

        If e.Control AndAlso (e.KeyCode = Keys.V Or e.KeyCode = Keys.C Or e.KeyCode = Keys.X) Then
            e.SuppressKeyPress = True
        End If

        If e.KeyCode = Keys.Back Then

            isBackspacing = True
        End If

    End Sub

    Private Sub txtemail_TextChanged(sender As Object, e As EventArgs) Handles txtemail.TextChanged

        Dim email As String = txtemail.Text.Trim()

        If String.IsNullOrWhiteSpace(email) Then

            lblexample.ForeColor = Color.Black
            lblexample.Text = "Name@domain.com"
            Exit Sub
        End If

        Dim emailRegex As New System.Text.RegularExpressions.Regex("^[a-zA-Z0-9]+(?:[._%+-][a-zA-Z0-9]+)*@(?:[a-zA-Z0-9](?:[a-zA-Z0-9-]*[a-zA-Z0-9])?\.)+[a-zA-Z]{2,6}$")


        If emailRegex.IsMatch(email) Then

            lblexample.ForeColor = Color.Green
            lblexample.Text = " Name@domain.com ✓"

        Else

            lblexample.ForeColor = Color.Red
            lblexample.Text = " Name@domain.com ✕"
        End If

    End Sub

    Private Sub txtemail_Validating(sender As Object, e As System.ComponentModel.CancelEventArgs) Handles txtemail.Validating

        Dim EmailText As String = txtemail.Text.Trim()

        Dim EmailPattern As String = "^[a-zA-Z0-9]+(?:[._%+-][a-zA-Z0-9]+)*@(?:[a-zA-Z0-9](?:[a-zA-Z0-9-]*[a-zA-Z0-9])?\.)+[a-zA-Z]{2,6}$"

        If String.IsNullOrEmpty(EmailText) Then
            e.Cancel = False
            Return
        End If

        If Not System.Text.RegularExpressions.Regex.IsMatch(EmailText, EmailPattern, System.Text.RegularExpressions.RegexOptions.IgnoreCase) Then

            MessageBox.Show("Invalid email format. Please check the structure (e.g., name@domain.com) and ensure there are no double dots or invalid characters.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)

            e.Cancel = True
        Else
            e.Cancel = False
        End If

    End Sub

    Private Sub txtaddress_KeyPress(sender As Object, e As KeyPressEventArgs) Handles txtaddress.KeyPress

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

    Private Sub txtaddress_Validating(sender As Object, e As System.ComponentModel.CancelEventArgs) Handles txtaddress.Validating

        Dim AddressText As String = txtaddress.Text.Trim()
        Dim AddressPattern As String = "^(?=.*[a-zA-Z0-9])(?!.*[#.,/\-:'()\s]{2,})[a-zA-Z0-9\s#.,/\-:'()]+$"

        If String.IsNullOrEmpty(AddressText) Then
            e.Cancel = False
            Return
        End If

        If Not System.Text.RegularExpressions.Regex.IsMatch(AddressText, AddressPattern, System.Text.RegularExpressions.RegexOptions.IgnoreCase) Then

            MessageBox.Show("Invalid address format. Please ensure it contains alphanumeric characters and avoids consecutive special symbols.", "Validation Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning)

            e.Cancel = True
        Else
            e.Cancel = False
        End If

    End Sub

    Private Sub txtaddress_KeyDown(sender As Object, e As KeyEventArgs) Handles txtaddress.KeyDown
        If e.Control AndAlso (e.KeyCode = Keys.V Or e.KeyCode = Keys.C Or e.KeyCode = Keys.X) Then
            e.SuppressKeyPress = True
        End If
    End Sub


    Private Function paswurdstringth(ByVal Password As String) As Integer
        Dim Score As Integer = 0

        If Password.Length >= 8 Then
            Score += 1
        End If
        If Password.Length >= 12 Then
            Score += 1
        End If

        If System.Text.RegularExpressions.Regex.IsMatch(Password, "[a-z]") Then
            Score += 1
        End If

        If System.Text.RegularExpressions.Regex.IsMatch(Password, "[A-Z]") Then
            Score += 1
        End If

        If System.Text.RegularExpressions.Regex.IsMatch(Password, "\d") Then
            Score += 1
        End If

        If System.Text.RegularExpressions.Regex.IsMatch(Password, "[^a-zA-Z0-9\s]") Then
            Score += 2
        End If

        If String.IsNullOrWhiteSpace(Password) Then
            Return 0
        End If

        Return Score
    End Function

    Private Sub txtpass(sender As Object, e As EventArgs) Handles txtpassword.TextChanged
        Dim Password As String = txtpassword.Text


        If String.IsNullOrEmpty(Password) Then
            lblpassword.Visible = False
            Exit Sub
        Else
            lblpassword.Visible = True
        End If


        Dim StrengthScore As Integer = paswurdstringth(Password)


        Select Case StrengthScore
            Case 0, 1, 2
                lblpassword.ForeColor = Color.Red
                lblpassword.Text = "Weak: Password must be longer and more complex."
            Case 3, 4
                lblpassword.ForeColor = Color.Orange
                lblpassword.Text = "Moderate: Try adding a number or symbol."
            Case 5, 6
                lblpassword.ForeColor = Color.Blue
                lblpassword.Text = "Strong: Good combination of characters."
            Case Is >= 7
                lblpassword.ForeColor = Color.Green
                lblpassword.Text = "Excellent: Very strong password!"
        End Select

    End Sub

    Private Sub PictureBox2_Click(sender As Object, e As EventArgs) Handles PictureBox2.Click

        If txtpassword.PasswordChar = "•" Then
            PictureBox2.Image = Image.FromFile(Application.StartupPath & "\Resources\dilat.png")
            txtpassword.PasswordChar = ""
        Else
            PictureBox2.Image = Image.FromFile(Application.StartupPath & "\Resources\pikit.png")
            txtpassword.PasswordChar = "•"
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

    Private Sub CheckBox2_MouseHover(sender As Object, e As EventArgs) Handles CheckBox2.MouseHover
        Cursor = Cursors.Hand
    End Sub

    Private Sub CheckBox2_MouseLeave(sender As Object, e As EventArgs) Handles CheckBox2.MouseLeave
        Cursor = Cursors.Default
    End Sub

    Private Sub PictureBox2_MouseHover(sender As Object, e As EventArgs) Handles PictureBox2.MouseHover
        Cursor = Cursors.Hand
    End Sub

    Private Sub PictureBox2_MouseLeave(sender As Object, e As EventArgs) Handles PictureBox2.MouseLeave
        Cursor = Cursors.Default
    End Sub


    Private Sub txtmname_KeyPress(sender As Object, e As KeyPressEventArgs) Handles txtmname.KeyPress


        If Char.IsControl(e.KeyChar) Then
            e.Handled = False
            Return
        End If

        Dim currentText As String = txtmname.Text
        Dim currentLengthWithoutPeriod As Integer = currentText.Replace(".", "").Length


        If Not Char.IsLetter(e.KeyChar) AndAlso e.KeyChar <> "."c Then
            e.Handled = True
            Return
        End If


        If e.KeyChar = "."c AndAlso currentText.Contains(".") Then
            e.Handled = True
            Return
        End If


        If Char.IsLetter(e.KeyChar) AndAlso currentLengthWithoutPeriod >= 1 Then
            e.Handled = True
            Return
        End If


        If currentText.Length >= 2 AndAlso currentText.EndsWith(".") Then
            e.Handled = True
            Return
        End If


        If Char.IsLetter(e.KeyChar) AndAlso currentText.Contains(".") Then
            e.Handled = True
            Return
        End If

    End Sub

    Private Sub txtmname_TextChanged(sender As Object, e As EventArgs) Handles txtmname.TextChanged
        Static isFormatting As Boolean = False

        If isFormatting Then Return

        Dim currentText As String = txtmname.Text


        Dim initial As String = ""
        For Each c As Char In currentText
            If Char.IsLetter(c) Then
                If initial.Length < 1 Then initial &= c
            ElseIf c = "."c AndAlso Not initial.Contains(".") Then
                initial &= c
            End If
        Next

        If initial.Length > 0 AndAlso Char.IsLetter(initial(0)) Then
            Dim letterPart As String = initial(0).ToString().ToUpper()
            Dim formattedText As String = letterPart

            If initial.Length > 1 AndAlso initial.EndsWith(".") Then
                formattedText &= "."
            End If

            isFormatting = True
            If txtmname.Text <> formattedText Then
                txtmname.Text = formattedText
                txtmname.SelectionStart = txtmname.Text.Length
            End If
            isFormatting = False
        ElseIf currentText.Length > 0 AndAlso Not Char.IsLetter(currentText(0)) Then

            isFormatting = True
            txtmname.Text = ""
            isFormatting = False
        End If

    End Sub


End Class