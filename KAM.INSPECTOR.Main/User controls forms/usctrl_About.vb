'===============================================================================
'Copyright Wigersma 2012
'All rights reserved.
'===============================================================================
Imports KAM.INSPECTOR.Main.My.Resources
Imports Inspector.BusinessLogic
Imports QlmLicenseLib
Imports KAM.LicenceTool
Imports KAM.INSPECTOR.info.modLicenseInfo
Imports Inspector.POService.LicenseValidator.POLicenseValidator
Imports Inspector.POService.LicenseValidator
Imports System.Drawing.Text



Public Class usctrl_About
#Region "Constructor"
    ''' <summary>
    ''' Create new
    ''' Load license key data
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()
        Dim infra As New Infra.clsGeneral
        Dim versionInfo As Version = System.Reflection.Assembly.GetExecutingAssembly.GetName.Version
        Dim a = New POLicenseValidator
        ' This call is required by the designer.
        InitializeComponent()
        rdLabelInspector.Text = QlmProductName
        rdlblVersion.Text += " " & versionInfo.ToString & vbCrLf & "Copyright © 2020" & vbCrLf & "Wigersma && Sikkema B.V."

        rdlblLicense.Text = INSPECTORMainResx.str_License_to & vbCrLf & LicenseValidator.CustomerName & vbCrLf & "" & LicenseValidator.CustomerCompany & vbCrLf

        rdlblLicenceStatus.Text += vbCrLf & LicenseValidator.LicenseStatus

        'MOD 67
        rdlblLicenceComputerKey.Text += vbCrLf & LicenseValidator.ComputerActionvationKey.ToString
        Console.WriteLine(a.ActualDeviceKey)
        If LicenseValidator.LicenseStatus = ELicenseStatus.EKeyDemo Then rdlblLicenceStatus.Text += vbCrLf & INSPECTORMainResx.str_demo_version_days_left & " " & LicenseValidator.GetLicenseDaysLeft & vbCrLf
        If LicenseValidator.LicenseStatus = ELicenseStatus.EKeyDemo Or LicenseValidator.LicenseStatus = ELicenseStatus.EKeyPermanent Then
        Else
            rdlblLicenceStatus.Text += vbCrLf & INSPECTORMainResx.str_license_expired
        End If
        SetInstalledComponents()
        rdtxtbInstalledComponents.TextBoxElement.TextBoxItem.BackColor() = Color.Transparent
    End Sub
    ''' <summary>
    ''' Displaying the component information
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub Usctrl_About_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub

    Private Sub SetInstalledComponents()
        'MOD 49
        'rdrtbInstalledComponents.IsReadOnly = True
        'rdrtbInstalledComponents.ChangeFontSize(12)
        'rdrtbInstalledComponents.ChangeParagraphLineSpacing(0.5)
        'Dim text As String = ""

        rdtxtbInstalledComponents.Text = ""

        rdtxtbInstalledComponents.Text += INSPECTORMainResx.str_Kamstrup_inspectieprocedure & " " & IP.clsGeneral.ComponentVersion & vbCrLf
        rdtxtbInstalledComponents.Text += INSPECTORMainResx.str_Kamstrup_PLEXOR_component & " " & PLEXOR.clsGeneral.ComponentVersion & vbCrLf
        rdtxtbInstalledComponents.Text += INSPECTORMainResx.str_Kamstrup_PRS_component & " " & PRS.clsGeneral.ComponentVersion & vbCrLf
        rdtxtbInstalledComponents.Text += INSPECTORMainResx.str_BusinessLogic_component & " " & VersionInformation.ComponentVersion & vbCrLf
        'rdtxtbInstalledComponents.Text += INSPECTORMainResx.str_Kamstrup_Infra_component & componentInformationInfra.ComponentVersion & vbCrLf
        'rdtxtbInstalledComponents.Text += "Kamstrup Result component: " & componentInformationResult.ComponentVersion & vbCrLf
        'rdrtbInstalledComponents.Insert(text)
        rdtxtbInstalledComponents.Select(0, 0)
    End Sub
#End Region
End Class
