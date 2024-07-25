Imports System.IO
Imports System.Xml
Public Module modFileHandling
#Region "File copy and check handling"
    ''' <summary>
    ''' Delete mulitple files with extension on PC
    ''' </summary>
    ''' <param name="localFileDir">Directory on the PC</param>
    ''' <param name="fileExtenstion">File extension of files to delete  (*.* = delete all files)</param>
    ''' <remarks></remarks>
    Public Sub DeleteMultipleFilePC(localFileDir As String, fileExtenstion As String)

        For Each fileFound As String In Directory.GetFiles(localFileDir, fileExtenstion)
            File.Delete(fileFound)
        Next
    End Sub

    ''' <summary>
    ''' Check if a file is present.
    ''' True if file exists. If false, a message is diplayed
    ''' </summary>
    ''' <param name="pathFileName">File path and file</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CheckFileExistsPC(ByVal pathFileName As String) As Boolean
        If File.Exists(pathFileName) Then
            Return True
        Else
            'TO DO MsgBox(My.Resources.INSPECTORMainResx.str_FileNotExists1 & " '" & pathFileName & "' " & My.Resources.INSPECTORMainResx.str_fileNotExists2, MsgBoxStyle.Critical, QlmProductName)
            Return False
        End If
    End Function
    ''' <summary>
    ''' Check if a file is present.
    ''' True if file exists. If false, a message is diplayed
    ''' </summary>
    ''' <param name="pathName">File path name</param>
    ''' <param name="fileName">File Name</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CheckFileExistsPC(ByVal pathName As String, ByVal fileName As String) As Boolean
        If File.Exists(Path.Combine(pathName, fileName)) Then
            Return True
        Else
            'TO DO  MsgBox(My.Resources.INSPECTORMainResx.str_FileNotExists1 & " '" & Path.Combine(pathName, fileName) & "' " & My.Resources.INSPECTORMainResx.str_fileNotExists2, MsgBoxStyle.Critical, QlmProductName)
            Return False
        End If
    End Function

    ''' <summary>
    ''' Check if a directory exists
    ''' </summary>
    ''' <param name="dirName">Directory to check</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function DirExistsPC(dirName As String) As Boolean
        Return Directory.Exists(dirName)
    End Function


    ''' <summary>
    ''' Copy mulitple files with extension at PC
    ''' </summary>
    ''' <param name="sourceDirectory">Source directory</param>
    ''' <param name="destDirectory">Destination directory</param>
    ''' <param name="fileExtenstion">File extension to copy (*.* = copy all files)</param>
    ''' <remarks></remarks>
    Public Sub CopyMultipleFilesPC(sourceDirectory As String, destDirectory As String, fileExtenstion As String)
        For Each fileFound As String In Directory.GetFiles(sourceDirectory, fileExtenstion)
            Try
                File.Copy(fileFound, Path.Combine(destDirectory, Path.GetFileName(fileFound)), True)
            Catch ex As Exception
            End Try
        Next
    End Sub

    ''' <summary>
    ''' Move mulitple files with extension at PC
    ''' </summary>
    ''' <param name="sourceDirectory">Source directory</param>
    ''' <param name="destDirectory">Destination directory</param>
    ''' <param name="fileExtenstion">File extension to move (*.* = move all files)</param>
    ''' <remarks></remarks>
    Public Sub MoveMultipleFilesPC(sourceDirectory As String, destDirectory As String, fileExtenstion As String)
        For Each fileFound As String In Directory.GetFiles(sourceDirectory, fileExtenstion)
            Try
                File.Copy(fileFound, Path.Combine(destDirectory, Path.GetFileName(fileFound)), True)
                File.Delete(fileFound)
            Catch ex As Exception
            End Try
        Next
    End Sub
    ''' <summary>
    ''' Copy files
    ''' </summary>
    ''' <param name="sourceFileName">Source directory</param>
    ''' <param name="destFileName">Destination directory</param>
    ''' <param name="overWrite">Overwrite current file</param>
    ''' <remarks></remarks>
    Public Sub CopyFilePC(sourceFileName As String, destFileName As String, overWrite As Boolean)
        If CheckFileExistsPC(sourceFileName) Then File.Copy(sourceFileName, destFileName, overWrite)
    End Sub

    ''' <summary>
    ''' Delete files
    ''' </summary>
    ''' <param name="sourceFileName">File to delete</param>
    ''' <remarks></remarks>
    Public Sub DeleteFilePC(sourceFileName As String)
        If CheckFileExistsPC(sourceFileName) Then File.Delete(sourceFileName)
    End Sub
#End Region
End Module
Public Class clsCompareFileInfo
    Implements IComparer
    Public Function Compare(ByVal x As Object, ByVal y As Object) As Integer Implements IComparer.Compare
        Dim File1 As FileInfo
        Dim File2 As FileInfo

        File1 = DirectCast(x, FileInfo)
        File2 = DirectCast(y, FileInfo)

        Compare = DateTime.Compare(File1.LastWriteTime, File2.LastWriteTime)
    End Function
End Class

Public Module xmlHelpers
    Public Sub ValidateXmlFile(xmlFile As String, xsdFile As String)
        Try
            Dim settings As New XmlReaderSettings()
            settings.Schemas.Add(Nothing, xsdFile)
            settings.ValidationType = ValidationType.Schema
            Dim document As New XmlDocument()
            document.Load(xmlFile)

            Using stringReader As New IO.StringReader(document.InnerXml)
                Using xmlReader1 As XmlReader = XmlReader.Create(stringReader, settings)
                    While xmlReader1.Read()
                    End While
                End Using
            End Using
        Catch ex As Exception
            Throw New XmlException(String.Format("Failed to validate XML File: {0} to XSD file: {1}. Exception: {2}", xmlFile, xsdFile, ex.Message), ex)
        End Try

    End Sub
End Module