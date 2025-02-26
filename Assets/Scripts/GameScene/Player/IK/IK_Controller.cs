using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IK_Controller : MonoBehaviour
{
    [Header("IK Components")]
    [SerializeField] private IK_Foot _footIK;
    
    public IK_Foot FootIK => _footIK;
    
    private bool _initialized = false;
    private int _layerMask;
    private void Awake()
    {
        // PlayerController와 동일한 레이어 마스크 사용
        _layerMask = ~(LayerMask.GetMask("Player", "Hitbox"));
    }
    
    private void Start()
    {
        InitializeIK();
    }
    
    public void InitializeIK()
    {
        if (_initialized) return;
        
        // 발 IK 컴포넌트 가져오기 또는 생성
        if (_footIK == null)
        {
            _footIK = GetComponent<IK_Foot>();
            if (_footIK == null) _footIK = gameObject.AddComponent<IK_Foot>();
        }
        
        // 각 컴포넌트 초기화
        _footIK.Initialize(_layerMask);
        
        _initialized = true;
    }

    public void UpdateAllIK(Camera playerCamera = null)
    {
        if (!_initialized) InitializeIK();

        // 발 IK 업데이트
        if (_footIK != null)
        {
            _footIK.UpdateFootIK();
        }
    }
}
