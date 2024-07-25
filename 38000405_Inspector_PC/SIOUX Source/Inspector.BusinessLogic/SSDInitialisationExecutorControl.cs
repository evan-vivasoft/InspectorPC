using System;
using System.Threading;
using Inspector.BusinessLogic.Interfaces;
using Inspector.BusinessLogic.Interfaces.Events;
using Inspector.Connection.Manager.Interfaces;
using Inspector.Infra;
using Inspector.Model;

namespace Inspector.BusinessLogic
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable")]
    public class SSDInitialisationExecutorControl
    {
        public event EventHandler<StartInitializationStepEventArgs> InitialisationStepStarted;
        public event EventHandler<FinishInitializationStepEventArgs> InitialisationStepFinished;
        public event EventHandler SSDInitialisationFinished;
        public event EventHandler<UiRequestEventArgs> UiInputNeeded;

        private readonly ManualResetEvent UiQuestionAnsweredEvent = new ManualResetEvent(false);
        private readonly ICommunicationControl m_CommunicationControl;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        private UiResponse UiResponse;
        public void SetUiReponse(UiResponse response)
        {
            UiResponse = response;
            UiQuestionAnsweredEvent.Set();
        }

        public SSDInitialisationExecutorControl(ICommunicationControl communicationControl)
        {
            m_CommunicationControl = communicationControl;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId =
            "0#")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId =
            "Flag")]
        public bool DoInitialisation(out bool scriptCommandFlag)
        {
            scriptCommandFlag = false;
            SendCommand(DeviceCommand.EnterRemoteLocalCommandMode, out bool result);
            if (!result) return false;

            var value = SendCommand(DeviceCommand.CheckIo3Status, out result);
            if (!result) return false;
            if (value == "ON")
            {
                SendCommand(DeviceCommand.EnableIOStatus, out result);
                if (!result) return false;
                SendCommand(DeviceCommand.ActivatePortSensor, out result);
                if (!result) return false;
                SendCommand(DeviceCommand.ExitRemoteLocalCommandMode, out result);
                if (!result) return false;
                SendCommand(DeviceCommand.StopSensorRun, out result);
                if (!result) return false;
                var id = RetrySendCommand(DeviceCommand.ReadSensorId, 3, out result);
                if (!result) return false;

                if (string.IsNullOrWhiteSpace(id))
                {
                    var continueMeasurement = AskUiToContinueMeasurement();
                    ActivatePortIrDa();
                    return continueMeasurement;
                }

                scriptCommandFlag = true;
                ActivatePortIrDa();
                return true;
            }
            else
            {
                var continueMeasurement = AskUiToContinueMeasurement();
                if (continueMeasurement)
                {
                    SendCommand(DeviceCommand.DisableIOStatus, out result);
                    if (!result) return false;
                    SendCommand(DeviceCommand.ExitRemoteLocalCommandMode, out result);
                    return result;
                }

                SendCommand(DeviceCommand.ExitRemoteLocalCommandMode, out result);
                return false;
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Inspector.BusinessLogic.SSDInitialisationExecutorControl.OnUiInputNeeded(System.String,Inspector.BusinessLogic.Interfaces.UIRequestResponseType)")]
        private bool AskUiToContinueMeasurement()
        {
            //return true;
            UiQuestionAnsweredEvent.Reset();
            OnUiInputNeeded("Continue with measurement?", UIRequestResponseType.YesNo);
            UiQuestionAnsweredEvent.WaitOne(TimeSpan.FromMinutes(10));
            switch (UiResponse)
            {
                case UiResponse.Yes:
                    return true;
                case UiResponse.No:
                case UiResponse.Recheck:
                default:
                    return false;
            }
        }

        private void ActivatePortIrDa()
        {
            var result = false;
            SendCommand(DeviceCommand.EnterRemoteLocalCommandMode, out result);
            if (!result) return;
            SendCommand(DeviceCommand.ActivatePortIrDa, out result);
            if (!result) return;
            SendCommand(DeviceCommand.ExitRemoteLocalCommandMode, out result);
        }

        private string RetrySendCommand(DeviceCommand command, int attempts, out bool result)
        {
            return RetrySendCommand(command, string.Empty, attempts, out result);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2201:DoNotRaiseReservedExceptionTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        private string RetrySendCommand(DeviceCommand command, string parameter, int attempts, out bool result)
        {
            Exception lastException = null;
            while (attempts > 0)
            {
                try
                {
                    return SendCommand(command, parameter, out result);
                }
                catch (Exception ex)
                {
                    lastException = ex;
                    attempts--;
                }
            }
            throw lastException ?? new Exception("an unknown error occurred");
        }

        private string SendCommand(DeviceCommand command, out bool result)
        {
            return SendCommand(command, string.Empty, out result);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2201:DoNotRaiseReservedExceptionTypes")]
        private string SendCommand(DeviceCommand command, string parameter, out bool result)
        {
            OnInitialisationStepStarted(command);
            var resetEvent = new ManualResetEvent(false);
            var resultMessage = "";
            var sendCommandResult = false;
            var error = ErrorCodes.INITIALIZATION_STEP_EXECUTED_SUCCESSFULLY;
            m_CommunicationControl.SendCommand(command, parameter, (commandResult, errorCode, message) =>
            {
                sendCommandResult = commandResult;
                error = errorCode;
                resultMessage = message;
                resetEvent.Set();
            });
            if (!resetEvent.WaitOne(TimeSpan.FromMinutes(5)))
            {
                OnInitialisationStepFinished(command, resultMessage, error, InitializationStepResult.TIMEOUT);
            }
            else
            {
                OnInitialisationStepFinished(command, resultMessage, error, sendCommandResult ? InitializationStepResult.SUCCESS : InitializationStepResult.ERROR);
            }
            result = sendCommandResult;

            return resultMessage;
        }

        protected virtual void OnInitialisationStepFinished(DeviceCommand command, string message, int errorCode, InitializationStepResult stepResult)
        {
            InitialisationStepFinished?.Invoke(this, new FinishInitializationStepEventArgs(command.ToString(), stepResult, message, errorCode , InitializationManometer.UNSET));
        }

        protected virtual void OnInitialisationStepStarted(DeviceCommand command)
        {
            InitialisationStepStarted?.Invoke(this, new StartInitializationStepEventArgs(command.ToString()));
        }

        protected virtual void OnSsdInitialisationFinished()
        {
            SSDInitialisationFinished?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnUiInputNeeded(string message, UIRequestResponseType responseType)
        {
            UiInputNeeded?.Invoke(this, new UiRequestEventArgs(message, responseType));
        }
    }
}
