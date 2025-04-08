using Photon.Pun;
using UnityEngine;
using System;
using System.Collections;

public class Generator : MonoBehaviourPun, IInteractableObject
{
	[SerializeField] private bool isComputerPhase;
	[SerializeField] private int installedBatteryCount = 0;
	[SerializeField] private GameObject[] installedBattery;
	private GeneratorSound generatorSound;
	private GeneratorVibration generatorVibration;
	private int maxBatteryCount = 3;
	private Vector3 batteryPositionOffset;

	private IEnumerator WaitForEventManager()
	{
		while (EventManager_Game.Instance == null)
		{
			yield return new WaitForSeconds(0.1f);
		}
		EventManager_Game.Instance.OnAllGeneratorsActivated += OnChangePhase;
	}

	private void OnEnable()
	{
		StartCoroutine(WaitForEventManager());
	}

	private void OnDisable()
	{
		EventManager_Game.Instance.OnAllGeneratorsActivated -= OnChangePhase;
	}

	void Start()
	{
		isComputerPhase = false;
		installedBatteryCount = 0;
		installedBattery = new GameObject[3];
		batteryPositionOffset = new Vector3(-0.4f, 0.5f, 0.3f);
		if (TryGetComponent(out GeneratorSound _generatorSound))
		{
			generatorSound = _generatorSound;
		}
		if (TryGetComponent(out GeneratorVibration _generatorVibration))
		{
			generatorVibration = _generatorVibration;
		}
	}


	public void Interact(int playerID)
	{
		if (installedBatteryCount > 0 && !isComputerPhase)
		{
			photonView.RPC(nameof(UninstallBattery), RpcTarget.MasterClient, playerID);
		}
	}

	public void TryInstallBattery(int playerID)
	{
		if (!PhotonNetwork.IsMasterClient)
		{
			PhotonView playerPV = PhotonView.Find(playerID);
			if (playerPV == null) return;
			photonView.RPC(nameof(RequestInstallBattery), RpcTarget.MasterClient, playerPV.ViewID);
			return;
		}
		if (IsAllBatteryInstalled())
		{
			Debug.Log("Generator : 배터리가 이미 가득 찼습니다.");
			return;
		}
		InstallBattery(playerID);
		
	}

	[PunRPC]
	private void RequestInstallBattery(int playerID)
	{
		if (!PhotonNetwork.IsMasterClient) return;
		TryInstallBattery(playerID); 
	}

	private void InstallBattery(int playerID)
	{
		PhotonView playerPV = PhotonView.Find(playerID);
		HeldItem heldItem = playerPV.GetComponent<HeldItem>();

		Vector3 worldPosition = transform.position + transform.right * batteryPositionOffset.x
				 + transform.up * batteryPositionOffset.y
				 + transform.forward * batteryPositionOffset.z;

		GameObject playerObject = heldItem.gameObject;
		int viewID = playerObject.GetPhotonView().ViewID;
		int prevCount = installedBatteryCount;
		photonView.RPC(nameof(BatterySync), RpcTarget.All, true, viewID, worldPosition);
		StartCoroutine(WaitAndCheckGeneratorState(prevCount, installedBatteryCount));
	}

	[PunRPC]
	private void UninstallBattery(int playerId)
	{
		if (installedBatteryCount <= 0 || installedBattery[installedBatteryCount - 1] == null)
		{
			return;
		}

		GameObject uninstallBatteryObject = installedBattery[installedBatteryCount - 1];
		FarmingObject farmingObject = uninstallBatteryObject.GetComponent<FarmingObject>();

		if (!farmingObject.GetIsPickUp())
		{
			farmingObject.Interact(playerId);
			int prevCount = installedBatteryCount;
			photonView.RPC(nameof(BatterySync), RpcTarget.All, false, 0, Vector3.zero);

			StartCoroutine(WaitAndCheckGeneratorState(prevCount, installedBatteryCount));
		}
	}

	private IEnumerator WaitAndCheckGeneratorState(int prevCount, int currCount)
	{
		yield return new WaitForSeconds(0.05f);

		photonView.RPC(nameof(GeneratorStateChange), RpcTarget.All, prevCount, currCount);
	}

	[PunRPC]
	private void BatterySync(bool isInstall, int playerViewID, Vector3 worldPosition)
	{
		if (isInstall)
		{
			PhotonView playerPV = PhotonView.Find(playerViewID);
			HeldItem heldItem = playerPV.GetComponent<HeldItem>();
			installedBattery[installedBatteryCount] = heldItem.GetItemObject();
	
			heldItem.ReplaceItem(worldPosition, false);
			photonView.RPC(nameof(SyncParent), RpcTarget.All, installedBattery[installedBatteryCount].GetComponent<PhotonView>().ViewID);
			IncreaseBatteryCount();
		}
		else
		{   
			installedBattery[installedBatteryCount - 1] = null;
			DecreaseBatteryCount();
		}
	}

	[PunRPC]
	private void SyncParent(int itemPhotonViewID)
	{
		PhotonView itemPhotonView = PhotonView.Find(itemPhotonViewID);
		if (itemPhotonView != null)
		{
			itemPhotonView.transform.SetParent(this.gameObject.transform);
		}
	}

	private void IncreaseBatteryCount()
	{
		installedBatteryCount++;
		batteryPositionOffset += new Vector3(0.4f, 0, 0);
		generatorSound.PlayInstallBatterySound();
		Debug.Log($"Generator : 배터리 장착 성공. 현재 장착된 배터리 갯수 : {installedBatteryCount}");
	}

	private void DecreaseBatteryCount()
	{
		installedBatteryCount--;
		batteryPositionOffset -= new Vector3(0.4f, 0, 0);
		generatorSound.PlayInstallBatterySound();
		Debug.Log($"Generator : 배터리 회수 성공. 현재 장착된 배터리 갯수 : {installedBatteryCount}");
	}

	[PunRPC]
	private void GeneratorStateChange(int prevCount, int currCount)
	{
		Debug.Log($"Generator : battery count {prevCount}, {currCount}");
		if (prevCount == 2 && currCount == 3)
		{
			generatorVibration.SetIsGeneratorActive(true);
			generatorSound.PlayStartSound();
			GameManager.Instance.AddActiveGenerator();
		}
		else if (prevCount == 3 && currCount == 2)
		{
			generatorVibration.SetIsGeneratorActive(false);
			generatorSound.PlayEndSound();
			GameManager.Instance.SubActiveGenerator();
		}
	}

	private bool IsAllBatteryInstalled()
	{
		return installedBatteryCount >= maxBatteryCount;
	}

	public void OnChangePhase()
	{
		isComputerPhase = true;
	}
}