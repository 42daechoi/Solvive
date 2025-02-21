using Photon.Pun;
using UnityEngine;
using System;

public class Generator : MonoBehaviourPun, IInteractableObject
{
	private bool isComputerPhase;
    private int maxBatteryCount = 3;
	[SerializeField] private int installedBatteryCount;
	[SerializeField] private GameObject[] installedBattery;
	private Vector3 batteryPositionOffset;

    private void OnEnable()
    {
		EventManager_Game.Instance.OnAllGeneratorsActivated += OnChangePhase;
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
	}

	public void Interact(int playerID)
	{
		if (installedBatteryCount != 0 && !isComputerPhase)
		{
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
			photonView.RPC("ExecuteGenerator", RpcTarget.All);
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
	private void IncreaseBatteryCount()
	{
		installedBatteryCount++;
		batteryPositionOffset += new Vector3(0.4f, 0, 0);
		Debug.Log($"Generator : 배터리 장착 성공. 현재 장착된 배터리 갯수 : {installedBatteryCount}");
	}

	[PunRPC]
	private void DecreaseBatteryCount()
	{
		installedBatteryCount--;
		batteryPositionOffset -= new Vector3(0.4f, 0, 0);
		Debug.Log($"Generator : 배터리 회수 성공. 현재 장착된 배터리 갯수 : {installedBatteryCount}");
	}

	[PunRPC]
	private void ExecuteGenerator()
	{
		if (IsAllBatteryInstalled())
		{
            // 발전기 가동 애니메이션 또는 발전기 Light On
            Debug.Log("Generator : 발전기 가동 완료.");
            GameManager.Instance.AddActiveGenerator();
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