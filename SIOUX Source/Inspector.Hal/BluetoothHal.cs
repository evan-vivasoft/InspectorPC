/*
//===============================================================================
// Copyright Kamstrup
// All rights reserved.
//===============================================================================
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using Inspector.Hal.Interfaces;
using Inspector.Hal.Interfaces.Events;
using Inspector.Hal.Interfaces.Exceptions;
using Inspector.Infra;
using Inspector.Infra.Utils;
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
    public sealed class BluetoothHal : IHal
    {
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
        private const int DEFAULT_MEASUREMENT_RETRIES = 3;

        private const int FINALIZE_MEASUREMENT_WAIT_TIME = 1500; // 680ms is maximum required for device * 2 (rounded to 1500)

        private const string PlexorBluetoothIrDADeviceName = "PLEXOR BT IrDA";
        private const string PlexorBluetoothWISDeviceName = "PLEXOR BT WIS";
        #endregion Constants

        #region Class Members
        private readonly wclAPI m_WclApi;

        private readonly wclBluetoothDiscovery m_WclBluetoothDiscovery;

        private SerialConnection m_SerialConnection;

        private readonly Dictionary<DeviceCommand, string> m_SerialCommands = new Dictionary<DeviceCommand, string>();
        private DeviceType m_DeviceType = DeviceType.Unknown;

        private DeviceCommand m_CurrentCommand = DeviceCommand.None;
        private string m_CurrentCommandParameter = string.Empty;
        private Dictionary<string, string> m_ConnectionParameters = new Dictionary<string, string>();
        private List<BluetoothDongleInformation> m_AllowedBluetoothDongles = new List<BluetoothDongleInformation>();

        private bool m_Disposed;

        private string m_SerialDataReceived = string.Empty;
        private string m_MeasurementsReceived = string.Empty;

        private int m_ManometerCommandMaxRetries = 3;
        private int m_FlushManometerCacheMaxRetries = 3;
        private int m_ConnectMaxRetries = 3;
        private int m_ManometerCommandTimeout = 3000;
        private int m_FlushManometerCacheTimeout = 3000;
        private int m_ConnectTimeout = 15000;
        private int m_MeasurementTimeout = 3000;
        private int m_MeasurementInterval = 1000;
        private int m_MeasurementCommandMaxRetries = 3;

        private readonly Timer m_CommandTimeoutTimer;
        private readonly Timer m_ConnectTimer;
        private readonly Timer m_FlushManometerCacheTimeoutTimer;
        private readonly Timer m_MeasurementTimer;
        private readonly Timer m_MeasurementTimeoutTimer;
        private readonly Timer m_StoppingMeasurementTimer;

        private bool m_StoppedMeasurementTimedOut;
        private bool m_ContinuousMeasurementStartedEventRaised;
        private bool m_UseChecksum = true;
        private bool m_StoppingMeasurement;

        private readonly object m_LockMeasurementData = new object();
        
        // Do not lock m_BusyLock after locking m_MeasuringLock, as this may cause a deadlock.
        // The correct order is to first lock m_BusyLock, and then lock m_MeasuringLock
        private readonly object m_BusyLock = new object();

        private bool m_IsMeasuring = false;

        // Do not lock m_BusyLock after locking m_MeasuringLock, as this may cause a deadlock.
        // The correct order is to first lock m_BusyLock, and then m_MeasuringLock
        private readonly object m_MeasuringLock = new object();

        private int m_Retry = 0;
        private int m_RetryMeasurement = 0;
        private bool m_IsBusy = false;

        private static readonly ILog Log = LogManager.GetLogger(typeof(BluetoothHal));
        private static readonly ILog CommunicationLogger = LogManager.GetLogger("CommunicationLogger");

        #endregion Class Members

        #region Properties
        /// <summary>
        /// Gets a value indicating whether [bluetooth active].
        /// For unit testing purposes only!
        /// </summary>
        /// <value>
        ///   <c>true</c> if [bluetooth active]; otherwise, <c>false</c>.
        /// </value>
        public bool BluetoothActive => SerialPortOpen;

        /// <summary>
        /// Gets a value indicating whether [serial port open].
        /// For unit testing purposes only!
        /// </summary>
        /// <value>
        ///   <c>true</c> if [serial port open]; otherwise, <c>false</c>.
        /// </value>
        public bool SerialPortOpen => m_SerialConnection != null && m_SerialConnection.SerialPortOpen;

        #endregion Properties

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="BluetoothHal"/> class.
        /// </summary>
        public BluetoothHal()
        {
            SetRetryAndTimeoutSettings();

            SetUseChecksumSetting();

            m_CommandTimeoutTimer = new Timer(OnCommandTimeout, null, Timeout.Infinite, Timeout.Infinite);
            m_ConnectTimer = new Timer(OnConnectTimeout, null, Timeout.Infinite, Timeout.Infinite);
            m_FlushManometerCacheTimeoutTimer = new Timer(OnFlushManometerCacheTimeout, null, Timeout.Infinite, Timeout.Infinite);
            m_MeasurementTimer = new Timer(OnMeasurementTimerTick, null, Timeout.Infinite, Timeout.Infinite);
            m_MeasurementTimeoutTimer = new Timer(OnMeasurementTimerTimeout, null, Timeout.Infinite, Timeout.Infinite);
            m_StoppingMeasurementTimer = new Timer(OnStoppingMeasurementTimerTimeout, null, Timeout.Infinite, Timeout.Infinite);

            m_WclApi = new wclAPI();
            m_WclApi.Load();

            m_WclBluetoothDiscovery = new wclBluetoothDiscovery();
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
        private void Dispose(bool disposing)
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
                    m_StoppingMeasurementTimer.Dispose();

                    try
                    {
                        Disconnect();
                    }
                    catch
                    {
                        // ignored
                    }

                    try
                    {
                        if (m_SerialConnection != null)
                        {
                            m_SerialConnection.Dispose();
                        }
                    }
                    catch
                    {
                        // ignored
                    }

                    try
                    {
                        m_WclBluetoothDiscovery.Dispose();
                    }
                    catch
                    {
                        // ignored
                    }

                    try
                    {
                        var wclErrorCode = m_WclApi.Unload();

                        if (wclErrorCode != wclErrors.WCL_E_SUCCESS)
                        {
                            CommunicationLogger.DebugFormat("Failed to unload wcl api. ErrorCode: '{0}'", wclErrorCode);
                        }

                        m_WclApi.Dispose();
                    }
                    catch
                    {
                        // ignored
                    }
                }
            }

            m_Disposed = true;
        }

        #endregion IDisposable

        #region Serial Port Events
        /// <summary>
        /// Handles the SerialDataErrorEvent event of the SerialConnection control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Inspector.Hal.SerialDataErrorEventArgs"/> instance containing the event data.</param>
        private void SerialConnection_SerialDataErrorEvent(object sender, SerialDataErrorEventArgs e)
        {
            CommunicationLogger.Info("SerialData error received: " + e.Message);
            m_SerialDataReceived = string.Empty;
            CommunicationLogger.Debug("HAL: Stopping Command Timer");
            m_CommandTimeoutTimer.Change(Timeout.Infinite, Timeout.Infinite);
            CommunicationLogger.Debug("HAL: Stopping Flush Manometer Cache Timer");
            m_FlushManometerCacheTimeoutTimer.Change(Timeout.Infinite, Timeout.Infinite);
            CommunicationLogger.Debug("HAL: Stopping Measurement Timer");
            m_MeasurementTimer.Change(Timeout.Infinite, Timeout.Infinite);
            CommunicationLogger.Debug("HAL: Stopping Measurement Timeout Timer");
            m_MeasurementTimeoutTimer.Change(Timeout.Infinite, Timeout.Infinite);
            CommunicationLogger.Debug("HAL: Stopping Wake Up Timer");
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
            CommunicationLogger.Info("Serial data received288: " + e.SerialData);
            
            if (m_CurrentCommand == DeviceCommand.None) return;
            
            RestartCommandTimer();

            m_SerialDataReceived += e.SerialData;
            switch (m_CurrentCommand)
            {
                case DeviceCommand.EnterRemoteLocalCommandMode:
                case DeviceCommand.ExitRemoteLocalCommandMode:
                case DeviceCommand.FlushBluetoothCache:
                    switch (m_DeviceType)
                    {
                        case DeviceType.PlexorBluetoothIrDA:
                            // Regex validates following inputformats that can be received from the device:
                            // "\r\ndata\r\n" --> Reaction on Bluetooth serial mode
                            ParseDeviceData("^.*\r\n(?<Data>.+?)\r\n$", "\n");
                            break;
                        case DeviceType.PlexorBluetoothWIS:
                            // Regex validates following inputformats that can be received from the device:
                            // "data\r\n" --> Reaction on Bluetooth wireless mode
                            ParseDeviceData("^(?<Data>.+?)\r\n$", "\n");
                            break;
                        case DeviceType.Unknown:
                        default:
                            break;
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
                case DeviceCommand.CheckSystemSoftwareVersion:
                case DeviceCommand.CheckCalibrationDate:
                case DeviceCommand.EnableIOStatus:
                case DeviceCommand.DisableIOStatus:
                case DeviceCommand.CheckIo3Status:
                case DeviceCommand.ActivatePortSensor:
                case DeviceCommand.StopSensorRun:
                case DeviceCommand.ReadSensorId:
                case DeviceCommand.ActivatePortIrDa:
                    ParseManometerData();
                    break;
                case DeviceCommand.MeasureSingleValue:
                    ParseSingleMeasureValue();
                    break;
                case DeviceCommand.SwitchToManometerTH1:
                case DeviceCommand.SwitchToManometerTH2:
                    switch (m_DeviceType)
                    {
                        case DeviceType.PlexorBluetoothIrDA:
                            // Regex validates following inputformats that can be received from the device:
                            // "\r\ndata\r\n" --> Reaction on Bluetooth serial mode
                            ParseDeviceData("^.*\r\n(?<Data>.+?)\r\n$", "\n");
                            break;
                        case DeviceType.PlexorBluetoothWIS:
                            // Regex validates following inputformats that can be received from the device:
                            // "data\t\r" --> Reaction on Bluetooth wireless mode
                            ParseDeviceData("^(?<Data>.+?)\t\r$", "\r");
                            break;
                        case DeviceType.Unknown:
                        default:
                            break;
                    }
                    break;
                case DeviceCommand.Connect:
                case DeviceCommand.Disconnect:
                case DeviceCommand.MeasureContinuously:
                case DeviceCommand.None:
                default:
                    break;
            }
        }

        /// <summary>
        /// Handles the SerialDataEvent event of the SerialConnection control for measurements.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Inspector.Hal.SerialDataEventArgs"/> instance containing the event data.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1307:SpecifyStringComparison", MessageId = "System.String.EndsWith(System.String)")]
        private void SerialConnection_SerialDataEvent_Measurement(object sender, SerialDataEventArgs e)
        {
            lock (m_MeasuringLock)
            {
                CommunicationLogger.Info("Serial data received: " + e.SerialData);

                if (m_CurrentCommand == DeviceCommand.None || m_StoppingMeasurement) return;

                m_MeasurementTimeoutTimer.Change(m_MeasurementTimeout, Timeout.Infinite);
                lock (m_LockMeasurementData)
                {
                    m_MeasurementsReceived += e.SerialData;

                    if (m_ContinuousMeasurementStartedEventRaised) return;

                    OnContinuousMeasurementStarted();
                    m_ContinuousMeasurementStartedEventRaised = true;
                }
            }
        }

        /// <summary>
        /// Handles the StoppingMeasurement event of the SerialConnection_SerialDataEvent control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Inspector.Hal.SerialDataEventArgs"/> instance containing the event data.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "log4net.ILog.DebugFormat(System.String,System.Object[])"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1307:SpecifyStringComparison", MessageId = "System.String.EndsWith(System.String)")]
        private void SerialConnection_SerialDataEvent_StoppingMeasurement(object sender, SerialDataEventArgs e)
        {
            CommunicationLogger.Info("Serial data received: " + e.SerialData);

            if (m_CurrentCommand == DeviceCommand.None) return;

            StopCommandTimer();
            m_SerialDataReceived += e.SerialData;

            if (m_StoppedMeasurementTimedOut)
            {
                CommunicationLogger.Debug("HAL: SerialConnection_SerialDataEvent_StoppingMeasurement m_StoppedMeasurementTimedOut == true");
                // Any answer ending on \r is okay :)

                if (!m_SerialDataReceived.EndsWith("\r") && !m_SerialDataReceived.EndsWith("\n")) return;

                CommunicationLogger.Debug("HAL: SerialConnection_SerialDataEvent_StoppingMeasurement if (m_SerialDataReceived.EndsWith(\"\\r\")) == true");
                m_StoppingMeasurementTimer.Change(Timeout.Infinite, Timeout.Infinite);
                DetachAllEventhandlers();
                m_SerialDataReceived = string.Empty;
                m_SerialConnection.SerialDataEvent += SerialConnection_SerialDataEvent;
                OnContinuousMeasurementStopped();
            }
            else
            {
                CommunicationLogger.Debug("HAL: SerialConnection_SerialDataEvent_StoppingMeasurement m_StoppedMeasurementTimedOut == false");
                CommunicationLogger.DebugFormat("HAL: OK DATA RECEIVED: '{0}'", new object[]
                {
                    m_SerialDataReceived.Replace("\t", "[HT]").Replace("\r", "[CR]").Replace("\n", "[LF]")
                });

                if (!m_SerialDataReceived.EndsWith("ok\t*13\r", StringComparison.OrdinalIgnoreCase)) return;

                CommunicationLogger.Debug("HAL: SerialConnection_SerialDataEvent_StoppingMeasurement if (m_SerialDataReceived.EndsWith(@\"ok\t*13\\r\", StringComparison.OrdinalIgnoreCase)) == true");
                m_StoppingMeasurementTimer.Change(Timeout.Infinite, Timeout.Infinite);
                DetachAllEventhandlers();
                m_SerialDataReceived = String.Empty;
                m_SerialConnection.SerialDataEvent += SerialConnection_SerialDataEvent;
                OnContinuousMeasurementStopped();
            }
        }

        /// <summary>
        /// Stops the command timer.
        /// </summary>
        private void StopCommandTimer()
        {
            if (m_CurrentCommand == DeviceCommand.FlushManometerCache)
            {
                CommunicationLogger.Debug("HAL: Stopping Flush Manometer Cache Timer");
                m_FlushManometerCacheTimeoutTimer.Change(Timeout.Infinite, Timeout.Infinite);
            }
            else
            {
                CommunicationLogger.Debug("HAL: Stopping Command Timer");
                m_CommandTimeoutTimer.Change(Timeout.Infinite, Timeout.Infinite);
            }
        }

        /// <summary>
        /// Restarts the command timer.
        /// </summary>
        private void RestartCommandTimer()
        {
            if (m_CurrentCommand == DeviceCommand.FlushManometerCache)
            {
                CommunicationLogger.Debug("HAL: Restarting Flush Manometer Cache Timer");
                m_FlushManometerCacheTimeoutTimer.Change(m_FlushManometerCacheTimeout, Timeout.Infinite);
            }
            else
            {
                CommunicationLogger.Debug("HAL: Restarting Command Timer");
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
            var availableBluetoothDongles = new List<string>();

            var radios = new wclBluetoothRadios();

            var errorCode = m_WclBluetoothDiscovery.EnumRadios(radios);
            if (errorCode == wclErrors.WCL_E_SUCCESS)
            {
                if (radios.Count <= 0) return availableBluetoothDongles;

                for (uint i = 0; i < radios.Count; i++)
                {
                    var btApi = radios[i].API.ToString();

                    if (btApi != bluetoothApi) continue;

                    var btAddress = string.Empty;
                    var result = radios[i].GetAddress(ref btAddress);
                    availableBluetoothDongles.Add(btAddress);
                }
            }
            else
            {
                throw new ConnectionException(wclErrors.wclGetErrorMessage(errorCode), errorCode);
            }

            return availableBluetoothDongles;
        }

        private IEnumerable<wclBluetoothRadio> RetrieveAvailableBluetoothRadios(string bluetoothApi)
        {
            var availableBluetoothRadios = new List<wclBluetoothRadio>();

            using (var radios = new wclBluetoothRadios())
            {
                var errorCode = m_WclBluetoothDiscovery.EnumRadios(radios);
                if (errorCode == wclErrors.WCL_E_SUCCESS)
                {
                    if (radios.Count <= 0) return availableBluetoothRadios;

                    for (uint i = 0; i < radios.Count; i++)
                    {
                        var btRadio = radios[i];

                        if (btRadio.API.ToString() != bluetoothApi) continue;

                        availableBluetoothRadios.Add(btRadio);
                    }
                }
                else
                {
                    throw new ConnectionException(wclErrors.wclGetErrorMessage(errorCode), errorCode);
                }
            }
            return availableBluetoothRadios;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the HAL is signaled that a command is to be expected.
        /// </summary>
        /// <value><c>true</c> if a command is expected; otherwise, <c>false</c>.</value>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "log4net.ILog.DebugFormat(System.String,System.Object[])")]
        public bool IsBusy
        {
            get
            {
                lock (m_BusyLock)
                {
                    return m_IsBusy;
                }
            }
            set
            {
                lock (m_BusyLock)
                {
                    m_IsBusy = value;
                }
            }
        }

        /// <summary>
        /// Connects the specified bluetooth API.
        /// </summary>
        /// <param name="connectionProperties">The connection properties.</param>
        /// <param name="allowedBluetoothDongles">The allowed blutooth dongles</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Inspector.Hal.BluetoothHal.OnConnectFailed(System.String,System.Int32)")]
        public void Connect(Dictionary<string, string> connectionProperties, List<BluetoothDongleInformation> allowedBluetoothDongles)
        {
            CommunicationLogger.Debug("Connecting");
            m_ConnectionParameters = connectionProperties;
            m_AllowedBluetoothDongles = allowedBluetoothDongles;

            m_ConnectTimer.Change(m_ConnectTimeout, Timeout.Infinite);

            try
            {
                if (connectionProperties == null)
                {
                    throw new ConnectionException("No connection properties has been filled in.", ErrorCodes.HAL_CONNECTION_PROPERTIES_EMPTY);
                }

                ValidateConnectionProperties(connectionProperties);

                var bluetoothApi = connectionProperties[CONNECTIONPROPERTY_BLUETOOTHAPI];
                var destinationAddress = connectionProperties[CONNECTIONPROPERTY_DESTINATIONADDRESS];

                var radios = RetrieveAvailableBluetoothRadios(bluetoothApi);
                var radio = radios.FirstOrDefault();

                if (radio == null) throw new ConnectionException(string.Format(CultureInfo.CurrentCulture, "Failed to discover the Bluetooth radio."));

                Connect(destinationAddress, radio);
            }
            catch (ConnectionException ex)
            {
                CommunicationLogger.Debug("HAL: Connect error " + ex.Message);
                HandleConnectFailed(ex.Message, ex.ErrorCode);
            }
            catch (Exception ex)
            {
                CommunicationLogger.Debug("HAL: Connect error " + ex.Message);
                HandleConnectFailed(ex.Message, ErrorCodes.HAL_UNEXPECTED_ERROR);
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
            m_StoppingMeasurementTimer.Change(Timeout.Infinite, Timeout.Infinite);

            var disconnectWorkerThread = new Thread(DisconnectWorkerThread)
            {
                Name = "DisconnectWorkerThread"
            };

            disconnectWorkerThread.Start();
        }

        /// <summary>
        /// Disconnects the worker thread.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        private void DisconnectWorkerThread()
        {
            CommunicationLogger.Debug("Disconnecting");

            try
            {
                m_SerialConnection?.Disconnect();
            }
            catch
            {
                CommunicationLogger.Debug("HAL: Failed to close the connection");
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
            lock (m_BusyLock)
            {
                m_SerialDataReceived = string.Empty;

                CommunicationLogger.Debug($"HAL: Send command {command}");
                
                m_CurrentCommand = command;
                m_CurrentCommandParameter = commandParameter;

                switch (command)
                {
                    case DeviceCommand.EnterRemoteLocalCommandMode:
                        DoEnterRemoteLocalCommandMode(command);
                        break;
                    case DeviceCommand.ExitRemoteLocalCommandMode:
                    case DeviceCommand.FlushBluetoothCache:
                        DoSendDeviceCommand(command, commandParameter);
                        break;
                    case DeviceCommand.SwitchToManometerTH1:
                    case DeviceCommand.SwitchToManometerTH2:
                        DoSwitchToManometer(command);
                        break;
                    case DeviceCommand.CheckManometerPresent:
                    case DeviceCommand.CheckBatteryStatus:
                    case DeviceCommand.CheckSCPIInterface:
                    case DeviceCommand.InitiateSelfTest:
                    case DeviceCommand.CheckIdentification:
                    case DeviceCommand.CheckRange:
                    case DeviceCommand.SetPressureUnit:
                    case DeviceCommand.CheckPressureUnit:
                    case DeviceCommand.CheckSystemSoftwareVersion:
                    case DeviceCommand.CheckCalibrationDate:
                    case DeviceCommand.EnableIOStatus:
                    case DeviceCommand.DisableIOStatus:
                    case DeviceCommand.ActivatePortSensor:
                    case DeviceCommand.ActivatePortIrDa:
                    case DeviceCommand.StopSensorRun:
                    case DeviceCommand.ReadSensorId:
                    case DeviceCommand.CheckIo3Status:
                        DoSendManometerCommand(command, commandParameter);
                        break;
                    case DeviceCommand.IRAlwaysOn:
                        DoSendManometerCommand(command, string.Empty);
                        break;
                    case DeviceCommand.FlushManometerCache:
                        DoSendFlushManometerCommand(command, commandParameter);
                        break;
                    case DeviceCommand.MeasureContinuously:
                    case DeviceCommand.MeasureSingleValue:
                        try
                        {
                            DoSendCommand(command, commandParameter);
                        }
                        catch (ChecksumException)
                        {
                            /* Errors are already handled */
                        }
                        break;
                    case DeviceCommand.None:
                    case DeviceCommand.Connect:
                    case DeviceCommand.Disconnect:
                    default:
                        break;

                }
            }
        }

        /// <summary>
        /// Sends the command.
        /// </summary>
        /// <param name="command">The command.</param>
        public void SendCommand(DeviceCommand command)
        {
            SendCommand(command, string.Empty);
        }

        /// <summary>
        /// Starts the continuous measurement.
        /// </summary>
        /// <param name="frequency">The frequency in measurements per second.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.String.Format(System.String,System.Object)"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Inspector.Hal.BluetoothHal.OnMessageReceivedError(System.String,System.Int32)")]
        public void StartContinuousMeasurement(int frequency)
        {
            lock (m_BusyLock)
            {
                m_RetryMeasurement = 0;

                DoStartContinuousMeasurement(frequency.ToString(CultureInfo.InvariantCulture));
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Inspector.Hal.BluetoothHal.OnMessageReceivedError(System.String,System.Int32)")]
        private void DoStartContinuousMeasurement(string frequency)
        {
            lock (m_MeasuringLock)
            {
                DetachAllEventhandlers();

                m_SerialConnection.SerialDataEvent += SerialConnection_SerialDataEvent_Measurement;

                try
                {
                    SendCommand(DeviceCommand.MeasureContinuously, frequency);
                    m_MeasurementTimer.Change(m_MeasurementInterval, Timeout.Infinite);
                    m_MeasurementTimeoutTimer.Change(m_MeasurementTimeout, Timeout.Infinite);
                }
                catch (ChecksumException)
                {
                    CommunicationLogger.Debug("HAL: StartContinuousMeasurement, invalid checksum error.");
                    DetachAllEventhandlers();
                    m_SerialConnection.SerialDataEvent += SerialConnection_SerialDataEvent;
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
                m_StoppingMeasurement = true;

                CommunicationLogger.Debug("HAL: Begin of StopContinuousMeasurement");

                DetachAllEventhandlers();
                
                m_SerialConnection.SerialDataEvent += SerialConnection_SerialDataEvent_StoppingMeasurement;
                
                m_ContinuousMeasurementStartedEventRaised = false;

                lock (m_LockMeasurementData)
                {
                    m_MeasurementsReceived = string.Empty;
                }

                try
                {
                    m_MeasurementTimer.Change(Timeout.Infinite, Timeout.Infinite);
                    m_MeasurementTimeoutTimer.Change(Timeout.Infinite, Timeout.Infinite);
                    m_SerialDataReceived = string.Empty;
                    m_StoppedMeasurementTimedOut = false;
                    SendCommand(DeviceCommand.MeasureContinuously, "0");
                    m_StoppingMeasurementTimer.Change(FINALIZE_MEASUREMENT_WAIT_TIME, Timeout.Infinite);
                }
                catch (ChecksumException)
                {
                    DetachAllEventhandlers();
                    m_SerialConnection.SerialDataEvent += SerialConnection_SerialDataEvent;

                    CommunicationLogger.Debug("HAL: Stop continuous measurement, invalid checksum error.");
                    
                    OnMessageReceivedError("Failed to stop continuous measurement.", ErrorCodes.HAL_CONTINUOUS_MEASUREMENT_STOP_FAILED);
                }
                
                CommunicationLogger.Debug("HAL: End of StopContinuousMeasurement");
                
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
            m_StoppingMeasurement = false;

            if (ContinuousMeasurementStopped == null) return;

            CommunicationLogger.Debug("OnContinuousMeasurementStopped called");
            ContinuousMeasurementStopped(this, EventArgs.Empty);
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
            if (ContinuousMeasurementStarted == null) return;

            CommunicationLogger.Debug("OnContinuousMeasurementStarted called");
            ContinuousMeasurementStarted(this, EventArgs.Empty);
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
            if (!IsBusy) return;

            MessageReceived?.Invoke(this, new MessageReceivedEventArgs(data));
        }

        /// <summary>
        /// Occurs when [message received error].
        /// </summary>
        public event EventHandler MessageReceivedError;

        /// <summary>
        /// Called when [message received].
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="errorCode">The error code.</param>
        private void OnMessageReceivedError(string message, int errorCode)
        {
            if (IsBusy)
            {
                m_CurrentCommand = DeviceCommand.None;

                MessageReceivedError?.Invoke(this, new MessageErrorEventArgs(message, errorCode));
            }
            else
            {
                // This means a message is receiving a unexpecting error (devicecommand.none)
                // This indicates that the state is 'connected' in which errors cannot be handled.
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
            Connected?.Invoke(this, new ConnectedEventArgs(m_DeviceType));
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
            Disconnected?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Occurs when [device unpaired]
        /// </summary>
        public event EventHandler DeviceUnPaired;

        /// <summary>
        /// Called when [device unpaired]
        /// </summary>
        /// <param name="address">the address</param>
        private void OnDeviceUnPaired(string address)
        {
            DeviceUnPaired?.Invoke(this, new DeviceUnPairedEventArgs(address));
        }

        /// <summary>
        /// Occurs when [device unpair finished]
        /// </summary>
        public event EventHandler DeviceUnPairFinished;

        private void OnDeviceUnPairFinished()
        {
            DeviceUnPairFinished?.Invoke(this, EventArgs.Empty);
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
            int checksum;

            try
            {
                var chkSum = command.Sum(b => b).ToString("X");
                chkSum = chkSum.Substring(chkSum.Length - 2);

                checksum = int.Parse(chkSum, NumberStyles.HexNumber);
            }
            catch
            {
                throw new ChecksumException("Failed to compute Checksum.");
            }
            return checksum;
        }

        // this function is called only from the UI, which is a violation of the design (skipping over all layers)
        // this is 
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
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
                    CommunicationLogger.Debug("Failed to stop continuous measurement");
                }

                if (m_SerialConnection != null)
                {
                    CommunicationLogger.Debug("Disconnecting the connection");
                    m_SerialConnection.Disconnect();
                }

                CommunicationLogger.Debug("Succeeded to disconnect");

                return true;
            }
            catch
            {
                CommunicationLogger.Debug("Failed to disconnect");
                return false;
                //we are already handling unhandled exceptions. Throwing new exceptions is a bad idea.
            }
        }
        #endregion Public functions

        #region Private functions

        public void UnPairDevices(string address)
        {
            if (address != null)
            {
                using (var device = new wclBluetoothDevice())
                {
                    try
                    {
                        device.Address = address;

                        var settings = new clsSettings();

                        var bluetoothApi = settings.get_GetSetting(SETTING_CATEGORY, SETTING_BLUETOOTH_API).ToString();

                        var radios = RetrieveAvailableBluetoothRadios(bluetoothApi);
                        var radio = radios.FirstOrDefault();

                        device.Unpair(radio);
                        CommunicationLogger.Debug("device with address '" + address +"' succesfully unpaired");
                        OnDeviceUnPaired(device.Address);
                    }
                    finally
                    {
                        OnDeviceUnPairFinished();
                    }
                }
            }
            else
            {
                // Get radio
                var settings = new clsSettings();

                var bluetoothApi = settings.get_GetSetting(SETTING_CATEGORY, SETTING_BLUETOOTH_API).ToString();

                var radios = RetrieveAvailableBluetoothRadios(bluetoothApi);
                var radio = radios.FirstOrDefault();

                m_WclBluetoothDiscovery.OnDiscoveryComplete += WclBluetoothDiscovery_OnDiscoveryComplete;
                m_WclBluetoothDiscovery.Discovery(radio, 1);
            }
        }

        private void WclBluetoothDiscovery_OnDiscoveryComplete(object sender, wclBluetoothDiscoveryCompleteEventArgs e)
        {
            m_WclBluetoothDiscovery.OnDiscoveryComplete -= WclBluetoothDiscovery_OnDiscoveryComplete;

            try
            {
                var radio = e.Radio;
                for (uint i = 0; i < e.Devices.Count; i++)
                {
                    var device = e.Devices[i];

                    var paired = false;
                    device.GetPaired(radio, ref paired);

                    var name = string.Empty;
                    device.GetName(radio, ref name);

                    if (paired && (name.Equals("PLEXOR BT IRDA", StringComparison.OrdinalIgnoreCase) || name.Equals("PLEXOR BT WIS", StringComparison.OrdinalIgnoreCase)))
                    {
                        device.Unpair(radio);
                        CommunicationLogger.Debug("device with address '" + device.Address + "' succesfully unpaired");
                        OnDeviceUnPaired(device.Address);
                    }
                }
            }
            finally
            {
                OnDeviceUnPairFinished();
            }

        }

        /// <summary>
        /// Stops the continuous measurement without sending an error via OnMessageReceivedError event.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        private void StopContinuousMeasurementOnError()
        {
            lock (m_MeasuringLock)
            {
                DetachAllEventhandlers();
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
                        m_MeasurementsReceived = string.Empty;
                    }
                }
                catch
                {
                    /* OnMessageReceivedError must be handled by the caller of this function */
                }

                m_SerialConnection.SerialDataEvent += SerialConnection_SerialDataEvent;
                m_IsMeasuring = false;
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
                CommunicationLogger.Debug("OnStoppingMeasurementTimerTimeout m_StoppedMeasurementTimedOut == true");
                DetachAllEventhandlers();
                m_SerialDataReceived = string.Empty;
                m_StoppedMeasurementTimedOut = false;
                m_SerialConnection.SerialDataEvent += SerialConnection_SerialDataEvent;
                OnMessageReceivedError("Failed to stop the continuous measurement", ErrorCodes.HAL_CONTINUOUS_MEASUREMENT_STOP_FAILED);
            }
            else
            {
                CommunicationLogger.Debug("OnStoppingMeasurementTimerTimeout m_StoppedMeasurementTimedOut == false");
                m_SerialDataReceived = string.Empty;
                m_StoppedMeasurementTimedOut = true;
                CommunicationLogger.Debug("OnStoppingMeasurementTimerTimeout doing sendcommand flushamnometercache");
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
            errorCode = null;

            if (!Regex.IsMatch(reply, ManometerErrorReply.ERRORMESSAGE_FORMAT_REGEX)) return false;

            var messageContainsError = ManometerErrorReply.ErrorCodeLookup.ContainsKey(reply);

            errorCode = messageContainsError ? ManometerErrorReply.ErrorCodeLookup[reply] : ErrorCodes.MANOMETER_UNKNOWN_ERROR;

            return messageContainsError;
        }

        /// <summary>
        /// Parses the received device data if the data is complete.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Inspector.Hal.BluetoothHal.OnMessageReceivedError(System.String,System.Int32)")]
        private void ParseDeviceData(string pattern, string newline)
        {
            if (!m_SerialDataReceived.EndsWith(newline, StringComparison.OrdinalIgnoreCase)) return;

            m_Retry = 0;

            CommunicationLogger.Debug("HAL: Stopping Command Timer");
            m_CommandTimeoutTimer.Change(Timeout.Infinite, Timeout.Infinite);

            m_CurrentCommand = DeviceCommand.None;

            var dataReceived = m_SerialDataReceived;

            var m = Regex.Match(dataReceived, pattern, RegexOptions.Compiled);
            var data = m.Groups["Data"].ToString();
            if (!string.IsNullOrWhiteSpace(pattern) && m.Success)
            {
                OnMessageReceived(data);
            }
            else
            {
                var message = string.Format(CultureInfo.InvariantCulture, "Unknown data format received from Plexor device: '{0}'", m_SerialDataReceived);
                OnMessageReceivedError(message, ErrorCodes.HAL_MESSAGE_RECEIVED_INCORRECT_FORMAT);
            }

            m_SerialDataReceived = string.Empty;
        }

        /// <summary>
        /// Parses the manometer data if the data is complete.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "log4net.ILog.DebugFormat(System.String,System.Object[])"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.String.Format(System.String,System.Object[])"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.String.Format(System.String,System.Object,System.Object,System.Object)"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.Int32.ToString"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2241:Provide correct arguments to formatting methods"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Inspector.Hal.BluetoothHal.OnMessageReceivedError(System.String,System.Int32)")]
        private void ParseManometerData()
        {
            if (m_SerialDataReceived.EndsWith("\r", StringComparison.OrdinalIgnoreCase) || m_SerialDataReceived.EndsWith("\n", StringComparison.OrdinalIgnoreCase))
            {
                CommunicationLogger.DebugFormat("HAL: SendManometerData received '{0}', time: {1}", new object[]
                {
                    m_SerialDataReceived,
                    DateTime.Now.ToString("HH:mm:ss", CultureInfo.InvariantCulture)
                });

                StopCommandTimer();

                var requiresManometerErrorCheck = (m_CurrentCommand != DeviceCommand.FlushManometerCache);
                m_CurrentCommand = DeviceCommand.None;
                m_Retry = 0;

                var dataReceived = m_SerialDataReceived;

                try
                {
                    int computedChecksum;
                    int checksum;
                    string io;

                    var data = ParseManometerData(dataReceived, out checksum, out computedChecksum, out io);
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
                                var message = string.Format(CultureInfo.InvariantCulture, "The manometer replied with an error: '{0}' ", data);

                                OnMessageReceivedError(message, manometerErrorCode ?? 0);
                            }
                            else
                            {
                                OnMessageReceived(data);
                            }
                        }
                        else
                        {
                            Log.Info($"Error in checksum:\n\tDatareceived: {dataReceived}\n\tdata: {data}\n\tchecksum: {checksum}\n\tcomputedChecksum: {computedChecksum}");
                            OnMessageReceivedError(string.Format(CultureInfo.InvariantCulture, "Checksum for received message '{0}' incorrect (Expected: '{1}', Was: '{2}'", dataReceived, computedChecksum, checksum), ErrorCodes.HAL_MESSAGE_RECEIVED_INCORRECT_CHECKSUM);
                        }
                    }
                }
                catch (ChecksumException e)
                {
                    CommunicationLogger.Debug("HAL: SendManometerData checksumerror detected!");
                    Log.Info($"Exception in checksum:\n\tDatareceived: {dataReceived}\n\tException message: {e.Message}\n\tException data: {e.Data}");
                    OnMessageReceivedError(string.Format(CultureInfo.InvariantCulture, "Could not calculate the checksum for received message '{0}'.", dataReceived), ErrorCodes.HAL_MESSAGE_RECEIVED_INCORRECT_CHECKSUM);
                }
                catch (InvalidDataException e)
                {
                    CommunicationLogger.Debug("HAL: SendManometerData invalid data exception!");
                    Log.Info($"Exception in SendManometerData:\n\tDatareceived: {dataReceived}\n\tException message: {e.Message}\n\tException data: {e.Data}");
                    OnMessageReceivedError(string.Format(CultureInfo.InvariantCulture, "The received messsage has an incorrect format.", dataReceived), ErrorCodes.HAL_MESSAGE_RECEIVED_INCORRECT_FORMAT);
                }
                m_SerialDataReceived = string.Empty;
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "log4net.ILog.DebugFormat(System.String,System.Object[])"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.String.Format(System.String,System.Object[])"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.String.Format(System.String,System.Object,System.Object,System.Object)"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.Int32.ToString"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2241:Provide correct arguments to formatting methods"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Inspector.Hal.BluetoothHal.OnMessageReceivedError(System.String,System.Int32)")]
        private void ParseSingleMeasureValue()
        {
            if (m_SerialDataReceived.EndsWith("\r", StringComparison.OrdinalIgnoreCase) || m_SerialDataReceived.EndsWith("\n", StringComparison.OrdinalIgnoreCase))
            {
                CommunicationLogger.DebugFormat("HAL: MeasureSingleValue received '{0}', time: {1}", new object[]
                {
                    m_SerialDataReceived,
                    DateTime.Now.ToString("HH:mm:ss", CultureInfo.InvariantCulture)
                });
                StopCommandTimer();

                int computedChecksum;
                int checksum;
                string io;
                var dataReceived = m_SerialDataReceived;
                ParseManometerData(dataReceived, out checksum, out computedChecksum, out io);
                OnMessageReceived(Convert.ToInt32(io, 16).ToString());
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        private void SetUseChecksumSetting()
        {
            try
            {
                var settings = new clsSettings();

                var settingValue = settings.get_GetSetting(SETTING_CATEGORY, SETTING_USE_CHECKSUM).ToString();

                if (!settingValue.Equals(SETTING_RETURN_NO_VALUE, StringComparison.OrdinalIgnoreCase))
                {
                    m_UseChecksum = bool.Parse(settingValue);
                }
            }
            catch
            {
                CommunicationLogger.Debug("Failed to get 'UseChecksum' setting, using default value");
            }
        }

        /// <summary>
        /// Parses the manometer data.
        /// </summary>
        /// <param name="dataReceived">The data received.</param>
        /// <param name="checksum">The checksum.</param>
        /// <param name="computedChecksum">The computed checksum.</param>
        /// <param name="io">the status of the io</param>
        /// <returns>The data part of the <paramref name="dataReceived"/></returns>
        /// <exception cref="ChecksumException">Thrown when the checksum can not be computed of <paramref name="dataReceived"/>.</exception>
        /// <exception cref="InvalidDataException">Thrown when the received data is invalid.</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.String.Format(System.String,System.Object)")]
        private static string ParseManometerData(string dataReceived, out int checksum, out int computedChecksum, out string io)
        {
            string data;

            io = "0"; //RandomGenerator.Next(0,4).ToString(CultureInfo.InvariantCulture);
            checksum = int.MinValue;
            computedChecksum = int.MinValue;

            // Regex validates following inputformats that can be received from the device:
            //
            // "data\t\r"              --> Reaction from Manometer without Checksum
            // "data\t*checksum\r"     --> Reaction from Manometer with checksum. data may be empty.
            // "data\t*checksum\rIO\n" --> Reaction with new Manometer wireless connection framework.

            var regexStr = "^(?<Data>.*?)[\t|\r]{1,2}(\\*(?<Checksum>[^\r]+)\r)?(?<io>[0-9A-F]{0,2}?)[\r|\n]?$";
            var m = Regex.Match(dataReceived, regexStr, RegexOptions.Compiled);

            if (m.Success)
            {
                data = m.Groups["Data"].ToString();
                io = m.Groups["io"].ToString();

                CommunicationLogger.Debug($"Data received     : {dataReceived.Replace("\r", "[CR]").Replace("\n", "[LF]")}");
                CommunicationLogger.Debug($"Data              : {data}");
                CommunicationLogger.Debug($"Checksum          : {m.Groups["Checksum"]}");
                CommunicationLogger.Debug($"IO status         : {io}\n");
                try
                {
                    var requiresChecksumValidation = m.Groups["Checksum"].Success;
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
            var commandString = m_SerialCommands[command];
            
            Thread.Sleep(250);

            switch (m_DeviceType)
            {
                case DeviceType.PlexorBluetoothIrDA:
                    // Enter Remote local command mode: Send three "!" with pauses of 250 ms.
                    LogAndSendCommand(commandString);
                    Thread.Sleep(250);
                    LogAndSendCommand(commandString);
                    Thread.Sleep(250);
                    LogAndSendCommand(commandString);
                    Thread.Sleep(250);
                    break;
                case DeviceType.PlexorBluetoothWIS:
                    // Enter Remote local command mode: Send once "!".
                    LogAndSendCommand(commandString);
                    Thread.Sleep(250);
                    break;
                case DeviceType.Unknown:
                default:
                    break;
            }

            CommunicationLogger.Debug("HAL: Starting Command Timer");
            m_CommandTimeoutTimer.Change(m_ManometerCommandTimeout, Timeout.Infinite);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Inspector.Hal.BluetoothHal.OnMessageReceivedError(System.String,System.Int32)")]
        private void DoSwitchToManometer(DeviceCommand command)
        {
            switch (m_DeviceType)
            {
                case DeviceType.PlexorBluetoothIrDA:
                    var commandParameter = SettingsUtils.DTRIOAddress;
                    DoSendCommand(command, commandParameter);
                    break;
                case DeviceType.PlexorBluetoothWIS:
                    DoSendCommand(command, string.Empty);
                    break;
                case DeviceType.Unknown:
                default:
                    break;
            }

            CommunicationLogger.Debug("HAL: Starting Command Timer");
            m_CommandTimeoutTimer.Change(m_ManometerCommandTimeout, Timeout.Infinite);
        }

        /// <summary>
        /// Creates the virtual COM port.
        /// </summary>
        /// <param name="address">The destination address.</param>
        /// <param name="radio">The bluetooth radio</param>
        /// <exception cref="ConnectionException">Thrown if Creating the virtual comport fails</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.String.Format(System.String,System.Object)")]
        private static DeviceType GetBluetoothDeviceType(string address, wclBluetoothRadio radio)
        {
            var device = DeviceType.Unknown;

            if (radio == null) return device;

            using (var wclBluetoothDevice = new wclBluetoothDevice())
            {
                wclBluetoothDevice.Address = address;

                var deviceName = string.Empty;

                var errorCode = wclBluetoothDevice.GetName(radio, ref deviceName);
                if (errorCode == wclErrors.WCL_E_SUCCESS)
                {
                    if (deviceName.Equals(PlexorBluetoothIrDADeviceName, StringComparison.OrdinalIgnoreCase))
                    {
                        device = DeviceType.PlexorBluetoothIrDA;
                    }
                    else if (deviceName.Equals(PlexorBluetoothWISDeviceName, StringComparison.OrdinalIgnoreCase))
                    {
                        device = DeviceType.PlexorBluetoothWIS;
                    }
                }
                else
                {
                    var message = string.Format(CultureInfo.CurrentCulture, "Failed to get the Bluetooth device name. Error code: '{0}'. Error Message: '{1}'", errorCode.ToString(CultureInfo.CurrentCulture), wclErrors.wclGetErrorMessage(errorCode));
                    throw new ConnectionException(message, errorCode);
                }
            }

            return device;
        }

        /// <summary>
        /// Does the send device command.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="commandParameter">The command parameter.</param>
        private void DoSendDeviceCommand(DeviceCommand command, string commandParameter)
        {
            var commandString = m_SerialCommands[command];
            commandString = string.Format(CultureInfo.InvariantCulture, commandString, commandParameter);

            LogAndSendCommand(commandString);

            CommunicationLogger.Debug("HAL: Starting Command Timer");
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

                CommunicationLogger.Debug("HAL: Starting Command Timer");
                m_CommandTimeoutTimer.Change(m_ManometerCommandTimeout, Timeout.Infinite);
            }
            catch (ChecksumException)
            {
                /* Errors are handled by DoSendCommand */
                CommunicationLogger.Debug("HAL: Checksum error while Starting Command Timer.");
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
                CommunicationLogger.Debug("HAL: Starting Flush Manometer Cache Timer.");
                m_FlushManometerCacheTimeoutTimer.Change(m_FlushManometerCacheTimeout, Timeout.Infinite);
            }
            catch (ChecksumException)
            {
                /* Errors are handled by DoSendCommand */
                CommunicationLogger.Debug("HAL: Checksum error while Starting Flush Manometer Cache Timer.");
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
            var commandString = m_SerialCommands[command];
            try
            {
                commandString = string.Format(CultureInfo.InvariantCulture, commandString, commandParameter);
                switch (command)
                {
                    case DeviceCommand.SwitchToManometerTH1:
                    case DeviceCommand.SwitchToManometerTH2:
                        switch (m_DeviceType)
                        {
                            case DeviceType.PlexorBluetoothIrDA:
                                commandString = string.Format(CultureInfo.InvariantCulture, "{0}\r", commandString);
                                break;
                            case DeviceType.PlexorBluetoothWIS:
                                commandString += "\t";
                                commandString = string.Format(CultureInfo.InvariantCulture, "{0}\r", commandString);
                                break;
                            case DeviceType.Unknown:
                            default:
                                break;
                        }
                        break;
                    case DeviceCommand.CheckSystemSoftwareVersion:
                    case DeviceCommand.CheckCalibrationDate:
                    case DeviceCommand.EnableIOStatus:
                    case DeviceCommand.DisableIOStatus:
                    case DeviceCommand.CheckIo3Status:
                    case DeviceCommand.ActivatePortSensor:
                    case DeviceCommand.ReadSensorId:
                    case DeviceCommand.StopSensorRun:
                    case DeviceCommand.ActivatePortIrDa:
                        // Specific commands for Plexor Bluetooth WIS
                        commandString += "\t";
                        commandString = string.Format(CultureInfo.InvariantCulture, "{0}\r", commandString);
                        break;
                    case DeviceCommand.Connect:
                    case DeviceCommand.Disconnect:
                    case DeviceCommand.EnterRemoteLocalCommandMode:
                    case DeviceCommand.ExitRemoteLocalCommandMode:
                    case DeviceCommand.FlushManometerCache:
                    case DeviceCommand.CheckBatteryStatus:
                    case DeviceCommand.CheckSCPIInterface:
                    case DeviceCommand.InitiateSelfTest:
                    case DeviceCommand.CheckIdentification:
                    case DeviceCommand.CheckRange:
                    case DeviceCommand.SetPressureUnit:
                    case DeviceCommand.CheckPressureUnit:
                    case DeviceCommand.MeasureContinuously:
                    case DeviceCommand.MeasureSingleValue:
                    case DeviceCommand.FlushBluetoothCache:
                    case DeviceCommand.CheckManometerPresent:
                    case DeviceCommand.IRAlwaysOn:
                    case DeviceCommand.None:
                    default:
                        commandString += "\t*";
                        commandString = string.Format(CultureInfo.InvariantCulture, "{0}{1}\r", commandString, ComputeChecksum(commandString));
                        break;
                }
                CommunicationLogger.Debug(string.Format(CultureInfo.InvariantCulture, "HAL: DoSendCommand sending '{0}'", commandString.Replace("\t", "[HT]").Replace("\r", "[CR]").Replace("\n", "[LF]")));
                LogAndSendCommand(commandString);
            }
            catch (ChecksumException checksumException)
            {
                CommunicationLogger.DebugFormat("HAL: Checksum exception. Message: '{0}'", checksumException.Message);
                OnMessageReceivedError(string.Format(CultureInfo.InvariantCulture, "Failed to compute checksum: '{0}'", commandString), ErrorCodes.HAL_FAILED_CHECKSUM_COMPUTATION);
                throw;
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
            {
                // If a BT address is in allowed BT dongles: verify the existence
                ValidateBluetoothDongle(allowedBluetoothDongles, radios);
            }
            else
            {
                // If no BT address is in allowed BT dongles: try to find the first correct api radio and save that one to the settings.
                UseFirstBluetoothDongle();
            }
        }

        /// <summary>
        /// Uses the first bluetooth dongle.
        /// </summary>
        /// <exception cref="ConnectionException">Thrown when no bluetooth dongle could be found.</exception>
        private void UseFirstBluetoothDongle()
        {
            var settings = new clsSettings();
            var btApi = settings.get_GetSetting(SETTING_CATEGORY, SETTING_BLUETOOTH_API).ToString();
            if (btApi.Equals(SETTING_RETURN_NO_VALUE, StringComparison.OrdinalIgnoreCase))
            {
                throw new ConnectionException("No Bluetooth Api is defined in the settings.", ErrorCodes.HAL_BLUETOOTH_API_NOT_FOUND);
            }

            var btAddresses = RetrieveAvailableBluetoothDongles(btApi);
            if (btAddresses.Count == 0)
            {
                throw new ConnectionException("No Bluetooth dongles that are usable are found on the PC.", ErrorCodes.HAL_BLUETOOTH_DONGLE_NOT_FOUND);
            }

            settings.set_SaveSetting(SETTING_CATEGORY, SETTING_BLUETOOTH_DONGLE_ADDRESS, btAddresses[0]);
        }

        /// <summary>
        /// Validates the bluetooth dongle.
        /// </summary>
        /// <param name="allowedBluetoothDongles">The allowed bluetooth dongles.</param>
        /// <param name="radios">The radios.</param>
        /// <exception cref="ConnectionException">Thrown when no bluetooth dongle is available from the <paramref name="allowedBluetoothDongles"/> collection./></exception>
        private static void ValidateBluetoothDongle(List<BluetoothDongleInformation> allowedBluetoothDongles, wclBluetoothRadios radios)
        {
            var hasFoundAllowedDongle = false;

            for (uint i = 0; i < radios.Count; i++)
            {
                var btAddress = string.Empty;
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
            int result;

            try
            {
                var settings = new clsSettings();
                var settingValue = settings.get_GetSetting(SETTING_CATEGORY, setting).ToString();

                result = settingValue.Equals(SETTING_RETURN_NO_VALUE, StringComparison.OrdinalIgnoreCase) ? defaultValue : int.Parse(settingValue, CultureInfo.InvariantCulture);
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
            CommunicationLogger.Debug("HAL: Connect Timeout");

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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "log4net.ILog.DebugFormat(System.String,System.Object[])"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Inspector.Hal.BluetoothHal.OnMessageReceivedError(System.String,System.Int32)")]
        private void OnCommandTimeout(object state)
        {
            CommunicationLogger.Debug("HAL: OnCommandTimeout: starting, requesting m_BusyLock");

            lock (m_BusyLock)
            {
                CommunicationLogger.Debug("HAL: OnCommandTimeout: command timed out");
                m_CommandTimeoutTimer.Change(Timeout.Infinite, Timeout.Infinite);

                // Do a retry if needed, otherwise quit.
                if (m_Retry < m_ManometerCommandMaxRetries)
                {
                    m_Retry++;
                    System.Diagnostics.Debug.WriteLine("HAL: OnCommandTimeout retrying");
                    SendCommand(m_CurrentCommand, m_CurrentCommandParameter);
                }
                else
                {
                    m_Retry = 0;
                    OnMessageReceivedError("Timeout", ErrorCodes.HAL_COMMAND_TIMEOUT_RECEIVED);
                }
            }

            CommunicationLogger.Debug("HAL: OnCommandTimeout: ended, released m_BusyLock");
        }

        /// <summary>
        /// Called when [flush manometer cache timeout].
        /// </summary>
        /// <param name="state">The state.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Inspector.Hal.BluetoothHal.OnMessageReceivedError(System.String,System.Int32)"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Inspector.Hal.BluetoothHal.HandleConnectFailed(System.String,System.Int32)")]
        private void OnFlushManometerCacheTimeout(object state)
        {
            CommunicationLogger.Debug("HAL: Flush Manometer Cache Timeout");
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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.String.Format(System.String,System.Object,System.Object)"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.String.Format(System.String,System.Object)"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.String.Format(System.String,System.Object[])"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Inspector.Hal.BluetoothHal.OnMessageReceivedError(System.String,System.Int32)")]
        private void OnMeasurementTimerTick(object state)
        {
            lock (m_MeasuringLock)
            {
                if (m_StoppingMeasurement)
                {
                    return;
                }

                var checksumError = false;
                var measurementParseError = false;
                var invalidDataError = false;
                var checksum = 0;
                var computedChecksum = 0;
                var receivedMeasurement = string.Empty;
                var receivedIoStatus = string.Empty;
                var ioStatus = -1;
                var measurementValue = double.NaN;
                var parsedMeasurementValue = string.Empty;
                var measurementsData = GetMeasurementData();

                if (measurementsData.Length > 0)
                {
                    CommunicationLogger.Debug($"Measurements data : {measurementsData.Replace("\r", "[CR]").Replace("\n", "[LF]")}\n");

                    var splitter = measurementsData.Contains('\n') ? '\n' : '\r';
                    var measurements = measurementsData.Split(splitter);
                    var parsedMeasurements = new List<Measurement>();

                    foreach (var item in measurements)
                    {
                        if (string.IsNullOrEmpty(item))
                        {
                            continue;
                        }

                        var measurement = $"{item}{splitter}";

                        CommunicationLogger.Debug($"Measurement       : {measurement.Replace("\r", "[CR]").Replace("\n", "[LF]")}");

                        if (m_UseChecksum)
                        {
                            try
                            {
                                parsedMeasurementValue = ParseManometerData(measurement, out checksum, out computedChecksum, out receivedIoStatus);
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

                            try
                            {
                                if (!string.IsNullOrEmpty(receivedIoStatus))
                                {
                                    // IO status is a hexadecimal string
                                    ioStatus = Convert.ToInt32(receivedIoStatus, 16);
                                }
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
                                parsedMeasurementValue = ParseManometerData(measurement, out checksum, out computedChecksum, out receivedIoStatus);
                                measurementValue = double.Parse(parsedMeasurementValue, CultureInfo.InvariantCulture);

                                if (!string.IsNullOrEmpty(receivedIoStatus))
                                {
                                    ioStatus = Convert.ToInt32(receivedIoStatus, 16);
                                }
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

                        parsedMeasurements.Add(new Measurement(measurementValue, ioStatus));
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
                                Log.Info($"Exception in checksum:\n\t ReceivedMeasurement: {receivedMeasurement}\n\t MeasurementValue: {measurementValue}\n\t checksum: {checksum}\n\t computedchecksum: {computedChecksum}");
                                OnMessageReceivedError(string.Format(CultureInfo.InvariantCulture, "Checksum for received message '{0}' incorrect (Expected: '{1}', Was: '{2}'", measurementValue, checksum, computedChecksum), ErrorCodes.HAL_MESSAGE_RECEIVED_INCORRECT_CHECKSUM);
                            }
                            else
                            {
                                OnMessageReceivedError(string.Format(CultureInfo.InvariantCulture, "Invalid measurement value '{0}'. Value is not a valid double type.", parsedMeasurementValue), ErrorCodes.HAL_MESSAGE_RECEIVED_INCORRECT_FORMAT);
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
        }

        private void RetryContinuousMeasurement()
        {
            CommunicationLogger.Debug("HAL: Retrying Continuous Measurement");
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
            string measurementsString;

            lock (m_LockMeasurementData)
            {
                var splitter = m_MeasurementsReceived.Contains('\n') ? '\n' : '\r';

                measurementsString = m_MeasurementsReceived.Substring(0, m_MeasurementsReceived.LastIndexOf(splitter) + 1);

                m_MeasurementsReceived = measurementsString.Length == m_MeasurementsReceived.Length ? string.Empty : m_MeasurementsReceived.Substring(m_MeasurementsReceived.LastIndexOf(splitter) + 1);
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
                m_MeasurementsReceived = string.Empty;
            }

            lock (m_MeasuringLock)
            {
                //Note that this assumes that a timeout on this timer can only occur when we are in the state Continuousmeasure
                //if this is not the case, and we have retried more than allowed and m_Ismeasuring is false, a stateMeachineException will be thrown
                if (!m_IsMeasuring && m_RetryMeasurement < m_MeasurementCommandMaxRetries)
                {
                    CommunicationLogger.Debug("Measurement timeout, retrying measurement.");
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
        private void Connect(string address, wclBluetoothRadio radio)
        {
            try
            {
                m_SerialConnection?.Dispose();

                m_SerialConnection = new SerialConnection();
                m_SerialConnection.Connected += (sender, args) =>
                {
                    m_Retry = 0;

                    m_ConnectTimer.Change(Timeout.Infinite, Timeout.Infinite);

                    m_DeviceType = GetBluetoothDeviceType(address, radio);

                    CreateCommandList();

                    OnConnected();                    
                };

                m_SerialConnection.Disconnected += (sender, args) => { };
                m_SerialConnection.Connect(address, radio);
                
				DetachAllEventhandlers();
				
                m_SerialConnection.SerialDataEvent += SerialConnection_SerialDataEvent;
                m_SerialConnection.SerialDataErrorEvent += SerialConnection_SerialDataErrorEvent;
            }
            catch (ConnectionException connectionException)
            {
                HandleConnectFailed(connectionException.Message, connectionException.ErrorCode);
            }
            catch (Exception exception)
            {
                var message = string.Format(CultureInfo.InvariantCulture, "Creation of the serial connection failed. Exception: '{0}'.", exception.Message);
                HandleConnectFailed(message, ErrorCodes.HAL_SERIAL_ERROR_PORT_CREATION);
            }
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
            if (connectionProperties == null || !connectionProperties.ContainsKey(property))
            {
                throw new ConnectionException(string.Format(CultureInfo.InvariantCulture, "Required key '{0}' not found in the connection properties", property), ErrorCodes.HAL_CONNECTION_PROPERTY_NOT_FOUND);
            }
        }

        /// <summary>
        /// Creates the command list.
        /// </summary>
        private void CreateCommandList()
        {
            m_SerialCommands.Clear();

            // When the command is executed, replace <var> ({0}) with the actual value
            m_SerialCommands.Add(DeviceCommand.EnterRemoteLocalCommandMode, "!"); // repeat three times
            m_SerialCommands.Add(DeviceCommand.ExitRemoteLocalCommandMode, "ATO\r");
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
            m_SerialCommands.Add(DeviceCommand.IRAlwaysOn, "ENG:IRDA:INIT ON");
            m_SerialCommands.Add(DeviceCommand.CheckIo3Status, "IO:IN2:INPUT ?");
            m_SerialCommands.Add(DeviceCommand.ActivatePortSensor, "IO:RS232 SENSOR");
            m_SerialCommands.Add(DeviceCommand.ReadSensorId, "SENS:IDEN ?");
            m_SerialCommands.Add(DeviceCommand.ActivatePortIrDa, "IO:RS232 IRDA");
            m_SerialCommands.Add(DeviceCommand.StopSensorRun, "SENS:RUN STOP");
            m_SerialCommands.Add(DeviceCommand.MeasureSingleValue, "MEAS:PRES?");
            switch (m_DeviceType)
            {
                case DeviceType.PlexorBluetoothIrDA:
                    m_SerialCommands.Add(DeviceCommand.SwitchToManometerTH1, "ATS{0}=1");
                    m_SerialCommands.Add(DeviceCommand.SwitchToManometerTH2, "ATS{0}=0");
                    break;
                case DeviceType.PlexorBluetoothWIS:
                case DeviceType.Unknown:
                default:
                    m_SerialCommands.Add(DeviceCommand.SwitchToManometerTH1, "IRDA:CH1 ON");
                    m_SerialCommands.Add(DeviceCommand.SwitchToManometerTH2, "IRDA:CH2 ON");
                    break;
            }
            m_SerialCommands.Add(DeviceCommand.CheckSystemSoftwareVersion, "SYST:VERS?");
            m_SerialCommands.Add(DeviceCommand.CheckCalibrationDate, "SYST:CALIB:DATE?");
            m_SerialCommands.Add(DeviceCommand.EnableIOStatus, "SYST:IO ON");
            m_SerialCommands.Add(DeviceCommand.DisableIOStatus, "SYST:IO OFF");
        }

        private void LogAndSendCommand(string command)
        {
            CommunicationLogger.Info("Sending data: " + command);
            m_SerialConnection.SendCommand(command);
        }

        private void DetachAllEventhandlers()
        {
            CommunicationLogger.Info("detaching all event handlers");
            m_SerialConnection.SerialDataEvent -= SerialConnection_SerialDataEvent;
            m_SerialConnection.SerialDataEvent -= SerialConnection_SerialDataEvent_Measurement;
            m_SerialConnection.SerialDataEvent -= SerialConnection_SerialDataEvent_StoppingMeasurement;
            
        }
        #endregion Private functions
    }
}
