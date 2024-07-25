'===============================================================================
'Copyright Wigersma 2013
'All rights reserved.
'===============================================================================

Imports System.Xml.Xsl


Namespace Model.Station
    ''' <summary>
    ''' Information of PRS and contained GasControlLines
    ''' </summary>
    Public Module XmlXsltTransformation


#Region "Properties"
        ''' <summary>
        ''' Convert the output from PRSsyncEntities to the correct XML file
        ''' </summary>
        ''' <param name="xmlInputFileName">PRS sync entities XML file</param>
        ''' <param name="xmlOutputFileName">XML output file</param>
        ''' <param name="xsltFileName">Transformation file</param>
        ''' <remarks></remarks>
        Public Sub ConvertxmlFile(ByVal xmlInputFileName As String, ByVal xmlOutputFileName As String, ByVal xsltFileName As String)
            Dim transformer As XslCompiledTransform
            transformer = New XslCompiledTransform() ' create transformer
            transformer.Load(xsltFileName) ' load and compile the style sheet
            transformer.Transform(xmlInputFileName, xmlOutputFileName)
        End Sub

#End Region

    End Module
End Namespace