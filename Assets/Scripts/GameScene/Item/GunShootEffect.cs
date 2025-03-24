using UnityEngine;

public class GunShootEffect : MonoBehaviour
{
    public GameObject shootEffect;
    public GameObject muzzlePosition;

    public void PlayShootEffect()
    {
        if (shootEffect != null)
        {
            Vector3 spawnPosition = muzzlePosition.transform.position + muzzlePosition.transform.forward * 0.01f;
            Quaternion effectRotation = Quaternion.LookRotation(muzzlePosition.transform.forward);
            GameObject muzzleFlash = Instantiate(shootEffect, muzzlePosition.transform.position, effectRotation);

            Destroy(muzzleFlash, 2f);
        }
    }
}
