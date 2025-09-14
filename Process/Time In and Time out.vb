Imports MySql.Data.MySqlClient

Public Class oras

    Dim selectedID As Integer

    Private Sub btnregisterview_Click(sender As Object, e As EventArgs) Handles btnregisterview.Click
        RegisteredBrwr.ShowDialog()
        RegisteredBrwr.ludeyngborrower()
    End Sub

    Private Sub oras_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        Dim con As New MySqlConnection(connectionString)
        Dim com As String = "SELECT * FROM `oras_tbl`"
        Dim adap As New MySqlDataAdapter(com, con)
        Dim ds As New DataSet

        adap.Fill(ds, "INFO")

        DataGridView1.DataSource = ds.Tables("INFO")
        DataGridView1.Columns("ID").Visible = False

        DataGridView1.EnableHeadersVisualStyles = False
        DataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(207, 58, 109)
        DataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White

        clearlahat()

    End Sub

    Public Sub clearlahat()

        txtborrower.Enabled = False
        txtcontact.Enabled = False
        txtdepartment.Enabled = False
        txtemployee.Enabled = False
        txtfname.Enabled = False
        txtlname.Enabled = False
        txtmname.Enabled = False
        txtgrade.Enabled = False
        txtlrn.Enabled = False
        txtsection.Enabled = False
        txtstrand.Enabled = False
        rbtimeout.Enabled = False
        rbtimeout.Checked = False

        txtborrower.Text = ""
        txtcontact.Text = ""
        txtdepartment.Text = ""
        txtemployee.Text = ""
        txtfname.Text = ""
        txtlname.Text = ""
        txtmname.Text = ""
        txtsection.Text = ""
        txtstrand.Text = ""
        txtlrn.Text = ""
        txtgrade.Text = ""

    End Sub

    Public Sub brwrinfo(borrowerType As String, firstName As String, lastName As String, middleName As String, lrn As String, employeeNo As String, contactNumber As String, department As String, grade As String, section As String, strand As String)

        txtborrower.Text = borrowerType
        txtfname.Text = firstName
        txtlname.Text = lastName
        txtmname.Text = middleName
        txtlrn.Text = lrn
        txtemployee.Text = employeeNo
        txtcontact.Text = contactNumber
        txtdepartment.Text = department
        txtgrade.Text = grade
        txtsection.Text = section
        txtstrand.Text = strand


    End Sub

    Private Sub btnclear_Click(sender As Object, e As EventArgs) Handles btnclear.Click

        clearlahat()

    End Sub

    Private Sub btnadd_Click(sender As Object, e As EventArgs) Handles btnadd.Click

        If String.IsNullOrWhiteSpace(txtfname.Text) OrElse String.IsNullOrWhiteSpace(txtlname.Text) Then
            MessageBox.Show("Please select a borrower first.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If


        Dim timeinna As Boolean = False
        Dim con As New MySqlConnection(connectionString)


        Dim comsu As String = "SELECT COUNT(*) FROM `oras_tbl` WHERE `Borrower` = @Borrower AND `TimeOut` IS NULL"
        Dim comsii As New MySqlCommand(comsu, con)
        comsii.Parameters.AddWithValue("@Borrower", txtborrower.Text)

        Try
            con.Open()
            Dim count As Integer = CInt(comsii.ExecuteScalar())
            If count > 0 Then
                timeinna = True
            End If
        Catch ex As Exception
            MessageBox.Show("Error checking borrower status: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            con.Close()
            Return
        End Try

        con.Close()


        If timeinna Then
            MessageBox.Show("This borrower is already timed in.", "Duplication Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If


        Dim com As String = "INSERT INTO `oras_tbl` (`Borrower`, `FirstName`, `LastName`, `MiddleName`, `LRN`, `EmployeeNo`, `Contact`, `Department`, `GradeLevel`, `Section`, `Strand`, `TimeIn`) VALUES (@Borrower, @FirstName, @LastName, @MiddleName, @LRN, @EmployeeNo, @ContactNumber, @Department, @Grade, @Section, @Strand, @TimeIn)"
        Dim cmd As New MySqlCommand(com, con)

        cmd.Parameters.AddWithValue("@Borrower", txtborrower.Text)
        cmd.Parameters.AddWithValue("@FirstName", txtfname.Text)
        cmd.Parameters.AddWithValue("@LastName", txtlname.Text)
        cmd.Parameters.AddWithValue("@MiddleName", txtmname.Text)
        cmd.Parameters.AddWithValue("@LRN", txtlrn.Text)
        cmd.Parameters.AddWithValue("@EmployeeNo", txtemployee.Text)
        cmd.Parameters.AddWithValue("@ContactNumber", txtcontact.Text)
        cmd.Parameters.AddWithValue("@Department", txtdepartment.Text)
        cmd.Parameters.AddWithValue("@Grade", txtgrade.Text)
        cmd.Parameters.AddWithValue("@Section", txtsection.Text)
        cmd.Parameters.AddWithValue("@Strand", txtstrand.Text)
        cmd.Parameters.AddWithValue("@TimeIn", DateTime.Now)

        Try
            con.Open()
            cmd.ExecuteNonQuery()
            MessageBox.Show("Time-in recorded successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
            oras_Load(sender, e)

            Dim registeredForm As RegisteredBrwr = Application.OpenForms.OfType(Of RegisteredBrwr)().FirstOrDefault()
            If registeredForm IsNot Nothing Then
                registeredForm.ludeyngborrower()
            End If

        Catch ex As Exception
            MessageBox.Show("Error recording time-in: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            con.Close()
        End Try

    End Sub

    Private Sub btnedit_Click(sender As Object, e As EventArgs) Handles btnedit.Click

        If selectedID = 0 Then
            MessageBox.Show("Please select a record to update.", "Selection Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If


        If Not rbtimeout.Checked Then
            MessageBox.Show("Please check the 'Time out' option to record time out.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Dim con As New MySqlConnection(connectionString)
        Dim com As String = "UPDATE `oras_tbl` SET `TimeOut` = @TimeOut WHERE `ID` = @ID"
        Dim cmd As New MySqlCommand(com, con)

        cmd.Parameters.AddWithValue("@TimeOut", DateTime.Now)
        cmd.Parameters.AddWithValue("@ID", selectedID)

        Try
            con.Open()
            cmd.ExecuteNonQuery()
            MessageBox.Show("Time-out recorded successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
            oras_Load(sender, e)


            Dim registeredForm As RegisteredBrwr = Application.OpenForms.OfType(Of RegisteredBrwr)().FirstOrDefault()
            If registeredForm IsNot Nothing Then
                registeredForm.ludeyngborrower()
            End If
        Catch ex As Exception
            MessageBox.Show("Error recording time-out: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            con.Close()
        End Try

    End Sub

    Private Sub btndelete_Click(sender As Object, e As EventArgs) Handles btndelete.Click

        If DataGridView1.SelectedRows.Count > 0 Then
            Dim selectedRow As DataGridViewRow = DataGridView1.SelectedRows(0)


            Dim timeout As Object = selectedRow.Cells("TimeOut").Value


            If timeout Is DBNull.Value OrElse String.IsNullOrWhiteSpace(timeout.ToString()) Then
                MessageBox.Show("This record cannot be deleted because the borrower has not timed out yet.", "Deletion Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                clearlahat()
                Return
            End If

            Dim dialogResult As DialogResult = MessageBox.Show("Are you sure you want to delete this time-in/time-out record?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)

            If dialogResult = DialogResult.Yes Then
                Dim con As New MySqlConnection(connectionString)
                Dim ID As Integer = CInt(selectedRow.Cells("ID").Value)

                Try
                    con.Open()

                    Dim delete As New MySqlCommand("DELETE FROM `oras_tbl` WHERE `ID` = @id", con)
                    delete.Parameters.AddWithValue("@id", ID)
                    delete.ExecuteNonQuery()

                    MessageBox.Show("Record deleted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
                    oras_Load(sender, e)

                Catch ex As Exception
                    MessageBox.Show("Error deleting record: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Finally
                    con.Close()
                End Try
            End If
        Else
            MessageBox.Show("Please select a record to delete.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If

    End Sub

    Private Sub DataGridView1_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellClick

        If e.RowIndex >= 0 Then
            Dim row As DataGridViewRow = DataGridView1.Rows(e.RowIndex)


            txtborrower.Text = row.Cells("Borrower").Value.ToString()
            txtfname.Text = row.Cells("FirstName").Value.ToString()
            txtlname.Text = row.Cells("LastName").Value.ToString()
            txtmname.Text = row.Cells("MiddleName").Value.ToString()
            txtlrn.Text = row.Cells("LRN").Value.ToString()
            txtemployee.Text = row.Cells("EmployeeNo").Value.ToString()
            txtcontact.Text = row.Cells("Contact").Value.ToString()
            txtdepartment.Text = row.Cells("Department").Value.ToString()
            txtgrade.Text = row.Cells("GradeLevel").Value.ToString()
            txtsection.Text = row.Cells("Section").Value.ToString()
            txtstrand.Text = row.Cells("Strand").Value.ToString()


            Dim timeout As Object = row.Cells("TimeOut").Value

            If timeout Is DBNull.Value OrElse String.IsNullOrWhiteSpace(timeout.ToString()) Then

                rbtimeout.Enabled = True
            Else

                rbtimeout.Enabled = False
                rbtimeout.Checked = True
            End If

            selectedID = CInt(row.Cells("ID").Value)
        End If

    End Sub

End Class