<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Genre
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
        Dim CustomizableEdges21 As Guna.UI2.WinForms.Suite.CustomizableEdges = New Guna.UI2.WinForms.Suite.CustomizableEdges()
        Dim CustomizableEdges22 As Guna.UI2.WinForms.Suite.CustomizableEdges = New Guna.UI2.WinForms.Suite.CustomizableEdges()
        Dim CustomizableEdges13 As Guna.UI2.WinForms.Suite.CustomizableEdges = New Guna.UI2.WinForms.Suite.CustomizableEdges()
        Dim CustomizableEdges14 As Guna.UI2.WinForms.Suite.CustomizableEdges = New Guna.UI2.WinForms.Suite.CustomizableEdges()
        Dim CustomizableEdges15 As Guna.UI2.WinForms.Suite.CustomizableEdges = New Guna.UI2.WinForms.Suite.CustomizableEdges()
        Dim CustomizableEdges16 As Guna.UI2.WinForms.Suite.CustomizableEdges = New Guna.UI2.WinForms.Suite.CustomizableEdges()
        Dim CustomizableEdges17 As Guna.UI2.WinForms.Suite.CustomizableEdges = New Guna.UI2.WinForms.Suite.CustomizableEdges()
        Dim CustomizableEdges18 As Guna.UI2.WinForms.Suite.CustomizableEdges = New Guna.UI2.WinForms.Suite.CustomizableEdges()
        Dim CustomizableEdges19 As Guna.UI2.WinForms.Suite.CustomizableEdges = New Guna.UI2.WinForms.Suite.CustomizableEdges()
        Dim CustomizableEdges20 As Guna.UI2.WinForms.Suite.CustomizableEdges = New Guna.UI2.WinForms.Suite.CustomizableEdges()
        Dim CustomizableEdges23 As Guna.UI2.WinForms.Suite.CustomizableEdges = New Guna.UI2.WinForms.Suite.CustomizableEdges()
        Dim CustomizableEdges24 As Guna.UI2.WinForms.Suite.CustomizableEdges = New Guna.UI2.WinForms.Suite.CustomizableEdges()
        Dim DataGridViewCellStyle5 As DataGridViewCellStyle = New DataGridViewCellStyle()
        Dim DataGridViewCellStyle6 As DataGridViewCellStyle = New DataGridViewCellStyle()
        Dim DataGridViewCellStyle7 As DataGridViewCellStyle = New DataGridViewCellStyle()
        Dim DataGridViewCellStyle8 As DataGridViewCellStyle = New DataGridViewCellStyle()
        Guna2GradientPanel1 = New Guna.UI2.WinForms.Guna2GradientPanel()
        Label1 = New Label()
        txtgenre = New Guna.UI2.WinForms.Guna2TextBox()
        btndelete = New Guna.UI2.WinForms.Guna2Button()
        btnedit = New Guna.UI2.WinForms.Guna2Button()
        btnadd = New Guna.UI2.WinForms.Guna2Button()
        PictureBox1 = New PictureBox()
        txtsearch = New Guna.UI2.WinForms.Guna2TextBox()
        Guna2Elipse1 = New Guna.UI2.WinForms.Guna2Elipse(components)
        DataGridView1 = New DataGridView()
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
        Guna2GradientPanel1.Controls.Add(txtgenre)
        Guna2GradientPanel1.Controls.Add(btndelete)
        Guna2GradientPanel1.Controls.Add(btnedit)
        Guna2GradientPanel1.Controls.Add(btnadd)
        Guna2GradientPanel1.CustomizableEdges = CustomizableEdges21
        Guna2GradientPanel1.Location = New Point(30, 69)
        Guna2GradientPanel1.Name = "Guna2GradientPanel1"
        Guna2GradientPanel1.ShadowDecoration.CustomizableEdges = CustomizableEdges22
        Guna2GradientPanel1.Size = New Size(415, 150)
        Guna2GradientPanel1.TabIndex = 18
        ' 
        ' Label1
        ' 
        Label1.AutoSize = True
        Label1.Font = New Font("Tahoma", 9.75F, FontStyle.Bold, GraphicsUnit.Point, CByte(0))
        Label1.ForeColor = Color.FromArgb(CByte(207), CByte(58), CByte(109))
        Label1.Location = New Point(36, 18)
        Label1.Name = "Label1"
        Label1.Size = New Size(50, 16)
        Label1.TabIndex = 4
        Label1.Text = "Genre:"
        ' 
        ' txtgenre
        ' 
        txtgenre.BorderColor = Color.FromArgb(CByte(207), CByte(58), CByte(109))
        txtgenre.BorderRadius = 12
        txtgenre.CustomizableEdges = CustomizableEdges13
        txtgenre.DefaultText = ""
        txtgenre.DisabledState.BorderColor = Color.FromArgb(CByte(208), CByte(208), CByte(208))
        txtgenre.DisabledState.FillColor = Color.FromArgb(CByte(226), CByte(226), CByte(226))
        txtgenre.DisabledState.ForeColor = Color.FromArgb(CByte(138), CByte(138), CByte(138))
        txtgenre.DisabledState.PlaceholderForeColor = Color.FromArgb(CByte(138), CByte(138), CByte(138))
        txtgenre.FocusedState.BorderColor = Color.FromArgb(CByte(94), CByte(148), CByte(255))
        txtgenre.Font = New Font("Segoe UI", 9F)
        txtgenre.HoverState.BorderColor = Color.FromArgb(CByte(94), CByte(148), CByte(255))
        txtgenre.Location = New Point(36, 37)
        txtgenre.Name = "txtgenre"
        txtgenre.PlaceholderText = ""
        txtgenre.SelectedText = ""
        txtgenre.ShadowDecoration.CustomizableEdges = CustomizableEdges14
        txtgenre.Size = New Size(342, 33)
        txtgenre.TabIndex = 3
        ' 
        ' btndelete
        ' 
        btndelete.BorderColor = Color.FromArgb(CByte(207), CByte(58), CByte(109))
        btndelete.BorderRadius = 9
        btndelete.BorderThickness = 1
        btndelete.CustomizableEdges = CustomizableEdges15
        btndelete.DisabledState.BorderColor = Color.DarkGray
        btndelete.DisabledState.CustomBorderColor = Color.DarkGray
        btndelete.DisabledState.FillColor = Color.FromArgb(CByte(169), CByte(169), CByte(169))
        btndelete.DisabledState.ForeColor = Color.FromArgb(CByte(141), CByte(141), CByte(141))
        btndelete.FillColor = Color.Empty
        btndelete.Font = New Font("Tahoma", 11.25F, FontStyle.Bold)
        btndelete.ForeColor = Color.FromArgb(CByte(207), CByte(58), CByte(109))
        btndelete.Location = New Point(290, 98)
        btndelete.Name = "btndelete"
        btndelete.ShadowDecoration.CustomizableEdges = CustomizableEdges16
        btndelete.Size = New Size(90, 27)
        btndelete.TabIndex = 2
        btndelete.Text = "DELETE"
        ' 
        ' btnedit
        ' 
        btnedit.BorderColor = Color.FromArgb(CByte(207), CByte(58), CByte(109))
        btnedit.BorderRadius = 9
        btnedit.BorderThickness = 1
        btnedit.CustomizableEdges = CustomizableEdges17
        btnedit.DisabledState.BorderColor = Color.DarkGray
        btnedit.DisabledState.CustomBorderColor = Color.DarkGray
        btnedit.DisabledState.FillColor = Color.FromArgb(CByte(169), CByte(169), CByte(169))
        btnedit.DisabledState.ForeColor = Color.FromArgb(CByte(141), CByte(141), CByte(141))
        btnedit.FillColor = Color.Empty
        btnedit.Font = New Font("Tahoma", 11.25F, FontStyle.Bold)
        btnedit.ForeColor = Color.FromArgb(CByte(207), CByte(58), CByte(109))
        btnedit.Location = New Point(160, 98)
        btnedit.Name = "btnedit"
        btnedit.ShadowDecoration.CustomizableEdges = CustomizableEdges18
        btnedit.Size = New Size(90, 27)
        btnedit.TabIndex = 1
        btnedit.Text = "EDIT"
        ' 
        ' btnadd
        ' 
        btnadd.BorderColor = Color.FromArgb(CByte(207), CByte(58), CByte(109))
        btnadd.BorderRadius = 9
        btnadd.BorderThickness = 1
        btnadd.CustomizableEdges = CustomizableEdges19
        btnadd.DisabledState.BorderColor = Color.DarkGray
        btnadd.DisabledState.CustomBorderColor = Color.DarkGray
        btnadd.DisabledState.FillColor = Color.FromArgb(CByte(169), CByte(169), CByte(169))
        btnadd.DisabledState.ForeColor = Color.FromArgb(CByte(141), CByte(141), CByte(141))
        btnadd.FillColor = Color.Empty
        btnadd.Font = New Font("Tahoma", 11.25F, FontStyle.Bold)
        btnadd.ForeColor = Color.FromArgb(CByte(207), CByte(58), CByte(109))
        btnadd.Location = New Point(36, 98)
        btnadd.Name = "btnadd"
        btnadd.ShadowDecoration.CustomizableEdges = CustomizableEdges20
        btnadd.Size = New Size(90, 27)
        btnadd.TabIndex = 0
        btnadd.Text = "ADD"
        ' 
        ' PictureBox1
        ' 
        PictureBox1.BackColor = Color.White
        PictureBox1.BackgroundImage = My.Resources.Resources.magnifier
        PictureBox1.BackgroundImageLayout = ImageLayout.Stretch
        PictureBox1.Location = New Point(419, 26)
        PictureBox1.Name = "PictureBox1"
        PictureBox1.Size = New Size(20, 18)
        PictureBox1.TabIndex = 16
        PictureBox1.TabStop = False
        ' 
        ' txtsearch
        ' 
        txtsearch.BorderColor = Color.FromArgb(CByte(207), CByte(58), CByte(109))
        txtsearch.BorderRadius = 16
        txtsearch.CustomizableEdges = CustomizableEdges23
        txtsearch.DefaultText = ""
        txtsearch.DisabledState.BorderColor = Color.FromArgb(CByte(208), CByte(208), CByte(208))
        txtsearch.DisabledState.FillColor = Color.FromArgb(CByte(226), CByte(226), CByte(226))
        txtsearch.DisabledState.ForeColor = Color.FromArgb(CByte(138), CByte(138), CByte(138))
        txtsearch.DisabledState.PlaceholderForeColor = Color.FromArgb(CByte(138), CByte(138), CByte(138))
        txtsearch.FocusedState.BorderColor = Color.FromArgb(CByte(94), CByte(148), CByte(255))
        txtsearch.Font = New Font("Baskerville Old Face", 12F, FontStyle.Bold)
        txtsearch.HoverState.BorderColor = Color.FromArgb(CByte(94), CByte(148), CByte(255))
        txtsearch.Location = New Point(29, 20)
        txtsearch.Margin = New Padding(4)
        txtsearch.Name = "txtsearch"
        txtsearch.PlaceholderText = "Search Genre"
        txtsearch.SelectedText = ""
        txtsearch.ShadowDecoration.CustomizableEdges = CustomizableEdges24
        txtsearch.Size = New Size(420, 30)
        txtsearch.TabIndex = 17
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
        DataGridViewCellStyle5.Alignment = DataGridViewContentAlignment.TopCenter
        DataGridViewCellStyle5.BackColor = Color.RosyBrown
        DataGridViewCellStyle5.Font = New Font("Tahoma", 9.75F, FontStyle.Bold)
        DataGridViewCellStyle5.ForeColor = SystemColors.WindowText
        DataGridViewCellStyle5.SelectionBackColor = SystemColors.Highlight
        DataGridViewCellStyle5.SelectionForeColor = SystemColors.HighlightText
        DataGridViewCellStyle5.WrapMode = DataGridViewTriState.True
        DataGridView1.ColumnHeadersDefaultCellStyle = DataGridViewCellStyle5
        DataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize
        DataGridViewCellStyle6.Alignment = DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle6.BackColor = SystemColors.Window
        DataGridViewCellStyle6.Font = New Font("Segoe UI", 9F)
        DataGridViewCellStyle6.ForeColor = SystemColors.Control
        DataGridViewCellStyle6.SelectionBackColor = SystemColors.Highlight
        DataGridViewCellStyle6.SelectionForeColor = SystemColors.HighlightText
        DataGridViewCellStyle6.WrapMode = DataGridViewTriState.False
        DataGridView1.DefaultCellStyle = DataGridViewCellStyle6
        DataGridView1.Location = New Point(29, 241)
        DataGridView1.Name = "DataGridView1"
        DataGridView1.ReadOnly = True
        DataGridViewCellStyle7.Alignment = DataGridViewContentAlignment.TopCenter
        DataGridViewCellStyle7.BackColor = SystemColors.Control
        DataGridViewCellStyle7.Font = New Font("Segoe UI", 9F)
        DataGridViewCellStyle7.ForeColor = SystemColors.WindowText
        DataGridViewCellStyle7.SelectionBackColor = SystemColors.Highlight
        DataGridViewCellStyle7.SelectionForeColor = SystemColors.HighlightText
        DataGridViewCellStyle7.WrapMode = DataGridViewTriState.True
        DataGridView1.RowHeadersDefaultCellStyle = DataGridViewCellStyle7
        DataGridView1.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing
        DataGridViewCellStyle8.Alignment = DataGridViewContentAlignment.TopLeft
        DataGridViewCellStyle8.Font = New Font("Tahoma", 9F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        DataGridViewCellStyle8.ForeColor = Color.Black
        DataGridView1.RowsDefaultCellStyle = DataGridViewCellStyle8
        DataGridView1.Size = New Size(420, 150)
        DataGridView1.TabIndex = 22
        ' 
        ' Genre
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        ClientSize = New Size(474, 413)
        Controls.Add(DataGridView1)
        Controls.Add(Guna2GradientPanel1)
        Controls.Add(PictureBox1)
        Controls.Add(txtsearch)
        MaximizeBox = False
        MinimizeBox = False
        Name = "Genre"
        ShowIcon = False
        StartPosition = FormStartPosition.CenterScreen
        Text = "Genre"
        Guna2GradientPanel1.ResumeLayout(False)
        Guna2GradientPanel1.PerformLayout()
        CType(PictureBox1, ComponentModel.ISupportInitialize).EndInit()
        CType(DataGridView1, ComponentModel.ISupportInitialize).EndInit()
        ResumeLayout(False)
    End Sub
    Friend WithEvents Guna2GradientPanel1 As Guna.UI2.WinForms.Guna2GradientPanel
    Friend WithEvents Label1 As Label
    Friend WithEvents txtgenre As Guna.UI2.WinForms.Guna2TextBox
    Friend WithEvents btndelete As Guna.UI2.WinForms.Guna2Button
    Friend WithEvents btnedit As Guna.UI2.WinForms.Guna2Button
    Friend WithEvents btnadd As Guna.UI2.WinForms.Guna2Button
    Friend WithEvents PictureBox1 As PictureBox
    Friend WithEvents txtsearch As Guna.UI2.WinForms.Guna2TextBox
    Friend WithEvents Guna2Elipse1 As Guna.UI2.WinForms.Guna2Elipse
    Friend WithEvents DataGridView1 As DataGridView
End Class
