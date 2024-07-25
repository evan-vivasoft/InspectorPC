Imports Microsoft.Win32
Imports System.Management
Imports QlmLicenseLib 'InteractiveStudios.QlmLicenseLib
Imports System.Security.Permissions
Imports System.IO

Public Module LicenseValidator
    Public license As QlmLicense

    Public Event EvntErrorMessage(message As String)
    Public Event LicenseValidated()
    Public Event LicenseNotVallidated()
    Private ReadOnly regInfoFolder As String = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "ws-gas\" & _installShieldGuid)
#Region "Properties"
    ''' <summary>
    ''' Get or Set the application version 
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property ApplicationVersion() As String
        Get
            Return _applicationVersion
        End Get
        Set(ByVal value As String)
            _applicationVersion = value
        End Set
    End Property
    Private _applicationVersion As String

    ''' <summary>
    ''' Get or Set the customer name of the license from the configuration file
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property CustomerName() As String
        'MOD 55
        Set(value As String)
            My.License.Default.CustomerName = value
            My.License.Default.Save()
        End Set
        Get
            Return My.License.Default.CustomerName
        End Get

    End Property


    ''' <summary>
    ''' Get or set the customer company of the license from the configuration file
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property CustomerCompany() As String
        'MOD 55
        Set(value As String)
            My.License.Default.CompanyName = value
            My.License.Default.Save()
        End Set
        Get
            Return My.License.Default.CompanyName
        End Get

    End Property

#Region "Expired"
    'MOD 55
    'Public Sub ModifyRegistryKey(ByVal rKey As String, ByVal rValName As String, ByVal rValue As String)
    '    Exit Sub
    '    Dim registryKey As Microsoft.Win32.RegistryKey
    '    Try
    '        Dim f As New RegistryPermission(RegistryPermissionAccess.Read Or _
    '                            RegistryPermissionAccess.Write Or _
    '                            RegistryPermissionAccess.Create, "HKEY_LOCAL_MACHINE\" & rKey)

    '        registryKey = My.Computer.Registry.LocalMachine.OpenSubKey(rKey, True)

    '        If registryKey Is Nothing Then
    '            registryKey = My.Computer.Registry.LocalMachine.CreateSubKey(rKey)
    '        End If

    '        If registryKey.GetValue(rValName) Is Nothing _
    '        OrElse CStr(registryKey.GetValue(rValName)) <> rValue Then
    '            registryKey.SetValue(rValName, rValue)
    '            'My.Computer.Registry.SetValue(rClass & "\" & rKey, rValName, rValue)
    '        End If
    '    Catch ex As Exception
    '        MsgBox("No administrator rights for windows register" & vbCrLf & vbCrLf & ex.Message, vbCritical, Application.ProductName)
    '    End Try
    'End Sub
#End Region
    ''' <summary>
    ''' Corporate License file GUID
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property CorporateLicenseProductGUID As String
        Get
            Return _corporateLicenseProductGUID
        End Get
    End Property
    Private _corporateLicenseProductGUID As String = ""

    ''' <summary>
    ''' Corporate license Activation Code
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property CorporateLicenseComputerActivationCode As String
        Get
            Return _corporateLicenseComputerActivationCode
        End Get
    End Property
    Private _corporateLicenseComputerActivationCode As String = ""

    ''' <summary>
    ''' Corporate license Computer Key
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property CorporateLicenseComputerKey As String
        Get
            Return _corporateLicenseComputerKey
        End Get
    End Property
    Private _corporateLicenseComputerKey As String = ""

    ''' <summary>
    ''' Get the activationkey send by Wigersma. 
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property ComputerLicenseKey() As String
        Get
            'MOD 003 Dim actionvationKey As String = ""
            'MOD 003 Dim computerKey As String = ""
            'MOD 003 license.ReadKeys(actionvationKey, computerKey)
            Return _computerKey
        End Get

    End Property


    ''' <summary>
    ''' Get or set the computer activation key of the PC system of customer
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property ComputerActionvationKey() As String
        Get
            Return _computerActivationKey
        End Get
        Private Set(ByVal value As String)
            _computerActivationKey = value
            license.StoreKeys(value, ComputerLicenseKey)
        End Set
    End Property


    Private _computerKey As String 'The activationkey send by Wigersma (computer key)
    Private _computerActivationKey As String 'The computer key of the PC system of customer 

    ''' <summary>
    ''' Get the license product ID from QLM library
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property GetLicenseProductID()
        Get
            Return license.ProductID
        End Get
    End Property
    ''' <summary>
    ''' Get the license major and minor version from QLM library
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property GetLicenseversion()
        Get
            Return license.MajorVersion & "." & license.MinorVersion
        End Get
    End Property
    ''' <summary>
    ''' Get how many days of license left from QLM library.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property GetLicenseDaysLeft()
        Get
            Return license.DaysLeft
        End Get
    End Property
    ''' <summary>
    ''' Get the duration of the license from QLM library
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property GetLicenseDuration()
        Get
            Return license.Duration
        End Get
    End Property
    ''' <summary>
    ''' Get the expired date of the license from QLM library
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property GetLicenseExpiryDate()
        Get
            Return license.ExpiryDate
        End Get
    End Property


    ''' <summary>
    ''' Return the license status
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property LicenseStatus As ELicenseStatus
        Get
            Return _licenseStatus
        End Get
        Set(value As ELicenseStatus)
            _licenseStatus = value
        End Set
    End Property
    Private _licenseStatus As ELicenseStatus

    ''' <summary>
    ''' Get or Set the application product name 
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property ProductName() As String
        Get
            Return _productName
        End Get
        Set(ByVal value As String)
            _productName = value
        End Set
    End Property
    Private _productName As String

    ''' <summary>
    ''' MOD 01 Set the licensefile. This can be used for a corporate license. The file should be saved into the app data folder
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property LicenseFileName() As String
        Get
            Return _LicenseFileName
        End Get
        Set(ByVal value As String)
            _LicenseFileName = value
        End Set
    End Property
    Private _LicenseFileName As String = ""



    ''' <summary>
    ''' Get the ActivationCode
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property InstallShieldGuid() As String
        Get
            Return _installShieldGuid
        End Get
        Private Set(ByVal value As String)
            _installShieldGuid = value
        End Set
    End Property
    Private _installShieldGuid As String
#End Region

#Region "Constructor"
    ''' <summary>
    ''' New
    ''' </summary>
    ''' <remarks></remarks>
    Sub New()
        Try
            license = New QlmLicense
        Catch ex As Exception

        End Try
    End Sub
    ''' <summary>
    ''' Define the product to license
    ''' </summary>
    ''' <param name="productID"></param> 
    ''' <param name="productName"></param>
    ''' <param name="majorVersion"></param>
    ''' <param name="minorVersion"></param>
    ''' <param name="encryptionKey"></param>
    ''' <param name="persistenceKey"></param>
    ''' <remarks></remarks>
    Public Sub DefineProduct(ByVal productID As Integer, ByVal productName As String, ByVal majorVersion As Integer, ByVal minorVersion As Integer, ByVal encryptionKey As String, ByVal persistenceKey As String, ByVal publicKey As String, ByVal licenseVersion As String, ByVal installShieldGuid As String)
        license.DefineProduct(productID, productName, majorVersion, minorVersion, encryptionKey, persistenceKey)
        license.PublicKey = publicKey
        license.Version = licenseVersion
        'MOD 003
        If File.Exists(LicenseFileName) Then
            license.StoreKeysLocation = EStoreKeysTo.EFile
        Else
            license.StoreKeysLocation = EStoreKeysTo.ERegistry
        End If
        license.FavorMachineLevelLicenseKey = True 'MOD 70
        license.StoreKeysOptions = EStoreKeysOptions.EStoreKeysPerMachine 'MOD 70
        license.CommunicationEncryptionKey = modLicenseServerInfo.CommunicationEncryptionKey
        'MOD 39
        license.EvaluationPerUser = True 'MOD 003 Was false

        _installShieldGuid = installShieldGuid

        'Getting the computer ID en computer activaiton KEY
        GetLicenseinformation()

        'MOD 37 modSettingsFile.EncryptKey = publicKey
        'MOD 37 modSettingsFile.ReadData()
        'Get the activationCode (ComputerKey)
        GetActivationCodePC()
    End Sub

#End Region

#Region "License Handling"
    ''' <remarks>Call ValidateLicenseAtStartup when your application is launched. 
    ''' If this function returns false, exit your application.
    ''' </remarks>
    ''' <summary>
    ''' Validates the license when the application starts up. 
    ''' The first time a license key is validated successfully,
    ''' it is stored in a hidden file on the system. 
    ''' When the application is restarted, this code will load the license
    ''' key from the hidden file and attempt to validate it again. 
    ''' </summary>
    Public Sub ValidateLicenseAtStartup()
        Dim lTemp = String.Empty
        Dim computerKeyTemp = String.Empty

        license.ReadKeys(lTemp, computerKeyTemp)
        If computerKeyTemp = "" Then
            LicenseStatus = ELicenseStatus.EKeyNeedsActivation
            Exit Sub
        End If

        ValidateLicense(computerKeyTemp)
    End Sub

    ''' <remarks>Call this function in the dialog where the user enters the license key to validate the license.</remarks>
    ''' <summary>
    ''' Validates a license key. If you provide a computer key, the computer key is validated. 
    ''' Otherwise, the activation key is validated. 
    ''' If you are using machine bound keys (UserDefined), you can provide the computer identifier, 
    ''' otherwise set the computerID to an empty string.
    ''' </summary>
    ''' <param name="computerKey">Computer key</param>
    Public Sub ValidateLicense(ByVal computerKey As String)
        Dim ret As Boolean = False
        Try
            Dim nStatus As Integer
            'Dim errorMsg As String = license.ValidateLicenseEx(licenseKey, computerID)
            license.ValidateLicenseEx(computerKey, ComputerActionvationKey)
            nStatus = license.GetStatus()

            Select Case nStatus
                Case ELicenseStatus.EKeyDemo : LicenseStatus = ELicenseStatus.EKeyDemo
                Case ELicenseStatus.EKeyExpired : LicenseStatus = ELicenseStatus.EKeyExpired
                Case ELicenseStatus.EKeyInvalid : LicenseStatus = ELicenseStatus.EKeyInvalid
                Case ELicenseStatus.EKeyMachineInvalid : LicenseStatus = ELicenseStatus.EKeyMachineInvalid
                Case ELicenseStatus.EKeyNeedsActivation : LicenseStatus = ELicenseStatus.EKeyNeedsActivation
                Case ELicenseStatus.EKeyPermanent : LicenseStatus = ELicenseStatus.EKeyPermanent
                Case ELicenseStatus.EKeyProductInvalid : LicenseStatus = ELicenseStatus.EKeyProductInvalid
                Case ELicenseStatus.EKeyTampered : LicenseStatus = ELicenseStatus.EKeyTampered
                Case ELicenseStatus.EKeyVersionInvalid : LicenseStatus = ELicenseStatus.EKeyVersionInvalid
                Case ELicenseStatus.EKeyExceededAllowedInstances : LicenseStatus = ELicenseStatus.EKeyExceededAllowedInstances
                Case 68 : LicenseStatus = ELicenseStatus.EKeyExpired
                Case Else : LicenseStatus = ELicenseStatus.EKeyExpired
            End Select

            If IsTrue(nStatus, ELicenseStatus.EKeyInvalid) Or _
                IsTrue(nStatus, ELicenseStatus.EKeyProductInvalid) Or _
                IsTrue(nStatus, ELicenseStatus.EKeyVersionInvalid) Or _
                IsTrue(nStatus, ELicenseStatus.EKeyMachineInvalid) Or _
                IsTrue(nStatus, ELicenseStatus.EKeyTampered) Then
                ' the key is invalid
                ret = False
            ElseIf (IsTrue(nStatus, ELicenseStatus.EKeyDemo)) Then
                If (IsTrue(nStatus, ELicenseStatus.EKeyExpired)) Then
                    ' the key has expired
                    ret = False
                Else
                    ' the demo key is still valid
                    ret = True
                End If
            ElseIf (IsTrue(nStatus, ELicenseStatus.EKeyPermanent)) Then
                ' the key is OK
                ret = True
            ElseIf (IsTrue(nStatus, ELicenseStatus.EKeyNeedsActivation)) Then
                ' the key needs activation            
                ret = False
            End If

        Catch ex As Exception
            ret = False
            RaiseEvent EvntErrorMessage(ex.ToString)
        End Try
        If ret = True Then RaiseEvent LicenseValidated() Else RaiseEvent LicenseNotVallidated()
    End Sub

    ''' <summary>
    ''' Compares flags
    ''' </summary>
    ''' <param name="nVal1">Value 1</param>
    ''' <param name="nVal2">Value 2</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function IsTrue(ByVal nVal1 As Integer, ByVal nVal2 As Integer) As Boolean
        If (((nVal1 And nVal2) = nVal1) Or ((nVal1 And nVal2) = nVal2)) Then
            IsTrue = True
            Exit Function
        End If
        IsTrue = False
    End Function

    ''' <summary>
    ''' Create a evaluation version. the computer name is read out.
    ''' </summary>
    ''' <param name="durationDays"></param> The duration of the license
    ''' <remarks></remarks>
    Public Sub CreateEvalutionLicence(ByVal durationDays As Integer, ByVal privateKey As String)
        'Create an temperary durationdays days evaluation version.
        Dim lsLicensekey As String
        'Create a license key 
        license.PrivateKey = privateKey
        'MOD 39
        license.EvaluationPerUser = True 'MOD 003 Was false
        lsLicensekey = license.CreateLicenseKeyEx(Nothing, durationDays)
        ' lsLicensekey = license.CreateLicenseKeyEx2(Nothing, durationDays, 1, ELicenseType.ComputerName, ComputerKey)
        SaveLicenseKey(lsLicensekey)
    End Sub
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="computerLicenseKey"></param>The actual license key.
    ''' <remarks></remarks>
    Public Sub SaveLicenseKey(ByVal computerLicenseKey As String)
        'Save the license key to the settings file
        license.DeleteKeys()
        license.StoreKeys(ComputerActionvationKey, computerLicenseKey)
        GetLicenseinformation() 'MOD 003 refresh information
    End Sub
    ''' <summary>
    ''' Delete the license key.
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub DeleteLicenseKey()
        license.DeleteKeys()
    End Sub

    ''' <summary>
    ''' License; get the stored computer key
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub GetLicenseinformation()
        license.ReadKeys(_computerActivationKey, _computerKey)

    End Sub
#End Region

#Region "Online activation"
    Public Sub ActivateLicense(webServiceUrl As String, activationKey As String, computerID As String, computerName As String, qlmVersion As String, userData1 As String, ByRef response As String)
        license.ActivateLicense(webServiceUrl, activationKey, computerID, computerName, qlmVersion, userData1, response)
    End Sub
#End Region

#Region "Evaluation and Corporate license generate key"
    ''' <summary>
    ''' If keyNeedsActivation a 90 days evaluation version is created.
    ''' In case of Corporate license. Create license.
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub CreateEvaluationVersion(demoKey As String)
        If LicenseValidator.LicenseStatus = ELicenseStatus.EKeyNeedsActivation And LicenseValidator.ComputerLicenseKey = "" Then
            'MOD 004
            'In case of a corporate license file; Create license on startup
            If File.Exists(LicenseFileName) Then
                If _corporateLicenseComputerKey <> "" Then LicenseValidator.SaveLicenseKey(_corporateLicenseComputerKey)
            Else
                'If no license is defined, create an evaluation version
                'Set the licenseKey for 90 days evaluation 
                LicenseValidator.SaveLicenseKey(demoKey)
            End If
            LicenseValidator.ValidateLicenseAtStartup()
        End If
    End Sub

#End Region

#Region "Computer ID"
    ''' <summary>
    ''' Get the IP or CPU id. This is used for the license key. The CPU id is not always unique for a PC. 
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub GetActivationCodePC()
        Dim r As New Random
        Dim activationCodePC As String = String.Empty
        Dim objMos As ManagementObjectSearcher
        Dim objMoc As Management.ManagementObjectCollection
        Dim objMO As Management.ManagementObject

        'MOD 71
        'This can be used for a corporate license.
        If File.Exists(LicenseFileName) Then
            Dim objReader As New System.IO.StreamReader(LicenseFileName)
            Dim numberOflines As Integer = 0
            Dim line As String = "1"

            While (line <> Nothing)
                line = objReader.ReadLine()
                numberOflines += 1
                Select Case numberOflines
                    Case 1
                        _corporateLicenseProductGUID = line
                    Case 2
                        _corporateLicenseComputerActivationCode = line
                    Case 3
                        If line <> "" Then
                            _corporateLicenseComputerKey = line
                        Else
                            _corporateLicenseComputerKey = ""
                        End If
                End Select

            End While

            If _corporateLicenseProductGUID = license.GUID Then
                'MOD 003 Only set the license key incase it is not equal
                If ComputerActionvationKey <> _corporateLicenseComputerActivationCode Then ComputerActionvationKey = _corporateLicenseComputerActivationCode
            End If

            Exit Sub
        End If

        'Check if the ActivationCode already is saved in configuration file. This is done if a IP address is used from a dongle which does not exists
        If ComputerActionvationKey <> "" Then
            'Check if the machine name complies with the machine name save in the activation code
            If Left(ComputerActionvationKey.ToString, Len(Environment.MachineName & "-")) <> Environment.MachineName & "-" Then ComputerActionvationKey = "" Else Exit Sub
        End If

        'No current key exists.
        'Generate a new activation Code
        Try
            objMos = New ManagementObjectSearcher("Select * From Win32_NetworkAdapterConfiguration")

            'Finally, get the IP address id.
            For Each objMO In objMos.Get
                If objMO.Item("IPEnabled") = True Then
                    If InStr(UCase(objMO.Item("Caption")), "BLUETOOTH") = 0 Then
                        activationCodePC = objMO("MacAddress").ToString
                    End If
                End If
            Next

            objMos.Dispose()
            objMos = Nothing
            objMO.Dispose()
            objMO = Nothing
        Catch
            activationCodePC = r.Next(10000, 100000)
        End Try

        'If no IP address, then try to read out the CPU ID
        If activationCodePC = "" Then
            Try
                'Now, execute the query to get the results
                objMos = New ManagementObjectSearcher("Select * From Win32_Processor")

                objMoc = objMos.Get
                activationCodePC = ""
                'Finally, get the CPU's id.
                For Each objMO In objMoc
                    If activationCodePC = String.Empty Then
                        activationCodePC = objMO("ProcessorID").ToString
                    End If
                Next

                'Dispose object variables
                objMos.Dispose()
                objMos = Nothing
                objMO.Dispose()
                objMO = Nothing
            Catch
                r.Next(10000, 100000)
            End Try
        End If
        'Save the activation code in the settings file. This will be used next time
        If ComputerActionvationKey = "" Then ComputerActionvationKey = Environment.MachineName & "-" & activationCodePC
    End Sub
#End Region
End Module
