Imports System.ComponentModel
Imports System.Linq
Imports System.Collections.Concurrent
Imports System.Net
Imports System.Net.Sockets
Imports System.Threading.Tasks
Imports MySql.Data.MySqlClient
Imports System.IO
Imports System.Diagnostics
Imports System.Net.Mail
Module GlobalVarsModule

    Public GlobalAutoRefreshTimer As Timer
    Public ShouldShowMainFormNextLogin As Boolean = False
    Private WithEvents backgroundInboxTimer As New Timer() With {.Interval = 1000}


    Private _connectionString As String =
        $"Server={My.Settings.Server};Database={My.Settings.Database};Uid={My.Settings.Username};Pwd={My.Settings.Password};"

    'wag mo to alisin
    'Private _connectionString As String =
    '    $"Server=localhost;Database=laybsisu_dbs;Uid=root;Pwd=root;"

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

            If dbRefreshTimer_MD5.Enabled Then
                dbRefreshTimer_MD5.Stop()
            End If
            Try
                AddHandler Microsoft.Win32.SystemEvents.SessionEnding, AddressOf OnSessionEnding
                Try
                    AddHandler Application.ApplicationExit, AddressOf OnApplicationExit
                Catch
                End Try
                Try
                    AddHandler AppDomain.CurrentDomain.ProcessExit, AddressOf OnProcessExit
                Catch
                End Try
            Catch
            End Try

            Try
                CleanupLocalMachineLogins()
            Catch
            End Try
        Catch
        End Try
    End Sub

    Private Sub CleanupLocalMachineLogins()
        Try
            Dim localIP As String = GetLocalIPAddress()
            If String.IsNullOrWhiteSpace(localIP) Then Return

            Using con As New MySqlConnection(connectionString)
                con.Open()

                Using cmd As New MySqlCommand("UPDATE superadmin_tbl SET is_logged_in = 0, CurrentIP = '0.0.0.0' WHERE CurrentIP = @ip AND is_logged_in = 1", con)
                    cmd.Parameters.AddWithValue("@ip", localIP)
                    cmd.ExecuteNonQuery()
                End Using

                Using cmd2 As New MySqlCommand("UPDATE user_staff_tbl SET is_logged_in = 0, CurrentIP = '0.0.0.0' WHERE CurrentIP = @ip AND is_logged_in = 1", con)
                    cmd2.Parameters.AddWithValue("@ip", localIP)
                    cmd2.ExecuteNonQuery()
                End Using

                Using cmd3 As New MySqlCommand("UPDATE borroweredit_tbl SET is_logged_in = 0, CurrentIP = NULL WHERE CurrentIP = @ip AND is_logged_in = 1", con)
                    cmd3.Parameters.AddWithValue("@ip", localIP)
                    cmd3.ExecuteNonQuery()
                End Using

                con.Close()
            End Using
        Catch
        End Try
    End Sub

    Private Sub OnSessionEnding(sender As Object, e As Microsoft.Win32.SessionEndingEventArgs)
        Try
            ShutdownCleanup()
        Catch
        End Try
    End Sub

    Private Sub OnApplicationExit(sender As Object, e As EventArgs)
        Try
            ShutdownCleanup()
        Catch
        End Try
    End Sub

    Private Sub OnProcessExit(sender As Object, e As EventArgs)
        Try
            ShutdownCleanup()
        Catch
        End Try
    End Sub

    Public Sub ShutdownCleanup()
        Try
            Using con As New MySqlConnection(connectionString)
                con.Open()

                Try

                    Using cmd As New MySqlCommand("UPDATE superadmin_tbl SET is_logged_in = 0, CurrentIP = '0.0.0.0'", con)
                        cmd.ExecuteNonQuery()
                    End Using

                    Using cmd2 As New MySqlCommand("UPDATE user_staff_tbl SET is_logged_in = 0, CurrentIP = '0.0.0.0'", con)
                        cmd2.ExecuteNonQuery()
                    End Using

                    Using cmd3 As New MySqlCommand("UPDATE borroweredit_tbl SET is_logged_in = 0, CurrentIP = NULL", con)
                        cmd3.ExecuteNonQuery()
                    End Using


                    If Not SuppressShutdownCleanup Then
                        Using cmd4 As New MySqlCommand("UPDATE oras_tbl SET TimeOut = NOW() WHERE TimeOut IS NULL", con)
                            cmd4.ExecuteNonQuery()
                        End Using
                    End If
                Catch
                End Try

                con.Close()
            End Using
        Catch
        End Try
    End Sub

    Private Sub AppendLog(msg As String)
        Try
            Dim logPath As String = Path.Combine(Application.StartupPath, "overdue_notify.log")
            Dim line As String = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {msg}{Environment.NewLine}"
            File.AppendAllText(logPath, line)
        Catch
        End Try
    End Sub

    Public Sub EnsureInboxTableExists()
        Try
            Using con As New MySqlConnection(connectionString)
                con.Open()
                Dim createSql As String = "CREATE TABLE IF NOT EXISTS `inbox_tbl` (" &
                                          "ID INT AUTO_INCREMENT PRIMARY KEY, " &
                                          "TransactionReceipt VARCHAR(255), " &
                                          "Email VARCHAR(255), " &
                                          "FullName VARCHAR(255), " &
                                          "Subject VARCHAR(255), " &
                                          "Body TEXT, " &
                                          "IsSent TINYINT(1) DEFAULT 0, " &
                                          "NoticeType VARCHAR(20) NULL, " &
                                          "CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP, " &
                                          "SentAt DATETIME NULL" &
                                          ") ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;"
                Using cmd As New MySqlCommand(createSql, con)
                    cmd.ExecuteNonQuery()
                End Using

                EnsureInboxColumns(con)
            End Using
        Catch ex As Exception

        End Try
    End Sub

    Private Sub EnsureInboxColumns(con As MySqlConnection)
        Try

            Dim requiredCols As New Dictionary(Of String, String) From {
                {"TransactionReceipt", "VARCHAR(255)"},
                {"Email", "VARCHAR(255)"},
                {"FullName", "VARCHAR(255)"},
                {"Subject", "VARCHAR(255)"},
                {"Body", "TEXT"},
                {"IsSent", "TINYINT(1) DEFAULT 0"},
                {"CreatedAt", "DATETIME DEFAULT CURRENT_TIMESTAMP"},
                {"SentAt", "DATETIME NULL"},
                {"DueDate", "VARCHAR(255)"},
                {"Date", "DATETIME NULL"},
                {"NoticeType", "VARCHAR(20) NULL"}
            }

            For Each kvp In requiredCols
                If Not ColumnExists(con, "inbox_tbl", kvp.Key) Then
                    Try
                        Using a As New MySqlCommand($"ALTER TABLE `inbox_tbl` ADD COLUMN `{kvp.Key}` {kvp.Value};", con)
                            a.ExecuteNonQuery()
                        End Using
                    Catch

                    End Try
                End If
            Next

            ' Dati pang mga record (bago pa magkaroon ng NoticeType) ay puro OVERDUE notice lang.
            ' Minamarkahan sila para hindi ma-resend ang overdue notice sa mga dati nang na-notify.
            Try
                Using mig As New MySqlCommand("UPDATE `inbox_tbl` SET `NoticeType` = 'OVERDUE' " &
                                              "WHERE `NoticeType` IS NULL " &
                                              "AND `TransactionReceipt` IS NOT NULL " &
                                              "AND `TransactionReceipt` <> ''", con)
                    mig.ExecuteNonQuery()
                End Using
            Catch
            End Try
        Catch
        End Try
    End Sub

    Private Function ColumnExists(con As MySqlConnection, tableName As String, columnName As String) As Boolean
        Try
            Using cmd As New MySqlCommand("SELECT COUNT(*) FROM information_schema.COLUMNS WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = @tbl AND COLUMN_NAME = @col", con)
                cmd.Parameters.AddWithValue("@tbl", tableName)
                cmd.Parameters.AddWithValue("@col", columnName)
                Dim obj = cmd.ExecuteScalar()
                If obj IsNot Nothing AndAlso obj IsNot DBNull.Value Then
                    Return Convert.ToInt32(obj) > 0
                End If
            End Using
        Catch
        End Try
        Return False
    End Function


    Public Sub CapitalizeFirstLetter_ControlTextChanged(sender As Object, e As EventArgs)
        Dim ctrl As Control = TryCast(sender, Control)
        If ctrl Is Nothing Then Return
        If Not String.IsNullOrEmpty(ctrl.Name) AndAlso ctrl.Name.ToLower().Contains("email") Then Return
        CapitalizeFirstLetter(ctrl)
    End Sub

    Public Sub CapitalizeFirstLetter(ctrl As Control)
        Try
            If ctrl Is Nothing Then Return
            If Not String.IsNullOrEmpty(ctrl.Name) AndAlso ctrl.Name.ToLower().Contains("email") Then Return
            Dim textProp = ctrl.GetType().GetProperty("Text")
            If textProp Is Nothing Then Return

            Dim original As String = Convert.ToString(textProp.GetValue(ctrl))
            If String.IsNullOrEmpty(original) Then Return


            Dim selStart As Integer = 0
            Dim selLen As Integer = 0

            Dim selStartProp = ctrl.GetType().GetProperty("SelectionStart")
            Dim selLenProp = ctrl.GetType().GetProperty("SelectionLength")

            If selStartProp IsNot Nothing Then
                selStart = Convert.ToInt32(selStartProp.GetValue(ctrl))
            ElseIf TypeOf ctrl Is TextBoxBase Then
                selStart = DirectCast(ctrl, TextBoxBase).SelectionStart
            End If

            If selLenProp IsNot Nothing Then
                selLen = Convert.ToInt32(selLenProp.GetValue(ctrl))
            ElseIf TypeOf ctrl Is TextBoxBase Then
                selLen = DirectCast(ctrl, TextBoxBase).SelectionLength
            End If

            Dim newText As String = original
            If original.Length >= 1 Then
                Dim firstChar As Char = original(0)
                Dim rest As String = If(original.Length > 1, original.Substring(1), String.Empty)
                newText = Char.ToUpperInvariant(firstChar) & rest
            End If

            If Not newText.Equals(original) Then
                textProp.SetValue(ctrl, newText)


                Try
                    If selStartProp IsNot Nothing Then selStartProp.SetValue(ctrl, selStart)
                    If selLenProp IsNot Nothing Then selLenProp.SetValue(ctrl, selLen)
                    If TypeOf ctrl Is TextBoxBase Then
                        Dim tb = DirectCast(ctrl, TextBoxBase)
                        tb.SelectionStart = selStart
                        tb.SelectionLength = selLen
                    End If
                Catch
                End Try
            End If
        Catch
        End Try
    End Sub

    Public Sub EnableCapitalizeFirstLetterForControls(parent As Control)
        Try
            For Each ctrl As Control In parent.Controls
                If ctrl Is Nothing Then Continue For


                Dim textProp = ctrl.GetType().GetProperty("Text")
                If textProp IsNot Nothing Then
                    If String.IsNullOrEmpty(ctrl.Name) OrElse Not ctrl.Name.ToLower().Contains("email") Then
                        AddHandler ctrl.TextChanged, AddressOf CapitalizeFirstLetter_ControlTextChanged
                    End If
                End If

                If ctrl.HasChildren Then
                    EnableCapitalizeFirstLetterForControls(ctrl)
                End If
            Next
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


                If changesDetected AndAlso Not dbRefreshTimer.Enabled Then
                    RaiseEvent DatabaseUpdated()
                End If
            End Using
        Catch ex As Exception

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
    Public GlobalFullname As String = ""
    Public ActiveMainForm As MainForm = Nothing

    Public connectdatabase As ServerConnection
    Public loginform As login

    Public studentLimit As Integer = 1
    Public teacherLimit As Integer = 1
    Public filePath As String = Application.StartupPath & "\duration_settings.txt"


    Public Sub LoadDurationSettings()
        Try
            If File.Exists(filePath) Then
                Dim lines() As String = File.ReadAllLines(filePath)
                If lines.Length >= 2 Then
                    studentLimit = Val(lines(0))
                    teacherLimit = Val(lines(1))
                End If
            End If
        Catch ex As Exception

            studentLimit = 1
            teacherLimit = 1
        End Try
    End Sub

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
        End If
        Return idTrimmed
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
    Private WithEvents dbRefreshTimer As New Timer() With {.Interval = 3000}
    Private WithEvents autoTimeoutTimer As New Timer() With {.Interval = 60 * 1000}
    Private lastTableCounts As New Dictionary(Of String, Integer)
    Private databaseUpdatedHandlerRegistered As Boolean = False


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
        If Not dbRefreshTimer.Enabled Then
            dbRefreshTimer.Start()
        End If

        If Not databaseUpdatedHandlerRegistered Then
            AddHandler DatabaseUpdated, AddressOf GlobalComboBoxUpdater
            databaseUpdatedHandlerRegistered = True
        End If
        Try
            autoTimeoutTimer.Start()
            Try
                dailyOverdueTimer.Start()

                ' I-check agad ang Due Date / Overdue notifications pagka-start (hindi na hihintayin ang unang timer tick)
                If LastProcessedDate < Date.Today Then
                    Task.Run(Sub()
                                 Try
                                     SendOverdueBorrowerNotifications()
                                 Catch
                                 End Try
                             End Sub)
                End If
            Catch
            End Try
            Try
                inboxCheckTimer.Start()

                ' DISABLED: ang backgroundInboxTimer ay nag-i-insert ng "sent" na record kahit hindi naman talaga nagse-send ng email
                ' (placeholder credentials, walang smtp.Send) at magdodoble ng overdue notice. Ang SendOverdueBorrowerNotifications na ang gumagawa nito.
                'backgroundInboxTimer.Start()
            Catch
            End Try

        Catch
        End Try
    End Sub


    Private Sub autoTimeoutTimer_Tick(sender As Object, e As EventArgs) Handles autoTimeoutTimer.Tick
        Try

            Using con As New MySqlConnection(connectionString)
                con.Open()


                Dim sql14 As String = "SELECT ID FROM oras_tbl WHERE TimeOut IS NULL AND TimeIn <= DATE_SUB(NOW(), INTERVAL 14 HOUR)"
                Using cmd14 As New MySqlCommand(sql14, con)
                    Using rdr14 = cmd14.ExecuteReader()
                        Dim ids As New List(Of Integer)
                        While rdr14.Read()
                            ids.Add(Convert.ToInt32(rdr14("ID")))
                        End While
                        rdr14.Close()

                        For Each id In ids
                            AutomaticTimeOut(id)
                        Next
                    End Using
                End Using


                Dim nowTime As DateTime = DateTime.Now
                If nowTime.TimeOfDay >= New TimeSpan(20, 0, 0) Then
                    Dim sql8pm As String = "SELECT ID FROM oras_tbl WHERE TimeOut IS NULL AND DATE(TimeIn) = DATE(NOW())"
                    Using cmd8 As New MySqlCommand(sql8pm, con)
                        Using rdr8 = cmd8.ExecuteReader()
                            Dim ids8 As New List(Of Integer)
                            While rdr8.Read()
                                ids8.Add(Convert.ToInt32(rdr8("ID")))
                            End While
                            rdr8.Close()

                            For Each id In ids8
                                AutomaticTimeOut(id)
                            Next
                        End Using
                    End Using
                End If

            End Using
        Catch
        End Try
    End Sub

    Public Sub StopAutoRefresh()
        dbRefreshTimer.Stop()
        Try
            autoTimeoutTimer.Stop()
        Catch
        End Try
    End Sub



    Private Sub dbRefreshTimer_Tick(sender As Object, e As EventArgs) Handles dbRefreshTimer.Tick
        Try
            Using con As New MySqlConnection(connectionString)
                con.Open()

                Dim changesDetected As Boolean = False

                For Each tableName As String In monitoredTables

                    Dim com As New MySqlCommand($"SELECT MD5(GROUP_CONCAT(CONCAT_WS('|', *))) FROM `{tableName}`", con)
                    Dim currentHash As String = Convert.ToString(com.ExecuteScalar())

                    If String.IsNullOrEmpty(currentHash) Then
                        currentHash = ""
                    End If

                    If lastTableCounts.ContainsKey(tableName) Then

                        If lastTableCounts(tableName).ToString() <> currentHash Then
                            changesDetected = True
                            lastTableCounts(tableName) = currentHash
                        End If
                    Else
                        lastTableCounts(tableName) = currentHash
                        changesDetected = True
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
        If grid Is Nothing OrElse grid.IsDisposed Then Return

        Dim loadVersion As Integer =
            gridLoadVersions.AddOrUpdate(grid, 1, Function(key, oldValue) oldValue + 1)

        Try
            Dim dt As DataTable = Nothing

            Await Task.Run(
                Sub()
                    Using con As New MySqlConnection(connectionString)
                        Using adap As New MySqlDataAdapter(query, con)
                            Dim ds As New DataSet()
                            adap.Fill(ds)

                            If ds.Tables.Count > 0 Then
                                dt = ds.Tables(0)
                            Else
                                dt = New DataTable()
                            End If
                        End Using
                    End Using
                End Sub)

            If grid Is Nothing OrElse grid.IsDisposed Then Return

            Dim latestVersion As Integer = 0
            If gridLoadVersions.TryGetValue(grid, latestVersion) AndAlso
               latestVersion <> loadVersion Then
                Return
            End If

            Dim applyData As Action =
                Sub()
                    If grid.IsDisposed Then Return

                    Dim currentVersion As Integer = 0
                    If gridLoadVersions.TryGetValue(grid, currentVersion) AndAlso
                       currentVersion <> loadVersion Then
                        Return
                    End If

                    grid.DataSource = dt
                End Sub

            If grid.InvokeRequired Then
                grid.Invoke(applyData)
            Else
                applyData()
            End If

        Catch ex As Exception
            Debug.WriteLine("LoadToGridAsync error: " & ex.Message)
        End Try
    End Function


    Public refreshTimers As New Dictionary(Of DataGridView, Timer)
    Private ReadOnly gridLoadVersions As New ConcurrentDictionary(Of DataGridView, Integer)

    Private Function IsUserEditingForm(ctrl As Control) As Boolean
        Try
            If ctrl Is Nothing Then Return False
            Dim frm As Form = ctrl.FindForm()
            If frm Is Nothing Then Return False

            Dim active As Control = frm.ActiveControl
            If active Is Nothing Then Return False

            While active IsNot Nothing
                If TypeOf active Is TextBoxBase OrElse
                   TypeOf active Is ComboBox OrElse
                   TypeOf active Is RadioButton OrElse
                   TypeOf active Is CheckBox Then
                    Return True
                End If
                active = active.Parent
            End While
        Catch
        End Try
        Return False
    End Function

    Public Async Sub AutoRefreshGrid(grid As DataGridView, query As String, Optional intervalMs As Integer = 2000)
        If grid Is Nothing OrElse grid.IsDisposed Then Return

        Try
            If refreshTimers.ContainsKey(grid) Then
                refreshTimers(grid).Stop()
                refreshTimers(grid).Dispose()
                refreshTimers.Remove(grid)
            End If

            Await LoadToGridAsync(grid, query)
            HideStandardColumns(grid)

            Dim t As New Timer() With {.Interval = Math.Max(1000, intervalMs)}

            AddHandler t.Tick,
                Async Sub(sender As Object, e As EventArgs)
                    If grid Is Nothing OrElse grid.IsDisposed Then
                        Return
                    End If

                    If Not t.Enabled Then
                        Return
                    End If

                    If IsUserEditingForm(grid) Then
                        Return
                    End If

                    Try
                        Dim selectedColumn As String = ""
                        Dim selectedValue As Object = Nothing
                        Dim currentColumn As String = ""
                        Dim currentValue As Object = Nothing
                        Dim firstDisplayedRow As Integer = -1

                        PreserveSelection(grid, selectedColumn, selectedValue)

                        If grid.CurrentCell IsNot Nothing Then
                            currentColumn = grid.CurrentCell.OwningColumn.Name
                            currentValue = grid.CurrentCell.Value
                        End If

                        If grid.RowCount > 0 AndAlso grid.FirstDisplayedScrollingRowIndex >= 0 Then
                            firstDisplayedRow = grid.FirstDisplayedScrollingRowIndex
                        End If

                        Await LoadToGridAsync(grid, query)

                        HideStandardColumns(grid)
                        RestoreSelection(grid, selectedColumn, selectedValue, currentColumn, currentValue, firstDisplayedRow)

                    Catch ex As Exception
                        Debug.WriteLine("AutoRefreshGrid error: " & ex.Message)
                    End Try
                End Sub

            refreshTimers(grid) = t
            t.Start()

        Catch ex As Exception
            Debug.WriteLine("AutoRefreshGrid initialization error: " & ex.Message)
        End Try
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

    'ito sa radiobutton/checkbox---pang pause yan bes''
    Public Sub PauseAutoRefresh(grid As DataGridView)
        Try
            If refreshTimers.ContainsKey(grid) Then
                refreshTimers(grid).Stop()
            End If
        Catch
        End Try
    End Sub

    ''ito sa textbox---pang resume yan bes :p''
    Public Sub ResumeAutoRefresh(grid As DataGridView)
        Try
            If refreshTimers.ContainsKey(grid) Then
                refreshTimers(grid).Start()
            End If
        Catch
        End Try
    End Sub

    Public Function IsAutoRefreshPaused(grid As DataGridView) As Boolean
        Try
            If grid Is Nothing Then Return False
            If Not refreshTimers.ContainsKey(grid) Then Return False
            Return Not refreshTimers(grid).Enabled
        Catch
            Return False
        End Try
    End Function

    Private Sub HideStandardColumns(grid As DataGridView)
        Dim colsToHide() As String = {"ID", "CurrentIP", "is_logged_in"}
        Try
            Dim parentFormName As String = ""
            If TypeOf grid.FindForm() Is Form Then
                parentFormName = grid.FindForm().Name
            End If

            For Each colName In colsToHide
                If grid.Columns.Contains(colName) Then

                    If colName = "ID" AndAlso parentFormName.Equals("Inbox", StringComparison.OrdinalIgnoreCase) Then
                        Continue For
                    End If
                    grid.Columns(colName).Visible = False
                End If
            Next
        Catch
        End Try
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

    Private Sub RestoreSelection(grid As DataGridView,
                                  selectedColumn As String,
                                  selectedValue As Object,
                                  Optional currentColumn As String = "",
                                  Optional currentValue As Object = Nothing,
                                  Optional firstDisplayedRow As Integer = -1)

        Try
            If grid Is Nothing OrElse grid.IsDisposed Then Return

            grid.SuspendLayout()
            grid.ClearSelection()

            Dim restoredRow As DataGridViewRow = Nothing

            If selectedValue IsNot Nothing AndAlso
               Not String.IsNullOrEmpty(selectedColumn) AndAlso
               grid.Rows.Count > 0 AndAlso
               grid.Columns.Contains(selectedColumn) Then

                For Each row As DataGridViewRow In grid.Rows
                    If row.IsNewRow Then Continue For

                    Dim cellValue As Object = row.Cells(selectedColumn).Value

                    If cellValue IsNot Nothing AndAlso
                       Not IsDBNull(cellValue) AndAlso
                       cellValue.ToString() = selectedValue.ToString() Then

                        row.Selected = True
                        restoredRow = row
                        Exit For
                    End If
                Next
            End If

            If restoredRow IsNot Nothing Then
                If Not String.IsNullOrEmpty(currentColumn) AndAlso
                   grid.Columns.Contains(currentColumn) Then

                    Dim currentCellValue As Object = restoredRow.Cells(currentColumn).Value

                    If currentValue Is Nothing OrElse
                       (currentCellValue IsNot Nothing AndAlso
                        Not IsDBNull(currentCellValue) AndAlso
                        currentCellValue.ToString() = currentValue.ToString()) Then

                        grid.CurrentCell = restoredRow.Cells(currentColumn)
                    Else
                        grid.CurrentCell = restoredRow.Cells(selectedColumn)
                    End If
                Else
                    grid.CurrentCell = restoredRow.Cells(selectedColumn)
                End If

                If firstDisplayedRow >= 0 AndAlso grid.RowCount > 0 Then
                    Dim safeIndex As Integer = Math.Min(firstDisplayedRow, grid.RowCount - 1)
                    If safeIndex >= 0 Then
                        grid.FirstDisplayedScrollingRowIndex = safeIndex
                    End If
                ElseIf restoredRow.Index >= 0 AndAlso restoredRow.Index < grid.RowCount Then
                    grid.FirstDisplayedScrollingRowIndex = restoredRow.Index
                End If
            Else
                grid.CurrentCell = Nothing
            End If

        Catch ex As Exception
            Debug.WriteLine("RestoreSelection error: " & ex.Message)
        Finally
            Try
                grid.ResumeLayout()
            Catch
            End Try
        End Try
    End Sub

    'ito sa search textbox pang pause''
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


    Public Sub SendOverdueBorrowerNotifications()

        SyncLock OverdueEmailLock
            If OverdueProcessing Then
                Return
            End If
            OverdueProcessing = True
        End SyncLock

        Dim completed As Boolean = False

        Try
            EnsureInboxTableExists()

            Using con As New MySqlConnection(connectionString)
                con.Open()

                Dim candSql As String =
                    "SELECT bw.TransactionReceipt, be.Email, bw.Name, bw.DueDate " &
                    "FROM borrowing_tbl bw " &
                    "JOIN borroweredit_tbl be ON (bw.LRN = be.LRN OR bw.EmployeeNo = be.EmployeeNo) " &
                    "JOIN acession_tbl ac ON bw.AccessionID = ac.AccessionID " &
                    "WHERE (ac.Status = 'borrowed' OR ac.Status = 'overdue')"

                Dim candidates As New List(Of (receipt As String, email As String, name As String, dueStr As String))()
                Using candCmd As New MySqlCommand(candSql, con)
                    Using rdrCand = candCmd.ExecuteReader()
                        While rdrCand.Read()
                            Dim tr As String = If(rdrCand("TransactionReceipt") Is DBNull.Value, "", rdrCand("TransactionReceipt").ToString())
                            Dim em As String = If(rdrCand("Email") Is DBNull.Value, "", rdrCand("Email").ToString())
                            Dim nm As String = If(rdrCand("Name") Is DBNull.Value, "", rdrCand("Name").ToString())
                            Dim dd As String = If(rdrCand("DueDate") Is DBNull.Value, "", rdrCand("DueDate").ToString())
                            candidates.Add((tr, em, nm, dd))
                        End While
                        rdrCand.Close()
                    End Using
                End Using

                Dim dateFormats() As String = {"MMMM-dd-yyyy", "MMMM dd, yyyy", "yyyy-MM-dd", "MM/dd/yyyy", "M/d/yyyy", "MMMM-dd-yy"}
                Dim handledKeys As New HashSet(Of String)()

                For Each c In candidates
                    Try
                        If String.IsNullOrWhiteSpace(c.dueStr) Then Continue For
                        If String.IsNullOrWhiteSpace(c.receipt) Then Continue For

                        Dim dueDate As DateTime
                        Dim parsed As Boolean = DateTime.TryParse(c.dueStr, dueDate)
                        If Not parsed Then
                            For Each fmt In dateFormats
                                If DateTime.TryParseExact(c.dueStr, fmt, Globalization.CultureInfo.InvariantCulture, Globalization.DateTimeStyles.None, dueDate) Then
                                    parsed = True
                                    Exit For
                                End If
                            Next
                        End If

                        If Not parsed Then Continue For


                        Dim noticeKind As String
                        If dueDate.Date = Date.Today Then
                            noticeKind = NOTICE_DUE
                        ElseIf dueDate.Date < Date.Today Then
                            noticeKind = NOTICE_OVERDUE
                        Else
                            Continue For
                        End If

                        If Not handledKeys.Add(c.receipt & "|" & noticeKind) Then Continue For


                        Using existsCmd As New MySqlCommand("SELECT COUNT(*) FROM inbox_tbl WHERE TransactionReceipt = @tr AND NoticeType = @type", con)
                            existsCmd.Parameters.AddWithValue("@tr", c.receipt)
                            existsCmd.Parameters.AddWithValue("@type", noticeKind)
                            Dim cnt As Integer = Convert.ToInt32(existsCmd.ExecuteScalar())

                            If cnt = 0 Then
                                Using insCmd As New MySqlCommand("INSERT INTO inbox_tbl (TransactionReceipt, Email, FullName, Subject, Body, DueDate, `Date`, NoticeType) VALUES (@tr, @em, @nm, @sub, @body, @dueDate, NOW(), @type)", con)
                                    insCmd.Parameters.AddWithValue("@tr", c.receipt)
                                    insCmd.Parameters.AddWithValue("@em", c.email)
                                    insCmd.Parameters.AddWithValue("@nm", c.name)
                                    insCmd.Parameters.AddWithValue("@sub", If(noticeKind = NOTICE_DUE, "MDA-LMS Due Date Notice", "MDA-LMS Overdue Notice"))
                                    insCmd.Parameters.AddWithValue("@body", "Due Date: " & dueDate.ToString("yyyy-MM-dd"))
                                    insCmd.Parameters.AddWithValue("@dueDate", dueDate.ToString("yyyy-MM-dd"))
                                    insCmd.Parameters.AddWithValue("@type", noticeKind)
                                    insCmd.ExecuteNonQuery()
                                End Using
                            End If
                        End Using
                    Catch
                        Continue For
                    End Try
                Next


                ' ===== Ipadala ang lahat ng hindi pa naipapadala =====
                Dim pending As New List(Of (id As Integer, receipt As String, email As String, fullname As String, body As String, dueDate As String, noticeKind As String))()

                Dim selectSql As String =
                    "SELECT ID, TransactionReceipt, Email, FullName, Body, DueDate, COALESCE(NoticeType, 'OVERDUE') AS NoticeKind " &
                    "FROM inbox_tbl WHERE IsSent = 0 ORDER BY CreatedAt"

                Using selCmd As New MySqlCommand(selectSql, con)
                    Using rdr = selCmd.ExecuteReader()
                        While rdr.Read()
                            Dim email As String = If(rdr("Email") Is DBNull.Value, "", rdr("Email").ToString())
                            If String.IsNullOrWhiteSpace(email) Then Continue While

                            pending.Add((
                                Convert.ToInt32(rdr("ID")),
                                If(rdr("TransactionReceipt") Is DBNull.Value, "", rdr("TransactionReceipt").ToString()),
                                email,
                                If(rdr("FullName") Is DBNull.Value, "", rdr("FullName").ToString()),
                                If(rdr("Body") Is DBNull.Value, "", rdr("Body").ToString()),
                                If(rdr("DueDate") Is DBNull.Value, "", rdr("DueDate").ToString()),
                                rdr("NoticeKind").ToString()))
                        End While
                        rdr.Close()
                    End Using
                End Using

                If pending.Count > 0 Then
                    Dim processedInboxIds As New ConcurrentBag(Of Integer)()
                    Dim tasks As New List(Of Task)()

                    ' Isang email bawat tao bawat uri ng notice (Due Date at Overdue ay hiwalay)
                    For Each grp In pending.GroupBy(Function(p) p.email.Trim().ToLower() & "|" & p.noticeKind.ToUpper())
                        Dim items = grp.ToList()
                        Dim toEmail As String = items(0).email.Trim()
                        Dim isDueNotice As Boolean = items(0).noticeKind.Equals(NOTICE_DUE, StringComparison.OrdinalIgnoreCase)

                        tasks.Add(Task.Run(Sub()
                                               Try
                                                   Dim firstName As String = If(String.IsNullOrWhiteSpace(items(0).fullname), "Borrower", items(0).fullname)
                                                   Dim subjectText As String = If(isDueNotice, "MDA-LMS Due Date Notice", "MDA-LMS Overdue Notice")

                                                   Dim sb As New System.Text.StringBuilder()
                                                   sb.AppendLine("Hello " & firstName & ",")
                                                   sb.AppendLine()

                                                   If isDueNotice Then
                                                       sb.AppendLine("This is a friendly reminder that the following borrowed item(s) are due today:")
                                                   Else
                                                       sb.AppendLine("Our records show that you have the following overdue item(s):")
                                                   End If
                                                   sb.AppendLine()

                                                   For Each it In items
                                                       Dim dueTxt As String = If(String.IsNullOrWhiteSpace(it.dueDate), it.body, it.dueDate)
                                                       sb.AppendLine($"- Transaction: {it.receipt} | Due Date: {dueTxt}")
                                                   Next
                                                   sb.AppendLine()

                                                   If isDueNotice Then
                                                       sb.AppendLine("Please return the book(s) to the library today to avoid penalties.")
                                                   Else
                                                       sb.AppendLine("Please return the book(s) immediately to avoid penalties.")
                                                   End If
                                                   sb.AppendLine()
                                                   sb.AppendLine("Monlimar Development Academy Library Management System (MDA-LMS)")

                                                   Dim fullBody As String = sb.ToString()


                                                   Try
                                                       Using updCon As New MySqlConnection(connectionString)
                                                           updCon.Open()
                                                           For Each it In items
                                                               Using updCmd As New MySqlCommand("UPDATE inbox_tbl SET Body = @desc, `Date` = NOW() WHERE ID = @id", updCon)
                                                                   updCmd.Parameters.AddWithValue("@desc", fullBody)
                                                                   updCmd.Parameters.AddWithValue("@id", it.id)
                                                                   updCmd.ExecuteNonQuery()
                                                               End Using
                                                           Next
                                                       End Using
                                                   Catch exUpd As Exception
                                                       AppendLog("Failed to update inbox body: " & exUpd.Message)
                                                   End Try

                                                   Dim sentOk As Boolean = SendEmailNotification_Global(toEmail, subjectText, fullBody)
                                                   If sentOk Then
                                                       For Each it In items
                                                           processedInboxIds.Add(it.id)
                                                       Next
                                                   End If
                                               Catch ex As Exception
                                                   AppendLog("Notification task error: " & ex.Message)
                                               End Try
                                           End Sub))
                    Next

                    If tasks.Count > 0 Then
                        Task.WaitAll(tasks.ToArray())
                    End If


                    For Each id In processedInboxIds.ToArray().Distinct()
                        Try
                            Using updateCmd As New MySqlCommand("UPDATE inbox_tbl SET IsSent = 1, SentAt = NOW() WHERE ID = @id", con)
                                updateCmd.Parameters.AddWithValue("@id", id)
                                updateCmd.ExecuteNonQuery()
                            End Using
                        Catch ex As Exception
                            AppendLog("Failed to mark inbox row sent for ID " & id & ": " & ex.Message)
                        End Try
                    Next

                End If
            End Using

            completed = True
        Catch ex As Exception
            AppendLog("SendOverdueBorrowerNotifications error: " & ex.Message)
        Finally
            ' Mamarkahan lang na tapos na ngayong araw kung walang error (para mag-retry kung nag-fail ang DB)
            If completed Then
                Try
                    LastProcessedDate = Date.Today
                Catch
                End Try
            End If
            SyncLock OverdueEmailLock
                OverdueProcessing = False
            End SyncLock
        End Try
    End Sub

    Private Sub dailyOverdueTimer_Tick(sender As Object, e As EventArgs) Handles dailyOverdueTimer.Tick
        Try
            If LastProcessedDate < Date.Today Then

                Task.Run(Sub()
                             Try
                                 SendOverdueBorrowerNotifications()
                             Catch
                             End Try
                         End Sub)
            End If
        Catch
        End Try
    End Sub

    Private Sub inboxCheckTimer_Tick(sender As Object, e As EventArgs) Handles inboxCheckTimer.Tick
        Try

            Using con As New MySqlConnection(connectionString)
                Dim sql As String = "SELECT COUNT(*) FROM inbox_tbl WHERE IsSent = 0"
                Using cmd As New MySqlCommand(sql, con)
                    Try
                        con.Open()
                        Dim cntObj = cmd.ExecuteScalar()
                        Dim cnt As Integer = 0
                        If cntObj IsNot Nothing AndAlso cntObj IsNot DBNull.Value Then
                            Integer.TryParse(cntObj.ToString(), cnt)
                        End If
                        If cnt > 0 Then
                            Task.Run(Sub()
                                         Try
                                             SendOverdueBorrowerNotifications()
                                         Catch
                                         End Try
                                     End Sub)
                        End If
                    Catch
                    Finally
                        If con.State = ConnectionState.Open Then con.Close()
                    End Try
                End Using
            End Using

            Try
                Using con2 As New MySqlConnection(connectionString)
                    Using adap As New MySqlDataAdapter("SELECT ID, FullName, COALESCE(DueDate, Body) AS DueDate, COALESCE(`Date`, CreatedAt) AS `Date` FROM inbox_tbl ORDER BY COALESCE(`Date`, CreatedAt) DESC", con2)
                        Dim dt As New DataTable()
                        adap.Fill(dt)
                        SyncLock OverdueEmailLock
                            inboxCache = dt
                        End SyncLock
                        RaiseEvent InboxUpdated()
                    End Using
                End Using
            Catch
            End Try
        Catch
        End Try
    End Sub


    Private Function SendEmailNotification_Global(targetEmail As String, subject As String, body As String) As Boolean

        Dim fromEmail As String = ""
        Dim appPassword As String = ""

        GetEmailConfig_Global(fromEmail, appPassword)
        If String.IsNullOrWhiteSpace(fromEmail) OrElse String.IsNullOrWhiteSpace(appPassword) Then
            AppendLog("Email config missing; cannot send email to " & targetEmail)
            Return False
        End If

        Try
            Dim mail As New MailMessage(fromEmail, targetEmail, subject, body)
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12

            Using smtp As New SmtpClient("smtp.gmail.com", 587)
                smtp.EnableSsl = True
                smtp.Credentials = New NetworkCredential(fromEmail, appPassword)
                smtp.Send(mail)
            End Using
            Return True
        Catch ex As Exception
            AppendLog("SMTP error sending to " & targetEmail & ": " & ex.Message)
            Return False
        End Try

    End Function


    Private Sub GetEmailConfig_Global(ByRef email As String, ByRef password As String)

        Using con As New MySqlConnection(connectionString)
            Using cmd As New MySqlCommand("SELECT Email, AppPassword FROM email_config LIMIT 1", con)
                con.Open()

                Using rdr = cmd.ExecuteReader()
                    If rdr.Read() Then
                        email = rdr("Email").ToString()
                        password = rdr("AppPassword").ToString()
                    End If
                End Using
            End Using
        End Using

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


                ' Do not rebind controls while the user is interacting with the form.
                If IsUserEditingForm(cb) Then
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

    Public Function IsDatabaseConnected() As Boolean
        Try
            Using con As New MySqlConnection(connectionString)
                con.Open()
                Return True
            End Using
        Catch
            Return False
        End Try
    End Function

    Private Function SafeCellValue(row As DataGridViewRow, columnName As String) As String
        Try
            If row.Cells(columnName).Value IsNot Nothing Then
                Return row.Cells(columnName).Value.ToString()
            End If
        Catch
        End Try
        Return ""
    End Function

    Public Sub TriggerDatabaseUpdated()

        RaiseEvent DatabaseUpdated()

    End Sub

    Private Const NOTICE_DUE As String = "DUE"
    Private Const NOTICE_OVERDUE As String = "OVERDUE"

    Public OverdueEmailAlreadySent As Boolean = False
    Public LastProcessedDate As Date = Date.MinValue
    Private OverdueEmailLock As New Object()
    Private OverdueProcessing As Boolean = False
    Private WithEvents dailyOverdueTimer As New Timer() With {.Interval = 60 * 1000}
    Private WithEvents inboxCheckTimer As New Timer() With {.Interval = 1000}
    Public inboxCache As DataTable = Nothing
    Public Event InboxUpdated()

    Public SuppressShutdownCleanup As Boolean = False


    Private Async Sub backgroundInboxTimer_Tick(sender As Object, e As EventArgs) Handles backgroundInboxTimer.Tick

        backgroundInboxTimer.Stop()

        Try
            Using con As New MySqlConnection(connectionString)
                Await con.OpenAsync()


                Dim overdueQuery As String = "SELECT b.Email, b.FullName, br.BookTitle, br.DueDate " &
                                         "FROM borrowing_tbl br " &
                                         "INNER JOIN borrower_tbl b ON br.BorrowerID = b.BorrowerID " &
                                         "WHERE br.DueDate < NOW() AND br.Status = 'Borrowed' " &
                                         "AND NOT EXISTS (SELECT 1 FROM inbox_tbl i WHERE i.Email = b.Email AND DATE(i.CreatedAt) = DATE(NOW()) AND i.Subject LIKE '%Overdue%')"

                Dim overdueList As New List(Of Dictionary(Of String, String))

                Using cmd As New MySqlCommand(overdueQuery, con)
                    Using rdr = Await cmd.ExecuteReaderAsync()
                        While Await rdr.ReadAsync()
                            Dim data As New Dictionary(Of String, String) From {
                            {"Email", rdr("Email").ToString()},
                            {"FullName", rdr("FullName").ToString()},
                            {"BookTitle", rdr("BookTitle").ToString()},
                            {"DueDate", Convert.ToDateTime(rdr("DueDate")).ToString("yyyy-MM-dd")}
                        }
                            overdueList.Add(data)
                        End While
                    End Using
                End Using


                For Each row In overdueList
                    Dim emailTo As String = row("Email")
                    Dim nameTo As String = row("FullName")
                    Dim book As String = row("BookTitle")
                    Dim due As String = row("DueDate")

                    Dim subject As String = "Overdue Book Notification"
                    Dim body As String = $"Dear {nameTo}," & Environment.NewLine & Environment.NewLine &
                                     $"This is to notify you that the book '{book}' you borrowed was due on {due}." & Environment.NewLine &
                                     "Please return it to the library as soon as possible to avoid further penalties."


                    Dim isSentSuccess As Integer = 0
                    Try
                        Dim mail As New MailMessage("email@gmail.com", emailTo, subject, body)
                        Dim smtp As New SmtpClient("smtp.gmail.com")
                        smtp.Port = 587
                        smtp.EnableSsl = True
                        smtp.Credentials = New NetworkCredential("email@gmail.com", "apppass")


                        isSentSuccess = 1
                        AppendLog($"Email successfully sent to {emailTo} for book '{book}'")
                    Catch ex As Exception
                        isSentSuccess = 0
                        AppendLog($"Email failed to send to {emailTo}: {ex.Message}")
                    End Try


                    Dim insertSql As String = "INSERT INTO `inbox_tbl` (Email, FullName, Subject, Body, IsSent, DueDate, `Date`, CreatedAt, SentAt) " &
                                          "VALUES (@email, @name, @subject, @body, @isSent, @dueDate, NOW(), NOW(), @sentAt)"

                    Using insCmd As New MySqlCommand(insertSql, con)
                        insCmd.Parameters.AddWithValue("@email", emailTo)
                        insCmd.Parameters.AddWithValue("@name", nameTo)
                        insCmd.Parameters.AddWithValue("@subject", subject)
                        insCmd.Parameters.AddWithValue("@body", body)
                        insCmd.Parameters.AddWithValue("@isSent", isSentSuccess)
                        insCmd.Parameters.AddWithValue("@dueDate", due)
                        insCmd.Parameters.AddWithValue("@sentAt", If(isSentSuccess = 1, DateTime.Now, DBNull.Value))

                        Await insCmd.ExecuteNonQueryAsync()
                    End Using
                Next

            End Using
        Catch ex As Exception
            AppendLog($"Background Monitor Error: {ex.Message}")
        Finally

            backgroundInboxTimer.Start()
        End Try
    End Sub


End Module