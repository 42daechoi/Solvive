using UnityEngine;
using Photon.Pun;

public class GeneratorSpawner : MonoBehaviour
{
    private SpawnPointManager spawnPointManager = new SpawnPointManager();
    public GameObject generatorSpawnPointsObject;
    [SerializeField] private int generatorCount = 3;

    private void Start()
    {
        spawnPointManager.InitializeSpawnPoints(generatorSpawnPointsObject);
        if (PhotonNetwork.IsMasterClient) SpawnGenerators();
    }

    public void SpawnGenerators()
    {
        for (int i = 0; i < generatorCount; i++)
        {
            SpawnGenerator();
        }
    }

    private void SpawnGenerator()
    {
        Transform selectedSpawnPoint = spawnPointManager.GetRandomAvailableSpawnPoint();
        if (selectedSpawnPoint == null)
        {
            Debug.Log("GeneratorSpawner : Generator를 스폰할 위치가 없습니다.");
            return;
        }

        PhotonNetwork.InstantiateRoomObject(
            "InteractableObjects/Generator",
            spawnPointManager.GetGroundPosition(selectedSpawnPoint.position),
            Quaternion.Euler(-90, 0, 0)
        );

        spawnPointManager.MarkOccupied(selectedSpawnPoint);
    }
}
