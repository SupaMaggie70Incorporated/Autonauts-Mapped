using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;

public class WorkerInterpreter : MonoBehaviour
{
	private delegate bool GetObjects(WorkerScriptLocal Script, int Index, TileCoord TopLeft, TileCoord BottomRight, HighInstruction.FindType NewFindType);

	private static string m_Interpreter = "Default";

	private static int m_Version = 0;

	private WorkerScriptLocal m_CurrentScript;

	public static int m_MaxGlobalVariables = 16;

	public string[] m_GlobalVariables;

	private bool m_ActionActive;

	private bool m_ActionSuccess;

	private bool m_WaitingForNonActive;

	[HideInInspector]
	public bool m_InstructionFailed;

	[HideInInspector]
	public bool m_InstructionProcessing;

	private bool m_InstructionProcessed;

	private Worker m_Worker;

	public bool m_Paused;

	private Dictionary<string, int> m_Labels;

	[HideInInspector]
	public HighInstructionList m_HighInstructions;

	private List<string> m_FailJumpStack;

	private int m_MoveAttempts;

	private bool m_CheckRequestPause;

	private AFO m_ReusableActionFromObject;

	private bool m_FullSearch;

	private bool m_BestSearch;

	public void Restart()
	{
		m_Paused = false;
		m_ActionActive = false;
		m_WaitingForNonActive = false;
		m_MoveAttempts = 0;
		m_Labels = new Dictionary<string, int>();
		m_FailJumpStack = new List<string>();
		m_ReusableActionFromObject = new AFO();
		CheckRunning();
	}

	private void Awake()
	{
		m_CurrentScript = null;
		m_HighInstructions = new HighInstructionList();
		m_Worker = GetComponent<Worker>();
		m_GlobalVariables = new string[m_MaxGlobalVariables];
		for (int i = 0; i < m_MaxGlobalVariables; i++)
		{
			m_GlobalVariables[i] = "";
		}
	}

	private void OnDestroy()
	{
		m_HighInstructions.ShowAreaIndicators(Show: false);
	}

	public void RestartLastInstruction()
	{
		m_WaitingForNonActive = false;
	}

	public WorkerScriptLocal GetCurrentScript()
	{
		return m_CurrentScript;
	}

	public WorkerInstruction GetWorkerInstruction()
	{
		if (m_CurrentScript == null)
		{
			return default(WorkerInstruction);
		}
		int currentInstruction = m_CurrentScript.m_CurrentInstruction;
		if (currentInstruction >= m_CurrentScript.m_Script.m_Instructions.Count)
		{
			return default(WorkerInstruction);
		}
		return m_CurrentScript.m_Script.m_Instructions[currentInstruction];
	}

	public BaseClass GetCurrentInstructionReference()
	{
		if (m_CurrentScript == null)
		{
			return null;
		}
		int currentInstruction = m_CurrentScript.m_CurrentInstruction;
		if (currentInstruction >= m_CurrentScript.m_Script.m_Instructions.Count)
		{
			return null;
		}
		WorkerInstruction workerInstruction = m_CurrentScript.m_Script.m_Instructions[currentInstruction];
		if (workerInstruction.m_Instruction == WorkerInstruction.Instruction.MoveTo || workerInstruction.m_Instruction == WorkerInstruction.Instruction.MoveToLessOne || workerInstruction.m_Instruction == WorkerInstruction.Instruction.MoveToRange)
		{
			int @int = GetInt(m_CurrentScript, currentInstruction, 2);
			if (@int != 0)
			{
				return ObjectTypeList.Instance.GetObjectFromUniqueID(@int, ErrorCheck: false);
			}
		}
		if (workerInstruction.m_Instruction == WorkerInstruction.Instruction.AddResource || workerInstruction.m_Instruction == WorkerInstruction.Instruction.TakeResource)
		{
			int int2 = GetInt(m_CurrentScript, currentInstruction, 0);
			return ObjectTypeList.Instance.GetObjectFromUniqueID(int2, ErrorCheck: false);
		}
		return null;
	}

	public void Save(JSONNode Node)
	{
		Node["Interpreter"] = m_Interpreter;
		Node["Version"] = m_Version;
		JSONArray jSONArray = (JSONArray)(Node["ScriptLocalArray"] = new JSONArray());
		if (m_CurrentScript != null)
		{
			jSONArray[0] = new JSONObject();
			m_CurrentScript.Save(jSONArray[0]);
		}
		JSONArray jSONArray2 = (JSONArray)(Node["GlobalArray"] = new JSONArray());
		for (int i = 0; i < m_MaxGlobalVariables; i++)
		{
			jSONArray2[i] = m_GlobalVariables[i];
		}
		if (m_HighInstructions.m_List.Count > 0)
		{
			m_HighInstructions.Save(Node);
		}
		JSONArray jSONArray3 = (JSONArray)(Node["JSArray"] = new JSONArray());
		for (int j = 0; j < m_FailJumpStack.Count; j++)
		{
			jSONArray3[j] = m_FailJumpStack[j];
		}
	}

	public void Load(JSONNode Node)
	{
		JSONUtils.GetAsString(Node, "Interpreter", m_Interpreter);
		JSONUtils.GetAsInt(Node, "Version", m_Version);
		JSONArray asArray = Node["ScriptLocalArray"].AsArray;
		for (int i = 0; i < asArray.Count; i++)
		{
			_ = asArray[i].AsObject;
			WorkerScriptLocal workerScriptLocal = new WorkerScriptLocal();
			workerScriptLocal.Start();
			workerScriptLocal.Load(asArray[i]);
			AddScript(workerScriptLocal);
		}
		JSONNode jSONNode = Node["JSArray"];
		if (jSONNode != null && !jSONNode.IsNull)
		{
			for (int j = 0; j < jSONNode.Count; j++)
			{
				m_FailJumpStack.Add(jSONNode[j]);
			}
		}
		JSONArray asArray2 = Node["GlobalArray"].AsArray;
		for (int k = 0; k < m_MaxGlobalVariables; k++)
		{
			m_GlobalVariables[k] = asArray2[k];
		}
		m_HighInstructions.Load(Node);
	}

	public void PostLoad()
	{
		if (m_HighInstructions != null)
		{
			m_HighInstructions.PostLoad();
		}
		if (m_CurrentScript != null)
		{
			if (m_CurrentScript.m_Script != null)
			{
				m_CurrentScript.m_Script.PostLoad();
			}
			OldFileUtils.CheckScript(m_CurrentScript, m_GlobalVariables, m_Worker);
		}
	}

	public void UpdateMemorySize()
	{
	}

	public void StartScript()
	{
		if (m_Worker.GetFreeMemory() >= 0)
		{
			WorkerScript newScript = HighInstructionConverter.ConvertHighInstructions(m_HighInstructions.m_List, m_Worker);
			StartScript(newScript);
		}
	}

	public void SetHighInstructions(List<HighInstruction> NewInstructions, bool Start)
	{
		m_HighInstructions.Copy(NewInstructions);
		if (Start && m_Worker.GetFreeMemory() >= 0)
		{
			WorkerScript newScript = HighInstructionConverter.ConvertHighInstructions(m_HighInstructions.m_List, m_Worker);
			StartScript(newScript);
		}
	}

	public void UpdateCurrentScript()
	{
		if (m_HighInstructions.m_List.Count <= 0 || m_CurrentScript == null || m_Worker.GetFreeMemory() < 0)
		{
			return;
		}
		WorkerScript script = HighInstructionConverter.ConvertHighInstructions(m_HighInstructions.m_List, m_Worker);
		m_CurrentScript.m_Script = script;
		m_Labels.Clear();
		WorkerScriptLocal currentScript = m_CurrentScript;
		for (int i = 0; i < currentScript.m_Script.m_Instructions.Count; i++)
		{
			if (currentScript.m_Script.m_Instructions[i].m_Instruction == WorkerInstruction.Instruction.Label)
			{
				InstructionLabel(currentScript, i);
			}
		}
	}

	private void AddScript(WorkerScriptLocal NewScriptLocal)
	{
		m_CurrentScript = NewScriptLocal;
		m_Labels.Clear();
		m_FailJumpStack.Clear();
		WorkerScriptLocal currentScript = m_CurrentScript;
		for (int i = 0; i < currentScript.m_Script.m_Instructions.Count; i++)
		{
			if (currentScript.m_Script.m_Instructions[i].m_Instruction == WorkerInstruction.Instruction.Label)
			{
				InstructionLabel(currentScript, i);
			}
		}
		CheckRunning();
		m_Worker.InterpreterScriptChanged();
	}

	public void StartScript(WorkerScript NewScript)
	{
		if (NewScript != null && NewScript.m_Instructions != null && NewScript.m_Instructions.Count != 0)
		{
			WorkerScriptLocal workerScriptLocal = new WorkerScriptLocal();
			workerScriptLocal.m_Script = NewScript;
			workerScriptLocal.Start();
			AddScript(workerScriptLocal);
		}
	}

	public void CheckRunning()
	{
		if (m_CurrentScript == null || m_Worker.m_Energy + m_Worker.m_AddEnergy == 0f || m_Worker.m_WorkerIndicator.m_NoTool || m_Worker.m_NoToolTimer != 0f || m_Worker.m_WorkerIndicator.m_State == WorkerStatusIndicator.State.Busy)
		{
			m_Worker.SetLayer(Layers.WorkersDead);
		}
		else
		{
			m_Worker.SetLayer(Layers.Workers);
		}
	}

	public void StopCurrent()
	{
		m_CurrentScript = null;
		CheckRunning();
		m_Worker.SetBaggedObject(null);
		m_Worker.SetBaggedTile(new TileCoord(0, 0));
		m_Worker.InterpreterScriptChanged();
	}

	public void StopAll()
	{
		m_CurrentScript = null;
		m_ActionActive = false;
		CheckRunning();
		m_Worker.SetBaggedObject(null);
		m_Worker.SetBaggedTile(new TileCoord(0, 0));
		m_Worker.InterpreterScriptChanged();
	}

	public void ActionActive(bool Active, bool Success)
	{
		if (m_CurrentScript != null)
		{
			bool actionActive = m_ActionActive;
			m_ActionActive = Active;
			m_ActionSuccess = Success;
			if (actionActive && !m_ActionActive)
			{
				CheckWaitingForActive();
			}
		}
	}

	public bool CheckTrackBuilding(Building NewBuilding)
	{
		if (m_CurrentScript == null)
		{
			return false;
		}
		List<WorkerInstruction> instructions = m_CurrentScript.m_Script.m_Instructions;
		if (m_CurrentScript.m_CurrentInstruction < instructions.Count - 1)
		{
			int num = m_CurrentScript.m_CurrentInstruction + 1;
			WorkerInstruction workerInstruction = instructions[num];
			if (workerInstruction.m_Instruction == WorkerInstruction.Instruction.TurnAt || workerInstruction.m_Instruction == WorkerInstruction.Instruction.StopAt)
			{
				int @int = GetInt(m_CurrentScript, num, 0);
				BaseClass objectFromUniqueID = ObjectTypeList.Instance.GetObjectFromUniqueID(@int, ErrorCheck: false);
				if ((bool)objectFromUniqueID && objectFromUniqueID.GetComponent<Building>() == NewBuilding)
				{
					m_CurrentScript.m_CurrentInstruction++;
					return true;
				}
			}
		}
		return false;
	}

	public bool GetTurnAtSoon(TrainTrackPoints NewPoints)
	{
		if (m_CurrentScript == null)
		{
			return false;
		}
		List<WorkerInstruction> instructions = m_CurrentScript.m_Script.m_Instructions;
		if (m_CurrentScript.m_CurrentInstruction < instructions.Count)
		{
			for (int i = 0; i < 3; i++)
			{
				int num = m_CurrentScript.m_CurrentInstruction + i;
				if (num == instructions.Count)
				{
					return false;
				}
				if (instructions[num].m_Instruction == WorkerInstruction.Instruction.TurnAt)
				{
					int @int = GetInt(m_CurrentScript, num, 0);
					BaseClass objectFromUniqueID = ObjectTypeList.Instance.GetObjectFromUniqueID(@int, ErrorCheck: false);
					if ((bool)objectFromUniqueID && objectFromUniqueID.GetComponent<TrainTrackPoints>() == NewPoints)
					{
						return true;
					}
				}
			}
		}
		return false;
	}

	private void ReportError(string Error, WorkerScriptLocal Script, int InstructionIndex)
	{
		Debug.Log("**** Error in script " + Script.m_Script.m_Name + " at line number " + InstructionIndex + " : " + Error);
	}

	public string GetGlobalVariable(int VariableIndex)
	{
		return m_GlobalVariables[VariableIndex];
	}

	public void SetGlobalVariable(int VariableIndex, string NewValue)
	{
		m_GlobalVariables[VariableIndex] = NewValue;
	}

	public bool IsAccessingObject(int NewUID)
	{
		if (m_CurrentScript == null)
		{
			return false;
		}
		WorkerScriptLocal currentScript = m_CurrentScript;
		int currentInstruction = currentScript.m_CurrentInstruction;
		WorkerInstruction workerInstruction = currentScript.m_Script.m_Instructions[currentInstruction];
		if ((workerInstruction.m_Instruction == WorkerInstruction.Instruction.MoveTo || workerInstruction.m_Instruction == WorkerInstruction.Instruction.MoveToLessOne || workerInstruction.m_Instruction == WorkerInstruction.Instruction.MoveToRange) && GetInt(currentScript, currentInstruction, 2) == NewUID)
		{
			return true;
		}
		if ((workerInstruction.m_Instruction == WorkerInstruction.Instruction.AddResource || workerInstruction.m_Instruction == WorkerInstruction.Instruction.TakeResource) && GetInt(currentScript, currentInstruction, 0) == NewUID)
		{
			return true;
		}
		return false;
	}

	private string GetVariable(WorkerScriptLocal Script, int InstructionIndex, int VariableIndex)
	{
		string text = "";
		switch (VariableIndex)
		{
		case 0:
			text = Script.m_Script.m_Instructions[InstructionIndex].m_Variable1;
			break;
		case 1:
			text = Script.m_Script.m_Instructions[InstructionIndex].m_Variable2;
			break;
		case 2:
			text = Script.m_Script.m_Instructions[InstructionIndex].m_Variable3;
			break;
		case 3:
			text = Script.m_Script.m_Instructions[InstructionIndex].m_Variable4;
			break;
		}
		if (text.Length > 5 && text.Substring(0, 5) == "Local")
		{
			int result = 0;
			if (int.TryParse(text.Substring(5, text.Length - 5), out result))
			{
				text = Script.m_LocalVariables[result];
			}
			else
			{
				ReportError("Bad Local variable number " + text, Script, InstructionIndex);
			}
		}
		else if (text.Length > 6 && text.Substring(0, 6) == "Global")
		{
			int result2 = 0;
			if (int.TryParse(text.Substring(6, text.Length - 6), out result2))
			{
				text = m_GlobalVariables[result2];
			}
			else
			{
				ReportError("Bad Global variable number " + text, Script, InstructionIndex);
			}
		}
		return text;
	}

	private void SetVariable(WorkerScriptLocal Script, int InstructionIndex, int VariableIndex, string NewValue)
	{
		string text = "";
		switch (VariableIndex)
		{
		case 0:
			text = Script.m_Script.m_Instructions[InstructionIndex].m_Variable1;
			break;
		case 1:
			text = Script.m_Script.m_Instructions[InstructionIndex].m_Variable2;
			break;
		case 2:
			text = Script.m_Script.m_Instructions[InstructionIndex].m_Variable3;
			break;
		case 3:
			text = Script.m_Script.m_Instructions[InstructionIndex].m_Variable4;
			break;
		}
		if (text.Length > 5 && text.Substring(0, 5) == "Local")
		{
			int result = 0;
			if (int.TryParse(text.Substring(5, text.Length - 5), out result))
			{
				Script.m_LocalVariables[result] = NewValue;
			}
			else
			{
				ReportError("Bad Local variable number " + text, Script, InstructionIndex);
			}
		}
		else if (text.Length > 6 && text.Substring(0, 6) == "Global")
		{
			int result2 = 0;
			if (int.TryParse(text.Substring(6, text.Length - 6), out result2))
			{
				m_GlobalVariables[result2] = NewValue;
			}
			else
			{
				ReportError("Bad Global variable number " + text, Script, InstructionIndex);
			}
		}
		else
		{
			ReportError("Global/Local variable expected : " + text, Script, InstructionIndex);
		}
	}

	private int GetInt(WorkerScriptLocal Script, int InstructionIndex, int VariableIndex)
	{
		int result = 0;
		string variable = GetVariable(Script, InstructionIndex, VariableIndex);
		if (int.TryParse(variable, out result))
		{
			return result;
		}
		ReportError("Unable to convert string to int : " + variable, Script, InstructionIndex);
		return 0;
	}

	private void InstructionIf(WorkerScriptLocal Script, int Index)
	{
		string variable = Script.m_Script.m_Instructions[Index].m_Variable1;
		int @int = GetInt(Script, Index, 1);
		int int2 = GetInt(Script, Index, 2);
		bool flag = false;
		switch (variable)
		{
		case "=":
			flag = @int == int2;
			break;
		case "<":
			flag = @int < int2;
			break;
		case ">":
			flag = @int > int2;
			break;
		case "!=":
			flag = @int != int2;
			break;
		default:
			ReportError("Bad If rule " + variable, Script, Index);
			break;
		}
		if (flag)
		{
			return;
		}
		while (Script.m_CurrentInstruction < Script.m_Script.m_Instructions.Count)
		{
			Script.m_CurrentInstruction++;
			WorkerInstruction.Instruction instruction = Script.m_Script.m_Instructions[Script.m_CurrentInstruction].m_Instruction;
			if (instruction == WorkerInstruction.Instruction.Else || instruction == WorkerInstruction.Instruction.EndIf)
			{
				Script.m_CurrentInstruction++;
				break;
			}
		}
	}

	private void InstructionElse(WorkerScriptLocal Script, int Index)
	{
		while (Script.m_CurrentInstruction < Script.m_Script.m_Instructions.Count)
		{
			Script.m_CurrentInstruction++;
			if (Script.m_Script.m_Instructions[Script.m_CurrentInstruction].m_Instruction == WorkerInstruction.Instruction.EndIf)
			{
				Script.m_CurrentInstruction++;
				break;
			}
		}
	}

	private void InstructionEndIf(WorkerScriptLocal Script, int Index)
	{
	}

	private void InstructionSetVariable(WorkerScriptLocal Script, int Index)
	{
		SetVariable(Script, Index, 0, GetVariable(Script, Index, 1));
	}

	private void InstructionAddVariable(WorkerScriptLocal Script, int Index)
	{
		int @int = GetInt(Script, Index, 0);
		int int2 = GetInt(Script, Index, 1);
		string newValue = (@int + int2).ToString();
		SetVariable(Script, Index, 2, newValue);
	}

	private void InstructionGoTo(WorkerScriptLocal Script, int Index)
	{
		int num = (Script.m_CurrentInstruction = GetInt(Script, Index, 0));
		m_InstructionProcessed = true;
	}

	private void InstructionStartScript(WorkerScriptLocal Script, int Index)
	{
		WorkerScript newScript = null;
		StartScript(newScript);
	}

	private void InstructionEnd(WorkerScriptLocal Script, int Index)
	{
		Script.m_CurrentInstruction = Script.m_Script.m_Instructions.Count;
	}

	private void InstructionWait(WorkerScriptLocal Script, int Index)
	{
		float result = 0f;
		if (float.TryParse(GetVariable(Script, Index, 0), out result))
		{
			Script.m_WaitTimer += TimeManager.Instance.m_NormalDelta;
			if (Script.m_WaitTimer > result * (1f / 60f))
			{
				Script.m_WaitTimer = 0f;
			}
			else
			{
				m_InstructionProcessed = true;
			}
		}
		else
		{
			m_InstructionProcessed = true;
		}
	}

	private void InstructionMoveDelay(WorkerScriptLocal Script, int Index)
	{
		float num = m_Worker.GetTotalMovementDelay();
		Script.m_WaitTimer += TimeManager.Instance.m_NormalDelta;
		if (Script.m_WaitTimer > num)
		{
			Script.m_WaitTimer = 0f;
		}
		else
		{
			m_InstructionProcessed = true;
		}
	}

	private void AddLabelLine(WorkerScriptLocal Script, int Index, string Label, int Line)
	{
		if (!m_Labels.ContainsKey(Label))
		{
			m_Labels.Add(Label, Line);
		}
	}

	private int GetLabelLine(WorkerScriptLocal Script, int Index, string Label)
	{
		int value = 0;
		if (m_Labels.TryGetValue(Label, out value))
		{
			return value;
		}
		ReportError("Unable to find label " + Label, Script, Index);
		return 0;
	}

	private void InstructionLabel(WorkerScriptLocal Script, int Index)
	{
		string variable = GetVariable(Script, Index, 0);
		AddLabelLine(Script, Index, variable, Index + 1);
	}

	private void InstructionJumpTo(WorkerScriptLocal Script, int Index)
	{
		string variable = GetVariable(Script, Index, 0);
		int num = (Script.m_CurrentInstruction = GetLabelLine(Script, Index, variable));
		m_InstructionProcessed = true;
	}

	private void InstructionJumpIfZero(WorkerScriptLocal Script, int Index)
	{
		if (GetInt(Script, Index, 0) == 0)
		{
			string variable = GetVariable(Script, Index, 1);
			int num = (Script.m_CurrentInstruction = GetLabelLine(Script, Index, variable));
			m_InstructionProcessed = true;
		}
	}

	private void InstructionJumpIfNotZero(WorkerScriptLocal Script, int Index)
	{
		if (GetInt(Script, Index, 0) != 0)
		{
			string variable = GetVariable(Script, Index, 1);
			int num = (Script.m_CurrentInstruction = GetLabelLine(Script, Index, variable));
			m_InstructionProcessed = true;
		}
	}

	private void InstructionPushFailJump(WorkerScriptLocal Script, int Index)
	{
		string variable = GetVariable(Script, Index, 0);
		m_FailJumpStack.Add(variable);
	}

	private void InstructionPopFailJump(WorkerScriptLocal Script, int Index)
	{
		if (m_FailJumpStack.Count != 0)
		{
			m_FailJumpStack.RemoveAt(m_FailJumpStack.Count - 1);
		}
	}

	private bool InstructionFailed(WorkerScriptLocal Script, int Index)
	{
		if (m_FailJumpStack.Count == 0)
		{
			m_InstructionFailed = true;
			return false;
		}
		string text = m_FailJumpStack[m_FailJumpStack.Count - 1];
		if (text == "")
		{
			m_InstructionFailed = true;
			return false;
		}
		int num = (Script.m_CurrentInstruction = GetLabelLine(Script, Index, text));
		m_InstructionProcessed = true;
		return true;
	}

	private void DoMoveDelay(WorkerScriptLocal Script)
	{
		if (!SaveLoadManager.m_Video)
		{
			float num = (float)m_Worker.GetTotalMovementDelay() / 60f;
			Script.m_WaitTimer += TimeManager.Instance.m_NormalDelta;
			if (Script.m_WaitTimer > num)
			{
				Script.m_WaitTimer = 0f;
			}
			else
			{
				m_InstructionProcessed = true;
			}
		}
	}

	private AFO.AT GetActionType(WorkerScriptLocal Script, int Index)
	{
		AFO.AT result = AFO.AT.Primary;
		switch (GetInt(Script, Index, 3))
		{
		case 1:
			result = AFO.AT.Secondary;
			break;
		case 3:
			result = AFO.AT.AltSecondary;
			break;
		}
		return result;
	}

	private void InstructionMoveTo(WorkerScriptLocal Script, int Index)
	{
		DoMoveDelay(Script);
		if (m_InstructionProcessed)
		{
			return;
		}
		int @int = GetInt(Script, Index, 0);
		int int2 = GetInt(Script, Index, 1);
		int int3 = GetInt(Script, Index, 2);
		Actionable actionable = null;
		if (int3 != 0)
		{
			BaseClass objectFromUniqueID = ObjectTypeList.Instance.GetObjectFromUniqueID(int3, ErrorCheck: false);
			if (!objectFromUniqueID)
			{
				return;
			}
			actionable = objectFromUniqueID.GetComponent<Actionable>();
		}
		TileCoord tileCoord = new TileCoord(@int, int2);
		if (tileCoord != m_Worker.m_TileCoord || ((bool)actionable && actionable.m_TypeIdentifier == ObjectType.ConverterFoundation))
		{
			m_WaitingForNonActive = true;
			AFO.AT actionType = GetActionType(Script, Index);
			if (!m_Worker.CanDoAction(new ActionInfo(ActionType.MoveTo, tileCoord, actionable, "", "", actionType), RightNow: true))
			{
				m_MoveAttempts++;
				if (actionable == null || !MobileStorage.GetIsTypeMobileStorage(actionable.m_TypeIdentifier) || m_MoveAttempts < 10)
				{
					m_WaitingForNonActive = false;
					m_InstructionFailed = true;
				}
				else
				{
					InstructionFailed(Script, Index);
				}
			}
		}
		else
		{
			m_InstructionProcessed = true;
			Script.m_CurrentInstruction++;
		}
	}

	private void InstructionMoveToLessOne(WorkerScriptLocal Script, int Index)
	{
		DoMoveDelay(Script);
		if (m_InstructionProcessed)
		{
			return;
		}
		m_WaitingForNonActive = true;
		int @int = GetInt(Script, Index, 0);
		int int2 = GetInt(Script, Index, 1);
		int int3 = GetInt(Script, Index, 2);
		Actionable actionable = null;
		if (int3 != 0)
		{
			BaseClass objectFromUniqueID = ObjectTypeList.Instance.GetObjectFromUniqueID(int3, ErrorCheck: false);
			if ((bool)objectFromUniqueID)
			{
				actionable = objectFromUniqueID.GetComponent<Actionable>();
			}
		}
		AFO.AT actionType = GetActionType(Script, Index);
		m_WaitingForNonActive = true;
		TileCoord tileCoord = new TileCoord(@int, int2);
		if (!m_Worker.CanDoAction(new ActionInfo(ActionType.MoveToLessOne, tileCoord, actionable, "", "", actionType), RightNow: true) && tileCoord != m_Worker.m_TileCoord)
		{
			m_MoveAttempts++;
			if (actionable == null || !MobileStorage.GetIsTypeMobileStorage(actionable.m_TypeIdentifier) || m_MoveAttempts < 10)
			{
				m_WaitingForNonActive = false;
				m_InstructionFailed = true;
			}
			else
			{
				InstructionFailed(Script, Index);
			}
		}
	}

	private void InstructionMoveToRange(WorkerScriptLocal Script, int Index)
	{
		DoMoveDelay(Script);
		if (m_InstructionProcessed)
		{
			return;
		}
		m_WaitingForNonActive = true;
		int @int = GetInt(Script, Index, 0);
		int int2 = GetInt(Script, Index, 1);
		int int3 = GetInt(Script, Index, 2);
		Actionable actionable = null;
		if (int3 != 0)
		{
			BaseClass objectFromUniqueID = ObjectTypeList.Instance.GetObjectFromUniqueID(int3, ErrorCheck: false);
			if ((bool)objectFromUniqueID)
			{
				actionable = objectFromUniqueID.GetComponent<Actionable>();
			}
		}
		AFO.AT actionType = GetActionType(Script, Index);
		m_WaitingForNonActive = true;
		TileCoord tileCoord = new TileCoord(@int, int2);
		int num = 0;
		Holdable topObject = m_Worker.m_FarmerCarry.GetTopObject();
		if ((bool)topObject)
		{
			num = VariableManager.Instance.GetVariableAsInt(topObject.m_TypeIdentifier, "UseRange", CheckValid: false);
		}
		if (!m_Worker.CanDoAction(new ActionInfo(ActionType.MoveToRange, tileCoord, actionable, "", num.ToString(), actionType), RightNow: true) && tileCoord != m_Worker.m_TileCoord)
		{
			m_MoveAttempts++;
			if (actionable == null || !MobileStorage.GetIsTypeMobileStorage(actionable.m_TypeIdentifier) || m_MoveAttempts < 10)
			{
				m_WaitingForNonActive = false;
				m_InstructionFailed = true;
			}
			else
			{
				InstructionFailed(Script, Index);
			}
		}
	}

	private void InstructionMoveForwards(WorkerScriptLocal Script, int Index)
	{
		DoMoveDelay(Script);
		if (!m_InstructionProcessed)
		{
			m_WaitingForNonActive = true;
			if (!m_Worker.CanDoAction(new ActionInfo(ActionType.MoveForwards, default(TileCoord)), RightNow: true))
			{
				InstructionFailed(Script, Index);
			}
		}
	}

	private void InstructionMoveBackwards(WorkerScriptLocal Script, int Index)
	{
		DoMoveDelay(Script);
		if (!m_InstructionProcessed)
		{
			m_WaitingForNonActive = true;
			if (!m_Worker.CanDoAction(new ActionInfo(ActionType.MoveBackwards, default(TileCoord)), RightNow: true))
			{
				InstructionFailed(Script, Index);
			}
		}
	}

	private void InstructionMoveDirection(WorkerScriptLocal Script, int Index)
	{
		m_WaitingForNonActive = true;
		int @int = GetInt(Script, Index, 0);
		int int2 = GetInt(Script, Index, 1);
		m_Worker.SendAction(new ActionInfo(ActionType.MoveDirection, new TileCoord(@int, int2)));
	}

	private void InstructionMoveForward(WorkerScriptLocal Script, int Index)
	{
		m_WaitingForNonActive = true;
		m_Worker.SetState(Farmer.State.Moving);
		m_Worker.SendAction(new ActionInfo(ActionType.MoveForward, default(TileCoord)));
	}

	private void InstructionTurn(WorkerScriptLocal Script, int Index)
	{
		m_Worker.SetState(Farmer.State.Turning);
		int @int = GetInt(Script, Index, 0);
		m_Worker.Turn(@int);
	}

	private void InstructionSetTool(WorkerScriptLocal Script, int Index)
	{
		string variable = Script.m_Script.m_Instructions[Index].m_Variable1;
		m_Worker.SendAction(new ActionInfo(ActionType.SetTool, default(TileCoord), null, variable));
	}

	private void InstructionUseInHands(WorkerScriptLocal Script, int Index)
	{
		TileCoord tileCoord = default(TileCoord);
		Actionable @object = null;
		int @int = GetInt(Script, Index, 0);
		int int2 = GetInt(Script, Index, 1);
		tileCoord = new TileCoord(@int, int2);
		int int3 = GetInt(Script, Index, 2);
		if (int3 != 0)
		{
			if ((bool)ObjectTypeList.Instance.GetObjectFromUniqueID(int3, ErrorCheck: false))
			{
				@object = ObjectTypeList.Instance.GetObjectFromUniqueID(int3).GetComponent<Actionable>();
			}
		}
		else
		{
			@object = PlotManager.Instance.GetPlotAtTile(tileCoord);
		}
		string variable = GetVariable(Script, Index, 3);
		AFO.AT actionType = GetActionType();
		string globalVariable = GetGlobalVariable(7);
		ObjectType identifierFromSaveName = ObjectTypeList.Instance.GetIdentifierFromSaveName(GetGlobalVariable(8));
		ActionInfo info = new ActionInfo(ActionType.UseInHands, tileCoord, @object, "", variable, actionType, globalVariable, identifierFromSaveName);
		m_Worker.SendAction(info);
		if (m_Worker.m_WorkerIndicator.m_NoTool || m_Worker.m_NoToolTimer != 0f)
		{
			InstructionFailed(Script, Index);
		}
	}

	private void InstructionDropAll(WorkerScriptLocal Script, int Index)
	{
		m_Worker.SendAction(new ActionInfo(ActionType.DropAll, new TileCoord(0, 0)));
	}

	private void InstructionPickup(WorkerScriptLocal Script, int Index)
	{
		TileCoord tileCoord = m_Worker.m_TileCoord;
		int @int = GetInt(Script, Index, 0);
		BaseClass objectFromUniqueID = ObjectTypeList.Instance.GetObjectFromUniqueID(@int, ErrorCheck: false);
		if ((bool)objectFromUniqueID)
		{
			if (m_Worker.m_FarmerCarry.CanAddCarry(objectFromUniqueID.GetComponent<Holdable>()))
			{
				m_Worker.SendAction(new ActionInfo(ActionType.Pickup, tileCoord, objectFromUniqueID.GetComponent<Actionable>()));
			}
			else
			{
				InstructionFailed(Script, Index);
			}
		}
	}

	private void InstructionMake(WorkerScriptLocal Script, int Index)
	{
		TileCoord tileCoord = m_Worker.m_TileCoord;
		string variable = Script.m_Script.m_Instructions[Index].m_Variable1;
		ObjectType identifierFromSaveName = ObjectTypeList.Instance.GetIdentifierFromSaveName(variable);
		string variable2 = Script.m_Script.m_Instructions[Index].m_Variable2;
		Plot plotAtTile = PlotManager.Instance.GetPlotAtTile(tileCoord);
		if ((bool)plotAtTile)
		{
			TileCoordObject objectTypeAtTile = plotAtTile.GetObjectTypeAtTile(identifierFromSaveName, tileCoord);
			if ((bool)objectTypeAtTile)
			{
				m_Worker.SendAction(new ActionInfo(ActionType.Make, tileCoord, objectTypeAtTile, variable2));
				m_InstructionFailed = false;
			}
		}
	}

	private void InstructionRecharge(WorkerScriptLocal Script, int Index)
	{
		int @int = GetInt(Script, Index, 0);
		BaseClass objectFromUniqueID = ObjectTypeList.Instance.GetObjectFromUniqueID(@int, ErrorCheck: false);
		if ((bool)objectFromUniqueID)
		{
			Actionable component = objectFromUniqueID.GetComponent<Actionable>();
			if ((bool)component)
			{
				m_Worker.SendAction(new ActionInfo(ActionType.Recharge, default(TileCoord), component));
			}
		}
	}

	private void InstructionShout(WorkerScriptLocal Script, int Index)
	{
		string variable = Script.m_Script.m_Instructions[Index].m_Variable1;
		m_Worker.SendAction(new ActionInfo(ActionType.Shout, default(TileCoord), null, variable));
	}

	private void InstructionTakeResource(WorkerScriptLocal Script, int Index)
	{
		int @int = GetInt(Script, Index, 0);
		BaseClass objectFromUniqueID = ObjectTypeList.Instance.GetObjectFromUniqueID(@int, ErrorCheck: false);
		if (!objectFromUniqueID)
		{
			return;
		}
		Actionable component = objectFromUniqueID.GetComponent<Actionable>();
		if (!component)
		{
			return;
		}
		MyTool tool = m_Worker.m_FarmerCarry.GetTool();
		if ((bool)tool && ToolFillable.GetIsTypeFillable(tool.m_TypeIdentifier) && tool.GetComponent<ToolFillable>().GetIsFull())
		{
			return;
		}
		AFO.AT actionType = GetActionType();
		object actionInfo = component.GetActionInfo(new GetActionInfo(GetAction.GetObjectType, null, actionType.ToString()));
		if (actionInfo != null)
		{
			ObjectType objectType = (ObjectType)actionInfo;
			if (objectType != ObjectTypeList.m_Total && m_Worker.m_FarmerCarry.CanAddCarry(objectType))
			{
				Holdable topObject = m_Worker.m_FarmerCarry.GetTopObject();
				objectType = ObjectTypeList.m_Total;
				if (topObject != null)
				{
					objectType = topObject.m_TypeIdentifier;
				}
				Actionable.m_ReusableActionFromObject.Init(topObject, objectType, m_Worker, actionType, "", default(TileCoord));
				ActionType actionFromObjectSafe = component.GetActionFromObjectSafe(Actionable.m_ReusableActionFromObject);
				if (actionFromObjectSafe != ActionType.Total && actionFromObjectSafe != ActionType.Fail)
				{
					m_Worker.SendAction(new ActionInfo(ActionType.TakeResource, new TileCoord(0, 0), component, "", "", Actionable.m_ReusableActionFromObject.m_ActionType, Actionable.m_ReusableActionFromObject.m_RequirementsOut));
				}
				else if (actionFromObjectSafe == ActionType.Total)
				{
					m_InstructionFailed = true;
				}
				else
				{
					InstructionFailed(Script, Index);
				}
			}
			else if (m_FailJumpStack.Count != 0)
			{
				InstructionFailed(Script, Index);
			}
		}
		else
		{
			InstructionFailed(Script, Index);
		}
	}

	private AFO.AT GetActionType()
	{
		AFO.AT result = AFO.AT.Primary;
		if (GetGlobalVariable(5) == "1")
		{
			result = AFO.AT.Secondary;
		}
		else if (GetGlobalVariable(5) == "2")
		{
			result = AFO.AT.AltPrimary;
		}
		if (GetGlobalVariable(5) == "3")
		{
			result = AFO.AT.AltSecondary;
		}
		return result;
	}

	private void InstructionAddResource(WorkerScriptLocal Script, int Index)
	{
		int @int = GetInt(Script, Index, 0);
		BaseClass objectFromUniqueID = ObjectTypeList.Instance.GetObjectFromUniqueID(@int, ErrorCheck: false);
		if ((bool)objectFromUniqueID)
		{
			Actionable component = objectFromUniqueID.GetComponent<Actionable>();
			Actionable actioner = m_Worker;
			if (!component)
			{
				return;
			}
			Holdable holdable = m_Worker.m_FarmerCarry.GetTopObject();
			if (m_Worker.m_EngagedObject != null && m_Worker.m_EngagedObject.m_TypeIdentifier == ObjectType.CraneCrude)
			{
				holdable = m_Worker.m_EngagedObject.GetComponent<CraneCrude>().m_HeldObject.GetComponent<Holdable>();
				actioner = m_Worker.m_EngagedObject;
			}
			if (holdable != null)
			{
				if (m_Worker.m_FarmerAction.IsTargetNear(component))
				{
					if (component.m_TypeIdentifier == ObjectType.Worker && (component.transform.position - m_Worker.transform.position).magnitude > Tile.m_Size * (float)FarmerAction.m_ThrowDistanceTiles)
					{
						InstructionFailed(Script, Index);
						return;
					}
					ObjectType typeIdentifier = holdable.m_TypeIdentifier;
					AFO.AT actionType = GetActionType();
					Actionable.m_ReusableActionFromObject.Init(holdable, typeIdentifier, actioner, actionType, "", default(TileCoord));
					ActionType actionFromObjectSafe = component.GetActionFromObjectSafe(Actionable.m_ReusableActionFromObject);
					if (actionFromObjectSafe != ActionType.Total && actionFromObjectSafe != ActionType.Fail)
					{
						m_Worker.SendAction(new ActionInfo(ActionType.AddResource, new TileCoord(0, 0), component, "", "", Actionable.m_ReusableActionFromObject.m_ActionType, Actionable.m_ReusableActionFromObject.m_RequirementsOut));
					}
					else if (actionFromObjectSafe == ActionType.Total)
					{
						m_InstructionFailed = true;
					}
					else if (Housing.GetIsTypeHouse(component.m_TypeIdentifier))
					{
						if (m_FailJumpStack.Count != 0 || Folk.GetWillFolkAcceptObjectType(typeIdentifier))
						{
							InstructionFailed(Script, Index);
						}
					}
					else if (component.m_TypeIdentifier != ObjectType.ConverterFoundation || m_FailJumpStack.Count != 0)
					{
						InstructionFailed(Script, Index);
					}
				}
				else
				{
					m_Worker.SetBaggedObject(null);
					if (ObjectTypeList.Instance.GetIsBuilding(component.m_TypeIdentifier) && m_FailJumpStack.Count != 0)
					{
						InstructionFailed(Script, Index);
					}
				}
			}
			else
			{
				m_Worker.SetBaggedObject(null);
				if (m_FailJumpStack.Count != 0)
				{
					InstructionFailed(Script, Index);
				}
			}
		}
		else if (m_FailJumpStack.Count != 0)
		{
			InstructionFailed(Script, Index);
		}
	}

	private void InstructionStowObject(WorkerScriptLocal Script, int Index)
	{
		m_Worker.SendAction(new ActionInfo(ActionType.StowObject, default(TileCoord)));
	}

	private void InstructionRecallObject(WorkerScriptLocal Script, int Index)
	{
		string variable = Script.m_Script.m_Instructions[Index].m_Variable1;
		m_Worker.SendAction(new ActionInfo(ActionType.RecallObject, default(TileCoord), null, variable));
	}

	private void InstructionCycleObject(WorkerScriptLocal Script, int Index)
	{
		string variable = Script.m_Script.m_Instructions[Index].m_Variable1;
		m_Worker.SendAction(new ActionInfo(ActionType.CycleObject, default(TileCoord), null, variable));
	}

	private void InstructionSwapObject(WorkerScriptLocal Script, int Index)
	{
		m_Worker.SendAction(new ActionInfo(ActionType.SwapObject, default(TileCoord)));
	}

	private void InstructionCheckHandsFull(WorkerScriptLocal Script, int Index)
	{
		if (m_Worker.m_FarmerCarry.AreHandsFull())
		{
			SetVariable(Script, Index, 0, "1");
		}
		else
		{
			SetVariable(Script, Index, 0, "0");
		}
	}

	private void InstructionCheckHandsNotFull(WorkerScriptLocal Script, int Index)
	{
		if (!m_Worker.m_FarmerCarry.AreHandsFull())
		{
			SetVariable(Script, Index, 0, "1");
		}
		else
		{
			SetVariable(Script, Index, 0, "0");
		}
	}

	private void InstructionCheckHandsEmpty(WorkerScriptLocal Script, int Index)
	{
		if (m_Worker.m_FarmerCarry.GetCarryCount() == 0)
		{
			SetVariable(Script, Index, 0, "1");
		}
		else
		{
			SetVariable(Script, Index, 0, "0");
		}
	}

	private void InstructionCheckHandsNotEmpty(WorkerScriptLocal Script, int Index)
	{
		if (m_Worker.m_FarmerCarry.GetCarryCount() != 0)
		{
			SetVariable(Script, Index, 0, "1");
		}
		else
		{
			SetVariable(Script, Index, 0, "0");
		}
	}

	private void InstructionCheckInventoryFull(WorkerScriptLocal Script, int Index)
	{
		if (m_Worker.m_FarmerInventory.GetFreeSlots() == 0)
		{
			SetVariable(Script, Index, 0, "1");
		}
		else
		{
			SetVariable(Script, Index, 0, "0");
		}
	}

	private void InstructionCheckInventoryNotFull(WorkerScriptLocal Script, int Index)
	{
		if (m_Worker.m_FarmerInventory.GetFreeSlots() != 0)
		{
			SetVariable(Script, Index, 0, "1");
		}
		else
		{
			SetVariable(Script, Index, 0, "0");
		}
	}

	private void InstructionCheckInventoryEmpty(WorkerScriptLocal Script, int Index)
	{
		if (m_Worker.m_FarmerInventory.GetFreeSlots() == m_Worker.m_FarmerInventory.m_TotalCapacity)
		{
			SetVariable(Script, Index, 0, "1");
		}
		else
		{
			SetVariable(Script, Index, 0, "0");
		}
	}

	private void InstructionCheckInventoryNotEmpty(WorkerScriptLocal Script, int Index)
	{
		if (m_Worker.m_FarmerInventory.GetFreeSlots() != m_Worker.m_FarmerInventory.m_TotalCapacity)
		{
			SetVariable(Script, Index, 0, "1");
		}
		else
		{
			SetVariable(Script, Index, 0, "0");
		}
	}

	private void InstructionCheckBuildingFull(WorkerScriptLocal Script, int Index)
	{
		int @int = GetInt(Script, Index, 0);
		BaseClass objectFromUniqueID = ObjectTypeList.Instance.GetObjectFromUniqueID(@int, ErrorCheck: false);
		if (objectFromUniqueID == null)
		{
			SetVariable(Script, Index, 1, "1");
		}
		else if (Storage.GetIsTypeStorage(objectFromUniqueID.m_TypeIdentifier))
		{
			Storage component = objectFromUniqueID.GetComponent<Storage>();
			component.GetStored();
			component.GetCapacity();
			_ = Storage.m_PercentFull;
			if (component.GetStoredPercentFull())
			{
				SetVariable(Script, Index, 1, "1");
			}
			else
			{
				SetVariable(Script, Index, 1, "0");
			}
		}
		else if (Converter.GetIsTypeConverter(objectFromUniqueID.m_TypeIdentifier))
		{
			if (objectFromUniqueID.GetComponent<Converter>().GetIsResultCreated())
			{
				SetVariable(Script, Index, 1, "1");
			}
			else
			{
				SetVariable(Script, Index, 1, "0");
			}
		}
		else if (MobileStorage.GetIsTypeMobileStorage(objectFromUniqueID.m_TypeIdentifier))
		{
			MobileStorage component2 = objectFromUniqueID.GetComponent<MobileStorage>();
			int stored = component2.GetStored();
			int capacity = component2.GetCapacity();
			if (capacity > 0 && stored >= capacity)
			{
				SetVariable(Script, Index, 1, "1");
			}
			else
			{
				SetVariable(Script, Index, 1, "0");
			}
		}
		else if (Minecart.GetIsTypeMinecart(objectFromUniqueID.m_TypeIdentifier))
		{
			if (objectFromUniqueID.GetComponent<Minecart>().GetIsFull())
			{
				SetVariable(Script, Index, 1, "1");
			}
			else
			{
				SetVariable(Script, Index, 1, "0");
			}
		}
		else if (Crane.GetIsTypeCrane(objectFromUniqueID.m_TypeIdentifier))
		{
			if ((bool)objectFromUniqueID.GetComponent<Crane>().m_HeldObject)
			{
				SetVariable(Script, Index, 1, "1");
			}
			else
			{
				SetVariable(Script, Index, 1, "0");
			}
		}
		else if (objectFromUniqueID.m_TypeIdentifier == ObjectType.TrainRefuellingStation)
		{
			TrainRefuellingStation component3 = objectFromUniqueID.GetComponent<TrainRefuellingStation>();
			if (component3.m_Fuel >= TrainRefuellingStation.m_MaxFuel * TrainRefuellingStation.m_FullPercent && component3.m_Water == TrainRefuellingStation.m_MaxWater)
			{
				SetVariable(Script, Index, 1, "1");
			}
			else
			{
				SetVariable(Script, Index, 1, "0");
			}
		}
		else if (objectFromUniqueID.m_TypeIdentifier == ObjectType.StationaryEngine)
		{
			StationaryEngine component4 = objectFromUniqueID.GetComponent<StationaryEngine>();
			int energy = component4.m_Energy;
			int energyCapacity = component4.m_EnergyCapacity;
			if (energyCapacity > 0 && energy >= energyCapacity)
			{
				SetVariable(Script, Index, 1, "1");
			}
			else
			{
				SetVariable(Script, Index, 1, "0");
			}
		}
	}

	private void InstructionCheckBuildingNotFull(WorkerScriptLocal Script, int Index)
	{
		int @int = GetInt(Script, Index, 0);
		BaseClass objectFromUniqueID = ObjectTypeList.Instance.GetObjectFromUniqueID(@int, ErrorCheck: false);
		if (objectFromUniqueID == null)
		{
			SetVariable(Script, Index, 1, "1");
		}
		else if ((bool)objectFromUniqueID.GetComponent<Storage>())
		{
			if (!objectFromUniqueID.GetComponent<Storage>().GetStoredPercentFull())
			{
				SetVariable(Script, Index, 1, "1");
			}
			else
			{
				SetVariable(Script, Index, 1, "0");
			}
		}
		else if ((bool)objectFromUniqueID.GetComponent<Converter>())
		{
			if (!objectFromUniqueID.GetComponent<Converter>().GetIsResultCreated())
			{
				SetVariable(Script, Index, 1, "1");
			}
			else
			{
				SetVariable(Script, Index, 1, "0");
			}
		}
		else if ((bool)objectFromUniqueID.GetComponent<MobileStorage>())
		{
			if (!objectFromUniqueID.GetComponent<MobileStorage>().GetStoredPercentFull())
			{
				SetVariable(Script, Index, 1, "1");
			}
			else
			{
				SetVariable(Script, Index, 1, "0");
			}
		}
		else if (Minecart.GetIsTypeMinecart(objectFromUniqueID.m_TypeIdentifier))
		{
			if (!objectFromUniqueID.GetComponent<Minecart>().GetIsFull())
			{
				SetVariable(Script, Index, 1, "1");
			}
			else
			{
				SetVariable(Script, Index, 1, "0");
			}
		}
		else if (Crane.GetIsTypeCrane(objectFromUniqueID.m_TypeIdentifier))
		{
			if (objectFromUniqueID.GetComponent<Crane>().m_HeldObject == null)
			{
				SetVariable(Script, Index, 1, "1");
			}
			else
			{
				SetVariable(Script, Index, 1, "0");
			}
		}
		else if (objectFromUniqueID.m_TypeIdentifier == ObjectType.TrainRefuellingStation)
		{
			TrainRefuellingStation component = objectFromUniqueID.GetComponent<TrainRefuellingStation>();
			if (component.m_Fuel < TrainRefuellingStation.m_MaxFuel * TrainRefuellingStation.m_FullPercent || component.m_Water != TrainRefuellingStation.m_MaxWater)
			{
				SetVariable(Script, Index, 1, "1");
			}
			else
			{
				SetVariable(Script, Index, 1, "0");
			}
		}
	}

	private void InstructionCheckBuildingEmpty(WorkerScriptLocal Script, int Index)
	{
		int @int = GetInt(Script, Index, 0);
		BaseClass objectFromUniqueID = ObjectTypeList.Instance.GetObjectFromUniqueID(@int, ErrorCheck: false);
		if (objectFromUniqueID == null)
		{
			SetVariable(Script, Index, 1, "1");
		}
		else if ((bool)objectFromUniqueID.GetComponent<Storage>())
		{
			if (objectFromUniqueID.GetComponent<Storage>().GetStored() == 0)
			{
				SetVariable(Script, Index, 1, "1");
			}
			else
			{
				SetVariable(Script, Index, 1, "0");
			}
		}
		else if ((bool)objectFromUniqueID.GetComponent<Converter>())
		{
			if (!objectFromUniqueID.GetComponent<Converter>().GetIsResultCreated())
			{
				SetVariable(Script, Index, 1, "1");
			}
			else
			{
				SetVariable(Script, Index, 1, "0");
			}
		}
		else if ((bool)objectFromUniqueID.GetComponent<MobileStorage>())
		{
			if (objectFromUniqueID.GetComponent<MobileStorage>().GetStored() == 0)
			{
				SetVariable(Script, Index, 1, "1");
			}
			else
			{
				SetVariable(Script, Index, 1, "0");
			}
		}
		else if (Minecart.GetIsTypeMinecart(objectFromUniqueID.m_TypeIdentifier))
		{
			if (objectFromUniqueID.GetComponent<Minecart>().GetIsEmpty())
			{
				SetVariable(Script, Index, 1, "1");
			}
			else
			{
				SetVariable(Script, Index, 1, "0");
			}
		}
		else if (Crane.GetIsTypeCrane(objectFromUniqueID.m_TypeIdentifier))
		{
			if (objectFromUniqueID.GetComponent<Crane>().m_HeldObject == null)
			{
				SetVariable(Script, Index, 1, "1");
			}
			else
			{
				SetVariable(Script, Index, 1, "0");
			}
		}
		else if (objectFromUniqueID.m_TypeIdentifier == ObjectType.TrainRefuellingStation)
		{
			TrainRefuellingStation component = objectFromUniqueID.GetComponent<TrainRefuellingStation>();
			if (component.m_Fuel == 0f && component.m_Water == 0f)
			{
				SetVariable(Script, Index, 1, "1");
			}
			else
			{
				SetVariable(Script, Index, 1, "0");
			}
		}
	}

	private void InstructionCheckBuildingNotEmpty(WorkerScriptLocal Script, int Index)
	{
		int @int = GetInt(Script, Index, 0);
		BaseClass objectFromUniqueID = ObjectTypeList.Instance.GetObjectFromUniqueID(@int, ErrorCheck: false);
		if (objectFromUniqueID == null)
		{
			SetVariable(Script, Index, 1, "1");
		}
		else if ((bool)objectFromUniqueID.GetComponent<Storage>())
		{
			if (objectFromUniqueID.GetComponent<Storage>().GetStored() != 0)
			{
				SetVariable(Script, Index, 1, "1");
			}
			else
			{
				SetVariable(Script, Index, 1, "0");
			}
		}
		else if ((bool)objectFromUniqueID.GetComponent<Converter>())
		{
			if (objectFromUniqueID.GetComponent<Converter>().GetIsResultCreated())
			{
				SetVariable(Script, Index, 1, "1");
			}
			else
			{
				SetVariable(Script, Index, 1, "0");
			}
		}
		else if ((bool)objectFromUniqueID.GetComponent<MobileStorage>())
		{
			if (objectFromUniqueID.GetComponent<MobileStorage>().GetStored() != 0)
			{
				SetVariable(Script, Index, 1, "1");
			}
			else
			{
				SetVariable(Script, Index, 1, "0");
			}
		}
		else if (Minecart.GetIsTypeMinecart(objectFromUniqueID.m_TypeIdentifier))
		{
			if (!objectFromUniqueID.GetComponent<Minecart>().GetIsEmpty())
			{
				SetVariable(Script, Index, 1, "1");
			}
			else
			{
				SetVariable(Script, Index, 1, "0");
			}
		}
		else if (Crane.GetIsTypeCrane(objectFromUniqueID.m_TypeIdentifier))
		{
			if (objectFromUniqueID.GetComponent<Crane>().m_HeldObject != null)
			{
				SetVariable(Script, Index, 1, "1");
			}
			else
			{
				SetVariable(Script, Index, 1, "0");
			}
		}
		else if (objectFromUniqueID.m_TypeIdentifier == ObjectType.TrainRefuellingStation)
		{
			TrainRefuellingStation component = objectFromUniqueID.GetComponent<TrainRefuellingStation>();
			if (component.m_Fuel != 0f || component.m_Water != 0f)
			{
				SetVariable(Script, Index, 1, "1");
			}
			else
			{
				SetVariable(Script, Index, 1, "0");
			}
		}
	}

	private void InstructionCheckHeldObjectFull(WorkerScriptLocal Script, int Index)
	{
		Holdable topObject = m_Worker.m_FarmerCarry.GetTopObject();
		if (topObject == null || topObject.GetComponent<ToolFillable>() == null)
		{
			SetVariable(Script, Index, 0, "1");
		}
		else if (topObject.GetComponent<ToolFillable>().GetIsFull())
		{
			SetVariable(Script, Index, 0, "1");
		}
		else
		{
			SetVariable(Script, Index, 0, "0");
		}
	}

	private void InstructionCheckHeldObjectNotFull(WorkerScriptLocal Script, int Index)
	{
		Holdable topObject = m_Worker.m_FarmerCarry.GetTopObject();
		if (topObject == null || topObject.GetComponent<ToolFillable>() == null)
		{
			SetVariable(Script, Index, 0, "1");
		}
		else if (!topObject.GetComponent<ToolFillable>().GetIsFull())
		{
			SetVariable(Script, Index, 0, "1");
		}
		else
		{
			SetVariable(Script, Index, 0, "0");
		}
	}

	private void InstructionCheckHeldObjectEmpty(WorkerScriptLocal Script, int Index)
	{
		Holdable topObject = m_Worker.m_FarmerCarry.GetTopObject();
		if (topObject == null || topObject.GetComponent<ToolFillable>() == null)
		{
			SetVariable(Script, Index, 0, "1");
		}
		else if (topObject.GetComponent<ToolFillable>().GetIsEmpty())
		{
			SetVariable(Script, Index, 0, "1");
		}
		else
		{
			SetVariable(Script, Index, 0, "0");
		}
	}

	private void InstructionCheckHeldObjectNotEmpty(WorkerScriptLocal Script, int Index)
	{
		Holdable topObject = m_Worker.m_FarmerCarry.GetTopObject();
		if (topObject == null || topObject.GetComponent<ToolFillable>() == null)
		{
			SetVariable(Script, Index, 0, "1");
		}
		else if (!topObject.GetComponent<ToolFillable>().GetIsEmpty())
		{
			SetVariable(Script, Index, 0, "1");
		}
		else
		{
			SetVariable(Script, Index, 0, "0");
		}
	}

	private void InstructionCheckHear(WorkerScriptLocal Script, int Index)
	{
		string variable = Script.m_Script.m_Instructions[Index].m_Variable1;
		if (WorkerScriptManager.Instance.GetCurrentShout(variable, this))
		{
			SetVariable(Script, Index, 1, "1");
		}
		else
		{
			SetVariable(Script, Index, 1, "0");
		}
	}

	private void InstructionEngageObject(WorkerScriptLocal Script, int Index)
	{
		int @int = GetInt(Script, Index, 0);
		BaseClass objectFromUniqueID = ObjectTypeList.Instance.GetObjectFromUniqueID(@int, ErrorCheck: false);
		if ((bool)objectFromUniqueID)
		{
			Actionable component = objectFromUniqueID.GetComponent<Actionable>();
			if (m_Worker.m_FarmerAction.IsTargetNear(component, TestBots: true))
			{
				if (component.CanDoAction(new ActionInfo(ActionType.Engaged, default(TileCoord), m_Worker)))
				{
					AFO.AT actionType = GetActionType();
					m_Worker.SendAction(new ActionInfo(ActionType.EngageObject, default(TileCoord), component.GetComponent<Actionable>(), "", "", actionType));
					m_ActionActive = false;
					m_InstructionProcessed = true;
					Script.m_CurrentInstruction++;
				}
				else
				{
					InstructionFailed(Script, Index);
				}
			}
			else
			{
				InstructionFailed(Script, Index);
			}
		}
		else
		{
			Debug.Log("WorkerInterpreter.InstructionEngageObject : Couldn't find object with UID " + @int + ". Bot " + m_Worker.m_WorkerName + ". Line " + Script.m_CurrentInstruction);
			InstructionFailed(Script, Index);
		}
	}

	private void InstructionDisengageObject(WorkerScriptLocal Script, int Index)
	{
		int @int = GetInt(Script, Index, 0);
		int int2 = GetInt(Script, Index, 1);
		if (m_Worker.m_State == Farmer.State.Engaged && (bool)m_Worker.m_EngagedObject && m_Worker.m_EngagedObject.m_TypeIdentifier == ObjectType.Worker)
		{
			m_InstructionProcessed = true;
		}
		else
		{
			m_Worker.SendAction(new ActionInfo(ActionType.DisengageObject, new TileCoord(@int, int2)));
		}
	}

	private void InstructionSetValue(WorkerScriptLocal Script, int Index)
	{
		int @int = GetInt(Script, Index, 0);
		BaseClass objectFromUniqueID = ObjectTypeList.Instance.GetObjectFromUniqueID(@int, ErrorCheck: false);
		if (objectFromUniqueID == null)
		{
			Debug.Log("WorkerInterpreter.InstructionSetValue : Couldn't find object with UID " + @int + ". Bot " + m_Worker.m_WorkerName + ". Line " + Script.m_CurrentInstruction);
		}
		else
		{
			string variable = GetVariable(Script, Index, 1);
			string variable2 = GetVariable(Script, Index, 2);
			m_Worker.SendAction(new ActionInfo(ActionType.SetValue, default(TileCoord), objectFromUniqueID.GetComponent<Actionable>(), variable, variable2));
		}
	}

	private void InstructionGetValue(WorkerScriptLocal Script, int Index)
	{
		string variable = GetVariable(Script, Index, 0);
		string text = "";
		m_Worker.SendAction(new ActionInfo(ActionType.GetValue, default(TileCoord), null, variable, text));
		SetVariable(Script, Index, 1, text);
	}

	private void InstructionGetObjectTileCoords(WorkerScriptLocal Script, int Index)
	{
		int @int = GetInt(Script, Index, 0);
		BaseClass objectFromUniqueID = ObjectTypeList.Instance.GetObjectFromUniqueID(@int, ErrorCheck: false);
		if (objectFromUniqueID == null)
		{
			Debug.Log("WorkerInterpreter.InstructionGetObjectTileCoords : Couldn't find object with UID " + @int + ". Bot " + m_Worker.m_WorkerName + ". Line " + Script.m_CurrentInstruction);
		}
		else
		{
			TileCoord tileCoord = objectFromUniqueID.GetComponent<TileCoordObject>().m_TileCoord;
			SetVariable(Script, Index, 1, tileCoord.x.ToString());
			SetVariable(Script, Index, 2, tileCoord.y.ToString());
		}
	}

	public void GetWorkingRange(out TileCoord TopLeft, out TileCoord BottomRight, out HighInstruction.FindType NewFindType)
	{
		char[] separator = new char[1] { ' ' };
		string[] array = GetGlobalVariable(3).Split(separator);
		if (array.Length < 4)
		{
			TopLeft = new TileCoord(0, 0);
			BottomRight = new TileCoord(0, 0);
			NewFindType = HighInstruction.FindType.Full;
			return;
		}
		int nx = GeneralUtils.IntParseFast(array[0]);
		int ny = GeneralUtils.IntParseFast(array[1]);
		int nx2 = GeneralUtils.IntParseFast(array[2]);
		int ny2 = GeneralUtils.IntParseFast(array[3]);
		string text = HighInstruction.FindType.Full.ToString();
		if (array.Length > 4)
		{
			text = array[4];
		}
		NewFindType = HighInstruction.GetFindTypeFromName(text);
		TopLeft = new TileCoord(nx, ny);
		BottomRight = new TileCoord(nx2, ny2);
		if (!int.TryParse(GetGlobalVariable(6), out var result) || result == 0)
		{
			return;
		}
		BaseClass objectFromUniqueID = ObjectTypeList.Instance.GetObjectFromUniqueID(result, ErrorCheck: false);
		if ((bool)objectFromUniqueID)
		{
			Sign component = objectFromUniqueID.GetComponent<Sign>();
			if ((bool)component)
			{
				TopLeft = component.m_TopLeft;
				BottomRight = component.m_BottomRight;
			}
			Converter component2 = objectFromUniqueID.GetComponent<Converter>();
			if ((bool)component2)
			{
				TopLeft = component2.GetSpawnPoint();
				BottomRight = TopLeft;
				NewFindType = HighInstruction.FindType.Full;
			}
		}
	}

	private void DoFindNearestDelay(WorkerScriptLocal Script)
	{
		if (!SaveLoadManager.m_TestBuild || !SaveLoadManager.m_Video)
		{
			float num = (float)m_Worker.GetTotalSearchDelay() / 60f;
			Script.m_WaitTimer += TimeManager.Instance.m_NormalDelta;
			if (Script.m_WaitTimer > num)
			{
				Script.m_WaitTimer = 0f;
			}
			else
			{
				m_InstructionProcessed = true;
			}
		}
	}

	private bool DoFindNearest(WorkerScriptLocal Script, int Index, GetObjects GetObjectsFunction)
	{
		DoFindNearestDelay(Script);
		if (m_InstructionProcessed)
		{
			m_InstructionProcessing = true;
			return false;
		}
		if (m_Worker.m_WorkerName == WorkerScriptManager.Instance.m_BotBreakName)
		{
			m_Worker.m_WorkerName = m_Worker.m_WorkerName;
		}
		if (!Script.m_GetNearestStarted)
		{
			GetWorkingRange(out var TopLeft, out var BottomRight, out var NewFindType);
			if (GetObjectsFunction(Script, Index, TopLeft, BottomRight, NewFindType))
			{
				Script.m_GetNearestStarted = true;
			}
			else
			{
				InstructionFailed(Script, Index);
			}
			m_InstructionProcessing = true;
			m_InstructionProcessed = true;
		}
		else if (m_Worker.m_FindFinished)
		{
			Script.m_GetNearestStarted = false;
			m_Worker.RemoveBagged();
			if ((m_Worker.m_FoundObjects != null && m_Worker.m_FoundObjects.Count > 0) || (m_Worker.m_FoundDestinations != null && m_Worker.m_FoundDestinations.Count > 0))
			{
				m_InstructionProcessing = true;
				return true;
			}
			if (m_FullSearch)
			{
				GetWorkingRange(out var TopLeft2, out var BottomRight2, out var NewFindType2);
				if (GetObjectsFunction(Script, Index, TopLeft2, BottomRight2, NewFindType2))
				{
					Script.m_GetNearestStarted = true;
				}
				else
				{
					InstructionFailed(Script, Index);
				}
				m_InstructionProcessing = true;
				m_InstructionProcessed = true;
			}
			else
			{
				InstructionFailed(Script, Index);
			}
		}
		else
		{
			m_InstructionProcessing = true;
			m_InstructionProcessed = true;
		}
		return false;
	}

	private bool FindTiles(WorkerScriptLocal Script, int Index, TileCoord TopLeft, TileCoord BottomRight, HighInstruction.FindType NewFindType)
	{
		Tile.TileType typeFromName = Tile.GetTypeFromName(GetVariable(Script, Index, 0));
		ObjectType identifierFromSaveName = ObjectTypeList.Instance.GetIdentifierFromSaveName(GetGlobalVariable(4));
		AFO.AT actionType = GetActionType();
		Dictionary<Tile.TileType, int> dictionary = null;
		if (Tile.m_HiddenTypes.ContainsKey(typeFromName))
		{
			dictionary = Tile.m_HiddenTypes;
		}
		if (Tile.m_StoneTypes.ContainsKey(typeFromName))
		{
			dictionary = Tile.m_StoneTypes;
		}
		if (Tile.m_ClayTypes.ContainsKey(typeFromName))
		{
			dictionary = Tile.m_ClayTypes;
		}
		if (Tile.m_IronTypes.ContainsKey(typeFromName))
		{
			dictionary = Tile.m_IronTypes;
		}
		if (Tile.m_CoalTypes.ContainsKey(typeFromName))
		{
			dictionary = Tile.m_CoalTypes;
		}
		List<TileCoord> list = ((dictionary != null) ? (m_FullSearch ? TileHelpers.GetTilesOfTypes(dictionary, TopLeft, BottomRight, NewFindType, m_Worker, identifierFromSaveName, actionType, "") : TileHelpers.GetNearestTileOfTypes(dictionary, TopLeft, BottomRight, NewFindType, m_Worker, identifierFromSaveName, actionType, "")) : (m_FullSearch ? TileHelpers.GetTilesOfType(typeFromName, TopLeft, BottomRight, NewFindType, m_Worker, identifierFromSaveName, actionType, "") : TileHelpers.GetNearestTileOfType(typeFromName, TopLeft, BottomRight, NewFindType, m_Worker, identifierFromSaveName, actionType, "")));
		if (list.Count > 0)
		{
			m_FullSearch = true;
			if (Tile.GetIsSolid(typeFromName))
			{
				TileCoord item = default(TileCoord);
				float num = 1000000f;
				foreach (TileCoord item2 in list)
				{
					float num2 = (item2 - m_Worker.m_TileCoord).Magnitude();
					if (num > num2)
					{
						num = num2;
						item = item2;
						list.Remove(item2);
						break;
					}
				}
				m_Worker.m_FindFinished = true;
				m_Worker.m_FoundDestinations = new List<TileCoord>();
				m_Worker.m_FoundDestinations.Add(item);
			}
			else
			{
				m_Worker.RequestFind(list);
			}
			return true;
		}
		return false;
	}

	private void InstructionFindNearestTile(WorkerScriptLocal Script, int Index)
	{
		m_Worker.SetBaggedTile(new TileCoord(0, 0));
		if (DoFindNearest(Script, Index, FindTiles))
		{
			m_FullSearch = false;
			if (m_Worker.m_FoundDestinations.Count == 0 || BaggedManager.Instance.IsTileBagged(m_Worker.m_FoundDestinations[0]))
			{
				InstructionFailed(Script, Index);
				return;
			}
			TileCoord baggedTile = m_Worker.m_FoundDestinations[0];
			m_Worker.SetBaggedTile(baggedTile);
			SetVariable(Script, Index, 1, baggedTile.x.ToString());
			SetVariable(Script, Index, 2, baggedTile.y.ToString());
		}
	}

	private bool IsObjectTypeFindBest(ObjectType NewType, ObjectType NewObjectType)
	{
		if (NewType == ObjectType.Folk && Food.GetIsTypeFood(NewObjectType))
		{
			return true;
		}
		if ((Housing.m_Types.Contains(NewType) || NewType == ObjectType.HouseAny) && Food.GetIsTypeFood(NewObjectType))
		{
			return true;
		}
		if (Fueler.GetIsTypeFueler(NewType) && BurnableFuel.GetIsBurnableFuel(NewObjectType))
		{
			return true;
		}
		if (NewType == ObjectType.Trough && Trough.GetIsObjectAcceptable(NewObjectType))
		{
			return true;
		}
		if (NewType == ObjectType.TrainRefuellingStation)
		{
			if (Train.IsObjectTypeAcceptableFuel(NewObjectType))
			{
				return true;
			}
			if (ToolFillable.GetIsTypeFillable(NewObjectType))
			{
				return true;
			}
		}
		if (NewType == ObjectType.StationaryEngine)
		{
			if (StationaryEngine.GetIsObjectAcceptableAsFuel(NewObjectType))
			{
				return true;
			}
			if (ToolFillable.GetIsTypeFillable(NewObjectType))
			{
				return true;
			}
		}
		return false;
	}

	private bool FindNearestObject(WorkerScriptLocal Script, int Index, TileCoord TopLeft, TileCoord BottomRight, HighInstruction.FindType NewFindType)
	{
		string variable = Script.m_Script.m_Instructions[Index].m_Variable1;
		ObjectType identifierFromSaveName = ObjectTypeList.Instance.GetIdentifierFromSaveName(GetGlobalVariable(4), Check: false);
		AFO.AT actionType = GetActionType();
		string globalVariable = GetGlobalVariable(7);
		ObjectType identifierFromSaveName2 = ObjectTypeList.Instance.GetIdentifierFromSaveName(variable, Check: false);
		m_BestSearch = false;
		if (IsObjectTypeFindBest(identifierFromSaveName2, identifierFromSaveName))
		{
			m_FullSearch = true;
			m_BestSearch = true;
		}
		Actionable requester = m_Worker;
		if ((bool)m_Worker.m_EngagedObject && Crane.GetIsTypeCrane(m_Worker.m_EngagedObject.m_TypeIdentifier))
		{
			requester = m_Worker.m_EngagedObject;
		}
		List<TileCoordObject> list = null;
		if (m_FullSearch)
		{
			list = identifierFromSaveName2 switch
			{
				ObjectType.HouseAny => PlotManager.Instance.GetObjectsOfTypes(Housing.m_Types, TopLeft, BottomRight, requester, identifierFromSaveName, actionType, globalVariable), 
				ObjectType.JunkAny => PlotManager.Instance.GetObjectsForBroom(TopLeft, BottomRight, requester, identifierFromSaveName, actionType, globalVariable), 
				_ => PlotManager.Instance.GetObjectsOfTypes(identifierFromSaveName2, TopLeft, BottomRight, requester, identifierFromSaveName, actionType, globalVariable), 
			};
		}
		else
		{
			list = identifierFromSaveName2 switch
			{
				ObjectType.HouseAny => PlotManager.Instance.GetObjectsOfTypes(Housing.m_Types, TopLeft, BottomRight, requester, identifierFromSaveName, actionType, globalVariable), 
				ObjectType.JunkAny => PlotManager.Instance.GetNearestObjectForBroom(TopLeft, BottomRight, requester, identifierFromSaveName, actionType, globalVariable), 
				_ => PlotManager.Instance.GetNearestObjectOfType(identifierFromSaveName2, TopLeft, BottomRight, requester, identifierFromSaveName, actionType, globalVariable), 
			};
			if (list.Count > 0)
			{
				m_FullSearch = true;
			}
		}
		if (list.Count > 0)
		{
			m_ReusableActionFromObject.Clone(Actionable.m_ReusableActionFromObject);
			m_Worker.RequestFind(list);
			return true;
		}
		return false;
	}

	private TileCoordObject GetBestObject()
	{
		TileCoordObject result = null;
		float num = 100000f;
		foreach (TileCoordObject foundObject in m_Worker.m_FoundObjects)
		{
			float num2 = 100000f;
			if (foundObject.m_TypeIdentifier == ObjectType.Folk)
			{
				num2 = foundObject.GetComponent<Folk>().m_Energy;
				if (num2 < num)
				{
					num = num2;
					result = foundObject;
				}
			}
			if (Housing.m_Types.Contains(foundObject.m_TypeIdentifier))
			{
				List<Folk> folks = foundObject.GetComponent<Housing>().m_Folks;
				if (folks != null && folks.Count > 0)
				{
					num2 = folks[0].m_Energy;
				}
			}
			if (Fueler.GetIsTypeFueler(foundObject.m_TypeIdentifier))
			{
				num2 = foundObject.GetComponent<Fueler>().m_Fuel;
			}
			if (foundObject.m_TypeIdentifier == ObjectType.Trough)
			{
				num2 = foundObject.GetComponent<Trough>().m_Hay;
			}
			if (foundObject.m_TypeIdentifier == ObjectType.TrainRefuellingStation)
			{
				num2 = ((!ToolFillable.GetIsTypeFillable(ObjectTypeList.Instance.GetIdentifierFromSaveName(GetGlobalVariable(4)))) ? foundObject.GetComponent<TrainRefuellingStation>().m_Fuel : foundObject.GetComponent<TrainRefuellingStation>().m_Water);
			}
			if (foundObject.m_TypeIdentifier == ObjectType.StationaryEngine)
			{
				num2 = ((!ToolFillable.GetIsTypeFillable(ObjectTypeList.Instance.GetIdentifierFromSaveName(GetGlobalVariable(4)))) ? ((float)foundObject.GetComponent<StationaryEngine>().m_Energy) : foundObject.GetComponent<StationaryEngine>().m_Water);
			}
			if (num2 < num)
			{
				num = num2;
				result = foundObject;
			}
		}
		return result;
	}

	private void InstructionFindNearestObject(WorkerScriptLocal Script, int Index)
	{
		m_Worker.SetBaggedObject(null);
		if (!DoFindNearest(Script, Index, FindNearestObject))
		{
			return;
		}
		m_FullSearch = false;
		TileCoordObject tileCoordObject = ((!m_BestSearch) ? m_Worker.m_FoundObjects[0] : GetBestObject());
		bool flag = false;
		if (tileCoordObject == null || BaggedManager.Instance.IsObjectBagged(tileCoordObject) || (ObjectTypeList.Instance.GetIsHoldable(tileCoordObject.m_TypeIdentifier) && !tileCoordObject.GetIsSavable()))
		{
			flag = true;
		}
		if (!flag)
		{
			ActionType actionFromObjectSafe = tileCoordObject.GetActionFromObjectSafe(m_ReusableActionFromObject);
			if (actionFromObjectSafe == ActionType.Total || actionFromObjectSafe == ActionType.Fail)
			{
				flag = true;
			}
		}
		if (flag)
		{
			m_InstructionFailed = true;
			return;
		}
		m_Worker.SetBaggedObject(tileCoordObject);
		TileCoord tileCoord = tileCoordObject.m_TileCoord;
		if (ObjectTypeList.Instance.GetIsBuilding(tileCoordObject.m_TypeIdentifier))
		{
			tileCoord = tileCoordObject.GetComponent<Building>().GetAccessPosition();
		}
		SetVariable(Script, Index, 1, tileCoord.x.ToString());
		SetVariable(Script, Index, 2, tileCoord.y.ToString());
		SetVariable(Script, Index, 3, tileCoordObject.m_UniqueID.ToString());
	}

	private void ProcessInstruction(WorkerScriptLocal Script)
	{
		int currentInstruction = Script.m_CurrentInstruction;
		WorkerInstruction.Instruction instruction = Script.m_Script.m_Instructions[currentInstruction].m_Instruction;
		m_InstructionFailed = false;
		m_InstructionProcessed = false;
		m_InstructionProcessing = false;
		if (instruction != WorkerInstruction.Instruction.MoveTo && instruction != WorkerInstruction.Instruction.MoveToLessOne && instruction != WorkerInstruction.Instruction.MoveToRange)
		{
			m_MoveAttempts = 0;
		}
		switch (instruction)
		{
		case WorkerInstruction.Instruction.If:
			InstructionIf(Script, currentInstruction);
			break;
		case WorkerInstruction.Instruction.Else:
			InstructionElse(Script, currentInstruction);
			break;
		case WorkerInstruction.Instruction.EndIf:
			InstructionEndIf(Script, currentInstruction);
			break;
		case WorkerInstruction.Instruction.GoTo:
			InstructionGoTo(Script, currentInstruction);
			return;
		case WorkerInstruction.Instruction.StartScript:
			InstructionStartScript(Script, currentInstruction);
			break;
		case WorkerInstruction.Instruction.End:
			InstructionEnd(Script, currentInstruction);
			break;
		case WorkerInstruction.Instruction.SetVariable:
			InstructionSetVariable(Script, currentInstruction);
			break;
		case WorkerInstruction.Instruction.AddVariable:
			InstructionAddVariable(Script, currentInstruction);
			break;
		case WorkerInstruction.Instruction.Wait:
			InstructionWait(Script, currentInstruction);
			break;
		case WorkerInstruction.Instruction.MoveDelay:
			InstructionMoveDelay(Script, currentInstruction);
			break;
		case WorkerInstruction.Instruction.JumpTo:
			InstructionJumpTo(Script, currentInstruction);
			break;
		case WorkerInstruction.Instruction.JumpIfZero:
			InstructionJumpIfZero(Script, currentInstruction);
			break;
		case WorkerInstruction.Instruction.JumpIfNotZero:
			InstructionJumpIfNotZero(Script, currentInstruction);
			break;
		case WorkerInstruction.Instruction.PushFailJump:
			InstructionPushFailJump(Script, currentInstruction);
			break;
		case WorkerInstruction.Instruction.PopFailJump:
			InstructionPopFailJump(Script, currentInstruction);
			break;
		case WorkerInstruction.Instruction.MoveTo:
			InstructionMoveTo(Script, currentInstruction);
			break;
		case WorkerInstruction.Instruction.MoveToLessOne:
			InstructionMoveToLessOne(Script, currentInstruction);
			break;
		case WorkerInstruction.Instruction.MoveToRange:
			InstructionMoveToRange(Script, currentInstruction);
			break;
		case WorkerInstruction.Instruction.MoveForwards:
			InstructionMoveForwards(Script, currentInstruction);
			break;
		case WorkerInstruction.Instruction.MoveBackwards:
			InstructionMoveBackwards(Script, currentInstruction);
			break;
		case WorkerInstruction.Instruction.MoveDirection:
			InstructionMoveDirection(Script, currentInstruction);
			break;
		case WorkerInstruction.Instruction.MoveForward:
			InstructionMoveForward(Script, currentInstruction);
			break;
		case WorkerInstruction.Instruction.Turn:
			InstructionTurn(Script, currentInstruction);
			break;
		case WorkerInstruction.Instruction.SetTool:
			InstructionSetTool(Script, currentInstruction);
			break;
		case WorkerInstruction.Instruction.UseInHands:
			InstructionUseInHands(Script, currentInstruction);
			break;
		case WorkerInstruction.Instruction.DropAll:
			InstructionDropAll(Script, currentInstruction);
			break;
		case WorkerInstruction.Instruction.Pickup:
			InstructionPickup(Script, currentInstruction);
			break;
		case WorkerInstruction.Instruction.Make:
			InstructionMake(Script, currentInstruction);
			break;
		case WorkerInstruction.Instruction.Recharge:
			InstructionRecharge(Script, currentInstruction);
			break;
		case WorkerInstruction.Instruction.Shout:
			InstructionShout(Script, currentInstruction);
			break;
		case WorkerInstruction.Instruction.TakeResource:
			InstructionTakeResource(Script, currentInstruction);
			break;
		case WorkerInstruction.Instruction.AddResource:
			InstructionAddResource(Script, currentInstruction);
			break;
		case WorkerInstruction.Instruction.StowObject:
			InstructionStowObject(Script, currentInstruction);
			break;
		case WorkerInstruction.Instruction.RecallObject:
			InstructionRecallObject(Script, currentInstruction);
			break;
		case WorkerInstruction.Instruction.CycleObject:
			InstructionCycleObject(Script, currentInstruction);
			break;
		case WorkerInstruction.Instruction.SwapObject:
			InstructionSwapObject(Script, currentInstruction);
			break;
		case WorkerInstruction.Instruction.CheckHandsFull:
			InstructionCheckHandsFull(Script, currentInstruction);
			break;
		case WorkerInstruction.Instruction.CheckHandsNotFull:
			InstructionCheckHandsNotFull(Script, currentInstruction);
			break;
		case WorkerInstruction.Instruction.CheckHandsEmpty:
			InstructionCheckHandsEmpty(Script, currentInstruction);
			break;
		case WorkerInstruction.Instruction.CheckHandsNotEmpty:
			InstructionCheckHandsNotEmpty(Script, currentInstruction);
			break;
		case WorkerInstruction.Instruction.CheckInventoryFull:
			InstructionCheckInventoryFull(Script, currentInstruction);
			break;
		case WorkerInstruction.Instruction.CheckInventoryNotFull:
			InstructionCheckInventoryNotFull(Script, currentInstruction);
			break;
		case WorkerInstruction.Instruction.CheckInventoryEmpty:
			InstructionCheckInventoryEmpty(Script, currentInstruction);
			break;
		case WorkerInstruction.Instruction.CheckInventoryNotEmpty:
			InstructionCheckInventoryNotEmpty(Script, currentInstruction);
			break;
		case WorkerInstruction.Instruction.CheckBuildingFull:
			InstructionCheckBuildingFull(Script, currentInstruction);
			break;
		case WorkerInstruction.Instruction.CheckBuildingNotFull:
			InstructionCheckBuildingNotFull(Script, currentInstruction);
			break;
		case WorkerInstruction.Instruction.CheckBuildingEmpty:
			InstructionCheckBuildingEmpty(Script, currentInstruction);
			break;
		case WorkerInstruction.Instruction.CheckBuildingNotEmpty:
			InstructionCheckBuildingNotEmpty(Script, currentInstruction);
			break;
		case WorkerInstruction.Instruction.CheckHeldObjectFull:
			InstructionCheckHeldObjectFull(Script, currentInstruction);
			break;
		case WorkerInstruction.Instruction.CheckHeldObjectNotFull:
			InstructionCheckHeldObjectNotFull(Script, currentInstruction);
			break;
		case WorkerInstruction.Instruction.CheckHeldObjectEmpty:
			InstructionCheckHeldObjectEmpty(Script, currentInstruction);
			break;
		case WorkerInstruction.Instruction.CheckHeldObjectNotEmpty:
			InstructionCheckHeldObjectNotEmpty(Script, currentInstruction);
			break;
		case WorkerInstruction.Instruction.CheckHear:
			InstructionCheckHear(Script, currentInstruction);
			break;
		case WorkerInstruction.Instruction.EngageObject:
			InstructionEngageObject(Script, currentInstruction);
			break;
		case WorkerInstruction.Instruction.DisengageObject:
			InstructionDisengageObject(Script, currentInstruction);
			break;
		case WorkerInstruction.Instruction.SetValue:
			InstructionSetValue(Script, currentInstruction);
			break;
		case WorkerInstruction.Instruction.GetValue:
			InstructionGetValue(Script, currentInstruction);
			break;
		case WorkerInstruction.Instruction.GetObjectTileCoords:
			InstructionGetObjectTileCoords(Script, currentInstruction);
			break;
		case WorkerInstruction.Instruction.FindNearestTile:
			InstructionFindNearestTile(Script, currentInstruction);
			break;
		case WorkerInstruction.Instruction.FindNearestObject:
			InstructionFindNearestObject(Script, currentInstruction);
			break;
		}
		if (!m_InstructionFailed && !m_Paused)
		{
			if (!m_InstructionProcessed)
			{
				if (m_Worker.GetIsDoingSomething())
				{
					m_WaitingForNonActive = true;
					return;
				}
				Script.m_CurrentInstruction++;
				m_Worker.CheckRequestPause();
			}
		}
		else if (m_InstructionFailed)
		{
			m_Worker.CheckRequestPause();
		}
	}

	public void Pause(bool Pause)
	{
		m_Paused = Pause;
	}

	private void CheckWaitingForActive()
	{
		if (m_WaitingForNonActive)
		{
			m_WaitingForNonActive = false;
			if (m_ActionSuccess)
			{
				m_CurrentScript.m_CurrentInstruction++;
				m_InstructionProcessed = true;
			}
			else
			{
				m_InstructionFailed = true;
			}
			m_CheckRequestPause = true;
		}
	}

	public void UpdateScript()
	{
		if (m_CheckRequestPause)
		{
			m_CheckRequestPause = false;
			m_Worker.CheckRequestPause();
		}
		if (!m_Paused && !m_ActionActive && m_CurrentScript != null)
		{
			CheckWaitingForActive();
			WorkerScriptLocal currentScript = m_CurrentScript;
			if (currentScript.m_CurrentInstruction >= currentScript.m_Script.m_Instructions.Count)
			{
				StopCurrent();
			}
			else
			{
				ProcessInstruction(currentScript);
			}
		}
	}
}
