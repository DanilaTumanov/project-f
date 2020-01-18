using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;


namespace StateMachineSystem
{

    public class EventTransition : Transition
    {

        public Action<Action> setEventTrigger;

        
        public event Action OnTransitionTriggered;



        public EventTransition(string nextState, Action<Action> setEventTrigger) : base(nextState, 0)
        {
            this.setEventTrigger = setEventTrigger;

            setEventTrigger(TriggerTransition);
        }




        private void TriggerTransition()
        {
            if (OnTransitionTriggered != null)
            {
                OnTransitionTriggered.Invoke();
            }
        }

    }

}
