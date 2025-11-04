Imports MySql.Data.MySqlClient
Imports System.Data

Public Class Strand
    Private Sub Strand_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        DisablePaste_AllTextBoxes()
        TopMost = True
        Me.Refresh()
        LoadStrandData()
        GlobalVarsModule.AutoRefreshGrid(DataGridView1, "SELECT * FROM `strand_tbl`", 2000)
        AddHandler GlobalVarsModule.DatabaseUpdated, AddressOf OnDatabaseUpdated
    End Sub

    Private Sub LoadStrandData()
        Dim con As New MySqlConnection(GlobalVarsModule.connectionString)
        Dim com As String = "SELECT * FROM `strand_tbl`"
        Dim adap As New MySqlDataAdapter(com, con)
        Dim dt As New DataSet
        Try
            adap.Fill(dt, "INFO")
            DataGridView1.DataSource = dt.Tables("INFO")
            SetupStrandGridStyle()
        Catch ex As Exception
            MsgBox($"Error loading data: {ex.Message}", vbCritical)
        End Try
    End Sub

    Private Async Sub OnDatabaseUpdated()
        Try
            Await GlobalVarsModule.LoadToGridAsync(DataGridView1, "SELECT * FROM `strand_tbl`")
            SetupStrandGridStyle()
        Catch
        End Try
    End Sub

    Private Sub SetupStrandGridStyle()
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


    Private Sub Strand_Shown(sender As Object, e As EventArgs) Handles MyBase.Shown

        DataGridView1.ClearSelection()

    End Sub

    Private Sub Strand_FormClosed(sender As Object, e As FormClosedEventArgs) Handles Me.FormClosed

        For Each form In Application.OpenForms
            If TypeOf form Is MainForm Then
                Dim load = DirectCast(form, MainForm)
                load.loadsu()
            End If
        Next

        Dim activeMain As MainForm = GlobalVarsModule.ActiveMainForm
        If activeMain Is Nothing OrElse activeMain.IsDisposed Then
            activeMain = New MainForm()
            GlobalVarsModule.ActiveMainForm = activeMain
            activeMain.Show()
        End If

        activeMain.MaintenanceToolStripMenuItem.ShowDropDown()
        activeMain.MaintenanceToolStripMenuItem.ForeColor = Color.Gray
        txtstrand.Text = ""
        txtdescription.Text = ""

    End Sub

    Private Sub btnadd_Click(sender As Object, e As EventArgs) Handles btnadd.Click

        Dim con As New MySqlConnection(GlobalVarsModule.connectionString)
        Dim strandd As String = txtstrand.Text.Trim()
        Dim des As String = txtdescription.Text.Trim()
        Dim newID As Integer = 0

        If String.IsNullOrWhiteSpace(strandd) OrElse String.IsNullOrWhiteSpace(des) Then
            MsgBox("Please fill in the required fields.", vbExclamation, "Missing Information")
            Exit Sub
        End If

        If strandd.Length < 3 Then
            MsgBox("Strand Code must be 3 characters or more.", vbExclamation, "Input Error: Strand")
            Exit Sub
        End If

        If Not System.Text.RegularExpressions.Regex.IsMatch(strandd, "^[A-Z]+$") Then
            MsgBox("Strand Code must contain only uppercase letters (e.g., STEM, ABM, GAS).", vbExclamation, "Invalid Strand Code Format")
            Exit Sub
        End If

        If des.Length < 10 Then
            MsgBox("Description must be at least 10 characters long.", vbExclamation, "Input Error: Description")
            Exit Sub
        End If



        Try
            con.Open()

            Dim coms As New MySqlCommand("SELECT COUNT(*) FROM `strand_tbl` WHERE `Strand` = @strand OR `Description` = @description", con)
            coms.Parameters.AddWithValue("@strand", strandd)
            coms.Parameters.AddWithValue("@description", des)
            Dim count As Integer = Convert.ToInt32(coms.ExecuteScalar())

            If count > 0 Then
                MsgBox("This strand or description already exists.", vbExclamation, "Duplication not allowed.")
                Exit Sub
            End If

            Dim com As New MySqlCommand("INSERT INTO `strand_tbl`(`Strand`, `Description`) VALUES (@strand, @description); SELECT LAST_INSERT_ID();", con)
            com.Parameters.AddWithValue("@strand", strandd)
            com.Parameters.AddWithValue("@description", des)
            newID = Convert.ToInt32(com.ExecuteScalar())

            GlobalVarsModule.LogAudit(
            actionType:="ADD",
            formName:="STRAND FORM",
            description:=$"Added new strand: {strandd} ({des})",
            recordID:=newID.ToString()
        )

            For Each form In Application.OpenForms
                If TypeOf form Is AuditTrail Then
                    DirectCast(form, AuditTrail).refreshaudit()
                End If
            Next

            For Each form In Application.OpenForms
                If TypeOf form Is Borrower Then
                    Dim borrower = DirectCast(form, Borrower)
                    borrower.cbstrandd()
                End If
            Next

            For Each form In Application.OpenForms
                If TypeOf form Is Section Then
                    Dim strnd = DirectCast(form, Section)
                    strnd.cbstrandsu()
                End If
            Next

            MsgBox("Strand added successfully", vbInformation)
            Strand_Load(sender, e)

        Catch ex As Exception
            MsgBox(ex.Message, vbCritical)
        Finally
            If con.State = ConnectionState.Open Then con.Close()
            txtstrand.Clear()
            txtdescription.Text = ""
        End Try

    End Sub

    Private Sub btnedit_Click(sender As Object, e As EventArgs) Handles btnedit.Click

        If DataGridView1.SelectedRows.Count > 0 Then

            Dim con As New MySqlConnection(GlobalVarsModule.connectionString)
            Dim selectedRow As DataGridViewRow = DataGridView1.SelectedRows(0)
            Dim ID As Integer = CInt(selectedRow.Cells("ID").Value)

            Dim oldStrand As String = selectedRow.Cells("Strand").Value.ToString().Trim()
            Dim oldDescription As String = selectedRow.Cells("Description").Value.ToString().Trim()
            Dim newStrand As String = txtstrand.Text.Trim()
            Dim newDescription As String = txtdescription.Text.Trim()

            If String.IsNullOrWhiteSpace(newStrand) OrElse String.IsNullOrWhiteSpace(newDescription) Then
                MsgBox("Please fill in the required fields.", vbExclamation, "Missing Information")
                Exit Sub
            End If

            If newStrand.Length < 3 Then
                MsgBox("Strand Code must be 3 characters or more.", vbExclamation, "Input Error: Strand")
                Exit Sub
            End If

            If Not System.Text.RegularExpressions.Regex.IsMatch(newStrand, "^[A-Z]+$") Then
                MsgBox("Strand Code must contain only uppercase letters (e.g., STEM, ABM, GAS).", vbExclamation, "Invalid Strand Code Format")
                Exit Sub
            End If

            If newDescription.Length < 10 Then
                MsgBox("Description must be at least 10 characters long.", vbExclamation, "Input Error: Description")
                Exit Sub
            End If


            If oldStrand.Equals(newStrand, StringComparison.OrdinalIgnoreCase) AndAlso oldDescription.Equals(newDescription, StringComparison.OrdinalIgnoreCase) Then
                MsgBox("No changes were made.", vbInformation)
                Exit Sub
            End If

            Try
                con.Open()

                Dim coms As New MySqlCommand("SELECT COUNT(*) FROM `strand_tbl` WHERE (`Strand` = @strand OR `Description` = @description) AND `ID` <> @id", con)
                coms.Parameters.AddWithValue("@strand", newStrand)
                coms.Parameters.AddWithValue("@description", newDescription)
                coms.Parameters.AddWithValue("@id", ID)

                Dim count As Integer = Convert.ToInt32(coms.ExecuteScalar())

                If count > 0 Then
                    MsgBox("This strand or description already exists.", vbExclamation, "Duplication not allowed.")
                    Exit Sub
                End If

                Dim com As New MySqlCommand("UPDATE `strand_tbl` SET `Strand`= @strand, `Description` = @description WHERE `ID` = @id", con)
                com.Parameters.AddWithValue("@strand", newStrand)
                com.Parameters.AddWithValue("@description", newDescription)
                com.Parameters.AddWithValue("@id", ID)
                com.ExecuteNonQuery()


                Dim comsus As New MySqlCommand("UPDATE `section_tbl` SET `Strand` = @newStrand WHERE `Strand` = @oldStrand", con)
                comsus.Parameters.AddWithValue("@newStrand", newStrand)
                comsus.Parameters.AddWithValue("@oldStrand", oldStrand)
                comsus.ExecuteNonQuery()

                Dim comsis As New MySqlCommand("UPDATE `borrower_tbl` SET `Strand` = @newStrand WHERE `Strand` = @oldStrand", con)
                comsis.Parameters.AddWithValue("@newStrand", newStrand)
                comsis.Parameters.AddWithValue("@oldStrand", oldStrand)
                comsis.ExecuteNonQuery()

                GlobalVarsModule.LogAudit(
                actionType:="UPDATE",
                formName:="STRAND FORM",
                description:=$"Updated strand ID {ID} from '{oldStrand} ({oldDescription})' to '{newStrand} ({newDescription})'",
                recordID:=ID.ToString(),
                oldValue:=$"{oldStrand}|{oldDescription}",
                newValue:=$"{newStrand}|{newDescription}"
            )

                For Each form In Application.OpenForms
                    If TypeOf form Is AuditTrail Then
                        DirectCast(form, AuditTrail).refreshaudit()
                    End If
                Next

                For Each form In Application.OpenForms
                    If TypeOf form Is Borrower Then
                        Dim borrower = DirectCast(form, Borrower)
                        borrower.cbstrandd()
                        borrower.refreshData()
                    End If

                    If TypeOf form Is Section Then
                        Dim strnd = DirectCast(form, Section)
                        strnd.cbstrandsu()
                        strnd.refreshsecs()
                    End If
                Next

                MsgBox("Updated successfully!", vbInformation)
                Strand_Load(sender, e)
                txtstrand.Clear()
                txtdescription.Text = ""
            Catch ex As Exception
                MsgBox(ex.Message, vbCritical)
            Finally
                If con.State = ConnectionState.Open Then con.Close()
            End Try
        Else
            MsgBox("Please select a row to edit.", vbExclamation)
        End If

    End Sub

    Private Sub btndelete_Click(sender As Object, e As EventArgs) Handles btndelete.Click

        If DataGridView1.SelectedRows.Count > 0 Then

            Dim dialogResult As DialogResult = MessageBox.Show("Are you sure you want to delete this strand?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)

            If dialogResult = DialogResult.Yes Then

                Dim con As New MySqlConnection(GlobalVarsModule.connectionString)
                Dim selectedRow As DataGridViewRow = DataGridView1.SelectedRows(0)
                Dim ID As Integer = CInt(selectedRow.Cells("ID").Value)
                Dim strandName As String = selectedRow.Cells("Strand").Value.ToString().Trim()
                Dim des As String = selectedRow.Cells("Description").Value.ToString().Trim()

                Try
                    con.Open()


                    Dim sectionCom As New MySqlCommand("SELECT COUNT(*) FROM `section_tbl` WHERE Strand = @strand", con)
                    sectionCom.Parameters.AddWithValue("@strand", strandName)
                    Dim sectionCount As Integer = CInt(sectionCom.ExecuteScalar())

                    If sectionCount > 0 Then
                        MessageBox.Show("Cannot delete this strand. It is assigned to " & sectionCount & " section(s).", "Information", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        Return
                    End If


                    Dim delete As New MySqlCommand("DELETE FROM `strand_tbl` WHERE `ID` = @id", con)
                    delete.Parameters.AddWithValue("@id", ID)
                    delete.ExecuteNonQuery()

                    GlobalVarsModule.LogAudit(
                        actionType:="DELETE",
                        formName:="STRAND FORM",
                        description:=$"Deleted strand: {strandName} ({des})",
                        recordID:=ID.ToString()
                    )

                    For Each form In Application.OpenForms
                        If TypeOf form Is AuditTrail Then
                            DirectCast(form, AuditTrail).refreshaudit()
                        End If
                    Next

                    For Each form In Application.OpenForms
                        If TypeOf form Is Borrower Then
                            DirectCast(form, Borrower).cbstrandd()
                        End If
                        If TypeOf form Is Section Then
                            DirectCast(form, Section).cbstrandsu()
                        End If
                    Next

                    MsgBox("Strand deleted successfully.", vbInformation)
                    Strand_Load(sender, e)
                    txtstrand.Clear()
                    txtdescription.Text = ""

                    Dim count As New MySqlCommand("SELECT COUNT(*) FROM `strand_tbl`", con)
                    Dim rowCount As Long = CLng(count.ExecuteScalar())

                    If rowCount = 0 Then
                        Dim reset As New MySqlCommand("ALTER TABLE `strand_tbl` AUTO_INCREMENT = 1", con)
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

    Private Sub DataGridView1_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellClick

        If e.RowIndex >= 0 Then

            Dim row = DataGridView1.Rows(e.RowIndex)
            txtstrand.Text = row.Cells("Strand").Value.ToString
            txtdescription.Text = row.Cells("Description").Value.ToString
        End If

    End Sub

    Private Sub txtsearch_TextChanged(sender As Object, e As EventArgs) Handles txtsearch.TextChanged


        Dim dt As DataTable = DirectCast(DataGridView1.DataSource, DataTable)
        If dt IsNot Nothing Then
            If txtsearch.Text.Trim() <> "" Then
                Dim filter As String = String.Format("Strand LIKE '*{0}*' OR Description LIKE '*{0}*'", txtsearch.Text.Trim())
                dt.DefaultView.RowFilter = filter
            Else
                dt.DefaultView.RowFilter = ""
            End If
        End If

    End Sub


    Private Sub txtstrand_TextChanged(sender As Object, e As EventArgs) Handles txtstrand.TextChanged

        Dim strandInput As String = txtstrand.Text.Trim().ToUpper()

        txtdescription.Text = ""

        Select Case strandInput
            Case "STEM"
                txtdescription.Text = "Science, Technology, Engineering, and Mathematics"
            Case "ABM"
                txtdescription.Text = "Accountancy, Business, and Management"
            Case "ICT"
                txtdescription.Text = "Information and Communications Technology"
            Case "HUMSS"
                txtdescription.Text = "Humanities and Social Sciences"
            Case "GAS"
                txtdescription.Text = "General Academic Strand"
            Case "TVL"
                txtdescription.Text = "Technical-Vocational-Livelihood"
            Case "SPORTS"
                txtdescription.Text = "Sports Track"
            Case "ARTS AND DESIGN"
                txtdescription.Text = "Arts and Design Track"
            Case Else

        End Select

        If txtstrand.Text <> txtstrand.Text.ToUpper() Then
            txtstrand.Text = txtstrand.Text.ToUpper()

            txtstrand.SelectionStart = txtstrand.Text.Length
        End If
    End Sub
    Private Sub txtstrand_KeyDown(sender As Object, e As KeyEventArgs) Handles txtstrand.KeyDown

        If e.Control AndAlso (e.KeyCode = Keys.V Or e.KeyCode = Keys.C Or e.KeyCode = Keys.X) Then
            e.SuppressKeyPress = True
        End If

    End Sub

    Private Sub txtstrand_KeyPress(sender As Object, e As KeyPressEventArgs) Handles txtstrand.KeyPress


        If Not Char.IsLetter(e.KeyChar) AndAlso Not Char.IsControl(e.KeyChar) AndAlso Not Char.IsWhiteSpace(e.KeyChar) Then
            e.Handled = True
        End If

    End Sub

    Private Sub txtsearch_KeyDown(sender As Object, e As KeyEventArgs) Handles txtsearch.KeyDown

        If e.Control AndAlso (e.KeyCode = Keys.V Or e.KeyCode = Keys.C Or e.KeyCode = Keys.X) Then
            e.SuppressKeyPress = True
        End If

    End Sub



    Private Sub Strand_KeyDown(sender As Object, e As KeyEventArgs) Handles MyBase.KeyDown
        If e.KeyCode = Keys.Escape Then
            Me.Close()
        End If
    End Sub

    Private Sub txtdescription_KeyDown(sender As Object, e As KeyEventArgs) Handles txtdescription.KeyDown

        If e.Control AndAlso (e.KeyCode = Keys.V Or e.KeyCode = Keys.C Or e.KeyCode = Keys.X) Then
            e.SuppressKeyPress = True
        End If

    End Sub

    Private Sub txtdescription_KeyPress(sender As Object, e As KeyPressEventArgs) Handles txtdescription.KeyPress


        If Not Char.IsLetter(e.KeyChar) AndAlso Not Char.IsControl(e.KeyChar) AndAlso Not Char.IsWhiteSpace(e.KeyChar) Then
            e.Handled = True
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