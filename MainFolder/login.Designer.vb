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
        Dim CustomizableEdges1 As Guna.UI2.WinForms.Suite.CustomizableEdges = New Guna.UI2.WinForms.Suite.CustomizableEdges()
        Dim CustomizableEdges2 As Guna.UI2.WinForms.Suite.CustomizableEdges = New Guna.UI2.WinForms.Suite.CustomizableEdges()
        Dim CustomizableEdges3 As Guna.UI2.WinForms.Suite.CustomizableEdges = New Guna.UI2.WinForms.Suite.CustomizableEdges()
        Dim CustomizableEdges4 As Guna.UI2.WinForms.Suite.CustomizableEdges = New Guna.UI2.WinForms.Suite.CustomizableEdges()
        Dim CustomizableEdges5 As Guna.UI2.WinForms.Suite.CustomizableEdges = New Guna.UI2.WinForms.Suite.CustomizableEdges()
        Dim CustomizableEdges6 As Guna.UI2.WinForms.Suite.CustomizableEdges = New Guna.UI2.WinForms.Suite.CustomizableEdges()
        txtuser = New Guna.UI2.WinForms.Guna2TextBox()
        txtpass = New Guna.UI2.WinForms.Guna2TextBox()
        Label1 = New Label()
        Label2 = New Label()
        Guna2Button1 = New Guna.UI2.WinForms.Guna2Button()
        Panel1 = New Panel()
        Label3 = New Label()
        CheckBox1 = New CheckBox()
        Panel2 = New Panel()
        Panel1.SuspendLayout()
        Panel2.SuspendLayout()
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
        txtuser.Location = New Point(117, 120)
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
        txtpass.Location = New Point(117, 164)
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
        Label1.Location = New Point(30, 122)
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
        Label2.Location = New Point(34, 166)
        Label2.Name = "Label2"
        Label2.Size = New Size(81, 18)
        Label2.TabIndex = 3
        Label2.Text = "Password:"
        ' 
        ' Guna2Button1
        ' 
        Guna2Button1.BorderRadius = 15
        Guna2Button1.CustomizableEdges = CustomizableEdges5
        Guna2Button1.DisabledState.BorderColor = Color.DarkGray
        Guna2Button1.DisabledState.CustomBorderColor = Color.DarkGray
        Guna2Button1.DisabledState.FillColor = Color.FromArgb(CByte(169), CByte(169), CByte(169))
        Guna2Button1.DisabledState.ForeColor = Color.FromArgb(CByte(141), CByte(141), CByte(141))
        Guna2Button1.FillColor = Color.FromArgb(CByte(207), CByte(58), CByte(109))
        Guna2Button1.Font = New Font("Baskerville Old Face", 12F, FontStyle.Bold)
        Guna2Button1.ForeColor = Color.White
        Guna2Button1.Location = New Point(21, 12)
        Guna2Button1.Name = "Guna2Button1"
        Guna2Button1.ShadowDecoration.CustomizableEdges = CustomizableEdges6
        Guna2Button1.Size = New Size(155, 33)
        Guna2Button1.TabIndex = 4
        Guna2Button1.Text = "login"
        ' 
        ' Panel1
        ' 
        Panel1.Anchor = AnchorStyles.Top
        Panel1.BackColor = Color.FromArgb(CByte(207), CByte(58), CByte(109))
        Panel1.Controls.Add(Label3)
        Panel1.Location = New Point(-1, 0)
        Panel1.Name = "Panel1"
        Panel1.Size = New Size(445, 58)
        Panel1.TabIndex = 5
        ' 
        ' Label3
        ' 
        Label3.Anchor = AnchorStyles.Top
        Label3.AutoSize = True
        Label3.Font = New Font("Baskerville Old Face", 9.75F, FontStyle.Bold, GraphicsUnit.Point, CByte(0))
        Label3.ForeColor = Color.White
        Label3.Location = New Point(78, 9)
        Label3.Name = "Label3"
        Label3.Size = New Size(279, 56)
        Label3.TabIndex = 1
        Label3.Text = "Monlimar Development Academy Integrated" & vbCrLf & "                        Library System" & vbCrLf & "                             (MDILS)" & vbCrLf & "                                        "
        ' 
        ' CheckBox1
        ' 
        CheckBox1.AutoSize = True
        CheckBox1.Location = New Point(332, 171)
        CheckBox1.Name = "CheckBox1"
        CheckBox1.Size = New Size(15, 14)
        CheckBox1.TabIndex = 6
        CheckBox1.UseVisualStyleBackColor = True
        ' 
        ' Panel2
        ' 
        Panel2.Controls.Add(Guna2Button1)
        Panel2.Location = New Point(127, 207)
        Panel2.Name = "Panel2"
        Panel2.Size = New Size(191, 57)
        Panel2.TabIndex = 7
        ' 
        ' login
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        ClientSize = New Size(443, 280)
        ControlBox = False
        Controls.Add(Panel2)
        Controls.Add(CheckBox1)
        Controls.Add(Panel1)
        Controls.Add(Label2)
        Controls.Add(Label1)
        Controls.Add(txtpass)
        Controls.Add(txtuser)
        Name = "login"
        ShowIcon = False
        StartPosition = FormStartPosition.CenterScreen
        Panel1.ResumeLayout(False)
        Panel1.PerformLayout()
        Panel2.ResumeLayout(False)
        ResumeLayout(False)
        PerformLayout()
    End Sub

    Friend WithEvents txtuser As Guna.UI2.WinForms.Guna2TextBox
    Friend WithEvents txtpass As Guna.UI2.WinForms.Guna2TextBox
    Friend WithEvents Label1 As Label
    Friend WithEvents Label2 As Label
    Friend WithEvents Guna2Button1 As Guna.UI2.WinForms.Guna2Button
    Friend WithEvents Panel1 As Panel
    Friend WithEvents Label3 As Label
    Friend WithEvents CheckBox1 As CheckBox
    Friend WithEvents Panel2 As Panel
End Class
