Imports MySql.Data.MySqlClient
Imports System.Drawing

Public Class SelectBarcode

    Private Sub SelectBarcode_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        cbfilter.Items.Clear()
        cbfilter.Items.AddRange({"All", "Used", "NotUsed"})
        cbfilter.SelectedIndex = 0

        LoadAndSyncBarcodes()

        GlobalVarsModule.AutoRefreshGrid(DataGridView1, BuildSelectBarcodeQuery(cbfilter.SelectedItem.ToString()), 2000)


        AddHandler GlobalVarsModule.DatabaseUpdated, AddressOf OnDatabaseUpdated


        DataGridView1.EnableHeadersVisualStyles = False
        DataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(207, 58, 109)
        DataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White
        DataGridView1.ClearSelection()
        DataGridView1.AllowUserToAddRows = False
    End Sub


    Private Function BuildSelectBarcodeQuery() As String
        Dim baseQuery As String = "
        SELECT 
            s.ID,
            s.Barcode,
            s.BookTitle,
            CASE 
                WHEN a.Barcode IS NOT NULL THEN 'Used'
                ELSE 'NotUsed'
            END AS Status
        FROM selectbarcode_tbl s
        LEFT JOIN acquisition_tbl a ON s.Barcode = a.Barcode"

        Select Case cbfilter.SelectedItem.ToString()
            Case "Used"
                baseQuery &= " HAVING Status = 'Used'"
            Case "NotUsed"
                baseQuery &= " HAVING Status = 'NotUsed'"
        End Select

        Return baseQuery
    End Function


    Private Function BuildSelectBarcodeQuery(Optional filter As String = "All") As String
        Dim baseQuery As String = "
            SELECT 
                s.ID,
                s.Barcode,
                s.BookTitle,
                CASE 
                    WHEN a.Barcode IS NOT NULL THEN 'Used'
                    ELSE 'NotUsed'
                END AS Status
            FROM selectbarcode_tbl s
            LEFT JOIN acquisition_tbl a ON s.Barcode = a.Barcode"

        Select Case filter
            Case "Used"
                baseQuery &= " HAVING Status = 'Used'"
            Case "NotUsed"
                baseQuery &= " HAVING Status = 'NotUsed'"
        End Select

        Return baseQuery
    End Function


    Public Sub LoadAndSyncBarcodes(Optional ByVal filter As String = "All")
        Dim con As New MySqlConnection(GlobalVarsModule.connectionString)

        Try
            con.Open()


            Dim insertQuery As String = "
                INSERT INTO selectbarcode_tbl (Barcode, BookTitle)
                SELECT b.Barcode, b.BookTitle 
                FROM book_tbl b
                WHERE b.Barcode IS NOT NULL
                AND b.Barcode <> ''
                AND NOT EXISTS (
                    SELECT 1 FROM selectbarcode_tbl s WHERE s.Barcode = b.Barcode
                );"
            Using cmdInsert As New MySqlCommand(insertQuery, con)
                cmdInsert.ExecuteNonQuery()
            End Using


            Dim query As String = BuildSelectBarcodeQuery(filter)
            Dim adap As New MySqlDataAdapter(query, con)
            Dim ds As New DataSet
            adap.Fill(ds, "barcode_info")
            DataGridView1.DataSource = ds.Tables("barcode_info")

            With DataGridView1
                If .Columns.Contains("ID") Then .Columns("ID").Visible = False
                .Columns("Barcode").HeaderText = "Barcode"
                .Columns("BookTitle").HeaderText = "Book Title"
                .Columns("Status").HeaderText = "Status"
            End With

        Catch ex As Exception
            MessageBox.Show("Error while loading barcodes: " & ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            con.Close()
        End Try

        DataGridView1.ClearSelection()
        DataGridView1.Refresh()
    End Sub


    Private Async Sub OnDatabaseUpdated()
        Try
            Dim filter As String = If(cbfilter.SelectedItem IsNot Nothing, cbfilter.SelectedItem.ToString(), "All")
            Dim query As String = BuildSelectBarcodeQuery(filter)
            Await GlobalVarsModule.LoadToGridAsync(DataGridView1, query)

            If DataGridView1.Columns.Contains("ID") Then DataGridView1.Columns("ID").Visible = False
            DataGridView1.ClearSelection()
            DataGridView1.Refresh()

        Catch ex As Exception
            MessageBox.Show("Error refreshing barcode list: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub selectbarcode_Shown(sender As Object, e As EventArgs) Handles MyBase.Shown
        DataGridView1.ClearSelection()
    End Sub


    Private Sub cbfilter_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbfilter.SelectedIndexChanged
        Dim selectedFilter As String = cbfilter.SelectedItem.ToString()
        LoadAndSyncBarcodes(selectedFilter)
    End Sub


    Private Sub DataGridView1_DataBindingComplete(sender As Object, e As DataGridViewBindingCompleteEventArgs) Handles DataGridView1.DataBindingComplete
        For Each row As DataGridViewRow In DataGridView1.Rows
            If Not row.IsNewRow Then
                Dim status As String = row.Cells("Status").Value.ToString()
                If status = "Used" Then
                    row.DefaultCellStyle.BackColor = Color.LightGreen
                Else
                    row.DefaultCellStyle.BackColor = Color.LightCoral
                End If
            End If
        Next
    End Sub


    Private Sub DataGridView1_CellDoubleClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellDoubleClick
        If e.RowIndex >= 0 Then
            Dim selectedBarcode As String = DataGridView1.Rows(e.RowIndex).Cells("Barcode").Value.ToString()

            If Application.OpenForms().OfType(Of Acquisition).Any() Then
                Dim f As Acquisition = Application.OpenForms().OfType(Of Acquisition).First()
                f.txtbarcodes.Text = selectedBarcode
            End If

            Me.Close()
        End If
    End Sub


    Private Sub SelectBarcode_KeyDown(sender As Object, e As KeyEventArgs) Handles MyBase.KeyDown
        If e.KeyCode = Keys.Escape Then
            Me.Close()
        End If
    End Sub

End Class
