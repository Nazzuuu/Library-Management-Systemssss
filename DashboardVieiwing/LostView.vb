Imports MySql.Data.MySqlClient
Imports System.Data
Imports System.Windows.Forms
Imports System.Drawing

Public Class LostView
    Private Sub LostView_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        refreshlost()
    End Sub

    Public Sub refreshlost()

        Dim con As New MySqlConnection(GlobalVarsModule.connectionString)

        Dim com As String = "SELECT " &
                                "AccessionID, " &
                                "BookTitle " &
                                "FROM acession_tbl " &
                                "WHERE Status LIKE 'Lost%'"

        Dim adap As New MySqlDataAdapter(com, con)
        Dim ds As New DataSet

        Try
            con.Open()
            adap.Fill(ds, "info")

            DataGridView1.DataSource = ds.Tables("info")

            Me.Text = $"Lost Books ({ds.Tables("info").Rows.Count} Total Books)"

            If DataGridView1.Columns.Count > 0 Then
                DataGridView1.EnableHeadersVisualStyles = False
                DataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(207, 58, 109)
                DataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White

                If DataGridView1.Columns.Contains("AccessionID") Then
                    DataGridView1.Columns("AccessionID").HeaderText = "ACCESSION ID"
                End If
                If DataGridView1.Columns.Contains("BookTitle") Then
                    DataGridView1.Columns("BookTitle").HeaderText = "BOOK TITLE"
                End If
            End If

        Catch ex As Exception
            MessageBox.Show("Error loading lost books: " & ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            If con.State = ConnectionState.Open Then con.Close()
        End Try

    End Sub

    Private Sub LostView_KeyDown(sender As Object, e As KeyEventArgs) Handles MyBase.KeyDown
        If e.KeyCode = Keys.Escape Then
            Me.Close()
        End If
    End Sub

    Private Sub LShown(sender As Object, e As EventArgs) Handles MyBase.Shown

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