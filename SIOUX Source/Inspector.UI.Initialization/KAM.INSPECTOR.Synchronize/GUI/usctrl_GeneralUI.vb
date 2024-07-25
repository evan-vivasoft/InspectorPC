'===============================================================================
'Copyright Wigersma 2013
'All rights reserved.
'===============================================================================

Imports System.IO
Imports Microsoft.Win32
Imports Telerik.WinControls
Imports KAM.COMMUNICATOR.Infra
Imports KAM.Infra

Public Class usctrl_GeneralUI
#Region "Class members"

    'Selected theme name
    Private themeName As String = enumTelerikThemes.Office2007Black
    Private Const PathRapidll As String = "C:\Windows\System32\Rapi.dll"
    Private allowChangeCheck As Boolean = False

    Public Class enumTelerikThemes
        Public Const Aqua As String = "Aqua"
        Public Const Breeze As String = "Breeze"
        Public Const Desert As String = "Desert"
        Public Const HighContrastBlack As String = "HighContrastBlack"
        Public Const Office2007Black As String = "Office2007Black"
        Public Const Office2007Silver As String = "Office2007Silver"
        Public Const Office2010Black As String = "Office2010Black"
        Public Const Office2010Blue As String = "Office2010Blue"
        Public Const Office2010Silver As String = "Office2010Silver"
        Public Const TelerikMetro As String = "TelerikMetro"
        Public Const TelerikMetroBlue As String = "TelerikMetroBlue"
        Public Const TelerikMetroTouch As String = "TelerikMetroTouch"
        Public Const VisualStudio2012Dark As String = "VisualStudio2012Dark"
        Public Const VisualStudio2012Light As String = "VisualStudio2012Light"
        Public Const Windows7 As String = "Windows7"
    End Class
#End Region

#Region "Constructor"
    ''' <summary>
    ''' Create new instance
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()
        ' This call is required by the designer.
        InitializeComponent()
        Dim pathSyncData As String = SettingFile.GetSetting(GsSectionApplication, GsSettingApplicatioPathSyncData)
        'Relative or absolute path
        pathSyncData = Path.GetFullPath(pathSyncData)

        rdBrowsCommunicatorPath.Value = pathSyncData
        ' Add any initialization after the InitializeComponent() call.
    End Sub
    ''' <summary>
    ''' Loading the form. Intitialize theme
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub usctrl_GeneralUI_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        SetThemeName()
        LoadThemes()
        CheckInspectorAvailable()
    End Sub
#End Region
#Region "Theming"
    ''' <summary>
    ''' Set the theme for the GUI 
    ''' Telerik themes only
    ''' The settings is in the key "Localization", "ThemeName"
    ''' Default theme is officeblack
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub SetThemeName()
        'Get the theme name from the settings file. If no value, Office2007Black theme is loaded
        themeName = ModuleCommunicatorSettings.SettingFile.GetSetting(GsSectionLocalization, GsSettingLocalizationThemeName)
        If themeName = ModuleCommunicatorSettings.SettingFile.NoValue Or themeName Is Nothing Then
            themeName = enumTelerikThemes.Office2007Black
        End If
        'Assign theme to UI
        ThemeResolutionService.ApplicationThemeName = themeName
    End Sub
    ''' <summary>
    ''' Loading the different telerik themes'
    ''' Remark: Before loading the Theme's, Read out the selected theme name from the settings file.
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub LoadThemes()
        rDropDownThemesSelection.Items.Add(enumTelerikThemes.Aqua)
        rDropDownThemesSelection.Items.Add(enumTelerikThemes.Breeze)
        rDropDownThemesSelection.Items.Add(enumTelerikThemes.Desert)
        rDropDownThemesSelection.Items.Add(enumTelerikThemes.HighContrastBlack)
        rDropDownThemesSelection.Items.Add(enumTelerikThemes.Office2007Black)
        rDropDownThemesSelection.Items.Add(enumTelerikThemes.Office2007Silver)
        rDropDownThemesSelection.Items.Add(enumTelerikThemes.Office2010Black)
        rDropDownThemesSelection.Items.Add(enumTelerikThemes.Office2010Blue)
        rDropDownThemesSelection.Items.Add(enumTelerikThemes.Office2010Silver)
        rDropDownThemesSelection.Items.Add(enumTelerikThemes.TelerikMetro)
        rDropDownThemesSelection.Items.Add(enumTelerikThemes.TelerikMetroBlue)
        rDropDownThemesSelection.Items.Add(enumTelerikThemes.TelerikMetroTouch)
        rDropDownThemesSelection.Items.Add(enumTelerikThemes.VisualStudio2012Dark)
        rDropDownThemesSelection.Items.Add(enumTelerikThemes.VisualStudio2012Light)
        rDropDownThemesSelection.Items.Add(enumTelerikThemes.Windows7)
        'Assign selected theme to the selection box
        rDropDownThemesSelection.Text = themeName.ToString
    End Sub
#End Region
#Region "Check Available Inspector"
    Private Sub CheckInspectorAvailable()

        Dim windowsRegisterKey As String = ""

        '{23BDBC3D-117A-4246-AD3F-2C8E8E98E6C0} is the GUID of the installation of INSPECTOR PC
        If Environment.Is64BitOperatingSystem Then
            windowsRegisterKey = "HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\UnInstall\{23BDBC3D-117A-4246-AD3F-2C8E8E98E6C0}"
        Else
            windowsRegisterKey = "HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\UnInstall\{23BDBC3D-117A-4246-AD3F-2C8E8E98E6C0}"
        End If
        allowChangeCheck = True
        If CheckFileExistsPC(PathRapidll) Then rdchkInspectorPda.Checked = True Else rdchkInspectorPda.Checked = False
        If Registry.GetValue(windowsRegisterKey, "InstallLocation", "") <> "" Then rdchkInspectorPc.Checked = True Else rdchkInspectorPc.Checked = False
        allowChangeCheck = False
    End Sub


#End Region

#Region "Form handling"
    ''' <summary>
    ''' Handling of change index 
    ''' The selected theme is saved into the settings file
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub rDropDownThemesSelection_SelectedIndexChanged(sender As System.Object, e As Telerik.WinControls.UI.Data.PositionChangedEventArgs) Handles rDropDownThemesSelection.SelectedIndexChanged
        'Assign theme to UI
        If themeName <> rDropDownThemesSelection.Text.ToString Then
            themeName = rDropDownThemesSelection.Text.ToString
            ThemeResolutionService.ApplicationThemeName = themeName
            'Saving the setting to the settings file
            ModuleCommunicatorSettings.SettingFile.SaveSetting(GsSectionLocalization, GsSettingLocalizationThemeName) = themeName
        End If
    End Sub
    ''' <summary>
    ''' Disable that the user can change th state of the checkbox
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="args"></param>
    ''' <remarks></remarks>
    Private Sub RadCheckBox1_ToggleStateChanging(ByVal sender As Object, ByVal args As Telerik.WinControls.UI.StateChangingEventArgs) Handles rdchkInspectorPc.ToggleStateChanging, rdchkInspectorPda.ToggleStateChanging
        If allowChangeCheck = False Then args.Cancel = True
    End Sub
    'MOD 09
    ''' <summary>
    ''' Handling of changing the data path of COMMUNICATOR
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub rdBrowsCommunicatorPath_ValueChanged(sender As System.Object, e As System.EventArgs) Handles rdBrowsCommunicatorPath.ValueChanged
        If SettingFile.GetSetting(GsSectionApplication, GsSettingApplicatioPathSyncData) <> rdBrowsCommunicatorPath.Value Then
            SettingFile.SaveSetting(GsSectionApplication, GsSettingApplicatioPathSyncData) = rdBrowsCommunicatorPath.Value
        End If
    End Sub
#End Region

End Class
