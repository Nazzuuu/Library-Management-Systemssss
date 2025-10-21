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
    End Sub


    Public Sub refreshaudit(Optional ByVal selectedRole As String = "All")

        Dim con As New MySqlConnection(GlobalVarsModule.connectionString)
        Dim sqlFilter As String = ""


        If selectedRole <> "All" Then
            sqlFilter = $" WHERE Role = '{selectedRole}'"
        End If


        Dim com As String = "SELECT * FROM `audit_trail_tbl`" & sqlFilter & " ORDER BY DateTime DESC"

        Dim adap As New MySqlDataAdapter(com, con)
        Dim ds As New DataSet


        If DataGridView1.SortedColumn IsNot Nothing Then
            DataGridView1.Sort(DataGridView1.SortedColumn, System.ComponentModel.ListSortDirection.Ascending)
        End If


        Try
            adap.Fill(ds, "INFO")
            DataGridView1.DataSource = ds.Tables("INFO")

            DataGridView1.Columns("ID").Visible = False
            If DataGridView1.Columns.Contains("DateTime") Then

                DataGridView1.Sort(DataGridView1.Columns("DateTime"), System.ComponentModel.ListSortDirection.Descending)
            End If

            DataGridView1.EnableHeadersVisualStyles = False
            DataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(207, 58, 109)
            DataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White

        Catch ex As Exception
            MsgBox(ex.Message, vbCritical)
        End Try

    End Sub

    Private Sub cbfilter_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbfilter.SelectedIndexChanged


        If cbfilter.SelectedItem IsNot Nothing Then
            Dim selectedValue As String = cbfilter.SelectedItem.ToString()
            refreshaudit(selectedValue)
        End If

    End Sub
End Class