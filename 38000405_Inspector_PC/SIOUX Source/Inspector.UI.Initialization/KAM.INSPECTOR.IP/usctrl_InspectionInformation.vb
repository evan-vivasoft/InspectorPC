Imports Telerik.WinControls.UI

Imports Inspector.BusinessLogic.Data.Configuration.Interfaces.Managers
Imports Inspector.Infra.Ioc
Imports Inspector.Model
Imports KAM.INSPECTOR.Infra
Imports KAM.INSPECTOR.IP.My.Resources


Public Class usctrl_InspectionInformation
#Region "class members"
    Private inspectionProcedures As String
    Private m_InspectionInformationManager As IInspectionInformationManager
    Private _inspectionProcedureName As String


    Enum enumlistType
        Reinspection = 0
        FirstInspection = 1
    End Enum
    Private _listType As enumlistType

#End Region
#Region "Constructor"
    ''' <summary>
    ''' Initiate form 
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()
        ' This call is required by the Windows Form Designer.
        InitializeComponent()
        ' Add any initialization after the InitializeComponent() call.
        GridViewLayout()


    End Sub
    ''' <summary>
    ''' Loading the form
    ''' Apply inspectionprocedure names to list
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub usctrl_InspectionInformation_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        'MOD 56
        If ModuleSettings.SettingFile.GetSetting(GsSectionGUI, GsSettingGUIShowDropDownSelectInspection) = False Then
            rdDropIPNames.Visible = False
        Else
            rdDropIPNames.Visible = True
        End If
    End Sub

#End Region
#Region "Properties"
    ''' <summary>
    ''' Gets or sets the inspection information manager.
    ''' </summary>
    ''' <value>
    ''' The inspection information manager.
    ''' </value>
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
    ''' <summary>
    ''' define if it is a reinspection (boundaries) or new inspection
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Listtype As enumlistType
        Get
            Return _listType
        End Get
        Set(value As enumlistType)
            _listType = value
        End Set
    End Property
    Dim _sectionIPSelection As InspectionProcedure.SectionSelection
    ''' <summary>
    ''' The Inspection procedure sections
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property SectionIPSelection() As InspectionProcedure.SectionSelection
        Get
            Dim lSectionSelection As New InspectionProcedure.SectionSelection
            lSectionSelection.InspectionProcedureName = _inspectionProcedureName
            lSectionSelection.SectionSelectionEntities = rdGridInspectionSteps.DataSource
            Return lSectionSelection
        End Get
        Private Set(value As InspectionProcedure.SectionSelection)
            _sectionIPSelection = value
        End Set
    End Property
    'MOD 21
    ''' <summary>
    ''' Fill the grid with data
    ''' Data from the first inspectionprocedure is used
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub UpdateIPSectionInformation()
        'Load the inspection procedure sections
        rdGridInspectionSteps.DataSource = InspectionInformationManager.LookupInspectionProcedureSections(InspectionInformationManager.InspectionProcedureNames.First).SectionSelectionEntities
        'MOD 21
        _inspectionProcedureName = InspectionInformationManager.InspectionProcedureNames.First
        AddCheckAllColumn()
        'Apply the different inspection procedure names to drop down list

        'MOD 56 rdDropIPNames.Visible = True
        Me.rdDropIPNames.DataSource = InspectionInformationManager.InspectionProcedureNames

        rdGridInspectionSteps.BeginUpdate()
        rdGridInspectionSteps.MasterTemplate.AutoSizeColumnsMode = GridViewAutoSizeColumnsMode.Fill
        rdGridInspectionSteps.EndUpdate()
        rdGridInspectionSteps.BestFitColumns()
    End Sub

    ''' <summary>
    ''' Load the sections with the section and subsections after a inspection out of boundaries
    ''' </summary>
    ''' <param name="sectionSelectionEntities">The list of sections and subsections. Marked isSelected if out of bound</param>
    ''' <remarks>With an out of bound it is not possible to select a new inspection procedure</remarks>
    Public Sub UpdateIPSectionInformationByBoundaries(sectionSelectionEntities As InspectionProcedure.SectionSelection)

        rdGridInspectionSteps.DataSource = sectionSelectionEntities.SectionSelectionEntities
        _inspectionProcedureName = sectionSelectionEntities.InspectionProcedureName.ToString
        rlblIPName.Text = _inspectionProcedureName
        'MOD 56 rdDropIPNames.Visible = False

        AddCheckAllColumn()

        rdGridInspectionSteps.BeginUpdate()
        rdGridInspectionSteps.MasterTemplate.AutoSizeColumnsMode = GridViewAutoSizeColumnsMode.Fill
        rdGridInspectionSteps.EndUpdate()
        rdGridInspectionSteps.BestFitColumns()
    End Sub
#End Region
#Region "Grid handling"
    ''' <summary>
    ''' Set the properties of the grid
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub GridViewLayout()
        With Me.rdGridInspectionSteps
            .BeginUpdate()
            .MasterTemplate.Reset()
            .TableElement.RowHeight = 20
            .AutoGenerateHierarchy = True
            .UseScrollbarsInHierarchy = False
            .MultiSelect = False
            .MasterTemplate.BestFitColumns()

            .ReadOnly = True
            .EnableFiltering = False
            .EnableGrouping = True
            .EnableSorting = False
            .EnableAlternatingRowColor = True
            .AllowColumnHeaderContextMenu = False
            .AllowCellContextMenu = False
            .TableElement.RowHeight = 40
            .EndUpdate()
        End With
    End Sub
    ''' <summary>
    ''' Handling if a grid is selected
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub rdGridInspectionSteps_CellClick(sender As Object, e As Telerik.WinControls.UI.GridViewCellEventArgs) Handles rdGridInspectionSteps.CellClick
        rdGridInspectionSteps.EndEdit()
        If e.RowIndex < 0 Then Exit Sub
        rdGridInspectionSteps.Rows(e.Row.Index).Cells(IpInformationDataFields.FldIsSelectedA).Value = Not rdGridInspectionSteps.Rows(e.Row.Index).Cells(IpInformationDataFields.FldIsSelectedA).Value
    End Sub
    ''' <summary>
    ''' Set colomns 
    ''' Visible; in columnchooser and headertext
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub SetColumnsIPInformationGrid()
        Try
            With Me.rdGridInspectionSteps.MasterTemplate
                .Columns(IpInformationDataFields.FldSection).IsVisible = True
                .Columns(IpInformationDataFields.FldSection).VisibleInColumnChooser = True
                .Columns(IpInformationDataFields.FldSection).HeaderText = InspectionProcedureResx.str_Section

                .Columns(IpInformationDataFields.FldSubSection).IsVisible = True
                .Columns(IpInformationDataFields.FldSubSection).VisibleInColumnChooser = True
                .Columns(IpInformationDataFields.FldSubSection).HeaderText = InspectionProcedureResx.str_SubSection

                .Columns(IpInformationDataFields.FldSequence).IsVisible = False
                .Columns(IpInformationDataFields.FldSequence).VisibleInColumnChooser = False
                .Columns(IpInformationDataFields.FldSequence).HeaderText = InspectionProcedureResx.str_Sequence
                .BestFitColumns()
                '.Columns(IpInformationDataFields.FldIsSelected).IsVisible = True
                '.Columns(IpInformationDataFields.FldIsSelected).VisibleInColumnChooser = True
                '.Columns(IpInformationDataFields.FldIsSelected).HeaderText = InspectionProcedureResx.str_Select
                '.Columns(IpInformationDataFields.FldIsSelected).Width = 20
            End With

        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub
#End Region
#Region "Form handling"
    ''' <summary>
    ''' Handling of drop down menu changed
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub rdDropIPNames_SelectedIndexChanged(sender As System.Object, e As Telerik.WinControls.UI.Data.PositionChangedEventArgs) Handles rdDropIPNames.SelectedIndexChanged
        UpdateIPSectionInformation(rdDropIPNames.Text)
    End Sub
#End Region
#Region "Update information"
    ''' <summary>
    ''' Update the information of the selected inspection procedure
    ''' The function will check if the procedure exists
    ''' </summary>
    ''' <param name="inspectionProcedureName"></param>
    ''' <remarks></remarks>
    Public Sub UpdateInspectionInformation(inspectionProcedureName As String)
        Try
            If CheckIfInspectionProcedureExists(inspectionProcedureName) = True Then
                Me.rdDropIPNames.Text = inspectionProcedureName
                rlblIPName.Text = inspectionProcedureName
                UpdateIPSectionInformation(inspectionProcedureName)
            Else
                rlblIPName.Text = InspectionProcedureResx.str_Inspection_procedure_name_does
                rdGridInspectionSteps.Visible = False
            End If

        Catch ex As Exception

        End Try
    End Sub

    ''' <summary>
    ''' Update the information in the grid. 
    ''' First be sure that the inspection procedure name exists
    ''' </summary>
    ''' <param name="inspectionProcedureName"></param>
    ''' <remarks></remarks>
    Private Sub UpdateIPSectionInformation(inspectionProcedureName As String)
        'MOD 34
        rdGridInspectionSteps.Visible = True
        Try
            With Me.rdGridInspectionSteps
                .DataSource = InspectionInformationManager.LookupInspectionProcedureSections(inspectionProcedureName).SectionSelectionEntities
                'MOD 21
                _inspectionProcedureName = inspectionProcedureName
                rlblIPName.Text = rdDropIPNames.Text
                SetColumnsIPInformationGrid()

                If _listType = enumlistType.FirstInspection Then SelectAllItems()

                rdGridInspectionSteps.BestFitColumns()
            End With

        Catch ex As Exception
            MsgBox(Me.Name & " ; " & ex.Message, MsgBoxStyle.Exclamation)
        End Try
    End Sub

#End Region
#Region "Functions"
    ''' <summary>
    ''' Check if the inspection procedure exists
    ''' If exists return true else false
    ''' </summary>
    ''' <param name="inspectionProcedureName"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CheckIfInspectionProcedureExists(inspectionProcedureName As String) As Boolean
        _inspectionProcedureName = inspectionProcedureName
        If (InspectionInformationManager.InspectionProcedureNames.ToList.Find(AddressOf CheckInspectionProcedureExists)) Is Nothing Then
            Return False
        Else : Return True
        End If
    End Function
    ''' <summary>
    ''' Handling of checking if the inspection procedure exists
    ''' </summary>
    ''' <param name="s"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function CheckInspectionProcedureExists(s As String) As Boolean
        If s = _inspectionProcedureName Then Return True Else Return False
    End Function
#End Region

#Region "Grid Select All"
    ''' <summary>
    ''' Select all items
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub SelectAllItems()
        For i = 0 To rdGridInspectionSteps.Rows.Count - 1
            With rdGridInspectionSteps
                .Rows(i).Cells(IpInformationDataFields.FldIsSelectedA).Value = True
            End With
        Next
    End Sub
    ''' <summary>
    ''' Rename the column IsSelectedA to IsSelected
    ''' This is done to return the InspectionProcedure.SectionSelection
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub RemoveColumnIsSelected()
        rdGridInspectionSteps.Columns.Remove(IpInformationDataFields.FldIsSelected)
    End Sub
    ''' <summary>
    ''' Add a new column IsSelectedA
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub AddCheckAllColumn()
        Dim checkcolumn As New CustomCheckBoxColumn
        checkcolumn.Name = IpInformationDataFields.FldIsSelectedA
        checkcolumn.HeaderText = "A"
        checkcolumn.FieldName() = IpInformationDataFields.FldIsSelected
        rdGridInspectionSteps.Columns.Insert(3, checkcolumn)
        'MOD 21
        RemoveColumnIsSelected()
        rdGridInspectionSteps.BestFitColumns()
    End Sub
#End Region
    ''' <summary>
    ''' The different fields in the XML file of plexor. Used in the grid of displaying the data
    ''' </summary>
    ''' <remarks></remarks>
    Public Class IpInformationDataFields
        Public Const FldSection As String = "Section"
        Public Const FldSubSection As String = "SubSection"
        Public Const FldSequence As String = "SequenceNumber"
        Public Const FldIsSelected As String = "IsSelected"
        Public Const FldIsSelectedA As String = "IsSelectedA"
    End Class
#Region "Class Custom cell Grid CheckBox select all"
    ''' <summary>
    ''' Create a custom check box item with option for select all
    ''' </summary>
    ''' <remarks></remarks>
    Public Class CustomCheckBoxColumn
        Inherits GridViewCheckBoxColumn
        Public Overrides Function GetCellType(row As Telerik.WinControls.UI.GridViewRowInfo) As System.Type
            If TypeOf row Is GridViewTableHeaderRowInfo Then
                Return (GetType(CheckBoxHeaderCell))
            End If
            Return MyBase.GetCellType(row)
        End Function

    End Class
#End Region
End Class
