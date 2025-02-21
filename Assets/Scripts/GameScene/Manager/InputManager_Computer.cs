using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager_Computer : MonoBehaviour
{
    private PlayerController _playerController;

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

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            EventManager_Game.Instance.InvokeUseComputer(false);
        }
        if (Input.GetKeyDown (KeyCode.Backspace))
        {
            EventManager_Game.Instance.InvokeTypeBackspaceAtComputer();
        }
        NumberInput();
    }

    private void NumberInput()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0)) EventManager_Game.Instance.InvokeTypeNumberAtComputer('0');
        if (Input.GetKeyDown(KeyCode.Alpha1)) { EventManager_Game.Instance.InvokeTypeNumberAtComputer('1');}
        if (Input.GetKeyDown(KeyCode.Alpha2)) EventManager_Game.Instance.InvokeTypeNumberAtComputer('2');
        if (Input.GetKeyDown(KeyCode.Alpha3)) EventManager_Game.Instance.InvokeTypeNumberAtComputer('3');
        if (Input.GetKeyDown(KeyCode.Alpha4)) EventManager_Game.Instance.InvokeTypeNumberAtComputer('4');
        if (Input.GetKeyDown(KeyCode.Alpha5)) EventManager_Game.Instance.InvokeTypeNumberAtComputer('5');
        if (Input.GetKeyDown(KeyCode.Alpha6)) EventManager_Game.Instance.InvokeTypeNumberAtComputer('6');
        if (Input.GetKeyDown(KeyCode.Alpha7)) EventManager_Game.Instance.InvokeTypeNumberAtComputer('7');
        if (Input.GetKeyDown(KeyCode.Alpha8)) EventManager_Game.Instance.InvokeTypeNumberAtComputer('8');
        if (Input.GetKeyDown(KeyCode.Alpha9)) EventManager_Game.Instance.InvokeTypeNumberAtComputer('9');

        if (Input.GetKeyDown(KeyCode.Keypad0)) EventManager_Game.Instance.InvokeTypeNumberAtComputer('0');
        if (Input.GetKeyDown(KeyCode.Keypad1)) EventManager_Game.Instance.InvokeTypeNumberAtComputer('1');
        if (Input.GetKeyDown(KeyCode.Keypad2)) EventManager_Game.Instance.InvokeTypeNumberAtComputer('2');
        if (Input.GetKeyDown(KeyCode.Keypad3)) EventManager_Game.Instance.InvokeTypeNumberAtComputer('3');
        if (Input.GetKeyDown(KeyCode.Keypad4)) EventManager_Game.Instance.InvokeTypeNumberAtComputer('4');
        if (Input.GetKeyDown(KeyCode.Keypad5)) EventManager_Game.Instance.InvokeTypeNumberAtComputer('5');
        if (Input.GetKeyDown(KeyCode.Keypad6)) EventManager_Game.Instance.InvokeTypeNumberAtComputer('6');
        if (Input.GetKeyDown(KeyCode.Keypad7)) EventManager_Game.Instance.InvokeTypeNumberAtComputer('7');
        if (Input.GetKeyDown(KeyCode.Keypad8)) EventManager_Game.Instance.InvokeTypeNumberAtComputer('8');
        if (Input.GetKeyDown(KeyCode.Keypad9)) EventManager_Game.Instance.InvokeTypeNumberAtComputer('9');
    }
}
