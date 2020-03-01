using System;
using StateMachineSystem;
using StateMachineSystem.CustomStates.Enemy;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace Enemy
{
    public class EnemyController : MonoBehaviour
    {
        private class MovementState
        {
            public const string Patrol = "Patrol";
            public const string Wait = "Wait";
        }

        [SerializeField] private NavMeshAgent _navMeshAgent;
        [SerializeField] private float _patrolRadius;
        [SerializeField] private float _minWaitTime = 1f;
        [SerializeField] private float _maxWaitTime = 4f;

        private Vector3 _startPosition;
        private StateMachine _movementSm;
        private PatrolState _patrolState;

        private event Action OnMovementCompleted;

        private void Awake()
        {
            _startPosition = transform.position;
            
            InitStateMachine();
        }

        private void InitStateMachine()
        {
            _movementSm = new StateMachine(this, MovementState.Wait);

            _patrolState = new PatrolState(new Transition[]
                {
                    new ConditionalTransition(MovementState.Wait,
                        () => Input.GetKeyDown(KeyCode.M)),
                    new EventTransition(MovementState.Wait, 
                        action => OnMovementCompleted += action)
                },
                _navMeshAgent,
                _startPosition,
                _patrolRadius);
            
            _movementSm.AddState(MovementState.Wait, 
                new WaitState(new []
                    {
                        new ConditionalTransition(MovementState.Patrol, () => true) 
                    },
                    Random.Range(_minWaitTime, _maxWaitTime)));
            
            _movementSm.AddState(MovementState.Patrol, _patrolState);

            _patrolState.OnMovementCompleted += () => OnMovementCompleted?.Invoke();

            _movementSm.Start();
        }

        private void Update()
        {
            
        }

        // TODO: DEBUG
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(_startPosition, _patrolRadius);
        }
    }
}