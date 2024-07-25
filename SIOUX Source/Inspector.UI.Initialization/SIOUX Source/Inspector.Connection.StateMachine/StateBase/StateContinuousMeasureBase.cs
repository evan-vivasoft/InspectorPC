///////////////////////////////////////////////////////
// Generated by Sioux C# StateMachine Code Generator //
//                 DO NOT EDIT				         //
///////////////////////////////////////////////////////

using System.Collections.Generic;

using StateMachines.Base;

namespace Inspector.Connection
{
    public class ContinuousMeasureBase : State
    {
        public ContinuousMeasureBase(StateMachine stateMachine, State superState) : base(stateMachine, superState) { }
		
		// Signals
		public virtual void ErrorTrigger(string message, int errorCode)
		{
			((ConnectionStateMachine)stateMachine).Message = message;
			((ConnectionStateMachine)stateMachine).ErrorCode = errorCode;
			
			SetState(typeof(Error));
		}
		
		public virtual void ContinuousMeasurementStartedTrigger()
		{
			
			SetState(typeof(ContinuousMeasure));
		}
		
		public virtual void StopContinuousMeasurementTrigger()
		{
			
			SetState(typeof(ContinuousMeasure));
		}
		
		public virtual void MeasurementReceivedTrigger(IList<double> measurementData)
		{
			((ConnectionStateMachine)stateMachine).MeasurementData = measurementData;
			
			SetState(typeof(ContinuousMeasure));
		}
		
		public virtual void ContinuousMeasurementStoppedTrigger()
		{
			
			SetState(typeof(Connected));
		}
		
	
		// State Behaviors

		public override void body()
		{
			((ConnectionStateMachine)stateMachine).Do_ContinuousMeasure();
		}
	}
}
