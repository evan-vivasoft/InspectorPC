'===============================================================================
'Copyright Wigersma & Sikkema 2013
'All rights reserved.
'===============================================================================

Imports Telerik.WinControls.UI
Imports System.Windows.Forms


Public Class SettingsForm
#Region "Class members"
    Private WithEvents ucTranslationSelection As usctrl_TranslationSelection
    Private WithEvents ucGeneral As usctrl_GeneralUI
    Private WithEvents ucSyncOptions As usctrl_SyncOptions
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
    End Sub
#End Region
#Region "User control"
    ''' <summary>
    ''' Loading a user control to one of the pages of radpageview
    ''' </summary>
    ''' <param name="pageSelected"></param>
    ''' <param name="userControl"></param>
    ''' <remarks></remarks>
    Private Sub loadUserControl(ByVal pageSelected As RadPageViewPage, ByVal userControl As UserControl, ByVal dockStyle As DockStyle)
        'Loading the usercontrol into the toolwindow
        If pageSelected.Controls.Count > 0 Then
            'If any usercontrol already exists in the toolwindow. Dispose it.
            For i As Integer = 1 To pageSelected.Controls.Count
                Dim control As Control = pageSelected.Controls(i - 1)
                If control IsNot Nothing Then
                    control.Dispose()
                End If
            Next
        End If

        pageSelected.Controls.Add(userControl)
        userControl.Dock = dockStyle
        userControl.Left = 0
        userControl.Top = 0
        userControl.Show()
    End Sub
    ''' <summary>
    ''' Loading the different user controls to the form
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub SettingsForm_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        ucTranslationSelection = New usctrl_TranslationSelection
        loadUserControl(PageLanguage, ucTranslationSelection, DockStyle.None)

        ucGeneral = New usctrl_GeneralUI
        loadUserControl(PageGeneral, ucGeneral, DockStyle.Fill)

        ucSyncOptions = New usctrl_SyncOptions
        loadUserControl(PageSyncOptions, ucSyncOptions, DockStyle.Fill)

    End Sub
#End Region
End Class
