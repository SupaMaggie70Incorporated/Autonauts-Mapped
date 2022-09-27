using UnityEngine;

public class Planning : BaseMenu
{
	private PlanningArea m_SelectedArea;

	private BaseToggle m_ShowAreasToggle;

	private BaseButton m_DeleteButton;

	private BaseInputField m_NameInputField;

	private ButtonList m_ColourButtonList;

	private StandardButtonImage m_NewButton;

	private StandardButtonImage m_SelectButton;

	private BaseInputField m_WidthEdit;

	private BaseInputField m_HeightEdit;

	protected new void Awake()
	{
		base.Awake();
		m_ColourButtonList = base.transform.Find("AreaPanel/ColourButtonList").GetComponent<ButtonList>();
		m_ColourButtonList.m_CreateObjectCallback = OnCreateColourButton;
		m_ColourButtonList.m_ObjectCount = WorkerGroup.m_Colours.Length;
	}

	protected new void Start()
	{
		base.Start();
		CheckGadgets();
		SetSelectedArea(null);
	}

	private void CheckGadgets()
	{
		if (!(m_NameInputField != null))
		{
			StandardButtonImage component = base.transform.Find("EditButton/StandardButtonImage").GetComponent<StandardButtonImage>();
			AddAction(component, OnEditClicked);
			m_ShowAreasToggle = base.transform.Find("ShowAreas").GetComponent<BaseToggle>();
			m_ShowAreasToggle.SetStartOn(PlanningManager.Instance.m_ShowAreas);
			AddAction(m_ShowAreasToggle, OnShowAreasClicked);
			m_NewButton = base.transform.Find("ToolsPanel/NewButton/StandardButtonImage").GetComponent<StandardButtonImage>();
			AddAction(m_NewButton, OnNewClicked);
			m_SelectButton = base.transform.Find("ToolsPanel/SelectButton/StandardButtonImage").GetComponent<StandardButtonImage>();
			AddAction(m_SelectButton, OnSelectClicked);
			BaseToggle component2 = base.transform.Find("ToolsPanel/ShowNamesToggle").GetComponent<BaseToggle>();
			component2.SetStartOn(PlanningManager.Instance.m_ShowNames);
			AddAction(component2, OnShowNamesClicked);
			BaseToggle component3 = base.transform.Find("ToolsPanel/ShowDimensionsToggle").GetComponent<BaseToggle>();
			component3.SetStartOn(PlanningManager.Instance.m_ShowDimensions);
			AddAction(component3, OnShowDimensionsClicked);
			m_NameInputField = base.transform.Find("AreaPanel/NameInputField").GetComponent<BaseInputField>();
			AddAction(m_NameInputField, OnNameInputChanged);
			m_DeleteButton = base.transform.Find("AreaPanel/DeleteButton/StandardButtonImage").GetComponent<BaseButton>();
			AddAction(m_DeleteButton, OnDeleteClicked);
			m_WidthEdit = base.transform.Find("AreaPanel/EditDimensions/Width").GetComponent<BaseInputField>();
			AddAction(m_WidthEdit, OnWidthInputChanged);
			m_HeightEdit = base.transform.Find("AreaPanel/EditDimensions/Height").GetComponent<BaseInputField>();
			AddAction(m_HeightEdit, OnHeightInputChanged);
		}
	}

	private void OnDestroy()
	{
	}

	public void SetNewSelected(bool Selected)
	{
		m_SelectButton.SetSelected(Selected: false);
		m_NewButton.SetSelected(Selected);
	}

	public void SetSelectSelected(bool Selected)
	{
		m_NewButton.SetSelected(Selected: false);
		m_SelectButton.SetSelected(Selected);
	}

	public void OnCreateColourButton(GameObject NewObject, int Index)
	{
		BaseButtonImage component = NewObject.GetComponent<BaseButtonImage>();
		component.SetBackingColour(WorkerGroup.m_Colours[Index]);
		AddAction(component, OnColourClicked);
	}

	public void UpdateArea()
	{
		if (m_SelectedArea != null)
		{
			TileCoord tileCoord = m_SelectedArea.m_AreaIndicator.m_BottomRight - m_SelectedArea.m_AreaIndicator.m_TopLeft + new TileCoord(1, 1);
			m_WidthEdit.SetText(tileCoord.x.ToString());
			m_HeightEdit.SetText(tileCoord.y.ToString());
		}
	}

	public void SetSelectedArea(PlanningArea NewArea)
	{
		CheckGadgets();
		m_SelectedArea = NewArea;
		bool areaInteractable = false;
		if (m_SelectedArea != null)
		{
			areaInteractable = true;
			m_NameInputField.SetStartText(m_SelectedArea.m_Name);
			SetColour(m_SelectedArea.m_Colour);
			UpdateArea();
		}
		SetAreaInteractable(areaInteractable);
	}

	public void SetAreaInteractable(bool Interactable)
	{
		m_NameInputField.SetInteractable(Interactable);
		foreach (BaseGadget button in m_ColourButtonList.m_Buttons)
		{
			button.SetInteractable(Interactable);
		}
		m_DeleteButton.SetInteractable(Interactable);
		m_WidthEdit.SetInteractable(Interactable);
		m_HeightEdit.SetInteractable(Interactable);
	}

	public void SetColour(int Colour)
	{
		m_ColourButtonList.m_Buttons[m_SelectedArea.m_Colour].SetSelected(Selected: false);
		m_SelectedArea.SetColour(Colour);
		m_ColourButtonList.m_Buttons[m_SelectedArea.m_Colour].SetSelected(Selected: true);
	}

	public override void OnBackClicked(BaseGadget NewGadget)
	{
		base.OnBackClicked(NewGadget);
		GameStateManager.Instance.PopState();
	}

	public void OnShowAreasClicked(BaseGadget NewGadget)
	{
		PlanningManager.Instance.SetShowAreas(m_ShowAreasToggle.GetOn());
	}

	public void OnNameInputChanged(BaseGadget NewGadget)
	{
		if (m_SelectedArea != null)
		{
			m_SelectedArea.SetName(m_NameInputField.GetText());
		}
	}

	public void OnColourClicked(BaseGadget NewGadget)
	{
		if (m_SelectedArea != null)
		{
			int colour = m_ColourButtonList.m_Buttons.IndexOf(NewGadget);
			SetColour(colour);
		}
	}

	public void OnDeleteClicked(BaseGadget NewGadget)
	{
		if (m_SelectedArea != null)
		{
			GameStatePlanning.Instance.DeleteSelectedArea();
		}
	}

	public void OnNewClicked(BaseGadget NewGadget)
	{
		GameStatePlanning.Instance.SetState(GameStatePlanning.State.New);
	}

	public void OnSelectClicked(BaseGadget NewGadget)
	{
		GameStatePlanning.Instance.SetState(GameStatePlanning.State.Select);
	}

	public void OnEditClicked(BaseGadget NewGadget)
	{
		GameStateManager.Instance.PopState();
	}

	public void OnShowNamesClicked(BaseGadget NewGadget)
	{
		PlanningManager.Instance.SetShowNames(NewGadget.GetComponent<BaseToggle>().GetOn());
	}

	public void OnShowDimensionsClicked(BaseGadget NewGadget)
	{
		PlanningManager.Instance.SetShowDimensions(NewGadget.GetComponent<BaseToggle>().GetOn());
	}

	public void OnWidthInputChanged(BaseGadget NewGadget)
	{
		if (m_SelectedArea != null)
		{
			int result = 1;
			int.TryParse(m_WidthEdit.GetText(), out result);
			if (result < 1)
			{
				result = 1;
				m_WidthEdit.SetText(result.ToString());
			}
			int y = (m_SelectedArea.m_AreaIndicator.m_BottomRight - m_SelectedArea.m_AreaIndicator.m_TopLeft).y;
			TileCoord bottomRight = m_SelectedArea.m_AreaIndicator.m_TopLeft + new TileCoord(result - 1, y);
			if (bottomRight.x >= TileManager.Instance.m_TilesWide)
			{
				bottomRight.x = TileManager.Instance.m_TilesWide - 1;
			}
			m_SelectedArea.SetCoords(m_SelectedArea.m_AreaIndicator.m_TopLeft, bottomRight);
			int num = (m_SelectedArea.m_AreaIndicator.m_BottomRight - m_SelectedArea.m_AreaIndicator.m_TopLeft).x + 1;
			m_WidthEdit.SetText(num.ToString());
		}
	}

	public void OnHeightInputChanged(BaseGadget NewGadget)
	{
		if (m_SelectedArea != null)
		{
			int result = 1;
			int.TryParse(m_HeightEdit.GetText(), out result);
			if (result < 1)
			{
				result = 1;
				m_HeightEdit.SetText(result.ToString());
			}
			int x = (m_SelectedArea.m_AreaIndicator.m_BottomRight - m_SelectedArea.m_AreaIndicator.m_TopLeft).x;
			TileCoord bottomRight = m_SelectedArea.m_AreaIndicator.m_TopLeft + new TileCoord(x, result - 1);
			if (bottomRight.y >= TileManager.Instance.m_TilesHigh)
			{
				bottomRight.y = TileManager.Instance.m_TilesHigh - 1;
			}
			m_SelectedArea.SetCoords(m_SelectedArea.m_AreaIndicator.m_TopLeft, bottomRight);
			int num = (m_SelectedArea.m_AreaIndicator.m_BottomRight - m_SelectedArea.m_AreaIndicator.m_TopLeft).y + 1;
			m_HeightEdit.SetText(num.ToString());
		}
	}

	public void UpdateShowToggle()
	{
		m_ShowAreasToggle.SetOn(PlanningManager.Instance.m_ShowAreas);
	}
}
