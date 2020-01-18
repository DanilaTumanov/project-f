using System.Collections.Generic;
using UnityEngine;

namespace StateMachineSystem.CustomStates
{
    public class StartState : State
    {
        public StartState(IEnumerable<Transition> transitions, float period = 0) : base(transitions, period)
        {
        }

        protected override void Start()
        {
            Debug.Log("Start Start))");
        }

        protected override void Stop()
        {
            Debug.Log("Start Stop");
        }
    }
}