Imports MySql.Data.MySqlClient
Imports System.Drawing
Imports System.Windows.Forms

Public Class AuditTrail
    Private Sub AuditTrail_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        cbfilter.Items.Clear()
        cbfilter.Items.Add("All")
        cbfilter.Items.Add("Librarian")
        cbfilter.Items.Add("Assistant Librarian")
        cbfilter.Items.Add("Staff")
        cbfilter.SelectedIndex = 0

        refreshaudit()


        GlobalVarsModule.AutoRefreshGrid(DataGridView1, "SELECT * FROM `audit_trail_tbl` ORDER BY DateTime DESC", 2000)


        AddHandler GlobalVarsModule.DatabaseUpdated, AddressOf OnDatabaseUpdated_Audit
    End Sub

    Public Sub refreshaudit(Optional ByVal selectedRole As String = "All")
        Dim con As New MySqlConnection(GlobalVarsModule.connectionString)
        Dim com As String = "SELECT * FROM `audit_trail_tbl`"
        Dim ds As New DataSet

        If selectedRole <> "All" Then
            com &= " WHERE Role = @Role"
        End If
        com &= " ORDER BY DateTime DESC"

        Dim adap As New MySqlDataAdapter()
        adap.SelectCommand = New MySqlCommand(com, con)

        If selectedRole <> "All" Then
            adap.SelectCommand.Parameters.AddWithValue("@Role", selectedRole)
        End If

        Try
            adap.Fill(ds, "INFO")
            DataGridView1.DataSource = ds.Tables("INFO")

            If DataGridView1.Columns.Contains("ID") Then
                DataGridView1.Columns("ID").Visible = False
            End If

            If DataGridView1.Columns.Contains("DateTime") Then
                DataGridView1.Sort(DataGridView1.Columns("DateTime"), System.ComponentModel.ListSortDirection.Descending)
            End If

            DataGridView1.EnableHeadersVisualStyles = False
            DataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(207, 58, 109)
            DataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White
            DataGridView1.ClearSelection()

        Catch ex As Exception
            MessageBox.Show("Error loading audit trail: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Async Sub OnDatabaseUpdated_Audit()
        Try
            Await GlobalVarsModule.LoadToGridAsync(DataGridView1, "SELECT * FROM `audit_trail_tbl` ORDER BY DateTime DESC")
            DataGridView1.ClearSelection()
        Catch
        End Try
    End Sub


    Private Sub cbfilter_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbfilter.SelectedIndexChanged


        If cbfilter.SelectedItem IsNot Nothing Then
            Dim selectedValue As String = cbfilter.SelectedItem.ToString()
            refreshaudit(selectedValue)
        End If

    End Sub
End Class