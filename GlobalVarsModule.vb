Imports MySql.Data.MySqlClient

Module GlobalVarsModule

    Public connectionString As String = "Server=localhost;Database=laybsis_dbs;Uid=root;Pwd="

    Public CurrentUserID As String = ""
    Public CurrentUserRole As String = "Guest"

    Public CurrentBorrowerID As String = ""
    Public CurrentBorrowerType As String = ""

    Public GlobalUsername As String = ""
    Public GlobalRole As String = ""

    Public Function GetCleanCurrentBorrowerID() As String
        Dim idTrimmed As String = CurrentBorrowerID.Trim()
        Dim tempID As Long

        If Long.TryParse(idTrimmed, tempID) Then
            Return tempID.ToString()
        Else
            Return idTrimmed
        End If
    End Function



    Public Function IsBorrowerStillTimedIn(ByVal borrowerID As String) As Boolean
        Dim isTimedIn As Boolean = False

        Using con As New MySqlConnection(connectionString)
            Try
                con.Open()

                Dim checkCom As String = "SELECT COUNT(*) FROM `oras_tbl` " &
                                     "WHERE (LRN = @ID OR EmployeeNo = @ID) " &
                                     "AND DATE(TimeIn) = DATE(NOW()) " &
                                     "AND TimeOut IS NULL"

                Using checkCmd As New MySqlCommand(checkCom, con)
                    checkCmd.Parameters.AddWithValue("@ID", borrowerID)
                    Dim count As Integer = Convert.ToInt32(checkCmd.ExecuteScalar())

                    If count > 0 Then
                        isTimedIn = True
                    End If
                End Using

            Catch ex As Exception
                MessageBox.Show("Database error during Time-In check: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End Using

        Return isTimedIn
    End Function
End Module