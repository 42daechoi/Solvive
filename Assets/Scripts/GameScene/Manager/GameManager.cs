using System.Collections;
using Photon.Pun;
using UnityEngine;

public class GameManager : MonoBehaviourPun
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private int activeGeneratorCount;
    [SerializeField] private int maxGeneratorCount;
    [SerializeField] private int unlockedComputerCount;

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
        unlockedComputerCount = 0;
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

    private void OnEnable()
    {
        StartCoroutine(WaitForEventManager());
    }

    private IEnumerator WaitForEventManager()
    {
        while (EventManager_Game.Instance == null)
        {
            yield return new WaitForSeconds(0.1f);
        }
        EventManager_Game.Instance.OnChangeUnlockedComputerCount += AddUnlockedComputerCount;
    }

    private void OnDisable()
    {
        EventManager_Game.Instance.OnChangeUnlockedComputerCount -= AddUnlockedComputerCount;
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

    public void AddUnlockedComputerCount(int n)
    {
        unlockedComputerCount += n;
        Debug.Log("GameManager : 잠금 해제된 컴퓨터 개수" + unlockedComputerCount);
        if (unlockedComputerCount == 2)
        {
            Debug.Log("GameManger : 모든 컴퓨터 잠금해제 완료.");
            // 카드키 제공하는 작업 필요
        }
    }

    public int GetUnlockedComputerCount()
    {
        return unlockedComputerCount;
    }

}