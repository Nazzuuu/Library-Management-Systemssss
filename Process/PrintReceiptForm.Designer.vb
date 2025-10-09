<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class PrintReceiptForm
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
        Dim CustomizableEdges7 As Guna.UI2.WinForms.Suite.CustomizableEdges = New Guna.UI2.WinForms.Suite.CustomizableEdges()
        Dim CustomizableEdges8 As Guna.UI2.WinForms.Suite.CustomizableEdges = New Guna.UI2.WinForms.Suite.CustomizableEdges()
        Dim CustomizableEdges1 As Guna.UI2.WinForms.Suite.CustomizableEdges = New Guna.UI2.WinForms.Suite.CustomizableEdges()
        Dim CustomizableEdges2 As Guna.UI2.WinForms.Suite.CustomizableEdges = New Guna.UI2.WinForms.Suite.CustomizableEdges()
        Dim CustomizableEdges3 As Guna.UI2.WinForms.Suite.CustomizableEdges = New Guna.UI2.WinForms.Suite.CustomizableEdges()
        Dim CustomizableEdges4 As Guna.UI2.WinForms.Suite.CustomizableEdges = New Guna.UI2.WinForms.Suite.CustomizableEdges()
        Dim DataGridViewCellStyle1 As DataGridViewCellStyle = New DataGridViewCellStyle()
        Dim DataGridViewCellStyle2 As DataGridViewCellStyle = New DataGridViewCellStyle()
        Dim DataGridViewCellStyle3 As DataGridViewCellStyle = New DataGridViewCellStyle()
        Dim DataGridViewCellStyle4 As DataGridViewCellStyle = New DataGridViewCellStyle()
        Dim CustomizableEdges5 As Guna.UI2.WinForms.Suite.CustomizableEdges = New Guna.UI2.WinForms.Suite.CustomizableEdges()
        Dim CustomizableEdges6 As Guna.UI2.WinForms.Suite.CustomizableEdges = New Guna.UI2.WinForms.Suite.CustomizableEdges()
        OpenFileDialog1 = New OpenFileDialog()
        panel_book = New Panel()
        Guna2GradientPanel1 = New Guna.UI2.WinForms.Guna2GradientPanel()
        Label14 = New Label()
        Label12 = New Label()
        lbltransacno = New Label()
        Label9 = New Label()
        picbarcode = New PictureBox()
        btnprint = New Guna.UI2.WinForms.Guna2Button()
        Guna2GradientPanel2 = New Guna.UI2.WinForms.Guna2GradientPanel()
        DataGridView1 = New DataGridView()
        PictureBox1 = New PictureBox()
        txtsearch = New Guna.UI2.WinForms.Guna2TextBox()
        Guna2Elipse1 = New Guna.UI2.WinForms.Guna2Elipse(components)
        panel_book.SuspendLayout()
        Guna2GradientPanel1.SuspendLayout()
        CType(picbarcode, ComponentModel.ISupportInitialize).BeginInit()
        Guna2GradientPanel2.SuspendLayout()
        CType(DataGridView1, ComponentModel.ISupportInitialize).BeginInit()
        CType(PictureBox1, ComponentModel.ISupportInitialize).BeginInit()
        SuspendLayout()
        ' 
        ' OpenFileDialog1
        ' 
        OpenFileDialog1.FileName = "OpenFileDialog1"
        ' 
        ' panel_book
        ' 
        panel_book.Controls.Add(Guna2GradientPanel1)
        panel_book.Dock = DockStyle.Fill
        panel_book.Location = New Point(0, 0)
        panel_book.Name = "panel_book"
        panel_book.Size = New Size(1354, 661)
        panel_book.TabIndex = 2
        ' 
        ' Guna2GradientPanel1
        ' 
        Guna2GradientPanel1.BorderColor = Color.FromArgb(CByte(207), CByte(58), CByte(109))
        Guna2GradientPanel1.BorderRadius = 9
        Guna2GradientPanel1.BorderThickness = 2
        Guna2GradientPanel1.Controls.Add(Label14)
        Guna2GradientPanel1.Controls.Add(Label12)
        Guna2GradientPanel1.Controls.Add(lbltransacno)
        Guna2GradientPanel1.Controls.Add(Label9)
        Guna2GradientPanel1.Controls.Add(picbarcode)
        Guna2GradientPanel1.Controls.Add(btnprint)
        Guna2GradientPanel1.Controls.Add(Guna2GradientPanel2)
        Guna2GradientPanel1.Controls.Add(PictureBox1)
        Guna2GradientPanel1.Controls.Add(txtsearch)
        Guna2GradientPanel1.CustomizableEdges = CustomizableEdges7
        Guna2GradientPanel1.Location = New Point(12, 13)
        Guna2GradientPanel1.Name = "Guna2GradientPanel1"
        Guna2GradientPanel1.ShadowDecoration.CustomizableEdges = CustomizableEdges8
        Guna2GradientPanel1.Size = New Size(1333, 626)
        Guna2GradientPanel1.TabIndex = 46
        ' 
        ' Label14
        ' 
        Label14.AutoSize = True
        Label14.Font = New Font("Tahoma", 9.75F, FontStyle.Bold, GraphicsUnit.Point, CByte(0))
        Label14.ForeColor = Color.Red
        Label14.Location = New Point(79, 72)
        Label14.Name = "Label14"
        Label14.Size = New Size(192, 16)
        Label14.TabIndex = 109
        Label14.Text = "Search and select borrower."
        ' 
        ' Label12
        ' 
        Label12.AutoSize = True
        Label12.Font = New Font("Tahoma", 9.75F, FontStyle.Bold, GraphicsUnit.Point, CByte(0))
        Label12.ForeColor = Color.Black
        Label12.Location = New Point(30, 71)
        Label12.Name = "Label12"
        Label12.Size = New Size(49, 16)
        Label12.TabIndex = 108
        Label12.Text = "Notes:"
        ' 
        ' lbltransacno
        ' 
        lbltransacno.AutoSize = True
        lbltransacno.Font = New Font("Tahoma", 9.75F, FontStyle.Bold, GraphicsUnit.Point, CByte(0))
        lbltransacno.ForeColor = Color.FromArgb(CByte(207), CByte(58), CByte(109))
        lbltransacno.Location = New Point(768, 83)
        lbltransacno.Name = "lbltransacno"
        lbltransacno.Size = New Size(165, 16)
        lbltransacno.TabIndex = 107
        lbltransacno.Text = "Transaction No. Receipt:"
        lbltransacno.Visible = False
        ' 
        ' Label9
        ' 
        Label9.AutoSize = True
        Label9.Font = New Font("Tahoma", 9.75F, FontStyle.Bold, GraphicsUnit.Point, CByte(0))
        Label9.ForeColor = Color.FromArgb(CByte(207), CByte(58), CByte(109))
        Label9.Location = New Point(1041, 26)
        Label9.Name = "Label9"
        Label9.Size = New Size(165, 16)
        Label9.TabIndex = 106
        Label9.Text = "Transaction No. Receipt:"
        ' 
        ' picbarcode
        ' 
        picbarcode.BackColor = Color.White
        picbarcode.Location = New Point(1041, 46)
        picbarcode.Name = "picbarcode"
        picbarcode.Size = New Size(188, 53)
        picbarcode.TabIndex = 105
        picbarcode.TabStop = False
        ' 
        ' btnprint
        ' 
        btnprint.BorderColor = Color.FromArgb(CByte(207), CByte(58), CByte(109))
        btnprint.BorderRadius = 9
        btnprint.BorderThickness = 1
        btnprint.CustomizableEdges = CustomizableEdges1
        btnprint.DisabledState.BorderColor = Color.DarkGray
        btnprint.DisabledState.CustomBorderColor = Color.DarkGray
        btnprint.DisabledState.FillColor = Color.FromArgb(CByte(169), CByte(169), CByte(169))
        btnprint.DisabledState.ForeColor = Color.FromArgb(CByte(141), CByte(141), CByte(141))
        btnprint.FillColor = Color.Empty
        btnprint.Font = New Font("Tahoma", 11.25F, FontStyle.Bold)
        btnprint.ForeColor = Color.FromArgb(CByte(207), CByte(58), CByte(109))
        btnprint.Location = New Point(567, 564)
        btnprint.Name = "btnprint"
        btnprint.ShadowDecoration.CustomizableEdges = CustomizableEdges2
        btnprint.Size = New Size(177, 27)
        btnprint.TabIndex = 52
        btnprint.Text = "PRINT RECEIPT"
        ' 
        ' Guna2GradientPanel2
        ' 
        Guna2GradientPanel2.BorderColor = Color.FromArgb(CByte(207), CByte(58), CByte(109))
        Guna2GradientPanel2.BorderRadius = 9
        Guna2GradientPanel2.BorderThickness = 2
        Guna2GradientPanel2.Controls.Add(DataGridView1)
        Guna2GradientPanel2.CustomizableEdges = CustomizableEdges3
        Guna2GradientPanel2.Location = New Point(30, 120)
        Guna2GradientPanel2.Name = "Guna2GradientPanel2"
        Guna2GradientPanel2.ShadowDecoration.CustomizableEdges = CustomizableEdges4
        Guna2GradientPanel2.Size = New Size(1267, 411)
        Guna2GradientPanel2.TabIndex = 51
        ' 
        ' DataGridView1
        ' 
        DataGridView1.AllowUserToAddRows = False
        DataGridView1.AllowUserToDeleteRows = False
        DataGridView1.AllowUserToResizeColumns = False
        DataGridView1.AllowUserToResizeRows = False
        DataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        DataGridView1.BackgroundColor = SystemColors.Control
        DataGridViewCellStyle1.Alignment = DataGridViewContentAlignment.TopCenter
        DataGridViewCellStyle1.BackColor = Color.FromArgb(CByte(207), CByte(58), CByte(109))
        DataGridViewCellStyle1.Font = New Font("Tahoma", 9.75F, FontStyle.Bold)
        DataGridViewCellStyle1.ForeColor = Color.White
        DataGridViewCellStyle1.SelectionBackColor = Color.FromArgb(CByte(207), CByte(58), CByte(109))
        DataGridViewCellStyle1.SelectionForeColor = Color.White
        DataGridViewCellStyle1.WrapMode = DataGridViewTriState.True
        DataGridView1.ColumnHeadersDefaultCellStyle = DataGridViewCellStyle1
        DataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize
        DataGridView1.Cursor = Cursors.Hand
        DataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle2.BackColor = SystemColors.Window
        DataGridViewCellStyle2.Font = New Font("Segoe UI", 9F)
        DataGridViewCellStyle2.ForeColor = Color.FromArgb(CByte(125), CByte(137), CByte(149))
        DataGridViewCellStyle2.SelectionBackColor = SystemColors.Highlight
        DataGridViewCellStyle2.SelectionForeColor = SystemColors.HighlightText
        DataGridViewCellStyle2.WrapMode = DataGridViewTriState.False
        DataGridView1.DefaultCellStyle = DataGridViewCellStyle2
        DataGridView1.Location = New Point(24, 25)
        DataGridView1.MultiSelect = False
        DataGridView1.Name = "DataGridView1"
        DataGridView1.ReadOnly = True
        DataGridViewCellStyle3.Alignment = DataGridViewContentAlignment.TopCenter
        DataGridViewCellStyle3.BackColor = SystemColors.Control
        DataGridViewCellStyle3.Font = New Font("Segoe UI", 9F)
        DataGridViewCellStyle3.ForeColor = SystemColors.WindowText
        DataGridViewCellStyle3.SelectionBackColor = SystemColors.Highlight
        DataGridViewCellStyle3.SelectionForeColor = SystemColors.HighlightText
        DataGridViewCellStyle3.WrapMode = DataGridViewTriState.True
        DataGridView1.RowHeadersDefaultCellStyle = DataGridViewCellStyle3
        DataGridView1.RowHeadersVisible = False
        DataGridView1.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing
        DataGridViewCellStyle4.Alignment = DataGridViewContentAlignment.TopCenter
        DataGridViewCellStyle4.Font = New Font("Tahoma", 9F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        DataGridViewCellStyle4.ForeColor = Color.Black
        DataGridView1.RowsDefaultCellStyle = DataGridViewCellStyle4
        DataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        DataGridView1.Size = New Size(1213, 361)
        DataGridView1.TabIndex = 23
        ' 
        ' PictureBox1
        ' 
        PictureBox1.BackColor = Color.White
        PictureBox1.BackgroundImage = My.Resources.Resources.magnifier
        PictureBox1.BackgroundImageLayout = ImageLayout.Stretch
        PictureBox1.Location = New Point(880, 24)
        PictureBox1.Name = "PictureBox1"
        PictureBox1.Size = New Size(20, 18)
        PictureBox1.TabIndex = 49
        PictureBox1.TabStop = False
        ' 
        ' txtsearch
        ' 
        txtsearch.BorderColor = Color.FromArgb(CByte(207), CByte(58), CByte(109))
        txtsearch.BorderRadius = 16
        txtsearch.CustomizableEdges = CustomizableEdges5
        txtsearch.DefaultText = ""
        txtsearch.DisabledState.BorderColor = Color.FromArgb(CByte(208), CByte(208), CByte(208))
        txtsearch.DisabledState.FillColor = Color.FromArgb(CByte(226), CByte(226), CByte(226))
        txtsearch.DisabledState.ForeColor = Color.FromArgb(CByte(138), CByte(138), CByte(138))
        txtsearch.DisabledState.PlaceholderForeColor = Color.FromArgb(CByte(138), CByte(138), CByte(138))
        txtsearch.FocusedState.BorderColor = Color.FromArgb(CByte(94), CByte(148), CByte(255))
        txtsearch.Font = New Font("Baskerville Old Face", 12F, FontStyle.Bold)
        txtsearch.ForeColor = Color.Black
        txtsearch.HoverState.BorderColor = Color.FromArgb(CByte(94), CByte(148), CByte(255))
        txtsearch.Location = New Point(25, 17)
        txtsearch.Margin = New Padding(4)
        txtsearch.Name = "txtsearch"
        txtsearch.PlaceholderText = "Search Borrower"
        txtsearch.SelectedText = ""
        txtsearch.ShadowDecoration.CustomizableEdges = CustomizableEdges6
        txtsearch.Size = New Size(889, 30)
        txtsearch.TabIndex = 50
        ' 
        ' Guna2Elipse1
        ' 
        Guna2Elipse1.BorderRadius = 9
        Guna2Elipse1.TargetControl = DataGridView1
        ' 
        ' PrintReceiptForm
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        ClientSize = New Size(1354, 661)
        ControlBox = False
        Controls.Add(panel_book)
        FormBorderStyle = FormBorderStyle.None
        MaximizeBox = False
        MinimizeBox = False
        Name = "PrintReceiptForm"
        ShowIcon = False
        Text = "PrintReceiptForm"
        panel_book.ResumeLayout(False)
        Guna2GradientPanel1.ResumeLayout(False)
        Guna2GradientPanel1.PerformLayout()
        CType(picbarcode, ComponentModel.ISupportInitialize).EndInit()
        Guna2GradientPanel2.ResumeLayout(False)
        CType(DataGridView1, ComponentModel.ISupportInitialize).EndInit()
        CType(PictureBox1, ComponentModel.ISupportInitialize).EndInit()
        ResumeLayout(False)
    End Sub

    Friend WithEvents OpenFileDialog1 As OpenFileDialog
    Friend WithEvents panel_book As Panel
    Friend WithEvents Guna2Elipse1 As Guna.UI2.WinForms.Guna2Elipse
    Friend WithEvents Guna2GradientPanel1 As Guna.UI2.WinForms.Guna2GradientPanel
    Friend WithEvents PictureBox1 As PictureBox
    Friend WithEvents txtsearch As Guna.UI2.WinForms.Guna2TextBox
    Friend WithEvents Guna2GradientPanel2 As Guna.UI2.WinForms.Guna2GradientPanel
    Friend WithEvents DataGridView1 As DataGridView
    Friend WithEvents btnprint As Guna.UI2.WinForms.Guna2Button
    Friend WithEvents picbarcode As PictureBox
    Friend WithEvents Label9 As Label
    Friend WithEvents lbltransacno As Label
    Friend WithEvents Label14 As Label
    Friend WithEvents Label12 As Label
End Class
