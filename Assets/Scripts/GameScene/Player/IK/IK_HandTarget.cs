using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class IK_HandTarget : MonoBehaviourPun
{
    [SerializeField] private Transform rightHandTarget;     // 오른손 타겟
    [SerializeField] private Transform leftHandTarget;      // 왼손 타겟
    [SerializeField] private PlayerCamera playerCamera;
    
    void Start()
    {
        if (!photonView.IsMine) return;
        
        // PlayerCamera 찾기
        if (playerCamera == null)
        {
            playerCamera = GetComponentInChildren<PlayerCamera>();
        }
    }
    
    void LateUpdate()
    {
        if (!photonView.IsMine || playerCamera == null) return;
        
        // 카메라의 방향 벡터 계산
        float xRotation = playerCamera.xAxis.Value;
        float yRotation = playerCamera.yAxis.Value;
        
        // 카메라 방향으로 회전 계산
        Quaternion cameraRotation = Quaternion.Euler(yRotation, xRotation, 0);
        
        // 손 타겟의 회전만 업데이트
        if (rightHandTarget != null)
        {
            rightHandTarget.rotation = cameraRotation;
        }
        
        if (leftHandTarget != null)
        {
            leftHandTarget.rotation = cameraRotation;
        }
    }
    
}
