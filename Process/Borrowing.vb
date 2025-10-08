Imports MySql.Data.MySqlClient
Imports System.Data
Imports System.Drawing
Imports ZXing
Imports ZXing.Rendering
Imports ZXing.Windows.Compatibility
Public Class Borrowing

    Private isLoadingData As Boolean = False
    Private WithEvents timerSystemDate As New Timer()

    Private Sub Borrowing_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        refreshborrowingsu()

        timerSystemDate.Interval = 1000
        timerSystemDate.Start()

        btntimein.Visible = False
        UpdateTransactionBarcode()

    End Sub

    Private Function GenerateUniqueTransactionID() As String

        Return DateTime.Now.ToString("yyMMddHHmmss")
    End Function

    Public Sub UpdateTransactionBarcode()
        Dim newID As String = GenerateUniqueTransactionID()

        Try

            Dim writer As New BarcodeWriter With {
            .Format = BarcodeFormat.CODE_128,
            .Renderer = New BitmapRenderer()
        }


            writer.Options = New ZXing.Common.EncodingOptions With {
            .Height = picbarcode.Height,
            .Width = picbarcode.Width,
            .PureBarcode = False,
            .Margin = 10
        }

            Dim barcodeBitmap As Bitmap = writer.Write(newID)


            If picbarcode.Image IsNot Nothing Then
                picbarcode.Image.Dispose()
            End If
            picbarcode.Image = barcodeBitmap


            lbltransac.Text = newID

        Catch ex As Exception
            MessageBox.Show("Error generating barcode with ZXing.Net: " & ex.Message, "Barcode Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
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

    Private Sub Borrowing_Activated(sender As Object, e As EventArgs) Handles Me.Activated

        refreshborrowingsu()
    End Sub

    Public Sub refreshborrowingsu()


        Dim borrowerType As String = GlobalVarsModule.CurrentBorrowerType
        Dim borrowerID As String = GlobalVarsModule.CurrentBorrowerID

        Dim con As New MySqlConnection(GlobalVarsModule.connectionString)
        Dim com As String = ""
        Dim identifierColumn As String = ""

        Dim showAllRecords As Boolean = True


        If GlobalVarsModule.CurrentUserRole = "Borrower" AndAlso Not String.IsNullOrWhiteSpace(borrowerID) Then
            showAllRecords = False

            If borrowerType = "Student" Then
                identifierColumn = "LRN"

                com = $"SELECT * FROM `borrowing_tbl` WHERE `{identifierColumn}` = @BorrowerID ORDER BY `BorrowedDate` DESC"
            ElseIf borrowerType = "Teacher" Then
                identifierColumn = "EmployeeNo"

                com = $"SELECT * FROM `borrowing_tbl` WHERE `{identifierColumn}` = @BorrowerID ORDER BY `BorrowedDate` DESC"
            End If
        End If

        If showAllRecords OrElse String.IsNullOrWhiteSpace(com) Then
            com = "SELECT * FROM `borrowing_tbl` ORDER BY `BorrowedDate` DESC"
        End If


        Dim adap As New MySqlDataAdapter()
        Dim ds As New DataSet

        Try
            con.Open()

            Using cmd As New MySqlCommand(com, con)

                If Not showAllRecords Then

                    cmd.Parameters.AddWithValue("@BorrowerID", borrowerID)
                End If

                adap.SelectCommand = cmd
                adap.Fill(ds, "borrowing_data")

                DataGridView1.DataSource = ds.Tables("borrowing_data")


                If DataGridView1.Columns.Contains("ID") Then
                    DataGridView1.Columns("ID").Visible = False
                End If


                If GlobalVarsModule.CurrentUserRole = "Borrower" Then
                    If DataGridView1.Columns.Contains("LRN") AndAlso borrowerType = "Teacher" Then
                        DataGridView1.Columns("LRN").Visible = False
                    End If
                    If DataGridView1.Columns.Contains("EmployeeNo") AndAlso borrowerType = "Student" Then
                        DataGridView1.Columns("EmployeeNo").Visible = False
                    End If
                End If


                DataGridView1.EnableHeadersVisualStyles = False
                DataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(207, 58, 109)
                DataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White
            End Using
        Catch ex As Exception
            MessageBox.Show("Error loading borrowing records: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
        End Try

        clearlahat()

    End Sub

    Private Sub Borrowing_Shown(sender As Object, e As EventArgs) Handles MyBase.Shown

        DataGridView1.ClearSelection()

    End Sub


    Public Sub clearlahat()


        DataGridView1.ClearSelection()
        DataGridView1.CurrentCell = Nothing


        txtname.Enabled = False
        txtbooktitle.Enabled = False
        txtisbn.Enabled = False
        txtbarcode.Enabled = False
        txtshelf.Enabled = False
        txtaccessionid.Enabled = False
        txtduedate.Enabled = False

        txtlrn.Text = ""
        txtemployee.Text = ""
        txtisbn.Text = ""
        txtname.Text = ""
        txtbooktitle.Text = ""
        txtbarcode.Text = ""
        txtaccessionid.Text = ""
        txtshelf.Text = ""


        btntimein.Visible = False

        DateTimePicker1.Value = DateTime.Now

    End Sub


    Private Sub rbteacher_CheckedChanged(sender As Object, e As EventArgs) Handles rbteacher.CheckedChanged

        If rbteacher.Checked Then

            txtemployee.Enabled = True
            txtlrn.Enabled = False

            txtlrn.Text = ""
            txtname.Text = ""
            btntimein.Visible = False

        End If

    End Sub

    Private Sub rbstudent_CheckedChanged(sender As Object, e As EventArgs) Handles rbstudent.CheckedChanged

        If rbstudent.Checked Then

            txtemployee.Enabled = False
            txtlrn.Enabled = True

            txtemployee.Text = ""
            txtname.Text = ""
            btntimein.Visible = False

        End If

    End Sub

    Private Const MAX_BORROWED_BOOKS As Integer = 3
    Public Function limitniborrower(ByVal identifier As String, ByVal identifierType As String) As Boolean

        Dim con As New MySqlConnection(connectionString)
        Dim count As Integer = 0
        Dim identifierColumn As String = ""

        If identifierType = "LRN" Then
            identifierColumn = "LRN"
        ElseIf identifierType = "EmployeeNo" Then
            identifierColumn = "EmployeeNo"
        Else
            Return False
        End If


        Dim com As String = $"SELECT COUNT(*) FROM `borrowing_tbl` WHERE `{identifierColumn}` = @Identifier"

        Try
            con.Open()
            Using cmd As New MySqlCommand(com, con)
                cmd.Parameters.AddWithValue("@Identifier", identifier)

                count = CInt(cmd.ExecuteScalar())
            End Using


            Return count < MAX_BORROWED_BOOKS

        Catch ex As Exception

            MessageBox.Show("Error checking borrower limit: " & ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        Finally
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
        End Try

    End Function

    Private Sub txtemployee_TextChanged(sender As Object, e As EventArgs) Handles txtemployee.TextChanged

        If isLoadingData Then Exit Sub

        If String.IsNullOrWhiteSpace(txtemployee.Text) Then

            txtname.Text = ""
            btntimein.Visible = False
            Exit Sub
        End If


        Dim enteredEmployeeID As String = txtemployee.Text.Trim()


        Dim currentUserID_Cleaned As String = GlobalVarsModule.GetCleanCurrentBorrowerID()
        Dim enteredEmployeeID_Cleaned As String = enteredEmployeeID

        Dim con As New MySqlConnection(GlobalVarsModule.connectionString)
        Dim foundBorrower As Boolean = False
        Dim borrowerName As String = ""

        Try
            con.Open()

            Dim com As String = "SELECT CONCAT_WS(' ', `FirstName`, `LastName`) FROM `borrower_tbl` WHERE `EmployeeNo` = @emp"
            Using comsi As New MySqlCommand(com, con)
                comsi.Parameters.AddWithValue("@emp", enteredEmployeeID)
                Dim emp As Object = comsi.ExecuteScalar()
                If emp IsNot Nothing AndAlso Not String.IsNullOrWhiteSpace(emp.ToString()) Then
                    borrowerName = emp.ToString()
                    txtname.Text = borrowerName
                    foundBorrower = True
                Else
                    txtname.Text = ""
                End If
            End Using
        Catch ex As Exception
            MessageBox.Show("An error occurred while retrieving borrower information: " & ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
        End Try


        If GlobalVarsModule.CurrentUserRole = "Borrower" AndAlso GlobalVarsModule.CurrentBorrowerType = "Teacher" Then

            If foundBorrower AndAlso Not String.Equals(enteredEmployeeID_Cleaned, currentUserID_Cleaned, StringComparison.Ordinal) Then

                txtname.Text = ""
                btntimein.Visible = False

                MessageBox.Show($"The Employee No. '{enteredEmployeeID}' belongs to {borrowerName}. You are only allowed to search your own Employee No.", "Security Restriction", MessageBoxButtons.OK, MessageBoxIcon.Warning)

                Exit Sub
            End If
        End If

        If foundBorrower AndAlso Not String.IsNullOrWhiteSpace(enteredEmployeeID) Then


            Dim isTimedIn As Boolean = CheckTimeInStatus(enteredEmployeeID, "EmployeeNo")

            If Not isTimedIn Then
                MessageBox.Show($"NOTICE: {borrowerName} has not yet Timed In.", "Time In Required", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                btntimein.Visible = True
            Else
                btntimein.Visible = False
            End If
        Else
            btntimein.Visible = False
        End If

    End Sub


    Private Sub txtlrn_TextChanged(sender As Object, e As EventArgs) Handles txtlrn.TextChanged


        If isLoadingData Then Exit Sub

        Dim enteredLRN As String = txtlrn.Text.Trim()


        Dim currentUserID_Trimmed As String = GlobalVarsModule.CurrentBorrowerID.Trim()
        Dim currentUserID_Cleaned As String = ""
        Dim tempID As Long


        If Long.TryParse(currentUserID_Trimmed, tempID) Then
            currentUserID_Cleaned = tempID.ToString()
        Else
            currentUserID_Cleaned = currentUserID_Trimmed
        End If


        Dim enteredLRN_Cleaned As String = ""
        If Long.TryParse(enteredLRN, tempID) Then
            enteredLRN_Cleaned = tempID.ToString()
        Else
            enteredLRN_Cleaned = enteredLRN
        End If


        Dim con As New MySqlConnection(connectionString)
        Dim foundBorrower As Boolean = False
        Dim borrowerName As String = ""

        Try
            con.Open()

            Dim com As String = "SELECT CONCAT_WS(' ', `FirstName`, `LastName`) FROM `borrower_tbl` WHERE `LRN` = @lrn"
            Using comsi As New MySqlCommand(com, con)
                comsi.Parameters.AddWithValue("@lrn", enteredLRN)
                Dim lrn As Object = comsi.ExecuteScalar()
                If lrn IsNot Nothing AndAlso Not String.IsNullOrWhiteSpace(lrn.ToString()) Then
                    borrowerName = lrn.ToString()
                    txtname.Text = borrowerName
                    foundBorrower = True
                Else
                    txtname.Text = ""
                End If
            End Using
        Catch ex As Exception
            MessageBox.Show("An error occurred while retrieving borrower information: " & ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
        End Try


        If GlobalVarsModule.CurrentUserRole = "Borrower" AndAlso GlobalVarsModule.CurrentBorrowerType = "Student" Then

            If foundBorrower AndAlso Not String.Equals(enteredLRN_Cleaned, currentUserID_Cleaned, StringComparison.Ordinal) Then

                txtname.Text = ""
                btntimein.Visible = False


                MessageBox.Show($"The LRN '{enteredLRN}' belongs to {borrowerName}. You are only allowed to search your own LRN.", "Security Restriction", MessageBoxButtons.OK, MessageBoxIcon.Warning)


                Exit Sub
            End If
        End If



        If foundBorrower AndAlso Not String.IsNullOrWhiteSpace(enteredLRN) Then
            Dim isTimedIn As Boolean = CheckTimeInStatus(enteredLRN, "LRN")

            If Not isTimedIn Then
                MessageBox.Show($"NOTICE: {borrowerName} has not yet Timed In.", "Time In Required", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                btntimein.Visible = True
            Else
                btntimein.Visible = False
            End If
        Else
            btntimein.Visible = False
        End If

    End Sub
    Private Sub txtaccessionid_TextChanged(sender As Object, e As EventArgs) Handles txtaccessionid.TextChanged

        If isLoadingData Then
            Exit Sub
        End If

        If String.IsNullOrWhiteSpace(txtaccessionid.Text) Then

            txtbooktitle.Text = ""
            txtisbn.Text = ""
            txtbarcode.Text = ""
            txtshelf.Text = ""
            Exit Sub
        End If

        Dim con As New MySqlConnection(connectionString)
        Dim status As String = ""

        Try
            con.Open()

            Dim com As String = "SELECT `BookTitle`, `ISBN`, `Barcode`, `Shelf`, `Status` FROM `acession_tbl` WHERE `AccessionID` = @acs"

            Using comsi As New MySqlCommand(com, con)
                comsi.Parameters.AddWithValue("@acs", txtaccessionid.Text)

                Using reader As MySqlDataReader = comsi.ExecuteReader()
                    If reader.Read() Then

                        status = reader("Status").ToString()


                        If status.Equals("Available", StringComparison.OrdinalIgnoreCase) Then

                            txtbooktitle.Text = reader("BookTitle").ToString()
                            txtisbn.Text = reader("ISBN").ToString()
                            txtbarcode.Text = reader("Barcode").ToString()
                            txtshelf.Text = reader("Shelf").ToString()
                        Else

                            MessageBox.Show("WARNING: This book has a status of '" & status & "'. Only 'Available' books can be borrowed.", "Book Not Available", MessageBoxButtons.OK, MessageBoxIcon.Warning)


                            txtbooktitle.Text = ""
                            txtisbn.Text = ""
                            txtbarcode.Text = ""
                            txtshelf.Text = ""
                        End If
                    Else

                        txtbooktitle.Text = ""
                        txtisbn.Text = ""
                        txtbarcode.Text = ""
                        txtshelf.Text = ""
                    End If
                End Using
            End Using

        Catch ex As Exception
            MessageBox.Show("Database Error: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
        End Try
    End Sub

    Private Sub DateTimePicker1_ValueChanged(sender As Object, e As EventArgs) Handles DateTimePicker1.ValueChanged

        txtduedate.Text = DateTimePicker1.Value.AddDays(7).ToString("MMMM-dd-yyyy")
    End Sub



    Private Sub timerSystemDate_Tick(sender As Object, e As EventArgs) Handles timerSystemDate.Tick

        If DateTimePicker1.Value.Date <> DateTime.Now.Date Then
            DateTimePicker1.Value = DateTime.Now
        End If
    End Sub

    Private Sub Borrowing_FormClosed(sender As Object, e As FormClosedEventArgs) Handles MyBase.FormClosed

        timerSystemDate.Stop()
        timerSystemDate.Dispose()

    End Sub

    Private Sub btnclear_Click(sender As Object, e As EventArgs) Handles btnclear.Click
        clearlahat()
    End Sub

    Private Function acessionstats(accessionID As String) As Boolean

        Try

            Dim con As New MySqlConnection(connectionString)
            Using con
                Dim comm As String = "SELECT Status FROM `acession_tbl` WHERE `AccessionID` = @AccessionID"
                Using cmd As New MySqlCommand(comm, con)
                    cmd.Parameters.AddWithValue("@AccessionID", accessionID)

                    con.Open()
                    Dim status As Object = cmd.ExecuteScalar()
                    If status IsNot DBNull.Value AndAlso status IsNot Nothing Then
                        Return status.ToString().Equals("Available", StringComparison.OrdinalIgnoreCase)
                    Else
                        Return False
                    End If
                End Using
            End Using
        Catch ex As Exception
            MessageBox.Show("Error checking AccessionID availability: " & ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End Try
    End Function

    Private Sub pendingstats(accessionID As String, status As String)
        Try

            Dim con As New MySqlConnection(connectionString)
            Using con
                Dim comm As String = "UPDATE `acession_tbl` SET `Status` = @status WHERE `AccessionID` = @accessionID"

                con.Open()
                Dim cmd As New MySqlCommand(comm, con)
                cmd.Parameters.AddWithValue("@status", status)
                cmd.Parameters.AddWithValue("@accessionID", accessionID)
                cmd.ExecuteNonQuery()

            End Using

            For Each form In Application.OpenForms
                If TypeOf form Is AvailableBooks Then
                    Dim avail = DirectCast(form, AvailableBooks)
                    avail.refreshavail()
                    avail.counts()
                End If
            Next

            If Accession IsNot Nothing AndAlso Not Accession.IsDisposed Then
                Accession.RefreshAccessionData()
            End If

        Catch ex As Exception
            MessageBox.Show("Error updating accession table: " & ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub


    Private Sub btnadd_Click(sender As Object, e As EventArgs) Handles btnadd.Click

        Dim borrower As String = ""
        Dim con As New MySqlConnection(connectionString)
        Dim borrowerIdentifier As String = ""
        Dim identifierType As String = ""


        If rbstudent.Checked Then
            borrower = "Student"
            identifierType = "LRN"
            borrowerIdentifier = txtlrn.Text
        ElseIf rbteacher.Checked Then
            borrower = "Teacher"
            identifierType = "EmployeeNo"
            borrowerIdentifier = txtemployee.Text
        Else
            MsgBox("Please select a borrower type (Student or Teacher).", vbExclamation, "Missing Information")
            Exit Sub
        End If


        If String.IsNullOrWhiteSpace(txtaccessionid.Text) OrElse
    String.IsNullOrWhiteSpace(txtname.Text) Then

            MsgBox("Accession ID and Borrower Name are required.", vbExclamation, "Missing Information")
            Exit Sub
        End If

        If borrower = "Student" And String.IsNullOrWhiteSpace(txtlrn.Text) Then
            MsgBox("LRN is required for Students.", vbExclamation, "Missing Information")
            Exit Sub
        ElseIf borrower = "Teacher" And String.IsNullOrWhiteSpace(txtemployee.Text) Then
            MsgBox("Employee No is required for Teachers.", vbExclamation, "Missing Information")
            Exit Sub
        End If

        Dim isTimedIn As Boolean = False
        If borrower = "Student" Then
            isTimedIn = CheckTimeInStatus(txtlrn.Text, "LRN")
        ElseIf borrower = "Teacher" Then
            isTimedIn = CheckTimeInStatus(txtemployee.Text, "EmployeeNo")
        End If

        If Not isTimedIn Then
            MsgBox("The borrower must be Timed In before borrowing a book.", vbExclamation, "Time-In Required")
            Exit Sub
        End If

        If Not acessionstats(txtaccessionid.Text) Then
            MsgBox("The selected book is not available for borrowing.", vbExclamation, "Book Not Available")
            Exit Sub
        End If


        If Not limitniborrower(borrowerIdentifier, identifierType) Then
            MsgBox($"Borrowing limit reached. This borrower has already borrowed {MAX_BORROWED_BOOKS} books.", vbExclamation, "Borrowing Limit Exceeded")
            Exit Sub
        End If


        Dim booktitleee As String = txtbooktitle.Text.Trim
        Dim identifierValue As String = If(borrower = "Student", txtlrn.Text, txtemployee.Text)
        Dim identifierColumn As String = If(borrower = "Student", "LRN", "EmployeeNo")

        Try
            con.Open()


            Dim checkDuplicateQuery As String = $"SELECT COUNT(*) FROM borrowing_tbl WHERE {identifierColumn} = @IdentifierValue AND AccessionID = @AccessionID"

            Using cmdCheck As New MySqlCommand(checkDuplicateQuery, con)
                cmdCheck.Parameters.AddWithValue("@IdentifierValue", identifierValue)
                cmdCheck.Parameters.AddWithValue("@AccessionID", txtaccessionid.Text)

                Dim count As Integer = Convert.ToInt32(cmdCheck.ExecuteScalar())

                If count > 0 Then
                    MsgBox("You cannot borrow same book with the same accessionID.", vbExclamation, "Duplication is not allowed.")
                    con.Close()
                    Exit Sub
                End If
            End Using


            Dim checkTitleQuery As String = $"SELECT COUNT(*) FROM borrowing_tbl WHERE {identifierColumn} = @IdentifierValue AND BookTitle = @BookTitle"
            Using cmdCheckTitle As New MySqlCommand(checkTitleQuery, con)
                cmdCheckTitle.Parameters.AddWithValue("@IdentifierValue", identifierValue)
                cmdCheckTitle.Parameters.AddWithValue("@BookTitle", booktitleee)

                Dim titleCount As Integer = Convert.ToInt32(cmdCheckTitle.ExecuteScalar())

                If titleCount > 0 Then
                    MsgBox("You cannot borrow same book.", vbExclamation, "Book Title Duplication.")
                    con.Close()
                    Exit Sub
                End If
            End Using


            Dim transactionReceiptID As String = lbltransac.Text

            Dim com As String = "INSERT INTO borrowing_tbl (Borrower, LRN, EmployeeNo, Name, BookTitle, ISBN, Barcode, AccessionID, Shelf, BorrowedDate, DueDate, TransactionReceipt) " &
                            "VALUES (@Borrower, @LRN, @EmpNo, @Name, @Title, @ISBN, @Barcode, @AccessionID, @Shelf, @BDate, @DDate, @TransactionReceipt)"

            Using comsi As New MySqlCommand(com, con)

                comsi.Parameters.AddWithValue("@Borrower", borrower)
                comsi.Parameters.AddWithValue("@LRN", If(String.IsNullOrWhiteSpace(txtlrn.Text), DBNull.Value, txtlrn.Text))
                comsi.Parameters.AddWithValue("@EmpNo", If(String.IsNullOrWhiteSpace(txtemployee.Text), DBNull.Value, txtemployee.Text))
                comsi.Parameters.AddWithValue("@Name", txtname.Text)
                comsi.Parameters.AddWithValue("@Title", txtbooktitle.Text)
                comsi.Parameters.AddWithValue("@ISBN", txtisbn.Text)
                comsi.Parameters.AddWithValue("@Barcode", txtbarcode.Text)
                comsi.Parameters.AddWithValue("@AccessionID", txtaccessionid.Text)
                comsi.Parameters.AddWithValue("@Shelf", txtshelf.Text)
                comsi.Parameters.AddWithValue("@BDate", DateTimePicker1.Value.ToString("MMMM-dd-yyyy"))
                comsi.Parameters.AddWithValue("@DDate", DateTime.Parse(txtduedate.Text).ToString("MMMM-dd-yyyy"))
                comsi.Parameters.AddWithValue("@TransactionReceipt", transactionReceiptID)

                comsi.ExecuteNonQuery()


                Dim accessionID As String = txtaccessionid.Text.ToString

                pendingstats(accessionID, "Pending")

                For Each form In Application.OpenForms
                    If TypeOf form Is MainForm Then
                        Dim mform = DirectCast(form, MainForm)
                        mform.lblborrowcount()
                    End If
                Next


                MsgBox("Borrowing record successfully added.", vbInformation, "Success")


                InsertPrintReceipt(borrower, txtlrn.Text, txtemployee.Text, txtname.Text, txtbooktitle.Text, txtisbn.Text, txtbarcode.Text, txtaccessionid.Text, txtshelf.Text, DateTimePicker1.Value.ToString("MMMM-dd-yyyy"), DateTime.Parse(txtduedate.Text).ToString("MMMM-dd-yyyy"), transactionReceiptID)

                Dim printForm As New PrintReceiptForm()

                printForm.LoadPrintReceiptDataByTransaction(transactionReceiptID)
                UpdateTransactionBarcode()

                refreshborrowingsu()
                clearlahat()

            End Using

        Catch ex As Exception
            MessageBox.Show("Error adding record: " & ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
        End Try

    End Sub
    Private Sub btnedit_Click(sender As Object, e As EventArgs) Handles btnedit.Click

        Dim borrower As String = ""
        Dim con As New MySqlConnection(connectionString)


        If DataGridView1.SelectedRows.Count = 0 Then
            MsgBox("Please select a record from the table to edit.", vbExclamation, "No Record Selected")
            Exit Sub
        End If
        Dim ID As String = DataGridView1.CurrentRow.Cells("ID").Value.ToString()

        Dim oldAccessionID As String = DataGridView1.CurrentRow.Cells("AccessionID").Value.ToString()


        If rbstudent.Checked Then
            borrower = "Student"
        ElseIf rbteacher.Checked Then
            borrower = "Teacher"
        Else
            MsgBox("Please select a borrower type (Student or Teacher).", vbExclamation, "Missing Information")
            Exit Sub
        End If


        If String.IsNullOrWhiteSpace(txtaccessionid.Text) OrElse
        String.IsNullOrWhiteSpace(txtname.Text) Then

            MsgBox("Accession ID and Borrower Name are required.", vbExclamation, "Missing Information")
            Exit Sub
        End If


        If borrower = "Student" And String.IsNullOrWhiteSpace(txtlrn.Text) Then
            MsgBox("LRN is required for Students.", vbExclamation, "Missing Information")
            Exit Sub
        ElseIf borrower = "Teacher" And String.IsNullOrWhiteSpace(txtemployee.Text) Then
            MsgBox("Employee No is required for Teachers.", vbExclamation, "Missing Information")
            Exit Sub
        End If


        Try
            con.Open()

            Dim com As String = "UPDATE borrowing_tbl SET " &
                                "Borrower = @Borrower, LRN = @LRN, EmployeeNo = @EmpNo, Name = @Name, " &
                                "BookTitle = @Title, ISBN = @ISBN, Barcode = @Barcode, AccessionID = @AccessionID, " &
                                "Shelf = @Shelf, BorrowedDate = @BDate, DueDate = @DDate " &
                                "WHERE ID = @ID"

            Using comsi As New MySqlCommand(com, con)

                comsi.Parameters.AddWithValue("@Borrower", borrower)
                comsi.Parameters.AddWithValue("@LRN", If(String.IsNullOrWhiteSpace(txtlrn.Text), DBNull.Value, txtlrn.Text)) ' Gamitin ang DBNull para sa empty fields
                comsi.Parameters.AddWithValue("@EmpNo", If(String.IsNullOrWhiteSpace(txtemployee.Text), DBNull.Value, txtemployee.Text)) ' Gamitin ang DBNull para sa empty fields
                comsi.Parameters.AddWithValue("@Name", txtname.Text)
                comsi.Parameters.AddWithValue("@Title", txtbooktitle.Text)
                comsi.Parameters.AddWithValue("@ISBN", txtisbn.Text)
                comsi.Parameters.AddWithValue("@Barcode", txtbarcode.Text)
                comsi.Parameters.AddWithValue("@AccessionID", txtaccessionid.Text)
                comsi.Parameters.AddWithValue("@Shelf", txtshelf.Text)
                comsi.Parameters.AddWithValue("@BDate", DateTimePicker1.Value.ToString("MMMM-dd-yyyy"))
                comsi.Parameters.AddWithValue("@DDate", DateTime.Parse(txtduedate.Text).ToString("MMMM-dd-yyyy"))
                comsi.Parameters.AddWithValue("@ID", ID)


                comsi.ExecuteNonQuery()


                Dim newAccessionID As String = txtaccessionid.Text.ToString()


                If oldAccessionID <> newAccessionID Then
                    pendingstats(oldAccessionID, "Available")
                End If

                pendingstats(newAccessionID, "Pending")



                MsgBox("Borrowing record successfully updated.", vbInformation, "Success")
                refreshborrowingsu()
                txtaccessionid_TextChanged(txtaccessionid, EventArgs.Empty)
                clearlahat()

            End Using

        Catch ex As Exception
            MessageBox.Show("Error updating record: " & ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
        End Try

    End Sub


    Private Sub DataGridView1_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellClick

        If e.RowIndex >= 0 Then

            isLoadingData = True

            Dim row As DataGridViewRow = DataGridView1.Rows(e.RowIndex)

            txtemployee.Text = If(row.Cells("EmployeeNo").Value Is DBNull.Value, "", row.Cells("EmployeeNo").Value.ToString())
            txtlrn.Text = If(row.Cells("LRN").Value Is DBNull.Value, "", row.Cells("LRN").Value.ToString())

            If row.Cells("Borrower").Value.ToString() = "Student" Then
                rbstudent.Checked = True
            ElseIf row.Cells("Borrower").Value.ToString() = "Teacher" Then
                rbteacher.Checked = True
            End If

            txtname.Text = row.Cells("Name").Value.ToString()
            txtbooktitle.Text = row.Cells("BookTitle").Value.ToString()
            txtisbn.Text = row.Cells("ISBN").Value.ToString()
            txtbarcode.Text = row.Cells("Barcode").Value.ToString()

            txtaccessionid.Text = row.Cells("AccessionID").Value.ToString()

            txtshelf.Text = row.Cells("Shelf").Value.ToString()
            txtduedate.Text = row.Cells("DueDate").Value.ToString()

            If row.Cells("BorrowedDate").Value IsNot DBNull.Value Then
                If IsDate(row.Cells("BorrowedDate").Value) Then
                    DateTimePicker1.Value = CDate(row.Cells("BorrowedDate").Value)
                End If
            End If

            btntimein.Visible = False

            isLoadingData = False

            If rbstudent.Checked Then
                txtlrn_TextChanged(txtlrn, EventArgs.Empty)
            ElseIf rbteacher.Checked Then
                txtemployee_TextChanged(txtemployee, EventArgs.Empty)
            End If


        End If

    End Sub


    Private Sub txtemployee_KeyDown(sender As Object, e As KeyEventArgs) Handles txtemployee.KeyDown

        If e.Control AndAlso (e.KeyCode = Keys.V Or e.KeyCode = Keys.C Or e.KeyCode = Keys.X) Then
            e.SuppressKeyPress = True
        End If

    End Sub

    Private Sub txtemployee_KeyPress(sender As Object, e As KeyPressEventArgs) Handles txtemployee.KeyPress


        If Not Char.IsDigit(e.KeyChar) And Not Char.IsControl(e.KeyChar) Then
            e.Handled = True
        End If

    End Sub

    Private Sub txtlrn_KeyDown(sender As Object, e As KeyEventArgs) Handles txtlrn.KeyDown

        If e.Control AndAlso (e.KeyCode = Keys.V Or e.KeyCode = Keys.C Or e.KeyCode = Keys.X) Then
            e.SuppressKeyPress = True
        End If

    End Sub

    Private Sub txtlrn_KeyPress(sender As Object, e As KeyPressEventArgs) Handles txtlrn.KeyPress


        If Not Char.IsDigit(e.KeyChar) And Not Char.IsControl(e.KeyChar) Then
            e.Handled = True
        End If

    End Sub

    Private Sub txtaccessionid_KeyDown(sender As Object, e As KeyEventArgs) Handles txtaccessionid.KeyDown

        If e.Control AndAlso (e.KeyCode = Keys.V Or e.KeyCode = Keys.C Or e.KeyCode = Keys.X) Then
            e.SuppressKeyPress = True
        End If

    End Sub

    Private Sub txtaccessionid_KeyPress(sender As Object, e As KeyPressEventArgs) Handles txtaccessionid.KeyPress


        If Not Char.IsDigit(e.KeyChar) And Not Char.IsControl(e.KeyChar) Then
            e.Handled = True
        End If

    End Sub

    Private Sub btntimein_Click(sender As Object, e As EventArgs) Handles btntimein.Click

        Dim borrowerID As String = ""
        Dim borrowerType As String = ""
        Dim borrowerName As String = ""
        Dim borrowertayp As String = ""


        If rbstudent.Checked AndAlso Not String.IsNullOrWhiteSpace(txtlrn.Text) Then
            borrowerID = txtlrn.Text
            borrowerType = "LRN"
        ElseIf rbteacher.Checked AndAlso Not String.IsNullOrWhiteSpace(txtemployee.Text) Then
            borrowerID = txtemployee.Text
            borrowerType = "EmployeeNo"
        Else
            MessageBox.Show("Please enter the LRN or Employee Number first.", "Required Input", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        End If


        RegisteredBrwr.IsTimeInMode = True

        RegisteredBrwr.SetTimeInFilter(borrowerID, borrowerType, borrowerName, borrowertayp)

        AddHandler RegisteredBrwr.ListView1.MouseDoubleClick, AddressOf RegisteredBrwr.ListView1_MouseDoubleClick

        RegisteredBrwr.lbl_action.ForeColor = Color.Red
        RegisteredBrwr.lbl_action.Text = "Selecting"

        RegisteredBrwr.ListView1.Enabled = True

        RegisteredBrwr.ShowDialog()

        If rbstudent.Checked AndAlso Not String.IsNullOrWhiteSpace(txtlrn.Text) Then
            txtlrn_TextChanged(txtlrn, EventArgs.Empty)
        ElseIf rbteacher.Checked AndAlso Not String.IsNullOrWhiteSpace(txtemployee.Text) Then
            txtemployee_TextChanged(txtemployee, EventArgs.Empty)
        End If

    End Sub

    Private Sub btnview_Click(sender As Object, e As EventArgs) Handles btnview.Click

        AvailableBooks.ShowDialog()

        For Each form In Application.OpenForms
            If TypeOf form Is AvailableBooks Then
                Dim avail = DirectCast(form, AvailableBooks)
                avail.refreshavail()
                avail.counts()
            End If
        Next
    End Sub

    Public Sub SetupBorrowerFields()

        Dim borrowerType As String = GlobalVarsModule.CurrentBorrowerType

        rbstudent.Enabled = True
        rbteacher.Enabled = True

        If GlobalVarsModule.CurrentUserRole = "Borrower" AndAlso Not String.IsNullOrEmpty(borrowerType) Then



            If borrowerType = "Student" Then

                lblemployee.Visible = False
                txtemployee.Visible = False
                rbteacher.Visible = False


                lbllrn.Visible = True
                txtlrn.Visible = True
                rbstudent.Visible = True

                lbllrn.Location = New Point(30, 78)
                txtlrn.Location = New Point(30, 97)
                rbstudent.Location = New Point(121, 8)


                rbstudent.Checked = True
                rbstudent.Enabled = False

            ElseIf borrowerType = "Teacher" Then

                lbllrn.Visible = False
                txtlrn.Visible = False
                rbstudent.Visible = False


                lblemployee.Visible = True
                txtemployee.Visible = True
                rbteacher.Visible = True


                lblemployee.Location = New Point(30, 78)
                txtemployee.Location = New Point(30, 97)
                rbteacher.Location = New Point(121, 8)


                rbteacher.Checked = True
                rbteacher.Enabled = False

            End If

        Else


            lblemployee.Visible = True
            txtemployee.Visible = True
            rbteacher.Visible = True
            lbllrn.Visible = True
            txtlrn.Visible = True
            rbstudent.Visible = True

            rbstudent.Checked = True

        End If


    End Sub



    Private Sub InsertPrintReceipt(borrower As String, lrn As String, empNo As String, name As String, bookTitle As String, isbn As String, barcode As String, accessionID As String, shelf As String, borrowedDate As String, dueDate As String, transactionReceipt As String)
        Try
            Dim con As New MySqlConnection(connectionString)
            con.Open()

            Dim com As String = "INSERT INTO printreceipt_tbl (Borrower, LRN, EmployeeNo, Name, BookTitle, ISBN, Barcode, AccessionID, Shelf, BorrowedDate, DueDate, TransactionReceipt) " &
                            "VALUES (@Borrower, @LRN, @EmpNo, @Name, @Title, @ISBN, @Barcode, @AccessionID, @Shelf, @BDate, @DDate, @TransactionReceipt)"

            Using comsi As New MySqlCommand(com, con)
                comsi.Parameters.AddWithValue("@Borrower", borrower)
                comsi.Parameters.AddWithValue("@LRN", If(String.IsNullOrWhiteSpace(lrn), DBNull.Value, lrn))
                comsi.Parameters.AddWithValue("@EmpNo", If(String.IsNullOrWhiteSpace(empNo), DBNull.Value, empNo))
                comsi.Parameters.AddWithValue("@Name", name)
                comsi.Parameters.AddWithValue("@Title", bookTitle)
                comsi.Parameters.AddWithValue("@ISBN", If(String.IsNullOrWhiteSpace(isbn), DBNull.Value, isbn))
                comsi.Parameters.AddWithValue("@Barcode", If(String.IsNullOrWhiteSpace(barcode), DBNull.Value, barcode))
                comsi.Parameters.AddWithValue("@AccessionID", accessionID)
                comsi.Parameters.AddWithValue("@Shelf", shelf)
                comsi.Parameters.AddWithValue("@BDate", borrowedDate)
                comsi.Parameters.AddWithValue("@DDate", dueDate)
                comsi.Parameters.AddWithValue("@TransactionReceipt", transactionReceipt)

                comsi.ExecuteNonQuery()

            End Using

        Catch ex As Exception
            MessageBox.Show("Error inserting into printreceipt_tbl: " & ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
    Private Sub btnadd_MouseHover(sender As Object, e As EventArgs) Handles btnadd.MouseHover
        Cursor = Cursors.Hand
    End Sub

    Private Sub btnadd_MouseLeave(sender As Object, e As EventArgs) Handles btnadd.MouseLeave
        Cursor = Cursors.Default
    End Sub

    Private Sub btnedit_MouseHover(sender As Object, e As EventArgs) Handles btnedit.MouseHover
        Cursor = Cursors.Hand
    End Sub

    Private Sub btnedit_MouseLeave(sender As Object, e As EventArgs) Handles btnedit.MouseLeave
        Cursor = Cursors.Default
    End Sub


    Private Sub btnclear_MouseHover(sender As Object, e As EventArgs) Handles btnclear.MouseHover
        Cursor = Cursors.Hand
    End Sub

    Private Sub btnclear_MouseLeave(sender As Object, e As EventArgs) Handles btnclear.MouseLeave
        Cursor = Cursors.Default
    End Sub

    Private Sub btntimein_MouseHover(sender As Object, e As EventArgs) Handles btntimein.MouseHover
        Cursor = Cursors.Hand
    End Sub

    Private Sub btntimein_MouseLeave(sender As Object, e As EventArgs) Handles btntimein.MouseLeave
        Cursor = Cursors.Default
    End Sub

    Private Sub btnview_MouseHover(sender As Object, e As EventArgs) Handles btnview.MouseHover
        Cursor = Cursors.Hand
    End Sub

    Private Sub btnview_MouseLeave(sender As Object, e As EventArgs) Handles btnview.MouseLeave
        Cursor = Cursors.Default
    End Sub
End Class