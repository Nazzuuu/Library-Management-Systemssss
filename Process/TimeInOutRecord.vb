Imports MySql.Data.MySqlClient
Imports System.Data
Imports System.Windows.Forms
Imports System.Drawing
Imports System.Linq

Public Class TimeInOutRecord


    Private ReadOnly connectionString As String = GlobalVarsModule.connectionString

    Private Sub TimeInOutRecord_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        refreshtimeoutrecrod()
    End Sub

    Public Sub refreshtimeoutrecrod()

        DataGridView1.DataSource = Nothing
        DataGridView1.Columns.Clear()

        Dim com As String = "SELECT " &
                        "o.`ID`, " &
                        "DATE_FORMAT(o.`TimeIn`, '%m/%d/%Y') AS `Date`, " &
                        "b.`Borrower`, " &
                        "CONCAT(b.`LastName`, ', ', b.`FirstName`, " &
                        "IF(b.`MiddleName` IS NULL OR b.`MiddleName` = '' OR UPPER(b.`MiddleName`) = 'N/A', '', CONCAT(' ', b.`MiddleName`))" &
                        ") AS `FullName`, " &
                        "TIME_FORMAT(o.`TimeIn`, '%I:%i %p') AS `TimeIn`, " &
                        "TIME_FORMAT(o.`TimeOut`, '%I:%i %p') AS `TimeOut` " &
                        "FROM `oras_tbl` o " &
                        "LEFT JOIN `borrower_tbl` b " &
                        "ON o.`LRN` = b.`LRN` OR o.`EmployeeNo` = b.`EmployeeNo` " &
                        "ORDER BY o.`TimeIn` DESC"

        Using con As New MySqlConnection(connectionString)
            Try
                con.Open()
                Using cmd As New MySqlCommand(com, con)
                    Dim adap As New MySqlDataAdapter(cmd)
                    Dim ds As New DataSet
                    adap.Fill(ds, "info")

                    DataGridView1.DataSource = ds.Tables("info")

                    If DataGridView1.Columns.Contains("ID") Then
                        DataGridView1.Columns("ID").Visible = False
                    End If


                    If DataGridView1.Columns.Contains("Date") Then DataGridView1.Columns("Date").HeaderText = "DATE"
                    If DataGridView1.Columns.Contains("Borrower") Then DataGridView1.Columns("Borrower").HeaderText = "BORROWER TYPE"
                    If DataGridView1.Columns.Contains("FullName") Then DataGridView1.Columns("FullName").HeaderText = "FULL NAME"
                    If DataGridView1.Columns.Contains("TimeIn") Then DataGridView1.Columns("TimeIn").HeaderText = "TIME IN" ' Headers are set here
                    If DataGridView1.Columns.Contains("TimeOut") Then
                        DataGridView1.Columns("TimeOut").HeaderText = "TIME OUT"
                        DataGridView1.Columns("TimeOut").Visible = True
                    End If

                    DataGridView1.ClearSelection()
                    DataGridView1.EnableHeadersVisualStyles = False
                    DataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(207, 58, 109)
                    DataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White

                End Using
            Catch ex As Exception
                MessageBox.Show("Error loading records: " & ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End Using

        DataGridView1.ReadOnly = True
        DataGridView1.SelectionMode = DataGridViewSelectionMode.CellSelect
    End Sub


    Private Sub DataGridView1_CellFormatting(sender As Object, e As DataGridViewCellFormattingEventArgs) Handles DataGridView1.CellFormatting

        If e.RowIndex >= 0 AndAlso e.Value IsNot DBNull.Value Then

            Dim colName As String = DataGridView1.Columns(e.ColumnIndex).Name


            If colName = "TimeIn" OrElse colName = "TimeOut" Then

                Try

                    If TypeOf e.Value Is DateTime Then

                        Dim dt As DateTime = CDate(e.Value)

                        e.Value = dt.ToLocalTime().ToString("hh:mm tt")
                        e.FormattingApplied = True
                    End If

                Catch

                End Try

            End If
        End If

    End Sub

    Private Sub recrod_shown(sender As Object, e As EventArgs) Handles MyBase.Shown
        DataGridView1.ClearSelection()

        lblnote.Visible = False
        lblmessage.Visible = False
    End Sub


    Private Sub chkSelectRecord_CheckedChanged(sender As Object, e As EventArgs) Handles chkSelectRecord.CheckedChanged

        If chkSelectRecord.Checked Then


            DataGridView1.MultiSelect = True
            DataGridView1.ClearSelection()
            DataGridView1.CurrentCell = Nothing
            DataGridView1.Enabled = True
            DataGridView1.ReadOnly = True
            DataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect

            lblnote.Visible = True
            lblmessage.Visible = True
            lblmessage.Text = "Select record and delete."


            Dim chkSelectAll As Control = Me.Controls.Find("chkSelectAll", True).FirstOrDefault()
            If chkSelectAll IsNot Nothing AndAlso TypeOf chkSelectAll Is CheckBox Then
                DirectCast(chkSelectAll, CheckBox).Enabled = False
                DirectCast(chkSelectAll, CheckBox).Checked = False
            End If

        Else



            DataGridView1.MultiSelect = False
            DataGridView1.ClearSelection()
            DataGridView1.CurrentCell = Nothing
            DataGridView1.Enabled = True

            lblnote.Visible = False
            lblmessage.Visible = False
            lblmessage.Text = ""
            DataGridView1.ReadOnly = True
            DataGridView1.SelectionMode = DataGridViewSelectionMode.CellSelect

            Dim chkSelectAll As Control = Me.Controls.Find("chkSelectAll", True).FirstOrDefault()
            If chkSelectAll IsNot Nothing AndAlso TypeOf chkSelectAll Is CheckBox Then
                DirectCast(chkSelectAll, CheckBox).Enabled = True
            End If

        End If
    End Sub


    Private Sub chkSelectAll_CheckedChanged(sender As Object, e As EventArgs) Handles chkSelectAll.CheckedChanged

        If chkSelectAll.Checked Then



            DataGridView1.MultiSelect = True
            DataGridView1.SelectAll()
            DataGridView1.CurrentCell = Nothing
            DataGridView1.Enabled = False


            lblnote.Visible = True
            lblmessage.Visible = True
            lblmessage.Text = "All records are selected for deletion. Click the Delete button to proceed."

            Dim chkSelectRecord As Control = Me.Controls.Find("chkSelectRecord", True).FirstOrDefault()
            If chkSelectRecord IsNot Nothing AndAlso TypeOf chkSelectRecord Is CheckBox Then
                DirectCast(chkSelectRecord, CheckBox).Enabled = False
                DirectCast(chkSelectRecord, CheckBox).Checked = False
            End If

        Else

            DataGridView1.ClearSelection()
            DataGridView1.MultiSelect = False
            DataGridView1.CurrentCell = Nothing


            lblnote.Visible = False
            lblmessage.Visible = False
            lblmessage.Text = ""
            DataGridView1.ReadOnly = True
            DataGridView1.Enabled = True
            DataGridView1.SelectionMode = DataGridViewSelectionMode.CellSelect

            Dim chkSelectRecord As Control = Me.Controls.Find("chkSelectRecord", True).FirstOrDefault()
            If chkSelectRecord IsNot Nothing AndAlso TypeOf chkSelectRecord Is CheckBox Then
                DirectCast(chkSelectRecord, CheckBox).Enabled = True
            End If

        End If
    End Sub

    Private Sub btndelete_Click(sender As Object, e As EventArgs) Handles btndelete.Click

        If DataGridView1.Rows.Count = 0 Then
            MessageBox.Show("There are no records to delete.", "Deletion Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Dim chkSelectRecordControl As Control = Me.Controls.Find("chkSelectRecord", True).FirstOrDefault()
        Dim chkSelectAllControl As Control = Me.Controls.Find("chkSelectAll", True).FirstOrDefault()

        Dim isSelectRecordChecked As Boolean = If(chkSelectRecordControl IsNot Nothing AndAlso TypeOf chkSelectRecordControl Is CheckBox, DirectCast(chkSelectRecordControl, CheckBox).Checked, False)
        Dim isSelectAllChecked As Boolean = If(chkSelectAllControl IsNot Nothing AndAlso TypeOf chkSelectAllControl Is CheckBox, DirectCast(chkSelectAllControl, CheckBox).Checked, False)

        Dim recordIDsToDelete As New List(Of Integer)
        Dim selectionMode As String = ""

        If isSelectAllChecked Then
            selectionMode = "All"
            If MessageBox.Show("Are you sure you want to permanently delete ALL Time-In/Out records? This action cannot be undone.", "Confirm Delete All", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) = DialogResult.No Then
                Return
            End If

            For Each row As DataGridViewRow In DataGridView1.Rows
                Dim idValue As Object = row.Cells("ID").Value
                Dim idInt As Integer
                If idValue IsNot DBNull.Value AndAlso Integer.TryParse(idValue.ToString(), idInt) Then
                    recordIDsToDelete.Add(idInt)
                End If
            Next

        ElseIf isSelectRecordChecked OrElse DataGridView1.SelectedRows.Count > 0 Then

            For Each row As DataGridViewRow In DataGridView1.SelectedRows
                Dim idValue As Object = row.Cells("ID").Value
                Dim idInt As Integer
                If idValue IsNot DBNull.Value AndAlso Integer.TryParse(idValue.ToString(), idInt) Then
                    recordIDsToDelete.Add(idInt)
                End If
            Next

            selectionMode = If(isSelectRecordChecked, "Selected", "Single")

        End If

        If recordIDsToDelete.Count = 0 Then
            MessageBox.Show("No records were selected for deletion. Please select a row.", "Deletion Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        If selectionMode <> "All" Then
            Dim deleteMsg As String = If(recordIDsToDelete.Count = 1, "Are you sure you want to delete the selected record?", $"Are you sure you want to delete these {recordIDsToDelete.Count} selected records?")
            If MessageBox.Show(deleteMsg, "Confirm Deletion", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) = DialogResult.No Then
                Return
            End If
        End If

        Dim successCount As Integer = 0
        Dim idList As String = String.Join(",", recordIDsToDelete)

        Using con As New MySqlConnection(connectionString)
            Dim com As String = $"DELETE FROM `oras_tbl` WHERE `ID` IN ({idList})"

            Using cmd As New MySqlCommand(com, con)
                Try
                    con.Open()
                    successCount = cmd.ExecuteNonQuery()
                Catch ex As Exception
                    MessageBox.Show($"Database error during deletion: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Return
                End Try
            End Using
        End Using

        If successCount > 0 Then
            MessageBox.Show($"{successCount} record(s) deleted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
            chkSelectAll.Checked = False
            chkSelectRecord.Checked = False
            refreshtimeoutrecrod()
        Else
            MessageBox.Show("Deletion failed or no records were found.", "Deletion Failed", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End If

    End Sub
End Class