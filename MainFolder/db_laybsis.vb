Imports MySql.Data.MySqlClient

Module db_laybsis

    Public cown As MySqlConnection

    Public connectionString As String = "server=localhost;userid=root;database=laybsis_dbs;"



    Sub koneksyon()

        Try
            cown = New MySqlConnection(connectionString)
            cown.Open()

        Catch ex As Exception
            MessageBox.Show("CONNECTING FAILED.", "INFORMATION", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Application.Exit()
        Finally
            cown.Close()
        End Try
    End Sub

End Module
