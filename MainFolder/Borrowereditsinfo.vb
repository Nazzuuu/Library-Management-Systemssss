Imports MySql.Data.MySqlClient
Imports System.Data

Public Class Borrowereditsinfo

    Private selectedBorrowerID As Integer = -1


    Private originalIDValue As String = String.Empty

    Private Sub Borrowereditinfo_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        refresheditt()
    End Sub

    Private Sub borroweredit_shown(sender As Object, e As EventArgs) Handles MyBase.Shown

        DataGridView1.ClearSelection()

    End Sub

    Public Sub refresheditt()

        txtpass.PasswordChar = "•"
        PictureBox1.Image = Image.FromFile(Application.StartupPath & "\Resources\pikit.png")


        Dim borrowerType As String = GlobalVarsModule.CurrentBorrowerType
        Dim borrowerID As String = GlobalVarsModule.CurrentBorrowerID
        Dim userRole As String = GlobalVarsModule.CurrentUserRole

        Dim con As New MySqlConnection(connectionString)
        Dim com As String = ""
        Dim showAllRecords As Boolean = True


        If userRole = "Borrower" AndAlso Not String.IsNullOrWhiteSpace(borrowerID) Then
            showAllRecords = False

            If borrowerType = "Student" Then

                com = "SELECT `ID`, `LRN`, `EmployeeNo`, `Username`, `Password`, `Email` FROM `borroweredit_tbl` WHERE `LRN` = @BorrowerID"
            ElseIf borrowerType = "Teacher" Then

                com = "SELECT `ID`, `LRN`, `EmployeeNo`, `Username`, `Password`, `Email` FROM `borroweredit_tbl` WHERE `EmployeeNo` = @BorrowerID"
            End If
        End If


        If showAllRecords OrElse String.IsNullOrWhiteSpace(com) Then

            com = "SELECT `ID`, `LRN`, `EmployeeNo`, `Username`, `Password`, `Email` FROM `borroweredit_tbl`"
        End If

        Dim adap As New MySqlDataAdapter()
        Dim ds As New DataSet

        Try
            con.Open()

            Using cmd As New MySqlCommand(com, con)

                If Not showAllRecords Then
                    cmd.Parameters.AddWithValue("@BorrowerID", borrowerID)
                End If

                adap.SelectCommand = cmd
                adap.Fill(ds, "info")

                DataGridView1.DataSource = ds.Tables("info")

                If DataGridView1.Columns.Contains("ID") Then
                    DataGridView1.Columns("ID").Visible = False
                End If


                If userRole = "Borrower" Then
                    If DataGridView1.Columns.Contains("LRN") AndAlso borrowerType = "Teacher" Then
                        DataGridView1.Columns("LRN").Visible = False
                    End If
                    If DataGridView1.Columns.Contains("EmployeeNo") AndAlso borrowerType = "Student" Then
                        DataGridView1.Columns("EmployeeNo").Visible = False
                    End If
                End If

                DataGridView1.EnableHeadersVisualStyles = False
                DataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(207, 58, 109)
                DataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White
            End Using
        Catch ex As Exception
            MessageBox.Show("Error loading data: " & ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
        End Try

        clearlahat()
    End Sub


    Public Sub clearlahat()
        txtemployeeno.Enabled = False
        txtlrn.Enabled = False

        txtemployeeno.Text = ""
        txtlrn.Text = ""
        txtuser.Text = ""
        txtpass.Text = ""
        txtemail.Text = ""
        selectedBorrowerID = -1
        originalIDValue = String.Empty


        lbldomain.ForeColor = Color.Black
        lbldomain.Text = "Name@domain.com"


        If Not lblpassword Is Nothing Then
            lblpassword.Text = ""
            lblpassword.Visible = False
        End If
    End Sub

    Private Sub DataGridView1_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellClick

        If e.RowIndex < 0 Then Return

        Dim row As DataGridViewRow = DataGridView1.Rows(e.RowIndex)

        selectedBorrowerID = Convert.ToInt32(row.Cells("ID").Value)


        txtuser.Text = row.Cells("Username").Value.ToString()
        txtpass.Text = row.Cells("Password").Value.ToString()
        txtemail.Text = row.Cells("Email").Value.ToString()

        Dim lrnValue As String = row.Cells("LRN").Value.ToString()
        Dim empNoValue As String = row.Cells("EmployeeNo").Value.ToString()


        If Not String.IsNullOrWhiteSpace(lrnValue) Then

            txtlrn.Text = lrnValue
            txtlrn.Enabled = True
            txtemployeeno.Text = ""
            txtemployeeno.Enabled = False
            originalIDValue = lrnValue
        ElseIf Not String.IsNullOrWhiteSpace(empNoValue) Then

            txtemployeeno.Text = empNoValue
            txtemployeeno.Enabled = True
            txtlrn.Text = ""
            txtlrn.Enabled = False
            originalIDValue = empNoValue
        Else

            txtlrn.Text = ""
            txtemployeeno.Text = ""
            txtlrn.Enabled = False
            txtemployeeno.Enabled = False
            originalIDValue = String.Empty
        End If


        txtemail_TextChanged(txtemail, New EventArgs())
    End Sub

    Private Function GetOriginalEmail(ByVal ID As Integer) As String
        Dim email As String = ""
        Dim query As String = "SELECT Email FROM `borroweredit_tbl` WHERE ID = @ID"

        Using con As New MySqlConnection(connectionString)
            Using cmd As New MySqlCommand(query, con)
                cmd.Parameters.AddWithValue("@ID", ID)
                Try
                    con.Open()
                    Dim result As Object = cmd.ExecuteScalar()
                    If result IsNot Nothing AndAlso result IsNot DBNull.Value Then
                        email = result.ToString()
                    End If
                Catch ex As Exception

                End Try
            End Using
        End Using
        Return email.Trim()
    End Function
    Private Sub btnedit_Click(sender As Object, e As EventArgs) Handles btnedit.Click

        If selectedBorrowerID = -1 Then
            MessageBox.Show("Please select an account from the table to edit.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If


        Dim newUsername As String = txtuser.Text.Trim()
        Dim newPassword As String = txtpass.Text
        Dim newEmail As String = txtemail.Text.Trim()
        Dim newIDValue As String
        Dim IDColumn As String

        If txtlrn.Enabled AndAlso Not String.IsNullOrWhiteSpace(txtlrn.Text) Then
            newIDValue = txtlrn.Text.Trim()
            IDColumn = "LRN"
        ElseIf txtemployeeno.Enabled AndAlso Not String.IsNullOrWhiteSpace(txtemployeeno.Text) Then
            newIDValue = txtemployeeno.Text.Trim()
            IDColumn = "EmployeeNo"
        Else
            MessageBox.Show("The ID field (LRN or Employee No.) cannot be empty.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End If

        If String.IsNullOrWhiteSpace(newUsername) OrElse String.IsNullOrWhiteSpace(newPassword) Then
            MessageBox.Show("Username and Password are required.", "Required Fields", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If


        If userexistsu(newUsername, selectedBorrowerID) Then
            MessageBox.Show("The username '" & newUsername & "' is already taken by another account.", "Duplicate Username", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If


        If newIDValue <> originalIDValue And idexistsu(newIDValue, IDColumn, selectedBorrowerID) Then
            MessageBox.Show("The entered " & IDColumn & " is already associated with another account.", "Duplicate ID", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If


        Dim coms As String = $"UPDATE `borroweredit_tbl` SET `Username` = @User, `Password` = @Pass, `Email` = @Email, `LRN` = CASE WHEN @IDColumn = 'LRN' THEN @NewID ELSE NULL END, `EmployeeNo` = CASE WHEN @IDColumn = 'EmployeeNo' THEN @NewID ELSE NULL END WHERE `ID` = @SelectedID"

        Dim originalEmail As String = GetOriginalEmail(selectedBorrowerID)

        Using con As New MySqlConnection(connectionString)
            Try
                con.Open()
                Using commandsu As New MySqlCommand(coms, con)
                    commandsu.Parameters.AddWithValue("@NewID", newIDValue)
                    commandsu.Parameters.AddWithValue("@User", newUsername)
                    commandsu.Parameters.AddWithValue("@Pass", newPassword)
                    commandsu.Parameters.AddWithValue("@Email", newEmail)
                    commandsu.Parameters.AddWithValue("@SelectedID", selectedBorrowerID)
                    commandsu.Parameters.AddWithValue("@IDColumn", IDColumn)

                    Dim rowsAffected As Integer = commandsu.ExecuteNonQuery()

                    If rowsAffected > 0 Then


                        If originalEmail.Equals(MainForm.lblgmail.Text.Trim(), StringComparison.OrdinalIgnoreCase) Then

                            MainForm.lblgmail.Text = newEmail

                        End If

                        MessageBox.Show("Borrower account updated successfully.", "Update Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
                        refresheditt()
                    Else
                        MessageBox.Show("Update failed or no changes were made.", "Update Failed", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    End If
                End Using
            Catch ex As MySqlException
                MessageBox.Show("Database Error during update: " & ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End Using

    End Sub


    Private Sub btnclear_Click(sender As Object, e As EventArgs) Handles btnclear.Click
        clearlahat()
    End Sub


    Private Sub PictureBox1_Click(sender As Object, e As EventArgs) Handles PictureBox1.Click
        If txtpass.PasswordChar = "•" Then
            PictureBox1.Image = Image.FromFile(Application.StartupPath & "\Resources\dilat.png")
            txtpass.PasswordChar = ""
        Else
            PictureBox1.Image = Image.FromFile(Application.StartupPath & "\Resources\pikit.png")
            txtpass.PasswordChar = "•"
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

    Private Sub txtpassword(sender As Object, e As EventArgs) Handles txtpass.TextChanged
        Dim Password As String = txtpass.Text
        If String.IsNullOrEmpty(Password) Then
            If Not lblpassword Is Nothing Then lblpassword.Visible = False
            Exit Sub
        Else
            If Not lblpassword Is Nothing Then lblpassword.Visible = True
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


    Private Function userexistsu(ByVal Username As String, ByVal CurrentID As Integer) As Boolean
        Dim exists As Boolean = False
        Dim sql As String = "SELECT 1 FROM `borroweredit_tbl` WHERE `Username` = @User AND `ID` <> @CurrentID LIMIT 1"

        Using con As New MySqlConnection(connectionString)
            Try
                con.Open()
                Using cmd As New MySqlCommand(sql, con)
                    cmd.Parameters.AddWithValue("@User", Username)
                    cmd.Parameters.AddWithValue("@CurrentID", CurrentID)

                    If cmd.ExecuteScalar() IsNot Nothing Then
                        exists = True
                    End If
                End Using
            Catch ex As MySqlException
                MessageBox.Show("A database error occurred during username check: " & ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End Using
        Return exists
    End Function


    Private Function idexistsu(ByVal IDValue As String, ByVal IDColumn As String, ByVal CurrentID As Integer) As Boolean
        Dim exists As Boolean = False
        Dim sql As String = $"SELECT 1 FROM `borroweredit_tbl` WHERE `{IDColumn}` = @ID AND `ID` <> @CurrentID LIMIT 1"

        Using con As New MySqlConnection(connectionString)
            Try
                con.Open()
                Using cmd As New MySqlCommand(sql, con)
                    cmd.Parameters.AddWithValue("@ID", IDValue)
                    cmd.Parameters.AddWithValue("@CurrentID", CurrentID)

                    If cmd.ExecuteScalar() IsNot Nothing Then
                        exists = True
                    End If
                End Using
            Catch ex As MySqlException
                MessageBox.Show("A database error occurred during ID check: " & ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End Using
        Return exists
    End Function

    Private Sub Borrowereditsinfo_KeyDown(sender As Object, e As KeyEventArgs) Handles MyBase.KeyDown
        If e.KeyCode = Keys.Escape Then
            Me.Close()
        End If
    End Sub

    Private Sub Borrowereditsinfo_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing


        MainForm.ProcessStripMenuItem.ShowDropDown()
        MainForm.ProcessStripMenuItem.ForeColor = Color.Gray
    End Sub
End Class