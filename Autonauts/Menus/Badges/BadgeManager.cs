using System.Collections.Generic;
using UnityEngine;

public class BadgeManager : MonoBehaviour
{
	public static BadgeManager Instance;

	private static float m_DelayBetweenSaves = 10f;

	private static bool m_Debug = false;

	private static bool m_LogCounts = false;

	public BadgeData m_BadgeData;

	public BadgeEvent[] m_BadgeEvents;

	private bool m_Save;

	private float m_TimeSinceLastSave;

	private Dictionary<BadgeEvent.Type, int> m_ChangedEvents;

	private Dictionary<BadgeEvent.Type, int> m_Counts;

	private void Awake()
	{
		Instance = this;
		Object.DontDestroyOnLoad(base.gameObject);
		m_ChangedEvents = new Dictionary<BadgeEvent.Type, int>();
		if (m_LogCounts)
		{
			m_Counts = new Dictionary<BadgeEvent.Type, int>();
			for (int i = 0; i < 21; i++)
			{
				m_Counts.Add((BadgeEvent.Type)i, 0);
			}
		}
		m_BadgeData = new BadgeData();
		m_BadgeEvents = new BadgeEvent[21];
		for (int j = 0; j < 21; j++)
		{
			BadgeEvent badgeEvent = new BadgeEvent((BadgeEvent.Type)j);
			m_BadgeEvents[j] = badgeEvent;
		}
		m_TimeSinceLastSave = 0f;
		Load();
	}

	private void Save()
	{
		if (m_Debug)
		{
			Debug.Log("Save Badge events " + m_ChangedEvents.Count);
		}
		foreach (KeyValuePair<BadgeEvent.Type, int> changedEvent in m_ChangedEvents)
		{
			BadgeEvent badgeEvent = m_BadgeEvents[(int)changedEvent.Key];
			SteamTest.Instance.StatChanged(badgeEvent);
			badgeEvent.Save();
		}
		m_ChangedEvents.Clear();
		m_BadgeData.Save();
	}

	public void Load()
	{
		if (m_Debug)
		{
			Debug.Log("Load Badge data");
		}
		for (int i = 0; i < 21; i++)
		{
			m_BadgeEvents[i].Load();
		}
		m_BadgeData.Load();
	}

	public void Clear()
	{
		if (m_Debug)
		{
			Debug.Log("Clear Badge data");
		}
		for (int i = 0; i < 21; i++)
		{
			m_BadgeEvents[i].Clear();
		}
		m_BadgeData.Clear();
		SteamTest.Instance.ClearAll();
	}

	public void AddEvent(BadgeEvent.Type NewEventType, int Amount = 1)
	{
		if (GameOptionsManager.Instance.m_Options.m_GameMode == GameOptions.GameMode.ModeCampaign)
		{
			if (m_Debug)
			{
				Debug.Log("Add Badge event " + NewEventType.ToString() + " : " + Amount);
			}
			m_BadgeEvents[(int)NewEventType].AddEvent(Amount);
			if (!m_ChangedEvents.ContainsKey(NewEventType))
			{
				m_ChangedEvents.Add(NewEventType, 1);
			}
			if (m_LogCounts)
			{
				m_Counts[NewEventType] += Amount;
			}
			m_Save = true;
		}
	}

	public int GetEventCount(BadgeEvent.Type NewEventType)
	{
		return m_BadgeEvents[(int)NewEventType].m_Count;
	}

	public void SetEventCount(BadgeEvent.Type NewEventType, int Count)
	{
		m_BadgeEvents[(int)NewEventType].m_Count = Count;
		m_BadgeEvents[(int)NewEventType].Save();
	}

	public BadgeEvent GetEvent(BadgeEvent.Type NewEventType)
	{
		return m_BadgeEvents[(int)NewEventType];
	}

	public Badge GetBadge(Badge.ID NewID)
	{
		return m_BadgeData.m_Badges[(int)NewID];
	}

	public void ForceBadgeComplete(Badge NewBadge)
	{
		NewBadge.ForceComplete(Complete: true);
		NewBadge.Save();
	}

	private void Update()
	{
		foreach (Badge badge in m_BadgeData.m_Badges)
		{
			if (!badge.m_Complete && badge.GetIsComplete())
			{
				if (m_Debug)
				{
					Debug.Log("Badge completed " + badge.m_ID);
				}
				if ((bool)CeremonyManager.Instance)
				{
					CeremonyManager.Instance.AddCeremony(CeremonyManager.CeremonyType.BadgeRevealed, badge);
				}
				SteamTest.Instance.UnlockAchievement(badge);
				AnalyticsManager.BadgeComplete(badge);
				m_TimeSinceLastSave = m_DelayBetweenSaves;
			}
		}
		m_TimeSinceLastSave += Time.deltaTime;
		if (m_Save && m_TimeSinceLastSave >= m_DelayBetweenSaves)
		{
			m_TimeSinceLastSave = 0f;
			m_Save = false;
			Save();
		}
		if (!m_LogCounts)
		{
			return;
		}
		string text = "";
		foreach (KeyValuePair<BadgeEvent.Type, int> count in m_Counts)
		{
			text = text + count.Key.ToString() + ":" + count.Value + " ";
		}
		Debug.Log(text);
	}
}
