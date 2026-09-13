Imports System.Drawing.Printing
Imports System.Drawing.Drawing2D
Imports System.Drawing.Text
Imports System.Drawing.Imaging
Imports System.Linq
Imports System.Threading.Tasks
Imports MySql.Data.MySqlClient
Imports ZXing
Imports ZXing.Common


Public Class LibraryCardPreview

    Private _imageToPrint As Image = Nothing

    Private WithEvents pd As New PrintDocument()


    ' ============================================================
    ' EXPOSE THE RENDERED CARD IMAGE
    '
    ' Lets other forms (e.g. reprintlibrarycard.vb) reuse this
    ' exact same rendered bitmap - same panel, same fonts, same
    ' colors, same barcode placement - so a reprinted card looks
    ' IDENTICAL to the one produced here, instead of being redrawn
    ' from scratch with different code/fonts elsewhere.
    ' ============================================================
    Public ReadOnly Property RenderedCardImage As Image
        Get
            Return _imageToPrint
        End Get
    End Property

    ' ============================================================
    ' PRINTING STATE
    ' ============================================================
    Private isPrinting As Boolean = False


    ' ============================================================
    ' LIBRARY CARD PHYSICAL SIZE
    '
    ' 100 mm x 140 mm
    ' ============================================================
    Private Const CARD_WIDTH_HUNDREDTH_INCH As Integer = 394
    Private Const CARD_HEIGHT_HUNDREDTH_INCH As Integer = 551


    ' ============================================================
    ' CARD POSITION
    '
    ' X MUST NOT BE CHANGED.
    '
    ' Negative X = LEFT
    ' Positive X = RIGHT
    '
    ' Negative Y = UP
    ' Positive Y = DOWN
    ' ============================================================
    Private Const CARD_OFFSET_X As Integer = -8
    Private Const CARD_OFFSET_Y As Integer = -325


    ' ============================================================
    ' BOTTOM BORDER PROTECTION
    ' ============================================================
    Private Const BOTTOM_BORDER_SAFETY As Integer = 8


    ' ============================================================
    ' LOAD LIBRARY CARD
    ' ============================================================
    Public Sub LoadPreview(
        borrowerType As String,
        img As Image,
        fullname As String,
        lrn As String,
        department As String,
        librarianName As String)

        Try

            ' ----------------------------------------------------
            ' CLEAN FULL NAME
            ' ----------------------------------------------------
            If Not String.IsNullOrWhiteSpace(fullname) Then

                Try

                    Dim parts =
                        fullname.Split(
                            New Char() {" "c},
                            StringSplitOptions.RemoveEmptyEntries)

                    parts =
                        parts.Where(
                            Function(s)

                                Return Not s.Equals(
                                    "N/A",
                                    StringComparison.OrdinalIgnoreCase)

                            End Function
                        ).ToArray()

                    fullname =
                        String.Join(" ", parts).Trim()

                Catch
                End Try

            End If


            ' ----------------------------------------------------
            ' DEFAULT VALUES
            ' ----------------------------------------------------
            fullname =
                If(
                    String.IsNullOrWhiteSpace(fullname),
                    "..",
                    fullname)

            lrn =
                If(
                    String.IsNullOrWhiteSpace(lrn),
                    "..",
                    lrn)

            department =
                If(
                    String.IsNullOrWhiteSpace(department),
                    "..",
                    department)

            librarianName =
                If(
                    String.IsNullOrWhiteSpace(librarianName),
                    "Librarian Name",
                    librarianName)


            ' ----------------------------------------------------
            ' SET LABELS
            ' ----------------------------------------------------
            lblfullname.Text = fullname
            lbllrnsu.Text = lrn
            lbldepartment.Text = department
            lbllabrian.Text = librarianName


            ' ----------------------------------------------------
            ' BORROWER PHOTO
            ' ----------------------------------------------------
            If img IsNot Nothing Then

                If PictureBox1.Image IsNot Nothing Then

                    Try
                        PictureBox1.Image.Dispose()
                    Catch
                    End Try

                End If

                PictureBox1.Image =
                    New Bitmap(img)

                PictureBox1.SizeMode =
                    PictureBoxSizeMode.StretchImage

            End If


            ' ----------------------------------------------------
            ' GENERATE BARCODE
            ' ----------------------------------------------------
            Try

                If PictureBox2.Image IsNot Nothing Then

                    Try
                        PictureBox2.Image.Dispose()
                    Catch
                    End Try

                End If


                Dim codeText As String =
                    If(
                        String.IsNullOrWhiteSpace(lrn) OrElse
                        lrn = "..",
                        "00000",
                        lrn)


                Dim writer As New ZXing.Windows.Compatibility.BarcodeWriter()

                writer.Format =
                    BarcodeFormat.CODE_128

                writer.Options =
                    New ZXing.Common.EncodingOptions With {
                        .Height = Math.Max(
                            40,
                            PictureBox2.Height),
                        .Width = Math.Max(
                            150,
                            PictureBox2.Width),
                        .PureBarcode = True
                    }


                Dim barBmp As Bitmap =
                    writer.Write(codeText)


                Try

                    Dim textFont As Font =
                        New Font(
                            SystemFonts.DefaultFont.FontFamily,
                            9,
                            FontStyle.Regular)


                    Dim textHeight As Integer =
                        CInt(
                            Math.Ceiling(
                                textFont.GetHeight()
                            )
                        ) + 4


                    Dim combined As New Bitmap(
                        Math.Max(
                            barBmp.Width,
                            120),
                        barBmp.Height +
                        textHeight)


                    Using g As Graphics =
                        Graphics.FromImage(combined)

                        g.Clear(Color.White)

                        g.InterpolationMode =
                            InterpolationMode.HighQualityBicubic

                        g.SmoothingMode =
                            SmoothingMode.HighQuality

                        g.PixelOffsetMode =
                            PixelOffsetMode.HighQuality


                        Dim bx As Integer =
                            (combined.Width -
                             barBmp.Width) \ 2


                        g.DrawImage(
                            barBmp,
                            bx,
                            0,
                            barBmp.Width,
                            barBmp.Height)


                        Dim txt As String =
                            codeText


                        Dim sf As New StringFormat() With {
                            .Alignment =
                                StringAlignment.Center,
                            .LineAlignment =
                                StringAlignment.Center
                        }


                        Dim txtRect As New RectangleF(
                            0,
                            barBmp.Height,
                            combined.Width,
                            textHeight)


                        Using br As New SolidBrush(
                            Color.Black)

                            g.DrawString(
                                txt,
                                textFont,
                                br,
                                txtRect,
                                sf)

                        End Using

                    End Using


                    PictureBox2.Image =
                        combined

                    PictureBox2.SizeMode =
                        PictureBoxSizeMode.StretchImage

                Finally

                    Try
                        barBmp.Dispose()
                    Catch
                    End Try

                End Try

            Catch

                ' Continue without barcode if generation fails.

            End Try


            ' ----------------------------------------------------
            ' PREPARE CARD IMAGE
            ' ----------------------------------------------------
            Try

                If _imageToPrint IsNot Nothing Then

                    Try
                        _imageToPrint.Dispose()
                    Catch
                    End Try

                    _imageToPrint = Nothing

                End If


                Guna2Panel1.Refresh()

                Application.DoEvents()


                _imageToPrint =
                    RenderPanelToBitmap(
                        Guna2Panel1)

            Catch

                _imageToPrint = Nothing

            End Try

        Catch

            ' Do not crash application from preview loading.

        End Try

    End Sub


    ' ============================================================
    ' DIRECT PRINT
    '
    ' NOTE:
    ' This is now an ASYNC SUB. It shows a "Printing..." dialog
    ' right before sending the job to the printer, and only
    ' shows the success message once Windows reports that the
    ' print job has actually left the print queue (i.e. the
    ' paper has physically finished printing), not just when it
    ' has been handed off to the spooler.
    ' ============================================================
    Public Async Sub PrintPreviewImage()

        ' ========================================================
        ' PREVENT DOUBLE PRINT
        ' ========================================================
        If isPrinting Then
            Return
        End If


        Dim printingStatusForm As Form = Nothing


        Try

            isPrinting = True


            ' ====================================================
            ' MAKE SURE CARD IMAGE EXISTS
            ' ====================================================
            If _imageToPrint Is Nothing Then

                Try

                    Guna2Panel1.Refresh()
                    Application.DoEvents()

                    _imageToPrint =
                    RenderPanelToBitmap(
                        Guna2Panel1)

                Catch

                    _imageToPrint = Nothing

                End Try

            End If


            If _imageToPrint Is Nothing Then

                MessageBox.Show(
                "Nothing to print.",
                "Print",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information)

                Return

            End If


            ' ====================================================
            ' CAPTURE LATEST CARD
            ' ====================================================
            Try

                If _imageToPrint IsNot Nothing Then

                    Try
                        _imageToPrint.Dispose()
                    Catch
                    End Try

                    _imageToPrint = Nothing

                End If


                Guna2Panel1.Refresh()
                Application.DoEvents()


                _imageToPrint =
                RenderPanelToBitmap(
                    Guna2Panel1)

            Catch

                _imageToPrint = Nothing

            End Try


            If _imageToPrint Is Nothing Then

                MessageBox.Show(
                "Unable to prepare the Library Card for printing.",
                "Print",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning)

                Return

            End If


            ' ====================================================
            ' CONFIGURE PRINT DOCUMENT
            ' ====================================================
            Try

                pd.DocumentName =
                "Library Card"

                pd.OriginAtMargins =
                False

                pd.DefaultPageSettings.Landscape =
                False

                pd.PrintController =
                New StandardPrintController()

            Catch
            End Try


            ' ====================================================
            ' WINDOWS PRINTER SELECTION
            '
            ' FIX:
            ' UseEXDialog = True calls the MODERN Windows print
            ' dialog, which runs in a SEPARATE OS process
            ' (PrintDialogHost). This is a known WinForms issue
            ' where the owner window (and sometimes the whole
            ' app) gets minimized / hidden behind other windows
            ' once that external process talks to the printer.
            '
            ' Switching to the CLASSIC print dialog
            ' (UseEXDialog = False) avoids that external host
            ' process entirely, so the owner window no longer
            ' gets hidden.
            ' ====================================================
            Using printDialog As New PrintDialog()

                printDialog.Document = pd

                printDialog.AllowPrintToFile = True

                printDialog.UseEXDialog = False


                ' =================================================
                ' SHOW PRINTER DIALOG
                ' =================================================
                Dim result As DialogResult =
                printDialog.ShowDialog(Me)


                ' =================================================
                ' USER CANCELLED
                ' =================================================
                If result <> DialogResult.OK Then
                    Return
                End If


                ' =================================================
                ' APPLY SELECTED PRINTER
                ' =================================================
                Try

                    pd.PrinterSettings =
                    printDialog.PrinterSettings

                Catch ex As Exception

                    MessageBox.Show(
                    "Unable to use the selected printer." &
                    Environment.NewLine &
                    Environment.NewLine &
                    ex.Message,
                    "Printer Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error)

                    Return

                End Try


                ' =================================================
                ' KEEP PORTRAIT
                ' =================================================
                Try

                    pd.DefaultPageSettings.Landscape =
                    False

                Catch
                End Try


                ' =================================================
                ' SHOW "PRINTING..." DIALOG
                '
                ' Shown right before the job is sent, so the user
                ' sees feedback immediately instead of a silent
                ' wait.
                ' =================================================
                Try

                    printingStatusForm =
                        CreatePrintingStatusForm()

                    printingStatusForm.Show(Me)

                    Application.DoEvents()

                Catch

                    printingStatusForm = Nothing

                End Try


                ' =================================================
                ' PRINT
                '
                ' NO MainForm.Hide()
                ' NO Me.Hide()
                ' NO MainForm.Show()
                ' NO Owner.Show()
                ' =================================================
                Try

                    pd.Print()

                Catch ex As Exception

                    CloseFormSafely(printingStatusForm)
                    printingStatusForm = Nothing

                    MessageBox.Show(
                    "The Library Card could not be printed." &
                    Environment.NewLine &
                    Environment.NewLine &
                    ex.Message,
                    "Print Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error)

                    Return

                End Try

            End Using


            ' ====================================================
            ' WAIT FOR THE PRINTER TO ACTUALLY FINISH
            '
            ' pd.Print() only means the document was handed to
            ' the spooler - NOT that the paper has come out yet.
            ' We poll the Windows print queue (WMI) until this
            ' job is no longer listed, which means the printer
            ' has finished physically printing it.
            ' ====================================================
            Dim printerNameForWait As String =
                pd.PrinterSettings.PrinterName

            Dim documentNameForWait As String =
                pd.DocumentName

            Await WaitForPrintJobCompletionAsync(
                printerNameForWait,
                documentNameForWait)


            CloseFormSafely(printingStatusForm)
            printingStatusForm = Nothing


            ' ====================================================
            ' SAVE TO reprint_tbl
            '
            ' Logged only AFTER the print job has actually
            ' finished, so we never save a record for a card
            ' that failed to print. A failure here does not
            ' block the success message - the card DID print;
            ' the user is just warned that the reprint log entry
            ' was not saved.
            ' ====================================================
            Try

                SaveReprintRecord()

            Catch
            End Try


            ' ====================================================
            ' SUCCESS MESSAGE
            '
            ' Only reached AFTER the print job has finished
            ' (paper is out of the printer).
            ' ====================================================
            MessageBox.Show(
            "Library Card successfully printed!",
            "Print Successful",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information)


        Catch ex As Exception

            CloseFormSafely(printingStatusForm)
            printingStatusForm = Nothing

            MessageBox.Show(
            "Print error: " &
            ex.Message,
            "Print Error",
            MessageBoxButtons.OK,
            MessageBoxIcon.Error)

        Finally

            ' ====================================================
            ' MAKE SURE THE PRINTING DIALOG NEVER GETS STUCK ON
            ' SCREEN, NO MATTER HOW THIS SUB EXITED.
            ' ====================================================
            CloseFormSafely(printingStatusForm)
            printingStatusForm = Nothing


            ' ====================================================
            ' RESET PRINTING STATE
            ' ====================================================
            isPrinting = False


            ' ====================================================
            ' FIX:
            ' RESTORE THIS FORM AND MAINFORM VISIBILITY.
            '
            ' Using BeginInvoke queues the restore AFTER the
            ' current message pump cycle, giving Windows time to
            ' finish whatever it was doing with the print/dialog
            ' process before we force the windows back to
            ' visible/foreground. Running this synchronously
            ' right away is what previously made the restore
            ' unreliable (race condition with the dialog process
            ' releasing the window).
            ' ====================================================
            Try

                RestoreFormsAfterPrint()

            Catch
            End Try

        End Try

    End Sub


    ' ============================================================
    ' SAVE REPRINT RECORD
    '
    ' Inserts the just-printed card's info into reprint_tbl so it
    ' can later be searched and reprinted from
    ' reprintlibrarycard.vb. Reads directly from the labels/photo
    ' that are already showing on this preview, so it always
    ' matches exactly what was printed.
    '
    ' FIX:
    ' The photo used to be saved as a raw, uncompressed PNG,
    ' which can easily be several megabytes. MySQL rejects the
    ' whole INSERT once the statement (photo included) is bigger
    ' than the server's max_allowed_packet setting, which is what
    ' caused "Packets larger than max_allowed_packet are not
    ' allowed." The photo is now resized and re-encoded as a
    ' compact JPEG (see CompressPhotoForStorage / EncodeAsJpeg
    ' below) before it is sent to the database, so it reliably
    ' fits and the record - photo included - actually gets saved.
    ' ============================================================
    Private Sub SaveReprintRecord()

        Try

            Dim nameVal As String =
                If(lblfullname IsNot Nothing, lblfullname.Text, "")

            Dim lrnVal As String =
                If(lbllrnsu IsNot Nothing, lbllrnsu.Text, "")

            Dim deptVal As String =
                If(lbldepartment IsNot Nothing, lbldepartment.Text, "")


            Dim photoBytes As Byte() = Nothing

            If PictureBox1.Image IsNot Nothing Then

                Try

                    photoBytes =
                        CompressPhotoForStorage(
                            PictureBox1.Image)

                Catch

                    photoBytes = Nothing

                End Try

            End If


            Using con As New MySqlConnection(connectionString)

                con.Open()

                Using cmd As New MySqlCommand(
                    "INSERT INTO reprint_tbl (Name, Lrn, Department, Photo) " &
                    "VALUES (@name, @lrn, @dept, @photo)",
                    con)

                    cmd.CommandTimeout = 60

                    cmd.Parameters.AddWithValue(
                        "@name", nameVal)

                    cmd.Parameters.AddWithValue(
                        "@lrn", lrnVal)

                    cmd.Parameters.AddWithValue(
                        "@dept", deptVal)

                    If photoBytes IsNot Nothing Then

                        cmd.Parameters.AddWithValue(
                            "@photo", photoBytes)

                    Else

                        cmd.Parameters.AddWithValue(
                            "@photo", DBNull.Value)

                    End If

                    cmd.ExecuteNonQuery()

                End Using

            End Using

        Catch ex As Exception

            Try

                MessageBox.Show(
                "The Library Card was printed, but saving the reprint record failed." &
                Environment.NewLine &
                Environment.NewLine &
                ex.Message,
                "Database Warning",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning)

            Catch
            End Try

        End Try

    End Sub


    ' ============================================================
    ' COMPRESS PHOTO BEFORE SAVING TO DATABASE
    '
    ' FIX FOR: "Packets larger than max_allowed_packet are not
    ' allowed."
    '
    ' The photo captured/uploaded can be several megabytes as a
    ' raw PNG. MySQL rejects the INSERT once the packet (the
    ' whole SQL statement, photo included) is bigger than the
    ' server's max_allowed_packet setting.
    '
    ' This resizes the photo down to a reasonable ID-photo size
    ' and re-encodes it as a JPEG at a modest quality, then keeps
    ' shrinking it further (smaller dimension, lower quality) if
    ' it is still too big, until it comfortably fits under
    ' maxBytes. This guarantees the INSERT will not fail on photo
    ' size again, no matter how large the original image is.
    ' ============================================================
    Private Function CompressPhotoForStorage(
        img As Image,
        Optional maxBytes As Integer = 500000) As Byte()

        If img Is Nothing Then
            Return Nothing
        End If

        Try

            Dim dimension As Integer = 500
            Dim quality As Long = 80L

            Dim result As Byte() = Nothing

            Dim attempts As Integer = 0

            While attempts < 8

                result =
                    EncodeAsJpeg(img, dimension, quality)

                If result Is Nothing Then
                    Exit While
                End If

                If result.Length <= maxBytes Then
                    Return result
                End If

                ' Still too big - shrink further and try again.
                If quality > 30L Then

                    quality -= 15L

                Else

                    dimension = CInt(dimension * 0.75)
                    quality = 60L

                End If

                If dimension < 80 Then
                    Exit While
                End If

                attempts += 1

            End While

            ' Return whatever was last produced, even if it is
            ' still above maxBytes - better than losing the photo
            ' entirely, and it is still far smaller than the
            ' original PNG.
            Return result

        Catch

            Return Nothing

        End Try

    End Function


    ' ============================================================
    ' RESIZE + JPEG-ENCODE AN IMAGE
    ' ============================================================
    Private Function EncodeAsJpeg(
        img As Image,
        maxDimension As Integer,
        quality As Long) As Byte()

        Try

            Dim srcW As Integer = img.Width
            Dim srcH As Integer = img.Height

            If srcW <= 0 OrElse srcH <= 0 Then
                Return Nothing
            End If

            Dim scale As Double =
                Math.Min(
                    1.0,
                    maxDimension / CDbl(Math.Max(srcW, srcH)))

            Dim newW As Integer =
                Math.Max(1, CInt(Math.Round(srcW * scale)))

            Dim newH As Integer =
                Math.Max(1, CInt(Math.Round(srcH * scale)))

            Using resized As New Bitmap(newW, newH)

                Using g As Graphics = Graphics.FromImage(resized)

                    g.InterpolationMode = InterpolationMode.HighQualityBicubic
                    g.SmoothingMode = SmoothingMode.HighQuality
                    g.PixelOffsetMode = PixelOffsetMode.HighQuality

                    g.DrawImage(img, 0, 0, newW, newH)

                End Using

                Dim jpegCodec As ImageCodecInfo =
                    ImageCodecInfo.GetImageEncoders().FirstOrDefault(
                        Function(c) c.FormatID = ImageFormat.Jpeg.Guid)

                Using ms As New IO.MemoryStream()

                    If jpegCodec IsNot Nothing Then

                        Using encParams As New EncoderParameters(1)

                            encParams.Param(0) =
                                New EncoderParameter(
                                    Encoder.Quality,
                                    quality)

                            resized.Save(ms, jpegCodec, encParams)

                        End Using

                    Else

                        resized.Save(ms, ImageFormat.Jpeg)

                    End If

                    Return ms.ToArray()

                End Using

            End Using

        Catch

            Return Nothing

        End Try

    End Function


    ' ============================================================
    ' CREATE "PRINTING..." STATUS DIALOG
    '
    ' Built entirely in code so no extra designer form needs to
    ' be added to the project. Modeless (Show, not ShowDialog),
    ' no close/minimize/maximize buttons so the user can't
    ' dismiss it early.
    ' ============================================================
    Private Function CreatePrintingStatusForm() As Form

        Dim frm As New Form()

        frm.Text = "Printing"
        frm.FormBorderStyle = FormBorderStyle.FixedDialog
        frm.ControlBox = False
        frm.MinimizeBox = False
        frm.MaximizeBox = False
        frm.ShowInTaskbar = False
        frm.StartPosition = FormStartPosition.CenterParent
        frm.Size = New Size(340, 140)
        frm.TopMost = True


        Dim lbl As New Label()

        lbl.Text =
            "Printing, please wait..." &
            Environment.NewLine &
            "Do not remove the paper or turn off the printer."

        lbl.TextAlign = ContentAlignment.MiddleCenter
        lbl.Dock = DockStyle.Top
        lbl.Height = 65
        lbl.Padding = New Padding(12, 10, 12, 0)


        Dim prog As New ProgressBar()

        prog.Style = ProgressBarStyle.Marquee
        prog.MarqueeAnimationSpeed = 30
        prog.Height = 22


        Dim progPanel As New Panel()

        progPanel.Dock = DockStyle.Fill
        progPanel.Padding = New Padding(15, 5, 15, 15)
        progPanel.Controls.Add(prog)

        prog.Dock = DockStyle.Bottom


        frm.Controls.Add(progPanel)
        frm.Controls.Add(lbl)

        Return frm

    End Function


    ' ============================================================
    ' SAFELY CLOSE AND DISPOSE A FORM
    ' ============================================================
    Private Sub CloseFormSafely(f As Form)

        Try

            If f IsNot Nothing AndAlso
               Not f.IsDisposed Then

                f.Close()
                f.Dispose()

            End If

        Catch
        End Try

    End Sub


    ' ============================================================
    ' WAIT UNTIL THE PRINT JOB HAS ACTUALLY FINISHED
    '
    ' pd.Print() returns as soon as the document is spooled -
    ' the printer may still be physically printing for several
    ' seconds after that. This polls the Windows print queue
    ' (Win32_PrintJob via WMI) for the given printer/document
    ' until the matching job is no longer listed, which means
    ' the printer is done with it.
    '
    ' Requires a reference to System.Management.dll
    ' (Project > Add Reference > Assemblies > System.Management).
    ' ============================================================
    Private Function WaitForPrintJobCompletionAsync(
        printerName As String,
        documentName As String) As Task(Of Boolean)

        Return Task.Run(
            Function() As Boolean

                Try

                    Dim scope As New Management.ManagementScope(
                        "\\.\root\cimv2")

                    scope.Connect()


                    Dim safePrinterName As String =
                        printerName.Replace(
                            "\", "\\").Replace(
                            "'", "''")


                    Const maxWaitMs As Integer = 45000
                    Const pollInterval As Integer = 500

                    Dim elapsed As Integer = 0
                    Dim jobSeenAtLeastOnce As Boolean = False
                    Dim notFoundStreak As Integer = 0


                    ' Give the spooler a moment to register the
                    ' job before the first check.
                    Threading.Thread.Sleep(500)


                    While elapsed < maxWaitMs

                        Dim hasMatchingJob As Boolean = False

                        Try

                            Dim query As New Management.ObjectQuery(
                                "SELECT Document FROM Win32_PrintJob WHERE Name LIKE '" &
                                safePrinterName &
                                ",%'")


                            Using searcher As New Management.ManagementObjectSearcher(
                                scope,
                                query)

                                Using results =
                                    searcher.Get()

                                    For Each mo As Management.ManagementObject In
                                        results

                                        Dim jobDocName As String = ""

                                        Try
                                            jobDocName =
                                                CStr(
                                                    mo("Document"))
                                        Catch
                                        End Try

                                        If Not String.IsNullOrEmpty(
                                            jobDocName) AndAlso
                                           jobDocName.Equals(
                                            documentName,
                                            StringComparison.OrdinalIgnoreCase) Then

                                            hasMatchingJob = True

                                        End If

                                        Try
                                            mo.Dispose()
                                        Catch
                                        End Try

                                    Next

                                End Using

                            End Using

                        Catch

                            ' WMI query failed (e.g. permissions,
                            ' service unavailable) - don't hang
                            ' the user, assume it's done.
                            Return True

                        End Try


                        If hasMatchingJob Then

                            jobSeenAtLeastOnce = True
                            notFoundStreak = 0

                        Else

                            If jobSeenAtLeastOnce Then

                                ' Job existed before and is now
                                ' gone from the queue - finished
                                ' printing.
                                Return True

                            Else

                                notFoundStreak += 1

                                ' Never saw the job after a few
                                ' checks - it most likely already
                                ' finished before we could catch
                                ' it (small/fast document).
                                If notFoundStreak >= 3 Then

                                    Return True

                                End If

                            End If

                        End If


                        Threading.Thread.Sleep(pollInterval)

                        elapsed += pollInterval

                    End While


                    ' Timeout reached - don't block the user
                    ' forever waiting on the queue.
                    Return True

                Catch

                    Return False

                End Try

            End Function)

    End Function


    ' ============================================================
    ' RESTORE FORM VISIBILITY AFTER PRINTING
    ' ============================================================
    Private Sub RestoreFormsAfterPrint()

        Dim restoreAction As Action =
            Sub()

                ' --------------------------------------------
                ' RESTORE THIS FORM (LibraryCardPreview)
                ' --------------------------------------------
                Try

                    If Not Me.IsDisposed Then

                        If Not Me.Visible Then
                            Me.Show()
                        End If

                        If Me.WindowState = FormWindowState.Minimized Then
                            Me.WindowState = FormWindowState.Normal
                        End If

                    End If

                Catch
                End Try


                ' --------------------------------------------
                ' RESTORE MAINFORM
                ' --------------------------------------------
                Try

                    Dim mf =
                        Application.OpenForms.OfType(Of MainForm)().FirstOrDefault()

                    If mf IsNot Nothing AndAlso Not mf.IsDisposed Then

                        If Not mf.Visible Then
                            mf.Show()
                        End If

                        If mf.WindowState = FormWindowState.Minimized Then
                            mf.WindowState = FormWindowState.Normal
                        End If

                        Try
                            mf.BringToFront()
                            mf.Activate()
                        Catch
                        End Try

                    End If

                Catch
                End Try


                ' --------------------------------------------
                ' BRING THIS FORM BACK ON TOP LAST
                ' SO THE PREVIEW STAYS THE ACTIVE WINDOW.
                ' --------------------------------------------
                Try

                    If Not Me.IsDisposed Then
                        Me.BringToFront()
                        Me.Activate()
                    End If

                Catch
                End Try

            End Sub


        If Me.IsHandleCreated Then

            ' Queue after current message cycle finishes.
            Me.BeginInvoke(restoreAction)

            ' Extra safety net: some printer/dialog hosts release
            ' the window a moment later, so re-check shortly after.
            Dim safetyTimer As New Timer()
            safetyTimer.Interval = 400

            AddHandler safetyTimer.Tick,
                Sub(s, e)
                    safetyTimer.Stop()
                    safetyTimer.Dispose()

                    Try
                        If Not Me.IsDisposed AndAlso Me.IsHandleCreated Then
                            Me.BeginInvoke(restoreAction)
                        End If
                    Catch
                    End Try
                End Sub

            safetyTimer.Start()

        Else

            restoreAction()

        End If

    End Sub


    ' ============================================================
    ' OPTIONAL CUSTOM CARD SIZE HELPER
    ' ============================================================
    Private Function CreateCardPaperSize() As PaperSize

        Return New PaperSize(
            "Library Card - 100 x 140 mm",
            CARD_WIDTH_HUNDREDTH_INCH,
            CARD_HEIGHT_HUNDREDTH_INCH)

    End Function


    ' ============================================================
    ' PRINT PAGE
    ' ============================================================
    Private Sub pd_PrintPage(
        sender As Object,
        e As PrintPageEventArgs) Handles pd.PrintPage

        Try

            e.HasMorePages = False


            If _imageToPrint Is Nothing Then

                Return

            End If


            ' ====================================================
            ' REQUESTED CARD SIZE
            ' ====================================================
            Dim requestedCardWidth As Integer =
                CARD_WIDTH_HUNDREDTH_INCH

            Dim requestedCardHeight As Integer =
                CARD_HEIGHT_HUNDREDTH_INCH


            ' ====================================================
            ' ACTUAL PAGE SIZE
            ' ====================================================
            Dim pageWidth As Integer =
                e.PageBounds.Width

            Dim pageHeight As Integer =
                e.PageBounds.Height


            If pageWidth <= 0 OrElse
               pageHeight <= 0 Then

                Return

            End If


            ' ====================================================
            ' PRINTER HARD MARGINS
            ' ====================================================
            Dim hardMarginX As Integer = 0
            Dim hardMarginY As Integer = 0


            Try

                hardMarginX =
                    CInt(
                        Math.Ceiling(
                            e.PageSettings.HardMarginX))

                hardMarginY =
                    CInt(
                        Math.Ceiling(
                            e.PageSettings.HardMarginY))

            Catch

                hardMarginX = 0
                hardMarginY = 0

            End Try


            ' ====================================================
            ' GENERAL SAFETY
            ' ====================================================
            Dim safety As Integer = 5


            ' ====================================================
            ' PRINTABLE AREA
            ' ====================================================
            Dim printableLeft As Integer =
                Math.Max(
                    0,
                    hardMarginX + safety)


            Dim printableTop As Integer =
                Math.Max(
                    0,
                    hardMarginY + safety)


            Dim printableRight As Integer =
                Math.Min(
                    pageWidth,
                    pageWidth -
                    hardMarginX -
                    safety)


            Dim printableBottom As Integer =
                Math.Min(
                    pageHeight,
                    pageHeight -
                    hardMarginY -
                    safety)


            ' ====================================================
            ' VALIDATE PRINTABLE AREA
            ' ====================================================
            If printableRight <= printableLeft OrElse
               printableBottom <= printableTop Then

                printableLeft = 0
                printableTop = 0
                printableRight = pageWidth
                printableBottom = pageHeight

            End If


            Dim printableWidth As Integer =
                printableRight -
                printableLeft


            Dim printableHeight As Integer =
                printableBottom -
                printableTop


            ' ====================================================
            ' CARD SIZE
            ' ====================================================
            Dim finalWidth As Integer =
                requestedCardWidth

            Dim finalHeight As Integer =
                requestedCardHeight


            ' ====================================================
            ' AUTO-SHRINK IF PAPER IS TOO SMALL
            ' ====================================================
            If finalWidth > printableWidth OrElse
               finalHeight > printableHeight Then

                Dim scaleX As Double =
                    printableWidth /
                    CDbl(finalWidth)


                Dim scaleY As Double =
                    printableHeight /
                    CDbl(finalHeight)


                Dim scale As Double =
                    Math.Min(
                        scaleX,
                        scaleY)


                finalWidth =
                    Math.Max(
                        1,
                        CInt(
                            Math.Floor(
                                finalWidth *
                                scale)))


                finalHeight =
                    Math.Max(
                        1,
                        CInt(
                            Math.Floor(
                                finalHeight *
                                scale)))

            End If


            ' ====================================================
            ' CENTER
            '
            ' X CENTERING IS LEFT AS BEFORE.
            ' ====================================================
            Dim drawX As Integer =
                printableLeft +
                CInt(
                    Math.Round(
                        (printableWidth -
                         finalWidth) /
                        2.0))


            Dim drawY As Integer =
                printableTop +
                CInt(
                    Math.Round(
                        (printableHeight -
                         finalHeight) /
                        2.0))


            ' ====================================================
            ' APPLY USER'S POSITION
            '
            ' X = -8
            ' Y = -325
            ' ====================================================
            drawX += CARD_OFFSET_X
            drawY += CARD_OFFSET_Y


            ' ====================================================
            ' BOTTOM BORDER FIX
            '
            ' IMPORTANT:
            '
            ' DO NOT CUT finalHeight.
            '
            ' The previous code reduced finalHeight directly.
            ' That could make the bottom portion appear cut or
            ' make the bottom border / fourth line disappear.
            '
            ' Instead, if the card reaches the bottom printable
            ' limit, SCALE THE WHOLE CARD proportionally.
            '
            ' This keeps:
            '   - all four lines
            '   - bottom border
            '   - top position
            '   - correct aspect ratio
            ' ====================================================
            Dim availableBottom As Integer =
                printableBottom -
                BOTTOM_BORDER_SAFETY


            If drawY + finalHeight >
               availableBottom Then

                Dim allowedHeight As Integer =
                    availableBottom -
                    drawY


                If allowedHeight > 0 AndAlso
                   allowedHeight < finalHeight Then

                    Dim scale As Double =
                        allowedHeight /
                        CDbl(finalHeight)


                    finalHeight =
                        Math.Max(
                            1,
                            CInt(
                                Math.Floor(
                                    finalHeight *
                                    scale)))


                    finalWidth =
                        Math.Max(
                            1,
                            CInt(
                                Math.Floor(
                                    finalWidth *
                                    scale)))

                End If

            End If


            ' ====================================================
            ' IMPORTANT:
            '
            ' DO NOT CHANGE X POSITION AFTER THIS.
            ' ====================================================
            If drawX < printableLeft Then

                drawX =
                    printableLeft

            End If


            If drawX + finalWidth >
               printableRight Then

                drawX =
                    printableRight -
                    finalWidth

            End If


            ' ====================================================
            ' Y TOP PROTECTION
            ' ====================================================
            If drawY < 0 Then

                drawY = 0

            End If


            ' ====================================================
            ' FINAL BOTTOM SAFETY
            '
            ' IMPORTANT:
            '
            ' WE DO NOT SHRINK finalHeight HERE.
            '
            ' If the proportional scaling above worked, the
            ' entire card already fits.
            '
            ' This fallback only handles an extremely unusual
            ' printer page condition.
            ' ====================================================
            If drawY + finalHeight >
               printableBottom Then

                Dim availableHeight As Integer =
                    printableBottom -
                    drawY


                If availableHeight > 0 Then

                    Dim scale As Double =
                        availableHeight /
                        CDbl(finalHeight)


                    finalHeight =
                        Math.Max(
                            1,
                            CInt(
                                Math.Floor(
                                    finalHeight *
                                    scale)))


                    finalWidth =
                        Math.Max(
                            1,
                            CInt(
                                Math.Floor(
                                    finalWidth *
                                    scale)))

                Else

                    Return

                End If

            End If


            If finalHeight < 1 OrElse
               finalWidth < 1 Then

                Return

            End If


            ' ====================================================
            ' HIGH QUALITY PRINTING
            ' ====================================================
            e.Graphics.InterpolationMode =
                InterpolationMode.HighQualityBicubic

            e.Graphics.SmoothingMode =
                SmoothingMode.HighQuality

            e.Graphics.PixelOffsetMode =
                PixelOffsetMode.HighQuality

            e.Graphics.CompositingQuality =
                CompositingQuality.HighQuality

            e.Graphics.TextRenderingHint =
                TextRenderingHint.ClearTypeGridFit


            ' ====================================================
            ' DESTINATION RECTANGLE
            ' ====================================================
            Dim destinationRect As New Rectangle(
                drawX,
                drawY,
                finalWidth,
                finalHeight)


            ' ====================================================
            ' WHITE BACKGROUND
            ' ====================================================
            Using whiteBrush As New SolidBrush(
                Color.White)

                e.Graphics.FillRectangle(
                    whiteBrush,
                    destinationRect)

            End Using


            ' ====================================================
            ' SOURCE RECTANGLE
            ' ====================================================
            Dim sourceRect As New Rectangle(
                0,
                0,
                _imageToPrint.Width,
                _imageToPrint.Height)


            ' ====================================================
            ' DRAW ENTIRE CARD
            '
            ' ENTIRE IMAGE IS DRAWN.
            '
            ' No bottom cropping.
            ' ====================================================
            e.Graphics.DrawImage(
                _imageToPrint,
                destinationRect,
                sourceRect,
                GraphicsUnit.Pixel)


            ' ====================================================
            ' INNER BORDER
            '
            ' Draw border INSIDE the card.
            ' ====================================================
            Dim borderInset As Integer = 3


            If finalWidth >
               (borderInset * 2 + 4) AndAlso
               finalHeight >
               (borderInset * 2 + 4) Then


                Dim borderRect As New Rectangle(
                    drawX + borderInset,
                    drawY + borderInset,
                    finalWidth -
                    (borderInset * 2) -
                    1,
                    finalHeight -
                    (borderInset * 2) -
                    1)


                Using borderPen As New Pen(
                    Color.Black,
                    1.0F)

                    borderPen.Alignment =
                        PenAlignment.Inset

                    e.Graphics.DrawRectangle(
                        borderPen,
                        borderRect)

                End Using

            End If


            ' ====================================================
            ' ONE PAGE ONLY
            ' ====================================================
            e.HasMorePages = False


        Catch ex As Exception

            e.HasMorePages = False

            MessageBox.Show(
                "Unable to print the Library Card." &
                Environment.NewLine &
                Environment.NewLine &
                ex.Message,
                "Print Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error)

        End Try

    End Sub


    ' ============================================================
    ' RENDER PANEL TO BITMAP
    ' ============================================================
    Private Function RenderPanelToBitmap(
        p As Control) As Bitmap

        If p Is Nothing Then

            Return Nothing

        End If


        ' ========================================================
        ' METHOD 1
        '
        ' Capture actual displayed panel.
        ' ========================================================
        Try

            If p.IsHandleCreated AndAlso
               p.Visible AndAlso
               p.Width > 0 AndAlso
               p.Height > 0 Then


                Dim bmpScreen As New Bitmap(
                    Math.Max(
                        1,
                        p.Width),
                    Math.Max(
                        1,
                        p.Height))


                Dim screenPos As Point =
                    p.PointToScreen(
                        Point.Empty)


                Using g As Graphics =
                    Graphics.FromImage(
                        bmpScreen)

                    g.CopyFromScreen(
                        screenPos,
                        New Point(0, 0),
                        p.Size,
                        CopyPixelOperation.SourceCopy)

                End Using


                Return bmpScreen

            End If

        Catch

        End Try


        ' ========================================================
        ' METHOD 2
        '
        ' Draw controls directly to bitmap.
        ' ========================================================
        Try

            Dim bmp As New Bitmap(
                Math.Max(
                    1,
                    p.Width),
                Math.Max(
                    1,
                    p.Height))


            Using g As Graphics =
                Graphics.FromImage(bmp)


                g.InterpolationMode =
                    InterpolationMode.HighQualityBicubic

                g.SmoothingMode =
                    SmoothingMode.AntiAlias

                g.PixelOffsetMode =
                    PixelOffsetMode.HighQuality


                ' ------------------------------------------------
                ' BACKGROUND
                ' ------------------------------------------------
                Using b As New SolidBrush(
                    p.BackColor)

                    g.FillRectangle(
                        b,
                        0,
                        0,
                        p.Width,
                        p.Height)

                End Using


                ' ------------------------------------------------
                ' RECURSIVE DRAW
                ' ------------------------------------------------
                Dim SubDraw As Action(
                    Of Control,
                    Integer,
                    Integer) = Nothing


                SubDraw =
                    Sub(
                        ctrl As Control,
                        offsetX As Integer,
                        offsetY As Integer)


                        If Not ctrl.Visible Then

                            Return

                        End If


                        Dim rect As New Rectangle(
                            offsetX + ctrl.Left,
                            offsetY + ctrl.Top,
                            ctrl.Width,
                            ctrl.Height)


                        ' ----------------------------------------
                        ' TRY DrawToBitmap
                        ' ----------------------------------------
                        Try

                            If ctrl.Width > 0 AndAlso
                               ctrl.Height > 0 Then


                                Using tb As New Bitmap(
                                    Math.Max(
                                        1,
                                        ctrl.Width),
                                    Math.Max(
                                        1,
                                        ctrl.Height))


                                    ctrl.DrawToBitmap(
                                        tb,
                                        New Rectangle(
                                            0,
                                            0,
                                            tb.Width,
                                            tb.Height))


                                    g.DrawImage(
                                        tb,
                                        rect)


                                    Return

                                End Using

                            End If

                        Catch

                        End Try


                        ' ----------------------------------------
                        ' TRY IMAGE PROPERTY
                        ' ----------------------------------------
                        Try

                            Dim imgProp =
                                ctrl.GetType().GetProperty(
                                    "Image")


                            If imgProp IsNot Nothing Then


                                Dim imgObj =
                                    imgProp.GetValue(
                                        ctrl)


                                If TypeOf imgObj Is Image Then


                                    Dim img As Image =
                                        CType(
                                            imgObj,
                                            Image)


                                    If img IsNot Nothing Then


                                        g.DrawImage(
                                            img,
                                            rect)


                                        Return

                                    End If

                                End If

                            End If

                        Catch

                        End Try


                        ' ----------------------------------------
                        ' TRY TEXT PROPERTY
                        ' ----------------------------------------
                        Try

                            Dim textProp =
                                ctrl.GetType().GetProperty(
                                    "Text")


                            If textProp IsNot Nothing Then


                                Dim txt As String =
                                    CStr(
                                        textProp.GetValue(
                                            ctrl))


                                If Not String.IsNullOrEmpty(
                                    txt) Then


                                    Dim f As Font =
                                        ctrl.Font


                                    Dim fc As Color =
                                        ctrl.ForeColor


                                    Dim format As New StringFormat()


                                    format.Alignment =
                                        StringAlignment.Near

                                    format.LineAlignment =
                                        StringAlignment.Near


                                    Using br As New SolidBrush(
                                        fc)

                                        g.DrawString(
                                            txt,
                                            f,
                                            br,
                                            rect,
                                            format)

                                    End Using

                                End If

                            End If

                        Catch

                        End Try


                        ' ----------------------------------------
                        ' DRAW CHILD CONTROLS
                        ' ----------------------------------------
                        For Each ch As Control In
                            ctrl.Controls

                            SubDraw(
                                ch,
                                rect.Left,
                                rect.Top)

                        Next

                    End Sub


                ' ------------------------------------------------
                ' DRAW PANEL CHILDREN
                ' ------------------------------------------------
                For Each child As Control In
                    p.Controls

                    SubDraw(
                        child,
                        0,
                        0)

                Next

            End Using


            Return bmp


        Catch

        End Try


        Return Nothing

    End Function


    ' ============================================================
    ' FORM LOAD
    ' ============================================================
    Private Sub LibraryCardPreview_Load(
        sender As Object,
        e As EventArgs) Handles MyBase.Load

        ' Intentionally empty.
        '
        ' NO Me.Hide()
        ' NO Me.Close()
        ' NO Application.Exit()

    End Sub

End Class