Imports MySql.Data.MySqlClient
Imports System.Data
Imports System.Linq

Public Class AvailableBooks

    Private selectedAccessions As New List(Of String)()
    Private Const STUDENT_SELECT_LIMIT As Integer = 3
    Private Const TEACHER_SELECT_LIMIT As Integer = 5
    Private desiredSelectionCount As Integer = 1

    Private Sub AvailableBooks_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        refreshavail()
        counts()
        GlobalVarsModule.AutoRefreshGrid(DataGridView1, "SELECT t1.ISBN, t1.Barcode, t1.AccessionID, t1.BookTitle, t1.Shelf, t1.Status FROM `available_tbl` t1 JOIN `acession_tbl` t2 ON t1.AccessionID = t2.AccessionID WHERE t1.Status = 'Available' ORDER BY t1.BookTitle ASC, CAST(t1.AccessionID AS UNSIGNED)", 2000)
        AddHandler GlobalVarsModule.DatabaseUpdated, AddressOf OnDatabaseUpdated

        Try
            Dim numCtrl = Me.Controls.Find("numupdown", True).FirstOrDefault()
            If numCtrl IsNot Nothing Then
                If TypeOf numCtrl Is NumericUpDown Then
                    AddHandler DirectCast(numCtrl, NumericUpDown).ValueChanged, AddressOf numupdown_ValueChanged
                ElseIf TypeOf numCtrl Is Guna.UI2.WinForms.Guna2NumericUpDown Then
                    AddHandler DirectCast(numCtrl, Guna.UI2.WinForms.Guna2NumericUpDown).ValueChanged, AddressOf numupdown_ValueChanged
                End If
            End If
        Catch
        End Try

        Try
            numupdown_ValueChanged(Nothing, EventArgs.Empty)
        Catch
        End Try
    End Sub

    Public Sub refreshavail()
        Dim con As New MySqlConnection(GlobalVarsModule.connectionString)
        Try
            con.Open()
            Dim syncDeleteSql As String = "DELETE av.* FROM available_tbl av " &
                                  "LEFT JOIN acession_tbl ac ON av.AccessionID = ac.AccessionID " &
                                  "WHERE ac.Status <> 'Available' OR ac.Status IS NULL"
            Using syncDeleteCmd As New MySqlCommand(syncDeleteSql, con)
                syncDeleteCmd.ExecuteNonQuery()
            End Using

            Dim syncInsertSql As String = "INSERT IGNORE INTO available_tbl (AccessionID, ISBN, Barcode, BookTitle, Shelf, Status) " &
                                  "SELECT ac.AccessionID, ac.ISBN, ac.Barcode, ac.BookTitle, ac.Shelf, ac.Status " &
                                  "FROM acession_tbl ac " &
                                  "LEFT JOIN available_tbl av ON ac.AccessionID = av.AccessionID " &
                                  "WHERE ac.Status = 'Available' AND av.AccessionID IS NULL"
            Using syncInsertCmd As New MySqlCommand(syncInsertSql, con)
                syncInsertCmd.ExecuteNonQuery()
            End Using


            Dim syncUpdateSql As String = "UPDATE available_tbl av " &
                                      "JOIN acession_tbl ac ON av.AccessionID = ac.AccessionID " &
                                      "SET av.BookTitle = ac.BookTitle, " &
                                      "av.ISBN = ac.ISBN, " &
                                      "av.Barcode = ac.Barcode, " &
                                      "av.Shelf = ac.Shelf " &
                                      "WHERE av.BookTitle <> ac.BookTitle " &
                                      "OR av.ISBN <> ac.ISBN " &
                                      "OR av.Barcode <> ac.Barcode " &
                                      "OR av.Shelf <> ac.Shelf"
            Using syncUpdateCmd As New MySqlCommand(syncUpdateSql, con)
                syncUpdateCmd.ExecuteNonQuery()
            End Using


            Dim com As String = "SELECT t1.ISBN, t1.Barcode, t1.AccessionID, t1.BookTitle, t1.Shelf, t1.Status " &
                        "FROM `available_tbl` t1 " &
                        "JOIN `acession_tbl` t2 ON t1.AccessionID = t2.AccessionID " &
                        "WHERE t1.Status = 'Available' " &
                        "ORDER BY t1.BookTitle ASC, CAST(t1.AccessionID AS UNSIGNED)"
            Dim adap As New MySqlDataAdapter(com, con)
            Dim ds As New DataSet
            adap.SelectCommand.Connection = con
            adap.Fill(ds, "avail_info")
            DataGridView1.DataSource = ds.Tables("avail_info")
        Catch ex As Exception
            MessageBox.Show("Error refreshing Available Books data: " & ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
        End Try

        If DataGridView1.Columns.Contains("ID") Then
            DataGridView1.Columns("ID").Visible = False
        End If

        DataGridView1.EnableHeadersVisualStyles = False
        DataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(207, 58, 109)
        DataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White
        DataGridView1.ClearSelection()
        DataGridView1.AllowUserToAddRows = False
    End Sub

    Private Sub numupdown_ValueChanged(sender As Object, e As EventArgs)
        Try

            Dim requestedQty As Integer = 1
            Dim numCtrl = Me.Controls.Find("numupdown", True).FirstOrDefault()
            If numCtrl IsNot Nothing Then
                If TypeOf numCtrl Is NumericUpDown Then
                    requestedQty = CInt(DirectCast(numCtrl, NumericUpDown).Value)
                ElseIf numCtrl.GetType().FullName.Contains("Guna.UI2.WinForms.Guna2NumericUpDown") Then
                    requestedQty = CInt(Convert.ToDecimal(numCtrl.GetType().GetProperty("Value").GetValue(numCtrl)))
                End If
            End If

            If requestedQty <= 0 Then
                desiredSelectionCount = 0
                Return
            End If


            Dim borrowerLimit As Integer = Integer.MaxValue
            Dim brType As String = If(String.IsNullOrWhiteSpace(GlobalVarsModule.CurrentBorrowerType), String.Empty, GlobalVarsModule.CurrentBorrowerType)
            Try
                If String.IsNullOrWhiteSpace(brType) Then
                    Dim activeMain As MainForm = GlobalVarsModule.ActiveMainForm
                    If activeMain Is Nothing Then activeMain = Application.OpenForms.OfType(Of MainForm)().FirstOrDefault()
                    If activeMain IsNot Nothing Then
                        For Each ctrl As Control In activeMain.Panel_dash.Controls
                            If TypeOf ctrl Is Borrowing Then
                                Dim bf = DirectCast(ctrl, Borrowing)
                                If bf.rbstudent.Checked Then
                                    brType = "Student"
                                ElseIf bf.rbteacher.Checked Then
                                    brType = "Teacher"
                                End If
                                Exit For
                            End If
                        Next
                    End If
                End If
            Catch
            End Try

            If String.Equals(brType, "Student", StringComparison.OrdinalIgnoreCase) Then borrowerLimit = STUDENT_SELECT_LIMIT
            If String.Equals(brType, "Teacher", StringComparison.OrdinalIgnoreCase) Then borrowerLimit = TEACHER_SELECT_LIMIT

            desiredSelectionCount = Math.Min(requestedQty, If(borrowerLimit = Integer.MaxValue, requestedQty, borrowerLimit))

            Try
                Dim displayText As String = String.Empty
                If desiredSelectionCount <= 0 Then
                    displayText = "No selection. Set the quantity to a value greater than 0."
                Else
                    If borrowerLimit <> Integer.MaxValue Then
                        displayText = $"You can now select {desiredSelectionCount} book(s). (Limit: {borrowerLimit})"
                    Else
                        displayText = $"You can now select {desiredSelectionCount} book(s)."
                    End If
                End If

                If Me.Label13 IsNot Nothing Then
                    Label13.Text = displayText
                End If
            Catch
            End Try
        Catch
        End Try
    End Sub


    Private Async Sub OnDatabaseUpdated()
        Try
            Dim query As String = "SELECT t1.ISBN, t1.Barcode, t1.AccessionID, t1.BookTitle, t1.Shelf, t1.Status " &
                              "FROM `available_tbl` t1 " &
                              "JOIN `acession_tbl` t2 ON t1.AccessionID = t2.AccessionID " &
                              "WHERE t1.Status = 'Available' " &
                              "ORDER BY t1.BookTitle ASC, CAST(t1.AccessionID AS UNSIGNED)"
            Await GlobalVarsModule.LoadToGridAsync(DataGridView1, query)
            DataGridView1.ClearSelection()
        Catch
        End Try
    End Sub

    Public Sub counts()

        Dim con As New MySqlConnection(GlobalVarsModule.connectionString)

        Try
            con.Open()

            Dim countss As String = "SELECT COUNT(*) FROM available_tbl WHERE Status = 'Available'"
            Using comms As New MySqlCommand(countss, con)
                Dim count As Integer = CInt(comms.ExecuteScalar())
                lblavailable.Text = count.ToString()
            End Using

        Catch ex As Exception
            MessageBox.Show("Error updating available count: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
        End Try

    End Sub

    Private Sub AvailableBooks_Shown(sender As Object, e As EventArgs) Handles MyBase.Shown
        DataGridView1.ClearSelection()
    End Sub

    Private Sub AvailableBooks_KeyDown(sender As Object, e As KeyEventArgs) Handles MyBase.KeyDown

        If e.KeyCode = Keys.Escape Then
            Me.Close()
            Accession.btnview.Visible = False

            Accession.btnview.Visible = False
            Accession.CheckBox1.Checked = False

        End If

    End Sub

    Private Sub DataGridView1_DataBindingComplete(sender As Object, e As DataGridViewBindingCompleteEventArgs) Handles DataGridView1.DataBindingComplete

        Try

            Dim presentIds As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)
            Dim dt As DataTable = TryCast(DataGridView1.DataSource, DataTable)
            If dt Is Nothing AndAlso TypeOf DataGridView1.DataSource Is BindingSource Then
                Dim bs = DirectCast(DataGridView1.DataSource, BindingSource)
                dt = TryCast(bs.DataSource, DataTable)
            End If

            If dt IsNot Nothing Then
                For Each dr As DataRow In dt.Rows
                    Try
                        If dt.Columns.Contains("AccessionID") AndAlso dr("AccessionID") IsNot DBNull.Value Then
                            presentIds.Add(dr("AccessionID").ToString())
                        End If
                    Catch
                    End Try
                Next
            Else

                For Each r As DataGridViewRow In DataGridView1.Rows
                    Try
                        If DataGridView1.Columns.Contains("AccessionID") AndAlso r.Cells("AccessionID").Value IsNot Nothing Then
                            presentIds.Add(r.Cells("AccessionID").Value.ToString())
                        End If
                    Catch
                    End Try
                Next
            End If

            Dim newSelection As New List(Of String)()
            For Each id In selectedAccessions
                If presentIds.Contains(id) Then newSelection.Add(id)
            Next
            selectedAccessions = newSelection


            For Each r As DataGridViewRow In DataGridView1.Rows
                Try
                    r.DefaultCellStyle.BackColor = Color.White
                    r.DefaultCellStyle.ForeColor = Color.Black
                    If DataGridView1.Columns.Contains("AccessionID") AndAlso r.Cells("AccessionID").Value IsNot Nothing Then
                        Dim aid = r.Cells("AccessionID").Value.ToString()
                        If selectedAccessions.Contains(aid) Then
                            r.DefaultCellStyle.BackColor = Color.LightGreen
                            r.DefaultCellStyle.ForeColor = Color.Black
                        End If
                    End If
                Catch
                End Try
            Next
        Catch
        End Try
    End Sub

    Private Sub DataGridView1_CellDoubleClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellDoubleClick

        If e.RowIndex >= 0 AndAlso Not DataGridView1.Rows(e.RowIndex).IsNewRow Then
            Try

                Dim activeBorrowing As Borrowing = Nothing


                Dim activeMain As MainForm = GlobalVarsModule.ActiveMainForm
                If activeMain Is Nothing Then
                    activeMain = Application.OpenForms.OfType(Of MainForm)().FirstOrDefault()
                    If activeMain IsNot Nothing Then
                        GlobalVarsModule.ActiveMainForm = activeMain
                    End If
                End If

                If activeMain IsNot Nothing Then
                    For Each ctrl As Control In activeMain.Panel_dash.Controls
                        If TypeOf ctrl Is Borrowing Then
                            activeBorrowing = DirectCast(ctrl, Borrowing)
                            Exit For
                        End If
                    Next


                    If activeBorrowing Is Nothing Then
                        activeBorrowing = New Borrowing()
                        With activeBorrowing
                            .TopLevel = False
                            .Dock = DockStyle.Fill
                            activeMain.Panel_dash.Controls.Add(activeBorrowing)
                            .BringToFront()
                            .Show()
                        End With
                    End If
                End If


                If activeBorrowing Is Nothing OrElse activeBorrowing.IsDisposed Then
                    activeBorrowing = New Borrowing()
                    If activeMain IsNot Nothing Then
                        activeMain.Panel_dash.Controls.Add(activeBorrowing)
                        activeBorrowing.TopLevel = False
                        activeBorrowing.BringToFront()
                        activeBorrowing.Show()
                    End If
                End If


                Dim row As DataGridViewRow = DataGridView1.Rows(e.RowIndex)
                Dim accessionID As String = row.Cells("AccessionID").Value.ToString()
                Dim bookTitle As String = row.Cells("BookTitle").Value.ToString()

                activeBorrowing.txtaccessionid.Text = accessionID
                activeBorrowing.txtsus.Text = bookTitle.Trim()


                'activeBorrowing.SetupBorrowerFields()

                If activeBorrowing.rbteacher.Checked Then

                    activeBorrowing.txtemployee.Enabled = True
                    activeBorrowing.txtlrn.Enabled = False
                    activeBorrowing.txtlrn.Text = ""

                ElseIf activeBorrowing.rbstudent.Checked Then

                    activeBorrowing.txtlrn.Enabled = True
                    activeBorrowing.txtemployee.Enabled = False
                    activeBorrowing.txtemployee.Text = ""

                End If


                DataGridView1.ClearSelection()
                Me.Dispose()


                Accession.btnview.Visible = False
                Accession.CheckBox1.Checked = False

            Catch ex As Exception
                MessageBox.Show("Error selecting book: " & ex.Message, "Selection Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End If
    End Sub

    Private Sub DataGridView1_CellClick_SelectToggle(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellClick
        If e.RowIndex < 0 Then Return

        Dim row As DataGridViewRow = DataGridView1.Rows(e.RowIndex)

        Dim accIDObj = Nothing
        Dim statusObj = Nothing
        Try
            If DataGridView1.Columns.Contains("AccessionID") Then accIDObj = row.Cells("AccessionID").Value
            If DataGridView1.Columns.Contains("Status") Then statusObj = row.Cells("Status").Value
        Catch
        End Try
        If accIDObj Is Nothing OrElse accIDObj Is DBNull.Value Then Return

        Dim accID As String = accIDObj.ToString()
        Dim status As String = If(statusObj Is Nothing OrElse statusObj Is DBNull.Value, String.Empty, statusObj.ToString())

        If Not status.Equals("Available", StringComparison.OrdinalIgnoreCase) Then
            MessageBox.Show("Only Available books can be selected.", "Selection Restricted", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Dim requestedQty As Integer = 1
        Try
            Dim numCtrl = Me.Controls.Find("numupdown", True).FirstOrDefault()
            If numCtrl IsNot Nothing Then
                If TypeOf numCtrl Is NumericUpDown Then
                    requestedQty = CInt(DirectCast(numCtrl, NumericUpDown).Value)
                ElseIf numCtrl.GetType().FullName.Contains("Guna.UI2.WinForms.Guna2NumericUpDown") Then
                    requestedQty = CInt(Convert.ToDecimal(numCtrl.GetType().GetProperty("Value").GetValue(numCtrl)))
                End If
            End If
        Catch
        End Try

        Dim borrowerLimit As Integer = Integer.MaxValue
        Dim brType As String = If(String.IsNullOrWhiteSpace(GlobalVarsModule.CurrentBorrowerType), String.Empty, GlobalVarsModule.CurrentBorrowerType)
        Try
            If String.IsNullOrWhiteSpace(brType) Then
                Dim activeMain As MainForm = GlobalVarsModule.ActiveMainForm
                If activeMain Is Nothing Then activeMain = Application.OpenForms.OfType(Of MainForm)().FirstOrDefault()
                If activeMain IsNot Nothing Then

                    For Each ctrl As Control In activeMain.Panel_dash.Controls
                        If TypeOf ctrl Is Borrowing Then
                            Dim bf = DirectCast(ctrl, Borrowing)
                            If bf.rbstudent.Checked Then
                                brType = "Student"
                            ElseIf bf.rbteacher.Checked Then
                                brType = "Teacher"
                            End If
                            Exit For
                        End If
                    Next


                    If String.IsNullOrWhiteSpace(brType) AndAlso activeMain.Controls.Contains(activeMain.lbl_currentuser) Then
                        Try
                            brType = activeMain.lbl_currentuser.Text
                        Catch
                        End Try
                    End If
                End If
            End If
        Catch
        End Try

        If String.Equals(brType, "Student", StringComparison.OrdinalIgnoreCase) Then borrowerLimit = STUDENT_SELECT_LIMIT
        If String.Equals(brType, "Teacher", StringComparison.OrdinalIgnoreCase) Then borrowerLimit = TEACHER_SELECT_LIMIT

        If borrowerLimit <> Integer.MaxValue Then
            Try
                Dim numCtrlCap = Me.Controls.Find("numupdown", True).FirstOrDefault()
                If numCtrlCap IsNot Nothing Then
                    If TypeOf numCtrlCap Is NumericUpDown Then
                        Dim nud = DirectCast(numCtrlCap, NumericUpDown)
                        nud.Maximum = borrowerLimit
                        If nud.Value > borrowerLimit Then nud.Value = borrowerLimit
                    Else

                        Dim t = numCtrlCap.GetType()
                        Dim maxProp = t.GetProperty("Maximum")
                        Dim valProp = t.GetProperty("Value")
                        If maxProp IsNot Nothing Then
                            maxProp.SetValue(numCtrlCap, Convert.ChangeType(borrowerLimit, maxProp.PropertyType), Nothing)
                        End If
                        If valProp IsNot Nothing Then
                            Dim curVal = Convert.ToDecimal(valProp.GetValue(numCtrlCap))
                            If curVal > borrowerLimit Then
                                valProp.SetValue(numCtrlCap, Convert.ChangeType(borrowerLimit, valProp.PropertyType), Nothing)
                            End If
                        End If
                    End If
                End If
            Catch
            End Try
        End If

        Dim effectiveLimit As Integer = If(borrowerLimit = Integer.MaxValue, requestedQty, Math.Min(requestedQty, borrowerLimit))

        If selectedAccessions.Contains(accID) Then
            selectedAccessions.Remove(accID)
            Try
                row.DefaultCellStyle.BackColor = Color.White
                row.DefaultCellStyle.ForeColor = Color.Black
            Catch
            End Try
            Return
        End If


        Dim currentTitle As String = ""
        Try
            If DataGridView1.Columns.Contains("BookTitle") AndAlso row.Cells("BookTitle").Value IsNot Nothing Then
                currentTitle = row.Cells("BookTitle").Value.ToString().Trim()
            End If
        Catch
        End Try

        If Not String.IsNullOrEmpty(currentTitle) Then
            For Each selId In selectedAccessions

                For Each rr As DataGridViewRow In DataGridView1.Rows
                    Try
                        If DataGridView1.Columns.Contains("AccessionID") AndAlso rr.Cells("AccessionID").Value IsNot Nothing AndAlso rr.Cells("AccessionID").Value.ToString() = selId Then
                            If DataGridView1.Columns.Contains("BookTitle") AndAlso rr.Cells("BookTitle").Value IsNot Nothing Then
                                If String.Equals(rr.Cells("BookTitle").Value.ToString().Trim(), currentTitle, StringComparison.OrdinalIgnoreCase) Then
                                    MessageBox.Show("You have already selected this book title.", "Duplicate Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                                    Return
                                End If
                            End If
                            Exit For
                        End If
                    Catch
                    End Try
                Next
            Next
        End If


        If selectedAccessions.Count >= effectiveLimit Then
            MessageBox.Show($"Selection limit reached. You can select up to {effectiveLimit} book(s).", "Limit Reached", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        selectedAccessions.Add(accID)
        Try
            row.DefaultCellStyle.BackColor = Color.LightGreen
            row.DefaultCellStyle.ForeColor = Color.Black
        Catch
        End Try

    End Sub

    Private Sub DataGridView1_KeyDown_SelectConfirm(sender As Object, e As KeyEventArgs) Handles DataGridView1.KeyDown
        If e.KeyCode = Keys.Enter Then
            e.SuppressKeyPress = True

            If selectedAccessions.Count = 0 Then
                MessageBox.Show("Please select at least one book before confirming.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Return
            End If

            Dim borrowCount As Integer = selectedAccessions.Count


            Dim selTitles As New List(Of String)()
            For Each accId In selectedAccessions
                For Each r As DataGridViewRow In DataGridView1.Rows
                    Try
                        If DataGridView1.Columns.Contains("AccessionID") AndAlso r.Cells("AccessionID").Value IsNot Nothing AndAlso r.Cells("AccessionID").Value.ToString() = accId Then
                            If DataGridView1.Columns.Contains("BookTitle") AndAlso r.Cells("BookTitle").Value IsNot Nothing Then
                                Dim bt = r.Cells("BookTitle").Value.ToString().Trim()
                                If bt <> String.Empty Then selTitles.Add(bt)
                            End If
                            Exit For
                        End If
                    Catch
                    End Try
                Next
            Next

            Dim uniqueTitles = selTitles.Distinct().ToList()
            Dim sb As New System.Text.StringBuilder()
            If uniqueTitles.Count = 0 Then

                sb.AppendLine($"Selected books ({selectedAccessions.Count}):")
                sb.AppendLine()
                For Each accId In selectedAccessions
                    sb.AppendLine(accId)
                Next
            Else
                sb.AppendLine($"Selected books ({uniqueTitles.Count}):")
                sb.AppendLine()
                For Each t In uniqueTitles
                    sb.AppendLine(t)
                Next
            End If

            Dim dialogResult As DialogResult = MessageBox.Show(sb.ToString(), "Confirm Selection", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
            If dialogResult = DialogResult.Yes Then

                Try
                    Dim activeBorrowing As Borrowing = Nothing

                    Dim activeMain As MainForm = GlobalVarsModule.ActiveMainForm
                    If activeMain Is Nothing Then
                        activeMain = Application.OpenForms.OfType(Of MainForm)().FirstOrDefault()
                        If activeMain IsNot Nothing Then
                            GlobalVarsModule.ActiveMainForm = activeMain
                        End If
                    End If

                    If activeMain IsNot Nothing Then
                        For Each ctrl As Control In activeMain.Panel_dash.Controls
                            If TypeOf ctrl Is Borrowing Then
                                activeBorrowing = DirectCast(ctrl, Borrowing)
                                Exit For
                            End If
                        Next

                        If activeBorrowing Is Nothing Then
                            activeBorrowing = New Borrowing()
                            With activeBorrowing
                                .TopLevel = False
                                .Dock = DockStyle.Fill
                                activeMain.Panel_dash.Controls.Add(activeBorrowing)
                                .BringToFront()
                                .Show()
                            End With
                        End If
                    End If

                    If activeBorrowing IsNot Nothing Then
                        activeBorrowing.txtaccessionid.Text = String.Join(",", selectedAccessions)

                        Dim titles As New List(Of String)()
                        Dim isbns As New List(Of String)()
                        Dim barcodes As New List(Of String)()
                        Dim shelves As New List(Of String)()

                        For Each accId In selectedAccessions
                            For Each r As DataGridViewRow In DataGridView1.Rows
                                Try
                                    If DataGridView1.Columns.Contains("AccessionID") AndAlso r.Cells("AccessionID").Value IsNot Nothing AndAlso r.Cells("AccessionID").Value.ToString() = accId Then
                                        If DataGridView1.Columns.Contains("BookTitle") Then
                                            Dim bt = If(r.Cells("BookTitle").Value IsNot DBNull.Value, r.Cells("BookTitle").Value.ToString().Trim(), "")
                                            If bt <> "" Then titles.Add(bt)
                                        End If
                                        If DataGridView1.Columns.Contains("ISBN") Then
                                            Dim isv = If(r.Cells("ISBN").Value IsNot DBNull.Value, r.Cells("ISBN").Value.ToString().Trim(), "")
                                            If isv <> "" Then isbns.Add(isv)
                                        End If
                                        If DataGridView1.Columns.Contains("Barcode") Then
                                            Dim bv = If(r.Cells("Barcode").Value IsNot DBNull.Value, r.Cells("Barcode").Value.ToString().Trim(), "")
                                            If bv <> "" Then barcodes.Add(bv)
                                        End If
                                        If DataGridView1.Columns.Contains("Shelf") Then
                                            Dim sh = If(r.Cells("Shelf").Value IsNot DBNull.Value, r.Cells("Shelf").Value.ToString().Trim(), "")
                                            If sh <> "" Then shelves.Add(sh)
                                        End If
                                        Exit For
                                    End If
                                Catch
                                End Try
                            Next
                        Next


                        activeBorrowing.txtsus.Text = String.Join(",", titles.Distinct())
                        activeBorrowing.txtisbn.Text = String.Join(",", isbns.Distinct())
                        activeBorrowing.txtbarcode.Text = String.Join(",", barcodes.Distinct())
                        activeBorrowing.txtshelf.Text = String.Join(",", shelves.Distinct())

                        'activeBorrowing.SetupBorrowerFields()
                    End If
                Catch ex As Exception
                End Try


                Me.Dispose()
            Else

            End If
        End If
    End Sub

    Private Sub txtsearch_TextChanged(sender As Object, e As EventArgs) Handles txtsearch.TextChanged

        HandleAutoRefreshPause(DataGridView1, txtsearch)

        Dim dt As DataTable = DirectCast(DataGridView1.DataSource, DataTable)
        If dt IsNot Nothing Then
            If txtsearch.Text.Trim() <> "" Then

                Dim filter As String = String.Format("BookTitle LIKE '%{0}%'", txtsearch.Text.Trim())

                dt.DefaultView.RowFilter = filter
            Else
                dt.DefaultView.RowFilter = ""
            End If
        End If

    End Sub

    Private Sub DataGridView1_MouseHover(sender As Object, e As EventArgs) Handles DataGridView1.MouseHover
        PauseAutoRefresh(DataGridView1)
    End Sub

    Private Sub datagridview1_mouseleave(sender As Object, e As EventArgs) Handles DataGridView1.MouseLeave
        ResumeAutoRefresh(DataGridView1)
    End Sub

End Class