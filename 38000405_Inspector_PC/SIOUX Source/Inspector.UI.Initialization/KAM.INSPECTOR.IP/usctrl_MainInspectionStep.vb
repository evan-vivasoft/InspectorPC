'===============================================================================
'Copyright Wigersma 2012
'All rights reserved.
'===============================================================================
Imports Inspector.Model
Imports System.Windows.Forms
Imports KAM.INSPECTOR.Infra
''' <summary>
''' Form to inherit to display script commands
''' </summary>
''' <remarks></remarks>


Public Class usctrl_MainInspectionStep
#Region "Class members"
    Public Event evntNext()
    Private _scriptcommand2 As New InspectionProcedure.ScriptCommand2
    Private _currentStep As Integer = 0
    Private _totalStep As Integer = 0
    Private _showNextButtonAtStartup As Boolean = False
    'MOD 85
    Public _ClickOnlyOnce As Boolean = False
    Public _ClickEventOnlyOnce As Boolean = False
#End Region

#Region "Constructor"
    ''' <summary>
    ''' Initialize form
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()
        ' This call is required by the designer.
        InitializeComponent()


        ' Add any initialization after the InitializeComponent() call.
        rdtbSection.TextBoxElement.Border.Visibility = Telerik.WinControls.ElementVisibility.Collapsed
        rdtbSubSection.TextBoxElement.Border.Visibility = Telerik.WinControls.ElementVisibility.Collapsed
    End Sub

    Public Sub FormLoad(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        btnNext.Visible = False
        If ShowNextButtonAtStartup = True Then
            'MOD 97
            TimerBtnNext.Interval = 2000
            TimerBtnNext.Start()
        End If
    End Sub

    ''' <summary>
    ''' Timer handling
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub Timer1_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TimerBtnNext.Tick
        btnNext.Visible = True
        TimerBtnNext.Stop()
    End Sub



#End Region
#Region "Commands"
    ''' <summary>
    ''' Button handling; Raise evntNext
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Overridable Sub btnNext_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnNext.Click
        DebugGUILogger.Debug(Me.Name & "; Button click next") 'mod 83
        'MOD 85
        If _ClickOnlyOnce = False Then
            _ClickOnlyOnce = True
            InspectionStepCompleted()
        End If
    End Sub
    ''' <summary>
    ''' The inspection step is completes
    ''' Event evntNext is raised
    ''' </summary>
    ''' <remarks></remarks>
    Overridable Sub InspectionStepCompleted()
        ''MOD 85 Only raise this event once. Add something to prevent
        DebugGUILogger.Debug(Me.Name & "; Main InspectionStepCompleted event clicked")
        DebugGUILogger.Debug(Me.Name & "; Main Status _ClickEventOnlyOnce" & _ClickEventOnlyOnce)
        DebugGUILogger.Debug(Me.Name & "; Main Status _ClickOnlyOnce" & _ClickOnlyOnce)
        DebugGUILogger.Debug(Me.Name & "; Main Status btnnext" & btnNext.Visible)
        If _ClickEventOnlyOnce = False Then
            _ClickEventOnlyOnce = True
            DebugGUILogger.Debug(Me.Name & "; Main InspectionStepCompleted event handeled")
            RaiseEvent evntNext()
        End If
    End Sub

    ''' <summary>
    ''' Set the script command 2 information to the form
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub LoadinformationSC2()
        rdtbSection.Text = Scriptcommand2.Section
        rdtbSubSection.Text = Scriptcommand2.SubSection
        lblIPStep.Text = My.Resources.InspectionProcedureResx.str_Step & " " & CurrentStep & "/" & TotalStep
        Me.Refresh()
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
                Case Keys.F11
                    Console.WriteLine("F11 Captured")
                    DebugGUILogger.Debug(Me.Name & "F11 Captured") 'MOD 88
                    If btnNext.Visible = True Then InspectionStepCompleted()
            End Select
        End If
        Return MyBase.ProcessCmdKey(msg, keyData)
    End Function

#End Region
#Region "Properties"
    ''' <summary>
    ''' Script command 2 information
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Scriptcommand2() As InspectionProcedure.ScriptCommand2
        Set(ByVal value As InspectionProcedure.ScriptCommand2)
            _scriptcommand2 = value
        End Set
        Get
            Return _scriptcommand2
        End Get
    End Property
    ''' <summary>
    ''' Inspection procedure current step
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property CurrentStep() As Integer
        Set(ByVal value As Integer)
            _currentStep = value
        End Set
        Get
            Return _currentStep
        End Get
    End Property
    ''' <summary>
    ''' Inspection procedure total amount of steps
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property TotalStep() As Integer
        Set(ByVal value As Integer)
            _totalStep = value
        End Set
        Get
            Return _totalStep
        End Get
    End Property

    Public Property ShowNextButtonAtStartup() As Boolean
        Set(ByVal value As Boolean)
            _showNextButtonAtStartup = value
        End Set
        Get
            Return _showNextButtonAtStartup
        End Get
    End Property

#End Region



End Class
