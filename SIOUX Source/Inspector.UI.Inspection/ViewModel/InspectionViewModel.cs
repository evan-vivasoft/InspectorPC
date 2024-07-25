/*
//===============================================================================
// Copyright Kamstrup
// All rights reserved.
//===============================================================================
*/

using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Inspector.BusinessLogic.Data.Configuration.Interfaces.Managers;
using Inspector.BusinessLogic.Data.Reporting.Interfaces;
using Inspector.BusinessLogic.Interfaces;
using Inspector.BusinessLogic.Interfaces.Events;
using Inspector.Infra.Ioc;
using Inspector.Model;
using Inspector.Model.InspectionMeasurement;
using Inspector.Model.InspectionProcedure;
using Inspector.Model.InspectionStepResult;
using Inspector.UI.Inspection.Commands;
using Inspector.UI.Inspection.Model;

namespace Inspector.UI.Inspection.ViewModels
{
    /// <summary>
    /// InspectionViewModel
    /// </summary>
    public class InspectionViewModel : ViewModelBase, IDisposable
    {
        #region Class Members
        private Dispatcher m_Dispatcher;
        private bool m_Disposed = false;
        private RelayCommand m_StartInspectionCommand;
        private RelayCommand m_StartPartialInspectionCommand;
        private RelayCommand m_StartContinuousMeasurementCommand;
        private RelayCommand m_StopContinuousMeasurementCommand;
        private RelayCommand m_StartContinuousMeasurement5x70Command;
        private RelayCommand m_AbortMeasurementCommand;
        private RelayCommand m_AbortInitCommand;
        private RelayCommand m_GenerateProblemCommand;
        private RelayCommand m_UnpairDeviceCommand;
        private RelayCommand m_UnpairAllDevicesCommand;
        private string m_Message;
        private string m_StepMessage;
        private string m_MeasurementValue;
        private string m_MeasurementValueOutOfLimits;
        private string m_ContinuousMeasurementStatus;

        private bool m_NextClicked = false;

        private ObservableCollection<InspectionStepModel> m_InspectionSteps;

        private IInspectionActivityControl m_InspectionActivityControl;
        private IInspectionInformationManager m_InspectionInformationManager;
        private static object m_LockAttaching = new object();
        private volatile bool m_IsAttached = false;

        private bool m_IsManualMeasurementStop = false;

        #endregion Class Members

        #region Properties
        /// <summary>
        /// Gets the inspection steps.
        /// </summary>
        public ObservableCollection<InspectionStepModel> InspectionSteps
        {
            get
            {
                if (m_InspectionSteps == null)
                {
                    m_InspectionSteps = new ObservableCollection<InspectionStepModel>();
                }
                return m_InspectionSteps;
            }
            private set
            {
                m_InspectionSteps = value;
                OnPropertyChanged("InspectionSteps");
            }
        }

        /// <summary>
        /// Gets the message.
        /// </summary>
        public string Message
        {
            get
            {
                return m_Message;
            }
            private set
            {
                m_Message = value;
                OnPropertyChanged("Message");
            }
        }

        /// <summary>
        /// Gets the measurement value.
        /// </summary>
        public string MeasurementValue
        {
            get
            {
                return m_MeasurementValue;
            }
            private set
            {
                m_MeasurementValue = value;
                OnPropertyChanged("MeasurementValue");
            }
        }

        /// <summary>
        /// Gets the measurement value out of limits.
        /// </summary>
        public string MeasurementValueOutOfLimits
        {
            get
            {
                return m_MeasurementValueOutOfLimits;
            }
            private set
            {
                m_MeasurementValueOutOfLimits = value;
                OnPropertyChanged("MeasurementValueOutOfLimits");
            }
        }


        public string ContinuousMeasurementStatus
        {
            get
            {
                return m_ContinuousMeasurementStatus;
            }
            set
            {
                m_ContinuousMeasurementStatus = value;
                OnPropertyChanged("ContinuousMeasurementStatus");
            }
        }
        /// <summary>
        /// Gets or sets the step message.
        /// </summary>
        /// <value>The step message.</value>
        public string StepMessage
        {
            get
            {
                return m_StepMessage;
            }
            private set
            {
                m_StepMessage = value;
                OnPropertyChanged("StepMessage");
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
                    //AttachEvents();
                }
                return m_InspectionActivityControl;
            }
            set
            {
                m_InspectionActivityControl = value;
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
                if (m_InspectionInformationManager == null)
                {
                    m_InspectionInformationManager = ContextRegistry.Context.Resolve<IInspectionInformationManager>();
                }
                return m_InspectionInformationManager;
            }
            set
            {
                m_InspectionInformationManager = value;
            }
        }

        private IInspectionResultReader m_InspectionResultReader;

        /// <summary>
        /// Gets or sets the inspection information manager.
        /// </summary>
        /// <value>
        /// The inspection information manager.
        /// </value>
        public IInspectionResultReader InspectionResultReader
        {
            get
            {
                if (m_InspectionResultReader == null)
                {
                    m_InspectionResultReader = ContextRegistry.Context.Resolve<IInspectionResultReader>();
                }
                return m_InspectionResultReader;
            }
            set
            {
                m_InspectionResultReader = value;
            }
        }
        #endregion Properties

        #region Commands
        /// <summary>
        /// Gets the start initialization command.
        /// </summary>
        public ICommand StartInspectionCommand
        {
            get
            {
                return m_StartInspectionCommand;
            }
        }

        /// <summary>
        /// Gets the start partial initialization command.
        /// </summary>
        public ICommand StartPartialInspectionCommand
        {
            get
            {
                return m_StartPartialInspectionCommand;
            }
        }

        /// <summary>
        /// Gets the start continuous measurement command.
        /// </summary>
        /// <value>The start continuous measurement command.</value>
        public ICommand StartContinuousMeasurementCommand
        {
            get
            {
                return m_StartContinuousMeasurementCommand;
            }
        }

        /// <summary>
        /// Gets the stop continuous measurement command.
        /// </summary>
        /// <value>The stop continuous measurement command.</value>
        public ICommand StopContinuousMeasurementCommand
        {
            get
            {
                return m_StopContinuousMeasurementCommand;
            }
        }

        /// <summary>
        /// Gets the start continuous measurement command.
        /// </summary>
        /// <value>The start continuous measurement command.</value>
        public ICommand StartContinuousMeasurement5x70Command
        {
            get
            {
                return m_StartContinuousMeasurement5x70Command;
            }
        }

        /// <summary>
        /// Gets the abort init command.
        /// </summary>
        public ICommand AbortInitCommand
        {
            get
            {
                return m_AbortInitCommand;
            }
        }

        /// <summary>
        /// Gets the abort measurement command.
        /// </summary>
        public ICommand AbortMeasurementCommand
        {
            get
            {
                return m_AbortMeasurementCommand;
            }
        }

        /// <summary>
        /// Gets the Generate Problem command.
        /// </summary>
        public ICommand GenerateProblemCommand
        {
            get
            {
                return m_GenerateProblemCommand;
            }
        }

        public ICommand UnpairAllDevicesCommand
        {
            get
            {
                return m_UnpairAllDevicesCommand;
            }
        }

        public ICommand UnpairDeviceCommand
        {
            get
            {
                return m_UnpairDeviceCommand;
            }
        }
        #endregion Commands

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="InspectionViewModel"/> class.
        /// </summary>
        public InspectionViewModel()
        {
            m_Dispatcher = Dispatcher.CurrentDispatcher;
            m_StartInspectionCommand = new RelayCommand(HandleStartInspectionCommand);
            m_StartPartialInspectionCommand = new RelayCommand(HandleStartPartialInspectionCommand);
            m_StartContinuousMeasurementCommand = new RelayCommand(HandleStartContinuousMeasurementCommand);
            m_StopContinuousMeasurementCommand = new RelayCommand(HandleStopContinuousMeasurementCommand);
            m_StartContinuousMeasurement5x70Command = new RelayCommand(HandleStartContinuousMeasurement5x70Command);
            m_AbortMeasurementCommand = new RelayCommand(HandleAbortMeasurementCommand);
            m_AbortInitCommand = new RelayCommand(HandleAbortInitCommand);
            m_GenerateProblemCommand = new RelayCommand(HandleGenerateProblemCommand);
            m_UnpairAllDevicesCommand = new RelayCommand(HandleUnPairAllDevicesCommand);
            m_UnpairDeviceCommand = new RelayCommand(HandleUnPairDeviceCommand);
        }

        private void HandleUnPairAllDevicesCommand(object obj)
        {
            InspectionActivityControl.UnPairAllDevices();
        }

        private void HandleUnPairDeviceCommand(object obj)
        {
            //InspectionActivityControl.UnPairDevice("(00:80:98:C4:21:13)");
            InspectionActivityControl.Retry();
        }
        #endregion

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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        protected virtual void Dispose(bool disposing)
        {
            if (!m_Disposed)
            {
                if (disposing)
                {
                    if (m_InspectionActivityControl != null)
                    {
                        m_InspectionActivityControl.Dispose();
                    }
                }
            }

            m_Disposed = true;
        }
        #endregion IDisposable

        #region Event Handling
        /// <summary>
        /// Attaches the events.
        /// </summary>
        private void AttachEvents()
        {
            lock (m_LockAttaching)
            {
                Console.WriteLine("lock attaching");

                if (m_IsAttached) return;
                InspectionActivityControl.MeasurementsCompleted += InspectionActivityControl_MeasurementsCompleted;
                InspectionActivityControl.MeasurementsReceived += InspectionActivityControl_MeasurementsReceived;
                InspectionActivityControl.InitializationStepStarted += InspectionActivityControl_InitializationStepStarted;
                InspectionActivityControl.InitializationStepFinished += InspectionActivityControl_InitializationStepFinished;
                InspectionActivityControl.InitializationFinished += InspectionActivityControl_InitializationFinished;
                InspectionActivityControl.ExecuteInspectionStep += InspectionActivityControl_ExecuteInspectionStep;
                InspectionActivityControl.InspectionFinished += InspectionActivityControl_InspectionFinished;
                InspectionActivityControl.InspectionError += InspectionActivityControl_InspectionError;
                InspectionActivityControl.MeasurementResult += InspectionActivityControl_MeasurementResult;
                InspectionActivityControl.ContinuousMeasurementStarted += InspectionActivityControl_ContinuousMeasurementStarted;
                InspectionActivityControl.SafetyValueTriggered += InspectionActivityControl_SafetyValueTriggered;
                InspectionActivityControl.DeviceUnPaired += InspectionActivityControl_DeviceUnPaired;
                InspectionActivityControl.DeviceUnPairFinished += InspectionActivityControl_DeviceUnPairFinished;
                InspectionActivityControl.UiRequest += InspectionActivityControlOnUiRequest;

                m_IsAttached = true;
                Console.WriteLine("unlock attaching");

            }
        }

        private void InspectionActivityControl_DeviceUnPairFinished(object sender, EventArgs e)
        {
            InspectionSteps?.Add(new InspectionStepModel("all devices unpaired", "OK"));

        }

        private void InspectionActivityControl_DeviceUnPaired(object sender, EventArgs e)
        {
            InspectionSteps?.Add(new InspectionStepModel("device unpaired", (e as DeviceUnPairedEventArgs).Address));
        }

        private void InspectionActivityControl_SafetyValueTriggered(object sender, EventArgs e)
        {
            var safetyTriggeredEventArgs = e as SafetyValueTriggeredEventArgs;

            System.Diagnostics.Debug.WriteLine("THE SAFETY HAS BEEN TRIGGERED!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
            InspectionActivityControl.StopContinuousMeasurement();
        }

        private void InspectionActivityControl_ContinuousMeasurementStarted(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Viewmodel received OnContinuousMeasurementStarted");

            ContinuousMeasurementStatus = "Measurement started";
        }

        private void InspectionActivityControl_MeasurementResult(object sender, EventArgs e)
        {
            var measurementResultEventArgs = e as MeasurementResultEventArgs;

            if (measurementResultEventArgs != null)
            {
                MeasurementValue = measurementResultEventArgs.MeasurementValue.ToString(CultureInfo.InvariantCulture);
                MeasurementValueOutOfLimits = measurementResultEventArgs.MeasurementValueOutOfLimits.ToString();
            }

            ContinuousMeasurementStatus = "Measurement stopped";
        }

        /// <summary>
        /// Handles the InspectionError event of the InspectionActivityControl control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void InspectionActivityControl_InspectionError(object sender, EventArgs e)
        {
            var eventArgs = e as InspectionErrorEventArgs;

            if (eventArgs == null) return;

            System.Diagnostics.Debug.WriteLine("inspection error: " + eventArgs.ErrorCode);

            string errorMessage = $"Retry for error '{eventArgs.ErrorCode}'?";

            if (MessageBox.Show("Error", errorMessage, MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.Yes)
            {
                InspectionActivityControl.Retry();
            }
            else
            {
                InspectionActivityControl.Abort();
            }
        }

        /// <summary>
        /// Handles the MeasurementsReceived event of the InspectionActivityControl control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void InspectionActivityControl_MeasurementsReceived(object sender, EventArgs e)
        {
            var eventArgs = e as MeasurementEventArgs;

            if (eventArgs == null) return;

            foreach (var m in eventArgs.Measurements)
            {
                string values = $"Values | '{m.Measurement}' | '{m.Minimum}' | '{m.Maximum}' | '{m.Average}' | ";

                m_Dispatcher.BeginInvoke(new Action<string, ScriptCommand5XMeasurement>((vals, ms) => InspectionSteps.Add(new InspectionStepModel(vals, ms.SequenceNumber.ToString()))), values, m);
            }
        }

        /// <summary>
        /// Handles the MeasurementsCompleted event of the InspectionActivityControl control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void InspectionActivityControl_MeasurementsCompleted(object sender, EventArgs e)
        {
            var eventArgs = e as ExecuteInspectionStepEventArgs;

            var model = new InspectionStepModel("Measurement fininished", "DONE");

            m_Dispatcher.BeginInvoke(new Action<InspectionStepModel>((inspectionModel) => InspectionSteps.Add(inspectionModel)), model);

            if (eventArgs != null)
            {
                InspectionActivityControl.InspectionStepComplete(new InspectionStepResultEmpty(eventArgs.ScriptCommand.SequenceNumber));
            }
        }

        /// <summary>
        /// Detaches the events.
        /// </summary>
        private void DetachEvents()
        {
            lock (m_LockAttaching)
            {
                if (!m_IsAttached) return;

                InspectionActivityControl.MeasurementsCompleted -= InspectionActivityControl_MeasurementsCompleted;
                InspectionActivityControl.MeasurementsReceived -= InspectionActivityControl_MeasurementsReceived;
                InspectionActivityControl.InitializationStepStarted -= InspectionActivityControl_InitializationStepStarted;
                InspectionActivityControl.InitializationStepFinished -= InspectionActivityControl_InitializationStepFinished;
                InspectionActivityControl.InitializationFinished -= InspectionActivityControl_InitializationFinished;
                InspectionActivityControl.ExecuteInspectionStep -= InspectionActivityControl_ExecuteInspectionStep;
                InspectionActivityControl.InspectionFinished -= InspectionActivityControl_InspectionFinished;
                InspectionActivityControl.InspectionError -= InspectionActivityControl_InspectionError;
                InspectionActivityControl.MeasurementResult -= InspectionActivityControl_MeasurementResult;
                InspectionActivityControl.ContinuousMeasurementStarted -= InspectionActivityControl_ContinuousMeasurementStarted;
                InspectionActivityControl.SafetyValueTriggered -= InspectionActivityControl_SafetyValueTriggered;
                InspectionActivityControl.DeviceUnPaired -= InspectionActivityControl_DeviceUnPaired;
                InspectionActivityControl.DeviceUnPairFinished -= InspectionActivityControl_DeviceUnPairFinished;
                InspectionActivityControl.UiRequest -= InspectionActivityControlOnUiRequest;
                m_IsAttached = false;
            }
        }

        private void InspectionActivityControlOnUiRequest(object sender, UiRequestEventArgs uiRequestEventArgs)
        {
            var result = MessageBox.Show(uiRequestEventArgs.RequestMessage, "caption", MessageBoxButton.YesNoCancel,
                MessageBoxImage.Question);

            switch (result)
            {
                   case MessageBoxResult.Cancel:
                    m_InspectionActivityControl.SetUiResponse(UiResponse.Recheck);
                       break;
                    case MessageBoxResult.Yes:
                    m_InspectionActivityControl.SetUiResponse(UiResponse.Yes);
                        break;
                    case MessageBoxResult.No:
                    m_InspectionActivityControl.SetUiResponse(UiResponse.No);
                        break;
            }
        }

        #endregion Event Handling

        #region Command Handlers
        /// <summary>
        /// Handles the start inspection command.
        /// </summary>
        /// <param name="param">The param.</param>
        private void HandleStartInspectionCommand(object param)
        {
            if (!m_StartInspectionCommand.IsExecutable)
            {
                throw new InvalidOperationException("Command may not be executed.");
            }

            StartInspection();
        }

        /// <summary>
        /// Starts the initialization.
        /// </summary>
        private void StartInspection()
        {
            if (!m_IsAttached)
            {
                InspectionSteps = new ObservableCollection<InspectionStepModel>();
            }

            AttachEvents();
            
            //bit stupid
            var prsName = "EWA Bonhöfferstraße, Демонстрационный стенд B-249"; //"5282720/AS 'T ANKER";//
            var gclName = "Arbeitsschiene Schiene 1"; //"1044435 1";//
            if (!InspectionActivityControl.ExecuteInspection(prsName,gclName))
            {
                MessageBox.Show("It is not allowed to run multiple inspections at the same time");
            }
        }

        /// <summary>
        /// Handles the InspectionFinished event of the InspectionActivityControl control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void InspectionActivityControl_InspectionFinished(object sender, EventArgs e)
        {
            var finishEventArgs = e as InspectionFinishedEventArgs;

            if (finishEventArgs != null)
            {
                System.Diagnostics.Debug.WriteLine("inspection error: " + finishEventArgs.ErrorCode);

                Message = finishEventArgs.ErrorCode.ToString();
            }

            DetachEvents();
        }

        /// <summary>
        /// Handles the ExecuteInspectionStep event of the InspectionActivityControl control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void InspectionActivityControl_ExecuteInspectionStep(object sender, EventArgs e)
        {
            var stepEventArgs = e as ExecuteInspectionStepEventArgs;

            if (stepEventArgs != null)
            {
                m_Dispatcher.BeginInvoke(new Action<ScriptCommandBase, int, int>(ExecuteInspectionCommand), stepEventArgs.ScriptCommand, stepEventArgs.CurrentInspectionStep, stepEventArgs.TotalInspectionSteps);
            }
        }

        /// <summary>
        /// Handles the start inspection command.
        /// </summary>
        /// <param name="param">The param.</param>
        private void HandleStartPartialInspectionCommand(object param)
        {
            if (!m_StartPartialInspectionCommand.IsExecutable)
            {
                throw new InvalidOperationException("Command may not be executed.");
            }

            StartPartialInspection();
        }

        /// <summary>
        /// Starts the initialization.
        /// </summary>
        private void StartPartialInspection()
        {
            if (!m_IsAttached)
            {
                InspectionSteps = new ObservableCollection<InspectionStepModel>();
            }

            AttachEvents();

            var sectionSelection = InspectionInformationManager.LookupInspectionProcedureSections("SC5XAND70 Inspection");
            sectionSelection.SectionSelectionEntities[0].IsSelected = true;
            sectionSelection.SectionSelectionEntities[1].IsSelected = true;
            sectionSelection.SectionSelectionEntities[14].IsSelected = true;
            sectionSelection.SectionSelectionEntities[40].IsSelected = true;

            if (!InspectionActivityControl.ExecutePartialInspection(sectionSelection, "APELDOORN ORDERMOLENWEG 1 | 5 007 230.1", "5 007 230.1.1 OS LS"))
            {
                MessageBox.Show("It is not allowed to run multiple inspections at the same time");
            }
        }

        /// <summary>
        /// Handles the start continuous measurement command.
        /// </summary>
        /// <param name="param">The param.</param>
        private void HandleStartContinuousMeasurementCommand(object param)
        {
            if (!m_StartContinuousMeasurementCommand.IsExecutable)
            {
                throw new InvalidOperationException("Command may not be executed.");
            }

            StartContinuousMeasurement();
        }

        /// <summary>
        /// Handles the stop continuous measurement command.
        /// </summary>
        /// <param name="param">The param.</param>
        private void HandleStopContinuousMeasurementCommand(object param)
        {
            if (!m_StopContinuousMeasurementCommand.IsExecutable)
            {
                throw new InvalidOperationException("Command may not be executed.");
            }

            if (m_IsManualMeasurementStop)
            {
                InspectionActivityControl.StartContinuousMeasurement();
                m_IsManualMeasurementStop = false;
            }
            else
            {
                InspectionActivityControl.StopContinuousMeasurement();
                m_IsManualMeasurementStop = true;
            }
        }

        /// <summary>
        /// Starts the continuous measurement.
        /// </summary>
        private void StartContinuousMeasurement()
        {
            if (!m_IsAttached)
            {
                InspectionSteps = new ObservableCollection<InspectionStepModel>();
            }

            AttachEvents();

            if (!InspectionActivityControl.ExecuteInspection("Kamstrup AS PN16 (ohne SBV)", "EWA Bonhöfferstraße, Демонстрационный стенд B-249", "Arbeitsschiene Schiene 1"))
            {
                MessageBox.Show("It is not allowed to run multiple inspections at the same time");
            }
        }

        /// <summary>
        /// Handles the start continuous measurement5x70 command.
        /// </summary>
        /// <param name="param">The param.</param>
        private void HandleStartContinuousMeasurement5x70Command(object param)
        {
            if (!m_StartContinuousMeasurement5x70Command.IsExecutable)
            {
                throw new InvalidOperationException("Command may not be executed.");
            }

            StartContinuousMeasurement5x70();
        }

        /// <summary>
        /// Starts the continuous measurement5x70.
        /// </summary>
        private void StartContinuousMeasurement5x70()
        {
            if (!m_IsAttached)
            {
                InspectionSteps = new ObservableCollection<InspectionStepModel>();
            }

            AttachEvents();

            if (!InspectionActivityControl.ExecuteInspection("SC5XAND70 Inspection", "APELDOORN ORDERMOLENWEG 1 | 5 007 230.1", "5 007 230.1.1 OS LS"))
            {

                System.Windows.MessageBox.Show("It is not allowed to run multiple inspections at the same time");
            }
        }

        /// <summary>
        /// Handles the abort init command.
        /// </summary>
        /// <param name="param">The param.</param>
        private void HandleAbortInitCommand(object param)
        {
            if (!m_AbortInitCommand.IsExecutable)
            {
                throw new InvalidOperationException("Command may not be executed.");
            }

            AbortInitialization();
        }


        /// <summary>
        /// Handles the abort measurement command.
        /// </summary>
        /// <param name="param">The param.</param>
        private void HandleAbortMeasurementCommand(object param)
        {
            if (!m_AbortMeasurementCommand.IsExecutable)
            {
                throw new InvalidOperationException("Command may not be executed.");
            }

            AbortMeasurement();
        }

        /// <summary>
        /// Aborts the measurement.
        /// </summary>
        private void AbortInitialization()
        {
            InspectionActivityControl.Abort();
        }

        /// <summary>
        /// Aborts the measurement.
        /// </summary>
        private void AbortMeasurement()
        {
            InspectionActivityControl.Abort();
        }

        /// <summary>
        /// Generates an exception that should be handled by the catch all
        /// </summary>
        private void HandleGenerateProblemCommand(object param)
        {
            var prsName = "Aesch Belreba Kunde"; //"5282720/AS 'T ANKER";//
            var gclName = "Betriebsschiene";// "Arbeitsschiene Schiene 1"; //"1044435 1";//
            //get report stuff

            var allReports = InspectionResultReader.LookupAllResults(prsName, gclName);

            //var lastReportResult = InspectionResultReader.LookupLastReportResult(prsName, gclName, string.Empty, string.Empty);
            //var secondToLastReportResult = InspectionResultReader.LookupPreviousToLastReportResult(prsName, gclName,
            //    string.Empty, String.Empty);
            //var lastResult = InspectionResultReader.LookupLastResult(prsName, gclName);
            //var secondToLastResult = InspectionResultReader.LookupPreviousToLastResult(prsName, gclName);
            StringBuilder info = new StringBuilder();
            foreach (var report in allReports)
            {
                try
                {
                    info.AppendLine($"ReportResult: start: {report.DateTimeStamp.StartDate} {report.DateTimeStamp.StartTime}");
                    foreach (var result in report.Results)
                    {
                        info.AppendLine($"\t {result.Text} {result.Time} {result.MeasurePointDescription} {result.ObjectNameDescription}");
                    }
                    info.AppendLine($"ReportResult: end: {report.DateTimeStamp.StartDate} {report.DateTimeStamp.EndTime}");
                    info.AppendLine();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
            //try
            //{
            //    info.AppendLine("SecondToLastReportResult:");
            //    info.AppendLine("MPD: " + secondToLastReportResult.MeasurePointDescription);
            //    info.AppendLine("OND: " + secondToLastReportResult.ObjectNameDescription);
            //}
            //catch (Exception e)
            //{
            //    Console.WriteLine(e);
            //}

            //try
            //{
            //    info.AppendLine("lastResult:");
            //    info.AppendLine("MPD: " + lastResult.Results.First().MeasurePointDescription);
            //    info.AppendLine("OND: " + lastResult.Results.First().ObjectNameDescription);
            //}
            //catch (Exception e)
            //{
            //    Console.WriteLine(e);
            //}

            //try
            //{
            //    info.AppendLine("secondToLastResult:");
            //    info.AppendLine("MPD: " + secondToLastResult.Results.First().MeasurePointDescription);
            //    info.AppendLine("OND: " + secondToLastResult.Results.First().ObjectNameDescription);
            //}
            //catch (Exception e)
            //{
            //    Console.WriteLine(e);
            //}

            MessageBox.Show(info.ToString());
            //throw new Exception("generated problem");
        }
        #endregion Command Handlers

        #region Functions
        /// <summary>
        /// Adds the initialization step.
        /// </summary>
        /// <param name="id">The id.</param>
        private void ExecuteInspectionCommand(ScriptCommandBase scriptCommand, int currentInspectionStep, int totalInspectionSteps)
        {
            StepMessage = String.Format("{0} / {1} inspection procedure steps", currentInspectionStep, totalInspectionSteps);
            InspectionSteps.Add(new InspectionStepModel(scriptCommand.GetType().Name, scriptCommand.SequenceNumber.ToString()));

            // For the scriptcommands that expect user input we insert static test data from the code instead of the UI
            // This is only appropriate for this testtool, in the final application this information must be extracted from the GUI
            if (ScriptCommandRequiresNoReply(scriptCommand))
            {
                InspectionActivityControl.InspectionStepComplete(new InspectionStepResultEmpty(scriptCommand.SequenceNumber));
            }
            else if (ScriptCommandRequiresTextReply(scriptCommand))
            {
                var readMeFromTheUiText = "Reply to scriptcommand";
                var inspectionStepResult = new InspectionStepResultText(scriptCommand.SequenceNumber, readMeFromTheUiText);

                InspectionActivityControl.InspectionStepComplete(inspectionStepResult);
            }
            else if (ScriptCommandRequiresSelectedAnswersAndOptionalRemark(scriptCommand))
            {
                var readMeFromTheUiRemark = "Remark of the script command";
                var readMeFromTheUiSelection1 = "Selection 1";
                var readMeFromTheUiSelection2 = "Selection 3";
                var readMeFromTheUiSelection3 = "Selection 3";

                var inspectionStepResult = new InspectionStepResultSelections(readMeFromTheUiRemark, scriptCommand.SequenceNumber, readMeFromTheUiSelection1, readMeFromTheUiSelection2, readMeFromTheUiSelection3);

                InspectionActivityControl.InspectionStepComplete(inspectionStepResult);
            }
            else if (scriptCommand is ScriptCommand5X)
            {
                // do nothing, measurement completed event signals the completion of the measurements and includes the corresponding scriptcommand in its event arguments.
            }
        }

        /// <summary>
        /// Scripts the command requires selected answers and optional remark.
        /// </summary>
        /// <param name="scriptCommand">The script command.</param>
        /// <returns></returns>
        private static bool ScriptCommandRequiresSelectedAnswersAndOptionalRemark(ScriptCommandBase scriptCommand)
        {
            return scriptCommand is ScriptCommand41;
        }

        /// <summary>
        /// Checks if the scriptcommand is of the type that requires a text input of the user
        /// </summary>
        /// <param name="scriptCommand">The script command.</param>
        /// <returns></returns>
        private static bool ScriptCommandRequiresTextReply(ScriptCommandBase scriptCommand)
        {
            return scriptCommand is ScriptCommand4 || scriptCommand is ScriptCommand43;
        }

        /// <summary>
        /// Checks if the scriptcommand is of the type that requires no user input
        /// </summary>
        /// <param name="scriptCommand">The script command.</param>
        /// <returns>True is user input is expected, otherwise false.</returns>
        private static bool ScriptCommandRequiresNoReply(ScriptCommandBase scriptCommand)
        {
            return scriptCommand is ScriptCommand1 || scriptCommand is ScriptCommand2 || scriptCommand is ScriptCommand3 || scriptCommand is ScriptCommand42;
        }
        #endregion Functions

        /// <summary>
        /// Handles the InitializationFinished event of the InitializationActivityControl control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="eventArgs">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private static void InspectionActivityControl_InitializationFinished(object sender, EventArgs eventArgs)
        {
            var finishEventArgs = eventArgs as FinishInitializationEventArgs;

            if (finishEventArgs != null)
            {
                System.Diagnostics.Debug.WriteLine("Initialization finished with code: " + finishEventArgs.ErrorCode);
            }
        }

        /// <summary>
        /// Handles the InitializationStepFinished event of the InitializationActivityControl control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="eventArgs">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void InspectionActivityControl_InitializationStepFinished(object sender, EventArgs eventArgs)
        {
            var finishEventArgs = eventArgs as FinishInitializationStepEventArgs;

            if (finishEventArgs != null)
            {
                m_Dispatcher.BeginInvoke(new Action<string, InitializationStepResult, string>(UpdateInitializationStep), finishEventArgs.StepId, finishEventArgs.Result, finishEventArgs.Message);
            }
        }

        /// <summary>
        /// Handles the InitializationStepStarted event of the InitializationActivityControl control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="eventArgs">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void InspectionActivityControl_InitializationStepStarted(object sender, EventArgs eventArgs)
        {
            var startEventArgs = eventArgs as StartInitializationStepEventArgs;

            if (startEventArgs != null)
            {
                m_Dispatcher.BeginInvoke(new Action<string>(AddInitializationStep), startEventArgs.StepId);
            }
        }

        /// <summary>
        /// Adds the initialization step.
        /// </summary>
        /// <param name="id">The id.</param>
        private void AddInitializationStep(string id)
        {
            var init = new InspectionStepModel(id, "busy"); // second ID localization has not been done here.

            InspectionSteps.Add(init);
        }

        /// <summary>
        /// Updates the initialization step.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="result">The result.</param>
        /// <param name="message">The message.</param>
        private void UpdateInitializationStep(string id, InitializationStepResult result, string message)
        {
            var stepToUpdate = InspectionSteps[InspectionSteps.Count - 1];

            if (stepToUpdate == null) return;

            stepToUpdate.Result = result;
            stepToUpdate.SequenceNumber = message;
        }
    }
}
