Imports MySql.Data.MySqlClient

Public Class Category
    Private Sub Category_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        TopMost = True
        Me.Refresh()

        Dim con As New MySqlConnection(GlobalVarsModule.connectionString)
        Dim com As String = "SELECT * FROM `category_tbl`"
        Dim adap As New MySqlDataAdapter(com, con)
        Dim dt As New DataSet

        adap.Fill(dt, "INFO")
        DataGridView1.DataSource = dt.Tables("INFO")



        DataGridView1.EnableHeadersVisualStyles = False
        DataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(207, 58, 109)
        DataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White
        DataGridView1.Columns("ID").Visible = False


    End Sub

    Private Sub Category_Shown(sender As Object, e As EventArgs) Handles MyBase.Shown

        DataGridView1.ClearSelection()

    End Sub

    Private Sub Category_FormClosed(sender As Object, e As FormClosedEventArgs) Handles Me.FormClosed

        For Each form In Application.OpenForms
            If TypeOf form Is MainForm Then
                Dim load = DirectCast(form, MainForm)
                load.loadsu()
            End If
        Next

        MainForm.MaintenanceToolStripMenuItem.ForeColor = Color.White
        txtcategory.Text = ""

    End Sub

    Private Sub btnadd_Click(sender As Object, e As EventArgs) Handles btnadd.Click

        Dim con As New MySqlConnection(GlobalVarsModule.connectionString)
        Dim category As String = txtcategory.Text.Trim
        Dim newID As Integer = 0

        If String.IsNullOrWhiteSpace(category) Then
            MsgBox("Please fill in the required fields.", vbExclamation, "Missing Information")
            Exit Sub
        End If

        Try
            con.Open()

            Dim comss As New MySqlCommand("SELECT COUNT(*) FROM `category_tbl` WHERE `Category` = @category", con)
            comss.Parameters.AddWithValue("@category", category)
            Dim count As Integer = Convert.ToInt32(comss.ExecuteScalar)

            If count > 0 Then
                MsgBox("This category already exists.", vbExclamation, "Duplication is not allowed.")
                Exit Sub
            End If

            Dim com As New MySqlCommand("INSERT INTO `category_tbl`(`Category`) VALUES (@category); SELECT LAST_INSERT_ID();", con)
            com.Parameters.AddWithValue("@category", category)
            newID = Convert.ToInt32(com.ExecuteScalar())

            GlobalVarsModule.LogAudit(
                actionType:="ADD",
                formName:="CATEGORY FORM",
                description:=$"Added new category: {category}",
                recordID:=newID.ToString()
            )

            For Each form In Application.OpenForms
                If TypeOf form Is AuditTrail Then
                    DirectCast(form, AuditTrail).refreshaudit()
                End If
            Next

            For Each form In Application.OpenForms
                If TypeOf form Is Book Then
                    Dim book = DirectCast(form, Book)
                    book.cbcategoryy()
                End If
            Next

            MsgBox("Category added successfully", vbInformation)
            Category_Load(sender, e)

        Catch ex As Exception
            MsgBox(ex.Message, vbCritical)
        Finally
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
            txtcategory.Clear()
        End Try
    End Sub

    Private Sub btnedit_Click(sender As Object, e As EventArgs) Handles btnedit.Click

        If DataGridView1.SelectedRows.Count > 0 Then

            Dim con As New MySqlConnection(GlobalVarsModule.connectionString)

            Dim selectedRow As DataGridViewRow = DataGridView1.SelectedRows(0)
            Dim ID As Integer = CInt(selectedRow.Cells("ID").Value)

            Dim cat As String = txtcategory.Text.Trim

            If String.IsNullOrWhiteSpace(cat) Then
                MsgBox("Please fill in the required fields.", vbExclamation, "Missing Information")
                Exit Sub
            End If

            Dim oldcats As String = selectedRow.Cells("Category").Value.ToString()
            Dim newcats As String = txtcategory.Text.Trim()

            If oldcats.ToUpper() = newcats.ToUpper() Then
                MsgBox("No changes were made.", vbExclamation, "No Update")
                Exit Sub
            End If

            Try
                con.Open()

                Dim comss As New MySqlCommand("SELECT COUNT(*) FROM `category_tbl` WHERE `Category` = @category AND `ID` <> @ID", con)
                comss.Parameters.AddWithValue("@category", cat)
                comss.Parameters.AddWithValue("@ID", ID)
                Dim count As Integer = Convert.ToInt32(comss.ExecuteScalar)

                If count > 0 Then
                    MsgBox("This category already exists.", vbExclamation, "Duplication is not allowed.")
                    Exit Sub
                End If



                Dim com As New MySqlCommand("UPDATE `category_tbl` SET `Category` = @newCat WHERE `ID` = @ID", con)
                com.Parameters.AddWithValue("@newCat", newcats)
                com.Parameters.AddWithValue("@ID", ID)
                com.ExecuteNonQuery()


                Dim comsus As New MySqlCommand("UPDATE `book_tbl` SET `Category` = @newCat WHERE `Category` = @oldCat", con)
                comsus.Parameters.AddWithValue("@newCat", newcats)
                comsus.Parameters.AddWithValue("@oldCat", oldcats)
                comsus.ExecuteNonQuery()

                GlobalVarsModule.LogAudit(
                    actionType:="UPDATE",
                    formName:="CATEGORY FORM",
                    description:=$"Updated category ID {ID} from '{oldcats}' to '{newcats}'",
                    recordID:=ID.ToString(),
                    oldValue:=$"Category: {oldcats}",
                    newValue:=$"Category: {newcats}"
                )

                For Each form In Application.OpenForms
                    If TypeOf form Is AuditTrail Then
                        DirectCast(form, AuditTrail).refreshaudit()
                    End If
                Next

                For Each form In Application.OpenForms
                    If TypeOf form Is Book Then
                        Dim book = DirectCast(form, Book)
                        book.cbcategoryy()
                    End If
                Next

                For Each form In Application.OpenForms
                    If TypeOf form Is MainForm Then
                        Dim load = DirectCast(form, MainForm)
                        load.loadsu()
                    End If
                Next

                MsgBox("Updated successfully!", vbInformation)
                Category_Load(sender, e)
                txtcategory.Clear()
            Catch ex As Exception
                MsgBox(ex.Message, vbCritical)
            Finally
                If con.State = ConnectionState.Open Then
                    con.Close()
                End If
            End Try
        Else
            MsgBox("Please select a row to edit.", vbExclamation)
        End If

    End Sub

    Private Sub btndelete_Click(sender As Object, e As EventArgs) Handles btndelete.Click

        If DataGridView1.SelectedRows.Count > 0 Then

            Dim dialogResult As DialogResult = MessageBox.Show("Are you sure you want to delete this category?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)

            If dialogResult = DialogResult.Yes Then

                Dim con As New MySqlConnection(GlobalVarsModule.connectionString)
                Dim selectedRow As DataGridViewRow = DataGridView1.SelectedRows(0)
                Dim ID As Integer = CInt(selectedRow.Cells("ID").Value)
                Dim categoryName As String = selectedRow.Cells("Category").Value.ToString().Trim()

                Try
                    con.Open()


                    Dim bookCom As New MySqlCommand("SELECT COUNT(*) FROM `book_tbl` WHERE Category = @category", con)
                    bookCom.Parameters.AddWithValue("@category", categoryName)
                    Dim bookCount As Integer = CInt(bookCom.ExecuteScalar())

                    If bookCount > 0 Then
                        MessageBox.Show("Cannot delete this category. It is assigned to " & bookCount & " book(s).", "Information", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        Return
                    End If


                    Dim delete As New MySqlCommand("DELETE FROM `category_tbl` WHERE `ID` = @id", con)
                    delete.Parameters.AddWithValue("@id", ID)
                    delete.ExecuteNonQuery()

                    GlobalVarsModule.LogAudit(
                        actionType:="DELETE",
                        formName:="CATEGORY FORM",
                        description:=$"Deleted category: {categoryName}",
                        recordID:=ID.ToString()
                    )

                    For Each form In Application.OpenForms
                        If TypeOf form Is AuditTrail Then
                            DirectCast(form, AuditTrail).refreshaudit()
                        End If
                    Next

                    For Each form In Application.OpenForms
                        If TypeOf form Is Book Then
                            Dim book = DirectCast(form, Book)
                            book.cbcategoryy()
                        End If
                    Next

                    MsgBox("Category deleted successfully.", vbInformation)
                    Category_Load(sender, e)
                    txtcategory.Clear()

                    Dim count As New MySqlCommand("SELECT COUNT(*) FROM `category_tbl`", con)
                    Dim rowCount As Long = CLng(count.ExecuteScalar())

                    If rowCount = 0 Then
                        Dim reset As New MySqlCommand("ALTER TABLE `category_tbl` AUTO_INCREMENT = 1", con)
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
            txtcategory.Text = row.Cells("Category").Value.ToString

        End If

    End Sub

    Private Sub txtsearch_TextChanged(sender As Object, e As EventArgs) Handles txtsearch.TextChanged


        Dim dt As DataTable = DirectCast(DataGridView1.DataSource, DataTable)
        If dt IsNot Nothing Then
            If txtsearch.Text.Trim() <> "" Then
                Dim filter As String = String.Format("Category LIKE '*{0}*'", txtsearch.Text.Trim())
                dt.DefaultView.RowFilter = filter
            Else
                dt.DefaultView.RowFilter = ""
            End If
        End If

    End Sub

    Private Sub txtcategory_KeyDown(sender As Object, e As KeyEventArgs) Handles txtcategory.KeyDown

        If e.Control AndAlso (e.KeyCode = Keys.V Or e.KeyCode = Keys.C Or e.KeyCode = Keys.X) Then
            e.SuppressKeyPress = True
        End If

        If e.KeyCode = Keys.Enter Then
            btnadd_Click(sender, e)
            e.Handled = True
        End If

    End Sub

    Private Sub txtcategory_KeyPress(sender As Object, e As KeyPressEventArgs) Handles txtcategory.KeyPress


        If Not Char.IsLetter(e.KeyChar) AndAlso Not Char.IsControl(e.KeyChar) AndAlso Not Char.IsWhiteSpace(e.KeyChar) Then
            e.Handled = True
        End If

    End Sub

    Private Sub txtsearch_KeyDown(sender As Object, e As KeyEventArgs) Handles txtsearch.KeyDown

        If e.Control AndAlso (e.KeyCode = Keys.V Or e.KeyCode = Keys.C Or e.KeyCode = Keys.X) Then
            e.SuppressKeyPress = True
        End If

    End Sub

    Private Sub Category_KeyDown(sender As Object, e As KeyEventArgs) Handles MyBase.KeyDown
        If e.KeyCode = Keys.Escape Then
            Me.Close()
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
End Class