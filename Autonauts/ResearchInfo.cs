using System.Collections.Generic;

public class ResearchInfo
{
	public Quest.ID m_ID;

	public int m_Level;

	public int m_HeartsRequired;

	public List<ObjectType> m_RequiredObjects;

	public List<ObjectType> m_UnlockedObjects;

	public ResearchInfo(Quest.ID NewID, int HeartsRequired, int Level)
	{
		m_ID = NewID;
		m_HeartsRequired = HeartsRequired;
		m_Level = Level;
		m_RequiredObjects = new List<ObjectType>();
		m_UnlockedObjects = new List<ObjectType>();
	}

	public void AddRequiredObject(ObjectType NewType)
	{
		m_RequiredObjects.Add(NewType);
	}

	public void AddUnlockedObject(ObjectType NewType)
	{
		m_UnlockedObjects.Add(NewType);
	}
}
