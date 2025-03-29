using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class PlayerFPSAnimator : MonoBehaviour
{
	[SerializeField] private WeaponPoseDatabase weaponPoseDB;
	
	[SerializeField] private Transform r_ArmStrech;
	[SerializeField] private Transform r_Forearm;
	[SerializeField] private Transform r_Hand;
	[SerializeField] private Transform l_ArmStrech;
	[SerializeField] private Transform l_Forearm;
	[SerializeField] private Transform l_Hand;
	void Start()
	{
		
	}

	private void OnEnable()
	{
		EventManager_Game.Instance.OnFPSUseItem += HandleFPSUseItem;
	}

	private void OnDisable()
	{
		EventManager_Game.Instance.OnFPSUseItem -= HandleFPSUseItem;
	}
	
	void Update()
	{
		
	}

	private void HandleFPSUseItem(string itemName)
	{
		WeaponPoseData poseData = weaponPoseDB.GetPose(itemName);
		if (poseData == null)
		{
			Debug.LogWarning($"Pose not found for {itemName}");
			return;
		}

		if (itemName == "Gun")
		{
			PlayGunRecoil(poseData);
		}

		if (itemName == "Knife")
		{
			PlayKnifeStab(poseData);
		}

		if (itemName == "Keycard")
		{
			PlayKeycard(poseData);
		}
	}

	private void PlayGunRecoil(WeaponPoseData poseData)
	{
		r_Hand.DOKill();

		float recoilAngleZ = -43f;
		float duration = 0.2f;
		float returnDuration = 0.3f;

		Quaternion defaultRot = Quaternion.Euler(poseData.r_HandRotation);
		Quaternion recoilRot = defaultRot * Quaternion.AngleAxis(recoilAngleZ, Vector3.forward);

		Sequence seq = DOTween.Sequence();
		seq.Append(r_Hand.DOLocalRotateQuaternion(recoilRot, duration).SetEase(Ease.OutQuad));
		seq.Append(r_Hand.DOLocalRotateQuaternion(defaultRot, returnDuration).SetEase(Ease.OutExpo));
	}
	
	private void PlayKnifeStab(WeaponPoseData poseData)
	{
		r_ArmStrech.DOKill();
		r_Forearm.DOKill();
		r_Hand.DOKill();

		float duration = 0.1f;
		float returnDuration = 0.25f;
		
		Quaternion stabForearmRot = Quaternion.Euler(-36.8f, -39.7f, 2.4f);
		Quaternion stabArmRot = Quaternion.Euler(-77f, 283.2f, -6f);
		Quaternion stabHandRot = Quaternion.Euler(-7.5f, 141.4f, 56.4f);
		
		Quaternion defaultArmRot = Quaternion.Euler(poseData.r_ArmStrech);
		Quaternion defaultForearmRot = Quaternion.Euler(poseData.r_ForearmRotation);
		Quaternion defaultHandRot = Quaternion.Euler(poseData.r_HandRotation);
		
		Sequence seq = DOTween.Sequence();
		
		seq.Append(r_ArmStrech.DOLocalRotateQuaternion(stabArmRot, duration).SetEase(Ease.OutSine));
		seq.Join(r_Forearm.DOLocalRotateQuaternion(stabForearmRot, duration).SetEase(Ease.OutSine));
		seq.Join(r_Hand.DOLocalRotateQuaternion(stabHandRot, duration).SetEase(Ease.OutSine));
		
		seq.Append(r_ArmStrech.DOLocalRotateQuaternion(defaultArmRot, returnDuration).SetEase(Ease.OutQuad));
		seq.Join(r_Forearm.DOLocalRotateQuaternion(defaultForearmRot, returnDuration).SetEase(Ease.OutQuad));
		seq.Join(r_Hand.DOLocalRotateQuaternion(defaultHandRot, returnDuration).SetEase(Ease.OutQuad));
	}

	private void PlayKeycard(WeaponPoseData poseData)
	{
		r_ArmStrech.DOKill();
		r_Forearm.DOKill();
		r_Hand.DOKill();

		float duration = 0.5f;
		float returnDuration = 0.6f;
		
		Quaternion stabForearmRot = Quaternion.Euler(115.4f, -65.35f, -170.5f);
		Quaternion stabArmRot = Quaternion.Euler(-38.9f, -7.3f, -46.8f);
		Quaternion stabHandRot = Quaternion.Euler(47f, -151.3f, -16f);
		
		Quaternion defaultArmRot = Quaternion.Euler(poseData.r_ArmStrech);
		Quaternion defaultForearmRot = Quaternion.Euler(poseData.r_ForearmRotation);
		Quaternion defaultHandRot = Quaternion.Euler(poseData.r_HandRotation);
		
		Sequence seq = DOTween.Sequence();
		
		seq.Append(r_ArmStrech.DOLocalRotateQuaternion(stabArmRot, duration).SetEase(Ease.OutSine));
		seq.Join(r_Forearm.DOLocalRotateQuaternion(stabForearmRot, duration).SetEase(Ease.OutSine));
		seq.Join(r_Hand.DOLocalRotateQuaternion(stabHandRot, duration).SetEase(Ease.OutSine));
		
		seq.Append(r_ArmStrech.DOLocalRotateQuaternion(defaultArmRot, returnDuration).SetEase(Ease.OutQuad));
		seq.Join(r_Forearm.DOLocalRotateQuaternion(defaultForearmRot, returnDuration).SetEase(Ease.OutQuad));
		seq.Join(r_Hand.DOLocalRotateQuaternion(defaultHandRot, returnDuration).SetEase(Ease.OutQuad));
	}
}
