using UnityEngine;
using Photon.Pun;

namespace GameScene.Item
{
    [CreateAssetMenu(fileName = "Knife", menuName = "ScriptableObjects/Knife")]
    
    public class Knife : global::ItemData
    {
        [Header("레이 모듈")]
        public RayModule rayModule;
        private RaycastHit hit;
        public float damage = 10f;

        public override void UseItem()
        {
            Transform shooterTransform = GetShooterTransform();

            PlaySFX(shooterTransform);
            if (rayModule != null && shooterTransform != null)
            {
                RaycastHit? raycastHit = rayModule.ExecuteRayAction(shooterTransform, 0f);
                // 2) 맞은 대상이 있으면 처리
                if (raycastHit.HasValue)
                {
                    RaycastHit hit = raycastHit.Value;
                    
                    // 맞은 대상 PhotonView 찾기
                    string hitTag = hit.collider.gameObject.tag;
                    
                    // 각 부위별 데미지 배수 설정
                    float damageMultiplier = 1f;
                    switch (hitTag)
                    {
                        case "Head":
                            damageMultiplier = 10f;
                            break;
                        case "Body":
                            damageMultiplier = 6f;
                            break;
                        case "Arm":
                            damageMultiplier = 2f;
                            break;
                        case "Leg":
                            damageMultiplier = 2f;
                            break;
                    }
                    
                    // PhotonView는 부위 콜라이더가 없을 수 있으므로, 상위 오브젝트에서 찾아봄.
                    PhotonView targetView = hit.collider.GetComponent<PhotonView>();
                    if (targetView == null)
                    {
                        // 일반적으로 캐릭터의 루트에 PhotonView가 있으므로.
                        targetView = hit.collider.transform.root.GetComponent<PhotonView>();
                    }
                    
                    if (targetView != null)
                    {
                        float finalDamage = damage * damageMultiplier;
                        Debug.Log($"맞은 부위: {hitTag}, 배수 적용 데미지: {finalDamage}");
                        // RPC를 이용해 데미지 적용
                        targetView.RPC("TakeDamage", RpcTarget.All, finalDamage);
                    }
                }
            }
            else
            {
                Debug.LogWarning($"RayModule 또는 ShooterTransform이 유효하지 않습니다.");
            }
        }

        private void PlaySFX(Transform shooterTransform)
        {
            KnifeStabSound knifeStabSound = shooterTransform.GetComponentInChildren<KnifeStabSound>();
            if (knifeStabSound != null)
            {
                knifeStabSound.PlayStabSound();
            }
        }

        private Transform GetShooterTransform()
        {
            GameObject playerObj = PlayerController.Instance.gameObject;
            if (playerObj != null)
                return playerObj.transform;
            return null;
        }
    }
}