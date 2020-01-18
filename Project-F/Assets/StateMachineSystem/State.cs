using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace StateMachineSystem
{

    /// <summary>
    /// Состояние конечного автомата
    /// </summary>
    public abstract class State
    {
        
        /// <summary>
        /// Набор переходов состояния
        /// </summary>
        protected List<Transition> _transitions = new List<Transition>();

        /// <summary>
        /// Набор корутин, опрашивающих условия перехода
        /// </summary>
        protected List<Coroutine> _transitionsPolls = new List<Coroutine>();

        /// <summary>
        /// Словарь обработчиков, подписанных на переходы по событиям
        /// </summary>
        protected Dictionary<EventTransition, Action> _eventTransitionHandlers = new Dictionary<EventTransition, Action>();

        /// <summary>
        /// Конечный автомат, которому принадлежит состояние
        /// </summary>
        protected StateMachine _stateMachine;

        /// <summary>
        /// Период проверки завершения состояния. Если не указан, то проверка осуществляется каждый кадр
        /// </summary>
        protected float _period = 0;

        /// <summary>
        /// Признак активности состояния
        /// </summary>
        protected bool _isActive;



        /// <summary>
        /// Название состояние в которое должен перейти конечный автомат после завершения текущего
        /// </summary>
        public string NextStateName { get; private set; }



        /// <summary>
        /// Создание состояния
        /// </summary>
        /// <param name="transitions">Набор переходов</param>
        /// <param name="period">Период проверки завершения состояния</param>
        public State(IEnumerable<Transition> transitions, float period = 0)
        {
            foreach (Transition transition in transitions)
            {
                _transitions.Add(transition);
            }

            _period = period;
        }





        /// <summary>
        /// Активация состояния
        /// </summary>
        /// <returns></returns>
        public IEnumerator Activate()
        {
            // Определим абстрактное ожидание опроса переходов
            YieldInstruction wait;

            // Если указан период и он больше 0
            if (_period > 0)
                // Устанавливаем время ожидания, равное заданному периоду
                wait = new WaitForSeconds(_period);
            else
                // Иначе ожидаем конец кадра
                wait = new WaitForEndOfFrame();


            _isActive = true;
            NextStateName = null;

            // Запускаем опросы переходов
            ActivateTransitions();

            // Хук срабатывающий в начале активации состояния
            Debug.Log($"SM: {GetType().Name} Start");
            Start();

            while (_isActive)
            {
                // Хук, срабатывающий перед проверкой активности состояния
                Update();
                yield return wait;
            }

            // Хук, срабатывающий при деактивации состояния
            Debug.Log($"SM: {GetType().Name} Stop");
            Stop();
        }

        



        /// <summary>
        /// Установка родительского конечного автомата
        /// </summary>
        /// <param name="SM"></param>
        public void SetStateMachine(StateMachine SM)
        {
            _stateMachine = SM;
        }






        /// <summary>
        /// Деактивация состояния
        /// </summary>
        protected void Deactivate()
        {
            // Останавливаем все опросы переходов
            foreach (Coroutine polling in _transitionsPolls)
            {
                _stateMachine.HandlingScript.StopCoroutine(polling);
            }

            // Очищаем набор опросов переходов
            _transitionsPolls.Clear();


            // Отписываем все обработчики переходов по событиям
            foreach(var transitionHandler in _eventTransitionHandlers)
            {
                transitionHandler.Key.OnTransitionTriggered -= transitionHandler.Value;
            }

            // Очищаем список обработчиков переходов по событиям
            _eventTransitionHandlers.Clear();


            _isActive = false;
        }


        /// <summary>
        /// Активация переходов
        /// </summary>
        protected void ActivateTransitions()
        {
            foreach(var transition in _transitions)
            {
                if(transition is ConditionalTransition conditionalTransition)
                {
                    SetConditionalTransitionPoll(conditionalTransition);
                }   
                else if(transition is EventTransition eventTransition)
                {
                    SetEventTransitionHandler(eventTransition);
                }
            }
        }



        /// <summary>
        /// Подписка на срабатывание перехода по событию
        /// </summary>
        /// <param name="transition"></param>
        protected void SetEventTransitionHandler(EventTransition transition)
        {
            Action handler = () => SetStateResult(transition.nextState);

            _eventTransitionHandlers.Add(transition, handler);

            transition.OnTransitionTriggered += handler;
        }


        /// <summary>
        /// Установка опроса перехода
        /// </summary>
        /// <param name="transition">Переход, для которого устанавливается опрос</param>
        protected void SetConditionalTransitionPoll(ConditionalTransition transition)
        {
            Coroutine cor = _stateMachine.HandlingScript.StartCoroutine(TransitionPolling(transition));
            
            _transitionsPolls.Add(cor);
        }


        /// <summary>
        /// Корутина опроса перехода
        /// </summary>
        /// <param name="transition">Опрашиваемый переход</param>
        /// <returns></returns>
        protected IEnumerator TransitionPolling(ConditionalTransition transition)
        {
            // Установим время ожидания, равное указанному периоду
            var wait = new WaitForSeconds(transition.period);

            // Пока не выполнилось условие перехода
            do
            {
                // Ожидаем
                yield return wait;
            }
            while (!transition.condition());
            
            // Если условие перехода выполнилось, то устанавливаем результат работы состояния - название следующего состояния
            SetStateResult(transition.nextState);
        }


        /// <summary>
        /// Установка результата работы состояния
        /// </summary>
        /// <param name="stateName">Следующее состояние КА</param>
        protected void SetStateResult(string stateName)
        {
            Deactivate();

            NextStateName = stateName;
        }






        /// <summary>
        /// Хук срабатывающий в начале активации состояния
        /// </summary>
        protected virtual void Start() { }

        /// <summary>
        /// Хук, срабатывающий перед проверкой активности состояния
        /// </summary>
        protected virtual void Update() { }

        /// <summary>
        /// Хук, срабатывающий при деактивации состояния
        /// </summary>
        protected virtual void Stop() { }

    }

}


