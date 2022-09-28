using System.Collections.Generic;
using UnityEngine;

public class CertificateInfo
{
	public Quest.ID m_ID;

	public List<Quest.ID> m_ResearchIDs;

	public List<QuestEvent> m_Events;

	public List<ObjectType> m_UnlockedObjects;

	public List<Quest.ID> m_UnlockedQuests;

	public Color m_Colour;

	public bool m_Tutorial;

	public Quest.ID m_LessonID;

	public CertificateInfo(Quest.ID NewID, Color NewColour)
	{
		m_ID = NewID;
		m_Colour = NewColour;
		m_Tutorial = false;
		m_Events = new List<QuestEvent>();
		m_UnlockedObjects = new List<ObjectType>();
		m_UnlockedQuests = new List<Quest.ID>();
		m_ResearchIDs = new List<Quest.ID>();
		m_LessonID = Quest.ID.Total;
	}

	public void AddEvent(QuestEvent.Type NewType, bool BotOnly, object ExtraData, int Required, string Description = "")
	{
		QuestEvent item = new QuestEvent(NewType, BotOnly, ExtraData, Required, Description);
		m_Events.Add(item);
	}

	public void AddUnlockedObject(ObjectType NewType)
	{
		m_UnlockedObjects.Add(NewType);
	}

	public void AddResearchID(Quest.ID NewID)
	{
		m_ResearchIDs.Add(NewID);
	}

	public void AddUnlockedQuest(Quest.ID NewID)
	{
		m_UnlockedQuests.Add(NewID);
	}
}
