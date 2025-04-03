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
                GunBullet gunBullet = shooterTransform.GetComponentInChildren<GunBullet>();
                if (gunBullet != null && gunBullet.TryGunShoot())
                {
                    ShootEffect(shooterTransform);
                    EventManager_Game.Instance.InvokeFPSUseItem(itemName);

                    RaycastHit? raycastHit = rayModule.ExecuteRayAction(shooterTransform, currentSpeed);

                    if (raycastHit.HasValue)
                    {
                        RaycastHit hit = raycastHit.Value;
                        string hitTag = hit.collider.gameObject.tag;

                        float damageMultiplier = 1f;
                        switch (hitTag)
                        {
                            case "Head": damageMultiplier = 10f; break;
                            case "Body": damageMultiplier = 6f; break;
                            case "Arm":  damageMultiplier = 2f; break;
                            case "Leg":  damageMultiplier = 2f; break;
                        }

                        PhotonView targetView = hit.collider.GetComponent<PhotonView>();
                        if (targetView == null)
                            targetView = hit.collider.transform.root.GetComponent<PhotonView>();

                        if (targetView != null)
                        {
                            float finalDamage = damage * damageMultiplier;
                            Debug.Log($"맞은 부위: {hitTag}, 배수 적용 데미지: {finalDamage}");
                            targetView.RPC("TakeDamage", RpcTarget.All, finalDamage);
                        }
                    }
                }
                else
                {
                    MissShootEffect(shooterTransform);
                }
            }
            else
            {
                Debug.LogWarning($"RayModule 또는 ShooterTransform이 유효하지 않습니다.");
            }
        }

        private void ShootEffect(Transform shooterTransform)
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

        private void MissShootEffect(Transform shooterTransform)
        {
            GunShootSound gunShootSound = shooterTransform.GetComponentInChildren<GunShootSound>();
            if (gunShootSound != null)
            {
                gunShootSound.PlayMissShootSound();
            }
        }

        private void CheckBullet(Transform shooterTransform)
        {
            GunBullet gunBullet = shooterTransform.GetComponentInChildren<GunBullet>();
            if (gunBullet != null)
            {
                if (gunBullet.TryGunShoot())
                {
                    ShootEffect(shooterTransform);
                    EventManager_Game.Instance.InvokeFPSUseItem(itemName);
                }
                else
                {
                    MissShootEffect(shooterTransform);
                }
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