using System.Collections.Generic;

public class TutorialInfo
{
	public Quest.ID m_ID;

	public List<QuestEvent> m_Events;

	public List<TutorialPointerManager.Type> m_EventPointers;

	public List<ObjectType> m_UnlockedObjects;

	public List<Quest.ID> m_UnlockedQuests;

	public TutorialData.Lesson m_LessonNumber;

	public TutorialInfo(Quest.ID NewID, TutorialData.Lesson NewLessonNumber)
	{
		m_ID = NewID;
		m_LessonNumber = NewLessonNumber;
		m_Events = new List<QuestEvent>();
		m_EventPointers = new List<TutorialPointerManager.Type>();
		m_UnlockedObjects = new List<ObjectType>();
		m_UnlockedQuests = new List<Quest.ID>();
	}

	public void AddEvent(QuestEvent.Type NewType, bool BotOnly, object ExtraData, int Required, string Description = "", TutorialPointerManager.Type PointerType = TutorialPointerManager.Type.Total)
	{
		QuestEvent item = new QuestEvent(NewType, BotOnly, ExtraData, Required, Description);
		m_Events.Add(item);
		m_EventPointers.Add(PointerType);
	}

	public void AddUnlockedObject(ObjectType NewType)
	{
		m_UnlockedObjects.Add(NewType);
	}

	public void AddUnlockedQuest(Quest.ID NewID)
	{
		m_UnlockedQuests.Add(NewID);
	}
}
