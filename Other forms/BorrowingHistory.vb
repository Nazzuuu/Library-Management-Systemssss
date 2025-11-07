Imports MySql.Data.MySqlClient

Public Class BorrowingHistory
    Private Sub BorrowingHistory_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        refreshhistory()


    End Sub

    Public Sub refreshhistory()


        GlobalVarsModule.AutoRefreshGrid(DataGridView1, "SELECT * FROM borrowinghistory_tbl", 2000)

        AddHandler GlobalVarsModule.DatabaseUpdated, AddressOf OnDatabaseUpdated


        SetupGridStyle()
    End Sub

    Private Async Sub OnDatabaseUpdated()

        Await GlobalVarsModule.LoadToGridAsync(DataGridView1, "SELECT * FROM borrowinghistory_tbl")

    End Sub

    Private Sub SetupGridStyle()
        With DataGridView1
            .EnableHeadersVisualStyles = False
            .ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(207, 58, 109)
            .ColumnHeadersDefaultCellStyle.ForeColor = Color.White
            .ColumnHeadersDefaultCellStyle.Font = New Font("Segoe UI", 10, FontStyle.Bold)
        End With
    End Sub

    Private Sub BorrowingHistory_Shown(sender As Object, e As EventArgs) Handles MyBase.Shown

        DataGridView1.ClearSelection()

    End Sub

    Private Sub txtsearch_TextChanged(sender As Object, e As EventArgs) Handles txtsearch.TextChanged

        HandleAutoRefreshPause(DataGridView1, txtsearch)

        Dim dt As DataTable = DirectCast(DataGridView1.DataSource, DataTable)
        If dt IsNot Nothing Then
            If txtsearch.Text.Trim() <> "" Then

                Dim filter As String = String.Format("Borrower LIKE '%{0}%' OR BookTitle LIKE '%{0}%' OR Name LIKE '%{0}%' OR Status LIKE '%{0}%'", txtsearch.Text.Trim())

                dt.DefaultView.RowFilter = filter
            Else
                dt.DefaultView.RowFilter = ""
            End If
        End If

    End Sub
End Class