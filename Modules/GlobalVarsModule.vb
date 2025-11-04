Imports MySql.Data.MySqlClient
Imports System.Net
Imports System.Net.Sockets
Imports System.Threading.Tasks

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


    Public Function GetLocalIPAddress() As String
        Try
            Dim host As String = Dns.GetHostName()
            Dim ipEntry As IPHostEntry = Dns.GetHostEntry(host)

            For Each ipAddress As IPAddress In ipEntry.AddressList
                If ipAddress.AddressFamily = AddressFamily.InterNetwork Then
                    Return ipAddress.ToString()
                End If
            Next

            Return "127.0.0.1"
        Catch ex As Exception
            Return "0.0.0.0"
        End Try
    End Function

    Public Sub UpdateUserIP(ByVal newIP As String, ByVal userID As String, ByVal userRole As String)

        Using con As New MySqlConnection(connectionString)
            Try
                con.Open()
                Dim tableName As String = ""
                Dim idColumn As String = ""


                Select Case userRole.ToLower()
                    Case "librarian"
                        tableName = "superadmin_tbl"
                        idColumn = "ID"
                    Case "staff", "assistant librarian"
                        tableName = "user_staff_tbl"
                        idColumn = "ID"
                    Case Else
                        Return
                End Select

                Dim sqlQuery As String = $"UPDATE {tableName} SET CurrentIP = @ip WHERE {idColumn} = @userID"

                Using cmd As New MySqlCommand(sqlQuery, con)
                    cmd.Parameters.AddWithValue("@ip", newIP)
                    cmd.Parameters.AddWithValue("@userID", userID)
                    cmd.ExecuteNonQuery()
                End Using

            Catch ex As Exception

            End Try
        End Using
    End Sub

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



    Public Event DatabaseUpdated()
    Private WithEvents dbRefreshTimer As New Timer() With {.Interval = 200}


    Private lastTableCounts As New Dictionary(Of String, Integer)


    Private monitoredTables As String() = {
        "acession_tbl", "acquisition_tbl", "audit_trail_tbl", "author_tbl", "available_tbl",
        "book_tbl", "borrowerview_tbl", "borroweredit_tbl", "borrower_tbl", "borrowinghistory_tbl",
        "borrowing_tbl", "category_tbl", "confirmation_tbl", "damagedview_tbl", "department_tbl",
        "genre_tbl", "grade_tbl", "language_tbl", "lostview_tbl", "oras_tbl", "overdueview_tbl",
        "penalty_management_tbl", "penalty_tbl", "printreceipt_tbl", "publisher_tbl",
        "reservecopiess_tbl", "reserveview_tbl", "returnedview_tbl", "returning_tbl",
        "section_tbl", "shelf_tbl", "strand_tbl", "superadmin_tbl", "supplier_tbl",
        "timeoutrecord_tbl", "totalbooksview_tbl", "user_staff_tbl"
    }

    Public Sub StartAutoRefresh()
        dbRefreshTimer.Start()
    End Sub

    Public Sub StopAutoRefresh()
        dbRefreshTimer.Stop()
    End Sub


    Private Sub dbRefreshTimer_Tick(sender As Object, e As EventArgs) Handles dbRefreshTimer.Tick
        Try
            Using con As New MySqlConnection(connectionString)
                con.Open()

                Dim changesDetected As Boolean = False

                For Each tableName As String In monitoredTables
                    Dim com As New MySqlCommand($"SELECT COUNT(*) FROM `{tableName}`", con)
                    Dim currentCount As Integer = Convert.ToInt32(com.ExecuteScalar())

                    If lastTableCounts.ContainsKey(tableName) Then
                        If lastTableCounts(tableName) <> currentCount Then
                            changesDetected = True
                            lastTableCounts(tableName) = currentCount
                        End If
                    Else
                        lastTableCounts(tableName) = currentCount
                    End If
                Next


                If changesDetected Then
                    RaiseEvent DatabaseUpdated()
                End If
            End Using
        Catch ex As Exception

        End Try
    End Sub

    Public Async Function LoadToGridAsync(grid As DataGridView, query As String) As Task
        Await Task.Run(Sub()
                           Try
                               Using con As New MySqlConnection(connectionString)
                                   Using adap As New MySqlDataAdapter(query, con)
                                       Dim ds As New DataSet()
                                       adap.Fill(ds)
                                       Dim dt As DataTable = ds.Tables(0)

                                       grid.Invoke(Sub()
                                                       grid.DataSource = dt
                                                   End Sub)
                                   End Using
                               End Using
                           Catch ex As MySqlException
                               grid.Invoke(Sub()
                                               MessageBox.Show("Error loading data: " & ex.Message,
                                                           "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                                           End Sub)
                           Catch ex As Exception
                               grid.Invoke(Sub()
                                               MessageBox.Show("Unexpected error while loading data: " & ex.Message,
                                                           "System Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                                           End Sub)
                           End Try
                       End Sub)
    End Function


    Private refreshTimers As New Dictionary(Of DataGridView, Timer)


    Public Async Sub AutoRefreshGrid(grid As DataGridView, query As String, Optional intervalMs As Integer = 2000)
        Try
            Await LoadToGridAsync(grid, query)
        Catch ex As Exception
            MessageBox.Show("Initial data load failed: " & ex.Message)
        End Try


        If refreshTimers.ContainsKey(grid) Then
            refreshTimers(grid).Stop()
            refreshTimers.Remove(grid)
        End If


        Dim t As New Timer() With {.Interval = intervalMs}
        AddHandler t.Tick, Async Sub(sender As Object, e As EventArgs)
                               Try

                                   Dim selectedValue As Object = Nothing
                                   Dim selectedColumn As String = ""

                                   If grid.SelectedRows.Count > 0 Then

                                       If grid.Columns.Contains("ID") Then
                                           selectedColumn = "ID"
                                           selectedValue = grid.SelectedRows(0).Cells("ID").Value
                                       Else

                                           For Each col As DataGridViewColumn In grid.Columns
                                               If col.Visible Then
                                                   selectedColumn = col.Name
                                                   selectedValue = grid.SelectedRows(0).Cells(col.Name).Value
                                                   Exit For
                                               End If
                                           Next
                                       End If
                                   End If

                                   Await LoadToGridAsync(grid, query)


                                   If selectedValue IsNot Nothing AndAlso grid.Rows.Count > 0 AndAlso grid.Columns.Contains(selectedColumn) Then
                                       For Each row As DataGridViewRow In grid.Rows
                                           If row.Cells(selectedColumn).Value IsNot Nothing AndAlso
                                              row.Cells(selectedColumn).Value.ToString() = selectedValue.ToString() Then
                                               row.Selected = True
                                               grid.FirstDisplayedScrollingRowIndex = row.Index
                                               Exit For
                                           End If
                                       Next
                                   Else

                                       grid.ClearSelection()
                                       grid.CurrentCell = Nothing
                                   End If
                               Catch

                               End Try
                           End Sub

        refreshTimers(grid) = t
        t.Start()
    End Sub



End Module