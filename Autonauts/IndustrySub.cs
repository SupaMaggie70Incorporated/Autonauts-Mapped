using System.Collections.Generic;

public class IndustrySub
{
	public string m_Name;

	public List<IndustryLevel> m_IndustryLevels;

	public Industry m_Parent;

	public IndustrySub(string Name, Industry Parent)
	{
		m_Parent = Parent;
		m_Name = Name;
		m_IndustryLevels = new List<IndustryLevel>();
	}

	public IndustryLevel AddLevel(Quest.ID NewID, Quest.Type Type = Quest.Type.Industry, bool MajorNode = false, bool ShowUnlockedQuests = true)
	{
		IndustryLevel industryLevel = new IndustryLevel(NewID, this, Type, MajorNode, ShowUnlockedQuests);
		m_IndustryLevels.Add(industryLevel);
		return industryLevel;
	}

	public void ConvertToQuests()
	{
		int num = 0;
		foreach (IndustryLevel industryLevel in m_IndustryLevels)
		{
			industryLevel.ConvertToQuest(num);
			num++;
		}
	}

	public string GetIcon()
	{
		string text = m_Parent.m_Name.Remove(0, "Industry".Length);
		return "Industries/" + text + "/" + m_Name;
	}

	public bool GetAnyLevelsUnlocked()
	{
		foreach (IndustryLevel industryLevel in m_IndustryLevels)
		{
			if (industryLevel.m_Quest.GetState() != 0)
			{
				return true;
			}
		}
		return false;
	}

	public bool GetContainsQuestID(Quest.ID NewID)
	{
		foreach (IndustryLevel industryLevel in m_IndustryLevels)
		{
			if (industryLevel.m_Quest.m_ID == NewID)
			{
				return true;
			}
		}
		return false;
	}

	public bool GetAllQuestsCompleted()
	{
		foreach (IndustryLevel industryLevel in m_IndustryLevels)
		{
			if (!industryLevel.m_Quest.m_Complete)
			{
				return false;
			}
		}
		return true;
	}
}
