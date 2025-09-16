Imports MySql.Data.MySqlClient

Public Class Acquisition

    ' I-delete na ang TNForm at lastTransactionNo dahil hindi na kailangan sa bagong logic
    ' Dim tnForm As New TransactionNumber()
    ' Private lastTransactionNo As String = ""

    Private Sub Acquisition_Load_1(sender As Object, e As EventArgs) Handles MyBase.Load
        ' ... (No change sa DGV loading)
        Dim con As New MySqlConnection(connectionString)
        Dim com As String = "SELECT * FROM `acquisition_tbl`"
        Dim adp As New MySqlDataAdapter(com, con)
        Dim dt As New DataSet

        adp.Fill(dt, "INFO")
        dgv_acquisition.DataSource = dt.Tables("INFO")

        dgv_acquisition.Columns("ID").Visible = False
        dgv_acquisition.EnableHeadersVisualStyles = False
        dgv_acquisition.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(207, 58, 109)
        dgv_acquisition.ColumnHeadersDefaultCellStyle.ForeColor = Color.White


        jineret()
        cbsupplierr()
        clear()

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

    Private Sub txtsearch_TextChanged(sender As Object, e As EventArgs) Handles txtsearch.TextChanged

        Dim dt As DataTable = DirectCast(dgv_acquisition.DataSource, DataTable)
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

    Private Sub btnadd_Click(sender As Object, e As EventArgs) Handles btnadd.Click


        Dim con As New MySqlConnection(connectionString)
        If String.IsNullOrWhiteSpace(txtbooktitle.Text) OrElse String.IsNullOrWhiteSpace(txtbookprice.Text) Then
            MessageBox.Show("Please fill the required fields.", "Required", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        If NumericUpDown1.Value <= 0 Then
            MessageBox.Show("Quantity cannot be 0 or less.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning)
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

        Try
            con.Open()

            If Not String.IsNullOrWhiteSpace(txttransactionno.Text) Then
                Dim coms As String = "SELECT COUNT(*) FROM acquisition_tbl WHERE TransactionNo = @TransactionNo"
                Dim counts As New MySqlCommand(coms, con)
                counts.Parameters.AddWithValue("@TransactionNo", txttransactionno.Text)
                Dim bilang As Integer = CInt(counts.ExecuteScalar())
                If bilang > 0 Then
                    MessageBox.Show("This Transaction Number already exists.", "Duplication Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    jineret()
                    Return
                End If
            End If

            If Not String.IsNullOrWhiteSpace(txtbarcodes.Text) Then
                Dim comsis As String = "SELECT COUNT(*) FROM book_tbl WHERE Barcode = @Barcode"
                Dim kownts As New MySqlCommand(comsis, con)
                kownts.Parameters.AddWithValue("@Barcode", txtbarcodes.Text)
                Dim sotired As Integer = CInt(kownts.ExecuteScalar())
                If sotired > 0 Then
                    MessageBox.Show("This Barcode already exists.", "Duplication Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
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
                MessageBox.Show("Successfully added!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)


                Acquisition_Load_1(sender, e)
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


        If dgv_acquisition.SelectedRows.Count = 0 Then
            MessageBox.Show("Please select a record to edit.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If

        Dim selectedRow As DataGridViewRow = dgv_acquisition.SelectedRows(0)
        Dim ID As Integer = CInt(selectedRow.Cells("ID").Value)
        Dim old As Integer = CInt(selectedRow.Cells("Quantity").Value)
        Dim neww As Integer = CInt(NumericUpDown1.Value)

        If String.IsNullOrWhiteSpace(txtisbn.Text) OrElse String.IsNullOrWhiteSpace(txtbooktitle.Text) Then
            MessageBox.Show("Please fill out the required fields.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Dim con As New MySqlConnection(connectionString)
        Dim originalShelf As Object = DBNull.Value

        Dim deyts As DateTime = DateTimePicker1.Value
        If deyts.Date > DateTime.Today.Date Then
            MsgBox("You cannot select a future date.", vbExclamation)
            Return
        End If
        Dim purmatdeyt As String = deyts.ToString("yyyy-MM-dd")

        Try
            con.Open()

            Dim transactionNo As String = selectedRow.Cells("TransactionNo").Value.ToString()



            If neww < old Then
                Dim recordsdelete As Integer = old - neww
                Dim deleteAcs As String = "DELETE FROM `acession_tbl` WHERE TransactionNo = @TransactionNo AND Status = 'Available' ORDER BY AccessionID DESC LIMIT @limit"
                Dim deleyt As New MySqlCommand(deleteAcs, con)
                deleyt.Parameters.AddWithValue("@TransactionNo", transactionNo)
                deleyt.Parameters.AddWithValue("@limit", recordsdelete)
                deleyt.ExecuteNonQuery()


            ElseIf neww > old Then

                Dim recordsadd As Integer = neww - old

                Dim rand As New Random()
                Dim newAccessionID As String = ""
                Dim isUnique As Boolean = False
                Dim shelf As String = ""

                Dim getShelfQuery As String = "SELECT Shelf FROM `acession_tbl` WHERE TransactionNo = @TransactionNo LIMIT 1"
                Dim getShelfCommand As New MySqlCommand(getShelfQuery, con)

                getShelfCommand.Parameters.AddWithValue("@TransactionNo", transactionNo)

                Dim shelfResult As Object = getShelfCommand.ExecuteScalar()
                If shelfResult IsNot Nothing AndAlso shelfResult IsNot DBNull.Value Then
                    shelf = shelfResult.ToString()
                End If


                For i As Integer = 1 To recordsadd
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
                        insertAcession.Parameters.AddWithValue("@BookTitle", txtbooktitle.Text)
                        insertAcession.Parameters.AddWithValue("@Shelf", shelf)
                        insertAcession.Parameters.AddWithValue("@SupplierName", cbsuppliername.Text)
                        insertAcession.Parameters.AddWithValue("@Status", "Available")
                        insertAcession.ExecuteNonQuery()
                    End Using
                Next
            End If


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
                Updateacqt.Parameters.AddWithValue("@BookTitle", txtbooktitle.Text)
                Updateacqt.Parameters.AddWithValue("@SupplierName", cbsuppliername.Text)
                Updateacqt.Parameters.AddWithValue("@Quantity", neww)
                Updateacqt.Parameters.AddWithValue("@BookPrice", CDbl(txtbookprice.Text))
                Updateacqt.Parameters.AddWithValue("@TotalCost", CDbl(txttotalcost.Text))
                Updateacqt.Parameters.AddWithValue("@DateAcquired", purmatdeyt)
                Updateacqt.Parameters.AddWithValue("@ID", ID)
                Updateacqt.ExecuteNonQuery()
            End Using

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

        If dgv_acquisition.SelectedRows.Count > 0 Then
            Dim dialogResult As DialogResult = MessageBox.Show("Are you sure you want to delete this acquisition record?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)
            If dialogResult = DialogResult.Yes Then
                Dim con As New MySqlConnection(connectionString)
                Dim selectedRow As DataGridViewRow = dgv_acquisition.SelectedRows(0)
                Dim ID As Integer = CInt(selectedRow.Cells("ID").Value)
                Try
                    con.Open()
                    Dim deleteCmd As New MySqlCommand("DELETE FROM `acquisition_tbl` WHERE `ID` = @id", con)
                    deleteCmd.Parameters.AddWithValue("@id", ID)
                    deleteCmd.ExecuteNonQuery()

                    MessageBox.Show("Record deleted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)

                    Acquisition_Load_1(sender, e)

                Catch ex As Exception
                    MessageBox.Show("Error deleting record: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Finally
                    If con.State = ConnectionState.Open Then
                        con.Close()
                    End If
                End Try
            End If
        Else
            MessageBox.Show("Please select a row to delete.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If
    End Sub

    Private Sub btnclear_Click(sender As Object, e As EventArgs) Handles btnclear.Click

        clear()

    End Sub

    Private Sub dgv_acquisition_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles dgv_acquisition.CellClick

        If e.RowIndex >= 0 Then

            clear()


            txtisbn.Enabled = False
            txtbarcodes.Enabled = False
            txtbooktitle.Enabled = False
            txttotalcost.Enabled = False
            txttransactionno.Enabled = False

            Dim selectedRow As DataGridViewRow = dgv_acquisition.Rows(e.RowIndex)


            txtisbn.Text = selectedRow.Cells("ISBN").Value.ToString()
            If selectedRow.Cells("Barcode").Value IsNot DBNull.Value AndAlso selectedRow.Cells("Barcode").Value IsNot Nothing Then
                txtbarcodes.Text = selectedRow.Cells("Barcode").Value.ToString()
            Else
                txtbarcodes.Text = ""
            End If
            txtbooktitle.Text = selectedRow.Cells("BookTitle").Value.ToString()
            cbsuppliername.Text = selectedRow.Cells("SupplierName").Value.ToString()
            NumericUpDown1.Value = Convert.ToDecimal(selectedRow.Cells("Quantity").Value)
            txtbookprice.Text = selectedRow.Cells("BookPrice").Value.ToString()
            txttotalcost.Text = selectedRow.Cells("TotalCost").Value.ToString()
            DateTimePicker1.Value = CDate(selectedRow.Cells("DateAcquired").Value)
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


    Public Sub clear()

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

        DateTimePicker1.Value = DateTime.Now
        dgv_acquisition.ClearSelection()
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

End Class