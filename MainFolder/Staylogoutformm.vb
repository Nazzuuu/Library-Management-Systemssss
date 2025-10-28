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
                Else
                    Me.Close()
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
            End If
        Else
            Me.DialogResult = DialogResult.Cancel
            Me.Close()
        End If
    End Sub

    Private Sub btnlogout_Click(sender As Object, e As EventArgs) Handles btnlogout.Click

        Dim result As DialogResult = MessageBox.Show("Are you sure you want to logout?",
                                                     "Confirmation",
                                                     MessageBoxButtons.YesNo,
                                                     MessageBoxIcon.Question)

        If result = DialogResult.Yes Then
            Try
                Dim previousRole As String = GlobalVarsModule.GlobalRole
                Dim userEmail As String = GlobalVarsModule.GlobalEmail
                Dim userName As String = GlobalVarsModule.GlobalUsername

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


                Dim borrowerLogin As BorrowerLoginForm = Application.OpenForms.OfType(Of BorrowerLoginForm)().FirstOrDefault()
                If borrowerLogin Is Nothing OrElse borrowerLogin.IsDisposed Then
                    borrowerLogin = New BorrowerLoginForm()
                    GlobalVarsModule.ActiveBorrowerLoginForm = borrowerLogin
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
        End If

    End Sub

End Class
