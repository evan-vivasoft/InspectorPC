'===============================================================================
'Copyright Wigersma 2012
'All rights reserved.
'===============================================================================
Imports Telerik.WinControls
Imports KAM.INSPECTOR.Infra

Public Class usctrl_GeneralUI
#Region "Class members"

    'Selected theme name
    Private themeName As String = enumTelerikThemes.Office2007Black
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

        'MOD 06
        'Set the text sizes
        For i = 8 To 26 Step 2
            rDropDownInstructionTextSize.Items.Add(i)
        Next i
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

        'MOD 06
        Dim textSize As Integer
        textSize = ModuleSettings.SettingFile.GetSetting(GsSectionText, GsSettingTextSize)
        rDropDownInstructionTextSize.Text = textSize

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
        themeName = ModuleSettings.SettingFile.GetSetting(GsSectionLocalization, GsSettingLocalizationThemeName)
        If themeName = ModuleSettings.SettingFile.NoValue Or themeName Is Nothing Then
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
    Public Sub LoadThemes()

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
            ModuleSettings.SettingFile.SaveSetting(GsSectionLocalization, GsSettingLocalizationThemeName) = themeName
        End If
    End Sub
    'MOD 06
    ''' <summary>
    ''' Save if the text size is changed
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub rDropDownInstructionTextSize_SelectedIndexChanged(sender As System.Object, e As Telerik.WinControls.UI.Data.PositionChangedEventArgs) Handles rDropDownInstructionTextSize.SelectedIndexChanged
        ModuleSettings.SettingFile.SaveSetting(GsSectionText, GsSettingTextSize) = rDropDownInstructionTextSize.Text
    End Sub
#End Region


    Private Sub SetrdCheckSync()
        If ModuleSettings.SettingFile.GetSetting(GsSectionFunctions, GsSettingFunctionsSynchronize) = True Or ModuleSettings.SettingFile.GetSetting(GsSectionFunctions, GsSettingFunctionsSynchronize) = ModuleSettings.SettingFile.NoValue Then
            rdCheckSync.ToggleState = Enumerations.ToggleState.On
        Else
            rdCheckSync.ToggleState = Enumerations.ToggleState.Off
        End If

    End Sub

    Private Sub rdCheckSync_ToggleStateChanged(sender As System.Object, args As Telerik.WinControls.UI.StateChangedEventArgs) Handles rdCheckSync.ToggleStateChanged
        If rdCheckSync.ToggleState = Enumerations.ToggleState.On Then
            ModuleSettings.SettingFile.SaveSetting(GsSectionFunctions, GsSettingFunctionsSynchronize) = True
        Else
            ModuleSettings.SettingFile.SaveSetting(GsSectionFunctions, GsSettingFunctionsSynchronize) = False
        End If
    End Sub



End Class
