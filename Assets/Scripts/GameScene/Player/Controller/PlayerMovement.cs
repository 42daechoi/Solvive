using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Vector3 _inputDirection;
    private bool _isSprinting;
    private bool _isCrouch;
    private float _offset;

    public Vector3 InputDirection => _inputDirection;
    public float Offset => _offset;
    public bool IsSprinting => _isSprinting;
    public bool IsCrouch => _isCrouch;
    
    private void OnEnable()
    {
        EventManager_Game.Instance.OnPlayerMove += UpdateMoveInput;
        EventManager_Game.Instance.OnPlayerSprintWithStamina += UpdateSprintInput;
        EventManager_Game.Instance.OnPlayerCrouch += HandlePlayerCrouch;
    }

    private void OnDisable()
    {
        EventManager_Game.Instance.OnPlayerMove -= UpdateMoveInput;
        EventManager_Game.Instance.OnPlayerSprintWithStamina -= UpdateSprintInput;
        EventManager_Game.Instance.OnPlayerCrouch -= HandlePlayerCrouch;
    }
    
    private void UpdateMoveInput(float horizontal, float vertical)
    {
        _inputDirection = new Vector3(horizontal, 0, vertical);
        UpdateOffset();
    }

    private void UpdateSprintInput(bool isSprinting)
    {
        _isSprinting = isSprinting;
        UpdateOffset();
    }

    private void HandlePlayerCrouch(bool isCrouch)
    {
        _isCrouch = isCrouch;
        UpdateOffset();
    }

    private void UpdateOffset()
    {
        if (_isCrouch)
            _offset = 0.2f;
        else if (_isSprinting)
            _offset = 1.0f;
        else
            _offset = 0.5f;
    }
}
