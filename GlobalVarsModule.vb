Imports MySql.Data.MySqlClient

Module GlobalVarsModule

    Public connectionString As String = "Server=localhost;Database=laybsis_dbs;Uid=root;Pwd="

    Public CurrentUserID As String = ""
    Public CurrentUserRole As String = "Guest"

    Public CurrentBorrowerID As String = ""
    Public CurrentBorrowerType As String = ""

    Public Function GetCleanCurrentBorrowerID() As String
        Dim idTrimmed As String = CurrentBorrowerID.Trim()
        Dim tempID As Long

        If Long.TryParse(idTrimmed, tempID) Then
            Return tempID.ToString()
        Else
            Return idTrimmed
        End If
    End Function

    Public Function IsBorrowerTimedIn() As Boolean

        If CurrentBorrowerID = "" Or CurrentUserRole <> "Borrower" Then
            Return False
        End If

        Dim isTimedIn As Boolean = False
        Dim query As String


        If CurrentBorrowerType = "Student" Then
            query = "SELECT 1 FROM `oras_tbl` WHERE `LRN` = @ID AND `TimeOut` IS NULL LIMIT 1"
        ElseIf CurrentBorrowerType = "Teacher" Then
            query = "SELECT 1 FROM `oras_tbl` WHERE `EmployeeNo` = @ID AND `TimeOut` IS NULL LIMIT 1"
        Else
            Return False
        End If

        Using con As New MySqlConnection(connectionString)
            Try
                con.Open()
                Using cmd As New MySqlCommand(query, con)
                    cmd.Parameters.AddWithValue("@ID", CurrentBorrowerID)

                    If cmd.ExecuteScalar() IsNot Nothing Then
                        isTimedIn = True
                    End If
                End Using
            Catch ex As Exception

                Return False
            End Try
        End Using

        Return isTimedIn
    End Function

End Module