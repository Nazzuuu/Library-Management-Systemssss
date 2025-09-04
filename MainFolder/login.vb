Imports MySql.Data.MySqlClient
Public Class login

    Public Sub clear()
        txtpass.Text = ""
        txtuser.Text = ""
    End Sub


    Private Sub btnlogin_Click(sender As Object, e As EventArgs) Handles btnlogin.Click

        Dim User As String = txtuser.Text.Trim()
        Dim Pass As String = txtpass.Text.Trim()


        If User = "Librarian01" And Pass = "Librarian123" Then

            MessageBox.Show("Librarian successfully logged in.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information)

            MainForm.Show()
            MainForm.lbl_currentuser.Text = "Librarian"
            Me.Hide()
            clear()

            Return
        End If


        Dim con As New MySqlConnection(connectionString)
        Dim com As New MySqlCommand("SELECT COUNT(*) FROM user_staff_tbl WHERE Username = @username AND Password = @password", con)

        com.Parameters.AddWithValue("@username", User)
        com.Parameters.AddWithValue("@password", Pass)

        Try
            con.Open()

            Dim userCount As Integer = Convert.ToInt32(com.ExecuteScalar())

            If userCount > 0 Then

                MessageBox.Show("Staff successfully logged in.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information)

                MainForm.Show()
                MainForm.lbl_currentuser.Text = "Staff"

                MainForm.AuthorMaintenanceToolStripMenuItem.Visible = False
                MainForm.GenreMaintenanceToolStripMenuItem.Visible = False
                MainForm.SupplierMaintenanceToolStripMenuItem.Visible = False
                MainForm.PublisherMaintenanceToolStripMenuItem.Visible = False
                MainForm.LanguageToolStripMenuItem.Visible = False
                MainForm.CategoryToolStripMenuItem.Visible = False
                MainForm.AcquisitionToolStripMenuItem.Visible = False
                MainForm.AccessionToolStripMenuItem.Visible = False
                MainForm.ReportsToolStripMenuItem1.Visible = False
                MainForm.UserMaintenanceToolStripMenuItem.Visible = False
                MainForm.Audit_Trail.Visible = False

                Me.Hide()
                clear()

            Else

                MessageBox.Show("Invalid username or password.", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End If

        Catch ex As Exception
            MsgBox(ex.Message, vbCritical)
        Finally
            con.Close()
        End Try
    End Sub




    Private Sub login_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        TopMost = True
        koneksyon()



    End Sub

    Private Sub txtpass_TextChanged(sender As Object, e As EventArgs) Handles txtpass.TextChanged
        If CheckBox1.Checked = True Then
            txtpass.UseSystemPasswordChar = False
        Else
            txtpass.UseSystemPasswordChar = True
        End If
    End Sub

    Private Sub CheckBox1_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox1.CheckedChanged

        If CheckBox1.Checked = True Then
            txtpass.UseSystemPasswordChar = False
        Else
            txtpass.UseSystemPasswordChar = True
        End If

    End Sub

    Private Sub txtpass_KeyDown(sender As Object, e As KeyEventArgs) Handles txtpass.KeyDown

        If e.KeyCode = Keys.Enter Then
            btnlogin_Click(sender, e)
            e.Handled = True
        End If

    End Sub

    Private Sub btnlogin_KeyDown(sender As Object, e As KeyEventArgs) Handles btnlogin.KeyDown

        If e.KeyCode = Keys.Enter Then
            btnlogin_Click(sender, e)
            e.Handled = True
        End If

    End Sub

    Private Sub btnlogin_MouseHover(sender As Object, e As EventArgs) Handles btnlogin.MouseHover

        Cursor = Cursors.Hand

    End Sub

    Private Sub btnlogin_MouseLeave(sender As Object, e As EventArgs) Handles btnlogin.MouseLeave

        Cursor = Cursors.Default

    End Sub

    Private Sub CheckBox1_MouseHover(sender As Object, e As EventArgs) Handles CheckBox1.MouseHover

        Cursor = Cursors.Hand

    End Sub

    Private Sub CheckBox1_MouseLeave(sender As Object, e As EventArgs) Handles CheckBox1.MouseLeave

        Cursor = Cursors.Default

    End Sub
End Class