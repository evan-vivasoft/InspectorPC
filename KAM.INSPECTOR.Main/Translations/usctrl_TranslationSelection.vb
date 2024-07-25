Imports Telerik.WinControls.UI
Imports KAM.INSPECTOR.Main.My.Resources
Imports KAM.INSPECTOR.Infra
Imports KAM.INSPECTOR.info.modLicenseInfo
Imports System.Xml
Imports System.IO
Imports KAM.Infra

Public Class usctrl_TranslationSelection
#Region "Class members"
    Public Const GsSectionCulture As String = "Culture"
    Public Const GsSettingCultureName As String = "CultureName"
#End Region
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
        LoadTranslations()

        'MOD 18
        If ModuleSettings.SettingFile.GetSetting(GsSectionCulture, GsSettingCultureName) = ModuleSettings.SettingFile.NoValue Then
            Me.rdListTranslationSelection.Items(2).Selected = True
        Else
            For i As Integer = 0 To Me.rdListTranslationSelection.Items.Count

                If rdListTranslationSelection.Items(i).Tag = ModuleSettings.SettingFile.GetSetting(GsSectionCulture, GsSettingCultureName) Then
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
        'MOD 99
        If Directory.Exists(Path.Combine(Application.StartupPath, rdListTranslationSelection.ActiveItem.Tag)) Then
            If rdListTranslationSelection.ActiveItem.Tag <> ModuleSettings.SettingFile.GetSetting(GsSectionCulture, GsSettingCultureName) Then
                If rdListTranslationSelection.SelectedIndex <> -1 Then
                    Dim cultureName As String = rdListTranslationSelection.ActiveItem.Tag
                    ModuleSettings.SettingFile.SaveSetting(GsSectionCulture, GsSettingCultureName) = cultureName

                    Dim ok
                    ok = MsgBox(INSPECTORMainResx.str_Restart_application, MsgBoxStyle.YesNo, QlmProductName.ToString)
                    'Review. Should the application be restarted after changing settings? 
                    If ok = vbYes Then Application.Restart()
                End If
            End If

        Else
            MsgBox(INSPECTORMainResx.str_TranslationNotExists, MsgBoxStyle.Information, Application.ProductName)
        End If
    End Sub
#End Region

    ''' <summary>
    ''' MOD 99
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub LoadTranslations()

        'Set the default languages
        Me.rdListTranslationSelection.Items.Clear()

        Dim countryFlagList(2) As RadListDataItem
        countryFlagList(0) = New RadListDataItem(INSPECTORMainResx.str_Deutsch)
        countryFlagList(0).Tag = "de-DE"
        countryFlagList(0).Image = My.Resources.countryFlags.Germany_Flag.ToBitmap
        countryFlagList(0).TextImageRelation = TextImageRelation.ImageBeforeText
        countryFlagList(1) = New RadListDataItem(INSPECTORMainResx.str_English)
        countryFlagList(1).Tag = "en-GB"
        countryFlagList(1).Image = My.Resources.countryFlags.United_Kingdom_flag.ToBitmap
        countryFlagList(1).TextImageRelation = TextImageRelation.ImageBeforeText
        countryFlagList(2) = New RadListDataItem(INSPECTORMainResx.str_Nederlands)
        countryFlagList(2).Tag = "nl-NL"
        countryFlagList(2).Image = My.Resources.countryFlags.Netherlands_Flag.ToBitmap
        countryFlagList(2).TextImageRelation = TextImageRelation.ImageBeforeText

        Me.rdListTranslationSelection.Items.AddRange(countryFlagList)

        Dim translationFile As String

        'Check if file exists
        If DirExistsPC(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "ws-gas\CONNEXION V5.x\INSPECTORPC")) Then
            translationFile = (Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "WS-gas\CONNEXION V5.x\INSPECTORPC", "Languages.xml"))
        End If

        'Select languages from setting fils
        If CheckFileExistsPC(translationFile) Then
            Dim xmldoc As New XmlDataDocument()
            Dim xmlnode As XmlNodeList
            Dim i, j As Integer
            Dim iconFileName As String = ""
            Dim fs As New FileStream(translationFile, FileMode.Open, FileAccess.Read)
            xmldoc.Load(fs)
            xmlnode = xmldoc.GetElementsByTagName("Language")
            For i = 0 To xmlnode.Count - 1
                xmlnode(i).ChildNodes.Item(0).InnerText.Trim()

                Dim languageText As String = ""
                Dim languageImage As Image = Nothing
                Dim languageTranslate As String = ""
                Dim countryFlagList2 As RadListDataItem
                For j = 0 To 2


                    Select Case xmlnode(i).ChildNodes.Item(j).LocalName
                        Case "Language_id"
                            languageText = xmlnode(i).ChildNodes.Item(j).InnerText.Trim().ToString
                        Case "Country_Flag"
                            'To do check if flag exists
                            iconFileName = xmlnode(i).ChildNodes.Item(j).InnerText.Trim().ToString
                            iconFileName = Replace(iconFileName, "-", "_")
                            If My.Resources.countryFlags.ResourceManager.GetObject(iconFileName) IsNot Nothing Then
                                languageImage = My.Resources.countryFlags.ResourceManager.GetObject(iconFileName).ToBitmap
                            Else
                                languageImage = My.Resources.countryFlags.European_Union_Flag.ToBitmap()
                            End If
                        Case "Display_Text"
                            languageTranslate = xmlnode(i).ChildNodes.Item(j).InnerText.Trim().ToString
                    End Select

                Next
                countryFlagList2 = New RadListDataItem(languageTranslate)
                countryFlagList2.Tag = languageText
                countryFlagList2.Image = languageImage
                Me.rdListTranslationSelection.Items.Add(countryFlagList2)
            Next i

            ''MOD 92 translate
            'countryFlagList(3) = New RadListDataItem(INSPECTORMainResx.str_Russia)
            'countryFlagList(3).Tag = "ru-RU"
            'countryFlagList(3).Image = My.Resources.countryFlags.Russia_Flag.ToBitmap
            'countryFlagList(3).TextImageRelation = TextImageRelation.ImageBeforeText

            ''MOD 92
            'countryFlagList(4) = New RadListDataItem(INSPECTORMainResx.str_China)
            'countryFlagList(4).Tag = "zh-CN"
            'countryFlagList(4).Image = My.Resources.countryFlags.China_Flag.ToBitmap()
            'countryFlagList(4).TextImageRelation = TextImageRelation.ImageBeforeText

            ''MOD 98
            'countryFlagList(4) = New RadListDataItem(INSPECTORMainResx.str_Poland)
            'countryFlagList(4).Tag = "pl"
            'countryFlagList(4).Image = My.Resources.countryFlags.Poland_Flag.ToBitmap()
            'countryFlagList(4).TextImageRelation = TextImageRelation.ImageBeforeText

        End If
    End Sub

End Class



