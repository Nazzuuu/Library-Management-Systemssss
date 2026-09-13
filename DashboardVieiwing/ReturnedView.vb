Imports MySql.Data.MySqlClient
Imports System.Data
Imports System.Windows.Forms
Imports System.Drawing

Public Class ReturnedView
    Private Sub ReturnedView_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        refreshreturned()
    End Sub

    Public Sub refreshreturned()

        Dim con As New MySqlConnection(GlobalVarsModule.connectionString)
        Dim com As String = "SELECT " &
                            "r.Borrower, " &
                            "r.FullName, " &
                            "h.AccessionID, " &
                            "h.BookTitle, " &
                            "r.BookTotal, " &
                            "r.ReturnDate " &
                            "FROM returning_tbl r " &
                            "INNER JOIN borrowinghistory_tbl h ON r.TransactionReceipt = h.TransactionReceipt " &
                            "AND r.ReturnedBook LIKE CONCAT('%', h.BookTitle, '%') " &
                            "WHERE r.Status NOT LIKE 'Lost%'"

        Dim adap As New MySqlDataAdapter(com, con)
        Dim ds As New DataSet

        Dim dtFinal As New DataTable()
        dtFinal.Columns.Add("Borrower", GetType(String))
        dtFinal.Columns.Add("FullName", GetType(String))
        dtFinal.Columns.Add("AccessionID", GetType(String))
        dtFinal.Columns.Add("BookTitle", GetType(String))
        dtFinal.Columns.Add("ReturnedDate", GetType(String))

        Dim totalBooksReturned As Integer = 0

        Try
            con.Open()
            adap.Fill(ds, "info")


            For Each dr As DataRow In ds.Tables("info").Rows
                Dim borrower As String = If(ds.Tables("info").Columns.Contains("Borrower") AndAlso Not IsDBNull(dr("Borrower")), dr("Borrower").ToString(), "")
                Dim fullname As String = If(ds.Tables("info").Columns.Contains("FullName") AndAlso Not IsDBNull(dr("FullName")), dr("FullName").ToString(), "")
                Dim accessionID As String = If(ds.Tables("info").Columns.Contains("AccessionID") AndAlso Not IsDBNull(dr("AccessionID")), dr("AccessionID").ToString(), "")
                Dim bookTitle As String = If(ds.Tables("info").Columns.Contains("BookTitle") AndAlso Not IsDBNull(dr("BookTitle")), dr("BookTitle").ToString(), "")
                Dim returnedDate As String = ""
                If ds.Tables("info").Columns.Contains("ReturnDate") AndAlso Not IsDBNull(dr("ReturnDate")) Then
                    Try
                        Dim dt As DateTime = Convert.ToDateTime(dr("ReturnDate"))
                        returnedDate = dt.ToString("yyyy-MM-dd")
                    Catch
                        returnedDate = dr("ReturnDate").ToString()
                    End Try
                End If

                If Not String.IsNullOrWhiteSpace(accessionID) Or Not String.IsNullOrWhiteSpace(bookTitle) Then
                    dtFinal.Rows.Add(New Object() {borrower, fullname, accessionID, bookTitle, returnedDate})
                    totalBooksReturned += 1
                End If
            Next

            DataGridView1.DataSource = dtFinal

            Me.Text = $"Returned Books ({totalBooksReturned} Total Books)"

        Catch ex As Exception
            MessageBox.Show("Error loading returned books: " & ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            If con.State = ConnectionState.Open Then con.Close()


            If DataGridView1.Columns.Count > 0 Then
                DataGridView1.EnableHeadersVisualStyles = False
                DataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(207, 58, 109)
                DataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White

                If DataGridView1.Columns.Contains("Borrower") Then
                    DataGridView1.Columns("Borrower").HeaderText = "BORROWER"
                End If
                If DataGridView1.Columns.Contains("FullName") Then
                    DataGridView1.Columns("FullName").HeaderText = "FULL NAME"
                End If
                If DataGridView1.Columns.Contains("BookTitle") Then
                    DataGridView1.Columns("BookTitle").HeaderText = "BOOK TITLE"
                End If
                If DataGridView1.Columns.Contains("AccessionID") Then
                    DataGridView1.Columns("AccessionID").HeaderText = "ACCESSION ID"
                End If
                If DataGridView1.Columns.Contains("ReturnedDate") Then
                    DataGridView1.Columns("ReturnedDate").HeaderText = "RETURNED DATE"
                End If
            End If

        End Try
    End Sub

    Private Sub ReturnedView_KeyDown(sender As Object, e As KeyEventArgs) Handles MyBase.KeyDown
        If e.KeyCode = Keys.Escape Then
            Me.Close()
        End If
    End Sub

    Private Sub RRShown(sender As Object, e As EventArgs) Handles MyBase.Shown

        DataGridView1.ClearSelection()

    End Sub

    Private Sub txtsearch_TextChanged(sender As Object, e As EventArgs) Handles txtsearch.TextChanged


        Dim dt As DataTable = DirectCast(DataGridView1.DataSource, DataTable)
        If dt IsNot Nothing Then
            If txtsearch.Text.Trim() <> "" Then
                Dim filter As String = String.Format("BookTitle LIKE '*{0}*'", txtsearch.Text.Trim())
                dt.DefaultView.RowFilter = filter
            Else
                dt.DefaultView.RowFilter = ""
            End If
        End If

    End Sub

End Class