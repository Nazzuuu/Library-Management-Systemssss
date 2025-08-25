Imports MySql.Data.MySqlClient

Public Class Author
    Private Sub Author_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        TopMost = True

        Me.Font = New Font("Baskerville Old Face", 9)
        Me.Refresh()


        Dim con As New MySqlConnection(connectionString)
        Dim comm As String = "SELECT * FROM `author_tbl`"
        Dim adap As New MySqlDataAdapter(comm, con)
        Dim ds As New DataSet
        adap.Fill(ds, "INFO")
        DataGridView1.DataSource = ds.Tables("INFO")



    End Sub

    Private Sub Guna2ControlBox1_Click(sender As Object, e As EventArgs)
        MainForm.Show()

    End Sub

    Private Sub Author_FormClosed(sender As Object, e As FormClosedEventArgs) Handles Me.FormClosed
        MainForm.MaintenanceToolStripMenuItem.ShowDropDown()
        MainForm.MaintenanceToolStripMenuItem.ForeColor = Color.Gray

    End Sub

    Private Sub btnadd_Click(sender As Object, e As EventArgs) Handles btnadd.Click

        Dim con As New MySqlConnection(connectionString)
        Dim author As String = txtauthor.Text.Trim
        Try
            con.Open()

            Dim com As New MySqlCommand("INSERT INTO `author_tbl`(`AuthorName`) VALUES (@author)", con)
            com.Parameters.AddWithValue("@author", author)

            com.ExecuteNonQuery()

            For Each form In Application.OpenForms
                If TypeOf form Is Book Then
                    Dim book = DirectCast(form, Book)
                    book.cbauthorr()
                    Exit For
                End If
            Next


            Author_Load(sender, e)
            MsgBox("Author added successfully", vbInformation)

        Catch ex As Exception
            MsgBox(ex.Message, vbCritical)
        Finally
            txtauthor.Clear()
        End Try
    End Sub

    Private Sub btnedit_Click(sender As Object, e As EventArgs) Handles btnedit.Click

    End Sub

    Private Sub btndelete_Click(sender As Object, e As EventArgs) Handles btndelete.Click

    End Sub
End Class