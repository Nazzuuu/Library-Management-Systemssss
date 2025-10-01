Imports MySql.Data.MySqlClient

Public Class Accession
    Public Sub RefreshAccessionData()

        Dim con As New MySqlConnection(connectionString)
        Dim com As String = "SELECT * FROM `acession_tbl`"
        Dim adap As New MySqlDataAdapter(com, con)
        Dim ds As New DataSet

        adap.Fill(ds, "INFO")
        Datagridview1.DataSource = ds.Tables("INFO")

        Datagridview1.EnableHeadersVisualStyles = False
        Datagridview1.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(207, 58, 109)
        Datagridview1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White

        Datagridview1.Columns("ID").Visible = False

        shelfsu()


    End Sub

    Private Sub Accession_Shown(sender As Object, e As EventArgs) Handles MyBase.Shown

        DataGridView1.ClearSelection()

    End Sub

    Private Sub Acession_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        RefreshAccessionData()

        Datagridview1.EnableHeadersVisualStyles = False
        Datagridview1.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(207, 58, 109)
        Datagridview1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White

        txtaccessionid.Enabled = False
        txtbooktitle.Enabled = False
        txtisbn.Enabled = False
        txtbarcodes.Enabled = False
        txttransactionno.Enabled = False
        txtsuppliername.Enabled = False

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

    Private Sub btntransaction_Click(sender As Object, e As EventArgs) Handles btntransaction.Click

        TransactionNumber.ShowDialog()
        TransactionNumber.ludtransaksyun()

    End Sub

    Private Sub btnadd_Click(sender As Object, e As EventArgs) Handles btnadd.Click

        Dim con As New MySqlConnection(connectionString)

        Try
            con.Open()


            If String.IsNullOrWhiteSpace(txttransactionno.Text) OrElse String.IsNullOrWhiteSpace(txtaccessionid.Text) OrElse cbshelf.SelectedIndex = -1 Then
                MessageBox.Show("Please fill all required fields.", "Required Fields", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If


            Dim kowm As String = "SELECT COUNT(*) FROM `acession_tbl` WHERE TransactionNo = @TransactionNo AND AccessionID IN (@AccessionIDs)" ' Binago para i-check ang AccessionID duplicates sa loob ng TransactionNo
            Dim gogo As New MySqlCommand(kowm, con)
            gogo.Parameters.AddWithValue("@TransactionNo", txttransactionno.Text)


            Dim statusValue As String = If(rbborrowable.Checked, "Available", "For In-Library Use Only")
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
                        comsu.Parameters.AddWithValue("@BookTitle", txtbooktitle.Text)
                        comsu.Parameters.AddWithValue("@Shelf", cbshelf.Text)
                        comsu.Parameters.AddWithValue("@SupplierName", txtsuppliername.Text)

                        comsu.Parameters.AddWithValue("@Status", statusValue)
                        comsu.ExecuteNonQuery()
                    End Using
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

        If DataGridView1.SelectedRows.Count = 0 Then
            MessageBox.Show("Please select a record from the table to edit.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If

        Dim selectedRow As DataGridViewRow = DataGridView1.SelectedRows(0)
        Dim currentStatus As String = selectedRow.Cells("Status").Value.ToString().Trim()


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
        Dim acsss As String = selectedRow.Cells("AccessionID").Value.ToString().Trim()

        Dim newAccessionID As String = txtaccessionid.Text.Trim()
        Dim statusValue As String = If(rbborrowable.Checked, "Available", "For In-Library Use Only")

        Try
            con.Open()


            If newAccessionID <> acsss Then
                Dim checkAccSql As String = "SELECT COUNT(*) FROM `acession_tbl` WHERE AccessionID = @AccessionID AND ID <> @ID"
                Using checkCmd As New MySqlCommand(checkAccSql, con)
                    checkCmd.Parameters.AddWithValue("@AccessionID", newAccessionID)
                    checkCmd.Parameters.AddWithValue("@ID", ID)
                    If CInt(checkCmd.ExecuteScalar()) > 0 Then
                        MessageBox.Show("Accession ID '" & newAccessionID & "' already exists for another record. Update aborted.", "Duplicate Accession ID", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                        Return
                    End If
                End Using
            End If


            Dim coms As String = "SELECT COUNT(*) FROM `borrowing_tbl` WHERE AccessionID = @accessionID"
            Dim comsuu As New MySqlCommand(coms, con)
            comsuu.Parameters.AddWithValue("@accessionID", acsss)
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

            Using updateCmd As New MySqlCommand(updt, con)
                updateCmd.Parameters.AddWithValue("@TransactionNo", txttransactionno.Text)
                updateCmd.Parameters.AddWithValue("@AccessionID", newAccessionID)
                updateCmd.Parameters.AddWithValue("@ISBN", If(String.IsNullOrWhiteSpace(txtisbn.Text), CType(DBNull.Value, Object), txtisbn.Text))
                updateCmd.Parameters.AddWithValue("@Barcode", If(String.IsNullOrWhiteSpace(txtbarcodes.Text), CType(DBNull.Value, Object), txtbarcodes.Text))
                updateCmd.Parameters.AddWithValue("@BookTitle", txtbooktitle.Text)
                updateCmd.Parameters.AddWithValue("@Shelf", cbshelf.Text)
                updateCmd.Parameters.AddWithValue("@SupplierName", txtsuppliername.Text)
                updateCmd.Parameters.AddWithValue("@Status", statusValue)
                updateCmd.Parameters.AddWithValue("@ID", ID)

                updateCmd.ExecuteNonQuery()
            End Using

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

    Private Sub btndelete_Click(sender As Object, e As EventArgs) Handles btndelete.Click

        If DataGridView1.SelectedRows.Count > 0 Then

            Dim selectedRow As DataGridViewRow = DataGridView1.SelectedRows(0)
            Dim statss As String = selectedRow.Cells("Status").Value.ToString().Trim()


            Dim ristrik As New List(Of String) From {"Pending", "Lost", "Damage"}

            If ristrik.Contains(statss, StringComparer.OrdinalIgnoreCase) Then
                MessageBox.Show($"Cannot delete this accession record. The current status is '{statss}'.", "Deletion Restricted", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If



            Dim dialogResult As DialogResult = MessageBox.Show("Are you sure you want to delete this accession record?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)

            If dialogResult = DialogResult.Yes Then

                Dim con As New MySqlConnection(connectionString)



                Dim accessionID As String = selectedRow.Cells("AccessionID").Value.ToString().Trim()
                Dim transactionNo As String = selectedRow.Cells("TransactionNo").Value.ToString().Trim()


                Dim ID As Integer = CInt(selectedRow.Cells("ID").Value)

                Try
                    con.Open()

                    Dim coms As String = "SELECT COUNT(*) FROM `borrowing_tbl` WHERE AccessionID = @accessionID"
                    Dim comsuu As New MySqlCommand(coms, con)
                    comsuu.Parameters.AddWithValue("@accessionID", accessionID)
                    Dim isBorrowed As Integer = CInt(comsuu.ExecuteScalar())

                    If isBorrowed > 0 Then
                        MessageBox.Show("Cannot delete this accession record. It is currently being borrowed.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        Return
                    End If


                    Dim koms As String = "DELETE FROM `acession_tbl` WHERE `ID` = @id"
                    Dim komsi As New MySqlCommand(koms, con)
                    komsi.Parameters.AddWithValue("@id", ID)
                    komsi.ExecuteNonQuery()

                    MsgBox("Accession record deleted successfully.", vbInformation)


                    Acession_Load(sender, e)
                    clearlahat()

                Catch ex As Exception
                    MsgBox("Error deleting record: " & ex.Message, vbCritical)
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

    Private Sub btndeleteall_Click(sender As Object, e As EventArgs) Handles btndeleteall.Click

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


                    Dim coms As String = "DELETE FROM `acession_tbl`"
                    Dim comsuus As New MySqlCommand(coms, con)
                    comsuus.ExecuteNonQuery()


                    Dim resett As String = "ALTER TABLE `acession_tbl` AUTO_INCREMENT = 1"
                    Dim incrementsu As New MySqlCommand(resett, con)
                    incrementsu.ExecuteNonQuery()

                    MessageBox.Show("All accession records have been successfully deleted.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)


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

    End Sub

    Private Sub DataGridView1_CellClick_1(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellClick

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



        ReserveCopies.ShowDialog()

    End Sub

    Private Sub CheckBox1_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox1.CheckedChanged

    End Sub

End Class