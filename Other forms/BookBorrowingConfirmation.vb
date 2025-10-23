Imports MySql.Data.MySqlClient
Imports System.Data
Imports System.Windows.Forms

Public Class BookBorrowingConfirmation
    Private ReadOnly connectionString As String = GlobalVarsModule.connectionString

    Private currentBookTitle As String = ""

    Private Sub BookBorrowingConfirmation_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        refreshconfirmation()
    End Sub

    Public Sub refreshconfirmation()
        Dim con As New MySqlConnection(connectionString)
        Dim com As String = "SELECT ID, Borrower, Name, BorrowedDate, BorrowedBookCount, DaysLimit, DueDate, TransactionReceipt, Status FROM `confimation_tbl`"
        Dim adap As New MySqlDataAdapter(com, con)
        Dim ds As New DataSet

        Try
            con.Open()
            adap.Fill(ds, "info")
            DataGridView1.DataSource = ds.Tables("info")
            DataGridView1.Columns("ID").Visible = False

            DataGridView1.EnableHeadersVisualStyles = False
            DataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(207, 58, 109)
            DataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White
        Catch ex As Exception
            MessageBox.Show("Error refreshing confirmation data: " & ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            If con.State = ConnectionState.Open Then con.Close()
        End Try
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
        Dim transactionReceiptID As String = selectedRow.Cells("TransactionReceipt").Value.ToString
        Dim borrowedBookCount As Integer = CInt(selectedRow.Cells("BorrowedBookCount").Value)
        Dim borrowerType As String = selectedRow.Cells("Borrower").Value.ToString
        Dim borrowerName As String = selectedRow.Cells("Name").Value.ToString

        Dim con As New MySqlConnection(connectionString)


        Dim finalBorrowedDate As String = selectedRow.Cells("BorrowedDate").Value.ToString
        Dim finalDueDate As String = lblduedate.Text
        Dim finalLRN As String = If(lbllrn.Text = "--", "", lbllrn.Text)
        Dim finalEmpNo As String = If(lblemployeeno.Text = "--", "", lblemployeeno.Text)

        Dim dbDueDate As String = ""
        Try

            dbDueDate = DateTime.Parse(finalDueDate).ToString("yyyy-MM-dd")
        Catch

            dbDueDate = selectedRow.Cells("DueDate").Value.ToString()
        End Try

        Dim bookDetailsList As New List(Of Dictionary(Of String, String))
        Dim totalBooksConfirmed As Integer = 0

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

            If bookDetailsList.Count = 0 Then
                MsgBox("No books found for this transaction receipt to confirm. Please check the 'borrowing_tbl' data.", vbExclamation, "No Books Found")
                Exit Sub
            End If


            For Each bookData In bookDetailsList

                Dim finalAccessionID As String = bookData("AccessionID")
                Dim finalBookTitle As String = bookData("BookTitle")
                Dim finalISBN As String = bookData("ISBN")
                Dim finalBarcode As String = bookData("Barcode")


                Dim checkHistoryCom As String = "SELECT COUNT(*) FROM borrowinghistory_tbl WHERE TransactionReceipt = @TID AND AccessionID = @ACCID AND Status = 'Granted'"
                Using cmdCheckHistory As New MySqlCommand(checkHistoryCom, con)
                    cmdCheckHistory.Parameters.AddWithValue("@TID", transactionReceiptID)
                    cmdCheckHistory.Parameters.AddWithValue("@ACCID", finalAccessionID)

                    If CInt(cmdCheckHistory.ExecuteScalar()) = 0 Then


                        Dim insertHistoryCom As String = "INSERT INTO borrowinghistory_tbl (Borrower, LRN, EmployeeNo, Name, BookTitle, ISBN, Barcode, AccessionID, BorrowedDate, DueDate, TransactionReceipt, Status) " &
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
                            cmdHistory.Parameters.AddWithValue("@BDate", finalBorrowedDate)
                            cmdHistory.Parameters.AddWithValue("@DDate", dbDueDate)
                            cmdHistory.Parameters.AddWithValue("@TransactionReceipt", transactionReceiptID)

                            cmdHistory.Parameters.AddWithValue("@Status", "Granted")

                            cmdHistory.ExecuteNonQuery()

                            pendingstats(finalAccessionID, "Borrowed")

                            totalBooksConfirmed += 1

                            Dim updateAcquisitionSql As String = "UPDATE acquisition_tbl SET Quantity = Quantity - 1 WHERE BookTitle = @BookTitle AND Quantity > 0"
                            Using updateAcquisitionCmd As New MySqlCommand(updateAcquisitionSql, con)

                                updateAcquisitionCmd.Parameters.AddWithValue("@BookTitle", finalBookTitle)
                                updateAcquisitionCmd.ExecuteNonQuery()
                            End Using

                        End Using
                    End If
                End Using
            Next



            Dim deleteComConfirm As String = "DELETE FROM confimation_tbl WHERE ID = @ID"
            Using cmdDeleteConfirm As New MySqlCommand(deleteComConfirm, con)
                cmdDeleteConfirm.Parameters.AddWithValue("@ID", idToConfirm)
                cmdDeleteConfirm.ExecuteNonQuery()
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
                If TypeOf form Is PrintReceiptForm Then
                    Dim lblload = DirectCast(form, PrintReceiptForm)
                    lblload.refreshreceipt()
                End If
            Next

            InsertPrintReceipt(borrowerType, borrowerName, finalBorrowedDate, transactionReceiptID, borrowedBookCount, dbDueDate)

            MsgBox(totalBooksConfirmed.ToString() & " books successfully GRANTED and recorded. Proceed to Print Receipt.", vbInformation, "Success")

        Catch ex As Exception
            MessageBox.Show("Error granting record: " & ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            If con.State = ConnectionState.Open Then con.Close()
            refreshconfirmation()
            ClearDetails()
        End Try
    End Sub

    Private Sub btnclear_Click(sender As Object, e As EventArgs) Handles btnclear.Click
        ClearDetails()
    End Sub



    Public Sub InsertPrintReceipt(ByVal borrowerType As String, ByVal borrowerName As String, ByVal borrowedDate As String, ByVal transactionReceiptID As String, ByVal bookCount As Integer, ByVal dueDate As String)
        Dim con As New MySqlConnection(connectionString)

        Try
            con.Open()

            Dim insertCom As String = "INSERT INTO printreceipt_tbl (Borrower, Name, BorrowedDate, TransactionReceipt, BorrowedBookCount, DueDate, IsPrinted) " &
                                  "VALUES (@Borrower, @Name, @BDate, @TID, @BookCount, @DDate, @IsPrinted)"

            Using cmdInsert As New MySqlCommand(insertCom, con)
                cmdInsert.Parameters.AddWithValue("@Borrower", borrowerType)
                cmdInsert.Parameters.AddWithValue("@Name", borrowerName)
                cmdInsert.Parameters.AddWithValue("@BDate", borrowedDate)
                cmdInsert.Parameters.AddWithValue("@TID", transactionReceiptID)
                cmdInsert.Parameters.AddWithValue("@BookCount", bookCount)
                cmdInsert.Parameters.AddWithValue("@DDate", dueDate)

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
        LoadBorrowerDetails()
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
        Dim currentDaysLimit As Integer = If(IsDBNull(selectedRow.Cells("DaysLimit").Value) OrElse String.IsNullOrWhiteSpace(selectedRow.Cells("DaysLimit").Value.ToString), 0, CInt(selectedRow.Cells("DaysLimit").Value))
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
        lblduedate.Text = "--"

        Dim defaultLimitDays As Integer = 0
        Select Case borrowerType.ToLower()
            Case "student"
                defaultLimitDays = 3
            Case "teacher"
                defaultLimitDays = 7
            Case Else
                defaultLimitDays = 999
        End Select


        If Me.Controls.Find("NumericUpDown1", True).Length > 0 Then
            Dim numControl As NumericUpDown = DirectCast(Me.Controls.Find("NumericUpDown1", True)(0), NumericUpDown)

            Dim finalDaysLimit As Integer = 0

            numControl.Minimum = 1
            numControl.Maximum = If(defaultLimitDays > 0, defaultLimitDays, 999)

            If currentDaysLimit > 0 AndAlso currentDaysLimit <= defaultLimitDays Then
                numControl.Value = currentDaysLimit
                finalDaysLimit = currentDaysLimit
            Else
                numControl.Value = defaultLimitDays
                finalDaysLimit = defaultLimitDays
            End If

            CalculateAndDisplayDueDate(finalDaysLimit, borrowedDateStr)

        End If

        Dim identifierValue As String = ""
        Dim identifierColumn As String = ""

        Dim con As New MySqlConnection(connectionString)

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
            cbbooks_SelectedIndexChanged(cbbooks, EventArgs.Empty)
        End If
    End Sub


    Private Sub CalculateAndDisplayDueDate(ByVal daysLimit As Decimal, ByVal borrowedDateStr As String)
        Try
            Dim borrowedDate As DateTime = DateTime.Parse(borrowedDateStr)
            Dim dueDate As DateTime = borrowedDate.AddDays(CInt(daysLimit))

            lblduedate.Text = dueDate.ToString("MMMM-dd-yyyy")


            If DataGridView1.SelectedRows.Count > 0 Then
                Dim selectedRow As DataGridViewRow = DataGridView1.SelectedRows(0)

                selectedRow.Cells("DueDate").Value = dueDate.ToString("yyyy-MM-dd")
                selectedRow.Cells("DaysLimit").Value = CInt(daysLimit)
            End If

        Catch ex As Exception
            lblduedate.Text = "-- Invalid Date --"
        End Try
    End Sub


    Private Sub NumericUpDown1_ValueChanged(sender As Object, e As EventArgs) Handles NumericUpDown1.ValueChanged
        If DataGridView1.SelectedRows.Count > 0 Then
            Dim selectedRow As DataGridViewRow = DataGridView1.SelectedRows(0)
            Dim borrowedDateStr As String = selectedRow.Cells("BorrowedDate").Value.ToString()
            Dim daysLimit As Decimal = NumericUpDown1.Value


            CalculateAndDisplayDueDate(daysLimit, borrowedDateStr)
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

    Private Sub BShown(sender As Object, e As EventArgs) Handles MyBase.Shown

        DataGridView1.ClearSelection()

    End Sub

    Private Sub LoadBookList(transactionReceiptID As String)
        Dim con As New MySqlConnection(connectionString)


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
            lblaccessionid.Text = "--"
            lblisbnbarcode.Text = "--"
            currentBookTitle = ""
            lblborroweddate.Text = "--"
            lblduedate.Text = "--"
            lbldetails.Visible = True
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


        Dim comDates As String = "SELECT `BorrowedDate`, `DueDate` FROM `borrowing_tbl` WHERE `AccessionID` = @AccessionID AND `TransactionReceipt` = @TID LIMIT 1"

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
                    lblduedate.Text = readerDates("DueDate").ToString()
                Else

                    If DataGridView1.SelectedRows.Count > 0 Then
                        Dim selectedRow As DataGridViewRow = DataGridView1.SelectedRows(0)
                        lblborroweddate.Text = selectedRow.Cells("BorrowedDate").Value.ToString()
                        lblduedate.Text = selectedRow.Cells("DueDate").Value.ToString()
                    Else
                        lblborroweddate.Text = "--"
                        lblduedate.Text = "--"
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
        lblduedate.Text = "--"

        currentBookTitle = ""
        lbldetails.Visible = False

        cbbooks.DataSource = Nothing
        cbbooks.Items.Clear()


        refreshconfirmation()
    End Sub
End Class