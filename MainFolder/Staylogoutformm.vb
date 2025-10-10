Public Class StayLogoutFormm

    Public IsTimedIn As Boolean = False

    Private Sub StayLogoutForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        lblmessage.Font = New Font("Tahoma", 11.25!, FontStyle.Bold)
        lblmessage.Text = "Are you sure you want to logout?"
        Me.Text = "Logout Confirmation"


        If GlobalVarsModule.CurrentUserRole = "Borrower" Then

            btnstay.Visible = IsTimedIn


            If IsTimedIn Then

                lblmessage.Text = "Do you want to stay or logout?."
            Else

            End If

        Else

            btnstay.Visible = True

            lblmessage.Text = "Do you want to stay or logout?."

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
                    "You are currently TIMED IN. Remember to Time Out, before leaving the library. Proceed with Stay?",
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

        Me.Hide()
        MainForm.Hide()
        BorrowerLoginForm.Show()
    End Sub
End Class