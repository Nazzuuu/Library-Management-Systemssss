Imports MySql.Data.MySqlClient

Public Class ServerConnection

    Private Sub ServerConnection_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        txtserver.Text = My.Settings.Server
        txtusername.Text = My.Settings.Username
        txtpassword.Text = My.Settings.Password
        txtdatabase.Text = My.Settings.Database
    End Sub

    Private Sub btnconnect_Click(sender As Object, e As EventArgs) Handles btnconnect.Click
        Dim server As String = txtserver.Text.Trim()
        Dim username As String = txtusername.Text.Trim()
        Dim password As String = txtpassword.Text.Trim()
        Dim database As String = txtdatabase.Text.Trim()

        Dim testConnectionString As String =
            $"Server={server};Database={database};Uid={username};Pwd={password};"

        Try
            Using con As New MySqlConnection(testConnectionString)
                con.Open()
                MessageBox.Show("✅ Connection successful!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
                con.Close()
            End Using
        Catch ex As Exception
            MessageBox.Show("❌ Connection failed: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub btnsave_Click(sender As Object, e As EventArgs) Handles btnsave.Click
        My.Settings.Server = txtserver.Text.Trim()
        My.Settings.Username = txtusername.Text.Trim()
        My.Settings.Password = txtpassword.Text.Trim()
        My.Settings.Database = txtdatabase.Text.Trim()
        My.Settings.Save()

        ' 🔹 Refresh the global connection
        GlobalVarsModule.RefreshConnectionString()

        MessageBox.Show("✅ Settings saved successfully!", "Saved", MessageBoxButtons.OK, MessageBoxIcon.Information)

        Dim result = MessageBox.Show("Do you want to test the new connection?",
                                     "Test Connection", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
        If result = DialogResult.Yes Then
            btnconnect.PerformClick()
        End If
    End Sub

    Private Sub btncancel_Click(sender As Object, e As EventArgs) Handles btncancel.Click
        Me.Close()
    End Sub

End Class