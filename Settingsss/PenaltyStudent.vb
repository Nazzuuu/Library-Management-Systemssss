Imports System.Data
Imports MySql.Data.MySqlClient

Public Class PenaltyStudents

    Private IsLoadingTransaction As Boolean = False
    Private Const MIN_LENGTH As Integer = 12
    Private _defaultFee As Decimal = 0D
    Private _currentBooksList As New List(Of String)
    Private _borrowerDepartment As String = ""

    Private Sub PenaltyStudents_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        InitializeRemarksAndSort()
        RefreshPenaltyData()
        clear_details_only()
    End Sub

    Private Sub InitializeRemarksAndSort()
        If cbremarks.Items.Count = 0 Then
            cbremarks.Items.Add("Paid")
            cbremarks.Items.Add("Replaced Book")
            cbremarks.Items.Add("Not Paid")
        End If

        'If cbsort.Items.Count = 0 Then
        '    cbsort.Items.Add("Not Penalized")
        '    cbsort.Items.Add("Penalized")
        'End If
        'cbsort.SelectedIndex = 0
    End Sub

    Public Sub RefreshPenaltyData()
        Dim statusFilter As String = "NOT PENALIZED"

        'If cbsort.SelectedItem IsNot Nothing Then
        '    Select Case cbsort.SelectedItem.ToString()
        '        Case "Not Penalized" : statusFilter = "NOT PENALIZED"
        '        Case "Penalized" : statusFilter = "PENALIZED"
        '    End Select
        'End If

        Dim con As New MySqlConnection(GlobalVarsModule.connectionString)

        Dim com As String =
            "SELECT " &
            "MAX(`ID`) AS `ID`, " &
            "MAX(`Borrower`) AS `Borrower`, " &
            "MAX(`LRN`) AS `LRN`, " &
            "MAX(`EmployeeNo`) AS `EmployeeNo`, " &
            "MAX(`FullName`) AS `FullName`, " &
            "MAX(`Department`) AS `Department`, " &
            "MAX(`Grade`) AS `Grade`, " &
            "MAX(`Section`) AS `Section`, " &
            "MAX(`Strand`) AS `Strand`, " &
            "MAX(`DueDate`) AS `DueDate`, " &
            "MAX(`Status`) AS `Status`, " &
            "MAX(`TransactionReceipt`) AS `TransactionReceipt`, " &
            "MAX(`BorrowerStatus`) AS `BorrowerStatus`, " &
            "GROUP_CONCAT(DISTINCT NULLIF(TRIM(`ReturnedBook`), '') " &
            "ORDER BY `ID` SEPARATOR ' | ') AS `ReturnedBook`, " &
            "SUM(COALESCE(`BookTotal`, 0)) AS `BookTotal` " &
            "FROM `returning_tbl` " &
            "WHERE UPPER(TRIM(`Borrower`)) = 'STUDENT' " &
            "AND UPPER(TRIM(`BorrowerStatus`)) = @statusFilter " &
            "GROUP BY TRIM(`TransactionReceipt`) " &
            "ORDER BY MAX(`ID`) DESC"

        Dim adap As New MySqlDataAdapter(com, con)
        adap.SelectCommand.Parameters.AddWithValue("@statusFilter", statusFilter)

        Dim ds As New DataSet

        Try
            adap.Fill(ds, "info")
            DataGridView1.DataSource = ds.Tables("info")
        Catch ex As Exception
            MessageBox.Show(
                "Error loading Penalty data: " & ex.Message,
                "Database Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error
            )
        End Try

        If DataGridView1.Columns.Contains("ID") Then
            DataGridView1.Columns("ID").Visible = False
        End If

        DataGridView1.EnableHeadersVisualStyles = False
        DataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(207, 58, 109)
        DataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White
    End Sub

    Private Sub cbsort_SelectedIndexChanged(sender As Object, e As EventArgs)
        RefreshPenaltyData()
    End Sub

    Private Sub clear_details_only()
        txttransaction.Clear()
        lbllrn.Text = ""
        lblduedate.Text = ""
        lblbookstatus.Text = ""
        lblbookcounts.Text = ""
        lblborrowedbooks.Text = ""
        lblstatusus.Text = ""
        lblfullname.Text = ""

        _defaultFee = 0D
        _currentBooksList.Clear()
        _borrowerDepartment = ""

        txtfee.Text = ""
        txtfee.Enabled = False
        chknewfee.Checked = False
        cbremarks.SelectedIndex = -1

        btnpenalize.Enabled = False
    End Sub

    Private Sub txttransaction_TextChanged(sender As Object, e As EventArgs) Handles txttransaction.TextChanged
        If IsLoadingTransaction Then Return

        Dim TransactionNo As String = txttransaction.Text.Trim()

        If String.IsNullOrWhiteSpace(TransactionNo) OrElse TransactionNo.Length < MIN_LENGTH Then
            lbllrn.Text = ""
            lblduedate.Text = ""
            lblbookstatus.Text = ""
            lblbookcounts.Text = ""
            lblborrowedbooks.Text = ""
            lblstatusus.Text = ""
            lblfullname.Text = ""
            btnpenalize.Enabled = False
            Return
        End If

        If TransactionNo.Length > MIN_LENGTH Then
            TransactionNo = TransactionNo.Substring(TransactionNo.Length - MIN_LENGTH)
        End If

        LoadPenaltyDetails(TransactionNo)
    End Sub

    Private Sub LoadPenaltyDetails(ByVal TransactionNo As String)
        Dim con As New MySqlConnection(GlobalVarsModule.connectionString)

        Try
            con.Open()

            Dim com As String =
                "SELECT `LRN`, `FullName`, `Department`, `DueDate`, `Status`, `BorrowerStatus` " &
                "FROM `returning_tbl` " &
                "WHERE `TransactionReceipt` = @transNo AND UPPER(TRIM(Borrower)) = 'STUDENT' " &
                "ORDER BY ID DESC LIMIT 1"

            Dim recordFound As Boolean = False

            Using cmd As New MySqlCommand(com, con)
                cmd.Parameters.AddWithValue("@transNo", TransactionNo)

                Using reader As MySqlDataReader = cmd.ExecuteReader()
                    If reader.Read() Then
                        recordFound = True
                        lbllrn.Text = If(reader("LRN") Is DBNull.Value, "N/A", reader("LRN").ToString())
                        lblfullname.Text = If(reader("FullName") Is DBNull.Value, "", reader("FullName").ToString())
                        _borrowerDepartment = If(reader("Department") Is DBNull.Value, "", reader("Department").ToString())
                        lblduedate.Text = reader("DueDate").ToString()
                        lblbookstatus.Text = reader("Status").ToString()
                        lblstatusus.Text = reader("BorrowerStatus").ToString()
                    End If
                End Using
            End Using

            If Not recordFound Then
                MessageBox.Show(
                    "No penalty record (student) found for this Transaction Number.",
                    "Not Found",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                )

                clear_details_only()
                Return
            End If

            LoadBorrowedBooksFromReturning(TransactionNo, con)

            ComputeDefaultFee(con)

            If lblstatusus.Text.Trim().ToUpper() = "PENALIZED" Then
                MessageBox.Show(
                    "This transaction has already been settled.",
                    "Already Settled",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                )

                btnpenalize.Enabled = False
            Else
                btnpenalize.Enabled = True
            End If

        Catch ex As Exception
            MessageBox.Show(
                "Error loading penalty details: " & ex.Message,
                "Database Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error
            )
        Finally
            If con.State = ConnectionState.Open Then con.Close()
        End Try
    End Sub


    Private Sub LoadBorrowedBooksFromReturning(
        ByVal TransactionNo As String,
        ByVal con As MySqlConnection
    )

        _currentBooksList.Clear()

        Dim totalCount As Integer = 0

        Dim com As String =
            "SELECT `ReturnedBook`, `BookTotal` " &
            "FROM `returning_tbl` " &
            "WHERE `TransactionReceipt` = @transNo"

        Using cmd As New MySqlCommand(com, con)

            cmd.Parameters.AddWithValue("@transNo", TransactionNo)

            Using reader As MySqlDataReader = cmd.ExecuteReader()

                While reader.Read()

                    Dim bookVal As String =
                        If(
                            reader("ReturnedBook") Is DBNull.Value,
                            "",
                            reader("ReturnedBook").ToString()
                        )

                    If Not String.IsNullOrWhiteSpace(bookVal) Then

                        Dim splitBooks As String() =
                            bookVal.Split(
                                New String() {" | "},
                                StringSplitOptions.RemoveEmptyEntries
                            )

                        For Each b In splitBooks

                            Dim cleanBook As String = b.Trim()

                            If Not String.IsNullOrWhiteSpace(cleanBook) Then

                                If Not _currentBooksList.Any(
                                    Function(x) x.Equals(
                                        cleanBook,
                                        StringComparison.OrdinalIgnoreCase
                                    )
                                ) Then

                                    _currentBooksList.Add(cleanBook)

                                End If

                            End If

                        Next

                    End If

                    If Not IsDBNull(reader("BookTotal")) Then
                        totalCount += Convert.ToInt32(reader("BookTotal"))
                    End If

                End While

            End Using

        End Using

        lblborrowedbooks.Text =
            String.Join(vbCrLf, _currentBooksList)

        lblbookcounts.Text =
            totalCount.ToString()

    End Sub

    Private Sub ComputeDefaultFee(ByVal con As MySqlConnection)
        Dim totalPrice As Decimal = 0D

        For Each bookTitle As String In _currentBooksList

            Dim com As String =
                "SELECT `BookPrice` " &
                "FROM `acquisition_tbl` " &
                "WHERE `BookTitle` = @bookTitle " &
                "LIMIT 1"

            Using cmd As New MySqlCommand(com, con)

                cmd.Parameters.AddWithValue(
                    "@bookTitle",
                    bookTitle
                )

                Dim result As Object =
                    cmd.ExecuteScalar()

                If result IsNot Nothing AndAlso
                   result IsNot DBNull.Value Then

                    totalPrice += Convert.ToDecimal(result)

                End If

            End Using

        Next

        _defaultFee = totalPrice

        If chknewfee.Checked Then

        Else
            txtfee.Text = _defaultFee.ToString("N2")
            txtfee.Enabled = False
        End If
    End Sub


    Private Sub chknewfee_CheckedChanged(sender As Object, e As EventArgs) Handles chknewfee.CheckedChanged

        If chknewfee.Checked Then

            txtfee.Enabled = True
            txtfee.Clear()
            txtfee.Focus()

        Else

            txtfee.Enabled = False
            txtfee.Text = _defaultFee.ToString("N2")

        End If

    End Sub

    Private Sub btnpenalized_Click(
        sender As Object,
        e As EventArgs
    ) Handles btnpenalize.Click

        Dim TransactionNo As String =
            txttransaction.Text.Trim()

        If String.IsNullOrWhiteSpace(TransactionNo) Then

            MessageBox.Show(
                "Please load a valid Transaction Number first.",
                "Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning
            )

            Return

        End If


        If String.IsNullOrWhiteSpace(txtfee.Text) Then

            MessageBox.Show(
                "Please enter the fee amount.",
                "Fee Required",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning
            )

            Return

        End If


        Dim feeAmount As Decimal

        If Not Decimal.TryParse(
            txtfee.Text,
            feeAmount
        ) Then

            MessageBox.Show(
                "Invalid fee amount.",
                "Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning
            )

            Return

        End If


        If cbremarks.SelectedItem Is Nothing Then

            MessageBox.Show(
                "Please select a remark.",
                "Remarks Required",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning
            )

            Return

        End If


        Dim remarks As String =
            cbremarks.SelectedItem.ToString().Trim()


        Dim isSettled As Boolean =
            remarks.Equals(
                "Paid",
                StringComparison.OrdinalIgnoreCase
            ) OrElse
            remarks.Equals(
                "Replaced Book",
                StringComparison.OrdinalIgnoreCase
            )


        Dim con As New MySqlConnection(
            GlobalVarsModule.connectionString
        )

        Dim trans As MySqlTransaction = Nothing


        Try

            con.Open()

            trans = con.BeginTransaction()

            Dim purExists As Boolean = False

            Dim checkPurQuery As String =
                "SELECT COUNT(*) " &
                "FROM `pur_tbl` " &
                "WHERE TRIM(`TransactionReceipt`) = TRIM(@transNo)"

            Using cmd As New MySqlCommand(
                checkPurQuery,
                con,
                trans
            )

                cmd.Parameters.AddWithValue(
                    "@transNo",
                    TransactionNo
                )

                purExists =
                    Convert.ToInt32(
                        cmd.ExecuteScalar()
                    ) > 0

            End Using


            If purExists Then

                Dim updatePurQuery As String =
                    "UPDATE `pur_tbl` " &
                    "SET `Status` = @status " &
                    "WHERE TRIM(`TransactionReceipt`) = TRIM(@transNo) " &
                    "AND UPPER(TRIM(`Status`)) = 'NOT PAID'"

                Using cmd As New MySqlCommand(
                    updatePurQuery,
                    con,
                    trans
                )

                    cmd.Parameters.AddWithValue(
                        "@status",
                        remarks
                    )

                    cmd.Parameters.AddWithValue(
                        "@transNo",
                        TransactionNo
                    )

                    cmd.ExecuteNonQuery()

                End Using


            Else

                '==========================================================
                ' INSERT EACH BOOK AS ITS OWN ROW IN pur_tbl
                ' DO NOT CREATE A COMBINED BOOKTITLE WITH " | "
                '==========================================================

                If _currentBooksList.Count > 0 Then

                    For Each individualBook As String In _currentBooksList

                        Dim insertPurQuery As String =
                            "INSERT INTO `pur_tbl` " &
                            "(`Borrower`, `Fullname`, `Department`, `BookTitle`, `Status`, `TransactionReceipt`) " &
                            "VALUES " &
                            "(@borrower, @fullname, @department, @bookTitle, @status, @transNo)"


                        Using cmd As New MySqlCommand(
                            insertPurQuery,
                            con,
                            trans
                        )

                            cmd.Parameters.AddWithValue(
                                "@borrower",
                                "Student"
                            )

                            cmd.Parameters.AddWithValue(
                                "@fullname",
                                lblfullname.Text
                            )

                            If String.IsNullOrWhiteSpace(
                                _borrowerDepartment
                            ) Then

                                cmd.Parameters.AddWithValue(
                                    "@department",
                                    DBNull.Value
                                )

                            Else

                                cmd.Parameters.AddWithValue(
                                    "@department",
                                    _borrowerDepartment
                                )

                            End If


                            If String.IsNullOrWhiteSpace(
                                individualBook
                            ) Then

                                cmd.Parameters.AddWithValue(
                                    "@bookTitle",
                                    DBNull.Value
                                )

                            Else

                                cmd.Parameters.AddWithValue(
                                    "@bookTitle",
                                    individualBook.Trim()
                                )

                            End If


                            cmd.Parameters.AddWithValue(
                                "@status",
                                remarks
                            )

                            cmd.Parameters.AddWithValue(
                                "@transNo",
                                TransactionNo
                            )

                            cmd.ExecuteNonQuery()

                        End Using

                    Next

                Else

                    Dim insertPurQuery As String =
                        "INSERT INTO `pur_tbl` " &
                        "(`Borrower`, `Fullname`, `Department`, `BookTitle`, `Status`, `TransactionReceipt`) " &
                        "VALUES " &
                        "(@borrower, @fullname, @department, @bookTitle, @status, @transNo)"


                    Using cmd As New MySqlCommand(
                        insertPurQuery,
                        con,
                        trans
                    )

                        cmd.Parameters.AddWithValue(
                            "@borrower",
                            "Student"
                        )

                        cmd.Parameters.AddWithValue(
                            "@fullname",
                            lblfullname.Text
                        )

                        If String.IsNullOrWhiteSpace(
                            _borrowerDepartment
                        ) Then

                            cmd.Parameters.AddWithValue(
                                "@department",
                                DBNull.Value
                            )

                        Else

                            cmd.Parameters.AddWithValue(
                                "@department",
                                _borrowerDepartment
                            )

                        End If


                        cmd.Parameters.AddWithValue(
                            "@bookTitle",
                            DBNull.Value
                        )

                        cmd.Parameters.AddWithValue(
                            "@status",
                            remarks
                        )

                        cmd.Parameters.AddWithValue(
                            "@transNo",
                            TransactionNo
                        )

                        cmd.ExecuteNonQuery()

                    End Using

                End If

            End If


            If isSettled Then

                Dim updateReturningQuery As String =
                    "UPDATE `returning_tbl` " &
                    "SET `BorrowerStatus` = 'PENALIZED' " &
                    "WHERE TRIM(`TransactionReceipt`) = TRIM(@transNo) " &
                    "AND UPPER(TRIM(`Borrower`)) = 'STUDENT'"

                Using cmd As New MySqlCommand(
                    updateReturningQuery,
                    con,
                    trans
                )

                    cmd.Parameters.AddWithValue(
                        "@transNo",
                        TransactionNo
                    )

                    cmd.ExecuteNonQuery()

                End Using

            End If


            trans.Commit()


            GlobalVarsModule.LogAudit(
                actionType:="UPDATE",
                formName:="PENALTY SETTLEMENT",
                description:=$"Settled penalty for transaction {TransactionNo}. Fee: {feeAmount}, Remarks: {remarks}.",
                recordID:=TransactionNo,
                oldValue:=$"Status: {lblstatusus.Text}",
                newValue:=$"Status: {(If(isSettled, "PENALIZED", "NOT PENALIZED"))}, Fee: {feeAmount}, Remarks: {remarks}"
            )


            For Each form In Application.OpenForms

                If TypeOf form Is AuditTrail Then

                    DirectCast(
                        form,
                        AuditTrail
                    ).refreshaudit()

                End If


                If TypeOf form Is Returning Then

                    DirectCast(
                        form,
                        Returning
                    ).RefreshReturningData()

                End If


                If TypeOf form Is PUR Then

                    DirectCast(
                        form,
                        PUR
                    ).refreshPUR()

                End If

            Next


            MessageBox.Show(
                "Penalty settled successfully!" &
                Environment.NewLine &
                Environment.NewLine &
                "Transaction: " & TransactionNo &
                Environment.NewLine &
                "Remarks: " & remarks,
                "Success",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            )


            clear_details_only()

            RefreshPenaltyData()


        Catch ex As Exception

            Try

                If trans IsNot Nothing Then
                    trans.Rollback()
                End If

            Catch
            End Try


            MessageBox.Show(
                "Error settling penalty:" &
                Environment.NewLine &
                Environment.NewLine &
                ex.Message,
                "Database Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error
            )


        Finally

            If con.State = ConnectionState.Open Then
                con.Close()
            End If

        End Try

    End Sub

    Private Sub DataGridView1_CellClick(
        sender As Object,
        e As DataGridViewCellEventArgs
    ) Handles DataGridView1.CellClick

        If e.RowIndex < 0 Then Return

        Dim selectedRow As DataGridViewRow =
            DataGridView1.Rows(e.RowIndex)

        IsLoadingTransaction = True

        txttransaction.Text =
            selectedRow.Cells("TransactionReceipt").Value?.ToString()

        IsLoadingTransaction = False

        LoadPenaltyDetails(
            txttransaction.Text
        )

    End Sub


    Private Sub txtsearch_TextChanged(
        sender As Object,
        e As EventArgs
    ) Handles txtsearch.TextChanged

        Dim dt As DataTable =
            TryCast(
                DataGridView1.DataSource,
                DataTable
            )

        If dt IsNot Nothing Then

            If txtsearch.Text.Trim() <> "" Then

                Dim filter As String =
                    String.Format(
                        "FullName LIKE '%{0}%' OR TransactionReceipt LIKE '%{0}%'",
                        txtsearch.Text.Trim()
                    )

                dt.DefaultView.RowFilter = filter

            Else

                dt.DefaultView.RowFilter = ""

            End If

        End If

    End Sub

    Private Sub LinkLabel1_LinkClicked(
        sender As Object,
        e As LinkLabelLinkClickedEventArgs
    ) Handles LinkLabel1.LinkClicked

        Try

            Dim purForm =
                Application.OpenForms.
                OfType(Of PUR)().
                FirstOrDefault()

            If purForm IsNot Nothing Then

                purForm.refreshPUR("PAID")
                purForm.BringToFront()
                purForm.Show()

            Else

                Dim pf As New PUR()

                pf.refreshPUR("PAID")
                pf.ShowDialog(Me)

            End If

        Catch ex As Exception

            MessageBox.Show(
                "Error opening PUR view: " & ex.Message,
                "Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error
            )

        End Try

    End Sub

End Class