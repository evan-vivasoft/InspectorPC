'===============================================================================
'Copyright Wigersma & Sikkema 2012
'All rights reserved.
'===============================================================================
Imports KAM.INSPECTOR.Main.My.Resources
Imports QlmLicenseLib
Imports KAM.LicenceTool
Imports KAM.INSPECTOR.info.modLicenseInfo
Imports Microsoft.Win32
Imports KAM.INSPECTOR.Infra


Module General


#Region "Constructor"
    ''' <summary>
    ''' Load the license
    ''' </summary>
    ''' <remarks></remarks>
    Sub New()
        'MOD 83
        DebugGUILogger = log4net.LogManager.GetLogger("INSPECTORGUILogger")
        DebugGUILogger.Info("Application Started")

        'MOD 84 Define file name for Corporate license
        LicenseValidator.LicenseFileName = LicenseFileName
        LicenseValidator.DefineProduct(QlmProductId, QlmProductName, QlmProductMajorVersion, QlmProductMinorVersion, QlmProductEncryptKey, QlmProductGuid, QlmProductPublicKey, QlmLicenseVersion, InstallshieldGuid)
        LicenseValidator.ProductName = QlmProductName
        LicenseValidator.ApplicationVersion = System.Reflection.Assembly.GetExecutingAssembly.GetName.Version.ToString
        'LicenseValidator.DeleteLicenseKey() 'Use for testing only. It removes the current license keys.
        LicenseValidator.ValidateLicenseAtStartup()
        LicenseValidator.CreateEvaluationVersion(QlmDemoLicenseKey)

        'MOD 83
        log4net.Config.XmlConfigurator.Configure()

        'MOD 32
        Try
            '{23BDBC3D-117A-4246-AD3F-2C8E8E98E6C0} is the GUID of the installation
            If Environment.Is64BitOperatingSystem Then
                If Registry.GetValue("HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\UnInstall\" & InstallshieldGuid, "License computerID", "0") <> LicenseValidator.ComputerActionvationKey Then
                    Registry.SetValue("HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\UnInstall\" & InstallshieldGuid, "License computerID", LicenseValidator.ComputerActionvationKey)
                End If
            Else
                If Registry.GetValue("HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\UnInstall\" & InstallshieldGuid, "License computerID", "0") <> LicenseValidator.ComputerActionvationKey Then
                    Registry.SetValue("HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\UnInstall\" & InstallshieldGuid, "License computerID", LicenseValidator.ComputerActionvationKey)
                End If
            End If
        Catch ex As Exception
            'MOD 87 MsgBox(INSPECTORMainResx.str_NoAdministratorAccessToRegister & vbCrLf & vbCrLf & ex.Message, vbCritical, Application.ProductName)
        End Try
    End Sub
    Public Sub LoadLicense()

    End Sub
#End Region


End Module
