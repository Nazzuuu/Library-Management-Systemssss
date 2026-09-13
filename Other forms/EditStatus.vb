Imports MySql.Data.MySqlClient
Imports System.Linq
Imports System.Data

Public Class EditStatus

    Public RowIndex As Integer = -1
    Public TargetRow As DataGridViewRow = Nothing

    Private Sub EditStatus_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        ' Optional:
        ' Kung gusto mong automatic na blank ang textbox every time
        txtauthor.Clear()
        txtauthor.Focus()

    End Sub


    ' ============================================================
    ' GET TARGET ROW FROM PUR FORM
    ' ============================================================
    Private Function GetTargetRow(purForm As PUR) As DataGridViewRow

        Try
            ' 1. TargetRow - pinaka reliable
            If Me.TargetRow IsNot Nothing Then
                Return Me.TargetRow
            End If

            ' 2. RowIndex
            If Me.RowIndex >= 0 AndAlso
               Me.RowIndex < purForm.DataGridView1.Rows.Count Then

                Return purForm.DataGridView1.Rows(Me.RowIndex)
            End If

            ' 3. SelectedRows
            If purForm.DataGridView1.SelectedRows.Count > 0 Then
                Return purForm.DataGridView1.SelectedRows(0)
            End If

        Catch
            ' Ignore
        End Try

        Return Nothing

    End Function


    ' ============================================================
    ' GET CELL VALUE SAFELY
    ' ============================================================
    Private Function GetCellValue(row As DataGridViewRow,
                                  columnName As String) As String

        Try

            If row Is Nothing Then
                Return String.Empty
            End If

            If row.DataGridView Is Nothing Then
                Return String.Empty
            End If

            If Not row.DataGridView.Columns.Contains(columnName) Then
                Return String.Empty
            End If

            Dim value As Object = row.Cells(columnName).Value

            If value Is Nothing OrElse IsDBNull(value) Then
                Return String.Empty
            End If

            Return value.ToString().Trim()

        Catch
            Return String.Empty
        End Try

    End Function


    ' ============================================================
    ' FIND COLUMN NAME CASE-INSENSITIVE
    ' ============================================================
    Private Function FindColumnName(grid As DataGridView,
                                    possibleNames As String()) As String

        Try

            For Each col As DataGridViewColumn In grid.Columns

                For Each name As String In possibleNames

                    If String.Equals(
                        col.Name,
                        name,
                        StringComparison.OrdinalIgnoreCase) Then

                        Return col.Name

                    End If

                Next

            Next

        Catch
            ' Ignore
        End Try

        Return String.Empty

    End Function


    ' ============================================================
    ' NORMALIZE STATUS
    ' ============================================================
    Private Function NormalizeStatus(value As String) As String

        If String.IsNullOrWhiteSpace(value) Then
            Return String.Empty
        End If

        Select Case value.Trim().ToLowerInvariant()

            Case "paid"
                Return "Paid"

            Case "unpaid"
                Return "Unpaid"

            Case "replaced"
                Return "Replaced"

            Case Else
                Return String.Empty

        End Select

    End Function


    ' ============================================================
    ' UPDATE DATAGRIDVIEW STATUS
    ' ============================================================
    Private Sub UpdateGridStatus(purForm As PUR,
                                 targetRow As DataGridViewRow,
                                 normalizedStatus As String)

        Try

            If purForm Is Nothing OrElse targetRow Is Nothing Then
                Return
            End If

            Dim grid As DataGridView = purForm.DataGridView1

            ' Find Status column
            Dim statusColumn As String =
                FindColumnName(
                    grid,
                    New String() {"Status", "status"}
                )

            If Not String.IsNullOrWhiteSpace(statusColumn) Then

                targetRow.Cells(statusColumn).Value = normalizedStatus

            End If


            ' ====================================================
            ' IF DATAGRIDVIEW IS BOUND TO DATATABLE
            ' UPDATE THE DATASOURCE ALSO
            ' ====================================================
            Try

                If TypeOf grid.DataSource Is DataTable Then

                    Dim dt As DataTable = CType(grid.DataSource, DataTable)

                    Dim drv As DataRowView =
                        TryCast(targetRow.DataBoundItem, DataRowView)

                    If drv IsNot Nothing Then

                        If dt.Columns.Contains(statusColumn) Then
                            drv(statusColumn) = normalizedStatus
                            drv.EndEdit()
                        End If

                    ElseIf targetRow.Index >= 0 AndAlso
                           targetRow.Index < dt.Rows.Count Then

                        If dt.Columns.Contains(statusColumn) Then
                            dt.Rows(targetRow.Index)(statusColumn) =
                                normalizedStatus
                        End If

                    End If

                End If

            Catch
                ' Ignore datasource update errors
            End Try


            grid.Refresh()

        Catch
            ' Ignore grid errors
        End Try

    End Sub


    ' ============================================================
    ' CHECK IF RECORD EXISTS
    '
    ' This prevents false "0 rows updated" when the status is
    ' already the same value.
    ' ============================================================
    Private Function RecordHasStatusByReceipt(
        transactionReceipt As String,
        expectedStatus As String) As Boolean

        If String.IsNullOrWhiteSpace(transactionReceipt) Then
            Return False
        End If

        Try

            Using con As New MySqlConnection(
                GlobalVarsModule.connectionString)

                con.Open()

                Dim sql As String =
                    "SELECT Status " &
                    "FROM pur_tbl " &
                    "WHERE TransactionReceipt = @receipt " &
                    "LIMIT 1"

                Using cmd As New MySqlCommand(sql, con)

                    cmd.Parameters.AddWithValue(
                        "@receipt",
                        transactionReceipt
                    )

                    Dim result As Object = cmd.ExecuteScalar()

                    If result IsNot Nothing AndAlso
                       Not IsDBNull(result) Then

                        Return String.Equals(
                            result.ToString().Trim(),
                            expectedStatus,
                            StringComparison.OrdinalIgnoreCase
                        )

                    End If

                End Using

            End Using

        Catch
            Return False
        End Try

        Return False

    End Function


    ' ============================================================
    ' UPDATE DATABASE BY TRANSACTION RECEIPT
    ' ============================================================
    Private Function UpdateByTransactionReceipt(
        transactionReceipt As String,
        normalizedStatus As String,
        ByRef rowsAffected As Integer) As Boolean

        rowsAffected = 0

        If String.IsNullOrWhiteSpace(transactionReceipt) Then
            Return False
        End If

        Try

            Using con As New MySqlConnection(
                GlobalVarsModule.connectionString)

                con.Open()

                Dim sql As String =
                    "UPDATE pur_tbl " &
                    "SET Status = @status " &
                    "WHERE TransactionReceipt = @receipt"

                Using cmd As New MySqlCommand(sql, con)

                    cmd.Parameters.AddWithValue(
                        "@status",
                        normalizedStatus
                    )

                    cmd.Parameters.AddWithValue(
                        "@receipt",
                        transactionReceipt
                    )

                    rowsAffected = cmd.ExecuteNonQuery()

                End Using

            End Using


            ' ====================================================
            ' IMPORTANT:
            ' ExecuteNonQuery can return 0 if the value is already
            ' the same. Check the actual database value.
            ' ====================================================
            If rowsAffected = 0 Then

                If RecordHasStatusByReceipt(
                    transactionReceipt,
                    normalizedStatus) Then

                    Return True

                End If

            End If


            Return rowsAffected > 0

        Catch ex As Exception

            Throw New Exception(
                "Error updating by TransactionReceipt: " &
                ex.Message,
                ex
            )

        End Try

    End Function


    ' ============================================================
    ' UPDATE DATABASE BY ID
    ' FALLBACK ONLY
    ' ============================================================
    Private Function UpdateByID(
        idValue As Object,
        normalizedStatus As String,
        ByRef rowsAffected As Integer) As Boolean

        rowsAffected = 0

        If idValue Is Nothing OrElse
           idValue Is DBNull.Value Then

            Return False

        End If

        Try

            Using con As New MySqlConnection(
                GlobalVarsModule.connectionString)

                con.Open()

                Dim sql As String =
                    "UPDATE pur_tbl " &
                    "SET Status = @status " &
                    "WHERE ID = @id"

                Using cmd As New MySqlCommand(sql, con)

                    cmd.Parameters.AddWithValue(
                        "@status",
                        normalizedStatus
                    )

                    cmd.Parameters.AddWithValue(
                        "@id",
                        idValue
                    )

                    rowsAffected = cmd.ExecuteNonQuery()

                End Using

            End Using

            Return rowsAffected > 0

        Catch ex As Exception

            Throw New Exception(
                "Error updating by ID: " &
                ex.Message,
                ex
            )

        End Try

    End Function


    ' ============================================================
    ' SAVE BUTTON
    ' ============================================================
    Private Sub btnsave_Click(
        sender As Object,
        e As EventArgs) Handles btnsave.Click

        Try

            ' ====================================================
            ' 1. VALIDATE STATUS (allow blank to clear)
            ' ====================================================
            Dim statusText As String = If(txtauthor.Text, String.Empty).ToString().Trim()
            Dim normalizedStatus As String = String.Empty

            If String.IsNullOrEmpty(statusText) Then
                ' empty -> user wants to clear the status
                normalizedStatus = String.Empty
            Else
                normalizedStatus = NormalizeStatus(statusText)
                If String.IsNullOrWhiteSpace(normalizedStatus) Then
                    MessageBox.Show(
                        "Status must be one of: Paid, Unpaid, or Replaced (or leave blank to clear).",
                        "Validation",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    )

                    txtauthor.SelectAll()
                    txtauthor.Focus()

                    Return
                End If
            End If


            ' ====================================================
            ' 2. FIND PUR FORM
            ' ====================================================
            Dim purForm As PUR =
                Application.OpenForms.
                OfType(Of PUR)().
                FirstOrDefault()

            If purForm Is Nothing Then

                MessageBox.Show(
                    "Unable to find the PUR form.",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                )

                Return

            End If


            ' ====================================================
            ' 3. GET SELECTED ROW
            ' ====================================================
            Dim targetRow As DataGridViewRow =
                GetTargetRow(purForm)

            If targetRow Is Nothing Then

                MessageBox.Show(
                    "Please select a record first.",
                    "No Record Selected",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                )

                Return

            End If


            ' ====================================================
            ' 4. GET TRANSACTION RECEIPT
            ' ====================================================
            Dim transactionReceipt As String = String.Empty

            Dim receiptColumn As String =
                FindColumnName(
                    purForm.DataGridView1,
                    New String() {
                        "TransactionReceipt",
                        "Transaction Receipt",
                        "TransactionReceiptNo",
                        "Receipt"
                    }
                )

            If Not String.IsNullOrWhiteSpace(receiptColumn) Then

                Dim receiptValue As Object =
                    targetRow.Cells(receiptColumn).Value

                If receiptValue IsNot Nothing AndAlso
                   Not IsDBNull(receiptValue) Then

                    transactionReceipt =
                        receiptValue.ToString().Trim()

                End If

            End If


            ' ====================================================
            ' 5. GET ID FROM TAG
            '    ID IS ONLY FALLBACK
            ' ====================================================
            Dim idValue As Object = Nothing

            If Me.Tag IsNot Nothing Then
                idValue = Me.Tag
            End If


            ' ====================================================
            ' 6. IF TAG IS EMPTY, TRY TO GET ID FROM GRID
            ' ====================================================
            If idValue Is Nothing Then

                Dim idColumn As String =
                    FindColumnName(
                        purForm.DataGridView1,
                        New String() {"ID", "Id", "id"}
                    )

                If Not String.IsNullOrWhiteSpace(idColumn) Then

                    Dim v As Object =
                        targetRow.Cells(idColumn).Value

                    If v IsNot Nothing AndAlso
                       Not IsDBNull(v) Then

                        idValue = v

                    End If

                End If

            End If


            ' ====================================================
            ' 7. REQUIRE EITHER TRANSACTION RECEIPT OR ID
            ' ====================================================
            If String.IsNullOrWhiteSpace(transactionReceipt) AndAlso
               idValue Is Nothing Then

                MessageBox.Show(
                    "The selected record has no TransactionReceipt or ID." &
                    vbCrLf & vbCrLf &
                    "Please check the selected record.",
                    "Missing Record ID",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                )

                Return

            End If


            ' ====================================================
            ' 8. UPDATE DATABASE
            '
            ' PRIORITY:
            ' TransactionReceipt FIRST
            ' ID SECOND
            ' ====================================================
            Dim saved As Boolean = False
            Dim rowsAffected As Integer = 0
            Dim saveMethod As String = String.Empty


            ' ----------------------------------------------------
            ' FIRST: TRANSACTION RECEIPT
            ' ----------------------------------------------------
            If Not String.IsNullOrWhiteSpace(transactionReceipt) Then

                saved =
                    UpdateByTransactionReceipt(
                        transactionReceipt,
                        normalizedStatus,
                        rowsAffected
                    )

                If saved Then
                    saveMethod = "TransactionReceipt"
                End If

            End If


            ' ----------------------------------------------------
            ' SECOND: ID FALLBACK
            ' ----------------------------------------------------
            If Not saved AndAlso idValue IsNot Nothing Then

                saved =
                    UpdateByID(
                        idValue,
                        normalizedStatus,
                        rowsAffected
                    )

                If saved Then
                    saveMethod = "ID"
                End If

            End If


            ' ====================================================
            ' 9. IF DATABASE SAVE FAILED
            ' ====================================================
            If Not saved Then

                Dim identifierMessage As String = String.Empty

                If Not String.IsNullOrWhiteSpace(
                    transactionReceipt) Then

                    identifierMessage =
                        "TransactionReceipt = " &
                        transactionReceipt

                ElseIf idValue IsNot Nothing Then

                    identifierMessage =
                        "ID = " &
                        idValue.ToString()

                End If

                MessageBox.Show(
                    "No record was updated." &
                    vbCrLf & vbCrLf &
                    identifierMessage &
                    vbCrLf & vbCrLf &
                    "Please check if the selected record really exists in pur_tbl.",
                    "Save Failed",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                )

                Return

            End If


            ' ====================================================
            ' 10. UPDATE DATAGRIDVIEW IMMEDIATELY
            ' ====================================================
            UpdateGridStatus(
                purForm,
                targetRow,
                normalizedStatus
            )


            ' ====================================================
            ' 11. REFRESH PUR FORM
            ' ====================================================
            Try

                purForm.refreshPUR("ALL")

            Catch refreshEx As Exception

                ' Database already saved.
                ' Don't treat refresh error as save failure.

            End Try


            ' ====================================================
            ' 12. SUCCESS MESSAGE
            ' ====================================================
            Dim statusLabel As String = If(String.IsNullOrWhiteSpace(normalizedStatus), "(cleared)", normalizedStatus)
            MessageBox.Show(
                "Status successfully saved." &
                vbCrLf & vbCrLf &
                "Status: " & statusLabel &
                vbCrLf &
                "Saved using: " & saveMethod &
                If(
                    Not String.IsNullOrWhiteSpace(
                        transactionReceipt),
                    vbCrLf &
                    "TransactionReceipt: " &
                    transactionReceipt,
                    String.Empty
                ),
                "Saved",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            )


            ' ====================================================
            ' 13. CLOSE EDIT STATUS FORM
            ' ====================================================
            Me.Close()


        Catch ex As Exception

            MessageBox.Show(
                "Error saving status:" &
                vbCrLf & vbCrLf &
                ex.Message,
                "Database Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error
            )

        End Try

    End Sub

End Class
