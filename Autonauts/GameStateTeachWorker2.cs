using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameStateTeachWorker2 : GameStateBase
{
	public static GameStateTeachWorker2 Instance;

	[HideInInspector]
	public Worker m_CurrentTarget;

	public TeachWorkerScriptEdit m_ScriptEdit;

	private Text m_InputField;

	private WorkerScript m_NewScript;

	private bool m_Repeat;

	private int m_PreviousScriptLength;

	private int m_ActionCount;

	protected new void Awake()
	{
		Instance = this;
		base.Awake();
		m_Repeat = false;
		_ = HudManager.Instance.m_MenusRootTransform;
		m_ScriptEdit = TeachWorkerScriptEdit.Instance;
		m_ScriptEdit.SetTeaching(Teaching: true);
		m_ActionCount = 0;
		HudManager.Instance.SetHudButtonsActive(Active: false);
		HudManager.Instance.m_InventoryBar.SetActive(Active: true);
	}

	protected new void OnDestroy()
	{
		base.OnDestroy();
		HudManager.Instance.SetHudButtonsActive(Active: true);
	}

	public override void Pushed(GameStateManager.State NewState)
	{
		base.Pushed(NewState);
		m_ScriptEdit.Pushed();
		HudManager.Instance.StopPointingToTile();
	}

	public override void Popped(GameStateManager.State NewState)
	{
		base.Popped(NewState);
		m_ScriptEdit.Popped();
		HudManager.Instance.m_InventoryBar.SetActive(Active: true);
	}

	public void SetTarget(Worker Target)
	{
		CollectionManager.Instance.GetPlayers()[0].GetComponent<FarmerPlayer>().SetIsTeaching(Teaching: true, Target);
		m_CurrentTarget = Target;
		m_CurrentTarget.m_WorkerInterpreter.StopAll();
		m_ScriptEdit.SetTarget(Target);
	}

	private void AddAction(ActionInfo Info)
	{
		if ((bool)Info.m_Object)
		{
			Info.m_ObjectUID = Info.m_Object.m_UniqueID;
			Info.m_ObjectType = Info.m_Object.m_TypeIdentifier;
		}
		else
		{
			Info.m_ObjectUID = 0;
			Info.m_ObjectType = ObjectTypeList.m_Total;
		}
		switch (Info.m_Action)
		{
		case ActionType.MoveTo:
			Info.m_ObjectType = ObjectType.Plot;
			Info.m_ObjectUID = 0;
			Info.m_Object = null;
			Info.m_ActionRequirement = "";
			m_ScriptEdit.NewInstruction(HighInstruction.Type.MoveTo, Info);
			break;
		case ActionType.MoveToLessOne:
			Info.m_ObjectType = ObjectType.Plot;
			Info.m_ObjectUID = 0;
			Info.m_Object = null;
			Info.m_ActionRequirement = "";
			m_ScriptEdit.NewInstruction(HighInstruction.Type.MoveToLessOne, Info);
			break;
		case ActionType.MoveToRange:
			Info.m_ObjectType = ObjectType.Plot;
			Info.m_ObjectUID = 0;
			Info.m_Object = null;
			Info.m_ActionRequirement = "";
			m_ScriptEdit.NewInstruction(HighInstruction.Type.MoveToRange, Info);
			break;
		case ActionType.MoveForwards:
			Info.m_ObjectType = ObjectType.Plot;
			Info.m_ObjectUID = 0;
			Info.m_Object = null;
			Info.m_ActionRequirement = "";
			m_ScriptEdit.NewInstruction(HighInstruction.Type.MoveForwards, Info);
			break;
		case ActionType.MoveBackwards:
			Info.m_ObjectType = ObjectType.Plot;
			Info.m_ObjectUID = 0;
			Info.m_Object = null;
			Info.m_ActionRequirement = "";
			m_ScriptEdit.NewInstruction(HighInstruction.Type.MoveBackwards, Info);
			break;
		case ActionType.UseInHands:
			m_ScriptEdit.NewInstruction(HighInstruction.Type.UseInHands, Info);
			break;
		case ActionType.DropAll:
			m_ScriptEdit.NewInstruction(HighInstruction.Type.DropAll, Info);
			break;
		case ActionType.Pickup:
			m_ScriptEdit.NewInstruction(HighInstruction.Type.Pickup, Info);
			break;
		case ActionType.Make:
			m_ScriptEdit.NewInstruction(HighInstruction.Type.Make, Info);
			break;
		case ActionType.TakeResource:
			m_ScriptEdit.NewInstruction(HighInstruction.Type.TakeResource, Info);
			break;
		case ActionType.AddResource:
			m_ScriptEdit.NewInstruction(HighInstruction.Type.AddResource, Info);
			break;
		case ActionType.StowObject:
			m_ScriptEdit.NewInstruction(HighInstruction.Type.StowObject, Info);
			break;
		case ActionType.RecallObject:
			m_ScriptEdit.NewInstruction(HighInstruction.Type.RecallObject, Info);
			break;
		case ActionType.CycleObject:
			m_ScriptEdit.NewInstruction(HighInstruction.Type.CycleObject, Info);
			break;
		case ActionType.SwapObject:
			m_ScriptEdit.NewInstruction(HighInstruction.Type.SwapObject, Info);
			break;
		case ActionType.Recharge:
			m_ScriptEdit.NewInstruction(HighInstruction.Type.Recharge, Info);
			break;
		case ActionType.Shout:
			m_ScriptEdit.NewInstruction(HighInstruction.Type.Shout, Info);
			break;
		case ActionType.EngageObject:
			m_ScriptEdit.NewInstruction(HighInstruction.Type.EngageObject, Info);
			break;
		case ActionType.DisengageObject:
			m_ScriptEdit.NewInstruction(HighInstruction.Type.DisengageObject, Info);
			break;
		case ActionType.SetValue:
			m_ScriptEdit.NewInstruction(HighInstruction.Type.SetValue, Info);
			break;
		}
	}

	private void UpdateTeachInstructions()
	{
		List<ActionInfo> teachingActions = CollectionManager.Instance.GetPlayers()[0].GetComponent<FarmerPlayer>().m_TeachingActions;
		if (m_ActionCount != teachingActions.Count)
		{
			int num = teachingActions.Count - m_ActionCount;
			for (int i = 0; i < num; i++)
			{
				AddAction(teachingActions[m_ActionCount + i]);
			}
			m_ActionCount = teachingActions.Count;
			if (num > 0)
			{
				TutorialScriptManager.Instance.TeachingInstructionsChanged();
			}
		}
	}

	public void Close(bool StartScript)
	{
		CollectionManager.Instance.GetPlayers()[0].GetComponent<FarmerPlayer>().SetIsTeaching(Teaching: false);
		GameStateManager.Instance.SetState(GameStateManager.State.Normal);
		if (StartScript)
		{
			GameStateManager.Instance.GetCurrentState().GetComponent<GameStateNormal>().SetSelectedWorker(m_CurrentTarget, Confirmed: false, Scale: false);
		}
	}

	public void ToggleRepeat()
	{
		m_Repeat = !m_Repeat;
		UpdateTeachInstructions();
	}

	public override void UpdateState()
	{
		if (!m_EventSystem.IsUIInFocus() && !m_ScriptEdit.IsDragging())
		{
			UpdateNormalGameControl();
		}
		else
		{
			HighlightObject(null);
			HudManager.Instance.ActivateTileRollover(Activate: false, Tile.TileType.Total, default(TileCoord));
			Cursor.Instance.NoTarget();
		}
		if (!m_EventSystem.IsUIInUse() && !m_ScriptEdit.IsDragging())
		{
			CameraManager.Instance.UpdateInput();
			UpdateInventoryKeys();
		}
		UpdateTeachInstructions();
		if (MyInputManager.m_Rewired.GetButtonDown("Quit") && !m_ScriptEdit.IsDragging())
		{
			m_ScriptEdit.OnClickBack();
		}
	}
}
