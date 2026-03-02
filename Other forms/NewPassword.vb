Imports System.Data.SqlClient
Imports System.IO
Imports MySql.Data.MySqlClient

Public Class NewPassword

    Dim con As New MySqlConnection(GlobalVarsModule.connectionString)

    Private Sub btnReset_Click(sender As Object, e As EventArgs) Handles btnreset.Click

        If txtpass.Text.Length < 6 Then
            MessageBox.Show("Password must be at least 6 characters.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        End If

        If txtpass.Text <> txtconfirm.Text Then
            MessageBox.Show("Passwords do not match.")
            Exit Sub
        End If

        Try
            con.Open()

            Dim query As String = "UPDATE " & ResetPassword1.userTable &
                                  " SET password=@pass WHERE " &
                                  ResetPassword1.userIdColumn & "=@id"

            Dim cmd As New MySqlCommand(query, con)
            cmd.Parameters.AddWithValue("@pass", txtpass.Text)
            cmd.Parameters.AddWithValue("@id", ResetPassword1.userIdentifierValue)

            cmd.ExecuteNonQuery()

            con.Close()

            MessageBox.Show("Password reset successful.", "Successful", MessageBoxButtons.OK, MessageBoxIcon.Information)

            Dim frm As New login
            frm.Show()
            Me.Close()

        Catch ex As Exception
            MessageBox.Show("Error: " & ex.Message)
            con.Close()
        End Try

    End Sub

    Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnback.Click

        Dim frm As New login
        frm.Show()
        Me.Close()

    End Sub

    Private Sub PictureBox2_Click(sender As Object, e As EventArgs) Handles PictureBox2.Click

        If txtpass.PasswordChar = "•" Then

            Try
                Dim filePath As String = Application.StartupPath & "\Resources\dilat.png"
                If File.Exists(filePath) Then
                    Using fs As New FileStream(filePath, FileMode.Open, FileAccess.Read)
                        PictureBox2.Image = New Bitmap(fs)
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
                        PictureBox2.Image = New Bitmap(fs)
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

    Private Sub NewPassword_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        txtpass.PasswordChar = "•"
        txtconfirm.PasswordChar = "•"
        Try
            Dim filePath As String = Application.StartupPath & "\Resources\pikit.png"
            If File.Exists(filePath) Then
                Using fs As New FileStream(filePath, FileMode.Open, FileAccess.Read)
                    PictureBox2.Image = New Bitmap(fs)
                    PictureBox1.Image = New Bitmap(fs)
                End Using
            Else
                MessageBox.Show("Image file not found: pikit.png sa Users_Staffs.", "Image Load Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End If
        Catch ex As Exception
            MessageBox.Show("Error loading pikit.png for Users_Staffs Form: " & ex.Message, "Image Load Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub PictureBox1_Click(sender As Object, e As EventArgs) Handles PictureBox1.Click

        If txtconfirm.PasswordChar = "•" Then

            Try
                Dim filePath As String = Application.StartupPath & "\Resources\dilat.png"
                If File.Exists(filePath) Then
                    Using fs As New FileStream(filePath, FileMode.Open, FileAccess.Read)
                        PictureBox1.Image = New Bitmap(fs)
                    End Using
                    txtconfirm.PasswordChar = ""
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
                    txtconfirm.PasswordChar = "•"
                Else
                    MessageBox.Show("Image file not found: pikit.png", "Image Load Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End If
            Catch ex As Exception
                MessageBox.Show("Error loading 'pikit.png': " & ex.Message, "Image Load Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End If

    End Sub
End Class