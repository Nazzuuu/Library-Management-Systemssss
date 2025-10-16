Imports System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel
Imports MySql.Data.MySqlClient
Public Class login

    Public Sub clear()
        txtpass.Text = ""
        txtuser.Text = ""

    End Sub


    Private Sub btnlogin_Click(sender As Object, e As EventArgs) Handles btnlogin.Click

        Dim User As String = txtuser.Text.Trim()
        Dim Pass As String = txtpass.Text.Trim()

        Dim con As New MySqlConnection(connectionString)


        Dim comsus As String = "SELECT Username, Password, Email, 'Librarian' AS Role FROM superadmin_tbl WHERE Username = @username AND Password = @password " &
            "UNION " &
            "SELECT Username, Password, Email, Role FROM user_staff_tbl WHERE Username = @username AND Password = @password"

        Dim com As New MySqlCommand(comsus, con)
        com.Parameters.AddWithValue("@username", User)
        com.Parameters.AddWithValue("@password", Pass)

        Try
            con.Open()
            Dim lahatngrole As MySqlDataReader = com.ExecuteReader()

            If lahatngrole.Read() Then
                Dim role As String = lahatngrole("Role").ToString()
                Dim userEmail As String = lahatngrole("Email").ToString()


                GlobalRole = role
                GlobalUsername = userEmail


                MainForm.AcquisitionToolStripMenuItem.Visible = True
                MainForm.AccessionToolStripMenuItem.Visible = True
                MainForm.UserMaintenanceToolStripMenuItem.Visible = True
                MainForm.Audit_Trail.Visible = True
                MainForm.EditInfoToolStripMenuItem.Visible = True
                MainForm.EditsToolStripMenuItem1.Visible = True
                MainForm.BorrowToolStripMenuItem.Visible = True
                MainForm.StudentLogsToolStripMenuItem.Visible = True
                MainForm.Panel_Studentlogs.Visible = True
                MainForm.PenaltyManagementToolStripMenuItem.Visible = True
                MainForm.PenaltyToolStripMenuItem.Visible = True


                If role = "Librarian" Then


                    MainForm.Refresh()
                    MainForm.StudentLogsToolStripMenuItem.Visible = False
                    MainForm.EditsToolStripMenuItem1.Visible = False
                    MainForm.BorrowToolStripMenuItem.Visible = False

                    MainForm.lbl_currentuser.Text = "Librarian"
                    MainForm.lblgmail.Text = userEmail
                    MainForm.lblform.Text = "MAIN FORM"


                    MessageBox.Show("Librarian successfully logged in.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information)
                    MainForm.Show()

                    Me.Hide()
                    clear()


                ElseIf role = "Staff" Then

                    MainForm.Refresh()

                    MainForm.AcquisitionToolStripMenuItem.Visible = False
                    MainForm.AccessionToolStripMenuItem.Visible = False
                    MainForm.UserMaintenanceToolStripMenuItem.Visible = False
                    MainForm.Audit_Trail.Visible = False
                    MainForm.EditInfoToolStripMenuItem.Visible = False
                    MainForm.EditsToolStripMenuItem1.Visible = False
                    MainForm.BorrowToolStripMenuItem.Visible = False

                    MainForm.Audit_Trail.Visible = False
                    MainForm.PenaltyToolStripMenuItem.Visible = False
                    MainForm.PenaltyManagementToolStripMenuItem.Visible = False
                    MainForm.StudentLogsToolStripMenuItem.Visible = False
                    MainForm.EditsToolStripMenuItem1.Visible = False
                    MainForm.BorrowToolStripMenuItem.Visible = False

                    MessageBox.Show("Staff successfully logged in.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information)
                    MainForm.Show()
                    MainForm.lbl_currentuser.Text = "Staff"




                    MainForm.lblgmail.Text = userEmail
                    MainForm.lblform.Text = "MAIN FORM"
                    Me.Hide()
                    clear()

                ElseIf role = "Assistant Librarian" Then


                    MainForm.Refresh()

                    MainForm.UserMaintenanceToolStripMenuItem.Visible = False
                    MainForm.Audit_Trail.Visible = False
                    MainForm.EditInfoToolStripMenuItem.Visible = False
                    MainForm.EditsToolStripMenuItem1.Visible = False
                    MainForm.BorrowToolStripMenuItem.Visible = False
                    MainForm.StudentLogsToolStripMenuItem.Visible = False
                    MainForm.EditsToolStripMenuItem1.Visible = False
                    MainForm.BorrowToolStripMenuItem.Visible = False

                    MessageBox.Show("Assistant Librarian successfully logged in.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information)
                    MainForm.Show()
                    MainForm.lbl_currentuser.Text = "Asst. Librarian"




                    MainForm.lblgmail.Text = userEmail
                    MainForm.lblform.Text = "MAIN FORM"
                    Me.Hide()
                    clear()

                Else
                    MessageBox.Show("Invalid role.", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End If
            Else

                GlobalRole = ""
                GlobalUsername = ""
                MessageBox.Show("Invalid Credentials.", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End If

        Catch ex As Exception
            MsgBox(ex.Message, vbCritical)
        Finally
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
        End Try
    End Sub


    Private Sub login_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        TopMost = True
        Koneksyon()


        MainForm.Refresh()

        txtpass.PasswordChar = "•"
        PictureBox1.Image = Image.FromFile(Application.StartupPath & "\Resources\pikit.png")

        Dim con As New MySqlConnection(connectionString)
        Dim com As New MySqlCommand("SELECT COUNT(*) FROM superadmin_tbl WHERE Role = 'Librarian'", con)

        Try
            con.Open()
            Dim count As Integer = CInt(com.ExecuteScalar())

            If count = 0 Then

                MessageBox.Show("No Librarian account found. Please create one.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Superadmin.ShowDialog()

            End If
        Catch ex As Exception
            MessageBox.Show("Errort: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
        End Try


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

    Private Sub txtpass_KeyDown(sender As Object, e As KeyEventArgs) Handles txtpass.KeyDown
        If e.KeyCode = Keys.Enter Then
            btnlogin_Click(sender, e)
            e.Handled = True
        End If
    End Sub

    Private Sub btnlogin_KeyDown(sender As Object, e As KeyEventArgs) Handles btnlogin.KeyDown

        If e.KeyCode = Keys.Enter Then
            btnlogin_Click(sender, e)
            e.Handled = True
        End If

    End Sub

    Private Sub btnlogin_MouseHover(sender As Object, e As EventArgs) Handles btnlogin.MouseHover

        Cursor = Cursors.Hand

    End Sub

    Private Sub btnlogin_MouseLeave(sender As Object, e As EventArgs) Handles btnlogin.MouseLeave

        Cursor = Cursors.Default

    End Sub

    Private Sub Guna2ControlBox1_Click(sender As Object, e As EventArgs) Handles Guna2ControlBox1.Click
        Me.Close()
    End Sub

    Private Sub PictureBox1_MouseHover_1(sender As Object, e As EventArgs) Handles PictureBox1.MouseHover

        Cursor = Cursors.Hand

    End Sub

    Private Sub PictureBox1_MouseLeave_1(sender As Object, e As EventArgs) Handles PictureBox1.MouseLeave

        Cursor = Cursors.Default

    End Sub
End Class