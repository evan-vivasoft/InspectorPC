'===============================================================================
'Copyright Wigersma 2012
'All rights reserved.
'===============================================================================
Imports Telerik.WinControls.UI
Imports System.Windows.Forms
Imports Telerik.WinControls.UI.Docking

Imports Inspector.BusinessLogic.Interfaces.Events
Imports Inspector.BusinessLogic.Interfaces
Imports Inspector.BusinessLogic.Data.Reporting.Interfaces
Imports Inspector.BusinessLogic.Data.Configuration.Interfaces.Managers
Imports Inspector.Model.Plexor
Imports Inspector.Infra.Ioc
Imports Inspector.Model
Imports Inspector


Imports KAM.INSPECTOR.Infra
Imports KAM.INSPECTOR.PLEXOR
Imports KAM.INSPECTOR.info.modLicenseInfo
Imports KAM.INSPECTOR.IP.My.Resources


Public Class usctrl_MainInspection
#Region "Class members"

    'The different script command user controls
    Private WithEvents ucScriptCommand1 As usctrl_scriptCommand1
    Private WithEvents ucScriptCommand3 As usctrl_scriptCommand3
    Private WithEvents ucScriptCommand4 As usctrl_scriptCommand4
    Private WithEvents ucScriptCommand41 As usctrl_Scriptcommand41
    'Private WithEvents ucScriptCommand42 As usctrl_Scriptcommand42
    Private WithEvents ucScriptCommand43 As usctrl_scriptCommand43
    Private WithEvents ucScriptCommand5X As usctrl_scriptCommand5x
    Private WithEvents ucScriptCommand70 As Scriptcommand70

    Private ucScriptCommand42 As usctrl_Scriptcommand42

    Private WithEvents WiManometerErrorHandling As New clsErrorInitHandling
    Private lbShowPrsGclFormAfterInspection As Boolean = True

    Private _scriptCommand2Settings As InspectionProcedure.ScriptCommand2           'Store the settings of script command 2.
    Private _scriptCommand42Settings As InspectionProcedure.ScriptCommand42         'MOD 61
    Private _bsetSc2toSc4TextValue As Boolean                                        'MOD 61

    Private m_InspectionActivityControl As IInspectionActivityControl               'Initialization control
    Private m_InspectionResultReader As IInspectionResultReader

    Private m_Disposed As Boolean = False

    Private WithEvents ucInspectionInformation As New KAM.INSPECTOR.IP.usctrl_InspectionInformation
    Private lbloadUCInspectionInformation As Boolean = False                        'Checks if the form is unloaded

    Private _gclName As String = ""
    Private _prsName As String = ""
    Private _inspectionProcedureName As String = ""

    Private firstStart As Boolean = True

    Private infraResources As New clsResourceConvert
    'MOD 07
    Private _NewPlexorDevice As Boolean = True
    Private m_PlexorInformationManager As IPlexorInformationManager

    'Initialization
    Private ucInitialization As usctrl_Initialization
    'MOD 74
    Private manometerInitializationEnabled As Boolean = False
    Private plexorInitStepNumber As Integer = 0

    'Set manometer units
    Private _manometerTh1Unit As String = "-"
    Private _manometerTh2Unit As String = "-"

    'MOD 21
    Private sectionSelectionvalueOutOfBoundaries As InspectionProcedure.SectionSelection

    '' MOD 79
    Private inspectionRetryDoUnpair As Boolean = False

    Public Event evntInspectionCompleted(ByVal prsName As String, ByVal gclName As String) 'Raise the event in case the inspection is completed (no matter which way)
    Public Event evntShowRemarkWindow(ByVal script42Item As InspectionProcedure.ScriptCommand42)
    Public Event evntInspectionStarted()
    Public Event evntInspectionFinished()
#End Region

#Region "Constructor"
    ''' <summary>
    ''' Initialize form 
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()
        ' This call is required by the designer.
        Me.InitializeComponent()
        ' Add any initialization after the InitializeComponent() call.


        Me.rButStop.Top = rButStart.Top
        Me.rButStop.Left = rButStart.Left

        Me.rButStop.Visible = False
        Me.rButStop.Enabled = False
        Me.rButStart.Visible = True
        Me.rButStart.Enabled = True
        Me.rButStartBoundInspect.Visible = False
        Me.rButStartBoundInspect.Enabled = False
        Me.rButStopBoundInspect.Visible = False
        Me.rButStopBoundInspect.Enabled = False

        'MOD 38
        rlblPRSName.Text = ""
        rlblGCLName.Text = My.Resources.InspectionProcedureResx.str_LinePRStoGCL

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
                If m_InspectionActivityControl IsNot Nothing Then
                    m_InspectionActivityControl.Dispose()
                End If
            End If
        End If
        m_Disposed = True
    End Sub
#End Region

#Region "Properties"
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
#End Region
#Region "Button Handling"
    ''' <summary>
    ''' Start the inspection
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub rButStart_Click(sender As System.Object, e As System.EventArgs) Handles rButStart.Click, rButStartBoundInspect.Click
        'MOD 60
        DebugGUILogger.Debug(Me.Name & "Inspection started") 'MOD 88
        rButStartBoundInspect.Enabled = False
        rButStart.Enabled = False
        'MOD 78
        'StartInspection()
        StartInspectionUnpair()
    End Sub

    'MOD 78
    ''' <summary>
    ''' Unpair Bt device if another is selected. and start the initialisation''' 
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub StartInspectionUnpair()

        'Save selected bluetooth adres to Settings File and unpair all other PLEXOR devices. Only if other PLEXOR device is selected
        'If ModuleSettings.SettingFile.GetSetting(GsSectionPlexor, GsSettingPLexorSelected) <> plexorDeviceSelected Then

        Dim readConfig As String
        readConfig = ModuleSettings.SettingFile.GetSetting(GsSectionPlexor, GsSettingPlexorUnpairBeforeInspection)

        If inspectionRetryDoUnpair = True Or readConfig.ToUpper = "TRUE" Then
            'Unpair all devices
            AddHandler InspectionActivityControl.DeviceUnPairFinished, AddressOf InspectionActivityControl_DeviceUnPairedFinished 'DeviceUnPaired
            InspectionActivityControl.UnPairAllDevices()
            'Inspection is started after an unpair all devices
        Else
            inspectionRetryDoUnpair = True
            'No new device. Do not unpair
            StartInspection()
        End If
    End Sub


    ''' <summary>
    ''' Starting the inspection
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub StartInspection()
        'MOD 20
        'Check if any partial inspection is selected.
        Dim lbSelectionEntities As Boolean = False
        For i = 1 To ucInspectionInformation.SectionIPSelection.SectionSelectionEntities.Count
            If ucInspectionInformation.SectionIPSelection.SectionSelectionEntities(i - 1).IsSelected = True Then
                lbSelectionEntities = True
                Exit For
            End If
        Next i
        If lbSelectionEntities = False Then
            MsgBox(My.Resources.InspectionProcedureResx.str_NoSectionSelected, MsgBoxStyle.Information, QlmProductName)
            Exit Sub
        End If

        lbShowPrsGclFormAfterInspection = True
        plexorInitStepNumber = 0

        Me.rButStart.Visible = False
        Me.rButStart.Enabled = False

        Me.rButStartBoundInspect.Visible = False
        Me.rButStartBoundInspect.Enabled = False
        Me.rButStopBoundInspect.Visible = False
        Me.rButStopBoundInspect.Enabled = False

        If _gclName = "" Then
            StartPartialInspection(ucInspectionInformation.SectionIPSelection, _prsName, Nothing)
            'StartInspection(_prsName, Nothing)
        Else
            StartPartialInspection(ucInspectionInformation.SectionIPSelection, _prsName, _gclName)
            'StartInspection(_prsName, _gclName)
        End If

        Me.rButStop.Visible = True
        Me.rButStop.Enabled = True
    End Sub
    ''' <summary>
    ''' The user stops the inspection
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub rButStop_Click(sender As System.Object, e As System.EventArgs) Handles rButStop.Click
        'MOD 10 
        'MOD 31
        'MOD 60
        rButStop.Enabled = False
        StopInspection()
    End Sub
    ''' <summary>
    ''' Handling of stopping the inspection
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub StopInspection()
        Dim ok = MsgBox(InspectionProcedureResx.str_DoYouWishStopInspectionByUser, MsgBoxStyle.Information + MsgBoxStyle.YesNo, QlmProductName)
        If ok = vbYes Then

            rButStop.Visible = False
            rButStop.Enabled = False
            ScriptCommandStepStoreRemark()
            StopInspectionByUser()
        Else
            rButStop.Enabled = True
        End If
    End Sub
    'MOD 21
    ''' <summary>
    ''' The user stop a inspection with is out of Boundaries
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub rButStopBoundInspect_Click(sender As System.Object, e As System.EventArgs) Handles rButStopBoundInspect.Click
        'MOD 60
        rButStopBoundInspect.Enabled = False
        StopBoundInspection()
    End Sub
    ''' <summary>
    ''' Hanlding of stopping a out of bound inspection
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub StopBoundInspection()
        Me.rButStartBoundInspect.Visible = False
        Me.rButStartBoundInspect.Enabled = False
        Me.rButStopBoundInspect.Visible = False
        Me.rButStopBoundInspect.Enabled = False
        Me.rButStart.Visible = True
        Me.rButStart.Enabled = True
        RaiseEvent evntInspectionFinished()
        'Raise event to display the PRS form and update status informations
        'This wil unload the remark window
        RaiseEvent evntInspectionCompleted(_prsName, _gclName)

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
                Case (Keys.Shift Or Keys.F9)
                    'Stop inspection or bound inspection
                    Console.WriteLine("<SHIFT> + F9 Captured")
                    If rButStop.Visible = True Then
                        StopInspection()
                    ElseIf rButStopBoundInspect.Visible = True Then
                        StopBoundInspection()
                    End If

                Case (Keys.F9)
                    Console.WriteLine("F9 Captured")
                    'start inspection or bound inspection
                    If rButStart.Visible = True Then
                        StartInspection()
                    ElseIf rButStartBoundInspect.Visible = True Then
                        StartInspection()
                    End If

            End Select
        End If
        Return MyBase.ProcessCmdKey(msg, keyData)
    End Function
#End Region


#Region "Main Inspection procedure handling"

#Region "Properties"
    ''' <summary>
    ''' Gets or sets the initialization activity control.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks>The initialization activity control.</remarks>
    Public Property InspectionActivityControl() As IInspectionActivityControl
        Get
            If m_InspectionActivityControl Is Nothing Then
                m_InspectionActivityControl = ContextRegistry.Context.Resolve(Of IInspectionActivityControl)()
            End If
            Return m_InspectionActivityControl
        End Get
        Set(value As IInspectionActivityControl)
            m_InspectionActivityControl = value
        End Set
    End Property
    ''' <summary>
    ''' Gets or sets the initialization results reader.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks>The initialization results reader.</remarks>
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
    ''' Property of user control script command 42
    ''' This control is loaded in the main window application
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property ScriptCommand42 As usctrl_Scriptcommand42
        Get
            Return ucScriptCommand42
        End Get
        Set(value As usctrl_Scriptcommand42)
            ucScriptCommand42 = value
        End Set
    End Property



#End Region
#Region "Interface"
    ''' <summary>
    ''' Passing the data to the Inspection form. 
    ''' </summary>
    ''' <param name="prsName"></param>
    ''' <param name="gclName"></param>
    ''' <param name="inspectionProcedureName"></param>
    ''' <remarks></remarks>
    Public Sub UpdatePRSGCLInformation(prsName As String, gclName As String, inspectionProcedureName As String)
        If IsNothing(prsName) Then Exit Sub
        _prsName = prsName
        _gclName = gclName
        _inspectionProcedureName = inspectionProcedureName
        rlblPRSName.Text = prsName.ToString
        If gclName <> "" Then rlblGCLName.Text = My.Resources.InspectionProcedureResx.str_LinePRStoGCL & " " & gclName.ToString Else rlblGCLName.Text = ""

        If lbloadUCInspectionInformation = False Then
            'Load user control with inspectionInformation
            ucInspectionInformation = New KAM.INSPECTOR.IP.usctrl_InspectionInformation
            LoadUserControl(rdPanelInspection, ucInspectionInformation, False)
            ucInspectionInformation.UpdateIPSectionInformation()
        End If

        ucInspectionInformation.Listtype = usctrl_InspectionInformation.enumlistType.FirstInspection
        'check if the inspection procedure name exists 
        ucInspectionInformation.UpdateInspectionInformation(_inspectionProcedureName)
        If ucInspectionInformation.CheckIfInspectionProcedureExists(inspectionProcedureName) = True Then
            rButStart.Visible = True
            rButStart.Enabled = True
        Else
            rButStart.Visible = False
            rButStart.Enabled = False
        End If

    End Sub
#End Region
#Region "Start stop inspection procedure handling"
    'NOT USED
    ' ''' <summary>
    ' ''' Start a complete inspection procedure for the selected PRS and/or GCL
    ' ''' </summary>
    ' ''' <param name="prsName"></param>
    ' ''' <param name="gclName"></param>
    ' ''' <remarks></remarks>
    'Public Sub StartInspection(prsName As String, gclName As String)
    '    RaiseEvent evntInspectionStarted()
    '    DetachEvents()
    '    AttachEvents()

    '    If Not InspectionActivityControl.ExecuteInspection(prsName, gclName) Then
    '        MsgBox(My.Resources.InspectionProcedureResx.str_It_is_not_allowed_to_run, , QlmProductName)
    '    End If
    'End Sub
    ''' <summary>
    ''' Start a partional inspection for the selected PRS and/or GCL
    ''' </summary>
    ''' <param name="sectionIPSelection"></param>
    ''' <param name="prsName"></param>
    ''' <param name="gclName"></param>
    ''' <remarks></remarks>
    Public Sub StartPartialInspection(sectionIPSelection As InspectionProcedure.SectionSelection, prsName As String, gclName As String)
        RaiseEvent evntInspectionStarted()
        DetachEvents()
        AttachEvents()

        If Not InspectionActivityControl.ExecutePartialInspection(sectionIPSelection, prsName, gclName) Then
            MsgBox(My.Resources.InspectionProcedureResx.str_It_is_not_allowed_to_run, , QlmProductName)
        End If
    End Sub
    ''' <summary>
    ''' Stop the inspection procedure
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub StopInspectionByUser()
        lbShowPrsGclFormAfterInspection = True
        InspectionActivityControl.Abort()
    End Sub
#End Region
#Region "Attach and detach event for InspectionActivityControl; Measurement and steps"
    ''' <summary>
    ''' Attach the events for Manometer initialization
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub AttachEvents()
        Debug.Print(Now & " Events attached")
        AddHandler InspectionActivityControl.ExecuteInspectionStep, AddressOf InspectionActivityControl_ExecuteInspectionStep
        AddHandler InspectionActivityControl.InspectionFinished, AddressOf InspectionActivityControl_InspectionFinished
        'MOD 59
        AddHandler InspectionActivityControl.SafetyValueTriggered, AddressOf InspectionActivityControl_SafetyValueTriggered
        'Initialization PLEXOR Manometer Events
        AddHandler InspectionActivityControl.InitializationStepStarted, AddressOf InspectionActivityControl_InitializationStepStarted
        AddHandler InspectionActivityControl.InitializationStepFinished, AddressOf InspectionActivityControl_InitializationStepFinished
        AddHandler InspectionActivityControl.InitializationFinished, AddressOf InspectionActivityControl_InitializationFinished

        AddHandler InspectionActivityControl.InspectionError, AddressOf InspectionActivityControl_InspectionError

        'MOD 82
        AddHandler InspectionActivityControl.UiRequest, AddressOf InspectionActivityControl_UiRequest

    End Sub
    ''' <summary>
    ''' Detach the events for Manometer initialization
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub DetachEvents()
        Debug.Print(Now & " Events detached")
        RemoveHandler InspectionActivityControl.ExecuteInspectionStep, AddressOf InspectionActivityControl_ExecuteInspectionStep
        RemoveHandler InspectionActivityControl.InspectionFinished, AddressOf InspectionActivityControl_InspectionFinished
        'MOD 59
        RemoveHandler InspectionActivityControl.SafetyValueTriggered, AddressOf InspectionActivityControl_SafetyValueTriggered

        'Initialization PLEXOR Manometer Events
        RemoveHandler InspectionActivityControl.InitializationStepStarted, AddressOf InspectionActivityControl_InitializationStepStarted
        RemoveHandler InspectionActivityControl.InitializationStepFinished, AddressOf InspectionActivityControl_InitializationStepFinished
        RemoveHandler InspectionActivityControl.InitializationFinished, AddressOf InspectionActivityControl_InitializationFinished

        RemoveHandler InspectionActivityControl.InspectionError, AddressOf InspectionActivityControl_InspectionError

        'MOD 78
        RemoveHandler InspectionActivityControl.DeviceUnPairFinished, AddressOf InspectionActivityControl_DeviceUnPairedFinished 'DeviceUnPaired

        'MOD 82
        RemoveHandler InspectionActivityControl.UiRequest, AddressOf InspectionActivityControl_UiRequest

    End Sub
    ''' <summary>
    ''' Attach the events for the manometer measurement
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub AttachEventsMeasurements()
        Debug.Print(Now & " Measurement Events attached")

        'Event Handling of Measurement; Script command 5x and 70
        'MOD 26
        AddHandler InspectionActivityControl.ContinuousMeasurementStarted, AddressOf InspectionActivityControl_ContinuousMeasurementStarted

        AddHandler InspectionActivityControl.MeasurementsCompleted, AddressOf InspectionActivityControl_MeasurementsCompleted
        AddHandler InspectionActivityControl.ExtraMeasurementStarted, AddressOf InspectionActivityControl_ExtraMeasurementStarted
        AddHandler InspectionActivityControl.MeasurementResult, AddressOf InspectionActivityControl_MeasurementResult
    End Sub
    ''' <summary>
    ''' Detach the events for the manometer measurment
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub DetachEventsMeasurements()

        Debug.Print(Now & " Measurement Events detached")

        'Event Handling of Measurement; Script command 5x and 70
        'MOD 26
        RemoveHandler InspectionActivityControl.ContinuousMeasurementStarted, AddressOf InspectionActivityControl_ContinuousMeasurementStarted
        RemoveHandler InspectionActivityControl.MeasurementsReceived, AddressOf InspectionActivityControl_MeasurementsReceived
        RemoveHandler InspectionActivityControl.MeasurementsCompleted, AddressOf InspectionActivityControl_MeasurementsCompleted
        RemoveHandler InspectionActivityControl.ExtraMeasurementStarted, AddressOf InspectionActivityControl_ExtraMeasurementStarted
        RemoveHandler InspectionActivityControl.MeasurementResult, AddressOf InspectionActivityControl_MeasurementResult
    End Sub
#End Region

#Region "Inspection steps handling; Events and user interfaces"
#Region "Event handling of Inspection Step Handling"
    ''' <summary>
    ''' Handles the ExecuteInspectionStep event of the InspectionActivityControl control.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub InspectionActivityControl_ExecuteInspectionStep(ByVal sender As Object, ByVal e As EventArgs)
        Dim stepEventArgs As ExecuteInspectionStepEventArgs = TryCast(e, ExecuteInspectionStepEventArgs)
        Debug.Print(Now & " Execute Inspection Step : " & Me.Name & ": " & stepEventArgs.CurrentInspectionStep.ToString & " : " & stepEventArgs.ScriptCommand.ToString)
        BeginInvoke(
            New Action(Of InspectionProcedure.ScriptCommandBase, Integer, Integer)(AddressOf ExecuteInspectionCommand),
            stepEventArgs.ScriptCommand,
            stepEventArgs.CurrentInspectionStep,
            stepEventArgs.TotalInspectionSteps)
    End Sub
    ''' <summary>
    ''' Handles the InspectionFinished event of the InspectionActivityControl control.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub InspectionActivityControl_InspectionFinished(ByVal sender As Object, ByVal e As EventArgs)
        Dim finishedEventargs As InspectionFinishedEventArgs = TryCast(e, InspectionFinishedEventArgs)
        Debug.Print(Now & " Inspection finished : " & Me.Name & ": " & finishedEventargs.ErrorCode)

        BeginInvoke(New Action(Of String, InspectionProcedure.InspectionStatus, InspectionProcedure.SectionSelection)(AddressOf InvokeExecuteInspectionFishished), finishedEventargs.ErrorCode.ToString, finishedEventargs.Result, finishedEventargs.PartialInspection)
    End Sub
    ''' <summary>
    ''' Handling of the invoke of inspection finished
    ''' </summary>
    ''' <param name="errorcode"></param>
    ''' <remarks></remarks>
    Private Sub InvokeExecuteInspectionFishished(errorcode As String, status As InspectionProcedure.InspectionStatus, valueOutOfBoundaries As InspectionProcedure.SectionSelection)
        'MOD 21

        '<MODDLR MOD 101'> 
        ucScriptCommand42?.Dispose()
        ucScriptCommand42 = Nothing
        '</MODDLR>

        sectionSelectionvalueOutOfBoundaries = valueOutOfBoundaries

        WiManometerErrorHandling.ErrorCodeHandling(errorcode)
    End Sub

#End Region
#Region "User control interface of inspection steps"
    ''' <summary>
    ''' Handling of the different steps of the inspection procedure. Display the different user controls for the inspection steps
    ''' </summary>
    ''' <param name="scriptCommand"></param>
    ''' <param name="currentInspectionStep"></param>
    ''' <param name="totalInspectionSteps"></param>
    ''' <remarks></remarks>
    Private Sub ExecuteInspectionCommand(scriptCommand As InspectionProcedure.ScriptCommandBase, currentInspectionStep As Integer, totalInspectionSteps As Integer)
        Debug.Print(Now & " Script sequencenumber : " & Me.Name & ": SC General " & scriptCommand.SequenceNumber)
        'MOD 79
        If inspectionRetryDoUnpair = True Then inspectionRetryDoUnpair = False

        ''<MODDLR>
        Debug.Print($"ExecuteInspectionCommand: {scriptCommand.GetType().Name}")
        ''</MODDLR>

        Select Case scriptCommand.GetType
            Case GetType(InspectionProcedure.ScriptCommand1)
                Dim scType As InspectionProcedure.ScriptCommand1 = TryCast(scriptCommand, InspectionProcedure.ScriptCommand1)
                ucScriptCommand1 = New usctrl_scriptCommand1

                ucScriptCommand1.Scriptcommand1 = scType
                ucScriptCommand1.CurrentStep = currentInspectionStep
                ucScriptCommand1.TotalStep = totalInspectionSteps
                ucScriptCommand1.Scriptcommand2 = _scriptCommand2Settings

                'MOD 60
                ucScriptCommand1.ShowNextButtonAtStartup = True

                ucScriptCommand1.LoadinformationSC2()
                LoadUserControl(Me.rdPanelInspection, ucScriptCommand1, True)

                ucScriptCommand1.LoadInformation()
            Case GetType(InspectionProcedure.ScriptCommand2)
                Dim scType As InspectionProcedure.ScriptCommand2 = TryCast(scriptCommand, InspectionProcedure.ScriptCommand2)
                'Store the information to display during other scriptcommands
                _scriptCommand2Settings = scType
                Debug.Print(Now & " Script sequencenumber : " & Me.Name & ": SC2 " & scriptCommand.SequenceNumber)
                InspectionActivityControl.InspectionStepComplete(New InspectionStepResult.InspectionStepResultEmpty(scriptCommand.SequenceNumber))

            Case (GetType(InspectionProcedure.ScriptCommand3))
                Dim scType As InspectionProcedure.ScriptCommand3 = TryCast(scriptCommand, InspectionProcedure.ScriptCommand3)
                ucScriptCommand3 = New usctrl_scriptCommand3

                ucScriptCommand3.Scriptcommand3 = scType
                ucScriptCommand3.CurrentStep = currentInspectionStep
                ucScriptCommand3.TotalStep = totalInspectionSteps
                ucScriptCommand3.Scriptcommand2 = _scriptCommand2Settings

                ucScriptCommand3.LoadinformationSC2()
                LoadUserControl(Me.rdPanelInspection, ucScriptCommand3, True)
                ucScriptCommand3.LoadInformation()

            Case GetType(InspectionProcedure.ScriptCommand4)
                Dim scType As InspectionProcedure.ScriptCommand4 = TryCast(scriptCommand, InspectionProcedure.ScriptCommand4)
                ucScriptCommand4 = New usctrl_scriptCommand4

                ucScriptCommand4.Scriptcommand4 = scType

                'MOD 61
                _bsetSc2toSc4TextValue = False
                If ucScriptCommand42 IsNot Nothing Then
                    'Display the value of scriptcommand 42 at scriptcommand 4
                    If scType.StationStepObject.FieldNo = _scriptCommand42Settings.StationStepObject.FieldNo And _
                        (scType.StationStepObject.MeasurePoint = _scriptCommand42Settings.StationStepObject.MeasurePoint And scType.StationStepObject.ObjectName = _scriptCommand42Settings.StationStepObject.ObjectName) Then

                        If scType.TypeQuestion = TypeQuestion.InputMultiLines Or scType.TypeQuestion = TypeQuestion.InputSingleLine Then
                            _bsetSc2toSc4TextValue = True
                            ucScriptCommand4.Scriptcommand4Result = ucScriptCommand42.RemarkValue
                        End If
                    End If
                End If
                ucScriptCommand4.CurrentStep = currentInspectionStep
                ucScriptCommand4.TotalStep = totalInspectionSteps
                ucScriptCommand4.Scriptcommand2 = _scriptCommand2Settings
                'MOD 60
                If scType.Required = False Then ucScriptCommand4.ShowNextButtonAtStartup = True

                ucScriptCommand4.LoadinformationSC2()
                LoadUserControl(Me.rdPanelInspection, ucScriptCommand4, True)
                ucScriptCommand4.LoadInformation()
            Case GetType(InspectionProcedure.ScriptCommand41)
                Dim scType As InspectionProcedure.ScriptCommand41 = TryCast(scriptCommand, InspectionProcedure.ScriptCommand41)
                ucScriptCommand41 = New usctrl_Scriptcommand41

                ucScriptCommand41.Scriptcommand41 = scType
                ucScriptCommand41.CurrentStep = currentInspectionStep
                ucScriptCommand41.TotalStep = totalInspectionSteps
                ucScriptCommand41.Scriptcommand2 = _scriptCommand2Settings

                ucScriptCommand41.LoadinformationSC2()
                LoadUserControl(Me.rdPanelInspection, ucScriptCommand41, True)
                ucScriptCommand41.LoadInformation()

            Case GetType(InspectionProcedure.ScriptCommand42)
                Dim scType As InspectionProcedure.ScriptCommand42 = TryCast(scriptCommand, InspectionProcedure.ScriptCommand42)

                'MOD 61; Store previous result
                If ucScriptCommand42 IsNot Nothing Then ScriptCommandStepStoreRemark()

                'MOD 61
                _scriptCommand42Settings = scType
                'The user control is on the main form
                ucScriptCommand42 = New usctrl_Scriptcommand42
                ucScriptCommand42.Scriptcommand42 = scType

                RaiseEvent evntShowRemarkWindow(scType)
                InspectionActivityControl.InspectionStepComplete(New InspectionStepResult.InspectionStepResultEmpty(scriptCommand.SequenceNumber))

            Case GetType(InspectionProcedure.ScriptCommand43)
                Dim scType As InspectionProcedure.ScriptCommand43 = TryCast(scriptCommand, InspectionProcedure.ScriptCommand43)
                ucScriptCommand43 = New usctrl_scriptCommand43

                ucScriptCommand43.Scriptcommand43 = scType
                ucScriptCommand43.CurrentStep = currentInspectionStep
                ucScriptCommand43.TotalStep = totalInspectionSteps
                ucScriptCommand43.Scriptcommand2 = _scriptCommand2Settings
                'MOD 60
                ucScriptCommand43.ShowNextButtonAtStartup = True

                ucScriptCommand43.LoadinformationSC2()
                LoadUserControl(Me.rdPanelInspection, ucScriptCommand43, True)
                ucScriptCommand43.LoadInformation()

            Case GetType(InspectionProcedure.ScriptCommand5X)
                AttachEventsMeasurements()
                firstStart = True
                Dim scType As InspectionProcedure.ScriptCommand5X = TryCast(scriptCommand, InspectionProcedure.ScriptCommand5X)
                ucScriptCommand5X = New usctrl_scriptCommand5x
                'Set the unit of the manometer
                Select Case scType.DigitalManometer
                    Case DigitalManometer.TH1
                        ucScriptCommand5X.ManometerUnit = _manometerTh1Unit
                    Case DigitalManometer.TH2
                        ucScriptCommand5X.ManometerUnit = _manometerTh2Unit
                End Select

                Dim lastResult1 As New InspectionReportingResults.ReportResult
                Dim lastResult As New InspectionReportingResults.ReportResult
                'Search for the last result


                lastResult1 = InspectionResultReader.LookupPreviousToLastReportResult(_prsName, _gclName, scType.StationStepObject.MeasurePoint, scType.StationStepObject.ObjectName, scType.StationStepObject.FieldNo)

                If lastResult1 IsNot Nothing Then
                    'MOD 57
                    If lastResult1.MeasureValue IsNot Nothing Then
                        Dim values As String = [String].Format("Values | '{0}' | '{1}' | '{2}' | '{3}' | '{4}' | '{5}' | '{6}' | '{7}' | '{8}' |", lastResult1.FieldNo.ToString, lastResult1.MeasurePoint.ToString, lastResult1.MeasurePointDescription, lastResult1.MeasurePointID.ToString, lastResult1.ObjectName.ToString, lastResult1.ObjectID.ToString, lastResult1.ObjectNameDescription, lastResult1.MeasureValue.Value.ToString, lastResult1.MeasureValue.UOM.ToString)
                        Debug.Print(Now & " : previous result: " & values)
                    End If
                End If


                lastResult = InspectionResultReader.LookupLastReportResult(_prsName, _gclName, scType.StationStepObject.MeasurePoint, scType.StationStepObject.ObjectName, scType.StationStepObject.FieldNo)

                If lastResult IsNot Nothing Then
                    'MOD 57
                    If lastResult.MeasureValue IsNot Nothing Then
                        Dim values As String = [String].Format("Values | '{0}' | '{1}' | '{2}' | '{3}' | '{4}' | '{5}' | '{6}' | '{7}' | '{8}' |", lastResult.FieldNo.ToString, lastResult.MeasurePoint.ToString, lastResult.MeasurePointDescription, lastResult.MeasurePointID.ToString, lastResult.ObjectName.ToString, lastResult.ObjectID.ToString, lastResult.ObjectNameDescription, lastResult.MeasureValue.Value.ToString, lastResult.MeasureValue.UOM.ToString)
                        Debug.Print(Now & " : last result: " & values)
                    End If
                End If



                ucScriptCommand5X.Scriptcommand5x = scType
                ucScriptCommand5X.CurrentStep = currentInspectionStep
                ucScriptCommand5X.TotalStep = totalInspectionSteps
                ucScriptCommand5X.Scriptcommand2 = _scriptCommand2Settings
                ucScriptCommand5X.lastResult = lastResult

                'MOD 60
                ucScriptCommand5X.ShowNextButtonAtStartup = False


                ucScriptCommand5X.LoadinformationSC2()
                LoadUserControl(Me.rdPanelInspection, ucScriptCommand5X, True)
                ucScriptCommand5X.LoadInformation()

            Case GetType(InspectionProcedure.ScriptCommand70)
                InspectionActivityControl.InspectionStepComplete(New InspectionStepResult.InspectionStepResultEmpty(scriptCommand.SequenceNumber))
        End Select
    End Sub
#End Region
#Region "Events from User controls scriptcommands"
    ''' <summary>
    ''' Handling the events of the user control; Step with no result
    ''' </summary>
    ''' <param name="stepSequenceNumber"></param>
    ''' <remarks></remarks>
    Private Sub ScriptCommamdStepExecutedNoResult(stepSequenceNumber As Integer) Handles ucScriptCommand1.evntNext, ucScriptCommand3.evntNext, ucScriptCommand4.evntNext, ucScriptCommand41.evntNext, ucScriptCommand43.evntNext, ucScriptCommand5X.evntNext ', ucScriptCommand42.evntNext
        ScriptCommandStepStoreRemark()
        Debug.Print(Now & " Script sequencenumber : " & Me.Name & " 1: " & stepSequenceNumber)
        DebugGUILogger.Debug(Me.Name & "Event Next With result Script sequencenumber : " & Me.Name & " 1: " & stepSequenceNumber) 'MOD 88
        DebugGUILogger.Debug(Me.Name & "Inspectionstepcompleted 1:") 'MOD 88
        InspectionActivityControl.InspectionStepComplete(New InspectionStepResult.InspectionStepResultEmpty(stepSequenceNumber))
    End Sub
    ''' <summary>
    ''' Handling the events of the user control; Step with list result
    ''' </summary>
    ''' <param name="listResult"></param>
    ''' <remarks></remarks>
    Private Sub ScriptCommamdStepExecutedWitListResult(listResult As InspectionStepResult.InspectionStepResultSelections) Handles ucScriptCommand41.evntNextWithListResult
        ScriptCommandStepStoreRemark()
        Debug.Print(Now & " Script sequencenumber : " & Me.Name & " 2: " & listResult.SequenceNumber)
        DebugGUILogger.Debug(Me.Name & "Event Next With result Script sequencenumber : " & Me.Name & " 2: " & listResult.SequenceNumber) 'MOD 88
        If listResult.Remark <> "" Then
            DebugGUILogger.Debug(Me.Name & "Inspectionstepcompleted 2a:") 'MOD 88
            InspectionActivityControl.InspectionStepComplete(New InspectionStepResult.InspectionStepResultSelections(listResult.Remark, listResult.SequenceNumber, listResult.AnswerSelection1, listResult.AnswerSelection2, listResult.AnswerSelection3, listResult.AnswerSelection4, listResult.AnswerSelection5))
        Else
            DebugGUILogger.Debug(Me.Name & "Inspectionstepcompleted 2b:") 'MOD 88
            InspectionActivityControl.InspectionStepComplete(New InspectionStepResult.InspectionStepResultSelections(listResult.SequenceNumber, listResult.AnswerSelection1, listResult.AnswerSelection2, listResult.AnswerSelection3, listResult.AnswerSelection4, listResult.AnswerSelection5))
        End If
    End Sub
    ''' <summary>
    ''' Handling the events of the user control; Step with text result
    ''' </summary>
    ''' <param name="stepSequenceNumber"></param>
    ''' <param name="result"></param>
    ''' <remarks></remarks>
    Private Sub ScriptCommamd4StepExecutedWithTextResult(stepSequenceNumber As Integer, result As String) Handles ucScriptCommand4.evntNextWithResult
        Debug.Print(Now & " Script sequencenumber : " & Me.Name & " 3: " & stepSequenceNumber)
        DebugGUILogger.Debug(Me.Name & "Event Next With result Script sequencenumber : " & Me.Name & " 3: " & stepSequenceNumber) 'MOD 88
        'MOD 61
        If _bsetSc2toSc4TextValue = True Then
            'The value is stored in scriptcommand 42. Because the resultsfields of SC2 and SC42 are the same
            ucScriptCommand42.RemarkValue = result
            ScriptCommandStepStoreRemark()
            DebugGUILogger.Debug(Me.Name & "Inspectionstepcompleted 3a:") 'MOD 88
            InspectionActivityControl.InspectionStepComplete(New InspectionStepResult.InspectionStepResultEmpty(stepSequenceNumber))
            _bsetSc2toSc4TextValue = False
        Else
            ScriptCommandStepStoreRemark()
            DebugGUILogger.Debug(Me.Name & "Inspectionstepcompleted 3b:") 'MOD 88
            InspectionActivityControl.InspectionStepComplete(New InspectionStepResult.InspectionStepResultText(stepSequenceNumber, result))
        End If
    End Sub

    ''' <summary>
    ''' Handling the events of the user control; Step with text result
    ''' </summary>
    ''' <param name="stepSequenceNumber"></param>
    ''' <param name="result"></param>
    ''' <remarks></remarks>
    Private Sub ScriptCommamd43StepExecutedWithTextResult(stepSequenceNumber As Integer, result As String) Handles ucScriptCommand43.evntNextWithResult
        Debug.Print(Now & " Script sequencenumber : " & Me.Name & " 4: " & stepSequenceNumber)
        DebugGUILogger.Debug(Me.Name & "Event Next With result Script sequencenumber : " & Me.Name & " 4: " & stepSequenceNumber) 'MOD 88
        ScriptCommandStepStoreRemark()
        InspectionActivityControl.InspectionStepComplete(New InspectionStepResult.InspectionStepResultText(stepSequenceNumber, result))
    End Sub


    ''' <summary>
    ''' Store remark scriptcommand 42
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub ScriptCommandStepStoreRemark()
        If ucScriptCommand42 IsNot Nothing Then
            Wait(100)
            DebugGUILogger.Debug(Me.Name & "Event Next With result Script sequencenumber : " & Me.Name & " 5: ") 'MOD 88
            InspectionActivityControl.StoreRemark(New InspectionStepResult.InspectionStepResultText(ucScriptCommand42._scriptcommand42.SequenceNumber, ucScriptCommand42.RemarkValue))
            'Wait; If no wait an error on saving the results can occur
            Wait(100)
        End If
    End Sub

#End Region
#Region "Handling events from form UcScriptCommand5X"
    ''' <summary>
    ''' THe user has restarted the measurement. Reload form information and chart. Trigger event to InspectionActivityControl.StartContinuousMeasurement
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub UUcScriptCommand5X_RestartMeasurement() Handles ucScriptCommand5X.evntRestartMeasurement
        ucScriptCommand5X.ReloadInformation()
        AttachEventsMeasurements()
        InspectionActivityControl.StartContinuousMeasurement()
    End Sub
    ''' <summary>
    ''' The user has stop the measurement; The event InspectionActivityControl.StopContinuousMeasurement is triggered
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub UcScriptCommand5X_StopMeasurement() Handles ucScriptCommand5X.evntStopMeasurement
        InspectionActivityControl.StopContinuousMeasurement()
    End Sub
#End Region
#End Region

#Region "Initialization proces PLEXOR and manometers; Events and user interfaces"
#Region "Event Handling Initialization PLEXOR And Manometers"
    ''' <summary>
    ''' Handles the event InitializationStepStarted of the InspectionActivityControl control
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="eventArgs"></param>
    ''' <remarks></remarks>
    Sub InspectionActivityControl_InitializationStepStarted(ByVal sender As Object, ByVal eventArgs As System.EventArgs)
        Dim startEventArgs As StartInitializationStepEventArgs = TryCast(eventArgs, StartInitializationStepEventArgs)
        Debug.Print(Now & " Step started : " & Me.Name & ": " & startEventArgs.StepId)
        Invoke(New Action(Of String)(AddressOf InvokeInitializationStart), startEventArgs.StepId)
    End Sub
    ''' <summary>
    ''' Handles the event InitializationStepFinished ot the InspectionActivityControl control
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="eventArgs"></param>
    ''' <remarks></remarks>
    Sub InspectionActivityControl_InitializationStepFinished(ByVal sender As Object, ByVal eventArgs As System.EventArgs)
        Dim finishEventArgs As FinishInitializationStepEventArgs = TryCast(eventArgs, FinishInitializationStepEventArgs)
        Debug.Print(Now & " Step finished : " & Me.Name & ": " & finishEventArgs.Message & " " & finishEventArgs.ErrorCode.ToString)
        BeginInvoke(New Action(Of String, Model.InitializationStepResult, String, Integer)(AddressOf InvokeInitializationStep), finishEventArgs.StepId, finishEventArgs.Result, finishEventArgs.Message, finishEventArgs.ErrorCode)
    End Sub
    ''' <summary>
    ''' Handles the event InitializationFinished of the InspectionActivityControl control
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="eventArgs"></param>
    ''' <remarks></remarks>
    Sub InspectionActivityControl_InitializationFinished(ByVal sender As Object, ByVal eventArgs As System.EventArgs)
        Dim finishEventArgs As FinishInitializationEventArgs = TryCast(eventArgs, FinishInitializationEventArgs)
        Debug.Print(Now & " Init finished : " & Me.Name & ": " & finishEventArgs.Result)
        BeginInvoke(New Action(Of Model.InitializationResult, Integer)(AddressOf InvokeItializationFinished), finishEventArgs.Result, finishEventArgs.ErrorCode)

    End Sub
    ''' <summary>
    ''' Handles the event InspectionActivityControl.InspectionError
    ''' This Event can be raised during initialisation or during the inspection
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="eventArgs"></param>
    ''' <remarks></remarks>
    Sub InspectionActivityControl_InspectionError(ByVal sender As Object, ByVal eventArgs As System.EventArgs)
        Dim errorEventArgs As InspectionErrorEventArgs = TryCast(eventArgs, InspectionErrorEventArgs)
        Debug.Print(Now & " InspectionError : " & Me.Name & ": " & errorEventArgs.ErrorCode)
        BeginInvoke(New Action(Of Integer)(AddressOf InvokeInspectionError), errorEventArgs.ErrorCode)
    End Sub

    '' MOD 75
    ''' <summary>
    ''' Handles the event DeviceUnPairedFinished of the InitializationActivityControl control
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="eventArgs"></param>
    ''' <remarks></remarks>
    Sub InspectionActivityControl_DeviceUnPairedFinished(ByVal sender As Object, ByVal eventArgs As System.EventArgs)
        Dim deviceUnEventArgs As DeviceUnPairedEventArgs = TryCast(eventArgs, DeviceUnPairedEventArgs)
        Debug.Print(Now & " Device unpaired finished " & Me.Name)
        BeginInvoke(New Action(AddressOf StartInspection))
    End Sub

    'MOD 82
    ''' <summary>
    ''' MOD 82
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="eventArgs"></param>
    ''' <remarks></remarks>
    Sub InspectionActivityControl_UiRequest(ByVal sender As Object, ByVal eventArgs As System.EventArgs)
        ''   Dim startEventArgs As StartInitializationStepEventArgs = TryCast(eventArgs, StartInitializationStepEventArgs)
        Debug.Print(Now & " UI reguest : " & Me.Name)
        BeginInvoke(New Action(AddressOf InvokeInitializationUiRequest))
    End Sub

#End Region
#Region "User control Interfacing for Initialzation PLEXOR and Manometers"
    ''' <summary>
    ''' Handles invoke of InspectionActivityControl_InitializationStepFinished.
    ''' Updates the information about the initialization step.
    ''' </summary>
    ''' <param name="stepId"></param>
    ''' <param name="result"></param>
    ''' <param name="message"></param>
    ''' <param name="errorcode"></param>
    ''' <remarks></remarks>
    Public Sub InvokeInitializationStep(ByVal stepId As String, ByVal result As Model.InitializationStepResult, ByVal message As String, ByVal errorCode As Integer)
        Dim stepInitializationUpdate As Initialization.Model.InitializationStepModel = New Initialization.Model.InitializationStepModel(plexorInitStepNumber, stepId, "", result, message, errorCode)
        Debug.Print(Now & " InvokeInitializationStep : " & Me.Name & ": " & stepInitializationUpdate.StepId & "," & stepInitializationUpdate.StepResult.ToString)
        ucInitialization.InitializationUpdateStep(stepInitializationUpdate)

    End Sub
    ''' <summary>
    ''' Handles invoke of InspectionActivityControl_InitializationStepStarted. Initialization manometer started
    ''' </summary>
    ''' <param name="stepId"></param>
    ''' <remarks></remarks>
    Public Sub InvokeInitializationStart(ByVal stepId As String)
        If plexorInitStepNumber = 0 Then
            Debug.Print(Now & " InvokeInitializationStart: " & Me.Name & ": " & stepId)
            manometerInitializationEnabled = True
            ' Me.rButStop.Visible = False
            Me.ucInitialization = New KAM.INSPECTOR.PLEXOR.usctrl_Initialization

            'MOD 07
            ucInitialization.NewPlexorDevice = _NewPlexorDevice
            _NewPlexorDevice = False
            Dim currentDeviceAddress As String = ModuleSettings.SettingFile.GetSetting(GsSectionPlexor, GsSettingPLexorSelected)
            currentDeviceAddress = currentDeviceAddress.Replace("(", "")
            currentDeviceAddress = currentDeviceAddress.Replace(")", "")

            Try
                Dim inspectionProcedureEntity As PlexorInformation = PlexorInformationManager.PlexorsInformation.[Single](Function(ip) ip.BlueToothAddress.Equals(currentDeviceAddress, StringComparison.OrdinalIgnoreCase))
                ucInitialization.PlexorCalibarionDate = inspectionProcedureEntity.CalibrationDate
            Catch ex As Exception
            End Try

            LoadUserControl(rdPanelInspection, ucInitialization, False)
        End If
        plexorInitStepNumber += 1
        Dim stepInitializationUpdate As Initialization.Model.InitializationStepModel = New Initialization.Model.InitializationStepModel(plexorInitStepNumber, stepId, "")
        ucInitialization.InitilizationNewStep(stepInitializationUpdate)
    End Sub
    ''' <summary>
    ''' Handles invoke of InspectionActivityControl_InitializationFinished
    ''' The initialization is finished
    ''' </summary>
    ''' <param name="result"></param>
    ''' <param name="errorCode"></param>
    ''' <remarks></remarks>
    Public Sub InvokeItializationFinished(result As Model.InitializationResult, errorCode As Integer)
        _manometerTh1Unit = ucInitialization.ManometerTH1unit
        _manometerTh2Unit = ucInitialization.ManometerTH2unit
        ucInitialization.InitializationFinished(result.ToString)
        manometerInitializationEnabled = False
        'rButStop.Visible = True
        Debug.Print(Now & " :" & Me.Name & " Initialization finished" & " ErrorCode:" & errorCode.ToString & "  Resultcode:" & result.ToString)
    End Sub
    ''' <summary>
    ''' Handles invoke of InspectionActivityControl_InspectionError
    ''' 
    ''' </summary>
    ''' <param name="errorCode"></param>
    ''' <remarks></remarks>
    Public Sub InvokeInspectionError(errorCode As Integer)
        If ucScriptCommand5X IsNot Nothing Then
            DetachEventsMeasurements()
            ucScriptCommand5X.InspectionError()
        End If

        Debug.Print(Now & " :" & Me.Name & " InvokeInspectionError" & errorCode)
        WiManometerErrorHandling.ErrorCodeHandling(errorCode)
    End Sub

    'MOD 82
    Public Sub InvokeInitializationUiRequest()
        Dim msgboxAnswer
        msgboxAnswer = MsgBox(InspectionProcedureResx.str_InitializationUiRequest, MsgBoxStyle.Question + vbAbortRetryIgnore, QlmProductName)
        Select Case msgboxAnswer
            Case vbAbort
                InspectionActivityControl.SetUiResponse(UiResponse.No)
            Case vbRetry
                InspectionActivityControl.SetUiResponse(UiResponse.Recheck)
            Case vbIgnore
                InspectionActivityControl.SetUiResponse(UiResponse.Yes)
        End Select

        ' Dim stepInitializationUpdate As Initialization.Model.InitializationStepModel = New Initialization.Model.InitializationStepModel(stepNumber, stepId, "", result, message, errorCode)
        '  ucInitialization.InitializationUpdateStep(stepInitializationUpdate)

    End Sub

#End Region
#Region "Measurement Data Handling"
    ''' <summary>
    ''' Handles the event InspectionActivityControl.MeasurementsReceived
    ''' Receiving of measurement (real time) data
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="eventArgs"></param>
    ''' <remarks></remarks>
    Sub InspectionActivityControl_MeasurementsReceived(ByVal sender As Object, ByVal eventArgs As System.EventArgs)
        Dim measurementDataEventArgs As MeasurementEventArgs = TryCast(eventArgs, MeasurementEventArgs)
        BeginInvoke(New Action(Of Global.Inspector.BusinessLogic.Interfaces.Events.MeasurementEventArgs)(AddressOf InvokeMeasurementsReceived), measurementDataEventArgs)
    End Sub
    ''' <summary>
    ''' Handles the event InspectionActivityControl.MeasurementsCompleted
    ''' The measurement is completed
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="eventArgs"></param>
    ''' <remarks></remarks>
    Sub InspectionActivityControl_MeasurementsCompleted(ByVal sender As Object, ByVal eventArgs As System.EventArgs)
        BeginInvoke(New Action(AddressOf InvokeMeasurementsCompleted))
    End Sub
    ''' <summary>
    ''' Handles the event InspectionActivityControl.ExtraMeasurementStarted
    ''' An eextra measurement period is started
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="eventArgs"></param>
    ''' <remarks></remarks>
    Sub InspectionActivityControl_ExtraMeasurementStarted(ByVal sender As Object, ByVal eventArgs As System.EventArgs)
        BeginInvoke(New Action(AddressOf InvokeExtraMeasurementStarted))
    End Sub
    ''' <summary>
    ''' Handles the event InspectionActivityControl.MeasurementResult
    ''' The measurement result is collected. This is the result saved to the result.xml file
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="eventArgs"></param>
    ''' <remarks></remarks>
    Sub InspectionActivityControl_MeasurementResult(ByVal sender As Object, ByVal eventArgs As System.EventArgs)
        Dim measurementResultEventArgs As MeasurementResultEventArgs = TryCast(eventArgs, MeasurementResultEventArgs)
        BeginInvoke(New Action(Of Global.Inspector.BusinessLogic.Interfaces.Events.MeasurementResultEventArgs)(AddressOf InvokeMeasurementResult), measurementResultEventArgs)
    End Sub

    ''' <summary>
    ''' Hndles the event continous measurement
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="eventArgs"></param>
    ''' <remarks></remarks>
    Sub InspectionActivityControl_ContinuousMeasurementStarted(ByVal sender As Object, ByVal eventArgs As System.EventArgs)
        Debug.Print(Now & ": continousstarted")

        BeginInvoke(New Action(AddressOf InvokeContinuousMeasurementStarted))
    End Sub
    ''' <summary>
    ''' Handles invoke InspectionActivityControl_MeasurementsCompleted
    ''' </summary> 
    ''' <remarks></remarks>
    Public Sub InvokeMeasurementsCompleted()
        Debug.Print(Now & " :" & Me.Name & " : completed")
        DetachEventsMeasurements()
        ucScriptCommand5X.MeasurementCompleted()
    End Sub
    ''' <summary>
    ''' Handles invoke InspectionActivityControl_ContinuousMeasurementStarted
    ''' </summary>
    ''' <param name="measurementvalues"></param>
    ''' <remarks></remarks>
    Public Sub InvokeMeasurementsReceived(measurementvalues As Global.Inspector.BusinessLogic.Interfaces.Events.MeasurementEventArgs)
        'MOD 26
        If firstStart = True Then
            firstStart = False
            ucScriptCommand5X.IntializeStartMeasurement()
        End If

        ucScriptCommand5X.UpdateMeasureValues(measurementvalues)
    End Sub
    ''' <summary>
    '''  Handles invoke InspectionActivityControl_MeasurementsReceived
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub InvokeContinuousMeasurementStarted()
        'TO DO Should be implemented by SIOUX
        'BUG: MOD 26 Is now started also during the retry in case no retrun on meas pres ucScriptCommand5X.IntializeStartMeasurement()
        'Work around see mod 26
        'MOD 77 20170424 
        Debug.Print(Now & " :" & Me.Name & " : Attach event MeasurementsReceived")
        AddHandler InspectionActivityControl.MeasurementsReceived, AddressOf InspectionActivityControl_MeasurementsReceived
    End Sub


    ''' <summary>
    ''' Handles invoke InspectionActivityControl_MeasurementResult
    ''' The actual result
    ''' </summary>
    ''' <param name="measurementResult"></param>
    ''' <remarks></remarks>
    Public Sub InvokeMeasurementResult(measurementResult As Global.Inspector.BusinessLogic.Interfaces.Events.MeasurementResultEventArgs)
        ucScriptCommand5X.MeasurementResultComplete(measurementResult)
    End Sub
    ''' <summary>
    ''' The extra measurement period is started. 
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub InvokeExtraMeasurementStarted()
        Debug.Print(Now & " :" & Me.Name & " : Extra measurement period started")
        ucScriptCommand5X.StartExtraMeasurement()
    End Sub

#End Region
#End Region

#Region "User control Interfacing"
    ''' <summary>
    ''' Load the user control dynamic into a panel
    ''' </summary>
    ''' <param name="panel"></param>
    ''' <param name="UserControl"></param>
    ''' <remarks></remarks>
    Private Sub LoadUserControl(ByVal panel As RadPanel, userControl As UserControl, ByVal isSelect As Boolean)
        'Loading the usercontrol into the toolwindow
        If panel.Controls.Count > 0 Then
            'If any usercontrol already exists in the panel. Dispose it.
            For i As Integer = 1 To panel.Controls.Count
                Dim control As Control = panel.Controls(i - 1)
                If control.Name = ucInspectionInformation.Name Then lbloadUCInspectionInformation = False
                If control IsNot Nothing Then
                    control.Dispose()
                End If
            Next
        End If
        userControl.Hide()
        panel.Controls.Add(userControl)
        If userControl.Name = ucInspectionInformation.Name Then lbloadUCInspectionInformation = True
        userControl.Dock = DockStyle.Fill
        userControl.Left = 0
        userControl.Top = 0
        userControl.Show()
        '  If isSelect = True Then userControl.Select()
    End Sub
    ''' <summary>
    ''' Dispose the load user control
    ''' </summary>
    ''' <param name="panel"></param>
    ''' <remarks></remarks>
    Private Sub UnloadUserControl(ByVal panel As RadPanel)
        If panel.Controls.Count > 0 Then
            'If any usercontrol already exists in the toolwindow. Dispose it.
            For i As Integer = 1 To panel.Controls.Count
                Dim control As Control = panel.Controls(i - 1)
                If control.Name = ucInspectionInformation.Name Then lbloadUCInspectionInformation = False
                If control IsNot Nothing Then
                    control.Dispose()
                End If
            Next
        End If
    End Sub


#End Region
#End Region

    'Handling of error codes during initialization of during execute an inspection
#Region "WiManometerErrorHandling; KAM.INSPECTOR.PLEXOR clsErrorInitHandling"
    ''' <summary>
    ''' Handling of a inspection finished
    ''' </summary>
    ''' <param name="errorcode"></param>
    ''' <remarks></remarks>
    Sub WiManometerErrorHandling_EvntInspectionFinished(errorcode As Integer) Handles WiManometerErrorHandling.EvntInspectionFinished
        'The inspection has finished
        'In case caused by an without an error display the PRS window
        If lbShowPrsGclFormAfterInspection = True Then
            DetachEventsMeasurements()
            DetachEvents()
            'Unload the inspection window

            UnloadUserControl(Me.rdPanelInspection)

            rButStop.Visible = False
            rButStop.Enabled = False
            rButStart.Visible = True
            rButStart.Enabled = True

            'MsgBox(My.Resources.InspectionProcedureResx.str_InspectionFinished, MsgBoxStyle.Information, QlmProductName)

            'MOD 21
            'Check if no boundaries are exceeded
            'MOD 30
            Dim result
            If Not IsNothing(sectionSelectionvalueOutOfBoundaries) Then
                result = sectionSelectionvalueOutOfBoundaries.SectionSelectionEntities.Find(Function(c) c.IsSelected = True)
            Else
                result = Nothing
            End If

            If IsNothing(result) Then
                RaiseEvent evntInspectionFinished()
                'Raise event to display the PRS form and update status informations
                'This wil unload the remark window
                RaiseEvent evntInspectionCompleted(_prsName, _gclName)
            Else
                Dim ok
                ok = MsgBox(My.Resources.InspectionProcedureResx.str_RestartBoundariesInspection, MsgBoxStyle.Information + MsgBoxStyle.YesNo, QlmProductName)
                'Restart
                If ok = vbYes Then
                    Me.rButStartBoundInspect.Visible = True
                    Me.rButStartBoundInspect.Enabled = True
                    Me.rButStopBoundInspect.Visible = True
                    Me.rButStopBoundInspect.Enabled = True
                    Me.rButStart.Visible = False
                    Me.rButStart.Enabled = False
                    'Load user control with restart options
                    'Load user control with inspectionInformation
                    ucInspectionInformation = New KAM.INSPECTOR.IP.usctrl_InspectionInformation
                    LoadUserControl(rdPanelInspection, ucInspectionInformation, False)
                    ucInspectionInformation.UpdateIPSectionInformationByBoundaries(sectionSelectionvalueOutOfBoundaries)
                Else
                    'No restart
                    RaiseEvent evntInspectionFinished()
                    'Raise event to display the PRS form and update status informations
                    'This wil unload the remark window
                    RaiseEvent evntInspectionCompleted(_prsName, _gclName)
                End If
            End If
        Else
            'In case of an error; Display the selection inspection window
        End If
    End Sub


    'MOD 64 
    ''' <summary>
    ''' Handling of error during inspection procedure
    ''' </summary>
    ''' <param name="errorcode"></param>
    ''' <remarks></remarks>
    Sub WiManometerErrorHandling_EvntDoNotRestartInspection(errorcode As Integer) Handles WiManometerErrorHandling.EvntDoNotRestartInspection
        Dim OK
        OK = MsgBox(infraResources.getErrorTranslation(errorcode) & vbCrLf, MsgBoxStyle.Critical, QlmProductName & " " & InspectionProcedureResx.str_Errorcode & " : " & errorcode)
        'stops the initilisation process
        lbShowPrsGclFormAfterInspection = False
        InspectionActivityControl.Abort()
        RaiseEvent evntInspectionFinished()
        rButStop.Visible = False
        rButStop.Enabled = False
        rButStart.Visible = True
        rButStart.Enabled = True
    End Sub
    ''' <summary>
    ''' Handling of error during inspection procedure
    ''' </summary>
    ''' <param name="errorcode"></param>
    ''' <remarks></remarks>
    Sub WiManometerErrorHandling_EvntRestartInspection(errorcode As Integer) Handles WiManometerErrorHandling.EvntRestartInspection
        Dim ok
        'Check if error was caused by the intialization function
        'Done by checking if window is present
        ok = MsgBox(infraResources.getErrorTranslation(errorcode) & vbCrLf & InspectionProcedureResx.str_Do_you_wich_to_restart_the, MsgBoxStyle.YesNo Or MsgBoxStyle.Critical, QlmProductName & " " & InspectionProcedureResx.str_Errorcode & " : " & errorcode)

        Select Case ok
            Case vbYes
                'Start the reinitialisation proces
                plexorInitStepNumber = 0
                Try

                    InspectionActivityControl.Retry()
                Catch ex As Exception
                    MsgBox("Unable to start inspection" & vbCrLf & ex.Message, vbCritical)
                    'stops the initilisation process
                    lbShowPrsGclFormAfterInspection = True
                    InspectionActivityControl.Abort()
                    RaiseEvent evntInspectionFinished()
                    rButStop.Visible = False
                    rButStop.Enabled = False
                    rButStart.Visible = True
                    rButStart.Enabled = True
                End Try
            Case Else
                If manometerInitializationEnabled = True Then
                    'stops the initilisation process

                    'MOD 74
                    ucInitialization.InitializationFinished("")

                    lbShowPrsGclFormAfterInspection = False
                    InspectionActivityControl.Abort()
                    RaiseEvent evntInspectionFinished()
                    rButStop.Visible = False
                    rButStop.Enabled = False
                    rButStart.Visible = True
                    rButStart.Enabled = True
                Else
                    lbShowPrsGclFormAfterInspection = True
                    InspectionActivityControl.Abort()
                End If
        End Select


        '' ''Dim ok
        '' ''Dim tmpErrorCode As Integer
        '' ''Dim tmpInitProcess As Boolean = False
        ' '' ''Check if error was caused by the intialization function
        ' '' ''Done by checking if window is present
        '' ''Try
        '' ''    If ucInitialization IsNot Nothing Then
        '' ''        ucInitialization.InitializationFinished(errorcode.ToString)
        '' ''        tmpErrorCode = ucInitialization.InitErrorCode
        '' ''        tmpInitProcess = True
        '' ''    End If
        '' ''Catch ex As Exception
        '' ''End Try

        ' '' ''MOD 64 problem with restart init DetachEventsMeasurements()
        '' ''If tmpInitProcess = True Then
        '' ''    'MOD 64 add replace tmpErrorCode by errorcode. Mantis ID 27971
        '' ''    If tmpErrorCode = "0" Then tmpErrorCode = errorcode
        '' ''    ok = MsgBox(infraResources.getErrorTranslation(tmpErrorCode) & vbCrLf & InspectionProcedureResx.str_Do_you_wich_to_restart_the, MsgBoxStyle.YesNo Or MsgBoxStyle.Critical, QlmProductName & " " & InspectionProcedureResx.str_Errorcode & " : " & errorcode)
        '' ''Else
        '' ''    ok = MsgBox(infraResources.getErrorTranslation(errorcode) & vbCrLf & InspectionProcedureResx.str_Do_you_wich_to_restart_the, MsgBoxStyle.YesNo Or MsgBoxStyle.Critical, QlmProductName & " " & InspectionProcedureResx.str_Errorcode & " : " & errorcode)
        '' ''End If
        '' ''Select Case ok
        '' ''    Case vbYes
        '' ''        'Start the reinitialisation proces
        '' ''        plexorInitStepNumber = 0
        '' ''        Try
        '' ''            InspectionActivityControl.Retry()
        '' ''        Catch ex As Exception
        '' ''            MsgBox("Unable to start inspection" & vbCrLf & ex.Message, vbCritical)
        '' ''            'stops the initilisation process
        '' ''            lbShowPrsGclFormAfterInspection = True
        '' ''            InspectionActivityControl.Abort()
        '' ''            RaiseEvent evntInspectionFinished()
        '' ''            rButStop.Visible = False
        '' ''            rButStop.Enabled = False
        '' ''            rButStart.Visible = True
        '' ''            rButStart.Enabled = True
        '' ''        End Try
        '' ''    Case Else
        '' ''        'MOD 17
        '' ''        If tmpInitProcess = True Then
        '' ''            'stops the initilisation process
        '' ''            lbShowPrsGclFormAfterInspection = False
        '' ''            InspectionActivityControl.Abort()
        '' ''            RaiseEvent evntInspectionFinished()
        '' ''            rButStop.Visible = False
        '' ''            rButStop.Enabled = False
        '' ''            rButStart.Visible = True
        '' ''            rButStart.Enabled = True
        '' ''        Else
        '' ''            lbShowPrsGclFormAfterInspection = True
        '' ''            InspectionActivityControl.Abort()
        '' ''        End If
        '' ''End Select
    End Sub
#End Region

    'MOD 64
    ''' <summary>
    ''' Handles the event SafetyValueTriggered of the InspectionActivityControl control
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="eventArgs"></param>
    ''' <remarks></remarks>
    Sub InspectionActivityControl_SafetyValueTriggered(ByVal sender As Object, ByVal eventArgs As System.EventArgs)
        Dim startEventArgs As SafetyValueTriggeredEventArgs = TryCast(eventArgs, SafetyValueTriggeredEventArgs)
        Debug.Print(Now & " IO status : " & Me.Name & ": " & startEventArgs.IoStatus)
        Debug.Print(Now & " Measurement value : " & Me.Name & ": " & startEventArgs.MeasurementValue)
        Invoke(New Action(Of SafetyValueTriggeredEventArgs)(AddressOf InvokeSafetyValueTriggered), startEventArgs)

    End Sub

    ''' <summary>
    ''' Handles invoke InspectionActivityControl_SafetyValueTriggered
    ''' IO val sensor trigger
    ''' </summary>
    ''' <param name="IOStatusTrigger"></param>
    ''' <remarks></remarks>
    Public Sub InvokeSafetyValueTriggered(IOStatusTrigger As Global.Inspector.BusinessLogic.Interfaces.Events.SafetyValueTriggeredEventArgs)
        ucScriptCommand5X.MeasurementIoTriggerd(IOStatusTrigger)
    End Sub




#Region "RadDock Handling"
    ''' <summary>
    ''' Handling of context menu of rad dock Documentwindows and tool windows. Disable menu-items Close
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub menuService_ContextMenuDisplaying(sender As Object, e As ContextMenuDisplayingEventArgs)
        'remove the "Close" menu items
        For i As Integer = 0 To e.MenuItems.Count - 1
            Dim menuItem As RadMenuItemBase = e.MenuItems(i)
            If menuItem.Name = "CloseWindow" OrElse menuItem.Name = "CloseAllButThis" OrElse menuItem.Name = "CloseAll" OrElse menuItem.Name = "Hidden" OrElse TypeOf menuItem Is RadMenuSeparatorItem Then
                menuItem.Visibility = Telerik.WinControls.ElementVisibility.Collapsed
            End If
        Next
    End Sub

#End Region


    'TO DO 
    Private Sub RadButton1_Click(sender As Object, e As EventArgs)
        Dim ReportInspectionResult1 As New InspectionReportingResults.ReportInspectionResult
        ReportInspectionResult1 = InspectionResultReader.LookupLastResult(_prsName, _gclName)

        Dim ReportInspectionResult2 As New InspectionReportingResults.ReportInspectionResult
        ReportInspectionResult2 = InspectionResultReader.LookupPreviousToLastResult(_prsName, _gclName)


        Dim lastResult2 As New InspectionReportingResults.ReportResult
        lastResult2 = InspectionResultReader.LookupLastReportResult(_prsName, _gclName, "", "", 1)

        Dim lastResult3 As New InspectionReportingResults.ReportResult
        lastResult3 = InspectionResultReader.LookupPreviousToLastReportResult(_prsName, _gclName, "", "", 1)



    End Sub



End Class
