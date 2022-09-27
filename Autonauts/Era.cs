using System.Collections.Generic;

public class Era
{
	public enum ID
	{
		First,
		Second,
		Third,
		Total
	}

	public ID m_ID;

	public string m_Title;

	public List<Quest.ID> m_RequiredQuests;

	public static ID GetIDFromString(string Name)
	{
		for (int i = 0; i < 3; i++)
		{
			ID result = (ID)i;
			string text = result.ToString();
			if (Name == text)
			{
				return result;
			}
		}
		return ID.Total;
	}

	public Era()
	{
		m_RequiredQuests = new List<Quest.ID>();
	}

	public void SetInfo(ID NewID)
	{
		m_ID = NewID;
		m_Title = "Era" + NewID;
	}

	public void AddRequiredQuest(Quest.ID NewID)
	{
		m_RequiredQuests.Add(NewID);
	}

	public void SetActive()
	{
	}

	private bool GetRequiredQuestsComplete()
	{
		foreach (Quest.ID requiredQuest in m_RequiredQuests)
		{
			if (!QuestManager.Instance.m_Data.GetQuest(requiredQuest).GetIsComplete())
			{
				return false;
			}
		}
		return true;
	}

	public bool CheckCompleted()
	{
		if (!GetRequiredQuestsComplete())
		{
			return false;
		}
		return true;
	}
}
