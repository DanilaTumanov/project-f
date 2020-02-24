using System;
using Portals;
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
    [SerializeField] private float _movementSpeed = 10f;

    [Header("Portal Settings")]
    [SerializeField] private PortalMarker _portalMarker;
    [SerializeField] private GameObject _portalPrefab;

    [Header("Control")] 
    [SerializeField] private GameObject _joystickGroup;
    [SerializeField] private FixedJoystick _fixedJoystick;
    [SerializeField] private ButtonControlView _climbButton;

    private Camera _mainCamera;
    private State _currentState;
    private Vector3 _normalMovementVector;
    
    private Vector3 _wallDirection;
    private Vector3 _climbedPosition;
    private bool _climbIsPressed;
    private bool _isWallForward;
    private int _controlModeIndex;

    private ControlMode _currentControlMode;
    
    private bool _isPortalThrowAvailable;
    private GameObject _portal;
    
    private const KeyCode CHANGE_CONTROL_KEY = KeyCode.P;
    private const KeyCode APP_QUIT_KEY = KeyCode.Q;
    private const KeyCode PORTAL_THROW_KEY = KeyCode.L;

    private readonly ControlMode[] _controlModes =
    {
        ControlMode.Wasd,
        ControlMode.WasdSpace,
        ControlMode.Navmesh
    };

    private const float DELTA_CLIMBED_POS = 1f; 
    private const float CHECK_WALL_DIST = 2.0f; 

    private void Awake()
    {
        _mainCamera = Camera.main;

        _normalMovementVector = Vector3.zero;
        _wallDirection = Vector3.zero;
        
        _isPortalThrowAvailable = false;
        
        _currentState = State.Moving;
        _controlModeIndex = _controlModes.Length - 1;
        ChangeControlHandler();
    }

    private void Update()
    {
        //change control
        if (Input.GetKeyDown(CHANGE_CONTROL_KEY))
        {
            ChangeControlHandler();
        }

        if (Input.GetKeyDown(APP_QUIT_KEY))
        {
            Application.Quit();
        }

        if (Input.GetKeyDown(PORTAL_THROW_KEY))
        {
            SwitchPortalThrowing(true);
        }
        
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

        if (_isPortalThrowAvailable)
        {
            UpdatePortalMarkerPosition();
        }
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
        //джойстик
        if (Mathf.Abs(_fixedJoystick.Horizontal) > 0.01 || Mathf.Abs(_fixedJoystick.Vertical) > 0.01)
        {
            return new Vector3(
                    _fixedJoystick.Horizontal, 
                    0, 
                    _fixedJoystick.Vertical)
                .normalized;
        }

        return new Vector3(
            Input.GetAxis("Horizontal"),
            0,
            Input.GetAxis("Vertical")
        ).normalized;
    }

    /// <summary>
    /// Управление через WASD
    /// </summary>
    private void UpdatePosition()
    {
        switch (_currentState)
        {
            case State.Moving:
                _normalMovementVector = GetNormalMovementVector();
                _rigidbody.transform.Translate(_movementSpeed * Time.deltaTime * _normalMovementVector);
                
                if (_currentControlMode == ControlMode.WasdSpace)
                {
                    if (Input.GetKeyDown(KeyCode.Space) || _climbButton.IsPressed)
                    {
                        _climbIsPressed = true;
                    }
                }
                break;
        }
    }

    /// <summary>
    /// Управление через navmesh
    /// </summary>
    private void NavMeshControl()
    {
        if (Input.GetMouseButtonDown(0) && !_isPortalThrowAvailable)
        {
            var ray = _mainCamera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out var hit))
            {
                _navMeshAgent.SetDestination(hit.point);
            }
        }
    }

    /// <summary>
    /// Смена управления
    /// </summary>
    private void ChangeControlHandler()
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
            
            _joystickGroup.SetActive(false);
        }
        else
        {
            transform.rotation = Quaternion.identity;
            _navMeshAgent.enabled = false;
            _capsuleCollider.enabled = true;
            
            _joystickGroup.SetActive(true);
            
            _climbButton.gameObject.SetActive(_currentControlMode == ControlMode.WasdSpace);
        }
    }

    //TODO: Input manager
    private Vector3 _oldMousePosition = new Vector3();
    private Vector3 _oldPlayerPosition = new Vector3();
    private const float CHANGE_POS_PRECISION = 0.1f;
    
    /// <summary>
    /// Управление броском портала
    /// </summary>
    private void UpdatePortalMarkerPosition()
    {
        var mousePos = Input.mousePosition;
        var playerPos = transform.position;

        bool isMousePosChanged = Mathf.Abs(_oldMousePosition.x - mousePos.x) > CHANGE_POS_PRECISION
                                 || Mathf.Abs(_oldMousePosition.y - mousePos.y) > CHANGE_POS_PRECISION
                                 || Mathf.Abs(_oldMousePosition.z - mousePos.z) > CHANGE_POS_PRECISION;

        bool isPlayerPosChanged = Mathf.Abs(_oldPlayerPosition.x - playerPos.x) > CHANGE_POS_PRECISION
                                  || Mathf.Abs(_oldPlayerPosition.y - playerPos.y) > CHANGE_POS_PRECISION
                                  || Mathf.Abs(_oldPlayerPosition.z - playerPos.z) > CHANGE_POS_PRECISION;

        if (isMousePosChanged || isPlayerPosChanged)
        {
            var ray = _mainCamera.ScreenPointToRay(mousePos);
            if (Physics.Raycast(ray, out var hit))
            {
                _portalMarker.SetPosition(transform.position, hit.point);
            }
            
            _oldMousePosition = mousePos;
            _oldPlayerPosition = playerPos;
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (_portal != null)
            {
                Destroy(_portal.gameObject);
            }

            _portal = Instantiate(_portalPrefab);
            _portal.transform.position = _portalMarker.transform.position;
            SwitchPortalThrowing(false);
        }
    }

    private void SwitchPortalThrowing(bool isAvailable)
    {
        _isPortalThrowAvailable = isAvailable;
        _portalMarker.SetAvailable(isAvailable);
    }

    private string _infoText = string.Empty;
    
    // TODO: DEBUG
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
    
    // TODO: DEBUG
    private void OnDrawGizmos()
    {
        if (_isPortalThrowAvailable)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(_playerBottom.position, _portalMarker.ThrowRadius);
        }
    }
}
