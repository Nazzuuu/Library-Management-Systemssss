Imports MySql.Data.MySqlClient

Public Class Grade
    Private Sub Grade_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        TopMost = True

        Dim con As New MySqlConnection(connectionString)
        Dim com As String = "SELECT * FROM `grade_tbl`"
        Dim adap As New MySqlDataAdapter(com, con)
        Dim dt As New DataSet

        adap.Fill(dt, "INFO")
        DataGridView1.DataSource = dt.Tables("INFO")

        DataGridView1.EnableHeadersVisualStyles = False
        DataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(207, 58, 109)
        DataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White

        DataGridView1.Columns("ID").Visible = False
    End Sub

    Private Sub Grade_FormClosed(sender As Object, e As FormClosedEventArgs) Handles Me.FormClosed

        MainForm.MaintenanceToolStripMenuItem.ShowDropDown()
        MainForm.MaintenanceToolStripMenuItem.ForeColor = Color.Gray
        txtgrade.Text = ""
    End Sub

    Private Sub btnadd_Click(sender As Object, e As EventArgs) Handles btnadd.Click

        Dim con As New MySqlConnection(connectionString)
        Dim grds As String = txtgrade.Text.Trim

        If String.IsNullOrWhiteSpace(grds) Then
            MsgBox("Please fill in the required fields.", vbExclamation, "Missing Information")
            Exit Sub
        End If

        Dim grade As Integer
        If Not Integer.TryParse(grds, grade) Then
            MsgBox("Please enter a valid number.", vbExclamation, "Invalid Input")
            txtgrade.Clear()
            Exit Sub
        End If

        If grade < 7 OrElse grade > 12 Then
            MsgBox("Please enter a grade between 7 and 12.", vbExclamation, "Invalid Grade")
            txtgrade.Clear()
            Exit Sub
        End If

        Try
            con.Open()

            Dim coms As New MySqlCommand("SELECT COUNT(*) FROM `grade_tbl` WHERE `Grade` = @grade", con)
            coms.Parameters.AddWithValue("@grade", grds)
            Dim count As Integer = Convert.ToInt32(coms.ExecuteScalar)

            If count > 0 Then
                MsgBox("This grade is already exists.", vbExclamation, "Duplication not allowed.")
                Exit Sub
            End If

            Dim com As New MySqlCommand("INSERT INTO `grade_tbl`(`Grade`) VALUES (@grade)", con)
            com.Parameters.AddWithValue("@grade", grds)
            com.ExecuteNonQuery()

            For Each form In Application.OpenForms
                If TypeOf form Is Borrower Then
                    Dim borrower = DirectCast(form, Borrower)
                    borrower.cbgradee()
                End If
            Next


            For Each form In Application.OpenForms
                If TypeOf form Is Section Then
                    Dim gradesucakes = DirectCast(form, Section)
                    gradesucakes.cbgradesu()
                End If
            Next

            MsgBox("Grade added successfully", vbInformation)
            Grade_Load(sender, e)

        Catch ex As Exception
            MsgBox(ex.Message, vbCritical)
        Finally
            txtgrade.Clear()
        End Try

    End Sub

    Private Sub btnedit_Click(sender As Object, e As EventArgs) Handles btnedit.Click

        If DataGridView1.SelectedRows.Count > 0 Then

            Dim con As New MySqlConnection(connectionString)

            Dim selectedRow As DataGridViewRow = DataGridView1.SelectedRows(0)
            Dim ID As Integer = CInt(selectedRow.Cells("ID").Value)

            Dim grd As String = txtgrade.Text.Trim

            If String.IsNullOrWhiteSpace(grd) Then
                MsgBox("Please fill in the required fields.", vbExclamation, "Missing Information")
                Exit Sub
            End If

            Dim grade As Integer
            If Not Integer.TryParse(grd, grade) Then
                MsgBox("Please enter a valid number.", vbExclamation, "Invalid Input")
                Exit Sub
            End If

            If grade < 7 OrElse grade > 12 Then
                MsgBox("Please enter a grade between 7 and 12.", vbExclamation, "Invalid Grade")
                Exit Sub
            End If

            Try
                con.Open()

                Dim coms As New MySqlCommand("SELECT COUNT(*) FROM `grade_tbl` WHERE `Grade` = @grade", con)
                coms.Parameters.AddWithValue("@grade", grd)
                Dim count As Integer = Convert.ToInt32(coms.ExecuteScalar)

                If count > 0 Then
                    MsgBox("This grade is already exists.", vbExclamation, "Duplication not allowed.")
                    Exit Sub
                End If

                Dim com As New MySqlCommand("UPDATE `grade_tbl` SET `Grade`= @grade WHERE  `ID` = @id", con)
                com.Parameters.AddWithValue("@grade", grd)
                com.Parameters.AddWithValue("@id", ID)
                com.ExecuteNonQuery()

                For Each form In Application.OpenForms
                    If TypeOf form Is Borrower Then
                        Dim borrower = DirectCast(form, Borrower)
                        borrower.cbgradee()
                    End If
                Next


                For Each form In Application.OpenForms
                    If TypeOf form Is Section Then
                        Dim gradesucakes = DirectCast(form, Section)
                        gradesucakes.cbgradesu()
                    End If
                Next

                MsgBox("Updated successfully!", vbInformation)
                Grade_Load(sender, e)
                txtgrade.Clear()
            Catch ex As Exception
                MsgBox(ex.Message, vbCritical)
            End Try

        End If

    End Sub

    Private Sub btndelete_Click(sender As Object, e As EventArgs) Handles btndelete.Click

        If DataGridView1.SelectedRows.Count > 0 Then

            Dim dialogResult As DialogResult = MessageBox.Show("Are you sure you want to delete this grade?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)

            If dialogResult = DialogResult.Yes Then

                Dim con As New MySqlConnection(connectionString)
                Dim selectedRow As DataGridViewRow = DataGridView1.SelectedRows(0)
                Dim ID As Integer = CInt(selectedRow.Cells("ID").Value)
                Dim gradeLevel As String = selectedRow.Cells("Grade").Value.ToString().Trim()

                Try
                    con.Open()


                    Dim sectionCom As New MySqlCommand("SELECT COUNT(*) FROM `section_tbl` WHERE GradeLevel = @grade", con)
                    sectionCom.Parameters.AddWithValue("@grade", gradeLevel)
                    Dim sectionCount As Integer = CInt(sectionCom.ExecuteScalar())

                    If sectionCount > 0 Then
                        MessageBox.Show("Cannot delete this grade. It is currently assigned to " & sectionCount & " section(s).", "Information", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        Return
                    End If


                    Dim borrowerCom As New MySqlCommand("SELECT COUNT(*) FROM `borrower_tbl` WHERE Grade = @grade", con)
                    borrowerCom.Parameters.AddWithValue("@grade", gradeLevel)
                    Dim borrowerCount As Integer = CInt(borrowerCom.ExecuteScalar())

                    If borrowerCount > 0 Then
                        MessageBox.Show("Cannot delete this grade. It is currently assigned to " & borrowerCount & " borrower(s).", "Information", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        Return
                    End If


                    Dim delete As New MySqlCommand("DELETE FROM `grade_tbl` WHERE `ID` = @id", con)
                    delete.Parameters.AddWithValue("@id", ID)
                    delete.ExecuteNonQuery()


                    For Each form In Application.OpenForms
                        If TypeOf form Is Borrower Then
                            DirectCast(form, Borrower).cbgradee()
                        End If
                        If TypeOf form Is Section Then
                            DirectCast(form, Section).cbgradesu()
                        End If
                    Next

                    MsgBox("Grade deleted successfully.", vbInformation)
                    Grade_Load(sender, e)
                    txtgrade.Clear()

                    Dim count As New MySqlCommand("SELECT COUNT(*) FROM `grade_tbl`", con)
                    Dim rowCount As Long = CLng(count.ExecuteScalar())

                    If rowCount = 0 Then
                        Dim reset As New MySqlCommand("ALTER TABLE `grade_tbl` AUTO_INCREMENT = 1", con)
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
            txtgrade.Text = row.Cells("Grade").Value.ToString

        End If

    End Sub

    Private Sub txtsearch_TextChanged(sender As Object, e As EventArgs) Handles txtsearch.TextChanged


        Dim dt As DataTable = DirectCast(DataGridView1.DataSource, DataTable)
        If dt IsNot Nothing Then
            If txtsearch.Text.Trim() <> "" Then
                Dim filter As String = String.Format("Grade LIKE '*{0}*'", txtsearch.Text.Trim())
                dt.DefaultView.RowFilter = filter
            Else
                dt.DefaultView.RowFilter = ""
            End If
        End If

    End Sub

    Private Sub txtgrade_KeyDown(sender As Object, e As KeyEventArgs) Handles txtgrade.KeyDown

        If e.Control AndAlso (e.KeyCode = Keys.V Or e.KeyCode = Keys.C Or e.KeyCode = Keys.X) Then
            e.SuppressKeyPress = True
        End If

        If e.KeyCode = Keys.Enter Then
            btnadd_Click(sender, e)
            e.Handled = True
        End If

    End Sub

    Private Sub txtgrade_KeyPress(sender As Object, e As KeyPressEventArgs) Handles txtgrade.KeyPress

        If Not Char.IsDigit(e.KeyChar) And Not Char.IsControl(e.KeyChar) Then
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
End Class