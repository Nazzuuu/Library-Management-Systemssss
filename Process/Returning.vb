Imports System.Data
Imports MySql.Data.MySqlClient

Public Class Returning


    Private Sub RefreshReturningData()
        Dim con As New MySqlConnection(GlobalVarsModule.connectionString)
        Dim com As String = "SELECT * FROM `returning_tbl` ORDER BY ID DESC"
        Dim adap As New MySqlDataAdapter(com, con)
        Dim ds As New DataSet

        Try
            adap.Fill(ds, "info")
            DataGridView1.DataSource = ds.Tables("info")
        Catch ex As Exception
            MessageBox.Show("Error loading Returning data: " & ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

        DataGridView1.Columns("ID").Visible = False
        DataGridView1.EnableHeadersVisualStyles = False
        DataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(207, 58, 109)
        DataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White

    End Sub


    Private Sub clear_details_only()

        lblborrowertype.Text = ""
        lbllrn.Text = ""
        lblemployeeno.Text = ""
        lblfullname.Text = ""
        lblbooktotal.Text = ""
        lblborroweddate.Text = ""
        lblduedate.Text = ""
        lblisbnbarcode.Text = ""
        lblaccessionid.Text = ""
        lblgrade.Text = ""
        lblsection.Text = ""
        lblstrand.Text = ""
        lbldepartment.Text = ""
        cbbooks.Text = ""


        cbbooks.Items.Clear()
        chkSelectAll.Checked = False
        cbbooks.Enabled = True


        rboverdue.Checked = False
        rbdamage.Checked = False
        rblost.Checked = False
        lbldamage.Visible = False
        cbdamage.Visible = False
        cbdamage.SelectedIndex = -1
    End Sub


    Private Sub InitializeConditionControls()
        lbldamage.Visible = False
        cbdamage.Visible = False

        If cbdamage.Items.Count = 0 Then
            cbdamage.Items.Add("Minor")
            cbdamage.Items.Add("Major")
            cbdamage.Items.Add("Irreparable")
        End If
        cbdamage.SelectedIndex = -1
    End Sub

    Private Sub returning_shown(sender As Object, e As EventArgs) Handles MyBase.Shown
        DataGridView1.ClearSelection()
        InitializeConditionControls()
    End Sub

    Private Sub Returning_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        RefreshReturningData()
        InitializeConditionControls()

        chkSelectAll.Checked = False
        cbbooks.Enabled = True

        rboverdue.Checked = False
        rbdamage.Checked = False
        rblost.Checked = False
    End Sub


    Private Sub rbdamage_CheckedChanged(sender As Object, e As EventArgs) Handles rbdamage.CheckedChanged
        If rbdamage.Checked Then
            lbldamage.Visible = True
            cbdamage.Visible = True
        Else
            lbldamage.Visible = False
            cbdamage.Visible = False
            cbdamage.SelectedIndex = -1
        End If
    End Sub

    Private Sub rboverdue_CheckedChanged(sender As Object, e As EventArgs) Handles rboverdue.CheckedChanged
        If rboverdue.Checked Then
            rbdamage.Checked = False
        End If
    End Sub

    Private Sub rblost_CheckedChanged(sender As Object, e As EventArgs) Handles rblost.CheckedChanged
        If rblost.Checked Then
            rbdamage.Checked = False
        End If
    End Sub



    Private Sub chkSelectAll_CheckedChanged(sender As Object, e As EventArgs) Handles chkSelectAll.CheckedChanged
        cbbooks.Enabled = Not chkSelectAll.Checked

        If chkSelectAll.Checked Then
            cbbooks.SelectedIndex = -1
            lblisbnbarcode.Text = "All books are selected will be returned."
            lblaccessionid.Text = ""
            lbldetails.Visible = False
        Else
            lblisbnbarcode.Text = ""
            lblaccessionid.Text = ""
            lbldetails.Visible = True
        End If
    End Sub

    Private Sub btnreturn_Click(sender As Object, e As EventArgs) Handles btnreturn.Click

        Dim TransactionNo As String = txttransactionreceipt.Text.Trim()
        Dim booksToReturn As New List(Of String)
        Dim bookStatus As String = "Normal"
        Dim newAccessionStatus As String = "Available"


        If rboverdue.Checked Then
            bookStatus = "Overdue"
            newAccessionStatus = "Available"
        ElseIf rbdamage.Checked Then
            If cbdamage.SelectedItem Is Nothing Then
                MessageBox.Show("Please select the level of damage (Minor, Major, Irreparable).", "Damage Required", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If
            bookStatus = "Damaged (" & cbdamage.SelectedItem.ToString() & ")"
            newAccessionStatus = "Damaged"
        ElseIf rblost.Checked Then
            bookStatus = "Lost"
            newAccessionStatus = "Lost"
        Else
            bookStatus = "Normal"
            newAccessionStatus = "Available"
        End If


        If String.IsNullOrWhiteSpace(lblfullname.Text) OrElse cbbooks.Items.Count = 0 Then
            MessageBox.Show("Please load a valid transaction receipt first.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        If chkSelectAll.Checked Then
            For Each item As Object In cbbooks.Items
                booksToReturn.Add(item.ToString())
            Next
        ElseIf cbbooks.SelectedItem IsNot Nothing Then
            booksToReturn.Add(cbbooks.SelectedItem.ToString())
        Else
            MessageBox.Show("Please select a book or check 'All' to return.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Dim con As New MySqlConnection(GlobalVarsModule.connectionString)
        Dim trans As MySqlTransaction = Nothing

        Try
            con.Open()
            trans = con.BeginTransaction()

            Dim returnedBookTitles As String = String.Join(" | ", booksToReturn)

            Dim insert_com As String = "INSERT INTO `returning_tbl` (`Borrower`, `LRN`, `EmployeeNo`, `FullName`, `ReturnedBook`, `BookTotal`, `BorrowedDate`, `DueDate`, `ReturnDate`, `TransactionReceipt`, `Status`) " &
                  "VALUES (@borrowerType, @lrn, @empNo, @fullName, @returnedBook, @bookTotal, @borrowDate, @dueDate, @returnDate, @transNo, @bookStatus)"

            Using insert_cmd As New MySqlCommand(insert_com, con, trans)
                insert_cmd.Parameters.AddWithValue("@borrowerType", lblborrowertype.Text)
                insert_cmd.Parameters.AddWithValue("@lrn", If(String.IsNullOrWhiteSpace(lbllrn.Text), Nothing, lbllrn.Text))
                insert_cmd.Parameters.AddWithValue("@empNo", If(String.IsNullOrWhiteSpace(lblemployeeno.Text), Nothing, lblemployeeno.Text))
                insert_cmd.Parameters.AddWithValue("@fullName", lblfullname.Text)
                insert_cmd.Parameters.AddWithValue("@returnedBook", returnedBookTitles)
                insert_cmd.Parameters.AddWithValue("@bookTotal", booksToReturn.Count)
                insert_cmd.Parameters.AddWithValue("@borrowDate", lblborroweddate.Text)
                insert_cmd.Parameters.AddWithValue("@dueDate", lblduedate.Text)
                insert_cmd.Parameters.AddWithValue("@returnDate", DateTime.Now.ToShortDateString())
                insert_cmd.Parameters.AddWithValue("@transNo", TransactionNo)
                insert_cmd.Parameters.AddWithValue("@bookStatus", bookStatus)

                insert_cmd.ExecuteNonQuery()
            End Using


            For Each bookTitle As String In booksToReturn

                Dim get_accession_com As String = "SELECT `AccessionID` FROM `borrowing_tbl` WHERE `TransactionReceipt` = @transNo AND `BookTitle` = @bookTitle LIMIT 1"
                Dim accessionID As String = String.Empty

                Using get_accession_cmd As New MySqlCommand(get_accession_com, con, trans)
                    get_accession_cmd.Parameters.AddWithValue("@transNo", TransactionNo)
                    get_accession_cmd.Parameters.AddWithValue("@bookTitle", bookTitle)

                    Dim result As Object = get_accession_cmd.ExecuteScalar()
                    If result IsNot Nothing AndAlso result IsNot DBNull.Value Then
                        accessionID = result.ToString()
                    End If
                End Using

                If Not String.IsNullOrWhiteSpace(accessionID) Then


                    Dim update_accession_com As String = "UPDATE `acession_tbl` SET `Status` = @newStatus WHERE `AccessionID` = @accessionId"
                    Using update_accession_cmd As New MySqlCommand(update_accession_com, con, trans)
                        update_accession_cmd.Parameters.AddWithValue("@newStatus", newAccessionStatus)
                        update_accession_cmd.Parameters.AddWithValue("@accessionId", accessionID)
                        update_accession_cmd.ExecuteNonQuery()
                    End Using
                End If



                Dim delete_com As String = "DELETE FROM `borrowing_tbl` WHERE `TransactionReceipt` = @transNo AND `BookTitle` = @bookTitle LIMIT 1"
                Using delete_cmd As New MySqlCommand(delete_com, con, trans)
                    delete_cmd.Parameters.AddWithValue("@transNo", TransactionNo)
                    delete_cmd.Parameters.AddWithValue("@bookTitle", bookTitle)
                    delete_cmd.ExecuteNonQuery()
                End Using
            Next

            trans.Commit()

            For Each form In Application.OpenForms

                If TypeOf form Is Accession Then
                    DirectCast(form, Accession).RefreshAccessionData()
                End If

            Next

            For Each form In Application.OpenForms
                If TypeOf form Is MainForm Then
                    Dim mform = DirectCast(form, MainForm)
                    mform.lblborrowcount()
                End If
            Next

            MessageBox.Show($"{booksToReturn.Count} book(s) returned successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)


            clear_details_only()
            clear_click(sender, e)

            Dim tempTransNo As String = TransactionNo
            txttransactionreceipt.Text = tempTransNo

            RefreshReturningData()

        Catch ex As Exception
            trans?.Rollback()
            MessageBox.Show("Error during book return: " & ex.Message, "Database Error: " & ex.Message, MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
        End Try
    End Sub


    Private Sub clear_click(sender As Object, e As EventArgs) Handles btnclear.Click
        txttransactionreceipt.Clear()
        clear_details_only()
    End Sub

    Private Sub LoadBorrowerDetails(ByVal borrowerName As String, ByVal borrowerType As String)

        lblgrade.Text = ""
        lbldepartment.Text = ""
        lblsection.Text = ""
        lblstrand.Text = ""
        lbllrn.Text = ""
        lblemployeeno.Text = ""

        Dim con As New MySqlConnection(GlobalVarsModule.connectionString)
        Dim com As String = String.Empty

        If borrowerType.ToLower() = "student" Then
            com = "SELECT LRN, Grade, Section, Strand FROM `borrower_tbl` " &
               "WHERE CONCAT(`FirstName`, ' ', `LastName`) LIKE @name " &
               "AND `Borrower` = 'Student' LIMIT 1"
        ElseIf borrowerType.ToLower() = "teacher" OrElse borrowerType.ToLower() = "employee" OrElse borrowerType.ToLower() = "faculty" Then
            com = "SELECT EmployeeNo, Department FROM `borrower_tbl` " &
               "WHERE CONCAT(`FirstName`, ' ', `LastName`) LIKE @name " &
               "AND (`Borrower` = 'Teacher' OR `Borrower` = 'Employee') LIMIT 1"
        Else
            Return
        End If

        Try
            con.Open()
            Using cmd As New MySqlCommand(com, con)
                cmd.Parameters.AddWithValue("@name", borrowerName)

                Using reader As MySqlDataReader = cmd.ExecuteReader()
                    If reader.Read() Then
                        If borrowerType.ToLower() = "student" Then
                            lbllrn.Text = reader("LRN").ToString()
                            lblgrade.Text = reader("Grade").ToString()
                            lblsection.Text = reader("Section").ToString()
                            lblstrand.Text = reader("Strand").ToString()
                            lblemployeeno.Text = ""
                            lbldepartment.Text = ""
                        ElseIf borrowerType.ToLower() = "teacher" OrElse borrowerType.ToLower() = "employee" OrElse borrowerType.ToLower() = "faculty" Then
                            lblemployeeno.Text = reader("EmployeeNo").ToString()
                            lbldepartment.Text = reader("Department").ToString()
                            lbllrn.Text = ""
                            lblgrade.Text = ""
                            lblsection.Text = ""
                            lblstrand.Text = ""
                        End If
                    Else
                        lbllrn.Text = "N/A"
                        lblemployeeno.Text = "N/A"
                    End If
                End Using
            End Using

        Catch ex As Exception
            MessageBox.Show("Error loading borrower details from borrower_tbl: " & ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
        End Try
    End Sub

    Private Sub txttransactionreceipt_TextChanged(sender As Object, e As EventArgs) Handles txttransactionreceipt.TextChanged

        Const MIN_LENGTH As Integer = 11

        clear_details_only()

        Dim TransactionNo As String = txttransactionreceipt.Text.Trim()

        If String.IsNullOrWhiteSpace(TransactionNo) OrElse TransactionNo.Length < MIN_LENGTH Then
            Return
        End If

        Dim con As New MySqlConnection(GlobalVarsModule.connectionString)

        Try
            con.Open()


            Dim com_header As String = "SELECT `Borrower`, `Name`, `BorrowedDate`, `DueDate`, `BorrowedBookCount` " &
                         "FROM `printreceipt_tbl` " &
                         "WHERE `TransactionReceipt` = @transNo LIMIT 1"

            Using comsi_header As New MySqlCommand(com_header, con)
                comsi_header.Parameters.AddWithValue("@transNo", TransactionNo)

                Using reader_header As MySqlDataReader = comsi_header.ExecuteReader()
                    If reader_header.Read() Then
                        Dim borrowerName As String = reader_header("Name").ToString()
                        Dim borrowerType As String = reader_header("Borrower").ToString()

                        lblfullname.Text = borrowerName
                        lblborrowertype.Text = borrowerType
                        lblborroweddate.Text = Convert.ToDateTime(reader_header("BorrowedDate")).ToShortDateString()
                        lblduedate.Text = Convert.ToDateTime(reader_header("DueDate")).ToShortDateString()
                        lblbooktotal.Text = reader_header("BorrowedBookCount").ToString() & " (Total)"

                        reader_header.Close()
                        LoadBorrowerDetails(borrowerName, borrowerType)

                    Else
                        reader_header.Close()
                        Return
                    End If
                End Using
            End Using


            Dim com_items As String = "SELECT `BookTitle` FROM `borrowing_tbl` " &
                         "WHERE `TransactionReceipt` = @transNo"

            Using comsi_items As New MySqlCommand(com_items, con)
                comsi_items.Parameters.AddWithValue("@transNo", TransactionNo)

                Using reader_items As MySqlDataReader = comsi_items.ExecuteReader()
                    While reader_items.Read()
                        Dim bookTitle As String = reader_items("BookTitle").ToString()
                        cbbooks.Items.Add(bookTitle)
                    End While

                    If cbbooks.Items.Count = 0 Then
                        reader_items.Close()

                        MessageBox.Show("This transaction is complete. All borrowed books have been returned.", "Transaction Complete", MessageBoxButtons.OK, MessageBoxIcon.Information)
                        Return
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


    Private Sub cbbooks_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbbooks.SelectedIndexChanged

        If cbbooks.SelectedItem Is Nothing Then
            lblisbnbarcode.Text = ""
            lblaccessionid.Text = ""
            Return
        End If


        chkSelectAll.Checked = False

        Dim selectedBookTitle As String = cbbooks.SelectedItem.ToString()
        Dim TransactionNo As String = txttransactionreceipt.Text

        Dim con As New MySqlConnection(GlobalVarsModule.connectionString)

        Try
            con.Open()



            Dim com As String = "SELECT `ISBN`, `Barcode`, `AccessionID` FROM `borrowing_tbl` " &
                      "WHERE `TransactionReceipt` = @transNo AND `BookTitle` = @bookTitle LIMIT 1"

            Using comsi As New MySqlCommand(com, con)
                comsi.Parameters.AddWithValue("@transNo", TransactionNo)
                comsi.Parameters.AddWithValue("@bookTitle", selectedBookTitle)

                Using reader As MySqlDataReader = comsi.ExecuteReader()
                    If reader.Read() Then
                        Dim isbn As String = reader("ISBN").ToString()
                        Dim barcode As String = reader("Barcode").ToString()
                        Dim accessionID As String = reader("AccessionID").ToString()

                        lblisbnbarcode.Text = If(String.IsNullOrWhiteSpace(barcode), isbn, barcode)
                        lblaccessionid.Text = accessionID
                    Else
                        lblisbnbarcode.Text = "ISBN/Barcode not found for this book."
                        lblaccessionid.Text = "N/A"
                    End If
                End Using

            End Using

        Catch ex As Exception
            MessageBox.Show("Error getting book details: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
        End Try
    End Sub

    Private Sub DataGridView1_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellClick

    End Sub

    Private Sub Returning_KeyDown(sender As Object, e As KeyEventArgs) Handles MyBase.KeyDown

        If e.KeyCode = Keys.Escape Then
            Me.Close()
        End If

    End Sub
End Class