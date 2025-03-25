using UnityEngine;
using Photon.Pun;

public class GunShootEffect : MonoBehaviourPun
{
	public GameObject muzzlePosition;

	public void PlayShootEffect()
	{
		Vector3 spawnPosition = muzzlePosition.transform.position + muzzlePosition.transform.forward * 0.01f;
		Quaternion effectRotation = Quaternion.LookRotation(muzzlePosition.transform.forward);
		GameObject muzzleFlash = PhotonNetwork.Instantiate("Particles/GunShoot", muzzlePosition.transform.position, effectRotation);
		if (photonView.IsMine) muzzleFlash.SetActive(false);

		Destroy(muzzleFlash, 2f);
	}
}
