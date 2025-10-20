<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class TransactionNumber
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
        Guna2GroupBox1 = New Guna.UI2.WinForms.Guna2GroupBox()
        ListView1 = New ListView()
        txttransactionno = New ColumnHeader()
        txtsearch = New Guna.UI2.WinForms.Guna2TextBox()
        PictureBox1 = New PictureBox()
        Label3 = New Label()
        Label1 = New Label()
        Label4 = New Label()
        Label2 = New Label()
        Panel3 = New Panel()
        Panel2 = New Panel()
        Guna2GroupBox1.SuspendLayout()
        CType(PictureBox1, ComponentModel.ISupportInitialize).BeginInit()
        SuspendLayout()
        ' 
        ' Guna2GroupBox1
        ' 
        Guna2GroupBox1.BorderColor = Color.FromArgb(CByte(207), CByte(58), CByte(109))
        Guna2GroupBox1.BorderRadius = 8
        Guna2GroupBox1.BorderThickness = 2
        Guna2GroupBox1.Controls.Add(ListView1)
        Guna2GroupBox1.CustomBorderColor = Color.Transparent
        Guna2GroupBox1.CustomizableEdges = CustomizableEdges5
        Guna2GroupBox1.FillColor = SystemColors.Control
        Guna2GroupBox1.Font = New Font("Segoe UI", 9F)
        Guna2GroupBox1.ForeColor = Color.Transparent
        Guna2GroupBox1.Location = New Point(37, 154)
        Guna2GroupBox1.Name = "Guna2GroupBox1"
        Guna2GroupBox1.ShadowDecoration.CustomizableEdges = CustomizableEdges6
        Guna2GroupBox1.Size = New Size(494, 322)
        Guna2GroupBox1.TabIndex = 90
        ' 
        ' ListView1
        ' 
        ListView1.AllowColumnReorder = True
        ListView1.Columns.AddRange(New ColumnHeader() {txttransactionno})
        ListView1.Cursor = Cursors.Hand
        ListView1.Font = New Font("Tahoma", 9.75F, FontStyle.Bold)
        ListView1.ForeColor = Color.Black
        ListView1.FullRowSelect = True
        ListView1.GridLines = True
        ListView1.Location = New Point(38, 24)
        ListView1.Name = "ListView1"
        ListView1.OwnerDraw = True
        ListView1.Size = New Size(418, 273)
        ListView1.TabIndex = 25
        ListView1.UseCompatibleStateImageBehavior = False
        ListView1.View = View.Details
        ' 
        ' txttransactionno
        ' 
        txttransactionno.Text = "TransactionNo"
        txttransactionno.Width = 150
        ' 
        ' txtsearch
        ' 
        txtsearch.BorderColor = Color.FromArgb(CByte(207), CByte(58), CByte(109))
        txtsearch.BorderRadius = 16
        txtsearch.CustomizableEdges = CustomizableEdges7
        txtsearch.DefaultText = ""
        txtsearch.DisabledState.BorderColor = Color.FromArgb(CByte(208), CByte(208), CByte(208))
        txtsearch.DisabledState.FillColor = Color.FromArgb(CByte(226), CByte(226), CByte(226))
        txtsearch.DisabledState.ForeColor = Color.FromArgb(CByte(138), CByte(138), CByte(138))
        txtsearch.DisabledState.PlaceholderForeColor = Color.FromArgb(CByte(138), CByte(138), CByte(138))
        txtsearch.FocusedState.BorderColor = Color.FromArgb(CByte(94), CByte(148), CByte(255))
        txtsearch.Font = New Font("Baskerville Old Face", 12F, FontStyle.Bold)
        txtsearch.ForeColor = Color.Black
        txtsearch.HoverState.BorderColor = Color.FromArgb(CByte(94), CByte(148), CByte(255))
        txtsearch.Location = New Point(27, 21)
        txtsearch.Margin = New Padding(4)
        txtsearch.Name = "txtsearch"
        txtsearch.PlaceholderText = "Search Transaction Number"
        txtsearch.SelectedText = ""
        txtsearch.ShadowDecoration.CustomizableEdges = CustomizableEdges8
        txtsearch.Size = New Size(504, 30)
        txtsearch.TabIndex = 89
        ' 
        ' PictureBox1
        ' 
        PictureBox1.BackColor = Color.White
        PictureBox1.BackgroundImage = My.Resources.Resources.magnifier
        PictureBox1.BackgroundImageLayout = ImageLayout.Stretch
        PictureBox1.Location = New Point(496, 27)
        PictureBox1.Name = "PictureBox1"
        PictureBox1.Size = New Size(21, 18)
        PictureBox1.TabIndex = 91
        PictureBox1.TabStop = False
        ' 
        ' Label3
        ' 
        Label3.AutoSize = True
        Label3.Font = New Font("Tahoma", 9.75F, FontStyle.Bold, GraphicsUnit.Point, CByte(0))
        Label3.ForeColor = Color.Red
        Label3.Location = New Point(41, 82)
        Label3.Name = "Label3"
        Label3.Size = New Size(42, 16)
        Label3.TabIndex = 93
        Label3.Text = "Note:"
        ' 
        ' Label1
        ' 
        Label1.AutoSize = True
        Label1.Font = New Font("Tahoma", 9.75F, FontStyle.Bold, GraphicsUnit.Point, CByte(0))
        Label1.ForeColor = Color.Black
        Label1.Location = New Point(88, 83)
        Label1.Name = "Label1"
        Label1.Size = New Size(296, 16)
        Label1.TabIndex = 92
        Label1.Text = "Choose and double click transaction number."
        ' 
        ' Label4
        ' 
        Label4.AutoSize = True
        Label4.Font = New Font("Tahoma", 9.75F, FontStyle.Bold, GraphicsUnit.Point, CByte(0))
        Label4.ForeColor = Color.Green
        Label4.Location = New Point(89, 127)
        Label4.Name = "Label4"
        Label4.Size = New Size(122, 16)
        Label4.TabIndex = 112
        Label4.Text = "Transaction Used."
        ' 
        ' Label2
        ' 
        Label2.AutoSize = True
        Label2.Font = New Font("Tahoma", 9.75F, FontStyle.Bold, GraphicsUnit.Point, CByte(0))
        Label2.ForeColor = Color.Maroon
        Label2.Location = New Point(89, 103)
        Label2.Name = "Label2"
        Label2.Size = New Size(148, 16)
        Label2.TabIndex = 111
        Label2.Text = "Transaction Not Used."
        ' 
        ' Panel3
        ' 
        Panel3.BackColor = Color.DarkGreen
        Panel3.Location = New Point(40, 126)
        Panel3.Name = "Panel3"
        Panel3.Size = New Size(40, 17)
        Panel3.TabIndex = 110
        ' 
        ' Panel2
        ' 
        Panel2.BackColor = Color.Maroon
        Panel2.Location = New Point(40, 102)
        Panel2.Name = "Panel2"
        Panel2.Size = New Size(40, 17)
        Panel2.TabIndex = 109
        ' 
        ' TransactionNumber
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        ClientSize = New Size(566, 500)
        Controls.Add(Label4)
        Controls.Add(Label2)
        Controls.Add(Panel3)
        Controls.Add(Panel2)
        Controls.Add(Label3)
        Controls.Add(Label1)
        Controls.Add(PictureBox1)
        Controls.Add(Guna2GroupBox1)
        Controls.Add(txtsearch)
        FormBorderStyle = FormBorderStyle.FixedDialog
        KeyPreview = True
        MaximizeBox = False
        MinimizeBox = False
        Name = "TransactionNumber"
        ShowIcon = False
        StartPosition = FormStartPosition.CenterScreen
        Text = "transactionnumber"
        Guna2GroupBox1.ResumeLayout(False)
        CType(PictureBox1, ComponentModel.ISupportInitialize).EndInit()
        ResumeLayout(False)
        PerformLayout()
    End Sub

    Friend WithEvents Guna2GroupBox1 As Guna.UI2.WinForms.Guna2GroupBox
    Friend WithEvents ListView1 As ListView
    Friend WithEvents txtsearch As Guna.UI2.WinForms.Guna2TextBox
    Friend WithEvents txttransactionno As ColumnHeader
    Friend WithEvents PictureBox1 As PictureBox
    Friend WithEvents Label3 As Label
    Friend WithEvents Label1 As Label
    Friend WithEvents Label4 As Label
    Friend WithEvents Label2 As Label
    Friend WithEvents Panel3 As Panel
    Friend WithEvents Panel2 As Panel
End Class
