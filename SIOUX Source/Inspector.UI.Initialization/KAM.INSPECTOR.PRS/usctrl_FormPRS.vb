'===============================================================================
'Copyright Wigersma 2012
'All rights reserved.
'===============================================================================
Imports System.Windows.Forms
Imports System.Drawing
Imports Telerik.WinControls.UI
Imports Inspector.BusinessLogic.Data.Configuration.Interfaces.Managers
Imports Inspector.BusinessLogic.Data.Reporting.Interfaces
Imports Inspector.Infra.Ioc
Imports Inspector.Model
Imports KAM.INSPECTOR.PRS.My.Resources

''' <summary>
''' Handling of Grid of PRS data.
''' Displaying the PRS and GCL data into a grid.
''' </summary>
''' <remarks></remarks>
Public Class usctrl_FormPRS
#Region "Class member"
    Public Event evntGridClicked(ByVal prsName As String, ByVal gclName As String, ByVal inspectionProcedureName As String)
    Private executingAssembly As System.Reflection.Assembly
    Private m_StationInformationManager As IStationInformationManager        'Interface with station information
    Private m_InspectionResultReader As IInspectionResultReader        'Interface with station information

    Private grdPRSRow As Integer 'MOD 48

#End Region
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
    ''' Gets or set the Inspection Result Reader
    ''' Used to retrieve the data about the last or previous inspection
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
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


    ''' <summary>
    ''' Update the status of a prs or gcl
    ''' This will only updates the column status image, not the column status
    ''' </summary>
    ''' <param name="prsName"></param>
    ''' <param name="gclName"></param>
    ''' <remarks></remarks>
    Public Sub UpdateStatusInformation(ByVal prsName As String, gclName As String)

        ApplyImageOnStatusChange(prsName, gclName)

        'update the inspection procedure data.
        Dim inspectionProcedureName As String
        If gclName = "" Then gclName = Nothing
        inspectionProcedureName = StationInformationManager.LookupInspectionProcedureName(prsName, gclName)
        RaiseEvent evntGridClicked(prsName, gclName, inspectionProcedureName)

    End Sub
    ''' <summary>
    ''' Enable controls on the form. 
    ''' When selecting the form during initialization, the user can not select items
    ''' </summary>
    ''' <value></value>
    ''' <remarks></remarks>
    Public WriteOnly Property DisableForm As Boolean
        Set(value As Boolean)
            rdGridPRSData.Enabled = value
        End Set
    End Property

#End Region
#Region "Constructor"
    ''' <summary>
    ''' Initiate form 
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()
        ' This call is required by the Windows Form Designer.
        InitializeComponent()
        clsGeneral.loaddata()
        ' Add any initialization after the InitializeComponent() call.
    End Sub

    ''' <summary>
    ''' Loading settings as display form
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub FormPRS_HandleCreated(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.HandleCreated
        'TO DO LoadGridviewSettings()
    End Sub

    ''' <summary>
    ''' Loading the PRS data and set grid properties
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub usctrl_FormPRS_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        FillPrsList()
        setColumnsPRSGrid()
        ApplyImages()
    End Sub
#End Region
#Region "Destructor"
    ''' <summary>
    ''' Saving settings at unloading form
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub FormPRS_HandleDestroyed(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.HandleDestroyed
        SavingGridviewSettings()
    End Sub
#End Region

#Region "Button Handling"

#End Region
#Region "Form Handling"
    ''' <summary>
    ''' Update the information when a item in the grid is selected
    ''' </summary>
    ''' <param name="currentRow"></param>
    ''' <remarks></remarks>
    Private Sub UpdatePanelInfo(ByVal currentRow As GridViewRowInfo)
        If currentRow IsNot Nothing AndAlso Not (TypeOf currentRow Is GridViewNewRowInfo) Then
            If currentRow.HierarchyLevel = 0 Then
                If currentRow.Index = -1 Then Exit Sub 'MOD XXX
                rtbGasstation.Text = currentRow.Cells(PRSDataFields.FldPRSName).Value
                rtbInformation.Text = currentRow.Cells(PRSDataFields.FldPRSInformation).Value
                rtbInspectionProcedure.Text = currentRow.Cells(PRSDataFields.FldPRSInspectionProcedure).Value
                rtbPRSCode.Text = currentRow.Cells(PRSDataFields.FldPRSCode).Value
                rtbPRSIdentification.Text = currentRow.Cells(PRSDataFields.FldPRSIdentification).Value

                rtbPRSStatus.Text = currentRow.Cells(PRSDataFields.FldPRSStatus).Value

                RaiseEvent evntGridClicked(currentRow.Cells(PRSDataFields.FldPRSName).Value, "", currentRow.Cells(PRSDataFields.FldPRSInspectionProcedure).Value)

                rtbGCLName.Text = ""
                'MOD 63
                rtbGCLIdentification.Text = ""
                rtbGCLCode1.Text = ""
                rtbGCLInspectionprocedure1.Text = ""
                rtbGCLPaRangeDM.Text = ""
                rtbGCLPeRangeDM.Text = ""
                rtbGCLPeMax.Text = ""
                rtbGCLPeMin.Text = ""
                rtbGCLVolumeVAK.Text = ""
                rtbGCLVolumeVA.Text = ""
                'MOD 73
                rdGridBoundaries.Columns.Clear()

            End If
            If currentRow.HierarchyLevel = 1 Then

                'Setting the parent information of the selected row.
                Dim CurrentRowParrent As GridViewRowInfo
                Dim gasControlLineInformation As InspectionProcedure.GasControlLineInformation

                CurrentRowParrent = currentRow.Parent
                UpdatePanelInfo(CurrentRowParrent)
                RaiseEvent evntGridClicked(CurrentRowParrent.Cells(PRSDataFields.FldPRSName).Value, currentRow.Cells(PRSDataFields.FldGCLName).Value, currentRow.Cells(PRSDataFields.FldGCLInspectionProcedure).Value)

                'Set the gas control line data to a set
                gasControlLineInformation = StationInformationManager.LookupGasControlLineInformation(CurrentRowParrent.Cells(PRSDataFields.FldPRSName).Value.ToString, currentRow.Cells(PRSDataFields.FldGCLName).Value.ToString)
                'Displaying boundaries of selected gas control line in grid
                'MOD 37
                If gasControlLineInformation.GclObjects IsNot Nothing Then
                    Me.rdGridBoundaries.Visible = True

                    With Me.rdGridBoundaries
                        .BeginUpdate()
                        .Columns.Clear()
                        .MasterTemplate.AllowAddNewRow = False
                        .MasterTemplate.AutoGenerateColumns = True
                        .MasterTemplate.Reset()
                        .TableElement.RowHeight = 30
                        .AutoGenerateHierarchy = True
                        .UseScrollbarsInHierarchy = False
                        .MultiSelect = False
                        .MasterTemplate.BestFitColumns()
                        .MasterTemplate.AutoSizeColumnsMode = GridViewAutoSizeColumnsMode.Fill
                        .ReadOnly = True
                        .EnableFiltering = False
                        .EnableGrouping = False
                        .EnableAlternatingRowColor = True

                        .EndUpdate()

                        Try
                            If gasControlLineInformation.GclObjects IsNot Nothing Then
                                '.DataMember = "GclObjects"
                                .DataSource = gasControlLineInformation.GclObjects
                                .Columns.Add("ValueMin", INSPECTORPRSResx.str_ValueMin)
                                .Columns.Add("ValueMax", INSPECTORPRSResx.str_ValueMax)
                                .Columns.Add("UOV", INSPECTORPRSResx.str_UOV)
                                'MDO 29
                                .Columns("ValueMin").FieldName = "GclBoundary.ValueMin" 'DO NOT TRANSLATE
                                .Columns("ValueMax").FieldName = "GclBoundary.ValueMax" 'DO NOT TRANSLATE
                                .Columns("UOV").FieldName = "GclBoundary.UOV" 'DO NOT TRANSLATE

                                .Columns("ObjectName").HeaderText = INSPECTORPRSResx.str_ObjectName
                                .Columns("ObjectID").HeaderText = INSPECTORPRSResx.str_ObjectNameID
                                .Columns("MeasurePoint").HeaderText = INSPECTORPRSResx.str_MeasurePoint
                                .Columns("MeasurePointID").HeaderText = INSPECTORPRSResx.str_MeasurePointID
                                .Columns("FieldNo").HeaderText = INSPECTORPRSResx.str_FieldNo
                                .Columns("GclBoundary").IsVisible = False

                                Dim lbMeasurePoint As Boolean = False
                                Dim lbMeasurePointDescr As Boolean = False
                                Dim lbMeasurePointID As Boolean = False
                                Dim lbObjectName As Boolean = False
                                Dim lbObjectID As Boolean = False
                                Dim lbObjectNameDescr As Boolean = False
                                Dim lbFieldNo As Boolean = False


                                'Do not show the columns which contains no data
                                For i As Integer = 0 To gasControlLineInformation.GclObjects.Count - 1
                                    If gasControlLineInformation.GclObjects(i).MeasurePoint <> "" Then lbMeasurePoint = True
                                    If gasControlLineInformation.GclObjects(i).MeasurePointId <> "" Then lbMeasurePointID = True
                                    If gasControlLineInformation.GclObjects(i).MeasurePointDescription <> "" Then lbMeasurePointDescr = True 'MOD 66

                                    If gasControlLineInformation.GclObjects(i).ObjectName <> "" Then lbObjectName = True
                                    If gasControlLineInformation.GclObjects(i).ObjectId <> "" Then lbObjectID = True
                                    If gasControlLineInformation.GclObjects(i).ObjectNameDescription <> "" Then lbObjectNameDescr = True 'MOD 66
                                    If gasControlLineInformation.GclObjects(i).FieldNo <> 0 Then lbFieldNo = True

                                    If lbMeasurePoint = True And lbMeasurePointID = True And lbObjectName = True And lbObjectID = True And lbFieldNo = True Then
                                        Exit For
                                    End If
                                Next

                                If lbMeasurePoint = False Then .Columns("MeasurePoint").IsVisible = False
                                If lbMeasurePointID = False Then .Columns("MeasurePointID").IsVisible = False
                                If lbMeasurePointDescr = False Then .Columns("MeasurePointDescription").IsVisible = False 'MOD 66

                                If lbObjectName = False Then .Columns("ObjectName").IsVisible = False
                                If lbObjectID = False Then .Columns("ObjectID").IsVisible = False
                                If lbObjectNameDescr = False Then .Columns("ObjectNameDescription").IsVisible = False 'MOD 66
                                If lbFieldNo = False Then .Columns("FieldNo").IsVisible = False

                            End If
                        Catch ex As Exception
                            MsgBox(ex.Message)
                        End Try

                    End With
                    'NOT USED
                    'Chart1.Series.Clear()
                    ''Chart1.Series.Add("ValueMax")
                    ''Chart1.Series.Add("ValueMin")
                    ''Chart1.Series("ValueMax").ChartType = SeriesChartType.Area
                    ''Chart1.Series("ValueMin").ChartType = SeriesChartType.Area
                    'Dim firstview As New DataView(BoundariesDataSet.Tables(0))
                    'Chart1.Series("ValueMax").Points.DataBindXY(firstview, "ID", firstview, "ValueMax")
                    'Chart1.Series("ValueMin").Points.DataBindXY(firstview, "ID", firstview, "ValueMin")
                    'Chart1.Update()
                    'END NOT USED
                Else
                    Me.rdGridBoundaries.Visible = False
                End If


                'Display the information of the selected gas control ine
                rtbGCLName.Text = gasControlLineInformation.GasControlLineName
                rtbGCLCode1.Text = gasControlLineInformation.GCLCode
                'MOD 63
                rtbGCLIdentification.Text = gasControlLineInformation.GCLIdentification

                rtbGCLInspectionprocedure1.Text = gasControlLineInformation.InspectionProcedure
                rtbGCLPaRangeDM.Text = gasControlLineInformation.PaRangeDM
                rtbGCLPeRangeDM.Text = gasControlLineInformation.PeRangeDM

                'MOD 04
                'to DO
                Dim searchText As String = "bar"
                rtbGCLPeMax.Text = SplitString(gasControlLineInformation.PeMax, searchText)
                rtbGCLPeMin.Text = SplitString(gasControlLineInformation.PeMin, searchText)

                'rtbGCLPeMax.Text = gasControlLineInformation.PeMax
                'rtbGCLPeMin.Text = gasControlLineInformation.PeMin
                'MOD 04
                'to do
                searchText = "dm3"
                rtbGCLVolumeVAK.Text = SplitString(gasControlLineInformation.VolumeVA, searchText)
                rtbGCLVolumeVA.Text = SplitString(gasControlLineInformation.VolumeVAK, searchText)
            End If
        Else
        End If
    End Sub

#End Region

    'MOD 04
    ''' <summary>
    ''' Insert a space in a text
    ''' </summary>
    ''' <param name="splitText">Text to insert</param>
    ''' <param name="searchText">Text to put space before</param>
    ''' <returns>String with space</returns>
    ''' <remarks></remarks>
    Private Function SplitString(splitText As String, ByVal searchText As String) As String
        Dim lipos As Integer

        lipos = InStr(splitText, searchText)
        If lipos > 0 Then
            Dim textReturn As String
            textReturn = Trim(splitText.Substring(0, lipos - 1))
            textReturn += " " & Trim(splitText.Substring(lipos - 1, searchText.Length))
            Return textReturn
        Else
            Return ""
        End If
    End Function
#Region "Gridview handling"

    ''' <summary>
    ''' Handling of double clicking the grid
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub rdGridPRSData_DoubleClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles rdGridPRSData.CellClick, rdGridPRSData.CellDoubleClick
        'Expand when record is double clicked in the main row (Hierarchylevel = 0)
        If Me.rdGridPRSData.CurrentRow.HierarchyLevel = 0 Then
            'MOD 48
            'MOD 51
            If Me.rdGridPRSData.CurrentRow.Index = -1 Then
                'MOD 91
                If Me.rdGridPRSData.ChildRows.Count <> 0 Then
                    Me.rdGridPRSData.ChildRows.Item(grdPRSRow).IsExpanded = False
                    grdPRSRow = 0
                End If
                Exit Sub

            End If

            If Me.rdGridPRSData.CurrentRow.IsExpanded = True Then
                Me.rdGridPRSData.CurrentRow.IsExpanded = False
            Else
                'MOD 45
                'MOD 48 Me.rdGridPRSData.MasterTemplate.CollapseAll()
                'MOD 51
                'Me.rdGridPRSData.GridNavigator.SelectPreviousRow(1, False, False)
                If Me.rdGridPRSData.CurrentRow.Index <> grdPRSRow And Me.rdGridPRSData.CurrentRow.Index >= 0 Then
                    Me.rdGridPRSData.ChildRows.Item(grdPRSRow).IsExpanded = False
                    grdPRSRow = Me.rdGridPRSData.CurrentRow.Index

                End If
                Me.rdGridPRSData.CurrentRow.IsExpanded = True
            End If


        End If
    End Sub
    ''' <summary>
    ''' Update the information in the GUI when a row is selected in the grid
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub rdGridPRSData_CurrentRowChanged(ByVal sender As Object, ByVal e As Telerik.WinControls.UI.CurrentRowChangedEventArgs) Handles rdGridPRSData.CurrentRowChanged
        UpdatePanelInfo(Me.rdGridPRSData.CurrentRow)
    End Sub
    ''' <summary>
    ''' Fill the grid with PRS and GCL data
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub FillPrsList()
        Try
            'Not uesed StationInformationManager.Refresh()
            'Not used; Problem with telerik control Me.rdGridPRSData.DataMember = "StationsInformation"
            Me.rdGridPRSData.DataSource = StationInformationManager.StationsInformation
            Me.rdGridPRSData.Refresh()
            Me.rdGridPRSData.MasterTemplate.ShowChildViewCaptions = False
            Me.rdGridPRSData.MasterTemplate.AutoSizeColumnsMode = GridViewAutoSizeColumnsMode.Fill
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
        GridViewLayout()
    End Sub
    ''' <summary>
    ''' Define the layour of the grid
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub GridViewLayout()
        With Me.rdGridPRSData

            .BeginUpdate()
            .EnableSorting = True
            .EnableGrouping = False
            .EnableFiltering = True
            .AllowColumnChooser = True
            .EndUpdate()
            .AllowAddNewRow = False
            .AllowRowReorder = False
            .ReadOnly = True
            .MultiSelect = False

            .AllowColumnHeaderContextMenu = True
            .AllowCellContextMenu = False

            .AutoGenerateHierarchy = True
            .UseScrollbarsInHierarchy = True

            .ShowItemToolTips = True

            .BestFitColumns()

            .EnableAlternatingRowColor = True
            ',.AutoSizeRows = True
            .TableElement.RowHeight = 40

            .TableElement.ShowSelfReferenceLines = True
        End With
    End Sub

    ''' <summary>
    ''' Create a grid for displaying the PRS and GCL data
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub setColumnsPRSGrid()
        Try
            With Me.rdGridPRSData.MasterTemplate
                'PRS Fields
                ' .ChildGridViewTemplates.Remove(.ChildGridViewTemplates(1))
                .Columns(PRSDataFields.FldPRSIdentification).IsVisible = True
                '.Columns(PRSDataFields.FldPRSIdentification).VisibleInColumnChooser = True
                .Columns(PRSDataFields.FldPRSIdentification).HeaderText = INSPECTORPRSResx.str_FldPRSIdentification
                rlblPRSIden.Text = INSPECTORPRSResx.str_FldPRSIdentification

                .Columns(PRSDataFields.FldPRSName).IsVisible = True
                .Columns(PRSDataFields.FldPRSName).VisibleInColumnChooser = False
                .Columns(PRSDataFields.FldPRSName).AllowHide = False
                .Columns(PRSDataFields.FldPRSName).HeaderText = INSPECTORPRSResx.str_FldPRSName
                rlblPrsName.Text = INSPECTORPRSResx.str_FldPRSName

                .Columns(PRSDataFields.FldPRSRoute).IsVisible = False
                .Columns(PRSDataFields.FldPRSRoute).VisibleInColumnChooser = True
                .Columns(PRSDataFields.FldPRSRoute).HeaderText = INSPECTORPRSResx.str_FldPRSRoute
                'rlblPrsName.Text = INSPECTORPRSResx.str_FldPRSName

                .Columns(PRSDataFields.FldPRSGasControlLines).IsVisible = False
                .Columns(PRSDataFields.FldPRSGasControlLines).VisibleInColumnChooser = False
                .Columns(PRSDataFields.FldPRSGasControlLines).HeaderText = INSPECTORPRSResx.str_FldPRSGasControlLines

                .Columns(PRSDataFields.FldPRSObject).IsVisible = False
                .Columns(PRSDataFields.FldPRSObject).VisibleInColumnChooser = False
                '.Columns(PRSDataFields.FldPRSObject).HeaderText = INSPECTORPRSResx.str_fldPRSObject

                .Columns(PRSDataFields.FldPRSInspectionProcedure).IsVisible = True
                .Columns(PRSDataFields.FldPRSInspectionProcedure).VisibleInColumnChooser = True
                .Columns(PRSDataFields.FldPRSInspectionProcedure).HeaderText = INSPECTORPRSResx.str_FldPRSInspectionProcedure
                rlblPRSIP.Text = INSPECTORPRSResx.str_FldPRSInspectionProcedure

                .Columns(PRSDataFields.FldPRSInformation).IsVisible = False
                .Columns(PRSDataFields.FldPRSInformation).VisibleInColumnChooser = True
                .Columns(PRSDataFields.FldPRSInformation).HeaderText = INSPECTORPRSResx.str_FldPRSInformation
                rlblPRSInformation.Text = INSPECTORPRSResx.str_FldPRSInformation

                .Columns(PRSDataFields.FldPRSCode).IsVisible = False
                .Columns(PRSDataFields.FldPRSCode).VisibleInColumnChooser = True
                .Columns(PRSDataFields.FldPRSCode).HeaderText = INSPECTORPRSResx.str_FldPRSCode
                rlblPRSCode.Text = INSPECTORPRSResx.str_FldPRSCode

                .Columns(PRSDataFields.FldPRSStatus).IsVisible = False
                .Columns(PRSDataFields.FldPRSStatus).VisibleInColumnChooser = False
                .Columns(PRSDataFields.FldPRSStatus).AllowHide = False
                .Columns(PRSDataFields.FldPRSStatus).HeaderText = INSPECTORPRSResx.str_Status
                rlblPRSStatus.Text = INSPECTORPRSResx.str_Status

                .Columns.Add(New GridViewImageColumn(PRSDataFields.FldPRSStatusImage, ""))
                .Columns(PRSDataFields.FldPRSStatusImage).ImageLayout = Windows.Forms.ImageLayout.Zoom
                .Columns(PRSDataFields.FldPRSStatusImage).MaxWidth = 30
                .Columns(PRSDataFields.FldPRSStatusImage).MinWidth = 30

                .Columns(PRSDataFields.FldPRSGCLOverallStatus).IsVisible = False
                .Columns(PRSDataFields.FldPRSGCLOverallStatus).VisibleInColumnChooser = False
                .Columns(PRSDataFields.FldPRSGCLOverallStatus).AllowHide = False
                .Columns(PRSDataFields.FldPRSGCLOverallStatus).HeaderText = INSPECTORPRSResx.str_Status
                rlblPRSStatus.Text = INSPECTORPRSResx.str_Status

                .Columns.Add(New GridViewImageColumn(PRSDataFields.FldPRSGCLOverallStatusImage, ""))
                .Columns(PRSDataFields.FldPRSGCLOverallStatusImage).ImageLayout = Windows.Forms.ImageLayout.Zoom
                .Columns(PRSDataFields.FldPRSGCLOverallStatusImage).MaxWidth = 30
                .Columns(PRSDataFields.FldPRSGCLOverallStatusImage).MinWidth = 30

                .BestFitColumns()

                'GCL fields
                .Templates(0).Columns(PRSDataFields.FldGCLName).IsVisible = True
                .Templates(0).Columns(PRSDataFields.FldGCLName).VisibleInColumnChooser = True
                .Templates(0).Columns(PRSDataFields.FldGCLName).AllowHide = False
                .Templates(0).Columns(PRSDataFields.FldGCLName).HeaderText = INSPECTORPRSResx.str_FldGCLName
                rlblGCLName.Text = INSPECTORPRSResx.str_FldGCLName

                .Templates(0).Columns(PRSDataFields.FldGCLPeMin).IsVisible = False
                .Templates(0).Columns(PRSDataFields.FldGCLPeMin).VisibleInColumnChooser = True
                .Templates(0).Columns(PRSDataFields.FldGCLPeMin).HeaderText = INSPECTORPRSResx.str_FldGCLPeMin
                rlblGCLPemin.Text = INSPECTORPRSResx.str_FldGCLPeMin

                .Templates(0).Columns(PRSDataFields.FldGCLPeMax).IsVisible = False
                .Templates(0).Columns(PRSDataFields.FldGCLPeMax).VisibleInColumnChooser = True
                .Templates(0).Columns(PRSDataFields.FldGCLPeMax).HeaderText = INSPECTORPRSResx.str_FldGCLPeMax
                rlblGCLPeMax.Text = INSPECTORPRSResx.str_FldGCLPeMax

                .Templates(0).Columns(PRSDataFields.FldGCLVolumeVAK).IsVisible = False
                .Templates(0).Columns(PRSDataFields.FldGCLVolumeVAK).VisibleInColumnChooser = True
                .Templates(0).Columns(PRSDataFields.FldGCLVolumeVAK).HeaderText = INSPECTORPRSResx.str_FldGCLVolumeVAK
                rlblGCLVolumeVAK.Text = INSPECTORPRSResx.str_FldGCLVolumeVAK

                .Templates(0).Columns(PRSDataFields.FldGCLVolumeVA).IsVisible = False
                .Templates(0).Columns(PRSDataFields.FldGCLVolumeVA).VisibleInColumnChooser = True
                .Templates(0).Columns(PRSDataFields.FldGCLVolumeVA).HeaderText = INSPECTORPRSResx.str_FldGCLVolumeVA
                rlblGCLVolumeVA.Text = INSPECTORPRSResx.str_FldGCLVolumeVA

                .Templates(0).Columns(PRSDataFields.FldGCLPaRangeDM).IsVisible = True
                .Templates(0).Columns(PRSDataFields.FldGCLPaRangeDM).VisibleInColumnChooser = True
                .Templates(0).Columns(PRSDataFields.FldGCLPaRangeDM).HeaderText = INSPECTORPRSResx.str_FldGCLPaRangeDM
                rlblGCLPaRange.Text = INSPECTORPRSResx.str_FldGCLPaRangeDM

                .Templates(0).Columns(PRSDataFields.FldGCLPeRangeDM).IsVisible = True
                .Templates(0).Columns(PRSDataFields.FldGCLPeRangeDM).VisibleInColumnChooser = True
                .Templates(0).Columns(PRSDataFields.FldGCLPeRangeDM).HeaderText = INSPECTORPRSResx.str_FldGCLPeRangeDM
                rlblGCLPeRange.Text = INSPECTORPRSResx.str_FldGCLPeRangeDM

                .Templates(0).Columns(PRSDataFields.FldGCLIdentification).IsVisible = False
                .Templates(0).Columns(PRSDataFields.FldGCLIdentification).VisibleInColumnChooser = True
                .Templates(0).Columns(PRSDataFields.FldGCLIdentification).HeaderText = INSPECTORPRSResx.str_FldGCLIdentification
                'rlblgcli.Text = INSPECTORPRSResx.str_FldGCLIdentification

                .Templates(0).Columns(PRSDataFields.FldGCLInspectionProcedure).IsVisible = True
                .Templates(0).Columns(PRSDataFields.FldGCLInspectionProcedure).VisibleInColumnChooser = True
                .Templates(0).Columns(PRSDataFields.FldGCLInspectionProcedure).HeaderText = INSPECTORPRSResx.str_FldGCLInspectionProcedure
                rlblGCLIp.Text = INSPECTORPRSResx.str_FldGCLInspectionProcedure

                .Templates(0).Columns(PRSDataFields.FldGCLCode).IsVisible = True
                .Templates(0).Columns(PRSDataFields.FldGCLCode).VisibleInColumnChooser = True
                .Templates(0).Columns(PRSDataFields.FldGCLCode).HeaderText = INSPECTORPRSResx.str_FldGCLCode
                rlblGCLCode.Text = INSPECTORPRSResx.str_FldGCLCode

                .Templates(0).Columns(PRSDataFields.FldGCLFSDStart).IsVisible = False
                .Templates(0).Columns(PRSDataFields.FldGCLFSDStart).VisibleInColumnChooser = True
                .Templates(0).Columns(PRSDataFields.FldGCLFSDStart).HeaderText = INSPECTORPRSResx.str_FldGCLFSDStart
                'rlblGCLCode.Text = INSPECTORPRSResx.str_FldGCLFSDStart

                .Templates(0).Columns(PRSDataFields.FldGCLObject).IsVisible = False
                .Templates(0).Columns(PRSDataFields.FldGCLObject).VisibleInColumnChooser = False
                .Templates(0).Columns(PRSDataFields.FldGCLObject).HeaderText = INSPECTORPRSResx.str_FldGCLObjectect

                .Templates(0).Columns(PRSDataFields.FldGCLStatus).IsVisible = False
                .Templates(0).Columns(PRSDataFields.FldGCLStatus).VisibleInColumnChooser = False
                .Templates(0).Columns(PRSDataFields.FldGCLStatus).AllowHide = False
                .Templates(0).Columns(PRSDataFields.FldGCLStatus).HeaderText = INSPECTORPRSResx.str_Status

                .Templates(0).Columns.Add(New GridViewImageColumn(PRSDataFields.FldGCLStatusImage, ""))
                .Templates(0).Columns(PRSDataFields.FldGCLStatusImage).ImageLayout = Windows.Forms.ImageLayout.Zoom
                .Templates(0).Columns(PRSDataFields.FldGCLStatusImage).MaxWidth = 30
                .Templates(0).Columns(PRSDataFields.FldGCLStatusImage).MinWidth = 30
                .Templates(0).AutoSizeColumnsMode = GridViewAutoSizeColumnsMode.Fill

                .BestFitColumns()

                .Templates(0).Templates.Clear()
                .Templates(0).EnableFiltering = False

                'Delete child PRS data
                .Templates.RemoveAt(1)
                Me.rdGridPRSData.EndUpdate()
            End With
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub
    ''' <summary>
    ''' Showing information; tooltip; The inspection status is displayed
    ''' </summary>
    ''' <param name="e"></param>
    ''' <param name="status"></param>
    ''' <param name="OverallStatus"></param>
    ''' <remarks></remarks>
    Private Sub ShowTooltip(e As Telerik.WinControls.ToolTipTextNeededEventArgs, ByVal status As InspectionProcedureStatus.InspectionStatusInformation, ByVal OverallStatus As Boolean)
        If OverallStatus = False Then
            Select Case status.Status
                Case InspectionProcedure.InspectionStatus.Completed
                    e.ToolTipText = My.Resources.INSPECTORPRSResx.str_InspectionCompleted
                Case (InspectionProcedure.InspectionStatus.CompletedValueOutOfLimits)
                    e.ToolTipText = My.Resources.INSPECTORPRSResx.str_InspectionOutOfLimits
                Case InspectionProcedure.InspectionStatus.GclOrPrsDeletedByUser
                    e.ToolTipText = My.Resources.INSPECTORPRSResx.str_InspectionDeleted
                Case InspectionProcedure.InspectionStatus.NoInspection
                    e.ToolTipText = My.Resources.INSPECTORPRSResx.str_InspectionNotInspected
                Case InspectionProcedure.InspectionStatus.StartNotCompleted
                    e.ToolTipText = My.Resources.INSPECTORPRSResx.str_InspectionStartedNotCompleted
                Case InspectionProcedure.InspectionStatus.NoInspectionFound
                    e.ToolTipText = My.Resources.INSPECTORPRSResx.str_InspectionNoProcedure
                Case InspectionProcedure.InspectionStatus.Warning
                    'TO DO 
                Case Else
                    e.ToolTipText = My.Resources.INSPECTORPRSResx.str_InspectionNoProcedure
            End Select

        Else
            Select Case status.OverallGasControlLineStatus
                Case InspectionProcedure.InspectionStatus.Completed
                    e.ToolTipText = My.Resources.INSPECTORPRSResx.str_PRSGCLInspectionCompleted
                Case (InspectionProcedure.InspectionStatus.CompletedValueOutOfLimits)
                    e.ToolTipText = My.Resources.INSPECTORPRSResx.str_PRSGCLInspectionOutOfLimits
                Case InspectionProcedure.InspectionStatus.GclOrPrsDeletedByUser
                    e.ToolTipText = My.Resources.INSPECTORPRSResx.str_PRSGCLInspectionDeleted
                Case InspectionProcedure.InspectionStatus.NoInspection
                    e.ToolTipText = My.Resources.INSPECTORPRSResx.str_PRSGCLInspectionNotInspected
                Case InspectionProcedure.InspectionStatus.StartNotCompleted
                    e.ToolTipText = My.Resources.INSPECTORPRSResx.str_PRSGCLInspectionStartedNotCompleted
                Case InspectionProcedure.InspectionStatus.NoInspectionFound
                    e.ToolTipText = My.Resources.INSPECTORPRSResx.str_PRSGCLInspectionNoProcedure
                Case InspectionProcedure.InspectionStatus.Warning
                    e.ToolTipText = My.Resources.INSPECTORPRSResx.str_PRSGCLInspectionWarning
                Case Else
                    e.ToolTipText = My.Resources.INSPECTORPRSResx.str_PRSGCLInspectionNoProcedure
            End Select
        End If
    End Sub

    ''' <summary>
    ''' Saving the grid settings to a file
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub SavingGridviewSettings()
        'NOT USED
        '************ NEED TO CHANGE. Set to class
        'Me.rdGridPRSData.SaveLayout(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) & "\WS-gas\" & Application.ProductName & "\" & Me.Name & "Gridview.xml")
        'END NOT USED

    End Sub
    ''' <summary>
    ''' Loading the grid settings
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub LoadGridviewSettings()
        'NOT USED
        'Me.rdGridPRSData.LoadLayout(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) & "\WS-gas\" & Application.ProductName & "\" & Me.Name & "Gridview.xml")
        'END NOT USED
    End Sub
#End Region

#Region "Row status; PRS and GCL"

    ''' <summary>
    ''' Apply the images depending on the status
    ''' For PRS and gcl status
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub ApplyImages()
        For i As Integer = 0 To rdGridPRSData.RowCount - 1
            GetAndSetRowStatus(rdGridPRSData.Rows(i), PRSDataFields.FldPRSStatus, PRSDataFields.FldPRSStatusImage)
            GetAndSetRowStatus(rdGridPRSData.Rows(i), PRSDataFields.FldPRSGCLOverallStatus, PRSDataFields.FldPRSGCLOverallStatusImage)

            'Apply the status for the GCL
            For j As Integer = 0 To rdGridPRSData.Rows(i).ChildRows.Count - 1
                GetAndSetRowStatus(rdGridPRSData.Rows(i).ChildRows(j), PRSDataFields.FldGCLStatus, PRSDataFields.FldGCLStatusImage)
            Next
        Next
    End Sub
    ''' <summary>
    ''' Apply the image on the record from which the status which is changed
    ''' </summary>
    ''' <param name="prsName"></param>
    ''' <param name="gclName"></param>
    ''' <remarks></remarks>
    Private Sub ApplyImageOnStatusChange(ByVal prsName As String, ByVal gclName As String)
        For i As Integer = 0 To rdGridPRSData.RowCount - 1
            'Looking for PRS name
            If rdGridPRSData.Rows(i).Cells(PRSDataFields.FldPRSName).Value = prsName Then
                Dim prsStatus As InspectionProcedureStatus.InspectionStatusInformation
                prsStatus = StationInformationManager.GetInspectionStatus(prsName)

                'In case of only GCL
                If gclName IsNot Nothing Then
                    If gclName <> "" Then
                        'Update the overal status of the PRS
                        SetStatusImage(rdGridPRSData.Rows(i).Cells(PRSDataFields.FldPRSGCLOverallStatusImage), prsStatus.OverallGasControlLineStatus)
                        'Looking for GCL Name
                        For j As Integer = 0 To rdGridPRSData.Rows(i).ChildRows.Count - 1
                            If rdGridPRSData.Rows(i).ChildRows(j).Cells(PRSDataFields.FldGCLName).Value = gclName Then
                                Dim gclStatus As InspectionProcedureStatus.InspectionStatusInformation
                                gclStatus = StationInformationManager.GetInspectionStatus(prsName, gclName)
                                SetStatusImage(rdGridPRSData.Rows(i).ChildRows(j).Cells(PRSDataFields.FldGCLStatusImage), gclStatus.Status)

                                Exit For
                            End If
                        Next
                    Else
                        'Case of only PRS
                        SetStatusImage(rdGridPRSData.Rows(i).Cells(PRSDataFields.FldPRSStatusImage), prsStatus.Status)
                        Exit For
                    End If
                Else
                    'Case of only PRS
                    SetStatusImage(rdGridPRSData.Rows(i).Cells(PRSDataFields.FldPRSStatusImage), prsStatus.Status)
                    Exit For
                End If
            End If
        Next
    End Sub

    ''' <summary>
    ''' Get the inspection status of the selected row
    ''' </summary>
    ''' <param name="row"></param>
    ''' <param name="rowName">Row name</param>
    ''' <param name="columnStatusImage">Column with status image</param>
    ''' <remarks></remarks>
    Private Sub GetAndSetRowStatus(ByVal row As Telerik.WinControls.UI.GridViewRowInfo, ByVal rowName As String, ByVal columnStatusImage As String)
        If row.Cells(rowName).Value IsNot Nothing Then
            SetStatusImage(row.Cells(columnStatusImage), row.Cells(rowName).Value)
        Else
        End If
    End Sub

    ''' <summary>
    ''' Apply the image on the selected cell
    ''' </summary>
    ''' <param name="rowCel">Cell of row</param>
    ''' <param name="status">The status</param>
    ''' <remarks></remarks>
    Private Sub SetStatusImage(ByVal rowCel As Telerik.WinControls.UI.GridViewCellInfo, ByVal status As InspectionProcedure.InspectionStatus)

        Select Case status
            Case InspectionProcedure.InspectionStatus.Completed
                rowCel.Value = My.Resources.StatusImageResx.Green_Check
            Case InspectionProcedure.InspectionStatus.CompletedValueOutOfLimits
                rowCel.Value = My.Resources.StatusImageResx.Warning
            Case InspectionProcedure.InspectionStatus.GclOrPrsDeletedByUser
                rowCel.Value = My.Resources.StatusImageResx.Delete
            Case InspectionProcedure.InspectionStatus.NoInspection
                rowCel.Value = My.Resources.StatusImageResx.Play
            Case InspectionProcedure.InspectionStatus.StartNotCompleted
                rowCel.Value = My.Resources.StatusImageResx.pause_orange
                'TO DO add option for no inspection; Cancel icon
                'TO DO add option for gcl at PRS
            Case InspectionProcedure.InspectionStatus.NoInspectionFound
                rowCel.Value = My.Resources.StatusImageResx.Green_Check
            Case InspectionProcedure.InspectionStatus.Warning
                rowCel.Value = My.Resources.StatusImageResx.Warning
            Case Else
                rowCel.Value = My.Resources.StatusImageResx.Green_Check
        End Select

    End Sub
    ''' <summary>
    ''' Display a tooltip depending on the applied status image
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub rdGridPRSData_ToolTipTextNeeded(sender As Object, e As Telerik.WinControls.ToolTipTextNeededEventArgs) Handles rdGridPRSData.ToolTipTextNeeded
        Dim lbOverallStatus As Boolean = False
        Dim dataCell As GridDataCellElement = TryCast(sender, GridDataCellElement)
        If dataCell IsNot Nothing Then

            Dim lprsName As String = ""
            Dim lgclName As String = ""

            If dataCell.ColumnInfo.Name = PRSDataFields.FldPRSStatusImage Then
                lprsName = dataCell.RowInfo.Cells(PRSDataFields.FldPRSName).Value
                lbOverallStatus = False
            ElseIf dataCell.ColumnInfo.Name = PRSDataFields.FldPRSGCLOverallStatusImage Then
                lprsName = dataCell.RowInfo.Cells(PRSDataFields.FldPRSName).Value
                lbOverallStatus = True
            ElseIf dataCell.ColumnInfo.Name = PRSDataFields.FldGCLStatusImage Then
                Dim currentRowParrent As GridViewRowInfo

                currentRowParrent = dataCell.RowInfo.Parent

                lprsName = currentRowParrent.Cells(PRSDataFields.FldPRSName).Value
                lgclName = dataCell.RowInfo.Cells(PRSDataFields.FldGCLName).Value
                lbOverallStatus = False
            Else : Exit Sub
            End If

            If lprsName Is Nothing Then Exit Sub

            'getting the status
            Dim status As InspectionProcedureStatus.InspectionStatusInformation
            status = StationInformationManager.GetInspectionStatus(lprsName, lgclName)

            ShowTooltip(e, status, lbOverallStatus)
        End If
    End Sub
#End Region


    ''' <summary>
    ''' The different fields in the XML file of plexor. Used in the grid of displaying the data
    ''' </summary>
    ''' <remarks></remarks>
    Public Class PRSDataFields
        Public Const FldPRSIdentification As String = "PRSIdentification"
        Public Const FldPRSName As String = "PRSName"
        Public Const FldPRSRoute As String = "Route"
        Public Const FldPRSGasControlLines As String = "GasControlLines"
        Public Const FldPRSInformation As String = "Information"
        Public Const FldPRSInspectionProcedure As String = "InspectionProcedure"
        Public Const FldPRSCode As String = "PRSCode"
        Public Const FldPRSObject As String = "PrsObjects"
        Public Const FldPRSStatus As String = "InspectionStatus"
        Public Const FldPRSStatusImage As String = "PRSInspectionStatusImage"
        Public Const FldPRSGCLOverallStatus As String = "OverAllGasControlLineStatus"
        Public Const FldPRSGCLOverallStatusImage As String = "PRSGCLInspectionStatusImage"

        Public Const FldGCLName As String = "GasControlLineName"
        Public Const FldGCLPeMin As String = "PeMin"
        Public Const FldGCLPeMax As String = "PeMax"
        Public Const FldGCLVolumeVAK As String = "VolumeVAK"
        Public Const FldGCLVolumeVA As String = "VolumeVA"
        Public Const FldGCLPaRangeDM As String = "PaRangeDM"
        Public Const FldGCLPeRangeDM As String = "PeRangeDM"
        Public Const FldGCLIdentification As String = "GCLIdentification"
        Public Const FldGCLInspectionProcedure As String = "InspectionProcedure"
        Public Const FldGCLCode As String = "GCLCode"
        Public Const FldGCLFSDStart As String = "FSDStart"
        Public Const FldGCLObject As String = "GclObjects"
        Public Const FldGCLStatus As String = "InspectionStatus"
        Public Const FldGCLStatusImage As String = "GCLInspectionStatusImage"
    End Class



#Region "Not used"
    'used to check if the boundaries of an individual PRS/gcl can be loaded
    Private Sub RadButton1_Click(sender As System.Object, e As System.EventArgs)
        Dim test As InspectionProcedure.GclObject
        test = StationInformationManager.LookupGasControlLineObject("APELDOORN ORDERMOLENWEG 1 | 5 007 230.1", "5 007 230.1.1 OS LS", "", "", 20)

    End Sub


    'This is not used in the program
    Private Sub SetConditions()
        'add a couple of sample formatting objects
        Dim c1 As New ConditionalFormattingObject("Orange, applied to entire row", ConditionTypes.Contains, "ENSO", "", True)
        c1.RowBackColor = Color.FromArgb(255, 209, 140)
        c1.CellBackColor = Color.FromArgb(255, 209, 140)
        rdGridPRSData.Columns("PRSName").ConditionalFormattingObjectList.Add(c1)

        Dim c2 As New ConditionalFormattingObject("Green, applied to cells only", ConditionTypes.Contains, "64", "", False)
        c2.RowBackColor = Color.FromArgb(219, 251, 91)
        c2.CellBackColor = Color.FromArgb(219, 251, 91)
        rdGridPRSData.Columns("PRSIdentification").ConditionalFormattingObjectList.Add(c2)

        'update the grid view for the conditional formatting to take effect
        'rdGridPRSData.TableElement.Update(false);
    End Sub

    Private Function GetRadMenuItemByText(ByVal menu As RadDropDownMenu, ByVal title As String)
        Dim item As RadMenuItemBase
        For Each item In menu.Items
            If item.Text = title Then
                Return item
            End If
        Next
        Return ""
    End Function
    Private Sub rdGridPRSData_ContextMenuOpening(ByVal sender As Object, ByVal e As Telerik.WinControls.UI.ContextMenuOpeningEventArgs)
        Dim item As RadMenuItemBase
        item = GetRadMenuItemByText(e.ContextMenu, "Conditional Formatting")
        If item IsNot Nothing Then
            e.ContextMenu.Items.Remove(item)
        End If
    End Sub


#End Region
    ''' <summary>
    ''' Only for testing
    ''' This will throw a unhandled exception
    ''' Will only works on runtime; not in debug mode
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub RadButton1_Click_1(sender As System.Object, e As System.EventArgs)
        Throw New Exception("generated problem")
    End Sub



    Private Sub rlblGCLIdentification_Click(sender As Object, e As EventArgs) Handles rlblGCLIdentification.Click

    End Sub
End Class