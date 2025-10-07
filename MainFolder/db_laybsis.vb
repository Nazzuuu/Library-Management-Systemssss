Imports MySql.Data.MySqlClient

' Filename: db_laybsis.vb
Module db_laybsis
    Public con As MySqlConnection

    ' TANGGALIN DITO ANG: Public connectionString As String = "..."
    ' Gagamitin na ang GlobalVarsModule.connectionString

    Public Sub Koneksyon()
        Try
            ' Dito gagamitin ang connection string mula sa GlobalVarsModule
            con = New MySqlConnection(GlobalVarsModule.connectionString)
            con.Open()

            ' (Tiyakin na naayos mo na ang logic sa ibang parts ng db_laybsis.vb kung gumagamit ito ng con.Close() / Messaging)

        Catch ex As Exception
            MessageBox.Show("CONNECTION FAILED: " & ex.Message, "INFORMATION", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Application.Exit()
        Finally
            ' Huwag i-close ang koneksyon dito kung may iba pang functions na gagamit nito. 
            ' Mas maganda kung sa bawat function na lang mag-open/close ng koneksyon.
            'con.Close()
        End Try
    End Sub
End Module