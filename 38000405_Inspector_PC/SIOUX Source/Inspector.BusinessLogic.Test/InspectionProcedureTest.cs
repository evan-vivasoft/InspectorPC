/*
//===============================================================================
// Copyright Kamstrup
// All rights reserved.
//===============================================================================
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Inspector.BusinessLogic.Data.Configuration.InspectionManager.Managers;
using Inspector.BusinessLogic.Interfaces;
using Inspector.BusinessLogic.Interfaces.Events;
using Inspector.Hal.Interfaces;
using Inspector.Hal.Stub;
using Inspector.Infra;
using Inspector.Infra.Ioc;
using Inspector.Model;
using Inspector.Model.InspectionProcedure;
using Inspector.Model.InspectionStepResult;
using NUnit.Framework;

namespace Inspector.BusinessLogic.Test
{
    /// <summary>
    /// 
    /// </summary>
    [TestFixture]
    public class InspectionProcedureTest
    {
        #region Class Members
        private const int EventTimeout = 30000;
        private const int ContinuousMeasurementEventTimeout = 30000;
        
        private readonly ManualResetEvent m_MeasurementReceivedManualResetEvent = new ManualResetEvent(false);
        private readonly ManualResetEvent m_MeasurementsCompletedManualResetEvent = new ManualResetEvent(false);

        private bool m_ExtraMeasurementsStartedEventFired;

        private int m_InspectionStepCount;
        private List<long> m_ExpectedSequenceNumbers;

        private const string ResultFilename = "results.xml";
        private const string TmpResultFilename = "tmpResults.xml";

        private bool m_MeasurementValueOutOfLimits;
        private bool m_MeasurementResultFired;
        #endregion Class Members

        #region Properties
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

        /// <summary>
        /// Gets or sets the initialization activity control.
        /// </summary>
        /// <value>
        /// The initialization activity control.
        /// </value>
        private static IInspectionActivityControl InspectionActivityControl
        {
            get
            {
                return ContextRegistry.Context.Resolve<IInspectionActivityControl>();
            }
        }
        #endregion Properties

        [SetUp]
        public void SetUp()
        {
            var path = Path.GetDirectoryName(typeof(InspectionProcedureTest).Assembly.Location);
            Assert.IsNotNull(path);
            Directory.SetCurrentDirectory(path);

            if (File.Exists(ResultFilename))
            {
                File.Delete(ResultFilename);
            }

            if (File.Exists(TmpResultFilename))
            {
                File.Delete(TmpResultFilename);
            }
        }

        [TearDown]
        public void TearDown()
        {
            InspectionActivityControl.Dispose();

            ContextRegistry.Context.Release();
        }

        #region Generic Handlers
        /// <summary>
        /// Handles the MeasurementsReceived event of the InspectionActivityControl control for the ScriptCommand5X tests.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private static void InspectionActivityControl_MeasurementsReceived(object sender, EventArgs args)
        {
            Assert.IsInstanceOf<MeasurementEventArgs>(args, "Expected MeasurementEventArgs");
            var eventArgs = args as MeasurementEventArgs;
            Assert.IsNotNull(eventArgs);
            Assert.AreEqual(10, eventArgs.Measurements.Count);
        }

        private void InspectionActivityControl_ExtraMeasurementStarted(object sender, EventArgs args)
        {
            Assert.AreEqual(EventArgs.Empty, args);
            m_ExtraMeasurementsStartedEventFired = true;
        }

        /// <summary>
        /// Executes the test.
        /// </summary>
        /// <param name="prsName">Name of the PRS.</param>
        /// <param name="gclName">Name of the GCL.</param>
        /// <param name="executeInspectionStepEvent">The execute inspection step event.</param>
        private static void ExecuteHappyFlowTest(string prsName, string gclName, Action<object, EventArgs> executeInspectionStepEvent)
        {
            var status = InspectionStatus.Unset;
            var sectionSelection = new SectionSelection();
            var errorCode = -1;
            var eventFired = false;

            var manualResetEvent = new ManualResetEvent(false);

            EventHandler handler = (sender, args) =>
            {
                Assert.IsInstanceOf<InspectionFinishedEventArgs>(args, "Expected InspectionFinishedEventArgs");

                var eventArgs = args as InspectionFinishedEventArgs;

                Assert.IsNotNull(eventArgs);

                status = eventArgs.Result;
                errorCode = eventArgs.ErrorCode;
                sectionSelection = eventArgs.PartialInspection;

                eventFired = true;

                manualResetEvent.Set();
            };

            InspectionActivityControl.ExecuteInspectionStep += new EventHandler(executeInspectionStepEvent);
            InspectionActivityControl.InspectionFinished += handler;

            Assert.IsTrue(InspectionActivityControl.ExecuteInspection(prsName, gclName), "Failed to execute inspection");

            manualResetEvent.WaitOne(EventTimeout);

            Assert.IsTrue(eventFired, "Expect that the event is fired");
            Assert.AreEqual(InspectionStatus.Completed, status);
            Assert.AreEqual(ErrorCodes.INSPECTION_FINISHED_SUCCESSFULLY, errorCode);
            Assert.IsNull(sectionSelection);

            InspectionActivityControl.ExecuteInspectionStep -= handler;
            InspectionActivityControl.ExecuteInspectionStep -= new EventHandler(executeInspectionStepEvent);
        }

        /// <summary>
        /// Executes the happy flow test continuous measurements with 0 measurement time defined where the user performs a manual stop measurement
        /// </summary>
        /// <param name="prsName">Name of the PRS.</param>
        /// <param name="gclName">Name of the GCL.</param>
        /// <param name="measurementsReceived">The measurements received.</param>
        private void ExecuteHappyFlowTestContinuousMeasurementsManualUserStop(string prsName, string gclName, Action<object, EventArgs> measurementsReceived)
        {
            var status = InspectionStatus.Unset;
            var sectionSelection = new SectionSelection();
            var errorCode = -1;
            var eventFired = false;

            var manualResetEvent = new ManualResetEvent(false);

            EventHandler handler = (sender, args) =>
            {
                Assert.IsInstanceOf<InspectionFinishedEventArgs>(args, "Expected InspectionFinishedEventArgs");

                var eventArgs = args as InspectionFinishedEventArgs;

                Assert.IsNotNull(eventArgs);

                status = eventArgs.Result;
                errorCode = eventArgs.ErrorCode;
                sectionSelection = eventArgs.PartialInspection;

                eventFired = true;

                manualResetEvent.Set();
            };

            InspectionActivityControl.MeasurementsReceived += new EventHandler(measurementsReceived);
            InspectionActivityControl.MeasurementsCompleted += InspectionActivityControl_MeasurementsCompleted;
            InspectionActivityControl.InspectionFinished += handler;

            m_MeasurementReceivedManualResetEvent.Reset();

            Assert.IsTrue(InspectionActivityControl.ExecuteInspection(prsName, gclName));

            // wait for at least one measurementsReceived action that sets the m_ManualResetEvent
            Assert.IsTrue(m_MeasurementReceivedManualResetEvent.WaitOne(ContinuousMeasurementEventTimeout));

            // stop continuous measurements
            InspectionActivityControl.StopContinuousMeasurement();

            // finish inspection
            manualResetEvent.WaitOne(EventTimeout);

            // the inspection should be fininished now
            Assert.IsFalse(m_ExtraMeasurementsStartedEventFired);

            Assert.IsTrue(eventFired, "Expect that the event is fired");
            Assert.AreEqual(InspectionStatus.Completed, status);
            Assert.AreEqual(ErrorCodes.INSPECTION_FINISHED_SUCCESSFULLY, errorCode);
            Assert.IsTrue(sectionSelection.SectionSelectionEntities.All(sectionEntity => sectionEntity.IsSelected == false));

            InspectionActivityControl.InspectionFinished -= handler;
            InspectionActivityControl.MeasurementsCompleted -= InspectionActivityControl_MeasurementsCompleted;
            InspectionActivityControl.MeasurementsReceived -= new EventHandler(measurementsReceived);
        }

        /// <summary>
        /// Executes the happy flow test restart continuous measurements with extra measurement period.
        /// </summary>
        /// <param name="prsName">Name of the PRS.</param>
        /// <param name="gclName">Name of the GCL.</param>
        /// <param name="executeInspectionStepEvent">The execute inspection step event.</param>
        /// <param name="measurementsReceived">The measurements received.</param>
        /// <param name="extraMeasurementsReceived">The extra measurements received.</param>
        private void ExecuteHappyFlowTestRestartContinuousMeasurementsWithExtraMeasurementPeriod(string prsName, string gclName, Action<object, EventArgs> executeInspectionStepEvent, Action<object, EventArgs> measurementsReceived, Action<object, EventArgs> extraMeasurementsReceived)
        {
            var status = InspectionStatus.Unset;
            var sectionSelection = new SectionSelection();
            var errorCode = -1;
            var eventFired = false;

            var manualResetEvent = new ManualResetEvent(false);

            EventHandler handler = (sender, args) =>
            {
                Assert.IsInstanceOf<InspectionFinishedEventArgs>(args, "Expected InspectionFinishedEventArgs");

                var eventArgs = args as InspectionFinishedEventArgs;

                Assert.IsNotNull(eventArgs);

                status = eventArgs.Result;
                errorCode = eventArgs.ErrorCode;
                sectionSelection = eventArgs.PartialInspection;

                eventFired = true;

                manualResetEvent.Set();
            };

            m_ExtraMeasurementsStartedEventFired = false;

            InspectionActivityControl.MeasurementsReceived += new EventHandler(measurementsReceived);
            InspectionActivityControl.ExtraMeasurementStarted += new EventHandler(extraMeasurementsReceived);
            InspectionActivityControl.MeasurementsCompleted += InspectionActivityControl_MeasurementsCompleted_NoReply;
            InspectionActivityControl.ExecuteInspectionStep += new EventHandler(executeInspectionStepEvent);
            InspectionActivityControl.InspectionFinished += handler;

            m_MeasurementReceivedManualResetEvent.Reset();
            Assert.IsTrue(InspectionActivityControl.ExecuteInspection(prsName, gclName));
            m_MeasurementReceivedManualResetEvent.WaitOne(ContinuousMeasurementEventTimeout);

            // stop continuous measurements
            m_MeasurementsCompletedManualResetEvent.Reset();
            InspectionActivityControl.StopContinuousMeasurement();
            m_MeasurementsCompletedManualResetEvent.WaitOne(ContinuousMeasurementEventTimeout);

            // finish inspection
            InspectionActivityControl.MeasurementsCompleted -= InspectionActivityControl_MeasurementsCompleted_NoReply;
            InspectionActivityControl.MeasurementsCompleted += InspectionActivityControl_MeasurementsCompleted;

            // before restart checks
            Assert.IsTrue(m_ExtraMeasurementsStartedEventFired);
            Assert.IsFalse(eventFired);
            m_ExtraMeasurementsStartedEventFired = false;

            // restart
            InspectionActivityControl.StartContinuousMeasurement();
            Thread.Sleep(2000);
            InspectionActivityControl.StopContinuousMeasurement();

            m_MeasurementsCompletedManualResetEvent.WaitOne(ContinuousMeasurementEventTimeout);

            // wait for finish inspection
            manualResetEvent.WaitOne(EventTimeout);

            // see if the inspection finished
            Assert.IsTrue(m_ExtraMeasurementsStartedEventFired);

            Assert.IsTrue(eventFired, "Expect that the event is fired");
            Assert.AreEqual(InspectionStatus.Completed, status);
            Assert.AreEqual(ErrorCodes.INSPECTION_FINISHED_SUCCESSFULLY, errorCode);
            Assert.IsTrue(sectionSelection.SectionSelectionEntities.All(sectionEntity => sectionEntity.IsSelected == false));

            InspectionActivityControl.InspectionFinished -= handler;
            InspectionActivityControl.ExecuteInspectionStep -= new EventHandler(executeInspectionStepEvent);
            InspectionActivityControl.MeasurementsCompleted -= InspectionActivityControl_MeasurementsCompleted;
            InspectionActivityControl.ExtraMeasurementStarted -= new EventHandler(extraMeasurementsReceived);
            InspectionActivityControl.MeasurementsReceived -= new EventHandler(measurementsReceived);
        }

        /// <summary>
        /// Executes the happy flow continuous measurements test with user stop action followed by a start action.
        /// </summary>
        /// <param name="prsName">Name of the PRS.</param>
        /// <param name="gclName">Name of the GCL.</param>
        /// <param name="executeInspectionStepEvent"></param>
        /// <param name="measurementsReceived">The measurements received.</param>
        /// <param name="extraMeasurementsReceived"></param>
        private void ExecuteHappyFlowTestStopContinuousMeasurementsWithExtraMeasurementPeriod(string prsName, string gclName, Action<object, EventArgs> executeInspectionStepEvent, Action<object, EventArgs> measurementsReceived, Action<object, EventArgs> extraMeasurementsReceived)
        {
            var status = InspectionStatus.Unset;
            var sectionSelection = new SectionSelection();
            var errorCode = -1;
            var eventFired = false;

            var manualResetEvent = new ManualResetEvent(false);

            EventHandler handler = (sender, args) =>
            {
                Assert.IsInstanceOf<InspectionFinishedEventArgs>(args, "Expected InspectionFinishedEventArgs");

                var eventArgs = args as InspectionFinishedEventArgs;

                Assert.IsNotNull(eventArgs);

                status = eventArgs.Result;
                errorCode = eventArgs.ErrorCode;
                sectionSelection = eventArgs.PartialInspection;

                eventFired = true;

                manualResetEvent.Set();
            };

            m_ExtraMeasurementsStartedEventFired = false;

            InspectionActivityControl.MeasurementsReceived += new EventHandler(measurementsReceived);
            InspectionActivityControl.ExtraMeasurementStarted += new EventHandler(extraMeasurementsReceived);
            InspectionActivityControl.MeasurementsCompleted += InspectionActivityControl_MeasurementsCompleted;
            InspectionActivityControl.ExecuteInspectionStep += new EventHandler(executeInspectionStepEvent);
            InspectionActivityControl.InspectionFinished += handler;

            m_MeasurementReceivedManualResetEvent.Reset();
            Assert.IsTrue(InspectionActivityControl.ExecuteInspection(prsName, gclName));
            m_MeasurementReceivedManualResetEvent.WaitOne(ContinuousMeasurementEventTimeout);

            // stop continuous measurements
            m_MeasurementsCompletedManualResetEvent.Reset();
            InspectionActivityControl.StopContinuousMeasurement();
            m_MeasurementsCompletedManualResetEvent.WaitOne(ContinuousMeasurementEventTimeout);

            // wait for finish inspection
            manualResetEvent.WaitOne(EventTimeout);

            // see if the inspection finished
            Assert.IsTrue(m_ExtraMeasurementsStartedEventFired);

            Assert.IsTrue(eventFired, "Expect that the event is fired");
            Assert.AreEqual(InspectionStatus.Completed, status);
            Assert.AreEqual(ErrorCodes.INSPECTION_FINISHED_SUCCESSFULLY, errorCode);
            Assert.IsTrue(sectionSelection.SectionSelectionEntities.All(sectionEntity => sectionEntity.IsSelected == false));

            InspectionActivityControl.InspectionFinished -= handler;
            InspectionActivityControl.ExecuteInspectionStep -= new EventHandler(executeInspectionStepEvent);
            InspectionActivityControl.MeasurementsCompleted -= InspectionActivityControl_MeasurementsCompleted;
            InspectionActivityControl.ExtraMeasurementStarted -= new EventHandler(extraMeasurementsReceived);
            InspectionActivityControl.MeasurementsReceived -= new EventHandler(measurementsReceived);
        }

        /// <summary>
        /// Executes the happy flow test with continuous measurements enabled.
        /// </summary>
        /// <param name="prsName">Name of the PRS.</param>
        /// <param name="gclName">Name of the GCL.</param>
        /// <param name="executeInspectionStepEvent"></param>
        /// <param name="measurementsReceived">The measurements received.</param>
        /// <param name="extraMeasurementsReceived"></param>
        private void ExecuteHappyFlowTestContinuousMeasurements5X70(string prsName, string gclName, Action<object, EventArgs> executeInspectionStepEvent, Action<object, EventArgs> measurementsReceived, Action<object, EventArgs> extraMeasurementsReceived)
        {
            var status = InspectionStatus.Unset;
            var sectionSelection = new SectionSelection();
            var errorCode = -1;
            var eventFired = false;

            var manualResetEvent = new ManualResetEvent(false);

            EventHandler handler = (sender, args) =>
            {
                Assert.IsInstanceOf<InspectionFinishedEventArgs>(args, "Expected InspectionFinishedEventArgs");

                var eventArgs = args as InspectionFinishedEventArgs;

                Assert.IsNotNull(eventArgs);

                status = eventArgs.Result;
                errorCode = eventArgs.ErrorCode;
                sectionSelection = eventArgs.PartialInspection;

                eventFired = true;

                manualResetEvent.Set();
            };

            m_ExtraMeasurementsStartedEventFired = false;

            InspectionActivityControl.MeasurementsReceived += new EventHandler(measurementsReceived);
            InspectionActivityControl.ExtraMeasurementStarted += new EventHandler(extraMeasurementsReceived);
            InspectionActivityControl.MeasurementsCompleted += InspectionActivityControl_MeasurementsCompleted;
            InspectionActivityControl.ExecuteInspectionStep += new EventHandler(executeInspectionStepEvent);
            InspectionActivityControl.InspectionFinished += handler;

            Assert.IsTrue(InspectionActivityControl.ExecuteInspection(prsName, gclName));

            manualResetEvent.WaitOne(ContinuousMeasurementEventTimeout);

            Assert.IsTrue(m_ExtraMeasurementsStartedEventFired);

            Assert.IsTrue(eventFired, "Expect that the event is fired");
            Assert.AreEqual(InspectionStatus.Completed, status);
            Assert.AreEqual(ErrorCodes.INSPECTION_FINISHED_SUCCESSFULLY, errorCode);
            Assert.IsTrue(sectionSelection.SectionSelectionEntities.All(sectionEntity => sectionEntity.IsSelected == false));

            InspectionActivityControl.InspectionFinished -= handler;
            InspectionActivityControl.ExecuteInspectionStep -= new EventHandler(executeInspectionStepEvent);
            InspectionActivityControl.MeasurementsCompleted -= InspectionActivityControl_MeasurementsCompleted;
            InspectionActivityControl.ExtraMeasurementStarted -= InspectionActivityControl_ExtraMeasurementStarted;
            InspectionActivityControl.MeasurementsReceived -= new EventHandler(measurementsReceived);
        }

        /// <summary>
        /// Executes the happy flow test with continuous measurements enabled.
        /// </summary>
        /// <param name="prsName">Name of the PRS.</param>
        /// <param name="gclName">Name of the GCL.</param>
        /// <param name="executeInspectionStepEvent"></param>
        /// <param name="measurementsReceived">The measurements received.</param>
        /// <param name="extraMeasurementsReceived"></param>
        private void ExecuteHappyFlowTestContinuousMeasurements(string prsName, string gclName, Action<object, EventArgs> executeInspectionStepEvent, Action<object, EventArgs> measurementsReceived, Action<object, EventArgs> extraMeasurementsReceived)
        {
            var status = InspectionStatus.Unset;
            var sectionSelection = new SectionSelection();
            var errorCode = -1;
            var eventFired = false;

            var manualResetEvent = new ManualResetEvent(false);

            EventHandler handler = (sender, args) =>
            {
                Assert.IsInstanceOf<InspectionFinishedEventArgs>(args, "Expected InspectionFinishedEventArgs");

                var eventArgs = args as InspectionFinishedEventArgs;

                Assert.IsNotNull(eventArgs);

                status = eventArgs.Result;
                errorCode = eventArgs.ErrorCode;
                sectionSelection = eventArgs.PartialInspection;

                eventFired = true;

                manualResetEvent.Set();
            };

            m_ExtraMeasurementsStartedEventFired = false;
            m_MeasurementResultFired = false;

            InspectionActivityControl.MeasurementsReceived += new EventHandler(measurementsReceived);
            InspectionActivityControl.ExtraMeasurementStarted += new EventHandler(extraMeasurementsReceived);
            InspectionActivityControl.MeasurementsCompleted += InspectionActivityControl_MeasurementsCompleted;
            InspectionActivityControl.ExecuteInspectionStep += new EventHandler(executeInspectionStepEvent);
            InspectionActivityControl.InspectionFinished += handler;
            InspectionActivityControl.MeasurementResult += InspectionActivityControl_MeasurementResult;

            Assert.IsTrue(InspectionActivityControl.ExecuteInspection(prsName, gclName));
            
            manualResetEvent.WaitOne(ContinuousMeasurementEventTimeout);

            Assert.IsTrue(m_ExtraMeasurementsStartedEventFired);

            Assert.IsTrue(eventFired, "Expect that the event is fired");
            Assert.AreEqual(InspectionStatus.Completed, status);
            Assert.AreEqual(ErrorCodes.INSPECTION_FINISHED_SUCCESSFULLY, errorCode);
            Assert.IsTrue(sectionSelection.SectionSelectionEntities.All(sectionEntity => sectionEntity.IsSelected == false));

            Assert.IsTrue(m_MeasurementResultFired, "MeasurementResult was fired");
            Assert.AreEqual(false, m_MeasurementValueOutOfLimits);

            InspectionActivityControl.MeasurementResult -= InspectionActivityControl_MeasurementResult;
            InspectionActivityControl.InspectionFinished -= handler;
            InspectionActivityControl.ExecuteInspectionStep -= new EventHandler(executeInspectionStepEvent);
            InspectionActivityControl.MeasurementsCompleted -= InspectionActivityControl_MeasurementsCompleted;
            InspectionActivityControl.ExtraMeasurementStarted -= InspectionActivityControl_ExtraMeasurementStarted;
            InspectionActivityControl.MeasurementsReceived -= new EventHandler(measurementsReceived);
        }

        private void ExecuteHappyFlowTestScriptCommand56Test(string prsName, string gclName, Action<object, EventArgs> executeInspectionStepEvent, Action<object, EventArgs> measurementsReceived, Action<object, EventArgs> extraMeasurementsReceived)
        {
            var status = InspectionStatus.Unset;
            var sectionSelection = new SectionSelection();
            var errorCode = -1;
            var eventFired = false;

            var manualResetEvent = new ManualResetEvent(false);

            void Handler (object sender, EventArgs args)
            {
                Assert.IsInstanceOf<InspectionFinishedEventArgs>(args, "Expected InspectionFinishedEventArgs");

                var eventArgs = args as InspectionFinishedEventArgs;

                Assert.IsNotNull(eventArgs);

                status = eventArgs.Result;
                errorCode = eventArgs.ErrorCode;
                sectionSelection = eventArgs.PartialInspection;

                eventFired = true;

                manualResetEvent.Set();
            }

            void SafetyValueTriggeredHandler(object sender, EventArgs e)
            {
                Console.WriteLine("safety value triggered, stopping continuous measurement");
                InspectionActivityControl.StopContinuousMeasurement(); //requested by michel, Ui wil always call StopContinuousMeasurement after safety value has triggered
            }

            m_ExtraMeasurementsStartedEventFired = false;
            m_MeasurementResultFired = false;

            InspectionActivityControl.MeasurementsReceived += new EventHandler(measurementsReceived);
            InspectionActivityControl.ExtraMeasurementStarted += new EventHandler(extraMeasurementsReceived);
            InspectionActivityControl.MeasurementsCompleted += InspectionActivityControl_MeasurementsCompleted;
            InspectionActivityControl.ExecuteInspectionStep += new EventHandler(executeInspectionStepEvent);
            InspectionActivityControl.InspectionFinished += Handler;
            InspectionActivityControl.MeasurementResult += InspectionActivityControl_MeasurementResult;
            InspectionActivityControl.SafetyValueTriggered += SafetyValueTriggeredHandler;
            Assert.IsTrue(InspectionActivityControl.ExecuteInspection(prsName, gclName));

            manualResetEvent.WaitOne(ContinuousMeasurementEventTimeout);

            //Assert.IsTrue(m_ExtraMeasurementsStartedEventFired);

            Assert.IsTrue(eventFired, "Expect that the event is fired");
            Assert.AreEqual(InspectionStatus.Completed, status);
            Assert.AreEqual(ErrorCodes.INSPECTION_FINISHED_SUCCESSFULLY, errorCode);
            Assert.IsTrue(sectionSelection.SectionSelectionEntities.All(sectionEntity => sectionEntity.IsSelected == false));

            Assert.IsTrue(m_MeasurementResultFired, "MeasurementResult was fired");
            Assert.AreEqual(false, m_MeasurementValueOutOfLimits);

            InspectionActivityControl.MeasurementResult -= InspectionActivityControl_MeasurementResult;
            InspectionActivityControl.InspectionFinished -= Handler;
            InspectionActivityControl.ExecuteInspectionStep -= new EventHandler(executeInspectionStepEvent);
            InspectionActivityControl.MeasurementsCompleted -= InspectionActivityControl_MeasurementsCompleted;
            InspectionActivityControl.ExtraMeasurementStarted -= InspectionActivityControl_ExtraMeasurementStarted;
            InspectionActivityControl.MeasurementsReceived -= new EventHandler(measurementsReceived);
            InspectionActivityControl.SafetyValueTriggered -= SafetyValueTriggeredHandler;
        }

        private void InspectionActivityControl_MeasurementResult(object sender, EventArgs args)
        {
            m_MeasurementResultFired = true;
            var eventArgs = args as MeasurementResultEventArgs;
            Assert.IsNotNull(eventArgs);
            m_MeasurementValueOutOfLimits = eventArgs.MeasurementValueOutOfLimits;
        }

        /// <summary>
        /// Executes the flow test with continuous measurements enabled and a value out of bounds expected.
        /// </summary>
        /// <param name="prsName">Name of the PRS.</param>
        /// <param name="gclName">Name of the GCL.</param>
        /// <param name="executeInspectionStepEvent"></param>
        /// <param name="measurementsReceived">The measurements received.</param>
        /// <param name="extraMeasurementsReceived"></param>
        private void ExecuteValueOutOfBoundsTestContinuousMeasurements(string prsName, string gclName, Action<object, EventArgs> executeInspectionStepEvent, Action<object, EventArgs> measurementsReceived, Action<object, EventArgs> extraMeasurementsReceived)
        {
            var status = InspectionStatus.Unset;
            var sectionSelection = new SectionSelection();
            var errorCode = -1;
            var eventFired = false;

            var manualResetEvent = new ManualResetEvent(false);

            EventHandler handler = (sender, args) =>
            {
                Assert.IsInstanceOf<InspectionFinishedEventArgs>(args, "Expected InspectionFinishedEventArgs");

                var eventArgs = args as InspectionFinishedEventArgs;

                Assert.IsNotNull(eventArgs);

                status = eventArgs.Result;
                errorCode = eventArgs.ErrorCode;
                sectionSelection = eventArgs.PartialInspection;

                eventFired = true;

                manualResetEvent.Set();
            };

            m_ExtraMeasurementsStartedEventFired = false;

            InspectionActivityControl.MeasurementsReceived += new EventHandler(measurementsReceived);
            InspectionActivityControl.ExtraMeasurementStarted += new EventHandler(extraMeasurementsReceived);
            InspectionActivityControl.MeasurementsCompleted += InspectionActivityControl_MeasurementsCompleted;
            InspectionActivityControl.ExecuteInspectionStep += new EventHandler(executeInspectionStepEvent);
            InspectionActivityControl.InspectionFinished += handler;
            InspectionActivityControl.MeasurementResult += InspectionActivityControl_MeasurementResult_OutOfBounds;

            Assert.IsTrue(InspectionActivityControl.ExecuteInspection(prsName, gclName));

            manualResetEvent.WaitOne(ContinuousMeasurementEventTimeout);

            Assert.IsTrue(m_ExtraMeasurementsStartedEventFired);

            Assert.IsTrue(eventFired, "Expect that the event is fired");
            Assert.AreEqual(InspectionStatus.CompletedValueOutOfLimits, status);
            Assert.AreEqual(ErrorCodes.INSPECTION_FINISHED_SUCCESSFULLY, errorCode);
            Assert.IsTrue(sectionSelection.SectionSelectionEntities.Any(sectionEntity => sectionEntity.IsSelected));
            
            Assert.IsTrue(m_MeasurementResultFired, "MeasurementResult was fired");
            Assert.AreEqual(true, m_MeasurementValueOutOfLimits);

            InspectionActivityControl.MeasurementResult -= InspectionActivityControl_MeasurementResult_OutOfBounds;
            InspectionActivityControl.InspectionFinished -= handler;
            InspectionActivityControl.ExecuteInspectionStep -= new EventHandler(executeInspectionStepEvent);
            InspectionActivityControl.MeasurementsCompleted -= InspectionActivityControl_MeasurementsCompleted;
            InspectionActivityControl.ExtraMeasurementStarted -= InspectionActivityControl_ExtraMeasurementStarted;
            InspectionActivityControl.MeasurementsReceived -= new EventHandler(measurementsReceived);
        }

        void InspectionActivityControl_MeasurementResult_OutOfBounds(object sender, EventArgs args)
        {
            m_MeasurementResultFired = true;
            var eventArgs = args as MeasurementResultEventArgs;
            Assert.IsNotNull(eventArgs);
            m_MeasurementValueOutOfLimits = eventArgs.MeasurementValueOutOfLimits;
        }


        /// <summary>
        /// Handles the MeasurementsCompleted event of the InspectionActivityControl control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void InspectionActivityControl_MeasurementsCompleted(object sender, EventArgs args)
        {
            Assert.IsInstanceOf<ExecuteInspectionStepEventArgs>(args, "Expected ExecuteInspectionStepEventArgs");
            var eventArgs = args as ExecuteInspectionStepEventArgs;
            Assert.IsNotNull(eventArgs);
            InspectionActivityControl.InspectionStepComplete(new InspectionStepResultEmpty(eventArgs.ScriptCommand.SequenceNumber));
            m_MeasurementsCompletedManualResetEvent.Set();
        }

        /// <summary>
        /// Handles the NoReply event of the InspectionActivityControl_MeasurementsCompleted control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void InspectionActivityControl_MeasurementsCompleted_NoReply(object sender, EventArgs args)
        {
            Assert.IsInstanceOf<ExecuteInspectionStepEventArgs>(args, "Expected ExecuteInspectionStepEventArgs");
            var eventArgs = args as ExecuteInspectionStepEventArgs;
            Assert.IsNotNull(eventArgs);
            m_MeasurementsCompletedManualResetEvent.Set();
        }

        /// <summary>
        /// Executes the test.
        /// </summary>
        /// <param name="inspectionProcedureName">Inspector procedure name</param>
        /// <param name="prsName">Name of the PRS.</param>
        /// <param name="gclName">Name of the GCL.</param>
        /// <param name="executeInspectionStepEvent">The execute inspection step event.</param>
        private static void ExecuteHappyFlowTest(string inspectionProcedureName, string prsName, string gclName, Action<object, EventArgs> executeInspectionStepEvent)
        {
            var status = InspectionStatus.Unset;
            var sectionSelection = new SectionSelection();
            var errorCode = -1;
            var eventFired = false;

            var manualResetEvent = new ManualResetEvent(false);

            EventHandler handler = (sender, args) =>
            {
                Assert.IsInstanceOf<InspectionFinishedEventArgs>(args, "Expected InspectionFinishedEventArgs");

                var eventArgs = args as InspectionFinishedEventArgs;

                Assert.IsNotNull(eventArgs);

                status = eventArgs.Result;
                errorCode = eventArgs.ErrorCode;
                sectionSelection = eventArgs.PartialInspection;

                eventFired = true;

                manualResetEvent.Set();
            };

            InspectionActivityControl.ExecuteInspectionStep += new EventHandler(executeInspectionStepEvent);
            InspectionActivityControl.InspectionFinished += handler;

            Assert.IsTrue(InspectionActivityControl.ExecuteInspection(inspectionProcedureName, prsName, gclName));

            manualResetEvent.WaitOne(EventTimeout);

            Assert.IsTrue(eventFired, "Expect that the event is fired");
            Assert.AreEqual(InspectionStatus.Completed, status);
            Assert.AreEqual(ErrorCodes.INSPECTION_FINISHED_SUCCESSFULLY, errorCode);
            Assert.IsNull(sectionSelection);

            InspectionActivityControl.InspectionFinished -= handler;
            InspectionActivityControl.ExecuteInspectionStep -= new EventHandler(executeInspectionStepEvent);
        }

        /// <summary>
        /// Executes the test.
        /// </summary>
        /// <param name="prsName">Name of the PRS.</param>
        /// <param name="gclName">Name of the GCL.</param>
        /// <param name="executeInspectionStepEvent">The execute inspection step event.</param>
        private static void ExecuteAbortTest(string prsName, string gclName, Action<object, EventArgs> executeInspectionStepEvent)
        {
            var status = InspectionStatus.Unset;
            var sectionSelection = new SectionSelection();
            var errorCode = -1;
            var eventFired = false;

            var manualResetEvent = new ManualResetEvent(false);

            EventHandler handler = (sender, args) =>
            {
                Assert.IsInstanceOf<InspectionFinishedEventArgs>(args, "Expected InspectionFinishedEventArgs");

                var eventArgs = args as InspectionFinishedEventArgs;

                Assert.IsNotNull(eventArgs);

                status = eventArgs.Result;
                errorCode = eventArgs.ErrorCode;
                sectionSelection = eventArgs.PartialInspection;

                eventFired = true;

                manualResetEvent.Set();
            };

            InspectionActivityControl.ExecuteInspectionStep += new EventHandler(executeInspectionStepEvent);
            InspectionActivityControl.InspectionFinished += handler;

            Assert.IsTrue(InspectionActivityControl.ExecuteInspection(prsName, gclName));

            manualResetEvent.WaitOne(EventTimeout);

            Assert.IsTrue(eventFired, "Expect that the event is fired");
            Assert.AreEqual(InspectionStatus.StartNotCompleted, status);
            Assert.AreEqual(ErrorCodes.INSPECTION_ABORTED_BY_USER, errorCode);
            Assert.IsNull(sectionSelection);

            InspectionActivityControl.InspectionFinished -= handler;
            InspectionActivityControl.ExecuteInspectionStep -= new EventHandler(executeInspectionStepEvent);
        }

        /// <summary>
        /// Sets up happy flow ScriptCommand5X TH2 stack.
        /// </summary>
        private static void SetUpHappyFlowSC5XTH2Stack(DeviceType deviceType)
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
            }

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
            reactionStack.Push("mbar");                             // Query pressure unit on TH2
            reactionStack.Push("ok");                               // Set Manometer Range on TH2
            reactionStack.Push("\"17 bar\"");                       // Query Manometer Range on TH2
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

            var stub = Hal as BluetoothHalSequentialStub;
            
            Assert.IsNotNull(stub);

            stub.DeviceType = deviceType;
            stub.SetUpReactionStack(reactionStack);
        }

        private static void SetupHappyFlowScriptCommand56Stack(DeviceType deviceType, bool includeSingleMeasurementResponse)
        {
            var reactionStack = new Stack<string>();

            if (deviceType == DeviceType.PlexorBluetoothWIS && includeSingleMeasurementResponse)
            {
                reactionStack.Push("5");                            // response to measure single value.
            }

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
            reactionStack.Push("mbar");                             // Query pressure unit on TH2
            reactionStack.Push("ok");                               // Set Manometer Range on TH2
            reactionStack.Push("\"17 bar\"");                       // Query Manometer Range on TH2
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
        /// Sets up happy flow ScriptCommand5X TH2 stack.
        /// </summary>
        private static void SetUpHappyFlowSC5XTH2ManualStopStartStack(DeviceType deviceType)
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
            }

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
            reactionStack.Push("mbar");                             // Query pressure unit on TH2
            reactionStack.Push("ok");                               // Set Manometer Range on TH2
            reactionStack.Push("\"17 bar\"");                       // Query Manometer Range on TH2
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

            var stub = Hal as BluetoothHalSequentialStub;

            Assert.IsNotNull(stub);

            stub.DeviceType = deviceType;
            stub.SetUpReactionStack(reactionStack);
        }

        #endregion Generic Handlers

        #region Test Unknown PRS/GCL
        /// <summary>
        /// Inspections the script command1 test.
        /// </summary>
        [Test]
        public void InspectionUnknownPrsGclPartialInspectionTest()
        {
                       Console.WriteLine("InspectionUnknownPrsGclPartialInspectionTest");
            var status = InspectionStatus.Unset;
            var sectionSelection = new SectionSelection();
            var errorCode = -1;
            var eventFired = false;

            var manualResetEvent = new ManualResetEvent(false);

            EventHandler handler = (sender, args) =>
            {
                Assert.IsInstanceOf<InspectionFinishedEventArgs>(args, "Expected InspectionFinishedEventArgs");

                var eventArgs = args as InspectionFinishedEventArgs;

                Assert.IsNotNull(eventArgs);

                status = eventArgs.Result;
                errorCode = eventArgs.ErrorCode;
                sectionSelection = eventArgs.PartialInspection;

                eventFired = true;

                manualResetEvent.Set();
            };

            InspectionActivityControl.InspectionFinished += handler;

            Assert.IsTrue(InspectionActivityControl.ExecutePartialInspection(sectionSelection, "UnknownPrs", "UnknownGcl"));

            manualResetEvent.WaitOne(EventTimeout);

            Assert.IsTrue(eventFired, "Expect that the event is fired");
            Assert.AreEqual(InspectionStatus.StartNotCompleted, status);
            Assert.AreEqual(ErrorCodes.INSPECTION_COULD_NOT_RETRIEVE_INSPECTION, errorCode);
            Assert.IsNull(sectionSelection);

            InspectionActivityControl.InspectionFinished -= handler;
        }

        /// <summary>
        /// Inspections the script command1 test.
        /// </summary>
        [Test]
        public void InspectionUnknownPrsGclTest()
        {
                       Console.WriteLine("InspectionUnknownPrsGclTest");
            var status = InspectionStatus.Unset;
            var sectionSelection = new SectionSelection();
            var errorCode = -1;
            var eventFired = false;

            var manualResetEvent = new ManualResetEvent(false);

            EventHandler handler = (sender, args) =>
            {
                Assert.IsInstanceOf<InspectionFinishedEventArgs>(args, "Expected InspectionFinishedEventArgs");

                var eventArgs = args as InspectionFinishedEventArgs;

                Assert.IsNotNull(eventArgs);

                status = eventArgs.Result;
                errorCode = eventArgs.ErrorCode;
                sectionSelection = eventArgs.PartialInspection;

                eventFired = true;

                manualResetEvent.Set();
            };

            InspectionActivityControl.InspectionFinished += handler;

            Assert.IsTrue(InspectionActivityControl.ExecuteInspection("UnknownPrs", "UnknownGcl"));

            manualResetEvent.WaitOne(EventTimeout);

            Assert.IsTrue(eventFired, "Expect that the event is fired");
            Assert.AreEqual(InspectionStatus.StartNotCompleted, status);
            Assert.AreEqual(ErrorCodes.INSPECTION_COULD_NOT_RETRIEVE_INSPECTION_PROCEDURE_NAME, errorCode);
            Assert.IsNull(sectionSelection);

            InspectionActivityControl.InspectionFinished -= handler;
        }
        #endregion Test Unknown PRS/GCL

        #region Test ScriptCommand Incorrect Sequence Number
        /// <summary>
        /// Inspections the script command1 test.
        /// </summary>
        [Test]
        public void InspectionIncorrectSequenceNumberTest()
        {
                       Console.WriteLine("InspectionIncorrectSequenceNumberTest");
            var status = InspectionStatus.Unset;
            var sectionSelection = new SectionSelection();
            var errorCode = -1;
            var eventFired = false;

            var manualResetEvent = new ManualResetEvent(false);

            EventHandler handler = (sender, args) =>
            {
                Assert.IsInstanceOf<InspectionFinishedEventArgs>(args, "Expected InspectionFinishedEventArgs");

                var eventArgs = args as InspectionFinishedEventArgs;

                Assert.IsNotNull(eventArgs);

                status = eventArgs.Result;
                errorCode = eventArgs.ErrorCode;
                sectionSelection = eventArgs.PartialInspection;

                eventFired = true;

                manualResetEvent.Set();
            };

            InspectionActivityControl.ExecuteInspectionStep += InspectionActivityControl_ExecuteInspectionStep_IncorrectSequence;
            InspectionActivityControl.InspectionFinished += handler;

            Assert.IsTrue(InspectionActivityControl.ExecuteInspection("TestScriptCommand1PRS", "TestScriptCommand1GCL"));

            manualResetEvent.WaitOne(EventTimeout);

            Assert.IsTrue(eventFired, "Expect that the event is fired");
            Assert.AreEqual(InspectionStatus.StartNotCompleted, status);
            Assert.AreEqual(ErrorCodes.INSPECTION_INCORRECT_SEQUENCE_NUMBER, errorCode);
            Assert.IsNull(sectionSelection);

            InspectionActivityControl.InspectionFinished -= handler;
            InspectionActivityControl.ExecuteInspectionStep -= InspectionActivityControl_ExecuteInspectionStep_IncorrectSequence;
        }

        /// <summary>
        /// Handles the ScriptCommand1Test event of the InspectionActivityControl_ExecuteInspectionStep control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private static void InspectionActivityControl_ExecuteInspectionStep_IncorrectSequence(object sender, EventArgs args)
        {
            Assert.IsInstanceOf<ExecuteInspectionStepEventArgs>(args, "Expected ExecuteInspectionStepEventArgs");
            InspectionActivityControl.InspectionStepComplete(new InspectionStepResultEmpty(12));
        }
        #endregion Test ScriptCommand Incorrect Sequence Number

        #region Test TestScriptCommand1 with inspectionProcedureName
        /// <summary>
        /// Inspections the script command1 testwith inspectionProcedureName.
        /// </summary>
        [Test]
        public void InspectionScriptCommand1WithInspectionProcedureNameTest()
        {
                       Console.WriteLine("InspectionScriptCommand1WithInspectionProcedureNameTest");
            ExecuteHappyFlowTest("TestScriptCommand1", "TestScriptCommand1PRS", "TestScriptCommand1GCL", InspectionActivityControl_ExecuteInspectionStep_ScriptCommand1WithInspectionProcedureNameTest);
        }

        /// <summary>
        /// Handles the ScriptCommand1Test event of the InspectionActivityControl_ExecuteInspectionStep control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private static void InspectionActivityControl_ExecuteInspectionStep_ScriptCommand1WithInspectionProcedureNameTest(object sender, EventArgs args)
        {
            Assert.IsInstanceOf<ExecuteInspectionStepEventArgs>(args, "Expected ExecuteInspectionStepEventArgs");
            var eventArgs = args as ExecuteInspectionStepEventArgs;
            Assert.IsNotNull(eventArgs);
            var sc1 = eventArgs.ScriptCommand as ScriptCommand1;
            Assert.IsNotNull(sc1);
            
            switch (sc1.SequenceNumber)
            {
                case 1:
                    Assert.AreEqual("Text1", sc1.Text);
                    break;
                case 2:
                    Assert.AreEqual("Text2", sc1.Text);
                    break;
            }

            InspectionActivityControl.InspectionStepComplete(new InspectionStepResultEmpty(sc1.SequenceNumber));
        }
        #endregion Test TestScriptCommand1

        #region Test Inspection with different inspectionProcedureName
        /// <summary>
        /// Inspections the script command1 testwith inspectionProcedureName.
        /// </summary>
        [Test]
        public void InspectionWithDifferentInspectionProcedureNameTest()
        {
                       Console.WriteLine("InspectionWithDifferentInspectionProcedureNameTest");
            ExecuteHappyFlowTest("TestScriptCommand2", "TestScriptCommand1PRS", "TestScriptCommand1GCL", InspectionActivityControl_ExecuteInspectionStep_DifferentInspectionProcedureNameTest);
        }

        /// <summary>
        /// Handles the ScriptCommand1Test event of the InspectionActivityControl_ExecuteInspectionStep control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private static void InspectionActivityControl_ExecuteInspectionStep_DifferentInspectionProcedureNameTest(object sender, EventArgs args)
        {
            Assert.IsInstanceOf<ExecuteInspectionStepEventArgs>(args, "Expected ExecuteInspectionStepEventArgs");
            var eventArgs = args as ExecuteInspectionStepEventArgs;
            Assert.IsNotNull(eventArgs);
            var sc2 = eventArgs.ScriptCommand as ScriptCommand2;
            Assert.IsNotNull(sc2);

            switch (sc2.SequenceNumber)
            {
                case 1:
                    Assert.AreEqual("Section1", sc2.Section);
                    Assert.AreEqual("SubSection1", sc2.SubSection);
                    break;
                case 2:
                    Assert.AreEqual("Section1", sc2.Section);
                    Assert.AreEqual("SubSection2", sc2.SubSection);
                    break;
            }

            InspectionActivityControl.InspectionStepComplete(new InspectionStepResultEmpty(sc2.SequenceNumber));
        }
        #endregion Test Inspection with different inspectionProcedureName

        #region Test TestPRS
        /// <summary>
        /// Inspections the script command1 test.
        /// </summary>
        [Test]
        public void InspectionPRSTest()
        {
                       Console.WriteLine("InspectionPRSTest");
            ExecuteHappyFlowTest("TestScriptCommand1PRS", null, InspectionActivityControl_ExecuteInspectionStep_PRSTest);
        }

        /// <summary>
        /// Handles the ScriptCommand1Test event of the InspectionActivityControl_ExecuteInspectionStep control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private static void InspectionActivityControl_ExecuteInspectionStep_PRSTest(object sender, EventArgs args)
        {
            Assert.IsInstanceOf<ExecuteInspectionStepEventArgs>(args, "Expected ExecuteInspectionStepEventArgs");
            var eventArgs = args as ExecuteInspectionStepEventArgs;
            Assert.IsNotNull(eventArgs);
            InspectionStepResultBase inspectionStepResult = null;

            if (eventArgs.ScriptCommand is ScriptCommand1 || eventArgs.ScriptCommand is ScriptCommand2 || eventArgs.ScriptCommand is ScriptCommand3 || eventArgs.ScriptCommand is ScriptCommand42)
            {
                inspectionStepResult = new InspectionStepResultEmpty(eventArgs.ScriptCommand.SequenceNumber);
            }
            else if (eventArgs.ScriptCommand is ScriptCommand4 || eventArgs.ScriptCommand is ScriptCommand43)
            {
                inspectionStepResult = new InspectionStepResultText(eventArgs.ScriptCommand.SequenceNumber, "test");
            }
            else if (eventArgs.ScriptCommand is ScriptCommand41)
            {
                inspectionStepResult = new InspectionStepResultSelections(eventArgs.ScriptCommand.SequenceNumber, "Ja", "Nee");
            }

            InspectionActivityControl.InspectionStepComplete(inspectionStepResult);
        }
        #endregion Test TestScriptCommand1

        #region Test TestScriptCommand1
        /// <summary>
        /// Inspections the script command1 test.
        /// </summary>
        [Test]
        public void InspectionScriptCommand1Test()
        {
                       Console.WriteLine("InspectionScriptCommand1Test");
            ExecuteHappyFlowTest("TestScriptCommand1PRS", "TestScriptCommand1GCL", InspectionActivityControl_ExecuteInspectionStep_ScriptCommand1Test);
        }

        /// <summary>
        /// Handles the ScriptCommand1Test event of the InspectionActivityControl_ExecuteInspectionStep control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private static void InspectionActivityControl_ExecuteInspectionStep_ScriptCommand1Test(object sender, EventArgs args)
        {
            Assert.IsInstanceOf<ExecuteInspectionStepEventArgs>(args, "Expected ExecuteInspectionStepEventArgs");
            var eventArgs = args as ExecuteInspectionStepEventArgs;
            Assert.IsNotNull(eventArgs);
            var sc1 = eventArgs.ScriptCommand as ScriptCommand1;
            Assert.IsNotNull(sc1);

            switch (sc1.SequenceNumber)
            {
                case 1:
                    Assert.AreEqual("Text1", sc1.Text);
                    break;
                case 2:
                    Assert.AreEqual("Text2", sc1.Text);
                    break;
            }

            InspectionActivityControl.InspectionStepComplete(new InspectionStepResultEmpty(sc1.SequenceNumber));
        }
        #endregion Test TestScriptCommand1

        #region Test TestScriptCommand2
        /// <summary>
        /// Inspections the script command2 test.
        /// </summary>
        [Test]
        public void InspectionScriptCommand2Test()
        {
                       Console.WriteLine("InspectionScriptCommand2Test");
            ExecuteHappyFlowTest("TestScriptCommand1PRS", "TestScriptCommand2GCL", InspectionActivityControl_ExecuteInspectionStep_ScriptCommand2Test);
        }

        /// <summary>
        /// Handles the ScriptCommand1Test event of the InspectionActivityControl_ExecuteInspectionStep control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private static void InspectionActivityControl_ExecuteInspectionStep_ScriptCommand2Test(object sender, EventArgs args)
        {
            Assert.IsInstanceOf<ExecuteInspectionStepEventArgs>(args, "Expected ExecuteInspectionStepEventArgs");
            var eventArgs = args as ExecuteInspectionStepEventArgs;
            Assert.IsNotNull(eventArgs);
            var sc2 = eventArgs.ScriptCommand as ScriptCommand2;
            Assert.IsNotNull(sc2);
            
            switch (sc2.SequenceNumber)
            {
                case 1:
                    Assert.AreEqual("Section1", sc2.Section);
                    Assert.AreEqual("SubSection1", sc2.SubSection);
                    break;
                case 2:
                    Assert.AreEqual("Section1", sc2.Section);
                    Assert.AreEqual("SubSection2", sc2.SubSection);
                    break;
            }

            InspectionActivityControl.InspectionStepComplete(new InspectionStepResultEmpty(sc2.SequenceNumber));
        }
        #endregion Test TestScriptCommand1

        #region Test TestScriptCommand3
        /// <summary>
        /// Inspections the script command2 test.
        /// </summary>
        [Test]
        public void InspectionScriptCommand3Test()
        {
                       Console.WriteLine("InspectionScriptCommand3Test");
            ExecuteHappyFlowTest("TestScriptCommand1PRS", "TestScriptCommand3GCL", InspectionActivityControl_ExecuteInspectionStep_ScriptCommand3Test);
        }

        /// <summary>
        /// Handles the ScriptCommand1Test event of the InspectionActivityControl_ExecuteInspectionStep control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private static void InspectionActivityControl_ExecuteInspectionStep_ScriptCommand3Test(object sender, EventArgs args)
        {
            Assert.IsInstanceOf<ExecuteInspectionStepEventArgs>(args, "Expected ExecuteInspectionStepEventArgs");
            var eventArgs = args as ExecuteInspectionStepEventArgs;
            Assert.IsNotNull(eventArgs);
            var sc3 = eventArgs.ScriptCommand as ScriptCommand3;
            Assert.IsNotNull(sc3);
            
            switch (sc3.SequenceNumber)
            {
                case 1:
                    Assert.AreEqual("Text1", sc3.Text);
                    Assert.AreEqual(60, sc3.Duration);
                    break;
                case 2:
                    Assert.AreEqual("Text2", sc3.Text);
                    Assert.AreEqual(120, sc3.Duration);
                    break;
            }

            InspectionActivityControl.InspectionStepComplete(new InspectionStepResultEmpty(sc3.SequenceNumber));
        }
        #endregion Test TestScriptCommand1

        #region Test TestScriptCommand4
        /// <summary>
        /// Inspections the script command4 test.
        /// </summary>
        [Test]
        public void InspectionScriptCommand4Test()
        {
                       Console.WriteLine("InspectionScriptCommand4Test");
            ExecuteHappyFlowTest("TestScriptCommand1PRS", "TestScriptCommand4GCL", InspectionActivityControl_ExecuteInspectionStep_ScriptCommand4Test);
        }

        /// <summary>
        /// Handles the ScriptCommand1Test event of the InspectionActivityControl_ExecuteInspectionStep control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private static void InspectionActivityControl_ExecuteInspectionStep_ScriptCommand4Test(object sender, EventArgs args)
        {
            Assert.IsInstanceOf<ExecuteInspectionStepEventArgs>(args, "Expected ExecuteInspectionStepEventArgs");
            var eventArgs = args as ExecuteInspectionStepEventArgs;
            Assert.IsNotNull(eventArgs);
            var sc4 = eventArgs.ScriptCommand as ScriptCommand4;
            Assert.IsNotNull(sc4);
            
            if (sc4.SequenceNumber == 1)
            {
                Assert.AreEqual("Question1", sc4.Question);
                Assert.AreEqual(TypeQuestion.InputMultiLines, sc4.TypeQuestion);
                Assert.AreEqual(3, sc4.TextOptions.Count);
                Assert.AreEqual(String.Empty, sc4.TextOptions[0]);
                Assert.AreEqual(String.Empty, sc4.TextOptions[1]);
                Assert.AreEqual(String.Empty, sc4.TextOptions[2]);
            }
            if (sc4.SequenceNumber == 2)
            {
                Assert.AreEqual("Question2", sc4.Question);
                Assert.AreEqual(TypeQuestion.InputSingleLine, sc4.TypeQuestion);
                Assert.AreEqual(3, sc4.TextOptions.Count);
                Assert.AreEqual(String.Empty, sc4.TextOptions[0]);
                Assert.AreEqual(String.Empty, sc4.TextOptions[1]);
                Assert.AreEqual(String.Empty, sc4.TextOptions[2]);
            }
            if (sc4.SequenceNumber == 3)
            {
                Assert.AreEqual("Question3", sc4.Question);
                Assert.AreEqual(TypeQuestion.TwoOptions, sc4.TypeQuestion);
                Assert.AreEqual(3, sc4.TextOptions.Count);
                Assert.AreEqual("Option2_1", sc4.TextOptions[0]);
                Assert.AreEqual("Option2_2", sc4.TextOptions[1]);
                Assert.AreEqual(String.Empty, sc4.TextOptions[2]);
            }
            if (sc4.SequenceNumber == 4)
            {
                Assert.AreEqual("Question4", sc4.Question);
                Assert.AreEqual(TypeQuestion.ThreeOptions, sc4.TypeQuestion);
                Assert.AreEqual(3, sc4.TextOptions.Count);
                Assert.AreEqual("Option3_1", sc4.TextOptions[0]);
                Assert.AreEqual("Option3_2", sc4.TextOptions[1]);
                Assert.AreEqual("Option3_3", sc4.TextOptions[2]);
            }

            InspectionActivityControl.InspectionStepComplete(new InspectionStepResultText(sc4.SequenceNumber, sc4.Question));
        }
        #endregion Test TestScriptCommand1

        #region Test TestScriptCommand41
        /// <summary>
        /// Inspections the script command41 test.
        /// </summary>
        [Test]
        public void InspectionScriptCommand41Test()
        {
                       Console.WriteLine("InspectionScriptCommand41Test");
            ExecuteHappyFlowTest("TestScriptCommand1PRS", "TestScriptCommand41GCL", InspectionActivityControl_ExecuteInspectionStep_ScriptCommand41Test);
        }

        /// <summary>
        /// Handles the ScriptCommand1Test event of the InspectionActivityControl_ExecuteInspectionStep control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private static void InspectionActivityControl_ExecuteInspectionStep_ScriptCommand41Test(object sender, EventArgs args)
        {
            Assert.IsInstanceOf<ExecuteInspectionStepEventArgs>(args, "Expected ExecuteInspectionStepEventArgs");
            var eventArgs = args as ExecuteInspectionStepEventArgs;
            Assert.IsNotNull(eventArgs);
            var sc41 = eventArgs.ScriptCommand as ScriptCommand41;
            Assert.IsNotNull(sc41);

            if (sc41.SequenceNumber == 1)
            {
                Assert.AreEqual(2, sc41.ScriptCommandList.Count);
                Assert.AreEqual("Question1", sc41.ScriptCommandList[0].ListQuestion);
                Assert.AreEqual(3, sc41.ScriptCommandList[0].ListConditionCodes.Count);
                Assert.AreEqual("Ja", sc41.ScriptCommandList[0].ListConditionCodes[0].ConditionCode);
                Assert.AreEqual("Nee", sc41.ScriptCommandList[0].ListConditionCodes[1].ConditionCode);
                Assert.AreEqual("nvt", sc41.ScriptCommandList[0].ListConditionCodes[2].ConditionCode);
                Assert.AreEqual(2, sc41.ScriptCommandList[1].ListConditionCodes.Count);
                Assert.AreEqual("Yes", sc41.ScriptCommandList[1].ListConditionCodes[0].ConditionCode);
                Assert.AreEqual("No", sc41.ScriptCommandList[1].ListConditionCodes[1].ConditionCode);
            }

            InspectionActivityControl.InspectionStepComplete(new InspectionStepResultSelections(sc41.SequenceNumber, "Ja", "Nee"));
        }
        #endregion Test TestScriptCommand1

        #region Test TestScriptCommand42
        /// <summary>
        /// Inspections the script command42 test.
        /// </summary>
        [Test]
        public void InspectionScriptCommand42Test()
        {
                       Console.WriteLine("InspectionScriptCommand42Test");

            ExecuteHappyFlowTest("TestScriptCommand1PRS", "TestScriptCommand42GCL", InspectionActivityControl_ExecuteInspectionStep_ScriptCommand42Test);
        }

        /// <summary>
        /// Handles the ScriptCommand1Test event of the InspectionActivityControl_ExecuteInspectionStep control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private static void InspectionActivityControl_ExecuteInspectionStep_ScriptCommand42Test(object sender, EventArgs args)
        {
            Assert.IsInstanceOf<ExecuteInspectionStepEventArgs>(args, "Expected ExecuteInspectionStepEventArgs");
            var eventArgs = args as ExecuteInspectionStepEventArgs;
            Assert.IsNotNull(eventArgs);
            var sc42 = eventArgs.ScriptCommand as ScriptCommand42;
            Assert.IsNotNull(sc42);
            
            switch (sc42.SequenceNumber)
            {
                case 1:
                    // no checks needed
                    break;
                case 2:
                    InspectionActivityControl.StoreRemark(new InspectionStepResultText(1, "Remark"));
                    break;
            }

            InspectionActivityControl.InspectionStepComplete(new InspectionStepResultEmpty(sc42.SequenceNumber));
        }
        #endregion Test TestScriptCommand42

        #region Test TestScriptCommand43
        /// <summary>
        /// Inspections the script command43 test.
        /// </summary>
        [Test]
        public void InspectionScriptCommand43Test()
        {
           Console.WriteLine("InspectionScriptCommand43Test");
            ExecuteHappyFlowTest("TestScriptCommand1PRS", "TestScriptCommand43GCL", InspectionActivityControl_ExecuteInspectionStep_ScriptCommand43Test);
        }

        /// <summary>
        /// Handles the ScriptCommand1Test event of the InspectionActivityControl_ExecuteInspectionStep control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private static void InspectionActivityControl_ExecuteInspectionStep_ScriptCommand43Test(object sender, EventArgs args)
        {
            Assert.IsInstanceOf<ExecuteInspectionStepEventArgs>(args, "Expected ExecuteInspectionStepEventArgs");
            var eventArgs = args as ExecuteInspectionStepEventArgs;
            Assert.IsNotNull(eventArgs);
            var sc43 = eventArgs.ScriptCommand as ScriptCommand43;
            Assert.IsNotNull(sc43);
            
            if (sc43.SequenceNumber == 1)
            {
                Assert.AreEqual("Instruction", sc43.Instruction);
                Assert.AreEqual(8, sc43.ListItems.Count);
                Assert.AreEqual("A", sc43.ListItems[0]);
                Assert.AreEqual("B", sc43.ListItems[1]);
                Assert.AreEqual("C", sc43.ListItems[2]);
                Assert.AreEqual("D", sc43.ListItems[3]);
                Assert.AreEqual("E", sc43.ListItems[4]);
                Assert.AreEqual("F", sc43.ListItems[5]);
                Assert.AreEqual("G", sc43.ListItems[6]);
                Assert.AreEqual("H", sc43.ListItems[7]);
            }

            InspectionActivityControl.InspectionStepComplete(new InspectionStepResultText(sc43.SequenceNumber, sc43.Instruction));
        }
        #endregion Test TestScriptCommand1

        #region Test TestScriptCommand5X
        [TestCase(DeviceType.PlexorBluetoothWIS)]
        [TestCase(DeviceType.PlexorBluetoothIrDA)]
        public void InspectionScriptCommand5XTest(DeviceType deviceType)
        {
           Console.WriteLine("InspectionScriptCommand5XTest");
            SetUpHappyFlowSC5XTH2Stack(deviceType);

            ExecuteHappyFlowTestContinuousMeasurements("TestScriptCommand1PRS", "TestScriptCommand5XGCL", InspectionActivityControl_ExecuteInspectionStep_ScriptCommand5XTest, InspectionActivityControl_MeasurementsReceived, InspectionActivityControl_ExtraMeasurementStarted);
        }

        #region Test TestScriptCommand5X
        [TestCase(DeviceType.PlexorBluetoothWIS)]
        [TestCase(DeviceType.PlexorBluetoothIrDA)]
        public void InspectionScriptCommand5XTestNegativeIoStatus(DeviceType deviceType)
        {
            Console.WriteLine("InspectionScriptCommand5XTest");
            SetUpHappyFlowSC5XTH2Stack(deviceType);
            BluetoothHalSequentialStub.CURRENT_IO_STATUS = -1;
            ExecuteHappyFlowTestContinuousMeasurements("TestScriptCommand1PRS", "TestScriptCommand5XGCL", InspectionActivityControl_ExecuteInspectionStep_ScriptCommand5XTest, InspectionActivityControl_MeasurementsReceived, InspectionActivityControl_ExtraMeasurementStarted);
        }
        #endregion
        /// <summary>
        /// Handles the ScriptCommand5XTest event of the InspectionActivityControl_ExecuteInspectionStep control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private static void InspectionActivityControl_ExecuteInspectionStep_ScriptCommand5XTest(object sender, EventArgs args)
        {
            Assert.IsInstanceOf<ExecuteInspectionStepEventArgs>(args, "Expected ExecuteInspectionStepEventArgs");
            var eventArgs = args as ExecuteInspectionStepEventArgs;
            Assert.IsNotNull(eventArgs);
            System.Diagnostics.Debug.WriteLine("InspectionActivityControl_ExecuteInspectionStep_ScriptCommand5XTest: Current Inspection Step '{0}', Sequencenumber: '{1}'", eventArgs.CurrentInspectionStep, eventArgs.ScriptCommand.SequenceNumber);

            if (eventArgs.ScriptCommand is ScriptCommand2)
            {
                InspectionActivityControl.InspectionStepComplete(new InspectionStepResultEmpty(eventArgs.ScriptCommand.SequenceNumber));
            }
        }
        #endregion Test TestScriptCommand5X

        #region Test TestScriptCommand5X manual restart with extra measurement time
        [TestCase(DeviceType.PlexorBluetoothWIS)]
        [TestCase(DeviceType.PlexorBluetoothIrDA)]
        public void InspectionScriptCommand5XUserRestartWithExtraMeasurementTest(DeviceType deviceType)
        {
           Console.WriteLine("InspectionScriptCommand5XUserRestartWithExtraMeasurementTest");
            SetUpHappyFlowSC5XTH2ManualStopStartStack(deviceType);

            ExecuteHappyFlowTestRestartContinuousMeasurementsWithExtraMeasurementPeriod("TestScriptCommand1PRS", "TestScriptCommand5XGCLZeroMeasurementAndExtraMeasurementTime", InspectionActivityControl_ExecuteInspectionStep_ScriptCommand5XTest, InspectionActivityControl_One_MeasurementsReceived_Executed, InspectionActivityControl_ExtraMeasurementStarted);
        }
        #endregion Test TestScriptCommand5X manual restart with extra measurement time

        #region Test TestScriptCommand5X manual user stop with extra measurement time
        [TestCase(DeviceType.PlexorBluetoothWIS)]
        [TestCase(DeviceType.PlexorBluetoothIrDA)]
        public void InspectionScriptCommand5XUserStopWithExtraMeasurementTest(DeviceType deviceType)
        {
           Console.WriteLine("InspectionScriptCommand5XUserStopWithExtraMeasurementTest");
            SetUpHappyFlowSC5XTH2ManualStopStartStack(deviceType);

            ExecuteHappyFlowTestStopContinuousMeasurementsWithExtraMeasurementPeriod("TestScriptCommand1PRS", "TestScriptCommand5XGCLZeroMeasurementAndExtraMeasurementTime", InspectionActivityControl_ExecuteInspectionStep_ScriptCommand5XTest, InspectionActivityControl_One_MeasurementsReceived_Executed, InspectionActivityControl_ExtraMeasurementStarted);
        }
        #endregion Test TestScriptCommand5X manual user stop with extra measurement time

        #region Test TestScriptCommand5X manual user stop (0 measurement time defined)
        [TestCase(DeviceType.PlexorBluetoothWIS)]
        [TestCase(DeviceType.PlexorBluetoothIrDA)]
        public void InspectionScriptCommand5XManualUserStopZeroMeasurementTimeDefinedTest(DeviceType deviceType)
        {
           Console.WriteLine("InspectionScriptCommand5XManualUserStopZeroMeasurementTimeDefinedTest");
            SetUpHappyFlowSC5XTH2Stack(deviceType);

            ExecuteHappyFlowTestContinuousMeasurementsManualUserStop("TestScriptCommand1PRS", "TestScriptCommand5XGCLZeroMeasurement", InspectionActivityControl_One_MeasurementsReceived_Executed);
        }

        /// <summary>
        /// Handles the Executed event of the InspectionActivityControl_One_MeasurementsReceived control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void InspectionActivityControl_One_MeasurementsReceived_Executed(object sender, EventArgs args)
        {
            Assert.IsInstanceOf<MeasurementEventArgs>(args, "Expected MeasurementEventArgs");
            var eventArgs = args as MeasurementEventArgs;
            Assert.IsNotNull(eventArgs);
            Assert.AreEqual(10, eventArgs.Measurements.Count);
            m_MeasurementReceivedManualResetEvent.Set();
        }
        #endregion Test TestScriptCommand5X manual user stop (0 measurement time)

        #region Test TestScriptCommand5XValueOutOfBounds
        [TestCase(DeviceType.PlexorBluetoothWIS)]
        [TestCase(DeviceType.PlexorBluetoothIrDA)]
        public void InspectionScriptCommand5XValueOutOfBoundsTest(DeviceType deviceType)
        {
           Console.WriteLine("InspectionScriptCommand5XValueOutOfBoundsTest");
            SetUpHappyFlowSC5XTH2Stack(deviceType);

            ExecuteValueOutOfBoundsTestContinuousMeasurements("TestScriptCommand1PRS", "TestScriptCommand5XValueOutOfBoundsGCL", InspectionActivityControl_ExecuteInspectionStep_ScriptCommand5XValueOutOfBoundsTest, InspectionActivityControl_MeasurementsReceived, InspectionActivityControl_ExtraMeasurementStarted);
        }

        /// <summary>
        /// Handles the ScriptCommand5XTest event of the InspectionActivityControl_ExecuteInspectionStep control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private static void InspectionActivityControl_ExecuteInspectionStep_ScriptCommand5XValueOutOfBoundsTest(object sender, EventArgs args)
        {
            Assert.IsInstanceOf<ExecuteInspectionStepEventArgs>(args, "Expected ExecuteInspectionStepEventArgs");
            var eventArgs = args as ExecuteInspectionStepEventArgs;
            Assert.IsNotNull(eventArgs);
            
            if (eventArgs.ScriptCommand is ScriptCommand2)
            {
                InspectionActivityControl.InspectionStepComplete(new InspectionStepResultEmpty(eventArgs.ScriptCommand.SequenceNumber));
            }
        }
        #endregion Test TestScriptCommand5XValueOutOfBounds

        #region Test TestScriptCommand70And5x
        [TestCase(DeviceType.PlexorBluetoothWIS)]
        [TestCase(DeviceType.PlexorBluetoothIrDA)]
        public void InspectionScriptCommand70And5XTest(DeviceType deviceType)
        {
           Console.WriteLine("InspectionScriptCommand70And5XTest");
            SetUpHappyFlowSC5XTH2Stack(deviceType);

            ExecuteHappyFlowTestContinuousMeasurements5X70("TestScriptCommand1PRS", "TestScriptCommand70And5XGCL", InspectionActivityControl_ExecuteInspectionStep_ScriptCommand70And5xTest, InspectionActivityControl_MeasurementsReceived, InspectionActivityControl_ExtraMeasurementStarted);
        }

        /// <summary>
        /// Handles the ScriptCommand5XTest event of the InspectionActivityControl_ExecuteInspectionStep control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private static void InspectionActivityControl_ExecuteInspectionStep_ScriptCommand70And5xTest(object sender, EventArgs args)
        {
            Assert.IsInstanceOf<ExecuteInspectionStepEventArgs>(args, "Expected ExecuteInspectionStepEventArgs");
            var eventArgs = args as ExecuteInspectionStepEventArgs;
            Assert.IsNotNull(eventArgs);
        }
        #endregion Test TestScriptCommand5X

        #region Test TestScriptCommandAbort
        /// <summary>
        /// Inspections the script command43 test.
        /// </summary>
        [Test]
        public void InspectionScriptCommandAbortTest()
        {
           Console.WriteLine("InspectionScriptCommandAbortTest");
            ExecuteAbortTest("TestScriptCommand1PRS", "TestScriptCommandAbortGCL", InspectionActivityControl_ExecuteInspectionStep_ScriptCommandAbortTest);
        }

        /// <summary>
        /// Handles the ScriptCommand1Test event of the InspectionActivityControl_ExecuteInspectionStep control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private static void InspectionActivityControl_ExecuteInspectionStep_ScriptCommandAbortTest(object sender, EventArgs args)
        {
            Assert.IsInstanceOf<ExecuteInspectionStepEventArgs>(args, "Expected ExecuteInspectionStepEventArgs");
            var eventArgs = args as ExecuteInspectionStepEventArgs;
            Assert.IsNotNull(eventArgs);
            var sc43 = eventArgs.ScriptCommand as ScriptCommand43;
            Assert.IsNotNull(sc43);

            if (sc43.SequenceNumber == 1)
            {
                InspectionActivityControl.Abort();
            }
        }

        /// <summary>
        /// Inspections the script command test.
        /// </summary>
        [Test]
        public void InspectionScriptCommandAbortAndMessageTest()
        {
           Console.WriteLine("InspectionScriptCommandAbortAndMessageTest");
            ExecuteAbortTest("TestScriptCommand1PRS", "TestScriptCommandAbortGCL", InspectionActivityControl_ExecuteInspectionStep_ScriptCommandAbortAndMessageTest);
        }

        /// <summary>
        /// Handles the ScriptCommand1Test event of the InspectionActivityControl_ExecuteInspectionStep control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private static void InspectionActivityControl_ExecuteInspectionStep_ScriptCommandAbortAndMessageTest(object sender, EventArgs args)
        {
            Assert.IsInstanceOf<ExecuteInspectionStepEventArgs>(args, "Expected ExecuteInspectionStepEventArgs");
            var eventArgs = args as ExecuteInspectionStepEventArgs;
            Assert.IsNotNull(eventArgs);
            var sc43 = eventArgs.ScriptCommand as ScriptCommand43;
            Assert.IsNotNull(sc43);

            if (sc43.SequenceNumber == 1)
            {
                InspectionActivityControl.InspectionStepComplete(new InspectionStepResultText(1, "text"));
                InspectionActivityControl.Abort();
            }
            else
            {
                Assert.Fail("sequencenumber should be 1, but was: " + sc43.SequenceNumber);
            }
        }
        #endregion Test TestScriptCommand1

        #region Test PartialInspection
        /// <summary>
        /// Test that in a partial inspection the correct steps are executed.
        /// </summary>
        [Test]
        public void PartialInspectionTest()
        {
           Console.WriteLine("PartialInspectionTest");
var status = InspectionStatus.Unset;
            var inspectionInfoManager = new InspectionInformationManager();
            var sectionSelection = inspectionInfoManager.LookupInspectionProcedureSections("TestScriptPartialInspection");
            var errorCode = -1;
            var eventFired = false;

            var manualResetEvent = new ManualResetEvent(false);

            EventHandler handler = (sender, args) =>
            {
                Assert.IsInstanceOf<InspectionFinishedEventArgs>(args, "Expected InspectionFinishedEventArgs");

                var eventArgs = args as InspectionFinishedEventArgs;

                Assert.IsNotNull(eventArgs);

                status = eventArgs.Result;
                errorCode = eventArgs.ErrorCode;
                sectionSelection = eventArgs.PartialInspection;

                eventFired = true;

                manualResetEvent.Set();
            };

            InspectionActivityControl.ExecuteInspectionStep += InspectionActivityControl_ExecuteInspectionStep_PartialInspection;
            InspectionActivityControl.InspectionFinished += handler;

            sectionSelection.SectionSelectionEntities[0].IsSelected = true; // Contains SequenceNumbers 1,2,3
            sectionSelection.SectionSelectionEntities[2].IsSelected = true; // Contains SequenceNumbers 7,8,9

            m_InspectionStepCount = 1;
            m_ExpectedSequenceNumbers = new List<long>() { 1, 2, 3, 7, 8, 9 };

            Assert.IsTrue(InspectionActivityControl.ExecutePartialInspection(sectionSelection, "TestScriptCommand1PRS", "TestScriptPartialInspectionGCL"));

            manualResetEvent.WaitOne(EventTimeout);

            Assert.IsTrue(eventFired, "Expect that the event is fired");
            Assert.AreEqual(InspectionStatus.Completed, status);
            Assert.AreEqual(ErrorCodes.INSPECTION_FINISHED_SUCCESSFULLY, errorCode);
            Assert.IsNull(sectionSelection);

            InspectionActivityControl.InspectionFinished -= handler;
            InspectionActivityControl.ExecuteInspectionStep -= InspectionActivityControl_ExecuteInspectionStep_PartialInspection;
        }

        /// <summary>
        /// Handles the ScriptCommand1Test event of the InspectionActivityControl_ExecuteInspectionStep control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void InspectionActivityControl_ExecuteInspectionStep_PartialInspection(object sender, EventArgs args)
        {
            Assert.IsInstanceOf<ExecuteInspectionStepEventArgs>(args, "Expected ExecuteInspectionStepEventArgs");
            var eventArgs = args as ExecuteInspectionStepEventArgs;
            Assert.IsNotNull(eventArgs);
            Assert.AreEqual(4, eventArgs.TotalInspectionSteps);
            Assert.AreEqual(m_ExpectedSequenceNumbers[m_InspectionStepCount - 1], eventArgs.ScriptCommand.SequenceNumber);
            m_InspectionStepCount++;

            InspectionActivityControl.InspectionStepComplete(new InspectionStepResultEmpty(eventArgs.ScriptCommand.SequenceNumber));
        }
        #endregion Test PartialInspection
        
        #region Test TestDoubleGCLForDifferentPRS
        /// <summary>
        /// Inspections the script command2 test.
        /// </summary>
        [Test]
        public void DoubleGCLForDifferentPRSTest()
        {
                       Console.WriteLine("DoubleGCLForDifferentPRSTest");
            ExecuteHappyFlowTest("TestPRSDouble", "TestScriptCommand2GCL", InspectionActivityControl_ExecuteInspectionStep_ScriptCommand2Test);
        }
        #endregion Test TestScriptCommand1

        #region Test Scriptcommand5657

        [TestCase(DeviceType.PlexorBluetoothWIS)]
        public void TestScriptCommand5657(DeviceType deviceType)
        {
            if (deviceType == DeviceType.PlexorBluetoothIrDA)
            {
                BluetoothHalSequentialStub.CURRENT_IO_STATUS = -1;
            }
            else
            {
                BluetoothHalSequentialStub.CURRENT_IO_STATUS = 0x0E;
            }
            Console.WriteLine("TestScriptCommand5657");

            SetupHappyFlowScriptCommand56Stack(deviceType, true);

            void InspectionstepEvent(object s, EventArgs e)
            {
            }

            void MeasurementsReceived(object s, EventArgs e)
            {
                BluetoothHalSequentialStub.CURRENT_IO_STATUS = 7;
            }

            void ExtraMeasurementsReceived(object s, EventArgs e)
            {
                Console.WriteLine("extra measurements!");
            }


            ExecuteHappyFlowTestScriptCommand56Test("TestScriptCommand1PRS", "TestScriptCommand56GCL",InspectionstepEvent,MeasurementsReceived,ExtraMeasurementsReceived);
        }

        [TestCase(DeviceType.PlexorBluetoothWIS)]
        public void TestScriptCommand5657NoIoTrigger(DeviceType deviceType)
        {
           Console.WriteLine("TestScriptCommand5657NoIoTrigger");
            SetupHappyFlowScriptCommand56Stack(deviceType, true);
            BluetoothHalSequentialStub.CURRENT_IO_STATUS = 5;
            void InspectionstepEvent(object s, EventArgs e)
            {
            }

            void MeasurementsReceived(object s, EventArgs e)
            {
            }

            void ExtraMeasurementsReceived(object s, EventArgs e)
            {
            }


            Assert.Throws<AssertionException>(() =>ExecuteHappyFlowTestScriptCommand56Test("TestScriptCommand1PRS", "TestScriptCommand56GCL", InspectionstepEvent, MeasurementsReceived, ExtraMeasurementsReceived));
        }

        [TestCase(DeviceType.PlexorBluetoothWIS)]
        public void TestScriptCommand5657NoSingleMeasurement(DeviceType deviceType)
        {
           Console.WriteLine("TestScriptCommand5657NoSingleMeasurement");

            SetupHappyFlowScriptCommand56Stack(deviceType, true);

            void InspectionstepEvent(object s, EventArgs e)
            {
            }

            void MeasurementsReceived(object s, EventArgs e)
            {
                BluetoothHalSequentialStub.CURRENT_IO_STATUS = 7;
                Task.Factory.StartNew(() =>
                {
                    Thread.Sleep(2000);
                    BluetoothHalSequentialStub.CURRENT_IO_STATUS = 5;
                });
            }

            void ExtraMeasurementsReceived(object s, EventArgs e)
            {
            }

            BluetoothHalSequentialStub.CURRENT_IO_STATUS = 5;

            ExecuteHappyFlowTestScriptCommand56Test("TestScriptCommand1PRS", "TestScriptCommand56NoSingleMeasurementGCL", InspectionstepEvent, MeasurementsReceived, ExtraMeasurementsReceived);
        }

        [TestCase(DeviceType.PlexorBluetoothWIS)]
        public void TestScriptCommand705656FprFile(DeviceType deviceType)
        {
            Console.WriteLine("TestScriptCommand705656FprFileTest");

            SetupHappyFlowScriptCommand56Stack(deviceType, true);

            void InspectionstepEvent(object s, EventArgs e)
            {
            }

            void MeasurementsReceived(object s, EventArgs e)
            {
                BluetoothHalSequentialStub.CURRENT_IO_STATUS = 7;
                Task.Factory.StartNew(() =>
                {
                    Thread.Sleep(2000);
                    BluetoothHalSequentialStub.CURRENT_IO_STATUS = 5;
                });
            }

            void ExtraMeasurementsReceived(object s, EventArgs e)
            {
            }
            BluetoothHalSequentialStub.CURRENT_IO_STATUS = 0x0E;

            ExecuteHappyFlowTestScriptCommand56Test("TestScriptCommand1PRS", "TestScriptCommand705656GCL", InspectionstepEvent, MeasurementsReceived, ExtraMeasurementsReceived);
        }

        #endregion

    }
}
