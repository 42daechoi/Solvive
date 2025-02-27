using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class HeadToCamera : MonoBehaviourPun
{
    [SerializeField] private Transform headBone;            // neck.x를 할당
    [SerializeField] private Transform targetLook;          // 기존에 있는 TargetLook 할당
    [SerializeField] private float lookDistance = 1f;       // 시선이 향할 거리
    
    [Tooltip("카메라의 방향을 가져올 PlayerCamera 컴포넌트")]
    [SerializeField] private PlayerCamera playerCamera;
    
    private IK_Head _ikHead;
    private bool _isHeadLookActive = true;

    void Start()
    {
        if (!photonView.IsMine)
        {
            // 자신의 캐릭터가 아니면 이 컴포넌트를 비활성화
            enabled = false;
            return;
        }

        if (playerCamera == null)
        {
            playerCamera = GetComponentInChildren<PlayerCamera>();
        }
    }

    private void OnEnable()
    {
        if (EventManager_Game.Instance != null)
        {
            EventManager_Game.Instance.OnCameraActive += HandleCameraActive;
        }
    }
    
    private void OnDisable()
    {
        if (EventManager_Game.Instance != null)
        {
            EventManager_Game.Instance.OnCameraActive -= HandleCameraActive;
        }
    }
    
    void LateUpdate()
    {
        if (targetLook == null || playerCamera == null || !_isHeadLookActive) return;
        
        // 카메라의 상하 회전 각도 가져오기
        float pitchAngle = playerCamera.yAxis.Value;
        
        // 캐릭터의 전방 벡터와 오른쪽 벡터
        Vector3 forward = transform.forward;
        Vector3 right = transform.right;
        
        // 상하 회전만 적용한 방향 계산
        Vector3 lookDirection = Quaternion.AngleAxis(pitchAngle, right) * forward;
        
        // 타겟 위치 설정
        targetLook.position = headBone.position + lookDirection * lookDistance;
    }
    
    private void HandleCameraActive(bool isActive)
    {
        _isHeadLookActive = isActive;
    }
}