Imports MySql.Data.MySqlClient

Module db_laybsis
    Public con As MySqlConnection

    ' 🔹 Open or reuse connection
    Public Sub Koneksyon()
        Try
            ' Refresh latest connection string before connecting
            GlobalVarsModule.RefreshConnectionString()

            If con IsNot Nothing AndAlso con.State = ConnectionState.Open Then Exit Sub

            con = New MySqlConnection(GlobalVarsModule.connectionString)
            con.Open()

        Catch ex As Exception
            MessageBox.Show("❌ CONNECTION FAILED: " & ex.Message,
                            "Database Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error)

            If MessageBox.Show("Do you want to open Server Settings to fix connection?",
                               "Connection Error", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
                Dim settingsForm As New ServerConnection()
                settingsForm.ShowDialog()
            Else
                Application.Exit()
            End If
        End Try
    End Sub

    ' 🔹 Check if connected
    Public Function IsConnected() As Boolean
        Try
            Return (con IsNot Nothing AndAlso con.State = ConnectionState.Open)
        Catch
            Return False
        End Try
    End Function

    ' 🔹 Close connection safely
    Public Sub CloseConnection()
        Try
            If con IsNot Nothing AndAlso con.State = ConnectionState.Open Then con.Close()
        Catch ex As Exception
            MessageBox.Show("Error closing connection: " & ex.Message,
                            "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        End Try
    End Sub

End Module