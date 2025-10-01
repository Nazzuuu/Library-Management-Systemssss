Imports MySql.Data.MySqlClient

Public Class ReserveCopies
    Private Sub ReserveCopies_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        reserveload()
    End Sub

    Public Sub reserveload()

        Dim con As New MySqlConnection(connectionString)
        Dim com As String = "SELECT * FROM `reservecopiess_tbl`"
        Dim adap As New MySqlDataAdapter(com, con)
        Dim ds As New DataSet

        adap.Fill(ds, "reserve_info")
        DataGridView1.DataSource = ds.Tables("reserve_info")

    End Sub

    Private Sub ReserveCopies_KeyDown(sender As Object, e As KeyEventArgs) Handles MyBase.KeyDown

        If e.KeyCode = Keys.Escape Then
            Me.Close()

        End If

    End Sub
End Class