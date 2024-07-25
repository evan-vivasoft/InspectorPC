/*
//===============================================================================
// Copyright Kamstrup
// All rights reserved.
//===============================================================================
*/

using System;
using System.Collections.Generic;
using System.Threading;
using Inspector.Connection.Manager.Interfaces;
using Inspector.Hal.Stub;
using Inspector.Infra;
using Inspector.Infra.Ioc;
using Inspector.Model;
using Inspector.Model.BluetoothDongle;
using NUnit.Framework;

namespace Inspector.Connection.Manager.Test
{
    [TestFixture]
    public class CommunicationControlTest
    {
        private ICommunicationControl m_CommunicationControl;
        private ManualResetEvent m_ManualResetEvent = new ManualResetEvent(false);
        private ManualResetEvent m_ManualMeasurementProcessedResetEvent = new ManualResetEvent(false);
        private bool m_CallBackExecuted = false;
        private bool m_MeasurementResultCallbackExecuted = false;
        private int m_ManualResetEventTimeout = 5000;
        private int m_ExpectedMeasurementCount;
        private bool m_MeasurementStartedCallbackExecuted = false;

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
                    m_CommunicationControl.Initialize();
                }
                return m_CommunicationControl;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommunicationControlTest"/> class.
        /// </summary>
        public CommunicationControlTest()
        {

        }

        /// <summary>
        /// Tests the claim connection control.
        /// </summary>
        [Test]
        public void TestClaimConnectionControl()
        {
            PrintTestName();
            CommunicationControl communicationControl = new CommunicationControl();

            Assert.IsTrue(communicationControl.StartCommunication(), "StartCommunication should have claimed the connection succesfully");
            Assert.IsTrue(communicationControl.CommunicationClaimed, "Connection should be claimed.");
            Assert.IsFalse(communicationControl.StartCommunication(), "Connection should have been claimed already");
            Assert.IsTrue(communicationControl.CommunicationClaimed, "Connection should still be claimed.");

            communicationControl.StopCommunication();
            Assert.False(communicationControl.CommunicationClaimed, "Connection shoould not be claimed");
        }

        /// <summary>
        /// Tests the connect without claiming.
        /// </summary>
        [Test]
        public void TestConnectWithoutClaiming()
        {
            PrintTestName();
            using (CommunicationControl communicationControl = new CommunicationControl())
            {
                communicationControl.Connect(new Dictionary<string, string>(), new List<BluetoothDongleInformation>(), ConnectWithoutClaimingConnectionCallBackResult);
            }
        }

        /// <summary>
        /// Connects the without claiming call back.
        /// </summary>
        /// <param name="commandSucceeded">if set to <c>true</c> [command succeeded].</param>
        /// <param name="errorCode">The error code.</param>
        /// <param name="message">The message.</param>
        /// <param name="deviceType">The device type.</param>
        public void ConnectWithoutClaimingConnectionCallBackResult(bool commandSucceeded, int errorCode, string message, DeviceType deviceType)
        {
            Assert.False(commandSucceeded);
            Assert.AreEqual(ErrorCodes.COMMUNICATIONCONTROL_NOT_YET_CLAIMED, errorCode);
            Assert.AreEqual("CommunicationControl has not yet been claimed.", message);
        }

        /// <summary>
        /// Tests the disconnect without claiming.
        /// </summary>
        [Test]
        public void TestDisconnectWithoutClaiming()
        {
#if DEBUG
            PrintTestName();
#endif
            using (var communicationControl = new CommunicationControl())
            {
                communicationControl.Disconnect(DisconnectWithoutClaimingConnectionCallBackResult);
            }
        }

        /// <summary>
        /// Disconnects the without claiming call back.
        /// </summary>
        /// <param name="commandSucceeded">if set to <c>true</c> [command succeeded].</param>
        /// <param name="errorCode">The error code.</param>
        /// <param name="message">The message.</param>
        /// <param name="deviceType">The device type.</param>
        public void DisconnectWithoutClaimingConnectionCallBackResult(bool commandSucceeded, int errorCode, string message, DeviceType deviceType)
        {
            Assert.False(commandSucceeded);
            Assert.AreEqual(ErrorCodes.COMMUNICATIONCONTROL_NOT_YET_CLAIMED, errorCode);
            Assert.AreEqual("CommunicationControl has not yet been claimed.", message);
        }

        /// <summary>
        /// Tests the send command without claiming.
        /// </summary>
        [Test]
        public void TestSendCommandWithoutClaiming()
        {
#if DEBUG
            PrintTestName();
#endif
            using (CommunicationControl communicationControl = new CommunicationControl())
            {
                communicationControl.SendCommand(DeviceCommand.CheckBatteryStatus, String.Empty, SendCommandWithoutClaimingCallBack);
            }
        }


        /// <summary>
        /// Sends the command without claiming call back.
        /// </summary>
        /// <param name="commandSucceeded">if set to <c>true</c> [command succeeded].</param>
        /// <param name="errorCode">The error code.</param>
        /// <param name="message">The message.</param>
        public void SendCommandWithoutClaimingCallBack(bool commandSucceeded, int errorCode, string message)
        {
            Assert.False(commandSucceeded);
            Assert.AreEqual(ErrorCodes.COMMUNICATIONCONTROL_NOT_YET_CLAIMED, errorCode);
            Assert.AreEqual("CommunicationControl has not yet been claimed.", message);
        }

        /// <summary>
        /// Tests the send command without claiming.
        /// </summary>
        [Test]
        public void TestRecoverFromErrorWithoutClaiming()
        {
#if DEBUG
            PrintTestName();
#endif
            using (CommunicationControl communicationControl = new CommunicationControl())
            {
                communicationControl.RecoverFromError(RecoverFromErrorWithoutClaimingCallBack);
            }
        }

        /// <summary>
        /// Sends the command without claiming call back.
        /// </summary>
        /// <param name="commandSucceeded">if set to <c>true</c> [command succeeded].</param>
        /// <param name="errorCode">The error code.</param>
        /// <param name="message">The message.</param>
        public void RecoverFromErrorWithoutClaimingCallBack(bool commandSucceeded, int errorCode, string message)
        {
            Assert.False(commandSucceeded);
            Assert.AreEqual(ErrorCodes.COMMUNICATIONCONTROL_NOT_YET_CLAIMED, errorCode);
            Assert.AreEqual("CommunicationControl has not yet been claimed.", message);
        }

        /// <summary>
        /// Tests the send command without claiming.
        /// </summary>
        [Test]
        public void TestStartContinuousMeasurementWithoutClaiming()
        {
#if DEBUG
            PrintTestName();
#endif
            using (CommunicationControl communicationControl = new CommunicationControl())
            {
                communicationControl.StartContinuousMeasurement(0, StartContinuousMeasurementWithoutClaimingCallBack, null, ProcessMeasurementStartedCallback);
            }
        }

        /// <summary>
        /// Sends the command without claiming call back.
        /// </summary>
        /// <param name="commandSucceeded">if set to <c>true</c> [command succeeded].</param>
        /// <param name="errorCode">The error code.</param>
        /// <param name="message">The message.</param>
        public void StartContinuousMeasurementWithoutClaimingCallBack(bool commandSucceeded, int errorCode, string message)
        {
            Assert.False(commandSucceeded);
            Assert.AreEqual(ErrorCodes.COMMUNICATIONCONTROL_NOT_YET_CLAIMED, errorCode);
            Assert.AreEqual("CommunicationControl has not yet been claimed.", message);
        }

        /// <summary>
        /// Tests the send command without claiming.
        /// </summary>
        [Test]
        public void TestStopContinuousMeasurementWithoutClaiming()
        {
#if DEBUG
            PrintTestName();
#endif
            using (CommunicationControl communicationControl = new CommunicationControl())
            {
                communicationControl.StopContinuousMeasurement(StopContinuousMeasurementWithoutClaimingCallBack);
            }
        }

        /// <summary>
        /// Sends the command without claiming call back.
        /// </summary>
        /// <param name="commandSucceeded">if set to <c>true</c> [command succeeded].</param>
        /// <param name="errorCode">The error code.</param>
        /// <param name="message">The message.</param>
        public void StopContinuousMeasurementWithoutClaimingCallBack(bool commandSucceeded, int errorCode, string message)
        {
            Assert.False(commandSucceeded);
            Assert.AreEqual(ErrorCodes.COMMUNICATIONCONTROL_NOT_YET_CLAIMED, errorCode);
            Assert.AreEqual("CommunicationControl has not yet been claimed.", message);
        }

        /// <summary>
        /// Calls the send command in incorrect state test.
        /// </summary>
        [Test]
        public void CallSendCommandInIncorrectStateTest()
        {
#if DEBUG
            PrintTestName();
#endif
            m_CallBackExecuted = false;

            Assert.IsTrue(CommunicationControl.StartCommunication());
            m_ManualResetEvent.Reset();
            CommunicationControl.SendCommand(DeviceCommand.FlushManometerCache, String.Empty, StateMachineErrorCommandCallBackResult);
            m_ManualResetEvent.WaitOne(m_ManualResetEventTimeout);

            Assert.IsTrue(m_CallBackExecuted);

            CommunicationControl.StopCommunication();
        }

        /// <summary>
        /// Calls the recover from error in incorrect state test.
        /// </summary>
        [Test]
        public void CallRecoverFromErrorInIncorrectStateTest()
        {
#if DEBUG
            PrintTestName();
#endif
            m_CallBackExecuted = false;

            Assert.IsTrue(CommunicationControl.StartCommunication());
            m_ManualResetEvent.Reset();
            CommunicationControl.RecoverFromError(StateMachineErrorCommandCallBackResult);
            m_ManualResetEvent.WaitOne(m_ManualResetEventTimeout);

            Assert.IsTrue(m_CallBackExecuted);

            CommunicationControl.StopCommunication();
        }

        /// <summary>
        /// Calls the connect in incorrect state test.
        /// </summary>
        [Test]
        public void CallConnectInIncorrectStateTest()
        {
#if DEBUG
            PrintTestName();
            PrintDebug();
#endif
            m_CallBackExecuted = false;

            // First connect, should go fine
            Assert.IsTrue(CommunicationControl.StartCommunication());
            m_ManualResetEvent.Reset();
            CommunicationControl.Connect(new Dictionary<string, string>(), new List<BluetoothDongleInformation>(), AcceptSucceededConnectionCallbackResult);
            m_ManualResetEvent.WaitOne(m_ManualResetEventTimeout);
            Assert.IsTrue(m_CallBackExecuted);

            // Then connect again, should fail
            m_CallBackExecuted = false;
            m_ManualResetEvent.Reset();
            CommunicationControl.Connect(new Dictionary<string, string>(), new List<BluetoothDongleInformation>(), StateMachineErrorConnectionCallBackResult);
            m_ManualResetEvent.WaitOne(m_ManualResetEventTimeout);
            Assert.IsTrue(m_CallBackExecuted);

            // Disconnect
            m_CallBackExecuted = false;
            m_ManualResetEvent.Reset();
            CommunicationControl.Disconnect(StatemachineNoErrorCallback);
            m_ManualResetEvent.WaitOne();
            Assert.IsTrue(m_CallBackExecuted);

            CommunicationControl.StopCommunication();
        }

        private void PrintDebug()
        {
            Console.WriteLine("Current state is: " + ((ConnectionStateMachine)(CommunicationControl as CommunicationControl).ConnectionStateMachine).CurrentState.GetType().Name);
        }

        /// <summary>
        /// Calls the disconnect in incorrect state test.
        /// </summary>
        [Test]
        public void CallDisconnectInIncorrectStateTest()
        {
#if DEBUG
            PrintTestName();
#endif
            m_CallBackExecuted = false;
            Assert.IsTrue(CommunicationControl.StartCommunication());

            // First connect, should go fine
            m_ManualResetEvent.Reset();
            CommunicationControl.Connect(new Dictionary<string, string>(), new List<BluetoothDongleInformation>(), AcceptSucceededConnectionCallbackResult);
            m_ManualResetEvent.WaitOne(m_ManualResetEventTimeout);
            Assert.IsTrue(m_CallBackExecuted);

            // Then send a command (no reactionstack, so state is in SendingCommand)
            CommunicationControl.SendCommand(DeviceCommand.FlushManometerCache, string.Empty, AcceptSucceededCallbacked);

            // Then disconnect, should fail
            m_CallBackExecuted = false;
            m_ManualResetEvent.Reset();
            CommunicationControl.Disconnect(StateMachineErrorConnectionCallBackResult);
            m_ManualResetEvent.WaitOne(m_ManualResetEventTimeout);
            Assert.IsTrue(m_CallBackExecuted);

            // force transition to error state
            m_CallBackExecuted = false;
            m_ManualResetEvent.Reset();
            
            var communicationControl = CommunicationControl as CommunicationControl;
            Assert.IsNotNull(communicationControl);

            var connectionStateMachine = communicationControl.ConnectionStateMachine as ConnectionStateMachine;
            Assert.IsNotNull(connectionStateMachine);
            
            var stub = connectionStateMachine.Hal as BluetoothHalSequentialStub;
            Assert.IsNotNull(stub);

            communicationControl.CommandResultCallback = StateMachineErrorCommandCallBackResult;

            stub.GenerateError();

            m_ManualResetEvent.WaitOne(m_ManualResetEventTimeout);
            Assert.IsTrue(m_CallBackExecuted);

            // disconnect normally
            m_CallBackExecuted = false;
            m_ManualResetEvent.Reset();
            CommunicationControl.Disconnect(StatemachineNoErrorCallback);
            m_ManualResetEvent.WaitOne(m_ManualResetEventTimeout);
            Assert.IsTrue(m_CallBackExecuted);

            CommunicationControl.StopCommunication();
        }

        [Test]
        public void CallContinuousMeasurement()
        {
#if DEBUG
            PrintTestName();
#endif
            const int measurementFrequency = 25;
            BluetoothHalSequentialStub.MEASUREMENT_FREQUENCY = measurementFrequency;
            m_ExpectedMeasurementCount = measurementFrequency;

            m_CallBackExecuted = false;
            Assert.IsTrue(CommunicationControl.StartCommunication());

            // First connect, should go fine
            m_ManualResetEvent.Reset();
            CommunicationControl.Connect(new Dictionary<string, string>(), new List<BluetoothDongleInformation>(), AcceptSucceededConnectionCallbackResult);
            m_ManualResetEvent.WaitOne(m_ManualResetEventTimeout);
            Assert.IsTrue(m_CallBackExecuted);
            m_MeasurementStartedCallbackExecuted = false;
            // start measuring
            m_ManualResetEvent.Reset();
            CommunicationControl.StartContinuousMeasurement(measurementFrequency, AcceptSucceededCallbacked, ProcessMeasurementResultCallback, ProcessMeasurementStartedCallback);
            m_ManualMeasurementProcessedResetEvent.Set(); // allow result processing to continue
            m_ManualResetEvent.WaitOne(m_ManualResetEventTimeout); // wait for result to be processed
            Assert.IsTrue(m_MeasurementStartedCallbackExecuted);
            Assert.IsTrue(m_MeasurementResultCallbackExecuted);
            
            // see if the next round of measurements is automatically sent again
            m_MeasurementResultCallbackExecuted = false;
            m_ManualResetEvent.Reset();
            m_ManualMeasurementProcessedResetEvent.Set(); // allow result processing to continue
            m_ManualResetEvent.WaitOne(m_ManualResetEventTimeout); // wait for result to be processed
            Assert.IsTrue(m_MeasurementResultCallbackExecuted);

            // stop measuring
            m_ManualResetEvent.Reset();
            CommunicationControl.StopContinuousMeasurement(AcceptSucceededCallbacked);
            m_ManualResetEvent.WaitOne(m_ManualResetEventTimeout);
            Assert.IsTrue(m_CallBackExecuted);

            // disconnect
            m_CallBackExecuted = false;
            m_ManualResetEvent.Reset();
            CommunicationControl.Disconnect(AcceptSucceededConnectionCallbackResult);
            m_ManualResetEvent.WaitOne(m_ManualResetEventTimeout);
            Assert.IsTrue(m_CallBackExecuted);

            CommunicationControl.StopCommunication();
        }

        [Test]
        public void CallStopContinuousMeasurementWhenNotStartedIsNotAllowed()
        {
#if DEBUG
            PrintTestName();
#endif
            const int measurementFrequency = 25;
            BluetoothHalSequentialStub.MEASUREMENT_FREQUENCY = measurementFrequency;
            m_ExpectedMeasurementCount = measurementFrequency;

            m_CallBackExecuted = false;
            Assert.IsTrue(CommunicationControl.StartCommunication());

            // First connect, should go fine
            m_ManualResetEvent.Reset();
            CommunicationControl.Connect(new Dictionary<string, string>(), new List<BluetoothDongleInformation>(), AcceptSucceededConnectionCallbackResult);
            m_ManualResetEvent.WaitOne(m_ManualResetEventTimeout);
            Assert.IsTrue(m_CallBackExecuted);

            // stop measuring, should fail
            m_ManualResetEvent.Reset();
            CommunicationControl.StopContinuousMeasurement(StateMachineErrorCommandCallBackResult);
            m_ManualResetEvent.WaitOne(m_ManualResetEventTimeout);
            Assert.IsTrue(m_CallBackExecuted);

            // disconnect
            m_CallBackExecuted = false;
            m_ManualResetEvent.Reset();
            CommunicationControl.Disconnect(AcceptSucceededConnectionCallbackResult);
            m_ManualResetEvent.WaitOne(m_ManualResetEventTimeout);
            Assert.IsTrue(m_CallBackExecuted);

            CommunicationControl.StopCommunication();
        }

        [Test]
        public void CallStartContinuousMeasurementWhileAlreadyStartedIsNotAllowed()
        {
#if DEBUG
            PrintTestName();
#endif
            const int measurementFrequency = 25;
            BluetoothHalSequentialStub.MEASUREMENT_FREQUENCY = measurementFrequency;
            m_ExpectedMeasurementCount = measurementFrequency;

            m_CallBackExecuted = false;
            m_MeasurementStartedCallbackExecuted = false;
            Assert.IsTrue(CommunicationControl.StartCommunication());

            // First connect, should go fine
            m_ManualResetEvent.Reset();
            CommunicationControl.Connect(new Dictionary<string, string>(), new List<BluetoothDongleInformation>(), AcceptSucceededConnectionCallbackResult);
            m_ManualResetEvent.WaitOne(m_ManualResetEventTimeout);
            Assert.IsTrue(m_CallBackExecuted);

            // start measuring, should go ok
            m_ManualResetEvent.Reset();
            CommunicationControl.StartContinuousMeasurement(measurementFrequency, AcceptSucceededCallbacked, ProcessMeasurementResultCallback,ProcessMeasurementStartedCallback);
            m_ManualMeasurementProcessedResetEvent.Set(); // allow result processing to continue
            m_ManualResetEvent.WaitOne(m_ManualResetEventTimeout); // wait for result to be processed
            Assert.IsTrue(m_MeasurementStartedCallbackExecuted);
            Assert.IsTrue(m_MeasurementResultCallbackExecuted);

            // start measuring again, should fail
            m_ManualResetEvent.Reset();
            CommunicationControl.StartContinuousMeasurement(measurementFrequency, StateMachineErrorCommandCallBackResult, null, ProcessMeasurementStartedCallback);
            m_ManualResetEvent.WaitOne(m_ManualResetEventTimeout);

            // stop measuring
            m_ManualResetEvent.Reset();
            CommunicationControl.StopContinuousMeasurement(AcceptSucceededCallbacked);
            m_ManualResetEvent.WaitOne(m_ManualResetEventTimeout);
            Assert.IsTrue(m_CallBackExecuted);

            // disconnect
            m_CallBackExecuted = false;
            m_ManualResetEvent.Reset();
            CommunicationControl.Disconnect(AcceptSucceededConnectionCallbackResult);
            m_ManualResetEvent.WaitOne(m_ManualResetEventTimeout);
            Assert.IsTrue(m_CallBackExecuted);

            CommunicationControl.StopCommunication();
        }

        [Test]
        public void ContinuousMeasurementTimeout()
        {
#if DEBUG
            PrintTestName();
#endif
            const int measurementFrequency = 25;
            BluetoothHalSequentialStub.MEASUREMENT_FREQUENCY = measurementFrequency;
            m_ExpectedMeasurementCount = measurementFrequency;

            try
            {
                (((CommunicationControl as CommunicationControl).ConnectionStateMachine as ConnectionStateMachine).Hal as BluetoothHalSequentialStub).EnableMeasurementTimeout = true;

                m_CallBackExecuted = false;
                Assert.IsTrue(CommunicationControl.StartCommunication());

                // First connect, should go fine
                m_ManualResetEvent.Reset();
                CommunicationControl.Connect(new Dictionary<string, string>(), new List<BluetoothDongleInformation>(), StatemachineNoErrorCallback);
                m_ManualResetEvent.WaitOne(m_ManualResetEventTimeout);
                Assert.IsTrue(m_CallBackExecuted);

                // start measuring
                m_CallBackExecuted = false;
                m_MeasurementResultCallbackExecuted = false;
				m_MeasurementStartedCallbackExecuted = false;
                m_ManualResetEvent.Reset();
                CommunicationControl.StartContinuousMeasurement(measurementFrequency, TimeoutCallBack, ProcessMeasurementResultCallback,ProcessMeasurementStartedCallback);
                m_ManualMeasurementProcessedResetEvent.Set(); // allow result processing to continue
                m_ManualResetEvent.WaitOne(m_ManualResetEventTimeout); // wait for result to be processed
                Assert.IsTrue(m_MeasurementStartedCallbackExecuted);
                Assert.IsFalse(m_MeasurementResultCallbackExecuted); // should not be processed
                Assert.IsTrue(m_CallBackExecuted); // should have timeout handled

                // disconnect
                m_CallBackExecuted = false;
                m_ManualResetEvent.Reset();
                CommunicationControl.Disconnect(StatemachineNoErrorCallback);
                m_ManualResetEvent.WaitOne(m_ManualResetEventTimeout);
                Assert.IsTrue(m_CallBackExecuted);

                CommunicationControl.StopCommunication();
            }
            finally
            {
                (((CommunicationControl as CommunicationControl).ConnectionStateMachine as ConnectionStateMachine).Hal as BluetoothHalSequentialStub).EnableMeasurementTimeout = false;
            }
        }

        /// <summary>
        /// States the machine error call back.
        /// </summary>
        /// <param name="commandSucceeded">if set to <c>true</c> [command succeeded].</param>
        /// <param name="errorCode">The error code.</param>
        /// <param name="message">The message.</param>
        /// <param name="deviceType">The device type.</param>
        public void StateMachineErrorConnectionCallBackResult(bool commandSucceeded, int errorCode, string message, DeviceType deviceType)
        {
            Assert.False(commandSucceeded);
            Assert.AreEqual(ErrorCodes.COMMUNICATIONCONTROL_STATEMACHINE_ERROR, errorCode);
            Assert.AreEqual("Statemachine error.", message);
            m_CallBackExecuted = true;
            m_ManualResetEvent.Set();
        }

        /// <summary>
        /// States the machine error call back.
        /// </summary>
        /// <param name="commandSucceeded">if set to <c>true</c> [command succeeded].</param>
        /// <param name="errorCode">The error code.</param>
        /// <param name="message">The message.</param>
        public void StateMachineErrorCommandCallBackResult(bool commandSucceeded, int errorCode, string message)
        {
            Assert.False(commandSucceeded);
            Assert.AreEqual(ErrorCodes.COMMUNICATIONCONTROL_STATEMACHINE_ERROR, errorCode);
            Assert.AreEqual("Statemachine error.", message);
            m_CallBackExecuted = true;
            m_ManualResetEvent.Set();
        }

        /// <summary>
        /// Accepts all call back.
        /// </summary>
        /// <param name="commandSucceeded">if set to <c>true</c> [command succeeded].</param>
        /// <param name="errorCode">The error code.</param>
        /// <param name="message">The message.</param>
        public void AcceptAllCallBack(bool commandSucceeded, int errorCode, string message)
        {
            m_CallBackExecuted = true;
            m_ManualResetEvent.Set();
        }

        /// <summary>
        /// Accepts a callback and asserts if the command succeeded.
        /// </summary>
        /// <param name="commandSucceeded">if set to <c>true</c> [command succeeded].</param>
        /// <param name="errorCode">The error code.</param>
        /// <param name="message">The message.</param>
        /// <param name="deviceType">The device type.</param>
        public void AcceptSucceededConnectionCallbackResult(bool commandSucceeded, int errorCode, string message, DeviceType deviceType)
        {
            Assert.IsTrue(commandSucceeded);
            Assert.AreEqual(0, errorCode);
            m_CallBackExecuted = true;
            m_ManualResetEvent.Set();
        }

        /// <summary>
        /// Accepts a callback and asserts if the command succeeded.
        /// </summary>
        /// <param name="commandSucceeded">if set to <c>true</c> [command succeeded].</param>
        /// <param name="errorCode">The error code.</param>
        /// <param name="message">The message.</param>
        public void AcceptSucceededCallbacked(bool commandSucceeded, int errorCode, string message)
        {
            Assert.IsTrue(commandSucceeded);
            Assert.AreEqual(0, errorCode);
            m_CallBackExecuted = true;
            m_ManualResetEvent.Set();
        }

        /// <summary>
        /// Processes the measurement result callback.
        /// </summary>
        /// <param name="measurements">The measurements.</param>
        public void ProcessMeasurementResultCallback(IList<Measurement> measurements)
        {
            m_ManualMeasurementProcessedResetEvent.WaitOne(m_ManualResetEventTimeout);
            m_ManualMeasurementProcessedResetEvent.Reset();

            double tolerance = Math.Abs(BluetoothHalSequentialStub.MEASUREMENT_INCREMENT_VALUE * .0001);

            Assert.AreEqual(m_ExpectedMeasurementCount, measurements.Count);
            for (int i = 1; i < measurements.Count; i++)
            {
                double difference = Math.Abs(measurements[i].Value - measurements[i - 1].Value);
                Assert.That(BluetoothHalSequentialStub.MEASUREMENT_INCREMENT_VALUE - difference <= tolerance);
            }
            m_MeasurementResultCallbackExecuted = true;
            m_ManualResetEvent.Set();
        }


        public void ProcessMeasurementStartedCallback()
        {
            m_MeasurementStartedCallbackExecuted = true;
        }
        /// <summary>
        /// Statemachines the no error callback.
        /// </summary>
        /// <param name="commandSucceeded">if set to <c>true</c> [command succeeded].</param>
        /// <param name="errorCode">The error code.</param>
        /// <param name="message">The message.</param>
        /// <param name="deviceType">The device type.</param>
        public void StatemachineNoErrorCallback(bool commandSucceeded, int errorCode, string message, DeviceType deviceType)
        {
            Assert.IsTrue(commandSucceeded);
            Assert.AreEqual(0, errorCode);
            m_CallBackExecuted = true;
            m_ManualResetEvent.Set();
        }

        /// <summary>
        /// Timeouts the call back.
        /// </summary>
        /// <param name="commandSucceeded">if set to <c>true</c> [command succeeded].</param>
        /// <param name="errorCode">The error code.</param>
        /// <param name="message">The message.</param>
        public void TimeoutCallBack(bool commandSucceeded, int errorCode, string message)
        {
            Assert.IsFalse(commandSucceeded);
            Assert.AreEqual(1041, errorCode);
            Assert.AreEqual("Timeout", message);
            m_CallBackExecuted = true;
            m_ManualResetEvent.Set();
        }

        /// <summary>
        /// Prints the name of the current test.
        /// </summary>
        private void PrintTestName()
        {
            string message = "Running test: " + TestContext.CurrentContext.Test.FullName;
            System.Diagnostics.Debug.WriteLine(message);
            Console.WriteLine(message);
        }
    }
}
