Public Class RegisteredBrwr
    Private Sub RegisteredBrwr_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub

    Private Sub RegisteredBrwr_KeyDown(sender As Object, e As KeyEventArgs) Handles MyBase.KeyDown
        If e.KeyCode = Keys.Escape Then
            Me.Close()
        End If
    End Sub


End Class