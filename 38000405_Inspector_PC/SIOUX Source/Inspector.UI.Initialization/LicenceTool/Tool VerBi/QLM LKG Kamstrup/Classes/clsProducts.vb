Imports System.Xml
Imports System.IO

Namespace Kamstrup.LicenseKeyGenerator.Model
    Public Class clsProducts
        Public Property ID() As String
            Get
                Return m_ID
            End Get
            Set(value As String)
                m_ID = value
            End Set
        End Property
        Dim m_ID As String

        Public Property Name() As String
            Get
                Return m_Name
            End Get
            Set(value As String)
                m_Name = value
            End Set
        End Property
        Dim m_Name As String

        Public Property MajorVersion() As String
            Get
                Return m_MajorVersion
            End Get
            Set(value As String)
                m_MajorVersion = value
            End Set
        End Property
        Dim m_MajorVersion As String
        Public Property MinorVersion() As String
            Get
                Return m_MinorVersion
            End Get
            Set(value As String)
                m_MinorVersion = value
            End Set
        End Property
        Dim m_MinorVersion As String

        Public Property Key() As String
            Get
                Return m_Key
            End Get
            Set(value As String)
                m_Key = value
            End Set
        End Property
        Dim m_Key As String
        Public Property GUID() As String
            Get
                Return m_GUID
            End Get
            Set(value As String)
                m_GUID = value
            End Set
        End Property
        Dim m_GUID As String

        Public Property ReleaseDate() As DateTime
            Get
                Return m_ReleaseDate
            End Get
            Set(value As DateTime)
                m_ReleaseDate = value
            End Set
        End Property
        Dim m_ReleaseDate As String
        Public Property PrivateKey() As String
            Get
                Return m_PrivateKey
            End Get
            Set(value As String)
                m_PrivateKey = value
            End Set
        End Property
        Dim m_PrivateKey As String
        Public Property PublicKey() As String
            Get
                Return m_PublicKey
            End Get
            Set(value As String)
                m_PublicKey = value
            End Set
        End Property
        Dim m_PublicKey As String


        Public Shared Function DatasetMapper(dataSet As String) As List(Of clsProducts)
            Dim productsList As New List(Of clsProducts)()
            Dim doc As New XmlDocument()
            doc.Load(New StringReader(dataSet))
            Dim nodeList As XmlNodeList = doc.SelectNodes("NewDataSet")
            If nodeList IsNot Nothing Then
                For Each newDataSetNode As XmlNode In nodeList
                    If newDataSetNode.HasChildNodes Then
                        For Each tableNode As XmlNode In newDataSetNode.ChildNodes
                            If tableNode.Name.Equals("Table") Then
                                Dim product As New Kamstrup.LicenseKeyGenerator.Model.clsProducts()
                                For Each node As XmlNode In tableNode.ChildNodes
                                    If node.Name.Equals("ID") Then
                                        product.ID = node.InnerText
                                    End If
                                    If node.Name.Equals("ProductName") Then
                                        product.Name = node.InnerText
                                    End If
                                    If node.Name.Equals("Major") Then
                                        product.MajorVersion = node.InnerText
                                    End If
                                    If node.Name.Equals("Minor") Then
                                        product.MinorVersion = node.InnerText
                                    End If
                                    If node.Name.Equals("Key") Then
                                        product.Key = node.InnerText
                                    End If
                                    If node.Name.Equals("GUID") Then
                                        product.GUID = node.InnerText
                                    End If
                                    If node.Name.Equals("ReleaseDate") Then
                                        product.ReleaseDate = node.InnerText
                                    End If
                                    If node.Name.Equals("PrK") Then
                                        product.PrivateKey = node.InnerText
                                    End If
                                    If node.Name.Equals("PuK") Then
                                        product.PublicKey = node.InnerText
                                    End If

                                Next
                                productsList.Add(product)
                            End If
                        Next
                    End If
                Next
            End If
            Return productsList
        End Function
    End Class


End Namespace
'<?xml version="1.0" encoding="UTF-8"?>
'<xs:complexType>
'- <xs:sequence>
'  <xs:element name="ID" type="xs:int" minOccurs="0" /> 
'  <xs:element name="ProductName" type="xs:string" minOccurs="0" /> 
'  <xs:element name="Major" type="xs:string" minOccurs="0" /> 
'  <xs:element name="Minor" type="xs:string" minOccurs="0" /> 
'  <xs:element name="Key" type="xs:string" minOccurs="0" /> 
'  <xs:element name="GUID" type="xs:string" minOccurs="0" /> 
'  <xs:element name="Features" type="xs:string" minOccurs="0" /> 
'  <xs:element name="LatestVersion" type="xs:string" minOccurs="0" /> 
'  <xs:element name="LatestVersionUrl" type="xs:string" minOccurs="0" /> 
'  <xs:element name="LatestVersionNotes" type="xs:string" minOccurs="0" /> 
'  <xs:element name="ReleaseDate" type="xs:dateTime" minOccurs="0" /> 
'  <xs:element name="PrK" type="xs:string" minOccurs="0" /> 
'  <xs:element name="PuK" type="xs:string" minOccurs="0" /> 
'  <xs:element name="VendorID" type="xs:string" minOccurs="0" /> 
'  <xs:element name="VendorProductID" type="xs:string" minOccurs="0" /> 
'  <xs:element name="MPrK" type="xs:string" minOccurs="0" /> 
'  <xs:element name="MPuK" type="xs:string" minOccurs="0" /> 
'  </xs:sequence>
