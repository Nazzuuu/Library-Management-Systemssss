Imports MySql.Data.MySqlClient

Public Class Publisher
    Private Sub Publisher_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        TopMost = True

        Dim con As New MySqlConnection(connectionString)
        Dim com As String = "SELECT * FROM `publisher_tbl`"
        Dim adap As New MySqlDataAdapter(com, con)
        Dim dt As New DataSet

        adap.Fill(dt, "INFO")
        DataGridView1.DataSource = dt.Tables("INFO")
    End Sub

    Private Sub Publisher_FormClosed(sender As Object, e As FormClosedEventArgs) Handles Me.FormClosed

        MainForm.MaintenanceToolStripMenuItem.ShowDropDown()
        MainForm.MaintenanceToolStripMenuItem.ForeColor = Color.Gray


    End Sub

    Public Sub clear()
        txtaddress.Text = ""
        txtcontact.Text = ""
        txtpublisher.Text = ""

    End Sub

    Private Sub btnadd_Click(sender As Object, e As EventArgs) Handles btnadd.Click

        Dim con As New MySqlConnection(connectionString)

        Dim publisher As String = txtpublisher.Text.Trim
        Dim address As String = txtaddress.Text.Trim
        Dim contact As String = txtcontact.Text.Trim

        Try
            con.Open()
            Dim com As New MySqlCommand("INSERT INTO `publisher_tbl`(`PublisherName`, `Address`, `ContactNumber`) VALUES (@publisher, @address, @contact)", con)

            com.Parameters.AddWithValue("@publisher", publisher)
            com.Parameters.AddWithValue("@address", address)
            com.Parameters.AddWithValue("@contact", contact)
            com.ExecuteNonQuery()

            For Each form In Application.OpenForms

                If TypeOf form Is Book Then
                    Dim book = DirectCast(form, Book)
                    book.cbpublisherr()

                End If
            Next

            MsgBox("Publisher added successfully", vbInformation)
            Publisher_Load(sender, e)

        Catch ex As Exception
            MsgBox(ex.Message, vbCritical)
        Finally
            clear()
        End Try
    End Sub
End Class