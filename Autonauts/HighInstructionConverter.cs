using System.Collections.Generic;
using UnityEngine;

public class HighInstructionConverter : MonoBehaviour
{
	private static int m_LabelCounter;

	private static bool m_MoveToCoordsSet;

	private static bool m_ObjectSet;

	private static int m_RepeatTimesStack;

	private static List<string> m_RepeatEndLabels;

	private static string GetLabel()
	{
		string result = "Label" + m_LabelCounter;
		m_LabelCounter++;
		return result;
	}

	private static string GetRepeatTimesVariable()
	{
		return "Global" + (16 - m_RepeatTimesStack);
	}

	private static void AddForever(HighInstruction NewInstruction, WorkerScript NewScript)
	{
		string label = GetLabel();
		NewScript.AddInstruction(WorkerInstruction.Instruction.Label, label);
		ConvertInstructionList(NewInstruction.m_Children, NewScript);
		m_MoveToCoordsSet = false;
		m_ObjectSet = false;
		NewScript.AddInstruction(WorkerInstruction.Instruction.JumpTo, label);
	}

	private static void AddCondition(HighInstruction NewInstruction, WorkerScript NewScript, HighInstruction.ConditionType RepeatType)
	{
		switch (RepeatType)
		{
		case HighInstruction.ConditionType.HandsFull:
			NewScript.AddInstruction(WorkerInstruction.Instruction.CheckHandsFull, "Global9");
			break;
		case HighInstruction.ConditionType.HandsNotFull:
			NewScript.AddInstruction(WorkerInstruction.Instruction.CheckHandsNotFull, "Global9");
			break;
		case HighInstruction.ConditionType.HandsEmpty:
			NewScript.AddInstruction(WorkerInstruction.Instruction.CheckHandsEmpty, "Global9");
			break;
		case HighInstruction.ConditionType.HandsNotEmpty:
			NewScript.AddInstruction(WorkerInstruction.Instruction.CheckHandsNotEmpty, "Global9");
			break;
		case HighInstruction.ConditionType.InventoryFull:
			NewScript.AddInstruction(WorkerInstruction.Instruction.CheckInventoryFull, "Global9");
			break;
		case HighInstruction.ConditionType.InventoryNotFull:
			NewScript.AddInstruction(WorkerInstruction.Instruction.CheckInventoryNotFull, "Global9");
			break;
		case HighInstruction.ConditionType.InventoryEmpty:
			NewScript.AddInstruction(WorkerInstruction.Instruction.CheckInventoryEmpty, "Global9");
			break;
		case HighInstruction.ConditionType.InventoryNotEmpty:
			NewScript.AddInstruction(WorkerInstruction.Instruction.CheckInventoryNotEmpty, "Global9");
			break;
		case HighInstruction.ConditionType.BuildingFull:
			NewScript.AddInstruction(WorkerInstruction.Instruction.CheckBuildingFull, NewInstruction.m_ActionInfo.m_ObjectUID.ToString(), "Global9");
			break;
		case HighInstruction.ConditionType.BuildingNotFull:
			NewScript.AddInstruction(WorkerInstruction.Instruction.CheckBuildingNotFull, NewInstruction.m_ActionInfo.m_ObjectUID.ToString(), "Global9");
			break;
		case HighInstruction.ConditionType.BuildingEmpty:
			NewScript.AddInstruction(WorkerInstruction.Instruction.CheckBuildingEmpty, NewInstruction.m_ActionInfo.m_ObjectUID.ToString(), "Global9");
			break;
		case HighInstruction.ConditionType.BuildingNotEmpty:
			NewScript.AddInstruction(WorkerInstruction.Instruction.CheckBuildingNotEmpty, NewInstruction.m_ActionInfo.m_ObjectUID.ToString(), "Global9");
			break;
		case HighInstruction.ConditionType.HeldObjectFull:
			NewScript.AddInstruction(WorkerInstruction.Instruction.CheckHeldObjectFull, "Global9");
			break;
		case HighInstruction.ConditionType.HeldObjectNotFull:
			NewScript.AddInstruction(WorkerInstruction.Instruction.CheckHeldObjectNotFull, "Global9");
			break;
		case HighInstruction.ConditionType.HeldObjectEmpty:
			NewScript.AddInstruction(WorkerInstruction.Instruction.CheckHeldObjectEmpty, "Global9");
			break;
		case HighInstruction.ConditionType.HeldObjectNotEmpty:
			NewScript.AddInstruction(WorkerInstruction.Instruction.CheckHeldObjectNotEmpty, "Global9");
			break;
		case HighInstruction.ConditionType.Times:
		{
			NewScript.AddInstruction(WorkerInstruction.Instruction.AddVariable, GetRepeatTimesVariable(), "-1", GetRepeatTimesVariable());
			string label = GetLabel();
			NewScript.AddInstruction(WorkerInstruction.Instruction.SetVariable, "Global9", "0");
			NewScript.AddInstruction(WorkerInstruction.Instruction.JumpIfNotZero, GetRepeatTimesVariable(), label);
			NewScript.AddInstruction(WorkerInstruction.Instruction.SetVariable, "Global9", "1");
			NewScript.AddInstruction(WorkerInstruction.Instruction.Label, label);
			break;
		}
		case HighInstruction.ConditionType.Forever:
			NewScript.AddInstruction(WorkerInstruction.Instruction.SetVariable, "Global9", "0");
			break;
		case HighInstruction.ConditionType.Hear:
			NewScript.AddInstruction(WorkerInstruction.Instruction.CheckHear, NewInstruction.m_ActionInfo.m_Value, "Global9");
			break;
		}
	}

	private static void AddRepeat(HighInstruction NewInstruction, WorkerScript NewScript)
	{
		string label = GetLabel();
		m_RepeatEndLabels.Add(label);
		if (NewInstruction.m_ActionInfo.m_Value2 != "")
		{
			NewScript.AddInstruction(WorkerInstruction.Instruction.PushFailJump, label);
		}
		HighInstruction.ConditionType conditionTypeFromRepeatName = HighInstruction.GetConditionTypeFromRepeatName(NewInstruction.m_Argument);
		if (conditionTypeFromRepeatName == HighInstruction.ConditionType.Times)
		{
			m_RepeatTimesStack++;
			int num = 1;
			if (NewInstruction.m_ActionInfo.m_Value != "")
			{
				int result = 0;
				int.TryParse(NewInstruction.m_ActionInfo.m_Value, out result);
				num += result;
			}
			string variable = num.ToString();
			NewScript.AddInstruction(WorkerInstruction.Instruction.SetVariable, GetRepeatTimesVariable(), variable);
		}
		string label2 = GetLabel();
		NewScript.AddInstruction(WorkerInstruction.Instruction.Label, label2);
		AddCondition(NewInstruction, NewScript, conditionTypeFromRepeatName);
		NewScript.AddInstruction(WorkerInstruction.Instruction.JumpIfNotZero, "Global9", label);
		ConvertInstructionList(NewInstruction.m_Children, NewScript);
		m_MoveToCoordsSet = false;
		m_ObjectSet = false;
		NewScript.AddInstruction(WorkerInstruction.Instruction.JumpTo, label2);
		NewScript.AddInstruction(WorkerInstruction.Instruction.Label, label);
		if (conditionTypeFromRepeatName == HighInstruction.ConditionType.Times)
		{
			m_RepeatTimesStack--;
		}
		if (NewInstruction.m_ActionInfo.m_Value2 != "")
		{
			NewScript.AddInstruction(WorkerInstruction.Instruction.PopFailJump);
		}
		m_RepeatEndLabels.RemoveAt(m_RepeatEndLabels.Count - 1);
	}

	private static void AddIf(HighInstruction NewInstruction, WorkerScript NewScript)
	{
		string label = GetLabel();
		HighInstruction.ConditionType conditionTypeFromIfName = HighInstruction.GetConditionTypeFromIfName(NewInstruction.m_Argument);
		AddCondition(NewInstruction, NewScript, conditionTypeFromIfName);
		NewScript.AddInstruction(WorkerInstruction.Instruction.JumpIfZero, "Global9", label);
		ConvertInstructionList(NewInstruction.m_Children, NewScript);
		m_MoveToCoordsSet = false;
		m_ObjectSet = false;
		NewScript.AddInstruction(WorkerInstruction.Instruction.Label, label);
	}

	private static void AddIfElse(HighInstruction NewInstruction, WorkerScript NewScript)
	{
		string label = GetLabel();
		string label2 = GetLabel();
		HighInstruction.ConditionType conditionTypeFromIfName = HighInstruction.GetConditionTypeFromIfName(NewInstruction.m_Argument);
		AddCondition(NewInstruction, NewScript, conditionTypeFromIfName);
		NewScript.AddInstruction(WorkerInstruction.Instruction.JumpIfZero, "Global9", label);
		ConvertInstructionList(NewInstruction.m_Children, NewScript);
		NewScript.AddInstruction(WorkerInstruction.Instruction.JumpTo, label2);
		NewScript.AddInstruction(WorkerInstruction.Instruction.Label, label);
		ConvertInstructionList(NewInstruction.m_Children2, NewScript);
		m_MoveToCoordsSet = false;
		m_ObjectSet = false;
		NewScript.AddInstruction(WorkerInstruction.Instruction.Label, label2);
	}

	private static void AddExitRepeat(HighInstruction NewInstruction, WorkerScript NewScript)
	{
		if (m_RepeatEndLabels.Count != 0)
		{
			NewScript.AddInstruction(WorkerInstruction.Instruction.JumpTo, m_RepeatEndLabels[m_RepeatEndLabels.Count - 1]);
		}
	}

	private static void AddMoveTo(HighInstruction NewInstruction, WorkerScript NewScript)
	{
		ActionInfo actionInfo = NewInstruction.m_ActionInfo;
		WorkerInstruction.Instruction instruction = WorkerInstruction.Instruction.MoveTo;
		if (NewInstruction.m_Type == HighInstruction.Type.MoveToLessOne)
		{
			instruction = WorkerInstruction.Instruction.MoveToLessOne;
		}
		else if (NewInstruction.m_Type == HighInstruction.Type.MoveToRange)
		{
			instruction = WorkerInstruction.Instruction.MoveToRange;
		}
		if (NewInstruction.IsObjectSelectAvailable())
		{
			m_MoveToCoordsSet = false;
		}
		if (NewInstruction.m_Type == HighInstruction.Type.MoveTo && NewInstruction.m_ActionInfo.m_ObjectType == ObjectType.Plot && NewInstruction.m_ActionInfo.m_ActionRequirement == "")
		{
			m_MoveToCoordsSet = false;
		}
		if (m_MoveToCoordsSet)
		{
			NewScript.AddInstruction(instruction, "Global0", "Global1", "Global2", "Global5");
			m_MoveToCoordsSet = false;
			return;
		}
		int actionType;
		if (actionInfo.m_ObjectType == ObjectType.Plot)
		{
			WorkerInstruction.Instruction newInstruction = instruction;
			string variable = actionInfo.m_Position.x.ToString();
			string variable2 = actionInfo.m_Position.y.ToString();
			actionType = (int)actionInfo.m_ActionType;
			NewScript.AddInstruction(newInstruction, variable, variable2, "0", actionType.ToString());
			return;
		}
		TileCoord tileCoord = actionInfo.m_Position;
		BaseClass objectFromUniqueID = ObjectTypeList.Instance.GetObjectFromUniqueID(actionInfo.m_ObjectUID, ErrorCheck: false);
		if ((bool)objectFromUniqueID && (bool)objectFromUniqueID.GetComponent<Building>())
		{
			tileCoord = objectFromUniqueID.GetComponent<Building>().GetAccessPosition();
		}
		WorkerInstruction.Instruction newInstruction2 = instruction;
		string variable3 = tileCoord.x.ToString();
		string variable4 = tileCoord.y.ToString();
		string variable5 = actionInfo.m_ObjectUID.ToString();
		actionType = (int)actionInfo.m_ActionType;
		NewScript.AddInstruction(newInstruction2, variable3, variable4, variable5, actionType.ToString());
		NewScript.AddInstruction(WorkerInstruction.Instruction.SetVariable, "Global0", actionInfo.m_Position.x.ToString());
		NewScript.AddInstruction(WorkerInstruction.Instruction.SetVariable, "Global1", actionInfo.m_Position.y.ToString());
		NewScript.AddInstruction(WorkerInstruction.Instruction.SetVariable, "Global2", actionInfo.m_ObjectUID.ToString());
	}

	private static void AddMoveForwards(HighInstruction NewInstruction, WorkerScript NewScript)
	{
		NewScript.AddInstruction(WorkerInstruction.Instruction.MoveForwards);
	}

	private static void AddMoveBackwards(HighInstruction NewInstruction, WorkerScript NewScript)
	{
		NewScript.AddInstruction(WorkerInstruction.Instruction.MoveBackwards);
	}

	private static void AddTurnAt(HighInstruction NewInstruction, WorkerScript NewScript)
	{
		ActionInfo actionInfo = NewInstruction.m_ActionInfo;
		NewScript.AddInstruction(WorkerInstruction.Instruction.TurnAt, actionInfo.m_ObjectUID.ToString());
	}

	private static void AddStopAt(HighInstruction NewInstruction, WorkerScript NewScript)
	{
		ActionInfo actionInfo = NewInstruction.m_ActionInfo;
		NewScript.AddInstruction(WorkerInstruction.Instruction.StopAt, actionInfo.m_ObjectUID.ToString());
	}

	private static void AddWait(HighInstruction NewInstruction, WorkerScript NewScript)
	{
		float result = 0f;
		float.TryParse(NewInstruction.m_ActionInfo.m_Value, out result);
		NewScript.AddInstruction(WorkerInstruction.Instruction.Wait, (result * 60f).ToString());
	}

	private static void AddDropAll(HighInstruction NewInstruction, WorkerScript NewScript)
	{
		NewScript.AddInstruction(WorkerInstruction.Instruction.DropAll);
	}

	private static void AddPickup(HighInstruction NewInstruction, WorkerScript NewScript)
	{
		m_MoveToCoordsSet = false;
		m_ObjectSet = false;
		NewScript.AddInstruction(WorkerInstruction.Instruction.Pickup, "Global2");
	}

	private static void AddTakeResource(HighInstruction NewInstruction, WorkerScript NewScript)
	{
		ActionInfo actionInfo = NewInstruction.m_ActionInfo;
		int actionType = (int)actionInfo.m_ActionType;
		NewScript.AddInstruction(WorkerInstruction.Instruction.SetVariable, "Global5", actionType.ToString());
		if (m_ObjectSet)
		{
			NewScript.AddInstruction(WorkerInstruction.Instruction.TakeResource, "Global2");
			m_ObjectSet = false;
		}
		else
		{
			NewScript.AddInstruction(WorkerInstruction.Instruction.TakeResource, actionInfo.m_ObjectUID.ToString());
		}
	}

	private static void AddAddResource(HighInstruction NewInstruction, WorkerScript NewScript)
	{
		ActionInfo actionInfo = NewInstruction.m_ActionInfo;
		int actionType = (int)actionInfo.m_ActionType;
		NewScript.AddInstruction(WorkerInstruction.Instruction.SetVariable, "Global5", actionType.ToString());
		if (m_ObjectSet)
		{
			NewScript.AddInstruction(WorkerInstruction.Instruction.AddResource, "Global2");
			m_ObjectSet = false;
		}
		else
		{
			NewScript.AddInstruction(WorkerInstruction.Instruction.AddResource, actionInfo.m_ObjectUID.ToString());
		}
	}

	private static void AddStowObject(HighInstruction NewInstruction, WorkerScript NewScript)
	{
		NewScript.AddInstruction(WorkerInstruction.Instruction.StowObject);
	}

	private static void AddRecallObject(HighInstruction NewInstruction, WorkerScript NewScript)
	{
		ActionInfo actionInfo = NewInstruction.m_ActionInfo;
		NewScript.AddInstruction(WorkerInstruction.Instruction.RecallObject, actionInfo.m_Value);
	}

	private static void AddCycleObject(HighInstruction NewInstruction, WorkerScript NewScript)
	{
		ActionInfo actionInfo = NewInstruction.m_ActionInfo;
		NewScript.AddInstruction(WorkerInstruction.Instruction.CycleObject, actionInfo.m_Value);
	}

	private static void AddSwapObject(HighInstruction NewInstruction, WorkerScript NewScript)
	{
		NewScript.AddInstruction(WorkerInstruction.Instruction.SwapObject);
	}

	private static void AddUseInHands(HighInstruction NewInstruction, WorkerScript NewScript)
	{
		ActionInfo actionInfo = NewInstruction.m_ActionInfo;
		if (m_ObjectSet)
		{
			m_ObjectSet = false;
		}
		else if (actionInfo.m_ObjectUID == 0)
		{
			NewScript.AddInstruction(WorkerInstruction.Instruction.SetVariable, "Global2", "0");
		}
		m_MoveToCoordsSet = false;
		int actionType = (int)actionInfo.m_ActionType;
		NewScript.AddInstruction(WorkerInstruction.Instruction.SetVariable, "Global5", actionType.ToString());
		NewScript.AddInstruction(WorkerInstruction.Instruction.SetVariable, "Global7", actionInfo.m_ActionRequirement);
		NewScript.AddInstruction(WorkerInstruction.Instruction.SetVariable, "Global8", ObjectTypeList.Instance.GetSaveNameFromIdentifier(actionInfo.m_ActionObjectType));
		if (actionInfo.m_ObjectType == ObjectType.Nothing)
		{
			NewScript.AddInstruction(WorkerInstruction.Instruction.UseInHands, actionInfo.m_Position.x.ToString(), actionInfo.m_Position.y.ToString(), ObjectTypeList.m_NothingObject.m_UniqueID.ToString(), actionInfo.m_Value2);
		}
		else
		{
			NewScript.AddInstruction(WorkerInstruction.Instruction.UseInHands, "Global0", "Global1", "Global2", actionInfo.m_Value2);
		}
	}

	private static void AddMake(HighInstruction NewInstruction, WorkerScript NewScript)
	{
		ActionInfo actionInfo = NewInstruction.m_ActionInfo;
		string saveNameFromIdentifier = ObjectTypeList.Instance.GetSaveNameFromIdentifier(actionInfo.m_ObjectType);
		NewScript.AddInstruction(WorkerInstruction.Instruction.Make, saveNameFromIdentifier, actionInfo.m_Value);
	}

	private static void AddRecharge(HighInstruction NewInstruction, WorkerScript NewScript)
	{
		NewScript.AddInstruction(WorkerInstruction.Instruction.Recharge, "Global2");
	}

	private static void AddShout(HighInstruction NewInstruction, WorkerScript NewScript)
	{
		NewScript.AddInstruction(WorkerInstruction.Instruction.Shout, NewInstruction.m_ActionInfo.m_Value);
	}

	private static void AddEngageObject(HighInstruction NewInstruction, WorkerScript NewScript)
	{
		ActionInfo actionInfo = NewInstruction.m_ActionInfo;
		int actionType = (int)actionInfo.m_ActionType;
		NewScript.AddInstruction(WorkerInstruction.Instruction.SetVariable, "Global5", actionType.ToString());
		if (m_ObjectSet)
		{
			NewScript.AddInstruction(WorkerInstruction.Instruction.EngageObject, "Global2");
			m_ObjectSet = false;
		}
		else
		{
			NewScript.AddInstruction(WorkerInstruction.Instruction.EngageObject, actionInfo.m_ObjectUID.ToString());
		}
	}

	private static void AddDisengageObject(HighInstruction NewInstruction, WorkerScript NewScript)
	{
		ActionInfo actionInfo = NewInstruction.m_ActionInfo;
		NewScript.AddInstruction(WorkerInstruction.Instruction.DisengageObject, actionInfo.m_Position.x.ToString(), actionInfo.m_Position.y.ToString());
	}

	private static void AddSetValue(HighInstruction NewInstruction, WorkerScript NewScript)
	{
		ActionInfo actionInfo = NewInstruction.m_ActionInfo;
		NewScript.AddInstruction(WorkerInstruction.Instruction.SetValue, actionInfo.m_ObjectUID.ToString(), actionInfo.m_Value, actionInfo.m_Value2);
	}

	private static void SetupSearchArea(HighInstruction NewInstruction, WorkerScript NewScript)
	{
		NewScript.AddInstruction(WorkerInstruction.Instruction.SetVariable, "Global3", NewInstruction.m_Argument);
	}

	private static void AddFindNearestObject(HighInstruction NewInstruction, WorkerScript NewScript)
	{
		ActionInfo actionInfo = NewInstruction.m_ActionInfo;
		string saveNameFromIdentifier = ObjectTypeList.Instance.GetSaveNameFromIdentifier(actionInfo.m_ObjectType);
		SetupSearchArea(NewInstruction, NewScript);
		NewScript.AddInstruction(WorkerInstruction.Instruction.SetVariable, "Global6", actionInfo.m_ObjectUID.ToString());
		NewScript.AddInstruction(WorkerInstruction.Instruction.SetVariable, "Global4", actionInfo.m_Value);
		int actionType = (int)actionInfo.m_ActionType;
		NewScript.AddInstruction(WorkerInstruction.Instruction.SetVariable, "Global5", actionType.ToString());
		NewScript.AddInstruction(WorkerInstruction.Instruction.SetVariable, "Global7", actionInfo.m_ActionRequirement);
		NewScript.AddInstruction(WorkerInstruction.Instruction.FindNearestObject, saveNameFromIdentifier, "Global0", "Global1", "Global2");
		m_MoveToCoordsSet = true;
		m_ObjectSet = true;
	}

	private static void AddFindNearestTile(HighInstruction NewInstruction, WorkerScript NewScript)
	{
		ActionInfo actionInfo = NewInstruction.m_ActionInfo;
		SetupSearchArea(NewInstruction, NewScript);
		NewScript.AddInstruction(WorkerInstruction.Instruction.SetVariable, "Global6", actionInfo.m_ObjectUID.ToString());
		NewScript.AddInstruction(WorkerInstruction.Instruction.SetVariable, "Global4", actionInfo.m_Value);
		int actionType = (int)actionInfo.m_ActionType;
		NewScript.AddInstruction(WorkerInstruction.Instruction.SetVariable, "Global5", actionType.ToString());
		string actionRequirement = actionInfo.m_ActionRequirement;
		string variable = actionInfo.m_Action.ToString();
		NewScript.AddInstruction(WorkerInstruction.Instruction.FindNearestTile, actionRequirement, "Global0", "Global1", variable);
		NewScript.AddInstruction(WorkerInstruction.Instruction.SetVariable, "Global2", actionInfo.m_ObjectUID.ToString());
		m_MoveToCoordsSet = true;
	}

	private static void ConvertInstructionList(List<HighInstruction> Instructions, WorkerScript NewScript)
	{
		foreach (HighInstruction Instruction in Instructions)
		{
			Instruction.m_ScriptLineNumber = NewScript.m_Instructions.Count;
			switch (Instruction.m_Type)
			{
			case HighInstruction.Type.Forever:
				AddForever(Instruction, NewScript);
				break;
			case HighInstruction.Type.Repeat:
				AddRepeat(Instruction, NewScript);
				break;
			case HighInstruction.Type.If:
				AddIf(Instruction, NewScript);
				break;
			case HighInstruction.Type.IfElse:
				AddIfElse(Instruction, NewScript);
				break;
			case HighInstruction.Type.ExitRepeat:
				AddExitRepeat(Instruction, NewScript);
				break;
			case HighInstruction.Type.MoveTo:
			case HighInstruction.Type.MoveToLessOne:
			case HighInstruction.Type.MoveToRange:
				AddMoveTo(Instruction, NewScript);
				break;
			case HighInstruction.Type.MoveForwards:
				AddMoveForwards(Instruction, NewScript);
				break;
			case HighInstruction.Type.MoveBackwards:
				AddMoveBackwards(Instruction, NewScript);
				break;
			case HighInstruction.Type.TurnAt:
				AddTurnAt(Instruction, NewScript);
				break;
			case HighInstruction.Type.StopAt:
				AddStopAt(Instruction, NewScript);
				break;
			case HighInstruction.Type.Wait:
				AddWait(Instruction, NewScript);
				break;
			case HighInstruction.Type.DropAll:
				AddDropAll(Instruction, NewScript);
				break;
			case HighInstruction.Type.Pickup:
				AddPickup(Instruction, NewScript);
				break;
			case HighInstruction.Type.TakeResource:
				AddTakeResource(Instruction, NewScript);
				break;
			case HighInstruction.Type.AddResource:
				AddAddResource(Instruction, NewScript);
				break;
			case HighInstruction.Type.StowObject:
				AddStowObject(Instruction, NewScript);
				break;
			case HighInstruction.Type.RecallObject:
				AddRecallObject(Instruction, NewScript);
				break;
			case HighInstruction.Type.CycleObject:
				AddCycleObject(Instruction, NewScript);
				break;
			case HighInstruction.Type.SwapObject:
				AddSwapObject(Instruction, NewScript);
				break;
			case HighInstruction.Type.UseInHands:
				AddUseInHands(Instruction, NewScript);
				break;
			case HighInstruction.Type.Make:
				AddMake(Instruction, NewScript);
				break;
			case HighInstruction.Type.Recharge:
				AddRecharge(Instruction, NewScript);
				break;
			case HighInstruction.Type.Shout:
				AddShout(Instruction, NewScript);
				break;
			case HighInstruction.Type.EngageObject:
				AddEngageObject(Instruction, NewScript);
				break;
			case HighInstruction.Type.DisengageObject:
				AddDisengageObject(Instruction, NewScript);
				break;
			case HighInstruction.Type.SetValue:
				AddSetValue(Instruction, NewScript);
				break;
			case HighInstruction.Type.FindNearestTile:
				AddFindNearestTile(Instruction, NewScript);
				break;
			case HighInstruction.Type.FindNearestObject:
				AddFindNearestObject(Instruction, NewScript);
				break;
			}
		}
	}

	public static WorkerScript ConvertHighInstructions(List<HighInstruction> Instructions, Worker TargetWorker)
	{
		m_LabelCounter = 0;
		m_MoveToCoordsSet = false;
		m_ObjectSet = false;
		m_RepeatTimesStack = 0;
		m_RepeatEndLabels = new List<string>();
		WorkerScript workerScript = new WorkerScript();
		workerScript.Start();
		workerScript.m_Name = "New Script";
		ConvertInstructionList(Instructions, workerScript);
		return workerScript;
	}
}
