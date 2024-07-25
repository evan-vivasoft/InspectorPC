'===============================================================================
'Copyright Wigersma 2013
'All rights reserved.
'===============================================================================

'MOD 08 Implement selection of Bluetooth dongle

Imports System.Globalization
Imports Inspector.BusinessLogic.Data.Configuration.Interfaces.Managers
Imports Inspector.BusinessLogic.Interfaces
Imports Inspector.Infra.Ioc
Imports KAM.INSPECTOR.Infra
''' <summary>
''' Getting the Bluetooth Dongle/ PC information
''' Only bluetooth driver BlueSoleil is supported
''' Other drivers give an undefinied behavior
''' </summary>
''' <remarks></remarks>
Public Class usctrl_BluetoothDongle
#Region "Class members"
    Private m_BluetoothDongleInformationManager As IBluetoothDongleInformationManager
#End Region

#Region "Properties"
    ''' <summary>
    ''' Gets or sets the plexor information manager.
    ''' </summary>
    ''' <value>
    ''' The Plexor information manager.
    ''' </value>
    Public Property BluetoothDongleInformationManager() As IBluetoothDongleInformationManager
        Get
            If m_BluetoothDongleInformationManager Is Nothing Then
                m_BluetoothDongleInformationManager = ContextRegistry.Context.Resolve(Of IBluetoothDongleInformationManager)()
            End If
            Return m_BluetoothDongleInformationManager
        End Get
        Set(value As IBluetoothDongleInformationManager)
            m_BluetoothDongleInformationManager = value
        End Set
    End Property

#End Region

#Region "Constructor"
    ''' <summary>
    ''' Create new instance
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()
        ' This call is required by the designer.
        InitializeComponent()

        'Do not change this names
        'MOD 52
        rDropDownBluetoothDriver.Items.Add("BlueSoleil")
        rDropDownBluetoothDriver.Items.Add("Microsoft") 'MOD 76
        '!!!!
        'ALWAYS USE BLUESOLEIL OR MICROSOFT DRIVER INSTEAD.
        'OTHER DRIVERS GIVE UNSTABLE BEHAVIOR
        '!!!!
        'rDropDownBluetoothDriver.Items.Add("Toshiba") 'NOT SUPPORTED;
        'rDropDownBluetoothDriver.Items.Add("WidComm")  'NOT SUPPORTED; 

        ' Add any initialization after the InitializeComponent() call.
    End Sub

    ''' <summary>
    ''' Loading the form. Intitialize theme
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub usctrl_GeneralUI_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim splitString As String = ModuleSettings.SettingFile.GetSetting(GsSectionPlexor, GsSettingPlexorBTDongleApi)
        rDropDownBluetoothDriver.Text = splitString.Substring(2, splitString.Length - 2)

        'MOD 78
        Dim readConfig As String
        readConfig = ModuleSettings.SettingFile.GetSetting(GsSectionPlexor, GsSettingPlexorUnpairBeforeInspection)
        If readConfig.ToUpper = "TRUE" Then
            rdCheckUnpairBefore.Checked = True
        Else
            rdCheckUnpairBefore.Checked = False
        End If

        updateBluetoothAddressList()
        Exit Sub

        'rDropDownBluetoothAddress.Text = ModuleSettings.SettingFile.GetSetting(GsSectionPlexor, GsSettingPlexorBTDongleAddress)
    End Sub

#End Region

#Region "Button handling"
    ''' <summary>
    ''' Handling of Bluetooth api changed
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub rDropDownBluetoothDriver_SelectedIndexChanged(sender As System.Object, e As Telerik.WinControls.UI.Data.PositionChangedEventArgs) Handles rDropDownBluetoothDriver.SelectedIndexChanged
        updateBluetoothAddressList()
    End Sub
    ''' <summary>
    ''' Handling of Bluetooth address changed
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub rDropDownBluetoothAdres_SelectedIndexChanged(sender As System.Object, e As Telerik.WinControls.UI.Data.PositionChangedEventArgs) Handles rDropDownBluetoothAddress.SelectedIndexChanged
        ModuleSettings.SettingFile.SaveSetting(GsSectionPlexor, GsSettingPlexorBTDongleAddress) = rDropDownBluetoothAddress.Text
    End Sub
    ''' <summary>
    ''' Update the list of Bluetooth addresses, depending on the Bluetooth driver
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub updateBluetoothAddressList()
        Dim bluetoothDongleList As System.Collections.ObjectModel.ReadOnlyCollection(Of String) = Nothing
        Dim noDongleFound As Boolean = True
        'MOD 81
        Try
            bluetoothDongleList = BluetoothDongleInformationManager.RetrieveAvailableBluetoothDongles("ba" & rDropDownBluetoothDriver.Text)
            If bluetoothDongleList.Count = 0 Then noDongleFound = True Else noDongleFound = False
        Catch ex As Exception
            noDongleFound = True
        End Try

        If noDongleFound = True Then
            rDropDownBluetoothAddress.DataSource = Nothing
            MsgBox(My.Resources.KamPLEXORresx.str_NoBTDevicePCFound, vbCritical)
        Else
            rDropDownBluetoothAddress.DataSource = bluetoothDongleList
            ModuleSettings.SettingFile.SaveSetting(GsSectionPlexor, GsSettingPlexorBTDongleAddress) = rDropDownBluetoothAddress.Text
            ModuleSettings.SettingFile.SaveSetting(GsSectionPlexor, GsSettingPlexorBTDongleApi) = "ba" & rDropDownBluetoothDriver.Text
        End If
    End Sub
#End Region

    Private Sub rdCheckUnpairBefore_ToggleStateChanged(sender As Object, args As Telerik.WinControls.UI.StateChangedEventArgs) Handles rdCheckUnpairBefore.ToggleStateChanged
        ModuleSettings.SettingFile.SaveSetting(GsSectionPlexor, GsSettingPlexorUnpairBeforeInspection) = rdCheckUnpairBefore.Checked

    End Sub
End Class
