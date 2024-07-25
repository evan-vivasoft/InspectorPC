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
using Inspector.BusinessLogic.Data.Configuration.InspectionManager.Managers;
using Inspector.BusinessLogic.Interfaces;
using Inspector.BusinessLogic.Interfaces.Events;
using Inspector.Hal.Interfaces;
using Inspector.Hal.Stub;
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
        private const int EVENT_TIMEOUT = 29000;
        private const int CONTINUOUS_MEASUREMENT_EVENT_TIMEOUT = 30000;
        private IInspectionActivityControl m_InspectionActivityControl;
        private ManualResetEvent m_ManualResetEvent = new ManualResetEvent(false);
        private ManualResetEvent m_MeasurementReceivedManualResetEvent = new ManualResetEvent(false);
        private ManualResetEvent m_MeasurementsCompletedManualResetEvent = new ManualResetEvent(false);
        private IHal m_Hal;

        private InspectionStatus m_FinishResult;
        private int m_FinishErrorCode = -1;
        private SectionSelection m_FinishSectionSelection;

        private bool m_InspectionFinishedEventFired = false;
        private bool m_ExtraMeasurementsStartedEventFired = false;

        private int m_InspectionStepCount;
        private List<long> m_ExpectedSequenceNumbers;

        private const string RESULT_FILENAME = "results.xml";
        private const string TMP_RESULT_FILENAME = "tmpResults.xml";

        private bool m_MeasurementValueOutOfLimits = false;
        private bool m_MeasurementResultFired = false;
        #endregion Class Members

        #region Properties
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

        /// <summary>
        /// Gets or sets the initialization activity control.
        /// </summary>
        /// <value>
        /// The initialization activity control.
        /// </value>
        public IInspectionActivityControl InspectionActivityControl
        {
            get
            {
                if (m_InspectionActivityControl == null)
                {
                    m_InspectionActivityControl = ContextRegistry.Context.Resolve<IInspectionActivityControl>();
                }
                return m_InspectionActivityControl;
            }
            set
            {
                m_InspectionActivityControl = value;
            }
        }
        #endregion Properties

        #region Constructors
        public InspectionProcedureTest()
        {

        }
        #endregion Constructors

        [TestFixtureTearDown]
        public void TearDownFixture()
        {
        }

        [SetUp]
        public void SetupTest()
        {
            if (File.Exists(RESULT_FILENAME))
            {
                File.Delete(RESULT_FILENAME);
            }

            if (File.Exists(TMP_RESULT_FILENAME))
            {
                File.Delete(TMP_RESULT_FILENAME);
            }
        }

        #region Generic Handlers
        /// <summary>
        /// Handles the MeasurementsReceived event of the InspectionActivityControl control for the ScriptCommand5X tests.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void InspectionActivityControl_MeasurementsReceived(object sender, EventArgs e)
        {
            Assert.IsInstanceOf<MeasurementEventArgs>(e, "Expected MeasurementEventArgs");
            MeasurementEventArgs eventArgs = e as MeasurementEventArgs;
            Assert.AreEqual(10, eventArgs.Measurements.Count);
        }


        private void InspectionActivityControl_ExtraMeasurementStarted(object sender, EventArgs e)
        {
            Assert.AreEqual(EventArgs.Empty, e);
            m_ExtraMeasurementsStartedEventFired = true;
        }

        /// <summary>
        /// Handles the ScriptCommand1Test event of the InspectionActivityControl_InspectionFinished control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void InspectionActivityControl_InspectionFinished(object sender, EventArgs e)
        {
            Assert.IsInstanceOf<InspectionFinishedEventArgs>(e, "Expected InspectionFinishedEventArgs");
            InspectionFinishedEventArgs eventArgs = e as InspectionFinishedEventArgs;
            m_FinishResult = eventArgs.Result;
            m_FinishErrorCode = eventArgs.ErrorCode;
            m_FinishSectionSelection = eventArgs.PartialInspection;

            m_InspectionFinishedEventFired = true;

            m_ManualResetEvent.Set();
        }

        /// <summary>
        /// Executes the test.
        /// </summary>
        /// <param name="prsName">Name of the PRS.</param>
        /// <param name="gclName">Name of the GCL.</param>
        /// <param name="executeInspectionStepEvent">The execute inspection step event.</param>
        public void ExecuteHappyFlowTest(string prsName, string gclName, Action<object, EventArgs> executeInspectionStepEvent)
        {
            m_InspectionFinishedEventFired = false;
            InspectionActivityControl.ExecuteInspectionStep += new EventHandler(executeInspectionStepEvent);
            InspectionActivityControl.InspectionFinished += new EventHandler(InspectionActivityControl_InspectionFinished);

            m_ManualResetEvent.Reset();
            Assert.IsTrue(InspectionActivityControl.ExecuteInspection(prsName, gclName));
            m_ManualResetEvent.WaitOne(EVENT_TIMEOUT);

            Assert.IsTrue(m_InspectionFinishedEventFired);
            Assert.AreEqual(InspectionStatus.Completed, m_FinishResult);
            Assert.AreEqual(ErrorCodes.INSPECTION_FINISHED_SUCCESSFULLY, m_FinishErrorCode);

            InspectionActivityControl.InspectionFinished -= new EventHandler(InspectionActivityControl_InspectionFinished);
            InspectionActivityControl.ExecuteInspectionStep -= new EventHandler(executeInspectionStepEvent);
        }

        /// <summary>
        /// Executes the happy flow test continuous measurements with 0 measurement time defined where the user performs a manual stop measurement
        /// </summary>
        /// <param name="prsName">Name of the PRS.</param>
        /// <param name="gclName">Name of the GCL.</param>
        /// <param name="executeInspectionStepEvent">The execute inspection step event.</param>
        /// <param name="measurementsReceived">The measurements received.</param>
        public void ExecuteHappyFlowTestContinuousMeasurementsManualUserStop(string prsName, string gclName, Action<object, EventArgs> measurementsReceived)
        {
            m_InspectionFinishedEventFired = false;
            InspectionActivityControl.MeasurementsReceived += new EventHandler(measurementsReceived);
            InspectionActivityControl.MeasurementsCompleted += new EventHandler(InspectionActivityControl_MeasurementsCompleted);
            InspectionActivityControl.InspectionFinished += new EventHandler(InspectionActivityControl_InspectionFinished);

            m_ManualResetEvent.Reset();
            m_MeasurementReceivedManualResetEvent.Reset();
            Assert.IsTrue(InspectionActivityControl.ExecuteInspection(prsName, gclName));
            // wait for at least one measurementsReceived action that sets the m_ManualResetEvent
            Assert.IsTrue(m_MeasurementReceivedManualResetEvent.WaitOne(CONTINUOUS_MEASUREMENT_EVENT_TIMEOUT));

            // stop continuous measurements
            InspectionActivityControl.StopContinuousMeasurement();

            // finish inspection
            m_ManualResetEvent.WaitOne(EVENT_TIMEOUT);

            // the inspection should be fininished now
            Assert.IsFalse(m_ExtraMeasurementsStartedEventFired);
            Assert.IsTrue(m_InspectionFinishedEventFired);
            Assert.AreEqual(InspectionStatus.Completed, m_FinishResult);
            Assert.AreEqual(ErrorCodes.INSPECTION_FINISHED_SUCCESSFULLY, m_FinishErrorCode);
            Assert.IsTrue(m_FinishSectionSelection.SectionSelectionEntities.All(sectionEntity => sectionEntity.IsSelected == false));

            InspectionActivityControl.InspectionFinished -= new EventHandler(InspectionActivityControl_InspectionFinished);
            InspectionActivityControl.MeasurementsCompleted -= new EventHandler(InspectionActivityControl_MeasurementsCompleted);
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
            m_InspectionFinishedEventFired = false;
            m_ExtraMeasurementsStartedEventFired = false;
            InspectionActivityControl.MeasurementsReceived += new EventHandler(measurementsReceived);
            InspectionActivityControl.ExtraMeasurementStarted += new EventHandler(extraMeasurementsReceived);
            InspectionActivityControl.MeasurementsCompleted += new EventHandler(InspectionActivityControl_MeasurementsCompleted_NoReply);
            InspectionActivityControl.ExecuteInspectionStep += new EventHandler(executeInspectionStepEvent);
            InspectionActivityControl.InspectionFinished += new EventHandler(InspectionActivityControl_InspectionFinished);

            m_ManualResetEvent.Reset();
            m_MeasurementReceivedManualResetEvent.Reset();
            Assert.IsTrue(InspectionActivityControl.ExecuteInspection(prsName, gclName));
            m_MeasurementReceivedManualResetEvent.WaitOne(CONTINUOUS_MEASUREMENT_EVENT_TIMEOUT);

            // stop continuous measurements
            m_MeasurementsCompletedManualResetEvent.Reset();
            InspectionActivityControl.StopContinuousMeasurement();
            m_MeasurementsCompletedManualResetEvent.WaitOne(CONTINUOUS_MEASUREMENT_EVENT_TIMEOUT);

            // finish inspection
            InspectionActivityControl.MeasurementsCompleted -= new EventHandler(InspectionActivityControl_MeasurementsCompleted_NoReply);
            InspectionActivityControl.MeasurementsCompleted += new EventHandler(InspectionActivityControl_MeasurementsCompleted);

            // before restart checks
            Assert.IsTrue(m_ExtraMeasurementsStartedEventFired);
            Assert.IsFalse(m_InspectionFinishedEventFired);
            m_ExtraMeasurementsStartedEventFired = false;

            // restart
            InspectionActivityControl.StartContinuousMeasurement();
            System.Threading.Thread.Sleep(2000);
            InspectionActivityControl.StopContinuousMeasurement();

            m_MeasurementsCompletedManualResetEvent.WaitOne(CONTINUOUS_MEASUREMENT_EVENT_TIMEOUT);

            // wait for finish inspection
            m_ManualResetEvent.WaitOne(EVENT_TIMEOUT);

            // see if the inspection finished
            Assert.IsTrue(m_ExtraMeasurementsStartedEventFired);
            Assert.IsTrue(m_InspectionFinishedEventFired);
            Assert.AreEqual(InspectionStatus.Completed, m_FinishResult);
            Assert.AreEqual(ErrorCodes.INSPECTION_FINISHED_SUCCESSFULLY, m_FinishErrorCode);
            Assert.IsTrue(m_FinishSectionSelection.SectionSelectionEntities.All(sectionEntity => sectionEntity.IsSelected == false));

            InspectionActivityControl.InspectionFinished -= new EventHandler(InspectionActivityControl_InspectionFinished);
            InspectionActivityControl.ExecuteInspectionStep -= new EventHandler(executeInspectionStepEvent);
            InspectionActivityControl.MeasurementsCompleted -= new EventHandler(InspectionActivityControl_MeasurementsCompleted);
            InspectionActivityControl.ExtraMeasurementStarted -= new EventHandler(extraMeasurementsReceived);
            InspectionActivityControl.MeasurementsReceived -= new EventHandler(measurementsReceived);
        }

        /// <summary>
        /// Executes the happy flow continuous measurements test with user stop action followed by a start action.
        /// </summary>
        /// <param name="prsName">Name of the PRS.</param>
        /// <param name="gclName">Name of the GCL.</param>
        /// <param name="measurementsReceived">The measurements received.</param>
        private void ExecuteHappyFlowTestStopContinuousMeasurementsWithExtraMeasurementPeriod(string prsName, string gclName, Action<object, EventArgs> executeInspectionStepEvent, Action<object, EventArgs> measurementsReceived, Action<object, EventArgs> extraMeasurementsReceived)
        {
            m_InspectionFinishedEventFired = false;
            m_ExtraMeasurementsStartedEventFired = false;
            InspectionActivityControl.MeasurementsReceived += new EventHandler(measurementsReceived);
            InspectionActivityControl.ExtraMeasurementStarted += new EventHandler(extraMeasurementsReceived);
            InspectionActivityControl.MeasurementsCompleted += new EventHandler(InspectionActivityControl_MeasurementsCompleted);
            InspectionActivityControl.ExecuteInspectionStep += new EventHandler(executeInspectionStepEvent);
            InspectionActivityControl.InspectionFinished += new EventHandler(InspectionActivityControl_InspectionFinished);

            m_ManualResetEvent.Reset();
            m_MeasurementReceivedManualResetEvent.Reset();
            Assert.IsTrue(InspectionActivityControl.ExecuteInspection(prsName, gclName));
            m_MeasurementReceivedManualResetEvent.WaitOne(CONTINUOUS_MEASUREMENT_EVENT_TIMEOUT);

            // stop continuous measurements
            m_MeasurementsCompletedManualResetEvent.Reset();
            InspectionActivityControl.StopContinuousMeasurement();
            m_MeasurementsCompletedManualResetEvent.WaitOne(CONTINUOUS_MEASUREMENT_EVENT_TIMEOUT);

            // wait for finish inspection
            m_ManualResetEvent.WaitOne(EVENT_TIMEOUT);

            // see if the inspection finished
            Assert.IsTrue(m_ExtraMeasurementsStartedEventFired);
            Assert.IsTrue(m_InspectionFinishedEventFired);
            Assert.AreEqual(InspectionStatus.Completed, m_FinishResult);
            Assert.AreEqual(ErrorCodes.INSPECTION_FINISHED_SUCCESSFULLY, m_FinishErrorCode);
            Assert.IsTrue(m_FinishSectionSelection.SectionSelectionEntities.All(sectionEntity => sectionEntity.IsSelected == false));

            InspectionActivityControl.InspectionFinished -= new EventHandler(InspectionActivityControl_InspectionFinished);
            InspectionActivityControl.ExecuteInspectionStep -= new EventHandler(executeInspectionStepEvent);
            InspectionActivityControl.MeasurementsCompleted -= new EventHandler(InspectionActivityControl_MeasurementsCompleted);
            InspectionActivityControl.ExtraMeasurementStarted -= new EventHandler(extraMeasurementsReceived);
            InspectionActivityControl.MeasurementsReceived -= new EventHandler(measurementsReceived);
        }

        /// <summary>
        /// Executes the happy flow test with continuous measurements enabled.
        /// </summary>
        /// <param name="prsName">Name of the PRS.</param>
        /// <param name="gclName">Name of the GCL.</param>
        /// <param name="measurementsReceived">The measurements received.</param>
        public void ExecuteHappyFlowTestContinuousMeasurements5X70(string prsName, string gclName, Action<object, EventArgs> executeInspectionStepEvent, Action<object, EventArgs> measurementsReceived, Action<object, EventArgs> extraMeasurementsReceived)
        {
            m_InspectionFinishedEventFired = false;
            m_ExtraMeasurementsStartedEventFired = false;
            InspectionActivityControl.MeasurementsReceived += new EventHandler(measurementsReceived);
            InspectionActivityControl.ExtraMeasurementStarted += new EventHandler(extraMeasurementsReceived);
            InspectionActivityControl.MeasurementsCompleted += new EventHandler(InspectionActivityControl_MeasurementsCompleted);
            InspectionActivityControl.ExecuteInspectionStep += new EventHandler(executeInspectionStepEvent);
            InspectionActivityControl.InspectionFinished += new EventHandler(InspectionActivityControl_InspectionFinished);

            m_ManualResetEvent.Reset();
            Assert.IsTrue(InspectionActivityControl.ExecuteInspection(prsName, gclName));
            m_ManualResetEvent.WaitOne(CONTINUOUS_MEASUREMENT_EVENT_TIMEOUT);

            Assert.IsTrue(m_ExtraMeasurementsStartedEventFired);
            Assert.IsTrue(m_InspectionFinishedEventFired);
            Assert.AreEqual(InspectionStatus.Completed, m_FinishResult);
            Assert.AreEqual(ErrorCodes.INSPECTION_FINISHED_SUCCESSFULLY, m_FinishErrorCode);
            Assert.IsTrue(m_FinishSectionSelection.SectionSelectionEntities.All(sectionEntity => sectionEntity.IsSelected == false));

            InspectionActivityControl.InspectionFinished -= new EventHandler(InspectionActivityControl_InspectionFinished);
            InspectionActivityControl.ExecuteInspectionStep -= new EventHandler(executeInspectionStepEvent);
            InspectionActivityControl.MeasurementsCompleted -= new EventHandler(InspectionActivityControl_MeasurementsCompleted);
            InspectionActivityControl.ExtraMeasurementStarted -= new EventHandler(InspectionActivityControl_ExtraMeasurementStarted);
            InspectionActivityControl.MeasurementsReceived -= new EventHandler(measurementsReceived);
        }


        /// <summary>
        /// Executes the happy flow test with continuous measurements enabled.
        /// </summary>
        /// <param name="prsName">Name of the PRS.</param>
        /// <param name="gclName">Name of the GCL.</param>
        /// <param name="measurementsReceived">The measurements received.</param>
        public void ExecuteHappyFlowTestContinuousMeasurements(string prsName, string gclName, Action<object, EventArgs> executeInspectionStepEvent, Action<object, EventArgs> measurementsReceived, Action<object, EventArgs> extraMeasurementsReceived)
        {
            m_InspectionFinishedEventFired = false;
            m_ExtraMeasurementsStartedEventFired = false;
            m_MeasurementResultFired = false;
            InspectionActivityControl.MeasurementsReceived += new EventHandler(measurementsReceived);
            InspectionActivityControl.ExtraMeasurementStarted += new EventHandler(extraMeasurementsReceived);
            InspectionActivityControl.MeasurementsCompleted += new EventHandler(InspectionActivityControl_MeasurementsCompleted);
            InspectionActivityControl.ExecuteInspectionStep += new EventHandler(executeInspectionStepEvent);
            InspectionActivityControl.InspectionFinished += new EventHandler(InspectionActivityControl_InspectionFinished);
            InspectionActivityControl.MeasurementResult += new EventHandler(InspectionActivityControl_MeasurementResult);

            m_ManualResetEvent.Reset();
            Assert.IsTrue(InspectionActivityControl.ExecuteInspection(prsName, gclName));
            m_ManualResetEvent.WaitOne(CONTINUOUS_MEASUREMENT_EVENT_TIMEOUT);

            Assert.IsTrue(m_ExtraMeasurementsStartedEventFired);
            Assert.IsTrue(m_InspectionFinishedEventFired);
            Assert.AreEqual(InspectionStatus.Completed, m_FinishResult);
            Assert.AreEqual(ErrorCodes.INSPECTION_FINISHED_SUCCESSFULLY, m_FinishErrorCode);
            Assert.IsTrue(m_FinishSectionSelection.SectionSelectionEntities.All(sectionEntity => sectionEntity.IsSelected == false));
            Assert.IsTrue(m_MeasurementResultFired, "MeasurementResult was fired");
            Assert.AreEqual(false, m_MeasurementValueOutOfLimits);

            InspectionActivityControl.MeasurementResult -= new EventHandler(InspectionActivityControl_MeasurementResult);
            InspectionActivityControl.InspectionFinished -= new EventHandler(InspectionActivityControl_InspectionFinished);
            InspectionActivityControl.ExecuteInspectionStep -= new EventHandler(executeInspectionStepEvent);
            InspectionActivityControl.MeasurementsCompleted -= new EventHandler(InspectionActivityControl_MeasurementsCompleted);
            InspectionActivityControl.ExtraMeasurementStarted -= new EventHandler(InspectionActivityControl_ExtraMeasurementStarted);
            InspectionActivityControl.MeasurementsReceived -= new EventHandler(measurementsReceived);
        }

        void InspectionActivityControl_MeasurementResult(object sender, EventArgs e)
        {
            m_MeasurementResultFired = true;
            MeasurementResultEventArgs measurementResultEventArgs = e as MeasurementResultEventArgs;
            m_MeasurementValueOutOfLimits = measurementResultEventArgs.MeasurementValueOutOfLimits;
        }

        /// <summary>
        /// Executes the flow test with continuous measurements enabled and a value out of bounds expected.
        /// </summary>
        /// <param name="prsName">Name of the PRS.</param>
        /// <param name="gclName">Name of the GCL.</param>
        /// <param name="measurementsReceived">The measurements received.</param>
        public void ExecuteValueOutOfBoundsTestContinuousMeasurements(string prsName, string gclName, Action<object, EventArgs> executeInspectionStepEvent, Action<object, EventArgs> measurementsReceived, Action<object, EventArgs> extraMeasurementsReceived)
        {
            m_InspectionFinishedEventFired = false;
            m_ExtraMeasurementsStartedEventFired = false;
            InspectionActivityControl.MeasurementsReceived += new EventHandler(measurementsReceived);
            InspectionActivityControl.ExtraMeasurementStarted += new EventHandler(extraMeasurementsReceived);
            InspectionActivityControl.MeasurementsCompleted += new EventHandler(InspectionActivityControl_MeasurementsCompleted);
            InspectionActivityControl.ExecuteInspectionStep += new EventHandler(executeInspectionStepEvent);
            InspectionActivityControl.InspectionFinished += new EventHandler(InspectionActivityControl_InspectionFinished);
            InspectionActivityControl.MeasurementResult += new EventHandler(InspectionActivityControl_MeasurementResult_OutOfBounds);

            m_ManualResetEvent.Reset();
            Assert.IsTrue(InspectionActivityControl.ExecuteInspection(prsName, gclName));
            m_ManualResetEvent.WaitOne(CONTINUOUS_MEASUREMENT_EVENT_TIMEOUT);

            Assert.IsTrue(m_ExtraMeasurementsStartedEventFired);
            Assert.IsTrue(m_InspectionFinishedEventFired);
            Assert.AreEqual(InspectionStatus.CompletedValueOutOfLimits, m_FinishResult);
            Assert.AreEqual(ErrorCodes.INSPECTION_FINISHED_SUCCESSFULLY, m_FinishErrorCode);
            Assert.IsTrue(m_FinishSectionSelection.SectionSelectionEntities.Any(sectionEntity => sectionEntity.IsSelected == true));
            Assert.IsTrue(m_MeasurementResultFired, "MeasurementResult was fired");
            Assert.AreEqual(true, m_MeasurementValueOutOfLimits);

            InspectionActivityControl.MeasurementResult -= new EventHandler(InspectionActivityControl_MeasurementResult_OutOfBounds);
            InspectionActivityControl.InspectionFinished -= new EventHandler(InspectionActivityControl_InspectionFinished);
            InspectionActivityControl.ExecuteInspectionStep -= new EventHandler(executeInspectionStepEvent);
            InspectionActivityControl.MeasurementsCompleted -= new EventHandler(InspectionActivityControl_MeasurementsCompleted);
            InspectionActivityControl.ExtraMeasurementStarted -= new EventHandler(InspectionActivityControl_ExtraMeasurementStarted);
            InspectionActivityControl.MeasurementsReceived -= new EventHandler(measurementsReceived);
        }

        void InspectionActivityControl_MeasurementResult_OutOfBounds(object sender, EventArgs e)
        {
            m_MeasurementResultFired = true;
            MeasurementResultEventArgs measurementResultEventArgs = e as MeasurementResultEventArgs;
            m_MeasurementValueOutOfLimits = measurementResultEventArgs.MeasurementValueOutOfLimits;
        }


        /// <summary>
        /// Handles the MeasurementsCompleted event of the InspectionActivityControl control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void InspectionActivityControl_MeasurementsCompleted(object sender, EventArgs e)
        {
            Assert.IsInstanceOf<ExecuteInspectionStepEventArgs>(e, "Expected ExecuteInspectionStepEventArgs");
            ExecuteInspectionStepEventArgs eventArgs = e as ExecuteInspectionStepEventArgs;
            InspectionActivityControl.InspectionStepComplete(new InspectionStepResultEmpty(eventArgs.ScriptCommand.SequenceNumber));
            m_MeasurementsCompletedManualResetEvent.Set();
        }

        /// <summary>
        /// Handles the NoReply event of the InspectionActivityControl_MeasurementsCompleted control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void InspectionActivityControl_MeasurementsCompleted_NoReply(object sender, EventArgs e)
        {
            Assert.IsInstanceOf<ExecuteInspectionStepEventArgs>(e, "Expected ExecuteInspectionStepEventArgs");
            ExecuteInspectionStepEventArgs eventArgs = e as ExecuteInspectionStepEventArgs;
            m_MeasurementsCompletedManualResetEvent.Set();
        }

        /// <summary>
        /// Executes the test.
        /// </summary>
        /// <param name="prsName">Name of the PRS.</param>
        /// <param name="gclName">Name of the GCL.</param>
        /// <param name="executeInspectionStepEvent">The execute inspection step event.</param>
        public void ExecuteHappyFlowTest(string inspectionProcedureName, string prsName, string gclName, Action<object, EventArgs> executeInspectionStepEvent)
        {
            m_InspectionFinishedEventFired = false;
            InspectionActivityControl.ExecuteInspectionStep += new EventHandler(executeInspectionStepEvent);
            InspectionActivityControl.InspectionFinished += new EventHandler(InspectionActivityControl_InspectionFinished);

            m_ManualResetEvent.Reset();
            Assert.IsTrue(InspectionActivityControl.ExecuteInspection(inspectionProcedureName, prsName, gclName));
            m_ManualResetEvent.WaitOne(EVENT_TIMEOUT);

            Assert.IsTrue(m_InspectionFinishedEventFired);
            Assert.AreEqual(InspectionStatus.Completed, m_FinishResult);
            Assert.AreEqual(ErrorCodes.INSPECTION_FINISHED_SUCCESSFULLY, m_FinishErrorCode);

            InspectionActivityControl.InspectionFinished -= new EventHandler(InspectionActivityControl_InspectionFinished);
            InspectionActivityControl.ExecuteInspectionStep -= new EventHandler(executeInspectionStepEvent);
        }

        /// <summary>
        /// Executes the test.
        /// </summary>
        /// <param name="prsName">Name of the PRS.</param>
        /// <param name="gclName">Name of the GCL.</param>
        /// <param name="executeInspectionStepEvent">The execute inspection step event.</param>
        public void ExecuteAbortTest(string prsName, string gclName, Action<object, EventArgs> executeInspectionStepEvent)
        {
            InspectionActivityControl.ExecuteInspectionStep += new EventHandler(executeInspectionStepEvent);
            InspectionActivityControl.InspectionFinished += new EventHandler(InspectionActivityControl_InspectionFinished);

            m_ManualResetEvent.Reset();
            Assert.IsTrue(InspectionActivityControl.ExecuteInspection(prsName, gclName));
            m_ManualResetEvent.WaitOne(EVENT_TIMEOUT);

            Assert.AreEqual(InspectionStatus.StartNotCompleted, m_FinishResult);
            Assert.AreEqual(ErrorCodes.INSPECTION_ABORTED_BY_USER, m_FinishErrorCode);

            InspectionActivityControl.InspectionFinished -= new EventHandler(InspectionActivityControl_InspectionFinished);
            InspectionActivityControl.ExecuteInspectionStep -= new EventHandler(executeInspectionStepEvent);
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
        /// Sets the happy flow stack.
        /// </summary>
        private void SetUpHappyFlowTH2Stack()
        {
            Stack<string> reactionStack = new Stack<string>();
            reactionStack.Push("Random data"); // Flush Manometer Cache

            reactionStack.Push("CONNECT Test"); // Exit remote local command mode
            reactionStack.Push("ok"); // Switch Init Led off
            reactionStack.Push("ok"); // FlushBluetoothcache
            reactionStack.Push("ok"); // Enter remote local command mode
            reactionStack.Push(String.Empty); // Switch to manometer TH2

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
            reactionStack.Push("ok"); // FlushBluetoothcache
            reactionStack.Push("ok"); // Enter remote local command mode

            BluetoothHalSequentialStub stub = Hal as BluetoothHalSequentialStub;
            stub.SetUpReactionStack(reactionStack);
        }

        /// <summary>
        /// Sets up happy flow ScriptCommand5X TH2 stack.
        /// </summary>
        private void SetUpHappyFlowSC5XTH2Stack()
        {
            Stack<string> reactionStack = new Stack<string>();
            reactionStack.Push(String.Empty); // Switch to manometer TH2
            reactionStack.Push("Random data"); // Flush Manometer Cache

            reactionStack.Push("CONNECT Test"); // Exit remote local command mode
            reactionStack.Push("ok"); // Switch Init Led off
            reactionStack.Push("ok"); // FlushBluetoothcache
            reactionStack.Push("ok"); // Enter remote local command mode

            reactionStack.Push(String.Empty); // Switch to manometer TH2

            reactionStack.Push("ok"); // set IRDA always on 
            reactionStack.Push("mbar"); // Query pressure unit on TH2
            reactionStack.Push("ok"); // Set Manometer Range on TH2
            reactionStack.Push("\"17 bar\""); // Query Manometer Range on TH2
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
        /// Sets up happy flow ScriptCommand5X TH2 stack.
        /// </summary>
        private void SetUpHappyFlowSC5XTH2ManualStopStartStack()
        {
            Stack<string> reactionStack = new Stack<string>();

            reactionStack.Push(String.Empty); // Switch to manometer TH2
            reactionStack.Push("Random data"); // Flush Manometer Cache

            reactionStack.Push(String.Empty); // Switch to manometer TH2
            reactionStack.Push("Random data"); // Flush Manometer Cache

            reactionStack.Push("CONNECT Test"); // Exit remote local command mode
            reactionStack.Push("ok"); // Switch Init Led off
            reactionStack.Push("ok"); // FlushBluetoothcache
            reactionStack.Push("ok"); // Enter remote local command mode

            reactionStack.Push(String.Empty); // Switch to manometer TH2

            reactionStack.Push("ok"); // set IRDA always on 
            reactionStack.Push("mbar"); // Query pressure unit on TH2
            reactionStack.Push("ok"); // Set Manometer Range on TH2
            reactionStack.Push("\"17 bar\""); // Query Manometer Range on TH2
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

        #endregion Generic Handlers

        #region Test Unknown PRS/GCL
        /// <summary>
        /// Inspections the script command1 test.
        /// </summary>
        [Test]
        public void InspectionUnknownPrsGclPartialInspectionTest()
        {
            InspectionActivityControl.InspectionFinished += new EventHandler(InspectionActivityControl_InspectionFinished_UnknownGclPrs);

            SectionSelection sectionSelection = new SectionSelection();

            m_ManualResetEvent.Reset();
            Assert.IsTrue(InspectionActivityControl.ExecutePartialInspection(sectionSelection, "UnknownPrs", "UnknownGcl"));
            m_ManualResetEvent.WaitOne(EVENT_TIMEOUT);

            Assert.AreEqual(InspectionStatus.StartNotCompleted, m_FinishResult);
            Assert.AreEqual(ErrorCodes.INSPECTION_COULD_NOT_RETRIEVE_INSPECTION, m_FinishErrorCode);

            InspectionActivityControl.InspectionFinished -= new EventHandler(InspectionActivityControl_InspectionFinished_UnknownGclPrs);
        }

        /// <summary>
        /// Inspections the script command1 test.
        /// </summary>
        [Test]
        public void InspectionUnknownPrsGclTest()
        {
            InspectionActivityControl.InspectionFinished += new EventHandler(InspectionActivityControl_InspectionFinished_UnknownGclPrs);

            m_ManualResetEvent.Reset();
            Assert.IsTrue(InspectionActivityControl.ExecuteInspection("UnknownPrs", "UnknownGcl"));
            m_ManualResetEvent.WaitOne(EVENT_TIMEOUT);

            Assert.AreEqual(ErrorCodes.INSPECTION_COULD_NOT_RETRIEVE_INSPECTION_PROCEDURE_NAME, m_FinishErrorCode);

            InspectionActivityControl.InspectionFinished -= new EventHandler(InspectionActivityControl_InspectionFinished_UnknownGclPrs);
        }

        /// <summary>
        /// Handles the UnknownGclPrs event of the InspectionActivityControl_InspectionFinished control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void InspectionActivityControl_InspectionFinished_UnknownGclPrs(object sender, EventArgs e)
        {
            Assert.IsInstanceOf<InspectionFinishedEventArgs>(e, "Expected InspectionFinishedEventArgs");
            InspectionFinishedEventArgs eventArgs = e as InspectionFinishedEventArgs;
            m_FinishResult = eventArgs.Result;
            m_FinishErrorCode = eventArgs.ErrorCode;
            m_FinishSectionSelection = eventArgs.PartialInspection;

            m_ManualResetEvent.Set();
        }
        #endregion Test Unknown PRS/GCL

        #region Test ScriptCommand Incorrect Sequence Number
        /// <summary>
        /// Inspections the script command1 test.
        /// </summary>
        [Test]
        public void InspectionIncorrectSequenceNumberTest()
        {
            InspectionActivityControl.ExecuteInspectionStep += new EventHandler(InspectionActivityControl_ExecuteInspectionStep_IncorrectSequence);
            InspectionActivityControl.InspectionFinished += new EventHandler(InspectionActivityControl_InspectionFinished_IncorrectSequence);

            m_ManualResetEvent.Reset();
            Assert.IsTrue(InspectionActivityControl.ExecuteInspection("TestScriptCommand1PRS", "TestScriptCommand1GCL"));
            m_ManualResetEvent.WaitOne(EVENT_TIMEOUT);

            Assert.AreEqual(InspectionStatus.StartNotCompleted, m_FinishResult);
            Assert.AreEqual(ErrorCodes.INSPECTION_INCORRECT_SEQUENCE_NUMBER, m_FinishErrorCode);

            InspectionActivityControl.InspectionFinished -= new EventHandler(InspectionActivityControl_InspectionFinished_IncorrectSequence);
            InspectionActivityControl.ExecuteInspectionStep -= new EventHandler(InspectionActivityControl_ExecuteInspectionStep_IncorrectSequence);
        }

        /// <summary>
        /// Handles the ScriptCommand1Test event of the InspectionActivityControl_InspectionFinished control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void InspectionActivityControl_InspectionFinished_IncorrectSequence(object sender, EventArgs e)
        {
            Assert.IsInstanceOf<InspectionFinishedEventArgs>(e, "Expected InspectionFinishedEventArgs");
            InspectionFinishedEventArgs eventArgs = e as InspectionFinishedEventArgs;
            m_FinishResult = eventArgs.Result;
            m_FinishErrorCode = eventArgs.ErrorCode;
            m_FinishSectionSelection = eventArgs.PartialInspection;

            m_ManualResetEvent.Set();
        }

        /// <summary>
        /// Handles the ScriptCommand1Test event of the InspectionActivityControl_ExecuteInspectionStep control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void InspectionActivityControl_ExecuteInspectionStep_IncorrectSequence(object sender, EventArgs e)
        {
            Assert.IsInstanceOf<ExecuteInspectionStepEventArgs>(e, "Expected ExecuteInspectionStepEventArgs");
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
            ExecuteHappyFlowTest("TestScriptCommand1", "TestScriptCommand1PRS", "TestScriptCommand1GCL", InspectionActivityControl_ExecuteInspectionStep_ScriptCommand1WithInspectionProcedureNameTest);
        }

        /// <summary>
        /// Handles the ScriptCommand1Test event of the InspectionActivityControl_ExecuteInspectionStep control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void InspectionActivityControl_ExecuteInspectionStep_ScriptCommand1WithInspectionProcedureNameTest(object sender, EventArgs e)
        {
            Assert.IsInstanceOf<ExecuteInspectionStepEventArgs>(e, "Expected ExecuteInspectionStepEventArgs");
            ExecuteInspectionStepEventArgs eventArgs = e as ExecuteInspectionStepEventArgs;
            ScriptCommand1 sc1 = eventArgs.ScriptCommand as ScriptCommand1;
            if (sc1.SequenceNumber == 1)
            {
                Assert.AreEqual("Text1", sc1.Text);
            }
            else if (sc1.SequenceNumber == 2)
            {
                Assert.AreEqual("Text2", sc1.Text);
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
            ExecuteHappyFlowTest("TestScriptCommand2", "TestScriptCommand1PRS", "TestScriptCommand1GCL", InspectionActivityControl_ExecuteInspectionStep_DifferentInspectionProcedureNameTest);
        }

        /// <summary>
        /// Handles the ScriptCommand1Test event of the InspectionActivityControl_ExecuteInspectionStep control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void InspectionActivityControl_ExecuteInspectionStep_DifferentInspectionProcedureNameTest(object sender, EventArgs e)
        {
            Assert.IsInstanceOf<ExecuteInspectionStepEventArgs>(e, "Expected ExecuteInspectionStepEventArgs");
            ExecuteInspectionStepEventArgs eventArgs = e as ExecuteInspectionStepEventArgs;
            ScriptCommand2 sc2 = eventArgs.ScriptCommand as ScriptCommand2;
            if (sc2.SequenceNumber == 1)
            {
                Assert.AreEqual("Section1", sc2.Section);
                Assert.AreEqual("SubSection1", sc2.SubSection);
            }
            else if (sc2.SequenceNumber == 2)
            {
                Assert.AreEqual("Section1", sc2.Section);
                Assert.AreEqual("SubSection2", sc2.SubSection);
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
            ExecuteHappyFlowTest("TestScriptCommand1PRS", null, InspectionActivityControl_ExecuteInspectionStep_PRSTest);
        }

        /// <summary>
        /// Handles the ScriptCommand1Test event of the InspectionActivityControl_ExecuteInspectionStep control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void InspectionActivityControl_ExecuteInspectionStep_PRSTest(object sender, EventArgs e)
        {
            Assert.IsInstanceOf<ExecuteInspectionStepEventArgs>(e, "Expected ExecuteInspectionStepEventArgs");
            ExecuteInspectionStepEventArgs eventArgs = e as ExecuteInspectionStepEventArgs;

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
            ExecuteHappyFlowTest("TestScriptCommand1PRS", "TestScriptCommand1GCL", InspectionActivityControl_ExecuteInspectionStep_ScriptCommand1Test);
        }

        /// <summary>
        /// Handles the ScriptCommand1Test event of the InspectionActivityControl_ExecuteInspectionStep control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void InspectionActivityControl_ExecuteInspectionStep_ScriptCommand1Test(object sender, EventArgs e)
        {
            Assert.IsInstanceOf<ExecuteInspectionStepEventArgs>(e, "Expected ExecuteInspectionStepEventArgs");
            ExecuteInspectionStepEventArgs eventArgs = e as ExecuteInspectionStepEventArgs;
            ScriptCommand1 sc1 = eventArgs.ScriptCommand as ScriptCommand1;
            if (sc1.SequenceNumber == 1)
            {
                Assert.AreEqual("Text1", sc1.Text);
            }
            else if (sc1.SequenceNumber == 2)
            {
                Assert.AreEqual("Text2", sc1.Text);
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
            ExecuteHappyFlowTest("TestScriptCommand1PRS", "TestScriptCommand2GCL", InspectionActivityControl_ExecuteInspectionStep_ScriptCommand2Test);
        }

        /// <summary>
        /// Handles the ScriptCommand1Test event of the InspectionActivityControl_ExecuteInspectionStep control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void InspectionActivityControl_ExecuteInspectionStep_ScriptCommand2Test(object sender, EventArgs e)
        {
            Assert.IsInstanceOf<ExecuteInspectionStepEventArgs>(e, "Expected ExecuteInspectionStepEventArgs");
            ExecuteInspectionStepEventArgs eventArgs = e as ExecuteInspectionStepEventArgs;
            ScriptCommand2 sc2 = eventArgs.ScriptCommand as ScriptCommand2;
            if (sc2.SequenceNumber == 1)
            {
                Assert.AreEqual("Section1", sc2.Section);
                Assert.AreEqual("SubSection1", sc2.SubSection);
            }
            else if (sc2.SequenceNumber == 2)
            {
                Assert.AreEqual("Section1", sc2.Section);
                Assert.AreEqual("SubSection2", sc2.SubSection);
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
            ExecuteHappyFlowTest("TestScriptCommand1PRS", "TestScriptCommand3GCL", InspectionActivityControl_ExecuteInspectionStep_ScriptCommand3Test);
        }

        /// <summary>
        /// Handles the ScriptCommand1Test event of the InspectionActivityControl_ExecuteInspectionStep control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void InspectionActivityControl_ExecuteInspectionStep_ScriptCommand3Test(object sender, EventArgs e)
        {
            Assert.IsInstanceOf<ExecuteInspectionStepEventArgs>(e, "Expected ExecuteInspectionStepEventArgs");
            ExecuteInspectionStepEventArgs eventArgs = e as ExecuteInspectionStepEventArgs;
            ScriptCommand3 sc3 = eventArgs.ScriptCommand as ScriptCommand3;
            if (sc3.SequenceNumber == 1)
            {
                Assert.AreEqual("Text1", sc3.Text);
                Assert.AreEqual(60, sc3.Duration);
            }
            else if (sc3.SequenceNumber == 2)
            {
                Assert.AreEqual("Text2", sc3.Text);
                Assert.AreEqual(120, sc3.Duration);
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
            ExecuteHappyFlowTest("TestScriptCommand1PRS", "TestScriptCommand4GCL", InspectionActivityControl_ExecuteInspectionStep_ScriptCommand4Test);
        }

        /// <summary>
        /// Handles the ScriptCommand1Test event of the InspectionActivityControl_ExecuteInspectionStep control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void InspectionActivityControl_ExecuteInspectionStep_ScriptCommand4Test(object sender, EventArgs e)
        {
            Assert.IsInstanceOf<ExecuteInspectionStepEventArgs>(e, "Expected ExecuteInspectionStepEventArgs");
            ExecuteInspectionStepEventArgs eventArgs = e as ExecuteInspectionStepEventArgs;
            ScriptCommand4 sc4 = eventArgs.ScriptCommand as ScriptCommand4;
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
            ExecuteHappyFlowTest("TestScriptCommand1PRS", "TestScriptCommand41GCL", InspectionActivityControl_ExecuteInspectionStep_ScriptCommand41Test);
        }

        /// <summary>
        /// Handles the ScriptCommand1Test event of the InspectionActivityControl_ExecuteInspectionStep control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void InspectionActivityControl_ExecuteInspectionStep_ScriptCommand41Test(object sender, EventArgs e)
        {
            Assert.IsInstanceOf<ExecuteInspectionStepEventArgs>(e, "Expected ExecuteInspectionStepEventArgs");
            ExecuteInspectionStepEventArgs eventArgs = e as ExecuteInspectionStepEventArgs;
            ScriptCommand41 sc41 = eventArgs.ScriptCommand as ScriptCommand41;
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
            ExecuteHappyFlowTest("TestScriptCommand1PRS", "TestScriptCommand42GCL", InspectionActivityControl_ExecuteInspectionStep_ScriptCommand42Test);
        }

        /// <summary>
        /// Handles the ScriptCommand1Test event of the InspectionActivityControl_ExecuteInspectionStep control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void InspectionActivityControl_ExecuteInspectionStep_ScriptCommand42Test(object sender, EventArgs e)
        {
            Assert.IsInstanceOf<ExecuteInspectionStepEventArgs>(e, "Expected ExecuteInspectionStepEventArgs");
            ExecuteInspectionStepEventArgs eventArgs = e as ExecuteInspectionStepEventArgs;
            ScriptCommandBase sc42 = eventArgs.ScriptCommand as ScriptCommandBase;
            if (sc42.SequenceNumber == 1)
            {
                // no checks needed
            }
            else if (sc42.SequenceNumber == 2)
            {
                InspectionActivityControl.StoreRemark(new InspectionStepResultText(1, "Remark"));
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
            ExecuteHappyFlowTest("TestScriptCommand1PRS", "TestScriptCommand43GCL", InspectionActivityControl_ExecuteInspectionStep_ScriptCommand43Test);
        }

        /// <summary>
        /// Handles the ScriptCommand1Test event of the InspectionActivityControl_ExecuteInspectionStep control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void InspectionActivityControl_ExecuteInspectionStep_ScriptCommand43Test(object sender, EventArgs e)
        {
            Assert.IsInstanceOf<ExecuteInspectionStepEventArgs>(e, "Expected ExecuteInspectionStepEventArgs");
            ExecuteInspectionStepEventArgs eventArgs = e as ExecuteInspectionStepEventArgs;
            ScriptCommand43 sc43 = eventArgs.ScriptCommand as ScriptCommand43;
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
        [Test]
        public void InspectionScriptCommand5XTest()
        {
            SetUpHappyFlowSC5XTH2Stack();
            ExecuteHappyFlowTestContinuousMeasurements("TestScriptCommand1PRS", "TestScriptCommand5XGCL", InspectionActivityControl_ExecuteInspectionStep_ScriptCommand5XTest, InspectionActivityControl_MeasurementsReceived, InspectionActivityControl_ExtraMeasurementStarted);
        }

        /// <summary>
        /// Handles the ScriptCommand5XTest event of the InspectionActivityControl_ExecuteInspectionStep control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void InspectionActivityControl_ExecuteInspectionStep_ScriptCommand5XTest(object sender, EventArgs e)
        {
            Assert.IsInstanceOf<ExecuteInspectionStepEventArgs>(e, "Expected ExecuteInspectionStepEventArgs");
            ExecuteInspectionStepEventArgs eventArgs = e as ExecuteInspectionStepEventArgs;

            System.Diagnostics.Debug.WriteLine("InspectionActivityControl_ExecuteInspectionStep_ScriptCommand5XTest: Current Inspection Step '{0}', Sequencenumber: '{1}'", eventArgs.CurrentInspectionStep, eventArgs.ScriptCommand.SequenceNumber);

            if (eventArgs.ScriptCommand is ScriptCommand2)
            {
                InspectionActivityControl.InspectionStepComplete(new InspectionStepResultEmpty(eventArgs.ScriptCommand.SequenceNumber));
            }
        }
        #endregion Test TestScriptCommand5X

        #region Test TestScriptCommand5X manual restart with extra measurement time
        [Test]
        public void InspectionScriptCommand5XUserRestartWithExtraMeasurementTest()
        {
            SetUpHappyFlowSC5XTH2ManualStopStartStack();
            ExecuteHappyFlowTestRestartContinuousMeasurementsWithExtraMeasurementPeriod("TestScriptCommand1PRS", "TestScriptCommand5XGCLZeroMeasurementAndExtraMeasurementTime", InspectionActivityControl_ExecuteInspectionStep_ScriptCommand5XTest, InspectionActivityControl_One_MeasurementsReceived_Executed, InspectionActivityControl_ExtraMeasurementStarted);
        }
        #endregion Test TestScriptCommand5X manual restart with extra measurement time

        #region Test TestScriptCommand5X manual user stop with extra measurement time
        [Test]
        public void InspectionScriptCommand5XUserStopWithExtraMeasurementTest()
        {
            SetUpHappyFlowSC5XTH2ManualStopStartStack();
            ExecuteHappyFlowTestStopContinuousMeasurementsWithExtraMeasurementPeriod("TestScriptCommand1PRS", "TestScriptCommand5XGCLZeroMeasurementAndExtraMeasurementTime", InspectionActivityControl_ExecuteInspectionStep_ScriptCommand5XTest, InspectionActivityControl_One_MeasurementsReceived_Executed, InspectionActivityControl_ExtraMeasurementStarted);
        }
        #endregion Test TestScriptCommand5X manual user stop with extra measurement time

        #region Test TestScriptCommand5X manual user stop (0 measurement time defined)
        [Test]
        public void InspectionScriptCommand5XManualUserStopZeroMeasurementTimeDefinedTest()
        {
            SetUpHappyFlowSC5XTH2Stack();
            ExecuteHappyFlowTestContinuousMeasurementsManualUserStop("TestScriptCommand1PRS", "TestScriptCommand5XGCLZeroMeasurement", InspectionActivityControl_One_MeasurementsReceived_Executed);
        }

        /// <summary>
        /// Handles the Executed event of the InspectionActivityControl_One_MeasurementsReceived control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void InspectionActivityControl_One_MeasurementsReceived_Executed(object sender, EventArgs e)
        {
            Assert.IsInstanceOf<MeasurementEventArgs>(e, "Expected MeasurementEventArgs");
            MeasurementEventArgs eventArgs = e as MeasurementEventArgs;
            Assert.AreEqual(10, eventArgs.Measurements.Count);
            m_MeasurementReceivedManualResetEvent.Set();
        }
        #endregion Test TestScriptCommand5X manual user stop (0 measurement time)

        #region Test TestScriptCommand5XValueOutOfBounds
        [Test]
        public void InspectionScriptCommand5XValueOutOfBoundsTest()
        {
            SetUpHappyFlowSC5XTH2Stack();
            ExecuteValueOutOfBoundsTestContinuousMeasurements("TestScriptCommand1PRS", "TestScriptCommand5XValueOutOfBoundsGCL", InspectionActivityControl_ExecuteInspectionStep_ScriptCommand5XValueOutOfBoundsTest, InspectionActivityControl_MeasurementsReceived, InspectionActivityControl_ExtraMeasurementStarted);
        }

        /// <summary>
        /// Handles the ScriptCommand5XTest event of the InspectionActivityControl_ExecuteInspectionStep control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void InspectionActivityControl_ExecuteInspectionStep_ScriptCommand5XValueOutOfBoundsTest(object sender, EventArgs e)
        {
            Assert.IsInstanceOf<ExecuteInspectionStepEventArgs>(e, "Expected ExecuteInspectionStepEventArgs");
            ExecuteInspectionStepEventArgs eventArgs = e as ExecuteInspectionStepEventArgs;

            if (eventArgs.ScriptCommand is ScriptCommand2)
            {
                InspectionActivityControl.InspectionStepComplete(new InspectionStepResultEmpty(eventArgs.ScriptCommand.SequenceNumber));
            }
        }
        #endregion Test TestScriptCommand5XValueOutOfBounds

        #region Test TestScriptCommand70And5x
        [Test]
        public void InspectionScriptCommand70And5xTest()
        {
            SetUpHappyFlowSC5XTH2Stack();
            ExecuteHappyFlowTestContinuousMeasurements5X70("TestScriptCommand1PRS", "TestScriptCommand70And5XGCL", InspectionActivityControl_ExecuteInspectionStep_ScriptCommand70And5xTest, InspectionActivityControl_MeasurementsReceived, InspectionActivityControl_ExtraMeasurementStarted);
        }

        /// <summary>
        /// Handles the ScriptCommand5XTest event of the InspectionActivityControl_ExecuteInspectionStep control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void InspectionActivityControl_ExecuteInspectionStep_ScriptCommand70And5xTest(object sender, EventArgs e)
        {
            Assert.IsInstanceOf<ExecuteInspectionStepEventArgs>(e, "Expected ExecuteInspectionStepEventArgs");
            ExecuteInspectionStepEventArgs eventArgs = e as ExecuteInspectionStepEventArgs;
        }
        #endregion Test TestScriptCommand5X

        #region Test TestScriptCommandAbort
        /// <summary>
        /// Inspections the script command43 test.
        /// </summary>
        [Test]
        public void InspectionScriptCommandAbortTest()
        {
            ExecuteAbortTest("TestScriptCommand1PRS", "TestScriptCommandAbortGCL", InspectionActivityControl_ExecuteInspectionStep_ScriptCommandAbortTest);
        }

        /// <summary>
        /// Handles the ScriptCommand1Test event of the InspectionActivityControl_ExecuteInspectionStep control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void InspectionActivityControl_ExecuteInspectionStep_ScriptCommandAbortTest(object sender, EventArgs e)
        {
            Assert.IsInstanceOf<ExecuteInspectionStepEventArgs>(e, "Expected ExecuteInspectionStepEventArgs");
            ExecuteInspectionStepEventArgs eventArgs = e as ExecuteInspectionStepEventArgs;
            ScriptCommand43 sc43 = eventArgs.ScriptCommand as ScriptCommand43;
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
            ExecuteAbortTest("TestScriptCommand1PRS", "TestScriptCommandAbortGCL", InspectionActivityControl_ExecuteInspectionStep_ScriptCommandAbortAndMessageTest);
        }

        /// <summary>
        /// Handles the ScriptCommand1Test event of the InspectionActivityControl_ExecuteInspectionStep control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void InspectionActivityControl_ExecuteInspectionStep_ScriptCommandAbortAndMessageTest(object sender, EventArgs e)
        {
            Assert.IsInstanceOf<ExecuteInspectionStepEventArgs>(e, "Expected ExecuteInspectionStepEventArgs");
            ExecuteInspectionStepEventArgs eventArgs = e as ExecuteInspectionStepEventArgs;
            ScriptCommand43 sc43 = eventArgs.ScriptCommand as ScriptCommand43;
            if (sc43.SequenceNumber == 1)
            {
                InspectionActivityControl.InspectionStepComplete(new InspectionStepResultText(1, "text"));
                InspectionActivityControl.Abort();
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
            m_InspectionFinishedEventFired = false;
            InspectionActivityControl.ExecuteInspectionStep += new EventHandler(InspectionActivityControl_ExecuteInspectionStep_PartialInspection);
            InspectionActivityControl.InspectionFinished += new EventHandler(InspectionActivityControl_InspectionFinished);

            InspectionInformationManager inspectionInfoManager = new InspectionInformationManager();
            SectionSelection sectionSelection = inspectionInfoManager.LookupInspectionProcedureSections("TestScriptPartialInspection");

            sectionSelection.SectionSelectionEntities[0].IsSelected = true; // Contains SequenceNumbers 1,2,3
            sectionSelection.SectionSelectionEntities[2].IsSelected = true; // Contains SequenceNumbers 7,8,9

            m_InspectionStepCount = 1;
            m_ExpectedSequenceNumbers = new List<long>() { 1, 2, 3, 7, 8, 9 };

            m_ManualResetEvent.Reset();
            Assert.IsTrue(InspectionActivityControl.ExecutePartialInspection(sectionSelection, "TestScriptCommand1PRS", "TestScriptPartialInspectionGCL"));
            m_ManualResetEvent.WaitOne(EVENT_TIMEOUT);

            Assert.IsTrue(m_InspectionFinishedEventFired);
            Assert.AreEqual(InspectionStatus.Completed, m_FinishResult);
            Assert.AreEqual(ErrorCodes.INSPECTION_FINISHED_SUCCESSFULLY, m_FinishErrorCode);

            InspectionActivityControl.InspectionFinished -= new EventHandler(InspectionActivityControl_InspectionFinished);
            InspectionActivityControl.ExecuteInspectionStep -= new EventHandler(InspectionActivityControl_ExecuteInspectionStep_PartialInspection);
        }

        /// <summary>
        /// Handles the ScriptCommand1Test event of the InspectionActivityControl_ExecuteInspectionStep control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void InspectionActivityControl_ExecuteInspectionStep_PartialInspection(object sender, EventArgs e)
        {
            Assert.IsInstanceOf<ExecuteInspectionStepEventArgs>(e, "Expected ExecuteInspectionStepEventArgs");
            ExecuteInspectionStepEventArgs eventArgs = e as ExecuteInspectionStepEventArgs;

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
            ExecuteHappyFlowTest("TestPRSDouble", "TestScriptCommand2GCL", InspectionActivityControl_ExecuteInspectionStep_ScriptCommand2Test);
        }
        #endregion Test TestScriptCommand1

    }
}
