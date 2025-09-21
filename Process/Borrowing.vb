Imports MySql.Data.MySqlClient

Public Class Borrowing
    Private Sub Borrowing_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        Dim con As New MySqlConnection(connectionString)
        Dim com As String = "SELECT * FROM `borrowing_tbl`"
        Dim adap As New MySqlDataAdapter(com, con)
        Dim ds As New DataSet

        adap.Fill(ds, "borrowing_data")

        DataGridView1.DataSource = ds.Tables("borrowing_data")
        DataGridView1.Columns("ID").Visible = False

        DataGridView1.EnableHeadersVisualStyles = False
        DataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(207, 58, 109)
        DataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White

    End Sub

    Private Sub Borrowing_Shown(sender As Object, e As EventArgs) Handles MyBase.Shown

        DataGridView1.ClearSelection()

    End Sub
    Private Sub btnadd_Click(sender As Object, e As EventArgs) Handles btnadd.Click

    End Sub

    Private Sub btnedit_Click(sender As Object, e As EventArgs) Handles btnedit.Click

    End Sub

    Private Sub btndelete_Click(sender As Object, e As EventArgs) Handles btndelete.Click


    End Sub

    Private Sub btnclear_Click(sender As Object, e As EventArgs) Handles btnclear.Click
        clearlahat()
    End Sub

    Public Sub clearlahat()


        DataGridView1.ClearSelection()
        DataGridView1.CurrentCell = Nothing


    End Sub


End Class