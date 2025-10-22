Imports MySql.Data.MySqlClient
Imports System.Data
Imports System.Drawing
Imports System.Collections.Generic

Public Class Penalty

    Private connectionString As String = GlobalVarsModule.connectionString
    Private originalCalculatedFee As Decimal = 0.00
    Private Const ABSOLUTE_MIN_FEE As Decimal = 50.0

    Private Sub Penalty_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ClearAllDetails()
        refreshpenalty()

        lblborrowerstatus.Text = "NOT PENALIZED"
    End Sub

    Public Sub refreshpenalty(Optional searchText As String = "")
        Dim con As New MySqlConnection(GlobalVarsModule.connectionString)
        Dim ds As New DataSet
        Dim com As String

        If String.IsNullOrWhiteSpace(searchText) Then
            com = "SELECT * FROM `penalty_tbl` ORDER BY ID DESC"
        Else
            com = "SELECT * FROM `penalty_tbl` WHERE `FullName` LIKE @search OR `LRN` LIKE @search OR `EmployeeNo` LIKE @search OR `TransactionReceipt` LIKE @search ORDER BY ID DESC"
        End If

        Try
            con.Open()
            Using adap As New MySqlDataAdapter(com, con)
                If Not String.IsNullOrWhiteSpace(searchText) Then
                    adap.SelectCommand.Parameters.AddWithValue("@search", searchText & "%")
                End If

                adap.Fill(ds, "INFO")
                DataGridView1.DataSource = ds.Tables("INFO")

                If DataGridView1.Rows.Count > 0 Then
                    If Not String.IsNullOrWhiteSpace(searchText) Then
                        DataGridView1.MultiSelect = True
                        DataGridView1.SelectAll()
                        DataGridView1.FirstDisplayedScrollingRowIndex = 0

                        LoadDetailsFromSelectedRow(DataGridView1.Rows(0))
                    Else
                        DataGridView1.ClearSelection()
                        DataGridView1.MultiSelect = False
                    End If
                Else
                    DataGridView1.ClearSelection()
                    DataGridView1.MultiSelect = False
                    ClearAllDetails()
                End If

                DataGridView1.Columns("ID").Visible = False
                DataGridView1.Columns("Grade").Visible = False
                DataGridView1.Columns("Section").Visible = False
                DataGridView1.Columns("Strand").Visible = False
                DataGridView1.Columns("BorrowedDate").Visible = False
                DataGridView1.Columns("DueDate").Visible = False
                DataGridView1.Columns("ReturnDate").Visible = False

                DataGridView1.DefaultCellStyle.WrapMode = DataGridViewTriState.False
                DataGridView1.EnableHeadersVisualStyles = False
                DataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(207, 58, 109)
                DataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White

            End Using
        Catch ex As Exception
            MessageBox.Show("Error loading penalty data: " & ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            If con.State = ConnectionState.Open Then con.Close()
        End Try
    End Sub

    Private Function GetAccessionIDsByTransaction(ByVal transactionNo As String, ByVal bookTitles As List(Of String)) As String
        If String.IsNullOrWhiteSpace(transactionNo) OrElse bookTitles Is Nothing OrElse bookTitles.Count = 0 Then Return String.Empty

        Dim con As New MySqlConnection(connectionString)
        Dim accessionIDs As New List(Of String)
        Dim conditionParts As New List(Of String)
        Dim i As Integer = 0

        For Each title In bookTitles
            conditionParts.Add($"`BookTitle` LIKE @title{i}")
            i += 1
        Next

        Dim query As String = $"SELECT DISTINCT `AccessionID` 
                                 FROM `borrowinghistory_tbl` 
                                 WHERE TRIM(`TransactionReceipt`) = @transNo 
                                 AND ({String.Join(" OR ", conditionParts)}) 
                                 AND `AccessionID` IS NOT NULL 
                                 AND `AccessionID` != ''"

        Try
            con.Open()
            Using cmd As New MySqlCommand(query, con)
                cmd.Parameters.AddWithValue("@transNo", transactionNo.Trim())

                i = 0
                For Each title In bookTitles
                    cmd.Parameters.AddWithValue($"@title{i}", "%" & title.Trim() & "%")
                    i += 1
                Next

                Using reader As MySqlDataReader = cmd.ExecuteReader()
                    While reader.Read()
                        Dim accessID As String = reader("AccessionID").ToString().Trim()
                        If Not String.IsNullOrWhiteSpace(accessID) Then
                            accessionIDs.Add(accessID)
                        End If
                    End While
                End Using
            End Using

        Catch ex As Exception
            Return "Error retrieving Accession IDs: " & ex.Message
        Finally
            If con.State = ConnectionState.Open Then con.Close()
        End Try

        Return String.Join(Environment.NewLine, New HashSet(Of String)(accessionIDs))
    End Function


    Private Function GetBookPriceByAccessionID(ByVal accessID As String) As Decimal
        If String.IsNullOrWhiteSpace(accessID) Then Return 0.00

        Dim con As New MySqlConnection(connectionString)
        Dim price As Decimal = 0.00
        Dim priceQuery As String = "SELECT T2.`BookPrice` " &
                                   "FROM `acession_tbl` T1 " &
                                   "INNER JOIN `acquisition_tbl` T2 ON T1.`TransactionNo` = T2.`TransactionNo` " &
                                   "WHERE T1.`AccessionID` = @accessID LIMIT 1"

        Try
            con.Open()
            Using cmd As New MySqlCommand(priceQuery, con)

                cmd.Parameters.AddWithValue("@accessID", accessID.Trim())

                Dim priceResult As Object = cmd.ExecuteScalar()

                If priceResult IsNot Nothing AndAlso priceResult IsNot DBNull.Value Then
                    If Decimal.TryParse(priceResult.ToString(), price) Then
                        Return price
                    End If
                End If
            End Using
        Catch ex As Exception
            MessageBox.Show("Error retrieving book price by Accession ID (JOIN failed). Check if 'TransactionID' is the correct join key: " & ex.Message, "Price Retrieval Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            If con.State = ConnectionState.Open Then con.Close()
        End Try

        Return 0.00
    End Function


    Private Function GetBookPriceByBookTitle(ByVal bookTitle As String) As Decimal
        If String.IsNullOrWhiteSpace(bookTitle) Then Return 0.00

        Dim con As New MySqlConnection(connectionString)
        Dim price As Decimal = 0.00
        Dim trimmedTitle As String = bookTitle.Trim()
        Dim priceQuery As String = "SELECT `BookPrice` FROM `acquisition_tbl` WHERE `BookTitle` LIKE @titleWildcard LIMIT 1"

        Try
            con.Open()
            Using cmd As New MySqlCommand(priceQuery, con)

                cmd.Parameters.AddWithValue("@titleWildcard", "%" & trimmedTitle & "%")

                Dim priceResult As Object = cmd.ExecuteScalar()

                If priceResult IsNot Nothing AndAlso priceResult IsNot DBNull.Value Then
                    If Decimal.TryParse(priceResult.ToString(), price) Then
                        Return price
                    End If
                End If
            End Using
        Catch ex As Exception
            MessageBox.Show("Error retrieving book price by Title: " & ex.Message, "Price Retrieval Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            If con.State = ConnectionState.Open Then con.Close()
        End Try

        Return 0.00
    End Function

    Private Function GetFixedPenaltyFeeByStatus(ByVal penaltyType As String) As Decimal
        If String.IsNullOrWhiteSpace(penaltyType) Then Return 0.00

        Dim con As New MySqlConnection(connectionString)
        Dim fee As Decimal = 0.00

        Dim query As String = "SELECT `Amount` FROM `penalty_management_tbl` WHERE `PenaltyType` = @type LIMIT 1"

        Try
            con.Open()
            Using cmd As New MySqlCommand(query, con)
                cmd.Parameters.AddWithValue("@type", penaltyType)
                Dim feeResult As Object = cmd.ExecuteScalar()
                If feeResult IsNot Nothing AndAlso feeResult IsNot DBNull.Value Then
                    If Decimal.TryParse(feeResult.ToString(), fee) Then
                        Return fee
                    End If
                End If
            End Using
        Catch ex As Exception
            MessageBox.Show("Error retrieving fixed penalty fee for " & penaltyType & ": " & ex.Message, "Fee Retrieval Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            If con.State = ConnectionState.Open Then con.Close()
        End Try

        Return 0.00
    End Function


    Private Function CalculateTotalPenaltyFee(ByVal transactionNo As String, ByVal allAccessionIDs As List(Of String)) As Decimal
        Dim totalFee As Decimal = 0.00
        Dim con As New MySqlConnection(connectionString)

        Dim query As String = "SELECT `Status`, `BookTotal`, `ReturnedBook` FROM `penalty_tbl` WHERE `TransactionReceipt` = @transNo"

        Dim accessIDIndex As Integer = 0

        Try
            con.Open()
            Using cmd As New MySqlCommand(query, con)
                cmd.Parameters.AddWithValue("@transNo", transactionNo)
                Using reader As MySqlDataReader = cmd.ExecuteReader()
                    While reader.Read()
                        Dim bookStatus As String = reader("Status").ToString()
                        Dim bookCount As Integer = CInt(reader("BookTotal"))
                        Dim currentFee As Decimal = 0.00

                        If bookStatus.StartsWith("Overdue") Then
                            currentFee = GetFixedPenaltyFeeByStatus("Overdue") * bookCount

                        ElseIf bookStatus.Contains("Damaged (Minor)") Then
                            currentFee = GetFixedPenaltyFeeByStatus("Damaged Book - Minor") * bookCount

                        ElseIf bookStatus.Contains("Damaged (Major)") Then
                            currentFee = GetFixedPenaltyFeeByStatus("Damaged Book - Major") * bookCount

                        ElseIf bookStatus.Contains("Damaged (Irreparable)") OrElse bookStatus.StartsWith("Lost") Then

                            For i As Integer = 1 To bookCount
                                If allAccessionIDs IsNot Nothing AndAlso allAccessionIDs.Count > accessIDIndex Then
                                    Dim currentAccessionID As String = allAccessionIDs(accessIDIndex)


                                    Dim bookPrice As Decimal = GetBookPriceByAccessionID(currentAccessionID)

                                    currentFee += bookPrice
                                    accessIDIndex += 1
                                Else

                                    Dim bookTitleForPrice As String = reader("ReturnedBook").ToString().Trim()
                                    Dim bookPrice As Decimal = GetBookPriceByBookTitle(bookTitleForPrice)
                                    currentFee += bookPrice
                                End If
                            Next
                        End If

                        totalFee += currentFee

                    End While
                End Using
            End Using

        Catch ex As Exception
            MessageBox.Show("Error calculating total penalty: " & ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return 0.00
        Finally
            If con.State = ConnectionState.Open Then con.Close()
        End Try

        Return totalFee
    End Function

    Private Function GetAllUniqueStatuses(ByVal transactionNo As String) As String
        Dim con As New MySqlConnection(connectionString)
        Dim statuses As New HashSet(Of String)
        Dim query As String = "SELECT DISTINCT `Status` FROM `penalty_tbl` WHERE `TransactionReceipt` = @transNo"

        Try
            con.Open()
            Using cmd As New MySqlCommand(query, con)
                cmd.Parameters.AddWithValue("@transNo", transactionNo)
                Using reader As MySqlDataReader = cmd.ExecuteReader()
                    While reader.Read()
                        statuses.Add(reader("Status").ToString())
                    End While
                End Using
            End Using

        Catch ex As Exception
            Return "Error: " & ex.Message
        Finally
            If con.State = ConnectionState.Open Then con.Close()
        End Try

        Return String.Join(Environment.NewLine, statuses.ToArray())
    End Function

    Private Sub ClearAllDetails()

        lbltransactionreceipt.Text = ".."
        lblborrowertype.Text = ".."
        lbllrn.Text = ".."
        lblemployeeno.Text = ".."
        lblfullname.Text = ".."
        lblgrade.Text = ".."
        lblsection.Text = ".."
        lblstrand.Text = ".."
        lbldepartment.Text = ".."
        lblborroweddate.Text = ".."
        lblduedate.Text = ".."
        lblbooktotal.Text = ".."
        lblaccessionid.Text = ".."
        lblbooktitle.Text = ".."
        lblbookstatus.Text = ".."
        lblborrowerstatus.Text = ".."
        txtfee.Text = String.Empty
        chkdisregard.Checked = False
        originalCalculatedFee = 0.00
        txtfee.ReadOnly = True
        chkdisregard.Enabled = False

    End Sub

    Private Function GetAggregatedBookDetails(ByVal transactionNo As String) As Tuple(Of String, Integer, List(Of String))
        Dim con As New MySqlConnection(connectionString)
        Dim titles As New List(Of String)
        Dim totalCount As Integer = 0
        Dim query As String = "SELECT `ReturnedBook`, `BookTotal` FROM `penalty_tbl` WHERE `TransactionReceipt` = @transNo"

        Try
            con.Open()
            Using cmd As New MySqlCommand(query, con)
                cmd.Parameters.AddWithValue("@transNo", transactionNo)
                Using reader As MySqlDataReader = cmd.ExecuteReader()
                    While reader.Read()
                        Dim returnedBookString As String = reader("ReturnedBook").ToString()
                        totalCount += CInt(reader("BookTotal"))

                        Dim bookTitles() As String = returnedBookString.Split(New Char() {"|"c}, StringSplitOptions.RemoveEmptyEntries)

                        For Each title As String In bookTitles
                            titles.Add(title.Trim())
                        Next
                    End While
                End Using
            End Using

        Catch ex As Exception
            MessageBox.Show("Error aggregating book details: " & ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return New Tuple(Of String, Integer, List(Of String))("Error", 0, New List(Of String))
        Finally
            If con.State = ConnectionState.Open Then con.Close()
        End Try

        Dim uniqueTitles As New HashSet(Of String)(titles)
        Return New Tuple(Of String, Integer, List(Of String))(String.Join(Environment.NewLine, uniqueTitles.ToArray()), totalCount, uniqueTitles.ToList())
    End Function

    Private Sub LoadDetailsFromSelectedRow(ByVal row As DataGridViewRow)
        If row Is Nothing Then Return

        originalCalculatedFee = 0.00

        Try
            Dim transNo As String = row.Cells("TransactionReceipt").Value.ToString()
            chkdisregard.Checked = False

            lbltransactionreceipt.Text = transNo
            lblborrowertype.Text = row.Cells("Borrower").Value.ToString()
            lbllrn.Text = If(row.Cells("LRN").Value Is DBNull.Value, "N/A", row.Cells("LRN").Value.ToString())
            lblemployeeno.Text = If(row.Cells("EmployeeNo").Value Is DBNull.Value, "N/A", row.Cells("EmployeeNo").Value.ToString())
            lblfullname.Text = row.Cells("FullName").Value.ToString()

            lbldepartment.Text = If(row.Cells("Department").Value Is DBNull.Value, "N/A", row.Cells("Department").Value.ToString())
            lblgrade.Text = If(row.Cells("Grade").Value Is DBNull.Value, "N/A", row.Cells("Grade").Value.ToString())
            lblsection.Text = If(row.Cells("Section").Value Is DBNull.Value, "N/A", row.Cells("Section").Value.ToString())
            lblstrand.Text = If(row.Cells("Strand").Value Is DBNull.Value, "N/A", row.Cells("Strand").Value.ToString())

            lblborroweddate.Text = row.Cells("BorrowedDate").Value.ToString()
            lblduedate.Text = row.Cells("DueDate").Value.ToString()

            Dim aggregatedDetails As Tuple(Of String, Integer, List(Of String)) = GetAggregatedBookDetails(transNo)
            Dim uniqueBookTitles As List(Of String) = aggregatedDetails.Item3

            lblbooktotal.Text = aggregatedDetails.Item2.ToString()
            lblbooktitle.Text = aggregatedDetails.Item1

            lblbookstatus.Text = GetAllUniqueStatuses(transNo)

            Dim borrowerStatusValue As Object = row.Cells("BorrowerStatus").Value
            Dim currentStatus As String = "NOT PENALIZED"
            If borrowerStatusValue IsNot DBNull.Value AndAlso borrowerStatusValue IsNot Nothing Then
                currentStatus = borrowerStatusValue.ToString().ToUpper()
            End If

            lblborrowerstatus.Text = currentStatus


            Dim accessionIDsString As String = GetAccessionIDsByTransaction(transNo, uniqueBookTitles)
            lblaccessionid.Text = accessionIDsString

            Dim allAccessionIDs As New List(Of String)
            If Not String.IsNullOrWhiteSpace(accessionIDsString) Then
                allAccessionIDs = accessionIDsString.Split(New String() {Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries).ToList()
            End If


            originalCalculatedFee = CalculateTotalPenaltyFee(transNo, allAccessionIDs)
            txtfee.Text = originalCalculatedFee.ToString("N2")

            txtfee.ReadOnly = True

            chkdisregard.Enabled = True
        Catch ex As Exception
            MessageBox.Show("Error loading details: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub txtsearch_TextChanged(sender As Object, e As EventArgs) Handles txtsearch.TextChanged
        Dim searchText As String = txtsearch.Text.Trim()

        If String.IsNullOrWhiteSpace(searchText) Then

            If DataGridView1.SelectedRows.Count > 0 Then
                DataGridView1.ClearSelection()
            End If

            ClearAllDetails()
            refreshpenalty()

        End If
    End Sub

    Private Sub txtsearch_KeyDown(sender As Object, e As KeyEventArgs) Handles txtsearch.KeyDown
        If e.KeyCode = Keys.Enter Then
            Dim searchText As String = txtsearch.Text.Trim()

            refreshpenalty(searchText)

            If String.IsNullOrWhiteSpace(searchText) Then

                ClearAllDetails()
                DataGridView1.ClearSelection()

            ElseIf DataGridView1.Rows.Count > 0 Then

                LoadDetailsFromSelectedRow(DataGridView1.Rows(0))
            Else

                ClearAllDetails()
            End If

            e.SuppressKeyPress = True
            e.Handled = True
        End If
    End Sub

    Private Sub chkdisregard_CheckedChanged(sender As Object, e As EventArgs) Handles chkdisregard.CheckedChanged
        If chkdisregard.Checked Then
            txtfee.Text = String.Empty
            txtfee.ReadOnly = False
            txtfee.Focus()
        Else
            txtfee.Text = originalCalculatedFee.ToString("N2")
            txtfee.ReadOnly = True
        End If
    End Sub

    Private Sub btnpenalized_Click(sender As Object, e As EventArgs) Handles btnpenalized.Click

        If lbltransactionreceipt.Text = ".." Then
            MessageBox.Show("Please select a transaction first.", "Missing Details", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        If lblborrowerstatus.Text = "PENALIZED" Then
            MessageBox.Show("This transaction is already penalized.", "Already Paid", MessageBoxButtons.OK, MessageBoxIcon.Information)
            ClearAllDetails()
            Return
        End If

        Dim enteredFee As Decimal
        If Not Decimal.TryParse(txtfee.Text.Replace(",", ""), enteredFee) Then
            MessageBox.Show("Invalid amount entered for Penalty Fee.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Error)
            txtfee.Focus()
            Return
        End If

        If enteredFee < ABSOLUTE_MIN_FEE Then
            MessageBox.Show($"The entered fee must not be less than {ABSOLUTE_MIN_FEE.ToString("N2")}.", "Fee Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            txtfee.Focus()
            Return
        End If

        Dim transNo As String = lbltransactionreceipt.Text

        Using con As New MySqlConnection(connectionString)
            Dim updateQuery As String = "UPDATE `penalty_tbl` SET `BorrowerStatus` = 'PENALIZED' WHERE `TransactionReceipt` = @transNo"

            Try
                con.Open()
                Using cmd As New MySqlCommand(updateQuery, con)
                    cmd.Parameters.AddWithValue("@transNo", transNo)
                    cmd.ExecuteNonQuery()
                End Using
            Catch ex As Exception
                MessageBox.Show("Database Update Error: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            Finally
                If con.State = ConnectionState.Open Then con.Close()
            End Try
        End Using

        Dim oldFeeStatus As String = If(chkdisregard.Checked, $"Disregarded Fee: {enteredFee.ToString("N2")}", $"Calculated Fee: {originalCalculatedFee.ToString("N2")}")

        GlobalVarsModule.LogAudit(
            actionType:="UPDATE",
            formName:="PENALTY PAYMENT",
            description:=$"Transaction {transNo} marked as PENALIZED.",
            recordID:=transNo,
            oldValue:=$"Status: NOT PENALIZED | Fee: {oldFeeStatus}",
            newValue:=$"Status: PENALIZED | Paid Amount: {enteredFee.ToString("N2")}"
        )
        For Each form In Application.OpenForms
            If TypeOf form Is AuditTrail Then
                DirectCast(form, AuditTrail).refreshaudit()
            End If
        Next

        MessageBox.Show($"Penalty of {enteredFee.ToString("N2")} for Transaction: {transNo} has been marked as PENALIZED.", "Penalty Applied", MessageBoxButtons.OK, MessageBoxIcon.Information)

        lblborrowerstatus.Text = "PENALIZED"

        ClearAllDetails()
        refreshpenalty()

    End Sub

    Private Sub penalty_shown(sender As Object, e As EventArgs) Handles MyBase.Shown

        DataGridView1.ClearSelection()

    End Sub
End Class