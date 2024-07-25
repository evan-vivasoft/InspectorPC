'===============================================================================
'Copyright Wigersma 21
'All rights reserved.
'===============================================================================

'MOD 47

Imports MediaDevices

Public Module ModAndroidDevice
    Private lConnectedMediaDevice As MediaDevice
    Private lConnectStatus As Boolean

#Region "Android Device connection"
    Public Function ConnectToMediaDevice() As Boolean
        Dim devices = MediaDevice.GetDevices
        If devices.Count = 0 Then Return False
        lConnectedMediaDevice = devices.First

        lConnectedMediaDevice.Connect()
        Return lConnectedMediaDevice.IsConnected

    End Function

    Public Function DisconnectToDevicase()
        lConnectedMediaDevice.Disconnect()
        lConnectStatus = False
    End Function
#End Region
#Region "Properties"
    Public ReadOnly Property ConnectStatus As Boolean
        Get
            Return lConnectedMediaDevice.IsConnected
        End Get
    End Property

    Public ReadOnly Property ConnectedDevice As MediaDevice
        Get
            Return lConnectedMediaDevice
        End Get
    End Property

#End Region

End Module
