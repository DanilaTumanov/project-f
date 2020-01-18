using System.Collections.Generic;
using UnityEngine;

namespace StateMachineSystem.CustomStates
{
    public class StopState : State
    {
        public StopState(IEnumerable<Transition> transitions, float period = 0) : base(transitions, period)
        {
        }

        protected override void Start()
        {
            Debug.Log("Stop Start");
        }

        protected override void Stop()
        {
            Debug.Log("Stop Stop");
        }
    }
}