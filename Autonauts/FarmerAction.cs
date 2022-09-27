using System.Collections.Generic;
using UnityEngine;

public class FarmerAction : MonoBehaviour
{
	[HideInInspector]
	public delegate void DoAction(ActionInfo Info);

	public static int m_ThrowDistanceTiles = 5;

	[HideInInspector]
	public DoAction[] m_Actions;

	[HideInInspector]
	public ActionInfo m_CurrentInfo;

	[HideInInspector]
	public Actionable m_CurrentObject;

	private Farmer m_Farmer;

	private void Awake()
	{
		m_Farmer = GetComponent<Farmer>();
		m_Actions = new DoAction[49];
		m_Actions[2] = MoveTo;
		m_Actions[3] = MoveToLessOne;
		m_Actions[4] = MoveToRange;
		m_Actions[5] = MoveForwards;
		m_Actions[6] = MoveBackwards;
		m_Actions[9] = MoveDirection;
		m_Actions[12] = Stop;
		m_Actions[13] = LookAt;
		m_Actions[14] = SetTool;
		m_Actions[15] = UseInHands;
		m_Actions[16] = DropAll;
		m_Actions[17] = Pickup;
		m_Actions[18] = Make;
		m_Actions[19] = TakeResource;
		m_Actions[20] = AddResource;
		m_Actions[23] = StowObject;
		m_Actions[24] = RecallObject;
		m_Actions[25] = CycleObject;
		m_Actions[26] = SwapObject;
		m_Actions[27] = BeingHeld;
		m_Actions[28] = Dropped;
		m_Actions[32] = EngageObject;
		m_Actions[33] = DisengageObject;
		m_Actions[40] = Recharge;
		m_Actions[44] = Whistle;
		m_Actions[45] = Shout;
	}

	public void SendAction(ActionInfo Info)
	{
		if (m_Actions[(int)Info.m_Action] == null)
		{
			return;
		}
		if (Info.m_Action != ActionType.MoveTo && Info.m_Action != ActionType.MoveToLessOne && Info.m_Action != ActionType.MoveToRange && Info.m_Action != ActionType.MoveForwards && Info.m_Action != ActionType.MoveBackwards && Info.m_Action != ActionType.Stop)
		{
			if ((m_Farmer.m_EngagedObject == null || m_Farmer.m_EngagedObject.m_TypeIdentifier == ObjectType.Worker) && m_Farmer.m_State != 0 && m_Farmer.m_State != Farmer.State.WorkerSelect)
			{
				return;
			}
			m_CurrentInfo = Info;
			m_CurrentObject = m_CurrentInfo.m_Object;
		}
		m_Actions[(int)Info.m_Action](Info);
	}

	public bool CanDoAction(ActionInfo Info, bool RightNow)
	{
		if (m_Farmer.m_Energy == 0f)
		{
			return false;
		}
		if (m_Actions[(int)Info.m_Action] != null)
		{
			if (Info.m_Action == ActionType.BeingHeld)
			{
				if (m_Farmer.m_TypeIdentifier == ObjectType.FarmerPlayer)
				{
					return false;
				}
				if (m_Farmer.m_Learning)
				{
					return false;
				}
				if (m_Farmer.m_State != 0)
				{
					return false;
				}
			}
			if (Info.m_Action == ActionType.MoveTo)
			{
				MoveTo(Info);
				return m_Farmer.m_Path != null;
			}
			if (Info.m_Action == ActionType.MoveToLessOne)
			{
				MoveToLessOne(Info);
				return m_Farmer.m_Path != null;
			}
			if (Info.m_Action == ActionType.MoveToRange)
			{
				MoveToRange(Info);
				return m_Farmer.m_Path != null;
			}
			if (Info.m_Action == ActionType.MoveForwards)
			{
				MoveForwards(Info);
				return m_Farmer.m_Path != null;
			}
			if (Info.m_Action == ActionType.MoveBackwards)
			{
				MoveBackwards(Info);
				return m_Farmer.m_Path != null;
			}
			return true;
		}
		return false;
	}

	private void CheckTargetPosition(ActionInfo Info)
	{
		if ((bool)Info.m_Object)
		{
			if ((bool)Info.m_Object.GetComponent<MobileStorage>())
			{
				Info.m_Position = Info.m_Object.GetComponent<TileCoordObject>().m_TileCoord;
			}
			if (TileManager.Instance.DoesObjectAffectRouteFinding(Info.m_Object.m_TypeIdentifier))
			{
				Info.m_Position = Info.m_Object.GetComponent<TileCoordObject>().GetNearestAdjacentTile(m_Farmer.m_TileCoord);
			}
		}
	}

	private void CheckForTargetWorker(ActionInfo Info)
	{
		if (!Info.m_Object || Info.m_Object.m_TypeIdentifier != ObjectType.Worker)
		{
			return;
		}
		Holdable topObject = m_Farmer.m_FarmerCarry.GetTopObject();
		if (topObject == null || (!Upgrade.GetIsTypeUpgrade(topObject.m_TypeIdentifier) && !WorkerPart.GetIsTypePart(topObject.m_TypeIdentifier) && !Clothing.GetIsTypeClothing(topObject.m_TypeIdentifier)) || Info.m_ActionType != AFO.AT.AltSecondary)
		{
			ObjectType newObjectType = ObjectTypeList.m_Total;
			if (topObject != null)
			{
				newObjectType = topObject.m_TypeIdentifier;
			}
			Actionable.m_ReusableActionFromObject.Init(topObject, newObjectType, m_Farmer, Info.m_ActionType, "", Info.m_Position);
			ActionType actionFromObjectSafe = Info.m_Object.GetActionFromObjectSafe(Actionable.m_ReusableActionFromObject);
			if (actionFromObjectSafe == ActionType.AddResource || actionFromObjectSafe == ActionType.TakeResource)
			{
				m_Farmer.m_TestTargetCloseDistance = Tile.m_Size * (float)m_ThrowDistanceTiles;
			}
		}
	}

	private void MoveTo(ActionInfo Info)
	{
		CheckTargetPosition(Info);
		TileCoord destination = Info.m_Position;
		if ((bool)Info.m_Object && (Info.m_Object.m_TypeIdentifier == ObjectType.ConverterFoundation || TileManager.Instance.DoesObjectAffectRouteFinding(Info.m_Object.m_TypeIdentifier)))
		{
			Holdable topObject = m_Farmer.m_FarmerCarry.GetTopObject();
			ObjectType newObjectType = ObjectTypeList.m_Total;
			if (topObject != null)
			{
				newObjectType = topObject.m_TypeIdentifier;
			}
			Actionable.m_ReusableActionFromObject.Init(topObject, newObjectType, m_Farmer, Info.m_ActionType, "", Info.m_Position);
			if (Info.m_Object.GetActionFromObjectSafe(Actionable.m_ReusableActionFromObject) != ActionType.Total)
			{
				destination = Info.m_Object.GetComponent<TileCoordObject>().GetNearestAdjacentTile(m_Farmer.m_TileCoord);
				if (destination.x == 0 && destination.y == 0)
				{
					destination = Info.m_Position;
				}
			}
		}
		CheckForTargetWorker(Info);
		m_Farmer.RequestGoTo(destination, Info.m_Object);
	}

	private void MoveToLessOne(ActionInfo Info)
	{
		CheckTargetPosition(Info);
		CheckForTargetWorker(Info);
		m_Farmer.RequestGoTo(Info.m_Position, Info.m_Object, LessOne: true);
	}

	private void MoveToRange(ActionInfo Info)
	{
		CheckTargetPosition(Info);
		CheckForTargetWorker(Info);
		int result = 0;
		int.TryParse(Info.m_Value2, out result);
		m_Farmer.RequestGoTo(Info.m_Position, Info.m_Object, LessOne: false, result);
	}

	private void MoveForwards(ActionInfo Info)
	{
		Debug.Log("Forwards");
	}

	private void MoveBackwards(ActionInfo Info)
	{
		Debug.Log("Backwards");
	}

	private void MoveDirection(ActionInfo Info)
	{
		TileCoord destination = m_Farmer.m_TileCoord + Info.m_Position;
		m_Farmer.RequestGoTo(destination, Info.m_Object, LessOne: true);
	}

	private void Stop(ActionInfo Info)
	{
		m_Farmer.EndGoTo();
	}

	private void LookAt(ActionInfo Info)
	{
		m_Farmer.LookAt(Info.m_Position);
	}

	private void SetTool(ActionInfo Info)
	{
		m_Farmer.m_FarmerCarry.TryAddCarry(Info.m_Object.GetComponent<Holdable>());
	}

	private bool CheckAdjacencyAndSetTarget(TileCoord Destination, bool Set)
	{
		TileCoord tileCoord = m_Farmer.m_TileCoord - Destination;
		if (Mathf.Abs(tileCoord.x) <= 1 && Mathf.Abs(tileCoord.y) <= 1)
		{
			m_Farmer.m_ActionTile = Destination;
			return true;
		}
		return false;
	}

	public Farmer.State GetUseInHandsState(ActionInfo Info, out bool AdjacentTile, out int Range)
	{
		Farmer.State state = Farmer.State.Total;
		AdjacentTile = false;
		Range = 0;
		Tile tile = TileManager.Instance.GetTile(Info.m_Position);
		if (tile.m_Building != null || tile.m_BuildingFootprint != null || tile.m_Floor != null)
		{
			state = Farmer.State.Total;
		}
		Holdable topObject = m_Farmer.m_FarmerCarry.GetTopObject();
		if ((bool)topObject)
		{
			ObjectType newObjectType = ObjectTypeList.m_Total;
			if (topObject != null)
			{
				newObjectType = topObject.m_TypeIdentifier;
			}
			Actionable.m_ReusableActionFromObject.Init(topObject, newObjectType, m_Farmer, Info.m_ActionType, "", Info.m_Position);
			Info.m_Object.GetActionFromObjectSafe(Actionable.m_ReusableActionFromObject);
			state = Actionable.m_ReusableActionFromObject.m_FarmerState;
			if (state != Farmer.State.Total)
			{
				AdjacentTile = m_Farmer.m_States[(int)state].GetIsAdjacentTile(Info.m_Position, Info.m_Object);
				Range = VariableManager.Instance.GetVariableAsInt(topObject.m_TypeIdentifier, "UseRange", CheckValid: false);
				return state;
			}
		}
		return state;
	}

	private void UseInHands(ActionInfo Info)
	{
		if ((bool)m_Farmer.m_EngagedObject)
		{
			m_Farmer.m_EngagedObject.SendAction(Info);
			return;
		}
		bool AdjacentTile;
		int Range;
		Farmer.State useInHandsState = GetUseInHandsState(Info, out AdjacentTile, out Range);
		if (useInHandsState == Farmer.State.Total)
		{
			return;
		}
		if (Range != 0)
		{
			if ((Info.m_Position.ToWorldPosition() - m_Farmer.m_TileCoord.ToWorldPosition()).magnitude > (float)Range * Tile.m_Size)
			{
				return;
			}
		}
		else if (AdjacentTile)
		{
			if (!CheckAdjacencyAndSetTarget(Info.m_Position, Set: true))
			{
				return;
			}
		}
		else if (m_Farmer.m_TileCoord != Info.m_Position && Info.m_Object.m_TypeIdentifier != 0 && (!TileManager.Instance.DoesObjectAffectRouteFinding(Info.m_Object.m_TypeIdentifier) || !CheckAdjacencyAndSetTarget(Info.m_Position, Set: false)))
		{
			return;
		}
		Info.m_Object.StartAction(m_Farmer.m_FarmerCarry.GetTopObject(), m_Farmer, Info.m_ActionType, Info.m_Position);
		m_Farmer.SetState(useInHandsState);
	}

	public void DoDrop()
	{
		m_Farmer.m_FarmerCarry.DropAllObjects();
	}

	private void DropAll(ActionInfo Info)
	{
		if (!TileManager.Instance.GetTile(m_Farmer.m_TileCoord).m_Building)
		{
			m_Farmer.SetState(Farmer.State.Dropping);
		}
	}

	private bool IsObjectCloseEnough()
	{
		if ((bool)m_CurrentInfo.m_Object && (bool)m_CurrentInfo.m_Object.GetComponent<TileCoordObject>())
		{
			TileCoord tileCoord = m_CurrentInfo.m_Object.GetComponent<TileCoordObject>().m_TileCoord;
			TileCoord tileCoord2 = tileCoord - m_Farmer.m_TileCoord;
			if ((Mathf.Abs(tileCoord2.x) > 1 || Mathf.Abs(tileCoord2.y) > 1) && TileManager.Instance.GetTile(tileCoord).m_Building == null)
			{
				return false;
			}
		}
		return true;
	}

	public bool DoPickup()
	{
		m_Farmer.SetBaggedObject(null);
		if ((bool)m_CurrentInfo.m_Object && m_CurrentInfo.m_Object.GetComponent<Plot>() == null)
		{
			BaseClass objectFromUniqueID = ObjectTypeList.Instance.GetObjectFromUniqueID(m_CurrentInfo.m_ObjectUID, ErrorCheck: false);
			if (objectFromUniqueID == null)
			{
				return false;
			}
			Actionable component = objectFromUniqueID.GetComponent<Actionable>();
			BaggedManager.Instance.Remove(component);
			if ((bool)m_CurrentInfo.m_Object && (bool)m_CurrentInfo.m_Object.GetComponent<Savable>() && !m_CurrentInfo.m_Object.GetComponent<Savable>().GetIsSavable())
			{
				return false;
			}
			if (!IsObjectCloseEnough())
			{
				return false;
			}
			if (!component.CanDoAction(new ActionInfo(ActionType.BeingHeld, default(TileCoord), m_Farmer)))
			{
				return false;
			}
		}
		if (m_Farmer.m_FarmerCarry.GetTopObject() != null)
		{
			Holdable topObject = m_Farmer.m_FarmerCarry.GetTopObject();
			ToolFillable component2 = topObject.GetComponent<ToolFillable>();
			if ((bool)component2 && (bool)m_CurrentInfo.m_Object && component2.GetSpace() > 0)
			{
				BaseClass @object = m_CurrentInfo.m_Object;
				if ((bool)@object.GetComponent<Plot>())
				{
					Tile.TileType tileType = TileManager.Instance.GetTileType(m_CurrentInfo.m_Position);
					bool flag = false;
					if (TileHelpers.GetTileWaterCollectable(tileType))
					{
						if (TileHelpers.GetTileWaterDrinkable(tileType))
						{
							component2.Fill(ObjectType.Water, component2.m_Capacity);
						}
						else
						{
							component2.Fill(ObjectType.SeaWater, component2.m_Capacity);
						}
						QuestManager.Instance.AddEvent(QuestEvent.Type.FillBucket, m_Farmer.m_TypeIdentifier == ObjectType.Worker, 0, m_Farmer);
						StatsManager.Instance.AddEvent(StatsManager.StatEvent.Water);
						flag = true;
					}
					else
					{
						switch (tileType)
						{
						case Tile.TileType.Sand:
							component2.Fill(ObjectType.Sand, component2.m_Capacity);
							QuestManager.Instance.AddEvent(QuestEvent.Type.FillBucketSand, m_Farmer.m_TypeIdentifier == ObjectType.Worker, 0, m_Farmer);
							QuestManager.Instance.AddEvent(QuestEvent.Type.FillBucketSandOrSoil, m_Farmer.m_TypeIdentifier == ObjectType.Worker, 0, m_Farmer);
							StatsManager.Instance.AddEvent(StatsManager.StatEvent.Sand);
							flag = true;
							break;
						case Tile.TileType.Soil:
						case Tile.TileType.SoilTilled:
							component2.Fill(ObjectType.Soil, component2.m_Capacity);
							flag = true;
							QuestManager.Instance.AddEvent(QuestEvent.Type.FillBucketSandOrSoil, m_Farmer.m_TypeIdentifier == ObjectType.Worker, 0, m_Farmer);
							break;
						}
					}
					if (flag)
					{
						return true;
					}
				}
				else if ((bool)@object && topObject.GetComponent<ToolFillable>().CanAcceptObjectType(@object.m_TypeIdentifier))
				{
					topObject.GetComponent<ToolFillable>().Fill(@object.m_TypeIdentifier, 1);
					@object.StopUsing();
					return true;
				}
			}
			if ((bool)topObject.GetComponent<ToolBasket>())
			{
				BaseClass object2 = m_CurrentInfo.m_Object;
				if ((bool)object2 && topObject.GetComponent<ToolBasket>().CanAcceptObject(object2))
				{
					topObject.GetComponent<ToolBasket>().Add(object2);
					return true;
				}
			}
		}
		if ((bool)m_CurrentInfo.m_Object)
		{
			return m_Farmer.m_FarmerCarry.TryAddCarry(m_CurrentInfo.m_Object.GetComponent<Holdable>());
		}
		return false;
	}

	private void Pickup(ActionInfo Info)
	{
		bool flag = m_Farmer.m_FarmerCarry.CanAddCarry(m_CurrentInfo.m_Object.GetComponent<Holdable>());
		Holdable topObject = m_Farmer.m_FarmerCarry.GetTopObject();
		if (topObject != null)
		{
			if ((bool)topObject.GetComponent<ToolFillable>() && topObject.GetComponent<ToolFillable>().CanAcceptObjectType(m_CurrentInfo.m_Object.m_TypeIdentifier))
			{
				flag = true;
			}
			if ((bool)topObject.GetComponent<ToolBasket>() && topObject.GetComponent<ToolBasket>().CanAcceptObject(m_CurrentInfo.m_Object))
			{
				flag = true;
			}
		}
		if (flag)
		{
			m_Farmer.SetState(Farmer.State.PickingUp);
		}
		else if (m_Farmer.m_TypeIdentifier == ObjectType.FarmerPlayer)
		{
			AudioManager.Instance.StartEvent("PlayerActionFail");
			m_Farmer.SetBaggedObject(null);
		}
	}

	private void Make(ActionInfo Info)
	{
		Holdable topObject = m_Farmer.m_FarmerCarry.GetTopObject();
		if ((bool)topObject && (topObject.m_TypeIdentifier == ObjectType.Stick || topObject.m_TypeIdentifier == ObjectType.Rock))
		{
			m_Farmer.SetState(Farmer.State.Make);
		}
	}

	public void MakeInHand()
	{
		m_Farmer.SetBaggedObject(null);
		m_CurrentInfo.m_Object.StopUsing();
		m_Farmer.m_FarmerCarry.DestroyTopObject();
		ObjectType identifierType = (ObjectType)int.Parse(m_CurrentInfo.m_Value);
		m_Farmer.m_FarmerCarry.TryAddCarry(identifierType);
	}

	public bool IsTargetNear(Actionable Target, bool TestBots = false)
	{
		if (Target.m_TypeIdentifier == ObjectType.FarmerPlayer || (!TestBots && Target.m_TypeIdentifier == ObjectType.Worker))
		{
			return true;
		}
		if (ObjectTypeList.Instance.GetIsBuilding(Target.m_TypeIdentifier))
		{
			Building component = Target.GetComponent<Building>();
			if (component.m_TypeIdentifier == ObjectType.ConverterFoundation)
			{
				if (component.GetAdjacentTiles().Contains(m_Farmer.m_TileCoord))
				{
					return true;
				}
			}
			else
			{
				TileCoord accessPosition = component.GetAccessPosition();
				if (m_Farmer.m_TileCoord == accessPosition)
				{
					return true;
				}
			}
		}
		else
		{
			TileCoord tileCoord = Target.GetComponent<TileCoordObject>().m_TileCoord - m_Farmer.m_TileCoord;
			if (Mathf.Abs(tileCoord.x) <= 1 && Mathf.Abs(tileCoord.y) <= 1)
			{
				return true;
			}
		}
		return false;
	}

	public void DoTake()
	{
		m_Farmer.SetBaggedObject(null);
		m_CurrentInfo.m_Object.SendAction(new ActionInfo(ActionType.UnreserveResource, default(TileCoord)));
		if (!IsTargetNear(m_CurrentInfo.m_Object))
		{
			return;
		}
		if (m_CurrentInfo.m_Object.m_TypeIdentifier == ObjectType.FolkSeedPod)
		{
			m_CurrentInfo.m_Object.StartAction(null, m_Farmer, m_CurrentInfo.m_ActionType, m_CurrentInfo.m_Position);
			return;
		}
		Holdable topObject = m_Farmer.m_FarmerCarry.GetTopObject();
		if ((bool)topObject && (bool)topObject.GetComponent<ToolFillable>())
		{
			ObjectType newType = ((topObject.m_TypeIdentifier != ObjectType.ToolPitchfork) ? ((ObjectType)m_CurrentInfo.m_Object.GetActionInfo(new GetActionInfo(GetAction.GetObjectType))) : m_CurrentInfo.m_Object.m_TypeIdentifier);
			if (topObject.GetComponent<ToolFillable>().CanAcceptObjectType(newType))
			{
				m_CurrentInfo.m_Object.StartAction(topObject, m_Farmer, m_CurrentInfo.m_ActionType, m_CurrentInfo.m_Position);
			}
			return;
		}
		m_CurrentInfo.m_Object.StartAction(topObject, m_Farmer, m_CurrentInfo.m_ActionType, m_CurrentInfo.m_Position);
		Actionable @object = m_CurrentInfo.m_Object;
		float num;
		if ((bool)@object.GetComponent<Building>())
		{
			Building component = @object.GetComponent<Building>();
			num = (float)((component.m_AccessPointRotation + component.m_Rotation + 2) % 4) * 360f / 4f;
		}
		else
		{
			Vector3 vector = @object.transform.position - m_Farmer.transform.position;
			num = (0f - Mathf.Atan2(vector.z, vector.x)) * 57.29578f;
		}
		m_Farmer.transform.rotation = Quaternion.Euler(0f, num - 90f, 0f);
		if (m_Farmer.m_FarmerCarry.GetTopObject() != null)
		{
			Vector3 position = m_Farmer.m_FarmerCarry.GetTopObject().transform.position;
			topObject = m_Farmer.m_FarmerCarry.RemoveTopObject();
			if ((bool)topObject)
			{
				Vector3 startPosition = m_CurrentInfo.m_Object.transform.position;
				if ((bool)m_CurrentInfo.m_Object.GetComponent<Storage>())
				{
					startPosition = m_CurrentInfo.m_Object.GetComponent<Storage>().GetAddPoint();
				}
				else if (Minecart.GetIsTypeMinecart(m_CurrentInfo.m_Object.m_TypeIdentifier))
				{
					startPosition = m_CurrentInfo.m_Object.GetComponent<Minecart>().GetAddPoint();
				}
				SpawnAnimationManager.Instance.AddJump(topObject, startPosition, position, 5f, 0.2f, m_Farmer, DustPuff: false, m_CurrentInfo.m_Object, m_Farmer);
			}
		}
		else
		{
			m_Farmer.SetState(Farmer.State.None);
		}
	}

	private void TakeResource(ActionInfo Info)
	{
		if (ObjectTypeList.Instance.GetIsBuilding(m_CurrentInfo.m_Object.m_TypeIdentifier) || MobileStorage.GetIsTypeMobileStorage(m_CurrentInfo.m_Object.m_TypeIdentifier))
		{
			if (!IsTargetNear(m_CurrentInfo.m_Object))
			{
				return;
			}
			if (m_CurrentInfo.m_Object.m_TypeIdentifier == ObjectType.FolkSeedPod)
			{
				m_Farmer.SetState(Farmer.State.Taking);
				return;
			}
			ObjectType newType = (ObjectType)m_CurrentInfo.m_Object.GetActionInfo(new GetActionInfo(GetAction.GetObjectType));
			Holdable topObject = m_Farmer.m_FarmerCarry.GetTopObject();
			if ((bool)topObject && (bool)topObject.GetComponent<ToolFillable>() && topObject.m_TypeIdentifier != ObjectType.ToolPitchfork)
			{
				ObjectType newType2 = (ObjectType)m_CurrentInfo.m_Object.GetActionInfo(new GetActionInfo(GetAction.GetObjectType));
				if (topObject.GetComponent<ToolFillable>().CanAcceptObjectType(newType2))
				{
					m_CurrentInfo.m_Object.SendAction(new ActionInfo(ActionType.ReserveResource, default(TileCoord)));
					m_Farmer.SetState(Farmer.State.Taking);
				}
			}
			else if (m_Farmer.m_FarmerCarry.CanAddCarry(newType))
			{
				m_CurrentInfo.m_Object.SendAction(new ActionInfo(ActionType.ReserveResource, default(TileCoord)));
				m_Farmer.SetState(Farmer.State.Taking);
			}
			else if (m_Farmer.m_TypeIdentifier == ObjectType.FarmerPlayer)
			{
				AudioManager.Instance.StartEvent("PlayerActionFail");
			}
		}
		else
		{
			m_Farmer.SetState(Farmer.State.Taking);
		}
	}

	public bool DoAdd()
	{
		m_Farmer.SetBaggedObject(null);
		if (m_CurrentInfo == null)
		{
			return false;
		}
		if ((bool)m_CurrentInfo.m_Object && IsTargetNear(m_CurrentInfo.m_Object))
		{
			Holdable topObject = m_Farmer.m_FarmerCarry.GetTopObject();
			if ((bool)topObject)
			{
				int uniqueID = topObject.m_UniqueID;
				BaseClass objectFromUniqueID = ObjectTypeList.Instance.GetObjectFromUniqueID(uniqueID, ErrorCheck: false);
				if ((bool)objectFromUniqueID && (bool)objectFromUniqueID.GetComponent<Actionable>().GetActionInfo(new GetActionInfo(GetAction.IsStorable, m_CurrentInfo.m_Object)))
				{
					Vector3 position = topObject.transform.position;
					Vector3 localScale = topObject.transform.localScale;
					if (ToolFillable.GetIsTypeFillable(topObject.m_TypeIdentifier))
					{
						if (topObject.GetComponent<ToolFillable>().GetIsEmpty() || (bool)m_CurrentInfo.m_Object.GetComponent<Farmer>() || !ToolFillable.GetIsTypeEmptyable(topObject.m_TypeIdentifier))
						{
							m_Farmer.m_FarmerCarry.RemoveTopObject();
						}
						else if (MobileStorageGeneral.GetIsTypeMobileStorageGeneral(m_CurrentInfo.m_Object.m_TypeIdentifier))
						{
							ObjectType heldType = topObject.GetComponent<ToolFillable>().m_HeldType;
							if (heldType == ObjectTypeList.m_Total || !m_CurrentInfo.m_Object.GetComponent<MobileStorageGeneral>().CanAcceptObject(topObject, heldType))
							{
								m_Farmer.m_FarmerCarry.RemoveTopObject();
							}
						}
						else if (m_CurrentInfo.m_Object.m_TypeIdentifier == ObjectType.StoneHeads || m_CurrentInfo.m_Object.m_TypeIdentifier == ObjectType.Catapult)
						{
							m_Farmer.m_FarmerCarry.RemoveTopObject();
						}
					}
					else
					{
						m_Farmer.m_FarmerCarry.RemoveTopObject();
					}
					if (!topObject.m_BeingHeld)
					{
						topObject.transform.position = m_CurrentInfo.m_Object.transform.position;
					}
					m_CurrentInfo.m_Object.StartAction(topObject, m_Farmer, m_CurrentInfo.m_ActionType, m_CurrentInfo.m_Position);
					topObject.ForceHighlight(Highlighted: false);
					topObject.transform.localScale = localScale;
					if (ObjectTypeList.Instance.GetIsAnimateAdd(topObject))
					{
						if (!BaseClass.TestSpawningObject(topObject, m_Farmer, m_CurrentInfo.m_Object))
						{
							SpawnAnimationManager.Instance.AddJump(topObject, position, topObject.transform.position, 5f, 0.2f, m_Farmer, DustPuff: false, m_Farmer, m_CurrentInfo.m_Object);
						}
						AudioManager.Instance.StartEvent("FarmerThrow", m_Farmer);
					}
					return true;
				}
				AudioManager.Instance.StartEvent("BuildingIngredientAddBad", m_CurrentInfo.m_Object.GetComponent<TileCoordObject>());
			}
		}
		return false;
	}

	public void UndoAdd(Actionable NewObject)
	{
		if (ToolFillable.GetIsTypeFillable(m_Farmer.m_FarmerCarry.GetTopObjectType()))
		{
			m_Farmer.m_FarmerCarry.GetTopObject().GetComponent<ToolFillable>().RestoreLastStored();
		}
		else if ((bool)NewObject)
		{
			m_Farmer.m_FarmerCarry.AddCarry(NewObject.GetComponent<Holdable>());
		}
	}

	private void AddResource(ActionInfo Info)
	{
		m_Farmer.SetState(Farmer.State.Adding);
	}

	private void AcceptResource(ActionInfo Info)
	{
	}

	public void DoStow()
	{
		if (m_Farmer.m_FarmerCarry.GetCarryCount() <= 0)
		{
			return;
		}
		int num = m_Farmer.m_FarmerCarry.StowObjects();
		if (num != 0 && (bool)m_Farmer.GetComponent<FarmerPlayer>())
		{
			if (num == 1)
			{
				AudioManager.Instance.StartEvent("PlayerInventoryStow");
			}
			else
			{
				AudioManager.Instance.StartEvent("PlayerUpgradeAdded");
			}
		}
	}

	private void StowObject(ActionInfo Info)
	{
		if (m_Farmer.m_FarmerCarry.GetCarryCount() > 0)
		{
			if (m_Farmer.m_State == Farmer.State.Engaged)
			{
				DoStow();
			}
			else
			{
				m_Farmer.SetState(Farmer.State.Stowing);
			}
		}
	}

	public void DoRecall()
	{
		ObjectType identifierFromSaveName = ObjectTypeList.Instance.GetIdentifierFromSaveName(m_CurrentInfo.m_Value, Check: false);
		m_Farmer.m_FarmerCarry.RecallObject(identifierFromSaveName);
		if ((bool)m_Farmer.GetComponent<FarmerPlayer>())
		{
			AudioManager.Instance.StartEvent("PlayerInventoryCycleBackwards");
		}
	}

	private void RecallObject(ActionInfo Info)
	{
		m_Farmer.SetState(Farmer.State.Recalling);
	}

	public void DoCycle()
	{
		bool flag = false;
		if (m_CurrentInfo.m_Value == "Up")
		{
			flag = true;
		}
		if (!m_Farmer.m_FarmerInventory.GetAnyInventory())
		{
			return;
		}
		if ((bool)m_Farmer.GetComponent<FarmerPlayer>())
		{
			if (flag)
			{
				AudioManager.Instance.StartEvent("PlayerInventoryCycleForwards");
			}
			else
			{
				AudioManager.Instance.StartEvent("PlayerInventoryCycleBackwards");
			}
		}
		Holdable carryObject = m_Farmer.m_FarmerInventory.PopObject(!flag);
		int carryCount = m_Farmer.m_FarmerCarry.GetCarryCount();
		for (int i = 0; i < carryCount; i++)
		{
			m_Farmer.m_FarmerInventory.PushObject(m_Farmer.m_FarmerCarry.RemoveTopObject(), flag);
		}
		m_Farmer.m_FarmerCarry.AddCarry(carryObject);
	}

	private void CycleObject(ActionInfo Info)
	{
		if (!m_Farmer.m_FarmerInventory.GetAnyInventory())
		{
			return;
		}
		bool flag = false;
		if (m_CurrentInfo.m_Value == "Up")
		{
			flag = true;
		}
		int num = Holdable.GetWeight(m_Farmer.m_FarmerInventory.GetObject(!flag).m_TypeIdentifier) + m_Farmer.m_FarmerInventory.GetFreeSlots();
		int heldWeight = m_Farmer.m_FarmerCarry.GetHeldWeight();
		if (num < heldWeight)
		{
			return;
		}
		Holdable topObject = m_Farmer.m_FarmerCarry.GetTopObject();
		if (topObject == null || m_Farmer.m_FarmerInventory.IsObjectTypeAcceptable(topObject))
		{
			if (m_Farmer.m_State == Farmer.State.Engaged)
			{
				DoCycle();
			}
			else
			{
				m_Farmer.SetState(Farmer.State.Cycling);
			}
		}
	}

	public void DoSwap()
	{
		if ((m_Farmer.m_FarmerCarry.GetCarryCount() > 0 || m_Farmer.m_FarmerInventory.GetFullSlots() > 0) && m_Farmer.m_FarmerCarry.SwapObjects() != 0 && (bool)m_Farmer.GetComponent<FarmerPlayer>())
		{
			AudioManager.Instance.StartEvent("PlayerInventoryStow");
		}
	}

	private void SwapObject(ActionInfo Info)
	{
		if (m_Farmer.m_FarmerCarry.GetCarryCount() > 0 || m_Farmer.m_FarmerInventory.GetFullSlots() > 0)
		{
			if (m_Farmer.m_State == Farmer.State.Engaged)
			{
				DoStow();
			}
			else
			{
				m_Farmer.SetState(Farmer.State.Swapping);
			}
		}
	}

	private void BeingHeld(ActionInfo Info)
	{
		m_Farmer.m_HolderObject = Info.m_Object.GetComponent<GoTo>();
		m_Farmer.SetState(Farmer.State.Held);
		PlotManager.Instance.RemoveObject(m_Farmer);
	}

	private void Dropped(ActionInfo Info)
	{
		base.transform.parent = MapManager.Instance.m_ObjectsRootTransform;
		m_Farmer.SetTilePosition(Info.m_Object.GetComponent<GoTo>().m_TileCoord);
		base.transform.position = m_Farmer.m_TileCoord.ToWorldPositionTileCentered();
		m_Farmer.SetState(Farmer.State.None);
	}

	private void EngageObject(ActionInfo Info)
	{
		bool flag = true;
		GameStateManager.State baseState = GameStateManager.Instance.GetCurrentState().m_BaseState;
		if (m_Farmer.m_TypeIdentifier == ObjectType.FarmerPlayer && baseState != 0 && baseState != GameStateManager.State.TeachWorker && ((Converter.GetIsTypeConverter(Info.m_Object.m_TypeIdentifier) && Info.m_Object.GetComponent<Converter>().m_State == Converter.State.Idle) || (ResearchStation.GetIsTypeResearchStation(Info.m_Object.m_TypeIdentifier) && Info.m_Object.GetComponent<ResearchStation>().m_State == ResearchStation.State.Idle) || (Info.m_Object.m_TypeIdentifier == ObjectType.SpacePort && Info.m_Object.GetComponent<SpacePort>().m_State == SpacePort.State.Idle)))
		{
			flag = false;
		}
		if (Vehicle.GetIsTypeVehicle(Info.m_Object.m_TypeIdentifier) && ObjectTypeList.Instance.GetIsHoldable(Info.m_Object.m_TypeIdentifier) && Info.m_Object.GetComponent<Holdable>().m_BeingHeld)
		{
			flag = false;
		}
		if (flag)
		{
			m_Farmer.m_EngagedObject = Info.m_Object;
			m_Farmer.SetState(Farmer.State.Engaged);
			AudioManager.Instance.StartEvent("FarmerEngagedObject", m_Farmer);
			m_Farmer.m_EngagedObject.SendAction(new ActionInfo(ActionType.Engaged, default(TileCoord), m_Farmer, "", "", Info.m_ActionType));
			m_Farmer.SetBaggedObject(null);
		}
	}

	private void DisengageObject(ActionInfo Info)
	{
		if ((bool)m_Farmer.m_EngagedObject)
		{
			m_Farmer.m_EngagedObject.SendAction(new ActionInfo(ActionType.Disengaged, default(TileCoord)));
			if (Vehicle.GetIsTypeVehicle(m_Farmer.m_EngagedObject.m_TypeIdentifier))
			{
				TileCoord nearestAdjacentTile = m_Farmer.m_EngagedObject.GetComponent<Vehicle>().GetNearestAdjacentTile(Info.m_Position);
				m_Farmer.UpdatePositionToTilePosition(nearestAdjacentTile);
			}
			AudioManager.Instance.StartEvent("FarmerDisengagedObject", m_Farmer);
			m_Farmer.SetState(Farmer.State.None);
			m_Farmer.m_EngagedObject = null;
			m_Farmer.SetBaggedObject(null);
		}
	}

	public void DoRecharge()
	{
		m_Farmer.SetBaggedObject(null);
		if ((bool)m_CurrentInfo.m_Object && m_CurrentInfo.m_Object.m_TypeIdentifier == ObjectType.Worker && m_CurrentInfo.m_Object.GetComponent<Worker>().m_Energy == 0f)
		{
			m_CurrentInfo.m_Object.GetComponent<Worker>().Recharge();
			Dictionary<BaseClass, int> collection = CollectionManager.Instance.GetCollection("Farmer");
			if (collection != null && collection.Count == 2 && m_CurrentInfo.m_Object.GetComponent<Worker>().m_WorkerInterpreter.GetCurrentScript() == null)
			{
				CeremonyManager.Instance.AddCeremony(CeremonyManager.CeremonyType.FirstBot);
			}
		}
	}

	private void Recharge(ActionInfo Info)
	{
		if (IsObjectCloseEnough())
		{
			m_Farmer.SetState(Farmer.State.Recharge);
		}
	}

	private void Whistle(ActionInfo Info)
	{
	}

	private void CallPets()
	{
		Dictionary<BaseClass, int> collection = CollectionManager.Instance.GetCollection("Pet");
		if (collection == null)
		{
			return;
		}
		foreach (KeyValuePair<BaseClass, int> item in collection)
		{
			item.Key.GetComponent<AnimalPet>().PlayerCall();
		}
	}

	private void Shout(ActionInfo Info)
	{
		if (m_Farmer.m_TypeIdentifier == ObjectType.FarmerPlayer)
		{
			AudioManager.Instance.StartEvent("PlayerShout", m_Farmer);
			CallPets();
		}
		else
		{
			AudioManager.Instance.StartEvent("WorkerShout", m_Farmer);
		}
		WorkerScriptManager.Instance.AddShout(Info.m_Value);
		BaseClass baseClass = ObjectTypeList.Instance.CreateObjectFromIdentifier(ObjectType.Emoticon, base.transform.position, Quaternion.identity);
		baseClass.GetComponent<Emoticon>().SetWorldPosition(base.transform.position + new Vector3(0f, 2f, 0f));
		baseClass.GetComponent<Emoticon>().SetScale(2f);
		baseClass.GetComponent<Emoticon>().SetEmoticon("", 1f, "EmoticonShout");
		if (Info.m_Value.StartsWith("#"))
		{
			string newMessage = "\"" + Info.m_Value.Substring(1) + "\"";
			CeremonyManager.Instance.DoMessage(m_Farmer.GetHumanReadableName(), newMessage, m_Farmer.GetComponent<Worker>());
		}
	}
}
