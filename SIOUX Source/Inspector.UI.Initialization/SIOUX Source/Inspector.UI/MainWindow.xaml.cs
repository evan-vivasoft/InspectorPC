/*
//===============================================================================
// Copyright Kamstrup
// All rights reserved.
//===============================================================================
*/

using System.Windows;
using Inspector.UI.Inspection.ViewModels;
using Spring.Context;
using System.Windows.Threading;
using Inspector.Hal;
using Inspector.Infra.Ioc;
using Inspector.Hal.Interfaces;
using log4net;


namespace Inspector
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(MainWindow));
        public MainWindow()
        {
            InitializeComponent();
            //Attach a handler to the "unhandled exception" event
            Application.Current.DispatcherUnhandledException += new DispatcherUnhandledExceptionEventHandler(Current_DispatcherUnhandledException);
        }

        //the exception handler that handles the unhandled exceptions
        void Current_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {

            MessageBox.Show(e.Exception.Message);
            //The bluetooth hal has been instantiated by spring, Context.resolve gets the object it instantiated
            BluetoothHal bluetoothHal = ContextRegistry.Context.Resolve<IHal>() as BluetoothHal;
            //this function sends an "endContinuousMeasurement" command to the device, shuts down the serial connection, and destroys the port
            bool disconnectSucceeded = bluetoothHal.ForceConnectionClose();
            if (!disconnectSucceeded)
            {
                MessageBox.Show("disconnecting failed");
            }

            e.Handled = true;
            this.Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            (vwInspection.DataContext as InspectionViewModel).Dispose();
        }
    }
}
