/*
//===============================================================================
// Copyright Kamstrup
// All rights reserved.
//===============================================================================
*/

using System;
using System.IO;
using System.Threading;
using Inspector.BusinessLogic.Interfaces.Events;
using Inspector.Model;
using NUnit.Framework;

namespace Inspector.BusinessLogic.Test
{
    /// <summary>
    /// InitializationExecutorControlTest
    /// </summary>
    [TestFixture]
    public class InitializationExecutorControlTest
    {
        private const int TIMEOUT = 5000;

        #region Class Members
        private ManualResetEvent m_ManualResetEvent = new ManualResetEvent(false);
        private InitializationExecutorControl m_InitializationExecutorControl;

        private string m_StepId;
        private InitializationStepResult m_StepResult;
        private string m_StepMessage;
        private int m_StepErrorCode;
        #endregion ClassMembers

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="InitializationExecutorControlTest"/> class.
        /// </summary>
        public InitializationExecutorControlTest()
        {

        }
        #endregion Constructors

        #region Test setup
        /// <summary>
        /// Sets up test.
        /// </summary>
        [SetUp]
        public void SetUpTest()
        {
            var path = Path.GetDirectoryName(typeof(InitializationExecutorControlTest).Assembly.Location);
            Assert.IsNotNull(path);
            Directory.SetCurrentDirectory(path);

            m_InitializationExecutorControl = new InitializationExecutorControl();
            m_InitializationExecutorControl.InitializationStepFinished += new EventHandler(m_InitializationExecutorControl_InitializationStepFinished);
        }

        /// <summary>
        /// Tears down test.
        /// </summary>
        [TearDown]
        public void TearDownTest()
        {
            m_InitializationExecutorControl.InitializationStepFinished -= new EventHandler(m_InitializationExecutorControl_InitializationStepFinished);
        }
        #endregion Test Setup

        #region Event Handlers
        /// <summary>
        /// Handles the InitializationStepFinished event of the m_InitializationExecutorControl control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void m_InitializationExecutorControl_InitializationStepFinished(object sender, EventArgs e)
        {
            FinishInitializationStepEventArgs eventArgs = e as FinishInitializationStepEventArgs;
            m_StepId = eventArgs.StepId;
            m_StepResult = eventArgs.Result;
            m_StepMessage = eventArgs.Message;
            m_StepErrorCode = eventArgs.ErrorCode;
            m_ManualResetEvent.Set();
        }
        #endregion Event Handlers

        /// <summary>
        /// Handles the check battery status warning test.
        /// </summary>
        [Test]
        public void HandleCheckBatteryStatusWarningTest()
        {
            Console.WriteLine(System.Reflection.MethodBase.GetCurrentMethod().Name);
            m_ManualResetEvent.Reset();
            m_InitializationExecutorControl.CurrentManometer = InitializationManometer.TH1;
            m_InitializationExecutorControl.HandleCheckBatteryStatus("60");
            m_ManualResetEvent.WaitOne(TIMEOUT);

            Assert.AreEqual(InitializationStepResult.WARNING, m_StepResult);
            Assert.AreEqual(ErrorCodes.INITIALIZATION_STEP_BATTERYLIMIT_LEVEL_WARNING, m_StepErrorCode);
            Assert.AreEqual("60", m_StepMessage);
        }

        /// <summary>
        /// Handles the check battery status test.
        /// </summary>
        [Test]
        public void HandleCheckBatteryStatusTest()
        {
            Console.WriteLine(System.Reflection.MethodBase.GetCurrentMethod().Name);
            m_ManualResetEvent.Reset();
            m_InitializationExecutorControl.CurrentManometer = InitializationManometer.TH1;
            m_InitializationExecutorControl.HandleCheckBatteryStatus("90");
            m_ManualResetEvent.WaitOne(TIMEOUT);

            Assert.AreEqual(InitializationStepResult.SUCCESS, m_StepResult);
            Assert.AreEqual(ErrorCodes.INITIALIZATION_STEP_EXECUTED_SUCCESSFULLY, m_StepErrorCode);
            Assert.AreEqual("90", m_StepMessage);
        }

        /// <summary>
        /// Handles the initiate self test test.
        /// </summary>
        [Test]
        public void HandleInitiateSelfTestTest()
        {
            Console.WriteLine(System.Reflection.MethodBase.GetCurrentMethod().Name);
            m_ManualResetEvent.Reset();
            m_InitializationExecutorControl.HandleInitiateSelfTest(String.Empty);
            m_ManualResetEvent.WaitOne(TIMEOUT);

            Assert.AreEqual(InitializationStepResult.SUCCESS, m_StepResult);
            Assert.AreEqual(ErrorCodes.INITIALIZATION_STEP_EXECUTED_SUCCESSFULLY, m_StepErrorCode);
            Assert.AreEqual(String.Empty, m_StepMessage);
        }

        /// <summary>
        /// Handles the check range test.
        /// </summary>
        [Test]
        public void HandleCheckRangeTest()
        {
            Console.WriteLine(System.Reflection.MethodBase.GetCurrentMethod().Name);
            m_ManualResetEvent.Reset();
            m_InitializationExecutorControl.PrsName = "TestScriptCommand1PRS";
            m_InitializationExecutorControl.GclName = "TestScriptCommand1GCL";
            m_InitializationExecutorControl.CurrentManometer = InitializationManometer.TH1;
            m_InitializationExecutorControl.HandleCheckRange("17 bar");
            m_ManualResetEvent.WaitOne(TIMEOUT);

            Assert.AreEqual(InitializationStepResult.SUCCESS, m_StepResult);
            Assert.AreEqual(ErrorCodes.INITIALIZATION_STEP_EXECUTED_SUCCESSFULLY, m_StepErrorCode);
            Assert.AreEqual("17 bar", m_StepMessage);
        }

        /// <summary>
        /// Handles the check range test.
        /// </summary>
        [Test]
        public void HandleCheckRangeIncorrectPressureRangeTest()
        {
            Console.WriteLine(System.Reflection.MethodBase.GetCurrentMethod().Name);
            m_ManualResetEvent.Reset();
            m_InitializationExecutorControl.PrsName = "TestScriptCommand1PRS";
            m_InitializationExecutorControl.GclName = "TestScriptCommand1GCL";
            m_InitializationExecutorControl.CurrentManometer = InitializationManometer.TH1;
            m_InitializationExecutorControl.IsInitializationForInspection = true;
            m_InitializationExecutorControl.HandleCheckRange("2000 mbar");
            m_ManualResetEvent.WaitOne(TIMEOUT);

            Assert.AreEqual(InitializationStepResult.ERROR, m_StepResult);
            Assert.AreEqual(ErrorCodes.INITIALIZATION_PRESSURE_RANGE_MANOMETER_TH1_INCORRECT, m_StepErrorCode);
            Assert.AreEqual("2000 mbar", m_StepMessage);
        }

        /// <summary>
        /// Handles the check range test.
        /// </summary>
        [Test]
        public void HandleCheckRangeIncorrectPressureRangeInitOnlyTest()
        {
            Console.WriteLine(System.Reflection.MethodBase.GetCurrentMethod().Name);
            m_ManualResetEvent.Reset();
            m_InitializationExecutorControl.PrsName = "TestScriptCommand1PRS";
            m_InitializationExecutorControl.GclName = "TestScriptCommand1GCL";
            m_InitializationExecutorControl.CurrentManometer = InitializationManometer.TH1;
            m_InitializationExecutorControl.IsInitializationForInspection = false;
            m_InitializationExecutorControl.HandleCheckRange("2000 mbar");
            m_ManualResetEvent.WaitOne(TIMEOUT);

            Assert.AreEqual(InitializationStepResult.SUCCESS, m_StepResult);
            Assert.AreEqual(ErrorCodes.INITIALIZATION_STEP_EXECUTED_SUCCESSFULLY, m_StepErrorCode);
            Assert.AreEqual("2000 mbar", m_StepMessage);
        }


        /// <summary>
        /// Handles the check range error test.
        /// </summary>
        [Test]
        public void HandleCheckRangeErrorTest()
        {
            Console.WriteLine(System.Reflection.MethodBase.GetCurrentMethod().Name);
            m_ManualResetEvent.Reset();
            m_InitializationExecutorControl.HandleCheckRange("2000");
            m_ManualResetEvent.WaitOne(TIMEOUT);

            Assert.AreEqual(InitializationStepResult.ERROR, m_StepResult);
            Assert.AreEqual(ErrorCodes.INITIALIZATION_STEP_MANOMETER_RANGE_ERROR, m_StepErrorCode);
            Assert.AreEqual("2000", m_StepMessage);
        }

        /// <summary>
        /// Handles the set pressure unit test.
        /// </summary>
        [Test]
        public void HandleSetPressureUnitTest()
        {
            Console.WriteLine(System.Reflection.MethodBase.GetCurrentMethod().Name);
            m_ManualResetEvent.Reset();
            m_InitializationExecutorControl.HandleSetPressureUnit("ok");
            m_ManualResetEvent.WaitOne(TIMEOUT);

            Assert.AreEqual(InitializationStepResult.SUCCESS, m_StepResult);
            Assert.AreEqual(ErrorCodes.INITIALIZATION_STEP_EXECUTED_SUCCESSFULLY, m_StepErrorCode);
            Assert.AreEqual("ok", m_StepMessage);
        }

        /// <summary>
        /// Handles the set pressure unit error test.
        /// </summary>
        [Test]
        public void HandleSetPressureUnitErrorTest()
        {
            Console.WriteLine(System.Reflection.MethodBase.GetCurrentMethod().Name);
            m_ManualResetEvent.Reset();
            m_InitializationExecutorControl.HandleSetPressureUnit("nok");
            m_ManualResetEvent.WaitOne(TIMEOUT);

            Assert.AreEqual(InitializationStepResult.ERROR, m_StepResult);
            Assert.AreEqual(ErrorCodes.INITIALIZATION_STEP_SET_PRESSURE_UNIT_ERROR, m_StepErrorCode);
            Assert.AreEqual("nok", m_StepMessage);
        }

        /// <summary>
        /// Handles the check SCPI interface test.
        /// </summary>
        [Test]
        public void HandleCheckSCPIInterfaceTest()
        {
            Console.WriteLine(System.Reflection.MethodBase.GetCurrentMethod().Name);
            m_ManualResetEvent.Reset();
            m_InitializationExecutorControl.HandleCheckSCPIInterface("0,\"no current scpi errors!\"");
            m_ManualResetEvent.WaitOne(TIMEOUT);

            Assert.AreEqual(InitializationStepResult.SUCCESS, m_StepResult);
            Assert.AreEqual(ErrorCodes.INITIALIZATION_STEP_EXECUTED_SUCCESSFULLY, m_StepErrorCode);
            Assert.AreEqual("0,\"no current scpi errors!\"", m_StepMessage);
        }

        /// <summary>
        /// Handles the check SCPI interface error test.
        /// </summary>
        [Test]
        public void HandleCheckSCPIInterfaceErrorTest()
        {
            Console.WriteLine(System.Reflection.MethodBase.GetCurrentMethod().Name);
            m_ManualResetEvent.Reset();
            m_InitializationExecutorControl.HandleCheckSCPIInterface("-1,Error");
            m_ManualResetEvent.WaitOne(TIMEOUT);

            Assert.AreEqual(InitializationStepResult.WARNING, m_StepResult);
            Assert.AreEqual(ErrorCodes.INITIALIZATION_STEP_SCPI_WARNING, m_StepErrorCode);
            Assert.AreEqual("-1,Error", m_StepMessage);

            m_ManualResetEvent.Reset();
            m_InitializationExecutorControl.HandleCheckSCPIInterface("-1,Error");
            m_ManualResetEvent.WaitOne(TIMEOUT);

            Assert.AreEqual(InitializationStepResult.WARNING, m_StepResult);
            Assert.AreEqual(ErrorCodes.INITIALIZATION_STEP_SCPI_WARNING, m_StepErrorCode);
            Assert.AreEqual("-1,Error", m_StepMessage);

            m_ManualResetEvent.Reset();
            m_InitializationExecutorControl.HandleCheckSCPIInterface("-1,Error");
            m_ManualResetEvent.WaitOne(TIMEOUT);

            Assert.AreEqual(InitializationStepResult.WARNING, m_StepResult);
            Assert.AreEqual(ErrorCodes.INITIALIZATION_STEP_SCPI_WARNING, m_StepErrorCode);
            Assert.AreEqual("-1,Error", m_StepMessage);

            m_ManualResetEvent.Reset();
            m_InitializationExecutorControl.HandleCheckSCPIInterface("-1,Error");
            m_ManualResetEvent.WaitOne(TIMEOUT);

            Assert.AreEqual(InitializationStepResult.WARNING, m_StepResult);
            Assert.AreEqual(ErrorCodes.INITIALIZATION_STEP_SCPI_WARNING, m_StepErrorCode);
            Assert.AreEqual("-1,Error", m_StepMessage);

            m_ManualResetEvent.Reset();
            m_InitializationExecutorControl.HandleCheckSCPIInterface("-1,Error");
            m_ManualResetEvent.WaitOne(TIMEOUT);

            Assert.AreEqual(InitializationStepResult.WARNING, m_StepResult);
            Assert.AreEqual(ErrorCodes.INITIALIZATION_STEP_SCPI_WARNING, m_StepErrorCode);
            Assert.AreEqual("-1,Error", m_StepMessage);

            m_ManualResetEvent.Reset();
            m_InitializationExecutorControl.HandleCheckSCPIInterface("-1,Error");
            m_ManualResetEvent.WaitOne(TIMEOUT);

            Assert.AreEqual(InitializationStepResult.WARNING, m_StepResult);
            Assert.AreEqual(ErrorCodes.INITIALIZATION_STEP_SCPI_WARNING, m_StepErrorCode);
            Assert.AreEqual("-1,Error", m_StepMessage);

            m_ManualResetEvent.Reset();
            m_InitializationExecutorControl.HandleCheckSCPIInterface("0, No errors");
            m_ManualResetEvent.WaitOne(TIMEOUT);

            Assert.AreEqual(InitializationStepResult.SUCCESS, m_StepResult);
            Assert.AreEqual(ErrorCodes.INITIALIZATION_STEP_EXECUTED_SUCCESSFULLY, m_StepErrorCode);
            Assert.AreEqual("0, No errors", m_StepMessage);
        }
    }
}
