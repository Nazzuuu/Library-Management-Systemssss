<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class login
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
        Dim CustomizableEdges3 As Guna.UI2.WinForms.Suite.CustomizableEdges = New Guna.UI2.WinForms.Suite.CustomizableEdges()
        Dim CustomizableEdges4 As Guna.UI2.WinForms.Suite.CustomizableEdges = New Guna.UI2.WinForms.Suite.CustomizableEdges()
        Dim CustomizableEdges5 As Guna.UI2.WinForms.Suite.CustomizableEdges = New Guna.UI2.WinForms.Suite.CustomizableEdges()
        Dim CustomizableEdges6 As Guna.UI2.WinForms.Suite.CustomizableEdges = New Guna.UI2.WinForms.Suite.CustomizableEdges()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(login))
        Dim CustomizableEdges7 As Guna.UI2.WinForms.Suite.CustomizableEdges = New Guna.UI2.WinForms.Suite.CustomizableEdges()
        Dim CustomizableEdges8 As Guna.UI2.WinForms.Suite.CustomizableEdges = New Guna.UI2.WinForms.Suite.CustomizableEdges()
        Dim CustomizableEdges9 As Guna.UI2.WinForms.Suite.CustomizableEdges = New Guna.UI2.WinForms.Suite.CustomizableEdges()
        txtuser = New Guna.UI2.WinForms.Guna2TextBox()
        txtpass = New Guna.UI2.WinForms.Guna2TextBox()
        Label1 = New Label()
        Label2 = New Label()
        btnlogin = New Guna.UI2.WinForms.Guna2Button()
        Panel1 = New Panel()
        Guna2CirclePictureBox1 = New Guna.UI2.WinForms.Guna2CirclePictureBox()
        Guna2ControlBox1 = New Guna.UI2.WinForms.Guna2ControlBox()
        Label3 = New Label()
        Panel_btnlogin = New Panel()
        Guna2Elipse1 = New Guna.UI2.WinForms.Guna2Elipse(components)
        PictureBox1 = New PictureBox()
        Panel1.SuspendLayout()
        CType(Guna2CirclePictureBox1, ComponentModel.ISupportInitialize).BeginInit()
        Panel_btnlogin.SuspendLayout()
        CType(PictureBox1, ComponentModel.ISupportInitialize).BeginInit()
        SuspendLayout()
        ' 
        ' txtuser
        ' 
        txtuser.BorderColor = Color.FromArgb(CByte(207), CByte(58), CByte(109))
        txtuser.BorderRadius = 14
        txtuser.CustomizableEdges = CustomizableEdges1
        txtuser.DefaultText = ""
        txtuser.DisabledState.BorderColor = Color.FromArgb(CByte(208), CByte(208), CByte(208))
        txtuser.DisabledState.FillColor = Color.FromArgb(CByte(226), CByte(226), CByte(226))
        txtuser.DisabledState.ForeColor = Color.FromArgb(CByte(138), CByte(138), CByte(138))
        txtuser.DisabledState.PlaceholderForeColor = Color.FromArgb(CByte(138), CByte(138), CByte(138))
        txtuser.FocusedState.BorderColor = Color.FromArgb(CByte(94), CByte(148), CByte(255))
        txtuser.Font = New Font("Calibri", 9.75F, FontStyle.Bold Or FontStyle.Italic)
        txtuser.ForeColor = Color.Black
        txtuser.HoverState.BorderColor = Color.FromArgb(CByte(94), CByte(148), CByte(255))
        txtuser.Location = New Point(152, 110)
        txtuser.Margin = New Padding(5)
        txtuser.Name = "txtuser"
        txtuser.PlaceholderText = ""
        txtuser.SelectedText = ""
        txtuser.ShadowDecoration.CustomizableEdges = CustomizableEdges2
        txtuser.Size = New Size(238, 28)
        txtuser.TabIndex = 0
        ' 
        ' txtpass
        ' 
        txtpass.BorderColor = Color.FromArgb(CByte(207), CByte(58), CByte(109))
        txtpass.BorderRadius = 14
        txtpass.CustomizableEdges = CustomizableEdges3
        txtpass.DefaultText = ""
        txtpass.DisabledState.BorderColor = Color.FromArgb(CByte(208), CByte(208), CByte(208))
        txtpass.DisabledState.FillColor = Color.FromArgb(CByte(226), CByte(226), CByte(226))
        txtpass.DisabledState.ForeColor = Color.FromArgb(CByte(138), CByte(138), CByte(138))
        txtpass.DisabledState.PlaceholderForeColor = Color.FromArgb(CByte(138), CByte(138), CByte(138))
        txtpass.FocusedState.BorderColor = Color.FromArgb(CByte(94), CByte(148), CByte(255))
        txtpass.Font = New Font("Calibri", 9.75F, FontStyle.Bold Or FontStyle.Italic)
        txtpass.ForeColor = Color.Black
        txtpass.HoverState.BorderColor = Color.FromArgb(CByte(94), CByte(148), CByte(255))
        txtpass.Location = New Point(152, 154)
        txtpass.Margin = New Padding(5)
        txtpass.Name = "txtpass"
        txtpass.PlaceholderText = ""
        txtpass.SelectedText = ""
        txtpass.ShadowDecoration.CustomizableEdges = CustomizableEdges4
        txtpass.Size = New Size(238, 28)
        txtpass.TabIndex = 1
        ' 
        ' Label1
        ' 
        Label1.AutoSize = True
        Label1.Font = New Font("Baskerville Old Face", 12F, FontStyle.Bold)
        Label1.ForeColor = Color.FromArgb(CByte(207), CByte(58), CByte(109))
        Label1.Location = New Point(65, 112)
        Label1.Name = "Label1"
        Label1.Size = New Size(87, 18)
        Label1.TabIndex = 2
        Label1.Text = "Username:"
        ' 
        ' Label2
        ' 
        Label2.AutoSize = True
        Label2.Font = New Font("Baskerville Old Face", 12F, FontStyle.Bold)
        Label2.ForeColor = Color.FromArgb(CByte(207), CByte(58), CByte(109))
        Label2.Location = New Point(65, 159)
        Label2.Name = "Label2"
        Label2.Size = New Size(81, 18)
        Label2.TabIndex = 3
        Label2.Text = "Password:"
        ' 
        ' btnlogin
        ' 
        btnlogin.BorderRadius = 15
        btnlogin.CustomizableEdges = CustomizableEdges5
        btnlogin.DisabledState.BorderColor = Color.DarkGray
        btnlogin.DisabledState.CustomBorderColor = Color.DarkGray
        btnlogin.DisabledState.FillColor = Color.FromArgb(CByte(169), CByte(169), CByte(169))
        btnlogin.DisabledState.ForeColor = Color.FromArgb(CByte(141), CByte(141), CByte(141))
        btnlogin.FillColor = Color.FromArgb(CByte(207), CByte(58), CByte(109))
        btnlogin.Font = New Font("Baskerville Old Face", 12F, FontStyle.Bold)
        btnlogin.ForeColor = Color.White
        btnlogin.Location = New Point(21, 12)
        btnlogin.Name = "btnlogin"
        btnlogin.ShadowDecoration.CustomizableEdges = CustomizableEdges6
        btnlogin.Size = New Size(155, 33)
        btnlogin.TabIndex = 4
        btnlogin.Text = "login"
        ' 
        ' Panel1
        ' 
        Panel1.BackColor = Color.FromArgb(CByte(207), CByte(58), CByte(109))
        Panel1.Controls.Add(Guna2CirclePictureBox1)
        Panel1.Controls.Add(Guna2ControlBox1)
        Panel1.Controls.Add(Label3)
        Panel1.Dock = DockStyle.Top
        Panel1.Location = New Point(0, 0)
        Panel1.Name = "Panel1"
        Panel1.Size = New Size(482, 62)
        Panel1.TabIndex = 5
        ' 
        ' Guna2CirclePictureBox1
        ' 
        Guna2CirclePictureBox1.BackgroundImageLayout = ImageLayout.Stretch
        Guna2CirclePictureBox1.Image = CType(resources.GetObject("Guna2CirclePictureBox1.Image"), Image)
        Guna2CirclePictureBox1.ImageRotate = 0F
        Guna2CirclePictureBox1.Location = New Point(17, 4)
        Guna2CirclePictureBox1.Name = "Guna2CirclePictureBox1"
        Guna2CirclePictureBox1.ShadowDecoration.CustomizableEdges = CustomizableEdges7
        Guna2CirclePictureBox1.ShadowDecoration.Mode = Guna.UI2.WinForms.Enums.ShadowMode.Circle
        Guna2CirclePictureBox1.Size = New Size(82, 53)
        Guna2CirclePictureBox1.SizeMode = PictureBoxSizeMode.StretchImage
        Guna2CirclePictureBox1.TabIndex = 9
        Guna2CirclePictureBox1.TabStop = False
        ' 
        ' Guna2ControlBox1
        ' 
        Guna2ControlBox1.Anchor = AnchorStyles.Top Or AnchorStyles.Right
        Guna2ControlBox1.CustomizableEdges = CustomizableEdges8
        Guna2ControlBox1.FillColor = Color.Transparent
        Guna2ControlBox1.IconColor = Color.White
        Guna2ControlBox1.Location = New Point(444, 2)
        Guna2ControlBox1.Name = "Guna2ControlBox1"
        Guna2ControlBox1.ShadowDecoration.CustomizableEdges = CustomizableEdges9
        Guna2ControlBox1.Size = New Size(36, 30)
        Guna2ControlBox1.TabIndex = 2
        ' 
        ' Label3
        ' 
        Label3.Anchor = AnchorStyles.Top
        Label3.AutoSize = True
        Label3.Font = New Font("Baskerville Old Face", 9.75F, FontStyle.Bold, GraphicsUnit.Point, CByte(0))
        Label3.ForeColor = Color.White
        Label3.Location = New Point(139, 12)
        Label3.Name = "Label3"
        Label3.Size = New Size(212, 56)
        Label3.TabIndex = 1
        Label3.Text = "Monlimar Development Academy" & vbCrLf & "    Library Management System" & vbCrLf & "                 (MDA-LMS)" & vbCrLf & "                                        "
        ' 
        ' Panel_btnlogin
        ' 
        Panel_btnlogin.Controls.Add(btnlogin)
        Panel_btnlogin.Location = New Point(162, 197)
        Panel_btnlogin.Name = "Panel_btnlogin"
        Panel_btnlogin.Size = New Size(191, 57)
        Panel_btnlogin.TabIndex = 7
        ' 
        ' Guna2Elipse1
        ' 
        Guna2Elipse1.BorderRadius = 9
        Guna2Elipse1.TargetControl = Me
        ' 
        ' PictureBox1
        ' 
        PictureBox1.BackColor = Color.White
        PictureBox1.BackgroundImageLayout = ImageLayout.Zoom
        PictureBox1.Location = New Point(354, 159)
        PictureBox1.Name = "PictureBox1"
        PictureBox1.Size = New Size(28, 18)
        PictureBox1.SizeMode = PictureBoxSizeMode.Zoom
        PictureBox1.TabIndex = 8
        PictureBox1.TabStop = False
        ' 
        ' login
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        BackColor = Color.White
        ClientSize = New Size(482, 276)
        ControlBox = False
        Controls.Add(PictureBox1)
        Controls.Add(Panel_btnlogin)
        Controls.Add(Panel1)
        Controls.Add(Label2)
        Controls.Add(Label1)
        Controls.Add(txtpass)
        Controls.Add(txtuser)
        FormBorderStyle = FormBorderStyle.None
        MaximizeBox = False
        MinimizeBox = False
        Name = "login"
        ShowIcon = False
        StartPosition = FormStartPosition.CenterScreen
        Panel1.ResumeLayout(False)
        Panel1.PerformLayout()
        CType(Guna2CirclePictureBox1, ComponentModel.ISupportInitialize).EndInit()
        Panel_btnlogin.ResumeLayout(False)
        CType(PictureBox1, ComponentModel.ISupportInitialize).EndInit()
        ResumeLayout(False)
        PerformLayout()
    End Sub

    Friend WithEvents txtuser As Guna.UI2.WinForms.Guna2TextBox
    Friend WithEvents txtpass As Guna.UI2.WinForms.Guna2TextBox
    Friend WithEvents Label1 As Label
    Friend WithEvents Label2 As Label
    Friend WithEvents btnlogin As Guna.UI2.WinForms.Guna2Button
    Friend WithEvents Panel1 As Panel
    Friend WithEvents Label3 As Label
    Friend WithEvents Panel_btnlogin As Panel
    Friend WithEvents logintimer As Timer
    Friend WithEvents Guna2Elipse1 As Guna.UI2.WinForms.Guna2Elipse
    Friend WithEvents Guna2ControlBox1 As Guna.UI2.WinForms.Guna2ControlBox
    Friend WithEvents PictureBox1 As PictureBox
    Friend WithEvents Guna2CirclePictureBox1 As Guna.UI2.WinForms.Guna2CirclePictureBox
End Class
