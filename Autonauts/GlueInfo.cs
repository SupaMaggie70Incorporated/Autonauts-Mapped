using System.Collections.Generic;

public class GlueInfo
{
	public Quest.ID m_ID;

	public CeremonyManager.CeremonyType m_CeremonyType;

	public Quest.Type m_QuestType;

	public List<Quest.ID> m_RequiredQuests;

	public List<Quest.ID> m_UnlockedQuests;

	public List<ObjectType> m_UnlockedObjects;

	public List<QuestEvent> m_Events;

	public GlueInfo(Quest.ID NewID, CeremonyManager.CeremonyType NewCeremonyType, Quest.Type NewQuestType)
	{
		m_ID = NewID;
		m_CeremonyType = NewCeremonyType;
		m_QuestType = NewQuestType;
		m_RequiredQuests = new List<Quest.ID>();
		m_UnlockedQuests = new List<Quest.ID>();
		m_UnlockedObjects = new List<ObjectType>();
		m_Events = new List<QuestEvent>();
	}

	public void AddEvent(QuestEvent.Type NewType, bool BotOnly, object ExtraData, int Required, string Description = "")
	{
		QuestEvent item = new QuestEvent(NewType, BotOnly, ExtraData, Required, Description);
		m_Events.Add(item);
	}

	public void AddRequiredQuest(Quest.ID NewID)
	{
		m_RequiredQuests.Add(NewID);
	}

	public void AddUnlockedQuest(Quest.ID NewID)
	{
		m_UnlockedQuests.Add(NewID);
	}

	public void AddUnlockedObject(ObjectType NewType)
	{
		m_UnlockedObjects.Add(NewType);
	}
}
