#if !(UNITY_STANDALONE_WIN || UNITY_STANDALONE_LINUX || UNITY_STANDALONE_OSX || STEAMWORKS_WIN || STEAMWORKS_LIN_OSX)
#define DISABLESTEAMWORKS
#endif

using UnityEngine;
#if !DISABLESTEAMWORKS
using System.Text;
using Steamworks;
using Photon.Pun;
#endif

[DisallowMultipleComponent]
public class SteamManager : MonoBehaviour
{
#if !DISABLESTEAMWORKS
	protected static bool s_EverInitialized = false;
	protected static SteamManager s_instance;
	protected static SteamManager Instance
	{
		get
		{
			if (s_instance == null)
				return new GameObject("SteamManager").AddComponent<SteamManager>();
			return s_instance;
		}
	}

	protected bool m_bInitialized = false;
	public static bool Initialized => Instance.m_bInitialized;

	protected SteamAPIWarningMessageHook_t m_SteamAPIWarningMessageHook;

	[AOT.MonoPInvokeCallback(typeof(SteamAPIWarningMessageHook_t))]
	protected static void SteamAPIDebugTextHook(int nSeverity, StringBuilder pchDebugText)
	{
		Debug.LogWarning(pchDebugText);
	}

#if UNITY_2019_3_OR_NEWER
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
	private static void InitOnPlayMode()
	{
		s_EverInitialized = false;
		s_instance = null;
	}
#endif

	protected virtual void Awake()
	{
		if (s_instance != null)
		{
			Destroy(gameObject);
			return;
		}
		s_instance = this;

		if (s_EverInitialized)
			throw new System.Exception("SteamManager 중복 초기화 시도!");

		DontDestroyOnLoad(gameObject);

		if (!Packsize.Test())
			Debug.LogError("[Steamworks.NET] Packsize 오류");

		if (!DllCheck.Test())
			Debug.LogError("[Steamworks.NET] DLL 버전 오류");

		try
		{
			if (SteamAPI.RestartAppIfNecessary(AppId_t.Invalid))
			{
				Application.Quit();
				return;
			}
		}
		catch (System.DllNotFoundException e)
		{
			Debug.LogError("[Steamworks.NET] steam_api.dll 로드 실패\n" + e);
			Application.Quit();
			return;
		}

		m_bInitialized = SteamAPI.Init();
		if (!m_bInitialized)
		{
			Debug.LogError("[Steamworks.NET] SteamAPI_Init() 실패");
			return;
		}

		s_EverInitialized = true;

		// ✅ 여기서 Photon 닉네임 자동 설정
		SetPhotonNickname();
	}

	protected virtual void OnEnable()
	{
		if (s_instance == null)
			s_instance = this;

		if (!m_bInitialized) return;

		if (m_SteamAPIWarningMessageHook == null)
		{
			m_SteamAPIWarningMessageHook = new SteamAPIWarningMessageHook_t(SteamAPIDebugTextHook);
			SteamClient.SetWarningMessageHook(m_SteamAPIWarningMessageHook);
		}
	}

	protected virtual void OnDestroy()
	{
		if (s_instance != this) return;
		s_instance = null;

		if (!m_bInitialized) return;
		SteamAPI.Shutdown();
	}

	protected virtual void Update()
	{
		if (m_bInitialized)
			SteamAPI.RunCallbacks();
	}

	/// <summary>Steam 닉네임 반환</summary>
	public static string GetSteamNickname()
	{
		if (!Initialized) return "Not Initialized";
		return SteamFriends.GetPersonaName();
	}

	/// <summary>Photon 닉네임 자동 지정</summary>
	private void SetPhotonNickname()
	{
		if (PhotonNetwork.IsConnected || !Initialized) return;

		string nickname = GetSteamNickname();
		PhotonNetwork.NickName = nickname;
		Debug.Log("📛 Photon 닉네임 설정됨: " + nickname);
	}
#else
	public static bool Initialized => false;
	public static string GetSteamNickname() => "Steam Disabled";
#endif
}
