<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Shelf
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
        Dim CustomizableEdges33 As Guna.UI2.WinForms.Suite.CustomizableEdges = New Guna.UI2.WinForms.Suite.CustomizableEdges()
        Dim CustomizableEdges34 As Guna.UI2.WinForms.Suite.CustomizableEdges = New Guna.UI2.WinForms.Suite.CustomizableEdges()
        Dim CustomizableEdges25 As Guna.UI2.WinForms.Suite.CustomizableEdges = New Guna.UI2.WinForms.Suite.CustomizableEdges()
        Dim CustomizableEdges26 As Guna.UI2.WinForms.Suite.CustomizableEdges = New Guna.UI2.WinForms.Suite.CustomizableEdges()
        Dim CustomizableEdges27 As Guna.UI2.WinForms.Suite.CustomizableEdges = New Guna.UI2.WinForms.Suite.CustomizableEdges()
        Dim CustomizableEdges28 As Guna.UI2.WinForms.Suite.CustomizableEdges = New Guna.UI2.WinForms.Suite.CustomizableEdges()
        Dim CustomizableEdges29 As Guna.UI2.WinForms.Suite.CustomizableEdges = New Guna.UI2.WinForms.Suite.CustomizableEdges()
        Dim CustomizableEdges30 As Guna.UI2.WinForms.Suite.CustomizableEdges = New Guna.UI2.WinForms.Suite.CustomizableEdges()
        Dim CustomizableEdges31 As Guna.UI2.WinForms.Suite.CustomizableEdges = New Guna.UI2.WinForms.Suite.CustomizableEdges()
        Dim CustomizableEdges32 As Guna.UI2.WinForms.Suite.CustomizableEdges = New Guna.UI2.WinForms.Suite.CustomizableEdges()
        Dim CustomizableEdges35 As Guna.UI2.WinForms.Suite.CustomizableEdges = New Guna.UI2.WinForms.Suite.CustomizableEdges()
        Dim CustomizableEdges36 As Guna.UI2.WinForms.Suite.CustomizableEdges = New Guna.UI2.WinForms.Suite.CustomizableEdges()
        Dim DataGridViewCellStyle9 As DataGridViewCellStyle = New DataGridViewCellStyle()
        Dim DataGridViewCellStyle10 As DataGridViewCellStyle = New DataGridViewCellStyle()
        Dim DataGridViewCellStyle11 As DataGridViewCellStyle = New DataGridViewCellStyle()
        Dim DataGridViewCellStyle12 As DataGridViewCellStyle = New DataGridViewCellStyle()
        Guna2GradientPanel1 = New Guna.UI2.WinForms.Guna2GradientPanel()
        Label1 = New Label()
        txtshelf = New Guna.UI2.WinForms.Guna2TextBox()
        btndelete = New Guna.UI2.WinForms.Guna2Button()
        btnedit = New Guna.UI2.WinForms.Guna2Button()
        btnadd = New Guna.UI2.WinForms.Guna2Button()
        PictureBox1 = New PictureBox()
        txtsearch = New Guna.UI2.WinForms.Guna2TextBox()
        Guna2Elipse1 = New Guna.UI2.WinForms.Guna2Elipse(components)
        DataGridView1 = New DataGridView()
        Label12 = New Label()
        Label13 = New Label()
        Guna2GradientPanel1.SuspendLayout()
        CType(PictureBox1, ComponentModel.ISupportInitialize).BeginInit()
        CType(DataGridView1, ComponentModel.ISupportInitialize).BeginInit()
        SuspendLayout()
        ' 
        ' Guna2GradientPanel1
        ' 
        Guna2GradientPanel1.BorderColor = Color.FromArgb(CByte(207), CByte(58), CByte(109))
        Guna2GradientPanel1.BorderRadius = 9
        Guna2GradientPanel1.BorderThickness = 2
        Guna2GradientPanel1.Controls.Add(Label1)
        Guna2GradientPanel1.Controls.Add(txtshelf)
        Guna2GradientPanel1.Controls.Add(btndelete)
        Guna2GradientPanel1.Controls.Add(btnedit)
        Guna2GradientPanel1.Controls.Add(btnadd)
        Guna2GradientPanel1.CustomizableEdges = CustomizableEdges33
        Guna2GradientPanel1.Location = New Point(27, 70)
        Guna2GradientPanel1.Name = "Guna2GradientPanel1"
        Guna2GradientPanel1.ShadowDecoration.CustomizableEdges = CustomizableEdges34
        Guna2GradientPanel1.Size = New Size(415, 150)
        Guna2GradientPanel1.TabIndex = 30
        ' 
        ' Label1
        ' 
        Label1.AutoSize = True
        Label1.Font = New Font("Tahoma", 9.75F, FontStyle.Bold, GraphicsUnit.Point, CByte(0))
        Label1.ForeColor = Color.FromArgb(CByte(207), CByte(58), CByte(109))
        Label1.Location = New Point(36, 27)
        Label1.Name = "Label1"
        Label1.Size = New Size(44, 16)
        Label1.TabIndex = 4
        Label1.Text = "Shelf:"
        ' 
        ' txtshelf
        ' 
        txtshelf.BorderColor = Color.FromArgb(CByte(207), CByte(58), CByte(109))
        txtshelf.BorderRadius = 12
        txtshelf.CustomizableEdges = CustomizableEdges25
        txtshelf.DefaultText = ""
        txtshelf.DisabledState.BorderColor = Color.FromArgb(CByte(208), CByte(208), CByte(208))
        txtshelf.DisabledState.FillColor = Color.FromArgb(CByte(226), CByte(226), CByte(226))
        txtshelf.DisabledState.ForeColor = Color.FromArgb(CByte(138), CByte(138), CByte(138))
        txtshelf.DisabledState.PlaceholderForeColor = Color.FromArgb(CByte(138), CByte(138), CByte(138))
        txtshelf.FocusedState.BorderColor = Color.FromArgb(CByte(94), CByte(148), CByte(255))
        txtshelf.Font = New Font("Microsoft Sans Serif", 9F)
        txtshelf.ForeColor = Color.Black
        txtshelf.HoverState.BorderColor = Color.FromArgb(CByte(94), CByte(148), CByte(255))
        txtshelf.Location = New Point(36, 55)
        txtshelf.Margin = New Padding(3, 4, 3, 4)
        txtshelf.MaxLength = 2
        txtshelf.Name = "txtshelf"
        txtshelf.PlaceholderText = ""
        txtshelf.SelectedText = ""
        txtshelf.ShadowDecoration.CustomizableEdges = CustomizableEdges26
        txtshelf.Size = New Size(347, 33)
        txtshelf.TabIndex = 3
        ' 
        ' btndelete
        ' 
        btndelete.BorderColor = Color.FromArgb(CByte(207), CByte(58), CByte(109))
        btndelete.BorderRadius = 9
        btndelete.BorderThickness = 1
        btndelete.CustomizableEdges = CustomizableEdges27
        btndelete.DisabledState.BorderColor = Color.DarkGray
        btndelete.DisabledState.CustomBorderColor = Color.DarkGray
        btndelete.DisabledState.FillColor = Color.FromArgb(CByte(169), CByte(169), CByte(169))
        btndelete.DisabledState.ForeColor = Color.FromArgb(CByte(141), CByte(141), CByte(141))
        btndelete.FillColor = Color.Empty
        btndelete.Font = New Font("Tahoma", 11.25F, FontStyle.Bold)
        btndelete.ForeColor = Color.FromArgb(CByte(207), CByte(58), CByte(109))
        btndelete.Location = New Point(290, 98)
        btndelete.Name = "btndelete"
        btndelete.ShadowDecoration.CustomizableEdges = CustomizableEdges28
        btndelete.Size = New Size(90, 27)
        btndelete.TabIndex = 2
        btndelete.Text = "DELETE"
        ' 
        ' btnedit
        ' 
        btnedit.BorderColor = Color.FromArgb(CByte(207), CByte(58), CByte(109))
        btnedit.BorderRadius = 9
        btnedit.BorderThickness = 1
        btnedit.CustomizableEdges = CustomizableEdges29
        btnedit.DisabledState.BorderColor = Color.DarkGray
        btnedit.DisabledState.CustomBorderColor = Color.DarkGray
        btnedit.DisabledState.FillColor = Color.FromArgb(CByte(169), CByte(169), CByte(169))
        btnedit.DisabledState.ForeColor = Color.FromArgb(CByte(141), CByte(141), CByte(141))
        btnedit.FillColor = Color.Empty
        btnedit.Font = New Font("Tahoma", 11.25F, FontStyle.Bold)
        btnedit.ForeColor = Color.FromArgb(CByte(207), CByte(58), CByte(109))
        btnedit.Location = New Point(160, 98)
        btnedit.Name = "btnedit"
        btnedit.ShadowDecoration.CustomizableEdges = CustomizableEdges30
        btnedit.Size = New Size(90, 27)
        btnedit.TabIndex = 1
        btnedit.Text = "EDIT"
        ' 
        ' btnadd
        ' 
        btnadd.BorderColor = Color.FromArgb(CByte(207), CByte(58), CByte(109))
        btnadd.BorderRadius = 9
        btnadd.BorderThickness = 1
        btnadd.CustomizableEdges = CustomizableEdges31
        btnadd.DisabledState.BorderColor = Color.DarkGray
        btnadd.DisabledState.CustomBorderColor = Color.DarkGray
        btnadd.DisabledState.FillColor = Color.FromArgb(CByte(169), CByte(169), CByte(169))
        btnadd.DisabledState.ForeColor = Color.FromArgb(CByte(141), CByte(141), CByte(141))
        btnadd.FillColor = Color.Empty
        btnadd.Font = New Font("Tahoma", 11.25F, FontStyle.Bold)
        btnadd.ForeColor = Color.FromArgb(CByte(207), CByte(58), CByte(109))
        btnadd.Location = New Point(36, 98)
        btnadd.Name = "btnadd"
        btnadd.ShadowDecoration.CustomizableEdges = CustomizableEdges32
        btnadd.Size = New Size(90, 27)
        btnadd.TabIndex = 0
        btnadd.Text = "ADD"
        ' 
        ' PictureBox1
        ' 
        PictureBox1.BackColor = Color.White
        PictureBox1.BackgroundImage = My.Resources.Resources.magnifier
        PictureBox1.BackgroundImageLayout = ImageLayout.Stretch
        PictureBox1.Location = New Point(416, 27)
        PictureBox1.Name = "PictureBox1"
        PictureBox1.Size = New Size(20, 18)
        PictureBox1.TabIndex = 28
        PictureBox1.TabStop = False
        ' 
        ' txtsearch
        ' 
        txtsearch.BorderColor = Color.FromArgb(CByte(207), CByte(58), CByte(109))
        txtsearch.BorderRadius = 16
        txtsearch.CustomizableEdges = CustomizableEdges35
        txtsearch.DefaultText = ""
        txtsearch.DisabledState.BorderColor = Color.FromArgb(CByte(208), CByte(208), CByte(208))
        txtsearch.DisabledState.FillColor = Color.FromArgb(CByte(226), CByte(226), CByte(226))
        txtsearch.DisabledState.ForeColor = Color.FromArgb(CByte(138), CByte(138), CByte(138))
        txtsearch.DisabledState.PlaceholderForeColor = Color.FromArgb(CByte(138), CByte(138), CByte(138))
        txtsearch.FocusedState.BorderColor = Color.FromArgb(CByte(94), CByte(148), CByte(255))
        txtsearch.Font = New Font("Baskerville Old Face", 12F, FontStyle.Bold)
        txtsearch.ForeColor = Color.Black
        txtsearch.HoverState.BorderColor = Color.FromArgb(CByte(94), CByte(148), CByte(255))
        txtsearch.Location = New Point(26, 21)
        txtsearch.Margin = New Padding(4)
        txtsearch.Name = "txtsearch"
        txtsearch.PlaceholderText = "Search Shelf Number"
        txtsearch.SelectedText = ""
        txtsearch.ShadowDecoration.CustomizableEdges = CustomizableEdges36
        txtsearch.Size = New Size(420, 30)
        txtsearch.TabIndex = 29
        ' 
        ' Guna2Elipse1
        ' 
        Guna2Elipse1.BorderRadius = 9
        Guna2Elipse1.TargetControl = DataGridView1
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
        DataGridViewCellStyle9.Alignment = DataGridViewContentAlignment.TopCenter
        DataGridViewCellStyle9.BackColor = Color.FromArgb(CByte(207), CByte(58), CByte(109))
        DataGridViewCellStyle9.Font = New Font("Tahoma", 9.75F, FontStyle.Bold)
        DataGridViewCellStyle9.ForeColor = Color.White
        DataGridViewCellStyle9.SelectionBackColor = Color.FromArgb(CByte(207), CByte(58), CByte(109))
        DataGridViewCellStyle9.SelectionForeColor = Color.White
        DataGridViewCellStyle9.WrapMode = DataGridViewTriState.True
        DataGridView1.ColumnHeadersDefaultCellStyle = DataGridViewCellStyle9
        DataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize
        DataGridView1.Cursor = Cursors.Hand
        DataGridViewCellStyle10.Alignment = DataGridViewContentAlignment.TopCenter
        DataGridViewCellStyle10.BackColor = SystemColors.Window
        DataGridViewCellStyle10.Font = New Font("Segoe UI", 9F)
        DataGridViewCellStyle10.ForeColor = Color.White
        DataGridViewCellStyle10.SelectionBackColor = SystemColors.Highlight
        DataGridViewCellStyle10.SelectionForeColor = SystemColors.HighlightText
        DataGridViewCellStyle10.WrapMode = DataGridViewTriState.False
        DataGridView1.DefaultCellStyle = DataGridViewCellStyle10
        DataGridView1.Location = New Point(29, 241)
        DataGridView1.MultiSelect = False
        DataGridView1.Name = "DataGridView1"
        DataGridView1.ReadOnly = True
        DataGridViewCellStyle11.Alignment = DataGridViewContentAlignment.TopCenter
        DataGridViewCellStyle11.BackColor = SystemColors.Control
        DataGridViewCellStyle11.Font = New Font("Segoe UI", 9F)
        DataGridViewCellStyle11.ForeColor = SystemColors.WindowText
        DataGridViewCellStyle11.SelectionBackColor = SystemColors.Highlight
        DataGridViewCellStyle11.SelectionForeColor = SystemColors.HighlightText
        DataGridViewCellStyle11.WrapMode = DataGridViewTriState.True
        DataGridView1.RowHeadersDefaultCellStyle = DataGridViewCellStyle11
        DataGridView1.RowHeadersVisible = False
        DataGridView1.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing
        DataGridViewCellStyle12.Alignment = DataGridViewContentAlignment.TopCenter
        DataGridViewCellStyle12.Font = New Font("Tahoma", 9F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        DataGridViewCellStyle12.ForeColor = Color.Black
        DataGridView1.RowsDefaultCellStyle = DataGridViewCellStyle12
        DataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        DataGridView1.Size = New Size(420, 150)
        DataGridView1.TabIndex = 31
        ' 
        ' Label12
        ' 
        Label12.AutoSize = True
        Label12.Font = New Font("Tahoma", 9.75F, FontStyle.Bold, GraphicsUnit.Point, CByte(0))
        Label12.ForeColor = Color.Black
        Label12.Location = New Point(32, 421)
        Label12.Name = "Label12"
        Label12.Size = New Size(42, 16)
        Label12.TabIndex = 110
        Label12.Text = "Note:"
        ' 
        ' Label13
        ' 
        Label13.AutoSize = True
        Label13.Font = New Font("Tahoma", 9.75F, FontStyle.Bold, GraphicsUnit.Point, CByte(0))
        Label13.ForeColor = Color.Red
        Label13.Location = New Point(80, 422)
        Label13.Name = "Label13"
        Label13.Size = New Size(296, 16)
        Label13.TabIndex = 109
        Label13.Text = "Select Row before clicking [Edit] or [Delete]."
        ' 
        ' Shelf
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        ClientSize = New Size(474, 447)
        Controls.Add(Label12)
        Controls.Add(Label13)
        Controls.Add(DataGridView1)
        Controls.Add(Guna2GradientPanel1)
        Controls.Add(PictureBox1)
        Controls.Add(txtsearch)
        FormBorderStyle = FormBorderStyle.FixedDialog
        KeyPreview = True
        MaximizeBox = False
        MinimizeBox = False
        Name = "Shelf"
        ShowIcon = False
        StartPosition = FormStartPosition.CenterScreen
        Text = "Shelf"
        Guna2GradientPanel1.ResumeLayout(False)
        Guna2GradientPanel1.PerformLayout()
        CType(PictureBox1, ComponentModel.ISupportInitialize).EndInit()
        CType(DataGridView1, ComponentModel.ISupportInitialize).EndInit()
        ResumeLayout(False)
        PerformLayout()
    End Sub

    Friend WithEvents Guna2GradientPanel1 As Guna.UI2.WinForms.Guna2GradientPanel
    Friend WithEvents Label1 As Label
    Friend WithEvents txtshelf As Guna.UI2.WinForms.Guna2TextBox
    Friend WithEvents btndelete As Guna.UI2.WinForms.Guna2Button
    Friend WithEvents btnedit As Guna.UI2.WinForms.Guna2Button
    Friend WithEvents btnadd As Guna.UI2.WinForms.Guna2Button
    Friend WithEvents PictureBox1 As PictureBox
    Friend WithEvents txtsearch As Guna.UI2.WinForms.Guna2TextBox
    Friend WithEvents Guna2Elipse1 As Guna.UI2.WinForms.Guna2Elipse
    Friend WithEvents DataGridView1 As DataGridView
    Friend WithEvents Label12 As Label
    Friend WithEvents Label13 As Label
End Class
