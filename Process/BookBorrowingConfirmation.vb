Imports MySql.Data.MySqlClient
Imports System.Data
Imports System.Windows.Forms

Public Class BookBorrowingConfirmation
    Private ReadOnly connectionString As String = GlobalVarsModule.connectionString

    Private currentBookTitle As String = ""
    Private isNumericEditing As Boolean = False
    Private lastNumericValue As Decimal = 0
    Private Sub BookBorrowingConfirmation_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        refreshconfirmation()
        GlobalVarsModule.AutoRefreshGrid(DataGridView1, "SELECT ID, Borrower, Name, BorrowedDate, BorrowedBookCount, DueDate,  TransactionReceipt, Status FROM `confimation_tbl`", 2000)
        AddHandler GlobalVarsModule.DatabaseUpdated, AddressOf OnDatabaseUpdated
    End Sub

    Public Sub refreshconfirmation()

        Dim con As New MySqlConnection(GlobalVarsModule.connectionString)
        Dim com As String = "SELECT ID, Borrower, Name, BorrowedDate, BorrowedBookCount, DueDate, TransactionReceipt, Status FROM `confimation_tbl`"
        Dim adap As New MySqlDataAdapter(com, con)
        Dim ds As New DataSet

        Try
            con.Open()
            adap.Fill(ds, "info")

            DataGridView1.AutoGenerateColumns = True
            DataGridView1.DataSource = ds.Tables("info")


            If DataGridView1.Columns.Contains("ID") Then
                DataGridView1.Columns("ID").Visible = False
            End If

            DataGridView1.EnableHeadersVisualStyles = False
            DataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(207, 58, 109)
            DataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White

        Catch ex As Exception
            MessageBox.Show(ex.Message)
        Finally
            If con.State = ConnectionState.Open Then con.Close()
        End Try
    End Sub


    Private Async Sub OnDatabaseUpdated()
        Try
            Dim query As String = "SELECT ID, Borrower, Name, BorrowedDate, BorrowedBookCount, DueDate, TransactionReceipt, Status FROM `confimation_tbl`"
            Await GlobalVarsModule.LoadToGridAsync(DataGridView1, query)
            DataGridView1.ClearSelection()
        Catch
        End Try
    End Sub


    Private Sub ComputeDueDate(userRole As String)
        Dim daysToAdd As Integer = 0
        Dim currentDate As DateTime = DateTime.Now

        If userRole = "Student" Then
            daysToAdd = studentLimit
        ElseIf userRole = "Teacher" Then
            daysToAdd = teacherLimit
        End If

        Dim dueDate As DateTime = currentDate.AddDays(daysToAdd)
        lblduedate.Text = dueDate.ToString("MMM dd, yyyy")

    End Sub

    Private Sub pendingstats(accessionID As String, status As String)
        Try
            Dim con As New MySqlConnection(connectionString)
            Using con
                Dim comm As String = "UPDATE `acession_tbl` SET `Status` = @status WHERE `AccessionID` = @accessionID"
                con.Open()
                Dim cmd As New MySqlCommand(comm, con)
                cmd.Parameters.AddWithValue("@status", status)
                cmd.Parameters.AddWithValue("@accessionID", accessionID)
                cmd.ExecuteNonQuery()
            End Using

            For Each form In Application.OpenForms
                If TypeOf form Is AvailableBooks Then
                    DirectCast(form, AvailableBooks).refreshavail()
                    Exit Sub
                End If
            Next
        Catch ex As Exception
            MessageBox.Show("Error updating accession table: " & ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub btnconfirm_Click(sender As Object, e As EventArgs) Handles btnconfirm.Click


        If DataGridView1.SelectedRows.Count = 0 Then
            MsgBox("Please select a record to confirm.", vbExclamation, "No Selection")
            Exit Sub
        End If

        Dim selectedRow As DataGridViewRow = DataGridView1.SelectedRows(0)

        Dim idToConfirm As Integer = CInt(selectedRow.Cells("ID").Value)
        Dim transactionReceiptID As String = selectedRow.Cells("TransactionReceipt").Value.ToString()
        Dim borrowedBookCount As Integer = CInt(selectedRow.Cells("BorrowedBookCount").Value)
        Dim borrowerType As String = selectedRow.Cells("Borrower").Value.ToString()
        Dim borrowerName As String = selectedRow.Cells("Name").Value.ToString()

        Dim con As New MySqlConnection(connectionString)

        Dim finalBorrowedDate As String = selectedRow.Cells("BorrowedDate").Value.ToString()
        Dim bDate As DateTime = Convert.ToDateTime(finalBorrowedDate)

        Dim daysToAdd As Integer = 0
        If borrowerType.ToLower() = "student" Then
            daysToAdd = studentLimit
        ElseIf borrowerType.ToLower() = "teacher" Then
            daysToAdd = teacherLimit
        End If

        Dim finalDueDate As DateTime = bDate.AddDays(daysToAdd)

        Dim finalLRN As String = If(lbllrn.Text = "--", "", lbllrn.Text)
        Dim finalEmpNo As String = If(lblemployeeno.Text = "--", "", lblemployeeno.Text)

        Dim bookDetailsList As New List(Of Dictionary(Of String, String))
        Dim totalBooksConfirmed As Integer = 0

        Try
            con.Open()

            Dim updateDueSql As String =
            "UPDATE borrowing_tbl 
             SET DueDate = @DueDate 
             WHERE TransactionReceipt = @TID"

            Using cmdUpdateDue As New MySqlCommand(updateDueSql, con)
                cmdUpdateDue.Parameters.AddWithValue("@DueDate", finalDueDate)
                cmdUpdateDue.Parameters.AddWithValue("@TID", transactionReceiptID)
                cmdUpdateDue.ExecuteNonQuery()
            End Using

            Dim comGetBooks As String =
            "SELECT AccessionID, BookTitle, ISBN, Barcode, Shelf 
             FROM borrowing_tbl 
             WHERE TransactionReceipt = @TID AND AccessionID IS NOT NULL"

            Using cmdGetBooks As New MySqlCommand(comGetBooks, con)
                cmdGetBooks.Parameters.AddWithValue("@TID", transactionReceiptID)
                Dim reader As MySqlDataReader = cmdGetBooks.ExecuteReader()

                While reader.Read()
                    Dim bookData As New Dictionary(Of String, String)
                    bookData.Add("AccessionID", reader("AccessionID").ToString())
                    bookData.Add("BookTitle", reader("BookTitle").ToString())
                    bookData.Add("ISBN", reader("ISBN").ToString())
                    bookData.Add("Barcode", reader("Barcode").ToString())
                    bookData.Add("Shelf", reader("Shelf").ToString())
                    bookDetailsList.Add(bookData)
                End While
                reader.Close()
            End Using

            If bookDetailsList.Count = 0 Then
                MsgBox("No books found for this transaction receipt.", vbExclamation)
                Exit Sub
            End If

            For Each bookData In bookDetailsList

                Dim finalAccessionID As String = bookData("AccessionID")
                Dim finalBookTitle As String = bookData("BookTitle")
                Dim finalISBN As String = bookData("ISBN")
                Dim finalBarcode As String = bookData("Barcode")
                Dim finalShelf As String = bookData("Shelf")

                Dim checkHistoryCom As String =
                "SELECT COUNT(*) FROM borrowinghistory_tbl 
                 WHERE TransactionReceipt = @TID 
                 AND AccessionID = @ACCID 
                 AND Status = 'Granted'"

                Using cmdCheckHistory As New MySqlCommand(checkHistoryCom, con)
                    cmdCheckHistory.Parameters.AddWithValue("@TID", transactionReceiptID)
                    cmdCheckHistory.Parameters.AddWithValue("@ACCID", finalAccessionID)

                    If CInt(cmdCheckHistory.ExecuteScalar()) = 0 Then

                        Dim insertHistoryCom As String =
                        "INSERT INTO borrowinghistory_tbl 
                        (Borrower, LRN, EmployeeNo, Name, BookTitle, ISBN, Barcode, AccessionID, Shelf, BorrowedDate, DueDate, TransactionReceipt, Status)
                        VALUES
                        (@Borrower, @LRN, @EmpNo, @Name, @Title, @ISBN, @Barcode, @AccessionID, @Shelf, @BDate, @DDate, @TransactionReceipt, @Status)"

                        Using cmdHistory As New MySqlCommand(insertHistoryCom, con)
                            cmdHistory.Parameters.AddWithValue("@Borrower", borrowerType)
                            cmdHistory.Parameters.AddWithValue("@LRN", If(String.IsNullOrWhiteSpace(finalLRN), DBNull.Value, finalLRN))
                            cmdHistory.Parameters.AddWithValue("@EmpNo", If(String.IsNullOrWhiteSpace(finalEmpNo), DBNull.Value, finalEmpNo))
                            cmdHistory.Parameters.AddWithValue("@Name", borrowerName)
                            cmdHistory.Parameters.AddWithValue("@Title", finalBookTitle)
                            cmdHistory.Parameters.AddWithValue("@ISBN", finalISBN)
                            cmdHistory.Parameters.AddWithValue("@Barcode", finalBarcode)
                            cmdHistory.Parameters.AddWithValue("@AccessionID", finalAccessionID)
                            cmdHistory.Parameters.AddWithValue("@Shelf", finalShelf)
                            cmdHistory.Parameters.AddWithValue("@BDate", bDate)
                            cmdHistory.Parameters.AddWithValue("@DDate", finalDueDate)
                            cmdHistory.Parameters.AddWithValue("@TransactionReceipt", transactionReceiptID)
                            cmdHistory.Parameters.AddWithValue("@Status", "Granted")
                            cmdHistory.ExecuteNonQuery()
                        End Using

                        pendingstats(finalAccessionID, "Borrowed")
                        totalBooksConfirmed += 1

                        Dim updateAcquisitionSql As String =
                        "UPDATE acquisition_tbl 
                         SET Quantity = Quantity - 1 
                         WHERE BookTitle = @BookTitle AND Quantity > 0"

                        Using updateAcquisitionCmd As New MySqlCommand(updateAcquisitionSql, con)
                            updateAcquisitionCmd.Parameters.AddWithValue("@BookTitle", finalBookTitle)
                            updateAcquisitionCmd.ExecuteNonQuery()
                        End Using
                    End If
                End Using
            Next

            Dim deleteComConfirm As String = "DELETE FROM confimation_tbl WHERE ID = @ID"
            Using cmdDeleteConfirm As New MySqlCommand(deleteComConfirm, con)
                cmdDeleteConfirm.Parameters.AddWithValue("@ID", idToConfirm)
                cmdDeleteConfirm.ExecuteNonQuery()
            End Using

            InsertPrintReceipt(borrowerType, borrowerName, finalBorrowedDate, transactionReceiptID, borrowedBookCount, finalDueDate.ToString("yyyy-MM-dd"))

            For Each form In Application.OpenForms
                If TypeOf form Is MainForm Then
                    Dim load = DirectCast(form, MainForm)
                    load.lblborrowcount()
                End If

            Next

            MsgBox(totalBooksConfirmed & " books successfully GRANTED.", vbInformation)

        Catch ex As Exception
            MessageBox.Show("Error granting record: " & ex.Message)

        Finally
            If con.State = ConnectionState.Open Then con.Close()

            refreshconfirmation()

            DataGridView1.Columns("DueDate").DefaultCellStyle.Format = "MMM dd, yyyy"


            ClearDetails()
            lblduedate.Text = "--"
        End Try

    End Sub



    Private Sub btnclear_Click(sender As Object, e As EventArgs) Handles btnclear.Click
        ClearDetails()
    End Sub



    Public Sub InsertPrintReceipt(ByVal borrowerType As String, ByVal borrowerName As String, ByVal borrowedDate As String, ByVal transactionReceiptID As String, ByVal bookCount As Integer, ByVal DueDatess As String)
        Dim con As New MySqlConnection(GlobalVarsModule.connectionString)

        Try
            con.Open()


            Dim insertCom As String = "INSERT INTO printreceipt_tbl (Borrower, Name, BorrowedDate, DueDate, TransactionReceipt, BorrowedBookCount, IsPrinted) " &
                                  "VALUES (@Borrower, @Name, @BDate, @DDate, @TID, @BookCount, @IsPrinted)"

            Using cmdInsert As New MySqlCommand(insertCom, con)
                cmdInsert.Parameters.AddWithValue("@Borrower", borrowerType)
                cmdInsert.Parameters.AddWithValue("@Name", borrowerName)
                cmdInsert.Parameters.AddWithValue("@BDate", borrowedDate)
                cmdInsert.Parameters.AddWithValue("@DDate", DueDatess)
                cmdInsert.Parameters.AddWithValue("@TID", transactionReceiptID)
                cmdInsert.Parameters.AddWithValue("@BookCount", bookCount)
                cmdInsert.Parameters.AddWithValue("@IsPrinted", False)

                cmdInsert.ExecuteNonQuery()
            End Using

        Catch ex As Exception
            MessageBox.Show("Error inserting into Print Receipt table: " & ex.Message, "DB Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            If con.State = ConnectionState.Open Then con.Close()
        End Try
    End Sub


    Private Sub txtsearch_TextChanged(sender As Object, e As EventArgs) Handles txtsearch.TextChanged

        HandleAutoRefreshPause(DataGridView1, txtsearch)

        If String.IsNullOrWhiteSpace(txtsearch.Text) Then
            refreshconfirmation()
            ClearDetails()
            Exit Sub
        End If

        Try
            Dim searchValue As String = txtsearch.Text.Trim()
            Dim bs As New BindingSource()
            bs.DataSource = DataGridView1.DataSource

            bs.Filter = String.Format("Name LIKE '%{0}%'", searchValue.Replace("'", "''"))
            DataGridView1.DataSource = bs

            If DataGridView1.Rows.Count > 0 AndAlso bs.Count > 0 Then
                DataGridView1.Rows(0).Selected = True
                LoadBorrowerDetails()
            Else
                ClearDetails()
            End If

        Catch ex As Exception
            MessageBox.Show("Error searching: " & ex.Message, "Search Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            ClearDetails()
        End Try
    End Sub

    Private Sub DataGridView1_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellClick

        PauseAutoRefresh(DataGridView1)
        LoadBorrowerDetails()

        Try
            If e.RowIndex >= 0 Then
                Dim selectedRow As DataGridViewRow = DataGridView1.Rows(e.RowIndex)
                Dim borrowerType As String = selectedRow.Cells("Borrower").Value.ToString()
                Dim borrowedDate As DateTime = Convert.ToDateTime(selectedRow.Cells("BorrowedDate").Value)
                Dim daysToAdd As Integer = 0

                If borrowerType.ToLower() = "student" Then
                    daysToAdd = studentLimit
                ElseIf borrowerType.ToLower() = "teacher" Then
                    daysToAdd = teacherLimit
                End If

                Dim dueDate As DateTime = borrowedDate.AddDays(daysToAdd)

                lblduedate.Text = dueDate.ToString("MMM dd, yyyy")

                selectedRow.Cells("DueDate").Value = dueDate.ToString("MMM dd, yyyy")

            End If
        Catch ex As Exception
            lblduedate.Text = "--"
        End Try

        Try
            If cbbooks.Items.Count > 1 Then
                cbbooks.SelectedIndex = 1
            ElseIf cbbooks.Items.Count > 0 Then
                cbbooks.SelectedIndex = 0
            End If
        Catch ex As Exception
        End Try

    End Sub

    Private Sub LoadBorrowerDetails()
        If DataGridView1.SelectedRows.Count = 0 Then
            ClearDetails()
            Exit Sub
        End If

        Dim selectedRow As DataGridViewRow = DataGridView1.SelectedRows(0)

        Dim transactionReceiptID As String = selectedRow.Cells("TransactionReceipt").Value.ToString
        Dim borrowerType As String = selectedRow.Cells("Borrower").Value.ToString
        Dim nameValue As String = selectedRow.Cells("Name").Value.ToString


        lblborrowertype.Text = borrowerType


        lbltransactionreceipt.Text = transactionReceiptID
        lblbooktotal.Text = selectedRow.Cells("BorrowedBookCount").Value.ToString
        lblfullname.Text = nameValue
        Dim borrowedDateStr As String = selectedRow.Cells("BorrowedDate").Value.ToString()


        lbllrn.Text = "--"
        lblemployeeno.Text = "--"
        lblgrade.Text = "--"
        lblsection.Text = "--"
        lblstrand.Text = "--"
        lbldepartment.Text = "--"
        lblisbnbarcode.Text = "--"
        lblaccessionid.Text = "--"
        lblborroweddate.Text = "--"


        Dim defaultLimitDays As Integer = 0
        Select Case borrowerType.ToLower()
            Case "student"
                defaultLimitDays = 3
            Case "teacher"
                defaultLimitDays = 7
            Case Else
                defaultLimitDays = 999
        End Select


        Dim identifierValue As String = ""
        Dim identifierColumn As String = ""

        Dim con As New MySqlConnection(GlobalVarsModule.connectionString)

        Dim comIdentifier As String = "SELECT LRN, EmployeeNo FROM `borrowing_tbl` WHERE `TransactionReceipt` = @TID LIMIT 1"

        Try
            con.Open()
            Using cmdIdentifier As New MySqlCommand(comIdentifier, con)

                cmdIdentifier.Parameters.AddWithValue("@TID", transactionReceiptID)
                Dim reader As MySqlDataReader = cmdIdentifier.ExecuteReader()

                If reader.Read() Then
                    Dim lrnData As Object = reader("LRN")
                    Dim empNoData As Object = reader("EmployeeNo")

                    lbllrn.Text = If(lrnData Is DBNull.Value, "--", lrnData.ToString)
                    lblemployeeno.Text = If(empNoData Is DBNull.Value, "--", empNoData.ToString)

                    If lrnData IsNot DBNull.Value AndAlso Not String.IsNullOrWhiteSpace(lrnData.ToString) Then
                        identifierValue = lrnData.ToString
                        identifierColumn = "LRN"
                    ElseIf empNoData IsNot DBNull.Value AndAlso Not String.IsNullOrWhiteSpace(empNoData.ToString) Then
                        identifierValue = empNoData.ToString
                        identifierColumn = "EmployeeNo"
                    End If
                End If
                reader.Close()
            End Using


            If Not String.IsNullOrWhiteSpace(identifierValue) Then
                LoadFullBorrowerInfo(con, identifierColumn, identifierValue, borrowerType)
            End If


        Catch ex As Exception
            MessageBox.Show("Error loading borrower identifier: " & ex.Message, "DB Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            If con.State = ConnectionState.Open Then con.Close()
        End Try


        LoadBookList(transactionReceiptID)

        If cbbooks.Items.Count > 0 Then

            cbbooks.SelectedIndex = 0
        End If
    End Sub



    Private Sub LoadFullBorrowerInfo(con As MySqlConnection, identifierColumn As String, identifierValue As String, borrowerType As String)

        Dim com As String = $"SELECT Grade, Section, Strand, Department FROM `borrower_tbl` WHERE `{identifierColumn}` = @Identifier"

        Try
            If con.State <> ConnectionState.Open Then con.Open()

            Using cmd As New MySqlCommand(com, con)
                cmd.Parameters.AddWithValue("@Identifier", identifierValue)
                Dim reader As MySqlDataReader = cmd.ExecuteReader()

                If reader.Read() Then

                    Dim departmentValue As String = reader("Department").ToString()


                    If borrowerType.ToLower() = "student" Then
                        lblgrade.Text = reader("Grade").ToString()
                        lblsection.Text = reader("Section").ToString()
                        lblstrand.Text = reader("Strand").ToString()
                        lbldepartment.Text = departmentValue
                    ElseIf borrowerType.ToLower() = "teacher" Then

                        lblgrade.Text = "--"
                        lblsection.Text = "--"
                        lblstrand.Text = "--"
                        lbldepartment.Text = departmentValue
                    Else

                        lblgrade.Text = "--"
                        lblsection.Text = "--"
                        lblstrand.Text = "--"
                        lbldepartment.Text = departmentValue
                    End If


                End If
                reader.Close()
            End Using
        Catch ex As Exception
            MessageBox.Show("Error loading full borrower info: " & ex.Message, "DB Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

    End Sub

    Private Sub Confirmation_Shown(sender As Object, e As EventArgs) Handles MyBase.Shown

        DataGridView1.ClearSelection()

    End Sub

    Private Sub LoadBookList(transactionReceiptID As String)
        Dim con As New MySqlConnection(GlobalVarsModule.connectionString)


        Dim comExistingBooks As String = "SELECT t1.AccessionID, t2.BookTitle FROM `borrowing_tbl` t1 " &
                                             "INNER JOIN `acession_tbl` t2 ON t1.AccessionID = t2.AccessionID " &
                                             "WHERE t1.`TransactionReceipt` = @TID"

        Try
            con.Open()
            Using cmd As New MySqlCommand(comExistingBooks, con)
                cmd.Parameters.AddWithValue("@TID", transactionReceiptID)
                Dim reader As MySqlDataReader = cmd.ExecuteReader()

                cbbooks.DataSource = Nothing
                cbbooks.Items.Clear()

                Dim dt As New DataTable()
                dt.Columns.Add("BookDetail")
                dt.Columns.Add("AccessionID")
                dt.Rows.Add("--- View Books ---", "")

                While reader.Read()


                    dt.Rows.Add(reader("BookTitle").ToString(), reader("AccessionID").ToString())

                End While

                reader.Close()

                cbbooks.DataSource = dt
                cbbooks.DisplayMember = "BookDetail"
                cbbooks.ValueMember = "AccessionID"
            End Using

        Catch ex As Exception
            MessageBox.Show("Error loading book list: " & ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            If con.State = ConnectionState.Open Then con.Close()
        End Try

        If cbbooks.Items.Count > 0 Then
            cbbooks.SelectedIndex = 0
        End If
    End Sub


    Private Sub cbbooks_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbbooks.SelectedIndexChanged

        If Not Me.IsHandleCreated OrElse Not cbbooks.Visible Then Exit Sub
        If cbbooks.SelectedIndex = -1 Then Exit Sub


        If cbbooks.SelectedValue IsNot Nothing AndAlso cbbooks.SelectedValue.ToString() <> "" Then
            Dim selectedAccessionID As String = cbbooks.SelectedValue.ToString()
            lblaccessionid.Text = selectedAccessionID

            lbldetails.Visible = False



            Dim con As New MySqlConnection(connectionString)
            Try
                con.Open()
                LoadBookDetailsByAccession(con, selectedAccessionID)
            Catch ex As Exception
                MessageBox.Show("Error loading book details on selection: " & ex.Message)
            Finally
                If con.State = ConnectionState.Open Then con.Close()
            End Try



        Else

            If cbbooks.Items.Count = 0 Then
                lblaccessionid.Text = "--"
                lblisbnbarcode.Text = "--"
                currentBookTitle = ""
                lblborroweddate.Text = "--"
                lbldetails.Visible = True
            End If
        End If
    End Sub



    Private Function GetBookDetailsFromAccession(con As MySqlConnection, accessionID As String) As Dictionary(Of String, String)
        Dim bookData As New Dictionary(Of String, String) From {
            {"ISBN", ""}, {"Barcode", ""}, {"BookTitle", ""}
        }


        Dim comBookDetails As String = "SELECT `ISBN`, `Barcode`, `BookTitle` FROM `acession_tbl` WHERE `AccessionID` = @AccessionID"

        Try

            If con.State <> ConnectionState.Open Then con.Open()

            Using cmdBookDetails As New MySqlCommand(comBookDetails, con)
                cmdBookDetails.Parameters.AddWithValue("@AccessionID", accessionID)
                Dim reader As MySqlDataReader = cmdBookDetails.ExecuteReader()

                If reader.Read() Then
                    bookData("ISBN") = reader("ISBN").ToString()
                    bookData("Barcode") = reader("Barcode").ToString()
                    bookData("BookTitle") = reader("BookTitle").ToString()
                End If
                reader.Close()
            End Using

        Catch ex As Exception
            MessageBox.Show("Error retrieving book details from accession table: " & ex.Message)
        End Try

        Return bookData
    End Function


    Private Sub LoadBookDetailsByAccession(con As MySqlConnection, accessionID As String)
        Dim transactionReceiptID As String = lbltransactionreceipt.Text


        Dim comDates As String = "SELECT `BorrowedDate` FROM `borrowing_tbl` WHERE `AccessionID` = @AccessionID AND `TransactionReceipt` = @TID LIMIT 1"

        Try
            If con.State <> ConnectionState.Open Then con.Open()


            Dim bookDetails = GetBookDetailsFromAccession(con, accessionID)


            lblisbnbarcode.Text = $"ISBN: {bookDetails("ISBN")} | Barcode: {bookDetails("Barcode")}"


            currentBookTitle = bookDetails("BookTitle")


            Using cmdDates As New MySqlCommand(comDates, con)
                cmdDates.Parameters.AddWithValue("@AccessionID", accessionID)
                cmdDates.Parameters.AddWithValue("@TID", transactionReceiptID)
                Dim readerDates As MySqlDataReader = cmdDates.ExecuteReader()

                If readerDates.Read() Then
                    lblborroweddate.Text = readerDates("BorrowedDate").ToString()

                Else

                    If DataGridView1.SelectedRows.Count > 0 Then
                        Dim selectedRow As DataGridViewRow = DataGridView1.SelectedRows(0)
                        lblborroweddate.Text = selectedRow.Cells("BorrowedDate").Value.ToString()

                    Else
                        lblborroweddate.Text = "--"

                    End If
                End If
                readerDates.Close()
            End Using

        Catch ex As Exception
            MessageBox.Show("Error loading book details: " & ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

    End Sub


    Public Sub ClearDetails()

        lblborrowertype.Text = "--"
        lbllrn.Text = "--"
        lblemployeeno.Text = "--"
        lblfullname.Text = "--"
        lblgrade.Text = "--"
        lblsection.Text = "--"
        lblstrand.Text = "--"
        lbldepartment.Text = "--"

        lbltransactionreceipt.Text = "--"
        lblbooktotal.Text = "--"


        lblisbnbarcode.Text = "--"
        lblaccessionid.Text = "--"
        lblborroweddate.Text = "--"


        currentBookTitle = ""
        lbldetails.Visible = False

        cbbooks.DataSource = Nothing
        cbbooks.Items.Clear()


        refreshconfirmation()
    End Sub

    Private Sub btndecline_Click(sender As Object, e As EventArgs) Handles btndecline.Click

        If DataGridView1.SelectedRows.Count = 0 Then
            MsgBox("Please select a record to decline.", vbExclamation, "No Selection")
            Exit Sub
        End If

        Dim selectedRow As DataGridViewRow = DataGridView1.SelectedRows(0)

        If MsgBox("Are you sure you want to DECLINE this borrowing request? This action will cancel the entire pending transaction (Receipt: " & selectedRow.Cells("TransactionReceipt").Value.ToString & ") ", vbQuestion + vbYesNo, "Confirm Decline") = vbNo Then
            Exit Sub
        End If

        Dim idToDecline As Integer = CInt(selectedRow.Cells("ID").Value)
        Dim transactionReceiptID As String = selectedRow.Cells("TransactionReceipt").Value.ToString

        Dim con As New MySqlConnection(GlobalVarsModule.connectionString)

        Dim bookDetailsList As New List(Of Dictionary(Of String, String))

        Dim borrowerType As String = selectedRow.Cells("Borrower").Value.ToString
        Dim borrowerName As String = selectedRow.Cells("Name").Value.ToString
        Dim borrowedDate As String = selectedRow.Cells("BorrowedDate").Value.ToString



        Dim finalLRN As String = If(lbllrn.Text = "--", "", lbllrn.Text)
        Dim finalEmpNo As String = If(lblemployeeno.Text = "--", "", lblemployeeno.Text)

        Try
            con.Open()

            Dim comGetBooks As String = "SELECT AccessionID, BookTitle, ISBN, Barcode FROM `borrowing_tbl` WHERE `TransactionReceipt` = @TID AND AccessionID IS NOT NULL"
            Using cmdGetBooks As New MySqlCommand(comGetBooks, con)
                cmdGetBooks.Parameters.AddWithValue("@TID", transactionReceiptID)
                Dim reader As MySqlDataReader = cmdGetBooks.ExecuteReader()
                While reader.Read()
                    Dim bookData As New Dictionary(Of String, String)
                    bookData.Add("AccessionID", reader("AccessionID").ToString())
                    bookData.Add("BookTitle", reader("BookTitle").ToString())
                    bookData.Add("ISBN", reader("ISBN").ToString())
                    bookData.Add("Barcode", reader("Barcode").ToString())
                    bookDetailsList.Add(bookData)
                End While
                reader.Close()
            End Using

            If bookDetailsList.Count = 0 AndAlso lblaccessionid.Text <> "--" Then
                Dim defaultBook As New Dictionary(Of String, String)
                defaultBook.Add("AccessionID", lblaccessionid.Text)
                defaultBook.Add("BookTitle", "Unknown")
                defaultBook.Add("ISBN", "")
                defaultBook.Add("Barcode", "")
                bookDetailsList.Add(defaultBook)
            End If

            For Each bookData In bookDetailsList

                Dim finalAccessionID As String = bookData("AccessionID")
                Dim finalBookTitle As String = bookData("BookTitle")
                Dim finalISBN As String = bookData("ISBN")
                Dim finalBarcode As String = bookData("Barcode")

                pendingstats(finalAccessionID, "Available")


                Dim insertHistoryCom As String = "INSERT INTO borrowinghistory_tbl (Borrower, LRN, EmployeeNo, Name, BookTitle, ISBN, Barcode, AccessionID, BorrowedDate, TransactionReceipt, Status) " &
                                             "VALUES (@Borrower, @LRN, @EmpNo, @Name, @Title, @ISBN, @Barcode, @AccessionID, @BDate, @DDate, @TransactionReceipt, @Status)"

                Using cmdHistory As New MySqlCommand(insertHistoryCom, con)
                    cmdHistory.Parameters.AddWithValue("@Borrower", borrowerType)
                    cmdHistory.Parameters.AddWithValue("@LRN", If(String.IsNullOrWhiteSpace(finalLRN), DBNull.Value, finalLRN))
                    cmdHistory.Parameters.AddWithValue("@EmpNo", If(String.IsNullOrWhiteSpace(finalEmpNo), DBNull.Value, finalEmpNo))
                    cmdHistory.Parameters.AddWithValue("@Name", borrowerName)
                    cmdHistory.Parameters.AddWithValue("@Title", finalBookTitle)
                    cmdHistory.Parameters.AddWithValue("@ISBN", finalISBN)
                    cmdHistory.Parameters.AddWithValue("@Barcode", finalBarcode)
                    cmdHistory.Parameters.AddWithValue("@AccessionID", finalAccessionID)
                    cmdHistory.Parameters.AddWithValue("@BDate", borrowedDate)
                    cmdHistory.Parameters.AddWithValue("@TransactionReceipt", transactionReceiptID)
                    cmdHistory.Parameters.AddWithValue("@Status", "Declined")
                    cmdHistory.ExecuteNonQuery()
                End Using
            Next

            Dim deleteComConfirm As String = "DELETE FROM confimation_tbl WHERE ID = @ID"
            Using cmdDeleteConfirm As New MySqlCommand(deleteComConfirm, con)
                cmdDeleteConfirm.Parameters.AddWithValue("@ID", idToDecline)
                cmdDeleteConfirm.ExecuteNonQuery()
            End Using

            Dim deleteComBorrowing As String = "DELETE FROM borrowing_tbl WHERE TransactionReceipt = @TID"
            Using cmdDeleteBorrowing As New MySqlCommand(deleteComBorrowing, con)
                cmdDeleteBorrowing.Parameters.AddWithValue("@TID", transactionReceiptID)
                cmdDeleteBorrowing.ExecuteNonQuery()
            End Using

            For Each form In Application.OpenForms
                If TypeOf form Is Borrowing Then
                    Dim brwr = DirectCast(form, Borrowing)
                    brwr.refreshborrowingsu()
                End If
            Next

            For Each form In Application.OpenForms
                If TypeOf form Is MainForm Then
                    Dim lbllabel = DirectCast(form, MainForm)
                    lbllabel.lblborrowcount()
                End If
            Next

            For Each form In Application.OpenForms
                If TypeOf form Is BorrowingHistory Then
                    Dim load = DirectCast(form, BorrowingHistory)
                    load.refreshhistory()
                End If
            Next

            MsgBox("Borrowing request successfully declined. Transaction: " & transactionReceiptID & "", vbInformation, "Declined")

        Catch ex As Exception
            MessageBox.Show("Error declining record: " & ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            If con.State = ConnectionState.Open Then con.Close()
            refreshconfirmation()
            ClearDetails()
        End Try

    End Sub
End Class