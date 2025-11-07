Imports MySql.Data.MySqlClient
Imports System.Data

Public Class Supplier
    Private Sub Supplier_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        DisablePaste_AllTextBoxes()
        TopMost = True
        Me.Refresh()
        LoadSupplierData()
        GlobalVarsModule.AutoRefreshGrid(DataGridView1, "SELECT * FROM `supplier_tbl`", 2000)
        AddHandler GlobalVarsModule.DatabaseUpdated, AddressOf OnDatabaseUpdated
    End Sub

    Private Sub LoadSupplierData()
        Dim con As New MySqlConnection(GlobalVarsModule.connectionString)
        Dim com As String = "SELECT * FROM `supplier_tbl`"
        Dim adp As New MySqlDataAdapter(com, con)
        Dim dt As New DataSet
        Try
            adp.Fill(dt, "INFO")
            DataGridView1.DataSource = dt.Tables("INFO")
            SetupSupplierGridStyle()
        Catch ex As Exception
            MsgBox($"Error loading data: {ex.Message}", vbCritical)
        End Try
    End Sub

    Private Async Sub OnDatabaseUpdated()
        Try
            Await GlobalVarsModule.LoadToGridAsync(DataGridView1, "SELECT * FROM `supplier_tbl`")
            SetupSupplierGridStyle()
        Catch
        End Try
    End Sub

    Private Sub SetupSupplierGridStyle()
        Try
            If DataGridView1.Columns.Contains("ID") Then
                DataGridView1.Columns("ID").Visible = False
            End If
            DataGridView1.ClearSelection()
            DataGridView1.CurrentCell = Nothing
            DataGridView1.EnableHeadersVisualStyles = False
            DataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(207, 58, 109)
            DataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White
        Catch
        End Try
    End Sub


    Private Sub Supplier_Shown(sender As Object, e As EventArgs) Handles MyBase.Shown

        DataGridView1.ClearSelection()

    End Sub
    Private Sub Supplier_FormClosed(sender As Object, e As FormClosedEventArgs) Handles Me.FormClosed

        For Each form In Application.OpenForms
            If TypeOf form Is MainForm Then
                Dim load = DirectCast(form, MainForm)
                load.loadsu()
            End If
        Next

        MainForm.MaintenanceToolStripMenuItem.ForeColor = Color.White

        clear()
    End Sub

    Public Sub clear()
        txtaddress.Text = ""
        txtcontact.Text = ""
        txtsupplier.Text = ""

    End Sub

    Private Sub btnadd_Click(sender As Object, e As EventArgs) Handles btnadd.Click

        Dim con As New MySqlConnection(GlobalVarsModule.connectionString)

        Dim supp As String = txtsupplier.Text.Trim
        Dim address As String = txtaddress.Text.Trim
        Dim contact As String = txtcontact.Text.Trim
        Dim newID As Integer = 0

        If String.IsNullOrWhiteSpace(supp) OrElse String.IsNullOrWhiteSpace(address) OrElse String.IsNullOrWhiteSpace(contact) Then
            MsgBox("Please fill in the required fields.", vbExclamation, "Missing Information")
            Exit Sub
        End If

        If supp.Length < 2 Then
            MsgBox("Supplier Name must be 2 characters or more.", vbExclamation, "Input Error")
            Exit Sub
        End If

        If contact.Length < 11 OrElse (contact.StartsWith("09") AndAlso contact.Length = 2) Then
            MsgBox("Contact Number must be a valid length (e.g., 11 digits).", vbExclamation, "Invalid Contact Number")
            Exit Sub
        End If

        If address.Length <= 5 Then
            MessageBox.Show("Address must be valid.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Try
            con.Open()

            Dim comsu As New MySqlCommand("SELECT COUNT(*) FROM `supplier_tbl` WHERE `SupplierName` = @supplier", con)
            comsu.Parameters.AddWithValue("@supplier", supp)

            Dim count = Convert.ToInt32(comsu.ExecuteScalar)
            If count > 0 Then
                MsgBox("Supplier name already exists.", vbExclamation, "Warning")
                Exit Sub
            End If

            Dim com As New MySqlCommand("INSERT INTO `supplier_tbl`(`SupplierName`, `Address`, `ContactNumber`) VALUES (@supplier, @address, @contact); SELECT LAST_INSERT_ID();", con)

            com.Parameters.AddWithValue("@supplier", supp)
            com.Parameters.AddWithValue("@address", address)
            com.Parameters.AddWithValue("@contact", contact)
            newID = Convert.ToInt32(com.ExecuteScalar())

            GlobalVarsModule.LogAudit(
            actionType:="ADD",
            formName:="SUPPLIER FORM",
            description:=$"Added new supplier: {supp}",
            recordID:=newID.ToString()
        )

            For Each form In Application.OpenForms
                If TypeOf form Is AuditTrail Then
                    DirectCast(form, AuditTrail).refreshaudit()
                End If
            Next

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
            If con.State = ConnectionState.Open Then con.Close()
            clear()
        End Try

    End Sub

    Private Sub btnedit_Click(sender As Object, e As EventArgs) Handles btnedit.Click

        If DataGridView1.SelectedRows.Count > 0 Then

            Dim con As New MySqlConnection(GlobalVarsModule.connectionString)
            Dim selectedRow = DataGridView1.SelectedRows(0)

            Dim ID As Integer = CInt(selectedRow.Cells("ID").Value)

            Dim oldSupp As String = selectedRow.Cells("SupplierName").Value.ToString().Trim()
            Dim oldAddress As String = selectedRow.Cells("Address").Value.ToString().Trim()
            Dim oldContact As String = selectedRow.Cells("ContactNumber").Value.ToString().Trim()

            Dim newSupp As String = txtsupplier.Text.Trim
            Dim newAddress As String = txtaddress.Text.Trim
            Dim newContact As String = txtcontact.Text.Trim

            If String.IsNullOrWhiteSpace(newSupp) OrElse String.IsNullOrWhiteSpace(newAddress) OrElse String.IsNullOrWhiteSpace(newContact) Then
                MsgBox("Please fill in the required fields.", vbExclamation, "Missing Information")
                Exit Sub
            End If


            If newSupp.Length < 2 Then
                MsgBox("Supplier Name must be 2 characters or more.", vbExclamation, "Input Error")
                Exit Sub
            End If

            If newContact.Length < 11 OrElse (newContact.StartsWith("09") AndAlso newContact.Length = 2) Then
                MsgBox("Contact Number must be a valid length (e.g., 11 digits).", vbExclamation, "Invalid Contact Number")
                Exit Sub
            End If


            If newAddress.Length <= 5 Then
                MessageBox.Show("Address must be valid.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If

            If oldSupp.Equals(newSupp, StringComparison.OrdinalIgnoreCase) AndAlso
             oldAddress.Equals(newAddress, StringComparison.OrdinalIgnoreCase) AndAlso
             oldContact.Equals(newContact, StringComparison.OrdinalIgnoreCase) Then

                MsgBox("No changes were made.", vbInformation)
                Exit Sub
            End If

            Try
                con.Open()

                Dim comsu As New MySqlCommand("SELECT COUNT(*) FROM `supplier_tbl` WHERE `SupplierName` = @supplier AND `ID` <> @id", con)
                comsu.Parameters.AddWithValue("@supplier", newSupp)
                comsu.Parameters.AddWithValue("@id", ID)
                Dim count = Convert.ToInt32(comsu.ExecuteScalar)
                If count > 0 Then
                    MsgBox("Supplier name already exists.", vbExclamation, "Warning")
                    Exit Sub
                End If

                Dim com As New MySqlCommand("UPDATE `supplier_tbl` SET `SupplierName` = @supplier, `Address` = @address, `ContactNumber` = @contact WHERE `ID` = @id", con)
                com.Parameters.AddWithValue("@supplier", newSupp)
                com.Parameters.AddWithValue("@address", newAddress)
                com.Parameters.AddWithValue("@contact", newContact)
                com.Parameters.AddWithValue("@id", ID)
                com.ExecuteNonQuery()

                Dim comss As New MySqlCommand("UPDATE `acquisition_tbl` SET `SupplierName` = @newSupplier WHERE `SupplierName` = @oldSupplier", con)
                comss.Parameters.AddWithValue("@newSupplier", newSupp)
                comss.Parameters.AddWithValue("@oldSupplier", oldSupp)
                comss.ExecuteNonQuery()

                Dim comsuss As New MySqlCommand("UPDATE `acession_tbl` SET `SupplierName` = @newSupplier WHERE `SupplierName` = @oldSupplier", con)
                comsuss.Parameters.AddWithValue("@newSupplier", newSupp)
                comsuss.Parameters.AddWithValue("@oldSupplier", oldSupp)
                comsuss.ExecuteNonQuery()

                GlobalVarsModule.LogAudit(
                actionType:="UPDATE",
                formName:="SUPPLIER FORM",
                description:=$"Updated supplier ID {ID} from '{oldSupp}' to '{newSupp}'",
                recordID:=ID.ToString(),
                oldValue:=$"{oldSupp}|{oldAddress}|{oldContact}",
                newValue:=$"{newSupp}|{newAddress}|{newContact}"
            )

                For Each form In Application.OpenForms
                    If TypeOf form Is AuditTrail Then
                        DirectCast(form, AuditTrail).refreshaudit()
                    End If
                Next

                For Each form In Application.OpenForms
                    If TypeOf form Is Acquisition Then
                        Dim acq = DirectCast(form, Acquisition)

                        acq.cbsupplierr()
                        acq.refreshData()
                    End If
                Next

                For Each form In Application.OpenForms
                    If TypeOf form Is Accession Then
                        Dim acs = DirectCast(form, Accession)
                        acs.RefreshAccessionData()
                    End If
                Next

                For Each form In Application.OpenForms
                    If TypeOf form Is MainForm Then
                        Dim load = DirectCast(form, MainForm)
                        load.loadsu()
                    End If

                Next
                MsgBox("Updated successfully", vbInformation)
                Supplier_Load(sender, e)
            Catch ex As Exception
                MsgBox(ex.Message, vbCritical)
            Finally
                If con.State = ConnectionState.Open Then
                    con.Close()
                End If
                clear()
            End Try
        Else
            MsgBox("Please select a row before edit.", vbExclamation)
        End If

    End Sub

    Private Sub btndelete_Click(sender As Object, e As EventArgs) Handles btndelete.Click

        If DataGridView1.SelectedRows.Count > 0 Then

            Dim dialogResult = MessageBox.Show("Are you sure you want to delete this supplier?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)

            If dialogResult = DialogResult.Yes Then

                Dim con As New MySqlConnection(GlobalVarsModule.connectionString)
                Dim selectedRow = DataGridView1.SelectedRows(0)
                Dim ID As Integer = selectedRow.Cells("ID").Value
                Dim supplierName = selectedRow.Cells("SupplierName").Value.ToString.Trim
                Dim address = selectedRow.Cells("Address").Value.ToString.Trim
                Dim contact = selectedRow.Cells("ContactNumber").Value.ToString.Trim

                Try
                    con.Open()


                    Dim acquisitionCom As New MySqlCommand("SELECT COUNT(*) FROM `acquisition_tbl` WHERE SupplierName = @supplier", con)
                    acquisitionCom.Parameters.AddWithValue("@supplier", supplierName)
                    Dim acquisitionCount As Integer = acquisitionCom.ExecuteScalar()

                    If acquisitionCount > 0 Then
                        MessageBox.Show("Cannot delete this supplier. They are assigned to " & acquisitionCount & " acquisition(s).", "Information", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        Return
                    End If

                    Dim delete As New MySqlCommand("DELETE FROM `supplier_tbl` WHERE `ID` = @id", con)
                    delete.Parameters.AddWithValue("@id", ID)
                    delete.ExecuteNonQuery()

                    GlobalVarsModule.LogAudit(
                        actionType:="DELETE",
                        formName:="SUPPLIER FORM",
                        description:=$"Deleted supplier: {supplierName}",
                        recordID:=ID.ToString()
                    )

                    For Each form In Application.OpenForms
                        If TypeOf form Is AuditTrail Then
                            DirectCast(form, AuditTrail).refreshaudit()
                        End If
                    Next

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
                    Dim rowCount As Long = count.ExecuteScalar()

                    If rowCount = 0 Then
                        Dim reset As New MySqlCommand("ALTER TABLE `supplier_tbl` AUTO_INCREMENT = 1", con)
                        reset.ExecuteNonQuery()
                    End If

                Catch ex As Exception
                    MsgBox(ex.Message, vbCritical)
                Finally
                    If con.State = ConnectionState.Open Then con.Close()
                End Try
            End If
        End If
    End Sub

    Private Sub txtsearch_TextChanged(sender As Object, e As EventArgs) Handles txtsearch.TextChanged

        HandleAutoRefreshPause(DataGridView1, txtsearch)

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

    Private Sub txtaddress_KeyDown(sender As Object, e As KeyEventArgs) Handles txtaddress.KeyDown

        If e.Control AndAlso (e.KeyCode = Keys.V Or e.KeyCode = Keys.C Or e.KeyCode = Keys.X) Then
            e.SuppressKeyPress = True
        End If

    End Sub

    Private Sub txtaddress_KeyPress(sender As Object, e As KeyPressEventArgs) Handles txtaddress.KeyPress


        If Char.IsLetterOrDigit(e.KeyChar) OrElse Char.IsControl(e.KeyChar) OrElse Char.IsWhiteSpace(e.KeyChar) Then
            e.Handled = False
            Return
        End If


        Select Case e.KeyChar
            Case "#"c, "."c, ","c, "-"c, "/"c, "'"c, ":"c, "("c, ")"c
                e.Handled = False
                Return
        End Select


        e.Handled = True

    End Sub


    Private Sub txtaddress_Validating(sender As Object, e As System.ComponentModel.CancelEventArgs) Handles txtaddress.Validating

        Dim AddressText As String = txtaddress.Text.Trim()
        Dim AddressPattern As String = "^(?=.*[a-zA-Z0-9])(?!.*[#.,/\-:'()\s]{2,})[a-zA-Z0-9\s#.,/\-:'()]+$"

        If String.IsNullOrEmpty(AddressText) Then
            e.Cancel = False
            Return
        End If

        If Not System.Text.RegularExpressions.Regex.IsMatch(AddressText, AddressPattern, System.Text.RegularExpressions.RegexOptions.IgnoreCase) Then

            MessageBox.Show("Invalid address format.", "Validation Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning)

            e.Cancel = True
        Else
            e.Cancel = False
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

        Dim oreyjeynal = txtcontact.Text

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

    Private Sub Supplier_KeyDown(sender As Object, e As KeyEventArgs) Handles MyBase.KeyDown
        If e.KeyCode = Keys.Escape Then
            Me.Close()
        End If
    End Sub

    Private Sub DataGridView1_CellClick_1(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellClick

        If e.RowIndex >= 0 Then

            Dim row = DataGridView1.Rows(e.RowIndex)
            txtsupplier.Text = row.Cells("SupplierName").Value.ToString
            txtaddress.Text = row.Cells("Address").Value.ToString
            txtcontact.Text = row.Cells("ContactNumber").Value.ToString

        End If

    End Sub


    Private Sub txtsupplier_KeyPress(sender As Object, e As KeyPressEventArgs) Handles txtsupplier.KeyPress

        If Not Char.IsLetter(e.KeyChar) AndAlso Not Char.IsControl(e.KeyChar) AndAlso Not Char.IsWhiteSpace(e.KeyChar) Then
            e.Handled = True
        End If
    End Sub



    Private Sub txtsupplier_TextChanged(sender As Object, e As EventArgs) Handles txtsupplier.TextChanged
        Dim InputText As String = txtsupplier.Text
        Dim FilteredText As New System.Text.StringBuilder()


        For Each c As Char In InputText
            If Char.IsLetter(c) OrElse Char.IsWhiteSpace(c) Then
                FilteredText.Append(c)
            End If
        Next


        If FilteredText.ToString() <> InputText Then
            Dim CursorPosition As Integer = txtsupplier.SelectionStart
            txtsupplier.Text = FilteredText.ToString()


            If CursorPosition > 0 Then

                txtsupplier.SelectionStart = Math.Min(CursorPosition - 1, txtsupplier.Text.Length)
            Else
                txtsupplier.SelectionStart = 0
            End If
        End If
    End Sub



    Private Sub txtsupplier_Validating(sender As Object, e As System.ComponentModel.CancelEventArgs) Handles txtsupplier.Validating

        Dim SupplierName As String = txtsupplier.Text.Trim()


        If String.IsNullOrEmpty(SupplierName) Then
            e.Cancel = False
            Return
        End If


        Dim Pattern As String = "^[A-Za-z]+(\s[A-Za-z]+)*$"

        If Not System.Text.RegularExpressions.Regex.IsMatch(SupplierName, Pattern) Then
            MessageBox.Show("Invalid supplier name format.", "Input Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning)

            e.Cancel = True
        Else
            e.Cancel = False
        End If
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

    Private Sub DisablePaste_AllTextBoxes()
        For Each ctrl As Control In Me.Controls
            AddHandlerToTextBoxes_NoPaste(ctrl)
        Next
    End Sub

    Private Sub AddHandlerToTextBoxes_NoPaste(parent As Control)
        For Each ctrl As Control In parent.Controls
            If TypeOf ctrl Is TextBox Then
                Dim tb As TextBox = CType(ctrl, TextBox)

                tb.ContextMenuStrip = New ContextMenuStrip()

                AddHandler tb.KeyDown, AddressOf BlockPasteKey
                AddHandler tb.MouseUp, AddressOf BlockRightClick

            End If

            If ctrl.HasChildren Then
                AddHandlerToTextBoxes_NoPaste(ctrl)
            End If
        Next

    End Sub


    Private Sub BlockPasteKey(sender As Object, e As KeyEventArgs)

        If (e.Control AndAlso e.KeyCode = Keys.V) OrElse (e.Shift AndAlso e.KeyCode = Keys.Insert) Then
            e.SuppressKeyPress = True
        End If

    End Sub

    Private Sub BlockRightClick(sender As Object, e As MouseEventArgs)

        If e.Button = MouseButtons.Right Then

            Dim tb As TextBox = TryCast(sender, TextBox)
            If tb IsNot Nothing Then
                tb.ContextMenuStrip = New ContextMenuStrip()
            End If
        End If

    End Sub


End Class