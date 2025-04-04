using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpawnPointManager
{
    private HashSet<Transform> occupiedSpawnPoints = new HashSet<Transform>();

    public Transform[] SpawnPoints { get; private set; }

    public void InitializeSpawnPoints(GameObject spawnPointsObject)
    {
        SpawnPoints = spawnPointsObject.GetComponentsInChildren<Transform>()
                              .Where(t => t != spawnPointsObject.transform)
                              .ToArray();
    }

    public Transform GetRandomAvailableSpawnPoint()
    {
        List<Transform> availableSpawnPoints = new List<Transform>();
        foreach (Transform point in SpawnPoints)
        {
            if (!occupiedSpawnPoints.Contains(point))
            {
                availableSpawnPoints.Add(point);
            }
        }

        if (availableSpawnPoints.Count == 0)
        {
            return null;
        }

        return availableSpawnPoints[Random.Range(0, availableSpawnPoints.Count)];
    }

    public void MarkOccupied(Transform point)
    {
        occupiedSpawnPoints.Add(point);
    }

    public Vector3 GetGroundPosition(Vector3 spawnPosition)
    {
        RaycastHit hit;
        if (Physics.Raycast(spawnPosition, Vector3.down, out hit, Mathf.Infinity))
        {
            return hit.point;
        }
        return spawnPosition;
    }
}
