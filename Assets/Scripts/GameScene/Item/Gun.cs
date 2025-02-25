using Photon.Pun;
using UnityEngine;

namespace GameScene.Item
{
    [CreateAssetMenu(fileName = "Gun", menuName = "ScriptableObjects/Gun")]
    public class Gun : global::Item
    {
        [Header("총 레이 모듈")]
        public RayModule rayModule;

        [Header("현재 이동 스피드 (예시용)")]
        public float currentSpeed;
        
        [Header("데미지")]
        public float damage = 10f;

        private RaycastHit hit;
        
        
        public override void UseItem()
        {
            Transform shooterTransform = GetShooterTransform();

            if (rayModule != null && shooterTransform != null)
            {
                RaycastHit? raycastHit = rayModule.ExecuteRayAction(shooterTransform, currentSpeed);
                // 2) 맞은 대상이 있으면 처리
                if (raycastHit.HasValue)
                {
                    RaycastHit hit = raycastHit.Value;
                    
                    // 맞은 대상 PhotonView 찾기
                    PhotonView targetView = hit.collider.GetComponent<PhotonView>();
                    if (targetView != null)
                    {
                        Debug.Log("총 맞음");
                        Debug.Log($"Raycast hit object: {hit.collider.gameObject.name}");
                        // 3) TakeDamage RPC 호출
                        targetView.RPC("TakeDamage", RpcTarget.All, damage);
                    }
                }
            }
            else
            {
                Debug.LogWarning($"RayModule 또는 ShooterTransform이 유효하지 않습니다.");
            }
        }

        private Transform GetShooterTransform()
        {
            GameObject playerObj = GameObject.FindWithTag("Player");
            if (playerObj != null)
                return playerObj.transform;
            return null;
        }
    }
}