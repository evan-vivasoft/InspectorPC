'===============================================================================
'Copyright Wigersma 2013
'All rights reserved.
'===============================================================================

Imports KAM.COMMUNICATOR.Infra
Imports Telerik.WinControls




Public Class MainForm
#Region "Class members"
    'The user control of PDA synchronization
    Private WithEvents ucPDA As KAM.COMMUNICATOR.Synchronize.Infra.usctrl_Synchr
    Private closeApplication As Boolean = True

#End Region
#Region "Enum class"
    ''' <summary>
    ''' Different telerik themes
    ''' </summary>
    ''' <remarks></remarks>
    Public Class enumTelerikThemes
        Public Const Windows7 As String = "Windows7"
        Public Const Office2007Black As String = "Office2007Black"
        Public Const Office2007Silver As String = "Office2007Silver"
        Public Const Office2010 As String = "Office2010"
        Public Const HighContrastBlack As String = "HighContrastBlack"
        Public Const Aqua As String = "Aqua"
        Public Const Breeze As String = "Breeze"
        Public Const BreezeExtends As String = "BreezeExtends"
        Public Const Desert As String = "Desert"
        Public Const Misc As String = "Miscellaneous"
        Public Const Telerik As String = "Telerik"
        Public Const Vista As String = "Vista"
    End Class
#End Region
#Region "Constructor"
    ''' <summary>
    ''' Loading the form
    ''' Load the user control to the form
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub MainForm_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        SetThemeName()
        DebugLogger = log4net.LogManager.GetLogger("LogCommunicatorAppl1")

        DebugLogger.Info("Load main")
        Me.ucPDA = New KAM.COMMUNICATOR.Synchronize.Infra.usctrl_Synchr
        ucPDA.Dock = Windows.Forms.DockStyle.Fill
        Me.RadPanel1.Controls.Add(ucPDA)


    End Sub
    ''' <summary>
    ''' Initialize form
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()
        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
    End Sub
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
    End Sub
#End Region
#Region "Telerik Theming"
    ''' <summary>
    ''' Set the theme for the GUI 
    ''' Telerik themes only
    ''' The settings is in the key "Localization", "ThemeName"
    ''' Default theme is officeblack
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub SetThemeName()
        'Get the theme name from the settings file. If no value, Office2007Black theme is loaded
        ThemeName = SettingFile.GetSetting(GsSectionLocalization, GsSettingLocalizationThemeName)
        If ThemeName = ModuleCommunicatorSettings.SettingFile.NoValue Or ThemeName Is Nothing Then
            ThemeName = enumTelerikThemes.Office2007Black
        End If
        'Assign theme to UI
        ThemeResolutionService.ApplicationThemeName = ThemeName
    End Sub
#End Region
#Region "Button Handling"
    ''' <summary>
    ''' Loading the sub form "About"
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub rdMenuAbout_Click(sender As System.Object, e As System.EventArgs) Handles rdMenuAbout.Click
        AboutLicenseForm.ShowDialog(Me)
    End Sub
    ''' <summary>
    ''' Loading the sub form "Settings"
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub rdMenuSettings_Click(sender As System.Object, e As System.EventArgs) Handles rdMenuSettings.Click
        SettingsForm.ShowDialog(Me)
    End Sub
#End Region

#Region "Event handling sync proces start/ stop"
    ''' <summary>
    ''' Event handling of synchronization process is finished
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub EvntHandlingSyncRunning(runningStatus As Boolean) Handles ucPDA.evntSyncRunning
        'MOD 06
        BeginInvoke(New Action(Of Boolean)(AddressOf InvokeSynchronizeSyncRunning), runningStatus)
    End Sub
    ''' <summary>
    ''' Invoke of synchronization process is finished
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub InvokeSynchronizeSyncRunning(runningStatus As Boolean)
        'MOD 06
        closeApplication = Not (runningStatus)
    End Sub
#End Region



End Class
