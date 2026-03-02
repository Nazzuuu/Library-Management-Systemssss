Imports System.Data.SqlClient
Imports System.Net
Imports System.Net.Mail
Imports MySql.Data.MySqlClient

Public Class ResetPassword1

    Public Shared resetCode As String
    Public Shared userTable As String
    Public Shared userIdColumn As String
    Public Shared userEmail As String
    Public Shared userIdentifierValue As String

    Dim con As New MySqlConnection(connectionString)

    Private Sub btnSendCode_Click(sender As Object, e As EventArgs) Handles btnreset.Click

        If txtenter1.Text.Trim = "" Then
            MessageBox.Show("Please enter your LRN / Employee No / Email.")
            Exit Sub
        End If

        Dim identifier As String = txtenter1.Text.Trim
        Dim cmd As MySqlCommand
        Dim reader As MySqlDataReader

        Try
            con.Open()

            If identifier.Contains("@") Then


                cmd = New MySqlCommand("SELECT LRN, email FROM borroweredit_tbl WHERE email=@id", con)
                cmd.Parameters.AddWithValue("@id", identifier)
                reader = cmd.ExecuteReader()

                If reader.Read() Then
                    userTable = "borroweredit_tbl"
                    userIdColumn = "LRN"
                    userEmail = reader("email").ToString()
                    userIdentifierValue = reader("LRN").ToString()
                    reader.Close()
                    GoTo SENDCODE
                End If
                reader.Close()


                cmd = New MySqlCommand("SELECT email FROM user_staff_tbl WHERE email=@id", con)
                cmd.Parameters.AddWithValue("@id", identifier)
                reader = cmd.ExecuteReader()

                If reader.Read() Then
                    userTable = "user_staff_tbl"
                    userIdColumn = "email"
                    userEmail = reader("email").ToString()
                    userIdentifierValue = identifier
                    reader.Close()
                    GoTo SENDCODE
                End If
                reader.Close()

                cmd = New MySqlCommand("SELECT email FROM superadmin_tbl WHERE email=@id", con)
                cmd.Parameters.AddWithValue("@id", identifier)
                reader = cmd.ExecuteReader()

                If reader.Read() Then
                    userTable = "superadmin_tbl"
                    userIdColumn = "email"
                    userEmail = reader("email").ToString()
                    userIdentifierValue = identifier
                    reader.Close()
                    GoTo SENDCODE
                End If
                reader.Close()

            Else

                cmd = New MySqlCommand("SELECT LRN, email FROM borroweredit_tbl WHERE LRN=@id", con)
                cmd.Parameters.AddWithValue("@id", identifier)
                reader = cmd.ExecuteReader()

                If reader.Read() Then
                    userTable = "borroweredit_tbl"
                    userIdColumn = "LRN"
                    userEmail = reader("email").ToString()
                    userIdentifierValue = identifier
                    reader.Close()
                    GoTo SENDCODE
                End If
                reader.Close()

                cmd = New MySqlCommand("SELECT EmployeeNo, email FROM borroweredit_tbl WHERE EmployeeNo=@id", con)
                cmd.Parameters.AddWithValue("@id", identifier)
                reader = cmd.ExecuteReader()

                If reader.Read() Then
                    userTable = "borroweredit_tbl"
                    userIdColumn = "EmployeeNo"
                    userEmail = reader("email").ToString()
                    userIdentifierValue = identifier
                    reader.Close()
                    GoTo SENDCODE
                End If
                reader.Close()

            End If

            MessageBox.Show("Account not found.")
            con.Close()
            Exit Sub

SENDCODE:

            resetCode = New Random().Next(100000, 999999).ToString()

            Dim smtp As New SmtpClient("smtp.gmail.com")
            smtp.Port = 587
            smtp.Credentials = New NetworkCredential("mdalms.bsit01@gmail.com", "sdsx acwh twht jnba")
            smtp.EnableSsl = True

            Dim mail As New MailMessage()
            mail.From = New MailAddress("mdalms.bsit01@gmail.com")
            mail.To.Add(userEmail)
            mail.Subject = "Password Reset Code"
            mail.Body = "Your 6-digit reset code is: " & resetCode

            smtp.Send(mail)

            MessageBox.Show("Reset code sent to your email.", "Successful", MessageBoxButtons.OK, MessageBoxIcon.Information)

            con.Close()

            ResetPassword2.Show()
            Me.Hide()

        Catch ex As Exception
            MessageBox.Show("Error: " & ex.Message)
            con.Close()
        End Try

    End Sub

End Class