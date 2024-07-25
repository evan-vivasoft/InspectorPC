/*
//===============================================================================
// Copyright Kamstrup
// All rights reserved.
//===============================================================================
*/

using System;
using System.Collections.Generic;
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
        private const int EVENT_TIMEOUT = 10000;
        private IHal m_Hal;
        private IInitializationActivityControl m_InitializationActivityControl;
        private ManualResetEvent m_ManualResetEvent = new ManualResetEvent(false);

        private InitializationResult m_FinishResult;
        private int m_FinishErrorCode = -1;
        private InitializationStepResult m_ExpectedStepResult;
        private int m_ExpectedStepErrorCode;
        private string m_ExpectedStepMessage;
        private InitializationManometer m_ExpectedStepManometer;

        private bool m_HasErrorBeenSet = false;
        private int m_Counter = 1;

        #endregion Class Members

        #region Properties
        /// <summary>
        /// Gets or sets the initialization activity control.
        /// </summary>
        /// <value>
        /// The initialization activity control.
        /// </value>
        public IInitializationActivityControl InitializationActivityControl
        {
            get
            {
                if (m_InitializationActivityControl == null)
                {
                    m_InitializationActivityControl = ContextRegistry.Context.Resolve<IInitializationActivityControl>();
                }
                return m_InitializationActivityControl;
            }
            set
            {
                m_InitializationActivityControl = value;
            }
        }

        /// <summary>
        /// Gets or sets the hal
        /// </summary>
        /// <value>
        /// The communication control.
        /// </value>
        public IHal Hal
        {
            get
            {
                if (m_Hal == null)
                {
                    m_Hal = ContextRegistry.Context.Resolve<IHal>();
                }
                return m_Hal;
            }
            set
            {
                m_Hal = value;
            }
        }
        #endregion Properties

        [SetUp]
        public void SetUpSingleTest()
        {
            m_HasErrorBeenSet = false;
            m_Counter = 1;
            m_ExpectedStepResult = InitializationStepResult.UNSET;
            m_ExpectedStepErrorCode = -1;
            m_ExpectedStepMessage = String.Empty;
            m_ExpectedStepManometer = InitializationManometer.BLUETOOTH_MODULE;
        }

        [TestFixtureTearDown]
        public void TestTearDown()
        {
        }

        #region Constructor
        public InitializationTest()
        {

        }
        #endregion Constructor

        #region Event handlers / generic functions
        /// <summary>
        /// Handles the InitializationFinished event of the InitializationActivityControl control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void InitializationActivityControl_InitializationFinished(object sender, EventArgs e)
        {
            Assert.IsInstanceOf<FinishInitializationEventArgs>(e, "Expected FinishInitializationEventArgs");
            FinishInitializationEventArgs eventArgs = e as FinishInitializationEventArgs;
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
            FinishInitializationStepEventArgs eventArgs = e as FinishInitializationStepEventArgs;
            if (eventArgs.StepId == commandId)
            {
                if (!m_HasErrorBeenSet)
                { // Make sure only the first occurrence of the command Id is stored.
                    m_HasErrorBeenSet = true;

                    m_ExpectedStepErrorCode = eventArgs.ErrorCode;
                    m_ExpectedStepManometer = eventArgs.Manometer;
                    m_ExpectedStepMessage = eventArgs.Message;
                    m_ExpectedStepResult = eventArgs.Result;
                }
            }
        }
        #endregion Event handlers


        /// <summary>
        /// Initializations the happy flow test.
        /// </summary>
        [Test]
        public void InitializationHappyFlowTest()
        {
            SetUpHappyFlowStack();
            InitializationActivityControl.InitializationFinished += new EventHandler(InitializationActivityControl_InitializationFinished);

            m_ManualResetEvent.Reset();
            InitializationActivityControl.ExecuteInitialization();
            m_ManualResetEvent.WaitOne(EVENT_TIMEOUT);

            InitializationActivityControl.InitializationFinished -= new EventHandler(InitializationActivityControl_InitializationFinished);

            Assert.AreEqual(InitializationResult.SUCCESS, m_FinishResult);
            Assert.AreEqual(ErrorCodes.INITIALIZATION_FINISHED_SUCCESSFULLY, m_FinishErrorCode);
        }

        /// <summary>
        /// Sets up manometer error stack.
        /// </summary>
        /// <param name="errorCodeToThrow">The error code to throw.</param>
        private void SetUpManometerErrorStack(int errorCodeToThrow)
        {
            Stack<string> reactionStack = new Stack<string>();

            // Close init led after error
            reactionStack.Push("Random data"); // Flush Manometer Cache
            reactionStack.Push("CONNECT Test"); // Exit remote local command mode
            reactionStack.Push("ok"); // Switch Init Led on
            reactionStack.Push("ok"); // Flush Bluetooth cache
            reactionStack.Push("ok"); // Enter remote local command mode

            reactionStack.Push("ManometerError:" + errorCodeToThrow); // Check Battery on TH2, throw error
            reactionStack.Push("ok"); //Check Manometer present
            reactionStack.Push(String.Empty); // Switch to manometer TH2

            reactionStack.Push("Random data"); // Flush Manometer Cache

            reactionStack.Push("CONNECT Test"); // Exit remote local command mode
            reactionStack.Push("ok"); // Switch Init Led on
            reactionStack.Push("ok"); // Flush Bluetooth cache
            reactionStack.Push("ok"); // Enter remote local command mode

            BluetoothHalSequentialStub stub = Hal as BluetoothHalSequentialStub;
            stub.SetUpReactionStack(reactionStack);
        }

        /// <summary>
        /// Sets the happy flow stack.
        /// </summary>
        private void SetUpHappyFlowStack()
        {
            Stack<string> reactionStack = new Stack<string>();
            reactionStack.Push("Random data"); // Flush Manometer Cache

            reactionStack.Push("CONNECT Test"); // Exit remote local command mode
            reactionStack.Push("ok"); // Switch Init Led off
            reactionStack.Push("ok"); // Flush Bluetooth cache
            reactionStack.Push("ok"); // Enter remote local command mode

            reactionStack.Push(String.Empty); // Switch to manometer TH2

            reactionStack.Push("ok"); // set IRDA always on 
            reactionStack.Push("mbar"); // Query pressure unit on TH1
            reactionStack.Push("ok"); // Set Manometer Range on TH1
            reactionStack.Push("\"2000 mbar\""); // Query Manometer Range on TH1
            reactionStack.Push("\"HM3500DLM110,MOD00B,B030504\""); // Identification on TH1
            reactionStack.Push(String.Empty); // Initiate Self test on TH1
            reactionStack.Push("0,\"no current Scpi errors!\""); // Check SCPI on TH1
            reactionStack.Push("80"); // Check Battery on TH1
            reactionStack.Push("ok"); //Check Manometer present
            reactionStack.Push(String.Empty); // Switch to manometer TH1

            reactionStack.Push("ok"); // set IRDA always on 
            reactionStack.Push("mbar"); // Query pressure unit on TH2
            reactionStack.Push("ok"); // Set Manometer Range on TH2
            reactionStack.Push("\"2000 mbar\""); // Query Manometer Range on TH2
            reactionStack.Push("\"HM3500DLM110,MOD00B,B030504\""); // Identification on TH2
            reactionStack.Push(String.Empty); // Initiate Self test on TH2
            reactionStack.Push("0,\"no current Scpi errors!\""); // Check SCPI on TH2
            reactionStack.Push("80"); // Check Battery on TH2
            reactionStack.Push("ok"); //Check Manometer present
            reactionStack.Push(String.Empty); // Switch to manometer TH2

            reactionStack.Push("Random data"); // Flush Manometer Cache

            reactionStack.Push("CONNECT Test"); // Exit remote local command mode
            reactionStack.Push("ok"); // Switch Init Led on
            reactionStack.Push("ok"); // Flush Bluetooth cache
            reactionStack.Push("ok"); // Enter remote local command mode

            BluetoothHalSequentialStub stub = Hal as BluetoothHalSequentialStub;
            stub.SetUpReactionStack(reactionStack);
        }

        /// <summary>
        /// Initializations the enter remote local command mode error test.
        /// </summary>
        [Test]
        public void InitializationEnterRemoteLocalCommandModeErrorTest()
        {
            SetUpEnterRemoteErrorStack();
            InitializationActivityControl.InitializationFinished += new EventHandler(InitializationActivityControl_InitializationFinished);
            InitializationActivityControl.InitializationStepFinished += new EventHandler(InitializationActivityControl_InitializationStepFinished_EnterRemoteLocal);

            m_ManualResetEvent.Reset();
            InitializationActivityControl.ExecuteInitialization();
            m_ManualResetEvent.WaitOne(EVENT_TIMEOUT);

            InitializationActivityControl.InitializationStepFinished -= new EventHandler(InitializationActivityControl_InitializationStepFinished_EnterRemoteLocal);
            InitializationActivityControl.InitializationFinished -= new EventHandler(InitializationActivityControl_InitializationFinished);

            Assert.AreEqual(InitializationResult.ERROR, m_FinishResult);
            Assert.AreEqual(ErrorCodes.INITIALIZATION_FINISHED_ERROR, m_FinishErrorCode);

            Assert.AreEqual(ErrorCodes.INITIALIZATION_STEP_ENTER_REMOTE_ERROR, m_ExpectedStepErrorCode);
            Assert.AreEqual(InitializationManometer.BLUETOOTH_MODULE, m_ExpectedStepManometer);
            Assert.AreEqual("nok", m_ExpectedStepMessage);
            Assert.AreEqual(InitializationStepResult.ERROR, m_ExpectedStepResult);
        }

        void InitializationActivityControl_InitializationStepFinished_EnterRemoteLocal(object sender, EventArgs e)
        {
            SetFirstStepResult(e, DeviceCommand.EnterRemoteLocalCommandMode.ToString());
        }

        /// <summary>
        /// Sets the enter remote error stack.
        /// </summary>
        private void SetUpEnterRemoteErrorStack()
        {
            Stack<string> reactionStack = new Stack<string>();

            // Close init led after error
            reactionStack.Push("Random data"); // Flush Manometer Cache
            reactionStack.Push("CONNECT Test"); // Exit remote local command mode
            reactionStack.Push("ok"); // Switch Init Led on
            reactionStack.Push("ok"); // Flush Bluetooth cache
            reactionStack.Push("ok"); // Enter remote local command mode


            reactionStack.Push("nok"); // Enter remote local command mode

            BluetoothHalSequentialStub stub = Hal as BluetoothHalSequentialStub;
            stub.SetUpReactionStack(reactionStack);
        }

        /// <summary>
        /// Initializations the switch initialization led error test.
        /// </summary>
        [Test]
        public void InitializationSwitchInitializationLedErrorTest()
        {
            SetUpSwitchInitalisationLedStack();
            InitializationActivityControl.InitializationFinished += new EventHandler(InitializationActivityControl_InitializationFinished);
            InitializationActivityControl.InitializationStepFinished += new EventHandler(InitializationActivityControl_InitializationStepFinished_SwitchInitLed);

            m_ManualResetEvent.Reset();
            InitializationActivityControl.ExecuteInitialization();
            m_ManualResetEvent.WaitOne(EVENT_TIMEOUT);

            InitializationActivityControl.InitializationStepFinished -= new EventHandler(InitializationActivityControl_InitializationStepFinished_SwitchInitLed);
            InitializationActivityControl.InitializationFinished -= new EventHandler(InitializationActivityControl_InitializationFinished);

            Assert.AreEqual(InitializationResult.ERROR, m_FinishResult);
            Assert.AreEqual(ErrorCodes.INITIALIZATION_FINISHED_ERROR, m_FinishErrorCode);

            Assert.AreEqual(ErrorCodes.INITIALIZATION_STEP_SWITCH_INITIALIZATION_LED_ERROR, m_ExpectedStepErrorCode);
            Assert.AreEqual(InitializationManometer.BLUETOOTH_MODULE, m_ExpectedStepManometer);
            Assert.AreEqual("nok", m_ExpectedStepMessage);
            Assert.AreEqual(InitializationStepResult.ERROR, m_ExpectedStepResult);
        }

        void InitializationActivityControl_InitializationStepFinished_SwitchInitLed(object sender, EventArgs e)
        {
            SetFirstStepResult(e, DeviceCommand.SwitchInitializationLedOn.ToString());
        }

        /// <summary>
        /// Sets the switch initalisation led stack.
        /// </summary>
        private void SetUpSwitchInitalisationLedStack()
        {
            Stack<string> reactionStack = new Stack<string>();
            // Close init led after error
            reactionStack.Push("Random data"); // Flush Manometer Cache
            reactionStack.Push("CONNECT Test"); // Exit remote local command mode
            reactionStack.Push("ok"); // Switch Init Led on
            reactionStack.Push("ok"); // FlushBluetoothcache
            reactionStack.Push("ok"); // Enter remote local command mode


            reactionStack.Push("nok"); // Switch Init Led to on
            reactionStack.Push("ok"); // FlushBluetoothcache
            reactionStack.Push("ok"); // Enter remote local command mode

            BluetoothHalSequentialStub stub = Hal as BluetoothHalSequentialStub;
            stub.SetUpReactionStack(reactionStack);
        }

        /// <summary>
        /// Initializations the exit remote local command mode error test.
        /// </summary>
        [Test]
        public void InitializationExitRemoteLocalCommandModeErrorTest()
        {
            SetUpExitRemoteErrorStack();
            InitializationActivityControl.InitializationFinished += new EventHandler(InitializationActivityControl_InitializationFinished);
            InitializationActivityControl.InitializationStepFinished += new EventHandler(InitializationActivityControl_InitializationStepFinished_ExitRemoteLocal);

            m_ManualResetEvent.Reset();
            InitializationActivityControl.ExecuteInitialization();
            m_ManualResetEvent.WaitOne(EVENT_TIMEOUT);

            InitializationActivityControl.InitializationStepFinished -= new EventHandler(InitializationActivityControl_InitializationStepFinished_ExitRemoteLocal);
            InitializationActivityControl.InitializationFinished -= new EventHandler(InitializationActivityControl_InitializationFinished);

            Assert.AreEqual(InitializationResult.ERROR, m_FinishResult);
            Assert.AreEqual(ErrorCodes.INITIALIZATION_FINISHED_ERROR, m_FinishErrorCode);

            Assert.AreEqual(ErrorCodes.INITIALIZATION_STEP_EXIT_REMOTE_ERROR, m_ExpectedStepErrorCode);
            Assert.AreEqual(InitializationManometer.BLUETOOTH_MODULE, m_ExpectedStepManometer);
            Assert.AreEqual("ERROR nn", m_ExpectedStepMessage);
            Assert.AreEqual(InitializationStepResult.ERROR, m_ExpectedStepResult);
        }

        void InitializationActivityControl_InitializationStepFinished_ExitRemoteLocal(object sender, EventArgs e)
        {
            SetFirstStepResult(e, DeviceCommand.ExitRemoteLocalCommandMode.ToString());
        }

        /// <summary>
        /// Sets the exit remote error stack.
        /// </summary>
        private void SetUpExitRemoteErrorStack()
        {
            Stack<string> reactionStack = new Stack<string>();
            // Close init led after error
            reactionStack.Push("Random data"); // Flush Manometer Cache
            reactionStack.Push("CONNECT Test"); // Exit remote local command mode
            reactionStack.Push("ok"); // Switch Init Led on
            reactionStack.Push("ok"); // FlushBluetoothcache
            reactionStack.Push("ok"); // Enter remote local command mode


            reactionStack.Push("ERROR nn"); // Exit remote local command mode
            reactionStack.Push("ok"); // Switch Init Led to on
            reactionStack.Push("ok"); // FlushBluetoothcache
            reactionStack.Push("ok"); // Enter remote local command mode

            BluetoothHalSequentialStub stub = Hal as BluetoothHalSequentialStub;
            stub.SetUpReactionStack(reactionStack);
        }


        /// <summary>
        /// Initializations the battery level warning test.
        /// </summary>
        [Test]
        public void InitializationBatteryLevelWarningTest()
        {
            m_HasErrorBeenSet = false;
            SetUpBatteryLevelWarningStack();
            InitializationActivityControl.InitializationFinished += new EventHandler(InitializationActivityControl_InitializationFinished);
            InitializationActivityControl.InitializationStepFinished += new EventHandler(InitializationActivityControl_InitializationStepFinished_BatteryLevelWarning);

            m_ManualResetEvent.Reset();
            InitializationActivityControl.ExecuteInitialization();
            m_ManualResetEvent.WaitOne(EVENT_TIMEOUT);

            InitializationActivityControl.InitializationFinished -= new EventHandler(InitializationActivityControl_InitializationFinished);
            InitializationActivityControl.InitializationStepFinished -= new EventHandler(InitializationActivityControl_InitializationStepFinished_BatteryLevelWarning);

            Assert.AreEqual(InitializationResult.WARNING, m_FinishResult);
            Assert.AreEqual(ErrorCodes.INITIALIZATION_FINISHED_WARNING, m_FinishErrorCode);

            Assert.AreEqual(ErrorCodes.INITIALIZATION_STEP_BATTERYLIMIT_LEVEL_WARNING, m_ExpectedStepErrorCode);
            Assert.AreEqual(InitializationManometer.TH2, m_ExpectedStepManometer);
            Assert.AreEqual("60", m_ExpectedStepMessage);
            Assert.AreEqual(InitializationStepResult.WARNING, m_ExpectedStepResult);
        }

        void InitializationActivityControl_InitializationStepFinished_BatteryLevelWarning(object sender, EventArgs e)
        {
            SetFirstStepResult(e, DeviceCommand.CheckBatteryStatus.ToString());
        }

        /// <summary>
        /// Sets the battery level warning stack.
        /// </summary>
        private void SetUpBatteryLevelWarningStack()
        {
            Stack<string> reactionStack = new Stack<string>();

            reactionStack.Push("Random data"); // Flush Manometer Cache

            reactionStack.Push("CONNECT Test"); // Exit remote local command mode
            reactionStack.Push("ok"); // Switch Init Led off
            reactionStack.Push("ok"); // FlushBluetoothcache
            reactionStack.Push("ok"); // Enter remote local command mode

            reactionStack.Push(String.Empty); // Switch to manometer TH2

            reactionStack.Push("ok"); // set IRDA always on 
            reactionStack.Push("mbar"); // Query pressure unit on TH1
            reactionStack.Push("ok"); // Set Manometer Range on TH1
            reactionStack.Push("\"17 bar\""); // Query Manometer Range on TH1
            reactionStack.Push("\"HM3500DLM110,MOD00B,B030504\""); // Identification on TH1
            reactionStack.Push(String.Empty); // Initiate Self test on TH1
            reactionStack.Push("0,\"no current Scpi errors!\""); // Check SCPI on TH1
            reactionStack.Push("80"); // Check Battery on TH1
            reactionStack.Push("ok"); //Check Manometer present
            reactionStack.Push(String.Empty); // Switch to manometer TH1

            reactionStack.Push("ok"); // set IRDA always on 
            reactionStack.Push("mbar"); // Query pressure unit on TH2
            reactionStack.Push("ok"); // Set Manometer Range on TH2
            reactionStack.Push("\"2000 mbar\""); // Query Manometer Range on TH2
            reactionStack.Push("\"HM3500DLM110,MOD00B,B030504\""); // Identification on TH2
            reactionStack.Push(String.Empty); // Initiate Self test on TH2
            reactionStack.Push("0,\"no current Scpi errors!\""); // Check SCPI on TH2
            reactionStack.Push("60"); // Check Battery on TH2
            reactionStack.Push("ok"); //Check Manometer present
            reactionStack.Push(String.Empty); // Switch to manometer TH2

            reactionStack.Push("Random data"); // Flush Manometer Cache

            reactionStack.Push("CONNECT Test"); // Exit remote local command mode
            reactionStack.Push("ok"); // Switch Init Led on
            reactionStack.Push("ok"); // FlushBluetoothcache
            reactionStack.Push("ok"); // Enter remote local command mode

            BluetoothHalSequentialStub stub = Hal as BluetoothHalSequentialStub;
            stub.SetUpReactionStack(reactionStack);
        }

        /// <summary>
        /// Initializations the battery level format error test.
        /// </summary>
        [Test]
        public void InitializationBatteryLevelFormatErrorTest()
        {
            SetUpBatteryLevelFormatErrorStack();
            InitializationActivityControl.InitializationFinished += new EventHandler(InitializationActivityControl_InitializationFinished);
            InitializationActivityControl.InitializationStepFinished += new EventHandler(InitializationActivityControl_InitializationStepFinished_BatteryLevelFormat);

            m_ManualResetEvent.Reset();
            InitializationActivityControl.ExecuteInitialization();
            m_ManualResetEvent.WaitOne(EVENT_TIMEOUT);

            InitializationActivityControl.InitializationStepFinished -= new EventHandler(InitializationActivityControl_InitializationStepFinished_BatteryLevelFormat);
            InitializationActivityControl.InitializationFinished -= new EventHandler(InitializationActivityControl_InitializationFinished);

            Assert.AreEqual(InitializationResult.ERROR, m_FinishResult);
            Assert.AreEqual(ErrorCodes.INITIALIZATION_FINISHED_ERROR, m_FinishErrorCode);

            Assert.AreEqual(ErrorCodes.INITIALIZATION_STEP_BATTERYLIMIT_FORMAT_ERROR, m_ExpectedStepErrorCode);
            Assert.AreEqual(InitializationManometer.TH2, m_ExpectedStepManometer);
            Assert.AreEqual("60a", m_ExpectedStepMessage);
            Assert.AreEqual(InitializationStepResult.ERROR, m_ExpectedStepResult);
        }

        void InitializationActivityControl_InitializationStepFinished_BatteryLevelFormat(object sender, EventArgs e)
        {
            SetFirstStepResult(e, DeviceCommand.CheckBatteryStatus.ToString());
        }

        /// <summary>
        /// Sets the battery level format error stack.
        /// </summary>
        private void SetUpBatteryLevelFormatErrorStack()
        {
            Stack<string> reactionStack = new Stack<string>();
            // Close init led after error
            reactionStack.Push("Random data"); // Flush Manometer Cache
            reactionStack.Push("CONNECT Test"); // Exit remote local command mode
            reactionStack.Push("ok"); // Switch Init Led on
            reactionStack.Push("ok"); // FlushBluetoothcache
            reactionStack.Push("ok"); // Enter remote local command mode


            reactionStack.Push("60a"); // Check Battery on TH2
            reactionStack.Push("ok"); //Check Manometer present
            reactionStack.Push(String.Empty); // Switch to manometer TH2

            reactionStack.Push("Random data"); // Flush Manometer Cache

            reactionStack.Push("CONNECT Test"); // Exit remote local command mode
            reactionStack.Push("ok"); // Switch Init Led on
            reactionStack.Push("ok"); // FlushBluetoothcache
            reactionStack.Push("ok"); // Enter remote local command mode

            BluetoothHalSequentialStub stub = Hal as BluetoothHalSequentialStub;
            stub.SetUpReactionStack(reactionStack);
        }


        /// <summary>
        /// Initializations the SCPI interface error test.
        /// </summary>
        [Test]
        public void InitializationSCPIInterfaceErrorTest()
        {
            SetUpSCPIErrorStack();
            InitializationActivityControl.InitializationFinished += new EventHandler(InitializationActivityControl_InitializationFinished);
            InitializationActivityControl.InitializationStepFinished += new EventHandler(InitializationActivityControl_InitializationStepFinished_SCPIError);

            m_ManualResetEvent.Reset();
            InitializationActivityControl.ExecuteInitialization();
            m_ManualResetEvent.WaitOne(EVENT_TIMEOUT);

            InitializationActivityControl.InitializationStepFinished -= new EventHandler(InitializationActivityControl_InitializationStepFinished_SCPIError);
            InitializationActivityControl.InitializationFinished -= new EventHandler(InitializationActivityControl_InitializationFinished);

            Assert.AreEqual(InitializationResult.ERROR, m_FinishResult);
            Assert.AreEqual(ErrorCodes.INITIALIZATION_FINISHED_ERROR, m_FinishErrorCode);

            Assert.AreEqual(ErrorCodes.INITIALIZATION_STEP_SCPI_ERROR, m_ExpectedStepErrorCode);
            Assert.AreEqual(InitializationManometer.TH2, m_ExpectedStepManometer);
            Assert.AreEqual("1,\"E1\"", m_ExpectedStepMessage);
            Assert.AreEqual(InitializationStepResult.ERROR, m_ExpectedStepResult);
        }

        void InitializationActivityControl_InitializationStepFinished_SCPIError(object sender, EventArgs e)
        {
            Assert.IsInstanceOf<FinishInitializationStepEventArgs>(e, "Expected FinishInitializationStepEventArgs");
            FinishInitializationStepEventArgs eventArgs = e as FinishInitializationStepEventArgs;
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
        private void SetUpSCPIErrorStack()
        {
            Stack<string> reactionStack = new Stack<string>();
            // Close init led after error
            reactionStack.Push("Random data"); // Flush Manometer Cache
            reactionStack.Push("CONNECT Test"); // Exit remote local command mode
            reactionStack.Push("ok"); // Switch Init Led on
            reactionStack.Push("ok"); // FlushBluetoothcache
            reactionStack.Push("ok"); // Enter remote local command mode



            reactionStack.Push("1,\"E1\""); // Check SCPI on TH2
            reactionStack.Push("2,\"E2\""); // Check SCPI on TH2
            reactionStack.Push("3,\"E3\""); // Check SCPI on TH2
            reactionStack.Push("4,\"E4\""); // Check SCPI on TH2
            reactionStack.Push("5,\"E5\""); // Check SCPI on TH2
            reactionStack.Push("6,\"E6\""); // Check SCPI on TH2
            reactionStack.Push("80"); // Check Battery on TH2
            reactionStack.Push("ok"); //Check Manometer present
            reactionStack.Push(String.Empty); // Switch to manometer TH2

            reactionStack.Push("Random data"); // Flush Manometer Cache

            reactionStack.Push("CONNECT Test"); // Exit remote local command mode
            reactionStack.Push("ok"); // Switch Init Led on
            reactionStack.Push("ok"); // FlushBluetoothcache
            reactionStack.Push("ok"); // Enter remote local command mode

            BluetoothHalSequentialStub stub = Hal as BluetoothHalSequentialStub;
            stub.SetUpReactionStack(reactionStack);
        }

        /// <summary>
        /// Initializations the SCPI interface warning test.
        /// </summary>
        [Test]
        public void InitializationSCPIInterfaceWarningTest()
        {
            SetUpSCPIWarningStack();
            InitializationActivityControl.InitializationFinished += new EventHandler(InitializationActivityControl_InitializationFinished);
            InitializationActivityControl.InitializationStepFinished += new EventHandler(InitializationActivityControl_InitializationStepFinished_CheckSCPIWarning);

            m_ManualResetEvent.Reset();
            InitializationActivityControl.ExecuteInitialization();
            m_ManualResetEvent.WaitOne(EVENT_TIMEOUT);

            InitializationActivityControl.InitializationStepFinished -= new EventHandler(InitializationActivityControl_InitializationStepFinished_CheckSCPIWarning);
            InitializationActivityControl.InitializationFinished -= new EventHandler(InitializationActivityControl_InitializationFinished);

            Assert.AreEqual(InitializationResult.WARNING, m_FinishResult);
            Assert.AreEqual(ErrorCodes.INITIALIZATION_FINISHED_WARNING, m_FinishErrorCode);

            Assert.AreEqual(ErrorCodes.INITIALIZATION_STEP_EXECUTED_SUCCESSFULLY, m_ExpectedStepErrorCode);
            Assert.AreEqual(InitializationManometer.TH2, m_ExpectedStepManometer);
            Assert.AreEqual("0,\"no current Scpi errors!\"", m_ExpectedStepMessage);
            Assert.AreEqual(InitializationStepResult.SUCCESS, m_ExpectedStepResult);
        }

        void InitializationActivityControl_InitializationStepFinished_CheckSCPIWarning(object sender, EventArgs e)
        {
            Assert.IsInstanceOf<FinishInitializationStepEventArgs>(e, "Expected FinishInitializationStepEventArgs");
            FinishInitializationStepEventArgs eventArgs = e as FinishInitializationStepEventArgs;
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
        private void SetUpSCPIWarningStack()
        {
            Stack<string> reactionStack = new Stack<string>();
            reactionStack.Push("Random data"); // Flush Manometer Cache

            reactionStack.Push("CONNECT Test"); // Exit remote local command mode
            reactionStack.Push("ok"); // Switch Init Led off
            reactionStack.Push("ok"); // FlushBluetoothcache
            reactionStack.Push("ok"); // Enter remote local command mode

            reactionStack.Push(String.Empty); // Switch to manometer TH2

            reactionStack.Push("ok"); // set IRDA always on 
            reactionStack.Push("mbar"); // Query pressure unit on TH1
            reactionStack.Push("ok"); // Set Manometer Range on TH1
            reactionStack.Push("\"2000 mbar\""); // Query Manometer Range on TH1
            reactionStack.Push("\"HM3500DLM110,MOD00B,B030504\""); // Identification on TH1
            reactionStack.Push(String.Empty); // Initiate Self test on TH1
            reactionStack.Push("0,\"no current Scpi errors!\""); // Check SCPI on TH1
            reactionStack.Push("80"); // Check Battery on TH1
            reactionStack.Push("ok"); //Check Manometer present
            reactionStack.Push(String.Empty); // Switch to manometer TH1

            reactionStack.Push("ok"); // set IRDA always on 
            reactionStack.Push("mbar"); // Query pressure unit on TH2
            reactionStack.Push("ok"); // Set Manometer Range on TH2
            reactionStack.Push("\"2000 mbar\""); // Query Manometer Range on TH2
            reactionStack.Push("\"HM3500DLM110,MOD00B,B030504\""); // Identification on TH2
            reactionStack.Push(String.Empty); // Initiate Self test on TH2
            reactionStack.Push("0,\"no current Scpi errors!\""); // Check SCPI on TH2
            reactionStack.Push("2,\"Other Error\""); // Check SCPI on TH2
            reactionStack.Push("1,\"Error\""); // Check SCPI on TH2
            reactionStack.Push("80"); // Check Battery on TH2
            reactionStack.Push("ok"); //Check Manometer present
            reactionStack.Push(String.Empty); // Switch to manometer TH2

            reactionStack.Push("Random data"); // Flush Manometer Cache

            reactionStack.Push("CONNECT Test"); // Exit remote local command mode
            reactionStack.Push("ok"); // Switch Init Led on
            reactionStack.Push("ok"); // FlushBluetoothcache
            reactionStack.Push("ok"); // Enter remote local command mode

            BluetoothHalSequentialStub stub = Hal as BluetoothHalSequentialStub;
            stub.SetUpReactionStack(reactionStack);
        }

        /// <summary>
        /// Initializations the set pressure unit error test.
        /// </summary>
        [Test]
        public void InitializationSetPressureUnitErrorTest()
        {
            SetUpSetPressureUnitErrorStack();
            InitializationActivityControl.InitializationFinished += new EventHandler(InitializationActivityControl_InitializationFinished);
            InitializationActivityControl.InitializationStepFinished += new EventHandler(InitializationActivityControl_InitializationStepFinished_SetPressureUnit);

            m_ManualResetEvent.Reset();
            InitializationActivityControl.ExecuteInitialization();
            m_ManualResetEvent.WaitOne(EVENT_TIMEOUT);

            InitializationActivityControl.InitializationStepFinished -= new EventHandler(InitializationActivityControl_InitializationStepFinished_SetPressureUnit);
            InitializationActivityControl.InitializationFinished -= new EventHandler(InitializationActivityControl_InitializationFinished);

            Assert.AreEqual(InitializationResult.ERROR, m_FinishResult);
            Assert.AreEqual(ErrorCodes.INITIALIZATION_FINISHED_ERROR, m_FinishErrorCode);

            Assert.AreEqual(ErrorCodes.INITIALIZATION_STEP_SET_PRESSURE_UNIT_ERROR, m_ExpectedStepErrorCode);
            Assert.AreEqual(InitializationManometer.TH2, m_ExpectedStepManometer);
            Assert.AreEqual("nok", m_ExpectedStepMessage);
            Assert.AreEqual(InitializationStepResult.ERROR, m_ExpectedStepResult);
        }

        void InitializationActivityControl_InitializationStepFinished_SetPressureUnit(object sender, EventArgs e)
        {
            SetFirstStepResult(e, DeviceCommand.SetPressureUnit.ToString());
        }

        /// <summary>
        /// Sets the set pressure unit error stack.
        /// </summary>
        private void SetUpSetPressureUnitErrorStack()
        {
            Stack<string> reactionStack = new Stack<string>();
            // Close init led after error
            reactionStack.Push("Random data"); // Flush Manometer Cache
            reactionStack.Push("CONNECT Test"); // Exit remote local command mode
            reactionStack.Push("ok"); // Switch Init Led on
            reactionStack.Push("ok"); // FlushBluetoothcache
            reactionStack.Push("ok"); // Enter remote local command mode



            reactionStack.Push("nok"); // Set Manometer Range on TH2
            reactionStack.Push("\"2000 mbar\""); // Query Manometer Range on TH2
            reactionStack.Push("\"HM3500DLM110,MOD00B,B030504\""); // Identification on TH2
            reactionStack.Push(String.Empty); // Initiate Self test on TH2
            reactionStack.Push("0,\"no current Scpi errors!\""); // Check SCPI on TH2
            reactionStack.Push("80"); // Check Battery on TH2
            reactionStack.Push("ok"); //Check Manometer present
            reactionStack.Push(String.Empty); // Switch to manometer TH2

            reactionStack.Push("Random data"); // Flush Manometer Cache

            reactionStack.Push("CONNECT Test"); // Exit remote local command mode
            reactionStack.Push("ok"); // Switch Init Led on
            reactionStack.Push("ok"); // FlushBluetoothcache
            reactionStack.Push("ok"); // Enter remote local command mode

            BluetoothHalSequentialStub stub = Hal as BluetoothHalSequentialStub;
            stub.SetUpReactionStack(reactionStack);
        }

        /// <summary>
        /// Initializations the query manometer range error test.
        /// </summary>
        [Test]
        public void InitializationQueryManometerRangeErrorTest()
        {
            SetUpQueryManometerRangeErrorStack();
            InitializationActivityControl.InitializationFinished += new EventHandler(InitializationActivityControl_InitializationFinished);
            InitializationActivityControl.InitializationStepFinished += new EventHandler(InitializationActivityControl_InitializationStepFinished_ManometerRange);

            m_ManualResetEvent.Reset();
            InitializationActivityControl.ExecuteInitialization();
            m_ManualResetEvent.WaitOne(EVENT_TIMEOUT);

            InitializationActivityControl.InitializationStepFinished -= new EventHandler(InitializationActivityControl_InitializationStepFinished_ManometerRange);
            InitializationActivityControl.InitializationFinished -= new EventHandler(InitializationActivityControl_InitializationFinished);

            Assert.AreEqual(InitializationResult.ERROR, m_FinishResult);
            Assert.AreEqual(ErrorCodes.INITIALIZATION_FINISHED_ERROR, m_FinishErrorCode);

            Assert.AreEqual(ErrorCodes.INITIALIZATION_STEP_MANOMETER_RANGE_ERROR, m_ExpectedStepErrorCode);
            Assert.AreEqual(InitializationManometer.TH2, m_ExpectedStepManometer);
            Assert.AreEqual("\"2000\"", m_ExpectedStepMessage);
            Assert.AreEqual(InitializationStepResult.ERROR, m_ExpectedStepResult);
        }

        void InitializationActivityControl_InitializationStepFinished_ManometerRange(object sender, EventArgs e)
        {
            SetFirstStepResult(e, DeviceCommand.CheckRange.ToString());
        }

        /// <summary>
        /// Sets the query manometer range error stack.
        /// </summary>
        private void SetUpQueryManometerRangeErrorStack()
        {
            Stack<string> reactionStack = new Stack<string>();
            // Close init led after error
            reactionStack.Push("Random data"); // Flush Manometer Cache
            reactionStack.Push("CONNECT Test"); // Exit remote local command mode
            reactionStack.Push("ok"); // Switch Init Led on
            reactionStack.Push("ok"); // FlushBluetoothcache
            reactionStack.Push("ok"); // Enter remote local command mode



            reactionStack.Push("\"2000\""); // Query Manometer Range on TH2
            reactionStack.Push("\"HM3500DLM110,MOD00B,B030504\""); // Identification on TH2
            reactionStack.Push(String.Empty); // Initiate Self test on TH2
            reactionStack.Push("0,\"no current Scpi errors!\""); // Check SCPI on TH2
            reactionStack.Push("80"); // Check Battery on TH2
            reactionStack.Push("ok"); //Check Manometer present
            reactionStack.Push(String.Empty); // Switch to manometer TH2

            reactionStack.Push("Random data"); // Flush Manometer Cache

            reactionStack.Push("CONNECT Test"); // Exit remote local command mode
            reactionStack.Push("ok"); // Switch Init Led on
            reactionStack.Push("ok"); // FlushBluetoothcache
            reactionStack.Push("ok"); // Enter remote local command mode

            BluetoothHalSequentialStub stub = Hal as BluetoothHalSequentialStub;
            stub.SetUpReactionStack(reactionStack);
        }

        /// <summary>
        /// Initializations the initiate self test error test.
        /// </summary>
        [Test]
        public void InitializationInitiateSelfTestErrorTest()
        {
            SetUpInitiateSelfTestErrorStack();
            InitializationActivityControl.InitializationFinished += new EventHandler(InitializationActivityControl_InitializationFinished);
            InitializationActivityControl.InitializationStepFinished += new EventHandler(InitializationActivityControl_InitializationStepFinished_SelfTest);

            m_ManualResetEvent.Reset();
            InitializationActivityControl.ExecuteInitialization();
            m_ManualResetEvent.WaitOne(EVENT_TIMEOUT);

            InitializationActivityControl.InitializationStepFinished -= new EventHandler(InitializationActivityControl_InitializationStepFinished_SelfTest);
            InitializationActivityControl.InitializationFinished -= new EventHandler(InitializationActivityControl_InitializationFinished);

            Assert.AreEqual(InitializationResult.ERROR, m_FinishResult);
            Assert.AreEqual(ErrorCodes.INITIALIZATION_FINISHED_ERROR, m_FinishErrorCode);

            Assert.AreEqual(ErrorCodes.INITIALIZATION_STEP_SELF_TEST_ERROR, m_ExpectedStepErrorCode);
            Assert.AreEqual(InitializationManometer.TH2, m_ExpectedStepManometer);
            Assert.AreEqual("nok", m_ExpectedStepMessage);
            Assert.AreEqual(InitializationStepResult.ERROR, m_ExpectedStepResult);
        }

        void InitializationActivityControl_InitializationStepFinished_SelfTest(object sender, EventArgs e)
        {
            SetFirstStepResult(e, DeviceCommand.InitiateSelfTest.ToString());
        }

        /// <summary>
        /// Sets the initiate self test error stack.
        /// </summary>
        private void SetUpInitiateSelfTestErrorStack()
        {
            Stack<string> reactionStack = new Stack<string>();
            // Close init led after error
            reactionStack.Push("Random data"); // Flush Manometer Cache
            reactionStack.Push("CONNECT Test"); // Exit remote local command mode
            reactionStack.Push("ok"); // Switch Init Led on
            reactionStack.Push("ok"); // FlushBluetoothcache
            reactionStack.Push("ok"); // Enter remote local command mode



            reactionStack.Push("nok"); // Initiate Self test on TH2
            reactionStack.Push("0,\"no current Scpi errors!\""); // Check SCPI on TH2
            reactionStack.Push("80"); // Check Battery on TH2
            reactionStack.Push("ok"); //Check Manometer present
            reactionStack.Push(String.Empty); // Switch to manometer TH2

            reactionStack.Push("Random data"); // Flush Manometer Cache

            reactionStack.Push("CONNECT Test"); // Exit remote local command mode
            reactionStack.Push("ok"); // Switch Init Led on
            reactionStack.Push("ok"); // FlushBluetoothcache
            reactionStack.Push("ok"); // Enter remote local command mode

            BluetoothHalSequentialStub stub = Hal as BluetoothHalSequentialStub;
            stub.SetUpReactionStack(reactionStack);
        }

        /// <summary>
        /// Initializations the initiate self test error test.
        /// </summary>
        [Test]
        public void InitializationConnectErrorTest()
        {
            SetUpConnectErrorStack();
            InitializationActivityControl.InitializationFinished += new EventHandler(InitializationActivityControl_InitializationFinished);
            InitializationActivityControl.InitializationStepFinished += new EventHandler(InitializationActivityControl_InitializationStepFinished_Connect);

            m_ManualResetEvent.Reset();
            InitializationActivityControl.ExecuteInitialization();
            m_ManualResetEvent.WaitOne(EVENT_TIMEOUT);

            InitializationActivityControl.InitializationStepFinished -= new EventHandler(InitializationActivityControl_InitializationStepFinished_Connect);
            InitializationActivityControl.InitializationFinished -= new EventHandler(InitializationActivityControl_InitializationFinished);

            Assert.AreEqual(InitializationResult.ERROR, m_FinishResult);
            Assert.AreEqual(ErrorCodes.INITIALIZATION_FINISHED_ERROR, m_FinishErrorCode);

            Assert.AreEqual(100, m_ExpectedStepErrorCode);
            Assert.AreEqual(InitializationManometer.BLUETOOTH_MODULE, m_ExpectedStepManometer);
            Assert.AreEqual(String.Empty, m_ExpectedStepMessage);
            Assert.AreEqual(InitializationStepResult.ERROR, m_ExpectedStepResult);
        }

        void InitializationActivityControl_InitializationStepFinished_Connect(object sender, EventArgs e)
        {
            SetFirstStepResult(e, DeviceCommand.Connect.ToString());
        }

        /// <summary>
        /// Sets up connect error stack.
        /// </summary>
        private void SetUpConnectErrorStack()
        {
            BluetoothHalSequentialStub stub = Hal as BluetoothHalSequentialStub;
            stub.SetConnectError(true);
        }

        public void InitializationErrorAfterErrorTest()
        {
            SetUpErrorAfterErrorStack();
            InitializationActivityControl.InitializationFinished += new EventHandler(InitializationActivityControl_InitializationFinished);

            m_ManualResetEvent.Reset();
            InitializationActivityControl.ExecuteInitialization();
            m_ManualResetEvent.WaitOne(EVENT_TIMEOUT);

            InitializationActivityControl.InitializationFinished -= new EventHandler(InitializationActivityControl_InitializationFinished);

            Assert.AreEqual(InitializationResult.ERROR, m_FinishResult);
            Assert.AreEqual(ErrorCodes.INITIALIZATION_FINISHED_ERROR, m_FinishErrorCode);
        }

        /// <summary>
        /// Sets the initiate self test error stack.
        /// </summary>
        private void SetUpErrorAfterErrorStack()
        {
            Stack<string> reactionStack = new Stack<string>();
            // Close init led after error
            reactionStack.Push("nok"); // Switch Init Led off
            reactionStack.Push("ok"); // FlushBluetoothcache
            reactionStack.Push("ok"); // Enter remote local command mode

            reactionStack.Push("nok"); // Initiate Self test on TH2
            reactionStack.Push("0,\"no current Scpi errors!\""); // Check SCPI on TH2
            reactionStack.Push("80"); // Check Battery on TH2
            reactionStack.Push("ok"); //Check Manometer present
            reactionStack.Push(String.Empty); // Switch to manometer TH2

            reactionStack.Push("Random data"); // Flush Manometer Cache

            reactionStack.Push("CONNECT Test"); // Exit remote local command mode
            reactionStack.Push("ok"); // Switch Init Led on
            reactionStack.Push("ok"); // FlushBluetoothcache
            reactionStack.Push("ok"); // Enter remote local command mode

            BluetoothHalSequentialStub stub = Hal as BluetoothHalSequentialStub;
            stub.SetUpReactionStack(reactionStack);
        }

        [Test, Sequential]
        public void ManometerThrowsErrorTest([Values(1600, 1601, 1602, 1603, 1604, 1605, 1606, 1607, 1608)]int errorCodeToThrow)
        {
            SetUpManometerErrorStack(errorCodeToThrow);
            InitializationActivityControl.InitializationFinished += new EventHandler(InitializationActivityControl_InitializationFinished);
            InitializationActivityControl.InitializationStepFinished += new EventHandler(InitializationActivityControl_InitializationStepFinished_Error);

            m_ManualResetEvent.Reset();
            InitializationActivityControl.ExecuteInitialization();
            m_ManualResetEvent.WaitOne(EVENT_TIMEOUT);

            InitializationActivityControl.InitializationStepFinished -= new EventHandler(InitializationActivityControl_InitializationStepFinished_Error);
            InitializationActivityControl.InitializationFinished -= new EventHandler(InitializationActivityControl_InitializationFinished);

            Assert.AreEqual(InitializationResult.ERROR, m_FinishResult);
            Assert.AreEqual(ErrorCodes.INITIALIZATION_FINISHED_ERROR, m_FinishErrorCode);

            Assert.AreEqual(errorCodeToThrow, m_ExpectedStepErrorCode);
            Assert.AreEqual(InitializationManometer.TH2, m_ExpectedStepManometer);
            Assert.AreEqual(String.Empty, m_ExpectedStepMessage);
            Assert.AreEqual(InitializationStepResult.ERROR, m_ExpectedStepResult);
        }

        /// <summary>
        /// Initializations the battery level format error test.
        /// </summary>
        [Test]
        public void InitializationErrorInStepTest()
        {
            SetUpErrorStack();
            InitializationActivityControl.InitializationFinished += new EventHandler(InitializationActivityControl_InitializationFinished);
            InitializationActivityControl.InitializationStepFinished += new EventHandler(InitializationActivityControl_InitializationStepFinished_Error);

            m_ManualResetEvent.Reset();
            InitializationActivityControl.ExecuteInitialization();
            m_ManualResetEvent.WaitOne(EVENT_TIMEOUT);

            InitializationActivityControl.InitializationStepFinished -= new EventHandler(InitializationActivityControl_InitializationStepFinished_Error);
            InitializationActivityControl.InitializationFinished -= new EventHandler(InitializationActivityControl_InitializationFinished);

            Assert.AreEqual(InitializationResult.ERROR, m_FinishResult);
            Assert.AreEqual(ErrorCodes.INITIALIZATION_FINISHED_ERROR, m_FinishErrorCode);

            Assert.AreEqual(100, m_ExpectedStepErrorCode);
            Assert.AreEqual(InitializationManometer.TH2, m_ExpectedStepManometer);
            Assert.AreEqual(String.Empty, m_ExpectedStepMessage);
            Assert.AreEqual(InitializationStepResult.ERROR, m_ExpectedStepResult);
        }

        void InitializationActivityControl_InitializationStepFinished_Error(object sender, EventArgs e)
        {
            SetFirstStepResult(e, DeviceCommand.CheckBatteryStatus.ToString());
        }

        /// <summary>
        /// Sets the battery level format error stack.
        /// </summary>
        private void SetUpErrorStack()
        {
            Stack<string> reactionStack = new Stack<string>();
            // Close init led after error
            reactionStack.Push("Random data"); // Flush Manometer Cache
            reactionStack.Push("CONNECT Test"); // Exit remote local command mode
            reactionStack.Push("ok"); // Switch Init Led on
            reactionStack.Push("ok"); // FlushBluetoothcache
            reactionStack.Push("ok"); // Enter remote local command mode


            reactionStack.Push("Error:100"); // Check Battery on TH2
            reactionStack.Push("ok"); //Check Manometer present
            reactionStack.Push(String.Empty); // Switch to manometer TH2

            reactionStack.Push("Random data"); // Flush Manometer Cache

            reactionStack.Push("CONNECT Test"); // Exit remote local command mode
            reactionStack.Push("ok"); // Switch Init Led on
            reactionStack.Push("ok"); // FlushBluetoothcache
            reactionStack.Push("ok"); // Enter remote local command mode

            BluetoothHalSequentialStub stub = Hal as BluetoothHalSequentialStub;
            stub.SetUpReactionStack(reactionStack);
        }

        /// <summary>
        /// Initializations the battery level warning test.
        /// </summary>
        [Test]
        public void InitializationTimeoutTest()
        {
            m_HasErrorBeenSet = false;
            SetUpTimeoutStack();
            InitializationActivityControl.InitializationFinished += new EventHandler(InitializationActivityControl_InitializationFinished);
            InitializationActivityControl.InitializationStepFinished += new EventHandler(InitializationActivityControl_InitializationStepFinished_Timeout);

            m_ManualResetEvent.Reset();
            InitializationActivityControl.ExecuteInitialization();
            m_ManualResetEvent.WaitOne(EVENT_TIMEOUT);

            InitializationActivityControl.InitializationFinished -= new EventHandler(InitializationActivityControl_InitializationFinished);
            InitializationActivityControl.InitializationStepFinished -= new EventHandler(InitializationActivityControl_InitializationStepFinished_Timeout);

            Assert.AreEqual(InitializationResult.TIMEOUT, m_FinishResult);
            Assert.AreEqual(ErrorCodes.INITIALIZATION_FINISHED_TIMEOUT, m_FinishErrorCode);

            Assert.AreEqual(ErrorCodes.HAL_COMMAND_TIMEOUT_RECEIVED, m_ExpectedStepErrorCode);
            Assert.AreEqual(InitializationManometer.TH2, m_ExpectedStepManometer);
            Assert.AreEqual(String.Empty, m_ExpectedStepMessage);
            Assert.AreEqual(InitializationStepResult.TIMEOUT, m_ExpectedStepResult);
        }

        void InitializationActivityControl_InitializationStepFinished_Timeout(object sender, EventArgs e)
        {
            SetFirstStepResult(e, DeviceCommand.CheckBatteryStatus.ToString());
        }

        /// <summary>
        /// Sets the battery level warning stack.
        /// </summary>
        private void SetUpTimeoutStack()
        {
            Stack<string> reactionStack = new Stack<string>();

            reactionStack.Push("Random data"); // Flush Manometer Cache

            reactionStack.Push("CONNECT Test"); // Exit remote local command mode
            reactionStack.Push("ok"); // Switch Init Led off
            reactionStack.Push("ok"); // FlushBluetoothcache
            reactionStack.Push("ok"); // Enter remote local command mode
            reactionStack.Push(String.Empty); // Switch to manometer TH2

            reactionStack.Push("ok"); // set IRDA always on 
            reactionStack.Push("mbar"); // Query pressure unit on TH1
            reactionStack.Push("ok"); // Set Manometer Range on TH1
            reactionStack.Push("\"2000 mbar\""); // Query Manometer Range on TH1
            reactionStack.Push("\"HM3500DLM110,MOD00B,B030504\""); // Identification on TH1
            reactionStack.Push(String.Empty); // Initiate Self test on TH1
            reactionStack.Push("0,\"no current Scpi errors!\""); // Check SCPI on TH1
            reactionStack.Push("80"); // Check Battery on TH1
            reactionStack.Push("ok"); //Check Manometer present
            reactionStack.Push(String.Empty); // Switch to manometer TH1

            reactionStack.Push("TIMEOUT"); // Check Battery on TH2
            reactionStack.Push("ok"); //Check Manometer present
            reactionStack.Push(String.Empty); // Switch to manometer TH2

            reactionStack.Push("Random data"); // Flush Manometer Cache

            reactionStack.Push("CONNECT Test"); // Exit remote local command mode
            reactionStack.Push("ok"); // Switch Init Led on
            reactionStack.Push("ok"); // FlushBluetoothcache
            reactionStack.Push("ok"); // Enter remote local command mode

            BluetoothHalSequentialStub stub = Hal as BluetoothHalSequentialStub;
            stub.SetUpReactionStack(reactionStack);
        }

        [Test]
        public void InitializationManometerNotPresentTest()
        {
            m_HasErrorBeenSet = false;
            SetUpManometerNotPresentStack();
            InitializationActivityControl.InitializationFinished += new EventHandler(InitializationActivityControl_InitializationFinished);
            InitializationActivityControl.InitializationStepFinished += new EventHandler(InitializationActivityControl_InitializationStepFinished_ManometerNotPresent);

            m_ManualResetEvent.Reset();
            InitializationActivityControl.ExecuteInitialization();
            m_ManualResetEvent.WaitOne(EVENT_TIMEOUT);

            InitializationActivityControl.InitializationFinished -= new EventHandler(InitializationActivityControl_InitializationFinished);
            InitializationActivityControl.InitializationStepFinished -= new EventHandler(InitializationActivityControl_InitializationStepFinished_ManometerNotPresent);

            Assert.AreEqual(InitializationResult.ERROR, m_FinishResult);
            Assert.AreEqual(ErrorCodes.INITIALIZATION_FINISHED_ERROR, m_FinishErrorCode);

            Assert.AreEqual(ErrorCodes.INITIALIZATION_MANOMETER_NOT_PRESENT, m_ExpectedStepErrorCode);
            Assert.AreEqual(InitializationManometer.TH2, m_ExpectedStepManometer);
            Assert.AreEqual("Manometer not present", m_ExpectedStepMessage);
            Assert.AreEqual(InitializationStepResult.ERROR, m_ExpectedStepResult);
        }

        void InitializationActivityControl_InitializationStepFinished_ManometerNotPresent(object sender, EventArgs e)
        {
            SetFirstStepResult(e, DeviceCommand.CheckManometerPresent.ToString());
        }

        /// <summary>
        /// Sets the manometer not present error stack.
        /// </summary>
        private void SetUpManometerNotPresentStack()
        {
            Stack<string> reactionStack = new Stack<string>();
            // Close init led after error
            reactionStack.Push("Random data"); // Flush Manometer Cache
            reactionStack.Push("CONNECT Test"); // Exit remote local command mode
            reactionStack.Push("ok"); // Switch Init Led on
            reactionStack.Push("ok"); // FlushBluetoothcache
            reactionStack.Push("ok"); // Enter remote local command mode

            reactionStack.Push("TIMEOUT"); //Check Manometer present
            reactionStack.Push(String.Empty); // Switch to manometer TH2

            reactionStack.Push("Random data"); // Flush Manometer Cache

            reactionStack.Push("CONNECT Test"); // Exit remote local command mode
            reactionStack.Push("ok"); // Switch Init Led on
            reactionStack.Push("ok"); // FlushBluetoothcache
            reactionStack.Push("ok"); // Enter remote local command mode

            BluetoothHalSequentialStub stub = Hal as BluetoothHalSequentialStub;
            stub.SetUpReactionStack(reactionStack);
        }

    }
}
