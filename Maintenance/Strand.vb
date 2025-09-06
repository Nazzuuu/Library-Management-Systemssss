Imports MySql.Data.MySqlClient

Public Class Strand
    Private Sub Strand_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        TopMost = True

        Dim con As New MySqlConnection(connectionString)
        Dim com As String = "SELECT * FROM `strand_tbl`"
        Dim adap As New MySqlDataAdapter(com, con)
        Dim dt As New DataSet

        adap.Fill(dt, "INFO")
        DataGridView1.DataSource = dt.Tables("INFO")

        DataGridView1.EnableHeadersVisualStyles = False
        DataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(207, 58, 109)
        DataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White

    End Sub

    Private Sub Strand_FormClosed(sender As Object, e As FormClosedEventArgs) Handles Me.FormClosed

        MainForm.MaintenanceToolStripMenuItem.ShowDropDown()
        MainForm.MaintenanceToolStripMenuItem.ForeColor = Color.Gray

    End Sub

    Private Sub btnadd_Click(sender As Object, e As EventArgs) Handles btnadd.Click

        Dim con As New MySqlConnection(connectionString)
        Dim strandd As String = txtstrand.Text.Trim
        Try
            con.Open()
            Dim com As New MySqlCommand("INSERT INTO `strand_tbl`(`Strand`) VALUES (@strand)", con)
            com.Parameters.AddWithValue("@strand", strandd)
            com.ExecuteNonQuery()

            For Each form In Application.OpenForms
                If TypeOf form Is Borrower Then
                    Dim borrower = DirectCast(form, Borrower)
                    borrower.cbstrandd()
                End If
            Next

            MsgBox("Strand added successfully", vbInformation)
            Strand_Load(sender, e)

        Catch ex As Exception
            MsgBox(ex.Message, vbCritical)
        Finally
            txtstrand.Clear()

        End Try

    End Sub

    Private Sub btnedit_Click(sender As Object, e As EventArgs) Handles btnedit.Click

        If DataGridView1.SelectedRows.Count > 0 Then

            Dim con As New MySqlConnection(connectionString)

            Dim selectedRow As DataGridViewRow = DataGridView1.SelectedRows(0)
            Dim ID As Integer = CInt(selectedRow.Cells("ID").Value)

            Dim strandd As String = txtstrand.Text.Trim

            Try
                con.Open()
                Dim com As New MySqlCommand("UPDATE `strand_tbl` SET `Strand`= @strand WHERE  `ID` = @id", con)
                com.Parameters.AddWithValue("@strand", strandd)
                com.Parameters.AddWithValue("@id", ID)
                com.ExecuteNonQuery()

                For Each form In Application.OpenForms
                    If TypeOf form Is Borrower Then
                        Dim borrower = DirectCast(form, Borrower)
                        borrower.cbstrandd()
                    End If
                Next

                MsgBox("Updated successfully!", vbInformation)
                Strand_Load(sender, e)
                txtstrand.Clear()
            Catch ex As Exception
                MsgBox(ex.Message, vbCritical)
            End Try

        End If

    End Sub

    Private Sub btndelete_Click(sender As Object, e As EventArgs) Handles btndelete.Click

        If DataGridView1.SelectedRows.Count > 0 Then

            Dim dialogResult As DialogResult = MessageBox.Show("Are you sure you want to delete this strand?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)

            If dialogResult = DialogResult.Yes Then

                Dim con As New MySqlConnection(connectionString)
                Dim selectedRow As DataGridViewRow = DataGridView1.SelectedRows(0)
                Dim ID As Integer = CInt(selectedRow.Cells("ID").Value)

                Try
                    con.Open()
                    Dim delete As New MySqlCommand("DELETE FROM `strand_tbl` WHERE `ID` = @id", con)
                    delete.Parameters.AddWithValue("@id", ID)
                    delete.ExecuteNonQuery()

                    For Each form In Application.OpenForms
                        If TypeOf form Is Borrower Then
                            Dim borrower = DirectCast(form, Borrower)
                            borrower.cbstrandd()
                        End If
                    Next

                    MsgBox("Strand deleted successfully.", vbInformation)
                    Strand_Load(sender, e)
                    txtstrand.Clear()


                    Dim count As New MySqlCommand("SELECT COUNT(*) FROM `strand_tbl`", con)
                    Dim rowCount As Long = CLng(count.ExecuteScalar())

                    If rowCount = 0 Then

                        Dim reset As New MySqlCommand("ALTER TABLE `strand_tbl` AUTO_INCREMENT = 1", con)
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
            txtstrand.Text = row.Cells("Strand").Value.ToString

        End If

    End Sub

    Private Sub txtsearch_TextChanged(sender As Object, e As EventArgs) Handles txtsearch.TextChanged


        Dim dt As DataTable = DirectCast(DataGridView1.DataSource, DataTable)
        If dt IsNot Nothing Then
            If txtsearch.Text.Trim() <> "" Then
                Dim filter As String = String.Format("Strand LIKE '*{0}*'", txtsearch.Text.Trim())
                dt.DefaultView.RowFilter = filter
            Else
                dt.DefaultView.RowFilter = ""
            End If
        End If

    End Sub

    Private Sub txtstrand_KeyDown(sender As Object, e As KeyEventArgs) Handles txtstrand.KeyDown

        If e.Control AndAlso (e.KeyCode = Keys.V Or e.KeyCode = Keys.C Or e.KeyCode = Keys.X) Then
            e.SuppressKeyPress = True
        End If

        If e.KeyCode = Keys.Enter Then
            btnadd_Click(sender, e)
            e.Handled = True
        End If

    End Sub

    Private Sub txtstrand_KeyPress(sender As Object, e As KeyPressEventArgs) Handles txtstrand.KeyPress


        If Not Char.IsLetter(e.KeyChar) AndAlso Not Char.IsControl(e.KeyChar) AndAlso Not Char.IsWhiteSpace(e.KeyChar) Then
            e.Handled = True
        End If

    End Sub

    Private Sub txtsearch_KeyDown(sender As Object, e As KeyEventArgs) Handles txtsearch.KeyDown

        If e.Control AndAlso (e.KeyCode = Keys.V Or e.KeyCode = Keys.C Or e.KeyCode = Keys.X) Then
            e.SuppressKeyPress = True
        End If

    End Sub

    Private Sub btnadd_KeyDown(sender As Object, e As KeyEventArgs) Handles btnadd.KeyDown

        If e.KeyCode = Keys.Enter Then
            btnadd_Click(sender, e)
            e.Handled = True
        End If

    End Sub
End Class