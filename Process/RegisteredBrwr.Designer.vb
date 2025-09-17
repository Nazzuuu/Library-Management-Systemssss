<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class RegisteredBrwr
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
        PictureBox1 = New PictureBox()
        txtsearch = New Guna.UI2.WinForms.Guna2TextBox()
        ListView1 = New ListView()
        txtborrower = New ColumnHeader()
        txtfname = New ColumnHeader()
        txtlname = New ColumnHeader()
        txtmname = New ColumnHeader()
        txtlrn = New ColumnHeader()
        txtemployeeno = New ColumnHeader()
        txtcontactno = New ColumnHeader()
        txtdepartment = New ColumnHeader()
        txtgrade = New ColumnHeader()
        txtsection = New ColumnHeader()
        txtstrand = New ColumnHeader()
        Guna2GroupBox1 = New Guna.UI2.WinForms.Guna2GroupBox()
        Label1 = New Label()
        Label2 = New Label()
        Label3 = New Label()
        Label4 = New Label()
        lbl_action = New Label()
        CType(PictureBox1, ComponentModel.ISupportInitialize).BeginInit()
        Guna2GroupBox1.SuspendLayout()
        SuspendLayout()
        ' 
        ' PictureBox1
        ' 
        PictureBox1.BackColor = Color.White
        PictureBox1.BackgroundImage = My.Resources.Resources.magnifier
        PictureBox1.BackgroundImageLayout = ImageLayout.Stretch
        PictureBox1.Location = New Point(755, 25)
        PictureBox1.Name = "PictureBox1"
        PictureBox1.Size = New Size(20, 18)
        PictureBox1.TabIndex = 22
        PictureBox1.TabStop = False
        ' 
        ' txtsearch
        ' 
        txtsearch.BorderColor = Color.FromArgb(CByte(207), CByte(58), CByte(109))
        txtsearch.BorderRadius = 16
        txtsearch.CustomizableEdges = CustomizableEdges5
        txtsearch.DefaultText = ""
        txtsearch.DisabledState.BorderColor = Color.FromArgb(CByte(208), CByte(208), CByte(208))
        txtsearch.DisabledState.FillColor = Color.FromArgb(CByte(226), CByte(226), CByte(226))
        txtsearch.DisabledState.ForeColor = Color.FromArgb(CByte(138), CByte(138), CByte(138))
        txtsearch.DisabledState.PlaceholderForeColor = Color.FromArgb(CByte(138), CByte(138), CByte(138))
        txtsearch.FocusedState.BorderColor = Color.FromArgb(CByte(94), CByte(148), CByte(255))
        txtsearch.Font = New Font("Baskerville Old Face", 12F, FontStyle.Bold)
        txtsearch.ForeColor = Color.Black
        txtsearch.HoverState.BorderColor = Color.FromArgb(CByte(94), CByte(148), CByte(255))
        txtsearch.Location = New Point(13, 20)
        txtsearch.Margin = New Padding(4)
        txtsearch.Name = "txtsearch"
        txtsearch.PlaceholderText = "Search Borrwer"
        txtsearch.SelectedText = ""
        txtsearch.ShadowDecoration.CustomizableEdges = CustomizableEdges6
        txtsearch.Size = New Size(774, 30)
        txtsearch.TabIndex = 23
        ' 
        ' ListView1
        ' 
        ListView1.Columns.AddRange(New ColumnHeader() {txtborrower, txtfname, txtlname, txtmname, txtlrn, txtemployeeno, txtcontactno, txtdepartment, txtgrade, txtsection, txtstrand})
        ListView1.Cursor = Cursors.Hand
        ListView1.Font = New Font("Tahoma", 9.75F, FontStyle.Bold)
        ListView1.ForeColor = Color.Black
        ListView1.FullRowSelect = True
        ListView1.GridLines = True
        ListView1.Location = New Point(25, 17)
        ListView1.Name = "ListView1"
        ListView1.OwnerDraw = True
        ListView1.Size = New Size(1114, 260)
        ListView1.TabIndex = 25
        ListView1.UseCompatibleStateImageBehavior = False
        ListView1.View = View.Details
        ' 
        ' txtborrower
        ' 
        txtborrower.Text = "Borrower"
        txtborrower.Width = 100
        ' 
        ' txtfname
        ' 
        txtfname.Text = "FirstName"
        txtfname.Width = 120
        ' 
        ' txtlname
        ' 
        txtlname.Text = "LastName"
        txtlname.Width = 120
        ' 
        ' txtmname
        ' 
        txtmname.Text = "MiddleName"
        txtmname.Width = 120
        ' 
        ' txtlrn
        ' 
        txtlrn.Text = "LRN"
        txtlrn.Width = 80
        ' 
        ' txtemployeeno
        ' 
        txtemployeeno.Text = "EmployeeNo"
        txtemployeeno.Width = 125
        ' 
        ' txtcontactno
        ' 
        txtcontactno.Text = "ContactNumber"
        txtcontactno.Width = 117
        ' 
        ' txtdepartment
        ' 
        txtdepartment.Text = "Department"
        txtdepartment.Width = 152
        ' 
        ' txtgrade
        ' 
        txtgrade.Text = "Grade"
        txtgrade.Width = 65
        ' 
        ' txtsection
        ' 
        txtsection.Text = "Section"
        txtsection.Width = 100
        ' 
        ' txtstrand
        ' 
        txtstrand.Text = "Strand"
        txtstrand.Width = 120
        ' 
        ' Guna2GroupBox1
        ' 
        Guna2GroupBox1.BorderColor = Color.FromArgb(CByte(207), CByte(58), CByte(109))
        Guna2GroupBox1.BorderRadius = 8
        Guna2GroupBox1.BorderThickness = 2
        Guna2GroupBox1.Controls.Add(ListView1)
        Guna2GroupBox1.CustomBorderColor = Color.Transparent
        Guna2GroupBox1.CustomizableEdges = CustomizableEdges7
        Guna2GroupBox1.FillColor = SystemColors.Control
        Guna2GroupBox1.Font = New Font("Segoe UI", 9F)
        Guna2GroupBox1.ForeColor = Color.Transparent
        Guna2GroupBox1.Location = New Point(13, 143)
        Guna2GroupBox1.Name = "Guna2GroupBox1"
        Guna2GroupBox1.ShadowDecoration.CustomizableEdges = CustomizableEdges8
        Guna2GroupBox1.Size = New Size(1170, 294)
        Guna2GroupBox1.TabIndex = 24
        ' 
        ' Label1
        ' 
        Label1.AutoSize = True
        Label1.Font = New Font("Tahoma", 9.75F, FontStyle.Bold, GraphicsUnit.Point, CByte(0))
        Label1.ForeColor = Color.Red
        Label1.Location = New Point(85, 83)
        Label1.Name = "Label1"
        Label1.Size = New Size(226, 16)
        Label1.TabIndex = 86
        Label1.Text = "The borrower is not in the library."
        ' 
        ' Label2
        ' 
        Label2.AutoSize = True
        Label2.Font = New Font("Tahoma", 9.75F, FontStyle.Bold, GraphicsUnit.Point, CByte(0))
        Label2.ForeColor = Color.Green
        Label2.Location = New Point(85, 102)
        Label2.Name = "Label2"
        Label2.Size = New Size(324, 16)
        Label2.TabIndex = 87
        Label2.Text = "This borrower is currently timed in to the library."
        ' 
        ' Label3
        ' 
        Label3.AutoSize = True
        Label3.Font = New Font("Tahoma", 9.75F, FontStyle.Bold, GraphicsUnit.Point, CByte(0))
        Label3.ForeColor = Color.Black
        Label3.Location = New Point(27, 89)
        Label3.Name = "Label3"
        Label3.Size = New Size(49, 16)
        Label3.TabIndex = 88
        Label3.Text = "Notes:"
        ' 
        ' Label4
        ' 
        Label4.AutoSize = True
        Label4.Font = New Font("Tahoma", 12F, FontStyle.Bold, GraphicsUnit.Point, CByte(0))
        Label4.ForeColor = Color.DimGray
        Label4.Location = New Point(909, 80)
        Label4.Name = "Label4"
        Label4.Size = New Size(134, 19)
        Label4.TabIndex = 89
        Label4.Text = "Current Action:"
        ' 
        ' lbl_action
        ' 
        lbl_action.AutoSize = True
        lbl_action.Font = New Font("Tahoma", 12F, FontStyle.Bold, GraphicsUnit.Point, CByte(0))
        lbl_action.ForeColor = Color.Black
        lbl_action.Location = New Point(1045, 81)
        lbl_action.Name = "lbl_action"
        lbl_action.Size = New Size(61, 19)
        lbl_action.TabIndex = 90
        lbl_action.Text = "Action"
        ' 
        ' RegisteredBrwr
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        ClientSize = New Size(1196, 470)
        Controls.Add(lbl_action)
        Controls.Add(Label4)
        Controls.Add(Label3)
        Controls.Add(Label2)
        Controls.Add(Label1)
        Controls.Add(Guna2GroupBox1)
        Controls.Add(PictureBox1)
        Controls.Add(txtsearch)
        KeyPreview = True
        MaximizeBox = False
        MinimizeBox = False
        Name = "RegisteredBrwr"
        ShowIcon = False
        StartPosition = FormStartPosition.CenterScreen
        Text = "RegisteredBrwr"
        CType(PictureBox1, ComponentModel.ISupportInitialize).EndInit()
        Guna2GroupBox1.ResumeLayout(False)
        ResumeLayout(False)
        PerformLayout()
    End Sub

    Friend WithEvents PictureBox1 As PictureBox
    Friend WithEvents txtsearch As Guna.UI2.WinForms.Guna2TextBox
    Friend WithEvents Guna2GroupBox1 As Guna.UI2.WinForms.Guna2GroupBox
    Friend WithEvents ListView1 As ListView
    Friend WithEvents txtborrower As ColumnHeader
    Friend WithEvents txtfname As ColumnHeader
    Friend WithEvents txtlname As ColumnHeader
    Friend WithEvents txtmname As ColumnHeader
    Friend WithEvents txtlrn As ColumnHeader
    Friend WithEvents txtemployeeno As ColumnHeader
    Friend WithEvents txtcontactno As ColumnHeader
    Friend WithEvents txtdepartment As ColumnHeader
    Friend WithEvents txtgrade As ColumnHeader
    Friend WithEvents txtsection As ColumnHeader
    Friend WithEvents txtstrand As ColumnHeader
    Friend WithEvents Label1 As Label
    Friend WithEvents Label2 As Label
    Friend WithEvents Label3 As Label
    Friend WithEvents Label4 As Label
    Friend WithEvents lbl_action As Label
End Class
