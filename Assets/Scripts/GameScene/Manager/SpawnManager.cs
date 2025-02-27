using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Photon.Pun;
using UnityEngine;
using UnityEngine.InputSystem;

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
		EventManager_Game.Instance.OnAllComputerUnlocked += SpawnKeycard;
		SpawnPlayers();
		if (PhotonNetwork.IsMasterClient)
		{
            SpawnItems();
			SpawnInteractableObjects();
        }
	}

    private void OnDisable()
    {
        EventManager_Game.Instance.OnAllComputerUnlocked -= SpawnKeycard;
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
			PhotonNetwork.InstantiateRoomObject("Items/PasswordPaper", new Vector3(x, 19.07345f, 24.25907f), Quaternion.identity);
		}
	}

	private void SpawnInteractableObjects()
	{
        PhotonNetwork.InstantiateRoomObject("InteractableObjects/Computer", new Vector3(-29.77029f, 20.269f, 21.33452f), Quaternion.identity);
        PhotonNetwork.InstantiateRoomObject("InteractableObjects/Computer", new Vector3(-30.77f, 20.269f, 21.33452f), Quaternion.identity);
    }

	private void SpawnKeycard()
	{
        List<Transform> spawnPoints = GetKeycardSpawnPoints();
        foreach (Transform spawnPoint in spawnPoints)
        {
            GameObject keycard = PhotonNetwork.InstantiateRoomObject("Items/Keycard", spawnPoint.position, Quaternion.Euler(-90, 0, 0));
            StartCoroutine(MoveKeycard(keycard.transform, spawnPoint));
        }

    }

    private List<Transform> GetKeycardSpawnPoints()
    {
        GameObject[] computers = GameObject.FindGameObjectsWithTag("Computer");
        List<Transform> spawnPoints = new List<Transform>();

        foreach (GameObject computer in computers)
        {
            Transform spawnPoint = computer.transform.Find("KeycardSpawnPoint");
            if (spawnPoint != null)
            {
                spawnPoints.Add(spawnPoint);
            }
            else
            {
                Debug.Log("SpawnManager : Computer에서 스폰 포인트를 찾을 수 없습니다.");
            }
        }
        return spawnPoints;
    }

    private IEnumerator MoveKeycard(Transform keycard, Transform spawnPoint)
    {
        float duration = 2f;
        float elapsedTime = 0f;

        Vector3 startPosition = spawnPoint.position;
        Vector3 endPosition = spawnPoint.position + spawnPoint.forward * 0.15f;

        while (elapsedTime < duration)
        {
            keycard.position = Vector3.Lerp(startPosition, endPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        keycard.position = endPosition;
    }
}
