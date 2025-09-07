Imports System.IO
Imports System.Security.Cryptography
Imports System.Text
Imports MySql.Data.MySqlClient
Imports TheArtOfDevHtmlRenderer.Adapters
Imports Windows.Win32.System
Imports System.Text.RegularExpressions
Public Class Users_Staffs

    Private Sub Users_Staffs_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        Dim con As New MySqlConnection(connectionString)
        Dim com As String = "SELECT * FROM `user_staff_tbl`"
        Dim adapp As New MySqlDataAdapter(com, con)
        Dim dt As New DataSet

        adapp.Fill(dt, "INFO")
        DataGridView1.DataSource = dt.Tables("INFO")

        DataGridView1.Columns("ID").Visible = False
        DataGridView1.EnableHeadersVisualStyles = False
        DataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(207, 58, 109)
        DataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White

        AddHandler DataGridView1.CellFormatting, AddressOf Me.DataGridView1_CellFormatting

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

        If String.IsNullOrWhiteSpace(firstName) OrElse String.IsNullOrWhiteSpace(lastName) OrElse String.IsNullOrWhiteSpace(user) OrElse String.IsNullOrWhiteSpace(pass) OrElse String.IsNullOrWhiteSpace(email) Then
            MsgBox("Please fill in all the required fields (First Name, Last Name, Username, Password, Email).", vbExclamation, "Missing Information")
            Exit Sub
        End If

        Dim emailRegex As New System.Text.RegularExpressions.Regex("^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$")
        If Not emailRegex.IsMatch(email) Then
            MsgBox("Invalid email format. Please enter a valid email address.", vbExclamation, "Invalid Email")
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


        Dim middleName As String
        If CheckBox2.Checked Then
            middleName = "N/A"
        Else
            middleName = txtmname.Text.Trim()
        End If

        Dim con As New MySqlConnection(connectionString)
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

            Dim com As New MySqlCommand("INSERT INTO `user_staff_tbl`(`FirstName`, `LastName`, `MiddleName`, `Username`, `Password`, `Email`, `ContactNumber`, `Gender`) VALUES (@firstName, @lastName, @middleName, @username, @password, @email, @contact, @gender)", con)
            com.Parameters.AddWithValue("@firstName", firstName)
            com.Parameters.AddWithValue("@lastName", lastName)
            com.Parameters.AddWithValue("@middleName", middleName)
            com.Parameters.AddWithValue("@username", user)
            com.Parameters.AddWithValue("@password", pass)
            com.Parameters.AddWithValue("@email", email)
            com.Parameters.AddWithValue("@contact", contact)
            com.Parameters.AddWithValue("@gender", gender)
            com.ExecuteNonQuery()

            MsgBox("Staff member added successfully!", vbInformation)
            Users_Staffs_Load(sender, e)
            clearfields()

        Catch ex As Exception
            MsgBox(ex.Message, vbCritical, "Database Error")
        Finally
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
        End Try
    End Sub



    Private Sub CheckBox1_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox1.CheckedChanged

        If CheckBox1.Checked = True Then
            txtpassword.UseSystemPasswordChar = False
        Else
            txtpassword.UseSystemPasswordChar = True
        End If
    End Sub

    Private Sub txtpassword_TextChanged(sender As Object, e As EventArgs) Handles txtpassword.TextChanged

        If CheckBox1.Checked = True Then
            txtpassword.UseSystemPasswordChar = False
        Else
            txtpassword.UseSystemPasswordChar = True
        End If
    End Sub



    Private Sub btnedit_Click(sender As Object, e As EventArgs) Handles btnedit.Click

        If DataGridView1.CurrentRow Is Nothing Then
            MsgBox("Please select a row before to edit.", vbExclamation)
            Exit Sub
        End If

        Dim ID As Integer = CInt(DataGridView1.CurrentRow.Cells("ID").Value)
        Dim firstName As String = txtfname.Text.Trim()
        Dim lastName As String = txtlname.Text.Trim()
        Dim user As String = txtusername.Text.Trim()
        Dim pass As String = txtpassword.Text.Trim()
        Dim email As String = txtemail.Text.Trim()
        Dim contact As String = txtcontactnumber.Text.Trim()
        Dim gender As String = ""

        If String.IsNullOrWhiteSpace(firstName) OrElse String.IsNullOrWhiteSpace(lastName) OrElse String.IsNullOrWhiteSpace(user) OrElse String.IsNullOrWhiteSpace(pass) OrElse String.IsNullOrWhiteSpace(email) Then
            MsgBox("Please fill in all required fields.", vbExclamation, "Missing Information")
            Exit Sub
        End If

        Dim emailRegex As New System.Text.RegularExpressions.Regex("^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$")
        If Not emailRegex.IsMatch(email) Then
            MsgBox("Invalid email format. Please enter a valid email address.", vbExclamation, "Invalid Email")
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


        Dim middleName As String
        If CheckBox2.Checked Then
            middleName = "N/A"
        Else
            middleName = txtmname.Text.Trim()
        End If

        Dim con As New MySqlConnection(connectionString)
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

            Dim Coms As New MySqlCommand("UPDATE `user_staff_tbl` SET `FirstName` = @firstName, `LastName` = @lastName, `MiddleName` = @middleName, `Username` = @username, `Password` = @password, `Email` = @email, `ContactNumber` = @contact, `Gender` = @gender WHERE `ID` = @id", con)
            Coms.Parameters.AddWithValue("@firstName", firstName)
            Coms.Parameters.AddWithValue("@lastName", lastName)
            Coms.Parameters.AddWithValue("@middleName", middleName)
            Coms.Parameters.AddWithValue("@username", user)
            Coms.Parameters.AddWithValue("@password", pass)
            Coms.Parameters.AddWithValue("@email", email)
            Coms.Parameters.AddWithValue("@contact", contact)
            Coms.Parameters.AddWithValue("@gender", gender)
            Coms.Parameters.AddWithValue("@id", ID)
            Coms.ExecuteNonQuery()

            MsgBox("Staff member updated successfully!", vbInformation)
            Users_Staffs_Load(sender, e)
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

                Dim con As New MySqlConnection(connectionString)
                Dim selectedRow As DataGridViewRow = DataGridView1.SelectedRows(0)
                Dim ID As Integer = CInt(selectedRow.Cells("ID").Value)

                Try
                    con.Open()
                    Dim delete As New MySqlCommand("DELETE FROM `user_staff_tbl` WHERE `ID` = @id", con)
                    delete.Parameters.AddWithValue("@id", ID)
                    delete.ExecuteNonQuery()

                    For Each form In Application.OpenForms
                        If TypeOf form Is Book Then
                            Dim book = DirectCast(form, Book)
                            book.cbcategoryy()
                        End If
                    Next

                    MsgBox("Staff member deleted successfully.", vbInformation)
                    Users_Staffs_Load(sender, e)
                    clearfields()


                    Dim count As New MySqlCommand("SELECT COUNT(*) FROM `user_staff_tbl`", con)
                    Dim rowCount As Long = CLng(count.ExecuteScalar())

                    If rowCount = 0 Then

                        Dim reset As New MySqlCommand("ALTER TABLE `user_staff_tbl` AUTO_INCREMENT = 1", con)
                        reset.ExecuteNonQuery()

                    End If

                Catch ex As Exception
                    MsgBox(ex.Message, vbCritical)
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

            Dim gender As String = If(IsDBNull(row.Cells("Gender").Value), String.Empty, row.Cells("Gender").Value.ToString())
            rbmale.Checked = (gender = "Male")
            rbfemale.Checked = (gender = "Female")

            Dim middleName As String = If(IsDBNull(row.Cells("MiddleName").Value), String.Empty, row.Cells("MiddleName").Value.ToString())

            If middleName = "N/A" Then

                CheckBox2.Checked = True
                txtmname.Text = "N/A"
                txtmname.Enabled = False
            Else

                CheckBox2.Checked = False
                txtmname.Text = middleName
                txtmname.Enabled = True
            End If
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


        rbmale.Checked = False
        rbfemale.Checked = False
        CheckBox2.Checked = False

        txtmname.Enabled = True

        DataGridView1.ClearSelection()

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

        If e.KeyChar = " "c AndAlso String.IsNullOrEmpty(txtemail.Text) Then
            e.Handled = True


        End If

        If Not Char.IsLetterOrDigit(e.KeyChar) AndAlso Not Char.IsControl(e.KeyChar) AndAlso e.KeyChar <> "@" AndAlso e.KeyChar <> "." Then
            e.Handled = True
        End If


        If Char.IsWhiteSpace(e.KeyChar) Then
            e.Handled = True
        End If

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

        Else

            If oreyjeynal.Length > 0 Then
                txtcontactnumber.Clear()
                txtcontactnumber.Text = "09"
                txtcontactnumber.SelectionStart = 2
            End If
        End If

    End Sub

    Private Sub txtemail_TextChanged(sender As Object, e As EventArgs) Handles txtemail.TextChanged


        Dim email As String = txtemail.Text.Trim()

        If String.IsNullOrWhiteSpace(email) Then

            lblexample.ForeColor = Color.Black
            lblexample.Text = "Example@email.com"
            Exit Sub
        End If


        Dim emailRegex As New Regex("^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$")


        If emailRegex.IsMatch(email) Then

            lblexample.ForeColor = Color.Green
            lblexample.Text = "Example@email.com ✓"

        Else

            lblexample.ForeColor = Color.Red
            lblexample.Text = "Example@email.com ✕"
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

    ''pagod na ako mga bes''
End Class