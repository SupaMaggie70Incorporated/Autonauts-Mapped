using UnityEngine;

public class WorkerGroupPanel : WorkerInfoBase
{
	private static float m_GapSize = 5f;

	private BaseImage m_CollapseImage;

	private BaseText m_NameText;

	private bool m_StatusChanged;

	public override void Init()
	{
		base.Init();
		m_NameText = base.transform.Find("Name").GetComponent<BaseText>();
		m_CollapseImage = base.transform.Find("Collapse").GetComponent<BaseImage>();
		m_CollapseImage.SetAction(OnCollapseClick, m_CollapseImage);
		m_IsGroup = true;
	}

	public override string GetName()
	{
		return m_Group.m_Name;
	}

	public void UpdateName()
	{
		string text = m_Group.m_Name + " (" + GetWorkersAmount() + ")";
		m_NameText.SetText(text);
	}

	public int GetWorkersAmount()
	{
		int num = m_Group.m_WorkerUIDs.Count;
		foreach (int groupUID in m_Group.m_GroupUIDs)
		{
			WorkerGroup groupFromID = WorkerGroupManager.Instance.GetGroupFromID(groupUID);
			num += groupFromID.m_WorkerGroupPanel.GetWorkersAmount();
		}
		return num;
	}

	public void SetGroup(WorkerGroup Group)
	{
		m_Group = Group;
		UpdateName();
		UpdateSelectedColour();
		UpdateCollapseButton(ForceAllCollapsed: false);
	}

	protected void CollapseEnabled(bool Enabled)
	{
		m_CollapseImage.SetActive(Enabled);
	}

	private void UpdatePanelSize()
	{
		float y = GetHeight() - m_GapSize;
		GetComponent<RectTransform>().sizeDelta = new Vector2(GetComponent<RectTransform>().sizeDelta.x, y);
	}

	public void UpdateCollapseButton(bool ForceAllCollapsed, bool RemoveHiddenWorkers = true)
	{
		string text = "GroupCollapse";
		string rolloverFromID = "TabWorkersGroupCollapse";
		if (ForceAllCollapsed)
		{
			m_Group.m_Collapsed = true;
		}
		if (m_Group.m_Collapsed)
		{
			text = "GroupExpand";
			rolloverFromID = "TabWorkersGroupExpand";
		}
		m_CollapseImage.SetSprite("Tabs/" + text);
		m_CollapseImage.SetRolloverFromID(rolloverFromID);
		UpdatePanelSize();
		GameStateNormal component = GameStateManager.Instance.GetCurrentState().GetComponent<GameStateNormal>();
		foreach (int workerUID in m_Group.m_WorkerUIDs)
		{
			BaseClass objectFromUniqueID = ObjectTypeList.Instance.GetObjectFromUniqueID(workerUID, ErrorCheck: false);
			if ((bool)objectFromUniqueID)
			{
				Worker component2 = objectFromUniqueID.GetComponent<Worker>();
				bool flag = TabWorkers.Instance.AreAnyParentGroupsCollapsed(m_Group) || m_Group.m_Collapsed;
				component2.m_WorkerInfoPanel.gameObject.SetActive(!flag);
				if (RemoveHiddenWorkers && (bool)component && flag)
				{
					component.RemoveSelectedWorker(component2);
				}
			}
		}
		foreach (int groupUID in m_Group.m_GroupUIDs)
		{
			WorkerGroup groupFromID = WorkerGroupManager.Instance.GetGroupFromID(groupUID);
			bool flag2 = TabWorkers.Instance.AreAnyParentGroupsCollapsed(m_Group) || m_Group.m_Collapsed;
			groupFromID.m_WorkerGroupPanel.gameObject.SetActive(!flag2);
			groupFromID.m_WorkerGroupPanel.UpdateCollapseButton(ForceAllCollapsed, RemoveHiddenWorkers);
			if (RemoveHiddenWorkers && (bool)component && flag2)
			{
				component.RemoveSelectedGroup(groupFromID);
			}
		}
	}

	public void Collapse(bool ForceAll)
	{
		m_Group.m_Collapsed = true;
		UpdateCollapseButton(ForceAll);
		TabWorkers.Instance.ContentsChanged();
		foreach (int workerUID in m_Group.m_WorkerUIDs)
		{
			BaseClass objectFromUniqueID = ObjectTypeList.Instance.GetObjectFromUniqueID(workerUID, ErrorCheck: false);
			if ((bool)objectFromUniqueID)
			{
				objectFromUniqueID.GetComponent<Worker>().m_WorkerInfoPanel.gameObject.SetActive(value: false);
			}
		}
	}

	public void Expand(bool RemoveHiddenWorkers = true)
	{
		m_Group.m_Collapsed = false;
		UpdateCollapseButton(ForceAllCollapsed: false, RemoveHiddenWorkers);
		if (m_Group.m_GroupParentGroup != -1)
		{
			WorkerGroupManager.Instance.GetGroupFromID(m_Group.m_GroupParentGroup).m_WorkerGroupPanel.UpdatePanelSize();
		}
		TabWorkers.Instance.ContentsChanged();
	}

	public void OnCollapseClick(BaseGadget NewGadget)
	{
		m_Group.m_Collapsed = !m_Group.m_Collapsed;
		UpdateCollapseButton(ForceAllCollapsed: false);
		TabWorkers.Instance.ContentsChanged();
	}

	public void StatusChanged()
	{
		m_StatusChanged = true;
		int groupParentGroup = m_Group.m_GroupParentGroup;
		WorkerGroupManager.Instance.GetGroupFromID(groupParentGroup)?.m_WorkerGroupPanel.StatusChanged();
	}

	public override void UpdateStatusImage()
	{
		int num = 0;
		foreach (int workerUID in m_Group.m_WorkerUIDs)
		{
			BaseClass objectFromUniqueID = ObjectTypeList.Instance.GetObjectFromUniqueID(workerUID, ErrorCheck: false);
			if ((bool)objectFromUniqueID)
			{
				Worker component = objectFromUniqueID.GetComponent<Worker>();
				WorkerStatusIndicator.State state = component.m_WorkerIndicator.m_State;
				if (num < 2 && component.m_InterruptState == Farmer.State.Paused)
				{
					num = 2;
				}
				else if (num < 1 && component.m_WorkerInterpreter.GetCurrentScript() == null)
				{
					num = 1;
				}
				else if (num < 4 && (state == WorkerStatusIndicator.State.NoEnergy || state == WorkerStatusIndicator.State.NoTool))
				{
					num = 4;
				}
				else if (num < 3 && state == WorkerStatusIndicator.State.LowEnergy)
				{
					num = 3;
				}
			}
		}
		foreach (int groupUID in m_Group.m_GroupUIDs)
		{
			WorkerGroup groupFromID = WorkerGroupManager.Instance.GetGroupFromID(groupUID);
			if (num < 4 && groupFromID.m_WorkerGroupPanel.m_StatusIndex == 4)
			{
				num = 4;
			}
			else if (num < 3 && groupFromID.m_WorkerGroupPanel.m_StatusIndex == 3)
			{
				num = 3;
			}
			else if (num < 2 && groupFromID.m_WorkerGroupPanel.m_StatusIndex == 2)
			{
				num = 2;
			}
			else if (num < 1 && groupFromID.m_WorkerGroupPanel.m_StatusIndex == 1)
			{
				num = 1;
			}
		}
		SetStatus(num, "");
	}

	public void UpdateStatusChanged()
	{
		foreach (int groupUID in m_Group.m_GroupUIDs)
		{
			WorkerGroupManager.Instance.GetGroupFromID(groupUID).m_WorkerGroupPanel.UpdateStatusChanged();
		}
		if (m_StatusChanged)
		{
			m_StatusChanged = false;
			UpdateStatusImage();
		}
	}

	public override void UpdateSelectedColour()
	{
		float num = 0.5f;
		if (m_Selected)
		{
			num = 1f;
		}
		Color colour = m_Group.GetColour();
		Color colour2 = (new Color(1f, 1f, 1f) - colour) * (1f - num) + colour;
		m_Panel.SetColour(colour2);
	}

	public void UpdateWorkersAndGroups()
	{
		UpdatePanelSize();
		UpdateName();
		UpdateStatusImage();
	}

	public override void OnClick(bool Release)
	{
		if (Release && m_JustSelected)
		{
			m_JustSelected = false;
		}
		else
		{
			if (GameStateManager.Instance.GetCurrentState() == null)
			{
				return;
			}
			GameStateNormal component = GameStateManager.Instance.GetCurrentState().GetComponent<GameStateNormal>();
			if (component == null)
			{
				return;
			}
			if (!m_Selected)
			{
				if (Release)
				{
					return;
				}
				if (Input.GetKey(KeyCode.LeftControl))
				{
					component.AddSelectedGroup(m_Group);
					m_JustSelected = true;
					return;
				}
				component.ClearSelectedGroups();
				component.AddSelectedGroup(m_Group);
				m_JustSelected = true;
			}
			else
			{
				if (!Release)
				{
					if (Input.GetKey(KeyCode.LeftControl))
					{
						component.RemoveSelectedGroup(m_Group);
					}
					return;
				}
				if (Input.GetKey(KeyCode.LeftControl))
				{
					return;
				}
				if (component.GetSelectedGroup() != m_Group)
				{
					component.ClearSelectedGroups();
					component.AddSelectedGroup(m_Group);
					return;
				}
				component.RemoveSelectedGroup(m_Group);
			}
			UpdateSelectedColour();
		}
	}

	public override void OnDrag()
	{
		m_JustSelected = false;
	}

	public override void SetPosition(Vector3 Position)
	{
		base.SetPosition(Position);
		if (!m_Group.m_Collapsed)
		{
			float y = Position.y;
			Position.x += 5f;
			Position.y -= 27f;
			foreach (int groupUID in m_Group.m_GroupUIDs)
			{
				WorkerGroup groupFromID = WorkerGroupManager.Instance.GetGroupFromID(groupUID);
				groupFromID.m_WorkerGroupPanel.transform.localPosition = Position;
				groupFromID.m_WorkerGroupPanel.SetPosition(Position);
				Vector2 offsetMax = groupFromID.m_WorkerGroupPanel.GetComponent<RectTransform>().offsetMax;
				groupFromID.m_WorkerGroupPanel.GetComponent<RectTransform>().offsetMax = new Vector2(-5f, offsetMax.y);
				Position.y -= groupFromID.m_WorkerGroupPanel.GetComponent<RectTransform>().rect.height;
			}
			foreach (int workerUID in m_Group.m_WorkerUIDs)
			{
				BaseClass objectFromUniqueID = ObjectTypeList.Instance.GetObjectFromUniqueID(workerUID, ErrorCheck: false);
				if ((bool)objectFromUniqueID)
				{
					Worker component = objectFromUniqueID.GetComponent<Worker>();
					component.m_WorkerInfoPanel.transform.localPosition = Position;
					Vector2 offsetMax2 = component.m_WorkerInfoPanel.GetComponent<RectTransform>().offsetMax;
					component.m_WorkerInfoPanel.GetComponent<RectTransform>().offsetMax = new Vector2(-5f, offsetMax2.y);
					Position.y -= component.m_WorkerInfoPanel.GetComponent<RectTransform>().rect.height;
				}
			}
			Position.y = y;
			Position.y -= GetComponent<RectTransform>().rect.height;
		}
		UpdateWorkersAndGroups();
	}

	public override void UpdateVisible(float PanelHeight, float PanelY)
	{
		base.UpdateVisible(PanelHeight, PanelY);
		UpdateVisibleChildren(m_Group, PanelHeight, PanelY);
	}

	private void UpdateVisibleChildren(WorkerGroup NewGroup, float PanelHeight, float PanelY)
	{
		if (NewGroup.m_Collapsed)
		{
			return;
		}
		foreach (int workerUID in NewGroup.m_WorkerUIDs)
		{
			BaseClass objectFromUniqueID = ObjectTypeList.Instance.GetObjectFromUniqueID(workerUID, ErrorCheck: false);
			if ((bool)objectFromUniqueID)
			{
				objectFromUniqueID.GetComponent<Worker>().m_WorkerInfoPanel.UpdateVisible(PanelHeight, PanelY);
			}
		}
		foreach (int groupUID in NewGroup.m_GroupUIDs)
		{
			UpdateVisibleChildren(WorkerGroupManager.Instance.GetGroupFromID(groupUID), PanelHeight, PanelY);
		}
	}

	public override float GetHeight()
	{
		float num = base.GetHeight();
		if (!m_Group.m_Collapsed)
		{
			foreach (int workerUID in m_Group.m_WorkerUIDs)
			{
				BaseClass objectFromUniqueID = ObjectTypeList.Instance.GetObjectFromUniqueID(workerUID, ErrorCheck: false);
				if ((bool)objectFromUniqueID)
				{
					Worker component = objectFromUniqueID.GetComponent<Worker>();
					if ((bool)component.m_WorkerInfoPanel)
					{
						num += component.m_WorkerInfoPanel.GetComponent<RectTransform>().rect.height;
					}
					else
					{
						Debug.Log("Bot " + component.m_WorkerName + " missing info panel");
					}
				}
			}
			foreach (int groupUID in m_Group.m_GroupUIDs)
			{
				WorkerGroup groupFromID = WorkerGroupManager.Instance.GetGroupFromID(groupUID);
				num += groupFromID.m_WorkerGroupPanel.GetHeight();
				num -= m_GapSize;
			}
			if (m_Group.m_GroupUIDs.Count != 0)
			{
				num += 3f;
			}
		}
		return num + m_GapSize;
	}
}
