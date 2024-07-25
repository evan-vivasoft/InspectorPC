Imports Kamstrup.LKG.Kamstrup.LicenseKeyGenerator.Model

Imports Telerik.WinControls.UI
''' <summary>
''' Main application form
''' </summary>
''' <remarks></remarks>
Public Class frmMainLicense
#Region "Class members"
    Private WithEvents ucLicenseOrders As uctrl_LicenseOrders
    Private WithEvents ucLicenseActivation As uctrl_LicenseActivation

#End Region

    Private Sub frmMainLicense_FormClosing(sender As Object, e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        If Not IsNothing(RadPageView1.Pages("rdPageGenerateLicenseKey")) Then ucLicenseActivation.SaveCustomGrid()
        If Not IsNothing(RadPageView1.Pages("rdPageOrder")) Then ucLicenseOrders.SaveCustomGrid()


    End Sub


    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        'Setting the QLM URL

        Me.Text = "Wigermsa & Sikkema BV; QLM licenses"
        rdElementQLMUrl.Text = modLicenseServer.QlmUrl
        rdElementVersion.Text = "Version: " & My.Application.Info.Version.ToString
        rdElementUser.Text = "User: " & Environment.UserName

        Me.RadPageView1.ViewElement.Children(0).Children(1).Children(3).Visibility = Telerik.WinControls.ElementVisibility.Collapsed

        'Getting the user affiliates. This is used to get the user right.
        'If windows user has is no affiliates, the user can not generate orders. 
        'The user only generate license keys.
        'The affiliates are maintaned in the QLM console license tool
        If modLicenseServer.UserAffiliates.Count > 0 Then
            Me.ucLicenseOrders = New uctrl_LicenseOrders
            AddPage("Order / Activation keys", "rdPageOrder", ucLicenseOrders)
            'Else
            '    Me.rdPageOrder.r()
            '    Me.rdPageOrder.Item.Visibility = Telerik.WinControls.ElementVisibility.Hidden
        End If



        'Load form for license activation; Generate license keys
        Me.ucLicenseActivation = New uctrl_LicenseActivation
        AddPage("License keys", "rdPageGenerateLicenseKey", ucLicenseActivation)


    End Sub

    Private Sub AddPage(ByVal pageText As String, ByVal pageName As String, ByVal userControl As UserControl)
        Dim page As New RadPageViewPage()
        page.Text = pageText
        page.Name = pageName
        RadPageView1.Pages.Add(page)
        LoadPageView(page, userControl)
    End Sub



    ''' <summary>
    ''' Loading a user control to the telerik RadPageViewPage 
    ''' </summary>
    ''' <param name="pageSelected"></param>
    ''' <param name="userControl"></param>
    ''' <remarks></remarks>
    Private Sub LoadPageView(ByVal pageSelected As Telerik.WinControls.UI.RadPageViewPage, ByVal userControl As UserControl)
        pageSelected.Controls.Add(userControl)
        userControl.Dock = DockStyle.Fill
        userControl.Left = 0
        userControl.Top = 0
        userControl.Show()
    End Sub




End Class