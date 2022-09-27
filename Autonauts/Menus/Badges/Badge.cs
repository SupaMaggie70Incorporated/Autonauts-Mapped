using System.Collections.Generic;
using UnityEngine;

public class Badge
{
	public enum ID
	{
		Tutorial,
		Berries,
		Mushrooms,
		Milk,
		Wool,
		Eggs,
		Fish,
		Tools,
		Honey,
		Colonists,
		Mining,
		Pottery,
		Clothes,
		Plots,
		Trees,
		Food,
		Cereal,
		MobileStorage,
		Stored,
		Bots,
		Complete,
		Total
	}

	public ID m_ID;

	public List<BadgeEventRequired> m_EventsRequired;

	public List<ObjectType> m_ObjectsUnlocked;

	public List<ObjectType> m_BuildingsUnlocked;

	public bool m_Complete;

	public Badge(ID NewID, BadgeEvent.Type NewEvent, int EventCount)
	{
		m_ID = NewID;
		m_EventsRequired = new List<BadgeEventRequired>();
		AddEvent(NewEvent, EventCount);
		m_ObjectsUnlocked = new List<ObjectType>();
		m_BuildingsUnlocked = new List<ObjectType>();
		SetNotCompleted();
	}

	public string GetSaveName()
	{
		return "Badge" + m_ID;
	}

	public void Save()
	{
		string saveName = GetSaveName();
		if (m_Complete)
		{
			PlayerPrefs.SetInt(saveName, 1);
		}
		else
		{
			PlayerPrefs.SetInt(saveName, 0);
		}
	}

	public void Load()
	{
		string saveName = GetSaveName();
		if (PlayerPrefs.HasKey(saveName))
		{
			if (PlayerPrefs.GetInt(saveName) == 1)
			{
				SetCompleted();
			}
			else
			{
				SetNotCompleted();
			}
		}
	}

	public void Clear()
	{
		SetNotCompleted();
		Save();
	}

	public int GetEventRequiredCount(BadgeEvent.Type NewEvent)
	{
		foreach (BadgeEventRequired item in m_EventsRequired)
		{
			if (item.m_Event == NewEvent)
			{
				return item.m_Count;
			}
		}
		return 0;
	}

	public void AddEvent(BadgeEvent.Type NewEvent, int EventCount)
	{
		BadgeEventRequired item = new BadgeEventRequired(NewEvent, EventCount);
		m_EventsRequired.Add(item);
	}

	public void AddObjectUnlocked(ObjectType NewType)
	{
		m_ObjectsUnlocked.Add(NewType);
	}

	public void AddBuildingUnlocked(ObjectType NewType)
	{
		m_BuildingsUnlocked.Add(NewType);
	}

	private void SetCompleted()
	{
		m_Complete = true;
		if (!GameOptionsManager.Instance.m_Options.m_BadgeUnlocksEnabled || !QuestManager.Instance)
		{
			return;
		}
		foreach (ObjectType item in m_ObjectsUnlocked)
		{
			QuestManager.Instance.UnlockObject(item);
		}
		foreach (ObjectType item2 in m_BuildingsUnlocked)
		{
			QuestManager.Instance.UnlockObject(item2);
		}
	}

	private void SetNotCompleted()
	{
		m_Complete = false;
		if (!QuestManager.Instance)
		{
			return;
		}
		foreach (ObjectType item in m_ObjectsUnlocked)
		{
			QuestManager.Instance.LockObject(item);
		}
		foreach (ObjectType item2 in m_BuildingsUnlocked)
		{
			QuestManager.Instance.LockObject(item2);
		}
	}

	public bool GetIsComplete()
	{
		if (m_Complete)
		{
			return true;
		}
		foreach (BadgeEventRequired item in m_EventsRequired)
		{
			if (BadgeManager.Instance.GetEventCount(item.m_Event) < item.m_Count)
			{
				return false;
			}
		}
		SetCompleted();
		return true;
	}

	public void ForceComplete(bool Complete)
	{
		if (Complete)
		{
			SetCompleted();
		}
		else
		{
			SetNotCompleted();
		}
	}

	public float GetCompletePercent()
	{
		if (m_Complete)
		{
			return 1f;
		}
		float num = 0f;
		foreach (BadgeEventRequired item in m_EventsRequired)
		{
			BadgeEvent @event = BadgeManager.Instance.GetEvent(item.m_Event);
			num += (float)@event.m_Count / (float)item.m_Count;
		}
		return num / (float)m_EventsRequired.Count;
	}

	public bool CheckStoredCompleted(int Stored)
	{
		if (m_Complete)
		{
			return false;
		}
		if (Stored >= m_EventsRequired[0].m_Count)
		{
			SetCompleted();
			return true;
		}
		return false;
	}
}
