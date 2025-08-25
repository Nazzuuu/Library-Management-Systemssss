Imports MySql.Data.MySqlClient
Public Class Genre
    Private Sub Genre_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        TopMost = True

        Dim con As New MySqlConnection(connectionString)
        Dim com As String = "SELECT * FROM `genre_tbl`"
        Dim adp As New MySqlDataAdapter(com, con)
        Dim dt As New DataSet

        adp.Fill(dt, "INFO")


        DataGridView1.DataSource = dt.Tables("INFO")
    End Sub

    Private Sub Genre_FormClosed(sender As Object, e As FormClosedEventArgs) Handles Me.FormClosed
        MainForm.MaintenanceToolStripMenuItem.ShowDropDown()
        MainForm.MaintenanceToolStripMenuItem.ForeColor = Color.Gray
    End Sub

    Private Sub btnadd_Click(sender As Object, e As EventArgs) Handles btnadd.Click

        Dim con As New MySqlConnection(connectionString)
        Dim genre As String = txtgenre.Text.Trim
        Try
            con.Open()
            Dim com As New MySqlCommand("INSERT INTO `genre_tbl`(`Genre`) VALUES (@genre)", con)
            com.Parameters.AddWithValue("@genre", genre)
            com.ExecuteNonQuery()

            For Each form In Application.OpenForms
                If TypeOf form Is Book Then
                    Dim book = DirectCast(form, Book)
                    book.cbgenree()
                End If
            Next

            MsgBox("Genre added successfully", vbInformation)
            Genre_Load(sender, e)

        Catch ex As Exception
            MsgBox(ex.Message, vbCritical)
        Finally
            txtgenre.Clear()
        End Try

    End Sub
End Class