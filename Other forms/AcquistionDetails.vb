Imports MySql.Data.MySqlClient

Public Class AcquistionDetails

    Private bookCount As Integer = 1
    Private addedPanels As New List(Of Guna.UI2.WinForms.Guna2Panel)
    Private allowRealClose As Boolean = False
    Private isLayouting As Boolean = False

    Private Const BASE_X As Integer = 12
    Private Const BASE_BOOK_Y As Integer = 185
    Private Const GAP As Integer = 15

    Public Property SelectedAcquisitionID As String = ""

    Private Sub AcquistionDetails_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.Size = New Size(1071, 701)
        Me.AutoScroll = True



        DisablePaste_AllTextBoxes()

        If cbsupplierdonator.Items.Count = 0 Then
            supplieracq()
        End If

        If Not String.IsNullOrEmpty(SelectedAcquisitionID) Then

            Debug.WriteLine("Editing ID: " & SelectedAcquisitionID)
        Else

            jineret()
            clearlahatsu(False)
        End If


        DateTimePicker1.MaxDate = DateTime.Now.Date
        If DateTimePicker1.Value > DateTimePicker1.MaxDate Then
            DateTimePicker1.Value = DateTimePicker1.MaxDate
        End If

        AddHandler cbsupplierdonator.DropDown, AddressOf RefreshComboBoxes
        AddHandler GlobalVarsModule.DatabaseUpdated, AddressOf OnDatabaseUpdated

        Panel_Duplicate.Location = New Point(BASE_X, BASE_BOOK_Y)

        If addedPanels.Count = 0 Then
            lblbooknumber.Text = "Book 1"
            lblremove.Visible = False
            addedPanels.Add(Panel_Duplicate)

            AddHandler cbisbnbarcode.SelectedIndexChanged, AddressOf DynamicISBNBarcode_Changed
            AddHandler txtbookprice.TextChanged, AddressOf DynamicCalculateTotal
            AddHandler numupdown.ValueChanged, AddressOf DynamicCalculateTotal
            AddHandler txtisbn.TextChanged, AddressOf DynamicSearch_TextChanged
            AddHandler txtbarcode.TextChanged, AddressOf DynamicSearch_TextChanged
        End If
    End Sub

    Public Sub clearlahatsu(Optional isEditMode As Boolean = False)

        txtisbn.Enabled = False
        txtbarcode.Enabled = False
        txttransactionno.Enabled = False
        txtbooktitle.Enabled = False

        txtisbn.Text = ""
        txtbarcode.Text = ""
        txttransactionno.Text = ""
        txtbooktitle.Text = ""
        txtbookprice.Text = ""
        numupdown.Value = 0
        txttotalcost.Text = ""
        txttotalcost.Enabled = False

        If Not isEditMode Then
            txttransactionno.Clear()
            cbisbnbarcode.SelectedIndex = -1
            cbsupplierdonator.SelectedIndex = -1
            cbacquistiontype.SelectedIndex = -1
            txtdonor.Clear()
        End If

        If addedPanels.Count > 1 Then

            For i As Integer = addedPanels.Count - 1 To 1 Step -1
                Dim pnl = addedPanels(i)
                addedPanels.RemoveAt(i)
                Me.Controls.Remove(pnl)
                pnl.Dispose()
            Next

            bookCount = 1

            ResetLayout()

        End If


    End Sub

    Private Sub RefreshComboBoxes(sender As Object, e As EventArgs)
        Dim cb As ComboBox = DirectCast(sender, ComboBox)

        Using con As New MySqlConnection(GlobalVarsModule.connectionString)
            Dim query As String = ""

            Select Case cb.Name.ToLower()
                Case "cbsupplierdonator"
                    query = "SELECT SupplierName FROM supplier_tbl ORDER BY SupplierName"
            End Select

            If query <> "" Then
                Dim dt As New DataTable()
                Dim da As New MySqlDataAdapter(query, con)
                da.Fill(dt)

                cb.DataSource = dt
                cb.DisplayMember = dt.Columns(0).ColumnName
                cb.ValueMember = dt.Columns(0).ColumnName
                cb.SelectedIndex = -1
            End If
        End Using
    End Sub

    Private Async Sub OnDatabaseUpdated()
        Try

            Await Task.Run(Sub()
                               Me.Invoke(Sub() supplieracq())
                           End Sub)
        Catch ex As Exception
            Debug.WriteLine("OnDatabaseUpdated error: " & ex.Message)
        End Try
    End Sub

    'Private Sub cbsuppliername_DropDown(sender As Object, e As EventArgs) Handles cbsupplierdonator.DropDown
    '    Try
    '        cbsupplierdonator.DataSource = Nothing
    '        supplieracq()
    '    Catch ex As Exception
    '        Debug.WriteLine("Error refreshing supplier combo: " & ex.Message)
    '    End Try
    'End Sub

    Public Sub supplieracq()
        Dim con As New MySqlConnection(GlobalVarsModule.connectionString)
        Dim com As String = "SELECT * FROM supplier_tbl"
        Dim adap As New MySqlDataAdapter(com, con)
        Dim dt As New DataTable()

        Try
            adap.Fill(dt)
            cbsupplierdonator.DataSource = dt
            cbsupplierdonator.DisplayMember = "SupplierName"
            cbsupplierdonator.ValueMember = "ID"
            cbsupplierdonator.SelectedIndex = -1
        Catch ex As Exception
            Debug.WriteLine("Supplier load error: " & ex.Message)
        End Try
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

    Public Sub jineret()

        Dim con As New MySqlConnection(GlobalVarsModule.connectionString)
        Dim com As String
        Dim lastTransactionNo As String = ""
        Dim newTransactionNo As Integer = 0

        If Not String.IsNullOrEmpty(SelectedAcquisitionID) Then
            Exit Sub
        End If

        Try
            con.Open()
            com = "SELECT TransactionNo FROM acquisition_tbl ORDER BY LENGTH(TransactionNo) DESC, TransactionNo DESC LIMIT 1"

            Using comsi As New MySqlCommand(com, con)
                Dim result As Object = comsi.ExecuteScalar()
                If result IsNot Nothing Then
                    lastTransactionNo = result.ToString()
                End If
            End Using
        Catch ex As Exception
            MessageBox.Show("Error: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        Finally
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
        End Try

        If String.IsNullOrEmpty(lastTransactionNo) Then
            newTransactionNo = 1
        Else
            Dim number As String = lastTransactionNo.Substring(lastTransactionNo.IndexOf("-") + 1)
            If Integer.TryParse(number, newTransactionNo) Then
                newTransactionNo += 1
            Else
                newTransactionNo = 1
            End If
        End If

        txttransactionno.Text = "T-" & newTransactionNo.ToString("D5")
    End Sub

    Private Sub AcquistionDetails_KeyDown(sender As Object, e As KeyEventArgs) Handles MyBase.KeyDown
        If e.KeyCode = Keys.Escape Then
            Me.Hide()
        End If
    End Sub

    Private Sub AcquistionDetails_VisibleChanged(sender As Object, e As EventArgs) Handles Me.VisibleChanged
        If Me.Visible Then
            ResetLayout()
            jineret()
        End If
    End Sub

    Private Sub btnaddanotherbook_Click(sender As Object, e As EventArgs) Handles btnaddanotherbook.Click
        DuplicateBookPanel()
    End Sub

    Private Sub DuplicateBookPanel()
        bookCount += 1


        Dim isPurchased As Boolean = (cbacquistiontype.SelectedItem IsNot Nothing AndAlso cbacquistiontype.SelectedItem.ToString() = "PURCHASED")

        Dim newPanel As New Guna.UI2.WinForms.Guna2Panel With {
        .Size = Panel_Duplicate.Size,
        .BorderColor = Panel_Duplicate.BorderColor,
        .BorderThickness = Panel_Duplicate.BorderThickness,
        .BorderRadius = Panel_Duplicate.BorderRadius,
        .CustomBorderColor = Panel_Duplicate.CustomBorderColor,
        .FillColor = Panel_Duplicate.FillColor,
        .BackColor = Panel_Duplicate.BackColor,
        .Name = "Panel_Book_" & bookCount
    }

        For Each ctrl As Control In Panel_Duplicate.Controls
            Dim newCtrl As Control = Nothing
            If TypeOf ctrl Is Guna.UI2.WinForms.Guna2TextBox Then
                Dim o = DirectCast(ctrl, Guna.UI2.WinForms.Guna2TextBox)
                Dim c As New Guna.UI2.WinForms.Guna2TextBox()
                c.BorderRadius = o.BorderRadius
                c.BorderColor = o.BorderColor
                c.FillColor = o.FillColor
                c.FocusedState.BorderColor = o.FocusedState.BorderColor
                c.HoverState.BorderColor = o.HoverState.BorderColor
                c.PlaceholderText = o.PlaceholderText
                c.PasswordChar = o.PasswordChar
                newCtrl = c
                c.Text = ""

                c.Enabled = o.Enabled

                If ctrl.Name = "txtisbn" Or ctrl.Name = "txtbarcode" Then
                    AddHandler c.TextChanged, AddressOf DynamicSearch_TextChanged
                ElseIf ctrl.Name = "txtbookprice" Then
                    AddHandler c.TextChanged, AddressOf DynamicCalculateTotal
                End If

            ElseIf TypeOf ctrl Is Guna.UI2.WinForms.Guna2ComboBox Then
                Dim o = DirectCast(ctrl, Guna.UI2.WinForms.Guna2ComboBox)
                Dim c As New Guna.UI2.WinForms.Guna2ComboBox()
                c.BorderRadius = o.BorderRadius
                c.BorderColor = o.BorderColor
                c.FillColor = o.FillColor
                For Each item In o.Items : c.Items.Add(item) : Next
                c.SelectedIndex = -1
                newCtrl = c


                c.Enabled = o.Enabled

                If ctrl.Name = "cbisbnbarcode" Then AddHandler c.SelectedIndexChanged, AddressOf DynamicISBNBarcode_Changed

            ElseIf TypeOf ctrl Is Guna.UI2.WinForms.Guna2NumericUpDown Or TypeOf ctrl Is NumericUpDown Then
                Dim c As Control
                If TypeOf ctrl Is Guna.UI2.WinForms.Guna2NumericUpDown Then
                    Dim o = DirectCast(ctrl, Guna.UI2.WinForms.Guna2NumericUpDown)
                    Dim n As New Guna.UI2.WinForms.Guna2NumericUpDown()
                    n.Minimum = o.Minimum : n.Maximum = o.Maximum : n.Value = 0
                    n.BorderRadius = o.BorderRadius : n.BorderColor = o.BorderColor : n.FillColor = o.FillColor
                    n.UpDownButtonFillColor = o.UpDownButtonFillColor : n.UpDownButtonForeColor = o.UpDownButtonForeColor
                    n.Enabled = o.Enabled
                    c = n
                    AddHandler n.ValueChanged, AddressOf DynamicCalculateTotal
                Else
                    Dim o = DirectCast(ctrl, NumericUpDown)
                    Dim n As New NumericUpDown()
                    n.Minimum = o.Minimum : n.Maximum = o.Maximum : n.Value = 0
                    n.Enabled = o.Enabled
                    c = n
                    AddHandler n.ValueChanged, AddressOf DynamicCalculateTotal
                End If
                newCtrl = c

            ElseIf TypeOf ctrl Is Label Then
                newCtrl = New Label()
            End If

            If newCtrl IsNot Nothing Then
                newCtrl.Size = ctrl.Size : newCtrl.Location = ctrl.Location : newCtrl.Font = ctrl.Font
                newCtrl.ForeColor = ctrl.ForeColor : newCtrl.Name = ctrl.Name

                If ctrl.Name = "lblbooknumber" Then newCtrl.Text = "Book " & bookCount
                If ctrl.Name = "lblremove" Then
                    newCtrl.Text = "REMOVE" : newCtrl.Visible = True : newCtrl.Cursor = Cursors.Hand
                    AddHandler newCtrl.Click, AddressOf RemovePanel_Click
                ElseIf TypeOf newCtrl Is Label Then
                    newCtrl.Text = ctrl.Text
                End If
                newPanel.Controls.Add(newCtrl)
            End If
        Next
        Me.Controls.Add(newPanel)
        addedPanels.Add(newPanel)
        ResetLayout()
    End Sub

    Private Sub DynamicISBNBarcode_Changed(sender As Object, e As EventArgs)
        Dim cb = DirectCast(sender, Guna.UI2.WinForms.Guna2ComboBox)
        Dim panel = DirectCast(cb.Parent, Guna.UI2.WinForms.Guna2Panel)

        Dim txtIsbn As Guna.UI2.WinForms.Guna2TextBox = panel.Controls("txtisbn")
        Dim txtBarcode As Guna.UI2.WinForms.Guna2TextBox = panel.Controls("txtbarcode")

        If cb.SelectedItem IsNot Nothing Then
            If cb.SelectedItem.ToString() = "ISBN" Then
                txtIsbn.Enabled = True
                txtBarcode.Enabled = False

                txtBarcode.Text = ""
            Else
                txtBarcode.Enabled = True
                txtIsbn.Enabled = False
                txtIsbn.Text = ""
            End If
        End If
    End Sub

    Private Sub DynamicCalculateTotal(sender As Object, e As EventArgs)
        Dim ctrl = DirectCast(sender, Control)
        Dim panel = DirectCast(ctrl.Parent, Guna.UI2.WinForms.Guna2Panel)

        Dim txtPrice As Guna.UI2.WinForms.Guna2TextBox = panel.Controls("txtbookprice")
        Dim numQty As Control = panel.Controls("numupdown")
        Dim txtTotal As Guna.UI2.WinForms.Guna2TextBox = panel.Controls("txttotalcost")

        Dim price As Decimal = 0
        Dim qty As Integer = 0

        Decimal.TryParse(txtPrice.Text, price)
        If TypeOf numQty Is Guna.UI2.WinForms.Guna2NumericUpDown Then
            qty = DirectCast(numQty, Guna.UI2.WinForms.Guna2NumericUpDown).Value
        ElseIf TypeOf numQty Is NumericUpDown Then
            qty = DirectCast(numQty, NumericUpDown).Value
        End If

        txtTotal.Text = (price * qty).ToString("N2")
    End Sub

    Private Sub DynamicSearch_TextChanged(sender As Object, e As EventArgs)
        Dim tb = DirectCast(sender, Guna.UI2.WinForms.Guna2TextBox)
        Dim panel = DirectCast(tb.Parent, Guna.UI2.WinForms.Guna2Panel)
        Dim txtTitle As Guna.UI2.WinForms.Guna2TextBox = panel.Controls("txtbooktitle")

        If String.IsNullOrWhiteSpace(tb.Text) Then
            txtTitle.Text = ""
            Return
        End If

        Dim query As String = ""
        If tb.Name = "txtisbn" Then
            query = "SELECT `BookTitle` FROM `book_tbl` WHERE `ISBN` = @Val"
        ElseIf tb.Name = "txtbarcode" Then
            query = "SELECT `BookTitle` FROM `book_tbl` WHERE `Barcode` = @Val"
        End If

        Using con As New MySqlConnection(GlobalVarsModule.connectionString)
            Try
                con.Open()
                Using cmd As New MySqlCommand(query, con)
                    cmd.Parameters.AddWithValue("@Val", tb.Text)
                    Dim result = cmd.ExecuteScalar()
                    If result IsNot Nothing Then
                        txtTitle.Text = result.ToString()
                    Else
                        txtTitle.Text = ""
                    End If
                End Using
            Catch ex As Exception
            End Try
        End Using
    End Sub

    Private Sub RemovePanel_Click(sender As Object, e As EventArgs)
        Dim lbl = DirectCast(sender, Label)
        Dim panel = DirectCast(lbl.Parent, Guna.UI2.WinForms.Guna2Panel)

        addedPanels.Remove(panel)
        Me.Controls.Remove(panel)
        panel.Dispose()
        ResetLayout()
    End Sub

    Private Sub ResetLayout()
        If isLayouting Then Exit Sub
        isLayouting = True

        Me.SuspendLayout()
        Me.AutoScrollPosition = New Point(0, 0)
        Panel_Transaction.Location = New Point(12, 29)
        Panel_Transaction.BringToFront()

        Dim currentY As Integer = BASE_BOOK_Y
        For i As Integer = 0 To addedPanels.Count - 1
            addedPanels(i).Location = New Point(BASE_X, currentY)
            addedPanels(i).BringToFront()
            currentY += addedPanels(i).Height + GAP
        Next

        Panel_buttons.Location = New Point(BASE_X, currentY)
        Panel_buttons.BringToFront()
        UpdateLabels()
        Me.ResumeLayout()
        isLayouting = False
    End Sub

    Private Sub UpdateLabels()
        For i As Integer = 0 To addedPanels.Count - 1
            For Each ctrl As Control In addedPanels(i).Controls
                If ctrl.Name = "lblbooknumber" Then
                    ctrl.Text = "Book " & (i + 1)
                End If
            Next
        Next
        lblsubmitcounts.Text = "Submit All (" & addedPanels.Count & " books)"
    End Sub

    Private Sub AcquistionDetails_Activated(sender As Object, e As EventArgs) Handles MyBase.Activated
        ResetLayout()
    End Sub

    Private Sub btnsubmitall_Click(sender As Object, e As EventArgs) Handles btnsubmitall.Click


        If cbacquistiontype.SelectedIndex = -1 Then
            MessageBox.Show("Please select Acquisition Type.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            cbacquistiontype.Focus()
            Exit Sub
        End If

        Dim isPurchased As Boolean = (cbacquistiontype.Text = "PURCHASED")


        If isPurchased AndAlso cbsupplierdonator.SelectedIndex = -1 Then
            MessageBox.Show("Please select Supplier.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            cbsupplierdonator.Focus()
            Exit Sub
        End If

        Dim usedISBNs As New List(Of String)

        For Each panel As Guna.UI2.WinForms.Guna2Panel In addedPanels
            Dim bookNum As String = ""
            For Each ctrl As Control In panel.Controls
                If ctrl.Name = "lblbooknumber" Then bookNum = ctrl.Text
            Next

            Dim currentISBN As String = DirectCast(panel.Controls("txtisbn"), Guna.UI2.WinForms.Guna2TextBox).Text.Trim()

            If Not String.IsNullOrEmpty(currentISBN) Then
                If usedISBNs.Contains(currentISBN) Then
                    MessageBox.Show("Duplicate ISBN detected: " & currentISBN & " in " & bookNum, "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Return
                End If
                usedISBNs.Add(currentISBN)
            End If


            For Each ctrl As Control In panel.Controls
                If ctrl.Name = "cbisbnbarcode" AndAlso DirectCast(ctrl, Guna.UI2.WinForms.Guna2ComboBox).SelectedIndex = -1 Then
                    MessageBox.Show("Please select ISBN/Barcode for " & bookNum, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    ctrl.Focus()
                    Exit Sub
                End If


                If TypeOf ctrl Is Guna.UI2.WinForms.Guna2TextBox Then
                    Dim tb = DirectCast(ctrl, Guna.UI2.WinForms.Guna2TextBox)
                    If isPurchased AndAlso String.IsNullOrWhiteSpace(tb.Text) AndAlso tb.Enabled = True AndAlso tb.Name <> "txttotalcost" Then
                        MessageBox.Show("Please fill in all fields for " & bookNum, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        tb.Focus()
                        Exit Sub
                    End If
                End If


                If ctrl.Name = "numupdown" Then
                    If DirectCast(ctrl, Guna.UI2.WinForms.Guna2NumericUpDown).Value <= 0 Then
                        MessageBox.Show("Quantity for " & bookNum & " must be greater than 0.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        ctrl.Focus()
                        Exit Sub
                    End If
                End If
            Next
        Next


        Try
            Dim dt As DataTable = TryCast(Acquisition2.DataGridView1.DataSource, DataTable)

            Using con As New MySqlConnection(GlobalVarsModule.connectionString)
                con.Open()
                For Each panel As Guna.UI2.WinForms.Guna2Panel In addedPanels
                    Dim isbn As String = DirectCast(panel.Controls("txtisbn"), Guna.UI2.WinForms.Guna2TextBox).Text
                    Dim barcode As String = DirectCast(panel.Controls("txtbarcode"), Guna.UI2.WinForms.Guna2TextBox).Text
                    Dim title As String = DirectCast(panel.Controls("txtbooktitle"), Guna.UI2.WinForms.Guna2TextBox).Text

                    Dim supplierName As String = If(isPurchased, cbsupplierdonator.Text, "")
                    Dim donorName As String = If(Not isPurchased, txtdonor.Text, "")


                    Dim qtyRaw = DirectCast(panel.Controls("numupdown"), Guna.UI2.WinForms.Guna2NumericUpDown).Value
                    Dim priceText = DirectCast(panel.Controls("txtbookprice"), Guna.UI2.WinForms.Guna2TextBox).Text


                    Dim qtyValue As Integer = CInt(qtyRaw)
                    Dim priceValue As Object = DBNull.Value
                    Dim totalValue As Object = DBNull.Value

                    Dim parsedPrice As Decimal
                    If isPurchased AndAlso Decimal.TryParse(priceText, parsedPrice) Then
                        priceValue = parsedPrice
                        totalValue = parsedPrice * qtyValue
                    End If

                    Dim transNo As String = txttransactionno.Text
                    Dim dateAcq As String = DateTimePicker1.Value.ToString("yyyy-MM-dd")

                    Dim sql As String = "INSERT INTO acquisition_tbl (TransactionNo, ISBN, Barcode, BookTitle, SupplierName, Donor, Quantity, BookPrice, TotalCost, DateAcquired) " &
                                    "VALUES (@trans, @isbn, @barcode, @title, @sup, @don, @qty, @price, @total, @date)"

                    Using cmd As New MySqlCommand(sql, con)
                        cmd.Parameters.AddWithValue("@trans", transNo)
                        cmd.Parameters.AddWithValue("@isbn", isbn)
                        cmd.Parameters.AddWithValue("@barcode", barcode)
                        cmd.Parameters.AddWithValue("@title", title)
                        cmd.Parameters.AddWithValue("@sup", supplierName)
                        cmd.Parameters.AddWithValue("@don", donorName)
                        cmd.Parameters.AddWithValue("@qty", qtyValue)
                        cmd.Parameters.AddWithValue("@price", priceValue)
                        cmd.Parameters.AddWithValue("@total", totalValue)
                        cmd.Parameters.AddWithValue("@date", dateAcq)
                        cmd.ExecuteNonQuery()
                    End Using


                    Dim newRow As DataRow = dt.NewRow()
                    newRow("ISBN") = isbn
                    newRow("Barcode") = barcode
                    newRow("BookTitle") = title
                    newRow("SupplierName") = supplierName
                    newRow("Donor") = donorName
                    newRow("Quantity") = qtyValue
                    newRow("BookPrice") = If(priceValue Is DBNull.Value, 0, priceValue)
                    newRow("TotalCost") = If(totalValue Is DBNull.Value, 0, totalValue)
                    newRow("TransactionNo") = transNo
                    newRow("DateAcquired") = dateAcq
                    dt.Rows.Add(newRow)
                Next
            End Using

            MessageBox.Show("Books Added Successfully", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)

            clearlahatsu(False)

        Catch ex As Exception
            MessageBox.Show("Error processing transaction: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub



    Private Sub cbacquistiontype_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbacquistiontype.SelectedIndexChanged
        Dim isPurchased As Boolean = (cbacquistiontype.SelectedItem IsNot Nothing AndAlso cbacquistiontype.SelectedItem.ToString() = "PURCHASED")

        If isPurchased Then
            lblsupdonator.Text = "SUPPLIER:"
            cbsupplierdonator.Visible = True
            For Each panel In addedPanels
                panel.Controls("txtbookprice").Enabled = True
                panel.Controls("numupdown").Enabled = True
            Next
        Else
            lblsupdonator.Text = "DONATOR:"
            cbsupplierdonator.Visible = False
            cbsupplierdonator.SelectedIndex = -1
            txtbookprice.Enabled = False

            For Each panel In addedPanels
                Dim txtPrice = DirectCast(panel.Controls("txtbookprice"), Guna.UI2.WinForms.Guna2TextBox)
                Dim txtTotal = DirectCast(panel.Controls("txttotalcost"), Guna.UI2.WinForms.Guna2TextBox)
                Dim numQty = DirectCast(panel.Controls("numupdown"), Guna.UI2.WinForms.Guna2NumericUpDown)


                txtPrice.Text = ""
                txtPrice.Enabled = False
                txtTotal.Text = ""
                numQty.Enabled = True

            Next
        End If
    End Sub

    Private Sub cbisbnbarcode_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbisbnbarcode.SelectedIndexChanged
        If cbisbnbarcode.Text = "ISBN" Then
            txtisbn.Enabled = True
            txtbarcode.Enabled = False
            txtbarcode.Clear()
        ElseIf cbisbnbarcode.Text = "BARCODE" Then
            txtisbn.Enabled = False
            txtbarcode.Enabled = True
            txtisbn.Clear()
        End If
    End Sub


    Private Sub CalculateTotal()
        Dim price As Decimal = 0
        Dim qty As Decimal = numupdown.Value

        If Decimal.TryParse(txtbookprice.Text, price) Then
            txttotalcost.Text = (price * qty).ToString("F2")
        Else
            txttotalcost.Text = "0.00"
        End If
    End Sub


    Private Sub btnupdate_Click(sender As Object, e As EventArgs) Handles btnupdate.Click
        If String.IsNullOrEmpty(SelectedAcquisitionID) Then Return

        Try
            Using con As New MySqlConnection(GlobalVarsModule.connectionString)
                con.Open()
                Dim isPurchased As Boolean = (cbacquistiontype.Text = "PURCHASED")

                Dim sql As String = "UPDATE acquisition_tbl SET " &
                                "ISBN=@isbn, Barcode=@barcode, BookTitle=@title, " &
                                "SupplierName=@sup, Donor=@don, Quantity=@qty, " &
                                "BookPrice=@price, TotalCost=@total, DateAcquired=@date " &
                                "WHERE ID=@id"

                Using cmd As New MySqlCommand(sql, con)
                    cmd.Parameters.AddWithValue("@isbn", txtisbn.Text)
                    cmd.Parameters.AddWithValue("@barcode", txtbarcode.Text)
                    cmd.Parameters.AddWithValue("@title", txtbooktitle.Text)
                    cmd.Parameters.AddWithValue("@sup", If(isPurchased, cbsupplierdonator.Text, ""))
                    cmd.Parameters.AddWithValue("@don", If(Not isPurchased, txtdonor.Text, ""))
                    cmd.Parameters.AddWithValue("@qty", numupdown.Value)

                    Dim price As Decimal = 0
                    Decimal.TryParse(txtbookprice.Text, price)
                    cmd.Parameters.AddWithValue("@price", If(isPurchased, price, DBNull.Value))
                    cmd.Parameters.AddWithValue("@total", If(isPurchased, price * numupdown.Value, DBNull.Value))

                    cmd.Parameters.AddWithValue("@date", DateTimePicker1.Value.ToString("yyyy-MM-dd"))
                    cmd.Parameters.AddWithValue("@id", SelectedAcquisitionID)

                    cmd.ExecuteNonQuery()
                End Using
            End Using

            MessageBox.Show("Record Updated Successfully", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
            GlobalVarsModule.TriggerDatabaseUpdated()
            Me.Close()
        Catch ex As Exception
            MessageBox.Show("Update Error: " & ex.Message)
        End Try
    End Sub

    Private Sub txtbookprice_TextChanged(sender As Object, e As EventArgs) Handles txtbookprice.TextChanged
        CalculateTotal()
    End Sub
End Class