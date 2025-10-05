Imports MySql.Data.MySqlClient
Imports System.IO

Public Class Superadmin
    Private Sub Superadmin_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        txtpassword.PasswordChar = "•"
        PictureBox1.Image = Image.FromFile(Application.StartupPath & "\Resources\pikit.png")

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
            MessageBox.Show("Please fill out all required fields (Username, Password, First Name, Last Name).", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning)
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

        Dim email As String = txtemail.Text.Trim()
        If Not String.IsNullOrEmpty(email) Then
            Dim emailRegex As New Text.RegularExpressions.Regex("^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.com$")
            If Not emailRegex.IsMatch(email) Then
                MessageBox.Show("Please enter a valid email address.", "Invalid Email", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If
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

        Dim con As New MySqlConnection(connectionString)
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

    Private Sub txtemail_TextChanged_1(sender As Object, e As EventArgs) Handles txtemail.TextChanged

        Dim email = txtemail.Text.Trim
        If String.IsNullOrWhiteSpace(email) Then
            lblexample.ForeColor = Color.Black
            lblexample.Text = "Example@gmail.com"
            Exit Sub
        End If

        Dim emailRegex As New Text.RegularExpressions.Regex("^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.com$")
        If emailRegex.IsMatch(email) Then
            lblexample.ForeColor = Color.Green
            lblexample.Text = "Example@gmail.com ✓"
        Else
            lblexample.ForeColor = Color.Red
            lblexample.Text = "Example@gmail.com ✕"
        End If

    End Sub

    Private Sub PictureBox1_Click(sender As Object, e As EventArgs) Handles PictureBox1.Click

        If txtpassword.PasswordChar = "•" Then
            PictureBox1.Image = Image.FromFile(Application.StartupPath & "\Resources\dilat.png")
            txtpassword.PasswordChar = ""
        Else
            PictureBox1.Image = Image.FromFile(Application.StartupPath & "\Resources\pikit.png")
            txtpassword.PasswordChar = "•"
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

    Private Sub txtaddress_KeyPress(sender As Object, e As KeyPressEventArgs) Handles txtaddress.KeyPress

        If Not Char.IsLetterOrDigit(e.KeyChar) AndAlso Not Char.IsControl(e.KeyChar) AndAlso Not Char.IsWhiteSpace(e.KeyChar) AndAlso e.KeyChar <> "#" Then
            e.Handled = True
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
                lblpassword.Text = "Excellent: Very strong password! ✓"
        End Select

    End Sub

    'hatdoggggg'''
    'palalala'
End Class