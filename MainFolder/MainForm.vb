Public Class MainForm


    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        Me.WindowState = FormWindowState.Maximized
        sizelocation()



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





        ElseIf Me.WindowState = FormWindowState.Normal Then

            Book.Size = New Size(1015, 391)
            Panel_dash.Size = New Size(1015, 391)

            lbl_borrow.Location = New Point(95, 39)
            lbl_return.Location = New Point(95, 39)
            lbl_overdue.Location = New Point(95, 39)
            lbl_damage.Location = New Point(95, 39)
            lbl_lost.Location = New Point(95, 39)




        End If
    End Sub


    Private Sub MaintenanceToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles MaintenanceToolStripMenuItem.Click

        MaintenanceToolStripMenuItem.ForeColor = Color.DarkGray

        Dim lumabasna As Boolean = False
        Book.Close()

        Panel_dash.Controls.Remove(Book)
        Panel_dash.Controls.Remove(Borrower)
        Panel_dash.Controls.Remove(Users_Staffs)
        Panel_dash.Controls.Remove(Acquisition)
        Panel_dash.Controls.Remove(Accession)
        Panel_User.Show()

        If lumabasna = False Then
            Panel_dash.Controls.Add(dshboard)
            Panel_dash.Controls.Add(Panel_User)
            lumabasna = True
        End If

    End Sub

    Private Sub AuthorMaintenanceToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles AuthorMaintenanceToolStripMenuItem.Click
        MaintenanceToolStripMenuItem.ForeColor = Color.White
        Author.ShowDialog()

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
        Panel_dash.Controls.Remove(Users_Staffs)
        Panel_dash.Controls.Remove(Acquisition)
        Panel_dash.Controls.Remove(Accession)

        If lumabasna = False Then
            Panel_dash.Controls.Add(dshboard)
            Panel_dash.Controls.Add(Panel_User)
            lumabasna = True
        End If

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
        Panel_dash.Controls.Remove(Users_Staffs)
        Panel_dash.Controls.Remove(Acquisition)
        Panel_dash.Controls.Remove(Accession)

        If lumabasna = False Then
            Panel_dash.Controls.Add(dshboard)
            Panel_dash.Controls.Add(Panel_User)
            lumabasna = True
        End If

    End Sub

    Private Sub ToolStripMenuItem1_DropDownClosed(sender As Object, e As EventArgs) Handles ProcessStripMenuItem.DropDownClosed
        ProcessStripMenuItem.ForeColor = Color.White
    End Sub

    Private Sub ToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles ProcessStripMenuItem.Click

        ProcessStripMenuItem.ForeColor = Color.DarkGray

        Dim lumabasna As Boolean = False
        Book.Close()

        Panel_dash.Controls.Remove(Book)
        Panel_dash.Controls.Remove(Borrower)
        Panel_dash.Controls.Remove(Users_Staffs)
        Panel_dash.Controls.Remove(Acquisition)
        Panel_dash.Controls.Remove(Accession)
        Panel_User.Show()

        If lumabasna = False Then
            Panel_dash.Controls.Add(dshboard)
            Panel_dash.Controls.Add(Panel_User)
            lumabasna = True
        End If

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

    Private Sub ToolStripMenuItem1_MouseHover(sender As Object, e As EventArgs) Handles ProcessStripMenuItem.MouseHover
        Me.Cursor = Cursors.Hand
    End Sub

    Private Sub ToolStripMenuItem1_MouseLeave(sender As Object, e As EventArgs) Handles ProcessStripMenuItem.MouseLeave
        Me.Cursor = Cursors.Default
    End Sub

    Private Sub logout_Click(sender As Object, e As EventArgs) Handles logout.Click

        Me.Close()
        login.Show()
    End Sub

    Private Sub btn_borrowed_MouseHover(sender As Object, e As EventArgs) Handles btn_borrowed.MouseHover
        Me.Cursor = Cursors.Hand

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
        Me.Cursor = Cursors.Hand

    End Sub

    Private Sub btn_borrowed_MouseLeave(sender As Object, e As EventArgs) Handles btn_borrowed.MouseLeave
        Me.Cursor = Cursors.Default
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
        Me.Cursor = Cursors.Default
    End Sub

    Private Sub GenreMaintenanceToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles GenreMaintenanceToolStripMenuItem.Click
        Genre.ShowDialog()
    End Sub

    Private Sub SectionToolStripMenuItem2_Click(sender As Object, e As EventArgs) Handles SectionToolStripMenuItem2.Click
        Section.ShowDialog()
    End Sub

    Private Sub SupplierMaintenanceToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SupplierMaintenanceToolStripMenuItem.Click
        Supplier.ShowDialog()
    End Sub

    Private Sub PublisherMaintenanceToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles PublisherMaintenanceToolStripMenuItem.Click
        Publisher.ShowDialog()
    End Sub

    Private Sub LanguageMaintenanceToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles LanguageToolStripMenuItem.Click
        Language.ShowDialog()
    End Sub

    Private Sub CategoryToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CategoryToolStripMenuItem.Click
        Category.ShowDialog()
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
        End With

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
        End With

    End Sub

    Private Sub UserMaintenanceToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles UserMaintenanceToolStripMenuItem.Click

        Panel_dash.Controls.Clear()

        With Users_Staffs
            .TopLevel = False
            .TopMost = True
            Panel_dash.Controls.Add(Users_Staffs)
            Users_Staffs.Size = New Size(1370, 700)
            .Show()
        End With

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

        End With

    End Sub

    Private Sub AccessionToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles AccessionToolStripMenuItem.Click

        Panel_dash.Controls.Clear()

        With Accession
            .TopMost = True
            .TopLevel = False
            .BringToFront()
            Panel_dash.Controls.Add(Accession)
            .Show()
        End With


    End Sub

    Private Sub TimeInToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles TimeInToolStripMenuItem.Click

        Panel_dash.Controls.Clear()




    End Sub
End Class
