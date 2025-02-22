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
    // Start is called before the first frame update
    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _horizontalHash = Animator.StringToHash("Horizontal");
        _verticalHash = Animator.StringToHash("Vertical");
        _isIdleHash = Animator.StringToHash("IsIdle");
        _isJumpingHash = Animator.StringToHash("IsJumping");
        _isFallingHash = Animator.StringToHash("IsFalling");
    }

    // Update is called once per frame
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
    
    public void SetJumpAnim(bool isJumping, bool isFalling)
    {
        if (_animator == null) return;

        _animator.SetBool(_isJumpingHash, isJumping);
        //_animator.SetBool(_isFallingHash, isFalling);
        
        // 점프/낙하 중에는 Idle 상태 해제
        if (isJumping || isFalling)
        {
            _animator.SetBool(_isIdleHash, false);
        }
    }
}
