using SimpleJSON;
using UnityEngine;

public class StorageLiquid : Storage
{
	private Transform m_Hinge;

	private int m_AddedAmount;

	public static bool GetIsTypeStorageLiquid(ObjectType NewType)
	{
		if (NewType == ObjectType.StorageLiquid || NewType == ObjectType.StorageLiquidMedium)
		{
			return true;
		}
		return false;
	}

	public static bool IsObjectTypeAcceptibleStatic(ObjectType NewType)
	{
		if (NewType == ObjectType.Water || NewType == ObjectType.Milk || NewType == ObjectType.Honey || NewType == ObjectType.SeaWater)
		{
			return true;
		}
		return false;
	}

	public override void Restart()
	{
		base.Restart();
		SetDimensions(new TileCoord(0, 0), new TileCoord(0, 0), new TileCoord(0, 1));
		SetObjectType(m_ObjectType);
	}

	protected new void Awake()
	{
		base.Awake();
		m_ObjectType = ObjectTypeList.m_Total;
		m_Capacity = 100;
		m_MaxLevels = 3;
		m_NumLevels = 1;
	}

	public override void PostCreate()
	{
		base.PostCreate();
		if ((bool)m_ModelRoot.transform.Find("Hinge"))
		{
			m_Hinge = m_ModelRoot.transform.Find("Hinge");
			m_Sign = m_Hinge.transform.Find("Plane").GetComponent<MeshRenderer>();
		}
	}

	public override void Load(JSONNode Node)
	{
		string asString = JSONUtils.GetAsString(Node, "ObjectType", "Unknown");
		if (asString == "Unknown")
		{
			m_ObjectType = ObjectTypeList.m_Total;
		}
		else
		{
			m_ObjectType = ObjectTypeList.Instance.GetIdentifierFromSaveName(asString, Check: false);
		}
		SetObjectType(m_ObjectType);
		base.Load(Node);
	}

	public override void StopUsing(bool AndDestroy = true)
	{
		base.StopUsing(AndDestroy);
		m_ObjectType = ObjectTypeList.m_Total;
	}

	public override bool GetNewLevelAllowed(Building NewBuilding)
	{
		return GetNewLevelAllowedStacked(NewBuilding);
	}

	public override void SetObjectType(ObjectType NewType)
	{
		base.SetObjectType(NewType);
		if (m_ObjectType != ObjectTypeList.m_Total)
		{
			ResourceManager.Instance.UnRegisterStorage(this);
		}
		m_ObjectType = NewType;
		SetSign(NewType);
	}

	private void StartRemoveFillable(AFO Info)
	{
		ToolFillable component = Info.m_Object.GetComponent<ToolFillable>();
		m_AddedAmount = component.GetSpace();
		if (m_AddedAmount > m_Stored)
		{
			m_AddedAmount = m_Stored;
		}
		ReleaseStored(m_ObjectType, Info.m_Actioner, m_AddedAmount);
		AddAnimationManager.Instance.Add(this, Adding: false);
		AudioManager.Instance.StartEvent("BuildingStorageTake", this);
		GetTopBuilding().GetComponent<Storage>().StartOpenLid();
		UpdateStored();
	}

	private void EndRemoveFillable(AFO Info)
	{
		Info.m_Object.GetComponent<ToolFillable>().Fill(m_ObjectType, m_AddedAmount);
	}

	private void AbortRemoveFillable(AFO Info)
	{
		AddToStored(m_ObjectType, m_AddedAmount, Info.m_Actioner);
		Info.m_Object.GetComponent<ToolFillable>().Empty(m_AddedAmount);
	}

	private ActionType GetPrimaryActionFromFillable(AFO Info)
	{
		Info.m_StartAction = StartRemoveFillable;
		Info.m_EndAction = EndRemoveFillable;
		Info.m_AbortAction = AbortRemoveFillable;
		Info.m_FarmerState = Farmer.State.Taking;
		if (GetStored() == 0)
		{
			return ActionType.Fail;
		}
		ToolFillable component = Info.m_Object.GetComponent<ToolFillable>();
		if (component == null)
		{
			return ActionType.Fail;
		}
		if (!component.CanAcceptObjectType(m_ObjectType))
		{
			return ActionType.Fail;
		}
		return ActionType.TakeResource;
	}

	private void StartAddFillable(AFO Info)
	{
		if (!(Info.m_Object == null))
		{
			ToolFillable component = Info.m_Object.GetComponent<ToolFillable>();
			int num = component.m_Stored;
			if (m_Stored + num > GetCapacity())
			{
				num = GetCapacity() - m_Stored;
			}
			ObjectType heldType = component.m_HeldType;
			component.Empty(num);
			if (m_ObjectType != heldType)
			{
				SetObjectType(heldType);
			}
			AddToStored(heldType, num, Info.m_Actioner);
			m_AddedAmount = num;
			AddAnimationManager.Instance.Add(this, Adding: true);
			AudioManager.Instance.StartEvent("BuildingStorageAdd", this);
			GetTopBuilding().GetComponent<Storage>().StartOpenLid();
		}
	}

	private void EndAddFillable(AFO Info)
	{
		UpdateStored();
	}

	private void AbortAddFillable(AFO Info)
	{
		ReleaseStored(m_ObjectType, Info.m_Actioner, m_AddedAmount);
	}

	private ActionType GetSecondaryActionFromFillable(AFO Info)
	{
		Info.m_StartAction = StartAddFillable;
		Info.m_EndAction = EndAddFillable;
		Info.m_AbortAction = AbortAddFillable;
		Info.m_FarmerState = Farmer.State.Adding;
		ToolFillable component = Info.m_Object.GetComponent<ToolFillable>();
		if (component == null)
		{
			return ActionType.Fail;
		}
		if (component.m_HeldType == ObjectTypeList.m_Total)
		{
			return ActionType.Fail;
		}
		if (!IsObjectTypeAcceptible(component.m_HeldType))
		{
			return ActionType.Total;
		}
		if (!CanAcceptObject(null, component.m_HeldType))
		{
			return ActionType.Fail;
		}
		if (m_ObjectType != component.m_HeldType && GetStored() > 0)
		{
			return ActionType.Fail;
		}
		int stored = GetStored(CheckStack: true, CheckReserved: false);
		int capacity = GetCapacity();
		if (stored >= capacity)
		{
			return ActionType.Fail;
		}
		return ActionType.AddResource;
	}

	public override ActionType GetActionFromObject(AFO Info)
	{
		if (Info.m_ActionType == AFO.AT.AltPrimary && Info.m_Actioner.m_TypeIdentifier == ObjectType.FarmerPlayer)
		{
			ActionType actionFromNothing = GetActionFromNothing(Info);
			if (actionFromNothing != ActionType.Total)
			{
				return actionFromNothing;
			}
		}
		ActionType actionType = CheckReset(Info);
		if (actionType != ActionType.Total)
		{
			return actionType;
		}
		return base.GetActionFromObject(Info);
	}

	protected override bool CanAcceptObject(BaseClass NewObject, ObjectType NewType)
	{
		if (!IsObjectTypeAcceptible(NewType))
		{
			return false;
		}
		return base.CanAcceptObject(NewObject, NewType);
	}

	protected override void Update()
	{
		base.Update();
	}

	public override void StartOpenLid()
	{
		base.StartOpenLid();
		if ((bool)m_Hinge)
		{
			m_Hinge.localRotation = Quaternion.Euler(-20f, 0f, 180f);
		}
	}

	public override void CloseLid()
	{
		base.CloseLid();
		if ((bool)m_Hinge)
		{
			m_Hinge.localRotation = Quaternion.Euler(-90f, 0f, 180f);
		}
	}

	protected override bool IsObjectTypeAcceptible(ObjectType NewType)
	{
		return IsObjectTypeAcceptibleStatic(NewType);
	}
}
