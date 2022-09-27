using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;

public class IndustryLevel
{
	public int m_Level;

	public Quest.ID m_ID;

	public Quest m_Quest;

	public Quest.Type m_Type;

	public bool m_MajorNode;

	public ObjectType m_ResearchObjectType;

	public Quest.ResearchType m_ResearchType;

	public List<IndustryLevelEvent> m_Events;

	public List<ObjectType> m_UnlockedObjects;

	public List<Quest.ID> m_UnlockedQuests;

	public bool m_ShowUnlockedQuests;

	public IndustrySub m_Parent;

	public IndustryLevel(Quest.ID NewID, IndustrySub Parent, Quest.Type Type, bool MajorNode, bool ShowUnlockedQuests)
	{
		m_Parent = Parent;
		m_ID = NewID;
		m_Type = Type;
		m_MajorNode = MajorNode;
		m_ShowUnlockedQuests = ShowUnlockedQuests;
		Clear();
	}

	public void Save(JSONNode NewNode)
	{
		JSONUtils.Set(NewNode, "Name", m_ID.ToString());
		JSONArray jSONArray = (JSONArray)(NewNode["Events"] = new JSONArray());
		int num = 0;
		foreach (IndustryLevelEvent @event in m_Events)
		{
			JSONNode jSONNode2 = new JSONObject();
			QuestEvent questEvent = new QuestEvent(@event.m_Type, @event.m_BotOnly, @event.m_ExtraData, @event.m_Count);
			JSONUtils.Set(jSONNode2, "T", QuestEvent.GetNameFromType(questEvent.m_Type));
			if (questEvent.m_BotOnly)
			{
				JSONUtils.Set(jSONNode2, "B", 1);
			}
			else
			{
				JSONUtils.Set(jSONNode2, "B", 0);
			}
			JSONUtils.Set(jSONNode2, "E", questEvent.GetExtraDataAsString());
			JSONUtils.Set(jSONNode2, "C", @event.m_Count);
			jSONArray[num] = jSONNode2;
			num++;
		}
		jSONArray = (JSONArray)(NewNode["Objects"] = new JSONArray());
		num = 0;
		foreach (ObjectType unlockedObject in m_UnlockedObjects)
		{
			jSONArray[num] = ObjectTypeList.Instance.GetSaveNameFromIdentifier(unlockedObject);
			num++;
		}
		jSONArray = (JSONArray)(NewNode["Missions"] = new JSONArray());
		num = 0;
		foreach (Quest.ID unlockedQuest in m_UnlockedQuests)
		{
			jSONArray[num] = QuestData.Instance.GetQuestNameFromID(unlockedQuest);
			num++;
		}
	}

	public void Load(JSONNode NewNode)
	{
		JSONUtils.GetAsString(NewNode, "Name", "");
		m_Events.Clear();
		m_UnlockedObjects.Clear();
		m_UnlockedQuests.Clear();
		JSONArray asArray = NewNode["Events"].AsArray;
		for (int i = 0; i < asArray.Count; i++)
		{
			JSONObject asObject = asArray[i].AsObject;
			string asString = JSONUtils.GetAsString(asObject, "T", "");
			QuestEvent.Type typeFromName = QuestEvent.GetTypeFromName(asString);
			if (typeFromName == QuestEvent.Type.Total)
			{
				Debug.Log("Event " + asString + " not found");
			}
			bool botOnly = false;
			if (JSONUtils.GetAsInt(asObject, "B", 0) != 0)
			{
				botOnly = true;
			}
			string asString2 = JSONUtils.GetAsString(asObject, "E", "");
			QuestEvent questEvent = new QuestEvent(typeFromName, BotOnly: false, 0, 0);
			questEvent.SetExtraDataFromString(asString2);
			int asInt = JSONUtils.GetAsInt(asObject, "C", 0);
			AddRequirement(typeFromName, botOnly, questEvent.m_ExtraData, asInt);
		}
		asArray = NewNode["Objects"].AsArray;
		for (int j = 0; j < asArray.Count; j++)
		{
			string saveName = asArray[j];
			ObjectType identifierFromSaveName = ObjectTypeList.Instance.GetIdentifierFromSaveName(saveName);
			if (identifierFromSaveName != ObjectTypeList.m_Total)
			{
				AddUnlocked(identifierFromSaveName);
			}
		}
		asArray = NewNode["Missions"].AsArray;
		for (int k = 0; k < asArray.Count; k++)
		{
			string name = asArray[k];
			Quest.ID questIDFromName = QuestData.Instance.GetQuestIDFromName(name);
			if (questIDFromName != Quest.ID.Total)
			{
				AddMissionUnlocked(questIDFromName);
			}
		}
	}

	public void Clear()
	{
		m_Events = new List<IndustryLevelEvent>();
		m_UnlockedObjects = new List<ObjectType>();
		m_UnlockedQuests = new List<Quest.ID>();
	}

	public void AddRequirement(QuestEvent.Type NewType, bool BotOnly, object ExtraData, int NewCount)
	{
		m_Events.Add(new IndustryLevelEvent(NewType, BotOnly, ExtraData, NewCount));
	}

	public void AddUnlocked(ObjectType NewType)
	{
		m_UnlockedObjects.Add(NewType);
	}

	public void AddMissionUnlocked(Quest.ID NewID)
	{
		m_UnlockedQuests.Add(NewID);
	}

	public void ConvertToQuest(int Index)
	{
		m_Level = Index;
		Quest quest = new Quest();
		quest.m_Simple = false;
		quest.m_MajorNode = m_MajorNode;
		quest.m_ShowUnlockedQuests = m_ShowUnlockedQuests;
		foreach (IndustryLevelEvent @event in m_Events)
		{
			quest.AddEvent(@event.m_Type, @event.m_BotOnly, @event.m_ExtraData, @event.m_Count);
		}
		foreach (ObjectType unlockedObject in m_UnlockedObjects)
		{
			if (!ObjectTypeList.Instance.GetIsBuilding(unlockedObject))
			{
				quest.AddObjectUnlocked(unlockedObject);
			}
			else
			{
				quest.AddBuildingUnlocked(unlockedObject);
			}
		}
		foreach (Quest.ID unlockedQuest in m_UnlockedQuests)
		{
			quest.AddQuestUnlocked(unlockedQuest);
		}
		CeremonyManager.CeremonyType newCeremonyType = CeremonyManager.CeremonyType.QuestEnded;
		if (m_Type == Quest.Type.Tutorial)
		{
			newCeremonyType = CeremonyManager.CeremonyType.Total;
		}
		else if (m_MajorNode)
		{
			newCeremonyType = CeremonyManager.CeremonyType.MainQuestEnded;
		}
		else if (m_Type == Quest.Type.Research)
		{
			newCeremonyType = CeremonyManager.CeremonyType.ResearchEnded;
		}
		else if (m_UnlockedObjects.Count > 0 && Wonder.GetIsTypeWonder(m_UnlockedObjects[0]))
		{
			newCeremonyType = CeremonyManager.CeremonyType.EraComplete;
		}
		string text = m_ID.ToString();
		quest.SetInfo(m_ID, Quest.Category.Total, text, text, text + "Desc", null, null, newCeremonyType, m_Type);
		quest.m_ObjectTypeRequired = m_ResearchObjectType;
		quest.m_ResearchType = m_ResearchType;
		quest.m_Importantance = m_Level;
		m_Quest = quest;
		QuestData.Instance.AddQuest(m_ID, quest);
	}

	public string GetIcon()
	{
		return m_Parent.GetIcon();
	}
}
