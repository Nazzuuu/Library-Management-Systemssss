Imports MySql.Data.MySqlClient
Imports System.Data
Imports System.Drawing

Public Class BorrowerLoginForm

    Private Sub BorrowerLoginForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        refreshbrwrlogin()
    End Sub


    Public Sub refreshbrwrlogin()

        TopMost = True
        txtuser.Text = ""
        txtpass.Text = ""

        txtpass.PasswordChar = "•"

        If System.IO.File.Exists(Application.StartupPath & "\Resources\pikit.png") Then
            PictureBox2.Image = Image.FromFile(Application.StartupPath & "\Resources\pikit.png")
        End If
    End Sub


    Private Sub btnlogin_Click(sender As Object, e As EventArgs) Handles btnlogin.Click

        Dim Username As String = txtuser.Text.Trim()
        Dim Password As String = txtpass.Text

        If String.IsNullOrWhiteSpace(Username) OrElse String.IsNullOrWhiteSpace(Password) Then
            MessageBox.Show("Please enter both Username and Password.", "Required Fields", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If


        Dim con As New MySqlConnection(GlobalVarsModule.connectionString)


        Dim com As String = "SELECT `Username`, `Password`, `Email`, " &
                            "    CASE " &
                            "        WHEN `LRN` IS NOT NULL AND `LRN` <> '0' AND `LRN` <> '' THEN 'Student' " &
                            "        WHEN `EmployeeNo` IS NOT NULL AND `EmployeeNo` <> '0' AND `EmployeeNo` <> '' THEN 'Teacher' " &
                            "        ELSE 'Unknown' " &
                            "    END AS BorrowerType, " &
                            "    IFNULL(`LRN`, `EmployeeNo`) AS BorrowerID " &
                            "FROM `borroweredit_tbl` " &
                            "WHERE `Username` = @User AND `Password` = @Pass LIMIT 1"

        Dim cmd As New MySqlCommand(com, con)
        cmd.Parameters.AddWithValue("@User", Username)
        cmd.Parameters.AddWithValue("@Pass", Password)

        Try
            con.Open()
            Dim reader As MySqlDataReader = cmd.ExecuteReader()
            If reader.Read() Then

                Dim userEmail As String = reader("Email").ToString()
                Dim borrowerID As String = reader("BorrowerID").ToString()
                Dim borrowerType As String = reader("BorrowerType").ToString()


                GlobalVarsModule.CurrentUserRole = "Borrower"
                GlobalVarsModule.CurrentBorrowerType = borrowerType
                GlobalVarsModule.CurrentBorrowerID = borrowerID


                GlobalVarsModule.CurrentUserID = borrowerID
                MainForm.SetupBorrowerUI(borrowerType)


                If borrowerType = "Unknown" Then
                    MessageBox.Show("Login successful, but borrower type (LRN or Employee No) is missing in the record. Please check your account details.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                End If

                MessageBox.Show("Borrower successfully logged in. Welcome, " & Username & "!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information)
                MainForm.PrintReceiptToolStripMenuItem.Visible = False
                MainForm.SetupBorrowerUI(borrowerType)
                MainForm.Show()
                Me.Hide()


                GlobalVarsModule.CurrentUserRole = "Borrower"
                GlobalVarsModule.CurrentBorrowerType = borrowerType
                GlobalVarsModule.CurrentBorrowerID = borrowerID


                GlobalVarsModule.CurrentUserID = borrowerID


                If MainForm.BorrowerEditsInfoForm Is Nothing Then
                    MainForm.BorrowerEditsInfoForm = New Borrowereditsinfo()
                End If


                MainForm.BorrowerEditsInfoForm.visibilitysus(borrowerType)

                MainForm.lbl_currentuser.Text = borrowerType
                MainForm.lblgmail.Text = userEmail
                MainForm.lblform.Text = "BORROWING FORM"


                If MainForm.MaintenanceToolStripMenuItem IsNot Nothing Then
                    MainForm.MaintenanceToolStripMenuItem.Visible = False
                End If
                If MainForm.SettingsStripMenuItem IsNot Nothing Then
                    MainForm.SettingsStripMenuItem.Visible = False
                End If


                If MainForm.ProcessStripMenuItem IsNot Nothing Then
                    MainForm.ProcessStripMenuItem.Visible = True

                    For Each item As ToolStripItem In MainForm.ProcessStripMenuItem.DropDownItems
                        item.Visible = False
                    Next

                    If MainForm.StudentLogsToolStripMenuItem IsNot Nothing Then
                        MainForm.StudentLogsToolStripMenuItem.Visible = True
                        If MainForm.TimeInToolStripMenuItem IsNot Nothing Then
                            MainForm.TimeInToolStripMenuItem.Visible = True
                        End If
                    End If

                    If MainForm.EditsToolStripMenuItem1 IsNot Nothing Then
                        MainForm.EditsToolStripMenuItem1.Visible = True
                    End If

                    If MainForm.CirculationToolStripMenuItem IsNot Nothing Then
                        MainForm.CirculationToolStripMenuItem.Visible = True
                        For Each circItem As ToolStripItem In MainForm.CirculationToolStripMenuItem.DropDownItems
                            If circItem Is MainForm.BorrowToolStripMenuItem Then
                                circItem.Visible = True
                            Else
                                circItem.Visible = False
                            End If
                        Next
                    End If
                End If

                If MainForm.btnlogoutt IsNot Nothing Then
                    MainForm.btnlogoutt.Visible = True
                End If


                MainForm.Panel_dash.Controls.Clear()

                If MainForm.dshboard IsNot Nothing Then

                    MainForm.Panel_dash.Controls.Add(MainForm.dshboard)
                    MainForm.dshboard.BringToFront()
                    MainForm.dshboard.Visible = True

                End If

                If MainForm.Panel_User IsNot Nothing Then

                    MainForm.Panel_dash.Controls.Add(MainForm.Panel_User)
                    MainForm.Panel_User.BringToFront()
                    MainForm.Panel_User.Visible = True
                End If

                If MainForm.Panel_welcome IsNot Nothing Then

                    MainForm.Panel_dash.Controls.Add(MainForm.Panel_welcome)
                    MainForm.Panel_welcome.BringToFront()
                    MainForm.Panel_welcome.Visible = True
                End If


                MainForm.lblform.Text = "MAIN FORM"



                refreshbrwrlogin()

            Else

                MessageBox.Show("Invalid Username or Password.", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Error)
                txtpass.Clear()
            End If

        Catch ex As MySqlException
            MessageBox.Show("Database Error: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Catch ex As Exception

            If ex.Message.Contains("MdiParent") Then
                MessageBox.Show("An unexpected error occurred: Form that was specified to be the MdiParent for this form is not an MdiContainer. (Parameter 'value')" & Environment.NewLine & "Please ensure the 'Borrowing' form is configured correctly to load inside MainForm's Panel_dash.", "Form Load Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Else
                MessageBox.Show("An unexpected error occurred: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End If
        Finally
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
        End Try

    End Sub



    Private Sub LinkLabel1_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles LinkLabel1.LinkClicked
        BorrowerCreateAccount.Show()
    End Sub

    Private Sub btnclose_Click(sender As Object, e As EventArgs) Handles btnclose.Click
        Application.Exit()

    End Sub


    Private Sub PictureBox2_Click(sender As Object, e As EventArgs) Handles PictureBox2.Click

        If txtpass.PasswordChar = "•" Then
            If System.IO.File.Exists(Application.StartupPath & "\Resources\dilat.png") Then
                PictureBox2.Image = Image.FromFile(Application.StartupPath & "\Resources\dilat.png")
            End If
            txtpass.PasswordChar = ""

        Else
            If System.IO.File.Exists(Application.StartupPath & "\Resources\pikit.png") Then
                PictureBox2.Image = Image.FromFile(Application.StartupPath & "\Resources\pikit.png")
            End If
            txtpass.PasswordChar = "•"
        End If
    End Sub

    Private Sub txtpass_KeyDown(sender As Object, e As KeyEventArgs) Handles txtpass.KeyDown
        If e.KeyCode = Keys.Enter Then
            btnlogin_Click(sender, e)
            e.Handled = True
        End If
    End Sub

End Class