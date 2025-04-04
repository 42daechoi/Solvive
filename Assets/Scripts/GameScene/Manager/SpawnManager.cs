using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;


public class SpawnManager : MonoBehaviourPun
{
	public Transform[] playerSpawnPoints;
    public Transform[] hatchSpawnPoints;
    public Transform[] ObserverSpawnPoints;
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
            SpawnItemsForDeveloper();
            SpawnInteractableObjectsForDeveloper();
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
		GameObject player = PhotonNetwork.Instantiate("Player", spawnPosition, spawnRotation);
        Debug.Log("Player spawned: " + player.name + " for player: " + PhotonNetwork.LocalPlayer.NickName);
        photonView.RPC("UsedPlayerSpawnPointSync", RpcTarget.All, spawnIdx);

        //아이템 임시 스폰 - 삭제 필요

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
	private void UsedPlayerSpawnPointSync(int spawnIdx)
	{
        isSpawned[spawnIdx] = true;
    }

	void SpawnItemsForDeveloper()
	{
        SpawnPasswordPapersForDeveloper();
	}

	private void SpawnPasswordPapersForDeveloper()
	{
		float x = -33.289f;

        for (int i = 0; i < 10; i++)
		{
			x += 0.3f;
			PhotonNetwork.InstantiateRoomObject("Items/PasswordPaper", new Vector3(x, 19.07345f, 24.25907f), Quaternion.identity);
		}
	}

	private void SpawnInteractableObjectsForDeveloper()
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
	    List<int> availableIndices = new List<int>();
	    for (int i = 0; i < playerSpawnPoints.Length; i++)
	    {
		    if (!isSpawned[i])
		    {
			    availableIndices.Add(i);
		    }
	    }
	    
	    if (availableIndices.Count == 0)
	    {
		    availableIndices.Add(Random.Range(0, playerSpawnPoints.Length));
	    }
	    
	    int spawnIdx = availableIndices[Random.Range(0, availableIndices.Count)];
	    Vector3 spawnPosition = playerSpawnPoints[spawnIdx].position;
	    Quaternion spawnRotation = playerSpawnPoints[spawnIdx].rotation;

	    PhotonView pv = mannequin.GetComponent<PhotonView>();
	    if (pv != null && pv.IsMine)
	    {
		    pv.RPC("UpdateMannequinPosition", RpcTarget.All, spawnPosition, spawnRotation);
	    }
	    else
	    {
		    Debug.Log("PhotonView가 없거나 소유자가 아님");
	    }
	    
	    photonView.RPC("UsedSpawnPointSync", RpcTarget.All, spawnIdx);
    }

    public void RespawnObserver(GameObject observer, int detectorIndex)
    {
	    int spawnIdx = 0;

	    if (detectorIndex == 0)
	    {
		    spawnIdx = 0;
	    }
	    else if (detectorIndex >= 1 && detectorIndex <= 3)
	    {
		    spawnIdx = 1;
	    }
	    else if (detectorIndex == 4)
	    {
		    spawnIdx = 2;
	    }

	    if (spawnIdx < 0 || spawnIdx >= ObserverSpawnPoints.Length)
	    {
		    Debug.LogWarning("인덱스가 유효하지 않습니다.");
		    return;
	    }

	    Vector3 spawnPosition = ObserverSpawnPoints[spawnIdx].position;
	    Quaternion spawnRotation = ObserverSpawnPoints[spawnIdx].rotation;

	    PhotonView pv = observer.GetComponent<PhotonView>();
	    if (pv != null && pv.IsMine)
	    {
		    pv.RPC("UpdateObserverPosition", RpcTarget.All, spawnPosition, spawnRotation);
	    }
	    else
	    {
		    Debug.Log("Observer의 PhotonView가 없거나 소유자가 아님");
	    }
    }
}
