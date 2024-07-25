/*
//===============================================================================
// Copyright Kamstrup
// All rights reserved.
//===============================================================================
*/

using System;
using System.Collections.Generic;
using Inspector.Infra;
using Inspector.Model.BluetoothDongle;

namespace Inspector.Hal.Interfaces
{
    /// <summary>
    /// Interface of the HAL
    /// </summary>
    public interface IHal : IDisposable
    {
        /// <summary>
        /// Retrieves the available bluetooth dongles.
        /// </summary>
        /// <param name="bluetoothApi">The bluetooth API.</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists")]
        List<string> RetrieveAvailableBluetoothDongles(string bluetoothApi);

        /// <summary>
        /// Gets or sets a value indicating whether the HAL is signaled that a command is to be expected.
        /// </summary>
        /// <value><c>true</c> if a command is expected; otherwise, <c>false</c>.</value>
        bool IsBusy { get; set; }

        /// <summary>
        /// Connects the specified bluetooth API.
        /// </summary>
        /// <param name="connectionProperties">The connection properties.</param>
        /// <param name="allowedBluetoothDongles">The allowed bluetooth dongles.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists")]
        void Connect(Dictionary<string, string> connectionProperties, List<BluetoothDongleInformation> allowedBluetoothDongles);

        /// <summary>
        /// Disconnects the connection.
        /// </summary>
        void Disconnect();

        /// <summary>
        /// Sends the command.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="commandParameter">The parameter.</param>
        void SendCommand(DeviceCommand command);

        /// <summary>
        /// Sends the command.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="commandParameter">The parameter.</param>
        void SendCommand(DeviceCommand command, string commandParameter);

        /// <summary>
        /// Starts the continuous measurement.
        /// </summary>
        /// <param name="frequency">The frequency in measurements per second.</param>
        void StartContinuousMeasurement(int frequency);

        /// <summary>
        /// Stops the continuous measurement.
        /// </summary>
        void StopContinuousMeasurement();

        /// <summary>
        /// Occurs when [connected].
        /// </summary>
        event EventHandler Connected;

        /// <summary>
        /// Occurs when [connect failed].
        /// </summary>
        event EventHandler ConnectFailed;

        /// <summary>
        /// Occurs when [disconnected].
        /// </summary>
        event EventHandler Disconnected;

        /// <summary>
        /// Occurs when [message received].
        /// </summary>
        event EventHandler MessageReceived;

        /// <summary>
        /// Occurs when [measurements received].
        /// </summary>
        event EventHandler MeasurementsReceived;

        /// <summary>
        /// Occurs when [message received error].
        /// </summary>
        event EventHandler MessageReceivedError;

        /// <summary>
        /// Occurs when [continuous measurement stopped].
        /// </summary>
        event EventHandler ContinuousMeasurementStopped;

        /// <summary>
        /// Occurs when [continuous measurement started]
        /// </summary>
        event EventHandler ContinuousMeasurementStarted;
    }
}
