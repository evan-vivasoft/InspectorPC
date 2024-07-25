/*
//===============================================================================
// Copyright Kamstrup
// All rights reserved.
//===============================================================================
*/

using System;
using System.IO.Ports;
using System.Text;
using Inspector.Hal.Interfaces.Exceptions;
using Inspector.Model;
using KAM.INSPECTOR.Infra;

namespace Inspector.Hal
{
    /// <summary>
    /// SerialDataEventArgs
    /// </summary>
    public class SerialDataEventArgs : EventArgs
    {
        public SerialDataEventArgs(string serialData)
        {
            this.SerialData = serialData;
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
            this.ErrorCode = errorCode;
            this.Message = message;
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
        #endregion Constants


        #region Class Members
        private bool m_Disposed;

        private SerialPort m_SerialPort;
        private readonly object thisLock = new object();
        private int m_CommandTimeout;
        private int m_BaudRate = 9600;
        private int m_DataBits = 8;
        private Handshake m_Handshake = Handshake.None;
        private Parity m_Parity = Parity.None;
        private string m_PortNumber = String.Empty;
        private StopBits m_StopBits = StopBits.One;
        private bool m_RtsEnable = false;
        private int m_ReadBufferSize = 100;
        private int m_ReadTimeout = 500;
        private int m_WriteBufferSize = 100;
        private int m_WriteTimeout = 500;
        #endregion Class Members

        #region Properties
        /// <summary>
        /// Gets a value indicating whether [serial port open].
        /// For unit testing purposes only!
        /// </summary>
        /// <value>
        ///   <c>true</c> if [serial port open]; otherwise, <c>false</c>.
        /// </value>
        public bool SerialPortOpen
        {
            get { return m_SerialPort.IsOpen; }
        }
        #endregion Properties

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="SerialConnection"/> class.
        /// </summary>
        /// <param name="serialPortNumber">The serial port number.</param>
        /// <exception cref="ArgumentException">Thrown when the <paramref name="m_PortNumber"/> is invalid.</exception>
        /// <exception cref="InvalidOperationException">Thrown when the <paramref name="m_PortNumber"/> was already open (in use).</exception>
        /// <exception cref="IOException">Thrown when the port is in an invalid state or when parameters passed to the serial port connection were invalid.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when data read or written to the port failed.</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.String.Format(System.String,System.Object)")]
        public SerialConnection(string serialPortNumber)
        {
            m_SerialPort = new SerialPort();

            m_PortNumber = String.Format("COM{0}", serialPortNumber);
            InitializeSerialPort();

            clsSettings settings = new clsSettings();
            m_CommandTimeout = int.Parse(settings.get_GetSetting(SETTING_CATEGORY, SETTING_COMMAND_INTERVAL).ToString());
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
                    m_SerialPort.DataReceived -= new SerialDataReceivedEventHandler(SerialPort_DataReceived);
                    m_SerialPort.ErrorReceived -= new SerialErrorReceivedEventHandler(SerialPort_ErrorReceived);

                    m_SerialPort.Dispose();
                }
            }

            m_Disposed = true;
        }

        #endregion IDisposable

        #region Events
        public event EventHandler<SerialDataEventArgs> SerialDataEvent;

        /// <summary>
        /// Called when [serial data event].
        /// </summary>
        /// <param name="serialData">The serial data.</param>
        private void OnSerialDataEvent(string serialData)
        {
            if (SerialDataEvent != null)
            {
                SerialDataEvent.Invoke(this, new SerialDataEventArgs(serialData));
            }
        }

        public event EventHandler<SerialDataErrorEventArgs> SerialDataErrorEvent;

        /// <summary>
        /// Called when [serial data error event].
        /// </summary>
        /// <param name="errorCode">The error code.</param>
        /// <param name="message">The message.</param>
        private void OnSerialDataErrorEvent(int errorCode, string message)
        {
            if (SerialDataErrorEvent != null)
            {
                SerialDataErrorEvent.Invoke(this, new SerialDataErrorEventArgs(errorCode, message));
            }
        }
        #endregion Events

        #region Serial Port Events
        /// <summary>
        /// Handles the DataReceived event of the m_SerialPort control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.IO.Ports.SerialDataReceivedEventArgs"/> instance containing the event data.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            lock (thisLock)
            {
                string data = String.Empty;
                try
                {
                    while (m_SerialPort.IsOpen && m_SerialPort.BytesToRead > 0)
                    {
                        byte[] byte_data = ReadExistingBytes();

                        //remove the parity
                        //byte_data = uniparity.SetParityNone(byte_data); 
                        data += Encoding.Default.GetString(byte_data);
                        //System.Diagnostics.Debug.WriteLine("SerialConnection: received part '{0}'", new object[] { data });
                    }
                    System.Diagnostics.Debug.WriteLine("SerialConnection: received total '{0}'", new object[] { data });

                    OnSerialDataEvent(data);
                }
                catch (Exception exception)
                {
                    System.Diagnostics.Debug.WriteLine("SerialConnection: Exception thrown '{0}'", new object[] { exception.ToString() });

                    OnSerialDataErrorEvent(ErrorCodes.HAL_SERIAL_ERROR_OVERRUN, exception.Message);
                }
            }
        }

        /// <summary>
        /// Reads the existing bytes.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">The buffer passed is null.</exception>
        /// <exception cref="InvalidOperationException">The specified port is not open.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The offset or count parameters are outside a valid region of the buffer being passed. Either offset or count is less than zero.</exception>
        /// <exception cref="ArgumentException">The offset plus count is greater than the length of the buffer.</exception>
        /// <exception cref="TimeoutException">No bytes were available to read.</exception>
        private byte[] ReadExistingBytes()
        {
            int readCount = 0;
            byte[] buffer = new byte[m_SerialPort.ReadBufferSize];

            readCount = m_SerialPort.Read(buffer, 0, m_SerialPort.BytesToRead);

            byte[] temp = new byte[readCount];
            for (int i = 0; i < readCount; i++)
            {
                temp[i] = buffer[i];
            }
            return temp;
        }

        /// <summary>
        /// Handles the ErrorReceived event of the m_SerialPort control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.IO.Ports.SerialErrorReceivedEventArgs"/> instance containing the event data.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Inspector.Hal.SerialConnection.OnSerialDataErrorEvent(System.Int32,System.String)")]
        void SerialPort_ErrorReceived(object sender, SerialErrorReceivedEventArgs e)
        {
            lock (thisLock)
            {
                // Standard report the error.
                string result = String.Empty;
                int errorCode = 0;
                bool reportError = true;

                // information retrieved from http://www.lookrs232.com/rs232/lsr.htm        
                switch (e.EventType)
                {
                    case SerialError.Frame: //receive
                        // This error occurs if the last bit is not a stop bit. This generally 
                        // happens because of synchronization error. You may face this 
                        // error when connecting two computers with the help of null 
                        // modem, if the baud rates of the transmitting computer 
                        // differs from the baud rates of the receiving computer.
                        result = "error received: Frame";
                        errorCode = ErrorCodes.HAL_SERIAL_ERROR_FRAME;
                        break;
                    case SerialError.Overrun: //receive
                        // This error usually occurs when the data are read from the port slower 
                        // than they are received. If you don't read the incoming bytes fast enough 
                        // the last byte can be overwritten with the byte which was received last, 
                        // in this case the last byte may be lost which will cause overrun error.
                        result = "error received: Overrun";
                        errorCode = ErrorCodes.HAL_SERIAL_ERROR_OVERRUN;
                        break;
                    case SerialError.RXOver: //receive
                        // An input buffer overflow has occurred. There is either no room in the 
                        // input buffer, or a character was received after the end-of-file 
                        // (EOF) character. 
                        result = "error received: RXOver";
                        errorCode = ErrorCodes.HAL_SERIAL_ERROR_RXOVER;
                        break;
                    case SerialError.RXParity: // receive
                        // This error occurs if the parity doesn't coincide with the 
                        // parameters set when the byte is received.
                        result = "error received: RXParity. Ignoring error.";
                        errorCode = ErrorCodes.HAL_SERIAL_ERROR_RXPARITY;

                        // Do not propagate this error. Only log.
                        reportError = false;
                        break;
                    case SerialError.TXFull: // transmit
                        // The application tried to transmit a character, but the output buffer 
                        // was full. 
                        result = "error received: TXFull";
                        errorCode = ErrorCodes.HAL_SERIAL_ERROR_TXFULL;
                        break;
                    default:
                        result = "error received: Undefined";
                        errorCode = ErrorCodes.HAL_SERIAL_ERROR_UNDEFINED;
                        break;
                }

                if (reportError)
                {
                    System.Diagnostics.Debug.WriteLine("SerialConnection: error receiving code '{0}'", errorCode);
                    OnSerialDataErrorEvent(errorCode, result);
                }
            }
        }
        #endregion Serial Port Events

        #region Public Functions
        /// <summary>
        /// Sets or gets the DTR enabled status.
        /// </summary>
        /// <param name="value">if set to <c>true</c> [value].</param>
        /// <exception cref="IOException">
        /// The port is in an invalid state or an attempt to set the state 
        /// of the underlying port failed. For example, the parameters passed 
        /// from this SerialPort object were invalid.
        /// </exception>
        public bool DtrEnabled
        {
            get
            {
                return m_SerialPort.DtrEnable;
            }
            set
            {
                m_SerialPort.DtrEnable = value;
            }
        }

        /// <summary>
        /// Connects this instance.
        /// </summary>
        /// <exception cref="ConnectionException">Could not connect to Serial Port</exception>
        public void Connect()
        {
            try
            {
                if (!m_SerialPort.IsOpen)
                {
                    m_SerialPort.Open();
                    System.Threading.Thread.Sleep(500); // Give the serial port some time to be opened before we continue.
                }
            }
            catch (Exception ex)
            {
                throw new ConnectionException("Could not connect to Serial Port. Error Message: " + ex.Message, ErrorCodes.HAL_SERIAL_ERROR_CREATING_CONNECTION);
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
                m_SerialPort.DataReceived -= new SerialDataReceivedEventHandler(SerialPort_DataReceived);
                m_SerialPort.ErrorReceived -= new SerialErrorReceivedEventHandler(SerialPort_ErrorReceived);

                m_SerialPort.Close();
            }
            catch (Exception ex)
            {
                throw new ConnectionException("Could not close Serial Port. Error Message: " + ex.Message, ErrorCodes.HAL_SERIAL_ERROR_CLOSING_CONNECTION);
            }
        }

        public void DetachEvents()
        {
            m_SerialPort.DataReceived -= new SerialDataReceivedEventHandler(SerialPort_DataReceived);
            m_SerialPort.ErrorReceived -= new SerialErrorReceivedEventHandler(SerialPort_ErrorReceived);
        }

        /// <summary>
        /// Sends the command.
        /// </summary>
        /// <param name="command">The command.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        public void SendCommand(string command)
        {
            try
            {
                byte[] data = Encoding.ASCII.GetBytes(command);

                if (m_SerialPort.IsOpen)
                {
                    System.Threading.Thread.Sleep(m_CommandTimeout);
                    m_SerialPort.Write(data, 0, data.Length);
                    //System.Diagnostics.Debug.WriteLine("Sending Command: " + command);
                }
            }
            catch (Exception ex)
            {
                OnSerialDataErrorEvent(ErrorCodes.HAL_SERIAL_ERROR_SEND_COMMAND, ex.Message);
            }
        }
        #endregion Public Functions

        #region Private Functions
        /// <summary>
        /// Initializes the serial port.
        /// </summary>
        /// <exception cref="ArgumentException">Thrown when the <paramref name="m_PortNumber"/> is invalid or the parity and databits combination is unsupported.</exception>
        /// <exception cref="InvalidOperationException">Thrown when the <paramref name="m_PortNumber"/> was already open (in use).</exception>
        /// <exception cref="IOException">Thrown when the port is in an invalid state or when parameters passed to the serial port connection were invalid.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when data read or written to the port failed.</exception>
        private void InitializeSerialPort()
        {
            m_SerialPort.PortName = m_PortNumber;
            m_SerialPort.BaudRate = m_BaudRate;
            m_SerialPort.Parity = m_Parity;

            if (m_Parity != Parity.None)
            {
                if (m_DataBits + 1 > 8)
                {
                    // Not supported by microsoft...
                    throw new ArgumentException("Rs232 failure. Databits cannot be 8 unless Parity is None");
                }

                m_SerialPort.DataBits = m_DataBits + 1;
            }
            else
            {
                m_SerialPort.DataBits = m_DataBits;
            }

            if (m_StopBits != StopBits.None)
            {
                m_SerialPort.StopBits = m_StopBits;
            }

            m_SerialPort.Handshake = m_Handshake;

            m_SerialPort.DtrEnable = true;

            m_SerialPort.RtsEnable = m_RtsEnable;
            m_SerialPort.ReadTimeout = m_ReadTimeout;
            m_SerialPort.WriteTimeout = m_WriteTimeout;
            m_SerialPort.ReadBufferSize = m_ReadBufferSize;
            m_SerialPort.WriteBufferSize = m_WriteBufferSize;

            m_SerialPort.ErrorReceived -= new SerialErrorReceivedEventHandler(SerialPort_ErrorReceived);
            m_SerialPort.ErrorReceived += new SerialErrorReceivedEventHandler(SerialPort_ErrorReceived);

            m_SerialPort.DataReceived -= new SerialDataReceivedEventHandler(SerialPort_DataReceived);
            m_SerialPort.DataReceived += new SerialDataReceivedEventHandler(SerialPort_DataReceived);
        }
        #endregion Private Functions
    }
}
