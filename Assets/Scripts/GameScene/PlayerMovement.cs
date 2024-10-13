using System;
using Photon.Pun;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private PhotonView photonView;
    
    [Header("Character Input Values")]
    public Vector2 move;
    public Vector2 look;
    public bool jump;
    public bool sprint;

    [Header("Movement Settings")]
    public bool analogMovement;

    [Header("Mouse Cursor Settings")]
    public bool cursorLocked = true;
    public bool cursorInputForLook = true;

    [SerializeField] private float speed = 4.0f;
    [SerializeField] private float rotationSpeed = 10.0f;
    
    private Vector3 _moveDirection;
    private Quaternion _targetRotation;
    
#if ENABLE_INPUT_SYSTEM
    public void OnMove(InputValue value)
    {
        MoveInput(value.Get<Vector2>());
    }

    public void OnLook(InputValue value)
    {
        if(cursorInputForLook)
        {
            LookInput(value.Get<Vector2>());
        }
    }

    public void OnJump(InputValue value)
    {
        JumpInput(value.isPressed);
    }

    public void OnSprint(InputValue value)
    {
        SprintInput(value.isPressed);
    }
#endif
    private void Start()
    {
        photonView = GetComponent<PhotonView>();

        if (!photonView.IsMine)
        {
            enabled = false;
        }
    }

    private void Update()
    {
        if (photonView.IsMine)
        {
            HandleMovement();
            HandleRotation();
        }
    }
    
    private void HandleMovement()
    {
        // PlayerInput으로 받은 이동 입력을 이용해 이동 방향 계산
        _moveDirection = new Vector3(move.x, 0, move.y).normalized;

        if (_moveDirection != Vector3.zero)
        {
            _targetRotation = Quaternion.LookRotation(_moveDirection);

            // 캐릭터 이동 처리
            transform.Translate(_moveDirection * speed * Time.deltaTime, Space.World);

            // 이동한 위치 및 회전 각도를 다른 클라이언트에 동기화
            photonView.RPC("Move", RpcTarget.All, transform.position, transform.rotation);
        }
    }
    
    private void HandleRotation()
    {
        // Look 입력을 통해 캐릭터 좌우 회전 처리
        float mouseX = look.x * rotationSpeed * Time.deltaTime;
        transform.Rotate(Vector3.up * mouseX);
    }

    public void MoveInput(Vector2 newMoveDirection)
    {
        move = newMoveDirection;
    } 

    public void LookInput(Vector2 newLookDirection)
    {
        look = newLookDirection;
    }

    public void JumpInput(bool newJumpState)
    {
        jump = newJumpState;
    }

    public void SprintInput(bool newSprintState)
    {
        sprint = newSprintState;
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        SetCursorState(cursorLocked);
    }

    private void SetCursorState(bool newState)
    {
        Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
    }

    [PunRPC]
    public void Move(Vector3 newPosition, Quaternion newRotation)
    {
        transform.position = newPosition;
        transform.rotation = newRotation;
    }
}
