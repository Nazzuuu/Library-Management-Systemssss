Imports MySql.Data.MySqlClient

Public Class Category
    Private Sub Category_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        TopMost = True

        Dim con As New MySqlConnection(connectionString)
        Dim com As String = "SELECT * FROM `category_tbl`"
        Dim adap As New MySqlDataAdapter(com, con)
        Dim dt As New DataSet

        adap.Fill(dt, "INFO")
        DataGridView1.DataSource = dt.Tables("INFO")
    End Sub

    Private Sub Category_FormClosed(sender As Object, e As FormClosedEventArgs) Handles Me.FormClosed

        MainForm.MaintenanceToolStripMenuItem.ShowDropDown()
        MainForm.MaintenanceToolStripMenuItem.ForeColor = Color.Gray

    End Sub

    Private Sub btnadd_Click(sender As Object, e As EventArgs) Handles btnadd.Click

        Dim con As New MySqlConnection(connectionString)
        Dim category As String = txtcategory.Text.Trim

        Try
            con.Open()
            Dim com As New MySqlCommand("INSERT INTO `category_tbl`(`Category`) VALUES (@category) ", con)
            com.Parameters.AddWithValue("@category", category)
            com.ExecuteNonQuery()

            For Each form In Application.OpenForms
                If TypeOf form Is Book Then
                    Dim book = DirectCast(form, Book)
                    book.cbcategoryy()
                End If
            Next

            MsgBox("Category added successfully", vbInformation)
            Category_Load(sender, e)

        Catch ex As Exception
            MsgBox(ex.Message, vbCritical)
        Finally
            txtcategory.Clear()
        End Try
    End Sub
End Class