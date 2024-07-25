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
using System.Threading.Tasks;
using Inspector.BusinessLogic.Data.Configuration.Interfaces.Exceptions;
using Inspector.BusinessLogic.Data.Configuration.Interfaces.Managers;
using Inspector.BusinessLogic.Exceptions;
using Inspector.BusinessLogic.Interfaces;
using Inspector.BusinessLogic.Interfaces.Events;
using Inspector.Connection.Manager.Interfaces;
using Inspector.Hal.Infra;
using Inspector.Infra;
using Inspector.Infra.Ioc;
using Inspector.Model;
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
        private const string SettingManometerth1 = "MANOMETER_TH1";
        private const string SettingManometerth2 = "MANOMETER_TH2";
        private const string SettingBatteryLimit = "BatteryLimit";
        private const string SettingUnits = "UNITS";
        private const string SettingReturnNoValue = "<NO VALUE>";
        private const string SettingLowPressureUnit = "UnitLowPressure";
        private const string SettingHighPressureUnit = "UnitHighPressure";

        private const string SettingCategory = "PLEXOR";
        private const string SettingEnableIOStatus = "EnableIOStatus";
        private const string SettingStoreIOStatus = "StoreIOStatus";
        private const bool DefaultEnableIOStatus = false;
        private const bool DefaultStoreIOStatus = false;

        #endregion Constants

        #region Class Members
        private readonly Stack<DeviceCommand> m_InitializationStack = new Stack<DeviceCommand>();
        private DeviceCommand m_CurrentCommand = DeviceCommand.None;
        private ICommunicationControl m_CommunicationControl;
        private IBluetoothDongleInformationManager m_BluetoothDongleInformationManager;
        private IStationInformationManager m_StationInformationManager;

        private InitializationResult m_InitializationOverallResult = InitializationResult.UNSET;
        private int m_InitializationOverallErrorCode;

        private string m_DeviceRangeUnit = string.Empty;
        private InitializationManometer m_CurrentManometer = InitializationManometer.UNSET;

        private int m_ScpiRetries;
        private readonly int m_ScpiMaxRetries = 5; // Max 6 tries -> 5 retries allowed

        private readonly ManualResetEvent m_RecoverFromErrorResetEvent = new ManualResetEvent(false);
        private readonly int m_RecoverFromErrorResetEventTimeout = 1000;

        private SSDInitialisationExecutorControl m_SsdInitialisation;

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

        public event EventHandler<UiRequestEventArgs> UiInputRequested;
        #endregion Events

        #region Event Handlers
        /// <summary>
        /// Called when [initialization step started].
        /// </summary>
        /// <param name="id">The id.</param>
        private void OnInitializationStepStarted(DeviceCommand id)
        {
            OnInitializationStepStarted(new StartInitializationStepEventArgs(id.ToString()));
        }

        private void OnInitializationStepStarted(StartInitializationStepEventArgs e)
        {
            InitializationStepStarted?.Invoke(this, e);
        }

        /// <summary>
        /// Called when [initialization step finished].
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="result">The result.</param>
        /// <param name="message">The message.</param>
        /// <param name="errorCode">The error code.</param>
        private void OnInitializationStepFinished(DeviceCommand id, InitializationStepResult result, string message, int errorCode)
        {
            OnInitializationStepFinished(new FinishInitializationStepEventArgs(id.ToString(), result, message, errorCode, m_CurrentManometer));
        }
        private void OnInitializationStepFinished(FinishInitializationStepEventArgs e)
        {
            if (InitializationStepFinished != null)
            {
                InitializationStepFinished(this, e);
            }
        }
        /// <summary>
        /// Called when [initialization finished].
        /// </summary>
        private void OnInitializationFinished()
        {

            if (InitializationFinished != null)
            {
                InitializationFinished(this, new FinishInitializationEventArgs(m_InitializationOverallResult, m_InitializationOverallErrorCode));
            }
        }

        private void OnUiInputRequested(UiRequestEventArgs e)
        {
            if (UiInputRequested != null)
            {
                UiInputRequested(this, e);
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
        public IStationInformationManager StationInformationManager => m_StationInformationManager ?? (m_StationInformationManager = ContextRegistry.Context.Resolve<IStationInformationManager>());

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
        public IBluetoothDongleInformationManager BluetoothDongleInformationManager => m_BluetoothDongleInformationManager ?? (m_BluetoothDongleInformationManager = ContextRegistry.Context.Resolve<IBluetoothDongleInformationManager>());

        /// <summary>
        /// Gets or sets the communication control.
        /// </summary>
        /// <value>
        /// The communication control.
        /// </value>
        public ICommunicationControl CommunicationControl => m_CommunicationControl ?? (m_CommunicationControl = ContextRegistry.Context.Resolve<ICommunicationControl>());

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

        /// <summary>
        /// Gets or sets the device type.
        /// </summary>
        /// <value>
        /// The device type.
        /// </value>
        public DeviceType DeviceType { get; set; }

        /// <summary>
        /// Gets or sets the connected manometer.
        /// </summary>
        /// <value>
        /// The connected manometer.
        /// </value>
        public DigitalManometer ConnectedDigitalManometer { get; set; }

        /// <summary>
        /// Gets the ScriptCommandFlag.
        /// </summary>
        public bool? ScriptCommandFlag { get; private set; }
        #endregion Properties

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="InitializationExecutorControl"/> class.
        /// </summary>
        public InitializationExecutorControl()
        {
            m_InitializationOverallErrorCode = ErrorCodes.INITIALIZATION_FINISHED_SUCCESSFULLY;
            DeviceType = DeviceType.Unknown;
            ConnectedDigitalManometer = DigitalManometer.Unknown;
            m_SsdInitialisation = new SSDInitialisationExecutorControl(CommunicationControl);
        }
        #endregion Constructors

        #region Public Functions
        /// <summary>
        /// Executes the initialization.
        /// </summary>
        public void ExecuteInitialization()
        {
            if (DeviceType == DeviceType.PlexorBluetoothWIS)
            {
                Task.Factory.StartNew(() =>
                {
                    AttachSSDEvents();

                    var ssdInitSuccess = m_SsdInitialisation.DoInitialisation(out bool flag);

                    DetachSSDEvents();

                    if (ssdInitSuccess)
                    {
                        ScriptCommandFlag = flag;
                        ExecuteNormalInitialisation();
                    }
                    else
                    {
                        HandleSsdInitError();
                    }
                });
            }
            else
            {
                ExecuteNormalInitialisation();
            }
        }

        private void DetachSSDEvents()
        {
            m_SsdInitialisation.InitialisationStepFinished -= M_SsdInitialisation_InitialisationStepFinished;
            m_SsdInitialisation.InitialisationStepStarted -= M_SsdInitialisation_InitialisationStepStarted;
            m_SsdInitialisation.UiInputNeeded -= M_SsdInitialisation_UiInputNeeded;
        }

        private void AttachSSDEvents()
        {
            m_SsdInitialisation.InitialisationStepFinished += M_SsdInitialisation_InitialisationStepFinished;
            m_SsdInitialisation.InitialisationStepStarted += M_SsdInitialisation_InitialisationStepStarted;
            m_SsdInitialisation.UiInputNeeded += M_SsdInitialisation_UiInputNeeded;
        }

        private void M_SsdInitialisation_InitialisationStepStarted(object sender, StartInitializationStepEventArgs e)
        {
            OnInitializationStepStarted(e);
        }

        public void SetUiResponse(UiResponse response)
        {
            m_SsdInitialisation.SetUiReponse(response);
        }

        private void M_SsdInitialisation_UiInputNeeded(object sender, UiRequestEventArgs e)
        {
            OnUiInputRequested(e);
        }

        private void M_SsdInitialisation_InitialisationStepFinished(object sender, FinishInitializationStepEventArgs e)
        {
            OnInitializationStepFinished(e);
        }

        private void HandleSsdInitError()
        {
            SetCommandStackError();
            UpdateInitializationOverallResult(InitializationStepResult.ERROR);
            ExecuteNextCommand();
        }

        private void ExecuteNormalInitialisation()
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
        /// <param name="performSsdInit"></param>
        public void ExecuteInitialization(List<DigitalManometer> requiredManometers, bool performSsdInit)
        {
            if (performSsdInit && DeviceType == DeviceType.PlexorBluetoothWIS)
            {
                Task.Factory.StartNew(() =>
                {
                    bool scriptCommandFlag = true;

                    AttachSSDEvents();

                    var ssdInitResult = m_SsdInitialisation.DoInitialisation(out scriptCommandFlag);

                    DetachSSDEvents();

                    if (ssdInitResult)
                    {
                        ScriptCommandFlag = scriptCommandFlag;
                        CreateCommandList(requiredManometers);

                        InitializeManometers();
                    }
                    else
                    {
                        HandleSsdInitError();
                    }
                });
            }

            else
            {
                CreateCommandList(requiredManometers);

                InitializeManometers();
            }

            
        }
        #endregion Public Functions

        #region Execute Command Functions
        /// <summary>
        /// Switches the manometer.
        /// </summary>
        /// <param name="command">The command.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Inspector.BusinessLogic.InitializationExecutorControl.OnInitializationFinished(System.String)"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2208:InstantiateArgumentExceptionsCorrectly")]
        private void SwitchManometer(DeviceCommand command)
        {
            try
            {
                SendCommand(command, string.Empty);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                SetCommandStackError();
                UpdateInitializationOverallResult(InitializationStepResult.ERROR);
                OnInitializationStepFinished(m_CurrentCommand, InitializationStepResult.ERROR, string.Empty, ErrorCodes.INITIALIZATION_STEP_SWITCH_MANOMETER_ERROR);
            }
        }

        /// <summary>
        /// Sets the pressure unit.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        private void SetPressureUnit()
        {
            var pressureUnit = string.Empty;
            try
            {
                var settings = new clsSettings();
                var pressureSetting = CurrentManoMeterIsLowPressureRange() ? SettingLowPressureUnit : SettingHighPressureUnit;
                pressureUnit = settings.get_GetSetting(SettingUnits, pressureSetting).ToString();
            }
            catch { /* do nothing */ }

            if (string.IsNullOrEmpty(pressureUnit) || pressureUnit.ToUpperInvariant().Equals(SettingReturnNoValue))
            {
                pressureUnit = m_DeviceRangeUnit;
            }
            
            SendCommand(m_CurrentCommand, pressureUnit);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1304:SpecifyCultureInfo", MessageId = "System.String.ToLower")]
        private bool CurrentManoMeterIsLowPressureRange()
        {
            return m_DeviceRangeUnit.ToLower().Equals("mbar") || m_DeviceRangeUnit.ToLower().Equals("inh2o");
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
            lock (m_InitializationStack)
            {
                if (m_InitializationStack.Count > 0)
                {
                    m_CurrentCommand = m_InitializationStack.Pop();

                    System.Diagnostics.Debug.WriteLine("BL InitExecutor: ExecuteNextCommand {0}", m_CurrentCommand);
                    
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
                        case DeviceCommand.CheckSystemSoftwareVersion:
                        case DeviceCommand.CheckCalibrationDate:
                        case DeviceCommand.EnableIOStatus:
                        case DeviceCommand.DisableIOStatus:
                        case DeviceCommand.MeasureSingleValue:
                            OnInitializationStepStarted(m_CurrentCommand);
                            SendCommand(m_CurrentCommand);
                            break;
                        case DeviceCommand.EnterRemoteLocalCommandMode:
                            m_CurrentManometer = InitializationManometer.BLUETOOTH_MODULE;
                            OnInitializationStepStarted(m_CurrentCommand);
                            SendCommand(m_CurrentCommand);
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
                        case DeviceCommand.Disconnect:
                        case DeviceCommand.None:
                        case DeviceCommand.MeasureContinuously:
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
            lock (m_InitializationStack)
            {
                m_InitializationStack.Clear();
            }
        }

        /// <summary>
        /// Sets the initialization error.
        /// </summary>
        private void SetCommandStackAbort()
        {
            lock (m_InitializationStack)
            {
                m_InitializationStack.Clear();
                m_InitializationStack.Push(DeviceCommand.Disconnect);
            }
        }

        /// <summary>
        /// Sets the command stack timeout.
        /// </summary>
        private void SetCommandStackTimeout()
        {
            lock (m_InitializationStack)
            {
                var initializationList = m_InitializationStack.ToList();
                var index = initializationList.FindIndex(c => c == DeviceCommand.SwitchToManometerTH1 || c == DeviceCommand.SwitchToManometerTH2);

                switch (DeviceType)
                {
                    case DeviceType.PlexorBluetoothIrDA:
                        if (index > 0) { index -= 1; } // Flush bluetooth cache
                        if (index > 0) { index -= 1; } // Enter local mode
                        break;
                    case DeviceType.PlexorBluetoothWIS:
                        if (index > 0) { index -= 1; } // Enter local mode
                        break;
                    case DeviceType.Unknown:
                    default:
                        break;
                }

                // Remove obsolete commands of timeout device
                for (var i = 0; i < index; i++)
                {
                    m_InitializationStack.Pop();
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
            var isNotError = (m_InitializationOverallResult != InitializationResult.ERROR);
            var isNotTimeout = (m_InitializationOverallResult != InitializationResult.TIMEOUT);
            var isNotWarning = (m_InitializationOverallResult != InitializationResult.WARNING);
            var isNotUserAborted = (m_InitializationOverallResult != InitializationResult.USERABORTED);

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
        private void CreateCommandList(IEnumerable<DigitalManometer> requiredManometers)
        {
            AddSwitchManometerCommands(DigitalManometer.TH2);

            foreach (var manometer in requiredManometers)
            {
                AddInitializeManometerCommands(manometer);
            }
        }

        /// <summary>
        /// Creates the command list.
        /// </summary>
        private void CreateCommandList()
        {
            AddSwitchManometerCommands(DigitalManometer.TH2);
            AddInitializeManometerCommands(DigitalManometer.TH1);
            AddInitializeManometerCommands(DigitalManometer.TH2);
        }

        /// <summary>
        /// Adds the initialize manomter commands.
        /// </summary>
        /// <param name="manometer">The manometer.</param>
        private void AddInitializeManometerCommands(DigitalManometer manometer)
        {
            lock (m_InitializationStack)
            {
                switch (DeviceType)
                {
                    case DeviceType.PlexorBluetoothWIS:
                        m_InitializationStack.Push(DeviceCommand.ExitRemoteLocalCommandMode);
                        m_InitializationStack.Push(GetEnableIOStatus() ? DeviceCommand.EnableIOStatus : DeviceCommand.DisableIOStatus);
                        m_InitializationStack.Push(DeviceCommand.CheckSystemSoftwareVersion);
                        m_InitializationStack.Push(DeviceCommand.CheckCalibrationDate);
                        m_InitializationStack.Push(DeviceCommand.EnterRemoteLocalCommandMode);
                        break;
                    case DeviceType.Unknown:
                    case DeviceType.PlexorBluetoothIrDA:
                    default:
                        break;
                }

                m_InitializationStack.Push(DeviceCommand.IRAlwaysOn);
                m_InitializationStack.Push(DeviceCommand.CheckPressureUnit);
                m_InitializationStack.Push(DeviceCommand.SetPressureUnit);
                m_InitializationStack.Push(DeviceCommand.CheckRange);
                m_InitializationStack.Push(DeviceCommand.CheckIdentification);
                m_InitializationStack.Push(DeviceCommand.InitiateSelfTest);
                m_InitializationStack.Push(DeviceCommand.CheckSCPIInterface);
                m_InitializationStack.Push(DeviceCommand.CheckBatteryStatus);
                m_InitializationStack.Push(DeviceCommand.CheckManometerPresent);

                AddSwitchManometerCommands(manometer);
            }
        }

        /// <summary>
        /// Adds the switch manometer commands.
        /// </summary>
        /// <param name="manometer">The manometer.</param>
        private void AddSwitchManometerCommands(DigitalManometer manometer)
        {
            switch (DeviceType)
            {
                case DeviceType.PlexorBluetoothIrDA:
                    m_InitializationStack.Push(DeviceCommand.FlushManometerCache);
                    m_InitializationStack.Push(DeviceCommand.ExitRemoteLocalCommandMode);
                    break;
                case DeviceType.PlexorBluetoothWIS:
                case DeviceType.Unknown:
                default:
                    break;
            }

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

            switch (DeviceType)
            {
                case DeviceType.PlexorBluetoothWIS:
                    m_InitializationStack.Push(DeviceCommand.EnterRemoteLocalCommandMode);
                    break;
                case DeviceType.PlexorBluetoothIrDA:
                    m_InitializationStack.Push(DeviceCommand.FlushBluetoothCache);
                    m_InitializationStack.Push(DeviceCommand.EnterRemoteLocalCommandMode);
                    break;
                case DeviceType.Unknown:
                default:
                    break;
            }
        }

        /// <summary>
        /// Sends the command.
        /// </summary>
        /// <param name="step">The step.</param>
        private void SendCommand(DeviceCommand step)
        {
            SendCommand(step, string.Empty);
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
        private void CommandResult(bool commandSucceeded, int errorCode, string message)
        {
            System.Diagnostics.Debug.WriteLine("BL InitExecutor: CommandResult {0}", m_CurrentCommand);

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
        private void RecoverFromErrorCommandResult(bool commandSucceeded, int errorCode, string message)
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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Inspector.BusinessLogic.InitializationExecutorControl.OnInitializationStepFinished(Inspector.Infra.DeviceCommand,Inspector.Model.InitializationStepResult,System.String,System.Int32)")]
        private void HandleCommandResult(string message)
        {
            switch (m_CurrentCommand)
            {
                case DeviceCommand.CheckSCPIInterface:
                    HandleCheckSCPIInterface(message);
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
                case DeviceCommand.EnterRemoteLocalCommandMode:
                    HandleEnterRemoteLocalCommandMode(message);
                    break;
                case DeviceCommand.ExitRemoteLocalCommandMode:
                    HandleExitRemoteLocalCommandMode(message);
                    break;
                case DeviceCommand.SwitchToManometerTH1:
                    ConnectedDigitalManometer = DigitalManometer.TH1;
                    OnInitializationStepFinished(m_CurrentCommand, InitializationStepResult.SUCCESS, message, ErrorCodes.INITIALIZATION_STEP_EXECUTED_SUCCESSFULLY);
                    break;
                case DeviceCommand.SwitchToManometerTH2:
                    ConnectedDigitalManometer = DigitalManometer.TH2;
                    OnInitializationStepFinished(m_CurrentCommand, InitializationStepResult.SUCCESS, message, ErrorCodes.INITIALIZATION_STEP_EXECUTED_SUCCESSFULLY);
                    break;
                case DeviceCommand.CheckPressureUnit:
                case DeviceCommand.CheckIdentification:
                case DeviceCommand.FlushManometerCache:
                case DeviceCommand.FlushBluetoothCache:
                case DeviceCommand.IRAlwaysOn:
                case DeviceCommand.CheckSystemSoftwareVersion:
                case DeviceCommand.CheckCalibrationDate:
                case DeviceCommand.EnableIOStatus:
                case DeviceCommand.DisableIOStatus:
                    OnInitializationStepFinished(m_CurrentCommand, InitializationStepResult.SUCCESS, message, ErrorCodes.INITIALIZATION_STEP_EXECUTED_SUCCESSFULLY);
                    break;
                case DeviceCommand.CheckManometerPresent:
                    OnInitializationStepFinished(m_CurrentCommand, InitializationStepResult.SUCCESS, "OK", ErrorCodes.INITIALIZATION_MANOMETER_PRESENT);
                    break;
                case DeviceCommand.Disconnect:
                    ConnectedDigitalManometer = DigitalManometer.Unknown;
                    break;
                case DeviceCommand.Connect:
                case DeviceCommand.None:
                case DeviceCommand.MeasureContinuously:
                default:
                    break;
            }
        }

        /// <summary>
        /// Handles the error.
        /// </summary>
        /// <param name="errorCode">The error code.</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Inspector.BusinessLogic.InitializationExecutorControl.OnInitializationStepFinished(Inspector.Infra.DeviceCommand,Inspector.Model.InitializationStepResult,System.String,System.Int32)")]
        private void HandleError(int errorCode)
        {
            if (errorCode == ErrorCodes.HAL_COMMAND_TIMEOUT_RECEIVED)
            {
                // If there is a timeout in this step, resume to the init is stil needed. 
                // If the manometer is not present, this will be detected in the next step.
                if (m_CurrentCommand != DeviceCommand.FlushManometerCache)
                {
                    SetCommandStackTimeout();
                }

                if (m_CurrentCommand == DeviceCommand.CheckManometerPresent )
                {
                    UpdateInitializationOverallResult(InitializationStepResult.ERROR);
                    OnInitializationStepFinished(m_CurrentCommand, InitializationStepResult.ERROR, "Manometer not present", ErrorCodes.INITIALIZATION_MANOMETER_NOT_PRESENT);
                }
                else
                {
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

                OnInitializationStepFinished(m_CurrentCommand, InitializationStepResult.ERROR, string.Empty, errorCode);
            }
        }

        /// <summary>
        /// Handles the exit remote local command mode.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        private void HandleExitRemoteLocalCommandMode(string message)
        {
            var resultForExecutedStep = InitializationStepResult.SUCCESS;
            var errorCode = ErrorCodes.INITIALIZATION_STEP_EXECUTED_SUCCESSFULLY;

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
        private void HandleEnterRemoteLocalCommandMode(string message)
        {
            var resultForExecutedStep = InitializationStepResult.SUCCESS;
            var errorCode = ErrorCodes.INITIALIZATION_STEP_EXECUTED_SUCCESSFULLY;

            var compare = string.Empty;

            switch (DeviceType)
            {
                case DeviceType.PlexorBluetoothIrDA:
                    compare = "OK";
                    break;
                case DeviceType.PlexorBluetoothWIS:
                    compare = "LOCAL";
                    break;
                case DeviceType.Unknown:
                default:
                    break;
            }

            if (!message.Equals(compare, StringComparison.OrdinalIgnoreCase))
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
        internal void HandleCheckSCPIInterface(string message)
        {
            var resultForExecutedStep = InitializationStepResult.SUCCESS;
            var errorCode = ErrorCodes.INITIALIZATION_STEP_EXECUTED_SUCCESSFULLY;

            if (!message.StartsWith("0,", StringComparison.OrdinalIgnoreCase))
            {
                if (m_ScpiRetries < m_ScpiMaxRetries)
                {
                    m_ScpiRetries++;
                    m_InitializationStack.Push(DeviceCommand.CheckSCPIInterface); //retry CheckSCPIInterface
                }
                else
                {
                    //Maximum retries exceeded
                    //Will not fail initialisation, so handle as warning
                    m_ScpiRetries = 0; // reset number of retries for a next run
                }

                errorCode = ErrorCodes.INITIALIZATION_STEP_SCPI_WARNING;
                resultForExecutedStep = InitializationStepResult.WARNING;

                OnInitializationStepFinished(m_CurrentCommand, resultForExecutedStep, message, errorCode);
            }
            else
            {
                // SCPI returned success
                m_ScpiRetries = 1; // reset number of retries for a next run

                OnInitializationStepFinished(m_CurrentCommand, resultForExecutedStep, message, errorCode);
            }
        }

        /// <summary>
        /// Handles the set pressure unit.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        internal void HandleSetPressureUnit(string message)
        {
            var resultForExecutedStep = InitializationStepResult.SUCCESS;
            var errorCode = ErrorCodes.INITIALIZATION_STEP_EXECUTED_SUCCESSFULLY;

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
        internal void HandleCheckRange(string message)
        {
            var resultForExecutedStep = InitializationStepResult.SUCCESS;
            var errorCode = ErrorCodes.INITIALIZATION_STEP_EXECUTED_SUCCESSFULLY;

            var range = message.Trim('"').Split(' ');
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
                    var isNotDemoMode = !HalUtils.IsDemoMode();

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
        internal void HandleInitiateSelfTest(string message)
        {
            var resultForExecutedStep = InitializationStepResult.SUCCESS;
            var errorCode = ErrorCodes.INITIALIZATION_STEP_EXECUTED_SUCCESSFULLY;

            if (!string.IsNullOrEmpty(message))
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
        internal void HandleCheckBatteryStatus(string message)
        {
            Func<InitializationStepResult, bool> resultForExecutedStepNotError = (stepResult => stepResult != InitializationStepResult.ERROR);

            var resultForExecutedStep = InitializationStepResult.SUCCESS;
            var errorCode = ErrorCodes.INITIALIZATION_STEP_EXECUTED_SUCCESSFULLY;

            var batteryLevelLimit = -1;
            var batteryLevel = -1;

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

            if (resultForExecutedStepNotError(resultForExecutedStep))
            {
                var parseSuccessfull = int.TryParse(message, out batteryLevel);
                if (!parseSuccessfull)
                {
                    SetCommandStackError();

                    UpdateInitializationOverallResult(InitializationStepResult.ERROR);

                    errorCode = ErrorCodes.INITIALIZATION_STEP_BATTERYLIMIT_FORMAT_ERROR;
                    resultForExecutedStep = InitializationStepResult.ERROR;

                    OnInitializationStepFinished(m_CurrentCommand, resultForExecutedStep, message, errorCode);
                }
            }

            if (resultForExecutedStepNotError(resultForExecutedStep))
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
            var batteryLevelLimitString = String.Empty;
            var batteryLevelLimit = -1;

            var settings = new clsSettings();
            switch (m_CurrentManometer)
            {
                case InitializationManometer.TH1:
                    batteryLevelLimitString = settings.get_GetSetting(SettingManometerth1, SettingBatteryLimit).ToString();
                    break;
                case InitializationManometer.TH2:
                    batteryLevelLimitString = settings.get_GetSetting(SettingManometerth2, SettingBatteryLimit).ToString();
                    break;
                default:
                    break;
            }

            if (batteryLevelLimitString.ToUpperInvariant().Equals(SettingReturnNoValue))
            {
                batteryLevelLimit = -1; // If not available always make sure a warning will be thrown.
            }
            else
            {
                batteryLevelLimit = int.Parse(batteryLevelLimitString, CultureInfo.InvariantCulture);
            }

            return batteryLevelLimit;
        }

        /// <summary>
        /// Gets the enable IO status.
        /// </summary>
        /// <returns>The boolean state of the IO status enabled option.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        private bool GetEnableIOStatus()
        {
            bool enableIOStatus;
            try
            {
                var settings = new clsSettings();
                enableIOStatus = bool.Parse(settings.get_GetSetting(SettingCategory, SettingEnableIOStatus).ToString());
            }
            catch (Exception)
            {
                enableIOStatus = DefaultEnableIOStatus;
            }

            return enableIOStatus && (DeviceType == DeviceType.PlexorBluetoothWIS);
        }

        /// <summary>
        /// Gets the store IO status.
        /// </summary>
        /// <returns>The boolean state of the store IO status enabled option.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        public bool GetStoreIOStatus()
        {
            bool storeIOStatus;
            try
            {
                var settings = new clsSettings();
                storeIOStatus = bool.Parse(settings.get_GetSetting(SettingCategory, SettingStoreIOStatus).ToString());
            }
            catch (Exception)
            {
                storeIOStatus = DefaultStoreIOStatus;
            }
            
            return storeIOStatus && GetEnableIOStatus() && (DeviceType == DeviceType.PlexorBluetoothWIS);
        }

        #endregion Handle Command Results
    }
}
