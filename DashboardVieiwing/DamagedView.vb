Imports MySql.Data.MySqlClient
Imports System.Data
Imports System.Windows.Forms
Imports System.Drawing

Public Class DamagedView
    Private Sub DamagedView_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        refreshdamage()
    End Sub

    Public Sub refreshdamage()

        Dim con As New MySqlConnection(GlobalVarsModule.connectionString)

        Dim com As String = "SELECT ID, AccessionID, BookTitle " &
                            "FROM acession_tbl " &
                            "WHERE Status LIKE 'Damaged%'"

        Dim adap As New MySqlDataAdapter(com, con)
        Dim ds As New DataSet

        Try
            con.Open()
            adap.Fill(ds, "info")
            DataGridView1.DataSource = ds.Tables("info")

            Me.Text = $"Damaged Books ({ds.Tables("info").Rows.Count})"

        Catch ex As Exception
            MessageBox.Show("Error loading damaged books: " & ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            If con.State = ConnectionState.Open Then con.Close()

            If DataGridView1.Columns.Count > 0 Then
                DataGridView1.EnableHeadersVisualStyles = False
                DataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(207, 58, 109)
                DataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White

                If DataGridView1.Columns.Contains("ID") Then
                    DataGridView1.Columns("ID").Visible = False
                End If
            End If

        End Try

    End Sub

    Private Sub DamagedView_KeyDown(sender As Object, e As KeyEventArgs) Handles MyBase.KeyDown
        If e.KeyCode = Keys.Escape Then
            Me.Close()
        End If
    End Sub

    Private Sub DShown(sender As Object, e As EventArgs) Handles MyBase.Shown

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