Imports System.IO

Public Class DurationForm

    Dim filePath As String = Application.StartupPath & "\duration_settings.txt"

    Private Sub DurationForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        NumericUpDown1.Minimum = 1
        NumericUpDown1.Maximum = 14

        NumericUpDown2.Minimum = 1
        NumericUpDown2.Maximum = 35

        NumericUpDown1.Value = studentLimit
        NumericUpDown2.Value = teacherLimit

        If File.Exists(filePath) Then
            Dim lines() As String = File.ReadAllLines(filePath)
            If lines.Length >= 2 Then
                NumericUpDown1.Value = Val(lines(0))
                NumericUpDown2.Value = Val(lines(1))
            End If
        End If
    End Sub

    Private Sub NumericUpDown1_ValueChanged(sender As Object, e As EventArgs) Handles NumericUpDown1.ValueChanged
        If NumericUpDown1.Value > 14 Then
            NumericUpDown1.Value = 14
        End If
    End Sub

    Private Sub NumericUpDown2_ValueChanged(sender As Object, e As EventArgs) Handles NumericUpDown2.ValueChanged
        If NumericUpDown2.Value > 35 Then
            NumericUpDown2.Value = 35
        End If
    End Sub

    Private Sub btnsave_Click(sender As Object, e As EventArgs) Handles btnsave.Click
        Dim studentDays As Integer = NumericUpDown1.Value
        Dim teacherDays As Integer = NumericUpDown2.Value


        Dim settings() As String = {studentDays.ToString(), teacherDays.ToString()}
        File.WriteAllLines(filePath, settings)

        studentLimit = NumericUpDown1.Value
        teacherLimit = NumericUpDown2.Value

        MessageBox.Show("Student Duration: " & studentDays & " days" & vbCrLf &
                        "Teacher Duration: " & teacherDays & " days", "Settings Saved", MessageBoxButtons.OK, MessageBoxIcon.Information)
    End Sub
End Class