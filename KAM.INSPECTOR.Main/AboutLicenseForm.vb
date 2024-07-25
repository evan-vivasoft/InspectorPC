'===============================================================================
'Copyright Wigersma 2012
'All rights reserved.
'===============================================================================
Imports Telerik.WinControls.UI
Imports System.Windows.Forms
Imports QlmLicenseLib
Imports KAM.LicenceTool
Public Class AboutLicenseForm
#Region "Class members"
    'Private WithEvents ucTranslationSelection As usctrl_TranslationSelection
    Private WithEvents ucAbout As usctrl_About
    Private WithEvents ucLicenceRequest As uscrtl_LicenseRequest
#End Region
#Region "Constructor"
    ''' <summary>
    ''' Initialize form
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()
        ' This call is required by the designer.
        InitializeComponent()
        ' Add any initialization after the InitializeComponent() call.
    End Sub
#End Region
#Region "User control"
    ''' <summary>
    ''' Loading a user control to one of the pages of radpageview
    ''' </summary>
    ''' <param name="pageSelected"></param>
    ''' <param name="userControl"></param>
    ''' <remarks></remarks>
    Private Sub loadUserControl(ByVal pageSelected As RadPageViewPage, ByVal userControl As UserControl)
        'Loading the usercontrol into the toolwindow
        If pageSelected.Controls.Count > 0 Then
            'If any usercontrol already exists in the toolwindow. Dispose it.
            For i As Integer = 1 To pageSelected.Controls.Count
                Dim control As Control = pageSelected.Controls(i - 1)
                If control IsNot Nothing Then
                    control.Dispose()
                End If
            Next
        End If

        pageSelected.Controls.Add(userControl)
        userControl.Dock = DockStyle.Fill
        userControl.Left = 0
        userControl.Top = 0
        userControl.Show()
    End Sub
    ''' <summary>
    ''' Loading the different user controls to the form
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub AboutLicenseForm_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        ucAbout = New usctrl_About
        loadUserControl(PageAbout, ucAbout)

        'If a permanent license is optained. the license page is not displayed
        If LicenseValidator.LicenseStatus <> ELicenseStatus.EKeyPermanent Then
            ucLicenceRequest = New uscrtl_LicenseRequest
            loadUserControl(PageRequestLicense, ucLicenceRequest)
        Else
            'MOD 94
            If RadPageView1.Pages.Contains(PageRequestLicense) Then
                RadPageView1.Pages.Remove(PageRequestLicense)
                'PageRequestLicense.IsContentVisible = False
            End If
        End If
    End Sub
#End Region


End Class
