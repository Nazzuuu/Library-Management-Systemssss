<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class AuditTrail
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
        Dim CustomizableEdges5 As Guna.UI2.WinForms.Suite.CustomizableEdges = New Guna.UI2.WinForms.Suite.CustomizableEdges()
        Dim CustomizableEdges6 As Guna.UI2.WinForms.Suite.CustomizableEdges = New Guna.UI2.WinForms.Suite.CustomizableEdges()
        Dim DataGridViewCellStyle1 As DataGridViewCellStyle = New DataGridViewCellStyle()
        Dim DataGridViewCellStyle2 As DataGridViewCellStyle = New DataGridViewCellStyle()
        Dim DataGridViewCellStyle3 As DataGridViewCellStyle = New DataGridViewCellStyle()
        Dim DataGridViewCellStyle4 As DataGridViewCellStyle = New DataGridViewCellStyle()
        Dim CustomizableEdges3 As Guna.UI2.WinForms.Suite.CustomizableEdges = New Guna.UI2.WinForms.Suite.CustomizableEdges()
        Dim CustomizableEdges4 As Guna.UI2.WinForms.Suite.CustomizableEdges = New Guna.UI2.WinForms.Suite.CustomizableEdges()
        panel_timein = New Panel()
        Guna2GroupBox1 = New Guna.UI2.WinForms.Guna2GroupBox()
        cbfilter = New Guna.UI2.WinForms.Guna2ComboBox()
        Guna2GradientPanel1 = New Guna.UI2.WinForms.Guna2GradientPanel()
        DataGridView1 = New DataGridView()
        Guna2Elipse1 = New Guna.UI2.WinForms.Guna2Elipse(components)
        Label3 = New Label()
        Label13 = New Label()
        Guna2Panel1 = New Guna.UI2.WinForms.Guna2Panel()
        panel_timein.SuspendLayout()
        Guna2GroupBox1.SuspendLayout()
        Guna2GradientPanel1.SuspendLayout()
        CType(DataGridView1, ComponentModel.ISupportInitialize).BeginInit()
        Guna2Panel1.SuspendLayout()
        SuspendLayout()
        ' 
        ' panel_timein
        ' 
        panel_timein.Controls.Add(Guna2GroupBox1)
        panel_timein.Dock = DockStyle.Fill
        panel_timein.Location = New Point(0, 0)
        panel_timein.Name = "panel_timein"
        panel_timein.Size = New Size(1370, 700)
        panel_timein.TabIndex = 3
        ' 
        ' Guna2GroupBox1
        ' 
        Guna2GroupBox1.BackColor = Color.Transparent
        Guna2GroupBox1.BorderColor = Color.FromArgb(CByte(207), CByte(58), CByte(109))
        Guna2GroupBox1.BorderRadius = 9
        Guna2GroupBox1.BorderThickness = 2
        Guna2GroupBox1.Controls.Add(cbfilter)
        Guna2GroupBox1.Controls.Add(Label3)
        Guna2GroupBox1.Controls.Add(Guna2Panel1)
        Guna2GroupBox1.Controls.Add(Guna2GradientPanel1)
        Guna2GroupBox1.CustomBorderColor = Color.Transparent
        Guna2GroupBox1.CustomizableEdges = CustomizableEdges7
        Guna2GroupBox1.FillColor = Color.Transparent
        Guna2GroupBox1.Font = New Font("Segoe UI", 9F)
        Guna2GroupBox1.ForeColor = SystemColors.Control
        Guna2GroupBox1.Location = New Point(12, 13)
        Guna2GroupBox1.Name = "Guna2GroupBox1"
        Guna2GroupBox1.ShadowDecoration.CustomizableEdges = CustomizableEdges8
        Guna2GroupBox1.Size = New Size(1333, 626)
        Guna2GroupBox1.TabIndex = 0
        ' 
        ' cbfilter
        ' 
        cbfilter.BackColor = Color.Transparent
        cbfilter.BorderColor = Color.FromArgb(CByte(207), CByte(58), CByte(109))
        cbfilter.BorderRadius = 12
        cbfilter.CustomizableEdges = CustomizableEdges1
        cbfilter.DrawMode = DrawMode.OwnerDrawFixed
        cbfilter.DropDownStyle = ComboBoxStyle.DropDownList
        cbfilter.FocusedColor = Color.FromArgb(CByte(94), CByte(148), CByte(255))
        cbfilter.FocusedState.BorderColor = Color.FromArgb(CByte(94), CByte(148), CByte(255))
        cbfilter.Font = New Font("Tahoma", 9.75F, FontStyle.Bold)
        cbfilter.ForeColor = Color.Black
        cbfilter.ItemHeight = 30
        cbfilter.Location = New Point(1090, 18)
        cbfilter.Name = "cbfilter"
        cbfilter.ShadowDecoration.CustomizableEdges = CustomizableEdges2
        cbfilter.Size = New Size(169, 36)
        cbfilter.TabIndex = 95
        ' 
        ' Guna2GradientPanel1
        ' 
        Guna2GradientPanel1.BorderColor = Color.FromArgb(CByte(207), CByte(58), CByte(109))
        Guna2GradientPanel1.BorderRadius = 9
        Guna2GradientPanel1.BorderThickness = 2
        Guna2GradientPanel1.Controls.Add(DataGridView1)
        Guna2GradientPanel1.CustomizableEdges = CustomizableEdges5
        Guna2GradientPanel1.Location = New Point(25, 68)
        Guna2GradientPanel1.Name = "Guna2GradientPanel1"
        Guna2GradientPanel1.ShadowDecoration.CustomizableEdges = CustomizableEdges6
        Guna2GradientPanel1.Size = New Size(1283, 541)
        Guna2GradientPanel1.TabIndex = 45
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
        DataGridView1.Location = New Point(24, 24)
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
        DataGridView1.Size = New Size(1234, 497)
        DataGridView1.TabIndex = 25
        ' 
        ' Guna2Elipse1
        ' 
        Guna2Elipse1.BorderRadius = 9
        Guna2Elipse1.TargetControl = DataGridView1
        ' 
        ' Label3
        ' 
        Label3.AutoSize = True
        Label3.Font = New Font("Tahoma", 9.75F, FontStyle.Bold, GraphicsUnit.Point, CByte(0))
        Label3.ForeColor = Color.FromArgb(CByte(207), CByte(58), CByte(109))
        Label3.Location = New Point(1031, 28)
        Label3.Name = "Label3"
        Label3.Size = New Size(53, 16)
        Label3.TabIndex = 94
        Label3.Text = "FILTER:"
        ' 
        ' Label13
        ' 
        Label13.AutoSize = True
        Label13.Font = New Font("Tahoma", 9.75F, FontStyle.Bold, GraphicsUnit.Point, CByte(0))
        Label13.ForeColor = Color.FromArgb(CByte(207), CByte(58), CByte(109))
        Label13.Location = New Point(15, 9)
        Label13.Name = "Label13"
        Label13.Size = New Size(73, 16)
        Label13.TabIndex = 92
        Label13.Text = "Audit Trail"
        ' 
        ' Guna2Panel1
        ' 
        Guna2Panel1.BorderColor = Color.FromArgb(CByte(207), CByte(58), CByte(109))
        Guna2Panel1.BorderRadius = 5
        Guna2Panel1.BorderStyle = Drawing2D.DashStyle.Dash
        Guna2Panel1.BorderThickness = 2
        Guna2Panel1.Controls.Add(Label13)
        Guna2Panel1.CustomizableEdges = CustomizableEdges3
        Guna2Panel1.Location = New Point(30, 18)
        Guna2Panel1.Name = "Guna2Panel1"
        Guna2Panel1.ShadowDecoration.CustomizableEdges = CustomizableEdges4
        Guna2Panel1.Size = New Size(103, 35)
        Guna2Panel1.TabIndex = 93
        ' 
        ' AuditTrail
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        ClientSize = New Size(1370, 700)
        Controls.Add(panel_timein)
        FormBorderStyle = FormBorderStyle.None
        MaximizeBox = False
        MinimizeBox = False
        Name = "AuditTrail"
        ShowIcon = False
        StartPosition = FormStartPosition.WindowsDefaultBounds
        Text = "AuditTrail"
        panel_timein.ResumeLayout(False)
        Guna2GroupBox1.ResumeLayout(False)
        Guna2GroupBox1.PerformLayout()
        Guna2GradientPanel1.ResumeLayout(False)
        CType(DataGridView1, ComponentModel.ISupportInitialize).EndInit()
        Guna2Panel1.ResumeLayout(False)
        Guna2Panel1.PerformLayout()
        ResumeLayout(False)
    End Sub

    Friend WithEvents panel_timein As Panel
    Friend WithEvents Guna2GroupBox1 As Guna.UI2.WinForms.Guna2GroupBox
    Friend WithEvents Guna2GradientPanel1 As Guna.UI2.WinForms.Guna2GradientPanel
    Friend WithEvents DataGridView1 As DataGridView
    Friend WithEvents Guna2Elipse1 As Guna.UI2.WinForms.Guna2Elipse
    Friend WithEvents cbfilter As Guna.UI2.WinForms.Guna2ComboBox
    Friend WithEvents Label3 As Label
    Friend WithEvents Guna2Panel1 As Guna.UI2.WinForms.Guna2Panel
    Friend WithEvents Label13 As Label
End Class
