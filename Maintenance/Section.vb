Imports MySql.Data.MySqlClient

Public Class Section
    Private Sub Section_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        TopMost = True

        Dim con As New MySqlConnection(connectionString)
        Dim com As String = "SELECT * FROM `section_tbl`"
        Dim adap As New MySqlDataAdapter(com, con)
        Dim dt As New DataSet

        adap.Fill(dt, "INFO")
        DataGridView1.DataSource = dt.Tables("INFO")

        DataGridView1.EnableHeadersVisualStyles = False
        DataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(207, 58, 109)
        DataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White

        cbdeptss()


        cbstrand.Visible = False
        cbgrade.Enabled = False
        txtsection.Enabled = False

    End Sub

    Private Sub Section_FormClosed(sender As Object, e As FormClosedEventArgs) Handles Me.FormClosed

        MainForm.MaintenanceToolStripMenuItem.ShowDropDown()
        MainForm.MaintenanceToolStripMenuItem.ForeColor = Color.Gray

    End Sub
    Private Sub btnadd_Click(sender As Object, e As EventArgs) Handles btnadd.Click

        Dim con As New MySqlConnection(connectionString)
        Dim dept As String = ""
        Dim grade As String = ""
        Dim secs As String = ""
        Dim strand As String = ""

        If cbdepartment.SelectedIndex <> -1 Then
            dept = cbdepartment.GetItemText(cbdepartment.SelectedItem)
        End If

        If cbgrade.SelectedIndex <> -1 Then
            grade = cbgrade.GetItemText(cbgrade.SelectedItem)
        End If

        If dept = "Junior High School" Then
            secs = txtsection.Text.Trim
            strand = ""
        ElseIf dept = "Senior High School" Then
            secs = ""
            If cbstrand.SelectedIndex <> -1 Then
                strand = cbstrand.GetItemText(cbstrand.SelectedItem)
            End If
        End If

        Try
            con.Open()
            Dim com As New MySqlCommand("INSERT INTO `section_tbl`(`Department`, `GradeLevel`, `Section`, `Strand`) VALUES (@dept, @grade, @section, @strand)", con)
            com.Parameters.AddWithValue("@dept", dept)
            com.Parameters.AddWithValue("@grade", grade)
            com.Parameters.AddWithValue("@section", If(secs = "", CType(DBNull.Value, Object), secs))
            com.Parameters.AddWithValue("@strand", If(strand = "", CType(DBNull.Value, Object), strand))
            com.ExecuteNonQuery()

            For Each form In Application.OpenForms
                If TypeOf form Is Borrower Then
                    Dim borrower = DirectCast(form, Borrower)
                    borrower.cbsecs()
                End If
            Next

            MsgBox("Section added successfully", vbInformation)
            Section_Load(sender, e)

            clearlahat()

        Catch ex As Exception
            MsgBox(ex.Message, vbCritical)
        End Try

    End Sub

    Private Sub btnedit_Click(sender As Object, e As EventArgs) Handles btnedit.Click

        If DataGridView1.SelectedRows.Count > 0 Then

            Dim con As New MySqlConnection(connectionString)
            Dim selectedRow As DataGridViewRow = DataGridView1.SelectedRows(0)
            Dim ID As Integer = CInt(selectedRow.Cells("ID").Value)

            Dim newDept As String = ""
            Dim newGrade As String = ""
            Dim newSecs As String = ""
            Dim newStrand As String = ""

            If cbdepartment.SelectedIndex <> -1 Then
                newDept = cbdepartment.GetItemText(cbdepartment.SelectedItem)
            End If

            If cbgrade.SelectedIndex <> -1 Then
                newGrade = cbgrade.GetItemText(cbgrade.SelectedItem)
            End If

            If newDept = "Junior High School" Then
                newSecs = txtsection.Text.Trim
                newStrand = ""
            ElseIf newDept = "Senior High School" Then
                newSecs = ""
                If cbstrand.SelectedIndex <> -1 Then
                    newStrand = cbstrand.GetItemText(cbstrand.SelectedItem)
                End If
            End If

            Try
                con.Open()
                Dim com As New MySqlCommand("UPDATE `section_tbl` SET `Department` = @dept, `GradeLevel` = @grade, `Section` = @section, `Strand` = @strand WHERE `ID` = @id", con)
                com.Parameters.AddWithValue("@dept", newDept)
                com.Parameters.AddWithValue("@grade", newGrade)
                com.Parameters.AddWithValue("@section", If(newSecs = "", CType(DBNull.Value, Object), newSecs))
                com.Parameters.AddWithValue("@strand", If(newStrand = "", CType(DBNull.Value, Object), newStrand))
                com.Parameters.AddWithValue("@id", ID)
                com.ExecuteNonQuery()

                For Each form In Application.OpenForms
                    If TypeOf form Is Borrower Then
                        Dim borrower = DirectCast(form, Borrower)
                        borrower.cbsecs()
                    End If
                Next

                MsgBox("Updated successfully!", vbInformation)
                Section_Load(sender, e)

                clearlahat()
            Catch ex As Exception
                MsgBox(ex.Message, vbCritical)
            End Try

        End If

    End Sub

    Private Sub btndelete_Click(sender As Object, e As EventArgs) Handles btndelete.Click

        If DataGridView1.SelectedRows.Count > 0 Then

            Dim dialogResult As DialogResult = MessageBox.Show("Are you sure you want to delete this section?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)

            If dialogResult = DialogResult.Yes Then

                Dim con As New MySqlConnection(connectionString)
                Dim selectedRow As DataGridViewRow = DataGridView1.SelectedRows(0)
                Dim ID As Integer = CInt(selectedRow.Cells("ID").Value)
                Dim sectionName As String = selectedRow.Cells("Section").Value.ToString().Trim()

                Try
                    con.Open()


                    Dim Borrower As New MySqlCommand("SELECT COUNT(*) FROM `borrower_tbl` WHERE Section = @section", con)
                    Borrower.Parameters.AddWithValue("@section", sectionName)
                    Dim borrowerCount As Integer = CInt(Borrower.ExecuteScalar())


                    If borrowerCount > 0 Then
                        MessageBox.Show("Cannot delete this section. It is already assigned to a borrower.", "information", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        Return
                    End If


                    Dim delete As New MySqlCommand("DELETE FROM `section_tbl` WHERE `ID` = @id", con)
                    delete.Parameters.AddWithValue("@id", ID)
                    delete.ExecuteNonQuery()

                    For Each form In Application.OpenForms
                        If TypeOf form Is Borrower Then
                            DirectCast(form, Borrower).cbsecs()
                        End If
                    Next

                    MsgBox("Section deleted successfully.", vbInformation)
                    Section_Load(sender, e)
                    clearlahat()

                    Dim count As New MySqlCommand("SELECT COUNT(*) FROM `section_tbl`", con)
                    Dim rowCount As Long = CLng(count.ExecuteScalar())

                    If rowCount = 0 Then
                        Dim reset As New MySqlCommand("ALTER TABLE `section_tbl` AUTO_INCREMENT = 1", con)
                        reset.ExecuteNonQuery()
                    End If

                Catch ex As Exception
                    MsgBox(ex.Message, vbCritical)
                Finally
                    If con.State = ConnectionState.Open Then
                        con.Close()
                    End If
                End Try
            End If
        End If
    End Sub

    Private Sub DataGridView1_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellClick
        If e.RowIndex >= 0 Then
            Dim row = DataGridView1.Rows(e.RowIndex)


            cbdepartment.Text = row.Cells("Department").Value.ToString
            cbgrade.Text = row.Cells("GradeLevel").Value.ToString

            Dim sectionValue As Object = row.Cells("Section").Value
            Dim strandValue As Object = row.Cells("Strand").Value


            If Not IsDBNull(sectionValue) AndAlso sectionValue.ToString <> "" Then
                txtsection.Visible = True
                cbstrand.Visible = False
                txtsection.Text = sectionValue.ToString
            ElseIf Not IsDBNull(strandValue) AndAlso strandValue.ToString <> "" Then
                cbstrand.Visible = True
                txtsection.Visible = False
                cbstrand.Text = strandValue.ToString
            End If
        End If
    End Sub

    Private Sub txtsearch_TextChanged(sender As Object, e As EventArgs) Handles txtsearch.TextChanged


        Dim dt As DataTable = DirectCast(DataGridView1.DataSource, DataTable)
        If dt IsNot Nothing Then
            If txtsearch.Text.Trim() <> "" Then
                Dim filter As String = String.Format("Section LIKE '*{0}*' OR Department LIKE '*{0}*'", txtsearch.Text.Trim())
                dt.DefaultView.RowFilter = filter
            Else
                dt.DefaultView.RowFilter = ""
            End If
        End If

    End Sub

    Private Sub txtsection_KeyDown(sender As Object, e As KeyEventArgs) Handles txtsection.KeyDown

        If e.Control AndAlso (e.KeyCode = Keys.V Or e.KeyCode = Keys.C Or e.KeyCode = Keys.X) Then
            e.SuppressKeyPress = True
        End If

        If e.KeyCode = Keys.Enter Then
            btnadd_Click(sender, e)
            e.Handled = True
        End If

    End Sub

    Private Sub txtsection_KeyPress(sender As Object, e As KeyPressEventArgs) Handles txtsection.KeyPress

        If Not Char.IsLetterOrDigit(e.KeyChar) AndAlso Not Char.IsControl(e.KeyChar) AndAlso Not Char.IsWhiteSpace(e.KeyChar) Then
            e.Handled = True
        End If


    End Sub

    Private Sub txtsearch_KeyDown(sender As Object, e As KeyEventArgs) Handles txtsearch.KeyDown

        If e.Control AndAlso (e.KeyCode = Keys.V Or e.KeyCode = Keys.C Or e.KeyCode = Keys.X) Then
            e.SuppressKeyPress = True
        End If

    End Sub

    Private Sub btnadd_KeyDown(sender As Object, e As KeyEventArgs) Handles btnadd.KeyDown

        If e.KeyCode = Keys.Enter Then
            btnadd_Click(sender, e)
            e.Handled = True
        End If

    End Sub

    Public Sub cbdeptss()

        Dim con As New MySqlConnection(connectionString)
        Dim com As String = "SELECT ID, Department FROM `department_tbl`"
        Dim adap As New MySqlDataAdapter(com, con)
        Dim ds As New DataTable

        adap.Fill(ds)

        cbdepartment.DataSource = ds
        cbdepartment.DisplayMember = "Department"
        cbdepartment.ValueMember = "ID"
        cbdepartment.SelectedIndex = -1

    End Sub

    Public Sub cbgradesu()

        Dim con As New MySqlConnection(connectionString)
        Dim com As String = "SELECT ID, Grade FROM `grade_tbl`"
        Dim adap As New MySqlDataAdapter(com, con)
        Dim ds As New DataTable

        adap.Fill(ds)

        cbgrade.DataSource = ds
        cbgrade.DisplayMember = "Grade"
        cbgrade.ValueMember = "ID"
        cbgrade.SelectedIndex = -1

    End Sub

    Public Sub cbstrandsu()

        Dim con As New MySqlConnection(connectionString)
        Dim com As String = "SELECT ID, Strand FROM `strand_tbl`"
        Dim adap As New MySqlDataAdapter(com, con)
        Dim ds As New DataTable

        adap.Fill(ds)

        cbstrand.DataSource = ds
        cbstrand.DisplayMember = "Strand"
        cbstrand.ValueMember = "ID"
        cbstrand.SelectedIndex = -1

    End Sub

    Private Sub cbdepartment_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbdepartment.SelectedIndexChanged

        If cbdepartment.SelectedIndex <> -1 Then
            Dim selectedDept As String = cbdepartment.GetItemText(cbdepartment.SelectedItem)

            cbgrade.DataSource = Nothing
            cbgrade.Items.Clear()


            cbgrade.Enabled = False
            txtsection.Enabled = False
            cbstrand.Enabled = False

            If selectedDept = "Junior High School" Then
                Dim con As New MySqlConnection(connectionString)
                Dim com As String = "SELECT ID, Grade FROM `grade_tbl` WHERE Grade BETWEEN 7 AND 10"
                Dim adap As New MySqlDataAdapter(com, con)
                Dim ds As New DataTable
                adap.Fill(ds)

                cbgrade.DataSource = ds
                cbgrade.DisplayMember = "Grade"
                cbgrade.ValueMember = "ID"
                cbgrade.SelectedIndex = -1

                txtsection.Visible = True
                cbstrand.Visible = False

                lbl_sectionandstrand.Text = "Section:"


                cbgrade.Enabled = True

            ElseIf selectedDept = "Senior High School" Then
                Dim con As New MySqlConnection(connectionString)
                Dim com As String = "SELECT ID, Grade FROM `grade_tbl` WHERE Grade BETWEEN 11 AND 12"
                Dim adap As New MySqlDataAdapter(com, con)
                Dim ds As New DataTable
                adap.Fill(ds)

                cbgrade.DataSource = ds
                cbgrade.DisplayMember = "Grade"
                cbgrade.ValueMember = "ID"
                cbgrade.SelectedIndex = -1

                cbstrand.Visible = True
                txtsection.Visible = False

                cbstrandsu()

                lbl_sectionandstrand.Text = "Strand:"

                cbgrade.Enabled = True

            End If
        End If
    End Sub

    Private Sub btnclear_Click(sender As Object, e As EventArgs) Handles btnclear.Click
        clearlahat()
    End Sub

    Public Sub clearlahat()

        cbdepartment.DataSource = Nothing
        cbgrade.DataSource = Nothing
        cbstrand.DataSource = Nothing

        cbdeptss()
        cbgradesu()
        cbstrandsu()

        txtsection.Text = ""
        txtsearch.Clear()


        cbgrade.Enabled = False
        txtsection.Enabled = False
        cbstrand.Enabled = False


        txtsection.Visible = True
        cbstrand.Visible = False

        rbjhs.Checked = False
        rbshs.Checked = False

        lbl_sectionandstrand.Text = "Section:"


        Dim dt As DataTable = DirectCast(DataGridView1.DataSource, DataTable)
        If dt IsNot Nothing Then
            dt.DefaultView.RowFilter = ""
        End If

        DataGridView1.ClearSelection()

    End Sub

    Private Sub cbgrade_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbgrade.SelectedIndexChanged

        If cbgrade.SelectedIndex <> -1 Then
            Dim selectedDept As String = cbdepartment.GetItemText(cbdepartment.SelectedItem)

            If selectedDept = "Junior High School" Then
                txtsection.Enabled = True
            ElseIf selectedDept = "Senior High School" Then
                cbstrand.Enabled = True
            End If
        Else

            txtsection.Enabled = False
            cbstrand.Enabled = False
        End If
    End Sub

    Private Sub RadioButton1_CheckedChanged(sender As Object, e As EventArgs) Handles rbjhs.CheckedChanged

        Dim dt As DataTable = DirectCast(DataGridView1.DataSource, DataTable)

        If dt IsNot Nothing AndAlso rbjhs.Checked Then
            dt.DefaultView.RowFilter = "Department = 'Junior High School'"
        End If

    End Sub

    Private Sub RadioButton2_CheckedChanged(sender As Object, e As EventArgs) Handles rbshs.CheckedChanged

        Dim dt As DataTable = DirectCast(DataGridView1.DataSource, DataTable)

        If dt IsNot Nothing AndAlso rbshs.Checked Then
            dt.DefaultView.RowFilter = "Department = 'Senior High School'"
        End If

    End Sub


End Class