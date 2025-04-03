using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ItemSpawner : MonoBehaviourPun
{
    private SpawnPointManager spawnPointManager = new SpawnPointManager();
    private Dictionary<string, int> itemNameCountMap = new Dictionary<string, int>();

    public GameObject ItemSpawnPointsObject;
    public Transform[] originalPrefabs;

    private void Start()
    {
        spawnPointManager.InitializeSpawnPoints(ItemSpawnPointsObject);
        if (PhotonNetwork.IsMasterClient) SpawnItems();
    }

    private void SetSpawnCount()
    {
        int playerCount = PhotonNetwork.CurrentRoom.PlayerCount;
        itemNameCountMap["Gun"] = playerCount;
        itemNameCountMap["Knife"] = playerCount * 2;
        itemNameCountMap["Battery"] = 15;
        itemNameCountMap["Flashlight"] = playerCount;
        itemNameCountMap["PasswordPaper"] = playerCount * 2;
    }

    public void SpawnItems()
    {
        SetSpawnCount();
        foreach (var item in itemNameCountMap)
        {
            for (int i = 0; i < item.Value; i++)
            {
                SpawnItem(item.Key);
            }
        }
    }

    private void SpawnItem(string itemName)
    {
        Transform selectedSpawnPoint = spawnPointManager.GetRandomAvailableSpawnPoint();
        if (selectedSpawnPoint == null)
        {
            Debug.Log($"ItemSpawner : 아이템 '{itemName}'을 스폰할 위치가 없습니다.");
            return;
        }

        PhotonNetwork.InstantiateRoomObject(
            "Items/" + itemName,
            spawnPointManager.GetGroundPosition(selectedSpawnPoint.position),
            GetRotationFromOriginalPrefab(itemName)
        );

        spawnPointManager.MarkOccupied(selectedSpawnPoint);
    }

    private Quaternion GetRotationFromOriginalPrefab(string itemName)
    {
        switch (itemName)
        {
            case "Battery": return originalPrefabs[0].localRotation;
            case "Flashlight": return originalPrefabs[1].localRotation;
            case "Gun": return originalPrefabs[2].localRotation;
            case "Knife": return originalPrefabs[3].localRotation;
            case "PasswordPaper": return originalPrefabs[4].localRotation;
        }
        return Quaternion.identity;
    }
}
