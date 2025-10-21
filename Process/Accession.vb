Imports MySql.Data.MySqlClient
Imports System.Data
Imports System.Drawing

Public Class Accession


    Private Const connectionString As String = "server=localhost;userid=root;database=laybsis_dbs;"



    Public Sub RefreshAccessionData(Optional ByVal filterStatus As String = "")

        Dim con As New MySqlConnection(connectionString)
        Dim com As String = "SELECT * FROM `acession_tbl`"

        If Not String.IsNullOrEmpty(filterStatus) Then
            com &= " WHERE Status = @Status"
        End If


        com &= " ORDER BY TransactionNo, BookTitle, AccessionID"

        Dim comsu As New MySqlCommand(com, con)

        If Not String.IsNullOrEmpty(filterStatus) Then
            comsu.Parameters.AddWithValue("@Status", filterStatus)
        End If

        Dim adap As New MySqlDataAdapter(comsu)
        Dim ds As New DataSet

        Try
            adap.Fill(ds, "INFO")
            DataGridView1.DataSource = ds.Tables("INFO")

            DataGridView1.EnableHeadersVisualStyles = False

            DataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(207, 58, 109)
            DataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White


            If DataGridView1.Columns.Contains("ID") Then
                DataGridView1.Columns("ID").Visible = False
            End If

        Catch ex As Exception
            MessageBox.Show("Error refreshing data: " & ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

        btnview.Enabled = True
        btnview.Visible = True
        shelfsu()
        DataGridView1.ClearSelection()

        For Each form In Application.OpenForms
            If TypeOf form Is MainForm Then
                Dim load = DirectCast(form, MainForm)
                load.lbldamagecount()
                load.lbllostcount()
                load.lbloverduecount()
            End If
        Next
    End Sub

    Public Sub shelfsu()

        Dim con As New MySqlConnection(connectionString)
        Dim com As String = "SELECT ID, Shelf FROM `shelf_tbl`"
        Dim adap As New MySqlDataAdapter(com, con)
        Dim dt As New DataTable

        adap.Fill(dt)

        cbshelf.DataSource = dt
        cbshelf.DisplayMember = "Shelf"
        cbshelf.ValueMember = "ID"
        cbshelf.SelectedIndex = -1

    End Sub

    Public Sub clearlahat()

        txtbarcodes.Text = ""
        txtisbn.Text = ""
        txtbooktitle.Text = ""
        txtaccessionid.Text = ""
        txtsuppliername.Text = ""
        txttransactionno.Text = ""

        cbshelf.DataSource = Nothing
        shelfsu()

        DataGridView1.ClearSelection()

        rbborrowable.Checked = False
        rbforlibraryonly.Checked = False


        CheckBox1.Checked = False



    End Sub



    Private Sub Accession_Shown(sender As Object, e As EventArgs) Handles MyBase.Shown
        DataGridView1.ClearSelection()
    End Sub

    Private Sub Acession_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        RefreshAccessionData()

        DataGridView1.EnableHeadersVisualStyles = False
        DataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(207, 58, 109)
        DataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White


        DataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        DataGridView1.MultiSelect = True

        txtaccessionid.Enabled = False
        txtbooktitle.Enabled = False
        txtisbn.Enabled = False
        txtbarcodes.Enabled = False
        txttransactionno.Enabled = False
        txtsuppliername.Enabled = False




    End Sub



    Private Sub CheckBox1_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox1.CheckedChanged

        If CheckBox1.Checked Then


            RefreshAccessionData("Available")


            DataGridView1.MultiSelect = True


            DataGridView1.ClearSelection()
            DataGridView1.CurrentCell = Nothing


            lblnotes.Visible = True
            lblnote.Visible = True

            lblnote.Text = "Please hold the 'CTRL' key and select multiple rows. Press 'ENTER' to save."


            btntransaction.Enabled = False

            rbborrowable.Enabled = False
            rbforlibraryonly.Enabled = False
            cbshelf.Enabled = False

            btnview.Visible = True
            btnshelf.Enabled = False
            clearna()


        Else


            RefreshAccessionData()


            DataGridView1.MultiSelect = False

            DataGridView1.ClearSelection()

            lblnotes.Visible = False
            lblnote.Visible = False

            btnview.Visible = True

            btntransaction.Enabled = True

            rbborrowable.Enabled = True
            rbforlibraryonly.Enabled = True
            cbshelf.Enabled = True
            btnshelf.Enabled = True

        End If
    End Sub

    Public Sub clearna()

        txtbarcodes.Text = ""
        txtisbn.Text = ""
        txtbooktitle.Text = ""
        txtaccessionid.Text = ""
        txtsuppliername.Text = ""
        txttransactionno.Text = ""

        btnview.Visible = True

        rbborrowable.Checked = False
        rbforlibraryonly.Checked = False
        cbshelf.DataSource = Nothing
        shelfsu()

    End Sub

    Private Sub DataGridView1_KeyDown(sender As Object, e As KeyEventArgs) Handles DataGridView1.KeyDown


        If CheckBox1.Checked AndAlso e.KeyCode = Keys.Enter Then
            e.SuppressKeyPress = True

            Dim selectedRowsCount As Integer = DataGridView1.SelectedRows.Count
            Dim availableRowCount As Integer = DataGridView1.Rows.Count

            If selectedRowsCount = 0 Then
                MessageBox.Show("Please select at least one row to reserve.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If

            If selectedRowsCount = availableRowCount Then
                MessageBox.Show("Cannot reserve all available copies", "Action Restricted", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If


            Dim dialogResult As DialogResult = MessageBox.Show($"Are you sure you want to reserve the {selectedRowsCount} selected copies?", "Confirm Reserve", MessageBoxButtons.YesNo, MessageBoxIcon.Question)

            If dialogResult = DialogResult.Yes Then

                SaveReserveCopies()

                CheckBox1.Checked = False

            End If
        End If

    End Sub


    Public Sub SaveReserveCopies()

        Dim con As New MySqlConnection(connectionString)
        Dim transaction As MySqlTransaction = Nothing

        Dim moveit As New List(Of String)
        For Each row As DataGridViewRow In DataGridView1.SelectedRows
            If Not row.IsNewRow Then
                Dim status As String = row.Cells("Status").Value.ToString().Trim()
                If status.Equals("Available", StringComparison.OrdinalIgnoreCase) Then
                    moveit.Add(row.Cells("AccessionID").Value.ToString())
                End If
            End If
        Next

        If moveit.Count = 0 Then
            MessageBox.Show("No available copies selected for reservation.", "Reservation Aborted", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If

        Try
            con.Open()
            transaction = con.BeginTransaction()

            Dim reserveCount As Integer = 0

            For Each accID As String In moveit


                Dim comsiss As String = "SELECT * FROM `acession_tbl` WHERE AccessionID = @AccessionID AND Status = 'Available'"

                Using comsusx As New MySqlCommand(comsiss, con, transaction)
                    comsusx.Parameters.AddWithValue("@AccessionID", accID)
                    Dim reader As MySqlDataReader = comsusx.ExecuteReader()

                    If reader.Read() Then

                        Dim tranNo As String = reader("TransactionNo").ToString()
                        Dim isbn As Object = reader("ISBN")
                        Dim barcode As Object = reader("Barcode")
                        Dim bookTitle As String = reader("BookTitle").ToString()
                        Dim shelf As String = reader("Shelf").ToString()
                        Dim supplierName As String = reader("SupplierName").ToString()

                        reader.Close()


                        Dim ins As String = "INSERT INTO `reservecopiess_tbl` (`TransactionNo`, `AccessionID`, `ISBN`, `Barcode`, `BookTitle`, `Shelf`, `SupplierName`, `Status`) " &
                                            "VALUES (@TransactionNo, @AccessionID, @ISBN, @Barcode, @BookTitle, @Shelf, @SupplierName, 'Reserved')"
                        Using insx As New MySqlCommand(ins, con, transaction)

                            insx.Parameters.AddWithValue("@TransactionNo", tranNo)
                            insx.Parameters.AddWithValue("@AccessionID", accID)
                            insx.Parameters.AddWithValue("@ISBN", If(IsDBNull(isbn), CType(DBNull.Value, Object), isbn))
                            insx.Parameters.AddWithValue("@Barcode", If(IsDBNull(barcode), CType(DBNull.Value, Object), barcode))
                            insx.Parameters.AddWithValue("@BookTitle", bookTitle)
                            insx.Parameters.AddWithValue("@Shelf", shelf)
                            insx.Parameters.AddWithValue("@SupplierName", supplierName)
                            insx.ExecuteNonQuery()
                        End Using

                        Dim deleyt As String = "DELETE FROM `acession_tbl` WHERE AccessionID = @AccessionID_Del"
                        Using deletesu As New MySqlCommand(deleyt, con, transaction)
                            deletesu.Parameters.AddWithValue("@AccessionID_Del", accID)
                            deletesu.ExecuteNonQuery()
                        End Using

                        Dim deleteAvailable As String = "DELETE FROM `available_tbl` WHERE AccessionID = @AccessionID_AvailDel"
                        Using deleteAvailCmd As New MySqlCommand(deleteAvailable, con, transaction)
                            deleteAvailCmd.Parameters.AddWithValue("@AccessionID_AvailDel", accID)
                            deleteAvailCmd.ExecuteNonQuery()
                        End Using


                        reserveCount += 1
                    Else
                        reader.Close()
                    End If
                End Using

            Next

            transaction.Commit()

            btnview.Visible = True

            MessageBox.Show("Reserve copies saved. Total reserved: " & reserveCount.ToString(), "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)

            For Each form In Application.OpenForms
                If TypeOf form Is AvailableBooks Then
                    Dim avail = DirectCast(form, AvailableBooks)
                    avail.refreshavail()
                    avail.counts()
                End If
            Next



        Catch ex As Exception
            If transaction IsNot Nothing Then
                Try
                    transaction.Rollback()
                Catch rollEx As Exception
                    MessageBox.Show("Error during rollback: " & rollEx.Message, "Rollback Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End Try
            End If
            MessageBox.Show("Error saving reserve copies: " & ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error)

        Finally
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
        End Try
    End Sub

    Private Sub btntransaction_Click(sender As Object, e As EventArgs) Handles btntransaction.Click

        TransactionNumber.ShowDialog()

        For Each form In Application.OpenForms
            If TypeOf form Is TransactionNumber Then
                Dim load = DirectCast(form, TransactionNumber)
                load.LoadTransactions()
            End If
        Next
    End Sub



    Private Sub btnadd_Click(sender As Object, e As EventArgs) Handles btnadd.Click

        If CheckBox1.Checked Then
            MessageBox.Show("Please uncheck 'Select Reserve Copies' to add new records.", "Action Restricted", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Dim con As New MySqlConnection(connectionString)

        Dim cleanedBookTitle As String = txtbooktitle.Text.Trim()

        Try
            con.Open()

            If String.IsNullOrWhiteSpace(txttransactionno.Text) OrElse String.IsNullOrWhiteSpace(txtaccessionid.Text) OrElse cbshelf.SelectedIndex = -1 Then
                MessageBox.Show("Please fill all required fields.", "Required Fields", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If



            Dim statusValue As String = ""

            Dim isAvailable As Boolean = rbborrowable.Checked
            If isAvailable Then
                statusValue = "Available"
            ElseIf rbforlibraryonly.Checked Then
                statusValue = "For In-Library Use Only"
            Else
                MsgBox("Please select a Book Status.", vbExclamation, "Missing Information")
                Exit Sub
            End If

            Dim accessionIDs As String() = txtaccessionid.Text.Split(","c)


            For Each accessionID As String In accessionIDs
                Dim acss As String = accessionID.Trim()


                If Not String.IsNullOrWhiteSpace(acss) Then

                    Dim ckacs As String = "SELECT COUNT(*) FROM `acession_tbl` WHERE AccessionID = @AccessionID"
                    Using comsx As New MySqlCommand(ckacs, con)
                        comsx.Parameters.AddWithValue("@AccessionID", acss)
                        If CInt(comsx.ExecuteScalar()) > 0 Then
                            MessageBox.Show("Accession ID '" & acss & "' already exists. Please use a unique Accession ID.", "Duplicate Accession ID", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                            Continue For
                        End If
                    End Using

                    Dim com As String = "INSERT INTO acession_tbl (`TransactionNo`, `AccessionID`, `ISBN`, `Barcode`, `BookTitle`, `Shelf`, `SupplierName`, `Status`) " &
                                    "VALUES (@TransactionNo, @AccessionID, @ISBN, @Barcode, @BookTitle, @Shelf, @SupplierName, @Status)"

                    Using comsu As New MySqlCommand(com, con)
                        comsu.Parameters.AddWithValue("@TransactionNo", txttransactionno.Text)
                        comsu.Parameters.AddWithValue("@AccessionID", acss)
                        comsu.Parameters.AddWithValue("@ISBN", If(String.IsNullOrWhiteSpace(txtisbn.Text), CType(DBNull.Value, Object), txtisbn.Text))
                        comsu.Parameters.AddWithValue("@Barcode", If(String.IsNullOrWhiteSpace(txtbarcodes.Text), CType(DBNull.Value, Object), txtbarcodes.Text))

                        comsu.Parameters.AddWithValue("@BookTitle", cleanedBookTitle)
                        comsu.Parameters.AddWithValue("@Shelf", cbshelf.Text)
                        comsu.Parameters.AddWithValue("@SupplierName", txtsuppliername.Text)

                        comsu.Parameters.AddWithValue("@Status", statusValue)
                        comsu.ExecuteNonQuery()
                    End Using

                    If isAvailable Then
                        Dim availableInsertSql As String = "INSERT INTO available_tbl (`ID`, `ISBN`, `Barcode`, `AccessionID`, `BookTitle`, `Shelf`, `Status`) " &
                                                     "VALUES (NULL, @ISBN, @Barcode, @AccessionID, @BookTitle, @Shelf, @Status)"

                        Using availableCmd As New MySqlCommand(availableInsertSql, con)
                            availableCmd.Parameters.AddWithValue("@AccessionID", acss)
                            availableCmd.Parameters.AddWithValue("@ISBN", If(String.IsNullOrWhiteSpace(txtisbn.Text), CType(DBNull.Value, Object), txtisbn.Text))
                            availableCmd.Parameters.AddWithValue("@Barcode", If(String.IsNullOrWhiteSpace(txtbarcodes.Text), CType(DBNull.Value, Object), txtbarcodes.Text))

                            availableCmd.Parameters.AddWithValue("@BookTitle", cleanedBookTitle)
                            availableCmd.Parameters.AddWithValue("@Shelf", cbshelf.Text)
                            availableCmd.Parameters.AddWithValue("@Status", statusValue)
                            availableCmd.ExecuteNonQuery()
                        End Using
                    End If

                End If
            Next


            For Each form In Application.OpenForms
                If TypeOf form Is AvailableBooks Then
                    Dim avail = DirectCast(form, AvailableBooks)
                    avail.refreshavail()
                    avail.counts()
                End If
            Next


            For Each form In Application.OpenForms
                If TypeOf form Is TransactionNumber Then
                    Dim load = DirectCast(form, TransactionNumber)
                    load.LoadTransactions()
                End If
            Next

            MessageBox.Show("Successfully added all accession records!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)


            Acession_Load(sender, e)
            clearlahat()

        Catch ex As Exception
            MessageBox.Show("Error: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
        End Try

    End Sub

    Private Sub btnedit_Click(sender As Object, e As EventArgs) Handles btnedit.Click

        If CheckBox1.Checked Then
            MessageBox.Show("Please uncheck 'Select Reserve Copies' to edit records.", "Action Restricted", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        If DataGridView1.SelectedRows.Count = 0 Then
            MessageBox.Show("Please select a record from the table to edit.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If

        Dim selectedRow As DataGridViewRow = DataGridView1.SelectedRows(0)
        Dim currentStatus As String = selectedRow.Cells("Status").Value.ToString().Trim()
        Dim oldAccessionID As String = selectedRow.Cells("AccessionID").Value.ToString().Trim()
        Dim abeyl As Boolean = currentStatus.Equals("Available", StringComparison.OrdinalIgnoreCase)
        Dim libraryonleh As Boolean = currentStatus.Equals("For In-Library Use Only", StringComparison.OrdinalIgnoreCase)


        Dim statsskie As New List(Of String) From {"Pending", "Lost", "Damage"}


        If statsskie.Contains(currentStatus, StringComparer.OrdinalIgnoreCase) Then
            MessageBox.Show($"Cannot edit this accession record. The current status is '{currentStatus}'.", "Editing Restricted", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        If String.IsNullOrWhiteSpace(txttransactionno.Text) OrElse String.IsNullOrWhiteSpace(txtaccessionid.Text) OrElse cbshelf.SelectedIndex = -1 Then
            MessageBox.Show("Please fill all required fields.", "Required Fields", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Dim con As New MySqlConnection(connectionString)
        Dim ID As Integer = CInt(selectedRow.Cells("ID").Value)
        Dim newAccessionID As String = txtaccessionid.Text.Trim()


        Dim newStatusValue As String = ""
        Dim avail As Boolean = rbborrowable.Checked
        Dim laybrarisu As Boolean = rbforlibraryonly.Checked

        If avail Then
            newStatusValue = "Available"
        ElseIf laybrarisu Then
            newStatusValue = "For In-Library Use Only"
        Else
            MsgBox("Please select a Book Status (Borrowable or For In-Library Use Only).", vbExclamation, "Missing Information")
            Return
        End If

        Try
            con.Open()


            If newAccessionID <> oldAccessionID Then
                Dim com As String = "SELECT COUNT(*) FROM `acession_tbl` WHERE AccessionID = @AccessionID AND ID <> @ID"
                Using comsss As New MySqlCommand(com, con)
                    comsss.Parameters.AddWithValue("@AccessionID", newAccessionID)
                    comsss.Parameters.AddWithValue("@ID", ID)
                    If CInt(comsss.ExecuteScalar()) > 0 Then
                        MessageBox.Show("Accession ID '" & newAccessionID & "' already exists for another record. Update aborted.", "Duplicate Accession ID", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                        Return
                    End If
                End Using
            End If


            Dim coms As String = "SELECT COUNT(*) FROM `borrowing_tbl` WHERE AccessionID = @accessionID"
            Dim comsuu As New MySqlCommand(coms, con)
            comsuu.Parameters.AddWithValue("@accessionID", oldAccessionID)
            Dim isBorrowed As Integer = CInt(comsuu.ExecuteScalar())

            If isBorrowed > 0 Then
                MessageBox.Show("Cannot modify this accession record. It is currently being borrowed.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If

            Dim updt As String = "UPDATE `acession_tbl` SET " &
                             "`TransactionNo` = @TransactionNo, " &
                             "`AccessionID` = @AccessionID, " &
                             "`ISBN` = @ISBN, " &
                             "`Barcode` = @Barcode, " &
                             "`BookTitle` = @BookTitle, " &
                             "`Shelf` = @Shelf, " &
                             "`SupplierName` = @SupplierName, " &
                             "`Status` = @Status " &
                             "WHERE `ID` = @ID"

            Using command As New MySqlCommand(updt, con)
                command.Parameters.AddWithValue("@TransactionNo", txttransactionno.Text)
                command.Parameters.AddWithValue("@AccessionID", newAccessionID)
                command.Parameters.AddWithValue("@ISBN", If(String.IsNullOrWhiteSpace(txtisbn.Text), CType(DBNull.Value, Object), txtisbn.Text))
                command.Parameters.AddWithValue("@Barcode", If(String.IsNullOrWhiteSpace(txtbarcodes.Text), CType(DBNull.Value, Object), txtbarcodes.Text))
                command.Parameters.AddWithValue("@BookTitle", txtbooktitle.Text)
                command.Parameters.AddWithValue("@Shelf", cbshelf.Text)
                command.Parameters.AddWithValue("@SupplierName", txtsuppliername.Text)
                command.Parameters.AddWithValue("@Status", newStatusValue)
                command.Parameters.AddWithValue("@ID", ID)

                command.ExecuteNonQuery()
            End Using

            If abeyl AndAlso laybrarisu Then
                Dim deleteSql As String = "DELETE FROM `available_tbl` WHERE AccessionID = @AccessionID"
                Using deleteCmd As New MySqlCommand(deleteSql, con)
                    deleteCmd.Parameters.AddWithValue("@AccessionID", oldAccessionID)
                    deleteCmd.ExecuteNonQuery()
                End Using


            ElseIf libraryonleh AndAlso avail Then
                Dim kowms As String = "INSERT INTO available_tbl (`ISBN`, `Barcode`, `AccessionID`, `BookTitle`, `Shelf`, `Status`) " &
                                     "VALUES (@ISBN, @Barcode, @AccessionID, @BookTitle, @Shelf, @Status)"

                Using commandsu As New MySqlCommand(kowms, con)
                    commandsu.Parameters.AddWithValue("@AccessionID", newAccessionID)
                    commandsu.Parameters.AddWithValue("@ISBN", If(String.IsNullOrWhiteSpace(txtisbn.Text), CType(DBNull.Value, Object), txtisbn.Text))
                    commandsu.Parameters.AddWithValue("@Barcode", If(String.IsNullOrWhiteSpace(txtbarcodes.Text), CType(DBNull.Value, Object), txtbarcodes.Text))
                    commandsu.Parameters.AddWithValue("@BookTitle", txtbooktitle.Text)
                    commandsu.Parameters.AddWithValue("@Shelf", cbshelf.Text)
                    commandsu.Parameters.AddWithValue("@Status", newStatusValue)
                    commandsu.ExecuteNonQuery()
                End Using


            ElseIf abeyl AndAlso avail Then
                Dim updit As String = "UPDATE `available_tbl` SET AccessionID = @NewAccessionID, BookTitle = @BookTitle, ISBN = @ISBN, Barcode = @Barcode, Shelf = @Shelf, Status = @Status WHERE AccessionID = @OldAccessionID"
                Using comssxx As New MySqlCommand(updit, con)
                    comssxx.Parameters.AddWithValue("@NewAccessionID", newAccessionID)
                    comssxx.Parameters.AddWithValue("@OldAccessionID", oldAccessionID)
                    comssxx.Parameters.AddWithValue("@BookTitle", txtbooktitle.Text)
                    comssxx.Parameters.AddWithValue("@ISBN", If(String.IsNullOrWhiteSpace(txtisbn.Text), CType(DBNull.Value, Object), txtisbn.Text))
                    comssxx.Parameters.AddWithValue("@Barcode", If(String.IsNullOrWhiteSpace(txtbarcodes.Text), CType(DBNull.Value, Object), txtbarcodes.Text))
                    comssxx.Parameters.AddWithValue("@Shelf", cbshelf.Text)
                    comssxx.Parameters.AddWithValue("@Status", newStatusValue)
                    comssxx.ExecuteNonQuery()
                End Using

            End If


            For Each form In Application.OpenForms
                If TypeOf form Is AvailableBooks Then
                    Dim availsu = DirectCast(form, AvailableBooks)
                    availsu.refreshavail()
                    availsu.counts()
                End If
            Next

            MessageBox.Show("Accession record updated successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)

            Acession_Load(sender, e)
            clearlahat()

        Catch ex As Exception
            MessageBox.Show("Error updating record: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
        End Try

    End Sub

    Private Sub btndeleteall_Click(sender As Object, e As EventArgs) Handles btndeleteall.Click

        If CheckBox1.Checked Then
            MessageBox.Show("Please uncheck 'Select Reserve Copies' to delete all records.", "Action Restricted", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Dim firstDialogResult As DialogResult = MessageBox.Show("Are you sure you want to delete ALL accession records? This action cannot be undone.", "Confirm Delete All", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)

        If firstDialogResult = DialogResult.Yes Then
            Dim secondDialogResult As DialogResult = MessageBox.Show("This will permanently delete all accession records. Are you absolutely sure?", "Final Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question)

            If secondDialogResult = DialogResult.Yes Then
                Dim con As New MySqlConnection(connectionString)

                Try
                    con.Open()

                    Dim comm As String = "SELECT COUNT(*) FROM `acession_tbl` WHERE `Status` IN ('Pending', 'Lost', 'Damage')"
                    Dim comssu As New MySqlCommand(comm, con)
                    Dim count As Integer = CInt(comssu.ExecuteScalar())

                    If count > 0 Then
                        MessageBox.Show("Cannot delete all accession records. There are currently " & count.ToString() & " record(s) with 'Pending', 'Lost', or 'Damage' status.", "Deletion Restricted", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        Return
                    End If

                    Dim com As String = "SELECT COUNT(*) FROM `borrowing_tbl`"
                    Dim kapagod As New MySqlCommand(com, con)
                    Dim borrowedCount As Integer = CInt(kapagod.ExecuteScalar())

                    If borrowedCount > 0 Then
                        MessageBox.Show("Cannot delete all accession records. Some records are currently borrowed.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                        Return
                    End If

                    Dim reserveCheckSql As String = "SELECT COUNT(a.TransactionNo) FROM `acession_tbl` a JOIN `reservecopiess_tbl` r ON a.TransactionNo = r.TransactionNo LIMIT 1"

                    Dim reserveCmd As New MySqlCommand(reserveCheckSql, con)
                    Dim reservedCount As Integer = CInt(reserveCmd.ExecuteScalar())

                    If reservedCount > 0 Then
                        MessageBox.Show("Cannot delete all accession records. Kindly pushback all transaction before deleting.", "Deletion Restricted - Reserved Books", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        Return
                    End If

                    Dim coms As String = "DELETE FROM `acession_tbl`"
                    Dim comsuus As New MySqlCommand(coms, con)
                    comsuus.ExecuteNonQuery()

                    Dim comsAvail As String = "DELETE FROM `available_tbl`"
                    Dim comsuusAvail As New MySqlCommand(comsAvail, con)
                    comsuusAvail.ExecuteNonQuery()

                    Dim resett As String = "ALTER TABLE `acession_tbl` AUTO_INCREMENT = 1"
                    Dim incrementsu As New MySqlCommand(resett, con)
                    incrementsu.ExecuteNonQuery()

                    MessageBox.Show("All accession records have been successfully deleted.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)

                    For Each form In Application.OpenForms
                        If TypeOf form Is AvailableBooks Then
                            Dim avail = DirectCast(form, AvailableBooks)
                            avail.refreshavail()
                            avail.counts()
                        End If
                    Next

                    For Each form In Application.OpenForms
                        If TypeOf form Is TransactionNumber Then
                            Dim load = DirectCast(form, TransactionNumber)
                            load.LoadTransactions()

                        End If
                    Next
                    Acession_Load(sender, e)
                    clearlahat()

                Catch ex As Exception
                    MessageBox.Show("Error deleting all records: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Finally
                    If con.State = ConnectionState.Open Then
                        con.Close()
                    End If
                End Try
            End If
        End If

    End Sub

    Private Sub btnclear_Click(sender As Object, e As EventArgs) Handles btnclear.Click
        clearlahat()
    End Sub

    Private Sub DataGridView1_CellClick_1(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellClick

        If CheckBox1.Checked Then
            Return
        End If


        If e.RowIndex >= 0 Then
            Dim row = DataGridView1.Rows(e.RowIndex)

            txtaccessionid.Text = row.Cells("AccessionID").Value.ToString
            txtbooktitle.Text = row.Cells("BookTitle").Value.ToString
            txtisbn.Text = row.Cells("ISBN").Value.ToString
            txtbarcodes.Text = row.Cells("Barcode").Value.ToString
            txtsuppliername.Text = row.Cells("SupplierName").Value.ToString
            txttransactionno.Text = row.Cells("TransactionNo").Value.ToString

            Dim shelfValue = row.Cells("Shelf").Value.ToString
            cbshelf.SelectedIndex = cbshelf.FindStringExact(shelfValue)


            Dim statusValue As String = row.Cells("Status").Value.ToString

            If statusValue.Equals("Available", StringComparison.OrdinalIgnoreCase) Then
                rbborrowable.Checked = True
            ElseIf statusValue.Equals("For In-Library Use Only", StringComparison.OrdinalIgnoreCase) Then
                rbforlibraryonly.Checked = True
            Else

                rbborrowable.Checked = True
            End If

        End If

    End Sub

    Private Sub btnview_Click(sender As Object, e As EventArgs) Handles btnview.Click

        For Each form In Application.OpenForms
            If TypeOf form Is ReserveCopies Then
                Dim risirb = DirectCast(form, ReserveCopies)
                risirb.reserveload()
            End If
        Next

        ReserveCopies.ShowDialog()

    End Sub

    Private Sub txtsearch_TextChanged(sender As Object, e As EventArgs) Handles txtsearch.TextChanged


        Dim dt As DataTable = DirectCast(DataGridView1.DataSource, DataTable)
        If dt IsNot Nothing Then
            If txtsearch.Text.Trim() <> "" Then
                Dim filter As String = String.Format("BookTitle LIKE '*{0}*' OR Status LIKE '*{0}*'", txtsearch.Text.Trim())
                dt.DefaultView.RowFilter = filter
            Else
                dt.DefaultView.RowFilter = ""
            End If
        End If

    End Sub

    Private Sub btnshelf_Click(sender As Object, e As EventArgs) Handles btnshelf.Click
        Shelf.ShowDialog()
    End Sub

    Private Sub btnshelf_MouseHover(sender As Object, e As EventArgs) Handles btnshelf.MouseHover
        Cursor = Cursors.Hand
    End Sub

    Private Sub btnshelf_MouseLeave(sender As Object, e As EventArgs) Handles btnshelf.MouseLeave
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

    Private Sub btndelete_MouseHover(sender As Object, e As EventArgs) Handles btndeleteall.MouseHover
        Cursor = Cursors.Hand
    End Sub

    Private Sub btndelete_MouseLeave(sender As Object, e As EventArgs) Handles btndeleteall.MouseLeave
        Cursor = Cursors.Default
    End Sub

    Private Sub btnclear_MouseHover(sender As Object, e As EventArgs) Handles btnclear.MouseHover
        Cursor = Cursors.Hand
    End Sub

    Private Sub btnclear_MouseLeave(sender As Object, e As EventArgs) Handles btnclear.MouseLeave
        Cursor = Cursors.Default
    End Sub

    Private Sub btntransaction_MouseHover(sender As Object, e As EventArgs) Handles btntransaction.MouseHover
        Cursor = Cursors.Hand
    End Sub

    Private Sub btntransaction_MouseLeave(sender As Object, e As EventArgs) Handles btntransaction.MouseLeave
        Cursor = Cursors.Default
    End Sub

    Private Sub btnview_MouseHover(sender As Object, e As EventArgs) Handles btnview.MouseHover
        Cursor = Cursors.Hand
    End Sub

    Private Sub btnview_MouseLeave(sender As Object, e As EventArgs) Handles btnview.MouseLeave
        Cursor = Cursors.Default
    End Sub

    Private Sub CheckBox1_MouseHover(sender As Object, e As EventArgs) Handles CheckBox1.MouseHover
        Cursor = Cursors.Hand
    End Sub

    Private Sub CheckBox1_MouseLeave(sender As Object, e As EventArgs) Handles CheckBox1.MouseLeave
        Cursor = Cursors.Default
    End Sub
End Class