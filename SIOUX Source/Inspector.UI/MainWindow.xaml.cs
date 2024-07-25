/*
//===============================================================================
// Copyright Kamstrup
// All rights reserved.
//===============================================================================
*/

using System.Windows;
using System.Windows.Threading;
using Inspector.Hal;
using Inspector.Hal.Interfaces;
using Inspector.Infra.Ioc;
using Inspector.UI.Inspection.ViewModels;
using log4net;

namespace Inspector
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        private static readonly ILog log = LogManager.GetLogger(typeof(MainWindow));
        public MainWindow()
        {
            InitializeComponent();
        
            //Attach a handler to the "unhandled exception" event
            Application.Current.DispatcherUnhandledException += Current_DispatcherUnhandledException;
        }

        //the exception handler that handles the unhandled exceptions
        private void Current_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show(e.Exception.Message);

            //The bluetooth hal has been instantiated by spring, Context.resolve gets the object it instantiated
            var bluetoothHal = ContextRegistry.Context.Resolve<IHal>() as BluetoothHal;
            
            // Send a "endContinuousMeasurement" command to the device, shuts down the serial connection, and destroys the port
            var disconnectSucceeded = bluetoothHal != null && bluetoothHal.ForceConnectionClose();
            if (!disconnectSucceeded)
            {
                MessageBox.Show("disconnecting failed");
            }

            e.Handled = true;

            Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var inspectionViewModel = vwInspection.DataContext as InspectionViewModel;

            inspectionViewModel?.Dispose();

            ContextRegistry.Context.Release();
        }
    }
}
