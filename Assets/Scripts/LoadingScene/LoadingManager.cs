using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using ExitGames.Client.Photon;

public class LoadingManager : MonoBehaviourPunCallbacks
{
    public static LoadingManager Instance { get; private set; }

    // 로딩 상태 추적용 변수
    private bool isLocalPlayerLoaded = false;
    private int loadedPlayersCount = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        Debug.Log("로딩 매니저 시작");
        
        // 로컬 플레이어의 로딩 상태 초기화
        StartCoroutine(LoadingProcess());
    }

    private IEnumerator LoadingProcess()
    {
        // 리소스 로드 (여기에 필요한 리소스 로딩 로직 추가)
        yield return StartCoroutine(LoadResources());

        // 로딩 완료 표시
        MarkLocalPlayerLoaded();

        // 모든 플레이어 로딩 대기
        yield return StartCoroutine(WaitForAllPlayersLoaded());

        // 게임 씬으로 전환
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel("GameScene");
        }
    }

    private IEnumerator LoadResources()
    {
        // 여기에 게임에 필요한 리소스 로딩 로직 구현
        Debug.Log("리소스 로딩 시작");
        yield return new WaitForSeconds(2f); // 임시 대기 시간
        Debug.Log("리소스 로딩 완료");
    }

    private void MarkLocalPlayerLoaded()
    {
        // 로컬 플레이어의 로딩 완료 상태 설정
        isLocalPlayerLoaded = true;
        
        // 다른 모든 플레이어에게 로딩 완료 알림
        photonView.RPC("PlayerLoadedRPC", RpcTarget.AllBuffered);
    }

    [PunRPC]
    private void PlayerLoadedRPC()
    {
        // 로딩된 플레이어 수 증가
        loadedPlayersCount++;
        Debug.Log($"로딩된 플레이어 수: {loadedPlayersCount}/{PhotonNetwork.PlayerList.Length}");
    }

    private IEnumerator WaitForAllPlayersLoaded()
    {
        // 모든 플레이어의 로딩이 완료될 때까지 대기
        float waitTime = 0f;
        while (loadedPlayersCount < PhotonNetwork.PlayerList.Length)
        {
            waitTime += Time.deltaTime;
            
            // 예를 들어 60초 타임아웃
            if (waitTime > 60f)
            {
                Debug.LogError("로딩 시간 초과");
                break;
            }

            yield return null;
        }

        Debug.Log("모든 플레이어 로딩 완료");
    }
}