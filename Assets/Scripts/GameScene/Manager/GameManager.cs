using System.Collections;
using Photon.Pun;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private int activeGeneratorCount;
    [SerializeField] private int maxGeneratorCount;

    private PasswordGenerator passwordGenerator;

    [SerializeField] private GameObject inputManager_Game;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        activeGeneratorCount = 0;
        maxGeneratorCount = 1;
        passwordGenerator = new PasswordGenerator();
        StartCoroutine(WaitForAllPlayersSpawned());
    }

    private IEnumerator WaitForAllPlayersSpawned()
    {
        int SpawnedPlayerCount = GameObject.FindGameObjectsWithTag("Player").Length;
        while (PhotonNetwork.CurrentRoom.PlayerCount != SpawnedPlayerCount)
        {
            yield return new WaitForSeconds(0.1f);
            SpawnedPlayerCount = GameObject.FindGameObjectsWithTag("Player").Length;
        }
        EventManager_Game.Instance.InvokeAllPlayerSpawned();
        inputManager_Game.SetActive(true);
    }

    public PasswordGenerator GetPasswordGenerator()
    {
        return passwordGenerator;
    }

    public void AddActiveGenerator()
    {
        activeGeneratorCount++;
        if (activeGeneratorCount >= maxGeneratorCount)
        {
            EventManager_Game.Instance.InvokeAllGeneratorsActivated();
            Debug.Log("모든 발전기 가동 완료.");
        }
    }
}