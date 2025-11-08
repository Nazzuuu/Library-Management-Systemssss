Imports MySql.Data.MySqlClient

Public Class StayLogoutFormm

    Public IsTimedIn As Boolean = False

    Private Sub StayLogoutForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        lblmessage.Font = New Font("Tahoma", 11.25!, FontStyle.Bold)
        lblmessage.Text = "Are you sure you want to logout?"
        Me.Text = "Logout Confirmation"

        If GlobalVarsModule.CurrentUserRole = "Borrower" Then
            btnstay.Visible = IsTimedIn

            If IsTimedIn Then
                lblmessage.Text = "Proceed to Log Out, or Stay Inside?"
            Else
                lblmessage.Text = "You are not timed in yet. Proceed to logout?"
            End If
        Else
            btnstay.Visible = True
            lblmessage.Text = "Proceed to Log Out, or Stay Inside?"
        End If
    End Sub

    Private Sub btnstay_Click(sender As Object, e As EventArgs) Handles btnstay.Click
        If GlobalVarsModule.CurrentUserRole = "Borrower" Then
            If Not IsTimedIn Then
                Dim timeInPrompt = MessageBox.Show(
                    "You are NOT yet TIMED IN. You must time-in first. Do you want to proceed to the Time-In/Out form now?",
                    "Time In Required",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning
                )

                If timeInPrompt = DialogResult.Yes Then
                    Me.DialogResult = DialogResult.Retry
                    Me.Close()
                    login.txtpass.Text = ""
                    login.txtuser.Text = ""
                Else
                    Me.Close()
                    login.txtpass.Text = ""
                    login.txtuser.Text = ""
                End If
            Else
                MessageBox.Show(
                    "You are currently TIMED IN. Remember to Time Out before leaving the library.",
                    "Time Out Reminder",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                )

                Me.DialogResult = DialogResult.Yes
                Me.Close()
                login.txtpass.Text = ""
                login.txtuser.Text = ""
            End If
        Else
            Me.DialogResult = DialogResult.Cancel
            Me.Close()
            login.txtpass.Text = ""
            login.txtuser.Text = ""
        End If
    End Sub

    Private Sub btnlogout_Click(sender As Object, e As EventArgs) Handles btnlogout.Click

        Dim result As DialogResult = MessageBox.Show("Are you sure you want to logout?",
                                                     "Confirmation",
                                                     MessageBoxButtons.YesNo,
                                                     MessageBoxIcon.Question)

        If result <> DialogResult.Yes Then Exit Sub

        Try
            Dim previousRole As String = GlobalVarsModule.GlobalRole
            Dim userEmail As String = GlobalVarsModule.GlobalEmail
            Dim userName As String = GlobalVarsModule.GlobalUsername


            If GlobalVarsModule.CurrentUserRole.Equals("Borrower", StringComparison.OrdinalIgnoreCase) Then
                Dim borrowerID As String = GlobalVarsModule.GetCleanCurrentBorrowerID()
                If Not String.IsNullOrEmpty(borrowerID) Then

                    Dim activeRecordID As Integer = GlobalVarsModule.GetLastTimeInRecordID(borrowerID)
                    If activeRecordID > 0 Then
                        Dim timeoutSuccess As Boolean = GlobalVarsModule.AutomaticTimeOut(activeRecordID)
                        If timeoutSuccess Then
                            MessageBox.Show("You have successfully logged out. Your Time-In session was automatically Timed Out by the system.",
                                            "Auto Time-Out",
                                            MessageBoxButtons.OK,
                                            MessageBoxIcon.Information)
                        End If
                    End If

                    Using con As New MySqlConnection(GlobalVarsModule.connectionString)
                        con.Open()
                        Dim updateLogout As String =
                            "UPDATE borroweredit_tbl " &
                            "SET is_logged_in = 0, CurrentIP = NULL " &
                            "WHERE (LRN = @id OR EmployeeNo = @id) LIMIT 1"

                        Using cmd As New MySqlCommand(updateLogout, con)
                            cmd.Parameters.AddWithValue("@id", borrowerID)
                            cmd.ExecuteNonQuery()
                        End Using
                    End Using
                End If


                GlobalVarsModule.ShouldShowMainFormNextLogin = True
            End If


            If previousRole.Equals("Librarian", StringComparison.OrdinalIgnoreCase) OrElse
               previousRole.Equals("Assistant Librarian", StringComparison.OrdinalIgnoreCase) OrElse
               previousRole.Equals("Staff", StringComparison.OrdinalIgnoreCase) Then

                GlobalVarsModule.LogAudit(
                    actionType:="LOGOUT SUCCESS",
                    formName:="MAIN FORM",
                    description:=$"User '{userName}' ({previousRole}) successfully logged out.",
                    recordID:="N/A"
                )
            End If



            GlobalVarsModule.GlobalRole = "Guest"
            GlobalVarsModule.GlobalEmail = ""
            GlobalVarsModule.GlobalUsername = ""
            GlobalVarsModule.CurrentUserRole = "Guest"
            GlobalVarsModule.CurrentBorrowerType = ""
            GlobalVarsModule.CurrentBorrowerID = ""
            GlobalVarsModule.CurrentUserID = ""
            GlobalVarsModule.CurrentEmployeeID = ""


            If Borrowing IsNot Nothing AndAlso Not Borrowing.IsDisposed Then
                Borrowing.Close()
                Borrowing.Dispose()
                Borrowing = Nothing
            End If



            Dim activeMain As MainForm = GlobalVarsModule.ActiveMainForm
            If activeMain IsNot Nothing Then
                activeMain.Hide()
            End If

            Me.DialogResult = DialogResult.OK
            Me.Hide()


            Dim borrowerLogin As login = Application.OpenForms.OfType(Of login)().FirstOrDefault()
            If borrowerLogin Is Nothing OrElse borrowerLogin.IsDisposed Then
                borrowerLogin = New login()
                GlobalVarsModule.loginform = borrowerLogin
            End If

            borrowerLogin.txtuser.Clear()
            borrowerLogin.txtpass.Clear()
            borrowerLogin.Show()
            borrowerLogin.BringToFront()

        Catch ex As Exception
            MessageBox.Show("An error occurred while logging out: " & ex.Message,
                            "Logout Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error)
        End Try
    End Sub




End Class
