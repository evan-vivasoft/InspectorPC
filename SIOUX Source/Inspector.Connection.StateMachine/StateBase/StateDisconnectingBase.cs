///////////////////////////////////////////////////////
// Generated by Sioux C# StateMachine Code Generator //
//                 DO NOT EDIT				         //
///////////////////////////////////////////////////////


using StateMachines.Base;

namespace Inspector.Connection
{
    public class DisconnectingBase : State
    {
        public DisconnectingBase(StateMachine stateMachine, State superState) : base(stateMachine, superState) { }

        // Signals
        public virtual void DisconnectedTrigger()
        {

            SetState(typeof(Disconnected));
        }


        // State Behaviors

        public override void body()
        {
            ((ConnectionStateMachine)stateMachine).Do_Disconnecting();
        }
    }
}
