'===============================================================================
'Copyright Wigersma 2012
'All rights reserved.
'===============================================================================
Imports Inspector.Model
Imports KAM.INSPECTOR.Infra

''' <summary>
''' The user can or has to input text or select from 2 or 3 options. If a selection/ input is required the next button is displayed after the selection/ input
''' The user presses the button Next to resume the inspection procedure
''' 
''' This script command should always raise the event evntNextWithResult. 
''' The event evntNext will raise an error in the program
'''
'''Question to the inspector. The selected or inputted data is saved in the inspection result
'''xsd:element name="SequenceNumber" type="xsd:long"/>                       ; Sequence number in inspection procedure
'''xsd:element name="ObjectName" type="xsd:string" minOccurs="0"/>           ; Result/ boundary link
'''xsd:element name="MeasurePoint" type="xsd:string" minOccurs="0"/>         ; Result/ boundary link
'''xsd:element name="FieldNo" type="xsd:integer" minOccurs="0"/>             ; Result/ boundary link
'''xsd:element name="Question" type="xsd:string"/>                           ; Tekst/ Question to display in inspector; 
'''xsd:element name="TypeQuestion">                                          ; Define the type of queston
'''xsd:simpleType>
'''xsd:restriction base="xsd:string">
'''xsd:enumeration value="0; Input multi lines"/>                            ; Type multilines; the user can input text in a text box
'''xsd:enumeration value="1; Input single line"/>                            ; type single lines; same as multilines
'''xsd:enumeration value="2; 2 options"/>                                    ; 2 options; The user can select one of two option
'''xsd:enumeration value="3; 3 options"/>                                    ; 3 options; The user can select one of three option
'''xsd:restriction>
'''/xsd:simpleType>
'''/xsd:element>
'''xsd:element name="TextOptions" type="xsd:string" minOccurs="0" maxOccurs="3"/>  ; The different options (for TypeQuestion 2 or 3 options) With multi/ single line this is not needed
'''xsd:element name="Required" type="xsd:boolean"/>                          ; Define if the user has to input/ select something. 
''' 
''' </summary>
''' <remarks></remarks>

Public Class usctrl_scriptCommand4
#Region "Class members"
    Shadows Event evntNext(stepSequenceNumber As Integer)
    Public Event evntNextWithResult(stepSequenceNumber As Integer, resultText As String)

#End Region

#Region "Commands"
    ''' <summary>
    ''' Completion of inspection step
    ''' Raise event evntNext with script sequence number
    ''' </summary>
    ''' <remarks></remarks>
    Public Overrides Sub InspectionStepCompleted()
        DebugGUILogger.Debug(Me.Name & " ; step completed ; " & _scriptcommand4.SequenceNumber)
        DebugGUILogger.Debug(Me.Name & " ; InspectionStepCompleted event clicked")
        DebugGUILogger.Debug(Me.Name & " ; Status _ClickEventOnlyOnce" & _ClickEventOnlyOnce) 'MOD 88
        DebugGUILogger.Debug(Me.Name & " ; Status _ClickOnlyOnce" & _ClickOnlyOnce) 'MOD 88
        DebugGUILogger.Debug(Me.Name & " ; Status btnnext" & btnNext.Visible) 'MOD 88
        'Check if the next button is visable/ enabled to resume
        'If has to input data.
        If btnNext.Visible = False Or btnNext.Enabled = False Then Exit Sub
        'MOD 60
        btnNext.Visible = False
        DebugGUILogger.Debug(Me.Name & " ; Check status script 4 ") 'MOD 88
        'MOD 85
        If _ClickEventOnlyOnce = False Then
            _ClickEventOnlyOnce = True
            DebugGUILogger.Debug(Me.Name & "; InspectionStepCompleted event handeled")

            'Sending the information with the inspection result
            Select Case _scriptcommand4.TypeQuestion
                Case TypeQuestion.InputSingleLine, TypeQuestion.InputMultiLines
                    DebugGUILogger.Debug(Me.Name & "Event type 1 raised") 'MOD 88
                    RaiseEvent evntNextWithResult(_scriptcommand4.SequenceNumber, rdTextInput.Text)
                Case TypeQuestion.TwoOptions
                    Dim lsButtonToggled As String = ""
                    If rdRadioOption1.ToggleState = Telerik.WinControls.Enumerations.ToggleState.On Then lsButtonToggled = _scriptcommand4.TextOptions.Item(0).ToString
                    If rdRadioOption2.ToggleState = Telerik.WinControls.Enumerations.ToggleState.On Then lsButtonToggled = _scriptcommand4.TextOptions.Item(1).ToString
                    DebugGUILogger.Debug(Me.Name & "Event type 2 raised") 'MOD 88
                    RaiseEvent evntNextWithResult(_scriptcommand4.SequenceNumber, lsButtonToggled)
                Case TypeQuestion.ThreeOptions
                    Dim lsButtonToggled As String = ""
                    If rdRadioOption1.ToggleState = Telerik.WinControls.Enumerations.ToggleState.On Then lsButtonToggled = _scriptcommand4.TextOptions.Item(0).ToString
                    If rdRadioOption2.ToggleState = Telerik.WinControls.Enumerations.ToggleState.On Then lsButtonToggled = _scriptcommand4.TextOptions.Item(1).ToString
                    If rdRadioOption3.ToggleState = Telerik.WinControls.Enumerations.ToggleState.On Then lsButtonToggled = _scriptcommand4.TextOptions.Item(2).ToString
                    DebugGUILogger.Debug(Me.Name & "Event type 3 raised") 'MOD 88
                    RaiseEvent evntNextWithResult(_scriptcommand4.SequenceNumber, lsButtonToggled)
                Case Else
                    DebugGUILogger.Debug(Me.Name & "Event type 4 raised") 'MOD 88
                    RaiseEvent evntNextWithResult(_scriptcommand4.SequenceNumber, "")
            End Select
        End If
    End Sub

    ''' <summary>
    ''' Load the information to the form; Set items correct
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub LoadInformation()
        'MOD 06
        rdrtbInstruction.IsReadOnly = True
        rdrtbInstruction.ChangeFontSize(ModuleSettings.SettingFile.GetSetting(GsSectionText, GsSettingTextSize))
        rdrtbInstruction.Insert(_scriptcommand4.Question)

        Dim posLeft As Integer = rdpInstruction.Left
        Dim posTop As Integer = rdpInstruction.Top + rdpInstruction.Height + 3

        'MOD 60 If _scriptcommand4.Required = True Then btnNext.Visible = False Else btnNext.Visible = True
        'MOD 06 Me.rdtbInstruction.Text = _scriptcommand4.Question
        Select Case _scriptcommand4.TypeQuestion
            Case TypeQuestion.InputSingleLine
                rdScrolPanelSelection.Visible = False
                rdTextInput.Visible = True
                rdTextInput.Multiline = False
                rdTextInput.Left = PosLeft
                rdTextInput.Top = posTop
                rdTextInput.Text = _scriptcommand4Result 'MOD 61
                rdTextInput.TextBoxElement.TextBoxItem.HostedControl.Focus()
                rdTextInput.Focus()

            Case (TypeQuestion.InputMultiLines)
                rdScrolPanelSelection.Visible = False
                rdTextInput.Visible = True
                rdTextInput.Multiline = True
                rdTextInput.Left = PosLeft
                rdTextInput.Top = posTop
                rdTextInput.Text = _scriptcommand4Result 'MOD 61
                rdTextInput.Focus()
            Case TypeQuestion.TwoOptions
                rdScrolPanelSelection.Visible = True
                rdScrolPanelSelection.Left = PosLeft
                rdScrolPanelSelection.Top = PosTop
                rdTextInput.Visible = False
                rdRadioOption1.Text = _scriptcommand4.TextOptions.Item(0).ToString
                rdRadioOption1.Visible = True
                rdRadioOption2.Text = _scriptcommand4.TextOptions.Item(1).ToString
                rdRadioOption2.Visible = True
                rdRadioOption3.Visible = False
                rdScrolPanelSelection.Focus()

            Case TypeQuestion.ThreeOptions
                rdScrolPanelSelection.Visible = True
                rdScrolPanelSelection.Left = PosLeft
                rdScrolPanelSelection.Top = PosTop
                rdTextInput.Visible = False
                rdRadioOption1.Text = _scriptcommand4.TextOptions.Item(0).ToString
                rdRadioOption1.Visible = True
                rdRadioOption2.Text = _scriptcommand4.TextOptions.Item(1).ToString
                rdRadioOption2.Visible = True
                rdRadioOption3.Text = _scriptcommand4.TextOptions.Item(2).ToString
                rdRadioOption3.Visible = True
                rdScrolPanelSelection.Focus()
        End Select
        Me.Refresh()

    End Sub
    ''' <summary>
    ''' Check if the button is toggle to enable the button next
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="args"></param>
    ''' <remarks></remarks>
    Private Sub RadRadioButton_ToggleStateChanged(sender As System.Object, args As Telerik.WinControls.UI.StateChangedEventArgs) Handles rdRadioOption1.ToggleStateChanged, rdRadioOption2.ToggleStateChanged, rdRadioOption3.ToggleStateChanged
        If _scriptcommand4.Required = True Then btnNext.Visible = True
    End Sub
    ''' <summary>
    ''' Handling of text changed; Check if something is input; to enable the btn next.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub rdTextInput_TextChanged(sender As System.Object, e As System.EventArgs) Handles rdTextInput.TextChanged
        If _scriptcommand4.Required = False Then
            btnNext.Visible = True
        Else
            If Len(rdTextInput.Text) > 0 Then btnNext.Visible = True Else btnNext.Visible = False
        End If
    End Sub

#End Region
#Region "Properties"
    ''' <summary>
    ''' Set script command 4 information
    ''' </summary>
    ''' <value></value>
    ''' <remarks></remarks>
    Public WriteOnly Property Scriptcommand4() As InspectionProcedure.ScriptCommand4
        Set(ByVal value As InspectionProcedure.ScriptCommand4)
            _scriptcommand4 = value
        End Set
    End Property
    Dim _scriptcommand4 As New InspectionProcedure.ScriptCommand4


    'MOD 61
    ''' <summary>
    ''' Set this value with the value of Scriptcommando 42.
    ''' This to ensure that the result is only saved once
    ''' </summary>
    ''' <value></value>
    ''' <remarks></remarks>
    Public WriteOnly Property Scriptcommand4Result() As String
        Set(ByVal value As String)
            _scriptcommand4Result = value
        End Set
    End Property
    Dim _scriptcommand4Result As String

#End Region

End Class
