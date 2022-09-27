using System.Collections.Generic;
using UnityEngine;

public class Research : AutopediaPage
{
	public static Research Instance;

	private BaseScrollView m_ScrollView;

	private ResearchLevel m_ResearchLevelPrefab;

	private ResearchTechnology m_ResearchTechnologyPrefab;

	private GameObject m_SelectedPanel;

	private SelectedTechnology m_SelectedTechnology;

	private List<ResearchLevel> m_Levels;

	private Quest.ID m_SelectedResearch;

	private void Awake()
	{
		Instance = this;
		m_ScrollView = base.transform.Find("BaseScrollView").GetComponent<BaseScrollView>();
		m_ResearchLevelPrefab = m_ScrollView.GetContent().transform.Find("ResearchLevel").GetComponent<ResearchLevel>();
		m_ResearchLevelPrefab.gameObject.SetActive(value: false);
		m_SelectedPanel = base.transform.Find("SelectedPanel").gameObject;
		m_SelectedPanel.SetActive(value: false);
		m_SelectedTechnology = m_SelectedPanel.transform.Find("SelectedTechnology").GetComponent<SelectedTechnology>();
		m_SelectedResearch = Quest.ID.Total;
		CreateLevels();
	}

	public void TechnologySelected(ResearchTechnology NewTechnology)
	{
		if ((bool)NewTechnology)
		{
			m_SelectedPanel.SetActive(value: true);
			m_SelectedTechnology.SetQuest(NewTechnology.m_Quest.m_ID);
		}
		else
		{
			m_SelectedPanel.SetActive(value: false);
		}
	}

	private void CreateLevels()
	{
		m_Levels = new List<ResearchLevel>();
		int highestLevel = QuestData.Instance.m_ResearchData.GetHighestLevel();
		float num = -120f;
		for (int i = 0; i <= highestLevel; i++)
		{
			Transform parent = m_ScrollView.GetContent().transform;
			ResearchLevel researchLevel = Object.Instantiate(m_ResearchLevelPrefab, parent);
			researchLevel.SetActive(Active: true);
			researchLevel.SetLevel(i);
			researchLevel.SetPosition(new Vector2(0f, num));
			num -= researchLevel.GetHeight();
			m_Levels.Add(researchLevel);
		}
		float scrollSize = 0f - num;
		m_ScrollView.SetScrollSize(scrollSize);
	}

	public ResearchLevel GetLevelFromQuest(Quest NewQuest)
	{
		if (NewQuest == null)
		{
			return m_Levels[0];
		}
		foreach (ResearchLevel level in m_Levels)
		{
			if (level.m_LevelInfo.m_ID == NewQuest.m_ID)
			{
				return level;
			}
		}
		return null;
	}

	public void ScrollToLevel(Quest NewQuest)
	{
		ResearchLevel levelFromQuest = GetLevelFromQuest(NewQuest);
		if ((bool)levelFromQuest)
		{
			float num = 0f - levelFromQuest.transform.localPosition.y + m_ResearchLevelPrefab.transform.localPosition.y;
			float num2 = m_ScrollView.GetScrollSize() - m_ScrollView.GetHeight();
			float scrollValue = 1f - num / num2;
			m_ScrollView.SetScrollValue(scrollValue);
		}
		else
		{
			Debug.Log("None");
		}
	}

	public void ScrollToQuest(Quest.ID NewQuestID)
	{
		ResearchLevel researchLevel = FindLevelFromResearchQuest(NewQuestID);
		if ((bool)researchLevel)
		{
			Quest quest = QuestManager.Instance.GetQuest(NewQuestID);
			float num = 0f - researchLevel.GetTechnologyFromQuest(quest).transform.localPosition.y;
			num -= researchLevel.transform.localPosition.y + m_ScrollView.GetHeight() / 2f;
			float num2 = m_ScrollView.GetScrollSize() - m_ScrollView.GetHeight();
			float scrollValue = 1f - num / num2;
			m_ScrollView.SetScrollValue(scrollValue);
		}
		else
		{
			Debug.Log("None");
		}
	}

	public ResearchLevel FindLevelFromResearchQuest(Quest.ID NewQuest)
	{
		foreach (ResearchLevel level in m_Levels)
		{
			if (level.GetResearchFromQuest(NewQuest) != null)
			{
				return level;
			}
		}
		return null;
	}

	public void SetResearch(Quest.ID NewQuestID)
	{
		if (m_SelectedResearch != Quest.ID.Total)
		{
			ResearchLevel researchLevel = FindLevelFromResearchQuest(m_SelectedResearch);
			QuestManager.Instance.GetQuest(researchLevel.m_LevelInfo.m_ID);
			researchLevel.SetTechnologySelected(m_SelectedResearch, Selected: true);
		}
		ScrollToQuest(NewQuestID);
		FindLevelFromResearchQuest(NewQuestID).SetTechnologySelected(NewQuestID, Selected: true);
		m_SelectedResearch = NewQuestID;
	}

	public override void CeremonyPlaying(bool Playing, Quest NewQuest)
	{
		if (Playing)
		{
			ScrollToLevel(NewQuest);
		}
	}

	public bool SelectedTechnologyActive()
	{
		if (m_SelectedPanel == null)
		{
			return false;
		}
		return m_SelectedPanel.activeSelf;
	}

	public void HideSelectedTechnology()
	{
		m_SelectedTechnology.OnBackClicked(null);
	}
}
