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

        DataGridView1.EnableHeadersVisualStyles = False
        DataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(207, 58, 109)
        DataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White

    End Sub

    Private Sub Publisher_FormClosed(sender As Object, e As FormClosedEventArgs) Handles Me.FormClosed

        MainForm.MaintenanceToolStripMenuItem.ShowDropDown()
        MainForm.MaintenanceToolStripMenuItem.ForeColor = Color.Gray

        DataGridView1.EnableHeadersVisualStyles = False
        DataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(207, 58, 109)
        DataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White

        clear()
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

        If String.IsNullOrWhiteSpace(publisher) OrElse String.IsNullOrWhiteSpace(address) OrElse String.IsNullOrWhiteSpace(contact) Then
            MsgBox("Please fill in the required fields.", vbExclamation, "Missing Information")
            Exit Sub
        End If

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

    Private Sub btnedit_Click(sender As Object, e As EventArgs) Handles btnedit.Click

        If DataGridView1.SelectedRows.Count > 0 Then

            Dim con As New MySqlConnection(connectionString)

            Dim pub As String = txtpublisher.Text.Trim
            Dim address As String = txtaddress.Text.Trim
            Dim contact As String = txtcontact.Text.Trim

            Dim selectedRow As DataGridViewRow = DataGridView1.SelectedRows(0)
            Dim ID As Integer = CInt(selectedRow.Cells("ID").Value)

            If String.IsNullOrWhiteSpace(pub) OrElse String.IsNullOrWhiteSpace(address) OrElse String.IsNullOrWhiteSpace(contact) Then
                MsgBox("Please fill in the required fields.", vbExclamation, "Missing Information")
                Exit Sub
            End If

            Try
                con.Open()
                Dim com As New MySqlCommand("UPDATE `publisher_tbl` SET `PublisherName`= @publisher,`Address`= @address,`ContactNumber`= @contact  WHERE `ID` = @id", con)

                com.Parameters.AddWithValue("@publisher", pub)
                com.Parameters.AddWithValue("@address", address)
                com.Parameters.AddWithValue("@contact", contact)
                com.Parameters.AddWithValue("@id", ID)
                com.ExecuteNonQuery()

                For Each form In Application.OpenForms

                    If TypeOf form Is Book Then
                        Dim book = DirectCast(form, Book)
                        book.cbpublisherr()

                    End If
                Next

                MsgBox("Updated successfully", vbInformation)
                Publisher_Load(sender, e)

            Catch ex As Exception
                MsgBox(ex.Message, vbCritical)
            Finally
                clear()
            End Try
        Else
            MsgBox("Please select a row to edit.", vbExclamation)
        End If
    End Sub

    Private Sub btndelete_Click(sender As Object, e As EventArgs) Handles btndelete.Click

        If DataGridView1.SelectedRows.Count > 0 Then

            Dim dialogResult As DialogResult = MessageBox.Show("Are you sure you want to delete this publisher?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)

            If dialogResult = DialogResult.Yes Then

                Dim con As New MySqlConnection(connectionString)
                Dim selectedRow As DataGridViewRow = DataGridView1.SelectedRows(0)
                Dim ID As Integer = CInt(selectedRow.Cells("ID").Value)
                Dim publisherName As String = selectedRow.Cells("PublisherName").Value.ToString().Trim()

                Try
                    con.Open()


                    Dim bookCom As New MySqlCommand("SELECT COUNT(*) FROM `book_tbl` WHERE Publisher = @publisher", con)
                    bookCom.Parameters.AddWithValue("@publisher", publisherName)
                    Dim bookCount As Integer = CInt(bookCom.ExecuteScalar())

                    If bookCount > 0 Then
                        MessageBox.Show("Cannot delete this publisher. They are assigned to " & bookCount & " book(s).", "Information", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        Return
                    End If


                    Dim delete As New MySqlCommand("DELETE FROM `publisher_tbl` WHERE `ID` = @id", con)
                    delete.Parameters.AddWithValue("@id", ID)
                    delete.ExecuteNonQuery()

                    For Each form In Application.OpenForms
                        If TypeOf form Is Book Then
                            Dim book = DirectCast(form, Book)
                            book.cbpublisherr()
                        End If
                    Next

                    MsgBox("Publisher deleted successfully.", vbInformation)
                    Publisher_Load(sender, e)
                    clear()

                    Dim count As New MySqlCommand("SELECT COUNT(*) FROM `publisher_tbl`", con)
                    Dim rowCount As Long = CLng(count.ExecuteScalar())

                    If rowCount = 0 Then
                        Dim reset As New MySqlCommand("ALTER TABLE `publisher_tbl` AUTO_INCREMENT = 1", con)
                        reset.ExecuteNonQuery()
                    End If

                Catch ex As Exception
                    MsgBox(ex.Message, vbCritical)
                End Try
            End If
        End If
    End Sub

    Private Sub DataGridView1_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellClick

        If e.RowIndex >= 0 Then

            Dim row = DataGridView1.Rows(e.RowIndex)
            txtpublisher.Text = row.Cells("PublisherName").Value.ToString
            txtaddress.Text = row.Cells("Address").Value.ToString
            txtcontact.Text = row.Cells("ContactNumber").Value.ToString

        End If

    End Sub

    Private Sub txtsearch_TextChanged(sender As Object, e As EventArgs) Handles txtsearch.TextChanged


        Dim dt As DataTable = DirectCast(DataGridView1.DataSource, DataTable)
        If dt IsNot Nothing Then
            If txtsearch.Text.Trim() <> "" Then
                Dim filter As String = String.Format("PublisherName LIKE '*{0}*'", txtsearch.Text.Trim())
                dt.DefaultView.RowFilter = filter
            Else
                dt.DefaultView.RowFilter = ""
            End If
        End If

    End Sub


    Private Sub txtpublisher_KeyDown(sender As Object, e As KeyEventArgs) Handles txtpublisher.KeyDown

        If e.Control AndAlso (e.KeyCode = Keys.V Or e.KeyCode = Keys.C Or e.KeyCode = Keys.X) Then
            e.SuppressKeyPress = True
        End If

    End Sub

    Private Sub txtpublisher_KeyPress(sender As Object, e As KeyPressEventArgs) Handles txtpublisher.KeyPress

        If Not Char.IsLetter(e.KeyChar) AndAlso Not Char.IsControl(e.KeyChar) AndAlso Not Char.IsWhiteSpace(e.KeyChar) Then
            e.Handled = True
        End If

    End Sub

    Private Sub txtcontact_KeyDown(sender As Object, e As KeyEventArgs) Handles txtcontact.KeyDown

        If e.Control AndAlso (e.KeyCode = Keys.V Or e.KeyCode = Keys.C Or e.KeyCode = Keys.X) Then
            e.SuppressKeyPress = True
        End If

        If e.KeyCode = Keys.Back Then
            RemoveHandler txtcontact.TextChanged, AddressOf txtcontact_TextChanged
        End If

    End Sub

    Private Sub txtcontact_KeyPress(sender As Object, e As KeyPressEventArgs) Handles txtcontact.KeyPress


        If Not Char.IsDigit(e.KeyChar) And Not Char.IsControl(e.KeyChar) Then
            e.Handled = True
        End If

    End Sub

    Private Sub txtaddress_KeyDown(sender As Object, e As KeyEventArgs) Handles txtaddress.KeyDown

        If e.Control AndAlso (e.KeyCode = Keys.V Or e.KeyCode = Keys.C Or e.KeyCode = Keys.X) Then
            e.SuppressKeyPress = True
        End If

    End Sub

    Private Sub txtaddress_KeyPress(sender As Object, e As KeyPressEventArgs) Handles txtaddress.KeyPress

        If Not Char.IsLetterOrDigit(e.KeyChar) AndAlso Not Char.IsControl(e.KeyChar) AndAlso Not Char.IsWhiteSpace(e.KeyChar) AndAlso e.KeyChar <> "#" Then
            e.Handled = True
        End If

    End Sub

    Private Sub txtcontact_TextChanged(sender As Object, e As EventArgs) Handles txtcontact.TextChanged


        Dim oreyjeynal As String = txtcontact.Text

        If String.IsNullOrEmpty(oreyjeynal) Then
            Return
        End If

        If oreyjeynal.StartsWith("09") Then

        Else

            If oreyjeynal.Length > 0 Then
                txtcontact.Clear()
                txtcontact.Text = "09"
                txtcontact.SelectionStart = 2
            End If
        End If

    End Sub

    Private Sub txtsearch_KeyDown(sender As Object, e As KeyEventArgs) Handles txtsearch.KeyDown

        If e.Control AndAlso (e.KeyCode = Keys.V Or e.KeyCode = Keys.C Or e.KeyCode = Keys.X) Then
            e.SuppressKeyPress = True
        End If

    End Sub

    Private Sub txtcontact_KeyUp(sender As Object, e As KeyEventArgs) Handles txtcontact.KeyUp

        If e.KeyCode = Keys.Back Then
            AddHandler txtcontact.TextChanged, AddressOf txtcontact_TextChanged

        End If

    End Sub
End Class