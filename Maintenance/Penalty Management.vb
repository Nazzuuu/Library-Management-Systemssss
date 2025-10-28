Imports MySql.Data.MySqlClient
Imports System.Data
Imports System.Drawing

Public Class Penalty_Management


    Dim selectrow As Integer

    Private Function IsPenaltyTypeExist(ByVal penaltyType As String, Optional ByVal excludeId As Integer = 0) As Boolean
        Dim con As New MySqlConnection(GlobalVarsModule.connectionString)
        Dim query As String = "SELECT COUNT(*) FROM `penalty_management_tbl` WHERE `PenaltyType` = @type AND `ID` <> @id"
        Dim count As Integer = 0

        Try
            con.Open()
            Using cmd As New MySqlCommand(query, con)
                cmd.Parameters.AddWithValue("@type", penaltyType)
                cmd.Parameters.AddWithValue("@id", excludeId)
                count = Convert.ToInt32(cmd.ExecuteScalar())
            End Using
        Catch ex As Exception
            MessageBox.Show("Error checking penalty type redundancy: " & ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            If con.State = ConnectionState.Open Then con.Close()
        End Try

        Return count > 0
    End Function

    Private Function IsValidAmount(ByVal amountText As String) As Boolean
        Dim amount As Decimal
        If Not Decimal.TryParse(amountText, amount) Then
            Return False
        End If
        Return amount > 0
    End Function

    Private Sub Penalty_Management_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        TopMost = True
        Me.Refresh()

        Dim con As New MySqlConnection(GlobalVarsModule.connectionString)
        Dim com As String = "SELECT * FROM `penalty_management_tbl`"
        Dim adap As New MySqlDataAdapter(com, con)
        Dim ds As New DataSet

        adap.Fill(ds, "INFO")
        DataGridView1.DataSource = ds.Tables("INFO")

        DataGridView1.EnableHeadersVisualStyles = False
        DataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(207, 58, 109)
        DataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White

        DataGridView1.Columns("ID").Visible = False


    End Sub

    Private Sub Penalty_Management_Shown(sender As Object, e As EventArgs) Handles MyBase.Shown

        DataGridView1.ClearSelection()

    End Sub

    Private Sub Penalty_Management_FormClosed(sender As Object, e As FormClosedEventArgs) Handles Me.FormClosed

        Dim activeMain As MainForm = GlobalVarsModule.ActiveMainForm
        If activeMain Is Nothing OrElse activeMain.IsDisposed Then
            activeMain = New MainForm()
            GlobalVarsModule.ActiveMainForm = activeMain
            activeMain.Show()
        End If

        activeMain.MaintenanceToolStripMenuItem.ShowDropDown()
        activeMain.MaintenanceToolStripMenuItem.ForeColor = Color.Gray
        txtamount.Text = ""
        txtdescription.Text = ""
        cbpenaltytype.SelectedIndex = -1


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
        selectrow = 0

    End Sub

    Private Sub btnadd_Click(sender As Object, e As EventArgs) Handles btnadd.Click

        If String.IsNullOrWhiteSpace(cbpenaltytype.Text) Or String.IsNullOrWhiteSpace(txtamount.Text) Or String.IsNullOrWhiteSpace(txtdescription.Text) Then
            MessageBox.Show("Please fill out all fields.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        If Not IsValidAmount(txtamount.Text) Then
            MessageBox.Show("Amount must be a positive number (greater than zero).", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Dim penaltyType = cbpenaltytype.Text

        If IsPenaltyTypeExist(penaltyType) Then
            MessageBox.Show("Penalty Type '" & penaltyType & "' already exists. Penalty types must be unique.", "Redundancy Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Dim amount = Decimal.Parse(txtamount.Text)
        Dim description = txtdescription.Text
        Dim newID As Integer = 0

        Dim con As New MySqlConnection(GlobalVarsModule.connectionString)
        Dim com = "INSERT INTO `penalty_management_tbl` (`PenaltyType`, `Amount`, `Description`) VALUES (@type, @amount, @desc); SELECT LAST_INSERT_ID();"
        Dim cmd As New MySqlCommand(com, con)

        cmd.Parameters.AddWithValue("@type", penaltyType)
        cmd.Parameters.AddWithValue("@amount", amount)
        cmd.Parameters.AddWithValue("@desc", description)

        Try
            con.Open()
            newID = Convert.ToInt32(cmd.ExecuteScalar())

            GlobalVarsModule.LogAudit(
                actionType:="ADD",
                formName:="PENALTY MANAGEMENT",
                description:=$"Added new penalty: {penaltyType}",
                recordID:=newID.ToString()
            )

            For Each form In Application.OpenForms
                If TypeOf form Is AuditTrail Then
                    DirectCast(form, AuditTrail).refreshaudit()
                End If
            Next

            MessageBox.Show("Penalty added successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Penalty_Management_Load(sender, e)
            clearlahat()
        Catch ex As Exception
            MessageBox.Show("Error adding penalty: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            If con.State = ConnectionState.Open Then con.Close()
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

        If Not IsValidAmount(txtamount.Text) Then
            MessageBox.Show("Amount must be a positive number (greater than zero).", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Dim penaltyType = cbpenaltytype.Text

        If IsPenaltyTypeExist(penaltyType, selectrow) Then
            MessageBox.Show("Penalty Type '" & penaltyType & "' already exists. Penalty types must be unique.", "Redundancy Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Dim amount = Decimal.Parse(txtamount.Text)
        Dim description = txtdescription.Text

        ' Get old values for logging
        Dim oldRow As DataGridViewRow = DataGridView1.Rows.Cast(Of DataGridViewRow)().
            Where(Function(r) Convert.ToInt32(r.Cells("ID").Value) = selectrow).FirstOrDefault()

        Dim oldPenaltyType As String = oldRow.Cells("PenaltyType").Value.ToString()
        Dim oldAmount As Decimal = Convert.ToDecimal(oldRow.Cells("Amount").Value)
        Dim oldDescription As String = oldRow.Cells("Description").Value.ToString()

        ' Check if there are actual changes
        If oldPenaltyType.Equals(penaltyType) And oldAmount.Equals(amount) And oldDescription.Equals(description) Then
            MessageBox.Show("No changes were made.", "No Update", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If

        Dim con As New MySqlConnection(GlobalVarsModule.connectionString)
        Dim com = "UPDATE `penalty_management_tbl` SET `PenaltyType` = @type, `Amount` = @amount, `Description` = @desc WHERE `ID` = @id"
        Dim cmd As New MySqlCommand(com, con)

        cmd.Parameters.AddWithValue("@type", penaltyType)
        cmd.Parameters.AddWithValue("@amount", amount)
        cmd.Parameters.AddWithValue("@desc", description)
        cmd.Parameters.AddWithValue("@id", selectrow)

        Try
            con.Open()
            cmd.ExecuteNonQuery()

            GlobalVarsModule.LogAudit(
                actionType:="UPDATE",
                formName:="PENALTY MANAGEMENT",
                description:=$"Updated penalty ID {selectrow} from '{oldPenaltyType}' to '{penaltyType}'",
                recordID:=selectrow.ToString(),
                oldValue:=$"Type: {oldPenaltyType}, Amount: {oldAmount}, Description: {oldDescription}",
                newValue:=$"Type: {penaltyType}, Amount: {amount}, Description: {description}"
            )

            For Each form In Application.OpenForms
                If TypeOf form Is AuditTrail Then
                    DirectCast(form, AuditTrail).refreshaudit()
                End If
            Next

            MessageBox.Show("Penalty updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Penalty_Management_Load(sender, e)
            clearlahat()
        Catch ex As Exception
            MessageBox.Show("Error updating penalty: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            If con.State = ConnectionState.Open Then con.Close()
        End Try

    End Sub


    Private Sub btnclear_Click(sender As Object, e As EventArgs) Handles btnclear.Click
        clearlahat()
    End Sub

    Private Sub DataGridView1_CellClick_1(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellClick


        If e.RowIndex >= 0 Then
            Dim row = DataGridView1.Rows(e.RowIndex)

            selectrow = Convert.ToInt32(row.Cells("ID").Value)

            cbpenaltytype.Text = row.Cells("PenaltyType").Value.ToString()
            txtamount.Text = row.Cells("Amount").Value.ToString()
            txtdescription.Text = row.Cells("Description").Value.ToString()

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



    Private Sub btnclear_MouseHover(sender As Object, e As EventArgs) Handles btnclear.MouseHover
        Cursor = Cursors.Hand
    End Sub

    Private Sub btnclear_MouseLeave(sender As Object, e As EventArgs) Handles btnclear.MouseLeave
        Cursor = Cursors.Default
    End Sub

End Class