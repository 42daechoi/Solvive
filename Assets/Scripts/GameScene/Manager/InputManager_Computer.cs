using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager_Computer : MonoBehaviour
{
    private PlayerController _playerController;
    private int playerViewID;

    void Start()
    {
        StopAllCoroutines();
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
        playerViewID = _playerController.GetPhotonView().ViewID;
        Debug.Log("PlayerController 초기화 완료!");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            EventManager_Game.Instance.InvokeUseComputer(false);
        }

        ComputerInput();
    }

    private void ComputerInput()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0)) EventManager_Game.Instance.InvokeTypeNumberAtComputer('0', playerViewID);
        if (Input.GetKeyDown(KeyCode.Alpha1)) EventManager_Game.Instance.InvokeTypeNumberAtComputer('1', playerViewID);
        if (Input.GetKeyDown(KeyCode.Alpha2)) EventManager_Game.Instance.InvokeTypeNumberAtComputer('2', playerViewID);
        if (Input.GetKeyDown(KeyCode.Alpha3)) EventManager_Game.Instance.InvokeTypeNumberAtComputer('3', playerViewID);
        if (Input.GetKeyDown(KeyCode.Alpha4)) EventManager_Game.Instance.InvokeTypeNumberAtComputer('4', playerViewID);
        if (Input.GetKeyDown(KeyCode.Alpha5)) EventManager_Game.Instance.InvokeTypeNumberAtComputer('5', playerViewID);
        if (Input.GetKeyDown(KeyCode.Alpha6)) EventManager_Game.Instance.InvokeTypeNumberAtComputer('6', playerViewID);
        if (Input.GetKeyDown(KeyCode.Alpha7)) EventManager_Game.Instance.InvokeTypeNumberAtComputer('7', playerViewID);
        if (Input.GetKeyDown(KeyCode.Alpha8)) EventManager_Game.Instance.InvokeTypeNumberAtComputer('8', playerViewID);
        if (Input.GetKeyDown(KeyCode.Alpha9)) EventManager_Game.Instance.InvokeTypeNumberAtComputer('9', playerViewID);

        if (Input.GetKeyDown(KeyCode.Keypad0)) EventManager_Game.Instance.InvokeTypeNumberAtComputer('0', playerViewID);
        if (Input.GetKeyDown(KeyCode.Keypad1)) EventManager_Game.Instance.InvokeTypeNumberAtComputer('1', playerViewID);
        if (Input.GetKeyDown(KeyCode.Keypad2)) EventManager_Game.Instance.InvokeTypeNumberAtComputer('2', playerViewID);
        if (Input.GetKeyDown(KeyCode.Keypad3)) EventManager_Game.Instance.InvokeTypeNumberAtComputer('3', playerViewID);
        if (Input.GetKeyDown(KeyCode.Keypad4)) EventManager_Game.Instance.InvokeTypeNumberAtComputer('4', playerViewID);
        if (Input.GetKeyDown(KeyCode.Keypad5)) EventManager_Game.Instance.InvokeTypeNumberAtComputer('5', playerViewID);
        if (Input.GetKeyDown(KeyCode.Keypad6)) EventManager_Game.Instance.InvokeTypeNumberAtComputer('6', playerViewID);
        if (Input.GetKeyDown(KeyCode.Keypad7)) EventManager_Game.Instance.InvokeTypeNumberAtComputer('7', playerViewID);
        if (Input.GetKeyDown(KeyCode.Keypad8)) EventManager_Game.Instance.InvokeTypeNumberAtComputer('8', playerViewID);
        if (Input.GetKeyDown(KeyCode.Keypad9)) EventManager_Game.Instance.InvokeTypeNumberAtComputer('9', playerViewID);

        if (Input.GetKeyDown(KeyCode.Backspace)) EventManager_Game.Instance.InvokeTypeBackspaceAtComputer(playerViewID);
    }

}
