<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Staylogoutformm
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
        btnlogout = New Guna.UI2.WinForms.Guna2Button()
        btnstay = New Guna.UI2.WinForms.Guna2Button()
        lblmessage = New Label()
        SuspendLayout()
        ' 
        ' btnlogout
        ' 
        btnlogout.BackColor = SystemColors.ButtonFace
        btnlogout.BorderColor = Color.Red
        btnlogout.BorderRadius = 5
        btnlogout.BorderThickness = 1
        btnlogout.CustomizableEdges = CustomizableEdges1
        btnlogout.DisabledState.BorderColor = Color.DarkGray
        btnlogout.DisabledState.CustomBorderColor = Color.DarkGray
        btnlogout.DisabledState.FillColor = Color.FromArgb(CByte(169), CByte(169), CByte(169))
        btnlogout.DisabledState.ForeColor = Color.FromArgb(CByte(141), CByte(141), CByte(141))
        btnlogout.FillColor = Color.Empty
        btnlogout.Font = New Font("Segoe UI", 11.25F)
        btnlogout.ForeColor = Color.Red
        btnlogout.Location = New Point(271, 98)
        btnlogout.Name = "btnlogout"
        btnlogout.ShadowDecoration.CustomizableEdges = CustomizableEdges2
        btnlogout.Size = New Size(98, 27)
        btnlogout.TabIndex = 51
        btnlogout.Text = "LOGOUT"
        ' 
        ' btnstay
        ' 
        btnstay.BackColor = SystemColors.ButtonFace
        btnstay.BorderColor = Color.Green
        btnstay.BorderRadius = 5
        btnstay.BorderThickness = 1
        btnstay.CustomizableEdges = CustomizableEdges3
        btnstay.DisabledState.BorderColor = Color.DarkGray
        btnstay.DisabledState.CustomBorderColor = Color.DarkGray
        btnstay.DisabledState.FillColor = Color.FromArgb(CByte(169), CByte(169), CByte(169))
        btnstay.DisabledState.ForeColor = Color.FromArgb(CByte(141), CByte(141), CByte(141))
        btnstay.FillColor = Color.Empty
        btnstay.Font = New Font("Segoe UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        btnstay.ForeColor = Color.Green
        btnstay.Location = New Point(158, 98)
        btnstay.Name = "btnstay"
        btnstay.ShadowDecoration.CustomizableEdges = CustomizableEdges4
        btnstay.Size = New Size(90, 27)
        btnstay.TabIndex = 50
        btnstay.Text = "Stay"
        ' 
        ' lblmessage
        ' 
        lblmessage.AutoSize = True
        lblmessage.Location = New Point(33, 49)
        lblmessage.Name = "lblmessage"
        lblmessage.Size = New Size(41, 15)
        lblmessage.TabIndex = 49
        lblmessage.Text = "Label1"
        ' 
        ' StayLogoutFormm
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        ClientSize = New Size(403, 146)
        Controls.Add(btnlogout)
        Controls.Add(btnstay)
        Controls.Add(lblmessage)
        MaximizeBox = False
        MinimizeBox = False
        Name = "StayLogoutFormm"
        ShowIcon = False
        StartPosition = FormStartPosition.CenterScreen
        Text = "Stay or Logout Form"
        ResumeLayout(False)
        PerformLayout()
    End Sub

    Friend WithEvents btnlogout As Guna.UI2.WinForms.Guna2Button
    Friend WithEvents btnstay As Guna.UI2.WinForms.Guna2Button
    Friend WithEvents lblmessage As Label
End Class
