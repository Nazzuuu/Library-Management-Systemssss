Imports System.Data
Imports MySql.Data.MySqlClient

Public Class Returning


    Private IsLoadingTransaction As Boolean = False

    Public Sub RefreshReturningData()
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
        clear_details_only()
    End Sub

    Private Sub clear_details_only()

        If DataGridView1.SelectedRows.Count > 0 Then
            DataGridView1.ClearSelection()
        End If


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
        Dim bookStatusDescription As String = String.Empty
        Dim borrowerStatus As String = "NOT PENALIZED"


        If rboverdue.Checked Then
            Dim dueDateString As String = lblduedate.Text
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
            bookStatusDescription = "Overdue"
            borrowerStatus = "NOT PENALIZED"

        ElseIf rbdamage.Checked Then
            If cbdamage.SelectedItem Is Nothing Then
                MessageBox.Show("Please select the level of damage (Minor, Major, Irreparable).", "Damage Required", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If
            bookStatus = "Damaged (" & cbdamage.SelectedItem.ToString() & ")"
            newAccessionStatus = "Damaged"
            bookStatusDescription = "Damaged"
            borrowerStatus = "NOT PENALIZED"

        ElseIf rblost.Checked Then
            bookStatus = "Lost"
            newAccessionStatus = "Lost"
            bookStatusDescription = "Lost"
            borrowerStatus = "NOT PENALIZED"

        Else


            Dim dueDateStringCheck As String = lblduedate.Text
            Dim dueDateCheck As Date

            If Date.TryParse(dueDateStringCheck, dueDateCheck) Then

                If DateTime.Now.Date > dueDateCheck.Date Then
                    MessageBox.Show("Book is overdue. Please select 'Overdue' status.", "Status Conflict", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Return
                End If
            Else

                MessageBox.Show("Due Date format is invalid. Cannot check for Overdue status.", "Date Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If


            bookStatus = "Normal"
            newAccessionStatus = "Available"
            bookStatusDescription = "Normal Return"
            borrowerStatus = "N/A"
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

            Dim insert_com As String = "INSERT INTO `returning_tbl` " &
                                 "(`Borrower`, `LRN`, `EmployeeNo`, `FullName`, `Department`, `Grade`, `Section`, `Strand`, `ReturnedBook`, `BookTotal`, `BorrowedDate`, `DueDate`, `ReturnDate`, `TransactionReceipt`, `Status`, `BorrowerStatus`) " &
                                 "VALUES (@borrowerType, @lrn, @empNo, @fullName, @dept, @grade, @section, @strand, @returnedBook, @bookTotal, @borrowDate, @dueDate, @returnDate, @transNo, @bookStatus, @borrowerStatus)"

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
                insert_cmd.Parameters.AddWithValue("@borrowerStatus", borrowerStatus)
                insert_cmd.ExecuteNonQuery()
            End Using

            If rboverdue.Checked OrElse rbdamage.Checked OrElse rblost.Checked Then
                Dim insert_penalty_com As String = "INSERT INTO `penalty_tbl` " &
                                               "(`Borrower`, `LRN`, `EmployeeNo`, `FullName`, `Department`, `Grade`, `Section`, `Strand`, `ReturnedBook`, `BookTotal`, `BorrowedDate`, `DueDate`, `ReturnDate`, `TransactionReceipt`, `Status`, `BorrowerStatus`) " &
                                               "VALUES (@borrowerType, @lrn, @empNo, @fullName, @dept, @grade, @section, @strand, @returnedBook, @bookTotal, @borrowDate, @dueDate, @returnDate, @transNo, @bookStatus, @borrowerStatus)"

                Using insert_penalty_cmd As New MySqlCommand(insert_penalty_com, con, trans)
                    insert_penalty_cmd.Parameters.AddWithValue("@borrowerType", lblborrowertype.Text)
                    insert_penalty_cmd.Parameters.AddWithValue("@lrn", If(String.IsNullOrWhiteSpace(lbllrn.Text) OrElse lbllrn.Text = "N/A", Nothing, lbllrn.Text))
                    insert_penalty_cmd.Parameters.AddWithValue("@empNo", If(String.IsNullOrWhiteSpace(lblemployeeno.Text) OrElse lblemployeeno.Text = "N/A", Nothing, lblemployeeno.Text))
                    insert_penalty_cmd.Parameters.AddWithValue("@fullName", lblfullname.Text)
                    insert_penalty_cmd.Parameters.AddWithValue("@dept", If(String.IsNullOrWhiteSpace(lbldepartment.Text), Nothing, lbldepartment.Text))
                    insert_penalty_cmd.Parameters.AddWithValue("@grade", If(String.IsNullOrWhiteSpace(lblgrade.Text), Nothing, lblgrade.Text))
                    insert_penalty_cmd.Parameters.AddWithValue("@section", If(String.IsNullOrWhiteSpace(lblsection.Text), Nothing, lblsection.Text))
                    insert_penalty_cmd.Parameters.AddWithValue("@strand", If(String.IsNullOrWhiteSpace(lblstrand.Text), Nothing, lblstrand.Text))
                    insert_penalty_cmd.Parameters.AddWithValue("@returnedBook", returnedBookTitles)
                    insert_penalty_cmd.Parameters.AddWithValue("@bookTotal", booksToReturn.Count)
                    insert_penalty_cmd.Parameters.AddWithValue("@borrowDate", lblborroweddate.Text)
                    insert_penalty_cmd.Parameters.AddWithValue("@dueDate", lblduedate.Text)
                    insert_penalty_cmd.Parameters.AddWithValue("@returnDate", DateTime.Now.ToShortDateString())
                    insert_penalty_cmd.Parameters.AddWithValue("@transNo", TransactionNo)
                    insert_penalty_cmd.Parameters.AddWithValue("@bookStatus", bookStatus)
                    insert_penalty_cmd.Parameters.AddWithValue("@borrowerStatus", borrowerStatus)
                    insert_penalty_cmd.ExecuteNonQuery()
                End Using
            End If


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

                If newAccessionStatus = "Available" OrElse (newAccessionStatus = "Damaged" AndAlso bookStatus.Contains("Minor")) Then
                    Dim update_acquisition_com As String = "UPDATE `acquisition_tbl` SET `Quantity` = `Quantity` + 1 WHERE `BookTitle` = @bookTitle"
                    Using update_acquisition_cmd As New MySqlCommand(update_acquisition_com, con, trans)
                        update_acquisition_cmd.Parameters.AddWithValue("@bookTitle", bookTitle)
                        update_acquisition_cmd.ExecuteNonQuery()
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


            GlobalVarsModule.LogAudit(
            actionType:="ADD",
            formName:="BOOK RETURN",
            description:=$"Returned {booksToReturn.Count} book(s) for transaction {TransactionNo}. Status: {bookStatusDescription}.",
            recordID:=TransactionNo,
            oldValue:=$"Borrower: {lblfullname.Text}",
            newValue:=$"Returned Books: {String.Join(" | ", booksToReturn)}"
        )

            For Each form In Application.OpenForms
                If TypeOf form Is AuditTrail Then DirectCast(form, AuditTrail).refreshaudit()
                If TypeOf form Is Accession Then DirectCast(form, Accession).RefreshAccessionData()
                If TypeOf form Is MainForm Then
                    DirectCast(form, MainForm).lblborrowcount()
                    DirectCast(form, MainForm).lblreturncount()
                End If
            Next

            MessageBox.Show($"{booksToReturn.Count} book(s) returned successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
            clear_click(sender, e)
            RefreshReturningData()

            Dim finalStatus As String = GetPenaltyStatus(TransactionNo)


        Catch ex As Exception
            trans?.Rollback()
            MessageBox.Show("Error during book return: " & ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            If con.State = ConnectionState.Open Then con.Close()
        End Try
    End Sub


    Private Sub btnedit_Click(sender As Object, e As EventArgs) Handles btnedit.Click


        If DataGridView1.SelectedRows.Count = 0 Then
            MessageBox.Show("Please select a record from the history to edit the status.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If

        Dim selectedRow As DataGridViewRow = DataGridView1.SelectedRows(0)
        Dim transacReceipt As String = txttransactionreceipt.Text


        Dim penaltyStatus As String = GetPenaltyStatus(transacReceipt)

        If penaltyStatus = "PENALIZED" Then
            MessageBox.Show("Cannot edit status. The penalty for this transaction has already been settled.", "Operation Not Allowed", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
            clear_details_only()
            Return
        End If


        If cbbooks.SelectedItem Is Nothing Then
            MessageBox.Show("Please select the specific book from the list (dropdown) to change its status.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If


        Dim bookToSplitTitle As String = cbbooks.SelectedItem.ToString()
        Dim originalID As Integer = Convert.ToInt32(selectedRow.Cells("ID").Value)
        Dim originalBookTotal As Integer = Convert.ToInt32(selectedRow.Cells("BookTotal").Value)
        Dim currentStatus As String = selectedRow.Cells("Status").Value?.ToString()


        Dim oldAccessionStatus As String = String.Empty
        Dim tempCon As New MySqlConnection(GlobalVarsModule.connectionString)
        Try
            tempCon.Open()

            Dim get_old_accession_status_com As String = "SELECT T1.Status FROM acession_tbl T1 JOIN borrowinghistory_tbl T2 ON T1.AccessionID = T2.AccessionID WHERE T2.TransactionReceipt = @transNo AND T2.BookTitle = @bookTitle LIMIT 1"
            Using get_old_accession_status_cmd As New MySqlCommand(get_old_accession_status_com, tempCon)
                get_old_accession_status_cmd.Parameters.AddWithValue("@transNo", transacReceipt)
                get_old_accession_status_cmd.Parameters.AddWithValue("@bookTitle", bookToSplitTitle)
                Dim result As Object = get_old_accession_status_cmd.ExecuteScalar()
                If result IsNot Nothing AndAlso result IsNot DBNull.Value Then
                    oldAccessionStatus = result.ToString()
                Else
                    oldAccessionStatus = "N/A or Missing in Accession Table"
                End If
            End Using
        Catch ex As Exception
            oldAccessionStatus = "Error Fetching Status"
        Finally
            If tempCon.State = ConnectionState.Open Then tempCon.Close()
        End Try


        Dim accessionID As String = String.Empty

        Dim con As New MySqlConnection(GlobalVarsModule.connectionString)
        Dim trans As MySqlTransaction = Nothing

        Try
            con.Open()

            Dim bookStatus As String = "Normal"
            Dim newAccessionStatus As String = "Available"
            Dim bookStatusDescription As String = "Normal"
            Dim borrowerStatus As String = "N/A"

            If rbdamage.Checked Then
                If cbdamage.SelectedItem Is Nothing Then
                    MessageBox.Show("Please select the level of damage (Minor, Major, Irreparable).", "Damage Required", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Return
                End If
                bookStatus = "Damaged (" & cbdamage.SelectedItem.ToString() & ")"
                newAccessionStatus = "Damaged"
                bookStatusDescription = "Damaged"
                borrowerStatus = "NOT PENALIZED"

            ElseIf rblost.Checked Then
                bookStatus = "Lost"
                newAccessionStatus = "Lost"
                bookStatusDescription = "Lost"
                borrowerStatus = "NOT PENALIZED"

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
                bookStatusDescription = "Overdue"
                borrowerStatus = "NOT PENALIZED"

            Else
                Dim dueDateStringCheck As String = selectedRow.Cells("DueDate").Value?.ToString()
                Dim dueDateCheck As Date

                If Date.TryParse(dueDateStringCheck, dueDateCheck) Then
                    If DateTime.Now.Date > dueDateCheck.Date Then
                        MessageBox.Show("Cannot edit as normal. This book is already overdue, Please select another status.", "Status Conflict", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        Return
                    End If
                Else
                End If

                If currentStatus IsNot Nothing AndAlso currentStatus.ToLower().Contains("normal") Then
                    If MessageBox.Show("No new status is selected. Do you want to proceed and keep the status as 'Normal'?", "Confirm Edit", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.No Then
                        Return
                    End If
                    borrowerStatus = "N/A"
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


            Dim identifier_isbn As String = String.Empty
            Dim identifier_barcode As String = String.Empty
            Dim get_identifiers_com As String = "SELECT ISBN, Barcode FROM acession_tbl WHERE AccessionID = @accessionId"
            Using get_identifiers_cmd As New MySqlCommand(get_identifiers_com, con, trans)
                get_identifiers_cmd.Parameters.AddWithValue("@accessionId", accessionID)


                Using reader As MySqlDataReader = get_identifiers_cmd.ExecuteReader()
                    If reader.Read() Then
                        identifier_isbn = If(reader("ISBN") Is DBNull.Value, String.Empty, reader("ISBN").ToString().Trim())
                        identifier_barcode = If(reader("Barcode") Is DBNull.Value, String.Empty, reader("Barcode").ToString().Trim())
                    End If
                End Using
            End Using

            Dim quantityUpdated As Boolean = False
            Dim identifierToUpdate As String = If(Not String.IsNullOrWhiteSpace(identifier_isbn), identifier_isbn, identifier_barcode)
            Dim identifierColumn As String = If(Not String.IsNullOrWhiteSpace(identifier_isbn), "ISBN", "Barcode")


            If bookStatus = "Lost" AndAlso oldAccessionStatus.ToUpper() <> "LOST" Then

                If Not String.IsNullOrWhiteSpace(identifierToUpdate) Then
                    Dim update_com As String = $"UPDATE `acquisition_tbl` SET `Quantity` = `Quantity` - 1 WHERE `{identifierColumn}` = @identifier AND `Quantity` > 0"
                    Using update_cmd As New MySqlCommand(update_com, con, trans)
                        update_cmd.Parameters.AddWithValue("@identifier", identifierToUpdate)
                        If update_cmd.ExecuteNonQuery() > 0 Then
                            quantityUpdated = True
                        End If
                    End Using
                End If

                If Not quantityUpdated Then

                End If


            ElseIf oldAccessionStatus.ToUpper() = "LOST" AndAlso (newAccessionStatus.ToUpper() = "AVAILABLE" OrElse newAccessionStatus.ToUpper() = "DAMAGED") Then


                If Not String.IsNullOrWhiteSpace(identifierToUpdate) Then
                    Dim restore_com As String = $"UPDATE `acquisition_tbl` SET `Quantity` = `Quantity` + 1 WHERE `{identifierColumn}` = @identifier"
                    Using restore_cmd As New MySqlCommand(restore_com, con, trans)
                        restore_cmd.Parameters.AddWithValue("@identifier", identifierToUpdate)
                        If restore_cmd.ExecuteNonQuery() > 0 Then
                            quantityUpdated = True

                        End If
                    End Using
                End If

                If Not quantityUpdated Then
                    MessageBox.Show("Warning: Failed to restore book quantity in Acquisition table. Please check book identifiers.", "Data Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                End If
            End If


            Dim insert_com As String = "INSERT INTO `returning_tbl` (`Borrower`, `LRN`, `EmployeeNo`, `FullName`, `Department`, `Grade`, `Section`, `Strand`, `ReturnedBook`, `BookTotal`, `BorrowedDate`, `DueDate`, `ReturnDate`, `TransactionReceipt`, `Status`, `BorrowerStatus`) " &
                                     "VALUES (@borrowerType, @lrn, @empNo, @fullName, @dept, @grade, @section, @strand, @returnedBook, 1, @borrowDate, @dueDate, @returnDate, @transNo, @bookStatus, @borrowerStatus)" ' <--- Tama: Kasama na ang BorrowerStatus

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
                insert_cmd.Parameters.AddWithValue("@borrowerStatus", borrowerStatus)
                insert_cmd.ExecuteNonQuery()
            End Using


            Dim newBookTotal As Integer = originalBookTotal - 1
            Dim newReturnedBookList As String = String.Empty

            Dim originalReturnedBooksString As String = selectedRow.Cells("ReturnedBook").Value?.ToString()

            If originalReturnedBooksString IsNot Nothing Then
                Dim currentBooksInOriginalRow As New List(Of String)(originalReturnedBooksString.Split(New String() {" | "}, StringSplitOptions.RemoveEmptyEntries))
                currentBooksInOriginalRow.Remove(bookToSplitTitle)
                newReturnedBookList = String.Join(" | ", currentBooksInOriginalRow)
            End If

            If newBookTotal > 0 Then

                Dim update_com As String = "UPDATE `returning_tbl` SET `ReturnedBook` = @newReturnedBook, `BookTotal` = @newBookTotal, `BorrowerStatus` = @borrowerStatusUpdate " &
                                         "WHERE `ID` = @originalID"

                Using update_cmd As New MySqlCommand(update_com, con, trans)
                    update_cmd.Parameters.AddWithValue("@newReturnedBook", newReturnedBookList)
                    update_cmd.Parameters.AddWithValue("@newBookTotal", newBookTotal)
                    update_cmd.Parameters.AddWithValue("@originalID", originalID)
                    update_cmd.Parameters.AddWithValue("@borrowerStatusUpdate", borrowerStatus)
                    update_cmd.ExecuteNonQuery()
                End Using
            Else

                Dim delete_original_com As String = "DELETE FROM `returning_tbl` WHERE `ID` = @originalID"
                Using delete_original_cmd As New MySqlCommand(delete_original_com, con, trans)
                    delete_original_cmd.Parameters.AddWithValue("@originalID", originalID)
                    delete_original_cmd.ExecuteNonQuery()
                End Using
            End If


            Dim delete_penalty_com As String = "DELETE FROM `penalty_tbl` WHERE `TransactionReceipt` = @transNo"
            Using delete_penalty_cmd As New MySqlCommand(delete_penalty_com, con, trans)
                delete_penalty_cmd.Parameters.AddWithValue("@transNo", transacReceipt)
                delete_penalty_cmd.ExecuteNonQuery()
            End Using


            Dim returningData As New DataTable()

            Dim select_returning_com As String = "SELECT * FROM `returning_tbl` WHERE `TransactionReceipt` = @transNo AND (`Status` <> 'Normal')"

            Using select_returning_cmd As New MySqlCommand(select_returning_com, con, trans)
                select_returning_cmd.Parameters.AddWithValue("@transNo", transacReceipt)

                Using adapter As New MySqlDataAdapter(select_returning_cmd)
                    adapter.Fill(returningData)
                End Using
            End Using

            For Each row As DataRow In returningData.Rows
                Dim currentReturnedBookStatus As String = row("Status").ToString()

                If currentReturnedBookStatus.StartsWith("Overdue") OrElse currentReturnedBookStatus.Contains("Damaged") OrElse currentReturnedBookStatus.StartsWith("Lost") Then

                    InsertPenaltyRecord(row, con, trans)
                End If
            Next

            trans.Commit()

            For Each form In Application.OpenForms

                If TypeOf form Is Acquisition Then
                    DirectCast(form, Acquisition).refreshData()
                End If
            Next


            GlobalVarsModule.LogAudit(
actionType:="UPDATE",
formName:="RETURN FORM",
description:=$"Edited status of book '{bookToSplitTitle}' in transaction {transacReceipt}.",
recordID:=transacReceipt,
oldValue:=$"Book: {bookToSplitTitle}, Old Status: {currentStatus}, Old Accession: {oldAccessionStatus}",
newValue:=$"New Status: {bookStatus}, New Accession: {newAccessionStatus}"
)
            For Each form In Application.OpenForms
                If TypeOf form Is AuditTrail Then
                    DirectCast(form, AuditTrail).refreshaudit()
                End If
            Next


            For Each form In Application.OpenForms

                If TypeOf form Is Accession Then
                    DirectCast(form, Accession).RefreshAccessionData()
                End If


                If TypeOf form Is MainForm Then
                    DirectCast(form, MainForm).lbltotalbookscount()
                End If
            Next

            MessageBox.Show($"Book status for '{bookToSplitTitle}' updated to '{bookStatus}' successfully!{(If(bookStatus = "Lost", Environment.NewLine & "", If(quantityUpdated, Environment.NewLine & "", "")))}", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)

            clear_click(sender, e)
            RefreshReturningData()


            For Each form In Application.OpenForms
                If TypeOf form Is Penalty Then
                    DirectCast(form, Penalty).refreshpenalty()
                End If
            Next

        Catch ex As Exception
            trans?.Rollback()
            MessageBox.Show("Error during book status edit: " & ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
        End Try
    End Sub

    Private Sub DataGridView1_RowPrePaint(sender As Object, e As DataGridViewRowPrePaintEventArgs) Handles DataGridView1.RowPrePaint
        If e.RowIndex >= 0 AndAlso e.RowIndex < DataGridView1.Rows.Count Then
            Dim row As DataGridViewRow = DataGridView1.Rows(e.RowIndex)
            Dim statusValue As String = row.Cells("BorrowerStatus").Value?.ToString().Trim().ToUpper()

            If statusValue = "PENALIZED" Then
                row.DefaultCellStyle.BackColor = Color.LightGreen
            ElseIf statusValue = "NOT PENALIZED" Then
                row.DefaultCellStyle.BackColor = Color.LightCoral

            ElseIf statusValue = "N/A" Then
                row.DefaultCellStyle.BackColor = Color.SteelBlue
            Else
                row.DefaultCellStyle.BackColor = Color.SteelBlue
            End If
        End If
    End Sub

    Private Sub InsertPenaltyRecord(ByVal row As DataRow, ByVal con As MySqlConnection, ByVal trans As MySqlTransaction)
        Dim insert_penalty_com As String = "INSERT INTO `penalty_tbl` (`Borrower`, `LRN`, `EmployeeNo`, `FullName`, `Department`, `Grade`, `Section`, `Strand`, `ReturnedBook`, `BookTotal`, `BorrowedDate`, `DueDate`, `ReturnDate`, `TransactionReceipt`, `Status`, `BorrowerStatus`) " &
                                        "VALUES (@borrowerType, @lrn, @empNo, @fullName, @dept, @grade, @section, @strand, @returnedBook, @bookTotal, @borrowDate, @dueDate, @returnDate, @transNo, @bookStatus, @borrowerStatus)" ' ✨ IDINAGDAG ANG `BorrowerStatus`

        Using insert_penalty_cmd As New MySqlCommand(insert_penalty_com, con, trans)
            insert_penalty_cmd.Parameters.AddWithValue("@borrowerType", row("Borrower"))
            insert_penalty_cmd.Parameters.AddWithValue("@lrn", row("LRN"))
            insert_penalty_cmd.Parameters.AddWithValue("@empNo", row("EmployeeNo"))
            insert_penalty_cmd.Parameters.AddWithValue("@fullName", row("FullName"))
            insert_penalty_cmd.Parameters.AddWithValue("@dept", row("Department"))
            insert_penalty_cmd.Parameters.AddWithValue("@grade", row("Grade"))
            insert_penalty_cmd.Parameters.AddWithValue("@section", row("Section"))
            insert_penalty_cmd.Parameters.AddWithValue("@strand", row("Strand"))


            insert_penalty_cmd.Parameters.AddWithValue("@returnedBook", row("ReturnedBook"))
            insert_penalty_cmd.Parameters.AddWithValue("@bookTotal", row("BookTotal"))

            insert_penalty_cmd.Parameters.AddWithValue("@borrowDate", row("BorrowedDate"))
            insert_penalty_cmd.Parameters.AddWithValue("@dueDate", row("DueDate"))
            insert_penalty_cmd.Parameters.AddWithValue("@returnDate", row("ReturnDate"))
            insert_penalty_cmd.Parameters.AddWithValue("@transNo", row("TransactionReceipt"))
            insert_penalty_cmd.Parameters.AddWithValue("@bookStatus", row("Status"))

            insert_penalty_cmd.Parameters.AddWithValue("@borrowerStatus", row("BorrowerStatus"))

            insert_penalty_cmd.ExecuteNonQuery()
        End Using
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

    Private scannedBarcodes As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)

    Private Sub txttransactionreceipt_TextChanged(sender As Object, e As EventArgs) Handles txttransactionreceipt.TextChanged
        If IsLoadingTransaction Then Return
        Const MIN_LENGTH As Integer = 12

        clear_details_only()
        cbbooks.Items.Clear()

        Dim TransactionNo As String = txttransactionreceipt.Text.Trim()

        If String.IsNullOrWhiteSpace(TransactionNo) OrElse TransactionNo.Length < MIN_LENGTH Then
            Return
        End If


        If TransactionNo.Length > MIN_LENGTH Then

            TransactionNo = TransactionNo.Substring(TransactionNo.Length - MIN_LENGTH)

        End If

        Dim con As New MySqlConnection(GlobalVarsModule.connectionString)

        Try
            con.Open()

            Dim com_check_return As String = "SELECT COUNT(*) FROM `borrowing_tbl` WHERE `TransactionReceipt` = @transNo"
            Using comsi_check As New MySqlCommand(com_check_return, con)
                comsi_check.Parameters.AddWithValue("@transNo", TransactionNo)
                Dim active_borrows As Integer = CInt(comsi_check.ExecuteScalar())


                If active_borrows = 0 Then

                    Dim com_check_history As String = "SELECT COUNT(*) FROM `returning_tbl` WHERE `TransactionReceipt` = @transNo"
                    Using comsi_history As New MySqlCommand(com_check_history, con)
                        comsi_history.Parameters.AddWithValue("@transNo", TransactionNo)
                        Dim history_count As Integer = CInt(comsi_history.ExecuteScalar())

                        If history_count > 0 Then

                            MessageBox.Show("This transaction is complete. All borrowed books have been returned.", "Transaction Complete", MessageBoxButtons.OK, MessageBoxIcon.Information)
                            txttransactionreceipt.Clear()
                            Return
                        Else

                            MessageBox.Show("Transaction record not found.", "Transaction Info", MessageBoxButtons.OK, MessageBoxIcon.Information)
                            txttransactionreceipt.Clear()
                            Return
                        End If
                    End Using
                End If
            End Using



            scannedBarcodes.Clear()


            Dim borrowerIDValue As String = ""
            Dim borrowerIDColumn As String = ""
            Dim borrowerType As String = ""
            Dim borrowerName As String = ""
            Dim borrowedDateStr As String = ""
            Dim dueDateStr As String = ""
            Dim totalBooksCount As String = ""

            Dim com_id As String = "SELECT `LRN`, `EmployeeNo`, `Name`, `Borrower` FROM `borrowing_tbl` WHERE `TransactionReceipt` = @transNo LIMIT 1"
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
                    End If
                End Using
            End Using

            Dim com_header As String = "SELECT `Borrower`, `Name`, `BorrowedDate`, `DueDate`, `BorrowedBookCount` FROM `printreceipt_tbl` WHERE `TransactionReceipt` = @transNo LIMIT 1"
            Using comsi_header As New MySqlCommand(com_header, con)
                comsi_header.Parameters.AddWithValue("@transNo", TransactionNo)
                Using reader_header As MySqlDataReader = comsi_header.ExecuteReader()
                    If reader_header.Read() Then
                        borrowerName = reader_header("Name").ToString()
                        borrowerType = reader_header("Borrower").ToString()
                        borrowedDateStr = Convert.ToDateTime(reader_header("BorrowedDate")).ToShortDateString()
                        dueDateStr = Convert.ToDateTime(reader_header("DueDate")).ToShortDateString()
                        totalBooksCount = reader_header("BorrowedBookCount").ToString()
                        reader_header.Close()
                    Else
                        reader_header.Close()

                    End If
                End Using
            End Using

            lblfullname.Text = borrowerName
            lblborrowertype.Text = borrowerType
            lblborroweddate.Text = borrowedDateStr
            lblduedate.Text = dueDateStr
            lblbooktotal.Text = totalBooksCount & " (Total)"

            If String.IsNullOrWhiteSpace(borrowerIDValue) Then
                Dim com_history_id As String = "SELECT `LRN`, `EmployeeNo`, `Borrower` FROM `borrowinghistory_tbl` WHERE `TransactionReceipt` = @transNo LIMIT 1"
                Using comsi_history_id As New MySqlCommand(com_history_id, con)
                    comsi_history_id.Parameters.AddWithValue("@transNo", TransactionNo)
                    Using reader_history_id As MySqlDataReader = comsi_history_id.ExecuteReader()
                        If reader_history_id.Read() Then
                            borrowerType = reader_history_id("Borrower").ToString()
                            If borrowerType.ToLower() = "student" Then
                                borrowerIDValue = reader_history_id("LRN").ToString()
                                borrowerIDColumn = "LRN"
                            Else
                                borrowerIDValue = reader_history_id("EmployeeNo").ToString()
                                borrowerIDColumn = "EmployeeNo"
                            End If
                        End If
                    End Using
                End Using
            End If

            If Not String.IsNullOrWhiteSpace(borrowerIDValue) Then
                LoadBorrowerDetails(borrowerIDValue, borrowerIDColumn)
            End If

            Dim com_items As String = "SELECT `BookTitle` FROM `borrowing_tbl` WHERE `TransactionReceipt` = @transNo"
            Using comsi_items As New MySqlCommand(com_items, con)
                comsi_items.Parameters.AddWithValue("@transNo", TransactionNo)
                Using reader_items As MySqlDataReader = comsi_items.ExecuteReader()
                    While reader_items.Read()
                        cbbooks.Items.Add(reader_items("BookTitle").ToString())
                    End While
                    If cbbooks.Items.Count = 0 Then
                        reader_items.Close()
                        If Not IsLoadingTransaction Then

                        End If
                        txttransactionreceipt.Clear()
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

                Dim filter As String = String.Format("Borrower LIKE '%{0}%' OR ReturnedBook LIKE '%{0}%'", txtsearch.Text.Trim())


                dt.DefaultView.RowFilter = filter
            Else
                dt.DefaultView.RowFilter = ""
            End If
        End If

    End Sub

    Private Function GetPenaltyStatus(ByVal transactionNo As String) As String
        Dim status As String = "NOT PENALIZED"
        Using con As New MySqlConnection(connectionString)
            Try
                con.Open()

                Dim penaltyQuery As String = "SELECT BorrowerStatus FROM penalty_tbl WHERE TransactionReceipt = @transNo LIMIT 1"
                Using cmdPenalty As New MySqlCommand(penaltyQuery, con)
                    cmdPenalty.Parameters.AddWithValue("@transNo", transactionNo)
                    Dim result As Object = cmdPenalty.ExecuteScalar()

                    If result IsNot Nothing AndAlso result IsNot DBNull.Value Then
                        Dim penaltyStatus As String = result.ToString().Trim().ToUpper()
                        If penaltyStatus = "PENALIZED" Then

                            Dim updateReturnQuery As String = "UPDATE returning_tbl SET BorrowerStatus = 'PENALIZED' WHERE TransactionReceipt = @transNo"
                            Using updateCmd As New MySqlCommand(updateReturnQuery, con)
                                updateCmd.Parameters.AddWithValue("@transNo", transactionNo)
                                updateCmd.ExecuteNonQuery()
                            End Using
                            Return "PENALIZED"
                        End If
                    End If
                End Using

                Dim returningQuery As String = "
                SELECT COUNT(*) 
                FROM returning_tbl 
                WHERE TransactionReceipt = @transNo 
                AND (Status LIKE 'Overdue%' OR Status LIKE 'Lost%' OR Status LIKE 'Damaged%')"
                Using cmdReturning As New MySqlCommand(returningQuery, con)
                    cmdReturning.Parameters.AddWithValue("@transNo", transactionNo)
                    Dim count As Integer = Convert.ToInt32(cmdReturning.ExecuteScalar())


                    If count > 0 Then
                        Dim updateReturnQuery As String = "UPDATE returning_tbl SET BorrowerStatus = 'NOT PENALIZED' WHERE TransactionReceipt = @transNo"
                        Using updateCmd As New MySqlCommand(updateReturnQuery, con)
                            updateCmd.Parameters.AddWithValue("@transNo", transactionNo)
                            updateCmd.ExecuteNonQuery()
                        End Using
                        Return "NOT PENALIZED"
                    End If
                End Using

            Catch ex As Exception
                MessageBox.Show("Error checking penalty status: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End Using

        Return status
    End Function



End Class