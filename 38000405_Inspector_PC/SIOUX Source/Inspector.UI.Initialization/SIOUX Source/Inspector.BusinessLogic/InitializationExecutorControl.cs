/*
//===============================================================================
// Copyright Kamstrup
// All rights reserved.
//===============================================================================
*/

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using Inspector.BusinessLogic.Data.Configuration.Interfaces.Exceptions;
using Inspector.BusinessLogic.Data.Configuration.Interfaces.Managers;
using Inspector.BusinessLogic.Exceptions;
using Inspector.BusinessLogic.Interfaces.Events;
using Inspector.Connection.Manager.Interfaces;
using Inspector.Hal.Infra;
using Inspector.Infra;
using Inspector.Infra.Ioc;
using Inspector.Infra.Utils;
using Inspector.Model;
using Inspector.Model.BluetoothDongle;
using KAM.INSPECTOR.Infra;

namespace Inspector.BusinessLogic
{
    /// <summary>
    /// InitializationExecutorControl
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable")]
    internal class InitializationExecutorControl
    {
        #region Constants
        private const string SETTING_PLEXOR = "PLEXOR";
        private const string SETTING_MANOMETERTH1 = "MANOMETER_TH1";
        private const string SETTING_MANOMETERTH2 = "MANOMETER_TH2";
        private const string SETTING_BATTERY_LIMIT = "BatteryLimit";
        private const string SETTING_INITIALIZATION_LED_ADDRESS = "InitializationLedAddress";
        private const string SETTING_UNIT = "Unit";
        private const string SETTING_RETURN_NO_VALUE = "<NO VALUE>";
        private const string SETTING_BLUETOOTH_DONGLE_ADDRESS = "BluetoothDongleAddress";
        private const string SETTING_CATEGORY = "PLEXOR";
        #endregion Constants

        #region Class Members

        private object m_CommandStackLock = new object();

        private Stack<DeviceCommand> m_InitializationStack = new Stack<DeviceCommand>();
        private DeviceCommand m_CurrentCommand = DeviceCommand.None;
        private ICommunicationControl m_CommunicationControl;
        private IBluetoothDongleInformationManager m_BluetoothDongleInformationManager;
        private IStationInformationManager m_StationInformationManager;

        private InitializationResult m_InitializationOverallResult = InitializationResult.UNSET;
        private int m_InitializationOverallErrorCode = ErrorCodes.INITIALIZATION_FINISHED_SUCCESSFULLY;

        private string m_DeviceRangeUnit = String.Empty;
        private InitializationManometer m_CurrentManometer = InitializationManometer.UNSET;

        private int m_ScpiRetries = 0;
        private int m_ScpiMaxRetries = 5; // Max 6 tries -> 5 retries allowed

        private ManualResetEvent m_RecoverFromErrorResetEvent = new ManualResetEvent(false);
        private int m_RecoverFromErrorResetEventTimeout = 1000;
        #endregion Class Members

        #region Events
        /// <summary>
        /// Occurs when [initialization step started].
        /// </summary>
        public event EventHandler InitializationStepStarted;

        /// <summary>
        /// Occurs when [initialization step finished].
        /// </summary>
        public event EventHandler InitializationStepFinished;

        /// <summary>
        /// Occurs when [initialization finished].
        /// </summary>
        public event EventHandler InitializationFinished;
        #endregion Events

        #region Event Handlers
        /// <summary>
        /// Called when [initialization step started].
        /// </summary>
        /// <param name="id">The id.</param>
        protected void OnInitializationStepStarted(DeviceCommand id)
        {
            if (InitializationStepStarted != null)
            {
                InitializationStepStarted(this, new StartInitializationStepEventArgs(id.ToString()));
            }
        }

        /// <summary>
        /// Called when [initialization step finished].
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="result">The result.</param>
        /// <param name="message">The message.</param>
        /// <param name="errorCode">The error code.</param>
        protected void OnInitializationStepFinished(DeviceCommand id, InitializationStepResult result, string message, int errorCode)
        {
            if (InitializationStepFinished != null)
            {
                InitializationStepFinished(this, new FinishInitializationStepEventArgs(id.ToString(), result, message, errorCode, m_CurrentManometer));
            }
        }

        /// <summary>
        /// Called when [initialization finished].
        /// </summary>
        /// <param name="message">The message.</param>
        protected void OnInitializationFinished()
        {

            if (InitializationFinished != null)
            {
                InitializationFinished(this, new FinishInitializationEventArgs(m_InitializationOverallResult, m_InitializationOverallErrorCode));
            }
        }
        #endregion Event Handlers

        #region Properties
        /// <summary>
        /// Gets or sets the station information manager.
        /// </summary>
        /// <value>
        /// The station information manager.
        /// </value>
        public IStationInformationManager StationInformationManager
        {
            get
            {
                if (m_StationInformationManager == null)
                {
                    m_StationInformationManager = ContextRegistry.Context.Resolve<IStationInformationManager>();
                }
                return m_StationInformationManager;
            }
        }

        /// <summary>
        /// Gets or sets the connection properties.
        /// </summary>
        /// <value>
        /// The connection properties.
        /// </value>
        public Dictionary<string, string> ConnectionProperties { get; set; }

        /// <summary>
        /// Gets the bluetooth dongle information manager.
        /// </summary>
        public IBluetoothDongleInformationManager BluetoothDongleInformationManager
        {
            get
            {
                if (m_BluetoothDongleInformationManager == null)
                {
                    m_BluetoothDongleInformationManager = ContextRegistry.Context.Resolve<IBluetoothDongleInformationManager>();
                }
                return m_BluetoothDongleInformationManager;
            }
        }

        /// <summary>
        /// Gets or sets the communication control.
        /// </summary>
        /// <value>
        /// The communication control.
        /// </value>
        public ICommunicationControl CommunicationControl
        {
            get
            {
                if (m_CommunicationControl == null)
                {
                    m_CommunicationControl = ContextRegistry.Context.Resolve<ICommunicationControl>();
                }
                return m_CommunicationControl;
            }
        }

        /// <summary>
        /// Gets or sets the current manometer.
        /// </summary>
        /// <value>
        /// The current manometer.
        /// </value>
        /// <remarks>For testing only.</remarks>
        internal InitializationManometer CurrentManometer
        {
            set { m_CurrentManometer = value; }
        }

        /// <summary>
        /// Gets or sets the name of the GCL.
        /// </summary>
        /// <value>
        /// The name of the GCL.
        /// </value>
        internal string GclName { get; set; }

        /// <summary>
        /// Gets or sets the name of the PRS.
        /// </summary>
        /// <value>
        /// The name of the PRS.
        /// </value>
        internal string PrsName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is initialization for inspection.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is initialization for inspection; otherwise, <c>false</c>.
        /// </value>
        internal bool IsInitializationForInspection { get; set; }
        #endregion Properties

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="InitializationExecutorControl"/> class.
        /// </summary>
        public InitializationExecutorControl()
        {
            m_InitializationOverallErrorCode = ErrorCodes.INITIALIZATION_FINISHED_SUCCESSFULLY;
        }
        #endregion Constructors

        #region Public Functions
        /// <summary>
        /// Executes the initialization.
        /// </summary>
        public void ExecuteInitialization()
        {
            CreateCommandList();
            InitializeManometers();
        }

        /// <summary>
        /// aborts the current initialization
        /// </summary>
        public void AbortInitialization()
        {
            SetCommandStackAbort();
            m_InitializationOverallResult = InitializationResult.USERABORTED;
            m_InitializationOverallErrorCode = ErrorCodes.INITIALIZATION_FINISHED_USER_ABORTED;
        }


        /// <summary>
        /// Executes the initialization.
        /// </summary>
        /// <param name="requiredManometers">The required manometers.</param>
        public void ExecuteInitialization(List<DigitalManometer> requiredManometers)
        {
            CreateCommandList(requiredManometers);
            InitializeManometers();
        }
        #endregion Public Functions

        #region Execute Command Functions
        /// <summary>
        /// Connects this instance.
        /// </summary>
        private void Connect()
        {
            List<BluetoothDongleInformation> allowedBluetoothDongles = new List<BluetoothDongleInformation>();
            clsSettings settings = new clsSettings();
            string btDongleAddress = settings.get_GetSetting(SETTING_CATEGORY, SETTING_BLUETOOTH_DONGLE_ADDRESS).ToString();
            bool isBtDongleAddressDefined = !btDongleAddress.Equals(SETTING_RETURN_NO_VALUE, StringComparison.OrdinalIgnoreCase);
            if (isBtDongleAddressDefined)
            {
                BluetoothDongleInformation btDongleInfo = new BluetoothDongleInformation(btDongleAddress);
                allowedBluetoothDongles.Add(btDongleInfo);
            }
            CommunicationControl.Connect(ConnectionProperties, allowedBluetoothDongles, CommandResult);
        }

        /// <summary>
        /// Disconnects this instance.
        /// </summary>
        private void Disconnect()
        {
            CommunicationControl.Disconnect(CommandResult);
        }

        /// <summary>
        /// Switches the initialization led.
        /// </summary>
        /// <param name="command">The command.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Inspector.BusinessLogic.InitializationExecutorControl.OnInitializationFinished(System.String)")]
        private void SwitchInitializationLed(DeviceCommand command)
        {
            string address = string.Empty;
            try
            {
                clsSettings settings = new clsSettings();
                address = settings.get_GetSetting(SETTING_PLEXOR, SETTING_INITIALIZATION_LED_ADDRESS).ToString();
                SendCommand(command, address);
            }
            catch
            {
                SetCommandStackError();
                UpdateInitializationOverallResult(InitializationStepResult.ERROR);
                OnInitializationStepFinished(m_CurrentCommand, InitializationStepResult.ERROR, String.Empty, ErrorCodes.INITIALIZATION_STEP_SWITCH_INITIALIZATION_LED_CONFIG_ERROR);
            }
        }


        /// <summary>
        /// Switches the manometer.
        /// </summary>
        /// <param name="command">The command.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Inspector.BusinessLogic.InitializationExecutorControl.OnInitializationFinished(System.String)"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2208:InstantiateArgumentExceptionsCorrectly")]
        private void SwitchManometer(DeviceCommand command)
        {
            string dtr = string.Empty;
            try
            {
                dtr = SettingsUtils.GetDTR();
                SendCommand(command, dtr);
            }
            catch
            {
                SetCommandStackError();
                UpdateInitializationOverallResult(InitializationStepResult.ERROR);
                OnInitializationStepFinished(m_CurrentCommand, InitializationStepResult.ERROR, String.Empty, ErrorCodes.INITIALIZATION_STEP_SWITCH_MANOMETER_ERROR);
            }
        }

        /// <summary>
        /// Sets the pressure unit.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        private void SetPressureUnit()
        {
            string pressureUnit = string.Empty;
            try
            {
                clsSettings settings = new clsSettings();
                switch (m_CurrentManometer)
                {
                    case InitializationManometer.TH1:
                        pressureUnit = settings.get_GetSetting(SETTING_MANOMETERTH1, SETTING_UNIT).ToString();
                        break;
                    case InitializationManometer.TH2:
                        pressureUnit = settings.get_GetSetting(SETTING_MANOMETERTH2, SETTING_UNIT).ToString();
                        break;
                    default:
                        break;
                }
            }
            catch { /* do nothing */ }

            if (String.IsNullOrEmpty(pressureUnit) || pressureUnit.ToUpperInvariant().Equals(SETTING_RETURN_NO_VALUE))
            {
                pressureUnit = m_DeviceRangeUnit;
            }

            SendCommand(m_CurrentCommand, pressureUnit);
        }
        #endregion Execute Command Functions

        #region Private Functions
        /// <summary>
        /// Initializes the manometers.
        /// </summary>
        private void InitializeManometers()
        {
            m_InitializationOverallResult = InitializationResult.SUCCESS;
            m_InitializationOverallErrorCode = ErrorCodes.INITIALIZATION_FINISHED_SUCCESSFULLY;
            m_CurrentManometer = InitializationManometer.BLUETOOTH_MODULE;
            ExecuteNextCommand();
        }

        /// <summary>
        /// Executes the next command.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Inspector.BusinessLogic.InitializationExecutorControl.OnInitializationFinished(System.String)")]
        private void ExecuteNextCommand()
        {
            lock (m_CommandStackLock)
            {
                if (m_InitializationStack.Count > 0)
                {
                    m_CurrentCommand = m_InitializationStack.Pop();
                    System.Diagnostics.Debug.WriteLine("BL InitExecutor: ExecuteNextCommand " + m_CurrentCommand.ToString());
                    switch (m_CurrentCommand)
                    {
                        case DeviceCommand.SetPressureUnit:
                            OnInitializationStepStarted(m_CurrentCommand);
                            SetPressureUnit();
                            break;

                        case DeviceCommand.CheckBatteryStatus:
                            OnInitializationStepStarted(m_CurrentCommand);
                            SendCommand(m_CurrentCommand);
                            break;

                        case DeviceCommand.CheckSCPIInterface:
                        case DeviceCommand.CheckPressureUnit:
                        case DeviceCommand.CheckRange:
                        case DeviceCommand.InitiateSelfTest:
                        case DeviceCommand.CheckIdentification:
                        case DeviceCommand.ExitRemoteLocalCommandMode:
                        case DeviceCommand.FlushManometerCache:
                        case DeviceCommand.FlushBluetoothCache:
                        case DeviceCommand.CheckManometerPresent:
                        case DeviceCommand.IRAlwaysOn:
                            OnInitializationStepStarted(m_CurrentCommand);
                            SendCommand(m_CurrentCommand);
                            break;


                        case DeviceCommand.EnterRemoteLocalCommandMode:
                            m_CurrentManometer = InitializationManometer.BLUETOOTH_MODULE;
                            OnInitializationStepStarted(m_CurrentCommand);
                            SendCommand(m_CurrentCommand);
                            break;

                        case DeviceCommand.SwitchInitializationLedOn:
                        case DeviceCommand.SwitchInitializationLedOff:
                            OnInitializationStepStarted(m_CurrentCommand);
                            SwitchInitializationLed(m_CurrentCommand);
                            break;

                        case DeviceCommand.SwitchToManometerTH1:
                            m_CurrentManometer = InitializationManometer.TH1;
                            OnInitializationStepStarted(m_CurrentCommand);
                            SwitchManometer(m_CurrentCommand);
                            break;

                        case DeviceCommand.SwitchToManometerTH2:
                            m_CurrentManometer = InitializationManometer.TH2;
                            OnInitializationStepStarted(m_CurrentCommand);
                            SwitchManometer(m_CurrentCommand);
                            break;

                        case DeviceCommand.Connect:
                            Connect();
                            break;
                        case DeviceCommand.Disconnect:
                            Disconnect();
                            break;

                        default:
                            break;
                    }

                }

                else
                {
                    System.Diagnostics.Debug.WriteLine("BL InitExecutor: ExecuteNextCommand OnInitializationFinished");
                    OnInitializationFinished();
                }
            }
        }

        /// <summary>
        /// Sets the initialization error.
        /// </summary>
        private void SetCommandStackError()
        {
            lock (m_CommandStackLock)
            {
                if (m_InitializationOverallResult != InitializationResult.ERROR)
                { // We did not have an error yet, so try to close turn off the initialization led
                    m_InitializationStack.Clear();
                    // Flush Manometer cache
                    m_InitializationStack.Push(DeviceCommand.FlushManometerCache);

                    // Led off
                    m_InitializationStack.Push(DeviceCommand.ExitRemoteLocalCommandMode);
                    m_InitializationStack.Push(DeviceCommand.SwitchInitializationLedOff);
                    m_InitializationStack.Push(DeviceCommand.FlushBluetoothCache);
                    m_InitializationStack.Push(DeviceCommand.EnterRemoteLocalCommandMode);

                    m_InitializationStack.Push(DeviceCommand.Connect);
                    m_InitializationStack.Push(DeviceCommand.Disconnect);
                }
                else
                { // As we had an error already, turning off the initialization led failed. Therefore: Take no more action.
                    m_InitializationStack.Clear();
                }
            }
        }

        /// <summary>
        /// Sets the initialization error.
        /// </summary>
        private void SetCommandStackAbort()
        {
            lock (m_CommandStackLock)
            {
                m_InitializationStack.Clear();
                // Flush Manometer cache
                m_InitializationStack.Push(DeviceCommand.Disconnect);
                m_InitializationStack.Push(DeviceCommand.FlushManometerCache);

                // Led off
                m_InitializationStack.Push(DeviceCommand.ExitRemoteLocalCommandMode);
                m_InitializationStack.Push(DeviceCommand.SwitchInitializationLedOff);
                m_InitializationStack.Push(DeviceCommand.FlushBluetoothCache);
                m_InitializationStack.Push(DeviceCommand.EnterRemoteLocalCommandMode);

                m_InitializationStack.Push(DeviceCommand.Connect);
                m_InitializationStack.Push(DeviceCommand.Disconnect);
            }
        }

        /// <summary>
        /// Sets the command stack timeout.
        /// </summary>
        private void SetCommandStackTimeout()
        {
            lock (m_CommandStackLock)
            {
                bool hasFinished = false;
                while (!hasFinished)
                {
                    if (m_InitializationStack.Count > 0)
                    {
                        DeviceCommand nextCommand = m_InitializationStack.Peek();

                        bool isCommandForOtherDevice = ((nextCommand == DeviceCommand.SwitchToManometerTH1) ||
                                                        (nextCommand == DeviceCommand.SwitchToManometerTH2) ||
                                                        (nextCommand == DeviceCommand.EnterRemoteLocalCommandMode));

                        if (!isCommandForOtherDevice)
                        {
                            m_InitializationStack.Pop();
                        }
                        else
                        {
                            hasFinished = true;
                        }
                    }
                    else
                    {
                        hasFinished = true;
                    }
                }
            }
        }


        /// <summary>
        /// Updates the initialization overall result.
        /// </summary>
        /// <param name="result">The result.</param>
        private void UpdateInitializationOverallResult(InitializationStepResult result)
        {
            //solution for issue 14234: if the FlushManometercommand fails, we ignore the result, even if it was a timeout
            if (m_CurrentCommand == DeviceCommand.FlushManometerCache) return;

            // Priority of error codes: USERABORTED > ERROR > TIMEOUT > WARNING > SUCCESS
            bool isNotError = (m_InitializationOverallResult != InitializationResult.ERROR);
            bool isNotTimeout = (m_InitializationOverallResult != InitializationResult.TIMEOUT);
            bool isNotWarning = (m_InitializationOverallResult != InitializationResult.WARNING);
            bool isNotUserAborted = (m_InitializationOverallResult != InitializationResult.USERABORTED);
            switch (result)
            {
                case InitializationStepResult.ERROR:
                    if (isNotError)
                    {
                        m_InitializationOverallResult = InitializationResult.ERROR;
                        m_InitializationOverallErrorCode = ErrorCodes.INITIALIZATION_FINISHED_ERROR;
                    }
                    break;
                case InitializationStepResult.TIMEOUT:
                    if (isNotError && isNotTimeout && isNotUserAborted)
                    {
                        m_InitializationOverallResult = InitializationResult.TIMEOUT;
                        m_InitializationOverallErrorCode = ErrorCodes.INITIALIZATION_FINISHED_TIMEOUT;
                    }
                    break;
                case InitializationStepResult.WARNING:
                    if (isNotError && isNotTimeout && isNotWarning && isNotUserAborted)
                    {
                        m_InitializationOverallResult = InitializationResult.WARNING;
                        m_InitializationOverallErrorCode = ErrorCodes.INITIALIZATION_FINISHED_WARNING;
                    }
                    break;
				case InitializationStepResult.USERABORTED:
                case InitializationStepResult.SUCCESS:
                case InitializationStepResult.UNSET:
                default:
                    break;
            }
        }

        /// <summary>
        /// Creates the command list.
        /// </summary>
        /// <param name="requiredManometers">The required manometers.</param>
        private void CreateCommandList(List<DigitalManometer> requiredManometers)
        {
            AddFinishInitializationCommands();
            foreach (DigitalManometer manometer in requiredManometers)
            {
                AddInitializeManometerCommands(manometer);
            }
            AddStartInitializationCommands();

        }

        /// <summary>
        /// Creates the command list.
        /// </summary>
        private void CreateCommandList()
        {
            AddFinishInitializationCommands();
            AddInitializeManometerCommands(DigitalManometer.TH1);
            AddInitializeManometerCommands(DigitalManometer.TH2);
            AddStartInitializationCommands();
        }

        /// <summary>
        /// Adds the initialize manomter commands.
        /// </summary>
        /// <param name="manometer">The manometer.</param>
        private void AddInitializeManometerCommands(DigitalManometer manometer)
        {
            lock (m_CommandStackLock)
            {
                m_InitializationStack.Push(DeviceCommand.IRAlwaysOn);
                m_InitializationStack.Push(DeviceCommand.CheckPressureUnit);
                m_InitializationStack.Push(DeviceCommand.SetPressureUnit);
                m_InitializationStack.Push(DeviceCommand.CheckRange);
                m_InitializationStack.Push(DeviceCommand.CheckIdentification);
                m_InitializationStack.Push(DeviceCommand.InitiateSelfTest);
                m_InitializationStack.Push(DeviceCommand.CheckSCPIInterface);
                m_InitializationStack.Push(DeviceCommand.CheckBatteryStatus);
                m_InitializationStack.Push(DeviceCommand.CheckManometerPresent);
                switch (manometer)
                {
                    case DigitalManometer.TH1:
                        m_InitializationStack.Push(DeviceCommand.SwitchToManometerTH1);
                        break;
                    case DigitalManometer.TH2:
                        m_InitializationStack.Push(DeviceCommand.SwitchToManometerTH2);
                        break;
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// Adds the start initialization commands.
        /// </summary>
        private void AddStartInitializationCommands()
        {
            lock (m_CommandStackLock)
            {
                // Flush Manometer cache
                m_InitializationStack.Push(DeviceCommand.FlushManometerCache);
                // Led on
                m_InitializationStack.Push(DeviceCommand.ExitRemoteLocalCommandMode);
                m_InitializationStack.Push(DeviceCommand.SwitchInitializationLedOn);
                m_InitializationStack.Push(DeviceCommand.FlushBluetoothCache);
                m_InitializationStack.Push(DeviceCommand.EnterRemoteLocalCommandMode);
            }
        }

        /// <summary>
        /// Adds the finish initialization commands.
        /// </summary>
        private void AddFinishInitializationCommands()
        {
            lock (m_CommandStackLock)
            {
                // Flush Manometer cache
                m_InitializationStack.Push(DeviceCommand.FlushManometerCache);

                // Led off
                m_InitializationStack.Push(DeviceCommand.ExitRemoteLocalCommandMode);
                m_InitializationStack.Push(DeviceCommand.SwitchInitializationLedOff);
                m_InitializationStack.Push(DeviceCommand.FlushBluetoothCache);
                m_InitializationStack.Push(DeviceCommand.EnterRemoteLocalCommandMode);

                // Select Manometer TH2 again
                m_InitializationStack.Push(DeviceCommand.SwitchToManometerTH2);
            }
        }

        /// <summary>
        /// Sends the command.
        /// </summary>
        /// <param name="step">The step.</param>
        private void SendCommand(DeviceCommand step)
        {
            SendCommand(step, String.Empty);
        }

        /// <summary>
        /// Sends the command.
        /// </summary>
        /// <param name="step">The step.</param>
        /// <param name="commandParameter">The command parameter.</param>
        private void SendCommand(DeviceCommand step, string commandParameter)
        {
            CommunicationControl.SendCommand(step, commandParameter, CommandResult);
        }
        #endregion Private Functions

        #region CallBacks
        /// <summary>
        /// Commands the result.
        /// </summary>
        /// <param name="commandSucceeded">if set to <c>true</c> [command success].</param>
        /// <param name="errorCode">The error code.</param>
        /// <param name="message">The message.</param>
        public void CommandResult(bool commandSucceeded, int errorCode, string message)
        {
            System.Diagnostics.Debug.WriteLine("BL InitExecutor: CommandResult " + m_CurrentCommand.ToString());

            if (!commandSucceeded)
            {
                HandleError(errorCode);
            }
            else
            {
                HandleCommandResult(message);
            }

            ExecuteNextCommand();
        }

        /// <summary>
        /// Commands the result.
        /// </summary>
        /// <param name="commandSucceeded">if set to <c>true</c> [command success].</param>
        /// <param name="errorCode">The error code.</param>
        /// <param name="message">The message.</param>
        public void RecoverFromErrorCommandResult(bool commandSucceeded, int errorCode, string message)
        {
            m_RecoverFromErrorResetEvent.Set();
        }
        #endregion Callbacks

        #region Handle Command Results
        /// <summary>
        /// Handles the command result.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        private void HandleCommandResult(string message)
        {
            switch (m_CurrentCommand)
            {
                case DeviceCommand.CheckSCPIInterface:
                    HandleCheckSCPIInterface(message);
                    break;
                case DeviceCommand.CheckPressureUnit:
                    HandleCheckPressureUnit(message);
                    break;
                case DeviceCommand.SetPressureUnit:
                    HandleSetPressureUnit(message);
                    break;
                case DeviceCommand.CheckRange:
                    HandleCheckRange(message);
                    break;
                case DeviceCommand.CheckBatteryStatus:
                    HandleCheckBatteryStatus(message);
                    break;
                case DeviceCommand.InitiateSelfTest:
                    HandleInitiateSelfTest(message);
                    break;
                case DeviceCommand.Wakeup:
                case DeviceCommand.CheckIdentification:
                    HandleCheckIdentification(message);
                    break;
                case DeviceCommand.SwitchToManometerTH1:
                case DeviceCommand.SwitchToManometerTH2:
                    HandleSwitchToManometer(message);
                    break;
                case DeviceCommand.SwitchInitializationLedOff:
                case DeviceCommand.SwitchInitializationLedOn:
                    HandleSwitchInitializationLed(message);
                    break;
                case DeviceCommand.EnterRemoteLocalCommandMode:
                    HandleEnterRemoteLocalCommandMode(message);
                    break;
                case DeviceCommand.ExitRemoteLocalCommandMode:
                    HandleExitRemoteLocalCommandMode(message);
                    break;
                case DeviceCommand.FlushManometerCache:
                    HandleFlushManomterCache(message);
                    break;
                case DeviceCommand.FlushBluetoothCache:
                    HandleFlushBluetoothCache(message);
                    break;
                case DeviceCommand.CheckManometerPresent:
                    HandleCheckManoMeterPresent(message);
                    break;
                case DeviceCommand.IRAlwaysOn:
                    HandleIRAlwaysOn(message);
                    break;
                case DeviceCommand.Connect:
                case DeviceCommand.Disconnect:
                default:
                    break;
            }
        }

        private void HandleIRAlwaysOn(string message)
        {
            OnInitializationStepFinished(m_CurrentCommand, InitializationStepResult.SUCCESS, message, ErrorCodes.INITIALIZATION_STEP_EXECUTED_SUCCESSFULLY);
        }

        /// <summary>
        /// Handles the error.
        /// </summary>
        /// <param name="errorCode">The error code.</param>
        /// <returns></returns>
        private void HandleError(int errorCode)
        {
            if (errorCode == ErrorCodes.HAL_COMMAND_TIMEOUT_RECEIVED)
            {



                if (m_CurrentCommand == DeviceCommand.CheckManometerPresent)
                {
                    SetCommandStackError();
                    UpdateInitializationOverallResult(InitializationStepResult.ERROR);
                    OnInitializationStepFinished(m_CurrentCommand, InitializationStepResult.ERROR, "Manometer not present", ErrorCodes.INITIALIZATION_MANOMETER_NOT_PRESENT);
                }
                else
                {
                    SetCommandStackTimeout();
                    UpdateInitializationOverallResult(InitializationStepResult.TIMEOUT);
                    OnInitializationStepFinished(m_CurrentCommand, InitializationStepResult.TIMEOUT, String.Empty, errorCode);
                }
                m_RecoverFromErrorResetEvent.Reset();
                CommunicationControl.RecoverFromError(RecoverFromErrorCommandResult);
                m_RecoverFromErrorResetEvent.WaitOne(m_RecoverFromErrorResetEventTimeout);

            }
            else
            {
                SetCommandStackError();
                UpdateInitializationOverallResult(InitializationStepResult.ERROR);
                OnInitializationStepFinished(m_CurrentCommand, InitializationStepResult.ERROR, String.Empty, errorCode);
            }
        }


        private void HandleCheckManoMeterPresent(string message)
        {
            OnInitializationStepFinished(m_CurrentCommand, InitializationStepResult.SUCCESS, "OK", ErrorCodes.INITIALIZATION_MANOMETER_PRESENT);
        }

        private void HandleFlushBluetoothCache(string message)
        {
            OnInitializationStepFinished(m_CurrentCommand, InitializationStepResult.SUCCESS, message, ErrorCodes.INITIALIZATION_STEP_EXECUTED_SUCCESSFULLY);
        }
        /// <summary>
        /// Handles the switch to manometer.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        /// <remarks>Current spec: All answers are correct</remarks>
        private void HandleSwitchToManometer(string message)
        {
            OnInitializationStepFinished(m_CurrentCommand, InitializationStepResult.SUCCESS, message, ErrorCodes.INITIALIZATION_STEP_EXECUTED_SUCCESSFULLY);
        }

        /// <summary>
        /// Handles the flush manomter cache.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        /// <remarks>Current spec: All answers are correct</remarks>
        private void HandleFlushManomterCache(string message)
        {
            OnInitializationStepFinished(m_CurrentCommand, InitializationStepResult.SUCCESS, message, ErrorCodes.INITIALIZATION_STEP_EXECUTED_SUCCESSFULLY);
        }

        /// <summary>
        /// Handles the exit remote local command mode.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        private void HandleExitRemoteLocalCommandMode(string message)
        {
            InitializationStepResult resultForExecutedStep = InitializationStepResult.SUCCESS;
            int errorCode = ErrorCodes.INITIALIZATION_STEP_EXECUTED_SUCCESSFULLY;

            if (!message.StartsWith("CONNECT", StringComparison.OrdinalIgnoreCase))
            {
                SetCommandStackError();
                UpdateInitializationOverallResult(InitializationStepResult.ERROR);
                errorCode = ErrorCodes.INITIALIZATION_STEP_EXIT_REMOTE_ERROR;
                resultForExecutedStep = InitializationStepResult.ERROR;
            }
            OnInitializationStepFinished(m_CurrentCommand, resultForExecutedStep, message, errorCode);
        }

        /// <summary>
        /// Handles the enter remote local command mode.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        public void HandleSwitchInitializationLed(string message)
        {
            InitializationStepResult resultForExecutedStep = InitializationStepResult.SUCCESS;
            int errorCode = ErrorCodes.INITIALIZATION_STEP_EXECUTED_SUCCESSFULLY;

            if (!message.Equals("OK", StringComparison.OrdinalIgnoreCase))
            {
                SetCommandStackError();
                UpdateInitializationOverallResult(InitializationStepResult.ERROR);
                errorCode = ErrorCodes.INITIALIZATION_STEP_SWITCH_INITIALIZATION_LED_ERROR;
                resultForExecutedStep = InitializationStepResult.ERROR;
            }
            OnInitializationStepFinished(m_CurrentCommand, resultForExecutedStep, message, errorCode);
        }

        /// <summary>
        /// Handles the enter remote local command mode.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        public void HandleEnterRemoteLocalCommandMode(string message)
        {
            InitializationStepResult resultForExecutedStep = InitializationStepResult.SUCCESS;
            int errorCode = ErrorCodes.INITIALIZATION_STEP_EXECUTED_SUCCESSFULLY;

            if (!message.Equals("OK", StringComparison.OrdinalIgnoreCase))
            {
                SetCommandStackError();
                UpdateInitializationOverallResult(InitializationStepResult.ERROR);
                errorCode = ErrorCodes.INITIALIZATION_STEP_ENTER_REMOTE_ERROR;
                resultForExecutedStep = InitializationStepResult.ERROR;
            }
            OnInitializationStepFinished(m_CurrentCommand, resultForExecutedStep, message, errorCode);
        }

        /// <summary>
        /// Handles the check SCPI interface.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        public void HandleCheckSCPIInterface(string message)
        {
            InitializationStepResult resultForExecutedStep = InitializationStepResult.SUCCESS;
            int errorCode = ErrorCodes.INITIALIZATION_STEP_EXECUTED_SUCCESSFULLY;

            if (!message.StartsWith("0,", StringComparison.OrdinalIgnoreCase))
            {
                if (m_ScpiRetries < m_ScpiMaxRetries)
                {
                    m_ScpiRetries++;

                    UpdateInitializationOverallResult(InitializationStepResult.WARNING);
                    errorCode = ErrorCodes.INITIALIZATION_STEP_SCPI_WARNING;
                    resultForExecutedStep = InitializationStepResult.WARNING;

                    m_InitializationStack.Push(DeviceCommand.CheckSCPIInterface);

                    OnInitializationStepFinished(m_CurrentCommand, resultForExecutedStep, message, errorCode);
                }
                else
                { // Exceeded maximum retries
                    SetCommandStackError();
                    UpdateInitializationOverallResult(InitializationStepResult.ERROR);
                    errorCode = ErrorCodes.INITIALIZATION_STEP_SCPI_ERROR;
                    resultForExecutedStep = InitializationStepResult.ERROR;

                    m_ScpiRetries = 0; // reset number of retries for a next run

                    OnInitializationStepFinished(m_CurrentCommand, resultForExecutedStep, message, errorCode);
                }
            }
            else
            { // SCPI returned success
                m_ScpiRetries = 1; // reset number of retries for a next run

                OnInitializationStepFinished(m_CurrentCommand, resultForExecutedStep, message, errorCode);
            }
        }

        /// <summary>
        /// Handles the check identification.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        public void HandleCheckIdentification(string message)
        {
            OnInitializationStepFinished(m_CurrentCommand, InitializationStepResult.SUCCESS, message, ErrorCodes.INITIALIZATION_STEP_EXECUTED_SUCCESSFULLY);
        }

        /// <summary>
        /// Handles the check pressure unit.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        public void HandleCheckPressureUnit(string message)
        {
            OnInitializationStepFinished(m_CurrentCommand, InitializationStepResult.SUCCESS, message, ErrorCodes.INITIALIZATION_STEP_EXECUTED_SUCCESSFULLY);
        }

        /// <summary>
        /// Handles the set pressure unit.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        public void HandleSetPressureUnit(string message)
        {
            InitializationStepResult resultForExecutedStep = InitializationStepResult.SUCCESS;
            int errorCode = ErrorCodes.INITIALIZATION_STEP_EXECUTED_SUCCESSFULLY;

            if (!message.Equals("OK", StringComparison.OrdinalIgnoreCase))
            {
                SetCommandStackError();
                UpdateInitializationOverallResult(InitializationStepResult.ERROR);
                errorCode = ErrorCodes.INITIALIZATION_STEP_SET_PRESSURE_UNIT_ERROR;
                resultForExecutedStep = InitializationStepResult.ERROR;
            }
            OnInitializationStepFinished(m_CurrentCommand, resultForExecutedStep, message, errorCode);
        }

        /// <summary>
        /// Handles the device range.
        /// </summary>
        /// <param name="message">The message.</param>
        public void HandleCheckRange(string message)
        {
            InitializationStepResult resultForExecutedStep = InitializationStepResult.SUCCESS;
            int errorCode = ErrorCodes.INITIALIZATION_STEP_EXECUTED_SUCCESSFULLY;

            string[] range = message.Trim('"').Split(' ');
            if (range.Count() != 2)
            {
                SetCommandStackError();
                UpdateInitializationOverallResult(InitializationStepResult.ERROR);
                errorCode = ErrorCodes.INITIALIZATION_STEP_MANOMETER_RANGE_ERROR;
                resultForExecutedStep = InitializationStepResult.ERROR;
            }
            else
            {
                try
                {
                    bool isNotDemoMode = !HalUtils.IsDemoMode();
                    if (isNotDemoMode && IsInitializationForInspection)
                    {
                        VerifyManometerPressureRange(message);
                    }
                    m_DeviceRangeUnit = range[1];
                }
                catch (InspectionException exception)
                {
                    SetCommandStackError();
                    UpdateInitializationOverallResult(InitializationStepResult.ERROR);
                    errorCode = exception.ErrorCode;
                    resultForExecutedStep = InitializationStepResult.ERROR;
                }
            }
            OnInitializationStepFinished(m_CurrentCommand, resultForExecutedStep, message, errorCode);
        }

        /// <summary>
        /// Verifies the manometer pressure range.
        /// </summary>
        /// <exception cref="InspectionException">Thrown when required pressure range could not be verified.</exception>
        private void VerifyManometerPressureRange(string message)
        {
            string expectedRange;
            string measuredRange;
            switch (m_CurrentManometer)
            {
                case InitializationManometer.TH1:
                    try
                    {
                        expectedRange = StationInformationManager.LookupPeRangeDM(PrsName, GclName).Replace("0..", "").Replace(" ", ""); // Format: 0..xxunit, so remove 0..
                    }
                    catch (InspectorLookupException)
                    {
                        throw new InspectionException(InspectionProcedureResult.ERROR, ErrorCodes.INITIALIZATION_COULD_NOT_RETRIEVE_PRESSURE_RANGE);
                    }
                    measuredRange = message.Trim('"').Replace(" ", "");

                    if (!expectedRange.Equals(measuredRange))
                    {
                        throw new InspectionException(InspectionProcedureResult.ERROR, ErrorCodes.INITIALIZATION_PRESSURE_RANGE_MANOMETER_TH1_INCORRECT);
                    }
                    break;
                case InitializationManometer.TH2:
                    try
                    {
                        expectedRange = StationInformationManager.LookupPaRangeDM(PrsName, GclName).Replace("0..", "").Replace(" ", ""); // Format: 0..xxunit, so remove 0..
                    }
                    catch (InspectorLookupException)
                    {
                        throw new InspectionException(InspectionProcedureResult.ERROR, ErrorCodes.INITIALIZATION_COULD_NOT_RETRIEVE_PRESSURE_RANGE);
                    }
                    measuredRange = message.Trim('"').Replace(" ", "");
                    if (!expectedRange.Equals(measuredRange))
                    {
                        throw new InspectionException(InspectionProcedureResult.ERROR, ErrorCodes.INITIALIZATION_PRESSURE_RANGE_MANOMETER_TH2_INCORRECT);
                    }

                    break;
                default:
                    throw new InspectionException(InspectionProcedureResult.ERROR, ErrorCodes.INITIALIZATION_COULD_NOT_RETRIEVE_PRESSURE_RANGE);
            }
        }


        /// <summary>
        /// Handles the self test.
        /// </summary>
        /// <param name="message">The message.</param>
        public void HandleInitiateSelfTest(string message)
        {
            InitializationStepResult resultForExecutedStep = InitializationStepResult.SUCCESS;
            int errorCode = ErrorCodes.INITIALIZATION_STEP_EXECUTED_SUCCESSFULLY;

            if (!String.IsNullOrEmpty(message))
            {
                SetCommandStackError();
                UpdateInitializationOverallResult(InitializationStepResult.ERROR);
                errorCode = ErrorCodes.INITIALIZATION_STEP_SELF_TEST_ERROR;
                resultForExecutedStep = InitializationStepResult.ERROR;
            }
            OnInitializationStepFinished(m_CurrentCommand, resultForExecutedStep, message, errorCode);
        }

        /// <summary>
        /// Handles the battery status.
        /// </summary>
        /// <param name="message">The message.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        public void HandleCheckBatteryStatus(string message)
        {
            Func<InitializationStepResult, bool> ResultForExecutedStepNotError = (stepResult => stepResult != InitializationStepResult.ERROR);
            int errorCode = ErrorCodes.INITIALIZATION_STEP_EXECUTED_SUCCESSFULLY;

            InitializationStepResult resultForExecutedStep = InitializationStepResult.SUCCESS;

            int batteryLevelLimit = -1;
            int batteryLevel = -1;

            try
            {
                batteryLevelLimit = RetrieveBatteryLevelLimit();
            }
            catch
            {
                SetCommandStackError();
                UpdateInitializationOverallResult(InitializationStepResult.ERROR);
                errorCode = ErrorCodes.INITIALIZATION_STEP_BATTERYLIMIT_CONFIG_ERROR;
                resultForExecutedStep = InitializationStepResult.ERROR;
                OnInitializationStepFinished(m_CurrentCommand, resultForExecutedStep, message, errorCode);
            }

            if (ResultForExecutedStepNotError(resultForExecutedStep))
            {
                bool parseSuccessfull = int.TryParse(message, out batteryLevel);
                if (!parseSuccessfull)
                {
                    SetCommandStackError();
                    UpdateInitializationOverallResult(InitializationStepResult.ERROR);
                    errorCode = ErrorCodes.INITIALIZATION_STEP_BATTERYLIMIT_FORMAT_ERROR;
                    resultForExecutedStep = InitializationStepResult.ERROR;
                    OnInitializationStepFinished(m_CurrentCommand, resultForExecutedStep, message, errorCode);
                }
            }

            if (ResultForExecutedStepNotError(resultForExecutedStep))
            {

                if (batteryLevel <= batteryLevelLimit)
                {
                    UpdateInitializationOverallResult(InitializationStepResult.WARNING);
                    errorCode = ErrorCodes.INITIALIZATION_STEP_BATTERYLIMIT_LEVEL_WARNING;
                    resultForExecutedStep = InitializationStepResult.WARNING;
                }

                OnInitializationStepFinished(m_CurrentCommand, resultForExecutedStep, message, errorCode);
            }
        }

        /// <summary>
        /// Retrieves the battery level limit.
        /// </summary>
        /// <returns></returns>
        private int RetrieveBatteryLevelLimit()
        {
            string batteryLevelLimitString = String.Empty;
            int batteryLevelLimit = -1;

            clsSettings settings = new clsSettings();
            switch (m_CurrentManometer)
            {
                case InitializationManometer.TH1:
                    batteryLevelLimitString = settings.get_GetSetting(SETTING_MANOMETERTH1, SETTING_BATTERY_LIMIT).ToString();
                    break;
                case InitializationManometer.TH2:
                    batteryLevelLimitString = settings.get_GetSetting(SETTING_MANOMETERTH2, SETTING_BATTERY_LIMIT).ToString();
                    break;
                default:
                    break;
            }

            if (batteryLevelLimitString.ToUpperInvariant().Equals(SETTING_RETURN_NO_VALUE))
            {
                batteryLevelLimit = -1; // If not available always make sure a warning will be thrown.
            }
            else
            {
                batteryLevelLimit = int.Parse(batteryLevelLimitString, CultureInfo.InvariantCulture);
            }

            return batteryLevelLimit;
        }
        #endregion Handle Command Results

    }
}
