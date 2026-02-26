Imports PdfSharp.Fonts
Imports System.IO

Public Class CustomFontResolver
    Implements IFontResolver

    Public Function ResolveTypeface(familyName As String, isBold As Boolean, isItalic As Boolean) As FontResolverInfo Implements IFontResolver.ResolveTypeface
        If familyName.Equals("Arial", StringComparison.OrdinalIgnoreCase) Then
            Return New FontResolverInfo("Arial#")
        End If

        Return Nothing
    End Function

    Public Function GetFont(faceName As String) As Byte() Implements IFontResolver.GetFont
        If faceName = "Arial#" Then
            '
            Dim fontPath As String = "C:\Windows\Fonts\arial.ttf"
            Return File.ReadAllBytes(fontPath)
        End If

        Return Nothing
    End Function
End Class