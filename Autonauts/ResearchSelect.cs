using System.Collections.Generic;
using UnityEngine;

public class ResearchSelect : ConverterSelect
{
	private ResearchStation m_Research;

	private List<Quest.ID> m_Quests;

	private bool IsQuestSelectable(Quest NewQuest)
	{
		if (NewQuest != null && NewQuest.m_Started && !NewQuest.GetIsComplete() && NewQuest.m_Active)
		{
			return true;
		}
		return false;
	}

	private static int SortQuestsByCost(Quest.ID p1, Quest.ID p2)
	{
		Quest quest = QuestManager.Instance.GetQuest(p1);
		Quest quest2 = QuestManager.Instance.GetQuest(p2);
		int required = quest.m_EventsRequired[0].m_Required;
		int required2 = quest2.m_EventsRequired[0].m_Required;
		return required - required2;
	}

	public override void SetBuilding(Building NewBuilding)
	{
		BaseSetBuilding(NewBuilding);
		m_Research = NewBuilding.GetComponent<ResearchStation>();
		m_Quests = new List<Quest.ID>();
		foreach (Quest.ID quest3 in m_Research.m_Quests)
		{
			Quest quest = QuestManager.Instance.GetQuest(quest3);
			if (IsQuestSelectable(quest) && !CeremonyManager.Instance.IsQuestTypeInCeremonyQueue(quest3) && !m_Quests.Contains(quest.m_ID))
			{
				m_Quests.Add(quest.m_ID);
			}
		}
		m_Quests.Sort(SortQuestsByCost);
		int currentlySelected = m_Quests.IndexOf(m_Research.m_CurrentResearchQuest);
		List<ObjectType> list = new List<ObjectType>();
		List<bool> list2 = new List<bool>();
		foreach (Quest.ID quest4 in m_Quests)
		{
			Quest quest2 = QuestManager.Instance.GetQuest(quest4);
			list.Add(quest2.m_ObjectTypeRequired);
			list2.Add(NewIconManager.Instance.IsResearchNew(quest4));
		}
		SetupBlueprintButtons(list, list2, currentlySelected);
		NewIconManager.Instance.ResearchSeen(m_Research);
		ObjectType buildingToUpgradeTo = m_Research.m_BuildingToUpgradeTo;
		if (buildingToUpgradeTo != ObjectTypeList.m_Total && !QuestManager.Instance.GetIsBuildingLocked(buildingToUpgradeTo))
		{
			m_UpgradeButton.SetActive(Active: true);
			NewIconManager.Instance.IsObjectNew(buildingToUpgradeTo);
		}
		else
		{
			m_UpgradeButton.SetActive(Active: false);
		}
	}

	public override void OnCreateBlueprintButton(GameObject NewGadget, int Index)
	{
		base.OnCreateBlueprintButton(NewGadget, Index);
		ResearchSelectButton component = NewGadget.GetComponent<ResearchSelectButton>();
		if (Index < m_ObjectTypes.Count)
		{
			component.SetQuest(m_Quests[Index]);
		}
	}

	public override void OnBlueprintClicked(BaseGadget NewGadget)
	{
		if (!(m_Research.m_Engager == null))
		{
			ResearchSelectButton component = NewGadget.GetComponent<ResearchSelectButton>();
			m_Research.SetCurrentResearchQuest(component.m_Quest.m_ID);
			m_Research.m_Engager.SendAction(new ActionInfo(ActionType.DisengageObject, m_Research.m_Engager.GetComponent<TileCoordObject>().m_TileCoord, m_Research));
			GameStateManager.Instance.PopState();
		}
	}

	public override void OnUpgradeClicked(BaseGadget NewGadget)
	{
		m_Research.m_Engager.SendAction(new ActionInfo(ActionType.DisengageObject, m_Research.m_Engager.GetComponent<TileCoordObject>().m_TileCoord, m_Research));
		m_Research.BeginUpgrade();
		GameStateManager.Instance.PopState();
	}
}
