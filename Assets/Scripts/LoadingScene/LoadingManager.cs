using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;

public class LoadingManager : MonoBehaviourPunCallbacks
{
    public static LoadingManager Instance { get; private set; }

    // UI 관련
    [Header("Loading UI")]
    public Slider loadingSlider;
    //public Text loadingText;

    // 로딩 상태 추적용 변수
    private bool isLocalPlayerLoaded = false;
    private int loadedPlayersCount = 0;
    private float totalLoadingTime = 5f; // 총 로딩 예상 시간

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

        // 시작할 때 슬라이더 초기화
        if (loadingSlider != null)
        {
            loadingSlider.minValue = 0f;
            loadingSlider.maxValue = 1f;
            loadingSlider.value = 0f;
        }
    }

    private void Start()
    {
        Debug.Log("로딩 매니저 시작");
        UpdateLoadingUI("리소스 로딩 준비 중...", 0f);
        
        StartCoroutine(LoadingProcess());
    }

    // UI 업데이트 메서드
    private void UpdateLoadingUI(string message, float progress)
    {
        Debug.Log("업데이트 로딩 UI");
        //if (loadingText != null)
        //{
        //loadingText.text = message;
        //}

        if (loadingSlider != null)
        {
            // 디버그 로그 추가
            Debug.Log($"Updating slider: {progress}");
            loadingSlider.value = progress;
        }
        else
        {
            Debug.LogError("로딩 슬라이더가 null입니다!");
        }
    }

    private IEnumerator LoadingProcess()
    {
        float elapsedTime = 0f;

        // 리소스 로드
        yield return StartCoroutine(LoadResources(elapsedTime));

        // 로딩 완료 표시
        MarkLocalPlayerLoaded();

        // 모든 플레이어 로딩 대기
        yield return StartCoroutine(WaitForAllPlayersLoaded());

        // 게임 씬으로 전환
        if (PhotonNetwork.IsMasterClient)
        {
            UpdateLoadingUI("게임 씬 로딩 중...", 1f);
            yield return new WaitForSeconds(0.5f);
            PhotonNetwork.LoadLevel("GameScene");
        }
    }

    private IEnumerator LoadResources(float startTime)
    {
        float totalLoadTime = totalLoadingTime;
        
        // 리소스 로딩 시뮬레이션
        while (startTime < totalLoadTime)
        {
            startTime += Time.deltaTime;
            float progress = Mathf.Clamp01(startTime / totalLoadTime);
            
            UpdateLoadingUI($"리소스 로딩 중... {Mathf.RoundToInt(progress * 100)}%", progress);
            
            yield return null;
        }

        Debug.Log("리소스 로딩 완료");
    }

    private void MarkLocalPlayerLoaded()
    {
        isLocalPlayerLoaded = true;
        
        // 다른 모든 플레이어에게 로딩 완료 알림
        photonView.RPC("PlayerLoadedRPC", RpcTarget.AllBuffered);
    }

    [PunRPC]
    private void PlayerLoadedRPC()
    {
        loadedPlayersCount++;
        Debug.Log($"로딩된 플레이어 수: {loadedPlayersCount}/{PhotonNetwork.PlayerList.Length}");
        
        // UI 업데이트 (대략적인 로딩 진행률)
        float progress = (float)loadedPlayersCount / PhotonNetwork.PlayerList.Length;
        UpdateLoadingUI($"플레이어 동기화 중... {loadedPlayersCount}/{PhotonNetwork.PlayerList.Length}", progress);
    }

    private IEnumerator WaitForAllPlayersLoaded()
    {
        float waitTime = 0f;
        while (loadedPlayersCount < PhotonNetwork.PlayerList.Length)
        {
            waitTime += Time.deltaTime;
            
            // 60초 타임아웃
            if (waitTime > 120f)
            {
                UpdateLoadingUI("로딩 시간 초과!", 1f);
                Debug.LogError("로딩 시간 초과");
                break;
            }

            yield return null;
        }

        UpdateLoadingUI("모든 플레이어 로딩 완료", 1f);
        Debug.Log("모든 플레이어 로딩 완료");
    }
}