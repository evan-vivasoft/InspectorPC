Imports Kamstrup.LKG.Kamstrup.LicenseKeyGenerator.Model
Imports System.Threading
Imports Telerik.WinControls

Public Class SplashForm
    Private trdSyncTask As Thread
    Private Sub SplashForm_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        RadWaitingBar1.StartWaiting()
        RadPanel1.PanelElement.PanelBorder.Visibility = ElementVisibility.Collapsed
        rdlblVersion.Text = "Version: " & My.Application.Info.Version.ToString
        AddHandler modLicenseServer.evntLoadingmodule, AddressOf EvntHandlingSynchronizeStarted
        rdlblWebServer.Text = modLicenseServer.QlmUrl
        trdSyncTask = New Thread(AddressOf SyncThreadTask)
        trdSyncTask.IsBackground = True
        trdSyncTask.Start()


    End Sub
    Private Sub SyncThreadTask()
        modLicenseServer.LoadLicenseInformation()
    End Sub

    Private Sub EvntHandlingSynchronizeStarted(text As String)
        BeginInvoke(New Action(Of String)(AddressOf InvokeSynchronizeStarted), text)
    End Sub
    Private Sub InvokeSynchronizeStarted(text As String)
        rdlblStatus.Text = text

        If text = "Finished loading" Then
            frmMainLicense.Show()
            Me.Close()
        End If
    End Sub
End Class