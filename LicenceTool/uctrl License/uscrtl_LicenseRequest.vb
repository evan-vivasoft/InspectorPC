'===============================================================================
'Copyright Wigersma 2012
'All rights reserved.
'===============================================================================
Imports Inspector.Infra.Ioc
Imports Inspector.POService.LicenseValidator
Imports KAM.LicenceTool.My.Resources
Imports QlmLicenseLib
Imports Telerik.WinControls.UI

Public Class uscrtl_LicenseRequest
    Private _poLicenseValidator As POLicenseValidator
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

    End Sub
#End Region

    Public Function HandleRestOfTheLogic(licenseStatus As String, expiresIn As Int64)
        If expiresIn > 0 Then
            licenseStatusText.Text = licenseStatus
            expiresInText.Text = expiresIn
            MsgBox("Information successfully verified. You can restart the app")
        Else
            MsgBox("License Expired. Please contact with your token provider")
        End If
    End Function

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim maybeToken = VerificationTokenInput.Text
        POLicenseValidator.ProcessVerificationToken(maybeToken, AddressOf HandleRestOfTheLogic)
    End Sub
    Public ReadOnly Property POLicenseValidator As IPOLicenseValidator
        Get
            If _poLicenseValidator Is Nothing Then
                _poLicenseValidator = ContextRegistry.Context.Resolve(Of IPOLicenseValidator)
            End If
            Return _poLicenseValidator
        End Get
    End Property

End Class
