Imports System.Management
Imports MySql.Data.MySqlClient
Public Class Borrower

    Private Sub Borrower_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        Dim con As New MySqlConnection(connectionString)
        Dim com As String = "SELECT * FROM `borrower_tbl`"
        Dim adap As New MySqlDataAdapter(com, con)
        Dim dt As New DataSet
        Try
            con.Open()
            adap.Fill(dt, "INFO")
            DataGridView1.DataSource = dt.Tables("INFO")
            DataGridView1.Columns("ID").Visible = False
            DataGridView1.EnableHeadersVisualStyles = False
            DataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(207, 58, 109)
            DataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White
        Catch ex As Exception
            MessageBox.Show("Error loading data: " & ex.Message)
        Finally
            con.Close()
        End Try

        cbgradee()
        cbsecs()
        cbdepts()
        cbstrandd()

        cbgrade.Enabled = False
        cbsection.Enabled = False
        cbstrand.Enabled = False
        cbdepartment.Enabled = False

    End Sub

    Private Sub btnimport_Click(sender As Object, e As EventArgs) Handles btnimport.Click

        If OpenFileDialog1.ShowDialog <> DialogResult.Cancel Then
            Pic1.Image = Image.FromFile(OpenFileDialog1.FileName)
            Pic1.SizeMode = PictureBoxSizeMode.StretchImage
        End If
    End Sub
    Private Sub btnclearr_Click(sender As Object, e As EventArgs) Handles btnclearr.Click

        Pic1.Image = Nothing

    End Sub

    Public Sub cbgradee()
        Dim con As New MySqlConnection(connectionString)
        Dim com As String = "SELECT ID, Grade FROM `grade_tbl`"
        Dim adap As New MySqlDataAdapter(com, con)
        Dim dt As New DataTable
        Try
            con.Open()
            adap.Fill(dt)
            cbgrade.DataSource = dt
            cbgrade.DisplayMember = "Grade"
            cbgrade.ValueMember = "ID"
            cbgrade.SelectedIndex = -1
        Catch ex As Exception
            MessageBox.Show("Error loading grades: " & ex.Message)
        Finally
            con.Close()
        End Try
    End Sub

    Public Sub cbsecs()
        Dim con As New MySqlConnection(connectionString)
        Dim com As String = "SELECT ID, Section FROM `section_tbl`"
        Dim adap As New MySqlDataAdapter(com, con)
        Dim dt As New DataTable
        Try
            con.Open()
            adap.Fill(dt)
            cbsection.DataSource = dt
            cbsection.DisplayMember = "Section"
            cbsection.ValueMember = "ID"
            cbsection.SelectedIndex = -1
        Catch ex As Exception
            MessageBox.Show("Error loading sections: " & ex.Message)
        Finally
            con.Close()
        End Try
    End Sub

    Public Sub cbdepts()
        Dim con As New MySqlConnection(connectionString)
        Dim com As String = "SELECT ID, Department FROM `department_tbl`"
        Dim adap As New MySqlDataAdapter(com, con)
        Dim dt As New DataTable
        Try
            con.Open()
            adap.Fill(dt)
            cbdepartment.DataSource = dt
            cbdepartment.DisplayMember = "Department"
            cbdepartment.ValueMember = "ID"
            cbdepartment.SelectedIndex = -1
        Catch ex As Exception
            MessageBox.Show("Error loading departments: " & ex.Message)
        Finally
            con.Close()
        End Try

    End Sub

    Public Sub cbstrandd()

        Dim con As New MySqlConnection(connectionString)
        Dim com As String = "SELECT ID, Strand FROM `strand_tbl`"
        Dim adap As New MySqlDataAdapter(com, con)
        Dim dt As New DataTable
        Try
            con.Open()
            adap.Fill(dt)
            cbstrand.DataSource = dt
            cbstrand.DisplayMember = "Strand"
            cbstrand.ValueMember = "ID"
            cbstrand.SelectedIndex = -1
        Catch ex As Exception
            MessageBox.Show("Error loading strands: " & ex.Message)
        Finally
            con.Close()
        End Try

    End Sub

    Private Sub txtsearch_TextChanged(sender As Object, e As EventArgs) Handles txtsearch.TextChanged

        Dim dt As DataTable = DirectCast(DataGridView1.DataSource, DataTable)
        If dt IsNot Nothing Then
            Dim searchValue As String = txtsearch.Text.Trim()
            If Not String.IsNullOrEmpty(searchValue) Then
                Dim filter As String = String.Format("FirstName LIKE '%{0}%' OR LastName LIKE '%{0}%' OR MiddleName LIKE '%{0}%' OR LRN LIKE '%{0}%' OR Borrower LIKE '%{0}%'", searchValue)
                dt.DefaultView.RowFilter = filter
            Else
                dt.DefaultView.RowFilter = ""
            End If
        End If

    End Sub
    Private Sub btnadd_Click(sender As Object, e As EventArgs) Handles btnadd.Click

        Dim con As New MySqlConnection(connectionString)

        Dim borrowerType As String = ""
        Dim middleName As String = txtmname.Text.Trim()

        If rbstudent.Checked Then
            borrowerType = "Student"
        ElseIf rbteacher.Checked Then
            borrowerType = "Teacher"
        End If

        If rbnone.Checked Then
            middleName = "N/A"
        End If

        If Pic1.Image Is Nothing Then
            MessageBox.Show("Please import an image for the borrower.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End If

        Dim ms As New IO.MemoryStream
        Pic1.Image.Save(ms, Pic1.Image.RawFormat)
        Dim arrImage() As Byte = ms.GetBuffer

        Try
            con.Open()
            Dim com As New MySqlCommand("INSERT INTO `borrower_tbl` (Borrower, FirstName, MiddleName, LastName, LRN, ContactNumber, Department, Grade, Section, Strand, Image) VALUES (@Borrower, @FirstName, @MiddleName, @LastName, @LRN, @ContactNumber, @Department, @Grade, @Section, @Strand, @Image)", con)

            com.Parameters.AddWithValue("@Borrower", borrowerType)
            com.Parameters.AddWithValue("@FirstName", txtfname.Text.Trim())
            com.Parameters.AddWithValue("@MiddleName", middleName)
            com.Parameters.AddWithValue("@LastName", txtlname.Text.Trim())
            com.Parameters.AddWithValue("@LRN", txtlrn.Text.Trim())
            com.Parameters.AddWithValue("@ContactNumber", txtcontactnumber.Text.Trim())
            com.Parameters.AddWithValue("@Department", cbdepartment.Text.Trim())
            com.Parameters.AddWithValue("@Grade", cbgrade.Text.Trim())
            com.Parameters.AddWithValue("@Section", cbsection.Text.Trim())
            com.Parameters.AddWithValue("@Strand", cbstrand.Text.Trim())
            com.Parameters.Add("@Image", MySqlDbType.LongBlob, arrImage.Length).Value = arrImage
            com.ExecuteNonQuery()


            MsgBox("Borrower added successfully!", vbInformation)
            Borrower_Load(sender, e)
            ClearFields()

        Catch ex As Exception
            MessageBox.Show("Error adding borrower: " & ex.Message)
        Finally
            con.Close()
        End Try

    End Sub


    Private Sub btnedit_Click(sender As Object, e As EventArgs) Handles btnedit.Click

        If DataGridView1.SelectedRows.Count = 0 Then
            MessageBox.Show("Please select a record to edit.")
            Return
        End If

        Dim selectedID As Integer = Convert.ToInt32(DataGridView1.SelectedRows(0).Cells("ID").Value)

        Dim con As New MySqlConnection(connectionString)
        Dim arrImage() As Byte = Nothing
        Dim borrowerType As String = ""
        Dim middleName As String = txtmname.Text.Trim()

        If rbstudent.Checked Then
            borrowerType = "Student"
        ElseIf rbteacher.Checked Then
            borrowerType = "Teacher"
        End If

        If rbnone.Checked Then
            middleName = "N/A"
        End If

        If Pic1.Image IsNot Nothing Then
            Dim ms As New IO.MemoryStream
            Pic1.Image.Save(ms, Pic1.Image.RawFormat)
            arrImage = ms.GetBuffer
        End If

        Try
            con.Open()
            Dim com As New MySqlCommand("UPDATE `borrower_tbl` SET Borrower=@Borrower, FirstName=@FirstName, MiddleName=@MiddleName, LastName=@LastName, LRN=@LRN, ContactNumber=@ContactNumber, Department=@Department, Grade=@Grade, Section=@Section, Strand=@Strand, Image=@Image WHERE ID=@ID", con)

            com.Parameters.AddWithValue("@Borrower", borrowerType)
            com.Parameters.AddWithValue("@FirstName", txtfname.Text.Trim())
            com.Parameters.AddWithValue("@MiddleName", middleName)
            com.Parameters.AddWithValue("@LastName", txtlname.Text.Trim())
            com.Parameters.AddWithValue("@LRN", txtlrn.Text.Trim())
            com.Parameters.AddWithValue("@ContactNumber", txtcontactnumber.Text.Trim())
            com.Parameters.AddWithValue("@Department", cbdepartment.Text.Trim())
            com.Parameters.AddWithValue("@Grade", cbgrade.Text.Trim())
            com.Parameters.AddWithValue("@Section", cbsection.Text.Trim())
            com.Parameters.AddWithValue("@Strand", cbstrand.Text.Trim())
            com.Parameters.Add("@Image", MySqlDbType.LongBlob, If(arrImage Is Nothing, 0, arrImage.Length)).Value = If(arrImage Is Nothing, DBNull.Value, arrImage)
            com.Parameters.AddWithValue("@ID", selectedID)
            com.ExecuteNonQuery()

            MsgBox("Borrower updated successfully!", vbInformation)
            Borrower_Load(sender, e)
            ClearFields()

        Catch ex As Exception
            MessageBox.Show("Error updating borrower: " & ex.Message)
        Finally
            con.Close()
        End Try

    End Sub


    Private Sub btndelete_Click(sender As Object, e As EventArgs) Handles btndelete.Click


        If DataGridView1.SelectedRows.Count > 0 Then

            Dim dialogResult As DialogResult = MessageBox.Show("Are you sure you want to delete this borrower?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)

            If dialogResult = DialogResult.Yes Then

                Dim con As New MySqlConnection(connectionString)
                Dim selectedRow As DataGridViewRow = DataGridView1.SelectedRows(0)
                Dim ID As Integer = CInt(selectedRow.Cells("ID").Value)

                Try
                    con.Open()
                    Dim delete As New MySqlCommand("DELETE FROM `borrower_tbl` WHERE `ID` = @id", con)
                    delete.Parameters.AddWithValue("@id", ID)
                    delete.ExecuteNonQuery()


                    MsgBox("Borrower deleted successfully.", vbInformation)
                    Borrower_Load(sender, e)
                    ClearFields()


                    Dim count As New MySqlCommand("SELECT COUNT(*) FROM `borrower_tbl`", con)
                    Dim rowCount As Long = CLng(count.ExecuteScalar())

                    If rowCount = 0 Then

                        Dim reset As New MySqlCommand("ALTER TABLE `borrower_tbl` AUTO_INCREMENT = 1", con)
                        reset.ExecuteNonQuery()

                    End If

                Catch ex As Exception
                    MsgBox(ex.Message, vbCritical)
                End Try
            End If

        End If

    End Sub


    Private Sub btnclear_Click(sender As Object, e As EventArgs) Handles btnclear.Click
        ClearFields()
    End Sub



    Private Sub DataGridView1_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellClick

        If e.RowIndex >= 0 Then
            Dim row As DataGridViewRow = DataGridView1.Rows(e.RowIndex)
            Dim borrowerType As String = row.Cells("Borrower").Value.ToString()

            If borrowerType = "Student" Then
                rbstudent.Checked = True
            ElseIf borrowerType = "Teacher" Then
                rbteacher.Checked = True
            End If

            txtfname.Text = row.Cells("FirstName").Value.ToString()

            Dim middleName As String = row.Cells("MiddleName").Value.ToString()
            If middleName.Trim().ToUpper() = "N/A" Then
                rbnone.Checked = True
                txtmname.Text = ""
            Else
                rbnone.Checked = False
                txtmname.Text = middleName
            End If

            txtlname.Text = row.Cells("LastName").Value.ToString()
            txtlrn.Text = row.Cells("LRN").Value.ToString()
            txtcontactnumber.Text = row.Cells("ContactNumber").Value.ToString()
            cbdepartment.Text = row.Cells("Department").Value.ToString()
            cbgrade.Text = row.Cells("Grade").Value.ToString()
            cbsection.Text = row.Cells("Section").Value.ToString()
            cbstrand.Text = row.Cells("Strand").Value.ToString()

            If row.Cells("Image").Value IsNot DBNull.Value Then
                Dim arrImage() As Byte = CType(row.Cells("Image").Value, Byte())
                Dim ms As New IO.MemoryStream(arrImage)
                Pic1.Image = Image.FromStream(ms)
                Pic1.SizeMode = PictureBoxSizeMode.StretchImage
            Else
                Pic1.Image = Nothing
            End If
        End If

    End Sub



    Private Sub cbdepartment_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbdepartment.SelectedIndexChanged

        cbgrade.Enabled = False
        cbgrade.SelectedIndex = -1
        cbsection.Enabled = False
        cbsection.SelectedIndex = -1
        cbstrand.Enabled = False
        cbstrand.SelectedIndex = -1


        cbsection.Visible = True
        lblsection.Visible = True
        cbstrand.Visible = True
        lblstrand.Visible = True

        cbstrand.Location = New Point(682, 285)
        lblstrand.Location = New Point(682, 266)

        If cbdepartment.SelectedIndex <> -1 Then
            cbgrade.Enabled = True
            If cbdepartment.Text.Trim().ToLower() = "jhs" Then
                Dim con As New MySqlConnection(connectionString)
                Dim dt As New DataTable
                Try
                    con.Open()
                    Dim adap As New MySqlDataAdapter("SELECT ID, Grade FROM `grade_tbl` WHERE Grade IN ('7', '8', '9', '10')", con)
                    adap.Fill(dt)
                    cbgrade.DataSource = dt
                    cbgrade.DisplayMember = "Grade"
                    cbgrade.ValueMember = "ID"
                    cbstrand.Enabled = False
                    cbstrand.SelectedIndex = -1
                    cbstrand.Visible = False
                    lblstrand.Visible = False

                    cbsection.Enabled = True
                    cbsection.Visible = True
                    lblsection.Visible = True

                Catch ex As Exception
                    MessageBox.Show("Error filtering grades: " & ex.Message)
                Finally
                    con.Close()
                End Try
            ElseIf cbdepartment.Text.Trim().ToLower() = "shs" Then
                Dim con As New MySqlConnection(connectionString)
                Dim dt As New DataTable
                Try
                    con.Open()
                    Dim adap As New MySqlDataAdapter("SELECT ID, Grade FROM `grade_tbl` WHERE Grade IN ('11', '12')", con)
                    adap.Fill(dt)
                    cbgrade.DataSource = dt
                    cbgrade.DisplayMember = "Grade"
                    cbgrade.ValueMember = "ID"

                    cbsection.Enabled = False
                    cbsection.Visible = False
                    lblsection.Visible = False
                    cbsection.SelectedIndex = -1

                    cbstrand.Location = New Point(682, 216)
                    lblstrand.Location = New Point(682, 197)

                    cbstrand.Enabled = True
                    cbstrand.Visible = True
                    lblstrand.Visible = True

                Catch ex As Exception
                    MessageBox.Show("Error filtering grades: " & ex.Message)
                Finally
                    con.Close()
                End Try
            Else
                cbgrade.Enabled = False
                cbsection.Enabled = False
                cbstrand.Enabled = False
                cbgrade.SelectedIndex = -1
                cbsection.SelectedIndex = -1
                cbstrand.SelectedIndex = -1
            End If
        End If
    End Sub

    Private Sub cbsection_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbsection.SelectedIndexChanged

        If cbsection.SelectedIndex <> -1 AndAlso cbgrade.SelectedIndex <> -1 Then
            cbstrand.Enabled = True
        Else
            cbstrand.Enabled = False
        End If
    End Sub

    Private Sub cbgrade_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbgrade.SelectedIndexChanged

        If cbgrade.SelectedIndex <> -1 Then

            cbsection.Enabled = True
        Else

            cbsection.Enabled = False
            cbsection.SelectedIndex = -1
        End If

    End Sub

    Private Sub rbstudent_CheckedChanged(sender As Object, e As EventArgs) Handles rbstudent.CheckedChanged

        If rbstudent.Checked Then
            lblborrowertype.Text = "LRN:"

            cbdepartment.Enabled = True
            cbgrade.Enabled = False
            cbsection.Enabled = False
            cbstrand.Enabled = False


        End If

    End Sub

    Private Sub rbteacher_CheckedChanged(sender As Object, e As EventArgs) Handles rbteacher.CheckedChanged

        If rbteacher.Checked Then
            lblborrowertype.Text = "Employee no.:"

            cbgrade.Enabled = False
            cbsection.Enabled = False
            cbstrand.Enabled = False
            cbdepartment.Enabled = False

            cbgrade.SelectedIndex = -1
            cbsection.SelectedIndex = -1
            cbstrand.SelectedIndex = -1
        End If

    End Sub

    Private Sub rbnone_CheckedChanged(sender As Object, e As EventArgs) Handles rbnone.CheckedChanged

        If rbnone.Checked Then
            txtmname.Enabled = False
            txtmname.Text = ""
        Else
            txtmname.Enabled = True
        End If
    End Sub

    Private Sub ClearFields()

        txtfname.Clear()
        txtmname.Clear()
        txtlname.Clear()
        txtlrn.Clear()
        txtcontactnumber.Clear()
        DataGridView1.ClearSelection()

        cbdepartment.DataSource = Nothing
        cbdepartment.Enabled = False
        cbgrade.DataSource = Nothing
        cbsection.DataSource = Nothing
        cbstrand.DataSource = Nothing
        Pic1.Image = Nothing

        cbdepts()
        cbgradee()
        cbsecs()
        cbstrandd()

        rbstudent.Checked = False
        rbteacher.Checked = False
        rbnone.Checked = False
        txtmname.Enabled = True
        lblborrowertype.Text = "LRN:"
    End Sub

End Class