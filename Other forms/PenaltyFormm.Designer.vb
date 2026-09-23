<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class PenaltyFormm
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
        Dim CustomizableEdges5 As Guna.UI2.WinForms.Suite.CustomizableEdges = New Guna.UI2.WinForms.Suite.CustomizableEdges()
        Dim CustomizableEdges6 As Guna.UI2.WinForms.Suite.CustomizableEdges = New Guna.UI2.WinForms.Suite.CustomizableEdges()
        Dim CustomizableEdges7 As Guna.UI2.WinForms.Suite.CustomizableEdges = New Guna.UI2.WinForms.Suite.CustomizableEdges()
        Dim CustomizableEdges8 As Guna.UI2.WinForms.Suite.CustomizableEdges = New Guna.UI2.WinForms.Suite.CustomizableEdges()
        GroupBox1 = New GroupBox()
        btnpenaltystudents = New Guna.UI2.WinForms.Guna2Button()
        btnpenaltyteachers = New Guna.UI2.WinForms.Guna2Button()
        GroupBox1.SuspendLayout()
        SuspendLayout()
        ' 
        ' GroupBox1
        ' 
        GroupBox1.Controls.Add(btnpenaltystudents)
        GroupBox1.Controls.Add(btnpenaltyteachers)
        GroupBox1.Font = New Font("Segoe UI Emoji", 11.25F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        GroupBox1.Location = New Point(12, 12)
        GroupBox1.Name = "GroupBox1"
        GroupBox1.Size = New Size(260, 199)
        GroupBox1.TabIndex = 0
        GroupBox1.TabStop = False
        GroupBox1.Text = "Select Penalty Form"
        ' 
        ' btnpenaltystudents
        ' 
        btnpenaltystudents.BorderColor = Color.FromArgb(CByte(207), CByte(58), CByte(109))
        btnpenaltystudents.BorderRadius = 9
        btnpenaltystudents.BorderThickness = 1
        btnpenaltystudents.CustomizableEdges = CustomizableEdges5
        btnpenaltystudents.DisabledState.BorderColor = Color.DarkGray
        btnpenaltystudents.DisabledState.CustomBorderColor = Color.DarkGray
        btnpenaltystudents.DisabledState.FillColor = Color.FromArgb(CByte(169), CByte(169), CByte(169))
        btnpenaltystudents.DisabledState.ForeColor = Color.FromArgb(CByte(141), CByte(141), CByte(141))
        btnpenaltystudents.FillColor = Color.Empty
        btnpenaltystudents.Font = New Font("Tahoma", 11.25F, FontStyle.Bold)
        btnpenaltystudents.ForeColor = Color.FromArgb(CByte(207), CByte(58), CByte(109))
        btnpenaltystudents.Location = New Point(27, 66)
        btnpenaltystudents.Name = "btnpenaltystudents"
        btnpenaltystudents.ShadowDecoration.CustomizableEdges = CustomizableEdges6
        btnpenaltystudents.Size = New Size(206, 27)
        btnpenaltystudents.TabIndex = 113
        btnpenaltystudents.Text = "Penalty Student Form"
        ' 
        ' btnpenaltyteachers
        ' 
        btnpenaltyteachers.BorderColor = Color.FromArgb(CByte(207), CByte(58), CByte(109))
        btnpenaltyteachers.BorderRadius = 9
        btnpenaltyteachers.BorderThickness = 1
        btnpenaltyteachers.CustomizableEdges = CustomizableEdges7
        btnpenaltyteachers.DisabledState.BorderColor = Color.DarkGray
        btnpenaltyteachers.DisabledState.CustomBorderColor = Color.DarkGray
        btnpenaltyteachers.DisabledState.FillColor = Color.FromArgb(CByte(169), CByte(169), CByte(169))
        btnpenaltyteachers.DisabledState.ForeColor = Color.FromArgb(CByte(141), CByte(141), CByte(141))
        btnpenaltyteachers.FillColor = Color.Empty
        btnpenaltyteachers.Font = New Font("Tahoma", 11.25F, FontStyle.Bold)
        btnpenaltyteachers.ForeColor = Color.FromArgb(CByte(207), CByte(58), CByte(109))
        btnpenaltyteachers.Location = New Point(27, 114)
        btnpenaltyteachers.Name = "btnpenaltyteachers"
        btnpenaltyteachers.ShadowDecoration.CustomizableEdges = CustomizableEdges8
        btnpenaltyteachers.Size = New Size(206, 27)
        btnpenaltyteachers.TabIndex = 114
        btnpenaltyteachers.Text = "Penalty Teacher Form"
        ' 
        ' PenaltyFormm
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        ClientSize = New Size(284, 223)
        Controls.Add(GroupBox1)
        FormBorderStyle = FormBorderStyle.FixedDialog
        MaximizeBox = False
        MinimizeBox = False
        Name = "PenaltyFormm"
        StartPosition = FormStartPosition.CenterScreen
        GroupBox1.ResumeLayout(False)
        ResumeLayout(False)
    End Sub

    Friend WithEvents GroupBox1 As GroupBox
    Friend WithEvents btnpenaltystudents As Guna.UI2.WinForms.Guna2Button
    Friend WithEvents btnpenaltyteachers As Guna.UI2.WinForms.Guna2Button
End Class
