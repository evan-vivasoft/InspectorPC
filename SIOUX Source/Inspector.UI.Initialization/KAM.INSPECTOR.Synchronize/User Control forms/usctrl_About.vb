'===============================================================================
'Copyright Wigersma 2012
'All rights reserved.
'===============================================================================
Imports KAM.LicenceTool
Imports KAM.INSPECTOR.info.modLicenseInfo

Public Class usctrl_About
#Region "Constructor"
    ''' <summary>
    ''' Create new
    ''' Load license key data
    ''' </summary>
    ''' <remarks></remarks>
       Public Sub New()
        '' This call is required by the designer.
        InitializeComponent()

        SetInstalledComponents()
    End Sub
    ''' <summary>
    ''' Displaying the component information
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub Usctrl_About_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Dim versionInfo As Version = System.Reflection.Assembly.GetExecutingAssembly.GetName.Version

        Dim versionInfostr As String = versionInfo.Major & "." & versionInfo.Minor & "." & versionInfo.Build & "." & versionInfo.Revision
        rdlblVersion.Text += " " & versionInfostr & vbCrLf & "Copyright © 2020" & vbCrLf & "Wigersma && Sikkema B.V."

    End Sub
    ''' <summary>
    ''' Displaying the installed components
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub SetInstalledComponents()
        rdtxtbInstalledComponents.Text = ""
        rdtxtbInstalledComponents.Text += My.Resources.COMMUNICATORMainResx.str_Kamstrup_Infra & " " & KAM.COMMUNICATOR.Infra.clsGeneral.ComponentVersion & vbCrLf
        rdtxtbInstalledComponents.Text += My.Resources.COMMUNICATORMainResx.str_Kamstrup_Synchronize_database & " " & KAM.COMMUNICATOR.Synchronize.Bussiness.clsGeneral.ComponentVersion & vbCrLf
        rdtxtbInstalledComponents.Text += My.Resources.COMMUNICATORMainResx.str_Kamstrup_Synchronize_bussiness & " " & KAM.COMMUNICATOR.Synchronize.Infra.clsGeneral.ComponentVersion & vbCrLf
        rdtxtbInstalledComponents.Select(0, 0)
    End Sub
#End Region



End Class
