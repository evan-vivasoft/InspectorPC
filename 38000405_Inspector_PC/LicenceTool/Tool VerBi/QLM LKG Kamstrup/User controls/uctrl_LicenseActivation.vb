Imports Kamstrup.LKG.Kamstrup.LicenseKeyGenerator.Model
Imports System.Linq
Imports Telerik.WinControls.UI
Imports System.IO


''' <summary>
''' QLM Computer key = for kamstrup b.v License key
''' Kamstrup b.v. Computer key = the QLM computer ID
''' </summary>
''' <remarks></remarks>
Public Class uctrl_LicenseActivation

#Region "Class members"
    Private loadProgram As Boolean = True 'Disable controls to be update
    Private dsCustomerEmails As DataSet

#End Region

#Region "Constructor"
    ''' <summary>
    ''' Loading the form
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub uctrl_LicenseActivation_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        loadProgram = True
        rdLicenseDate.CustomFormat = "dd-MM-yyyy"

        'Set the products
        SetProducts()
        SetCustomers()
        rdDropCustomerList.Text = "None"
        UpdateCustomerTextControl()
        loadProgram = False
        rdDropProducts.Items(1).Selected = True

        rdDropLanguage.Items.Add("Nederlands")
        rdDropLanguage.Items.Add("Deutsch")
        rdDropLanguage.Items.Add("English")
        rdDropLanguage.Text = "English"

    End Sub

#End Region


    ''' <summary>
    ''' Get the products from the license server and set them to the form
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub SetProducts()
        rdDropProducts.DataSource = modLicenseServer.ProductsList
        rdDropProducts.ValueMember = "Name"
        rdDropProducts.SortStyle = Telerik.WinControls.Enumerations.SortStyle.Ascending
    End Sub

#Region "Button handling"
    ''' <summary>
    ''' Button handling of generation a license keyd
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub rdButGenerateLicense_Click(sender As System.Object, e As System.EventArgs) Handles rdButGenerateLicense.Click
        Dim licenseKey As String = ""
        Dim qlmVersion As String = "5.0.00"
        Dim userData As String = ""

        If rdTxtComputerKey.Text = "" Then
            MsgBox("No computer key is empty!", vbExclamation, Application.ProductName)
            rdTxtComputerKey.Select()
            Exit Sub
        End If

        'Define the selected product
        Dim selectedProduct As Kamstrup.LicenseKeyGenerator.Model.clsProducts
        selectedProduct = ProductsList.Find(Function(p) p.Name = rdDropProducts.Text)

        'Define the selected product to create an activation Key
        modLicenseServer.License.DefineProduct(selectedProduct.ID, selectedProduct.Name, selectedProduct.MajorVersion, selectedProduct.MinorVersion, _
                              selectedProduct.Key, selectedProduct.GUID)
        modLicenseServer.License.PublicKey = selectedProduct.PublicKey

        If rdDropProducts.Text.ToUpper = "DIAGNOSTICS" Then
            licenseKey = GenerateLicenseDiagnostics(rdTxtComputerKey.Text)
            If licenseKey = "" Then Exit Sub
            'Set the user data, because for DIAGNOSTICS 2.0 the license key is not set by the QLM application
            'But it is stored in the QLM Database
            userData = String.Format("DIAGNOSTICS 2.0 license key: {0}", licenseKey)
            If userData = "" Then Exit Sub
        End If

        Dim licenseUpdateData As String = String.Format("<licenseArguments Email='{0}'/>", rdDropCustomerList.Text)

        Dim response As String = String.Empty
        modLicenseServer.License.UpdateLicenseInfo(modLicenseServer.QlmUrl, rdDropActivationKey.Text, licenseUpdateData, response)
        response = String.Empty

        'Generate a license key
        modLicenseServer.License.ActivateLicenseEx(modLicenseServer.QlmUrl, rdDropActivationKey.Text, rdTxtComputerKey.Text, _
                                "", qlmVersion, userData, String.Empty, response)


        Dim message As String = String.Empty

        If modLicenseServer.License.ParseResults(response, LicenseInfo, message) Then
            If rdDropProducts.Text.ToUpper = "DIAGNOSTICS" Then
                rdTxtLicenseKey.Text = licenseKey
            Else
                'For kamstrup b.v. = QLM computer key = license key
                rdTxtLicenseKey.Text = LicenseInfo.ComputerKey
            End If
            MsgBox(message, vbInformation, Application.ProductName)
            'LicenseListSelectedProductNoLisense()
        Else
            MsgBox(message, vbCritical, Application.ProductName)
        End If

    End Sub
    ''' <summary>
    ''' Button handling of sending email
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub btnSendKeysEmail_Click(sender As System.Object, e As System.EventArgs) Handles btnSendKeysEmail.Click
        If rdTxtLicenseKey.Text = "" Then
            MsgBox("No license key is generated.", vbExclamation, Application.ProductName)
            Exit Sub
        End If

        Dim subjectText As String = ""
        Dim emailFileName As String = ""
        Dim languageSet As String = "enGB"
        Select Case rdDropLanguage.Text
            Case "English"
                emailFileName = "enGB"
                subjectText = "License key for product "
            Case "Nederlands"
                languageSet = "nlNL"
                subjectText = "Licentiecode voor product "
            Case "Deutsch"
                languageSet = "dD"
                subjectText = "Lizenzschlüssel für Produkt "
            Case Else
                languageSet = "enGB"
                subjectText = "License key for product "
        End Select

        emailFileName = languageSet & "SendLicenseKeysEmail.txt"

        Dim sendText As String = ""
        Try
            Using sr As New StreamReader(Path.Combine(Application.StartupPath, "Text Files Email", emailFileName))
                Dim line As String
                line = sr.ReadToEnd()

                line = line.Replace("@PRODUCT", rdDropProducts.Text)
                line = line.Replace("@COMPUTERKEY", rdTxtComputerKey.Text)
                line = line.Replace("@ACTIVATIONKEY", rdDropActivationKey.Text)
                line = line.Replace("@LICENSEKEY", rdTxtLicenseKey.Text)
                sendText = line

            End Using
        Catch ex As Exception
            MsgBox(ex.Message, vbExclamation, Application.ProductName)
            Exit Sub
        End Try


        Dim objApp As Object
        Dim objEmail As Object
        objApp = CreateObject("Outlook.Application")
        objEmail = objApp.CreateItem(0)

        With objEmail
            .To = rdDropCustomerList.Text ' rdGridLicenses.CurrentRow.Cells("Email").Value
            .Subject = subjectText & rdDropProducts.Text
            .body = sendText
            .display()
        End With
        objEmail = Nothing
        objApp = Nothing
    End Sub
    ''' <summary>
    ''' Handling of sent activations keys of selected order number
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub rdButSendActivationKeys_Click(sender As System.Object, e As System.EventArgs) Handles rdButSendActivationKeys.Click
        Dim subjectText As String = ""
        Dim emailFileName As String = ""
        Dim languageSet As String = "enGB"
        Dim actionvationText As String = ""
        Select Case rdDropLanguage.Text
            Case "English"
                languageSet = "enGB"
                subjectText = "Activation key(s) for product @PRODUCT. Order number: @ORDERNUMBER "
                actionvationText = "Activation key "
            Case "Nederlands"
                languageSet = "nlNL"
                subjectText = "Activatiecode voor product @PRODUCT. Ordernummer: @ORDERNUMBER "
                actionvationText = "Activatiecode "
            Case "Deutsch"
                languageSet = "dD"
                subjectText = "Aktivierungskode für Produkt @PRODUCT. Bestellnummer: @ORDERNUMBER "
                actionvationText = "Aktivierungskode "
            Case Else
                languageSet = "enGB"
                subjectText = "Activation key(s) for product @PRODUCT. Order number: @ORDERNUMBER "
                actionvationText = "Activation key "
        End Select


        Dim selectedProduct As Kamstrup.LicenseKeyGenerator.Model.clsProducts

        selectedProduct = modLicenseServer.ProductsList.Find(Function(p) p.Name = rdDropProducts.Text)

        If IsNothing(selectedProduct) Then Exit Sub
        Dim selectedProductLicenses As List(Of Kamstrup.LicenseKeyGenerator.Model.clsLicense)
        selectedProductLicenses = modLicenseServer.LicenseList.FindAll(Function(p) p.ProductID = selectedProduct.ID And p.OrderNo = rdDropOrderNumber.Text And p.ComputerKey = "")
        Dim activationText As String = ""
        Dim selectedProductLicensesDistinctActivationKey
        If selectedProductLicenses.Count > 0 Then
            selectedProductLicensesDistinctActivationKey = From c In selectedProductLicenses Select c.ActivationKey Distinct

            Dim row

            Dim i = 1
            For Each row In selectedProductLicensesDistinctActivationKey

                activationText += actionvationText & i & ": " & row & vbCrLf
                i += 1
            Next
        Else
            MsgBox("Nothing to sent", MsgBoxStyle.Information, Application.ProductName)
            Exit Sub
        End If

        


        emailFileName = languageSet & "SendActivationKeysEmail.txt"

        Dim sendText As String = ""
        Try
            Using sr As New StreamReader(Path.Combine(Application.StartupPath, "Text Files Email", emailFileName))
                Dim line As String
                line = sr.ReadToEnd()

                line = line.Replace("@PRODUCT", rdDropProducts.Text)
                line = line.Replace("@ORDERNUMBER", rdDropOrderNumber.Text)
                line = line.Replace("@ACTIVATIONKEYS", activationText)
                sendText = line

                subjectText = subjectText.Replace("@PRODUCT", rdDropProducts.Text)
                subjectText = subjectText.Replace("@ORDERNUMBER", rdDropOrderNumber.Text)

            End Using
        Catch ex As Exception

            MsgBox(ex.Message, vbExclamation, Application.ProductName)
            Exit Sub
        End Try


        Dim objApp As Object
        Dim objEmail As Object
        objApp = CreateObject("Outlook.Application")
        objEmail = objApp.CreateItem(0)
        'Create the text to send


        With objEmail
            .To = rdGridLicenses.CurrentRow.Cells("Email").Value
            .Subject = subjectText
            .body = sendText
            .display()
        End With
        objEmail = Nothing
        objApp = Nothing
    End Sub

    ''' <summary>
    ''' Add customer to QLM database
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub rdButAddCustomer_Click(sender As System.Object, e As System.EventArgs) Handles rdButAddCustomer.Click
        Dim response As String = ""
        Dim message As String = ""
        Dim tmpCustomerEmail As String = ""

        If rdTxtFullName.Text = "" Then
            MsgBox("No name filled in", vbCritical, Application.ProductName)
            rdTxtFullName.Select()
            Exit Sub
        End If
        If rdDropCustomerList.Text = "" Then
            MsgBox("No email filled in", vbCritical, Application.ProductName)
            rdDropCustomerList.Select()
            Exit Sub
        End If
        If rdTxtCompany.Text = "" Then
            MsgBox("No company filled in", vbCritical, Application.ProductName)
            rdTxtCompany.Select()
            Exit Sub
        End If

        tmpCustomerEmail = rdDropCustomerList.Text
        modLicenseServer.License.AddUser(modLicenseServer.QlmUrl, rdTxtFullName.Text, rdDropCustomerList.Text, "", "", "", rdTxtCompany.Text, "", "", "", "", "", "", "", "", True, response)
        If modLicenseServer.License.ParseResults(response, LicenseInfo, message) Then MsgBox(message, vbInformation, Application.ProductName)
        'SetCustomers()
        rdDropCustomerList.Items.Add(tmpCustomerEmail)
        UpdateCustomerList()
    End Sub

    ''' <summary>
    ''' Handling of products selection change
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub rdDropProducts_SelectedIndexChanged(sender As System.Object, e As Telerik.WinControls.UI.Data.PositionChangedEventArgs) Handles rdDropProducts.SelectedIndexChanged
        If IsNothing(modLicenseServer.LicenseList) Then Exit Sub
        'Update the data
        LicenseListSelectedProductNoLicense()
    End Sub

    ''' <summary>
    ''' Handling of refresh list button
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub rdButRefreshLicenseList_Click(sender As System.Object, e As System.EventArgs) Handles rdButRefreshLicenseList.Click
        LicenseListSelectedProductNoLicense()
    End Sub
    ''' <summary>
    ''' Handling of button show all license of selected product
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub rdButShowAllOrderProduct_Click(sender As System.Object, e As System.EventArgs) Handles rdButShowAllOrderProduct.Click
        Dim selectedProduct As Kamstrup.LicenseKeyGenerator.Model.clsProducts

        selectedProduct = modLicenseServer.ProductsList.Find(Function(p) p.Name = rdDropProducts.Text)
        If IsNothing(selectedProduct) Then Exit Sub

        Dim selectedProductLicenses As List(Of Kamstrup.LicenseKeyGenerator.Model.clsLicense)
        selectedProductLicenses = modLicenseServer.LicenseList.FindAll(Function(p) p.ProductID = selectedProduct.ID)
        RefreshLicenseList(selectedProductLicenses)
    End Sub
    ''' <summary>
    ''' Handling of grid row change
    ''' The information is updated.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub rdGridLicenses_CurrentRowChanged(sender As Object, e As Telerik.WinControls.UI.CurrentRowChangedEventArgs) Handles rdGridLicenses.CurrentRowChanged
        If loadProgram = True Then Exit Sub
        If IsNothing(rdGridLicenses.CurrentRow) Then Exit Sub
        rdDropOrderNumber.Text = rdGridLicenses.CurrentRow.Cells("OrderNo").Value
        rdDropActivationKey.Text = rdGridLicenses.CurrentRow.Cells("ActivationKey").Value
        GetActivationKeyInformation(rdGridLicenses.CurrentRow.Cells("ActivationKey").Value)
        rdTxtComputerKey.Text = rdGridLicenses.CurrentRow.Cells("ComputerKey").Value
        rdDropCustomerList.Text = rdGridLicenses.CurrentRow.Cells("Email").Value
        UpdateCustomerTextControl()
        If rdDropProducts.Text = "DIAGNOSTICS" Then
            Dim strArr() As String
            strArr = rdGridLicenses.CurrentRow.Cells("UserData").Value.ToString.Split(":")
            If strArr.Length > 1 Then rdTxtLicenseKey.Text = strArr(1) Else rdTxtLicenseKey.Text = ""
        Else
            rdTxtLicenseKey.Text = rdGridLicenses.CurrentRow.Cells("LicenseKey").Value
        End If
    End Sub
    ''' <summary>
    ''' Handling of activation key change
    ''' The grid is updated
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub rdDropActivationKey_SelectedIndexChanged(sender As System.Object, e As Telerik.WinControls.UI.Data.PositionChangedEventArgs) Handles rdDropActivationKey.SelectedIndexChanged
        For Each row As GridViewRowInfo In rdGridLicenses.Rows
            If row.Cells("ActivationKey").Value = rdDropActivationKey.Text Then
                row.IsCurrent = True
            End If
        Next
    End Sub
    ''' <summary>
    ''' Check if the license key is valid
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub rdButValidatieKey_Click(sender As System.Object, e As System.EventArgs) Handles rdButValidatieKey.Click
        'Define the selected product
        Dim selectedProduct As Kamstrup.LicenseKeyGenerator.Model.clsProducts
        selectedProduct = ProductsList.Find(Function(p) p.Name = rdDropProducts.Text)

        'Define the selected product to create an activation Key
        modLicenseServer.License.DefineProduct(selectedProduct.ID, selectedProduct.Name, selectedProduct.MajorVersion, selectedProduct.MinorVersion, _
                              selectedProduct.Key, selectedProduct.GUID)
        modLicenseServer.License.PublicKey = selectedProduct.PublicKey

        Dim keyInvalidText As String
        Dim lbKeyCheck As Boolean
        keyInvalidText = modLicenseServer.License.ValidateLicenseEx(rdTxtLicenseKey.Text, rdTxtComputerKey.Text)
        lbKeyCheck = modLicenseServer.License.IsValid
        If lbKeyCheck = True Then
            MsgBox("The license key is valid", vbInformation, Application.ProductName)
        Else
            MsgBox(keyInvalidText, vbCritical, Application.ProductName)
        End If
    End Sub
    ''' <summary>
    ''' Handling of changing the customer. 
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub rdDropCustomerList_SelectedIndexChanged(sender As System.Object, e As Telerik.WinControls.UI.Data.PositionChangedEventArgs) Handles rdDropCustomerList.SelectedIndexChanged
        If loadProgram = True Then Exit Sub
        UpdateCustomerTextControl()
    End Sub
#End Region
#Region "Private functions"
    ''' <summary>
    ''' Generate a license key for the CONNEXION module DIAGNOSTICS.
    ''' This is a now QLM license
    ''' </summary>
    ''' <param name="computerKey"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function GenerateLicenseDiagnostics(computerKey As String) As String
        'In case of DIAGNOSTICS another license key is generated.
        Dim lsHarddiskNumber As String
        Dim lsActivationCode(5)
        Dim lsDatumCode(3)
        If Strings.Left(computerKey, 2) = "HD" Then
            lsActivationCode(0) = "FE"
        Else
            MsgBox("DIAGNOSTICS is not installed on a Hard disk", vbExclamation, Application.ProductName)
            Return ""
        End If

        lsActivationCode(1) = Hex(Asc(Mid(computerKey, 13, 1))) + Hex(15) ''Hex(Hard disk drive letter) + F
        lsHarddiskNumber = Mid(computerKey, 15, Len(computerKey) - 14)
        If Len(lsHarddiskNumber) > 6 Then
            lsActivationCode(2) = Hex(Mid(computerKey, 8, 2) + Asc(Strings.Left(lsHarddiskNumber, 1))) 'DD
            lsActivationCode(3) = Hex(Mid(computerKey, 4, 2) + 10 + Asc(Mid(lsHarddiskNumber, 3, 1))) 'MM
            lsActivationCode(4) = Hex(Mid(computerKey, 6, 2) + 25 + Asc(Mid(lsHarddiskNumber, 5, 1))) 'YY(1)
            If rdRadioLicenseType2.IsChecked = True Then
                Dim lsdatum As Date
                If rdRadioLicenseEval1.IsChecked = True Then
                    lsdatum = Now.AddDays(Trim(rdLicenseDays.Text))

                ElseIf rdRadioLicenseEval2.IsChecked = True Then
                    lsdatum = rdLicenseDate.Text
                End If

                lsActivationCode(5) = Hex(Format(lsdatum, "yyyyMMdd") + 19274316)
            End If
        End If

        ' Build and show activationcode
        If rdRadioLicenseType2.IsChecked = True Then
            Return (lsActivationCode(1) + "-" + lsActivationCode(4) + lsActivationCode(2) + "-" + lsActivationCode(0) + lsActivationCode(3)) & "!" + lsActivationCode(5)
        Else
            Return (lsActivationCode(1) + "-" + lsActivationCode(4) + lsActivationCode(2) + "-" + lsActivationCode(0) + lsActivationCode(3))
        End If

    End Function

    ''' <summary>
    ''' Set the customer information to the dropdown list
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub SetCustomers()
        'Set the customer information
        rdDropCustomerList.DataSource = modLicenseServer.CustomerEmails.Tables(0)
        rdDropCustomerList.ValueMember = "Email"
        rdDropCustomerList.SortStyle = Telerik.WinControls.Enumerations.SortStyle.Ascending
    End Sub

    ' ''' <summary>
    ' ''' Set the properties of the grid
    ' ''' </summary>
    ' ''' <remarks></remarks>
    'Private Sub GridViewLayout()
    '    With Me.rdGridLicenses
    '        .BeginUpdate()
    '        .MasterTemplate.Reset()
    '        .TableElement.RowHeight = 20
    '        .AutoGenerateHierarchy = True
    '        .UseScrollbarsInHierarchy = False
    '        .MultiSelect = False
    '        .MasterTemplate.BestFitColumns()

    '        .ReadOnly = True
    '        .EnableFiltering = True
    '        .EnableGrouping = True
    '        .EnableSorting = True
    '        .EnableAlternatingRowColor = True
    '        .AllowColumnHeaderContextMenu = False
    '        .AllowCellContextMenu = False
    '        .TableElement.RowHeight = 40

    '        .EndUpdate()
    '    End With
    'End Sub

    ''' <summary>
    ''' Get the data of all activation keys with no license
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub LicenseListSelectedProductNoLicense()
        If loadProgram = True Then Exit Sub
        modLicenseServer.UpdateLicenseList()
        rdDropOrderNumber.Text = ""
        rdDropActivationKey.Text = ""
        Dim selectedProduct As Kamstrup.LicenseKeyGenerator.Model.clsProducts

        selectedProduct = modLicenseServer.ProductsList.Find(Function(p) p.Name = rdDropProducts.Text)

        If IsNothing(selectedProduct) Then Exit Sub
        Dim selectedProductLicenses As List(Of Kamstrup.LicenseKeyGenerator.Model.clsLicense)
        selectedProductLicenses = modLicenseServer.LicenseList.FindAll(Function(p) p.ProductID = selectedProduct.ID And p.OrderNo <> "" And p.ComputerKey = "")
        Dim selectedProductLicensesDistinctOrderNumber
        Dim selectedProductLicensesDistinctActivationKey
        If selectedProductLicenses.Count > 0 Then
            selectedProductLicensesDistinctOrderNumber = From c In selectedProductLicenses Select c.OrderNo Distinct
            selectedProductLicensesDistinctActivationKey = From c In selectedProductLicenses Select c.ActivationKey Distinct
        Else
            selectedProductLicensesDistinctOrderNumber = selectedProductLicenses
            selectedProductLicensesDistinctActivationKey = selectedProductLicenses

        End If


        RefreshLicenseList(selectedProductLicenses)

        rdDropOrderNumber.DataSource = selectedProductLicensesDistinctOrderNumber
        rdDropOrderNumber.ValueMember = "OrderNo"
        rdDropOrderNumber.SortStyle = Telerik.WinControls.Enumerations.SortStyle.Ascending

        'rdGridLicenses.DataSource = selectedProductLicensesDistinct
        rdDropActivationKey.DataSource = selectedProductLicensesDistinctActivationKey
        rdDropActivationKey.ValueMember = "ActivationKey"
        rdDropActivationKey.SortStyle = Telerik.WinControls.Enumerations.SortStyle.Ascending
    End Sub


    ''' <summary>
    ''' Refresh the data in the grid
    ''' </summary>
    ''' <param name="selectedProductLicenses"></param>
    ''' <remarks></remarks>
    Private Sub RefreshLicenseList(selectedProductLicenses As List(Of Kamstrup.LicenseKeyGenerator.Model.clsLicense))
        modLicenseServer.UpdateLicenseList()

        loadProgram = True
        With rdGridLicenses
            .Columns.Clear()
            .TableElement.BeginUpdate()
            .MasterTemplate.EnableFiltering = True
            .MasterTemplate.AutoExpandGroups = True
            .DataSource = Nothing
            .DataSource = selectedProductLicenses

            .Columns.Add("FullName", "FullName")
            .Columns.Add("Email", "Email")
            .Columns.Add("Company", "Company")
            'MDO 29
            .Columns("FullName").FieldName = "Customer.FullName" 'DO NOT TRANSLATE
            .Columns("Email").FieldName = "Customer.Email" 'DO NOT TRANSLATE
            .Columns("Company").FieldName = "Customer.Company"  'DO NOT TRANSLATE

            .Columns("ProductID").IsVisible = False
            .Columns("Customer").IsVisible = False

            .ReadOnly = True
            .TableElement.EndUpdate()
            loadProgram = False
            LoadCustomGrid()
        End With
    End Sub

    ''' <summary>
    ''' Check if a activation key a permanent, duration or an expiry date activation
    ''' </summary>
    ''' <param name="activationCode"></param>
    ''' <remarks></remarks>
    Private Sub GetActivationKeyInformation(activationCode As String)
        'Define the selected product
        Dim selectedProduct As Kamstrup.LicenseKeyGenerator.Model.clsProducts
        selectedProduct = ProductsList.Find(Function(p) p.Name = rdDropProducts.Text)

        'Define the selected product to create an activation Key
        modLicenseServer.License.DefineProduct(selectedProduct.ID, selectedProduct.Name, selectedProduct.MajorVersion, selectedProduct.MinorVersion, _
                              selectedProduct.Key, selectedProduct.GUID)
        modLicenseServer.License.PublicKey = selectedProduct.PublicKey

        modLicenseServer.License.ValidateLicense(activationCode)

        Dim duration As Integer = 0
        duration = modLicenseServer.License.Duration
        Dim defaultDate As Date = #1/1/1970#
        Dim expiryDate As Date = defaultDate

        expiryDate = modLicenseServer.License.ExpiryDate
        If duration > 0 Then
            rdGroupLicenseEval.Visible = True
            rdRadioLicenseType2.IsChecked = True
            rdRadioLicenseEval1.IsChecked = True
            rdLicenseDays.Value = duration
        ElseIf expiryDate > defaultDate Then
            rdGroupLicenseEval.Visible = True

            rdRadioLicenseType2.IsChecked = True
            rdRadioLicenseEval2.IsChecked = True
            rdLicenseDate.Value = expiryDate
        Else
            rdGroupLicenseEval.Visible = False
            rdRadioLicenseType1.IsChecked = True
        End If

    End Sub
    ''' <summary>
    ''' Updating the customer information when user selects a customer in the dropdown
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub UpdateCustomerTextControl()
        Dim dsCustomer As System.Data.DataSet
        dsCustomer = modLicenseServer.GetCustomerInfo(rdDropCustomerList.Text)
        If IsNothing(dsCustomer) Then Exit Sub
        If dsCustomer.Tables(0).Rows.Count = 0 Then Exit Sub
        rdTxtCompany.Text = dsCustomer.Tables(0).Rows(0).Item("Company").ToString
        rdTxtFullName.Text = dsCustomer.Tables(0).Rows(0).Item("FullName").ToString
    End Sub
#End Region

#Region "Grid handling"
    Public Sub SaveCustomGrid()
        If rdGridLicenses.RowCount < 1 Then Exit Sub

        If Not Directory.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Kamstrup\LKG\UserSettings")) Then
            Directory.CreateDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Kamstrup\LKG\UserSettings"))
        End If
        rdGridLicenses.SaveLayout(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Kamstrup\LKG\UserSettings", Me.Name & "_" & rdGridLicenses.Name & ".xml"))

    End Sub

    Private Sub LoadCustomGrid()
        If File.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Kamstrup\LKG\UserSettings", Me.Name & "_" & rdGridLicenses.Name & ".xml")) Then
            rdGridLicenses.LoadLayout(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Kamstrup\LKG\UserSettings", Me.Name & "_" & rdGridLicenses.Name & ".xml"))
        End If
    End Sub
#End Region


End Class
