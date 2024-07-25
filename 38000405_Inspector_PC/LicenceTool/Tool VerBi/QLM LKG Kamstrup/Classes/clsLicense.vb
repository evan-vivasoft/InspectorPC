Imports System.Xml
Imports System.IO

Namespace Kamstrup.LicenseKeyGenerator.Model
    Public Class clsLicense

        Friend UserID As String

        Public Property ProductName() As String
            Get
                Return m_ProductName
            End Get
            Set(value As String)
                m_ProductName = value
            End Set
        End Property
        Private m_ProductName As String
        Public Property ActivationKey() As String
            Get
                Return m_ActivationKey
            End Get
            Set(value As String)
                m_ActivationKey = value
            End Set
        End Property
        Private m_ActivationKey As String
        Public Property ActivationDate() As String
            Get
                Return m_ActivationDate
            End Get
            Set(value As String)
                m_ActivationDate = value
            End Set
        End Property
        Private m_ActivationDate As String
        Public Property ComputerKey() As String
            Get
                Return m_ComputerKey
            End Get
            Set(value As String)
                m_ComputerKey = value
            End Set
        End Property
        Private m_ComputerKey As String
        'Public Property ComputerName() As String
        '    Get
        '        Return m_ComputerName
        '    End Get
        '    Set(value As String)
        '        m_ComputerName = value
        '    End Set
        'End Property
        'Private m_ComputerName As String
        Public Property LicenseKey() As String
            Get
                Return m_LicenseKey
            End Get
            Set(value As String)
                m_LicenseKey = value
            End Set
        End Property
        Private m_LicenseKey As String

        Public Property UserData() As String
            Get
                Return m_UserData
            End Get
            Set(value As String)
                m_UserData = value
            End Set
        End Property
        Private m_UserData As String
        Public Property Customer() As clsCustomerInfo
            Get
                Return m_Customer
            End Get
            Set(value As clsCustomerInfo)
                m_Customer = value
            End Set
        End Property
        Private m_Customer As clsCustomerInfo

        Public Property OrderNo() As String
            Get
                Return m_OrderNo
            End Get
            Set(value As String)
                m_OrderNo = value
            End Set
        End Property
        Private m_OrderNo As String

        Public Property OrderDate() As DateTime
            Get
                Return m_OrderDates
            End Get
            Set(value As DateTime)
                m_OrderDates = value
            End Set
        End Property
        Private m_OrderDates As DateTime

        Public Property OrderStatus() As String
            Get
                Return m_OrderStatus
            End Get
            Set(value As String)
                m_OrderStatus = value
            End Set
        End Property
        Private m_OrderStatus As String


        ''' <summary>
        ''' Number of times this license has been released i.e. moved from old computer to a new one
        ''' </summary>
        Public Property ReleaseCount() As String
            Get
                Return m_ReleaseCount
            End Get
            Set(value As String)
                m_ReleaseCount = value
            End Set
        End Property
        Private m_ReleaseCount As String

        Public Property ProductID() As String
            Get
                Return m_ProductID
            End Get
            Set(value As String)
                m_ProductID = value
            End Set
        End Property
        Private m_ProductID As String

        Public Shared Function DatasetMapper(dataSet As String, productsList As List(Of Kamstrup.LicenseKeyGenerator.Model.clsProducts), customerList As List(Of Kamstrup.LicenseKeyGenerator.Model.clsCustomerInfo)) As List(Of clsLicense)
            Dim licenseList As New List(Of clsLicense)()
            Dim doc As New XmlDocument()
            doc.Load(New StringReader(dataSet))
            Dim nodeList As XmlNodeList = doc.SelectNodes("NewDataSet")
            If nodeList IsNot Nothing Then
                For Each newDataSetNode As XmlNode In nodeList
                    If newDataSetNode.HasChildNodes Then
                        For Each tableNode As XmlNode In newDataSetNode.ChildNodes
                            If tableNode.Name.Equals("Table") Then
                                Dim license As New Kamstrup.LicenseKeyGenerator.Model.clsLicense()
                                For Each node As XmlNode In tableNode.ChildNodes
                                    If node.Name.Equals("ActivationKey") Then
                                        license.ActivationKey = node.InnerText
                                    End If
                                    If node.Name.Equals("ComputerKey") Then
                                        license.LicenseKey = node.InnerText
                                    End If
                                    'If node.Name.Equals("ComputerName") Then
                                    '    license.ComputerName = node.InnerText
                                    'End If
                                    'If node.Name.Equals("ReleaseCount") Then
                                    '    license.ReleaseCount = node.InnerText
                                    'End If
                                    If node.Name.Equals("ComputerID") Then
                                        license.ComputerKey = node.InnerText
                                    End If
                                    If node.Name.Equals("UserData1") Then
                                        license.UserData = node.InnerText
                                    End If

                                    If node.Name.Equals("OrderID") Then
                                        license.OrderNo = node.InnerText
                                    End If
                                    If node.Name.Equals("OrderStatus") Then
                                        license.OrderStatus = node.InnerText
                                    End If
                                    If node.Name.Equals("OrderDate") Then
                                        license.OrderDate = node.InnerText
                                    End If
                                    If node.Name.Equals("ActivationDate") Then
                                        license.ActivationDate = node.InnerText
                                    End If
                                    If node.Name.Equals("UserID") Then
                                        Dim id As Integer
                                        Integer.TryParse(node.InnerText, id)

                                        Dim selectedCustomer As Kamstrup.LicenseKeyGenerator.Model.clsCustomerInfo
                                        selectedCustomer = customerList.Find(Function(p) p.CustomerID = id)
                                        If Not IsNothing(selectedCustomer) Then
                                            license.Customer = selectedCustomer
                                        End If
                                    End If
                                    If node.Name.Equals("ProductID") Then
                                        Dim id As Integer
                                        Integer.TryParse(node.InnerText, id)
                                        license.ProductID = node.InnerText
                                        Dim selectedProduct As Kamstrup.LicenseKeyGenerator.Model.clsProducts
                                        selectedProduct = productsList.Find(Function(p) p.ID = license.ProductID)
                                        If Not IsNothing(selectedProduct) Then
                                            license.ProductName = selectedProduct.Name
                                        End If
                                        'license.ProductName = ProductController.Instance.GetSpecificProduct(id).ProductName
                                    End If

                                Next
                                licenseList.Add(license)
                            End If
                        Next
                    End If
                Next
            End If
            Return licenseList
        End Function


    End Class

    '<xs:element name="ActivationKey" type="xs:string" minOccurs="0" />
    '<xs:element name="ComputerKey" type="xs:string" minOccurs="0" />
    '<xs:element name="ComputerID" type="xs:string" minOccurs="0" />
    '<xs:element name="UserID" type="xs:int" minOccurs="0" />
    '<xs:element name="ProductID" type="xs:int" minOccurs="0" />
    '<xs:element name="MajorVersion" type="xs:string" minOccurs="0" />
    '<xs:element name="MinorVersion" type="xs:string" minOccurs="0" />
    '<xs:element name="OrderDate" type="xs:dateTime" minOccurs="0" />
    '<xs:element name="ActivationDate" type="xs:dateTime" minOccurs="0" />
    '<xs:element name="LastAccessedDate" type="xs:dateTime" minOccurs="0" />
    '<xs:element name="ActivationCount" type="xs:int" minOccurs="0" />
    '<xs:element name="OrderID" type="xs:string" minOccurs="0" />
    '<xs:element name="Comment" type="xs:string" minOccurs="0" />
    '<xs:element name="MaintenanceRenewalDate" type="xs:dateTime" minOccurs="0" />
    '<xs:element name="MaintenancePlanNotification" type="xs:boolean" minOccurs="0" />
    '<xs:element name="SubscriptionExpiryDate" type="xs:dateTime" minOccurs="0" />
    '<xs:element name="GenericLicense" type="xs:boolean" minOccurs="0" />
    '<xs:element name="ReleaseCount" type="xs:int" minOccurs="0" />
    '<xs:element name="ReleaseDate" type="xs:dateTime" minOccurs="0" />
    '<xs:element name="NumLicenses" type="xs:int" minOccurs="0" />
    '<xs:element name="AvailableLicenses" type="xs:int" minOccurs="0" />
    '<xs:element name="ComputerName" type="xs:string" minOccurs="0" />
    '<xs:element name="ClientVersion" type="xs:string" minOccurs="0" />
    '<xs:element name="Disabled" type="xs:boolean" minOccurs="0" />
    '<xs:element name="UserData1" type="xs:string" minOccurs="0" />
    '<xs:element name="AffiliateID" type="xs:string" minOccurs="0" />
    '<xs:element name="ReceiptID" type="xs:string" minOccurs="0" />
    '<xs:element name="OrderStatus" type="xs:long" minOccurs="0" />
    '<xs:element name="FloatingSeats" type="xs:int" minOccurs="0" />
    '<xs:element name="FloatingLicenseLocation" type="xs:string" minOccurs="0" />

End Namespace
