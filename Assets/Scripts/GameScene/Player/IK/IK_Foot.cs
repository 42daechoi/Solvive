using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DitzelGames.FastIK;
using UnityEngine;

public class IK_Foot : MonoBehaviour
{
    [Header("Foot IK Settings")]
    [SerializeField] private Transform[] _footTargets;
    [SerializeField] private float _raycastHeight = 0.5f;
    [SerializeField] private float _raycastRadius = 0.05f;
    [SerializeField] private float _raycastDistance = 0.5f;
    [SerializeField] private float _footHeight = 0.05f;
    
    private FastIKFabric[] _footIKs;
    private int _layerMask;
    private bool _initialized = false;

    public void Initialize(int layerMask)
    {
        if (_initialized) return;
        
        _layerMask = layerMask;
        
        // 발 타겟이 설정되지 않은 경우 자동 검색
        if (_footTargets == null || _footTargets.Length == 0)
        {
            AutoDetectFootTargets();
        }
        
        // FastIKFabric 컴포넌트 연결
        if (_footTargets != null && _footTargets.Length > 0)
        {
            InitializeFootIKs();
            _initialized = true;
        }
    }
    
    private void AutoDetectFootTargets()
    {
        var fabrics = GetComponentsInChildren<FastIKFabric>();
        var footTargets = fabrics
            .Where(f => f.Target != null && 
                        (f.Target.name.ToLower().Contains("foot") || 
                         f.Target.name.ToLower().Contains("leg") || 
                         f.Target.name.ToLower().Contains("발")))
            .Select(f => f.Target)
            .ToArray();
            
        if (footTargets.Length > 0)
        {
            _footTargets = footTargets;
            Debug.Log($"자동으로 {_footTargets.Length}개의 발 타겟을 찾았습니다.");
        }
        else
        {
            Debug.LogWarning("발 타겟을 자동으로 찾을 수 없습니다. Inspector에서 수동으로 설정하세요.");
        }
    }
    
    private void InitializeFootIKs()
    {
        var allFabrics = GetComponentsInChildren<FastIKFabric>();
        _footIKs = new FastIKFabric[_footTargets.Length];
        
        for (int i = 0; i < _footTargets.Length; i++)
        {
            _footIKs[i] = allFabrics.FirstOrDefault(ik => ik.Target == _footTargets[i]);
            
            if (_footIKs[i] == null)
            {
                Debug.LogWarning($"발 타겟 {i}({_footTargets[i].name})에 대한 FastIKFabric을 찾을 수 없습니다.");
            }
        }
    }
    
    public void UpdateFootIK()
    {
        if (!_initialized) return;
        
        for (int i = 0; i < _footTargets.Length; i++)
        {
            UpdateFootPosition(_footTargets[i]);
        }
    }
    
    private void UpdateFootPosition(Transform foot)
    {
        if (foot == null) return;

        // 발 위치에서 위로 레이캐스트 시작
        Ray ray = new Ray(foot.position + Vector3.up * _raycastHeight, Vector3.down);
        RaycastHit hitInfo;

        // 구체 캐스트로 더 정확한 지면 감지
        if (Physics.SphereCast(ray, _raycastRadius, out hitInfo, _raycastDistance, _layerMask))
        {
            // 히트 포인트 약간 위에 발 위치
            foot.position = hitInfo.point + Vector3.up * _footHeight;
        }
    }
    
    // 발 타겟들 가져오기
    public Transform[] GetFootTargets()
    {
        return _footTargets;
    }
    void Update()
    {
        
    }
}
