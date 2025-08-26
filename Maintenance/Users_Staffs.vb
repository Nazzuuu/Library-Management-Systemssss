Imports MySql.Data.MySqlClient
Imports System.Security.Cryptography
Imports System.Text
Imports System.IO

Public Class Users_Staffs

    Private Sub Users_Staffs_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        Dim con As New MySqlConnection(connectionString)
        Dim com As String = "SELECT `ID`, `Username`, `Password`, `Email`, `ContactNumber`, `Gender`, `Image` FROM `user_staff_tbl`"
        Dim adapp As New MySqlDataAdapter(com, con)
        Dim dt As New DataSet

        adapp.Fill(dt, "INFO")
        DataGridView1.DataSource = dt.Tables("INFO")

        DataGridView1.Columns("ID").Visible = False
        DataGridView1.EnableHeadersVisualStyles = False
        DataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(207, 58, 109)
        DataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White

        AddHandler DataGridView1.CellFormatting, AddressOf Me.DataGridView1_CellFormatting

    End Sub

    Private Sub DataGridView1_CellFormatting(sender As Object, e As DataGridViewCellFormattingEventArgs)

        If DataGridView1.Columns(e.ColumnIndex).Name = "Password" AndAlso e.Value IsNot Nothing Then
            Dim passText As String = e.Value.ToString()
            Dim formattedString As String = New String("*"c, passText.Length)
            e.Value = formattedString
            e.FormattingApplied = True
        End If
    End Sub

    Private Sub btnimport_Click(sender As Object, e As EventArgs) Handles btnimport.Click


        If PictureBox2.Image IsNot Nothing Then
            PictureBox2.Image.Dispose()
        End If


        If OpenFileDialog1.ShowDialog <> DialogResult.Cancel Then

            PictureBox2.Image = Image.FromFile(OpenFileDialog1.FileName)
            PictureBox2.SizeMode = PictureBoxSizeMode.StretchImage
        End If
    End Sub

    Private Sub btnclearr_Click(sender As Object, e As EventArgs) Handles btnclearr.Click

        If PictureBox2.Image IsNot Nothing Then
            PictureBox2.Image.Dispose()
            PictureBox2.Image = Nothing
        End If

    End Sub

    Private Sub btnadd_Click(sender As Object, e As EventArgs) Handles btnadd.Click

        Dim user As String = txtusername.Text.Trim
        Dim pass As String = txtpassword.Text.Trim
        Dim email As String = txtemail.Text.Trim
        Dim contact As String = txtcontactnumber.Text.Trim

        If PictureBox2.Image Is Nothing Then
            MsgBox("Please import an image first.", vbExclamation)
            Exit Sub
        End If

        Dim con As New MySqlConnection(connectionString)
        Try
            con.Open()
            Dim checkCom As New MySqlCommand("SELECT COUNT(*) FROM `user_staff_tbl` WHERE `Username` = @username OR `Email` = @email", con)
            checkCom.Parameters.AddWithValue("@username", user)
            checkCom.Parameters.AddWithValue("@email", email)
            Dim count As Integer = Convert.ToInt32(checkCom.ExecuteScalar())

            If count > 0 Then
                MsgBox("The username or email already exists. Please use a different one.", vbExclamation, "Duplication Not Allowed")
                Exit Sub
            End If

            Dim com As New MySqlCommand("INSERT INTO `user_staff_tbl`(`Username`, `Password`, `Email`, `ContactNumber`, `Gender`, `Image`) VALUES (@username, @password, @email, @contact, @gender, @image)", con)
            com.Parameters.AddWithValue("@username", user)
            com.Parameters.AddWithValue("@password", pass)
            com.Parameters.AddWithValue("@email", email)
            com.Parameters.AddWithValue("@contact", contact)

            Dim gender As String = ""
            If rbmale.Checked Then
                gender = "Male"
            ElseIf rbfemale.Checked Then
                gender = "Female"
            End If
            com.Parameters.AddWithValue("@gender", gender)

            Dim newImageBytes As Byte() = Nothing
            If PictureBox2.Image IsNot Nothing Then
                Using ms As New IO.MemoryStream()
                    Using tempBitmap As New Bitmap(PictureBox2.Image)
                        tempBitmap.Save(ms, Imaging.ImageFormat.Jpeg)
                        newImageBytes = ms.ToArray()
                    End Using
                End Using
            End If

            com.ExecuteNonQuery()
            MsgBox("Staff member added successfully!", vbInformation)
            Users_Staffs_Load(sender, e)
        Catch ex As Exception
            MsgBox(ex.Message, vbCritical)
        Finally
            con.Close()
            clearfields()
        End Try
    End Sub

    Private Sub CheckBox1_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox1.CheckedChanged
        If CheckBox1.Checked = True Then
            txtpassword.UseSystemPasswordChar = False
        Else
            txtpassword.UseSystemPasswordChar = True
        End If
    End Sub

    Private Sub txtpassword_TextChanged(sender As Object, e As EventArgs) Handles txtpassword.TextChanged
        If CheckBox1.Checked = True Then
            txtpassword.UseSystemPasswordChar = False
        Else
            txtpassword.UseSystemPasswordChar = True
        End If
    End Sub


    Private Sub btnedit_Click(sender As Object, e As EventArgs) Handles btnedit.Click

        Dim user As String = txtusername.Text.Trim
        Dim pass As String = txtpassword.Text.Trim
        Dim email As String = txtemail.Text.Trim
        Dim contact As String = txtcontactnumber.Text.Trim

        If String.IsNullOrEmpty(user) Then
            MsgBox("Please select a staff member to edit.", vbExclamation)
            Exit Sub
        End If

        Dim con As New MySqlConnection(connectionString)
        Try
            con.Open()
            Dim updateCom As New MySqlCommand("UPDATE `user_staff_tbl` SET `Username` = @username, `Password` = @password, `Email` = @email, `ContactNumber` = @contact, `Gender` = @gender, `Image` = @image WHERE `Username` = @originalUsername", con)


            Dim originalUsername As String = DataGridView1.CurrentRow.Cells("Username").Value.ToString()

            updateCom.Parameters.AddWithValue("@username", user)
            updateCom.Parameters.AddWithValue("@password", pass)
            updateCom.Parameters.AddWithValue("@email", email)
            updateCom.Parameters.AddWithValue("@contact", contact)

            Dim gender As String = ""
            If rbmale.Checked Then
                gender = "Male"
            ElseIf rbfemale.Checked Then
                gender = "Female"
            End If
            updateCom.Parameters.AddWithValue("@gender", gender)


            If PictureBox2.Image IsNot Nothing Then
                Using ms As New IO.MemoryStream()
                    Try
                        PictureBox2.Image.Save(ms, PictureBox2.Image.RawFormat)
                        Dim img As Byte() = ms.ToArray()
                        updateCom.Parameters.AddWithValue("@image", img)
                    Catch ex As Exception
                        MsgBox("Error saving image: " & ex.Message, vbCritical)
                        updateCom.Parameters.AddWithValue("@image", DBNull.Value)
                    End Try
                End Using
            Else

                updateCom.Parameters.AddWithValue("@image", DBNull.Value)
            End If

            updateCom.Parameters.AddWithValue("@originalUsername", originalUsername)

            updateCom.ExecuteNonQuery()
            MsgBox("Staff member updated successfully!", vbInformation)
            Users_Staffs_Load(sender, e)
        Catch ex As Exception
            MsgBox(ex.Message, vbCritical)
        Finally
            con.Close()
            clearfields()
        End Try
    End Sub



    Private Sub btndelete_Click(sender As Object, e As EventArgs) Handles btndelete.Click
        Dim user As String = txtusername.Text.Trim

        If MsgBox("Are you sure you want to delete this staff member?", vbQuestion + vbYesNo) = vbYes Then
            Dim con As New MySqlConnection(connectionString)
            Try
                con.Open()
                Dim com As New MySqlCommand("DELETE FROM `user_staff_tbl` WHERE `Username` = @username", con)
                com.Parameters.AddWithValue("@username", user)
                com.ExecuteNonQuery()

                MsgBox("Staff member deleted successfully!", vbInformation)

                Users_Staffs_Load(sender, e)
            Catch ex As Exception
                MsgBox(ex.Message, vbCritical)
            Finally
                con.Close()
                ResetAutoIncrement()
                clearfields()
            End Try
        End If
    End Sub

    Private Sub ResetAutoIncrement()
        Dim con As New MySqlConnection(connectionString)
        Try
            con.Open()
            Dim com As New MySqlCommand("ALTER TABLE `user_staff_tbl` AUTO_INCREMENT = 1", con)
            com.ExecuteNonQuery()
        Catch ex As Exception
            MsgBox("Error resetting auto-increment: " & ex.Message, vbCritical)
        Finally
            con.Close()
        End Try
    End Sub



    Private Sub DataGridView1_CellClick(sender As Object, e As DataGridViewCellEventArgs)
        If e.RowIndex >= 0 Then
            Dim row = DataGridView1.Rows(e.RowIndex)

            txtusername.Text = row.Cells("Username").Value.ToString
            txtemail.Text = row.Cells("Email").Value.ToString
            txtcontactnumber.Text = row.Cells("ContactNumber").Value.ToString


            Dim password = row.Cells("Password").Value.ToString
            txtpassword.Text = password
            txtpassword.UseSystemPasswordChar = False
            CheckBox1.Checked = True


            Dim gender = row.Cells("Gender").Value.ToString
            If gender = "Male" Then
                rbmale.Checked = True
            ElseIf gender = "Female" Then
                rbfemale.Checked = True
            End If


            If Not IsDBNull(row.Cells("Image").Value) Then
                Dim imgData = DirectCast(row.Cells("Image").Value, Byte())
                Using ms As New MemoryStream(imgData)
                    Try

                        If ms.Length > 0 Then
                            PictureBox2.Image = Image.FromStream(ms)
                            PictureBox2.SizeMode = PictureBoxSizeMode.StretchImage
                        Else
                            PictureBox2.Image = Nothing
                        End If
                    Catch ex As Exception

                        MsgBox("Error loading image: " & ex.Message, vbCritical)
                        PictureBox2.Image = Nothing
                    End Try
                End Using
            Else
                PictureBox2.Image = Nothing
            End If
        End If
    End Sub



    Private Sub txtsearch_TextChanged(sender As Object, e As EventArgs) Handles txtsearch.TextChanged
        Dim dt As DataTable = DirectCast(DataGridView1.DataSource, DataTable)
        If dt IsNot Nothing Then
            If txtsearch.Text.Trim() <> "" Then
                Dim filter As String = String.Format("Username LIKE '*{0}*' OR Email LIKE '*{0}*' OR ContactNumber LIKE '*{0}*'", txtsearch.Text.Trim())
                dt.DefaultView.RowFilter = filter
            Else
                dt.DefaultView.RowFilter = ""
            End If
        End If
    End Sub

    Private Sub btnclear_Click(sender As Object, e As EventArgs) Handles btnclear.Click
        clearfields()
    End Sub

    Public Sub clearfields()
        txtusername.Text = ""
        txtpassword.Text = ""
        txtemail.Text = ""
        txtcontactnumber.Text = ""
        rbmale.Checked = False
        rbfemale.Checked = False
        If PictureBox2.Image IsNot Nothing Then
            PictureBox2.Image.Dispose()
            PictureBox2.Image = Nothing
        End If
    End Sub

End Class