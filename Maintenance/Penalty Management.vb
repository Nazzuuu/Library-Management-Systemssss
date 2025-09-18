Imports MySql.Data.MySqlClient

Public Class Penalty_Management

    Dim selectrow As Integer

    Private Sub Penalty_Management_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        TopMost = True
        Me.Refresh()

        Dim con As New MySqlConnection(connectionString)
        Dim com As String = "SELECT * FROM `penalty_management_tbl`"
        Dim adap As New MySqlDataAdapter(com, con)
        Dim ds As New DataSet

        adap.Fill(ds, "INFO")
        DataGridView1.DataSource = ds.Tables("INFO")

        DataGridView1.EnableHeadersVisualStyles = False
        DataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(207, 58, 109)
        DataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White

        DataGridView1.Columns("ID").Visible = False
        DataGridView1.ClearSelection()

    End Sub

    Private Sub Penalty_Management_FormClosed(sender As Object, e As FormClosedEventArgs) Handles Me.FormClosed

        MainForm.MaintenanceToolStripMenuItem.ShowDropDown()
        MainForm.MaintenanceToolStripMenuItem.ForeColor = Color.Gray
        txtamount.Text = ""
        txtdescription.Text = ""
        cbpenaltytype.SelectedIndex = -1
        Me.Dispose()

    End Sub

    Private Sub txtsearch_TextChanged(sender As Object, e As EventArgs) Handles txtsearch.TextChanged

        Dim dt As DataTable = DirectCast(DataGridView1.DataSource, DataTable)
        If dt IsNot Nothing Then
            If txtsearch.Text.Trim() <> "" Then
                Dim filter As String = String.Format("PenaltyType LIKE '*{0}*'", txtsearch.Text.Trim())
                dt.DefaultView.RowFilter = filter
            Else
                dt.DefaultView.RowFilter = ""
            End If
        End If

    End Sub

    Private Sub txtsearch_KeyDown(sender As Object, e As KeyEventArgs) Handles txtsearch.KeyDown
        If e.Control AndAlso (e.KeyCode = Keys.V Or e.KeyCode = Keys.C Or e.KeyCode = Keys.X) Then
            e.SuppressKeyPress = True
        End If
    End Sub

    Private Sub txtamount_KeyDown(sender As Object, e As KeyEventArgs) Handles txtamount.KeyDown
        If e.Control AndAlso (e.KeyCode = Keys.V Or e.KeyCode = Keys.C Or e.KeyCode = Keys.X) Then
            e.SuppressKeyPress = True
        End If
    End Sub

    Private Sub txtamount_KeyPress(sender As Object, e As KeyPressEventArgs) Handles txtamount.KeyPress
        If Not Char.IsDigit(e.KeyChar) And Not Char.IsControl(e.KeyChar) And Not e.KeyChar = "." Then
            e.Handled = True
        End If

    End Sub

    Private Sub txtdescription_KeyDown(sender As Object, e As KeyEventArgs) Handles txtdescription.KeyDown
        If e.Control AndAlso (e.KeyCode = Keys.V Or e.KeyCode = Keys.C Or e.KeyCode = Keys.X) Then
            e.SuppressKeyPress = True
        End If
    End Sub

    Private Sub txtdescription_KeyPress(sender As Object, e As KeyPressEventArgs) Handles txtdescription.KeyPress

        If Not Char.IsLetter(e.KeyChar) AndAlso Not Char.IsControl(e.KeyChar) AndAlso Not Char.IsWhiteSpace(e.KeyChar) Then
            e.Handled = True
        End If

    End Sub

    Private Sub Penalty_Management_KeyDown(sender As Object, e As KeyEventArgs) Handles MyBase.KeyDown

        If e.KeyCode = Keys.Escape Then
            Me.Close()
        End If

    End Sub

    Public Sub clearlahat()

        txtamount.Text = ""
        txtdescription.Text = ""
        cbpenaltytype.SelectedIndex = -1
        DataGridView1.ClearSelection()

    End Sub

    Private Sub btnadd_Click(sender As Object, e As EventArgs) Handles btnadd.Click

        If String.IsNullOrWhiteSpace(cbpenaltytype.Text) Or String.IsNullOrWhiteSpace(txtamount.Text) Or String.IsNullOrWhiteSpace(txtdescription.Text) Then
            MessageBox.Show("Please fill out all fields.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Dim penaltyType = cbpenaltytype.Text
        Dim amount = Decimal.Parse(txtamount.Text)
        Dim description = txtdescription.Text

        Dim con As New MySqlConnection(connectionString)
        Dim com = "INSERT INTO `penalty_management_tbl` (`PenaltyType`, `Amount`, `Description`) VALUES (@type, @amount, @desc)"
        Dim cmd As New MySqlCommand(com, con)

        cmd.Parameters.AddWithValue("@type", penaltyType)
        cmd.Parameters.AddWithValue("@amount", amount)
        cmd.Parameters.AddWithValue("@desc", description)

        Try
            con.Open
            cmd.ExecuteNonQuery
            MessageBox.Show("Penalty added successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Penalty_Management_Load(sender, e)
            clearlahat
        Catch ex As Exception
            MessageBox.Show("Error adding penalty: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            con.Close
        End Try

    End Sub

    Private Sub btnedit_Click(sender As Object, e As EventArgs) Handles btnedit.Click

        If selectrow = 0 Then
            MessageBox.Show("Please select a record to edit.", "Selection Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        If String.IsNullOrWhiteSpace(cbpenaltytype.Text) Or String.IsNullOrWhiteSpace(txtamount.Text) Or String.IsNullOrWhiteSpace(txtdescription.Text) Then
            MessageBox.Show("Please fill out all fields.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Dim penaltyType = cbpenaltytype.Text
        Dim amount = Decimal.Parse(txtamount.Text)
        Dim description = txtdescription.Text

        Dim con As New MySqlConnection(connectionString)
        Dim com = "UPDATE `penalty_management_tbl` SET `PenaltyType` = @type, `Amount` = @amount, `Description` = @desc WHERE `ID` = @id"
        Dim cmd As New MySqlCommand(com, con)

        cmd.Parameters.AddWithValue("@type", penaltyType)
        cmd.Parameters.AddWithValue("@amount", amount)
        cmd.Parameters.AddWithValue("@desc", description)
        cmd.Parameters.AddWithValue("@id", selectrow)

        Try
            con.Open
            cmd.ExecuteNonQuery
            MessageBox.Show("Penalty updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Penalty_Management_Load(sender, e)
            clearlahat
        Catch ex As Exception
            MessageBox.Show("Error updating penalty: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            con.Close
        End Try

    End Sub

    Private Sub btndelete_Click(sender As Object, e As EventArgs) Handles btndelete.Click

        If DataGridView1.SelectedRows.Count > 0 Then

            Dim dialogResult = MessageBox.Show("Are you sure you want to delete this penalty?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)

            If dialogResult = DialogResult.Yes Then

                Dim con As New MySqlConnection(connectionString)
                Dim selectedRow = DataGridView1.SelectedRows(0)
                Dim ID As Integer = selectedRow.Cells("ID").Value
                Dim penaltyType = selectedRow.Cells("PenaltyType").Value.ToString.Trim

                Try
                    con.Open



                    Dim delete As New MySqlCommand("DELETE FROM `penalty_management_tbl` WHERE `ID` = @id", con)
                    delete.Parameters.AddWithValue("@id", ID)
                    delete.ExecuteNonQuery


                    Dim count As New MySqlCommand("SELECT COUNT(*) FROM `penalty_management_tbl`", con)
                    Dim rowCount As Long = count.ExecuteScalar()

                    If rowCount = 0 Then
                        Dim reset As New MySqlCommand("ALTER TABLE `penalty_management_tbl` AUTO_INCREMENT = 1", con)
                        reset.ExecuteNonQuery
                    End If

                    MessageBox.Show("Penalty deleted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)

                    Penalty_Management_Load(sender, e)
                    clearlahat

                Catch ex As Exception
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Finally
                    con.Close
                End Try
            End If
        Else
            MessageBox.Show("Please select a record to delete.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        End If

    End Sub


    Private Sub btnclear_Click(sender As Object, e As EventArgs) Handles btnclear.Click
        clearlahat
    End Sub

    Private Sub DataGridView1_CellClick_1(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellClick


        If e.RowIndex >= 0 Then
            Dim row = DataGridView1.Rows(e.RowIndex)

            selectrow = Convert.ToInt32(row.Cells("ID").Value)

            cbpenaltytype.Text = row.Cells("PenaltyType").Value.ToString
            txtamount.Text = row.Cells("Amount").Value.ToString
            txtdescription.Text = row.Cells("Description").Value.ToString

        End If


    End Sub
End Class