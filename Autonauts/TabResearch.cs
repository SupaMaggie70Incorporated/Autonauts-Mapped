using System.Collections.Generic;
using UnityEngine;

public class TabResearch : TabQuestBase
{
	public static TabResearch Instance;

	private List<Quest> m_NewQuests;

	private QuestPanel m_ResearchPanelPrefab;

	private Transform m_Parent;

	protected new void Awake()
	{
		Instance = this;
		base.Awake();
	}

	protected new void Start()
	{
		base.Start();
		m_NewQuests = new List<Quest>();
		m_Parent = GetScrollView().GetContent().transform;
		m_ResearchPanelPrefab = m_Parent.Find("DefaultResearchPanel").GetComponent<QuestPanel>();
		m_ResearchPanelPrefab.gameObject.SetActive(value: false);
	}

	public void NewQuestAdded(Quest NewQuest)
	{
		if (!m_NewQuests.Contains(NewQuest))
		{
			m_NewQuests.Add(NewQuest);
		}
	}

	private static int SortQuestsByCost(QuestPanel p1, QuestPanel p2)
	{
		int required = p1.m_Quest.m_EventsRequired[0].m_Required;
		int required2 = p2.m_Quest.m_EventsRequired[0].m_Required;
		return required - required2;
	}

	public void UpdateLists()
	{
		HudManager.Instance.ActivateQuestRollover(Activate: false, null);
		foreach (QuestPanel activeQuest in m_ActiveQuests)
		{
			activeQuest.m_Used = false;
		}
		bool flag = false;
		foreach (Quest activeQuest2 in QuestManager.Instance.m_ActiveQuests)
		{
			if (activeQuest2.m_Type != Quest.Type.Research || !activeQuest2.m_Started)
			{
				continue;
			}
			QuestPanel questPanel = null;
			foreach (QuestPanel activeQuest3 in m_ActiveQuests)
			{
				if (activeQuest3.m_Quest == activeQuest2)
				{
					activeQuest3.m_Used = true;
					questPanel = activeQuest3;
					break;
				}
			}
			if (!(questPanel != null))
			{
				if (m_PanelCache.Count == 0)
				{
					questPanel = Object.Instantiate(m_ResearchPanelPrefab, m_ResearchPanelPrefab.transform.position, Quaternion.identity, m_Parent).GetComponent<QuestPanel>();
					RegisterGadget(questPanel.m_Panel);
				}
				else
				{
					questPanel = m_PanelCache[m_PanelCache.Count - 1];
					m_PanelCache.RemoveAt(m_PanelCache.Count - 1);
				}
				questPanel.m_Used = true;
				questPanel.gameObject.SetActive(value: true);
				AddAction(questPanel.m_Panel, OnClick);
				bool @new = false;
				if (m_NewQuests.Contains(activeQuest2))
				{
					@new = true;
					flag = true;
				}
				questPanel.SetQuest(activeQuest2, @new);
				if (!m_ActiveQuests.Contains(questPanel))
				{
					m_ActiveQuests.Add(questPanel);
				}
			}
		}
		List<QuestPanel> list = new List<QuestPanel>();
		foreach (QuestPanel activeQuest4 in m_ActiveQuests)
		{
			if (!activeQuest4.m_Used)
			{
				list.Add(activeQuest4);
			}
		}
		foreach (QuestPanel item in list)
		{
			m_ActiveQuests.Remove(item);
			RemoveAction(item.m_Panel);
			item.gameObject.SetActive(value: false);
			m_PanelCache.Add(item);
		}
		if (flag)
		{
			AddNew();
		}
		m_ActiveQuests.Sort(SortQuestsByCost);
		float num = -10f;
		foreach (QuestPanel activeQuest5 in m_ActiveQuests)
		{
			Vector3 localPosition = activeQuest5.transform.localPosition;
			localPosition.x = 10f;
			localPosition.y = num;
			activeQuest5.transform.localPosition = localPosition;
			num -= activeQuest5.GetComponent<RectTransform>().rect.height + 5f;
		}
		m_Parent.GetComponent<RectTransform>().sizeDelta = new Vector2(0f, 0f - num);
	}

	public void UpdateAll()
	{
		for (int i = 0; i < m_ActiveQuests.Count; i++)
		{
			m_ActiveQuests[i].UpdateAll();
		}
	}

	public void QuestSeen(Quest NewQuest)
	{
		if (m_NewQuests.Contains(NewQuest))
		{
			m_NewQuests.Remove(NewQuest);
		}
	}

	public void OnClick(BaseGadget NewGadget)
	{
		foreach (QuestPanel activeQuest in m_ActiveQuests)
		{
			if (activeQuest.m_Panel.GetComponent<BaseGadget>() == NewGadget)
			{
				GameStateIndustry.SetSelectedQuest(activeQuest.m_Quest);
				break;
			}
		}
		GameStateManager.Instance.SetState(GameStateManager.State.Industry);
	}
}
