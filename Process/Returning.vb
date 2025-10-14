Imports System.Data
Imports MySql.Data.MySqlClient

Public Class Returning


    Private IsLoadingTransaction As Boolean = False

    Private Sub RefreshReturningData()
        Dim con As New MySqlConnection(GlobalVarsModule.connectionString)
        Dim com As String = "SELECT * FROM `returning_tbl` ORDER BY ID DESC"
        Dim adap As New MySqlDataAdapter(com, con)
        Dim ds As New DataSet

        Try
            adap.Fill(ds, "info")
            DataGridView1.DataSource = ds.Tables("info")
        Catch ex As Exception
            MessageBox.Show("Error loading Returning data: " & ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

        DataGridView1.Columns("ID").Visible = False
        DataGridView1.EnableHeadersVisualStyles = False
        DataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(207, 58, 109)
        DataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White
    End Sub

    Private Sub clear_details_only()

        lblborrowertype.Text = ""
        lbllrn.Text = ""
        lblemployeeno.Text = ""
        lblfullname.Text = ""
        lblbooktotal.Text = ""
        lblborroweddate.Text = ""
        lblisbnbarcode.Text = ""
        lblaccessionid.Text = ""
        lblgrade.Text = ""
        lblsection.Text = ""
        lblstrand.Text = ""
        lbldepartment.Text = ""
        cbbooks.Text = ""
        lblduedate.Text = ""

        cbbooks.Items.Clear()
        chkSelectAll.Checked = False
        cbbooks.Enabled = True
        chkSelectAll.Enabled = True

        rboverdue.Checked = False
        rbdamage.Checked = False
        rblost.Checked = False
        lbldamage.Visible = False
        cbdamage.Visible = False
        cbdamage.SelectedIndex = -1

        btnreturn.Enabled = True

    End Sub

    Private Sub InitializeConditionControls()
        lbldamage.Visible = False
        cbdamage.Visible = False

        If cbdamage.Items.Count = 0 Then
            cbdamage.Items.Add("Minor")
            cbdamage.Items.Add("Major")
            cbdamage.Items.Add("Irreparable")
        End If
        cbdamage.SelectedIndex = -1
    End Sub

    Private Sub returning_shown(sender As Object, e As EventArgs) Handles MyBase.Shown
        DataGridView1.ClearSelection()
        InitializeConditionControls()
    End Sub

    Private Sub Returning_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        RefreshReturningData()
        InitializeConditionControls()

        chkSelectAll.Checked = False
        cbbooks.Enabled = True

        rboverdue.Checked = False
        rbdamage.Checked = False
        rblost.Checked = False
    End Sub

    Private Sub rbdamage_CheckedChanged(sender As Object, e As EventArgs) Handles rbdamage.CheckedChanged
        If rbdamage.Checked Then
            lbldamage.Visible = True
            cbdamage.Visible = True
            rboverdue.Checked = False
            rblost.Checked = False
        Else
            lbldamage.Visible = False
            cbdamage.Visible = False
            cbdamage.SelectedIndex = -1
        End If
    End Sub

    Private Sub rblost_CheckedChanged(sender As Object, e As EventArgs) Handles rblost.CheckedChanged
        If rblost.Checked Then
            rbdamage.Checked = False
            rboverdue.Checked = False
        End If
    End Sub

    Private Sub rboverdue_CheckedChanged(sender As Object, e As EventArgs) Handles rboverdue.CheckedChanged
        If rboverdue.Checked Then
            rbdamage.Checked = False
            rblost.Checked = False
        End If
    End Sub

    Private Sub chkSelectAll_CheckedChanged(sender As Object, e As EventArgs) Handles chkSelectAll.CheckedChanged
        cbbooks.Enabled = Not chkSelectAll.Checked

        If chkSelectAll.Checked Then
            cbbooks.SelectedIndex = -1
            lblisbnbarcode.Text = "All books are selected will be returned."
            lblaccessionid.Text = ""
            lbldetails.Visible = False
        Else
            lblisbnbarcode.Text = ""
            lblaccessionid.Text = ""
            lbldetails.Visible = True
        End If
    End Sub

    Private Sub btnreturn_Click(sender As Object, e As EventArgs) Handles btnreturn.Click

        Dim TransactionNo As String = txttransactionreceipt.Text.Trim()
        Dim booksToReturn As New List(Of String)
        Dim bookStatus As String = "Normal"
        Dim newAccessionStatus As String = "Available"


        If rboverdue.Checked Then


            Dim dueDateString As String = lblborroweddate.Text
            Dim dueDate As Date

            If Date.TryParse(dueDateString, dueDate) Then

                If DateTime.Now.Date <= dueDate.Date Then
                    MessageBox.Show("Cannot set status to Overdue. The current date is not past the Due Date (" & dueDateString & ").", "Invalid Status", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    rboverdue.Checked = False
                    Return
                End If
            Else

                MessageBox.Show("Due Date format is invalid. Cannot check for Overdue status.", "Date Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If


            bookStatus = "Overdue"
            newAccessionStatus = "Available"

        ElseIf rbdamage.Checked Then
            If cbdamage.SelectedItem Is Nothing Then
                MessageBox.Show("Please select the level of damage (Minor, Major, Irreparable).", "Damage Required", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If
            bookStatus = "Damaged (" & cbdamage.SelectedItem.ToString() & ")"
            newAccessionStatus = "Damaged"
        ElseIf rblost.Checked Then
            bookStatus = "Lost"
            newAccessionStatus = "Lost"
        Else
            bookStatus = "Normal"
            newAccessionStatus = "Available"
        End If


        If String.IsNullOrWhiteSpace(lblfullname.Text) OrElse cbbooks.Items.Count = 0 Then
            MessageBox.Show("Please load a valid transaction receipt first.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        If chkSelectAll.Checked Then
            For Each item As Object In cbbooks.Items
                booksToReturn.Add(item.ToString())
            Next
        ElseIf cbbooks.SelectedItem IsNot Nothing Then
            booksToReturn.Add(cbbooks.SelectedItem.ToString())
        Else
            MessageBox.Show("Please select a book or check 'All' to return.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Dim con As New MySqlConnection(GlobalVarsModule.connectionString)
        Dim trans As MySqlTransaction = Nothing

        Try
            con.Open()
            trans = con.BeginTransaction()

            Dim returnedBookTitles As String = String.Join(" | ", booksToReturn)


            Dim insert_com As String = "INSERT INTO `returning_tbl` (`Borrower`, `LRN`, `EmployeeNo`, `FullName`, `Department`, `Grade`, `Section`, `Strand`, `ReturnedBook`, `BookTotal`, `BorrowedDate`, `DueDate`, `ReturnDate`, `TransactionReceipt`, `Status`) " &
                                   "VALUES (@borrowerType, @lrn, @empNo, @fullName, @dept, @grade, @section, @strand, @returnedBook, @bookTotal, @borrowDate, @dueDate, @returnDate, @transNo, @bookStatus)"

            Using insert_cmd As New MySqlCommand(insert_com, con, trans)
                insert_cmd.Parameters.AddWithValue("@borrowerType", lblborrowertype.Text)
                insert_cmd.Parameters.AddWithValue("@lrn", If(String.IsNullOrWhiteSpace(lbllrn.Text) OrElse lbllrn.Text = "N/A", Nothing, lbllrn.Text))
                insert_cmd.Parameters.AddWithValue("@empNo", If(String.IsNullOrWhiteSpace(lblemployeeno.Text) OrElse lblemployeeno.Text = "N/A", Nothing, lblemployeeno.Text))
                insert_cmd.Parameters.AddWithValue("@fullName", lblfullname.Text)


                insert_cmd.Parameters.AddWithValue("@dept", If(String.IsNullOrWhiteSpace(lbldepartment.Text), Nothing, lbldepartment.Text))
                insert_cmd.Parameters.AddWithValue("@grade", If(String.IsNullOrWhiteSpace(lblgrade.Text), Nothing, lblgrade.Text))
                insert_cmd.Parameters.AddWithValue("@section", If(String.IsNullOrWhiteSpace(lblsection.Text), Nothing, lblsection.Text))
                insert_cmd.Parameters.AddWithValue("@strand", If(String.IsNullOrWhiteSpace(lblstrand.Text), Nothing, lblstrand.Text))

                insert_cmd.Parameters.AddWithValue("@returnedBook", returnedBookTitles)
                insert_cmd.Parameters.AddWithValue("@bookTotal", booksToReturn.Count)
                insert_cmd.Parameters.AddWithValue("@borrowDate", lblborroweddate.Text)
                insert_cmd.Parameters.AddWithValue("@dueDate", lblduedate.Text)
                insert_cmd.Parameters.AddWithValue("@returnDate", DateTime.Now.ToShortDateString())
                insert_cmd.Parameters.AddWithValue("@transNo", TransactionNo)
                insert_cmd.Parameters.AddWithValue("@bookStatus", bookStatus)

                insert_cmd.ExecuteNonQuery()
            End Using

            For Each bookTitle As String In booksToReturn


                Dim get_accession_com As String = "SELECT `AccessionID` FROM `borrowing_tbl` WHERE `TransactionReceipt` = @transNo AND `BookTitle` = @bookTitle LIMIT 1"
                Dim accessionID As String = String.Empty

                Using get_accession_cmd As New MySqlCommand(get_accession_com, con, trans)
                    get_accession_cmd.Parameters.AddWithValue("@transNo", TransactionNo)
                    get_accession_cmd.Parameters.AddWithValue("@bookTitle", bookTitle)

                    Dim result As Object = get_accession_cmd.ExecuteScalar()
                    If result IsNot Nothing AndAlso result IsNot DBNull.Value Then
                        accessionID = result.ToString()
                    End If
                End Using


                If Not String.IsNullOrWhiteSpace(accessionID) Then
                    Dim update_accession_com As String = "UPDATE `acession_tbl` SET `Status` = @newStatus WHERE `AccessionID` = @accessionId"
                    Using update_accession_cmd As New MySqlCommand(update_accession_com, con, trans)
                        update_accession_cmd.Parameters.AddWithValue("@newStatus", newAccessionStatus)
                        update_accession_cmd.Parameters.AddWithValue("@accessionId", accessionID)
                        update_accession_cmd.ExecuteNonQuery()
                    End Using
                End If


                Dim delete_com As String = "DELETE FROM `borrowing_tbl` WHERE `TransactionReceipt` = @transNo AND `BookTitle` = @bookTitle LIMIT 1"
                Using delete_cmd As New MySqlCommand(delete_com, con, trans)
                    delete_cmd.Parameters.AddWithValue("@transNo", TransactionNo)
                    delete_cmd.Parameters.AddWithValue("@bookTitle", bookTitle)
                    delete_cmd.ExecuteNonQuery()
                End Using

            Next

            trans.Commit()


            For Each form In Application.OpenForms
                If TypeOf form Is Accession Then DirectCast(form, Accession).RefreshAccessionData()
                If TypeOf form Is MainForm Then DirectCast(form, MainForm).lblborrowcount()
            Next

            MessageBox.Show($"{booksToReturn.Count} book(s) returned successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)

            clear_click(sender, e)
            RefreshReturningData()

        Catch ex As Exception
            trans?.Rollback()
            MessageBox.Show("Error during book return: " & ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
        End Try
    End Sub

    Private Sub btnedit_Click(sender As Object, e As EventArgs) Handles btnedit.Click


        If DataGridView1.SelectedRows.Count = 0 Then
            MessageBox.Show("Please select a record from the history to edit the status.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If

        If cbbooks.SelectedItem Is Nothing Then
            MessageBox.Show("Please select the specific book from the list (dropdown) to change its status.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Dim selectedRow As DataGridViewRow = DataGridView1.SelectedRows(0)
        Dim bookToSplitTitle As String = cbbooks.SelectedItem.ToString()
        Dim transacReceipt As String = txttransactionreceipt.Text
        Dim originalID As Integer = Convert.ToInt32(selectedRow.Cells("ID").Value)
        Dim originalBookTotal As Integer = Convert.ToInt32(selectedRow.Cells("BookTotal").Value)
        Dim currentStatus As String = selectedRow.Cells("Status").Value?.ToString()

        Dim accessionID As String = String.Empty

        Dim con As New MySqlConnection(GlobalVarsModule.connectionString)
        Dim trans As MySqlTransaction = Nothing

        Try
            con.Open()


            Dim bookStatus As String = "Normal"
            Dim newAccessionStatus As String = "Available"

            If rbdamage.Checked Then
                If cbdamage.SelectedItem Is Nothing Then
                    MessageBox.Show("Please select the level of damage (Minor, Major, Irreparable).", "Damage Required", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Return
                End If
                bookStatus = "Damaged (" & cbdamage.SelectedItem.ToString() & ")"
                newAccessionStatus = "Damaged"
            ElseIf rblost.Checked Then
                bookStatus = "Lost"
                newAccessionStatus = "Lost"
            ElseIf rboverdue.Checked Then


                Dim dueDateString As String = selectedRow.Cells("DueDate").Value?.ToString()
                Dim dueDate As Date

                If Date.TryParse(dueDateString, dueDate) Then

                    If DateTime.Now.Date <= dueDate.Date Then
                        MessageBox.Show("Cannot change status to Overdue. The current date is not past the original Due Date (" & dueDateString & ").", "Invalid Status", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        rboverdue.Checked = False
                        Return
                    End If
                Else

                    MessageBox.Show("Due Date format is invalid. Cannot check for Overdue status.", "Date Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Return
                End If


                bookStatus = "Overdue"
                newAccessionStatus = "Available"
            Else

                If currentStatus IsNot Nothing AndAlso currentStatus.ToLower().Contains("normal") Then
                    If MessageBox.Show("No new status is selected. Do you want to proceed and keep the status as 'Normal'?", "Confirm Edit", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.No Then
                        Return
                    End If
                End If
            End If

            Dim get_accession_com As String = "SELECT `AccessionID` FROM `borrowinghistory_tbl` WHERE `TransactionReceipt` = @transNo AND `BookTitle` = @bookTitle LIMIT 1"

            Using get_accession_cmd As New MySqlCommand(get_accession_com, con)
                get_accession_cmd.Parameters.AddWithValue("@transNo", transacReceipt)
                get_accession_cmd.Parameters.AddWithValue("@bookTitle", bookToSplitTitle)

                Dim result As Object = get_accession_cmd.ExecuteScalar()
                If result IsNot Nothing AndAlso result IsNot DBNull.Value Then
                    accessionID = result.ToString()
                End If
            End Using

            If accessionID = "N/A" OrElse String.IsNullOrWhiteSpace(accessionID) Then
                MessageBox.Show("Cannot proceed with edit. AccessionID for the selected book is missing from the borrowing history. Please ensure the book details are loaded correctly.", "Accession ID Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If


            trans = con.BeginTransaction()


            Dim update_accession_com As String = "UPDATE `acession_tbl` SET `Status` = @newStatus WHERE `AccessionID` = @accessionId"
            Using update_accession_cmd As New MySqlCommand(update_accession_com, con, trans)
                update_accession_cmd.Parameters.AddWithValue("@newStatus", newAccessionStatus)
                update_accession_cmd.Parameters.AddWithValue("@accessionId", accessionID)
                update_accession_cmd.ExecuteNonQuery()
            End Using



            Dim insert_com As String = "INSERT INTO `returning_tbl` (`Borrower`, `LRN`, `EmployeeNo`, `FullName`, `Department`, `Grade`, `Section`, `Strand`, `ReturnedBook`, `BookTotal`, `BorrowedDate`, `DueDate`, `ReturnDate`, `TransactionReceipt`, `Status`) " &
                                   "VALUES (@borrowerType, @lrn, @empNo, @fullName, @dept, @grade, @section, @strand, @returnedBook, 1, @borrowDate, @dueDate, @returnDate, @transNo, @bookStatus)"

            Using insert_cmd As New MySqlCommand(insert_com, con, trans)
                insert_cmd.Parameters.AddWithValue("@borrowerType", lblborrowertype.Text)
                insert_cmd.Parameters.AddWithValue("@lrn", If(String.IsNullOrWhiteSpace(lbllrn.Text) OrElse lbllrn.Text = "N/A", Nothing, lbllrn.Text))
                insert_cmd.Parameters.AddWithValue("@empNo", If(String.IsNullOrWhiteSpace(lblemployeeno.Text) OrElse lblemployeeno.Text = "N/A", Nothing, lblemployeeno.Text))
                insert_cmd.Parameters.AddWithValue("@fullName", lblfullname.Text)


                insert_cmd.Parameters.AddWithValue("@dept", If(String.IsNullOrWhiteSpace(lbldepartment.Text), Nothing, lbldepartment.Text))
                insert_cmd.Parameters.AddWithValue("@grade", If(String.IsNullOrWhiteSpace(lblgrade.Text), Nothing, lblgrade.Text))
                insert_cmd.Parameters.AddWithValue("@section", If(String.IsNullOrWhiteSpace(lblsection.Text), Nothing, lblsection.Text))
                insert_cmd.Parameters.AddWithValue("@strand", If(String.IsNullOrWhiteSpace(lblstrand.Text), Nothing, lblstrand.Text))

                insert_cmd.Parameters.AddWithValue("@returnedBook", bookToSplitTitle)
                insert_cmd.Parameters.AddWithValue("@borrowDate", selectedRow.Cells("BorrowedDate").Value?.ToString())
                insert_cmd.Parameters.AddWithValue("@dueDate", selectedRow.Cells("DueDate").Value?.ToString())
                insert_cmd.Parameters.AddWithValue("@returnDate", DateTime.Now.ToShortDateString())
                insert_cmd.Parameters.AddWithValue("@transNo", transacReceipt)
                insert_cmd.Parameters.AddWithValue("@bookStatus", bookStatus)
                insert_cmd.ExecuteNonQuery()
            End Using


            Dim newBookTotal As Integer = originalBookTotal - 1


            Dim currentBooks As New List(Of String)(cbbooks.Items.Cast(Of String)())
            currentBooks.Remove(bookToSplitTitle)
            Dim newReturnedBookList As String = String.Join(" | ", currentBooks)

            Dim update_com As String = "UPDATE `returning_tbl` SET `ReturnedBook` = @newReturnedBook, `BookTotal` = @newBookTotal " &
                                   "WHERE `ID` = @originalID"

            Using update_cmd As New MySqlCommand(update_com, con, trans)
                update_cmd.Parameters.AddWithValue("@newReturnedBook", newReturnedBookList)
                update_cmd.Parameters.AddWithValue("@newBookTotal", newBookTotal)
                update_cmd.Parameters.AddWithValue("@originalID", originalID)
                update_cmd.ExecuteNonQuery()
            End Using


            If newBookTotal = 0 Then
                Dim delete_original_com As String = "DELETE FROM `returning_tbl` WHERE `ID` = @originalID"
                Using delete_original_cmd As New MySqlCommand(delete_original_com, con, trans)
                    delete_original_cmd.Parameters.AddWithValue("@originalID", originalID)
                    delete_original_cmd.ExecuteNonQuery()
                End Using
            End If

            trans.Commit()


            For Each form In Application.OpenForms
                If TypeOf form Is Accession Then DirectCast(form, Accession).RefreshAccessionData()
            Next

            MessageBox.Show($"Book status for '{bookToSplitTitle}' updated to '{bookStatus}' successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)

            clear_click(sender, e)
            RefreshReturningData()

        Catch ex As Exception
            trans?.Rollback()
            MessageBox.Show("Error during book status edit: " & ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
        End Try
    End Sub
    Private Sub clear_click(sender As Object, e As EventArgs) Handles btnclear.Click
        txttransactionreceipt.Clear()
        clear_details_only()
    End Sub

    Private Sub LoadBorrowerDetails(ByVal identifierValue As String, ByVal identifierColumn As String)


        lblgrade.Text = ""
        lbldepartment.Text = ""
        lblsection.Text = ""
        lblstrand.Text = ""
        lbllrn.Text = ""
        lblemployeeno.Text = ""

        Dim con As New MySqlConnection(GlobalVarsModule.connectionString)
        Dim com As String = String.Empty
        Dim borrowerType As String = ""

        If identifierColumn = "LRN" Then
            com = "SELECT Borrower, LRN, Grade, Section, Strand, Department FROM `borrower_tbl` " &
              "WHERE `LRN` = @IdentifierValue LIMIT 1"
            borrowerType = "Student"
        ElseIf identifierColumn = "EmployeeNo" Then
            com = "SELECT Borrower, EmployeeNo, Department FROM `borrower_tbl` " &
              "WHERE `EmployeeNo` = @IdentifierValue LIMIT 1"
            borrowerType = "Teacher"
        Else
            Return
        End If

        Try
            con.Open()
            Using cmd As New MySqlCommand(com, con)
                cmd.Parameters.AddWithValue("@IdentifierValue", identifierValue)

                Using reader As MySqlDataReader = cmd.ExecuteReader()
                    If reader.Read() Then

                        If identifierColumn = "LRN" Then
                            lbllrn.Text = reader("LRN").ToString()
                            lblgrade.Text = reader("Grade").ToString()
                            lblsection.Text = reader("Section").ToString()
                            lblstrand.Text = reader("Strand").ToString()
                            lbldepartment.Text = reader("Department").ToString()
                            lblemployeeno.Text = ""
                        Else
                            lblemployeeno.Text = reader("EmployeeNo").ToString()
                            lbldepartment.Text = reader("Department").ToString()
                            lbllrn.Text = ""
                            lblgrade.Text = ""
                            lblsection.Text = ""
                            lblstrand.Text = ""
                        End If
                    Else
                        lbllrn.Text = "N/A"
                        lblemployeeno.Text = "N/A"

                        lblgrade.Text = ""
                        lbldepartment.Text = ""
                        lblsection.Text = ""
                        lblstrand.Text = ""
                    End If
                End Using
            End Using

        Catch ex As Exception
            MessageBox.Show("Error loading borrower details from borrower_tbl: " & ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            If con.State = ConnectionState.Open Then con.Close()
        End Try
    End Sub

    Private Sub txttransactionreceipt_TextChanged(sender As Object, e As EventArgs) Handles txttransactionreceipt.TextChanged


        If IsLoadingTransaction Then Return

        Const MIN_LENGTH As Integer = 11

        clear_details_only()
        cbbooks.Items.Clear()

        Dim TransactionNo As String = txttransactionreceipt.Text.Trim()

        If String.IsNullOrWhiteSpace(TransactionNo) OrElse TransactionNo.Length < MIN_LENGTH Then
            Return
        End If

        Dim con As New MySqlConnection(GlobalVarsModule.connectionString)

        Try
            con.Open()

            Dim borrowerIDValue As String = ""
            Dim borrowerIDColumn As String = ""
            Dim borrowerType As String = ""
            Dim borrowerName As String = ""

            Dim com_id As String = "SELECT `LRN`, `EmployeeNo`, `Name`, `Borrower` FROM `borrowing_tbl` " &
                               "WHERE `TransactionReceipt` = @transNo LIMIT 1"

            Using comsi_id As New MySqlCommand(com_id, con)
                comsi_id.Parameters.AddWithValue("@transNo", TransactionNo)

                Using reader_id As MySqlDataReader = comsi_id.ExecuteReader()
                    If reader_id.Read() Then
                        borrowerName = reader_id("Name").ToString()
                        borrowerType = reader_id("Borrower").ToString()

                        If borrowerType.ToLower() = "student" Then
                            borrowerIDValue = reader_id("LRN").ToString()
                            borrowerIDColumn = "LRN"
                        Else
                            borrowerIDValue = reader_id("EmployeeNo").ToString()
                            borrowerIDColumn = "EmployeeNo"
                        End If

                        reader_id.Close()
                    Else
                        reader_id.Close()

                        MessageBox.Show("This transaction is complete or record not found in active borrowings.", "Transaction Info", MessageBoxButtons.OK, MessageBoxIcon.Information)
                        Return
                    End If
                End Using
            End Using


            Dim com_header As String = "SELECT `BorrowedDate`, `DueDate`, `BorrowedBookCount` " &
                                   "FROM `printreceipt_tbl` " &
                                   "WHERE `TransactionReceipt` = @transNo LIMIT 1"

            Using comsi_header As New MySqlCommand(com_header, con)
                comsi_header.Parameters.AddWithValue("@transNo", TransactionNo)

                Using reader_header As MySqlDataReader = comsi_header.ExecuteReader()
                    If reader_header.Read() Then
                        lblfullname.Text = borrowerName
                        lblborrowertype.Text = borrowerType
                        lblborroweddate.Text = Convert.ToDateTime(reader_header("BorrowedDate")).ToShortDateString()
                        lblduedate.Text = Convert.ToDateTime(reader_header("DueDate")).ToShortDateString()
                        lblbooktotal.Text = reader_header("BorrowedBookCount").ToString() & " (Total)"

                        reader_header.Close()
                    Else
                        reader_header.Close()
                        Return '
                    End If
                End Using
            End Using


            LoadBorrowerDetails(borrowerIDValue, borrowerIDColumn)

            Dim com_items As String = "SELECT `BookTitle` FROM `borrowing_tbl` " &
                                 "WHERE `TransactionReceipt` = @transNo"

            Using comsi_items As New MySqlCommand(com_items, con)
                comsi_items.Parameters.AddWithValue("@transNo", TransactionNo)

                Using reader_items As MySqlDataReader = comsi_items.ExecuteReader()
                    While reader_items.Read()
                        cbbooks.Items.Add(reader_items("BookTitle").ToString())
                    End While

                    If cbbooks.Items.Count = 0 Then
                        reader_items.Close()
                        If Not IsLoadingTransaction Then
                            MessageBox.Show("This transaction is complete. All borrowed books have been returned.", "Transaction Complete", MessageBoxButtons.OK, MessageBoxIcon.Information)
                        End If
                        Return
                    Else
                        cbbooks.SelectedIndex = 0
                    End If
                End Using
            End Using

        Catch ex As Exception
            MessageBox.Show("Database Error: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
        End Try
    End Sub


    Private Sub cbbooks_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbbooks.SelectedIndexChanged

        If cbbooks.SelectedItem Is Nothing Then
            lblisbnbarcode.Text = ""
            lblaccessionid.Text = ""
            Return
        End If


        chkSelectAll.Checked = False

        Dim selectedBookTitle As String = cbbooks.SelectedItem.ToString()
        Dim TransactionNo As String = txttransactionreceipt.Text

        If String.IsNullOrWhiteSpace(TransactionNo) Then
            lblisbnbarcode.Text = "N/A"
            lblaccessionid.Text = "N/A"
            Return
        End If

        Dim con As New MySqlConnection(GlobalVarsModule.connectionString)

        Try
            con.Open()

            Dim accessionID As String = "N/A"
            Dim isbnBarcode As String = "N/A"


            Dim com_active As String = "SELECT `ISBN`, `Barcode`, `AccessionID` FROM `borrowing_tbl` " &
                                       "WHERE `TransactionReceipt` = @transNo AND `BookTitle` = @bookTitle LIMIT 1"

            Using comsi_active As New MySqlCommand(com_active, con)
                comsi_active.Parameters.AddWithValue("@transNo", TransactionNo)
                comsi_active.Parameters.AddWithValue("@bookTitle", selectedBookTitle)

                Using reader_active As MySqlDataReader = comsi_active.ExecuteReader()
                    If reader_active.Read() Then
                        Dim isbn As String = reader_active("ISBN").ToString()
                        Dim barcode As String = reader_active("Barcode").ToString()
                        accessionID = reader_active("AccessionID").ToString()
                        isbnBarcode = If(String.IsNullOrWhiteSpace(barcode), isbn, barcode)
                    End If
                End Using
            End Using


            If accessionID = "N/A" Then
                Dim com_history As String = "SELECT `ISBN`, `Barcode`, `AccessionID` FROM `borrowinghistory_tbl` " &
                                            "WHERE `TransactionReceipt` = @transNo AND `BookTitle` = @bookTitle LIMIT 1"

                Using comsi_history As New MySqlCommand(com_history, con)
                    comsi_history.Parameters.AddWithValue("@transNo", TransactionNo)
                    comsi_history.Parameters.AddWithValue("@bookTitle", selectedBookTitle)

                    Using reader_history As MySqlDataReader = comsi_history.ExecuteReader()
                        If reader_history.Read() Then
                            Dim isbn As String = reader_history("ISBN").ToString()
                            Dim barcode As String = reader_history("Barcode").ToString()
                            accessionID = reader_history("AccessionID").ToString()
                            isbnBarcode = If(String.IsNullOrWhiteSpace(barcode), isbn, barcode)
                        End If
                    End Using
                End Using
            End If

            lblisbnbarcode.Text = isbnBarcode
            lblaccessionid.Text = accessionID

        Catch ex As Exception

            lblisbnbarcode.Text = "N/A (Book details error)"
            lblaccessionid.Text = "N/A"
            MessageBox.Show("Error getting book details: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
        End Try
    End Sub

    Private Sub DataGridView1_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellClick

        If e.RowIndex < 0 OrElse DataGridView1.Rows.Count = 0 Then
            clear_details_only()

            btnreturn.Enabled = True
            btnedit.Enabled = False
            Exit Sub
        End If

        IsLoadingTransaction = True

        clear_details_only()

        DataGridView1.ClearSelection()
        DataGridView1.Rows(e.RowIndex).Selected = True

        Dim selectedRow As DataGridViewRow = DataGridView1.Rows(e.RowIndex)

        lblborrowertype.Text = selectedRow.Cells("Borrower").Value?.ToString()
        lblfullname.Text = selectedRow.Cells("FullName").Value?.ToString()
        lblbooktotal.Text = selectedRow.Cells("BookTotal").Value?.ToString()
        lblborroweddate.Text = selectedRow.Cells("BorrowedDate").Value?.ToString()
        lblduedate.Text = selectedRow.Cells("DueDate").Value?.ToString()


        lbldepartment.Text = selectedRow.Cells("Department").Value?.ToString()
        lblgrade.Text = selectedRow.Cells("Grade").Value?.ToString()
        lblsection.Text = selectedRow.Cells("Section").Value?.ToString()
        lblstrand.Text = selectedRow.Cells("Strand").Value?.ToString()


        lbllrn.Text = selectedRow.Cells("LRN").Value?.ToString()
        lblemployeeno.Text = selectedRow.Cells("EmployeeNo").Value?.ToString()
        txttransactionreceipt.Text = selectedRow.Cells("TransactionReceipt").Value?.ToString()

        IsLoadingTransaction = False


        Dim returnedBooksString As String = selectedRow.Cells("ReturnedBook").Value?.ToString()

        cbbooks.Items.Clear()

        If Not String.IsNullOrWhiteSpace(returnedBooksString) Then

            Dim booksArray As String() = returnedBooksString.Split(New String() {" | "}, StringSplitOptions.RemoveEmptyEntries)
            cbbooks.Items.AddRange(booksArray)

            If booksArray.Length > 0 Then
                cbbooks.SelectedIndex = 0
                chkSelectAll.Enabled = False
                cbbooks.Enabled = True
            Else
                cbbooks.Enabled = False
                chkSelectAll.Enabled = False
            End If
        Else
            cbbooks.Enabled = False
            chkSelectAll.Enabled = False
        End If

        rboverdue.Checked = False
        rbdamage.Checked = False
        rblost.Checked = False
        cbdamage.SelectedIndex = -1

        btnreturn.Enabled = False
        btnedit.Enabled = True
        btnclear.Enabled = True


    End Sub

    Private Sub txtsearch_TextChanged(sender As Object, e As EventArgs) Handles txtsearch.TextChanged

        Dim dt As DataTable = DirectCast(DataGridView1.DataSource, DataTable)
        If dt IsNot Nothing Then
            If txtsearch.Text.Trim() <> "" Then

                Dim filter As String = String.Format("Borrower LIKE '%{0}%' OR BookTitle LIKE '%{0}%'", txtsearch.Text.Trim())

                dt.DefaultView.RowFilter = filter
            Else
                dt.DefaultView.RowFilter = ""
            End If
        End If

    End Sub
End Class