Imports System.Windows.Forms
Imports Microsoft.VisualBasic.ApplicationServices


Namespace Library_Management_Systemssss

    Public Class SingleInstanceController
        Inherits WindowsFormsApplicationBase


        Public Sub New()
            Me.IsSingleInstance = True
        End Sub

        Protected Overrides Sub OnCreateMainForm()

            Me.MainForm = New login()
        End Sub


        Protected Overrides Sub OnStartupNextInstance(ByVal eventArgs As StartupNextInstanceEventArgs)
            MyBase.OnStartupNextInstance(eventArgs)

            If Not IsNothing(GlobalVarsModule.ActiveMainForm) Then
                GlobalVarsModule.ActiveMainForm.BringToFront()
            End If
        End Sub
    End Class

    Module Program

        <STAThread()>
        Public Sub Main()
            Application.EnableVisualStyles()
            Application.SetCompatibleTextRenderingDefault(False)

            Dim applicationController As New SingleInstanceController()
            applicationController.Run(New String() {})
        End Sub

    End Module

End Namespace