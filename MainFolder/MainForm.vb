Imports Microsoft.VisualBasic.ApplicationServices
Imports System.Drawing
Imports MySql.Data.MySqlClient
Public Class MainForm
    Public Shared BorrowerEditsInfoForm As Borrowereditsinfo
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        Me.WindowState = FormWindowState.Maximized
        sizelocation()


        GlobalVarsModule.CurrentUserRole = "Guest"

        lblborrowcount()
        btnexit.Visible = False
    End Sub

    Private Sub Form1_Resize(sender As Object, e As EventArgs) Handles MyBase.Resize
        sizelocation()
    End Sub

    Public Sub sizelocation()

        If Me.WindowState = FormWindowState.Maximized Then
            lbl_borrow.Location = New Point(150, 90)
            lbl_return.Location = New Point(150, 90)
            lbl_overdue.Location = New Point(150, 90)
            lbl_damage.Location = New Point(150, 90)
            lbl_lost.Location = New Point(150, 90)
            lbl_reserve.Location = New Point(150, 90)

        ElseIf Me.WindowState = FormWindowState.Normal Then

            Panel_dash.Size = New Size(1015, 391)

            lbl_borrow.Location = New Point(95, 39)
            lbl_return.Location = New Point(95, 39)
            lbl_overdue.Location = New Point(95, 39)
            lbl_damage.Location = New Point(95, 39)
            lbl_lost.Location = New Point(95, 39)
            lbl_reserve.Location = New Point(95, 39)

        End If
    End Sub


    Public Sub SetupBorrowerUI(ByVal userType As String)

        Dim newProcessLocation As New Point(25, 13)


        If Panel_Process IsNot Nothing Then
            Panel_Process.Location = newProcessLocation
        End If


        If Panel_maintenance IsNot Nothing Then
            Panel_maintenance.Visible = False
        End If


        If Panel_Studentlogs IsNot Nothing Then
            Panel_Studentlogs.Visible = False
        End If

        If btnexit IsNot Nothing Then
            btnexit.Visible = True
        End If

    End Sub



    Private Sub btnlogoutt_Click(sender As Object, e As EventArgs) Handles btnlogoutt.Click

        Dim dialogResult = MessageBox.Show("Are you sure you want to logout?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question)

        If dialogResult = DialogResult.Yes Then

            Dim previousRole As String = GlobalVarsModule.CurrentUserRole


            GlobalVarsModule.CurrentUserRole = "Guest"
            GlobalVarsModule.CurrentBorrowerType = ""
            GlobalVarsModule.CurrentBorrowerID = ""
            GlobalVarsModule.CurrentUserID = ""

            If Borrowing IsNot Nothing AndAlso Not Borrowing.IsDisposed Then
                Borrowing.Close()
            End If

            Me.Hide()


            If previousRole <> "Borrower" AndAlso Not String.IsNullOrWhiteSpace(previousRole) Then

                login.Show()
            Else

                BorrowerLoginForm.Show()
            End If

        End If

    End Sub


    Private Sub MaintenanceToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles MaintenanceToolStripMenuItem.Click

        MaintenanceToolStripMenuItem.ForeColor = Color.DarkGray

        Dim lumabasna As Boolean = False
        Book.Close()

        Panel_dash.Controls.Remove(Book)
        Panel_dash.Controls.Remove(Borrower)
        Panel_dash.Controls.Remove(Users)
        Panel_dash.Controls.Remove(Acquisition)
        Panel_dash.Controls.Remove(Accession)
        Panel_dash.Controls.Remove(oras)
        Panel_dash.Controls.Remove(Borrowing)
        Panel_dash.Controls.Remove(PrintReceiptForm)
        Panel_dash.Controls.Remove(TimeInOutRecord)
        Panel_dash.Controls.Remove(BorrowingHistory)
        Panel_User.Show()

        If lumabasna = False Then
            Panel_dash.Controls.Add(dshboard)
            Panel_dash.Controls.Add(Panel_User)
            Panel_dash.Controls.Add(Panel_welcome)
            Panel_dash.Controls.Add(Guna2Button1)
            lumabasna = True
        End If

        lblform.Text = "MAIN FORM"

    End Sub

    Private Sub MaintenanceToolStripMenuItem_DropDownClosed(sender As Object, e As EventArgs) Handles MaintenanceToolStripMenuItem.DropDownClosed
        MaintenanceToolStripMenuItem.ForeColor = Color.White
    End Sub

    Private Sub ToolStripMenuItem2_Click(sender As Object, e As EventArgs) Handles SettingsStripMenuItem.Click

        SettingsStripMenuItem.ForeColor = Color.DarkGray

        Dim lumabasna As Boolean = False
        Book.Close()

        Panel_dash.Controls.Remove(Book)
        Panel_dash.Controls.Remove(Borrower)
        Panel_dash.Controls.Remove(Users)
        Panel_dash.Controls.Remove(Acquisition)
        Panel_dash.Controls.Remove(Accession)
        Panel_dash.Controls.Remove(oras)
        Panel_dash.Controls.Remove(Borrowing)
        Panel_dash.Controls.Remove(PrintReceiptForm)
        Panel_dash.Controls.Remove(TimeInOutRecord)
        Panel_dash.Controls.Remove(BorrowingHistory)

        If lumabasna = False Then
            Panel_dash.Controls.Add(dshboard)
            Panel_dash.Controls.Add(Panel_User)
            Panel_dash.Controls.Add(Panel_welcome)
            Panel_dash.Controls.Add(Guna2Button1)
            lumabasna = True
        End If

        lblform.Text = "MAIN FORM"
    End Sub

    Private Sub ToolStripMenuItem2_DropDownClosed(sender As Object, e As EventArgs) Handles SettingsStripMenuItem.DropDownClosed
        SettingsStripMenuItem.ForeColor = Color.White
    End Sub

    Private Sub ActiveToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles Audit_Trail.Click

        SettingsStripMenuItem.ForeColor = Color.White

        Dim lumabasna As Boolean = False
        Book.Close()

        Panel_dash.Controls.Remove(Book)
        Panel_dash.Controls.Remove(Borrower)
        Panel_dash.Controls.Remove(Users)
        Panel_dash.Controls.Remove(Acquisition)
        Panel_dash.Controls.Remove(Accession)
        Panel_dash.Controls.Remove(oras)
        Panel_dash.Controls.Remove(Borrowing)
        Panel_dash.Controls.Remove(PrintReceiptForm)
        Panel_dash.Controls.Remove(TimeInOutRecord)
        Panel_dash.Controls.Remove(BorrowingHistory)

        If lumabasna = False Then
            Panel_dash.Controls.Add(dshboard)
            Panel_dash.Controls.Add(Panel_User)
            Panel_dash.Controls.Add(Panel_welcome)
            Panel_dash.Controls.Add(Guna2Button1)
            lumabasna = True
        End If

    End Sub

    Private Sub ToolStripMenuItem1_DropDownClosed(sender As Object, e As EventArgs) Handles ProcessStripMenuItem.DropDownClosed
        ProcessStripMenuItem.ForeColor = Color.White
    End Sub

    Private Sub ToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles ProcessStripMenuItem.Click

        ProcessStripMenuItem.ShowDropDown()

        ProcessStripMenuItem.ForeColor = Color.DarkGray

        Dim lumabasna As Boolean = False
        Book.Close()

        Panel_dash.Controls.Remove(Book)
        Panel_dash.Controls.Remove(Borrower)
        Panel_dash.Controls.Remove(Users)
        Panel_dash.Controls.Remove(Acquisition)
        Panel_dash.Controls.Remove(Accession)
        Panel_dash.Controls.Remove(oras)
        Panel_dash.Controls.Remove(Borrowing)
        Panel_dash.Controls.Remove(PrintReceiptForm)
        Panel_dash.Controls.Remove(TimeInOutRecord)
        Panel_dash.Controls.Remove(BorrowingHistory)
        Panel_User.Show()

        If lumabasna = False Then
            Panel_dash.Controls.Add(dshboard)
            Panel_dash.Controls.Add(Panel_User)
            Panel_dash.Controls.Add(Panel_welcome)
            Panel_dash.Controls.Add(Guna2Button1)
            lumabasna = True
        End If

        Accession.DataGridView1.ClearSelection()
        Acquisition.DataGridView1.ClearSelection()
        oras.DataGridView1.ClearSelection()

        lblform.Text = "MAIN FORM"

    End Sub


    Private Sub ToolStripMenuItem2_MouseHover(sender As Object, e As EventArgs) Handles SettingsStripMenuItem.MouseHover

        Cursor = Cursors.Hand
    End Sub

    Private Sub ToolStripMenuItem2_MouseLeave(sender As Object, e As EventArgs) Handles SettingsStripMenuItem.MouseLeave
        Cursor = Cursors.Default
    End Sub

    Private Sub MaintenanceToolStripMenuItem_MouseHover(sender As Object, e As EventArgs) Handles MaintenanceToolStripMenuItem.MouseHover
        Me.Cursor = Cursors.Hand
    End Sub

    Private Sub MaintenanceToolStripMenuItem_Mouseleave(sender As Object, e As EventArgs) Handles MaintenanceToolStripMenuItem.MouseLeave
        Me.Cursor = Cursors.Default
    End Sub




    Private Sub btn_borrowed_MouseHover(sender As Object, e As EventArgs) Handles btn_borrowed.MouseHover
        Cursor = Cursors.Hand

    End Sub

    Private Sub btn_rtn_MouseHover(sender As Object, e As EventArgs) Handles btn_rtn.MouseHover
        Me.Cursor = Cursors.Hand
    End Sub

    Private Sub btn_overdue_MouseHover(sender As Object, e As EventArgs) Handles btn_overdue.MouseHover
        Me.Cursor = Cursors.Hand
    End Sub

    Private Sub btn_dmg_MouseHover(sender As Object, e As EventArgs) Handles btn_dmg.MouseHover
        Me.Cursor = Cursors.Hand
    End Sub

    Private Sub btn_lost_MouseHover(sender As Object, e As EventArgs) Handles btn_lost.MouseHover
        Cursor = Cursors.Hand

    End Sub

    Private Sub btn_borrowed_MouseLeave(sender As Object, e As EventArgs) Handles btn_borrowed.MouseLeave
        Cursor = Cursors.Default
    End Sub

    Private Sub btn_rtn_MouseLeave(sender As Object, e As EventArgs) Handles btn_rtn.MouseLeave
        Me.Cursor = Cursors.Default
    End Sub

    Private Sub btn_overdue_MouseLeave(sender As Object, e As EventArgs) Handles btn_overdue.MouseLeave
        Me.Cursor = Cursors.Default
    End Sub

    Private Sub btn_dmg_MouseLeave(sender As Object, e As EventArgs) Handles btn_dmg.MouseLeave
        Me.Cursor = Cursors.Default
    End Sub

    Private Sub btn_lost_MouseLeave(sender As Object, e As EventArgs) Handles btn_lost.MouseLeave
        Cursor = Cursors.Default
    End Sub



    Private Sub SectionToolStripMenuItem2_Click(sender As Object, e As EventArgs) Handles SectionToolStripMenuItem2.Click
        Section.ShowDialog()
    End Sub


    Private Sub BookMaintenanceToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles BookMaintenanceToolStripMenuItem.Click
        Panel_dash.Controls.Clear()

        With Book
            .TopLevel = False
            .TopMost = True
            .BringToFront()
            Panel_dash.Controls.Add(Book)
            Book.Size = New Size(1370, 700)

            .Show()

            Book.DataGridView1.ClearSelection()
            Book.DataGridView1.CurrentCell = Nothing
        End With

        lblform.Text = "BOOK FORM"

    End Sub

    Private Sub SectionToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles SectionToolStripMenuItem1.Click
        Department.ShowDialog()
    End Sub

    Private Sub RegisterToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles RegisterToolStripMenuItem.Click

        Panel_dash.Controls.Clear()


        With Borrower
            .TopLevel = False
            .TopMost = True
            Panel_dash.Controls.Add(Borrower)
            Borrower.Size = New Size(1370, 700)

            .Show()

            Borrower.DataGridView1.ClearSelection()
            Borrower.DataGridView1.CurrentCell = Nothing

        End With

        lblform.Text = "BORROWER FORM"
    End Sub

    Private Sub UserMaintenanceToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles UserMaintenanceToolStripMenuItem.Click

        Panel_dash.Controls.Clear()

        With Users
            .TopLevel = False
            .TopMost = True
            Panel_dash.Controls.Add(Users)
            Users.Size = New Size(1370, 700)

            .Show()

            Users.DataGridView1.ClearSelection()
            Users.DataGridView1.CurrentCell = Nothing
        End With

        lblform.Text = "USER FORM"
    End Sub


    Private Sub GradeToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles GradeToolStripMenuItem1.Click
        Grade.ShowDialog()
    End Sub

    Private Sub StrandToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles StrandToolStripMenuItem1.Click
        Strand.ShowDialog()
    End Sub

    Private Sub AcquisitionToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles AcquisitionToolStripMenuItem.Click

        Panel_dash.Controls.Clear()

        With Acquisition
            .TopMost = True
            .TopLevel = False
            .BringToFront()
            Panel_dash.Controls.Add(Acquisition)


            .Show()

            Acquisition.DataGridView1.ClearSelection()
            Acquisition.DataGridView1.CurrentCell = Nothing


        End With

        lblform.Text = "ACQUISITION FORM"
    End Sub

    Private Sub AccessionToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles AccessionToolStripMenuItem.Click

        Panel_dash.Controls.Clear()

        With Accession
            .TopMost = True
            .TopLevel = False
            .BringToFront()
            Panel_dash.Controls.Add(Accession)

            .Show()

            Accession.DataGridView1.ClearSelection()
            Accession.DataGridView1.CurrentCell = Nothing

        End With

        lblform.Text = "ACCESSION FORM"
    End Sub

    Private Sub TimeInToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles TimeInToolStripMenuItem.Click

        Panel_dash.Controls.Clear()

        With oras

            .TopMost = True
            .TopLevel = False
            .BringToFront()
            Panel_dash.Controls.Add(oras)

            .Show()

            oras.DataGridView1.ClearSelection()
            oras.DataGridView1.CurrentCell = Nothing


        End With

        lblform.Text = "TIME-IN/OUT FORM"
    End Sub

    Private Sub ShelfToolStripMenuItem_Click(sender As Object, e As EventArgs)
        Shelf.ShowDialog()
    End Sub

    Private Sub PenaltyManagementToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles PenaltyManagementToolStripMenuItem.Click
        Penalty_Management.ShowDialog()
    End Sub

    Private Sub BorrowToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles BorrowToolStripMenuItem.Click

        With Borrowing

            Panel_dash.Controls.Clear()
            .TopMost = True
            .TopLevel = False
            .BringToFront()
            Panel_dash.Controls.Add(Borrowing)

            .SetupBorrowerFields()
            .Show()

            Borrowing.DataGridView1.ClearSelection()
            Borrowing.DataGridView1.CurrentCell = Nothing

        End With

        lblform.Text = "BORROWING FORM"

        Borrowing.SetupBorrowerFields()


    End Sub

    Private Sub btninfo_MouseHover(sender As Object, e As EventArgs)

        Cursor = Cursors.Hand

    End Sub

    Private Sub btninfo_Mouseleave(sender As Object, e As EventArgs)

        Cursor = Cursors.Default

    End Sub

    Private Sub EditInfoToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles EditInfoToolStripMenuItem.Click
        Superadmin.ShowDialog()
    End Sub

    Private Sub btn_reserve_MouseHover(sender As Object, e As EventArgs) Handles btn_reserve.MouseHover
        Cursor = Cursors.Hand
    End Sub

    Private Sub btn_reserve_Mouseleave(sender As Object, e As EventArgs) Handles btn_reserve.MouseLeave
        Cursor = Cursors.Default
    End Sub

    Private Sub BookMaintenanceToolStripMenuItem_MouseHover(sender As Object, e As EventArgs) Handles BookMaintenanceToolStripMenuItem.MouseHover
        Cursor = Cursors.Hand

    End Sub

    Private Sub BookMaintenanceToolStripMenuItem_MouseLeave(sender As Object, e As EventArgs) Handles BookMaintenanceToolStripMenuItem.MouseLeave
        Cursor = Cursors.Default

    End Sub

    Private Sub PenaltyManagementToolStripMenuItem_MouseHover(sender As Object, e As EventArgs) Handles PenaltyManagementToolStripMenuItem.MouseHover
        Cursor = Cursors.Hand

    End Sub

    Private Sub PenaltyManagementToolStripMenuItem_MouseLeave(sender As Object, e As EventArgs) Handles PenaltyManagementToolStripMenuItem.MouseLeave
        Cursor = Cursors.Default
    End Sub

    Private Sub DepartmentToolStripMenuItem_MouseHover(sender As Object, e As EventArgs) Handles DepartmentToolStripMenuItem.MouseHover
        Cursor = Cursors.Hand

    End Sub

    Private Sub DepartmentToolStripMenuItem_MouseLeave(sender As Object, e As EventArgs) Handles DepartmentToolStripMenuItem.MouseLeave
        Cursor = Cursors.Default
    End Sub

    Private Sub SectionToolStripMenuItem1_MouseHover(sender As Object, e As EventArgs) Handles SectionToolStripMenuItem1.MouseHover
        Cursor = Cursors.Hand
    End Sub

    Private Sub SectionToolStripMenuItem1_MouseLeave(sender As Object, e As EventArgs) Handles SectionToolStripMenuItem1.MouseLeave
        Cursor = Cursors.Default
    End Sub

    Private Sub SectionToolStripMenuItem2_MouseHover(sender As Object, e As EventArgs) Handles SectionToolStripMenuItem2.MouseHover
        Cursor = Cursors.Hand
    End Sub

    Private Sub SectionToolStripMenuItem2_MouseLeave(sender As Object, e As EventArgs) Handles SectionToolStripMenuItem2.MouseLeave
        Cursor = Cursors.Default
    End Sub

    Private Sub GradeToolStripMenuItem1_MouseHover(sender As Object, e As EventArgs) Handles GradeToolStripMenuItem1.MouseHover
        Cursor = Cursors.Hand
    End Sub

    Private Sub GradeToolStripMenuItem1_MouseLeave(sender As Object, e As EventArgs) Handles GradeToolStripMenuItem1.MouseLeave
        Cursor = Cursors.Default
    End Sub

    Private Sub StrandToolStripMenuItem1_MouseHover(sender As Object, e As EventArgs) Handles StrandToolStripMenuItem1.MouseHover
        Cursor = Cursors.Hand
    End Sub

    Private Sub StrandToolStripMenuItem1_MouseLeave(sender As Object, e As EventArgs) Handles StrandToolStripMenuItem1.MouseLeave
        Cursor = Cursors.Default
    End Sub

    Private Sub RegisterToolStripMenuItem_MouseHover(sender As Object, e As EventArgs) Handles RegisterToolStripMenuItem.MouseHover
        Cursor = Cursors.Hand
    End Sub

    Private Sub RegisterToolStripMenuItem_MouseLeave(sender As Object, e As EventArgs) Handles RegisterToolStripMenuItem.MouseLeave
        Cursor = Cursors.Default
    End Sub

    Private Sub UserMaintenanceToolStripMenuItem_MouseHover(sender As Object, e As EventArgs) Handles UserMaintenanceToolStripMenuItem.MouseHover
        Cursor = Cursors.Hand
    End Sub

    Private Sub UserMaintenanceToolStripMenuItem_MouseLeave(sender As Object, e As EventArgs) Handles UserMaintenanceToolStripMenuItem.MouseLeave
        Cursor = Cursors.Default
    End Sub

    Private Sub AcquisitionToolStripMenuItem_MouseHover(sender As Object, e As EventArgs) Handles AcquisitionToolStripMenuItem.MouseHover
        Cursor = Cursors.Hand
    End Sub

    Private Sub AcquisitionToolStripMenuItem_MouseLeave(sender As Object, e As EventArgs) Handles AcquisitionToolStripMenuItem.MouseLeave
        Cursor = Cursors.Default
    End Sub

    Private Sub AccessionToolStripMenuItem_MouseHover(sender As Object, e As EventArgs) Handles AccessionToolStripMenuItem.MouseHover
        Cursor = Cursors.Hand
    End Sub

    Private Sub AccessionToolStripMenuItem_MouseLeave(sender As Object, e As EventArgs) Handles AccessionToolStripMenuItem.MouseLeave
        Cursor = Cursors.Default
    End Sub

    Private Sub StudentLogsToolStripMenuItem_MouseHover(sender As Object, e As EventArgs) Handles StudentLogsToolStripMenuItem.MouseHover
        Cursor = Cursors.Hand
    End Sub

    Private Sub StudentLogsToolStripMenuItem_MouseLeave(sender As Object, e As EventArgs) Handles StudentLogsToolStripMenuItem.MouseLeave
        Cursor = Cursors.Default
    End Sub

    Private Sub TimeInToolStripMenuItem_MouseHover(sender As Object, e As EventArgs) Handles TimeInToolStripMenuItem.MouseHover
        Cursor = Cursors.Hand
    End Sub

    Private Sub TimeInToolStripMenuItem_MouseLeave(sender As Object, e As EventArgs) Handles TimeInToolStripMenuItem.MouseLeave
        Cursor = Cursors.Default
    End Sub

    Private Sub BorrowToolStripMenuItem_MouseHover(sender As Object, e As EventArgs) Handles BorrowToolStripMenuItem.MouseHover
        Cursor = Cursors.Hand
    End Sub

    Private Sub CirculationToolStripMenuItem_MouseHover(sender As Object, e As EventArgs) Handles CirculationToolStripMenuItem.MouseHover
        Cursor = Cursors.Hand
    End Sub

    Private Sub CirculationToolStripMenuItem_MouseLeave(sender As Object, e As EventArgs) Handles CirculationToolStripMenuItem.MouseLeave
        Cursor = Cursors.Default
    End Sub

    Private Sub BorrowToolStripMenuItem_MouseLeave(sender As Object, e As EventArgs) Handles BorrowToolStripMenuItem.MouseLeave
        Cursor = Cursors.Default
    End Sub

    Private Sub ReturnToolStripMenuItem_MouseHover(sender As Object, e As EventArgs) Handles ReturnToolStripMenuItem.MouseHover
        Cursor = Cursors.Hand
    End Sub

    Private Sub ReturnToolStripMenuItem_MouseLeave(sender As Object, e As EventArgs) Handles ReturnToolStripMenuItem.MouseLeave
        Cursor = Cursors.Default
    End Sub

    Private Sub PenaltyToolStripMenuItem_MouseHover(sender As Object, e As EventArgs) Handles PenaltyToolStripMenuItem.MouseHover
        Cursor = Cursors.Hand
    End Sub

    Private Sub PenaltyToolStripMenuItem_MouseLeave(sender As Object, e As EventArgs) Handles PenaltyToolStripMenuItem.MouseLeave
        Cursor = Cursors.Default
    End Sub

    Private Sub Audit_Trail_MouseHover(sender As Object, e As EventArgs) Handles Audit_Trail.MouseHover
        Cursor = Cursors.Hand
    End Sub

    Private Sub Audit_Trail_MouseLeave(sender As Object, e As EventArgs) Handles Audit_Trail.MouseLeave
        Cursor = Cursors.Default
    End Sub

    Private Sub EditInfoToolStripMenuItem_MouseHover(sender As Object, e As EventArgs) Handles EditInfoToolStripMenuItem.MouseHover
        Cursor = Cursors.Hand
    End Sub

    Private Sub EditInfoToolStripMenuItem_MouseLeave(sender As Object, e As EventArgs) Handles EditInfoToolStripMenuItem.MouseLeave
        Cursor = Cursors.Default
    End Sub

    Private Sub btnlogoutt_MouseHover(sender As Object, e As EventArgs) Handles btnlogoutt.MouseHover
        Cursor = Cursors.Hand
    End Sub

    Private Sub btnlogoutt_MouseLeave(sender As Object, e As EventArgs) Handles btnlogoutt.MouseLeave
        Cursor = Cursors.Default
    End Sub

    Private Sub EditsToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles EditsToolStripMenuItem1.Click
        Borrowereditsinfo.ShowDialog()
    End Sub



    Public Sub lblborrowcount()
        Dim con As New MySqlConnection(GlobalVarsModule.connectionString)
        Dim count As Integer = 0

        Try
            con.Open()


            Dim query As String = "SELECT COUNT(AccessionID) FROM acession_tbl WHERE Status = 'Borrowed'"


            Using cmd As New MySqlCommand(query, con)
                Dim result As Object = cmd.ExecuteScalar()

                If result IsNot DBNull.Value AndAlso result IsNot Nothing Then
                    count = Convert.ToInt32(result)
                Else
                    count = 0
                End If
            End Using

            lbl_borrow.Text = count.ToString()

        Catch ex As Exception
            MessageBox.Show("Error counting currently borrowed books: " & ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
        End Try
    End Sub


    Private Sub PrintReceiptToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles PrintReceiptToolStripMenuItem.Click

        Panel_dash.Controls.Clear()

        With PrintReceiptForm

            .TopMost = True
            .TopLevel = False
            .BringToFront()
            Panel_dash.Controls.Add(PrintReceiptForm)

            .Show()


        End With

        lblform.Text = "PRINT RECEIPT FORM"

        For Each form In Application.OpenForms
            If TypeOf form Is PrintReceiptForm Then
                Dim lblload = DirectCast(form, PrintReceiptForm)
                lblload.refreshreceipt()
            End If
        Next

    End Sub

    Private Sub Guna2Button1_Click(sender As Object, e As EventArgs) Handles Guna2Button1.Click
        BorrowerLoginForm.Show()
        Me.Hide()
    End Sub

    Private Sub AttendanceRecordToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles AttendanceRecordToolStripMenuItem.Click
        Panel_dash.Controls.Clear()

        With TimeInOutRecord
            .TopLevel = False
            .TopMost = True
            .BringToFront()
            Panel_dash.Controls.Add(TimeInOutRecord)

            .Show()

            TimeInOutRecord.DataGridView1.ClearSelection()
            TimeInOutRecord.DataGridView1.CurrentCell = Nothing
            TimeInOutRecord.chkSelectAll.Checked = False
        End With

        lblform.Text = "TIME-IN/OUT RECORD FORM"
    End Sub



    Private Sub btnexit_Click(sender As Object, e As EventArgs) Handles btnexit.Click

        Dim isBorrowerTimedIn As Boolean = False
        Dim borrowerID As String = GlobalVarsModule.CurrentBorrowerID

        If GlobalVarsModule.CurrentUserRole = "Borrower" Then

            isBorrowerTimedIn = GlobalVarsModule.IsBorrowerStillTimedIn(borrowerID)
        End If

        Using exitForm As New StayLogoutFormm()

            exitForm.IsTimedIn = isBorrowerTimedIn

            Dim dialogResult = exitForm.ShowDialog()

            If dialogResult = DialogResult.Yes Then

                If GlobalVarsModule.CurrentUserRole = "Borrower" Then

                    Me.Hide()
                    BorrowerLoginForm.Show()
                Else
                    Return
                End If

            ElseIf dialogResult = DialogResult.Retry Then

                If GlobalVarsModule.CurrentUserRole = "Borrower" Then

                    TimeInToolStripMenuItem_Click(sender, e)
                End If
                Exit Sub

            ElseIf dialogResult = DialogResult.No Then

                btnlogoutt_Click(sender, e)

            ElseIf dialogResult = DialogResult.Cancel Then

                Return
            End If
        End Using

    End Sub

    Private Sub ReturnToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ReturnToolStripMenuItem.Click
        Returning.ShowDialog()
    End Sub

    Private Sub BorrowingHistoryToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles BorrowingHistoryToolStripMenuItem.Click

        Panel_dash.Controls.Clear()

        With BorrowingHistory
            .TopMost = True
            .TopLevel = False
            .BringToFront()
            Panel_dash.Controls.Add(BorrowingHistory)


            .Show()

            Acquisition.DataGridView1.ClearSelection()
            Acquisition.DataGridView1.CurrentCell = Nothing


        End With

        lblform.Text = "BORROWING HISTORY FORM"

        For Each form In Application.OpenForms
            If TypeOf form Is BorrowingHistory Then
                Dim load = DirectCast(form, BorrowingHistory)
                load.refreshhistory()
            End If
        Next

    End Sub

    Private Sub BorrowingConfirmationToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles BorrowingConfirmationToolStripMenuItem.Click
        BookBorrowingConfirmation.ShowDialog()
    End Sub
End Class