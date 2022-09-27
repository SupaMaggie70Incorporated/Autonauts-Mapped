using SimpleJSON;
using UnityEngine;

public class StorageSand : Storage
{
	private Transform m_Hinge;

	private int m_AddedAmount;

	public static bool GetIsTypeStorageSand(ObjectType NewType)
	{
		if (NewType == ObjectType.StorageSand || NewType == ObjectType.StorageSandMedium)
		{
			return true;
		}
		return false;
	}

	public static bool IsObjectTypeAcceptibleStatic(ObjectType NewType)
	{
		if (NewType == ObjectType.Sand || NewType == ObjectType.Soil || NewType == ObjectType.WheatSeed || NewType == ObjectType.CottonSeeds || NewType == ObjectType.BullrushesSeeds || NewType == ObjectType.Mortar || NewType == ObjectType.CarrotSeed)
		{
			return true;
		}
		return false;
	}

	public override void Restart()
	{
		base.Restart();
		SetDimensions(new TileCoord(0, 0), new TileCoord(1, 1), new TileCoord(0, 2));
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
		m_Hinge = m_ModelRoot.transform.Find("Hinge");
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

	protected override void CalcHeight()
	{
		m_LevelHeight = ObjectTypeList.Instance.GetHeight(m_TypeIdentifier) - 0.7f;
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

	private void StartRemoveBucket(AFO Info)
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

	private void EndRemoveBucket(AFO Info)
	{
		Info.m_Object.GetComponent<ToolFillable>().Fill(m_ObjectType, m_AddedAmount);
	}

	private void AbortRemoveBucket(AFO Info)
	{
		AddToStored(m_ObjectType, m_AddedAmount, Info.m_Actioner);
		Info.m_Object.GetComponent<ToolFillable>().Empty(m_AddedAmount);
	}

	private ActionType GetPrimaryActionFromBucket(AFO Info)
	{
		Info.m_StartAction = StartRemoveBucket;
		Info.m_EndAction = EndRemoveBucket;
		Info.m_AbortAction = AbortRemoveBucket;
		Info.m_FarmerState = Farmer.State.Taking;
		if (GetStored() == 0)
		{
			return ActionType.Fail;
		}
		ToolBucket component = Info.m_Object.GetComponent<ToolBucket>();
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

	private void StartAddBucket(AFO Info)
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

	private void EndAddBucket(AFO Info)
	{
		UpdateStored();
	}

	private void AbortAddBucket(AFO Info)
	{
		ReleaseStored(m_ObjectType, Info.m_Actioner, m_AddedAmount);
	}

	private ActionType GetSecondaryActionFromBucket(AFO Info)
	{
		Info.m_StartAction = StartAddBucket;
		Info.m_EndAction = EndAddBucket;
		Info.m_AbortAction = AbortAddBucket;
		Info.m_FarmerState = Farmer.State.Adding;
		ToolBucket component = Info.m_Object.GetComponent<ToolBucket>();
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
		if (Info.m_ActionType == AFO.AT.Primary)
		{
			if (GetTopBuilding().m_TypeIdentifier == ObjectType.ConverterFoundation)
			{
				return ActionType.Fail;
			}
			if (ToolBucket.GetIsTypeBucket(Info.m_ObjectType))
			{
				return GetPrimaryActionFromBucket(Info);
			}
		}
		else if (Info.m_ActionType == AFO.AT.Secondary)
		{
			if (GetTopBuilding().m_TypeIdentifier == ObjectType.ConverterFoundation)
			{
				return ActionType.Fail;
			}
			if (ToolBucket.GetIsTypeBucket(Info.m_ObjectType))
			{
				return GetSecondaryActionFromBucket(Info);
			}
		}
		return base.GetActionFromObject(Info);
	}

	protected override bool CanAcceptObject(BaseClass NewObject, ObjectType NewType)
	{
		if (!IsObjectTypeAcceptible(NewType))
		{
			return false;
		}
		if (NewType != m_ObjectType && GetStored() != 0)
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
