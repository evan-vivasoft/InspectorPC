Imports System.Xml
Imports System.IO

Namespace Kamstrup.LicenseKeyGenerator.Model
    ''' <summary>
    ''' Get the affiliates user names
    ''' </summary>
    ''' <remarks></remarks>
    Public Class clsUserAffiliates
        Public Property UserName() As String
            Get
                Return m_UserName
            End Get
            Set(value As String)
                m_UserName = value
            End Set
        End Property
        Dim m_UserName As String
        Public Property ProductsName() As List(Of String)
            Get
                Return m_ProductName
            End Get
            Set(value As List(Of String))
                m_ProductName = value
            End Set
        End Property
        Dim m_ProductName As New List(Of String)

        ''' <summary>
        ''' Maps the data received from the webserver (XML) to a class list
        ''' </summary>
        ''' <param name="dataSet"></param>
        ''' <param name="affiliates"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function DatasetMapper(dataSet As String, affiliates As List(Of clsAffiliates)) As List(Of clsUserAffiliates)
            Dim userAffiliatesList As New List(Of clsUserAffiliates)()
            Dim doc As New XmlDocument()
            doc.Load(New StringReader(dataSet))
            Dim nodeList As XmlNodeList = doc.SelectNodes("UserAccounts")
            If nodeList IsNot Nothing Then
                For Each newDataSetNode As XmlNode In nodeList
                    If newDataSetNode.HasChildNodes Then
                        For Each tableNode As XmlNode In newDataSetNode.ChildNodes
                            If tableNode.Name.Equals("UserAccount") Then
                                Dim userAffiliate As New Kamstrup.LicenseKeyGenerator.Model.clsUserAffiliates()
                                For Each attribute As XmlAttribute In tableNode.Attributes
                                    If attribute.Name.Equals("UserName") Then
                                        userAffiliate.UserName = attribute.Value
                                    End If
                                    If attribute.Name.Equals("AffiliateID") Then
                                        Dim selectedAffiliates As Kamstrup.LicenseKeyGenerator.Model.clsAffiliates
                                        selectedAffiliates = affiliates.Find(Function(p) p.Id = attribute.Value)
                                        If Not IsNothing(selectedAffiliates) Then
                                            userAffiliate.ProductsName.AddRange(selectedAffiliates.ProductsName)
                                        End If
                                    End If
                                Next
                                userAffiliatesList.Add(userAffiliate)
                            End If
                        Next
                    End If
                Next
            End If
            Return userAffiliatesList
        End Function
    End Class

    ''' <summary>
    ''' 'The QLM webserver Affiliates are use to assign the user right to the products.
    ''' If the user has no products, the user can not create orders.
    ''' He can only create license keys
    ''' </summary>
    ''' <remarks></remarks>
    Public Class clsAffiliates
        Public Property Id() As String
            Get
                Return m_ID
            End Get
            Set(value As String)
                m_ID = value
            End Set
        End Property
        Dim m_ID As String

        Public Property ProductsName() As List(Of String)
            Get
                Return m_ProductName
            End Get
            Set(value As List(Of String))
                m_ProductName = value
            End Set
        End Property
        Dim m_ProductName As New List(Of String)
        ''' <summary>
        ''' Maps the data received from the webserver (XML) to a class list
        ''' </summary>
        ''' <param name="dataSet"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function DatasetMapper(dataSet As String) As List(Of clsAffiliates)
            Dim affiliatesList As New List(Of clsAffiliates)()
            Dim doc As New XmlDocument()
            doc.Load(New StringReader(dataSet))
            Dim nodeList As XmlNodeList = doc.SelectNodes("NewDataSet")
            If nodeList IsNot Nothing Then
                For Each newDataSetNode As XmlNode In nodeList
                    If newDataSetNode.HasChildNodes Then
                        For Each tableNode As XmlNode In newDataSetNode.ChildNodes

                            If tableNode.Name.Equals("Affiliates") Then
                                Dim affiliate As New Kamstrup.LicenseKeyGenerator.Model.clsAffiliates()
                                Dim productsList As New List(Of String)
                                For Each node As XmlNode In tableNode.ChildNodes
                                    If node.Name.Equals("AffiliateID") Then
                                        affiliate.Id = node.InnerText.Trim
                                    End If
                                    If node.Name.Equals("Products") Then
                                        Dim productsSplit() As String
                                        productsSplit = Split(node.InnerText, ";")
                                        Dim productFound As String

                                        For Each productFound In productsSplit
                                            If productFound <> "" Then productsList.Add(productFound.Trim)
                                        Next
                                        affiliate.ProductsName.AddRange(productsList)
                                    End If

                                Next

                                affiliatesList.Add(affiliate)
                            End If
                        Next
                    End If
                Next
            End If
            Return affiliatesList
        End Function
    End Class
End Namespace
