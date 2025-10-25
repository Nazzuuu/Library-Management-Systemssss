Imports System.IO
Imports System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel
Imports Guna.UI2.WinForms
Imports MySql.Data.MySqlClient

Public Class login

    'shettt, kapagoddddddd'''''

    Public Sub clear()
        txtpass.Text = ""
        txtuser.Text = ""
    End Sub

    Private Sub btnlogin_Click(sender As Object, e As EventArgs) Handles btnlogin.Click

        For i As Integer = Application.OpenForms.Count - 1 To 0 Step -1
            Dim formInApp As Form = Application.OpenForms(i)
            If formInApp IsNot Me AndAlso formInApp.Name <> "MainForm" Then
                formInApp.Hide()
            End If
        Next

        Dim User As String = txtuser.Text.Trim()
        Dim Pass As String = txtpass.Text.Trim()

        If User = "" OrElse Pass = "" Then
            MessageBox.Show("Please enter both Username and Password.", "Login Required", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        End If

        Using con As New MySqlConnection(GlobalVarsModule.connectionString)
            Dim comsus As String =
            "SELECT 0 AS ID, Username, Password, Email, 'Librarian' AS Role FROM superadmin_tbl WHERE Username = @username AND Password = @password " &
            "UNION " &
            "SELECT ID, Username, Password, Email, Role FROM user_staff_tbl WHERE Username = @username AND Password = @password"

            Using com As New MySqlCommand(comsus, con)
                com.Parameters.AddWithValue("@username", User)
                com.Parameters.AddWithValue("@password", Pass)

                Try
                    con.Open()
                    Using lahatngrole As MySqlDataReader = com.ExecuteReader()
                        If lahatngrole.Read() Then
                            Dim role As String = lahatngrole("Role").ToString()
                            Dim userEmail As String = lahatngrole("Email").ToString()
                            Dim employeeID As String = lahatngrole("ID").ToString()


                            GlobalVarsModule.GlobalRole = role
                            GlobalVarsModule.GlobalUsername = User
                            GlobalVarsModule.GlobalEmail = userEmail
                            GlobalVarsModule.CurrentUserID = employeeID
                            GlobalVarsModule.CurrentEmployeeID = employeeID


                            GlobalVarsModule.LogAudit(
                            actionType:="LOGIN SUCCESS",
                            formName:="LOGIN FORM",
                            description:=$"User '{User}' ({role}) successfully logged in."
                        )


                            MessageBox.Show($"Welcome, {User}! You are logged in as {role}.", "Login Successful", MessageBoxButtons.OK, MessageBoxIcon.Information)

                            Dim activeMain As MainForm = GlobalVarsModule.ActiveMainForm
                            If activeMain Is Nothing OrElse activeMain.IsDisposed Then
                                activeMain = New MainForm()
                                GlobalVarsModule.ActiveMainForm = activeMain
                                activeMain.Show()
                            End If


                            activeMain.ResetToMainDashboard()
                            activeMain.lblgmail.Text = userEmail
                            activeMain.lblform.Text = "MAIN FORM"


                            Select Case role
                                Case "Librarian"
                                    activeMain.lbl_currentuser.Text = "Librarian"

                                    activeMain.AcquisitionToolStripMenuItem.Visible = True
                                    activeMain.AccessionToolStripMenuItem.Visible = True
                                    activeMain.UserMaintenanceToolStripMenuItem.Visible = True
                                    activeMain.Audit_Trail.Visible = True
                                    activeMain.EditInfoToolStripMenuItem.Visible = True
                                    activeMain.EditsToolStripMenuItem1.Visible = False
                                    activeMain.BorrowToolStripMenuItem.Visible = True
                                    activeMain.StudentLogsToolStripMenuItem.Visible = True
                                    activeMain.Panel_Studentlogs.Visible = True
                                    activeMain.PenaltyManagementToolStripMenuItem.Visible = True
                                    activeMain.PenaltyToolStripMenuItem.Visible = True

                                Case "Staff"
                                    activeMain.lbl_currentuser.Text = "Staff"

                                    activeMain.AcquisitionToolStripMenuItem.Visible = False
                                    activeMain.AccessionToolStripMenuItem.Visible = False
                                    activeMain.Audit_Trail.Visible = False
                                    activeMain.EditInfoToolStripMenuItem.Visible = False
                                    activeMain.EditsToolStripMenuItem1.Visible = False
                                    activeMain.BorrowToolStripMenuItem.Visible = False
                                    activeMain.PenaltyToolStripMenuItem.Visible = False
                                    activeMain.PenaltyManagementToolStripMenuItem.Visible = False

                                Case "Assistant Librarian"
                                    activeMain.lbl_currentuser.Text = "Asst. Librarian"

                                    activeMain.Audit_Trail.Visible = False
                                    activeMain.EditInfoToolStripMenuItem.Visible = False
                                    activeMain.EditsToolStripMenuItem1.Visible = False
                                    activeMain.BorrowToolStripMenuItem.Visible = False
                                    activeMain.AcquisitionToolStripMenuItem.Visible = False
                                    activeMain.AccessionToolStripMenuItem.Visible = False

                                    For Each form In Application.OpenForms
                                        If TypeOf form Is Penalty Then
                                            DirectCast(form, Penalty).refreshpenalty()
                                        End If
                                    Next
                                Case Else
                                    MessageBox.Show("Invalid role detected.", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Error)
                                    Exit Sub
                            End Select


                            For Each formInApp As Form In Application.OpenForms
                                If TypeOf formInApp Is Users Then
                                    DirectCast(formInApp, Users).LoadUserData()
                                End If
                            Next

                            Me.Hide()
                            clear()

                        Else

                            GlobalVarsModule.GlobalRole = ""
                            GlobalVarsModule.GlobalUsername = ""
                            GlobalVarsModule.GlobalEmail = ""
                            GlobalVarsModule.CurrentUserID = ""
                            GlobalVarsModule.CurrentEmployeeID = ""

                            MessageBox.Show("Invalid Credentials.", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Error)
                        End If
                    End Using
                Catch ex As Exception
                    MessageBox.Show("Login Error: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End Try
            End Using
        End Using
    End Sub



    Private Sub login_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        TopMost = True
        MainForm.Refresh()

        txtpass.PasswordChar = "•"


        Try

            Dim filePath As String = Application.StartupPath & "\Resources\pikit.png"
            If File.Exists(filePath) Then
                Using fs As New FileStream(filePath, FileMode.Open, FileAccess.Read)
                    PictureBox1.Image = New Bitmap(fs)
                End Using
            Else

                MessageBox.Show("Image file not found: pikit.png", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End If
        Catch ex As Exception
            MessageBox.Show("Error loading pikit.png: " & ex.Message, "Image Load Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try


        Dim con As New MySqlConnection(GlobalVarsModule.connectionString)
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


    Private Sub PictureBox1_MouseHover_1(sender As Object, e As EventArgs) Handles PictureBox1.MouseHover

        Cursor = Cursors.Hand

    End Sub

    Private Sub PictureBox1_MouseLeave_1(sender As Object, e As EventArgs) Handles PictureBox1.MouseLeave

        Cursor = Cursors.Default

    End Sub

    Private Sub LinkLabel1_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles LinkLabel1.LinkClicked
        ServerConnection.Show()

    End Sub

    Private Sub lblborrowerloginform_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles lblborrowerloginform.LinkClicked
        Me.Hide()

        Dim activeMain As MainForm = GlobalVarsModule.ActiveMainForm
        If activeMain IsNot Nothing Then
            activeMain.Hide()
        End If

        Dim borrowerLogin As BorrowerLoginForm = Application.OpenForms.OfType(Of BorrowerLoginForm)().FirstOrDefault()
        If borrowerLogin Is Nothing Then
            borrowerLogin = New BorrowerLoginForm()
            GlobalVarsModule.ActiveBorrowerLoginForm = borrowerLogin
        End If

        borrowerLogin.Show()

    End Sub
End Class