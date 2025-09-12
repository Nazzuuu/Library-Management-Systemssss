Imports MySql.Data.MySqlClient

Public Class Accession
    Private Sub Acession_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        Dim con As New MySqlConnection(connectionString)
        Dim com As String = "SELECT * FROM `acession_tbl`"
        Dim adap As New MySqlDataAdapter(com, con)
        Dim ds As New DataSet

        adap.Fill(ds, "INFO")
        Datagridview1.DataSource = ds.Tables("INFO")
        Datagridview1.Columns("ID").Visible = False

        Datagridview1.EnableHeadersVisualStyles = False
        Datagridview1.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(207, 58, 109)
        Datagridview1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White

        txtaccessionid.Enabled = False
        txtbooktitle.Enabled = False
        txtisbn.Enabled = False
        txtbookprice.Enabled = False
        txtsuppliername.Enabled = False
        rbavailable.Visible = False
        rbavailable.Checked = True
        Label4.Visible = False

        shelfsu()
    End Sub

    Public Sub shelfsu()

        Dim con As New MySqlConnection(connectionString)
        Dim com As String = "SELECT ID, Shelf FROM `shelf_tbl`"
        Dim adap As New MySqlDataAdapter(com, con)
        Dim dt As New DataTable

        adap.Fill(dt)

        cbshelf.DataSource = dt
        cbshelf.DisplayMember = "Shelf"
        cbshelf.ValueMember = "ID"
        cbshelf.SelectedIndex = -1

    End Sub
End Class