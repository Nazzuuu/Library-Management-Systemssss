Imports System.IO
Imports System.Security.Cryptography
Imports System.Text
Imports MySql.Data.MySqlClient
Imports TheArtOfDevHtmlRenderer.Adapters
Imports Windows.Win32.System

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
            MsgBox("Please import an image first.", vbExclamation, "No Image")
            Exit Sub
        End If


        If String.IsNullOrWhiteSpace(user) OrElse String.IsNullOrWhiteSpace(pass) OrElse String.IsNullOrWhiteSpace(email) Then
            MsgBox("Please fill in all the required fields (Username, Password, Email).", vbExclamation, "Missing Information")
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
            Else

                MsgBox("Please select a gender.", vbExclamation, "Missing Information")
                Exit Sub
            End If
            com.Parameters.AddWithValue("@gender", gender)


            Dim ms As New IO.MemoryStream()
            PictureBox2.Image.Save(ms, PictureBox2.Image.RawFormat)
            Dim arrImage() As Byte = ms.ToArray()

            com.Parameters.Add("@Image", MySqlDbType.LongBlob, arrImage.Length).Value = arrImage

            com.ExecuteNonQuery()
            MsgBox("Staff member added successfully!", vbInformation)

            RefreshDataGrid()
            clearfields()

        Catch ex As Exception
            MsgBox(ex.Message & vbCrLf & "Details: " & ex.StackTrace, vbCritical, "Database Error")
        Finally
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
        End Try
    End Sub


    Public Sub RefreshDataGrid()
        Dim con As New MySqlConnection(connectionString)
        Dim com As String = "SELECT `ID`, `Username`, `Password`, `Email`, `ContactNumber`, `Gender`, `Image` FROM `user_staff_tbl`"
        Dim adapp As New MySqlDataAdapter(com, con)
        Dim dt As New DataSet
        Try
            adapp.Fill(dt, "INFO")
            DataGridView1.DataSource = dt.Tables("INFO")
        Catch ex As Exception
            MsgBox("Error refreshing data: " & ex.Message, vbCritical)
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

        If DataGridView1.CurrentRow Is Nothing Then
            MsgBox("Please select a staff member to edit.", vbExclamation)
            Exit Sub
        End If


        Dim originalID As Integer = CInt(DataGridView1.CurrentRow.Cells("ID").Value)

        Dim user As String = txtusername.Text.Trim()
        Dim pass As String = txtpassword.Text.Trim()
        Dim email As String = txtemail.Text.Trim()
        Dim contact As String = txtcontactnumber.Text.Trim()
        Dim gender As String = ""
        Dim arrImage() As Byte = Nothing

        If String.IsNullOrWhiteSpace(user) OrElse String.IsNullOrWhiteSpace(pass) OrElse String.IsNullOrWhiteSpace(email) Then
            MsgBox("Please fill in all required fields.", vbExclamation, "Missing Information")
            Exit Sub
        End If

        If rbmale.Checked Then
            gender = "Male"
        ElseIf rbfemale.Checked Then
            gender = "Female"
        Else
            MsgBox("Please select a gender.", vbExclamation, "Missing Information")
            Exit Sub
        End If

        Dim con As New MySqlConnection(connectionString)
        Try
            con.Open()


            Dim checkCom As New MySqlCommand("SELECT COUNT(*) FROM `user_staff_tbl` WHERE (`Username` = @username OR `Email` = @email) AND `ID` <> @id", con)
            checkCom.Parameters.AddWithValue("@username", user)
            checkCom.Parameters.AddWithValue("@email", email)
            checkCom.Parameters.AddWithValue("@id", originalID)
            Dim count As Integer = Convert.ToInt32(checkCom.ExecuteScalar())

            If count > 0 Then
                MsgBox("The username or email already exists. Please use a different one.", vbExclamation, "Duplication Not Allowed")
                Exit Sub
            End If


            Dim getPassCom As New MySqlCommand("SELECT `Password` FROM `user_staff_tbl` WHERE `ID` = @id", con)
            getPassCom.Parameters.AddWithValue("@id", originalID)
            Dim oldPass As String = getPassCom.ExecuteScalar().ToString()

            Dim updateCom As New MySqlCommand("UPDATE `user_staff_tbl` SET `Username` = @username, `Password` = @password, `Email` = @email, `ContactNumber` = @contact, `Gender` = @gender, `Image` = @image WHERE `ID` = @id", con)


            updateCom.Parameters.AddWithValue("@username", user)

            If pass = "********" Then
                updateCom.Parameters.AddWithValue("@password", oldPass)
            Else
                updateCom.Parameters.AddWithValue("@password", pass)
            End If
            updateCom.Parameters.AddWithValue("@email", email)
            updateCom.Parameters.AddWithValue("@contact", contact)
            updateCom.Parameters.AddWithValue("@gender", gender)
            updateCom.Parameters.AddWithValue("@id", originalID)


            If PictureBox2.Image IsNot Nothing Then
                Using ms As New IO.MemoryStream()
                    PictureBox2.Image.Save(ms, PictureBox2.Image.RawFormat)
                    arrImage = ms.ToArray()
                    updateCom.Parameters.Add("@image", MySqlDbType.LongBlob, arrImage.Length).Value = arrImage
                End Using
            Else

                Dim getImageCom As New MySqlCommand("SELECT `Image` FROM `user_staff_tbl` WHERE `ID` = @id", con)
                getImageCom.Parameters.AddWithValue("@id", originalID)
                Dim oldImage As Object = getImageCom.ExecuteScalar()

                If Not IsDBNull(oldImage) Then
                    arrImage = DirectCast(oldImage, Byte())
                    updateCom.Parameters.Add("@image", MySqlDbType.LongBlob, arrImage.Length).Value = arrImage
                Else
                    updateCom.Parameters.AddWithValue("@image", DBNull.Value)
                End If
            End If

            updateCom.ExecuteNonQuery()
            MsgBox("Staff member updated successfully!", vbInformation)
            RefreshDataGrid()
            clearfields()

        Catch ex As Exception
            MsgBox("Error updating staff member: " & ex.Message, vbCritical)
        Finally
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
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



    Private Sub DataGridView1_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellClick

        If e.RowIndex >= 0 Then
            Dim row As DataGridViewRow = DataGridView1.Rows(e.RowIndex)

            txtusername.Text = If(IsDBNull(row.Cells("Username").Value), String.Empty, row.Cells("Username").Value.ToString())
            txtemail.Text = If(IsDBNull(row.Cells("Email").Value), String.Empty, row.Cells("Email").Value.ToString())
            txtcontactnumber.Text = If(IsDBNull(row.Cells("ContactNumber").Value), String.Empty, row.Cells("ContactNumber").Value.ToString())


            txtpassword.Text = "********"
            txtpassword.UseSystemPasswordChar = True
            CheckBox1.Checked = False

            Dim gender As String = If(IsDBNull(row.Cells("Gender").Value), String.Empty, row.Cells("Gender").Value.ToString())
            rbmale.Checked = (gender = "Male")
            rbfemale.Checked = (gender = "Female")

            If Not IsDBNull(row.Cells("Image").Value) Then
                Dim imgData As Byte() = Nothing
                Try
                    imgData = CType(row.Cells("Image").Value, Byte())
                    Using ms As New MemoryStream(imgData)
                        If ms.Length > 0 Then
                            PictureBox2.Image = Image.FromStream(ms)
                            PictureBox2.SizeMode = PictureBoxSizeMode.StretchImage
                        Else
                            PictureBox2.Image = Nothing
                        End If
                    End Using
                Catch ex As Exception
                    MsgBox("Error loading image: " & ex.Message, vbCritical)
                    PictureBox2.Image = Nothing
                End Try
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
        CheckBox1.Checked = False
        rbmale.Checked = False
        rbfemale.Checked = False

        If PictureBox2.Image IsNot Nothing Then
            PictureBox2.Image.Dispose()
            PictureBox2.Image = Nothing
        End If

        DataGridView1.ClearSelection()
    End Sub

    Private Sub txtusername_KeyDown(sender As Object, e As KeyEventArgs) Handles txtusername.KeyDown

        If e.Control AndAlso (e.KeyCode = Keys.V Or e.KeyCode = Keys.C Or e.KeyCode = Keys.X) Then
            e.SuppressKeyPress = True
        End If

    End Sub

    Private Sub txtpassword_KeyDown(sender As Object, e As KeyEventArgs) Handles txtpassword.KeyDown

        If e.Control AndAlso (e.KeyCode = Keys.V Or e.KeyCode = Keys.C Or e.KeyCode = Keys.X) Then
            e.SuppressKeyPress = True
        End If

    End Sub

    Private Sub txtemail_KeyDown(sender As Object, e As KeyEventArgs) Handles txtemail.KeyDown

        If e.Control AndAlso (e.KeyCode = Keys.V Or e.KeyCode = Keys.C Or e.KeyCode = Keys.X) Then
            e.SuppressKeyPress = True
        End If

    End Sub

    Private Sub txtcontactnumber_KeyDown(sender As Object, e As KeyEventArgs) Handles txtcontactnumber.KeyDown

        If e.Control AndAlso (e.KeyCode = Keys.V Or e.KeyCode = Keys.C Or e.KeyCode = Keys.X) Then
            e.SuppressKeyPress = True
        End If

        If e.KeyCode = Keys.Back Then
            RemoveHandler txtcontactnumber.TextChanged, AddressOf txtcontactnumber_TextChanged
        End If

    End Sub

    Private Sub txtcontactnumber_KeyPress(sender As Object, e As KeyPressEventArgs) Handles txtcontactnumber.KeyPress

        If Not Char.IsDigit(e.KeyChar) And Not Char.IsControl(e.KeyChar) Then
            e.Handled = True
        End If

    End Sub

    Private Sub txtemail_KeyPress(sender As Object, e As KeyPressEventArgs) Handles txtemail.KeyPress

        If e.KeyChar = " "c AndAlso String.IsNullOrEmpty(txtemail.Text) Then
            e.Handled = True


        End If

        If Not Char.IsLetterOrDigit(e.KeyChar) AndAlso Not Char.IsControl(e.KeyChar) AndAlso e.KeyChar <> "@" AndAlso e.KeyChar <> "." Then
            e.Handled = True
        End If


        If Char.IsWhiteSpace(e.KeyChar) Then
            e.Handled = True
        End If

    End Sub

    Private Sub txtsearch_KeyDown(sender As Object, e As KeyEventArgs) Handles txtsearch.KeyDown

        If e.Control AndAlso (e.KeyCode = Keys.V Or e.KeyCode = Keys.C Or e.KeyCode = Keys.X) Then
            e.SuppressKeyPress = True
        End If

    End Sub

    Private Sub txtcontactnumber_KeyUp(sender As Object, e As KeyEventArgs) Handles txtcontactnumber.KeyUp

        If e.KeyCode = Keys.Back Then
            AddHandler txtcontactnumber.TextChanged, AddressOf txtcontactnumber_TextChanged
        End If

    End Sub

    Private Sub txtcontactnumber_TextChanged(sender As Object, e As EventArgs) Handles txtcontactnumber.TextChanged

        Dim oreyjeynal As String = txtcontactnumber.Text

        If String.IsNullOrEmpty(oreyjeynal) Then
            Return
        End If

        If oreyjeynal.StartsWith("09") Then

        Else

            If oreyjeynal.Length > 0 Then
                txtcontactnumber.Clear()
                txtcontactnumber.Text = "09"
                txtcontactnumber.SelectionStart = 2
            End If
        End If

    End Sub

    ''pagod na ako mga bes''
End Class