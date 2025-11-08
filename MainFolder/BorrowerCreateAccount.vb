Imports System.Data
Imports System.IO
Imports MySql.Data.MySqlClient

Public Class BorrowerCreateAccount

    Private Function fullnamsesu(ByVal ID As String, ByVal IDType As String) As String


        If String.IsNullOrWhiteSpace(ID) Then
            Return "Borrower Full Name:"
        End If

        Dim fullName As String = Nothing
        Dim columnName As String = If(IDType = "LRN", "LRN", "EmployeeNo")


        Dim com As String = $"SELECT CONCAT_WS(' ', `FirstName`, NULLIF(NULLIF(TRIM(`MiddleInitial`), 'N/A'), ''), `LastName`) FROM `borrower_tbl` WHERE `{columnName}` = @ID LIMIT 1"

        Using con As New MySqlConnection(connectionString)
            Try
                con.Open()
                Using cmd As New MySqlCommand(com, con)
                    cmd.Parameters.AddWithValue("@ID", ID)

                    Dim result As Object = cmd.ExecuteScalar()

                    If result IsNot Nothing AndAlso Not IsDBNull(result) Then

                        fullName = result.ToString().Trim()
                    End If
                End Using

            Catch ex As MySqlException
                Console.WriteLine("Database Error in GetBorrowerFullName: " & ex.Message)
                Return "Error retrieving data"
            Finally
                If con.State = ConnectionState.Open Then
                    con.Close()
                End If
            End Try
        End Using


        If Not String.IsNullOrWhiteSpace(fullName) Then

            Return fullName
        Else

            Return "Borrower Full Name: Not Found"
        End If
    End Function


    Private Sub BorrowerCreateAccount_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        clear()
        refreshcreate()
        DisablePaste_AllTextBoxes()
    End Sub

    Public Sub refreshcreate()
        TopMost = True
        txtpass.PasswordChar = "•"

        Try
            Dim filePath As String = Application.StartupPath & "\Resources\pikit.png"
            If File.Exists(filePath) Then

                Using fs As New FileStream(filePath, FileMode.Open, FileAccess.Read)
                    PictureBox1.Image = New Bitmap(fs)
                End Using
            Else
                MessageBox.Show("Image file not found: pikit.png sa BorrowerCreateAccount.", "Image Load Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End If
        Catch ex As Exception
            MessageBox.Show("Error loading pikit.png for BorrowerCreateAccount Form: " & ex.Message, "Image Load Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try


    End Sub

    Public Sub clear()
        txtemployeeno.Enabled = False
        txtlrn.Enabled = False

        txtemployeeno.Text = ""
        txtlrn.Text = ""
        txtuser.Text = ""
        txtpass.Text = ""
        txtemail.Text = ""
        lblfullname.Text = "Borrower Full Name:"
    End Sub

    Private Sub PictureBox1_Click(sender As Object, e As EventArgs) Handles PictureBox1.Click

        If txtpass.PasswordChar = "•" Then

            Try
                Dim filePath As String = Application.StartupPath & "\Resources\dilat.png"
                If File.Exists(filePath) Then
                    Using fs As New FileStream(filePath, FileMode.Open, FileAccess.Read)
                        PictureBox1.Image = New Bitmap(fs)
                    End Using
                    txtpass.PasswordChar = ""
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
                    txtpass.PasswordChar = "•"
                Else
                    MessageBox.Show("Image file not found: pikit.png", "Image Load Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End If
            Catch ex As Exception
                MessageBox.Show("Error loading 'pikit.png': " & ex.Message, "Image Load Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
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

    Private Sub txtpasswords(sender As Object, e As EventArgs) Handles txtpass.TextChanged
        Dim Password As String = txtpass.Text

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


    Private Sub txtlrn_TextChanged(sender As Object, e As EventArgs) Handles txtlrn.TextChanged
        If rbstudent.Checked Then
            lblfullname.Text = fullnamsesu(txtlrn.Text.Trim(), "LRN")
        End If
    End Sub


    Private Sub txtemployeeno_TextChanged(sender As Object, e As EventArgs) Handles txtemployeeno.TextChanged
        If rbteacher.Checked Then
            lblfullname.Text = fullnamsesu(txtemployeeno.Text.Trim(), "EmployeeNo")
        End If
    End Sub


    Private Sub rbteacher_CheckedChanged(sender As Object, e As EventArgs) Handles rbteacher.CheckedChanged
        If rbteacher.Checked Then
            txtemployeeno.Enabled = True
            txtlrn.Enabled = False

            txtlrn.Text = ""
            lblfullname.Text = "Borrower Full Name:"
            txtemployeeno.Focus()
        End If
    End Sub

    Private Sub rbstudent_CheckedChanged(sender As Object, e As EventArgs) Handles rbstudent.CheckedChanged
        If rbstudent.Checked Then
            txtlrn.Enabled = True
            txtemployeeno.Enabled = False

            txtemployeeno.Text = ""
            lblfullname.Text = "Borrower Full Name:"
            txtlrn.Focus()
        End If
    End Sub

    Private Sub txtemail_TextChanged(sender As Object, e As EventArgs) Handles txtemail.TextChanged
        Dim email As String = txtemail.Text.Trim()
        If String.IsNullOrWhiteSpace(email) Then
            lbldomain.ForeColor = Color.Black
            lbldomain.Text = "Name@domain.com"
            Exit Sub
        End If

        Dim emailRegex As New System.Text.RegularExpressions.Regex("^[a-zA-Z0-9]+(?:[._%+-][a-zA-Z0-9]+)*@(?:[a-zA-Z0-9](?:[a-zA-Z0-9-]*[a-zA-Z0-9])?\.)+[a-zA-Z]{2,6}$")

        If emailRegex.IsMatch(email) Then
            lbldomain.ForeColor = Color.Green
            lbldomain.Text = " Name@domain.com ✓"
        Else
            lbldomain.ForeColor = Color.Red
            lbldomain.Text = " Name@domain.com ✕"
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

    Private Function CheckIfAccountExists(ByVal IDValue As String, ByVal IDType As String, ByVal Username As String) As Boolean
        Dim exists As Boolean = False
        Dim IDColumn As String = If(IDType = "LRN", "LRN", "EmployeeNo")


        Dim sql As String = $"SELECT 1 FROM `borroweredit_tbl` WHERE `{IDColumn}` = @ID OR `Username` = @User LIMIT 1"

        Using con As New MySqlConnection(connectionString)
            Try
                con.Open()
                Using cmd As New MySqlCommand(sql, con)
                    cmd.Parameters.AddWithValue("@ID", IDValue)
                    cmd.Parameters.AddWithValue("@User", Username)

                    Dim result As Object = cmd.ExecuteScalar()
                    If result IsNot Nothing AndAlso Not IsDBNull(result) Then
                        exists = True
                    End If
                End Using
            Catch ex As MySqlException
                MessageBox.Show("A database error occurred during existence check: " & ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End Using
        Return exists
    End Function

    Private Sub btnsignup_Click(sender As Object, e As EventArgs) Handles btnsignup.Click
        Dim IDValue As String = If(rbstudent.Checked, txtlrn.Text.Trim(), txtemployeeno.Text.Trim())
        Dim IDType As String = If(rbstudent.Checked, "LRN", "EmployeeNo")
        Dim Username As String = txtuser.Text.Trim()
        Dim Password As String = txtpass.Text.Trim()
        Dim Email As String = txtemail.Text.Trim()
        Dim FullName As String = lblfullname.Text.Trim()


        If String.IsNullOrWhiteSpace(IDValue) Then
            MessageBox.Show($"Please enter your {IDType}.", "Required Field", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        If String.IsNullOrWhiteSpace(Username) OrElse String.IsNullOrWhiteSpace(Password) Then
            MessageBox.Show("Please enter both username and password.", "Required Fields", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        If FullName = "Borrower Full Name: Not Found" OrElse FullName = "Error retrieving data" OrElse FullName = "Borrower Full Name:" Then
            MessageBox.Show($"No borrower found with the entered {IDType}.", "Verification Failed", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End If


        If AccountExistsInAllTables(Username, Email) Then
            MessageBox.Show("The username or email is already taken by another account.", "Duplicate Account", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If


        If CheckIfAccountExists(IDValue, IDType, Username) Then
            MessageBox.Show("An account already exists for this ID or username.", "Account Exists", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If


        Dim con As New MySqlConnection(connectionString)
        Dim sql As String = $"INSERT INTO `borroweredit_tbl` (`{IDType}`, `Username`, `Password`, `Email`) VALUES (@ID, @User, @Pass, @Email)"

        Try
            con.Open()
            Using cmd As New MySqlCommand(sql, con)
                cmd.Parameters.AddWithValue("@ID", IDValue)
                cmd.Parameters.AddWithValue("@User", Username)
                cmd.Parameters.AddWithValue("@Pass", Password)
                cmd.Parameters.AddWithValue("@Email", Email)

                Dim rowsAffected As Integer = cmd.ExecuteNonQuery()

                If rowsAffected > 0 Then
                    MessageBox.Show("Account successfully created.", "Registration Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
                    clear()
                    login.Show()
                    Me.Hide()
                Else
                    MessageBox.Show("Account creation failed. Please try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End If
            End Using
        Catch ex As MySqlException
            MessageBox.Show("A database error occurred during registration: " & ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            If con.State = ConnectionState.Open Then con.Close()
        End Try
    End Sub

    Private Function AccountExistsInAllTables(username As String, email As String) As Boolean
        Dim query As String =
        "SELECT COUNT(*) FROM (" &
        "SELECT Username, Email FROM borroweredit_tbl " &
        "UNION ALL " &
        "SELECT Username, Email FROM user_staff_tbl " &
        "UNION ALL " &
        "SELECT Username, Email FROM superadmin_tbl" &
        ") AS combined WHERE Username = @Username OR Email = @Email"

        Using con As New MySqlConnection(connectionString)
            Using cmd As New MySqlCommand(query, con)
                cmd.Parameters.AddWithValue("@Username", username)
                cmd.Parameters.AddWithValue("@Email", email)
                con.Open()
                Dim count As Integer = Convert.ToInt32(cmd.ExecuteScalar())
                Return count > 0
            End Using
        End Using
    End Function


    Private Sub LinkLabel1_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles LinkLabel1.LinkClicked

        login.Show()
        Me.Hide()
    End Sub

    Private Sub PictureBox1_MouseHover_1(sender As Object, e As EventArgs) Handles PictureBox1.MouseHover

        Cursor = Cursors.Hand

    End Sub

    Private Sub PictureBox1_MouseLeave_1(sender As Object, e As EventArgs) Handles PictureBox1.MouseLeave

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