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

        borrowerName = If(borrowerName?.Trim(), String.Empty)
        lrn = If(lrn?.Trim(), String.Empty)
        employeeNo = If(employeeNo?.Trim(), String.Empty)


        lbltyp.Text = If(String.IsNullOrWhiteSpace(borrowerType), "...", borrowerType)
        AdjustLabelPositions(borrowerType)

        Dim resolvedFullName As String = String.Empty
        Try
            Using con As New MySqlConnection(GlobalVarsModule.connectionString)
                con.Open()
                If Not String.IsNullOrWhiteSpace(employeeNo) Then
                    Using cmd As New MySqlCommand("SELECT CONCAT_WS(' ', FirstName, LastName, NULLIF(TRIM(REPLACE(MiddleInitial, '.', '')), 'N/A')) FROM borrower_tbl WHERE EmployeeNo = @id LIMIT 1", con)
                        cmd.Parameters.AddWithValue("@id", employeeNo)
                        Dim obj = cmd.ExecuteScalar()
                        If obj IsNot Nothing AndAlso Not IsDBNull(obj) Then resolvedFullName = obj.ToString()
                    End Using
                ElseIf Not String.IsNullOrWhiteSpace(lrn) Then
                    Using cmd As New MySqlCommand("SELECT CONCAT_WS(' ', FirstName, LastName, NULLIF(TRIM(REPLACE(MiddleInitial, '.', '')), 'N/A')) FROM borrower_tbl WHERE LRN = @id LIMIT 1", con)
                        cmd.Parameters.AddWithValue("@id", lrn)
                        Dim obj = cmd.ExecuteScalar()
                        If obj IsNot Nothing AndAlso Not IsDBNull(obj) Then resolvedFullName = obj.ToString()
                    End Using
                ElseIf Not String.IsNullOrWhiteSpace(borrowerName) Then
                    resolvedFullName = borrowerName
                ElseIf GlobalVarsModule.CurrentUserRole = "Borrower" Then

                    If Not String.IsNullOrWhiteSpace(GlobalVarsModule.GlobalFullname) Then
                        resolvedFullName = GlobalVarsModule.GlobalFullname
                    ElseIf Not String.IsNullOrWhiteSpace(GlobalVarsModule.CurrentBorrowerID) Then
                        If String.Equals(GlobalVarsModule.CurrentBorrowerType, "Student", StringComparison.OrdinalIgnoreCase) Then
                            Using cmd As New MySqlCommand("SELECT CONCAT_WS(' ', FirstName, LastName, NULLIF(TRIM(REPLACE(MiddleInitial, '.', '')), 'N/A')) FROM borrower_tbl WHERE LRN = @id LIMIT 1", con)
                                cmd.Parameters.AddWithValue("@id", GlobalVarsModule.CurrentBorrowerID)
                                Dim obj = cmd.ExecuteScalar()
                                If obj IsNot Nothing AndAlso Not IsDBNull(obj) Then resolvedFullName = obj.ToString()
                            End Using
                        ElseIf String.Equals(GlobalVarsModule.CurrentBorrowerType, "Teacher", StringComparison.OrdinalIgnoreCase) Then
                            Using cmd As New MySqlCommand("SELECT CONCAT_WS(' ', FirstName, LastName, NULLIF(TRIM(REPLACE(MiddleInitial, '.', '')), 'N/A')) FROM borrower_tbl WHERE EmployeeNo = @id LIMIT 1", con)
                                cmd.Parameters.AddWithValue("@id", GlobalVarsModule.CurrentBorrowerID)
                                Dim obj = cmd.ExecuteScalar()
                                If obj IsNot Nothing AndAlso Not IsDBNull(obj) Then resolvedFullName = obj.ToString()
                            End Using
                        End If
                    End If
                End If
            End Using
        Catch
        End Try


        lblname.Text = If(String.IsNullOrWhiteSpace(resolvedFullName), "...", resolvedFullName)
        lbllrnn.Text = If(String.IsNullOrWhiteSpace(lrn), "...", lrn)
        lblempp.Text = If(String.IsNullOrWhiteSpace(employeeNo), "...", employeeNo)

        LoadHistory(resolvedFullName, lrn, employeeNo)

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

        Dim query As String
        If Not String.IsNullOrWhiteSpace(employeeNo) Then
            query = "SELECT * FROM borrowinghistory_tbl WHERE EmployeeNo = @EmployeeNo ORDER BY BorrowedDate DESC"
        ElseIf Not String.IsNullOrWhiteSpace(lrn) Then
            query = "SELECT * FROM borrowinghistory_tbl WHERE LRN = @LRN ORDER BY BorrowedDate DESC"
        ElseIf Not String.IsNullOrWhiteSpace(borrowerName) Then

            query = "SELECT * FROM borrowinghistory_tbl WHERE Name = @Name ORDER BY BorrowedDate DESC"
        Else
            query = "SELECT * FROM borrowinghistory_tbl ORDER BY BorrowedDate DESC"
        End If

        Dim adap As New MySql.Data.MySqlClient.MySqlDataAdapter()
        Dim ds As New DataSet()

        Try
            adap.SelectCommand = New MySql.Data.MySqlClient.MySqlCommand(query, con)
            If query.Contains("@EmployeeNo") Then adap.SelectCommand.Parameters.AddWithValue("@EmployeeNo", employeeNo)
            If query.Contains("@LRN") Then adap.SelectCommand.Parameters.AddWithValue("@LRN", lrn)
            If query.Contains("@Name") Then adap.SelectCommand.Parameters.AddWithValue("@Name", borrowerName)

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