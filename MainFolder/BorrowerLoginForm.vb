Imports MySql.Data.MySqlClient
Imports System.Data
Imports System.Drawing

Public Class BorrowerLoginForm

    Private Sub BorrowerLoginForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        refreshbrwrlogin()
    End Sub


    Public Sub refreshbrwrlogin()

        'TopMost = True
        txtuser.Text = ""
        txtpass.Text = ""

        txtpass.PasswordChar = "•"

        If System.IO.File.Exists(Application.StartupPath & "\Resources\pikit.png") Then
            PictureBox2.Image = Image.FromFile(Application.StartupPath & "\Resources\pikit.png")
        End If
    End Sub


    Private Sub btnlogin_Click(sender As Object, e As EventArgs) Handles btnlogin.Click


        Dim activeMain As MainForm = GlobalVarsModule.ActiveMainForm

        If activeMain Is Nothing Then
            activeMain = Application.OpenForms.OfType(Of MainForm)().FirstOrDefault()
            If activeMain Is Nothing Then
                activeMain = New MainForm()
                GlobalVarsModule.ActiveMainForm = activeMain
                activeMain.Show()
                activeMain.Hide()
            End If
        End If


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

        Dim reader As MySqlDataReader = Nothing

        Try
            con.Open()
            reader = cmd.ExecuteReader()

            If reader.Read() Then

                Dim userEmail As String = reader("Email").ToString()
                Dim borrowerID As String = reader("BorrowerID").ToString()
                Dim borrowerType As String = reader("BorrowerType").ToString()

                GlobalVarsModule.CurrentUserRole = "Borrower"
                GlobalVarsModule.CurrentBorrowerType = borrowerType
                GlobalVarsModule.CurrentBorrowerID = borrowerID
                GlobalVarsModule.CurrentUserID = borrowerID
                GlobalVarsModule.GlobalRole = "Borrower"
                GlobalVarsModule.GlobalEmail = userEmail

                activeMain.SetupBorrowerUI(borrowerType)

                If borrowerType = "Unknown" Then
                    MessageBox.Show("Login successful, but borrower type (LRN or Employee No) is missing in the record. Please check your account details.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                End If

                reader.Close()

                If Not GlobalVarsModule.IsBorrowerStillTimedIn(borrowerID) Then

                    Dim lrn As Object = If(borrowerType = "Student", borrowerID, CType(DBNull.Value, Object))
                    Dim employeeNo As Object = If(borrowerType = "Teacher", borrowerID, CType(DBNull.Value, Object))

                    Dim comInsert As String = "INSERT INTO `oras_tbl` (`LRN`, `EmployeeNo`, `TimeIn`) VALUES (@lrn, @emp, NOW())"

                    Using cmdInsert As New MySqlCommand(comInsert, con)
                        cmdInsert.Parameters.AddWithValue("@lrn", lrn)
                        cmdInsert.Parameters.AddWithValue("@emp", employeeNo)

                        cmdInsert.ExecuteNonQuery()

                        MessageBox.Show($"Welcome, {Username}!", "Time In Complete", MessageBoxButtons.OK, MessageBoxIcon.Information)
                    End Using
                Else
                    Dim reminderMessage As String = $"Welcome back!, {Username}. You are currently Timed In," & Environment.NewLine &
                                            $"Kindly ensure that you (Time Out) before leaving the library premises."

                    MessageBox.Show(reminderMessage, "Time Out Reminder 🔔", MessageBoxButtons.OK, MessageBoxIcon.Information)
                End If

                Dim orasForm As oras = Application.OpenForms.OfType(Of oras)().FirstOrDefault()
                If orasForm IsNot Nothing Then
                    orasForm.ludeyngoras()
                End If

                activeMain.PrintReceiptToolStripMenuItem.Visible = False

                If activeMain.BorrowerEditsInfoForm Is Nothing Then
                    activeMain.BorrowerEditsInfoForm = New Borrowereditsinfo()
                End If

                activeMain.BorrowerEditsInfoForm.visibilitysus(borrowerType)

                activeMain.lbl_currentuser.Text = borrowerType
                activeMain.lblgmail.Text = userEmail
                activeMain.SetupBorrowerUI(borrowerType)

                If activeMain.MaintenanceToolStripMenuItem IsNot Nothing Then
                    activeMain.MaintenanceToolStripMenuItem.Visible = False
                    activeMain.btnlogoutt.Visible = False
                End If
                If activeMain.SettingsStripMenuItem IsNot Nothing Then
                    activeMain.SettingsStripMenuItem.Visible = False
                End If

                If activeMain.ProcessStripMenuItem IsNot Nothing Then
                    activeMain.ProcessStripMenuItem.Visible = True

                    For Each item As ToolStripItem In activeMain.ProcessStripMenuItem.DropDownItems
                        item.Visible = False
                    Next

                    If activeMain.StudentLogsToolStripMenuItem IsNot Nothing Then
                        activeMain.StudentLogsToolStripMenuItem.Visible = True
                        If activeMain.TimeInToolStripMenuItem IsNot Nothing Then
                            activeMain.TimeInToolStripMenuItem.Visible = True
                        End If
                    End If

                    If activeMain.EditsToolStripMenuItem1 IsNot Nothing Then
                        activeMain.EditsToolStripMenuItem1.Visible = True
                    End If

                    If activeMain.CirculationToolStripMenuItem IsNot Nothing Then
                        activeMain.CirculationToolStripMenuItem.Visible = True
                        For Each circItem As ToolStripItem In activeMain.CirculationToolStripMenuItem.DropDownItems
                            If circItem Is activeMain.BorrowToolStripMenuItem Then
                                circItem.Visible = True
                            Else
                                circItem.Visible = False
                            End If
                        Next
                    End If
                End If

                activeMain.Panel_dash.Controls.Clear()
                Dim borrowingForm As New Borrowing()
                borrowingForm.TopLevel = False
                borrowingForm.BringToFront()
                activeMain.Panel_dash.Controls.Add(borrowingForm)
                borrowingForm.SetupBorrowerFields()
                borrowingForm.Show()

                activeMain.lblform.Text = "BORROWING FORM"
                activeMain.Show()
                Me.Hide()
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
            If reader IsNot Nothing AndAlso Not reader.IsClosed Then
                reader.Close()
            End If
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