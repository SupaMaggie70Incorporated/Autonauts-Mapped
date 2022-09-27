using System.Collections.Generic;
using UnityEngine;

public class Industry
{
	public string m_Name;

	public List<IndustrySub> m_SubIndustries;

	public Color m_PanelColour;

	public Industry(string Name, Color PanelColour)
	{
		m_Name = Name;
		m_PanelColour = PanelColour;
		m_SubIndustries = new List<IndustrySub>();
	}

	public IndustrySub AddSub(string Name)
	{
		IndustrySub industrySub = new IndustrySub(m_Name + Name, this);
		m_SubIndustries.Add(industrySub);
		return industrySub;
	}

	public void ConvertToQuests()
	{
		foreach (IndustrySub subIndustry in m_SubIndustries)
		{
			subIndustry.ConvertToQuests();
		}
	}

	public string GetIcon()
	{
		return "Industries/" + m_Name;
	}

	public bool GetAnyLevelsUnlocked()
	{
		foreach (IndustrySub subIndustry in m_SubIndustries)
		{
			if (subIndustry.GetAnyLevelsUnlocked())
			{
				return true;
			}
		}
		return false;
	}

	public bool GetContainsQuestID(Quest.ID NewID)
	{
		foreach (IndustrySub subIndustry in m_SubIndustries)
		{
			if (subIndustry.GetContainsQuestID(NewID))
			{
				return true;
			}
		}
		return false;
	}
}
