using System.Collections.Generic;

namespace StateMachineSystem.CustomStates.Enemy
{
    public class WaitState : State
    {
        public WaitState(IEnumerable<Transition> transitions, float period = 0) 
            : base(transitions, period) { }
    }
}