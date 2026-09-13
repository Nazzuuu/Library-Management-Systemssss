Imports System.Data
Imports System.Drawing.Printing
Imports System.Drawing.Drawing2D
Imports System.Drawing.Text
Imports System.Drawing.Imaging
Imports System.Threading.Tasks
Imports MySql.Data.MySqlClient
Imports ZXing
Imports ZXing.Common


Public Class reprintlibrarycard

    ' ============================================================
    ' PRINTING STATE
    ' ============================================================
    Private isPrinting As Boolean = False

    Private WithEvents pd As New PrintDocument()


    ' ============================================================
    ' LIBRARY CARD PHYSICAL SIZE
    '
    ' Same physical card size / printer offsets as
    ' LibraryCardPreview.vb, since reprints go on the same card
    ' stock through the same printer. If the original ever gets
    ' re-tuned, mirror the same CARD_OFFSET_X / CARD_OFFSET_Y
    ' values here.
    ' ============================================================
    Private Const CARD_WIDTH_HUNDREDTH_INCH As Integer = 394
    Private Const CARD_HEIGHT_HUNDREDTH_INCH As Integer = 551

    Private Const CARD_OFFSET_X As Integer = -8
    Private Const CARD_OFFSET_Y As Integer = -325

    Private Const BOTTOM_BORDER_SAFETY As Integer = 8


    ' ============================================================
    ' DATA FOR THE CARD CURRENTLY BEING PRINTED
    ' ============================================================
    Private _printName As String = ""
    Private _printLrn As String = ""
    Private _printDepartment As String = ""
    Private _printLibrarianName As String = ""
    Private _printPhoto As Image = Nothing

    ' ============================================================
    ' FINAL COMPOSED CARD IMAGE
    '
    ' FIX:
    ' Instead of redrawing the card from scratch here with plain
    ' Arial fonts and hand-rolled layout math (which looked
    ' different from the actual LibraryCardPreview.vb card), this
    ' now holds the SAME rendered bitmap that LibraryCardPreview
    ' produces - same panel, same fonts/colors, same barcode. See
    ' BuildCardImageFromPreview() below.
    ' ============================================================
    Private _printCardImage As Image = Nothing


    ' ============================================================
    ' FORM LOAD
    ' ============================================================
    Private Sub reprintlibrarycard_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        Try

            ConfigureGrid()

            LoadSearchResults("")

        Catch
        End Try

    End Sub


    ' ============================================================
    ' CLEAN UP ON CLOSE
    ' ============================================================
    Private Sub reprintlibrarycard_FormClosed(sender As Object, e As FormClosedEventArgs) Handles Me.FormClosed

        Try
            If _printPhoto IsNot Nothing Then
                _printPhoto.Dispose()
            End If
        Catch
        End Try

        Try
            If _printCardImage IsNot Nothing Then
                _printCardImage.Dispose()
            End If
        Catch
        End Try

    End Sub


    ' ============================================================
    ' SEARCH BOX
    ' ============================================================
    Private Sub txtsearch_TextChanged(sender As Object, e As EventArgs) Handles txtsearch.TextChanged

        Try

            LoadSearchResults(txtsearch.Text.Trim())

        Catch
        End Try

    End Sub


    ' ============================================================
    ' LOAD SEARCH RESULTS INTO THE GRID
    ' ============================================================
    Private Sub LoadSearchResults(searchTerm As String)

        Try

            Using con As New MySqlConnection(connectionString)

                con.Open()

                Dim cmd As MySqlCommand

                If String.IsNullOrWhiteSpace(searchTerm) Then

                    cmd = New MySqlCommand(
                        "SELECT ID, Name, Lrn, Department FROM reprint_tbl ORDER BY Name ASC",
                        con)

                Else

                    cmd = New MySqlCommand(
                        "SELECT ID, Name, Lrn, Department FROM reprint_tbl " &
                        "WHERE Name LIKE @search ORDER BY Name ASC",
                        con)

                    cmd.Parameters.AddWithValue(
                        "@search",
                        "%" & searchTerm & "%")

                End If

                Using adap As New MySqlDataAdapter(cmd)

                    Dim dt As New DataTable()

                    adap.Fill(dt)

                    DataGridView1.DataSource = dt

                End Using

            End Using

            FormatGrid()

        Catch ex As Exception

            MessageBox.Show(
            "Unable to load records." &
            Environment.NewLine &
            Environment.NewLine &
            ex.Message,
            "Database Error",
            MessageBoxButtons.OK,
            MessageBoxIcon.Error)

        End Try

    End Sub


    ' ============================================================
    ' GRID APPEARANCE / BEHAVIOR
    ' ============================================================
    Private Sub ConfigureGrid()

        Try

            DataGridView1.ReadOnly = True
            DataGridView1.AllowUserToAddRows = False
            DataGridView1.AllowUserToDeleteRows = False
            DataGridView1.MultiSelect = False
            DataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect
            DataGridView1.RowHeadersVisible = False
            DataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill

        Catch
        End Try

    End Sub


    Private Sub FormatGrid()

        Try

            If DataGridView1.Columns.Contains("ID") Then
                DataGridView1.Columns("ID").Visible = False
            End If

            If DataGridView1.Columns.Contains("Name") Then
                DataGridView1.Columns("Name").HeaderText = "Full Name"
            End If

            If DataGridView1.Columns.Contains("Lrn") Then
                DataGridView1.Columns("Lrn").HeaderText = "LRN / Employee No."
            End If

            If DataGridView1.Columns.Contains("Department") Then
                DataGridView1.Columns("Department").HeaderText = "Department"
            End If

        Catch
        End Try

    End Sub


    ' ============================================================
    ' REPRINT BUTTON
    ' ============================================================
    Private Sub btnreprint_Click(sender As Object, e As EventArgs) Handles btnreprint.Click

        Try

            If DataGridView1.CurrentRow Is Nothing OrElse
               DataGridView1.SelectedRows.Count = 0 Then

                MessageBox.Show(
                "Please select a record to reprint.",
                "Reprint Library Card",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information)

                Return

            End If

            Dim selectedId As Integer =
                Convert.ToInt32(
                    DataGridView1.SelectedRows(0).Cells("ID").Value)

            PrintSelectedCardAsync(selectedId)

        Catch ex As Exception

            MessageBox.Show(
            "Error: " & ex.Message,
            "Reprint Error",
            MessageBoxButtons.OK,
            MessageBoxIcon.Error)

        End Try

    End Sub


    ' ============================================================
    ' FETCH RECORD, BUILD CARD DATA, AND PRINT
    ' ============================================================
    Private Async Sub PrintSelectedCardAsync(recordId As Integer)

        If isPrinting Then
            Return
        End If

        Dim printingStatusForm As Form = Nothing

        Try

            isPrinting = True
            btnreprint.Enabled = False


            ' ================================================
            ' FETCH RECORD FROM DATABASE
            ' ================================================
            Dim fetchedName As String = ""
            Dim fetchedLrn As String = ""
            Dim fetchedDepartment As String = ""
            Dim photoBytes As Byte() = Nothing
            Dim found As Boolean = False

            Using con As New MySqlConnection(connectionString)

                con.Open()

                Using cmd As New MySqlCommand(
                    "SELECT Name, Lrn, Department, Photo FROM reprint_tbl WHERE ID = @id",
                    con)

                    cmd.Parameters.AddWithValue("@id", recordId)

                    Using rdr = cmd.ExecuteReader()

                        If rdr.Read() Then

                            found = True

                            fetchedName =
                                If(rdr("Name") Is DBNull.Value, "", rdr("Name").ToString())

                            fetchedLrn =
                                If(rdr("Lrn") Is DBNull.Value, "", rdr("Lrn").ToString())

                            fetchedDepartment =
                                If(rdr("Department") Is DBNull.Value, "", rdr("Department").ToString())

                            If Not (rdr("Photo") Is DBNull.Value) Then
                                photoBytes = CType(rdr("Photo"), Byte())
                            End If

                        End If

                    End Using

                End Using

            End Using

            If Not found Then

                MessageBox.Show(
                "Record not found. It may have been removed.",
                "Reprint",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning)

                Return

            End If


            ' ================================================
            ' PREPARE CARD DATA
            ' ================================================
            _printName =
                If(String.IsNullOrWhiteSpace(fetchedName), "..", fetchedName)

            _printLrn =
                If(String.IsNullOrWhiteSpace(fetchedLrn), "..", fetchedLrn)

            _printDepartment =
                If(String.IsNullOrWhiteSpace(fetchedDepartment), "..", fetchedDepartment)

            _printLibrarianName =
                If(String.IsNullOrWhiteSpace(GlobalFullname), "Librarian Name", GlobalFullname)


            If _printPhoto IsNot Nothing Then
                Try
                    _printPhoto.Dispose()
                Catch
                End Try
                _printPhoto = Nothing
            End If

            If photoBytes IsNot Nothing AndAlso photoBytes.Length > 0 Then

                Try

                    Using ms As New IO.MemoryStream(photoBytes)

                        Using tempImg As Image = Image.FromStream(ms)
                            _printPhoto = New Bitmap(tempImg)
                        End Using

                    End Using

                Catch

                    _printPhoto = Nothing

                End Try

            End If


            ' ================================================
            ' BUILD THE CARD IMAGE
            '
            ' FIX: build it using the EXACT SAME rendering as
            ' LibraryCardPreview.vb (same panel, fonts, colors,
            ' barcode) so the reprinted card looks identical to
            ' the original, instead of the old hand-drawn version
            ' with plain Arial text that looked different.
            ' ================================================
            If _printCardImage IsNot Nothing Then
                Try
                    _printCardImage.Dispose()
                Catch
                End Try
                _printCardImage = Nothing
            End If

            _printCardImage = BuildCardImageFromPreview()

            If _printCardImage Is Nothing Then

                MessageBox.Show(
                "Unable to prepare the Library Card for printing.",
                "Reprint",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning)

                Return

            End If


            ' ================================================
            ' CONFIGURE PRINT DOCUMENT
            ' ================================================
            Try

                pd.DocumentName = "Library Card Reprint"
                pd.OriginAtMargins = False
                pd.DefaultPageSettings.Landscape = False
                pd.PrintController = New StandardPrintController()

            Catch
            End Try


            ' ================================================
            ' PRINTER SELECTION
            ' ================================================
            Using printDialog As New PrintDialog()

                printDialog.Document = pd
                printDialog.AllowPrintToFile = True
                printDialog.UseEXDialog = False

                Dim result As DialogResult =
                    printDialog.ShowDialog(Me)

                If result <> DialogResult.OK Then
                    Return
                End If

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

                Try
                    pd.DefaultPageSettings.Landscape = False
                Catch
                End Try


                Try

                    printingStatusForm = CreatePrintingStatusForm()
                    printingStatusForm.Show(Me)
                    Application.DoEvents()

                Catch

                    printingStatusForm = Nothing

                End Try


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


            ' ================================================
            ' WAIT FOR THE PRINTER TO ACTUALLY FINISH
            ' ================================================
            Dim printerNameForWait As String =
                pd.PrinterSettings.PrinterName

            Dim documentNameForWait As String =
                pd.DocumentName

            Await WaitForPrintJobCompletionAsync(
                printerNameForWait,
                documentNameForWait)

            CloseFormSafely(printingStatusForm)
            printingStatusForm = Nothing


            MessageBox.Show(
            "Library Card successfully reprinted!",
            "Print Successful",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information)


        Catch ex As Exception

            CloseFormSafely(printingStatusForm)
            printingStatusForm = Nothing

            MessageBox.Show(
            "Print error: " & ex.Message,
            "Print Error",
            MessageBoxButtons.OK,
            MessageBoxIcon.Error)

        Finally

            CloseFormSafely(printingStatusForm)
            printingStatusForm = Nothing

            isPrinting = False
            btnreprint.Enabled = True

        End Try

    End Sub


    ' ============================================================
    ' BUILD THE CARD IMAGE USING LibraryCardPreview'S OWN RENDERER
    '
    ' This spins up a LibraryCardPreview instance in the
    ' background, feeds it the SAME data (photo, name, LRN,
    ' department, librarian name) through its own LoadPreview()
    ' method - the exact same method used for the original card -
    ' and grabs the exact same rendered bitmap it produces
    ' (RenderedCardImage). That bitmap already contains the photo,
    ' text, and barcode laid out and styled exactly like the
    ' original LibraryCardPreview card, so printing it here
    ' guarantees an identical result.
    '
    ' NOTE: the helper form is briefly shown (so its controls
    ' actually get laid out/painted) and closed right away - this
    ' can cause a very quick flash on screen while reprinting.
    ' That is expected and is what makes the render 100% accurate;
    ' it closes again immediately after the image is captured.
    ' ============================================================
    Private Function BuildCardImageFromPreview() As Image

        Dim previewForm As LibraryCardPreview = Nothing
        Dim capturedImage As Image = Nothing

        Try

            previewForm = New LibraryCardPreview()

            previewForm.ShowInTaskbar = False
            previewForm.FormBorderStyle = FormBorderStyle.None
            previewForm.StartPosition = FormStartPosition.Manual
            previewForm.Location = New Point(0, 0)

            previewForm.Show()
            previewForm.Refresh()

            Application.DoEvents()

            previewForm.LoadPreview(
                "",
                _printPhoto,
                _printName,
                _printLrn,
                _printDepartment,
                _printLibrarianName)

            Application.DoEvents()

            Dim rendered As Image = previewForm.RenderedCardImage

            If rendered IsNot Nothing Then
                capturedImage = New Bitmap(rendered)
            End If

        Catch

            capturedImage = Nothing

        Finally

            Try

                If previewForm IsNot Nothing AndAlso Not previewForm.IsDisposed Then
                    previewForm.Close()
                    previewForm.Dispose()
                End If

            Catch
            End Try

        End Try

        Return capturedImage

    End Function


    ' ============================================================
    ' PRINT PAGE - DRAWS THE SAME CARD IMAGE PRODUCED BY
    ' LibraryCardPreview.vb (VIA BuildCardImageFromPreview ABOVE)
    ' ============================================================
    Private Sub pd_PrintPage(
        sender As Object,
        e As PrintPageEventArgs) Handles pd.PrintPage

        Try

            e.HasMorePages = False


            ' ============================================
            ' PAGE / PRINTABLE AREA (same safety logic as
            ' LibraryCardPreview.vb)
            ' ============================================
            Dim pageWidth As Integer = e.PageBounds.Width
            Dim pageHeight As Integer = e.PageBounds.Height

            If pageWidth <= 0 OrElse pageHeight <= 0 Then
                Return
            End If

            Dim hardMarginX As Integer = 0
            Dim hardMarginY As Integer = 0

            Try
                hardMarginX = CInt(Math.Ceiling(e.PageSettings.HardMarginX))
                hardMarginY = CInt(Math.Ceiling(e.PageSettings.HardMarginY))
            Catch
                hardMarginX = 0
                hardMarginY = 0
            End Try

            Dim safety As Integer = 5

            Dim printableLeft As Integer =
                Math.Max(0, hardMarginX + safety)

            Dim printableTop As Integer =
                Math.Max(0, hardMarginY + safety)

            Dim printableRight As Integer =
                Math.Min(pageWidth, pageWidth - hardMarginX - safety)

            Dim printableBottom As Integer =
                Math.Min(pageHeight, pageHeight - hardMarginY - safety)

            If printableRight <= printableLeft OrElse
               printableBottom <= printableTop Then

                printableLeft = 0
                printableTop = 0
                printableRight = pageWidth
                printableBottom = pageHeight

            End If

            Dim printableWidth As Integer = printableRight - printableLeft
            Dim printableHeight As Integer = printableBottom - printableTop


            ' ============================================
            ' CARD SIZE / AUTO-SHRINK
            ' ============================================
            Dim finalWidth As Integer = CARD_WIDTH_HUNDREDTH_INCH
            Dim finalHeight As Integer = CARD_HEIGHT_HUNDREDTH_INCH

            If finalWidth > printableWidth OrElse
               finalHeight > printableHeight Then

                Dim scaleX As Double = printableWidth / CDbl(finalWidth)
                Dim scaleY As Double = printableHeight / CDbl(finalHeight)
                Dim scale As Double = Math.Min(scaleX, scaleY)

                finalWidth = Math.Max(1, CInt(Math.Floor(finalWidth * scale)))
                finalHeight = Math.Max(1, CInt(Math.Floor(finalHeight * scale)))

            End If


            ' ============================================
            ' POSITION
            ' ============================================
            Dim drawX As Integer =
                printableLeft +
                CInt(Math.Round((printableWidth - finalWidth) / 2.0))

            Dim drawY As Integer =
                printableTop +
                CInt(Math.Round((printableHeight - finalHeight) / 2.0))

            drawX += CARD_OFFSET_X
            drawY += CARD_OFFSET_Y

            Dim availableBottom As Integer =
                printableBottom - BOTTOM_BORDER_SAFETY

            If drawY + finalHeight > availableBottom Then

                Dim allowedHeight As Integer = availableBottom - drawY

                If allowedHeight > 0 AndAlso allowedHeight < finalHeight Then

                    Dim scale As Double = allowedHeight / CDbl(finalHeight)

                    finalHeight = Math.Max(1, CInt(Math.Floor(finalHeight * scale)))
                    finalWidth = Math.Max(1, CInt(Math.Floor(finalWidth * scale)))

                End If

            End If

            If drawX < printableLeft Then
                drawX = printableLeft
            End If

            If drawX + finalWidth > printableRight Then
                drawX = printableRight - finalWidth
            End If

            If drawY < 0 Then
                drawY = 0
            End If

            If drawY + finalHeight > printableBottom Then

                Dim availableHeight As Integer = printableBottom - drawY

                If availableHeight > 0 Then

                    Dim scale As Double = availableHeight / CDbl(finalHeight)

                    finalHeight = Math.Max(1, CInt(Math.Floor(finalHeight * scale)))
                    finalWidth = Math.Max(1, CInt(Math.Floor(finalWidth * scale)))

                Else

                    Return

                End If

            End If

            If finalHeight < 1 OrElse finalWidth < 1 Then
                Return
            End If


            ' ============================================
            ' HIGH QUALITY RENDERING
            ' ============================================
            e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic
            e.Graphics.SmoothingMode = SmoothingMode.HighQuality
            e.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality
            e.Graphics.CompositingQuality = CompositingQuality.HighQuality
            e.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit

            Dim destinationRect As New Rectangle(drawX, drawY, finalWidth, finalHeight)


            ' ============================================
            ' WHITE BACKGROUND
            ' ============================================
            Using whiteBrush As New SolidBrush(Color.White)
                e.Graphics.FillRectangle(whiteBrush, destinationRect)
            End Using


            ' ============================================
            ' DRAW THE CARD
            '
            ' This is the SAME bitmap LibraryCardPreview.vb
            ' renders (photo + name + LRN + department +
            ' barcode + librarian name, all in the original
            ' styling) - drawn here exactly like
            ' LibraryCardPreview.vb draws it during a direct
            ' print, so the reprint looks identical.
            ' ============================================
            If _printCardImage IsNot Nothing Then

                Dim sourceRect As New Rectangle(
                    0,
                    0,
                    _printCardImage.Width,
                    _printCardImage.Height)

                e.Graphics.DrawImage(
                    _printCardImage,
                    destinationRect,
                    sourceRect,
                    GraphicsUnit.Pixel)

            End If


            ' ============================================
            ' INNER BORDER
            ' ============================================
            Dim borderInset As Integer = 3

            If finalWidth > (borderInset * 2 + 4) AndAlso
               finalHeight > (borderInset * 2 + 4) Then

                Dim borderRect As New Rectangle(
                    drawX + borderInset,
                    drawY + borderInset,
                    finalWidth - (borderInset * 2) - 1,
                    finalHeight - (borderInset * 2) - 1)

                Using borderPen As New Pen(Color.Black, 1.0F)
                    borderPen.Alignment = PenAlignment.Inset
                    e.Graphics.DrawRectangle(borderPen, borderRect)
                End Using

            End If


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
    ' CREATE "PRINTING..." STATUS DIALOG
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

            If f IsNot Nothing AndAlso Not f.IsDisposed Then
                f.Close()
                f.Dispose()
            End If

        Catch
        End Try

    End Sub


    ' ============================================================
    ' WAIT UNTIL THE PRINT JOB HAS ACTUALLY FINISHED
    '
    ' Same WMI polling approach as LibraryCardPreview.vb.
    ' Requires a reference to System.Management.dll.
    ' ============================================================
    Private Function WaitForPrintJobCompletionAsync(
        printerName As String,
        documentName As String) As Task(Of Boolean)

        Return Task.Run(
            Function() As Boolean

                Try

                    Dim scope As New Management.ManagementScope("\\.\root\cimv2")
                    scope.Connect()

                    Dim safePrinterName As String =
                        printerName.Replace("\", "\\").Replace("'", "''")

                    Const maxWaitMs As Integer = 45000
                    Const pollInterval As Integer = 500

                    Dim elapsed As Integer = 0
                    Dim jobSeenAtLeastOnce As Boolean = False
                    Dim notFoundStreak As Integer = 0

                    Threading.Thread.Sleep(500)

                    While elapsed < maxWaitMs

                        Dim hasMatchingJob As Boolean = False

                        Try

                            Dim query As New Management.ObjectQuery(
                                "SELECT Document FROM Win32_PrintJob WHERE Name LIKE '" &
                                safePrinterName & ",%'")

                            Using searcher As New Management.ManagementObjectSearcher(scope, query)

                                Using results = searcher.Get()

                                    For Each mo As Management.ManagementObject In results

                                        Dim jobDocName As String = ""

                                        Try
                                            jobDocName = CStr(mo("Document"))
                                        Catch
                                        End Try

                                        If Not String.IsNullOrEmpty(jobDocName) AndAlso
                                           jobDocName.Equals(documentName, StringComparison.OrdinalIgnoreCase) Then

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

                            Return True

                        End Try

                        If hasMatchingJob Then

                            jobSeenAtLeastOnce = True
                            notFoundStreak = 0

                        Else

                            If jobSeenAtLeastOnce Then

                                Return True

                            Else

                                notFoundStreak += 1

                                If notFoundStreak >= 3 Then
                                    Return True
                                End If

                            End If

                        End If

                        Threading.Thread.Sleep(pollInterval)
                        elapsed += pollInterval

                    End While

                    Return True

                Catch

                    Return False

                End Try

            End Function)

    End Function

End Class