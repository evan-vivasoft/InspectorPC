Imports Inspector.Model
Imports KAM.INSPECTOR.Infra
''' <summary>
''' Display a list of items which the user can select from. Selection can be required.
''' 
''' This script command should always raise the event evntNextWithResult. 
''' The event evntNext will raise an error in the program
''' 
''' xsd:element name="SequenceNumber" type="xsd:long"/>                           ; Sequence number in inspection procedure
''' xsd:element name="ObjectName" type="xsd:string" minOccurs="0"/>               ; Result/ boundary link
''' xsd:element name="MeasurePoint" type="xsd:string" minOccurs="0"/>             ; Result/ boundary link
''' xsd:element name="FieldNo" type="xsd:integer" minOccurs="0"/>                 ; Result/ boundary link
''' xsd:element name="Instruction" type="xsd:string"/>                            ; Instruction/ text to be displayed to used
''' xsd:element name="ListItem" type="xsd:string" minOccurs="2" maxOccurs="20"/>  ; List items (maximum 20) which the user can select from; The option "No option" is added. 
''' xsd:element name="Required" type="xsd:boolean"/>                              ; Selection of al list item is required.
''' </summary>
''' <remarks></remarks>

Public Class usctrl_scriptCommand43
#Region "Class members"
    Shadows Event evntNext(stepSequenceNumber As Integer)
    Public Event evntNextWithResult(stepSequenceNumber As Integer, resultText As String)
    Public _scriptcommand43 As New InspectionProcedure.ScriptCommand43
#End Region
#Region "Constructor"
    ''' <summary>
    ''' Initialize
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()
        ' This call is required by the designer.
        InitializeComponent()
        'Capture clicking the the next button of the main screen
    End Sub
    ''' <summary>
    ''' Load the information to the form; Set items correct
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub LoadInformation()
        'mod 06 rdtbInstruction.Text = _scriptcommand43.Instruction

        'MOD 06
        rdrtbInstruction.IsReadOnly = True
        rdrtbInstruction.ChangeFontSize(ModuleSettings.SettingFile.GetSetting(GsSectionText, GsSettingTextSize))
        rdrtbInstruction.Insert(_scriptcommand43.Instruction)

        'Add option !No option only if it is not required to select an option
        If _scriptcommand43.Required = False Then rdListControl.Items.Add(My.Resources.InspectionProcedureResx.str_No_option)

        For i As Integer = 1 To _scriptcommand43.ListItems.Count
            rdListControl.Items.Add(_scriptcommand43.ListItems(i - 1).ToString)
        Next
        'Default select item "No option"
        rdListControl.Items(0).Selected = True
        rdListControl.Focus()
    End Sub
#End Region
#Region "Event triggering"
    ''' <summary>
    ''' Completion of inspection step
    ''' Raise event evntNext with script sequence number
    ''' </summary>
    ''' <remarks></remarks>
    Public Overrides Sub InspectionStepCompleted()
        DebugGUILogger.Debug(Me.Name & " ; step completed ; " & _scriptcommand43.SequenceNumber) 'MOD 83
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
            Dim selecteditem As String = ""
            If rdListControl.SelectedIndex = 0 And _scriptcommand43.Required = False Then selecteditem = "No option" Else selecteditem = rdListControl.SelectedItem.ToString

            RaiseEvent evntNextWithResult(_scriptcommand43.SequenceNumber, selecteditem)
        End If
    End Sub
#End Region

#Region "Properties"
    ''' <summary>
    ''' Set script command 43 information
    ''' </summary>
    ''' <value></value>
    ''' <remarks></remarks>
    Public WriteOnly Property Scriptcommand43() As InspectionProcedure.ScriptCommand43
        Set(ByVal value As InspectionProcedure.ScriptCommand43)
            _scriptcommand43 = value
        End Set
    End Property
#End Region
End Class


