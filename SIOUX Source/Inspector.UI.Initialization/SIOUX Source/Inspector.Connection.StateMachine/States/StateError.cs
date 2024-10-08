//////////////////////////////////////////////////////////////////////////////////
// Initially Generated by Sioux C# StateMachine Code Generator 					//
//  User changes allowed, requires merging when re-generated		            //
//////////////////////////////////////////////////////////////////////////////////


using StateMachines.Base;

namespace Inspector.Connection
{
    public class Error : ErrorBase
    {
        public Error(StateMachine stateMachine, State superState) : base(stateMachine, superState) { }

        // Signals
        public override void NonFatalErrorTrigger()
        {
            // PUT YOUR OWN CODE HERE IF THE ACTION SHOULD OCCUR BEFORE THE TRIGGER CALL

            ((ConnectionStateMachine)stateMachine).ConnectedReason = ConnectedReason.RECOVEREDFROMERROR;

            base.NonFatalErrorTrigger();

            // PUT YOUR OWN CODE HERE IF THE ACTION SHOULD OCCUR AFTER THE TRIGGER CALL		
        }

        public override void DisconnectTrigger()
        {
            // PUT YOUR OWN CODE HERE IF THE ACTION SHOULD OCCUR BEFORE THE TRIGGER CALL

            base.DisconnectTrigger();

            // PUT YOUR OWN CODE HERE IF THE ACTION SHOULD OCCUR AFTER THE TRIGGER CALL		
        }


        // State Behaviors

        public override void body()
        {
            // PUT YOUR OWN CODE HERE IF THE ACTION SHOULD OCCUR BEFORE THE BODY (DO) CALL

            base.body();

            // PUT YOUR OWN CODE HERE IF THE ACTION SHOULD OCCUR AFTER THE BODY (DO) CALL
        }
    }
}
