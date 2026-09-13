Imports MySql.Data.MySqlClient

Public Class TimeInOutScan

    Dim conn As New MySqlConnection(GlobalVarsModule.connectionString)
    Dim scanTimer As New Timer()

    Private Sub TimeInOutScan_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        cbaction.SelectedIndex = 0

        scanTimer.Interval = 1000
        AddHandler scanTimer.Tick, AddressOf ScanFinishedTyping
    End Sub

    Private Sub txtScan_KeyDown(sender As Object, e As KeyEventArgs) Handles txtidtype.KeyDown
        If e.KeyCode = Keys.Enter Then
            e.SuppressKeyPress = True
            ProcessScan(txtidtype.Text.Trim())
        End If
    End Sub

    Private Sub txtidtype_TextChanged(sender As Object, e As EventArgs) Handles txtidtype.TextChanged
        Dim input As String = txtidtype.Text.Trim()

        If input.Length = 12 Then
            scanTimer.Stop()
            ProcessScan(input)
            Return
        End If

        If input.Length > 0 Then
            scanTimer.Stop()
            scanTimer.Start()
        Else
            scanTimer.Stop()
        End If

    End Sub

    Private Sub ScanFinishedTyping(sender As Object, e As EventArgs)
        scanTimer.Stop()

        Dim input As String = txtidtype.Text.Trim()

        If input = "" Then Exit Sub

        If input.Length = 12 OrElse input.Length = 8 Then
            ProcessScan(input)
        End If

    End Sub

    Private Sub ProcessScan(scanValue As String)

        If scanValue = "" Then Exit Sub

        Dim isStudent As Boolean = False

        Try
            conn.Open()


            Dim query As String = "SELECT * FROM borrower_tbl WHERE LRN = @id OR EmployeeNo = @id"

            Dim cmd As New MySqlCommand(query, conn)
            cmd.Parameters.AddWithValue("@id", scanValue)

            Dim reader As MySqlDataReader = cmd.ExecuteReader()

            If Not reader.HasRows Then
                reader.Close()


                If scanValue.Length <> 12 AndAlso scanValue.Length <> 8 Then
                    MessageBox.Show("Invalid ID format!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Else
                    MessageBox.Show("Borrower not registered!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                End If

                Exit Sub
            End If

            reader.Read()

            Dim lrnDB = reader("LRN").ToString()
            Dim empDB = reader("EmployeeNo").ToString()

            If scanValue = lrnDB Then
                isStudent = True
            Else
                isStudent = False
            End If

            Dim borrowerType = reader("Borrower").ToString()
            Dim firstName = reader("FirstName").ToString()
            Dim lastName = reader("LastName").ToString()
            Dim middleInitial = reader("MiddleInitial").ToString()
            Dim lrn = reader("LRN").ToString()
            Dim employeeNo = reader("EmployeeNo").ToString()
            Dim contact = reader("ContactNumber").ToString()
            Dim department = reader("Department").ToString()
            Dim grade = reader("Grade").ToString()
            Dim section = reader("Section").ToString()
            Dim strand = reader("Strand").ToString()

            reader.Close()

            Dim action As String = cbaction.Text

            If action = "Time-In" Then
                TimeIn(scanValue, isStudent, borrowerType, firstName, lastName, middleInitial, lrn, employeeNo, contact, department, grade, section, strand)

            ElseIf action = "Time-Out" Then
                TimeOut(scanValue, isStudent)

            Else
                MessageBox.Show("Please select action!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            End If

        Catch ex As Exception
            MessageBox.Show(ex.Message)
        Finally
            conn.Close()
            txtidtype.Clear()
        End Try

    End Sub

    Private Sub TimeIn(scanValue As String, isStudent As Boolean,
                       borrowerType As String, firstName As String, lastName As String,
                       middleInitial As String, lrn As String, employeeNo As String,
                       contact As String, department As String, grade As String,
                       section As String, strand As String)

        Dim checkQuery As String

        If isStudent Then
            checkQuery = "SELECT * FROM oras_tbl WHERE LRN=@id AND TimeOut IS NULL AND DATE(TimeIn)=CURDATE()"
        Else
            checkQuery = "SELECT * FROM oras_tbl WHERE EmployeeNo=@id AND TimeOut IS NULL AND DATE(TimeIn)=CURDATE()"
        End If

        Dim checkCmd As New MySqlCommand(checkQuery, conn)
        checkCmd.Parameters.AddWithValue("@id", scanValue)

        Dim reader As MySqlDataReader = checkCmd.ExecuteReader()

        If reader.HasRows Then
            reader.Close()
            MessageBox.Show("Already timed in!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        End If

        reader.Close()

        Dim insertQuery As String = "INSERT INTO oras_tbl 
        (Borrower, FirstName, LastName, MiddleInitial, LRN, EmployeeNo, Contact, Department, GradeLevel, Section, Strand, TimeIn) 
        VALUES 
        (@Borrower, @FirstName, @LastName, @MiddleInitial, @LRN, @EmployeeNo, @ContactNumber, @Department, @Grade, @Section, @Strand, NOW())"

        Dim insertCmd As New MySqlCommand(insertQuery, conn)

        insertCmd.Parameters.AddWithValue("@Borrower", borrowerType)
        insertCmd.Parameters.AddWithValue("@FirstName", firstName)
        insertCmd.Parameters.AddWithValue("@LastName", lastName)
        insertCmd.Parameters.AddWithValue("@MiddleInitial", middleInitial)
        insertCmd.Parameters.AddWithValue("@LRN", lrn)
        insertCmd.Parameters.AddWithValue("@EmployeeNo", employeeNo)
        insertCmd.Parameters.AddWithValue("@ContactNumber", contact)
        insertCmd.Parameters.AddWithValue("@Department", department)
        insertCmd.Parameters.AddWithValue("@Grade", grade)
        insertCmd.Parameters.AddWithValue("@Section", section)
        insertCmd.Parameters.AddWithValue("@Strand", strand)

        insertCmd.ExecuteNonQuery()

        MessageBox.Show("Time-In recorded!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)

    End Sub

    Private Sub TimeOut(scanValue As String, isStudent As Boolean)

        Dim checkQuery As String

        If isStudent Then
            checkQuery = "SELECT * FROM oras_tbl WHERE LRN=@id AND TimeOut IS NULL AND DATE(TimeIn)=CURDATE()"
        Else
            checkQuery = "SELECT * FROM oras_tbl WHERE EmployeeNo=@id AND TimeOut IS NULL AND DATE(TimeIn)=CURDATE()"
        End If

        Dim checkCmd As New MySqlCommand(checkQuery, conn)
        checkCmd.Parameters.AddWithValue("@id", scanValue)

        Dim reader As MySqlDataReader = checkCmd.ExecuteReader()

        If Not reader.HasRows Then
            reader.Close()
            MessageBox.Show("No Time-In record found!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        End If

        reader.Read()
        Dim recordID As String = reader("ID").ToString()
        reader.Close()

        Dim updateQuery As String = "UPDATE oras_tbl SET TimeOut = NOW() WHERE ID=@rid"

        Dim updateCmd As New MySqlCommand(updateQuery, conn)
        updateCmd.Parameters.AddWithValue("@rid", recordID)
        updateCmd.ExecuteNonQuery()

        MessageBox.Show("Time-Out recorded!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)

    End Sub

    Private Sub TimeOutClosing(sender As Object, e As EventArgs) Handles MyBase.FormClosing

        If Me.DialogResult <> DialogResult.OK Then
            login.Show()
            Me.Hide()

        End If
    End Sub

End Class