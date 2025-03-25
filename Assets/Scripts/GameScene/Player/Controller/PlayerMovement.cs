using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Vector3 _inputDirection;
    private bool _isSprinting;
    private float _offset;

    public Vector3 InputDirection => _inputDirection;
    public float Offset => _offset;
    public bool IsSprinting => _isSprinting;
    
    private void OnEnable()
    {
        EventManager_Game.Instance.OnPlayerMove += UpdateMoveInput;
        EventManager_Game.Instance.OnPlayerSprintWithStamina += UpdateSprintInput;
    }

    private void OnDisable()
    {
        EventManager_Game.Instance.OnPlayerMove -= UpdateMoveInput;
        EventManager_Game.Instance.OnPlayerSprintWithStamina -= UpdateSprintInput;
    }
    
    private void UpdateMoveInput(float horizontal, float vertical)
    {
        _inputDirection = new Vector3(horizontal, 0, vertical);
        _offset = 0.5f + (_isSprinting ? 0.5f : 0f);
    }

    private void UpdateSprintInput(bool isSprinting)
    {
        _isSprinting = isSprinting;
    }
}
