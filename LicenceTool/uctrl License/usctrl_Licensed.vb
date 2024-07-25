'===============================================================================
'Copyright Wigersma 2012
'All rights reserved.
'===============================================================================
Public Class usctrl_Licensed
#Region "Constructor"
    ''' <summary>
    ''' Initialize component
    ''' </summary>
    ''' <remarks></remarks>
    Sub New()
        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
    End Sub
    ''' <summary>
    ''' On load; Loading the information 
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub usctrl_Licensed_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        txtBoxLicenseKey.Text = LicenseValidator.ComputerLicenseKey
    End Sub
#End Region
End Class
