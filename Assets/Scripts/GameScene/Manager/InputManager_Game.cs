using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using System.Collections;

public class InputManager_Game : MonoBehaviour
{
    private PlayerController _playerController;
    private bool isPlayingGame = true;

    private bool prevSprint = false;
    private bool prevCrouch = false;
    private bool prevVoice = false;
    private Vector2 prevMove = Vector2.zero;

    void Start()
    {
        StartCoroutine(WaitForPlayerController());
    }

    private IEnumerator WaitForPlayerController()
    {
        while (_playerController == null)
        {
            _playerController = PlayerController.Instance;
            if (_playerController == null)
            {
                Debug.Log("PlayerController가 아직 초기화되지 않았습니다. 대기 중...");
                yield return new WaitForSeconds(0.1f);
            }
        }
        Debug.Log("PlayerController 초기화 완료!");
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        isPlayingGame = hasFocus;
        if (!hasFocus)
        {
            EventManager_Game.Instance.InvokePlayerMove(0f, 0f);
            EventManager_Game.Instance.InvokeSprint(false);
        }
    }

    void Update()
    {
        if (!isPlayingGame || _playerController == null)
            return;

        if (_playerController.GetCurrentState() is UseComputerState)
            return;

        HandleMovementInput();
        HandleStateInput();
        HandleActionInput();
    }

    private void HandleMovementInput()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector2 currentMove = new Vector2(horizontal, vertical);

        if (Vector2.Distance(currentMove, prevMove) > 0.01f)
        {
            prevMove = currentMove;
            EventManager_Game.Instance.InvokePlayerMove(horizontal, vertical);
        }
    }

    private void HandleStateInput()
    {
        HandleKeyState(KeyCode.LeftShift, ref prevSprint, EventManager_Game.Instance.InvokeSprint);
        HandleKeyState(KeyCode.LeftControl, ref prevCrouch, EventManager_Game.Instance.InvokeCrouch);
        HandleKeyState(KeyCode.V, ref prevVoice, EventManager_Game.Instance.InvokeVoiceOn);
    }

    private void HandleKeyState(KeyCode key, ref bool prevState, System.Action<bool> action)
    {
        bool currentState = Input.GetKey(key);
        if (currentState != prevState)
        {
            prevState = currentState;
            action(currentState);
        }
    }

    private void HandleActionInput()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            EventManager_Game.Instance.InvokePlayerJump();
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            EventManager_Game.Instance.InvokeInteraction();
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            EventManager_Game.Instance.InvokeDropItem();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            EventManager_Game.Instance.InvokeEscUI();
            EscUI.Instance.esctoogle = !EscUI.Instance.esctoogle;
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (!EscUI.Instance.escUI && !EndGameUI.Instance.EndGameToggle)
            {
                EventManager_Game.Instance.InvokeUseItem();
            }
        }

        HandleHeldItemInput();
    }

    private void HandleHeldItemInput()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) EventManager_Game.Instance.InvokeHeldItem(1);
        else if (Input.GetKeyDown(KeyCode.Alpha2)) EventManager_Game.Instance.InvokeHeldItem(2);
        else if (Input.GetKeyDown(KeyCode.Alpha3)) EventManager_Game.Instance.InvokeHeldItem(3);
        else if (Input.GetKeyDown(KeyCode.Alpha4)) EventManager_Game.Instance.InvokeHeldItem(4);
        else if (Input.GetKeyDown(KeyCode.Alpha5)) EventManager_Game.Instance.InvokeHeldItem(5);
    }
}
