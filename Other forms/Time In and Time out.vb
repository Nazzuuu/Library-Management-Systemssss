Imports MySql.Data.MySqlClient
Imports System.Data
Imports System.Windows.Forms
Imports System.Drawing

Public Class oras

    Dim selectedID As Integer
    Private WithEvents Timer1 As New Timer()


    Private ReadOnly connectionString As String = GlobalVarsModule.connectionString


    Private Sub btnregisterview_Click(sender As Object, e As EventArgs) Handles btnregisterview.Click

        RegisteredBrwr.lbl_action.ForeColor = Color.Red
        RegisteredBrwr.lbl_action.Text = "Selecting"

        RegisteredBrwr.ListView1.Enabled = True

        RegisteredBrwr.IsTimeInMode = True
        RegisteredBrwr.IsSearchOverride = True

        If GlobalVarsModule.CurrentUserRole = "Borrower" Then

            RegisteredBrwr.SetSearchID(GlobalVarsModule.CurrentUserID)
        End If


        RegisteredBrwr.ShowDialog()

        RegisteredBrwr.inibol()

        RegisteredBrwr.ludeyngborrower()
        RegisteredBrwr.ludeyngtimedinborrower()

    End Sub

    Private Sub btnview_Click(sender As Object, e As EventArgs) Handles btnview.Click

        RegisteredBrwr.IsInViewMode = True

        RegisteredBrwr.lbl_action.ForeColor = Color.Green
        RegisteredBrwr.lbl_action.Text = "Viewing"

        RegisteredBrwr.ListView1.FullRowSelect = True
        RegisteredBrwr.ListView1.MultiSelect = False
        RegisteredBrwr.ListView1.HideSelection = False
        RegisteredBrwr.ListView1.LabelEdit = False

        If GlobalVarsModule.CurrentUserRole = "Borrower" Then

            RegisteredBrwr.SetSearchID(GlobalVarsModule.CurrentUserID)
        End If


        RegisteredBrwr.ShowDialog()
        RegisteredBrwr.ludeyngborrower()
        RegisteredBrwr.ludeyngtimedinborrower()


        RegisteredBrwr.IsInViewMode = False

    End Sub



    Private Sub oras_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ludeyngoras()
        clearlahat()

        Timer1.Interval = 1000
        Timer1.Start()


        Dim isBorrowerRole As Boolean = (GlobalVarsModule.CurrentUserRole = "Borrower" OrElse GlobalVarsModule.CurrentUserRole = "Student" OrElse GlobalVarsModule.CurrentUserRole = "Teacher")

        If isBorrowerRole Then


            btnregisterview.Visible = False
            btnedit.Enabled = True
            btnclear.Enabled = True
            btnview.Location = New Point(398, 274) ' Adjusted location for single button
            txtsearch.Enabled = False ' Assuming Borrower cannot search other's records

        Else

            txtsearch.Enabled = True
            btnregisterview.Visible = True
            btnedit.Enabled = True
            btnclear.Enabled = True
            btnview.Location = New Point(607, 274)

        End If
    End Sub

    Private Sub Oras_Shown(sender As Object, e As EventArgs) Handles MyBase.Shown

        DataGridView1.ClearSelection()

    End Sub

    Private Sub oras_FormClosed(sender As Object, e As FormClosedEventArgs) Handles MyBase.FormClosed

        Timer1.Stop()
    End Sub


    Public Sub ludeyngoras()

        DataGridView1.DataSource = Nothing
        DataGridView1.Columns.Clear()

        Dim currentUserRole As String = GlobalVarsModule.CurrentUserRole
        Dim currentBorrowerID As String = GlobalVarsModule.CurrentBorrowerID

        Dim com As String = "SELECT " &
                            "o.`ID`, " &
                            "b.`Borrower`, " &
                            "o.`LRN`, " &
                            "o.`EmployeeNo`, " &
                            "b.`FirstName`, " &
                            "b.`LastName`, " &
                            "b.`MiddleInitial`, " &
                            "b.`ContactNumber`, " &
                            "b.`Department`, " &
                            "b.`Grade`, " &
                            "b.`Section`, " &
                            "b.`Strand`, " &
                            "o.`TimeIn`, " &
                            "o.`TimeOut` " &
                            "FROM `oras_tbl` o " &
                            "LEFT JOIN `borrower_tbl` b " &
                            "ON o.`LRN` = b.`LRN` OR o.`EmployeeNo` = b.`EmployeeNo` " &
                            "WHERE o.`TimeOut` IS NULL "


        If currentUserRole = "Borrower" AndAlso Not String.IsNullOrWhiteSpace(currentBorrowerID) Then

            com &= " AND (o.`LRN` = @ID OR o.`EmployeeNo` = @ID)"
        End If


        com &= " ORDER BY o.`TimeIn` DESC"

        Using con As New MySqlConnection(connectionString)
            Try
                con.Open()
                Using cmd As New MySqlCommand(com, con)

                    If currentUserRole = "Borrower" AndAlso Not String.IsNullOrWhiteSpace(currentBorrowerID) Then

                        cmd.Parameters.AddWithValue("@ID", currentBorrowerID)
                    End If


                    Dim adap As New MySqlDataAdapter(cmd)
                    Dim ds As New DataSet
                    adap.Fill(ds, "oras_data")

                    DataGridView1.DataSource = ds.Tables("oras_data")

                    If DataGridView1.Columns.Contains("ID") Then
                        DataGridView1.Columns("ID").Visible = False
                    End If

                    DataGridView1.ClearSelection()

                    DataGridView1.EnableHeadersVisualStyles = False
                    DataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(207, 58, 109)
                    DataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White

                End Using
            Catch ex As Exception
                MessageBox.Show("Error loading time records: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End Using

    End Sub

    Public Sub clearlahat()

        txtborrower.Enabled = False
        txtcontact.Enabled = False
        txtdepartment.Enabled = False
        txtemployee.Enabled = False
        txtfname.Enabled = False
        txtlname.Enabled = False
        txtmname.Enabled = False
        txtgrade.Enabled = False
        txtlrn.Enabled = False
        txtsection.Enabled = False
        txtstrand.Enabled = False
        rbtimeout.Enabled = False
        rbtimeout.Checked = False


        txtborrower.Text = ""
        txtcontact.Text = ""
        txtdepartment.Text = ""
        txtemployee.Text = ""
        txtfname.Text = ""
        txtlname.Text = ""
        txtmname.Text = ""
        txtsection.Text = ""
        txtstrand.Text = ""
        txtlrn.Text = ""
        txtgrade.Text = ""
        selectedID = 0

        DataGridView1.ClearSelection()

    End Sub


    Public Sub brwrinfo(borrowerType As String, firstName As String, lastName As String, middleInitial As String, lrn As String, employeeNo As String, contactNumber As String, department As String, grade As String, section As String, strand As String)

        txtborrower.Text = borrowerType
        txtfname.Text = firstName
        txtlname.Text = lastName

        txtmname.Text = middleInitial
        txtlrn.Text = lrn
        txtemployee.Text = employeeNo
        txtcontact.Text = contactNumber
        txtdepartment.Text = department
        txtgrade.Text = grade
        txtsection.Text = section
        txtstrand.Text = strand

    End Sub

    Private Sub btnclear_Click(sender As Object, e As EventArgs) Handles btnclear.Click

        clearlahat()

    End Sub



    Private Sub btnedit_Click(sender As Object, e As EventArgs) Handles btnedit.Click

        If selectedID = 0 Then
            MessageBox.Show("Please select a record to update.", "Selection Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        If Not rbtimeout.Checked Then
            MessageBox.Show("Please check the 'Time out' option to record time out.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If


        If GlobalVarsModule.CurrentUserRole = "Borrower" Then

            If Not IsRecordOwnedByCurrentUser(selectedID) Then
                MessageBox.Show("Security Alert: You can only Time Out your own record.", "Unauthorized Action", MessageBoxButtons.OK, MessageBoxIcon.Stop)
                Return
            End If
        End If


        Dim oldTimeIn As String = ""
        Dim borrowerIdentifier As String = ""
        Dim borrowerType As String = ""

        Using conCheck As New MySqlConnection(connectionString)
            Try
                conCheck.Open()
                Dim checkCmd As New MySqlCommand("SELECT TimeIn, Borrower, LRN, EmployeeNo FROM `oras_tbl` WHERE ID = @ID", conCheck)
                checkCmd.Parameters.AddWithValue("@ID", selectedID)
                Using reader As MySqlDataReader = checkCmd.ExecuteReader()
                    If reader.Read() Then
                        oldTimeIn = reader("TimeIn").ToString()
                        borrowerType = reader("Borrower").ToString()
                        borrowerIdentifier = If(Not reader.IsDBNull(reader.GetOrdinal("LRN")), reader("LRN").ToString(), reader("EmployeeNo").ToString())
                    End If
                End Using
            Catch ex As Exception

                Console.WriteLine("Error retrieving old TimeIn for audit: " & ex.Message)
            End Try
        End Using


        Using con As New MySqlConnection(connectionString)

            Dim com As String = "UPDATE `oras_tbl` SET `TimeOut` = NOW() WHERE `ID` = @ID"

            Using cmd As New MySqlCommand(com, con)
                cmd.Parameters.AddWithValue("@ID", selectedID)

                Try
                    con.Open()
                    cmd.ExecuteNonQuery()


                    Dim timeOutTime As String = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                    Dim logDescription As String = $"Time-Out recorded for {borrowerType} ID {borrowerIdentifier}. Time In: {oldTimeIn}"

                    GlobalVarsModule.LogAudit(
                        actionType:="UPDATE",
                        formName:="TIME-IN/OUT FORM",
                        description:=logDescription,
                        recordID:=selectedID.ToString(),
                        oldValue:=$"Time In: {oldTimeIn}, Time Out: NULL",
                        newValue:=$"Time In: {oldTimeIn}, Time Out: {timeOutTime}"
                    )

                    For Each form In Application.OpenForms
                        If TypeOf form Is AuditTrail Then
                            DirectCast(form, AuditTrail).refreshaudit()
                        End If
                    Next



                    Dim stayLogoutFormInstance As StayLogoutFormm = Application.OpenForms.OfType(Of StayLogoutFormm)().FirstOrDefault()

                    If stayLogoutFormInstance IsNot Nothing Then

                        stayLogoutFormInstance.IsTimedIn = False

                        stayLogoutFormInstance.btnstay.Visible = False
                        stayLogoutFormInstance.lblmessage.Text = "Attendance must before borrowing."

                    End If



                    MessageBox.Show("Time-out recorded successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)

                    btnview.Visible = True

                    ludeyngoras()
                    clearlahat()

                    Dim registeredForm As RegisteredBrwr = Application.OpenForms.OfType(Of RegisteredBrwr)().FirstOrDefault()
                    If registeredForm IsNot Nothing Then
                        registeredForm.ludeyngtimedinborrower()
                    End If

                    Dim hays As TimeInOutRecord = Application.OpenForms.OfType(Of TimeInOutRecord)().FirstOrDefault()
                    If hays IsNot Nothing Then
                        hays.refreshtimeoutrecrod()
                    End If

                Catch ex As Exception
                    MessageBox.Show("Error recording time-out: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End Try
            End Using
        End Using

    End Sub


    Private Function IsRecordOwnedByCurrentUser(recordID As Integer) As Boolean
        Dim isOwner As Boolean = False
        Dim currentID As String = GlobalVarsModule.CurrentBorrowerID

        If String.IsNullOrWhiteSpace(currentID) OrElse GlobalVarsModule.CurrentUserRole <> "Borrower" Then

            Return True
        End If

        Dim query As String = "SELECT 1 FROM `oras_tbl` WHERE `ID` = @RecID AND (`LRN` = @BorrowerID OR `EmployeeNo` = @BorrowerID) LIMIT 1"

        Using con As New MySqlConnection(connectionString)
            Try
                con.Open()
                Using cmd As New MySqlCommand(query, con)
                    cmd.Parameters.AddWithValue("@RecID", recordID)
                    cmd.Parameters.AddWithValue("@BorrowerID", currentID)

                    If cmd.ExecuteScalar() IsNot Nothing Then
                        isOwner = True
                    End If
                End Using
            Catch ex As Exception
                MessageBox.Show("Database error during security check: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End Using

        Return isOwner
    End Function

    Private Sub DataGridView1_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellClick

        If e.RowIndex >= 0 Then
            Dim row As DataGridViewRow = DataGridView1.Rows(e.RowIndex)

            txtborrower.Text = If(row.Cells("Borrower").Value Is DBNull.Value, "", row.Cells("Borrower").Value.ToString())
            txtfname.Text = If(row.Cells("FirstName").Value Is DBNull.Value, "", row.Cells("FirstName").Value.ToString())
            txtlname.Text = If(row.Cells("LastName").Value Is DBNull.Value, "", row.Cells("LastName").Value.ToString())

            txtmname.Text = If(row.Cells("MiddleInitial").Value Is DBNull.Value, "", row.Cells("MiddleInitial").Value.ToString())

            txtlrn.Text = If(row.Cells("LRN").Value Is DBNull.Value, "", row.Cells("LRN").Value.ToString())
            txtemployee.Text = If(row.Cells("EmployeeNo").Value Is DBNull.Value, "", row.Cells("EmployeeNo").Value.ToString())
            txtcontact.Text = If(row.Cells("ContactNumber").Value Is DBNull.Value, "", row.Cells("ContactNumber").Value.ToString())
            txtdepartment.Text = If(row.Cells("Department").Value Is DBNull.Value, "", row.Cells("Department").Value.ToString())
            txtgrade.Text = If(row.Cells("Grade").Value Is DBNull.Value, "", row.Cells("Grade").Value.ToString())
            txtsection.Text = If(row.Cells("Section").Value Is DBNull.Value, "", row.Cells("Section").Value.ToString())
            txtstrand.Text = If(row.Cells("Strand").Value Is DBNull.Value, "", row.Cells("Strand").Value.ToString())

            Dim timeout As Object = row.Cells("TimeOut").Value

            If timeout Is DBNull.Value OrElse String.IsNullOrWhiteSpace(timeout.ToString()) Then
                rbtimeout.Enabled = True
                rbtimeout.Checked = False
            Else
                rbtimeout.Enabled = False
                rbtimeout.Checked = True
            End If

            selectedID = CInt(row.Cells("ID").Value)
            rbtimeout.Checked = True
        End If

    End Sub



    Private Sub txtsearch_TextChanged(sender As Object, e As EventArgs) Handles txtsearch.TextChanged

        Dim dt As DataTable = DirectCast(DataGridView1.DataSource, DataTable)
        If dt IsNot Nothing Then
            If txtsearch.Text.Trim() <> "" Then

                Dim filter As String = String.Format("Borrower LIKE '%{0}%' OR FirstName LIKE '%{0}%' OR LastName LIKE '%{0}%' OR LRN LIKE '%{0}%' OR EmployeeNo LIKE '%{0}%'", txtsearch.Text.Trim())

                dt.DefaultView.RowFilter = filter
            Else
                dt.DefaultView.RowFilter = ""
            End If
        End If

    End Sub

End Class