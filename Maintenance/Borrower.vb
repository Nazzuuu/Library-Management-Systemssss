Imports MySql.Data.MySqlClient
Public Class Borrower

    Private Sub Borrower_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        Dim con As New MySqlConnection(connectionString)
        Dim com As String = "SELECT * FROM `borrower_tbl`"
        Dim adap As New MySqlDataAdapter(com, con)
        Dim dt As New DataSet

        adap.Fill(dt, "INFO")

        DataGridView1.DataSource = dt.Tables("INFO")

        DataGridView1.Columns("ID").Visible = False
        DataGridView1.EnableHeadersVisualStyles = False
        DataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(207, 58, 109)
        DataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White

    End Sub

    Private Sub btnimport_Click(sender As Object, e As EventArgs) Handles btnimport.Click

        If Pic1.Image IsNot Nothing Then
            Pic1.Image.Dispose()
        End If

        If OpenFileDialog1.ShowDialog <> DialogResult.Cancel Then

            Pic1.Image = Image.FromFile(OpenFileDialog1.FileName)
            Pic1.SizeMode = PictureBoxSizeMode.StretchImage
        End If
    End Sub

    Private Sub btnclearr_Click(sender As Object, e As EventArgs) Handles btnclearr.Click

        If Pic1.Image IsNot Nothing Then

            Pic1.Image.Dispose()
            Pic1.Image = Nothing

        End If

    End Sub


End Class