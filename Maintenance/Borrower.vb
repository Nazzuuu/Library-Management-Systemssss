Imports MySql.Data.MySqlClient
Public Class Borrower

    Private Sub Borrower_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        Dim con As New MySqlConnection(connectionString)
        Dim com As String = "SELECT * FROM `borrower_tbl`"
        Dim adap As New MySqlDataAdapter(com, con)
        Dim dt As New DataSet

        adap.Fill(dt, "INFO")

        DataGridView1.DataSource = dt.Tables("INFO")

        DataGridView1.Columns("ID").Visible = False
        DataGridView1.EnableHeadersVisualStyles = False
        DataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(207, 58, 109)
        DataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White

        cbgradee()
        cbsecs()
        cbdepts()
        cbstrandd()


    End Sub

    Private Sub btnimport_Click(sender As Object, e As EventArgs) Handles btnimport.Click

        If Pic1.Image IsNot Nothing Then
            Pic1.Image.Dispose()
        End If

        If OpenFileDialog1.ShowDialog <> DialogResult.Cancel Then

            Pic1.Image = Image.FromFile(OpenFileDialog1.FileName)
            Pic1.SizeMode = PictureBoxSizeMode.StretchImage
        End If
    End Sub

    Private Sub btnclearr_Click(sender As Object, e As EventArgs) Handles btnclearr.Click

        If Pic1.Image IsNot Nothing Then

            Pic1.Image.Dispose()
            Pic1.Image = Nothing

        End If

    End Sub

    Public Sub cbgradee()

        Dim con As New MySqlConnection(connectionString)
        Dim com As String = "SELECT ID, Grade FROM `grade_tbl`"
        Dim adap As New MySqlDataAdapter(com, con)
        Dim dt As New DataTable

        adap.Fill(dt)

        cbgrade.DataSource = dt
        cbgrade.DisplayMember = "Grade"
        cbgrade.ValueMember = "ID"
        cbgrade.SelectedIndex = -1

    End Sub

    Public Sub cbsecs()

        Dim con As New MySqlConnection(connectionString)
        Dim com As String = "SELECT ID, Section FROM `section_tbl`"
        Dim adap As New MySqlDataAdapter(com, con)
        Dim dt As New DataTable

        adap.Fill(dt)

        cbsection.DataSource = dt
        cbsection.DisplayMember = "Section"
        cbsection.ValueMember = "ID"
        cbsection.SelectedIndex = -1

    End Sub

    Public Sub cbdepts()

        Dim con As New MySqlConnection(connectionString)
        Dim com As String = "SELECT ID, Department FROM `department_tbl`"
        Dim adap As New MySqlDataAdapter(com, con)
        Dim dt As New DataTable

        adap.Fill(dt)

        cbdepartment.DataSource = dt
        cbdepartment.DisplayMember = "Department"
        cbdepartment.ValueMember = "ID"
        cbdepartment.SelectedIndex = -1

    End Sub

    Public Sub cbstrandd()

        Dim con As New MySqlConnection(connectionString)
        Dim com As String = "SELECT ID, Strand FROM `strand_tbl`"
        Dim adap As New MySqlDataAdapter(com, con)
        Dim dt As New DataTable

        adap.Fill(dt)

        cbstrand.DataSource = dt
        cbstrand.DisplayMember = "Strand"
        cbstrand.ValueMember = "ID"
        cbstrand.SelectedIndex = -1
    End Sub

    Private Sub txtsearch_TextChanged(sender As Object, e As EventArgs) Handles txtsearch.TextChanged

        Dim dt As DataTable = DirectCast(DataGridView1.DataSource, DataTable)

        If dt IsNot Nothing Then
            If txtsearch.Text.Trim() <> "" Then

                Dim filter As String = String.Format("FirstName LIKE '*{0}*' OR LastName LIKE '*{0}*' OR MiddleName LIKE '*{0}*' OR BorrowerType LIKE '*{0}*'", txtsearch.Text.Trim())
                dt.DefaultView.RowFilter = filter
            Else

                dt.DefaultView.RowFilter = ""
            End If
        End If

    End Sub


    'bwisitttt'''
End Class