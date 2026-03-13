<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class LibraryCard
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        components = New ComponentModel.Container()
        Dim CustomizableEdges15 As Guna.UI2.WinForms.Suite.CustomizableEdges = New Guna.UI2.WinForms.Suite.CustomizableEdges()
        Dim CustomizableEdges16 As Guna.UI2.WinForms.Suite.CustomizableEdges = New Guna.UI2.WinForms.Suite.CustomizableEdges()
        Dim CustomizableEdges9 As Guna.UI2.WinForms.Suite.CustomizableEdges = New Guna.UI2.WinForms.Suite.CustomizableEdges()
        Dim CustomizableEdges10 As Guna.UI2.WinForms.Suite.CustomizableEdges = New Guna.UI2.WinForms.Suite.CustomizableEdges()
        Dim CustomizableEdges11 As Guna.UI2.WinForms.Suite.CustomizableEdges = New Guna.UI2.WinForms.Suite.CustomizableEdges()
        Dim CustomizableEdges12 As Guna.UI2.WinForms.Suite.CustomizableEdges = New Guna.UI2.WinForms.Suite.CustomizableEdges()
        Dim CustomizableEdges13 As Guna.UI2.WinForms.Suite.CustomizableEdges = New Guna.UI2.WinForms.Suite.CustomizableEdges()
        Dim CustomizableEdges14 As Guna.UI2.WinForms.Suite.CustomizableEdges = New Guna.UI2.WinForms.Suite.CustomizableEdges()
        Dim DataGridViewCellStyle5 As DataGridViewCellStyle = New DataGridViewCellStyle()
        Dim DataGridViewCellStyle6 As DataGridViewCellStyle = New DataGridViewCellStyle()
        Dim DataGridViewCellStyle7 As DataGridViewCellStyle = New DataGridViewCellStyle()
        Dim DataGridViewCellStyle8 As DataGridViewCellStyle = New DataGridViewCellStyle()
        Guna2GroupBox1 = New Guna.UI2.WinForms.Guna2GroupBox()
        chkSelectAll = New Guna.UI2.WinForms.Guna2CheckBox()
        btnPrint = New Guna.UI2.WinForms.Guna2Button()
        PictureBox1 = New PictureBox()
        txtSearch = New Guna.UI2.WinForms.Guna2TextBox()
        Guna2GradientPanel1 = New Guna.UI2.WinForms.Guna2GradientPanel()
        dgvLibraryCard = New DataGridView()
        Guna2Elipse1 = New Guna.UI2.WinForms.Guna2Elipse(components)
        Guna2GroupBox1.SuspendLayout()
        CType(PictureBox1, ComponentModel.ISupportInitialize).BeginInit()
        Guna2GradientPanel1.SuspendLayout()
        CType(dgvLibraryCard, ComponentModel.ISupportInitialize).BeginInit()
        SuspendLayout()
        ' 
        ' Guna2GroupBox1
        ' 
        Guna2GroupBox1.BackColor = Color.Transparent
        Guna2GroupBox1.BorderColor = Color.FromArgb(CByte(207), CByte(58), CByte(109))
        Guna2GroupBox1.BorderRadius = 9
        Guna2GroupBox1.BorderThickness = 2
        Guna2GroupBox1.Controls.Add(chkSelectAll)
        Guna2GroupBox1.Controls.Add(btnPrint)
        Guna2GroupBox1.Controls.Add(PictureBox1)
        Guna2GroupBox1.Controls.Add(txtSearch)
        Guna2GroupBox1.Controls.Add(Guna2GradientPanel1)
        Guna2GroupBox1.CustomBorderColor = Color.Transparent
        Guna2GroupBox1.CustomizableEdges = CustomizableEdges15
        Guna2GroupBox1.FillColor = Color.Transparent
        Guna2GroupBox1.Font = New Font("Segoe UI", 9F)
        Guna2GroupBox1.ForeColor = Color.FromArgb(CByte(125), CByte(137), CByte(149))
        Guna2GroupBox1.Location = New Point(11, 18)
        Guna2GroupBox1.Name = "Guna2GroupBox1"
        Guna2GroupBox1.ShadowDecoration.CustomizableEdges = CustomizableEdges16
        Guna2GroupBox1.Size = New Size(1333, 621)
        Guna2GroupBox1.TabIndex = 1
        ' 
        ' chkSelectAll
        ' 
        chkSelectAll.AutoSize = True
        chkSelectAll.CheckedState.BorderColor = Color.FromArgb(CByte(94), CByte(148), CByte(255))
        chkSelectAll.CheckedState.BorderRadius = 0
        chkSelectAll.CheckedState.BorderThickness = 0
        chkSelectAll.CheckedState.FillColor = Color.FromArgb(CByte(94), CByte(148), CByte(255))
        chkSelectAll.Font = New Font("Tahoma", 11.25F, FontStyle.Bold)
        chkSelectAll.ForeColor = Color.FromArgb(CByte(207), CByte(58), CByte(109))
        chkSelectAll.Location = New Point(25, 73)
        chkSelectAll.Name = "chkSelectAll"
        chkSelectAll.Size = New Size(98, 22)
        chkSelectAll.TabIndex = 49
        chkSelectAll.Text = "Select All"
        chkSelectAll.UncheckedState.BorderColor = Color.FromArgb(CByte(125), CByte(137), CByte(149))
        chkSelectAll.UncheckedState.BorderRadius = 0
        chkSelectAll.UncheckedState.BorderThickness = 0
        chkSelectAll.UncheckedState.FillColor = Color.FromArgb(CByte(125), CByte(137), CByte(149))
        ' 
        ' btnPrint
        ' 
        btnPrint.BorderColor = Color.FromArgb(CByte(207), CByte(58), CByte(109))
        btnPrint.BorderRadius = 9
        btnPrint.BorderThickness = 1
        btnPrint.CustomizableEdges = CustomizableEdges9
        btnPrint.DisabledState.BorderColor = Color.DarkGray
        btnPrint.DisabledState.CustomBorderColor = Color.DarkGray
        btnPrint.DisabledState.FillColor = Color.FromArgb(CByte(169), CByte(169), CByte(169))
        btnPrint.DisabledState.ForeColor = Color.FromArgb(CByte(141), CByte(141), CByte(141))
        btnPrint.FillColor = Color.Empty
        btnPrint.Font = New Font("Tahoma", 11.25F, FontStyle.Bold)
        btnPrint.ForeColor = Color.FromArgb(CByte(207), CByte(58), CByte(109))
        btnPrint.Location = New Point(129, 68)
        btnPrint.Name = "btnPrint"
        btnPrint.ShadowDecoration.CustomizableEdges = CustomizableEdges10
        btnPrint.Size = New Size(127, 27)
        btnPrint.TabIndex = 48
        btnPrint.Text = "Print"
        ' 
        ' PictureBox1
        ' 
        PictureBox1.BackColor = Color.White
        PictureBox1.BackgroundImageLayout = ImageLayout.Stretch
        PictureBox1.Location = New Point(881, 23)
        PictureBox1.Name = "PictureBox1"
        PictureBox1.Size = New Size(20, 18)
        PictureBox1.TabIndex = 47
        PictureBox1.TabStop = False
        ' 
        ' txtSearch
        ' 
        txtSearch.BorderColor = Color.FromArgb(CByte(207), CByte(58), CByte(109))
        txtSearch.BorderRadius = 16
        txtSearch.CustomizableEdges = CustomizableEdges11
        txtSearch.DefaultText = ""
        txtSearch.DisabledState.BorderColor = Color.FromArgb(CByte(208), CByte(208), CByte(208))
        txtSearch.DisabledState.FillColor = Color.FromArgb(CByte(226), CByte(226), CByte(226))
        txtSearch.DisabledState.ForeColor = Color.FromArgb(CByte(138), CByte(138), CByte(138))
        txtSearch.DisabledState.PlaceholderForeColor = Color.FromArgb(CByte(138), CByte(138), CByte(138))
        txtSearch.FocusedState.BorderColor = Color.FromArgb(CByte(94), CByte(148), CByte(255))
        txtSearch.Font = New Font("Baskerville Old Face", 12F, FontStyle.Bold)
        txtSearch.ForeColor = Color.Black
        txtSearch.HoverState.BorderColor = Color.FromArgb(CByte(94), CByte(148), CByte(255))
        txtSearch.Location = New Point(25, 17)
        txtSearch.Margin = New Padding(4)
        txtSearch.MaxLength = 15
        txtSearch.Name = "txtSearch"
        txtSearch.PlaceholderText = "Search Borrower"
        txtSearch.SelectedText = ""
        txtSearch.ShadowDecoration.CustomizableEdges = CustomizableEdges12
        txtSearch.Size = New Size(889, 30)
        txtSearch.TabIndex = 9
        ' 
        ' Guna2GradientPanel1
        ' 
        Guna2GradientPanel1.BorderColor = Color.FromArgb(CByte(207), CByte(58), CByte(109))
        Guna2GradientPanel1.BorderRadius = 9
        Guna2GradientPanel1.BorderThickness = 2
        Guna2GradientPanel1.Controls.Add(dgvLibraryCard)
        Guna2GradientPanel1.CustomizableEdges = CustomizableEdges13
        Guna2GradientPanel1.Location = New Point(25, 106)
        Guna2GradientPanel1.Name = "Guna2GradientPanel1"
        Guna2GradientPanel1.ShadowDecoration.CustomizableEdges = CustomizableEdges14
        Guna2GradientPanel1.Size = New Size(1283, 493)
        Guna2GradientPanel1.TabIndex = 45
        ' 
        ' dgvLibraryCard
        ' 
        dgvLibraryCard.AllowUserToAddRows = False
        dgvLibraryCard.AllowUserToDeleteRows = False
        dgvLibraryCard.AllowUserToResizeColumns = False
        dgvLibraryCard.AllowUserToResizeRows = False
        dgvLibraryCard.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        dgvLibraryCard.BackgroundColor = SystemColors.Control
        dgvLibraryCard.CellBorderStyle = DataGridViewCellBorderStyle.SunkenHorizontal
        DataGridViewCellStyle5.Alignment = DataGridViewContentAlignment.TopCenter
        DataGridViewCellStyle5.BackColor = Color.FromArgb(CByte(207), CByte(58), CByte(109))
        DataGridViewCellStyle5.Font = New Font("Tahoma", 9.75F, FontStyle.Bold)
        DataGridViewCellStyle5.ForeColor = Color.White
        DataGridViewCellStyle5.SelectionBackColor = Color.FromArgb(CByte(207), CByte(58), CByte(109))
        DataGridViewCellStyle5.SelectionForeColor = Color.White
        DataGridViewCellStyle5.WrapMode = DataGridViewTriState.True
        dgvLibraryCard.ColumnHeadersDefaultCellStyle = DataGridViewCellStyle5
        dgvLibraryCard.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize
        dgvLibraryCard.Cursor = Cursors.Hand
        DataGridViewCellStyle6.Alignment = DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle6.BackColor = SystemColors.Window
        DataGridViewCellStyle6.Font = New Font("Segoe UI", 9F)
        DataGridViewCellStyle6.ForeColor = Color.FromArgb(CByte(125), CByte(137), CByte(149))
        DataGridViewCellStyle6.SelectionBackColor = SystemColors.Highlight
        DataGridViewCellStyle6.SelectionForeColor = SystemColors.HighlightText
        DataGridViewCellStyle6.WrapMode = DataGridViewTriState.False
        dgvLibraryCard.DefaultCellStyle = DataGridViewCellStyle6
        dgvLibraryCard.Location = New Point(24, 26)
        dgvLibraryCard.MultiSelect = False
        dgvLibraryCard.Name = "dgvLibraryCard"
        dgvLibraryCard.ReadOnly = True
        DataGridViewCellStyle7.Alignment = DataGridViewContentAlignment.TopCenter
        DataGridViewCellStyle7.BackColor = SystemColors.Control
        DataGridViewCellStyle7.Font = New Font("Segoe UI", 9F)
        DataGridViewCellStyle7.ForeColor = SystemColors.WindowText
        DataGridViewCellStyle7.SelectionBackColor = SystemColors.Highlight
        DataGridViewCellStyle7.SelectionForeColor = SystemColors.HighlightText
        DataGridViewCellStyle7.WrapMode = DataGridViewTriState.True
        dgvLibraryCard.RowHeadersDefaultCellStyle = DataGridViewCellStyle7
        dgvLibraryCard.RowHeadersVisible = False
        dgvLibraryCard.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing
        DataGridViewCellStyle8.Alignment = DataGridViewContentAlignment.TopCenter
        DataGridViewCellStyle8.Font = New Font("Tahoma", 9F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        DataGridViewCellStyle8.ForeColor = Color.Black
        dgvLibraryCard.RowsDefaultCellStyle = DataGridViewCellStyle8
        dgvLibraryCard.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        dgvLibraryCard.Size = New Size(1234, 441)
        dgvLibraryCard.TabIndex = 26
        ' 
        ' Guna2Elipse1
        ' 
        Guna2Elipse1.BorderRadius = 9
        Guna2Elipse1.TargetControl = dgvLibraryCard
        ' 
        ' LibraryCard
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        ClientSize = New Size(1370, 700)
        Controls.Add(Guna2GroupBox1)
        DoubleBuffered = True
        FormBorderStyle = FormBorderStyle.None
        Name = "LibraryCard"
        Text = "LibraryCard"
        Guna2GroupBox1.ResumeLayout(False)
        Guna2GroupBox1.PerformLayout()
        CType(PictureBox1, ComponentModel.ISupportInitialize).EndInit()
        Guna2GradientPanel1.ResumeLayout(False)
        CType(dgvLibraryCard, ComponentModel.ISupportInitialize).EndInit()
        ResumeLayout(False)
    End Sub

    Friend WithEvents Guna2GroupBox1 As Guna.UI2.WinForms.Guna2GroupBox
    Friend WithEvents btnPrint As Guna.UI2.WinForms.Guna2Button
    Friend WithEvents PictureBox1 As PictureBox
    Friend WithEvents txtSearch As Guna.UI2.WinForms.Guna2TextBox
    Friend WithEvents Guna2GradientPanel1 As Guna.UI2.WinForms.Guna2GradientPanel
    Friend WithEvents chkSelectAll As Guna.UI2.WinForms.Guna2CheckBox
    Friend WithEvents dgvLibraryCard As DataGridView
    Friend WithEvents Guna2Elipse1 As Guna.UI2.WinForms.Guna2Elipse
End Class
