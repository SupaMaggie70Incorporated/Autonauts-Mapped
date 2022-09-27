using System.Collections.Generic;
using UnityEngine;

public class EditGroup : BaseMenu
{
	private BasePanelOptions m_Panel;

	private BaseText m_Title;

	private ButtonList m_ColourButtonList;

	private BaseButtonImage m_BackButton;

	private BaseButtonImage m_StopButton;

	private BaseButtonImage m_PlayButton;

	private BaseButtonImage m_PauseButton;

	private BaseButtonImage m_GroupButton;

	private BaseInputField m_NameInputField;

	private BaseInputField m_DescriptionInputField;

	public WorkerGroup m_Group;

	private WorkerGroup m_CurrentGroup;

	private WorkerGroup m_NewGroup;

	private bool m_FirstTime;

	private int m_StartColoutIndex;

	protected new void Awake()
	{
		base.Awake();
		m_ColourButtonList = base.transform.Find("ColourButtonList").GetComponent<ButtonList>();
		m_ColourButtonList.m_CreateObjectCallback = OnCreateColourButton;
		m_ColourButtonList.m_ObjectCount = WorkerGroup.m_Colours.Length;
		m_FirstTime = true;
	}

	protected new void Start()
	{
		base.Start();
		GetButtons();
	}

	private void GetButtons()
	{
		m_Panel = base.transform.Find("BasePanelOptions").GetComponent<BasePanelOptions>();
		m_Title = base.transform.Find("BasePanelOptions/Panel/TitleStrip/Title").GetComponent<BaseText>();
		m_NameInputField = base.transform.Find("NameInputField").GetComponent<BaseInputField>();
		AddAction(m_NameInputField, OnNameInputChanged);
		m_DescriptionInputField = base.transform.Find("DescriptionInputField").GetComponent<BaseInputField>();
		AddAction(m_DescriptionInputField, OnDescriptionInputChanged);
		m_StopButton = base.transform.Find("StopButton").GetComponent<BaseButtonImage>();
		AddAction(m_StopButton, OnStopClicked);
		m_PlayButton = base.transform.Find("PlayButton").GetComponent<BaseButtonImage>();
		AddAction(m_PlayButton, OnPlayClicked);
		m_PauseButton = base.transform.Find("PauseButton").GetComponent<BaseButtonImage>();
		AddAction(m_PauseButton, OnPauseClicked);
		BaseButtonImage component = base.transform.Find("DropAllButton").GetComponent<BaseButtonImage>();
		AddAction(component, OnDropAllClicked);
		component = base.transform.Find("CallButton").GetComponent<BaseButtonImage>();
		AddAction(component, OnCallClicked);
		component = base.transform.Find("UngroupButton").GetComponent<BaseButtonImage>();
		AddAction(component, OnUngroupClicked);
		component = base.transform.Find("StandardAcceptButton").GetComponent<BaseButtonImage>();
		AddAction(component, OnAcceptClicked);
		if (base.transform.Find("GroupButton") != null)
		{
			m_GroupButton = base.transform.Find("GroupButton").GetComponent<BaseButtonImage>();
			AddAction(m_GroupButton, OnGroupClicked);
		}
	}

	public void OnCreateColourButton(GameObject NewObject, int Index)
	{
		BaseButtonImage component = NewObject.GetComponent<BaseButtonImage>();
		component.SetBackingColour(WorkerGroup.m_Colours[Index]);
		AddAction(component, OnColourClicked);
	}

	public void OnNameInputChanged(BaseGadget NewGadget)
	{
		if (m_CurrentGroup != null)
		{
			m_CurrentGroup.m_Name = m_NameInputField.GetText();
			if ((bool)m_CurrentGroup.m_WorkerGroupPanel)
			{
				m_CurrentGroup.m_WorkerGroupPanel.UpdateName();
			}
			TabWorkers.Instance.ContentsChanged();
		}
	}

	public void OnDescriptionInputChanged(BaseGadget NewGadget)
	{
		m_Group.m_Description = m_DescriptionInputField.GetText();
	}

	public void OnColourClicked(BaseGadget NewGadget)
	{
		int colour = m_ColourButtonList.m_Buttons.IndexOf(NewGadget);
		SetColour(colour);
	}

	public void OnStopClicked(BaseGadget NewGadget)
	{
		StopChildren(m_CurrentGroup);
	}

	private void StopChildren(WorkerGroup NewGroup)
	{
		foreach (int workerUID in NewGroup.m_WorkerUIDs)
		{
			Worker component = ObjectTypeList.Instance.GetObjectFromUniqueID(workerUID).GetComponent<Worker>();
			if (!component.m_BeingHeld && component.m_WorkerInterpreter.GetCurrentScript() != null && component.m_InterruptState != Farmer.State.Paused)
			{
				component.StopAllScripts();
			}
		}
		foreach (int groupUID in NewGroup.m_GroupUIDs)
		{
			StopChildren(WorkerGroupManager.Instance.GetGroupFromID(groupUID));
		}
	}

	public void OnPlayClicked(BaseGadget NewGadget)
	{
		PlayChildren(m_CurrentGroup);
	}

	private void PlayChildren(WorkerGroup NewGroup)
	{
		foreach (int workerUID in NewGroup.m_WorkerUIDs)
		{
			Worker component = ObjectTypeList.Instance.GetObjectFromUniqueID(workerUID).GetComponent<Worker>();
			if (component.m_BeingHeld)
			{
				continue;
			}
			if (component.m_WorkerInterpreter.GetCurrentScript() == null)
			{
				List<HighInstruction> list = component.m_WorkerInterpreter.m_HighInstructions.m_List;
				if (list != null)
				{
					component.NewHighScriptTaught(list);
				}
			}
			else if (component.m_InterruptState == Farmer.State.Paused)
			{
				component.TogglePauseScript();
			}
		}
		foreach (int groupUID in NewGroup.m_GroupUIDs)
		{
			PlayChildren(WorkerGroupManager.Instance.GetGroupFromID(groupUID));
		}
	}

	public void OnPauseClicked(BaseGadget NewGadget)
	{
		PauseChildren(m_CurrentGroup);
	}

	private void PauseChildren(WorkerGroup NewGroup)
	{
		foreach (int workerUID in NewGroup.m_WorkerUIDs)
		{
			Worker component = ObjectTypeList.Instance.GetObjectFromUniqueID(workerUID).GetComponent<Worker>();
			if (!component.m_BeingHeld && component.m_WorkerInterpreter.GetCurrentScript() != null && component.m_InterruptState != Farmer.State.Paused)
			{
				component.TogglePauseScript();
			}
		}
		foreach (int groupUID in NewGroup.m_GroupUIDs)
		{
			PauseChildren(WorkerGroupManager.Instance.GetGroupFromID(groupUID));
		}
	}

	public void OnDropAllClicked(BaseGadget NewGadget)
	{
		CollectionManager.Instance.GetPlayers()[0].GetComponent<FarmerPlayer>().UseWhistle(FarmerPlayer.WhistleCall.DropAll);
		DropAllChildren(m_CurrentGroup);
	}

	private void DropAllChildren(WorkerGroup NewGroup)
	{
		for (int num = NewGroup.m_WorkerUIDs.Count - 1; num >= 0; num--)
		{
			int iD = NewGroup.m_WorkerUIDs[num];
			Worker component = ObjectTypeList.Instance.GetObjectFromUniqueID(iD).GetComponent<Worker>();
			if (component.m_WorkerInterpreter.GetCurrentScript() == null)
			{
				component.m_FarmerCarry.DropAllObjects();
				component.m_FarmerInventory.DropAllObjects();
			}
		}
		foreach (int groupUID in NewGroup.m_GroupUIDs)
		{
			DropAllChildren(WorkerGroupManager.Instance.GetGroupFromID(groupUID));
		}
	}

	public void OnCallClicked(BaseGadget NewGadget)
	{
		CollectionManager.Instance.GetPlayers()[0].GetComponent<FarmerPlayer>().UseWhistle(FarmerPlayer.WhistleCall.ToMe);
		CallAllChildren(m_CurrentGroup);
	}

	private void CallAllChildren(WorkerGroup NewGroup)
	{
		List<BaseClass> players = CollectionManager.Instance.GetPlayers();
		for (int num = NewGroup.m_WorkerUIDs.Count - 1; num >= 0; num--)
		{
			int iD = NewGroup.m_WorkerUIDs[num];
			Worker component = ObjectTypeList.Instance.GetObjectFromUniqueID(iD).GetComponent<Worker>();
			if (component.m_WorkerInterpreter.GetCurrentScript() == null)
			{
				component.GoToPlayer(players[0].GetComponent<FarmerPlayer>());
			}
		}
		foreach (int groupUID in NewGroup.m_GroupUIDs)
		{
			CallAllChildren(WorkerGroupManager.Instance.GetGroupFromID(groupUID));
		}
	}

	public void OnUngroupClicked(BaseGadget NewGadget)
	{
		for (int num = m_CurrentGroup.m_GroupUIDs.Count - 1; num >= 0; num--)
		{
			int desiredID = m_CurrentGroup.m_GroupUIDs[num];
			WorkerGroup groupFromID = WorkerGroupManager.Instance.GetGroupFromID(desiredID);
			TabWorkers.Instance.SetGroupGroup(groupFromID, null);
		}
		for (int num2 = m_CurrentGroup.m_WorkerUIDs.Count - 1; num2 >= 0; num2--)
		{
			int iD = m_CurrentGroup.m_WorkerUIDs[num2];
			Worker component = ObjectTypeList.Instance.GetObjectFromUniqueID(iD).GetComponent<Worker>();
			component.GetComponent<Worker>().m_WorkerInterpreter.m_HighInstructions.ScaleAreaIndicators(Up: false);
			TabWorkers.Instance.SetWorkerGroup(component, null);
		}
		WorkerGroup currentGroup = m_CurrentGroup;
		GameStateNormal component2 = GameStateManager.Instance.GetCurrentState().GetComponent<GameStateNormal>();
		if ((bool)component2)
		{
			component2.ClearSelectedGroups();
		}
		TabWorkers.Instance.SetGroupGroup(currentGroup, null);
		currentGroup.m_GroupParentGroup = -1;
		TabWorkers.Instance.RemoveGroup(currentGroup);
	}

	public void OnGroupClicked(BaseGadget NewGadget)
	{
		GameStateNormal component = GameStateManager.Instance.GetCurrentState().GetComponent<GameStateNormal>();
		TabWorkers.Instance.NewGroup(component.m_SelectedWorkers);
	}

	public void RevertChanges()
	{
		m_NameInputField.SetText(m_NameInputField.m_StartText);
		OnNameInputChanged(m_NameInputField);
		m_DescriptionInputField.SetText(m_DescriptionInputField.m_StartText);
		OnDescriptionInputChanged(m_DescriptionInputField);
		SetColour(m_StartColoutIndex);
	}

	public override void OnBackClicked(BaseGadget NewGadget)
	{
		GameStateNormal component = GameStateManager.Instance.GetCurrentState().GetComponent<GameStateNormal>();
		if ((bool)component)
		{
			RevertChanges();
			component.ClearSelectedWorkers();
			component.ClearSelectedGroups();
		}
	}

	public void OnAcceptClicked(BaseGadget NewGadget)
	{
		GameStateNormal component = GameStateManager.Instance.GetCurrentState().GetComponent<GameStateNormal>();
		if ((bool)component)
		{
			component.ClearSelectedWorkers();
			component.ClearSelectedGroups();
		}
	}

	public void SetGroup(WorkerGroup NewGroup)
	{
		m_Group = NewGroup;
		if (!m_FirstTime)
		{
			if (NewGroup != null)
			{
				m_StartColoutIndex = NewGroup.m_ColourIndex;
			}
			UpdateGroup();
		}
	}

	private void UpdateGroup()
	{
		m_CurrentGroup = m_Group;
		if (m_CurrentGroup == null)
		{
			base.gameObject.SetActive(value: false);
			return;
		}
		base.gameObject.SetActive(value: true);
		if (!m_NameInputField.GetActive())
		{
			string text = TextManager.Instance.Get("EditGroupTitleTemp");
			text = text + ": " + m_CurrentGroup.m_WorkerUIDs.Count;
			m_Title.SetText(text);
		}
		m_NameInputField.SetStartText(m_CurrentGroup.m_Name);
		m_DescriptionInputField.SetStartText(m_CurrentGroup.m_Description);
		foreach (BaseGadget button in m_ColourButtonList.m_Buttons)
		{
			button.SetSelected(Selected: false);
		}
		SetColour(m_CurrentGroup.m_ColourIndex);
	}

	public void SetColour(int Colour)
	{
		if (Colour != -1)
		{
			m_ColourButtonList.m_Buttons[m_CurrentGroup.m_ColourIndex].SetSelected(Selected: false);
			m_CurrentGroup.SetColourIndex(Colour);
			m_ColourButtonList.m_Buttons[m_CurrentGroup.m_ColourIndex].SetSelected(Selected: true);
			m_Panel.SetTitleStripColour(WorkerGroup.m_Colours[m_CurrentGroup.m_ColourIndex]);
		}
	}

	protected new void Update()
	{
		base.Update();
		if (m_FirstTime)
		{
			m_FirstTime = false;
			UpdateGroup();
			base.gameObject.SetActive(value: false);
		}
		List<BaseClass> players = CollectionManager.Instance.GetPlayers();
		if (players != null && players.Count > 0)
		{
			players[0].GetComponent<FarmerPlayer>();
			bool interactable = true;
			m_StopButton.SetInteractable(interactable);
			m_PlayButton.SetInteractable(interactable);
		}
	}
}
