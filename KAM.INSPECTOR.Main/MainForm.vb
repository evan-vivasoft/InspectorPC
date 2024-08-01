'===============================================================================
'Copyright Wigersma 2012
'All rights reserved.
'===============================================================================
Imports Telerik.WinControls.UI.Docking
Imports Telerik.WinControls.UI
Imports System.Xml
Imports KAM.INSPECTOR.Infra
Imports KAM.INSPECTOR.IP
Imports Inspector.Infra.Ioc
Imports KAM.LicenceTool
Imports QlmLicenseLib
Imports System.Globalization
Imports System.Threading
Imports Microsoft.Win32
Imports System.Configuration
Imports System.Runtime.Remoting.Contexts
Imports Inspector.POService.LicenseValidator


Public Class MainForm
#Region "Class members"
    Private WithEvents ucPlexor As KAM.INSPECTOR.PLEXOR.usctrl_FormPlexor
    Private WithEvents ucInspection As KAM.INSPECTOR.IP.usctrl_MainInspection
    Private WithEvents ucPrs As KAM.INSPECTOR.PRS.usctrl_FormPRS
    Private WithEvents ucResults As KAM.INSPECTOR.ResultsGridView.usctrl_FormDisplayResults
    Private WithEvents usctrl_Synchr As usctrl_Synchr
    Private _poLicenseValidator As IPOLicenseValidator
    'KAM.INSPECTOR.Results.DisplayResults

    Private WithEvents ucLicenceRequest As KAM.LicenceTool.uscrtl_LicenseRequest

    Private m_Disposed As Boolean = False
    Private closeApplication As Boolean = True

    Private WithEvents ucScriptCommand42 As usctrl_Scriptcommand42

    'For mode without wireless interface
    Private _CommunicationMode As _enumWICommunicationMode
    Private Enum _enumWICommunicationMode
        Demomode = 0
        FullMode = 1
    End Enum

#End Region

#Region "Constructor"
    ''' <summary>
    ''' Initialize component
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()
        ' This call is required by the designer.
        Me.InitializeComponent()

        'Handling of context menu; To disable close buttons from the context menu (right mouse click)
        Dim menuService As ContextMenuService = Me.rdDockMain.GetService(Of ContextMenuService)()
        AddHandler menuService.ContextMenuDisplaying, AddressOf MenuService_ContextMenuDisplaying

        'Only show pin button
        toolWindowRemarks.ToolCaptionButtons = ToolStripCaptionButtons.AutoHide
        toolWindowRemarks.Hide()

        ' Add any initialization after the InitializeComponent() call.
    End Sub

    ''' <summary>
    ''' Loading the different user controls to the form
    ''' First load Inspection procedure; To display data of selected PRS/ GCL</summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub SettingsForm_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        If Not POLicenseValidator.IsNewUser Then
            'If LicenseValidator.LicenseStatus = ELicenseStatus.EKeyDemo Or LicenseValidator.LicenseStatus = ELicenseStatus.EKeyPermanent Then

            Me.ucInspection = New KAM.INSPECTOR.IP.usctrl_MainInspection
            Me.LoadDocumentWindow(DocWindowInspection, ucInspection)
            'FullOrDemoMode()

            Me.ucPlexor = New KAM.INSPECTOR.PLEXOR.usctrl_FormPlexor
            Me.LoadDocumentWindow(DocWindowPLEXOR, ucPlexor)


            Me.ucPrs = New KAM.INSPECTOR.PRS.usctrl_FormPRS
            Me.LoadDocumentWindow(DocWindowPRS, ucPrs)

            'MOD xxx Me.ucResults = New KAM.INSPECTOR.Results.DisplayResults
            Me.ucResults = New KAM.INSPECTOR.ResultsGridView.usctrl_FormDisplayResults
            Me.LoadDocumentWindow(DocWindowResults, ucResults)

            Me.usctrl_Synchr = New usctrl_Synchr
            Me.LoadDocumentWindow(DocWindowSync, usctrl_Synchr)
            'MOD NEW
            'If ModuleSettings.SettingFile.GetSetting(GsSectionFunctions, GsSettingFunctionsSynchronize) = False Then
            '    Me.rdDockMain.DockWindows(Me.DocWindowSync.Name).Hide()
            'Else
            '    Me.rdDockMain.DockWindows(Me.DocWindowSync.Name).Show()
            'End If

            Me.DocWindowPRS.Select()

        Else
            rdDockMain.Visible = False
            ucLicenceRequest = New uscrtl_LicenseRequest

            Me.Controls.Add(ucLicenceRequest)
            ucLicenceRequest.Top = (Me.Height - ucLicenceRequest.Height) / 2
            ucLicenceRequest.Left = (Me.Width - ucLicenceRequest.Width) / 2
        End If


    End Sub
#End Region

#Region "Form event handling"
    ''' <summary>
    ''' Handling of if the user select/ change the prs or gcl
    ''' </summary>
    ''' <param name="prsName"></param>
    ''' <param name="gclName"></param>
    ''' <remarks></remarks>
    Private Sub EvntHandlingucPrs(ByVal prsName As String, ByVal gclName As String, ByVal inspectionProcedureName As String) Handles ucPrs.evntGridClicked
        ucInspection.UpdatePRSGCLInformation(prsName, gclName, inspectionProcedureName)
        ucResults.UpdatePRSGCLInformation(prsName)
    End Sub
    ''' <summary>
    ''' Handling of event of user control usctrl_MainInspection
    ''' Will be triggered when the inspection is completed
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub EvntHandlingInspectionCompleted(ByVal prsName As String, ByVal gclName As String) Handles ucInspection.evntInspectionCompleted
        'Update the status of prs data
        ucPrs.UpdateStatusInformation(prsName, gclName)
        Me.toolWindowRemarks.Hide()
        Me.DocWindowPRS.Select()
    End Sub
    ''' <summary>
    ''' Handling of event of user control usctrl_MainInspection
    ''' Will be triggered when a remark window is displayed.
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub EvntHandlingToolWindowRemark() Handles ucInspection.evntShowRemarkWindow
        toolWindowRemarks.Show()
        Me.LoadToolWindow(toolWindowRemarks, ucInspection.ScriptCommand42)
    End Sub

    ''' <summary>
    ''' Handles the event if the inspection or initilization proces is finished
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub EvntHandlingInspectionFinished() Handles ucInspection.evntInspectionFinished
        closeApplication = True
        ucPrs.DisableForm = True
        ucPlexor.DisableForm = True
    End Sub
    ''' <summary>
    ''' Handles the event if the inspection or initilization proces is started
    ''' ''' </summary>
    ''' <remarks></remarks>
    Private Sub EvntHandlingInspectionStarted() Handles ucInspection.evntInspectionStarted
        closeApplication = False
        ucPrs.DisableForm = False
        ucPlexor.DisableForm = False
    End Sub
    ''' <summary>
    ''' Handles the event if the initilization proces is finished
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub EvntHandlingInitializationFinished() Handles ucPlexor.evntInitializationFinished
        closeApplication = True
        ucPrs.DisableForm = True
    End Sub
    ''' <summary>
    ''' Handles the event if the initilization proces is started
    ''' ''' </summary>
    ''' <remarks></remarks>
    Private Sub EvntHandlingInitializationStarted() Handles ucPlexor.evntInitializationStarted
        closeApplication = False
        ucPrs.DisableForm = False
    End Sub

#End Region


#Region "User control"
    ''' <summary>
    ''' Loading a user control to the telerik document window
    ''' </summary>
    ''' <param name="documentSelected"></param>
    ''' <param name="userControl"></param>
    ''' <remarks></remarks>
    Private Sub LoadDocumentWindow(ByVal documentSelected As DocumentWindow, ByVal userControl As UserControl)
        'Loading the usercontrol into the toolwindow
        If documentSelected.Controls.Count > 0 Then
            'If any usercontrol already exists in the toolwindow. Dispose it.
            For i As Integer = 1 To documentSelected.Controls.Count
                Dim control As Control = documentSelected.Controls(i - 1)
                If control IsNot Nothing Then
                    control.Dispose()
                End If
            Next
        End If

        documentSelected.Controls.Add(userControl)
        userControl.Dock = DockStyle.Fill
        userControl.Left = 0
        userControl.Top = 0
        userControl.Show()
    End Sub
    ''' <summary>
    ''' Loading a user control to the telerik toolwindow
    ''' </summary>
    ''' <param name="documentSelected"></param>
    ''' <param name="userControl"></param>
    ''' <remarks></remarks>
    Private Sub LoadToolWindow(ByVal documentSelected As ToolWindow, ByVal userControl As UserControl)
        'Loading the usercontrol into the toolwindow
        If documentSelected.Controls.Count > 0 Then
            'If any usercontrol already exists in the toolwindow. Dispose it.
            For i As Integer = 1 To documentSelected.Controls.Count
                Dim control As Control = documentSelected.Controls(i - 1)
                If control IsNot Nothing Then
                    control.Dispose()
                End If
            Next
        End If

        documentSelected.Controls.Add(userControl)
        userControl.Dock = DockStyle.Fill
        userControl.Left = 0
        userControl.Top = 0
        userControl.Show()
    End Sub

    ''' <summary>
    ''' Handling of pressing a menu-item on top of window
    ''' Display settings form
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub rdMenuSettings_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles rdMenuSettings.Click
        SettingsForm.ShowDialog(Me)
    End Sub
    Private Sub rdMenuExit_Click(sender As System.Object, e As System.EventArgs) Handles rdMenuExit.Click
        Me.Close()
    End Sub
    Private Sub rdMenuAbout_Click(sender As System.Object, e As System.EventArgs) Handles rdMenuAbout.Click
        AboutLicenseForm.ShowDialog(Me)
    End Sub

    ''' <summary>
    ''' Handling of short cuts keys
    ''' </summary>
    ''' <param name="msg"></param>
    ''' <param name="keyData"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Protected Overrides Function ProcessCmdKey(ByRef msg As Message,
                  ByVal keyData As Keys) As Boolean
        Const WM_KEYDOWN As Integer = &H100
        Const WM_SYSKEYDOWN As Integer = &H104

        If ((msg.Msg = WM_KEYDOWN) Or (msg.Msg = WM_SYSKEYDOWN)) Then
            Select Case (keyData)
                Case Keys.F1
                    Console.WriteLine("F1 Captured")
                    ' Help.ShowHelp(Me, HelpProvider1.HelpNamespace)
                    'Help.ShowHelp(Me, "C:\Users\mcl.KAMSTRUPDK\Documents\My HelpAndManual Projects\Examples\Get_Me_Started.chm")
                    'Help.ShowHelp("C:\Users\mcl.KAMSTRUPDK\Documents\My HelpAndManual Projects\Examples\Get_Me_Started.chm")
                    'HelpProvider1.HelpNamespace 
                Case Keys.F2
                    Console.WriteLine("F2 Captured")
                    CType(Me.rdDockMain.DockWindows(DocWindowPRS.Name), DocumentWindow).Select()

                Case Keys.F3
                    Console.WriteLine("F3 Captured")
                    CType(Me.rdDockMain.DockWindows(DocWindowInspection.Name), DocumentWindow).Select()

                Case Keys.F4
                    Console.WriteLine("F4 Captured")
                    CType(Me.rdDockMain.DockWindows(DocWindowPLEXOR.Name), DocumentWindow).Select()

                Case Keys.F5
                    Console.WriteLine("F5 Captured")
                    CType(Me.rdDockMain.DockWindows(DocWindowResults.Name), DocumentWindow).Select()

            End Select
        End If
        Return MyBase.ProcessCmdKey(msg, keyData)
    End Function

#End Region
#Region "Destructor"
    ''' <summary>
    ''' Disable closing the application if the inspection or initialization process is running
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub MainForm_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        e.Cancel = Not (closeApplication)
        'MOD 82
        If closeApplication = True Then ContextRegistry.Context.Release()
    End Sub
#End Region

#Region "Inspector Full or Demo mode"
    ''' <summary>
    ''' Check if INSPECTOR runs in demo or full mode
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub FullOrDemoMode()
        Dim XMLDocReader As XmlTextReader = New XmlTextReader("INSPECTORPC.exe.config")
        With XMLDocReader
            While XMLDocReader.Read()
                Select Case XMLDocReader.NodeType
                    Case System.Xml.XmlNodeType.Element
                        If XMLDocReader.Name.Equals("object") Then
                            If .HasAttributes Then
                                If XMLDocReader.GetAttribute("id") = "IHal" Then
                                    If XMLDocReader.GetAttribute("type") = "Inspector.Hal.BluetoothHal, Inspector.Hal" Then
                                        _CommunicationMode = _enumWICommunicationMode.FullMode
                                        XMLDocReader.Close()
                                        Exit Sub
                                    ElseIf XMLDocReader.GetAttribute("type") = "Inspector.Hal.BluetoothHalDemoMode, Inspector.Hal" Then
                                        _CommunicationMode = _enumWICommunicationMode.Demomode
                                        XMLDocReader.Close()
                                        Exit Sub
                                    End If

                                End If
                            End If
                        End If
                End Select
            End While
        End With
        XMLDocReader.Close()
    End Sub
#End Region
#Region "Form resize"
    ''' <summary>
    ''' Start up maximized
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub MainForm_Shown(sender As Object, e As System.EventArgs) Handles Me.Shown
        Me.WindowState = FormWindowState.Maximized
    End Sub
    ''' <summary>
    ''' Resize end form handling
    ''' Call applyResizeForm
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub MainForm_ResizeEnd(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.ResizeEnd
        ApplyResizeForm()
    End Sub
    ''' <summary>
    ''' Resize form handling
    ''' Call ApplyResizeForm
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub MainForm_Resize(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Resize
        ApplyResizeForm()
    End Sub
    ''' <summary>
    ''' Position of the ucLicenseRequest form
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub ApplyResizeForm()
        If ucLicenceRequest IsNot Nothing Then
            ucLicenceRequest.Top = (Me.Height - ucLicenceRequest.Height) / 2
            ucLicenceRequest.Left = (Me.Width - ucLicenceRequest.Width) / 2
        End If
    End Sub

    Public ReadOnly Property POLicenseValidator As IPOLicenseValidator
        Get
            If _poLicenseValidator Is Nothing Then
                _poLicenseValidator = ContextRegistry.Context.Resolve(Of IPOLicenseValidator)
            End If
            Return _poLicenseValidator
        End Get
    End Property
#End Region


#Region "RadDock Handling"
    ''' <summary>
    ''' Handling of context menu of rad dock Documentwindows and tool windows. Disable menu-items Close
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub MenuService_ContextMenuDisplaying(sender As Object, e As ContextMenuDisplayingEventArgs)
        'remove the "Close" menu items
        For i As Integer = 0 To e.MenuItems.Count - 1
            Dim menuItem As RadMenuItemBase = e.MenuItems(i)
            If menuItem.Name = "CloseWindow" OrElse menuItem.Name = "CloseAllButThis" OrElse menuItem.Name = "CloseAll" OrElse menuItem.Name = "Hidden" OrElse TypeOf menuItem Is RadMenuSeparatorItem Then
                menuItem.Visibility = Telerik.WinControls.ElementVisibility.Collapsed
            End If
        Next
    End Sub
    ''' <summary>
    ''' Handling of tab window change
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>'MOD 33</remarks>
    Private Sub rdDockMain_ActiveWindowChanged(sender As Object, e As Telerik.WinControls.UI.Docking.DockWindowEventArgs) Handles rdDockMain.ActiveWindowChanged
        Select Case e.DockWindow.Name
            Case DocWindowPRS.Name
            Case DocWindowInspection.Name
            Case DocWindowPLEXOR.Name
            Case DocWindowResults.Name
                'mod xxx ucResults.DisplayResults()
                'ucResults

        End Select
    End Sub
    ''' <summary>
    ''' Disable if a documentwindow is close (by pressing ALT-F4)
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub RadDock1_DockWindowClosing(sender As Object, e As Telerik.WinControls.UI.Docking.DockWindowCancelEventArgs) Handles rdDockMain.DockWindowClosing
        If e.NewWindow.Name = toolWindowRemarks.Name Then 'MOD NEW Or e.NewWindow.Name = DocWindowSync.Name
            e.Cancel = False
        Else
            e.Cancel = True
        End If
    End Sub
#End Region



#Region "Not Used"
    ''Private Sub radDock1_ActiveWindowChanged(ByVal sender As Object, ByVal e As DockWindowEventArgs) Handles rdDockMain.ActiveWindowChanged
    ''    'Me.AddLog("ActiveWindow changed; current active window: " & e.DockWindow.Text, True)

    ''    If TypeOf e.DockWindow Is DocumentWindow Then
    ''        e.DockWindow.
    ''        Me.SetSelectedDocument(CType(e.DockWindow, DocumentWindow))

    ''    End If
    ''End Sub
    ''Private Sub SetSelectedDocument(ByVal selected As DocumentWindow)
    ''    Select Case selected.Name
    ''        Case Me.DocWindowPLEXOR.Name ' "DocWindowPLEXOR"
    ''            'Usctrl_FormPlexor1.LoadPLEXORData()
    ''        Case Me.DocWindowPRS.Name

    ''        Case Else

    ''            ' MsgBox(selected.Name)
    ''    End Select
    ''End Sub
#End Region

#Region "Test functions"

#End Region

End Class
