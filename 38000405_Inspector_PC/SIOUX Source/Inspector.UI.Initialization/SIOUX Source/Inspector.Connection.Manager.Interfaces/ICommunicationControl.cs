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

namespace Inspector.Connection.Manager.Interfaces
{
    /// <summary>
    /// CommandResult delegate
    /// </summary>
    public delegate void CommandResultCallback(bool commandSucceeded, int errorCode, string message);

    /// <summary>
    /// MeasurementResult delegate
    /// </summary>
    public delegate void MeasurementResultCallback(IList<double> measurements);

    /// <summary>
    /// Measurementstarted callback
    /// </summary>
    public delegate void MeasurementStartedCallback();

    /// <summary>
    /// ICommunicationControl
    /// </summary>
    public interface ICommunicationControl : IDisposable
    {
        /// <summary>
        /// Initializes this instance.
        /// </summary>
        void Initialize();

        /// <summary>
        /// Starts the communication.
        /// </summary>
        /// <returns>True if the communication is claimed successfull otherwise false.</returns>
        bool StartCommunication();

        /// <summary>
        /// Stops the communication.
        /// </summary>
        void StopCommunication();

        /// <summary>
        /// Connects the specified connection properties.
        /// </summary>
        /// <param name="connectionProperties">The connection properties.</param>
        /// <param name="allowedBluetoothDongles">The allowed bluetooth dongles.</param>
        /// <param name="commandResultCallback">The command result callback.</param>
        void Connect(Dictionary<string, string> connectionProperties, List<BluetoothDongleInformation> allowedBluetoothDongles, CommandResultCallback commandResultCallback);

        /// <summary>
        /// Disconnects the specified command result.
        /// </summary>
        /// <param name="commandResultCallback">The command result callback.</param>
        void Disconnect(CommandResultCallback commandResultCallback);

        /// <summary>
        /// Sends the command.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="commandParameter">The parameter.</param>
        /// <param name="commandResultCallback">The command result callback.</param>
        void SendCommand(DeviceCommand command, string commandParameter, CommandResultCallback commandResultCallback);

        /// <summary>
        /// Recovers from error.
        /// </summary>
        /// <param name="commandResultCallback">The command result callback.</param>
        void RecoverFromError(CommandResultCallback commandResultCallback);

        /// <summary>
        /// Starts the continuous measurement.
        /// </summary>
        /// <param name="measurementFrequency">The measurement frequency in measurements per second.</param>
        /// <param name="commandResultCallback">The command result callback.</param>
        /// <param name="measurementResultCallback">The measurement result callback.</param>
        void StartContinuousMeasurement(int measurementFrequency, CommandResultCallback commandResultCallback, MeasurementResultCallback measurementResultCallback, MeasurementStartedCallback measurementStartedCallback);

        /// <summary>
        /// Stops the continuous measurement.
        /// </summary>
        /// <param name="commandResultCallback">The command result callback.</param>
        void StopContinuousMeasurement(CommandResultCallback commandResultCallback);
    }
}
