using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TabWorkers : Tab
{
	public static TabWorkers Instance;

	private List<WorkerInfoBase> m_AllPanels;

	private List<WorkerInfoPanel> m_WorkerInfo;

	private List<WorkerInfoPanel> m_WorkerInfoUnused;

	public List<WorkerGroupPanel> m_Group;

	private List<WorkerGroupPanel> m_GroupUnused;

	private WorkerInfoPanel m_WorkerInfoPrefab;

	private WorkerGroupPanel m_GroupPrefab;

	private BaseText m_Title;

	private bool m_Changed;

	private bool m_ScrollChanged;

	private BaseButtonImage m_SortButton;

	private static bool m_SortDown;

	private string m_LastGroupSprite;

	public WorkerInfoPanel m_SelectedGroup;

	private WorkerInfoBase m_CurrentHighlighted;

	private bool m_Drag;

	private BaseScrollView m_ScrollView;

	private Transform m_ContentParent;

	private Image m_WorkerDrag;

	private List<Worker> m_DragWorkerList;

	private Image m_GroupDrag;

	private List<WorkerGroup> m_DragGroupList;

	private BaseButtonImage m_SearchButton;

	public BotServer m_BotServer;

	private int MaximumGroupNestsAllowed = 5;

	private int m_ChildStepsBest;

	private int m_ChildStepsCurrent;

	protected new void Awake()
	{
		Instance = this;
		base.Awake();
	}

	protected new void Start()
	{
		base.Start();
		SetType(TabManager.TabType.Workers);
		Transform transform = GetPanel().transform;
		m_Title = transform.GetComponent<BasePanelOptions>().GetTitle();
		BaseButtonImage component = transform.Find("NewGroupButton").GetComponent<BaseButtonImage>();
		AddAction(component, OnGroupButtonClicked);
		component = transform.Find("ExpandButton").GetComponent<BaseButtonImage>();
		AddAction(component, OnExpandAllButtonClicked);
		component = transform.Find("CollapseButton").GetComponent<BaseButtonImage>();
		AddAction(component, OnCollapseAllButtonClicked);
		m_SortButton = transform.Find("SortButton").GetComponent<BaseButtonImage>();
		AddAction(m_SortButton, OnSortButtonClicked);
		m_SearchButton = transform.Find("SearchButton").GetComponent<BaseButtonImage>();
		AddAction(m_SearchButton, OnSearchButtonClicked);
		m_SearchButton.SetActive(Active: false);
		m_ScrollView = GetScrollView();
		m_ScrollView.SetScrollChangedAction(OnScrollChanged);
		m_ScrollView.verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.AutoHide;
		m_ContentParent = m_ScrollView.GetContent().transform;
		m_WorkerInfoPrefab = m_ContentParent.Find("WorkerInfoPanel").GetComponent<WorkerInfoPanel>();
		m_WorkerInfoPrefab.gameObject.SetActive(value: false);
		m_GroupPrefab = m_ContentParent.Find("GroupPanel").GetComponent<WorkerGroupPanel>();
		m_GroupPrefab.gameObject.SetActive(value: false);
		m_AllPanels = new List<WorkerInfoBase>();
		m_WorkerInfo = new List<WorkerInfoPanel>();
		m_WorkerInfoUnused = new List<WorkerInfoPanel>();
		m_Group = new List<WorkerGroupPanel>();
		m_GroupUnused = new List<WorkerGroupPanel>();
		m_Changed = false;
		m_SortDown = true;
		UpdateSortButton();
		m_Drag = false;
		m_DragWorkerList = new List<Worker>();
		m_DragGroupList = new List<WorkerGroup>();
	}

	public override void SetActive(bool Active, bool Open)
	{
		base.SetActive(Active, Open);
		Dictionary<BaseClass, int> collection = CollectionManager.Instance.GetCollection("Worker");
		if (collection != null)
		{
			foreach (KeyValuePair<BaseClass, int> item in collection)
			{
				item.Key.GetComponent<Worker>().ShowName(Active);
			}
		}
		if (Active)
		{
			ContentsChanged();
		}
	}

	public WorkerInfoPanel AddWorker(Worker NewWorker)
	{
		WorkerInfoPanel workerInfoPanel;
		if (m_WorkerInfoUnused.Count > 0)
		{
			workerInfoPanel = m_WorkerInfoUnused[0];
			workerInfoPanel.gameObject.SetActive(value: true);
			m_WorkerInfoUnused.RemoveAt(0);
		}
		else
		{
			workerInfoPanel = Object.Instantiate(m_WorkerInfoPrefab, new Vector3(0f, 0f, 0f), Quaternion.identity, m_ContentParent).GetComponent<WorkerInfoPanel>();
			workerInfoPanel.gameObject.SetActive(value: true);
			workerInfoPanel.Init();
		}
		workerInfoPanel.SetWorker(NewWorker);
		m_WorkerInfo.Add(workerInfoPanel);
		m_AllPanels.Add(workerInfoPanel);
		ContentsChanged();
		return workerInfoPanel;
	}

	public void RemoveWorker(Worker NewWorker)
	{
		WorkerInfoPanel workerInfoPanel = NewWorker.m_WorkerInfoPanel;
		workerInfoPanel.gameObject.SetActive(value: false);
		m_WorkerInfoUnused.Add(workerInfoPanel);
		if (NewWorker.m_Group != null)
		{
			NewWorker.m_Group.RemoveWorker(NewWorker);
		}
		else
		{
			m_WorkerInfo.Remove(workerInfoPanel);
			m_AllPanels.Remove(workerInfoPanel);
		}
		if (GameStateManager.Instance != null && GameStateManager.Instance.GetState(GameStateManager.State.Normal) != null)
		{
			GameStateNormal component = GameStateManager.Instance.GetState(GameStateManager.State.Normal).GetComponent<GameStateNormal>();
			if ((bool)component)
			{
				component.RemoveSelectedWorker(NewWorker);
			}
		}
		ContentsChanged();
	}

	public WorkerGroupPanel AddGroup(WorkerGroup NewGroup)
	{
		WorkerGroupPanel component = Object.Instantiate(m_GroupPrefab, default(Vector3), Quaternion.identity, m_ContentParent).GetComponent<WorkerGroupPanel>();
		component.gameObject.SetActive(value: true);
		component.Init();
		component.SetGroup(NewGroup);
		NewGroup.m_WorkerGroupPanel = component;
		m_Group.Add(component);
		m_AllPanels.Add(component);
		ContentsChanged();
		return component;
	}

	public void RemoveGroup(WorkerGroup NewGroup)
	{
		if (NewGroup.m_WorkerGroupPanel == m_SelectedGroup)
		{
			m_SelectedGroup = null;
		}
		m_Group.Remove(NewGroup.m_WorkerGroupPanel);
		m_AllPanels.Remove(NewGroup.m_WorkerGroupPanel);
		Object.Destroy(NewGroup.m_WorkerGroupPanel.gameObject);
		WorkerGroupManager.Instance.DeleteGroup(NewGroup);
		ContentsChanged();
	}

	private void UpdatePositions()
	{
		float num = 0f;
		foreach (WorkerInfoBase allPanel in m_AllPanels)
		{
			allPanel.SetPosition(new Vector3(0f, num, 0f));
			num -= allPanel.GetHeight();
		}
		num = 0f;
		UpdateVisible();
	}

	private void UpdateVisible()
	{
		float height = m_ScrollView.GetComponent<RectTransform>().rect.height;
		float scrollPosition = m_ScrollView.GetScrollPosition();
		foreach (WorkerInfoBase allPanel in m_AllPanels)
		{
			allPanel.UpdateVisible(height, scrollPosition);
		}
	}

	private static int SortPanelsByName(WorkerInfoBase p1, WorkerInfoBase p2)
	{
		if (!m_SortDown)
		{
			WorkerInfoBase workerInfoBase = p1;
			p1 = p2;
			p2 = workerInfoBase;
		}
		if (!p1.m_IsGroup && p2.m_IsGroup)
		{
			if (!m_SortDown)
			{
				return -1;
			}
			return 1;
		}
		if (p1.m_IsGroup && !p2.m_IsGroup)
		{
			if (!m_SortDown)
			{
				return 1;
			}
			return -1;
		}
		if (!p1.m_IsGroup && !p2.m_IsGroup && p1.GetName() == p2.GetName())
		{
			return p1.m_Worker.m_UniqueID - p2.m_Worker.m_UniqueID;
		}
		return string.CompareOrdinal(p1.GetName().ToUpper(), p2.GetName().ToUpper());
	}

	private static int SortPanelsByNameFromWorkerID(int p1ID, int p2ID)
	{
		Worker component = ObjectTypeList.Instance.GetObjectFromUniqueID(p1ID).GetComponent<Worker>();
		Worker component2 = ObjectTypeList.Instance.GetObjectFromUniqueID(p2ID).GetComponent<Worker>();
		WorkerInfoBase workerInfoPanel = component.m_WorkerInfoPanel;
		WorkerInfoBase workerInfoPanel2 = component2.m_WorkerInfoPanel;
		return SortPanelsByName(workerInfoPanel, workerInfoPanel2);
	}

	private static int SortPanelsByNameFromGroupID(int p1ID, int p2ID)
	{
		WorkerGroup groupFromID = WorkerGroupManager.Instance.GetGroupFromID(p1ID);
		WorkerGroup groupFromID2 = WorkerGroupManager.Instance.GetGroupFromID(p2ID);
		WorkerInfoBase workerGroupPanel = groupFromID.m_WorkerGroupPanel;
		WorkerInfoBase workerGroupPanel2 = groupFromID2.m_WorkerGroupPanel;
		return SortPanelsByName(workerGroupPanel, workerGroupPanel2);
	}

	private void UpdateTitle()
	{
		Dictionary<BaseClass, int> collection = CollectionManager.Instance.GetCollection("Worker");
		int num = 0;
		if (collection != null)
		{
			num = collection.Count;
		}
		string text = TextManager.Instance.Get("TabWorkersBots", num.ToString());
		m_Title.SetText(text);
	}

	private void UpdateContents()
	{
		foreach (WorkerInfoBase allPanel in m_AllPanels)
		{
			if (allPanel == null)
			{
				m_AllPanels.Remove(allPanel);
				break;
			}
		}
		float num = 0f;
		foreach (WorkerInfoBase allPanel2 in m_AllPanels)
		{
			num += allPanel2.GetHeight();
		}
		m_ContentParent.GetComponent<RectTransform>().sizeDelta = new Vector2(0f, num + 40f);
		m_AllPanels.Sort(SortPanelsByName);
		foreach (WorkerGroup group in WorkerGroupManager.Instance.m_Groups)
		{
			if (group.m_GroupUIDs.Count > 0)
			{
				group.m_GroupUIDs.Sort(SortPanelsByNameFromGroupID);
			}
		}
		foreach (WorkerGroup group2 in WorkerGroupManager.Instance.m_Groups)
		{
			group2.m_WorkerUIDs.Sort(SortPanelsByNameFromWorkerID);
		}
		UpdatePositions();
		UpdateVisible();
		UpdateTitle();
	}

	public void ContentsChanged()
	{
		m_Changed = true;
	}

	private void UpdateGroupStatus()
	{
		foreach (WorkerGroup group in WorkerGroupManager.Instance.m_Groups)
		{
			if (group.m_GroupParentGroup == -1)
			{
				group.m_WorkerGroupPanel.UpdateStatusChanged();
			}
		}
	}

	protected new void Update()
	{
		base.Update();
		if (m_Active)
		{
			UpdateDrag();
			if (m_Changed)
			{
				m_Changed = false;
				UpdateContents();
			}
			if (m_ScrollChanged)
			{
				m_ScrollChanged = false;
				UpdateVisible();
			}
			UpdateSearchAvailable();
			UpdateGroupStatus();
		}
	}

	public void OnValueChanged()
	{
		ContentsChanged();
	}

	public void OnScrollChanged(BaseScrollView NewView)
	{
		m_ScrollChanged = true;
	}

	private void UpdateSortButton()
	{
		string text = "SortUpArrow";
		if (m_SortDown)
		{
			text = "SortDownArrow";
		}
		m_SortButton.SetSprite("Tabs/" + text);
		string rolloverFromID = "TabWorkersSortUp";
		if (m_SortDown)
		{
			rolloverFromID = "TabWorkersSortDown";
		}
		m_SortButton.SetRolloverFromID(rolloverFromID);
	}

	private void ExpandPanel(WorkerInfoBase NewPanel)
	{
		if (!NewPanel.m_IsGroup || NewPanel.m_Group == null)
		{
			return;
		}
		if (NewPanel.m_Group.m_GroupParentGroup != -1)
		{
			WorkerGroup groupFromID = WorkerGroupManager.Instance.GetGroupFromID(NewPanel.m_Group.m_GroupParentGroup);
			if (groupFromID != null)
			{
				ExpandPanel(groupFromID.m_WorkerGroupPanel);
			}
		}
		if (NewPanel.m_Group.m_Collapsed)
		{
			NewPanel.m_Group.m_WorkerGroupPanel.Expand(RemoveHiddenWorkers: false);
		}
	}

	public void ScrollToPanel(WorkerInfoBase NewPanel)
	{
		if (!NewPanel.m_IsGroup && NewPanel.m_Worker.m_Group != null)
		{
			ExpandPanel(NewPanel.m_Worker.m_Group.m_WorkerGroupPanel);
		}
		else if (NewPanel.m_IsGroup)
		{
			ExpandPanel(NewPanel.m_Group.m_WorkerGroupPanel);
		}
		float y = 0f - NewPanel.transform.localPosition.y - NewPanel.GetComponent<RectTransform>().rect.height;
		Vector3 localPosition = new Vector3(0f, y, 0f);
		m_ContentParent.transform.localPosition = localPosition;
	}

	public void OnSortButtonClicked(BaseGadget NewGadget)
	{
		m_SortDown = !m_SortDown;
		UpdateSortButton();
		ContentsChanged();
	}

	public void OnSearchButtonClicked(BaseGadget NewGadget)
	{
		if ((bool)m_BotServer)
		{
			GameStateManager.Instance.StartSelectBuilding(m_BotServer);
		}
	}

	public void NewGroup(List<Worker> NewBots)
	{
		WorkerGroup workerGroup = WorkerGroupManager.Instance.CreateNewGroup();
		GameStateNormal component = GameStateManager.Instance.GetCurrentState().GetComponent<GameStateNormal>();
		if (workerGroup == null)
		{
			return;
		}
		AddGroup(workerGroup);
		if (NewBots != null)
		{
			AddWorkersToGroup(workerGroup, NewBots);
			if (workerGroup != null && workerGroup.m_WorkerUIDs != null && workerGroup.m_WorkerUIDs.Count >= 3)
			{
				QuestManager.Instance.AddEvent(QuestEvent.Type.Group3Bots, Bot: false, 0, null);
			}
			AudioManager.Instance.StartEvent("UIBotGrouped");
		}
		else if (component != null)
		{
			component.ClearSelectedGroups();
		}
		UpdateContents();
		ScrollToPanel(workerGroup.m_WorkerGroupPanel);
		if (component != null)
		{
			component.AddSelectedGroup(workerGroup);
		}
	}

	public void OnGroupButtonClicked(BaseGadget NewGadget)
	{
		if (Input.GetKey(KeyCode.LeftControl))
		{
			GameStateNormal component = GameStateManager.Instance.GetCurrentState().GetComponent<GameStateNormal>();
			NewGroup(component.m_SelectedWorkers);
		}
		else
		{
			NewGroup(null);
		}
	}

	public void OnCollapseAllButtonClicked(BaseGadget NewGadget)
	{
		foreach (WorkerGroupPanel item in m_Group)
		{
			item.Collapse(ForceAll: true);
		}
	}

	public void OnExpandAllButtonClicked(BaseGadget NewGadget)
	{
		foreach (WorkerGroup group in WorkerGroupManager.Instance.m_Groups)
		{
			group.m_WorkerGroupPanel.Expand();
		}
	}

	public void WorkerSelected(Worker NewWorker, bool Selected)
	{
		NewWorker.m_WorkerInfoPanel.Select(Selected);
	}

	public void GroupSelected(WorkerGroup NewGroup, bool Selected)
	{
		if ((bool)NewGroup.m_WorkerGroupPanel)
		{
			NewGroup.m_WorkerGroupPanel.Select(Selected);
		}
	}

	public void SetCurrentHighlighted(WorkerInfoBase NewPanel)
	{
		if (m_Drag && (bool)NewPanel)
		{
			WorkerInfoPanel component = NewPanel.GetComponent<WorkerInfoPanel>();
			if ((bool)component && component.m_Worker.m_Group != null)
			{
				NewPanel = component.m_Worker.m_Group.m_WorkerGroupPanel;
			}
		}
		m_CurrentHighlighted = NewPanel;
	}

	public void StartDrag()
	{
		GameStateNormal component = GameStateManager.Instance.GetCurrentState().GetComponent<GameStateNormal>();
		if (component != null && component.m_SelectedGroups.Count > 0 && component.m_SelectedGroups[0] != WorkerGroupManager.Instance.m_TempGroup)
		{
			m_Drag = true;
			m_WorkerDrag = null;
			GameStateManager.Instance.PushState(GameStateManager.State.Drag);
			if (m_GroupDrag == null)
			{
				GameObject original = (GameObject)Resources.Load("Prefabs/Hud/Tabs/TabWorkerDrag", typeof(GameObject));
				m_GroupDrag = Object.Instantiate(original, new Vector3(0f, 0f, 0f), Quaternion.identity).GetComponent<Image>();
				m_GroupDrag.transform.SetParent(HudManager.Instance.m_HUDRootTransform);
			}
			m_GroupDrag.gameObject.SetActive(value: true);
			foreach (WorkerGroup selectedGroup in component.m_SelectedGroups)
			{
				m_DragGroupList.Add(selectedGroup);
			}
			component.ClearSelectedGroups();
			component.ClearSelectedWorkers();
			foreach (WorkerGroup dragGroup in m_DragGroupList)
			{
				component.AddSelectedGroup(dragGroup);
			}
			m_DragGroupList.Clear();
		}
		else
		{
			if (!(component != null) || component.m_SelectedWorkers.Count <= 0)
			{
				return;
			}
			m_Drag = true;
			m_GroupDrag = null;
			GameStateManager.Instance.PushState(GameStateManager.State.Drag);
			if (m_WorkerDrag == null)
			{
				GameObject original2 = (GameObject)Resources.Load("Prefabs/Hud/Tabs/TabWorkerDrag", typeof(GameObject));
				m_WorkerDrag = Object.Instantiate(original2, new Vector3(0f, 0f, 0f), Quaternion.identity).GetComponent<Image>();
				m_WorkerDrag.transform.SetParent(HudManager.Instance.m_HUDRootTransform);
			}
			m_WorkerDrag.gameObject.SetActive(value: true);
			foreach (Worker selectedWorker in component.m_SelectedWorkers)
			{
				m_DragWorkerList.Add(selectedWorker);
			}
			component.ClearSelectedGroups();
			component.ClearSelectedWorkers();
			foreach (Worker dragWorker in m_DragWorkerList)
			{
				component.AddSelectedWorker(dragWorker);
			}
			m_DragWorkerList.Clear();
		}
	}

	public void EndDrag()
	{
		if (!m_Drag || (m_WorkerDrag == null && m_GroupDrag == null))
		{
			return;
		}
		GameStateManager.Instance.PopState();
		m_Drag = false;
		if (m_GroupDrag != null)
		{
			m_GroupDrag.gameObject.SetActive(value: false);
			if (QuestManager.Instance.GetQuestComplete(Quest.ID.GlueBotServer))
			{
				WorkerGroup workerGroup = null;
				if ((bool)m_CurrentHighlighted && (bool)m_CurrentHighlighted.GetComponent<WorkerGroupPanel>())
				{
					workerGroup = m_CurrentHighlighted.GetComponent<WorkerGroupPanel>().m_Group;
				}
				AddSelectedGroupToGroup(workerGroup);
				if (workerGroup != null)
				{
					AudioManager.Instance.StartEvent("UIBotGrouped");
				}
				else
				{
					AudioManager.Instance.StartEvent("UIBotUnGrouped");
				}
			}
		}
		else if (m_WorkerDrag != null)
		{
			m_WorkerDrag.gameObject.SetActive(value: false);
			WorkerGroup workerGroup2 = null;
			if ((bool)m_CurrentHighlighted && (bool)m_CurrentHighlighted.GetComponent<WorkerGroupPanel>())
			{
				workerGroup2 = m_CurrentHighlighted.GetComponent<WorkerGroupPanel>().m_Group;
			}
			AddSelectedWorkersToGroup(workerGroup2);
			if (workerGroup2 != null && workerGroup2.m_WorkerUIDs != null && workerGroup2.m_WorkerUIDs.Count >= 3)
			{
				QuestManager.Instance.AddEvent(QuestEvent.Type.Group3Bots, Bot: false, 0, null);
			}
			if (workerGroup2 != null)
			{
				AudioManager.Instance.StartEvent("UIBotGrouped");
			}
			else
			{
				AudioManager.Instance.StartEvent("UIBotUnGrouped");
			}
		}
		UpdateContents();
		SetCurrentHighlighted(m_CurrentHighlighted);
	}

	private void UpdateDrag()
	{
		if (!m_Drag || (m_WorkerDrag == null && m_GroupDrag == null))
		{
			return;
		}
		if (!Input.GetMouseButton(0))
		{
			EndDrag();
			return;
		}
		Vector3 localPosition = HudManager.Instance.ScreenToCanvas(Input.mousePosition);
		Image obj = ((m_WorkerDrag == null) ? m_GroupDrag : m_WorkerDrag);
		obj.transform.localPosition = localPosition;
		string text = "GroupRemove";
		if (m_CurrentHighlighted != null && (bool)m_CurrentHighlighted.GetComponent<WorkerGroupPanel>())
		{
			text = "GroupAdd";
		}
		Sprite sprite2 = (obj.sprite = (Sprite)Resources.Load("Textures/Hud/Tabs/" + text, typeof(Sprite)));
		Vector3 vector = m_ScrollView.transform.InverseTransformPoint(Input.mousePosition);
		float height = m_ScrollView.gameObject.GetComponent<RectTransform>().rect.height;
		float height2 = m_ContentParent.GetComponent<RectTransform>().rect.height;
		float num = 50f;
		if (vector.y > height / 2f - num)
		{
			float num2 = (vector.y - (height / 2f - num)) / num;
			Vector3 localPosition2 = m_ContentParent.localPosition;
			localPosition2.y -= 500f * TimeManager.Instance.m_NormalDeltaUnscaled * num2;
			if (localPosition2.y < 0f)
			{
				localPosition2.y = 0f;
			}
			m_ContentParent.localPosition = localPosition2;
		}
		else if (vector.y < (0f - height) / 2f + num * 2f)
		{
			float num3 = ((0f - height) / 2f + num * 2f - vector.y) / num;
			Vector3 localPosition3 = m_ContentParent.localPosition;
			localPosition3.y += 500f * TimeManager.Instance.m_NormalDeltaUnscaled * num3;
			float num4 = height2 - height;
			if (num4 < 0f)
			{
				num4 = 0f;
			}
			if (localPosition3.y > num4)
			{
				localPosition3.y = num4;
			}
			m_ContentParent.localPosition = localPosition3;
		}
	}

	public void StopDrag()
	{
		m_Drag = false;
		if (m_WorkerDrag != null)
		{
			m_WorkerDrag.gameObject.SetActive(value: false);
		}
		if (m_GroupDrag != null)
		{
			m_GroupDrag.gameObject.SetActive(value: false);
		}
		m_WorkerDrag = null;
		m_GroupDrag = null;
		SetCurrentHighlighted(m_CurrentHighlighted);
	}

	protected override void UpdateVisibility()
	{
		base.UpdateVisibility();
		if (m_Visible)
		{
			foreach (WorkerGroup group in WorkerGroupManager.Instance.m_Groups)
			{
				if (group.m_GroupParentGroup == -1)
				{
					UpdateChildrenVisibility(group);
				}
			}
			return;
		}
		GameStateNormal component = GameStateManager.Instance.GetCurrentState().GetComponent<GameStateNormal>();
		if ((bool)component)
		{
			component.ClearSelectedWorkers();
			component.ClearSelectedGroups();
		}
		ClearSelectedGroup();
	}

	private void UpdateChildrenVisibility(WorkerGroup ChildGroup)
	{
		if (!m_Visible)
		{
			return;
		}
		ChildGroup.m_WorkerGroupPanel.transform.SetParent(m_ScrollView.transform);
		ChildGroup.m_WorkerGroupPanel.transform.SetParent(m_ContentParent.transform);
		if (!AreAnyParentGroupsCollapsed(ChildGroup))
		{
			ChildGroup.m_WorkerGroupPanel.gameObject.SetActive(value: true);
		}
		else
		{
			ChildGroup.m_WorkerGroupPanel.gameObject.SetActive(value: false);
		}
		foreach (int workerUID in ChildGroup.m_WorkerUIDs)
		{
			BaseClass objectFromUniqueID = ObjectTypeList.Instance.GetObjectFromUniqueID(workerUID, ErrorCheck: false);
			if ((bool)objectFromUniqueID)
			{
				Worker component = objectFromUniqueID.GetComponent<Worker>();
				component.m_WorkerInfoPanel.transform.SetParent(m_ScrollView.transform);
				component.m_WorkerInfoPanel.transform.SetParent(m_ContentParent.transform);
				component.m_WorkerInfoPanel.gameObject.SetActive(!ChildGroup.m_Collapsed);
			}
		}
		ChildGroup.m_WorkerGroupPanel.UpdateCollapseButton(ForceAllCollapsed: false);
		foreach (int groupUID in ChildGroup.m_GroupUIDs)
		{
			WorkerGroup groupFromID = WorkerGroupManager.Instance.GetGroupFromID(groupUID);
			UpdateChildrenVisibility(groupFromID);
		}
	}

	public bool AreAnyParentGroupsCollapsed(WorkerGroup ChildGroup)
	{
		if (ChildGroup.m_GroupParentGroup != -1)
		{
			WorkerGroup groupFromID = WorkerGroupManager.Instance.GetGroupFromID(ChildGroup.m_GroupParentGroup);
			if (groupFromID.m_WorkerGroupPanel.m_Group.m_Collapsed)
			{
				return true;
			}
			return AreAnyParentGroupsCollapsed(groupFromID);
		}
		return false;
	}

	public void SetWorkerGroup(Worker NewWorker, WorkerGroup NewGroup)
	{
		if (NewWorker.m_Group != null)
		{
			NewWorker.m_Group.RemoveWorker(NewWorker);
			m_WorkerInfo.Add(NewWorker.m_WorkerInfoPanel);
			m_AllPanels.Add(NewWorker.m_WorkerInfoPanel);
			NewWorker.m_WorkerInfoPanel.UpdateSelectedColour();
		}
		if (NewGroup != null)
		{
			m_WorkerInfo.Remove(NewWorker.m_WorkerInfoPanel);
			m_AllPanels.Remove(NewWorker.m_WorkerInfoPanel);
			NewGroup.AddWorker(NewWorker);
			NewWorker.m_WorkerInfoPanel.UpdateSelectedColour();
			if (NewGroup.m_Collapsed)
			{
				NewWorker.m_WorkerInfoPanel.gameObject.SetActive(value: false);
			}
			if (m_Visible)
			{
				NewWorker.m_WorkerInfoPanel.transform.SetParent(m_ScrollView.transform);
				NewWorker.m_WorkerInfoPanel.transform.SetParent(m_ContentParent.transform);
			}
		}
		NewWorker.m_WorkerArrow.UpdateColour();
	}

	public void SetGroupGroup(WorkerGroup CurrentGroup, WorkerGroup NewGroup)
	{
		WorkerGroup groupFromID = WorkerGroupManager.Instance.GetGroupFromID(CurrentGroup.m_GroupParentGroup);
		if ((NewGroup == null || (groupFromID != null && NewGroup.m_ID != groupFromID.m_ID)) && groupFromID != null)
		{
			groupFromID.RemoveGroup(CurrentGroup);
			CurrentGroup.m_GroupParentGroup = -1;
			m_Group.Add(CurrentGroup.m_WorkerGroupPanel);
			m_AllPanels.Add(CurrentGroup.m_WorkerGroupPanel);
			CurrentGroup.m_WorkerGroupPanel.UpdateSelectedColour();
		}
		if (NewGroup == null || NewGroup.m_GroupUIDs.Contains(CurrentGroup.m_ID))
		{
			return;
		}
		m_Group.Remove(CurrentGroup.m_WorkerGroupPanel);
		m_AllPanels.Remove(CurrentGroup.m_WorkerGroupPanel);
		NewGroup.AddGroup(CurrentGroup);
		CurrentGroup.m_WorkerGroupPanel.UpdateSelectedColour();
		if (!m_Visible)
		{
			return;
		}
		UpdateChildrenVisibility(CurrentGroup);
		foreach (int groupUID in CurrentGroup.m_GroupUIDs)
		{
			WorkerGroup groupFromID2 = WorkerGroupManager.Instance.GetGroupFromID(groupUID);
			UpdateChildrenVisibility(groupFromID2);
		}
	}

	public void AddWorkersToGroup(WorkerGroup NewGroup, List<Worker> Bots)
	{
		GameStateNormal component = GameStateManager.Instance.GetCurrentState().GetComponent<GameStateNormal>();
		if (!component)
		{
			return;
		}
		foreach (Worker Bot in Bots)
		{
			SetWorkerGroup(Bot, NewGroup);
		}
		if (NewGroup != null && NewGroup.m_Collapsed)
		{
			component.ClearSelectedWorkers();
		}
	}

	private void AddSelectedWorkersToGroup(WorkerGroup NewGroup)
	{
		GameStateNormal component = GameStateManager.Instance.GetCurrentState().GetComponent<GameStateNormal>();
		if ((bool)component)
		{
			AddWorkersToGroup(NewGroup, component.m_SelectedWorkers);
		}
	}

	public void AddSelectedGroupToGroup(WorkerGroup NewGroup)
	{
		GameStateNormal component = GameStateManager.Instance.GetCurrentState().GetComponent<GameStateNormal>();
		if (!component)
		{
			return;
		}
		List<WorkerGroup> list = new List<WorkerGroup>();
		foreach (WorkerGroup selectedGroup in component.m_SelectedGroups)
		{
			list.Add(selectedGroup);
		}
		foreach (WorkerGroup item in list)
		{
			if ((NewGroup != null && (NewGroup == null || item.m_ID == NewGroup.m_ID)) || (NewGroup != null && NewGroup.m_GroupParentGroup == item.m_ID))
			{
				continue;
			}
			if (item == WorkerGroupManager.Instance.m_TempGroup)
			{
				return;
			}
			int num = 1;
			m_ChildStepsBest = 0;
			if (NewGroup != null)
			{
				num = CheckParentSteps(NewGroup, num);
				m_ChildStepsCurrent = 0;
				CheckChildSteps(item);
			}
			if (num + m_ChildStepsBest < MaximumGroupNestsAllowed)
			{
				SetGroupGroup(item, NewGroup);
				if (item != null)
				{
					HudManager.Instance.SetGroup(item);
					item.m_WorkerGroupPanel.Select(Selected: true);
				}
			}
		}
		if (NewGroup != null && NewGroup.m_Collapsed)
		{
			component.ClearSelectedGroups();
		}
	}

	public void SetSelectedGroup(WorkerInfoPanel Group)
	{
		ClearSelectedGroup();
		GameStateNormal component = GameStateManager.Instance.GetCurrentState().GetComponent<GameStateNormal>();
		if (!component)
		{
			return;
		}
		component.ClearSelectedWorkers();
		Group.Select(Selected: true);
		m_SelectedGroup = Group;
		if (!Group.m_Group.m_Collapsed)
		{
			foreach (int workerUID in m_SelectedGroup.m_Group.m_WorkerUIDs)
			{
				BaseClass objectFromUniqueID = ObjectTypeList.Instance.GetObjectFromUniqueID(workerUID, ErrorCheck: false);
				if (objectFromUniqueID != null)
				{
					component.AddSelectedWorker(objectFromUniqueID.GetComponent<Worker>(), Confirmed: true, Scale: true, ViaGroup: true);
				}
			}
		}
		HudManager.Instance.SetGroup(Group.m_Group);
	}

	public void ClearSelectedGroup()
	{
		if ((bool)m_SelectedGroup)
		{
			m_SelectedGroup.Select(Selected: false);
			m_SelectedGroup = null;
			HudManager.Instance.SetGroup(null);
		}
	}

	private int CheckParentSteps(WorkerGroup CurrentGroup, int CurrentSteps)
	{
		if (CurrentGroup.m_GroupParentGroup != -1)
		{
			WorkerGroup groupFromID = WorkerGroupManager.Instance.GetGroupFromID(CurrentGroup.m_GroupParentGroup);
			if (groupFromID != null)
			{
				CurrentSteps = CheckParentSteps(groupFromID, ++CurrentSteps);
			}
		}
		return CurrentSteps;
	}

	private void CheckChildSteps(WorkerGroup CurrentGroup)
	{
		foreach (int groupUID in CurrentGroup.m_GroupUIDs)
		{
			WorkerGroup groupFromID = WorkerGroupManager.Instance.GetGroupFromID(groupUID);
			if (groupFromID != null)
			{
				m_ChildStepsCurrent++;
				if (m_ChildStepsBest < m_ChildStepsCurrent)
				{
					m_ChildStepsBest = m_ChildStepsCurrent;
				}
				CheckChildSteps(groupFromID);
				m_ChildStepsCurrent--;
			}
		}
	}

	public void UpdateSearchAvailable()
	{
		bool active = false;
		m_BotServer = null;
		if ((bool)BotServer.m_FirstBotServer)
		{
			active = true;
			m_BotServer = BotServer.m_FirstBotServer;
		}
		m_SearchButton.SetActive(active);
	}
}
