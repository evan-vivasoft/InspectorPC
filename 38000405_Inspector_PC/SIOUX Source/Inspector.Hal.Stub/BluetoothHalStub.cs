/*
//===============================================================================
// Copyright Kamstrup
// All rights reserved.
//===============================================================================
*/

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Inspector.Hal.Interfaces;
using Inspector.Hal.Interfaces.Events;
using Inspector.Hal.Interfaces.Exceptions;
using Inspector.Infra;
using Inspector.Model;
using Inspector.Model.BluetoothDongle;

namespace Inspector.Hal.Stub
{
    /// <summary>
    /// 
    /// </summary>
    public class BluetoothHalStub : IHal
    {
        private Task SendMeasurementsTask { get; set; }
        private List<Measurement> Measurements { get; set; }

        public BluetoothHalStub()
        {
            Measurements = new List<Measurement>();
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
        public void SendCommand(DeviceCommand command)
        {
            SendCommand(command, String.Empty);
        }

        /// <summary>
        /// Sends the command.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="commandParameter">The parameter.</param>
        public void SendCommand(DeviceCommand command, string commandParameter)
        {
            Thread.Sleep(250);

            switch (command)
            {
                case DeviceCommand.FlushManometerCache:
                case DeviceCommand.FlushBluetoothCache:
                case DeviceCommand.CheckManometerPresent:
                    OnMessageReceived("TEST");
                    break;
                case DeviceCommand.CheckSCPIInterface:
                    OnMessageReceived("0,\"no current Scpi errors!\"");
                    break;
                case DeviceCommand.CheckPressureUnit:
                    OnMessageReceived("mbar");
                    break;
                case DeviceCommand.SetPressureUnit:
                    OnMessageReceived("ok");
                    break;
                case DeviceCommand.CheckRange:
                    OnMessageReceived("\"2000 mbar\"");
                    break;
                case DeviceCommand.CheckBatteryStatus:
                    OnMessageReceived("60");
                    break;
                case DeviceCommand.InitiateSelfTest:
                    OnMessageReceived(string.Empty);
                    break;
                case DeviceCommand.CheckIdentification:
                    OnMessageReceived("\"HM3500DLM110,MOD00B,B030504\"");
                    break;
                case DeviceCommand.IRAlwaysOn:
                    OnMessageReceived("ok");
                    break;
                case DeviceCommand.EnterRemoteLocalCommandMode:
                    OnMessageReceived("ok");
                    break;
                case DeviceCommand.ExitRemoteLocalCommandMode:
                    OnMessageReceived("CONNECT O");
                    break;
                case DeviceCommand.SwitchToManometerTH1:
                    OnMessageReceived("ok");
                    break;
                case DeviceCommand.SwitchToManometerTH2:
                    OnMessageReceived("ok");
                    break;
                case DeviceCommand.Connect:
                case DeviceCommand.Disconnect:
                case DeviceCommand.None:
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
            OnContinuousMeasurementStarted();

            SendMeasurementsTask = Task.Factory.StartNew(SendMeasurements);
        }

        public void AddMeasurements(List<Measurement> measurements)
        {
            lock (Measurements)
            {
                Measurements = measurements;
            }
        }

        private void SendMeasurements()
        {
            Thread.Sleep(1000);

            lock (Measurements)
            {
                if (Measurements.Count > 0)
                {
                    OnMeasurementsReceived(Measurements);
                }
            }
        }

        /// <summary>
        /// Stops the continuous measurement.
        /// </summary>
        public void StopContinuousMeasurement()
        {
            OnContinuousMeasurementStopped();
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
            Connected?.Invoke(this, new ConnectedEventArgs(DeviceType.PlexorBluetoothIrDA));
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
            ConnectFailed?.Invoke(this, new ConnectFailedEventArgs(message, errorCode));
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
            Disconnected?.Invoke(this, new EventArgs());
        }

        /// <summary>
        /// Occurs when [measurements receieved].
        /// </summary>
        public event EventHandler MeasurementsReceived;

        /// <summary>
        /// Called when [measurements received].
        /// </summary>
        /// <param name="measurements">The measurements.</param>
        private void OnMeasurementsReceived(IList<Measurement> measurements)
        {
            MeasurementsReceived?.Invoke(this, new MeasurementsReceivedEventArgs(measurements));
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
            MessageReceived?.Invoke(this, new MessageReceivedEventArgs(data));
        }

        public event EventHandler MessageReceivedError;

        /// <summary>
        /// Called when [message received].
        /// </summary>
        private void OnMessageReceivedError(string message, int errorCode)
        {
            MessageReceivedError?.Invoke(this, new MessageErrorEventArgs(message, errorCode));
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
            ContinuousMeasurementStopped?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Occurs when [continuous measurement stopped].
        /// </summary>
        public event EventHandler ContinuousMeasurementStarted;
        public event EventHandler DeviceUnPaired;
        public event EventHandler DeviceUnPairFinished;

        /// <summary>
        /// Called when [continuous measurement stopped].
        /// </summary>
        private void OnContinuousMeasurementStarted()
        {
            ContinuousMeasurementStarted?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            SendMeasurementsTask.Dispose();
            Measurements.Clear();
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

        public void UnPairDevices(string address)
        {
            throw new NotImplementedException();
        }
    }
}
