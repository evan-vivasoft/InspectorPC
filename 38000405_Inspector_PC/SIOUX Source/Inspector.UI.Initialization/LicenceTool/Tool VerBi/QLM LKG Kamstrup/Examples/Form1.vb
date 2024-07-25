
Imports System.Net
Imports System.IO
Imports System.Text
Imports QlmLicenseLib
Imports System.Xml

Public Class Form1

    Dim license As QlmLicense
    Dim email As String

    Private Sub btnGo_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnGo.Click

        Dim errorMessage As String
        Dim response As String

        errorMessage = String.Empty


        response = GetResponseFromURL(txtUrlGetActivationKey.Text, errorMessage)
        txtOutput.Text = response
        txtActivationKey.Text = response
    End Sub

    Private Sub AddUser()

        Dim response As String

        license.AddUser(txtUrlQlmWeb.Text, "Jonathan Smith", email, "514-876-1234", "514-876-1235", "514-876-1236", _
                        "Soraco", "123 Interactive Road", "Suite 123", "Montreal", "Quebec", _
                        "h3p 3n4", "canada", "", "no comments", True, response)


        Dim licenseInfo As New LicenseInfo
        licenseInfo = New LicenseInfo()

        Dim message As String

        If license.ParseResults(response, licenseInfo, message) Then
        Else
            MessageBox.Show(message)
        End If
    End Sub

    Private Sub btnCreateActivationKey_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCreateActivationKey.Click
        AddUser()

        Dim response As String


        ' For the web method, we will strip out the URL arguments
        Dim uri As Uri
        uri = New Uri(txtUrlQlmWeb.Text)

        license.CreateActivationKey(uri.AbsoluteUri, email, 255, 1, True, "5.0.00", String.Empty, "my data", String.Empty, response)

        Dim licenseInfo As New LicenseInfo
        licenseInfo = New LicenseInfo()

        Dim message As String

        If license.ParseResults(response, licenseInfo, message) Then
            txtOutput.Text = licenseInfo.ActivationKey
            txtActivationKey.Text = licenseInfo.ActivationKey
        Else
            MessageBox.Show(message)
        End If

        txtOutput2.Text = response
    End Sub


    Private Function GetResponseFromURL(ByVal strURL As String, ByRef strError As String) As String

        Dim strResponse As String
        Dim webResponse As HttpWebResponse

        strResponse = String.Empty


        Try

            Dim webReq As HttpWebRequest
            webReq = WebRequest.Create(strURL)

            Dim proxyObject As IWebProxy

            proxyObject = WebProxy.GetDefaultProxy()

            webReq.Credentials = CredentialCache.DefaultCredentials
            webReq.Proxy = proxyObject

            webResponse = webReq.GetResponse()

            If Not webResponse.Equals(vbNull) Then

                Dim sr As StreamReader
                sr = New StreamReader(webResponse.GetResponseStream(), Encoding.ASCII)
                ' Convert the stream to a string
                strResponse = sr.ReadToEnd()
                sr.Close()
            End If

        Catch wex As WebException

            strError = wex.Message

        Catch ex As Exception

            strError = ex.Message

        Finally

            If Not webResponse.Equals(vbNull) Then

                webResponse.Close()
            End If
        End Try

        GetResponseFromURL = strResponse

    End Function

    Private Sub btnActivate_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnActivate.Click

        Dim response As String
        response = String.Empty

        license.ActivateLicenseEx(txtUrlQlmWeb.Text, txtActivationKey.Text, Environment.MachineName, _
                                Environment.MachineName, "5.0.00", "My data", String.Empty, response)

        txtOutput2.Text = response

        Dim licenseInfo As New LicenseInfo
        licenseInfo = New LicenseInfo()

        Dim message As String

        If license.ParseResults(response, licenseInfo, message) Then
            txtComputerKey.Text = licenseInfo.ComputerKey
        Else
            MessageBox.Show(message)
        End If


    End Sub


    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load


        license = New QlmLicense()
        license.CommunicationEncryptionKey = "{20132F19-A619-435F-A58D-50D83154B15A}"
        license.AdminEncryptionKey = "{41A752AC-CD13-4EA8-ADC7-6D7583BD9473}"
        email = "jsmith@hotmail.com"

        license.DefineProduct(8, "INSPECTOR PC", 5, 0, "757784-O11200", "{A42769B5-75AC-4FA2-A90C-6C2C53F13C83}")
        license.PublicKey = "H7ZaiUkOHl1xcw=="

    End Sub

    Private Sub Button1_Click(sender As System.Object, e As System.EventArgs) Handles Button1.Click
        Dim dataset, response As String
        response = String.Empty
        dataset = String.Empty
        Dim uri As Uri
        uri = New Uri(Kamstrup.LicenseKeyGenerator.Constants.Url)
        license.GetDataSet(uri.AbsoluteUri, "", dataset, response)


        Dim list As List(Of Kamstrup.LicenseKeyGenerator.Model.clsLicense) = datasetMapper(dataset)

        RadGridView1.DataSource = list
    End Sub


    Private Shared Function datasetMapper(dataSet As String) As List(Of Kamstrup.LicenseKeyGenerator.Model.clsLicense)
        Dim licenseList As New List(Of Kamstrup.LicenseKeyGenerator.Model.clsLicense)()
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
                                If node.Name.Equals("ReleaseCount") Then
                                    license.ReleaseCount = node.InnerText
                                End If
                                If node.Name.Equals("ComputerID") Then
                                    license.LicenseKey = node.InnerText
                                End If
                                If node.Name.Equals("UserData1") Then
                                    license.UserData = node.InnerText
                                End If

                                If node.Name.Equals("OrderID") Then
                                    license.OrderNo = node.InnerText
                                End If
                                If node.Name.Equals("OrderSTatus") Then
                                    license.OrderStatus = node.InnerText
                                End If
                                If node.Name.Equals("ActivationDate") Then
                                    license.ActivationDate = node.InnerText
                                End If
                                If node.Name.Equals("ProductID") Then
                                    Dim id As Integer
                                    Integer.TryParse(node.InnerText, id)
                                    license.ProductID = node.InnerText
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