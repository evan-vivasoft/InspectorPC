Imports Inspector.hal
Imports Inspector.Infra.Ioc
Imports Inspector.Hal.Interfaces
Namespace My

    ' The following events are available for MyApplication:
    ' 
    ' Startup: Raised when the application starts, before the startup form is created.
    ' Shutdown: Raised after all application forms are closed.  This event is not raised if the application terminates abnormally.
    ' UnhandledException: Raised if the application encounters an unhandled exception.
    ' StartupNextInstance: Raised when launching a single-instance application and the application is already active. 
    ' NetworkAvailabilityChanged: Raised when the network connection is connected or disconnected.
    'MOD 22
    Partial Friend Class MyApplication
        Private Sub MyApplication_UnhandledException(ByVal sender As Object, ByVal e As Microsoft.VisualBasic.ApplicationServices.UnhandledExceptionEventArgs) Handles Me.UnhandledException
            ' If the user clicks No, then exit.
            ' e.ExitApplication = 

            MsgBox("Unhandled Exception: " & e.Exception.Message & vbCrLf, vbCritical)
            'The bluetooth hal has been instantiated by spring, Context.resolve gets the object it instantiated
            Dim bluetoothHal As BluetoothHal = TryCast(ContextRegistry.Context.Resolve(Of IHal)(), BluetoothHal)
            'this function sends an "endContinuousMeasurement" command to the device, shuts down the serial connection, and destroys the port
            Dim disconnectSucceeded As Boolean = bluetoothHal.ForceConnectionClose()
            If Not disconnectSucceeded Then
                MsgBox("disconnecting failed")
            End If
            ContextRegistry.Context.Release()
            End


        End Sub



    End Class


End Namespace

