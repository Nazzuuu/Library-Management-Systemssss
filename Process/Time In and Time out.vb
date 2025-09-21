Imports MySql.Data.MySqlClient
Imports System.Data

Public Class oras

    Dim selectedID As Integer


    Private Sub btnregisterview_Click(sender As Object, e As EventArgs) Handles btnregisterview.Click

        AddHandler RegisteredBrwr.ListView1.MouseDoubleClick, AddressOf RegisteredBrwr.ListView1_MouseDoubleClick

        RegisteredBrwr.lbl_action.ForeColor = Color.Red
        RegisteredBrwr.lbl_action.Text = "Selecting"

        RegisteredBrwr.ListView1.Enabled = True
        RegisteredBrwr.ShowDialog
        RegisteredBrwr.ludeyngborrower
        RegisteredBrwr.ludeyngtimedinborrower

    End Sub


    Private Sub oras_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        ludeyngoras()
        clearlahat()

    End Sub

    Private Sub Oras_Shown(sender As Object, e As EventArgs) Handles MyBase.Shown

        DataGridView1.ClearSelection()

    End Sub

    Public Sub ludeyngoras()

        Dim con As New MySqlConnection(connectionString)
        Dim com As String = "SELECT `ID`, `Borrower`, `FirstName`, `LastName`, `MiddleName`, `LRN`, `EmployeeNo`, `Contact`, `Department`, `GradeLevel`, `Section`, `Strand`, `TimeIn`, `TimeOut` FROM `oras_tbl` ORDER BY `TimeIn` DESC"

        Try
            con.Open()
            Dim adap As New MySqlDataAdapter(com, con)
            Dim ds As New DataSet
            adap.Fill(ds, "oras_data")

            DataGridView1.DataSource = ds.Tables("oras_data")

            DataGridView1.Columns("ID").Visible = False
            DataGridView1.EnableHeadersVisualStyles = False
            DataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(207, 58, 109)
            DataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White



        Catch ex As Exception
            MessageBox.Show("Error loading time records: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            con.Close()
        End Try

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

        DataGridView1.ClearSelection()

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

        Dim con As New MySqlConnection(connectionString)

        Dim comsu As String = "SELECT COUNT(*) FROM `oras_tbl` WHERE (`LRN` = @LRN AND `LRN` IS NOT NULL AND `LRN` <> '') OR (`EmployeeNo` = @EmployeeNo AND `EmployeeNo` IS NOT NULL AND `EmployeeNo` <> '') AND `TimeOut` IS NULL"
        Dim comsii As New MySqlCommand(comsu, con)


        comsii.Parameters.AddWithValue("@LRN", If(String.IsNullOrWhiteSpace(txtlrn.Text), DBNull.Value, txtlrn.Text))
        comsii.Parameters.AddWithValue("@EmployeeNo", If(String.IsNullOrWhiteSpace(txtemployee.Text), DBNull.Value, txtemployee.Text))

        Try
            con.Open()
            Dim count As Integer = CInt(comsii.ExecuteScalar())
            If count > 0 Then
                MessageBox.Show("This borrower is already timed in.", "Duplication Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If
        Catch ex As Exception
            MessageBox.Show("Error checking borrower status: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            con.Close()
            Return
        Finally
            con.Close()
        End Try

        Dim com As String = "INSERT INTO `oras_tbl` (`Borrower`, `FirstName`, `LastName`, `MiddleName`, `LRN`, `EmployeeNo`, `Contact`, `Department`, `GradeLevel`, `Section`, `Strand`, `TimeIn`) VALUES (@Borrower, @FirstName, @LastName, @MiddleName, @LRN, @EmployeeNo, @ContactNumber, @Department, @Grade, @Section, @Strand, @TimeIn)"
        Dim cmd As New MySqlCommand(com, con)

        cmd.Parameters.AddWithValue("@Borrower", txtborrower.Text)
        cmd.Parameters.AddWithValue("@FirstName", txtfname.Text)
        cmd.Parameters.AddWithValue("@LastName", txtlname.Text)
        cmd.Parameters.AddWithValue("@MiddleName", If(String.IsNullOrWhiteSpace(txtmname.Text), DBNull.Value, txtmname.Text))
        cmd.Parameters.AddWithValue("@LRN", If(String.IsNullOrWhiteSpace(txtlrn.Text), DBNull.Value, txtlrn.Text))
        cmd.Parameters.AddWithValue("@EmployeeNo", If(String.IsNullOrWhiteSpace(txtemployee.Text), DBNull.Value, txtemployee.Text))
        cmd.Parameters.AddWithValue("@ContactNumber", txtcontact.Text)
        cmd.Parameters.AddWithValue("@Department", txtdepartment.Text)
        cmd.Parameters.AddWithValue("@Grade", If(String.IsNullOrWhiteSpace(txtgrade.Text), DBNull.Value, txtgrade.Text))
        cmd.Parameters.AddWithValue("@Section", If(String.IsNullOrWhiteSpace(txtsection.Text), DBNull.Value, txtsection.Text))
        cmd.Parameters.AddWithValue("@Strand", If(String.IsNullOrWhiteSpace(txtstrand.Text), DBNull.Value, txtstrand.Text))
        cmd.Parameters.AddWithValue("@TimeIn", DateTime.Now.ToString("MM/dd/yyyy h:mm tt"))

        Try
            con.Open()
            cmd.ExecuteNonQuery()
            MessageBox.Show("Time-in recorded successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)

            btnview.Visible = True
            ludeyngoras()
            clearlahat()

            Dim luydingclr As RegisteredBrwr = Application.OpenForms.OfType(Of RegisteredBrwr)().FirstOrDefault()
            If luydingclr IsNot Nothing Then
                luydingclr.ludeyngborrower()
            End If

            Dim registeredForm As RegisteredBrwr = Application.OpenForms.OfType(Of RegisteredBrwr)().FirstOrDefault()
            If registeredForm IsNot Nothing Then
                registeredForm.ludeyngtimedinborrower()
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

        cmd.Parameters.AddWithValue("@TimeOut", DateTime.Now.ToString("MM/dd/yyyy h:mm tt"))
        cmd.Parameters.AddWithValue("@ID", selectedID)

        Try
            con.Open()
            cmd.ExecuteNonQuery()
            MessageBox.Show("Time-out recorded successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)

            btnview.Visible = True

            ludeyngoras()
            clearlahat()


            Dim luydingclr As RegisteredBrwr = Application.OpenForms.OfType(Of RegisteredBrwr)().FirstOrDefault()
            If luydingclr IsNot Nothing Then
                luydingclr.ludeyngborrower()
            End If

            Dim registeredForm As RegisteredBrwr = Application.OpenForms.OfType(Of RegisteredBrwr)().FirstOrDefault()
            If registeredForm IsNot Nothing Then
                registeredForm.ludeyngtimedinborrower()
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



                    ludeyngoras()
                    clearlahat()

                    Dim luydingclr As RegisteredBrwr = Application.OpenForms.OfType(Of RegisteredBrwr)().FirstOrDefault()
                    If luydingclr IsNot Nothing Then
                        luydingclr.ludeyngborrower()
                    End If

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
            txtmname.Text = If(row.Cells("MiddleName").Value Is DBNull.Value, "N/A", row.Cells("MiddleName").Value.ToString())
            txtlrn.Text = If(row.Cells("LRN").Value Is DBNull.Value, "N/A", row.Cells("LRN").Value.ToString())
            txtemployee.Text = If(row.Cells("EmployeeNo").Value Is DBNull.Value, "N/A", row.Cells("EmployeeNo").Value.ToString())
            txtcontact.Text = row.Cells("Contact").Value.ToString()
            txtdepartment.Text = row.Cells("Department").Value.ToString()
            txtgrade.Text = If(row.Cells("GradeLevel").Value Is DBNull.Value, "N/A", row.Cells("GradeLevel").Value.ToString())
            txtsection.Text = If(row.Cells("Section").Value Is DBNull.Value, "N/A", row.Cells("Section").Value.ToString())
            txtstrand.Text = If(row.Cells("Strand").Value Is DBNull.Value, "N/A", row.Cells("Strand").Value.ToString())

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

    Private Sub btnview_Click(sender As Object, e As EventArgs) Handles btnview.Click

        RegisteredBrwr.IsInViewMode = True

        RegisteredBrwr.lbl_action.ForeColor = Color.Green
        RegisteredBrwr.lbl_action.Text = "Viewing"

        RegisteredBrwr.ListView1.FullRowSelect = True
        RegisteredBrwr.ListView1.MultiSelect = False
        RegisteredBrwr.ListView1.HideSelection = False
        RegisteredBrwr.ListView1.LabelEdit = False

        RegisteredBrwr.ShowDialog()
        RegisteredBrwr.ludeyngborrower()
        RegisteredBrwr.ludeyngtimedinborrower()


        RegisteredBrwr.IsInViewMode = False

    End Sub

    Private Sub txtsearch_TextChanged(sender As Object, e As EventArgs) Handles txtsearch.TextChanged

        Dim dt As DataTable = DirectCast(DataGridView1.DataSource, DataTable)
        If dt IsNot Nothing Then
            If txtsearch.Text.Trim() <> "" Then
                Dim filter As String = String.Format("Borrower LIKE '*{0}*' OR FirstName LIKE '*{0}*' OR LastName LIKE '*{0}*'", txtsearch.Text.Trim())
                dt.DefaultView.RowFilter = filter
            Else
                dt.DefaultView.RowFilter = ""
            End If
        End If

    End Sub
End Class