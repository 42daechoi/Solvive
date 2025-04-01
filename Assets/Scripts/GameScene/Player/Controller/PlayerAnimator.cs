using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    private Animator _animator;
    
    private int _horizontalHash;
    private int _verticalHash;
    private int _isIdleHash;
    private int _isJumpingHash;
    private int _shootHash;
    
    private int _pistolLayerIndex;
    private int _knifeLayerIndex;
    
    // Start is called before the first frame update
    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _horizontalHash = Animator.StringToHash("Horizontal");
        _verticalHash = Animator.StringToHash("Vertical");
        _isIdleHash = Animator.StringToHash("IsIdle");
        _isJumpingHash = Animator.StringToHash("IsJumping");
        _shootHash = Animator.StringToHash("isShoot");
        
        _pistolLayerIndex = _animator.GetLayerIndex("Pistol");
        _knifeLayerIndex = _animator.GetLayerIndex("Knife");
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
                    float currentWeight = _animator.GetLayerWeight(_pistolLayerIndex);
                    DOTween.To(() => currentWeight, 
                        x => {
                            currentWeight = x;
                            _animator.SetLayerWeight(_pistolLayerIndex, currentWeight);
                        },
                        1f,
                        0.5f);
                }
                break;
            case "Knife":
                if (_knifeLayerIndex != -1)
                {
                    float currentWeight = _animator.GetLayerWeight(_knifeLayerIndex);
                    DOTween.To(() => currentWeight,
                        x => _animator.SetLayerWeight(_knifeLayerIndex, x),
                        1f,
                        0.5f).SetEase(Ease.OutSine);
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
            float currentWeight = _animator.GetLayerWeight(_pistolLayerIndex);
            DOTween.To(() => currentWeight,
                x => _animator.SetLayerWeight(_pistolLayerIndex, x),
                0f,
                0.3f).SetEase(Ease.OutSine);
        }
        
        if (_knifeLayerIndex != -1)
        {
            float currentWeight = _animator.GetLayerWeight(_knifeLayerIndex);
            DOTween.To(() => currentWeight,
                x => _animator.SetLayerWeight(_knifeLayerIndex, x),
                0f,
                0.3f).SetEase(Ease.OutSine);
        }
    }

    public void TriggerShootAnim()
    {
        _animator.SetBool(_shootHash, true);
        DOVirtual.DelayedCall(0.1f, () =>
        {
            _animator.SetBool(_shootHash, false);
        });
    }
}
