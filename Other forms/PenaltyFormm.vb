Public Class PenaltyFormm
    Private Sub PenaltyFormm_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub

    Private Sub btnpenalize_Click(sender As Object, e As EventArgs) Handles btnpenaltystudents.Click
        PenaltyStudents.RefreshPenaltyData()
        PenaltyStudents.ShowDialog()
    End Sub

    Private Sub Guna2Button1_Click(sender As Object, e As EventArgs) Handles btnpenaltyteachers.Click

    End Sub
End Class