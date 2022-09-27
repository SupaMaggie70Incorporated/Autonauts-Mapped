using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;

public class HighInstruction
{
	public enum Type
	{
		Forever,
		Repeat,
		Wait,
		If,
		IfElse,
		ExitRepeat,
		MoveTo,
		MoveToLessOne,
		MoveToRange,
		MoveForwards,
		MoveBackwards,
		TurnAt,
		StopAt,
		DropAll,
		Pickup,
		TakeResource,
		AddResource,
		StowObject,
		RecallObject,
		CycleObject,
		SwapObject,
		UseInHands,
		Make,
		Recharge,
		Shout,
		EngageObject,
		DisengageObject,
		SetValue,
		FindNearestTile,
		FindNearestObject,
		Total
	}

	public enum ConditionType
	{
		HandsFull,
		HandsNotFull,
		HandsEmpty,
		HandsNotEmpty,
		InventoryFull,
		InventoryNotFull,
		InventoryEmpty,
		InventoryNotEmpty,
		BuildingFull,
		BuildingNotFull,
		BuildingEmpty,
		BuildingNotEmpty,
		HeldObjectFull,
		HeldObjectNotFull,
		HeldObjectEmpty,
		HeldObjectNotEmpty,
		Times,
		Forever,
		Hear,
		Total
	}

	public enum FindType
	{
		Full,
		Even,
		HStripes,
		VStripes,
		Checkers,
		Total
	}

	public Type m_Type;

	public static HighInstructionInfo[] m_Info = new HighInstructionInfo[30]
	{
		new HighInstructionInfo(HighInstructionInfo.Category.Control, "InstructionForever"),
		new HighInstructionInfo(HighInstructionInfo.Category.Control, "InstructionRepeat"),
		new HighInstructionInfo(HighInstructionInfo.Category.Control, "InstructionWait"),
		new HighInstructionInfo(HighInstructionInfo.Category.Control, "InstructionIf"),
		new HighInstructionInfo(HighInstructionInfo.Category.Control, "InstructionIfElse"),
		new HighInstructionInfo(HighInstructionInfo.Category.Control, "InstructionExitRepeat"),
		new HighInstructionInfo(HighInstructionInfo.Category.Movement, "InstructionMove"),
		new HighInstructionInfo(HighInstructionInfo.Category.Movement, "InstructionMoveLessOne"),
		new HighInstructionInfo(HighInstructionInfo.Category.Movement, "InstructionMoveRange"),
		new HighInstructionInfo(HighInstructionInfo.Category.Movement, "InstructionMoveForwards"),
		new HighInstructionInfo(HighInstructionInfo.Category.Movement, "InstructionMoveBackwards"),
		new HighInstructionInfo(HighInstructionInfo.Category.Movement, "InstructionTurnAt"),
		new HighInstructionInfo(HighInstructionInfo.Category.Movement, "InstructionStopAt"),
		new HighInstructionInfo(HighInstructionInfo.Category.Item, "InstructionDropAll"),
		new HighInstructionInfo(HighInstructionInfo.Category.Item, "InstructionPickup"),
		new HighInstructionInfo(HighInstructionInfo.Category.Item, "InstructionTakeResource"),
		new HighInstructionInfo(HighInstructionInfo.Category.Item, "InstructionAddResource"),
		new HighInstructionInfo(HighInstructionInfo.Category.Inventory, "InstructionStowObject"),
		new HighInstructionInfo(HighInstructionInfo.Category.Inventory, "InstructionRecallObject"),
		new HighInstructionInfo(HighInstructionInfo.Category.Inventory, "InstructionCycleObject"),
		new HighInstructionInfo(HighInstructionInfo.Category.Inventory, "InstructionSwapObject"),
		new HighInstructionInfo(HighInstructionInfo.Category.Action, "InstructionUseInHands"),
		new HighInstructionInfo(HighInstructionInfo.Category.Action, "InstructionMake"),
		new HighInstructionInfo(HighInstructionInfo.Category.Action, "InstructionRecharge"),
		new HighInstructionInfo(HighInstructionInfo.Category.Action, "InstructionShout"),
		new HighInstructionInfo(HighInstructionInfo.Category.Action, "InstructionEngageObject"),
		new HighInstructionInfo(HighInstructionInfo.Category.Action, "InstructionDisengageObject"),
		new HighInstructionInfo(HighInstructionInfo.Category.Action, "InstructionSetValue"),
		new HighInstructionInfo(HighInstructionInfo.Category.Find, "InstructionFindNearestTile"),
		new HighInstructionInfo(HighInstructionInfo.Category.Find, "InstructionFindNearestObject")
	};

	[HideInInspector]
	public HighInstruction m_Parent;

	public List<HighInstruction> m_Children;

	public List<HighInstruction> m_Children2;

	[HideInInspector]
	public HudInstruction m_HudParent;

	[HideInInspector]
	public string m_Argument;

	[HideInInspector]
	public ActionInfo m_ActionInfo;

	[HideInInspector]
	public int m_ScriptLineNumber;

	public static ConditionType[] m_RepeatTypes = new ConditionType[19]
	{
		ConditionType.HandsFull,
		ConditionType.HandsNotFull,
		ConditionType.HandsEmpty,
		ConditionType.HandsNotEmpty,
		ConditionType.InventoryFull,
		ConditionType.InventoryNotFull,
		ConditionType.InventoryEmpty,
		ConditionType.InventoryNotEmpty,
		ConditionType.BuildingFull,
		ConditionType.BuildingNotFull,
		ConditionType.BuildingEmpty,
		ConditionType.BuildingNotEmpty,
		ConditionType.HeldObjectFull,
		ConditionType.HeldObjectNotFull,
		ConditionType.HeldObjectEmpty,
		ConditionType.HeldObjectNotEmpty,
		ConditionType.Times,
		ConditionType.Forever,
		ConditionType.Hear
	};

	private static string[] m_RepeatTypeNames;

	private static string[] m_RepeatTypeNamesFull;

	public static ConditionType[] m_IfTypes = new ConditionType[17]
	{
		ConditionType.HandsFull,
		ConditionType.HandsNotFull,
		ConditionType.HandsEmpty,
		ConditionType.HandsNotEmpty,
		ConditionType.InventoryFull,
		ConditionType.InventoryNotFull,
		ConditionType.InventoryEmpty,
		ConditionType.InventoryNotEmpty,
		ConditionType.BuildingFull,
		ConditionType.BuildingNotFull,
		ConditionType.BuildingEmpty,
		ConditionType.BuildingNotEmpty,
		ConditionType.HeldObjectFull,
		ConditionType.HeldObjectNotFull,
		ConditionType.HeldObjectEmpty,
		ConditionType.HeldObjectNotEmpty,
		ConditionType.Hear
	};

	private static string[] m_IfTypeNames;

	private static string[] m_IfTypeNamesFull;

	private static string[] m_FindTypeNames;

	public static void Init()
	{
		m_RepeatTypeNames = new string[m_RepeatTypes.Length];
		m_RepeatTypeNamesFull = new string[19];
		for (int i = 0; i < m_RepeatTypes.Length; i++)
		{
			m_RepeatTypeNames[i] = "Repeat" + m_RepeatTypes[i];
			int num = (int)m_RepeatTypes[i];
			m_RepeatTypeNamesFull[num] = m_RepeatTypeNames[i];
		}
		m_IfTypeNames = new string[m_IfTypes.Length];
		m_IfTypeNamesFull = new string[19];
		for (int j = 0; j < m_IfTypes.Length; j++)
		{
			m_IfTypeNames[j] = "If" + m_IfTypes[j];
			int num2 = (int)m_IfTypes[j];
			m_IfTypeNamesFull[num2] = m_IfTypeNames[j];
		}
		m_FindTypeNames = new string[5];
		for (int k = 0; k < 5; k++)
		{
			string[] findTypeNames = m_FindTypeNames;
			int num3 = k;
			FindType findType = (FindType)k;
			findTypeNames[num3] = findType.ToString();
		}
	}

	public HighInstruction(Type NewType, ActionInfo Info)
	{
		m_Type = NewType;
		if (Info == null)
		{
			m_ActionInfo = new ActionInfo(ActionType.Total, default(TileCoord));
		}
		else
		{
			m_ActionInfo = new ActionInfo(Info);
		}
		m_Children = new List<HighInstruction>();
		m_Children2 = new List<HighInstruction>();
		m_Parent = null;
		m_HudParent = null;
		m_Argument = "";
		m_ScriptLineNumber = -1;
		SetDefaultValues();
	}

	private void SetDefaultValues()
	{
		if (m_Type == Type.Wait)
		{
			m_ActionInfo.m_Value = "1";
		}
		if (m_Type == Type.Shout)
		{
			m_ActionInfo.m_Value = "Go!";
		}
		if (GetIsGetNearest())
		{
			GameStateEditArea.MakeAreaInVisiblePlots(m_ActionInfo.m_Position, out var TopLeft, out var BottomRight, 5);
			SetFindNearestArea(TopLeft, BottomRight, FindType.Full);
		}
	}

	public HighInstruction(HighInstruction OldInstruction)
	{
		m_Type = OldInstruction.m_Type;
		m_ActionInfo = new ActionInfo(OldInstruction.m_ActionInfo);
		m_Argument = OldInstruction.m_Argument;
		m_ScriptLineNumber = OldInstruction.m_ScriptLineNumber;
		m_Children = new List<HighInstruction>();
		m_Children2 = new List<HighInstruction>();
		m_Parent = null;
		m_HudParent = null;
	}

	public bool CanTakeChildren()
	{
		if (m_Type == Type.Forever || m_Type == Type.Repeat || m_Type == Type.If || m_Type == Type.IfElse)
		{
			return true;
		}
		return false;
	}

	public void Save(JSONNode Node)
	{
		JSONUtils.Set(Node, "Type", m_Info[(int)m_Type].m_Name);
		JSONUtils.Set(Node, "ArgName", m_Argument);
		JSONUtils.Set(Node, "Line", m_ScriptLineNumber);
		if (m_ActionInfo != null)
		{
			JSONUtils.Set(Node, "OT", ObjectTypeList.Instance.GetSaveNameFromIdentifier(m_ActionInfo.m_ObjectType));
			JSONUtils.Set(Node, "UID", m_ActionInfo.m_ObjectUID);
			JSONUtils.Set(Node, "X", m_ActionInfo.m_Position.x);
			JSONUtils.Set(Node, "Y", m_ActionInfo.m_Position.y);
			JSONUtils.Set(Node, "V1", m_ActionInfo.m_Value);
			JSONUtils.Set(Node, "V2", m_ActionInfo.m_Value2);
			JSONUtils.Set(Node, "A", m_ActionInfo.m_Action.ToString());
			JSONUtils.Set(Node, "R", m_ActionInfo.m_ActionRequirement);
			JSONUtils.Set(Node, "AT", (int)m_ActionInfo.m_ActionType);
			JSONUtils.Set(Node, "AOT", ObjectTypeList.Instance.GetSaveNameFromIdentifier(m_ActionInfo.m_ActionObjectType));
		}
		if (m_Children.Count > 0)
		{
			JSONArray jSONArray = (JSONArray)(Node["Children"] = new JSONArray());
			int num = 0;
			foreach (HighInstruction child in m_Children)
			{
				jSONArray[num] = new JSONObject();
				child.Save(jSONArray[num]);
				num++;
			}
		}
		if (m_Children2.Count <= 0)
		{
			return;
		}
		JSONArray jSONArray2 = (JSONArray)(Node["Children2"] = new JSONArray());
		int num2 = 0;
		foreach (HighInstruction item in m_Children2)
		{
			jSONArray2[num2] = new JSONObject();
			item.Save(jSONArray2[num2]);
			num2++;
		}
	}

	private Type GetInstructionFromName(string Name)
	{
		for (int i = 0; i < m_Info.Length; i++)
		{
			if (Name == m_Info[i].m_Name)
			{
				return (Type)i;
			}
		}
		ErrorMessage.LogError("Couldn't find HighInstruction : " + Name);
		return Type.Total;
	}

	public void Load(JSONNode Node)
	{
		m_Type = GetInstructionFromName(JSONUtils.GetAsString(Node, "Type", ""));
		if (m_Type == Type.Forever)
		{
			m_Type = Type.Repeat;
			m_Argument = GetRepeatNameFromConditionType(ConditionType.Forever);
		}
		else
		{
			m_Argument = JSONUtils.GetAsString(Node, "ArgName", "");
		}
		m_ScriptLineNumber = JSONUtils.GetAsInt(Node, "Line", 0);
		m_ActionInfo = new ActionInfo(ActionType.Total, new TileCoord(0, 0));
		if (JSONUtils.GetAsString(Node, "OT", "") != "")
		{
			m_ActionInfo.m_ObjectType = ObjectTypeList.Instance.GetIdentifierFromSaveName(JSONUtils.GetAsString(Node, "OT", ""), Check: false);
			m_ActionInfo.m_ObjectUID = JSONUtils.GetAsInt(Node, "UID", 0);
			m_ActionInfo.m_Position.x = JSONUtils.GetAsInt(Node, "X", 0);
			m_ActionInfo.m_Position.y = JSONUtils.GetAsInt(Node, "Y", 0);
			m_ActionInfo.m_Value = JSONUtils.GetAsString(Node, "V1", "");
			m_ActionInfo.m_Value2 = JSONUtils.GetAsString(Node, "V2", "");
			m_ActionInfo.m_ActionRequirement = JSONUtils.GetAsString(Node, "R", "");
			m_ActionInfo.m_ActionType = (AFO.AT)JSONUtils.GetAsInt(Node, "AT", 0);
			m_ActionInfo.m_ActionObjectType = ObjectTypeList.Instance.GetIdentifierFromSaveName(JSONUtils.GetAsString(Node, "AOT", ""), Check: false);
			m_ActionInfo.m_Action = Actionable.GetActionFromName(JSONUtils.GetAsString(Node, "A", ""));
		}
		JSONNode jSONNode = Node["Children"];
		if (jSONNode != null && !jSONNode.IsNull)
		{
			for (int i = 0; i < jSONNode.Count; i++)
			{
				JSONNode asObject = jSONNode[i].AsObject;
				HighInstruction highInstruction = new HighInstruction(Type.Total, null);
				highInstruction.Load(asObject);
				highInstruction.m_Parent = this;
				m_Children.Add(highInstruction);
			}
		}
		jSONNode = Node["Children2"];
		if (jSONNode != null && !jSONNode.IsNull)
		{
			for (int j = 0; j < jSONNode.Count; j++)
			{
				JSONNode asObject2 = jSONNode[j].AsObject;
				HighInstruction highInstruction2 = new HighInstruction(Type.Total, null);
				highInstruction2.Load(asObject2);
				highInstruction2.m_Parent = this;
				m_Children2.Add(highInstruction2);
			}
		}
	}

	public void PostLoad()
	{
		OldFileUtils.CheckHighInstructionActioninfo(this);
		foreach (HighInstruction child in m_Children)
		{
			child.PostLoad();
		}
		foreach (HighInstruction item in m_Children2)
		{
			item.PostLoad();
		}
	}

	public bool GetIsGetNearest()
	{
		if (m_Type == Type.FindNearestTile || m_Type == Type.FindNearestObject)
		{
			return true;
		}
		return false;
	}

	private static void CopyChildren(List<HighInstruction> Children, HighInstruction Parent, List<HighInstruction> ParentsChildren)
	{
		for (int i = 0; i < Children.Count; i++)
		{
			HighInstruction highInstruction = new HighInstruction(Children[i]);
			highInstruction.m_Parent = Parent;
			CopyChildren(Children[i].m_Children, highInstruction, highInstruction.m_Children);
			CopyChildren(Children[i].m_Children2, highInstruction, highInstruction.m_Children2);
			ParentsChildren.Add(highInstruction);
		}
	}

	public static List<HighInstruction> Copy(List<HighInstruction> Instructions)
	{
		List<HighInstruction> list = new List<HighInstruction>();
		for (int i = 0; i < Instructions.Count; i++)
		{
			HighInstruction highInstruction = new HighInstruction(Instructions[i]);
			CopyChildren(Instructions[i].m_Children, highInstruction, highInstruction.m_Children);
			CopyChildren(Instructions[i].m_Children2, highInstruction, highInstruction.m_Children2);
			list.Add(highInstruction);
		}
		return list;
	}

	public static int GetInstructionCount(List<HighInstruction> Instructions)
	{
		int num = Instructions.Count;
		foreach (HighInstruction Instruction in Instructions)
		{
			num += GetInstructionCount(Instruction.m_Children);
			num += GetInstructionCount(Instruction.m_Children2);
		}
		return num;
	}

	public bool Contains(HighInstruction NewInstruction)
	{
		if (m_Children.Contains(NewInstruction) || m_Children2.Contains(NewInstruction))
		{
			return true;
		}
		foreach (HighInstruction child in m_Children)
		{
			if (child.Contains(NewInstruction))
			{
				return true;
			}
		}
		foreach (HighInstruction item in m_Children2)
		{
			if (item.Contains(NewInstruction))
			{
				return true;
			}
		}
		return false;
	}

	public bool ChangeUIDLocation(int UID, TileCoord NewPosition, bool Found)
	{
		if (m_ActionInfo.m_ObjectUID == UID)
		{
			m_ActionInfo.m_Position = NewPosition;
			Found = true;
		}
		foreach (HighInstruction child in m_Children)
		{
			Found = child.ChangeUIDLocation(UID, NewPosition, Found);
		}
		foreach (HighInstruction item in m_Children2)
		{
			Found = item.ChangeUIDLocation(UID, NewPosition, Found);
		}
		return Found;
	}

	public bool ChangeUID(int OldUID, int NewUID, bool Found)
	{
		if (m_ActionInfo.m_ObjectUID == OldUID)
		{
			m_ActionInfo.m_ObjectUID = NewUID;
			Found = true;
		}
		foreach (HighInstruction child in m_Children)
		{
			Found = child.ChangeUID(OldUID, NewUID, Found);
		}
		foreach (HighInstruction item in m_Children2)
		{
			Found = item.ChangeUID(OldUID, NewUID, Found);
		}
		return Found;
	}

	public void ScaleAreaIndicator(bool Up)
	{
		if ((bool)m_HudParent)
		{
			m_HudParent.ScaleAreaIndicator(Up);
		}
		foreach (HighInstruction child in m_Children)
		{
			child.ScaleAreaIndicator(Up);
		}
		foreach (HighInstruction item in m_Children2)
		{
			item.ScaleAreaIndicator(Up);
		}
	}

	public void CancelScale()
	{
		if ((bool)m_HudParent)
		{
			m_HudParent.CancelScale();
		}
		foreach (HighInstruction child in m_Children)
		{
			child.CancelScale();
		}
		foreach (HighInstruction item in m_Children2)
		{
			item.CancelScale();
		}
	}

	public void SetVisible(bool Visible)
	{
		if ((bool)m_HudParent)
		{
			m_HudParent.SetVisible(Visible);
		}
		foreach (HighInstruction child in m_Children)
		{
			child.SetVisible(Visible);
		}
		foreach (HighInstruction item in m_Children2)
		{
			item.SetVisible(Visible);
		}
	}

	public void SetFindNearestArea(TileCoord TopLeft, TileCoord BottomRight, FindType NewFindType)
	{
		int num = TopLeft.x;
		if (num < 0)
		{
			num = 0;
		}
		int num2 = TopLeft.y;
		if (num2 < 0)
		{
			num2 = 0;
		}
		int num3 = BottomRight.x;
		if (num3 >= TileManager.Instance.m_TilesWide)
		{
			num3 = TileManager.Instance.m_TilesWide - 1;
		}
		int num4 = BottomRight.y;
		if (num4 >= TileManager.Instance.m_TilesHigh)
		{
			num4 = TileManager.Instance.m_TilesHigh - 1;
		}
		m_Argument = num + " " + num2 + " " + num3 + " " + num4 + " " + NewFindType;
	}

	private string GetObjectName()
	{
		ObjectType objectType = m_ActionInfo.m_ObjectType;
		string text = "";
		if ((bool)m_ActionInfo.m_Object)
		{
			objectType = m_ActionInfo.m_Object.m_TypeIdentifier;
		}
		if (objectType == ObjectType.Plot)
		{
			text = ((!(m_ActionInfo.m_ActionRequirement != "")) ? (m_ActionInfo.m_Position.x + "," + m_ActionInfo.m_Position.y) : TextManager.Instance.Get(m_ActionInfo.m_ActionRequirement));
		}
		else if (Storage.GetIsTypeStorage(objectType) || (Converter.GetIsTypeConverter(objectType) && objectType != ObjectType.ConverterFoundation) || ResearchStation.GetIsTypeResearchStation(objectType) || TrainTrackStop.GetIsTypeTrainTrackStop(objectType) || TrainTrackPoints.GetIsTypeTrainTrackPoints(objectType) || Aquarium.GetIsTypeAquiarium(objectType) || objectType == ObjectType.SpacePort || objectType == ObjectType.TranscendBuilding || objectType == ObjectType.StoneHeads)
		{
			BaseClass objectFromUniqueID = ObjectTypeList.Instance.GetObjectFromUniqueID(m_ActionInfo.m_ObjectUID, ErrorCheck: false);
			text = ((!objectFromUniqueID || Sign.GetIsTypeSign(objectFromUniqueID.m_TypeIdentifier)) ? ObjectTypeList.Instance.GetHumanReadableNameFromIdentifier(objectType) : objectFromUniqueID.GetHumanReadableName());
		}
		else if (objectType != ObjectTypeList.m_Total)
		{
			text = ObjectTypeList.Instance.GetHumanReadableNameFromIdentifier(objectType);
		}
		if (m_ActionInfo.m_ActionRequirement != "" && objectType != ObjectType.Plot)
		{
			string text2 = TextManager.Instance.Get(m_ActionInfo.m_ActionRequirement);
			text = text + " (" + text2 + ")";
		}
		return text;
	}

	private string GetNearestObjectInstruction()
	{
		return GetObjectName();
	}

	private string GetCycleObjectInstruction()
	{
		if (m_ActionInfo.m_ObjectType != ObjectTypeList.m_Total)
		{
			return m_ActionInfo.m_Value;
		}
		if (m_ActionInfo.m_Value == "Up")
		{
			return TextManager.Instance.Get("InstructionCycleUp");
		}
		if (m_ActionInfo.m_Value == "Down")
		{
			return TextManager.Instance.Get("InstructionCycleDown");
		}
		return "";
	}

	private string GetSetValueInstruction()
	{
		string text = TextManager.Instance.Get("SetValueSet") + " ";
		text = text + TextManager.Instance.Get(m_ActionInfo.m_Value) + " ";
		text = text + TextManager.Instance.Get("SetValueTo") + " ";
		if (ObjectTypeList.Instance.GetObjectFromUniqueID(m_ActionInfo.m_ObjectUID, ErrorCheck: false) == null)
		{
			Debug.Log("HighInstruction.GetSetValueInstruction : Couldn't find object with UID " + m_ActionInfo.m_ObjectUID);
		}
		else
		{
			ObjectType identifierFromSaveName = ObjectTypeList.Instance.GetIdentifierFromSaveName(m_ActionInfo.m_Value2);
			text += ObjectTypeList.Instance.GetHumanReadableNameFromIdentifier(identifierFromSaveName);
		}
		return text;
	}

	private string GetActionObjectName()
	{
		if (MyTool.GetType(m_ActionInfo.m_ActionObjectType) != MyTool.Type.Total)
		{
			return MyTool.GetTypeName(m_ActionInfo.m_ActionObjectType);
		}
		return "";
	}

	private string GetUseInHandsInstruction()
	{
		return GetActionObjectName();
	}

	public string GetHumanReadableInstruction()
	{
		string val = "";
		if (m_ActionInfo != null)
		{
			switch (m_Type)
			{
			case Type.MoveTo:
			case Type.MoveToLessOne:
			case Type.MoveToRange:
			case Type.TurnAt:
			case Type.StopAt:
			case Type.Pickup:
			case Type.TakeResource:
			case Type.AddResource:
			case Type.EngageObject:
			case Type.DisengageObject:
			case Type.FindNearestTile:
				val = GetObjectName();
				break;
			case Type.FindNearestObject:
				val = GetNearestObjectInstruction();
				break;
			case Type.CycleObject:
				val = GetCycleObjectInstruction();
				break;
			case Type.SetValue:
				val = GetSetValueInstruction();
				break;
			case Type.UseInHands:
				val = GetUseInHandsInstruction();
				break;
			}
		}
		HighInstructionInfo highInstructionInfo = m_Info[(int)m_Type];
		return TextManager.Instance.Get(highInstructionInfo.m_Name, val);
	}

	public FindType GetFindType()
	{
		char[] separator = new char[1] { ' ' };
		string[] array = m_Argument.Split(separator);
		FindType result = FindType.Full;
		if (array.Length > 4)
		{
			result = GetFindTypeFromName(array[4]);
		}
		return result;
	}

	public void SetFindType(FindType NewFindType)
	{
		char[] separator = new char[1] { ' ' };
		string[] array = m_Argument.Split(separator);
		m_Argument = array[0] + " " + array[1].ToString() + " " + array[2].ToString() + " " + array[3].ToString() + " " + NewFindType;
	}

	public bool IsObjectSelectAvailable()
	{
		if ((GetIsGetNearest() && CollectionManager.Instance.GetCollection("Beacon") != null) || (m_ActionInfo.m_ObjectUID != 0 && (m_Type == Type.MoveTo || m_Type == Type.MoveToLessOne || m_Type == Type.MoveToRange || m_Type == Type.AddResource || m_Type == Type.TakeResource)))
		{
			return true;
		}
		return false;
	}

	public static string GetRepeatNameFromConditionType(ConditionType NewType)
	{
		return m_RepeatTypeNamesFull[(int)NewType];
	}

	public static ConditionType GetConditionTypeFromRepeatName(string Name)
	{
		for (int i = 0; i < m_RepeatTypeNames.Length; i++)
		{
			if (m_RepeatTypeNames[i] == Name)
			{
				return m_RepeatTypes[i];
			}
		}
		return ConditionType.Total;
	}

	public static string GetIfNameFromConditionType(ConditionType NewType)
	{
		return m_IfTypeNamesFull[(int)NewType];
	}

	public static ConditionType GetConditionTypeFromIfName(string Name)
	{
		for (int i = 0; i < m_IfTypeNames.Length; i++)
		{
			if (m_IfTypeNames[i] == Name)
			{
				return m_IfTypes[i];
			}
		}
		return ConditionType.Total;
	}

	public static string GetNameFromConditionType(Type NewInstruction, ConditionType NewCondition)
	{
		switch (NewInstruction)
		{
		case Type.Repeat:
			return GetRepeatNameFromConditionType(NewCondition);
		case Type.If:
		case Type.IfElse:
			return GetIfNameFromConditionType(NewCondition);
		default:
			return "";
		}
	}

	public static ConditionType GetConditionTypeFromName(Type NewInstruction, string Name)
	{
		switch (NewInstruction)
		{
		case Type.Repeat:
			return GetConditionTypeFromRepeatName(Name);
		case Type.If:
		case Type.IfElse:
			return GetConditionTypeFromIfName(Name);
		default:
			return ConditionType.Total;
		}
	}

	public static string GetFindNameFromType(FindType Index)
	{
		return "FindType" + m_FindTypeNames[(int)Index];
	}

	public static FindType GetFindTypeFromName(string Name)
	{
		for (int i = 0; i < 5; i++)
		{
			if (m_FindTypeNames[i] == Name)
			{
				return (FindType)i;
			}
		}
		return FindType.Total;
	}

	public static bool GetConditionRequireObject(ConditionType NewType)
	{
		if (NewType == ConditionType.BuildingFull || NewType == ConditionType.BuildingNotFull || NewType == ConditionType.BuildingEmpty || NewType == ConditionType.BuildingNotEmpty)
		{
			return true;
		}
		return false;
	}
}
