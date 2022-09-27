using System.Collections.Generic;
using UnityEngine;

public class TutorialScriptManager : MonoBehaviour
{
	public static TutorialScriptManager Instance;

	private int m_ScriptStage;

	private bool m_TeachingInstructionsChanged;

	private HighInstructionList m_CapturedInstructions;

	private int m_RepeatCount;

	private QuestEvent.Type m_LastEventType;

	private void Awake()
	{
		Instance = this;
		m_CapturedInstructions = new HighInstructionList();
		m_LastEventType = QuestEvent.Type.Total;
	}

	private List<HighInstruction> GetInstructions()
	{
		return TeachWorkerScriptEdit.Instance.m_Instructions.m_List;
	}

	private void CaptureInstructions()
	{
		HighInstructionList instructions = TeachWorkerScriptEdit.Instance.m_Instructions;
		m_CapturedInstructions.Copy(instructions);
	}

	private bool CheckChopTree(List<HighInstruction> NewInstructions)
	{
		if (NewInstructions.Count == 3 && NewInstructions[0].m_Type == HighInstruction.Type.FindNearestObject && NewInstructions[0].m_ActionInfo.m_ObjectType == ObjectType.TreePine && NewInstructions[1].m_Type == HighInstruction.Type.MoveToLessOne && NewInstructions[2].m_Type == HighInstruction.Type.UseInHands)
		{
			return true;
		}
		return false;
	}

	private bool CheckDigSoil(List<HighInstruction> NewInstructions)
	{
		if (NewInstructions.Count == 3 && NewInstructions[0].m_Type == HighInstruction.Type.FindNearestTile && NewInstructions[0].m_ActionInfo.m_ActionRequirement == "TileSoil" && NewInstructions[1].m_Type == HighInstruction.Type.MoveToLessOne && NewInstructions[2].m_Type == HighInstruction.Type.UseInHands)
		{
			return true;
		}
		return false;
	}

	private bool CheckPickupTreeSeedPart(List<HighInstruction> NewInstructions)
	{
		if (NewInstructions[0].m_Type == HighInstruction.Type.FindNearestObject && NewInstructions[0].m_ActionInfo.m_ObjectType == ObjectType.TreeSeed && NewInstructions[1].m_Type == HighInstruction.Type.MoveTo && NewInstructions[2].m_Type == HighInstruction.Type.Pickup)
		{
			return true;
		}
		return false;
	}

	private bool CheckPickupTreeSeed(List<HighInstruction> NewInstructions)
	{
		if (NewInstructions.Count == 3 && CheckPickupTreeSeedPart(NewInstructions))
		{
			return true;
		}
		return false;
	}

	private bool CheckPlantTreeSeed(List<HighInstruction> NewInstructions)
	{
		if (NewInstructions.Count == 6 && CheckPickupTreeSeedPart(NewInstructions) && NewInstructions[3].m_Type == HighInstruction.Type.FindNearestTile && NewInstructions[3].m_ActionInfo.m_ActionRequirement == "TileSoilHole" && NewInstructions[4].m_Type == HighInstruction.Type.MoveToLessOne && NewInstructions[5].m_Type == HighInstruction.Type.UseInHands)
		{
			return true;
		}
		return false;
	}

	private bool CheckTakeLog(List<HighInstruction> NewInstructions)
	{
		if (NewInstructions.Count == 2 && NewInstructions[0].m_Type == HighInstruction.Type.MoveTo && NewInstructions[0].m_ActionInfo.m_ObjectType == ObjectType.StoragePalette && NewInstructions[1].m_Type == HighInstruction.Type.TakeResource)
		{
			return true;
		}
		return false;
	}

	private bool CheckChopLog(List<HighInstruction> NewInstructions)
	{
		if (NewInstructions.Count == 4 && NewInstructions[0].m_Type == HighInstruction.Type.MoveTo && NewInstructions[0].m_ActionInfo.m_ObjectType == ObjectType.StoragePalette && NewInstructions[1].m_Type == HighInstruction.Type.TakeResource && NewInstructions[2].m_Type == HighInstruction.Type.MoveTo && NewInstructions[2].m_ActionInfo.m_ObjectType == ObjectType.ChoppingBlock && NewInstructions[3].m_Type == HighInstruction.Type.AddResource)
		{
			return true;
		}
		return false;
	}

	private bool CheckChopLogUntilFull(List<HighInstruction> NewInstructions)
	{
		if (NewInstructions.Count == 1 && NewInstructions[0].m_Type == HighInstruction.Type.Repeat && HighInstruction.GetConditionTypeFromRepeatName(NewInstructions[0].m_Argument) == HighInstruction.ConditionType.BuildingFull)
		{
			_ = NewInstructions[0].m_Children;
			if (CheckChopLog(NewInstructions[0].m_Children))
			{
				return true;
			}
		}
		return false;
	}

	private bool CheckMine(List<HighInstruction> NewInstructions)
	{
		if (NewInstructions.Count == 3 && NewInstructions[0].m_Type == HighInstruction.Type.FindNearestTile && NewInstructions[0].m_ActionInfo.m_ObjectType == ObjectType.Plot && Tile.GetTypeFromName(NewInstructions[0].m_ActionInfo.m_ActionRequirement) == Tile.TileType.StoneSoil && NewInstructions[1].m_Type == HighInstruction.Type.MoveToLessOne && NewInstructions[2].m_Type == HighInstruction.Type.UseInHands)
		{
			return true;
		}
		return false;
	}

	private bool CheckMineForever(List<HighInstruction> NewInstructions)
	{
		if (NewInstructions.Count == 1 && NewInstructions[0].m_Type == HighInstruction.Type.Repeat && HighInstruction.GetConditionTypeFromRepeatName(NewInstructions[0].m_Argument) == HighInstruction.ConditionType.Forever)
		{
			_ = NewInstructions[0].m_Children;
			if (CheckMine(NewInstructions[0].m_Children))
			{
				return true;
			}
		}
		return false;
	}

	private bool CheckMineUntilHandsEmpty(List<HighInstruction> NewInstructions)
	{
		if (NewInstructions.Count == 1 && NewInstructions[0].m_Type == HighInstruction.Type.Repeat && HighInstruction.GetConditionTypeFromRepeatName(NewInstructions[0].m_Argument) == HighInstruction.ConditionType.HandsEmpty)
		{
			_ = NewInstructions[0].m_Children;
			if (CheckMine(NewInstructions[0].m_Children))
			{
				return true;
			}
		}
		return false;
	}

	private bool CheckMineUntilHandsEmptyPickup(List<HighInstruction> NewInstructions)
	{
		if (NewInstructions.Count == 4 && NewInstructions[0].m_Type == HighInstruction.Type.Repeat && HighInstruction.GetConditionTypeFromRepeatName(NewInstructions[0].m_Argument) == HighInstruction.ConditionType.HandsEmpty)
		{
			_ = NewInstructions[0].m_Children;
			if (CheckMine(NewInstructions[0].m_Children) && NewInstructions[1].m_Type == HighInstruction.Type.FindNearestObject && NewInstructions[1].m_ActionInfo.m_ObjectType == ObjectType.ToolPickStone && NewInstructions[2].m_Type == HighInstruction.Type.MoveTo && NewInstructions[3].m_Type == HighInstruction.Type.Pickup)
			{
				return true;
			}
		}
		return false;
	}

	public void TeachingInstructionsChanged()
	{
		m_TeachingInstructionsChanged = true;
	}

	public void StartQuest()
	{
		m_ScriptStage = 0;
		m_CapturedInstructions = new HighInstructionList();
		m_RepeatCount = 0;
		m_LastEventType = QuestEvent.Type.Total;
	}

	public void NewInstructions()
	{
		CaptureInstructions();
	}

	private void GoBackAStep()
	{
		TutorialPanelController.Instance.GoBackAStep();
	}

	private void TestInstructionsChanged()
	{
		if (TutorialPanelController.Instance == null || !TutorialPanelController.Instance.GetActive())
		{
			return;
		}
		bool flag = true;
		Quest quest = TutorialPanelController.Instance.m_Quest;
		if (quest == null)
		{
			return;
		}
		QuestEvent firstAvailableEvent = quest.GetFirstAvailableEvent();
		if (firstAvailableEvent == null)
		{
			return;
		}
		if (quest.m_ID == Quest.ID.TutorialTeaching)
		{
			if (firstAvailableEvent.m_Type == QuestEvent.Type.ChopTree)
			{
				flag = CheckChopTree(GetInstructions());
			}
			else if (firstAvailableEvent.m_Type == QuestEvent.Type.ClickRepeat)
			{
				if (!CheckChopTree(GetInstructions()))
				{
					flag = CheckTutorialTeachingScript();
				}
				if (!flag && m_LastEventType != QuestEvent.Type.ClickRepeat)
				{
					GoBackAStep();
				}
			}
			else if (firstAvailableEvent.m_Type == QuestEvent.Type.ClickPlay)
			{
				flag = CheckTutorialTeachingScript();
			}
		}
		else if (quest.m_ID == Quest.ID.TutorialTeaching2)
		{
			if (firstAvailableEvent.m_Type == QuestEvent.Type.DigSoil || firstAvailableEvent.m_Type == QuestEvent.Type.UseMaxArea || firstAvailableEvent.m_Type == QuestEvent.Type.EndEditSearchArea)
			{
				flag = CheckDigSoil(GetInstructions());
			}
			else if (firstAvailableEvent.m_Type == QuestEvent.Type.EditSearchArea)
			{
				flag = CheckDigSoil(GetInstructions());
				if (!flag && m_LastEventType != QuestEvent.Type.EditSearchArea)
				{
					GoBackAStep();
				}
			}
			else if (firstAvailableEvent.m_Type == QuestEvent.Type.ClickRepeat)
			{
				if (!CheckDigSoil(GetInstructions()))
				{
					flag = CheckTutorialTeaching2Script();
				}
			}
			else if (firstAvailableEvent.m_Type == QuestEvent.Type.ClickPlay || firstAvailableEvent.m_Type == QuestEvent.Type.GiveBotAnything)
			{
				flag = CheckTutorialTeaching2Script();
			}
		}
		else if (quest.m_ID == Quest.ID.TutorialTeaching3)
		{
			if (firstAvailableEvent.m_Type == QuestEvent.Type.Pickup)
			{
				flag = CheckPickupTreeSeed(GetInstructions());
				Debug.Log(flag);
			}
			else if (firstAvailableEvent.m_Type == QuestEvent.Type.PlantTreeSeed)
			{
				if (!CheckPickupTreeSeed(GetInstructions()))
				{
					flag = CheckPlantTreeSeed(GetInstructions());
				}
				if (!flag && m_LastEventType != QuestEvent.Type.PlantTreeSeed)
				{
					GoBackAStep();
				}
			}
			else if (firstAvailableEvent.m_Type == QuestEvent.Type.ClickRepeat)
			{
				if (!CheckPlantTreeSeed(GetInstructions()))
				{
					flag = CheckTutorialTeaching3Script();
				}
				if (!flag && m_LastEventType != QuestEvent.Type.ClickRepeat)
				{
					GoBackAStep();
				}
			}
			else if (firstAvailableEvent.m_Type == QuestEvent.Type.ClickPlay)
			{
				flag = CheckTutorialTeaching3Script();
			}
		}
		else if (quest.m_ID == Quest.ID.TutorialScripting2)
		{
			if (firstAvailableEvent.m_Type == QuestEvent.Type.Take)
			{
				flag = CheckTakeLog(GetInstructions());
			}
			else if (firstAvailableEvent.m_Type == QuestEvent.Type.MakePlank)
			{
				if (!CheckTakeLog(GetInstructions()))
				{
					flag = CheckChopLog(GetInstructions());
				}
				if (!flag && m_LastEventType != QuestEvent.Type.MakePlank)
				{
					GoBackAStep();
				}
			}
			else if (firstAvailableEvent.m_Type == QuestEvent.Type.ClickRepeat)
			{
				if (!CheckChopLog(GetInstructions()))
				{
					flag = CheckTutorialScripting2Script();
				}
				if (!flag && m_LastEventType != QuestEvent.Type.ClickRepeat)
				{
					GoBackAStep();
				}
			}
			else if (firstAvailableEvent.m_Type == QuestEvent.Type.ClickPlay)
			{
				flag = CheckTutorialScripting2Script();
			}
		}
		else if (quest.m_ID == Quest.ID.TutorialScripting3)
		{
			if (firstAvailableEvent.m_Type == QuestEvent.Type.UntilBuildingFullChosen)
			{
				if (!CheckChopLog(GetInstructions()))
				{
					flag = CheckChopLogUntilFull(GetInstructions());
				}
			}
			else if (firstAvailableEvent.m_Type == QuestEvent.Type.UseObjectArea)
			{
				flag = CheckChopLogUntilFull(GetInstructions());
			}
			else if (firstAvailableEvent.m_Type == QuestEvent.Type.ObjectAreaSelect)
			{
				flag = CheckChopLogUntilFull(GetInstructions());
			}
			else if (firstAvailableEvent.m_Type == QuestEvent.Type.ClickRepeat)
			{
				if (!CheckChopLogUntilFull(GetInstructions()))
				{
					flag = CheckTutorialScripting3Script();
				}
			}
			else if (firstAvailableEvent.m_Type == QuestEvent.Type.ClickPlay)
			{
				flag = CheckTutorialScripting3Script();
			}
		}
		else if (quest.m_ID == Quest.ID.TutorialScripting5)
		{
			if (firstAvailableEvent.m_Type == QuestEvent.Type.MineStoneDeposits)
			{
				flag = CheckMine(GetInstructions());
			}
			else if (firstAvailableEvent.m_Type == QuestEvent.Type.ClickRepeat && m_RepeatCount == 0)
			{
				if (!CheckMine(GetInstructions()))
				{
					flag = CheckMineForever(GetInstructions());
				}
				if (!flag && m_LastEventType != QuestEvent.Type.ClickRepeat)
				{
					GoBackAStep();
				}
				if (flag)
				{
					m_RepeatCount++;
				}
			}
			else if (firstAvailableEvent.m_Type == QuestEvent.Type.UntilHandsEmptyChosen)
			{
				if (!CheckMineForever(GetInstructions()))
				{
					flag = CheckMineUntilHandsEmpty(GetInstructions());
				}
			}
			else if (firstAvailableEvent.m_Type == QuestEvent.Type.Pickup)
			{
				if (!CheckMineUntilHandsEmpty(GetInstructions()))
				{
					flag = CheckMineUntilHandsEmptyPickup(GetInstructions());
				}
			}
			else if (firstAvailableEvent.m_Type == QuestEvent.Type.ClickRepeat && m_RepeatCount == 1)
			{
				if (!CheckMineUntilHandsEmptyPickup(GetInstructions()))
				{
					flag = CheckTutorialScripting5Script();
				}
				if (!flag && m_LastEventType != QuestEvent.Type.ClickRepeat)
				{
					GoBackAStep();
				}
			}
			else if (firstAvailableEvent.m_Type == QuestEvent.Type.ClickPlay)
			{
				flag = CheckTutorialScripting5Script();
			}
		}
		if (!flag)
		{
			if ((bool)TeachWorkerScriptEdit.Instance)
			{
				TeachWorkerScriptEdit.Instance.SetInstructions(m_CapturedInstructions);
			}
			if ((bool)TutorialPanelController.Instance)
			{
				TutorialPanelController.Instance.DoError();
			}
		}
		else
		{
			CaptureInstructions();
			m_ScriptStage++;
		}
		m_LastEventType = firstAvailableEvent.m_Type;
	}

	private bool CheckForever(List<HighInstruction> NewInstructions)
	{
		if (NewInstructions.Count == 1 && NewInstructions[0].m_Type == HighInstruction.Type.Repeat && HighInstruction.GetConditionTypeFromRepeatName(NewInstructions[0].m_Argument) == HighInstruction.ConditionType.Forever)
		{
			return true;
		}
		return false;
	}

	private bool CheckTutorialTeachingScript()
	{
		List<HighInstruction> instructions = GetInstructions();
		if (CheckForever(instructions) && CheckChopTree(instructions[0].m_Children))
		{
			return true;
		}
		return false;
	}

	private bool CheckTutorialTeaching2Script()
	{
		List<HighInstruction> instructions = GetInstructions();
		if (CheckForever(instructions) && CheckDigSoil(instructions[0].m_Children))
		{
			return true;
		}
		return false;
	}

	private bool CheckTutorialTeaching3Script()
	{
		List<HighInstruction> instructions = GetInstructions();
		if (CheckForever(instructions) && CheckPlantTreeSeed(instructions[0].m_Children))
		{
			return true;
		}
		return false;
	}

	private bool CheckTutorialScripting2Script()
	{
		List<HighInstruction> instructions = GetInstructions();
		if (CheckForever(instructions) && CheckChopLog(instructions[0].m_Children))
		{
			return true;
		}
		return false;
	}

	private bool CheckTutorialScripting3Script()
	{
		List<HighInstruction> instructions = GetInstructions();
		if (CheckForever(instructions) && CheckChopLogUntilFull(instructions[0].m_Children))
		{
			return true;
		}
		return false;
	}

	private bool CheckTutorialScripting5Script()
	{
		List<HighInstruction> instructions = GetInstructions();
		if (CheckForever(instructions) && CheckMineUntilHandsEmptyPickup(instructions[0].m_Children))
		{
			return true;
		}
		return false;
	}

	public void EndTeaching()
	{
		if (!TutorialPanelController.Instance.GetActive())
		{
			return;
		}
		bool flag = true;
		Quest quest = TutorialPanelController.Instance.m_Quest;
		if (quest.m_ID == Quest.ID.TutorialTeaching)
		{
			flag = CheckTutorialTeachingScript();
		}
		else if (quest.m_ID == Quest.ID.TutorialTeaching2)
		{
			flag = CheckTutorialTeaching2Script();
			if (flag)
			{
				QuestEvent firstAvailableEvent = quest.GetFirstAvailableEvent();
				if (firstAvailableEvent != null && firstAvailableEvent.m_Type == QuestEvent.Type.GiveBotAnything)
				{
					QuestManager.Instance.AddEvent(QuestEvent.Type.GiveBotAnything, Bot: false, ObjectType.ToolShovelStone, null);
					QuestManager.Instance.AddEvent(QuestEvent.Type.ClickPlay, Bot: false, null, null);
					return;
				}
			}
		}
		else if (quest.m_ID == Quest.ID.TutorialTeaching3)
		{
			flag = CheckTutorialTeaching3Script();
		}
		else if (quest.m_ID == Quest.ID.TutorialScripting2)
		{
			flag = CheckTutorialScripting2Script();
		}
		else if (quest.m_ID == Quest.ID.TutorialScripting3)
		{
			flag = CheckTutorialScripting3Script();
		}
		else
		{
			if (quest.m_ID != Quest.ID.TutorialScripting5)
			{
				return;
			}
			flag = CheckTutorialScripting5Script();
		}
		if (!flag || !quest.m_Complete)
		{
			TutorialPanelController.Instance.ResetQuest();
			TeachWorkerScriptEdit.Instance.Clear();
			GameStateManager.Instance.GetCurrentState().GetComponent<GameStateNormal>().ClearSelectedWorkers();
		}
	}

	private void Update()
	{
		if (!(CollectionManager.Instance == null) && m_TeachingInstructionsChanged)
		{
			List<BaseClass> players = CollectionManager.Instance.GetPlayers();
			if ((bool)players[0] && (players[0].GetComponent<Farmer>().m_State == Farmer.State.None || players[0].GetComponent<Farmer>().m_State == Farmer.State.JumpTurf))
			{
				m_TeachingInstructionsChanged = false;
				TestInstructionsChanged();
			}
		}
	}
}
