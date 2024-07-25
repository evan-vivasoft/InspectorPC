using System;
using System.Reflection;
using System.Text;
using Inspector.Connection.SM.Exceptions;

namespace StateMachines.Base
{
    public class State
    {
        private StateMachine m_stateMachine;
        private State m_superState = null;
        private State m_initialState;


        public State(StateMachine stateMachine, State superState)
        {
            m_stateMachine = stateMachine;
            m_superState = superState;
        }

        public virtual void entry()
        {
        }

        public virtual void body()
        {
        }

        public virtual void exit()
        {
        }

        public void SetInitialState(Type stateType)
        {
            foreach (State s in m_stateMachine.AllStates)
            {
                if (s.GetType() == stateType)
                {
                    if (m_initialState == null)
                    {
                        m_initialState = s;
                    }
                    break;
                }
            }
        }

        public void SetState(Type stateType)
        {
            m_stateMachine.SetState(stateType);
        }

        public void Add(State s)
        {
            m_stateMachine.Add(s);
        }

        public void OnSignal(string signalName, object[] args)
        {
            var t = m_stateMachine.CurrentState.GetType();
            try
            {
                t.InvokeMember(signalName, BindingFlags.Default | BindingFlags.InvokeMethod, Type.DefaultBinder, m_stateMachine.CurrentState, args);
            }
            catch (MissingMethodException)
            {
                // this state does not implement the method, so try the parent 
                if (m_superState != null)
                {
                    m_stateMachine.CurrentState = m_superState;
                    m_superState.OnSignal(signalName, args);
                }
                else
                {
                    StringBuilder sb = new StringBuilder("State ");
                    sb.Append(m_stateMachine.CurrentState.GetType().Name);
                    sb.Append(" or its substates cannot process signal ");
                    sb.Append(signalName);
#if DEBUG
                    Console.WriteLine(sb.ToString());
                    System.Diagnostics.Debug.WriteLine(sb.ToString());
#endif
                    throw new StateMachineException(sb.ToString());
                }
            }
        }

        protected StateMachine stateMachine
        {
            get { return m_stateMachine; }
        }

        public bool HasSubStates()
        {
            return m_initialState != null;
        }

        public bool HasSuperState()
        {
            return m_superState != null;
        }

        public State SuperState
        {
            get { return m_superState; }
        }

        public bool EntryHasExecuted
        {
            get;
            set;
        }

        public State InitialState
        {
            get { return m_initialState; }
        }

    }
}
