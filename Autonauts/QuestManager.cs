using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
	public static QuestManager Instance;

	public Quest.ID m_CurrentEra;

	public QuestData m_Data;

	public List<Quest> m_ActiveQuests;

	public Dictionary<Quest.ID, Quest> m_CompletedQuests;

	private List<Quest>[] m_ActiveEvents;

	private bool m_Started;

	public bool m_ActiveQuestsStarted;

	public Dictionary<ObjectType, int> m_BuildingsLocked;

	public Dictionary<ObjectType, int> m_ObjectsLocked;

	private int m_PlayerMove;

	private int m_WorkerMove;

	private Dictionary<BaseClass, int> m_XPEffects;

	private void Awake()
	{
		Instance = this;
		QuestEvent.Init();
		m_ActiveQuests = new List<Quest>();
		m_CompletedQuests = new Dictionary<Quest.ID, Quest>();
		m_BuildingsLocked = new Dictionary<ObjectType, int>();
		m_ObjectsLocked = new Dictionary<ObjectType, int>();
		m_XPEffects = new Dictionary<BaseClass, int>();
		m_Data = new QuestData();
		m_Data.Init();
		AnimalStatusIndicator.m_NeedPenVisible = false;
	}

	public void LockQuests()
	{
		for (int i = 0; i < (int)ObjectTypeList.m_Total; i++)
		{
			ObjectType newType = (ObjectType)i;
			bool flag = false;
			if (GameOptionsManager.Instance.m_Options.m_GameMode == GameOptions.GameMode.ModeCampaign && VariableManager.Instance.GetVariableAsInt(newType, "Unlocked", CheckValid: false) != 1)
			{
				flag = true;
			}
			if (GameOptionsManager.Instance.m_Options.m_GameMode != 0 && ObjectTypeList.Instance.GetCategoryFromType(newType) == ObjectCategory.Prizes)
			{
				flag = true;
			}
			if (flag)
			{
				LockObject((ObjectType)i);
			}
		}
		m_Started = false;
		m_ActiveQuestsStarted = false;
		m_PlayerMove = 0;
		m_WorkerMove = 0;
	}

	public void Save(JSONNode Node)
	{
		JSONArray jSONArray = (JSONArray)(Node["Active"] = new JSONArray());
		int num = 0;
		foreach (Quest activeQuest in m_ActiveQuests)
		{
			JSONNode jSONNode2 = new JSONObject();
			activeQuest.Save(jSONNode2);
			jSONArray[num] = jSONNode2;
			num++;
		}
		JSONArray jSONArray2 = (JSONArray)(Node["Completed"] = new JSONArray());
		num = 0;
		foreach (KeyValuePair<Quest.ID, Quest> completedQuest in m_CompletedQuests)
		{
			JSONNode jSONNode4 = new JSONObject();
			completedQuest.Value.Save(jSONNode4);
			jSONArray2[num] = jSONNode4;
			num++;
		}
		TutorialPanelController.Instance.Save(Node);
	}

	private bool IsQuestAdded(Quest.ID NewID)
	{
		foreach (Quest activeQuest in m_ActiveQuests)
		{
			if (activeQuest.m_ID == NewID)
			{
				return true;
			}
		}
		if (m_CompletedQuests.ContainsKey(NewID))
		{
			return true;
		}
		return false;
	}

	private void DumpActive()
	{
		Debug.Log("******* Active");
		foreach (Quest activeQuest in m_ActiveQuests)
		{
			Debug.Log(activeQuest.m_ID);
		}
		Debug.Log("******* Complete");
		foreach (KeyValuePair<Quest.ID, Quest> completedQuest in m_CompletedQuests)
		{
			Debug.Log(completedQuest.Value.m_ID);
		}
	}

	public void Load(JSONNode Node)
	{
		Reset();
		JSONArray asArray = Node["Active"].AsArray;
		for (int i = 0; i < asArray.Count; i++)
		{
			JSONNode node = asArray[i];
			string asString = JSONUtils.GetAsString(node, "Name", "");
			Quest.ID questIDFromName = m_Data.GetQuestIDFromName(asString);
			if (questIDFromName != Quest.ID.Total)
			{
				m_Data.GetQuest(questIDFromName)?.Load(node);
			}
		}
		CreateActiveEventsArray();
		JSONArray asArray2 = Node["Completed"].AsArray;
		for (int j = 0; j < asArray2.Count; j++)
		{
			JSONNode node = asArray2[j];
			string asString = JSONUtils.GetAsString(node, "Name", "");
			Quest.ID questIDFromName2 = m_Data.GetQuestIDFromName(asString);
			if (questIDFromName2 == Quest.ID.Total)
			{
				continue;
			}
			Quest quest = m_Data.GetQuest(questIDFromName2);
			if (quest != null)
			{
				quest.Load(node);
				if (m_ActiveQuests.Contains(quest))
				{
					m_ActiveQuests.Remove(quest);
				}
				if (!m_CompletedQuests.ContainsKey(quest.m_ID))
				{
					m_CompletedQuests.Add(quest.m_ID, quest);
					DoUnlocked(quest, ShowNew: false);
				}
			}
		}
		List<Quest> list = new List<Quest>();
		foreach (KeyValuePair<Quest.ID, Quest> completedQuest in m_CompletedQuests)
		{
			list.Add(completedQuest.Value);
		}
		foreach (Quest item in list)
		{
			foreach (Quest.ID item2 in item.m_QuestsUnlocked)
			{
				if (!IsQuestAdded(item2))
				{
					AddQuest(item2);
				}
			}
		}
		Quest[] questData = m_Data.m_QuestData;
		foreach (Quest quest2 in questData)
		{
			if (quest2 != null && (quest2.m_Simple || quest2.m_ObjectTypeRequired != ObjectTypeList.m_Total) && !m_Data.DoesQuestUnlockQuest(quest2.m_ID) && !IsQuestAdded(quest2.m_ID))
			{
				AddQuest(quest2.m_ID);
			}
		}
		TabQuests.Instance.UpdateLists();
		QuestManagerTiles.Instance.Init();
		questData = m_Data.m_QuestData;
		for (int k = 0; k < questData.Length; k++)
		{
			questData[k]?.UpdateEventsCompletable();
		}
		m_ActiveQuestsStarted = true;
		TutorialPanelController.Instance.Load(Node);
		list = new List<Quest>();
		foreach (Quest activeQuest in m_ActiveQuests)
		{
			if (activeQuest.GetIsComplete())
			{
				list.Add(activeQuest);
			}
		}
		foreach (Quest item3 in list)
		{
			QuestCompleted(item3, DoCeremony: false);
		}
		m_Data.m_ResearchData.CheckQuestsUnlocked();
		OldFileUtils.CheckQuests();
	}

	public void TutorialFinished()
	{
		if (SettingsManager.Instance.m_TutorialEnabled)
		{
			SettingsManager.Instance.SetTutorialEnabled(Enabled: false);
			SettingsManager.Instance.Save();
		}
		UnlockObject(ObjectType.Workbench);
		UnlockObject(ObjectType.ToolAxeStone);
		UnlockObject(ObjectType.WorkerAssembler);
		UnlockObject(ObjectType.WorkerDriveMk0);
		UnlockObject(ObjectType.WorkerFrameMk0);
		UnlockObject(ObjectType.WorkerHeadMk0);
		UnlockObject(ObjectType.BasicWorker);
		UnlockObject(ObjectType.ToolShovelStone);
		UnlockObject(ObjectType.ToolPickStone);
		UnlockObject(ObjectType.ResearchStationCrude);
		UnlockObject(ObjectType.FolkSeed);
		UnlockObject(ObjectType.FolkSeedPod);
		UnlockObject(ObjectType.FolkSeedRehydrator);
		UnlockObject(ObjectType.StorageGeneric);
		UnlockObject(ObjectType.StoragePalette);
		ModeButton.Get(ModeButton.Type.BuildingPalette).Show();
		ModeButton.Get(ModeButton.Type.Autopedia).Show();
		AddQuest(Quest.ID.AcademyForestry);
		Quest[] questData = m_Data.m_QuestData;
		foreach (Quest quest in questData)
		{
			if (quest != null && quest.m_Type == Quest.Type.Glue)
			{
				AddQuest(quest.m_ID);
			}
		}
		m_Data.m_ResearchData.StartLevels();
		if (GameOptionsManager.Instance.m_Options.m_GameMode == GameOptions.GameMode.ModeCampaign)
		{
			TabManager.Instance.ActivateTab(TabManager.TabType.Quests);
		}
	}

	public void StartResearch()
	{
		m_Data.m_ResearchData.StartLevels();
	}

	public void CreateActiveEventsArray()
	{
		int num = 206;
		m_ActiveEvents = new List<Quest>[num];
		for (int i = 0; i < num; i++)
		{
			m_ActiveEvents[i] = new List<Quest>();
		}
		foreach (Quest activeQuest in m_ActiveQuests)
		{
			if (activeQuest.m_Complete)
			{
				continue;
			}
			foreach (QuestEvent item in activeQuest.m_EventsRequired)
			{
				if (!m_ActiveEvents[(int)item.m_Type].Contains(activeQuest))
				{
					m_ActiveEvents[(int)item.m_Type].Add(activeQuest);
				}
			}
		}
	}

	public void AddQuest(Quest.ID NewID, bool DoCeremony = true)
	{
		Quest quest = m_Data.GetQuest(NewID);
		if (quest != null && !quest.m_Started)
		{
			quest.m_Started = true;
			quest.SetActive();
			m_ActiveQuestsStarted = true;
			if (quest.m_Type == Quest.Type.Research)
			{
				NewIconManager.Instance.NewResearchQuestUnlocked(quest.m_ID);
			}
			if (quest.GetIsComplete())
			{
				QuestCompleted(quest, DoCeremony);
			}
			quest.UpdateEventsCompletable();
		}
	}

	public void RemoveQuest(Quest.ID NewID)
	{
		Quest quest = m_Data.GetQuest(NewID);
		if (quest != null && quest.m_Started)
		{
			quest.m_Started = false;
			quest.m_Active = false;
		}
	}

	private void ShowQuest(Quest NewQuest, bool DoCeremony = true)
	{
		NewQuest.SetActive();
	}

	private void RemoveActiveQuest(Quest NewQuest)
	{
		foreach (QuestEvent item in NewQuest.m_EventsRequired)
		{
			m_ActiveEvents[(int)item.m_Type].Remove(NewQuest);
		}
		m_ActiveQuests.Remove(NewQuest);
	}

	public void ResetQuest(Quest NewQuest)
	{
		if (m_CompletedQuests.ContainsKey(NewQuest.m_ID))
		{
			m_CompletedQuests.Remove(NewQuest.m_ID);
			m_ActiveQuests.Add(NewQuest);
		}
		NewQuest.Reset();
		RemoveQuest(NewQuest.m_ID);
	}

	private void TestLockedBuildings()
	{
		Dictionary<BaseClass, int> collection = CollectionManager.Instance.GetCollection("Converter");
		if (collection == null)
		{
			return;
		}
		foreach (KeyValuePair<BaseClass, int> item in collection)
		{
			if (item.Key.m_TypeIdentifier == ObjectType.FolkSeedPod)
			{
				item.Key.GetComponent<FolkSeedPod>().UpdateLocked();
			}
			if (item.Key.m_TypeIdentifier == ObjectType.FolkSeedRehydrator)
			{
				item.Key.GetComponent<FolkSeedRehydrator>().UpdateLocked();
			}
		}
	}

	private void QuestCompleted(Quest NewQuest, bool DoCeremony = true)
	{
		if (DoCeremony)
		{
			if (NewQuest.m_BuildingsUnlocked.Contains(ObjectType.FolkSeedPod))
			{
				RainManager.Instance.SetEnabled(Enabled: false);
			}
			List<ObjectType> list = new List<ObjectType>();
			foreach (ObjectType item in NewQuest.m_BuildingsUnlocked)
			{
				if (m_BuildingsLocked.ContainsKey(item) && QuestData.DoesUnlockedObjectHaveCeremony(item))
				{
					list.Add(item);
					if (VariableManager.Instance.GetVariableAsInt(item, "UpgradeFrom", CheckValid: false) != 0)
					{
						NewIconManager.Instance.NewObjectUnlocked(item);
					}
					else
					{
						BuildingPalette.AddNewUnlocked(item);
						ObjectsPanels.AddNewUnlocked(item);
					}
				}
				ModeButton.Get(ModeButton.Type.BuildingPalette).Show();
			}
			foreach (ObjectType item2 in NewQuest.m_ObjectsUnlocked)
			{
				if (m_ObjectsLocked.ContainsKey(item2) && QuestData.DoesUnlockedObjectHaveCeremony(item2))
				{
					list.Add(item2);
					NewIconManager.Instance.NewObjectUnlocked(item2);
					ObjectsPanels.AddNewUnlocked(item2);
					if (Clothing.GetIsTypeClothing(item2))
					{
						WardrobeManager.Instance.AddNewObject(item2);
					}
				}
			}
			if (NewQuest.m_ID == Quest.ID.AcademyColonisation8)
			{
				CeremonyManager.Instance.AddCeremony(CeremonyManager.CeremonyType.GameComplete, NewQuest, null);
			}
			else
			{
				CeremonyManager.Instance.AddCeremony(NewQuest.m_CeremonyType, NewQuest, list);
			}
			if (NewQuest.m_MajorNode)
			{
				ModeButton.Get(ModeButton.Type.Industry).SetNew(New: true);
			}
			TutorialPanelController.Instance.QuestCompleted(NewQuest);
			if (m_Data.m_AcademyData.GetInfoFromQuestID(NewQuest.m_ID) != null)
			{
				AnalyticsManager.CertificateComplete(NewQuest);
			}
			else if (m_Data.m_ResearchData.GetInfoFromQuestID(NewQuest.m_ID) != null)
			{
				AnalyticsManager.ResearchComplete(NewQuest);
			}
		}
		if (NewQuest.m_ID == Quest.ID.AcademyColonisation8)
		{
			OffworldMissionsManager.Instance.SetupDailyMission();
			BadgeManager.Instance.AddEvent(BadgeEvent.Type.GameComplete);
		}
		AddEvent(QuestEvent.Type.CompleteMission, Bot: false, NewQuest.m_ID, NewQuest.m_LastParentObject);
		if (NewQuest.m_Type != Quest.Type.Tutorial && NewQuest.m_Type != Quest.Type.Lesson)
		{
			AddEvent(QuestEvent.Type.CompleteAnyMission, Bot: false, 0, NewQuest.m_LastParentObject);
		}
		if (NewQuest.m_Type == Quest.Type.Research)
		{
			bool bot = false;
			if ((bool)NewQuest.m_LastParentObject && NewQuest.m_LastParentObject.m_TypeIdentifier == ObjectType.Worker)
			{
				bot = true;
			}
			AddEvent(QuestEvent.Type.CompleteResearch, bot, 0, NewQuest.m_LastParentObject);
		}
		RemoveActiveQuest(NewQuest);
		if (!m_CompletedQuests.ContainsKey(NewQuest.m_ID))
		{
			m_CompletedQuests.Add(NewQuest.m_ID, NewQuest);
		}
		DoUnlocked(NewQuest, ShowNew: true);
		foreach (Quest activeQuest in m_ActiveQuests)
		{
			_ = activeQuest;
			NewQuest.UpdateEventsCompletable();
		}
		TabQuests.Instance.UpdateLists();
		if (NewQuest.m_QuestsUnlocked.Count > 0)
		{
			foreach (Quest.ID item3 in NewQuest.m_QuestsUnlocked)
			{
				AddQuest(item3, DoCeremony);
			}
		}
		else
		{
			List<Quest> list2 = new List<Quest>();
			foreach (Quest activeQuest2 in m_ActiveQuests)
			{
				list2.Add(activeQuest2);
			}
			foreach (Quest item4 in list2)
			{
				if (item4 != null && item4.ContainsRequiredQuest(NewQuest.m_ID) && item4.GetIsComplete())
				{
					QuestCompleted(item4, DoCeremony);
				}
			}
		}
		if (NewQuest.m_ID == Quest.ID.GlueTranscendenceComplete || NewQuest.m_ID == Quest.ID.AcademyColonisation8)
		{
			TestLockedBuildings();
		}
	}

	public void LockObject(ObjectType NewType)
	{
		if (ObjectTypeList.Instance.GetIsBuilding(NewType))
		{
			if (!m_BuildingsLocked.ContainsKey(NewType))
			{
				m_BuildingsLocked.Add(NewType, 0);
			}
		}
		else if (!m_ObjectsLocked.ContainsKey(NewType))
		{
			m_ObjectsLocked.Add(NewType, 0);
		}
	}

	public void UnlockObject(ObjectType NewType)
	{
		if (ObjectTypeList.Instance.GetIsBuilding(NewType))
		{
			if (m_BuildingsLocked.ContainsKey(NewType))
			{
				m_BuildingsLocked.Remove(NewType);
				if (Wall.GetIsTypeWall(NewType))
				{
					AnimalStatusIndicator.m_NeedPenVisible = true;
				}
			}
		}
		else if (m_ObjectsLocked.ContainsKey(NewType))
		{
			m_ObjectsLocked.Remove(NewType);
		}
	}

	private void DoUnlocked(Quest NewQuest, bool ShowNew)
	{
		foreach (ObjectType item in NewQuest.m_BuildingsUnlocked)
		{
			UnlockObject(item);
			if (!ShowNew)
			{
				ModeButton.Get(ModeButton.Type.BuildingPalette).Show();
			}
		}
		foreach (ObjectType item2 in NewQuest.m_ObjectsUnlocked)
		{
			UnlockObject(item2);
		}
		_ = NewQuest.m_ID;
		_ = 68;
	}

	public Quest GetQuest(Quest.ID NewID)
	{
		foreach (Quest activeQuest in m_ActiveQuests)
		{
			if (activeQuest.m_ID == NewID)
			{
				return activeQuest;
			}
		}
		if (m_CompletedQuests.ContainsKey(NewID))
		{
			return m_CompletedQuests[NewID];
		}
		return null;
	}

	public Quest GetQuest(string Title)
	{
		foreach (Quest activeQuest in m_ActiveQuests)
		{
			if (activeQuest.m_Title == Title)
			{
				return activeQuest;
			}
		}
		foreach (KeyValuePair<Quest.ID, Quest> completedQuest in m_CompletedQuests)
		{
			if (completedQuest.Value.m_Title == Title)
			{
				return completedQuest.Value;
			}
		}
		return null;
	}

	public Quest GetEra(Quest.ID NewID)
	{
		return m_Data.m_QuestData[(int)NewID];
	}

	public bool GetQuestComplete(Quest.ID NewID)
	{
		if (GameOptionsManager.Instance.m_Options.m_GameMode != 0)
		{
			return true;
		}
		if (m_CompletedQuests.ContainsKey(NewID))
		{
			return true;
		}
		return false;
	}

	public bool GetIsObjectLocked(ObjectType NewType)
	{
		return m_ObjectsLocked.ContainsKey(NewType);
	}

	public bool GetIsBuildingLocked(ObjectType NewType)
	{
		return m_BuildingsLocked.ContainsKey(NewType);
	}

	public bool GetIsObjectLockedAny(ObjectType NewType)
	{
		if (m_ObjectsLocked.ContainsKey(NewType) || m_BuildingsLocked.ContainsKey(NewType))
		{
			return true;
		}
		return false;
	}

	public void AddEvent(QuestEvent.Type NewEvent, bool Bot, object ExtraData, BaseClass ParentObject, int Amount = 1, QuestEvent SpecificEvent = null)
	{
		if (m_ActiveEvents == null)
		{
			return;
		}
		List<Quest> list = m_ActiveEvents[(int)NewEvent];
		if (list.Count == 0)
		{
			return;
		}
		int num = 0;
		bool flag = false;
		List<Quest> list2 = new List<Quest>();
		foreach (Quest item in list)
		{
			if (((item.m_Type == Quest.Type.Tutorial || item.m_Type == Quest.Type.Lesson) && !item.m_Active) || (item.m_Type == Quest.Type.Tutorial && !item.m_Started) || (item.m_ID == Quest.ID.AcademyForestry && !item.m_Active))
			{
				continue;
			}
			item.GetIsStarted();
			if (item.m_Idea)
			{
				item.GetIdeaComplete();
			}
			int num2 = item.CheckEvent(NewEvent, Bot, ExtraData, ParentObject, Amount, SpecificEvent);
			if (num2 == 0)
			{
				continue;
			}
			if ((item.m_Active || item.m_MajorNode) && item.GetIsComplete())
			{
				if (!item.m_Active)
				{
					item.SetActive();
				}
				list2.Add(item);
			}
			num = 1;
			if (num2 == 2)
			{
				num = 2;
			}
			if (item.m_Type == Quest.Type.Tutorial || item.m_Type == Quest.Type.Lesson)
			{
				num += 2;
			}
			if (item.m_Complete || (item.GetDisplay() && !item.AreAnyEventsLocked()))
			{
				flag = true;
			}
		}
		bool flag2 = false;
		foreach (Quest item2 in list2)
		{
			item2.m_CompletedObject = ParentObject;
			QuestCompleted(item2);
			if (item2.m_QuestsUnlocked.Count != 0)
			{
				flag2 = true;
			}
		}
		if (flag2)
		{
			TabQuests.Instance.UpdateLists();
		}
		if (num != 0)
		{
			TabQuests.Instance.UpdateAll();
			HudManager.Instance.CheckQuestRolloverProgress(NewEvent, Bot, ExtraData);
			TutorialPanelController.Instance.UpdateEvent(NewEvent, Bot, ExtraData);
			if ((bool)ParentObject && flag)
			{
				AddXPEvent(ParentObject, num);
			}
		}
	}

	public void UnlockAll()
	{
		List<Quest> list = new List<Quest>();
		for (int i = 0; i < 155; i++)
		{
			Quest quest = QuestData.Instance.GetQuest((Quest.ID)i);
			if (quest != null && !m_CompletedQuests.ContainsKey((Quest.ID)i))
			{
				list.Add(quest);
				if (!quest.m_Active)
				{
					AddQuest(quest.m_ID, DoCeremony: false);
				}
			}
		}
	}

	public void CompleteAll()
	{
		UnlockAll();
		for (int i = 0; i < 155; i++)
		{
			Quest quest = QuestData.Instance.GetQuest((Quest.ID)i);
			if (quest != null)
			{
				quest.Complete();
				DoUnlocked(quest, ShowNew: true);
				if (!m_CompletedQuests.ContainsKey(quest.m_ID))
				{
					m_CompletedQuests.Add(quest.m_ID, quest);
				}
			}
		}
		m_ActiveQuests.Clear();
		for (int j = 0; j < 206; j++)
		{
			m_ActiveEvents[j].Clear();
		}
		TabQuests.Instance.UpdateLists();
	}

	public void CheatCompleteQuest(Quest NewQuest, bool DoCeremony = false)
	{
		if (!m_ActiveQuests.Contains(NewQuest))
		{
			AddQuest(NewQuest.m_ID, DoCeremony: false);
		}
		NewQuest.Complete();
		if (!m_CompletedQuests.ContainsKey(NewQuest.m_ID))
		{
			QuestCompleted(NewQuest, DoCeremony);
		}
	}

	public void UnlockObjects(ObjectType NewType)
	{
	}

	public void AddMove(Farmer NewFarmer)
	{
		bool bot = false;
		if (NewFarmer.m_TypeIdentifier == ObjectType.Worker)
		{
			bot = true;
		}
		if ((bool)NewFarmer.m_EngagedObject)
		{
			if (NewFarmer.m_EngagedObject.m_TypeIdentifier == ObjectType.Canoe)
			{
				AddEvent(QuestEvent.Type.MoveCanoe, bot, 0, NewFarmer);
			}
			else if (NewFarmer.m_EngagedObject.m_TypeIdentifier == ObjectType.Cart)
			{
				AddEvent(QuestEvent.Type.MoveCart, bot, 0, NewFarmer);
			}
			else if (NewFarmer.m_EngagedObject.m_TypeIdentifier == ObjectType.WheelBarrow)
			{
				AddEvent(QuestEvent.Type.MoveWheelbarrow, bot, 0, NewFarmer);
			}
		}
		else
		{
			if (NewFarmer.m_FarmerCarry.GetTopObject() != null || NewFarmer.m_FarmerInventory.m_TotalWeight != 0)
			{
				AddEvent(QuestEvent.Type.Carry, bot, 0, NewFarmer);
			}
			if (TileHelpers.GetTileWater(TileManager.Instance.GetTile(NewFarmer.m_TileCoord).m_TileType))
			{
				AddEvent(QuestEvent.Type.MoveWater, bot, 0, NewFarmer);
			}
			else
			{
				AddEvent(QuestEvent.Type.Move, bot, 0, NewFarmer);
			}
		}
	}

	public void AddPlayerMove(FarmerPlayer NewPlayer)
	{
		m_PlayerMove++;
		if (m_PlayerMove == 10)
		{
			m_PlayerMove = 0;
			AddMove(NewPlayer);
		}
	}

	public void AddWorkerMove(Worker NewWorker)
	{
		m_WorkerMove++;
		if (m_WorkerMove == 10)
		{
			m_WorkerMove = 0;
			AddMove(NewWorker);
		}
	}

	public bool GetCanResearch(ObjectType CurrentResearchType)
	{
		foreach (Quest activeQuest in m_ActiveQuests)
		{
			if (activeQuest.m_Type == Quest.Type.Research && activeQuest.m_ObjectTypeRequired == CurrentResearchType)
			{
				return true;
			}
		}
		return false;
	}

	public bool GetIsLastLevelActive()
	{
		if (Instance.GetQuestComplete(Quest.ID.GlueTranscendenceComplete) && !Instance.GetQuestComplete(Quest.ID.AcademyColonisation8))
		{
			return true;
		}
		return false;
	}

	public bool DoResearch(Holdable CurrentResearchObject, Quest.ID ResearchQuest, BaseClass ParentObject, int Value)
	{
		Quest quest = GetQuest(ResearchQuest);
		if (quest != null)
		{
			int num = quest.CheckEvent(QuestEvent.Type.Research, Bot: false, null, ParentObject, Value);
			if (num > 0)
			{
				TabQuests.Instance.UpdateLists();
				if ((bool)ParentObject)
				{
					AddXPEvent(ParentObject, num);
				}
				if (quest.GetIsComplete())
				{
					QuestCompleted(quest);
					return true;
				}
			}
		}
		return false;
	}

	private void AddXPEvent(BaseClass ParentObject, int EventUsed)
	{
		if (!m_XPEffects.ContainsKey(ParentObject))
		{
			m_XPEffects.Add(ParentObject, EventUsed);
		}
		else if (m_XPEffects[ParentObject] < EventUsed)
		{
			m_XPEffects[ParentObject] = EventUsed;
		}
	}

	private void UpdatePlus1Effects()
	{
		foreach (KeyValuePair<BaseClass, int> xPEffect in m_XPEffects)
		{
			BaseClass key = xPEffect.Key;
			int value = xPEffect.Value;
			XPPlus1 component = ObjectTypeList.Instance.CreateObjectFromIdentifier(ObjectType.XPPlus1, base.transform.position, Quaternion.identity).GetComponent<XPPlus1>();
			switch (value)
			{
			case 4:
				AudioManager.Instance.StartEvent("CeremonyIdeaComplete");
				component.SetComplete(Complete: true, Idea: true);
				break;
			case 3:
				AudioManager.Instance.StartEvent("CeremonyIdea");
				component.SetComplete(Complete: false, Idea: true);
				break;
			case 2:
				AudioManager.Instance.StartEvent("CeremonyPlus1Complete");
				component.SetComplete(Complete: true, Idea: false);
				break;
			default:
				AudioManager.Instance.StartEvent("CeremonyPlus1", key.GetComponent<TileCoordObject>());
				component.SetComplete(Complete: false, Idea: false);
				break;
			}
			component.SetScale(4f);
			component.SetWorldPosition(key.transform.position);
		}
		m_XPEffects.Clear();
	}

	public void UpdateColonisation8Requirement()
	{
		Quest quest = GetQuest(Quest.ID.AcademyColonisation8);
		if (quest == null || quest.m_Complete)
		{
			return;
		}
		foreach (QuestEvent item in quest.m_EventsRequired)
		{
			if (item.m_Type == QuestEvent.Type.FolkTranscended)
			{
				item.m_Required = FolkManager.Instance.GetTotalFolk();
				break;
			}
		}
	}

	private void Update()
	{
		if (!m_Started)
		{
			m_Started = true;
			if (m_ActiveQuests.Count == 0 && m_CompletedQuests.Count == 0)
			{
				Reset();
			}
		}
		UpdatePlus1Effects();
		UpdateColonisation8Requirement();
	}

	public void Reset()
	{
		m_ActiveQuests = new List<Quest>();
		m_CompletedQuests = new Dictionary<Quest.ID, Quest>();
		m_BuildingsLocked = new Dictionary<ObjectType, int>();
		m_ObjectsLocked = new Dictionary<ObjectType, int>();
		Quest[] questData = m_Data.m_QuestData;
		for (int i = 0; i < questData.Length; i++)
		{
			questData[i]?.Reset();
		}
		LockQuests();
		for (int j = 0; j < 155; j++)
		{
			Quest quest = QuestData.Instance.GetQuest((Quest.ID)j);
			if (quest != null)
			{
				if (GameOptionsManager.Instance.m_Options.m_GameMode == GameOptions.GameMode.ModeCampaign)
				{
					m_ActiveQuests.Add(quest);
				}
				else if (quest.m_Type == Quest.Type.Tutorial)
				{
					m_ActiveQuests.Add(quest);
				}
			}
		}
		CreateActiveEventsArray();
		QuestManagerTiles.Instance.Init();
		if (GameOptionsManager.Instance.m_Options.m_GameMode == GameOptions.GameMode.ModeCampaign)
		{
			TabQuests.Instance.UpdateLists();
		}
		TutorialPanelController.Instance.SetupFirstTutorial(Quest.ID.TutorialStart);
		if (!GameOptionsManager.Instance.m_Options.m_TutorialEnabled)
		{
			TutorialPanelController.Instance.EndTutorial(Complete: false);
			TutorialFinished();
			if (!SessionManager.Instance.m_LoadLevel && GameOptionsManager.Instance.m_Options.m_GameMode == GameOptions.GameMode.ModeCampaign)
			{
				CheatCompleteQuest(GetQuest(Quest.ID.GlueForestry));
				CheatCompleteQuest(GetQuest(Quest.ID.GlueFirstIndustries));
				CheatCompleteQuest(GetQuest(Quest.ID.GlueFinal));
			}
		}
	}
}
