'===============================================================================
'Copyright Wigersma 2013
'All rights reserved.
'===============================================================================

Imports System.Threading
Imports System.Globalization
Imports Telerik.WinControls.UI.Localization
Imports System.IO
Imports KAM.Infra
Imports KAM.COMMUNICATOR.Infra
Imports System.Windows.Forms

Public Class SplashScreen
#Region "Class members"
    Private ucAbout As usctrl_About
#End Region
#Region "Constructor"

    ''' <summary>
    ''' Initialize form
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()

        'Set the time to display the splash screen
        My.Application.MinimumSplashScreenDisplayTime = 4000

        '' '' This call is required by the designer.
        InitializeComponent()

        'check if the files exists
        If Not CheckFileExistsPC(SettingFilename) Then
            MsgBox(My.Resources.COMMUNICATORMainResx.str_FileNotExists1 & " '" & SettingFilename & "' " & My.Resources.COMMUNICATORMainResx.str_fileNotExists2, MsgBoxStyle.Critical, Application.ProductName)
            End
        End If
        'Load the theme
        Dim setThemes As New usctrl_GeneralUI
        setThemes.SetThemeName()
        setThemes = Nothing
        'Load the language
        Dim cultureName As String = ModuleCommunicatorSettings.SettingFile.GetSetting(GsSectionCulture, GsSettingCultureName)
        If cultureName = ModuleCommunicatorSettings.SettingFile.NoValue Then
            cultureName = "en-GB"
            ModuleCommunicatorSettings.SettingFile.SaveSetting(GsSectionCulture, GsSettingCultureName) = cultureName
        End If
        Dim cultureInfo As CultureInfo = New CultureInfo(cultureName)
        'MOD 24 Thread.CurrentThread.CurrentCulture = cultureInfo
        Thread.CurrentThread.CurrentUICulture = cultureInfo

        ''Apply on telerik grid
        RadGridLocalizationProvider.CurrentProvider = New MyCultureRadGridLocalizationProvider

        'Loading the control about
        ucAbout = New usctrl_About
        ucAbout.Dock = DockStyle.Fill

        Me.Controls.Add(ucAbout)

    End Sub

#End Region
#Region "File exists check"


    ''' <summary>
    ''' Checking if settings file, XML and XSD's are present
    ''' Returns false if a file is not found
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function CheckFilesExists() As Boolean
        Dim xsdDirPC = Path.GetDirectoryName((My.Application.Info.DirectoryPath))
        xsdDirPC = Path.Combine(xsdDirPC, "XSD")
        'check if the configuration file is present
        Dim fileName As String = ""

        fileName = Path.Combine(Application.StartupPath, "Config", "COMMUNICATORSettings.xml")
        If Not CheckFileExistsPC(fileName) Then GoTo NoFileFound

        fileName = Path.Combine(xsdDirPC, "InspectionProcedure.xsd")
        If Not CheckFileExistsPC(fileName) Then GoTo NoFileFound

        fileName = Path.Combine(xsdDirPC, "InspectionResultsData.xsd")
        If Not CheckFileExistsPC(fileName) Then GoTo NoFileFound

        fileName = Path.Combine(xsdDirPC, "InspectionStatus.xsd")
        If Not CheckFileExistsPC(fileName) Then GoTo NoFileFound

        fileName = Path.Combine(xsdDirPC, "PLEXOR.xsd")
        If Not CheckFileExistsPC(fileName) Then GoTo NoFileFound
        Return True

NoFileFound:
        MsgBox(My.Resources.COMMUNICATORMainResx.str_FileNotExists1 & " '" & fileName & "' " & My.Resources.COMMUNICATORMainResx.str_fileNotExists2, MsgBoxStyle.Critical, Application.ProductName)
        Return False

    End Function
#End Region

#Region "Telerik Grid translations"
    ''' <summary>
    ''' Class for apply culture on telerik grid
    ''' </summary>
    ''' <remarks></remarks>
    Public Class MyCultureRadGridLocalizationProvider
        Inherits RadGridLocalizationProvider
        Public Overrides Function GetLocalizedString(ByVal id As String) As string
            Select Case id
                Case RadGridStringId.FilterOperatorBetween
                    Return My.Resources.TelerikGridResx.str_FilterOperatorBetween '"between"
                Case RadGridStringId.FilterOperatorContains
                    Return My.Resources.TelerikGridResx.str_FilterOperatorContains '"Contains"
                Case RadGridStringId.FilterOperatorDoesNotContain
                    Return My.Resources.TelerikGridResx.str_FilterOperatorDoesNotContain '"Does not contain"
                Case RadGridStringId.FilterOperatorEndsWith
                    Return My.Resources.TelerikGridResx.str_FilterOperatorEndsWith '"Ends with"
                Case RadGridStringId.FilterOperatorEqualTo
                    Return My.Resources.TelerikGridResx.str_FilterOperatorEqualTo '"Equal to"
                Case RadGridStringId.FilterOperatorGreaterThan
                    Return My.Resources.TelerikGridResx.str_FilterOperatorGreaterThan '"Greater Than"
                Case RadGridStringId.FilterOperatorGreaterThanOrEqualTo
                    Return My.Resources.TelerikGridResx.str_FilterOperatorGreatorEqual '"Greater than or Equals to"
                Case RadGridStringId.FilterOperatorIsEmpty
                    Return My.Resources.TelerikGridResx.str_FilterOperatorIsEmpty '"Is empty"
                Case RadGridStringId.FilterOperatorIsNull
                    Return My.Resources.TelerikGridResx.str_FilterOperatorIsNull '"Is null"
                Case RadGridStringId.FilterOperatorLessThan
                    Return My.Resources.TelerikGridResx.str_FilterOperatorLessThan '"Less than"
                Case RadGridStringId.FilterOperatorLessThanOrEqualTo
                    Return My.Resources.TelerikGridResx.str_FilterOperatorLessThanOrEqualTo '"Less than or equal to"
                Case RadGridStringId.FilterOperatorNoFilter
                    Return My.Resources.TelerikGridResx.str_FilterOperatorNoFilter '"No Filter"
                Case RadGridStringId.FilterOperatorNotBetween
                    Return My.Resources.TelerikGridResx.str_FilterOperatorNotBetween '"Not between"
                Case RadGridStringId.FilterOperatorNotEqualTo
                    Return My.Resources.TelerikGridResx.str_FilterOperatorNotEqualTo '"Not equal to"
                Case RadGridStringId.FilterOperatorNotIsEmpty
                    Return My.Resources.TelerikGridResx.str_FilterOperatorNotIsEmpty '"Is not empyt"
                Case RadGridStringId.FilterOperatorNotIsNull
                    Return My.Resources.TelerikGridResx.str_FilterOperatorNotIsNull '"Is not Null"
                Case RadGridStringId.FilterOperatorStartsWith
                    Return My.Resources.TelerikGridResx.str_FilterOperatorStartsWith '"Starts with"
                Case RadGridStringId.FilterOperatorIsLike
                    Return My.Resources.TelerikGridResx.str_FilterOperatorIsLike '"Is Like"
                Case RadGridStringId.FilterOperatorNotIsLike
                    Return My.Resources.TelerikGridResx.str_FilterOperatorNotIsLike '"Is not like"
                Case RadGridStringId.FilterOperatorIsContainedIn
                    Return My.Resources.TelerikGridResx.str_FilterOperatorIsContainedIn '"Is contained in"
                Case RadGridStringId.FilterOperatorNotIsContainedIn
                    Return My.Resources.TelerikGridResx.str_FilterOperatorNotIsContainedIn '"Is not contained in"
                Case RadGridStringId.FilterOperatorCustom
                    Return My.Resources.TelerikGridResx.str_FilterOperatorCustom '"Custom operator"
                Case RadGridStringId.FilterFunctionBetween
                    Return My.Resources.TelerikGridResx.str_FilterFunctionBetween '"Between"
                Case RadGridStringId.FilterFunctionContains
                    Return My.Resources.TelerikGridResx.str_FilterFunctionContains '"Contains"
                Case RadGridStringId.FilterFunctionDoesNotContain
                    Return My.Resources.TelerikGridResx.str_FilterFunctionDoesNotContain '"Does not contains"
                Case RadGridStringId.FilterFunctionEndsWith
                    Return My.Resources.TelerikGridResx.str_FilterFunctionEndsWith '"Ends with"
                Case RadGridStringId.FilterFunctionEqualTo
                    Return My.Resources.TelerikGridResx.str_FilterFunctionEqualTo '"Equal to"
                Case RadGridStringId.FilterFunctionGreaterThan
                    Return My.Resources.TelerikGridResx.str_FilterFunctionGreaterThan '"Greater than"
                Case RadGridStringId.FilterFunctionGreaterThanOrEqualTo
                    Return My.Resources.TelerikGridResx.str_FilterFunctionGreatorEqual '"Greater than or equal to"
                Case RadGridStringId.FilterFunctionIsEmpty
                    Return My.Resources.TelerikGridResx.str_FilterFunctionIsEmpty '"Is empty"
                Case RadGridStringId.FilterFunctionIsNull
                    Return My.Resources.TelerikGridResx.str_FilterFunctionIsNull '"" '"Is null"
                Case RadGridStringId.FilterFunctionLessThan
                    Return My.Resources.TelerikGridResx.str_FilterFunctionLessThan '"Less than"
                Case RadGridStringId.FilterFunctionLessThanOrEqualTo
                    Return My.Resources.TelerikGridResx.str_FilterFunctionLessThanOrEqualTo '"Less than or equal to"
                Case RadGridStringId.FilterFunctionNoFilter
                    Return My.Resources.TelerikGridResx.str_FilterFunctionNoFilter '"No filter"
                Case RadGridStringId.FilterFunctionNotBetween
                    Return My.Resources.TelerikGridResx.str_FilterFunctionNotBetween '"Not between"
                Case RadGridStringId.FilterFunctionNotEqualTo
                    Return My.Resources.TelerikGridResx.str_FilterFunctionNotEqualTo '"" '"Not equal to"
                Case RadGridStringId.FilterFunctionNotIsEmpty
                    Return My.Resources.TelerikGridResx.str_FilterFunctionNotIsEmpty '"Is not empty"
                Case RadGridStringId.FilterFunctionNotIsNull
                    Return My.Resources.TelerikGridResx.str_FilterFunctionNotIsNull '"is not Null"
                Case RadGridStringId.FilterFunctionStartsWith
                    Return My.Resources.TelerikGridResx.str_FilterFunctionStartsWith '"Starts with"
                Case RadGridStringId.FilterFunctionCustom
                    Return My.Resources.TelerikGridResx.str_FilterFunctionCustom '"" '"Ma�geschneidert funktion"
                Case RadGridStringId.CustomFilterMenuItem
                    Return My.Resources.TelerikGridResx.str_CustomFilterMenuItem '"Ma�geschneidert Filter Men�punkt"
                Case RadGridStringId.CustomFilterDialogCaption
                    Return My.Resources.TelerikGridResx.str_CustomFilterDialogCaption '"Ma?geschneidert Filter Dialog"
                Case RadGridStringId.CustomFilterDialogLabel
                    Return My.Resources.TelerikGridResx.str_CustomFilterDialogLabel '"Zeig Zeilen die:"
                Case RadGridStringId.CustomFilterDialogRbAnd
                    Return My.Resources.TelerikGridResx.str_CustomFilterDialogRbAnd '"And"
                Case RadGridStringId.CustomFilterDialogRbOr
                    Return My.Resources.TelerikGridResx.str_CustomFilterDialogRbOr '"Or"
                Case RadGridStringId.CustomFilterDialogBtnOk
                    Return My.Resources.TelerikGridResx.str_CustomFilterDialogBtnOk '"OK"
                Case RadGridStringId.CustomFilterDialogBtnCancel
                    Return My.Resources.TelerikGridResx.str_CustomFilterDialogBtnCancel '"Cancel"
                Case RadGridStringId.DeleteRowMenuItem
                    Return My.Resources.TelerikGridResx.str_DeleteRowMenuItem '"Delete row"
                Case RadGridStringId.SortAscendingMenuItem
                    Return My.Resources.TelerikGridResx.str_SortAscendingMenuItem '"Sort Ascending"
                Case RadGridStringId.SortDescendingMenuItem
                    Return My.Resources.TelerikGridResx.str_SortDescendingMenuItem '"Sort descending"
                Case RadGridStringId.ClearSortingMenuItem
                    Return My.Resources.TelerikGridResx.str_ClearSortingMenuItem '"Clear sorting"
                Case RadGridStringId.ConditionalFormattingMenuItem
                    Return My.Resources.TelerikGridResx.str_ConditionalFormattingMenuItem '"Conditional formatting"
                Case RadGridStringId.GroupByThisColumnMenuItem
                    Return My.Resources.TelerikGridResx.str_GroupByThisColumnMenuItem '"Group by this column"
                Case RadGridStringId.UngroupThisColumn
                    Return My.Resources.TelerikGridResx.str_UngroupThisColumn '"Ungroup this column"
                Case RadGridStringId.ColumnChooserMenuItem
                    Return My.Resources.TelerikGridResx.str_ColumnChooserMenuItem '"Column chooser"
                Case RadGridStringId.HideMenuItem
                    Return My.Resources.TelerikGridResx.str_HideMenuItem '"Hide menu"
                Case RadGridStringId.UnpinMenuItem
                    Return My.Resources.TelerikGridResx.str_UnpinMenuItem '"Unpin menu"
                Case RadGridStringId.PinMenuItem
                    Return My.Resources.TelerikGridResx.str_PinMenuItem '"Pin menu"
                Case RadGridStringId.PinAtLeftMenuItem
                    Return My.Resources.TelerikGridResx.str_PinAtLeftMenuItem '"Pin at left menu"
                Case RadGridStringId.PinAtRightMenuItem
                    Return My.Resources.TelerikGridResx.str_PinAtRightMenuItem '"Pin at right menu"
                Case RadGridStringId.BestFitMenuItem
                    Return My.Resources.TelerikGridResx.str_BestFitMenuItem '"Best fit menu"
                Case RadGridStringId.PasteMenuItem
                    Return My.Resources.TelerikGridResx.str_PasteMenuItem '"Paste menu"
                Case RadGridStringId.EditMenuItem
                    Return My.Resources.TelerikGridResx.str_EditMenuItem '"Edit menu"
                Case RadGridStringId.CopyMenuItem
                    Return My.Resources.TelerikGridResx.str_CopyMenuItem '"Copy menu"
                Case RadGridStringId.AddNewRowString
                    Return My.Resources.TelerikGridResx.str_AddNewRowString '"Click here to add new row"
                Case RadGridStringId.ConditionalFormattingCaption
                    Return My.Resources.TelerikGridResx.str_ConditionalFormattingCaption '"Conditional formatting editor"
                Case RadGridStringId.ConditionalFormattingLblColumn
                    Return My.Resources.TelerikGridResx.str_ConditionalFormattingLblColumn '"Column:"
                Case RadGridStringId.ConditionalFormattingLblName
                    Return My.Resources.TelerikGridResx.str_ConditionalFormattingLblName '"Name:"
                Case RadGridStringId.ConditionalFormattingLblType
                    Return My.Resources.TelerikGridResx.str_ConditionalFormattingLblType '"Type:"
                Case RadGridStringId.ConditionalFormattingRuleAppliesOn
                    Return My.Resources.TelerikGridResx.str_ConditionalRuleAppliesOn '"Apply rule for:"
                Case RadGridStringId.ConditionalFormattingLblValue1
                    Return My.Resources.TelerikGridResx.str_ConditionalFormattingLblValue '"Value 1:"
                Case RadGridStringId.ConditionalFormattingLblValue2
                    Return My.Resources.TelerikGridResx.str_ConditionalFormattingLblValue1 '"Value 2:"
                Case RadGridStringId.ConditionalFormattingGrpConditions
                    Return My.Resources.TelerikGridResx.str_ConditionalGrpConditions '"Conditions"
                Case RadGridStringId.ConditionalFormattingGrpProperties
                    Return My.Resources.TelerikGridResx.str_ConditionalGrpProperties '"Properties"
                Case RadGridStringId.ConditionalFormattingChkApplyToRow
                    Return My.Resources.TelerikGridResx.str_Conditional_ApplyToRow '"Apply to row"
                Case RadGridStringId.ConditionalFormattingBtnAdd
                    Return My.Resources.TelerikGridResx.str_ConditionalFormattingBtnAdd '"Add"
                Case RadGridStringId.ConditionalFormattingBtnRemove
                    Return My.Resources.TelerikGridResx.str_ConditionalFormattingBtnRemove '"Remove"
                Case RadGridStringId.ConditionalFormattingBtnOK
                    Return My.Resources.TelerikGridResx.str_ConditionalFormattingBtnOK '"OK"
                Case RadGridStringId.ConditionalFormattingBtnCancel
                    Return My.Resources.TelerikGridResx.str_ConditionalFormattingBtnCancel '"Cancel"
                Case RadGridStringId.ConditionalFormattingBtnApply
                    Return My.Resources.TelerikGridResx.str_ConditionalFormattingBtnApply '"Apply"
                Case RadGridStringId.ColumnChooserFormCaption
                    Return My.Resources.TelerikGridResx.ColumnChooserFormCaption '"Caption"
                Case RadGridStringId.ColumnChooserFormMessage
                    Return My.Resources.TelerikGridResx.str_ColumnChooserFormMessage '"Um eine Spalte zu verstecken," & Constants.vbLf & "schieben Sie sie vom RadGridView" & Constants.vbLf & "auf dieses Fenster"
                Case RadGridStringId.CustomFilterDialogCheckBoxNot
                    Return My.Resources.TelerikGridResx.str_CustomFilterDialogCheckBoxNot '"Not"
                Case RadGridStringId.GroupingPanelDefaultMessage
                    Return "" '"!Drag a column here to group by this column"

                Case Else
                    Return MyBase.GetLocalizedString(id)
            End Select
        End Function

    End Class
#End Region
End Class





