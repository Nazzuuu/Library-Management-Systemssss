Imports System.Runtime.CompilerServices.RuntimeHelpers

Public Class ResetPassword2

    Private Sub btnVerify_Click(sender As Object, e As EventArgs) Handles btnreset.Click

        If txtenter2.Text.Trim = "" Then
            MessageBox.Show("Enter the 6-digit code.", "Successful", MessageBoxButtons.OK, MessageBoxIcon.Asterisk)
            Exit Sub
        End If

        If txtenter2.Text = ResetPassword1.resetCode Then
            MessageBox.Show("Code verified successfully.", "Successful", MessageBoxButtons.OK, MessageBoxIcon.Information)
            NewPassword.Show()
            Me.Hide()
        Else
            MessageBox.Show("Invalid code.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        End If

    End Sub

    Private Sub btnBack_Click(sender As Object, e As EventArgs) Handles btnback.Click
        ResetPassword1.Show()
        Me.Hide()
    End Sub

End Class