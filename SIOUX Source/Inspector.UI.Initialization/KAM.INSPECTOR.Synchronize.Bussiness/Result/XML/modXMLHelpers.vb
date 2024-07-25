Imports System.IO
Imports System.Globalization
Imports KAM.Infra
'===============================================================================
'Copyright Wigersma 2015
'All rights reserved.
'===============================================================================

Namespace Model.Result
    Module modXMLHelpers
        ''' <summary>
        ''' Reads the InspectionResult information into model.Results.Entities.InspectionResultEntities
        ''' </summary>
        ''' <param name="xmlFile">Xml file to read</param>
        ''' <param name="xsdFile">XSD file</param>
        ''' <param name="xmlDataSet">the XML dataset</param>
        ''' <param name="ioFileWriteTime">file last write time and date</param>
        ''' <remarks></remarks>
        Public Sub ReadInspectionResults(ByVal xmlFile As String, ByVal xsdFile As String, ByRef xmlDataSet As DataSet, ByRef ioFileWriteTime As DateTime)
            If File.Exists(xmlFile) = False Then
                xmlFile = Nothing
            End If

            If xmlFile <> Nothing Then
                xmlHelpers.ValidateXmlFile(xmlFile, xsdFile)
            End If

            Using dataSet As New DataSet()
                dataSet.Locale = CultureInfo.InvariantCulture
                dataSet.ReadXmlSchema(xsdFile)
                If xmlFile <> Nothing Then
                    dataSet.ReadXml(xmlFile, XmlReadMode.ReadSchema)
                    xmlDataSet = dataSet
                    ioFileWriteTime = IO.File.GetLastWriteTime(xmlFile)
                Else
                End If
            End Using
        End Sub
    End Module
End Namespace