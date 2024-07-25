Imports System.IO
Imports KAM.INSPECTOR.Infra
Imports KAM.COMMUNICATOR.Infra
Imports KAM.Infra
'MOD 34
''' <summary>
''' Module for handling PLEXOR units
''' </summary>
''' <remarks></remarks>
Public Module ModPLEXORUnit
    Public SettingFilePlexorUnits As New KAM.INSPECTOR.Infra.clsSettings(SettingFilename)

    ''' <summary>
    ''' Return the units from file PLEXORUnits.xml. File is in the Plexor data directory
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property SettingFilename As String
        Get
            If DirExistsPC(Path.Combine(modCommunicationPaths.PlexorDataDirPC)) Then
                Return (Path.Combine(modCommunicationPaths.PlexorDataDirPC, "PLEXORUnits.xml")) '"C:\Kamstrup\CONNEXION V5.x\Data\PLEXOR\PlexorUnits.xml")
            Else
                Return ""
            End If
        End Get
    End Property

    ''' <summary>
    ''' Read the PLEXOR units form file PLEXORUnits.xml
    ''' If no file is detected or value is not present then the default value is applied
    ''' </summary>
    ''' <returns>a list with the used PLEXOR units</returns>
    ''' <remarks></remarks>
    Public ReadOnly Property GetPlexorUnits As List(Of String)
        Get
            Dim lsUnitList As New List(Of String)
            Dim lsReadValue As String

            lsReadValue = SettingFilePlexorUnits.GetSetting(GsSectionUnits, GsSettingUnitLowPressure)
            If lsReadValue.ToUpper = "<No Value>".ToUpper Or lsReadValue = "" Then
                lsReadValue = "mbar"
            End If
            lsUnitList.Add(lsReadValue)

            lsReadValue = SettingFilePlexorUnits.GetSetting(GsSectionUnits, GsSettingUnitHighPressure)
            If lsReadValue.ToUpper = "<No Value>".ToUpper Or lsReadValue = "" Then
                lsReadValue = "bar"
            End If
            lsUnitList.Add(lsReadValue)

            lsReadValue = SettingFilePlexorUnits.GetSetting(GsSectionUnits, GsSettingUnitChangeRate)
            If lsReadValue.ToUpper = "<No Value>".ToUpper Or lsReadValue = "" Then
                lsReadValue = "mbar/min"
            End If
            lsUnitList.Add(lsReadValue)

            lsReadValue = SettingFilePlexorUnits.GetSetting(GsSectionUnits, GsSettingUnitQvsLeakage)
            If lsReadValue.ToUpper = "<No Value>".ToUpper Or lsReadValue = "" Then
                lsReadValue = "dm3/h"
            End If
            lsUnitList.Add(lsReadValue)

            Return lsUnitList

        End Get
    End Property

    ''' <summary>
    ''' 'Saving the unit settings to InspectorSettings.xml
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub SaveSettingsToInspector()
        'MOD 36
        Dim lsReadValue As String

        'MOD 39
        lsReadValue = SettingFilePlexorUnits.GetSetting(GsSectionUnits, GsSettingUnitLowPressure)
        If lsReadValue.ToUpper = "<No Value>".ToUpper Or lsReadValue = "" Then
            lsReadValue = "mbar"
        End If
        ModuleSettings.SettingFile.SaveSetting(GsSectionUnits, GsSettingUnitLowPressure) = lsReadValue


        lsReadValue = SettingFilePlexorUnits.GetSetting(GsSectionUnits, GsSettingUnitHighPressure)
        If lsReadValue.ToUpper = "<No Value>".ToUpper Or lsReadValue = "" Then
            lsReadValue = "bar"
        End If
        ModuleSettings.SettingFile.SaveSetting(GsSectionUnits, GsSettingUnitHighPressure) = lsReadValue

        lsReadValue = SettingFilePlexorUnits.GetSetting(GsSectionUnits, GsSettingFactorLowHighPressure)
        If lsReadValue.ToUpper = "<No Value>".ToUpper Or lsReadValue = "" Then
            lsReadValue = "0.001"
        End If
        ModuleSettings.SettingFile.SaveSetting(GsSectionUnits, GsSettingFactorLowHighPressure) = lsReadValue

        lsReadValue = SettingFilePlexorUnits.GetSetting(GsSectionUnits, GsSettingFactorMeasuredChangeRateToMbarMin)
        If lsReadValue.ToUpper = "<No Value>".ToUpper Or lsReadValue = "" Then
            lsReadValue = "1"
        End If
        ModuleSettings.SettingFile.SaveSetting(GsSectionUnits, GsSettingFactorMeasuredChangeRateToMbarMin) = lsReadValue

        lsReadValue = SettingFilePlexorUnits.GetSetting(GsSectionUnits, GsSettingFactorMbarMinToUnitChangeRate)
        If lsReadValue.ToUpper = "<No Value>".ToUpper Or lsReadValue = "" Then
            lsReadValue = "1"
        End If
        ModuleSettings.SettingFile.SaveSetting(GsSectionUnits, GsSettingFactorMbarMinToUnitChangeRate) = lsReadValue

        lsReadValue = SettingFilePlexorUnits.GetSetting(GsSectionUnits, GsSettingUnitChangeRate)
        If lsReadValue.ToUpper = "<No Value>".ToUpper Or lsReadValue = "" Then
            lsReadValue = "mbar/min"
        End If
        ModuleSettings.SettingFile.SaveSetting(GsSectionUnits, GsSettingUnitChangeRate) = lsReadValue

        lsReadValue = SettingFilePlexorUnits.GetSetting(GsSectionUnits, GsSettingUnitQvsLeakage)
        If lsReadValue.ToUpper = "<No Value>".ToUpper Or lsReadValue = "" Then
            lsReadValue = "dm3/h"
        End If
        ModuleSettings.SettingFile.SaveSetting(GsSectionUnits, GsSettingUnitQvsLeakage) = lsReadValue

        lsReadValue = SettingFilePlexorUnits.GetSetting(GsSectionUnits, GsSettingFactorQvs)
        If lsReadValue.ToUpper = "<No Value>".ToUpper Or lsReadValue = "" Then
            lsReadValue = "1"
        End If
        ModuleSettings.SettingFile.SaveSetting(GsSectionUnits, GsSettingFactorQvs) = lsReadValue

    End Sub

End Module
