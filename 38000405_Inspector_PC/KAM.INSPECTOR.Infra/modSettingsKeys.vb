Imports System.IO
Imports System.Reflection
Imports KAM.Infra
Public Module ModuleSettings
    'MOD 62
    Public ReadOnly Property SettingFilename As String
        Get
#If DEBUG Then
            Return (Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Config\INSPECTORSettings.xml"))
#End If
            If DirExistsPC(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "ws-gas\CONNEXION V5.x\INSPECTORPC")) Then
                Return (Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "ws-gas\CONNEXION V5.x\INSPECTORPC", "INSPECTORSettings.xml"))
            Else
                Return (Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "kamstrup\CONNEXION V5.x\INSPECTORPC", "INSPECTORSettings.xml"))
            End If
        End Get
    End Property

    'MOD 62
    Public ReadOnly Property ChartSettingsFile As String
        Get
#If DEBUG Then
            Return (Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Config\Chartsettings.xml"))
#End If
            If DirExistsPC(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "ws-gas\CONNEXION V5.x\INSPECTORPC")) Then
                Return (Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "WS-gas\CONNEXION V5.x\INSPECTORPC", "ChartSettings.xml"))
            Else
                Return (Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "kamstrup\CONNEXION V5.x\INSPECTORPC", "ChartSettings.xml"))
            End If
        End Get
    End Property

    'MOD 84
    Public ReadOnly Property SettingsFolder As String
        Get
#If DEBUG Then
            Return (Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)))
#End If
            If DirExistsPC(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "ws-gas\CONNEXION V5.x\INSPECTORPC")) Then
                Return (Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "WS-gas\CONNEXION V5.x\INSPECTORPC"))
            Else
                Return (Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "kamstrup\CONNEXION V5.x\INSPECTORPC"))
            End If
        End Get
    End Property

    Public SettingFile As New KAM.INSPECTOR.Infra.clsSettings(SettingFilename)




End Module
Public Module modSettingsKeys
#Region "Localization"
    ''' <summary>
    ''' Localization section
    ''' </summary>
    ''' <remarks></remarks>
    Public Const GsSectionLocalization As String = "Localization"                       'Setting to configuration file 
    ''' <summary>
    ''' Telerik Theme name
    ''' </summary>
    ''' <remarks></remarks>
    Public Const GsSettingLocalizationThemeName As String = "ThemeName"                 'Setting to configuration file 
#End Region
#Region "Options/ inspector functions"
    ''' <summary>
    ''' Items for the main form of inspector
    ''' </summary>
    ''' <remarks></remarks>
    Public Const GsSectionFunctions As String = "Functions"                             'Setting to configuration file 
    ''' <summary>
    ''' Item for displaying Synchronization option
    ''' </summary>
    ''' <remarks></remarks>
    Public Const GsSettingFunctionsSynchronize As String = "DocumentSynchronize"        'Setting to configuration file 
#End Region
#Region "Chart Settings"
    ''' <summary>
    ''' Item for chart
    ''' </summary>
    ''' <remarks></remarks>
    Public Const GsSectionChart As String = "Chart"
    ''' <summary>
    ''' Item for background color of chart
    ''' </summary>
    ''' <remarks></remarks>
    Public Const GsSettingChartBackColor As String = "BackColor"
    Public Const GsSettingChartAxisXMinimum As String = "AxisXMinimum"                  'Setting to configuration file 
    Public Const GsSettingChartAxisXMaximum As String = "AxisXMaximum"                  'Setting to configuration file 
    Public Const GsSettingChartSeriesLineSize As String = "SeriesLineSize"              'Setting to configuration file 


#End Region
#Region "Size Settings"
    ''' <summary>
    ''' Item for chart
    ''' </summary>
    ''' <remarks></remarks>
    Public Const GsSectionText As String = "Text"
    ''' <summary>
    ''' Item for background color of chart
    ''' </summary>
    ''' <remarks></remarks>
    Public Const GsSettingTextSize As String = "Size"
#End Region
#Region "PLEXOR"
    Public Const GsSectionPlexor As String = "PLEXOR"
    Public Const GsSettingPLexorSelected As String = "BluetoothDestinationAddress"
    Public Const GsSettingPlexorBTDongleAddress As String = "BluetoothDongleAddress"
    Public Const GsSettingPlexorBTDongleApi As String = "BluetoothApi"
    'MOD 78
    Public Const GsSettingPlexorUnpairBeforeInspection As String = "UnpairBeforeInspection"
#End Region
    'MOD 58
#Region "UNITS"
    Public Const GsSectionUnits As String = "UNITS"
    Public Const GsSettingUnitLowPressure As String = "UnitLowPressure"
    Public Const GsSettingUnitHighPressure As String = "UnitHighPressure"
    Public Const GsSettingFactorLowHighPressure As String = "FactorLowHighPressure"
    Public Const GsSettingFactorMeasuredChangeRateToMbarMin As String = "FactorMeasuredChangeRateToMbarMin"
    Public Const GsSettingFactorMbarMinToUnitChangeRate As String = "FactorMbarMinToUnitChangeRate"
    Public Const GsSettingUnitChangeRate As String = "UnitChangeRate"
    Public Const GsSettingUnitQvsLeakage As String = "UnitQVSLeakage"
    Public Const GsSettingFactorQvs As String = "FactorQVS"
#End Region
#Region "GUI"
    Public Const GsSectionGUI As String = "GUI"
    Public Const GsSettingGUIShowBtnRestartLeakage As String = "ShowBtnRestartLeakage"
    'MOD 56
    Public Const GsSettingGUIShowDropDownSelectInspection As String = "ShowDropDownSelectInspection"
#End Region
End Module


Public Module ModPlexorUnits
    Public ReadOnly UnitLowPressure = ModuleSettings.SettingFile.GetSetting(GsSectionUnits, GsSettingUnitLowPressure)
    Public ReadOnly UnitHighPressure = ModuleSettings.SettingFile.GetSetting(GsSectionUnits, GsSettingUnitHighPressure)
    Public ReadOnly FactorLowHighPressure = ModuleSettings.SettingFile.GetSetting(GsSectionUnits, GsSettingFactorLowHighPressure)
    Public ReadOnly FactorMeasuredChangeRateToMbarMin = ModuleSettings.SettingFile.GetSetting(GsSectionUnits, GsSettingFactorMeasuredChangeRateToMbarMin)
    Public ReadOnly FactorMbarMinToUnitChangeRate = ModuleSettings.SettingFile.GetSetting(GsSectionUnits, GsSettingFactorMbarMinToUnitChangeRate)
    Public ReadOnly UnitChangeRate = ModuleSettings.SettingFile.GetSetting(GsSectionUnits, GsSettingUnitChangeRate)
    Public ReadOnly UnitQvsLeakage = ModuleSettings.SettingFile.GetSetting(GsSectionUnits, GsSettingUnitQvsLeakage)

    Public ReadOnly FactorQVS = ModuleSettings.SettingFile.GetSetting(GsSectionUnits, GsSettingFactorQvs)

End Module
