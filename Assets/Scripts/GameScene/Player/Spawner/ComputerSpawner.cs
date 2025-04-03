using UnityEngine;
using Photon.Pun;

public class ComputerSpawner : MonoBehaviour
{
    private SpawnPointManager spawnPointManager = new SpawnPointManager();
    public GameObject computerSpawnPointsObject;
    [SerializeField] private int computerCount = 2;

    private void Start()
    {
        spawnPointManager.InitializeSpawnPoints(computerSpawnPointsObject);
        if (PhotonNetwork.IsMasterClient) SpawnComputers();
    }

    public void SpawnComputers()
    {
        for (int i = 0; i < computerCount; i++)
        {
            SpawnComputer();
        }
    }

    private void SpawnComputer()
    {
        Transform selectedSpawnPoint = spawnPointManager.GetRandomAvailableSpawnPoint();
        if (selectedSpawnPoint == null)
        {
            Debug.Log("ComputerSpawner : Computer를 스폰할 위치가 없습니다.");
            return;
        }

        PhotonNetwork.InstantiateRoomObject(
            "InteractableObjects/Computer",
            selectedSpawnPoint.position,
            selectedSpawnPoint.localRotation
        );

        spawnPointManager.MarkOccupied(selectedSpawnPoint);
    }
}
