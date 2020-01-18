using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace StateMachineSystem
{

    /// <summary>
    /// Переход из состояния конечного автомата
    /// </summary>
    public abstract class Transition
    {
        /// <summary>
        /// Состояние в которое осуществляется переход
        /// </summary>
        public string nextState;


        /// <summary>
        /// Период опроса условия перехода
        /// </summary>
        public float period;



        protected Transition(string nextState, float period)
        {
            this.nextState = nextState;
            this.period = period;
        }

    }

}


