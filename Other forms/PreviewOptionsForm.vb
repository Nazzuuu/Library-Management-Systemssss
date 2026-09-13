Imports System.Drawing
Imports System.Windows.Forms

Public Class PreviewOptionsForm
    Inherits Form

    Private lblMessage As Label
    Private btnYes As Button
    Private btnNo As Button
    Private pic As PictureBox

    Public Sub New(message As String)
        Me.FormBorderStyle = FormBorderStyle.FixedToolWindow
        Me.ClientSize = New Size(320, 120)
        Me.StartPosition = FormStartPosition.Manual
        Me.Text = "Preview Options"
        Me.ShowInTaskbar = False

        pic = New PictureBox()
        pic.Size = New Size(32, 32)
        pic.Location = New Point(16, 16)
        pic.Image = SystemIcons.Question.ToBitmap()
        pic.SizeMode = PictureBoxSizeMode.CenterImage
        Me.Controls.Add(pic)

        lblMessage = New Label()
        lblMessage.AutoSize = False
        lblMessage.Size = New Size(240, 64)
        lblMessage.Location = New Point(64, 12)
        lblMessage.Text = message
        lblMessage.Font = New Font("Segoe UI", 9)
        Me.Controls.Add(lblMessage)

        btnYes = New Button()
        btnYes.Text = "Yes"
        btnYes.Size = New Size(76, 26)
        btnYes.Location = New Point(150, 80)
        btnYes.DialogResult = DialogResult.Yes
        Me.Controls.Add(btnYes)

        btnNo = New Button()
        btnNo.Text = "No"
        btnNo.Size = New Size(76, 26)
        btnNo.Location = New Point(234, 80)
        btnNo.DialogResult = DialogResult.No
        Me.Controls.Add(btnNo)

        Me.AcceptButton = btnYes
        Me.CancelButton = btnNo
    End Sub

    Public Shared Function ShowForOwner(owner As Form, message As String) As DialogResult
        Dim dlg As New PreviewOptionsForm(message)
        Try
            If owner IsNot Nothing Then
                ' position dialog outside to the left of the owner window
                Dim desiredX As Integer = owner.Location.X - dlg.Width - 20
                Dim screenBounds As Rectangle = Screen.FromControl(owner).WorkingArea
                ' ensure dialog stays visible on screen
                Dim x As Integer = If(desiredX < screenBounds.Left + 10, screenBounds.Left + 10, desiredX)
                Dim y As Integer = owner.Location.Y + Math.Max(20, (owner.Height - dlg.Height) \ 2)
                dlg.StartPosition = FormStartPosition.Manual
                dlg.Location = New Point(x, y)
            Else
                dlg.StartPosition = FormStartPosition.CenterScreen
            End If

            Return dlg.ShowDialog(owner)
        Finally
            dlg.Dispose()
        End Try
    End Function

    ' Ensure Load event exists to match any designer or serialized event hookups.
    Private Sub PreviewOptionsForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' Intentionally left blank - prevents "Method 'PreviewOptionsForm_Load' not found" errors
        ' when the form instance gets created and the Load event is wired from designer/resx.
    End Sub
End Class
