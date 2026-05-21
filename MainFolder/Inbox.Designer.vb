<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Inbox
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
        Dim DataGridViewCellStyle1 As DataGridViewCellStyle = New DataGridViewCellStyle()
        Dim DataGridViewCellStyle2 As DataGridViewCellStyle = New DataGridViewCellStyle()
        Dim DataGridViewCellStyle3 As DataGridViewCellStyle = New DataGridViewCellStyle()
        Dim DataGridViewCellStyle4 As DataGridViewCellStyle = New DataGridViewCellStyle()
        Dim CustomizableEdges1 As Guna.UI2.WinForms.Suite.CustomizableEdges = New Guna.UI2.WinForms.Suite.CustomizableEdges()
        Dim CustomizableEdges2 As Guna.UI2.WinForms.Suite.CustomizableEdges = New Guna.UI2.WinForms.Suite.CustomizableEdges()
        DataGridView1 = New DataGridView()
        txtsearch = New Guna.UI2.WinForms.Guna2TextBox()
        Guna2Elipse1 = New Guna.UI2.WinForms.Guna2Elipse(components)
        CType(DataGridView1, ComponentModel.ISupportInitialize).BeginInit()
        SuspendLayout()
        ' 
        ' DataGridView1
        ' 
        DataGridView1.AllowUserToAddRows = False
        DataGridView1.AllowUserToDeleteRows = False
        DataGridView1.AllowUserToResizeColumns = False
        DataGridView1.AllowUserToResizeRows = False
        DataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells
        DataGridView1.BackgroundColor = SystemColors.Control
        DataGridViewCellStyle1.Alignment = DataGridViewContentAlignment.TopCenter
        DataGridViewCellStyle1.BackColor = Color.FromArgb(CByte(207), CByte(58), CByte(109))
        DataGridViewCellStyle1.Font = New Font("Tahoma", 9F, FontStyle.Bold, GraphicsUnit.Point, CByte(0))
        DataGridViewCellStyle1.ForeColor = Color.White
        DataGridViewCellStyle1.SelectionBackColor = Color.FromArgb(CByte(207), CByte(58), CByte(109))
        DataGridViewCellStyle1.SelectionForeColor = Color.White
        DataGridViewCellStyle1.WrapMode = DataGridViewTriState.True
        DataGridView1.ColumnHeadersDefaultCellStyle = DataGridViewCellStyle1
        DataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize
        DataGridView1.Cursor = Cursors.Hand
        DataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.TopCenter
        DataGridViewCellStyle2.BackColor = SystemColors.Window
        DataGridViewCellStyle2.Font = New Font("Tahoma", 9F, FontStyle.Bold, GraphicsUnit.Point, CByte(0))
        DataGridViewCellStyle2.ForeColor = Color.White
        DataGridViewCellStyle2.SelectionBackColor = SystemColors.Highlight
        DataGridViewCellStyle2.SelectionForeColor = SystemColors.HighlightText
        DataGridViewCellStyle2.WrapMode = DataGridViewTriState.False
        DataGridView1.DefaultCellStyle = DataGridViewCellStyle2
        DataGridView1.Location = New Point(12, 86)
        DataGridView1.MultiSelect = False
        DataGridView1.Name = "DataGridView1"
        DataGridView1.ReadOnly = True
        DataGridViewCellStyle3.Alignment = DataGridViewContentAlignment.TopCenter
        DataGridViewCellStyle3.BackColor = SystemColors.Control
        DataGridViewCellStyle3.Font = New Font("Tahoma", 9F, FontStyle.Bold, GraphicsUnit.Point, CByte(0))
        DataGridViewCellStyle3.ForeColor = SystemColors.WindowText
        DataGridViewCellStyle3.SelectionBackColor = SystemColors.Highlight
        DataGridViewCellStyle3.SelectionForeColor = SystemColors.HighlightText
        DataGridViewCellStyle3.WrapMode = DataGridViewTriState.True
        DataGridView1.RowHeadersDefaultCellStyle = DataGridViewCellStyle3
        DataGridView1.RowHeadersVisible = False
        DataGridView1.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing
        DataGridViewCellStyle4.Alignment = DataGridViewContentAlignment.TopCenter
        DataGridViewCellStyle4.Font = New Font("Tahoma", 9F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        DataGridViewCellStyle4.ForeColor = Color.Black
        DataGridView1.RowsDefaultCellStyle = DataGridViewCellStyle4
        DataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        DataGridView1.Size = New Size(495, 366)
        DataGridView1.TabIndex = 29
        ' 
        ' txtsearch
        ' 
        txtsearch.BorderColor = Color.FromArgb(CByte(207), CByte(58), CByte(109))
        txtsearch.BorderRadius = 10
        txtsearch.CustomizableEdges = CustomizableEdges1
        txtsearch.DefaultText = ""
        txtsearch.DisabledState.BorderColor = Color.FromArgb(CByte(208), CByte(208), CByte(208))
        txtsearch.DisabledState.FillColor = Color.FromArgb(CByte(226), CByte(226), CByte(226))
        txtsearch.DisabledState.ForeColor = Color.FromArgb(CByte(138), CByte(138), CByte(138))
        txtsearch.DisabledState.PlaceholderForeColor = Color.FromArgb(CByte(138), CByte(138), CByte(138))
        txtsearch.FocusedState.BorderColor = Color.FromArgb(CByte(94), CByte(148), CByte(255))
        txtsearch.Font = New Font("Baskerville Old Face", 12F, FontStyle.Bold)
        txtsearch.ForeColor = Color.Black
        txtsearch.HoverState.BorderColor = Color.FromArgb(CByte(94), CByte(148), CByte(255))
        txtsearch.Location = New Point(13, 28)
        txtsearch.Margin = New Padding(4)
        txtsearch.Name = "txtsearch"
        txtsearch.PlaceholderText = "Search Name"
        txtsearch.SelectedText = ""
        txtsearch.ShadowDecoration.CustomizableEdges = CustomizableEdges2
        txtsearch.Size = New Size(493, 30)
        txtsearch.TabIndex = 30
        ' 
        ' Guna2Elipse1
        ' 
        Guna2Elipse1.BorderRadius = 9
        Guna2Elipse1.TargetControl = DataGridView1
        ' 
        ' Inbox
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        ClientSize = New Size(519, 495)
        Controls.Add(txtsearch)
        Controls.Add(DataGridView1)
        FormBorderStyle = FormBorderStyle.FixedDialog
        KeyPreview = True
        MaximizeBox = False
        MinimizeBox = False
        Name = "Inbox"
        ShowIcon = False
        StartPosition = FormStartPosition.CenterScreen
        Text = "EMAIL_INBOX"
        CType(DataGridView1, ComponentModel.ISupportInitialize).EndInit()
        ResumeLayout(False)
    End Sub

    Friend WithEvents DataGridView1 As DataGridView
    Friend WithEvents txtsearch As Guna.UI2.WinForms.Guna2TextBox
    Friend WithEvents Guna2Elipse1 As Guna.UI2.WinForms.Guna2Elipse
End Class
