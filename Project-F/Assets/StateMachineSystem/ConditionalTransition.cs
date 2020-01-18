using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace StateMachineSystem
{

    public class ConditionalTransition : Transition
    {
        
        /// <summary>
        /// Условие перехода
        /// </summary>
        public Func<bool> condition;



        public ConditionalTransition(string nextState, Func<bool> condition, float period = 0) : base(nextState, period)
        {
            this.condition = condition;
        }
    }

}
