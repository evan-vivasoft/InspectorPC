/*
//===============================================================================
// Copyright Kamstrup
// All rights reserved.
//===============================================================================
*/

using System;
using System.Globalization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

using Inspector.Hal.Interfaces.Exceptions;
using Inspector.Infra.Utils;
using Inspector.Model;

using KAM.INSPECTOR.Infra;
using log4net;
using wcl;

namespace Inspector.Hal
{
    /// <summary>
    /// SerialDataEventArgs
    /// </summary>
    public class SerialDataEventArgs : EventArgs
    {
        public SerialDataEventArgs(string serialData)
        {
            SerialData = serialData;
        }

        public string SerialData { get; private set; }
    }

    /// <summary>
    /// SerialDataErrorEventArgs
    /// </summary>
    public class SerialDataErrorEventArgs : EventArgs
    {
        public SerialDataErrorEventArgs(int errorCode, string message)
        {
            ErrorCode = errorCode;
            Message = message;
        }

        public string Message { get; private set; }
        public int ErrorCode { get; private set; }
    }

    /// <summary>
    /// SerialConnection
    /// </summary>
    public class SerialConnection : IDisposable
    {
        #region Constants
        private const string SETTING_CATEGORY = "PLEXOR";
        private const string SETTING_COMMAND_INTERVAL = "CommandInterval";
        private const string SETTING_READ_BUFFER_SIZE = "ReadBufferSize";
        #endregion Constants

        #region Class Members
        private bool m_Disposed;

        private readonly ExecutionContext m_ThreadContext;

        private readonly wclAuthenticator m_WclAuthenticator;
        private readonly wclClient m_WclClient;

        private readonly object m_Lock = new object();
        private readonly int m_CommandTimeout;

        private readonly int m_ReadBufferSize = 10000;
        private readonly int m_WriteBufferSize = 100;

        private static readonly ILog CommunicationLogger = LogManager.GetLogger("CommunicationLogger");

        #endregion Class Members

        #region Properties
        /// <summary>
        /// Gets a value indicating whether [serial port open].
        /// For unit testing purposes only!
        /// </summary>
        /// <value>
        ///   <c>true</c> if [serial port open]; otherwise, <c>false</c>.
        /// </value>
        public bool SerialPortOpen => m_WclClient.State == wclClientState.csConnected;

        #endregion Properties

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="SerialConnection"/> class.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.Int32.Parse(System.String)")]
        public SerialConnection()
        {
            var settings = new clsSettings();

            m_CommandTimeout = int.Parse(settings.get_GetSetting(SETTING_CATEGORY, SETTING_COMMAND_INTERVAL).ToString());
            m_ReadBufferSize = int.Parse(settings.get_GetSetting(SETTING_CATEGORY, SETTING_READ_BUFFER_SIZE).ToString());

            m_ThreadContext = CreateExecutionContext();


            m_WclAuthenticator = new wclAuthenticator();

            m_WclAuthenticator.OnPaired += WclAuthenticatorOnOnPaired;
            m_WclAuthenticator.OnPINRequest += WclAuthenticatorOnOnPinRequest;
            m_WclAuthenticator.OnNumericComparison += WclAuthenticatorOnOnNumericComparison;
            m_WclAuthenticator.OnPasskey += WclAuthenticatorOnOnPasskey;
            m_WclAuthenticator.OnPasskeyNotification += WclAuthenticatorOnOnPasskeyNotification;

            m_WclClient = new wclClient();

            m_WclClient.Buffers.ReadBuffer = (uint)m_ReadBufferSize;
            m_WclClient.Buffers.WriteBuffer = (uint)m_WriteBufferSize;

            DetachEvents();

            m_WclClient.OnConnect += WclClientOnOnConnect;
            m_WclClient.OnDisconnect += WclClientOnOnDisconnect;
            m_WclClient.OnData += WclClientOnOnData;
        }
        #endregion Constructors

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
        protected virtual void Dispose(bool disposing)
        {
            if (!m_Disposed)
            {
                if (disposing)
                {
                    DetachEvents();
                    
                    m_WclClient.Dispose();

                    if (m_WclAuthenticator.Active)
                    {
                        m_WclAuthenticator.Close();
                    }

                    m_WclAuthenticator.Dispose();

                    m_ThreadContext.Dispatcher.InvokeShutdown();
                }
            }

            m_Disposed = true;
        }

        #endregion IDisposable

        #region Events

        public event EventHandler Connected;

        protected virtual void OnConnected(EventArgs e)
        {
            var handler = Connected;

            handler?.Invoke(this, e);
        }

        public event EventHandler Disconnected;

        protected virtual void OnDisconnected(EventArgs e)
        {
            var handler = Disconnected;

            handler?.Invoke(this, e);
        }

        public event EventHandler<SerialDataEventArgs> SerialDataEvent;

        /// <summary>
        /// Called when [serial data event].
        /// </summary>
        /// <param name="serialData">The serial data.</param>
        private void OnSerialDataEvent(string serialData)
        {
            SerialDataEvent?.Invoke(this, new SerialDataEventArgs(serialData));
        }

        public event EventHandler<SerialDataErrorEventArgs> SerialDataErrorEvent;

        /// <summary>
        /// Called when [serial data error event].
        /// </summary>
        /// <param name="errorCode">The error code.</param>
        /// <param name="message">The message.</param>
        private void OnSerialDataErrorEvent(int errorCode, string message)
        {
            SerialDataErrorEvent?.Invoke(this, new SerialDataErrorEventArgs(errorCode, message));
        }

        #endregion Events

        #region WclAuthenticator Events

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.String.Format(System.String,System.Object,System.Object)")]
        private static void WclAuthenticatorOnOnPaired(object sender, wclPairedEventArgs args)
        {
            if (args.Error == wclErrors.WCL_E_SUCCESS)
            {
                CommunicationLogger.Debug("Succeeded to pair the Bluetooth device.");
            }
            else
            {
                var message = $"Failed to pair the Bluetooth device. Error code: '{args.Error}'. Error Message: '{wclErrors.wclGetErrorMessage(args.Error)}'";
                CommunicationLogger.Debug(message);

                throw new ConnectionException(message, args.Error);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.String.Format(System.String,System.Object)")]
        private static void WclAuthenticatorOnOnPinRequest(object sender, wclPINRequestEventArgs args)
        {
            args.PIN = SettingsUtils.PinCode;
            CommunicationLogger.Debug($"PIN code requested by the Bluetooth device. Provide PIN code: {args.PIN}");
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.String.Format(System.String,System.Object)")]
        private static void WclAuthenticatorOnOnNumericComparison(object sender, wclNumericComparisonEventArgs args)
        {
            args.Confirm = true;
            CommunicationLogger.Debug($"Numeric comparision requested by the Bluetooth device. Provide Confirm: {args.Confirm}");
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.String.Format(System.String,System.Object)")]
        private static void WclAuthenticatorOnOnPasskey(object sender, wclNumericComparisonEventArgs args)
        {
            uint code;
            if (!uint.TryParse(SettingsUtils.PinCode, out code))
            {
                CommunicationLogger.Debug("Unable to read pinCode from settings, using default 0000");
                code = 0000;
            }

            args.Value = code;
            CommunicationLogger.Debug($"PIN code requested by the Bluetooth device. Provide Passkey: {args.Value}");
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.String.Format(System.String,System.Object)")]
        private static void WclAuthenticatorOnOnPasskeyNotification(object sender, wclPasskeyEventArgs args)
        {
            CommunicationLogger.Debug($"Passkey notification by the Bluetooth device. Received Passkey: {args.Value}");
        }

        #endregion WclAuthenticator Events

        #region WclClient Events

        /// <summary>
        /// Handles the OnConnect event of the wclClient control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args"></param>
        private void WclClientOnOnConnect(object sender, wclConnectEventArgs args)
        {
            if (args.Error != wclErrors.WCL_E_SUCCESS) 
			{			    
                var message = string.Format(CultureInfo.CurrentCulture, "Failed to create connection to the Bluetooth device. Error code: '{0}'. Error Message: '{1}'", args.Error.ToString(CultureInfo.CurrentCulture), wclErrors.wclGetErrorMessage(args.Error));

                CommunicationLogger.Debug(message);

                return;
			}

            CommunicationLogger.Debug("Succeeded to connect the Bluetooth device.");

            OnConnected(args);
        }

        /// <summary>
        /// Handles the OnDisconnect event of the wclClient control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args"></param>
        private void WclClientOnOnDisconnect(object sender, EventArgs args)
        {
            CommunicationLogger.Debug("Succeeded to disconnect the Bluetooth device.");

            OnDisconnected(args);
        }

        /// <summary>
        /// Handles the OnData event of the wclClient control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args"></param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.String.Format(System.String,System.Object)")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        private void WclClientOnOnData(object sender, wclDataEventArgs args)
        {
            CommunicationLogger.Debug($"Connection: received total '{args.Size}'");

            lock (m_Lock)
            {
                var data = Encoding.Default.GetString(args.Data);

                OnSerialDataEvent(data);
            }
        }

        #endregion WclClient Events

        #region Public Functions

        /// <summary>
        /// Connects this instance.
        /// </summary>
        /// <exception cref="ConnectionException">Could not connect to Serial Port</exception>
        public void Connect(string address, wclBluetoothRadio radio)
        {
            switch (m_WclClient.State)
            {
                case wclClientState.csDisconnected:
                    m_ThreadContext.Dispatcher.Invoke(new Action(() =>
                    {
                        m_WclAuthenticator.Radio = radio;
                        var errorCode = m_WclAuthenticator.Open();

                        if (errorCode != wclErrors.WCL_E_SUCCESS)
                        {
                            var message = string.Format(CultureInfo.CurrentCulture, "Failed to open authenticator for the Bluetooth device. Error code: '{0}'. Error Message: '{1}'", errorCode.ToString(CultureInfo.CurrentCulture), wclErrors.wclGetErrorMessage(errorCode));
                            throw new ConnectionException(message, errorCode);
                        }

                        m_WclClient.BluetoothParams.Address = address;
                        m_WclClient.BluetoothParams.Radio = radio;
                        m_WclClient.BluetoothParams.Service = wclUUIDs.SerialPortServiceClass_UUID;
                        m_WclClient.Transport = wclTransport.trBluetooth;
                        
                        errorCode = m_WclClient.Connect();

                        if (errorCode == wclErrors.WCL_E_SUCCESS) return;
                        {
                            var message = string.Format(CultureInfo.CurrentCulture, "Failed to create connection to the Bluetooth device. Error code: '{0}'. Error Message: '{1}'", errorCode.ToString(CultureInfo.CurrentCulture), wclErrors.wclGetErrorMessage(errorCode));
                            throw new ConnectionException(message, errorCode);
                        }
                    }));
                    break;
                case wclClientState.csDisconnecting:
                case wclClientState.csConnected:
                case wclClientState.csConnecting:
                default:
                    break;
            }
        }

        /// <summary>
        /// Disconnects this instance.
        /// </summary>
        /// <exception cref="ConnectionException">Could not close Serial Port.</exception>
        public void Disconnect()
        {
            try
            {
                DetachEvents();

                m_WclClient.Disconnect();

                if (m_WclAuthenticator.Active)
                {
                    m_WclAuthenticator.Close();
                }
            }
            catch (Exception ex)
            {
                throw new ConnectionException("Failed to disconnect. Error Message: " + ex.Message, ErrorCodes.HAL_SERIAL_ERROR_CLOSING_CONNECTION);
            }
        }

        public void DetachEvents()
        {
            m_WclClient.OnConnect -= WclClientOnOnConnect;
            m_WclClient.OnDisconnect -= WclClientOnOnDisconnect;
            m_WclClient.OnData -= WclClientOnOnData;
        }

        /// <summary>
        /// Sends the command.
        /// </summary>
        /// <param name="command">The command.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        public void SendCommand(string command)
        {
            System.Diagnostics.Debug.WriteLine("Sending command '"+ command +"' again, time: " + DateTime.Now.ToString("HH:mm:ss", CultureInfo.InvariantCulture));

            try
            {
                var data = Encoding.ASCII.GetBytes(command);

                if (m_WclClient.State != wclClientState.csConnected) return;

                Thread.Sleep(m_CommandTimeout);

                m_WclClient.Write(data, (uint)data.Length);
                
                //System.Diagnostics.Debug.WriteLine("Sending Command: " + command);
            }
            catch (Exception ex)
            {
                OnSerialDataErrorEvent(ErrorCodes.HAL_SERIAL_ERROR_SEND_COMMAND, ex.Message);
            }
        }
        #endregion Public Functions

        private static ExecutionContext CreateExecutionContext()
        {
            var taskCompletionSource = new TaskCompletionSource<ExecutionContext>();

            var thread = new Thread(() =>
            {
                // Create the context, and install it:
                var dispatcher = Dispatcher.CurrentDispatcher;
                var syncContext = new DispatcherSynchronizationContext(dispatcher);

                SynchronizationContext.SetSynchronizationContext(syncContext);

                taskCompletionSource.SetResult(new ExecutionContext
                {
                    DispatcherSynchronizationContext = syncContext,
                    Dispatcher = dispatcher
                });

                // Start the Dispatcher Processing
                Dispatcher.Run();
            });

            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();

            return taskCompletionSource.Task.Result;
        }

        private class ExecutionContext
        {
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
            public DispatcherSynchronizationContext DispatcherSynchronizationContext { get; set; }
            public Dispatcher Dispatcher { get; set; }
        }
    }
}
