Imports System.Management
Imports MySql.Data.MySqlClient
Imports System.Data
Imports System.IO
Imports System.Globalization
Imports System.IO.Compression
Imports System.Xml.Linq
Imports System.Collections.Generic
Imports System.Linq

'sheytsuu1'
Public Class Borrower


    Private ReadOnly connectionString As String = GlobalVarsModule.connectionString
    Private isBackspacing As Boolean = False

    Private Sub Borrower_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        refreshData()
        DisablePaste_AllTextBoxes()
        AddHandler GlobalVarsModule.DatabaseUpdated, AddressOf OnDatabaseUpdated
        GlobalVarsModule.EnableCapitalizeFirstLetterForControls(Me)
    End Sub

    Private Sub ImportData(importDt As DataTable)
        Try
            Using con As New MySqlConnection(connectionString)
                con.Open()
                For Each dr As DataRow In importDt.Rows
                    Dim borrowerType As String = GetColumnValue(dr, New String() {"Borrower"})
                    Dim firstName As String = GetColumnValue(dr, New String() {"FirstName", "Firstname", "First Name", "GivenName"})
                    Dim lastName As String = GetColumnValue(dr, New String() {"LastName", "Lastname", "Last Name", "Surname"})
                    Dim mid As String = GetColumnValue(dr, New String() {"MiddleName", "MiddleInitial", "Middlename"})
                    If String.IsNullOrWhiteSpace(mid) Then mid = ""
                    Dim lrnVal As String = GetColumnValue(dr, New String() {"LRN"})
                    Dim empVal As String = GetColumnValue(dr, New String() {"EmployeeNo", "EmployeeNo."})
                    Dim contactVal As String = GetColumnValue(dr, New String() {"ContactNumber", "Contact"})
                    Dim dept As String = GetColumnValue(dr, New String() {"Department"})
                    Dim grade As String = GetColumnValue(dr, New String() {"Grade"})
                    Dim section As String = GetColumnValue(dr, New String() {"Section"})
                    Dim strand As String = GetColumnValue(dr, New String() {"Strand"})

                    Dim com As New MySqlCommand("INSERT INTO `borrower_tbl`(`Borrower`, `FirstName`, `LastName`, `MiddleInitial`, `LRN`, `EmployeeNo`, `ContactNumber`, `Department`, `Grade`, `Section`, `Strand`) VALUES (@Borrower, @FirstName, @LastName, @MiddleInitial, @LRN, @EmployeeNo, @ContactNumber, @Department, @Grade, @Section, @Strand);", con)

                    com.Parameters.AddWithValue("@Borrower", If(String.IsNullOrWhiteSpace(borrowerType), DBNull.Value, borrowerType))
                    com.Parameters.AddWithValue("@FirstName", If(String.IsNullOrWhiteSpace(firstName), DBNull.Value, firstName))
                    com.Parameters.AddWithValue("@LastName", If(String.IsNullOrWhiteSpace(lastName), DBNull.Value, lastName))
                    com.Parameters.AddWithValue("@MiddleInitial", If(String.IsNullOrWhiteSpace(mid), DBNull.Value, mid))
                    com.Parameters.AddWithValue("@LRN", If(String.IsNullOrWhiteSpace(lrnVal), DBNull.Value, lrnVal))
                    com.Parameters.AddWithValue("@EmployeeNo", If(String.IsNullOrWhiteSpace(empVal), DBNull.Value, empVal))
                    com.Parameters.AddWithValue("@ContactNumber", If(String.IsNullOrWhiteSpace(contactVal), DBNull.Value, contactVal))
                    com.Parameters.AddWithValue("@Department", If(String.IsNullOrWhiteSpace(dept), DBNull.Value, dept))
                    com.Parameters.AddWithValue("@Grade", If(String.IsNullOrWhiteSpace(grade), DBNull.Value, grade))
                    com.Parameters.AddWithValue("@Section", If(String.IsNullOrWhiteSpace(section), DBNull.Value, section))
                    com.Parameters.AddWithValue("@Strand", If(String.IsNullOrWhiteSpace(strand), DBNull.Value, strand))

                    com.ExecuteNonQuery()
                Next
            End Using

            MessageBox.Show("Import completed.", "Import", MessageBoxButtons.OK, MessageBoxIcon.Information)
            refreshData()
        Catch ex As Exception
            MessageBox.Show("Error importing data: " & ex.Message, "Import Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub btnexport_Click(sender As Object, e As EventArgs) Handles btnexport.Click
        Try
            Using ofd As New OpenFileDialog()


                ofd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
                ofd.Filter = "Excel Files|*.xlsx|CSV Files|*.csv|All Files|*.*"
                ofd.FilterIndex = 1
                ofd.CheckFileExists = True
                ofd.Multiselect = False
                ofd.RestoreDirectory = True
                ofd.Title = "Select borrower import file"

                If ofd.ShowDialog() <> DialogResult.OK Then
                    Return
                End If

                Dim importDt As DataTable = Nothing

                Try
                    importDt = ReadExcelToDataTable(ofd.FileName)
                Catch ex As Exception
                    MessageBox.Show(
                        "Failed to read file: " & ex.Message,
                        "Import Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    )
                    Return
                End Try

                If importDt Is Nothing OrElse importDt.Rows.Count = 0 Then
                    MessageBox.Show(
                        "No data found in the selected file.",
                        "Import",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    )
                    Return
                End If

                ' Load existing identifiers and full names from DB
                Dim existingLRNs As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)
                Dim existingEmpNos As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)
                Dim existingContacts As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)
                Dim existingFullNames As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)

                Using con As New MySqlConnection(connectionString)

                    con.Open()

                    Using cmd As New MySqlCommand(
                        "SELECT IFNULL(LRN,'') AS LRN, " &
                        "IFNULL(EmployeeNo,'') AS EmployeeNo, " &
                        "IFNULL(ContactNumber,'') AS ContactNumber, " &
                        "IFNULL(FirstName,'') AS FirstName, IFNULL(LastName,'') AS LastName " &
                        "FROM borrower_tbl",
                        con
                    )

                        Using rdr As MySqlDataReader = cmd.ExecuteReader()

                            While rdr.Read()

                                Dim lrn As String =
                                    If(
                                        rdr("LRN") Is DBNull.Value,
                                        String.Empty,
                                        rdr("LRN").ToString().Trim()
                                    )

                                Dim emp As String =
                                    If(
                                        rdr("EmployeeNo") Is DBNull.Value,
                                        String.Empty,
                                        rdr("EmployeeNo").ToString().Trim()
                                    )

                                Dim cont As String =
                                    If(
                                        rdr("ContactNumber") Is DBNull.Value,
                                        String.Empty,
                                        rdr("ContactNumber").ToString().Trim()
                                    )

                                Dim fn As String = If(rdr("FirstName") Is DBNull.Value, String.Empty, rdr("FirstName").ToString().Trim())
                                Dim ln As String = If(rdr("LastName") Is DBNull.Value, String.Empty, rdr("LastName").ToString().Trim())

                                Dim normLrn As String = NormalizeIdentifier(lrn)
                                Dim normEmp As String = NormalizeIdentifier(emp)
                                Dim normCont As String = NormalizeIdentifier(cont)
                                Dim normFullName As String = NormalizeName(ln, fn)

                                If Not String.IsNullOrWhiteSpace(normLrn) Then
                                    existingLRNs.Add(normLrn)
                                End If

                                If Not String.IsNullOrWhiteSpace(normEmp) Then
                                    existingEmpNos.Add(normEmp)
                                End If

                                If Not String.IsNullOrWhiteSpace(normCont) Then
                                    existingContacts.Add(normCont)
                                End If

                                If Not String.IsNullOrWhiteSpace(normFullName) Then
                                    existingFullNames.Add(normFullName)
                                End If

                            End While

                        End Using

                    End Using

                End Using

                Dim duplicates As New List(Of String)()

                For Each dr As DataRow In importDt.Rows

                    Dim firstName As String =
                        GetColumnValue(
                            dr,
                            New String() {
                                "FirstName",
                                "Firstname",
                                "First Name",
                                "GivenName"
                            }
                        )

                    Dim lastName As String =
                        GetColumnValue(
                            dr,
                            New String() {
                                "LastName",
                                "Lastname",
                                "Last Name",
                                "Surname"
                            }
                        )

                    Dim lrnVal As String =
                        GetColumnValue(
                            dr,
                            New String() {
                                "LRN"
                            }
                        )

                    Dim empVal As String =
                        GetColumnValue(
                            dr,
                            New String() {
                                "EmployeeNo",
                                "EmployeeNo."
                            }
                        )

                    Dim contactVal As String =
                        GetColumnValue(
                            dr,
                            New String() {
                                "ContactNumber",
                                "Contact"
                            }
                        )

                    Dim normLrnVal As String = NormalizeIdentifier(lrnVal)
                    Dim normEmpVal As String = NormalizeIdentifier(empVal)
                    Dim normContactVal As String = NormalizeIdentifier(contactVal)

                    Dim normFullNameImport As String = NormalizeName(lastName, firstName)

                    Dim matched As New List(Of String)()

                    If Not String.IsNullOrWhiteSpace(normLrnVal) AndAlso existingLRNs.Contains(normLrnVal) Then
                        matched.Add("LRN: " & lrnVal.Trim())
                    End If

                    If Not String.IsNullOrWhiteSpace(normEmpVal) AndAlso existingEmpNos.Contains(normEmpVal) Then
                        matched.Add("EmployeeNo: " & empVal.Trim())
                    End If

                    If Not String.IsNullOrWhiteSpace(normContactVal) AndAlso existingContacts.Contains(normContactVal) Then
                        matched.Add("ContactNumber: " & contactVal.Trim())
                    End If

                    If Not String.IsNullOrWhiteSpace(normFullNameImport) AndAlso existingFullNames.Contains(normFullNameImport) Then
                        matched.Add("FullName duplicate")
                    End If

                    If matched.Count > 0 Then
                        Dim namePart As String
                        If String.IsNullOrWhiteSpace(lastName) AndAlso String.IsNullOrWhiteSpace(firstName) Then
                            namePart = "(No Name)"
                        Else
                            namePart = (lastName & ", " & firstName).Trim().Trim(","c)
                        End If

                        duplicates.Add(namePart & " -> " & String.Join(", ", matched))
                    End If

                Next

                If duplicates.Count > 0 Then

                    Dim msg As String =
                        "Found " &
                        duplicates.Count.ToString() &
                        " duplicate borrower(s) already in system:" &
                        Environment.NewLine &
                        Environment.NewLine &
                        String.Join(
                            Environment.NewLine,
                            duplicates
                        )

                    MessageBox.Show(
                        msg,
                        "Duplicate Borrowers",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    )

                Else

                    Dim result As DialogResult = MessageBox.Show(
                        "No duplicate borrowers found. Do you want to import now?",
                        "Import Check",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question
                    )

                    If result = DialogResult.Yes Then
                        ImportData(importDt)
                    End If

                End If

            End Using

        Catch ex As Exception

            MessageBox.Show(
                "Error during import check: " &
                ex.Message,
                "Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error
            )

        End Try

    End Sub


    Private Function GetColumnValue(
        dr As DataRow,
        possibleNames() As String
    ) As String

        For Each colName As String In possibleNames

            For Each column As DataColumn In dr.Table.Columns

                If String.Equals(
                    column.ColumnName.Trim(),
                    colName.Trim(),
                    StringComparison.OrdinalIgnoreCase
                ) Then

                    If dr(column) IsNot DBNull.Value Then

                        Dim v As String =
                            dr(column).ToString()

                        If Not String.IsNullOrWhiteSpace(v) Then
                            Return v.Trim()
                        End If

                    End If

                End If

            Next

        Next

        Return String.Empty

    End Function

    Private Function NormalizeName(lastName As String, firstName As String) As String
        Dim ln As String = If(lastName, String.Empty).Trim().ToUpperInvariant()
        Dim fn As String = If(firstName, String.Empty).Trim().ToUpperInvariant()
        If String.IsNullOrWhiteSpace(ln) AndAlso String.IsNullOrWhiteSpace(fn) Then
            Return String.Empty
        End If
        Return (ln & ", " & fn).Trim()
    End Function

    Private Function NormalizeIdentifier(val As String) As String
        If String.IsNullOrWhiteSpace(val) Then
            Return String.Empty
        End If

        val = val.Trim()

        Dim d As Double
        If Double.TryParse(val, NumberStyles.Any, CultureInfo.InvariantCulture, d) Then

            If Math.Abs(d - Math.Truncate(d)) < 0.0000001 Then
                Return Convert.ToInt64(Math.Truncate(d)).ToString()
            End If

        End If


        Dim digitsOnly = New String(val.Where(Function(c) Char.IsDigit(c)).ToArray())
        Return digitsOnly
    End Function


    Private Function ReadExcelToDataTable(filePath As String) As DataTable

        Dim ext As String =
            Path.GetExtension(filePath).ToLowerInvariant()

        Dim dt As New DataTable()


        If ext = ".csv" Then

            Dim lines() As String =
                File.ReadAllLines(filePath)

            If lines.Length = 0 Then
                Return dt
            End If

            Dim headers =
                lines(0).Split({","c})

            For Each h In headers

                Dim headerName As String =
                    h.Trim().Trim(""""c)

                If String.IsNullOrWhiteSpace(headerName) Then
                    headerName =
                        "Column" &
                        dt.Columns.Count.ToString()
                End If

                Dim originalName As String =
                    headerName

                Dim counter As Integer = 1

                While dt.Columns.Contains(headerName)

                    headerName =
                        originalName &
                        "_" &
                        counter.ToString()

                    counter += 1

                End While

                dt.Columns.Add(headerName)

            Next

            For i As Integer = 1 To lines.Length - 1

                If String.IsNullOrWhiteSpace(lines(i)) Then
                    Continue For
                End If

                Dim parts =
                    lines(i).Split({","c})

                Dim row As DataRow =
                    dt.NewRow()

                For c As Integer = 0 To Math.Min(parts.Length - 1, dt.Columns.Count - 1)

                    row(c) =
                        parts(c).
                        Trim().
                        Trim(""""c)

                Next

                dt.Rows.Add(row)

            Next

            Return dt

        End If


        If ext = ".xlsx" Then

            Return ReadXlsxToDataTable(filePath)

        End If


        Throw New NotSupportedException(
            "Only .xlsx and .csv files are supported for import." &
            Environment.NewLine &
            Environment.NewLine &
            "Please select an Excel .xlsx file."
        )

    End Function


    Private Function ReadXlsxToDataTable(filePath As String) As DataTable

        Dim dt As New DataTable()

        Using archive As ZipArchive =
            ZipFile.OpenRead(filePath)


            Dim sharedStrings As New List(Of String)()

            Dim sharedStringsEntry As ZipArchiveEntry =
                archive.GetEntry("xl/sharedStrings.xml")

            If sharedStringsEntry IsNot Nothing Then

                Using stream As Stream =
                    sharedStringsEntry.Open()

                    Dim sharedDoc As XDocument =
                        XDocument.Load(stream)

                    Dim ns As XNamespace =
                        "http://schemas.openxmlformats.org/spreadsheetml/2006/main"

                    For Each si As XElement In
                        sharedDoc.Descendants(ns + "si")

                        Dim textValue As String =
                            String.Concat(
                                si.Descendants(ns + "t").
                                Select(Function(t) t.Value)
                            )

                        sharedStrings.Add(textValue)

                    Next

                End Using

            End If


            Dim worksheetEntry As ZipArchiveEntry = Nothing

            For Each entry As ZipArchiveEntry In archive.Entries

                If entry.FullName.StartsWith(
                    "xl/worksheets/",
                    StringComparison.OrdinalIgnoreCase
                ) AndAlso
                   entry.FullName.EndsWith(
                       ".xml",
                       StringComparison.OrdinalIgnoreCase
                   ) Then

                    worksheetEntry = entry
                    Exit For

                End If

            Next


            If worksheetEntry Is Nothing Then

                Throw New InvalidDataException(
                    "No worksheet was found in the selected Excel file."
                )

            End If


            Using stream As Stream =
                worksheetEntry.Open()

                Dim sheetDoc As XDocument =
                    XDocument.Load(stream)

                Dim ns As XNamespace =
                    "http://schemas.openxmlformats.org/spreadsheetml/2006/main"

                Dim rows =
                    sheetDoc.Descendants(ns + "sheetData").
                    Descendants(ns + "row")

                Dim firstRow As Boolean = True

                For Each rowElement As XElement In rows

                    Dim cellValues As New Dictionary(Of Integer, String)()

                    For Each cell As XElement In
                        rowElement.Elements(ns + "c")

                        Dim cellReference As String =
                            If(
                                cell.Attribute("r") Is Nothing,
                                String.Empty,
                                cell.Attribute("r").Value
                            )

                        If String.IsNullOrWhiteSpace(cellReference) Then
                            Continue For
                        End If

                        Dim columnIndex As Integer =
                            GetExcelColumnIndex(cellReference)

                        Dim cellType As String =
                            If(
                                cell.Attribute("t") Is Nothing,
                                String.Empty,
                                cell.Attribute("t").Value
                            )

                        Dim value As String =
                            GetXlsxCellValue(
                                cell,
                                cellType,
                                sharedStrings,
                                ns
                            )

                        cellValues(columnIndex) = value

                    Next


                    If cellValues.Count = 0 Then
                        Continue For
                    End If

                    If firstRow Then

                        Dim maxColumn As Integer =
                            cellValues.Keys.Max()

                        For i As Integer = 0 To maxColumn

                            Dim headerName As String =
                                String.Empty

                            If cellValues.ContainsKey(i) Then

                                headerName =
                                    cellValues(i).Trim()

                            End If

                            If String.IsNullOrWhiteSpace(headerName) Then

                                headerName =
                                    "Column" &
                                    (i + 1).ToString()

                            End If

                            Dim originalName As String =
                                headerName

                            Dim counter As Integer = 1

                            While dt.Columns.Contains(headerName)

                                headerName =
                                    originalName &
                                    "_" &
                                    counter.ToString()

                                counter += 1

                            End While

                            dt.Columns.Add(headerName)

                        Next

                        firstRow = False

                    Else



                        Dim row As DataRow =
                            dt.NewRow()


                        For i As Integer = 0 To dt.Columns.Count - 1

                            If cellValues.ContainsKey(i) Then

                                row(i) =
                                    cellValues(i)

                            Else

                                row(i) =
                                    String.Empty

                            End If

                        Next


                        Dim hasData As Boolean = False


                        For i As Integer = 0 To dt.Columns.Count - 1

                            If Not String.IsNullOrWhiteSpace(
                                row(i).ToString()
                            ) Then

                                hasData = True
                                Exit For

                            End If

                        Next

                        If hasData Then
                            dt.Rows.Add(row)
                        End If

                    End If

                Next

            End Using

        End Using

        Return dt

    End Function


    Private Function GetXlsxCellValue(
        cell As XElement,
        cellType As String,
        sharedStrings As List(Of String),
        ns As XNamespace
    ) As String



        If cellType = "inlineStr" Then

            Return String.Concat(
                cell.Descendants(ns + "t").
                Select(Function(t) t.Value)
            ).Trim()

        End If



        If cellType = "s" Then

            Dim valueElement As XElement =
                cell.Element(ns + "v")

            If valueElement Is Nothing Then
                Return String.Empty
            End If

            Dim index As Integer

            If Integer.TryParse(
                valueElement.Value,
                index
            ) Then

                If index >= 0 AndAlso
                   index < sharedStrings.Count Then

                    Return sharedStrings(index).Trim()

                End If

            End If

            Return String.Empty

        End If


        If cellType = "b" Then

            Dim valueElement As XElement =
                cell.Element(ns + "v")

            If valueElement Is Nothing Then
                Return String.Empty
            End If

            If valueElement.Value = "1" Then
                Return "TRUE"
            Else
                Return "FALSE"
            End If

        End If




        Dim normalValue As XElement =
            cell.Element(ns + "v")

        If normalValue IsNot Nothing Then
            Return normalValue.Value.Trim()
        End If


        Dim formulaValue As XElement =
            cell.Element(ns + "f")

        If formulaValue IsNot Nothing Then

            If normalValue IsNot Nothing Then
                Return normalValue.Value.Trim()
            End If

        End If


        Return String.Empty

    End Function


    Private Function GetExcelColumnIndex(
        cellReference As String
    ) As Integer

        Dim columnLetters As String = ""

        For Each ch As Char In cellReference

            If Char.IsLetter(ch) Then

                columnLetters &=
                    ch

            Else

                Exit For

            End If

        Next

        If String.IsNullOrWhiteSpace(columnLetters) Then
            Return 0
        End If

        Dim columnNumber As Integer = 0

        For Each ch As Char In
            columnLetters.ToUpperInvariant()

            columnNumber =
                (columnNumber * 26) +
                (AscW(ch) - AscW("A"c) + 1)

        Next

        Return columnNumber - 1

    End Function

    Public Sub refreshData()
        Dim query As String =
        "SELECT b.*, " &
        "CASE WHEN e.ID IS NOT NULL THEN 1 ELSE 0 END AS HasAccount " &
        "FROM `borrower_tbl` b " &
        "LEFT JOIN `borroweredit_tbl` e ON b.LRN = e.LRN OR b.EmployeeNo = e.EmployeeNo"

        GlobalVarsModule.AutoRefreshGrid(DataGridView1, query, 2000)
        SetupGridStyle()
        cbgradee()
        cbsecs()
        cbdepts()
        cbstrandd()
        ClearFields()
        strandlocation()
    End Sub

    Private Async Sub OnDatabaseUpdated()
        Dim query As String =
        "SELECT b.*, " &
        "CASE WHEN e.ID IS NOT NULL THEN 1 ELSE 0 END AS HasAccount " &
        "FROM `borrower_tbl` b " &
        "LEFT JOIN `borroweredit_tbl` e ON b.LRN = e.LRN OR b.EmployeeNo = e.EmployeeNo"

        Await GlobalVarsModule.LoadToGridAsync(DataGridView1, query)
        SetupGridStyle()
    End Sub

    Private Sub SetupGridStyle()
        Try
            If DataGridView1.Columns.Contains("ID") Then
                DataGridView1.Columns("ID").Visible = False
            End If
            If DataGridView1.Columns.Contains("HasAccount") Then
                DataGridView1.Columns("HasAccount").Visible = False
            End If
            DataGridView1.ClearSelection()
            DataGridView1.CurrentCell = Nothing
            DataGridView1.EnableHeadersVisualStyles = False
            DataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(207, 58, 109)
            DataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White
            ColorRows()
        Catch
        End Try
    End Sub

    Public Sub ColorRows()

        If DataGridView1.DataSource Is Nothing Then Return

        For Each row As DataGridViewRow In DataGridView1.Rows


            If Not row.IsNewRow Then


                If row.DataBoundItem IsNot Nothing Then


                    Dim dataRowView As DataRowView = TryCast(row.DataBoundItem, DataRowView)

                    If dataRowView IsNot Nothing AndAlso dataRowView.Row.Table.Columns.Contains("HasAccount") Then


                        Dim hasAccountValue As Object = dataRowView.Row("HasAccount")
                        Dim hasAccount As Integer = 0


                        If hasAccountValue IsNot DBNull.Value AndAlso hasAccountValue IsNot Nothing Then

                            hasAccount = CInt(hasAccountValue)
                        End If


                        If hasAccount = 1 Then

                            row.DefaultCellStyle.BackColor = Color.DarkSeaGreen
                            row.DefaultCellStyle.ForeColor = Color.Black
                            row.DefaultCellStyle.SelectionBackColor = Color.Green
                            row.DefaultCellStyle.SelectionForeColor = Color.White
                        Else

                            row.DefaultCellStyle.BackColor = Color.Maroon
                            row.DefaultCellStyle.ForeColor = Color.White
                            row.DefaultCellStyle.SelectionBackColor = Color.IndianRed
                            row.DefaultCellStyle.SelectionForeColor = Color.White
                        End If

                    End If
                End If
            End If
        Next
    End Sub

    Private Sub DataGridView1_DataBindingComplete(sender As Object, e As DataGridViewBindingCompleteEventArgs) Handles DataGridView1.DataBindingComplete

        ColorRows()


        For Each col As DataGridViewColumn In DataGridView1.Columns
            If col.Name = "HasAccount" Then
                col.Visible = False
                Exit For
            End If
        Next

    End Sub

    Public Sub strandlocation()

        cbstrand.Visible = True

        cbstrand.Location = New Point(942, 285)


        lblstrand.Visible = True

        lblstrand.Location = New Point(942, 266)

    End Sub

    Public Sub cbgradee()
        Dim con As New MySqlConnection(GlobalVarsModule.connectionString)
        Dim com As String = "SELECT ID, Grade FROM `grade_tbl`"
        Dim adap As New MySqlDataAdapter(com, con)
        Dim dt As New DataTable
        Try
            con.Open()
            adap.Fill(dt)
            cbgrade.DataSource = dt
            cbgrade.DisplayMember = "Grade"
            cbgrade.ValueMember = "ID"
            cbgrade.SelectedIndex = -1
        Catch ex As Exception
            MessageBox.Show("Error loading grades: " & ex.Message)
        Finally
            con.Close()
        End Try
    End Sub

    Public Sub cbsecs()
        Dim con As New MySqlConnection(GlobalVarsModule.connectionString)
        Dim com As String = "SELECT ID, Section FROM `section_tbl`"
        Dim adap As New MySqlDataAdapter(com, con)
        Dim dt As New DataTable
        Try
            con.Open()
            adap.Fill(dt)
            cbsection.DataSource = dt
            cbsection.DisplayMember = "Section"
            cbsection.ValueMember = "ID"
            cbsection.SelectedIndex = -1
        Catch ex As Exception
            MessageBox.Show("Error loading sections: " & ex.Message)
        Finally
            con.Close()
        End Try
    End Sub

    Public Sub cbdepts()
        Dim con As New MySqlConnection(GlobalVarsModule.connectionString)
        Dim com As String = "SELECT ID, Department FROM `department_tbl`"
        Dim adap As New MySqlDataAdapter(com, con)
        Dim dt As New DataTable
        Try
            con.Open()
            adap.Fill(dt)
            cbdepartment.DataSource = dt
            cbdepartment.DisplayMember = "Department"
            cbdepartment.ValueMember = "ID"
            cbdepartment.SelectedIndex = -1
        Catch ex As Exception
            MessageBox.Show("Error loading departments: " & ex.Message)
        Finally
            con.Close()
        End Try

    End Sub

    Public Sub cbstrandd()

        Dim con As New MySqlConnection(GlobalVarsModule.connectionString)
        Dim com As String = "SELECT ID, Strand FROM `strand_tbl`"
        Dim adap As New MySqlDataAdapter(com, con)
        Dim dt As New DataTable
        Try
            con.Open()
            adap.Fill(dt)
            cbstrand.DataSource = dt
            cbstrand.DisplayMember = "Strand"
            cbstrand.ValueMember = "ID"
            cbstrand.SelectedIndex = -1
        Catch ex As Exception
            MessageBox.Show("Error loading strands: " & ex.Message)
        Finally
            con.Close()
        End Try

    End Sub

    Private Sub Borrower_Shown(sender As Object, e As EventArgs) Handles MyBase.Shown

        DataGridView1.ClearSelection()

    End Sub

    Private Sub txtsearch_TextChanged(sender As Object, e As EventArgs) Handles txtsearch.TextChanged

        HandleAutoRefreshPause(DataGridView1, txtsearch)

        Dim dt As DataTable = DirectCast(DataGridView1.DataSource, DataTable)
        If dt IsNot Nothing Then
            Dim searchValue As String = txtsearch.Text.Trim()
            If Not String.IsNullOrEmpty(searchValue) Then

                Dim filter As String = String.Format("FirstName LIKE '%{0}%' OR Department LIKE '%{0}%' OR LastName LIKE '%{0}%' OR MiddleInitial LIKE '%{0}%' OR LRN LIKE '%{0}%' OR Borrower LIKE '%{0}%'", searchValue)
                dt.DefaultView.RowFilter = filter
            Else
                dt.DefaultView.RowFilter = ""
            End If
        End If

    End Sub


    Private Sub btnadd_Click_1(sender As Object, e As EventArgs) Handles btnadd.Click


        Dim con As New MySqlConnection(GlobalVarsModule.connectionString)
        Dim borrowerType As String = ""

        Dim middleInitial As String = txtmname.Text.Trim()
        Dim lrn As Object = DBNull.Value
        Dim employeeNo As Object = DBNull.Value
        Dim contactNumber As String = txtcontactnumber.Text.Trim()
        Dim firstName As String = txtfname.Text.Trim()
        Dim lastName As String = txtlname.Text.Trim()
        Dim newID As Integer = 0
        Dim fullName As String = $"{lastName}, {firstName}"

        If rbstudent.Checked Then
            borrowerType = "Student"
        ElseIf rbteacher.Checked Then
            borrowerType = "Teacher"
        End If

        If CheckBox1.Checked Then
            middleInitial = "N/A"
        End If

        If middleInitial.ToUpper() <> "N/A" AndAlso Not String.IsNullOrWhiteSpace(middleInitial) Then
            fullName = $"{lastName}, {firstName} {middleInitial}"
        End If


        If String.IsNullOrWhiteSpace(firstName) OrElse String.IsNullOrWhiteSpace(lastName) OrElse String.IsNullOrWhiteSpace(contactNumber) Then
            MsgBox("Please fill in all required fields.", vbExclamation, "Missing Information")
            Exit Sub
        End If


        If firstName.Length < 2 Then
            MsgBox("First Name must be 2 characters or more.", vbExclamation, "Input Error")
            Exit Sub
        End If

        If lastName.Length < 2 Then
            MsgBox("Last Name must be 2 characters or more.", vbExclamation, "Input Error")
            Exit Sub
        End If


        If contactNumber.Length < 11 OrElse (contactNumber.StartsWith("09") AndAlso contactNumber.Length = 2) Then
            MsgBox("Contact Number must be a valid length (e.g., 11 digits).", vbExclamation, "Invalid Contact Number")
            Exit Sub
        End If



        If borrowerType = "Student" Then
            If String.IsNullOrWhiteSpace(txtlrn.Text) Then
                MsgBox("Please enter the student's LRN.", vbExclamation, "Missing Information")
                Exit Sub
            End If
            lrn = txtlrn.Text.Trim()


            If lrn.ToString().Length <> 12 OrElse Not IsNumeric(lrn) Then
                MsgBox("LRN must be 12 digits.", vbExclamation, "Invalid LRN")
                Exit Sub
            End If

        ElseIf borrowerType = "Teacher" Then
            If String.IsNullOrWhiteSpace(txtemployeeno.Text) Then
                MsgBox("Please enter the teacher's Employee Number.", vbExclamation, "Missing Information")
                Exit Sub
            End If
            employeeNo = txtemployeeno.Text.Trim()


            If employeeNo.ToString().Length <> 8 OrElse Not IsNumeric(employeeNo) Then
                MsgBox("Employee Number must be 8 digits.", vbExclamation, "Invalid Employee Number")
                Exit Sub
            End If

        End If


        Try
            con.Open()


            Dim checkCom As New MySqlCommand("SELECT COUNT(*) FROM `borrower_tbl` WHERE (`LRN` = @LRN AND `LRN` IS NOT NULL) OR (`EmployeeNo` = @EmployeeNo AND `EmployeeNo` IS NOT NULL) OR `ContactNumber` = @ContactNumber", con)
            checkCom.Parameters.AddWithValue("@LRN", If(lrn Is DBNull.Value, "", lrn))
            checkCom.Parameters.AddWithValue("@EmployeeNo", If(employeeNo Is DBNull.Value, "", employeeNo))
            checkCom.Parameters.AddWithValue("@ContactNumber", contactNumber)

            Dim count As Integer = Convert.ToInt32(checkCom.ExecuteScalar())

            If count > 0 Then
                MsgBox("LRN, Employee Number, or Contact Number already exists. Please use a different one.", vbExclamation, "Duplication Not Allowed")
                con.Close()
                Exit Sub
            End If


            Dim com As New MySqlCommand("INSERT INTO `borrower_tbl`(`Borrower`, `FirstName`, `LastName`, `MiddleInitial`, `LRN`, `EmployeeNo`, `ContactNumber`, `Department`, `Grade`, `Section`, `Strand`) VALUES (@Borrower, @FirstName, @LastName, @MiddleInitial, @LRN, @EmployeeNo, @ContactNumber, @Department, @Grade, @Section, @Strand); SELECT LAST_INSERT_ID();", con)

            com.Parameters.AddWithValue("@Borrower", borrowerType)
            com.Parameters.AddWithValue("@FirstName", firstName)
            com.Parameters.AddWithValue("@LastName", lastName)
            com.Parameters.AddWithValue("@MiddleInitial", middleInitial)
            com.Parameters.AddWithValue("@LRN", lrn)
            com.Parameters.AddWithValue("@EmployeeNo", employeeNo)
            com.Parameters.AddWithValue("@ContactNumber", contactNumber)
            com.Parameters.AddWithValue("@Department", cbdepartment.Text.Trim())
            com.Parameters.AddWithValue("@Grade", cbgrade.Text.Trim())
            com.Parameters.AddWithValue("@Section", cbsection.Text.Trim())
            com.Parameters.AddWithValue("@Strand", cbstrand.Text.Trim())

            newID = Convert.ToInt32(com.ExecuteScalar())

            GlobalVarsModule.LogAudit(
            actionType:="ADD",
            formName:="BORROWER FORM",
            description:=$"Added new {borrowerType}: {fullName}",
            recordID:=newID.ToString()
            )

            For Each form In Application.OpenForms
                If TypeOf form Is AuditTrail Then
                    DirectCast(form, AuditTrail).refreshaudit()
                End If
            Next

            MsgBox("Borrower added successfully!", vbInformation)
            Borrower_Load(sender, e)
            ClearFields()

            Dim registeredForm As RegisteredBrwr = Application.OpenForms.OfType(Of RegisteredBrwr)().FirstOrDefault()
            If registeredForm IsNot Nothing Then
                registeredForm.ludeyngborrower()
            End If

            cbstrand.Visible = True
            cbstrand.Location = New Point(942, 285)
            lblstrand.Visible = True
            lblstrand.Location = New Point(942, 266)

        Catch ex As Exception
            MessageBox.Show("Error adding borrower: " & ex.Message)
        Finally
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
        End Try

    End Sub


    Private Sub btnedit_Click_1(sender As Object, e As EventArgs) Handles btnedit.Click

        If DataGridView1.SelectedRows.Count > 0 Then



            Dim selectedRow As DataGridViewRow = DataGridView1.SelectedRows(0)
            Dim ID As Integer = Convert.ToInt32(selectedRow.Cells("ID").Value)

            Dim con As New MySqlConnection(GlobalVarsModule.connectionString)

            Dim originalBorrowerType As String = selectedRow.Cells("Borrower").Value.ToString().Trim()

            Dim oldFirstName As String = selectedRow.Cells("FirstName").Value.ToString().Trim()
            Dim oldLastName As String = selectedRow.Cells("LastName").Value.ToString().Trim()
            Dim oldMiddleInitialValue As Object = selectedRow.Cells("MiddleInitial").Value
            Dim oldMiddleInitial As String = If(oldMiddleInitialValue Is DBNull.Value OrElse oldMiddleInitialValue Is Nothing, "", oldMiddleInitialValue.ToString().Trim())
            Dim oldLRN As Object = selectedRow.Cells("LRN").Value
            Dim oldEmployeeNo As Object = selectedRow.Cells("EmployeeNo").Value
            Dim oldContactNumber As String = selectedRow.Cells("ContactNumber").Value.ToString().Trim()

            Dim oldFullNameInGrid As String = $"{oldLastName}, {oldFirstName}"
            If oldMiddleInitial.ToUpper() <> "N/A" AndAlso Not String.IsNullOrWhiteSpace(oldMiddleInitial) Then
                oldFullNameInGrid = $"{oldLastName}, {oldFirstName} {oldMiddleInitial}"
            End If


            Dim borrowerType As String = ""
            Dim middleInitial As String = txtmname.Text.Trim()
            Dim lrn As Object = DBNull.Value
            Dim employeeNo As Object = DBNull.Value
            Dim contactNumber As String = txtcontactnumber.Text.Trim()
            Dim firstName As String = txtfname.Text.Trim()
            Dim lastName As String = txtlname.Text.Trim()

            Dim newlrn As String = txtlrn.Text.Trim()
            Dim newemployeeno As String = txtemployeeno.Text.Trim()


            Dim newFullName As String = $"{lastName}, {firstName}"
            If Not CheckBox1.Checked AndAlso Not String.IsNullOrWhiteSpace(middleInitial) Then
                newFullName = $"{lastName}, {firstName} {middleInitial}"
            End If


            If rbstudent.Checked Then
                borrowerType = "Student"
            ElseIf rbteacher.Checked Then
                borrowerType = "Teacher"
            End If

            If CheckBox1.Checked Then
                middleInitial = "N/A"
            End If

            If String.IsNullOrWhiteSpace(firstName) OrElse String.IsNullOrWhiteSpace(lastName) OrElse String.IsNullOrWhiteSpace(contactNumber) Then
                MsgBox("Please fill in all required fields.", vbExclamation, "Missing Information")
                Exit Sub
            End If

            If firstName.Length < 2 Then
                MsgBox("First Name must be 2 characters or more.", vbExclamation, "Input Error")
                Exit Sub
            End If

            If lastName.Length < 2 Then
                MsgBox("Last Name must be 2 characters or more.", vbExclamation, "Input Error")
                Exit Sub
            End If


            If contactNumber.Length < 11 OrElse (contactNumber.StartsWith("09") AndAlso contactNumber.Length = 2) Then
                MsgBox("Contact Number must be a valid length (e.g., 11 digits).", vbExclamation, "Invalid Contact Number")
                Exit Sub
            End If



            If borrowerType = "Student" Then
                If String.IsNullOrWhiteSpace(txtlrn.Text) Then
                    MsgBox("Please enter the student's LRN.", vbExclamation, "Missing Information")
                    Exit Sub
                End If
                lrn = txtlrn.Text.Trim()
                employeeNo = DBNull.Value


                If lrn.ToString().Length <> 12 OrElse Not IsNumeric(lrn) Then
                    MsgBox("LRN must be 12 digits.", vbExclamation, "Invalid LRN")
                    Exit Sub
                End If

            ElseIf borrowerType = "Teacher" Then
                If String.IsNullOrWhiteSpace(txtemployeeno.Text) Then
                    MsgBox("Please enter the teacher's Employee Number.", vbExclamation, "Missing Information")
                    Exit Sub
                End If
                employeeNo = txtemployeeno.Text.Trim()
                lrn = DBNull.Value


                If employeeNo.ToString().Length <> 8 OrElse Not IsNumeric(employeeNo) Then
                    MsgBox("Employee Number must be 8 digits.", vbExclamation, "Invalid Employee Number")
                    Exit Sub
                End If

            End If

            Try
                con.Open()


                If originalBorrowerType <> borrowerType Then
                    Dim checkKey As String = ""
                    Dim keyColumn As String = ""

                    If originalBorrowerType = "Student" Then
                        Dim oldlrnCell As Object = selectedRow.Cells("LRN").Value
                        If oldlrnCell IsNot DBNull.Value AndAlso oldlrnCell IsNot Nothing Then
                            checkKey = oldlrnCell.ToString().Trim()
                            keyColumn = "LRN"
                        End If
                    ElseIf originalBorrowerType = "Teacher" Then
                        Dim oldemployeenoCell As Object = selectedRow.Cells("EmployeeNo").Value
                        If oldemployeenoCell IsNot DBNull.Value AndAlso oldemployeenoCell IsNot Nothing Then
                            checkKey = oldemployeenoCell.ToString().Trim()
                            keyColumn = "EmployeeNo"
                        End If
                    End If

                    If Not String.IsNullOrWhiteSpace(checkKey) Then
                        Dim cheyk As New MySqlCommand($"SELECT COUNT(*) FROM `borroweredit_tbl` WHERE `{keyColumn}` = @CheckKey", con)
                        cheyk.Parameters.AddWithValue("@CheckKey", checkKey)
                        Dim existingRecordCount As Integer = Convert.ToInt32(cheyk.ExecuteScalar())

                        If existingRecordCount > 0 Then
                            MsgBox($"Cannot change borrower type from {originalBorrowerType} to {borrowerType}, because this {originalBorrowerType} has existing accountt in Borrowing Edit Info.", vbExclamation, "Type Change Denied")
                            con.Close()
                            Exit Sub
                        End If
                    End If
                End If

                Dim coms As New MySqlCommand("SELECT COUNT(*) FROM `borrower_tbl` WHERE ((`LRN` = @LRN AND `LRN` IS NOT NULL) OR (`EmployeeNo` = @EmployeeNo AND `EmployeeNo` IS NOT NULL) OR `ContactNumber` = @ContactNumber) AND `ID` <> @ID", con)

                coms.Parameters.AddWithValue("@LRN", If(lrn Is DBNull.Value, "", lrn))
                coms.Parameters.AddWithValue("@EmployeeNo", If(employeeNo Is DBNull.Value, "", employeeNo))
                coms.Parameters.AddWithValue("@ContactNumber", contactNumber)
                coms.Parameters.AddWithValue("@ID", ID)

                If Convert.ToInt32(coms.ExecuteScalar()) > 0 Then
                    MsgBox("LRN, Employee Number, or Contact Number already exists.", vbExclamation, "Duplication Not Allowed")
                    con.Close()
                    Exit Sub
                End If


                Dim com As New MySqlCommand("UPDATE `borrower_tbl` SET `Borrower`=@Borrower, `FirstName`=@FirstName, `LastName`=@LastName, `MiddleInitial`=@MiddleInitial, `LRN`=@LRN, `EmployeeNo`=@EmployeeNo, `ContactNumber`=@ContactNumber, `Department`=@Department, `Grade`=@Grade, `Section`=@Section, `Strand`=@Strand WHERE `ID`=@ID", con)

                com.Parameters.AddWithValue("@Borrower", borrowerType)
                com.Parameters.AddWithValue("@FirstName", firstName)
                com.Parameters.AddWithValue("@LastName", lastName)
                com.Parameters.AddWithValue("@MiddleInitial", middleInitial)
                com.Parameters.AddWithValue("@LRN", lrn)
                com.Parameters.AddWithValue("@EmployeeNo", employeeNo)
                com.Parameters.AddWithValue("@ContactNumber", contactNumber)
                com.Parameters.AddWithValue("@Department", cbdepartment.Text.Trim())
                com.Parameters.AddWithValue("@Grade", cbgrade.Text.Trim())
                com.Parameters.AddWithValue("@Section", cbsection.Text.Trim())
                com.Parameters.AddWithValue("@Strand", cbstrand.Text.Trim())
                com.Parameters.AddWithValue("@ID", ID)
                com.ExecuteNonQuery()

                Dim auditDescription As String = $"Updated {originalBorrowerType} details for {oldFullNameInGrid}. New Name: {newFullName}"

                Dim oldValueLog As String = $"Name: {oldFullNameInGrid}, Type: {originalBorrowerType}, LRN: {If(oldLRN Is DBNull.Value, "N/A", oldLRN)}, EmployeeNo: {If(oldEmployeeNo Is DBNull.Value, "N/A", oldEmployeeNo)}, Contact: {oldContactNumber}"
                Dim newValueLog As String = $"Name: {newFullName}, Type: {borrowerType}, LRN: {If(lrn Is DBNull.Value, "N/A", lrn)}, EmployeeNo: {If(employeeNo Is DBNull.Value, "N/A", employeeNo)}, Contact: {contactNumber}"

                GlobalVarsModule.LogAudit(
                actionType:="UPDATE",
                formName:="BORROWER FORM",
                description:=auditDescription,
                recordID:=ID.ToString(),
                oldValue:=oldValueLog,
                newValue:=newValueLog
                )

                For Each form In Application.OpenForms
                    If TypeOf form Is AuditTrail Then
                        DirectCast(form, AuditTrail).refreshaudit()
                    End If
                Next


                Dim timeInOutRecordUpdateCom As String = "UPDATE `timeinoutrecord_tbl` SET `FullName` = @NewFullName WHERE `FullName` = @OldFullName AND `Borrower` = @Type"

                Using updateTimeInOutRecordCmd As New MySqlCommand(timeInOutRecordUpdateCom, con)
                    updateTimeInOutRecordCmd.Parameters.AddWithValue("@NewFullName", newFullName)
                    updateTimeInOutRecordCmd.Parameters.AddWithValue("@OldFullName", oldFullNameInGrid)
                    updateTimeInOutRecordCmd.Parameters.AddWithValue("@Type", originalBorrowerType)

                    updateTimeInOutRecordCmd.ExecuteNonQuery()
                End Using

                Dim tablesToUpdate As New List(Of String) From {"borroweredit_tbl", "oras_tbl", "borrowing_tbl"}
                Dim oldlrnForUpdate As String = ""
                Dim oldemployeenoForUpdate As String = ""


                If selectedRow.Cells("LRN").Value IsNot DBNull.Value AndAlso selectedRow.Cells("LRN").Value IsNot Nothing Then
                    oldlrnForUpdate = selectedRow.Cells("LRN").Value.ToString().Trim()
                End If
                If selectedRow.Cells("EmployeeNo").Value IsNot DBNull.Value AndAlso selectedRow.Cells("EmployeeNo").Value IsNot Nothing Then
                    oldemployeenoForUpdate = selectedRow.Cells("EmployeeNo").Value.ToString().Trim()
                End If

                If borrowerType = "Student" Then
                    If oldlrnForUpdate <> newlrn AndAlso Not String.IsNullOrWhiteSpace(oldlrnForUpdate) AndAlso Not String.IsNullOrWhiteSpace(newlrn) Then
                        For Each tableName As String In tablesToUpdate
                            Dim comLrnUpdate As New MySqlCommand($"UPDATE `{tableName}` SET `LRN` = @newVal WHERE `LRN` = @oldVal", con)
                            comLrnUpdate.Parameters.AddWithValue("@newVal", newlrn)
                            comLrnUpdate.Parameters.AddWithValue("@oldVal", oldlrnForUpdate)
                            comLrnUpdate.ExecuteNonQuery()
                        Next
                    End If
                ElseIf borrowerType = "Teacher" Then
                    If oldemployeenoForUpdate <> newemployeeno AndAlso Not String.IsNullOrWhiteSpace(oldemployeenoForUpdate) AndAlso Not String.IsNullOrWhiteSpace(newemployeeno) Then
                        For Each tableName As String In tablesToUpdate
                            Dim comEmpUpdate As New MySqlCommand($"UPDATE `{tableName}` SET `EmployeeNo` = @newVal WHERE `EmployeeNo` = @oldVal", con)
                            comEmpUpdate.Parameters.AddWithValue("@newVal", newemployeeno)
                            comEmpUpdate.Parameters.AddWithValue("@oldVal", oldemployeenoForUpdate)
                            comEmpUpdate.ExecuteNonQuery()
                        Next
                    End If
                End If


                For Each form In Application.OpenForms
                    If TypeOf form Is Borrowereditsinfo Then
                        Dim brwr = DirectCast(form, Borrowereditsinfo)
                        brwr.refresheditt()
                    End If
                Next

                Dim timeInOutForm As Form = Application.OpenForms.OfType(Of TimeInOutRecord)().FirstOrDefault()
                If timeInOutForm IsNot Nothing Then
                    DirectCast(timeInOutForm, TimeInOutRecord).refreshtimeoutrecrod()
                End If

                MsgBox("Borrower updated successfully!", vbInformation)
                Borrower_Load(sender, e)
                ClearFields()

                Dim registeredForm As RegisteredBrwr = Application.OpenForms.OfType(Of RegisteredBrwr)().FirstOrDefault()
                If registeredForm IsNot Nothing Then
                    registeredForm.ludeyngborrower()
                End If


                cbstrand.Visible = True
                cbstrand.Location = New Point(942, 285)
                lblstrand.Visible = True
                lblstrand.Location = New Point(942, 266)

            Catch ex As Exception
                MessageBox.Show("Error updating borrower: " & ex.Message)
            Finally
                If con.State = ConnectionState.Open Then
                    con.Close()
                End If
            End Try
        Else
            MsgBox("Please select a row to edit.", vbExclamation)
        End If

    End Sub


    Private Sub btndelete_Click_1(sender As Object, e As EventArgs) Handles btndelete.Click

        If DataGridView1.SelectedRows.Count = 0 Then
            MessageBox.Show("Please select a borrower to delete.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Dim selectedRow As DataGridViewRow = DataGridView1.SelectedRows(0)
        Dim hasAccount As Integer = 0
        Dim fullNameToDelete As String = ""
        Dim borrowerTypeToDelete As String = ""

        Try

            Dim cellValue As Object = selectedRow.Cells.Item("HasAccount").Value
            Dim firstName As String = selectedRow.Cells("FirstName").Value.ToString().Trim()
            Dim lastName As String = selectedRow.Cells("LastName").Value.ToString().Trim()
            Dim middleInitialValue As Object = selectedRow.Cells("MiddleInitial").Value
            Dim middleInitial As String = If(middleInitialValue Is DBNull.Value OrElse middleInitialValue Is Nothing, "", middleInitialValue.ToString().Trim())

            borrowerTypeToDelete = selectedRow.Cells("Borrower").Value.ToString().Trim()
            fullNameToDelete = $"{lastName}, {firstName}"
            If middleInitial.ToUpper() <> "N/A" AndAlso Not String.IsNullOrWhiteSpace(middleInitial) Then
                fullNameToDelete = $"{lastName}, {firstName} {middleInitial}"
            End If

            If cellValue IsNot DBNull.Value AndAlso cellValue IsNot Nothing Then

                hasAccount = Convert.ToInt32(cellValue)
            End If

        Catch ex As Exception

            hasAccount = 0
        End Try

        If hasAccount = 1 Then
            MessageBox.Show("This borrower has an existing account.", "Deletion Blocked", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If


        Dim dialogResult As DialogResult = MessageBox.Show("Are you sure you want to delete this borrower?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)

        If dialogResult = DialogResult.Yes Then

            Dim con As New MySqlConnection(GlobalVarsModule.connectionString)

            Dim ID As Integer = CInt(selectedRow.Cells("ID").Value)

            Try
                con.Open()


                Dim delete As New MySqlCommand("DELETE FROM `borrower_tbl` WHERE `ID` = @id", con)
                delete.Parameters.AddWithValue("@id", ID)
                delete.ExecuteNonQuery()

                GlobalVarsModule.LogAudit(
                actionType:="DELETE",
                formName:="BORROWER FORM",
                description:=$"Deleted {borrowerTypeToDelete}: {fullNameToDelete}",
                recordID:=ID.ToString()
            )

                For Each form In Application.OpenForms
                    If TypeOf form Is AuditTrail Then
                        DirectCast(form, AuditTrail).refreshaudit()
                    End If
                Next

                MessageBox.Show("Borrower deleted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)


                Borrower_Load(sender, e)
                ClearFields()


                Dim registeredForm As RegisteredBrwr = Application.OpenForms.OfType(Of RegisteredBrwr)().FirstOrDefault()
                If registeredForm IsNot Nothing Then
                    registeredForm.ludeyngborrower()
                End If


                cbstrand.Visible = True
                cbstrand.Location = New Point(942, 285)
                lblstrand.Visible = True
                lblstrand.Location = New Point(942, 266)

                Dim count As New MySqlCommand("SELECT COUNT(*) FROM `borrower_tbl`", con)
                Dim rowCount As Long = CLng(count.ExecuteScalar())

                If rowCount = 0 Then
                    Dim reset As New MySqlCommand("ALTER TABLE `borrower_tbl` AUTO_INCREMENT = 1", con)
                    reset.ExecuteNonQuery()
                End If

            Catch ex As Exception
                MessageBox.Show("Error deleting borrower: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Finally
                If con.State = ConnectionState.Open Then
                    con.Close()
                End If
            End Try
        End If

    End Sub


    Private Sub DataGridView1_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellClick

        If e.RowIndex >= 0 Then

            Dim row As DataGridViewRow = DataGridView1.Rows(e.RowIndex)
            Dim borrowerType As String = row.Cells("Borrower").Value.ToString()


            If Not DataGridView1.Columns.Contains("MiddleInitial") Then
                Exit Sub
            End If


            cbdepartment.Text = row.Cells("Department").Value.ToString()
            cbdepartment_SelectedIndexChanged(cbdepartment, EventArgs.Empty)

            If borrowerType = "Student" Then
                rbstudent.Checked = True

                If cbdepartment.Text = "Senior High School" Then
                    cbstrand.Visible = True
                    lblstrand.Visible = True
                Else
                    cbstrand.Visible = False
                    lblstrand.Visible = False
                End If

            ElseIf borrowerType = "Teacher" Then
                rbteacher.Checked = True

                txtemployeeno.Text = If(IsDBNull(row.Cells("EmployeeNo").Value), String.Empty, row.Cells("EmployeeNo").Value.ToString())
                txtlrn.Text = ""

                cbstrand.Visible = False
                lblstrand.Visible = False

            End If


            txtfname.Text = row.Cells("FirstName").Value.ToString()


            Dim middleInitial As String = row.Cells("MiddleInitial").Value.ToString().Trim().ToUpper()


            If middleInitial = "N/A" OrElse String.IsNullOrWhiteSpace(middleInitial) Then

                CheckBox1.Checked = False
                txtmname.Text = ""

            Else

                CheckBox1.Checked = False
                txtmname.Text = middleInitial

            End If


            txtlname.Text = row.Cells("LastName").Value.ToString()
            txtlrn.Text = If(IsDBNull(row.Cells("LRN").Value), "", row.Cells("LRN").Value.ToString())
            txtcontactnumber.Text = row.Cells("ContactNumber").Value.ToString()
            cbdepartment.Text = row.Cells("Department").Value.ToString()
            cbgrade.Text = row.Cells("Grade").Value.ToString()
            cbsection.Text = row.Cells("Section").Value.ToString()
            cbstrand.Text = row.Cells("Strand").Value.ToString()


        End If

    End Sub

    Private Sub cbdepartment_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbdepartment.SelectedIndexChanged


        cbgrade.Enabled = False
        cbgrade.SelectedIndex = -1
        cbsection.Enabled = False
        cbsection.SelectedIndex = -1
        cbstrand.Enabled = False
        cbstrand.SelectedIndex = -1


        cbgrade.Visible = True
        lblgrade.Visible = True
        cbsection.Visible = True
        lblsection.Visible = True
        cbstrand.Visible = True
        lblstrand.Visible = True

        If cbdepartment.SelectedIndex <> -1 Then
            Dim selectedDept As String = cbdepartment.GetItemText(cbdepartment.SelectedItem)


            If rbteacher.Checked Then
                cbgrade.Enabled = False
                cbgrade.Visible = False
                lblgrade.Visible = False
                cbsection.Enabled = False
                cbsection.Visible = False
                lblsection.Visible = False
                cbstrand.Enabled = False
                cbstrand.Visible = False
                lblstrand.Visible = False
                Return
            End If


            Select Case selectedDept
                Case "Junior High School"
                    Dim con As New MySqlConnection(connectionString)
                    Dim dt As New DataTable
                    Try
                        con.Open()
                        Dim adap As New MySqlDataAdapter("SELECT ID, Grade FROM `grade_tbl` WHERE Grade IN ('7', '8', '9', '10')", con)
                        adap.Fill(dt)
                        cbgrade.DataSource = dt
                        cbgrade.DisplayMember = "Grade"
                        cbgrade.ValueMember = "ID"
                        cbgrade.SelectedIndex = -1

                        cbgrade.Enabled = True
                        cbsection.Visible = True
                        lblsection.Visible = True
                        cbstrand.Visible = False
                        lblstrand.Visible = False
                    Catch ex As Exception
                        MessageBox.Show("Error filtering JHS grades: " & ex.Message)
                    End Try

                Case "Senior High School"
                    Dim con As New MySqlConnection(connectionString)
                    Dim dt As New DataTable
                    Try
                        con.Open()
                        Dim adap As New MySqlDataAdapter("SELECT ID, Grade FROM `grade_tbl` WHERE Grade IN ('11', '12')", con)
                        adap.Fill(dt)
                        cbgrade.DataSource = dt
                        cbgrade.DisplayMember = "Grade"
                        cbgrade.ValueMember = "ID"
                        cbgrade.SelectedIndex = -1

                        cbgrade.Enabled = True
                        cbsection.Visible = False
                        lblsection.Visible = False
                        cbstrand.Visible = True
                        lblstrand.Visible = True
                        cbstrand.Location = New Point(942, 216)
                        lblstrand.Location = New Point(942, 197)
                    Catch ex As Exception
                        MessageBox.Show("Error filtering SHS grades: " & ex.Message)
                    End Try

                Case "Elementary"
                    Dim con As New MySqlConnection(connectionString)
                    Dim dt As New DataTable
                    Try
                        con.Open()
                        Dim adap As New MySqlDataAdapter("SELECT ID, Grade FROM `grade_tbl` WHERE Grade IN ('1', '2', '3', '4', '5', '6')", con)
                        adap.Fill(dt)
                        cbgrade.DataSource = dt
                        cbgrade.DisplayMember = "Grade"
                        cbgrade.ValueMember = "ID"
                        cbgrade.SelectedIndex = -1

                        cbgrade.Enabled = True
                        cbsection.Visible = True
                        lblsection.Visible = True
                        cbstrand.Visible = False
                        lblstrand.Visible = False
                    Catch ex As Exception
                        MessageBox.Show("Error filtering Elementary grades: " & ex.Message)
                    End Try
            End Select
        End If
    End Sub

    Private Sub cbsection_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbsection.SelectedIndexChanged

        If cbsection.SelectedIndex <> -1 AndAlso cbgrade.SelectedIndex <> -1 Then
            cbstrand.Enabled = True
        Else
            cbstrand.Enabled = False
        End If
    End Sub

    Private Sub cbgrade_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbgrade.SelectedIndexChanged

        cbsection.Enabled = False
        cbsection.SelectedIndex = -1
        cbstrand.Enabled = False
        cbstrand.SelectedIndex = -1

        If cbgrade.SelectedIndex <> -1 Then
            Dim selectedGrade As String = cbgrade.GetItemText(cbgrade.SelectedItem)
            Dim selectedDept As String = cbdepartment.GetItemText(cbdepartment.SelectedItem)

            Select Case selectedDept
                Case "Junior High School"
                    cbsection.Enabled = True
                    Dim con As New MySqlConnection(connectionString)
                    Dim dt As New DataTable
                    Try
                        con.Open()
                        Dim com As New MySqlCommand("SELECT ID, Section FROM `section_tbl` WHERE Department = 'Junior High School' AND GradeLevel = @grade", con)
                        com.Parameters.AddWithValue("@grade", selectedGrade)
                        Dim adap As New MySqlDataAdapter(com)
                        adap.Fill(dt)
                        cbsection.DataSource = dt
                        cbsection.DisplayMember = "Section"
                        cbsection.ValueMember = "ID"
                        cbsection.SelectedIndex = -1
                        cbstrand.Enabled = False
                    Catch ex As Exception
                        MessageBox.Show("Error loading sections: " & ex.Message)
                    Finally
                        con.Close()
                    End Try

                Case "Senior High School"
                    cbstrand.Enabled = True
                    Dim con As New MySqlConnection(connectionString)
                    Dim dt As New DataTable
                    Try
                        con.Open()
                        Dim com As New MySqlCommand("SELECT ID, Strand FROM `section_tbl` WHERE Department = 'Senior High School' AND GradeLevel = @grade", con)
                        com.Parameters.AddWithValue("@grade", selectedGrade)
                        Dim adap As New MySqlDataAdapter(com)
                        adap.Fill(dt)
                        cbstrand.DataSource = dt
                        cbstrand.DisplayMember = "Strand"
                        cbstrand.ValueMember = "ID"
                        cbstrand.SelectedIndex = -1
                        cbsection.Enabled = False
                    Catch ex As Exception
                        MessageBox.Show("Error loading strands: " & ex.Message)
                    Finally
                        con.Close()
                    End Try

                Case "Elementary"
                    cbsection.Enabled = True
                    Dim con As New MySqlConnection(connectionString)
                    Dim dt As New DataTable
                    Try
                        con.Open()
                        Dim com As New MySqlCommand("SELECT ID, Section FROM `section_tbl` WHERE Department = 'Elementary' AND GradeLevel = @grade", con)
                        com.Parameters.AddWithValue("@grade", selectedGrade)
                        Dim adap As New MySqlDataAdapter(com)
                        adap.Fill(dt)
                        cbsection.DataSource = dt
                        cbsection.DisplayMember = "Section"
                        cbsection.ValueMember = "ID"
                        cbsection.SelectedIndex = -1
                        cbstrand.Enabled = False
                    Catch ex As Exception
                        MessageBox.Show("Error loading sections for Elementary: " & ex.Message)
                    Finally
                        con.Close()
                    End Try
            End Select
        End If
    End Sub

    Private Sub rbstudent_CheckedChanged(sender As Object, e As EventArgs) Handles rbstudent.CheckedChanged

        If rbstudent.Checked Then


            txtlrn.Visible = True
            txtlrn.Enabled = True
            txtemployeeno.Visible = False


            lblborrowertype.Text = "LRN:"


            cbdepartment.Enabled = True
            cbdepartment.DataSource = Nothing
            cbdepts()

            cbgrade.Enabled = False
            cbgrade.Visible = True
            lblgrade.Visible = True

            cbsection.Enabled = False
            cbsection.Visible = True
            lblsection.Visible = True

            cbstrand.Enabled = False
            cbstrand.Visible = True
            lblstrand.Visible = True

            cbstrand.Location = New Point(942, 285)
            lblstrand.Location = New Point(942, 266)


            txtfname.Enabled = True
            txtlname.Enabled = True
            txtmname.Enabled = True
            txtcontactnumber.Enabled = True
            CheckBox1.Enabled = True
            CheckBox1.Checked = False

        End If

    End Sub

    Private Sub rbteacher_CheckedChanged(sender As Object, e As EventArgs) Handles rbteacher.CheckedChanged

        If rbteacher.Checked Then


            txtlrn.Visible = False
            txtlrn.Enabled = False
            txtemployeeno.Visible = True


            lblborrowertype.Text = "Employee no.:"


            cbdepartment.Enabled = True
            cbdepartment.DataSource = Nothing
            cbdepts()


            cbgrade.Enabled = False
            cbgrade.Visible = False
            lblgrade.Visible = False

            cbsection.Enabled = False
            cbsection.Visible = False
            lblsection.Visible = False

            cbstrand.Enabled = False
            cbstrand.Visible = False
            lblstrand.Visible = False


            txtfname.Enabled = True
            txtlname.Enabled = True
            txtmname.Enabled = True
            txtemployeeno.Enabled = True
            txtcontactnumber.Enabled = True
            'rbnone.Enabled = True
            'rbnone.Checked = False

            CheckBox1.Enabled = True
            CheckBox1.Checked = False

        End If

    End Sub

    Private Sub chk1_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox1.CheckedChanged

        If CheckBox1.Checked Then
            txtmname.Enabled = False
            txtmname.Text = ""
        Else
            txtmname.Enabled = True
        End If

    End Sub

    Private Sub ClearFields()

        txtemployeeno.Text = ""
        txtfname.Text = ""
        txtmname.Text = ""
        txtlname.Text = ""
        txtlrn.Text = ""
        txtcontactnumber.Text = ""
        DataGridView1.ClearSelection()

        txtlrn.Visible = True
        txtemployeeno.Visible = False

        cbdepartment.Visible = True
        cbgrade.Visible = True
        cbsection.Visible = True
        lblsection.Visible = True
        cbstrand.Visible = True
        lblstrand.Visible = True

        cbdepartment.Enabled = False
        cbgrade.Enabled = False
        cbsection.Enabled = False
        cbstrand.Enabled = False


        cbdepartment.DataSource = Nothing
        cbgrade.DataSource = Nothing
        cbsection.DataSource = Nothing
        cbstrand.DataSource = Nothing

        cbdepts()
        cbsecs()
        cbgradee()
        cbstrandd()


        rbstudent.Checked = False
        rbteacher.Checked = False
        CheckBox1.Checked = False
        txtmname.Enabled = True
        lblborrowertype.Text = "LRN:"

        txtfname.Enabled = False
        txtlname.Enabled = False
        txtmname.Enabled = False
        txtlrn.Enabled = False
        txtcontactnumber.Enabled = False
        CheckBox1.Enabled = False

    End Sub


    Private Sub txtfname_KeyDown(sender As Object, e As KeyEventArgs) Handles txtfname.KeyDown

        If e.Control AndAlso (e.KeyCode = Keys.V Or e.KeyCode = Keys.C Or e.KeyCode = Keys.X) Then
            e.SuppressKeyPress = True
        End If

        If e.KeyCode = Keys.Back Then

            isBackspacing = True
        End If
    End Sub

    Private Sub txtfname_KeyPress(sender As Object, e As KeyPressEventArgs) Handles txtfname.KeyPress

        If Not Char.IsLetter(e.KeyChar) AndAlso Not Char.IsControl(e.KeyChar) AndAlso Not Char.IsWhiteSpace(e.KeyChar) Then
            e.Handled = True
        End If

    End Sub

    Private Sub txtlname_KeyDown(sender As Object, e As KeyEventArgs) Handles txtlname.KeyDown

        If e.Control AndAlso (e.KeyCode = Keys.V Or e.KeyCode = Keys.C Or e.KeyCode = Keys.X) Then
            e.SuppressKeyPress = True
        End If

    End Sub

    Private Sub txtlname_KeyPress(sender As Object, e As KeyPressEventArgs) Handles txtlname.KeyPress

        If Not Char.IsLetter(e.KeyChar) AndAlso Not Char.IsControl(e.KeyChar) AndAlso Not Char.IsWhiteSpace(e.KeyChar) Then
            e.Handled = True
        End If

    End Sub

    Private Sub txtmname_KeyDown(sender As Object, e As KeyEventArgs) Handles txtmname.KeyDown

        If e.Control AndAlso (e.KeyCode = Keys.V Or e.KeyCode = Keys.C Or e.KeyCode = Keys.X) Then
            e.SuppressKeyPress = True
        End If

    End Sub


    Private Sub txtlrn_KeyDown(sender As Object, e As KeyEventArgs) Handles txtlrn.KeyDown

        If e.Control AndAlso (e.KeyCode = Keys.V Or e.KeyCode = Keys.C Or e.KeyCode = Keys.X) Then
            e.SuppressKeyPress = True
        End If

    End Sub

    Private Sub txtlrn_KeyPress(sender As Object, e As KeyPressEventArgs) Handles txtlrn.KeyPress

        If Not Char.IsDigit(e.KeyChar) And Not Char.IsControl(e.KeyChar) Then
            e.Handled = True
        End If

    End Sub

    Private Sub txtcontactnumber_KeyDown(sender As Object, e As KeyEventArgs) Handles txtcontactnumber.KeyDown

        If e.Control AndAlso (e.KeyCode = Keys.V Or e.KeyCode = Keys.C Or e.KeyCode = Keys.X) Then
            e.SuppressKeyPress = True
        End If

        If e.KeyCode = Keys.Back Then
            RemoveHandler txtcontactnumber.TextChanged, AddressOf txtcontactnumber_TextChanged
        End If

    End Sub

    Private Sub txtcontactnumber_KeyPress(sender As Object, e As KeyPressEventArgs) Handles txtcontactnumber.KeyPress


        If Not Char.IsDigit(e.KeyChar) And Not Char.IsControl(e.KeyChar) Then
            e.Handled = True
        End If

    End Sub

    Private Sub txtcontactnumber_TextChanged(sender As Object, e As EventArgs) Handles txtcontactnumber.TextChanged

        Dim oreyjeynal = txtcontactnumber.Text

        If String.IsNullOrEmpty(oreyjeynal) Then
            Return
        End If

        If oreyjeynal.StartsWith("09") Then

        Else

            If oreyjeynal.Length > 0 Then
                txtcontactnumber.Clear()
                txtcontactnumber.Text = "09"
                txtcontactnumber.SelectionStart = 2
            End If
        End If

    End Sub

    Private Sub txtsearch_KeyDown(sender As Object, e As KeyEventArgs) Handles txtsearch.KeyDown

        If e.Control AndAlso (e.KeyCode = Keys.V Or e.KeyCode = Keys.C Or e.KeyCode = Keys.X) Then
            e.SuppressKeyPress = True
        End If

    End Sub

    Private Sub Borrower_FormClosed(sender As Object, e As FormClosedEventArgs) Handles Me.FormClosed
        ClearFields()
    End Sub

    Private Sub txtcontactnumber_KeyUp(sender As Object, e As KeyEventArgs) Handles txtcontactnumber.KeyUp


        If e.KeyCode = Keys.Back Then
            AddHandler txtcontactnumber.TextChanged, AddressOf txtcontactnumber_TextChanged

        End If

    End Sub


    Private Sub btnadd_MouseHover(sender As Object, e As EventArgs) Handles btnadd.MouseHover
        Cursor = Cursors.Hand
    End Sub

    Private Sub btnadd_MouseLeave(sender As Object, e As EventArgs) Handles btnadd.MouseLeave
        Cursor = Cursors.Default
    End Sub

    Private Sub btnedit_MouseHover(sender As Object, e As EventArgs) Handles btnedit.MouseHover
        Cursor = Cursors.Hand
    End Sub

    Private Sub btnedit_MouseLeave(sender As Object, e As EventArgs) Handles btnedit.MouseLeave
        Cursor = Cursors.Default
    End Sub

    Private Sub btndelete_MouseHover(sender As Object, e As EventArgs) Handles btndelete.MouseHover
        Cursor = Cursors.Hand
    End Sub

    Private Sub btndelete_MouseLeave(sender As Object, e As EventArgs) Handles btndelete.MouseLeave
        Cursor = Cursors.Default
    End Sub

    Private Sub btnclear_MouseHover(sender As Object, e As EventArgs) Handles btnclear.MouseHover
        Cursor = Cursors.Hand
    End Sub

    Private Sub btnclear_MouseLeave(sender As Object, e As EventArgs) Handles btnclear.MouseLeave
        Cursor = Cursors.Default
    End Sub


    Private Sub btnclear_Click_1(sender As Object, e As EventArgs) Handles btnclear.Click

        ClearFields()

        cbstrand.Visible = True
        cbstrand.Location = New Point(942, 285)


        lblstrand.Visible = True
        lblstrand.Location = New Point(942, 266)


    End Sub

    Private Sub txtmname_KeyPress(sender As Object, e As KeyPressEventArgs) Handles txtmname.KeyPress


        If Char.IsControl(e.KeyChar) Then
            e.Handled = False
            Return
        End If

        Dim currentText As String = txtmname.Text
        Dim currentLengthWithoutPeriod As Integer = currentText.Replace(".", "").Length


        If Not Char.IsLetter(e.KeyChar) AndAlso e.KeyChar <> "."c Then
            e.Handled = True
            Return
        End If


        If e.KeyChar = "."c AndAlso currentText.Contains(".") Then
            e.Handled = True
            Return
        End If


        If Char.IsLetter(e.KeyChar) AndAlso currentLengthWithoutPeriod >= 1 Then
            e.Handled = True
            Return
        End If


        If currentText.Length >= 2 AndAlso currentText.EndsWith(".") Then
            e.Handled = True
            Return
        End If


        If Char.IsLetter(e.KeyChar) AndAlso currentText.Contains(".") Then
            e.Handled = True
            Return
        End If

    End Sub

    Private Sub txtmname_TextChanged(sender As Object, e As EventArgs) Handles txtmname.TextChanged
        Static isFormatting As Boolean = False

        If isFormatting Then Return

        Dim currentText As String = txtmname.Text


        Dim initial As String = ""
        For Each c As Char In currentText
            If Char.IsLetter(c) Then
                If initial.Length < 1 Then initial &= c
            ElseIf c = "."c AndAlso Not initial.Contains(".") Then
                initial &= c
            End If
        Next

        If initial.Length > 0 AndAlso Char.IsLetter(initial(0)) Then
            Dim letterPart As String = initial(0).ToString().ToUpper()
            Dim formattedText As String = letterPart

            If initial.Length > 1 AndAlso initial.EndsWith(".") Then
                formattedText &= "."
            End If

            isFormatting = True
            If txtmname.Text <> formattedText Then
                txtmname.Text = formattedText
                txtmname.SelectionStart = txtmname.Text.Length
            End If
            isFormatting = False
        ElseIf currentText.Length > 0 AndAlso Not Char.IsLetter(currentText(0)) Then

            isFormatting = True
            txtmname.Text = ""
            isFormatting = False
        End If

    End Sub

    Private Sub txtemployeeno_KeyDown(sender As Object, e As KeyEventArgs) Handles txtemployeeno.KeyDown
        If e.Control AndAlso (e.KeyCode = Keys.V Or e.KeyCode = Keys.C Or e.KeyCode = Keys.X) Then
            e.SuppressKeyPress = True
        End If
    End Sub

    Private Sub txtemployeeno_KeyPress(sender As Object, e As KeyPressEventArgs) Handles txtemployeeno.KeyPress
        If Not Char.IsDigit(e.KeyChar) And Not Char.IsControl(e.KeyChar) Then
            e.Handled = True
        End If

    End Sub




    Private Sub DisablePaste_AllTextBoxes()
        For Each ctrl As Control In Me.Controls
            AddHandlerToTextBoxes_NoPaste(ctrl)
        Next
    End Sub

    Private Sub AddHandlerToTextBoxes_NoPaste(parent As Control)
        For Each ctrl As Control In parent.Controls
            If TypeOf ctrl Is TextBox Then
                Dim tb As TextBox = CType(ctrl, TextBox)

                tb.ContextMenuStrip = New ContextMenuStrip()

                AddHandler tb.KeyDown, AddressOf BlockPasteKey
                AddHandler tb.MouseUp, AddressOf BlockRightClick

            End If

            If ctrl.HasChildren Then
                AddHandlerToTextBoxes_NoPaste(ctrl)
            End If
        Next

    End Sub


    Private Sub BlockPasteKey(sender As Object, e As KeyEventArgs)

        If (e.Control AndAlso e.KeyCode = Keys.V) OrElse (e.Shift AndAlso e.KeyCode = Keys.Insert) Then
            e.SuppressKeyPress = True
        End If

    End Sub

    Private Sub BlockRightClick(sender As Object, e As MouseEventArgs)

        If e.Button = MouseButtons.Right Then

            Dim tb As TextBox = TryCast(sender, TextBox)
            If tb IsNot Nothing Then
                tb.ContextMenuStrip = New ContextMenuStrip()
            End If
        End If

    End Sub


    Private Sub DataGridView1_MouseHover(sender As Object, e As EventArgs) Handles DataGridView1.MouseHover
        PauseAutoRefresh(DataGridView1)
    End Sub

    Private Sub datagridview1_MouseLeave(sender As Object, e As EventArgs) Handles DataGridView1.MouseLeave
        ResumeAutoRefresh(DataGridView1)
    End Sub

End Class