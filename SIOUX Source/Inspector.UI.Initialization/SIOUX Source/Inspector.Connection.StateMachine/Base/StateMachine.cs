using System;
using System.Collections.Generic;
using Inspector.Connection.SM.Exceptions;

namespace StateMachines.Base
{
    public class StateChangeEventsArgs : EventArgs
    {
        public State OldState { get; private set; }
        public State NewState { get; private set; }
        public StateChangeEventsArgs(State oldState, State newState) { OldState = oldState; NewState = newState; }
    }

    public class StateMachine
    {
        private List<State> m_allStates;
        private State m_currentState;
        public event EventHandler<StateChangeEventsArgs> StateChanged;

        public StateMachine()
        {
            m_allStates = new List<State>();
            m_currentState = null;
        }

        public List<State> AllStates
        {
            get { return m_allStates; }
        }

        public void Add(State s)
        {
            m_allStates.Add(s);
        }

        public State CurrentState
        {
            get { return m_currentState; }
            set { m_currentState = value; }
        }

        public void SetInitialState(Type stateType)
        {
            foreach (State s in m_allStates)
            {
                if (s.GetType() == stateType)
                {
                    if (m_currentState == null)
                    {
                        m_currentState = s;
                        m_currentState.entry();
                        FireStateChange(null, s);
                        m_currentState.body();
                    }
                    break;
                }
            }
        }

        private void CallSuperStateEntry(State newState)
        {
            if (newState.HasSuperState())
            {
                if (!newState.SuperState.EntryHasExecuted)
                {
                    newState.SuperState.entry();
                    newState.SuperState.EntryHasExecuted = true;
                    CallSuperStateEntry(newState.SuperState);
                }
            }
        }

        private void CheckSuperStateExit(State currentState, State newState)
        {
            currentState.EntryHasExecuted = false;
            if (currentState.SuperState != null && currentState.SuperState != newState.SuperState)
            {
                CheckSuperStateExit(currentState.SuperState, newState);
            }
        }

        public void SetState(Type stateType)
        {

            State newState = null;
            State oldState = null;
            bool bFoundIt = false;
            foreach (State s in m_allStates)
            {
                if (s.GetType() == stateType)
                {
                    newState = s;
                    bFoundIt = true;
                    break;
                }
            }
            if (bFoundIt)
            {
                if (m_currentState != null)
                {
                    m_currentState.exit();
                    CheckSuperStateExit(m_currentState, newState);
                }
                if (m_currentState != newState)
                {
                    CallSuperStateEntry(newState);
                    newState.entry();
                    newState.EntryHasExecuted = true;
                    while (newState.HasSubStates())
                    {
                        newState = newState.InitialState;
                        newState.entry();
                        newState.EntryHasExecuted = true;
                    }
                    oldState = m_currentState;
                    m_currentState = newState;
                    FireStateChange(oldState, newState);
                }
                m_currentState.body();
            }
            else
            {
                string currentStateName = (m_currentState == null) ? "unknown state" : m_currentState.GetType().Name;
                string newStateName = (newState == null) ? "unknown state" : newState.GetType().Name;
                string exceptionMessage = String.Format("Specified class is not a valid state: could not set the state from '{0}' and '{1}'", currentStateName, newStateName);
#if DEBUG
                Console.WriteLine(exceptionMessage);
                System.Diagnostics.Debug.WriteLine(exceptionMessage);
#endif
                throw new StateMachineException(exceptionMessage);
            }
        }

        public void OnSignal(string signalName, object[] args)
        {
#if STATEMACHINE
            string message = String.Format("SM: OnSignal Trigger '{0}' on state '{1}'", signalName, CurrentState.GetType().Name);
            Console.WriteLine(message);
            System.Diagnostics.Debug.WriteLine(message);
#endif
            CurrentState.OnSignal(signalName, args);
        }

        private void FireStateChange(State oldState, State newState)
        {
#if DEBUG
            string oldStateName = (oldState == null) ? "Unknown" : oldState.GetType().Name;
            string newStateName = (newState == null) ? "Unknown" : newState.GetType().Name;
            string message = String.Format("SM: FireStateChange from state '{0}' to '{1}'", oldStateName, newStateName);
            System.Diagnostics.Debug.WriteLine(message);
#endif
            if (StateChanged != null)
            {
                StateChanged(this, new StateChangeEventsArgs(oldState, newState));
            }
        }
    }
}
