Imports System.ComponentModel
Imports System.Net
Imports System.Net.Sockets
Imports System.Threading.Tasks
Imports MySql.Data.MySqlClient

Module GlobalVarsModule

    Public GlobalAutoRefreshTimer As Timer
    Public ShouldShowMainFormNextLogin As Boolean = False


    Private _connectionString As String =
        $"Server={My.Settings.Server};Database={My.Settings.Database};Uid={My.Settings.Username};Pwd={My.Settings.Password};"

    Public ReadOnly Property connectionString As String
        Get
            Return _connectionString
        End Get
    End Property


    Public WithEvents dbRefreshTimer_MD5 As New Timer() With {.Interval = 3000}
    Public lastTableCounts_MD5 As New Dictionary(Of String, String)
    Public monitoredTables_MD5 As New List(Of String) From {
        "book_tbl", "author_tbl", "genre_tbl", "publisher_tbl", "language_tbl", "supplier_tbl", "shelf_tbl", "section_tbl"
    }

    Public Sub InitializeDatabaseMonitor()
        Try
            If Not dbRefreshTimer_MD5.Enabled Then
                dbRefreshTimer_MD5.Start()
            End If
        Catch
        End Try
    End Sub

    Private Sub dbRefreshTimer_MD5_Tick(sender As Object, e As EventArgs) Handles dbRefreshTimer_MD5.Tick
        Try
            Using con As New MySqlConnection(connectionString)
                con.Open()

                Dim changesDetected As Boolean = False

                For Each tableName As String In monitoredTables_MD5

                    Dim com As New MySqlCommand($"SELECT MD5(GROUP_CONCAT(CONCAT_WS('|', *))) FROM `{tableName}`", con)
                    Dim currentHash As String = Convert.ToString(com.ExecuteScalar())

                    If String.IsNullOrEmpty(currentHash) Then
                        currentHash = ""
                    End If

                    If lastTableCounts_MD5.ContainsKey(tableName) Then

                        If lastTableCounts_MD5(tableName).ToString() <> currentHash Then
                            changesDetected = True
                            lastTableCounts_MD5(tableName) = currentHash
                        End If
                    Else
                        lastTableCounts_MD5(tableName) = currentHash
                        changesDetected = True
                    End If
                Next

                If changesDetected Then
                    RaiseEvent DatabaseUpdated()
                End If
            End Using
        Catch ex As Exception
            Debug.WriteLine("dbRefreshTimer_MD5_Tick error: " & ex.Message)
        End Try
    End Sub





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
        AddHandler DatabaseUpdated, AddressOf GlobalComboBoxUpdater
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
                    ' ✅ Gumamit ng MD5 hash para malaman kung may UPDATE kahit walang dagdag/bawas
                    Dim com As New MySqlCommand($"SELECT MD5(GROUP_CONCAT(CONCAT_WS('|', *))) FROM `{tableName}`", con)
                    Dim currentHash As String = Convert.ToString(com.ExecuteScalar())

                    If String.IsNullOrEmpty(currentHash) Then
                        currentHash = ""
                    End If

                    If lastTableCounts.ContainsKey(tableName) Then
                        ' lastTableCounts ngayon nagho-hold ng hash (hindi count)
                        If lastTableCounts(tableName).ToString() <> currentHash Then
                            changesDetected = True
                            lastTableCounts(tableName) = currentHash
                        End If
                    Else
                        lastTableCounts(tableName) = currentHash
                        changesDetected = True
                    End If
                Next

                ' ✅ Trigger event kung may changes (add, delete, o update)
                If changesDetected Then
                    RaiseEvent DatabaseUpdated()
                End If
            End Using



        Catch ex As Exception
            Debug.WriteLine("dbRefreshTimer_Tick error: " & ex.Message)
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

                                           End Sub)
                           Catch ex As Exception
                               grid.Invoke(Sub()

                                           End Sub)
                           End Try
                       End Sub)
    End Function


    Private refreshTimers As New Dictionary(Of DataGridView, Timer)

    Public Async Sub AutoRefreshGrid(grid As DataGridView, query As String, Optional intervalMs As Integer = 2000)
        Try
            Await LoadToGridAsync(grid, query)


            HideStandardColumns(grid)
        Catch ex As Exception

        End Try

        If refreshTimers.ContainsKey(grid) Then
            refreshTimers(grid).Stop()
            refreshTimers.Remove(grid)
        End If

        Dim t As New Timer() With {.Interval = intervalMs}
        AddHandler t.Tick, Async Sub(sender As Object, e As EventArgs)
                               Try
                                   temporarynorefresh(grid)

                                   Dim selectedColumn As String = ""
                                   Dim selectedValue As Object = Nothing
                                   PreserveSelection(grid, selectedColumn, selectedValue)

                                   Await LoadToGridAsync(grid, query)

                                   HideStandardColumns(grid)
                                   RestoreSelection(grid, selectedColumn, selectedValue)

                               Catch ex As Exception

                               End Try
                           End Sub

        refreshTimers(grid) = t
        t.Start()
    End Sub


    Public Sub temporarynorefresh(grid As DataGridView)
        Try
            If TypeOf grid.FindForm() Is Form Then
                Dim parentForm = DirectCast(grid.FindForm(), Form)


                Dim numeric As NumericUpDown = parentForm.Controls.Find("NumericUpDown1", True).FirstOrDefault()
                If numeric IsNot Nothing AndAlso
            parentForm.GetType().GetField("isNumericEditing",
                Reflection.BindingFlags.NonPublic Or Reflection.BindingFlags.Instance)?.GetValue(parentForm) = True Then
                    Exit Sub
                End If



            End If
        Catch

        End Try
    End Sub


    Public Sub PauseAutoRefresh(grid As DataGridView)
        Try
            If refreshTimers.ContainsKey(grid) Then
                refreshTimers(grid).Stop()
            End If
        Catch
        End Try
    End Sub

    Public Sub ResumeAutoRefresh(grid As DataGridView)
        Try
            If refreshTimers.ContainsKey(grid) Then
                refreshTimers(grid).Start()
            End If
        Catch
        End Try
    End Sub

    Private Sub HideStandardColumns(grid As DataGridView)
        Dim colsToHide() As String = {"ID", "CurrentIP", "is_logged_in"}
        For Each colName In colsToHide
            If grid.Columns.Contains(colName) Then
                grid.Columns(colName).Visible = False
            End If
        Next
    End Sub


    Private Sub PreserveSelection(grid As DataGridView, ByRef selectedColumn As String, ByRef selectedValue As Object)
        selectedColumn = ""
        selectedValue = Nothing

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
    End Sub

    Private Sub RestoreSelection(grid As DataGridView, selectedColumn As String, selectedValue As Object)

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
    End Sub

    Public Sub HandleAutoRefreshPause(grid As DataGridView, txtSearch As Control)
        Try
            If refreshTimers.ContainsKey(grid) Then
                Dim t As Timer = refreshTimers(grid)


                If Not String.IsNullOrWhiteSpace(txtSearch.Text) Then
                    If t.Enabled Then t.Stop()
                Else
                    If Not t.Enabled Then t.Start()
                End If
            End If
        Catch ex As Exception

        End Try
    End Sub



    Private comboSources As New Dictionary(Of ComboBox, String)


    Public Sub AutoRefreshComboBox(cb As ComboBox, query As String, displayMember As String, valueMember As String)
        Try

            If comboSources.ContainsKey(cb) Then
                comboSources(cb) = query
            Else
                comboSources.Add(cb, query)
            End If


            RefreshComboBox(cb, query, displayMember, valueMember)
        Catch ex As Exception
        End Try
    End Sub


    Private Sub RefreshComboBox(cb As ComboBox, query As String, displayMember As String, valueMember As String)
        Try
            Using con As New MySqlConnection(connectionString)
                Using adap As New MySqlDataAdapter(query, con)
                    Dim dt As New DataTable()
                    adap.Fill(dt)

                    Dim prevValue As Object = cb.SelectedValue

                    cb.DataSource = dt
                    cb.DisplayMember = displayMember
                    cb.ValueMember = valueMember
                    cb.SelectedIndex = -1


                    If prevValue IsNot Nothing Then
                        For Each row As DataRow In dt.Rows
                            If row(valueMember).ToString() = prevValue.ToString() Then
                                cb.SelectedValue = prevValue
                                Exit For
                            End If
                        Next
                    End If
                End Using
            End Using
        Catch
        End Try
    End Sub


    Private Sub GlobalComboBoxUpdater()
        Try
            For Each pair In comboSources.ToList()
                Dim cb As ComboBox = pair.Key
                Dim query As String = pair.Value


                If cb Is Nothing OrElse cb.IsDisposed Then
                    comboSources.Remove(cb)
                    Continue For
                End If


                Dim displayMember As String = cb.DisplayMember
                Dim valueMember As String = cb.ValueMember
                RefreshComboBox(cb, query, displayMember, valueMember)
            Next
        Catch
        End Try
    End Sub

    Public Function IsInDesignMode(ctrl As Control) As Boolean
        Try
            Return (LicenseManager.UsageMode = LicenseUsageMode.Designtime) OrElse
                   (ctrl IsNot Nothing AndAlso ctrl.Site IsNot Nothing AndAlso ctrl.Site.DesignMode)
        Catch
            Return False
        End Try
    End Function


End Module