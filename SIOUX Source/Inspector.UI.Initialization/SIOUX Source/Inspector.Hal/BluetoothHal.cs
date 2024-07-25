/*
//===============================================================================
// Copyright Kamstrup
// All rights reserved.
//===============================================================================
*/

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using Inspector.Hal.Interfaces;
using Inspector.Hal.Interfaces.Events;
using Inspector.Hal.Interfaces.Exceptions;
using Inspector.Infra;
using Inspector.Model;
using Inspector.Model.BluetoothDongle;
using KAM.INSPECTOR.Infra;
using log4net;
using wcl;

namespace Inspector.Hal
{
    /// <summary>
    /// BluetoothHal
    /// </summary>
    public class BluetoothHal : IHal
    {
        #region Enumerations
        /// <summary>
        /// Indicates the state in which the wake up system is in (handling manometer 1 or 2 and completed)
        /// </summary>
        private enum WakeUpState
        {
            /// <summary>
            /// Not busy with an active wake up command
            /// </summary>
            IDLE,
            /// <summary>
            /// Indicates that manometer 1 should be handled
            /// </summary>
            MANOMETER1,
            /// <summary>
            /// Indicates that manometer 2 should be handled and manometer 1 is already handled
            /// </summary>
            MANOMETER2,
        }
        #endregion Enumerations

        #region Helper classes
        /// <summary>
        /// Data structure that can hold a command with optionally a parameter.
        /// </summary>
        private class CommandData
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="CommandData"/> class.
            /// </summary>
            public CommandData()
            {
                Command = DeviceCommand.None;
                Parameter = String.Empty;
            }

            /// <summary>
            /// Gets or sets the command.
            /// </summary>
            /// <value>The command.</value>
            public DeviceCommand Command { get; set; }

            /// <summary>
            /// Gets or sets the parameter of the command.
            /// </summary>
            /// <value>The parameter of the command.</value>
            public string Parameter { get; set; }

            /// <summary>
            /// Gets a value indicating whether the command data contains a command
            /// </summary>
            /// <value>
            /// 	<c>true</c> if the command data contains a command; otherwise, <c>false</c>.
            /// </value>
            public bool IsCommandAssigned
            {
                get
                {
                    return Command != DeviceCommand.None;
                }
            }
        }
        #endregion Helper classes

        #region Constants
        private const string CONNECTIONPROPERTY_BLUETOOTHAPI = "bluetoothApi";
        private const string CONNECTIONPROPERTY_DESTINATIONADDRESS = "destinationAddress";

        private const string SETTING_MANOMETER_COMMAND_TIMEOUT = "ManometerCommandTimeout";
        private const string SETTING_MANOMETER_COMMAND_RETRIES = "ManometerCommandRetries";
        private const string SETTING_CONNECT_TIMEOUT = "ConnectTimeout";
        private const string SETTING_CONNECT_RETRIES = "ConnectRetries";
        private const string SETTING_MEASUREMENT_RETRIES = "MeasurementRetries";
        private const string SETTING_FLUSH_MANOMETER_CACHE_TIMEOUT = "FlushManometerCacheTimeout";
        private const string SETTING_FLUSH_MANOMETER_CACHE_RETRIES = "FlushManometerCacheRetries";
        private const string SETTING_MEASUREMENT_TIMEOUT = "MeasurementTimeout";
        private const string SETTING_MEASUREMENT_INTERVAL = "MeasurementInterval";
        private const string SETTING_WAKEUP_INTERVAL = "WakeUpInterval";
        private const string SETTING_BLUETOOTH_API = "BluetoothApi";
        private const string SETTING_BLUETOOTH_DONGLE_ADDRESS = "BluetoothDongleAddress";
        private const string SETTING_CATEGORY = "PLEXOR";
        private const string SETTING_RETURN_NO_VALUE = "<NO VALUE>";
        private const string SETTING_USE_CHECKSUM = "UseChecksum";

        private const int DEFAULT_MANOMETER_COMMAND_TIMEOUT = 3000;
        private const int DEFAULT_CONNECT_TIMEOUT = 15000;
        private const int DEFAULT_FLUSH_MANOMETER_CACHE_TIMEOUT = 3000;
        private const int DEFAULT_MANOMETER_COMMAND_RETRIES = 3;
        private const int DEFAULT_CONNECT_RETRIES = 3;
        private const int DEFAULT_FLUSH_MANOMETER_CACHE_RETRIES = 3;
        private const int DEFAULT_MEASUREMENT_INTERVAL = 1000;
        private const int DEFAULT_MEASUREMENT_TIMER_TIMEOUT = 3000;
        private const int DEFAULT_WAKEUP_INTERVAL = 60000;
        private const int DEFAULT_MEASUREMENT_RETRIES = 3;

        private const int FINALIZE_MEASUREMENT_WAIT_TIME = 1500; // 680ms is maximum required for device * 2 (rounded to 1500)
        #endregion Constants

        #region Class Members
        private wclAPI m_WclApi;
        private bool m_WclApiInitialized = false;
        private bool m_VirtualComportInitialized = false;
        private bool m_BluetoothDiscoveryInitialized = false;
        private wclVirtualCOMPort m_WclVirtualComPort;
        private wclBluetoothDiscovery m_WclBluetoothDiscovery;
        private SerialConnection m_SerialConnection;
        private Dictionary<DeviceCommand, string> m_SerialCommands = new Dictionary<DeviceCommand, string>();

        private DeviceCommand m_CurrentCommand = DeviceCommand.None;
        private string m_CurrentCommandParameter = String.Empty;
        private Dictionary<string, string> m_ConnectionParameters = new Dictionary<string, string>();
        private List<BluetoothDongleInformation> m_AllowedBluetoothDongles = new List<BluetoothDongleInformation>();

        private string m_SerialPortNumber;

        private bool m_Disposed;

        private string m_SerialDataReceived = String.Empty;
        private string m_MeasurementsReceived = String.Empty;

        private int m_ManometerCommandMaxRetries = 3;
        private int m_FlushManometerCacheMaxRetries = 3;
        private int m_ConnectMaxRetries = 3;
        private int m_ManometerCommandTimeout = 3000;
        private int m_FlushManometerCacheTimeout = 3000;
        private int m_ConnectTimeout = 15000;
        private int m_MeasurementTimeout = 3000;
        private int m_MeasurementInterval = 1000;
        private int m_WakeUpInterval = 60000;
        private int m_MeasurementCommandMaxRetries = 3;

        private Timer m_CommandTimeoutTimer;
        private Timer m_ConnectTimer;
        private Timer m_FlushManometerCacheTimeoutTimer;
        private Timer m_MeasurementTimer;
        private Timer m_MeasurementTimeoutTimer;
        private Timer m_WakeUpTimer;
        private Timer m_StoppingMeasurementTimer;

        private bool m_StoppedMeasurementTimedOut = false;
        private bool m_ContinuousMeasurementStartedEventRaised = false;
        private bool m_UseChecksum = true;

        private readonly object m_LockMeasurementData = new object();
        //Do not lock m_BusyLock after locking m_MeasuringLock, as this may cause a deadlock.
        //The correct order is to first lock m_BusyLock, and then m_MeasuringLock
        private readonly object m_LockBusy = new object();

        private bool m_IsMeasuring = false;

        //Do not lock m_BusyLock after locking m_MeasuringLock, as this may cause a deadlock.
        //The correct order is to first lock m_BusyLock, and then m_MeasuringLock
        private readonly object m_MeasuringLock = new object();

        private int m_Retry = 0;
        private int m_RetryMeasurement = 0;
        private bool m_IsBusy = false;
        private WakeUpState m_WakeUpState = WakeUpState.IDLE;
        private CommandData m_QueuedCommand;

        private static readonly ILog log = LogManager.GetLogger(typeof(BluetoothHal));

        #endregion Class Members

        #region Properties
        /// <summary>
        /// Gets a value indicating whether [bluetooth active].
        /// For unit testing purposes only!
        /// </summary>
        /// <value>
        ///   <c>true</c> if [bluetooth active]; otherwise, <c>false</c>.
        /// </value>
        public bool BluetoothActive
        {
            get
            {
                return m_WclVirtualComPort.Active;
            }
        }

        /// <summary>
        /// Gets a value indicating whether [serial port open].
        /// For unit testing purposes only!
        /// </summary>
        /// <value>
        ///   <c>true</c> if [serial port open]; otherwise, <c>false</c>.
        /// </value>
        public bool SerialPortOpen
        {
            get
            {
                bool serialPortOpen = m_SerialConnection != null && m_SerialConnection.SerialPortOpen;
                return serialPortOpen;
            }
        }
        #endregion Properties

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="BluetoothHal"/> class.
        /// </summary>
        public BluetoothHal()
        {

            m_QueuedCommand = new CommandData();
            SetRetryAndTimeoutSettings();
            SetUseChecksumSetting();
            m_CommandTimeoutTimer = new Timer(OnCommandTimeout, null, Timeout.Infinite, Timeout.Infinite);
            m_ConnectTimer = new Timer(OnConnectTimeout, null, Timeout.Infinite, Timeout.Infinite);
            m_FlushManometerCacheTimeoutTimer = new Timer(OnFlushManometerCacheTimeout, null, Timeout.Infinite, Timeout.Infinite);
            m_MeasurementTimer = new Timer(OnMeasurementTimerTick, null, Timeout.Infinite, Timeout.Infinite);
            m_MeasurementTimeoutTimer = new Timer(OnMeasurementTimerTimeout, null, Timeout.Infinite, Timeout.Infinite);
            m_WakeUpTimer = new Timer(OnWakeUpTimerTick, null, Timeout.Infinite, Timeout.Infinite);
            m_StoppingMeasurementTimer = new Timer(OnStoppingMeasurementTimerTimeout, null, Timeout.Infinite, Timeout.Infinite);

            CreateCommandList();
        }
        #endregion Constructor

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
                    m_CommandTimeoutTimer.Dispose();
                    m_ConnectTimer.Dispose();
                    m_FlushManometerCacheTimeoutTimer.Dispose();
                    m_MeasurementTimer.Dispose();
                    m_MeasurementTimeoutTimer.Dispose();
                    m_WakeUpTimer.Dispose();
                    m_StoppingMeasurementTimer.Dispose();

                    DetachEventHandlers();
                    try
                    {
                        Disconnect();
                    }
                    catch { }

                    try
                    {
                        if (m_SerialConnection != null)
                        {
                            m_SerialConnection.Dispose();
                        }
                    }
                    catch { }

                    try
                    {
                        if (m_VirtualComportInitialized)
                        {
                            m_WclVirtualComPort.Dispose();
                        }
                    }
                    catch { }

                    try
                    {
                        if (m_BluetoothDiscoveryInitialized)
                        {
                            m_WclBluetoothDiscovery.Dispose();
                        }
                    }
                    catch { }

                    try
                    {
                        if (m_WclApiInitialized)
                        {
                            int wclErrorCode = m_WclApi.Unload();
                            if (wclErrorCode != wclErrors.WCL_E_SUCCESS)
                            {
                                System.Diagnostics.Debug.WriteLine("Failed to unload wcl api. ErrorCode: '{0}'", wclErrorCode);
                            }
                            m_WclApi.Dispose();
                        }
                    }
                    catch { }
                }
            }

            m_Disposed = true;
        }

        #endregion IDisposable

        #region Virtual Com Port Events
        /// <summary>
        /// Handles the BeforeClose event of the m_wclVirtualComPort control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void wclVirtualComPort_BeforeClose(object sender, EventArgs e)
        {
            // do nothing
        }

        /// <summary>
        /// Handles the AfterOpen event of the m_wclVirtualComPort control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="wcl.wclVirtualPortCreatedEventArgs"/> instance containing the event data.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Inspector.Hal.BluetoothHal.HandleConnectFailed(System.String,System.Int32)"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Inspector.Hal.BluetoothHal.OnConnectFailed(System.String,System.Int32)")]
        private void wclVirtualComPort_AfterOpen(object sender, wclVirtualPortCreatedEventArgs e)
        {
            if (e.Error == 0)
            { // Port was succesfully opened. Store the portNumber
                m_SerialPortNumber = e.Port.ToString(CultureInfo.InvariantCulture);
                ConnectToSerialPort();
            }
        }
        #endregion Virtual Com Port Events

        #region Serial Port Events
        /// <summary>
        /// Handles the SerialDataErrorEvent event of the SerialConnection control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Inspector.Hal.SerialDataErrorEventArgs"/> instance containing the event data.</param>
        private void SerialConnection_SerialDataErrorEvent(object sender, SerialDataErrorEventArgs e)
        {
            m_SerialDataReceived = String.Empty;
            System.Diagnostics.Debug.WriteLine("HAL: Stopping Command Timer");
            m_CommandTimeoutTimer.Change(Timeout.Infinite, Timeout.Infinite);
            System.Diagnostics.Debug.WriteLine("HAL: Stopping Flush Manometer Cache Timer");
            m_FlushManometerCacheTimeoutTimer.Change(Timeout.Infinite, Timeout.Infinite);
            System.Diagnostics.Debug.WriteLine("HAL: Stopping Measurement Timer");
            m_MeasurementTimer.Change(Timeout.Infinite, Timeout.Infinite);
            System.Diagnostics.Debug.WriteLine("HAL: Stopping Measurement Timeout Timer");
            m_MeasurementTimeoutTimer.Change(Timeout.Infinite, Timeout.Infinite);
            System.Diagnostics.Debug.WriteLine("HAL: Stopping Wake Up Timer");
            m_WakeUpTimer.Change(Timeout.Infinite, Timeout.Infinite);
            m_WakeUpState = WakeUpState.IDLE;
            OnMessageReceivedError(e.Message, e.ErrorCode);
        }

        /// <summary>
        /// Handles the SerialDataEvent event of the SerialConnection control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Inspector.Hal.SerialDataEventArgs"/> instance containing the event data.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1307:SpecifyStringComparison", MessageId = "System.String.EndsWith(System.String)")]
        private void SerialConnection_SerialDataEvent(object sender, SerialDataEventArgs e)
        {
            if (m_CurrentCommand != DeviceCommand.None)
            {
                RestartCommandTimer();

                m_SerialDataReceived += e.SerialData;
                switch (m_CurrentCommand)
                {
                    case DeviceCommand.EnterRemoteLocalCommandMode:
                    case DeviceCommand.ExitRemoteLocalCommandMode:
                    case DeviceCommand.SwitchInitializationLedOn:
                    case DeviceCommand.SwitchInitializationLedOff:
                    case DeviceCommand.FlushBluetoothCache:
                        ParseDeviceData();
                        break;
                    case DeviceCommand.Wakeup:
                        lock (m_LockBusy)
                        {
                            ParseWakeUpCall();
                        }
                        break;
                    case DeviceCommand.CheckIdentification:
                        ParseManometerData();
                        break;

                    case DeviceCommand.FlushManometerCache:
                    case DeviceCommand.CheckBatteryStatus:
                    case DeviceCommand.CheckSCPIInterface:
                    case DeviceCommand.InitiateSelfTest:
                    case DeviceCommand.CheckRange:
                    case DeviceCommand.SetPressureUnit:
                    case DeviceCommand.CheckPressureUnit:
                    case DeviceCommand.CheckManometerPresent:
                    case DeviceCommand.IRAlwaysOn:
                        ParseManometerData();
                        break;

                    case DeviceCommand.SwitchToManometerTH1:
                    case DeviceCommand.SwitchToManometerTH2:
                    case DeviceCommand.Connect:
                    case DeviceCommand.Disconnect:
                    case DeviceCommand.MeasureContinuously:
                    case DeviceCommand.None:
                    default:
                        break;
                }
            }
        }



        /// <summary>
        /// Handles the SerialDataEvent event of the SerialConnection control for measurements.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Inspector.Hal.SerialDataEventArgs"/> instance containing the event data.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1307:SpecifyStringComparison", MessageId = "System.String.EndsWith(System.String)")]
        void SerialConnection_SerialDataEvent_Measurement(object sender, SerialDataEventArgs e)
        {

            if (m_CurrentCommand != DeviceCommand.None)
            {
                m_MeasurementTimeoutTimer.Change(m_MeasurementTimeout, Timeout.Infinite);
                lock (m_LockMeasurementData)
                {
                    m_MeasurementsReceived += e.SerialData;
                    if (!m_ContinuousMeasurementStartedEventRaised)
                    {
                        OnContinuousMeasurementStarted();
                        m_ContinuousMeasurementStartedEventRaised = true;
                    }
                }

            }
        }

        /// <summary>
        /// Handles the StoppingMeasurement event of the SerialConnection_SerialDataEvent control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Inspector.Hal.SerialDataEventArgs"/> instance containing the event data.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1307:SpecifyStringComparison", MessageId = "System.String.EndsWith(System.String)")]
        void SerialConnection_SerialDataEvent_StoppingMeasurement(object sender, SerialDataEventArgs e)
        {
            if (m_CurrentCommand != DeviceCommand.None)
            {
                StopCommandTimer();
                m_SerialDataReceived += e.SerialData;

                if (m_StoppedMeasurementTimedOut)
                {
                    System.Diagnostics.Debug.WriteLine("HAL: SerialConnection_SerialDataEvent_StoppingMeasurement m_StoppedMeasurementTimedOut == true");
                    // Any answer ending on \r is okay :)
                    if (m_SerialDataReceived.EndsWith("\r"))
                    {
                        System.Diagnostics.Debug.WriteLine("HAL: SerialConnection_SerialDataEvent_StoppingMeasurement if (m_SerialDataReceived.EndsWith(\"\\r\")) == true");
                        m_StoppingMeasurementTimer.Change(Timeout.Infinite, Timeout.Infinite);
                        m_SerialConnection.SerialDataEvent -= new EventHandler<SerialDataEventArgs>(SerialConnection_SerialDataEvent_StoppingMeasurement);
                        m_SerialDataReceived = String.Empty;
                        m_SerialConnection.SerialDataEvent += new EventHandler<SerialDataEventArgs>(SerialConnection_SerialDataEvent);
                        OnContinuousMeasurementStopped();
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("HAL: SerialConnection_SerialDataEvent_StoppingMeasurement m_StoppedMeasurementTimedOut == false");
                    System.Diagnostics.Debug.WriteLine("HAL: OK DATA RECEIVED: '{0}'", new object[] { m_SerialDataReceived.Replace("\t", "[HT]").Replace("\r", "[CR]").Replace("\n", "[LF]") });
                    if (m_SerialDataReceived.EndsWith("ok\t*13\r", StringComparison.OrdinalIgnoreCase))
                    {
                        System.Diagnostics.Debug.WriteLine("HAL: SerialConnection_SerialDataEvent_StoppingMeasurement if (m_SerialDataReceived.EndsWith(@\"ok\t*13\\r\", StringComparison.OrdinalIgnoreCase)) == true");
                        m_StoppingMeasurementTimer.Change(Timeout.Infinite, Timeout.Infinite);
                        m_SerialConnection.SerialDataEvent -= new EventHandler<SerialDataEventArgs>(SerialConnection_SerialDataEvent_StoppingMeasurement);
                        m_SerialDataReceived = String.Empty;
                        m_SerialConnection.SerialDataEvent += new EventHandler<SerialDataEventArgs>(SerialConnection_SerialDataEvent);
                        OnContinuousMeasurementStopped();
                    }
                }
            }
        }

        /// <summary>
        /// Stops the command timer.
        /// </summary>
        private void StopCommandTimer()
        {
            bool isFlushingManometerCache = (m_CurrentCommand == DeviceCommand.FlushManometerCache);
            if (isFlushingManometerCache)
            {
                System.Diagnostics.Debug.WriteLine("HAL: Stopping Flush Manometer Cache Timer");
                m_FlushManometerCacheTimeoutTimer.Change(Timeout.Infinite, Timeout.Infinite);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("HAL: Stopping Command Timer");
                m_CommandTimeoutTimer.Change(Timeout.Infinite, Timeout.Infinite);
            }
        }

        /// <summary>
        /// Restarts the command timer.
        /// </summary>
        private void RestartCommandTimer()
        {
            bool isFlushingManometerCache = (m_CurrentCommand == DeviceCommand.FlushManometerCache);
            if (isFlushingManometerCache)
            {
                System.Diagnostics.Debug.WriteLine("HAL: Restarting Flush Manometer Cache Timer");
                m_FlushManometerCacheTimeoutTimer.Change(m_FlushManometerCacheTimeout, Timeout.Infinite);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("HAL: Restarting Command Timer");
                m_CommandTimeoutTimer.Change(m_ManometerCommandTimeout, Timeout.Infinite);
            }
        }

        #endregion Serial Port Events

        #region IHal
        /// <summary>
        /// Retrieves the available bluetooth dongles.
        /// </summary>
        /// <param name="bluetoothApi">The bluetooth API.</param>
        /// <returns>The available bluetooth dongles for the specified API</returns>
        /// <exception cref="ConnectionException">Thrown when Bluetooth Discovery failed</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "result"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "errorMessage"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists")]
        public List<string> RetrieveAvailableBluetoothDongles(string bluetoothApi)
        {
            List<string> availableBluetoothDongles = new List<string>();

            IntializeWclApi();

            if (!m_BluetoothDiscoveryInitialized)
            {
                m_WclBluetoothDiscovery = new wclBluetoothDiscovery();
            }

            wclBluetoothRadios radios = new wcl.wclBluetoothRadios();
            int errorCode = m_WclBluetoothDiscovery.EnumRadios(radios);
            if (errorCode == 0)
            {
                if (radios.Count > 0)
                {
                    for (uint i = 0; i < radios.Count; i++)
                    {
                        string btApi = radios[i].API.ToString();
                        if (btApi == bluetoothApi)
                        {
                            string btAddress = string.Empty;
                            int result = radios[i].GetAddress(ref btAddress);
                            availableBluetoothDongles.Add(btAddress);
                        }
                    }
                }
            }
            else
            {
                string errorMessage = wclErrors.wclGetErrorMessage(errorCode);
                throw new ConnectionException(errorCode);
            }

            return availableBluetoothDongles;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the HAL is signaled that a command is to be expected.
        /// </summary>
        /// <value><c>true</c> if a command is expected; otherwise, <c>false</c>.</value>
        public bool IsBusy
        {
            get
            {
                lock (m_LockBusy)
                {
                    return m_IsBusy;
                }
            }
            set
            {
                lock (m_LockBusy)
                {
                    if (m_IsBusy != value)
                    {
                        if (value)
                        {
                            System.Diagnostics.Debug.WriteLine("IsBusy true: disabling wakeup timer");
                            m_WakeUpTimer.Change(Timeout.Infinite, Timeout.Infinite);
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine("IsBusy false: enabling wakeup timer (to: '{0}' ms)", new object[] { m_WakeUpInterval });
                            m_WakeUpTimer.Change(m_WakeUpInterval, Timeout.Infinite);
                        }
                        m_IsBusy = value;
                    }
                }
            }
        }

        /// <summary>
        /// Connects the specified bluetooth API.
        /// </summary>
        /// <param name="connectionProperties">The connection properties.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Inspector.Hal.BluetoothHal.OnConnectFailed(System.String,System.Int32)")]
        public void Connect(Dictionary<string, string> connectionProperties, List<BluetoothDongleInformation> allowedBluetoothDongles)
        {
            System.Diagnostics.Debug.WriteLine("Connecting");
            m_ConnectionParameters = connectionProperties;
            m_AllowedBluetoothDongles = allowedBluetoothDongles;

            m_ConnectTimer.Change(m_ConnectTimeout, Timeout.Infinite);
            try
            {
                IntializeWclApi();

                if (!m_VirtualComportInitialized)
                {
                    m_WclVirtualComPort = new wclVirtualCOMPort();
                    AttachEventHandlers();

                    m_VirtualComportInitialized = true;
                }

                VerifyBluetoothDongle(allowedBluetoothDongles);

                if (connectionProperties == null)
                {
                    throw new ConnectionException("No connection properties has been filled in.", ErrorCodes.HAL_CONNECTION_PROPERTIES_EMPTY);
                }

                ValidateConnectionProperties(connectionProperties);
                string bluetoothApi = connectionProperties[CONNECTIONPROPERTY_BLUETOOTHAPI];
                string destinationAddress = connectionProperties[CONNECTIONPROPERTY_DESTINATIONADDRESS];

                if (!m_WclVirtualComPort.Active)
                {
                    CreateVirtualComPort(bluetoothApi, destinationAddress);
                }
                else
                {
                    throw new ConnectionException("Bluetooth Connection already active", ErrorCodes.HAL_CONNECTION_ALREADY_ACTIVE);
                }
            }
            catch (ConnectionException ex)
            {
                System.Diagnostics.Debug.WriteLine("HAL: Connect error " +  ex.Message);
                HandleConnectFailed(ex.Message, ex.ErrorCode);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("HAL: Connect error " + ex.Message);
                HandleConnectFailed(ex.Message, ErrorCodes.HAL_UNEXPECTED_ERROR);
            }
        }

        /// <summary>
        /// Intializes the WCL API.
        /// </summary>
        private void IntializeWclApi()
        {
            if (!m_WclApiInitialized)
            {
                m_WclApi = new wclAPI();
                m_WclApi.Load();

                m_WclApiInitialized = true;
            }
        }

        /// <summary>
        /// Disconnects the Bluetooth connection.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        public void Disconnect()
        {
            m_CommandTimeoutTimer.Change(Timeout.Infinite, Timeout.Infinite);
            m_ConnectTimer.Change(Timeout.Infinite, Timeout.Infinite);
            m_FlushManometerCacheTimeoutTimer.Change(Timeout.Infinite, Timeout.Infinite);
            m_MeasurementTimer.Change(Timeout.Infinite, Timeout.Infinite);
            m_MeasurementTimeoutTimer.Change(Timeout.Infinite, Timeout.Infinite);
            m_WakeUpTimer.Change(Timeout.Infinite, Timeout.Infinite);
            m_StoppingMeasurementTimer.Change(Timeout.Infinite, Timeout.Infinite);

            System.Threading.Thread disconnectWorkerThread = new Thread(() => DisconnectWorkerThread());
            disconnectWorkerThread.Name = "disconnectWorkerThread";
            disconnectWorkerThread.Start();
        }

        /// <summary>
        /// Disconnects the worker thread.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        public void DisconnectWorkerThread()
        {
            if (m_SerialConnection != null)
            {
                m_SerialConnection.DetachEvents();
            }

            System.Diagnostics.Debug.WriteLine("Disconnecting");
            try
            {
                if (m_SerialConnection != null)
                {
                    m_SerialConnection.Disconnect();
                }
            }
            catch
            {
                System.Diagnostics.Debug.WriteLine("HAL: Failed to close the serial connection");
                // Just continue as we have no feedback about disconnect
            }

            try
            {

                if (m_WclVirtualComPort.Active)
                {
                    m_WclVirtualComPort.Close();
                }
            }
            catch
            {
                System.Diagnostics.Debug.WriteLine("HAL: Failed to close the wcl virtual comport connection");

                // Just continue as we have no feedback about disconnect
            }

            OnDisconnected();
        }

        /// <summary>
        /// Sends the command.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="commandParameter">The parameter.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.String.Format(System.String,System.Object)")]
        public void SendCommand(DeviceCommand command, string commandParameter)
        {
            lock (m_LockBusy)
            {
                m_SerialDataReceived = String.Empty;

                if (m_WakeUpState != WakeUpState.IDLE && command != DeviceCommand.Wakeup)
                {
                    System.Diagnostics.Debug.WriteLine("HAL: SendCommand queueing command " + command.ToString());
                    m_QueuedCommand.Command = command;
                    m_QueuedCommand.Parameter = commandParameter;
                }
                else if (m_WakeUpState == WakeUpState.IDLE && command == DeviceCommand.Wakeup)
                {
                    System.Diagnostics.Debug.WriteLine("HAL: SendQueuedCommand instead of wakeup");
                    SendQueuedCommandOrDoWakeUp();
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("HAL: SendCommand " + command.ToString());
                    m_CurrentCommand = command;
                    m_CurrentCommandParameter = commandParameter;
                    switch (command)
                    {
                        case DeviceCommand.EnterRemoteLocalCommandMode:
                            DoEnterRemoteLocalCommandMode(command);
                            break;


                        case DeviceCommand.ExitRemoteLocalCommandMode:
                        case DeviceCommand.SwitchInitializationLedOn:
                        case DeviceCommand.SwitchInitializationLedOff:
                        case DeviceCommand.FlushBluetoothCache:
                            DoSendDeviceCommand(command, commandParameter);
                            break;

                        case DeviceCommand.SwitchToManometerTH1:
                            DoSwitchToManometerTh1(commandParameter);
                            break;

                        case DeviceCommand.SwitchToManometerTH2:
                            DoSwitchToManometerTh2(commandParameter);
                            break;

                        case DeviceCommand.CheckManometerPresent:
                        case DeviceCommand.CheckBatteryStatus:
                        case DeviceCommand.CheckSCPIInterface:
                        case DeviceCommand.InitiateSelfTest:
                        case DeviceCommand.CheckIdentification:
                        case DeviceCommand.CheckRange:
                        case DeviceCommand.SetPressureUnit:
                        case DeviceCommand.CheckPressureUnit:
                            DoSendManometerCommand(command, commandParameter);
                            break;

                        case DeviceCommand.IRAlwaysOn:
                            DoSendManometerCommand(command, string.Empty);
                            break;

                        case DeviceCommand.FlushManometerCache:
                            DoSendFlushManometerCommand(command, commandParameter);
                            break;

                        case DeviceCommand.MeasureContinuously:
                            try
                            {
                                DoSendCommand(command, commandParameter);
                            }
                            catch (ChecksumException) { /* Errors are already handled */ }
                            break;

                        case DeviceCommand.Wakeup:
                            m_CurrentCommandParameter = String.Empty;
                            DoSendManometerCommand(command, String.Empty);
                            break;

                        case DeviceCommand.None:
                        case DeviceCommand.Connect:
                        case DeviceCommand.Disconnect:
                        default:
                            break;
                    }
                }
            }
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
        /// Starts the continuous measurement.
        /// </summary>
        /// <param name="frequency">The frequency in measurements per second.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Inspector.Hal.BluetoothHal.OnMessageReceivedError(System.String,System.Int32)")]
        public void StartContinuousMeasurement(int frequency)
        {
            lock (m_LockBusy)
            {
                m_RetryMeasurement = 0;
                if (m_WakeUpState != WakeUpState.IDLE)
                {
                    System.Diagnostics.Debug.WriteLine("HAL: StartContinuousMeasurement queueing command " + DeviceCommand.MeasureContinuously.ToString());
                    m_QueuedCommand.Command = DeviceCommand.MeasureContinuously;
                    m_QueuedCommand.Parameter = frequency.ToString(CultureInfo.InvariantCulture);
                }
                else
                {
                    DoStartContinuousMeasurement(frequency.ToString(CultureInfo.InvariantCulture));
                }
            }
        }

        private void DoStartContinuousMeasurement(string frequency)
        {
            lock (m_MeasuringLock)
            {
                m_SerialConnection.SerialDataEvent -= new EventHandler<SerialDataEventArgs>(SerialConnection_SerialDataEvent);
                m_SerialConnection.SerialDataEvent += new EventHandler<SerialDataEventArgs>(SerialConnection_SerialDataEvent_Measurement);

                try
                {
                    SendCommand(DeviceCommand.MeasureContinuously, frequency);
                    m_MeasurementTimer.Change(m_MeasurementInterval, Timeout.Infinite);
                    m_MeasurementTimeoutTimer.Change(m_MeasurementTimeout, Timeout.Infinite);
                }
                catch (ChecksumException)
                {
                    System.Diagnostics.Debug.WriteLine("HAL: StartContinuousMeasurement, invalid checksum error.");
                    m_SerialConnection.SerialDataEvent -= new EventHandler<SerialDataEventArgs>(SerialConnection_SerialDataEvent_Measurement);
                    m_SerialConnection.SerialDataEvent += new EventHandler<SerialDataEventArgs>(SerialConnection_SerialDataEvent);
                    m_ContinuousMeasurementStartedEventRaised = false;
                    OnMessageReceivedError("Failed to start continuous measurement.", ErrorCodes.HAL_CONTINUOUS_MEASUREMENT_START_FAILED);
                }
            }
        }

        /// <summary>
        /// Stops the continuous measurement.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Inspector.Hal.BluetoothHal.OnMessageReceivedError(System.String,System.Int32)")]
        public void StopContinuousMeasurement()
        {
            lock (m_MeasuringLock)
            {
                System.Diagnostics.Debug.WriteLine("HAL: Begin of StopContinuousMeasurement");
                m_SerialConnection.SerialDataEvent -= new EventHandler<SerialDataEventArgs>(SerialConnection_SerialDataEvent_Measurement);
                m_SerialConnection.SerialDataEvent += new EventHandler<SerialDataEventArgs>(SerialConnection_SerialDataEvent_StoppingMeasurement);
                m_ContinuousMeasurementStartedEventRaised = false;

                lock (m_LockMeasurementData)
                {
                    m_MeasurementsReceived = String.Empty;
                }

                try
                {
                    m_MeasurementTimer.Change(Timeout.Infinite, Timeout.Infinite);
                    m_MeasurementTimeoutTimer.Change(Timeout.Infinite, Timeout.Infinite);
                    m_SerialDataReceived = String.Empty;
                    m_StoppedMeasurementTimedOut = false;
                    SendCommand(DeviceCommand.MeasureContinuously, "0");
                    m_StoppingMeasurementTimer.Change(FINALIZE_MEASUREMENT_WAIT_TIME, Timeout.Infinite);
                }
                catch (ChecksumException)
                {
                    m_SerialConnection.SerialDataEvent -= new EventHandler<SerialDataEventArgs>(SerialConnection_SerialDataEvent_StoppingMeasurement);
                    m_SerialConnection.SerialDataEvent += new EventHandler<SerialDataEventArgs>(SerialConnection_SerialDataEvent);

                    System.Diagnostics.Debug.WriteLine("HAL: StopContinuousMeasurement, invalid checksum error.");
                    OnMessageReceivedError("Failed to stop continuous measurement.", ErrorCodes.HAL_CONTINUOUS_MEASUREMENT_STOP_FAILED);
                }
                System.Diagnostics.Debug.WriteLine("HAL: End of StopContinuousMeasurement");
                m_IsMeasuring = false;
            }
        }

        #region Event and event handlers
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
                System.Diagnostics.Debug.WriteLine("OnContinuousMeasurementStopped called");
                ContinuousMeasurementStopped(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Occurs when [continuous measurement started]
        /// </summary>
        public event EventHandler ContinuousMeasurementStarted;

        /// <summary>
        /// Called when [continuous measurement started]
        /// </summary>
        private void OnContinuousMeasurementStarted()
        {
            if (ContinuousMeasurementStarted != null)
            {
                System.Diagnostics.Debug.WriteLine("OnContinuousMeasurementStarted called");
                ContinuousMeasurementStarted(this, EventArgs.Empty);
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
            if (m_IsBusy)
            {
                if (MessageReceived != null)
                {
                    MessageReceived(this, new MessageReceivedEventArgs(data));
                }
            }

        }

        /// <summary>
        /// Occurs when [message received error].
        /// </summary>
        public event EventHandler MessageReceivedError;

        /// <summary>
        /// Called when [message received].
        /// </summary>
        /// <param name="data">The data.</param>
        private void OnMessageReceivedError(string message, int errorCode)
        {
            if (m_IsBusy)
            {
                m_CurrentCommand = DeviceCommand.None;
                if (MessageReceivedError != null)
                {
                    MessageReceivedError(this, new MessageErrorEventArgs(message, errorCode));
                }
            }
            else
            {
                //this means we are receiving an error while we are:
                //1: not expecting anything (devicecommand.none)
                //2: in a wakeup state
                //both of these indicate that we are in the state 'connected' in which we cannot handle errors, so we don't.
            }
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
                Connected(this, EventArgs.Empty);
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
                Disconnected(this, EventArgs.Empty);
            }
        }
        #endregion Event and event handlers
        #endregion IHal

        #region Public functions
        /// <summary>
        /// Computes the checksum.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns>The checksum</returns>
        /// <exception cref="ChecksumException">Thrown when <paramref name="command"/> is invalid./></exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.Int32.ToString(System.String)"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.Int32.Parse(System.String,System.Globalization.NumberStyles)")]
        internal static int ComputeChecksum(string command)
        {
            int checksum = 0;

            try
            {
                string chkSum = command.Sum(b => b).ToString("X");
                chkSum = chkSum.Substring(chkSum.Length - 2);
                checksum = int.Parse(chkSum, System.Globalization.NumberStyles.HexNumber);
            }
            catch
            {
                throw new ChecksumException("Failed to compute Checksum.");
            }
            return checksum;
        }

        // this function is called only from the UI, which is a violation of the design (skipping over all layers)
        // this is 
        public bool ForceConnectionClose()
        {
            try
            {
                if (m_IsMeasuring)
                {
                    StopContinuousMeasurement();
                }
                if (m_IsMeasuring)
                {
                    System.Diagnostics.Debug.WriteLine("unable to stop continuous measurement");
                }

                if (m_SerialConnection != null)
                {
                    System.Diagnostics.Debug.WriteLine("disconnecting serial connection");
                    m_SerialConnection.Disconnect();
                }

                if (m_WclVirtualComPort != null)
                {
                    System.Diagnostics.Debug.WriteLine("destroying virtual com port");
                    m_WclVirtualComPort.Close();
                }
                System.Diagnostics.Debug.WriteLine("disconnecting succeeded");
                return true;
            }
            catch
            {
                System.Diagnostics.Debug.WriteLine("disconnecting failed");
                return false;
                //we are already handling unhandled exceptions. Throwing new exceptions is a bad idea.
            }

        }
        #endregion Public functions

        #region Private functions
        /// <summary>
        /// Stops the continuous measurement without sending an error via OnMessageReceivedError event.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        private void StopContinuousMeasurementOnError()
        {
            lock (m_MeasuringLock)
            {
                m_SerialConnection.SerialDataEvent -= new EventHandler<SerialDataEventArgs>(SerialConnection_SerialDataEvent_Measurement);
                lock (m_LockMeasurementData)
                {
                    m_ContinuousMeasurementStartedEventRaised = false;
                }
                try
                {
                    m_MeasurementTimer.Change(Timeout.Infinite, Timeout.Infinite);
                    m_MeasurementTimeoutTimer.Change(Timeout.Infinite, Timeout.Infinite);
                    SendCommand(DeviceCommand.MeasureContinuously, "0");
                    // Wait some seconds to make sure no more data is received
                    Thread.Sleep(FINALIZE_MEASUREMENT_WAIT_TIME);
                    lock (m_LockMeasurementData)
                    {
                        m_MeasurementsReceived = String.Empty;
                    }

                }
                catch { /* OnMessageReceivedError must be handled by the caller of this function */ }

                m_SerialConnection.SerialDataEvent += new EventHandler<SerialDataEventArgs>(SerialConnection_SerialDataEvent);
                m_IsMeasuring = false;
            }
        }

        /// <summary>
        /// Called when [wake up timer tick].
        /// </summary>
        /// <param name="state">The state.</param>
        internal void OnWakeUpTimerTick(object state)
        {
            lock (m_LockBusy)
            {
                if (!IsBusy)
                {
                    System.Diagnostics.Debug.WriteLine("HAL: OnWakeUpTimerTick performed");
                    m_WakeUpState = WakeUpState.MANOMETER1;
                    SendQueuedCommandOrDoWakeUp();
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("HAL: OnWakeUpTimerTick not performed becuase IsBusy == true");
                }
            }
        }

        /// <summary>
        /// Called when [stopping measurement timer timeout].
        /// </summary>
        /// <param name="state">The state.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Inspector.Hal.BluetoothHal.OnMessageReceivedError(System.String,System.Int32)")]
        private void OnStoppingMeasurementTimerTimeout(object state)
        {
            if (m_StoppedMeasurementTimedOut)
            {
                System.Diagnostics.Debug.WriteLine("OnStoppingMeasurementTimerTimeout m_StoppedMeasurementTimedOut == true");
                m_SerialConnection.SerialDataEvent -= new EventHandler<SerialDataEventArgs>(SerialConnection_SerialDataEvent_StoppingMeasurement);
                m_SerialDataReceived = String.Empty;
                m_StoppedMeasurementTimedOut = false;
                m_SerialConnection.SerialDataEvent += new EventHandler<SerialDataEventArgs>(SerialConnection_SerialDataEvent);
                OnMessageReceivedError("Failed to stop the continuous measurement", ErrorCodes.HAL_CONTINUOUS_MEASUREMENT_STOP_FAILED);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("OnStoppingMeasurementTimerTimeout m_StoppedMeasurementTimedOut == false");
                m_SerialDataReceived = String.Empty;
                m_StoppedMeasurementTimedOut = true;
                System.Diagnostics.Debug.WriteLine("OnStoppingMeasurementTimerTimeout doing sendcommand flushamnometercache");
                SendCommand(DeviceCommand.FlushManometerCache);
                m_StoppingMeasurementTimer.Change(FINALIZE_MEASUREMENT_WAIT_TIME, Timeout.Infinite);
            }
        }

        /// <summary>
        /// Determine if the manometer has send an errorcode reply and parse the error.
        /// </summary>
        /// <param name="reply">The reply.</param>
        /// <param name="errorCode">The error code that corresponds with the manometer error if the reply contains an error; otherwise null.</param>
        /// <returns>
        /// 	<c>True</c> if the manometer's reply contained an error; otherwise <c>false</c>
        /// </returns>
        private static bool ManometerReplyContainsErrorCode(string reply, out int? errorCode)
        {
            bool messageContainsError = false;
            errorCode = null;

            if (Regex.IsMatch(reply, ManometerErrorReply.ERRORMESSAGE_FORMAT_REGEX))
            {
                messageContainsError = ManometerErrorReply.ErrorCodeLookup.ContainsKey(reply);
                if (messageContainsError)
                {
                    errorCode = ManometerErrorReply.ErrorCodeLookup[reply];
                }
                else
                {
                    // an errorcode is returned that is unknown to us
                    errorCode = ErrorCodes.MANOMETER_UNKNOWN_ERROR;
                }
            }

            return messageContainsError;
        }

        /// <summary>
        /// Parses the received device data if the data is complete.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Inspector.Hal.BluetoothHal.OnMessageReceivedError(System.String,System.Int32)")]
        private void ParseDeviceData()
        {
            if (m_SerialDataReceived.EndsWith("\n", StringComparison.OrdinalIgnoreCase))
            {
                m_Retry = 0;
                System.Diagnostics.Debug.WriteLine("HAL: Stopping Command Timer: SendDeviceData");
                m_CommandTimeoutTimer.Change(Timeout.Infinite, Timeout.Infinite);
                m_CurrentCommand = DeviceCommand.None;

                string dataReceived = m_SerialDataReceived;

                // Regex validates following inputformats that can be received from the device:
                //"\r\ndata\r\n"              --> Reaction on Bluetooth serial mode
                string regexStr = "^.*\r\n(?<Data>.+?)\r\n$";
                Match m = Regex.Match(dataReceived, regexStr, RegexOptions.Compiled);
                string data = m.Groups["Data"].ToString();
                if (m.Success)
                {
                    OnMessageReceived(data);
                }
                else
                {
                    string message = String.Format(CultureInfo.InvariantCulture, "Unknown data format received from Plexor device: '{0}'", m_SerialDataReceived);
                    OnMessageReceivedError(message, ErrorCodes.HAL_MESSAGE_RECEIVED_INCORRECT_FORMAT);
                }

                m_SerialDataReceived = String.Empty;
            }
        }

        /// <summary>
        /// Parses the wake up call data if the data is complete.
        /// </summary>
        private void ParseWakeUpCall()
        {
            if (m_SerialDataReceived.EndsWith("\r", StringComparison.OrdinalIgnoreCase))
            {
                lock (m_LockBusy)
                {
                    System.Diagnostics.Debug.WriteLine("HAL: SendWakeUpCall '{0}'", new object[] { m_SerialDataReceived });
                    StopCommandTimer();
                    m_CurrentCommand = DeviceCommand.None;
                    m_Retry = 0;
                    m_SerialDataReceived = String.Empty;

                    System.Diagnostics.Debug.WriteLine("HAL: SendWakeUpCall received wake up reply from manometer '{0}'", new object[] { m_WakeUpState.ToString() });


                    SendQueuedCommandOrDoWakeUp();
                }
            }
        }

        /// <summary>
        /// Parses the manometer data if the data is complete.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2241:Provide correct arguments to formatting methods"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Inspector.Hal.BluetoothHal.OnMessageReceivedError(System.String,System.Int32)")]
        private void ParseManometerData()
        {
            if (m_SerialDataReceived.EndsWith("\r", StringComparison.OrdinalIgnoreCase))
            {
                System.Diagnostics.Debug.WriteLine("HAL: SendManometerData received '{0}'", new object[] { m_SerialDataReceived });

                StopCommandTimer();

                bool requiresManometerErrorCheck = (m_CurrentCommand != DeviceCommand.FlushManometerCache);
                m_CurrentCommand = DeviceCommand.None;
                m_Retry = 0;

                string dataReceived = m_SerialDataReceived;

                int checksum;
                int computedChecksum;

                try
                {
                    string data = ParseManometerData(dataReceived, out checksum, out computedChecksum);
                    if (!m_UseChecksum)
                    {
                        OnMessageReceived(data);
                    }
                    else
                    {
                        if (checksum == computedChecksum)
                        {
                            int? manometerErrorCode = null;

                            if (requiresManometerErrorCheck && ManometerReplyContainsErrorCode(data, out manometerErrorCode))
                            {
                                string message = String.Format(CultureInfo.InvariantCulture, "The manometer replied with an error: '{0}' ", data);

                                OnMessageReceivedError(message, manometerErrorCode.Value);
                            }
                            else
                            {
                                OnMessageReceived(data);
                            }
                        }
                        else
                        {
                            log.Info(string.Format("Error in checksum:\n\tDatareceived: {0}\n\tdata: {1}\n\tchecksum: {2}\n\tcomputedChecksum: {3}",
                                dataReceived,
                                data,
                                checksum.ToString(),
                                computedChecksum.ToString()));
                            OnMessageReceivedError(String.Format(CultureInfo.InvariantCulture, "Checksum for received message '{0}' incorrect (Expected: '{1}', Was: '{2}'", dataReceived, computedChecksum, checksum), ErrorCodes.HAL_MESSAGE_RECEIVED_INCORRECT_CHECKSUM);
                        }
                    }
                }
                catch (ChecksumException e)
                {
                    System.Diagnostics.Debug.WriteLine("HAL: SendManometerData checksumerror detected!");
                    log.Info(string.Format("Exception in checksum:\n\tDatareceived: {0}\n\tException message: {1}\n\tException data: {2}",
                        dataReceived,
                        e.Message,
                        e.Data));
                    OnMessageReceivedError(String.Format(CultureInfo.InvariantCulture, "Could not calculate the checksum for received message '{0}'.", dataReceived), ErrorCodes.HAL_MESSAGE_RECEIVED_INCORRECT_CHECKSUM);
                }
                catch (InvalidDataException e)
                {
                    System.Diagnostics.Debug.WriteLine("HAL: SendManometerData invalid data exception!");
                    log.Info(string.Format("Exception in SendManometerData:\n\tDatareceived: {0}\n\tException message: {1}\n\tException data: {2}",
                        dataReceived,
                        e.Message,
                        e.Data));
                    OnMessageReceivedError(String.Format(CultureInfo.InvariantCulture, "The received messsage has an incorrect format.", dataReceived), ErrorCodes.HAL_MESSAGE_RECEIVED_INCORRECT_FORMAT);
                }
                m_SerialDataReceived = String.Empty;
            }
        }

        private void SetUseChecksumSetting()
        {
            string settingValue = String.Empty;
            try
            {
                clsSettings settings = new clsSettings();
                settingValue = settings.get_GetSetting(SETTING_CATEGORY, SETTING_USE_CHECKSUM).ToString();
                if (!settingValue.Equals(SETTING_RETURN_NO_VALUE, StringComparison.OrdinalIgnoreCase))
                {
                    m_UseChecksum = bool.Parse(settingValue);
                }
            }
            catch
            {
                System.Diagnostics.Debug.WriteLine("getting useCheckSum setting failed, using default value");
            }
        }

        /// <summary>
        /// Parses the manometer data.
        /// </summary>
        /// <param name="dataReceived">The data received.</param>
        /// <param name="checksum">The checksum.</param>
        /// <param name="computedChecksum">The computed checksum.</param>
        /// <returns>The data part of the <paramref name="dataReceived"/></returns>
        /// <exception cref="ChecksumException">Thrown when the checksum can not be computed of <paramref name="dataReceived"/>.</exception>
        /// <exception cref="InvalidDataException">Thrown when the received data is invalid.</exception>
        private static string ParseManometerData(string dataReceived, out int checksum, out int computedChecksum)
        {
            string data = String.Empty;
            checksum = int.MinValue;
            computedChecksum = int.MinValue;

            // Regex validates following inputformats that can be received from the device:
            //"data\t\r"            --> Reaction from Manometer without Checksum
            //"data\t*checksum\r"   --> Reaction from Manometer with checksum. data may be empty.
            string regexStr = "^(?<Data>.*?)[\t|\r]{1,2}(\\*(?<Checksum>.+)\r)?$";
            Match m = Regex.Match(dataReceived, regexStr, RegexOptions.Compiled);

            if (m.Success)
            {
                data = m.Groups["Data"].ToString();
                System.Diagnostics.Debug.WriteLine("DataReceived: " + dataReceived);
                System.Diagnostics.Debug.WriteLine("Data: " + data);
                System.Diagnostics.Debug.WriteLine("Checksum: " + m.Groups["Checksum"].ToString());

                try
                {
                    bool requiresChecksumValidation = m.Groups["Checksum"].Success;
                    if (requiresChecksumValidation)
                    {
                        checksum = int.Parse(m.Groups["Checksum"].ToString(), CultureInfo.InvariantCulture);
                        computedChecksum = ComputeChecksum(data + "\t*");
                    }
                }
                catch (Exception)
                {
                    throw new InvalidDataException("Invalid manometer data received.");
                }
            }
            else
            {
                throw new InvalidDataException("Invalid manometer data received.");
            }

            return data;
        }

        /// <summary>
        /// Does the enter remote local command mode.
        /// </summary>
        /// <param name="command">The command.</param>
        private void DoEnterRemoteLocalCommandMode(DeviceCommand command)
        {
            // Enter Remote local command mode: Send three "!" with pauses of 150 ms between them.
            string commandString = m_SerialCommands[command];
            System.Threading.Thread.Sleep(250);
            m_SerialConnection.SendCommand(commandString);
            System.Threading.Thread.Sleep(250);
            m_SerialConnection.SendCommand(commandString);
            System.Threading.Thread.Sleep(250);
            m_SerialConnection.SendCommand(commandString);
            System.Threading.Thread.Sleep(250);
            m_CommandTimeoutTimer.Change(m_ManometerCommandTimeout, Timeout.Infinite);
        }

        /// <summary>
        /// Does the switch to manometer TH2.
        /// </summary>
        /// <param name="commandParameter">The command parameter.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Inspector.Hal.BluetoothHal.OnMessageReceivedError(System.String,System.Int32)")]
        private void DoSwitchToManometerTh2(string commandParameter)
        {
            try
            {
                if (commandParameter.Equals("Low"))
                {
                    m_SerialConnection.DtrEnabled = true;
                }
                else if (commandParameter.Equals("High"))
                {
                    m_SerialConnection.DtrEnabled = false;
                }
                OnMessageReceived(String.Empty);
            }
            catch (IOException ioException)
            {
                string message = String.Format(CultureInfo.InvariantCulture, "Failed to set the DTR for Manomemter TH2. Exception: '{0}'", ioException.Message);
                OnMessageReceivedError(message, ErrorCodes.HAL_SERIAL_ERROR_DTR);
            }
        }

        /// <summary>
        /// Does the switch to manometer TH1.
        /// </summary>
        /// <param name="commandParameter">The command parameter.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Inspector.Hal.BluetoothHal.OnMessageReceivedError(System.String,System.Int32)")]
        private void DoSwitchToManometerTh1(string commandParameter)
        {
            try
            {
                if (commandParameter.Equals("Low"))
                {
                    m_SerialConnection.DtrEnabled = false;
                }
                else if (commandParameter.Equals("High"))
                {
                    m_SerialConnection.DtrEnabled = true;
                }
                OnMessageReceived(String.Empty);
            }
            catch (IOException ioException)
            {
                string message = String.Format(CultureInfo.InvariantCulture, "Failed to set the DTR for Manomemter TH1. Exception: '{0}'", ioException.Message);
                OnMessageReceivedError(message, ErrorCodes.HAL_SERIAL_ERROR_DTR);
            }
        }

        /// <summary>
        /// Does the send device command.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="commandParameter">The command parameter.</param>
        private void DoSendDeviceCommand(DeviceCommand command, string commandParameter)
        {
            string commandString = m_SerialCommands[command];
            commandString = String.Format(CultureInfo.InvariantCulture, commandString, commandParameter);
            m_SerialConnection.SendCommand(commandString);
            m_CommandTimeoutTimer.Change(m_ManometerCommandTimeout, Timeout.Infinite);
        }

        /// <summary>
        /// Does the send command.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="commandParameter">The command parameter.</param>
        private void DoSendManometerCommand(DeviceCommand command, string commandParameter)
        {
            try
            {
                DoSendCommand(command, commandParameter);
                System.Diagnostics.Debug.WriteLine("HAL: Starting Command Timer");
                m_CommandTimeoutTimer.Change(m_ManometerCommandTimeout, Timeout.Infinite);
            }
            catch (ChecksumException)
            {
                /* Errors are handled by DoSendCommand */
                System.Diagnostics.Debug.WriteLine("HAL: Checksum error while Starting Command Timer.");
            }
        }

        /// <summary>
        /// Does the send flush manometer command.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="commandParameter">The command parameter.</param>
        private void DoSendFlushManometerCommand(DeviceCommand command, string commandParameter)
        {
            try
            {
                DoSendCommand(command, commandParameter);
                System.Diagnostics.Debug.WriteLine("HAL: Starting Flush Manometer Cache Timer.");
                m_FlushManometerCacheTimeoutTimer.Change(m_FlushManometerCacheTimeout, Timeout.Infinite);
            }
            catch (ChecksumException)
            {
                /* Errors are handled by DoSendCommand */
                System.Diagnostics.Debug.WriteLine("HAL: Checksum error while Starting Flush Manometer Cache Timer.");
            }
        }

        /// <summary>
        /// Does the send command.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="commandParameter">The command parameter.</param>
        /// <exception cref="ChecksumException">Thrown when the checksum contained within <paramref name="command"/> is invalid./></exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Inspector.Hal.BluetoothHal.OnMessageReceivedError(System.String,System.Int32)")]
        private void DoSendCommand(DeviceCommand command, string commandParameter)
        {
            string commandString = m_SerialCommands[command];
            try
            {
                commandString = String.Format(CultureInfo.InvariantCulture, commandString, commandParameter);
                commandString += "\t*";
                commandString = String.Format(CultureInfo.InvariantCulture, "{0}{1}\r", commandString, ComputeChecksum(commandString));
                System.Diagnostics.Debug.WriteLine(String.Format(CultureInfo.InvariantCulture, "HAL: DoSendCommand sending '{0}'", commandString.Replace("\t", "[HT]").Replace("\r", "[CR]")));
                m_SerialConnection.SendCommand(commandString);
            }
            catch (ChecksumException checksumException)
            {
                System.Diagnostics.Debug.WriteLine("HAL: Checksum exception. Message: '{0}'", checksumException.Message);
                OnMessageReceivedError(String.Format(CultureInfo.InvariantCulture, "Failed to compute checksum: '{0}'", commandString), ErrorCodes.HAL_FAILED_CHECKSUM_COMPUTATION);
                throw;
            }
        }

        /// <summary>
        /// Validates the bluetooth dongle.
        /// </summary>
        /// <param name="allowedBluetoothDongles">The allowed bluetooth dongles.</param>
        /// <exception cref="ConnectionException">Thrown if an error occured while discovering bluetooth radios when no allowed bluetooth radio is available.</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        private void VerifyBluetoothDongle(List<BluetoothDongleInformation> allowedBluetoothDongles)
        {
            if (!m_BluetoothDiscoveryInitialized)
            {
                m_WclBluetoothDiscovery = new wclBluetoothDiscovery();
            }

            wclBluetoothRadios radios = new wcl.wclBluetoothRadios();
            int errorCode = m_WclBluetoothDiscovery.EnumRadios(radios);
            if (errorCode == 0)
            {
                if (radios.Count == 0)
                {
                    throw new ConnectionException("No bluetooth radios found.", ErrorCodes.HAL_BLUETOOTH_DONGLE_NOT_FOUND);
                }
                else
                {
                    UpdateAndValidateBluetoothDongle(allowedBluetoothDongles, radios);
                }
            }
            else
            {
                string errorMessage = String.Format(CultureInfo.InvariantCulture, "Error discovering bluetooth radios. Error message: {0}", wclErrors.wclGetErrorMessage(errorCode));
                throw new ConnectionException(errorMessage, errorCode);
            }
        }

        /// <summary>
        /// Validates the bluetooth dongle existance.
        /// </summary>
        /// <param name="allowedBluetoothDongles">The allowed bluetooth dongles.</param>
        /// <param name="radios">The radios.</param>
        /// <exception cref="ConnectionException">Thrown when no bluetooth dongle is available from the <paramref name="allowedBluetoothDongles"/> collection./></exception>
        private void UpdateAndValidateBluetoothDongle(List<BluetoothDongleInformation> allowedBluetoothDongles, wclBluetoothRadios radios)
        {
            if (allowedBluetoothDongles.Count > 0)
            { // If a BT address is in allowed BT dongles: verify the existence
                ValidateBluetoothDongle(allowedBluetoothDongles, radios);
            }
            else
            { // If no BT address is in allowed BT dongles: try to find the first correct api radio and save that one to the settings.
                UseFirstBluetoothDongle();
            }
        }

        /// <summary>
        /// Uses the first bluetooth dongle.
        /// </summary>
        /// <exception cref="ConnectionException">Thrown when no bluetooth dongle could be found.</exception>
        private void UseFirstBluetoothDongle()
        {
            clsSettings settings = new clsSettings();
            string btApi = settings.get_GetSetting(SETTING_CATEGORY, SETTING_BLUETOOTH_API).ToString();
            if (btApi.Equals(SETTING_RETURN_NO_VALUE, StringComparison.OrdinalIgnoreCase))
            {
                throw new ConnectionException("No Bluetooth Api is defined in the settings.", ErrorCodes.HAL_BLUETOOTH_API_NOT_FOUND);
            }
            else
            {
                List<string> btAddresses = RetrieveAvailableBluetoothDongles(btApi);
                if (btAddresses.Count == 0)
                {
                    throw new ConnectionException("No Bluetooth dongles that are usable are found on the PC.", ErrorCodes.HAL_BLUETOOTH_DONGLE_NOT_FOUND);
                }
                else
                {
                    settings.set_SaveSetting(SETTING_CATEGORY, SETTING_BLUETOOTH_DONGLE_ADDRESS, btAddresses[0]);
                }
            }
        }

        /// <summary>
        /// Validates the bluetooth dongle.
        /// </summary>
        /// <param name="allowedBluetoothDongles">The allowed bluetooth dongles.</param>
        /// <param name="radios">The radios.</param>
        /// <exception cref="ConnectionException">Thrown when no bluetooth dongle is available from the <paramref name="allowedBluetoothDongles"/> collection./></exception>
        private static void ValidateBluetoothDongle(List<BluetoothDongleInformation> allowedBluetoothDongles, wclBluetoothRadios radios)
        {
            bool hasFoundAllowedDongle = false;

            for (uint i = 0; i < radios.Count; i++)
            {
                string btAddress = string.Empty;
                radios[i].GetAddress(ref btAddress);
                hasFoundAllowedDongle = allowedBluetoothDongles.Exists(btAllowedAddress => btAllowedAddress.BluetoothAddress.Equals(btAddress, StringComparison.OrdinalIgnoreCase));
                if (hasFoundAllowedDongle)
                {
                    break;
                }
            }

            if (!hasFoundAllowedDongle)
            {
                throw new ConnectionException("No Bluetooth Radio connected to the PC is allowed to be used.", ErrorCodes.HAL_BLUETOOTH_DONGLE_NOT_ALLOWED);
            }
        }

        /// <summary>
        /// Sets the retry and timeout settings.
        /// </summary>
        private void SetRetryAndTimeoutSettings()
        {
            m_ManometerCommandMaxRetries = GetValuesFromSettingsOrDefault(SETTING_MANOMETER_COMMAND_RETRIES, DEFAULT_MANOMETER_COMMAND_RETRIES);
            m_MeasurementCommandMaxRetries = GetValuesFromSettingsOrDefault(SETTING_MEASUREMENT_RETRIES, DEFAULT_MEASUREMENT_RETRIES);
            m_FlushManometerCacheMaxRetries = GetValuesFromSettingsOrDefault(SETTING_FLUSH_MANOMETER_CACHE_RETRIES, DEFAULT_FLUSH_MANOMETER_CACHE_RETRIES);
            m_ConnectMaxRetries = GetValuesFromSettingsOrDefault(SETTING_CONNECT_RETRIES, DEFAULT_CONNECT_RETRIES);
            m_ManometerCommandTimeout = GetValuesFromSettingsOrDefault(SETTING_MANOMETER_COMMAND_TIMEOUT, DEFAULT_MANOMETER_COMMAND_TIMEOUT);
            m_FlushManometerCacheTimeout = GetValuesFromSettingsOrDefault(SETTING_FLUSH_MANOMETER_CACHE_TIMEOUT, DEFAULT_FLUSH_MANOMETER_CACHE_TIMEOUT);
            m_ConnectTimeout = GetValuesFromSettingsOrDefault(SETTING_CONNECT_TIMEOUT, DEFAULT_CONNECT_TIMEOUT);
            m_MeasurementTimeout = GetValuesFromSettingsOrDefault(SETTING_MEASUREMENT_TIMEOUT, DEFAULT_MEASUREMENT_TIMER_TIMEOUT);
            m_MeasurementInterval = GetValuesFromSettingsOrDefault(SETTING_MEASUREMENT_INTERVAL, DEFAULT_MEASUREMENT_INTERVAL);
            m_WakeUpInterval = GetValuesFromSettingsOrDefault(SETTING_WAKEUP_INTERVAL, DEFAULT_WAKEUP_INTERVAL);
        }

        /// <summary>
        /// Gets the value from settings.
        /// </summary>
        /// <param name="setting">The setting.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>The value of the requested setting or the default value if the setting is unspecified.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        private static int GetValuesFromSettingsOrDefault(string setting, int defaultValue)
        {
            string settingValue = String.Empty;
            int result = -1;

            try
            {
                clsSettings settings = new clsSettings();
                settingValue = settings.get_GetSetting(SETTING_CATEGORY, setting).ToString();
                if (settingValue.Equals(SETTING_RETURN_NO_VALUE, StringComparison.OrdinalIgnoreCase))
                {
                    result = defaultValue;
                }
                else
                {
                    result = int.Parse(settingValue, CultureInfo.InvariantCulture);
                }
            }
            catch
            {
                result = defaultValue;
            }

            return result;
        }

        /// <summary>
        /// Called when [connect timeout].
        /// </summary>
        /// <param name="state">The state.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Inspector.Hal.BluetoothHal.HandleConnectFailed(System.String,System.Int32)")]
        private void OnConnectTimeout(object state)
        {
            HandleConnectFailed("Timeout", ErrorCodes.HAL_COMMAND_TIMEOUT_RECEIVED);
        }

        /// <summary>
        /// Handles the connect failed.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="errorCode">The error code.</param>
        private void HandleConnectFailed(string message, int errorCode = -1)
        {
            System.Diagnostics.Debug.WriteLine("HAL: Connect Timeout");

            m_ConnectTimer.Change(Timeout.Infinite, Timeout.Infinite);

            if (m_Retry < m_ConnectMaxRetries)
            {
                m_Retry++;
                Connect(m_ConnectionParameters, m_AllowedBluetoothDongles);
            }
            else
            {
                m_Retry = 0;
                OnConnectFailed(message, errorCode);
            }

        }

        /// <summary>
        /// Called when [command timeout].
        /// </summary>
        /// <param name="state">The state.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Inspector.Hal.BluetoothHal.OnMessageReceivedError(System.String,System.Int32)")]
        private void OnCommandTimeout(object state)
        {
            System.Diagnostics.Debug.WriteLine("HAL: OnCommandTimeout starting, requesting m_LockBusy");
            lock (m_LockBusy)
            {
                System.Diagnostics.Debug.WriteLine("HAL: OnCommandTimeout: wakeupstate is '{0}'", new object[] { m_WakeUpState.ToString() });
                if (m_CurrentCommand != DeviceCommand.Wakeup && IsBusy)
                {
                    System.Diagnostics.Debug.WriteLine("HAL: Command Timeout");
                    m_CommandTimeoutTimer.Change(Timeout.Infinite, Timeout.Infinite);
                    // Do a retry if needed, otherwise quit.
                    if (m_Retry < m_ManometerCommandMaxRetries)
                    {
                        m_Retry++;
                        SendCommand(m_CurrentCommand, m_CurrentCommandParameter);
                    }
                    else
                    {
                        m_Retry = 0;
                        OnMessageReceivedError("Timeout", ErrorCodes.HAL_COMMAND_TIMEOUT_RECEIVED);
                    }
                }
                else
                {
                    SendQueuedCommandOrDoWakeUp();
                }
            }
            System.Diagnostics.Debug.WriteLine("HAL: OnCommandTimeout ended, releasing m_LockBusy");
        }

        /// <summary>
        /// Sends the queued command or does the wake up if nothing is queued.
        /// Also continues to wake up if it not completely finished.
        /// </summary>
        private void SendQueuedCommandOrDoWakeUp()
        {
            System.Diagnostics.Debug.WriteLine("HAL: SendQueuedCommandOrDoWakeUp requesting lock");
            lock (m_LockBusy)
            {
                System.Diagnostics.Debug.WriteLine("HAL: SendQueuedCommandOrDoWakeUp wakeupstate '{0}'", new object[] { m_WakeUpState.ToString() });
                if (m_WakeUpState == WakeUpState.IDLE)
                {
                    if (m_QueuedCommand.IsCommandAssigned)
                    {
                        System.Diagnostics.Debug.WriteLine("HAL: Sending queued command '{0}' '{1}'", new object[] { m_QueuedCommand.Command, m_QueuedCommand.Parameter });
                        if (m_QueuedCommand.Command == DeviceCommand.MeasureContinuously)
                        {
                            DoStartContinuousMeasurement(m_QueuedCommand.Parameter);
                        }
                        else
                        {
                            SendCommand(m_QueuedCommand.Command, m_QueuedCommand.Parameter);
                        }

                        m_QueuedCommand = new CommandData();
                    }
                    else
                    {
                        // repeat wake up
                        System.Diagnostics.Debug.WriteLine("HAL: Repeating wake up with (interval '{0}' ms", new object[] { m_WakeUpInterval });
                        m_WakeUpTimer.Change(m_WakeUpInterval, Timeout.Infinite);
                    }
                }
                else if (m_WakeUpState == WakeUpState.MANOMETER1 || m_WakeUpState == WakeUpState.MANOMETER2)
                {
                    System.Diagnostics.Debug.WriteLine("HAL: Sending wake up signal to '{0}'", new object[] { m_WakeUpState.ToString() });
                    m_SerialConnection.DtrEnabled = !m_SerialConnection.DtrEnabled;
                    SendCommand(DeviceCommand.Wakeup);
                    switch (m_WakeUpState)
                    {
                        case WakeUpState.MANOMETER1:
                            m_WakeUpState = WakeUpState.MANOMETER2;
                            break;
                        case WakeUpState.MANOMETER2:
                            m_WakeUpState = WakeUpState.IDLE;
                            break;
                    }
                }
            }
            System.Diagnostics.Debug.WriteLine("HAL: SendQueuedCommandOrDoWakeUp releasing lock");
        }

        /// <summary>
        /// Called when [flush manometer cache timeout].
        /// </summary>
        /// <param name="state">The state.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Inspector.Hal.BluetoothHal.OnMessageReceivedError(System.String,System.Int32)"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Inspector.Hal.BluetoothHal.HandleConnectFailed(System.String,System.Int32)")]
        private void OnFlushManometerCacheTimeout(object state)
        {
            System.Diagnostics.Debug.WriteLine("HAL: Flush Manometer Cache Timeout");
            m_FlushManometerCacheTimeoutTimer.Change(Timeout.Infinite, Timeout.Infinite);
            // Do a retry if needed, otherwise quit.
            if (m_Retry < m_FlushManometerCacheMaxRetries)
            {
                m_Retry++;
                try
                {
                    SendCommand(m_CurrentCommand, m_CurrentCommandParameter);
                }
                catch (ChecksumException)
                {
                    m_Retry = 0;
                    OnMessageReceivedError("Invalid checksum format", ErrorCodes.HAL_MESSAGE_RECEIVED_INCORRECT_FORMAT);
                }
            }
            else
            {
                m_Retry = 0;
                OnMessageReceivedError("Timeout", ErrorCodes.HAL_COMMAND_TIMEOUT_RECEIVED);
            }
        }

        /// <summary>
        /// Called when [measurement timer tick].
        /// </summary>
        /// <param name="state">The state.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Inspector.Hal.BluetoothHal.OnMessageReceivedError(System.String,System.Int32)")]
        private void OnMeasurementTimerTick(object state)
        {
            bool checksumError = false;
            bool measurementParseError = false;
            bool invalidDataError = false;
            int checksum = 0;
            int computedChecksum = 0;
            string receivedMeasurement = String.Empty;
            double measurementValue = double.NaN;
            string parsedMeasurementValue = String.Empty;
            string measurementsData = GetMeasurementData();
            if (measurementsData.Length > 0)
            {
                string[] measurements = measurementsData.Split('\r');
                IList<double> parsedMeasurements = new List<double>();
                foreach (string measurement in measurements)
                {
                    if (!String.IsNullOrEmpty(measurement))
                    {
                        System.Diagnostics.Debug.WriteLine("Measurement: " + measurement);
                        if (m_UseChecksum)
                        {
                            try
                            {
                                parsedMeasurementValue = ParseManometerData(measurement + "\r", out checksum, out computedChecksum);
                                checksumError = (checksum != computedChecksum);
                            }
                            catch (ChecksumException)
                            {
                                checksumError = true;
                            }
                            catch (InvalidDataException)
                            {
                                invalidDataError = true;
                            }

                            try
                            {
                                measurementValue = double.Parse(parsedMeasurementValue, CultureInfo.InvariantCulture);
                            }
                            catch
                            {
                                measurementParseError = true;
                            }

                            if (checksumError || measurementParseError || invalidDataError)
                            {
                                receivedMeasurement = measurement;
                                break;
                            }
                        }
                        else
                        {
                            try
                            {
                                parsedMeasurementValue = ParseManometerData(measurement + "\r", out checksum, out computedChecksum);
                                measurementValue = double.Parse(parsedMeasurementValue, CultureInfo.InvariantCulture);
                            }
                            catch (InvalidDataException)
                            {
                                invalidDataError = true;
                                break;
                            }
                            catch
                            {
                                measurementParseError = true;
                                break;
                            }
                        }

                        parsedMeasurements.Add(measurementValue);
                    }
                }

                if (checksumError || measurementParseError || invalidDataError)
                {
                    if (!m_IsMeasuring && m_RetryMeasurement < m_MeasurementCommandMaxRetries)
                    {
                        RetryContinuousMeasurement();
                    }
                    else
                    {
                        StopContinuousMeasurementOnError();
                        if (checksumError)
                        {
                            log.Info(string.Format("Exception in checksum:\n\t ReceivedMeasurement: {0}\n\t MeasurementValue: {1}\n\t checksum: {2}\n\t computedchecksum: {3}", receivedMeasurement, measurementValue, checksum, computedChecksum));
                            OnMessageReceivedError(String.Format(CultureInfo.InvariantCulture, "Checksum for received message '{0}' incorrect (Expected: '{1}', Was: '{2}'", measurementValue, checksum, computedChecksum), ErrorCodes.HAL_MESSAGE_RECEIVED_INCORRECT_CHECKSUM);
                        }
                        else
                        {
                            OnMessageReceivedError(String.Format(CultureInfo.InvariantCulture, "Invalid measurement value '{0}'. Value is not a valid double type.", parsedMeasurementValue), ErrorCodes.HAL_MESSAGE_RECEIVED_INCORRECT_FORMAT);
                        }
                    }
                }
                else
                {
                    m_IsMeasuring = true;
                    m_RetryMeasurement = 0;
                    OnMeasurementsReceived(parsedMeasurements);
                }
            }

            if (!checksumError && !measurementParseError && !invalidDataError)
            {
                m_MeasurementTimer.Change(m_MeasurementInterval, Timeout.Infinite);
            }
        }

        private void RetryContinuousMeasurement()
        {
            System.Diagnostics.Debug.WriteLine("HAL: Retrying Continuous Measurement");
            SendCommand(m_CurrentCommand, m_CurrentCommandParameter);
            m_MeasurementTimer.Change(m_MeasurementInterval, Timeout.Infinite);
            m_RetryMeasurement++;

        }

        /// <summary>
        /// Gets the measurement data.
        /// </summary>
        /// <returns></returns>
        private string GetMeasurementData()
        {
            string measurementsString = String.Empty;
            lock (m_LockMeasurementData)
            {
                if (m_MeasurementsReceived.Contains('\r'))
                {
                    measurementsString = m_MeasurementsReceived.Substring(0, m_MeasurementsReceived.LastIndexOf('\r') + 1);
                    if (measurementsString.Length == m_MeasurementsReceived.Length)
                    {
                        m_MeasurementsReceived = String.Empty;
                    }
                    else
                    {
                        m_MeasurementsReceived = m_MeasurementsReceived.Substring(m_MeasurementsReceived.LastIndexOf('\r') + 1);
                    }
                }
            }
            return measurementsString;
        }

        /// <summary>
        /// Called when [measurement timer timeout].
        /// </summary>
        /// <param name="state">The state.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Inspector.Hal.BluetoothHal.OnMessageReceivedError(System.String,System.Int32)")]
        private void OnMeasurementTimerTimeout(object state)
        {
            lock (m_LockMeasurementData)
            {
                m_MeasurementsReceived = String.Empty;
            }

            lock (m_MeasuringLock)
            {
                //Note that this assumes that a timeout on this timer can only occur when we are in the state Continuousmeasure
                //if this is not the case, and we have retried more than allowed and m_Ismeasuring is false, a stateMeachineException will be thrown
                if (!m_IsMeasuring && m_RetryMeasurement < m_MeasurementCommandMaxRetries)
                {
                    System.Diagnostics.Debug.WriteLine("measurement timeout, retrying measurement.");
                    RetryContinuousMeasurement();
                    m_MeasurementTimeoutTimer.Change(m_MeasurementTimeout, Timeout.Infinite);
                }
                else
                {
                    StopContinuousMeasurementOnError();
                    OnMessageReceivedError("Timeout", ErrorCodes.HAL_MEASUREMENT_TIMEOUT_RECEIVED);
                }
            }
        }

        /// <summary>
        /// Connects to serial port.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Inspector.Hal.BluetoothHal.HandleConnectFailed(System.String,System.Int32)"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        private void ConnectToSerialPort()
        {
            try
            {
                m_SerialConnection = new SerialConnection(m_SerialPortNumber);
                m_SerialConnection.Connect();
                m_SerialConnection.SerialDataEvent += new EventHandler<SerialDataEventArgs>(SerialConnection_SerialDataEvent);
                m_SerialConnection.SerialDataErrorEvent += new EventHandler<SerialDataErrorEventArgs>(SerialConnection_SerialDataErrorEvent);
                m_Retry = 0;
                m_ConnectTimer.Change(Timeout.Infinite, Timeout.Infinite);
                OnConnected();
            }
            catch (ConnectionException connectionException)
            {
                HandleConnectFailed(connectionException.Message, connectionException.ErrorCode);
            }
            catch (Exception exception)
            {
                string message = String.Format(CultureInfo.InvariantCulture, "Creation of the serial connection failed. Exception: '{0}'.", exception.Message);
                HandleConnectFailed(message, ErrorCodes.HAL_SERIAL_ERROR_PORT_CREATION);
            }
        }

        /// <summary>
        /// Creates the virtual COM port.
        /// </summary>
        /// <param name="bluetoothApi">The bluetooth API.</param>
        /// <param name="destinationAddress">The destination address.</param>
        /// <exception cref="ConnectionException">Thrown if Creating the virtual comport fails</exception>
        private void CreateVirtualComPort(string bluetoothApi, string destinationAddress)
        {
            wclBluetoothRadio radio;

            try
            {
                radio = GetRadioForApi(bluetoothApi);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                string message = String.Format(CultureInfo.CurrentCulture, "Could not create Bluetooth Radio for API: '{0}'. API not supported", bluetoothApi);
                throw new ConnectionException(message, ex, ErrorCodes.HAL_BLUETOOTH_API_NOT_SUPPORTED);
            }

            m_WclVirtualComPort.Address = destinationAddress;
            m_WclVirtualComPort.Radio = radio;
            m_WclVirtualComPort.Service = wclUUIDs.SerialPortServiceClass_UUID;
            int errorCode = m_WclVirtualComPort.Open();
            if (errorCode != 0)
            {
                string message = String.Format(CultureInfo.CurrentCulture, "Could not create a Bluetooth Connection. Error code: '{0}'. Error Message: '{1}'",
                                                                           errorCode.ToString(CultureInfo.CurrentCulture), wclErrors.wclGetErrorMessage(errorCode));
                throw new ConnectionException(message, errorCode);
            }
        }

        /// <summary>
        /// Gets the radio for API.
        /// </summary>
        /// <param name="api">The API.</param>
        /// <returns></returns>
        /// <remarks>Supported API's are:
        /// - baBlueSoleil
        /// - baMicrosoft
        /// - baToshiba
        /// - baWidComm</remarks>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if an unsupported API is given.</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        private static wclBluetoothRadio GetRadioForApi(string api)
        {
            wclBluetoothRadio radio = new wclBluetoothRadio();

            if (api == wclBluetoothAPI.baBlueSoleil.ToString())
            {
                radio.API = wclBluetoothAPI.baBlueSoleil;
            }
            else if (api == wclBluetoothAPI.baMicrosoft.ToString())
            {
                radio.API = wclBluetoothAPI.baMicrosoft;
            }
            else if (api == wclBluetoothAPI.baToshiba.ToString())
            {
                radio.API = wclBluetoothAPI.baToshiba;
            }
            else if (api == wclBluetoothAPI.baWidComm.ToString())
            {
                radio.API = wclBluetoothAPI.baWidComm;
            }
            else
            {
                throw new ArgumentOutOfRangeException(String.Format(CultureInfo.CurrentCulture, "Bluetooth API '{0}' not supported by the HAL.", api));
            }

            return radio;
        }

        /// <summary>
        /// Checks the connection properties.
        /// </summary>
        /// <param name="connectionProperties">The connection properties.</param>
        /// <exception cref="ConnectionException">Thrown if a required key cannot be found in the connectionProperties</exception>
        private static void ValidateConnectionProperties(Dictionary<string, string> connectionProperties)
        {
            ValidateConnectionProperty(connectionProperties, CONNECTIONPROPERTY_BLUETOOTHAPI);
            ValidateConnectionProperty(connectionProperties, CONNECTIONPROPERTY_DESTINATIONADDRESS);
        }

        /// <summary>
        /// Validates the connection property.
        /// </summary>
        /// <param name="connectionProperties">The connection properties.</param>
        /// <param name="property">The property.</param>
        /// <exception cref="ConnectionException">Thrown when a required key is is not found in the connection properties.</exception>
        private static void ValidateConnectionProperty(Dictionary<string, string> connectionProperties, string property)
        {
            if (!connectionProperties.ContainsKey(property))
            {
                throw new ConnectionException(String.Format(CultureInfo.InvariantCulture, "Required key '{0}' not found in the connection properties", property), ErrorCodes.HAL_CONNECTION_PROPERTY_NOT_FOUND);
            }
        }

        /// <summary>
        /// Creates the command list.
        /// </summary>
        private void CreateCommandList()
        {
            // When the command is executed, replace <var> ({0}) with the actual value
            m_SerialCommands.Add(DeviceCommand.EnterRemoteLocalCommandMode, "!"); // repeat three times
            m_SerialCommands.Add(DeviceCommand.ExitRemoteLocalCommandMode, "ATO\r");
            m_SerialCommands.Add(DeviceCommand.SwitchInitializationLedOn, "ATS{0}=1\r");
            m_SerialCommands.Add(DeviceCommand.SwitchInitializationLedOff, "ATS{0}=0\r");
            m_SerialCommands.Add(DeviceCommand.CheckIdentification, "SYST:IDEN?");
            m_SerialCommands.Add(DeviceCommand.CheckBatteryStatus, "SYST:BATT?");
            m_SerialCommands.Add(DeviceCommand.InitiateSelfTest, "*TST?");
            m_SerialCommands.Add(DeviceCommand.CheckRange, "SYST:RANG?");
            m_SerialCommands.Add(DeviceCommand.SetPressureUnit, "UNIT:PRES {0}");
            m_SerialCommands.Add(DeviceCommand.CheckPressureUnit, "UNIT:PRES?");
            m_SerialCommands.Add(DeviceCommand.CheckSCPIInterface, "SYST:ERR:Next?");
            m_SerialCommands.Add(DeviceCommand.FlushManometerCache, "SYST:IDEN?");
            m_SerialCommands.Add(DeviceCommand.MeasureContinuously, "MEAS:PRES {0}");
            m_SerialCommands.Add(DeviceCommand.FlushBluetoothCache, "AT\r\n");
            m_SerialCommands.Add(DeviceCommand.CheckManometerPresent, "SYST:IDEN?");
            m_SerialCommands.Add(DeviceCommand.Wakeup, "SYST:IDEN?");
            m_SerialCommands.Add(DeviceCommand.IRAlwaysOn, "ENG:IRDA:INIT ON");
        }
        #endregion Private functions

        #region Event Handlers
        /// <summary>
        /// Attaches the handlers.
        /// </summary>
        private void AttachEventHandlers()
        {
            m_WclVirtualComPort.AfterOpen += new wclVirtualPortCreatedEventHandler(wclVirtualComPort_AfterOpen);
            m_WclVirtualComPort.BeforeClose += new EventHandler(wclVirtualComPort_BeforeClose);
        }

        /// <summary>
        /// Detaches the handlers.
        /// </summary>
        private void DetachEventHandlers()
        {
            m_WclVirtualComPort.BeforeClose -= new EventHandler(wclVirtualComPort_BeforeClose);
            m_WclVirtualComPort.AfterOpen -= new wclVirtualPortCreatedEventHandler(wclVirtualComPort_AfterOpen);
        }
        #endregion Event Handlers
    }
}
