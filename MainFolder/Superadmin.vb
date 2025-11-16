Imports System.IO
Imports Microsoft.VisualBasic.ApplicationServices
Imports MySql.Data.MySqlClient

Public Class Superadmin
    Private Sub Superadmin_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        DisablePaste_AllTextBoxes()
        txtpassword.PasswordChar = "•"

        Try
            Dim filePath As String = Application.StartupPath & "\Resources\pikit.png"
            If File.Exists(filePath) Then

                Using fs As New FileStream(filePath, FileMode.Open, FileAccess.Read)
                    PictureBox1.Image = New Bitmap(fs)
                End Using
            Else
                MessageBox.Show("Image file not found: pikit.png", "Image Load Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End If
        Catch ex As Exception
            MessageBox.Show("Error loading pikit.png for Superadmin Form: " & ex.Message, "Image Load Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try



        Dim con As New MySqlConnection(connectionString)
        Dim com As String = "SELECT * FROM `superadmin_tbl`"
        Dim adap As New MySqlDataAdapter(com, con)
        Dim ds As New DataSet

        adap.Fill(ds, "superadmin_info")

        DataGridView1.DataSource = ds.Tables("superadmin_info")
        DataGridView1.ClearSelection()
        DataGridView1.CurrentCell = Nothing


        DataGridView1.Columns("ID").Visible = False
        DataGridView1.EnableHeadersVisualStyles = False
        DataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(207, 58, 109)
        DataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White

    End Sub
    Private Sub Superadmin_Shown(sender As Object, e As EventArgs) Handles MyBase.Shown

        DataGridView1.ClearSelection()

    End Sub
    Private Sub btnclear_Click(sender As Object, e As EventArgs) Handles btnclear.Click
        clearlahat()
    End Sub

    Public Sub clearlahat()
        txtusername.Text = ""
        txtpassword.Text = ""
        txtfname.Text = ""
        txtlname.Text = ""
        txtmname.Text = ""
        txtcontact.Text = ""
        txtaddress.Text = ""
        txtemail.Text = ""
        rbmale.Checked = False
        rbfemale.Checked = False
        CheckBox1.Checked = False
        DataGridView1.ClearSelection()

    End Sub

    Private Sub btnadd_Click(sender As Object, e As EventArgs) Handles btnadd.Click

        Dim firstName As String = txtfname.Text.Trim()
        Dim lastName As String = txtlname.Text.Trim()
        Dim user As String = txtusername.Text.Trim()
        Dim contact As String = txtcontact.Text.Trim()
        Dim adr As String = txtaddress.Text.Trim()

        If firstName.Length < 2 Then
            MessageBox.Show("First Name must be 2 characters or more.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        If lastName.Length < 2 Then
            MessageBox.Show("Last Name must be 2 characters or more.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        If user.Length < 5 Then
            MessageBox.Show("Username must be atleast 5 characters or more.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        If adr.Length <= 5 Then
            MessageBox.Show("Address must be valid, atleast 5 characters or more.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Dim email As String = txtemail.Text.Trim()
        If Not String.IsNullOrEmpty(email) Then
            Dim emailRegex As New Text.RegularExpressions.Regex("^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.com$")
            If Not emailRegex.IsMatch(email) Then
                MessageBox.Show("Please enter a valid email address.", "Invalid Email", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If
        End If

        Dim gender As String = ""
        Dim middlename As String = txtmname.Text.Trim

        If rbmale.Checked Then
            gender = "Male"
        ElseIf rbfemale.Checked Then
            gender = "Female"
        End If

        If CheckBox1.Checked Then
            middlename = "N/A"
        End If

        If String.IsNullOrEmpty(txtusername.Text.Trim) OrElse String.IsNullOrEmpty(txtpassword.Text.Trim) OrElse String.IsNullOrEmpty(txtfname.Text.Trim) OrElse String.IsNullOrEmpty(txtlname.Text.Trim) Then
            MessageBox.Show("Please fill out all required fields.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        If Not IsPasswordValid() Then
            Return
        End If

        If contact.Length < 11 OrElse (contact.StartsWith("09") AndAlso contact.Length = 2) Then
            MessageBox.Show("Contact Number must be a valid length (e.g., 11 digits).", "Invalid Contact Number", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If


        Dim con As New MySqlConnection(connectionString)
        Dim com As New MySqlCommand("INSERT INTO superadmin_tbl (Username, Password, FirstName, LastName, MiddleName, ContactNumber, Address, Email, Gender, Role) VALUES (@username, @password, @fname, @lname, @mname, @contact, @address, @email, @gender, @role)", con)

        com.Parameters.AddWithValue("@username", txtusername.Text.Trim)
        com.Parameters.AddWithValue("@password", txtpassword.Text.Trim)
        com.Parameters.AddWithValue("@fname", txtfname.Text.Trim)
        com.Parameters.AddWithValue("@lname", txtlname.Text.Trim)
        com.Parameters.AddWithValue("@mname", middlename)
        com.Parameters.AddWithValue("@contact", txtcontact.Text.Trim)
        com.Parameters.AddWithValue("@address", txtaddress.Text.Trim)
        com.Parameters.AddWithValue("@email", txtemail.Text.Trim)
        com.Parameters.AddWithValue("@gender", gender)
        com.Parameters.AddWithValue("@role", "Librarian")


        Dim isalang As New MySqlCommand("SELECT COUNT(*) FROM `superadmin_tbl` WHERE `Role` = 'Librarian'", con)


        Try
            con.Open()

            Dim count As Integer = Convert.ToInt32(isalang.ExecuteScalar)

            If count > 0 Then
                MsgBox("Only one librarian is allowed.", vbExclamation)
                Return
            End If

            com.ExecuteNonQuery()
            MessageBox.Show("Librarian account successfully added!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Superadmin_Load(sender, e)
            clearlahat()
        Catch ex As Exception
            MessageBox.Show("Error adding account: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
        End Try
    End Sub

    Private Sub btnedit_Click(sender As Object, e As EventArgs) Handles btnedit.Click

        If DataGridView1.SelectedRows.Count = 0 Then
            MessageBox.Show("Please select a row to edit.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Dim firstName As String = txtfname.Text.Trim()
        Dim lastName As String = txtlname.Text.Trim()
        Dim user As String = txtusername.Text.Trim()
        Dim contact As String = txtcontact.Text.Trim()
        Dim adr As String = txtaddress.Text.Trim

        Dim editedEmail As String = txtemail.Text.Trim()
        Dim editedFirstName As String = txtfname.Text.Trim()
        Dim editedLastName As String = txtlname.Text.Trim()


        If String.IsNullOrEmpty(txtusername.Text.Trim) OrElse String.IsNullOrEmpty(txtpassword.Text.Trim) OrElse String.IsNullOrEmpty(txtfname.Text.Trim) OrElse String.IsNullOrEmpty(txtlname.Text.Trim) Then
            MessageBox.Show("Please fill out all required fields.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Dim email As String = txtemail.Text.Trim()
        If Not String.IsNullOrEmpty(email) Then
            Dim emailRegex As New Text.RegularExpressions.Regex("^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.com$")
            If Not emailRegex.IsMatch(email) Then
                MessageBox.Show("Please enter a valid email address.", "Invalid Email", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If
        End If

        If firstName.Length < 2 Then
            MessageBox.Show("First Name must be 2 characters or more.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        If lastName.Length < 2 Then
            MessageBox.Show("Last Name must be 2 characters or more.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        If user.Length < 5 Then
            MessageBox.Show("Username must be atleast 5 characters or more.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        If adr.Length <= 5 Then
            MessageBox.Show("Address must be valid, atleast 5 characters or more.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If


        If contact.Length < 11 OrElse (contact.StartsWith("09") AndAlso contact.Length = 2) Then
            MessageBox.Show("Contact Number must be a valid length (e.g., 11 digits).", "Invalid Contact Number", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Dim selectedRow As DataGridViewRow = DataGridView1.SelectedRows(0)
        Dim superadminId As Integer = CInt(selectedRow.Cells("ID").Value)

        Dim gender As String = ""
        If rbmale.Checked Then
            gender = "Male"
        ElseIf rbfemale.Checked Then
            gender = "Female"
        End If

        Dim middlename As String = txtmname.Text.Trim()
        If CheckBox1.Checked Then
            middlename = "N/A"
        End If

        If String.IsNullOrEmpty(txtusername.Text.Trim) OrElse String.IsNullOrEmpty(txtpassword.Text.Trim) OrElse String.IsNullOrEmpty(txtfname.Text.Trim) OrElse String.IsNullOrEmpty(txtlname.Text.Trim) Then
            MessageBox.Show("Please fill out all required fields (Username, Password, First Name, Last Name).", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        If Not IsPasswordValid() Then
            Return
        End If

        Dim con As New MySqlConnection(GlobalVarsModule.connectionString)
        Dim com As New MySqlCommand("UPDATE superadmin_tbl SET Username = @username, Password = @password, FirstName = @fname, LastName = @lname, MiddleName = @mname, ContactNumber = @contact, Address = @address, Email = @email, Gender = @gender, Role = @role WHERE ID = @id", con)

        com.Parameters.AddWithValue("@id", superadminId)
        com.Parameters.AddWithValue("@username", txtusername.Text.Trim)
        com.Parameters.AddWithValue("@password", txtpassword.Text.Trim)
        com.Parameters.AddWithValue("@fname", txtfname.Text.Trim)
        com.Parameters.AddWithValue("@lname", txtlname.Text.Trim)
        com.Parameters.AddWithValue("@mname", middlename)
        com.Parameters.AddWithValue("@contact", txtcontact.Text.Trim)
        com.Parameters.AddWithValue("@address", txtaddress.Text.Trim)
        com.Parameters.AddWithValue("@email", txtemail.Text.Trim)
        com.Parameters.AddWithValue("@gender", gender)
        com.Parameters.AddWithValue("@role", "Librarian")

        Try
            con.Open()
            com.ExecuteNonQuery()

            If GlobalVarsModule.GlobalEmail.Equals(editedEmail, StringComparison.OrdinalIgnoreCase) Then


                GlobalVarsModule.GlobalFullname = $"{editedFirstName} {editedLastName}".Trim()

                For Each form As Form In Application.OpenForms
                    If TypeOf form Is MainForm Then

                        Dim mainForm As MainForm = DirectCast(form, MainForm)
                        mainForm.lblgmail.Text = GlobalVarsModule.GlobalFullname
                    End If
                Next
            End If

            If MainForm.lblgmail.Text = selectedRow.Cells("Email").Value.ToString Then
                MainForm.lblgmail.Text = txtemail.Text.Trim
            End If



            MessageBox.Show("Librarian account successfully updated!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Superadmin_Load(sender, e)
            clearlahat()
        Catch ex As Exception
            MessageBox.Show("Error updating account: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
        End Try
    End Sub

    Private Function IsPasswordValid() As Boolean

        Const MIN_LENGTH As Integer = 10

        Const MIN_STRENGTH_SCORE As Integer = 5

        Dim Password As String = txtpassword.Text.Trim()


        If Password.Length < MIN_LENGTH Then
            MessageBox.Show($"Password must be between {MIN_LENGTH} characters long.", "Password Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtpassword.Focus()
            Return False
        End If

        Dim StrengthScore As Integer = paswurdstringth(Password)

        If StrengthScore < MIN_STRENGTH_SCORE Then
            MessageBox.Show("Password is too weak.", "Password Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtpassword.Focus()
            Return False
        End If

        Return True
    End Function

    Private Sub DataGridView1_CellClick_1(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellClick

        If e.RowIndex >= 0 Then
            Dim selectedRow = DataGridView1.Rows(e.RowIndex)

            txtusername.Text = selectedRow.Cells("Username").Value.ToString
            txtpassword.Text = selectedRow.Cells("Password").Value.ToString
            txtfname.Text = selectedRow.Cells("FirstName").Value.ToString
            txtlname.Text = selectedRow.Cells("LastName").Value.ToString

            Dim mname As String = selectedRow.Cells("MiddleName").Value.ToString
            If mname = "N/A" Then
                txtmname.Text = ""
                CheckBox1.Checked = True
            Else
                txtmname.Text = mname
                CheckBox1.Checked = False
            End If

            txtcontact.Text = selectedRow.Cells("ContactNumber").Value.ToString
            txtaddress.Text = selectedRow.Cells("Address").Value.ToString
            txtemail.Text = selectedRow.Cells("Email").Value.ToString

            Dim gender = selectedRow.Cells("Gender").Value.ToString
            If gender = "Male" Then
                rbmale.Checked = True
                rbfemale.Checked = False
            ElseIf gender = "Female" Then
                rbmale.Checked = False
                rbfemale.Checked = True
            Else
                rbmale.Checked = False
                rbfemale.Checked = False
            End If

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

    Private Sub PictureBox1_Click(sender As Object, e As EventArgs) Handles PictureBox1.Click

        If txtpassword.PasswordChar = "•" Then

            Try
                Dim filePath As String = Application.StartupPath & "\Resources\dilat.png"
                If File.Exists(filePath) Then
                    Using fs As New FileStream(filePath, FileMode.Open, FileAccess.Read)
                        PictureBox1.Image = New Bitmap(fs)
                    End Using
                    txtpassword.PasswordChar = ""
                Else
                    MessageBox.Show("Image file not found: dilat.png", "Image Load Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End If
            Catch ex As Exception
                MessageBox.Show("Error loading 'dilat.png': " & ex.Message, "Image Load Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        Else

            Try
                Dim filePath As String = Application.StartupPath & "\Resources\pikit.png"
                If File.Exists(filePath) Then
                    Using fs As New FileStream(filePath, FileMode.Open, FileAccess.Read)
                        PictureBox1.Image = New Bitmap(fs)
                    End Using
                    txtpassword.PasswordChar = "•"
                Else
                    MessageBox.Show("Image file not found: pikit.png", "Image Load Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End If
            Catch ex As Exception
                MessageBox.Show("Error loading 'pikit.png': " & ex.Message, "Image Load Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End If

    End Sub

    Private Sub CheckBox1_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox1.CheckedChanged

        If CheckBox1.Checked Then
            txtmname.Enabled = False
            txtmname.Text = ""
        Else
            txtmname.Enabled = True
        End If
    End Sub

    Private Sub Superadmin_KeyDown(sender As Object, e As KeyEventArgs) Handles MyBase.KeyDown

        If e.KeyCode = Keys.Escape Then
            Me.Close()
        End If

    End Sub

    Private Sub txtcontact_KeyDown(sender As Object, e As KeyEventArgs) Handles txtcontact.KeyDown

        If e.Control AndAlso (e.KeyCode = Keys.V Or e.KeyCode = Keys.C Or e.KeyCode = Keys.X) Then
            e.SuppressKeyPress = True
        End If

        If e.KeyCode = Keys.Back Then
            RemoveHandler txtcontact.TextChanged, AddressOf txtcontact_TextChanged
        End If

    End Sub

    Private Sub txtcontact_KeyPress(sender As Object, e As KeyPressEventArgs) Handles txtcontact.KeyPress


        If Not Char.IsDigit(e.KeyChar) And Not Char.IsControl(e.KeyChar) Then
            e.Handled = True
        End If

    End Sub

    Private Sub txtaddress_KeyDown(sender As Object, e As KeyEventArgs) Handles txtaddress.KeyDown

        If e.Control AndAlso (e.KeyCode = Keys.V Or e.KeyCode = Keys.C Or e.KeyCode = Keys.X) Then
            e.SuppressKeyPress = True
        End If

    End Sub


    Private Sub txtcontact_TextChanged(sender As Object, e As EventArgs) Handles txtcontact.TextChanged


        Dim oreyjeynal As String = txtcontact.Text

        If String.IsNullOrEmpty(oreyjeynal) Then
            Return
        End If

        If oreyjeynal.StartsWith("09") Then

        Else

            If oreyjeynal.Length > 0 Then
                txtcontact.Clear()
                txtcontact.Text = "09"
                txtcontact.SelectionStart = 2
            End If
        End If

    End Sub

    Private Sub txtfname_KeyPress(sender As Object, e As KeyPressEventArgs) Handles txtfname.KeyPress

        If Not Char.IsLetter(e.KeyChar) And Not Char.IsControl(e.KeyChar) And Not Char.IsWhiteSpace(e.KeyChar) Then
            e.Handled = True
        End If
    End Sub


    Private Sub txtlname_KeyPress(sender As Object, e As KeyPressEventArgs) Handles txtlname.KeyPress

        If Not Char.IsLetter(e.KeyChar) And Not Char.IsControl(e.KeyChar) And Not Char.IsWhiteSpace(e.KeyChar) Then
            e.Handled = True
        End If
    End Sub


    Private Sub txtmname_KeyPress(sender As Object, e As KeyPressEventArgs) Handles txtmname.KeyPress

        If Not Char.IsLetter(e.KeyChar) And Not Char.IsControl(e.KeyChar) And Not Char.IsWhiteSpace(e.KeyChar) Then
            e.Handled = True
        End If
    End Sub



    Private Sub txtcontactnumber_KeyUp(sender As Object, e As KeyEventArgs) Handles txtcontact.KeyUp


        If e.KeyCode = Keys.Back Then
            AddHandler txtcontact.TextChanged, AddressOf txtcontact_TextChanged

        End If

    End Sub

    Private Sub txtfname_KeyDown(sender As Object, e As KeyEventArgs) Handles txtfname.KeyDown

        If e.Control AndAlso (e.KeyCode = Keys.V Or e.KeyCode = Keys.C Or e.KeyCode = Keys.X) Then
            e.SuppressKeyPress = True
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

    End Sub

    Private Sub txtuser_KeyDown(sender As Object, e As KeyEventArgs) Handles txtusername.KeyDown

        If e.Control AndAlso (e.KeyCode = Keys.V Or e.KeyCode = Keys.C Or e.KeyCode = Keys.X) Then
            e.SuppressKeyPress = True
        End If

    End Sub

    Private Sub txtpass_KeyDown(sender As Object, e As KeyEventArgs) Handles txtpassword.KeyDown

        If e.Control AndAlso (e.KeyCode = Keys.V Or e.KeyCode = Keys.C Or e.KeyCode = Keys.X) Then
            e.SuppressKeyPress = True
        End If

    End Sub

    Private Sub txtemail_KeyDown(sender As Object, e As KeyEventArgs) Handles txtemail.KeyDown

        If e.Control AndAlso (e.KeyCode = Keys.V Or e.KeyCode = Keys.C Or e.KeyCode = Keys.X) Then
            e.SuppressKeyPress = True
        End If

    End Sub

    Private Sub Superadmin_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing

        If DataGridView1.Rows.Count = 0 Then
            Dim result As DialogResult = MessageBox.Show("You must add at least one librarian account.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            e.Cancel = True
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


    Private Sub txtpassword_TextChanged(sender As Object, e As EventArgs) Handles txtpassword.TextChanged

        Dim Password As String = txtpassword.Text
        Dim Length As Integer = Password.Length
        Const MIN_LENGTH As Integer = 10

        If String.IsNullOrEmpty(Password) Then
            lblpassword.Visible = False
            Exit Sub
        Else
            lblpassword.Visible = True
        End If


        If Length < MIN_LENGTH Then
            lblpassword.ForeColor = Color.Red
            lblpassword.Text = $"CRITICAL: Password must be at least {MIN_LENGTH} characters long."
            Exit Sub
        End If


        Dim StrengthScore As Integer = paswurdstringth(Password)

        Select Case StrengthScore
            Case 0, 1, 2
                lblpassword.ForeColor = Color.OrangeRed
                lblpassword.Text = "Weak: Need more character types (Uppercase, number, or symbol)."
            Case 3, 4
                lblpassword.ForeColor = Color.Orange
                lblpassword.Text = "Moderate: Try adding another character type."
            Case 5, 6
                lblpassword.ForeColor = Color.Blue
                lblpassword.Text = "Strong: Good combination of characters."
            Case Is >= 7
                lblpassword.ForeColor = Color.Green
                lblpassword.Text = "Excellent: Very strong password!"
        End Select

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

            MessageBox.Show("Invalid address format.", "Validation Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning)

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

    Private Sub btnclear_MouseHover(sender As Object, e As EventArgs) Handles btnclear.MouseHover
        Cursor = Cursors.Hand
    End Sub

    Private Sub btnclear_MouseLeave(sender As Object, e As EventArgs) Handles btnclear.MouseLeave
        Cursor = Cursors.Default
    End Sub

    Private Sub CheckBox1_MouseHover(sender As Object, e As EventArgs) Handles CheckBox1.MouseHover
        Cursor = Cursors.Hand
    End Sub

    Private Sub CheckBox1_MouseLeave(sender As Object, e As EventArgs) Handles CheckBox1.MouseLeave
        Cursor = Cursors.Default
    End Sub

    Private Sub PictureBox1_MouseHover(sender As Object, e As EventArgs) Handles PictureBox1.MouseHover
        Cursor = Cursors.Hand
    End Sub

    Private Sub PictureBox1_MouseLeave(sender As Object, e As EventArgs) Handles PictureBox1.MouseLeave
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


    'hatdoggggg'''
    'palalala'
End Class