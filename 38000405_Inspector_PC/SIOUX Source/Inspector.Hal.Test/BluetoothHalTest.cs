/*
//===============================================================================
// Copyright Kamstrup
// All rights reserved.
//===============================================================================
*/

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using Inspector.Hal.Interfaces.Events;
using Inspector.Infra;
using Inspector.Model.BluetoothDongle;
using NUnit.Framework;

namespace Inspector.Hal.Test
{
    /// <summary>
    /// BluetoothHalTest
    /// Tests in this file are on [Ignore] as they cannot be executed on the buildserver.
    /// All these tests assume the baBlueSoleil bluetooth dongle being connected to the PC.
    /// All these tests assume the manometer device with bluetooth address BLUETOOTH_INCORRECT_ADDRESS being on.
    /// </summary>
    [TestFixture]
    public class BluetoothHalTest
    {
        #region Constants
        private const string BLUETOOTH_CORRECT_ADDRESS = "(00:80:98:C4:21:13)";
        private const string BLUETOOTH_INCORRECT_ADDRESS = "(00:80:98:C4:21:A4)";
        private const string BLUETOOTH_API_BLUE_SOLEIL = "baBlueSoleil";
        private const string BLUETOOTH_API_TOSHIBA = "baToshiba";
        private const string BLUETOOTH_API_UNKNOWN = "UnknownApi";
        private const int BLUETOOTH_SHORT_TIMEOUT = 5000;
        private const int BLUETOOTH_LONG_TIMEOUT = 10000;
        private const int BLUETOOTH_INSANE_TIMEOUT = 15000;
        #endregion Constants

        #region Class Members
        private BluetoothHal m_BluetootHal;
        private string m_Message;
        private int m_ErrorCode;
        private string m_Data;
        private ManualResetEvent m_ManualResetEvent = new ManualResetEvent(false);
        private List<BluetoothDongleInformation> m_AllowedBluetoothDongles = new List<BluetoothDongleInformation>();
        #endregion Class Members

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="BluetoothHalTest"/> class.
        /// </summary>
        public BluetoothHalTest()
        {

        }
        #endregion Constructors

        /// <summary>
        /// Sets up test.
        /// </summary>
        //[SetUp]
        public void SetUpTest()
        {
            m_BluetootHal = new BluetoothHal();

            BluetoothDongleInformation btItem = new BluetoothDongleInformation("00:80:98:C4:21:13");
            m_AllowedBluetoothDongles.Add(btItem);
        }

        /// <summary>
        /// Tears down test.
        /// </summary>
        //[TearDown]
        public void TearDownTest()
        {
            m_BluetootHal.Disconnect();

            m_BluetootHal.Dispose();
        }

        #region Events
        /// <summary>
        /// Handles the MessageReceived event of the m_BluetootHal control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void BluetootHal_MessageReceived(object sender, EventArgs e)
        {
            MessageReceivedEventArgs messageReceivedEventArgs = e as MessageReceivedEventArgs;
            m_Data = messageReceivedEventArgs.Data;
            m_ManualResetEvent.Set();
        }

        /// <summary>
        /// Handles the ConnectFailed event of the m_BluetootHal control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void BluetootHal_ConnectFailed(object sender, EventArgs e)
        {
            ConnectFailedEventArgs connectFailedEventArgs = e as ConnectFailedEventArgs;
            m_Message = connectFailedEventArgs.Message;
            m_ErrorCode = connectFailedEventArgs.ErrorCode;
            m_ManualResetEvent.Set();
        }
        #endregion Events

        [Ignore("Can only be executed locally as a BT device is needed.")]
        public void TestMeasurement()
        {
            bool connected = false;

            m_BluetootHal = new BluetoothHal();
            BluetoothDongleInformation btItem = new BluetoothDongleInformation("(00:1B:DC:0F:4D:DF)");
            m_AllowedBluetoothDongles.Add(btItem);
            //btItem = new BluetoothDongleInformation("00:80:98:C4:21:13");
            //m_AllowedBluetoothDongles.Add(btItem);

            m_BluetootHal.Connected += delegate
            {
                connected = true;
                m_ManualResetEvent.Set();
            };

            m_BluetootHal.Disconnected += delegate
            {
                connected = false;
                m_ManualResetEvent.Set();
            };

            try
            {
                m_BluetootHal.MeasurementsReceived += new EventHandler(m_BluetootHal_MeasurementsReceived);

                m_ManualResetEvent.Reset();
                m_BluetootHal.Connect(CreateProperties(BLUETOOTH_API_BLUE_SOLEIL, BLUETOOTH_CORRECT_ADDRESS), m_AllowedBluetoothDongles);
                m_ManualResetEvent.WaitOne(BLUETOOTH_SHORT_TIMEOUT);

                Assert.IsTrue(connected, "Bluetooth should be connected");

                m_ManualResetEvent.Reset();

                System.Diagnostics.Debug.WriteLine("turn 'em off");
                System.Threading.Thread.Sleep(5000);

                m_BluetootHal.StartContinuousMeasurement(1);
                m_BluetootHal.StartContinuousMeasurement(1);
                m_BluetootHal.StartContinuousMeasurement(1);
                m_BluetootHal.StartContinuousMeasurement(1);
                m_BluetootHal.StartContinuousMeasurement(1);
                m_BluetootHal.StartContinuousMeasurement(1);
                m_ManualResetEvent.WaitOne(BLUETOOTH_INSANE_TIMEOUT);
                m_BluetootHal.StopContinuousMeasurement();
                
                Thread.Sleep(1000);
            }
            finally
            {
                m_ManualResetEvent.Reset();
                m_BluetootHal.Disconnect();
                m_ManualResetEvent.WaitOne(BLUETOOTH_SHORT_TIMEOUT);
            }

            Assert.IsFalse(connected, "Bluetooth should be disconnected");
        }

        void m_BluetootHal_MeasurementsReceived(object sender, EventArgs e)
        {
            MeasurementsReceivedEventArgs eventArgs = e as MeasurementsReceivedEventArgs;
            Assert.Greater(eventArgs.Measurements.Count, 0);
            System.Diagnostics.Debug.WriteLine("Nr of measurements: " + eventArgs.Measurements.Count);
            System.Diagnostics.Debug.WriteLine("First value: " + eventArgs.Measurements[0].ToString());
            //m_ManualResetEvent.Set();
        }

        /// <summary>
        /// Tests the checksum.
        /// </summary>
        [Ignore("Can only be executed locally as a BT device is needed.")]
        public void TestChecksum()
        {
            int checksum = BluetoothHal.ComputeChecksum("SYST:Date?\t*");
            Assert.AreEqual(125, checksum);

            checksum = BluetoothHal.ComputeChecksum("\t*");
            Assert.AreEqual(51, checksum);

            checksum = BluetoothHal.ComputeChecksum("SYST:IDEN?\t*");
            Assert.AreEqual(31, checksum);
        }

        /// <summary>
        /// Tests the correct connection behavior.
        /// </summary>
        [Ignore("Can only be executed locally as a BT device is needed.")]
        public void TestCorrectConnectionBehavior()
        {
            bool connected = false;

            m_BluetootHal.Connected += delegate
            {
                connected = true;
                m_ManualResetEvent.Set();
            };

            m_BluetootHal.Disconnected += delegate
            {
                connected = false;
                m_ManualResetEvent.Set();
            };

            Assert.IsFalse(m_BluetootHal.BluetoothActive, "Bluetooth connection was initially active.");
            Assert.IsFalse(m_BluetootHal.SerialPortOpen, "Serial Connection was initially active.");

            m_ManualResetEvent.Reset();
            m_BluetootHal.Connect(CreateProperties(BLUETOOTH_API_BLUE_SOLEIL, BLUETOOTH_CORRECT_ADDRESS), m_AllowedBluetoothDongles);
            m_ManualResetEvent.WaitOne(BLUETOOTH_SHORT_TIMEOUT);

            Assert.IsTrue(connected, "Bluetooth should be connected");
            Assert.IsTrue(m_BluetootHal.BluetoothActive, "Bluetooth connection was inactive.");
            Assert.IsTrue(m_BluetootHal.SerialPortOpen, "Serial Connection was inactive.");

            m_ManualResetEvent.Reset();
            m_BluetootHal.Disconnect();
            m_ManualResetEvent.WaitOne(BLUETOOTH_SHORT_TIMEOUT);

            Assert.IsFalse(connected, "Bluetooth should be disconnected");
            Assert.IsFalse(m_BluetootHal.BluetoothActive, "Bluetooth connection was active");
            Assert.IsFalse(m_BluetootHal.SerialPortOpen, "Serial Connection was  active.");
        }

        /// <summary>
        /// Tests the unsupported radio.
        /// </summary>
        [Ignore("Can only be executed locally as a BT device is needed.")]
        public void TestUnsupportedRadio()
        {
            m_BluetootHal.ConnectFailed += new EventHandler(BluetootHal_ConnectFailed);

            m_ManualResetEvent.Reset();
            m_BluetootHal.Connect(CreateProperties(BLUETOOTH_API_UNKNOWN, BLUETOOTH_CORRECT_ADDRESS), m_AllowedBluetoothDongles);
            m_ManualResetEvent.WaitOne(BLUETOOTH_SHORT_TIMEOUT);

            Assert.AreEqual("Could not create Bluetooth Radio for API: 'UnknownApi'. API not supported", m_Message, "Unexpected error message");
            Assert.AreEqual(1004, m_ErrorCode, "Unexpected error code");
            Assert.IsFalse(m_BluetootHal.BluetoothActive, "Bluetooth connection was active");
        }

        /// <summary>
        /// Tests the unknown bluetooth address.
        /// </summary>
        [Ignore("Can only be executed locally as a BT device is needed.")]
        public void TestUnknownBluetoothAddress()
        {
            m_BluetootHal.ConnectFailed += new EventHandler(BluetootHal_ConnectFailed);

            m_ManualResetEvent.Reset();
            m_BluetootHal.Connect(CreateProperties(BLUETOOTH_API_BLUE_SOLEIL, BLUETOOTH_INCORRECT_ADDRESS), m_AllowedBluetoothDongles);
            m_ManualResetEvent.WaitOne(BLUETOOTH_SHORT_TIMEOUT);

            Assert.AreEqual("Could not create a Bluetooth Connection. Error code: '44'. Error Message: 'Specified service was not found.'", m_Message, "Unexpected error message");
            Assert.AreEqual(44, m_ErrorCode, "Unexpected error code");
            Assert.IsFalse(m_BluetootHal.BluetoothActive, "Bluetooth connection was active");
        }

        /// <summary>
        /// Tests the incorrect bluetooth radio.
        /// </summary>
        [Ignore("Can only be executed locally as a BT device is needed.")]
        public void TestIncorrectBluetoothRadio()
        {
            m_BluetootHal.ConnectFailed += new EventHandler(BluetootHal_ConnectFailed);

            m_ManualResetEvent.Reset();
            m_BluetootHal.Connect(CreateProperties(BLUETOOTH_API_TOSHIBA, BLUETOOTH_CORRECT_ADDRESS), m_AllowedBluetoothDongles);
            m_ManualResetEvent.WaitOne(BLUETOOTH_SHORT_TIMEOUT);

            Assert.AreEqual("Could not create a Bluetooth Connection. Error code: '14'. Error Message: 'Specified bluetooth API is not available.'", m_Message, "Unexpected error message");
            Assert.AreEqual(14, m_ErrorCode, "Unexpected error code");
            Assert.IsFalse(m_BluetootHal.BluetoothActive, "Bluetooth connection was active");
        }

        /// <summary>
        /// Tests the connect while connected.
        /// </summary>
        [Ignore("Can only be executed locally as a BT device is needed.")]
        public void TestConnectWhileConnected()
        {
            m_BluetootHal.ConnectFailed += new EventHandler(BluetootHal_ConnectFailed);

            m_ManualResetEvent.Reset();
            m_BluetootHal.Connect(CreateProperties(BLUETOOTH_API_BLUE_SOLEIL, BLUETOOTH_CORRECT_ADDRESS), m_AllowedBluetoothDongles);
            m_ManualResetEvent.WaitOne(BLUETOOTH_SHORT_TIMEOUT);

            m_ManualResetEvent.Reset();
            m_BluetootHal.Connect(CreateProperties(BLUETOOTH_API_BLUE_SOLEIL, BLUETOOTH_CORRECT_ADDRESS), m_AllowedBluetoothDongles);
            m_ManualResetEvent.WaitOne(BLUETOOTH_SHORT_TIMEOUT);

            Assert.AreEqual("Bluetooth Connection already active", m_Message, "Unexpected error message");
            Assert.AreEqual(1003, m_ErrorCode, "Unexpected error code");
        }

        /// <summary>
        /// Tests the send command.
        /// </summary>
        [Ignore("Can only be executed locally as a BT device is needed.")]
        public void TestSendCommand()
        {
            bool connectSucceeded = false;
            m_BluetootHal.Connected += delegate
            {
                connectSucceeded = true;
                m_ManualResetEvent.Set();
            };

            m_BluetootHal.ConnectFailed += delegate
            {
                connectSucceeded = false;
                m_ManualResetEvent.Set();
            };

            m_BluetootHal.MessageReceived += new EventHandler(BluetootHal_MessageReceived);

            m_ManualResetEvent.Reset();
            m_BluetootHal.Connect(CreateProperties(BLUETOOTH_API_BLUE_SOLEIL, BLUETOOTH_CORRECT_ADDRESS), m_AllowedBluetoothDongles);
            m_ManualResetEvent.WaitOne(BLUETOOTH_SHORT_TIMEOUT);

            Assert.IsTrue(connectSucceeded, "Connecting should have succeeded");

            m_ManualResetEvent.Reset();
            m_BluetootHal.SendCommand(DeviceCommand.CheckIdentification);
            m_ManualResetEvent.WaitOne(BLUETOOTH_LONG_TIMEOUT);

            Assert.AreEqual("\"HM3500DLM110,MOD00B,B030504\"", m_Data, "Expected identification from device");
        }

        /// <summary>
        /// Creates the properties.
        /// </summary>
        /// <param name="api">The API.</param>
        /// <param name="address">The address.</param>
        /// <returns></returns>
        private Dictionary<string, string> CreateProperties(string api, string address)
        {
            Dictionary<string, string> properties = new Dictionary<string, string>();
            properties.Add("bluetoothApi", api);
            properties.Add("destinationAddress", address);
            return properties;
        }

        /// <summary>
        /// Regtests this instance.
        /// </summary>
        //[Ignore("Obsolete")] // Regular expression test.
        [Test]
        public void RegexManometerDataTest()
        {
            // data\t\r
            // data\t*12\r

            string input = "\t*51\r";
            //string input = "data\t\r";
            //string input = "data\t*12\r";
            input = "-1\t*239\r0A\n";
            input = "-0.9\t*24\r";
            // Regex validates following inputformats that can be received from the device
            // Remark: this is not fully idiot proof.

            string regexStr = "^(?<Data>.*?)[\t|\r]{1,2}(\\*(?<Checksum>[^\r]+)\r)?(?<io>[0-9A-Z]{0,2}?)[\r|\n]?$";
            Regex r = new Regex(regexStr, RegexOptions.Compiled);
            System.Diagnostics.Debug.WriteLine("Ismatch {0}", r.IsMatch(input));
            System.Diagnostics.Debug.WriteLine("Data: {0}", Regex.Match(input, regexStr).Groups["Data"]);
            System.Diagnostics.Debug.WriteLine("Checksum: {0}", Regex.Match(input, regexStr).Groups["Checksum"]);
            System.Diagnostics.Debug.WriteLine("io: {0}", Regex.Match(input, regexStr).Groups["io"]);
        }

        //tests the new io data in the measurement data
        [Test]
        public void TestRegexForParsing()
        {
            var regexStr = "^(?<Data>.*?)[\t|\r]{1,2}(\\*(?<Checksum>[^\r]+)\r)(?<io>[0-9]{0,2}?)[\r|\n]?$";

            var dataReceived = "er-001\t*200\r03\n";
            var m = Regex.Match(dataReceived, regexStr, RegexOptions.Compiled);

            Assert.IsTrue(m.Success);
            Assert.AreEqual("er-001", m.Groups["Data"].ToString());
            Assert.AreEqual("200", m.Groups["Checksum"].ToString());
            Assert.AreEqual("03", m.Groups["io"].ToString());
            dataReceived = "er-001\t*200\r";
            m = Regex.Match(dataReceived, regexStr, RegexOptions.Compiled);
            Assert.AreEqual("er-001", m.Groups["Data"].ToString());
            Assert.AreEqual("200", m.Groups["Checksum"].ToString());
            Assert.IsTrue(m.Success);
            dataReceived = "-1.0\t*239\r01\r";
            m = Regex.Match(dataReceived, regexStr, RegexOptions.Compiled);
            Assert.AreEqual("-1.0", m.Groups["Data"].ToString());
            Assert.AreEqual("239", m.Groups["Checksum"].ToString());
            Assert.AreEqual("01", m.Groups["io"].ToString());
            Assert.IsTrue(m.Success);
        }

        /// <summary>
        /// Regtests this instance.
        /// </summary>
        [Ignore("Obsolete")] // Regular expression test.
        public void RegexRemoteCommandTest()
        {
            // \r\ndata\r\n

            string input = "\r\ndata\r\n";

            // Regex validates following inputformats that can be received from the device
            // Remark: this is not fully idiot proof.

            string regexStr = "^\r\n(?<Data>.+?)\r\n$";
            Regex r = new Regex(regexStr, RegexOptions.Compiled);
            System.Diagnostics.Debug.WriteLine("Ismatch {0}", r.IsMatch(input));
            System.Diagnostics.Debug.WriteLine("Data: {0}", Regex.Match(input, regexStr).Groups["Data"]);
        }
    }
}
