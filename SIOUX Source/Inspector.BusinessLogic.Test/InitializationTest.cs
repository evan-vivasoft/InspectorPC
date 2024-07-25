/*
//===============================================================================
// Copyright Kamstrup
// All rights reserved.
//===============================================================================
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Inspector.BusinessLogic.Interfaces;
using Inspector.BusinessLogic.Interfaces.Events;
using Inspector.Hal.Interfaces;
using Inspector.Hal.Stub;
using Inspector.Infra;
using Inspector.Infra.Ioc;
using Inspector.Model;
using NUnit.Framework;

namespace Inspector.BusinessLogic.Test
{
    /// <summary>
    /// 
    /// </summary>
    [TestFixture]
    public class InitializationTest
    {
        #region Class Members
        private const int EventTimeout = 10000;
        private IInitializationActivityControl m_InitializationActivityControl;
        private readonly ManualResetEvent m_ManualResetEvent = new ManualResetEvent(false);

        private InitializationResult m_FinishResult;
        private int m_FinishErrorCode = -1;
        private InitializationStepResult m_ExpectedStepResult;
        private int m_ExpectedStepErrorCode;
        private string m_ExpectedStepMessage;
        private InitializationManometer m_ExpectedStepManometer;

        private bool m_HasErrorBeenSet;
        private int m_Counter = 1;
        #endregion Class Members

        #region Properties
        /// <summary>
        /// Gets or sets the initialization activity control.
        /// </summary>
        /// <value>
        /// The initialization activity control.
        /// </value>
        private IInitializationActivityControl InitializationActivityControl
        {
            get
            {
                return m_InitializationActivityControl ?? (m_InitializationActivityControl = ContextRegistry.Context.Resolve<IInitializationActivityControl>());
            }
        }

        /// <summary>
        /// Gets or sets the hal
        /// </summary>
        /// <value>
        /// The communication control.
        /// </value>
        private static IHal Hal
        {
            get
            {
                return ContextRegistry.Context.Resolve<IHal>();
            }
        }
        #endregion Properties

        [SetUp]
        public void SetUpSingleTest()
        {
            var path = Path.GetDirectoryName(typeof(InitializationTest).Assembly.Location);
            Assert.IsNotNull(path);
            Directory.SetCurrentDirectory(path);

            m_HasErrorBeenSet = false;
            m_Counter = 1;
            m_ExpectedStepResult = InitializationStepResult.UNSET;
            m_ExpectedStepErrorCode = -1;
            m_ExpectedStepMessage = string.Empty;
            m_ExpectedStepManometer = InitializationManometer.BLUETOOTH_MODULE;
        }

        [TestFixtureTearDown]
        public void TestTearDown()
        {
        }

        #region Event handlers / generic functions
        /// <summary>
        /// Handles the InitializationFinished event of the InitializationActivityControl control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void InitializationActivityControl_InitializationFinished(object sender, EventArgs e)
        {
            Assert.IsInstanceOf<FinishInitializationEventArgs>(e, "Expected FinishInitializationEventArgs");
            
            var eventArgs = e as FinishInitializationEventArgs;

            Assert.IsNotNull(eventArgs);

            m_FinishResult = eventArgs.Result;
            m_FinishErrorCode = eventArgs.ErrorCode;
            
            m_ManualResetEvent.Set();
        }

        /// <summary>
        /// Sets the last step result.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <param name="commandId">The command id.</param>
        private void SetFirstStepResult(EventArgs e, string commandId)
        {
            Assert.IsInstanceOf<FinishInitializationStepEventArgs>(e, "Expected FinishInitializationStepEventArgs");

            var eventArgs = e as FinishInitializationStepEventArgs;

            Assert.IsNotNull(eventArgs);

            if (eventArgs.StepId != commandId) return;

            if (m_HasErrorBeenSet) return; // Make sure only the first occurrence of the command Id is stored.
                
            m_HasErrorBeenSet = true;

            m_ExpectedStepErrorCode = eventArgs.ErrorCode;
            m_ExpectedStepManometer = eventArgs.Manometer;
            m_ExpectedStepMessage = eventArgs.Message;
            m_ExpectedStepResult = eventArgs.Result;
        }
        #endregion Event handlers


        /// <summary>
        /// Initializations the happy flow test.
        /// </summary>        
        [TestCase(DeviceType.PlexorBluetoothWIS)]
        [TestCase(DeviceType.PlexorBluetoothIrDA)]
        public void InitializationHappyFlowTest(DeviceType deviceType)
        {
            Console.WriteLine(System.Reflection.MethodBase.GetCurrentMethod().Name);
            SetUpHappyFlowStack(deviceType);

            InitializationActivityControl.InitializationFinished += InitializationActivityControl_InitializationFinished;

            m_ManualResetEvent.Reset();
            
            InitializationActivityControl.ExecuteInitialization();

            Assert.IsTrue(m_ManualResetEvent.WaitOne(EventTimeout));

            InitializationActivityControl.InitializationFinished -= InitializationActivityControl_InitializationFinished;

            Assert.AreEqual(InitializationResult.SUCCESS, m_FinishResult);
            Assert.AreEqual(ErrorCodes.INITIALIZATION_FINISHED_SUCCESSFULLY, m_FinishErrorCode);
        }

        /// <summary>
        /// Sets the happy flow stack.
        /// </summary>
        private static void SetUpHappyFlowStack(DeviceType deviceType)
        {
            var reactionStack = new Stack<string>();

            // Disconnect
            if (deviceType == DeviceType.PlexorBluetoothIrDA)
            {
                reactionStack.Push("er-001");                       // Flush manometer cache
                reactionStack.Push("CONNECT");                      // Exit local mode
            }

            reactionStack.Push("ok");                               // Switch to manometer TH2

            if (deviceType == DeviceType.PlexorBluetoothIrDA)
            {
                reactionStack.Push("ERROR 05");                     // Flush bluetooth cache
                reactionStack.Push("OK");                           // Enter local mode
            }

            if (deviceType == DeviceType.PlexorBluetoothWIS)
            {
                reactionStack.Push("LOCAL");                        // Enter local mode

                reactionStack.Push("CONNECT");                      // Exit local mode
                reactionStack.Push("OK");                           // Enbale/Disable IO status
                reactionStack.Push("1.0.0");                        // Check system software version
                reactionStack.Push("20150624");                     // Check calibration date
                reactionStack.Push("LOCAL");                        // Enter local mode
            }

            reactionStack.Push("ok");                               // Set IRDA always on 
            reactionStack.Push("mbar");                             // Query pressure unit on TH1
            reactionStack.Push("ok");                               // Set Manometer Range on TH1
            reactionStack.Push("\"2000 mbar\"");                    // Query Manometer Range on TH1
            reactionStack.Push("\"HM3500DLM110,MOD00B,B030504\"");  // Identification on TH1
            reactionStack.Push(string.Empty);                       // Initiate Self test on TH1
            reactionStack.Push("0,\"no current Scpi errors!\"");    // Check SCPI on TH1

            reactionStack.Push("80");                               // Check battery on TH1
            reactionStack.Push("OK");                               // Check Manometer present ('OK' is Hardcoded).

            if (deviceType == DeviceType.PlexorBluetoothIrDA)
            {
                reactionStack.Push("er-001");                       // Flush manometer cache
                reactionStack.Push("CONNECT");                      // Exit local mode
            }

            reactionStack.Push("ok");                               // Switch to manometer TH1

            if (deviceType == DeviceType.PlexorBluetoothIrDA)
            {
                reactionStack.Push("ERROR 05");                     // Flush bluetooth cache
                reactionStack.Push("OK");                           // Enter local mode
            }

            if (deviceType == DeviceType.PlexorBluetoothWIS)
            {
                reactionStack.Push("LOCAL");                        // Enter local mode

                reactionStack.Push("CONNECT");                      // Exit local mode
                reactionStack.Push("OK");                           // Enbale/Disable IO status
                reactionStack.Push("1.0.0");                        // Check system software version
                reactionStack.Push("20150624");                     // Check calibration date
                reactionStack.Push("LOCAL");                        // Enter local mode
            }

            reactionStack.Push("ok");                               // Set IRDA always on 
            reactionStack.Push("mbar");                             // Query pressure unit on TH2
            reactionStack.Push("ok");                               // Set Manometer Range on TH2
            reactionStack.Push("\"2000 mbar\"");                    // Query Manometer Range on TH2
            reactionStack.Push("\"HM3500DLM110,MOD00B,B030504\"");  // Identification on TH2
            reactionStack.Push(string.Empty);                       // Initiate Self test on TH2
            reactionStack.Push("0,\"no current Scpi errors!\"");    // Check SCPI on TH2

            reactionStack.Push("80");                               // Check battery on TH2
            reactionStack.Push("OK");                               // Check Manometer present ('OK' is Hardcoded).

            if (deviceType == DeviceType.PlexorBluetoothIrDA)
            {
                reactionStack.Push("er-001");                       // Flush manometer cache
                reactionStack.Push("CONNECT");                      // Exit local mode
            }

            reactionStack.Push("ok");                               // Switch to manometer TH2

            if (deviceType == DeviceType.PlexorBluetoothIrDA)
            {
                reactionStack.Push("ERROR 05");                     // Flush bluetooth cache
                reactionStack.Push("OK");                           // Enter local mode
            }

            if (deviceType == DeviceType.PlexorBluetoothWIS)
            {
                reactionStack.Push("LOCAL");                        // Enter local mode
            }
            if (deviceType == DeviceType.PlexorBluetoothWIS)
            {
                //add SSD init responses
                reactionStack.Push("CONNECT");                      // Exit local mode
                reactionStack.Push("OK");                           // Activate Port IrDA
                reactionStack.Push("OK");                           // Enter local mode
                reactionStack.Push("SSD Sensor,37000303,35000101"); // SENS:IDEN ?
                reactionStack.Push("OK");                           // Stop sensor run
                reactionStack.Push("CONNECT");                      // Exit local mode
                reactionStack.Push("OK");                           // Activate port sensor
                reactionStack.Push("OK");                           // Enable io status
                reactionStack.Push("ON");                           // Check Io3 status
                reactionStack.Push("OK");                           // Enter local mode
            }

            var stub = Hal as BluetoothHalSequentialStub;

            Assert.IsNotNull(stub);
            
            stub.DeviceType = deviceType;
            stub.SetUpReactionStack(reactionStack);
        }

        /// <summary>
        /// Initializations the enter remote local command mode error test.
        /// </summary>
        //[TestCase(DeviceType.PlexorBluetoothWIS)]
        [TestCase(DeviceType.PlexorBluetoothIrDA)]
        public void InitializationEnterRemoteLocalCommandModeErrorTest(DeviceType deviceType)
        {
            Console.WriteLine(System.Reflection.MethodBase.GetCurrentMethod().Name);
            SetUpEnterRemoteErrorStack(deviceType);

            InitializationActivityControl.InitializationFinished += InitializationActivityControl_InitializationFinished;
            InitializationActivityControl.InitializationStepFinished += InitializationActivityControl_InitializationStepFinished_EnterRemoteLocal;

            m_ManualResetEvent.Reset();

            InitializationActivityControl.ExecuteInitialization();
            
            Assert.IsTrue(m_ManualResetEvent.WaitOne(EventTimeout));

            InitializationActivityControl.InitializationStepFinished -= InitializationActivityControl_InitializationStepFinished_EnterRemoteLocal;
            InitializationActivityControl.InitializationFinished -= InitializationActivityControl_InitializationFinished;

            Assert.AreEqual(InitializationResult.ERROR, m_FinishResult);
            Assert.AreEqual(ErrorCodes.INITIALIZATION_FINISHED_ERROR, m_FinishErrorCode);

            Assert.AreEqual(ErrorCodes.INITIALIZATION_STEP_ENTER_REMOTE_ERROR, m_ExpectedStepErrorCode);
            Assert.AreEqual(InitializationManometer.BLUETOOTH_MODULE, m_ExpectedStepManometer);
            Assert.AreEqual("nok", m_ExpectedStepMessage);
            Assert.AreEqual(InitializationStepResult.ERROR, m_ExpectedStepResult);
        }

        private void InitializationActivityControl_InitializationStepFinished_EnterRemoteLocal(object sender, EventArgs e)
        {
            SetFirstStepResult(e, DeviceCommand.EnterRemoteLocalCommandMode.ToString());
        }

        /// <summary>
        /// Sets the enter remote error stack.
        /// </summary>
        private static void SetUpEnterRemoteErrorStack(DeviceType deviceType)
        {
            var reactionStack = new Stack<string>();

            // Disconnect
            reactionStack.Push("nok");                              // Enter local mode
                                                                    // Connect

            if (deviceType == DeviceType.PlexorBluetoothWIS)
            {
                //add SSD init responses
                reactionStack.Push("CONNECT");                      // Exit local mode
                reactionStack.Push("OK");                           // Activate Port IrDA
                reactionStack.Push("OK");                           // Enter local mode
                reactionStack.Push("SSD Sensor,37000303,35000101"); // SENS:IDEN ?
                reactionStack.Push("OK");                           // Stop sensor run
                reactionStack.Push("CONNECT");                      // Exit local mode
                reactionStack.Push("OK");                           // Activate port sensor
                reactionStack.Push("OK");                           // Enable io status
                reactionStack.Push("ON");                           // Check Io3 status
                reactionStack.Push("OK");                           // Enter local mode
            }

            var stub = Hal as BluetoothHalSequentialStub;

            Assert.IsNotNull(stub);

            stub.DeviceType = deviceType;
            stub.SetUpReactionStack(reactionStack);
        }

        /// <summary>
        /// Initializations the exit remote local command mode error test.
        /// </summary>
       // [TestCase(DeviceType.PlexorBluetoothWIS)]
        [TestCase(DeviceType.PlexorBluetoothIrDA)]
        public void InitializationExitRemoteLocalCommandModeErrorTest(DeviceType deviceType)
        {
            Console.WriteLine(System.Reflection.MethodBase.GetCurrentMethod().Name);
            SetUpExitRemoteErrorStack(deviceType);

            InitializationActivityControl.InitializationFinished += InitializationActivityControl_InitializationFinished;
            InitializationActivityControl.InitializationStepFinished += InitializationActivityControl_InitializationStepFinished_ExitRemoteLocal;

            m_ManualResetEvent.Reset();
            
            InitializationActivityControl.ExecuteInitialization();

            Assert.IsTrue(m_ManualResetEvent.WaitOne(EventTimeout));

            InitializationActivityControl.InitializationStepFinished -= InitializationActivityControl_InitializationStepFinished_ExitRemoteLocal;
            InitializationActivityControl.InitializationFinished -= InitializationActivityControl_InitializationFinished;

            if (deviceType == DeviceType.PlexorBluetoothIrDA)
            {
                Assert.AreEqual(InitializationResult.SUCCESS, m_FinishResult);
                Assert.AreEqual(ErrorCodes.INITIALIZATION_FINISHED_SUCCESSFULLY, m_FinishErrorCode);

                Assert.AreEqual(ErrorCodes.INITIALIZATION_STEP_EXECUTED_SUCCESSFULLY, m_ExpectedStepErrorCode);
                Assert.AreEqual("CONNECT", m_ExpectedStepMessage);
                Assert.AreEqual(InitializationStepResult.SUCCESS, m_ExpectedStepResult);
            }

            if (deviceType == DeviceType.PlexorBluetoothWIS)
            {
                Assert.AreEqual(InitializationResult.ERROR, m_FinishResult);
                Assert.AreEqual(ErrorCodes.INITIALIZATION_FINISHED_ERROR, m_FinishErrorCode);

                Assert.AreEqual(ErrorCodes.INITIALIZATION_STEP_EXIT_REMOTE_ERROR, m_ExpectedStepErrorCode);
                Assert.AreEqual("ERROR nn", m_ExpectedStepMessage);
                Assert.AreEqual(InitializationStepResult.ERROR, m_ExpectedStepResult);
            }
        }

        private void InitializationActivityControl_InitializationStepFinished_ExitRemoteLocal(object sender, EventArgs e)
        {
            SetFirstStepResult(e, DeviceCommand.ExitRemoteLocalCommandMode.ToString());
        }

        /// <summary>
        /// Sets the exit remote error stack.
        /// </summary>
        private static void SetUpExitRemoteErrorStack(DeviceType deviceType)
        {
            var reactionStack = new Stack<string>();

            // Disconnect
            if (deviceType == DeviceType.PlexorBluetoothIrDA)
            {
                reactionStack.Push("er-001");                       // Flush manometer cache
                reactionStack.Push("CONNECT");                      // Exit local mode
            }

            reactionStack.Push("ok");                               // Switch to manometer TH2

            if (deviceType == DeviceType.PlexorBluetoothIrDA)
            {
                reactionStack.Push("ERROR 05");                     // Flush bluetooth cache
                reactionStack.Push("OK");                           // Enter local mode

            reactionStack.Push("ok");                               // Set IRDA always on 
            reactionStack.Push("mbar");                             // Query pressure unit on TH1
            reactionStack.Push("ok");                               // Set Manometer Range on TH1
            reactionStack.Push("\"17 bar\"");                       // Query Manometer Range on TH1
            reactionStack.Push("\"HM3500DLM110,MOD00B,B030504\"");  // Identification on TH1
            reactionStack.Push(string.Empty);                       // Initiate Self test on TH1
            reactionStack.Push("0,\"no current Scpi errors!\"");    // Check SCPI on TH1

            reactionStack.Push("80");                               // Check battery on TH1
            reactionStack.Push("OK");                               // Check Manometer present ('OK' is Hardcoded).

                reactionStack.Push("er-001");                       // Flush manometer cache
                reactionStack.Push("CONNECT");                      // Exit local mode

                reactionStack.Push("ok");                               // Switch to manometer TH1

            }

            if (deviceType == DeviceType.PlexorBluetoothIrDA)
            {
                reactionStack.Push("ERROR 05");                     // Flush bluetooth cache
                reactionStack.Push("OK");                           // Enter local mode
            }

            if (deviceType == DeviceType.PlexorBluetoothWIS)
            {
                reactionStack.Push("ERROR nn");                     // Exit local mode
                reactionStack.Push("OK");                           // Enbale/Disable IO status
                reactionStack.Push("1.0.0");                        // Check system software version
                reactionStack.Push("20150624");                     // Check calibration date
                reactionStack.Push("LOCAL");                        // Enter local mode
            }

            reactionStack.Push("ok");                               // Set IRDA always on 
            reactionStack.Push("mbar");                             // Query pressure unit on TH2
            reactionStack.Push("ok");                               // Set Manometer Range on TH2
            reactionStack.Push("\"2000 mbar\"");                    // Query Manometer Range on TH2
            reactionStack.Push("\"HM3500DLM110,MOD00B,B030504\"");  // Identification on TH2
            reactionStack.Push(string.Empty);                       // Initiate Self test on TH2

            reactionStack.Push("1,\"E1\"");                         // Check SCPI on TH2
            reactionStack.Push("2,\"E2\"");                         // Check SCPI on TH2
            reactionStack.Push("3,\"E3\"");                         // Check SCPI on TH2
            reactionStack.Push("4,\"E4\"");                         // Check SCPI on TH2
            reactionStack.Push("5,\"E5\"");                         // Check SCPI on TH2
            reactionStack.Push("6,\"E6\"");                         // Check SCPI on TH2

            reactionStack.Push("80");                               // Check battery on TH2
            reactionStack.Push("OK");                               // Check Manometer present ('OK' is Hardcoded).

            if (deviceType == DeviceType.PlexorBluetoothIrDA)
            {
                reactionStack.Push("er-001");                       // Flush manometer cache
                reactionStack.Push("CONNECT");                      // Exit local mode
            }

            reactionStack.Push("ok");                               // Switch to manometer TH2

            if (deviceType == DeviceType.PlexorBluetoothIrDA)
            {
                reactionStack.Push("ERROR 05");                     // Flush bluetooth cache
                reactionStack.Push("OK");                           // Enter local mode
            }

            if (deviceType == DeviceType.PlexorBluetoothWIS)
            {
                reactionStack.Push("LOCAL");                        // Enter local mode
            }            
            // Connect

            var stub = Hal as BluetoothHalSequentialStub;

            Assert.IsNotNull(stub);

            stub.DeviceType = deviceType;
            stub.SetUpReactionStack(reactionStack);
        }


        /// <summary>
        /// Initializations the battery level warning test.
        /// </summary>
        [TestCase(DeviceType.PlexorBluetoothWIS)]
        [TestCase(DeviceType.PlexorBluetoothIrDA)]
        public void InitializationBatteryLevelWarningTest(DeviceType deviceType)
        {
            Console.WriteLine(System.Reflection.MethodBase.GetCurrentMethod().Name);
            m_HasErrorBeenSet = false;

            SetUpBatteryLevelWarningStack(deviceType);
            
            InitializationActivityControl.InitializationFinished += InitializationActivityControl_InitializationFinished;
            InitializationActivityControl.InitializationStepFinished += InitializationActivityControl_InitializationStepFinished_BatteryLevelWarning;

            m_ManualResetEvent.Reset();
            
            InitializationActivityControl.ExecuteInitialization();
            
            Assert.IsTrue(m_ManualResetEvent.WaitOne(EventTimeout));

            InitializationActivityControl.InitializationFinished -= InitializationActivityControl_InitializationFinished;
            InitializationActivityControl.InitializationStepFinished -= InitializationActivityControl_InitializationStepFinished_BatteryLevelWarning;

            Assert.AreEqual(InitializationResult.WARNING, m_FinishResult);
            Assert.AreEqual(ErrorCodes.INITIALIZATION_FINISHED_WARNING, m_FinishErrorCode);

            Assert.AreEqual(ErrorCodes.INITIALIZATION_STEP_BATTERYLIMIT_LEVEL_WARNING, m_ExpectedStepErrorCode);
            Assert.AreEqual(InitializationManometer.TH2, m_ExpectedStepManometer);
            Assert.AreEqual("60", m_ExpectedStepMessage);
            Assert.AreEqual(InitializationStepResult.WARNING, m_ExpectedStepResult);
        }

        private void InitializationActivityControl_InitializationStepFinished_BatteryLevelWarning(object sender, EventArgs e)
        {
            SetFirstStepResult(e, DeviceCommand.CheckBatteryStatus.ToString());
        }

        /// <summary>
        /// Sets the battery level warning stack.
        /// </summary>
        private static void SetUpBatteryLevelWarningStack(DeviceType deviceType)
        {
            var reactionStack = new Stack<string>();

            // Disconnect
            if (deviceType == DeviceType.PlexorBluetoothIrDA)
            {
                reactionStack.Push("er-001");                       // Flush manometer cache
                reactionStack.Push("CONNECT");                      // Exit local mode
            }

            reactionStack.Push("ok");                               // Switch to manometer TH2

            if (deviceType == DeviceType.PlexorBluetoothIrDA)
            {
                reactionStack.Push("ERROR 05");                     // Flush bluetooth cache
                reactionStack.Push("OK");                           // Enter local mode
            }

            if (deviceType == DeviceType.PlexorBluetoothWIS)
            {
                reactionStack.Push("LOCAL");                        // Enter local mode

                reactionStack.Push("CONNECT");                      // Exit local mode
                reactionStack.Push("OK");                           // Enbale/Disable IO status
                reactionStack.Push("1.0.0");                        // Check system software version
                reactionStack.Push("20150624");                     // Check calibration date
                reactionStack.Push("LOCAL");                        // Enter local mode
            }

            reactionStack.Push("ok");                               // Set IRDA always on 
            reactionStack.Push("mbar");                             // Query pressure unit on TH1
            reactionStack.Push("ok");                               // Set Manometer Range on TH1
            reactionStack.Push("\"17 bar\"");                       // Query Manometer Range on TH1
            reactionStack.Push("\"HM3500DLM110,MOD00B,B030504\"");  // Identification on TH1
            reactionStack.Push(string.Empty);                       // Initiate Self test on TH1
            reactionStack.Push("0,\"no current Scpi errors!\"");    // Check SCPI on TH1

            reactionStack.Push("80");                               // Check battery on TH1
            reactionStack.Push("OK");                               // Check Manometer present ('OK' is Hardcoded).

            if (deviceType == DeviceType.PlexorBluetoothIrDA)
            {
                reactionStack.Push("er-001");                       // Flush manometer cache
                reactionStack.Push("CONNECT");                      // Exit local mode
            }

            reactionStack.Push("ok");                               // Switch to manometer TH1

            if (deviceType == DeviceType.PlexorBluetoothIrDA)
            {
                reactionStack.Push("ERROR 05");                     // Flush bluetooth cache
                reactionStack.Push("OK");                           // Enter local mode
            }

            if (deviceType == DeviceType.PlexorBluetoothWIS)
            {
                reactionStack.Push("LOCAL");                        // Enter local mode

                reactionStack.Push("CONNECT");                      // Exit local mode
                reactionStack.Push("OK");                           // Enbale/Disable IO status
                reactionStack.Push("1.0.0");                        // Check system software version
                reactionStack.Push("20150624");                     // Check calibration date
                reactionStack.Push("LOCAL");                        // Enter local mode
            }

            reactionStack.Push("ok");                               // Set IRDA always on 
            reactionStack.Push("mbar");                             // Query pressure unit on TH2
            reactionStack.Push("ok");                               // Set Manometer Range on TH2
            reactionStack.Push("\"2000 mbar\"");                    // Query Manometer Range on TH2
            reactionStack.Push("\"HM3500DLM110,MOD00B,B030504\"");  // Identification on TH2
            reactionStack.Push(string.Empty);                       // Initiate Self test on TH2
            reactionStack.Push("0,\"no current Scpi errors!\"");    // Check SCPI on TH2

            reactionStack.Push("60");                               // Check battery on TH2
            reactionStack.Push("OK");                               // Check Manometer present ('OK' is Hardcoded).

            if (deviceType == DeviceType.PlexorBluetoothIrDA)
            {
                reactionStack.Push("er-001");                       // Flush manometer cache
                reactionStack.Push("CONNECT");                      // Exit local mode
            }

            reactionStack.Push("ok");                               // Switch to manometer TH2

            if (deviceType == DeviceType.PlexorBluetoothIrDA)
            {
                reactionStack.Push("ERROR 05");                     // Flush bluetooth cache
                reactionStack.Push("OK");                           // Enter local mode
            }

            if (deviceType == DeviceType.PlexorBluetoothWIS)
            {
                reactionStack.Push("LOCAL");                        // Enter local mode
            }
            // Connect

            if (deviceType == DeviceType.PlexorBluetoothWIS)
            {
                //add SSD init responses
                reactionStack.Push("CONNECT");                      // Exit local mode
                reactionStack.Push("OK");                           // Activate Port IrDA
                reactionStack.Push("OK");                           // Enter local mode
                reactionStack.Push("SSD Sensor,37000303,35000101"); // SENS:IDEN ?
                reactionStack.Push("OK");                           // Stop sensor run
                reactionStack.Push("CONNECT");                      // Exit local mode
                reactionStack.Push("OK");                           // Activate port sensor
                reactionStack.Push("OK");                           // Enable io status
                reactionStack.Push("ON");                           // Check Io3 status
                reactionStack.Push("OK");                           // Enter local mode
            }

            var stub = Hal as BluetoothHalSequentialStub;

            Assert.IsNotNull(stub);

            stub.DeviceType = deviceType;
            stub.SetUpReactionStack(reactionStack);
        }

        /// <summary>
        /// Initializations the battery level format error test.
        /// </summary>
        [TestCase(DeviceType.PlexorBluetoothWIS)]
        [TestCase(DeviceType.PlexorBluetoothIrDA)]
        public void InitializationBatteryLevelFormatErrorTest(DeviceType deviceType)
        {
            Console.WriteLine(System.Reflection.MethodBase.GetCurrentMethod().Name);
            SetUpBatteryLevelFormatErrorStack(deviceType);

            InitializationActivityControl.InitializationFinished += InitializationActivityControl_InitializationFinished;
            InitializationActivityControl.InitializationStepFinished += InitializationActivityControl_InitializationStepFinished_BatteryLevelFormat;

            m_ManualResetEvent.Reset();

            InitializationActivityControl.ExecuteInitialization();

            Assert.IsTrue(m_ManualResetEvent.WaitOne(EventTimeout));

            InitializationActivityControl.InitializationStepFinished -= InitializationActivityControl_InitializationStepFinished_BatteryLevelFormat;
            InitializationActivityControl.InitializationFinished -= InitializationActivityControl_InitializationFinished;

            Assert.AreEqual(InitializationResult.ERROR, m_FinishResult);
            Assert.AreEqual(ErrorCodes.INITIALIZATION_FINISHED_ERROR, m_FinishErrorCode);

            Assert.AreEqual(ErrorCodes.INITIALIZATION_STEP_BATTERYLIMIT_FORMAT_ERROR, m_ExpectedStepErrorCode);
            Assert.AreEqual(InitializationManometer.TH2, m_ExpectedStepManometer);
            Assert.AreEqual("60a", m_ExpectedStepMessage);
            Assert.AreEqual(InitializationStepResult.ERROR, m_ExpectedStepResult);
        }

        private void InitializationActivityControl_InitializationStepFinished_BatteryLevelFormat(object sender, EventArgs e)
        {
            SetFirstStepResult(e, DeviceCommand.CheckBatteryStatus.ToString());
        }

        /// <summary>
        /// Sets the battery level format error stack.
        /// </summary>
        private static void SetUpBatteryLevelFormatErrorStack(DeviceType deviceType)
        {
            var reactionStack = new Stack<string>();

            // Disconnect
            reactionStack.Push("60a");                              // Check battery on TH2
            reactionStack.Push("OK");                               // Check Manometer present ('OK' is Hardcoded).

            if (deviceType == DeviceType.PlexorBluetoothIrDA)
            {
                reactionStack.Push("er-001");                       // Flush manometer cache
                reactionStack.Push("CONNECT");                      // Exit local mode
            }

            reactionStack.Push("ok");                               // Switch to manometer TH2
            
            if (deviceType == DeviceType.PlexorBluetoothWIS)
            {
                reactionStack.Push("LOCAL");                        // Enter local mode
            }

            if (deviceType == DeviceType.PlexorBluetoothIrDA)
            {
                reactionStack.Push("ERROR 05");                     // Flush bluetooth cache
                reactionStack.Push("OK");                           // Enter local mode
            }
            // Connect

            if (deviceType == DeviceType.PlexorBluetoothWIS)
            {
                //add SSD init responses
                reactionStack.Push("CONNECT");                      // Exit local mode
                reactionStack.Push("OK");                           // Activate Port IrDA
                reactionStack.Push("OK");                           // Enter local mode
                reactionStack.Push("SSD Sensor,37000303,35000101"); // SENS:IDEN ?
                reactionStack.Push("OK");                           // Stop sensor run
                reactionStack.Push("CONNECT");                      // Exit local mode
                reactionStack.Push("OK");                           // Activate port sensor
                reactionStack.Push("OK");                           // Enable io status
                reactionStack.Push("ON");                           // Check Io3 status
                reactionStack.Push("OK");                           // Enter local mode
            }

            var stub = Hal as BluetoothHalSequentialStub;
            Assert.IsNotNull(stub);

            stub.DeviceType = deviceType;
            stub.SetUpReactionStack(reactionStack);
        }

        /// <summary>
        /// Initializations the SCPI interface error test.
        /// </summary>
        [TestCase(DeviceType.PlexorBluetoothWIS)]
        [TestCase(DeviceType.PlexorBluetoothIrDA)]
        public void InitializationSCPIInterfaceErrorTest(DeviceType deviceType)
        {
            Console.WriteLine(System.Reflection.MethodBase.GetCurrentMethod().Name);
            SetUpSCPIErrorStack(deviceType);

            InitializationActivityControl.InitializationFinished += InitializationActivityControl_InitializationFinished;
            InitializationActivityControl.InitializationStepFinished += InitializationActivityControl_InitializationStepFinished_SCPIError;

            m_ManualResetEvent.Reset();
            
            InitializationActivityControl.ExecuteInitialization();

            Assert.IsTrue(m_ManualResetEvent.WaitOne(EventTimeout));

            InitializationActivityControl.InitializationStepFinished -= InitializationActivityControl_InitializationStepFinished_SCPIError;
            InitializationActivityControl.InitializationFinished -= InitializationActivityControl_InitializationFinished;

            Assert.AreEqual(InitializationResult.SUCCESS, m_FinishResult);
            Assert.AreEqual(ErrorCodes.INITIALIZATION_FINISHED_SUCCESSFULLY, m_FinishErrorCode);

            Assert.AreEqual(ErrorCodes.INITIALIZATION_STEP_SCPI_WARNING, m_ExpectedStepErrorCode);
            Assert.AreEqual(InitializationManometer.TH2, m_ExpectedStepManometer);
            Assert.AreEqual("1,\"E1\"", m_ExpectedStepMessage);
            Assert.AreEqual(InitializationStepResult.WARNING, m_ExpectedStepResult);
        }

        private void InitializationActivityControl_InitializationStepFinished_SCPIError(object sender, EventArgs e)
        {
            Assert.IsInstanceOf<FinishInitializationStepEventArgs>(e, "Expected FinishInitializationStepEventArgs");
            
            var eventArgs = e as FinishInitializationStepEventArgs;
            
            Assert.IsNotNull(eventArgs);

            if (eventArgs.StepId == DeviceCommand.CheckSCPIInterface.ToString())
            {
                if (m_Counter == 6)
                {
                    m_ExpectedStepErrorCode = eventArgs.ErrorCode;
                    m_ExpectedStepManometer = eventArgs.Manometer;
                    m_ExpectedStepMessage = eventArgs.Message;
                    m_ExpectedStepResult = eventArgs.Result;
                }
                m_Counter++;
            }
        }

        /// <summary>
        /// Sets the SCPI error stack.
        /// </summary>
        private static void SetUpSCPIErrorStack(DeviceType deviceType)
        {
            var reactionStack = new Stack<string>();

            // Disconnect
            if (deviceType == DeviceType.PlexorBluetoothIrDA)
            {
                reactionStack.Push("er-001");                       // Flush manometer cache
                reactionStack.Push("CONNECT");                      // Exit local mode
            }

            reactionStack.Push("ok");                               // Switch to manometer TH2

            if (deviceType == DeviceType.PlexorBluetoothIrDA)
            {
                reactionStack.Push("ERROR 05");                     // Flush bluetooth cache
                reactionStack.Push("OK");                           // Enter local mode
            }

            if (deviceType == DeviceType.PlexorBluetoothWIS)
            {
                reactionStack.Push("LOCAL");                        // Enter local mode

                reactionStack.Push("CONNECT");                      // Exit local mode
                reactionStack.Push("OK");                           // Enbale/Disable IO status
                reactionStack.Push("1.0.0");                        // Check system software version
                reactionStack.Push("20150624");                     // Check calibration date
                reactionStack.Push("LOCAL");                        // Enter local mode
            }

            reactionStack.Push("ok");                               // Set IRDA always on 
            reactionStack.Push("mbar");                             // Query pressure unit on TH1
            reactionStack.Push("ok");                               // Set Manometer Range on TH1
            reactionStack.Push("\"2000 mbar\"");                    // Query Manometer Range on TH1
            reactionStack.Push("\"HM3500DLM110,MOD00B,B030504\""); // Identification on TH1
            reactionStack.Push(string.Empty);                       // Initiate Self test on TH1
            reactionStack.Push("0,\"no current Scpi errors!\"");    // Check SCPI on TH1

            reactionStack.Push("80");                               // Check battery on TH1
            reactionStack.Push("OK");                               // Check Manometer present ('OK' is Hardcoded).

            if (deviceType == DeviceType.PlexorBluetoothIrDA)
            {
                reactionStack.Push("er-001");                       // Flush manometer cache
                reactionStack.Push("CONNECT");                      // Exit local mode
            }

            reactionStack.Push("ok");                               // Switch to manometer TH1

            if (deviceType == DeviceType.PlexorBluetoothIrDA)
            {
                reactionStack.Push("ERROR 05");                     // Flush bluetooth cache
                reactionStack.Push("OK");                           // Enter local mode
            }

            if (deviceType == DeviceType.PlexorBluetoothWIS)
            {
                reactionStack.Push("LOCAL");                        // Enter local mode

                reactionStack.Push("CONNECT");                      // Exit local mode
                reactionStack.Push("OK");                           // Enbale/Disable IO status
                reactionStack.Push("1.0.0");                        // Check system software version
                reactionStack.Push("20150624");                     // Check calibration date
                reactionStack.Push("LOCAL");                        // Enter local mode
            }

            reactionStack.Push("ok");                               // Set IRDA always on 
            reactionStack.Push("mbar");                             // Query pressure unit on TH2
            reactionStack.Push("ok");                               // Set Manometer Range on TH2
            reactionStack.Push("\"2000 mbar\"");                    // Query Manometer Range on TH2
            reactionStack.Push("\"HM3500DLM110,MOD00B,B030504\"");  // Identification on TH2
            reactionStack.Push(string.Empty);                       // Initiate Self test on TH2

            reactionStack.Push("1,\"E1\"");                         // Check SCPI on TH2
            reactionStack.Push("2,\"E2\"");                         // Check SCPI on TH2
            reactionStack.Push("3,\"E3\"");                         // Check SCPI on TH2
            reactionStack.Push("4,\"E4\"");                         // Check SCPI on TH2
            reactionStack.Push("5,\"E5\"");                         // Check SCPI on TH2
            reactionStack.Push("6,\"E6\"");                         // Check SCPI on TH2

            reactionStack.Push("80");                               // Check battery on TH2
            reactionStack.Push("OK");                               // Check Manometer present ('OK' is Hardcoded).

            if (deviceType == DeviceType.PlexorBluetoothIrDA)
            {
                reactionStack.Push("er-001");                       // Flush manometer cache
                reactionStack.Push("CONNECT");                      // Exit local mode
            }

            reactionStack.Push("ok");                               // Switch to manometer TH2

            if (deviceType == DeviceType.PlexorBluetoothIrDA)
            {
                reactionStack.Push("ERROR 05");                     // Flush bluetooth cache
                reactionStack.Push("OK");                           // Enter local mode
            }

            if (deviceType == DeviceType.PlexorBluetoothWIS)
            {
                reactionStack.Push("LOCAL");                        // Enter local mode
            }
            // Connect

            if (deviceType == DeviceType.PlexorBluetoothWIS)
            {
                //add SSD init responses
                reactionStack.Push("CONNECT");                      // Exit local mode
                reactionStack.Push("OK");                           // Activate Port IrDA
                reactionStack.Push("OK");                           // Enter local mode
                reactionStack.Push("SSD Sensor,37000303,35000101"); // SENS:IDEN ?
                reactionStack.Push("OK");                           // Stop sensor run
                reactionStack.Push("CONNECT");                      // Exit local mode
                reactionStack.Push("OK");                           // Activate port sensor
                reactionStack.Push("OK");                           // Enable io status
                reactionStack.Push("ON");                           // Check Io3 status
                reactionStack.Push("OK");                           // Enter local mode
            }

            var stub = Hal as BluetoothHalSequentialStub;
            Assert.IsNotNull(stub);

            stub.DeviceType = deviceType;
            stub.SetUpReactionStack(reactionStack);
        }

        /// <summary>
        /// Initializations the SCPI interface warning test.
        /// </summary>
        [TestCase(DeviceType.PlexorBluetoothWIS)]
        [TestCase(DeviceType.PlexorBluetoothIrDA)]
        public void InitializationSCPIInterfaceWarningTest(DeviceType deviceType)
        {
            Console.WriteLine(System.Reflection.MethodBase.GetCurrentMethod().Name);
            SetUpSCPIWarningStack(deviceType);

            InitializationActivityControl.InitializationFinished += InitializationActivityControl_InitializationFinished;
            InitializationActivityControl.InitializationStepFinished += InitializationActivityControl_InitializationStepFinished_CheckSCPIWarning;

            m_ManualResetEvent.Reset();

            InitializationActivityControl.ExecuteInitialization();

            Assert.IsTrue(m_ManualResetEvent.WaitOne(EventTimeout));

            InitializationActivityControl.InitializationStepFinished -= InitializationActivityControl_InitializationStepFinished_CheckSCPIWarning;
            InitializationActivityControl.InitializationFinished -= InitializationActivityControl_InitializationFinished;

            Assert.AreEqual(InitializationResult.SUCCESS, m_FinishResult);
            Assert.AreEqual(ErrorCodes.INITIALIZATION_FINISHED_SUCCESSFULLY, m_FinishErrorCode);

            Assert.AreEqual(ErrorCodes.INITIALIZATION_STEP_EXECUTED_SUCCESSFULLY, m_ExpectedStepErrorCode);
            Assert.AreEqual(InitializationManometer.TH2, m_ExpectedStepManometer);
            Assert.AreEqual("0,\"no current Scpi errors!\"", m_ExpectedStepMessage);
            Assert.AreEqual(InitializationStepResult.SUCCESS, m_ExpectedStepResult);
        }

        private void InitializationActivityControl_InitializationStepFinished_CheckSCPIWarning(object sender, EventArgs e)
        {
            Assert.IsInstanceOf<FinishInitializationStepEventArgs>(e, "Expected FinishInitializationStepEventArgs");
            
            var eventArgs = e as FinishInitializationStepEventArgs;

            Assert.IsNotNull(eventArgs);
                
            if (eventArgs.StepId == DeviceCommand.CheckSCPIInterface.ToString())
            {
                if (m_Counter == 3)
                {
                    m_ExpectedStepErrorCode = eventArgs.ErrorCode;
                    m_ExpectedStepManometer = eventArgs.Manometer;
                    m_ExpectedStepMessage = eventArgs.Message;
                    m_ExpectedStepResult = eventArgs.Result;
                }
                m_Counter++;
            }
        }

        /// <summary>
        /// Sets the SCPI warning stack.
        /// </summary>
        private static void SetUpSCPIWarningStack(DeviceType deviceType)
        {
            var reactionStack = new Stack<string>();

            // Disconnect
            if (deviceType == DeviceType.PlexorBluetoothIrDA)
            {
                reactionStack.Push("er-001");                       // Flush manometer cache
                reactionStack.Push("CONNECT");                      // Exit local mode
            }

            reactionStack.Push("ok");                               // Switch to manometer TH2

            if (deviceType == DeviceType.PlexorBluetoothIrDA)
            {
                reactionStack.Push("ERROR 05");                     // Flush bluetooth cache
                reactionStack.Push("OK");                           // Enter local mode
            }

            if (deviceType == DeviceType.PlexorBluetoothWIS)
            {
                reactionStack.Push("LOCAL");                        // Enter local mode

                reactionStack.Push("CONNECT");                      // Exit local mode
                reactionStack.Push("OK");                           // Enbale/Disable IO status
                reactionStack.Push("1.0.0");                        // Check system software version
                reactionStack.Push("20150624");                     // Check calibration date
                reactionStack.Push("LOCAL");                        // Enter local mode
            }

            reactionStack.Push("ok");                               // Set IRDA always on 
            reactionStack.Push("mbar");                             // Query pressure unit on TH1
            reactionStack.Push("ok");                               // Set Manometer Range on TH1
            reactionStack.Push("\"2000 mbar\"");                    // Query Manometer Range on TH1
            reactionStack.Push("\"HM3500DLM110,MOD00B,B030504\"");  // Identification on TH1
            reactionStack.Push(string.Empty);                       // Initiate Self test on TH1
            reactionStack.Push("0,\"no current Scpi errors!\"");    // Check SCPI on TH1

            reactionStack.Push("80");                               // Check battery on TH1
            reactionStack.Push("OK");                               // Check Manometer present ('OK' is Hardcoded).

            if (deviceType == DeviceType.PlexorBluetoothIrDA)
            {
                reactionStack.Push("er-001");                       // Flush manometer cache
                reactionStack.Push("CONNECT");                      // Exit local mode
            }

            reactionStack.Push("ok");                               // Switch to manometer TH1

            if (deviceType == DeviceType.PlexorBluetoothIrDA)
            {
                reactionStack.Push("ERROR 05");                     // Flush bluetooth cache
                reactionStack.Push("OK");                           // Enter local mode
            }

            if (deviceType == DeviceType.PlexorBluetoothWIS)
            {
                reactionStack.Push("LOCAL");                        // Enter local mode

                reactionStack.Push("CONNECT");                      // Exit local mode
                reactionStack.Push("OK");                           // Enbale/Disable IO status
                reactionStack.Push("1.0.0");                        // Check system software version
                reactionStack.Push("20150624");                     // Check calibration date
                reactionStack.Push("LOCAL");                        // Enter local mode
            }

            reactionStack.Push("ok");                               // Set IRDA always on 
            reactionStack.Push("mbar");                             // Query pressure unit on TH2
            reactionStack.Push("ok");                               // Set Manometer Range on TH2
            reactionStack.Push("\"2000 mbar\"");                    // Query Manometer Range on TH2
            reactionStack.Push("\"HM3500DLM110,MOD00B,B030504\"");  // Identification on TH2
            reactionStack.Push(string.Empty);                       // Initiate Self test on TH2
            reactionStack.Push("0,\"no current Scpi errors!\"");    // Check SCPI on TH2
            reactionStack.Push("2,\"Other Error\"");                // Check SCPI on TH2
            reactionStack.Push("1,\"Error\"");                      // Check SCPI on TH2

            reactionStack.Push("80");                               // Check battery on TH2
            reactionStack.Push("OK");                               // Check Manometer present ('OK' is Hardcoded).

            if (deviceType == DeviceType.PlexorBluetoothIrDA)
            {
                reactionStack.Push("er-001");                       // Flush manometer cache
                reactionStack.Push("CONNECT");                      // Exit local mode
            }

            reactionStack.Push("ok");                               // Switch to manometer TH2

            if (deviceType == DeviceType.PlexorBluetoothIrDA)
            {
                reactionStack.Push("ERROR 05");                     // Flush bluetooth cache
                reactionStack.Push("OK");                           // Enter local mode
            }

            if (deviceType == DeviceType.PlexorBluetoothWIS)
            {
                reactionStack.Push("LOCAL");                        // Enter local mode
            }
            // Connect

            if (deviceType == DeviceType.PlexorBluetoothWIS)
            {
                //add SSD init responses
                reactionStack.Push("CONNECT");                      // Exit local mode
                reactionStack.Push("OK");                           // Activate Port IrDA
                reactionStack.Push("OK");                           // Enter local mode
                reactionStack.Push("SSD Sensor,37000303,35000101"); // SENS:IDEN ?
                reactionStack.Push("OK");                           // Stop sensor run
                reactionStack.Push("CONNECT");                      // Exit local mode
                reactionStack.Push("OK");                           // Activate port sensor
                reactionStack.Push("OK");                           // Enable io status
                reactionStack.Push("ON");                           // Check Io3 status
                reactionStack.Push("OK");                           // Enter local mode
            }

            var stub = Hal as BluetoothHalSequentialStub;
            Assert.IsNotNull(stub);

            stub.DeviceType = deviceType;
            stub.SetUpReactionStack(reactionStack);
        }

        /// <summary>
        /// Initializations the set pressure unit error test.
        /// </summary>
        [TestCase(DeviceType.PlexorBluetoothWIS)]
        [TestCase(DeviceType.PlexorBluetoothIrDA)]
        public void InitializationSetPressureUnitErrorTest(DeviceType deviceType)
        {
            Console.WriteLine(System.Reflection.MethodBase.GetCurrentMethod().Name);
            SetUpSetPressureUnitErrorStack(deviceType);

            InitializationActivityControl.InitializationFinished += InitializationActivityControl_InitializationFinished;
            InitializationActivityControl.InitializationStepFinished += InitializationActivityControl_InitializationStepFinished_SetPressureUnit;

            m_ManualResetEvent.Reset();
            
            InitializationActivityControl.ExecuteInitialization();

            Assert.IsTrue(m_ManualResetEvent.WaitOne(EventTimeout));

            InitializationActivityControl.InitializationStepFinished -= InitializationActivityControl_InitializationStepFinished_SetPressureUnit;
            InitializationActivityControl.InitializationFinished -= InitializationActivityControl_InitializationFinished;

            Assert.AreEqual(InitializationResult.ERROR, m_FinishResult);
            Assert.AreEqual(ErrorCodes.INITIALIZATION_FINISHED_ERROR, m_FinishErrorCode);

            Assert.AreEqual(ErrorCodes.INITIALIZATION_STEP_SET_PRESSURE_UNIT_ERROR, m_ExpectedStepErrorCode);
            Assert.AreEqual(InitializationManometer.TH2, m_ExpectedStepManometer);
            Assert.AreEqual("nok", m_ExpectedStepMessage);
            Assert.AreEqual(InitializationStepResult.ERROR, m_ExpectedStepResult);
        }

        private void InitializationActivityControl_InitializationStepFinished_SetPressureUnit(object sender, EventArgs e)
        {
            SetFirstStepResult(e, DeviceCommand.SetPressureUnit.ToString());
        }

        /// <summary>
        /// Sets the set pressure unit error stack.
        /// </summary>
        private static void SetUpSetPressureUnitErrorStack(DeviceType deviceType)
        {
            var reactionStack = new Stack<string>();

            // Disconnect
            reactionStack.Push("nok");                              // Set Manometer Range on TH2
            reactionStack.Push("\"2000 mbar\"");                    // Query Manometer Range on TH2
            reactionStack.Push("\"HM3500DLM110,MOD00B,B030504\"");  // Identification on TH2
            reactionStack.Push(string.Empty);                       // Initiate Self test on TH2
            reactionStack.Push("0,\"no current Scpi errors!\"");    // Check SCPI on TH2

            reactionStack.Push("80");                               // Check battery on TH2
            reactionStack.Push("OK");                               // Check Manometer present ('OK' is Hardcoded).

            if (deviceType == DeviceType.PlexorBluetoothIrDA)
            {
                reactionStack.Push("er-001");                       // Flush manometer cache
                reactionStack.Push("CONNECT");                      // Exit local mode
            }

            reactionStack.Push("ok");                               // Switch to manometer TH2

            if (deviceType == DeviceType.PlexorBluetoothIrDA)
            {
                reactionStack.Push("ERROR 05");                     // Flush bluetooth cache
                reactionStack.Push("OK");                           // Enter local mode
            }

            if (deviceType == DeviceType.PlexorBluetoothWIS)
            {
                reactionStack.Push("LOCAL");                        // Enter local mode
            }
            // Connect

            if (deviceType == DeviceType.PlexorBluetoothWIS)
            {
                //add SSD init responses
                reactionStack.Push("CONNECT");                      // Exit local mode
                reactionStack.Push("OK");                           // Activate Port IrDA
                reactionStack.Push("OK");                           // Enter local mode
                reactionStack.Push("SSD Sensor,37000303,35000101"); // SENS:IDEN ?
                reactionStack.Push("OK");                           // Stop sensor run
                reactionStack.Push("CONNECT");                      // Exit local mode
                reactionStack.Push("OK");                           // Activate port sensor
                reactionStack.Push("OK");                           // Enable io status
                reactionStack.Push("ON");                           // Check Io3 status
                reactionStack.Push("OK");                           // Enter local mode
            }

            var stub = Hal as BluetoothHalSequentialStub;

            Assert.IsNotNull(stub);

            stub.DeviceType = deviceType;
            stub.SetUpReactionStack(reactionStack);
        }

        /// <summary>
        /// Initializations the query manometer range error test.
        /// </summary>
        [TestCase(DeviceType.PlexorBluetoothWIS)]
        [TestCase(DeviceType.PlexorBluetoothIrDA)]
        public void InitializationQueryManometerRangeErrorTest(DeviceType deviceType)
        {
            Console.WriteLine(System.Reflection.MethodBase.GetCurrentMethod().Name);
            SetUpQueryManometerRangeErrorStack(deviceType);

            InitializationActivityControl.InitializationFinished += InitializationActivityControl_InitializationFinished;
            InitializationActivityControl.InitializationStepFinished += InitializationActivityControl_InitializationStepFinished_ManometerRange;

            m_ManualResetEvent.Reset();

            InitializationActivityControl.ExecuteInitialization();

            Assert.IsTrue(m_ManualResetEvent.WaitOne(EventTimeout));

            InitializationActivityControl.InitializationStepFinished -= InitializationActivityControl_InitializationStepFinished_ManometerRange;
            InitializationActivityControl.InitializationFinished -= InitializationActivityControl_InitializationFinished;

            Assert.AreEqual(InitializationResult.ERROR, m_FinishResult);
            Assert.AreEqual(ErrorCodes.INITIALIZATION_FINISHED_ERROR, m_FinishErrorCode);

            Assert.AreEqual(ErrorCodes.INITIALIZATION_STEP_MANOMETER_RANGE_ERROR, m_ExpectedStepErrorCode);
            Assert.AreEqual(InitializationManometer.TH2, m_ExpectedStepManometer);
            Assert.AreEqual("\"2000\"", m_ExpectedStepMessage);
            Assert.AreEqual(InitializationStepResult.ERROR, m_ExpectedStepResult);
        }

        private void InitializationActivityControl_InitializationStepFinished_ManometerRange(object sender, EventArgs e)
        {
            SetFirstStepResult(e, DeviceCommand.CheckRange.ToString());
        }

        /// <summary>
        /// Sets the query manometer range error stack.
        /// </summary>
        private static void SetUpQueryManometerRangeErrorStack(DeviceType deviceType)
        {
            var reactionStack = new Stack<string>();
                       
            // Disconnect
            reactionStack.Push("\"2000\"");                         // Query Manometer Range on TH2
            reactionStack.Push("\"HM3500DLM110,MOD00B,B030504\"");  // Identification on TH2
            reactionStack.Push(string.Empty);                       // Initiate Self test on TH2
            reactionStack.Push("0,\"no current Scpi errors!\"");    // Check SCPI on TH2

            reactionStack.Push("80");                               // Check battery on TH2
            reactionStack.Push("OK");                               // Check Manometer present ('OK' is Hardcoded).

            if (deviceType == DeviceType.PlexorBluetoothIrDA)
            {
                reactionStack.Push("er-001");                       // Flush manometer cache
                reactionStack.Push("CONNECT");                      // Exit local mode
            }

            reactionStack.Push("ok");                               // Switch to manometer TH2

            if (deviceType == DeviceType.PlexorBluetoothIrDA)
            {
                reactionStack.Push("ERROR 05");                     // Flush bluetooth cache
                reactionStack.Push("OK");                           // Enter local mode
            }

            if (deviceType == DeviceType.PlexorBluetoothWIS)
            {
                reactionStack.Push("LOCAL");                        // Enter local mode
            }
            // Connect
            if (deviceType == DeviceType.PlexorBluetoothWIS)
            {
                //add SSD init responses
                reactionStack.Push("CONNECT");                      // Exit local mode
                reactionStack.Push("OK");                           // Activate Port IrDA
                reactionStack.Push("OK");                           // Enter local mode
                reactionStack.Push("SSD Sensor,37000303,35000101"); // SENS:IDEN ?
                reactionStack.Push("OK");                           // Stop sensor run
                reactionStack.Push("CONNECT");                      // Exit local mode
                reactionStack.Push("OK");                           // Activate port sensor
                reactionStack.Push("OK");                           // Enable io status
                reactionStack.Push("ON");                           // Check Io3 status
                reactionStack.Push("OK");                           // Enter local mode
            }
            var stub = Hal as BluetoothHalSequentialStub;

            Assert.IsNotNull(stub);

            stub.DeviceType = deviceType;
            stub.SetUpReactionStack(reactionStack);
        }

        /// <summary>
        /// Initializations the initiate self test error test.
        /// </summary>
        [TestCase(DeviceType.PlexorBluetoothWIS)]
        [TestCase(DeviceType.PlexorBluetoothIrDA)]
        public void InitializationInitiateSelfTestErrorTest(DeviceType deviceType)
        {
            Console.WriteLine(System.Reflection.MethodBase.GetCurrentMethod().Name);
            SetUpInitiateSelfTestErrorStack(deviceType);
            
            InitializationActivityControl.InitializationFinished += InitializationActivityControl_InitializationFinished;
            InitializationActivityControl.InitializationStepFinished += InitializationActivityControl_InitializationStepFinished_SelfTest;

            m_ManualResetEvent.Reset();
            
            InitializationActivityControl.ExecuteInitialization();

            Assert.IsTrue(m_ManualResetEvent.WaitOne(EventTimeout));

            InitializationActivityControl.InitializationStepFinished -= InitializationActivityControl_InitializationStepFinished_SelfTest;
            InitializationActivityControl.InitializationFinished -= InitializationActivityControl_InitializationFinished;

            Assert.AreEqual(InitializationResult.ERROR, m_FinishResult);
            Assert.AreEqual(ErrorCodes.INITIALIZATION_FINISHED_ERROR, m_FinishErrorCode);

            Assert.AreEqual(ErrorCodes.INITIALIZATION_STEP_SELF_TEST_ERROR, m_ExpectedStepErrorCode);
            Assert.AreEqual(InitializationManometer.TH2, m_ExpectedStepManometer);
            Assert.AreEqual("nok", m_ExpectedStepMessage);
            Assert.AreEqual(InitializationStepResult.ERROR, m_ExpectedStepResult);
        }

        private void InitializationActivityControl_InitializationStepFinished_SelfTest(object sender, EventArgs e)
        {
            SetFirstStepResult(e, DeviceCommand.InitiateSelfTest.ToString());
        }

        /// <summary>
        /// Sets the initiate self test error stack.
        /// </summary>
        private static void SetUpInitiateSelfTestErrorStack(DeviceType deviceType)
        {
            var reactionStack = new Stack<string>();

            // Disconnect
            reactionStack.Push("nok");                              // Initiate Self test on TH2
            reactionStack.Push("0,\"no current Scpi errors!\"");    // Check SCPI on TH2

            reactionStack.Push("80");                               // Check battery on TH2
            reactionStack.Push("OK");                               // Check Manometer present ('OK' is Hardcoded).

            if (deviceType == DeviceType.PlexorBluetoothIrDA)
            {
                reactionStack.Push("er-001");                       // Flush manometer cache
                reactionStack.Push("CONNECT");                      // Exit local mode
            }

            reactionStack.Push("ok");                               // Switch to manometer TH2

            if (deviceType == DeviceType.PlexorBluetoothIrDA)
            {
                reactionStack.Push("ERROR 05");                     // Flush bluetooth cache
                reactionStack.Push("OK");                           // Enter local mode
            }

            if (deviceType == DeviceType.PlexorBluetoothWIS)
            {
                reactionStack.Push("LOCAL");                        // Enter local mode
            }
            // Connect
            if (deviceType == DeviceType.PlexorBluetoothWIS)
            {
                //add SSD init responses
                reactionStack.Push("CONNECT");                      // Exit local mode
                reactionStack.Push("OK");                           // Activate Port IrDA
                reactionStack.Push("OK");                           // Enter local mode
                reactionStack.Push("SSD Sensor,37000303,35000101"); // SENS:IDEN ?
                reactionStack.Push("OK");                           // Stop sensor run
                reactionStack.Push("CONNECT");                      // Exit local mode
                reactionStack.Push("OK");                           // Activate port sensor
                reactionStack.Push("OK");                           // Enable io status
                reactionStack.Push("ON");                           // Check Io3 status
                reactionStack.Push("OK");                           // Enter local mode
            }
            var stub = Hal as BluetoothHalSequentialStub;

            Assert.IsNotNull(stub);

            stub.DeviceType = deviceType;
            stub.SetUpReactionStack(reactionStack);
        }

        /// <summary>
        /// Initializations the initiate self test error test.
        /// </summary>
        [TestCase(DeviceType.PlexorBluetoothWIS)]
        [TestCase(DeviceType.PlexorBluetoothIrDA)]
        public void InitializationConnectErrorTest(DeviceType deviceType)
        {
            Console.WriteLine(System.Reflection.MethodBase.GetCurrentMethod().Name);
            SetUpConnectErrorStack(deviceType);

            InitializationActivityControl.InitializationFinished += InitializationActivityControl_InitializationFinished;
            InitializationActivityControl.InitializationStepFinished += InitializationActivityControl_InitializationStepFinished_Connect;

            m_ManualResetEvent.Reset();

            InitializationActivityControl.ExecuteInitialization();

            Assert.IsTrue(m_ManualResetEvent.WaitOne(EventTimeout));

            InitializationActivityControl.InitializationStepFinished -= InitializationActivityControl_InitializationStepFinished_Connect;
            InitializationActivityControl.InitializationFinished -= InitializationActivityControl_InitializationFinished;

            Assert.AreEqual(InitializationResult.ERROR, m_FinishResult);
            Assert.AreEqual(ErrorCodes.INITIALIZATION_FINISHED_ERROR, m_FinishErrorCode);

            Assert.AreEqual(100, m_ExpectedStepErrorCode);
            Assert.AreEqual(InitializationManometer.BLUETOOTH_MODULE, m_ExpectedStepManometer);
            Assert.AreEqual(string.Empty, m_ExpectedStepMessage);
            Assert.AreEqual(InitializationStepResult.ERROR, m_ExpectedStepResult);
        }

        private void InitializationActivityControl_InitializationStepFinished_Connect(object sender, EventArgs e)
        {
            SetFirstStepResult(e, DeviceCommand.Connect.ToString());
        }

        /// <summary>
        /// Sets up connect error stack.
        /// </summary>
        private static void SetUpConnectErrorStack(DeviceType deviceType)
        {
            var stub = Hal as BluetoothHalSequentialStub;

            Assert.IsNotNull(stub);

            stub.DeviceType = deviceType;
            stub.SetConnectError(true);
        }

        [TestCase(DeviceType.PlexorBluetoothWIS)]
        [TestCase(DeviceType.PlexorBluetoothIrDA)]
        public void InitializationErrorAfterErrorTest(DeviceType deviceType)
        {
            Console.WriteLine(System.Reflection.MethodBase.GetCurrentMethod().Name);
            SetUpErrorAfterErrorStack(deviceType);

            InitializationActivityControl.InitializationFinished += InitializationActivityControl_InitializationFinished;

            m_ManualResetEvent.Reset();
            
            InitializationActivityControl.ExecuteInitialization();

            Assert.IsTrue(m_ManualResetEvent.WaitOne(EventTimeout));

            InitializationActivityControl.InitializationFinished -= InitializationActivityControl_InitializationFinished;

            Assert.AreEqual(InitializationResult.ERROR, m_FinishResult);
            Assert.AreEqual(ErrorCodes.INITIALIZATION_FINISHED_ERROR, m_FinishErrorCode);
        }

        /// <summary>
        /// Sets the initiate self test error stack.
        /// </summary>
        private static void SetUpErrorAfterErrorStack(DeviceType deviceType)
        {
            var reactionStack = new Stack<string>();

            // Disconnect
            reactionStack.Push("nok");                              // Initiate Self test on TH2
            reactionStack.Push("0,\"no current Scpi errors!\"");    // Check SCPI on TH2

            reactionStack.Push("80");                               // Check battery on TH2
            reactionStack.Push("OK");                               // Check Manometer present ('OK' is Hardcoded).
            reactionStack.Push("ok");                               // Switch to manometer TH2

            if (deviceType == DeviceType.PlexorBluetoothWIS)
            {
                reactionStack.Push("LOCAL");                        // Enter local mode
            }
            // Connect

            if (deviceType == DeviceType.PlexorBluetoothWIS)
            {
                //add SSD init responses
                reactionStack.Push("CONNECT");                      // Exit local mode
                reactionStack.Push("OK");                           // Activate Port IrDA
                reactionStack.Push("OK");                           // Enter local mode
                reactionStack.Push("SSD Sensor,37000303,35000101"); // SENS:IDEN ?
                reactionStack.Push("OK");                           // Stop sensor run
                reactionStack.Push("CONNECT");                      // Exit local mode
                reactionStack.Push("OK");                           // Activate port sensor
                reactionStack.Push("OK");                           // Enable io status
                reactionStack.Push("ON");                           // Check Io3 status
                reactionStack.Push("OK");                           // Enter local mode
            }

            var stub = Hal as BluetoothHalSequentialStub;

            Assert.IsNotNull(stub);

            stub.DeviceType = deviceType;
            stub.SetUpReactionStack(reactionStack);
        }

        [Test, Sequential]
        public void ManometerThrowsErrorTestPlexorBluetoothWIS([Values(1600, 1601, 1602, 1603, 1604, 1605, 1606, 1607, 1608)] int errorCodeToThrow)
        {
            Console.WriteLine(System.Reflection.MethodBase.GetCurrentMethod().Name);
            var deviceType = DeviceType.PlexorBluetoothWIS;

            SetUpManometerErrorStack(deviceType, errorCodeToThrow);

            InitializationActivityControl.InitializationFinished += InitializationActivityControl_InitializationFinished;
            InitializationActivityControl.InitializationStepFinished += InitializationActivityControl_InitializationStepFinished_Error;

            m_ManualResetEvent.Reset();

            InitializationActivityControl.ExecuteInitialization();

            Assert.IsTrue(m_ManualResetEvent.WaitOne(EventTimeout));

            InitializationActivityControl.InitializationStepFinished -= InitializationActivityControl_InitializationStepFinished_Error;
            InitializationActivityControl.InitializationFinished -= InitializationActivityControl_InitializationFinished;

            Assert.AreEqual(InitializationResult.ERROR, m_FinishResult);
            Assert.AreEqual(ErrorCodes.INITIALIZATION_FINISHED_ERROR, m_FinishErrorCode);

            Assert.AreEqual(errorCodeToThrow, m_ExpectedStepErrorCode);
            Assert.AreEqual(InitializationManometer.TH2, m_ExpectedStepManometer);
            Assert.AreEqual(string.Empty, m_ExpectedStepMessage);
            Assert.AreEqual(InitializationStepResult.ERROR, m_ExpectedStepResult);
        }

        [Test, Sequential]
        public void ManometerThrowsErrorTestPlexorBluetoothIrDA([Values(1600, 1601, 1602, 1603, 1604, 1605, 1606, 1607, 1608)] int errorCodeToThrow)
        {
            Console.WriteLine(System.Reflection.MethodBase.GetCurrentMethod().Name);
            var deviceType = DeviceType.PlexorBluetoothIrDA;

            SetUpManometerErrorStack(deviceType, errorCodeToThrow);

            InitializationActivityControl.InitializationFinished += InitializationActivityControl_InitializationFinished;
            InitializationActivityControl.InitializationStepFinished += InitializationActivityControl_InitializationStepFinished_Error;

            m_ManualResetEvent.Reset();

            InitializationActivityControl.ExecuteInitialization();

            Assert.IsTrue(m_ManualResetEvent.WaitOne(EventTimeout));

            InitializationActivityControl.InitializationStepFinished -= InitializationActivityControl_InitializationStepFinished_Error;
            InitializationActivityControl.InitializationFinished -= InitializationActivityControl_InitializationFinished;

            Assert.AreEqual(InitializationResult.ERROR, m_FinishResult);
            Assert.AreEqual(ErrorCodes.INITIALIZATION_FINISHED_ERROR, m_FinishErrorCode);

            Assert.AreEqual(errorCodeToThrow, m_ExpectedStepErrorCode);
            Assert.AreEqual(InitializationManometer.TH2, m_ExpectedStepManometer);
            Assert.AreEqual(string.Empty, m_ExpectedStepMessage);
            Assert.AreEqual(InitializationStepResult.ERROR, m_ExpectedStepResult);
        }

        /// <summary>
        /// Sets up manometer error stack.
        /// </summary>
        /// <param name="deviceType">The device type.</param>
        /// <param name="errorCodeToThrow">The error code to throw.</param>
        private static void SetUpManometerErrorStack(DeviceType deviceType, int errorCodeToThrow)
        {
            var reactionStack = new Stack<string>();

            // Disconnect
            reactionStack.Push("ManometerError:" + errorCodeToThrow);   // Check battery on TH2 (throw error)
            reactionStack.Push("OK");                                   // Check Manometer present ('OK' is Hardcoded).

            if (deviceType == DeviceType.PlexorBluetoothIrDA)
            {
                reactionStack.Push("er-001");                       // Flush manometer cache
                reactionStack.Push("CONNECT");                      // Exit local mode
            }

            reactionStack.Push("ok");                                   // Switch to manometer TH2

            if (deviceType == DeviceType.PlexorBluetoothIrDA)
            {
                reactionStack.Push("ERROR 05");                     // Flush bluetooth cache
                reactionStack.Push("OK");                           // Enter local mode
            }

            if (deviceType == DeviceType.PlexorBluetoothWIS)
            {
                reactionStack.Push("LOCAL");                        // Enter local mode
            }
            // Connect

            if (deviceType == DeviceType.PlexorBluetoothWIS)
            {
                //add SSD init responses
                reactionStack.Push("CONNECT");                      // Exit local mode
                reactionStack.Push("OK");                           // Activate Port IrDA
                reactionStack.Push("OK");                           // Enter local mode
                reactionStack.Push("SSD Sensor,37000303,35000101"); // SENS:IDEN ?
                reactionStack.Push("OK");                           // Stop sensor run
                reactionStack.Push("CONNECT");                      // Exit local mode
                reactionStack.Push("OK");                           // Activate port sensor
                reactionStack.Push("OK");                           // Enable io status
                reactionStack.Push("ON");                           // Check Io3 status
                reactionStack.Push("OK");                           // Enter local mode
            }

            var stub = Hal as BluetoothHalSequentialStub;

            Assert.IsNotNull(stub);

            stub.DeviceType = deviceType;
            stub.SetUpReactionStack(reactionStack);
        }

        /// <summary>
        /// Initializations the battery level format error test.
        /// </summary>
        //[TestCase(DeviceType.PlexorBluetoothWIS)]
        //[TestCase(DeviceType.PlexorBluetoothIrDA)]
        public void InitializationErrorInStepTest(DeviceType deviceType)
        {
            Console.WriteLine(System.Reflection.MethodBase.GetCurrentMethod().Name);
            SetUpErrorStack(deviceType);

            InitializationActivityControl.InitializationFinished += InitializationActivityControl_InitializationFinished;
            InitializationActivityControl.InitializationStepFinished += InitializationActivityControl_InitializationStepFinished_Error;

            m_ManualResetEvent.Reset();

            InitializationActivityControl.ExecuteInitialization();
            
            Assert.IsTrue(m_ManualResetEvent.WaitOne(EventTimeout));

            InitializationActivityControl.InitializationStepFinished -= InitializationActivityControl_InitializationStepFinished_Error;
            InitializationActivityControl.InitializationFinished -= InitializationActivityControl_InitializationFinished;

            Assert.AreEqual(InitializationResult.ERROR, m_FinishResult);
            Assert.AreEqual(ErrorCodes.INITIALIZATION_FINISHED_ERROR, m_FinishErrorCode);

            Assert.AreEqual(100, m_ExpectedStepErrorCode);
            Assert.AreEqual(InitializationManometer.TH2, m_ExpectedStepManometer);
            Assert.AreEqual(string.Empty, m_ExpectedStepMessage);
            Assert.AreEqual(InitializationStepResult.ERROR, m_ExpectedStepResult);
        }

        private void InitializationActivityControl_InitializationStepFinished_Error(object sender, EventArgs e)
        {
            SetFirstStepResult(e, DeviceCommand.CheckBatteryStatus.ToString());
        }

        /// <summary>
        /// Sets the battery level format error stack.
        /// </summary>
        private static void SetUpErrorStack(DeviceType deviceType)
        {
            var reactionStack = new Stack<string>();

            // Disconnect
            reactionStack.Push("Error:100");                        // Check Battery on TH2
            reactionStack.Push("ok");                               // Check Manometer present

            reactionStack.Push("Error:100");                        // Check battery on TH2
            reactionStack.Push("OK");                               // Check Manometer present ('OK' is Hardcoded).

            if (deviceType == DeviceType.PlexorBluetoothIrDA)
            {
                reactionStack.Push("er-001");                       // Flush manometer cache
                reactionStack.Push("CONNECT");                      // Exit local mode
            }

            reactionStack.Push("ok");                               // Switch to manometer TH2

            if (deviceType == DeviceType.PlexorBluetoothIrDA)
            {
                reactionStack.Push("ERROR 05");                     // Flush bluetooth cache
                reactionStack.Push("OK");                           // Enter local mode
            }

            if (deviceType == DeviceType.PlexorBluetoothWIS)
            {
                reactionStack.Push("LOCAL");                        // Enter local mode
            }
            // Connect

            if (deviceType == DeviceType.PlexorBluetoothWIS)
            {
                //add SSD init responses
                reactionStack.Push("CONNECT");                      // Exit local mode
                reactionStack.Push("OK");                           // Activate Port IrDA
                reactionStack.Push("OK");                           // Enter local mode
                reactionStack.Push("SSD Sensor,37000303,35000101"); // SENS:IDEN ?
                reactionStack.Push("OK");                           // Stop sensor run
                reactionStack.Push("CONNECT");                      // Exit local mode
                reactionStack.Push("OK");                           // Activate port sensor
                reactionStack.Push("OK");                           // Enable io status
                reactionStack.Push("ON");                           // Check Io3 status
                reactionStack.Push("OK");                           // Enter local mode
            }

            var stub = Hal as BluetoothHalSequentialStub;

            Assert.IsNotNull(stub);

            stub.DeviceType = deviceType;
            stub.SetUpReactionStack(reactionStack);
        }

        /// <summary>
        /// Initializations the battery level warning test.
        /// </summary>
        [TestCase(DeviceType.PlexorBluetoothWIS)]
        [TestCase(DeviceType.PlexorBluetoothIrDA)]
        public void InitializationTimeoutTest(DeviceType deviceType)
        {
            Console.WriteLine(System.Reflection.MethodBase.GetCurrentMethod().Name);
            m_HasErrorBeenSet = false;

            SetUpTimeoutStack(deviceType);

            InitializationActivityControl.InitializationFinished += InitializationActivityControl_InitializationFinished;
            InitializationActivityControl.InitializationStepFinished += InitializationActivityControl_InitializationStepFinished_Timeout;

            m_ManualResetEvent.Reset();

            InitializationActivityControl.ExecuteInitialization();

            Assert.IsTrue(m_ManualResetEvent.WaitOne(EventTimeout));

            InitializationActivityControl.InitializationFinished -= InitializationActivityControl_InitializationFinished;
            InitializationActivityControl.InitializationStepFinished -= InitializationActivityControl_InitializationStepFinished_Timeout;

            Assert.AreEqual(InitializationResult.TIMEOUT, m_FinishResult);
            Assert.AreEqual(ErrorCodes.INITIALIZATION_FINISHED_TIMEOUT, m_FinishErrorCode);

            Assert.AreEqual(ErrorCodes.HAL_COMMAND_TIMEOUT_RECEIVED, m_ExpectedStepErrorCode);
            Assert.AreEqual(InitializationManometer.TH2, m_ExpectedStepManometer);
            Assert.AreEqual(string.Empty, m_ExpectedStepMessage);
            Assert.AreEqual(InitializationStepResult.TIMEOUT, m_ExpectedStepResult);
        }

        private void InitializationActivityControl_InitializationStepFinished_Timeout(object sender, EventArgs e)
        {
            SetFirstStepResult(e, DeviceCommand.CheckBatteryStatus.ToString());
        }

        /// <summary>
        /// Sets the battery level warning stack.
        /// </summary>
        private static void SetUpTimeoutStack(DeviceType deviceType)
        {
            var reactionStack = new Stack<string>();

            // Disconnect
            if (deviceType == DeviceType.PlexorBluetoothIrDA)
            {
                reactionStack.Push("er-001");                       // Flush manometer cache
                reactionStack.Push("CONNECT");                      // Exit local mode
            }

            reactionStack.Push("ok");                               // Switch to manometer TH2

            if (deviceType == DeviceType.PlexorBluetoothIrDA)
            {
                reactionStack.Push("ERROR 05");                     // Flush bluetooth cache
                reactionStack.Push("OK");                           // Enter local mode
            }

            if (deviceType == DeviceType.PlexorBluetoothWIS)
            {
                reactionStack.Push("LOCAL");                        // Enter local mode

                reactionStack.Push("CONNECT");                      // Exit local mode
                reactionStack.Push("OK");                           // Enbale/Disable IO status
                reactionStack.Push("1.0.0");                        // Check system software version
                reactionStack.Push("20150624");                     // Check calibration date
                reactionStack.Push("LOCAL");                        // Enter local mode
            }

            reactionStack.Push("ok");                               // Set IRDA always on 
            reactionStack.Push("mbar");                             // Query pressure unit on TH1
            reactionStack.Push("ok");                               // Set Manometer Range on TH1
            reactionStack.Push("\"2000 mbar\"");                    // Query Manometer Range on TH1
            reactionStack.Push("\"HM3500DLM110,MOD00B,B030504\"");  // Identification on TH1
            reactionStack.Push(string.Empty);                       // Initiate Self test on TH1
            reactionStack.Push("0,\"no current Scpi errors!\"");    // Check SCPI on TH1

            reactionStack.Push("80");                               // Check battery on TH1
            reactionStack.Push("OK");                               // Check Manometer present ('OK' is Hardcoded).

            if (deviceType == DeviceType.PlexorBluetoothIrDA)
            {
                reactionStack.Push("er-001");                       // Flush manometer cache
                reactionStack.Push("CONNECT");                      // Exit local mode
            }

            reactionStack.Push("ok");                               // Switch to manometer TH1

            if (deviceType == DeviceType.PlexorBluetoothIrDA)
            {
                reactionStack.Push("ERROR 05");                     // Flush bluetooth cache
                reactionStack.Push("OK");                           // Enter local mode
            }

            if (deviceType == DeviceType.PlexorBluetoothWIS)
            {
                reactionStack.Push("LOCAL");                        // Enter local mode
            }
            
            reactionStack.Push("TIMEOUT");                          // Check battery on TH2
            reactionStack.Push("OK");                               // Check Manometer present ('OK' is Hardcoded).

            if (deviceType == DeviceType.PlexorBluetoothIrDA)
            {
                reactionStack.Push("er-001");                       // Flush manometer cache
                reactionStack.Push("CONNECT");                      // Exit local mode
            }

            reactionStack.Push("ok");                               // Switch to manometer TH2

            if (deviceType == DeviceType.PlexorBluetoothIrDA)
            {
                reactionStack.Push("ERROR 05");                     // Flush bluetooth cache
                reactionStack.Push("OK");                           // Enter local mode
            }

            if (deviceType == DeviceType.PlexorBluetoothWIS)
            {
                reactionStack.Push("LOCAL");                        // Enter local mode
            }
            // Connect

            if (deviceType == DeviceType.PlexorBluetoothWIS)
            {
                //add SSD init responses
                reactionStack.Push("CONNECT");                      // Exit local mode
                reactionStack.Push("OK");                           // Activate Port IrDA
                reactionStack.Push("OK");                           // Enter local mode
                reactionStack.Push("SSD Sensor,37000303,35000101"); // SENS:IDEN ?
                reactionStack.Push("OK");                           // Stop sensor run
                reactionStack.Push("CONNECT");                      // Exit local mode
                reactionStack.Push("OK");                           // Activate port sensor
                reactionStack.Push("OK");                           // Enable io status
                reactionStack.Push("ON");                           // Check Io3 status
                reactionStack.Push("OK");                           // Enter local mode
            }

            var stub = Hal as BluetoothHalSequentialStub;

            Assert.IsNotNull(stub);

            stub.DeviceType = deviceType;
            stub.SetUpReactionStack(reactionStack);
        }

        [TestCase(DeviceType.PlexorBluetoothWIS)]
        [TestCase(DeviceType.PlexorBluetoothIrDA)]
        public void InitializationManometerNotPresentTest(DeviceType deviceType)
        {
            Console.WriteLine(System.Reflection.MethodBase.GetCurrentMethod().Name);
            m_HasErrorBeenSet = false;

            SetUpManometerNotPresentStack(deviceType);
            
            InitializationActivityControl.InitializationFinished += InitializationActivityControl_InitializationFinished;
            InitializationActivityControl.InitializationStepFinished += InitializationActivityControl_InitializationStepFinished_ManometerNotPresent;

            m_ManualResetEvent.Reset();

            InitializationActivityControl.ExecuteInitialization();

            Assert.IsTrue(m_ManualResetEvent.WaitOne(EventTimeout));

            InitializationActivityControl.InitializationFinished -= InitializationActivityControl_InitializationFinished;
            InitializationActivityControl.InitializationStepFinished -= InitializationActivityControl_InitializationStepFinished_ManometerNotPresent;

            Assert.AreEqual(InitializationResult.ERROR, m_FinishResult);
            Assert.AreEqual(ErrorCodes.INITIALIZATION_FINISHED_ERROR, m_FinishErrorCode);

            Assert.AreEqual(ErrorCodes.INITIALIZATION_MANOMETER_NOT_PRESENT, m_ExpectedStepErrorCode);
            Assert.AreEqual(InitializationManometer.TH2, m_ExpectedStepManometer);
            Assert.AreEqual("Manometer not present", m_ExpectedStepMessage);
            Assert.AreEqual(InitializationStepResult.ERROR, m_ExpectedStepResult);
        }

        private void InitializationActivityControl_InitializationStepFinished_ManometerNotPresent(object sender, EventArgs e)
        {
            SetFirstStepResult(e, DeviceCommand.CheckManometerPresent.ToString());
        }

        /// <summary>
        /// Sets the manometer not present error stack.
        /// </summary>
        private static void SetUpManometerNotPresentStack(DeviceType deviceType)
        {
            var reactionStack = new Stack<string>();

            // Disconnect
            if (deviceType == DeviceType.PlexorBluetoothIrDA)
            {
                reactionStack.Push("er-001");                       // Flush manometer cache
                reactionStack.Push("CONNECT");                      // Exit local mode
            }

            reactionStack.Push("ok");                               // Switch to manometer TH2

            if (deviceType == DeviceType.PlexorBluetoothIrDA)
            {
                reactionStack.Push("ERROR 05");                     // Flush bluetooth cache
                reactionStack.Push("OK");                           // Enter local mode
            }

            if (deviceType == DeviceType.PlexorBluetoothWIS)
            {
                reactionStack.Push("LOCAL");                        // Enter local mode

                reactionStack.Push("CONNECT");                      // Exit local mode
                reactionStack.Push("OK");                           // Enbale/Disable IO status
                reactionStack.Push("1.0.0");                        // Check system software version
                reactionStack.Push("20150624");                     // Check calibration date
                reactionStack.Push("LOCAL");                        // Enter local mode
            }

            reactionStack.Push("ok");                               // Set IRDA always on 
            reactionStack.Push("mbar");                             // Query pressure unit on TH1
            reactionStack.Push("ok");                               // Set Manometer Range on TH1
            reactionStack.Push("\"2000 mbar\"");                    // Query Manometer Range on TH1
            reactionStack.Push("\"HM3500DLM110,MOD00B,B030504\"");  // Identification on TH1
            reactionStack.Push(string.Empty);                       // Initiate Self test on TH1
            reactionStack.Push("0,\"no current Scpi errors!\"");    // Check SCPI on TH1

            reactionStack.Push("80");                               // Check battery on TH1
            reactionStack.Push("OK");                               // Check Manometer present ('OK' is Hardcoded).

            if (deviceType == DeviceType.PlexorBluetoothIrDA)
            {
                reactionStack.Push("er-001");                       // Flush manometer cache
                reactionStack.Push("CONNECT");                      // Exit local mode
            }

            reactionStack.Push("ok");                               // Switch to manometer TH1

            if (deviceType == DeviceType.PlexorBluetoothIrDA)
            {
                reactionStack.Push("ERROR 05");                     // Flush bluetooth cache
                reactionStack.Push("OK");                           // Enter local mode
            }

            if (deviceType == DeviceType.PlexorBluetoothWIS)
            {
                reactionStack.Push("LOCAL");                        // Enter local mode
            }
            
            reactionStack.Push("TIMEOUT");                          // Check Manometer present ('OK' is Hardcoded).

            if (deviceType == DeviceType.PlexorBluetoothIrDA)
            {
                reactionStack.Push("er-001");                       // Flush manometer cache
                reactionStack.Push("CONNECT");                      // Exit local mode
            }

            reactionStack.Push("ok");                               // Switch to manometer TH2

            if (deviceType == DeviceType.PlexorBluetoothIrDA)
            {
                reactionStack.Push("ERROR 05");                     // Flush bluetooth cache
                reactionStack.Push("OK");                           // Enter local mode
            }

            if (deviceType == DeviceType.PlexorBluetoothWIS)
            {
                reactionStack.Push("LOCAL");                        // Enter local mode
            }
            // Connect
            if (deviceType == DeviceType.PlexorBluetoothWIS)
            {
                //add SSD init responses
                reactionStack.Push("CONNECT");                      // Exit local mode
                reactionStack.Push("OK");                           // Activate Port IrDA
                reactionStack.Push("OK");                           // Enter local mode
                reactionStack.Push("SSD Sensor,37000303,35000101"); // SENS:IDEN ?
                reactionStack.Push("OK");                           // Stop sensor run
                reactionStack.Push("CONNECT");                      // Exit local mode
                reactionStack.Push("OK");                           // Activate port sensor
                reactionStack.Push("OK");                           // Enable io status
                reactionStack.Push("ON");                           // Check Io3 status
                reactionStack.Push("OK");                           // Enter local mode
            }
            var stub = Hal as BluetoothHalSequentialStub;

            Assert.IsNotNull(stub);

            stub.DeviceType = deviceType;
            stub.SetUpReactionStack(reactionStack);
        }

    }
}
