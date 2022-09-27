using System.Collections.Generic;

public class EditArea : ObjectSelectBar
{
	public enum State
	{
		EditArea,
		DraggingAnchor,
		DraggingArea,
		SelectSign,
		Total
	}

	public State m_State;

	private BaseButtonImage m_CancelButton;

	public BaseButtonImage m_ApplyButton;

	private BaseButtonImage m_ObjectButton;

	public BaseButtonImage m_MaxButton;

	private BaseButtonImage m_FindButton;

	private BaseDropdown m_FindTypeDropdown;

	private GameStateEditArea m_Parent;

	private BaseInputField m_WidthEdit;

	private BaseInputField m_HeightEdit;

	private void Awake()
	{
		m_CancelButton = base.transform.Find("StandardCancelButton").GetComponent<BaseButtonImage>();
		m_CancelButton.SetAction(OnCancelClicked, m_CancelButton);
		m_ApplyButton = base.transform.Find("StandardAcceptButton").GetComponent<BaseButtonImage>();
		m_ApplyButton.SetAction(OnApplyClicked, m_ApplyButton);
		m_ObjectButton = base.transform.Find("ToggleButton").GetComponent<BaseButtonImage>();
		m_ObjectButton.SetAction(OnObjectClicked, m_ObjectButton);
		m_MaxButton = base.transform.Find("MaxButton").GetComponent<BaseButtonImage>();
		m_MaxButton.SetAction(OnMaxClicked, m_MaxButton);
		m_FindButton = base.transform.Find("FindButton").GetComponent<BaseButtonImage>();
		m_FindButton.SetAction(OnFindClicked, m_FindButton);
		m_FindTypeDropdown = base.transform.Find("FindTypeDropdown").GetComponent<BaseDropdown>();
		m_FindTypeDropdown.ClearOptions();
		List<string> list = new List<string>();
		for (int i = 0; i < 5; i++)
		{
			string findNameFromType = HighInstruction.GetFindNameFromType((HighInstruction.FindType)i);
			list.Add(TextManager.Instance.Get(findNameFromType));
		}
		m_FindTypeDropdown.SetOptions(list);
		m_FindTypeDropdown.SetAction(OnDropdownChanged, m_FindTypeDropdown);
		m_WidthEdit = base.transform.Find("EditDimensions/Width").GetComponent<BaseInputField>();
		m_WidthEdit.SetAction(OnWidthInputChanged, m_WidthEdit);
		m_HeightEdit = base.transform.Find("EditDimensions/Height").GetComponent<BaseInputField>();
		m_HeightEdit.SetAction(OnHeightInputChanged, m_HeightEdit);
	}

	public void SetParent(GameStateEditArea Parent)
	{
		m_Parent = Parent;
	}

	public void UpdateDimensions()
	{
		AreaIndicator areaIndicator = m_Parent.GetAreaIndicator();
		bool interactable = false;
		if ((bool)areaIndicator)
		{
			TileCoord tileCoord = areaIndicator.m_BottomRight - areaIndicator.m_TopLeft + new TileCoord(1, 1);
			m_WidthEdit.SetText(tileCoord.x.ToString());
			m_HeightEdit.SetText(tileCoord.y.ToString());
			interactable = true;
		}
		if (m_State == State.SelectSign)
		{
			interactable = false;
		}
		m_WidthEdit.SetInteractable(interactable);
		m_HeightEdit.SetInteractable(interactable);
	}

	public void UpdateIndicator()
	{
		AreaIndicator areaIndicator = m_Parent.GetAreaIndicator();
		if (m_Parent.m_Instruction != null)
		{
			HighInstruction.FindType findType = m_Parent.m_Instruction.GetFindType();
			m_FindTypeDropdown.SetStartValue((int)findType);
			if (areaIndicator != null)
			{
				areaIndicator.SetFindType(findType);
			}
		}
		UpdateIndicatorVisibility();
	}

	public void UpdateIndicatorVisibility()
	{
		AreaIndicator areaIndicator = m_Parent.GetAreaIndicator();
		if ((bool)areaIndicator)
		{
			if ((areaIndicator.m_Instruction != null && areaIndicator.m_Instruction.m_Type == HighInstruction.Type.FindNearestTile) || (m_Parent.m_Instruction != null && m_Parent.m_Instruction.m_Type == HighInstruction.Type.FindNearestTile))
			{
				m_FindTypeDropdown.SetInteractable(Interactable: true);
			}
			else
			{
				m_FindTypeDropdown.SetInteractable(Interactable: false);
			}
		}
		else
		{
			m_FindTypeDropdown.SetInteractable(Interactable: false);
		}
		UpdateDimensions();
	}

	public void DisableToggleButton()
	{
		m_ObjectButton.SetInteractable(Interactable: false);
	}

	public void SetState(State NewState)
	{
		m_State = NewState;
		switch (m_State)
		{
		case State.EditArea:
			m_ApplyButton.SetInteractable(Interactable: true);
			m_ObjectButton.SetSprite("EditArea/EditAreaObjectButton");
			m_ObjectButton.SetRolloverFromID("EditAreaObjectButton");
			m_MaxButton.SetInteractable(Interactable: true);
			UpdateIndicatorVisibility();
			break;
		case State.DraggingAnchor:
			m_ApplyButton.SetInteractable(Interactable: false);
			m_MaxButton.SetInteractable(Interactable: false);
			break;
		case State.DraggingArea:
			m_ApplyButton.SetInteractable(Interactable: false);
			m_MaxButton.SetInteractable(Interactable: false);
			break;
		case State.SelectSign:
			m_ApplyButton.SetInteractable(Interactable: true);
			m_ObjectButton.SetSprite("EditArea/EditAreaAreaButton");
			m_ObjectButton.SetRolloverFromID("EditAreaAreaButton");
			m_MaxButton.SetInteractable(Interactable: false);
			UpdateIndicatorVisibility();
			break;
		}
	}

	public void OnObjectClicked(BaseGadget NewGadget)
	{
		m_Parent.ToggleSign();
	}

	public void OnMaxClicked(BaseGadget NewGadget)
	{
		QuestManager.Instance.AddEvent(QuestEvent.Type.UseMaxArea, Bot: false, 0, null);
		m_Parent.MaxArea();
	}

	public void OnFindClicked(BaseGadget NewGadget)
	{
		m_Parent.FindClicked();
	}

	public void OnDropdownChanged(BaseGadget NewGadget)
	{
		HighInstruction.FindType value = (HighInstruction.FindType)m_FindTypeDropdown.GetValue();
		m_Parent.SetFindType(value);
	}

	public void OnApplyClicked(BaseGadget NewGadget)
	{
		GameStateManager.Instance.PopState(Immediate: true);
	}

	public void OnCancelClicked(BaseGadget NewGadget)
	{
		m_Parent.Restore();
		GameStateManager.Instance.PopState(Immediate: true);
	}

	public void OnWidthInputChanged(BaseGadget NewGadget)
	{
		int result = 1;
		int.TryParse(m_WidthEdit.GetText(), out result);
		if (result < 1)
		{
			result = 1;
			m_WidthEdit.SetText(result.ToString());
		}
		AreaIndicator areaIndicator = m_Parent.GetAreaIndicator();
		int y = (areaIndicator.m_BottomRight - areaIndicator.m_TopLeft).y;
		TileCoord anchorPosition = areaIndicator.m_TopLeft + new TileCoord(result - 1, y);
		areaIndicator.m_AnchorX = 1f;
		areaIndicator.m_AnchorY = 1f;
		m_Parent.SetAnchorPosition(anchorPosition);
		int num = (areaIndicator.m_BottomRight - areaIndicator.m_TopLeft).x + 1;
		m_WidthEdit.SetText(num.ToString());
	}

	public void OnHeightInputChanged(BaseGadget NewGadget)
	{
		int result = 1;
		int.TryParse(m_HeightEdit.GetText(), out result);
		if (result < 1)
		{
			result = 1;
			m_HeightEdit.SetText(result.ToString());
		}
		AreaIndicator areaIndicator = m_Parent.GetAreaIndicator();
		int x = (areaIndicator.m_BottomRight - areaIndicator.m_TopLeft).x;
		TileCoord anchorPosition = areaIndicator.m_TopLeft + new TileCoord(x, result - 1);
		areaIndicator.m_AnchorX = 1f;
		areaIndicator.m_AnchorY = 1f;
		m_Parent.SetAnchorPosition(anchorPosition);
		int num = (areaIndicator.m_BottomRight - areaIndicator.m_TopLeft).y + 1;
		m_HeightEdit.SetText(num.ToString());
	}
}
