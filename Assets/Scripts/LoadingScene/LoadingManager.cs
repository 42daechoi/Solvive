using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class LoadingManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private Slider progressSlider;
    
    private PhotonView photonView;
    private Dictionary<string, bool> resourceStatus = new Dictionary<string, bool>();
    private Dictionary<int, bool> playerLoadStatus = new Dictionary<int, bool>();
    
    private readonly string[] requiredResources = new string[]
    {
        "CowBoy",
        "Item/Battery"
    };

    private void Awake()
    {
        photonView = GetComponent<PhotonView>();
        InitializeLoadingUI();
    }

    private void Start()
    {
        StartCoroutine(LoadGameResources());
    }

    private void InitializeLoadingUI()
    {
        progressSlider.value = 0f;
    }

    private IEnumerator LoadGameResources()
    {
        float totalProgress = 0f;
        float progressPerResource = 1f / requiredResources.Length;

        Debug.Log("=== 리소스 로딩 시작 ===");

        for (int i = 0; i < requiredResources.Length; i++)
        {
            string resourcePath = requiredResources[i];
            Debug.Log($"\n{resourcePath} 로딩 시작");

            ResourceRequest request = Resources.LoadAsync(resourcePath);
            float startTime = Time.realtimeSinceStartup;
        
            while (!request.isDone)
            {
                totalProgress = (i + request.progress) * progressPerResource;
                Debug.Log($"진행률: {request.progress:P2}, 경과시간: {Time.realtimeSinceStartup - startTime:F2}초");
                progressSlider.value = totalProgress;
                yield return null;
            }

            float endTime = Time.realtimeSinceStartup - startTime;
        
            if (request.asset != null)
            {
                GameObject prefab = request.asset as GameObject;
                if (prefab != null)
                {
                    Debug.Log($"프리팹 '{prefab.name}' 로드 완료 ({endTime:F2}초)");
                }
                resourceStatus[resourcePath] = true;
            }
            else
            {
                Debug.LogError($"리소스 로드 실패: Resources/{resourcePath}");
                resourceStatus[resourcePath] = false;
            }

            totalProgress = (i + 1) * progressPerResource;
            progressSlider.value = totalProgress;
        }

        bool allResourcesLoaded = resourceStatus.All(status => status.Value);
        Debug.Log($"리소스 로드 {(allResourcesLoaded ? "성공" : "실패")}");

        photonView.RPC("ReportLoadStatus", RpcTarget.MasterClient, 
            PhotonNetwork.LocalPlayer.ActorNumber, allResourcesLoaded);
    }

    private void UpdateUI(string status, float progress)
    {
        progressSlider.value = progress;
    }

    [PunRPC]
    private void ReportLoadStatus(int playerNumber, bool loadComplete)
    {
        playerLoadStatus[playerNumber] = loadComplete;

        // 마스터 클라이언트만 체크
        if (!PhotonNetwork.IsMasterClient) return;
        
        bool allPlayersLoaded = true;
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (!playerLoadStatus.ContainsKey(player.ActorNumber) || 
                !playerLoadStatus[player.ActorNumber])
            {
                allPlayersLoaded = false;
                break;
            }
        }
        
        if (allPlayersLoaded)
        {
            photonView.RPC("StartGame", RpcTarget.All);
        }
    }

    [PunRPC]
    private void StartGame()
    {
        if (progressSlider.value < 1f)
        {
            progressSlider.value = 1f;
        }
        UpdateUI("게임 시작중...", 1f);
        PhotonNetwork.LoadLevel("GameScene");
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogError($"서버 연결 끊김: {cause}");
        PhotonNetwork.LoadLevel("MainScene");
    }
}