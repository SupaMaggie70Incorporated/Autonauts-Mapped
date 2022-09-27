using System.Collections.Generic;
using Steamworks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SteamTest : MonoBehaviour
{
	public static SteamTest Instance;

	private static bool m_Log;

	private CGameID m_GameID;

	private bool m_bRequestedStats;

	private bool m_bStatsValid;

	private bool m_bStoreStats;

	private List<BadgeEvent> m_BadgeStatsChanged;

	protected Callback<GameOverlayActivated_t> m_GameOverlayActivated;

	protected Callback<UserStatsReceived_t> m_UserStatsReceived;

	protected Callback<UserStatsStored_t> m_UserStatsStored;

	protected Callback<UserAchievementStored_t> m_UserAchievementStored;

	private void Awake()
	{
		Instance = this;
		m_BadgeStatsChanged = new List<BadgeEvent>();
	}

	private void Start()
	{
		if (SteamManager.Initialized)
		{
			Debug.Log(SteamFriends.GetPersonaName());
		}
	}

	private void OnEnable()
	{
		if (SteamManager.Initialized)
		{
			m_GameID = new CGameID(SteamUtils.GetAppID());
			m_GameOverlayActivated = Callback<GameOverlayActivated_t>.Create(OnGameOverlayActivated);
			m_UserStatsReceived = Callback<UserStatsReceived_t>.Create(OnUserStatsReceived);
			m_UserStatsStored = Callback<UserStatsStored_t>.Create(OnUserStatsStored);
			m_UserAchievementStored = Callback<UserAchievementStored_t>.Create(OnAchievementStored);
			m_bRequestedStats = false;
			m_bStatsValid = false;
		}
	}

	private string GetAchievementName(Badge NewBadge)
	{
		return "Achievement" + NewBadge.m_ID;
	}

	private string GetStatName(BadgeEvent NewBadgeEvent)
	{
		return "Stat" + NewBadgeEvent.m_Type;
	}

	private void Update()
	{
		if (!SteamManager.Initialized)
		{
			return;
		}
		if (!m_bRequestedStats)
		{
			if (!SteamManager.Initialized)
			{
				m_bRequestedStats = true;
				return;
			}
			Debug.Log("RequestStats");
			bool flag = (m_bRequestedStats = SteamUserStats.RequestCurrentStats());
		}
		if (!m_bStatsValid || !m_bStoreStats)
		{
			return;
		}
		foreach (BadgeEvent item in m_BadgeStatsChanged)
		{
			string statName = GetStatName(item);
			int eventCount = BadgeManager.Instance.GetEventCount(item.m_Type);
			SteamUserStats.SetStat(statName, eventCount);
		}
		m_BadgeStatsChanged.Clear();
		bool flag2 = SteamUserStats.StoreStats();
		m_bStoreStats = !flag2;
	}

	public void StatChanged(BadgeEvent NewBadgeEvent)
	{
		if (SteamManager.Initialized && m_bStatsValid)
		{
			m_BadgeStatsChanged.Add(NewBadgeEvent);
			m_bStoreStats = true;
		}
	}

	public void UnlockAchievement(Badge NewBadge)
	{
		if (SteamManager.Initialized)
		{
			Debug.Log("UnlockAchievement " + NewBadge.m_ID);
			SteamUserStats.SetAchievement(GetAchievementName(NewBadge));
			m_bStoreStats = true;
		}
	}

	private void OnUserStatsReceived(UserStatsReceived_t pCallback)
	{
		if (!SteamManager.Initialized || (ulong)m_GameID != pCallback.m_nGameID)
		{
			return;
		}
		if (EResult.k_EResultOK == pCallback.m_eResult)
		{
			if (m_Log)
			{
				Debug.Log("Received stats and achievements from Steam\n");
			}
			m_bStatsValid = true;
			foreach (Badge badge in BadgeManager.Instance.m_BadgeData.m_Badges)
			{
				string achievementName = GetAchievementName(badge);
				bool pbAchieved = false;
				if (SteamUserStats.GetAchievement(achievementName, out pbAchieved))
				{
					if (pbAchieved == badge.m_Complete)
					{
						continue;
					}
					if (pbAchieved)
					{
						BadgeManager.Instance.ForceBadgeComplete(badge);
						if (m_Log)
						{
							Debug.Log("Badge Complete (game synced) " + achievementName);
						}
					}
					else
					{
						UnlockAchievement(badge);
						if (m_Log)
						{
							Debug.Log("Badge Complete (Steam synced)" + achievementName);
						}
					}
				}
				else if (m_Log)
				{
					Debug.Log("Badge not found " + badge.m_ID);
				}
			}
			BadgeEvent[] badgeEvents = BadgeManager.Instance.m_BadgeEvents;
			foreach (BadgeEvent badgeEvent in badgeEvents)
			{
				string statName = GetStatName(badgeEvent);
				int pData = 0;
				if (SteamUserStats.GetStat(statName, out pData))
				{
					if (badgeEvent.m_Count == pData)
					{
						continue;
					}
					if (pData > badgeEvent.m_Count)
					{
						BadgeManager.Instance.SetEventCount(badgeEvent.m_Type, pData);
						if (m_Log)
						{
							Debug.Log("Badge Event (game synced) " + statName + " " + pData);
						}
					}
					else
					{
						StatChanged(badgeEvent);
						if (m_Log)
						{
							Debug.Log("Badge Event (Steam synced) " + statName + " " + badgeEvent.m_Count);
						}
					}
				}
				else if (m_Log)
				{
					Debug.Log("Stat not found " + badgeEvent.m_Type);
				}
			}
		}
		else
		{
			Debug.Log("RequestStats - failed, " + pCallback.m_eResult);
		}
	}

	private void OnUserStatsStored(UserStatsStored_t pCallback)
	{
		if ((ulong)m_GameID == pCallback.m_nGameID && EResult.k_EResultOK != pCallback.m_eResult)
		{
			if (EResult.k_EResultInvalidParam == pCallback.m_eResult)
			{
				Debug.Log("StoreStats - some failed to validate");
				UserStatsReceived_t pCallback2 = default(UserStatsReceived_t);
				pCallback2.m_eResult = EResult.k_EResultOK;
				pCallback2.m_nGameID = (ulong)m_GameID;
				OnUserStatsReceived(pCallback2);
			}
			else
			{
				Debug.Log("StoreStats - failed, " + pCallback.m_eResult);
			}
		}
	}

	private void OnAchievementStored(UserAchievementStored_t pCallback)
	{
		if ((ulong)m_GameID == pCallback.m_nGameID)
		{
			if (pCallback.m_nMaxProgress == 0)
			{
				Debug.Log("Achievement '" + pCallback.m_rgchAchievementName + "' unlocked!");
				return;
			}
			Debug.Log("Achievement '" + pCallback.m_rgchAchievementName + "' progress callback, (" + pCallback.m_nCurProgress + "," + pCallback.m_nMaxProgress + ")");
		}
	}

	public void ClearAll()
	{
		Debug.Log("Clear achievements");
		SteamUserStats.ResetAllStats(bAchievementsToo: true);
		SteamUserStats.RequestCurrentStats();
	}

	private void OnGameOverlayActivated(GameOverlayActivated_t pCallback)
	{
		if (pCallback.m_bActive != 0)
		{
			Debug.Log("Steam Overlay has been activated");
			if (SceneManager.GetActiveScene().name == "Main" && GameStateManager.Instance.GetCurrentState().m_BaseState != GameStateManager.State.PlatformPaused)
			{
				GameStateManager.Instance.PushState(GameStateManager.State.PlatformPaused);
			}
			return;
		}
		Debug.Log("Steam Overlay has been closed");
		SteamWorkshopManager.Instance.NeedsMenuRefresh = true;
		if (SceneManager.GetActiveScene().name == "Main" && GameStateManager.Instance.GetCurrentState().m_BaseState == GameStateManager.State.PlatformPaused)
		{
			GameStateManager.Instance.PopState();
		}
	}

	public void TestToggleOverlay()
	{
		if (SteamManager.Initialized)
		{
			GameOverlayActivated_t pCallback = default(GameOverlayActivated_t);
			if (GameStateManager.Instance.GetCurrentState().m_BaseState == GameStateManager.State.PlatformPaused)
			{
				pCallback.m_bActive = 0;
			}
			else
			{
				pCallback.m_bActive = 1;
			}
			OnGameOverlayActivated(pCallback);
		}
	}
}
