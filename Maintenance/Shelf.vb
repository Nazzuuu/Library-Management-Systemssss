Imports System.Runtime.Serialization
Imports MySql.Data.MySqlClient
Imports System.Data

Public Class Shelf
    Private Sub Shelf_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        TopMost = True
        Me.Refresh()

        Dim con As New MySqlConnection(GlobalVarsModule.connectionString)
        Dim com As String = "SELECT * FROM `shelf_tbl`"
        Dim adap As New MySqlDataAdapter(com, con)
        Dim ds As New DataSet

        Try
            adap.Fill(ds, "INFO")
            DataGridView1.DataSource = ds.Tables("INFO")

            DataGridView1.EnableHeadersVisualStyles = False
            DataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(207, 58, 109)
            DataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White

            DataGridView1.Columns("ID").Visible = False
            DataGridView1.ClearSelection()
        Catch ex As Exception
            MsgBox($"Error loading data: {ex.Message}", vbCritical)
        End Try

    End Sub

    Private Sub btnadd_Click(sender As Object, e As EventArgs) Handles btnadd.Click

        Dim con As New MySqlConnection(GlobalVarsModule.connectionString)
        Dim shelf As String = txtshelf.Text.Trim
        Dim newID As Integer = 0

        If String.IsNullOrWhiteSpace(shelf) Then
            MsgBox("Please fill in the required fields (Shelf Number).", vbExclamation, "Missing Information")
            Exit Sub
        End If

        Try
            con.Open()

            Dim comsu As New MySqlCommand("SELECT COUNT(*) FROM `shelf_tbl` WHERE `Shelf` = @shelf", con)
            comsu.Parameters.AddWithValue("@shelf", shelf)
            Dim count As Integer = Convert.ToInt32(comsu.ExecuteScalar)

            If count > 0 Then
                MsgBox("This shelf is already exists.", vbExclamation)
                Exit Sub
            End If

            Dim com As New MySqlCommand("INSERT INTO `shelf_tbl`(`Shelf`) VALUES (@shelf); SELECT LAST_INSERT_ID();", con)
            com.Parameters.AddWithValue("@shelf", shelf)
            newID = Convert.ToInt32(com.ExecuteScalar())

            GlobalVarsModule.LogAudit(
                actionType:="ADD",
                formName:="SHELF FORM",
                description:=$"Added new shelf: {shelf}",
                recordID:=newID.ToString()
            )

            For Each form In Application.OpenForms
                If TypeOf form Is AuditTrail Then
                    DirectCast(form, AuditTrail).refreshaudit()
                End If
            Next

            For Each form In Application.OpenForms
                If TypeOf form Is Accession Then
                    Dim acs = DirectCast(form, Accession)
                    acs.shelfsu()
                End If
            Next

            MsgBox("Shelf added successfully!!", vbInformation)
            Shelf_Load(sender, e)
            txtshelf.Clear()
        Catch ex As Exception
            MsgBox(ex.Message, vbCritical)
        Finally
            If con.State = ConnectionState.Open Then con.Close()
        End Try
    End Sub

    Private Sub btnedit_Click(sender As Object, e As EventArgs) Handles btnedit.Click

        If DataGridView1.SelectedRows.Count > 0 Then

            Dim con As New MySqlConnection(GlobalVarsModule.connectionString)
            Dim selectedRow As DataGridViewRow = DataGridView1.SelectedRows(0)

            Dim ID As Integer = CInt(selectedRow.Cells("ID").Value)

            Dim oldShelf As String = selectedRow.Cells("Shelf").Value.ToString().Trim()
            Dim newShelf As String = txtshelf.Text.Trim

            If String.IsNullOrWhiteSpace(newShelf) Then
                MsgBox("Please fill in the required fields.", vbExclamation, "Missing Information")
                Exit Sub
            End If

            If oldShelf.Equals(newShelf, StringComparison.OrdinalIgnoreCase) Then
                MsgBox("No changes were made.", vbInformation)
                Exit Sub
            End If

            Try
                con.Open()

                Dim comsu As New MySqlCommand("SELECT COUNT(*) FROM `shelf_tbl` WHERE `Shelf` = @shelf AND `ID` <> @id", con)
                comsu.Parameters.AddWithValue("@shelf", newShelf)
                comsu.Parameters.AddWithValue("@id", ID)

                Dim count As Integer = Convert.ToInt32(comsu.ExecuteScalar)

                If count > 0 Then
                    MsgBox("This shelf already exists.", vbExclamation)
                    Exit Sub
                End If

                Dim com As New MySqlCommand("UPDATE `shelf_tbl` SET `Shelf` = @shelf WHERE `ID` = @id", con)
                com.Parameters.AddWithValue("@shelf", newShelf)
                com.Parameters.AddWithValue("@id", ID)
                com.ExecuteNonQuery()


                Dim comss As New MySqlCommand("UPDATE `acession_tbl` SET `Shelf` = @newShelf WHERE `Shelf` = @oldShelf", con)
                comss.Parameters.AddWithValue("@newShelf", newShelf)
                comss.Parameters.AddWithValue("@oldShelf", oldShelf)
                comss.ExecuteNonQuery()

                GlobalVarsModule.LogAudit(
                    actionType:="UPDATE",
                    formName:="SHELF FORM",
                    description:=$"Updated shelf ID {ID} from '{oldShelf}' to '{newShelf}'",
                    recordID:=ID.ToString(),
                    oldValue:=oldShelf,
                    newValue:=newShelf
                )

                For Each form In Application.OpenForms
                    If TypeOf form Is AuditTrail Then
                        DirectCast(form, AuditTrail).refreshaudit()
                    End If
                Next

                For Each form In Application.OpenForms
                    If TypeOf form Is Accession Then
                        Dim acs = DirectCast(form, Accession)
                        acs.shelfsu()
                        acs.RefreshAccessionData()
                    End If
                Next

                MsgBox("Update successfully!!", vbInformation)
                txtshelf.Clear()
                Shelf_Load(sender, e)
            Catch ex As Exception
                MsgBox(ex.Message, vbCritical)
            Finally
                If con.State = ConnectionState.Open Then con.Close()
            End Try
        Else
            MsgBox("Please select a row to edit.", vbExclamation)
        End If

    End Sub

    Private Sub btndelete_Click(sender As Object, e As EventArgs) Handles btndelete.Click

        If DataGridView1.SelectedRows.Count > 0 Then

            Dim dialogResult As DialogResult = MessageBox.Show("Are you sure you want to delete this shelf?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)

            If dialogResult = DialogResult.Yes Then

                Dim con As New MySqlConnection(GlobalVarsModule.connectionString)
                Dim selectedRow As DataGridViewRow = DataGridView1.SelectedRows(0)
                Dim ID As Integer = CInt(selectedRow.Cells("ID").Value)
                Dim shelf As String = selectedRow.Cells("Shelf").Value.ToString().Trim()

                Try
                    con.Open()

                    Dim acsCountCmd As New MySqlCommand("SELECT COUNT(*) FROM `acession_tbl` WHERE Shelf = @shelf", con)
                    acsCountCmd.Parameters.AddWithValue("@shelf", shelf)
                    Dim accessionCount As Integer = CInt(acsCountCmd.ExecuteScalar())

                    If accessionCount > 0 Then
                        MessageBox.Show("Cannot delete this shelf. They are assigned to " & accessionCount & " accession(s).", "Information", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        Return
                    End If


                    Dim delete As New MySqlCommand("DELETE FROM `shelf_tbl` WHERE `ID` = @id", con)
                    delete.Parameters.AddWithValue("@id", ID)
                    delete.ExecuteNonQuery()

                    GlobalVarsModule.LogAudit(
                        actionType:="DELETE",
                        formName:="SHELF FORM",
                        description:=$"Deleted shelf: {shelf}",
                        recordID:=ID.ToString()
                    )

                    For Each form In Application.OpenForms
                        If TypeOf form Is AuditTrail Then
                            DirectCast(form, AuditTrail).refreshaudit()
                        End If
                    Next

                    For Each form In Application.OpenForms
                        If TypeOf form Is Accession Then
                            Dim acss = DirectCast(form, Accession)
                            acss.shelfsu()
                        End If
                    Next

                    MsgBox("Shelf deleted successfully.", vbInformation)
                    Shelf_Load(sender, e)
                    txtshelf.Clear()

                    Dim count As New MySqlCommand("SELECT COUNT(*) FROM `shelf_tbl`", con)
                    Dim rowCount As Long = CLng(count.ExecuteScalar())

                    If rowCount = 0 Then
                        Dim reset As New MySqlCommand("ALTER TABLE `shelf_tbl` AUTO_INCREMENT = 1", con)
                        reset.ExecuteNonQuery()
                    End If

                Catch ex As Exception
                    MsgBox(ex.Message, vbCritical)
                Finally
                    If con.State = ConnectionState.Open Then con.Close()
                End Try
            End If
        End If

    End Sub

    Private Sub DataGridView1_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellClick

        If e.RowIndex >= 0 Then

            Dim row = DataGridView1.Rows(e.RowIndex)
            txtshelf.Text = row.Cells("Shelf").Value.ToString

        End If

    End Sub

    Private Sub txtshelf_KeyPress(sender As Object, e As KeyPressEventArgs) Handles txtshelf.KeyPress
        If Not Char.IsDigit(e.KeyChar) And Not Char.IsControl(e.KeyChar) Then
            e.Handled = True
        End If
    End Sub

    Private Sub txtshelf_KeyDown(sender As Object, e As KeyEventArgs) Handles txtshelf.KeyDown
        If e.Control AndAlso (e.KeyCode = Keys.V Or e.KeyCode = Keys.C Or e.KeyCode = Keys.X) Then
            e.SuppressKeyPress = True
        End If
    End Sub

    Private Sub Shelf_KeyDown(sender As Object, e As KeyEventArgs) Handles MyBase.KeyDown
        If e.KeyCode = Keys.Escape Then
            Me.Close()
        End If
    End Sub

    Private Sub txtsearch_TextChanged(sender As Object, e As EventArgs) Handles txtsearch.TextChanged

        Dim dt As DataTable = DirectCast(DataGridView1.DataSource, DataTable)
        If dt IsNot Nothing Then
            If txtsearch.Text.Trim() <> "" Then
                Dim filter As String = String.Format("Shelf LIKE '*{0}*'", txtsearch.Text.Trim())
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

    Private Sub Shelf_FormClosed(sender As Object, e As FormClosedEventArgs) Handles Me.FormClosed

        For Each form In Application.OpenForms
            If TypeOf form Is MainForm Then
                Dim load = DirectCast(form, MainForm)
                load.loadsu()
            End If
        Next

        MainForm.MaintenanceToolStripMenuItem.ForeColor = Color.White
        txtshelf.Text = ""

    End Sub

    Private Sub Shelf_Shown(sender As Object, e As EventArgs) Handles MyBase.Shown

        DataGridView1.ClearSelection()

    End Sub

    Private Sub txtshelf_Validating(sender As Object, e As System.ComponentModel.CancelEventArgs) Handles txtshelf.Validating

        Dim ShelfNumber As Integer

        If String.IsNullOrWhiteSpace(txtshelf.Text) Then
            e.Cancel = False
            Return
        End If

        If Integer.TryParse(txtshelf.Text.Trim(), ShelfNumber) Then

            If ShelfNumber < 1 Then
                MessageBox.Show("Shelf number must be 1 or higher. Zero is not allowed.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                e.Cancel = True
            Else
                e.Cancel = False
            End If
        Else

            MessageBox.Show("Invalid shelf number.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            e.Cancel = True
        End If
    End Sub

    Private Sub btnadd_MouseHover(sender As Object, e As EventArgs) Handles btnadd.MouseHover
        Cursor = Cursors.Hand
    End Sub

    Private Sub btnadd_MouseLeave(sender As Object, e As EventArgs) Handles btnadd.MouseLeave
        Cursor = Cursors.Default
    End Sub

    Private Sub btnedit_MouseHover(sender As Object, e As EventArgs) Handles btnedit.MouseHover
        Cursor = Cursors.Hand
    End Sub

    Private Sub btnedit_MouseLeave(sender As Object, e As EventArgs) Handles btnedit.MouseLeave
        Cursor = Cursors.Default
    End Sub

    Private Sub btndelete_MouseHover(sender As Object, e As EventArgs) Handles btndelete.MouseHover
        Cursor = Cursors.Hand
    End Sub

    Private Sub btndelete_MouseLeave(sender As Object, e As EventArgs) Handles btndelete.MouseLeave
        Cursor = Cursors.Default
    End Sub
End Class