'===============================================================================
'Copyright Wigersma 2013
'All rights reserved.
'===============================================================================
Imports System.Xml.Xsl
Imports System.IO
Imports System.Reflection
Imports System.Xml
Imports System.Windows.Forms
Imports KAM.INSPECTOR.Infra
Imports KAM.INSPECTOR.Results.My.Resources
Public Class DisplayResults
    Private Sub DisplayResult_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        DisplayResults()
    End Sub

#Region "Functions"
    ''' <summary>
    ''' Transform the xml file by xslt and display in a webbrowser
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub DisplayResults()
        Dim tempDir As String = ModuleSettings.SettingFile.GetSetting("Application", "TemporaryFilesPath")
        Dim dataXMLDir As String = ModuleSettings.SettingFile.GetSetting("Application", "XmlFilesPath")

        tempDir = Path.Combine(Application.StartupPath, tempDir)
        dataXMLDir = Path.Combine(Application.StartupPath, dataXMLDir)

        If File.Exists(Path.Combine(dataXMLDir, "Results.xml")) Then
            Using strm As Stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("KAM.INSPECTOR.Results.Report INSPECTOR PC.xslt")
                Using reader As XmlReader = XmlReader.Create(strm)
                    Dim transform As New XslCompiledTransform()
                    ' use the XslTransform object
                    transform.Load(reader)
                    transform.Transform(Path.Combine(dataXMLDir, "Results.xml"), Path.Combine(tempDir, "tmpFile.html"))
                    webBrResults.Navigate(Path.Combine(tempDir, "tmpFile.html"))
                    'File.Delete(Path.Combine(tempDir, "tmpFile.html"))
                End Using
            End Using
        Else
            webBrResults.DocumentText = INSPECTORResultResx.str_NoResultsAvailable
        End If
        webBrResults.Focus()
    End Sub
#End Region
#Region "Button handling"
    ''' <summary>
    ''' Handling of button click Show results
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub rdbShowResults_Click(sender As System.Object, e As System.EventArgs) Handles rdbShowResults.Click

    End Sub
#End Region
End Class

