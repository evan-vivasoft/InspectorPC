/*
//===============================================================================
// Copyright Kamstrup
// All rights reserved.
//===============================================================================
*/

using System;
using System.Collections.Generic;
using System.IO;
using Inspector.BusinessLogic.Interfaces;
using Inspector.BusinessLogic.Interfaces.Events;
using Inspector.Connection.Manager.Interfaces;
using Inspector.Infra;
using Inspector.Infra.Ioc;
using Inspector.Infra.Utils;
using Inspector.Model;
using Inspector.Model.BluetoothDongle;
using KAM.INSPECTOR.Infra;

namespace Inspector.BusinessLogic
{
    /// <summary>
    /// InitializationActivityControl
    /// </summary>
    public class InitializationActivityControl : IInitializationActivityControl
    {
        #region Class Members
        private Dictionary<string, string> m_ConnectionProperties = new Dictionary<string, string>();
        private Stack<DeviceCommand> m_CommandStack = new Stack<DeviceCommand>();
        private DeviceCommand m_CurrentCommand = DeviceCommand.None;
        private InitializationExecutorControl m_Initialization;
        private InitializationResult m_InitializationResult = InitializationResult.UNSET;

        private ICommunicationControl m_CommunicationControl;
        private bool m_Disposed = false;
        #endregion Class Members

        #region Constants
        private const string SETTING_BLUETOOTH_DONGLE_ADDRESS = "BluetoothDongleAddress";
        private const string SETTING_CATEGORY = "PLEXOR";
        private const string SETTING_RETURN_NO_VALUE = "<NO VALUE>";
        #endregion Constants

        #region Properties
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
                }
                return m_CommunicationControl;
            }
        }
        #endregion Properties

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="InitializationActivityControl"/> class.
        /// </summary>
        public InitializationActivityControl()
        {

        }

        #endregion Constructors

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
                    if (m_CommunicationControl != null)
                    {
                        m_CommunicationControl.Dispose();
                    }
                }
            }

            m_Disposed = true;
        }
        #endregion Dispose

        #region IActivityControl
        /// <summary>
        /// Executes the initialization activity in a thread.
        /// </summary>
        public void ExecuteInitialization()
        {
            System.Threading.Thread thread = new System.Threading.Thread(ExecuteInitializationThread);
            thread.Name = "ExecuteInitializationThread";
            thread.Start();
        }

        public void Abort()
        {
            m_Initialization.AbortInitialization();
        }

        /// <summary>
        /// Executes the activity
        /// </summary>
        private void ExecuteInitializationThread()
        {
            if (CommunicationControl.StartCommunication())
            {
                try
                {
                    m_ConnectionProperties = SettingsUtils.RetrieveConnectionProperties();
                    Initialize();
                    m_InitializationResult = InitializationResult.UNSET;

                    m_Initialization = new InitializationExecutorControl();
                    m_Initialization.InitializationStepStarted += new EventHandler(m_Initialization_InitializationStepStarted);
                    m_Initialization.InitializationStepFinished += new EventHandler(m_Initialization_InitializationStepFinished);
                    m_Initialization.InitializationFinished += new EventHandler(m_Initialization_InitializationFinished);
                    CreateCommandStack();
                    ExecuteNextCommand();
                }
                catch (FileNotFoundException)
                {
                    OnInitializationFinished(InitializationResult.ERROR, ErrorCodes.GENERAL_COULD_NOT_FIND_INSPECTOR_SETTINGS);
                }
                catch (ArgumentOutOfRangeException)
                {
                    OnInitializationFinished(InitializationResult.ERROR, ErrorCodes.GENERAL_COULD_NOT_READ_PROPERTY_INSPECTOR_SETTINGS);
                }
            }
            else
            {
                OnInitializationFinished(InitializationResult.ERROR, ErrorCodes.COMMUNICATIONCONTROL_ALREADY_CLAIMED);
            }
        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        private void Initialize()
        {
            CommunicationControl.Initialize();
        }

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
        #endregion IActivityControl

        #region Event Handlers
        /// <summary>
        /// Raises the <see cref="E:InitializationStepStarted"/> event.
        /// </summary>
        /// <param name="eventArgs">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void OnInitializationStepStarted(EventArgs eventArgs)
        {
            if (InitializationStepStarted != null)
            {
                StartInitializationStepEventArgs startEventArgs = eventArgs as StartInitializationStepEventArgs;
                InitializationStepStarted(this, startEventArgs);
            }
        }

        /// <summary>
        /// Called when [initialization step started].
        /// </summary>
        /// <param name="id">The id.</param>
        protected void OnInitializationStepStarted(DeviceCommand id)
        {
            if (InitializationStepStarted != null)
            {
                InitializationStepStarted(this, new StartInitializationStepEventArgs(id.ToString()));
            }
        }

        /// <summary>
        /// Called when [initialization step finished].
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="result">The result.</param>
        /// <param name="message">The message.</param>
        protected void OnInitializationStepFinished(EventArgs eventArgs)
        {
            if (InitializationStepFinished != null)
            {
                FinishInitializationStepEventArgs finishEventargs = eventArgs as FinishInitializationStepEventArgs;
                InitializationStepFinished(this, finishEventargs);
            }
        }

        /// <summary>
        /// Called when [initialization step finished].
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="result">The result.</param>
        /// <param name="message">The message.</param>
        /// <param name="errorCode">The error code.</param>
        /// <param name="manometer">The manometer.</param>
        protected void OnInitializationStepFinished(DeviceCommand id, InitializationStepResult result, string message, int errorCode, InitializationManometer manometer)
        {
            if (InitializationStepFinished != null)
            {
                InitializationStepFinished(this, new FinishInitializationStepEventArgs(id.ToString(), result, message, errorCode, manometer));
            }
        }

        /// <summary>
        /// Called when [initialization finished].
        /// </summary>
        /// <param name="result">The result.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        protected void OnInitializationFinished(InitializationResult result, int errorCode)
        {
            if (InitializationFinished != null)
            {
                InitializationFinished(this, new FinishInitializationEventArgs(result, errorCode));
            }
        }
        #endregion Event Handlers

        #region Private Functions
        /// <summary>
        /// Handles the InitializationFinished event of the m_Initialization control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void m_Initialization_InitializationFinished(object sender, EventArgs e)
        {
            FinishInitializationEventArgs eventArgs = e as FinishInitializationEventArgs;
            m_InitializationResult = eventArgs.Result;
            ExecuteNextCommand();
        }

        /// <summary>
        /// Handles the InitializationStepFinished event of the m_Initialization control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void m_Initialization_InitializationStepFinished(object sender, EventArgs e)
        {
            OnInitializationStepFinished(e);
        }

        /// <summary>
        /// Handles the InitializationStepStarted event of the m_Initialization control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void m_Initialization_InitializationStepStarted(object sender, EventArgs e)
        {
            OnInitializationStepStarted(e);
        }

        /// <summary>
        /// Creates the command stack.
        /// </summary>
        private void CreateCommandStack()
        {
            // As we use a stack, insert in reverse order
            m_CommandStack.Push(DeviceCommand.Disconnect);
            m_CommandStack.Push(DeviceCommand.Connect);
        }

        /// <summary>
        /// Executes the next command.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Inspector.BusinessLogic.ActivityControl.OnInitializationFinished(System.String)")]
        private void ExecuteNextCommand()
        {

            if (m_CommandStack.Count > 0)
            {
                m_CurrentCommand = m_CommandStack.Pop();
                System.Diagnostics.Debug.WriteLine("BL InitActivity: ExecuteNextCommand " + m_CurrentCommand.ToString());

                switch (m_CurrentCommand)
                {
                    case DeviceCommand.Connect:
                        OnInitializationStepStarted(DeviceCommand.Connect);
                        Connect();
                        break;
                    case DeviceCommand.Disconnect:
                        OnInitializationStepStarted(DeviceCommand.Disconnect);
                        Disconnect();
                        break;
                    default:
                        m_CommandStack.Clear();
                        m_InitializationResult = InitializationResult.ERROR;
                        Disconnect();
                        break;
                }
            }
            else
            {
                FinishInitialization();
                CommunicationControl.StopCommunication();
                m_Initialization.InitializationFinished -= new EventHandler(m_Initialization_InitializationFinished);
                m_Initialization.InitializationStepFinished -= new EventHandler(m_Initialization_InitializationStepFinished);
                m_Initialization.InitializationStepStarted -= new EventHandler(m_Initialization_InitializationStepStarted);
            }
        }

        /// <summary>
        /// Finishes the initialization.
        /// </summary>
        private void FinishInitialization()
        {
            int errorCode = ErrorCodes.INITIALIZATION_FINISHED_SUCCESSFULLY;
            switch (m_InitializationResult)
            {
                case InitializationResult.SUCCESS:
                    errorCode = ErrorCodes.INITIALIZATION_FINISHED_SUCCESSFULLY;
                    break;
                case InitializationResult.WARNING:
                    errorCode = ErrorCodes.INITIALIZATION_FINISHED_WARNING;
                    break;
                case InitializationResult.TIMEOUT:
                    errorCode = ErrorCodes.INITIALIZATION_FINISHED_TIMEOUT;
                    break;
                case InitializationResult.ERROR:
                case InitializationResult.UNSET:
                default:
                    errorCode = ErrorCodes.INITIALIZATION_FINISHED_ERROR;
                    break;
            }
            OnInitializationFinished(m_InitializationResult, errorCode);
        }

        /// <summary>
        /// Connects this instance.
        /// </summary>
        private void Connect()
        {
            List<BluetoothDongleInformation> allowedBluetoothDongles = new List<BluetoothDongleInformation>();
            clsSettings settings = new clsSettings();
            string btDongleAddress = settings.get_GetSetting(SETTING_CATEGORY, SETTING_BLUETOOTH_DONGLE_ADDRESS).ToString();
            bool isBtDongleAddressDefined = !btDongleAddress.Equals(SETTING_RETURN_NO_VALUE, StringComparison.OrdinalIgnoreCase);
            if (isBtDongleAddressDefined)
            {
                BluetoothDongleInformation btDongleInfo = new BluetoothDongleInformation(btDongleAddress);
                allowedBluetoothDongles.Add(btDongleInfo);
            }

            CommunicationControl.Connect(m_ConnectionProperties, allowedBluetoothDongles, CommandResultCallback);
        }

        /// <summary>
        /// Disconnects this instance.
        /// </summary>
        private void Disconnect()
        {
            CommunicationControl.Disconnect(CommandResultCallback);
        }
        #endregion Private Functions

        #region CallBack
        /// <summary>
        /// CommandResult callback
        /// </summary>
        /// <param name="commandSucceeded">if set to <c>true</c> [command succeeded].</param>
        /// <param name="errorCode">The error code.</param>
        /// <param name="message">The message.</param>
        public void CommandResultCallback(bool commandSucceeded, int errorCode, string message)
        {
            System.Diagnostics.Debug.WriteLine("BL InitActivity: CommandResult " + m_CurrentCommand.ToString());

            InitializationStepResult result = commandSucceeded ? InitializationStepResult.SUCCESS : InitializationStepResult.ERROR;

            if (errorCode == 0)
            { // As the connect and disconnect return 0, we translate it to the Step being executed successfully.
                errorCode = ErrorCodes.INITIALIZATION_STEP_EXECUTED_SUCCESSFULLY;
            }

            if (!commandSucceeded)
            { // If the command did not succeed: Set the message to empty to prevent an error message from being shown.
                message = String.Empty;
            }

            OnInitializationStepFinished(m_CurrentCommand, result, message, errorCode, InitializationManometer.BLUETOOTH_MODULE);

            HandleCommandResultCallback(result);
        }

        /// <summary>
        /// Handles the command result callback.
        /// </summary>
        /// <param name="result">The result.</param>
        private void HandleCommandResultCallback(InitializationStepResult result)
        {
            switch (m_CurrentCommand)
            {
                case DeviceCommand.Connect:
                    if (result == InitializationStepResult.SUCCESS)
                    {
                        m_Initialization.ConnectionProperties = m_ConnectionProperties;
                        m_Initialization.IsInitializationForInspection = false;
                        m_Initialization.ExecuteInitialization();
                    }
                    else
                    {
                        m_InitializationResult = InitializationResult.ERROR;

                        m_CommandStack.Clear();
                        m_CommandStack.Push(DeviceCommand.Disconnect);
                        ExecuteNextCommand();
                    }
                    break;
                case DeviceCommand.Disconnect:
                    ExecuteNextCommand();
                    break;
                default:
                    // do nothing, though the application will hang
                    // Due to the setup of this class, this will not occur.
                    break;
            }
        }
        #endregion CallBack
    }
}

