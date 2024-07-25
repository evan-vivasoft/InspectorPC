'===============================================================================
'Copyright Wigersma 2012
'All rights reserved.
'===============================================================================

Imports Inspector.Model
Imports KAM.INSPECTOR.Infra
''' <summary>
''' Script command 1 is used to display an instruction to the user. The user presses the button Next to resume the inspection procedure
''' 
''' Raise event evntNext when the user presses the next button
''' 
''' xsd:element name="SequenceNumber" type="xsd:long"/>                           ; Sequence number in inspection procedure
''' xsd:element name="Text" type="xsd:string"/>                                   ; Display text/ instruction to the user
''' </summary>
''' <remarks></remarks>

Public Class usctrl_scriptCommand1
#Region "Class members"
    Shadows Event evntNext(stepSequenceNumber As Integer)
    Private _scriptcommand1 As New InspectionProcedure.ScriptCommand1
#End Region



#Region "Commands"
    ''' <summary>
    ''' New instance
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()
        ' This call is required by the designer.
        InitializeComponent()
    End Sub
    ''' <summary>
    ''' Load the information to the form; Set items correct
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub LoadInformation()
        'MOD 47
        'MOD 60 btnNext.Visible = False
        'MOD 06
        rdrtbInstruction.IsReadOnly = True
        rdrtbInstruction.ChangeFontSize(ModuleSettings.SettingFile.GetSetting(GsSectionText, GsSettingTextSize))
        rdrtbInstruction.Insert(_scriptcommand1.Text)
        Me.Refresh()
        rdrtbInstruction.Focus()
        'MOD 47
        'MOD 60  Wait(1000)
        'MOD 60 btnNext.Visible = True

    End Sub



    ''' <summary>
    ''' Completion of inspection step
    ''' Raise event evntNext with script sequence number
    ''' </summary>
    ''' <remarks></remarks>
    Public Overrides Sub InspectionStepCompleted()

        DebugGUILogger.Debug(Me.Name & " ; step completed ; " & _scriptcommand1.SequenceNumber)
        DebugGUILogger.Debug(Me.Name & "; InspectionStepCompleted event clicked")
        If btnNext.Visible = False Or btnNext.Enabled = False Then Exit Sub
        'MOD 60
        btnNext.Visible = False

        'MOD 85
        If _ClickEventOnlyOnce = False Then
            _ClickEventOnlyOnce = True
            DebugGUILogger.Debug(Me.Name & "; InspectionStepCompleted event handeled")

            'Sending the information with the inspection result
            RaiseEvent evntNext(_scriptcommand1.SequenceNumber)
        End If
    End Sub
#End Region

#Region "Properties"
    ''' <summary>
    ''' Set script command 1 information
    ''' </summary>
    ''' <value></value>
    ''' <remarks></remarks>
    Public WriteOnly Property Scriptcommand1() As InspectionProcedure.ScriptCommand1
        Set(ByVal value As InspectionProcedure.ScriptCommand1)
            _scriptcommand1 = value
        End Set
    End Property
#End Region
End Class
