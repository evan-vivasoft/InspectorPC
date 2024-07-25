Imports Kamstrup.LKG.Kamstrup.LicenseKeyGenerator.Model
Imports Telerik.WinControls.UI
Imports System.IO
''' <summary>
''' Handling of creating orders and activation keys
''' The activation keys are stored in the QLM database 
''' </summary>
''' <remarks></remarks>
Public Class uctrl_LicenseOrders
#Region "Class members"
    Private loadProgram As Boolean = True 'Disable controls to be update
#End Region

#Region "Form Initialization"

    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        loadProgram = True
        rdLicenseDate.CustomFormat = "dd-MM-yyyy"
        rdLicenseDate.Value = Now.AddDays(90)
        SetCustomers()
        SetProductsUser()
        loadProgram = False

        rdDropCustomerList.Text = "None"
        UpdateCustomerTextControl()
        'GridViewLayout()
        refreshLicenseList()

        rdTxtOrderNumber.Select()

        rdDropLanguage.Items.Add("Nederlands")
        rdDropLanguage.Items.Add("Deutsch")
        rdDropLanguage.Items.Add("English")
        rdDropLanguage.Text = "English"
    End Sub


#End Region

#Region "Button Handling"
    ''' <summary>
    ''' Handling of changing the customer. 
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub rdDropCustomerList_SelectedIndexChanged_1(sender As System.Object, e As Telerik.WinControls.UI.Data.PositionChangedEventArgs) Handles rdDropCustomerList.SelectedIndexChanged
        If loadProgram = True Then Exit Sub
        UpdateCustomerTextControl()
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
    ''' Handling of creating an activation key
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub rdCreateKey_Click(sender As System.Object, e As System.EventArgs) Handles rdCreateKey.Click
        Dim response As String = ""
        Dim qlmVersion As String = "5.0.00"
        rdActivationKeys.Text = ""

        If rdTxtOrderNumber.Text = "" Then
            MsgBox("No order number filled in", vbCritical, Application.ProductName)
            rdTxtOrderNumber.Select()
            Exit Sub
        End If
        'search selected product in productsList
        Dim selectedProduct As Kamstrup.LicenseKeyGenerator.Model.clsProducts
        selectedProduct = ProductsList.Find(Function(p) p.Name = rdDropProducts.Text)

        'Define the selected product to create an activation Key
        modLicenseServer.License.DefineProduct(selectedProduct.ID, selectedProduct.Name, selectedProduct.MajorVersion, selectedProduct.MinorVersion, _
                              selectedProduct.Key, selectedProduct.GUID)
        modLicenseServer.License.PublicKey = selectedProduct.PublicKey

        'Create the order

        'For UNITOOL another QLM version is used
        If rdDropProducts.Text.ToUpper = "UNITOOL" Then qlmVersion = "4.0.00"

        'TO do remove test demo

        If rdRadioLicenseType1.ToggleState = Telerik.WinControls.Enumerations.ToggleState.On Then
            'Permanent license
            modLicenseServer.License.CreateOrder(modLicenseServer.QlmUrl, rdDropCustomerList.Text, 255, rdSpinAmountOfLicenses.Value, False, qlmVersion, String.Empty, "", String.Empty, Nothing, -1, rdTxtOrderNumber.Text, 2, response)
        ElseIf rdRadioLicenseType2.ToggleState = Telerik.WinControls.Enumerations.ToggleState.On Then
            'Evaluation license
            If rdRadioLicenseEval1.ToggleState = Telerik.WinControls.Enumerations.ToggleState.On Then
                'Amount of days
                modLicenseServer.License.CreateOrder(modLicenseServer.QlmUrl, rdDropCustomerList.Text, 255, rdSpinAmountOfLicenses.Value, False, qlmVersion, String.Empty, "", String.Empty, Nothing, rdLicenseDays.Value, rdTxtOrderNumber.Text, 2, response)
            ElseIf rdRadioLicenseEval2.ToggleState = Telerik.WinControls.Enumerations.ToggleState.On Then
                'End date.
                modLicenseServer.License.CreateOrder(modLicenseServer.QlmUrl, rdDropCustomerList.Text, 255, rdSpinAmountOfLicenses.Value, False, qlmVersion, String.Empty, "", String.Empty, rdLicenseDate.Text, -1, rdTxtOrderNumber.Text, 2, response)
            End If
        End If


        'Display the message and keys
        Dim message As String = ""

        If modLicenseServer.License.ParseResults(response, modLicenseServer.LicenseInfo, message) Then
            Dim aryActivationKey() As String
            aryActivationKey = LicenseInfo.ActivationKey.Split(";")
            rdActivationKeys.Text = String.Format("Activation key for {0}.{1}Order number: {2}.{1}Amount of Activation keys {3}.{1}", selectedProduct.Name, vbNewLine, rdTxtOrderNumber.Text, UBound(aryActivationKey) + 1)

            For i As Integer = 0 To UBound(aryActivationKey)
                If i <> 0 Then rdActivationKeys.Text += vbCrLf
                rdActivationKeys.Text += "Activation Key " & i + 1 & ": " & aryActivationKey(i).Replace("-", "")
            Next i
            MsgBox(message.Replace("-", ""), vbInformation, Application.ProductName)
            'MOD 1.0.0.4
            modLicenseServer.UpdateLicenseList()
            refreshLicenseList()
        Else
            MsgBox(message, vbCritical, Application.ProductName)
        End If

    End Sub
    ''' <summary>
    ''' Copy the activation keys to clipboard
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub rdCopyToClipboard_Click(sender As System.Object, e As System.EventArgs) Handles rdCopyToClipboard.Click
        Clipboard.SetDataObject(rdActivationKeys.Text)
        MsgBox("Activation keys copied to clipboard", vbInformation, Application.ProductName)
    End Sub
    ''' <summary>
    ''' Sent the activation keys by email
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub rdButSendActivationKeys_Click(sender As System.Object, e As System.EventArgs) Handles rdButSendActivationKeys.Click

        Dim subjectText As String = ""
        Dim emailFileName As String = ""
        Dim languageSet As String = "enGB"
        Dim activation1TranslateText As String = ""
        Dim activation2TranslateText As String = ""
        Select Case rdDropLanguage.Text
            Case "English"
                languageSet = "enGB"
                subjectText = "Activation key(s) for product @PRODUCT. Order number: @ORDERNUMBER "
                activation1TranslateText = "Number of Activation key (s)"
                activation2TranslateText = "Activation key"

            Case "Nederlands"
                languageSet = "nlNL"
                subjectText = "Activatiecode voor product @PRODUCT. Ordernummer: @ORDERNUMBER "
                activation1TranslateText = "Aantal activatiecode(s)"
                activation2TranslateText = "Activatiecode"
            Case "Deutsch"
                languageSet = "dD"
                subjectText = "Aktivierungskode für Produkt @PRODUCT. Bestellnummer: @ORDERNUMBER "
                activation1TranslateText = "Anzalh Aktivierungskode(s)"
                activation2TranslateText = "Aktivierungskode"
            Case Else
                languageSet = "enGB"
                subjectText = "Activation key(s) for product @PRODUCT. Order number: @ORDERNUMBER "
                activation1TranslateText = "Number of Activation key (s)"
                activation2TranslateText = "Activation key"
        End Select

        emailFileName = languageSet & "SendActivationKeysEmail.txt"

        Dim sendText As String = ""
        Try
            Using sr As New StreamReader(Path.Combine(Application.StartupPath, "Text Files Email", emailFileName))
                Dim line As String
                line = sr.ReadToEnd()

                line = line.Replace("@PRODUCT", rdDropProducts.Text)
                line = line.Replace("@ORDERNUMBER", rdTxtOrderNumber.Text)


                Dim newListActivationKeys() As String
                newListActivationKeys = rdActivationKeys.Lines


                Dim activationText As String = ""
                Dim replaceText As String = ""
                For i As Integer = 2 To UBound(newListActivationKeys)
                    If i > 2 Then activationText += vbCrLf
                    replaceText = newListActivationKeys(i).Replace("Amount of Activation keys", activation1TranslateText)
                    replaceText = replaceText.Replace("Activation Key", activation2TranslateText)
                    activationText += replaceText
                Next i

                line = line.Replace("@ACTIVATIONKEYS", activationText)
                sendText = line

                subjectText = subjectText.Replace("@PRODUCT", rdDropProducts.Text)
                subjectText = subjectText.Replace("@ORDERNUMBER", rdTxtOrderNumber.Text)

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
            .To = rdDropCustomerList.Text
            .Subject = subjectText
            .body = sendText
            .display()
        End With
        objEmail = Nothing
        objApp = Nothing
    End Sub
#End Region

    ''' <summary>
    ''' Set the products of the windows user
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub SetProductsUser()
        'Setting the product list depending on the user access rights

        Dim productsListUser As New List(Of Kamstrup.LicenseKeyGenerator.Model.clsProducts)
        For i As Integer = 0 To modLicenseServer.UserAffiliates(0).ProductsName.Count - 1
            For j As Integer = 0 To ProductsList.Count - 1
                If ProductsList(j).Name.Equals(modLicenseServer.UserAffiliates(0).ProductsName(i).ToString.Remove(modLicenseServer.UserAffiliates(0).ProductsName(i).Length - 4, 4)) Then
                    productsListUser.Add(ProductsList(j))
                    Exit For
                End If
            Next

        Next

        'Apply list to drop down
        rdDropProducts.DataSource = productsListUser
        rdDropProducts.ValueMember = "Name"
        rdDropProducts.SortStyle = Telerik.WinControls.Enumerations.SortStyle.Ascending
    End Sub

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


    ''' <summary>
    ''' Set the properties of the grid
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub GridViewLayout()
        With Me.rdGridLicenses
            '.MasterTemplate.AllowAutoSizeColumns = Telerik.WinControls.UI.GridViewAutoSizeColumnsMode.None
            '.TableElement.BeginUpdate()
            '.MasterTemplate.EnableFiltering = True
            '.MasterTemplate.AutoExpandGroups = True
            '.TableElement.EndUpdate()
            '.BeginUpdate()
            '.MasterTemplate.Reset()
            '.TableElement.RowHeight = 20
            '.AutoGenerateHierarchy = False
            '.UseScrollbarsInHierarchy = False
            '.MultiSelect = True
            '.MasterTemplate.BestFitColumns()
            '.AutoExpandGroups = True
            '.ReadOnly = True
            '.EnableFiltering = True
            '.EnableGrouping = True
            '.EnableSorting = True
            '.EnableAlternatingRowColor = True
            '.AllowColumnHeaderContextMenu = False
            '.AllowCellContextMenu = False
            '.TableElement.RowHeight = 40
            '.EndUpdate()
        End With
    End Sub



    Private Sub rdButDeleteLicense_Click(sender As System.Object, e As System.EventArgs) Handles rdButReleaseLicense.Click
        Dim response As String = ""
        Dim message As String = ""
        Dim ok
        ok = MsgBox("Are you sure you want to release this license?", vbYesNo + vbExclamation, Application.ProductName)
        If ok = vbNo Then Exit Sub

        modLicenseServer.License.ReleaseLicense(modLicenseServer.QlmUrl, rdGridLicenses.CurrentRow.Cells("ActivationKey").Value, rdGridLicenses.CurrentRow.Cells("ComputerKey").Value, rdGridLicenses.CurrentRow.Cells("LicenseKey").Value, response)
        If modLicenseServer.License.ParseResults(response, modLicenseServer.LicenseInfo, message) Then
            MsgBox(message, vbInformation, Application.ProductName)
        Else
            MsgBox(message, vbCritical, Application.ProductName)
        End If
        'MOD 1.0.0.4
        modLicenseServer.UpdateLicenseList()
        refreshLicenseList()
    End Sub

    Private Sub rdButRefreshLicenseList_Click(sender As System.Object, e As System.EventArgs) Handles rdButRefreshLicenseList.Click
        'MOD 1.0.0.4
        modLicenseServer.UpdateLicenseList()
        refreshLicenseList()
    End Sub

    Private Sub refreshLicenseList()
        ''MOD 1.0.0.4 modLicenseServer.UpdateLicenseList()
        With rdGridLicenses
            .Columns.Clear()
            '.MasterTemplate.AllowAutoSizeColumns = Telerik.WinControls.UI.GridViewAutoSizeColumnsMode.Fill
            .TableElement.BeginUpdate()
            .MasterTemplate.EnableFiltering = True
            .MasterTemplate.AutoExpandGroups = True
            .DataSource = modLicenseServer.LicenseList

            .Columns.Add("FullName", "FullName")
            .Columns.Add("Email", "Email")
            .Columns.Add("Company", "Company")
            'MDO 29
            .Columns("FullName").FieldName = "Customer.FullName" 'DO NOT TRANSLATE
            .Columns("Email").FieldName = "Customer.Email" 'DO NOT TRANSLATE
            .Columns("Company").FieldName = "Customer.Company" '"GclBoundary.UOV" 'DO NOT TRANSLATE

            .Columns("ProductID").IsVisible = False
            .Columns("Customer").IsVisible = False
            .ReadOnly = True

            .TableElement.EndUpdate()
            LoadCustomGrid()
        End With

    End Sub



    Private Sub rdRadioLicenseType1_ToggleStateChanged(sender As System.Object, args As Telerik.WinControls.UI.StateChangedEventArgs) Handles rdRadioLicenseType1.ToggleStateChanged
        If rdRadioLicenseType1.ToggleState = Telerik.WinControls.Enumerations.ToggleState.On Then rdGroupLicenseEval.Enabled = False
    End Sub

    Private Sub rdRadioLicenseType2_ToggleStateChanged(sender As System.Object, args As Telerik.WinControls.UI.StateChangedEventArgs) Handles rdRadioLicenseType2.ToggleStateChanged
        If rdRadioLicenseType2.ToggleState = Telerik.WinControls.Enumerations.ToggleState.On Then rdGroupLicenseEval.Enabled = True
    End Sub

    Private Sub rdRadioLicenseEval1_ToggleStateChanged(sender As System.Object, args As Telerik.WinControls.UI.StateChangedEventArgs) Handles rdRadioLicenseEval1.ToggleStateChanged
        If rdRadioLicenseEval1.ToggleState = Telerik.WinControls.Enumerations.ToggleState.On Then
            rdLicenseDate.Enabled = False
            rdLicenseDays.Enabled = True
        End If

    End Sub

    Private Sub rdRadioLicenseEval2_ToggleStateChanged(sender As System.Object, args As Telerik.WinControls.UI.StateChangedEventArgs) Handles rdRadioLicenseEval2.ToggleStateChanged
        If rdRadioLicenseEval2.ToggleState = Telerik.WinControls.Enumerations.ToggleState.On Then
            rdLicenseDate.Enabled = True
            rdLicenseDays.Enabled = False
        End If

    End Sub

    Public Sub SaveCustomGrid()
        If rdGridLicenses.RowCount < 1 Then Exit Sub

        If Not Directory.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Kamstrup\LKG\UserSettings")) Then
            Directory.CreateDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Kamstrup\LKG\UserSettings"))
        End If
        rdGridLicenses.SaveLayout(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Kamstrup\LKG\UserSettings", Me.Name & "_" & rdGridLicenses.Name & ".xml"))

    End Sub

    Private Sub LoadCustomGrid()
        If File.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Kamstrup\LKG\UserSettings", Me.Name & "_" & rdGridLicenses.Name & ".xml")) Then
            'MOD XXX rdGridLicenses.LoadLayout(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Kamstrup\LKG\UserSettings", Me.Name & "_" & rdGridLicenses.Name & ".xml"))
        End If
    End Sub

End Class
