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
    public Transform[] hatchSpawnPoints;
	private bool[] isSpawned;

    private void Awake()
    {
        isSpawned = new bool[playerSpawnPoints.Length];
    }
    void Start()
	{
		EventManager_Game.Instance.OnAllComputerUnlocked += SpawnKeycard;
        EventManager_Game.Instance.OnOneCitizenAlive += SpawnHatch;
		SpawnPlayers();
		if (PhotonNetwork.IsMasterClient)
		{
            SpawnItems();
			SpawnInteractableObjects();
            SpawnHatch();
        }
	}

    private void OnDisable()
    {
        EventManager_Game.Instance.OnAllComputerUnlocked -= SpawnKeycard;
        EventManager_Game.Instance.OnOneCitizenAlive -= SpawnHatch;
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
		PhotonNetwork.InstantiateRoomObject("Items/Knife", spawnPosition - new Vector3(-3, 2, 0), spawnRotation);
        PhotonNetwork.InstantiateRoomObject("Items/Flashlight", spawnPosition - new Vector3(-2, 2, 0), spawnRotation);
		PhotonNetwork.InstantiateRoomObject("Items/Battery", spawnPosition - new Vector3(2, 2, 0), spawnRotation);
		PhotonNetwork.InstantiateRoomObject("Items/Battery", spawnPosition - new Vector3(5, 2, 0), spawnRotation);
		PhotonNetwork.InstantiateRoomObject("Items/Battery", spawnPosition - new Vector3(4, 2, 0), spawnRotation);
		PhotonNetwork.InstantiateRoomObject("Items/Gun", spawnPosition - new Vector3(0, 2, 0), spawnRotation);
		PhotonNetwork.InstantiateRoomObject("Items/Keycard", spawnPosition - new Vector3(-4, 2, 0), spawnRotation);
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

    [PunRPC]
    public void RpcSpawnKeycard()
    {
        SpawnKeycard();
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

    private void SpawnHatch()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        int randomNumber = Random.Range(0, 8);
        Vector3 spawnPosition = GetGroundPosition(hatchSpawnPoints[randomNumber].position);
        PhotonNetwork.InstantiateRoomObject("InteractableObjects/Hatch", spawnPosition, Quaternion.identity);
    }

    private Vector3 GetGroundPosition(Vector3 spawnPosition)
    {
        RaycastHit hit;
        if (Physics.Raycast(spawnPosition, Vector3.down, out hit, Mathf.Infinity))
        {
            return hit.point;
        }
        return spawnPosition;
    }
    
    public void RespawnMannequin(GameObject mannequin)
    {
	    // 사용 가능한 스폰 포인트 목록 생성
	    List<int> availableIndices = new List<int>();
	    for (int i = 0; i < playerSpawnPoints.Length; i++)
	    {
		    if (!isSpawned[i])
		    {
			    availableIndices.Add(i);
		    }
	    }

	    // 만약 사용 가능한 포인트가 없으면 임의의 인덱스를 추가
	    if (availableIndices.Count == 0)
	    {
		    availableIndices.Add(Random.Range(0, playerSpawnPoints.Length));
	    }

	    // 랜덤하게 스폰 인덱스 선택
	    int spawnIdx = availableIndices[Random.Range(0, availableIndices.Count)];
	    Vector3 spawnPosition = playerSpawnPoints[spawnIdx].position;
	    Quaternion spawnRotation = playerSpawnPoints[spawnIdx].rotation;

	    // PhotonView가 자신의 오브젝트인지 확인 후 RPC 호출하여 위치 업데이트
	    PhotonView pv = mannequin.GetComponent<PhotonView>();
	    if (pv != null && pv.IsMine)
	    {
		    // 'UpdateMannequinPosition' RPC는 마네킹 오브젝트에 붙은 스크립트에 정의되어 있어야 합니다.
		    pv.RPC("UpdateMannequinPosition", RpcTarget.All, spawnPosition, spawnRotation);
		    Debug.Log("RPC 호출로 위치 이동: " + mannequin.name + " at spawn index " + spawnIdx);
	    }
	    else
	    {
		    Debug.Log("PhotonView가 없거나 소유자가 아님");
	    }

	    // 선택된 스폰 포인트를 사용중으로 표시 (모든 클라이언트에 동기화)
	    photonView.RPC("UsedSpawnPointSync", RpcTarget.All, spawnIdx);
    }
}
