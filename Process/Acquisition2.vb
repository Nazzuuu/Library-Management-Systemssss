Imports MySql.Data.MySqlClient

Public Class Acquisition2


    Private Sub Acquisition2_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        LoadInitialData()
        SetupGridVisuals()

        GlobalVarsModule.AutoRefreshGrid(DataGridView1, "SELECT * FROM `acquisition_tbl`", 2000)
        AddHandler GlobalVarsModule.DatabaseUpdated, AddressOf OnDatabaseUpdated
    End Sub

    Private Sub LoadInitialData()
        Try
            Using con As New MySqlConnection(GlobalVarsModule.connectionString)
                Dim com As String = "SELECT * FROM acquisition_tbl"
                Dim adap As New MySqlDataAdapter(com, con)
                Dim dt As New DataSet()

                adap.Fill(dt, "acquisition_tbl")
                DataGridView1.DataSource = dt.Tables("acquisition_tbl")
            End Using
        Catch ex As Exception
            Debug.WriteLine("Initial load error: " & ex.Message)
        End Try
    End Sub

    Private Sub SetupGridVisuals()

        If DataGridView1.Columns.Contains("ID") Then
            DataGridView1.Columns("ID").Visible = False
        End If


        DataGridView1.EnableHeadersVisualStyles = False
        DataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(207, 58, 109)
        DataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White
        DataGridView1.ClearSelection()
    End Sub


    Private Async Sub OnDatabaseUpdated()
        Try

            Await GlobalVarsModule.LoadToGridAsync(DataGridView1, "SELECT * FROM `acquisition_tbl`")
            SetupGridVisuals()
        Catch ex As Exception
            Debug.WriteLine("OnDatabaseUpdated error: " & ex.Message)
        End Try
    End Sub

    Public Sub refreshData()
        Try
            Using con As New MySqlConnection(GlobalVarsModule.connectionString)
                Dim com As String = "SELECT * FROM `acquisition_tbl`"
                Dim adp As New MySqlDataAdapter(com, con)
                Dim dt As New DataSet()

                adp.Fill(dt, "INFO")
                DataGridView1.DataSource = dt.Tables("INFO")
                SetupGridVisuals()
            End Using
        Catch ex As Exception
            MessageBox.Show("Error refreshing data: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub Guna2Button1_Click(sender As Object, e As EventArgs) Handles Guna2Button1.Click

        With AcquistionDetails
            .SelectedAcquisitionID = ""
            .clearlahatsu(False)
            .btnupdate.Visible = False
            .btnsubmitall.Visible = True
            .btnaddanotherbook.Visible = True
            .lblsubmitcounts.Visible = True
            .cbisbnbarcode.Enabled = True
            .ShowDialog()
        End With

    End Sub
    Private Sub DataGridView1_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellClick

        If e.RowIndex >= 0 Then

            Dim result As DialogResult = MessageBox.Show(
            "Are you sure you want to edit this row?",
            "Edit Confirmation",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question
        )

            If result = DialogResult.Yes Then

                Dim row As DataGridViewRow = DataGridView1.Rows(e.RowIndex)

                With AcquistionDetails

                    .SelectedAcquisitionID = row.Cells("ID").Value.ToString()

                    .txttransactionno.Enabled = True
                    .txttransactionno.Text = row.Cells("TransactionNo").Value.ToString()
                    .txttransactionno.Enabled = False
                    .DateTimePicker1.Value = Convert.ToDateTime(row.Cells("DateAcquired").Value)

                    .supplieracq()

                    Dim supplierValue As String = If(row.Cells("SupplierName").Value IsNot Nothing, row.Cells("SupplierName").Value.ToString().Trim(), "")
                    Dim donorValue As String = If(row.Cells("Donor").Value IsNot Nothing, row.Cells("Donor").Value.ToString().Trim(), "")

                    If Not String.IsNullOrEmpty(donorValue) Then
                        .cbacquistiontype.SelectedIndex = .cbacquistiontype.FindStringExact("DONATE")
                        If .cbacquistiontype.SelectedIndex = -1 Then .cbacquistiontype.Text = "DONATE"

                        .txtdonor.Text = donorValue
                        .cbsupplierdonator.Visible = False
                        .cbsupplierdonator.SelectedIndex = -1
                        .lblsupdonator.Text = "DONATOR:"
                        .txtbooktitle.Enabled = False
                    Else
                        .cbacquistiontype.SelectedIndex = .cbacquistiontype.FindStringExact("PURCHASED")
                        If .cbacquistiontype.SelectedIndex = -1 Then .cbacquistiontype.Text = "PURCHASED"

                        .lblsupdonator.Text = "SUPPLIER:"
                        .cbsupplierdonator.Visible = True
                        .cbsupplierdonator.SelectedIndex = .cbsupplierdonator.FindStringExact(supplierValue)

                        If .cbsupplierdonator.SelectedIndex = -1 Then
                            .cbsupplierdonator.Text = supplierValue
                        End If
                        .txtdonor.Clear()
                    End If


                    .txtisbn.Text = row.Cells("ISBN").Value.ToString()
                    .txtbarcode.Text = row.Cells("Barcode").Value.ToString()
                    .txtbooktitle.Text = row.Cells("BookTitle").Value.ToString()

                    Application.DoEvents()

                    .txtbooktitle.Text = row.Cells("BookTitle").Value.ToString()

                    Dim price As Decimal = If(IsDBNull(row.Cells("BookPrice").Value), 0D, Convert.ToDecimal(row.Cells("BookPrice").Value))
                    Dim qty As Integer = If(IsDBNull(row.Cells("Quantity").Value), 0, Convert.ToInt32(row.Cells("Quantity").Value))

                    .txtbookprice.Text = price.ToString("F2")
                    .numupdown.Value = qty
                    .txttotalcost.Text = (price * qty).ToString("F2")

                    If Not String.IsNullOrEmpty(.txtisbn.Text) AndAlso .txtisbn.Text <> "N/A" Then
                        .cbisbnbarcode.Text = "ISBN"
                        .txtisbn.Enabled = True
                        .txtbarcode.Enabled = False
                    Else
                        .cbisbnbarcode.Text = "BARCODE"
                        .txtisbn.Enabled = False
                        .txtbarcode.Enabled = True
                    End If

                    .btnupdate.Visible = True
                    .btnupdate.Enabled = True
                    .btnupdate.Location = New Point(408, 41)
                    .lblsubmitcounts.Visible = False
                    .btnsubmitall.Visible = False
                    .btnaddanotherbook.Visible = False
                    .txttotalcost.Enabled = False
                    .txtbooktitle.Enabled = False
                    .btnselectsu.Enabled = False
                    .cbisbnbarcode.Enabled = False


                    .ShowDialog()

                    .SelectedAcquisitionID = ""

                End With
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

    Private Sub txtsearch_TextChanged(sender As Object, e As EventArgs) Handles txtsearch.TextChanged

        HandleAutoRefreshPause(DataGridView1, txtsearch)

        Dim dt As DataTable = DirectCast(DataGridView1.DataSource, DataTable)
        If dt IsNot Nothing Then
            If txtsearch.Text.Trim() <> "" Then
                Dim filter As String = String.Format("BookTitle LIKE '*{0}*' OR ISBN LIKE '*{0}*'", txtsearch.Text.Trim())
                dt.DefaultView.RowFilter = filter
            Else
                dt.DefaultView.RowFilter = ""
            End If
        End If
    End Sub

End Class