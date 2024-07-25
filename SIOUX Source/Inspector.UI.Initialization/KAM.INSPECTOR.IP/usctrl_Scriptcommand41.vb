Imports Inspector.Model
Imports Telerik.WinControls.UI
Imports System.Windows.Forms
Imports KAM.INSPECTOR.Infra 'MOD 83

'TO DO handling of shownextlistimmidally

Public Class usctrl_Scriptcommand41
#Region "Class members"
    Shadows Event evntNext(stepSequenceNumber As Integer)
    Public Event evntNextWithListResult(inspectionStepResultList As InspectionStepResult.InspectionStepResultSelections)
    Public _scriptcommand41 As New InspectionProcedure.ScriptCommand41
    Private WithEvents ucScript41List1 As usctrl_ScriptCommand41_ControlList
    Private WithEvents ucScript41List2 As usctrl_ScriptCommand41_ControlList
    Private WithEvents ucScript41List3 As usctrl_ScriptCommand41_ControlList
    Private WithEvents ucScript41List4 As usctrl_ScriptCommand41_ControlList
    Private WithEvents ucScript41List5 As usctrl_ScriptCommand41_ControlList
    Private WithEvents ucScript41List6 As usctrl_ScriptCommand41_ControlList

    Enum enumstateResumeNextList As Integer
        DisplayNextList = 0
        ResumeCheck = 1
        EndScriptCommand = 2
    End Enum

#End Region

#Region "Constructor"
    ''' <summary>
    ''' Initialization
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()
        ' This call is required by the designer.
        InitializeComponent()
        'Capture clicking the the next button of the main screen
    End Sub
    ''' <summary>
    ''' Load scriptcommand 41 information
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub LoadInformation()
        'TO do Only at check list
        rdCheckNextListImm.Checked = _scriptcommand41.ShowNextListImmediately
        For i As Integer = 0 To _scriptcommand41.ScriptCommandList.Count - 1
            Dim page As New Telerik.WinControls.UI.RadPageViewPage
            page.Text = My.Resources.InspectionProcedureResx.str_List & " " & i + 1
            rdPageSC41List.Pages.Add(page)

            Select Case i
                Case 0
                    ucScript41List1 = New usctrl_ScriptCommand41_ControlList
                    ucScript41List1.Scriptcommand41List = _scriptcommand41.ScriptCommandList(0)
                    ucScript41List1.ShowNextListImmediately = False 'rdCheckNextListImm.Checked
                    loadUserControl(rdPageSC41List.Pages(i), ucScript41List1)
                    ucScript41List1.Intializedata()
                Case 1
                    ucScript41List2 = New usctrl_ScriptCommand41_ControlList
                    ucScript41List2.Scriptcommand41List = _scriptcommand41.ScriptCommandList(1)
                    ucScript41List1.ShowNextListImmediately = False
                    loadUserControl(rdPageSC41List.Pages(i), ucScript41List2)
                    rdPageSC41List.Pages(i).Enabled = False
                    ucScript41List2.Intializedata()
                Case 2
                    ucScript41List3 = New usctrl_ScriptCommand41_ControlList
                    ucScript41List3.Scriptcommand41List = _scriptcommand41.ScriptCommandList(2)
                    ucScript41List1.ShowNextListImmediately = False
                    loadUserControl(rdPageSC41List.Pages(i), ucScript41List3)
                    ucScript41List3.Intializedata()
                    rdPageSC41List.Pages(i).Enabled = False
                Case 3
                    ucScript41List4 = New usctrl_ScriptCommand41_ControlList
                    ucScript41List4.Scriptcommand41List = _scriptcommand41.ScriptCommandList(3)
                    ucScript41List1.ShowNextListImmediately = False
                    ucScript41List4.Intializedata()
                    loadUserControl(rdPageSC41List.Pages(i), ucScript41List4)
                    rdPageSC41List.Pages(i).Enabled = False
                Case 4
                    ucScript41List5 = New usctrl_ScriptCommand41_ControlList
                    ucScript41List5.Scriptcommand41List = _scriptcommand41.ScriptCommandList(4)
                    ucScript41List1.ShowNextListImmediately = False
                    ucScript41List5.Intializedata()
                    loadUserControl(rdPageSC41List.Pages(i), ucScript41List5)
                    rdPageSC41List.Pages(i).Enabled = False
            End Select
        Next


    End Sub

    ''' <summary>
    ''' Load usercontrols as page 
    ''' </summary>
    ''' <param name="panel"></param>
    ''' <param name="UserControl"></param>
    ''' <remarks></remarks>
    Private Sub loadUserControl(ByVal panel As RadPageViewPage, UserControl As UserControl)
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
        UserControl.Hide()
        panel.Controls.Add(UserControl)
        UserControl.Dock = DockStyle.Fill
        UserControl.Left = 0
        UserControl.Top = 0
        UserControl.Show()
    End Sub




#End Region

#Region "Commands"
    ''' <summary>
    ''' Completion of inspection step
    ''' Raise event evntNext with script sequence number
    ''' </summary>
    ''' <remarks></remarks>
    Public Overrides Sub InspectionStepCompleted()
        DebugGUILogger.Debug(Me.Name & " ; step completed ; " & _scriptcommand41.SequenceNumber)
        DebugGUILogger.Debug(Me.Name & "; InspectionStepCompleted event clicked")
        If btnNext.Visible = False Or btnNext.Enabled = False Then Exit Sub
        'MOD 60
        btnNext.Visible = False

        'MOD 85
        If _ClickEventOnlyOnce = False Then
            _ClickEventOnlyOnce = True
            DebugGUILogger.Debug(Me.Name & "; InspectionStepCompleted event handeled")

            Dim stateNextList As enumstateResumeNextList

            If ucScript41List1 IsNot Nothing Then stateNextList = EnableNextPage(ucScript41List1, ucScript41List2, 1)
            If stateNextList = enumstateResumeNextList.EndScriptCommand Then GoTo EndScript
            If stateNextList = enumstateResumeNextList.DisplayNextList Then Exit Sub

            If ucScript41List2 IsNot Nothing Then stateNextList = EnableNextPage(ucScript41List2, ucScript41List3, 2)
            If stateNextList = enumstateResumeNextList.EndScriptCommand Then GoTo EndScript
            If stateNextList = enumstateResumeNextList.DisplayNextList Then Exit Sub

            If ucScript41List3 IsNot Nothing Then stateNextList = EnableNextPage(ucScript41List3, ucScript41List4, 3)
            If stateNextList = enumstateResumeNextList.EndScriptCommand Then GoTo EndScript
            If stateNextList = enumstateResumeNextList.DisplayNextList Then Exit Sub

            If ucScript41List4 IsNot Nothing Then stateNextList = EnableNextPage(ucScript41List4, ucScript41List5, 4)
            If stateNextList = enumstateResumeNextList.EndScriptCommand Then GoTo EndScript
            If stateNextList = enumstateResumeNextList.DisplayNextList Then Exit Sub

            If ucScript41List5 IsNot Nothing Then stateNextList = EnableNextPage(ucScript41List5, ucScript41List6, 5)
            If stateNextList = enumstateResumeNextList.EndScriptCommand Then GoTo EndScript
            If stateNextList = enumstateResumeNextList.DisplayNextList Then Exit Sub

EndScript:

            'Check if the next button is visable/ enabled to resume
            'Sending the information with the inspection result
            Dim scriptcommand41Result As New InspectionStepResult.InspectionStepResultSelections
            scriptcommand41Result.SequenceNumber = _scriptcommand41.SequenceNumber
            If ucScript41List1 IsNot Nothing Then scriptcommand41Result.AnswerSelection1 = ucScript41List1.ListResult
            If ucScript41List2 IsNot Nothing Then scriptcommand41Result.AnswerSelection2 = ucScript41List2.ListResult
            If ucScript41List3 IsNot Nothing Then scriptcommand41Result.AnswerSelection3 = ucScript41List3.ListResult
            If ucScript41List4 IsNot Nothing Then scriptcommand41Result.AnswerSelection4 = ucScript41List4.ListResult
            If ucScript41List5 IsNot Nothing Then scriptcommand41Result.AnswerSelection5 = ucScript41List5.ListResult
            If radTxtBRemarks.Text.ToString <> "" Then scriptcommand41Result.Remark = radTxtBRemarks.Text.ToString

            RaiseEvent evntNextWithListResult(scriptcommand41Result)
        End If
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="scriptCommand41List"></param>
    ''' <param name="pageNumber"></param>
    ''' <returns></returns> True is to resume with the next list; 
    ''' <remarks></remarks>
    Private Function EnableNextPage(scriptCommand41List As usctrl_ScriptCommand41_ControlList, scriptCommand41NextList As usctrl_ScriptCommand41_ControlList, pageNumber As Integer) As enumstateResumeNextList
        If scriptCommand41List.ListCompleted = False Then
            scriptCommand41List.ListCompleted = True
            scriptCommand41List.CreateListResult()
            If scriptCommand41List.ItemNoNextListSelected = True Then Return enumstateResumeNextList.EndScriptCommand
            If scriptCommand41NextList IsNot Nothing Then
                rdPageSC41List.Pages(pageNumber).Enabled = True
                rdPageSC41List.SelectedPage = rdPageSC41List.Pages(pageNumber)
                rdPageSC41List.Pages(pageNumber - 1).Enabled = False
                'MOD 86
                _ClickOnlyOnce = False
                _ClickEventOnlyOnce = False
                Return enumstateResumeNextList.DisplayNextList
            Else
                Return enumstateResumeNextList.EndScriptCommand
            End If
        Else
            Return enumstateResumeNextList.ResumeCheck
        End If
    End Function



#End Region

#Region "Properties"
    ''' <summary>
    ''' Scriptcommand 41 properties
    ''' </summary>
    ''' <value></value>
    ''' <remarks></remarks>
    Public WriteOnly Property Scriptcommand41() As InspectionProcedure.ScriptCommand41
        Set(ByVal value As InspectionProcedure.ScriptCommand41)
            _scriptcommand41 = value
        End Set
    End Property
#End Region
#Region "Event Handling"
    ''' <summary>
    ''' Event of page added
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub rdPageSC41List_PageAdded(sender As Object, e As Telerik.WinControls.UI.RadPageViewEventArgs) Handles rdPageSC41List.PageAdded
        If _scriptcommand41.ScriptCommandList(rdPageSC41List.Pages.IndexOf(rdPageSC41List.SelectedPage).ToString()).SelectionRequired = True Then btnNext.Visible = False Else btnNext.Visible = True
    End Sub

    ''' <summary>
    ''' Event of selected page changed
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub rdPageSC41List_SelectedPageChanged(sender As Object, e As System.EventArgs) Handles rdPageSC41List.SelectedPageChanged
        If _scriptcommand41.ScriptCommandList(rdPageSC41List.Pages.IndexOf(rdPageSC41List.SelectedPage).ToString()).SelectionRequired = True Then btnNext.Visible = False Else btnNext.Visible = True
    End Sub
    ''' <summary>
    ''' Handling of events of script command 41 lists to enable or disable btnNext
    ''' </summary>
    ''' <param name="value"></param>
    ''' <remarks></remarks>
    Private Sub EvntHandlingShowNextButton(value As Boolean) Handles ucScript41List1.evntSelectionRequired, ucScript41List2.evntSelectionRequired, ucScript41List3.evntSelectionRequired, ucScript41List4.evntSelectionRequired, ucScript41List5.evntSelectionRequired
        btnNext.Visible = value
    End Sub

#End Region



    Private Sub radTxtBRemarks_TextChanged(sender As Object, e As EventArgs) Handles radTxtBRemarks.TextChanged

    End Sub
End Class
