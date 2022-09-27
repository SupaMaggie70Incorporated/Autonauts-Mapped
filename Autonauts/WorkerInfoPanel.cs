using UnityEngine;

public class WorkerInfoPanel : WorkerInfoBase
{
	public string m_WorkerName;

	public ObjectType m_LastHeldItem;

	private BaseText m_NameInput;

	private BaseImage m_HeldItemImage;

	public override void Init()
	{
		base.Init();
		m_Worker = null;
		m_IsGroup = false;
	}

	protected override void CheckGadgets()
	{
		if (!m_NameInput)
		{
			base.CheckGadgets();
			m_NameInput = base.transform.Find("Name").GetComponent<BaseText>();
			m_HeldItemImage = base.transform.Find("Item").GetComponent<BaseImage>();
		}
	}

	public override string GetName()
	{
		return m_WorkerName;
	}

	protected void HeldItemEnabled(bool Enabled)
	{
		m_HeldItemImage.SetActive(Enabled);
	}

	public void SetWorker(Worker NewWorker)
	{
		CheckGadgets();
		m_Worker = NewWorker;
		UpdateName();
		UpdateStatusImage();
		UpdateHeldItem(FirstTime: true);
		UpdateSelectedColour();
	}

	public void UpdateName()
	{
		string workerName = m_Worker.GetWorkerName();
		m_NameInput.SetText(workerName);
		m_WorkerName = workerName;
	}

	public override void UpdateStatusImage()
	{
		int statusIndex = 0;
		WorkerStatusIndicator.State state = m_Worker.m_WorkerIndicator.m_State;
		if (m_Worker.m_InterruptState == Farmer.State.Paused)
		{
			statusIndex = 2;
		}
		else if (m_Worker.m_WorkerInterpreter.GetCurrentScript() == null)
		{
			statusIndex = 1;
		}
		else
		{
			switch (state)
			{
			case WorkerStatusIndicator.State.NoTool:
			case WorkerStatusIndicator.State.NoEnergy:
				statusIndex = 4;
				break;
			case WorkerStatusIndicator.State.Question:
			case WorkerStatusIndicator.State.LowEnergy:
				statusIndex = 3;
				break;
			}
		}
		string stateRolloverName = m_Worker.m_WorkerIndicator.GetStateRolloverName();
		SetStatus(statusIndex, stateRolloverName);
		if (m_Worker.m_Group != null)
		{
			m_Worker.m_Group.m_WorkerGroupPanel.StatusChanged();
		}
	}

	public void UpdateHeldItem(bool FirstTime = false)
	{
		if (m_Worker == null)
		{
			return;
		}
		ObjectType lastObjectType = m_Worker.m_FarmerCarry.GetLastObjectType();
		if (lastObjectType != ObjectTypeList.m_Total || FirstTime)
		{
			m_LastHeldItem = lastObjectType;
			if (lastObjectType != ObjectTypeList.m_Total)
			{
				m_HeldItemImage.SetSprite(IconManager.Instance.GetIcon(lastObjectType));
			}
			else
			{
				m_HeldItemImage.SetSprite("Icons/IconEmpty");
			}
			string rolloverFromID = "Nothing";
			if (lastObjectType != ObjectTypeList.m_Total)
			{
				rolloverFromID = ObjectTypeList.Instance.GetSaveNameFromIdentifier(lastObjectType);
			}
			m_HeldItemImage.SetRolloverFromID(rolloverFromID);
		}
	}

	public override void UpdateSelectedColour()
	{
		float num = 0.5f;
		if (m_Selected)
		{
			num = 1f;
		}
		num *= 0.8f;
		Color color = WorkerGroup.m_DefaultColour;
		if (m_Worker.m_Group != null)
		{
			color = m_Worker.m_Group.GetColour();
		}
		Color colour = (new Color(1f, 1f, 1f) - color) * (1f - num) + color;
		m_Panel.SetColour(colour);
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
			if (m_Worker.m_Group != null)
			{
				foreach (WorkerGroup selectedGroup in component.m_SelectedGroups)
				{
					if (selectedGroup.m_ID == m_Worker.m_Group.m_ID)
					{
						m_Selected = false;
					}
				}
			}
			component.ClearSelectedGroups();
			if (!m_Selected)
			{
				if (Release)
				{
					return;
				}
				if (Input.GetKey(KeyCode.LeftControl))
				{
					component.AddSelectedWorker(m_Worker);
					m_JustSelected = true;
					return;
				}
				component.ClearSelectedWorkers();
				component.AddSelectedWorker(m_Worker);
				CameraManager.Instance.Focus(m_Worker.transform.position);
				m_JustSelected = true;
			}
			else
			{
				if (!Release)
				{
					if (Input.GetKey(KeyCode.LeftControl))
					{
						component.RemoveSelectedWorker(m_Worker);
					}
					return;
				}
				if (Input.GetKey(KeyCode.LeftControl))
				{
					return;
				}
				if (component.GetSelectedWorker() != m_Worker)
				{
					component.ClearSelectedWorkers();
					component.AddSelectedWorker(m_Worker);
					return;
				}
				component.RemoveSelectedWorker(m_Worker);
			}
			UpdateSelectedColour();
		}
	}

	public override void OnDrag()
	{
		m_JustSelected = false;
	}

	public override void SetHighlight(bool Highlight)
	{
		base.SetHighlight(Highlight);
		m_Worker.m_WorkerArrow.SetHighlighted(Highlight);
	}
}
