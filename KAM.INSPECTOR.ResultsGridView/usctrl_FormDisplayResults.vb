Imports System.ComponentModel
Imports Inspector.BusinessLogic.Data.Reporting.Interfaces
Imports Inspector.Infra.Ioc
Imports Inspector.Model.InspectionReportingResults
Imports Inspector.Model
Imports Inspector.BusinessLogic.Data.Configuration.Interfaces.Managers
Imports Telerik.WinControls.UI
Imports KAM.INSPECTOR.Infra
Imports KAM.INSPECTOR.Infra.ClsCastingHelpers
Imports KAM.INSPECTOR.ResultsGridView.My.Resources

Imports Telerik.WinControls.Data
Imports Telerik.Reporting


Public Class usctrl_FormDisplayResults

#Region "Class members"
    Private m_InspectionResultReader As IInspectionResultReader
    Private m_gasControlLineInformation As InspectionProcedure.GasControlLineInformation
    Private m_StationInformationManager As IStationInformationManager        'Interface with station information

    Private m_InspectionResultsEntities As Model.ResultView.Entities.ResultsEntityStruct = New Model.ResultView.Entities.ResultsEntityStruct
    Private m_ListInspectionResultsEntities As Model.ResultView.Entities.ListInspectionResultsEntityStruct = New Model.ResultView.Entities.ListInspectionResultsEntityStruct
#End Region

    ''' <summary>
    ''' Initialize Form
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()
        ' This call is required by the designer.
        InitializeComponent()
        GridViewLayout(rdGridResultSelection)
        ' Add any initialization after the InitializeComponent() call.
    End Sub

#Region "Properties"
    ''' <summary>
    ''' Gets or sets the inspection information manager.
    ''' </summary>
    ''' <value>
    ''' The inspection information manager.
    ''' </value>
    Public Property StationInformationManager() As IStationInformationManager
        Get
            If m_StationInformationManager Is Nothing Then
                m_StationInformationManager = ContextRegistry.Context.Resolve(Of IStationInformationManager)()
            End If
            Return m_StationInformationManager
        End Get
        Set(value As IStationInformationManager)
            m_StationInformationManager = value
        End Set

    End Property

    ''' <summary>
    ''' Gets or sets the inspection information manager.
    ''' </summary>
    ''' <value>
    ''' The inspection information manager.
    ''' </value>
    Public Property InspectionResultReader() As IInspectionResultReader
        Get
            If m_InspectionResultReader Is Nothing Then
                m_InspectionResultReader = ContextRegistry.Context.Resolve(Of IInspectionResultReader)()
            End If
            Return m_InspectionResultReader
        End Get
        Set(value As IInspectionResultReader)
            m_InspectionResultReader = value
        End Set

    End Property


    Public Property InspectionInformationManager() As IInspectionInformationManager
        Get
            If m_InspectionInformationManager Is Nothing Then
                m_InspectionInformationManager = ContextRegistry.Context.Resolve(Of IInspectionInformationManager)()
            End If
            Return m_InspectionInformationManager
        End Get
        Set(value As IInspectionInformationManager)
            m_InspectionInformationManager = value
        End Set
    End Property
    Private m_InspectionInformationManager As IInspectionInformationManager
#End Region

#Region "Update list PRS"

    ''' <summary>
    ''' Passing the data to the Inspection form. 
    ''' </summary>
    ''' <param name="prsName"></param>
    ''' <remarks></remarks>
    Public Sub UpdatePRSGCLInformation(prsName As String)
        rlblPRSName.Text = prsName
        ' ReportViewer1.ClearHistory()
        If IsNothing(prsName) Then Exit Sub

        CreateListInspectionResultsEntity(prsName)
        'Setting the datasource of the gridview
        rdGridResultSelection.DataSource = Nothing
        rdGridResultSelection.DataSource = m_ListInspectionResultsEntities.InspectionResults

        rdGridResultSelection.ShowGroupPanel = False

        Dim descriptor As New GroupDescriptor()
        descriptor.GroupNames.Add("GCLName", ListSortDirection.Ascending)
        Me.rdGridResultSelection.GroupDescriptors.Add(descriptor)

        If m_ListInspectionResultsEntities.InspectionResults Is Nothing Then
            ReportViewer1.ReportSource = Nothing
            ReportViewer1.RefreshReport()
            ReportViewer1.ClearHistory()
        End If
        If m_ListInspectionResultsEntities.InspectionResults.Count = 0 Then
            ReportViewer1.ReportSource = Nothing
            ReportViewer1.RefreshReport()
            ReportViewer1.ClearHistory()
        End If
        SetColumnsResultsSelectionGrid()
        UpdateInspectionReport(Me.rdGridResultSelection.CurrentRow)
    End Sub

    Private Sub rdGridResultSelection_GroupSummaryEvaluate(ByVal sender As Object, ByVal e As Telerik.WinControls.UI.GroupSummaryEvaluationEventArgs) Handles rdGridResultSelection.GroupSummaryEvaluate
        If e.SummaryItem.Name = "GCLName" Then
            e.FormatString = [String].Format(e.Value)
        End If
    End Sub


    ''' <summary>
    ''' Update the information in the GUI when a row is selected in the grid
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub rdGridResultSelection_CurrentRowChanged(ByVal sender As Object, ByVal e As Telerik.WinControls.UI.CurrentRowChangedEventArgs) Handles rdGridResultSelection.CurrentRowChanged
        UpdateInspectionReport(Me.rdGridResultSelection.CurrentRow)
    End Sub


    ''' <summary>
    ''' Update the report, with the information of the selected inspection date
    ''' </summary>
    ''' <param name="currentRow"></param>
    ''' <remarks></remarks>
    Private Sub UpdateInspectionReport(ByVal currentRow As GridViewRowInfo)
        If currentRow IsNot Nothing AndAlso Not (TypeOf currentRow Is GridViewNewRowInfo) Then
            If currentRow.HierarchyLevel = 1 Then
                Dim prsNameCellvalue As String
                Dim gclNameCellvalue As String
                Dim inspectionProcedureName As String
                Dim DateTimeCellValue As String
                prsNameCellvalue = currentRow.Cells("PRSName").Value
                gclNameCellvalue = currentRow.Cells("GCLName").Value
                inspectionProcedureName = currentRow.Cells("InspectionProcedure").Value
                DateTimeCellValue = currentRow.Cells("DateTime").Value
                If DateTimeCellValue IsNot Nothing Then
                    Dim resultEntityValues As New Model.ResultView.Entities.ResultsEntityStruct
                    resultEntityValues = CreateInspectionResultsEntity(prsNameCellvalue, gclNameCellvalue, inspectionProcedureName, DateTimeCellValue)
                    If "- " & prsNameCellvalue = gclNameCellvalue Then
                        PopulatieReportInspectionResults(resultEntityValues, "PRS")
                    Else
                        PopulatieReportInspectionResults(resultEntityValues, "GCL")
                    End If
                End If
            End If
        End If
    End Sub

#End Region

#Region "Handling of All inspection results list"

    ''' <summary>
    ''' Create list with the Inspection dates from the selected PRS
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function CreateListInspectionResultsEntity(prsName As String)
        Dim resultValuesList As New List(Of ReportInspectionResult)
        m_ListInspectionResultsEntities.InspectionResults.Clear()
        'Search the gas control lines of the selected PRS
        Dim gasControlLines As InspectionProcedure.StationInformation
        gasControlLines = StationInformationManager.LookupStationInformation(prsName)

        'Search for PRS results only
        resultValuesList = InspectionResultReader.LookupAllResults(prsName)

        If resultValuesList IsNot Nothing Then
            For j As Integer = 0 To resultValuesList.Count - 1
                Dim newEntity As New Model.ResultView.Entities.ListInspectionResultsEntityStruct.InspectionResult_entity

                newEntity.PRSName = resultValuesList.Item(j).PRSName
                newEntity.GCLName = "- " & resultValuesList.Item(j).PRSName
                newEntity.InspectionProcedure = resultValuesList.Item(j).InspectionProcedureName
                Dim oDate As DateTime = Convert.ToDateTime(resultValuesList.Item(j).DateTimeStamp.StartDate & " " & resultValuesList.Item(j).DateTimeStamp.StartTime)
                newEntity.DateTime = oDate

                m_ListInspectionResultsEntities.InspectionResults.Add(newEntity)
            Next
        End If

        If gasControlLines.GasControlLines IsNot Nothing Then

            For i As Integer = 0 To gasControlLines.GasControlLines.Count - 1
                'Search for a result of the selected station / gas control line
                resultValuesList = InspectionResultReader.LookupAllResults(rlblPRSName.Text, gasControlLines.GasControlLines(i).GasControlLineName)
                'If no result is found
                If resultValuesList IsNot Nothing Then
                    For j As Integer = 0 To resultValuesList.Count - 1
                        Dim newEntity As New Model.ResultView.Entities.ListInspectionResultsEntityStruct.InspectionResult_entity

                        newEntity.PRSName = resultValuesList.Item(j).PRSName
                        newEntity.GCLName = resultValuesList.Item(j).GasControlLineName
                        newEntity.InspectionProcedure = resultValuesList.Item(j).InspectionProcedureName
                        Dim oDate As DateTime = Convert.ToDateTime(resultValuesList.Item(j).DateTimeStamp.StartDate & " " & resultValuesList.Item(j).DateTimeStamp.StartTime)
                        newEntity.DateTime = oDate

                        m_ListInspectionResultsEntities.InspectionResults.Add(newEntity)
                    Next
                End If
            Next
        End If
    End Function

#End Region

#Region "Handling of the inspectionResult"

    ''' <summary>
    ''' Create a dataobject of inspection results. 
    ''' The information is collected from the inspection procedure, station information file
    ''' and the inspection results
    ''' </summary>
    ''' <param name="prsName"></param>
    ''' <param name="gclName"></param>
    ''' <param name="inspectionDateTimeSearch"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function CreateInspectionResultsEntity(prsName As String, gclName As String, inspectionProcedureName As String, inspectionDateTimeSearch As DateTime) As Model.ResultView.Entities.ResultsEntityStruct

        If "- " & prsName = gclName Then gclName = ""

        'Loading the Results
        Dim resultvaluesList As New List(Of ReportInspectionResult)
        resultvaluesList = InspectionResultReader.LookupAllResults(prsName, gclName)

        'Check first if the inspection procedure exists
        'TODO Check if procedure exists
        Dim lbCheck As Boolean
        lbCheck = InspectionInformationManager.InspectionProcedureExists(inspectionProcedureName)

        If lbCheck = False Then Return Nothing

        Dim inspectionProcedureList As InspectionProcedure.InspectionProcedureInformation
        inspectionProcedureList = InspectionInformationManager.LookupInspectionProcedure(inspectionProcedureName, prsName, gclName)

        '"Kamstrup as PN16 (ohne Whu)"
        Dim foundReportInspectionResult As New Model.ResultView.Entities.ResultsEntityStruct()

        'Search for PRS/ GCL and date time (selected result record)
        For i As Integer = 0 To resultvaluesList.Count - 1

            'Searching for the Result
            If resultvaluesList.Item(i).PRSName = prsName And resultvaluesList.Item(i).GasControlLineName = gclName Then
                'Search on the correct datatime
                Dim oDate As DateTime = Convert.ToDateTime(resultvaluesList.Item(i).DateTimeStamp.StartDate & " " & resultvaluesList.Item(i).DateTimeStamp.StartTime)

                If oDate = inspectionDateTimeSearch Then
                    foundReportInspectionResult.CRC = resultvaluesList.Item(i).CRC
                    foundReportInspectionResult.DateTimeStamp = resultvaluesList.Item(i).DateTimeStamp
                    foundReportInspectionResult.GasControlLineName = resultvaluesList.Item(i).GasControlLineName
                    foundReportInspectionResult.GCLCode = resultvaluesList.Item(i).GCLCode
                    foundReportInspectionResult.GCLIdentification = resultvaluesList.Item(i).GCLIdentification
                    foundReportInspectionResult.InspectionProcedureName = resultvaluesList.Item(i).InspectionProcedureName
                    foundReportInspectionResult.InspectionProcedureVersion = resultvaluesList.Item(i).InspectionProcedureVersion
                    foundReportInspectionResult.Measurement_Equipment = resultvaluesList.Item(i).Measurement_Equipment
                    foundReportInspectionResult.PRSCode = resultvaluesList.Item(i).PRSCode
                    foundReportInspectionResult.PRSIdentification = resultvaluesList.Item(i).PRSIdentification
                    foundReportInspectionResult.PRSName = resultvaluesList.Item(i).PRSName
                    foundReportInspectionResult.Status = resultvaluesList.Item(i).Status


                    Dim ipSection As String = ""
                    Dim ipSubSection As String = ""
                    Dim ipScriptcommand As String = ""

                    Dim ipObjectDescription As String = ""
                    Dim ipMeasurePointDescription As String = ""

                    Dim inspectorStepInformation As InspectionProcedure.StationStepObject = Nothing
                    Dim scriptCommandList As List(Of InspectionProcedure.ScriptCommand41List) = Nothing

                    Dim ipScText As String = ""
                    Dim resultUnit As String = ""

                    Dim handlingResultEntity As Boolean = False

                    For k As Integer = 0 To inspectionProcedureList.ScriptCommandSequence.Count - 1
                        resultUnit = ""
                        scriptCommandList = Nothing
                        Select Case inspectionProcedureList.ScriptCommandSequence(k).GetType()
                            Case GetType(InspectionProcedure.ScriptCommand1)
                                'Dim sc1 As InspectionProcedure.ScriptCommand1
                                'Do nothing; for displaying text)
                                handlingResultEntity = False
                            Case GetType(InspectionProcedure.ScriptCommand2)
                                'Handling; Setting is Section and subsection
                                Dim sc2 As InspectionProcedure.ScriptCommand2
                                sc2 = TryCast(inspectionProcedureList.ScriptCommandSequence(k), InspectionProcedure.ScriptCommand2)
                                handlingResultEntity = False

                                ipSection = sc2.Section
                                ipSubSection = sc2.SubSection
                            Case GetType(InspectionProcedure.ScriptCommand3)
                                'Do nothing; for pause)
                                handlingResultEntity = False

                            Case GetType(InspectionProcedure.ScriptCommand4)
                                'Handling; Getting the question type
                                Dim sc4 As InspectionProcedure.ScriptCommand4
                                sc4 = TryCast(inspectionProcedureList.ScriptCommandSequence(k), InspectionProcedure.ScriptCommand4)
                                ipScText = sc4.Question

                                handlingResultEntity = True
                                ipScriptcommand = "SC4"

                                inspectorStepInformation = sc4.StationStepObject

                                ipObjectDescription = sc4.ObjectNameDescription
                                ipMeasurePointDescription = sc4.MeasurePointDescription

                            Case GetType(InspectionProcedure.ScriptCommand41)
                                'Handling; Getting the list
                                Dim sc41 As InspectionProcedure.ScriptCommand41
                                sc41 = TryCast(inspectionProcedureList.ScriptCommandSequence(k), InspectionProcedure.ScriptCommand41)
                                ipScText = ""

                                handlingResultEntity = True
                                ipScriptcommand = "SC41"

                                inspectorStepInformation = sc41.StationStepObject

                                scriptCommandList = sc41.ScriptCommandList

                                ipObjectDescription = sc41.ObjectNameDescription
                                ipMeasurePointDescription = sc41.MeasurePointDescription

                            Case GetType(InspectionProcedure.ScriptCommand41List)
                                handlingResultEntity = False
                            Case GetType(InspectionProcedure.ScriptCommand42)
                                'Handling; Remarks
                                Dim sc42 As InspectionProcedure.ScriptCommand42
                                sc42 = TryCast(inspectionProcedureList.ScriptCommandSequence(k), InspectionProcedure.ScriptCommand42)
                                ipScText = "Remarks"

                                handlingResultEntity = True
                                ipScriptcommand = "SC42"

                                inspectorStepInformation = sc42.StationStepObject

                                ipObjectDescription = sc42.ObjectNameDescription
                                ipMeasurePointDescription = sc42.MeasurePointDescription

                            Case GetType(InspectionProcedure.ScriptCommand43)
                                'Handling; Instruction 
                                Dim sc43 As InspectionProcedure.ScriptCommand43
                                sc43 = TryCast(inspectionProcedureList.ScriptCommandSequence(k), InspectionProcedure.ScriptCommand43)
                                ipScText = sc43.Instruction

                                handlingResultEntity = True
                                ipScriptcommand = "SC43"

                                inspectorStepInformation = sc43.StationStepObject

                                ipObjectDescription = sc43.ObjectNameDescription
                                ipMeasurePointDescription = sc43.MeasurePointDescription

                            Case GetType(InspectionProcedure.ScriptCommand5X)
                                'Handling; Instruction
                                Dim sc5x As InspectionProcedure.ScriptCommand5X
                                sc5x = TryCast(inspectionProcedureList.ScriptCommandSequence(k), InspectionProcedure.ScriptCommand5X)
                                ipScText = sc5x.Instruction

                                handlingResultEntity = True
                                ipScriptcommand = "SC5x"

                                inspectorStepInformation = sc5x.StationStepObject

                                ipObjectDescription = sc5x.ObjectNameDescription
                                ipMeasurePointDescription = sc5x.MeasurePointDescription

                            Case GetType(InspectionProcedure.ScriptCommand70)
                                'Do nothing; 
                                handlingResultEntity = False
                            Case Else
                                handlingResultEntity = False
                        End Select

                        If handlingResultEntity = True Then

                            Dim newEntity As New Model.ResultView.Entities.ResultsEntityStruct.Value_entity

                            newEntity.Section = ipSection
                            newEntity.SubSection = ipSubSection
                            newEntity.ScriptCommand = ipScriptcommand
                            If inspectorStepInformation.FieldNo IsNot Nothing Then
                                newEntity.FieldNo = inspectorStepInformation.FieldNo
                            Else
                                newEntity.FieldNo = -1
                            End If

                            'Setting object information
                            newEntity.ObjectName = inspectorStepInformation.ObjectName
                            newEntity.ObjectID = inspectorStepInformation.ObjectID
                            newEntity.ObjectDescription = ipObjectDescription

                            newEntity.MeasurePoint = inspectorStepInformation.MeasurePoint
                            newEntity.MeasurePointID = inspectorStepInformation.MeasurePointID
                            newEntity.MeasurePointDescription = ipMeasurePointDescription

                            newEntity.InstructionText = ipScText

                            If inspectorStepInformation.Boundaries IsNot Nothing Then
                                resultUnit = GetUomUnit(inspectorStepInformation.Boundaries.UOV)
                                newEntity.ValueMin = inspectorStepInformation.Boundaries.ValueMin & " " & resultUnit
                                newEntity.ValueMax = inspectorStepInformation.Boundaries.ValueMax & " " & resultUnit
                            End If

                            Dim valueActuel As New List(Of InspectionReportingResults.ReportResult)
                            If inspectorStepInformation.ObjectName = "" And inspectorStepInformation.MeasurePoint = "" Then
                                valueActuel = (From vActuel In resultvaluesList.Item(i).Results Where vActuel.FieldNo = newEntity.FieldNo Select vActuel).ToList
                            Else
                                valueActuel = (From vActuel In resultvaluesList.Item(i).Results Where vActuel.MeasurePoint = newEntity.MeasurePoint And vActuel.ObjectName = newEntity.ObjectName Select vActuel).ToList
                            End If

                            For Each vActuel In valueActuel
                                If Double.IsNaN(vActuel.MeasureValue.Value) Then
                                    If vActuel.Text = "" Then
                                        newEntity.ValueText = "-"
                                    Else
                                        newEntity.ValueText = vActuel.Text
                                    End If

                                Else
                                    resultUnit = GetUomUnit(vActuel.MeasureValue.UOM)
                                    newEntity.ValueActuel = vActuel.MeasureValue.Value & " " & resultUnit
                                End If

                                Dim oDate2 As DateTime = Convert.ToDateTime(resultvaluesList.Item(i).DateTimeStamp.StartDate)
                                newEntity.DateTime = DateAdd(DateInterval.Day, 3, oDate2).ToString & " " & resultvaluesList.Item(i).DateTimeStamp.StartTime.ToString
                                newEntity.MeasurePointID = vActuel.MeasurePointID
                                newEntity.ObjectID = vActuel.ObjectID
                            Next

                            ''Everything ok; Add to 
                            'If newEntity.ValueText = "" And ipScriptcommand = "SC41" Then

                            'Else
                            '    If ipScriptcommand = "SC41" Then newEntity.InstructionText = "Opmerking: "
                            '    foundReportInspectionResult.Values.Add(newEntity)
                            'End If

                            'For the list (scriptcommando 41) a copy is made from the original entity. This because the list may contains a result text value
                            Dim listValue As InspectionProcedure.ScriptCommand41List
                            If scriptCommandList IsNot Nothing Then
                                Dim newEntityList As New Model.ResultView.Entities.ResultsEntityStruct.Value_entity
                                newEntityList.Section = ipSection
                                newEntityList.SubSection = ipSubSection
                                newEntityList.ScriptCommand = ipScriptcommand
                                If inspectorStepInformation.FieldNo IsNot Nothing Then
                                    newEntityList.FieldNo = inspectorStepInformation.FieldNo
                                Else
                                    newEntityList.FieldNo = -1
                                End If

                                'Setting object information
                                newEntityList.ObjectName = inspectorStepInformation.ObjectName
                                newEntityList.ObjectID = inspectorStepInformation.ObjectID
                                newEntityList.ObjectDescription = ipObjectDescription

                                newEntityList.MeasurePoint = inspectorStepInformation.MeasurePoint
                                newEntityList.MeasurePointID = inspectorStepInformation.MeasurePointID
                                newEntityList.MeasurePointDescription = ipMeasurePointDescription

                                newEntityList.InstructionText = ipScText

                                Dim listCounter As Integer = 1
                                For Each listValue In scriptCommandList
                                    Dim newList As Model.ResultView.Entities.ResultsEntityStruct.List_Value
                                    If valueActuel.Count = 0 Then
                                        newList = New Model.ResultView.Entities.ResultsEntityStruct.List_Value
                                        newList.ListText = "-"
                                        newList.ListCode = "-"
                                        newList.ListQuestion = listValue.ListQuestion
                                        newEntityList.ListSC4.Add(newList)
                                    End If
                                    For Each vActuel In valueActuel
                                        newList = New Model.ResultView.Entities.ResultsEntityStruct.List_Value
                                        If vActuel.List IsNot Nothing And listCounter <= vActuel.List.Count Then 'MOD xxx
                                            Dim listText As New List(Of InspectionProcedure.ListConditionCode)
                                            listText = (From ListvalueSearch In listValue.ListConditionCodes Where ListvalueSearch.ConditionCode = vActuel.List(listCounter - 1).ToString Select ListvalueSearch).ToList
                                            If listText.Count > 0 Then
                                                newList.ListText = listText(0).ConditionCodeDescription
                                                newList.ListCode = vActuel.List(listCounter - 1).ToString
                                                newList.ListQuestion = listValue.ListQuestion
                                            Else
                                                newList.ListText = "-"
                                                newList.ListCode = "-"
                                                newList.ListQuestion = listValue.ListQuestion
                                            End If

                                        Else
                                            newList.ListText = "-"
                                            newList.ListCode = "-"
                                            newList.ListQuestion = listValue.ListQuestion

                                        End If
                                        newEntityList.ListSC4.Add(newList)
                                    Next
                                    listCounter += 1

                                Next
                                foundReportInspectionResult.Values.Add(newEntityList)
                            End If
                            'Everything ok; Add to 
                            If newEntity.ValueText = "-" And ipScriptcommand = "SC41" Then

                            Else
                                If ipScriptcommand = "SC41" Then newEntity.InstructionText = ResultsResx.str_Remark & ": "
                                foundReportInspectionResult.Values.Add(newEntity)
                            End If
                        End If
                    Next
                    Return foundReportInspectionResult
                End If
            End If
        Next



    End Function


#End Region

#Region "Misc"
    Private Function GetUomUnit(valueUnit As UnitOfMeasurement) As String
        Select Case valueUnit
            Case CastToTypeUnitsValueOrUnset(ModPlexorUnits.UnitHighPressure)
                Return (ModPlexorUnits.UnitHighPressure.ToString)
            Case CastToTypeUnitsValueOrUnset(ModPlexorUnits.UnitLowPressure)
                Return (ModPlexorUnits.UnitLowPressure.ToString)
            Case CastToTypeUnitsValueOrUnset(ModPlexorUnits.UnitChangeRate)
                Return (ModPlexorUnits.UnitChangeRate.ToString)
            Case CastToTypeUnitsValueOrUnset(ModPlexorUnits.UnitQvsLeakage)
                Return (ModPlexorUnits.UnitQvsLeakage.ToString)
        End Select
    End Function
#End Region

#Region "GridProperties"
    ''' <summary>
    ''' Setting the column properties of the grid.
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub SetColumnsResultsSelectionGrid()
        Me.rdGridResultSelection.MasterTemplate.ShowChildViewCaptions = False

        Try
            With Me.rdGridResultSelection.MasterTemplate
                .Columns.Move(3, 0)

                .Columns("DateTime").HeaderText = ResultsResx.str_FldDateTime
                .Columns("DateTime").MinWidth = 200
                .Columns("InspectionProcedure").HeaderText = ResultsResx.str_FldGCLInspectionProcedure

                .Columns("PRSName").HeaderText = ResultsResx.str_FldGCLName
                .Columns("PRSName").IsVisible = False
                .Columns("PRSName").MinWidth = 50
                .Columns("PRSName").HeaderText = ResultsResx.str_FldPRSName

                .Columns("GCLName").IsVisible = True
                .Columns("GCLName").MinWidth = 400
                .Columns("GCLName").HeaderText = ""
                .ShowColumnHeaders = False

                .ExpandAllGroups()
                .BestFitColumns()
                .Refresh()

            End With
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub

    ''' <summary>
    ''' Formating the Grid properties
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub GridViewLayout(rdGrid As RadGridView)
        With rdGrid

            .EnableSorting = False
            .EnableGrouping = True
            .ShowGroupPanel = False
            .EnableFiltering = False
            .AllowColumnChooser = False
            .AllowAddNewRow = False
            .AllowRowReorder = False
            ' 
            .ReadOnly = True
            .MultiSelect = False

            .AllowColumnHeaderContextMenu = False
            .AllowCellContextMenu = False

            .AutoGenerateHierarchy = True
            .UseScrollbarsInHierarchy = True

            .ShowItemToolTips = False

            .BestFitColumns(BestFitColumnMode.DisplayedDataCells)
            .AutoSizeColumnsMode = GridViewAutoSizeColumnsMode.Fill

            .EnableAlternatingRowColor = True
            '.AutoSizeRows = True
            .TableElement.RowHeight = 30
            .TableElement.ShowSelfReferenceLines = True



            '.EnableSorting = True
            '.EnableGrouping = False
            '.EnableFiltering = True
            '.AllowColumnChooser = True
            '.EndUpdate()
            '.AllowAddNewRow = False
            '.AllowRowReorder = False
            '.ReadOnly = True
            '.MultiSelect = False

            '.AllowColumnHeaderContextMenu = True
            '.AllowCellContextMenu = False

            '.AutoGenerateHierarchy = True
            '.UseScrollbarsInHierarchy = True

            '.ShowItemToolTips = True

            '.BestFitColumns()

            '.EnableAlternatingRowColor = True
            '',.AutoSizeRows = True
            '.TableElement.RowHeight = 40

            '.TableElement.ShowSelfReferenceLines = True


        End With
    End Sub
#End Region

#Region "Populate Report"
    ''' <summary>
    ''' Populate the Report with the correct datasource
    ''' </summary>
    ''' <param name="resultValues"></param>
    ''' <remarks></remarks>
    Private Sub PopulatieReportInspectionResults(resultValues As Model.ResultView.Entities.ResultsEntityStruct, subReportType As String)
        Dim objectDataSource As New Telerik.Reporting.ObjectDataSource()

        objectDataSource.DataSource = resultValues 'Setting the datasource
        'objectDataSource.DataMember = "Values" ' Indicating the exact table to bind to. If the DataMember is not specified the first data table will be used.

        'Attach to report InspectionResults
        Dim report As New ReportInspectionResults

        ' Assigning the ObjectDataSource component to the DataSource property of the report.
        report.DataSource = objectDataSource

        ' Use the InstanceReportSource to pass the report to the viewer for displaying.
        Dim reportSource As New Telerik.Reporting.InstanceReportSource
        reportSource.ReportDocument = report

        UpdateReportSource(reportSource, subReportType)

        ReportViewer1.ClearHistory()
        ' Assigning the report to the report viewer.
        ReportViewer1.ReportSource = reportSource

        ' Calling the RefreshReport method in case this is a WinForms application.
        ReportViewer1.RefreshReport()
    End Sub
    ''' <summary>
    ''' Change the reportsource of the subreport 
    ''' </summary>
    ''' <param name="reportItemBase"></param>
    ''' <param name="subReportType">GCL or PRS</param>
    ''' <remarks></remarks>
    Public Sub SetReportsubReportSource(reportItemBase As ReportItemBase, subReportType As String)

        If reportItemBase.Items.Count < 1 Then Return

        For Each item As Telerik.Reporting.ReportItemBase In reportItemBase.Items

            If TypeOf item Is DetailSection Then
                For Each itemDetail As Telerik.Reporting.ReportItemBase In item.Items
                    If TypeOf itemDetail Is SubReport Then
                        ' Use the InstanceReportSource to pass the report to the viewer for displaying.
                        'Attach to report InspectionResults
                        Dim reportGcl As New subReportInspectionResults
                        Dim reportPrs As New SubReportPRSInspectionResults


                        Dim reportSource As New Telerik.Reporting.InstanceReportSource
                        If subReportType = "GCL" Then
                            reportSource.ReportDocument = reportGcl
                        Else
                            reportSource.ReportDocument = reportPrs
                        End If

                        Dim subReport = DirectCast(itemDetail, SubReport)
                        subReport.ReportSource = reportSource ' Me.UpdateReportSource(reportSource, subReportType)

                        Exit For
                    End If
                Next
            End If
        Next
    End Sub

    ''' <summary>
    ''' Update the report source
    ''' </summary>
    ''' <param name="sourceReportSource"></param>
    ''' <param name="subReportType"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function UpdateReportSource(sourceReportSource As ReportSource, subReportType As String) As ReportSource
        If TypeOf sourceReportSource Is InstanceReportSource Then
            Dim instanceReportSource = DirectCast(sourceReportSource, InstanceReportSource)
            Me.SetReportsubReportSource(DirectCast(instanceReportSource.ReportDocument, ReportItemBase), subReportType)
            Return instanceReportSource
        End If

        'If TypeOf sourceReportSource Is TypeReportSource Then
        '    Dim typeReportSource = DirectCast(sourceReportSource, TypeReportSource)
        '    Dim typeName = typeReportSource.TypeName
        '    ValidateReportSource(typeName)
        '    Dim reportType = Type.[GetType](typeName)
        '    Dim reportInstance = DirectCast(Activator.CreateInstance(reportType), Report)
        '    Me.SetReportsubReportSource(DirectCast(reportInstance, ReportItemBase), subReportType)
        '    Return CreateInstanceReportSource(reportInstance, typeReportSource)
        'End If

        'Throw New NotImplementedException("Handler for the used ReportSource type is not implemented.")

    End Function

#End Region

#Region "Not Used"

    '' ''Getting Min/max and objectID
    ' ''If gclName = "" Then
    ' ''    newEntity = searchPRSObject(prsName, ipFieldNo, ipMeasurePoint, ipObjectName)
    ' ''Else
    ' ''    newEntity = searchGclObject(prsName, gclName, ipFieldNo, ipMeasurePoint, ipObjectName)
    ' ''End If

    ' ''If sc4.FieldNo IsNot Nothing Then ipFieldNo = sc4.FieldNo

    ' '' ''' <summary>
    ' '' ''' Search for the correct GCL object
    ' '' ''' </summary>
    ' '' ''' <param name="prsName"></param>
    ' '' ''' <param name="gclName"></param>
    ' '' ''' <param name="fieldNo"></param>
    ' '' ''' <returns></returns>
    ' '' ''' <remarks></remarks>
    ' ''Private Function searchGclObject(prsName As String, gclName As String, fieldNo As Integer, objectName As String, measurePoint As String) As Model.ResultView.Entities.ResultsEntityStruct.Value_entity
    ' ''    Dim gasControlLineInformation As InspectionProcedure.GasControlLineInformation
    ' ''    'Search for the gascontrol line in the station file
    ' ''    gasControlLineInformation = StationInformationManager.LookupGasControlLineInformation(prsName, gclName)
    ' ''    Dim newEntity As New Model.ResultView.Entities.ResultsEntityStruct.Value_entity

    ' ''    Dim gclObjects
    ' ''    If objectName = "" And measurePoint = "" Then
    ' ''        gclObjects = From gclObject In gasControlLineInformation.GclObjects Where gclObject.FieldNo = fieldNo
    ' ''    Else
    ' ''        gclObjects = From gclObject In gasControlLineInformation.GclObjects Where gclObject.MeasurePoint = measurePoint And gclObject.ObjectName = objectName
    ' ''    End If

    ' ''    For Each gclObject In gclObjects
    ' ''        If gclObject.GclBoundary IsNot Nothing Then
    ' ''            newEntity.ValueMin = gclObject.GclBoundary.ValueMin & " " & gclObject.GclBoundary.UOV
    ' ''            newEntity.ValueMax = gclObject.GclBoundary.ValueMax & " " & gclObject.GclBoundary.UOV
    ' ''        End If
    ' ''        newEntity.MeasurePointID = gclObject.MeasurePointId
    ' ''        newEntity.ObjectID = gclObject.ObjectId
    ' ''    Next
    ' ''    Return newEntity
    ' ''End Function

    ' '' ''' <summary>
    ' '' ''' Search for the correct GCL object
    ' '' ''' </summary>
    ' '' ''' <param name="prsName"></param>
    ' '' ''' <param name="fieldNo"></param>
    ' '' ''' <returns></returns>
    ' '' ''' <remarks></remarks>
    ' ''Private Function searchPRSObject(prsName As String, fieldNo As Integer, objectName As String, measurePoint As String) As Model.ResultView.Entities.ResultsEntityStruct.Value_entity

    ' ''    'Search for the gascontrol line in the station file
    ' ''    Dim stationInformation As InspectionProcedure.StationInformation
    ' ''    stationInformation = StationInformationManager.LookupStationInformation(prsName)
    ' ''    Dim newEntity As New Model.ResultView.Entities.ResultsEntityStruct.Value_entity

    ' ''    Dim prsObjects
    ' ''    If objectName = "" And measurePoint = "" Then
    ' ''        prsObjects = From prsObject In stationInformation.PrsObjects Where prsObject.FieldNo = fieldNo
    ' ''    Else
    ' ''        prsObjects = From prsObject In stationInformation.PrsObjects Where prsObject.MeasurePoint = measurePoint And prsObject.ObjectName = objectName
    ' ''    End If

    ' ''    For Each prsObject In prsObjects
    ' ''        newEntity.MeasurePointID = prsObject.MeasurePointId
    ' ''        newEntity.ObjectID = prsObject.ObjectId
    ' ''    Next
    ' ''    Return newEntity
    ' ''End Function

    'Private Function CreateInstanceReportSource(report As IReportDocument, originalReportSource As ReportSource) As ReportSource
    '    Dim instanceReportSource = New InstanceReportSource() With {.ReportDocument = report}
    '    instanceReportSource.Parameters.AddRange(originalReportSource.Parameters)
    '    Return instanceReportSource
    'End Function


    'Public Sub ValidateReportSource(value As String)
    '    If value.Trim().StartsWith("=") Then
    '        Throw New InvalidOperationException("Expressions for ReportSource are not supported when changing the connection string dynamically")
    '    End If
    'End Sub

    'Public Sub SetConnectionString2(reportItemBase As ReportItemBase)

    '    If reportItemBase.Items.Count < 1 Then Return

    '    If TypeOf reportItemBase Is Report Then
    '        Dim report = DirectCast(reportItemBase, Report)
    '        'If TypeOf report.DataSource Is SqlDataSource Then
    '        '    Dim sqlDataSource = DirectCast(report.DataSource, SqlDataSource)
    '        '    sqlDataSource.ConnectionString = connectionString
    '        'End If
    '        'For Each parameter As Telerik.Reporting.ReportParameter In report.ReportParameters
    '        '    If TypeOf parameter.AvailableValues.DataSource Is SqlDataSource Then
    '        '        Dim sqlDataSource = DirectCast(parameter.AvailableValues.DataSource, SqlDataSource)
    '        '        sqlDataSource.ConnectionString = connectionString
    '        '    End If
    '        'Next
    '    End If


    '    For Each item As Telerik.Reporting.ReportItemBase In reportItemBase.Items

    '        'recursively set the connection string to the items from the Items collection

    '        'SetConnectionString(item)
    '        'set the drillthrough report connection strings
    '        'Dim drillThroughAction = TryCast(item.Action, NavigateToReportAction)
    '        'If drillThroughAction IsNot Nothing Then
    '        '    Dim updatedReportInstance = Me.UpdateReportSource(drillThroughAction.ReportSource)
    '        '    drillThroughAction.ReportSource = updatedReportInstance
    '        'End If

    '        If TypeOf item Is DetailSection Then
    '            For Each itemDetail As Telerik.Reporting.ReportItemBase In item.Items
    '                If TypeOf itemDetail Is SubReport Then
    '                    ' Use the InstanceReportSource to pass the report to the viewer for displaying.
    '                    'Attach to report InspectionResults
    '                    Dim report As New subReportInspectionResults

    '                    Dim reportSource As New Telerik.Reporting.InstanceReportSource
    '                    reportSource.ReportDocument = report

    '                    Dim subReport = DirectCast(itemDetail, SubReport)
    '                    subReport.ReportSource = Me.UpdateReportSource(reportSource, subReportType) 'subReport.ReportSource
    '                    Exit For
    '                End If
    '            Next
    '        End If


    '        'Covers all data items(Crosstab, Table, List, Graph, Map and Chart)

    '        'If TypeOf item Is DataItem Then
    '        '    Dim dataItem = DirectCast(item, DataItem)
    '        '    If TypeOf dataItem.DataSource Is SqlDataSource Then
    '        '        Dim sqlDataSource = DirectCast(dataItem.DataSource, SqlDataSource)
    '        '        sqlDataSource.ConnectionString = connectionString
    '        '        Continue For
    '        '    End If
    '        'End If

    '    Next

    'End Sub
#End Region
End Class

