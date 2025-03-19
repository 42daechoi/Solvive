using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    private Animator _animator;
    private int _horizontalHash;
    private int _verticalHash;
    private int _isIdleHash;
    private int _isJumpingHash;
    private int _isFallingHash;
    private int _isGroundedHash;

    private int _pistolLayerIndex;
    
    // Start is called before the first frame update
    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _horizontalHash = Animator.StringToHash("Horizontal");
        _verticalHash = Animator.StringToHash("Vertical");
        _isIdleHash = Animator.StringToHash("IsIdle");
        _isJumpingHash = Animator.StringToHash("IsJumping");
        _isGroundedHash = Animator.StringToHash("IsGrounded");
        
        _pistolLayerIndex = _animator.GetLayerIndex("Pistol");
    }
    
    public void SetMoveAnim(float horizontal, float vertical, float offset)
    {
        if (_animator == null) return;
        
        float scaledHorizontal = horizontal * offset;
        float scaledVertical = vertical * offset;
        
        _animator.SetFloat(_horizontalHash, scaledHorizontal);
        _animator.SetFloat(_verticalHash, scaledVertical);

        bool isIdle = Mathf.Abs(scaledHorizontal) < 0.1f && 
                      Mathf.Abs(scaledVertical) < 0.1f;
        _animator.SetBool(_isIdleHash, isIdle);
    }
    
    public void SetJumpAnim(bool isJumping, bool isGrounded)
    {
        if (_animator == null) return;

        _animator.SetBool(_isJumpingHash, isJumping);
        _animator.SetBool(_isGroundedHash, isGrounded);
    }

    public void SetAnimationState(string state)
    {
        if (_animator == null) return;

        ResetWeaponLayerWeights();

        switch (state)
        {
            case "Gun":
                if (_pistolLayerIndex != -1)
                {
                    _animator.SetLayerWeight(_pistolLayerIndex, 1f);
                }

                break;
            case "Default":
            default:
                break;
        }
    }

    private void ResetWeaponLayerWeights()
    {
        if (_pistolLayerIndex != -1)
        {
            _animator.SetLayerWeight(_pistolLayerIndex, 0f);
        }
    }
}
