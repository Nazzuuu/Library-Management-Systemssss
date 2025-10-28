Imports MySql.Data.MySqlClient


Module GlobalVarsModule



    Private _connectionString As String =
        $"Server={My.Settings.Server};Database={My.Settings.Database};Uid={My.Settings.Username};Pwd={My.Settings.Password};"

    Public ReadOnly Property connectionString As String
        Get
            Return _connectionString
        End Get
    End Property



    Public Sub RefreshConnectionString()
        _connectionString =
            $"Server={My.Settings.Server};Database={My.Settings.Database};Uid={My.Settings.Username};Pwd={My.Settings.Password};"
    End Sub

    Public CurrentUserID As String = ""
    Public CurrentUserRole As String = "Guest"
    Public CurrentBorrowerID As String = ""
    Public CurrentBorrowerType As String = ""
    Public GlobalUsername As String = ""
    Public GlobalRole As String = ""
    Public CurrentEmployeeID As String = ""
    Public GlobalEmail As String = ""
    Public ActiveMainForm As MainForm = Nothing
    Public ActiveBorrowerLoginForm As BorrowerLoginForm
    Public connectdatabase As ServerConnection
    Public loginform As login

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
                    If count > 0 Then isTimedIn = True
                End Using
            Catch ex As Exception
                MessageBox.Show("Database error during Time-In check: " & ex.Message,
                                 "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End Using

        Return isTimedIn
    End Function


    Public Function GetLastTimeInRecordID(ByVal UserIDString As String) As Integer
        Dim recordID As Integer = 0
        If String.IsNullOrEmpty(UserIDString) Then Return 0

        Using con As New MySqlConnection(connectionString)
            Dim com As String = "SELECT ID FROM oras_tbl WHERE (LRN = @UserID OR EmployeeNo = @UserID) AND TimeOut IS NULL ORDER BY ID DESC LIMIT 1"
            Using cmd As New MySqlCommand(com, con)
                cmd.Parameters.AddWithValue("@UserID", UserIDString)
                Try
                    con.Open()
                    Dim result As Object = cmd.ExecuteScalar()
                    If result IsNot Nothing AndAlso result IsNot DBNull.Value Then
                        recordID = Convert.ToInt32(result)
                    End If
                Catch ex As Exception
                    MessageBox.Show($"Database Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End Try
            End Using
        End Using

        Return recordID
    End Function


    Public Function AutomaticTimeOut(ByVal RecordID As Integer) As Boolean
        If RecordID = 0 Then Return False
        Dim success As Boolean = False
        Using con As New MySqlConnection(connectionString)
            Dim com As String = "UPDATE oras_tbl SET TimeOut = NOW() WHERE ID = @RecordID"
            Using cmd As New MySqlCommand(com, con)
                cmd.Parameters.AddWithValue("@RecordID", RecordID)
                Try
                    con.Open()
                    Dim affectedRows As Integer = cmd.ExecuteNonQuery()
                    If affectedRows > 0 Then success = True
                Catch ex As Exception
                    MessageBox.Show($"Database Error during Auto Time-Out: {ex.Message}",
                                     "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End Try
            End Using
        End Using
        Return success
    End Function


    Public Sub LogAudit(ByVal actionType As String, ByVal formName As String, ByVal description As String,
                        Optional ByVal recordID As String = "", Optional ByVal oldValue As String = "", Optional ByVal newValue As String = "")
        If String.IsNullOrWhiteSpace(GlobalEmail) Then Return

        Dim allowedRoles As New List(Of String) From {"Librarian", "Assistant Librarian", "Staff"}
        If Not allowedRoles.Contains(GlobalRole, StringComparer.OrdinalIgnoreCase) Then Return

        Dim formattedDateTime As String = DateTime.Now.ToString("MM/dd/yy-h:mm tt")
        Using con As New MySqlConnection(connectionString)
            Dim query As String = "INSERT INTO `audit_trail_tbl` (`Role`, `Email`, `ActionType`, `FormName`, `Description`, `DateTime`) " &
                                     "VALUES (@role, @email, @action, @formName, @description, @formattedDateTime)"
            Try
                con.Open()
                Using cmd As New MySqlCommand(query, con)
                    cmd.Parameters.AddWithValue("@role", GlobalRole)
                    cmd.Parameters.AddWithValue("@email", GlobalEmail)
                    cmd.Parameters.AddWithValue("@action", actionType)
                    cmd.Parameters.AddWithValue("@formName", formName)
                    cmd.Parameters.AddWithValue("@formattedDateTime", formattedDateTime)
                    Dim fullDescription As String = description
                    If Not String.IsNullOrWhiteSpace(oldValue) Or Not String.IsNullOrWhiteSpace(newValue) Then
                        fullDescription &= $" [Change: {oldValue} -> {newValue}]"
                    End If
                    cmd.Parameters.AddWithValue("@description", fullDescription)
                    cmd.ExecuteNonQuery()
                End Using
            Catch ex As Exception
                MessageBox.Show("AUDIT LOG FAILED! Database Error: " & ex.Message,
                                 "Audit Trail Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End Using
    End Sub

End Module