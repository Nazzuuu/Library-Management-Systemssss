Imports MySql.Data.MySqlClient

Public Class Borrowing

    Private WithEvents timerSystemDate As New Timer()

    Private Sub Borrowing_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        refreshborrowingsu()


        timerSystemDate.Interval = 1000
        timerSystemDate.Start()

    End Sub


    Public Sub refreshborrowingsu()

        Dim con As New MySqlConnection(connectionString)
        Dim com As String = "SELECT * FROM `borrowing_tbl`"
        Dim adap As New MySqlDataAdapter(com, con)
        Dim ds As New DataSet

        adap.Fill(ds, "borrowing_data")

        DataGridView1.DataSource = ds.Tables("borrowing_data")
        DataGridView1.Columns("ID").Visible = False

        DataGridView1.EnableHeadersVisualStyles = False
        DataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(207, 58, 109)
        DataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White

        clearlahat()

    End Sub
    Private Sub Borrowing_Shown(sender As Object, e As EventArgs) Handles MyBase.Shown

        DataGridView1.ClearSelection()

    End Sub


    Public Sub clearlahat()


        DataGridView1.ClearSelection()
        DataGridView1.CurrentCell = Nothing

        txtemployee.Enabled = False
        txtlrn.Enabled = False
        txtname.Enabled = False
        txtbooktitle.Enabled = False
        txtisbn.Enabled = False
        txtbarcode.Enabled = False
        txtshelf.Enabled = False
        txtduedate.Enabled = False


        rbstudent.Checked = False
        rbteacher.Checked = False


        txtlrn.Text = ""
        txtemployee.Text = ""
        txtisbn.Text = ""
        txtname.Text = ""
        txtbooktitle.Text = ""
        txtbarcode.Text = ""
        txtaccessionid.Text = ""
        txtshelf.Text = ""


        DateTimePicker1.Value = DateTime.Now

    End Sub


    Private Sub rbteacher_CheckedChanged(sender As Object, e As EventArgs) Handles rbteacher.CheckedChanged

        If rbteacher.Checked Then

            txtemployee.Enabled = True
            txtlrn.Enabled = False

            txtlrn.Text = ""
            txtname.Text = ""
        End If

    End Sub

    Private Sub rbstudent_CheckedChanged(sender As Object, e As EventArgs) Handles rbstudent.CheckedChanged

        If rbstudent.Checked Then

            txtemployee.Enabled = False
            txtlrn.Enabled = True

            txtemployee.Text = ""
            txtname.Text = ""
        End If

    End Sub

    Private Sub txtemployee_TextChanged(sender As Object, e As EventArgs) Handles txtemployee.TextChanged

        Dim con As New MySqlConnection(connectionString)
        Try
            con.Open()
            Dim com As String = "SELECT `FirstName` FROM `borrower_tbl` WHERE `EmployeeNo` = @emp"
            Using comsi As New MySqlCommand(com, con)
                comsi.Parameters.AddWithValue("@emp", txtemployee.Text)
                Dim emp As Object = comsi.ExecuteScalar()
                If emp IsNot Nothing Then
                    txtname.Text = emp.ToString()
                Else
                    txtname.Text = ""
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

    Private Sub txtlrn_TextChanged(sender As Object, e As EventArgs) Handles txtlrn.TextChanged

        Dim con As New MySqlConnection(connectionString)
        Try
            con.Open()
            Dim com As String = "SELECT `FirstName` FROM `borrower_tbl` WHERE `LRN` = @lrn"
            Using comsi As New MySqlCommand(com, con)
                comsi.Parameters.AddWithValue("@lrn", txtlrn.Text)
                Dim lrn As Object = comsi.ExecuteScalar()
                If lrn IsNot Nothing Then
                    txtname.Text = lrn.ToString()
                Else
                    txtname.Text = ""
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

    Private Sub txtaccessionid_TextChanged(sender As Object, e As EventArgs) Handles txtaccessionid.TextChanged

        If String.IsNullOrWhiteSpace(txtaccessionid.Text) Then

            txtbooktitle.Text = ""
            txtisbn.Text = ""
            txtbarcode.Text = ""
            txtshelf.Text = ""
            Exit Sub
        End If

        Dim con As New MySqlConnection(connectionString)
        Dim status As String = ""

        Try
            con.Open()

            Dim com As String = "SELECT `BookTitle`, `ISBN`, `Barcode`, `Shelf`, `Status` FROM `acession_tbl` WHERE `AccessionID` = @acs"

            Using comsi As New MySqlCommand(com, con)
                comsi.Parameters.AddWithValue("@acs", txtaccessionid.Text)

                Using reader As MySqlDataReader = comsi.ExecuteReader()
                    If reader.Read() Then

                        status = reader("Status").ToString()


                        If status.Equals("Available", StringComparison.OrdinalIgnoreCase) Then

                            txtbooktitle.Text = reader("BookTitle").ToString()
                            txtisbn.Text = reader("ISBN").ToString()
                            txtbarcode.Text = reader("Barcode").ToString()
                            txtshelf.Text = reader("Shelf").ToString()
                        Else

                            MessageBox.Show("WARNING: This book has a status of '" & status & "'. Only 'Available' books can be borrowed.", "Book Not Available", MessageBoxButtons.OK, MessageBoxIcon.Warning)


                            txtbooktitle.Text = ""
                            txtisbn.Text = ""
                            txtbarcode.Text = ""
                            txtshelf.Text = ""
                        End If
                    Else

                        txtbooktitle.Text = ""
                        txtisbn.Text = ""
                        txtbarcode.Text = ""
                        txtshelf.Text = ""
                    End If
                End Using
            End Using

        Catch ex As Exception
            MessageBox.Show("Database Error: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
        End Try
    End Sub

    Private Sub DateTimePicker1_ValueChanged(sender As Object, e As EventArgs) Handles DateTimePicker1.ValueChanged

        txtduedate.Text = DateTimePicker1.Value.AddDays(7).ToString("MMMM-dd-yyyy")
    End Sub



    Private Sub timerSystemDate_Tick(sender As Object, e As EventArgs) Handles timerSystemDate.Tick

        If DateTimePicker1.Value.Date <> DateTime.Now.Date Then
            DateTimePicker1.Value = DateTime.Now
        End If
    End Sub

    Private Sub Borrowing_FormClosed(sender As Object, e As FormClosedEventArgs) Handles MyBase.FormClosed

        timerSystemDate.Stop()
        timerSystemDate.Dispose()

    End Sub

    Private Sub btnclear_Click(sender As Object, e As EventArgs) Handles btnclear.Click
        clearlahat()
    End Sub

    Private Sub btnadd_Click(sender As Object, e As EventArgs) Handles btnadd.Click

        Dim borrower As String = ""
        Dim con As New MySqlConnection(connectionString)


        If rbstudent.Checked Then
            borrower = "Student"
        ElseIf rbteacher.Checked Then
            borrower = "Teacher"
        Else
            MsgBox("Please select a borrower type (Student or Teacher).", vbExclamation, "Missing Information")
            Exit Sub
        End If


        If String.IsNullOrWhiteSpace(txtaccessionid.Text) OrElse
           String.IsNullOrWhiteSpace(txtname.Text) Then

            MsgBox("Accession ID and Borrower Name are required.", vbExclamation, "Missing Information")
            Exit Sub
        End If

        If borrower = "Student" And String.IsNullOrWhiteSpace(txtlrn.Text) Then
            MsgBox("LRN is required for Students.", vbExclamation, "Missing Information")
            Exit Sub
        ElseIf borrower = "Teacher" And String.IsNullOrWhiteSpace(txtemployee.Text) Then
            MsgBox("Employee No is required for Teachers.", vbExclamation, "Missing Information")
            Exit Sub
        End If


        Try
            con.Open()

            Dim com As String = "INSERT INTO borrowing_tbl (Borrower, LRN, EmployeeNo, Name, BookTitle, ISBN, Barcode, AccessionID, Shelf, BorrowedDate, DueDate) " &
                                "VALUES (@Borrower, @LRN, @EmpNo, @Name, @Title, @ISBN, @Barcode, @AccessionID, @Shelf, @BDate, @DDate)"

            Using comsi As New MySqlCommand(com, con)

                comsi.Parameters.AddWithValue("@Borrower", borrower)
                comsi.Parameters.AddWithValue("@LRN", txtlrn.Text)
                comsi.Parameters.AddWithValue("@EmpNo", txtemployee.Text)
                comsi.Parameters.AddWithValue("@Name", txtname.Text)
                comsi.Parameters.AddWithValue("@Title", txtbooktitle.Text)
                comsi.Parameters.AddWithValue("@ISBN", txtisbn.Text)
                comsi.Parameters.AddWithValue("@Barcode", txtbarcode.Text)
                comsi.Parameters.AddWithValue("@AccessionID", txtaccessionid.Text)
                comsi.Parameters.AddWithValue("@Shelf", txtshelf.Text)

                comsi.Parameters.AddWithValue("@BDate", DateTimePicker1.Value.ToString("yyyy-MM-dd"))

                comsi.Parameters.AddWithValue("@DDate", DateTime.Parse(txtduedate.Text).ToString("yyyy-MM-dd"))

                comsi.ExecuteNonQuery()

                MsgBox("Borrowing record successfully added.", vbInformation, "Success")
                refreshborrowingsu()
                clearlahat()

            End Using

        Catch ex As Exception
            MessageBox.Show("Error adding record: " & ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
        End Try

    End Sub

    Private Sub btnedit_Click(sender As Object, e As EventArgs) Handles btnedit.Click

        Dim borrower As String = ""
        Dim con As New MySqlConnection(connectionString)


        If DataGridView1.SelectedRows.Count = 0 Then
            MsgBox("Please select a record from the table to edit.", vbExclamation, "No Record Selected")
            Exit Sub
        End If

        Dim ID As String = DataGridView1.CurrentRow.Cells("ID").Value.ToString()

        If rbstudent.Checked Then
            borrower = "Student"
        ElseIf rbteacher.Checked Then
            borrower = "Teacher"
        Else
            MsgBox("Please select a borrower type (Student or Teacher).", vbExclamation, "Missing Information")
            Exit Sub
        End If


        If String.IsNullOrWhiteSpace(txtaccessionid.Text) OrElse
           String.IsNullOrWhiteSpace(txtname.Text) Then

            MsgBox("Accession ID and Borrower Name are required.", vbExclamation, "Missing Information")
            Exit Sub
        End If


        If borrower = "Student" And String.IsNullOrWhiteSpace(txtlrn.Text) Then
            MsgBox("LRN is required for Students.", vbExclamation, "Missing Information")
            Exit Sub
        ElseIf borrower = "Teacher" And String.IsNullOrWhiteSpace(txtemployee.Text) Then
            MsgBox("Employee No is required for Teachers.", vbExclamation, "Missing Information")
            Exit Sub
        End If


        Try
            con.Open()

            Dim com As String = "UPDATE borrowing_tbl SET " &
                                "Borrower = @Borrower, LRN = @LRN, EmployeeNo = @EmpNo, Name = @Name, " &
                                "BookTitle = @Title, ISBN = @ISBN, Barcode = @Barcode, AccessionID = @AccessionID, " &
                                "Shelf = @Shelf, BorrowedDate = @BDate, DueDate = @DDate " &
                                "WHERE ID = @ID"

            Using comsi As New MySqlCommand(com, con)

                comsi.Parameters.AddWithValue("@Borrower", borrower)
                comsi.Parameters.AddWithValue("@LRN", txtlrn.Text)
                comsi.Parameters.AddWithValue("@EmpNo", txtemployee.Text)
                comsi.Parameters.AddWithValue("@Name", txtname.Text)
                comsi.Parameters.AddWithValue("@Title", txtbooktitle.Text)
                comsi.Parameters.AddWithValue("@ISBN", txtisbn.Text)
                comsi.Parameters.AddWithValue("@Barcode", txtbarcode.Text)
                comsi.Parameters.AddWithValue("@AccessionID", txtaccessionid.Text)
                comsi.Parameters.AddWithValue("@Shelf", txtshelf.Text)
                comsi.Parameters.AddWithValue("@BDate", DateTimePicker1.Value.ToString("yyyy-MM-dd"))
                comsi.Parameters.AddWithValue("@DDate", DateTime.Parse(txtduedate.Text).ToString("yyyy-MM-dd"))
                comsi.Parameters.AddWithValue("@ID", ID)

                comsi.ExecuteNonQuery()

                MsgBox("Borrowing record successfully updated.", vbInformation, "Success")
                refreshborrowingsu()
                clearlahat()

            End Using

        Catch ex As Exception
            MessageBox.Show("Error updating record: " & ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
        End Try

    End Sub

    Private Sub btndelete_Click(sender As Object, e As EventArgs) Handles btndelete.Click

        Dim con As New MySqlConnection(connectionString)

        If DataGridView1.SelectedRows.Count = 0 Then
            MsgBox("Please select a record from the table to delete.", vbExclamation, "No Record Selected")
            Exit Sub
        End If

        Dim ID As String = DataGridView1.CurrentRow.Cells("ID").Value.ToString()

        If MsgBox("Are you sure you want to DELETE the selected borrowing record?", vbYesNo + vbQuestion, "Confirm Delete") = vbNo Then
            Exit Sub
        End If

        Try
            con.Open()

            Dim com As String = "DELETE FROM borrowing_tbl WHERE ID = @ID"

            Using comsi As New MySqlCommand(com, con)
                comsi.Parameters.AddWithValue("@ID", ID)

                comsi.ExecuteNonQuery()

                MsgBox("Borrowing record successfully deleted.", vbInformation, "Success")
                refreshborrowingsu()
                clearlahat()

            End Using

        Catch ex As Exception
            MessageBox.Show("Error deleting record: " & ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
        End Try

    End Sub


    Private Sub DataGridView1_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellClick

        If e.RowIndex >= 0 Then

            Dim row As DataGridViewRow = DataGridView1.Rows(e.RowIndex)

            txtemployee.Text = If(row.Cells("EmployeeNo").Value Is DBNull.Value, "", row.Cells("EmployeeNo").Value.ToString())
            txtlrn.Text = If(row.Cells("LRN").Value Is DBNull.Value, "", row.Cells("LRN").Value.ToString())

            If row.Cells("Borrower").Value.ToString() = "Student" Then
                rbstudent.Checked = True
            ElseIf row.Cells("Borrower").Value.ToString() = "Teacher" Then
                rbteacher.Checked = True
            End If

            txtname.Text = row.Cells("Name").Value.ToString()
            txtbooktitle.Text = row.Cells("BookTitle").Value.ToString()
            txtisbn.Text = row.Cells("ISBN").Value.ToString()
            txtbarcode.Text = row.Cells("Barcode").Value.ToString()
            txtaccessionid.Text = row.Cells("AccessionID").Value.ToString()
            txtshelf.Text = row.Cells("Shelf").Value.ToString()
            txtduedate.Text = row.Cells("DueDate").Value.ToString()

            If row.Cells("BorrowedDate").Value IsNot DBNull.Value Then
                If IsDate(row.Cells("BorrowedDate").Value) Then
                    DateTimePicker1.Value = CDate(row.Cells("BorrowedDate").Value)
                End If
            End If

        End If

    End Sub

End Class