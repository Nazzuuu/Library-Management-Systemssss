Imports MySql.Data.MySqlClient
Imports System.Data

Public Class AvailableBooks

    Private Const connectionString As String = "server=localhost;userid=root;database=laybsis_dbs;"

    Private Sub AvailableBooks_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        refreshavail()
        counts()
    End Sub

    Public Sub refreshavail()

        Dim con As New MySqlConnection(connectionString)

        Try
            con.Open()

            Dim syncSql As String = "DELETE av.* FROM available_tbl av " &
                                    "LEFT JOIN acession_tbl ac ON av.AccessionID = ac.AccessionID " &
                                    "WHERE ac.Status <> 'Available' OR ac.Status IS NULL"

            Using syncCmd As New MySqlCommand(syncSql, con)
                syncCmd.ExecuteNonQuery()
            End Using


            Dim com As String = "SELECT ISBN, Barcode, AccessionID, BookTitle, Shelf, Status FROM `available_tbl` WHERE Status = 'Available'"
            Dim adap As New MySqlDataAdapter(com, con)
            Dim ds As New DataSet

            adap.SelectCommand.Connection = con
            adap.Fill(ds, "avail_info")

            DataGridView1.DataSource = ds.Tables("avail_info")

        Catch ex As Exception
            MessageBox.Show("Error refreshing Available Books data: " & ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
        End Try


        If DataGridView1.Columns.Contains("ID") Then
            DataGridView1.Columns("ID").Visible = False
        End If

        DataGridView1.EnableHeadersVisualStyles = False
        DataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(207, 58, 109)
        DataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White
        DataGridView1.ClearSelection()
        DataGridView1.AllowUserToAddRows = False
    End Sub

    Public Sub counts()

        Dim con As New MySqlConnection(connectionString)

        Try
            con.Open()

            Dim countss As String = "SELECT COUNT(*) FROM available_tbl WHERE Status = 'Available'"
            Using comms As New MySqlCommand(countss, con)
                Dim count As Integer = CInt(comms.ExecuteScalar())
                lblavailable.Text = count.ToString()
            End Using

        Catch ex As Exception
            MessageBox.Show("Error updating available count: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
        End Try

    End Sub

    Private Sub AvailableBooks_Shown(sender As Object, e As EventArgs) Handles MyBase.Shown
        DataGridView1.ClearSelection()
    End Sub

    Private Sub AvailableBooks_KeyDown(sender As Object, e As KeyEventArgs) Handles MyBase.KeyDown

        If e.KeyCode = Keys.Escape Then
            Me.Close()
            Accession.btnview.Visible = False

            Accession.btnview.Visible = False
            Accession.CheckBox1.Checked = False

        End If

    End Sub


    Private Sub DataGridView1_CellDoubleClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellDoubleClick

        If e.RowIndex >= 0 AndAlso Not DataGridView1.Rows(e.RowIndex).IsNewRow Then

            Try
                Dim row As DataGridViewRow = DataGridView1.Rows(e.RowIndex)
                Dim brwr As String = row.Cells("AccessionID").Value.ToString()

                Borrowing.txtaccessionid.Text = brwr

                Me.Close()

                Accession.btnview.Visible = False
                Accession.CheckBox1.Checked = False

            Catch ex As Exception
                MessageBox.Show("Error selecting book: " & ex.Message, "Selection Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try

        End If
    End Sub

End Class