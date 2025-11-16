<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class LibrarianPassword
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
        Label1 = New Label()
        txtpassword = New Guna.UI2.WinForms.Guna2TextBox()
        PictureBox2 = New PictureBox()
        CType(PictureBox2, ComponentModel.ISupportInitialize).BeginInit()
        SuspendLayout()
        ' 
        ' Label1
        ' 
        Label1.AutoSize = True
        Label1.Font = New Font("Tahoma", 9.75F, FontStyle.Bold, GraphicsUnit.Point, CByte(0))
        Label1.ForeColor = Color.FromArgb(CByte(207), CByte(58), CByte(109))
        Label1.Location = New Point(17, 21)
        Label1.Name = "Label1"
        Label1.Size = New Size(114, 16)
        Label1.TabIndex = 6
        Label1.Text = "Enter Password:"
        ' 
        ' txtpassword
        ' 
        txtpassword.BorderColor = Color.FromArgb(CByte(207), CByte(58), CByte(109))
        txtpassword.BorderRadius = 8
        txtpassword.CustomizableEdges = CustomizableEdges1
        txtpassword.DefaultText = ""
        txtpassword.DisabledState.BorderColor = Color.FromArgb(CByte(208), CByte(208), CByte(208))
        txtpassword.DisabledState.FillColor = Color.FromArgb(CByte(226), CByte(226), CByte(226))
        txtpassword.DisabledState.ForeColor = Color.FromArgb(CByte(138), CByte(138), CByte(138))
        txtpassword.DisabledState.PlaceholderForeColor = Color.FromArgb(CByte(138), CByte(138), CByte(138))
        txtpassword.FocusedState.BorderColor = Color.FromArgb(CByte(94), CByte(148), CByte(255))
        txtpassword.Font = New Font("Microsoft Sans Serif", 9F)
        txtpassword.ForeColor = Color.Black
        txtpassword.HoverState.BorderColor = Color.FromArgb(CByte(94), CByte(148), CByte(255))
        txtpassword.Location = New Point(17, 40)
        txtpassword.Margin = New Padding(3, 4, 3, 4)
        txtpassword.MaxLength = 20
        txtpassword.Name = "txtpassword"
        txtpassword.PlaceholderText = ""
        txtpassword.SelectedText = ""
        txtpassword.ShadowDecoration.CustomizableEdges = CustomizableEdges2
        txtpassword.Size = New Size(262, 33)
        txtpassword.TabIndex = 5
        ' 
        ' PictureBox2
        ' 
        PictureBox2.BackColor = Color.White
        PictureBox2.BackgroundImageLayout = ImageLayout.Zoom
        PictureBox2.Location = New Point(240, 47)
        PictureBox2.Name = "PictureBox2"
        PictureBox2.Size = New Size(28, 18)
        PictureBox2.SizeMode = PictureBoxSizeMode.Zoom
        PictureBox2.TabIndex = 126
        PictureBox2.TabStop = False
        ' 
        ' LibrarianPassword
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        ClientSize = New Size(297, 112)
        Controls.Add(PictureBox2)
        Controls.Add(Label1)
        Controls.Add(txtpassword)
        FormBorderStyle = FormBorderStyle.FixedDialog
        KeyPreview = True
        MaximizeBox = False
        MinimizeBox = False
        Name = "LibrarianPassword"
        StartPosition = FormStartPosition.CenterScreen
        Text = "Librarian Password"
        CType(PictureBox2, ComponentModel.ISupportInitialize).EndInit()
        ResumeLayout(False)
        PerformLayout()
    End Sub

    Friend WithEvents Label1 As Label
    Friend WithEvents txtpassword As Guna.UI2.WinForms.Guna2TextBox
    Friend WithEvents PictureBox2 As PictureBox
End Class
