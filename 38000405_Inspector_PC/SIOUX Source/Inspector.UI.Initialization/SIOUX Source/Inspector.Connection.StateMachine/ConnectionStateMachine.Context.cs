
/////////////////////////////////////////////////////////////////
// Initially generated by Sioux C# StateMachine Code Generator //
/////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using Inspector.Connection.EventTranslation;
using Inspector.Hal.Interfaces;
using Inspector.Infra.Ioc;
using Inspector.Model.BluetoothDongle;

namespace Inspector.Connection
{
    public enum DisconnectedReason
    {
        UNSET,
        DISCONNECTED,
        DISCONNECTING,
        CONNECTFAILED
    }

    public enum ConnectedReason
    {
        UNSET,
        SENDMESSAGE,
        CONNECTING,
        RECOVEREDFROMERROR,
        CONTINEOUSMEASUREMENTSTOPPED,
    }

    public enum ContinuousMeasureReason
    {
        UNSET,
        START,
        MEASUREMENTRECEIVED,
        MEASUREMENTSTARTED,
    }

    public partial class ConnectionStateMachine : IConnectionStateMachine
    {
        private EventToCallConnectionStateMachine m_EventToCallConnectionStateMachine;

        public EventToCallConnectionStateMachine EventToCallConnectionStateMachine
        {
            //get { return m_EventToCallConnectionStateMachine; }
            set { m_EventToCallConnectionStateMachine = value; }
        }

        // Events declarations
        public event EventHandler<MessageReceivedEventArgs> MessageReceived;
        public event EventHandler<ErrorEventArgs> Error;
        public event EventHandler<DisconnectingEventArgs> Disconnecting;
        public event EventHandler<DisconnectedEventArgs> Disconnected;
        public event EventHandler<ConnectingEventArgs> Connecting;
        public event EventHandler<ConnectFailedEventArgs> ConnectFailed;
        public event EventHandler<ConnectedEventArgs> Connected;
        public event EventHandler<SendCommandEventArgs> SendCommand;
        public event EventHandler<RecoveredFromErrorEventArgs> RecoveredFromError;
        public event EventHandler<MeasurementEventArgs> MeasurementReceived;
        public event EventHandler ContinuousMeasurementStopped;
        public event EventHandler ContinuousMeasurementStarted;

        private bool m_Disposed;

        private IHal m_Hal;

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
                    if (m_Hal != null)
                    {
                        m_Hal.Dispose();
                    }
                }
            }

            m_Disposed = true;
        }


        #region Trigger supplied parameters
        public string Data { get; set; }
        public string Message { get; set; }
        public int ErrorCode { get; set; }
        public Dictionary<string, string> ConnectionParameters { get; set; }
        public List<BluetoothDongleInformation> AllowedBluetoothDongles { get; set; }
        public IList<double> MeasurementData { get; set; }
        public Infra.DeviceCommand Command { get; set; }
        public string CommandParameter { get; set; }
        public int MeasurementFrequency { get; set; }
        #endregion

        public DisconnectedReason DisconnectedReason { get; set; }
        public ConnectedReason ConnectedReason { get; set; }
        public ContinuousMeasureReason ContinuousMeasureReason { get; set; }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        public void Initialize()
        {
            // PUT YOUR INITIALIZATION CODE HERE 
        }


        // State Behaviors to be implemented
        public void Do_SendCommand()
        {
            Hal.SendCommand(Command, CommandParameter);
            OnSendCommand();
        }

        public void Do_Error()
        {
            OnError(Message, ErrorCode);
        }

        public void Do_Disconnecting()
        {
            Hal.Disconnect();
            OnDisconnecting();
        }

        public void Do_Disconnected()
        {
            switch (DisconnectedReason)
            {
                case DisconnectedReason.DISCONNECTING:
                case DisconnectedReason.DISCONNECTED:
                    OnDisconnected();
                    break;
                case DisconnectedReason.CONNECTFAILED:
                    OnConnectFailed(Message, ErrorCode);
                    break;
                case DisconnectedReason.UNSET:
                default:
                    break;
            }
        }

        public void Do_Connecting()
        {
            Hal.Connect(ConnectionParameters, AllowedBluetoothDongles);
            OnConnecting();
        }

        public void Do_Connected()
        {
            switch (ConnectedReason)
            {
                case ConnectedReason.SENDMESSAGE:
                    OnMessageReceived(Data);
                    break;
                case ConnectedReason.CONNECTING:
                    OnConnected();
                    break;
                case ConnectedReason.RECOVEREDFROMERROR:
                    OnRecoveredFromError();
                    break;
                case ConnectedReason.CONTINEOUSMEASUREMENTSTOPPED:
                    OnContinuousMeasurementStopped();
                    break;
                case ConnectedReason.UNSET:
                default:
                    break;
            }
        }

        public void Entry_Connected()
        {
            Hal.IsBusy = false;
        }

        public void Exit_Connected()
        {
            Hal.IsBusy = true;
        }

        public void Do_ContinuousMeasure()
        {
            switch (ContinuousMeasureReason)
            {
                case ContinuousMeasureReason.START:
                    Hal.StartContinuousMeasurement(MeasurementFrequency);
                    break;
                case ContinuousMeasureReason.MEASUREMENTRECEIVED:
                    OnMeasurementReceived(MeasurementData);
                    break;
                case ContinuousMeasureReason.MEASUREMENTSTARTED:
                    OnContinuousMeasurementStarted();
                    break;
                case ContinuousMeasureReason.UNSET:
                default:
                    break;
            }
        }

        // Event handling
        public void OnMessageReceived(string data)
        {
            if (MessageReceived != null)
            {
                MessageReceived(this, new MessageReceivedEventArgs(data));
            }
        }

        public void OnError(string message, int errorCode)
        {
            if (Error != null)
            {
                Error(this, new ErrorEventArgs(message, errorCode));
            }
        }

        public void OnDisconnecting()
        {
            if (Disconnecting != null)
            {
                Disconnecting(this, new DisconnectingEventArgs());
            }
        }

        public void OnDisconnected()
        {
            if (Disconnected != null)
            {
                Disconnected(this, new DisconnectedEventArgs());
            }
        }

        public void OnConnecting()
        {
            if (Connecting != null)
            {
                Connecting(this, new ConnectingEventArgs());
            }
        }

        public void OnConnectFailed(string message, int errorCode)
        {
            if (ConnectFailed != null)
            {
                ConnectFailed(this, new ConnectFailedEventArgs(message, errorCode));
            }
        }

        public void OnConnected()
        {
            if (Connected != null)
            {
                Connected(this, new ConnectedEventArgs());
            }
        }

        public void OnRecoveredFromError()
        {
            if (RecoveredFromError != null)
            {
                RecoveredFromError(this, new RecoveredFromErrorEventArgs());
            }
        }

        public void OnSendCommand()
        {
            if (SendCommand != null)
            {
                SendCommand(this, new SendCommandEventArgs());
            }
        }

        public void OnMeasurementReceived(IList<double> measurements)
        {
            if (MeasurementReceived != null)
            {
                MeasurementReceived(this, new MeasurementEventArgs(measurements));
            }
        }

        public void OnContinuousMeasurementStopped()
        {
            if (ContinuousMeasurementStopped != null)
            {
                ContinuousMeasurementStopped(this, EventArgs.Empty);
            }
        }

        public void OnContinuousMeasurementStarted()
        {
            if (ContinuousMeasurementStarted != null)
            {
                ContinuousMeasurementStarted(this, EventArgs.Empty);
            }
        }
        // Guards to be implemented

        // Effects to be implemented
    }
}  // end namespace Connection 
