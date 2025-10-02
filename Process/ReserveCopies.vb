Imports MySql.Data.MySqlClient
Imports System.Data
Imports System.Drawing

Public Class ReserveCopies

    Private Const connectionString As String = "server=localhost;userid=root;database=laybsis_dbs;"

    Private Sub ReserveCopies_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        reserveload()


        DataGridView1.EnableHeadersVisualStyles = False
        DataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(207, 58, 109)
        DataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White
        DataGridView1.ClearSelection()
        DataGridView1.AllowUserToAddRows = False

    End Sub

    Public Sub reserveload()

        Dim con As New MySqlConnection(connectionString)

        Dim com As String = "SELECT ID, TransactionNo, AccessionID, ISBN, Barcode, BookTitle, Shelf, SupplierName, Status FROM `reservecopiess_tbl` WHERE Status = 'Reserved'"

        Dim adap As New MySqlDataAdapter(com, con)
        Dim ds As New DataSet

        Try
            adap.Fill(ds, "reserve_info")
            DataGridView1.DataSource = ds.Tables("reserve_info")


            If DataGridView1.Columns.Contains("ID") Then
                DataGridView1.Columns("ID").Visible = False
            End If

            DataGridView1.Columns("Shelf").Visible = False


        Catch ex As Exception
            MessageBox.Show("Error loading reserved copies: " & ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

        DataGridView1.ClearSelection()
        DataGridView1.Refresh()

    End Sub


    Private Sub Accession_Shown(sender As Object, e As EventArgs) Handles MyBase.Shown
        DataGridView1.ClearSelection()
    End Sub

    Private Sub ReserveCopies_KeyDown(sender As Object, e As KeyEventArgs) Handles MyBase.KeyDown

        If e.KeyCode = Keys.Escape Then
            Me.Close()
        End If

    End Sub

    Private Sub btnpush_Click(sender As Object, e As EventArgs) Handles btnpush.Click

        If DataGridView1.SelectedRows.Count = 0 Then
            MessageBox.Show("Please select a reserved copy to push back to Accession.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If


        Dim selectedRow As DataGridViewRow = DataGridView1.SelectedRows(0)
        Dim reserveID As Integer = CInt(selectedRow.Cells("ID").Value)
        Dim accID As String = selectedRow.Cells("AccessionID").Value.ToString()
        Dim tranNo As String = selectedRow.Cells("TransactionNo").Value.ToString()
        Dim isbn As Object = selectedRow.Cells("ISBN").Value
        Dim barcode As Object = selectedRow.Cells("Barcode").Value
        Dim bookTitle As String = selectedRow.Cells("BookTitle").Value.ToString()
        Dim shelf As String = selectedRow.Cells("Shelf").Value.ToString()
        Dim supplierName As String = selectedRow.Cells("SupplierName").Value.ToString()

        Dim defaultStatus As String = "Available"

        Dim dialogResult As DialogResult = MessageBox.Show($"Are you sure you want to push Accession ID {accID}?", "Confirm Push Back", MessageBoxButtons.YesNo, MessageBoxIcon.Question)

        If dialogResult = DialogResult.Yes Then

            Dim con As New MySqlConnection(connectionString)
            Dim transaction As MySqlTransaction = Nothing

            Try
                con.Open()
                transaction = con.BeginTransaction()


                Dim insertAcs As String = "INSERT INTO `acession_tbl` (`TransactionNo`, `AccessionID`, `ISBN`, `Barcode`, `BookTitle`, `Shelf`, `SupplierName`, `Status`) " &
                                      "VALUES (@TransactionNo, @AccessionID, @ISBN, @Barcode, @BookTitle, @Shelf, @SupplierName, @Status)"

                Dim ins As New MySqlCommand(insertAcs, con, transaction)
                ins.Parameters.AddWithValue("@TransactionNo", tranNo)
                ins.Parameters.AddWithValue("@AccessionID", accID)
                ins.Parameters.AddWithValue("@ISBN", If(IsDBNull(isbn), CType(DBNull.Value, Object), isbn))
                ins.Parameters.AddWithValue("@Barcode", If(IsDBNull(barcode), CType(DBNull.Value, Object), barcode))
                ins.Parameters.AddWithValue("@BookTitle", bookTitle)
                ins.Parameters.AddWithValue("@Shelf", shelf)
                ins.Parameters.AddWithValue("@SupplierName", supplierName)
                ins.Parameters.AddWithValue("@Status", defaultStatus)
                ins.ExecuteNonQuery()


                Dim dilit As String = "DELETE FROM `reservecopiess_tbl` WHERE ID = @ReserveID"
                Dim com As New MySqlCommand(dilit, con, transaction)
                com.Parameters.AddWithValue("@ReserveID", reserveID)
                com.ExecuteNonQuery()

                transaction.Commit()

                MessageBox.Show($"Accession ID {accID} has been successfully pushed back to the accession form.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
                reserveload()


                If Accession IsNot Nothing Then
                    Accession.RefreshAccessionData()

                    Accession.CheckBox1.Checked = False
                End If


            Catch ex As Exception
                If transaction IsNot Nothing Then
                    Try
                        transaction.Rollback()
                    Catch rollEx As Exception

                    End Try
                End If
                MessageBox.Show("Error pushing back copy: " & ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error)

            Finally
                If con.State = ConnectionState.Open Then
                    con.Close()
                End If
            End Try
        End If

    End Sub

    Private Sub txtsearch_TextChanged(sender As Object, e As EventArgs) Handles txtsearch.TextChanged

        Dim dt As DataTable = DirectCast(DataGridView1.DataSource, DataTable)
        If dt IsNot Nothing Then
            If txtsearch.Text.Trim() <> "" Then
                Dim filter As String = String.Format("BookTitle LIKE '*{0}*'", txtsearch.Text.Trim())
                dt.DefaultView.RowFilter = filter
            Else
                dt.DefaultView.RowFilter = ""
            End If
        End If

    End Sub
End Class