'===============================================================================
'Copyright Wigersma 2012
'All rights reserved.
'===============================================================================
Imports Telerik.WinControls.UI
Imports KAM.COMMUNICATOR.Infra
Imports System.Windows.Forms

Public Class usctrl_TranslationSelection

#Region "Constructor"
    Public Sub New()
        ' This call is required by the designer.
        InitializeComponent()
        ' Add any initialization after the InitializeComponent() call.
    End Sub
    ''' <summary>
    ''' Load the form with the data
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub usctrl_TranslationSelection_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        FillListCountry()
    End Sub
    ''' <summary>
    ''' Fill the list with flags and select the country with the saved settings
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub FillListCountry()
        Me.rdListTranslationSelection.Items.Clear()
        Dim countryFlagList(5) As RadListDataItem
        countryFlagList(0) = New RadListDataItem(My.Resources.COMMUNICATORMainResx.str_Deutsch)
        countryFlagList(0).Tag = "de-DE"
        countryFlagList(0).Image = My.Resources.countryFlags.Germany_Flag.ToBitmap
        countryFlagList(0).TextImageRelation = TextImageRelation.ImageBeforeText
        countryFlagList(1) = New RadListDataItem(My.Resources.COMMUNICATORMainResx.str_English)
        countryFlagList(1).Tag = "en-GB"
        countryFlagList(1).Image = My.Resources.countryFlags.United_Kingdom_flag.ToBitmap
        countryFlagList(1).TextImageRelation = TextImageRelation.ImageBeforeText
        countryFlagList(2) = New RadListDataItem(My.Resources.COMMUNICATORMainResx.str_Nederlands)
        countryFlagList(2).Tag = "nl-NL"
        countryFlagList(2).Image = My.Resources.countryFlags.Netherlands_Flag.ToBitmap
        countryFlagList(2).TextImageRelation = TextImageRelation.ImageBeforeText

        'MOD translate
        countryFlagList(3) = New RadListDataItem(My.Resources.COMMUNICATORMainResx.str_Russia)
        countryFlagList(3).Tag = "ru-RU"
        countryFlagList(3).Image = My.Resources.countryFlags.Russia_flag.ToBitmap
        countryFlagList(3).TextImageRelation = TextImageRelation.ImageBeforeText

        'MOD xxxt
        countryFlagList(4) = New RadListDataItem(My.Resources.COMMUNICATORMainResx.str_Italian)
        countryFlagList(4).Tag = "it-IT"
        countryFlagList(4).Image = My.Resources.countryFlags.Italy_Flag.ToBitmap
        countryFlagList(4).TextImageRelation = TextImageRelation.ImageBeforeText

        'MOD xxxt
        countryFlagList(5) = New RadListDataItem(My.Resources.COMMUNICATORMainResx.str_Polish)
        countryFlagList(5).Tag = "pl-PL"
        countryFlagList(5).Image = My.Resources.countryFlags.Poland_Flag.ToBitmap
        countryFlagList(5).TextImageRelation = TextImageRelation.ImageBeforeText
        Me.rdListTranslationSelection.Items.AddRange(countryFlagList)

        If ModuleCommunicatorSettings.SettingFile.GetSetting(GsSectionCulture, GsSettingCultureName) = ModuleCommunicatorSettings.SettingFile.NoValue Then
            Me.rdListTranslationSelection.Items(2).Selected = True
        Else
            For i As Integer = 0 To Me.rdListTranslationSelection.Items.Count
                If rdListTranslationSelection.Items(i).Tag = ModuleCommunicatorSettings.SettingFile.GetSetting(GsSectionCulture, GsSettingCultureName) Then
                    Me.rdListTranslationSelection.Items(i).Selected = True
                    Exit For
                End If
            Next i
        End If
    End Sub

#End Region
#Region "Destructor"
    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub
#End Region

#Region "Form handling"
    ''' <summary>
    ''' If the user selectes another item, save the data and display message to restart the program to apply settings
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub rdListTranslationSelection_SelectedIndexChanged(sender As System.Object, e As Telerik.WinControls.UI.Data.PositionChangedEventArgs) Handles rdListTranslationSelection.SelectedIndexChanged

        If rdListTranslationSelection.ActiveItem.Tag <> ModuleCommunicatorSettings.SettingFile.GetSetting(GsSectionCulture, GsSettingCultureName) Then
            If rdListTranslationSelection.SelectedIndex <> -1 Then
                Dim cultureName As String = rdListTranslationSelection.ActiveItem.Tag
                ModuleCommunicatorSettings.SettingFile.SaveSetting(GsSectionCulture, GsSettingCultureName) = cultureName

            End If
        End If
    End Sub
#End Region

End Class



