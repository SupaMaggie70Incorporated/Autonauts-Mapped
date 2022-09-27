using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;

public class FolkManager : MonoBehaviour
{
	public static FolkManager Instance;

	private List<Folk> m_OwnedFolks;

	public int[] m_FolkTierCounts;

	public int m_HighestActiveFolk;

	private bool[] m_FirstFolk;

	private bool m_UpdateHappy;

	private int m_Happy;

	private bool m_UpdateHoused;

	private int m_Housed;

	private List<Folk> m_TranscendedFolk;

	private void Awake()
	{
		Instance = this;
		m_OwnedFolks = new List<Folk>();
		m_UpdateHappy = false;
		m_Happy = 0;
		m_UpdateHoused = false;
		m_Housed = 0;
		int num = 20;
		m_FolkTierCounts = new int[num];
		m_FirstFolk = new bool[num];
		for (int i = 0; i < num; i++)
		{
			m_FirstFolk[i] = true;
		}
		m_TranscendedFolk = new List<Folk>();
	}

	public void Save(JSONNode Node)
	{
		JSONArray jSONArray = (JSONArray)(Node["First"] = new JSONArray());
		for (int i = 0; i < m_FirstFolk.Length; i++)
		{
			jSONArray[i] = m_FirstFolk[i];
		}
		JSONArray jSONArray2 = (JSONArray)(Node["Folk"] = new JSONArray());
		int num = 0;
		for (int j = 0; j < m_TranscendedFolk.Count; j++)
		{
			if ((bool)m_TranscendedFolk[j])
			{
				JSONNode jSONNode3 = new JSONObject();
				string saveNameFromIdentifier = ObjectTypeList.Instance.GetSaveNameFromIdentifier(m_TranscendedFolk[j].GetComponent<BaseClass>().m_TypeIdentifier);
				JSONUtils.Set(jSONNode3, "ID", saveNameFromIdentifier);
				m_TranscendedFolk[j].GetComponent<Savable>().Save(jSONNode3);
				jSONArray2[num] = jSONNode3;
				num++;
			}
		}
	}

	public void Load(JSONNode Node)
	{
		JSONArray asArray = Node["First"].AsArray;
		for (int i = 0; i < asArray.Count; i++)
		{
			m_FirstFolk[i] = asArray[i].AsBool;
		}
		JSONArray asArray2 = Node["Folk"].AsArray;
		for (int j = 0; j < asArray2.Count; j++)
		{
			JSONNode asObject = asArray2[j].AsObject;
			int asInt = JSONUtils.GetAsInt(asObject, "UID", -1);
			BaseClass objectFromUniqueID = ObjectTypeList.Instance.GetObjectFromUniqueID(asInt, ErrorCheck: false);
			if (objectFromUniqueID != null && objectFromUniqueID.GetComponent<TileCoordObject>().m_Plot != null)
			{
				objectFromUniqueID.StopUsing();
			}
			if (ObjectTypeList.Instance.GetObjectFromUniqueID(asInt, ErrorCheck: false) == null)
			{
				BaseClass baseClass = ObjectTypeList.Instance.CreateObjectFromSaveName(asObject, new Vector3(0f, 0f, 0f), Quaternion.identity);
				if ((bool)baseClass)
				{
					baseClass.GetComponent<Savable>().Load(asObject);
					Folk component = baseClass.GetComponent<Folk>();
					AddLoadedFolk(component);
				}
			}
		}
	}

	public void AddTranscendedFolk(Folk NewFolk)
	{
		NewFolk.gameObject.SetActive(value: false);
		NewFolk.SetTier(7);
		m_TranscendedFolk.Add(NewFolk);
		UpdateFolkTiers();
		Evolution.SetLastLevelSeen(7);
	}

	public void AddLoadedFolk(Folk NewFolk)
	{
		NewFolk.SendAction(new ActionInfo(ActionType.BeingHeld, NewFolk.m_TileCoord));
		NewFolk.GetComponent<Savable>().SetIsSavable(IsSavable: false);
		NewFolk.gameObject.SetActive(value: false);
		m_TranscendedFolk.Add(NewFolk);
	}

	public int GetTotalFolk()
	{
		return m_OwnedFolks.Count;
	}

	private QuestEvent GetTranscendEvent()
	{
		Quest quest = QuestManager.Instance.GetQuest(Quest.ID.AcademyColonisation8);
		if (quest != null)
		{
			foreach (QuestEvent item in quest.m_EventsRequired)
			{
				if (item.m_Type == QuestEvent.Type.FolkTranscended)
				{
					return item;
				}
			}
		}
		return null;
	}

	public float GetTranscendPercent()
	{
		QuestEvent transcendEvent = GetTranscendEvent();
		if (transcendEvent != null)
		{
			if (transcendEvent.m_Required == 0)
			{
				return 0f;
			}
			return (float)transcendEvent.m_Progress / (float)transcendEvent.m_Required;
		}
		return 0f;
	}

	public int GetTranscended()
	{
		return GetTranscendEvent()?.m_Progress ?? 0;
	}

	public int GetTranscendTarget()
	{
		return GetTranscendEvent()?.m_Required ?? 0;
	}

	public int GetTranscendLeft()
	{
		QuestEvent transcendEvent = GetTranscendEvent();
		if (transcendEvent != null)
		{
			return transcendEvent.m_Required - transcendEvent.m_Progress;
		}
		return 0;
	}

	public int GetFedFolk()
	{
		Dictionary<BaseClass, int> collection = CollectionManager.Instance.GetCollection("Folk");
		int num = 0;
		foreach (KeyValuePair<BaseClass, int> item in collection)
		{
			if (item.Key.GetComponent<Folk>().m_Energy > 0f)
			{
				num++;
			}
		}
		return num;
	}

	public int GetHousedFolk()
	{
		Dictionary<BaseClass, int> collection = CollectionManager.Instance.GetCollection("Folk");
		int num = 0;
		foreach (KeyValuePair<BaseClass, int> item in collection)
		{
			if ((bool)item.Key.GetComponent<Folk>().m_Housing)
			{
				num++;
			}
		}
		return num;
	}

	public void AddFolk(Folk NewFolk)
	{
		if (!m_OwnedFolks.Contains(NewFolk))
		{
			m_OwnedFolks.Add(NewFolk);
			StartUpdateHappy();
		}
		UpdateFolkTiers();
	}

	public void RemoveFolk(Folk NewFolk)
	{
		if (m_OwnedFolks.Contains(NewFolk))
		{
			m_OwnedFolks.Remove(NewFolk);
			StartUpdateHappy();
		}
		UpdateFolkTiers();
	}

	public void StartUpdateHappy()
	{
		m_UpdateHappy = true;
	}

	public int GetHappy()
	{
		return m_Happy;
	}

	private void UpdateHappy()
	{
		int num = 0;
		foreach (Folk ownedFolk in m_OwnedFolks)
		{
			if (ownedFolk.m_Happiness == 1f)
			{
				num++;
			}
		}
		if (num != m_Happy)
		{
			m_Happy = num;
			QuestManager.Instance.AddEvent(QuestEvent.Type.MakeFolkHappy, Bot: false, 0, null);
		}
	}

	public void UpdateFolkTiers()
	{
		for (int i = 0; i < m_FolkTierCounts.Length; i++)
		{
			m_FolkTierCounts[i] = 0;
		}
		m_HighestActiveFolk = 0;
		foreach (Folk ownedFolk in m_OwnedFolks)
		{
			int tier = ownedFolk.GetTier();
			m_FolkTierCounts[tier]++;
			if (tier > m_HighestActiveFolk)
			{
				m_HighestActiveFolk = tier;
				ModeButton.Get(ModeButton.Type.Evolution).UpdateFolkLevel();
			}
		}
	}

	public bool IsFirstFolk(int Tier)
	{
		return m_FirstFolk[Tier];
	}

	public void RegisterFirstFolk(int Tier)
	{
		m_FirstFolk[Tier] = false;
	}

	public void StartUpdateHoused()
	{
		m_UpdateHoused = true;
	}

	public int GetHoused()
	{
		return m_Housed;
	}

	private void UpdateHoused()
	{
		int num = 0;
		foreach (Folk ownedFolk in m_OwnedFolks)
		{
			if (ownedFolk.m_Housing != null)
			{
				num++;
			}
		}
		if (num != m_Housed)
		{
			m_Housed = num;
			QuestManager.Instance.AddEvent(QuestEvent.Type.MakeFolkHoused, Bot: false, 0, null);
		}
	}

	private void Update()
	{
		if (m_UpdateHappy)
		{
			m_UpdateHappy = false;
			UpdateHappy();
		}
		if (m_UpdateHoused)
		{
			m_UpdateHoused = false;
			UpdateHoused();
		}
	}
}
