using System.Collections.Generic;

public class TabQuestBase : Tab
{
	protected List<QuestPanel> m_ActiveQuests;

	protected List<QuestPanel> m_PanelCache;

	public bool m_SomeIngredientsMissing;

	protected new void Awake()
	{
		base.Awake();
		m_ActiveQuests = new List<QuestPanel>();
		m_PanelCache = new List<QuestPanel>();
	}

	private bool DoesMissionUnlockObject(Quest NewQuest, ObjectType NewType)
	{
		if (NewQuest.m_ObjectsUnlocked.Contains(NewType))
		{
			return true;
		}
		foreach (Quest.ID item in NewQuest.m_QuestsUnlocked)
		{
			Quest quest = QuestData.Instance.GetQuest(item);
			if (DoesMissionUnlockObject(quest, NewType))
			{
				return true;
			}
		}
		return false;
	}

	private QuestPanel GetMissionFromMissingIngredient(ObjectType NewType)
	{
		foreach (QuestPanel activeQuest in m_ActiveQuests)
		{
			if (DoesMissionUnlockObject(activeQuest.m_Quest, NewType))
			{
				return activeQuest;
			}
		}
		return null;
	}

	public void SetMissingIngredients(List<ObjectType> Missing)
	{
		foreach (ObjectType item in Missing)
		{
			QuestPanel missionFromMissingIngredient = GetMissionFromMissingIngredient(item);
			if (missionFromMissingIngredient != null)
			{
				missionFromMissingIngredient.SetMissing(Missing: true);
				SetMissing(Missing: true);
			}
		}
	}

	public void ClearMissingIngredients()
	{
		foreach (QuestPanel activeQuest in m_ActiveQuests)
		{
			activeQuest.SetMissing(Missing: false);
		}
		SetMissing(Missing: false);
	}

	private void SetMissing(bool Missing)
	{
		m_SomeIngredientsMissing = Missing;
	}
}
