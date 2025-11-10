Imports MySql.Data.MySqlClient
Imports System.Drawing
Imports System.Windows.Forms

Public Class AuditTrail

    Private isFormReady As Boolean = False

    Private Sub AuditTrail_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            cbfilter.Items.Clear()
            cbfilter.Items.Add("All")
            cbfilter.Items.Add("Librarian")
            cbfilter.Items.Add("Assistant Librarian")
            cbfilter.Items.Add("Staff")
            cbfilter.SelectedIndex = 0

            refreshaudit()

        Catch ex As Exception
            MessageBox.Show("Error during form load: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub



    Private Async Sub AuditTrail_Shown(sender As Object, e As EventArgs) Handles MyBase.Shown
        Try
            Await Task.Delay(300)

            GlobalVarsModule.AutoRefreshGrid(DataGridView1, "SELECT * FROM `audit_trail_tbl` ORDER BY DateTime DESC", 2000)
            AddHandler GlobalVarsModule.DatabaseUpdated, AddressOf OnDatabaseUpdated_Audit

            isFormReady = True

        Catch ex As Exception

        End Try
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
            If Not IsHandleCreated OrElse Not isFormReady Then Exit Sub
            Await GlobalVarsModule.LoadToGridAsync(DataGridView1, "SELECT * FROM `audit_trail_tbl` ORDER BY DateTime DESC")
            DataGridView1.ClearSelection()
        Catch
        End Try
    End Sub


    Private Sub cbfilter_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbfilter.SelectedIndexChanged
        If Not isFormReady Then Exit Sub
        If cbfilter.SelectedItem IsNot Nothing Then
            Dim selectedValue As String = cbfilter.SelectedItem.ToString()
            refreshaudit(selectedValue)

            Dim query As String
            If selectedValue = "All" Then
                query = "SELECT * FROM `audit_trail_tbl` ORDER BY DateTime DESC"
            Else
                query = "SELECT * FROM `audit_trail_tbl` WHERE Role = '" & selectedValue & "' ORDER BY DateTime DESC"
            End If

            If IsHandleCreated Then
                GlobalVarsModule.AutoRefreshGrid(DataGridView1, query, 2000)
            End If
        End If
    End Sub

End Class
