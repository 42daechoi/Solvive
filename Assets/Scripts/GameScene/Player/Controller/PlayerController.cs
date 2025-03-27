using Photon.Pun;
using System.Collections;
using Cinemachine;
using RootMotion.FinalIK;
using UnityEngine;

public class PlayerController : MonoBehaviourPun
{
    public static PlayerController Instance { get; private set; }
    
    /* 참조 내역
     * PlayerMovement, PlayerAnimator, CharacterController, PhotonView, Interaction
     * InputManager_Game, InputManager_Computer, MovementSettings, PlayerCamera
     */
    #region Reference
    private PhotonView _photonView;
    private PlayerCamera _playerCamera;
    private PlayerMovement _playerMovement;
    private PlayerAnimator _playerAnimator;
    private CharacterController _controller;
    private Interaction _interaction;
    private InputManager_Game _defaultInputManager;
    private InputManager_Computer _computerInputManager;
    public MovementSettings localSpeedSettings;
    
    [Header("Speed Settings")]
    [SerializeField] private MovementSettings _speedSettings;
    #endregion
    
    //상태 초기화 변수
    #region State
    private IState IdleState { get; set; }
    private IState JumpState { get; set; }
    private IState UseComputerState { get; set; }
    private IState _previousState;
    private IState _currentState;
    #endregion
    
    [Header("Player Camera")] 
    [SerializeField] private CinemachineVirtualCamera fpsCam;
    [SerializeField] private CinemachineVirtualCamera obCam;
    [SerializeField] private Transform obFollow;
    
    [Header("Model")]
    [SerializeField] private GameObject thirdPersonModel;
    [SerializeField] private GameObject firstPersonArms;
    [SerializeField] private GameObject obRender;
    
    public float VerticalVelocity { get; set; }
    private bool _mannequinEscape = false;
    
    private float _currentSpeed;
    private Vector3 _computerInteractionPoint;
    private Quaternion _computerInteractionRotation;
    
    // 호출부
    public MovementSettings SpeedSettings => _speedSettings;
    public CharacterController Controller => _controller;
    public CinemachineVirtualCamera FPSCam => fpsCam;
    public CinemachineVirtualCamera ObserverCam => obCam;
    public Transform ObserverTarget => obFollow;
    public PlayerCamera PlayerCamera => _playerCamera;
    
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
        _playerMovement = gameObject.AddComponent<PlayerMovement>();
        _playerAnimator = gameObject.AddComponent<PlayerAnimator>();
        
        _controller = GetComponent<CharacterController>();
        _photonView = GetComponent<PhotonView>();
        _interaction = GetComponent<Interaction>();
        _playerAnimator = GetComponent<PlayerAnimator>();
        _currentSpeed = _speedSettings.walkSpeed;
        _playerMovement = GetComponent<PlayerMovement>();
        _playerCamera = GetComponent<PlayerCamera>();

        IdleState = new IdleState();
        JumpState = new JumpState();

        _computerInputManager = gameObject.AddComponent<InputManager_Computer>();
        _computerInputManager.enabled = false;
        localSpeedSettings = Instantiate(SpeedSettings);
        
        if (IdleState != null)
        {
            TransitionToState(IdleState);
        }
        else
        {
            Debug.LogError("IdleState가 초기화되지 않았습니다!");
        }
        StartCoroutine(WaitForInputManager());

        if (_photonView.IsMine)
        {
            thirdPersonModel.SetActive(false);
            firstPersonArms.SetActive(true);
        }
        else
        {
            thirdPersonModel.SetActive(true);
            firstPersonArms.SetActive(false);
        }
    }

    private IEnumerator WaitForInputManager()
    {
        while (_defaultInputManager == null)
        {
            GameObject inputManagerObj = GameObject.Find("InputManager");
            if (inputManagerObj != null)
            {
                _defaultInputManager = inputManagerObj.GetComponent<InputManager_Game>();
            }
            yield return new WaitForSeconds(0.1f);
        }
    }


    private void OnEnable()
    {
        EventManager_Game.Instance.OnPlayerJump += HandlePlayerJump;
        EventManager_Game.Instance.OnInteraction += HandleInteraction;
        EventManager_Game.Instance.OnUseComputer += HandleUseComputer;
        EventManager_Game.Instance.OnMoveToComputer += HandleMoveToComputer;
        EventManager_Game.Instance.OnObserverState += HandleObserverState;
        EventManager_Game.Instance.OnAnimationStateChanged += HandleAnimationStateChange;
    }

    private void OnDisable()
    {
        EventManager_Game.Instance.OnPlayerJump -= HandlePlayerJump;
        EventManager_Game.Instance.OnInteraction -= HandleInteraction;
        EventManager_Game.Instance.OnUseComputer -= HandleUseComputer;
        EventManager_Game.Instance.OnMoveToComputer -= HandleMoveToComputer;
        EventManager_Game.Instance.OnObserverState -= HandleObserverState;
        EventManager_Game.Instance.OnAnimationStateChanged -= HandleAnimationStateChange;
    }
    
    private void Update()
    {
        if (_photonView.IsMine)
        {
            _currentState.UpdateState(this, _playerMovement.InputDirection, _playerMovement.Offset);
            //if (Time.time % 1f < 0.02f)
            //{
            //    SavePlayerPosition();
            //}
        }
    }

    private void FixedUpdate()
    {
        if (_photonView.IsMine)
        {
            _currentState.FixedUpdateState(this, _playerMovement.InputDirection, _playerMovement.Offset, _mannequinEscape);
        }
    }

    //private void SavePlayerPosition()
    //{
    //    ExitGames.Client.Photon.Hashtable playerProps = PhotonNetwork.LocalPlayer.CustomProperties;
    //    playerProps["LastPosition"] = transform.position;
    //    PhotonNetwork.LocalPlayer.SetCustomProperties(playerProps);
    //}


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
            EventManager_Game.Instance.InvokeExitComputer(photonView.ViewID);
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
        bool isGrounded = IsGrounded();
        bool isJumping = VerticalVelocity > 0.1f;
        _playerAnimator.SetJumpAnim(isJumping, isGrounded); 
        _playerAnimator.SetMoveAnim(_playerMovement.InputDirection.x, _playerMovement.InputDirection.z, _playerMovement.Offset);
    }
    
    private void HandleAnimationStateChange(string state)
    {
        if (!_photonView.IsMine) return;
        
        if (_playerAnimator != null)
        {
            _playerAnimator.SetAnimationState(state);
        }
    }

    private void HandleObserverState(int viewID)
    {
        Debug.Log($"PlayerController: ObserverState 이벤트 수신 - 내 ViewID: {_photonView.ViewID}, 이벤트 ViewID: {viewID}");
        if (_photonView.ViewID == viewID)
        {
            obRender.SetActive(false);
            TransitionToState(new ObserverState());
            GetComponent<Inventory>().enabled = false;
            GetComponent<HeldItem>().enabled = false;
            GetComponent<EquipItem>().enabled = false;
            GetComponent<PlayerHealth>().enabled = false;
            GetComponent<Animator>().enabled = false;
            GetComponent<PlayerObserver>().enabled = false;
            GetComponent<FullBodyBipedIK>().enabled = false;
            GetComponent<PhotonAnimatorView>().enabled = false;
            GetComponent<PhotonTransformView>().enabled = false;
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
        float characterHeight = _controller.height;
        Vector3 rayStart = transform.position + Vector3.up * (characterHeight * 0.5f);
        float rayLength = characterHeight * 0.55f;
        Vector3 boxSize = new Vector3(_controller.radius, 0.1f, _controller.radius);
    
        int layerMask = ~(LayerMask.GetMask("Player", "Hitbox"));

        RaycastHit hit;
        return Physics.BoxCast(rayStart, boxSize * 0.5f, Vector3.down, out hit, transform.rotation, rayLength, layerMask);
    }
    
    public void ApplyGravity()
    {
        if (IsGrounded() && VerticalVelocity < 0)
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

    public PhotonView GetPhotonView()
    {
        return _photonView;
    }
    
    public void MannequinEscapeTrigger()
    {
        Debug.Log("MannequinEscapeTrigger");
        if(!_photonView.IsMine)
            return;
        localSpeedSettings.walkSpeed += 1f;
        localSpeedSettings.sprintSpeed += 1f;
        _mannequinEscape = true;
    }
    
    [PunRPC]
    public void UpdateMannequinPosition(Vector3 newPosition, Quaternion newRotation)
    {
        CharacterController cc = GetComponent<CharacterController>();
        if (cc != null)
        {
            cc.enabled = false;
        }

        transform.position = newPosition;
        transform.rotation = newRotation;
        if (cc != null)
        {
            cc.enabled = true;
        }
    }
}
