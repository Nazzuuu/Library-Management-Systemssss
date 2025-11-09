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
        Dim currentDeviceIP As String = GlobalVarsModule.GetLocalIPAddress()

        If Not String.IsNullOrEmpty(GlobalVarsModule.CurrentUserID) Then
            GlobalVarsModule.UpdateUserIP(currentDeviceIP, GlobalVarsModule.CurrentUserID, GlobalVarsModule.GlobalRole)
        End If

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
                "SELECT ID, Username, Password, Email, Role, CurrentIP, is_logged_in, 'superadmin_tbl' AS SourceTable " &
                "FROM superadmin_tbl WHERE Username = @username AND Password = @password " &
                "UNION ALL " &
                "SELECT ID, Username, Password, Email, Role, CurrentIP, is_logged_in, 'user_staff_tbl' AS SourceTable " &
                "FROM user_staff_tbl WHERE Username = @username AND Password = @password"

            Using com As New MySqlCommand(comsus, con)
                com.Parameters.AddWithValue("@username", User)
                com.Parameters.AddWithValue("@password", Pass)

                Try
                    con.Open()

                    Dim role As String = ""
                    Dim userEmail As String = ""
                    Dim employeeID As String = ""
                    Dim activeIP As String = ""
                    Dim isLoggedInStatus As Integer = 0
                    Dim updateTable As String = ""

                    Using lahatngrole As MySqlDataReader = com.ExecuteReader()
                        If lahatngrole.Read() Then
                            role = lahatngrole("Role").ToString()
                            userEmail = lahatngrole("Email").ToString()
                            employeeID = lahatngrole("ID").ToString()
                            activeIP = If(IsDBNull(lahatngrole("CurrentIP")), "", lahatngrole("CurrentIP").ToString()).Trim()
                            isLoggedInStatus = If(IsDBNull(lahatngrole("is_logged_in")), 0, Convert.ToInt32(lahatngrole("is_logged_in")))
                            updateTable = lahatngrole("SourceTable").ToString()
                        Else
                            lahatngrole.Close()
                            con.Close()
                            GoTo TryBorrower
                        End If
                    End Using


                    If isLoggedInStatus = 1 Then
                        GlobalVarsModule.LogAudit("LOGIN BLOCKED (Is Logged In)", "LOGIN FORM",
                            $"User '{User}' ({role}) blocked. Account is already active. Current IP: {currentDeviceIP}.")
                        MessageBox.Show($"Account '{User}' ({role}) is already active.", "Login Blocked", MessageBoxButtons.OK, MessageBoxIcon.Stop)
                        Exit Sub
                    End If

                    If Not String.IsNullOrWhiteSpace(activeIP) AndAlso activeIP <> "0.0.0.0" AndAlso activeIP <> currentDeviceIP Then
                        GlobalVarsModule.LogAudit("LOGIN BLOCKED (Conflict)", "LOGIN FORM",
                            $"User '{User}' ({role}) blocked. Already logged in on IP: {activeIP}. Attempted IP: {currentDeviceIP}.")
                        MessageBox.Show($"Account '{User}' ({role}) is already logged in on another device with IP: {activeIP}.", "Login Blocked", MessageBoxButtons.OK, MessageBoxIcon.Stop)
                        Exit Sub
                    End If


                    Dim updateQuery As String = $"UPDATE {updateTable} SET CurrentIP = @newIP, is_logged_in = 1 WHERE Username = @username"
                    Using updateCmd As New MySqlCommand(updateQuery, con)
                        updateCmd.Parameters.AddWithValue("@newIP", currentDeviceIP)
                        updateCmd.Parameters.AddWithValue("@username", User)
                        updateCmd.ExecuteNonQuery()
                    End Using


                    GlobalVarsModule.GlobalRole = role
                    GlobalVarsModule.GlobalUsername = User
                    GlobalVarsModule.GlobalEmail = userEmail
                    GlobalVarsModule.CurrentUserID = employeeID
                    GlobalVarsModule.CurrentEmployeeID = employeeID

                    GlobalVarsModule.LogAudit("LOGIN SUCCESS", "LOGIN FORM",
                        $"User '{User}' ({role}) successfully logged in from IP: {currentDeviceIP}.")

                    MessageBox.Show($"Welcome, {User}! You are logged in as {role}.", "Login Successful", MessageBoxButtons.OK, MessageBoxIcon.Information)


                    If GlobalVarsModule.ActiveMainForm IsNot Nothing Then
                        If Not GlobalVarsModule.ActiveMainForm.IsDisposed Then
                            GlobalVarsModule.ActiveMainForm.Close()
                        End If
                        GlobalVarsModule.ActiveMainForm = Nothing
                    End If


                    Dim activeMain As New MainForm()
                    GlobalVarsModule.ActiveMainForm = activeMain
                    activeMain.Show()
                    activeMain.ResetToMainDashboard()


                    Me.Hide()

                    activeMain.lblgmail.Text = userEmail
                    activeMain.lblform.Text = "MAIN FORM"

                    Select Case role
                        Case "Librarian"
                            activeMain.lbl_currentuser.Text = "Librarian"
                            activeMain.AcquisitionToolStripMenuItem.Visible = True
                            activeMain.AccessionToolStripMenuItem.Visible = True
                            activeMain.UserMaintenanceToolStripMenuItem.Visible = True
                            activeMain.Audit_Trail.Visible = True
                            activeMain.MyAccountToolStripMenuItem.Visible = True
                            activeMain.BorrowerAccountToolStripMenuItem.Visible = True
                            activeMain.BorrowerAccountToolStripMenuItem1.Visible = False
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
                            activeMain.MyAccountToolStripMenuItem.Visible = False
                            activeMain.BorrowerAccountToolStripMenuItem.Visible = False
                            activeMain.BorrowerAccountToolStripMenuItem1.Visible = False
                            activeMain.BorrowToolStripMenuItem.Visible = False
                            activeMain.PenaltyToolStripMenuItem.Visible = False
                            activeMain.PenaltyManagementToolStripMenuItem.Visible = False

                        Case "Assistant Librarian"
                            activeMain.lbl_currentuser.Text = "Asst. Librarian"
                            activeMain.Audit_Trail.Visible = False
                            activeMain.MyAccountToolStripMenuItem.Visible = False
                            activeMain.BorrowerAccountToolStripMenuItem.Visible = False
                            activeMain.BorrowerAccountToolStripMenuItem1.Visible = False
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
                    Exit Sub

                Catch ex As MySqlException
                    MessageBox.Show("Database Error: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Catch ex As Exception
                    MessageBox.Show("Unexpected error: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Finally
                    If con.State = ConnectionState.Open Then con.Close()
                End Try
            End Using
        End Using

TryBorrower:

        Dim activeMain_borrower As MainForm = GlobalVarsModule.ActiveMainForm

        If activeMain_borrower Is Nothing Then
            activeMain_borrower = Application.OpenForms.OfType(Of MainForm)().FirstOrDefault()
            If activeMain_borrower Is Nothing Then
                activeMain_borrower = New MainForm()
                GlobalVarsModule.ActiveMainForm = activeMain_borrower
                activeMain_borrower.Show()
                activeMain_borrower.Hide()
            End If
        End If



        Dim Username_borrower As String = txtuser.Text.Trim()
        Dim Password_borrower As String = txtpass.Text.Trim()

        If String.IsNullOrWhiteSpace(Username_borrower) OrElse String.IsNullOrWhiteSpace(Password_borrower) Then
            MessageBox.Show("Please enter both Username and Password.", "Required Fields", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Dim currentDeviceIP_borrower As String = GlobalVarsModule.GetLocalIPAddress()

        Using con_borrower As New MySqlConnection(GlobalVarsModule.connectionString)
            Dim sql_borrower As String =
        "SELECT Username, Password, Email, " &
        "CASE WHEN LRN IS NOT NULL AND LRN <> '0' AND LRN <> '' THEN 'Student' " &
        "     WHEN EmployeeNo IS NOT NULL AND EmployeeNo <> '0' AND EmployeeNo <> '' THEN 'Teacher' " &
        "     ELSE 'Unknown' END AS BorrowerType, " &
        "IFNULL(LRN, EmployeeNo) AS BorrowerID, " &
        "IFNULL(CurrentIP, '0.0.0.0') AS CurrentIP, " &
        "IFNULL(is_logged_in, 0) AS is_logged_in " &
        "FROM borroweredit_tbl " &
        "WHERE BINARY TRIM(Username)=@User AND BINARY TRIM(Password)=@Pass LIMIT 1"

            Using cmd_borrower As New MySqlCommand(sql_borrower, con_borrower)
                cmd_borrower.Parameters.AddWithValue("@User", Username_borrower)
                cmd_borrower.Parameters.AddWithValue("@Pass", Password_borrower)

                Dim reader_borrower As MySqlDataReader = Nothing

                Try
                    con_borrower.Open()
                    reader_borrower = cmd_borrower.ExecuteReader()

                    If reader_borrower.Read() Then
                        Dim userEmail_borrower As String = reader_borrower("Email").ToString()
                        Dim borrowerID As String = reader_borrower("BorrowerID").ToString()
                        Dim borrowerType As String = reader_borrower("BorrowerType").ToString()
                        Dim activeIP_borrower As String = reader_borrower("CurrentIP").ToString().Trim()
                        Dim isLoggedInStatus_borrower As Integer = Convert.ToInt32(reader_borrower("is_logged_in"))

                        If isLoggedInStatus_borrower = 1 AndAlso activeIP_borrower <> currentDeviceIP_borrower Then
                            MessageBox.Show($"Account '{Username_borrower}' is already logged in on another device (IP: {activeIP_borrower}).", "Login Blocked", MessageBoxButtons.OK, MessageBoxIcon.Stop)
                            Exit Sub
                        End If

                        reader_borrower.Close()

                        Dim updateQuery_borrower As String = "UPDATE borroweredit_tbl SET CurrentIP=@newIP, is_logged_in=1 WHERE Username=@username"
                        Using updateCmd_borrower As New MySqlCommand(updateQuery_borrower, con_borrower)
                            updateCmd_borrower.Parameters.AddWithValue("@newIP", currentDeviceIP_borrower)
                            updateCmd_borrower.Parameters.AddWithValue("@username", Username_borrower)
                            updateCmd_borrower.ExecuteNonQuery()
                        End Using

                        GlobalVarsModule.CurrentUserRole = "Borrower"
                        GlobalVarsModule.CurrentBorrowerType = borrowerType
                        GlobalVarsModule.CurrentBorrowerID = borrowerID
                        GlobalVarsModule.CurrentUserID = borrowerID
                        GlobalVarsModule.GlobalRole = "Borrower"
                        GlobalVarsModule.GlobalEmail = userEmail_borrower

                        activeMain_borrower.SetupBorrowerUI(borrowerType)

                        If borrowerType = "Unknown" Then
                            MessageBox.Show("Login successful, but borrower type (LRN or Employee No) is missing in the record. Please check your account details.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        End If

                        If Not GlobalVarsModule.IsBorrowerStillTimedIn(borrowerID) Then
                            Dim lrn As Object = If(borrowerType = "Student", borrowerID, CType(DBNull.Value, Object))
                            Dim employeeNo As Object = If(borrowerType = "Teacher", borrowerID, CType(DBNull.Value, Object))

                            Dim comInsert_borrower As String = "INSERT INTO oras_tbl (LRN, EmployeeNo, TimeIn) VALUES (@lrn, @emp, NOW())"
                            Using cmdInsert_borrower As New MySqlCommand(comInsert_borrower, con_borrower)
                                cmdInsert_borrower.Parameters.AddWithValue("@lrn", lrn)
                                cmdInsert_borrower.Parameters.AddWithValue("@emp", employeeNo)
                                cmdInsert_borrower.ExecuteNonQuery()
                            End Using

                            MessageBox.Show($"Welcome, {Username_borrower}!", "Time In Complete", MessageBoxButtons.OK, MessageBoxIcon.Information)
                        Else
                            Dim reminderMessage As String = $"Welcome back, {Username_borrower}! You are currently Timed In." & Environment.NewLine &
                                                "Please Time Out before leaving the library."
                            MessageBox.Show(reminderMessage, "Time Out Reminder 🔔", MessageBoxButtons.OK, MessageBoxIcon.Information)
                        End If

                        Dim orasForm_borrower As oras = Application.OpenForms.OfType(Of oras)().FirstOrDefault()
                        If orasForm_borrower IsNot Nothing Then
                            orasForm_borrower.ludeyngoras()
                        End If

                        activeMain_borrower.PrintReceiptToolStripMenuItem.Visible = False

                        If activeMain_borrower.BorrowerEditsInfoForm Is Nothing Then
                            activeMain_borrower.BorrowerEditsInfoForm = New Borrowereditsinfo()
                        End If
                        activeMain_borrower.BorrowerEditsInfoForm.visibilitysus(borrowerType)

                        activeMain_borrower.lbl_currentuser.Text = borrowerType
                        activeMain_borrower.lblgmail.Text = userEmail_borrower

                        If activeMain_borrower.MaintenanceToolStripMenuItem IsNot Nothing Then
                            activeMain_borrower.MaintenanceToolStripMenuItem.Visible = False
                            activeMain_borrower.btnlogoutt.Visible = False
                        End If
                        If activeMain_borrower.SettingsStripMenuItem IsNot Nothing Then
                            activeMain_borrower.SettingsStripMenuItem.Visible = False
                        End If

                        If activeMain_borrower.ProcessStripMenuItem IsNot Nothing Then
                            activeMain_borrower.ProcessStripMenuItem.Visible = True
                            For Each item As ToolStripItem In activeMain_borrower.ProcessStripMenuItem.DropDownItems
                                item.Visible = False
                            Next

                            If activeMain_borrower.StudentLogsToolStripMenuItem IsNot Nothing Then
                                activeMain_borrower.StudentLogsToolStripMenuItem.Visible = True
                                If activeMain_borrower.TimeInToolStripMenuItem IsNot Nothing Then
                                    activeMain_borrower.TimeInToolStripMenuItem.Visible = True
                                End If
                            End If

                            If activeMain_borrower.BorrowerAccountToolStripMenuItem1 IsNot Nothing Then
                                activeMain_borrower.BorrowerAccountToolStripMenuItem1.Visible = True
                            End If

                            If activeMain_borrower.CirculationToolStripMenuItem IsNot Nothing Then
                                activeMain_borrower.CirculationToolStripMenuItem.Visible = True
                                For Each circItem As ToolStripItem In activeMain_borrower.CirculationToolStripMenuItem.DropDownItems
                                    If circItem Is activeMain_borrower.BorrowToolStripMenuItem Then
                                        circItem.Visible = True
                                    Else
                                        circItem.Visible = False
                                    End If
                                Next
                            End If
                        End If

                        activeMain_borrower.Panel_dash.Controls.Clear()
                        Dim borrowingForm_borrower As New Borrowing()
                        borrowingForm_borrower.TopLevel = False
                        borrowingForm_borrower.BringToFront()
                        activeMain_borrower.Panel_dash.Controls.Add(borrowingForm_borrower)
                        borrowingForm_borrower.SetupBorrowerFields()
                        borrowingForm_borrower.Show()

                        If borrowerType = "Student" Or borrowerType = "student" Then
                            borrowingForm_borrower.lblnotesu.Text = "Input your LRN."
                        ElseIf borrowerType = "Teacher" Or borrowerType = "teacher" Then
                            borrowingForm_borrower.lblnotesu.Text = "Input your Employee No."
                        Else
                            borrowingForm_borrower.lblnotesu.Text = ""
                        End If

                        activeMain_borrower.lblform.Text = "BORROWING FORM"
                        activeMain_borrower.Show()
                        Me.Hide()

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
                    If reader_borrower IsNot Nothing AndAlso Not reader_borrower.IsClosed Then reader_borrower.Close()
                    If con_borrower.State = ConnectionState.Open Then con_borrower.Close()
                End Try

            End Using
        End Using


    End Sub





    Private Sub login_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        TopMost = True
        MainForm.Refresh()
        Me.KeyPreview = True
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

    Private Sub LinkLabel2_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles LinkLabel2.LinkClicked
        BorrowerCreateAccount.Show()
    End Sub

    Private Sub login_KeyDown(sender As Object, e As KeyEventArgs) Handles MyBase.KeyDown
        If e.Control AndAlso e.Shift AndAlso e.KeyCode = Keys.Enter Then
            Try
                Dim serverForm As New ServerConnection()
                serverForm.Show()
                serverForm.BringToFront()
            Catch ex As Exception
                MessageBox.Show("Unable to open Server Connection form: " & ex.Message,
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End If
    End Sub
End Class