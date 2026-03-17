<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class TimeInOutScan
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
        cbaction = New Guna.UI2.WinForms.Guna2ComboBox()
        Label3 = New Label()
        txtidtype = New Guna.UI2.WinForms.Guna2TextBox()
        SuspendLayout()
        ' 
        ' cbaction
        ' 
        cbaction.BackColor = Color.Transparent
        cbaction.BorderColor = Color.FromArgb(CByte(207), CByte(58), CByte(109))
        cbaction.BorderRadius = 12
        cbaction.CustomizableEdges = CustomizableEdges1
        cbaction.DrawMode = DrawMode.OwnerDrawFixed
        cbaction.DropDownStyle = ComboBoxStyle.DropDownList
        cbaction.FocusedColor = Color.FromArgb(CByte(94), CByte(148), CByte(255))
        cbaction.FocusedState.BorderColor = Color.FromArgb(CByte(94), CByte(148), CByte(255))
        cbaction.Font = New Font("Tahoma", 9.75F, FontStyle.Bold)
        cbaction.ForeColor = Color.Black
        cbaction.ItemHeight = 30
        cbaction.Items.AddRange(New Object() {"Time-In", "Time-Out"})
        cbaction.Location = New Point(106, 35)
        cbaction.Name = "cbaction"
        cbaction.ShadowDecoration.CustomizableEdges = CustomizableEdges2
        cbaction.Size = New Size(162, 36)
        cbaction.TabIndex = 31
        ' 
        ' Label3
        ' 
        Label3.AutoSize = True
        Label3.Font = New Font("Tahoma", 9.75F, FontStyle.Bold, GraphicsUnit.Point, CByte(0))
        Label3.ForeColor = Color.FromArgb(CByte(207), CByte(58), CByte(109))
        Label3.Location = New Point(162, 16)
        Label3.Name = "Label3"
        Label3.Size = New Size(54, 16)
        Label3.TabIndex = 32
        Label3.Text = "Action:"
        ' 
        ' txtidtype
        ' 
        txtidtype.BorderColor = Color.FromArgb(CByte(207), CByte(58), CByte(109))
        txtidtype.BorderRadius = 8
        txtidtype.CustomizableEdges = CustomizableEdges3
        txtidtype.DefaultText = ""
        txtidtype.DisabledState.BorderColor = Color.FromArgb(CByte(208), CByte(208), CByte(208))
        txtidtype.DisabledState.FillColor = Color.FromArgb(CByte(226), CByte(226), CByte(226))
        txtidtype.DisabledState.ForeColor = Color.FromArgb(CByte(138), CByte(138), CByte(138))
        txtidtype.DisabledState.PlaceholderForeColor = Color.FromArgb(CByte(138), CByte(138), CByte(138))
        txtidtype.FocusedState.BorderColor = Color.FromArgb(CByte(94), CByte(148), CByte(255))
        txtidtype.Font = New Font("Segoe UI", 9F)
        txtidtype.ForeColor = Color.Black
        txtidtype.HoverState.BorderColor = Color.FromArgb(CByte(94), CByte(148), CByte(255))
        txtidtype.Location = New Point(12, 92)
        txtidtype.MaxLength = 1000
        txtidtype.Name = "txtidtype"
        txtidtype.PlaceholderForeColor = Color.Silver
        txtidtype.PlaceholderText = "LRN or Employee No"
        txtidtype.SelectedText = ""
        txtidtype.ShadowDecoration.CustomizableEdges = CustomizableEdges4
        txtidtype.Size = New Size(349, 42)
        txtidtype.TabIndex = 42
        ' 
        ' TimeInOutScan
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        ClientSize = New Size(373, 154)
        Controls.Add(txtidtype)
        Controls.Add(cbaction)
        Controls.Add(Label3)
        FormBorderStyle = FormBorderStyle.FixedDialog
        MaximizeBox = False
        MinimizeBox = False
        Name = "TimeInOutScan"
        ShowIcon = False
        StartPosition = FormStartPosition.CenterScreen
        Tag = ""
        Text = "Scan your ID Type"
        ResumeLayout(False)
        PerformLayout()
    End Sub

    Friend WithEvents cbaction As Guna.UI2.WinForms.Guna2ComboBox
    Friend WithEvents Label3 As Label
    Friend WithEvents txtidtype As Guna.UI2.WinForms.Guna2TextBox
End Class
