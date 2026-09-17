Imports MySql.Data.MySqlClient
Imports System.Data
Imports System.Linq

Class PUR

    Private Sub PUR_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        refreshPUR()
        DataGridView1.EnableHeadersVisualStyles = False
        DataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(207, 58, 109)
        DataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White
        DataGridView1.ReadOnly = True
    End Sub

    Private Async Sub OnDatabaseUpdated()
        Try
            ' Load penalty records (overdue / damaged / lost) instead of pur_tbl which may be unused
            Dim q As String = "SELECT Borrower, FullName, ReturnedBook AS BookTitle, BorrowedDate, DueDate, ReturnDate, TransactionReceipt, Status " & _
                              "FROM penalty_tbl ORDER BY BorrowedDate DESC"
            Await GlobalVarsModule.LoadToGridAsync(DataGridView1, q)
        Catch
        End Try
    End Sub

    Public Sub refreshPUR(Optional filter As String = "NOT_PENALIZED")
        ' Show penalty records by default (these include statuses like Overdue, Damaged, Lost)
        Dim query As String = "SELECT Borrower, FullName, ReturnedBook AS BookTitle, BorrowedDate, DueDate, ReturnDate, TransactionReceipt, Status FROM penalty_tbl ORDER BY BorrowedDate DESC"

        GlobalVarsModule.AutoRefreshGrid(DataGridView1, query, 2000)

        Try
            AddHandler GlobalVarsModule.DatabaseUpdated, AddressOf OnDatabaseUpdated
        Catch
        End Try
    End Sub

    Private Sub DataGridView1_DataBindingComplete(sender As Object, e As DataGridViewBindingCompleteEventArgs) Handles DataGridView1.DataBindingComplete
        DataGridView1.ClearSelection()
        btnedit.Enabled = False
    End Sub

    Private Function GetStatusFromRow(row As DataGridViewRow) As String
        If row Is Nothing Then Return String.Empty

        For Each col As DataGridViewColumn In DataGridView1.Columns
            If col.Name IsNot Nothing AndAlso col.Name.ToLower().Contains("status") Then
                Return If(row.Cells(col.Index).Value, String.Empty).ToString().Trim()
            End If
            If col.HeaderText IsNot Nothing AndAlso col.HeaderText.ToLower().Contains("status") Then
                Return If(row.Cells(col.Index).Value, String.Empty).ToString().Trim()
            End If
        Next

        Return String.Empty
    End Function

    Private Sub DataGridView1_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellClick
        If e.RowIndex < 0 Then Return

        Dim row = DataGridView1.Rows(e.RowIndex)
        Dim status = GetStatusFromRow(row)
        btnedit.Enabled = Not String.IsNullOrWhiteSpace(status)
    End Sub

    Private Sub txtsearch_TextChanged(sender As Object, e As EventArgs) Handles txtsearch.TextChanged

        HandleAutoRefreshPause(DataGridView1, txtsearch)

        Dim dt As DataTable = TryCast(DataGridView1.DataSource, DataTable)
        If dt Is Nothing Then Return

        If txtsearch.Text.Trim() <> String.Empty Then
            Dim q = txtsearch.Text.Trim().Replace("'", "''")

            Dim candidates = New String() {"transactionreceipt", "TransactionReceipt", "transactionno", "TransactionNo", "Borrower", "Name", "Status", "status", "BookTitle", "transaction"}
            Dim filters As New List(Of String)
            For Each c In candidates
                If dt.Columns.Contains(c) Then
                    filters.Add(String.Format("CONVERT([{0}], System.String) LIKE '%{1}%'", c, q))
                End If
            Next

            If filters.Count > 0 Then
                dt.DefaultView.RowFilter = String.Join(" OR ", filters)
            Else
                dt.DefaultView.RowFilter = String.Empty
            End If
        Else
            dt.DefaultView.RowFilter = String.Empty
        End If
    End Sub

    Private Sub DataGridView1_MouseHover(sender As Object, e As EventArgs) Handles DataGridView1.MouseHover
        PauseAutoRefresh(DataGridView1)
    End Sub

    Private Sub datagridview1_mouseleave(sender As Object, e As EventArgs) Handles DataGridView1.MouseLeave
        ResumeAutoRefresh(DataGridView1)
    End Sub

    Private Sub btnedit_Click(sender As Object, e As EventArgs) Handles btnedit.Click

    End Sub

End Class
