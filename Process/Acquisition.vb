Imports MySql.Data.MySqlClient

Public Class Acquisition

    Private Sub Acquisition_Load_1(sender As Object, e As EventArgs) Handles MyBase.Load


        Dim con As New MySqlConnection(connectionString)
        Dim com As String = "SELECT * FROM `acquisition_tbl`"
        Dim adp As New MySqlDataAdapter(com, con)
        Dim dt As New DataSet

        adp.Fill(dt, "INFO")

        dgv_acquisition.DataSource = dt.Tables("INFO")


        dgv_acquisition.Columns("ID").Visible = False
        dgv_acquisition.EnableHeadersVisualStyles = False
        dgv_acquisition.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(207, 58, 109)
        dgv_acquisition.ColumnHeadersDefaultCellStyle.ForeColor = Color.White

        txtbooktitle.Enabled = False
        txttotalcost.Enabled = False
        txttransactionno.Enabled = False

        cbsupplierr()

    End Sub

    Public Sub cbsupplierr()

        Dim con As New MySqlConnection(connectionString)
        Dim com As String = "SELECT ID, SupplierName FROM `supplier_tbl`"
        Dim adap As New MySqlDataAdapter(com, con)
        Dim dt As New DataTable

        adap.Fill(dt)

        cbsuppliername.DataSource = dt
        cbsuppliername.DisplayMember = "SupplierName"
        cbsuppliername.ValueMember = "ID"
        cbsuppliername.SelectedIndex = -1

    End Sub

    Private Sub txtsearch_TextChanged(sender As Object, e As EventArgs) Handles txtsearch.TextChanged

        Dim dt As DataTable = DirectCast(dgv_acquisition.DataSource, DataTable)
        If dt IsNot Nothing Then
            If txtsearch.Text.Trim() <> "" Then
                Dim filter As String = String.Format("BookTitle LIKE '*{0}*' OR ISBN LIKE '*{0}*'", txtsearch.Text.Trim())
                dt.DefaultView.RowFilter = filter
            Else
                dt.DefaultView.RowFilter = ""
            End If
        End If

    End Sub

    Private Sub btnadd_Click(sender As Object, e As EventArgs) Handles btnadd.Click

    End Sub

    Private Sub btnedit_Click(sender As Object, e As EventArgs) Handles btnedit.Click

    End Sub

    Private Sub btndelete_Click(sender As Object, e As EventArgs) Handles btndelete.Click

    End Sub

    Private Sub btnclear_Click(sender As Object, e As EventArgs) Handles btnclear.Click
        clear()
    End Sub

    Public Sub clear()

    End Sub

    Private Sub dgv_acquisition_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles dgv_acquisition.CellClick

    End Sub

    Private Sub Guna2GroupBox1_Click(sender As Object, e As EventArgs) Handles Guna2GroupBox1.Click

    End Sub
End Class