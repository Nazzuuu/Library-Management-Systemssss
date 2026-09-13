Imports MySql.Data.MySqlClient
Imports System.Data

Public Class ChooseBrwr

    Public Property SelectedID As Integer = -1
    Public Property SelectedBorrower As String = ""
    Public Property SelectedFirstName As String = ""
    Public Property SelectedLastName As String = ""
    Public Property SelectedMiddleInitial As String = ""
    Public Property SelectedLRN As String = ""
    Public Property SelectedEmployeeNo As String = ""
    Public Property SelectedContact As String = ""
    Public Property SelectedDepartment As String = ""
    Public Property SelectedGrade As String = ""
    Public Property SelectedSection As String = ""
    Public Property SelectedStrand As String = ""
    Public Property SelectedPosition As String = ""

    Private connectionString As String = GlobalVarsModule.connectionString
    Private dt As DataTable

    Public Sub LoadBorrowers(borrowerType As String)
        dt = New DataTable()

        Dim query As String = "SELECT ID, Borrower, LRN, EmployeeNo, FirstName, LastName, MiddleInitial, ContactNumber, Department, Grade, Section, Strand FROM borrower_tbl WHERE Borrower = @type ORDER BY LastName, FirstName"

        Using con As New MySqlConnection(connectionString)
            Try
                con.Open()
                Using cmd As New MySqlCommand(query, con)
                    cmd.Parameters.AddWithValue("@type", borrowerType)
                    Using adap As New MySqlDataAdapter(cmd)
                        adap.Fill(dt)
                    End Using
                End Using
            Catch ex As Exception
                MessageBox.Show("Error loading borrowers: " & ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End Using

        DataGridView1.DataSource = dt
        If DataGridView1.Columns.Contains("ID") Then DataGridView1.Columns("ID").Visible = False
        DataGridView1.ClearSelection()
    End Sub

    Private Sub txtsearch_TextChanged(sender As Object, e As EventArgs) Handles txtsearch.TextChanged
        If dt Is Nothing Then Exit Sub
        Dim filter As String = txtsearch.Text.Trim().Replace("'", "''")
        If filter = "" Then
            dt.DefaultView.RowFilter = ""
        Else
            dt.DefaultView.RowFilter = String.Format("Borrower LIKE '%{0}%' OR FirstName LIKE '%{0}%' OR LastName LIKE '%{0}%' OR LRN LIKE '%{0}%' OR EmployeeNo LIKE '%{0}%'", filter)
        End If
    End Sub

    Private Sub DataGridView1_CellDoubleClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellDoubleClick
        If e.RowIndex >= 0 Then
            Dim row As DataGridViewRow = DataGridView1.Rows(e.RowIndex)
            Try
                SelectedID = If(IsNumeric(row.Cells("ID").Value), CInt(row.Cells("ID").Value), -1)
            Catch
                SelectedID = -1
            End Try

            SelectedBorrower = If(row.Cells("Borrower").Value IsNot Nothing, row.Cells("Borrower").Value.ToString(), "")
            SelectedFirstName = If(row.Cells("FirstName").Value IsNot Nothing, row.Cells("FirstName").Value.ToString(), "")
            SelectedLastName = If(row.Cells("LastName").Value IsNot Nothing, row.Cells("LastName").Value.ToString(), "")
            SelectedMiddleInitial = If(row.Cells("MiddleInitial").Value IsNot Nothing, row.Cells("MiddleInitial").Value.ToString(), "")
            SelectedLRN = If(row.Cells("LRN").Value IsNot Nothing, row.Cells("LRN").Value.ToString(), "")
            SelectedEmployeeNo = If(row.Cells("EmployeeNo").Value IsNot Nothing, row.Cells("EmployeeNo").Value.ToString(), "")
            SelectedContact = If(row.Cells("ContactNumber").Value IsNot Nothing, row.Cells("ContactNumber").Value.ToString(), "")
            SelectedDepartment = If(row.Cells("Department").Value IsNot Nothing, row.Cells("Department").Value.ToString(), "")
            SelectedGrade = If(row.Cells("Grade").Value IsNot Nothing, row.Cells("Grade").Value.ToString(), "")
            SelectedSection = If(row.Cells("Section").Value IsNot Nothing, row.Cells("Section").Value.ToString(), "")
            SelectedStrand = If(row.Cells("Strand").Value IsNot Nothing, row.Cells("Strand").Value.ToString(), "")

            SelectedPosition = ""

            Me.DialogResult = DialogResult.OK
            Me.Close()
        End If
    End Sub

    Private Sub ChooseBrwr_KeyDown(sender As Object, e As KeyEventArgs) Handles Me.KeyDown
        If e.KeyCode = Keys.Escape Then
            Me.DialogResult = DialogResult.Cancel
            Me.Close()
        End If
    End Sub

End Class
