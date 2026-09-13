Imports System.Drawing.Printing
Imports ZXing
Imports ZXing.Common
Imports PdfSharp.Pdf
Imports PdfSharp.Drawing
Imports System.Diagnostics
Imports System.IO

Public Class LibraryCardPreview

    Private _imageToPrint As Image = Nothing
    Private WithEvents pd As New PrintDocument()

    Public Sub LoadPreview(borrowerType As String, img As Image, fullname As String, lrn As String, department As String, librarianName As String)
        Try

            If Not String.IsNullOrWhiteSpace(fullname) Then
                Try
                    Dim parts = fullname.Split(New Char() {" "c}, StringSplitOptions.RemoveEmptyEntries)
                    parts = parts.Where(Function(s) Not s.Equals("N/A", StringComparison.OrdinalIgnoreCase)).ToArray()
                    fullname = String.Join(" ", parts).Trim()
                Catch
                End Try
            End If

            fullname = If(String.IsNullOrWhiteSpace(fullname), "..", fullname)
            lrn = If(String.IsNullOrWhiteSpace(lrn), "..", lrn)
            department = If(String.IsNullOrWhiteSpace(department), "..", department)
            librarianName = If(String.IsNullOrWhiteSpace(librarianName), "Librarian Name", librarianName)

            lblfullname.Text = fullname
            lbllrnsu.Text = lrn
            lbldepartment.Text = department
            lbllabrian.Text = librarianName

            If img IsNot Nothing Then
                If PictureBox1.Image IsNot Nothing Then
                    Try
                        PictureBox1.Image.Dispose()
                    Catch
                    End Try
                End If

                PictureBox1.Image = New Bitmap(img)
                PictureBox1.SizeMode = PictureBoxSizeMode.StretchImage
            End If

            Try
                If PictureBox2.Image IsNot Nothing Then
                    Try
                        PictureBox2.Image.Dispose()
                    Catch
                    End Try
                End If

                Dim codeText As String = If(String.IsNullOrWhiteSpace(lrn) OrElse lrn = "..", "00000", lrn)
                Dim writer As New ZXing.Windows.Compatibility.BarcodeWriter()
                writer.Format = BarcodeFormat.CODE_128
                writer.Options = New ZXing.Common.EncodingOptions With {
                    .Height = Math.Max(40, PictureBox2.Height),
                    .Width = Math.Max(150, PictureBox2.Width),
                    .PureBarcode = True
                }
                Dim barBmp As Bitmap = writer.Write(codeText)


                Try
                    Dim textFont As Font = New Font(SystemFonts.DefaultFont.FontFamily, 9, FontStyle.Regular)
                    Dim textHeight As Integer = CInt(Math.Ceiling(textFont.GetHeight())) + 4
                    Dim combined As New Bitmap(Math.Max(barBmp.Width, 120), barBmp.Height + textHeight)
                    Using g As Graphics = Graphics.FromImage(combined)
                        g.Clear(Color.White)

                        Dim bx As Integer = (combined.Width - barBmp.Width) \ 2
                        g.DrawImage(barBmp, bx, 0, barBmp.Width, barBmp.Height)

                        Dim txt As String = codeText
                        Dim sf As New StringFormat() With {
                            .Alignment = StringAlignment.Center,
                            .LineAlignment = StringAlignment.Center
                        }
                        Dim txtRect As New RectangleF(0, barBmp.Height, combined.Width, textHeight)
                        Using br As New SolidBrush(Color.Black)
                            g.DrawString(txt, textFont, br, txtRect, sf)
                        End Using
                    End Using

                    PictureBox2.Image = combined
                    PictureBox2.SizeMode = PictureBoxSizeMode.StretchImage
                Finally
                    Try
                        barBmp.Dispose()
                    Catch
                    End Try
                End Try
            Catch

            End Try


            Try
                If _imageToPrint IsNot Nothing Then
                    _imageToPrint.Dispose()
                    _imageToPrint = Nothing
                End If

                _imageToPrint = RenderPanelToBitmap(Guna2Panel1)
            Catch
            End Try

        Catch
        End Try
    End Sub

    Public Sub PrintPreviewImage()
        Try
            If _imageToPrint Is Nothing Then
                MessageBox.Show("Nothing to print.", "Print", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Return
            End If

            Try
                If Me.Visible Then
                    Try
                        Me.BringToFront()
                        Me.Refresh()
                        Application.DoEvents()
                    Catch
                    End Try
                End If

                If _imageToPrint IsNot Nothing Then
                    Try
                        _imageToPrint.Dispose()
                    Catch
                    End Try
                    _imageToPrint = Nothing
                End If

                _imageToPrint = RenderPanelToBitmap(Guna2Panel1)
            Catch
            End Try


            Try
                Dim fullnameSafe As String = If(String.IsNullOrWhiteSpace(lblfullname.Text), "LibraryCard", lblfullname.Text)
                For Each c As Char In Path.GetInvalidFileNameChars()
                    fullnameSafe = fullnameSafe.Replace(c, "_")
                Next

                Dim fileName As String = String.Format("{0} (Library Card).pdf", fullnameSafe)
                Dim downloads As String = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads")
                Dim fullPath As String = Path.Combine(downloads, fileName)

                SavePreviewAsPdf(fullPath)

                Try
                    Dim psi As New ProcessStartInfo(fullPath) With {
                        .UseShellExecute = True
                    }
                    Process.Start(psi)
                Catch
                End Try
            Catch ex As Exception
                MessageBox.Show("Failed to save PDF: " & ex.Message, "PDF Save Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            End Try
        Catch ex As Exception
            MessageBox.Show("Print error: " & ex.Message, "Print Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub SavePreviewAsPdf(path As String)
        If _imageToPrint Is Nothing Then Return
        Using doc As New PdfDocument()
            Dim page As PdfPage = doc.AddPage()


            page.Size = PdfSharp.PageSize.A4

            Using gfx As XGraphics = XGraphics.FromPdfPage(page)

                Using ms As New MemoryStream()
                    _imageToPrint.Save(ms, Imaging.ImageFormat.Png)
                    ms.Position = 0
                    Using ximg As XImage = XImage.FromStream(ms)

                        Dim margin As Double = 40
                        Dim targetW As Double = page.Width.Point - (margin * 2)
                        Dim targetH As Double = page.Height.Point - (margin * 2)

                        Dim imgW As Double = ximg.PixelWidth
                        Dim imgH As Double = ximg.PixelHeight

                        Dim ratio As Double = Math.Min(targetW / imgW, targetH / imgH)
                        Dim drawW As Double = imgW * ratio
                        Dim drawH As Double = imgH * ratio

                        Dim drawX As Double = (page.Width.Point - drawW) / 2
                        Dim drawY As Double = (page.Height.Point - drawH) / 2

                        gfx.DrawImage(ximg, drawX, drawY, drawW, drawH)
                    End Using
                End Using
            End Using


            Try
                Dim dir = System.IO.Path.GetDirectoryName(path)
                If Not Directory.Exists(dir) Then Directory.CreateDirectory(dir)
            Catch
            End Try

            doc.Save(path)
            doc.Close()
        End Using
    End Sub

    Private Sub pd_PrintPage(sender As Object, e As PrintPageEventArgs) Handles pd.PrintPage
        Try
            If _imageToPrint Is Nothing Then
                e.HasMorePages = False
                Return
            End If

            Dim marginBounds As Rectangle = e.MarginBounds

            Dim ratio As Double = Math.Min(marginBounds.Width / CType(_imageToPrint.Width, Double), marginBounds.Height / CType(_imageToPrint.Height, Double))
            Dim drawW As Integer = CInt(_imageToPrint.Width * ratio)
            Dim drawH As Integer = CInt(_imageToPrint.Height * ratio)

            Dim offsetX As Integer = Math.Max(0, (marginBounds.Width - drawW) \ 2)
            Dim offsetY As Integer = Math.Max(0, (marginBounds.Height - drawH) \ 2)

            Dim x As Integer = marginBounds.Left + offsetX
            Dim y As Integer = marginBounds.Top + offsetY

            e.Graphics.DrawImage(_imageToPrint, New Rectangle(x, y, drawW, drawH))
            e.HasMorePages = False
        Catch
            e.HasMorePages = False
        End Try
    End Sub

    Private Function RenderPanelToBitmap(p As Control) As Bitmap

        Try
            For Each ch As Control In p.Controls
                If ch.GetType().FullName.IndexOf("Guna", StringComparison.OrdinalIgnoreCase) >= 0 Then
                    If p.IsHandleCreated AndAlso p.Visible Then
                        Dim bmpScreen As New Bitmap(Math.Max(1, p.Width), Math.Max(1, p.Height))
                        Dim screenPos As Point = p.PointToScreen(Point.Empty)
                        Using g As Graphics = Graphics.FromImage(bmpScreen)
                            g.CopyFromScreen(screenPos, New Point(0, 0), p.Size, CopyPixelOperation.SourceCopy)
                        End Using
                        Return bmpScreen
                    End If
                End If
            Next
        Catch
        End Try

        Dim bmp As New Bitmap(Math.Max(1, p.Width), Math.Max(1, p.Height))

        Using g As Graphics = Graphics.FromImage(bmp)

            g.InterpolationMode = Drawing.Drawing2D.InterpolationMode.HighQualityBicubic
            g.SmoothingMode = Drawing.Drawing2D.SmoothingMode.AntiAlias


            Using b As New SolidBrush(p.BackColor)
                g.FillRectangle(b, 0, 0, p.Width, p.Height)
            End Using


            Dim SubDraw As Action(Of Control, Integer, Integer) = Nothing
            SubDraw = Sub(ctrl As Control, offsetX As Integer, offsetY As Integer)
                          If Not ctrl.Visible Then Return

                          Dim rect As New Rectangle(offsetX + ctrl.Left, offsetY + ctrl.Top, ctrl.Width, ctrl.Height)


                          Try
                              Using tb As New Bitmap(Math.Max(1, ctrl.Width), Math.Max(1, ctrl.Height))
                                  ctrl.DrawToBitmap(tb, New Rectangle(0, 0, tb.Width, tb.Height))
                                  g.DrawImage(tb, rect)

                                  Return
                              End Using
                          Catch

                          End Try

                          Try
                              Dim imgProp = ctrl.GetType().GetProperty("Image")
                              If imgProp IsNot Nothing Then
                                  Dim imgObj = imgProp.GetValue(ctrl)
                                  If TypeOf imgObj Is Image Then
                                      Dim img As Image = CType(imgObj, Image)
                                      If img IsNot Nothing Then
                                          g.DrawImage(img, rect)
                                          Return
                                      End If
                                  End If
                              End If
                          Catch
                          End Try

                          Try
                              Dim textProp = ctrl.GetType().GetProperty("Text")
                              If textProp IsNot Nothing Then
                                  Dim txt As String = CStr(textProp.GetValue(ctrl))
                                  If Not String.IsNullOrEmpty(txt) Then
                                      Dim f As Font = ctrl.Font
                                      Dim fc As Color = ctrl.ForeColor
                                      Dim format As New StringFormat()
                                      format.Alignment = StringAlignment.Near
                                      format.LineAlignment = StringAlignment.Near
                                      g.DrawString(txt, f, New SolidBrush(fc), rect, format)
                                  End If
                              End If
                          Catch
                          End Try


                          For Each ch As Control In ctrl.Controls
                              SubDraw(ch, rect.Left, rect.Top)
                          Next
                      End Sub

            For Each child As Control In p.Controls
                SubDraw(child, 0, 0)
            Next
        End Using

        Return bmp
    End Function

    Private Sub LibraryCardPreview_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub
End Class
