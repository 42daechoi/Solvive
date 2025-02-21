using Photon.Pun;
using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviourPun
{
    public static PlayerController Instance { get; private set; }
    private IState IdleState { get; set; }
    private IState JumpState { get; set; }
    private IState UseComputerState { get; set; }
    public float VerticalVelocity { get; set; }
    
    private IState _previousState;
    private IState _currentState;

    private PlayerMovement _playerMovement;
    private PlayerAnimator _playerAnimator;
    private CharacterController _controller;
    [Header("Speed Settings")]
    [SerializeField] private MovementSettings _speedSettings;
    
    private PhotonView _photonView;
    private Interaction _interaction;
    
    private float _currentSpeed;
    private Vector3 _computerInteractionPoint;
    private Quaternion _computerInteractionRotation;
    
    private InputManager_Game _defaultInputManager;
    private InputManager_Computer _computerInputManager;
    
    
    public MovementSettings SpeedSettings => _speedSettings;
    public CharacterController Controller => _controller;

    private void Awake()
    {
        _photonView = GetComponent<PhotonView>();
        if (_photonView.IsMine)
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Debug.LogWarning("로컬 PlayerController가 이미 존재.");
                Destroy(gameObject);
            }
        }
    }
    
    private void Start()
    {
        _controller = GetComponent<CharacterController>();
        _photonView = GetComponent<PhotonView>();
        _interaction = GetComponent<Interaction>();
        _playerAnimator = GetComponent<PlayerAnimator>();
        _currentSpeed = _speedSettings.walkSpeed;
        _playerMovement = GetComponent<PlayerMovement>();
        
        _playerMovement = gameObject.AddComponent<PlayerMovement>();
        _playerAnimator = gameObject.AddComponent<PlayerAnimator>();
        

        IdleState = new IdleState();
        JumpState = new JumpState();
        
        _defaultInputManager = FindObjectOfType<InputManager_Game>();
        _computerInputManager = gameObject.AddComponent<InputManager_Computer>();
        _computerInputManager.enabled = false;
        
        if (IdleState != null)
        {
            TransitionToState(IdleState);
        }
        else
        {
            Debug.LogError("IdleState가 초기화되지 않았습니다!");
        }
        
    }
    
    private void OnEnable()
    {
        EventManager_Game.Instance.OnPlayerJump += HandlePlayerJump;
        EventManager_Game.Instance.OnInteraction += HandleInteraction;
        EventManager_Game.Instance.OnUseComputer += HandleUseComputer;
        EventManager_Game.Instance.OnMoveToComputer += HandleMoveToComputer;
    }

    private void OnDisable()
    {
        EventManager_Game.Instance.OnPlayerJump -= HandlePlayerJump;
        EventManager_Game.Instance.OnInteraction -= HandleInteraction;
        EventManager_Game.Instance.OnUseComputer -= HandleUseComputer;
        EventManager_Game.Instance.OnMoveToComputer -= HandleMoveToComputer;
    }
    
    private void Update()
    {
        if (_photonView.IsMine)
        {
            _currentState.UpdateState(this, _playerMovement.InputDirection, _playerMovement.Offset);
        }
    }

    private void FixedUpdate()
    {
        if (_photonView.IsMine)
        {
            _currentState.FixedUpdateState(this, _playerMovement.InputDirection, _playerMovement.Offset);
        }
    }

    public void TransitionToState(IState newState)
    {
        if (_currentState != null)
        {
            _previousState = _currentState;
            _currentState.ExitState(this);
        }

        _currentState = newState;
        _currentState.EnterState(this);
        
        if (_previousState is UseComputerState && _currentState is IdleState)
        {
            Debug.Log("컴퓨터종료 아이들로전환 컴퓨터강제종료이벤트발행");
            EventManager_Game.Instance.InvokeExitComputer();
        }
    }
    
    public bool WasInSprintState()
    {
        return _previousState is SprintState;
    }
    
    private void HandleInteraction()
    {
        if (_currentState.CanInteraction())
        {
            Debug.Log("인터렉션 실행");
            _interaction.RunInteraction();
        }
        else
        {
            Debug.Log("현재 상태에서 Interaction 실행 불가.");
        }
    }

    #region UseComputer Methods
    private void HandleUseComputer(bool isActComputer)
    {
        if (!_photonView.IsMine) return;

        if (EventManager_Game.Instance == null)
        {
            Debug.LogError("EventManager_Game 인스턴스가 null입니다.");
            return;
        }

        if (isActComputer)
        {
            if (_currentState is UseComputerState) return;

            if (UseComputerState == null)
            {
                UseComputerState = new UseComputerState();
            }
            SetInputManager(_computerInputManager);
            TransitionToState(UseComputerState);
            
            EventManager_Game.Instance.InvokeCameraActive(false);
        }
        else
        {
            if (_currentState is IdleState) return;

            SetInputManager(_defaultInputManager);
            TransitionToState(IdleState);
            
            EventManager_Game.Instance.InvokeCameraActive(true);
        }
    }

    private void HandleMoveToComputer(int playerId, Vector3 targetPosition, Quaternion targetRotation)
    {
        _computerInteractionPoint = targetPosition;
        _computerInteractionRotation = targetRotation;
    }
    
    public void StartMoveToComputer()
    {
        StopAllCoroutines();
        StartCoroutine(WalkToComputer(_computerInteractionPoint, _computerInteractionRotation));
    }
    
    private IEnumerator WalkToComputer(Vector3 targetPosition, Quaternion targetRotation)
    {
        float distanceThreshold = 0.1f;
        float moveSpeed = _speedSettings.walkSpeed;
        float rotationSpeed = 1f;
   
        Vector3 startPosition = transform.position;
        Vector3 direction = (targetPosition - startPosition).normalized;
   
        if (_playerAnimator != null)
        {
            _playerAnimator.SetMoveAnim(direction.x, direction.z, 1f);
        }

        while (Vector3.Distance(transform.position, targetPosition) > distanceThreshold)
        {
            Vector3 newPosition = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            Vector3 movement = newPosition - transform.position;
       
            // CharacterController를 사용하여 이동
            _controller.Move(movement);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            yield return null;
        }
   
        transform.position = targetPosition;
        transform.rotation = targetRotation;
   
        if (_playerAnimator != null)
        {
            _playerAnimator.SetMoveAnim(0, 0, 1f);
        }
    }
    
    private void SetInputManager(MonoBehaviour newInputManager)
    {
        if (!_photonView.IsMine) return;

        _defaultInputManager.enabled = false;
        _computerInputManager.enabled = false;

        newInputManager.enabled = true;
    }
    
    #endregion
    public void UpdateAnimator()
    {
        if (!_photonView.IsMine) return;
        
        if (_playerAnimator != null)
        {
            _playerAnimator.SetMoveAnim(_playerMovement.InputDirection.x, _playerMovement.InputDirection.z, _playerMovement.Offset);
        }
    }
    
    private void HandlePlayerJump()
    {
        if (IsGrounded())
        {
            TransitionToState(JumpState);
        }
    }
    
    public bool IsGrounded()
    {
        return _controller.isGrounded;
    }
    
    public void ApplyGravity()
    {
        if (IsGrounded())
        {
            VerticalVelocity = _speedSettings.groundedGravity;
        }
        else
        {
            VerticalVelocity += _speedSettings.gravity * Time.fixedDeltaTime;
        }
    }

    public IState GetCurrentState()
    {
        return _currentState;
    }
    public IState GetPreviousState()
    {
        return _previousState;
    }
}
