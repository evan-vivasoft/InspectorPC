'MOD 84
Imports System.IO
Imports KAM.INSPECTOR.infra


Public Module modLicenseInfo

    Public Const QlmProductId As Integer = 8
    Public Const QlmProductName As String = "INSPECTOR PC"
    Public Const QlmProductMajorVersion As Integer = 5
    Public Const QlmProductMinorVersion As Integer = 0
    Public Const QlmProductEncryptKey As String = "757784-O11200"
    Public Const QlmProductGuid As String = "{A42769B5-75AC-4FA2-A90C-6C2C53F13C83}"
    Public Const QlmProductPublicKey As String = "H7ZaiUkOHl1xcw=="
    Public Const QlmLicenseVersion = "5.0.0.0"
    Public Const QlmDemoLicenseKey = "E8HD0-Q0M00-G5AG9-U8D48-8Q5AIH5UT"
    Public Const InstallshieldGuid = "{23BDBC3D-117A-4246-AD3F-2C8E8E98E6C0}"
    'MOD 84
    Public LicenseFileName As String = Path.Combine(ModuleSettings.SettingsFolder, "License\WS-IPC.lic")
End Module
