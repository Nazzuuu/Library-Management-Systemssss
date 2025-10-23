Imports MySql.Data.MySqlClient
Imports System.Data

Public Class Acquisition

    Private isShowingWarning As Boolean = False
    Private isEditing As Boolean = False

    Private Sub Acquisition_Load_1(sender As Object, e As EventArgs) Handles MyBase.Load

        Dim con As New MySqlConnection(GlobalVarsModule.connectionString)
        Dim com As String = "SELECT * FROM `acquisition_tbl`"
        Dim adp As New MySqlDataAdapter(com, con)
        Dim dt As New DataSet

        adp.Fill(dt, "INFO")
        DataGridView1.DataSource = dt.Tables("INFO")

        DataGridView1.Columns("ID").Visible = False
        DataGridView1.EnableHeadersVisualStyles = False
        DataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(207, 58, 109)
        DataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White

        jineret()
        cbsupplierr()
        clear()
        NumericUpDown2.Focus()

    End Sub

    Public Sub refreshData()
        Dim con As New MySqlConnection(connectionString)
        Dim com As String = "SELECT * FROM `acquisition_tbl`"
        Dim adp As New MySqlDataAdapter(com, con)
        Dim dt As New DataSet
        Try
            adp.Fill(dt, "INFO")
            DataGridView1.DataSource = dt.Tables("INFO")

            DataGridView1.ClearSelection()
        Catch ex As Exception
            MessageBox.Show("Error refreshing data: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
        cbsupplierr()
    End Sub

    Public Sub cbsupplierr()

        Dim con As New MySqlConnection(connectionString)
        Dim com As String = "SELECT ID, SupplierName FROM `supplier_tbl`"
        Dim adap As New MySqlDataAdapter(com, con)
        Dim dt As New DataTable

        adap.Fill(dt)

        cbsuppliername.DataSource = dt
        cbsuppliername.DisplayMember = "SupplierName"
        cbsuppliername.ValueMember = "ID"
        cbsuppliername.SelectedIndex = -1
    End Sub


    Private Sub Acquisition_Shown(sender As Object, e As EventArgs) Handles MyBase.Shown

        DataGridView1.ClearSelection()

    End Sub

    Private Sub txtsearch_TextChanged(sender As Object, e As EventArgs) Handles txtsearch.TextChanged

        Dim dt As DataTable = DirectCast(DataGridView1.DataSource, DataTable)
        If dt IsNot Nothing Then
            If txtsearch.Text.Trim() <> "" Then
                Dim filter As String = String.Format("BookTitle LIKE '*{0}*' OR ISBN LIKE '*{0}*'", txtsearch.Text.Trim())
                dt.DefaultView.RowFilter = filter
            Else
                dt.DefaultView.RowFilter = ""
            End If
        End If
    End Sub

    Public Sub jineret()

        Dim con As New MySqlConnection(connectionString)
        Dim com As String
        Dim lastTransactionNo As String = ""
        Dim newTransactionNo As Integer = 0

        Try
            con.Open()
            com = "SELECT TransactionNo FROM acquisition_tbl ORDER BY LENGTH(TransactionNo) DESC, TransactionNo DESC LIMIT 1"

            Using comsi As New MySqlCommand(com, con)
                Dim result As Object = comsi.ExecuteScalar()
                If result IsNot Nothing Then
                    lastTransactionNo = result.ToString()
                End If
            End Using
        Catch ex As Exception
            MessageBox.Show("Error: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        Finally
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
        End Try

        If String.IsNullOrEmpty(lastTransactionNo) Then
            newTransactionNo = 1
        Else
            Dim number As String = lastTransactionNo.Substring(lastTransactionNo.IndexOf("-") + 1)
            If Integer.TryParse(number, newTransactionNo) Then
                newTransactionNo += 1
            Else
                newTransactionNo = 1
            End If
        End If

        txttransactionno.Text = "T-" & newTransactionNo.ToString("D5")
    End Sub


    Private Sub ClearBookEntryFields()
        txtisbn.Clear()
        txtbarcodes.Clear()
        txtbooktitle.Clear()
        NumericUpDown1.Value = 0
        txtbookprice.Clear()
        txttotalcost.Clear()

    End Sub


    Private Sub CompleteAndStartNewTransaction()

        NumericUpDown2.Value = 0
        txtbookprice.Text = ""
        txtisbn.Text = ""
        txtbarcodes.Text = ""
        txtbooktitle.Text = ""
        txttotalcost.Text = "0"
        cbsuppliername.SelectedIndex = -1
        NumericUpDown1.Value = 0

        txtisbn.Enabled = False
        txtbarcodes.Enabled = False
        txtbooktitle.Enabled = False
        txttotalcost.Enabled = False
        txttransactionno.Enabled = False

        rbbarcode.Checked = False
        rbisbn.Checked = False

        rbbarcode.Enabled = True
        rbisbn.Enabled = True

        DateTimePicker1.Value = DateTime.Now
        DateTimePicker1.Enabled = True
        DataGridView1.ClearSelection()

        txttransactionno.ReadOnly = False
        NumericUpDown2.Enabled = True

        jineret()
        ClearBookEntryFields()

        NumericUpDown2.Focus()

    End Sub



    Private Sub btnadd_Click(sender As Object, e As EventArgs) Handles btnadd.Click


        If Not IsTransactionQuantityValid() Then
            Return
        End If


        Dim con As New MySqlConnection(connectionString)


        If String.IsNullOrWhiteSpace(txtbooktitle.Text) OrElse String.IsNullOrWhiteSpace(txtbookprice.Text) Then
            MessageBox.Show("Please fill the required fields (Book Title, Book Price).", "Required", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        If NumericUpDown1.Value <= 0 Then
            MessageBox.Show("Quantity of this book cannot be 0 or less.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Dim bookPrice As Decimal
        If Not Decimal.TryParse(txtbookprice.Text, bookPrice) OrElse bookPrice <= 0 Then
            MessageBox.Show("Book Price cannot be 0 or less, or contain non-numeric characters.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Dim deyts As DateTime = DateTimePicker1.Value
        If deyts.Date > DateTime.Today.Date Then
            MsgBox("You cannot select a future date.", vbExclamation)
            Exit Sub
        End If

        Dim purmatdeyt As String = deyts.ToString("yyyy-MM-dd")


        Dim nudTransactionQty As Decimal = NumericUpDown2.Value

        If nudTransactionQty <= 0 Then
            MessageBox.Show("The Transaction Quantity for this transaction is already complete. Please set a new quantity or clear the form.", "Transaction Complete", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If


        Try
            con.Open()


            Dim isbnorbarcode As String = If(rbisbn.Checked, txtisbn.Text, txtbarcodes.Text)
            Dim fieldToCheck As String = If(rbisbn.Checked, "ISBN", "Barcode")

            If Not String.IsNullOrWhiteSpace(isbnorbarcode) Then
                Dim checkCurrentTrans As String = $"SELECT COUNT(*) FROM acquisition_tbl WHERE TransactionNo = @TransactionNo AND {fieldToCheck} = @ValueToCheck"
                Dim checkCmd As New MySqlCommand(checkCurrentTrans, con)
                checkCmd.Parameters.AddWithValue("@TransactionNo", txttransactionno.Text)
                checkCmd.Parameters.AddWithValue("@ValueToCheck", isbnorbarcode)

                If CInt(checkCmd.ExecuteScalar()) > 0 Then
                    MessageBox.Show($"This {fieldToCheck} has already been added in the current transaction ({txttransactionno.Text}).", "Duplication Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Return
                End If
            End If



            Dim com As String = "INSERT INTO acquisition_tbl (`ISBN`, `Barcode`, `BookTitle`, `SupplierName`, `Quantity`, `BookPrice`, `TotalCost`, `TransactionNo`, `DateAcquired`) " &
                            "VALUES (@ISBN, @Barcode, @BookTitle, @SupplierName, @Quantity, @BookPrice, @TotalCost, @TransactionNo, @DateAcquired)"

            Using comsu As New MySqlCommand(com, con)
                comsu.Parameters.AddWithValue("@ISBN", txtisbn.Text)
                comsu.Parameters.AddWithValue("@Barcode", txtbarcodes.Text)
                comsu.Parameters.AddWithValue("@BookTitle", txtbooktitle.Text)
                comsu.Parameters.AddWithValue("@SupplierName", cbsuppliername.Text)
                comsu.Parameters.AddWithValue("@Quantity", NumericUpDown1.Value)
                comsu.Parameters.AddWithValue("@BookPrice", CDbl(txtbookprice.Text))
                comsu.Parameters.AddWithValue("@TotalCost", CDbl(txttotalcost.Text))
                comsu.Parameters.AddWithValue("@TransactionNo", txttransactionno.Text)
                comsu.Parameters.AddWithValue("@DateAcquired", purmatdeyt)

                comsu.ExecuteNonQuery()

                GlobalVarsModule.LogAudit(
                actionType:="ADD",
                formName:="ACQUISITION FORM",
                description:=$"Added book entry to transaction {txttransactionno.Text}. Title: {txtbooktitle.Text}, Qty: {NumericUpDown1.Value}.",
                recordID:=txttransactionno.Text,
                oldValue:="N/A",
                newValue:=$"Title: {txtbooktitle.Text}, Qty: {NumericUpDown1.Value}, Price: {txtbookprice.Text}"
            )

                For Each form In Application.OpenForms
                    If TypeOf form Is AuditTrail Then
                        DirectCast(form, AuditTrail).refreshaudit()
                    End If
                Next

                MessageBox.Show("Successfully added book entry to transaction!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)

                NumericUpDown2.Enabled = False

                NumericUpDown2.Value = NumericUpDown2.Value - 1


                If NumericUpDown2.Value = 0 Then
                    MessageBox.Show($"Transaction {txttransactionno.Text} is now complete!.", "Transaction Complete", MessageBoxButtons.OK, MessageBoxIcon.Information)


                    CompleteAndStartNewTransaction()
                Else

                    ClearBookEntryFields()
                End If
                refreshData()
            End Using

        Catch ex As Exception
            MessageBox.Show("Error: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
        End Try

    End Sub

    Private Sub btnedit_Click(sender As Object, e As EventArgs) Handles btnedit.Click

        isEditing = False

        If DataGridView1.SelectedRows.Count = 0 Then
            MessageBox.Show("Please select a record to edit.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If

        Dim selectedRow As DataGridViewRow = DataGridView1.SelectedRows(0)

        Dim oldValues As New Dictionary(Of String, Object)
        oldValues.Add("ISBN", selectedRow.Cells("ISBN").Value)
        oldValues.Add("Barcode", selectedRow.Cells("Barcode").Value)
        oldValues.Add("BookTitle", selectedRow.Cells("BookTitle").Value)
        oldValues.Add("SupplierName", selectedRow.Cells("SupplierName").Value)
        oldValues.Add("Quantity", selectedRow.Cells("Quantity").Value)
        oldValues.Add("BookPrice", selectedRow.Cells("BookPrice").Value)
        oldValues.Add("TotalCost", selectedRow.Cells("TotalCost").Value)
        oldValues.Add("DateAcquired", selectedRow.Cells("DateAcquired").Value)


        Dim ID As Integer = CInt(selectedRow.Cells("ID").Value)
        Dim oldQuantity As Integer = CInt(selectedRow.Cells("Quantity").Value)
        Dim newQuantity As Integer = CInt(NumericUpDown1.Value)

        Dim transactionNo As String = selectedRow.Cells("TransactionNo").Value.ToString()
        Dim bookTitle As String = selectedRow.Cells("BookTitle").Value.ToString().Trim()

        Dim originalTotalCost As Double = CDbl(selectedRow.Cells("TotalCost").Value)
        Dim currentBookPrice As Double = 0.0

        If String.IsNullOrWhiteSpace(txtbookprice.Text) Then
            MessageBox.Show("Book Price cannot be empty.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End If

        If Not Double.TryParse(txtbookprice.Text, currentBookPrice) Then
            MessageBox.Show("Invalid format for Book Price. Please enter a numerical value.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End If

        If currentBookPrice < 0 Then
            MessageBox.Show("Book Price cannot be negative.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End If

        Dim newTotalCost As Double = newQuantity * currentBookPrice

        Dim con As New MySqlConnection(connectionString)

        Dim deyts As DateTime = DateTimePicker1.Value
        If deyts.Date > DateTime.Today.Date Then
            MsgBox("You cannot select a future date.", vbExclamation)
            Return
        End If
        Dim purmatdeyt As String = deyts.ToString("yyyy-MM-dd")

        Try
            con.Open()

            If newQuantity < oldQuantity Then

                Dim accessionCountSql As String = "SELECT COUNT(*) FROM `acession_tbl` WHERE TransactionNo = @TransactionNo AND BookTitle = @BookTitle"
                Dim accessionCmd As New MySqlCommand(accessionCountSql, con)
                accessionCmd.Parameters.AddWithValue("@TransactionNo", transactionNo)
                accessionCmd.Parameters.AddWithValue("@BookTitle", bookTitle)
                Dim totalAcessionCopies As Integer = CInt(accessionCmd.ExecuteScalar())

                Dim reserveCountSql As String = "SELECT COUNT(*) FROM `reservecopiess_tbl` WHERE TransactionNo = @TransactionNo AND BookTitle = @BookTitle"
                Dim reserveCmd As New MySqlCommand(reserveCountSql, con)
                reserveCmd.Parameters.AddWithValue("@TransactionNo", transactionNo)
                reserveCmd.Parameters.AddWithValue("@BookTitle", bookTitle)
                Dim reserveCount As Integer = CInt(reserveCmd.ExecuteScalar())

                Dim totalExistingCopies As Integer = totalAcessionCopies + reserveCount

                Dim nonAvailableCountSql As String = "SELECT COUNT(*) FROM `acession_tbl` WHERE TransactionNo = @TransactionNo AND BookTitle = @BookTitle AND Status <> 'Available'"
                Dim nonAvailableCmd As New MySqlCommand(nonAvailableCountSql, con)
                nonAvailableCmd.Parameters.AddWithValue("@TransactionNo", transactionNo)
                nonAvailableCmd.Parameters.AddWithValue("@BookTitle", bookTitle)
                Dim totalNonAvailableCopies As Integer = CInt(nonAvailableCmd.ExecuteScalar())


                If newQuantity = 0 AndAlso totalExistingCopies > 0 Then

                    Dim warningMessage As String = "WARNING: Cannot reduce quantity to " & newQuantity & ". " & Environment.NewLine & Environment.NewLine &
                                              "There are currently " & totalExistingCopies & " existing copies of '" & bookTitle & "' for Transaction No. " & transactionNo & ":" & Environment.NewLine &
                                              "- " & totalAcessionCopies & " copies in the Acession Table." & Environment.NewLine &
                                              "- " & reserveCount & " copies in the Reserve Copies Table."

                    MessageBox.Show(warningMessage, "Cannot Update Quantity", MessageBoxButtons.OK, MessageBoxIcon.Warning)

                    NumericUpDown1.Value = oldQuantity
                    txttotalcost.Text = originalTotalCost.ToString("F2")
                    Return
                End If

                If newQuantity < totalNonAvailableCopies Then
                    Dim warningMessage As String = "CRITICAL WARNING: The new quantity (" & newQuantity & ") is less than the total number of copies currently in use or non-Available (" & totalNonAvailableCopies & "). Please check Borrowed/Reserved/Damaged records."

                    MessageBox.Show(warningMessage, "Safety Error", MessageBoxButtons.OK, MessageBoxIcon.Error)

                    NumericUpDown1.Value = oldQuantity
                    txttotalcost.Text = originalTotalCost.ToString("F2")
                    Return
                End If

                Dim recordsToDelete As Integer = oldQuantity - newQuantity
                Dim deleteAcs As String = "DELETE FROM `acession_tbl` WHERE TransactionNo = @TransactionNo AND BookTitle = @BookTitle AND Status = 'Available' ORDER BY AccessionID DESC LIMIT @limit"
                Dim deleyt As New MySqlCommand(deleteAcs, con)
                deleyt.Parameters.AddWithValue("@TransactionNo", transactionNo)
                deleyt.Parameters.AddWithValue("@BookTitle", bookTitle)
                deleyt.Parameters.AddWithValue("@limit", recordsToDelete)
                deleyt.ExecuteNonQuery()

            ElseIf newQuantity > oldQuantity Then

                Dim recordsToAdd As Integer = newQuantity - oldQuantity

                Dim rand As New Random()
                Dim newAccessionID As String = ""
                Dim isUnique As Boolean = False
                Dim shelf As String = ""

                Dim getShelfQuery As String = "SELECT Shelf FROM `acession_tbl` WHERE TransactionNo = @TransactionNo AND BookTitle = @BookTitle LIMIT 1"
                Dim getShelfCommand As New MySqlCommand(getShelfQuery, con)
                getShelfCommand.Parameters.AddWithValue("@TransactionNo", transactionNo)
                getShelfCommand.Parameters.AddWithValue("@BookTitle", bookTitle)
                Dim shelfResult As Object = getShelfCommand.ExecuteScalar()
                If shelfResult IsNot Nothing AndAlso shelfResult IsNot DBNull.Value Then
                    shelf = shelfResult.ToString()
                End If

                If String.IsNullOrWhiteSpace(shelf) Then
                End If

                For i As Integer = 1 To recordsToAdd
                    isUnique = False
                    While Not isUnique
                        newAccessionID = rand.Next(10000, 99999).ToString()

                        Dim checksu As String = "SELECT COUNT(*) FROM `acession_tbl` WHERE AccessionID = @AccessionID"
                        Dim chiks As New MySqlCommand(checksu, con)
                        chiks.Parameters.AddWithValue("@AccessionID", newAccessionID)
                        Dim existingIDCount As Integer = CInt(chiks.ExecuteScalar())

                        If existingIDCount = 0 Then
                            isUnique = True
                        End If
                    End While

                    Dim insertAcs As String = "INSERT INTO acession_tbl (`TransactionNo`, `AccessionID`, `ISBN`, `Barcode`, `BookTitle`, `Shelf`, `SupplierName`, `Status`) " &
                                         "VALUES (@TransactionNo, @AccessionID, @ISBN, @Barcode, @BookTitle, @Shelf, @SupplierName, @Status)"

                    Using insertAcession As New MySqlCommand(insertAcs, con)
                        insertAcession.Parameters.AddWithValue("@TransactionNo", transactionNo)
                        insertAcession.Parameters.AddWithValue("@AccessionID", newAccessionID)
                        insertAcession.Parameters.AddWithValue("@ISBN", If(String.IsNullOrWhiteSpace(txtisbn.Text), CType(DBNull.Value, Object), txtisbn.Text))
                        insertAcession.Parameters.AddWithValue("@Barcode", If(String.IsNullOrWhiteSpace(txtbarcodes.Text), CType(DBNull.Value, Object), txtbarcodes.Text))
                        insertAcession.Parameters.AddWithValue("@BookTitle", bookTitle)
                        insertAcession.Parameters.AddWithValue("@Shelf", shelf)
                        insertAcession.Parameters.AddWithValue("@SupplierName", cbsuppliername.Text)
                        insertAcession.Parameters.AddWithValue("@Status", "Available")
                        insertAcession.ExecuteNonQuery()
                    End Using

                    Dim insertAvailSql As String = "INSERT INTO available_tbl (AccessionID, ISBN, Barcode, BookTitle, Shelf, Status) " &
                                              "VALUES (@AccessionID_A, @ISBN_A, @Barcode_A, @BookTitle_A, @Shelf_A, @Status_A)"
                    Using insertAvailCmd As New MySqlCommand(insertAvailSql, con)
                        insertAvailCmd.Parameters.AddWithValue("@AccessionID_A", newAccessionID)
                        insertAvailCmd.Parameters.AddWithValue("@ISBN_A", If(String.IsNullOrWhiteSpace(txtisbn.Text), CType(DBNull.Value, Object), txtisbn.Text))
                        insertAvailCmd.Parameters.AddWithValue("@Barcode_A", If(String.IsNullOrWhiteSpace(txtbarcodes.Text), CType(DBNull.Value, Object), txtbarcodes.Text))
                        insertAvailCmd.Parameters.AddWithValue("@BookTitle_A", bookTitle)
                        insertAvailCmd.Parameters.AddWithValue("@Shelf_A", shelf)
                        insertAvailCmd.Parameters.AddWithValue("@Status_A", "Available")
                        insertAvailCmd.ExecuteNonQuery()
                    End Using
                Next

            End If

            txttotalcost.Text = newTotalCost.ToString("F2")

            Dim updates As String = "UPDATE `acquisition_tbl` SET " &
                                "`ISBN` = @ISBN, " &
                                "`Barcode` = @Barcode, " &
                                "`BookTitle` = @BookTitle, " &
                                "`SupplierName` = @SupplierName, " &
                                "`Quantity` = @Quantity, " &
                                "`BookPrice` = @BookPrice, " &
                                "`TotalCost` = @TotalCost, " &
                                "`DateAcquired` = @DateAcquired " &
                                "WHERE `ID` = @ID"

            Using Updateacqt As New MySqlCommand(updates, con)
                Updateacqt.Parameters.AddWithValue("@ISBN", txtisbn.Text)
                Updateacqt.Parameters.AddWithValue("@Barcode", txtbarcodes.Text)
                Updateacqt.Parameters.AddWithValue("@BookTitle", txtbooktitle.Text.Trim())
                Updateacqt.Parameters.AddWithValue("@SupplierName", cbsuppliername.Text)
                Updateacqt.Parameters.AddWithValue("@Quantity", newQuantity)
                Updateacqt.Parameters.AddWithValue("@BookPrice", currentBookPrice)
                Updateacqt.Parameters.AddWithValue("@TotalCost", newTotalCost)
                Updateacqt.Parameters.AddWithValue("@DateAcquired", purmatdeyt)
                Updateacqt.Parameters.AddWithValue("@ID", ID)
                Updateacqt.ExecuteNonQuery()
            End Using

            Dim oldValueStr As String = $"ISBN: {oldValues("ISBN")}, Barcode: {oldValues("Barcode")}, Title: {oldValues("BookTitle")}, Qty: {oldValues("Quantity")}, Price: {oldValues("BookPrice")}, Cost: {oldValues("TotalCost")}"
            Dim newValueStr As String = $"ISBN: {txtisbn.Text}, Barcode: {txtbarcodes.Text}, Title: {txtbooktitle.Text}, Qty: {newQuantity}, Price: {currentBookPrice}, Cost: {newTotalCost}"

            GlobalVarsModule.LogAudit(
            actionType:="UPDATE",
            formName:="ACQUISITION FORM",
            description:=$"Updated acquisition record for Book ID {ID} in transaction {transactionNo}. Title: {txtbooktitle.Text}",
            recordID:=transactionNo,
            oldValue:=oldValueStr,
            newValue:=newValueStr
        )
            For Each form In Application.OpenForms
                If TypeOf form Is AuditTrail Then
                    DirectCast(form, AuditTrail).refreshaudit()
                End If
            Next

            For Each form In Application.OpenForms
                If TypeOf form Is AvailableBooks Then
                    Dim avail = DirectCast(form, AvailableBooks)
                    avail.refreshavail()
                    avail.counts()
                End If
            Next

            MessageBox.Show("Record updated successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)

            Acquisition_Load_1(sender, e)
            clear()

            For Each form As Form In Application.OpenForms
                If TypeOf form Is Accession Then
                    Dim accessionForm As Accession = CType(form, Accession)
                    accessionForm.RefreshAccessionData()
                    Exit For
                End If
            Next

        Catch ex As Exception
            MessageBox.Show("Error updating record: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
        End Try

    End Sub

    Private Sub btndelete_Click(sender As Object, e As EventArgs) Handles btndelete.Click

        If DataGridView1.SelectedRows.Count = 0 Then
            MessageBox.Show("Please select a row to delete.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If

        Dim selectedRow As DataGridViewRow = DataGridView1.SelectedRows(0)
        Dim acquisitionID As Integer = CInt(selectedRow.Cells("ID").Value)
        Dim transactionNo As String = selectedRow.Cells("TransactionNo").Value.ToString()
        Dim bookTitle As String = selectedRow.Cells("BookTitle").Value.ToString()
        Dim bookISBN As String = selectedRow.Cells("ISBN").Value.ToString().Trim()
        Dim bookBarcode As String = selectedRow.Cells("Barcode").Value.ToString()
        Dim oldQuantity As Integer = CInt(selectedRow.Cells("Quantity").Value)

        Dim con As New MySqlConnection(connectionString)
        Dim hasAccessions As Boolean = False

        Try
            con.Open()


            Dim checkCmd As New MySqlCommand("SELECT COUNT(*) FROM `acession_tbl` WHERE `ISBN` = @ISBN", con)
            checkCmd.Parameters.AddWithValue("@ISBN", bookISBN)

            Dim count As Integer = CInt(checkCmd.ExecuteScalar())

            If count > 0 Then
                hasAccessions = True
            End If


            If hasAccessions Then

                MessageBox.Show($"Cannot delete this acquisition record. There are {count} corresponding book accession(s) in the Accession Form.", "Deletion Restricted", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If



            Dim acs As New MySqlCommand("SELECT COUNT(*) FROM `acession_tbl` WHERE `ISBN` = @ISBN OR `Barcode` = @Barcode", con)
            acs.Parameters.AddWithValue("@ISBN", bookISBN)
            acs.Parameters.AddWithValue("@Barcode", bookBarcode)

            Dim countss As Integer = CInt(acs.ExecuteScalar())

            If countss > 0 Then
                MsgBox("Cannot delete this book. It has existing Accession ID records.", vbExclamation, "Deletion Blocked")
                Exit Sub
            End If

            Dim dialogResult As DialogResult = MessageBox.Show("Are you sure you want to delete this acquisition record?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)

            If dialogResult = DialogResult.Yes Then
                Dim deleteCmd As New MySqlCommand("DELETE FROM `acquisition_tbl` WHERE `ID` = @id", con)
                deleteCmd.Parameters.AddWithValue("@id", acquisitionID)
                deleteCmd.ExecuteNonQuery()

                Dim resett As String = "ALTER TABLE `acession_tbl` AUTO_INCREMENT = 1"
                Dim incrementsu As New MySqlCommand(resett, con)
                incrementsu.ExecuteNonQuery()

                GlobalVarsModule.LogAudit(
                actionType:="DELETE",
                formName:="ACQUISITION FORM",
                description:=$"Deleted acquisition record from transaction {transactionNo}. Title: {bookTitle}, Qty: {oldQuantity}.",
                recordID:=transactionNo,
                oldValue:=$"Book ID: {acquisitionID}, Title: {bookTitle}, Qty: {oldQuantity}",
                newValue:="N/A (Deleted)"
            )

                For Each form In Application.OpenForms
                    If TypeOf form Is AuditTrail Then
                        DirectCast(form, AuditTrail).refreshaudit()
                    End If
                Next

                MessageBox.Show("Record deleted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)


                Acquisition_Load_1(sender, e)
            End If

        Catch ex As Exception
            MessageBox.Show("Error processing deletion: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
        End Try
    End Sub

    Private Sub btnclear_Click(sender As Object, e As EventArgs) Handles btnclear.Click

        isEditing = False

        clear()
        jineret()
        NumericUpDown2.Value = 0


        txttransactionno.ReadOnly = False
        NumericUpDown2.Enabled = True

        NumericUpDown2.Focus()

    End Sub

    Private Sub dgv_acquisition_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellClick

        If e.RowIndex >= 0 Then

            isEditing = True

            txtisbn.Enabled = False
            txtbarcodes.Enabled = False
            txtbooktitle.Enabled = False
            txttotalcost.Enabled = False
            txttransactionno.Enabled = False

            Dim selectedRow = DataGridView1.Rows(e.RowIndex)


            txtisbn.Text = selectedRow.Cells("ISBN").Value.ToString
            If selectedRow.Cells("Barcode").Value IsNot DBNull.Value AndAlso selectedRow.Cells("Barcode").Value IsNot Nothing Then
                txtbarcodes.Text = selectedRow.Cells("Barcode").Value.ToString
            Else
                txtbarcodes.Text = ""
            End If

            txtbooktitle.Text = selectedRow.Cells("BookTitle").Value.ToString
            cbsuppliername.Text = selectedRow.Cells("SupplierName").Value.ToString
            NumericUpDown1.Value = Convert.ToDecimal(selectedRow.Cells("Quantity").Value)
            txtbookprice.Text = selectedRow.Cells("BookPrice").Value.ToString
            txttotalcost.Text = selectedRow.Cells("TotalCost").Value.ToString
            DateTimePicker1.Value = CDate(selectedRow.Cells("DateAcquired").Value)

            If Not String.IsNullOrEmpty(selectedRow.Cells("Barcode").Value?.ToString()) Then

                rbbarcode.Checked = False
                rbbarcode.Enabled = False
                rbisbn.Enabled = False
                rbisbn.Checked = False

            ElseIf Not String.IsNullOrEmpty(selectedRow.Cells("ISBN").Value?.ToString()) Then

                rbbarcode.Checked = False
                rbbarcode.Enabled = False
                rbisbn.Enabled = False
                rbisbn.Checked = False

            Else

                rbbarcode.Checked = False
                rbbarcode.Enabled = False
                rbisbn.Enabled = False
                rbisbn.Checked = False

            End If

        End If


    End Sub

    Private Sub txtisbn_TextChanged(sender As Object, e As EventArgs) Handles txtisbn.TextChanged

        If String.IsNullOrWhiteSpace(txtisbn.Text) Then
            txtbooktitle.Text = ""
            Return
        End If
        Dim con As New MySqlConnection(connectionString)
        Try
            con.Open()
            Dim com As String = "SELECT `BookTitle` FROM `book_tbl` WHERE `ISBN` = @ISBN"
            Using comsi As New MySqlCommand(com, con)
                comsi.Parameters.AddWithValue("@ISBN", txtisbn.Text)
                Dim bookTitle As Object = comsi.ExecuteScalar()
                If bookTitle IsNot Nothing Then
                    txtbooktitle.Text = bookTitle.ToString()
                Else
                    txtbooktitle.Text = ""
                End If
            End Using
        Catch ex As Exception
            MessageBox.Show("Error: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
        End Try
    End Sub

    Private Sub txtbarcodes_TextChanged(sender As Object, e As EventArgs) Handles txtbarcodes.TextChanged

        If String.IsNullOrWhiteSpace(txtbarcodes.Text) Then
            txtbooktitle.Text = ""
            Return
        End If
        Dim con As New MySqlConnection(connectionString)
        Try
            con.Open()
            Dim com As String = "SELECT `BookTitle` FROM `book_tbl` WHERE `Barcode` = @barcode"
            Using comsi As New MySqlCommand(com, con)
                comsi.Parameters.AddWithValue("@barcode", txtbarcodes.Text)
                Dim bookTitle As Object = comsi.ExecuteScalar()
                If bookTitle IsNot Nothing Then
                    txtbooktitle.Text = bookTitle.ToString()
                Else
                    txtbooktitle.Text = ""
                End If
            End Using
        Catch ex As Exception
            MessageBox.Show("Error: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
        End Try
    End Sub

    Private Function IsTransactionQuantityValid() As Boolean

        If NumericUpDown2.Enabled = False Then
            Return True
        End If

        If NumericUpDown2.Value <= 0 Then
            If isShowingWarning Then
                Return False
            End If

            isShowingWarning = True

            MessageBox.Show("Please set the 'Expected Book Types for Transaction No' before entering book details.", "Transaction Quantity Required", MessageBoxButtons.OK, MessageBoxIcon.Warning)

            NumericUpDown2.Focus()

            isShowingWarning = False


            Application.DoEvents()

            Return False
        End If

        Return True
    End Function

    Private Sub InputControl_GotFocus(sender As Object, e As EventArgs) Handles txtisbn.GotFocus, txtbarcodes.GotFocus,
                                                                    txtbooktitle.GotFocus, txtbookprice.GotFocus,
                                                                    cbsuppliername.GotFocus, NumericUpDown1.GotFocus,
                                                                    rbisbn.GotFocus, rbbarcode.GotFocus

        If Not isEditing Then
            If Not IsTransactionQuantityValid() Then

            End If
        End If
    End Sub
    Public Sub clear()

        isEditing = False

        txtbookprice.Text = ""
        txtisbn.Text = ""
        txtbarcodes.Text = ""
        txtbooktitle.Text = ""
        txttotalcost.Text = "0"
        cbsuppliername.SelectedIndex = -1
        NumericUpDown1.Value = 0

        txtisbn.Enabled = False
        txtbarcodes.Enabled = False
        txtbooktitle.Enabled = False
        txttotalcost.Enabled = False
        txttransactionno.Enabled = False

        rbbarcode.Checked = False
        rbisbn.Checked = False

        rbbarcode.Enabled = True
        rbisbn.Enabled = True

        DateTimePicker1.Value = DateTime.Now
        DateTimePicker1.Enabled = True
        DataGridView1.ClearSelection()
    End Sub


    Public Sub kalkuleyt()

        Dim quantity As Decimal = 0
        Dim bookPrice As Decimal = 0
        Dim totalCost As Decimal = 0
        If Not Decimal.TryParse(NumericUpDown1.Text, quantity) Then
            txttotalcost.Text = "0"
            Return
        End If
        If Not Decimal.TryParse(txtbookprice.Text, bookPrice) Then
            txttotalcost.Text = "0"
            Return
        End If
        totalCost = quantity * bookPrice
        txttotalcost.Text = totalCost.ToString("N2")
    End Sub

    Private Sub txtbookprice_TextChanged(sender As Object, e As EventArgs) Handles txtbookprice.TextChanged
        kalkuleyt()
    End Sub

    Private Sub NumericUpDown1_ValueChanged(sender As Object, e As EventArgs) Handles NumericUpDown1.ValueChanged
        kalkuleyt()
    End Sub

    Private Sub rbisbn_CheckedChanged(sender As Object, e As EventArgs) Handles rbisbn.CheckedChanged

        If rbisbn.Checked Then

            txtisbn.Enabled = True
            txtbarcodes.Enabled = False
            txtbarcodes.Text = ""

        End If


    End Sub



    Private Sub rbbarcode_CheckedChanged(sender As Object, e As EventArgs) Handles rbbarcode.CheckedChanged

        If rbbarcode.Checked Then

            txtbarcodes.Enabled = True
            txtisbn.Enabled = False
            txtisbn.Text = ""

        End If

    End Sub

    Private Sub txtisbn_KeyPress(sender As Object, e As KeyPressEventArgs) Handles txtisbn.KeyPress

        If Not Char.IsDigit(e.KeyChar) And Not Char.IsControl(e.KeyChar) Then
            e.Handled = True
        End If

    End Sub

    Private Sub txtisbn_KeyDown(sender As Object, e As KeyEventArgs) Handles txtisbn.KeyDown

        If e.Control AndAlso (e.KeyCode = Keys.V Or e.KeyCode = Keys.C Or e.KeyCode = Keys.X) Then
            e.SuppressKeyPress = True
        End If

    End Sub

    Private Sub txtbarcodes_KeyDown(sender As Object, e As KeyEventArgs) Handles txtbarcodes.KeyDown


        If e.Control AndAlso (e.KeyCode = Keys.V Or e.KeyCode = Keys.C Or e.KeyCode = Keys.X) Then
            e.SuppressKeyPress = True
        End If

    End Sub

    Private Sub txtbarcodes_KeyPress(sender As Object, e As KeyPressEventArgs) Handles txtbarcodes.KeyPress

        If Not Char.IsDigit(e.KeyChar) And Not Char.IsControl(e.KeyChar) Then
            e.Handled = True
        End If

    End Sub

    Private Sub btnsupplieradd_Click(sender As Object, e As EventArgs) Handles btnsupplieradd.Click
        Supplier.ShowDialog()
    End Sub

    Private Sub btnsupplieradd_MouseHover(sender As Object, e As EventArgs) Handles btnsupplieradd.MouseHover
        Cursor = Cursors.Hand
    End Sub

    Private Sub btnsupplieradd_MouseLeave(sender As Object, e As EventArgs) Handles btnsupplieradd.MouseLeave
        Cursor = Cursors.Default
    End Sub

    Private Sub btnadd_MouseHover(sender As Object, e As EventArgs) Handles btnadd.MouseHover
        Cursor = Cursors.Hand
    End Sub

    Private Sub btnadd_MouseLeave(sender As Object, e As EventArgs) Handles btnadd.MouseLeave
        Cursor = Cursors.Default
    End Sub

    Private Sub btnedit_MouseHover(sender As Object, e As EventArgs) Handles btnedit.MouseHover
        Cursor = Cursors.Hand
    End Sub

    Private Sub btnedit_MouseLeave(sender As Object, e As EventArgs) Handles btnedit.MouseLeave
        Cursor = Cursors.Default
    End Sub

    Private Sub btndelete_MouseHover(sender As Object, e As EventArgs) Handles btndelete.MouseHover
        Cursor = Cursors.Hand
    End Sub

    Private Sub btndelete_MouseLeave(sender As Object, e As EventArgs) Handles btndelete.MouseLeave
        Cursor = Cursors.Default
    End Sub

    Private Sub btnclear_MouseHover(sender As Object, e As EventArgs) Handles btnclear.MouseHover
        Cursor = Cursors.Hand
    End Sub

    Private Sub btnclear_MouseLeave(sender As Object, e As EventArgs) Handles btnclear.MouseLeave
        Cursor = Cursors.Default
    End Sub
End Class