using System.Collections;
using Photon.Realtime;
using Photon.Pun;
using UnityEngine;
using System.Collections.Generic;


public class GameManager : MonoBehaviourPunCallbacks
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
        maxGeneratorCount = 2;
        passwordGenerator = new PasswordGenerator();
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("SyncPasswordGenerator", RpcTarget.All, passwordGenerator.GetPasswords(), passwordGenerator.GetValidPasswords());
        }
        StartCoroutine(WaitForAllPlayersSpawned());
    }

    [PunRPC]
    private void SyncPasswordGenerator(string[] passwords, string[] validPasswords)
    {
        passwordGenerator.SetPasswords(passwords);
        passwordGenerator.SetValidPasswords(new List<string>(validPasswords));
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

    public override void OnEnable()
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

    public override void OnDisable()
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

    public void SubActiveGenerator()
    {
        activeGeneratorCount--;
    }

    public void AddUnlockedComputerCount(int n)
    {
        photonView.RPC("SyncUnlockedComputerCount", RpcTarget.All, n);
        if (unlockedComputerCount == 2)
        {
            Debug.Log("GameManger : 모든 컴퓨터 잠금해제 완료.");
            if (PhotonNetwork.IsMasterClient)
            {
                EventManager_Game.Instance.InvokeAllComputerUnlocked();
            }
            else
            {
                PhotonView spawnManagerPhotonView = GameObject.Find("SpawnManager").GetComponent<PhotonView>();
                spawnManagerPhotonView.RPC("RpcSpawnKeycard", RpcTarget.MasterClient);
            }
        }
    }

    [PunRPC]
    private void SyncUnlockedComputerCount(int n)
    {
        unlockedComputerCount += n;
    }

    public int GetUnlockedComputerCount()
    {
        return unlockedComputerCount;
    }
}