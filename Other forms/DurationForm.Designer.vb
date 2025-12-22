<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class DurationForm
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
        NumericUpDown1 = New Guna.UI2.WinForms.Guna2NumericUpDown()
        NumericUpDown2 = New Guna.UI2.WinForms.Guna2NumericUpDown()
        btnsave = New Guna.UI2.WinForms.Guna2Button()
        Label13 = New Label()
        Label1 = New Label()
        CType(NumericUpDown1, ComponentModel.ISupportInitialize).BeginInit()
        CType(NumericUpDown2, ComponentModel.ISupportInitialize).BeginInit()
        SuspendLayout()
        ' 
        ' NumericUpDown1
        ' 
        NumericUpDown1.BackColor = Color.Transparent
        NumericUpDown1.BorderRadius = 12
        NumericUpDown1.CustomizableEdges = CustomizableEdges1
        NumericUpDown1.Font = New Font("Segoe UI", 9F)
        NumericUpDown1.Location = New Point(47, 53)
        NumericUpDown1.Name = "NumericUpDown1"
        NumericUpDown1.ShadowDecoration.CustomizableEdges = CustomizableEdges2
        NumericUpDown1.Size = New Size(155, 36)
        NumericUpDown1.TabIndex = 0
        NumericUpDown1.UpDownButtonFillColor = Color.FromArgb(CByte(207), CByte(58), CByte(109))
        ' 
        ' NumericUpDown2
        ' 
        NumericUpDown2.BackColor = Color.Transparent
        NumericUpDown2.BorderRadius = 12
        NumericUpDown2.CustomizableEdges = CustomizableEdges3
        NumericUpDown2.Font = New Font("Segoe UI", 9F)
        NumericUpDown2.Location = New Point(47, 120)
        NumericUpDown2.Name = "NumericUpDown2"
        NumericUpDown2.ShadowDecoration.CustomizableEdges = CustomizableEdges4
        NumericUpDown2.Size = New Size(155, 36)
        NumericUpDown2.TabIndex = 1
        NumericUpDown2.UpDownButtonFillColor = Color.FromArgb(CByte(207), CByte(58), CByte(109))
        ' 
        ' btnsave
        ' 
        btnsave.BorderColor = Color.FromArgb(CByte(207), CByte(58), CByte(109))
        btnsave.BorderRadius = 9
        btnsave.BorderThickness = 1
        btnsave.CustomizableEdges = CustomizableEdges5
        btnsave.DisabledState.BorderColor = Color.DarkGray
        btnsave.DisabledState.CustomBorderColor = Color.DarkGray
        btnsave.DisabledState.FillColor = Color.FromArgb(CByte(169), CByte(169), CByte(169))
        btnsave.DisabledState.ForeColor = Color.FromArgb(CByte(141), CByte(141), CByte(141))
        btnsave.FillColor = Color.Empty
        btnsave.Font = New Font("Tahoma", 11.25F, FontStyle.Bold)
        btnsave.ForeColor = Color.FromArgb(CByte(207), CByte(58), CByte(109))
        btnsave.Location = New Point(71, 190)
        btnsave.Name = "btnsave"
        btnsave.ShadowDecoration.CustomizableEdges = CustomizableEdges6
        btnsave.Size = New Size(112, 27)
        btnsave.TabIndex = 98
        btnsave.Text = "SAVE"
        ' 
        ' Label13
        ' 
        Label13.AutoSize = True
        Label13.Font = New Font("Tahoma", 9.75F, FontStyle.Bold, GraphicsUnit.Point, CByte(0))
        Label13.ForeColor = Color.FromArgb(CByte(207), CByte(58), CByte(109))
        Label13.Location = New Point(47, 34)
        Label13.Name = "Label13"
        Label13.Size = New Size(64, 16)
        Label13.TabIndex = 99
        Label13.Text = "Student:"
        ' 
        ' Label1
        ' 
        Label1.AutoSize = True
        Label1.Font = New Font("Tahoma", 9.75F, FontStyle.Bold, GraphicsUnit.Point, CByte(0))
        Label1.ForeColor = Color.FromArgb(CByte(207), CByte(58), CByte(109))
        Label1.Location = New Point(47, 101)
        Label1.Name = "Label1"
        Label1.Size = New Size(64, 16)
        Label1.TabIndex = 100
        Label1.Text = "Teacher:"
        ' 
        ' DurationForm
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        ClientSize = New Size(258, 242)
        Controls.Add(Label1)
        Controls.Add(Label13)
        Controls.Add(btnsave)
        Controls.Add(NumericUpDown2)
        Controls.Add(NumericUpDown1)
        FormBorderStyle = FormBorderStyle.FixedDialog
        MaximizeBox = False
        MinimizeBox = False
        Name = "DurationForm"
        ShowIcon = False
        StartPosition = FormStartPosition.CenterScreen
        Text = "DurationForm"
        CType(NumericUpDown1, ComponentModel.ISupportInitialize).EndInit()
        CType(NumericUpDown2, ComponentModel.ISupportInitialize).EndInit()
        ResumeLayout(False)
        PerformLayout()
    End Sub

    Friend WithEvents NumericUpDown1 As Guna.UI2.WinForms.Guna2NumericUpDown
    Friend WithEvents NumericUpDown2 As Guna.UI2.WinForms.Guna2NumericUpDown
    Friend WithEvents btnsave As Guna.UI2.WinForms.Guna2Button
    Friend WithEvents Label13 As Label
    Friend WithEvents Label1 As Label
End Class
