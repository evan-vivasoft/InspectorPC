/*
//===============================================================================
// Copyright Kamstrup
// All rights reserved.
//===============================================================================
*/

using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Inspector.BusinessLogic.Interfaces;
using Inspector.BusinessLogic.Interfaces.Events;
using Inspector.Infra.Ioc;
using Inspector.Model;
using Inspector.UI.Initialization.Commands;
using Inspector.UI.Initialization.Model;

namespace Inspector.UI.Initialization.ViewModels
{
    /// <summary>
    /// InitializationViewModel
    /// </summary>
    public class InitializationViewModel : ViewModelBase, IDisposable
    {
        #region Class Members
        private Dispatcher m_Dispatcher;
        private bool m_Disposed = false;
        private ObservableCollection<InitializationStep> m_InitializationSteps;
        private RelayCommand m_StartInitializationCommand;
        private RelayCommand m_AbortInitializationCommand;
        private RelayCommand m_UnPairAllDevicesCommand;
        private string m_Message;

        private IInitializationActivityControl m_InitializationActivityControl;
        #endregion Class Members

        #region Properties
        /// <summary>
        /// Gets the initialization steps.
        /// </summary>
        public ObservableCollection<InitializationStep> InitializationSteps
        {
            get
            {
                return m_InitializationSteps;
            }
            private set
            {
                m_InitializationSteps = value;
                OnPropertyChanged("InitializationSteps");
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
        #endregion Properties

        #region Commands
        /// <summary>
        /// Gets the start initialization command.
        /// </summary>
        public ICommand StartInitializationCommand
        {
            get
            {
                return m_StartInitializationCommand;
            }
        }

        /// <summary>
        /// Gets the abort initialization command.
        /// </summary>
        public ICommand AbortInitializationCommand
        {
            get
            {
                return m_AbortInitializationCommand;
            }
        }
        public ICommand UnPairAllDevicesCommand
        {
            get
            {
                return m_UnPairAllDevicesCommand;
            }
        }

        #endregion Commands

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="InitializationViewModel"/> class.
        /// </summary>
        public InitializationViewModel()
        {
            m_Dispatcher = Dispatcher.CurrentDispatcher;
            InitializationSteps = new ObservableCollection<InitializationStep>();
            m_StartInitializationCommand = new RelayCommand(HandleStartInitializationCommand);
            m_AbortInitializationCommand = new RelayCommand(HandleAbortInitializationCommand);
            m_UnPairAllDevicesCommand = new RelayCommand(HandleUnPairAllDevicesCommand);

            AttachEvents();

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
                    if (m_InitializationActivityControl != null)
                    {
                        m_InitializationActivityControl.Dispose();
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
            InitializationActivityControl.InitializationStepStarted += new EventHandler(InitializationActivityControl_InitializationStepStarted);
            InitializationActivityControl.InitializationStepFinished += new EventHandler(InitializationActivityControl_InitializationStepFinished);
            InitializationActivityControl.InitializationFinished += new EventHandler(InitializationActivityControl_InitializationFinished);
            InitializationActivityControl.DeviceUnPairFinished += InitializationActivityControl_DeviceUnPairFinished;
            InitializationActivityControl.UiRequest += InitializationActivityControl_UiRequest;
        }





        /// <summary>
        /// Detaches the events.
        /// </summary>
        private void DetachEvents()
        {
            InitializationActivityControl.InitializationStepStarted -= new EventHandler(InitializationActivityControl_InitializationStepStarted);
            InitializationActivityControl.InitializationStepFinished -= new EventHandler(InitializationActivityControl_InitializationStepFinished);
            InitializationActivityControl.InitializationFinished -= new EventHandler(InitializationActivityControl_InitializationFinished);
            InitializationActivityControl.DeviceUnPairFinished -= InitializationActivityControl_DeviceUnPairFinished;
        }
        #endregion Event Handling

        #region Command Handlers
        /// <summary>
        /// Handles the start initialization command.
        /// </summary>
        /// <param name="param">The param.</param>
        private void HandleStartInitializationCommand(object param)
        {
            if (!m_StartInitializationCommand.IsExecutable)
            {
                throw new InvalidOperationException("Command may not be executed.");
            }

            StartInitialization();
        }

        /// <summary>
        /// Handles the start initialization command.
        /// </summary>
        /// <param name="param">The param.</param>
        private void HandleAbortInitializationCommand(object param)
        {
            if (!m_AbortInitializationCommand.IsExecutable)
            {
                throw new InvalidOperationException("Command may not be executed.");
            }

            AbortInitialization();
        }

        private void HandleUnPairAllDevicesCommand(object obj)
        {
            m_InitializationActivityControl?.UnPairAllDevices();
        }

        private void AbortInitialization()
        {
            m_InitializationActivityControl?.Abort();
        }

        /// <summary>
        /// Starts the initialization.
        /// </summary>
        private void StartInitialization()
        {
            InitializationSteps.Clear();


            InitializationActivityControl.ExecuteInitialization();
        }


        /// <summary>
        /// Handles the InitializationFinished event of the InitializationActivityControl control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="eventArgs">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void InitializationActivityControl_InitializationFinished(object sender, EventArgs eventArgs)
        {
            FinishInitializationEventArgs finishEventArgs = eventArgs as FinishInitializationEventArgs;
            Message = String.Format("{0} {1}", finishEventArgs.Result.ToString(), finishEventArgs.ErrorCode.ToString());
            DetachEvents();
        }

        private void InitializationActivityControl_DeviceUnPairFinished(object sender, EventArgs e)
        {
            InitializationSteps.Add(new InitializationStep("device unpair ", "finished"));
        }

        private void InitializationActivityControl_UiRequest(object sender, UiRequestEventArgs e)
        {
            //InitializationSteps.Add(new InitializationStep("Do You Want To Continue ?! ", "Answer me!"));
            var result = MessageBox.Show(e.RequestMessage, "CAPTION!!!", MessageBoxButton.YesNo,
                MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                m_InitializationActivityControl.SetUiResponse(UiResponse.Yes);
            }
            else
            {
                m_InitializationActivityControl.SetUiResponse(UiResponse.No);
            }
        }

        /// <summary>
        /// Handles the InitializationStepFinished event of the InitializationActivityControl control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="eventArgs">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void InitializationActivityControl_InitializationStepFinished(object sender, EventArgs eventArgs)
        {
            FinishInitializationStepEventArgs finishEventArgs = eventArgs as FinishInitializationStepEventArgs;
            m_Dispatcher.BeginInvoke(new Action<string, InitializationStepResult, string, int>(UpdateInitializationStep), finishEventArgs.StepId, finishEventArgs.Result, finishEventArgs.Message, finishEventArgs.ErrorCode);
        }

        /// <summary>
        /// Handles the InitializationStepStarted event of the InitializationActivityControl control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="eventArgs">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void InitializationActivityControl_InitializationStepStarted(object sender, EventArgs eventArgs)
        {
            StartInitializationStepEventArgs startEventArgs = eventArgs as StartInitializationStepEventArgs;
            m_Dispatcher.BeginInvoke(new Action<string>(AddInitializationStep), startEventArgs.StepId);
        }
        #endregion Command Handlers

        #region Functions
        /// <summary>
        /// Adds the initialization step.
        /// </summary>
        /// <param name="id">The id.</param>
        private void AddInitializationStep(string id)
        {
            InitializationStep init = new InitializationStep(id, id); // second ID localization has not been done here.
            InitializationSteps.Add(init);
        }

        /// <summary>
        /// Updates the initialization step.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="result">The result.</param>
        /// <param name="message">The message.</param>
        /// <param name="errorCode">The error code.</param>
        private void UpdateInitializationStep(string id, InitializationStepResult result, string message, int errorCode)
        {
            InitializationStep stepToUpdate = InitializationSteps.LastOrDefault(item => item.StepId == id);
            if (stepToUpdate != null)
            {
                stepToUpdate.StepResult = result;
                stepToUpdate.StepMessage = message;
                stepToUpdate.StepErrorCode = errorCode;
            }
        }
        #endregion Functions
    }
}
