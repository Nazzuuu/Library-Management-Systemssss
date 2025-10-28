<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class ServerConnection
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
        Dim CustomizableEdges7 As Guna.UI2.WinForms.Suite.CustomizableEdges = New Guna.UI2.WinForms.Suite.CustomizableEdges()
        Dim CustomizableEdges8 As Guna.UI2.WinForms.Suite.CustomizableEdges = New Guna.UI2.WinForms.Suite.CustomizableEdges()
        Label6 = New Label()
        txtserver = New Guna.UI2.WinForms.Guna2TextBox()
        txtusername = New Guna.UI2.WinForms.Guna2TextBox()
        txtpassword = New Guna.UI2.WinForms.Guna2TextBox()
        btnconnect = New Button()
        btnsave = New Button()
        btncancel = New Button()
        Label2 = New Label()
        Label3 = New Label()
        Label1 = New Label()
        txtdatabase = New Guna.UI2.WinForms.Guna2TextBox()
        PictureBox1 = New PictureBox()
        CType(PictureBox1, ComponentModel.ISupportInitialize).BeginInit()
        SuspendLayout()
        ' 
        ' Label6
        ' 
        Label6.AutoSize = True
        Label6.Font = New Font("Tahoma", 9.75F, FontStyle.Bold, GraphicsUnit.Point, CByte(0))
        Label6.ForeColor = Color.FromArgb(CByte(207), CByte(58), CByte(109))
        Label6.Location = New Point(30, 36)
        Label6.Name = "Label6"
        Label6.Size = New Size(77, 16)
        Label6.TabIndex = 119
        Label6.Text = "Server/IP:"
        ' 
        ' txtserver
        ' 
        txtserver.BorderColor = Color.FromArgb(CByte(207), CByte(58), CByte(109))
        txtserver.BorderRadius = 6
        txtserver.CustomizableEdges = CustomizableEdges1
        txtserver.DefaultText = ""
        txtserver.DisabledState.BorderColor = Color.FromArgb(CByte(208), CByte(208), CByte(208))
        txtserver.DisabledState.FillColor = Color.FromArgb(CByte(226), CByte(226), CByte(226))
        txtserver.DisabledState.ForeColor = Color.FromArgb(CByte(138), CByte(138), CByte(138))
        txtserver.DisabledState.PlaceholderForeColor = Color.FromArgb(CByte(138), CByte(138), CByte(138))
        txtserver.FocusedState.BorderColor = Color.FromArgb(CByte(94), CByte(148), CByte(255))
        txtserver.Font = New Font("Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        txtserver.ForeColor = Color.Black
        txtserver.HoverState.BorderColor = Color.FromArgb(CByte(94), CByte(148), CByte(255))
        txtserver.Location = New Point(30, 57)
        txtserver.Margin = New Padding(3, 5, 3, 5)
        txtserver.Name = "txtserver"
        txtserver.PlaceholderText = ""
        txtserver.SelectedText = ""
        txtserver.ShadowDecoration.CustomizableEdges = CustomizableEdges2
        txtserver.Size = New Size(242, 33)
        txtserver.TabIndex = 0
        ' 
        ' txtusername
        ' 
        txtusername.BorderColor = Color.FromArgb(CByte(207), CByte(58), CByte(109))
        txtusername.BorderRadius = 6
        txtusername.CustomizableEdges = CustomizableEdges3
        txtusername.DefaultText = ""
        txtusername.DisabledState.BorderColor = Color.FromArgb(CByte(208), CByte(208), CByte(208))
        txtusername.DisabledState.FillColor = Color.FromArgb(CByte(226), CByte(226), CByte(226))
        txtusername.DisabledState.ForeColor = Color.FromArgb(CByte(138), CByte(138), CByte(138))
        txtusername.DisabledState.PlaceholderForeColor = Color.FromArgb(CByte(138), CByte(138), CByte(138))
        txtusername.FocusedState.BorderColor = Color.FromArgb(CByte(94), CByte(148), CByte(255))
        txtusername.Font = New Font("Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        txtusername.ForeColor = Color.Black
        txtusername.HoverState.BorderColor = Color.FromArgb(CByte(94), CByte(148), CByte(255))
        txtusername.Location = New Point(30, 142)
        txtusername.Margin = New Padding(3, 5, 3, 5)
        txtusername.Name = "txtusername"
        txtusername.PlaceholderText = ""
        txtusername.SelectedText = ""
        txtusername.ShadowDecoration.CustomizableEdges = CustomizableEdges4
        txtusername.Size = New Size(242, 33)
        txtusername.TabIndex = 1
        ' 
        ' txtpassword
        ' 
        txtpassword.BorderColor = Color.FromArgb(CByte(207), CByte(58), CByte(109))
        txtpassword.BorderRadius = 6
        txtpassword.CustomizableEdges = CustomizableEdges5
        txtpassword.DefaultText = ""
        txtpassword.DisabledState.BorderColor = Color.FromArgb(CByte(208), CByte(208), CByte(208))
        txtpassword.DisabledState.FillColor = Color.FromArgb(CByte(226), CByte(226), CByte(226))
        txtpassword.DisabledState.ForeColor = Color.FromArgb(CByte(138), CByte(138), CByte(138))
        txtpassword.DisabledState.PlaceholderForeColor = Color.FromArgb(CByte(138), CByte(138), CByte(138))
        txtpassword.FocusedState.BorderColor = Color.FromArgb(CByte(94), CByte(148), CByte(255))
        txtpassword.Font = New Font("Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        txtpassword.ForeColor = Color.Black
        txtpassword.HoverState.BorderColor = Color.FromArgb(CByte(94), CByte(148), CByte(255))
        txtpassword.Location = New Point(30, 222)
        txtpassword.Margin = New Padding(3, 5, 3, 5)
        txtpassword.Name = "txtpassword"
        txtpassword.PlaceholderText = ""
        txtpassword.SelectedText = ""
        txtpassword.ShadowDecoration.CustomizableEdges = CustomizableEdges6
        txtpassword.Size = New Size(242, 33)
        txtpassword.TabIndex = 2
        ' 
        ' btnconnect
        ' 
        btnconnect.BackColor = Color.FromArgb(CByte(207), CByte(58), CByte(109))
        btnconnect.FlatStyle = FlatStyle.Flat
        btnconnect.Font = New Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, CByte(0))
        btnconnect.ForeColor = Color.White
        btnconnect.Location = New Point(316, 52)
        btnconnect.Name = "btnconnect"
        btnconnect.Size = New Size(130, 50)
        btnconnect.TabIndex = 123
        btnconnect.Text = "CONNECT"
        btnconnect.UseVisualStyleBackColor = False
        ' 
        ' btnsave
        ' 
        btnsave.BackColor = Color.FromArgb(CByte(207), CByte(58), CByte(109))
        btnsave.FlatStyle = FlatStyle.Flat
        btnsave.Font = New Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, CByte(0))
        btnsave.ForeColor = Color.White
        btnsave.Location = New Point(316, 132)
        btnsave.Name = "btnsave"
        btnsave.Size = New Size(130, 50)
        btnsave.TabIndex = 124
        btnsave.Text = "SAVE"
        btnsave.UseVisualStyleBackColor = False
        ' 
        ' btncancel
        ' 
        btncancel.BackColor = Color.FromArgb(CByte(207), CByte(58), CByte(109))
        btncancel.FlatStyle = FlatStyle.Flat
        btncancel.Font = New Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, CByte(0))
        btncancel.ForeColor = Color.White
        btncancel.Location = New Point(316, 213)
        btncancel.Name = "btncancel"
        btncancel.Size = New Size(130, 50)
        btncancel.TabIndex = 125
        btncancel.Text = "CANCEL"
        btncancel.UseVisualStyleBackColor = False
        ' 
        ' Label2
        ' 
        Label2.AutoSize = True
        Label2.Font = New Font("Tahoma", 9.75F, FontStyle.Bold, GraphicsUnit.Point, CByte(0))
        Label2.ForeColor = Color.FromArgb(CByte(207), CByte(58), CByte(109))
        Label2.Location = New Point(30, 121)
        Label2.Name = "Label2"
        Label2.Size = New Size(76, 16)
        Label2.TabIndex = 127
        Label2.Text = "Username:"
        ' 
        ' Label3
        ' 
        Label3.AutoSize = True
        Label3.Font = New Font("Tahoma", 9.75F, FontStyle.Bold, GraphicsUnit.Point, CByte(0))
        Label3.ForeColor = Color.FromArgb(CByte(207), CByte(58), CByte(109))
        Label3.Location = New Point(30, 201)
        Label3.Name = "Label3"
        Label3.Size = New Size(75, 16)
        Label3.TabIndex = 128
        Label3.Text = "Password:"
        ' 
        ' Label1
        ' 
        Label1.AutoSize = True
        Label1.Font = New Font("Tahoma", 9.75F, FontStyle.Bold, GraphicsUnit.Point, CByte(0))
        Label1.ForeColor = Color.FromArgb(CByte(207), CByte(58), CByte(109))
        Label1.Location = New Point(30, 313)
        Label1.Name = "Label1"
        Label1.Size = New Size(74, 16)
        Label1.TabIndex = 130
        Label1.Text = "Database:"
        Label1.Visible = False
        ' 
        ' txtdatabase
        ' 
        txtdatabase.BorderColor = Color.FromArgb(CByte(207), CByte(58), CByte(109))
        txtdatabase.BorderRadius = 6
        txtdatabase.CustomizableEdges = CustomizableEdges7
        txtdatabase.DefaultText = ""
        txtdatabase.DisabledState.BorderColor = Color.FromArgb(CByte(208), CByte(208), CByte(208))
        txtdatabase.DisabledState.FillColor = Color.FromArgb(CByte(226), CByte(226), CByte(226))
        txtdatabase.DisabledState.ForeColor = Color.FromArgb(CByte(138), CByte(138), CByte(138))
        txtdatabase.DisabledState.PlaceholderForeColor = Color.FromArgb(CByte(138), CByte(138), CByte(138))
        txtdatabase.FocusedState.BorderColor = Color.FromArgb(CByte(94), CByte(148), CByte(255))
        txtdatabase.Font = New Font("Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        txtdatabase.ForeColor = Color.Black
        txtdatabase.HoverState.BorderColor = Color.FromArgb(CByte(94), CByte(148), CByte(255))
        txtdatabase.Location = New Point(30, 334)
        txtdatabase.Margin = New Padding(3, 5, 3, 5)
        txtdatabase.Name = "txtdatabase"
        txtdatabase.PlaceholderText = ""
        txtdatabase.SelectedText = ""
        txtdatabase.ShadowDecoration.CustomizableEdges = CustomizableEdges8
        txtdatabase.Size = New Size(242, 33)
        txtdatabase.TabIndex = 129
        txtdatabase.Visible = False
        ' 
        ' PictureBox1
        ' 
        PictureBox1.BackColor = Color.White
        PictureBox1.BackgroundImageLayout = ImageLayout.Zoom
        PictureBox1.Location = New Point(237, 229)
        PictureBox1.Name = "PictureBox1"
        PictureBox1.Size = New Size(28, 18)
        PictureBox1.SizeMode = PictureBoxSizeMode.Zoom
        PictureBox1.TabIndex = 131
        PictureBox1.TabStop = False
        ' 
        ' ServerConnection
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        ClientSize = New Size(485, 314)
        Controls.Add(PictureBox1)
        Controls.Add(Label1)
        Controls.Add(txtdatabase)
        Controls.Add(Label3)
        Controls.Add(Label2)
        Controls.Add(btncancel)
        Controls.Add(btnsave)
        Controls.Add(btnconnect)
        Controls.Add(txtpassword)
        Controls.Add(txtusername)
        Controls.Add(Label6)
        Controls.Add(txtserver)
        FormBorderStyle = FormBorderStyle.FixedDialog
        KeyPreview = True
        MaximizeBox = False
        MinimizeBox = False
        Name = "ServerConnection"
        ShowIcon = False
        StartPosition = FormStartPosition.CenterScreen
        Text = "ServerConnection"
        CType(PictureBox1, ComponentModel.ISupportInitialize).EndInit()
        ResumeLayout(False)
        PerformLayout()
    End Sub

    Friend WithEvents Label6 As Label
    Friend WithEvents txtserver As Guna.UI2.WinForms.Guna2TextBox
    Friend WithEvents txtusername As Guna.UI2.WinForms.Guna2TextBox
    Friend WithEvents txtpassword As Guna.UI2.WinForms.Guna2TextBox
    Friend WithEvents btnconnect As Button
    Friend WithEvents btnsave As Button
    Friend WithEvents btncancel As Button
    Friend WithEvents Label2 As Label
    Friend WithEvents Label3 As Label
    Friend WithEvents Label1 As Label
    Friend WithEvents txtdatabase As Guna.UI2.WinForms.Guna2TextBox
    Friend WithEvents PictureBox1 As PictureBox
End Class
