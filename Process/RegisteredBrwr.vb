Imports MySql.Data.MySqlClient
Public Class RegisteredBrwr


    Private Sub RegisteredBrwr_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        ludeyngborrower()

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
            Dim timeIn As String = e.Item.SubItems(e.Item.SubItems.Count - 2).Text
            Dim timeOut As String = e.Item.SubItems(e.Item.SubItems.Count - 1).Text

            Dim statusBrush As Brush


            If timeIn <> "N/A" AndAlso timeOut = "N/A" Then

                statusBrush = New SolidBrush(Color.FromArgb(153, 255, 153))
            Else

                statusBrush = New SolidBrush(Color.FromArgb(255, 102, 102))
            End If

            e.Graphics.FillRectangle(statusBrush, e.Bounds)
        End If

        e.DrawText()
    End Sub

    Private Sub ListView1_MouseDoubleClick(sender As Object, e As MouseEventArgs) Handles ListView1.MouseDoubleClick

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


        Dim com As String = "SELECT b.*, t.TimeIn, t.TimeOut " &
                        "FROM `borrower_tbl` b " &
                        "LEFT JOIN `oras_tbl` t ON b.Borrower = t.Borrower " &
                        "WHERE t.TimeIn IS NULL OR t.ID = (SELECT MAX(ID) FROM `oras_tbl` WHERE Borrower = b.Borrower)" &
                        "ORDER BY b.LastName ASC"

        Try
            con.Open()
            Dim cmd As New MySqlCommand(com, con)
            Dim reader As MySqlDataReader = cmd.ExecuteReader()

            While reader.Read()
                Dim item As New ListViewItem(reader.GetString("Borrower"))
                item.SubItems.Add(reader.GetString("FirstName"))
                item.SubItems.Add(reader.GetString("LastName"))
                item.SubItems.Add(If(reader.IsDBNull(reader.GetOrdinal("MiddleName")), "N/A", reader.GetString("MiddleName")))
                item.SubItems.Add(If(reader.IsDBNull(reader.GetOrdinal("LRN")), "N/A", reader.GetString("LRN")))
                item.SubItems.Add(If(reader.IsDBNull(reader.GetOrdinal("EmployeeNo")), "N/A", reader.GetString("EmployeeNo")))
                item.SubItems.Add(reader.GetString("ContactNumber"))
                item.SubItems.Add(reader.GetString("Department"))
                item.SubItems.Add(If(reader.IsDBNull(reader.GetOrdinal("Grade")), "N/A", reader.GetString("Grade")))
                item.SubItems.Add(If(reader.IsDBNull(reader.GetOrdinal("Section")), "N/A", reader.GetString("Section")))
                item.SubItems.Add(If(reader.IsDBNull(reader.GetOrdinal("Strand")), "N/A", reader.GetString("Strand")))


                item.SubItems.Add(If(reader.IsDBNull(reader.GetOrdinal("TimeIn")), "N/A", reader.GetDateTime("TimeIn").ToString()))
                item.SubItems.Add(If(reader.IsDBNull(reader.GetOrdinal("TimeOut")), "N/A", reader.GetDateTime("TimeOut").ToString()))

                ListView1.Items.Add(item)
            End While

            reader.Close()
        Catch ex As Exception
            MessageBox.Show("Error loading borrower data: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            con.Close()
        End Try
    End Sub

End Class