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
using Inspector.Model;
using Inspector.Model.BluetoothDongle;

namespace Inspector.Hal.Stub
{
    /// <summary>
    /// Bluetooth HAL Sequential stub
    /// </summary>
    public class BluetoothHalSequentialStub : IHal
    {
        #region Constants
        private const int DEFAULT_MEASUREMENT_INTERVAL = 1000;
        private const int DEFAULT_MEASUREMENT_TIMER_TIMEOUT = 3000;

        public const double MIN_MEASUREMENT_VALUE = -1.0;
        public const double MAX_MEASUREMENT_VALUE = 4.0;
        public const double MEASUREMENT_INCREMENT_VALUE = 0.4;
        public static int MEASUREMENT_FREQUENCY = 10;
        #endregion Constants

        #region Class members
        private Stack<string> m_ReactionStack = new Stack<string>();
        private bool m_ConnectError = false;

        private Timer m_MeasurementTimer;
        private Timer m_MeasurementTimeoutTimer;
        private double m_CurrentMeasurementValue;
        private bool m_GeneratedMeasurementUpDirection;
        #endregion Class members

        /// <summary>
        /// Initializes a new instance of the <see cref="BluetoothHalSequentialStub"/> class.
        /// </summary>
        public BluetoothHalSequentialStub()
        {
            m_GeneratedMeasurementUpDirection = true;
            m_CurrentMeasurementValue = MIN_MEASUREMENT_VALUE;
            m_MeasurementTimer = new Timer(OnMeasurementTimerTick, null, Timeout.Infinite, Timeout.Infinite);
            m_MeasurementTimeoutTimer = new Timer(OnMeasurementTimerTimeout, null, Timeout.Infinite, Timeout.Infinite);
        }

        /// <summary>
        /// Called when [measurement timer timeout].
        /// </summary>
        /// <param name="state">The state.</param>
        private void OnMeasurementTimerTimeout(object state)
        {
            OnMessageReceivedError("Timeout", ErrorCodes.HAL_MEASUREMENT_TIMEOUT_RECEIVED);
        }

        private void OnMeasurementTimerTick(object state)
        {
            if (!EnableMeasurementTimeout)
            {
                IList<double> measurements = GenerateMeasurements(MEASUREMENT_FREQUENCY);
                OnMeasurementsReceived(measurements);
                m_MeasurementTimer.Change(DEFAULT_MEASUREMENT_INTERVAL, Timeout.Infinite);
                m_MeasurementTimeoutTimer.Change(DEFAULT_MEASUREMENT_TIMER_TIMEOUT, Timeout.Infinite);
            }
        }

        /// <summary>
        /// Generates the measurements.
        /// </summary>
        /// <param name="numberOfMeasurements">The number of measurements.</param>
        /// <returns>A list of <paramref="numberOfMeasurements"/> generated measurements</returns>
        private IList<double> GenerateMeasurements(int numberOfMeasurements)
        {
            IList<double> measurements = new List<double>();
            for (int i = 0; i < numberOfMeasurements; i++)
            {
                measurements.Add(m_CurrentMeasurementValue);
                if (m_CurrentMeasurementValue >= MAX_MEASUREMENT_VALUE)
                {
                    m_GeneratedMeasurementUpDirection = false;
                }
                else if (m_CurrentMeasurementValue <= MIN_MEASUREMENT_VALUE)
                {
                    m_GeneratedMeasurementUpDirection = true;
                }

                if (m_GeneratedMeasurementUpDirection)
                {
                    m_CurrentMeasurementValue += MEASUREMENT_INCREMENT_VALUE;
                }
                else
                {
                    m_CurrentMeasurementValue -= MEASUREMENT_INCREMENT_VALUE;
                }
            }
            return measurements;
        }

        /// <summary>
        /// Sets the reaction stack.
        /// </summary>
        /// <param name="reactionStack">The reaction stack.</param>
        public void SetUpReactionStack(Stack<string> reactionStack)
        {
            // Default Connect and Disconnect will succeed
            m_ConnectError = false;
            m_ReactionStack = reactionStack;
        }

        /// <summary>
        /// Sets the connect error.
        /// </summary>
        /// <param name="connectError">if set to <c>true</c> [connect error].</param>
        public void SetConnectError(bool connectError)
        {
            m_ConnectError = connectError;
        }

        /// <summary>
        /// Gets or sets a value indicating whether [enable measurement timeout].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [enable measurement timeout]; otherwise, <c>false</c>.
        /// </value>
        public bool EnableMeasurementTimeout { get; set; }

        /// <summary>
        /// Connects the specified bluetooth API.
        /// </summary>
        /// <param name="connectionProperties">The connection properties.</param>
        /// <param name="allowedBluetoothDongles">The allowed bluetooth dongles.</param>
        /// <exception cref="ConnectionException">Thrown when creation of the connection failed</exception>
        public void Connect(Dictionary<string, string> connectionProperties, List<BluetoothDongleInformation> allowedBluetoothDongles)
        {
            if (m_ConnectError)
            {
                OnConnectFailed("Connect failed.", 100);
            }
            else
            {
                OnConnected();
            }
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
            // Only send a reply when the stack is not empty.
            if (m_ReactionStack.Count > 0)
            {
                // 3 options: 
                // if m_ReactionStack.Pop().Equals("Timeout") Send an error with a timeout
                // if m_ReactionStack.Pop().Equals("Error:<ec>") Send an error with errorCode <ec>
                // othw Send OnMessageReceived
                string reaction = m_ReactionStack.Pop();
                if (reaction.StartsWith("ManometerError:"))
                {
                    OnMessageReceivedError("Simulated manometer error", int.Parse(reaction.Substring(15)));
                }
                else if (reaction.Equals("Timeout", StringComparison.OrdinalIgnoreCase))
                {
                    OnMessageReceivedError(reaction, ErrorCodes.HAL_COMMAND_TIMEOUT_RECEIVED);
                }
                else if (reaction.StartsWith("Error:", StringComparison.OrdinalIgnoreCase))
                {
                    OnMessageReceivedError(reaction, int.Parse(reaction.Substring(6)));
                }
                else
                {
                    OnMessageReceived(reaction);
                }
            }
        }

        /// <summary>
        /// Starts the continuous measurement.
        /// </summary>
        /// <param name="frequency">The frequency in measurements per second.</param>
        public void StartContinuousMeasurement(int frequency)
        {
            m_MeasurementTimer.Change(DEFAULT_MEASUREMENT_INTERVAL, Timeout.Infinite);
            m_MeasurementTimeoutTimer.Change(DEFAULT_MEASUREMENT_TIMER_TIMEOUT, Timeout.Infinite);
            OnContinuousMeasurementStarted();
        }

        /// <summary>
        /// Stops the continuous measurement.
        /// </summary>
        public void StopContinuousMeasurement()
        {
            m_MeasurementTimer.Change(Timeout.Infinite, Timeout.Infinite);
            m_MeasurementTimeoutTimer.Change(Timeout.Infinite, Timeout.Infinite);

            System.Threading.Thread t = new Thread(() => StopContinuousMeasurementThread());
            t.Name = "BTHAL Sequential stub: OnContinuousMeasurementStopped";
            t.Start();
        }

        private void StopContinuousMeasurementThread()
        {
            Thread.Sleep(1500);
            OnContinuousMeasurementStopped();
        }

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
        /// Generates the error.
        /// </summary>
        public void GenerateError()
        {
            OnMessageReceivedError("Statemachine error.", 1103);
        }

        /// <summary>
        /// Gets or sets a value indicating whether the HAL is signaled that a command is to be expected.
        /// </summary>
        /// <value><c>true</c> if a command is expected; otherwise, <c>false</c>.</value>
        public bool IsBusy { get; set; }

        public List<string> RetrieveAvailableBluetoothDongles(string bluetoothApi)
        {
            List<string> dongles = new List<string>();
            dongles.Add("(00:00:00:00:00:00)");
            return dongles;
        }
    }
}
