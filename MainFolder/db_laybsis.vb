Imports MySql.Data.MySqlClient

Module db_laybsis
    Public con As MySqlConnection


    Public Sub Koneksyon()
        Try

            con = New MySqlConnection(GlobalVarsModule.connectionString)
            con.Open()


        Catch ex As Exception
            MessageBox.Show("CONNECTION FAILED: " & ex.Message, "INFORMATION", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Application.Exit()
        Finally

        End Try
    End Sub
End Module