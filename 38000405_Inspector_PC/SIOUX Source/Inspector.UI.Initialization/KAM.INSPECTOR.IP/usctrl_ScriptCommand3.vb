'===============================================================================
'Copyright Wigersma 2012
'All rights reserved.
'===============================================================================

Imports System.Drawing
Imports Inspector.Model
Imports KAM.INSPECTOR.Infra
Imports System.Media
''' <summary>
''' Introduce a pause. The user presses the button Next to resume the inspection procedure
''' 
''' Raise event evntNext when the user presses the next button
''' 
''' xsd:element name="SequenceNumber" type="xsd:long"/>                ; Sequence number in inspection procedure
''' xsd:element name="Text" type="xsd:string"/>                        ; Text to be displayed during the pause
''' xsd:element name="Duration" type="xsd:positiveInteger"/>           ; Duration of the pause. During pause the next button is disabled and a progress bar is displayed
''' 
''' </summary>
''' <remarks></remarks>
Public Class usctrl_scriptCommand3
#Region "Class members"
    Shadows Event evntNext(stepSequenceNumber As Integer)
    Public _scriptcommand3 As New InspectionProcedure.ScriptCommand3
#End Region
#Region "Command"
    ''' <summary>
    ''' Load the information to the form; Set items correct
    ''' The timer for pause is started
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub LoadInformation()
        btnNext.Visible = False
        RadProgressBar1.Maximum = _scriptcommand3.Duration
        RadProgressBar1.Text = RadProgressBar1.Maximum - RadProgressBar1.Value1.ToString & " " & My.Resources.InspectionProcedureResx.str_seconds_remaining

        'MOD 06
        rdrtbInstruction.IsReadOnly = True
        rdrtbInstruction.ChangeFontSize(ModuleSettings.SettingFile.GetSetting(GsSectionText, GsSettingTextSize))
        rdrtbInstruction.Insert(_scriptcommand3.Text)

        RadProgressBar1.BackColor = Color.GreenYellow
        Me.Refresh()
        Timer1.Start()
        rdrtbInstruction.Focus()
    End Sub
    ''' <summary>
    ''' Timer handling
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub Timer1_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Timer1.Tick
        RadProgressBar1.Value1 += 1
        'Stop the timer
        If RadProgressBar1.Maximum <= RadProgressBar1.Value1 Then
            Timer1.Stop()
            btnNext.Visible = True
            RadProgressBar1.Text = My.Resources.InspectionProcedureResx.str_Completed
            'For i As Integer = 1 To 100
            ' Beep()
            ' Next i
            'MOD 23
            Dim player As SoundPlayer = New SoundPlayer(My.Resources.SoundsResx.beep_1)

            player.Play()
        Else
            RadProgressBar1.Text = RadProgressBar1.Maximum - RadProgressBar1.Value1.ToString & " " & My.Resources.InspectionProcedureResx.str_seconds_remaining
        End If
    End Sub
    ''' <summary>
    ''' Set timer interval = 1000
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub scriptCommand3_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        'Setting the timerinterval. Update every 1 second
        Timer1.Interval = 1000
    End Sub
    ''' <summary>
    ''' Completion of inspection step
    ''' Raise event evntNext with script sequence number
    ''' </summary>
    ''' <remarks></remarks>
    Public Overrides Sub InspectionStepCompleted()
        DebugGUILogger.Debug(Me.Name & " ; step completed ; " & _scriptcommand3.SequenceNumber)
        DebugGUILogger.Debug(Me.Name & "; InspectionStepCompleted event clicked")
        'Check if the next button is visable/ enabled to resume
        If btnNext.Visible = False Or btnNext.Enabled = False Then Exit Sub
        'MOD 60
        btnNext.Visible = False

        'MOD 85
        If _ClickEventOnlyOnce = False Then
            _ClickEventOnlyOnce = True
            DebugGUILogger.Debug(Me.Name & "; InspectionStepCompleted event handeled")

            'Sending the information with the inspection result
            RaiseEvent evntNext(_scriptcommand3.SequenceNumber)
        End If
    End Sub
#End Region
#Region "Properties"
    ''' <summary>
    ''' Set script command 3 information
    ''' </summary>
    ''' <value></value>
    ''' <remarks></remarks>
    Public WriteOnly Property Scriptcommand3() As InspectionProcedure.ScriptCommand3
        Set(ByVal value As InspectionProcedure.ScriptCommand3)
            _scriptcommand3 = value
        End Set
    End Property
#End Region
End Class


