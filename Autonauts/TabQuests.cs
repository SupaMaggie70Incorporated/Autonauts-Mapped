using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TabQuests : TabQuestBase
{
	public static TabQuests Instance;

	private List<Quest> m_OldQuests;

	private Transform m_Parent;

	private Image m_Divider;

	private QuestPanel m_QuestPanelPrefab;

	private BaseText m_Title;

	private StandardButtonImage m_PinButton;

	protected new void Awake()
	{
		Instance = this;
		base.Awake();
	}

	protected new void Start()
	{
		base.Start();
		SetType(TabManager.TabType.Quests);
		m_OldQuests = new List<Quest>();
		m_Parent = GetScrollView().GetContent().transform;
		m_QuestPanelPrefab = m_Parent.Find("DefaultQuestPanel").GetComponent<QuestPanel>();
		m_QuestPanelPrefab.gameObject.SetActive(value: false);
		Transform transform = base.transform.Find("TabPanel/BasePanelOptions/Panel");
		m_Title = transform.Find("TitleStrip/Title").GetComponent<BaseText>();
		m_PinButton = transform.Find("Pin").GetComponent<StandardButtonImage>();
		m_PinButton.SetAction(OnPinClicked, m_PinButton);
	}

	public void OnPinClicked(BaseGadget NewGadget)
	{
		GameStateManager.Instance.SetState(GameStateManager.State.Autopedia);
		Autopedia.Instance.SetPage(Autopedia.Page.Academy);
	}

	private static int SortQuests(QuestPanel p1, QuestPanel p2)
	{
		if (p1.m_Quest.m_Type == Quest.Type.Tutorial)
		{
			if (p1.m_Quest.m_Type == p2.m_Quest.m_Type)
			{
				return p1.m_Quest.m_ID - p2.m_Quest.m_ID;
			}
			return -1;
		}
		if (p2.m_Quest.m_Type == Quest.Type.Tutorial)
		{
			return 1;
		}
		if (p1.m_Quest.m_Type == Quest.Type.Infrastructure)
		{
			if (p1.m_Quest.m_Type == p2.m_Quest.m_Type)
			{
				return p1.m_Quest.m_ID - p2.m_Quest.m_ID;
			}
			return -1;
		}
		if (p2.m_Quest.m_Type == Quest.Type.Infrastructure)
		{
			return 1;
		}
		if (p1.m_Quest.m_Importantance != p2.m_Quest.m_Importantance)
		{
			return p1.m_Quest.m_Importantance - p2.m_Quest.m_Importantance;
		}
		return p1.m_Quest.m_ID - p2.m_Quest.m_ID;
	}

	private void CheckQuestDependencies(Quest CurrentQuest, List<Quest> DependantQuests)
	{
		if (CurrentQuest.m_Type == Quest.Type.Research)
		{
			Quest questFromUnlockedObject = QuestData.Instance.GetQuestFromUnlockedObject(ObjectType.FolkSeedRehydrator);
			if (questFromUnlockedObject != null && !DependantQuests.Contains(questFromUnlockedObject))
			{
				int index = DependantQuests.IndexOf(CurrentQuest);
				DependantQuests.Insert(index, questFromUnlockedObject);
				CheckQuestDependencies(questFromUnlockedObject, DependantQuests);
			}
			questFromUnlockedObject = QuestData.Instance.GetQuestFromUnlockedObject(ObjectType.ResearchStationCrude);
			if (questFromUnlockedObject != null && !DependantQuests.Contains(questFromUnlockedObject))
			{
				int index2 = DependantQuests.IndexOf(CurrentQuest);
				DependantQuests.Insert(index2, questFromUnlockedObject);
				CheckQuestDependencies(questFromUnlockedObject, DependantQuests);
			}
		}
		using (List<Quest.ID>.Enumerator enumerator = CurrentQuest.m_ReliesOn.GetEnumerator())
		{
			if (enumerator.MoveNext())
			{
				Quest.ID current = enumerator.Current;
				Quest quest = QuestManager.Instance.GetQuest(current);
				if (quest != null && !DependantQuests.Contains(quest))
				{
					int index3 = DependantQuests.IndexOf(CurrentQuest);
					DependantQuests.Insert(index3, quest);
					CheckQuestDependencies(quest, DependantQuests);
				}
			}
		}
		foreach (QuestEvent item in CurrentQuest.m_EventsRequired)
		{
			if (item.m_Type == QuestEvent.Type.CompleteResearch)
			{
				Quest questFromUnlockedObject2 = QuestData.Instance.GetQuestFromUnlockedObject(ObjectType.FolkSeedRehydrator);
				if (questFromUnlockedObject2 != null && !DependantQuests.Contains(questFromUnlockedObject2))
				{
					int index4 = DependantQuests.IndexOf(CurrentQuest);
					DependantQuests.Insert(index4, questFromUnlockedObject2);
					CheckQuestDependencies(questFromUnlockedObject2, DependantQuests);
				}
				questFromUnlockedObject2 = QuestData.Instance.GetQuestFromUnlockedObject(ObjectType.ResearchStationCrude);
				if (questFromUnlockedObject2 != null && !DependantQuests.Contains(questFromUnlockedObject2))
				{
					int index5 = DependantQuests.IndexOf(CurrentQuest);
					DependantQuests.Insert(index5, questFromUnlockedObject2);
					CheckQuestDependencies(questFromUnlockedObject2, DependantQuests);
				}
			}
			if (item.m_Type == QuestEvent.Type.CompleteMission)
			{
				string extraDataAsString = item.GetExtraDataAsString();
				Quest.ID questIDFromName = QuestData.Instance.GetQuestIDFromName(extraDataAsString);
				if (questIDFromName != Quest.ID.Total)
				{
					Quest quest2 = QuestManager.Instance.GetQuest(questIDFromName);
					if (quest2 != null && !DependantQuests.Contains(quest2))
					{
						int index6 = DependantQuests.IndexOf(CurrentQuest);
						DependantQuests.Insert(index6, quest2);
						CheckQuestDependencies(quest2, DependantQuests);
					}
				}
			}
			if (item.m_Type != QuestEvent.Type.Build && item.m_Type != 0)
			{
				continue;
			}
			string extraDataAsString2 = item.GetExtraDataAsString();
			ObjectType identifierFromSaveName = ObjectTypeList.Instance.GetIdentifierFromSaveName(extraDataAsString2);
			if (identifierFromSaveName == ObjectTypeList.m_Total)
			{
				continue;
			}
			Quest questFromUnlockedObject3 = QuestManager.Instance.m_Data.GetQuestFromUnlockedObject(identifierFromSaveName);
			if (questFromUnlockedObject3 != null && !DependantQuests.Contains(questFromUnlockedObject3))
			{
				int index7 = DependantQuests.IndexOf(CurrentQuest);
				DependantQuests.Insert(index7, questFromUnlockedObject3);
				CheckQuestDependencies(questFromUnlockedObject3, DependantQuests);
			}
			ObjectType converterForObject = VariableManager.Instance.GetConverterForObject(identifierFromSaveName);
			if (converterForObject != ObjectTypeList.m_Total)
			{
				Quest questFromUnlockedObject4 = QuestData.Instance.GetQuestFromUnlockedObject(converterForObject);
				if (questFromUnlockedObject4 != null && !DependantQuests.Contains(questFromUnlockedObject4))
				{
					int index8 = DependantQuests.IndexOf(CurrentQuest);
					DependantQuests.Insert(index8, questFromUnlockedObject4);
					CheckQuestDependencies(questFromUnlockedObject4, DependantQuests);
				}
			}
		}
	}

	private List<Quest> GetActiveQuests()
	{
		List<Quest> list = new List<Quest>();
		foreach (Quest activeQuest in QuestManager.Instance.m_ActiveQuests)
		{
			list.Add(activeQuest);
		}
		return list;
	}

	public void UpdateLists()
	{
		HudManager.Instance.ActivateQuestRollover(Activate: false, null);
		foreach (QuestPanel activeQuest in m_ActiveQuests)
		{
			activeQuest.m_Used = false;
		}
		if ((bool)m_Divider)
		{
			Object.Destroy(m_Divider.gameObject);
			m_Divider = null;
		}
		List<Quest> activeQuests = GetActiveQuests();
		bool flag = false;
		foreach (Quest item in activeQuests)
		{
			if (!item.m_Pinned || item.m_Complete)
			{
				continue;
			}
			QuestPanel questPanel = null;
			foreach (QuestPanel activeQuest2 in m_ActiveQuests)
			{
				if (activeQuest2.m_Quest == item)
				{
					activeQuest2.m_Used = true;
					questPanel = activeQuest2;
					break;
				}
			}
			if (!(questPanel != null))
			{
				if (m_PanelCache.Count == 0)
				{
					questPanel = Object.Instantiate(m_QuestPanelPrefab, new Vector3(0f, 0f, 0f), Quaternion.identity, m_Parent).GetComponent<QuestPanel>();
					RegisterGadget(questPanel.m_Panel);
				}
				else
				{
					questPanel = m_PanelCache[m_PanelCache.Count - 1];
					m_PanelCache.RemoveAt(m_PanelCache.Count - 1);
				}
				questPanel.m_Used = true;
				questPanel.transform.localPosition = m_QuestPanelPrefab.transform.localPosition;
				questPanel.gameObject.SetActive(value: true);
				AddAction(questPanel.m_Panel, OnClick);
				bool @new = false;
				if (!m_OldQuests.Contains(item) && !SaveLoadManager.Instance.m_Loading)
				{
					@new = true;
					flag = true;
				}
				questPanel.SetQuest(item, @new);
				m_ActiveQuests.Add(questPanel);
			}
		}
		List<QuestPanel> list = new List<QuestPanel>();
		foreach (QuestPanel activeQuest3 in m_ActiveQuests)
		{
			if (!activeQuest3.m_Used)
			{
				list.Add(activeQuest3);
			}
		}
		foreach (QuestPanel item2 in list)
		{
			m_ActiveQuests.Remove(item2);
			RemoveAction(item2.m_Panel);
			item2.gameObject.SetActive(value: false);
			m_PanelCache.Add(item2);
		}
		if (flag)
		{
			AddNew();
		}
		m_ActiveQuests.Sort(SortQuests);
		int num = 100;
		foreach (QuestPanel activeQuest4 in m_ActiveQuests)
		{
			if (activeQuest4.m_Quest.m_Importantance < num)
			{
				num = activeQuest4.m_Quest.m_Importantance;
			}
		}
		float num2 = -10f;
		foreach (QuestPanel activeQuest5 in m_ActiveQuests)
		{
			Vector3 localPosition = activeQuest5.transform.localPosition;
			localPosition.x = 10f;
			localPosition.y = num2;
			activeQuest5.transform.localPosition = localPosition;
			num2 -= activeQuest5.GetHeight() + 5f;
		}
		m_OldQuests.Clear();
		foreach (QuestPanel activeQuest6 in m_ActiveQuests)
		{
			m_OldQuests.Add(activeQuest6.m_Quest);
		}
		m_Parent.GetComponent<RectTransform>().sizeDelta = new Vector2(0f, 0f - num2);
	}

	public void UpdateAll()
	{
		for (int i = 0; i < m_ActiveQuests.Count; i++)
		{
			m_ActiveQuests[i].UpdateAll();
		}
	}

	public bool AnyActiveQuests()
	{
		return m_ActiveQuests.Count != 0;
	}

	public void UpdateLumber()
	{
		foreach (QuestPanel activeQuest in m_ActiveQuests)
		{
			if (activeQuest.m_Quest.m_ID == Quest.ID.AcademyLumber2)
			{
				activeQuest.UpdateLesson();
				break;
			}
		}
	}

	public void OnClick(BaseGadget NewGadget)
	{
		GameStateManager.Instance.SetState(GameStateManager.State.Autopedia);
		Autopedia.Instance.SetPage(Autopedia.Page.Academy);
		foreach (QuestPanel activeQuest in m_ActiveQuests)
		{
			if (activeQuest.m_Panel.GetComponent<BaseGadget>() == NewGadget)
			{
				Academy.Instance.ScrollToQuest(activeQuest.m_Quest);
				break;
			}
		}
	}
}
