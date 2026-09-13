<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class EditStatus
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
        txtauthor = New Guna.UI2.WinForms.Guna2TextBox()
        btnsave = New Guna.UI2.WinForms.Guna2Button()
        SuspendLayout()
        ' 
        ' txtauthor
        ' 
        txtauthor.BorderColor = Color.FromArgb(CByte(207), CByte(58), CByte(109))
        txtauthor.BorderRadius = 8
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
        txtauthor.Location = New Point(12, 36)
        txtauthor.Margin = New Padding(3, 5, 3, 5)
        txtauthor.MaxLength = 100
        txtauthor.Name = "txtauthor"
        txtauthor.PlaceholderText = ""
        txtauthor.SelectedText = ""
        txtauthor.ShadowDecoration.CustomizableEdges = CustomizableEdges2
        txtauthor.Size = New Size(271, 33)
        txtauthor.TabIndex = 4
        ' 
        ' btnsave
        ' 
        btnsave.BorderColor = Color.FromArgb(CByte(207), CByte(58), CByte(109))
        btnsave.BorderRadius = 9
        btnsave.BorderThickness = 1
        btnsave.CustomizableEdges = CustomizableEdges3
        btnsave.DisabledState.BorderColor = Color.DarkGray
        btnsave.DisabledState.CustomBorderColor = Color.DarkGray
        btnsave.DisabledState.FillColor = Color.FromArgb(CByte(169), CByte(169), CByte(169))
        btnsave.DisabledState.ForeColor = Color.FromArgb(CByte(141), CByte(141), CByte(141))
        btnsave.FillColor = Color.Empty
        btnsave.Font = New Font("Tahoma", 11.25F, FontStyle.Bold)
        btnsave.ForeColor = Color.FromArgb(CByte(207), CByte(58), CByte(109))
        btnsave.Location = New Point(84, 86)
        btnsave.Name = "btnsave"
        btnsave.ShadowDecoration.CustomizableEdges = CustomizableEdges4
        btnsave.Size = New Size(123, 27)
        btnsave.TabIndex = 97
        btnsave.Text = "SAVE"
        ' 
        ' EditStatus
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        ClientSize = New Size(295, 136)
        Controls.Add(btnsave)
        Controls.Add(txtauthor)
        FormBorderStyle = FormBorderStyle.FixedDialog
        KeyPreview = True
        MaximizeBox = False
        MinimizeBox = False
        Name = "EditStatus"
        StartPosition = FormStartPosition.CenterScreen
        ResumeLayout(False)
    End Sub

    Friend WithEvents txtauthor As Guna.UI2.WinForms.Guna2TextBox
    Friend WithEvents btnsave As Guna.UI2.WinForms.Guna2Button
End Class
