/*
//===============================================================================
// Copyright Kamstrup
// All rights reserved.
//===============================================================================
*/

using System;
using System.Collections.Generic;
using System.Threading;
using Inspector.Hal.Interfaces;
using Inspector.Hal.Interfaces.Events;
using Inspector.Model.BluetoothDongle;

namespace Inspector.Hal
{
    /// <summary>
    /// Bluetooth HAL Demo mode
    /// </summary>
    public class BluetoothHalDemoMode : IHal
    {
        #region Constants
        private const int DEFAULT_MEASUREMENT_INTERVAL = 1000;
        #endregion Constants

        #region Class members
        private Timer m_MeasurementTimer;
        private IList<double> m_Measurements;
        private bool m_Disposed = false;
        #endregion Class members

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="BluetoothHalDemoMode"/> class.
        /// </summary>
        public BluetoothHalDemoMode()
        {
            m_MeasurementTimer = new Timer(OnMeasurementTimerTick, null, Timeout.Infinite, Timeout.Infinite);
        }
        #endregion Constructors

        #region IHal
        /// <summary>
        /// Retrieves the available bluetooth dongles.
        /// </summary>
        /// <param name="bluetoothApi">The bluetooth API.</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists")]
        public List<string> RetrieveAvailableBluetoothDongles(string bluetoothApi)
        {
            return new List<string>();
        }

        /// <summary>
        /// Connects the specified bluetooth API.
        /// </summary>
        /// <param name="connectionProperties">The connection properties.</param>
        /// <param name="allowedBluetoothDongles">The allowed bluetooth dongles.</param>
        /// <exception cref="ConnectionException">Thrown when creation of the connection failed</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists")]
        public void Connect(Dictionary<string, string> connectionProperties, List<BluetoothDongleInformation> allowedBluetoothDongles)
        {
            OnConnected();
        }

        /// <summary>
        /// Disconnects the connection.
        /// </summary>
        public void Disconnect()
        {
            OnDisconnected();
        }

        /// <summary>
        /// Sends the command.
        /// </summary>
        /// <param name="command">The command.</param>
        public void SendCommand(Inspector.Infra.DeviceCommand command)
        {
            SendCommand(command, String.Empty);
        }

        /// <summary>
        /// Sends the command.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="commandParameter">The parameter.</param>
        public void SendCommand(Inspector.Infra.DeviceCommand command, string commandParameter)
        {
            System.Threading.Thread.Sleep(250);
            switch (command)
            {
                case Inspector.Infra.DeviceCommand.EnterRemoteLocalCommandMode:
                    OnMessageReceived("ok");
                    break;
                case Inspector.Infra.DeviceCommand.ExitRemoteLocalCommandMode:
                    OnMessageReceived("CONNECT O");
                    break;
                case Inspector.Infra.DeviceCommand.SwitchInitializationLedOn:
                    OnMessageReceived("ok");
                    break;
                case Inspector.Infra.DeviceCommand.SwitchInitializationLedOff:
                    OnMessageReceived("ok");
                    break;
                case Infra.DeviceCommand.FlushBluetoothCache:
                    OnMessageReceived("ok");
                    break;
                case Inspector.Infra.DeviceCommand.FlushManometerCache:
                    OnMessageReceived("\"HM3500DLM110,MOD00B,B030504\"");
                    break;
                case Inspector.Infra.DeviceCommand.CheckManometerPresent:
                    OnMessageReceived("\"HM3500DLM110,MOD00B,B030504\"");
                    break;
                case Inspector.Infra.DeviceCommand.SwitchToManometerTH1:
                    OnMessageReceived("ok");
                    break;
                case Inspector.Infra.DeviceCommand.SwitchToManometerTH2:
                    OnMessageReceived("ok");
                    break;
                case Inspector.Infra.DeviceCommand.CheckBatteryStatus:
                    OnMessageReceived("100");
                    break;
                case Inspector.Infra.DeviceCommand.CheckSCPIInterface:
                    OnMessageReceived("0,\"no current Scpi errors!\"");
                    break;
                case Inspector.Infra.DeviceCommand.InitiateSelfTest:
                    OnMessageReceived(String.Empty);
                    break;
                case Inspector.Infra.DeviceCommand.Wakeup:
                    OnMessageReceived("\"HM3500DLM110,MOD00B,B030504\"");
                    break;
                case Inspector.Infra.DeviceCommand.CheckIdentification:
                    OnMessageReceived("\"HM3500DLM110,MOD00B,B030504\"");
                    break;
                case Inspector.Infra.DeviceCommand.CheckRange:
                    OnMessageReceived("\"17 bar\"");
                    break;
                case Infra.DeviceCommand.IRAlwaysOn:
                    OnMessageReceived("ok");
                    break;
                case Inspector.Infra.DeviceCommand.SetPressureUnit:
                    OnMessageReceived("ok");
                    break;
                case Inspector.Infra.DeviceCommand.CheckPressureUnit:
                    OnMessageReceived("bar");
                    break;
                case Inspector.Infra.DeviceCommand.MeasureContinuously:
                case Inspector.Infra.DeviceCommand.Disconnect:
                case Inspector.Infra.DeviceCommand.Connect:
                case Inspector.Infra.DeviceCommand.None:
                default:
                    break;
            }
        }

        /// <summary>
        /// Starts the continuous measurement.
        /// </summary>
        /// <param name="frequency">The frequency in measurements per second.</param>
        public void StartContinuousMeasurement(int frequency)
        {
            m_Measurements = GenerateMeasurements(frequency);
            m_MeasurementTimer.Change(DEFAULT_MEASUREMENT_INTERVAL, Timeout.Infinite);
            OnContinuousMeasurementStarted();
        }

        /// <summary>
        /// Stops the continuous measurement.
        /// </summary>
        public void StopContinuousMeasurement()
        {
            m_MeasurementTimer.Change(Timeout.Infinite, Timeout.Infinite);

            System.Threading.Thread t = new Thread(() => StopContinuousMeasurementThread());
            t.Name = "BTHAL Demo mode: StopContinuousMeasurementThread";
            t.Start();
        }

        /// <summary>
        /// Gets or sets a value indicating whether the HAL is signaled that a command is to be expected.
        /// </summary>
        /// <value><c>true</c> if a command is expected; otherwise, <c>false</c>.</value>
        public bool IsBusy { get; set; }
        #endregion IHal

        #region IDisposable
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        protected virtual void Dispose(bool disposing)
        {
            if (!m_Disposed)
            {
                if (disposing)
                {
                    if (m_MeasurementTimer != null)
                    {
                        m_MeasurementTimer.Dispose();
                    }
                }
            }

            m_Disposed = true;
        }
        #endregion IDisposable

        #region Timer Handling
        /// <summary>
        /// Called when [measurement timer tick].
        /// </summary>
        /// <param name="state">The state.</param>
        private void OnMeasurementTimerTick(object state)
        {
            OnMeasurementsReceived(m_Measurements);
            m_MeasurementTimer.Change(DEFAULT_MEASUREMENT_INTERVAL, Timeout.Infinite);
        }
        #endregion Timer Handling

        #region Private functions
        /// <summary>
        /// Generates the measurements.
        /// </summary>
        /// <param name="numberOfMeasurements">The number of measurements.</param>
        /// <returns>A list of <paramref="numberOfMeasurements"/> generated measurements</returns>
        private static IList<double> GenerateMeasurements(int numberOfMeasurements)
        {
            IList<double> measurements = new List<double>();
            for (int i = 0; i < numberOfMeasurements; i++)
            {
                measurements.Add(0.0);
            }
            return measurements;
        }

        /// <summary>
        /// Stops the continuous measurement thread.
        /// </summary>
        private void StopContinuousMeasurementThread()
        {
            Thread.Sleep(1500);
            OnContinuousMeasurementStopped();
        }
        #endregion Private functions

        #region Event Handling
        public event EventHandler Connected;

        /// <summary>
        /// Called when [connected].
        /// </summary>
        private void OnConnected()
        {
            if (Connected != null)
            {
                Connected(this, new EventArgs());
            }
        }

        public event EventHandler ConnectFailed;

        /// <summary>
        /// Called when [connect failed].
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="errorCode">The error code.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        private void OnConnectFailed(string message, int errorCode = -1)
        {
            if (ConnectFailed != null)
            {
                ConnectFailed(this, new ConnectFailedEventArgs(message, errorCode));
            }
        }

        public event EventHandler Disconnected;

        /// <summary>
        /// Called when [disconnected].
        /// </summary>
        private void OnDisconnected()
        {
            if (Disconnected != null)
            {
                Disconnected(this, new EventArgs());
            }
        }

        /// <summary>
        /// Occurs when [measurements receieved].
        /// </summary>
        public event EventHandler MeasurementsReceived;

        /// <summary>
        /// Called when [measurements received].
        /// </summary>
        /// <param name="measurements">The measurements.</param>
        private void OnMeasurementsReceived(IList<double> measurements)
        {
            if (MeasurementsReceived != null)
            {
                MeasurementsReceived(this, new MeasurementsReceivedEventArgs(measurements));
            }
        }

        public event EventHandler MessageReceived;

        /// <summary>
        /// Called when [message received].
        /// </summary>
        /// <param name="data">The data.</param>
        private void OnMessageReceived(string data)
        {
            if (MessageReceived != null)
            {
                MessageReceived(this, new MessageReceivedEventArgs(data));
            }
        }

        public event EventHandler MessageReceivedError;

        /// <summary>
        /// Called when [message received].
        /// </summary>
        /// <param name="data">The data.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        private void OnMessageReceivedError(string message, int errorCode)
        {
            if (MessageReceivedError != null)
            {
                MessageReceivedError(this, new MessageErrorEventArgs(message, errorCode));
            }
        }

        /// <summary>
        /// Occurs when [continuous measurement stopped].
        /// </summary>
        public event EventHandler ContinuousMeasurementStopped;

        /// <summary>
        /// Called when [continuous measurement stopped].
        /// </summary>
        private void OnContinuousMeasurementStopped()
        {
            if (ContinuousMeasurementStopped != null)
            {
                ContinuousMeasurementStopped(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Occurs when [continuous measurement stopped].
        /// </summary>
        public event EventHandler ContinuousMeasurementStarted;

        /// <summary>
        /// Called when [continuous measurement stopped].
        /// </summary>
        private void OnContinuousMeasurementStarted()
        {
            if (ContinuousMeasurementStarted != null)
            {
                ContinuousMeasurementStarted(this, EventArgs.Empty);
            }
        }
        #endregion Event Handling

    }
}
