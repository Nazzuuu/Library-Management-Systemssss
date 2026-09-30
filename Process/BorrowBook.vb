Imports MySql.Data.MySqlClient
Imports System.Data
Imports System.Drawing
Imports System.Threading.Tasks
Imports ZXing
Imports ZXing.Rendering
Imports ZXing.Windows.Compatibility

Public Class BorrowBook

    Private Const ACC_TABLE As String = "acession_tbl"
    Private Const ACC_NO_COL As String = "AccessionID"
    Private Const ACC_TITLE_COL As String = "BookTitle"
    Private Const ACC_STATUS_COL As String = "Status"
    Private Const ACC_TXN_COL As String = "TransactionNo"
    Private Const ACC_AVAILABLE As String = "Available"
    Private Const ACC_BORROWED As String = "Borrowed"

    Private Const ACQ_TABLE As String = "acquisition_tbl"
    Private Const ACQ_TITLE_COL As String = "BookTitle"
    Private Const ACQ_QTY_COL As String = "Quantity"
    Private Const ACQ_DATE_COL As String = "DateAcquired"
    Private Const ACQ_TXN_COL As String = "TransactionNo"

    Private Const AVL_TABLE As String = "availablebook_tbl"
    Private Const AVL_TITLE_COL As String = "BookTitle"
    Private Const AVL_QTY_COL As String = "AvailableBooks"
    Private Const AVL_DATE_COL As String = "DateAcquired"

    Private Const BRW_TABLE As String = "borrowing_tbl"
    Private Const HIS_TABLE As String = "borrowinghistory_tbl"
    Private Const PRT_TABLE As String = "printreceipt_tbl"

    Private Const HIS_RECEIPT_COL As String = "TransactionReceipt"
    Private Const HIS_DATE_COL As String = "BorrowedDate"

    Private Const PRT_RECEIPT_COL As String = "TransactionReceipt"
    Private Const PRT_COUNT_COL As String = "BorrowedBookCount"
    Private Const PRT_BORROWED_FORMAT As String = "MMMM-dd-yyyy"
    Private Const PRT_DUE_FORMAT As String = "yyyy-MM-dd"

    Private Const SHOW_SKIPPED_COLUMNS_NOTE As Boolean = False

    Private Const AUTO_REFRESH_MS As Integer = 2000

    Private maxBooks As Integer = 0
    Private borrowedToday As Integer = 0
    Private ReadOnly selectedBooks As New Dictionary(Of String, Dictionary(Of String, Object))
    Private isLoadingData As Boolean = False
    Private lastEnteredBorrowerID As String = ""

    Private ReadOnly columnCache As New Dictionary(Of String, HashSet(Of String))(StringComparer.OrdinalIgnoreCase)
    Private ReadOnly lastSkippedColumns As New HashSet(Of String)
    Private skippedNoteShown As Boolean = False

    Private booksTimer As System.Windows.Forms.Timer
    Private availableTimer As System.Windows.Forms.Timer
    Private stateTimer As System.Windows.Forms.Timer
    Private isRefreshingBooks As Boolean = False
    Private isRefreshingAvailable As Boolean = False
    Private isRefreshingState As Boolean = False
    Private booksLoadVersion As Integer = 0
    Private availLoadVersion As Integer = 0

    Private Class ReceiptInfo
        Public Property ReceiptNo As String
        Public Property ReceiptDate As Date
    End Class
    Private ReadOnly receiptByBorrower As New Dictionary(Of String, ReceiptInfo)


    Private Sub BorrowBook_SetupSelectGrid(sender As Object, e As EventArgs) Handles MyBase.Load
        DataGridView2.ReadOnly = True
        DataGridView2.AllowUserToAddRows = False
        DataGridView2.SelectionMode = DataGridViewSelectionMode.FullRowSelect

        DataGridView1.ReadOnly = True
        DataGridView1.AllowUserToAddRows = False
        DataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect

        EnableDoubleBuffer(DataGridView1)
        EnableDoubleBuffer(DataGridView2)

        ApplyViewLinkAccess()
        ApplyBorrowerType()
        LoadSelectBooks()
        LoadAvailableBooks()

        StartAutoRefreshTimers()
    End Sub

    Private Sub BorrowBook_VisibleChanged(sender As Object, e As EventArgs) Handles MyBase.VisibleChanged
        If Me.Visible Then ApplyViewLinkAccess()
    End Sub

    Private Sub BorrowBook_Disposed(sender As Object, e As EventArgs) Handles MyBase.Disposed
        StopAutoRefreshTimers()
    End Sub

    Private Sub EnableDoubleBuffer(grid As DataGridView)
        Try
            Dim pi As System.Reflection.PropertyInfo =
                GetType(DataGridView).GetProperty("DoubleBuffered",
                    System.Reflection.BindingFlags.Instance Or System.Reflection.BindingFlags.NonPublic)
            If pi IsNot Nothing Then pi.SetValue(grid, True, Nothing)
        Catch
        End Try
    End Sub


    Private Sub StartAutoRefreshTimers()
        StopAutoRefreshTimers()

        booksTimer = New System.Windows.Forms.Timer() With {.Interval = AUTO_REFRESH_MS}
        AddHandler booksTimer.Tick, AddressOf BooksTimer_Tick
        GlobalVarsModule.refreshTimers(DataGridView2) = booksTimer
        booksTimer.Start()

        availableTimer = New System.Windows.Forms.Timer() With {.Interval = AUTO_REFRESH_MS}
        AddHandler availableTimer.Tick, AddressOf AvailableTimer_Tick
        GlobalVarsModule.refreshTimers(DataGridView1) = availableTimer
        availableTimer.Start()

        stateTimer = New System.Windows.Forms.Timer() With {.Interval = AUTO_REFRESH_MS}
        AddHandler stateTimer.Tick, AddressOf StateTimer_Tick
        stateTimer.Start()
    End Sub

    Private Sub StopAutoRefreshTimers()
        If booksTimer IsNot Nothing Then
            booksTimer.Stop()
            RemoveHandler booksTimer.Tick, AddressOf BooksTimer_Tick
            Try
                GlobalVarsModule.refreshTimers.Remove(DataGridView2)
            Catch
            End Try
            booksTimer.Dispose()
            booksTimer = Nothing
        End If

        If availableTimer IsNot Nothing Then
            availableTimer.Stop()
            RemoveHandler availableTimer.Tick, AddressOf AvailableTimer_Tick
            Try
                GlobalVarsModule.refreshTimers.Remove(DataGridView1)
            Catch
            End Try
            availableTimer.Dispose()
            availableTimer = Nothing
        End If

        If stateTimer IsNot Nothing Then
            stateTimer.Stop()
            RemoveHandler stateTimer.Tick, AddressOf StateTimer_Tick
            stateTimer.Dispose()
            stateTimer = Nothing
        End If
    End Sub

    Private Async Sub BooksTimer_Tick(sender As Object, e As EventArgs)
        If isRefreshingBooks OrElse Me.IsDisposed OrElse Not Me.Visible Then Return

        isRefreshingBooks = True
        Dim version As Integer = booksLoadVersion
        Try
            Dim dt As DataTable = Await Task.Run(Function() FetchSelectBooksData())

            If Me.IsDisposed OrElse version <> booksLoadVersion Then Return
            ApplySelectBooksData(dt)
        Catch ex As Exception
            Debug.WriteLine("Auto refresh (books) error: " & ex.Message)
        Finally
            isRefreshingBooks = False
        End Try
    End Sub

    Private Async Sub AvailableTimer_Tick(sender As Object, e As EventArgs)
        If isRefreshingAvailable OrElse Me.IsDisposed OrElse Not Me.Visible Then Return

        isRefreshingAvailable = True
        Dim version As Integer = availLoadVersion
        Try
            Dim dt As DataTable = Await Task.Run(Function() FetchAvailableBooksData())

            If Me.IsDisposed OrElse version <> availLoadVersion Then Return
            ApplyAvailableBooksData(dt)
        Catch ex As Exception
            Debug.WriteLine("Auto refresh (available) error: " & ex.Message)
        Finally
            isRefreshingAvailable = False
        End Try
    End Sub

    Private Async Sub StateTimer_Tick(sender As Object, e As EventArgs)
        If isRefreshingState OrElse Me.IsDisposed OrElse Not Me.Visible Then Return
        If maxBooks = 0 OrElse txtname.Text.Trim() = "" Then Return

        Dim idType As String = If(rbstudent.Checked, "LRN", "EmployeeNo")
        Dim id As String = If(rbstudent.Checked, txtlrnsu.Text.Trim(), txtemployee.Text.Trim())
        Dim receipt As String = lbltransac.Text.Trim()
        If id = "" Then Return

        isRefreshingState = True
        Try
            Dim st As (TimedIn As Boolean, Count As Integer) =
                Await Task.Run(Function() FetchBorrowerState(idType, id, receipt))

            If Me.IsDisposed Then Return

            Dim currentId As String = If(rbstudent.Checked, txtlrnsu.Text.Trim(), txtemployee.Text.Trim())
            If currentId <> id OrElse lbltransac.Text.Trim() <> receipt Then Return

            Dim shouldShow As Boolean = Not st.TimedIn
            If btntimein.Visible <> shouldShow Then btntimein.Visible = shouldShow

            If borrowedToday <> st.Count Then
                borrowedToday = st.Count
                UpdateSelectLabel()
                If selectedBooks.Count > RemainingBooks() Then ClearSelectedBooks()
            End If
        Catch ex As Exception
            Debug.WriteLine("Auto refresh (state) error: " & ex.Message)
        Finally
            isRefreshingState = False
        End Try
    End Sub

    Private Function FetchBorrowerState(idType As String, id As String, receiptNo As String) As (TimedIn As Boolean, Count As Integer)
        Dim timedIn As Boolean = False
        Dim cnt As Integer = 0

        Using con As New MySqlConnection(GlobalVarsModule.connectionString)
            con.Open()

            Using cmd As New MySqlCommand($"SELECT COUNT(*) FROM `oras_tbl` WHERE `{idType}` = @id AND `TimeOut` IS NULL", con)
                cmd.Parameters.AddWithValue("@id", id)
                Dim r As Object = cmd.ExecuteScalar()
                If r IsNot Nothing AndAlso Not IsDBNull(r) Then timedIn = Convert.ToInt32(r) > 0
            End Using

            If receiptNo <> "" Then
                cnt = CountBorrowedForReceipt(con, Nothing, receiptNo)
            End If
        End Using

        Return (timedIn, cnt)
    End Function


    Private Function ValuesEqual(a As Object, b As Object) As Boolean
        Dim aNull As Boolean = (a Is Nothing OrElse IsDBNull(a))
        Dim bNull As Boolean = (b Is Nothing OrElse IsDBNull(b))
        If aNull AndAlso bNull Then Return True
        If aNull OrElse bNull Then Return False
        Return String.Equals(Convert.ToString(a), Convert.ToString(b), StringComparison.Ordinal)
    End Function

    Private Function SyncTableInPlace(target As DataTable, source As DataTable, keyCol As String, valueCols As String()) As Boolean
        Dim changed As Boolean = False

        Dim srcByKey As New Dictionary(Of String, DataRow)(StringComparer.OrdinalIgnoreCase)
        For Each r As DataRow In source.Rows
            Dim k As String = Convert.ToString(r(keyCol)).Trim()
            If Not srcByKey.ContainsKey(k) Then srcByKey(k) = r
        Next

        Dim tgtByKey As New Dictionary(Of String, DataRow)(StringComparer.OrdinalIgnoreCase)
        Dim toRemove As New List(Of DataRow)
        For Each r As DataRow In target.Rows
            Dim k As String = Convert.ToString(r(keyCol)).Trim()
            If srcByKey.ContainsKey(k) AndAlso Not tgtByKey.ContainsKey(k) Then
                tgtByKey(k) = r
            Else
                toRemove.Add(r)
            End If
        Next

        For Each r As DataRow In toRemove
            target.Rows.Remove(r)
            changed = True
        Next

        For Each kv As KeyValuePair(Of String, DataRow) In tgtByKey
            Dim srcRow As DataRow = srcByKey(kv.Key)
            For Each c As String In valueCols
                If Not ValuesEqual(kv.Value(c), srcRow(c)) Then
                    kv.Value(c) = srcRow(c)
                    changed = True
                End If
            Next
        Next

        Dim added As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)
        For Each r As DataRow In source.Rows
            Dim k As String = Convert.ToString(r(keyCol)).Trim()
            If tgtByKey.ContainsKey(k) OrElse added.Contains(k) Then Continue For

            Dim nr As DataRow = target.NewRow()
            For Each col As DataColumn In target.Columns
                If source.Columns.Contains(col.ColumnName) Then
                    nr(col.ColumnName) = r(col.ColumnName)
                End If
            Next
            target.Rows.Add(nr)
            added.Add(k)
            changed = True
        Next

        Return changed
    End Function


    Private Function IsBorrowerRole() As Boolean
        Return String.Equals(GlobalVarsModule.CurrentUserRole, "Borrower", StringComparison.OrdinalIgnoreCase)
    End Function

    Private Sub ApplyViewLinkAccess()
        view_link.Visible = IsBorrowerRole()
    End Sub


    Private Function FetchSelectBooksData() As DataTable
        Dim titles As New List(Of String)
        Dim dt As New DataTable()

        Using con As New MySqlConnection(GlobalVarsModule.connectionString)
            con.Open()

            Dim titleSql As String =
                $"SELECT DISTINCT TRIM(`{ACC_TITLE_COL}`) FROM `{ACC_TABLE}` " &
                $"WHERE TRIM(`{ACC_STATUS_COL}`) = @avail ORDER BY TRIM(`{ACC_TITLE_COL}`)"

            Using cmd As New MySqlCommand(titleSql, con)
                cmd.Parameters.AddWithValue("@avail", ACC_AVAILABLE)
                Using rdr As MySqlDataReader = cmd.ExecuteReader()
                    While rdr.Read()
                        If Not rdr.IsDBNull(0) Then
                            Dim t As String = rdr.GetString(0).Trim()
                            If t <> "" Then titles.Add(t)
                        End If
                    End While
                End Using
            End Using

            Dim existing As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)
            Using cmd As New MySqlCommand("SELECT `BookTitle` FROM `selectbook_tbl`", con)
                Using rdr As MySqlDataReader = cmd.ExecuteReader()
                    While rdr.Read()
                        If Not rdr.IsDBNull(0) Then existing.Add(rdr.GetString(0).Trim())
                    End While
                End Using
            End Using

            Dim newSet As New HashSet(Of String)(titles, StringComparer.OrdinalIgnoreCase)

            For Each t As String In existing
                If Not newSet.Contains(t) Then
                    Using cmd As New MySqlCommand("DELETE FROM `selectbook_tbl` WHERE TRIM(`BookTitle`) = @t", con)
                        cmd.Parameters.AddWithValue("@t", t)
                        cmd.ExecuteNonQuery()
                    End Using
                End If
            Next

            For Each t As String In titles
                If Not existing.Contains(t) Then
                    Using cmd As New MySqlCommand("INSERT INTO `selectbook_tbl` (`BookTitle`) VALUES (@t)", con)
                        cmd.Parameters.AddWithValue("@t", t)
                        cmd.ExecuteNonQuery()
                    End Using
                    existing.Add(t)
                End If
            Next

            Using da As New MySqlDataAdapter("SELECT `BookTitle` FROM `selectbook_tbl` ORDER BY `BookTitle`", con)
                da.Fill(dt)
            End Using
        End Using

        Return dt
    End Function

    Private Sub ApplySelectBooksData(dt As DataTable)
        If Not TypeOf DataGridView2.DataSource Is DataTable Then
            DataGridView2.AutoGenerateColumns = True
            DataGridView2.DataSource = dt
            ApplyGridFilter(DataGridView2, txtsearch.Text.Trim())
            RestoreChecks()
            Return
        End If

        Dim target As DataTable = DirectCast(DataGridView2.DataSource, DataTable)
        Dim changed As Boolean = SyncTableInPlace(target, dt, "BookTitle", New String() {})

        Dim titles As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)
        For Each r As DataRow In target.Rows
            titles.Add(Convert.ToString(r("BookTitle")).Trim())
        Next

        Dim stale As New List(Of String)
        For Each k As String In selectedBooks.Keys
            If Not titles.Contains(k.Trim()) Then stale.Add(k)
        Next
        For Each k As String In stale
            selectedBooks.Remove(k)
            changed = True
        Next

        If changed Then RestoreChecks()
    End Sub

    Private Sub LoadSelectBooks(Optional silent As Boolean = False)
        booksLoadVersion += 1
        Try
            Dim dt As DataTable = FetchSelectBooksData()
            ApplySelectBooksData(dt)
        Catch ex As Exception
            If silent Then
                Debug.WriteLine("Error loading books: " & ex.Message)
            Else
                MessageBox.Show("Error loading books: " & ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End If
        End Try
    End Sub

    Private Sub SetupGridColumns()
        If DataGridView2.Columns.Contains("chk") Then
            DataGridView2.Columns("chk").HeaderText = "SELECT"
            DataGridView2.Columns("chk").DisplayIndex = 0
        End If

        If DataGridView2.Columns.Contains("BookTitle") Then
            DataGridView2.Columns("BookTitle").HeaderText = "BOOK TITLE"
            DataGridView2.Columns("BookTitle").ReadOnly = True
            DataGridView2.Columns("BookTitle").AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
        End If

        If DataGridView2.Columns.Contains("colAccNo") Then
            DataGridView2.Columns.Remove("colAccNo")
        End If
    End Sub


    Private Function FetchAvailableBooksData() As DataTable
        Dim dt As New DataTable()

        Using con As New MySqlConnection(GlobalVarsModule.connectionString)
            con.Open()

            Dim deleteSql As String =
                $"DELETE a FROM `{AVL_TABLE}` a WHERE NOT EXISTS (" &
                $"SELECT 1 FROM `{ACC_TABLE}` c " &
                $"WHERE TRIM(c.`{ACC_TITLE_COL}`) = TRIM(a.`{AVL_TITLE_COL}`) AND TRIM(c.`{ACC_STATUS_COL}`) = @avail)"

            Using cmdDel As New MySqlCommand(deleteSql, con)
                cmdDel.Parameters.AddWithValue("@avail", ACC_AVAILABLE)
                cmdDel.ExecuteNonQuery()
            End Using

            Dim insertSql As String =
                $"INSERT INTO `{AVL_TABLE}` (`{AVL_TITLE_COL}`, `{AVL_QTY_COL}`, `{AVL_DATE_COL}`) " &
                $"SELECT t.Title, COALESCE(q.Qty, 0), q.DateAcq " &
                $"FROM (SELECT DISTINCT TRIM(`{ACC_TITLE_COL}`) AS Title FROM `{ACC_TABLE}` WHERE TRIM(`{ACC_STATUS_COL}`) = @avail) t " &
                $"LEFT JOIN (SELECT TRIM(`{ACQ_TITLE_COL}`) AS Title, SUM(`{ACQ_QTY_COL}`) AS Qty, MAX(`{ACQ_DATE_COL}`) AS DateAcq " &
                $"FROM `{ACQ_TABLE}` GROUP BY TRIM(`{ACQ_TITLE_COL}`)) q ON q.Title = t.Title " &
                $"WHERE NOT EXISTS (SELECT 1 FROM `{AVL_TABLE}` a WHERE TRIM(a.`{AVL_TITLE_COL}`) = t.Title)"

            Using cmdIns As New MySqlCommand(insertSql, con)
                cmdIns.Parameters.AddWithValue("@avail", ACC_AVAILABLE)
                cmdIns.ExecuteNonQuery()
            End Using

            Dim selectSql As String =
                $"SELECT `{AVL_TITLE_COL}`, `{AVL_QTY_COL}`, `{AVL_DATE_COL}` FROM `{AVL_TABLE}` ORDER BY `{AVL_TITLE_COL}`"

            Using da As New MySqlDataAdapter(selectSql, con)
                da.Fill(dt)
            End Using
        End Using

        Return dt
    End Function

    Private Sub ApplyAvailableBooksData(dt As DataTable)
        If Not TypeOf DataGridView1.DataSource Is DataTable Then
            DataGridView1.AutoGenerateColumns = True
            DataGridView1.DataSource = dt
            SetupAvailableGridColumns()
            ApplyGridFilter(DataGridView1, txtsearch1.Text.Trim())
            Return
        End If

        Dim target As DataTable = DirectCast(DataGridView1.DataSource, DataTable)
        SyncTableInPlace(target, dt, AVL_TITLE_COL, New String() {AVL_QTY_COL, AVL_DATE_COL})
    End Sub

    Private Sub LoadAvailableBooks(Optional silent As Boolean = False)
        availLoadVersion += 1
        Try
            Dim dt As DataTable = FetchAvailableBooksData()
            ApplyAvailableBooksData(dt)
        Catch ex As Exception
            If silent Then
                Debug.WriteLine("Error loading available books: " & ex.Message)
            Else
                MessageBox.Show("Error loading available books: " & ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End If
        End Try
    End Sub

    Private Sub SetupAvailableGridColumns()
        If DataGridView1.Columns.Contains(AVL_TITLE_COL) Then
            DataGridView1.Columns(AVL_TITLE_COL).HeaderText = "BOOK TITLE"
            DataGridView1.Columns(AVL_TITLE_COL).AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
        End If

        If DataGridView1.Columns.Contains(AVL_QTY_COL) Then
            DataGridView1.Columns(AVL_QTY_COL).HeaderText = "AVAILABLE"
            DataGridView1.Columns(AVL_QTY_COL).AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells
        End If

        If DataGridView1.Columns.Contains(AVL_DATE_COL) Then
            DataGridView1.Columns(AVL_DATE_COL).HeaderText = "DATE ACQUIRED"
            DataGridView1.Columns(AVL_DATE_COL).DefaultCellStyle.Format = "MMM dd, yyyy"
            DataGridView1.Columns(AVL_DATE_COL).AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells
        End If
    End Sub

    Private Sub DataGridView1_DataBindingComplete(sender As Object, e As DataGridViewBindingCompleteEventArgs) Handles DataGridView1.DataBindingComplete
        If e.ListChangedType = System.ComponentModel.ListChangedType.Reset Then
            DataGridView1.ClearSelection()
            DataGridView1.CurrentCell = Nothing
        End If
    End Sub

    Private Sub DecrementAvailableBook(con As MySqlConnection, tran As MySqlTransaction, title As String)
        Dim sql As String =
            $"UPDATE `{AVL_TABLE}` SET `{AVL_QTY_COL}` = IF(`{AVL_QTY_COL}` > 0, `{AVL_QTY_COL}` - 1, 0) " &
            $"WHERE TRIM(`{AVL_TITLE_COL}`) = TRIM(@t)"

        Using cmd As New MySqlCommand(sql, con, tran)
            cmd.Parameters.AddWithValue("@t", title)
            cmd.ExecuteNonQuery()
        End Using
    End Sub

    Private Sub DecrementAcquisitionQuantity(con As MySqlConnection, tran As MySqlTransaction, title As String, txnNo As String)
        Dim affected As Integer = 0

        If Not String.IsNullOrWhiteSpace(txnNo) Then
            Dim sqlExact As String =
                $"UPDATE `{ACQ_TABLE}` SET `{ACQ_QTY_COL}` = IF(`{ACQ_QTY_COL}` > 0, `{ACQ_QTY_COL}` - 1, 0) " &
                $"WHERE TRIM(`{ACQ_TXN_COL}`) = TRIM(@tn) AND TRIM(`{ACQ_TITLE_COL}`) = TRIM(@t)"

            Using cmd As New MySqlCommand(sqlExact, con, tran)
                cmd.Parameters.AddWithValue("@tn", txnNo)
                cmd.Parameters.AddWithValue("@t", title)
                affected = cmd.ExecuteNonQuery()
            End Using
        End If

        If affected = 0 Then
            Dim sqlFallback As String =
                $"UPDATE `{ACQ_TABLE}` SET `{ACQ_QTY_COL}` = `{ACQ_QTY_COL}` - 1 " &
                $"WHERE TRIM(`{ACQ_TITLE_COL}`) = TRIM(@t) AND `{ACQ_QTY_COL}` > 0 " &
                $"ORDER BY `{ACQ_DATE_COL}` ASC LIMIT 1"

            Using cmd As New MySqlCommand(sqlFallback, con, tran)
                cmd.Parameters.AddWithValue("@t", title)
                cmd.ExecuteNonQuery()
            End Using
        End If
    End Sub


    Private Sub BorrowerType_CheckedChanged(sender As Object, e As EventArgs) Handles rbstudent.CheckedChanged, rbteacher.CheckedChanged
        ApplyBorrowerType()
    End Sub

    Private Sub ApplyBorrowerType()
        If rbstudent.Checked Then
            txtlrnsu.Enabled = True
            txtemployee.Enabled = False
            txtemployee.Clear()
            maxBooks = 3
        ElseIf rbteacher.Checked Then
            txtemployee.Enabled = True
            txtlrnsu.Enabled = False
            txtlrnsu.Clear()
            maxBooks = 5
        Else
            txtlrnsu.Enabled = False
            txtemployee.Enabled = False
            maxBooks = 0
        End If

        btntimein.Enabled = (rbstudent.Checked OrElse rbteacher.Checked)

        borrowedToday = 0
        UpdateSelectLabel()
        ClearSelectedBooks()
    End Sub

    Private Function RemainingBooks() As Integer
        Return Math.Max(maxBooks - borrowedToday, 0)
    End Function

    Private Sub UpdateSelectLabel()
        If maxBooks = 0 Then
            lblselectbooks.Text = "[Select Borrower Type first] Double Click the cell to select. Enter to Confirm."
        ElseIf RemainingBooks() = 0 Then
            lblselectbooks.Text = $"[Limit reached: {borrowedToday} of {maxBooks} Books already borrowed] You can't select more books."
        Else
            lblselectbooks.Text = $"[You can select {RemainingBooks()} Books] Double Click the cell to select. Enter to Confirm."
        End If
    End Sub

    Private Sub ResetBorrowedCount()
        borrowedToday = 0
        UpdateSelectLabel()
    End Sub

    Private Function CountBorrowedForReceipt(con As MySqlConnection, tran As MySqlTransaction, receiptNo As String) As Integer
        If String.IsNullOrWhiteSpace(receiptNo) Then Return 0

        Dim sql As String = $"SELECT COUNT(*) FROM `{HIS_TABLE}` WHERE `{HIS_RECEIPT_COL}` = @r"
        Using cmd As New MySqlCommand(sql, con, tran)
            cmd.Parameters.AddWithValue("@r", receiptNo)
            Dim r As Object = cmd.ExecuteScalar()
            If r Is Nothing OrElse IsDBNull(r) Then Return 0
            Return Convert.ToInt32(r)
        End Using
    End Function

    Private Sub RefreshBorrowedCount()
        borrowedToday = 0

        If lbltransac.Text.Trim() <> "" Then
            Try
                Using con As New MySqlConnection(GlobalVarsModule.connectionString)
                    con.Open()
                    borrowedToday = CountBorrowedForReceipt(con, Nothing, lbltransac.Text.Trim())
                End Using
            Catch
                borrowedToday = 0
            End Try
        End If

        UpdateSelectLabel()

        If selectedBooks.Count > RemainingBooks() Then
            ClearSelectedBooks()
        End If
    End Sub


    Private Sub btntimein_Click(sender As Object, e As EventArgs) Handles btntimein.Click

        Dim borrowerID As String = ""
        Dim borrowerType As String = ""
        Dim borrowerName As String = ""
        Dim borrowertayp As String = ""

        If rbstudent.Checked AndAlso Not String.IsNullOrWhiteSpace(txtlrnsu.Text) Then
            borrowerID = txtlrnsu.Text.Trim()
            borrowerType = "LRN"
        ElseIf rbteacher.Checked AndAlso Not String.IsNullOrWhiteSpace(txtemployee.Text) Then
            borrowerID = txtemployee.Text.Trim()
            borrowerType = "EmployeeNo"
        Else
            MessageBox.Show("Please enter the LRN or Employee Number first.", "Required Input", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        End If

        RegisteredBrwr.IsTimeInMode = True
        RegisteredBrwr.SetTimeInFilter(borrowerID, borrowerType, borrowerName, borrowertayp)

        RemoveHandler RegisteredBrwr.ListView1.MouseDoubleClick, AddressOf RegisteredBrwr.ListView1_MouseDoubleClick
        AddHandler RegisteredBrwr.ListView1.MouseDoubleClick, AddressOf RegisteredBrwr.ListView1_MouseDoubleClick

        RegisteredBrwr.lbl_action.ForeColor = Color.Red
        RegisteredBrwr.lbl_action.Text = "Selecting"
        RegisteredBrwr.ListView1.Enabled = True

        RegisteredBrwr.ShowDialog()

        If rbstudent.Checked AndAlso Not String.IsNullOrWhiteSpace(txtlrnsu.Text) Then
            txtlrnsu_TextChanged(txtlrnsu, EventArgs.Empty)
        ElseIf rbteacher.Checked AndAlso Not String.IsNullOrWhiteSpace(txtemployee.Text) Then
            txtemployee_TextChanged(txtemployee, EventArgs.Empty)
        End If

    End Sub


    Private Function IsHelperColumn(colName As String) As Boolean
        Return colName = "chk"
    End Function

    Private Function GetRowKey(row As DataGridViewRow) As String
        Dim parts As New List(Of String)
        For Each c As DataGridViewCell In row.Cells
            If Not IsHelperColumn(c.OwningColumn.Name) Then
                parts.Add(Convert.ToString(c.Value))
            End If
        Next
        Return String.Join("|", parts)
    End Function

    Private Function GetRowSnapshot(row As DataGridViewRow) As Dictionary(Of String, Object)
        Dim snap As New Dictionary(Of String, Object)
        For Each c As DataGridViewCell In row.Cells
            If Not IsHelperColumn(c.OwningColumn.Name) Then
                snap(c.OwningColumn.Name) = c.Value
            End If
        Next
        Return snap
    End Function

    Private Sub DataGridView2_CellDoubleClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView2.CellDoubleClick
        If e.RowIndex < 0 Then Exit Sub
        ToggleBookRow(DataGridView2.Rows(e.RowIndex))
    End Sub

    Private Sub ToggleBookRow(row As DataGridViewRow)
        If maxBooks = 0 Then
            MessageBox.Show("Please select Borrower Type first.", "Borrower Type", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        End If

        If Not DataGridView2.Columns.Contains("chk") Then Exit Sub

        Dim key As String = GetRowKey(row)

        If selectedBooks.ContainsKey(key) Then
            selectedBooks.Remove(key)
            row.Cells("chk").Value = False
        Else
            If selectedBooks.Count >= RemainingBooks() Then
                If borrowedToday > 0 Then
                    MessageBox.Show($"You already borrowed {borrowedToday} book(s) under this transaction. You can only select {RemainingBooks()} more (limit: {maxBooks}).",
                                    "Limit Reached", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Else
                    MessageBox.Show($"You can only select up to {maxBooks} book(s).", "Limit Reached", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                End If
                Exit Sub
            End If

            Dim snap As Dictionary(Of String, Object) = GetRowSnapshot(row)
            Dim title As String = Convert.ToString(snap("BookTitle"))

            Dim accId As String = PickRandomAccession(title)

            If accId Is Nothing Then Exit Sub

            If accId = "" Then
                MessageBox.Show($"No available copy for '{title}' right now.", "Not Available", MessageBoxButtons.OK, MessageBoxIcon.Information)
                LoadSelectBooks()
                LoadAvailableBooks()
                Exit Sub
            End If

            snap("AccessionID") = accId
            selectedBooks(key) = snap
            row.Cells("chk").Value = True
        End If
    End Sub

    Private Sub RestoreChecks()
        If Not DataGridView2.Columns.Contains("chk") Then Exit Sub

        For Each row As DataGridViewRow In DataGridView2.Rows
            If row.IsNewRow Then Continue For
            row.Cells("chk").Value = selectedBooks.ContainsKey(GetRowKey(row))
        Next
    End Sub

    Private Sub ClearSelectedBooks()
        selectedBooks.Clear()
        RestoreChecks()
    End Sub

    Private Sub DataGridView2_DataBindingComplete(sender As Object, e As DataGridViewBindingCompleteEventArgs) Handles DataGridView2.DataBindingComplete
        SetupGridColumns()
        RestoreChecks()

        If e.ListChangedType = System.ComponentModel.ListChangedType.Reset Then
            DataGridView2.ClearSelection()
            DataGridView2.CurrentCell = Nothing
        End If
    End Sub

    Protected Overrides Function ProcessCmdKey(ByRef msg As Message, keyData As Keys) As Boolean
        If keyData = Keys.Enter AndAlso DataGridView2.ContainsFocus Then
            ConfirmSelectedBooks()
            Return True
        End If
        Return MyBase.ProcessCmdKey(msg, keyData)
    End Function

    Private Sub ConfirmSelectedBooks()
        If Not (rbstudent.Checked OrElse rbteacher.Checked) Then
            MessageBox.Show("Please select Borrower Type first.", "Borrower Type", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        End If

        Dim borrowerID As String = If(rbstudent.Checked, txtlrnsu.Text.Trim(), txtemployee.Text.Trim())
        Dim idType As String = If(rbstudent.Checked, "LRN", "EmployeeNo")

        If borrowerID = "" OrElse txtname.Text.Trim() = "" Then
            MessageBox.Show("Please enter a valid LRN or Employee Number first.", "Required Input", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        End If

        If lbltransac.Text.Trim() = "" Then
            MessageBox.Show("No Transaction Receipt yet. Please re-enter the LRN or Employee Number.", "Required Input", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        End If

        If Not CheckTimeInStatus(borrowerID, idType) Then
            MessageBox.Show("This borrower has not yet Timed In.", "Time In Required", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        End If

        If RemainingBooks() = 0 Then
            MessageBox.Show($"Limit reached. You already borrowed {borrowedToday} of {maxBooks} book(s).", "Limit Reached", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        End If

        If selectedBooks.Count = 0 Then
            MessageBox.Show("Please double click a book to select it first.", "No Book Selected", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Exit Sub
        End If

        Dim ask As DialogResult = MessageBox.Show($"Confirm borrowing {selectedBooks.Count} book(s)?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
        If ask <> DialogResult.Yes Then Exit Sub

        Dim result As List(Of String) = SaveBorrowedBooks(borrowerID, idType, txtname.Text.Trim(), lbltransac.Text.Trim(), selectedBooks.Values.ToList())

        If result IsNot Nothing Then
            Dim msg As String = "Borrowed successfully:" & vbCrLf & String.Join(vbCrLf, result) & vbCrLf & vbCrLf &
                                "Transaction Receipt: " & lbltransac.Text

            If lastSkippedColumns.Count > 0 Then
                Debug.WriteLine("Skipped columns: " & String.Join(", ", lastSkippedColumns))

                If SHOW_SKIPPED_COLUMNS_NOTE AndAlso Not skippedNoteShown Then
                    msg &= vbCrLf & vbCrLf & "Note: these columns were not found and were skipped:" & vbCrLf &
                           String.Join(", ", lastSkippedColumns)
                    skippedNoteShown = True
                End If
            End If

            MessageBox.Show(msg, "Borrowed", MessageBoxButtons.OK, MessageBoxIcon.Information)

            selectedBooks.Clear()
            LoadSelectBooks()
            LoadAvailableBooks()
            RefreshBorrowedCount()
        End If
    End Sub


    Private Function PickRandomAccessionCore(con As MySqlConnection, tran As MySqlTransaction, title As String) As String
        Dim sql As String =
            $"SELECT `{ACC_NO_COL}` FROM `{ACC_TABLE}` " &
            $"WHERE TRIM(`{ACC_TITLE_COL}`) = TRIM(@t) AND TRIM(`{ACC_STATUS_COL}`) = @s ORDER BY RAND() LIMIT 1"

        Using cmd As New MySqlCommand(sql, con, tran)
            cmd.Parameters.AddWithValue("@t", title)
            cmd.Parameters.AddWithValue("@s", ACC_AVAILABLE)
            Dim r As Object = cmd.ExecuteScalar()
            If r Is Nothing OrElse IsDBNull(r) Then Return ""
            Return r.ToString()
        End Using
    End Function

    Private Function PickRandomAccession(title As String) As String
        Try
            Using con As New MySqlConnection(GlobalVarsModule.connectionString)
                con.Open()
                Return PickRandomAccessionCore(con, Nothing, title)
            End Using
        Catch ex As Exception
            MessageBox.Show("Error picking accession ID: " & ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return Nothing
        End Try
    End Function

    Private Function GetAccessionDetails(con As MySqlConnection, tran As MySqlTransaction, accId As String) As Dictionary(Of String, Object)
        Dim d As New Dictionary(Of String, Object)(StringComparer.OrdinalIgnoreCase)

        Dim sql As String = $"SELECT * FROM `{ACC_TABLE}` WHERE `{ACC_NO_COL}` = @a LIMIT 1"
        Using cmd As New MySqlCommand(sql, con, tran)
            cmd.Parameters.AddWithValue("@a", accId)
            Using rdr As MySqlDataReader = cmd.ExecuteReader()
                If rdr.Read() Then
                    For i As Integer = 0 To rdr.FieldCount - 1
                        Dim colName As String = rdr.GetName(i).ToLowerInvariant()
                        Dim v As Object = rdr.GetValue(i)
                        If IsDBNull(v) Then v = Nothing

                        If colName = ACC_TXN_COL.ToLowerInvariant() Then
                            d("TransactionNo") = v
                        Else
                            Select Case colName
                                Case "isbn"
                                    d("ISBN") = v
                                Case "barcode"
                                    d("Barcode") = v
                                Case "shelf", "bookshelf"
                                    d("Shelf") = v
                            End Select
                        End If
                    Next
                End If
            End Using
        End Using

        Return d
    End Function


    Private Function GetTableColumns(con As MySqlConnection, tran As MySqlTransaction, tableName As String) As HashSet(Of String)
        If columnCache.ContainsKey(tableName) Then Return columnCache(tableName)

        Dim cols As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)

        Using cmd As New MySqlCommand("SELECT COLUMN_NAME FROM information_schema.COLUMNS WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = @t", con, tran)
            cmd.Parameters.AddWithValue("@t", tableName)
            Using rdr As MySqlDataReader = cmd.ExecuteReader()
                While rdr.Read()
                    Dim v As Object = rdr.GetValue(0)
                    If TypeOf v Is Byte() Then
                        cols.Add(System.Text.Encoding.UTF8.GetString(DirectCast(v, Byte())))
                    Else
                        cols.Add(Convert.ToString(v))
                    End If
                End While
            End Using
        End Using

        columnCache(tableName) = cols
        Return cols
    End Function

    Private Function InsertRowFlexible(con As MySqlConnection, tran As MySqlTransaction, tableName As String, values As Dictionary(Of String, Object)) As Integer
        Dim cols As HashSet(Of String) = GetTableColumns(con, tran, tableName)
        If cols.Count = 0 Then
            Throw New Exception($"Table '{tableName}' was not found.")
        End If

        Dim names As New List(Of String)
        Dim paramNames As New List(Of String)

        Using cmd As New MySqlCommand("", con, tran)
            Dim i As Integer = 0
            For Each kv As KeyValuePair(Of String, Object) In values
                If cols.Contains(kv.Key) Then
                    Dim pn As String = "@p" & i.ToString()
                    names.Add($"`{kv.Key}`")
                    paramNames.Add(pn)
                    cmd.Parameters.AddWithValue(pn, If(kv.Value, DBNull.Value))
                    i += 1
                Else
                    lastSkippedColumns.Add($"{tableName}.{kv.Key}")
                End If
            Next

            If names.Count = 0 Then
                Throw New Exception($"No matching columns found in '{tableName}'.")
            End If

            cmd.CommandText = $"INSERT INTO `{tableName}` ({String.Join(", ", names)}) VALUES ({String.Join(", ", paramNames)})"
            Return cmd.ExecuteNonQuery()
        End Using
    End Function

    Private Sub UpsertPrintReceipt(con As MySqlConnection, tran As MySqlTransaction, receiptNo As String,
                                   borrowerTypeText As String, borrowerName As String,
                                   borrowedDate As DateTime, dueDate As DateTime, totalCount As Integer)

        Dim existsCount As Integer = 0
        Using cmdChk As New MySqlCommand($"SELECT COUNT(*) FROM `{PRT_TABLE}` WHERE `{PRT_RECEIPT_COL}` = @r", con, tran)
            cmdChk.Parameters.AddWithValue("@r", receiptNo)
            Dim r As Object = cmdChk.ExecuteScalar()
            If r IsNot Nothing AndAlso Not IsDBNull(r) Then existsCount = Convert.ToInt32(r)
        End Using

        If existsCount > 0 Then
            Using cmdUpd As New MySqlCommand($"UPDATE `{PRT_TABLE}` SET `{PRT_COUNT_COL}` = @c WHERE `{PRT_RECEIPT_COL}` = @r", con, tran)
                cmdUpd.Parameters.AddWithValue("@c", totalCount)
                cmdUpd.Parameters.AddWithValue("@r", receiptNo)
                cmdUpd.ExecuteNonQuery()
            End Using
        Else
            Dim pvals As New Dictionary(Of String, Object)(StringComparer.OrdinalIgnoreCase)
            pvals("Borrower") = borrowerTypeText
            pvals("Name") = borrowerName
            pvals("BorrowedDate") = borrowedDate.ToString(PRT_BORROWED_FORMAT, System.Globalization.CultureInfo.InvariantCulture)
            pvals("DueDate") = dueDate.ToString(PRT_DUE_FORMAT, System.Globalization.CultureInfo.InvariantCulture)
            pvals(PRT_COUNT_COL) = totalCount
            pvals(PRT_RECEIPT_COL) = receiptNo

            InsertRowFlexible(con, tran, PRT_TABLE, pvals)
        End If
    End Sub


    Private Function SaveBorrowedBooks(borrowerID As String, idType As String, borrowerName As String, receiptNo As String, books As List(Of Dictionary(Of String, Object))) As List(Of String)
        Dim done As New List(Of String)

        lastSkippedColumns.Clear()
        columnCache.Clear()

        GlobalVarsModule.LoadDurationSettings()
        Dim days As Integer = If(idType = "LRN", GlobalVarsModule.studentLimit, GlobalVarsModule.teacherLimit)
        If days <= 0 Then days = 1

        Dim borrowedDate As DateTime = Date.Today
        Dim dueDate As DateTime = Date.Today.AddDays(days)

        Dim borrowerTypeText As String = If(idType = "LRN", "Student", "Teacher")

        Using con As New MySqlConnection(GlobalVarsModule.connectionString)
            Dim tran As MySqlTransaction = Nothing
            Try
                con.Open()
                tran = con.BeginTransaction()

                Dim existing As Integer = CountBorrowedForReceipt(con, tran, receiptNo)
                If existing + books.Count > maxBooks Then
                    Throw New Exception($"Borrowing limit exceeded. Limit: {maxBooks} book(s), already borrowed: {existing}, selected now: {books.Count}.")
                End If

                Dim claimSql As String =
                    $"UPDATE `{ACC_TABLE}` SET `{ACC_STATUS_COL}` = @borrowed " &
                    $"WHERE `{ACC_NO_COL}` = @a AND TRIM(`{ACC_STATUS_COL}`) = @avail"

                For Each b As Dictionary(Of String, Object) In books
                    Dim title As String = Convert.ToString(b("BookTitle"))
                    Dim accId As String = Convert.ToString(b("AccessionID"))

                    Dim rows As Integer = ClaimAccession(con, tran, claimSql, accId)

                    If rows = 0 Then
                        accId = PickRandomAccessionCore(con, tran, title)
                        If accId = "" Then
                            Throw New Exception($"No more available copy for '{title}'.")
                        End If
                        rows = ClaimAccession(con, tran, claimSql, accId)
                        If rows = 0 Then
                            Throw New Exception($"Could not reserve a copy of '{title}'. Please try again.")
                        End If
                    End If

                    Dim details As Dictionary(Of String, Object) = GetAccessionDetails(con, tran, accId)

                    Dim vals As New Dictionary(Of String, Object)(StringComparer.OrdinalIgnoreCase)
                    vals("TransactionReceipt") = receiptNo
                    vals(idType) = borrowerID
                    vals("Name") = borrowerName
                    vals("Borrower") = borrowerTypeText
                    vals("BookTitle") = title
                    vals("AccessionID") = accId
                    vals("ISBN") = If(details.ContainsKey("ISBN") AndAlso details("ISBN") IsNot Nothing, details("ISBN"), "")
                    vals("Barcode") = If(details.ContainsKey("Barcode") AndAlso details("Barcode") IsNot Nothing, details("Barcode"), "")
                    If details.ContainsKey("Shelf") AndAlso details("Shelf") IsNot Nothing Then
                        vals("Shelf") = details("Shelf")
                    End If
                    vals("BorrowedDate") = borrowedDate
                    vals("DueDate") = dueDate

                    InsertRowFlexible(con, tran, BRW_TABLE, vals)
                    InsertRowFlexible(con, tran, HIS_TABLE, vals)

                    DecrementAvailableBook(con, tran, title)

                    Dim txnNo As String = ""
                    If details.ContainsKey("TransactionNo") AndAlso details("TransactionNo") IsNot Nothing Then
                        txnNo = Convert.ToString(details("TransactionNo"))
                    End If
                    DecrementAcquisitionQuantity(con, tran, title, txnNo)

                    done.Add($"{title}  -  Accession ID {accId}")
                Next

                Dim totalCount As Integer = CountBorrowedForReceipt(con, tran, receiptNo)
                UpsertPrintReceipt(con, tran, receiptNo, borrowerTypeText, borrowerName, borrowedDate, dueDate, totalCount)

                tran.Commit()
                Return done

            Catch ex As Exception
                Try
                    If tran IsNot Nothing Then tran.Rollback()
                Catch
                End Try
                MessageBox.Show("Error saving borrowed books: " & ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return Nothing
            End Try
        End Using
    End Function

    Private Function ClaimAccession(con As MySqlConnection, tran As MySqlTransaction, claimSql As String, accId As String) As Integer
        Using cmd As New MySqlCommand(claimSql, con, tran)
            cmd.Parameters.AddWithValue("@borrowed", ACC_BORROWED)
            cmd.Parameters.AddWithValue("@a", accId)
            cmd.Parameters.AddWithValue("@avail", ACC_AVAILABLE)
            Return cmd.ExecuteNonQuery()
        End Using
    End Function


    Private Sub RenderBarcode(text As String)
        Try
            Dim writer As New ZXing.BarcodeWriter(Of Bitmap)() With {
                .Format = ZXing.BarcodeFormat.CODE_128,
                .Renderer = New BitmapRenderer()
            }

            writer.Options = New ZXing.Common.EncodingOptions With {
                .Height = picbarcode.Height,
                .Width = picbarcode.Width,
                .PureBarcode = False,
                .Margin = 10
            }

            Dim bmp As Bitmap = writer.Write(text)

            If picbarcode.Image IsNot Nothing Then
                picbarcode.Image.Dispose()
            End If
            picbarcode.Image = bmp
        Catch ex As Exception
            MessageBox.Show("Error generating barcode with ZXing.Net: " & ex.Message, "Barcode Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Function GenerateUniqueTransactionID() As String
        Return DateTime.Now.ToString("yyMMddHHmmss")
    End Function

    Public Sub UpdateTransactionBarcode()
        Dim newID As String = GenerateUniqueTransactionID()
        RenderBarcode(newID)
        lbltransac.Text = newID
    End Sub

    Private Function LookupTodayReceiptFromDb(idType As String, id As String) As String
        Try
            Using con As New MySqlConnection(GlobalVarsModule.connectionString)
                con.Open()
                Dim sql As String =
                    $"SELECT `{HIS_RECEIPT_COL}` FROM `{HIS_TABLE}` " &
                    $"WHERE `{idType}` = @id AND DATE(`{HIS_DATE_COL}`) = CURDATE() " &
                    $"ORDER BY 1 DESC LIMIT 1"

                Using cmd As New MySqlCommand(sql, con)
                    cmd.Parameters.AddWithValue("@id", id)
                    Dim r As Object = cmd.ExecuteScalar()
                    If r Is Nothing OrElse IsDBNull(r) Then Return ""
                    Return r.ToString().Trim()
                End Using
            End Using
        Catch
            Return ""
        End Try
    End Function

    Private Sub ShowReceiptForBorrower(idType As String, id As String)
        Dim borrowerKey As String = idType & ":" & id
        Dim info As ReceiptInfo = Nothing

        If receiptByBorrower.TryGetValue(borrowerKey, info) AndAlso info.ReceiptDate.Date = Date.Today Then
            lbltransac.Text = info.ReceiptNo
            RenderBarcode(info.ReceiptNo)
        Else
            Dim dbReceipt As String = LookupTodayReceiptFromDb(idType, id)

            If dbReceipt <> "" Then
                lbltransac.Text = dbReceipt
                RenderBarcode(dbReceipt)
            Else
                UpdateTransactionBarcode()
            End If

            receiptByBorrower(borrowerKey) = New ReceiptInfo With {
                .ReceiptNo = lbltransac.Text,
                .ReceiptDate = Date.Today
            }
        End If

        RefreshBorrowedCount()
    End Sub

    Private Sub ClearReceipt()
        lbltransac.Text = ""
        If picbarcode.Image IsNot Nothing Then picbarcode.Image.Dispose()
        picbarcode.Image = Nothing
    End Sub


    Private Sub txtemployee_TextChanged(sender As Object, e As EventArgs) Handles txtemployee.TextChanged
        If isLoadingData Then Exit Sub

        If String.IsNullOrWhiteSpace(txtemployee.Text) Then
            txtname.Text = ""
            btntimein.Visible = False
            ClearReceipt()
            ClearSelectedBooks()
            ResetBorrowedCount()
            Exit Sub
        End If

        Dim enteredEmployeeID As String = txtemployee.Text.Trim()
        Dim currentUserID_Cleaned As String = If(GlobalVarsModule.GetCleanCurrentBorrowerID(), "").Trim()
        Dim enteredEmployeeID_Cleaned As String = enteredEmployeeID

        Dim tempID As Long
        If Long.TryParse(currentUserID_Cleaned, tempID) Then
            currentUserID_Cleaned = tempID.ToString()
        End If
        If Long.TryParse(enteredEmployeeID_Cleaned, tempID) Then
            enteredEmployeeID_Cleaned = tempID.ToString()
        End If

        Dim con As New MySqlConnection(GlobalVarsModule.connectionString)
        Dim foundBorrower As Boolean = False
        Dim borrowerName As String = ""

        Try
            con.Open()

            Dim com As String = "SELECT CONCAT_WS(' ', `FirstName`, `LastName`, NULLIF(TRIM(REPLACE(MiddleInitial, '.', '')), 'N/A')) 
                                 FROM `borrower_tbl` WHERE `EmployeeNo` = @emp"
            Using comsi As New MySqlCommand(com, con)
                comsi.Parameters.AddWithValue("@emp", enteredEmployeeID)
                Dim emp As Object = comsi.ExecuteScalar()
                If emp IsNot Nothing AndAlso Not String.IsNullOrWhiteSpace(emp.ToString()) Then
                    borrowerName = System.Text.RegularExpressions.Regex.Replace(emp.ToString().Trim(), "\s+", " ")
                    txtname.Text = borrowerName
                    foundBorrower = True
                Else
                    txtname.Text = ""
                End If
            End Using
        Catch ex As Exception
            MessageBox.Show("An error occurred while retrieving borrower information: " & ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            If con.State = ConnectionState.Open Then con.Close()
        End Try

        If GlobalVarsModule.CurrentUserRole = "Borrower" AndAlso GlobalVarsModule.CurrentBorrowerType = "Teacher" Then
            If foundBorrower AndAlso Not String.Equals(enteredEmployeeID_Cleaned, currentUserID_Cleaned, StringComparison.Ordinal) Then
                txtname.Text = ""
                btntimein.Visible = False
                ClearReceipt()
                ResetBorrowedCount()
                MessageBox.Show($"The Employee No. '{enteredEmployeeID}' belongs to {borrowerName}. You are only allowed to search your own Employee No.", "Security Restriction", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Sub
            End If
        End If

        If foundBorrower Then
            Dim isTimedIn As Boolean = CheckTimeInStatus(enteredEmployeeID, "EmployeeNo")
            If Not isTimedIn Then
                MessageBox.Show($"NOTICE: {borrowerName} has not yet Timed In.", "Time In Required", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                btntimein.Visible = True
            Else
                btntimein.Visible = False
            End If

            ShowReceiptForBorrower("EmployeeNo", enteredEmployeeID)
            lastEnteredBorrowerID = enteredEmployeeID
        Else
            btntimein.Visible = False
            ClearReceipt()
            ResetBorrowedCount()
        End If
    End Sub


    Private Sub txtlrnsu_TextChanged(sender As Object, e As EventArgs) Handles txtlrnsu.TextChanged
        If isLoadingData Then Exit Sub

        If String.IsNullOrWhiteSpace(txtlrnsu.Text) Then
            txtname.Text = ""
            btntimein.Visible = False
            ClearReceipt()
            ClearSelectedBooks()
            ResetBorrowedCount()
            Exit Sub
        End If

        Dim enteredLRN As String = txtlrnsu.Text.Trim()

        Dim currentUserID_Trimmed As String = GlobalVarsModule.CurrentBorrowerID.Trim()
        Dim currentUserID_Cleaned As String = currentUserID_Trimmed
        Dim tempID As Long

        If Long.TryParse(currentUserID_Trimmed, tempID) Then
            currentUserID_Cleaned = tempID.ToString()
        End If

        Dim enteredLRN_Cleaned As String = enteredLRN
        If Long.TryParse(enteredLRN, tempID) Then
            enteredLRN_Cleaned = tempID.ToString()
        End If

        Dim con As New MySqlConnection(GlobalVarsModule.connectionString)
        Dim foundBorrower As Boolean = False
        Dim borrowerName As String = ""

        Try
            con.Open()

            Dim com As String = "SELECT CONCAT_WS(' ', `FirstName`, `LastName`, NULLIF(TRIM(REPLACE(MiddleInitial, '.', '')), 'N/A')) FROM `borrower_tbl` WHERE `LRN` = @lrn"
            Using comsi As New MySqlCommand(com, con)
                comsi.Parameters.AddWithValue("@lrn", enteredLRN)
                Dim lrn As Object = comsi.ExecuteScalar()
                If lrn IsNot Nothing AndAlso Not String.IsNullOrWhiteSpace(lrn.ToString()) Then
                    borrowerName = System.Text.RegularExpressions.Regex.Replace(lrn.ToString().Trim(), "\s+", " ")
                    txtname.Text = borrowerName
                    foundBorrower = True
                Else
                    txtname.Text = ""
                End If
            End Using
        Catch ex As Exception
            MessageBox.Show("An error occurred while retrieving borrower information: " & ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            If con.State = ConnectionState.Open Then con.Close()
        End Try

        If GlobalVarsModule.CurrentUserRole = "Borrower" AndAlso GlobalVarsModule.CurrentBorrowerType = "Student" Then
            If foundBorrower AndAlso Not String.Equals(enteredLRN_Cleaned, currentUserID_Cleaned, StringComparison.Ordinal) Then
                txtname.Text = ""
                btntimein.Visible = False
                ClearReceipt()
                ResetBorrowedCount()
                MessageBox.Show($"The LRN '{enteredLRN}' belongs to {borrowerName}. You are only allowed to search your own LRN.", "Security Restriction", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Sub
            End If
        End If

        If foundBorrower Then
            Dim isTimedIn As Boolean = CheckTimeInStatus(enteredLRN, "LRN")
            If Not isTimedIn Then
                MessageBox.Show($"NOTICE: {borrowerName} has not yet Timed In.", "Time In Required", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                btntimein.Visible = True
            Else
                btntimein.Visible = False
            End If

            ShowReceiptForBorrower("LRN", enteredLRN)
            lastEnteredBorrowerID = enteredLRN
        Else
            btntimein.Visible = False
            ClearReceipt()
            ResetBorrowedCount()
        End If
    End Sub

    Private Function CheckTimeInStatus(identifierValue As String, identifierField As String) As Boolean
        If String.IsNullOrWhiteSpace(identifierValue) Then Return False

        Dim con As New MySqlConnection(GlobalVarsModule.connectionString)
        Dim com As String = $"SELECT COUNT(*) FROM `oras_tbl` WHERE `{identifierField}` = @IdentifierValue AND `TimeOut` IS NULL"
        Dim cmd As New MySqlCommand(com, con)

        cmd.Parameters.AddWithValue("@IdentifierValue", identifierValue)

        Try
            con.Open()
            Dim count As Integer = CInt(cmd.ExecuteScalar())
            Return count > 0
        Catch ex As Exception
            MessageBox.Show("Error checking Time In status: " & ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        Finally
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
        End Try
    End Function


    Private Sub ApplyGridFilter(grid As DataGridView, searchText As String)
        Dim view As DataView = Nothing

        If TypeOf grid.DataSource Is DataTable Then
            view = DirectCast(grid.DataSource, DataTable).DefaultView
        ElseIf TypeOf grid.DataSource Is DataView Then
            view = DirectCast(grid.DataSource, DataView)
        End If

        If view Is Nothing Then Exit Sub

        If searchText = "" Then
            view.RowFilter = ""
            Exit Sub
        End If

        Dim safe As String = System.Text.RegularExpressions.Regex.Replace(searchText, "[%*\[\]]", "[$0]")
        safe = safe.Replace("'", "''")

        Dim conditions As New List(Of String)
        For Each col As DataColumn In view.Table.Columns
            If col.DataType IsNot GetType(Byte()) Then
                conditions.Add($"Convert([{col.ColumnName}], 'System.String') LIKE '%{safe}%'")
            End If
        Next

        Try
            view.RowFilter = String.Join(" OR ", conditions)
        Catch ex As Exception
            view.RowFilter = ""
        End Try
    End Sub

    Private Sub txtsearch_TextChanged(sender As Object, e As EventArgs) Handles txtsearch.TextChanged
        GlobalVarsModule.HandleAutoRefreshPause(DataGridView2, txtsearch)

        ApplyGridFilter(DataGridView2, txtsearch.Text.Trim())
        RestoreChecks()
    End Sub

    Private Sub txtsearch1_TextChanged(sender As Object, e As EventArgs) Handles txtsearch1.TextChanged
        GlobalVarsModule.HandleAutoRefreshPause(DataGridView1, txtsearch1)

        ApplyGridFilter(DataGridView1, txtsearch1.Text.Trim())
    End Sub


    Private Sub view_link_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles view_link.LinkClicked
        If Not IsBorrowerRole() Then Exit Sub

        Try

            Dim borrowerType As String = GlobalVarsModule.CurrentBorrowerType
            If String.IsNullOrWhiteSpace(borrowerType) Then
                borrowerType = If(rbteacher.Checked, "Teacher", "Student")
            End If

            Dim lrn As String = String.Empty
            Dim empNo As String = String.Empty
            Dim borrowerName As String = String.Empty

            If GlobalVarsModule.CurrentUserRole = "Borrower" AndAlso Not String.IsNullOrWhiteSpace(GlobalVarsModule.CurrentBorrowerID) Then

                If String.Equals(GlobalVarsModule.CurrentBorrowerType, "Student", StringComparison.OrdinalIgnoreCase) Then
                    lrn = GlobalVarsModule.CurrentBorrowerID
                ElseIf String.Equals(GlobalVarsModule.CurrentBorrowerType, "Teacher", StringComparison.OrdinalIgnoreCase) Then
                    empNo = GlobalVarsModule.CurrentBorrowerID
                End If
            Else

                lrn = txtlrnsu.Text.Trim()
                empNo = txtemployee.Text.Trim()
                borrowerName = txtname.Text.Trim()
            End If


            Try
                If String.IsNullOrWhiteSpace(borrowerName) Then
                    Using con As New MySqlConnection(GlobalVarsModule.connectionString)
                        con.Open()
                        If Not String.IsNullOrWhiteSpace(empNo) Then
                            Using cmd As New MySqlCommand("SELECT CONCAT_WS(' ', FirstName, LastName, NULLIF(TRIM(REPLACE(MiddleInitial, '.', '')), 'N/A')) FROM borrower_tbl WHERE EmployeeNo = @id LIMIT 1", con)
                                cmd.Parameters.AddWithValue("@id", empNo)
                                Dim obj = cmd.ExecuteScalar()
                                If obj IsNot Nothing AndAlso Not IsDBNull(obj) Then borrowerName = obj.ToString()
                            End Using
                        ElseIf Not String.IsNullOrWhiteSpace(lrn) Then
                            Using cmd As New MySqlCommand("SELECT CONCAT_WS(' ', FirstName, LastName, NULLIF(TRIM(REPLACE(MiddleInitial, '.', '')), 'N/A')) FROM borrower_tbl WHERE LRN = @id LIMIT 1", con)
                                cmd.Parameters.AddWithValue("@id", lrn)
                                Dim obj = cmd.ExecuteScalar()
                                If obj IsNot Nothing AndAlso Not IsDBNull(obj) Then borrowerName = obj.ToString()
                            End Using
                        End If
                    End Using
                End If
            Catch
            End Try

            Dim popup As New popuphistory()
            popup.ShowForBorrower(borrowerName, borrowerType, lrn, empNo)
        Catch ex As Exception
            MessageBox.Show("Error opening history: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

End Class