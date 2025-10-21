Imports MySql.Data.MySqlClient
Imports System.Data
Imports System.Drawing
Imports System.Collections.Generic

Public Class TransactionNumber

    Private Const connectionString As String = "server=localhost;userid=root;database=laybsis_dbs;"

    Public Sub LoadTransactions()
        SearchTransactions(Nothing)
    End Sub


    Public Sub SearchTransactions(ByVal searchText As String)
        ListView1.Items.Clear()
        ListView1.View = View.Details

        Dim con As New MySqlConnection(connectionString)
        Dim com As String
        Dim isSearching As Boolean = Not String.IsNullOrEmpty(searchText)

        If isSearching Then

            com = "SELECT TransactionNo, BookTitle FROM acquisition_tbl WHERE TransactionNo LIKE @SearchText OR BookTitle LIKE @SearchText ORDER BY TransactionNo, BookTitle"
        Else

            com = "SELECT TransactionNo, BookTitle FROM acquisition_tbl ORDER BY TransactionNo, BookTitle"
        End If

        Try
            con.Open()

            Dim comsi As New MySqlCommand(com, con)

            If isSearching Then

                comsi.Parameters.AddWithValue("@SearchText", "%" & searchText.Trim() & "%")
            End If

            Dim reader As MySqlDataReader = comsi.ExecuteReader()

            If ListView1.Columns.Count = 0 OrElse ListView1.Columns.Count < 2 Then
                ListView1.Columns.Clear()
                ListView1.Columns.Add("Transaction No", 100)
                ListView1.Columns.Add("Book Title", 300)
            End If

            Do While reader.Read()
                Dim transactionNo As String = reader("TransactionNo").ToString()
                Dim bookTitle As String = reader("BookTitle").ToString().Trim()

                Dim item As New ListViewItem(transactionNo)
                item.SubItems.Add(bookTitle)

                If CheckIfAccessionIsComplete(transactionNo, bookTitle) Then
                    item.BackColor = Color.LightGreen
                Else
                    item.BackColor = Color.LightCoral
                End If

                ListView1.Items.Add(item)
            Loop

            reader.Close()

            ListView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent)
            ListView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize)

        Catch ex As Exception
            MessageBox.Show("Error loading transaction data: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
        End Try
    End Sub

    Public Function CheckIfAccessionIsComplete(ByVal tranNo As String, ByVal bookTitle As String) As Boolean

        Dim con As New MySqlConnection(connectionString)
        Dim totalRequiredQuantity As Integer = 0
        Dim totalCopiesFound As Integer = 0

        Try
            con.Open()

            Dim getRequiredSql As String = "SELECT Quantity FROM `acquisition_tbl` WHERE TransactionNo = @TranNo AND BookTitle = @BookTitle"
            Using getRequiredCmd As New MySqlCommand(getRequiredSql, con)
                getRequiredCmd.Parameters.AddWithValue("@TranNo", tranNo)
                getRequiredCmd.Parameters.AddWithValue("@BookTitle", bookTitle)
                Dim result As Object = getRequiredCmd.ExecuteScalar()

                If result IsNot Nothing AndAlso result IsNot DBNull.Value Then
                    totalRequiredQuantity = CInt(result)
                End If
            End Using

            Dim getAccessionCountSql As String = "SELECT COUNT(*) FROM `acession_tbl` WHERE TransactionNo = @TranNo AND BookTitle = @BookTitle"
            Using getAccessionCmd As New MySqlCommand(getAccessionCountSql, con)
                getAccessionCmd.Parameters.AddWithValue("@TranNo", tranNo)
                getAccessionCmd.Parameters.AddWithValue("@BookTitle", bookTitle)
                totalCopiesFound += CInt(getAccessionCmd.ExecuteScalar())
            End Using

            Dim getReserveCountSql As String = "SELECT COUNT(*) FROM `reservecopiess_tbl` WHERE TransactionNo = @TranNo AND BookTitle = @BookTitle AND Status = 'Reserved'"
            Using getReserveCmd As New MySqlCommand(getReserveCountSql, con)
                getReserveCmd.Parameters.AddWithValue("@TranNo", tranNo)
                getReserveCmd.Parameters.AddWithValue("@BookTitle", bookTitle)
                totalCopiesFound += CInt(getReserveCmd.ExecuteScalar())
            End Using

            Return totalCopiesFound >= totalRequiredQuantity

        Catch ex As Exception
            Return False
        Finally
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
        End Try
    End Function

    Private Sub TransactionNumber_load(sender As Object, e As EventArgs) Handles MyBase.Load
        LoadTransactions()
    End Sub

    Private Sub ListView1_DoubleClick(sender As Object, e As EventArgs) Handles ListView1.DoubleClick

        If ListView1.SelectedItems.Count > 0 Then

            Dim selectedItem As ListViewItem = ListView1.SelectedItems(0)
            Dim selectedTransactionNo As String = selectedItem.Text
            Dim selectedBookTitle As String = selectedItem.SubItems(1).Text.Trim()

            Dim accessionForm As Accession = DirectCast(Application.OpenForms("Accession"), Accession)

            If accessionForm IsNot Nothing Then

                accessionForm.txttransactionno.Text = selectedTransactionNo
                accessionForm.txtbooktitle.Text = selectedBookTitle

                Dim con As New MySqlConnection(connectionString)
                Dim quantity As Integer = 0
                Dim isbn As String = ""
                Dim suppliername As String = ""
                Dim barcode As String = ""

                Try
                    con.Open()

                    Dim com As String = "SELECT Quantity, ISBN, SupplierName, Barcode FROM acquisition_tbl WHERE TransactionNo = @TransactionNo AND BookTitle = @BookTitle"
                    Using comsu As New MySqlCommand(com, con)
                        comsu.Parameters.AddWithValue("@TransactionNo", selectedTransactionNo)
                        comsu.Parameters.AddWithValue("@BookTitle", selectedBookTitle)

                        Dim reader As MySqlDataReader = comsu.ExecuteReader()
                        If reader.Read() Then
                            quantity = CInt(reader("Quantity"))
                            isbn = reader("ISBN").ToString()
                            suppliername = reader("SupplierName").ToString()
                            barcode = reader("Barcode").ToString()
                        End If
                        reader.Close()
                    End Using

                    Dim countSql As String = "SELECT COUNT(*) FROM acession_tbl WHERE TransactionNo = @TranNo AND BookTitle = @Title"
                    Using countCmd As New MySqlCommand(countSql, con)
                        countCmd.Parameters.AddWithValue("@TranNo", selectedTransactionNo)
                        countCmd.Parameters.AddWithValue("@Title", selectedBookTitle)
                        Dim existingCount As Integer = CInt(countCmd.ExecuteScalar())

                        Dim remainingToGenerate As Integer = quantity - existingCount

                        If remainingToGenerate <= 0 Then
                            MessageBox.Show("All copies for this title in this transaction have already been accessioned.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information)
                            accessionForm.txtaccessionid.Text = "COMPLETED"
                            accessionForm.txtaccessionid.Enabled = False
                            Me.Close()
                            Return
                        End If

                        Dim generatedIDs As New List(Of String)
                        Dim rand As New Random()

                        For i As Integer = 1 To remainingToGenerate
                            Dim isUnique As Boolean = False
                            Dim newAccessionID As String = ""

                            While Not isUnique
                                newAccessionID = rand.Next(10000, 99999).ToString()

                                Dim checkIDQuery As String = "SELECT COUNT(*) FROM `acession_tbl` WHERE AccessionID = @AccessionID"
                                Using checkIDCommand As New MySqlCommand(checkIDQuery, con)
                                    checkIDCommand.Parameters.AddWithValue("@AccessionID", newAccessionID)
                                    Dim existingIDCount As Integer = CInt(checkIDCommand.ExecuteScalar())

                                    If existingIDCount = 0 AndAlso Not generatedIDs.Contains(newAccessionID) Then
                                        isUnique = True
                                        generatedIDs.Add(newAccessionID)
                                    End If
                                End Using
                            End While
                        Next

                        If generatedIDs.Count > 0 Then
                            accessionForm.txtaccessionid.Text = String.Join(", ", generatedIDs.ToArray())
                            accessionForm.txtaccessionid.Enabled = True
                        Else
                            accessionForm.txtaccessionid.Text = ""
                            accessionForm.txtaccessionid.Enabled = True
                        End If
                    End Using

                Catch ex As Exception
                    MessageBox.Show("Error getting book data: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Return
                Finally
                    If con.State = ConnectionState.Open Then
                        con.Close()
                    End If
                End Try

                accessionForm.txtisbn.Text = isbn
                accessionForm.txtsuppliername.Text = suppliername
                accessionForm.txtbarcodes.Text = barcode

                Me.Close()
            Else
                MessageBox.Show("Accession form not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End If
        End If
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
        e.DrawDefault = False
    End Sub

    Private Sub ListView1_DrawSubItem(sender As Object, e As DrawListViewSubItemEventArgs) Handles ListView1.DrawSubItem

        e.DrawBackground()

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

    Private Sub txtsearch_TextChanged(sender As Object, e As EventArgs) Handles txtsearch.TextChanged

        SearchTransactions(txtsearch.Text)
    End Sub
End Class