'===============================================================================
'Copyright Wigersma 2012
'All rights reserved.
'===============================================================================
Imports System.Globalization
Imports Telerik.WinControls.UI
Imports Inspector.BusinessLogic.Interfaces
Imports Inspector.BusinessLogic.Interfaces.Events
Imports Inspector.BusinessLogic.Data.Configuration.Interfaces.Managers
Imports Inspector.Infra.Ioc
Imports System.Windows.Forms
Imports Inspector
Imports KAM.INSPECTOR.Infra
Imports System.Drawing


Public Class usctrl_FormPlexor
#Region "Class members"
    'PLEXOR device settings

    Private plexorDeviceSelected As String = ""                                             'The selected PLEXOR device

    Private stepNumber As Integer = 0                                                       'Step number durint initialization proces. For linking status

    Private lbLoadingData As Boolean = False

    'Initialization Handling
    Private m_InitializationActivityControl As IInitializationActivityControl
    Private m_PlexorInformationManager As IPlexorInformationManager
    Private ucInitialization As usctrl_Initialization
    Private m_Disposed As Boolean = False
    'MOD 07
    Private _NewPlexorDevice As Boolean = True
    'MOD 75
    Private _previousPlexorDeviceBT As String = ""
    Public Event evntInitializationStarted()
    Public Event evntInitializationFinished()
#End Region
#Region "Properties"
    ''' <summary>
    ''' Gets or sets the initialization activity control
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property InitializationActivityControl() As IInitializationActivityControl
        Get
            If m_InitializationActivityControl Is Nothing Then
                m_InitializationActivityControl = ContextRegistry.Context.Resolve(Of IInitializationActivityControl)()
            End If
            Return m_InitializationActivityControl
        End Get
        Set(value As IInitializationActivityControl)
            m_InitializationActivityControl = value
        End Set
    End Property
    ''' <summary>
    ''' Gets or sets the plexor information manager.
    ''' </summary>
    ''' <value>
    ''' The Plexor information manager.
    ''' </value>
    Public Property PlexorInformationManager() As IPlexorInformationManager
        Get
            If m_PlexorInformationManager Is Nothing Then
                m_PlexorInformationManager = ContextRegistry.Context.Resolve(Of IPlexorInformationManager)()
            End If
            Return m_PlexorInformationManager
        End Get
        Set(value As IPlexorInformationManager)
            m_PlexorInformationManager = value
        End Set
    End Property


    ''' <summary>
    ''' Enable controls on the form. 
    ''' When selecting the form during initialization, the user can not select items
    ''' </summary>
    ''' <value></value>
    ''' <remarks></remarks>
    Public WriteOnly Property DisableForm As Boolean
        Set(value As Boolean)
            rdGridPlexorData.Enabled = value
            rbInit.Enabled = value
        End Set
    End Property

#End Region
#Region "Constructor"
    ''' <summary>
    ''' Create new instance. Loading 
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()
        ' This call is required by the Windows Form Designer.
        InitializeComponent()
    End Sub
    ''' <summary>
    ''' Loading the PLEXOR data
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub usctrl_FormPlexor_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        LoadPlexorData()
        'MOD 75
        AddHandler InitializationActivityControl.DeviceUnPaired, AddressOf InitializationActivityControl_DeviceUnPaired
    End Sub
    ''' <summary>
    ''' Load the PLEXOR data into a dataset and assign it to the grid
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub LoadPlexorData()
        lbLoadingData = True
        'Get the selected PLEXOR Device
        plexorDeviceSelected = ModuleSettings.SettingFile.GetSetting(GsSectionPlexor, GsSettingPLexorSelected)
        'Load the PLEXOR data.
        FillList()
        UIculture_SetColumnNames()

        lbLoadingData = False
    End Sub
#End Region
#Region "Destructor"
    ''' <summary>
    ''' Dispose the control
    ''' </summary>
    ''' <param name="disposing"></param>
    ''' <remarks></remarks>
    Protected Overrides Sub Dispose(disposing As Boolean)
        If Not m_Disposed Then
            If disposing Then
                If m_InitializationActivityControl IsNot Nothing Then
                    m_InitializationActivityControl.Dispose()
                End If
            End If
        End If
        m_Disposed = True
    End Sub
#End Region
#Region "Set and assign grid data"
    ''' <summary>
    ''' Fill the grid with  PLEXOR data
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub FillList()
        Dim lbPlexorExists As Boolean = False

        GridViewLayout()

        Me.rdGridPlexorData.DataMember = "PLEXOR"
        Me.rdGridPlexorData.DataSource = PlexorInformationManager.PlexorsInformation
        Me.rdGridPlexorData.MasterTemplate.AutoSizeColumnsMode = GridViewAutoSizeColumnsMode.Fill


        Me.rdGridPlexorData.BeginUpdate()
        'Getting the culture to get the shortdate pattern
        Dim cultureInfo As New CultureInfo(cultureInfo.CurrentCulture.Name)
        Me.rdGridPlexorData.Columns(PlexorDataFields.FldCalibrationDate).FormatString = "{0:" & cultureInfo.DateTimeFormat.ShortDatePattern & "}"
        Me.rdGridPlexorData.EndUpdate()
        'Select the row with the previous selected/ active Plexor device
        Me.rdGridPlexorData.CurrentRow = Nothing
        For Each row In Me.rdGridPlexorData.Rows
            If "(" & row.Cells(PlexorDataFields.FldBlueToothAddress).Value & ")" = plexorDeviceSelected Then
                lbPlexorExists = True       'The Plexor in the config file exists in PLEXOR.xml
                row.IsCurrent = True
                row.IsSelected = True

                conditionalformatPairedBT()

                Exit For
            End If
        Next

        'The PLEXOR in the configuration file does not exists in PLEXOR.xml; Select first item and save it
        If lbPlexorExists = False And Me.rdGridPlexorData.Rows.Count > 0 Then
            plexorDeviceSelected = "(" & Me.rdGridPlexorData.Rows(0).Cells(PlexorDataFields.FldBlueToothAddress).Value & ")"
            ModuleSettings.SettingFile.SaveSetting(GsSectionPlexor, GsSettingPLexorSelected) = plexorDeviceSelected

        End If
    End Sub

    Private Sub conditionalformatPairedBT()
        Dim plexorDeviceSelectedTrim1 As String = Replace(plexorDeviceSelected, "(", "")
        Dim plexorDeviceSelectedTrim2 As String = Replace(plexorDeviceSelectedTrim1, ")", "")

        Me.rdGridPlexorData.Columns(PlexorDataFields.FldBlueToothAddress).ConditionalFormattingObjectList.Clear()

        Dim obj = New ConditionalFormattingObject("MyCondition", ConditionTypes.Equal, plexorDeviceSelectedTrim2, "", True)
        'obj.CellForeColor = Color.Red
        obj.RowBackColor = Color.SkyBlue
        Me.rdGridPlexorData.Columns(PlexorDataFields.FldBlueToothAddress).ConditionalFormattingObjectList.Add(obj)
    End Sub

    ''' <summary>
    ''' Set the column names to the text of the labels
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub UIculture_SetColumnNames()
        Me.rdGridPlexorData.BeginUpdate()
        For Each column In Me.rdGridPlexorData.Columns
            Select Case column.Name
                Case PlexorDataFields.FldBlueToothAddress
                    column.HeaderText() = lbl_BluetoothAddress.Text
                Case PlexorDataFields.FldName
                    column.HeaderText() = lbl_Name.Text
                Case PlexorDataFields.FldSerialNumber
                    column.HeaderText() = lbl_SerialNumber.Text
                Case PlexorDataFields.FldPressureClass
                    column.HeaderText() = lbl_PN.Text
                Case PlexorDataFields.FldCalibrationDate
                    column.HeaderText() = lbl_CalibrationDate.Text
            End Select
        Next
        Me.rdGridPlexorData.EndUpdate()
    End Sub
    ''' <summary>
    ''' Set the properties of the grid
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub GridViewLayout()
        With Me.rdGridPlexorData

            .EnableSorting = True
            .EnableGrouping = False
            .EnableFiltering = False
            .AllowColumnChooser = True

            .AllowAddNewRow = False
            .AllowRowReorder = False
            .ReadOnly = True
            .MultiSelect = False


            .AllowColumnHeaderContextMenu = False
            .AllowCellContextMenu = False

            .AutoGenerateHierarchy = True
            .UseScrollbarsInHierarchy = False
            .ShowItemToolTips = True

            .EnableAlternatingRowColor = True
            .BestFitColumns()
            ' .AutoSizeRows = True
            .TableElement.RowHeight = 40

        End With

    End Sub
    ''' <summary>
    ''' Handling Row changing of the grid
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub rdGridPlexorData_CurrentRowChanged(ByVal sender As Object, ByVal e As Telerik.WinControls.UI.CurrentRowChangedEventArgs) Handles rdGridPlexorData.CurrentRowChanged
        UpdatePanelInfo(Me.rdGridPlexorData.CurrentRow)
    End Sub
    ''' <summary>
    ''' Update the information when a row is changed
    ''' </summary>
    ''' <param name="currentRow"></param>
    ''' <remarks></remarks>
    Private Sub UpdatePanelInfo(ByVal currentRow As GridViewRowInfo)
        If currentRow IsNot Nothing AndAlso Not (TypeOf currentRow Is GridViewNewRowInfo) Then

            rtbName.Text = currentRow.Cells(PlexorDataFields.FldName).Value
            rtbSerialNumber.Text = currentRow.Cells(PlexorDataFields.FldSerialNumber).Value
            rtbBluetoothadres.Text = currentRow.Cells(PlexorDataFields.FldBlueToothAddress).Value
            rtbPressureClass.Text = currentRow.Cells(PlexorDataFields.FldPressureClass).Value
            rtbCalibrationDate.Text = currentRow.Cells(PlexorDataFields.FldCalibrationDate).Value

            If plexorDeviceSelected <> "(" & Me.rdGridPlexorData.CurrentRow.Cells(PlexorDataFields.FldBlueToothAddress).Value & ")" And lbLoadingData = False Then
                plexorDeviceSelected = "(" & Me.rdGridPlexorData.CurrentRow.Cells(PlexorDataFields.FldBlueToothAddress).Value & ")"
                'MOD 75
                'Setting is saved after initialization
                'MOD 07
                '_NewPlexorDevice = True
            End If
        End If
    End Sub
#End Region
#Region "Button handling"" "
    ''' <summary>
    ''' Handling button PLEXOR initialization 
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub rdbInit_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles rbInit.Click
        'MOD 75
        StartInitializationUnpair()

    End Sub

    'MOD 75
    ''' <summary>
    ''' Button invisible; Used to unpair the selected BT device
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub rdUnpair_Click(sender As Object, e As EventArgs) Handles rdUnpair.Click
        InitializationActivityControl.UnPairDevice(plexorDeviceSelected)
    End Sub


    'MOD 75
    ''' <summary>
    ''' Unpair Bt device if another is selected. and start the initialisation''' 
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub StartInitializationUnpair()
        If rbInit.Enabled = False Then Exit Sub
        rbInit.Enabled = False

        'MOD 81
        Dim readConfig As String
        readConfig = ModuleSettings.SettingFile.GetSetting(GsSectionPlexor, GsSettingPlexorUnpairBeforeInspection)

        'Save selected bluetooth adres to Settings File and unpair all other PLEXOR devices. Only if other PLEXOR device is selected
        If ModuleSettings.SettingFile.GetSetting(GsSectionPlexor, GsSettingPLexorSelected) <> plexorDeviceSelected Or readConfig.ToUpper = "TRUE" Then
            ModuleSettings.SettingFile.SaveSetting(GsSectionPlexor, GsSettingPLexorSelected) = plexorDeviceSelected

            'Unpair all devices
            AddHandler InitializationActivityControl.DeviceUnPairFinished, AddressOf InitializationActivityControl_DeviceUnPairedFinished 'DeviceUnPaired
            InitializationActivityControl.UnPairAllDevices()
            _NewPlexorDevice = True
        Else
            'No new device. Do not unpair
            StartInitialization()
        End If
    End Sub

    ''' <summary>
    ''' Starting the initialization proces
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub StartInitialization()
        'MOD 75
        conditionalformatPairedBT()

        stepNumber = 0
        ucInitialization = New usctrl_Initialization
        loadUserControl(rdPanelInit, ucInitialization)
        'MOD 07
        ucInitialization.NewPlexorDevice = _NewPlexorDevice
        _NewPlexorDevice = False
        ucInitialization.PlexorCalibarionDate = rtbCalibrationDate.Text

        AttachEvents()
        InitializationActivityControl.ExecuteInitialization()
    End Sub

    ' ''' <summary>
    ' ''' Handling of short cuts keys
    ' ''' </summary>
    ' ''' <param name="msg"></param>
    ' ''' <param name="keyData"></param>
    ' ''' <returns></returns>
    ' ''' <remarks></remarks>
    Protected Overrides Function ProcessCmdKey(ByRef msg As Message, _
                   ByVal keyData As Keys) As Boolean
        Const WM_KEYDOWN As Integer = &H100
        Const WM_SYSKEYDOWN As Integer = &H104

        If ((msg.Msg = WM_KEYDOWN) Or (msg.Msg = WM_SYSKEYDOWN)) Then
            Select Case (keyData)
                Case (Keys.F9)
                    Console.WriteLine("F9 Captured")
                    'starting initialization
                    'MOD 75' 
                    StartInitializationUnpair()
            End Select
        End If
        Return MyBase.ProcessCmdKey(msg, keyData)
    End Function

#End Region
#Region "User control handling"
    ''' <summary>
    ''' Loading a user control to one of the pages of radpageview
    ''' </summary>
    ''' <param name="panel"></param>
    ''' <param name="userControl"></param>
    ''' <remarks></remarks>
    Private Sub loadUserControl(ByVal panel As RadPanel, ByVal userControl As UserControl)
        'Loading the usercontrol into the toolwindow
        If panel.Controls.Count > 0 Then
            'If any usercontrol already exists in the toolwindow. Dispose it.
            For i As Integer = 1 To panel.Controls.Count
                Dim control As Control = panel.Controls(i - 1)
                If control IsNot Nothing Then
                    control.Dispose()
                End If
            Next
        End If

        panel.Controls.Add(userControl)
        userControl.Dock = DockStyle.Fill
        userControl.Left = 0
        userControl.Top = 0
        userControl.Show()
    End Sub
#End Region
#Region "Event Handling Manometer/ Bluetooth"
    ''' <summary>
    ''' Attach the events of the InitializationActivityControl control
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub AttachEvents()
        Debug.Print(Now & " AttachEvents : " & Me.Name)
        AddHandler InitializationActivityControl.InitializationStepStarted, AddressOf InitializationActivityControl_InitializationStepStarted
        AddHandler InitializationActivityControl.InitializationStepFinished, AddressOf InitializationActivityControl_InitializationStepFinished
        AddHandler InitializationActivityControl.InitializationFinished, AddressOf InitializationActivityControl_InitializationFinished
        'MOD 82
        AddHandler InitializationActivityControl.UiRequest, AddressOf InitializationActivityControl_UiRequest
    End Sub
    ''' <summary>
    ''' Detach the events of the InitializationActivityControl control
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub DetachEvents()
        RemoveHandler InitializationActivityControl.InitializationStepStarted, AddressOf InitializationActivityControl_InitializationStepStarted
        RemoveHandler InitializationActivityControl.InitializationStepFinished, AddressOf InitializationActivityControl_InitializationStepFinished
        RemoveHandler InitializationActivityControl.InitializationFinished, AddressOf InitializationActivityControl_InitializationFinished
        'MOD 75
        RemoveHandler InitializationActivityControl.DeviceUnPairFinished, AddressOf InitializationActivityControl_DeviceUnPairedFinished

        'MOD 82
        RemoveHandler InitializationActivityControl.UiRequest, AddressOf InitializationActivityControl_UiRequest

    End Sub

    'MOD 82
    ''' <summary>
    ''' MOD 82
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="eventArgs"></param>
    ''' <remarks></remarks>
    Sub InitializationActivityControl_UiRequest(ByVal sender As Object, ByVal eventArgs As System.EventArgs)
        ''   Dim startEventArgs As StartInitializationStepEventArgs = TryCast(eventArgs, StartInitializationStepEventArgs)
        Debug.Print(Now & " UI reguest : " & Me.Name)
        BeginInvoke(New Action(AddressOf InvokeInitializationUiRequest))
    End Sub

    ''' <summary>
    ''' Handles the event InitializationStepStarted of the InitializationActivityControl control
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="eventArgs"></param>
    ''' <remarks></remarks>
    Sub InitializationActivityControl_InitializationStepStarted(ByVal sender As Object, ByVal eventArgs As System.EventArgs)
        Dim startEventArgs As StartInitializationStepEventArgs = TryCast(eventArgs, StartInitializationStepEventArgs)
        Debug.Print(Now & " Step started : " & Me.Name & ": " & startEventArgs.StepId)
        BeginInvoke(New Action(Of String)(AddressOf InvokeInitializationStart), startEventArgs.StepId)
    End Sub
    ''' <summary>
    ''' Handles the event InitializationStepFinished ot the InitializationActivityControl control
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="eventArgs"></param>
    ''' <remarks></remarks>
    Sub InitializationActivityControl_InitializationStepFinished(ByVal sender As Object, ByVal eventArgs As System.EventArgs)
        Dim finishEventArgs As FinishInitializationStepEventArgs = TryCast(eventArgs, FinishInitializationStepEventArgs)
        Debug.Print(Now & " Step finished : " & Me.Name & ": " & finishEventArgs.Message & " " & finishEventArgs.ErrorCode)
        BeginInvoke(New Action(Of String, Model.InitializationStepResult, String, Integer)(AddressOf InvokeInitializationStep), finishEventArgs.StepId, finishEventArgs.Result, finishEventArgs.Message, finishEventArgs.ErrorCode)
    End Sub
    ''' <summary>
    ''' Handles the event InitializationFinished of the InitializationActivityControl control
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="eventArgs"></param>
    ''' <remarks></remarks>
    Sub InitializationActivityControl_InitializationFinished(ByVal sender As Object, ByVal eventArgs As System.EventArgs)
        Dim finishEventArgs As FinishInitializationEventArgs = TryCast(eventArgs, FinishInitializationEventArgs)
        Debug.Print(Now & " Init finished : " & Me.Name & ": " & finishEventArgs.ErrorCode)
        BeginInvoke(New Action(Of Model.InitializationResult, Integer)(AddressOf InvokeItializationFinished), finishEventArgs.Result, finishEventArgs.ErrorCode)
    End Sub

    '' MOD 75
    ''' <summary>
    ''' Handles the event DeviceUnPaired of the InitializationActivityControl control
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="eventArgs"></param>
    ''' <remarks></remarks>
    Sub InitializationActivityControl_DeviceUnPaired(ByVal sender As Object, ByVal eventArgs As System.EventArgs)
        Dim deviceUnEventArgs As DeviceUnPairedEventArgs = TryCast(eventArgs, DeviceUnPairedEventArgs)
        Debug.Print(Now & " Device unpaired: " & Me.Name & ": " & deviceUnEventArgs.Address)
    End Sub

    '' MOD 75
    ''' <summary>
    ''' Handles the event DeviceUnPairedFinished of the InitializationActivityControl control
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="eventArgs"></param>
    ''' <remarks></remarks>
    Sub InitializationActivityControl_DeviceUnPairedFinished(ByVal sender As Object, ByVal eventArgs As System.EventArgs)
        Dim deviceUnEventArgs As DeviceUnPairedEventArgs = TryCast(eventArgs, DeviceUnPairedEventArgs)
        Debug.Print(Now & " Device unpaired finished " & Me.Name)
        BeginInvoke(New Action(AddressOf StartInitialization))
    End Sub
#End Region
#Region "User control Interfacing"
    'MOD 82
    Public Sub InvokeInitializationUiRequest()
        ' Dim stepInitializationUpdate As Initialization.Model.InitializationStepModel = New Initialization.Model.InitializationStepModel(stepNumber, stepId, "", result, message, errorCode)
        '  ucInitialization.InitializationUpdateStep(stepInitializationUpdate)
        InitializationActivityControl.SetUiResponse(False)
    End Sub

    ''' <summary>
    ''' Updates the information about the initialization step.
    ''' </summary>
    ''' <param name="stepId"></param>
    ''' <param name="result"></param>
    ''' <param name="message"></param>
    ''' <param name="errorcode"></param>
    ''' <remarks></remarks>
    Public Sub InvokeInitializationStep(ByVal stepId As String, ByVal result As Model.InitializationStepResult, ByVal message As String, ByVal errorCode As Integer)
        Dim stepInitializationUpdate As Initialization.Model.InitializationStepModel = New Initialization.Model.InitializationStepModel(stepNumber, stepId, "", result, message, errorCode)
        ucInitialization.InitializationUpdateStep(stepInitializationUpdate)
    End Sub
    ''' <summary>
    ''' Initialization of step is started
    ''' </summary>
    ''' <param name="stepId"></param>
    ''' <remarks></remarks>
    Public Sub InvokeInitializationStart(ByVal stepId As String)
        stepNumber += 1
        RaiseEvent evntInitializationStarted()
        Dim stepInitializationUpdate As Initialization.Model.InitializationStepModel = New Initialization.Model.InitializationStepModel(stepNumber, stepId, "")
        ucInitialization.InitilizationNewStep(stepInitializationUpdate)
    End Sub
    ''' <summary>
    ''' Initialization manometer is finished
    ''' </summary>
    ''' <param name="result"></param>
    ''' <param name="errorCode"></param>
    ''' <remarks></remarks>
    Public Sub InvokeItializationFinished(result As Model.InitializationResult, errorCode As Integer)
        DetachEvents()
        ucInitialization.InitializationFinished(result.ToString)
        RaiseEvent evntInitializationFinished()
        rbInit.Enabled = True
    End Sub
#End Region


End Class
''' <summary>
''' The different fields in the XML file of plexor. Used in the grid of displaying the data
''' </summary>
''' <remarks></remarks>
Public Class PlexorDataFields
    Public Const FldName As String = "Name"
    Public Const FldSerialNumber As String = "SerialNumber"
    Public Const FldBlueToothAddress As String = "BlueToothAddress"
    Public Const FldPressureClass As String = "PN"
    Public Const FldCalibrationDate As String = "CalibrationDate"
End Class

