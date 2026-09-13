<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class NewPassword
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
        Dim CustomizableEdges3 As Guna.UI2.WinForms.Suite.CustomizableEdges = New Guna.UI2.WinForms.Suite.CustomizableEdges()
        Dim CustomizableEdges4 As Guna.UI2.WinForms.Suite.CustomizableEdges = New Guna.UI2.WinForms.Suite.CustomizableEdges()
        Dim CustomizableEdges5 As Guna.UI2.WinForms.Suite.CustomizableEdges = New Guna.UI2.WinForms.Suite.CustomizableEdges()
        Dim CustomizableEdges6 As Guna.UI2.WinForms.Suite.CustomizableEdges = New Guna.UI2.WinForms.Suite.CustomizableEdges()
        Dim CustomizableEdges7 As Guna.UI2.WinForms.Suite.CustomizableEdges = New Guna.UI2.WinForms.Suite.CustomizableEdges()
        Dim CustomizableEdges8 As Guna.UI2.WinForms.Suite.CustomizableEdges = New Guna.UI2.WinForms.Suite.CustomizableEdges()
        Dim CustomizableEdges9 As Guna.UI2.WinForms.Suite.CustomizableEdges = New Guna.UI2.WinForms.Suite.CustomizableEdges()
        Dim CustomizableEdges10 As Guna.UI2.WinForms.Suite.CustomizableEdges = New Guna.UI2.WinForms.Suite.CustomizableEdges()
        Dim CustomizableEdges1 As Guna.UI2.WinForms.Suite.CustomizableEdges = New Guna.UI2.WinForms.Suite.CustomizableEdges()
        Dim CustomizableEdges2 As Guna.UI2.WinForms.Suite.CustomizableEdges = New Guna.UI2.WinForms.Suite.CustomizableEdges()
        Guna2Elipse1 = New Guna.UI2.WinForms.Guna2Elipse(components)
        Guna2Elipse2 = New Guna.UI2.WinForms.Guna2Elipse(components)
        Panel1 = New Panel()
        Label3 = New Label()
        Guna2ControlBox1 = New Guna.UI2.WinForms.Guna2ControlBox()
        Label5 = New Label()
        txtpass = New Guna.UI2.WinForms.Guna2TextBox()
        Label2 = New Label()
        Label1 = New Label()
        Guna2Elipse3 = New Guna.UI2.WinForms.Guna2Elipse(components)
        btnback = New Guna.UI2.WinForms.Guna2Button()
        btnreset = New Guna.UI2.WinForms.Guna2Button()
        txtconfirm = New Guna.UI2.WinForms.Guna2TextBox()
        PictureBox2 = New PictureBox()
        PictureBox1 = New PictureBox()
        Panel1.SuspendLayout()
        CType(PictureBox2, ComponentModel.ISupportInitialize).BeginInit()
        CType(PictureBox1, ComponentModel.ISupportInitialize).BeginInit()
        SuspendLayout()
        ' 
        ' Guna2Elipse1
        ' 
        Guna2Elipse1.BorderRadius = 15
        Guna2Elipse1.TargetControl = Me
        ' 
        ' Guna2Elipse2
        ' 
        Guna2Elipse2.BorderRadius = 15
        ' 
        ' Panel1
        ' 
        Panel1.BackColor = Color.FromArgb(CByte(207), CByte(58), CByte(109))
        Panel1.Controls.Add(Label3)
        Panel1.Controls.Add(Guna2ControlBox1)
        Panel1.Controls.Add(Label5)
        Panel1.Dock = DockStyle.Top
        Panel1.Location = New Point(0, 0)
        Panel1.Name = "Panel1"
        Panel1.Size = New Size(406, 70)
        Panel1.TabIndex = 90
        ' 
        ' Label3
        ' 
        Label3.AutoSize = True
        Label3.Font = New Font("Segoe UI", 24F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        Label3.ForeColor = Color.White
        Label3.Location = New Point(23, 15)
        Label3.Name = "Label3"
        Label3.Size = New Size(64, 45)
        Label3.TabIndex = 79
        Label3.Text = "🔒"
        ' 
        ' Guna2ControlBox1
        ' 
        Guna2ControlBox1.Anchor = AnchorStyles.Top Or AnchorStyles.Right
        Guna2ControlBox1.CustomizableEdges = CustomizableEdges3
        Guna2ControlBox1.FillColor = Color.Transparent
        Guna2ControlBox1.IconColor = Color.White
        Guna2ControlBox1.Location = New Point(571, 12)
        Guna2ControlBox1.Name = "Guna2ControlBox1"
        Guna2ControlBox1.ShadowDecoration.CustomizableEdges = CustomizableEdges4
        Guna2ControlBox1.Size = New Size(45, 29)
        Guna2ControlBox1.TabIndex = 78
        ' 
        ' Label5
        ' 
        Label5.AutoSize = True
        Label5.Font = New Font("Arial Narrow", 18F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        Label5.ForeColor = Color.White
        Label5.Location = New Point(120, 22)
        Label5.Name = "Label5"
        Label5.Size = New Size(178, 29)
        Label5.TabIndex = 77
        Label5.Text = "NEW PASSWORD"
        ' 
        ' txtpass
        ' 
        txtpass.BorderRadius = 8
        txtpass.CustomizableEdges = CustomizableEdges5
        txtpass.DefaultText = ""
        txtpass.DisabledState.BorderColor = Color.FromArgb(CByte(208), CByte(208), CByte(208))
        txtpass.DisabledState.FillColor = Color.FromArgb(CByte(226), CByte(226), CByte(226))
        txtpass.DisabledState.ForeColor = Color.FromArgb(CByte(138), CByte(138), CByte(138))
        txtpass.DisabledState.PlaceholderForeColor = Color.FromArgb(CByte(138), CByte(138), CByte(138))
        txtpass.FocusedState.BorderColor = Color.FromArgb(CByte(94), CByte(148), CByte(255))
        txtpass.Font = New Font("Segoe UI", 9F)
        txtpass.ForeColor = Color.Black
        txtpass.HoverState.BorderColor = Color.FromArgb(CByte(94), CByte(148), CByte(255))
        txtpass.Location = New Point(17, 169)
        txtpass.Name = "txtpass"
        txtpass.PlaceholderForeColor = Color.Gray
        txtpass.PlaceholderText = "New Password"
        txtpass.SelectedText = ""
        txtpass.ShadowDecoration.CustomizableEdges = CustomizableEdges6
        txtpass.Size = New Size(364, 53)
        txtpass.TabIndex = 93
        ' 
        ' Label2
        ' 
        Label2.AutoSize = True
        Label2.Font = New Font("Segoe UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        Label2.ForeColor = SystemColors.ControlDarkDark
        Label2.Location = New Point(18, 126)
        Label2.Name = "Label2"
        Label2.Size = New Size(337, 20)
        Label2.TabIndex = 92
        Label2.Text = "Enter your new password (minimum 6 characters)."
        ' 
        ' Label1
        ' 
        Label1.AutoSize = True
        Label1.Font = New Font("Arial Narrow", 15.75F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        Label1.ForeColor = Color.Black
        Label1.Location = New Point(18, 92)
        Label1.Name = "Label1"
        Label1.Size = New Size(278, 25)
        Label1.TabIndex = 91
        Label1.Text = "Final Step: Create New Password"
        ' 
        ' Guna2Elipse3
        ' 
        Guna2Elipse3.BorderRadius = 15
        ' 
        ' btnback
        ' 
        btnback.BorderRadius = 8
        btnback.CustomizableEdges = CustomizableEdges7
        btnback.DisabledState.BorderColor = Color.DarkGray
        btnback.DisabledState.CustomBorderColor = Color.DarkGray
        btnback.DisabledState.FillColor = Color.FromArgb(CByte(169), CByte(169), CByte(169))
        btnback.DisabledState.ForeColor = Color.FromArgb(CByte(141), CByte(141), CByte(141))
        btnback.FillColor = Color.FromArgb(CByte(207), CByte(58), CByte(109))
        btnback.Font = New Font("Segoe UI", 14.25F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        btnback.ForeColor = Color.White
        btnback.Location = New Point(17, 381)
        btnback.Name = "btnback"
        btnback.ShadowDecoration.CustomizableEdges = CustomizableEdges8
        btnback.Size = New Size(364, 53)
        btnback.TabIndex = 95
        btnback.Text = "Cancel"
        ' 
        ' btnreset
        ' 
        btnreset.BorderRadius = 8
        btnreset.CustomizableEdges = CustomizableEdges9
        btnreset.DisabledState.BorderColor = Color.DarkGray
        btnreset.DisabledState.CustomBorderColor = Color.DarkGray
        btnreset.DisabledState.FillColor = Color.FromArgb(CByte(169), CByte(169), CByte(169))
        btnreset.DisabledState.ForeColor = Color.FromArgb(CByte(141), CByte(141), CByte(141))
        btnreset.FillColor = Color.FromArgb(CByte(207), CByte(58), CByte(109))
        btnreset.Font = New Font("Segoe UI", 14.25F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        btnreset.ForeColor = Color.White
        btnreset.Location = New Point(17, 315)
        btnreset.Name = "btnreset"
        btnreset.ShadowDecoration.CustomizableEdges = CustomizableEdges10
        btnreset.Size = New Size(364, 53)
        btnreset.TabIndex = 94
        btnreset.Text = "🔑 Reset Password"
        ' 
        ' txtconfirm
        ' 
        txtconfirm.BorderRadius = 8
        txtconfirm.CustomizableEdges = CustomizableEdges1
        txtconfirm.DefaultText = ""
        txtconfirm.DisabledState.BorderColor = Color.FromArgb(CByte(208), CByte(208), CByte(208))
        txtconfirm.DisabledState.FillColor = Color.FromArgb(CByte(226), CByte(226), CByte(226))
        txtconfirm.DisabledState.ForeColor = Color.FromArgb(CByte(138), CByte(138), CByte(138))
        txtconfirm.DisabledState.PlaceholderForeColor = Color.FromArgb(CByte(138), CByte(138), CByte(138))
        txtconfirm.FocusedState.BorderColor = Color.FromArgb(CByte(94), CByte(148), CByte(255))
        txtconfirm.Font = New Font("Segoe UI", 9F)
        txtconfirm.ForeColor = Color.Black
        txtconfirm.HoverState.BorderColor = Color.FromArgb(CByte(94), CByte(148), CByte(255))
        txtconfirm.Location = New Point(17, 238)
        txtconfirm.Name = "txtconfirm"
        txtconfirm.PlaceholderForeColor = Color.Gray
        txtconfirm.PlaceholderText = "Confirm Password"
        txtconfirm.SelectedText = ""
        txtconfirm.ShadowDecoration.CustomizableEdges = CustomizableEdges2
        txtconfirm.Size = New Size(364, 53)
        txtconfirm.TabIndex = 96
        ' 
        ' PictureBox2
        ' 
        PictureBox2.BackColor = Color.White
        PictureBox2.BackgroundImageLayout = ImageLayout.Zoom
        PictureBox2.Location = New Point(338, 186)
        PictureBox2.Name = "PictureBox2"
        PictureBox2.Size = New Size(28, 18)
        PictureBox2.SizeMode = PictureBoxSizeMode.Zoom
        PictureBox2.TabIndex = 126
        PictureBox2.TabStop = False
        ' 
        ' PictureBox1
        ' 
        PictureBox1.BackColor = Color.White
        PictureBox1.BackgroundImageLayout = ImageLayout.Zoom
        PictureBox1.Location = New Point(338, 257)
        PictureBox1.Name = "PictureBox1"
        PictureBox1.Size = New Size(28, 18)
        PictureBox1.SizeMode = PictureBoxSizeMode.Zoom
        PictureBox1.TabIndex = 127
        PictureBox1.TabStop = False
        ' 
        ' NewPassword
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        ClientSize = New Size(406, 457)
        Controls.Add(PictureBox1)
        Controls.Add(PictureBox2)
        Controls.Add(txtconfirm)
        Controls.Add(Panel1)
        Controls.Add(txtpass)
        Controls.Add(Label2)
        Controls.Add(Label1)
        Controls.Add(btnback)
        Controls.Add(btnreset)
        FormBorderStyle = FormBorderStyle.None
        MaximizeBox = False
        MinimizeBox = False
        Name = "NewPassword"
        ShowIcon = False
        StartPosition = FormStartPosition.CenterScreen
        Text = "NewPassword"
        Panel1.ResumeLayout(False)
        Panel1.PerformLayout()
        CType(PictureBox2, ComponentModel.ISupportInitialize).EndInit()
        CType(PictureBox1, ComponentModel.ISupportInitialize).EndInit()
        ResumeLayout(False)
        PerformLayout()
    End Sub

    Friend WithEvents Guna2Elipse1 As Guna.UI2.WinForms.Guna2Elipse
    Friend WithEvents Panel1 As Panel
    Friend WithEvents Label3 As Label
    Friend WithEvents Guna2ControlBox1 As Guna.UI2.WinForms.Guna2ControlBox
    Friend WithEvents Label5 As Label
    Friend WithEvents txtpass As Guna.UI2.WinForms.Guna2TextBox
    Friend WithEvents Label2 As Label
    Friend WithEvents Label1 As Label
    Friend WithEvents btnback As Guna.UI2.WinForms.Guna2Button
    Friend WithEvents btnreset As Guna.UI2.WinForms.Guna2Button
    Friend WithEvents Guna2Elipse2 As Guna.UI2.WinForms.Guna2Elipse
    Friend WithEvents Guna2Elipse3 As Guna.UI2.WinForms.Guna2Elipse
    Friend WithEvents txtconfirm As Guna.UI2.WinForms.Guna2TextBox
    Friend WithEvents PictureBox1 As PictureBox
    Friend WithEvents PictureBox2 As PictureBox
End Class
