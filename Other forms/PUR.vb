Imports MySql.Data.MySqlClient
Imports System.Data
Imports System.Linq

Public Class PUR

    '==========================================================
    ' CURRENT FILTER
    '==========================================================
    Private _currentFilter As String = "NOT PAID"


    Private Async Sub PUR_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        DataGridView1.EnableHeadersVisualStyles = False

        DataGridView1.ColumnHeadersDefaultCellStyle.BackColor =
        Color.FromArgb(207, 58, 109)

        DataGridView1.ColumnHeadersDefaultCellStyle.ForeColor =
        Color.White

        DataGridView1.ReadOnly = True


        SyncNotPenalizedToPUR()
        SyncPaymentStatusToPUR()
        Await GlobalVarsModule.LoadToGridAsync(
        DataGridView1,
        BuildPurQuery(_currentFilter)
    )

        GlobalVarsModule.AutoRefreshGrid(
            DataGridView1,
            BuildPurQuery(_currentFilter),
            2000
        )

    End Sub


    Private Sub SyncNotPenalizedToPUR()

        Dim query As String =
            "INSERT INTO `pur_tbl` " &
            "(`Borrower`, `Fullname`, `Department`, `BookTitle`, `Status`, `TransactionReceipt`) " &
            "SELECT " &
            "r.`Borrower`, " &
            "r.`Fullname`, " &
            "r.`Department`, " &
            "r.`ReturnedBook`, " &
            "'NOT PAID', " &
            "r.`TransactionReceipt` " &
            "FROM `returning_tbl` r " &
            "WHERE UPPER(TRIM(r.`BorrowerStatus`)) = 'NOT PENALIZED' " &
            "AND r.`ReturnedBook` NOT LIKE '%|%' " &
            "AND NOT EXISTS ( " &
            "    SELECT 1 " &
            "    FROM `pur_tbl` p " &
            "    WHERE TRIM(p.`TransactionReceipt`) = TRIM(r.`TransactionReceipt`) " &
            ")"

        Try

            Using conn As New MySqlConnection(
                GlobalVarsModule.connectionString)

                conn.Open()

                Using cmd As New MySqlCommand(query, conn)

                    cmd.ExecuteNonQuery()

                End Using

            End Using

        Catch ex As Exception

            MessageBox.Show(
                "Error synchronizing returning records to PUR." &
                Environment.NewLine &
                Environment.NewLine &
                ex.Message,
                "PUR Synchronization Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error
            )

        End Try

    End Sub

    Private Sub SyncPaymentStatusToPUR()

        Try

            Using conn As New MySqlConnection(
                GlobalVarsModule.connectionString)

                conn.Open()

                Dim deleteCombinedQuery As String =
                    "DELETE FROM `pur_tbl` " &
                    "WHERE `BookTitle` LIKE '%|%'"

                Using deleteCombinedCmd As New MySqlCommand(
                    deleteCombinedQuery, conn)

                    deleteCombinedCmd.ExecuteNonQuery()

                End Using

                Dim transactionColumn As String = Nothing

                Dim transactionColumns As String() = {
                    "TransactionReceipt",
                    "TransactionNo",
                    "TransactionNumber",
                    "Transaction"
                }

                For Each col As String In transactionColumns

                    Dim checkQuery As String =
                        "SELECT COUNT(*) " &
                        "FROM INFORMATION_SCHEMA.COLUMNS " &
                        "WHERE TABLE_SCHEMA = DATABASE() " &
                        "AND TABLE_NAME = 'pstudent_tbl' " &
                        "AND COLUMN_NAME = @columnName"

                    Using checkCmd As New MySqlCommand(
                        checkQuery, conn)

                        checkCmd.Parameters.AddWithValue(
                            "@columnName",
                            col
                        )

                        Dim exists As Integer =
                            Convert.ToInt32(
                                checkCmd.ExecuteScalar()
                            )

                        If exists > 0 Then

                            transactionColumn = col
                            Exit For

                        End If

                    End Using

                Next


                Dim paymentColumn As String = Nothing

                Dim paymentColumns As String() = {
                    "Remarks",
                    "Remark",
                    "PaymentStatus",
                    "Payment",
                    "PaidStatus",
                    "Status"
                }

                For Each col As String In paymentColumns

                    Dim checkQuery As String =
                        "SELECT COUNT(*) " &
                        "FROM INFORMATION_SCHEMA.COLUMNS " &
                        "WHERE TABLE_SCHEMA = DATABASE() " &
                        "AND TABLE_NAME = 'pstudent_tbl' " &
                        "AND COLUMN_NAME = @columnName"

                    Using checkCmd As New MySqlCommand(
                        checkQuery, conn)

                        checkCmd.Parameters.AddWithValue(
                            "@columnName",
                            col
                        )

                        Dim exists As Integer =
                            Convert.ToInt32(
                                checkCmd.ExecuteScalar()
                            )

                        If exists > 0 Then

                            paymentColumn = col
                            Exit For

                        End If

                    End Using

                Next

                If String.IsNullOrWhiteSpace(transactionColumn) Then

                    Return

                End If

                If String.IsNullOrWhiteSpace(paymentColumn) Then

                    Return

                End If

                Dim updatePaidQuery As String =
                    "UPDATE `pur_tbl` p " &
                    "INNER JOIN `pstudent_tbl` ps " &
                    "ON TRIM(p.`TransactionReceipt`) = " &
                    "TRIM(ps.`" & transactionColumn & "`) " &
                    "SET p.`Status` = 'PAID' " &
                    "WHERE UPPER(TRIM(ps.`" & paymentColumn & "`)) = 'PAID' " &
                    "AND UPPER(TRIM(p.`Status`)) = 'NOT PAID'"

                Using cmd As New MySqlCommand(
                    updatePaidQuery, conn)

                    cmd.ExecuteNonQuery()

                End Using

                Dim updateReplacedQuery As String =
                    "UPDATE `pur_tbl` p " &
                    "INNER JOIN `pstudent_tbl` ps " &
                    "ON TRIM(p.`TransactionReceipt`) = " &
                    "TRIM(ps.`" & transactionColumn & "`) " &
                    "SET p.`Status` = 'REPLACED BOOK' " &
                    "WHERE UPPER(TRIM(ps.`" & paymentColumn & "`)) = 'REPLACED BOOK' " &
                    "AND UPPER(TRIM(p.`Status`)) = 'NOT PAID'"

                Using cmd As New MySqlCommand(
                    updateReplacedQuery, conn)

                    cmd.ExecuteNonQuery()

                End Using

            End Using

        Catch ex As Exception

            MessageBox.Show(
                "Error synchronizing PUR payment status." &
                Environment.NewLine &
                Environment.NewLine &
                ex.Message,
                "PUR Status Synchronization Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error
            )

        End Try

    End Sub


    Private Function BuildPurQuery(
        ByVal filter As String
    ) As String

        '==========================================================
        ' GROUP SAME TRANSACTION RECEIPT INTO ONE ROW
        '
        ' Example:
        '
        ' Holy Father
        ' Ibong Adarna
        ' Ibong Lawin
        '
        ' becomes:
        '
        ' Holy Father | Ibong Adarna | Ibong Lawin
        '
        '==========================================================

        If filter IsNot Nothing AndAlso
           filter.Trim().ToUpper() = "PAID" Then

            Return "SELECT " &
                   "MAX(`Borrower`) AS `Borrower`, " &
                   "MAX(`Fullname`) AS `Fullname`, " &
                   "MAX(`Department`) AS `Department`, " &
                   "GROUP_CONCAT(DISTINCT NULLIF(TRIM(`BookTitle`), '') " &
                   "ORDER BY TRIM(`BookTitle`) ASC SEPARATOR ' | ') AS `BookTitle`, " &
                   "MAX(`Status`) AS `Status`, " &
                   "`TransactionReceipt` " &
                   "FROM `pur_tbl` " &
                   "WHERE UPPER(TRIM(`Status`)) IN " &
                   "('PAID', 'REPLACED BOOK') " &
                   "GROUP BY `TransactionReceipt` " &
                   "ORDER BY MAX(`ID`) DESC"

        Else

            Return "SELECT " &
                   "MAX(`Borrower`) AS `Borrower`, " &
                   "MAX(`Fullname`) AS `Fullname`, " &
                   "MAX(`Department`) AS `Department`, " &
                   "GROUP_CONCAT(DISTINCT NULLIF(TRIM(`BookTitle`), '') " &
                   "ORDER BY TRIM(`BookTitle`) ASC SEPARATOR ' | ') AS `BookTitle`, " &
                   "MAX(`Status`) AS `Status`, " &
                   "`TransactionReceipt` " &
                   "FROM `pur_tbl` " &
                   "WHERE UPPER(TRIM(`Status`)) = 'NOT PAID' " &
                   "GROUP BY `TransactionReceipt` " &
                   "ORDER BY MAX(`ID`) DESC"

        End If

    End Function


    Private Async Sub OnDatabaseUpdated()

        Try

            SyncNotPenalizedToPUR()
            SyncPaymentStatusToPUR()

            Await GlobalVarsModule.LoadToGridAsync(
                DataGridView1,
                BuildPurQuery(_currentFilter)
            )

        Catch

        End Try

    End Sub


    Public Sub refreshPUR(
        Optional filter As String = "NOT PAID"
    )

        _currentFilter = filter

        SyncNotPenalizedToPUR()
        SyncPaymentStatusToPUR()

        Dim query As String =
            BuildPurQuery(_currentFilter)

        GlobalVarsModule.AutoRefreshGrid(
            DataGridView1,
            query,
            2000
        )


        Try
            RemoveHandler GlobalVarsModule.DatabaseUpdated,
                AddressOf OnDatabaseUpdated
        Catch
        End Try

        Try
            AddHandler GlobalVarsModule.DatabaseUpdated,
                AddressOf OnDatabaseUpdated
        Catch
        End Try

    End Sub


    Private Sub DataGridView1_DataBindingComplete(
        sender As Object,
        e As DataGridViewBindingCompleteEventArgs
    )

        Dim purColumns = {
            "Borrower",
            "Fullname",
            "Department",
            "BookTitle",
            "Status",
            "TransactionReceipt"
        }

        For i = DataGridView1.Columns.Count - 1 To 0 Step -1

            Dim column = DataGridView1.Columns(i)
            Dim columnName = column.Name
            Dim propertyName = column.DataPropertyName

            Dim keepColumn =
                purColumns.Any(
                    Function(x)
                        Return String.Equals(
                            x,
                            columnName,
                            StringComparison.OrdinalIgnoreCase
                        ) OrElse
                        String.Equals(
                            x,
                            propertyName,
                            StringComparison.OrdinalIgnoreCase
                        )
                    End Function
                )

            If Not keepColumn Then
                DataGridView1.Columns.RemoveAt(i)
            End If

        Next

        DataGridView1.ClearSelection()

        'btnedit.Enabled = False

    End Sub


    Private Function GetStatusFromRow(
        row As DataGridViewRow
    ) As String

        If row Is Nothing Then
            Return String.Empty
        End If

        For Each col As DataGridViewColumn In DataGridView1.Columns

            If col.Name IsNot Nothing AndAlso
               col.Name.ToLower().Contains("status") Then

                Return If(
                    row.Cells(col.Index).Value,
                    String.Empty
                ).ToString().Trim()

            End If

            If col.HeaderText IsNot Nothing AndAlso
               col.HeaderText.ToLower().Contains("status") Then

                Return If(
                    row.Cells(col.Index).Value,
                    String.Empty
                ).ToString().Trim()

            End If

        Next

        Return String.Empty

    End Function


    Private Function GetTransactionReceiptFromRow(
        row As DataGridViewRow
    ) As String

        If row Is Nothing Then
            Return String.Empty
        End If

        For Each col As DataGridViewColumn In DataGridView1.Columns

            If col.Name IsNot Nothing AndAlso
               col.Name.ToLower().Contains("transactionreceipt") Then

                Return If(
                    row.Cells(col.Index).Value,
                    String.Empty
                ).ToString().Trim()

            End If

            If col.HeaderText IsNot Nothing AndAlso
               col.HeaderText.ToLower().Contains("transactionreceipt") Then

                Return If(
                    row.Cells(col.Index).Value,
                    String.Empty
                ).ToString().Trim()

            End If

        Next

        Return String.Empty

    End Function


    Private Sub DataGridView1_CellClick(
        sender As Object,
        e As DataGridViewCellEventArgs
    )

        If e.RowIndex < 0 Then
            Return
        End If

        Dim row =
            DataGridView1.Rows(e.RowIndex)

        Dim status =
            GetStatusFromRow(row)

        ''btnedit.Enabled =
        '    Not String.IsNullOrWhiteSpace(status)

    End Sub


    Private Sub txtsearch_TextChanged(
        sender As Object,
        e As EventArgs
    ) Handles txtsearch.TextChanged

        HandleAutoRefreshPause(
            DataGridView1,
            txtsearch
        )

        Dim dt As DataTable =
            TryCast(
                DataGridView1.DataSource,
                DataTable
            )

        If dt Is Nothing Then
            Return
        End If

        If txtsearch.Text.Trim() <> String.Empty Then

            Dim q As String =
                txtsearch.Text.Trim().Replace("'", "''")

            Dim candidates =
                New String() {
                    "TransactionReceipt",
                    "transactionreceipt",
                    "TransactionNo",
                    "transactionno",
                    "Borrower",
                    "Fullname",
                    "Name",
                    "Department",
                    "Status",
                    "status",
                    "BookTitle",
                    "transaction"
                }

            Dim filters As New List(Of String)

            For Each c In candidates

                If dt.Columns.Contains(c) Then

                    filters.Add(
                        String.Format(
                            "CONVERT([{0}], System.String) LIKE '%{1}%'",
                            c,
                            q
                        )
                    )

                End If

            Next

            If filters.Count > 0 Then

                dt.DefaultView.RowFilter =
                    String.Join(" OR ", filters)

            Else

                dt.DefaultView.RowFilter =
                    String.Empty

            End If

        Else

            dt.DefaultView.RowFilter =
                String.Empty

        End If

    End Sub


    Private Sub DataGridView1_MouseHover(
        sender As Object,
        e As EventArgs
    )

        PauseAutoRefresh(DataGridView1)

    End Sub


    Private Sub DataGridView1_MouseLeave(
        sender As Object,
        e As EventArgs
    )

        ResumeAutoRefresh(DataGridView1)

    End Sub


    'Private Sub btnedit_Click(
    '    sender As Object,
    '    e As EventArgs
    ') Handles btnedit.Click

    '    If DataGridView1.CurrentRow Is Nothing Then

    '        MessageBox.Show(
    '            "Please select a record first.",
    '            "No Record Selected",
    '            MessageBoxButtons.OK,
    '            MessageBoxIcon.Information
    '        )

    '        Return

    '    End If

    '    Dim row As DataGridViewRow =
    '        DataGridView1.CurrentRow

    '    Dim status As String =
    '        GetStatusFromRow(row)

    '    Dim transactionReceipt As String =
    '        GetTransactionReceiptFromRow(row)

    '    If String.IsNullOrWhiteSpace(transactionReceipt) Then

    '        MessageBox.Show(
    '            "Transaction Receipt was not found.",
    '            "Invalid Record",
    '            MessageBoxButtons.OK,
    '            MessageBoxIcon.Warning
    '        )

    '        Return

    '    End If

    '    If status.Trim().ToUpper() = "PAID" Then

    '        MessageBox.Show(
    '            "This transaction is already marked as PAID.",
    '            "Already Paid",
    '            MessageBoxButtons.OK,
    '            MessageBoxIcon.Information
    '        )

    '        Return

    '    End If

    '    If status.Trim().ToUpper() <> "NOT PAID" Then

    '        MessageBox.Show(
    '            "Only NOT PAID records can be marked as PAID.",
    '            "Invalid Status",
    '            MessageBoxButtons.OK,
    '            MessageBoxIcon.Warning
    '        )

    '        Return

    '    End If

    '    Dim result As DialogResult =
    '        MessageBox.Show(
    '            "Mark this transaction as PAID?" &
    '            Environment.NewLine &
    '            Environment.NewLine &
    '            "Transaction Receipt: " &
    '            transactionReceipt,
    '            "Confirm Payment",
    '            MessageBoxButtons.YesNo,
    '            MessageBoxIcon.Question
    '        )

    '    If result <> DialogResult.Yes Then
    '        Return
    '    End If

    '    MarkAsPaid(transactionReceipt)

    'End Sub


    Private Sub MarkAsPaid(
        ByVal transactionReceipt As String
    )

        Dim query As String =
            "UPDATE `pur_tbl` " &
            "SET `Status` = 'PAID' " &
            "WHERE TRIM(`TransactionReceipt`) = TRIM(@TransactionReceipt) " &
            "AND UPPER(TRIM(`Status`)) = 'NOT PAID'"

        Try

            Using conn As New MySqlConnection(
                GlobalVarsModule.connectionString)

                conn.Open()

                Using cmd As New MySqlCommand(
                    query,
                    conn)

                    cmd.Parameters.AddWithValue(
                        "@TransactionReceipt",
                        transactionReceipt
                    )

                    Dim affectedRows As Integer =
                        cmd.ExecuteNonQuery()

                    If affectedRows > 0 Then

                        MessageBox.Show(
                            "Payment successfully recorded." &
                            Environment.NewLine &
                            Environment.NewLine &
                            "Transaction Receipt: " &
                            transactionReceipt &
                            Environment.NewLine &
                            "Status: PAID",
                            "Payment Recorded",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information
                        )

                    Else

                        MessageBox.Show(
                            "The record was not updated." &
                            Environment.NewLine &
                            Environment.NewLine &
                            "It may already be PAID or the transaction " &
                            "receipt does not exist.",
                            "Update Failed",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning
                        )

                    End If

                End Using

            End Using

            refreshPUR(_currentFilter)

        Catch ex As Exception

            MessageBox.Show(
                "Unable to update payment status." &
                Environment.NewLine &
                Environment.NewLine &
                ex.Message,
                "Payment Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error
            )

        End Try

    End Sub

End Class