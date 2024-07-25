/*
//===============================================================================
// Copyright Kamstrup
// All rights reserved.
//===============================================================================
*/

using System;
using System.Collections.Generic;
using Inspector.Hal.Interfaces;
using Inspector.Hal.Interfaces.Events;
using Inspector.Model.BluetoothDongle;

namespace Inspector.Hal.Stub
{
    /// <summary>
    /// 
    /// </summary>
    public class BluetoothHalStub : IHal
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BluetoothHalStub"/> class.
        /// </summary>
        public BluetoothHalStub()
        {

        }

        /// <summary>
        /// Connects the specified bluetooth API.
        /// </summary>
        /// <param name="connectionProperties">The connection properties.</param>
        /// <param name="allowedBluetoothDongles">The allowed bluetooth dongles.</param>
        /// <exception cref="ConnectionException">Thrown when creation of the connection failed</exception>
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
                case Inspector.Infra.DeviceCommand.FlushManometerCache:
                    OnMessageReceived("TEST");
                    break;
                case Inspector.Infra.DeviceCommand.CheckSCPIInterface:
                    OnMessageReceived("0,\"no current Scpi errors!\"");
                    break;
                case Inspector.Infra.DeviceCommand.CheckPressureUnit:
                    OnMessageReceived("mbar");
                    break;
                case Inspector.Infra.DeviceCommand.SetPressureUnit:
                    OnMessageReceived("ok");
                    break;
                case Inspector.Infra.DeviceCommand.CheckRange:
                    OnMessageReceived("\"2000 mbar\"");
                    break;
                case Inspector.Infra.DeviceCommand.CheckBatteryStatus:
                    OnMessageReceived("60");
                    break;
                case Inspector.Infra.DeviceCommand.InitiateSelfTest:
                    OnMessageReceived(String.Empty);
                    break;
                case Infra.DeviceCommand.Wakeup:
                    OnMessageReceived("\"HM3500DLM110,MOD00B,B030504\"");
                    break;
                case Inspector.Infra.DeviceCommand.CheckIdentification:
                    OnMessageReceived("\"HM3500DLM110,MOD00B,B030504\"");
                    break;
                case Inspector.Infra.DeviceCommand.IRAlwaysOn:
                    OnMessageReceived("ok");
                    break;
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
                case Inspector.Infra.DeviceCommand.SwitchToManometerTH1:
                    OnMessageReceived("ok");
                    break;
                case Inspector.Infra.DeviceCommand.SwitchToManometerTH2:
                    OnMessageReceived("ok");
                    break;
                case Inspector.Infra.DeviceCommand.Connect:
                case Inspector.Infra.DeviceCommand.Disconnect:
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
            throw new NotImplementedException();
        }

        /// <summary>
        /// Stops the continuous measurement.
        /// </summary>
        public void StopContinuousMeasurement()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Occurs when [connected].
        /// </summary>
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

        /// <summary>
        /// Occurs when [connect failed].
        /// </summary>
        public event EventHandler ConnectFailed;

        /// <summary>
        /// Called when [connect failed].
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="errorCode">The error code.</param>
        private void OnConnectFailed(string message, int errorCode = -1)
        {
            if (ConnectFailed != null)
            {
                ConnectFailed(this, new ConnectFailedEventArgs(message, errorCode));
            }
        }

        /// <summary>
        /// Occurs when [disconnected].
        /// </summary>
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

        /// <summary>
        /// Occurs when [message received].
        /// </summary>
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

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            // do nothing
        }

        /// <summary>
        /// Gets or sets a value indicating whether the HAL is signaled that a command is to be expected.
        /// </summary>
        /// <value><c>true</c> if a command is expected; otherwise, <c>false</c>.</value>
        public bool IsBusy { get; set; }

        public List<string> RetrieveAvailableBluetoothDongles(string bluetoothApi)
        {
            throw new NotImplementedException();
        }
    }
}
