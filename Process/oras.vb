Imports MySql.Data.MySqlClient

Public Class oras
    Private Sub btnregisterview_Click(sender As Object, e As EventArgs) Handles btnregisterview.Click
        RegisteredBrwr.ShowDialog()
    End Sub

    Private Sub oras_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        Dim con As New MySqlConnection(connectionString)
        Dim com As String = "SELECT * FROM `oras_tbl`"
        Dim adap As New MySqlDataAdapter(com, con)
        Dim ds As New DataSet

        adap.Fill(ds, "INFO")

        DataGridView1.DataSource = ds.Tables("INFO")
        DataGridView1.Columns("ID").Visible = False

        DataGridView1.EnableHeadersVisualStyles = False
        DataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(207, 58, 109)
        DataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White

    End Sub
End Class