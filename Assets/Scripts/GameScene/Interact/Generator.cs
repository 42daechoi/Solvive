using Photon.Pun;
using UnityEngine;
using System;
using System.Collections;

public class Generator : MonoBehaviourPun, IInteractableObject
{
	[SerializeField] private bool isComputerPhase;
	[SerializeField] private int installedBatteryCount;
	[SerializeField] private GameObject[] installedBattery;
	private GeneratorSound generatorSound;
	private GeneratorVibration generatorVibration;
    private int maxBatteryCount = 3;
    private Vector3 batteryPositionOffset;

    private IEnumerator WaitForEventManager()
    {
        while (EventManager_Game.Instance == null)
        {
            Debug.Log("Generator : EventManager_Game 초기화 대기 중");
            yield return new WaitForSeconds(0.1f);
        }
        EventManager_Game.Instance.OnAllGeneratorsActivated += OnChangePhase;
        Debug.Log("Generator : EventManager_Game 초기화 완료");
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
		else
		{
			Debug.LogError("Generator : GeneratorSound를 찾을 수 없습니다.");
		}
		if (TryGetComponent(out GeneratorVibration _generatorVibration))
		{
			generatorVibration = _generatorVibration;
		}
		else
		{
			Debug.LogError("Generator : GeneratorVibration을 찾을 수 없습니다.");
		}
	}


    public void Interact(int playerID)
	{
		if (installedBatteryCount != 0 && !isComputerPhase)
		{
			if (IsAllBatteryInstalled())
			{
                photonView.RPC("GeneratorStateChange", RpcTarget.All);
            }
            UninstallBattery(playerID);
        }
	}

	public void TryInstallBattery(int playerID)
	{
        if (!PhotonNetwork.IsMasterClient)
        {
			PhotonView playerPV = PhotonView.Find(playerID);
			if (playerPV == null) return;
            photonView.RPC("RequestInstallBattery", RpcTarget.MasterClient, playerPV.ViewID);
			return;
        }
        if (IsAllBatteryInstalled())
		{
			Debug.Log("Generator : 배터리가 이미 가득 찼습니다.");
			return;
		}
		InstallBattery(playerID);
		if (IsAllBatteryInstalled())
		{
			photonView.RPC("GeneratorStateChange", RpcTarget.All);
		}
	}

    [PunRPC]
    private void RequestInstallBattery(int playerID)
    {
        if (!PhotonNetwork.IsMasterClient) return;
		try
		{
            TryInstallBattery(playerID); 
		}
		catch (NullReferenceException e)
		{
			Debug.LogError($"Generator : {e.Message}");
		}
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
		photonView.RPC("BatteryObjectSync", RpcTarget.All, true, viewID, worldPosition);
		photonView.RPC("IncreaseBatteryCount", RpcTarget.All);
	}

	private void UninstallBattery(int playerId)
	{
		photonView.RPC("DecreaseBatteryCount", RpcTarget.All);
		GameObject uninstallBatteryObject = installedBattery[installedBatteryCount];
		FarmingObject farmingObject = uninstallBatteryObject.GetComponent<FarmingObject>();
		farmingObject.Interact(playerId);
		photonView.RPC("BatteryObjectSync", RpcTarget.All, false, 0, Vector3.zero);
		installedBattery[installedBatteryCount] = null;
	}

	[PunRPC]
	private void BatteryObjectSync(bool isInstall, int playerViewID, Vector3 worldPosition)
	{
        try
        {
            if (isInstall)
            {
                PhotonView playerPV = PhotonView.Find(playerViewID);
				HeldItem heldItem = playerPV.GetComponent<HeldItem>();
                installedBattery[installedBatteryCount] = heldItem.GetItemObject();

                heldItem.ReplaceItem(worldPosition, false);
                photonView.RPC("SyncParent", RpcTarget.All, installedBattery[installedBatteryCount].GetComponent<PhotonView>().ViewID);
            }
            else
            {
                installedBattery[installedBatteryCount] = null;
            }
        }
        catch (NullReferenceException e)
        {
            Debug.LogError($"Generator : {e.Message}");
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
    [PunRPC]
	private void IncreaseBatteryCount()
	{
		installedBatteryCount++;
		batteryPositionOffset += new Vector3(0.4f, 0, 0);
		generatorSound.PlayInstallBatterySound();
		Debug.Log($"Generator : 배터리 장착 성공. 현재 장착된 배터리 갯수 : {installedBatteryCount}");
	}

	[PunRPC]
	private void DecreaseBatteryCount()
	{
		installedBatteryCount--;
		batteryPositionOffset -= new Vector3(0.4f, 0, 0);
        generatorSound.PlayInstallBatterySound();
        Debug.Log($"Generator : 배터리 회수 성공. 현재 장착된 배터리 갯수 : {installedBatteryCount}");
	}

	[PunRPC]
	private void GeneratorStateChange()
	{
		if (IsAllBatteryInstalled())
		{
			generatorVibration.SetIsGeneratorActive(true);
			generatorSound.PlayStartSound();
            GameManager.Instance.AddActiveGenerator();
		}
		else
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