<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Department
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
        Dim CustomizableEdges5 As Guna.UI2.WinForms.Suite.CustomizableEdges = New Guna.UI2.WinForms.Suite.CustomizableEdges()
        Dim CustomizableEdges6 As Guna.UI2.WinForms.Suite.CustomizableEdges = New Guna.UI2.WinForms.Suite.CustomizableEdges()
        Dim CustomizableEdges1 As Guna.UI2.WinForms.Suite.CustomizableEdges = New Guna.UI2.WinForms.Suite.CustomizableEdges()
        Dim CustomizableEdges2 As Guna.UI2.WinForms.Suite.CustomizableEdges = New Guna.UI2.WinForms.Suite.CustomizableEdges()
        Dim CustomizableEdges3 As Guna.UI2.WinForms.Suite.CustomizableEdges = New Guna.UI2.WinForms.Suite.CustomizableEdges()
        Dim CustomizableEdges4 As Guna.UI2.WinForms.Suite.CustomizableEdges = New Guna.UI2.WinForms.Suite.CustomizableEdges()
        Dim CustomizableEdges7 As Guna.UI2.WinForms.Suite.CustomizableEdges = New Guna.UI2.WinForms.Suite.CustomizableEdges()
        Dim CustomizableEdges8 As Guna.UI2.WinForms.Suite.CustomizableEdges = New Guna.UI2.WinForms.Suite.CustomizableEdges()
        Dim DataGridViewCellStyle1 As DataGridViewCellStyle = New DataGridViewCellStyle()
        Dim DataGridViewCellStyle2 As DataGridViewCellStyle = New DataGridViewCellStyle()
        Dim DataGridViewCellStyle3 As DataGridViewCellStyle = New DataGridViewCellStyle()
        Dim DataGridViewCellStyle4 As DataGridViewCellStyle = New DataGridViewCellStyle()
        Guna2GradientPanel1 = New Guna.UI2.WinForms.Guna2GradientPanel()
        Label1 = New Label()
        txtdepartment = New Guna.UI2.WinForms.Guna2TextBox()
        btnadd = New Guna.UI2.WinForms.Guna2Button()
        PictureBox1 = New PictureBox()
        txtsearch = New Guna.UI2.WinForms.Guna2TextBox()
        DataGridView1 = New DataGridView()
        Edit = New DataGridViewButtonColumn()
        Delete = New DataGridViewButtonColumn()
        Guna2Elipse1 = New Guna.UI2.WinForms.Guna2Elipse(components)
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
        Guna2GradientPanel1.Controls.Add(txtdepartment)
        Guna2GradientPanel1.Controls.Add(btnadd)
        Guna2GradientPanel1.CustomizableEdges = CustomizableEdges5
        Guna2GradientPanel1.Location = New Point(30, 20)
        Guna2GradientPanel1.Name = "Guna2GradientPanel1"
        Guna2GradientPanel1.ShadowDecoration.CustomizableEdges = CustomizableEdges6
        Guna2GradientPanel1.Size = New Size(415, 150)
        Guna2GradientPanel1.TabIndex = 22
        ' 
        ' Label1
        ' 
        Label1.AutoSize = True
        Label1.Font = New Font("Tahoma", 9.75F, FontStyle.Bold, GraphicsUnit.Point, CByte(0))
        Label1.ForeColor = Color.FromArgb(CByte(207), CByte(58), CByte(109))
        Label1.Location = New Point(36, 27)
        Label1.Name = "Label1"
        Label1.Size = New Size(90, 16)
        Label1.TabIndex = 4
        Label1.Text = "Department:"
        ' 
        ' txtdepartment
        ' 
        txtdepartment.BorderColor = Color.FromArgb(CByte(207), CByte(58), CByte(109))
        txtdepartment.BorderRadius = 12
        txtdepartment.CustomizableEdges = CustomizableEdges1
        txtdepartment.DefaultText = ""
        txtdepartment.DisabledState.BorderColor = Color.FromArgb(CByte(208), CByte(208), CByte(208))
        txtdepartment.DisabledState.FillColor = Color.FromArgb(CByte(226), CByte(226), CByte(226))
        txtdepartment.DisabledState.ForeColor = Color.FromArgb(CByte(138), CByte(138), CByte(138))
        txtdepartment.DisabledState.PlaceholderForeColor = Color.FromArgb(CByte(138), CByte(138), CByte(138))
        txtdepartment.FocusedState.BorderColor = Color.FromArgb(CByte(94), CByte(148), CByte(255))
        txtdepartment.Font = New Font("Microsoft Sans Serif", 9F)
        txtdepartment.ForeColor = Color.Black
        txtdepartment.HoverState.BorderColor = Color.FromArgb(CByte(94), CByte(148), CByte(255))
        txtdepartment.Location = New Point(36, 55)
        txtdepartment.Margin = New Padding(3, 4, 3, 4)
        txtdepartment.MaxLength = 20
        txtdepartment.Name = "txtdepartment"
        txtdepartment.PlaceholderText = ""
        txtdepartment.SelectedText = ""
        txtdepartment.ShadowDecoration.CustomizableEdges = CustomizableEdges2
        txtdepartment.Size = New Size(347, 33)
        txtdepartment.TabIndex = 3
        ' 
        ' btnadd
        ' 
        btnadd.BorderColor = Color.FromArgb(CByte(207), CByte(58), CByte(109))
        btnadd.BorderRadius = 9
        btnadd.BorderThickness = 1
        btnadd.CustomizableEdges = CustomizableEdges3
        btnadd.DisabledState.BorderColor = Color.DarkGray
        btnadd.DisabledState.CustomBorderColor = Color.DarkGray
        btnadd.DisabledState.FillColor = Color.FromArgb(CByte(169), CByte(169), CByte(169))
        btnadd.DisabledState.ForeColor = Color.FromArgb(CByte(141), CByte(141), CByte(141))
        btnadd.FillColor = Color.Empty
        btnadd.Font = New Font("Tahoma", 11.25F, FontStyle.Bold)
        btnadd.ForeColor = Color.FromArgb(CByte(207), CByte(58), CByte(109))
        btnadd.Location = New Point(138, 98)
        btnadd.Name = "btnadd"
        btnadd.ShadowDecoration.CustomizableEdges = CustomizableEdges4
        btnadd.Size = New Size(141, 27)
        btnadd.TabIndex = 0
        btnadd.Text = "SAVE"
        ' 
        ' PictureBox1
        ' 
        PictureBox1.BackColor = Color.White
        PictureBox1.BackgroundImage = My.Resources.Resources.magnifier
        PictureBox1.BackgroundImageLayout = ImageLayout.Stretch
        PictureBox1.Location = New Point(416, 210)
        PictureBox1.Name = "PictureBox1"
        PictureBox1.Size = New Size(20, 18)
        PictureBox1.TabIndex = 20
        PictureBox1.TabStop = False
        ' 
        ' txtsearch
        ' 
        txtsearch.BorderColor = Color.FromArgb(CByte(207), CByte(58), CByte(109))
        txtsearch.BorderRadius = 16
        txtsearch.CustomizableEdges = CustomizableEdges7
        txtsearch.DefaultText = ""
        txtsearch.DisabledState.BorderColor = Color.FromArgb(CByte(208), CByte(208), CByte(208))
        txtsearch.DisabledState.FillColor = Color.FromArgb(CByte(226), CByte(226), CByte(226))
        txtsearch.DisabledState.ForeColor = Color.FromArgb(CByte(138), CByte(138), CByte(138))
        txtsearch.DisabledState.PlaceholderForeColor = Color.FromArgb(CByte(138), CByte(138), CByte(138))
        txtsearch.FocusedState.BorderColor = Color.FromArgb(CByte(94), CByte(148), CByte(255))
        txtsearch.Font = New Font("Baskerville Old Face", 12F, FontStyle.Bold)
        txtsearch.ForeColor = Color.Black
        txtsearch.HoverState.BorderColor = Color.FromArgb(CByte(94), CByte(148), CByte(255))
        txtsearch.Location = New Point(25, 203)
        txtsearch.Margin = New Padding(4)
        txtsearch.Name = "txtsearch"
        txtsearch.PlaceholderText = "Search Department"
        txtsearch.SelectedText = ""
        txtsearch.ShadowDecoration.CustomizableEdges = CustomizableEdges8
        txtsearch.Size = New Size(420, 30)
        txtsearch.TabIndex = 21
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
        DataGridView1.Columns.AddRange(New DataGridViewColumn() {Edit, Delete})
        DataGridView1.Cursor = Cursors.Hand
        DataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.TopCenter
        DataGridViewCellStyle2.BackColor = SystemColors.Window
        DataGridViewCellStyle2.Font = New Font("Segoe UI", 9F)
        DataGridViewCellStyle2.ForeColor = Color.White
        DataGridViewCellStyle2.SelectionBackColor = SystemColors.Highlight
        DataGridViewCellStyle2.SelectionForeColor = SystemColors.HighlightText
        DataGridViewCellStyle2.WrapMode = DataGridViewTriState.False
        DataGridView1.DefaultCellStyle = DataGridViewCellStyle2
        DataGridView1.Location = New Point(29, 241)
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
        DataGridView1.Size = New Size(420, 150)
        DataGridView1.TabIndex = 28
        ' 
        ' Edit
        ' 
        Edit.FillWeight = 20F
        Edit.HeaderText = "Edit"
        Edit.Name = "Edit"
        Edit.ReadOnly = True
        Edit.Text = "✎"
        Edit.UseColumnTextForButtonValue = True
        ' 
        ' Delete
        ' 
        Delete.FillWeight = 20F
        Delete.HeaderText = "Delete"
        Delete.Name = "Delete"
        Delete.ReadOnly = True
        Delete.Text = "🗑"
        Delete.UseColumnTextForButtonValue = True
        ' 
        ' Guna2Elipse1
        ' 
        Guna2Elipse1.BorderRadius = 9
        Guna2Elipse1.TargetControl = DataGridView1
        ' 
        ' Label12
        ' 
        Label12.AutoSize = True
        Label12.Font = New Font("Tahoma", 9.75F, FontStyle.Bold, GraphicsUnit.Point, CByte(0))
        Label12.ForeColor = Color.Black
        Label12.Location = New Point(32, 421)
        Label12.Name = "Label12"
        Label12.Size = New Size(42, 16)
        Label12.TabIndex = 108
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
        Label13.TabIndex = 107
        Label13.Text = "Select Row before clicking [Edit] or [Delete]."
        ' 
        ' Department
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
        Name = "Department"
        ShowIcon = False
        StartPosition = FormStartPosition.CenterScreen
        Text = "Department"
        Guna2GradientPanel1.ResumeLayout(False)
        Guna2GradientPanel1.PerformLayout()
        CType(PictureBox1, ComponentModel.ISupportInitialize).EndInit()
        CType(DataGridView1, ComponentModel.ISupportInitialize).EndInit()
        ResumeLayout(False)
        PerformLayout()
    End Sub
    Friend WithEvents Guna2GradientPanel1 As Guna.UI2.WinForms.Guna2GradientPanel
    Friend WithEvents Label1 As Label
    Friend WithEvents txtdepartment As Guna.UI2.WinForms.Guna2TextBox
    Friend WithEvents btnadd As Guna.UI2.WinForms.Guna2Button
    Friend WithEvents PictureBox1 As PictureBox
    Friend WithEvents txtsearch As Guna.UI2.WinForms.Guna2TextBox
    Friend WithEvents DataGridView1 As DataGridView
    Friend WithEvents Guna2Elipse1 As Guna.UI2.WinForms.Guna2Elipse
    Friend WithEvents Label12 As Label
    Friend WithEvents Label13 As Label
    Friend WithEvents Edit As DataGridViewButtonColumn
    Friend WithEvents Delete As DataGridViewButtonColumn
End Class
