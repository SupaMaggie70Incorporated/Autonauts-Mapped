using System;
using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;

public class OffworldMissionsManager : MonoBehaviour
{
	public static OffworldMissionsManager Instance;

	private static int m_NumRegularMissionsAvailable = 3;

	public OffworldMission m_SelectedMission;

	public List<OffworldMission> m_RegularMissionsAvailable;

	public OffworldMission m_DailyMissionAvailable;

	private DateTime m_DailyMissionDate;

	public List<OffworldMission> m_RegularMissionsCompleted;

	public List<OffworldMission> m_DailyMissionsCompleted;

	private Dictionary<string, int> m_Reputation;

	private int m_MissionID;

	public int m_Tickets;

	private List<ObjectType> m_PrizesUnlocked;

	private void Awake()
	{
		Instance = this;
		m_SelectedMission = null;
		m_RegularMissionsAvailable = new List<OffworldMission>();
		m_DailyMissionAvailable = null;
		m_DailyMissionDate = DateTime.MinValue;
		m_RegularMissionsCompleted = new List<OffworldMission>();
		m_DailyMissionsCompleted = new List<OffworldMission>();
		m_Reputation = new Dictionary<string, int>();
		m_MissionID = 0;
		InitTickets();
		if (GameOptionsManager.Instance.m_Options.m_GameMode != 0)
		{
			SetupRegularMissions();
			CheckDailyMission();
		}
	}

	public void SaveMissionList(JSONNode rootNode, string NodeName, List<OffworldMission> MissionList)
	{
		JSONArray jSONArray = (JSONArray)(rootNode[NodeName] = new JSONArray());
		int num = 0;
		foreach (OffworldMission Mission in MissionList)
		{
			JSONNode jSONNode2 = new JSONObject();
			Mission.Save(jSONNode2);
			jSONArray[num] = jSONNode2;
			num++;
		}
	}

	private List<OffworldMission> LoadMissionList(JSONNode rootNode, string NodeName, bool Daily)
	{
		List<OffworldMission> list = new List<OffworldMission>();
		JSONArray asArray = rootNode[NodeName].AsArray;
		for (int i = 0; i < asArray.Count; i++)
		{
			JSONNode asObject = asArray[i].AsObject;
			OffworldMission offworldMission = new OffworldMission(Daily);
			offworldMission.Load(asObject);
			list.Add(offworldMission);
		}
		return list;
	}

	public void SaveReputation(JSONNode rootNode)
	{
		JSONArray jSONArray = (JSONArray)(rootNode["Rep"] = new JSONArray());
		int num = 0;
		foreach (KeyValuePair<string, int> item in m_Reputation)
		{
			JSONNode jSONNode2 = new JSONObject();
			jSONNode2["Name"] = item.Key;
			jSONNode2["Points"] = item.Value;
			jSONArray[num] = jSONNode2;
			num++;
		}
	}

	private void LoadReputation(JSONNode rootNode)
	{
		m_Reputation = new Dictionary<string, int>();
		JSONArray asArray = rootNode["Rep"].AsArray;
		for (int i = 0; i < asArray.Count; i++)
		{
			JSONObject asObject = asArray[i].AsObject;
			string text = asObject["Name"];
			int num = asObject["Points"];
			if (Array.IndexOf(OffworldMission.m_PlanetNames, text) > -1)
			{
				if (!m_Reputation.ContainsKey(text))
				{
					m_Reputation.Add(text, 0);
				}
				m_Reputation[text] += num;
			}
		}
	}

	public void Save(JSONNode rootNode)
	{
		JSONUtils.Set(rootNode, "ID", m_MissionID);
		SaveMissionList(rootNode, "Regular", m_RegularMissionsAvailable);
		if (m_DailyMissionAvailable != null)
		{
			rootNode["Daily"] = new JSONObject();
			JSONNode rootNode2 = rootNode["Daily"];
			m_DailyMissionAvailable.Save(rootNode2);
		}
		if (m_DailyMissionDate != DateTime.MinValue)
		{
			rootNode["DailyDate"] = m_DailyMissionDate.ToString("yyyy-MM-dd");
		}
		SaveMissionList(rootNode, "RegularComplete", m_RegularMissionsCompleted);
		SaveMissionList(rootNode, "DailyComplete", m_DailyMissionsCompleted);
		SaveReputation(rootNode);
		SavePrizes(rootNode);
	}

	public void Load(JSONNode rootNode)
	{
		m_MissionID = JSONUtils.GetAsInt(rootNode, "ID", 0);
		m_RegularMissionsAvailable = LoadMissionList(rootNode, "Regular", Daily: false);
		JSONNode jSONNode = rootNode["Daily"];
		if (jSONNode != null && !jSONNode.IsNull)
		{
			m_DailyMissionAvailable = new OffworldMission(Daily: true);
			m_DailyMissionAvailable.Load(jSONNode);
		}
		string text = rootNode["DailyDate"];
		m_DailyMissionDate = DateTime.MinValue;
		if (text != null && text != "")
		{
			DateTime.TryParse(text, out m_DailyMissionDate);
		}
		m_RegularMissionsCompleted = LoadMissionList(rootNode, "RegularComplete", Daily: false);
		m_DailyMissionsCompleted = LoadMissionList(rootNode, "DailyComplete", Daily: true);
		if (m_DailyMissionAvailable != null && m_DailyMissionAvailable.m_Selected)
		{
			SelectMission(m_DailyMissionAvailable);
		}
		else
		{
			CheckDailyMission();
			foreach (OffworldMission item in m_RegularMissionsAvailable)
			{
				if (item.m_Selected)
				{
					SelectMission(item);
				}
			}
		}
		LoadReputation(rootNode);
		LoadPrizes(rootNode);
	}

	public void SelectMission(OffworldMission NewMission)
	{
		if (m_SelectedMission != null)
		{
			m_SelectedMission.m_Selected = false;
			if (m_SelectedMission.m_Daily)
			{
				CheckDailyMission();
			}
		}
		m_SelectedMission = NewMission;
		if (m_SelectedMission != null)
		{
			m_SelectedMission.m_Selected = true;
			AnalyticsManager.OffworldMissionStarted(m_SelectedMission);
		}
	}

	private List<ObjectType> GetPossibleObjectTypes(bool Daily)
	{
		List<ObjectType> list = new List<ObjectType>();
		for (int i = 0; i < 673; i++)
		{
			ObjectType objectType = (ObjectType)i;
			if (ObjectTypeList.Instance.GetMissionUsableFromType(objectType) && !QuestManager.Instance.m_ObjectsLocked.ContainsKey(objectType) && (!Vehicle.GetIsTypeVehicle(objectType) || !QuestManager.Instance.m_ObjectsLocked.ContainsKey(ObjectType.CraneCrude)) && objectType != ObjectType.FarmerPlayer)
			{
				list.Add(objectType);
			}
		}
		return list;
	}

	private ObjectType GetMissionObjectType(List<ObjectType> Exclude, bool Daily)
	{
		ObjectType total = ObjectTypeList.m_Total;
		List<ObjectType> possibleObjectTypes = GetPossibleObjectTypes(Daily);
		int num = 0;
		do
		{
			int index = UnityEngine.Random.Range(0, possibleObjectTypes.Count);
			total = possibleObjectTypes[index];
			num++;
		}
		while (Exclude != null && Exclude.Contains(total) && num < 1000);
		if (num == 1000)
		{
			ErrorMessage.LogError("Couldn't find random object type for mission  vOv");
		}
		return total;
	}

	private int GetMissionObjectCount(ObjectType NewType)
	{
		int num = VariableManager.Instance.GetVariableAsInt("MissionAmountTier0");
		int variableAsInt = VariableManager.Instance.GetVariableAsInt(NewType, "MissionAmount", CheckValid: false);
		if (variableAsInt > 0)
		{
			num = variableAsInt;
		}
		else
		{
			int tier = ObjectTypeList.Instance.GetTier(NewType);
			if (tier > 0)
			{
				num = VariableManager.Instance.GetVariableAsInt("MissionAmountTier" + tier);
			}
		}
		int num2 = num * VariableManager.Instance.GetVariableAsInt("MissionAmountVariation") / 100;
		int num3 = num - num2 / 2;
		return UnityEngine.Random.Range(num3, num3 + num2);
	}

	private OffworldMission SetupRegularMission()
	{
		OffworldMission offworldMission = new OffworldMission(Daily: false, m_MissionID++);
		ObjectType missionObjectType = GetMissionObjectType(null, Daily: false);
		int missionObjectCount = GetMissionObjectCount(missionObjectType);
		offworldMission.AddRequirement(missionObjectType, missionObjectCount);
		offworldMission.CalcTicketAmount();
		return offworldMission;
	}

	public void SetupRegularMissions()
	{
		if (m_RegularMissionsAvailable.Count == 0)
		{
			UnityEngine.Random.InitState((int)DateTime.Now.Ticks);
			m_RegularMissionsAvailable = new List<OffworldMission>();
			for (int i = 0; i < m_NumRegularMissionsAvailable; i++)
			{
				OffworldMission item = SetupRegularMission();
				m_RegularMissionsAvailable.Add(item);
			}
		}
	}

	public void SetupDailyMission()
	{
		if (m_DailyMissionAvailable == null)
		{
			UnityEngine.Random.InitState((int)DateTime.Now.Subtract(DateTime.MinValue).TotalDays);
			m_DailyMissionAvailable = new OffworldMission(Daily: true, m_MissionID++);
			m_DailyMissionDate = DateTime.Now;
			int num = 3;
			List<ObjectType> list = new List<ObjectType>();
			for (int i = 0; i < num; i++)
			{
				ObjectType missionObjectType = GetMissionObjectType(list, Daily: true);
				list.Add(missionObjectType);
				int missionObjectCount = GetMissionObjectCount(missionObjectType);
				m_DailyMissionAvailable.AddRequirement(missionObjectType, missionObjectCount);
			}
			m_DailyMissionAvailable.CalcTicketAmount();
		}
	}

	public void DiscardRegularMission(OffworldMission NewMission)
	{
		for (int i = 0; i < m_RegularMissionsAvailable.Count; i++)
		{
			if (m_RegularMissionsAvailable[i] == NewMission)
			{
				m_RegularMissionsAvailable[i] = SetupRegularMission();
				break;
			}
		}
		if (m_SelectedMission == NewMission)
		{
			SelectMission(null);
		}
	}

	public bool GetIsDailyMissionSameDate()
	{
		DateTime now = DateTime.Now;
		if (m_DailyMissionDate.Year != now.Year || m_DailyMissionDate.Month != now.Month || m_DailyMissionDate.Day != now.Day)
		{
			return false;
		}
		return true;
	}

	private void CheckDailyMission()
	{
		if (QuestManager.Instance.GetQuestComplete(Quest.ID.AcademyColonisation8))
		{
			bool isDailyMissionSameDate = GetIsDailyMissionSameDate();
			if (m_DailyMissionAvailable != null && !isDailyMissionSameDate)
			{
				m_DailyMissionAvailable = null;
			}
			if (m_DailyMissionAvailable == null && !isDailyMissionSameDate)
			{
				SetupDailyMission();
			}
		}
	}

	public bool GetIsObjectTypeRequired(ObjectType NewType)
	{
		if (m_SelectedMission != null && m_SelectedMission.GetIsObjectTypeRequired(NewType))
		{
			return true;
		}
		return false;
	}

	private void AddReputation(OffworldMission NewMission)
	{
		int reputationPoints = NewMission.GetReputationPoints();
		int planet = NewMission.m_Planet;
		string key = OffworldMission.m_PlanetNames[planet];
		if (!m_Reputation.ContainsKey(key))
		{
			m_Reputation.Add(key, 0);
		}
		m_Reputation[key] += reputationPoints;
	}

	private void ProcessCompleteMission()
	{
		OffworldMission selectedMission = m_SelectedMission;
		if (GameStateManager.Instance.GetActualState() == GameStateManager.State.SetSpacePort)
		{
			GameStateManager.Instance.PopState();
		}
		CeremonyManager.Instance.AddCeremony(CeremonyManager.CeremonyType.OffworldMissionComplete, selectedMission);
		CheckCompletedMissions();
		AwardTickets(selectedMission.GetTickets());
		AddReputation(selectedMission);
		AnalyticsManager.OffworldMissionComplete(selectedMission);
	}

	public OffworldMission AddObjectType(ObjectType NewType)
	{
		if (m_SelectedMission != null && m_SelectedMission.AddObject(NewType))
		{
			OffworldMission selectedMission = m_SelectedMission;
			if (selectedMission.m_Complete)
			{
				ProcessCompleteMission();
			}
			return selectedMission;
		}
		return null;
	}

	public void CheckCompletedMissions()
	{
		if (m_SelectedMission != null && m_SelectedMission.m_Complete)
		{
			OffworldMission selectedMission = m_SelectedMission;
			if (!selectedMission.m_Daily)
			{
				DiscardRegularMission(m_SelectedMission);
				m_RegularMissionsCompleted.Add(selectedMission);
				return;
			}
			SelectMission(null);
			m_DailyMissionAvailable = null;
			m_DailyMissionsCompleted.Add(selectedMission);
			CheckDailyMission();
		}
	}

	public void CompleteSelected()
	{
		if (m_SelectedMission != null)
		{
			m_SelectedMission.ForceComplete();
			ProcessCompleteMission();
		}
	}

	private void InitTickets()
	{
		m_Tickets = 0;
		m_PrizesUnlocked = new List<ObjectType>();
	}

	private void SavePrizes(JSONNode rootNode)
	{
		JSONUtils.Set(rootNode, "T", m_Tickets);
		JSONArray jSONArray = (JSONArray)(rootNode["PU"] = new JSONArray());
		int num = 0;
		foreach (ObjectType item in m_PrizesUnlocked)
		{
			string saveNameFromIdentifier = ObjectTypeList.Instance.GetSaveNameFromIdentifier(item);
			jSONArray[num] = saveNameFromIdentifier;
			num++;
		}
	}

	private void LoadPrizes(JSONNode rootNode)
	{
		m_Tickets = JSONUtils.GetAsInt(rootNode, "T", 0);
		m_PrizesUnlocked = new List<ObjectType>();
		JSONArray asArray = rootNode["PU"].AsArray;
		for (int i = 0; i < asArray.Count; i++)
		{
			ObjectType identifierFromSaveName = ObjectTypeList.Instance.GetIdentifierFromSaveName(asArray[i]);
			if (identifierFromSaveName != ObjectTypeList.m_Total)
			{
				m_PrizesUnlocked.Add(identifierFromSaveName);
				QuestManager.Instance.UnlockObject(identifierFromSaveName);
			}
		}
	}

	public void AwardTickets(int Amount)
	{
		m_Tickets += Amount;
	}

	public void BuyPrize(ObjectType NewType, bool Cheat = false)
	{
		if (!Cheat)
		{
			int variableAsInt = VariableManager.Instance.GetVariableAsInt(NewType, "PrizeCost");
			m_Tickets -= variableAsInt;
		}
		m_PrizesUnlocked.Add(NewType);
		QuestManager.Instance.UnlockObject(NewType);
		if (Cheat)
		{
			return;
		}
		List<ObjectType> list = new List<ObjectType>();
		list.Add(NewType);
		CeremonyManager.Instance.StartImmediateCeremony(CeremonyManager.CeremonyType.OffworldObjectUnlocked, null, list);
		if (ObjectTypeList.Instance.GetIsBuilding(NewType))
		{
			if (GameOptionsManager.Instance.m_Options.m_GameMode != GameOptions.GameMode.ModeCreative)
			{
				ResourceManager.Instance.AddResource(NewType, 1);
			}
			return;
		}
		Dictionary<BaseClass, int> collection = CollectionManager.Instance.GetCollection("SpacePort");
		SpacePort spacePort = null;
		if (collection != null && collection.Count > 0)
		{
			using Dictionary<BaseClass, int>.Enumerator enumerator = collection.GetEnumerator();
			if (enumerator.MoveNext())
			{
				spacePort = enumerator.Current.Key.GetComponent<SpacePort>();
			}
		}
		if ((bool)spacePort)
		{
			TileCoord spawnPoint = spacePort.GetSpawnPoint();
			BaseClass baseClass = ObjectTypeList.Instance.CreateObjectFromIdentifier(NewType, spawnPoint.ToWorldPositionTileCentered(), Quaternion.identity);
			SpawnAnimationManager.Instance.AddJump(baseClass, spawnPoint, spawnPoint, 0f, baseClass.transform.position.y, 3f);
		}
	}
}
