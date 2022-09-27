using System.Collections.Generic;
using UnityEngine;

public class GameStateNormal : GameStateBase
{
	private TeachWorkerScriptEdit m_ScriptEdit;

	[HideInInspector]
	public List<Worker> m_SelectedWorkers;

	public List<WorkerGroup> m_SelectedGroups;

	private Worker m_PushedWorker;

	private WorkerGroup m_PushedGroup;

	protected new void Awake()
	{
		base.Awake();
		m_ScriptEdit = TeachWorkerScriptEdit.Instance;
		m_SelectedWorkers = new List<Worker>();
		m_SelectedGroups = new List<WorkerGroup>();
		if ((bool)Cursor.Instance)
		{
			Cursor.Instance.SetUsable(Usable: true);
		}
		HudManager.Instance.EnablePauseButton(Enable: true);
	}

	public override void ShutDown()
	{
		HudManager.Instance.EnablePauseButton(Enable: false);
		ClearSelectedWorkers();
		ClearSelectedGroups();
		base.ShutDown();
	}

	public override void Pushed(GameStateManager.State NewState)
	{
		base.Pushed(NewState);
		if (NewState != GameStateManager.State.Drag)
		{
			m_ScriptEdit.Pushed();
			if (m_SelectedWorkers.Count == 1)
			{
				m_PushedWorker = m_SelectedWorkers[0];
			}
			else
			{
				m_PushedWorker = null;
			}
			if (m_SelectedGroups.Count == 1)
			{
				m_PushedGroup = m_SelectedGroups[0];
			}
			else
			{
				m_PushedGroup = null;
			}
			ClearSelectedWorkers();
			ClearSelectedGroups();
			HudManager.Instance.EnablePauseButton(Enable: false);
		}
	}

	public override void Popped(GameStateManager.State NewState)
	{
		base.Popped(NewState);
		if (NewState != GameStateManager.State.Drag)
		{
			if ((bool)m_PushedWorker)
			{
				SetSelectedWorker(m_PushedWorker);
			}
			if (m_PushedGroup != null)
			{
				SetSelectedGroup(m_PushedGroup);
			}
			m_ScriptEdit.Popped();
			HudManager.Instance.EnablePauseButton(Enable: true);
		}
	}

	private void UpdateBrainWithSelectedWorkers(bool Scale = true)
	{
		if (CollectionManager.Instance == null)
		{
			return;
		}
		List<BaseClass> players = CollectionManager.Instance.GetPlayers();
		if (players != null && players.Count > 0)
		{
			players[0].GetComponent<FarmerPlayer>();
			if (m_SelectedWorkers.Count == 1 && (HudManager.Instance.m_CurrentEditGroup == null || HudManager.Instance.m_CurrentEditGroup.m_Group == null))
			{
				m_ScriptEdit.SetTarget(m_SelectedWorkers[0]);
				m_ScriptEdit.SetTeaching(Teaching: false);
			}
			else
			{
				m_ScriptEdit.SetTarget(null);
				QuestManager.Instance.AddEvent(QuestEvent.Type.CloseBrain, Bot: false, null, null);
			}
		}
	}

	private void UpdateTempGroup()
	{
		WorkerGroup tempGroup = WorkerGroupManager.Instance.m_TempGroup;
		if (m_SelectedWorkers.Count > 1)
		{
			List<Worker> list = new List<Worker>();
			foreach (Worker selectedWorker in m_SelectedWorkers)
			{
				list.Add(selectedWorker);
			}
			tempGroup.ClearTemp();
			ClearSelectedWorkers();
			foreach (Worker item in list)
			{
				tempGroup.AddWorker(item, Temp: true);
			}
			SetSelectedGroup(tempGroup);
		}
		else
		{
			HudManager.Instance.SetGroup(null);
			if (m_SelectedGroups.Count == 1 && m_SelectedGroups[0] == tempGroup)
			{
				ClearSelectedGroups();
			}
		}
	}

	public void AddSelectedWorker(Worker NewWorker, bool Confirmed = true, bool Scale = true, bool ViaGroup = false)
	{
		if (!m_SelectedWorkers.Contains(NewWorker))
		{
			NewWorker.Selected(Select: true);
			TabWorkers.Instance.WorkerSelected(NewWorker, Selected: true);
			m_SelectedWorkers.Add(NewWorker);
			if (Confirmed || !Scale)
			{
				UpdateBrainWithSelectedWorkers(Scale);
			}
			if (!ViaGroup)
			{
				UpdateTempGroup();
			}
		}
	}

	public void RemoveSelectedWorker(Worker NewWorker)
	{
		if (m_SelectedWorkers.Contains(NewWorker))
		{
			NewWorker.Selected(Select: false);
			TabWorkers.Instance.WorkerSelected(NewWorker, Selected: false);
			m_SelectedWorkers.Remove(NewWorker);
			if (m_SelectedGroups.Count == 0 || (m_SelectedGroups.Count == 1 && m_SelectedGroups[0] == WorkerGroupManager.Instance.m_TempGroup))
			{
				UpdateTempGroup();
			}
			UpdateBrainWithSelectedWorkers();
		}
		else if (m_PushedWorker == NewWorker)
		{
			m_PushedWorker = null;
		}
	}

	public void SetSelectedWorker(Worker NewWorker, bool Confirmed = true, bool Scale = true)
	{
		ClearSelectedWorkers(UpdateBrain: false);
		AddSelectedWorker(NewWorker, Confirmed, Scale);
		if (Confirmed)
		{
			AudioManager.Instance.StartEvent("WorkerSelected", NewWorker);
		}
	}

	public Worker GetSelectedWorker()
	{
		if (m_SelectedWorkers.Count != 1)
		{
			return null;
		}
		return m_SelectedWorkers[0];
	}

	public void ClearSelectedWorkers(bool UpdateBrain = true)
	{
		foreach (Worker selectedWorker in m_SelectedWorkers)
		{
			selectedWorker.GetComponent<Worker>().m_WorkerInterpreter.m_HighInstructions.ScaleAreaIndicators(Up: false);
			selectedWorker.Selected(Select: false);
			TabWorkers.Instance.WorkerSelected(selectedWorker, Selected: false);
		}
		m_SelectedWorkers.Clear();
		if (UpdateBrain)
		{
			UpdateBrainWithSelectedWorkers();
		}
		UpdateTempGroup();
	}

	public void AddSelectedGroup(WorkerGroup NewGroup, bool Confirmed = true, bool Scale = true)
	{
		ClearSelectedWorkers();
		if (m_SelectedGroups.Contains(NewGroup))
		{
			return;
		}
		TabWorkers.Instance.GroupSelected(NewGroup, Selected: true);
		if (m_SelectedGroups.Count == 1 && m_SelectedGroups[0] == WorkerGroupManager.Instance.m_TempGroup)
		{
			ClearSelectedGroups(UpdateBrain: false);
		}
		m_SelectedGroups.Add(NewGroup);
		if (Confirmed || !Scale)
		{
			UpdateBrainWithSelectedWorkers(Scale);
		}
		if (m_SelectedGroups.Count == 1)
		{
			HudManager.Instance.SetGroup(NewGroup);
			if ((bool)NewGroup.m_WorkerGroupPanel)
			{
				NewGroup.m_WorkerGroupPanel.Select(Selected: true);
			}
			SelectAllWorkersInChildGroups(NewGroup);
		}
	}

	private void SelectAllWorkersInChildGroups(WorkerGroup NewGroup)
	{
		foreach (int workerUID in NewGroup.m_WorkerUIDs)
		{
			BaseClass objectFromUniqueID = ObjectTypeList.Instance.GetObjectFromUniqueID(workerUID);
			AddSelectedWorker(objectFromUniqueID.GetComponent<Worker>(), Confirmed: true, Scale: true, ViaGroup: true);
			objectFromUniqueID.GetComponent<Worker>().m_WorkerInterpreter.m_HighInstructions.ScaleAreaIndicators(Up: true);
		}
		foreach (int groupUID in NewGroup.m_GroupUIDs)
		{
			SelectAllWorkersInChildGroups(WorkerGroupManager.Instance.GetGroupFromID(groupUID));
		}
	}

	public void RemoveSelectedGroup(WorkerGroup NewGroup)
	{
		if (m_SelectedGroups.Contains(NewGroup))
		{
			if ((bool)NewGroup.m_WorkerGroupPanel)
			{
				NewGroup.m_WorkerGroupPanel.Select(Selected: false);
			}
			foreach (int workerUID in NewGroup.m_WorkerUIDs)
			{
				ObjectTypeList.Instance.GetObjectFromUniqueID(workerUID).GetComponent<Worker>().m_WorkerInterpreter.m_HighInstructions.ScaleAreaIndicators(Up: false);
			}
			ClearSelectedWorkers();
			TabWorkers.Instance.GroupSelected(NewGroup, Selected: false);
			m_SelectedGroups.Remove(NewGroup);
			HudManager.Instance.SetGroup(null);
			UpdateBrainWithSelectedWorkers();
		}
		else if (m_PushedGroup == NewGroup)
		{
			m_PushedGroup = null;
		}
	}

	public void SetSelectedGroup(WorkerGroup NewGroup, bool Confirmed = true, bool Scale = true)
	{
		ClearSelectedGroups(UpdateBrain: false);
		AddSelectedGroup(NewGroup, Confirmed, Scale);
	}

	public WorkerGroup GetSelectedGroup()
	{
		if (m_SelectedGroups.Count != 1)
		{
			return null;
		}
		return m_SelectedGroups[0];
	}

	public void ClearSelectedGroups(bool UpdateBrain = true)
	{
		foreach (WorkerGroup selectedGroup in m_SelectedGroups)
		{
			ClearWorkersFromSelectedGroups(selectedGroup);
			TabWorkers.Instance.GroupSelected(selectedGroup, Selected: false);
		}
		m_SelectedGroups.Clear();
		HudManager.Instance.SetGroup(null);
		if (UpdateBrain)
		{
			UpdateBrainWithSelectedWorkers();
		}
		UpdateTempGroup();
	}

	private void ClearWorkersFromSelectedGroups(WorkerGroup NewGroup)
	{
		foreach (int workerUID in NewGroup.m_WorkerUIDs)
		{
			ObjectTypeList.Instance.GetObjectFromUniqueID(workerUID).GetComponent<Worker>().m_WorkerInterpreter.m_HighInstructions.ScaleAreaIndicators(Up: false);
		}
		foreach (int groupUID in NewGroup.m_GroupUIDs)
		{
			ClearWorkersFromSelectedGroups(WorkerGroupManager.Instance.GetGroupFromID(groupUID));
		}
	}

	public override void UpdateState()
	{
		List<BaseClass> players = CollectionManager.Instance.GetPlayers();
		if (!m_EventSystem.IsUIInFocus() && !m_EventSystem.IsHover())
		{
			UpdateNormalGameControl();
		}
		else
		{
			HighlightObject(null);
			HudManager.Instance.ActivateTileRollover(Activate: false, Tile.TileType.Total, default(TileCoord));
			Cursor.Instance.NoTarget();
		}
		if (!m_EventSystem.IsUIInUse())
		{
			if (!Input.GetMouseButton(0))
			{
				if (!Input.GetKey(KeyCode.LeftShift) && MyInputManager.m_Rewired.GetButtonDown("Autopedia") && ModeButton.Get(ModeButton.Type.Autopedia).m_Show)
				{
					AudioManager.Instance.StartEvent("UIAcademySelected");
					m_ScriptEdit.SetActive(Active: false);
					GameStateManager.Instance.SetState(GameStateManager.State.Autopedia);
					return;
				}
				if (MyInputManager.m_Rewired.GetButtonDown("Edit") && ModeButton.Get(ModeButton.Type.BuildingPalette).m_Show)
				{
					AudioManager.Instance.StartEvent("UIEditModeSelected");
					HighlightObject(null);
					m_ScriptEdit.SetActive(Active: false);
					GameStateManager.Instance.SetState(GameStateManager.State.Edit);
					return;
				}
				if (MyInputManager.m_Rewired.GetButtonDown("Planning") && ModeButton.Get(ModeButton.Type.BuildingPalette).m_Show)
				{
					AudioManager.Instance.StartEvent("UIEditModeSelected");
					HighlightObject(null);
					m_ScriptEdit.SetActive(Active: false);
					GameStateManager.Instance.SetState(GameStateManager.State.Edit);
					GameStateManager.Instance.PushState(GameStateManager.State.Planning);
					return;
				}
				if (MyInputManager.m_Rewired.GetButtonDown("EditMove") && ModeButton.Get(ModeButton.Type.BuildingPalette).m_Show)
				{
					AudioManager.Instance.StartEvent("UIEditModeSelected");
					HighlightObject(null);
					m_ScriptEdit.SetActive(Active: false);
					GameStateManager.Instance.SetState(GameStateManager.State.Edit);
					GameStateEdit.Instance.SetState(GameStateEdit.State.Move);
					return;
				}
				if (MyInputManager.m_Rewired.GetButtonDown("EditDelete") && ModeButton.Get(ModeButton.Type.BuildingPalette).m_Show)
				{
					AudioManager.Instance.StartEvent("UIEditModeSelected");
					HighlightObject(null);
					m_ScriptEdit.SetActive(Active: false);
					GameStateManager.Instance.SetState(GameStateManager.State.Edit);
					GameStateEdit.Instance.SetState(GameStateEdit.State.Delete);
					return;
				}
				if (MyInputManager.m_Rewired.GetButtonDown("SelectBuilding") && ModeButton.Get(ModeButton.Type.BuildingPalette).m_Show)
				{
					AudioManager.Instance.StartEvent("UIEditModeSelected");
					HighlightObject(null);
					m_ScriptEdit.SetActive(Active: false);
					GameStateManager.Instance.SetState(GameStateManager.State.Edit);
					GameStateEdit.Instance.SetState(GameStateEdit.State.Select);
					return;
				}
				if (MyInputManager.m_Rewired.GetButtonDown("Whistle") && players[0].GetComponent<FarmerPlayer>().CanWhistle())
				{
					CameraManager.Instance.SetState(CameraManager.State.Normal);
					ClearSelectedWorkers();
					ClearSelectedGroups();
					players[0].GetComponent<FarmerPlayer>().UseWhistle(FarmerPlayer.WhistleCall.Select);
					m_ScriptEdit.SetActive(Active: false);
					if (CollectionManager.Instance.GetCollection("Farmer").Count > 1)
					{
						players[0].GetComponent<FarmerPlayer>().Whistle(Start: true);
						GameStateManager.Instance.SetState(GameStateManager.State.SelectWorker);
					}
					else
					{
						players[0].GetComponent<FarmerPlayer>().Whistle(Start: true, 0.5f);
					}
					QuestManager.Instance.AddEvent(QuestEvent.Type.UseWhistle, Bot: false, 0, players[0]);
					return;
				}
				if (MyInputManager.m_Rewired.GetButtonDown("Inventory"))
				{
					AudioManager.Instance.StartEvent("UIOptionSelected");
					GameStateManager.Instance.PushState(GameStateManager.State.Inventory);
					GameStateManager.Instance.GetCurrentState().GetComponent<GameStateInventory>().SetInfo(players[0].GetComponent<FarmerPlayer>(), null);
					return;
				}
				if (GameOptionsManager.Instance.m_Options.m_GameMode == GameOptions.GameMode.ModeCreative && MyInputManager.m_Rewired.GetButtonDown("CreativeTools") && players[0].GetComponent<FarmerPlayer>().m_State == Farmer.State.None)
				{
					AudioManager.Instance.StartEvent("UIOptionSelected");
					GameStateManager.Instance.PushState(GameStateManager.State.CreativeTools);
					return;
				}
			}
			CheatManager.Instance.UpdateNormal();
		}
		if (!m_EventSystem.IsUIInUse())
		{
			CameraManager.Instance.UpdateInput();
			UpdateInventoryKeys();
			if (MyInputManager.m_Rewired.GetButtonDown("ToggleFreeCam"))
			{
				AudioManager.Instance.StartEvent("UIOptionSelected");
				HighlightObject(null);
				Cursor.Instance.NoTarget();
				GameStateManager.Instance.PushState(GameStateManager.State.FreeCam);
				return;
			}
		}
		if (MyInputManager.m_Rewired.GetButtonDown("Quit"))
		{
			if (m_ScriptEdit.gameObject.activeSelf)
			{
				ClearSelectedWorkers();
			}
			else
			{
				if (!HudManager.Instance.m_CurrentEditGroup || !HudManager.Instance.m_CurrentEditGroup.gameObject.activeSelf)
				{
					GameStateManager.Instance.PushState(GameStateManager.State.Paused);
					return;
				}
				HudManager.Instance.m_CurrentEditGroup.OnBackClicked(null);
			}
		}
		if (MyInputManager.m_Rewired.GetButtonDown("QuickSave"))
		{
			SaveLoadManager.Instance.QuickSave();
		}
		if (CheatManager.Instance.m_CheatsEnabled && MyInputManager.m_Rewired.GetButtonDown("BeginSequence"))
		{
			FarmerPlayer component = players[0].GetComponent<FarmerPlayer>();
			if ((bool)component.m_EngagedObject && component.m_EngagedObject.m_TypeIdentifier == ObjectType.Train)
			{
				component.m_EngagedObject.GetComponent<Minecart>().StartMoving(Force: true);
				HighlightObject(null);
				Cursor.Instance.NoTarget();
				GameStateManager.Instance.PushState(GameStateManager.State.FreeCam);
				GameStateManager.Instance.PushState(GameStateManager.State.PlayCameraSequence);
			}
		}
	}

	public void CheckTargetPickedUp(Worker TestWorker)
	{
		if (m_SelectedWorkers.Contains(TestWorker))
		{
			RemoveSelectedWorker(TestWorker);
		}
	}
}
