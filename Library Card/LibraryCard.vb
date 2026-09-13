Imports MySql.Data.MySqlClient

Public Class LibraryCard
    Private SelectedIDs As New List(Of Integer)
    Private isRefreshing As Boolean = False

    Private Sub LibraryCard_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Dim query As String = "SELECT * FROM borrower_tbl WHERE Borrower = 'Student'"
        GlobalVarsModule.AutoRefreshGrid(dgvLibraryCard, query, 2000)


        dgvLibraryCard.EnableHeadersVisualStyles = False
        dgvLibraryCard.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(207, 58, 109)
        dgvLibraryCard.ColumnHeadersDefaultCellStyle.ForeColor = Color.White
    End Sub

    Private Sub dgvLibraryCard_DataBindingComplete(sender As Object, e As DataGridViewBindingCompleteEventArgs) Handles dgvLibraryCard.DataBindingComplete
        isRefreshing = True

        If Not dgvLibraryCard.Columns.Contains("chkSelect") Then
            Dim chk As New DataGridViewCheckBoxColumn()
            chk.Name = "chkSelect"
            chk.HeaderText = ""
            chk.Width = 40
            dgvLibraryCard.Columns.Insert(0, chk)
        End If


        If dgvLibraryCard.Columns.Contains("Department") Then
            Dim col = dgvLibraryCard.Columns("Department")
            col.Width = 180
            col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft
        End If

        For Each row As DataGridViewRow In dgvLibraryCard.Rows
            If Not row.IsNewRow Then
                Dim currentID As Integer = CInt(row.Cells("ID").Value)
                row.Cells("chkSelect").Value = SelectedIDs.Contains(currentID)
            End If
        Next

        isRefreshing = False
    End Sub

    Private Sub UpdateSelectedIDs(rowIndex As Integer)
        Dim row = dgvLibraryCard.Rows(rowIndex)
        Dim id As Integer = CInt(row.Cells("ID").Value)
        Dim cellValue = row.Cells("chkSelect").Value

        Dim isChecked As Boolean = False
        If cellValue IsNot Nothing AndAlso Not IsDBNull(cellValue) Then
            isChecked = CBool(cellValue)
        End If

        If isChecked Then
            If Not SelectedIDs.Contains(id) Then SelectedIDs.Add(id)
        Else
            If SelectedIDs.Contains(id) Then SelectedIDs.Remove(id)
        End If
    End Sub

    Private Sub dgvLibraryCard_CellValueChanged(sender As Object, e As DataGridViewCellEventArgs) Handles dgvLibraryCard.CellValueChanged
        If isRefreshing OrElse e.RowIndex < 0 Then Return

        If dgvLibraryCard.Columns(e.ColumnIndex).Name = "chkSelect" Then
            Dim row = dgvLibraryCard.Rows(e.RowIndex)
            Dim id As Integer = CInt(row.Cells("ID").Value)

            Dim isChecked As Boolean = False
            If row.Cells("chkSelect").Value IsNot Nothing AndAlso Not IsDBNull(row.Cells("chkSelect").Value) Then
                isChecked = CBool(row.Cells("chkSelect").Value)
            End If

            If isChecked Then
                If Not SelectedIDs.Contains(id) Then SelectedIDs.Add(id)
            Else
                If SelectedIDs.Contains(id) Then SelectedIDs.Remove(id)
            End If
        End If
    End Sub

    Private Sub dgvLibraryCard_CurrentCellDirtyStateChanged(sender As Object, e As EventArgs) Handles dgvLibraryCard.CurrentCellDirtyStateChanged
        If dgvLibraryCard.IsCurrentCellDirty Then
            dgvLibraryCard.CommitEdit(DataGridViewDataErrorContexts.Commit)
        End If
    End Sub

    Private Sub chkSelectAll_CheckedChanged(sender As Object, e As EventArgs) Handles chkSelectAll.CheckedChanged
        isRefreshing = True

        Dim isSelected As Boolean = chkSelectAll.Checked

        For Each row As DataGridViewRow In dgvLibraryCard.Rows
            If Not row.IsNewRow Then
                Dim id As Integer = CInt(row.Cells("ID").Value)

                row.Cells("chkSelect").Value = isSelected
                If isSelected Then
                    If Not SelectedIDs.Contains(id) Then SelectedIDs.Add(id)
                Else
                    SelectedIDs.Remove(id)
                End If
            End If
        Next

        isRefreshing = False
    End Sub

    Private Sub dgvLibraryCard_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles dgvLibraryCard.CellClick
        If e.RowIndex >= 0 AndAlso dgvLibraryCard.Columns(e.ColumnIndex).Name <> "chkSelect" Then

            Dim row As DataGridViewRow = dgvLibraryCard.Rows(e.RowIndex)
            Dim cell = row.Cells("chkSelect")
            Dim currentState As Boolean = False
            If cell.Value IsNot Nothing AndAlso Not IsDBNull(cell.Value) Then
                currentState = CBool(cell.Value)
            End If

            cell.Value = Not currentState

            UpdateSelectedIDs(e.RowIndex)
        End If
    End Sub

    Private Sub btnPrint_Click(sender As Object, e As EventArgs) Handles btnPrint.Click
        If SelectedIDs.Count = 0 Then
            MessageBox.Show("Please select at least one borrower to print.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Dim printForm As New PrintLibraryCard(SelectedIDs)
        printForm.ShowDialog()
    End Sub
End Class