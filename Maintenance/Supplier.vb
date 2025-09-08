Imports MySql.Data.MySqlClient

Public Class Supplier
    Private Sub Supplier_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        TopMost = True

        Dim con As New MySqlConnection(connectionString)
        Dim com As String = "SELECT * FROM `supplier_tbl`"
        Dim adp As New MySqlDataAdapter(com, con)
        Dim dt As New DataSet

        adp.Fill(dt, "INFO")
        DataGridView1.DataSource = dt.Tables("INFO")

        DataGridView1.EnableHeadersVisualStyles = False
        DataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(207, 58, 109)
        DataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White

    End Sub

    Private Sub Supplier_FormClosed(sender As Object, e As FormClosedEventArgs) Handles Me.FormClosed

        MainForm.MaintenanceToolStripMenuItem.ShowDropDown()
        MainForm.MaintenanceToolStripMenuItem.ForeColor = Color.Gray

    End Sub

    Public Sub clear()
        txtaddress.Text = ""
        txtcontact.Text = ""
        txtsupplier.Text = ""

    End Sub

    Private Sub btnadd_Click(sender As Object, e As EventArgs) Handles btnadd.Click

        Dim con As New MySqlConnection(connectionString)

        Dim supp As String = txtsupplier.Text.Trim
        Dim address As String = txtaddress.Text.Trim
        Dim contact As String = txtcontact.Text.Trim

        If String.IsNullOrWhiteSpace(supp) OrElse String.IsNullOrWhiteSpace(address) OrElse String.IsNullOrWhiteSpace(contact) Then
            MsgBox("Please fill in the required fields.", vbExclamation, "Missing Information")
            Exit Sub
        End If

        Try
            con.Open()
            Dim com As New MySqlCommand("INSERT INTO `supplier_tbl`(`SupplierName`, `Address`, `ContactNumber`) VALUES (@supplier, @address, @contact)", con)

            com.Parameters.AddWithValue("@supplier", supp)
            com.Parameters.AddWithValue("@address", address)
            com.Parameters.AddWithValue("@contact", contact)
            com.ExecuteNonQuery()

            For Each form In Application.OpenForms
                If TypeOf form Is Acquisition Then
                    Dim acq = DirectCast(form, Acquisition)
                    acq.cbsupplierr()
                End If
            Next

            MsgBox("Supplier added successfully", vbInformation)
            Supplier_Load(sender, e)

        Catch ex As Exception
            MsgBox(ex.Message, vbCritical)
        Finally
            clear()
        End Try

    End Sub

    Private Sub btnedit_Click(sender As Object, e As EventArgs) Handles btnedit.Click

        If DataGridView1.SelectedRows.Count > 0 Then



            Dim con As New MySqlConnection(connectionString)

            Dim supp As String = txtsupplier.Text.Trim
            Dim address As String = txtaddress.Text.Trim
            Dim contact As String = txtcontact.Text.Trim

            Dim selectedRow As DataGridViewRow = DataGridView1.SelectedRows(0)
            Dim ID As Integer = CInt(selectedRow.Cells("ID").Value)

            If String.IsNullOrWhiteSpace(supp) OrElse String.IsNullOrWhiteSpace(address) OrElse String.IsNullOrWhiteSpace(contact) Then
                MsgBox("Please fill in the required fields.", vbExclamation, "Missing Information")
                Exit Sub
            End If

            Try
                con.Open()
                Dim com As New MySqlCommand("UPDATE `supplier_tbl` SET `SupplierName`= @supplier,`Address`= @address,`ContactNumber`= @contact  WHERE `ID` = @id", con)

                com.Parameters.AddWithValue("@supplier", supp)
                com.Parameters.AddWithValue("@address", address)
                com.Parameters.AddWithValue("@contact", contact)
                com.Parameters.AddWithValue("@id", ID)
                com.ExecuteNonQuery()

                For Each form In Application.OpenForms
                    If TypeOf form Is Acquisition Then
                        Dim acq = DirectCast(form, Acquisition)
                        acq.cbsupplierr()
                    End If
                Next

                MsgBox("Updated successfully", vbInformation)
                Supplier_Load(sender, e)

            Catch ex As Exception
                MsgBox(ex.Message, vbCritical)
            Finally
                clear()
            End Try
        Else
            MsgBox("Please select a row before edit.", vbExclamation)
        End If

    End Sub

    Private Sub btndelete_Click(sender As Object, e As EventArgs) Handles btndelete.Click

        If DataGridView1.SelectedRows.Count > 0 Then

            Dim dialogResult As DialogResult = MessageBox.Show("Are you sure you want to delete this supplier?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)

            If dialogResult = DialogResult.Yes Then

                Dim con As New MySqlConnection(connectionString)
                Dim selectedRow As DataGridViewRow = DataGridView1.SelectedRows(0)
                Dim ID As Integer = CInt(selectedRow.Cells("ID").Value)

                Try
                    con.Open()
                    Dim delete As New MySqlCommand("DELETE FROM `supplier_tbl` WHERE `ID` = @id", con)
                    delete.Parameters.AddWithValue("@id", ID)
                    delete.ExecuteNonQuery()

                    For Each form In Application.OpenForms
                        If TypeOf form Is Acquisition Then
                            Dim acq = DirectCast(form, Acquisition)
                            acq.cbsupplierr()
                        End If
                    Next

                    MsgBox("Supplier deleted successfully.", vbInformation)
                    Supplier_Load(sender, e)
                    clear()


                    Dim count As New MySqlCommand("SELECT COUNT(*) FROM `supplier_tbl`", con)
                    Dim rowCount As Long = CLng(count.ExecuteScalar())

                    If rowCount = 0 Then

                        Dim reset As New MySqlCommand("ALTER TABLE `supplier_tbl` AUTO_INCREMENT = 1", con)
                        reset.ExecuteNonQuery()

                    End If

                Catch ex As Exception
                    MsgBox(ex.Message, vbCritical)
                End Try
            End If

        End If
    End Sub

    Private Sub DataGridView1_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellClick

        If e.RowIndex >= 0 Then

            Dim row = DataGridView1.Rows(e.RowIndex)
            txtsupplier.Text = row.Cells("SupplierName").Value.ToString
            txtaddress.Text = row.Cells("Address").Value.ToString
            txtcontact.Text = row.Cells("ContactNumber").Value.ToString

        End If

    End Sub

    Private Sub txtsearch_TextChanged(sender As Object, e As EventArgs) Handles txtsearch.TextChanged


        Dim dt As DataTable = DirectCast(DataGridView1.DataSource, DataTable)
        If dt IsNot Nothing Then
            If txtsearch.Text.Trim() <> "" Then
                Dim filter As String = String.Format("SupplierName LIKE '*{0}*'", txtsearch.Text.Trim())
                dt.DefaultView.RowFilter = filter
            Else
                dt.DefaultView.RowFilter = ""
            End If
        End If

    End Sub

    Private Sub txtsupplier_KeyDown(sender As Object, e As KeyEventArgs) Handles txtsupplier.KeyDown

        If e.Control AndAlso (e.KeyCode = Keys.V Or e.KeyCode = Keys.C Or e.KeyCode = Keys.X) Then
            e.SuppressKeyPress = True
        End If

    End Sub

    Private Sub txtsupplier_KeyPress(sender As Object, e As KeyPressEventArgs) Handles txtsupplier.KeyPress

        If Not Char.IsLetter(e.KeyChar) AndAlso Not Char.IsControl(e.KeyChar) AndAlso Not Char.IsWhiteSpace(e.KeyChar) Then
            e.Handled = True
        End If

    End Sub

    Private Sub txtaddress_KeyDown(sender As Object, e As KeyEventArgs) Handles txtaddress.KeyDown

        If e.Control AndAlso (e.KeyCode = Keys.V Or e.KeyCode = Keys.C Or e.KeyCode = Keys.X) Then
            e.SuppressKeyPress = True
        End If

    End Sub

    Private Sub txtaddress_KeyPress(sender As Object, e As KeyPressEventArgs) Handles txtaddress.KeyPress


        If Not Char.IsLetterOrDigit(e.KeyChar) AndAlso Not Char.IsControl(e.KeyChar) AndAlso Not Char.IsWhiteSpace(e.KeyChar) AndAlso e.KeyChar <> "#" Then
            e.Handled = True
        End If

    End Sub

    Private Sub txtcontact_KeyDown(sender As Object, e As KeyEventArgs) Handles txtcontact.KeyDown

        If e.Control AndAlso (e.KeyCode = Keys.V Or e.KeyCode = Keys.C Or e.KeyCode = Keys.X) Then
            e.SuppressKeyPress = True
        End If

        If e.KeyCode = Keys.Back Then
            RemoveHandler txtcontact.TextChanged, AddressOf txtcontact_TextChanged
        End If

    End Sub

    Private Sub txtcontact_KeyPress(sender As Object, e As KeyPressEventArgs) Handles txtcontact.KeyPress

        If Not Char.IsDigit(e.KeyChar) And Not Char.IsControl(e.KeyChar) Then
            e.Handled = True
        End If

    End Sub

    Private Sub txtcontact_TextChanged(sender As Object, e As EventArgs) Handles txtcontact.TextChanged

        Dim oreyjeynal As String = txtcontact.Text

        If String.IsNullOrEmpty(oreyjeynal) Then
            Return
        End If

        If oreyjeynal.StartsWith("09") Then

        Else

            If oreyjeynal.Length > 0 Then
                txtcontact.Clear()
                txtcontact.Text = "09"
                txtcontact.SelectionStart = 2
            End If
        End If

    End Sub

    Private Sub txtsearch_KeyDown(sender As Object, e As KeyEventArgs) Handles txtsearch.KeyDown

        If e.Control AndAlso (e.KeyCode = Keys.V Or e.KeyCode = Keys.C Or e.KeyCode = Keys.X) Then
            e.SuppressKeyPress = True
        End If

    End Sub

    Private Sub txtcontact_KeyUp(sender As Object, e As KeyEventArgs) Handles txtcontact.KeyUp

        If e.KeyCode = Keys.Back Then
            AddHandler txtcontact.TextChanged, AddressOf txtcontact_TextChanged
        End If

    End Sub
End Class