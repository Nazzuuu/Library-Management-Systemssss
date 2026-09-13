<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class ResetPassword1
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
        Panel1 = New Panel()
        Label4 = New Label()
        Guna2ControlBox1 = New Guna.UI2.WinForms.Guna2ControlBox()
        Label5 = New Label()
        Label1 = New Label()
        Label2 = New Label()
        Label3 = New Label()
        txtenter1 = New Guna.UI2.WinForms.Guna2TextBox()
        Guna2Elipse1 = New Guna.UI2.WinForms.Guna2Elipse(components)
        btnreset = New Guna.UI2.WinForms.Guna2Button()
        Panel1.SuspendLayout()
        SuspendLayout()
        ' 
        ' Panel1
        ' 
        Panel1.BackColor = Color.FromArgb(CByte(207), CByte(58), CByte(109))
        Panel1.Controls.Add(Label4)
        Panel1.Controls.Add(Guna2ControlBox1)
        Panel1.Controls.Add(Label5)
        Panel1.Dock = DockStyle.Top
        Panel1.Location = New Point(0, 0)
        Panel1.Name = "Panel1"
        Panel1.Size = New Size(422, 70)
        Panel1.TabIndex = 0
        ' 
        ' Label4
        ' 
        Label4.AutoSize = True
        Label4.Font = New Font("Segoe UI", 24F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        Label4.ForeColor = Color.White
        Label4.Location = New Point(23, 15)
        Label4.Name = "Label4"
        Label4.Size = New Size(64, 45)
        Label4.TabIndex = 83
        Label4.Text = "🔑"
        ' 
        ' Guna2ControlBox1
        ' 
        Guna2ControlBox1.Anchor = AnchorStyles.Top Or AnchorStyles.Right
        Guna2ControlBox1.CustomizableEdges = CustomizableEdges1
        Guna2ControlBox1.FillColor = Color.Transparent
        Guna2ControlBox1.IconColor = Color.White
        Guna2ControlBox1.Location = New Point(365, 12)
        Guna2ControlBox1.Name = "Guna2ControlBox1"
        Guna2ControlBox1.ShadowDecoration.CustomizableEdges = CustomizableEdges2
        Guna2ControlBox1.Size = New Size(45, 29)
        Guna2ControlBox1.TabIndex = 78
        ' 
        ' Label5
        ' 
        Label5.AutoSize = True
        Label5.Font = New Font("Arial Narrow", 18F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        Label5.ForeColor = Color.White
        Label5.Location = New Point(88, 22)
        Label5.Name = "Label5"
        Label5.Size = New Size(258, 29)
        Label5.TabIndex = 77
        Label5.Text = "RESET YOUR PASSWORD"
        ' 
        ' Label1
        ' 
        Label1.AutoSize = True
        Label1.Font = New Font("Arial Narrow", 15.75F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        Label1.ForeColor = Color.Black
        Label1.Location = New Point(18, 86)
        Label1.Name = "Label1"
        Label1.Size = New Size(333, 25)
        Label1.TabIndex = 78
        Label1.Text = "Borrower:  Enter Your LRN/Employee No"
        ' 
        ' Label2
        ' 
        Label2.AutoSize = True
        Label2.Font = New Font("Segoe UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        Label2.ForeColor = SystemColors.ControlDarkDark
        Label2.Location = New Point(18, 143)
        Label2.Name = "Label2"
        Label2.Size = New Size(275, 40)
        Label2.TabIndex = 79
        Label2.Text = "Enter your Student/Teacher ID to" & vbCrLf & "receive a password reset code via email."
        ' 
        ' Label3
        ' 
        Label3.AutoSize = True
        Label3.Font = New Font("Arial Narrow", 15.75F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        Label3.ForeColor = Color.Black
        Label3.Location = New Point(18, 111)
        Label3.Name = "Label3"
        Label3.Size = New Size(377, 25)
        Label3.TabIndex = 80
        Label3.Text = "Librarian/Assistant Lib/Staff:  Enter Your Email"
        ' 
        ' txtenter1
        ' 
        txtenter1.BorderRadius = 8
        txtenter1.CustomizableEdges = CustomizableEdges3
        txtenter1.DefaultText = ""
        txtenter1.DisabledState.BorderColor = Color.FromArgb(CByte(208), CByte(208), CByte(208))
        txtenter1.DisabledState.FillColor = Color.FromArgb(CByte(226), CByte(226), CByte(226))
        txtenter1.DisabledState.ForeColor = Color.FromArgb(CByte(138), CByte(138), CByte(138))
        txtenter1.DisabledState.PlaceholderForeColor = Color.FromArgb(CByte(138), CByte(138), CByte(138))
        txtenter1.FocusedState.BorderColor = Color.FromArgb(CByte(94), CByte(148), CByte(255))
        txtenter1.Font = New Font("Segoe UI", 9F)
        txtenter1.ForeColor = Color.Black
        txtenter1.HoverState.BorderColor = Color.FromArgb(CByte(94), CByte(148), CByte(255))
        txtenter1.Location = New Point(23, 212)
        txtenter1.Name = "txtenter1"
        txtenter1.PlaceholderForeColor = Color.Gray
        txtenter1.PlaceholderText = "Enter LRN/Employee No/Email"
        txtenter1.SelectedText = ""
        txtenter1.ShadowDecoration.CustomizableEdges = CustomizableEdges4
        txtenter1.Size = New Size(364, 53)
        txtenter1.TabIndex = 81
        ' 
        ' Guna2Elipse1
        ' 
        Guna2Elipse1.BorderRadius = 15
        Guna2Elipse1.TargetControl = Me
        ' 
        ' btnreset
        ' 
        btnreset.BorderRadius = 8
        btnreset.CustomizableEdges = CustomizableEdges5
        btnreset.DisabledState.BorderColor = Color.DarkGray
        btnreset.DisabledState.CustomBorderColor = Color.DarkGray
        btnreset.DisabledState.FillColor = Color.FromArgb(CByte(169), CByte(169), CByte(169))
        btnreset.DisabledState.ForeColor = Color.FromArgb(CByte(141), CByte(141), CByte(141))
        btnreset.FillColor = Color.FromArgb(CByte(207), CByte(58), CByte(109))
        btnreset.Font = New Font("Segoe UI", 14.25F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        btnreset.ForeColor = Color.White
        btnreset.Location = New Point(23, 286)
        btnreset.Name = "btnreset"
        btnreset.ShadowDecoration.CustomizableEdges = CustomizableEdges6
        btnreset.Size = New Size(364, 53)
        btnreset.TabIndex = 82
        btnreset.Text = "✉️ Send Reset Code"
        ' 
        ' ResetPassword1
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        ClientSize = New Size(422, 373)
        Controls.Add(btnreset)
        Controls.Add(txtenter1)
        Controls.Add(Label3)
        Controls.Add(Label2)
        Controls.Add(Label1)
        Controls.Add(Panel1)
        FormBorderStyle = FormBorderStyle.None
        MaximizeBox = False
        MinimizeBox = False
        Name = "ResetPassword1"
        ShowIcon = False
        StartPosition = FormStartPosition.CenterScreen
        Panel1.ResumeLayout(False)
        Panel1.PerformLayout()
        ResumeLayout(False)
        PerformLayout()
    End Sub

    Friend WithEvents Panel1 As Panel
    Friend WithEvents Label5 As Label
    Friend WithEvents Label1 As Label
    Friend WithEvents Label2 As Label
    Friend WithEvents Label3 As Label
    Friend WithEvents txtenter1 As Guna.UI2.WinForms.Guna2TextBox
    Friend WithEvents Guna2ControlBox1 As Guna.UI2.WinForms.Guna2ControlBox
    Friend WithEvents Guna2Elipse1 As Guna.UI2.WinForms.Guna2Elipse
    Friend WithEvents btnreset As Guna.UI2.WinForms.Guna2Button
    Friend WithEvents Label4 As Label
End Class
