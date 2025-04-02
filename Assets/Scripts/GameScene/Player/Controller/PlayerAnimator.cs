using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    private Animator animator;
    
    private int horizontalHash;
    private int verticalHash;
    private int isIdleHash;
    private int isCrouchHash;
    private int isJumpingHash;
    private int shootHash;
    
    private int pistolLayerIndex;
    private int knifeLayerIndex;
    private int computerLayerIndex;
    private int cardKeyLayerIndex;
    private int batteryLayerIndex;
    private int flashLayerIndex;
    
    // Start is called before the first frame update
    private void Awake()
    {
        animator = GetComponent<Animator>();
        horizontalHash = Animator.StringToHash("Horizontal");
        verticalHash = Animator.StringToHash("Vertical");
        isIdleHash = Animator.StringToHash("IsIdle");
        isCrouchHash = Animator.StringToHash("isCrouch");
        isJumpingHash = Animator.StringToHash("IsJumping");
        shootHash = Animator.StringToHash("isShoot");
        
        pistolLayerIndex = animator.GetLayerIndex("Gun");
        knifeLayerIndex = animator.GetLayerIndex("Knife");
        computerLayerIndex = animator.GetLayerIndex("Computer");
        cardKeyLayerIndex = animator.GetLayerIndex("Keycard");
        batteryLayerIndex = animator.GetLayerIndex("Battery");
        flashLayerIndex = animator.GetLayerIndex("Flashlight");
    }
    
    public void SetMoveAnim(float horizontal, float vertical, float offset)
    {
        if (animator == null) return;
        
        float scaledHorizontal = horizontal * offset;
        float scaledVertical = vertical * offset;
        
        animator.SetFloat(horizontalHash, scaledHorizontal);
        animator.SetFloat(verticalHash, scaledVertical);

        bool isIdle = Mathf.Abs(scaledHorizontal) < 0.1f && 
                      Mathf.Abs(scaledVertical) < 0.1f;
        animator.SetBool(isIdleHash, isIdle);
    }

    public void SetCrouch(bool isCrouch)
    {
        if (animator == null) return;
        animator.SetBool(isCrouchHash, isCrouch);
    }
    
    public void SetJumpAnim(bool isJumping, bool isGrounded)
    {
        if (animator == null) return;

        animator.SetBool(isJumpingHash, isJumping);
    }

    public void SetAnimationState(string state)
    {
        if (animator == null) return;

        ResetWeaponLayerWeights();

        switch (state)
        {
            case "Gun":
                if (pistolLayerIndex != -1)
                {
                    float currentWeight = animator.GetLayerWeight(pistolLayerIndex);
                    DOTween.To(() => currentWeight, 
                        x => {
                            currentWeight = x;
                            animator.SetLayerWeight(pistolLayerIndex, currentWeight);
                        },
                        1f,
                        0.5f);
                }
                break;
            case "Knife":
                if (knifeLayerIndex != -1)
                {
                    float currentWeight = animator.GetLayerWeight(knifeLayerIndex);
                    DOTween.To(() => currentWeight,
                        x => animator.SetLayerWeight(knifeLayerIndex, x),
                        1f,
                        0.5f).SetEase(Ease.OutSine);
                }
                break;
            case "Keycard":
                if (cardKeyLayerIndex != -1)
                {
                    float currentWeight = animator.GetLayerWeight(cardKeyLayerIndex);
                    DOTween.To(() => currentWeight,
                        x => animator.SetLayerWeight(cardKeyLayerIndex, x),
                        1f,
                        0.5f).SetEase(Ease.OutSine);
                }
                break;
            case "Battery":
                if (batteryLayerIndex != -1)
                {
                    float currentWeight = animator.GetLayerWeight(batteryLayerIndex);
                    DOTween.To(() => currentWeight,
                        x => animator.SetLayerWeight(batteryLayerIndex, x),
                        1f,
                        0.5f).SetEase(Ease.OutSine);
                }
                break;
            case "Flashlight":
                if (flashLayerIndex != -1)
                {
                    float currentWeight = animator.GetLayerWeight(flashLayerIndex);
                    DOTween.To(() => currentWeight,
                        x => animator.SetLayerWeight(flashLayerIndex, x),
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
        if (pistolLayerIndex != -1)
        {
            float currentWeight = animator.GetLayerWeight(pistolLayerIndex);
            DOTween.To(() => currentWeight,
                x => animator.SetLayerWeight(pistolLayerIndex, x),
                0f,
                0.3f).SetEase(Ease.OutSine);
        }
        
        if (knifeLayerIndex != -1)
        {
            float currentWeight = animator.GetLayerWeight(knifeLayerIndex);
            DOTween.To(() => currentWeight,
                x => animator.SetLayerWeight(knifeLayerIndex, x),
                0f,
                0.3f).SetEase(Ease.OutSine);
        }
        if (cardKeyLayerIndex != -1)
        {
            float currentWeight = animator.GetLayerWeight(cardKeyLayerIndex);
            DOTween.To(() => currentWeight,
                x => animator.SetLayerWeight(cardKeyLayerIndex, x),
                0f,
                0.3f).SetEase(Ease.OutSine);
        }

        if (batteryLayerIndex != -1)
        {
            float currentWeight = animator.GetLayerWeight(batteryLayerIndex);
            DOTween.To(() => currentWeight,
                x => animator.SetLayerWeight(batteryLayerIndex, x),
                0f,
                0.3f).SetEase(Ease.OutSine);
        }
        if (flashLayerIndex != -1)
        {
            float currentWeight = animator.GetLayerWeight(flashLayerIndex);
            DOTween.To(() => currentWeight,
                x => animator.SetLayerWeight(flashLayerIndex, x),
                0f,
                0.3f).SetEase(Ease.OutSine);
        }
    }

    public void TriggerShootAnim()
    {
        animator.SetBool(shootHash, true);
        DOVirtual.DelayedCall(0.1f, () =>
        {
            animator.SetBool(shootHash, false);
        });
    }
    
    public void EnterComputerAnim()
    {
        if (computerLayerIndex != -1)
        {
            float currentWeight = animator.GetLayerWeight(computerLayerIndex);
            DOTween.To(() => currentWeight,
                x => animator.SetLayerWeight(computerLayerIndex, x),
                1f,
                0.3f).SetEase(Ease.OutSine);
        }
    }
    
    public void ExitComputerAnim()
    {
        if (computerLayerIndex != -1)
        {
            float currentWeight = animator.GetLayerWeight(computerLayerIndex);
            DOTween.To(() => currentWeight,
                x => animator.SetLayerWeight(computerLayerIndex, x),
                0f,
                0.3f).SetEase(Ease.OutSine);
        }
    }
}
