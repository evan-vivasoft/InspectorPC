/*
//===============================================================================
// Copyright Kamstrup
// All rights reserved.
//===============================================================================
*/

using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Inspector.BusinessLogic.Data.Configuration.Interfaces.Exceptions;
using Inspector.BusinessLogic.Data.Configuration.Interfaces.Managers;
using Inspector.BusinessLogic.Data.Reporting.Interfaces;
using Inspector.BusinessLogic.Exceptions;
using Inspector.BusinessLogic.Interfaces;
using Inspector.BusinessLogic.Interfaces.Events;
using Inspector.Connection.Manager.Interfaces;
using Inspector.Infra;
using Inspector.Infra.Ioc;
using Inspector.Infra.Utils;
using Inspector.Model;
using Inspector.Model.BluetoothDongle;
using Inspector.Model.InspectionMeasurement;
using Inspector.Model.InspectionProcedure;
using Inspector.Model.InspectionStepResult;
using Inspector.Model.MeasurementResult;
using Inspector.Model.Plexor;
using KAM.INSPECTOR.Infra;
using log4net;
using log4net.Appender;
using log4net.Repository.Hierarchy;

namespace Inspector.BusinessLogic
{
    public delegate void MeasurementsReceived(object sender, IList<double> measurements);

    #region Enumerations
    /// <summary>
    /// Inspection Procedure Result
    /// </summary>
    internal enum InspectionProcedureResult
    {
        /// <summary>
        /// No Inspection Procedure Set
        /// </summary>
        UNSET,

        /// <summary>
        /// Inspection procedure finished successfully
        /// </summary>
        SUCCESS,

        /// <summary>
        /// Inspection procedure finished with an error
        /// </summary>
        ERROR,

        /// <summary>
        /// Inspection aborted
        /// </summary>
        ABORT,
    }
    #endregion Enumerations

    #region Structs
    /// <summary>
    /// The relevant manometer information used during an inspection
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1815:OverrideEqualsAndOperatorEqualsOnValueTypes")]
    internal struct ManometerInformation
    {
        /// <summary>
        /// The identification string of the manometer
        /// </summary>
        public string Identification { get; set; }

        /// <summary>
        /// The unit of the manometer
        /// </summary>
        public string Unit { get; set; }

        /// <summary>
        /// The pressure range of the manometer
        /// </summary>
        public string Range { get; set; }

        /// <summary>
        /// The resolution of the manometer
        /// </summary>
        public int Resolution { get; set; }
    }
    #endregion Structs

    /// <summary>
    /// InspectionActivityControl
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling")]
    public class InspectionActivityControl : IInspectionActivityControl
    {
        #region Constants

        private const string SETTING_CATEGORY_APPLICATION = "APPLICATION";
        private const string SETTING_MEASUREMENT_FILE_WRITE_ENABLED = "MeasurementFileWriteEnabled";
        private const string SETTING_MEASUREMENT_COMMAND_DELAY = "MeasurementCommandDelay";
        private const int DEFAULT_CONTINUOUS_MEASUREMENT_DELAY = 750;
        private const bool DEFAULT_MEASUREMENT_FILE_WRITE_ENABLED = true;
        private const string SETTING_RETURN_NO_VALUE = "<NO VALUE>";
        private const string BLUETOOTH_ADDRESS_IDENTIFIER = "destinationAddress";
        private const string SETTING_BLUETOOTH_DONGLE_ADDRESS = "BluetoothDongleAddress";
        private const string SETTING_RECONNECT_TIMEOUT = "ReConnectTimeout";
        private const string SETTING_CATEGORY = "PLEXOR";
        private const string SETTING_SAVE_COMMUNICATION_LOG_ON_SUCCESS = "SaveCommunicationLogOnSuccess";

        private const string SCRIPTCOMMAND_51_NAME = "Scriptcommand 51";
        private const string SCRIPTCOMMAND_52_NAME = "Scriptcommand 52";
        private const string SCRIPTCOMMAND_53_NAME = "Scriptcommand 53";
        private const string SCRIPTCOMMAND_54_NAME = "Scriptcommand 54";
        private const string SCRIPTCOMMAND_55_NAME = "Scriptcommand 55";
        private const string SCRIPTCOMMAND_56_NAME = "Scriptcommand 56";
        private const string SCRIPTCOMMAND_57_NAME = "Scriptcommand 57";
        private const string SCRIPTCOMMAND_70_NAME = "Scriptcommand 70";
        private const string UNKNOWN_SCRIPTCOMMAND_NAME = "Unknown";

        private const string SETTING_UNITS_SECTION = "UNITS";
        private const string SETTING_LOW_PRESSURE_UNIT = "UnitLowPressure";
        private const string SETTING_HIGH_PRESSURE_UNIT = "UnitHighPressure";
        private const string SETTING_LOW_HIGH_PRESSURE_FACTOR = "FactorLowHighPressure";
        private const string SETTING_CHANGERATE_UNIT = "UnitChangeRate";
        private const string SETTING_MBARMIN_TO_LEAKAGE_UNIT_FACTOR = "FactorMbarMinToUnitChangeRate";
        private const string SETTING_MEASURED_LEAKAGE_TO_MBARMIN = "FactorMeasuredChangeRateToMbarMin";
        private const string SETTING_QVS_LEAKAGE_UNIT = "UnitQVSLeakage";
        private const string SETTING_QVS_FACTOR = "FactorQVS";
        #endregion Constants

        #region Class Members
        private bool m_Disposed;
        //the below flag indicates whether there is a valsensor present that will automatically stop the scriptcommands 56 and 57
        private bool? m_5657AutoFlag = null;
        private bool m_userManualRequestedStop;
        private int m_LastIoStatus = -1;
        private int m_IoBitStatusStart = 0;
        private ManualResetEvent m_UiResponseReceived = new ManualResetEvent(true);
        private UiResponse m_UiResponse;
        private InspectionProcedureInformation m_InspectionProcedureInfo = new InspectionProcedureInformation();
        private ScriptCommandBase m_CurrentInspectionProcedureStep;
        private ScriptCommand5X m_LastScriptCommand5X;
        private DateTime m_InspectionStartTimestamp;

        private Dictionary<string, string> m_ConnectionProperties = new Dictionary<string, string>();
        private InitializationExecutorControl m_Initialization;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        private InitializationResult m_InitializationResult = InitializationResult.UNSET;
        private bool m_DisconnectRequired;
        private List<DigitalManometer> m_RequiredManometers;

        private ManometerInformation m_ManometerInformationTH1;
        private ManometerInformation m_ManometerInformationTH2;

        private SectionSelection m_SectionSelection;
        private string m_PrsName;
        private string m_GclName;
        private string m_InspectionProcedureName;

        private bool m_Isinitializing;

        private IReportControl m_ReportControl;
        private IStationInformationManager m_StationInformationManager;
        private IInspectionInformationManager m_InspectionInformationManager;
        private ICommunicationControl m_CommunicationControl;
        private IPlexorInformationManager m_PlexorInformationManager;

        private InspectionProcedureResult m_InspectionResult = InspectionProcedureResult.UNSET;
        private int m_InspectionResultErrorCode = -1;
        private InspectionStatus m_InspectionResultStatus = InspectionStatus.NoInspection;

        private SectionSelection m_ValueOutOfBoundsSections;

        private bool m_InspectionReportCreated;
        private bool m_IsScriptCommand70Enabled;
        private double m_MeasurementValue = double.NaN;
        private string m_MeasurementUnit = string.Empty;
        private UnitOfMeasurement m_UnitOfMeasurement;

        private readonly object m_LockExectionInspectionBusy = new object();
        private volatile bool m_IsHandleExecuteInspectionBusy;

        private volatile bool m_IsUserAbort;

        private int m_TotalInspectionSteps = -1;
        private int m_CurrentInspectionStep = -1;

        private ContinuousMeasurementWorker m_ContinuousMeasurementWorker;
        private Thread m_ContinuousMeasurementWorkerThread;
        private bool m_ContinuousMeasurementWorkerThreadInitialized;
        private Timer m_ContinuousMeasurementPeriodTimer;
        private long m_ContinuousExtraMeasurementPeriod;
        private long m_ContinuousMeasurementPeriod;
        private int m_MeasurementFrequency;

        private double m_VolumeVa = double.NaN;
        private double m_VolumeVak = double.NaN;

        private IMeasurementReportControl m_MeasurementReportControl;
        private readonly bool m_MeasurementFileCreationEnabled;
        private Thread m_MeasurementFileThread;
        private bool m_MeasurementReportWorkerThreadInitialized;

        private volatile bool m_ExecutingExtraMeasurements;
        private bool m_SafetyValueEventSent;
        private ScriptCommand5XMeasurement m_LastMeasurement;
        private DateTime m_MeasurementStartDate;
        private DateTime? m_MeasurementEndDate;
        private bool m_IsMeasuring;

        private readonly int m_MeasurementCommandDelay;

        private bool m_IsRetryCallAllowed;

        private readonly ManualResetEvent m_StoppingMeasurementResetEvent;
        private bool m_MeasurementStoppedSucceeded;

        private DeviceCommand m_CurrentCommand = DeviceCommand.None;

        private static readonly ILog Log = LogManager.GetLogger(typeof(InspectionActivityControl));

        private static readonly ILog CommunicationLogger = LogManager.GetLogger("CommunicationLogger");

        #endregion Class Members

        #region Properties
        /// <summary>
        /// Gets the plexor information manager.
        /// </summary>
        public IPlexorInformationManager PlexorInformationManager
        {
            get
            {
                return m_PlexorInformationManager ?? (m_PlexorInformationManager = ContextRegistry.Context.Resolve<IPlexorInformationManager>());
            }
        }


        /// <summary>
        /// Gets the measurement report control.
        /// </summary>
        public IMeasurementReportControl MeasurementReportControl
        {
            get
            {
                return m_MeasurementReportControl ?? (m_MeasurementReportControl = ContextRegistry.Context.Resolve<IMeasurementReportControl>());
            }
        }

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
                return m_CommunicationControl ?? (m_CommunicationControl = ContextRegistry.Context.Resolve<ICommunicationControl>());
            }
        }

        /// <summary>
        /// Gets or sets the inspection information manager.
        /// </summary>
        /// <value>
        /// The inspection information manager.
        /// </value>
        public IInspectionInformationManager InspectionInformationManager
        {
            get
            {
                return m_InspectionInformationManager ?? (m_InspectionInformationManager = ContextRegistry.Context.Resolve<IInspectionInformationManager>());
            }
        }

        /// <summary>
        /// Gets or sets the station information manager.
        /// </summary>
        /// <value>
        /// The station information manager.
        /// </value>
        public IStationInformationManager StationInformationManager
        {
            get
            {
                return m_StationInformationManager ?? (m_StationInformationManager = ContextRegistry.Context.Resolve<IStationInformationManager>());
            }
        }


        /// <summary>
        /// Gets or sets the report control.
        /// </summary>
        /// <value>
        /// The report control.
        /// </value>
        public IReportControl ReportControl
        {
            get
            {
                return m_ReportControl ?? (m_ReportControl = ContextRegistry.Context.Resolve<IReportControl>());
            }
        }
        #endregion Properties

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="InspectionActivityControl"/> class.
        /// </summary>
        public InspectionActivityControl()
        {
            m_MeasurementFileCreationEnabled = GetMeasurementFileCreationEnabled();
            m_MeasurementCommandDelay = GetMeasurementCommandDelay();
            m_StoppingMeasurementResetEvent = new ManualResetEvent(false);
        }
        #endregion Constructors

        #region IInspectionActivityControl

        #region Dispose
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
                    m_CommunicationControl?.Dispose();

                    if (m_ContinuousMeasurementPeriodTimer != null) m_ContinuousMeasurementPeriodTimer.Dispose();

                    m_MeasurementFileThread?.Abort();

                    m_MeasurementReportControl?.Dispose();

                    if (m_ContinuousMeasurementWorker != null)
                    {
                        m_ContinuousMeasurementWorker.MeasurementValuesReceived -=
                            continuousMeasurementWorker_MeasurementValuesReceived;
                        m_ContinuousMeasurementWorker.Dispose();
                    }


                    if (m_UiResponseReceived != null) m_UiResponseReceived.Dispose();

                    m_StoppingMeasurementResetEvent.Dispose();
                }
            }

            m_Disposed = true;
        }
        #endregion Dispose

        /// <summary>
        /// Executes the inspection.
        /// </summary>
        /// <param name="inspectionProcedureName"></param>
        /// <param name="prsName">Name of the PRS.</param>
        /// <param name="gclName">Name of the GCL.</param>
        /// <returns>True if the inspection is succesfully started, false if another inspection is already running which is not yet completed</returns>
        public bool ExecuteInspection(string inspectionProcedureName, string prsName, string gclName)
        {
            bool isInspectionStarted;

            lock (m_LockExectionInspectionBusy)
            {
                isInspectionStarted = !m_IsHandleExecuteInspectionBusy;

                if (m_IsHandleExecuteInspectionBusy) return isInspectionStarted;

                // not busy so we can start the inspection
                m_IsHandleExecuteInspectionBusy = true;
                m_PrsName = prsName;
                m_GclName = gclName;
                m_InspectionProcedureName = inspectionProcedureName;

                ThreadPool.QueueUserWorkItem(HandleExecuteInspection);
            }

            return isInspectionStarted;
        }

        /// <summary>
        /// Executes the inspection.
        /// </summary>
        /// <param name="prsName">Name of the PRS.</param>
        /// <param name="gclName">Name of the GCL.</param>
        /// <returns>True if the inspection is succesfully started, false if another inspection is already running which is not yet completed</returns>
        public bool ExecuteInspection(string prsName, string gclName)
        {
            bool isInspectionStarted;

            lock (m_LockExectionInspectionBusy)
            {
                isInspectionStarted = !m_IsHandleExecuteInspectionBusy;

                if (m_IsHandleExecuteInspectionBusy) return isInspectionStarted;

                // not busy so we can start the inspection
                m_IsHandleExecuteInspectionBusy = true;
                m_PrsName = prsName;
                m_GclName = gclName;

                ThreadPool.QueueUserWorkItem(HandleGetInspectionProcedureName);
            }

            return isInspectionStarted;
        }

        /// <summary>
        /// Executes the partial inspection.
        /// </summary>
        /// <param name="sectionSelection">The section selection.</param>
        /// <param name="prsName">Name of the PRS.</param>
        /// <param name="gclName">Name of the GCL.</param>
        /// <returns>True if the inspection is succesfully started, false if another inspection is already running which is not yet completed</returns>
        public bool ExecutePartialInspection(SectionSelection sectionSelection, string prsName, string gclName)
        {
            if (sectionSelection == null)
            {
                throw new InspectionException("The section selection is not set.", new ArgumentNullException("sectionSelection"));
            }

            bool isInspectionStarted;

            lock (m_LockExectionInspectionBusy)
            {
                isInspectionStarted = !m_IsHandleExecuteInspectionBusy;

                if (m_IsHandleExecuteInspectionBusy) return isInspectionStarted;

                // not busy so we can start the inspection
                m_IsHandleExecuteInspectionBusy = true;
                m_SectionSelection = sectionSelection;
                m_InspectionProcedureName = sectionSelection.InspectionProcedureName;
                m_PrsName = prsName;
                m_GclName = gclName;

                ThreadPool.QueueUserWorkItem(HandleExecutePartialInspection);
            }

            return isInspectionStarted;
        }

        /// <summary>
        /// Called when an inspection step is complete
        /// </summary>
        /// <param name="inspectionStepResult">The inspection step result.</param>
        public void InspectionStepComplete(InspectionStepResultBase inspectionStepResult)
        {
            ThreadPool.QueueUserWorkItem(HandleInspectionStepComplete, inspectionStepResult);
        }

        /// <summary>
        /// Stores the remark.
        /// </summary>
        /// <param name="remarkStepResult">The remark step result.</param>
        public void StoreRemark(InspectionStepResultText remarkStepResult)
        {
            ThreadPool.QueueUserWorkItem(HandleStoreRemark, remarkStepResult);
        }

        /// <summary>
        /// Retry the last failed action as notified by the InspectionError event.
        /// </summary>
        /// <exception cref="InspectionException">Thrown when a retry is attempted when the inspection is not in an error situation.</exception>
        public void Retry()
        {
            lock (m_LockExectionInspectionBusy)
            {
                if (m_IsRetryCallAllowed)
                {
                    m_IsRetryCallAllowed = false;
                    if (!m_Isinitializing && m_CurrentInspectionProcedureStep != null)
                    {
                        // The current command will be retried.
                        m_CurrentInspectionStep--;
                        m_InspectionProcedureInfo.ScriptCommandSequence.Push(m_CurrentInspectionProcedureStep.CopyScriptCommand());
                    }
                    ReInitialize();
                }
                else
                {
                    throw new InspectionException("Nothing to retry.");
                }
            }
        }

        /// <summary>
        /// Aborts the current action, be it initialization or inspection
        /// </summary>
        public void Abort()
        {
            if (m_Isinitializing)
            {
                System.Diagnostics.Debug.WriteLine("Aborting initialization");

                m_IsUserAbort = true;

                m_Initialization.AbortInitialization();
            }
            else
            {
                lock (m_LockExectionInspectionBusy)
                {
                    if (!m_IsHandleExecuteInspectionBusy) return;

                    m_IsUserAbort = true;
                    AbortInspection(InspectionProcedureResult.ABORT, ErrorCodes.INSPECTION_ABORTED_BY_USER);

                    // else not working on an inspection so nothing to abort
                }
            }
        }

        /// <summary>
        /// Starts the continuous measurements of the currrent .
        /// </summary>
        public void StartContinuousMeasurement()
        {
            lock (m_LockExectionInspectionBusy)
            {
                if (m_IsHandleExecuteInspectionBusy && !m_IsMeasuring)
                // an inspection is running and no measurements are currently running
                {
                    StartContinuousMeasurementFromCurrentInspectionProcedureStep();
                }
            }
        }

        /// <summary>
        /// Starts the continuous measurements.
        /// </summary>
        /// <param name="manometer">The manometer.</param>
        /// <param name="measurementFrequency">The measurement frequency in measurements per second.</param>
        /// <param name="measurementPeriod">The measurement period (optional). If no time is given the measurements continue until a manual stop.</param>
        /// <param name="extraMeasurementPeriod">The extra measurement period (optional). The time measurements continue after a stop (both automatic and manual stops).</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        private void StartContinuousMeasurement(DigitalManometer manometer, int measurementFrequency, int? measurementPeriod = null, int? extraMeasurementPeriod = null)
        {
            lock (m_LockExectionInspectionBusy)
            {
                // An inspection is running, so allow start of continuous measurements
                if (!m_IsHandleExecuteInspectionBusy) return;

                // Store variables for after switching
                m_MeasurementFrequency = measurementFrequency;
                m_ContinuousMeasurementPeriod = (measurementPeriod.HasValue && (measurementPeriod.Value != 0)) ? measurementPeriod.Value * 1000 : Timeout.Infinite;
                m_ContinuousExtraMeasurementPeriod = (extraMeasurementPeriod.HasValue && (extraMeasurementPeriod.Value != 0)) ? extraMeasurementPeriod.Value * 1000 : Timeout.Infinite;

                // now switch to the requested manometer, if succesfull then start the measurement
                SwitchManoMeter(manometer);
            }
        }

        private Stack<DeviceCommand> m_SwitchManometerStack;

        private void SwitchManoMeter(DigitalManometer manometer)
        {
            if (m_Initialization.ConnectedDigitalManometer == manometer)
            {
                ExecuteNextSwitchManoMeterStep();
            }
            else
            {
                switch (m_Initialization.DeviceType)
                {
                    case DeviceType.PlexorBluetoothIrDA:
                        m_SwitchManometerStack = new Stack<DeviceCommand>();
                        m_SwitchManometerStack.Push(DeviceCommand.FlushManometerCache);
                        m_SwitchManometerStack.Push(DeviceCommand.ExitRemoteLocalCommandMode);
                        m_SwitchManometerStack.Push(manometer.ToDeviceCommand());
                        m_SwitchManometerStack.Push(DeviceCommand.FlushBluetoothCache);
                        m_SwitchManometerStack.Push(DeviceCommand.EnterRemoteLocalCommandMode);
                        ExecuteNextSwitchManoMeterStep();
                        break;
                    case DeviceType.PlexorBluetoothWIS:
                        m_SwitchManometerStack = new Stack<DeviceCommand>();
                        m_SwitchManometerStack.Push(manometer.ToDeviceCommand());
                        m_SwitchManometerStack.Push(DeviceCommand.EnterRemoteLocalCommandMode);
                        ExecuteNextSwitchManoMeterStep();
                        break;
                    case DeviceType.Unknown:
                    default:
                        break;
                }
            }
        }

        private void ExecuteNextSwitchManoMeterStep()
        {
            if (m_SwitchManometerStack?.Count > 0)
            {
                m_CurrentCommand = m_SwitchManometerStack.Pop();
                CommunicationControl.SendCommand(m_CurrentCommand, string.Empty, CommandResult);
            }
            else
            {
                m_CurrentCommand = DeviceCommand.None;
                HandleCommandSwitchManometerCallback(true, 0);
            }
        }

        /// <summary>
        /// Commands the result.
        /// </summary>
        /// <param name="commandSucceeded">if set to <c>true</c> [command success].</param>
        /// <param name="errorCode">The error code.</param>
        /// <param name="message">The message.</param>
        private void CommandResult(bool commandSucceeded, int errorCode, string message)
        {
            if (commandSucceeded)
            {
                switch (m_CurrentCommand)
                {
                    case DeviceCommand.SwitchToManometerTH1:
                        m_Initialization.ConnectedDigitalManometer = DigitalManometer.TH1;
                        break;
                    case DeviceCommand.SwitchToManometerTH2:
                        m_Initialization.ConnectedDigitalManometer = DigitalManometer.TH2;
                        break;
                    case DeviceCommand.Disconnect:
                        m_Initialization.ConnectedDigitalManometer = DigitalManometer.Unknown;
                        break;
                    default:
                        break;
                }

                ExecuteNextSwitchManoMeterStep();
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("HandleCommandSwitchManometerCallback returned an error: {0}", errorCode);
                m_IsRetryCallAllowed = true; //just to check.
                m_SwitchManometerStack.Clear();
                OnInspectionError(new InspectionErrorEventArgs { ErrorCode = errorCode });
            }
        }

        /// <summary>
        /// Stops the continuous measurement.
        /// </summary>
        public void StopContinuousMeasurement()
        {
            m_userManualRequestedStop = !m_SafetyValueEventSent;
            DoStopContinuousMeasurement();
        }

        
        private void DoStopContinuousMeasurement()
        {
            CommunicationLogger.Info("StopContinuousMeasurement");
            lock (m_LockExectionInspectionBusy)
            {
                if (!m_IsHandleExecuteInspectionBusy || !m_IsMeasuring) return;

                if (m_ContinuousMeasurementPeriod == Timeout.Infinite) // infinite measurement period
                {
                    OnContinuousMeasurementPeriodCompleted(null);
                }
            }
        }

        /// <summary>
        /// Stops and finalizes the continuous measurement.
        /// </summary>
        /// <exception cref="InspectionException">Thrown when the continuous measurement could not be stopped</exception>
        private void StopAndFinalizeContinuousMeasurement()
        {
            lock (m_LockExectionInspectionBusy)
            {
                if (!m_IsHandleExecuteInspectionBusy) return;

                m_MeasurementEndDate = DateTime.Now;
                m_StoppingMeasurementResetEvent.Reset();
                System.Diagnostics.Debug.WriteLine("BL InspectionActivityControl: Stopping ContinuousMeasurement");
                CommunicationControl.StopContinuousMeasurement(HandleStopContinuousMeasurementResultCallback);
                m_StoppingMeasurementResetEvent.WaitOne();
                System.Diagnostics.Debug.WriteLine("BL InspectionActivityControl: ContinuousMeasurement stopped (succeeded = '{0}')", m_MeasurementStoppedSucceeded);

                if (m_MeasurementStoppedSucceeded)
                {
                    m_ContinuousMeasurementWorker.FinishMeasurements();
                    m_IsMeasuring = false;
                }
                else
                {
                    m_IsMeasuring = false;
                    CommunicationLogger.Error("Error while stopping measurement");
                    throw new InspectionException("Failed to stop continuous measurement", InspectionProcedureResult.ERROR, ErrorCodes.INSPECTION_COULD_NOT_STOP_CONTINUOUS_MEASUREMENT);
                }
            }
        }
        #endregion IInspectionActivityControl

        #region Private functions
        /// <summary>
        /// Handles the command switch manometer callback called before start of continuous measurement.
        /// </summary>
        /// <param name="commandSucceeded">if set to <c>true</c> [command succeeded].</param>
        /// <param name="errorCode">The error code.</param>
        private void HandleCommandSwitchManometerCallback(bool commandSucceeded, int errorCode)
        {

            m_userManualRequestedStop = false;
            if (!commandSucceeded)
            {
                System.Diagnostics.Debug.WriteLine("HandleCommandSwitchManometerCallback returned an error: {0}", errorCode);
                AbortInspection(InspectionProcedureResult.ERROR, errorCode);
            }
            else
            {
                try
                {
                    m_ContinuousMeasurementWorker.MeasurementFrequency = m_MeasurementFrequency;
                    var gclInformation = LookupGasControlLine();
                    ReadVolumVaAndVolumeVak(gclInformation);

                    var scriptCommand5X = m_CurrentInspectionProcedureStep as ScriptCommand5X;

                    if (scriptCommand5X == null)
                    {
                        throw new InspectionException("Invalid inspection procedure step.", InspectionProcedureResult.ERROR, ErrorCodes.INSPECTION_INVALID_PROCEDURE_STEP);
                    }

                    var resolution = (scriptCommand5X.DigitalManometer == DigitalManometer.TH1) ? m_ManometerInformationTH1.Resolution : m_ManometerInformationTH2.Resolution;

                    m_ContinuousMeasurementWorker.Resolution = resolution;

                    SetMeasurementUnit(scriptCommand5X);

                    CommunicationControl.StartContinuousMeasurement(m_MeasurementFrequency, HandleMeasurementErrorCommandResultCallback, HandleMeasureResultCallback, HandleMeasurementStartedCallback);
                }
                catch (InspectionException inspectionException)
                {
                    AbortInspection(inspectionException.InspectionProcedureResult, inspectionException.ErrorCode);
                }
            }
        }

        /// <summary>
        /// Sets the measurement unit for the continuous measurement worker and set the class member unit of measurement.
        /// </summary>
        /// <param name="scriptCommand5X">The scriptcommand5X.</param>
        /// <exception cref="InspectionException">Thrown when the measurement unit is invalid.</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2208:InstantiateArgumentExceptionsCorrectly")]
        private void SetMeasurementUnit(ScriptCommand5X scriptCommand5X)
        {
            try
            {
                m_UnitOfMeasurement = UnitOfMeasurement.UNSET;

                var unit = (scriptCommand5X.DigitalManometer == DigitalManometer.TH1) ? m_ManometerInformationTH1.Unit : m_ManometerInformationTH2.Unit;

                System.Diagnostics.Debug.WriteLine("InspectionActivityControl: MeasurementUnit {0}", new object[] { unit });

                m_ContinuousMeasurementWorker.MeasurementUnit = unit;

                SetContinuousMeasurementWorkerSettings(unit);

                m_UnitOfMeasurement = scriptCommand5X.ScriptCommand == ScriptCommand5XType.ScriptCommand51 ? GetLeakageUnit(scriptCommand5X) : unit.ToEnum<UnitOfMeasurement>();
            }
            catch (ArgumentOutOfRangeException argumentOutOfRangeException)
            {
                var errorMessage = string.Format(CultureInfo.InvariantCulture, "Invalid measurement unit. Exception '{0}'.", argumentOutOfRangeException.Message);
                throw new InspectionException(errorMessage, InspectionProcedureResult.ERROR, ErrorCodes.INSPECTION_INVALID_MANOMETER_UNIT);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.Double.Parse(System.String)")]
        private void SetContinuousMeasurementWorkerSettings(string unit)
        {
            var settings = new clsSettings();
            var changeRateUnit = settings.get_GetSetting(SETTING_UNITS_SECTION, SETTING_CHANGERATE_UNIT).ToString();
            var mbarMinToLeakageFactor = double.Parse(settings.get_GetSetting(SETTING_UNITS_SECTION, SETTING_MBARMIN_TO_LEAKAGE_UNIT_FACTOR).ToString(), CultureInfo.InvariantCulture);
            var measuredLeakageToMbarMinFactor = double.Parse(settings.get_GetSetting(SETTING_UNITS_SECTION, SETTING_MEASURED_LEAKAGE_TO_MBARMIN).ToString(), CultureInfo.InvariantCulture);
            var qvsLeakageUnit = settings.get_GetSetting(SETTING_UNITS_SECTION, SETTING_QVS_LEAKAGE_UNIT).ToString();
            var qvsFactor = double.Parse(settings.get_GetSetting(SETTING_UNITS_SECTION, SETTING_QVS_FACTOR).ToString(), CultureInfo.InvariantCulture);
            var lowHighPressureFactor = double.Parse(settings.get_GetSetting(SETTING_UNITS_SECTION, SETTING_LOW_HIGH_PRESSURE_FACTOR).ToString(), CultureInfo.InvariantCulture);
            var lowPressureUnit = settings.get_GetSetting(SETTING_UNITS_SECTION, SETTING_LOW_PRESSURE_UNIT).ToString();

            m_ContinuousMeasurementWorker.ChangeRateUnit = changeRateUnit;
            m_ContinuousMeasurementWorker.MbarMinToLeakageFactor = mbarMinToLeakageFactor;
            m_ContinuousMeasurementWorker.QVSLeakageUnit = qvsLeakageUnit;
            m_ContinuousMeasurementWorker.QVSFactor = qvsFactor;
            m_ContinuousMeasurementWorker.LowHighPressureFactor = (unit == lowPressureUnit) ? 1.0 : lowHighPressureFactor;
            m_ContinuousMeasurementWorker.MeasuredLeakageToMbarMinFactor = measuredLeakageToMbarMinFactor;
        }

        /// <summary>
        /// Reads the volum va and volume vak and sets them on the continuous measurement worker
        /// </summary>
        /// <param name="gclInformation">The GCL information.</param>
        /// <returns>True if volumeva and volumevak could be read, otherwise false.</returns>
        /// <exception cref="InspectionException">Thrown when volumeVa or volumeVak are unreadable.</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        private void ReadVolumVaAndVolumeVak(GasControlLineInformation gclInformation)
        {
            try
            {
                m_ContinuousMeasurementWorker.VolumeVa = double.NaN;
                m_ContinuousMeasurementWorker.VolumeVak = double.NaN;

                var volumeVa = Regex.Match(gclInformation.VolumeVA.Trim(), @"^[\d,.]+").Value;
                var volumeVak = Regex.Match(gclInformation.VolumeVAK.Trim(), @"^[\d,.]+").Value;

                if (!string.IsNullOrEmpty(volumeVa))
                {
                    m_VolumeVa = double.Parse(volumeVa.Replace(",", "."), CultureInfo.InvariantCulture);
                }

                if (!string.IsNullOrEmpty(volumeVak))
                {
                    m_VolumeVak = double.Parse(volumeVak.Replace(",", "."), CultureInfo.InvariantCulture);
                }

                m_ContinuousMeasurementWorker.VolumeVa = m_VolumeVa;
                m_ContinuousMeasurementWorker.VolumeVak = m_VolumeVak;
            }
            catch
            {
                throw new InspectionException("Failed to read VolumeVa(k).", InspectionProcedureResult.ERROR, ErrorCodes.INSPECTION_COULD_NOT_READ_VOLUMEVAK_OR_VOLUME_VA);
            }
        }

        /// <summary>
        /// Lookups the gas control line that belongs to the current PRS and GCL
        /// </summary>
        /// <returns>The gascontrolline or null if not available.</returns>
        /// <exception cref="InspectionException">Thrown when the required gascontroline is not available.</exception>
        private GasControlLineInformation LookupGasControlLine()
        {
            try
            {
                return StationInformationManager.StationsInformation.Where(stationInfo => stationInfo.PRSName.Equals(m_PrsName, StringComparison.Ordinal)).SelectMany(stationInfo => stationInfo.GasControlLines).Single(gcl => gcl.GasControlLineName.Equals(m_GclName, StringComparison.Ordinal));
            }
            catch (InvalidOperationException)
            {
                var message = string.Format(CultureInfo.InvariantCulture, "Could not locate the required gcl '{0}' of prs '{1}'", m_GclName, m_PrsName);
                throw new InspectionException(message, InspectionProcedureResult.ERROR, ErrorCodes.INSPECTION_COULD_NOT_READ_VOLUMEVAK_OR_VOLUME_VA);
            }
        }

        /// <summary>
        /// Handles the command result callback for the continuous measurements.
        /// </summary>
        /// <param name="commandSucceeded">if set to <c>true</c> [command succeeded].</param>
        /// <param name="errorCode">The error code.</param>
        /// <param name="message">The message.</param>
        private void HandleMeasurementErrorCommandResultCallback(bool commandSucceeded, int errorCode, string message)
        {
            if (!commandSucceeded)
            {
                System.Diagnostics.Debug.WriteLine("HandleMeasurementErrorCommandResultCallback returned an error: {0}", errorCode);

                m_ContinuousMeasurementPeriodTimer.Change(Timeout.Infinite, Timeout.Infinite);

                // Notify user
                var inspectionErrorEventArgs = new InspectionErrorEventArgs
                {
                    ErrorCode = errorCode
                };
                m_IsRetryCallAllowed = true;
                m_IsMeasuring = false;
                OnInspectionError(inspectionErrorEventArgs);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("HandleMeasurementErrorCommandResultCallback returned no error.");
            }
        }


        /// <summary>
        /// Handles the measure result callback for the continuous measurements.
        /// </summary>
        /// <param name="measurements">The measurements.</param>
        private void HandleMeasureResultCallback(IList<Measurement> measurements)
        {
            m_ContinuousMeasurementWorker.Enqueue = measurements;
            if (m_MeasurementFileCreationEnabled)
            {
                MeasurementReportControl.MeasurementsReceived(measurements);
            }
        }

        private void HandleMeasurementStartedCallback()
        {
            OnContinuousMeasurementStarted(EventArgs.Empty);
        }

        /// <summary>
        /// Aborts the inspection.
        /// </summary>
        /// <param name="inspectionProcedureResult">The inspection procedure result.</param>
        /// <param name="errorCode">The error code.</param>
        private void AbortInspection(InspectionProcedureResult inspectionProcedureResult, int errorCode)
        {
            System.Diagnostics.Debug.WriteLine("BL InspectionactivityControl: Aborting: {0}, {1}.", inspectionProcedureResult.ToString(), errorCode);

            SetInspectionResult(inspectionProcedureResult, errorCode);
            ClearInspectionAndUpdateStatus(inspectionProcedureResult);
            ExecuteNextStep();
        }

        /// <summary>
        /// Handles the InitializationFinished event of the m_Initialization control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.Int32.ToString")]
        private void Initialization_InitializationFinished(object sender, EventArgs args)
        {
            System.Diagnostics.Debug.WriteLine("BL InspectionActivityControl: received init finished");

            m_Isinitializing = false;

            var eventArgs = args as FinishInitializationEventArgs;

            if (eventArgs == null) return;

            m_InitializationResult = eventArgs.Result;

            switch (m_InitializationResult)
            {
                case InitializationResult.SUCCESS:
                case InitializationResult.WARNING:
                case InitializationResult.UNSET:
                    try
                    {
                        OnInitializationFinished(eventArgs);
                        DetermineManometerResolution();

                        // Gather all required inspection data and add it to the report since Initialization has finished.
                        if (m_MeasurementFileCreationEnabled)
                        {
                            AddInspectionProcedureMetadata();
                        }

                        System.Diagnostics.Debug.WriteLine("BL InspectionactivityControl: executing next step.");

                        ExecuteNextStep();
                    }
                    catch (InspectionException inspectionException)
                    {
                        AbortInspection(inspectionException.InspectionProcedureResult, inspectionException.ErrorCode);
                    }
                    break;
                case InitializationResult.USERABORTED:
                    System.Diagnostics.Debug.WriteLine("BL InspectionactivityControl: initialization was aborted by the user!");

                    OnInitializationFinished(eventArgs);
                    AbortInspection(InspectionProcedureResult.ABORT, eventArgs.ErrorCode);
                    break;
                case InitializationResult.ERROR:
                case InitializationResult.TIMEOUT:
                default:
                    Log.Error("BL InspectionactivityControl: initialization failed with errorcode: " + eventArgs.ErrorCode.ToString() + "and result: " + eventArgs.Result.ToString());

                    System.Diagnostics.Debug.WriteLine("BL InspectionactivityControl: initialization failed with errorcode: {0} and result: {1}", eventArgs.ErrorCode, eventArgs.Result);

                    m_IsRetryCallAllowed = true;

                    CommunicationControl.Disconnect(DisconnectBeforeRetryConnectionCallbackResult);
                    break;
            }
        }


        private void InitializationOnUiInputRequested(object sender, UiRequestEventArgs uiRequestEventArgs)
        {
            OnUiRequest(uiRequestEventArgs);
        }

        /// <summary>
        /// Called when the disconnect is finished after a failed initialization
        /// </summary>
        /// <param name="commandSucceeded">if set to <c>true</c> [command succeeded].</param>
        /// <param name="errorCode">The error code.</param>
        /// <param name="message">The message.</param>
        /// <param name="deviceType">The device type.</param>
        private void DisconnectBeforeRetryConnectionCallbackResult(bool commandSucceeded, int errorCode, string message, DeviceType deviceType)
        {
            m_Initialization.DeviceType = deviceType;

            var inspectionErrorEventArgs = new InspectionErrorEventArgs
            {
                ErrorCode = ErrorCodes.INSPECTION_ACTIVITY_CONTROL_INIT_FAILED
            };

            OnInspectionError(inspectionErrorEventArgs);
        }

        /// <summary>
        /// Verifies the manometer pressure range.
        /// </summary>
        /// <exception cref="InspectionException">Thrown when required pressure range could not be verified.</exception>
        private void DetermineManometerResolution()
        {
            foreach (var manometer in m_RequiredManometers)
            {
                string measuredRange;
                switch (manometer)
                {
                    case DigitalManometer.TH1:
                        measuredRange = m_ManometerInformationTH1.Range?.Trim('"').Replace(" ", "") ?? string.Empty;

                        try
                        {
                            m_ManometerInformationTH1.Resolution = ManometerUtils.GetResolutionForManometerPressure(measuredRange);
                        }
                        catch (ArgumentOutOfRangeException)
                        {
                            throw new InspectionException(InspectionProcedureResult.ERROR, ErrorCodes.INSPECTION_MANOMETER_TH1_RESOLUTION_COULD_NOT_BE_FOUND);
                        }
                        break;
                    case DigitalManometer.TH2:
                        measuredRange = m_ManometerInformationTH2.Range?.Trim('"').Replace(" ", "") ?? string.Empty;

                        try
                        {
                            m_ManometerInformationTH2.Resolution = ManometerUtils.GetResolutionForManometerPressure(measuredRange);
                        }
                        catch (ArgumentOutOfRangeException)
                        {
                            throw new InspectionException(InspectionProcedureResult.ERROR, ErrorCodes.INSPECTION_MANOMETER_TH2_RESOLUTION_COULD_NOT_BE_FOUND);
                        }
                        break;
                    case DigitalManometer.Unknown:
                    default:
                        throw new InspectionException(InspectionProcedureResult.ERROR, ErrorCodes.INSPECTION_COULD_NOT_RETRIEVE_PRESSURE_RANGE);
                }
            }
        }

        /// <summary>
        /// Adds the inspection procedure metadata.
        /// </summary>
        /// <exception cref="InspectionException">
        /// Thrown when the inspection procedure metadata could not be added or 
        /// the configured BT-address is undefined in the plexor information.
        /// </exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1806:DoNotIgnoreMethodResults", MessageId = "System.String.Format(System.IFormatProvider,System.String,System.Object[])"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        private void AddInspectionProcedureMetadata()
        {
            PlexorInformation plexorInformation;
            string bluetoothAddress = null;
            try
            {
                bluetoothAddress = m_ConnectionProperties[BLUETOOTH_ADDRESS_IDENTIFIER].TrimStart('(').TrimEnd(')');
                ReportControl.AddBluetoothAddress(bluetoothAddress);
                plexorInformation = PlexorInformationManager.PlexorsInformation.First(plexor => plexor.BlueToothAddress == bluetoothAddress);
            }
            catch (Exception ex)
            {
                var message = string.Format(CultureInfo.InvariantCulture, "AddInspectionProcedureMetadata: Plexor bluetooth address '{0}' is undefined in the plexor information.\n{1}", bluetoothAddress, ex);
                
                System.Diagnostics.Debug.WriteLine(message);
                
                throw new InspectionException("Plexor bluetooth address '{0}' is undefined in the plexor information.", InspectionProcedureResult.ERROR, ErrorCodes.INSPECTION_PLEXOR_BLUETOOTH_ADDRESS_UNDEFINED);
            }

            try
            {
                GasControlLineInformation gclInfo = null;
                var stationInfo = StationInformationManager.LookupStationInformation(m_PrsName);

                if (!string.IsNullOrEmpty(m_GclName))
                {
                    gclInfo = StationInformationManager.LookupGasControlLineInformation(m_PrsName, m_GclName);
                }

                var gclName = gclInfo != null ? gclInfo.GasControlLineName : string.Empty;
                var gclId = gclInfo != null ? gclInfo.GCLIdentification : string.Empty;
                var fsdStart = gclInfo?.FSDStart.ToString(CultureInfo.InvariantCulture) ?? string.Empty;

                // Get the serial number from the Identification (3rd entry, remove trailing ")
                var th1Id = (!string.IsNullOrEmpty(m_ManometerInformationTH1.Identification)) ? m_ManometerInformationTH1.Identification.Split(',')[2].TrimEnd('"') : string.Empty;
                var th2Id = (!string.IsNullOrEmpty(m_ManometerInformationTH2.Identification)) ? m_ManometerInformationTH2.Identification.Split(',')[2].TrimEnd('"') : string.Empty;

                var versionInformation = string.Empty;
                try
                {
                    var general = new clsGeneral();
                    versionInformation = general.ComponentVersion;
                }
                catch
                {
                    /* default is already String.Empty */
                }

                var inspectionProcedureMetadata = new InspectionProcedureMetadata
                {
                    PlexorName = plexorInformation.Name, // From plexor.xml
                    PlexorBtAddress = plexorInformation.BlueToothAddress, // From plexor.xml
                    TH1SerialNumber = th1Id, // Retrieve from init
                    TH2SerialNumber = th2Id, // Retrieve from init
                    Station = stationInfo.PRSName, // From StationInformation.xml
                    StationCode = stationInfo.PRSIdentification, // From StationInformation.xml
                    GasControlLine = gclName, // From StationInformation.xml
                    GasControlLineIdentificationCode = gclId, // From StationInformation.xml
                    TestProgram = m_InspectionProcedureInfo.Name, // From InspectionProcedure.xml
                    InspectionProcedureVersion = m_InspectionProcedureInfo.Version, // From InspectionProcedure.xml
                    InspectorVersion = versionInformation, // From Application
                    FSDStart = fsdStart // From StationInformation.xml
                };

                MeasurementReportControl.AddInspectionProcedureMetadata(inspectionProcedureMetadata);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("AddInspectionProcedureMetadata: Could not add the inspection procedure data\n {0}", ex);

                throw new InspectionException("Could not add the inspection procedure data.", InspectionProcedureResult.ERROR, ErrorCodes.INSPECTION_COULD_NOT_UPDATE_MEASUREMENT_RESULT_FILE);
            }
        }

        /// <summary>
        /// Handles the InitializationStepFinished event of the m_Initialization control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void Initialization_InitializationStepFinished(object sender, EventArgs e)
        {
            OnInitializationStepFinished(e);
        }

        /// <summary>
        /// Handles the InitializationStepStarted event of the m_Initialization control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void Initialization_InitializationStepStarted(object sender, EventArgs e)
        {
            OnInitializationStepStarted(e);
        }

        /// <summary>
        /// Sets the inspection result.
        /// </summary>
        private void SetInspectionResult(InspectionProcedureResult result, int errorCode)
        {
            m_InspectionResult = result;
            m_InspectionResultErrorCode = errorCode;
        }

        /// <summary>
        /// Aborts the inspection.
        /// </summary>
        /// <param name="result">The result.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        private void ClearInspectionAndUpdateStatus(InspectionProcedureResult result)
        {
            if (m_InspectionProcedureInfo == null)
            {
                m_InspectionProcedureInfo = new InspectionProcedureInformation();
            }
            m_InspectionProcedureInfo.ScriptCommandSequence.Clear();

            if (result == InspectionProcedureResult.ERROR)
            {
                m_InspectionResultStatus = InspectionStatus.StartNotCompleted;
                try
                {
                    m_StationInformationManager.SetInspectionStatus(InspectionStatus.StartNotCompleted, m_PrsName, m_GclName);
                }
                catch
                {
                    /* Inspection status could not be set for an unknown prs/gcl combination, since it is not present it safe to ignore the error (no status is set) */
                }
            }
            else if (result == InspectionProcedureResult.ABORT)
            {
                m_InspectionResultStatus = InspectionStatus.StartNotCompleted;
                try
                {
                    m_StationInformationManager.SetInspectionStatus(InspectionStatus.StartNotCompleted, m_PrsName, m_GclName);
                }
                catch
                {
                    /* Inspection status could not be set for an unknown prs/gcl combination, since it is not present it safe to ignore the error (no status is set) */
                }
            }
        }

        /// <summary>
        /// Handles the name of the get inspection procedure.
        /// </summary>
        /// <param name="threadContext">The thread context.</param>
        private void HandleGetInspectionProcedureName(object threadContext)
        {
            try
            {
                m_InspectionProcedureName = StationInformationManager.LookupInspectionProcedureName(m_PrsName, m_GclName);

                HandleExecuteInspection(threadContext);
            }
            catch (InspectorLookupException)
            {
                AbortInspection(InspectionProcedureResult.ERROR, ErrorCodes.INSPECTION_COULD_NOT_RETRIEVE_INSPECTION_PROCEDURE_NAME);
            }
        }

        /// <summary>
        /// Executes the inspection thread.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        private void HandleExecuteInspection(object threadContext)
        {
            CommunicationLogger.Info("Starting inspection: " + m_InspectionProcedureName);
            try
            {
                m_CurrentInspectionStep = 1;
                InitializeInspectionResult();
                m_InspectionProcedureInfo.ScriptCommandSequence = new Stack<ScriptCommandBase>();
                m_InspectionProcedureInfo = InspectionInformationManager.LookupInspectionProcedure(m_InspectionProcedureName, m_PrsName, m_GclName);
                m_TotalInspectionSteps = m_InspectionProcedureInfo.ScriptCommandSequence.Count(ScriptCommandCountsTowardTotalSteps);
                SetupInspectionReport();
                m_StationInformationManager.SetInspectionStatus(InspectionStatus.StartNotCompleted, m_PrsName, m_GclName);
                PrepareRequiredManometers();
            }
            catch (InspectionException inspectionException)
            {
                AbortInspection(inspectionException.InspectionProcedureResult, inspectionException.ErrorCode);
            }
            catch
            {
                AbortInspection(InspectionProcedureResult.ERROR, ErrorCodes.INSPECTION_COULD_NOT_RETRIEVE_INSPECTION);
            }
        }

        /// <summary>
        /// Re-initialize the connection.
        /// </summary>
        /// Pre-condition: PrepareRequiredManometers is executed and the current state is connected or disconnected.
        private void ReInitialize()
        {
            // First make sure we are in the required disconnect state.
            CommunicationControl.Disconnect(ReInitializeConnectionResultCallback);
        }

        /// <summary>
        /// Called when [re initialize result callback].
        /// </summary>
        /// <param name="commandSucceeded">if set to <c>true</c> [command succeeded].</param>
        /// <param name="errorCode">The error code.</param>
        /// <param name="message">The message.</param>
        /// <param name="deviceType">The device type.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        private void ReInitializeConnectionResultCallback(bool commandSucceeded, int errorCode, string message, DeviceType deviceType)
        {
            m_Initialization.DeviceType = deviceType;

            if (!commandSucceeded)
            {
                AbortInspection(InspectionProcedureResult.ERROR, errorCode);
            }
            else
            {
                // Do Connect
                OnInitializationStepStarted(DeviceCommand.Connect);

                var allowedBluetoothDongles = GetAllowedBluetoothDongles();

                //the plexor in not ready to connect immediatly after a connect.
                int timeout;
                try
                {
                    var settings = new clsSettings();
                    timeout = int.Parse(settings.get_GetSetting(SETTING_CATEGORY, SETTING_RECONNECT_TIMEOUT).ToString(), CultureInfo.InvariantCulture);
                }
                catch
                {
                    timeout = 5000;
                }
                
                Task.Factory.StartNew(() =>
                    {
                        Thread.Sleep(timeout);
                    }).ContinueWith((a) =>
                    {
                        // Dev. Note: We already claimed the communication in PrepareRequiredManometers, do not claim again
                        CommunicationControl.Connect(m_ConnectionProperties, allowedBluetoothDongles, ConnectionCallbackResult);
                    });

                //CommunicationControl.Connect(m_ConnectionProperties, allowedBluetoothDongles, ConnectionCallbackResult);
            }
        }

        /// <summary>
        /// Gets the allowed bluetooth dongles.
        /// </summary>
        /// <returns>The list of allowed bluetooth dongle: 1 in case the BT dongle address is defined in the setting, 0 if this is not the case</returns>
        private static List<BluetoothDongleInformation> GetAllowedBluetoothDongles()
        {
            var allowedBluetoothDongles = new List<BluetoothDongleInformation>();
            var settings = new clsSettings();
            var btDongleAddress = settings.get_GetSetting(SETTING_CATEGORY, SETTING_BLUETOOTH_DONGLE_ADDRESS).ToString();
            var isBtDongleAddressDefined = !btDongleAddress.Equals(SETTING_RETURN_NO_VALUE, StringComparison.OrdinalIgnoreCase);
            if (isBtDongleAddressDefined)
            {
                var btDongleInfo = new BluetoothDongleInformation(btDongleAddress);
                allowedBluetoothDongles.Add(btDongleInfo);
            }
            return allowedBluetoothDongles;
        }

        /// <summary>
        /// Prepares the manometers.
        /// </summary>
        /// <exception cref="InspectionException">Thrown when the required manometers could not be prepared or when the communication could not be claimed.</exception>
        private void PrepareRequiredManometers()
        {
            m_LastScriptCommand5X = null;
            m_ValueOutOfBoundsSections = null;
            m_ManometerInformationTH1 = new ManometerInformation();
            m_ManometerInformationTH2 = new ManometerInformation();
            m_RequiredManometers = m_InspectionProcedureInfo.ScriptCommandSequence.OfType<ScriptCommand5X>().Select(ip => ip.DigitalManometer).Distinct().ToList();
            if (m_RequiredManometers.Count > 0 && m_InspectionResult == InspectionProcedureResult.SUCCESS)
            {
                m_ValueOutOfBoundsSections = InspectionInformationManager.LookupInspectionProcedureSections(m_InspectionProcedureName);

                m_DisconnectRequired = true;
                try
                {
                    m_ConnectionProperties = SettingsUtils.RetrieveConnectionProperties();
                }
                catch
                {
                    throw new InspectionException("Failed to retrieve the connection properties from file.", InspectionProcedureResult.ERROR, ErrorCodes.INSPECTION_COULD_NOT_RETRIEVE_CONNECTION_PROPERTIES);
                }

                Initialize();

                if (m_MeasurementFileCreationEnabled)
                {
                    MeasurementReportControl.InitializeMeasurementReport(m_PrsName, m_GclName, m_InspectionStartTimestamp);
                }

                m_Initialization = null;
                m_InitializationResult = InitializationResult.SUCCESS;
                m_Initialization = new InitializationExecutorControl();
                
                System.Diagnostics.Debug.WriteLine("added events from initialization");

                m_Initialization.InitializationStepStarted += Initialization_InitializationStepStarted;
                m_Initialization.InitializationStepFinished += Initialization_InitializationStepFinished;
                m_Initialization.InitializationFinished += Initialization_InitializationFinished;
                m_Initialization.UiInputRequested += InitializationOnUiInputRequested;

                // Do Connect
                OnInitializationStepStarted(DeviceCommand.Connect);

                var allowedBluetoothDongles = GetAllowedBluetoothDongles();

                if (CommunicationControl.StartCommunication())
                {
                    CommunicationControl.Connect(m_ConnectionProperties, allowedBluetoothDongles, ConnectionCallbackResult);
                }
                else
                {
                    throw new InspectionException("Failed to claim the communication control.", InspectionProcedureResult.ERROR, ErrorCodes.COMMUNICATIONCONTROL_ALREADY_CLAIMED);
                }
            }
            else
            {
                ExecuteNextStep();
            }
        }


        /// <summary>
        /// Initializes this instance.
        /// </summary>
        /// <exception cref="InspectionException">Thrown when threads could not be created.</exception>
        private void Initialize()
        {
            try
            {
                InitializeContinuousMeasurementWorkerThread();
                InitializeMeasurementReportWorkerThread();
            }
            catch (OutOfMemoryException)
            {
                throw new InspectionException(InspectionProcedureResult.ERROR, ErrorCodes.INSPECTION_ACTIVITY_CONTROL_INIT_FAILED);
            }

            CommunicationControl.Initialize();
        }

        /// <summary>
        /// Connects the call back.
        /// </summary>
        /// <param name="commandSucceeded">if set to <c>true</c> [command succeeded].</param>
        /// <param name="errorCode">The error code.</param>
        /// <param name="message">The message.</param>
        /// <param name="deviceType">The device type.</param>
        private void ConnectionCallbackResult(bool commandSucceeded, int errorCode, string message, DeviceType deviceType)
        {
            m_Initialization.DeviceType = deviceType;

            var result = commandSucceeded ? InitializationStepResult.SUCCESS : InitializationStepResult.ERROR;

            if (errorCode == 0)
            {
                // As the connect and disconnect return 0, we translate it to the Step being executed successfully.
                errorCode = ErrorCodes.INITIALIZATION_STEP_EXECUTED_SUCCESSFULLY;
            }

            if (!commandSucceeded)
            {
                // If the command did not succeed: Set the message to empty to prevent an error message from being shown.
                message = string.Empty;
            }

            OnInitializationStepFinished(DeviceCommand.Connect, result, message, errorCode, InitializationManometer.BLUETOOTH_MODULE);

            HandleConnectResultCallback(result, errorCode);
        }

        /// <summary>
        /// Handles the command result callback.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <param name="errorCode"></param>
        private void HandleConnectResultCallback(InitializationStepResult result, int errorCode)
        {
            if (result == InitializationStepResult.SUCCESS)
            {
                m_Initialization.ConnectionProperties = m_ConnectionProperties;
                m_Initialization.PrsName = m_PrsName;
                m_Initialization.GclName = m_GclName;
                m_Initialization.IsInitializationForInspection = true;
                m_Isinitializing = true;
                var performSsdInit = m_InspectionProcedureInfo.ScriptCommandSequence.OfType<ScriptCommand5X>().Any(s => IsScriptCommand57Or57(s.ScriptCommand));
                m_Initialization.ExecuteInitialization(m_RequiredManometers, performSsdInit);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("HandleConnectResult returned an error: {0}", errorCode);

                m_IsRetryCallAllowed = true;
                // Notify user
                var inspectionErrorEventArgs = new InspectionErrorEventArgs
                {
                    ErrorCode = errorCode
                };
                OnInspectionError(inspectionErrorEventArgs);
            }
        }

        private bool MeasurementResultShouldBeStored()
        {
            if (m_CurrentInspectionProcedureStep is ScriptCommand5X)
            {
                return MeasurementResultShouldBeStored((m_CurrentInspectionProcedureStep as ScriptCommand5X).ScriptCommand);
            }
            else
            {
                return true;
            }
            
        }

        private bool MeasurementResultShouldBeStored(ScriptCommand5XType? type)
        {
            if (type == null)
            {
                return true;
            }
            return !(IsScriptCommand57Or57(type) && m_userManualRequestedStop && m_5657AutoFlag == true);
        }

        private static bool IsScriptCommand57Or57(ScriptCommand5XType? type)
        {
            if (type == null)
            {
                return false;
            }
            return type == ScriptCommand5XType.ScriptCommand56 || type == ScriptCommand5XType.ScriptCommand57;
        }

        /// <summary>
        /// Handles the stop continuous measurement result callback.
        /// </summary>
        /// <param name="commandSucceeded">if set to <c>true</c> [command succeeded].</param>
        /// <param name="errorCode">The error code.</param>
        /// <param name="message">The message.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.String.Format(System.String,System.Object)")]
        private void HandleStopContinuousMeasurementResultCallback(bool commandSucceeded, int errorCode, string message)
        {
            if (!commandSucceeded)
            {
                System.Diagnostics.Debug.WriteLine($"HandleStopContinuousMeasurementResultCallback returned an error: {errorCode}");
            }
            m_MeasurementStoppedSucceeded = commandSucceeded;
            m_StoppingMeasurementResetEvent.Set();
        }

        /// <summary>
        /// Executes the partial inspection thread.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        private void HandleExecutePartialInspection(object threadContext)
        {
            CommunicationLogger.Info("Starting partial inspection");
            try
            {
                m_CurrentInspectionStep = 1;
                InitializeInspectionResult();
                m_InspectionProcedureInfo.ScriptCommandSequence = new Stack<ScriptCommandBase>();
                m_InspectionProcedureInfo = InspectionInformationManager.LookupPartialInspectionProcedure(m_SectionSelection, m_PrsName, m_GclName);
                m_TotalInspectionSteps = m_InspectionProcedureInfo.ScriptCommandSequence.Count(ScriptCommandCountsTowardTotalSteps);
                SetupInspectionReport();
                m_StationInformationManager.SetInspectionStatus(InspectionStatus.StartNotCompleted, m_PrsName, m_GclName);
                PrepareRequiredManometers();
            }
            catch (InspectionException inspectionException)
            {
                AbortInspection(inspectionException.InspectionProcedureResult, inspectionException.ErrorCode);
            }
            catch
            {
                AbortInspection(InspectionProcedureResult.ERROR, ErrorCodes.INSPECTION_COULD_NOT_RETRIEVE_INSPECTION);
            }
        }

        /// <summary>
        /// Checks if the scriptcommand adds to the total amount of steps (some command types are not considered actual steps)
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns>The script command <paramref name="command"/> is part of the total amount of commands.</returns>
        private static bool ScriptCommandCountsTowardTotalSteps(ScriptCommandBase command)
        {
            return !(command is ScriptCommand2 || command is ScriptCommand70 || command is ScriptCommand42);
        }

        /// <summary>
        /// Initializes the inspection result.
        /// </summary>
        private void InitializeInspectionResult()
        {
            m_InspectionStartTimestamp = DateTime.Now;
            m_InspectionResult = InspectionProcedureResult.SUCCESS;
            m_InspectionResultErrorCode = ErrorCodes.INSPECTION_FINISHED_SUCCESSFULLY;
            m_InspectionResultStatus = InspectionStatus.Completed;
            m_InspectionReportCreated = false;
        }

        /// <summary>
        /// Setups the inspection report.
        /// </summary>
        /// <exception cref="InspectionException">Thrown when the setup of the inspection report failed.</exception>
        private void SetupInspectionReport()
        {
            try
            {
                ReportControl.AddTemporaryFileToResult();
                ReportControl.StartInspectionReport(InspectionStatus.StartNotCompleted, m_InspectionStartTimestamp);
                m_InspectionReportCreated = true;
            }
            catch (InspectorReportControlException)
            {
                throw new InspectionException("Failed to set up inspection report", InspectionProcedureResult.ERROR, ErrorCodes.INSPECTION_COULD_NOT_CREATE_INSPECTION_REPORT);
            }

            StationInformation stationInfo;
            GasControlLineInformation gclInfo = null;

            try
            {
                stationInfo = StationInformationManager.LookupStationInformation(m_PrsName);
                if (!string.IsNullOrEmpty(m_GclName))
                {
                    gclInfo = StationInformationManager.LookupGasControlLineInformation(stationInfo, m_GclName);
                }
            }
            catch (InspectorLookupException)
            {
                throw new InspectionException("Failed to retrieve the gas control line.", InspectionProcedureResult.ERROR, ErrorCodes.INSPECTION_COULD_NOT_RETRIEVE_STATION_INFORMATION);
            }

            try
            {
                var inspectionProcedure = new InspectionProcedureGenericInformation
                {
                    InpectionStatus = InspectionStatus.StartNotCompleted,
                    PrsIdentification = stationInfo.PRSIdentification,
                    PrsName = stationInfo.PRSName,
                    PrsCode = stationInfo.PRSCode,
                    Crc = string.Empty,
                    ProcedureName = m_InspectionProcedureInfo.Name,
                    ProcedureVersion = m_InspectionProcedureInfo.Version
                };

                if (!string.IsNullOrEmpty(m_GclName))
                {
                    inspectionProcedure.GclName = gclInfo?.GasControlLineName;
                    inspectionProcedure.GclIdentification = gclInfo?.GCLIdentification;
                    inspectionProcedure.GclCode = gclInfo?.GCLCode;
                }
                
                ReportControl.AddInspectionProcedure(inspectionProcedure);
            }
            catch (InspectorReportControlException)
            {
                throw new InspectionException("Failed to add inspection procedure to report.", InspectionProcedureResult.ERROR, ErrorCodes.INSPECTION_COULD_NOT_UPDATE_REPORT);
            }
        }

        /// <summary>
        /// Handles the store remark.
        /// </summary>
        /// <param name="remarkStepResult">The remark step result.</param>
        private void HandleStoreRemark(object remarkStepResult)
        {
            try
            {
                var remark = remarkStepResult as InspectionStepResultText;
                if (remark != null)
                {
                    ReportControl.UpdateRemark(remark.SequenceNumber, remark.Text);
                }
            }
            catch (InspectorReportControlException)
            {
                AbortInspection(InspectionProcedureResult.ERROR, ErrorCodes.INSPECTION_COULD_NOT_UPDATE_REPORT);
            }
        }

        /// <summary>
        /// Handles the inspection step complete (runs on the threadpool).
        /// </summary>
        /// <param name="stepResult">The step result.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.Int64.ToString")]
        private void HandleInspectionStepComplete(object stepResult)
        {
            CommunicationLogger.Info("Finished inspection procedure step: " + m_CurrentInspectionProcedureStep.SequenceNumber);
            try
            {
                ValidateInspectionResultSequenceNumberValid(stepResult as InspectionStepResultBase);

                // store results // And Set Value for measurement data
                if (m_CurrentInspectionProcedureStep is ScriptCommand4)
                {
                    if (!(stepResult is InspectionStepResultEmpty))
                    {
                        StoreResultsOfScriptCommand4(stepResult);
                    }
                }
                else if (m_CurrentInspectionProcedureStep is ScriptCommand41)
                {
                    StoreResultsOfScriptCommand41(stepResult);
                }
                else if (m_CurrentInspectionProcedureStep is ScriptCommand42)
                {
                    StoreResultsOfScriptCommand42(stepResult);
                }
                else if (m_CurrentInspectionProcedureStep is ScriptCommand43)
                {
                    StoreResultsOfScriptCommand43(stepResult);
                }

                // Finish Measurement report
                if (m_CurrentInspectionProcedureStep is ScriptCommand5X)
                {
                    AddMeasurementMetadataForScriptCommand5X();
                    m_MeasurementValue = double.NaN;
                }

                ExecuteNextStep();
            }
            catch (InspectionException inspectionException)
            {
                AbortInspection(inspectionException.InspectionProcedureResult, inspectionException.ErrorCode);
            }
        }

        /// <summary>
        /// Finishes the measurement.
        /// </summary>
        private void FinishNormalMeasurement()
        {
            var scriptCommand = m_CurrentInspectionProcedureStep as ScriptCommand5X;
            DetermineMeasurementValue();

            var valueOutOfBounds = DetermineValueOutOfBounds(scriptCommand);

            OnMeasurementResult(m_MeasurementValue, valueOutOfBounds);

            if (valueOutOfBounds)
            {
                m_InspectionResultStatus = InspectionStatus.CompletedValueOutOfLimits;
                // Select the step 2 closest to the current sequenceNumber.
                var sectionSelections = m_ValueOutOfBoundsSections.SectionSelectionEntities.Where(section => scriptCommand != null && section.SequenceNumber < scriptCommand.SequenceNumber).ToList();
                if (sectionSelections.Any())
                {
                    sectionSelections.Last().IsSelected = true;
                }
            }

            m_MeasurementEndDate = DateTime.Now;
            AddMeasurementMetadataForScriptCommand5X();
        }

        /// <summary>
        /// Determines the value out of bounds.
        /// </summary>
        /// <param name="scriptCommand">The script command.</param>
        /// <returns></returns>
        private bool DetermineValueOutOfBounds(ScriptCommand5X scriptCommand)
        {
            var valueOutOfBounds = false;

            if (scriptCommand.StationStepObject?.Boundaries != null)
            {
                valueOutOfBounds = DetermineValueOutOfBounds(scriptCommand.StationStepObject.Boundaries);
            }

            return valueOutOfBounds;
        }

        /// <summary>
        /// Determines the value out of bounds.
        /// </summary>
        /// <param name="boundaries">The boundaries.</param>
        /// <returns></returns>
        private bool DetermineValueOutOfBounds(StationStepObjectBoundaries boundaries)
        {
            var settings = new clsSettings();

            var lowPressureUnit = "";
            var conversionFactor = 1.0;
            var highPressureUnit = "";
            try
            {
                lowPressureUnit = settings.get_GetSetting(SETTING_UNITS_SECTION, SETTING_LOW_PRESSURE_UNIT).ToString();
                highPressureUnit = settings.get_GetSetting(SETTING_UNITS_SECTION, SETTING_HIGH_PRESSURE_UNIT).ToString();
                conversionFactor = double.Parse(settings.get_GetSetting(SETTING_UNITS_SECTION, SETTING_LOW_HIGH_PRESSURE_FACTOR).ToString(), CultureInfo.InvariantCulture);
            }
            catch (FormatException)
            {
                Log.Warn("OutOfBounds calculation:unable to read settings file, skipping conversion.");
            }

            var valueOutOfBounds = false;

            var measurementValue = m_MeasurementValue;

            if (m_MeasurementUnit.Equals(lowPressureUnit, StringComparison.OrdinalIgnoreCase) && boundaries.UOV.GetDescription().Equals(highPressureUnit, StringComparison.OrdinalIgnoreCase))
            {
                measurementValue = measurementValue * conversionFactor;
            }
            else if (m_MeasurementUnit.Equals(highPressureUnit, StringComparison.OrdinalIgnoreCase) && boundaries.UOV.GetDescription().Equals(lowPressureUnit, StringComparison.OrdinalIgnoreCase))
            {
                measurementValue = measurementValue / conversionFactor;
            }

            if (boundaries.ValueMin != 0)
            {
                valueOutOfBounds |= (measurementValue < boundaries.ValueMin);
            }

            if (boundaries.ValueMax != 0)
            {
                valueOutOfBounds |= (measurementValue > boundaries.ValueMax);
            }

            return valueOutOfBounds;
        }

        /// <summary>
        /// Adds the measurement metadata for script command70.
        /// </summary>
        /// <exception cref="InspectionException">Thrown when the Measurement Metadata for Script Command 70 cannot be added.</exception>
        private void AddMeasurementMetadataForScriptCommand70()
        {
            try
            {
                if (m_MeasurementFileCreationEnabled)
                {
                    var interval = DetermineMeasurementInterval();

                    var measurementMetadata = new MeasurementMetadata
                    {
                        ScriptCommand = SCRIPTCOMMAND_70_NAME,
                        EndOfMeasurement = m_MeasurementEndDate,
                        Interval = interval,
                        FieldInAccessDatabase = string.Empty,
                        ObjectName = string.Empty,
                        Measurepoint = string.Empty,
                        Value = string.Empty,
                    };
                    MeasurementReportControl.AddMeasurementMetadata(measurementMetadata);
                }
            }
            catch (MeasurementReportControlException ex)
            {
                System.Diagnostics.Debug.WriteLine("AddMeasurementMetadataSC70: Failed to add measurement metadata to measurement file\n {0}", ex);

                throw new InspectionException("Failed to add measurement metadata to measurement file.", InspectionProcedureResult.ERROR, ErrorCodes.INSPECTION_COULD_NOT_UPDATE_MEASUREMENT_RESULT_FILE);
            }
        }

        /// <summary>
        /// Adds the measurement metadata.
        /// </summary>
        /// <exception cref="InspectionException">Thrown when measurement meta data could not be added to the measurement file.</exception>
        private void AddMeasurementMetadataForScriptCommand5X()
        {
            try
            {
                if (!m_MeasurementFileCreationEnabled) return;

                var scriptCommand5X = m_CurrentInspectionProcedureStep as ScriptCommand5X;

                var interval = DetermineMeasurementInterval();
                var scriptCommandName = string.Empty;
                var fieldInAccessDatabase = string.Empty;
                var objectName = string.Empty;
                var measurepoint = string.Empty;
                var unit = string.Empty;
                if (scriptCommand5X != null)
                {
                    switch (scriptCommand5X.DigitalManometer)
                    {
                        case DigitalManometer.TH1:
                            unit = m_ManometerInformationTH1.Unit;
                            break;
                        case DigitalManometer.TH2:
                            unit = m_ManometerInformationTH2.Unit;
                            break;
                        default:
                            break;
                    }

                    switch (scriptCommand5X.ScriptCommand)
                    {
                        case ScriptCommand5XType.ScriptCommand51:
                            scriptCommandName = SCRIPTCOMMAND_51_NAME;
                            unit = m_UnitOfMeasurement.GetDescription();
                            break;
                        case ScriptCommand5XType.ScriptCommand52:
                            scriptCommandName = SCRIPTCOMMAND_52_NAME;
                            break;
                        case ScriptCommand5XType.ScriptCommand53:
                            scriptCommandName = SCRIPTCOMMAND_53_NAME;
                            break;
                        case ScriptCommand5XType.ScriptCommand54:
                            scriptCommandName = SCRIPTCOMMAND_54_NAME;
                            break;
                        case ScriptCommand5XType.ScriptCommand55:
                            scriptCommandName = SCRIPTCOMMAND_55_NAME;
                            break;
                        case ScriptCommand5XType.ScriptCommand56:
                            scriptCommandName = SCRIPTCOMMAND_56_NAME;
                            break;
                        case ScriptCommand5XType.ScriptCommand57:
                            scriptCommandName = SCRIPTCOMMAND_57_NAME;
                            break;
                        default:
                            scriptCommandName = UNKNOWN_SCRIPTCOMMAND_NAME;
                            break;
                    }

                    if (scriptCommand5X.StationStepObject != null)
                    {
                        if (scriptCommand5X.StationStepObject.FieldNo.HasValue)
                        {
                            fieldInAccessDatabase = scriptCommand5X.StationStepObject.FieldNo.Value.ToString(CultureInfo.InvariantCulture);
                        }
                        objectName = scriptCommand5X.StationStepObject.ObjectName;
                        measurepoint = scriptCommand5X.StationStepObject.MeasurePoint;
                    }
                }

                var measurementValue = string.Format(CultureInfo.InvariantCulture, "{0} {1}", m_MeasurementValue.ToString(CultureInfo.InvariantCulture), unit);

                var measurementMetadata = new MeasurementMetadata
                {
                    ScriptCommand = scriptCommandName,
                    EndOfMeasurement = m_MeasurementEndDate,
                    Interval = interval,
                    FieldInAccessDatabase = fieldInAccessDatabase,
                    ObjectName = objectName,
                    Measurepoint = measurepoint,
                    Value = MeasurementResultShouldBeStored(scriptCommand5X?.ScriptCommand) ? measurementValue : string.Empty,
                };
                MeasurementReportControl.AddMeasurementMetadata(measurementMetadata);
            }
            catch (MeasurementReportControlException ex)
            {
                System.Diagnostics.Debug.WriteLine("AddMeasurementMetadata: Failed to add measurement metadata to measurement file\n {0}", ex);

                throw new InspectionException("Failed to add measurement metadata to measurement file.", InspectionProcedureResult.ERROR, ErrorCodes.INSPECTION_COULD_NOT_UPDATE_MEASUREMENT_RESULT_FILE);
            }
        }

        /// <summary>
        /// Gets the leakage unit.
        /// </summary>
        /// <param name="scriptCommand"></param>
        private UnitOfMeasurement GetLeakageUnit(ScriptCommand5X scriptCommand)
        {
            var leakageUnit = UnitOfMeasurement.UNSET;
            var leakageType = scriptCommand.Leakage;
            var settings = new clsSettings();
            var changeRateUnit = UnitOfMeasurement.UNSET;
            var qvsLeakageUnit = UnitOfMeasurement.UNSET;
            try
            {
                changeRateUnit = settings.get_GetSetting(SETTING_UNITS_SECTION, SETTING_CHANGERATE_UNIT).ToString().ToEnum<UnitOfMeasurement>();
            }
            catch (ArgumentException e)
            {
                Log.Error(e.Message);
            }

            try
            {
                qvsLeakageUnit = settings.get_GetSetting(SETTING_UNITS_SECTION, SETTING_QVS_LEAKAGE_UNIT).ToString().ToEnum<UnitOfMeasurement>();
            }
            catch (ArgumentException e)
            {
                Log.Error(e.Message);
            }

            if (scriptCommand.StationStepObject.Boundaries != null)
            {
                if (scriptCommand.StationStepObject.Boundaries.UOV == changeRateUnit)
                {
                    leakageUnit = changeRateUnit;
                }
                else
                {
                    var volumeVaUnavailable = (leakageType == Leakage.V1) && Double.IsNaN(m_VolumeVa);
                    var volumeVakUnavailable = (leakageType == Leakage.V2) && Double.IsNaN(m_VolumeVak);
                    if (volumeVaUnavailable || volumeVakUnavailable)
                    {
                        leakageUnit = changeRateUnit;
                    }
                    else
                    {
                        switch (leakageType)
                        {
                            case Leakage.V1:
                            case Leakage.V2:
                            case Leakage.Membrane:
                                leakageUnit = qvsLeakageUnit;
                                break;
                            case Leakage.Dash:
                                leakageUnit = changeRateUnit;
                                break;
                        }
                    }
                }
            }
            else
            {
                leakageUnit = changeRateUnit;
            }

            return leakageUnit;
        }

        /// <summary>
        /// Determines the measurement interval.
        /// </summary>
        /// <returns>the measurement interval</returns>
        private int DetermineMeasurementInterval()
        {
            int interval;
            switch (m_LastScriptCommand5X.MeasurementFrequency)
            {
                case TypeMeasurementFrequency.Fingerprint:
                    interval = 25;
                    break;
                case TypeMeasurementFrequency.Default:
                default:
                    interval = 10;
                    break;
            }
            return interval;
        }

        /// <summary>
        /// Determines the measurement value.
        /// </summary>
        private void DetermineMeasurementValue()
        {
            var scriptCommand = m_CurrentInspectionProcedureStep as ScriptCommand5X;

            if (scriptCommand == null) return;

            var valueShouldBeRounded = true;

            if (m_LastMeasurement != null)
            {
                switch (scriptCommand.ScriptCommand)
                {
                    case ScriptCommand5XType.ScriptCommand51:
                        if (scriptCommand.StationStepObject.Boundaries == null || scriptCommand.StationStepObject.Boundaries.UOV == UnitOfMeasurement.ItemMbarMin)
                        {
                            m_MeasurementValue = m_LastMeasurement.LeakageValue;
                        }
                        else
                        {
                            switch (scriptCommand.Leakage)
                            {
                                case Leakage.V1:
                                    m_MeasurementValue = !double.IsNaN(m_LastMeasurement.LeakageV1) ? m_LastMeasurement.LeakageV1 : m_LastMeasurement.LeakageValue;
                                    break;
                                case Leakage.V2:
                                    m_MeasurementValue = !double.IsNaN(m_LastMeasurement.LeakageV2) ? m_LastMeasurement.LeakageV2 : m_LastMeasurement.LeakageValue;
                                    break;
                                case Leakage.Membrane:
                                    m_MeasurementValue = m_LastMeasurement.LeakageMembrane;
                                    break;
                                case Leakage.Dash:
                                    m_MeasurementValue = m_LastMeasurement.LeakageValue;
                                    break;
                                default:
                                    break;
                            }
                        }
                        break;
                    case ScriptCommand5XType.ScriptCommand52:
                        m_MeasurementValue = m_LastMeasurement.Average;
                        break;
                    case ScriptCommand5XType.ScriptCommand53:
                    case ScriptCommand5XType.ScriptCommand56:
                        valueShouldBeRounded = false;
                        m_MeasurementValue = m_LastMeasurement.Maximum;
                        break;
                    case ScriptCommand5XType.ScriptCommand54:
                    case ScriptCommand5XType.ScriptCommand57:
                        valueShouldBeRounded = false;
                        m_MeasurementValue = m_LastMeasurement.Minimum;
                        break;
                    case ScriptCommand5XType.ScriptCommand55:
                        valueShouldBeRounded = false;
                        m_MeasurementValue = m_LastMeasurement.Measurement;
                        break;
                    default:
                        break;
                }
            }
            if (valueShouldBeRounded)
            {
                var resolution = scriptCommand.DigitalManometer == DigitalManometer.TH1
                    ? m_ManometerInformationTH1.Resolution
                    : m_ManometerInformationTH2.Resolution;

                m_MeasurementValue = Math.Round(m_MeasurementValue, resolution, MidpointRounding.AwayFromZero);
            }
        }

        /// <summary>
        /// Stores the results of script command5X.
        /// </summary>
        /// <exception cref="InspectionException">Thrown when storage of script command failed.</exception>
        private void StoreResultsOfScriptCommand5X()
        {
            try
            {
                var scriptCommand5X = m_CurrentInspectionProcedureStep as ScriptCommand5X;

                if (scriptCommand5X == null || m_MeasurementEndDate == null) return;

                InspectionProcedureStepResultMeasureValue? measureValue = null;

                if (MeasurementResultShouldBeStored(scriptCommand5X.ScriptCommand))
                {
                    measureValue = new InspectionProcedureStepResultMeasureValue(m_MeasurementValue, m_UnitOfMeasurement);
                }
              
                var inspectionResult = new InspectionProcedureStepResult(scriptCommand5X.SequenceNumber, scriptCommand5X.StationStepObject.MeasurePoint, m_MeasurementEndDate.Value, scriptCommand5X.StationStepObject.FieldNo, scriptCommand5X.StationStepObject.ObjectName, scriptCommand5X.StationStepObject.ObjectID, scriptCommand5X.StationStepObject.MeasurePointID, string.Empty, scriptCommand5X.ObjectNameDescription, scriptCommand5X.MeasurePointDescription, null, measureValue);

                ReportControl.AddResult(inspectionResult);
            }
            catch (InspectorReportControlException)
            {
                throw new InspectionException("Failed to store the results of scriptcommand 43.", InspectionProcedureResult.ERROR, ErrorCodes.INSPECTION_COULD_NOT_UPDATE_REPORT);
            }
        }

        /// <summary>
        /// Stores the results of script command43.
        /// </summary>
        /// <param name="stepResult">The step result.</param>
        /// <exception cref="InspectionException">Thrown when storage of script command failed.</exception>
        private void StoreResultsOfScriptCommand43(object stepResult)
        {
            try
            {
                var stepResultText = stepResult as InspectionStepResultText;
                var scriptCommand43 = m_CurrentInspectionProcedureStep as ScriptCommand43;

                if (stepResultText == null) return;

                var answerText = stepResultText.Text.Equals("No option", StringComparison.OrdinalIgnoreCase) ? string.Empty : stepResultText.Text;

                if (scriptCommand43 == null) return;

                var inspectionResult = new InspectionProcedureStepResult(stepResultText.SequenceNumber, scriptCommand43.StationStepObject.MeasurePoint, DateTime.Now, scriptCommand43.StationStepObject.FieldNo, scriptCommand43.StationStepObject.ObjectName, scriptCommand43.StationStepObject.ObjectID, scriptCommand43.StationStepObject.MeasurePointID, answerText, scriptCommand43.ObjectNameDescription, scriptCommand43.MeasurePointDescription);

                ReportControl.AddResult(inspectionResult);
            }
            catch (InspectorReportControlException)
            {
                throw new InspectionException("Failed to store the results of scriptcommand 43.", InspectionProcedureResult.ERROR, ErrorCodes.INSPECTION_COULD_NOT_UPDATE_REPORT);
            }
        }

        /// <summary>
        /// Stores the results of script command42.
        /// </summary>
        /// <param name="stepResult">The step result.</param>
        /// <exception cref="InspectionException">Thrown when storage of script command failed.</exception>
        private void StoreResultsOfScriptCommand42(object stepResult)
        {
            try
            {
                var stepResultEmpty = stepResult as InspectionStepResultEmpty;
                var scriptCommand42 = m_CurrentInspectionProcedureStep as ScriptCommand42;

                if (stepResultEmpty == null || scriptCommand42 == null) return;

                var inspectionResult = new InspectionProcedureStepResult(stepResultEmpty.SequenceNumber, scriptCommand42.StationStepObject.MeasurePoint, DateTime.Now, scriptCommand42.StationStepObject.FieldNo, scriptCommand42.StationStepObject.ObjectName, scriptCommand42.StationStepObject.ObjectID, scriptCommand42.StationStepObject.MeasurePointID, String.Empty, scriptCommand42.ObjectNameDescription, scriptCommand42.MeasurePointDescription);

                ReportControl.AddResult(inspectionResult);
            }
            catch (InspectorReportControlException)
            {
                throw new InspectionException("Failed to store the results of scriptcommand 42.", InspectionProcedureResult.ERROR, ErrorCodes.INSPECTION_COULD_NOT_UPDATE_REPORT);
            }
        }

        /// <summary>
        /// Stores the results of script command41.
        /// </summary>
        /// <param name="stepResult">The step result.</param>
        /// <exception cref="InspectionException">Thrown when storage of script command failed.</exception>
        private void StoreResultsOfScriptCommand41(object stepResult)
        {
            try
            {
                var stepResultSelections = stepResult as InspectionStepResultSelections;
                var scriptCommand41 = m_CurrentInspectionProcedureStep as ScriptCommand41;

                if (stepResultSelections == null || scriptCommand41 == null) return;

                var inspectionResult = new InspectionProcedureStepResult(stepResultSelections.SequenceNumber, scriptCommand41.StationStepObject.MeasurePoint, DateTime.Now, scriptCommand41.StationStepObject.FieldNo, scriptCommand41.StationStepObject.ObjectName, scriptCommand41.StationStepObject.ObjectID, scriptCommand41.StationStepObject.MeasurePointID, stepResultSelections.Remark, scriptCommand41.ObjectNameDescription, scriptCommand41.MeasurePointDescription, stepResultSelections.ListSelections);

                ReportControl.AddResult(inspectionResult);
            }
            catch (InspectorReportControlException)
            {
                throw new InspectionException("Failed to store the results of scriptcommand 41.", InspectionProcedureResult.ERROR, ErrorCodes.INSPECTION_COULD_NOT_UPDATE_REPORT);
            }
        }

        /// <summary>
        /// Stores the results script command4.
        /// </summary>
        /// <param name="stepResult">The step result.</param>
        /// <exception cref="InspectionException">Thrown when storage of script command failed.</exception>
        private void StoreResultsOfScriptCommand4(object stepResult)
        {
            try
            {
                var stepResultText = stepResult as InspectionStepResultText;
                var scriptCommand4 = m_CurrentInspectionProcedureStep as ScriptCommand4;

                if (stepResultText == null || scriptCommand4 == null) return;

                var inspectionResult = new InspectionProcedureStepResult(stepResultText.SequenceNumber, scriptCommand4.StationStepObject.MeasurePoint, DateTime.Now, scriptCommand4.StationStepObject.FieldNo, scriptCommand4.StationStepObject.ObjectName, scriptCommand4.StationStepObject.ObjectID, scriptCommand4.StationStepObject.MeasurePointID, stepResultText.Text, scriptCommand4.ObjectNameDescription, scriptCommand4.MeasurePointDescription);
                ReportControl.AddResult(inspectionResult);
            }
            catch (InspectorReportControlException)
            {
                throw new InspectionException("Failed to store the results of scriptcommand 4.", InspectionProcedureResult.ERROR, ErrorCodes.INSPECTION_COULD_NOT_UPDATE_REPORT);
            }
        }

        /// <summary>
        /// Check if the received sequence number matches the current sequence number that is being handled.
        /// </summary>
        /// <param name="stepResult">The step result.</param>
        /// <exception cref="InspectionException">Thrown when the sequence number is invalid.</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Inspector.BusinessLogic.InspectionActivityControl.SetInspectionResult(Inspector.BusinessLogic.InspectionActivityControl+InspectionProcedureResult,System.String,System.Int32,System.String)")]
        private void ValidateInspectionResultSequenceNumberValid(InspectionStepResultBase stepResult)
        {
            var isResultSequenceMatchingCurrentStepSequence = (stepResult.SequenceNumber == m_CurrentInspectionProcedureStep.SequenceNumber);
            if (!isResultSequenceMatchingCurrentStepSequence)
            {
                // result is not for the current step -> error
                throw new InspectionException("Invalid sequence number.", InspectionProcedureResult.ERROR, ErrorCodes.INSPECTION_INCORRECT_SEQUENCE_NUMBER);
            }
        }

        /// <summary>
        /// Executes the next step.
        /// </summary>
        private void ExecuteNextStep()
        {
            lock (m_LockExectionInspectionBusy)
            {
                if (m_InspectionProcedureInfo.ScriptCommandSequence.Count > 0)
                {
                    System.Diagnostics.Debug.WriteLine("BL InspectionactivityControl: continueing with next inspection step.");
                    SendInspectionProcedureStep();
                    System.Diagnostics.Debug.WriteLine("BL InspectionactivityControl: continued with next inspection step.");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("BL InspectionactivityControl: finalizing inspection procedure.");
                    FinalizeInspectionProcedure();
                    System.Diagnostics.Debug.WriteLine("BL InspectionactivityControl: finalized inspection procedure.");
                }
            }
        }

        /// <summary>
        /// Finalizes the inspection procedure.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        private void FinalizeInspectionProcedure()
        {
            lock (m_LockExectionInspectionBusy)
            {
                if (!m_IsHandleExecuteInspectionBusy)
                {
                    return;
                }

                if (m_IsUserAbort)
                {
                    m_InspectionResultStatus = InspectionStatus.StartNotCompleted;
                    m_InspectionResultErrorCode = ErrorCodes.INSPECTION_ABORTED_BY_USER;
                    m_IsUserAbort = false;
                }

                if (m_IsMeasuring)
                {
                    m_ContinuousMeasurementPeriodTimer.Change(Timeout.Infinite, Timeout.Infinite);

                    try
                    {
                        CommunicationLogger.Debug("FinalizeInspectionProcedure: Stopping Continuous Measurement, measurement was still running.");
                        StopAndFinalizeContinuousMeasurement();
                    }
                    catch
                    {
                        /* Ignore, as we are finalizing anyway */
                    }

                    try
                    {
                        if (m_CurrentInspectionProcedureStep is ScriptCommand5X)
                        {
                            m_ContinuousMeasurementWorker.FinishMeasurements();
                            AddMeasurementMetadataForScriptCommand5X();
                        }
                        else
                        {
                            AddMeasurementMetadataForScriptCommand70();
                        }
                    }
                    catch (InspectionException exception)
                    {
                        SetInspectionResult(exception.InspectionProcedureResult, exception.ErrorCode);
                        ClearInspectionAndUpdateStatus(exception.InspectionProcedureResult);
                    }
                }

                if (m_DisconnectRequired)
                {
                    System.Diagnostics.Debug.WriteLine("FinalizeInspectionProcedure: Disconnecting");
                    CommunicationControl.Disconnect(DisconnectConnectionCallBackResult);
                }

                if (m_InspectionResult == InspectionProcedureResult.SUCCESS)
                {
                    try
                    {
                        m_StationInformationManager.SetInspectionStatus(m_InspectionResultStatus, m_PrsName, m_GclName);
                    }
                    catch
                    {
                        /* Inspection status could not be set for an unknown prs/gcl combination, since it is not present it safe to ignore the error (no status is set) */
                    }
                }

                try
                {
                    if (m_InspectionReportCreated)
                    {
                        ReportControl.FinishInspectionReport(m_InspectionResultStatus, DateTime.Now);
                    }

                    if (m_MeasurementFileCreationEnabled)
                    {
                        MeasurementReportControl.FinishMeasurementReport();
                    }

                    OnInspectionFinished();
                }
                catch (InspectorReportControlException)
                {
                    SetInspectionResult(InspectionProcedureResult.ERROR, ErrorCodes.INSPECTION_COULD_NOT_UPDATE_REPORT);
                    ClearInspectionAndUpdateStatus(InspectionProcedureResult.ERROR);
                }
                catch (MeasurementReportControlException ex)
                {
                    System.Diagnostics.Debug.WriteLine("FinalizeInspectionProcedure: Could not finish the measurement report\n {0}", ex);
                    SetInspectionResult(InspectionProcedureResult.ERROR, ErrorCodes.INSPECTION_COULD_NOT_UPDATE_MEASUREMENT_RESULT_FILE);
                    ClearInspectionAndUpdateStatus(InspectionProcedureResult.ERROR);
                }

                m_IsHandleExecuteInspectionBusy = false;
            }
        }

        /// <summary>
        /// Disconnects the call back.
        /// </summary>
        /// <param name="commandSucceeded">if set to <c>true</c> [command succeeded].</param>
        /// <param name="errorCode">The error code.</param>
        /// <param name="message">The message.</param>
        /// <param name="deviceType">The device type.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "CallBack")]
        private void DisconnectConnectionCallBackResult(bool commandSucceeded, int errorCode, string message, DeviceType deviceType)
        {
            m_Initialization.DeviceType = deviceType;

            m_Initialization.InitializationStepStarted -= Initialization_InitializationStepStarted;
            m_Initialization.InitializationStepFinished -= Initialization_InitializationStepFinished;
            m_Initialization.InitializationFinished -= Initialization_InitializationFinished;
            m_Initialization.UiInputRequested -= InitializationOnUiInputRequested;

            CommunicationControl.StopCommunication();
        }

        /// <summary>
        /// Sends the inspection procedure step.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.Int64.ToString")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        private void SendInspectionProcedureStep()
        {
            lock (m_LockExectionInspectionBusy)
            {
                
                    m_CurrentInspectionProcedureStep = m_InspectionProcedureInfo.ScriptCommandSequence.Pop();
                CommunicationLogger.Info("Starting inspection procedure step: " + m_CurrentInspectionProcedureStep.SequenceNumber);
                if (!(m_CurrentInspectionProcedureStep is ScriptCommand70))
                {
                    if (m_CurrentInspectionProcedureStep is ScriptCommand5X)
                    {
                        if (m_MeasurementFileCreationEnabled)
                        {
                            MeasurementReportControl.SetUpMeasurementFileWhenRequired();
                        }
                    }

                    var command = m_CurrentInspectionProcedureStep.CopyScriptCommand();
                    OnExecuteInspectionStep(command, m_CurrentInspectionStep, m_TotalInspectionSteps);
                    // send copy to not allow changes to our member scriptcommand.
                    UpdateCurrentScriptCommandStep();
                    StartContinuousMeasurementFromCurrentInspectionProcedureStep();
                }
                else
                {
                    UpdateCurrentScriptCommandStep();

                    try
                    {
                        var scriptCommand = m_CurrentInspectionProcedureStep as ScriptCommand70;
                        m_IsScriptCommand70Enabled = scriptCommand.Mode;
                        if (!scriptCommand.Mode && m_IsMeasuring)
                        {
                            CommunicationLogger.Debug("Stopping Continuous Measurement as SC70 is stopped.");
                            StopAndFinalizeContinuousMeasurement();
                            AddMeasurementMetadataForScriptCommand70();
                        }
                        ExecuteNextStep();
                    }
                    catch (InspectionException exception)
                    {
                        AbortInspection(exception.InspectionProcedureResult, exception.ErrorCode);
                    }
                }
            }
        }

        /// <summary>
        /// Starts the continuous measurement from current inspection procedure step.
        /// </summary>
        private void StartContinuousMeasurementFromCurrentInspectionProcedureStep()
        {
            try
            {
                // Also start continuous measurement
                if (!(m_CurrentInspectionProcedureStep is ScriptCommand5X)) return;
                CheckIoStatus(m_CurrentInspectionProcedureStep as ScriptCommand5X);

            }
            catch (InspectionException exception)
            {
                AbortInspection(exception.InspectionProcedureResult, exception.ErrorCode);
            }
        }

        private void DoStartContinuousMeasurementFromCurrentInspectionProcedureStep()
        {
            if (m_IsMeasuring)
            {
                CommunicationLogger.Debug("Stopping Continuous Measurement as a new SC5x is sent.");
                StopAndFinalizeContinuousMeasurement();
                AddMeasurementMetadataForScriptCommand70();
            }

            m_LastScriptCommand5X = (ScriptCommand5X) m_CurrentInspectionProcedureStep;
            m_MeasurementEndDate = null;
            HandleContinuousMeasurement(m_LastScriptCommand5X);
        }

        private void CheckIoStatus(ScriptCommand5X step)
        {

            if (!IsScriptCommand57Or57(step.ScriptCommand) || m_Initialization.DeviceType == DeviceType.PlexorBluetoothIrDA || m_5657AutoFlag == false)
            {
                DoStartContinuousMeasurementFromCurrentInspectionProcedureStep();
            }
            else if (m_IsMeasuring) //there are already commands pouring in, just check the las IO status
            {
                if (CheckIoStatusValue(m_LastIoStatus))
                {
                    DoStartContinuousMeasurementFromCurrentInspectionProcedureStep();
                }
                else
                {
                   AskUserToContinueMeasurement(step);
                }
            }

            else
            {
                CommunicationControl.SendCommand(DeviceCommand.MeasureSingleValue, string.Empty, HandleMeasureSingleValueResult);
            }
        }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.Int32.Parse(System.String)")]
        private void HandleMeasureSingleValueResult(bool succeeded, int errorCode, string message)
        {
            int iostatus;
            var parseSuccessful = int.TryParse(message, out iostatus);
            if (parseSuccessful && CheckIoStatusValue(iostatus))
            {
                DoStartContinuousMeasurementFromCurrentInspectionProcedureStep();
            }
            else
            {
                AskUserToContinueMeasurement(m_CurrentInspectionProcedureStep as ScriptCommand5X);
            }
        }

        private void AskUserToContinueMeasurement(ScriptCommand5X step)
        {
            m_UiResponse = UiResponse.No;
            m_UiResponseReceived.Reset();
            OnUiRequest(new UiRequestEventArgs("Io status incorrect, continue with measurement?",UIRequestResponseType.YesNoRecheck));
            m_UiResponseReceived.WaitOne(60000);
            HandleUiResponse(step);
        }

        private bool CheckIoStatusValue(int iostatus)
        {
            m_IoBitStatusStart = iostatus;
            return (iostatus == 0x0C || iostatus == 0x05 || iostatus == 0x07 || iostatus == 0x0E);
        }

        private void HandleUiResponse(ScriptCommand5X step)
        {
            switch (m_UiResponse)
            {
                case UiResponse.No:
                    AbortInspection(InspectionProcedureResult.ERROR, ErrorCodes.INSPECTION_INCORRECT_IO_STATUS);
                    break;
                case UiResponse.Yes:
                    m_5657AutoFlag = false;
                    DoStartContinuousMeasurementFromCurrentInspectionProcedureStep();
                    break;
                case UiResponse.Recheck:
                    CheckIoStatus(step);
                    break;
                default:
                    AbortInspection(InspectionProcedureResult.ERROR, ErrorCodes.GENERAL_INVALID_UIRESPONSE);
                    break;
            }
        }
        /// <summary>
        /// Handle the start of continuous measurement and measurement reporting.
        /// </summary>
        /// <param name="scriptCommand5X">The scriptcommand5X.</param>
        /// <exception cref="InspectionException">Thrown when the Continuous measurement could not be started</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        private void HandleContinuousMeasurement(ScriptCommand5X scriptCommand5X)
        {
            m_ExecutingExtraMeasurements = false;
            m_LastMeasurement = null;
            var frequency = (int)scriptCommand5X.MeasurementFrequency;
            var manometer = scriptCommand5X.DigitalManometer;
            var measurementPeriod = scriptCommand5X.MeasurementPeriod;
            var extraMeasurementPeriod = scriptCommand5X.ExtraMeasurementPeriod;

            m_MeasurementUnit = string.Empty;
            switch (scriptCommand5X.DigitalManometer)
            {
                case DigitalManometer.TH1:
                    m_MeasurementUnit = m_ManometerInformationTH1.Unit;
                    break;
                case DigitalManometer.TH2:
                    m_MeasurementUnit = m_ManometerInformationTH2.Unit;
                    break;
                default:
                    throw new InspectionException(InspectionProcedureResult.ERROR, ErrorCodes.INSPECTION_COULD_NOT_START_MEASUREMENT_REPORT);
            }

            try
            {
                StartContinuousMeasurement(manometer, frequency, measurementPeriod, extraMeasurementPeriod);
                m_MeasurementStartDate = DateTime.Now;
            }
            catch (ArgumentOutOfRangeException)
            {
                throw new InspectionException("Failed to retrieve the DTR setting for the manometer.", InspectionProcedureResult.ERROR, ErrorCodes.INSPECTION_COULD_NOT_RETRIEVE_MANOMETER_DTR);
            }

            if (!m_MeasurementFileCreationEnabled) return;

            try
            {
                MeasurementReportControl.StartMeasurement(m_MeasurementUnit, m_MeasurementStartDate, m_Initialization.GetStoreIOStatus());
            }
            catch (MeasurementReportControlException)
            {
                throw new InspectionException("Could not start measurement report control", InspectionProcedureResult.ERROR, ErrorCodes.INSPECTION_COULD_NOT_START_MEASUREMENT_REPORT);
            }
        }

        /// <summary>
        /// Updates the current script command step.
        /// </summary>
        private void UpdateCurrentScriptCommandStep()
        {
            if (ScriptCommandCountsTowardTotalSteps(m_CurrentInspectionProcedureStep))
            {
                m_CurrentInspectionStep++;
            }
        }

        /// <summary>
        /// Instantiates continuous measurement and starts the worker thread.
        /// </summary>
        /// <exception cref="OutOfMemoryException">Thrown when the thread could not be started because of out of memory exception.</exception>
        private void InitializeContinuousMeasurementWorkerThread()
        {
            if (m_ContinuousMeasurementWorkerThreadInitialized) return;

            m_ContinuousMeasurementWorker = new ContinuousMeasurementWorker();
            m_ContinuousMeasurementWorker.MeasurementValuesReceived += continuousMeasurementWorker_MeasurementValuesReceived;

            m_ContinuousMeasurementWorkerThread = new Thread(() => m_ContinuousMeasurementWorker.WorkerThread())
            {
                Name = "ContinuousMeasurementWorkerThread"
            };

            m_ContinuousMeasurementWorkerThread.Start();
            m_ContinuousMeasurementWorkerThreadInitialized = true;

            m_ContinuousMeasurementPeriodTimer = new Timer(OnContinuousMeasurementPeriodCompleted, null, Timeout.Infinite, Timeout.Infinite);
        }

        /// <summary>
        /// Initializes the measurement report worker thread.
        /// </summary>
        /// <exception cref="OutOfMemoryException">Thrown when the thread could not be started because of out of memory exception.</exception>
        private void InitializeMeasurementReportWorkerThread()
        {
            if (m_MeasurementReportWorkerThreadInitialized) return;

            if (m_MeasurementFileCreationEnabled)
            {
                m_MeasurementFileThread = new Thread(() => MeasurementReportControl.MeasurementDataWorkerThread())
                {
                    Name = "measurementThread"
                };

                m_MeasurementFileThread.Start();
            }

            m_MeasurementReportWorkerThreadInitialized = true;
        }

        /// <summary>
        /// Gets the measurement file creation enabled.
        /// </summary>
        /// <returns>The boolean state of the measurement file write enabled option.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        private static bool GetMeasurementFileCreationEnabled()
        {
            bool fileCreationEnabled;
            try
            {
                var settings = new clsSettings();
                fileCreationEnabled = bool.Parse(settings.get_GetSetting(SETTING_CATEGORY_APPLICATION, SETTING_MEASUREMENT_FILE_WRITE_ENABLED).ToString());
            }
            catch
            {
                fileCreationEnabled = DEFAULT_MEASUREMENT_FILE_WRITE_ENABLED;
            }
            return fileCreationEnabled;
        }

        /// <summary>
        /// Gets the measurement command delay.
        /// </summary>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        private static int GetMeasurementCommandDelay()
        {
            int measurementCommandDelay;
            try
            {
                var settings = new clsSettings();
                measurementCommandDelay = int.Parse(settings.get_GetSetting(SETTING_CATEGORY_APPLICATION, SETTING_MEASUREMENT_COMMAND_DELAY).ToString(), CultureInfo.InvariantCulture);
                if (measurementCommandDelay < 750)
                {
                    measurementCommandDelay = DEFAULT_CONTINUOUS_MEASUREMENT_DELAY;
                }
            }
            catch
            {
                measurementCommandDelay = DEFAULT_CONTINUOUS_MEASUREMENT_DELAY;
            }

            return measurementCommandDelay;
        }

        /// <summary>
        /// Called when [continuous measurement period completed].
        /// </summary>
        /// <param name="state">The state.</param>
        /// <exception cref="MeasurementReportControlException">Thrown when extra data could not be added to the measurements.</exception>
        private void OnContinuousMeasurementPeriodCompleted(object state)
        {
            CommunicationLogger.Info("OnContinuousMeasurementPeriodCompleted");
            lock (m_LockExectionInspectionBusy)
            {
                try
                {
                    var requiresExtraMeasurementTime = (m_ContinuousExtraMeasurementPeriod != 0 && m_ContinuousExtraMeasurementPeriod != Timeout.Infinite);
                    if (requiresExtraMeasurementTime)
                    {
                        FinishNormalMeasurement();
                        var dueTime = m_ContinuousExtraMeasurementPeriod;
                        m_ContinuousExtraMeasurementPeriod = Timeout.Infinite;
                        OnExtraMeasurementStarted(EventArgs.Empty);
                        m_ExecutingExtraMeasurements = true;

                        if (m_MeasurementFileCreationEnabled)
                        {
                            try
                            {
                                MeasurementReportControl.StartExtraDataMeasurement();
                            }
                            catch (MeasurementReportControlException ex)
                            {
                                System.Diagnostics.Debug.WriteLine("OnContinuousMeasurementPeriodCompleted: Failed to add extra data measurement to the measurement report\n {0}", ex);

                                throw new InspectionException("Failed to add extra data measurement to the measurement report.", InspectionProcedureResult.ERROR, ErrorCodes.INSPECTION_COULD_NOT_UPDATE_MEASUREMENT_RESULT_FILE);
                            }
                        }

                        m_ContinuousMeasurementPeriodTimer.Change(dueTime, Timeout.Infinite);
                    }
                    else
                    {
                        if (!m_ExecutingExtraMeasurements)
                        {
                            FinishNormalMeasurement();
                        }
                        m_ContinuousExtraMeasurementPeriod = Timeout.Infinite;
                        m_ContinuousMeasurementPeriodTimer.Change(Timeout.Infinite, Timeout.Infinite);
                        if (m_IsScriptCommand70Enabled)
                        {
                            // Finish the measurement data
                            AddMeasurementMetadataForScriptCommand5X();
                            //see if we want to store the iostatus:
                            var scriptCommand5X = m_CurrentInspectionProcedureStep as ScriptCommand5X;
                            var storeIOStatus = scriptCommand5X != null && m_Initialization.GetStoreIOStatus();
                            m_ContinuousMeasurementWorker.FinishMeasurements();
                            m_MeasurementStartDate = DateTime.Now;
                            if (m_MeasurementFileCreationEnabled)
                            {
                                MeasurementReportControl.StartMeasurement(m_MeasurementUnit, m_MeasurementStartDate, storeIOStatus);
                            }
                        }
                        else
                        {
                            CommunicationLogger.Debug("Stopping Continuous Measurement as a the measurement period is completed.");
                            StopAndFinalizeContinuousMeasurement();
                            AddMeasurementMetadataForScriptCommand5X();
                        }

                        StoreResultsOfScriptCommand5X();

                        var executeInspectionStepEventArgs = new ExecuteInspectionStepEventArgs(m_CurrentInspectionProcedureStep, m_CurrentInspectionStep, m_TotalInspectionSteps);
                        OnMeasurementsCompleted(executeInspectionStepEventArgs);
                    }
                }
                catch (MeasurementReportControlException)
                {
                    AbortInspection(InspectionProcedureResult.ERROR, ErrorCodes.INSPECTION_COULD_NOT_UPDATE_MEASUREMENT_RESULT_FILE);
                }
                catch (InspectionException inspectionException)
                {
                    AbortInspection(inspectionException.InspectionProcedureResult, inspectionException.ErrorCode);
                }
            } // lock
        }

        /// <summary>
        /// Handles the MeasurementValuesReceived event of the continuousMeasurementWorker control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Inspector.BusinessLogic.Interfaces.Events.MeasurementValuesEventArgs"/> instance containing the event data.</param>
        private void continuousMeasurementWorker_MeasurementValuesReceived(object sender, MeasurementValuesEventArgs e)
        {
            if (!m_ExecutingExtraMeasurements)
            {
                m_LastMeasurement = e.MeasurementValues.Last();
            }

            m_LastIoStatus = e.MeasurementValues.Last()?.IoStatus ?? 0;

            var eventArgs = new MeasurementEventArgs
            {
                Measurements = e.MeasurementValues
            };
            OnMeasurementsReceived(eventArgs);

            var step = m_CurrentInspectionProcedureStep as ScriptCommand5X;

            if (!m_ExecutingExtraMeasurements && step != null && IsScriptCommand57Or57(step.ScriptCommand))
            {
                if (m_5657AutoFlag == true)
                {
                    if (m_IoBitStatusStart == 0x07)
                    {
                        m_IoBitStatusStart = e.MeasurementValues?.FirstOrDefault(m => m.IoStatus != 0x07)?.IoStatus ?? 0x07;
                    }
                    else
                    {
                        var registeredMaxResult = e.MeasurementValues?.FirstOrDefault(m => m.IoStatus == 0x07);
                        if (registeredMaxResult != null && !m_SafetyValueEventSent)
                        {

                            m_MeasurementReportControl.RegisterMaximumValue(registeredMaxResult.Measurement,
                                DateTime.Now);
                            m_SafetyValueEventSent = true;
                            FinishScriptCommand5657();
                            Task.Factory.StartNew(() =>
                            {
                                OnSafetyValueTriggered(registeredMaxResult.Measurement, registeredMaxResult.IoStatus);
                            });
                        }
                    }
                }
                if (m_userManualRequestedStop && m_5657AutoFlag == false)
                {
                    FinishScriptCommand5657();
                }
            }
        }

        private void FinishScriptCommand5657()
        {
            Task.Factory.StartNew(() =>
            {
                m_ContinuousMeasurementPeriod = Timeout.Infinite;
                m_ContinuousMeasurementPeriodTimer.Change(Timeout.Infinite, Timeout.Infinite);

                if (!m_SafetyValueEventSent)
                {
                    DoStopContinuousMeasurement();
                }
            });
        }

        /// <summary>
        /// Stores the required manometer data.
        /// </summary>
        /// <param name="finishEventargs">The <see cref="Inspector.BusinessLogic.Interfaces.Events.FinishInitializationStepEventArgs"/> instance containing the event data.</param>
        /// <exception cref="InspectionException">Thrown when the manometer identification could not be added to the report file.</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        private void StoreRequiredManometerData(FinishInitializationStepEventArgs finishEventargs)
        {
            try
            {
                if (finishEventargs.StepId == DeviceCommand.CheckIdentification.ToString())
                {
                    if (finishEventargs.Manometer == InitializationManometer.TH1)
                    {
                        m_ManometerInformationTH1.Identification = finishEventargs.Message;
                        ReportControl.AddManometerIdentification(MeterNumber.ID_DM1, finishEventargs.Message);
                    }
                    else if (finishEventargs.Manometer == InitializationManometer.TH2)
                    {
                        m_ManometerInformationTH2.Identification = finishEventargs.Message;
                        ReportControl.AddManometerIdentification(MeterNumber.ID_DM2, finishEventargs.Message);
                    }
                }
                else if (finishEventargs.StepId == DeviceCommand.CheckPressureUnit.ToString())
                {
                    if (finishEventargs.Manometer == InitializationManometer.TH1)
                    {
                        m_ManometerInformationTH1.Unit = finishEventargs.Message;
                    }
                    else if (finishEventargs.Manometer == InitializationManometer.TH2)
                    {
                        m_ManometerInformationTH2.Unit = finishEventargs.Message;
                    }
                }
                else if (finishEventargs.StepId == DeviceCommand.CheckRange.ToString())
                {
                    if (finishEventargs.Manometer == InitializationManometer.TH1)
                    {
                        m_ManometerInformationTH1.Range = finishEventargs.Message;
                    }
                    else if (finishEventargs.Manometer == InitializationManometer.TH2)
                    {
                        m_ManometerInformationTH2.Range = finishEventargs.Message;
                    }
                }
            }
            catch (InspectorReportControlException)
            {
                throw new InspectionException("Failed to add the manometer identification to the report file.", InspectionProcedureResult.ERROR, ErrorCodes.INSPECTION_COULD_NOT_UPDATE_REPORT);
            }
        }
        #endregion Private functions

        #region IInspectionActivityControl Events
        /// <summary>
        /// Occurs when [execute step].
        /// </summary>
        public event EventHandler ExecuteInspectionStep;

        /// <summary>
        /// Occurs when [inspection finished].
        /// </summary>
        public event EventHandler InspectionFinished;

        /// <summary>
        /// Occurs when [initialization step started].
        /// </summary>
        public event EventHandler InitializationStepStarted;

        /// <summary>
        /// Occurs when [initialization step finished].
        /// </summary>
        public event EventHandler InitializationStepFinished;

        /// <summary>
        /// Occurs when [initialization finished].
        /// </summary>
        public event EventHandler InitializationFinished;

        /// <summary>
        /// Occurs when [inspection error].
        /// </summary>
        public event EventHandler InspectionError;

        /// <summary>
        /// Occurs when [measurements received].
        /// </summary>
        public event EventHandler MeasurementsReceived;

        /// <summary>
        /// Occurs when [measurements completed].
        /// </summary>
        public event EventHandler MeasurementsCompleted;

        /// <summary>
        /// Occurs when [extra measurement started].
        /// </summary>
        public event EventHandler ExtraMeasurementStarted;

        /// <summary>
        /// Occurs when [measurement result].
        /// </summary>
        public event EventHandler MeasurementResult;

        /// <summary>
        /// Occurs when [Continuous measurement started]
        /// </summary>
        public event EventHandler ContinuousMeasurementStarted;

        /// <summary>
        /// Occurs when [safety value triggered].
        /// </summary>
        public event EventHandler SafetyValueTriggered;

        /// <summary>
        /// Occurs when [device unpaired].
        /// </summary>
        public event EventHandler DeviceUnPaired;

        /// <summary>
        /// Occurs when [device unpair finished]
        /// </summary>
        public event EventHandler DeviceUnPairFinished;

        /// <summary>
        /// Occurs when [ui request]
        /// </summary>
        public event EventHandler<UiRequestEventArgs> UiRequest;

        #endregion IInspectionActivityControl Events

        #region Event Handlers
        /// <summary>
        /// Called when [inspection finished].
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        private void OnInspectionFinished()
        {
            var settings = new clsSettings();
            var settingString = settings.get_GetSetting(SETTING_CATEGORY_APPLICATION, SETTING_SAVE_COMMUNICATION_LOG_ON_SUCCESS).ToString();
            var saveLog = bool.Parse(string.IsNullOrEmpty(settingString) ? "false" : settingString);
            HandleCommunicationLog(saveLog);

            InspectionFinished?.Invoke(this, new InspectionFinishedEventArgs(m_InspectionResultStatus, m_InspectionResultErrorCode, m_ValueOutOfBoundsSections));
        }

        /// <summary>
        /// Called when [execute inspection step].
        /// </summary>
        /// <param name="scriptCommand">The script command.</param>
        /// <param name="currentInspectionStep"></param>
        /// <param name="totalInspectionSteps"></param>
        private void OnExecuteInspectionStep(ScriptCommandBase scriptCommand, int currentInspectionStep, int totalInspectionSteps)
        {
            ExecuteInspectionStep?.Invoke(this, new ExecuteInspectionStepEventArgs(scriptCommand, currentInspectionStep, totalInspectionSteps));
        }

        /// <summary>
        /// Called when [initialization step started].
        /// </summary>
        /// <param name="id">The id.</param>
        private void OnInitializationStepStarted(DeviceCommand id)
        {
            InitializationStepStarted?.Invoke(this, new StartInitializationStepEventArgs(id.ToString()));
        }

        /// <summary>
        /// Raises the <see cref="E:InitializationStepStarted"/> event.
        /// </summary>
        /// <param name="eventArgs">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void OnInitializationStepStarted(EventArgs eventArgs)
        {
            if (InitializationStepStarted == null) return;

            var startEventArgs = eventArgs as StartInitializationStepEventArgs;

            InitializationStepStarted(this, startEventArgs);
        }

        /// <summary>
        /// Called when [initialization step finished].
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="result">The result.</param>
        /// <param name="message">The message.</param>
        /// <param name="errorCode">The error code.</param>
        /// <param name="manometer">The manometer.</param>
        private void OnInitializationStepFinished(DeviceCommand id, InitializationStepResult result, string message, int errorCode, InitializationManometer manometer)
        {
            InitializationStepFinished?.Invoke(this, new FinishInitializationStepEventArgs(id.ToString(), result, message, errorCode, manometer));
        }

        /// <summary>
        /// Called when [initialization step finished].
        /// </summary>
        /// <param name="eventArgs"></param>
        private void OnInitializationStepFinished(EventArgs eventArgs)
        {
            try
            {
                var finishEventargs = eventArgs as FinishInitializationStepEventArgs;

                if (finishEventargs == null) return;

                System.Diagnostics.Debug.WriteLine("BL InspectionActivityControl: '{0}', '{1}', '{2}'", finishEventargs.StepId, finishEventargs.ErrorCode, finishEventargs.Message);

                StoreRequiredManometerData(finishEventargs);

                InitializationStepFinished?.Invoke(this, finishEventargs);
            }
            catch (InspectionException inspectionException)
            {
                AbortInspection(inspectionException.InspectionProcedureResult, inspectionException.ErrorCode);
            }
        }

        /// <summary>
        /// Called when [initialization finished].
        /// </summary>
        /// <param name="eventArgs"></param>
        private void OnInitializationFinished(EventArgs eventArgs)
        {
            if (m_IsUserAbort)
            {
                Abort();

                System.Diagnostics.Debug.WriteLine("Removed events from initialization");

                m_Initialization.InitializationStepStarted -= Initialization_InitializationStepStarted;
                m_Initialization.InitializationStepFinished -= Initialization_InitializationStepFinished;
                m_Initialization.InitializationFinished -= Initialization_InitializationFinished;
                m_Initialization.UiInputRequested -= InitializationOnUiInputRequested;
            }

            m_5657AutoFlag = m_Initialization.ScriptCommandFlag;


            if (InitializationFinished == null) return;


            var finishEventArgs = eventArgs as FinishInitializationEventArgs;

            InitializationFinished(this, finishEventArgs);
        }

        /// <summary>
        /// Raises the <see cref="E:InspectionError"/> event.
        /// </summary>
        /// <param name="eventArgs">The <see cref="EventArgs"/> instance containing the event data.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.Int32.ToString")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.String.Format(System.String,System.Object)"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        private void OnInspectionError(EventArgs eventArgs)
        {
            var inspectionErrorEventArgs = eventArgs as InspectionErrorEventArgs;
            if (inspectionErrorEventArgs != null)
            {
                CommunicationLogger.Error("Inspection failed with error code: " + inspectionErrorEventArgs.ErrorCode);
            }
            else
            {
                CommunicationLogger.Error("Inspection failed, no error code was found.");
            }

            Log.Error("An error occurred during inspection. Saving communication log.");

            HandleCommunicationLog(true);

            InspectionError?.Invoke(this, inspectionErrorEventArgs);
        }


        private void OnUiRequest(UiRequestEventArgs e)
        {
            var handler = UiRequest;
            if (handler != null)
            {
                handler(this, e);
            }
            else
            {
                AbortInspection(InspectionProcedureResult.ABORT, ErrorCodes.INSPECTION_UIREQUEST_UNATTACHED);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.String.Format(System.String,System.Object)")]
        private static void HandleCommunicationLog(bool saveLog)
        {
            try
            {
                var fileAppender = ((Hierarchy)LogManager.GetRepository()).GetAppenders().OfType<FileAppender>().FirstOrDefault(n => n.Name.Equals("CommunicationLoggerAppender"));

                if (fileAppender == null || !File.Exists(fileAppender.File))
                {
                    return;
                }

                if (saveLog)
                {
                    var dateTimeString = DateTime.Now.ToString("dd-MM-yyyy HH.mm.ss", CultureInfo.InvariantCulture);
                    var destinationFilename = $"CommuncationLog_{dateTimeString}.log";
                    var destinationDirectory = Path.GetDirectoryName(fileAppender.File) ?? ".";
                    var fullFileName = Path.Combine(destinationDirectory, destinationFilename);
                    File.Copy(fileAppender.File, fullFileName);
                }

                File.Delete(fileAppender.File);
            }
            catch (Exception ex)
            {
                Log.Warn("error while copying/removing communications log: ", ex);
            }
        }

        /// <summary>
        /// Raises the <see cref="E:MeasurementsReceived"/> event.
        /// </summary>
        /// <param name="eventArgs">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        private void OnMeasurementsReceived(EventArgs eventArgs)
        {
            if (MeasurementsReceived == null) return;

            var measurementEventArgs = eventArgs as MeasurementEventArgs;

            MeasurementsReceived(this, measurementEventArgs);
        }

        /// <summary>
        /// Raises the <see cref="E:MeasurementsCompleted"/> event.
        /// </summary>
        /// <param name="eventArgs">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void OnMeasurementsCompleted(EventArgs eventArgs)
        {
            if (MeasurementsCompleted != null)
            {
                var executeInspectionStepEventArgs = eventArgs as ExecuteInspectionStepEventArgs;
                MeasurementsCompleted(this, executeInspectionStepEventArgs);
            }
        }

        /// <summary>
        /// Raises the <see cref="E:ExtraMeasurementStarted"/> event.
        /// </summary>
        /// <param name="eventArgs">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void OnExtraMeasurementStarted(EventArgs eventArgs)
        {
            ExtraMeasurementStarted?.Invoke(this, eventArgs);
        }

        /// <summary>
        /// Raises the <see cref="E:ContinuousMeasurementStarted"/> event.
        /// </summary>
        private void OnContinuousMeasurementStarted(EventArgs eventArgs)
        {
            m_SafetyValueEventSent = false;
            if (m_ContinuousMeasurementPeriod != Timeout.Infinite)
            {
                m_ContinuousMeasurementPeriodTimer.Change(m_ContinuousMeasurementPeriod + m_MeasurementCommandDelay, Timeout.Infinite);
            }
            m_IsMeasuring = true;

            ContinuousMeasurementStarted?.Invoke(this, eventArgs);
        }

        /// <summary>
        /// Called when [measurement result].
        /// </summary>
        private void OnMeasurementResult(double measurementValue, bool measurementValueOutOfLimits)
        {
            MeasurementResult?.Invoke(this, new MeasurementResultEventArgs(measurementValue, measurementValueOutOfLimits, MeasurementResultShouldBeStored()));
        }

        private void OnSafetyValueTriggered(double measurementValue, int ioStatus)
        {
            SafetyValueTriggered?.Invoke(this, new SafetyValueTriggeredEventArgs(measurementValue, ioStatus));
        }

        private void OnDeviceUnPaired(string address)
        {
            DeviceUnPaired?.Invoke(this, new DeviceUnPairedEventArgs(address));
        }
        public void UnPairDevice(string address)
        {
            CommunicationControl.UnpairDevices(address, UnPairCallback, DeviceUnPairFinishedCallback);
        }

        public void UnPairAllDevices()
        {
            CommunicationControl.Initialize();
            CommunicationControl.UnpairDevices(null, UnPairCallback, DeviceUnPairFinishedCallback);
        }

        public void SetUiResponse(UiResponse response)
        {
            if (m_UiResponseReceived.WaitOne(0))
            {
                m_Initialization.SetUiResponse(response);
            }
            else
            {
                m_UiResponse = response;
                m_UiResponseReceived.Set();
            }
        }

        private void UnPairCallback(bool result, int errorCode, string message)
        {
            CommunicationControl.Initialize();
            OnDeviceUnPaired(message);
        }


        private void DeviceUnPairFinishedCallback()
        {
            DeviceUnPairFinished?.Invoke(this, EventArgs.Empty);
        }
        #endregion Event Handlers
    }
}
