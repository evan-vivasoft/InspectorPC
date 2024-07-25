'===============================================================================
'Copyright Wigersma 2012
'All rights reserved.
'===============================================================================
Imports KAM.LicenceTool.My.Resources
Imports QlmLicenseLib
Imports Telerik.WinControls.UI

Public Class uscrtl_LicenseRequest
#Region "Constructor"
    ''' <summary>
    ''' Initialize component
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()
        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
    End Sub
    ''' <summary>
    ''' Load the information about license to the form
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub uscrtl_LicenseRequest_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        txtBoxName.Text = LicenseValidator.CustomerName
        txtBoxCustomer.Text = LicenseValidator.CustomerCompany
        txtBoxComputerKey.Text = LicenseValidator.ComputerActionvationKey
        'mod 003
        If LicenseValidator.CorporateLicenseComputerActivationCode <> "" Then
            txtBoxLicenseKey.Text = LicenseValidator.CorporateLicenseComputerKey
        Else
            txtBoxLicenseKey.Text = LicenseValidator.ComputerLicenseKey
        End If

        txtBoxDays.Text = LicenseValidator.GetLicenseDaysLeft
    End Sub
#End Region
#Region "public"
    ''' <summary>
    ''' Set the color of the labels
    ''' </summary>
    ''' <param name="lblcolor">label color</param>
    ''' <remarks></remarks>
    Public Sub SetLabelColor(lblcolor As Color)
        For Each ctl As Control In Me.Controls
            If (TypeOf ctl Is RadGroupBox) Then
                For Each ctl2 As Control In ctl.Controls
                    If (TypeOf ctl2 Is RadLabel) Then '(ctl.Name.StartsWith("lbl")) AndAlso
                        Dim lbl As RadLabel = DirectCast(ctl2, RadLabel)
                        lbl.ForeColor = lblcolor
                    End If
                Next
            ElseIf (TypeOf ctl Is RadLabel) Then
                Dim lbl As RadLabel = DirectCast(ctl, RadLabel)
                lbl.ForeColor = lblcolor
            End If
        Next ctl
    End Sub
#End Region
#Region "Form handling"
    ''' <summary>
    ''' Handling button activate license
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub cmbLicense_Click(sender As System.Object, e As System.EventArgs) Handles cmbLicense.Click
        ValidateLicense(txtBoxLicenseKey.Text)
    End Sub
    ''' <summary>
    ''' Handling button request license
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub cmbRequest_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbRequest.Click
        Try
            Clipboard.Clear()
        Catch ex As Exception
        End Try
        Try
            Clipboard.SetText(KamLicenseResx.str_Version & ": " & LicenseValidator.ApplicationVersion & vbCrLf & _
                            LicenseValidator.ProductName & " " & LicenseValidator.GetLicenseversion & vbCrLf & _
                            KamLicenseResx.str_Please_sent_the_mail_to & vbCrLf & _
                            vbCrLf & _
                            KamLicenseResx.str_Customer_name & ": " & txtBoxName.Text & vbCrLf & _
                            KamLicenseResx.str_Customer_company & ": " & txtBoxCustomer.Text & vbCrLf & _
                            KamLicenseResx.str_ActivationCode & ": " & rdTextActivationCode.Text & vbCrLf & _
                            KamLicenseResx.str_ComputerKey & ": " & txtBoxComputerKey.Text & vbCrLf & _
                            vbCrLf & _
                            "Du" & LicenseValidator.GetLicenseDuration & " - DL " & LicenseValidator.GetLicenseDaysLeft, TextDataFormat.UnicodeText)
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try

        LicenseValidator.CustomerCompany = txtBoxCustomer.Text
        LicenseValidator.CustomerName = txtBoxName.Text

        MsgBox(KamLicenseResx.str_The_information_is_copied_to & vbCrLf & KamLicenseResx.str_you_can_copy_the_information, MsgBoxStyle.Information, LicenseValidator.ProductName)
    End Sub
#End Region
    ''' <summary>
    ''' Check if hte received license key is valid.
    ''' If so save the key
    ''' For online activation the data is also updated.
    ''' </summary>
    ''' <param name="computerkey"></param>
    ''' <remarks></remarks>
    Private Sub ValidateLicense(computerkey As String)
        Dim errormsg As String
        errormsg = String.Empty
        'MOD 001
        LicenseValidator.ValidateLicense(computerkey)

        If LicenseValidator.LicenseStatus = ELicenseStatus.EKeyDemo Or LicenseValidator.LicenseStatus = ELicenseStatus.EKeyPermanent Then
            LicenseValidator.SaveLicenseKey(computerkey)
            LicenseValidator.CustomerCompany = txtBoxCustomer.Text
            LicenseValidator.CustomerName = txtBoxName.Text
            txtBoxLicenseKey.Text = LicenseValidator.ComputerLicenseKey
            MsgBox(KamLicenseResx.str_The_application_is_licensed, MsgBoxStyle.Information, LicenseValidator.ProductName)
        Else
            'MOD 002
            Select Case LicenseValidator.LicenseStatus
                Case ELicenseStatus.EKeyDemo : errormsg = KamLicenseResx.str_EKeyDemo
                Case ELicenseStatus.EKeyExpired : errormsg = KamLicenseResx.str_EKeyExpired
                Case ELicenseStatus.EKeyInvalid : errormsg = KamLicenseResx.str_EKeyInvalid
                Case ELicenseStatus.EKeyMachineInvalid : errormsg = KamLicenseResx.str_EKeyMachineInvalid
                Case ELicenseStatus.EKeyNeedsActivation : errormsg = KamLicenseResx.str_EKeyNeedsActivation
                Case ELicenseStatus.EKeyPermanent : errormsg = KamLicenseResx.str_EKeyPermanent
                Case ELicenseStatus.EKeyProductInvalid : errormsg = KamLicenseResx.str_EKeyProductInvalid
                Case ELicenseStatus.EKeyTampered : errormsg = KamLicenseResx.str_EKeyTampered
                Case ELicenseStatus.EKeyVersionInvalid : errormsg = KamLicenseResx.str_EKeyVersionInvalid
                Case ELicenseStatus.EKeyExceededAllowedInstances : errormsg = KamLicenseResx.str_EKeyExceededAllowInstances
                Case 68 : errormsg = KamLicenseResx.str_EKeyDemo
                Case Else : errormsg = KamLicenseResx.str_EKeyUnexpected
            End Select
            MsgBox(LicenseValidator.LicenseStatus & "; " & errormsg, MsgBoxStyle.Critical, LicenseValidator.ProductName)
        End If
    End Sub

    ''' <summary>
    ''' Handling of online activation
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub rdButOnlineActivation_Click(sender As System.Object, e As System.EventArgs) Handles rdButOnlineActivation.Click

        If rdTextActivationCode.Text = "" Then
            MsgBox(KamLicenseResx.str_NoActivationCode, vbInformation, LicenseValidator.ProductName)
            Exit Sub
        End If
        Dim response As String = ""
        Dim computerKey As String = ""
        Dim m_qlmUrl As Uri = New Uri(modLicenseServerInfo.Url)
        LicenseValidator.ActivateLicense(m_qlmUrl.ToString, rdTextActivationCode.Text, LicenseValidator.ComputerActionvationKey, "", "5.0.0.0", "", response)

        Dim licenseInfo As ILicenseInfo = New LicenseInfo()
        Dim message As String = String.Empty
        If LicenseValidator.license.ParseResults(response, licenseInfo, message) Then
            ValidateLicense(licenseInfo.ComputerKey)
        Else
            MsgBox(message, vbCritical, LicenseValidator.ProductName)
        End If


    End Sub

End Class
