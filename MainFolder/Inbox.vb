Imports MySql.Data.MySqlClient

Public Class Inbox
    Private Sub Inbox_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            GlobalVarsModule.EnsureInboxTableExists()
        Catch
        End Try


        Dim query As String = "SELECT ID, FullName, COALESCE(DueDate, Body) AS DueDate, COALESCE(`Date`, CreatedAt) AS `Date` FROM `inbox_tbl` ORDER BY COALESCE(`Date`, CreatedAt) DESC"

        Try
            GlobalVarsModule.AutoRefreshGrid(DataGridView1, query, 2000)
        Catch

            Try
                Dim con As New MySqlConnection(GlobalVarsModule.connectionString)
                Dim com As String = query
                Dim adap As New MySqlDataAdapter(com, con)
                Dim ds As New DataSet
                adap.Fill(ds, "INFO")

                If ds.Tables.Count > 0 Then
                    DataGridView1.DataSource = ds.Tables("INFO")
                Else
                    DataGridView1.DataSource = Nothing
                End If


            Catch
            End Try
        End Try


        Try
            DataGridView1.EnableHeadersVisualStyles = False
            DataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(207, 58, 109)
            DataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White
            DataGridView1.ReadOnly = True
        Catch
        End Try

        AddHandler DataGridView1.DataBindingComplete, Sub(s, ev)
                                                          Try
                                                              If DataGridView1.Columns.Count = 0 Then Return
                                                              For Each col As DataGridViewColumn In DataGridView1.Columns
                                                                  col.Visible = False
                                                              Next

                                                              If DataGridView1.Columns.Contains("FullName") Then
                                                                  DataGridView1.Columns("FullName").Visible = True
                                                                  DataGridView1.Columns("FullName").HeaderText = "Full Name"
                                                                  DataGridView1.Columns("FullName").AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
                                                              End If

                                                              If DataGridView1.Columns.Contains("DueDate") Then
                                                                  DataGridView1.Columns("DueDate").Visible = True
                                                                  DataGridView1.Columns("DueDate").HeaderText = "Due Date"
                                                                  DataGridView1.Columns("DueDate").AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
                                                              End If

                                                              If DataGridView1.Columns.Contains("Date") Then
                                                                  DataGridView1.Columns("Date").Visible = True
                                                                  DataGridView1.Columns("Date").HeaderText = "Date"

                                                                  DataGridView1.Columns("Date").DefaultCellStyle.Format = "yyyy-MM-dd h:mm"
                                                                  DataGridView1.Columns("Date").AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells
                                                              End If

                                                              DataGridView1.ClearSelection()
                                                          Catch
                                                          End Try
                                                      End Sub
    End Sub




    Private Sub txtsearch_TextChanged(sender As Object, e As EventArgs) Handles txtsearch.TextChanged

        HandleAutoRefreshPause(DataGridView1, txtsearch)

        Dim dt As DataTable = DirectCast(DataGridView1.DataSource, DataTable)
        If dt IsNot Nothing Then
            If txtsearch.Text.Trim() <> "" Then
                Dim filter As String = String.Format("FullName LIKE '*{0}*'", txtsearch.Text.Trim())
                dt.DefaultView.RowFilter = filter
            Else
                dt.DefaultView.RowFilter = ""
            End If
        End If

    End Sub

    Public Sub HandleAutoRefreshPause(grid As DataGridView, txtSearch As Control)
        Try
            If refreshTimers.ContainsKey(grid) Then
                Dim t As Timer = refreshTimers(grid)


                If Not String.IsNullOrWhiteSpace(txtSearch.Text) Then
                    If t.Enabled Then t.Stop()
                Else
                    If Not t.Enabled Then t.Start()
                End If
            End If
        Catch ex As Exception

        End Try
    End Sub

End Class