<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class ReserveCopies
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
        Dim CustomizableEdges1 As Guna.UI2.WinForms.Suite.CustomizableEdges = New Guna.UI2.WinForms.Suite.CustomizableEdges()
        Dim CustomizableEdges2 As Guna.UI2.WinForms.Suite.CustomizableEdges = New Guna.UI2.WinForms.Suite.CustomizableEdges()
        Dim DataGridViewCellStyle1 As DataGridViewCellStyle = New DataGridViewCellStyle()
        Dim DataGridViewCellStyle2 As DataGridViewCellStyle = New DataGridViewCellStyle()
        Dim DataGridViewCellStyle3 As DataGridViewCellStyle = New DataGridViewCellStyle()
        Dim DataGridViewCellStyle4 As DataGridViewCellStyle = New DataGridViewCellStyle()
        Dim CustomizableEdges3 As Guna.UI2.WinForms.Suite.CustomizableEdges = New Guna.UI2.WinForms.Suite.CustomizableEdges()
        Dim CustomizableEdges4 As Guna.UI2.WinForms.Suite.CustomizableEdges = New Guna.UI2.WinForms.Suite.CustomizableEdges()
        Dim CustomizableEdges5 As Guna.UI2.WinForms.Suite.CustomizableEdges = New Guna.UI2.WinForms.Suite.CustomizableEdges()
        Dim CustomizableEdges6 As Guna.UI2.WinForms.Suite.CustomizableEdges = New Guna.UI2.WinForms.Suite.CustomizableEdges()
        Guna2GradientPanel1 = New Guna.UI2.WinForms.Guna2GradientPanel()
        DataGridView1 = New DataGridView()
        txtsearch = New Guna.UI2.WinForms.Guna2TextBox()
        PictureBox1 = New PictureBox()
        btnpush = New Guna.UI2.WinForms.Guna2Button()
        Guna2Elipse1 = New Guna.UI2.WinForms.Guna2Elipse(components)
        lblreserve = New Label()
        Label2 = New Label()
        Guna2GradientPanel1.SuspendLayout()
        CType(DataGridView1, ComponentModel.ISupportInitialize).BeginInit()
        CType(PictureBox1, ComponentModel.ISupportInitialize).BeginInit()
        SuspendLayout()
        ' 
        ' Guna2GradientPanel1
        ' 
        Guna2GradientPanel1.BorderColor = Color.FromArgb(CByte(207), CByte(58), CByte(109))
        Guna2GradientPanel1.BorderRadius = 9
        Guna2GradientPanel1.BorderThickness = 2
        Guna2GradientPanel1.Controls.Add(DataGridView1)
        Guna2GradientPanel1.CustomizableEdges = CustomizableEdges1
        Guna2GradientPanel1.Location = New Point(42, 81)
        Guna2GradientPanel1.Name = "Guna2GradientPanel1"
        Guna2GradientPanel1.ShadowDecoration.CustomizableEdges = CustomizableEdges2
        Guna2GradientPanel1.Size = New Size(1044, 300)
        Guna2GradientPanel1.TabIndex = 47
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
        DataGridView1.Location = New Point(24, 22)
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
        DataGridView1.Size = New Size(985, 255)
        DataGridView1.TabIndex = 24
        ' 
        ' txtsearch
        ' 
        txtsearch.BorderColor = Color.FromArgb(CByte(207), CByte(58), CByte(109))
        txtsearch.BorderRadius = 16
        txtsearch.CustomizableEdges = CustomizableEdges3
        txtsearch.DefaultText = ""
        txtsearch.DisabledState.BorderColor = Color.FromArgb(CByte(208), CByte(208), CByte(208))
        txtsearch.DisabledState.FillColor = Color.FromArgb(CByte(226), CByte(226), CByte(226))
        txtsearch.DisabledState.ForeColor = Color.FromArgb(CByte(138), CByte(138), CByte(138))
        txtsearch.DisabledState.PlaceholderForeColor = Color.FromArgb(CByte(138), CByte(138), CByte(138))
        txtsearch.FocusedState.BorderColor = Color.FromArgb(CByte(94), CByte(148), CByte(255))
        txtsearch.Font = New Font("Baskerville Old Face", 12F, FontStyle.Bold)
        txtsearch.ForeColor = Color.Black
        txtsearch.HoverState.BorderColor = Color.FromArgb(CByte(94), CByte(148), CByte(255))
        txtsearch.Location = New Point(42, 22)
        txtsearch.Margin = New Padding(4)
        txtsearch.Name = "txtsearch"
        txtsearch.PlaceholderText = "Search Book Title"
        txtsearch.SelectedText = ""
        txtsearch.ShadowDecoration.CustomizableEdges = CustomizableEdges4
        txtsearch.Size = New Size(889, 30)
        txtsearch.TabIndex = 49
        ' 
        ' PictureBox1
        ' 
        PictureBox1.BackColor = Color.White
        PictureBox1.BackgroundImage = My.Resources.Resources.magnifier
        PictureBox1.BackgroundImageLayout = ImageLayout.Stretch
        PictureBox1.Location = New Point(899, 27)
        PictureBox1.Name = "PictureBox1"
        PictureBox1.Size = New Size(20, 18)
        PictureBox1.TabIndex = 50
        PictureBox1.TabStop = False
        ' 
        ' btnpush
        ' 
        btnpush.BorderColor = Color.FromArgb(CByte(207), CByte(58), CByte(109))
        btnpush.BorderRadius = 9
        btnpush.BorderThickness = 1
        btnpush.CustomizableEdges = CustomizableEdges5
        btnpush.DisabledState.BorderColor = Color.DarkGray
        btnpush.DisabledState.CustomBorderColor = Color.DarkGray
        btnpush.DisabledState.FillColor = Color.FromArgb(CByte(169), CByte(169), CByte(169))
        btnpush.DisabledState.ForeColor = Color.FromArgb(CByte(141), CByte(141), CByte(141))
        btnpush.FillColor = Color.Empty
        btnpush.Font = New Font("Tahoma", 11.25F, FontStyle.Bold)
        btnpush.ForeColor = Color.FromArgb(CByte(207), CByte(58), CByte(109))
        btnpush.Location = New Point(429, 401)
        btnpush.Name = "btnpush"
        btnpush.ShadowDecoration.CustomizableEdges = CustomizableEdges6
        btnpush.Size = New Size(222, 27)
        btnpush.TabIndex = 51
        btnpush.Text = "PUSH TO ACCESSION"
        ' 
        ' Guna2Elipse1
        ' 
        Guna2Elipse1.BorderRadius = 9
        Guna2Elipse1.TargetControl = DataGridView1
        ' 
        ' lblreserve
        ' 
        lblreserve.AutoSize = True
        lblreserve.Font = New Font("Tahoma", 15.75F, FontStyle.Bold, GraphicsUnit.Point, CByte(0))
        lblreserve.ForeColor = Color.Red
        lblreserve.Location = New Point(168, 408)
        lblreserve.Name = "lblreserve"
        lblreserve.Size = New Size(25, 25)
        lblreserve.TabIndex = 62
        lblreserve.Text = "0"
        ' 
        ' Label2
        ' 
        Label2.AutoSize = True
        Label2.Font = New Font("Tahoma", 11.25F, FontStyle.Bold, GraphicsUnit.Point, CByte(0))
        Label2.ForeColor = Color.Black
        Label2.Location = New Point(42, 410)
        Label2.Name = "Label2"
        Label2.Size = New Size(129, 18)
        Label2.TabIndex = 61
        Label2.Text = "Reserve Copies:"
        ' 
        ' ReserveCopies
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        ClientSize = New Size(1120, 454)
        Controls.Add(lblreserve)
        Controls.Add(Label2)
        Controls.Add(btnpush)
        Controls.Add(PictureBox1)
        Controls.Add(txtsearch)
        Controls.Add(Guna2GradientPanel1)
        KeyPreview = True
        MaximizeBox = False
        MinimizeBox = False
        Name = "ReserveCopies"
        ShowIcon = False
        StartPosition = FormStartPosition.CenterScreen
        Text = "ReserveCopies"
        Guna2GradientPanel1.ResumeLayout(False)
        CType(DataGridView1, ComponentModel.ISupportInitialize).EndInit()
        CType(PictureBox1, ComponentModel.ISupportInitialize).EndInit()
        ResumeLayout(False)
        PerformLayout()
    End Sub

    Friend WithEvents Guna2GradientPanel1 As Guna.UI2.WinForms.Guna2GradientPanel
    Friend WithEvents DataGridView1 As DataGridView
    Friend WithEvents txtsearch As Guna.UI2.WinForms.Guna2TextBox
    Friend WithEvents PictureBox1 As PictureBox
    Friend WithEvents btnpush As Guna.UI2.WinForms.Guna2Button
    Friend WithEvents Guna2Elipse1 As Guna.UI2.WinForms.Guna2Elipse
    Friend WithEvents lblreserve As Label
    Friend WithEvents Label2 As Label
End Class
