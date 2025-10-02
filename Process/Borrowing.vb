Imports MySql.Data.MySqlClient

Public Class Borrowing
    Private Sub Borrowing_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        refreshborrowingsu()

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
    Private Sub btnadd_Click(sender As Object, e As EventArgs) Handles btnadd.Click

        Dim borrower As String = ""

        If rbstudent.Checked Then
            borrower = "Student"
        Else
            borrower = "Teacher"
            MsgBox("Please select a borrower type.", vbExclamation, "Missing Information")
            Exit Sub
        End If

    End Sub

    Private Sub btnedit_Click(sender As Object, e As EventArgs) Handles btnedit.Click

    End Sub

    Private Sub btndelete_Click(sender As Object, e As EventArgs) Handles btndelete.Click


    End Sub

    Private Sub btnclear_Click(sender As Object, e As EventArgs) Handles btnclear.Click
        clearlahat()
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

        Dim con As New MySqlConnection(connectionString)

        Try
            con.Open()
            Dim com As String = "SELECT `BookTitle` FROM `acession_tbl` WHERE `AccessionID` = @acs"

            Using comsi As New MySqlCommand(com, con)

                comsi.Parameters.AddWithValue("@acs", txtaccessionid.Text)
                Dim acs As Object = comsi.ExecuteScalar()
                If acs IsNot Nothing Then

                    txtbooktitle.Text = acs.ToString()

                Else

                    txtbooktitle.Text = ""

                End If
            End Using
        Catch ex As Exception
            MessageBox.Show("Error: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
        End Try

        Try
            con.Open()
            Dim com As String = "SELECT `ISBN` FROM `acession_tbl` WHERE `AccessionID` = @acs"

            Using comsi As New MySqlCommand(com, con)

                comsi.Parameters.AddWithValue("@acs", txtaccessionid.Text)
                Dim acs As Object = comsi.ExecuteScalar()
                If acs IsNot Nothing Then

                    txtisbn.Text = acs.ToString()

                Else
                    txtisbn.Text = ""

                End If
            End Using
        Catch ex As Exception
            MessageBox.Show("Error: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
        End Try

        Try
            con.Open()
            Dim com As String = "SELECT `Barcode` FROM `acession_tbl` WHERE `AccessionID` = @acs"

            Using comsi As New MySqlCommand(com, con)

                comsi.Parameters.AddWithValue("@acs", txtaccessionid.Text)
                Dim acs As Object = comsi.ExecuteScalar()
                If acs IsNot Nothing Then


                    txtbarcode.Text = acs.ToString()

                Else

                    txtbarcode.Text = ""

                End If
            End Using
        Catch ex As Exception
            MessageBox.Show("Error: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
        End Try


        Try
            con.Open()
            Dim com As String = "SELECT `Shelf` FROM `acession_tbl` WHERE `AccessionID` = @acs"

            Using comsi As New MySqlCommand(com, con)

                comsi.Parameters.AddWithValue("@acs", txtaccessionid.Text)
                Dim acs As Object = comsi.ExecuteScalar()
                If acs IsNot Nothing Then


                    txtshelf.Text = acs.ToString()

                Else

                    txtshelf.Text = ""

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
End Class