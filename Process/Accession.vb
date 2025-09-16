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
        rbavailable.Visible = False
        rbavailable.Checked = True
        Label4.Visible = False
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


            Dim kowm As String = "SELECT COUNT(*) FROM `acession_tbl` WHERE TransactionNo = @TransactionNo"
            Dim gogo As New MySqlCommand(kowm, con)
            gogo.Parameters.AddWithValue("@TransactionNo", txttransactionno.Text)
            Dim count As Integer = CInt(gogo.ExecuteScalar())

            If count > 0 Then
                MessageBox.Show("This Transaction Number has already been used. Cannot add records with this number again.", "Duplicate Transaction", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                Return
            End If


            Dim accessionIDs As String() = txtaccessionid.Text.Split(","c)


            For Each accessionID As String In accessionIDs
                Dim cleanedAccessionID As String = accessionID.Trim()


                If Not String.IsNullOrWhiteSpace(cleanedAccessionID) Then
                    Dim com As String = "INSERT INTO acession_tbl (`TransactionNo`, `AccessionID`, `ISBN`, `Barcode`, `BookTitle`, `Shelf`, `SupplierName`, `Status`) " &
                                    "VALUES (@TransactionNo, @AccessionID, @ISBN, @Barcode, @BookTitle, @Shelf, @SupplierName, @Status)"

                    Using comsu As New MySqlCommand(com, con)
                        comsu.Parameters.AddWithValue("@TransactionNo", txttransactionno.Text)
                        comsu.Parameters.AddWithValue("@AccessionID", cleanedAccessionID)
                        comsu.Parameters.AddWithValue("@ISBN", If(String.IsNullOrWhiteSpace(txtisbn.Text), CType(DBNull.Value, Object), txtisbn.Text))
                        comsu.Parameters.AddWithValue("@Barcode", If(String.IsNullOrWhiteSpace(txtbarcodes.Text), CType(DBNull.Value, Object), txtbarcodes.Text))
                        comsu.Parameters.AddWithValue("@BookTitle", txtbooktitle.Text)
                        comsu.Parameters.AddWithValue("@Shelf", cbshelf.Text)
                        comsu.Parameters.AddWithValue("@SupplierName", txtsuppliername.Text)
                        comsu.Parameters.AddWithValue("@Status", "Available")
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

    End Sub

    Private Sub btndelete_Click(sender As Object, e As EventArgs) Handles btndelete.Click

        If Datagridview1.SelectedRows.Count > 0 Then


            Dim dialogResult As DialogResult = MessageBox.Show("Are you sure you want to delete this accession record?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)

            If dialogResult = DialogResult.Yes Then

                Dim con As New MySqlConnection(connectionString)
                Dim selectedRow As DataGridViewRow = Datagridview1.SelectedRows(0)


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

    Private Sub Datagridview1_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles Datagridview1.CellClick

    End Sub

    Public Sub clearlahat()

        txtbarcodes.Text = ""
        txtisbn.Text = ""
        txtbooktitle.Text = ""
        txtaccessionid.Text = ""
        txtsuppliername.Text = ""
        txttransactionno.Text = ""

        cbshelf.SelectedIndex = -1

    End Sub



End Class