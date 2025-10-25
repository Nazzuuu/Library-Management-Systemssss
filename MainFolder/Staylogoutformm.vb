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

        If GlobalVarsModule.CurrentUserRole = "Borrower" AndAlso IsTimedIn Then
            Dim recordIDToUpdate As Integer = GlobalVarsModule.GetLastTimeInRecordID(GlobalVarsModule.CurrentUserID)

            If recordIDToUpdate > 0 Then
                Dim isAutoTimedOut As Boolean = GlobalVarsModule.AutomaticTimeOut(recordIDToUpdate)

                If isAutoTimedOut Then
                    MessageBox.Show(
                        "You have successfully logged out. Your Time-In session was automatically Timed Out by the system.",
                        "Auto Time-Out Complete",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    )
                Else
                    MessageBox.Show(
                        "Warning: Automatic Time-Out failed. Please notify the librarian about your active Time-In session.",
                        "Auto Time-Out Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    )
                End If
            End If
        End If

        Me.DialogResult = DialogResult.OK
        Me.Close()


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
