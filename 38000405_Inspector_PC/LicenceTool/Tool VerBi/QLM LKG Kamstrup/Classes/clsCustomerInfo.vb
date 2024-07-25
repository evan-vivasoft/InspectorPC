Imports System.Xml
Imports System.IO

Namespace Kamstrup.LicenseKeyGenerator.Model
    Public Class clsCustomerInfo
        Public Property CustomerID() As String
            Get
                Return m_CustomerID
            End Get
            Set(value As String)
                m_CustomerID = value
            End Set
        End Property
        Private m_CustomerID As String
        Public Property FullName() As String
            Get
                Return m_FullName
            End Get
            Set(value As String)
                m_FullName = value
            End Set
        End Property
        Private m_FullName As String
        Public Property Company() As String
            Get
                Return m_Company
            End Get
            Set(value As String)
                m_Company = value
            End Set
        End Property
        Private m_Company As String
        Public Property Email() As String
            Get
                Return m_Email
            End Get
            Set(value As String)
                m_Email = value
            End Set
        End Property
        Private m_Email As String


        Public Shared Function DatasetMapper(dataSet As DataSet) As List(Of clsCustomerInfo)
            Dim customerList As New List(Of clsCustomerInfo)()
            Dim dr As DataRow
            Dim dt As DataTable

            dt = dataSet.Tables(0)

            For Each dr In dt.Rows
                Dim customer As New Kamstrup.LicenseKeyGenerator.Model.clsCustomerInfo()
                customer.CustomerID = dr("CustomerID").ToString
                customer.FullName = dr("FullName").ToString
                customer.Email = dr("Email").ToString
                customer.Company = dr("Company").ToString
                customerList.Add(customer)
            Next

            Return customerList
        End Function
    End Class

End Namespace