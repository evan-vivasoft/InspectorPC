Imports Inspector.Model
Imports System.Drawing
Imports KAM.INSPECTOR.IP.My.Resources
Imports KAM.INSPECTOR.Infra
Imports KAM.INSPECTOR.info.modLicenseInfo
Imports System.Media
Imports System.Windows.Forms

Public Class usctrl_scriptCommand5x
#Region "Class members"
    Shadows Event evntNext(stepSequenceNumber As Integer)
    Private _scriptcommand5x As New InspectionProcedure.ScriptCommand5X
    Private _lastResult As InspectionReportingResults.ReportResult
    Private _manometermUnit As String = "'"
    Private _playSound As Boolean = True 'MOD 23

    Private textProcessBar As String = ""
    Public Event evntStopMeasurement()
    Public Event evntStartMeasurement()
    Public Event evntRestartMeasurement()

    Private _measurementValueOutOfLimits As Boolean = False
    'MOD 89 Private _measurementValueHasResult As Boolean = False
#End Region

#Region "Constructor'"
    ''' <summary>
    ''' Initialize form
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()
        ' This call is required by the designer.
        InitializeComponent()
        'Capture clicking the the next button of the main screen
    End Sub
#End Region
#Region "Information loading"

    ''' <summary>
    ''' Load the information to the form; Set items correct
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub LoadInformation()
        initializeTimer()
        'Measument Period is defined

        If _scriptcommand5x.MeasurementPeriod > 0 Then
            'Set timer
            'Display progress bar
            'Hide stop button
            btnStop.Visible = False
            btnNext.Visible = False
            btnRestart.Visible = False
            rdProgressBar.Visible = True
            rdProgressBar.Value1 = 0
            rdProgressBar.Maximum = _scriptcommand5x.MeasurementPeriod
            textProcessBar = ""
            rdProgressBar.Text = My.Resources.InspectionProcedureResx.str_WaitingForMeasurementToStart 'MOD 26 rdProgressBar.Maximum - rdProgressBar.Value1.ToString & " " & My.Resources.InspectionProcedureResx.str_seconds_remaining
            rdProgressBar.BackColor = Color.GreenYellow
            'MOD 26 timerProgressBar.Start()
            'MOD 23
            If _scriptcommand5x.ExtraMeasurementPeriod > 0 Then _playSound = False Else _playSound = True

        Else
            'No Measurement period
            'Hide Progress bar
            'Show stop button
            _playSound = False
            rdProgressBar.Visible = False
            btnStop.Visible = False
            btnNext.Visible = False
            btnRestart.Visible = False
        End If

        Usctrl_ChartResults.MeasurementResultComplete = False
        btnStop.Top = btnNext.Top
        btnStop.Left = btnNext.Left
        btnStop.Width = btnNext.Width
        btnRestart.Top = btnNext.Top

        'Fill the instruction
        If _scriptcommand5x.Instruction <> "" Then
            'MOD 06
            rdrtbInstruction.IsReadOnly = True
            rdrtbInstruction.ChangeFontSize(ModuleSettings.SettingFile.GetSetting(GsSectionText, GsSettingTextSize))
            rdrtbInstruction.Insert(_scriptcommand5x.Instruction)
            'MOD 06 rdtbInstruction.Text = _scriptcommand5x.Instruction
        End If
        Usctrl_ChartResults.Scriptcommand5x = _scriptcommand5x
        Usctrl_ChartResults.lastResult = _lastResult
        Usctrl_ChartResults.ManometerUnit = _manometermUnit
        Usctrl_ChartResults.LoadSettings()
        Me.Focus()
    End Sub

    'MOD 26
    ''' <summary>
    ''' Starts the measurement
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub IntializeStartMeasurement()
        rdProgressBar.Text = rdProgressBar.Maximum - rdProgressBar.Value1.ToString & " " & My.Resources.InspectionProcedureResx.str_seconds_remaining
        timerProgressBar.Start()
        Usctrl_ChartResults.IntializeStartMeasurement()
    End Sub


    ''' <summary>
    ''' Load the information after restarting a measurement
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub ReloadInformation()
        Usctrl_ChartResults.MeasurementResultComplete = False
        Usctrl_ChartResults.ReloadChart()
        If _scriptcommand5x.MeasurementPeriod > 0 Then
            btnStop.Visible = False
            btnNext.Visible = False
            btnRestart.Visible = False
            rdProgressBar.Value1 = 0
            rdProgressBar.Maximum = _scriptcommand5x.MeasurementPeriod
            textProcessBar = ""
            rdProgressBar.Text = rdProgressBar.Maximum - rdProgressBar.Value1.ToString & " " & My.Resources.InspectionProcedureResx.str_seconds_remaining
            timerProgressBar.Start()
        Else
            btnStop.Visible = True
            btnNext.Visible = False
            btnRestart.Visible = False
        End If
        Me.Focus()
    End Sub
#End Region


#Region "Measurement Handling"
    ''' <summary>
    ''' Update the measurement values
    ''' </summary>
    ''' <param name="measurementvalues"></param>
    ''' <remarks></remarks>
    Public Sub UpdateMeasureValues(measurementvalues As Global.Inspector.BusinessLogic.Interfaces.Events.MeasurementEventArgs)
        'MOD 93
        If btnStop.Visible = False Then
            If _scriptcommand5x.MeasurementPeriod = 0 Then btnStop.Visible = True
        End If
        Usctrl_ChartResults.UpdateUI(measurementvalues)
    End Sub
    ''' <summary>
    ''' An extra measurement period is active. 
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub StartExtraMeasurement()
        If _scriptcommand5x.ExtraMeasurementPeriod > 0 Then
            'MOD 23
            _playSound = True
            timerProgressBar.Stop()
            btnStop.Visible = False
            rdProgressBar.Visible = True
            rdProgressBar.Value1 = 0
            rdProgressBar.Maximum = _scriptcommand5x.ExtraMeasurementPeriod
            textProcessBar = My.Resources.InspectionProcedureResx.str_Extra_measurement_Period
            rdProgressBar.Text = rdProgressBar.Maximum - rdProgressBar.Value1.ToString & " " & My.Resources.InspectionProcedureResx.str_seconds_remaining
            rdProgressBar.BackColor = Color.GreenYellow
            timerProgressBar.Start()
        End If
    End Sub
    ''' <summary>
    ''' The measurement result is completed
    ''' In case of an extra measurement time. This information is not updated
    ''' </summary>
    ''' <param name="measurementResult"></param>
    ''' <remarks></remarks>
    Public Sub MeasurementResultComplete(measurementResult As Global.Inspector.BusinessLogic.Interfaces.Events.MeasurementResultEventArgs)
        'Set the value of Measurement value exceeds it's limits

        'MOD 89 _measurementValueHasResult = 
        _measurementValueOutOfLimits = measurementResult.MeasurementValueOutOfLimits

        If measurementResult.HasMeasurementValue = False Then _measurementValueOutOfLimits = True

        'When the extra measurement time can be started. This should not be displayed
        'Stop updating the value in the chart
        Usctrl_ChartResults.MeasurementResultComplete = True
        Usctrl_ChartResults.MeasurementHasValue = measurementResult.HasMeasurementValue
        Debug.Print(Now & " Value received: " & measurementResult.MeasurementValue)

    End Sub

    'MOD 59
    ''' <summary>
    ''' Handling event of IO status Trigger
    ''' </summary>
    ''' <param name="IOStatusTrigger">Value of IO status</param>
    ''' <remarks></remarks>
    Public Sub MeasurementIoTriggerd(iOStatusTrigger As Global.Inspector.BusinessLogic.Interfaces.Events.SafetyValueTriggeredEventArgs)
        btnStop.Enabled = False
        Debug.Print(Now & " IO triggerd")
        RaiseEvent evntStopMeasurement()
    End Sub
    ''' <summary>
    ''' The inspection of scriptcommand 5x is completed
    ''' Triggerd after expired of extrameasurement time
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub MeasurementCompleted()
        'MOD 24
        If _scriptcommand5x.ScriptCommand = ScriptCommand5XType.ScriptCommand51 And ModuleSettings.SettingFile.GetSetting(GsSectionGUI, GsSettingGUIShowBtnRestartLeakage) = True Then
            'MOD 69
            btnRestart.Enabled = True
            btnRestart.Visible = True
        End If
        btnNext.Visible = True
        btnStop.Visible = False
        Usctrl_ChartResults.DisplayAllChartdata()
        Usctrl_ChartResults.MeasurementComplete = True
        If _measurementValueOutOfLimits = True Then MsgBox(InspectionProcedureResx.str_OutSideBoundaries, vbExclamation, QlmProductName)
    End Sub
    ''' <summary>
    ''' The inspection is stopped caused by an connection failure
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub InspectionError()
        timerProgressBar.Stop()
        rdProgressBar.Text = InspectionProcedureResx.str_Communication_error
    End Sub


#End Region
#Region "Button Handling"
    ''' <summary>
    ''' The users stops the measurement
    ''' Stop button available if measurement Period  0
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub btnStop_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnStop.Click
        'MOD 60
        btnStop.Enabled = False
        RaiseEvent evntStopMeasurement()
    End Sub
    ''' <summary>
    ''' The user will restarts the measurement by hand
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub btnRestart_Click(sender As System.Object, e As System.EventArgs) Handles btnRestart.Click
        'MOD 60
        btnRestart.Enabled = False
        RaiseEvent evntRestartMeasurement()
    End Sub
    ''' <summary>
    ''' Completion of inspection step
    ''' Raise event evntNext with script sequence number
    ''' </summary>
    ''' <remarks></remarks>
    Public Overrides Sub InspectionStepCompleted()
        DebugGUILogger.Debug(Me.Name & " ; step completed ; " & _scriptcommand5x.SequenceNumber) 'MOD 83
        DebugGUILogger.Debug(Me.Name & "; InspectionStepCompleted event clicked")
        'Check if the next button is visable/ enabled to resume
        If btnNext.Visible = False Or btnNext.Enabled = False Then Exit Sub
        'MOD 60
        btnNext.Enabled = False

        'MOD 85
        If _ClickEventOnlyOnce = False Then
            _ClickEventOnlyOnce = True
            DebugGUILogger.Debug(Me.Name & "; InspectionStepCompleted event handeled")

            'Sending the information with the inspection result
            RaiseEvent evntNext(_scriptcommand5x.SequenceNumber)
        End If
    End Sub

    ''' <summary>
    ''' Handling of short cuts keys
    ''' </summary>
    ''' <param name="msg"></param>
    ''' <param name="keyData"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Protected Overrides Function ProcessCmdKey(ByRef msg As Message, _
          ByVal keyData As Keys) As Boolean
        Const WM_KEYDOWN As Integer = &H100
        Const WM_SYSKEYDOWN As Integer = &H104

        If ((msg.Msg = WM_KEYDOWN) Or (msg.Msg = WM_SYSKEYDOWN)) Then
            Select Case (keyData)
                Case Keys.F12
                    Console.WriteLine("F12 Captured")
                    If btnStop.Visible = True Then RaiseEvent evntStopMeasurement()
                    If btnRestart.Visible = True Then RaiseEvent evntRestartMeasurement()

            End Select
        End If
        Return MyBase.ProcessCmdKey(msg, keyData)
    End Function

#End Region
#Region "Timers"
    ''' <summary>
    ''' Timer handling
    ''' Updates the progress bar
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub Timer1_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles timerProgressBar.Tick
        rdProgressBar.Value1 += 1
        If rdProgressBar.Maximum <= rdProgressBar.Value1 Then
            'Stop the timer
            timerProgressBar.Stop()
            rdProgressBar.Text = My.Resources.InspectionProcedureResx.str_Completed
            'MOD 23
            If _playSound = True Then
                Dim player As SoundPlayer = New SoundPlayer(My.Resources.SoundsResx.beep_1)
                player.Play()
            End If
        Else
            rdProgressBar.Text = textProcessBar & " " & rdProgressBar.Maximum - rdProgressBar.Value1.ToString & " " & My.Resources.InspectionProcedureResx.str_seconds_remaining
        End If
    End Sub
    ''' <summary>
    ''' Initialize the timer
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub initializeTimer()

        timerProgressBar.Interval = 1000
    End Sub
#End Region

#Region "Properties"
    ''' <summary>
    ''' Set script command 5x information
    ''' </summary>
    ''' <value></value>
    ''' <remarks></remarks>
    Public WriteOnly Property Scriptcommand5x() As InspectionProcedure.ScriptCommand5X
        Set(ByVal value As InspectionProcedure.ScriptCommand5X)
            _scriptcommand5x = value
        End Set
    End Property
    ''' <summary>
    ''' The last colleted result
    ''' </summary>
    ''' <value></value>
    ''' <remarks></remarks>
    Public WriteOnly Property lastResult As InspectionReportingResults.ReportResult
        Set(value As InspectionReportingResults.ReportResult)
            _lastResult = value
        End Set
    End Property

    ''' <summary>
    ''' Set the unit of manometer 1.
    ''' This is for settings the unit with the measurement data
    ''' </summary>
    ''' <value></value>
    ''' <remarks></remarks>
    Public WriteOnly Property ManometerUnit As String
        Set(value As String)
            _manometermUnit = value
        End Set
    End Property


#End Region




End Class

