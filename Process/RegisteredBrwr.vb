Imports MySql.Data.MySqlClient
Imports Windows.Win32.System
Public Class RegisteredBrwr
    Public IsInViewMode As Boolean = False
    Private con As New MySqlConnection(connectionString)

    Private Sub RegisteredBrwr_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        ludeyngborrower()
        ludeyngtimedinborrower()

    End Sub

    Private Sub ListView1_DrawColumnHeader(sender As Object, e As DrawListViewColumnHeaderEventArgs) Handles ListView1.DrawColumnHeader

        Dim colorniheader As New SolidBrush(Color.FromArgb(207, 58, 109))
        e.Graphics.FillRectangle(colorniheader, e.Bounds)

        Dim font As Font = e.Font
        Dim textBrush As New SolidBrush(Color.White)

        Dim stringFormat As New StringFormat()
        stringFormat.Alignment = StringAlignment.Center
        stringFormat.LineAlignment = StringAlignment.Center
        e.Graphics.DrawString(e.Header.Text, font, textBrush, e.Bounds, stringFormat)

        Dim borderPen As New Pen(Color.White, 1)
        e.Graphics.DrawRectangle(borderPen, e.Bounds)

    End Sub

    Private Sub ListView1_DrawSubItem(sender As Object, e As DrawListViewSubItemEventArgs) Handles ListView1.DrawSubItem

        If e.Item.Selected AndAlso e.Item.ListView.Focused Then
            Dim sinelectnarow As New SolidBrush(Color.FromArgb(51, 153, 255))
            e.Graphics.FillRectangle(sinelectnarow, e.Bounds)
        Else
            Dim statusBrush As New SolidBrush(e.Item.BackColor)
            e.Graphics.FillRectangle(statusBrush, e.Bounds)
        End If

        e.DrawText()

    End Sub

    Public Sub ListView1_MouseDoubleClick(sender As Object, e As MouseEventArgs) Handles ListView1.MouseDoubleClick

        If IsInViewMode Then
            Exit Sub
        End If

        Dim selectedItem As ListViewItem = ListView1.GetItemAt(e.X, e.Y)
        If selectedItem IsNot Nothing Then
            Dim borrowerType As String = selectedItem.SubItems(0).Text
            Dim firstName As String = selectedItem.SubItems(1).Text
            Dim lastName As String = selectedItem.SubItems(2).Text
            Dim middleName As String = selectedItem.SubItems(3).Text
            Dim lrn As String = selectedItem.SubItems(4).Text
            Dim employeeNo As String = selectedItem.SubItems(5).Text
            Dim contactNumber As String = selectedItem.SubItems(6).Text
            Dim department As String = selectedItem.SubItems(7).Text
            Dim grade As String = selectedItem.SubItems(8).Text
            Dim section As String = selectedItem.SubItems(9).Text
            Dim strand As String = selectedItem.SubItems(10).Text

            Dim orasForm As oras = Application.OpenForms.OfType(Of oras)().FirstOrDefault()
            If orasForm IsNot Nothing Then
                orasForm.brwrinfo(borrowerType, firstName, lastName, middleName, lrn, employeeNo, contactNumber, department, grade, section, strand)
            End If

            Me.Close()

        End If

    End Sub

    Private Sub RegisteredBrwr_KeyDown(sender As Object, e As KeyEventArgs) Handles MyBase.KeyDown

        If e.KeyCode = Keys.Escape Then
            Me.Close()

        End If
    End Sub

    Public Sub ludeyngborrower()

        ListView1.Items.Clear()
        Dim con As New MySqlConnection(connectionString)
        Dim com As String = "SELECT * FROM `borrower_tbl` ORDER BY `LastName` ASC"

        Try
            con.Open()
            Dim cmd As New MySqlCommand(com, con)
            Dim reader As MySqlDataReader = cmd.ExecuteReader()

            While reader.Read()
                Dim item As New ListViewItem(reader.GetString("Borrower"))
                item.SubItems.Add(reader.GetString("FirstName"))
                item.SubItems.Add(reader.GetString("LastName"))
                item.SubItems.Add(If(reader.IsDBNull(reader.GetOrdinal("MiddleName")), "", reader.GetString("MiddleName")))
                item.SubItems.Add(If(reader.IsDBNull(reader.GetOrdinal("LRN")), "", reader.GetString("LRN")))
                item.SubItems.Add(If(reader.IsDBNull(reader.GetOrdinal("EmployeeNo")), "", reader.GetString("EmployeeNo")))
                item.SubItems.Add(reader.GetString("ContactNumber"))
                item.SubItems.Add(reader.GetString("Department"))
                item.SubItems.Add(If(reader.IsDBNull(reader.GetOrdinal("Grade")), "", reader.GetString("Grade")))
                item.SubItems.Add(If(reader.IsDBNull(reader.GetOrdinal("Section")), "", reader.GetString("Section")))
                item.SubItems.Add(If(reader.IsDBNull(reader.GetOrdinal("Strand")), "", reader.GetString("Strand")))

                ListView1.Items.Add(item)
            End While

            reader.Close()
        Catch ex As Exception
            MessageBox.Show("Error loading borrower data: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            con.Close()
        End Try

    End Sub

    Public Sub ludeyngtimedinborrower()

        If con.State = ConnectionState.Open Then
            con.Close()
        End If

        Dim timedInBorrowers As New List(Of String)
        Dim con2 As New MySqlConnection(connectionString)

        Try
            con2.Open()
            Dim com As String = "SELECT `LRN`, `EmployeeNo` FROM `oras_tbl` WHERE `TimeOut` IS NULL"
            Dim cmd As New MySqlCommand(com, con2)
            Dim reader As MySqlDataReader = cmd.ExecuteReader()

            While reader.Read()

                If Not reader.IsDBNull(reader.GetOrdinal("LRN")) AndAlso Not String.IsNullOrEmpty(reader.GetString("LRN")) Then
                    timedInBorrowers.Add(reader.GetString("LRN"))
                End If
                If Not reader.IsDBNull(reader.GetOrdinal("EmployeeNo")) AndAlso Not String.IsNullOrEmpty(reader.GetString("EmployeeNo")) Then
                    timedInBorrowers.Add(reader.GetString("EmployeeNo"))
                End If
            End While
            reader.Close()

            For Each item As ListViewItem In ListView1.Items
                Dim lrnValue As String = item.SubItems(4).Text
                Dim employeeNoValue As String = item.SubItems(5).Text

                If timedInBorrowers.Contains(lrnValue) OrElse timedInBorrowers.Contains(employeeNoValue) Then
                    item.BackColor = Color.FromArgb(153, 255, 153)
                Else
                    item.BackColor = Color.FromArgb(255, 102, 102)
                End If
            Next

        Catch ex As Exception
            MessageBox.Show("Error highlighting borrowers: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            If con2.State = ConnectionState.Open Then
                con2.Close()
            End If
        End Try

    End Sub

    Private Sub RegisteredBrwr_FormClosed(sender As Object, e As FormClosedEventArgs) Handles Me.FormClosed

        oras.btnview.Visible = False

    End Sub
End Class