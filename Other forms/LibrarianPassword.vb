Imports System.IO
Imports MySql.Data.MySqlClient
Public Class LibrarianPassword
    Private Sub Label1_Click(sender As Object, e As EventArgs) Handles Label1.Click

    End Sub

    Private Sub Password_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        txtpassword.PasswordChar = "•"
        Try
            Dim filePath As String = Application.StartupPath & "\Resources\pikit.png"
            If File.Exists(filePath) Then
                Using fs As New FileStream(filePath, FileMode.Open, FileAccess.Read)
                    PictureBox2.Image = New Bitmap(fs)
                End Using
            Else
                MessageBox.Show("Image file not found: pikit.png sa Users_Staffs.", "Image Load Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End If
        Catch ex As Exception
            MessageBox.Show("Error loading pikit.png for Users_Staffs Form: " & ex.Message, "Image Load Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

    End Sub

    Private Sub PictureBox2_Click(sender As Object, e As EventArgs) Handles PictureBox2.Click

        If txtpassword.PasswordChar = "•" Then

            Try
                Dim filePath As String = Application.StartupPath & "\Resources\dilat.png"
                If File.Exists(filePath) Then
                    Using fs As New FileStream(filePath, FileMode.Open, FileAccess.Read)
                        PictureBox2.Image = New Bitmap(fs)
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
                        PictureBox2.Image = New Bitmap(fs)
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

    Private Sub txtpassword_KeyDown(sender As Object, e As KeyEventArgs) Handles txtpassword.KeyDown
        If e.KeyCode = Keys.Enter Then
            e.SuppressKeyPress = True
            ValidateExitPassword()
        End If
    End Sub


    Private Sub ValidateExitPassword()
        Dim enteredPassword As String = txtpassword.Text.Trim()

        If String.IsNullOrWhiteSpace(enteredPassword) Then
            MessageBox.Show("Please enter the Librarian's password.", "Required", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Using con As New MySqlConnection(GlobalVarsModule.connectionString)

            Dim query As String = "SELECT COUNT(*) FROM superadmin_tbl WHERE Password = @password AND (Role = 'Librarian' OR Role = 'Superadmin')"

            Using cmd As New MySqlCommand(query, con)
                cmd.Parameters.AddWithValue("@password", enteredPassword)

                Try
                    con.Open()
                    Dim count As Integer = CInt(cmd.ExecuteScalar())

                    If count > 0 Then

                        Me.DialogResult = DialogResult.OK
                        Me.Close()
                    Else

                        MessageBox.Show("Invalid password.", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Stop)
                        txtpassword.Clear()
                        txtpassword.Focus()
                    End If

                Catch ex As Exception
                    MessageBox.Show("Database Error: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End Try
            End Using
        End Using
    End Sub

    Private Sub Password_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing

        login.Show()
        Me.Hide()

    End Sub

    Private Sub PictureBox1_MouseHover(sender As Object, e As EventArgs) Handles PictureBox2.MouseHover
        Cursor = Cursors.Hand
    End Sub

    Private Sub PictureBox1_MouseLeave(sender As Object, e As EventArgs) Handles PictureBox2.MouseLeave
        Cursor = Cursors.Default
    End Sub

End Class