Imports System.IO

Public Module ModuleCommunicatorSettings
    'MOD 10 Public SettingFile As New KAM.INSPECTOR.Infra.clsSettings(Path.Combine(My.Application.Info.DirectoryPath, "Config", "COMMUNICATORsettings.xml"))
    Public ReadOnly SettingFilename As String = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "WS-gas\CONNEXION V5.x\COMMUNICATOR", "COMMUNICATORsettings.xml")
    Public SettingFile As New KAM.INSPECTOR.Infra.clsSettings(SettingFilename)
End Module

Public Module modSettingsKeys
#Region "Localization"
    ''' <summary>
    ''' Localization section
    ''' </summary>
    ''' <remarks></remarks>
    Public Const GsSectionLocalization As String = "Localization"                                      'Setting to configuration file 
    ''' <summary>
    ''' Telerik Theme name
    ''' </summary>
    ''' <remarks></remarks>
    Public Const GsSettingLocalizationThemeName As String = "ThemeName"                                 'Setting to configuration file 
#End Region
#Region "Culture"
    Public Const GsSectionCulture As String = "Culture"
    Public Const GsSettingCultureName As String = "CultureName"
#End Region
#Region "Application"
    ''' <summary>
    ''' PRS file section
    ''' </summary>
    ''' <remarks></remarks>
    Public Const GsSectionApplication As String = "APPLICATION"                                             'Setting to configuration file 
    ''' <summary>
    ''' Settings Format of Input file
    ''' </summary>
    ''' <remarks></remarks>
    Public Const GsSettingApplicatioPathSyncData As String = "PathSyncData"                                 'Setting to configuration file
#End Region
#Region "PRS file"
    ''' <summary>
    ''' PRS file section
    ''' </summary>
    ''' <remarks></remarks>
    Public Const GsSectionPrs As String = "PRS file"                                                        'Setting to configuration file 
    ''' <summary>
    ''' Settings Format of Input file
    ''' </summary>
    ''' <remarks></remarks>
    Public Const GsSettingPrsDpcfileFormat As String = "Format PC"                                          'Setting to configuration file 
    ''' <summary>
    ''' Setting filter options of PRS file
    ''' </summary>
    ''' <remarks></remarks>
    Public Const GsSettingPrsFilterOption As String = "FilterOptionfromMsAccess"                            'Setting to configuration file 
    ''' <summary>
    ''' Do not remove the PRS on Inspector in case if no inspection is performed
    ''' </summary>
    ''' <remarks></remarks>
    Public Const GsSettingPrsDeletePrsOnStatusInspector As String = "DeletePRSOnStatusINSPECTOR"            'Setting to configuration file 
    ''' <summary>
    ''' Delete all data from prs msAccess database after synchronisation
    ''' </summary>
    ''' <remarks></remarks>
    Public Const GsSettingPrsDeletePrsFromAccess As String = "DeletePRSfromMsAccess"                        'Setting to configuration file 
    ''' <summary>
    ''' Alternative file name of the PRS.xml file; Is empty default name is used
    ''' </summary>
    ''' <remarks></remarks>
    Public Const GsSettingPrsAlternativeFileName As String = "AlternativeFileName"                          'Setting to configuration file 
#End Region
#Region "Result file"
    ''' <summary>
    ''' PRS file section
    ''' </summary>
    ''' <remarks></remarks>
    Public Const GsSectionResult As String = "Result file"                                                  'Setting to configuration file 
    ''' <summary>
    ''' Create a results file during synchronisation
    ''' </summary>
    ''' <remarks></remarks>
    Public Const GsSettingResultCreateFile As String = "CreateFile"                                         'Setting to configuration file 
    ''' <summary>
    ''' The file format of the results file on de PC
    ''' </summary>
    ''' <remarks></remarks>
    Public Const GsSettingResultDpcfileFormat As String = "Format PC"                                       'Setting to configuration file 
#End Region


End Module

