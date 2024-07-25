/*
//===============================================================================
// Copyright Kamstrup
// All rights reserved.
//===============================================================================
*/

using System;
using System.Collections.Generic;
using Common.Logging;
using Inspector.Connection.EventTranslation;
using Inspector.Connection.Manager.Interfaces;
using Inspector.Connection.SM.Exceptions;
using Inspector.Infra;
using Inspector.Infra.Ioc;
using Inspector.Model;
using Inspector.Model.BluetoothDongle;

namespace Inspector.Connection.Manager
{
    /// <summary>
    /// CommunicationControl
    /// </summary>
    public class CommunicationControl : ICommunicationControl
    {
        #region Constants
        private const string COMMUNICATION_NOT_YET_CLAIMED = "CommunicationControl has not yet been claimed.";
        private const string STATEMACHINE_ERROR = "Statemachine error.";
        private readonly ILog Log = LogManager.GetLogger(typeof (CommunicationControl));
        private static readonly ILog CommunicationLogger = LogManager.GetLogger("CommunicationLogger");

        #endregion Constants

        #region Class Members
        private IConnectionStateMachine m_StateMachine;
        private static object s_CommunicationClaimLock = new object();
        private bool m_Disposed = false;
        private bool m_IsInitialized = false;
        #endregion Class Members

        #region Properties
        /// <summary>
        /// Gets the connection state machine.
        /// </summary>
        /// <value>The connection state machine.</value>
        public IConnectionStateMachine ConnectionStateMachine
        {
            get
            {
                if (m_StateMachine == null)
                {
                    m_StateMachine = ContextRegistry.Context.Resolve<IConnectionStateMachine>();
                }
                return m_StateMachine;
            }
        }

        /// <summary>
        /// Gets a value indicating whether [communication claimed].
        /// For unit testing only
        /// </summary>
        /// <value>
        ///   <c>true</c> if [communication claimed]; otherwise, <c>false</c>.
        /// </value>
        public bool CommunicationClaimed { get; private set; }

        /// <summary>
        /// Gets or sets the command result callback.
        /// </summary>
        /// <value>The command result callback.</value>
        public ConnectionResultCallback ConnectionResultCallback { get; internal set; }

        /// <summary>
        /// Gets or sets the command result callback.
        /// </summary>
        /// <value>The command result callback.</value>
        public CommandResultCallback CommandResultCallback { get; internal set; }

        /// <summary>
        /// Gets or sets the measurement result callback.
        /// </summary>
        /// <value>The measurement result callback.</value>
        public MeasurementResultCallback MeasurementResultCallback { get; private set; }

        /// <summary>
        /// Gets or sets the measurement result callback.
        /// </summary>
        /// <value>The measurement result callback.</value>
        public MeasurementStartedCallback MeasurementStartedCallback { get; private set; }

        /// <summary>
        /// Gets or sets the Device UnPair Finished callback.
        /// </summary>
        /// <value>The Device UnPair Finished callback.</value>
        public DeviceUnPairFinishedCallback DeviceUnPairFinishedCallback { get; private set; }
        
        #endregion Properties

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="CommunicationControl"/> class.
        /// </summary>
        public CommunicationControl()
        {
        }

        #endregion Constructors

        #region Public
        /// <summary>
        /// Initializes this instance.
        /// </summary>
        public void Initialize()
        {
            if (!m_IsInitialized)
            {
                AttachStatemachineEvents();
                ConnectionStateMachine.Initialize();
                EventToCallConnectionStateMachine eventToCallConnectionStateMachine = new EventToCallConnectionStateMachine(ConnectionStateMachine);
                eventToCallConnectionStateMachine.Initialize();
                ConnectionStateMachine.EventToCallConnectionStateMachine = eventToCallConnectionStateMachine;
                m_IsInitialized = true;
            }
        }
        #endregion Public

        #region Callbacks

        /// <summary>
        /// Sends the call back.
        /// </summary>
        /// <param name="commandSucceeded">if set to <c>true</c> [command succeeded].</param>
        /// <param name="errorCode">The error code.</param>
        /// <param name="message">The message.</param>
        /// <param name="deviceType">The device type.</param>
        private void SendCallback(bool commandSucceeded, int errorCode, string message, DeviceType deviceType)
        {
            if (!commandSucceeded)
            {
                Log.Error(string.Format("Connection failed with errorcode '{0}' and message '{1}'", errorCode, message));
            }
            if (ConnectionResultCallback != null)
            {
                ConnectionResultCallback(commandSucceeded, errorCode, message, deviceType);
            }
        }

        /// <summary>
        /// Sends the call back.
        /// </summary>
        /// <param name="commandSucceeded">if set to <c>true</c> [command succeeded].</param>
        /// <param name="errorCode">The error code.</param>
        /// <param name="message">The message.</param>
        private void SendCallback(bool commandSucceeded, int errorCode, string message)
        {
            if (!commandSucceeded)
            {
                Log.Error(string.Format("Command failed with errorcode '{0}' and message '{1}'",errorCode,message));
            }
            if (CommandResultCallback != null)
            {
                CommandResultCallback(commandSucceeded, errorCode, message);
            }
        }

        /// <summary>
        /// Sends the callback.
        /// </summary>
        /// <param name="measurements">The measurements.</param>
        private void SendCallback(IList<Measurement> measurements)
        {
            if (MeasurementResultCallback != null)
            {
                MeasurementResultCallback(measurements);
            }
        }
        #endregion Callbacks

        #region Private functions
        /// <summary>
        /// Attaches the statemachine events.
        /// </summary>
        private void AttachStatemachineEvents()
        {
            ConnectionStateMachine.Connected += ConnectionStateMachineContext_Connected;
            ConnectionStateMachine.Disconnected += ConnectionStateMachineContext_Disconnected;
            ConnectionStateMachine.ConnectFailed += ConnectionStateMachineContext_ConnectFailed;
            ConnectionStateMachine.MessageReceived += ConnectionStateMachineContext_MessageReceived;
            ConnectionStateMachine.Error += ConnectionStateMachineContext_Error;
            ConnectionStateMachine.RecoveredFromError += ConnectionStateMachineContext_RecoveredFromError;
            ConnectionStateMachine.MeasurementReceived += ConnectionStateMachineContext_MeasurementReceived;
            ConnectionStateMachine.ContinuousMeasurementStopped += ConnectionStateMachine_ContinuousMeasurementStopped;
            ConnectionStateMachine.ContinuousMeasurementStarted += ConnectionStateMachine_ContinuousMeasurementStarted;
            ConnectionStateMachine.DeviceUnPaired += ConnectionStateMachine_DeviceUnPaired;
            ConnectionStateMachine.DeviceUnPairFinished += ConnectionStateMachine_DeviceUnPairFinished;
        }






        /// <summary>
        /// Detaches the statemachine events.
        /// </summary>
        internal void DetachStatemachineEvents()
        {
            ConnectionStateMachine.DeviceUnPairFinished -= ConnectionStateMachine_DeviceUnPairFinished;
            ConnectionStateMachine.DeviceUnPaired -= ConnectionStateMachine_DeviceUnPaired;
            ConnectionStateMachine.ContinuousMeasurementStarted -= ConnectionStateMachine_ContinuousMeasurementStarted;
            ConnectionStateMachine.ContinuousMeasurementStopped -= ConnectionStateMachine_ContinuousMeasurementStopped;
            ConnectionStateMachine.MeasurementReceived -= ConnectionStateMachineContext_MeasurementReceived;
            ConnectionStateMachine.RecoveredFromError -= ConnectionStateMachineContext_RecoveredFromError;
            ConnectionStateMachine.Error -= ConnectionStateMachineContext_Error;
            ConnectionStateMachine.MessageReceived -= ConnectionStateMachineContext_MessageReceived;
            ConnectionStateMachine.ConnectFailed -= ConnectionStateMachineContext_ConnectFailed;
            ConnectionStateMachine.Disconnected -= ConnectionStateMachineContext_Disconnected;
            ConnectionStateMachine.Connected -= ConnectionStateMachineContext_Connected;
        }
        #endregion Private functions

        #region Event Handlers
        /// <summary>
        /// Handles the Error event of the m_StateMachine control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Inspector.Connection.ErrorEventArgs"/> instance containing the event data.</param>
        void ConnectionStateMachineContext_Error(object sender, ErrorEventArgs e)
        {
            SendCallback(false, e.ErrorCode, e.Message);
        }

        /// <summary>
        /// Handles the MessageReceived event of the m_StateMachine control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Inspector.Connection.MessageReceivedEventArgs"/> instance containing the event data.</param>
        void ConnectionStateMachineContext_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            SendCallback(true, 0, e.Data);
        }

        /// <summary>
        /// Handles the ConnectFailed event of the m_StateMachine control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Inspector.Connection.ConnectFailedEventArgs"/> instance containing the event data.</param>
        void ConnectionStateMachineContext_ConnectFailed(object sender, ConnectFailedEventArgs e)
        {
            SendCallback(false, e.ErrorCode, e.Message, DeviceType.Unknown);
        }

        /// <summary>
        /// Handles the Disconnected event of the m_StateMachine control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Inspector.Connection.DisconnectedEventArgs"/> instance containing the event data.</param>
        void ConnectionStateMachineContext_Disconnected(object sender, DisconnectedEventArgs e)
        {
            // Send Disconnected event

            SendCallback(true, 0, String.Empty, DeviceType.Unknown);
        }

        /// <summary>
        /// Handles the Connected event of the m_StateMachine control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Inspector.Connection.ConnectedEventArgs"/> instance containing the event data.</param>
        void ConnectionStateMachineContext_Connected(object sender, ConnectedEventArgs e)
        {
            // Send Connected event

            SendCallback(true, 0, string.Empty, e.DeviceType);
        }

        /// <summary>
        /// Handles the RecoveredFromError event of the ConnectionStateMachine control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Inspector.Connection.RecoveredFromErrorEventArgs"/> instance containing the event data.</param>
        void ConnectionStateMachineContext_RecoveredFromError(object sender, RecoveredFromErrorEventArgs e)
        {
            SendCallback(true, 0, string.Empty);
        }

        /// <summary>
        /// Handles the MeasurementReceived event of the ConnectionStateMachineContext control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Inspector.Connection.MeasurementEventArgs"/> instance containing the event data.</param>
        void ConnectionStateMachineContext_MeasurementReceived(object sender, MeasurementEventArgs e)
        {
            SendCallback(e.Measurements);
        }

        /// <summary>
        /// Handles the ContinuousMeasurementStopped event of the ConnectionStateMachine control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void ConnectionStateMachine_ContinuousMeasurementStopped(object sender, EventArgs e)
        {
            SendCallback(true, 0, String.Empty);
        }

        /// <summary>
        /// Handles the ContinuousMeasurementStarted event of the ConnectionStateMachine control.
        /// </summary>
        /// <param name="sender">The source of the event</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void ConnectionStateMachine_ContinuousMeasurementStarted(object sender, EventArgs e)
        {
            MeasurementStartedCallback();
        }

        private void ConnectionStateMachine_DeviceUnPaired(object sender, EventArgs e)
        {
            SendCallback(true, 0,(e as DeviceUnPairedEventArgs).Address);
        }

        private void ConnectionStateMachine_DeviceUnPairFinished(object sender, EventArgs e)
        {
            DeviceUnPairFinishedCallback?.Invoke();
        }

        #endregion Event Handlers

        #region ICommunicationControl
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
        protected virtual void Dispose(bool disposing)
        {
            if (!m_Disposed)
            {
                if (disposing)
                {
                    if (m_StateMachine != null)
                    {
                        DetachStatemachineEvents();
                        m_StateMachine.Dispose();
                    }
                }
            }

            m_Disposed = true;
        }

        /// <summary>
        /// Starts the communication.
        /// </summary>
        public bool StartCommunication()
        {
            bool communicationClaimSucceeded = false;

            lock (s_CommunicationClaimLock)
            {
                if (!CommunicationClaimed)
                {
                    CommunicationClaimed = true;
                    communicationClaimSucceeded = true;
                }
            }

            return communicationClaimSucceeded;
        }

        /// <summary>
        /// Stops the communication.
        /// </summary>
        public void StopCommunication()
        {
            lock (s_CommunicationClaimLock)
            {
                if (CommunicationClaimed)
                {
                    CommunicationClaimed = false;
                }
            }
        }

        /// <summary>
        /// Connects the specified connection properties.
        /// </summary>
        /// <param name="connectionProperties">The connection properties.</param>
        /// <param name="allowedBluetoothDongles">The allowed bluetooth dongles.</param>
        /// <param name="connectionResultCallback">The connect result.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Inspector.BusinessLogic.CommunicationControl.SendCallback(System.Boolean,System.Int32,System.String)")]
        public void Connect(Dictionary<string, string> connectionProperties, List<BluetoothDongleInformation> allowedBluetoothDongles, ConnectionResultCallback connectionResultCallback)
        {
            ConnectionResultCallback = connectionResultCallback;
            
            if (CommunicationClaimed)
            {
                try
                {
                    ConnectionStateMachine.ConnectTrigger(connectionProperties, allowedBluetoothDongles);
                }
                catch (StateMachineException)
                {
                    SendCallback(false, ErrorCodes.COMMUNICATIONCONTROL_STATEMACHINE_ERROR, STATEMACHINE_ERROR, DeviceType.Unknown);
                }
            }
            else
            {
                SendCallback(false, ErrorCodes.COMMUNICATIONCONTROL_NOT_YET_CLAIMED, COMMUNICATION_NOT_YET_CLAIMED, DeviceType.Unknown);
            }
        }

        /// <summary>
        /// Disconnects the specified command result.
        /// </summary>
        /// <param name="connectionResultCallback">The connect result.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Inspector.BusinessLogic.CommunicationControl.SendCallback(System.Boolean,System.Int32,System.String)")]
        public void Disconnect(ConnectionResultCallback connectionResultCallback)
        {
            ConnectionResultCallback = connectionResultCallback;

            if (CommunicationClaimed)
            {
                try
                {
                    ConnectionStateMachine.DisconnectTrigger();
                }
                catch (StateMachineException)
                {
                    SendCallback(false, ErrorCodes.COMMUNICATIONCONTROL_STATEMACHINE_ERROR, STATEMACHINE_ERROR, DeviceType.Unknown);
                }
            }
            else
            {
                SendCallback(false, ErrorCodes.COMMUNICATIONCONTROL_NOT_YET_CLAIMED, COMMUNICATION_NOT_YET_CLAIMED, DeviceType.Unknown);
            }
        }

        /// <summary>
        /// Sends the command.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="commandParameter">The parameter.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Inspector.BusinessLogic.CommunicationControl.SendCallback(System.Boolean,System.Int32,System.String)")]
        public void SendCommand(DeviceCommand command, string commandParameter, CommandResultCallback commandResultCallback)
        {
            CommandResultCallback = commandResultCallback;
            if (CommunicationClaimed)
            {
                try
                {
                    ConnectionStateMachine.SendCommandTrigger(command, commandParameter);
                }
                catch (StateMachineException)
                {
                    SendCallback(false, ErrorCodes.COMMUNICATIONCONTROL_STATEMACHINE_ERROR, STATEMACHINE_ERROR);
                }
            }
            else
            {
                SendCallback(false, ErrorCodes.COMMUNICATIONCONTROL_NOT_YET_CLAIMED, COMMUNICATION_NOT_YET_CLAIMED);
            }
        }

        /// <summary>
        /// Recovers from error.
        /// </summary>
        /// <param name="commandResultCallback">The command result callback.</param>
        public void RecoverFromError(CommandResultCallback commandResultCallback)
        {
            CommandResultCallback = commandResultCallback;
            if (CommunicationClaimed)
            {
                try
                {
                    ConnectionStateMachine.NonFatalErrorTrigger();
                }
                catch (StateMachineException)
                {
                    SendCallback(false, ErrorCodes.COMMUNICATIONCONTROL_STATEMACHINE_ERROR, STATEMACHINE_ERROR);
                }
            }
            else
            {
                SendCallback(false, ErrorCodes.COMMUNICATIONCONTROL_NOT_YET_CLAIMED, COMMUNICATION_NOT_YET_CLAIMED);
            }
        }

        /// <summary>
        /// Starts the continuous measurement.
        /// </summary>
        /// <param name="measurementFrequency">The measurement frequency in measurements per second.</param>
        /// <param name="commandResultCallback">The command result callback.</param>
        /// <param name="measurementResultCallback">The measurement result callback.</param>
        public void StartContinuousMeasurement(int measurementFrequency, CommandResultCallback commandResultCallback, MeasurementResultCallback measurementResultCallback, MeasurementStartedCallback measurementStartedCallback)
        {
            CommandResultCallback = commandResultCallback;
            MeasurementResultCallback = measurementResultCallback;
            MeasurementStartedCallback = measurementStartedCallback;
            if (CommunicationClaimed)
            {
                try
                {
                    ConnectionStateMachine.StartContinuousMeasurementTrigger(measurementFrequency);
                }
                catch (StateMachineException)
                {
                    CommandResultCallback(false, ErrorCodes.COMMUNICATIONCONTROL_STATEMACHINE_ERROR, STATEMACHINE_ERROR);
                }
            }
            else
            {
                SendCallback(false, ErrorCodes.COMMUNICATIONCONTROL_NOT_YET_CLAIMED, COMMUNICATION_NOT_YET_CLAIMED);
            }
        }

        /// <summary>
        /// Stops the continuous measurement.
        /// </summary>
        public void StopContinuousMeasurement(CommandResultCallback commandResultCallback)
        {
            System.Diagnostics.Debug.WriteLine("Communication Control: Stop Continuous Measurement");
            CommandResultCallback = commandResultCallback;
            if (CommunicationClaimed)
            {
                try
                {
                    ConnectionStateMachine.StopContinuousMeasurementTrigger();
                }
                catch (StateMachineException ex)
                {
                    CommunicationLogger.Debug("StopcontinuousMeasurement Failed with a statemachine exception: " + ex.Message, ex);
                    CommandResultCallback(false, ErrorCodes.COMMUNICATIONCONTROL_STATEMACHINE_ERROR, STATEMACHINE_ERROR);
                }
            }
            else
            {
                SendCallback(false, ErrorCodes.COMMUNICATIONCONTROL_NOT_YET_CLAIMED, COMMUNICATION_NOT_YET_CLAIMED);
            }
        }

        public void UnpairDevices(string address, CommandResultCallback callback, DeviceUnPairFinishedCallback finishedCallback)
        {
            CommandResultCallback = callback;
            DeviceUnPairFinishedCallback = finishedCallback;
            try
            {
                ConnectionStateMachine.UnPairDevicesTrigger(address);
            }
            catch(StateMachineException ex)
            {
                CommunicationLogger.Debug("UnPairDevices Failed with a statemachine exception: " + ex.Message, ex);
                callback(false, ErrorCodes.COMMUNICATIONCONTROL_STATEMACHINE_ERROR, STATEMACHINE_ERROR);
                finishedCallback();
            }
            catch(Exception ex)
            {
                CommunicationLogger.Debug("UnPairDevices Failed with an unknown error: " + ex.Message, ex);
                callback(false, ErrorCodes.HAL_UNEXPECTED_ERROR, ex.Message);
                finishedCallback();
            }
        }


        #endregion ICommunicationControl
    }
}
