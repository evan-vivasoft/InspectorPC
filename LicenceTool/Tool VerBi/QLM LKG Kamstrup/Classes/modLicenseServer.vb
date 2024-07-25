Imports QlmLicenseLib
Namespace Kamstrup.LicenseKeyGenerator.Model
    Public Module modLicenseServer
#Region "Class members"
        Private m_qlmUrl As Uri = New Uri(Kamstrup.LicenseKeyGenerator.Constants.Url)
        Public Event evntLoadingmodule(information As String)
#End Region
#Region "properties"
        Public License As QlmLicense
        Public LicenseInfo As New LicenseInfo
        ''' <summary>
        ''' List of products
        ''' </summary>
        ''' <remarks></remarks>
        Public ProductsList As List(Of Kamstrup.LicenseKeyGenerator.Model.clsProducts)
        ''' <summary>
        ''' List of users right of the products. 
        ''' </summary>
        ''' <remarks></remarks>
        Public UserAffiliates As List(Of Kamstrup.LicenseKeyGenerator.Model.clsUserAffiliates)
        ''' <summary>
        ''' Get the different licenses\ activations
        ''' </summary>
        ''' <remarks></remarks>
        Public LicenseList As List(Of Kamstrup.LicenseKeyGenerator.Model.clsLicense)

        Public CustomerList As List(Of Kamstrup.LicenseKeyGenerator.Model.clsCustomerInfo)

        ''' <summary>
        ''' The QLM webserver URL
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property QlmUrl As String
            Get
                Return m_qlmUrl.AbsoluteUri.ToString
            End Get
        End Property
        ''' <summary>
        ''' Getting the customer emails
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property CustomerEmails As DataSet
            Get
                Return License.GetCustomerEmails(m_qlmUrl.AbsoluteUri)
            End Get

        End Property


#End Region
        ''' <summary>
        ''' Create new license
        ''' Loading customers
        ''' Loading productslist
        ''' Loading users
        ''' loading licenses.
        ''' </summary>
        ''' <remarks></remarks>
        Sub New()

        End Sub

        Public Sub LoadLicenseInformation()
            'Setting the QLM license
            RaiseEvent evntLoadingmodule("Loading license data")
            License = New QlmLicense()
            LicenseInfo = New LicenseInfo()
            License.CommunicationEncryptionKey = Kamstrup.LicenseKeyGenerator.Constants.CommunicationEncryptionKey
            License.AdminEncryptionKey = Kamstrup.LicenseKeyGenerator.Constants.AdministrationEncryptionKey
            RaiseEvent evntLoadingmodule("Getting customer information")
            CustomerList = GetCustomers()
            RaiseEvent evntLoadingmodule("Getting product information")
            ProductsList = GetProducts()
            RaiseEvent evntLoadingmodule("Getting user information")
            UserAffiliates = GetUserAffiliates()
            RaiseEvent evntLoadingmodule("Getting license information")
            LicenseList = GetLicenses("")
            RaiseEvent evntLoadingmodule("Finished loading")
        End Sub

        ''' <summary>
        ''' Getting the users and the access of the user to the products
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function GetUserAffiliates() As List(Of Kamstrup.LicenseKeyGenerator.Model.clsUserAffiliates)
            Dim response As String = ""
            Dim dsAffiliates As String = ""
            Dim dsUserAffiliates As String = ""
            Dim datasetAffiliates As List(Of Kamstrup.LicenseKeyGenerator.Model.clsAffiliates)
            Dim datasetUserAffiliates As List(Of Kamstrup.LicenseKeyGenerator.Model.clsUserAffiliates)

            'Get the windows current user
            Dim currentUser As String = ""
            currentUser = Environment.UserName

            License.DownloadAffiliates(m_qlmUrl.AbsoluteUri, "1970-1-1", dsAffiliates, response)
            datasetAffiliates = Kamstrup.LicenseKeyGenerator.Model.clsAffiliates.DatasetMapper(dsAffiliates)
            License.DownloadUserAccounts(m_qlmUrl.AbsoluteUri, dsUserAffiliates, response)
            datasetUserAffiliates = Kamstrup.LicenseKeyGenerator.Model.clsUserAffiliates.DatasetMapper(dsUserAffiliates, datasetAffiliates)

            'to do Dim selectedUserAffiliates As List(Of Kamstrup.LicenseKeyGenerator.Model.clsUserAffiliates)
            Return datasetUserAffiliates.FindAll(Function(p) p.UserName.ToUpper = currentUser.ToUpper)

        End Function

       

#Region "Public functions"
        ''' <summary>
        ''' Update the licenses; Retrieve all licenses.
        ''' Property LicenseList is updated.
        ''' Nothing is returned
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub UpdateLicenseList()
            LicenseList = GetLicenses("")
        End Sub

        ''' <summary>
        ''' Update the licenses; Retrieve all licenses.
        ''' Property LicenseList is updated.
        ''' Nothing is returned
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub UpdateCustomerList()
            CustomerList = GetCustomers()
        End Sub

        ''' <summary>
        ''' Retrieving the licenses for the webserver 
        ''' </summary>
        ''' <param name="filter">Filter parameter</param>
        ''' <returns>List of licenses</returns>
        ''' <remarks></remarks>
        Public Function GetLicenses(filter As String) As List(Of Kamstrup.LicenseKeyGenerator.Model.clsLicense)
            Dim dataset As String = String.Empty
            Dim response As String = String.Empty

            modLicenseServer.License.GetDataSet(modLicenseServer.QlmUrl, filter, dataset, response)
            Return Kamstrup.LicenseKeyGenerator.Model.clsLicense.DatasetMapper(dataset, ProductsList, CustomerList)
        End Function

        ''' <summary>
        ''' Get the information of a specific customer
        ''' Search on email
        ''' </summary>
        ''' <param name="email">Email adres</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetCustomerInfo(email As String) As DataSet
            Dim response As String = ""
            Return License.GetCustomersInfo(m_qlmUrl.AbsoluteUri, "Email", "=", "'" & email & "'", response)
        End Function

        ''' <summary>
        ''' Getting the products on the QLM webserver
        ''' </summary>
        ''' <returns>Returns a list of prodcuts</returns>
        ''' <remarks></remarks>
        Public Function GetProducts() As List(Of Kamstrup.LicenseKeyGenerator.Model.clsProducts)
            Dim dsProducts As String = String.Empty
            Dim response As String = String.Empty
            Dim updateDate As Date

            License.DownloadProducts(m_qlmUrl.AbsoluteUri, updateDate, dsProducts, response)

            Return Kamstrup.LicenseKeyGenerator.Model.clsProducts.DatasetMapper(dsProducts)
            'TO do handling response
        End Function

        ''' <summary>
        ''' Get all customer information
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetCustomers() As List(Of Kamstrup.LicenseKeyGenerator.Model.clsCustomerInfo)
            Dim response As String = String.Empty
            Dim dsCustomers As DataSet
            dsCustomers = License.GetCustomersInfo(m_qlmUrl.AbsoluteUri, "", "", "", response)

            Return Kamstrup.LicenseKeyGenerator.Model.clsCustomerInfo.DatasetMapper(dsCustomers)
        End Function



#End Region

    End Module
End Namespace
