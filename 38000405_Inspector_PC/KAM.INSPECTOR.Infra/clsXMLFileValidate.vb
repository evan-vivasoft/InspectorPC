'===============================================================================
'Copyright Wigersma 2012
'All rights reserved.
'===============================================================================
Imports System.Xml

Namespace XML
    Public Class Validate
#Region "Class members"
        Private xmlDoc As XmlDocument
        Public Event XMLValidationError(ByVal msg As String)
#End Region
#Region "Constructor"
        ''' <summary>
        ''' Loading of xml document
        ''' </summary>
        ''' <param name="xmlDocument">XML File</param>
        ''' <param name="xsdDocument">XSD file</param>
        ''' <remarks></remarks>
        Public Sub New(ByVal xmlDocument As String, ByVal xsdDocument As String)

            xmlDoc = New XmlDocument
            xmlDoc.Load(xmlDocument)

            xmlDoc.Schemas.Add(Nothing, xsdDocument)
            ValidateXML()

            xmlDoc = Nothing
        End Sub
#End Region
#Region "XML validation"
        ''' <summary>
        ''' An event is raise in case the document is not correct
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Private Sub XmlValidationEventHandler(ByVal sender As Object, ByVal e As System.Xml.Schema.ValidationEventArgs)
            RaiseEvent XMLValidationError(e.Message)
        End Sub
        ''' <summary>
        ''' Validate XML file. Event is raised
        ''' </summary>
        ''' <remarks></remarks>
        Private Sub ValidateXML()
            Dim validationHandler As System.Xml.Schema.ValidationEventHandler = New System.Xml.Schema.ValidationEventHandler(AddressOf XmlValidationEventHandler)
            Try
                xmlDoc.Validate(validationHandler)
            Catch ex As Exception
            End Try
        End Sub
#End Region
End Class

End Namespace
