using System.Collections.Generic;
using SimpleJSON;

public struct WorkerInstruction
{
	public enum Instruction
	{
		If,
		Else,
		EndIf,
		GoTo,
		StartScript,
		End,
		SetVariable,
		AddVariable,
		Wait,
		MoveDelay,
		Label,
		JumpTo,
		JumpIfZero,
		JumpIfNotZero,
		PushFailJump,
		PopFailJump,
		MoveTo,
		MoveToLessOne,
		MoveToRange,
		MoveDirection,
		MoveForward,
		TurnAt,
		StopAt,
		Turn,
		MoveForwards,
		MoveBackwards,
		SetTool,
		UseInHands,
		DropAll,
		Pickup,
		Make,
		Recharge,
		Shout,
		TakeResource,
		AddResource,
		StowObject,
		RecallObject,
		CycleObject,
		SwapObject,
		CheckHandsFull,
		CheckHandsNotFull,
		CheckHandsEmpty,
		CheckHandsNotEmpty,
		CheckInventoryFull,
		CheckInventoryNotFull,
		CheckInventoryEmpty,
		CheckInventoryNotEmpty,
		CheckBuildingFull,
		CheckBuildingNotFull,
		CheckBuildingEmpty,
		CheckBuildingNotEmpty,
		CheckHeldObjectFull,
		CheckHeldObjectNotFull,
		CheckHeldObjectEmpty,
		CheckHeldObjectNotEmpty,
		CheckHear,
		EngageObject,
		DisengageObject,
		SetValue,
		GetValue,
		GetObjectTileCoords,
		FindNearestTile,
		FindNearestObject,
		Total
	}

	public static string[] m_InstructionNames;

	public Instruction m_Instruction;

	public string m_Variable1;

	public string m_Variable2;

	public string m_Variable3;

	public string m_Variable4;

	private static Dictionary<string, Instruction> m_NameToInstruction;

	public WorkerInstruction(WorkerInstruction OldInstruction)
	{
		m_Instruction = OldInstruction.m_Instruction;
		m_Variable1 = OldInstruction.m_Variable1;
		m_Variable2 = OldInstruction.m_Variable2;
		m_Variable3 = OldInstruction.m_Variable3;
		m_Variable4 = OldInstruction.m_Variable4;
	}

	public void Set(Instruction NewInstruction)
	{
		m_Instruction = NewInstruction;
		m_Variable1 = "";
		m_Variable2 = "";
		m_Variable3 = "";
		m_Variable4 = "";
	}

	public static void Init()
	{
		m_NameToInstruction = new Dictionary<string, Instruction>();
		m_InstructionNames = new string[63];
		for (int i = 0; i < 63; i++)
		{
			string[] instructionNames = m_InstructionNames;
			int num = i;
			Instruction instruction = (Instruction)i;
			instructionNames[num] = instruction.ToString();
			m_NameToInstruction.Add(m_InstructionNames[i], (Instruction)i);
		}
	}

	public static Instruction GetInstructionFromName(string Name)
	{
		Instruction value = Instruction.Total;
		if (!m_NameToInstruction.TryGetValue(Name, out value))
		{
			value = Instruction.Total;
		}
		return value;
	}

	public void Save(JSONNode Node)
	{
		JSONUtils.Set(Node, "Ins", m_InstructionNames[(int)m_Instruction]);
		JSONUtils.Set(Node, "Var1", m_Variable1);
		JSONUtils.Set(Node, "Var2", m_Variable2);
		JSONUtils.Set(Node, "Var3", m_Variable3);
		JSONUtils.Set(Node, "Var4", m_Variable4);
	}

	public bool Load(JSONNode Node)
	{
		m_Instruction = GetInstructionFromName(JSONUtils.GetAsString(Node, "Ins", ""));
		if (m_Instruction != Instruction.Total)
		{
			m_Variable1 = JSONUtils.GetAsString(Node, "Var1", "");
			m_Variable2 = JSONUtils.GetAsString(Node, "Var2", "");
			m_Variable3 = JSONUtils.GetAsString(Node, "Var3", "");
			m_Variable4 = JSONUtils.GetAsString(Node, "Var4", "");
			return true;
		}
		return false;
	}

	public void PostLoad()
	{
		OldFileUtils.CheckScriptInstruction(ref this);
	}

	public string GetToString()
	{
		string text = "";
		text += m_InstructionNames[(int)m_Instruction];
		if (m_Variable1 != "")
		{
			if (m_Instruction == Instruction.TakeResource || m_Instruction == Instruction.AddResource || m_Instruction == Instruction.EngageObject || m_Instruction == Instruction.DisengageObject)
			{
				if (m_Variable1.Length < 6 || m_Variable1.Substring(0, 6) != "Global")
				{
					int iD = int.Parse(m_Variable1);
					BaseClass objectFromUniqueID = ObjectTypeList.Instance.GetObjectFromUniqueID(iD, ErrorCheck: false);
					if (objectFromUniqueID == null)
					{
						return "";
					}
					text = text + " " + ObjectTypeList.Instance.GetHumanReadableNameFromIdentifier(objectFromUniqueID.m_TypeIdentifier);
				}
				else
				{
					text = text + " " + m_Variable1;
				}
			}
			else
			{
				text = text + " " + m_Variable1;
			}
		}
		if (m_Variable2 != "")
		{
			text = text + " " + m_Variable2;
		}
		if (m_Variable3 != "")
		{
			text = text + " " + m_Variable3;
		}
		if (m_Variable4 != "")
		{
			text = text + " " + m_Variable4;
		}
		return text;
	}

	public void SetFromString()
	{
	}

	public bool IsGetNearest()
	{
		if (m_Instruction == Instruction.FindNearestObject || m_Instruction == Instruction.FindNearestTile)
		{
			return true;
		}
		return false;
	}
}
