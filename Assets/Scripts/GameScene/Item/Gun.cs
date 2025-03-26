using Photon.Pun;
using UnityEngine;

namespace GameScene.Item
{
    [CreateAssetMenu(fileName = "Gun", menuName = "ScriptableObjects/Gun")]
    public class Gun : global::ItemData
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
                PlayVFXAndSFX(shooterTransform);
                RaycastHit? raycastHit = rayModule.ExecuteRayAction(shooterTransform, currentSpeed);
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

        private void PlayVFXAndSFX(Transform shooterTransform)
        {
            GunShootEffect gunShootEffect = shooterTransform.GetComponentInChildren<GunShootEffect>();
            FPSGunShootEffect gunShootEffectFPS = shooterTransform.GetComponentInChildren<FPSGunShootEffect>();
            GunShootSound gunShootSound = shooterTransform.GetComponentInChildren<GunShootSound>();
            if (gunShootSound != null)
            {
                gunShootSound.PlayShootSound();
            }
            if (gunShootEffect != null && gunShootEffectFPS != null)
            {
                gunShootEffectFPS.PlayShootEffect();
                gunShootEffect.PlayShootEffect();
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