Imports System.Data
Imports System
Imports KAM.INSPECTOR.IP.My.Resources
Imports Telerik.WinControls.UI
Imports Inspector.Model

Public Class usctrl_ScriptCommand41_ControlList
#Region "Class members"
    Event evntSelectionRequired(required As Boolean)
    Event evntCheckListItemSelected()

    Private changingCellValue As Boolean = False
    Private datatable As New DataTable
#End Region
#Region "Properties"
    ''' <summary>
    ''' Set script command 41 List information
    ''' </summary>
    ''' <value></value>
    ''' <remarks></remarks>
    Public WriteOnly Property Scriptcommand41List() As InspectionProcedure.ScriptCommand41List
        Set(ByVal value As InspectionProcedure.ScriptCommand41List)
            _scriptcommand41List = value
        End Set
    End Property
    Private _scriptcommand41List As New InspectionProcedure.ScriptCommand41List

    ''' <summary>
    ''' Set script command 41 List information
    ''' </summary>
    ''' <value></value>
    ''' <remarks></remarks>
    Public WriteOnly Property ShowNextListImmediately() As Boolean
        Set(ByVal value As Boolean)
            _showNextListImmediately = value
        End Set
    End Property
    Private _showNextListImmediately As Boolean

    ''' <summary>
    ''' An item in the list is selected. No other lists will be displayed
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property ItemNoNextListSelected As Boolean
        Get
            Return _itemNoNextListSelected
        End Get
    End Property
    Private _itemNoNextListSelected As Boolean = False

    ''' <summary>
    ''' The result of the selection
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property ListResult As String
        Get
            Return _listResult
        End Get
    End Property
    Private _listResult As String

    ''' <summary>
    ''' Set if the list already is displayed
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property ListCompleted As Boolean
        Get
            Return _listCompleted
        End Get
        Set(value As Boolean)
            _listCompleted = value
        End Set
    End Property
    Private _listCompleted As Boolean
#End Region

#Region "Constructor"
    Public Sub Intializedata()
        With Me.rdGridConditionCodes

            .BeginUpdate()
            .Columns.Clear()
            .MasterTemplate.AllowAddNewRow = False
            .MasterTemplate.AutoGenerateColumns = True
            .MasterTemplate.Reset()
            .TableElement.RowHeight = 30
            .AutoGenerateHierarchy = True
            .UseScrollbarsInHierarchy = False
            .MultiSelect = False


            .ReadOnly = True
            .EnableFiltering = False
            .EnableGrouping = False
            .EnableSorting = False
            .AllowColumnChooser = False
            .EnableAlternatingRowColor = True


            .DataSource = _scriptcommand41List.ListConditionCodes

            'Add columns for OptionList or CheckList
            Select Case _scriptcommand41List.ListType
                Case ListType.OptionList
                    Dim optieCheckColumn As New GridViewCheckBoxColumn(ListDataFields.columnOptionList)
                    optieCheckColumn.HeaderText = InspectionProcedureResx.str_SC41List_column_OptionList
                    .Columns.Insert(0, optieCheckColumn)
                Case ListType.CheckList
                    Dim checkYesColumn As New GridViewCheckBoxColumn(ListDataFields.columnCheckYesList)
                    checkYesColumn.HeaderText = InspectionProcedureResx.str_SC41List_column_CheckListYes
                    .Columns.Insert(0, checkYesColumn)

                    Dim checkNoColumn As New GridViewCheckBoxColumn(ListDataFields.columnCheckNoList)
                    checkNoColumn.HeaderText = InspectionProcedureResx.str_SC41List_column_CheckListNo
                    .Columns.Insert(1, checkNoColumn)
            End Select

            .Columns(ListDataFields.columnConditionCode).HeaderText = InspectionProcedureResx.str_SC41List_column_ConditionCode
            .Columns(ListDataFields.columnConditionCodeDescription).HeaderText = InspectionProcedureResx.str_SC41List_column_ConditionCodeDescription
            .Columns(ListDataFields.columnConditionDisplayNextList).HeaderText = InspectionProcedureResx.str_SC41List_column_DoNotDisplayNextList

            '.MasterTemplate.AutoSizeColumnsMode = GridViewAutoSizeColumnsMode.Fill
            .MasterTemplate.BestFitColumns()

            .EndUpdate()

        End With

        rdCheckListResult.Checked = _scriptcommand41List.CheckListResult
        rdCheckOneSelectionAllowed.Checked = _scriptcommand41List.OneSelectionAllowed
        rdCheckRequired.Checked = _scriptcommand41List.SelectionRequired
        rdtbInstruction.Text = _scriptcommand41List.ListQuestion.ToString
    End Sub
#End Region

#Region "Grid click and check"
    Private Sub rdGridConditionCodes_CellClick(sender As Object, e As Telerik.WinControls.UI.GridViewCellEventArgs) Handles rdGridConditionCodes.CellClick
        'Apply EndEdit; This is done to get the correct behavior for setting the value in the checkbox. 
        'If this is not done; in some cases the value is not set.
        If _scriptcommand41List.ListType = ListType.OptionList Then GridHandlingOptionList(e)
        If _scriptcommand41List.ListType = ListType.CheckList Then GridHandlingCheckList(e)

    End Sub
    ''' <summary>
    ''' Setting and check function for option List
    ''' </summary>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub GridHandlingOptionList(e As Telerik.WinControls.UI.GridViewCellEventArgs)
        If e.Row.Index < 0 Then Exit Sub
        Dim lbShowNextButton As Boolean = False

        rdGridConditionCodes.Rows(e.Row.Index).Cells(ListDataFields.columnOptionList).Value = Not rdGridConditionCodes.Rows(e.Row.Index).Cells(ListDataFields.columnOptionList).Value
        
        'Only one selection is Allowed
        If _scriptcommand41List.OneSelectionAllowed = True Then
            'Set all other check boxes on empty value
            For i = 0 To rdGridConditionCodes.RowCount - 1
                If i <> e.RowIndex Then rdGridConditionCodes.Rows(i).Cells(ListDataFields.columnOptionList).Value = False
                If rdGridConditionCodes.Rows(i).Cells(ListDataFields.columnOptionList).Value = True Then
                    lbShowNextButton = True
                End If
            Next
            RaiseEvent evntSelectionRequired(lbShowNextButton)
            Exit Sub
        End If

        'Check if the items is marked as do not display next list.
        'MOD 40
        For i = 0 To rdGridConditionCodes.RowCount - 1
            If rdGridConditionCodes.Rows(i).Cells(ListDataFields.columnOptionList).Value = True And rdGridConditionCodes.Rows(i).Cells(ListDataFields.columnConditionDisplayNextList).Value = True Then
                RaiseEvent evntSelectionRequired(True)
                Exit Sub
            End If
        Next i
        lbShowNextButton = False
        If _scriptcommand41List.SelectionRequired Then
            'Check if all values are set

            For i = 0 To rdGridConditionCodes.RowCount - 1
                If rdGridConditionCodes.Rows(i).Cells(ListDataFields.columnOptionList).Value = True Then
                    lbShowNextButton = True
                    Exit For
                End If
            Next
            RaiseEvent evntSelectionRequired(lbShowNextButton)
            Exit Sub
        End If

    End Sub


    ''' <summary>
    ''' Setting and check function for CheckList
    ''' </summary>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub GridHandlingCheckList(e As Telerik.WinControls.UI.GridViewCellEventArgs)
        If e.Row.Index < 0 Then Exit Sub

        'If display next list is set for selected item
        'MOD 40
        If rdGridConditionCodes.Rows(e.Row.Index).Cells(ListDataFields.columnConditionDisplayNextList).Value = True Then
            If rdGridConditionCodes.Rows(e.Row.Index).Cells(ListDataFields.columnCheckYesList).Value = True Then
                'in case a value already was set clear both check boxes.
                e.Row.Cells(ListDataFields.columnCheckNoList).Value = False
                e.Row.Cells(ListDataFields.columnCheckYesList).Value = False
            Else
                SetColumnBooleanChecklist(e, True)
            End If
        Else
            Select Case e.Column.Name
                Case ListDataFields.columnCheckYesList
                    SetColumnBooleanChecklist(e, True)
                Case ListDataFields.columnCheckNoList
                    SetColumnBooleanChecklist(e, False)
            End Select
        End If

        'Check if a record is mark as display next list
        'MOD 40
        Dim doNotDisplayNextList As Boolean = False
        For i = 0 To rdGridConditionCodes.RowCount - 1
            If rdGridConditionCodes.Rows(i).Cells(ListDataFields.columnConditionDisplayNextList).Value = True And rdGridConditionCodes.Rows(i).Cells(ListDataFields.columnCheckYesList).Value = True Then
                doNotDisplayNextList = True
            End If
        Next i

        'MOD 40
        If doNotDisplayNextList = True Then
            For i = 0 To rdGridConditionCodes.RowCount - 1
                If e.RowIndex <> i Then
                    'Clear all other checkboxes.
                    rdGridConditionCodes.Rows(i).Cells(ListDataFields.columnCheckYesList).Value = False
                    rdGridConditionCodes.Rows(i).Cells(ListDataFields.columnCheckNoList).Value = False
                End If
            Next
        End If


        'Check if all values are set
        Dim lbShowNextButton As Boolean = True
        'For all rows; A checkbox should be selected.
        For i = 0 To rdGridConditionCodes.RowCount - 1
            If rdGridConditionCodes.Rows(i).Cells(ListDataFields.columnCheckYesList).Value = False And rdGridConditionCodes.Rows(i).Cells(ListDataFields.columnCheckNoList).Value = False And rdGridConditionCodes.Rows(i).Cells(ListDataFields.columnConditionDisplayNextList).Value = False Then
                lbShowNextButton = False
                Exit For

                'MOD 40
            ElseIf (rdGridConditionCodes.Rows(i).Cells(ListDataFields.columnCheckYesList).Value = True Or rdGridConditionCodes.Rows(i).Cells(ListDataFields.columnCheckNoList).Value = True) And rdGridConditionCodes.Rows(i).Cells(ListDataFields.columnConditionDisplayNextList).Value = True Then
                'Check if the value display next list is set. Now the next step button is displayed
                lbShowNextButton = True
                Exit For
            End If
        Next


        'Raise event to enable or disable Next button
        RaiseEvent evntSelectionRequired(lbShowNextButton)

    End Sub

    ''' <summary>
    ''' Set checkbox value for columns Yes and No; Column no = !lbvalueCorrect
    ''' </summary>
    ''' <param name="e"></param>
    ''' <param name="lbValueCorrect"></param> If value true Yes checkbox is checked; No checkbox is unchecked
    ''' <remarks></remarks>
    Private Sub SetColumnBooleanChecklist(e As Telerik.WinControls.UI.GridViewCellEventArgs, lbValueCorrect As Boolean)
        e.Row.Cells(ListDataFields.columnCheckNoList).Value = Not lbValueCorrect
        e.Row.Cells(ListDataFields.columnCheckYesList).Value = lbValueCorrect
    End Sub
#End Region
    ''' <summary>
    ''' Create the result for checklist of optionlist
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub CreateListResult()
        If _scriptcommand41List.ListType = ListType.OptionList Then createResultOptionList(ListDataFields.columnOptionList)
        If _scriptcommand41List.ListType = ListType.CheckList Then createResultOptionList(ListDataFields.columnCheckNoList)
    End Sub
    ''' <summary>
    ''' Create a result for an option list
    ''' In case of checkListResult = true only the checked items are saved
    ''' In case of checkListresult = false checked and unchecked are save. If checked ";1" is added, unchecked ";0" is added 
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub createResultOptionList(columnNameValue As String)

        'Check if a next list should be displayed
        If _scriptcommand41List.ListType = ListType.OptionList Then
            'For an optionList
            _itemNoNextListSelected = False
            For i = 0 To rdGridConditionCodes.RowCount - 1
                If rdGridConditionCodes.Rows(i).Cells(ListDataFields.columnConditionDisplayNextList).Value = True And rdGridConditionCodes.Rows(i).Cells(ListDataFields.columnOptionList).Value = True Then
                    _itemNoNextListSelected = True
                    Exit For
                End If
            Next
        Else
            'MOD 40
            'check if an item with no next list display is selected
            _itemNoNextListSelected = True
            For i = 0 To rdGridConditionCodes.RowCount - 1
                If rdGridConditionCodes.Rows(i).Cells(ListDataFields.columnCheckNoList).Value = True And rdGridConditionCodes.Rows(i).Cells(ListDataFields.columnConditionDisplayNextList).Value = False Then
                    _itemNoNextListSelected = False
                    Exit For
                End If
            Next
        End If


        _listResult = ""
        Dim lbSetValue As Boolean = False
        Dim charSep As String = My.Resources.InspectionProcedureResx.str_SC41_separator '"~"
        If _scriptcommand41List.CheckListResult = True Then
            For i = 0 To rdGridConditionCodes.RowCount - 1
                If rdGridConditionCodes.Rows(i).Cells(columnNameValue).Value = True Then
                    If lbSetValue = True Then _listResult = _listResult & charSep
                    _listResult = _listResult & rdGridConditionCodes.Rows(i).Cells(ListDataFields.columnConditionCode).Value
                    lbSetValue = True
                End If
            Next
        Else
            For i = 0 To rdGridConditionCodes.RowCount - 1
                If i <> 0 Then _listResult = _listResult & charSep
                'MOD 72
                If rdGridConditionCodes.Rows(i).Cells(columnNameValue).Value = True Then
                    _listResult = _listResult & rdGridConditionCodes.Rows(i).Cells(ListDataFields.columnConditionCode).Value & ";1"
                Else
                    _listResult = _listResult & rdGridConditionCodes.Rows(i).Cells(ListDataFields.columnConditionCode).Value & ";0"
                End If
            Next
        End If
    End Sub
End Class
''' <summary>
''' The different fields in the XML file of plexor. Used in the grid of displaying the data
''' </summary>
''' <remarks></remarks>
Public Class ListDataFields
    Public Const columnConditionCode As String = "ConditionCode"
    Public Const columnConditionCodeDescription As String = "ConditionCodeDescription"

    Public Const columnConditionDisplayNextList As String = "DisplayNextList"

    Public Const columnOptionList As String = "OptionList"
    Public Const columnCheckYesList As String = "CorrectYes"
    Public Const columnCheckNoList As String = "CorrectNo"
End Class