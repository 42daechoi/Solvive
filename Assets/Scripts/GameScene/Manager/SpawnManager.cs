using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Photon.Pun;
using UnityEngine;

public class SpawnManager : MonoBehaviourPun
{
	public Transform[] playerSpawnPoints;
	public Transform[] itemSpawnPoints;
	private bool[] isSpawned;

    private void Awake()
    {
        isSpawned = new bool[playerSpawnPoints.Length];
    }
    void Start()
	{

		SpawnPlayers();
		if (PhotonNetwork.IsMasterClient)
		{
            SpawnItems();
			SpawnInteractableObjects();
        }
	}

	void SpawnPlayers()
	{
		int playerIdx = PhotonNetwork.LocalPlayer.ActorNumber - 1;
		int spawnIdx = GetAvailableSpawnIndex(playerIdx);

		if (spawnIdx < 0) return;
		Vector3 spawnPosition = playerSpawnPoints[spawnIdx].position;
		Quaternion spawnRotation = playerSpawnPoints[spawnIdx].rotation;
		GameObject player = PhotonNetwork.Instantiate("CowBoy", spawnPosition, spawnRotation);
        Debug.Log("Player spawned: " + player.name + " for player: " + PhotonNetwork.LocalPlayer.NickName);
        photonView.RPC("UsedSpawnPointSync", RpcTarget.All, spawnIdx);
		
		// 아이템 임시 스폰 - 삭제 필요
		PhotonNetwork.InstantiateRoomObject("Items/Battery", spawnPosition - new Vector3(2, 2, 0), spawnRotation);
		PhotonNetwork.InstantiateRoomObject("Items/Battery", spawnPosition - new Vector3(5, 2, 0), spawnRotation);
		PhotonNetwork.InstantiateRoomObject("Items/Battery", spawnPosition - new Vector3(4, 2, 0), spawnRotation);
		PhotonNetwork.InstantiateRoomObject("Items/Gun", spawnPosition - new Vector3(0, 2, 0), spawnRotation);
	}
	
	private int GetAvailableSpawnIndex(int playerIdx)
	{
        if (playerIdx >= 0 && playerIdx < playerSpawnPoints.Length && !isSpawned[playerIdx])
        {
            return playerIdx;
        }

        for (int i = 0; i < playerSpawnPoints.Length; i++)
        {
            if (!isSpawned[i])
            {
                return i;
            }
        }

        return -1;
    }

	[PunRPC]
	private void UsedSpawnPointSync(int spawnIdx)
	{
        isSpawned[spawnIdx] = true;
    }

	void SpawnItems()
	{
		SpawnPasswordPapers();
	}

	private void SpawnPasswordPapers()
	{
		float x = -33.289f;

        for (int i = 0; i < 10; i++)
		{
			x += 0.3f;
			PhotonNetwork.InstantiateRoomObject("Items/PasswordPaper", new Vector3(x, 19.07345f, 24.25907f), Quaternion.Euler(Vector3.zero));
		}
	}

	private void SpawnInteractableObjects()
	{
        PhotonNetwork.InstantiateRoomObject("InteractableObjects/Computer", new Vector3(-29.77029f, 20.269f, 21.33452f), Quaternion.Euler(Vector3.zero));
        PhotonNetwork.InstantiateRoomObject("InteractableObjects/Computer", new Vector3(-30.77f, 20.269f, 21.33452f), Quaternion.Euler(Vector3.zero));
    }
}
