using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace StateMachineSystem.CustomStates.Enemy
{
    public class PatrolState : State
    {
        public event Action OnMovementCompleted;
        
        private readonly NavMeshAgent _navMeshAgent;
        private readonly Vector3 _startPosition;
        private readonly float _patrolRadius;
        
        private Vector3 _destPoint;
        private bool _destPointIsSet;

        private const float TARGET_OFFSET = 0.01f; 

        public PatrolState(IEnumerable<Transition> transitions,
            NavMeshAgent navMeshAgent,
            Vector3 startPosition,
            float patrolRadius,
            float period = 0)
            : base(transitions, period)
        {
            _navMeshAgent = navMeshAgent;
            _startPosition = startPosition;
            _patrolRadius = patrolRadius;
        }

        protected override void Start()
        {
            _navMeshAgent.isStopped = false;

            _destPoint = _startPosition 
                         + new Vector3(Random.Range(-_patrolRadius, _patrolRadius),
                             _startPosition.y,
                             Random.Range(-_patrolRadius, _patrolRadius));
            _navMeshAgent.SetDestination(_destPoint);
            _destPointIsSet = true;
        }

        protected override void Update()
        {
            if (_destPointIsSet)
            {
                if (Mathf.Abs(_navMeshAgent.transform.position.x - _destPoint.x) <= TARGET_OFFSET
                    && Mathf.Abs(_navMeshAgent.transform.position.z - _destPoint.z) <= TARGET_OFFSET)
                {
                    OnMovementCompleted?.Invoke();
                }
            }
        }

        protected override void Stop()
        {
            _navMeshAgent.isStopped = true;
            _destPointIsSet = false;
        }
    }
}