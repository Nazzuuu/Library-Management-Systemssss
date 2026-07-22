<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class popuphistory
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
        Dim CustomizableEdges1 As Guna.UI2.WinForms.Suite.CustomizableEdges = New Guna.UI2.WinForms.Suite.CustomizableEdges()
        Dim CustomizableEdges2 As Guna.UI2.WinForms.Suite.CustomizableEdges = New Guna.UI2.WinForms.Suite.CustomizableEdges()
        Dim CustomizableEdges3 As Guna.UI2.WinForms.Suite.CustomizableEdges = New Guna.UI2.WinForms.Suite.CustomizableEdges()
        Dim CustomizableEdges4 As Guna.UI2.WinForms.Suite.CustomizableEdges = New Guna.UI2.WinForms.Suite.CustomizableEdges()
        Dim DataGridViewCellStyle1 As DataGridViewCellStyle = New DataGridViewCellStyle()
        Dim DataGridViewCellStyle2 As DataGridViewCellStyle = New DataGridViewCellStyle()
        Dim DataGridViewCellStyle3 As DataGridViewCellStyle = New DataGridViewCellStyle()
        Dim DataGridViewCellStyle4 As DataGridViewCellStyle = New DataGridViewCellStyle()
        Guna2Panel1 = New Guna.UI2.WinForms.Guna2Panel()
        Label1 = New Label()
        Guna2GradientPanel1 = New Guna.UI2.WinForms.Guna2GradientPanel()
        DataGridView1 = New DataGridView()
        Label8 = New Label()
        lrnlabel = New Label()
        Label2 = New Label()
        lblempy = New Label()
        lblname = New Label()
        lbllrnn = New Label()
        lbltyp = New Label()
        lblempp = New Label()
        Guna2Panel1.SuspendLayout()
        Guna2GradientPanel1.SuspendLayout()
        CType(DataGridView1, ComponentModel.ISupportInitialize).BeginInit()
        SuspendLayout()
        ' 
        ' Guna2Panel1
        ' 
        Guna2Panel1.BackColor = Color.FromArgb(CByte(207), CByte(58), CByte(109))
        Guna2Panel1.Controls.Add(Label1)
        Guna2Panel1.CustomizableEdges = CustomizableEdges1
        Guna2Panel1.Location = New Point(0, 0)
        Guna2Panel1.Name = "Guna2Panel1"
        Guna2Panel1.ShadowDecoration.CustomizableEdges = CustomizableEdges2
        Guna2Panel1.Size = New Size(625, 39)
        Guna2Panel1.TabIndex = 0
        ' 
        ' Label1
        ' 
        Label1.AutoSize = True
        Label1.Font = New Font("Tahoma", 14.25F, FontStyle.Bold, GraphicsUnit.Point, CByte(0))
        Label1.ForeColor = Color.White
        Label1.Location = New Point(23, -2)
        Label1.Name = "Label1"
        Label1.Size = New Size(227, 23)
        Label1.TabIndex = 95
        Label1.Text = "Borrower Information:"
        ' 
        ' Guna2GradientPanel1
        ' 
        Guna2GradientPanel1.BorderColor = Color.FromArgb(CByte(207), CByte(58), CByte(109))
        Guna2GradientPanel1.BorderRadius = 9
        Guna2GradientPanel1.BorderThickness = 2
        Guna2GradientPanel1.Controls.Add(DataGridView1)
        Guna2GradientPanel1.CustomizableEdges = CustomizableEdges3
        Guna2GradientPanel1.Location = New Point(12, 185)
        Guna2GradientPanel1.Name = "Guna2GradientPanel1"
        Guna2GradientPanel1.ShadowDecoration.CustomizableEdges = CustomizableEdges4
        Guna2GradientPanel1.Size = New Size(601, 266)
        Guna2GradientPanel1.TabIndex = 46
        ' 
        ' DataGridView1
        ' 
        DataGridView1.AllowUserToAddRows = False
        DataGridView1.AllowUserToDeleteRows = False
        DataGridView1.AllowUserToResizeColumns = False
        DataGridView1.AllowUserToResizeRows = False
        DataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        DataGridView1.BackgroundColor = SystemColors.Control
        DataGridView1.CellBorderStyle = DataGridViewCellBorderStyle.SunkenHorizontal
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
        DataGridViewCellStyle2.ForeColor = SystemColors.Control
        DataGridViewCellStyle2.SelectionBackColor = SystemColors.Highlight
        DataGridViewCellStyle2.SelectionForeColor = SystemColors.HighlightText
        DataGridViewCellStyle2.WrapMode = DataGridViewTriState.False
        DataGridView1.DefaultCellStyle = DataGridViewCellStyle2
        DataGridView1.Location = New Point(16, 13)
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
        DataGridView1.Size = New Size(571, 239)
        DataGridView1.TabIndex = 26
        ' 
        ' Label8
        ' 
        Label8.AutoSize = True
        Label8.Font = New Font("Tahoma", 9.75F, FontStyle.Bold, GraphicsUnit.Point, CByte(0))
        Label8.ForeColor = Color.FromArgb(CByte(207), CByte(58), CByte(109))
        Label8.Location = New Point(25, 69)
        Label8.Name = "Label8"
        Label8.Size = New Size(112, 16)
        Label8.TabIndex = 87
        Label8.Text = "Borrower Name:"
        ' 
        ' lrnlabel
        ' 
        lrnlabel.AutoSize = True
        lrnlabel.Font = New Font("Tahoma", 9.75F, FontStyle.Bold, GraphicsUnit.Point, CByte(0))
        lrnlabel.ForeColor = Color.FromArgb(CByte(207), CByte(58), CByte(109))
        lrnlabel.Location = New Point(25, 101)
        lrnlabel.Name = "lrnlabel"
        lrnlabel.Size = New Size(36, 16)
        lrnlabel.TabIndex = 88
        lrnlabel.Text = "LRN:"
        ' 
        ' Label2
        ' 
        Label2.AutoSize = True
        Label2.Font = New Font("Tahoma", 9.75F, FontStyle.Bold, GraphicsUnit.Point, CByte(0))
        Label2.ForeColor = Color.FromArgb(CByte(207), CByte(58), CByte(109))
        Label2.Location = New Point(25, 132)
        Label2.Name = "Label2"
        Label2.Size = New Size(43, 16)
        Label2.TabIndex = 89
        Label2.Text = "Type:"
        ' 
        ' lblempy
        ' 
        lblempy.AutoSize = True
        lblempy.Font = New Font("Tahoma", 9.75F, FontStyle.Bold, GraphicsUnit.Point, CByte(0))
        lblempy.ForeColor = Color.FromArgb(CByte(207), CByte(58), CByte(109))
        lblempy.Location = New Point(332, 69)
        lblempy.Name = "lblempy"
        lblempy.Size = New Size(93, 16)
        lblempy.TabIndex = 90
        lblempy.Text = "Employee no:"
        ' 
        ' lblname
        ' 
        lblname.AutoSize = True
        lblname.Font = New Font("Tahoma", 9.75F, FontStyle.Bold, GraphicsUnit.Point, CByte(0))
        lblname.ForeColor = Color.FromArgb(CByte(207), CByte(58), CByte(109))
        lblname.Location = New Point(143, 69)
        lblname.Name = "lblname"
        lblname.Size = New Size(15, 16)
        lblname.TabIndex = 91
        lblname.Text = ".."
        ' 
        ' lbllrnn
        ' 
        lbllrnn.AutoSize = True
        lbllrnn.Font = New Font("Tahoma", 9.75F, FontStyle.Bold, GraphicsUnit.Point, CByte(0))
        lbllrnn.ForeColor = Color.FromArgb(CByte(207), CByte(58), CByte(109))
        lbllrnn.Location = New Point(143, 101)
        lbllrnn.Name = "lbllrnn"
        lbllrnn.Size = New Size(15, 16)
        lbllrnn.TabIndex = 92
        lbllrnn.Text = ".."
        ' 
        ' lbltyp
        ' 
        lbltyp.AutoSize = True
        lbltyp.Font = New Font("Tahoma", 9.75F, FontStyle.Bold, GraphicsUnit.Point, CByte(0))
        lbltyp.ForeColor = Color.FromArgb(CByte(207), CByte(58), CByte(109))
        lbltyp.Location = New Point(143, 132)
        lbltyp.Name = "lbltyp"
        lbltyp.Size = New Size(15, 16)
        lbltyp.TabIndex = 93
        lbltyp.Text = ".."
        ' 
        ' lblempp
        ' 
        lblempp.AutoSize = True
        lblempp.Font = New Font("Tahoma", 9.75F, FontStyle.Bold, GraphicsUnit.Point, CByte(0))
        lblempp.ForeColor = Color.FromArgb(CByte(207), CByte(58), CByte(109))
        lblempp.Location = New Point(442, 69)
        lblempp.Name = "lblempp"
        lblempp.Size = New Size(15, 16)
        lblempp.TabIndex = 94
        lblempp.Text = ".."
        ' 
        ' popuphistory
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        ClientSize = New Size(625, 470)
        Controls.Add(lblempp)
        Controls.Add(lbltyp)
        Controls.Add(lbllrnn)
        Controls.Add(lblname)
        Controls.Add(lblempy)
        Controls.Add(Label2)
        Controls.Add(lrnlabel)
        Controls.Add(Label8)
        Controls.Add(Guna2GradientPanel1)
        Controls.Add(Guna2Panel1)
        FormBorderStyle = FormBorderStyle.FixedDialog
        MaximizeBox = False
        MdiChildrenMinimizedAnchorBottom = False
        MinimizeBox = False
        Name = "popuphistory"
        ShowIcon = False
        StartPosition = FormStartPosition.CenterScreen
        Guna2Panel1.ResumeLayout(False)
        Guna2Panel1.PerformLayout()
        Guna2GradientPanel1.ResumeLayout(False)
        CType(DataGridView1, ComponentModel.ISupportInitialize).EndInit()
        ResumeLayout(False)
        PerformLayout()
    End Sub

    Friend WithEvents Guna2Panel1 As Guna.UI2.WinForms.Guna2Panel
    Friend WithEvents Guna2GradientPanel1 As Guna.UI2.WinForms.Guna2GradientPanel
    Friend WithEvents Label8 As Label
    Friend WithEvents lrnlabel As Label
    Friend WithEvents Label2 As Label
    Friend WithEvents lblempy As Label
    Friend WithEvents lblname As Label
    Friend WithEvents lbllrnn As Label
    Friend WithEvents lbltyp As Label
    Friend WithEvents lblempp As Label
    Friend WithEvents Label1 As Label
    Friend WithEvents DataGridView1 As DataGridView
End Class
