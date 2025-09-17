<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Author
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
        Dim CustomizableEdges9 As Guna.UI2.WinForms.Suite.CustomizableEdges = New Guna.UI2.WinForms.Suite.CustomizableEdges()
        Dim CustomizableEdges10 As Guna.UI2.WinForms.Suite.CustomizableEdges = New Guna.UI2.WinForms.Suite.CustomizableEdges()
        Dim CustomizableEdges1 As Guna.UI2.WinForms.Suite.CustomizableEdges = New Guna.UI2.WinForms.Suite.CustomizableEdges()
        Dim CustomizableEdges2 As Guna.UI2.WinForms.Suite.CustomizableEdges = New Guna.UI2.WinForms.Suite.CustomizableEdges()
        Dim CustomizableEdges3 As Guna.UI2.WinForms.Suite.CustomizableEdges = New Guna.UI2.WinForms.Suite.CustomizableEdges()
        Dim CustomizableEdges4 As Guna.UI2.WinForms.Suite.CustomizableEdges = New Guna.UI2.WinForms.Suite.CustomizableEdges()
        Dim CustomizableEdges5 As Guna.UI2.WinForms.Suite.CustomizableEdges = New Guna.UI2.WinForms.Suite.CustomizableEdges()
        Dim CustomizableEdges6 As Guna.UI2.WinForms.Suite.CustomizableEdges = New Guna.UI2.WinForms.Suite.CustomizableEdges()
        Dim CustomizableEdges7 As Guna.UI2.WinForms.Suite.CustomizableEdges = New Guna.UI2.WinForms.Suite.CustomizableEdges()
        Dim CustomizableEdges8 As Guna.UI2.WinForms.Suite.CustomizableEdges = New Guna.UI2.WinForms.Suite.CustomizableEdges()
        Dim CustomizableEdges11 As Guna.UI2.WinForms.Suite.CustomizableEdges = New Guna.UI2.WinForms.Suite.CustomizableEdges()
        Dim CustomizableEdges12 As Guna.UI2.WinForms.Suite.CustomizableEdges = New Guna.UI2.WinForms.Suite.CustomizableEdges()
        Dim DataGridViewCellStyle1 As DataGridViewCellStyle = New DataGridViewCellStyle()
        Dim DataGridViewCellStyle2 As DataGridViewCellStyle = New DataGridViewCellStyle()
        Dim DataGridViewCellStyle3 As DataGridViewCellStyle = New DataGridViewCellStyle()
        Dim DataGridViewCellStyle4 As DataGridViewCellStyle = New DataGridViewCellStyle()
        Label2 = New Label()
        Guna2GradientPanel1 = New Guna.UI2.WinForms.Guna2GradientPanel()
        Label1 = New Label()
        txtauthor = New Guna.UI2.WinForms.Guna2TextBox()
        btndelete = New Guna.UI2.WinForms.Guna2Button()
        btnedit = New Guna.UI2.WinForms.Guna2Button()
        btnadd = New Guna.UI2.WinForms.Guna2Button()
        Guna2Elipse1 = New Guna.UI2.WinForms.Guna2Elipse(components)
        txtsearch = New Guna.UI2.WinForms.Guna2TextBox()
        PictureBox1 = New PictureBox()
        DataGridView1 = New DataGridView()
        Guna2GradientPanel1.SuspendLayout()
        CType(PictureBox1, ComponentModel.ISupportInitialize).BeginInit()
        CType(DataGridView1, ComponentModel.ISupportInitialize).BeginInit()
        SuspendLayout()
        ' 
        ' Label2
        ' 
        Label2.AutoSize = True
        Label2.Font = New Font("Tahoma", 11.25F, FontStyle.Bold, GraphicsUnit.Point, CByte(0))
        Label2.ForeColor = Color.FromArgb(CByte(207), CByte(58), CByte(109))
        Label2.Location = New Point(25, 47)
        Label2.Name = "Label2"
        Label2.Size = New Size(107, 18)
        Label2.TabIndex = 7
        Label2.Text = "Author name:"
        ' 
        ' Guna2GradientPanel1
        ' 
        Guna2GradientPanel1.BorderColor = Color.FromArgb(CByte(207), CByte(58), CByte(109))
        Guna2GradientPanel1.BorderRadius = 9
        Guna2GradientPanel1.BorderThickness = 2
        Guna2GradientPanel1.Controls.Add(Label1)
        Guna2GradientPanel1.Controls.Add(txtauthor)
        Guna2GradientPanel1.Controls.Add(btndelete)
        Guna2GradientPanel1.Controls.Add(btnedit)
        Guna2GradientPanel1.Controls.Add(btnadd)
        Guna2GradientPanel1.CustomizableEdges = CustomizableEdges9
        Guna2GradientPanel1.Location = New Point(30, 67)
        Guna2GradientPanel1.Name = "Guna2GradientPanel1"
        Guna2GradientPanel1.ShadowDecoration.CustomizableEdges = CustomizableEdges10
        Guna2GradientPanel1.Size = New Size(415, 150)
        Guna2GradientPanel1.TabIndex = 13
        ' 
        ' Label1
        ' 
        Label1.AutoSize = True
        Label1.Font = New Font("Tahoma", 9.75F, FontStyle.Bold, GraphicsUnit.Point, CByte(0))
        Label1.ForeColor = Color.FromArgb(CByte(207), CByte(58), CByte(109))
        Label1.Location = New Point(36, 21)
        Label1.Name = "Label1"
        Label1.Size = New Size(58, 16)
        Label1.TabIndex = 4
        Label1.Text = "Author:"
        ' 
        ' txtauthor
        ' 
        txtauthor.BorderColor = Color.FromArgb(CByte(207), CByte(58), CByte(109))
        txtauthor.BorderRadius = 12
        txtauthor.CustomizableEdges = CustomizableEdges1
        txtauthor.DefaultText = ""
        txtauthor.DisabledState.BorderColor = Color.FromArgb(CByte(208), CByte(208), CByte(208))
        txtauthor.DisabledState.FillColor = Color.FromArgb(CByte(226), CByte(226), CByte(226))
        txtauthor.DisabledState.ForeColor = Color.FromArgb(CByte(138), CByte(138), CByte(138))
        txtauthor.DisabledState.PlaceholderForeColor = Color.FromArgb(CByte(138), CByte(138), CByte(138))
        txtauthor.FocusedState.BorderColor = Color.FromArgb(CByte(94), CByte(148), CByte(255))
        txtauthor.Font = New Font("Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        txtauthor.ForeColor = Color.Black
        txtauthor.HoverState.BorderColor = Color.FromArgb(CByte(94), CByte(148), CByte(255))
        txtauthor.Location = New Point(36, 48)
        txtauthor.Margin = New Padding(3, 5, 3, 5)
        txtauthor.Name = "txtauthor"
        txtauthor.PlaceholderText = ""
        txtauthor.SelectedText = ""
        txtauthor.ShadowDecoration.CustomizableEdges = CustomizableEdges2
        txtauthor.Size = New Size(347, 33)
        txtauthor.TabIndex = 3
        ' 
        ' btndelete
        ' 
        btndelete.BorderColor = Color.FromArgb(CByte(207), CByte(58), CByte(109))
        btndelete.BorderRadius = 9
        btndelete.BorderThickness = 1
        btndelete.CustomizableEdges = CustomizableEdges3
        btndelete.DisabledState.BorderColor = Color.DarkGray
        btndelete.DisabledState.CustomBorderColor = Color.DarkGray
        btndelete.DisabledState.FillColor = Color.FromArgb(CByte(169), CByte(169), CByte(169))
        btndelete.DisabledState.ForeColor = Color.FromArgb(CByte(141), CByte(141), CByte(141))
        btndelete.FillColor = Color.Empty
        btndelete.Font = New Font("Tahoma", 11.25F, FontStyle.Bold)
        btndelete.ForeColor = Color.FromArgb(CByte(207), CByte(58), CByte(109))
        btndelete.Location = New Point(290, 98)
        btndelete.Name = "btndelete"
        btndelete.ShadowDecoration.CustomizableEdges = CustomizableEdges4
        btndelete.Size = New Size(90, 27)
        btndelete.TabIndex = 2
        btndelete.Text = "DELETE"
        ' 
        ' btnedit
        ' 
        btnedit.BorderColor = Color.FromArgb(CByte(207), CByte(58), CByte(109))
        btnedit.BorderRadius = 9
        btnedit.BorderThickness = 1
        btnedit.CustomizableEdges = CustomizableEdges5
        btnedit.DisabledState.BorderColor = Color.DarkGray
        btnedit.DisabledState.CustomBorderColor = Color.DarkGray
        btnedit.DisabledState.FillColor = Color.FromArgb(CByte(169), CByte(169), CByte(169))
        btnedit.DisabledState.ForeColor = Color.FromArgb(CByte(141), CByte(141), CByte(141))
        btnedit.FillColor = Color.Empty
        btnedit.Font = New Font("Tahoma", 11.25F, FontStyle.Bold)
        btnedit.ForeColor = Color.FromArgb(CByte(207), CByte(58), CByte(109))
        btnedit.Location = New Point(160, 98)
        btnedit.Name = "btnedit"
        btnedit.ShadowDecoration.CustomizableEdges = CustomizableEdges6
        btnedit.Size = New Size(90, 27)
        btnedit.TabIndex = 1
        btnedit.Text = "EDIT"
        ' 
        ' btnadd
        ' 
        btnadd.BorderColor = Color.FromArgb(CByte(207), CByte(58), CByte(109))
        btnadd.BorderRadius = 9
        btnadd.BorderThickness = 1
        btnadd.CustomizableEdges = CustomizableEdges7
        btnadd.DisabledState.BorderColor = Color.DarkGray
        btnadd.DisabledState.CustomBorderColor = Color.DarkGray
        btnadd.DisabledState.FillColor = Color.FromArgb(CByte(169), CByte(169), CByte(169))
        btnadd.DisabledState.ForeColor = Color.FromArgb(CByte(141), CByte(141), CByte(141))
        btnadd.FillColor = Color.Empty
        btnadd.Font = New Font("Tahoma", 11.25F, FontStyle.Bold)
        btnadd.ForeColor = Color.FromArgb(CByte(207), CByte(58), CByte(109))
        btnadd.Location = New Point(36, 98)
        btnadd.Name = "btnadd"
        btnadd.ShadowDecoration.CustomizableEdges = CustomizableEdges8
        btnadd.Size = New Size(90, 27)
        btnadd.TabIndex = 0
        btnadd.Text = "ADD"
        ' 
        ' Guna2Elipse1
        ' 
        Guna2Elipse1.BorderRadius = 9
        ' 
        ' txtsearch
        ' 
        txtsearch.BorderColor = Color.FromArgb(CByte(207), CByte(58), CByte(109))
        txtsearch.BorderRadius = 16
        txtsearch.CustomizableEdges = CustomizableEdges11
        txtsearch.DefaultText = ""
        txtsearch.DisabledState.BorderColor = Color.FromArgb(CByte(208), CByte(208), CByte(208))
        txtsearch.DisabledState.FillColor = Color.FromArgb(CByte(226), CByte(226), CByte(226))
        txtsearch.DisabledState.ForeColor = Color.FromArgb(CByte(138), CByte(138), CByte(138))
        txtsearch.DisabledState.PlaceholderForeColor = Color.FromArgb(CByte(138), CByte(138), CByte(138))
        txtsearch.FocusedState.BorderColor = Color.FromArgb(CByte(94), CByte(148), CByte(255))
        txtsearch.Font = New Font("Baskerville Old Face", 12F, FontStyle.Bold)
        txtsearch.ForeColor = Color.Black
        txtsearch.HoverState.BorderColor = Color.FromArgb(CByte(94), CByte(148), CByte(255))
        txtsearch.Location = New Point(30, 13)
        txtsearch.Margin = New Padding(4)
        txtsearch.Name = "txtsearch"
        txtsearch.PlaceholderText = "Search Author"
        txtsearch.SelectedText = ""
        txtsearch.ShadowDecoration.CustomizableEdges = CustomizableEdges12
        txtsearch.Size = New Size(420, 30)
        txtsearch.TabIndex = 24
        ' 
        ' PictureBox1
        ' 
        PictureBox1.BackColor = Color.White
        PictureBox1.BackgroundImage = My.Resources.Resources.magnifier
        PictureBox1.BackgroundImageLayout = ImageLayout.Stretch
        PictureBox1.Location = New Point(422, 20)
        PictureBox1.Name = "PictureBox1"
        PictureBox1.Size = New Size(20, 18)
        PictureBox1.TabIndex = 25
        PictureBox1.TabStop = False
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
        DataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.TopCenter
        DataGridViewCellStyle2.BackColor = SystemColors.Window
        DataGridViewCellStyle2.Font = New Font("Segoe UI", 9F)
        DataGridViewCellStyle2.ForeColor = Color.White
        DataGridViewCellStyle2.SelectionBackColor = SystemColors.Highlight
        DataGridViewCellStyle2.SelectionForeColor = SystemColors.HighlightText
        DataGridViewCellStyle2.WrapMode = DataGridViewTriState.False
        DataGridView1.DefaultCellStyle = DataGridViewCellStyle2
        DataGridView1.Location = New Point(25, 248)
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
        ' Author
        ' 
        AutoScaleDimensions = New SizeF(7F, 14F)
        AutoScaleMode = AutoScaleMode.Font
        ClientSize = New Size(474, 413)
        Controls.Add(DataGridView1)
        Controls.Add(PictureBox1)
        Controls.Add(txtsearch)
        Controls.Add(Guna2GradientPanel1)
        Font = New Font("Tahoma", 9F, FontStyle.Bold, GraphicsUnit.Point, CByte(0))
        ForeColor = Color.White
        FormBorderStyle = FormBorderStyle.FixedDialog
        KeyPreview = True
        MaximizeBox = False
        MinimizeBox = False
        Name = "Author"
        ShowIcon = False
        StartPosition = FormStartPosition.CenterScreen
        Text = "Author Maintenance"
        Guna2GradientPanel1.ResumeLayout(False)
        Guna2GradientPanel1.PerformLayout()
        CType(PictureBox1, ComponentModel.ISupportInitialize).EndInit()
        CType(DataGridView1, ComponentModel.ISupportInitialize).EndInit()
        ResumeLayout(False)
    End Sub
    Friend WithEvents Guna2Elipse2 As Guna.UI2.WinForms.Guna2Elipse
    Friend WithEvents Guna2Panel1 As Guna.UI2.WinForms.Guna2Panel
    Friend WithEvents btnadd As Guna.UI2.WinForms.Guna2Button
    Friend WithEvents Label1 As Label
    Friend WithEvents btnedit As Guna.UI2.WinForms.Guna2Button
    Friend WithEvents btndelete As Guna.UI2.WinForms.Guna2Button
    Friend WithEvents Label2 As Label
    Friend WithEvents txtauthor As Guna.UI2.WinForms.Guna2TextBox
    Friend WithEvents txtsearch As Guna.UI2.WinForms.Guna2TextBox
    Friend WithEvents Guna2GradientPanel1 As Guna.UI2.WinForms.Guna2GradientPanel
    Friend WithEvents Guna2Elipse1 As Guna.UI2.WinForms.Guna2Elipse
    Friend WithEvents PictureBox1 As PictureBox
    Friend WithEvents DataGridView1 As DataGridView

End Class
