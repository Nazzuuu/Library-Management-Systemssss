Imports System.Data
Imports System.Drawing
Imports MySql.Data.MySqlClient
Imports System.Windows.Forms

Public Class popuphistory
    Private originalPositionsRecorded As Boolean = False
    Private origLrnLabelPos As Point
    Private origLrnValuePos As Point
    Private origEmpLabelPos As Point
    Private origEmpValuePos As Point

    Private Sub Guna2Panel1_Paint(sender As Object, e As PaintEventArgs) Handles Guna2Panel1.Paint

    End Sub

    Private Sub popuphistory_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        If Not originalPositionsRecorded Then
            Try
                origLrnLabelPos = lrnlabel.Location
                origLrnValuePos = lbllrnn.Location
                origEmpLabelPos = lblempy.Location
                origEmpValuePos = lblempp.Location
            Catch
            End Try
            originalPositionsRecorded = True
        End If
    End Sub

    Private Sub DataGridView1_CellContentClick(sender As Object, e As DataGridViewCellEventArgs)

    End Sub

    Private Sub Label4_Click(sender As Object, e As EventArgs) Handles lblname.Click

    End Sub

    Private Sub Label8_Click(sender As Object, e As EventArgs) Handles Label8.Click

    End Sub


    Public Sub ShowForBorrower(borrowerName As String, borrowerType As String, Optional lrn As String = "", Optional employeeNo As String = "")

        lbltyp.Text = If(String.IsNullOrWhiteSpace(borrowerType), "...", borrowerType)
        AdjustLabelPositions(borrowerType)

        LoadHistory(borrowerName, lrn, employeeNo)

        Dim loggedInName As String = String.Empty
        Try
            If GlobalVarsModule.CurrentUserRole = "Borrower" Then
                If Not String.IsNullOrWhiteSpace(GlobalVarsModule.GlobalFullname) Then
                    loggedInName = GlobalVarsModule.GlobalFullname
                ElseIf Not String.IsNullOrWhiteSpace(GlobalVarsModule.CurrentBorrowerID) Then
                    If String.Equals(GlobalVarsModule.CurrentBorrowerType, "Student", StringComparison.OrdinalIgnoreCase) Then
                        loggedInName = GetBorrowerFullNameByIdentifier("LRN", GlobalVarsModule.CurrentBorrowerID)
                    ElseIf String.Equals(GlobalVarsModule.CurrentBorrowerType, "Teacher", StringComparison.OrdinalIgnoreCase) Then
                        loggedInName = GetBorrowerFullNameByIdentifier("EmployeeNo", GlobalVarsModule.CurrentBorrowerID)
                    End If
                End If
            End If
        Catch
        End Try


        If DataGridView1.Rows IsNot Nothing AndAlso DataGridView1.Rows.Count > 0 Then
            Try
                Dim firstRow As DataGridViewRow = DataGridView1.Rows(0)
                Dim gridName As String = If(firstRow.Cells("Borrower")?.Value IsNot Nothing, firstRow.Cells("Borrower").Value.ToString(), String.Empty)
                Dim gridLrn As String = If(firstRow.Cells("LRN")?.Value IsNot Nothing, firstRow.Cells("LRN").Value.ToString(), String.Empty)
                Dim gridEmp As String = If(firstRow.Cells("EmployeeNo")?.Value IsNot Nothing, firstRow.Cells("EmployeeNo").Value.ToString(), String.Empty)

                Dim fallbackName As String = If(Not String.IsNullOrWhiteSpace(loggedInName), loggedInName, borrowerName)
                lblname.Text = If(String.IsNullOrWhiteSpace(gridName), If(String.IsNullOrWhiteSpace(fallbackName), "...", fallbackName), gridName)
                lbllrnn.Text = If(String.IsNullOrWhiteSpace(gridLrn), If(String.IsNullOrWhiteSpace(lrn), "...", lrn), gridLrn)
                lblempp.Text = If(String.IsNullOrWhiteSpace(gridEmp), If(String.IsNullOrWhiteSpace(employeeNo), "...", employeeNo), gridEmp)
            Catch
                Dim fallbackName As String = If(Not String.IsNullOrWhiteSpace(loggedInName), loggedInName, borrowerName)
                lblname.Text = If(String.IsNullOrWhiteSpace(fallbackName), "...", fallbackName)
                lbllrnn.Text = If(String.IsNullOrWhiteSpace(lrn), "...", lrn)
                lblempp.Text = If(String.IsNullOrWhiteSpace(employeeNo), "...", employeeNo)
            End Try
        Else

            Dim latest As DataRow = GetLatestHistoryRow(borrowerName, lrn, employeeNo)
            If latest IsNot Nothing Then
                Try
                    If latest.Table.Columns.Contains("LRN") Then lbllrnn.Text = If(IsDBNull(latest("LRN")), "...", latest("LRN").ToString())
                    If latest.Table.Columns.Contains("EmployeeNo") Then lblempp.Text = If(IsDBNull(latest("EmployeeNo")), "...", latest("EmployeeNo").ToString())
                    Dim resolvedName As String = String.Empty
                    If latest.Table.Columns.Contains("Name") AndAlso Not IsDBNull(latest("Name")) Then
                        resolvedName = latest("Name").ToString()
                    ElseIf latest.Table.Columns.Contains("LRN") AndAlso Not IsDBNull(latest("LRN")) Then
                        resolvedName = GetBorrowerFullNameByIdentifier("LRN", latest("LRN").ToString())
                    ElseIf latest.Table.Columns.Contains("EmployeeNo") AndAlso Not IsDBNull(latest("EmployeeNo")) Then
                        resolvedName = GetBorrowerFullNameByIdentifier("EmployeeNo", latest("EmployeeNo").ToString())
                    End If

                    If Not String.IsNullOrWhiteSpace(resolvedName) Then
                        lblname.Text = resolvedName
                    Else
                        Dim fallbackName As String = If(Not String.IsNullOrWhiteSpace(loggedInName), loggedInName, borrowerName)
                        lblname.Text = If(String.IsNullOrWhiteSpace(fallbackName), "...", fallbackName)
                    End If
                Catch
                    Dim fallbackName As String = If(Not String.IsNullOrWhiteSpace(loggedInName), loggedInName, borrowerName)
                    lblname.Text = If(String.IsNullOrWhiteSpace(fallbackName), "...", fallbackName)
                End Try
            Else
                Dim fallbackName As String = If(Not String.IsNullOrWhiteSpace(loggedInName), loggedInName, borrowerName)
                lblname.Text = If(String.IsNullOrWhiteSpace(fallbackName), "...", fallbackName)
                lbllrnn.Text = If(String.IsNullOrWhiteSpace(lrn), "...", lrn)
                lblempp.Text = If(String.IsNullOrWhiteSpace(employeeNo), "...", employeeNo)
            End If
        End If

        Me.ShowDialog()
    End Sub

    Private Sub AdjustLabelPositions(borrowerType As String)
        If Not originalPositionsRecorded Then
            popuphistory_Load(Nothing, Nothing)
        End If

        Dim isTeacher As Boolean = borrowerType.ToLowerInvariant().Contains("teach") OrElse borrowerType.ToLowerInvariant().Contains("emp")

        If isTeacher Then

            Try
                lrnlabel.Location = New Point(25, 101)
                lbllrnn.Location = New Point(143, 101)

                lblempy.Location = origLrnLabelPos
                lblempp.Location = origLrnValuePos
            Catch
            End Try
        Else

            Try
                lrnlabel.Location = origLrnLabelPos
                lbllrnn.Location = origLrnValuePos
                lblempy.Location = origEmpLabelPos
                lblempp.Location = origEmpValuePos
            Catch
            End Try
        End If
    End Sub

    Private Sub LoadHistory(borrowerName As String, lrn As String, employeeNo As String)

        Dim con As New MySql.Data.MySqlClient.MySqlConnection(GlobalVarsModule.connectionString)

        Dim conditions As New List(Of String)
        If Not String.IsNullOrWhiteSpace(borrowerName) Then conditions.Add("(Borrower = @Borrower)")
        If Not String.IsNullOrWhiteSpace(lrn) Then conditions.Add("(LRN = @LRN)")
        If Not String.IsNullOrWhiteSpace(employeeNo) Then conditions.Add("(EmployeeNo = @EmployeeNo)")

        Dim query As String
        If conditions.Count = 0 Then
            query = "SELECT * FROM borrowinghistory_tbl ORDER BY BorrowedDate DESC"
        Else
            query = "SELECT * FROM borrowinghistory_tbl WHERE " & String.Join(" OR ", conditions) & " ORDER BY BorrowedDate DESC"
        End If

        Dim adap As New MySql.Data.MySqlClient.MySqlDataAdapter()
        Dim ds As New DataSet()

        Try
            adap.SelectCommand = New MySql.Data.MySqlClient.MySqlCommand(query, con)
            If query.Contains("@Borrower") Then adap.SelectCommand.Parameters.AddWithValue("@Borrower", borrowerName)
            If query.Contains("@LRN") Then adap.SelectCommand.Parameters.AddWithValue("@LRN", lrn)
            If query.Contains("@EmployeeNo") Then adap.SelectCommand.Parameters.AddWithValue("@EmployeeNo", employeeNo)

            adap.Fill(ds, "history")

            Dim dtOut As New DataTable()
            dtOut.Columns.Add("Borrower", GetType(String))
            dtOut.Columns.Add("LRN", GetType(String))
            dtOut.Columns.Add("EmployeeNo", GetType(String))
            dtOut.Columns.Add("BookTitle", GetType(String))
            dtOut.Columns.Add("BorrowedDate", GetType(String))

            If ds.Tables.Contains("history") Then
                For Each r As DataRow In ds.Tables("history").Rows
                    Dim br As String = If(ds.Tables("history").Columns.Contains("Borrower") AndAlso Not IsDBNull(r("Borrower")), r("Borrower").ToString(), "")
                    Dim lrnVal As String = If(ds.Tables("history").Columns.Contains("LRN") AndAlso Not IsDBNull(r("LRN")), r("LRN").ToString(), "")
                    Dim empVal As String = If(ds.Tables("history").Columns.Contains("EmployeeNo") AndAlso Not IsDBNull(r("EmployeeNo")), r("EmployeeNo").ToString(), "")
                    Dim title As String = If(ds.Tables("history").Columns.Contains("BookTitle") AndAlso Not IsDBNull(r("BookTitle")), r("BookTitle").ToString(), "")
                    Dim bdate As String = If(ds.Tables("history").Columns.Contains("BorrowedDate") AndAlso Not IsDBNull(r("BorrowedDate")), r("BorrowedDate").ToString(), "")


                    Dim displayName As String = br
                    If String.Equals(br, "Student", StringComparison.OrdinalIgnoreCase) OrElse String.Equals(br, "Teacher", StringComparison.OrdinalIgnoreCase) OrElse String.IsNullOrWhiteSpace(br) Then
                        If Not String.IsNullOrWhiteSpace(lrnVal) Then
                            Dim resolved = GetBorrowerFullNameByIdentifier("LRN", lrnVal)
                            If Not String.IsNullOrWhiteSpace(resolved) Then displayName = resolved
                        End If
                        If String.IsNullOrWhiteSpace(displayName) AndAlso Not String.IsNullOrWhiteSpace(empVal) Then
                            Dim resolved = GetBorrowerFullNameByIdentifier("EmployeeNo", empVal)
                            If Not String.IsNullOrWhiteSpace(resolved) Then displayName = resolved
                        End If
                    End If

                    dtOut.Rows.Add(displayName, lrnVal, empVal, title, bdate)
                Next
            End If

            DataGridView1.DataSource = dtOut


            DataGridView1.AutoGenerateColumns = True

            DataGridView1.EnableHeadersVisualStyles = False
            DataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(207, 58, 109)
            DataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White
            DataGridView1.ClearSelection()

            For Each col As DataGridViewColumn In DataGridView1.Columns
                col.HeaderText = col.HeaderText.Trim()
                col.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            Next

            DataGridView1.ClearSelection()
        Catch ex As Exception
            MessageBox.Show("Error loading history: " & ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            If con.State = ConnectionState.Open Then con.Close()
        End Try
    End Sub

    Private Function GetLatestHistoryRow(borrowerName As String, lrn As String, employeeNo As String) As DataRow
        Dim con As New MySql.Data.MySqlClient.MySqlConnection(GlobalVarsModule.connectionString)
        Dim query As String = "SELECT * FROM borrowinghistory_tbl WHERE (Borrower = @Borrower)"
        If Not String.IsNullOrWhiteSpace(lrn) Then query &= " OR (LRN = @LRN)"
        If Not String.IsNullOrWhiteSpace(employeeNo) Then query &= " OR (EmployeeNo = @EmployeeNo)"
        query &= " ORDER BY BorrowedDate DESC LIMIT 1"

        Dim cmd As New MySql.Data.MySqlClient.MySqlCommand(query, con)
        cmd.Parameters.AddWithValue("@Borrower", borrowerName)
        If query.Contains("@LRN") Then cmd.Parameters.AddWithValue("@LRN", lrn)
        If query.Contains("@EmployeeNo") Then cmd.Parameters.AddWithValue("@EmployeeNo", employeeNo)

        Try
            con.Open()
            Dim reader = cmd.ExecuteReader()
            If reader.Read() Then
                Dim dt As New DataTable()
                dt.Load(reader)
                If dt.Rows.Count > 0 Then
                    Return dt.Rows(0)
                End If
            End If
            reader.Close()
        Catch
        Finally
            If con.State = ConnectionState.Open Then con.Close()
        End Try

        Return Nothing
    End Function

    Private Function GetBorrowerFullNameByIdentifier(idColumn As String, idValue As String) As String
        Try
            Using con As New MySql.Data.MySqlClient.MySqlConnection(GlobalVarsModule.connectionString)
                con.Open()
                Dim sql As String = String.Format("SELECT CONCAT_WS(' ', FirstName, LastName, NULLIF(TRIM(REPLACE(MiddleInitial, '.', '')), 'N/A')) AS FullName FROM borrower_tbl WHERE {0} = @id LIMIT 1", idColumn)
                Using cmd As New MySql.Data.MySqlClient.MySqlCommand(sql, con)
                    cmd.Parameters.AddWithValue("@id", idValue)
                    Dim obj = cmd.ExecuteScalar()
                    If obj IsNot Nothing AndAlso Not IsDBNull(obj) Then
                        Return obj.ToString()
                    End If
                End Using
            End Using
        Catch
        End Try
        Return String.Empty
    End Function
End Class