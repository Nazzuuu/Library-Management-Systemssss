Imports AForge.Video
Imports AForge.Video.DirectShow

Public Class LibraryCardss

    Private Sub Guna2Panel2_Paint(sender As Object, e As PaintEventArgs) Handles Guna2Panel2.Paint

    End Sub

    Private Sub btnstart_Click(sender As Object, e As EventArgs) Handles btnstart.Click

        userRequestedStart = True
        Try
            ' disable start button immediately to prevent multiple clicks
            Try
                btnstart.Enabled = False
            Catch
            End Try
            If videoDevices Is Nothing Then
                videoDevices = New FilterInfoCollection(FilterCategory.VideoInputDevice)
            End If

            If videoDevices Is Nothing OrElse videoDevices.Count = 0 Then
                userRequestedStart = False

                Try
                    btnstart.Enabled = False
                Catch
                End Try
                MessageBox.Show("No webcam detected. Please connect a webcam before starting.", "Camera Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If

            StartCamera()
        Catch ex As Exception
            userRequestedStart = False
            MessageBox.Show("Error while attempting to start camera: " & ex.Message, "Camera Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Try
                btnstart.Enabled = True
            Catch
            End Try
        End Try
    End Sub

    Private Sub btncapture_Click(sender As Object, e As EventArgs) Handles btncapture.Click

        Try
            If Not lbllink.Visible OrElse String.IsNullOrWhiteSpace(lblname.Text) OrElse lblname.Text = ".." Then
                MessageBox.Show("Please select a borrower first before capturing.", "Select Borrower", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If
        Catch
        End Try

        CapturePhoto()
    End Sub

    Private Sub btnretake_Click(sender As Object, e As EventArgs) Handles btnretake.Click
        RetakePhoto()
    End Sub

    Private Sub btnselect_Click(sender As Object, e As EventArgs) Handles btnselect.Click
        OpenChooseBorrower()
    End Sub

    Private Sub lbllink_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles lbllink.LinkClicked
        If PictureBox2.Image IsNot Nothing Then
            Try
                Dim borrowerType As String = "Student"

                Dim identifier As String = ".."
                Try
                    identifier = If(Not String.IsNullOrWhiteSpace(lblrn.Text) AndAlso lblrn.Text <> "..", lblrn.Text, If(Not String.IsNullOrWhiteSpace(selectedEmployeeNo) AndAlso selectedEmployeeNo <> "..", selectedEmployeeNo, ".."))
                Catch
                End Try

                Dim librarianName As String = GlobalVarsModule.GlobalFullname

                Dim frm As New LibraryCardPreview()
                Try
                    frm.LoadPreview(borrowerType, PictureBox2.Image, lblname.Text, identifier, lbldepartment.Text, librarianName)

                    ' hide this form then show preview (preview is modeless)
                    Try
                        Me.Hide()
                    Catch
                    End Try

                    AddHandler frm.FormClosed, Sub(s As Object, ev As FormClosedEventArgs)
                                                   Try
                                                       Me.Show()
                                                   Catch
                                                   End Try
                                                   ' ensure camera controls are restored when returning from preview
                                                   Try
                                                       ' if camera is not running, attempt to restart
                                                       If Not isCameraRunning Then
                                                           Try
                                                               Dim devs As FilterInfoCollection = New FilterInfoCollection(FilterCategory.VideoInputDevice)
                                                               If devs IsNot Nothing AndAlso devs.Count > 0 Then
                                                                   userRequestedStart = True
                                                                   StartCamera()
                                                               End If
                                                           Catch
                                                           End Try
                                                       End If

                                                       ' explicitly update control states
                                                       Try
                                                           btnstart.Enabled = Not isCameraRunning
                                                           btncapture.Enabled = isCameraRunning
                                                           btnretake.Enabled = (PictureBox2.Image IsNot Nothing)
                                                           btnselect.Enabled = True
                                                           lbllink.Enabled = True
                                                       Catch
                                                       End Try
                                                   Catch
                                                   End Try
                                               End Sub

                    frm.Show()

                    ' show custom dialog positioned toward left side of preview form
                    Dim dr As DialogResult = PreviewOptionsForm.ShowForOwner(frm, "Print card or Close preview?")

                    If dr = DialogResult.Yes Then
                        Try
                            frm.PrintPreviewImage()
                        Catch ex As Exception
                            MessageBox.Show("Print failed: " & ex.Message, "Print Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                        End Try
                    Else
                        Try
                            frm.Close()
                        Catch
                        End Try
                    End If

                Catch ex As Exception
                    Try
                        frm.Dispose()
                    Catch
                    End Try
                    MessageBox.Show("Unable to open preview: " & ex.Message, "Preview Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End Try

            Catch ex As Exception
                MessageBox.Show("Unable to open preview: " & ex.Message, "Preview Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End If
    End Sub



    Private videoDevices As FilterInfoCollection = Nothing
    Private videoSource As VideoCaptureDevice = Nothing

    Private isCameraRunning As Boolean = False

    Private watcherTimer As Timer = Nothing
    Private knownDeviceCount As Integer = -1

    Private userRequestedStart As Boolean = False

    Private noDeviceForm As Form = Nothing
    Private suppressNoDeviceForm As Boolean = False
    Private selectedEmployeeNo As String = ".."

    Private Sub SetCameraControlsEnabled(enabled As Boolean)
        Try
            ' combobox removed; always allow select button when controls enabled
            btnselect.Enabled = enabled
            ' start button should be disabled while camera is running
            btnstart.Enabled = enabled AndAlso Not isCameraRunning
            btncapture.Enabled = enabled AndAlso isCameraRunning
            btnretake.Enabled = enabled AndAlso (PictureBox2.Image IsNot Nothing)
            lbllink.Enabled = enabled
        Catch
        End Try
    End Sub



    Private Sub StartCamera()

        Try

            videoDevices = New FilterInfoCollection(FilterCategory.VideoInputDevice)

            If videoDevices Is Nothing OrElse videoDevices.Count = 0 Then
                isCameraRunning = False
                ShowNoDeviceForm()
                Return
            End If



            If videoSource IsNot Nothing AndAlso videoSource.IsRunning Then
                isCameraRunning = True
                CloseNoDeviceForm()
                Return
            End If


            Dim idx As Integer = GetPreferredDeviceIndex()

            videoSource = New VideoCaptureDevice(videoDevices(idx).MonikerString)

            AddHandler videoSource.NewFrame, AddressOf Video_NewFrame
            AddHandler videoSource.VideoSourceError, AddressOf VideoSource_Error
            AddHandler videoSource.PlayingFinished, AddressOf Video_PlayingFinished

            videoSource.Start()

            isCameraRunning = True
            userRequestedStart = True

            knownDeviceCount = videoDevices.Count

            SetCameraControlsEnabled(True)

            CloseNoDeviceForm()

        Catch ex As Exception

            isCameraRunning = False


            SetCameraControlsEnabled(False)
            ShowNoDeviceForm()

        End Try

    End Sub


    Private Sub VideoSource_Error(sender As Object, e As AForge.Video.VideoSourceErrorEventArgs)

        Try

            userRequestedStart = True

            If Me.IsDisposed OrElse Me.Disposing Then Return

            Me.BeginInvoke(New Action(
                Sub()

                    Try

                        isCameraRunning = False
                        SetCameraControlsEnabled(False)
                        ShowNoDeviceForm()

                    Catch
                    End Try

                    Try
                        For Each f As Form In Application.OpenForms
                            If TypeOf f Is MainForm Then
                                Dim mf = DirectCast(f, MainForm)
                                Try
                                    mf.ProcessStripMenuItem.ShowDropDown()
                                    mf.ProcessStripMenuItem.ForeColor = Color.Gray
                                Catch
                                End Try
                                Exit For
                            End If
                        Next
                    Catch
                    End Try

                End Sub))

        Catch
        End Try

    End Sub

    Private Sub Video_PlayingFinished(
        sender As Object,
        reason As AForge.Video.ReasonToFinishPlaying)

        Try

            If Me.IsDisposed OrElse Me.Disposing Then Return

            If reason = AForge.Video.ReasonToFinishPlaying.DeviceLost Then

                userRequestedStart = True

                Me.BeginInvoke(New Action(
                        Sub()

                            Try

                                isCameraRunning = False
                                SetCameraControlsEnabled(False)
                                ShowNoDeviceForm()
                            Catch
                            End Try

                        End Sub))

            End If

        Catch
        End Try

    End Sub


    Private Sub WatcherTimer_Tick(sender As Object, e As EventArgs)

        Try

            Dim devs As FilterInfoCollection =
                New FilterInfoCollection(FilterCategory.VideoInputDevice)

            Dim count As Integer = 0

            If devs IsNot Nothing Then
                count = devs.Count
            End If

            If count = 0 Then

                knownDeviceCount = 0

                If isCameraRunning Then
                    StopCamera()
                End If


                SetCameraControlsEnabled(False)

                If noDeviceForm Is Nothing OrElse noDeviceForm.IsDisposed Then
                    ShowNoDeviceForm()
                End If

                Return

            End If


            If count > 0 Then

                knownDeviceCount = count



                If Not isCameraRunning AndAlso userRequestedStart Then

                    StartCamera()

                ElseIf isCameraRunning Then

                    CloseNoDeviceForm()

                End If

            End If

        Catch
        End Try

    End Sub




    Private Sub ShowNoDeviceForm()

        Try

            If Me.IsDisposed OrElse Me.Disposing Then Return


            If noDeviceForm IsNot Nothing AndAlso
               Not noDeviceForm.IsDisposed Then

                Return

            End If


            suppressNoDeviceForm = False

            noDeviceForm = New Form()

            With noDeviceForm

                .StartPosition = FormStartPosition.CenterParent
                .FormBorderStyle = FormBorderStyle.FixedDialog

                .ControlBox = False

                .MinimizeBox = False
                .MaximizeBox = False

                .ShowInTaskbar = False
                .TopMost = True

                .Size = New Size(420, 150)

                .Text = "Camera Disconnected"

            End With


            Dim lbl As New Label()

            With lbl

                .Dock = DockStyle.Fill

                .TextAlign = ContentAlignment.MiddleCenter

                .Font = New Font(
                    "Segoe UI",
                    9,
                    FontStyle.Regular)

                .Text =
                    "Camera disconnected or not detected." &
                    vbCrLf &
                    vbCrLf &
                    "Please plug in a webcam." &
                    vbCrLf &
                    "The camera will start automatically when available."

            End With


            noDeviceForm.Controls.Add(lbl)

            AddHandler noDeviceForm.FormClosed,
                Sub(s As Object, ev As FormClosedEventArgs)

                    Try

                        noDeviceForm = Nothing

                        If suppressNoDeviceForm Then
                            Return
                        End If


                        Dim devices As FilterInfoCollection =
                            New FilterInfoCollection(
                                FilterCategory.VideoInputDevice)

                        Dim count As Integer = 0

                        If devices IsNot Nothing Then
                            count = devices.Count
                        End If


                        If count = 0 Then

                            Dim restartTimer As New Timer()

                            restartTimer.Interval = 500

                            AddHandler restartTimer.Tick,
                                Sub(ts As Object, te As EventArgs)

                                    Try

                                        restartTimer.Stop()
                                        restartTimer.Dispose()

                                        Dim currentDevices As FilterInfoCollection =
                                            New FilterInfoCollection(
                                                FilterCategory.VideoInputDevice)

                                        Dim currentCount As Integer = 0

                                        If currentDevices IsNot Nothing Then
                                            currentCount = currentDevices.Count
                                        End If


                                        If currentCount = 0 Then

                                            ShowNoDeviceForm()

                                        Else

                                            StartCamera()

                                        End If

                                    Catch
                                    End Try

                                End Sub

                            restartTimer.Start()

                        Else

                            StartCamera()

                        End If

                    Catch
                    End Try

                End Sub


            ' Use non-blocking show so main form remains responsive
            noDeviceForm.Show(Me)

        Catch
        End Try

    End Sub


    Private Sub CloseNoDeviceForm()

        Try

            If noDeviceForm Is Nothing Then Return

            If noDeviceForm.IsDisposed Then

                noDeviceForm = Nothing
                Return

            End If


            suppressNoDeviceForm = True

            If noDeviceForm.InvokeRequired Then

                noDeviceForm.Invoke(
                    New Action(
                        Sub()

                            Try
                                noDeviceForm.Close()
                            Catch
                            End Try

                        End Sub))

            Else

                noDeviceForm.Close()

            End If


            noDeviceForm = Nothing

        Catch
        End Try

    End Sub


    Private Function GetPreferredDeviceIndex() As Integer

        If videoDevices Is Nothing OrElse
           videoDevices.Count = 0 Then

            Return 0

        End If


        Dim keywords As String() = {
            "USB",
            "EXTERNAL",
            "LOGITECH",
            "HD",
            "WEBCAM",
            "HIKVISION",
            "VID",
            "UVC"
        }


        For i As Integer = 0 To videoDevices.Count - 1

            Dim name As String =
                videoDevices(i).Name.ToUpperInvariant()


            For Each k As String In keywords

                If name.Contains(k.ToUpperInvariant()) Then

                    Return i

                End If

            Next

        Next

        If videoDevices.Count > 1 Then

            Return videoDevices.Count - 1

        End If


        Return 0

    End Function



    Private Sub StopCamera()

        Try

            If videoSource IsNot Nothing Then

                Try

                    If videoSource.IsRunning Then

                        RemoveHandler videoSource.NewFrame,
                            AddressOf Video_NewFrame

                        RemoveHandler videoSource.VideoSourceError,
                            AddressOf VideoSource_Error

                        RemoveHandler videoSource.PlayingFinished,
                            AddressOf Video_PlayingFinished


                        videoSource.SignalToStop()

                        videoSource.WaitForStop()

                    End If

                Catch
                End Try


                videoSource = Nothing

            End If

        Catch

        Finally

            isCameraRunning = False

            btnstart.Enabled = True
            btncapture.Enabled = False

            btnretake.Enabled =
                (PictureBox2.Image IsNot Nothing)

        End Try

    End Sub


    Private Sub Video_NewFrame(
        sender As Object,
        eventArgs As NewFrameEventArgs)

        Try

            Dim frame As Bitmap =
                CType(eventArgs.Frame.Clone(), Bitmap)


            If PictureBox1.IsDisposed Then

                frame.Dispose()
                Return

            End If


            If PictureBox1.InvokeRequired Then

                PictureBox1.BeginInvoke(
                    New Action(
                        Sub()

                            Try

                                If PictureBox1.Image IsNot Nothing Then

                                    Dim oldImage As Image =
                                        PictureBox1.Image

                                    PictureBox1.Image = Nothing

                                    oldImage.Dispose()

                                End If


                                PictureBox1.Image = frame

                            Catch

                                frame.Dispose()

                            End Try

                        End Sub))

            Else

                If PictureBox1.Image IsNot Nothing Then

                    Dim oldImage As Image =
                        PictureBox1.Image

                    PictureBox1.Image = Nothing

                    oldImage.Dispose()

                End If


                PictureBox1.Image = frame

            End If

        Catch
        End Try

    End Sub


    Private Sub CapturePhoto()

        Try

            If PictureBox1.Image Is Nothing Then

                MessageBox.Show(
                    "No live image to capture.",
                    "Capture",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning)

                Return

            End If


            Dim bmp As New Bitmap(PictureBox1.Image)

            If PictureBox2.Image IsNot Nothing Then

                Dim oldImage As Image =
                    PictureBox2.Image

                PictureBox2.Image = Nothing

                oldImage.Dispose()

            End If

            PictureBox2.Image = bmp

            ' disable capture to prevent spamming until retake
            Try
                btncapture.Enabled = False
            Catch
            End Try

            ' enable retake
            Try
                btnretake.Enabled = True
            Catch
            End Try

        Catch ex As Exception

            MessageBox.Show(
                "Error capturing photo: " & ex.Message,
                "Capture Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error)

        End Try

    End Sub



    Private Sub RetakePhoto()

        Try

            If PictureBox2.Image IsNot Nothing Then

                Dim oldImage As Image =
                    PictureBox2.Image

                PictureBox2.Image = Nothing

                oldImage.Dispose()

            End If

            If Not isCameraRunning Then

                Dim devices As FilterInfoCollection =
                    New FilterInfoCollection(
                        FilterCategory.VideoInputDevice)

                If devices IsNot Nothing AndAlso
                   devices.Count > 0 Then

                    userRequestedStart = True
                    StartCamera()

                End If

            End If

            ' enable capture button on retake
            Try
                btncapture.Enabled = True
            Catch
            End Try

            ' ensure controls are updated
            SetCameraControlsEnabled(True)

        Catch ex As Exception

            MessageBox.Show(
                "Error preparing camera: " & ex.Message,
                "Retake Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning)

        End Try

    End Sub



    Private Sub OpenChooseBorrower()

        ' Always load students only
        Dim borrowerType As String = "Student"

        Using frm As New ChooseBrwr()
            frm.LoadBorrowers(borrowerType)

            If frm.ShowDialog() = DialogResult.OK Then
                lbllink.Visible = True

                lblrn.Text = If(String.IsNullOrWhiteSpace(frm.SelectedLRN), "..", frm.SelectedLRN)
                ' store selected employee no for use when previewing identifier (not shown on student card)
                selectedEmployeeNo = If(String.IsNullOrWhiteSpace(frm.SelectedEmployeeNo), "..", frm.SelectedEmployeeNo)
                lbldepartment.Text = If(String.IsNullOrWhiteSpace(frm.SelectedDepartment), "..", frm.SelectedDepartment)
                lblgrade.Text = If(String.IsNullOrWhiteSpace(frm.SelectedGrade), "..", frm.SelectedGrade)
                lblsection.Text = If(String.IsNullOrWhiteSpace(frm.SelectedSection), "..", frm.SelectedSection)
                lblstrand.Text = If(String.IsNullOrWhiteSpace(frm.SelectedStrand), "..", frm.SelectedStrand)

                Dim fullname As String = String.Join(" ", New String() {frm.SelectedFirstName, frm.SelectedMiddleInitial, frm.SelectedLastName}.Where(Function(s) Not String.IsNullOrWhiteSpace(s))).Trim()
                If String.IsNullOrWhiteSpace(fullname) Then fullname = ".."
                lblname.Text = fullname

                ' show student fields
                lblgrades.Visible = True
                lblgrade.Visible = True
                lblsection.Visible = True
                lblstrand.Visible = True
                Label3.Visible = True
                Label4.Visible = True
            End If
        End Using

    End Sub




    Private Sub LibraryCardss_Load(
        sender As Object,
        e As EventArgs) Handles MyBase.Load


        btnselect.Enabled = True

        lbllink.Visible = False

        ' combobox removed; select button will open ChooseBrwr for Students by default


        btncapture.Enabled = False
        btnretake.Enabled = False


        PictureBox1.SizeMode = PictureBoxSizeMode.StretchImage
        PictureBox2.SizeMode = PictureBoxSizeMode.StretchImage



        If watcherTimer Is Nothing Then

            watcherTimer = New Timer()

            watcherTimer.Interval = 1000

            AddHandler watcherTimer.Tick,
                AddressOf WatcherTimer_Tick

            watcherTimer.Start()

        End If


        userRequestedStart = True


        Try

            Dim devs As FilterInfoCollection =
                New FilterInfoCollection(
                    FilterCategory.VideoInputDevice)

            Dim devCount As Integer = 0

            If devs IsNot Nothing Then
                devCount = devs.Count
            End If


            knownDeviceCount = devCount


            If devCount = 0 Then


                ShowNoDeviceForm()

            Else

                StartCamera()

            End If

        Catch

            ShowNoDeviceForm()

        End Try

    End Sub


    Private Sub App_ThreadException(
        sender As Object,
        e As Threading.ThreadExceptionEventArgs)

        Try

            userRequestedStart = True

            StopCamera()

            ShowNoDeviceForm()

        Catch
        End Try

    End Sub


    Private Sub AppDomain_UnhandledException(
        sender As Object,
        e As UnhandledExceptionEventArgs)

        Try

            userRequestedStart = True

            StopCamera()

            If Not Me.IsDisposed AndAlso
               Not Me.Disposing Then

                Me.BeginInvoke(
                    New Action(
                        Sub()

                            Try

                                ShowNoDeviceForm()

                            Catch
                            End Try

                        End Sub))

            End If

        Catch
        End Try

    End Sub




    Private Sub LibraryCardss_FormClosing(
        sender As Object,
        e As FormClosingEventArgs) Handles MyBase.FormClosing

        Try

            If watcherTimer IsNot Nothing Then

                watcherTimer.Stop()

                watcherTimer.Dispose()

                watcherTimer = Nothing

            End If


            suppressNoDeviceForm = True

            CloseNoDeviceForm()


            If videoSource IsNot Nothing Then

                StopCamera()

            End If


        Catch
        End Try


        Try
            Dim mf As MainForm = Nothing

            If GlobalVarsModule.ActiveMainForm IsNot Nothing AndAlso Not GlobalVarsModule.ActiveMainForm.IsDisposed Then
                mf = GlobalVarsModule.ActiveMainForm
            Else
                mf = Application.OpenForms.OfType(Of MainForm)().FirstOrDefault()
            End If

            If mf IsNot Nothing Then
                Try
                    If mf.InvokeRequired Then
                        mf.BeginInvoke(New Action(Sub() mf.ProcessStripMenuItem.PerformClick()))
                    Else
                        mf.ProcessStripMenuItem.PerformClick()
                        mf.ProcessStripMenuItem.HideDropDown()
                    End If
                Catch

                End Try
            End If
        Catch
        End Try
    End Sub

    Private Sub lblreprint_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles lblreprint.LinkClicked
        reprintlibrarycard.ShowDialog()
    End Sub

End Class