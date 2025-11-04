Imports System.Data
Imports System.Drawing
Imports System.IO
Imports MySql.Data.MySqlClient

Public Class BorrowerLoginForm
    Private Sub BorrowerLoginForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        refreshbrwrlogin()
    End Sub

    Public Sub refreshbrwrlogin()

        txtuser.Text = ""
        txtpass.Text = ""

        txtpass.PasswordChar = "•"

        Dim filePath As String = Application.StartupPath & "\Resources\pikit.png"

        If File.Exists(filePath) Then
            Try

                Using fs As New FileStream(filePath, FileMode.Open, FileAccess.Read)

                    PictureBox2.Image = New Bitmap(fs)
                End Using
            Catch ex As Exception

                MessageBox.Show("Error loading pikit.png for Borrower Login: " & ex.Message,
                                "Image Load Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End If
    End Sub


    Private Sub btnlogin_Click(sender As Object, e As EventArgs) Handles btnlogin.Click
        'TopMost = True
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

        Dim currentDeviceIP As String = GlobalVarsModule.GetLocalIPAddress()

        Using con As New MySqlConnection(GlobalVarsModule.connectionString)
            Dim com As String =
            "SELECT Username, Password, Email, " &
            "CASE WHEN LRN IS NOT NULL AND LRN <> '0' AND LRN <> '' THEN 'Student' " &
            "     WHEN EmployeeNo IS NOT NULL AND EmployeeNo <> '0' AND EmployeeNo <> '' THEN 'Teacher' " &
            "     ELSE 'Unknown' END AS BorrowerType, " &
            "IFNULL(LRN, EmployeeNo) AS BorrowerID, " &
            "IFNULL(CurrentIP, '0.0.0.0') AS CurrentIP, " &
            "IFNULL(is_logged_in, 0) AS is_logged_in " &
            "FROM borroweredit_tbl " &
            "WHERE Username=@User AND Password=@Pass LIMIT 1"

            Using cmd As New MySqlCommand(com, con)
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
                        Dim activeIP As String = reader("CurrentIP").ToString().Trim()
                        Dim isLoggedInStatus As Integer = Convert.ToInt32(reader("is_logged_in"))


                        If isLoggedInStatus = 1 AndAlso activeIP <> currentDeviceIP Then
                            MessageBox.Show($"Account '{Username}' is already logged in on another device (IP: {activeIP}).", "Login Blocked", MessageBoxButtons.OK, MessageBoxIcon.Stop)
                            Exit Sub
                        End If


                        reader.Close()


                        Dim updateQuery As String = "UPDATE borroweredit_tbl SET CurrentIP=@newIP, is_logged_in=1 WHERE Username=@username"
                        Using updateCmd As New MySqlCommand(updateQuery, con)
                            updateCmd.Parameters.AddWithValue("@newIP", currentDeviceIP)
                            updateCmd.Parameters.AddWithValue("@username", Username)
                            updateCmd.ExecuteNonQuery()
                        End Using

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


                        If Not GlobalVarsModule.IsBorrowerStillTimedIn(borrowerID) Then
                            Dim lrn As Object = If(borrowerType = "Student", borrowerID, CType(DBNull.Value, Object))
                            Dim employeeNo As Object = If(borrowerType = "Teacher", borrowerID, CType(DBNull.Value, Object))

                            Dim comInsert As String = "INSERT INTO oras_tbl (LRN, EmployeeNo, TimeIn) VALUES (@lrn, @emp, NOW())"
                            Using cmdInsert As New MySqlCommand(comInsert, con)
                                cmdInsert.Parameters.AddWithValue("@lrn", lrn)
                                cmdInsert.Parameters.AddWithValue("@emp", employeeNo)
                                cmdInsert.ExecuteNonQuery()
                            End Using

                            MessageBox.Show($"Welcome, {Username}!", "Time In Complete", MessageBoxButtons.OK, MessageBoxIcon.Information)
                        Else
                            Dim reminderMessage As String = $"Welcome back, {Username}! You are currently Timed In." & Environment.NewLine &
                                                        "Please Time Out before leaving the library."
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

                        If borrowerType = "Student" Or borrowerType = "student" Then
                            borrowingForm.lblnotesu.Text = "Input your LRN."
                        ElseIf borrowerType = "Teacher" Or borrowerType = "teacher" Then
                            borrowingForm.lblnotesu.Text = "Input your Employee No."
                        Else
                            borrowingForm.lblnotesu.Text = ""
                        End If

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
                        MessageBox.Show("An unexpected error occurred: The MdiParent form is not an MdiContainer." & vbCrLf &
                                    "Please ensure Borrowing form loads correctly into MainForm's Panel_dash.",
                                    "Form Load Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Else
                        MessageBox.Show("An unexpected error occurred: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    End If
                Finally
                    If reader IsNot Nothing AndAlso Not reader.IsClosed Then reader.Close()
                    If con.State = ConnectionState.Open Then con.Close()
                End Try
            End Using
        End Using
    End Sub





    Private Sub LinkLabel1_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles LinkLabel1.LinkClicked
        BorrowerCreateAccount.Show()

    End Sub

    Private Sub btnclose_Click(sender As Object, e As EventArgs)
        Application.Exit()

    End Sub



    Private Sub txtpass_KeyDown(sender As Object, e As KeyEventArgs) Handles txtpass.KeyDown
        If e.KeyCode = Keys.Enter Then
            btnlogin_Click(sender, e)
            e.Handled = True
        End If
    End Sub

    Private Sub LinkLabel2_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles LinkLabel2.LinkClicked

        Dim activeMain As MainForm = GlobalVarsModule.ActiveMainForm
        If activeMain IsNot Nothing Then
            activeMain.Hide()
        End If

        Dim dbs As ServerConnection = Application.OpenForms.OfType(Of ServerConnection)().FirstOrDefault()
        If dbs Is Nothing Then
            dbs = New ServerConnection()
            GlobalVarsModule.connectdatabase = dbs
        End If

        dbs.Show()


    End Sub

    Private Sub PictureBox1_Click(sender As Object, e As EventArgs) Handles PictureBox2.Click

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

    Private Sub PictureBox2_MouseHover_1(sender As Object, e As EventArgs) Handles PictureBox2.MouseHover

        Cursor = Cursors.Hand

    End Sub

    Private Sub PictureBox2_MouseLeave_1(sender As Object, e As EventArgs) Handles PictureBox2.MouseLeave

        Cursor = Cursors.Default

    End Sub

End Class