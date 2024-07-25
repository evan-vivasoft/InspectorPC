'===============================================================================
'Copyright Wigersma 2013
'All rights reserved.
'===============================================================================

Imports KAM.COMMUNICATOR.Synchronize.Bussiness
Imports KAM.COMMUNICATOR.Infra
Imports Telerik.WinControls
Imports Inspector.Model.InspectionProcedure
Public Class usctrl_SyncOptions

#Region "Class members"

#End Region

#Region "Constructor"


    Public Sub New()


        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        Dim iniSetting As String

        'For PRS
        rdDropDownPRSFormat.Items.Add(clsDbGeneral.enumFileFormat.FormatMsAccess.ToString)
        rdDropDownPRSFormat.Items.Add(clsDbGeneral.enumFileFormat.FormatXml.ToString)
        iniSetting = ModuleCommunicatorSettings.SettingFile.GetSetting(GsSectionPrs, GsSettingPrsDpcfileFormat)
        rdDropDownPRSFormat.Text = iniSetting

        If ModuleCommunicatorSettings.SettingFile.GetSetting(GsSectionPrs, GsSettingPrsDeletePrsFromAccess) = True Or ModuleCommunicatorSettings.SettingFile.GetSetting(GsSectionPrs, GsSettingPrsDeletePrsFromAccess) = ModuleCommunicatorSettings.SettingFile.NoValue Then
            rdCheckPrsMsAccessClearPrs.ToggleState = Enumerations.ToggleState.On
        Else
            rdCheckPrsMsAccessClearPrs.ToggleState = Enumerations.ToggleState.Off
        End If

        'MOD 28
        If ModuleCommunicatorSettings.SettingFile.GetSetting(GsSectionPrs, GsSettingPrsDeletePrsOnStatusInspector) = Int(InspectionStatus.Unset) & ";" & Int(InspectionStatus.NoInspection) Then 'MOD XXX Or ModuleCommunicatorSettings.SettingFile.GetSetting(GsSectionPrs, GsSettingPrsDeletePrsOnStatusInspector) = ModuleCommunicatorSettings.SettingFile.NoValue Then
            'Status 1
            rdRadioPRSXMLPRSInspector1.ToggleState = Enumerations.ToggleState.On
        ElseIf ModuleCommunicatorSettings.SettingFile.GetSetting(GsSectionPrs, GsSettingPrsDeletePrsOnStatusInspector) = Int(InspectionStatus.Unset) & ";" & Int(InspectionStatus.NoInspection) & ";" & Int(InspectionStatus.StartNotCompleted) Then
            'Status 1;2
            rdRadioPRSXMLPRSInspector2.ToggleState = Enumerations.ToggleState.On
        Else
            'MOD 37
            rdRadioPRSXMLPRSInspector3.ToggleState = Enumerations.ToggleState.On
        End If

        If ModuleCommunicatorSettings.SettingFile.GetSetting(GsSectionPrs, GsSettingPrsFilterOption) = "PDASNR" Then
            rdCheckPrsMsAccessUsePRSSelectionPDASNR.ToggleState = Enumerations.ToggleState.On
        Else
            rdCheckPrsMsAccessUsePRSSelectionPDASNR.ToggleState = Enumerations.ToggleState.Off
        End If



        'For results
        rdDropResultFormat.Items.Add(clsDbGeneral.enumFileFormat.FormatMsAccess.ToString)
        rdDropResultFormat.Items.Add(clsDbGeneral.enumFileFormat.FormatXml.ToString)
        iniSetting = ModuleCommunicatorSettings.SettingFile.GetSetting(GsSectionResult, GsSettingResultDpcfileFormat)
        rdDropResultFormat.Text = iniSetting

        If ModuleCommunicatorSettings.SettingFile.GetSetting(GsSectionResult, GsSettingResultCreateFile) = True Or ModuleCommunicatorSettings.SettingFile.GetSetting(GsSectionResult, GsSettingResultCreateFile) = ModuleCommunicatorSettings.SettingFile.NoValue Then
            rdCheckResultXMLCreateResults.ToggleState = Enumerations.ToggleState.On
        Else
            rdCheckResultXMLCreateResults.ToggleState = Enumerations.ToggleState.Off
        End If

        setPrsFormatSettings()
        setResultFormatSettings()

        'MOD 18
        rdTextAltPrsFileName.Text = ModuleCommunicatorSettings.SettingFile.GetSetting(KAM.COMMUNICATOR.Infra.GsSectionPrs, KAM.COMMUNICATOR.Infra.GsSettingPrsAlternativeFileName)

        'MOD 28 'add handler of toggle button
        AddHandler rdRadioPRSXMLPRSInspector1.ToggleStateChanged, AddressOf rdCheckPRSNonInspection_ToggleStateChanged
        AddHandler rdRadioPRSXMLPRSInspector2.ToggleStateChanged, AddressOf rdCheckPRSNonInspection_ToggleStateChanged
    End Sub



#End Region
#Region "Form handling"
    ''' <summary>
    ''' Handling of combobox change
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub rdDropDownPRSFormat_SelectedIndexChanged(sender As System.Object, e As Telerik.WinControls.UI.Data.PositionChangedEventArgs) Handles rdDropDownPRSFormat.SelectedIndexChanged
        ModuleCommunicatorSettings.SettingFile.SaveSetting(GsSectionPrs, GsSettingPrsDpcfileFormat) = rdDropDownPRSFormat.Text.ToString
        setPrsFormatSettings()
    End Sub

    Private Sub setPrsFormatSettings()
        If rdDropDownPRSFormat.Text.ToString = clsDbGeneral.enumFileFormat.FormatMsAccess.ToString Then
            rdCheckPrsMsAccessClearPrs.Enabled = True
            rdCheckPrsMsAccessUsePRSSelectionPDASNR.Enabled = True
            'MOD 37 / MOD 38
            rdGroupXMLPrsStatus.Enabled = True
        Else
            rdCheckPrsMsAccessClearPrs.Enabled = False
            rdCheckPrsMsAccessUsePRSSelectionPDASNR.Enabled = False
            rdGroupXMLPrsStatus.Enabled = True
        End If
    End Sub

    ''' <summary>
    ''' Handling of combobox change
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub rdDropResultFormat_SelectedIndexChanged(sender As System.Object, e As Telerik.WinControls.UI.Data.PositionChangedEventArgs) Handles rdDropResultFormat.SelectedIndexChanged
        ModuleCommunicatorSettings.SettingFile.SaveSetting(GsSectionResult, GsSettingResultDpcfileFormat) = rdDropResultFormat.Text.ToString
        setResultFormatSettings()
    End Sub

    Private Sub setResultFormatSettings()
        If rdDropResultFormat.Text.ToString = clsDbGeneral.enumFileFormat.FormatMsAccess.ToString Then
            'MOD 38 rdCheckResultXMLCreateResults.Enabled = False
            rdCheckResultXmlAppendResultsCompletedInspection.Enabled = False
        Else
            'MOD 38 rdCheckResultXMLCreateResults.Enabled = True
            rdCheckResultXmlAppendResultsCompletedInspection.Enabled = True
        End If
    End Sub
#End Region

    Private Sub rdCheckClearPrs_ToggleStateChanged(sender As System.Object, args As Telerik.WinControls.UI.StateChangedEventArgs) Handles rdCheckPrsMsAccessClearPrs.ToggleStateChanged
        If rdCheckPrsMsAccessClearPrs.ToggleState = Enumerations.ToggleState.On Then
            ModuleCommunicatorSettings.SettingFile.SaveSetting(GsSectionPrs, GsSettingPrsDeletePrsFromAccess) = True
        Else
            ModuleCommunicatorSettings.SettingFile.SaveSetting(GsSectionPrs, GsSettingPrsDeletePrsFromAccess) = False
        End If
    End Sub



    Private Sub rdCheckPRSNonInspection_ToggleStateChanged(sender As System.Object, args As Telerik.WinControls.UI.StateChangedEventArgs)
        'Mod 28
        'Select on which state the PRS information is left on INSPECTOR
        '!Always add the state Unset

        '"Unset" and "inspection not started"
        If rdRadioPRSXMLPRSInspector1.ToggleState = Enumerations.ToggleState.On Then ModuleCommunicatorSettings.SettingFile.SaveSetting(GsSectionPrs, GsSettingPrsDeletePrsOnStatusInspector) = Int(InspectionStatus.Unset) & ";" & Int(InspectionStatus.NoInspection)
        '"Unset" and "Inspection not started" and "inspection not completed"
        If rdRadioPRSXMLPRSInspector2.ToggleState = Enumerations.ToggleState.On Then ModuleCommunicatorSettings.SettingFile.SaveSetting(GsSectionPrs, GsSettingPrsDeletePrsOnStatusInspector) = Int(InspectionStatus.Unset) & ";" & Int(InspectionStatus.NoInspection) & ";" & Int(InspectionStatus.StartNotCompleted)
        'MOD 37 / MOD 38
        If rdRadioPRSXMLPRSInspector3.ToggleState = Enumerations.ToggleState.On Then ModuleCommunicatorSettings.SettingFile.SaveSetting(GsSectionPrs, GsSettingPrsDeletePrsOnStatusInspector) = "-1"
    End Sub

    Private Sub rdCheckUsePRSSelectionPDASNR_ToggleStateChanged(sender As System.Object, args As Telerik.WinControls.UI.StateChangedEventArgs) Handles rdCheckPrsMsAccessUsePRSSelectionPDASNR.ToggleStateChanged
        If rdCheckPrsMsAccessUsePRSSelectionPDASNR.ToggleState = Enumerations.ToggleState.On Then
            ModuleCommunicatorSettings.SettingFile.SaveSetting(GsSectionPrs, GsSettingPrsFilterOption) = "PDASNR"
        Else
            ModuleCommunicatorSettings.SettingFile.SaveSetting(GsSectionPrs, GsSettingPrsFilterOption) = ""
        End If
    End Sub

    Private Sub rdCheckCreateResults_ToggleStateChanged(sender As System.Object, args As Telerik.WinControls.UI.StateChangedEventArgs) Handles rdCheckResultXMLCreateResults.ToggleStateChanged
        If rdCheckResultXMLCreateResults.ToggleState = Enumerations.ToggleState.On Then
            ModuleCommunicatorSettings.SettingFile.SaveSetting(GsSectionResult, GsSettingResultCreateFile) = True
        Else
            ModuleCommunicatorSettings.SettingFile.SaveSetting(GsSectionResult, GsSettingResultCreateFile) = False
        End If
    End Sub

    Private Function rdCheckPRSNonInspection() As Object
        Throw New NotImplementedException
    End Function
    'MOD 18
    Private Sub rdTextAltPrsFileName_TextChanged(sender As System.Object, e As System.EventArgs) Handles rdTextAltPrsFileName.TextChanged
        ModuleCommunicatorSettings.SettingFile.SaveSetting(KAM.COMMUNICATOR.Infra.GsSectionPrs, KAM.COMMUNICATOR.Infra.GsSettingPrsAlternativeFileName) = rdTextAltPrsFileName.Text
    End Sub







End Class

