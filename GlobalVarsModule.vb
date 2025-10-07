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

End Module