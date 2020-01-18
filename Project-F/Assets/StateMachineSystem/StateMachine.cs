using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace StateMachineSystem
{

    /// <summary>
    /// Конечный автомат. Позволяет настроить граф КА и управляет переключением состояний по корутинам
    /// </summary>
    public class StateMachine
    {
        
        /// <summary>
        /// Набор состояний КА
        /// </summary>
        private Dictionary<string, State> _states = new Dictionary<string, State>();

        /// <summary>
        /// Имя начального состояния
        /// </summary>
        private string _initialStateName = null;

        /// <summary>
        /// Текущее состояние
        /// </summary>
        private State _currentState;




        /// <summary>
        /// Скрипт, которым управляет КА, либо любой MB скрипт (для использования корутин)
        /// </summary>
        public MonoBehaviour HandlingScript { get; private set; }






        /// <summary>
        /// Создание конечного автомата
        /// </summary>
        /// <param name="handlingScript">Скрипт, которым управляет КА, либо любой скрипт MB</param>
        /// <param name="initialStateName">Название начального состояния</param>
        public StateMachine(MonoBehaviour handlingScript, string initialStateName)
        {
            HandlingScript = handlingScript;
            _initialStateName = initialStateName;
        }

        




        
        /// <summary>
        /// Добавить состояние КА
        /// </summary>
        /// <param name="stateName">Название состояния</param>
        /// <param name="state">Состояние</param>
        public void AddState(string stateName, State state)
        {
            if (_states.ContainsKey(stateName))
            {
                throw new Exception("Попытка добавить состояние с ранее зарегистрированным названием.");
            }

            state.SetStateMachine(this);
            _states.Add(stateName, state);
        }


        /// <summary>
        /// Запуск конечного автомата
        /// </summary>
        public void Start()
        {
            _currentState = GetState(_initialStateName);
            HandlingScript.StartCoroutine(StatesProcessing());
        }








        /// <summary>
        /// Получить состояние из справочника по названию
        /// </summary>
        /// <param name="stateName">Название состояния</param>
        /// <returns></returns>
        private State GetState(string stateName)
        {
            return stateName == null || stateName == "" ? null : _states[stateName];
        }

        
        /// <summary>
        /// Рабочий цикл конечного автомата. Обработка и переключение состояний
        /// </summary>
        /// <returns></returns>
        private IEnumerator StatesProcessing()
        {
            while (_currentState != null)
            {
                // Активируем текущее состояние
                yield return HandlingScript.StartCoroutine(_currentState.Activate());
                
                // После завершения работы состояния в его поле NextStateName будет указано название состояния, 
                // в которое должен осуществиться переход. Устанавливаем состояние в качестве текущего
                _currentState = GetState(_currentState.NextStateName);
            }
        }



    }

}


