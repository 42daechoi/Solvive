using System.Collections;
using Photon.Realtime;
using Photon.Pun;
using UnityEngine;
using System.Collections.Generic;
using Photon.Voice;


public class GameManager : MonoBehaviourPunCallbacks
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private int activeGeneratorCount;
    [SerializeField] private int maxGeneratorCount;
    [SerializeField] private int unlockedComputerCount;
    [SerializeField] private int citizenCount;
    [SerializeField] private string voiceAppId = "14f705a4-2975-4ab3-a0e2-84cedca9e602";
    [SerializeField] private string regionCode = "kr";

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
        PhotonAppSettings.Instance.UseCloud(voiceAppId, regionCode);
    }

    private void Start()
    {
        citizenCount = 0;
        unlockedComputerCount = 0;
        activeGeneratorCount = 0;
        maxGeneratorCount = 1;
        StartCoroutine(WaitForAllPlayersSpawned());
    }

    public void InitCitizenCount(int count)
    {
        photonView.RPC("SyncInitCitizenCount", RpcTarget.All, count);
    }

    [PunRPC]
    private void SyncInitCitizenCount(int count)
    {
        citizenCount = count;
    }

    private int GetRoleCount(PlayerRole playerRole)
    {
        int count = 0;
        PlayerRoleDistribution[] players = FindObjectsOfType<PlayerRoleDistribution>();

        foreach (PlayerRoleDistribution prd in players)
        {
            if (prd.role == playerRole)
            {
                count++;
            }
        }
        return count;
    }

    public void EliminateOrEscapeCitizen(int viewID, string flag)
    {
        if (citizenCount < 1)
        {
            Debug.Log("GameManager : citizenCount가 초기화 되지 않았습니다.");
            return;
        }
        else
        {
            photonView.RPC("SyncEliminateOrEscapeCitizen", RpcTarget.All, viewID, flag);
        }
    }

    [PunRPC]
    private void SyncEliminateOrEscapeCitizen(int viewID, string flag)
    {
        citizenCount--;
        EventManager_Game.Instance.InvokeObserverState(viewID);
        Debug.Log($"GameManager: InvokeObserverState발행요청 - ViewID: {viewID}");
        if (citizenCount == 0)
        {
            if (GetRoleCount(PlayerRole.Mannequin) > 0)
            {
                if (flag == "Escape") 
                {
                    EventManager_Game.Instance.InvokeEndGame(PlayerRole.Citizen);
                }
                else
                {
                    EventManager_Game.Instance.InvokeEndGame(PlayerRole.Mannequin);
                }

            }
            else
            {
                EventManager_Game.Instance.InvokeEndGame(PlayerRole.Citizen);
            }
        }
        if (citizenCount == 1) EventManager_Game.Instance.InvokeOneCitizenAlive();
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
        if (unlockedComputerCount % 2 == 0)
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