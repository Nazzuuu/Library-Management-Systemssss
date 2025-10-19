Imports MySql.Data.MySqlClient
Imports System.Data
Imports System.Drawing

Public Class Penalty

    Private connectionString As String = GlobalVarsModule.connectionString
    Private AccessionID As String = String.Empty

    Private Sub Penalty_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ClearAllDetails()
        refreshpenalty()
    End Sub

    Public Sub refreshpenalty(Optional searchText As String = "")
        Dim con As New MySqlConnection(connectionString)
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
                    adap.SelectCommand.Parameters.AddWithValue("@search", "%" & searchText & "%")
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

    Private Function GetAccessionIDsByTransaction(ByVal transactionNo As String) As String
        Dim con As New MySqlConnection(connectionString)
        Dim accessionIDs As New List(Of String)

        Dim query As String = "SELECT `AccessionID` FROM `borrowinghistory_tbl` WHERE `TransactionReceipt` = @transNo"

        Try
            con.Open()
            Using cmd As New MySqlCommand(query, con)
                cmd.Parameters.AddWithValue("@transNo", transactionNo)
                Using reader As MySqlDataReader = cmd.ExecuteReader()
                    While reader.Read()
                        accessionIDs.Add(reader("AccessionID").ToString())
                    End While
                End Using
            End Using

        Catch ex As Exception
            Return "Error retrieving Accession IDs: " & ex.Message
        Finally
            If con.State = ConnectionState.Open Then con.Close()
        End Try

        Return String.Join(Environment.NewLine, accessionIDs)
    End Function

    Private Function GetBookPriceByBookTitle(ByVal bookTitle As String) As Decimal
        If String.IsNullOrWhiteSpace(bookTitle) Then Return 0.00

        Dim con As New MySqlConnection(connectionString)
        Dim price As Decimal = 0.00

        Dim priceQuery As String = "SELECT `BookPrice` FROM `acquisition_tbl` WHERE `BookTitle` = @title LIMIT 1"

        Try
            con.Open()
            Using cmd As New MySqlCommand(priceQuery, con)
                cmd.Parameters.AddWithValue("@title", bookTitle)
                Dim priceResult As Object = cmd.ExecuteScalar()
                If priceResult IsNot Nothing AndAlso priceResult IsNot DBNull.Value Then
                    If Decimal.TryParse(priceResult.ToString(), price) Then
                        Return price
                    End If
                End If
            End Using
        Catch ex As Exception
            MessageBox.Show("Error retrieving book price by title: " & ex.Message, "Price Retrieval Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
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

    Private Function CalculateTotalPenaltyFee(ByVal transactionNo As String) As Decimal
        Dim totalFee As Decimal = 0.00
        Dim con As New MySqlConnection(connectionString)

        Dim query As String = "SELECT `Status`, `BookTotal`, `ReturnedBook` FROM `penalty_tbl` WHERE `TransactionReceipt` = @transNo"

        Try
            con.Open()
            Using cmd As New MySqlCommand(query, con)
                cmd.Parameters.AddWithValue("@transNo", transactionNo)
                Using reader As MySqlDataReader = cmd.ExecuteReader()
                    While reader.Read()
                        Dim bookStatus As String = reader("Status").ToString()
                        Dim bookCount As Integer = CInt(reader("BookTotal"))
                        Dim bookTitleForPrice As String = reader("ReturnedBook").ToString()
                        Dim currentFee As Decimal = 0.00

                        If bookStatus.StartsWith("Overdue") Then
                            currentFee = GetFixedPenaltyFeeByStatus("Overdue")

                        ElseIf bookStatus.Contains("Damaged (Minor)") Then
                            currentFee = GetFixedPenaltyFeeByStatus("Damaged Book - Minor")

                        ElseIf bookStatus.Contains("Damaged (Major)") Then
                            currentFee = GetFixedPenaltyFeeByStatus("Damaged Book - Major")

                        ElseIf bookStatus.Contains("Damaged (Irreparable)") OrElse bookStatus.StartsWith("Lost") Then

                            currentFee = GetBookPriceByBookTitle(bookTitleForPrice)

                        End If

                        totalFee += (currentFee * bookCount)

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
        lbldepartment.Text = ".."
        lblgrade.Text = ".."
        lblsection.Text = ".."
        lblstrand.Text = ".."

        lblborroweddate.Text = ".."
        lblduedate.Text = ".."
        lblbooktotal.Text = ".."
        lblaccessionid.Text = ".."

        lblbooktitle.Text = ".."
        lblbookstatus.Text = ".."
        lblborrowerstatus.Text = ".."
        txtfee.Text = String.Empty
    End Sub

    Private Function GetAggregatedBookDetails(ByVal transactionNo As String) As Tuple(Of String, Integer)
        Dim con As New MySqlConnection(connectionString)
        Dim titles As New HashSet(Of String)
        Dim totalCount As Integer = 0
        Dim query As String = "SELECT `ReturnedBook`, `BookTotal` FROM `penalty_tbl` WHERE `TransactionReceipt` = @transNo"

        Try
            con.Open()
            Using cmd As New MySqlCommand(query, con)
                cmd.Parameters.AddWithValue("@transNo", transactionNo)
                Using reader As MySqlDataReader = cmd.ExecuteReader()
                    While reader.Read()
                        titles.Add(reader("ReturnedBook").ToString())
                        totalCount += CInt(reader("BookTotal"))
                    End While
                End Using
            End Using

        Catch ex As Exception
            MessageBox.Show("Error aggregating book details: " & ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return New Tuple(Of String, Integer)("Error", 0)
        Finally
            If con.State = ConnectionState.Open Then con.Close()
        End Try

        Return New Tuple(Of String, Integer)(String.Join(Environment.NewLine, titles.ToArray()), totalCount)
    End Function

    Private Sub LoadDetailsFromSelectedRow(ByVal row As DataGridViewRow)
        If row Is Nothing Then Return

        Me.AccessionID = String.Empty

        Try
            Dim transNo As String = row.Cells("TransactionReceipt").Value.ToString()

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

            Dim aggregatedDetails As Tuple(Of String, Integer) = GetAggregatedBookDetails(transNo)

            lblbooktotal.Text = aggregatedDetails.Item2.ToString()
            lblbooktitle.Text = aggregatedDetails.Item1

            lblbookstatus.Text = GetAllUniqueStatuses(transNo)
            lblborrowerstatus.Text = "NOT PENALIZED"

            Dim calculatedFee As Decimal = CalculateTotalPenaltyFee(transNo)
            txtfee.Text = calculatedFee.ToString("N2")

            lblaccessionid.Text = GetAccessionIDsByTransaction(transNo)

        Catch ex As Exception
            MessageBox.Show("Error loading details: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub


    Private Sub txtsearch_TextChanged(sender As Object, e As EventArgs) Handles txtsearch.TextChanged
        Dim searchText As String = txtsearch.Text.Trim()

        refreshpenalty(searchText)

        If String.IsNullOrWhiteSpace(searchText) Then
            ClearAllDetails()
            refreshpenalty()
        ElseIf DataGridView1.Rows.Count > 0 Then
            LoadDetailsFromSelectedRow(DataGridView1.Rows(0))
        Else
            ClearAllDetails()
        End If
    End Sub

    Private Sub txtsearch_KeyDown(sender As Object, e As KeyEventArgs) Handles txtsearch.KeyDown
        If e.KeyCode = Keys.Enter Then
            If DataGridView1.Rows.Count > 0 Then
                LoadDetailsFromSelectedRow(DataGridView1.Rows(0))
                e.SuppressKeyPress = True
                e.Handled = True
            End If
        End If
    End Sub


    Private Sub Penalty_KeyDown(sender As Object, e As KeyEventArgs) Handles MyBase.KeyDown
        If e.KeyCode = Keys.Escape Then
            Me.Close()
        End If
    End Sub

    Private Sub btnpenalized_Click(sender As Object, e As EventArgs) Handles btnpenalized.Click

    End Sub


End Class