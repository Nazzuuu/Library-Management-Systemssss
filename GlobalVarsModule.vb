Imports MySql.Data.MySqlClient

Module GlobalVarsModule

    Public connectionString As String = "Server=localhost;Database=laybsis_dbs;Uid=root;Pwd="

    Public CurrentUserID As String = ""
    Public CurrentUserRole As String = "Guest"

    Public CurrentBorrowerID As String = ""
    Public CurrentBorrowerType As String = ""

    Public GlobalUsername As String = ""
    Public GlobalRole As String = ""
    Public CurrentEmployeeID As String = ""

    Public GlobalEmail As String = ""

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
                    If affectedRows > 0 Then
                        success = True
                    End If
                Catch ex As Exception
                    MessageBox.Show($"Database Error during Auto Time-Out: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End Try
            End Using
        End Using
        Return success
    End Function


    Public Sub LogAudit(ByVal actionType As String, ByVal formName As String, ByVal description As String, Optional ByVal recordID As String = "", Optional ByVal oldValue As String = "", Optional ByVal newValue As String = "")


        If String.IsNullOrWhiteSpace(GlobalEmail) Then Return


        Dim allowedRoles As New List(Of String) From {"Librarian", "Assistant Librarian", "Staff"}
        If Not allowedRoles.Contains(GlobalRole, StringComparer.OrdinalIgnoreCase) Then Return


        Dim formattedDateTime As String = DateTime.Now.ToString("MM/dd/yy-h:mm tt")

        Dim con As New MySqlConnection(connectionString)


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

            MessageBox.Show("AUDIT LOG FAILED! Database Error: " & ex.Message & vbCrLf & "CHECK: Is the column name 'Email' and 'Action' in audit_trail_tbl?", "Audit Trail Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            If con.State = ConnectionState.Open Then con.Close()
        End Try
    End Sub

End Module