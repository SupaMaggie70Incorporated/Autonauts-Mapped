using UnityEngine;

public class ConvertPlayerActionsToWorkers : MonoBehaviour
{
	private static FarmerPlayer m_Farmer;

	private static bool m_DropNextInstruction;

	private static void TransferInfo(HighInstruction NewInstruction, ActionInfo Info)
	{
		NewInstruction.m_ActionInfo = new ActionInfo(Info);
	}

	private static void AddFindNearestObject(TeachWorkerScriptEdit Parent, HighInstruction.Type NewInstructionType, int Index, ActionInfo Info, ObjectType CurrentHoldable, bool StoreHeldObject = true)
	{
		Holdable topObject = m_Farmer.m_FarmerCarry.GetTopObject();
		if ((bool)topObject && topObject.m_TypeIdentifier == ObjectType.ToolBroom)
		{
			Info.m_ObjectType = ObjectType.JunkAny;
			Info.m_Object = null;
		}
		Info.m_ObjectUID = 0;
		TransferInfo(Parent.m_Instructions.m_List[Index], Info);
		Parent.InsertInstruction(HighInstruction.Type.FindNearestObject, Index, Info);
		if (StoreHeldObject)
		{
			Parent.m_Instructions.m_List[Index].m_ActionInfo.m_Value = ObjectTypeList.GetIDName(m_Farmer.m_FarmerCarry.GetLastObjectType());
		}
		else
		{
			Parent.m_Instructions.m_List[Index].m_ActionInfo.m_Value = ObjectTypeList.GetIDName(ObjectTypeList.m_Total);
		}
	}

	private static void AddFindNearestTile(TeachWorkerScriptEdit Parent, HighInstruction.Type NewInstructionType, int Index, ActionInfo Info, ObjectType CurrentHoldable)
	{
		Info.m_ObjectUID = 0;
		TransferInfo(Parent.m_Instructions.m_List[Index], Info);
		Parent.InsertInstruction(HighInstruction.Type.FindNearestTile, Index, Info);
		Parent.m_Instructions.m_List[Index].m_ActionInfo.m_Value = ObjectTypeList.GetIDName(m_Farmer.m_FarmerCarry.GetLastObjectType());
	}

	private static HighInstruction.Type CheckMoveToThenMoveTo(TeachWorkerScriptEdit Parent, HighInstruction.Type NewInstructionType, int Index, ActionInfo Info, ObjectType CurrentHoldable)
	{
		if (Info.m_Position == m_Farmer.m_TileCoord)
		{
			NewInstructionType = HighInstruction.Type.Wait;
			Info.m_Value = "1";
		}
		return NewInstructionType;
	}

	private static HighInstruction.Type CheckMoveToThenPickUp(TeachWorkerScriptEdit Parent, HighInstruction.Type NewInstructionType, int Index, ActionInfo Info, ObjectType CurrentHoldable)
	{
		AddFindNearestObject(Parent, NewInstructionType, Index, Info, CurrentHoldable, StoreHeldObject: false);
		Tile tile = TileManager.Instance.GetTile(Info.m_Position);
		if ((bool)tile.m_BuildingFootprint && (bool)tile.m_BuildingFootprint.GetComponent<Converter>())
		{
			Converter component = tile.m_BuildingFootprint.GetComponent<Converter>();
			if (Info.m_Position == component.GetSpawnPoint())
			{
				Info.m_ObjectUID = component.m_UniqueID;
				Parent.m_Instructions.m_List[Index].m_ActionInfo.m_ObjectUID = Info.m_ObjectUID;
			}
		}
		return NewInstructionType;
	}

	private static HighInstruction.Type CheckMoveToThenRecharge(TeachWorkerScriptEdit Parent, HighInstruction.Type NewInstructionType, int Index, ActionInfo Info, ObjectType CurrentHoldable)
	{
		AddFindNearestObject(Parent, NewInstructionType, Index, Info, CurrentHoldable);
		return NewInstructionType;
	}

	private static HighInstruction.Type CheckMoveToThenUseInHands(TeachWorkerScriptEdit Parent, HighInstruction.Type NewInstructionType, int Index, ActionInfo Info, ObjectType CurrentHoldable)
	{
		if (Info.m_ObjectType == ObjectType.Nothing)
		{
			return NewInstructionType;
		}
		bool flag = true;
		if (Info.m_ObjectType == ObjectType.RockingChair)
		{
			flag = false;
		}
		if (Instrument.GetIsTypeInstrument(CurrentHoldable))
		{
			flag = false;
		}
		if (flag)
		{
			if (Info.m_ObjectType == ObjectType.Plot)
			{
				AddFindNearestTile(Parent, NewInstructionType, Index, Info, CurrentHoldable);
			}
			else
			{
				AddFindNearestObject(Parent, NewInstructionType, Index, Info, CurrentHoldable);
			}
		}
		else
		{
			TransferInfo(Parent.m_Instructions.m_List[Index], Info);
		}
		return NewInstructionType;
	}

	private static HighInstruction.Type CheckMoveToThenAddResource(TeachWorkerScriptEdit Parent, HighInstruction.Type NewInstructionType, int Index, ActionInfo Info, ObjectType CurrentHoldable)
	{
		if ((bool)Info.m_Object && (bool)Info.m_Object.GetComponent<Building>())
		{
			bool flag = true;
			if (Info.m_Object.m_TypeIdentifier == ObjectType.ConverterFoundation)
			{
				flag = false;
			}
			if (Housing.GetIsTypeHouse(Info.m_Object.m_TypeIdentifier))
			{
				flag = false;
			}
			if ((bool)Info.m_Object.GetComponent<Fueler>() && Info.m_Object.GetComponent<Fueler>().IsObjectTypeAcceptableFuel(CurrentHoldable))
			{
				flag = false;
			}
			if (Info.m_Object.m_TypeIdentifier == ObjectType.Trough && Trough.GetIsObjectAcceptable(CurrentHoldable))
			{
				flag = false;
			}
			if (Info.m_Object.m_TypeIdentifier == ObjectType.TrainRefuellingStation)
			{
				if (Train.IsObjectTypeAcceptableFuel(CurrentHoldable))
				{
					flag = false;
				}
				if (ToolFillable.GetIsTypeFillable(CurrentHoldable))
				{
					flag = false;
				}
			}
			if (Info.m_Object.m_TypeIdentifier == ObjectType.StationaryEngine)
			{
				if (StationaryEngine.GetIsObjectAcceptableAsFuel(CurrentHoldable))
				{
					flag = false;
				}
				if (ToolFillable.GetIsTypeFillable(CurrentHoldable))
				{
					flag = false;
				}
			}
			if (flag)
			{
				TransferInfo(Parent.m_Instructions.m_List[Index], Info);
				return NewInstructionType;
			}
			AddFindNearestObject(Parent, NewInstructionType, Index, Info, CurrentHoldable);
			return NewInstructionType;
		}
		AddFindNearestObject(Parent, NewInstructionType, Index, Info, CurrentHoldable);
		return NewInstructionType;
	}

	private static HighInstruction.Type CheckMoveToThenTakeResource(TeachWorkerScriptEdit Parent, HighInstruction.Type NewInstructionType, int Index, ActionInfo Info, ObjectType CurrentHoldable)
	{
		if ((bool)Info.m_Object && !Housing.GetIsTypeHouse(Info.m_Object.m_TypeIdentifier) && !Vehicle.GetIsTypeVehicle(Info.m_ObjectType) && !StorageBeehive.GetIsTypeBeehive(Info.m_ObjectType))
		{
			TransferInfo(Parent.m_Instructions.m_List[Index], Info);
		}
		else if ((bool)Info.m_Object && Housing.GetIsTypeHouse(Info.m_Object.m_TypeIdentifier))
		{
			AddFindNearestObject(Parent, NewInstructionType, Index, Info, CurrentHoldable, StoreHeldObject: false);
		}
		else
		{
			AddFindNearestObject(Parent, NewInstructionType, Index, Info, CurrentHoldable);
		}
		return NewInstructionType;
	}

	private static HighInstruction.Type CheckMoveToThenEngageObject(TeachWorkerScriptEdit Parent, HighInstruction.Type NewInstructionType, int Index, ActionInfo Info, ObjectType CurrentHoldable)
	{
		if (Vehicle.GetIsTypeVehicle(Info.m_ObjectType) || (Info.m_ObjectType == ObjectType.Worker && m_Farmer.m_FarmerCarry.GetLastObjectType() == ObjectType.DataStorageCrude))
		{
			AddFindNearestObject(Parent, NewInstructionType, Index, Info, CurrentHoldable);
		}
		else
		{
			TransferInfo(Parent.m_Instructions.m_List[Index], Info);
		}
		return NewInstructionType;
	}

	private static HighInstruction.Type CheckMoveToThenDisengageObject(TeachWorkerScriptEdit Parent, HighInstruction.Type NewInstructionType, int Index, ActionInfo Info, ObjectType CurrentHoldable)
	{
		return NewInstructionType;
	}

	private static HighInstruction.Type CheckWaitThenMoveTo(TeachWorkerScriptEdit Parent, HighInstruction.Type NewInstructionType, int Index, ActionInfo Info, ObjectType CurrentHoldable)
	{
		if (Info.m_Position == Parent.m_Instructions.m_List[Index].m_ActionInfo.m_Position)
		{
			NewInstructionType = HighInstruction.Type.Wait;
			Info.m_Value = "1";
		}
		return NewInstructionType;
	}

	private static HighInstruction.Type CheckUseInHands(TeachWorkerScriptEdit Parent, HighInstruction.Type NewInstructionType, int Index, ActionInfo Info, ObjectType CurrentHoldable)
	{
		if (Info.m_ObjectType == ObjectType.Plot)
		{
			Info.m_ObjectUID = 0;
			Parent.InsertInstruction(HighInstruction.Type.FindNearestTile, Index, Info);
			Parent.m_Instructions.m_List[Index].m_ActionInfo.m_Value = ObjectTypeList.GetIDName(m_Farmer.m_FarmerCarry.GetLastObjectType());
		}
		return NewInstructionType;
	}

	public static void Check(TeachWorkerScriptEdit Parent, HighInstruction.Type NewInstructionType, ActionInfo Info)
	{
		m_Farmer = CollectionManager.Instance.GetPlayers()[0].GetComponent<FarmerPlayer>();
		ObjectType lastObjectType = m_Farmer.m_FarmerCarry.GetLastObjectType();
		int num = Parent.m_Instructions.m_List.Count - 1;
		bool flag = false;
		if (num >= 0)
		{
			switch (Parent.m_Instructions.m_List[num].m_Type)
			{
			case HighInstruction.Type.MoveTo:
			case HighInstruction.Type.MoveToLessOne:
			case HighInstruction.Type.MoveToRange:
				switch (NewInstructionType)
				{
				case HighInstruction.Type.MoveTo:
				case HighInstruction.Type.MoveToLessOne:
				case HighInstruction.Type.MoveToRange:
					NewInstructionType = CheckMoveToThenMoveTo(Parent, NewInstructionType, num, Info, lastObjectType);
					flag = true;
					break;
				case HighInstruction.Type.Pickup:
					NewInstructionType = CheckMoveToThenPickUp(Parent, NewInstructionType, num, Info, lastObjectType);
					flag = true;
					break;
				case HighInstruction.Type.Recharge:
					NewInstructionType = CheckMoveToThenRecharge(Parent, NewInstructionType, num, Info, lastObjectType);
					flag = true;
					break;
				case HighInstruction.Type.UseInHands:
					NewInstructionType = CheckMoveToThenUseInHands(Parent, NewInstructionType, num, Info, lastObjectType);
					flag = true;
					break;
				case HighInstruction.Type.AddResource:
					NewInstructionType = CheckMoveToThenAddResource(Parent, NewInstructionType, num, Info, lastObjectType);
					flag = true;
					break;
				case HighInstruction.Type.TakeResource:
					NewInstructionType = CheckMoveToThenTakeResource(Parent, NewInstructionType, num, Info, lastObjectType);
					flag = true;
					break;
				case HighInstruction.Type.EngageObject:
					NewInstructionType = CheckMoveToThenEngageObject(Parent, NewInstructionType, num, Info, lastObjectType);
					flag = true;
					break;
				case HighInstruction.Type.DisengageObject:
					NewInstructionType = CheckMoveToThenDisengageObject(Parent, NewInstructionType, num, Info, lastObjectType);
					flag = true;
					break;
				}
				break;
			case HighInstruction.Type.Wait:
				if (NewInstructionType == HighInstruction.Type.MoveTo || NewInstructionType == HighInstruction.Type.MoveToLessOne || NewInstructionType == HighInstruction.Type.MoveToRange)
				{
					NewInstructionType = CheckWaitThenMoveTo(Parent, NewInstructionType, num, Info, lastObjectType);
					flag = true;
				}
				break;
			}
		}
		if (!flag)
		{
			num = Parent.m_Instructions.m_List.Count;
			if (NewInstructionType == HighInstruction.Type.UseInHands)
			{
				NewInstructionType = CheckUseInHands(Parent, NewInstructionType, num, Info, lastObjectType);
			}
		}
		if (m_DropNextInstruction)
		{
			m_DropNextInstruction = false;
		}
		else if (NewInstructionType == HighInstruction.Type.EngageObject && (bool)Info.m_Object && (((bool)Info.m_Object.GetComponent<Converter>() && Info.m_Object.GetComponent<Converter>().m_State == Converter.State.Converting) || ((bool)Info.m_Object.GetComponent<ResearchStation>() && Info.m_Object.GetComponent<ResearchStation>().m_State == ResearchStation.State.Researching)))
		{
			m_DropNextInstruction = true;
		}
		else
		{
			Parent.InsertInstruction(NewInstructionType, Parent.m_Instructions.m_List.Count, Info);
		}
	}
}
