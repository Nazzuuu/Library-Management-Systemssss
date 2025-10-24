<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class AdminBorower
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
        Dim CustomizableEdges2 As Guna.UI2.WinForms.Suite.CustomizableEdges = New Guna.UI2.WinForms.Suite.CustomizableEdges()
        Dim CustomizableEdges3 As Guna.UI2.WinForms.Suite.CustomizableEdges = New Guna.UI2.WinForms.Suite.CustomizableEdges()
        Dim CustomizableEdges4 As Guna.UI2.WinForms.Suite.CustomizableEdges = New Guna.UI2.WinForms.Suite.CustomizableEdges()
        Dim CustomizableEdges5 As Guna.UI2.WinForms.Suite.CustomizableEdges = New Guna.UI2.WinForms.Suite.CustomizableEdges()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(AdminBorower))
        Dim CustomizableEdges1 As Guna.UI2.WinForms.Suite.CustomizableEdges = New Guna.UI2.WinForms.Suite.CustomizableEdges()
        Guna2Elipse1 = New Guna.UI2.WinForms.Guna2Elipse(components)
        Guna2Elipse2 = New Guna.UI2.WinForms.Guna2Elipse(components)
        Guna2Button2 = New Guna.UI2.WinForms.Guna2Button()
        Guna2Elipse3 = New Guna.UI2.WinForms.Guna2Elipse(components)
        Guna2Button1 = New Guna.UI2.WinForms.Guna2Button()
        Panel1 = New Panel()
        Guna2CirclePictureBox1 = New Guna.UI2.WinForms.Guna2CirclePictureBox()
        Label3 = New Label()
        Guna2Elipse4 = New Guna.UI2.WinForms.Guna2Elipse(components)
        Guna2Elipse5 = New Guna.UI2.WinForms.Guna2Elipse(components)
        LinkLabel1 = New LinkLabel()
        Panel1.SuspendLayout()
        CType(Guna2CirclePictureBox1, ComponentModel.ISupportInitialize).BeginInit()
        SuspendLayout()
        ' 
        ' Guna2Elipse1
        ' 
        Guna2Elipse1.BorderRadius = 9
        Guna2Elipse1.TargetControl = Me
        ' 
        ' Guna2Elipse2
        ' 
        Guna2Elipse2.BorderRadius = 9
        Guna2Elipse2.TargetControl = Guna2Button2
        ' 
        ' Guna2Button2
        ' 
        Guna2Button2.CustomizableEdges = CustomizableEdges2
        Guna2Button2.DisabledState.BorderColor = Color.DarkGray
        Guna2Button2.DisabledState.CustomBorderColor = Color.DarkGray
        Guna2Button2.DisabledState.FillColor = Color.FromArgb(CByte(169), CByte(169), CByte(169))
        Guna2Button2.DisabledState.ForeColor = Color.FromArgb(CByte(141), CByte(141), CByte(141))
        Guna2Button2.FillColor = Color.FromArgb(CByte(207), CByte(58), CByte(109))
        Guna2Button2.Font = New Font("Baskerville Old Face", 9.75F, FontStyle.Bold)
        Guna2Button2.ForeColor = Color.White
        Guna2Button2.Location = New Point(232, 124)
        Guna2Button2.Name = "Guna2Button2"
        Guna2Button2.ShadowDecoration.CustomizableEdges = CustomizableEdges3
        Guna2Button2.Size = New Size(164, 108)
        Guna2Button2.TabIndex = 11
        Guna2Button2.Text = "BORROWERS"
        ' 
        ' Guna2Elipse3
        ' 
        Guna2Elipse3.BorderRadius = 9
        Guna2Elipse3.TargetControl = Guna2Button1
        ' 
        ' Guna2Button1
        ' 
        Guna2Button1.CustomizableEdges = CustomizableEdges4
        Guna2Button1.DisabledState.BorderColor = Color.DarkGray
        Guna2Button1.DisabledState.CustomBorderColor = Color.DarkGray
        Guna2Button1.DisabledState.FillColor = Color.FromArgb(CByte(169), CByte(169), CByte(169))
        Guna2Button1.DisabledState.ForeColor = Color.FromArgb(CByte(141), CByte(141), CByte(141))
        Guna2Button1.FillColor = Color.FromArgb(CByte(207), CByte(58), CByte(109))
        Guna2Button1.Font = New Font("Baskerville Old Face", 9.75F, FontStyle.Bold)
        Guna2Button1.ForeColor = Color.White
        Guna2Button1.Location = New Point(33, 124)
        Guna2Button1.Name = "Guna2Button1"
        Guna2Button1.ShadowDecoration.CustomizableEdges = CustomizableEdges5
        Guna2Button1.Size = New Size(164, 108)
        Guna2Button1.TabIndex = 10
        Guna2Button1.Text = "LIBRARIAN"
        ' 
        ' Panel1
        ' 
        Panel1.BackColor = Color.FromArgb(CByte(207), CByte(58), CByte(109))
        Panel1.Controls.Add(Guna2CirclePictureBox1)
        Panel1.Controls.Add(Label3)
        Panel1.Dock = DockStyle.Top
        Panel1.Location = New Point(0, 0)
        Panel1.Name = "Panel1"
        Panel1.Size = New Size(429, 62)
        Panel1.TabIndex = 9
        ' 
        ' Guna2CirclePictureBox1
        ' 
        Guna2CirclePictureBox1.BackgroundImageLayout = ImageLayout.Stretch
        Guna2CirclePictureBox1.Image = CType(resources.GetObject("Guna2CirclePictureBox1.Image"), Image)
        Guna2CirclePictureBox1.ImageRotate = 0F
        Guna2CirclePictureBox1.Location = New Point(17, 4)
        Guna2CirclePictureBox1.Name = "Guna2CirclePictureBox1"
        Guna2CirclePictureBox1.ShadowDecoration.CustomizableEdges = CustomizableEdges1
        Guna2CirclePictureBox1.ShadowDecoration.Mode = Guna.UI2.WinForms.Enums.ShadowMode.Circle
        Guna2CirclePictureBox1.Size = New Size(82, 53)
        Guna2CirclePictureBox1.SizeMode = PictureBoxSizeMode.StretchImage
        Guna2CirclePictureBox1.TabIndex = 9
        Guna2CirclePictureBox1.TabStop = False
        ' 
        ' Label3
        ' 
        Label3.Anchor = AnchorStyles.Top
        Label3.AutoSize = True
        Label3.Font = New Font("Baskerville Old Face", 9.75F, FontStyle.Bold, GraphicsUnit.Point, CByte(0))
        Label3.ForeColor = Color.White
        Label3.Location = New Point(127, 9)
        Label3.Name = "Label3"
        Label3.Size = New Size(212, 56)
        Label3.TabIndex = 1
        Label3.Text = "Monlimar Development Academy" & vbCrLf & "    Library Management System" & vbCrLf & "                 (MDA-LMS)" & vbCrLf & "                                        "
        ' 
        ' Guna2Elipse4
        ' 
        Guna2Elipse4.BorderRadius = 9
        ' 
        ' Guna2Elipse5
        ' 
        Guna2Elipse5.BorderRadius = 9
        ' 
        ' LinkLabel1
        ' 
        LinkLabel1.ActiveLinkColor = Color.RosyBrown
        LinkLabel1.AutoSize = True
        LinkLabel1.Font = New Font("Baskerville Old Face", 11.25F, FontStyle.Italic, GraphicsUnit.Point, CByte(0))
        LinkLabel1.LinkColor = Color.HotPink
        LinkLabel1.Location = New Point(12, 76)
        LinkLabel1.Name = "LinkLabel1"
        LinkLabel1.Size = New Size(133, 17)
        LinkLabel1.TabIndex = 12
        LinkLabel1.TabStop = True
        LinkLabel1.Text = "Connect to Database"
        ' 
        ' AdminBorower
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        ClientSize = New Size(429, 293)
        Controls.Add(LinkLabel1)
        Controls.Add(Panel1)
        Controls.Add(Guna2Button2)
        Controls.Add(Guna2Button1)
        FormBorderStyle = FormBorderStyle.None
        Name = "AdminBorower"
        StartPosition = FormStartPosition.CenterScreen
        Text = "AdminBorower"
        Panel1.ResumeLayout(False)
        Panel1.PerformLayout()
        CType(Guna2CirclePictureBox1, ComponentModel.ISupportInitialize).EndInit()
        ResumeLayout(False)
        PerformLayout()
    End Sub

    Friend WithEvents Guna2Elipse1 As Guna.UI2.WinForms.Guna2Elipse
    Friend WithEvents Panel1 As Panel
    Friend WithEvents Guna2CirclePictureBox1 As Guna.UI2.WinForms.Guna2CirclePictureBox
    Friend WithEvents Label3 As Label
    Friend WithEvents Guna2Button2 As Guna.UI2.WinForms.Guna2Button
    Friend WithEvents Guna2Button1 As Guna.UI2.WinForms.Guna2Button
    Friend WithEvents Guna2Elipse2 As Guna.UI2.WinForms.Guna2Elipse
    Friend WithEvents Guna2Elipse3 As Guna.UI2.WinForms.Guna2Elipse
    Friend WithEvents Guna2Elipse4 As Guna.UI2.WinForms.Guna2Elipse
    Friend WithEvents Guna2Elipse5 As Guna.UI2.WinForms.Guna2Elipse
    Friend WithEvents LinkLabel1 As LinkLabel
End Class
