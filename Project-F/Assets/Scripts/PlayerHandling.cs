using System;
using UnityEngine;
using UnityEngine.AI;


[RequireComponent(typeof(Rigidbody))]
public class PlayerHandling : MonoBehaviour
{
    private enum ControlMode
    {
        Wasd,
        WasdSpace,
        Navmesh
    }

    private enum State
    {
        Moving,
        Climbing,
        ClimbingMoving
    }
    
    [SerializeField] private NavMeshAgent _navMeshAgent;
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private CapsuleCollider _capsuleCollider;
    [SerializeField] private Transform _playerTop;
    [SerializeField] private Transform _playerBottom;
    
    [SerializeField] private float _movementSpeed = 10;

    private Camera _mainCamera;
    private State _currentState;
    private Vector3 _normalMovementVector;
    
    private Vector3 _wallDirection;
    private Vector3 _climbedPosition;

    private bool _climbIsPressed;
    private bool _isWallForward;

    private int _controlModeIndex;

    private ControlMode _currentControlMode;

    private readonly ControlMode[] _controlModes =
    {
        ControlMode.WasdSpace,
        ControlMode.Wasd,
        ControlMode.Navmesh
    };

    private const float DELTA_CLIMBED_POS = 1f; 
    private const float CHECK_WALL_DIST = 2.0f; 

    private void Awake()
    {
        _mainCamera = Camera.main;
        _currentState = State.Moving;
        
        _normalMovementVector = Vector3.zero;
        _wallDirection = Vector3.zero;

        _controlModeIndex = 0;
        _currentControlMode = _controlModes[_controlModeIndex];
    }

    private void Update()
    {
        switch (_currentControlMode)
        {
            case ControlMode.Wasd:
            case ControlMode.WasdSpace:
                UpdatePosition();
                break;
            case ControlMode.Navmesh:
                NavMeshControl();
                break;
        }

        //change control
        ChangeControlHandler();
    }

    private void FixedUpdate()
    {
        CheckWallHandler();
        ClimbingHandler();
    }
    
    private void ClimbingHandler()
    {
        if (_climbIsPressed)
        {
            if (_normalMovementVector != Vector3.zero)
            {
                //чекаем стену спереди
                if (Physics.Raycast(transform.position, 
                        _normalMovementVector, CHECK_WALL_DIST) 
                    && !Physics.Raycast(_playerTop.position, _normalMovementVector,CHECK_WALL_DIST))
                {
                    _wallDirection = _normalMovementVector;
                    _currentState = State.Climbing;
                    _rigidbody.useGravity = false;
                }
            }

            _climbIsPressed = false;
        }

        switch (_currentState)
        {
            case State.Climbing:
                _rigidbody.velocity = Vector3.up * _movementSpeed;
            
                if (!Physics.Raycast(_playerBottom.position, 
                    _wallDirection, CHECK_WALL_DIST))
                {
                    _currentState = State.ClimbingMoving;
                    _climbedPosition = transform.position;
                    _rigidbody.velocity = Vector3.zero;
                }
                break;
            case State.ClimbingMoving:
                _rigidbody.velocity = _wallDirection * _movementSpeed;

                if (Mathf.Abs(Vector3.Distance(_climbedPosition, transform.position)) >= DELTA_CLIMBED_POS)
                {
                    _rigidbody.velocity = Vector3.zero;
                    _rigidbody.useGravity = true;
                    _currentState = State.Moving;
                }
                break;
        }
    }


    private void CheckWallHandler()
    {
        if (_currentControlMode == ControlMode.Wasd)
        {
            if (_normalMovementVector != Vector3.zero)
            {
                //чекаем стену спереди
                if (Physics.Raycast(transform.position, 
                        _normalMovementVector, CHECK_WALL_DIST) 
                    && !Physics.Raycast(_playerTop.position, _normalMovementVector,CHECK_WALL_DIST))
                {
                    _wallDirection = _normalMovementVector;
                    _currentState = State.Climbing;
                    _rigidbody.useGravity = false;
                }
            }
        }
    }

    private Vector3 GetNormalMovementVector()
    {
        return new Vector3(
            Input.GetAxis("Horizontal"),
            0,
            Input.GetAxis("Vertical")
        ).normalized;
    }

    private void UpdatePosition()
    {
        switch (_currentState)
        {
            case State.Moving:
                _normalMovementVector = GetNormalMovementVector();
                _rigidbody.transform.Translate(_movementSpeed * Time.deltaTime * _normalMovementVector);
                
                if (_currentControlMode == ControlMode.WasdSpace)
                {
                    if (Input.GetKeyDown(KeyCode.Space))
                    {
                        _climbIsPressed = true;
                    }
                }
                break;
        }
    }

    private void NavMeshControl()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var ray = _mainCamera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out var hit))
            {
                _navMeshAgent.SetDestination(hit.point);
            }
        }
    }

    private void ChangeControlHandler()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            _controlModeIndex++;

            if (_controlModeIndex >= _controlModes.Length)
            {
                _controlModeIndex = 0;
            }
            
            _currentControlMode = _controlModes[_controlModeIndex];

            if (_currentControlMode == ControlMode.Navmesh)
            {
                _navMeshAgent.enabled = true;
                _capsuleCollider.enabled = false;
            }
            else
            {
                transform.rotation = Quaternion.identity;
                _navMeshAgent.enabled = false;
                _capsuleCollider.enabled = true;
            }
        }
    }

    private string _infoText = string.Empty;
    
    private void OnGUI()
    {
        switch (_currentControlMode)
        {
            case ControlMode.Navmesh:
                _infoText = "NavMesh";
                break;
            case ControlMode.Wasd:
                _infoText = "WASD\nTo climb press forward";
                break;
            case ControlMode.WasdSpace:
                _infoText = "WASD\nTo climb press forward + space";
                break;
            default:
                _infoText = String.Empty;
                break;
        }
        
        GUI.Label(new Rect(10, 10, 300, 60), $"Control (click P to change): {_infoText}");
    }
}
