using System.Collections.Generic;

public class ResearchLevelInfo
{
	public int m_Level;

	public Quest.ID m_ID;

	public int m_ResearchRequired;

	public List<ResearchInfo> m_ResearchInfos;

	public List<ObjectType> m_UnlockedObjects;

	public ResearchLevelInfo(int Level, Quest.ID NewID, int Required)
	{
		m_Level = Level;
		m_ID = NewID;
		m_ResearchRequired = Required;
		m_ResearchInfos = new List<ResearchInfo>();
		m_UnlockedObjects = new List<ObjectType>();
	}

	public ResearchInfo AddResearch(Quest.ID NewID, int HeartsRequired)
	{
		ResearchInfo researchInfo = new ResearchInfo(NewID, HeartsRequired, m_Level);
		m_ResearchInfos.Add(researchInfo);
		return researchInfo;
	}

	public void AddUnlockedObject(ObjectType NewType)
	{
		m_UnlockedObjects.Add(NewType);
	}
}
