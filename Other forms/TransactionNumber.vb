Imports MySql.Data.MySqlClient

Public Class TransactionNumber

    'punyeta pagod na ako. aray kooo:>''

    Private Sub TransactionNumber_load(sender As Object, e As EventArgs) Handles MyBase.Load
        ludtransaksyun()
    End Sub

    Public Sub ludtransaksyun()
        ListView1.Items.Clear()

        Dim con As New MySqlConnection(connectionString)
        Dim com As String = "SELECT TransactionNo FROM acquisition_tbl"

        Try
            con.Open()

            Dim comsi As New MySqlCommand(com, con)
            Dim reader As MySqlDataReader = comsi.ExecuteReader()

            If ListView1.Columns.Count = 0 Then
                ListView1.Columns.Add("TransactionNo", -2)
            End If

            Do While reader.Read()
                Dim transactionNo As String = reader("TransactionNo").ToString()
                Dim item As New ListViewItem(transactionNo)
                ListView1.Items.Add(item)
            Loop

            reader.Close()

            ListView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent)
            ListView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize)

        Catch ex As Exception
            MessageBox.Show("Error loading transaction numbers: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
        End Try
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

    Private Sub TransactionNumber_KeyDown(sender As Object, e As KeyEventArgs) Handles MyBase.KeyDown

        If e.KeyCode = Keys.Escape Then
            Me.Close()

        End If
    End Sub

    Private Sub ListView1_DoubleClick(sender As Object, e As EventArgs) Handles ListView1.DoubleClick

        If ListView1.SelectedItems.Count > 0 Then

            Dim selectedTransactionNo As String = ListView1.SelectedItems(0).Text
            Dim accessionForm As Accession = DirectCast(Application.OpenForms("Accession"), Accession)

            If accessionForm IsNot Nothing Then

                accessionForm.txttransactionno.Text = selectedTransactionNo

                Dim con As New MySqlConnection(connectionString)
                Dim quantity As Integer = 0
                Dim bookTitle As String = ""
                Dim isbn As String = ""
                Dim suppliername As String = ""
                Dim barcode As String = ""

                Try
                    con.Open()
                    Dim com As String = "SELECT Quantity, BookTitle, ISBN, SupplierName, Barcode FROM acquisition_tbl WHERE TransactionNo = @TransactionNo"
                    Dim comsu As New MySqlCommand(com, con)
                    comsu.Parameters.AddWithValue("@TransactionNo", selectedTransactionNo)

                    Dim reader As MySqlDataReader = comsu.ExecuteReader()
                    If reader.Read() Then
                        quantity = CInt(reader("Quantity"))
                        bookTitle = reader("BookTitle").ToString()
                        isbn = reader("ISBN").ToString()
                        suppliername = reader("SupplierName").ToString()
                        barcode = reader("Barcode").ToString()
                    End If
                    reader.Close()


                    Dim generatedIDs As New List(Of String)
                    Dim rand As New Random()
                    Dim isUnique As Boolean = False
                    Dim newAccessionID As String = ""

                    For i As Integer = 1 To quantity
                        isUnique = False
                        While Not isUnique

                            newAccessionID = rand.Next(10000, 99999).ToString()


                            Dim checkIDQuery As String = "SELECT COUNT(*) FROM `acession_tbl` WHERE AccessionID = @AccessionID"
                            Dim checkIDCommand As New MySqlCommand(checkIDQuery, con)
                            checkIDCommand.Parameters.AddWithValue("@AccessionID", newAccessionID)
                            Dim existingIDCount As Integer = CInt(checkIDCommand.ExecuteScalar())

                            If existingIDCount = 0 Then

                                If Not generatedIDs.Contains(newAccessionID) Then
                                    isUnique = True
                                    generatedIDs.Add(newAccessionID)
                                End If
                            End If
                        End While
                    Next

                    If generatedIDs.Count > 0 Then
                        accessionForm.txtaccessionid.Text = String.Join(", ", generatedIDs.ToArray())
                    Else
                        accessionForm.txtaccessionid.Text = ""
                    End If

                Catch ex As Exception
                    MessageBox.Show("Error getting book data: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Return
                Finally
                    If con.State = ConnectionState.Open Then
                        con.Close()
                    End If
                End Try

                accessionForm.txtbooktitle.Text = bookTitle
                accessionForm.txtisbn.Text = isbn
                accessionForm.txtsuppliername.Text = suppliername
                accessionForm.txtbarcodes.Text = barcode

                Me.Close()
            Else
                MessageBox.Show("Accession form not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End If
        End If
    End Sub

End Class