using SimpleJSON;
using UnityEngine;

public class Carriage : MobileStorageGeneral
{
	public Minecart m_Minecart;

	private GameObject m_Wheels1;

	private GameObject m_Wheels2;

	public static bool GetIsTypeCarriage(ObjectType NewType)
	{
		if (GetIsTypeCarriageGeneral(NewType) || GetIsTypeCarriageLiquid(NewType))
		{
			return true;
		}
		return false;
	}

	public static bool GetIsTypeCarriageGeneral(ObjectType NewType)
	{
		if (NewType == ObjectType.Carriage || NewType == ObjectType.CarriageTrain)
		{
			return true;
		}
		return false;
	}

	public static bool GetIsTypeCarriageLiquid(ObjectType NewType)
	{
		if (NewType == ObjectType.CarriageLiquid)
		{
			return true;
		}
		return false;
	}

	public override void RegisterClass()
	{
		base.RegisterClass();
		ClassManager.Instance.RegisterClass("Carriage", m_TypeIdentifier);
	}

	public override void Restart()
	{
		base.Restart();
		DontShowContents();
	}

	protected new void Awake()
	{
		base.Awake();
	}

	public override void PostCreate()
	{
		base.PostCreate();
		if ((bool)ObjectUtils.FindDeepChild(m_ModelRoot.transform, "Wheels.001"))
		{
			m_Wheels1 = ObjectUtils.FindDeepChild(m_ModelRoot.transform, "Wheels.001").gameObject;
			m_Wheels2 = ObjectUtils.FindDeepChild(m_ModelRoot.transform, "Wheels.002").gameObject;
		}
	}

	public override void Save(JSONNode Node)
	{
		base.Save(Node);
	}

	public override void Load(JSONNode Node)
	{
		base.Load(Node);
	}

	protected override void ActionBeingHeld(Actionable Holder)
	{
		if ((bool)m_Minecart)
		{
			m_Minecart.RemoveCarriage(this);
		}
		base.ActionBeingHeld(Holder);
		base.transform.localPosition = new Vector3(0f, -0.35f, 0f);
		base.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
	}

	protected override void ActionDropped(Actionable PreviousHolder, TileCoord DropLocation)
	{
		base.ActionDropped(PreviousHolder, DropLocation);
		Vector3 localPosition = base.transform.localPosition;
		localPosition.y = 0f;
		base.transform.localPosition = localPosition;
	}

	public void SetMinecart(Minecart NewMinecart)
	{
		m_Minecart = NewMinecart;
	}

	protected override void SetObjectType(ObjectType NewType)
	{
		base.SetObjectType(NewType);
		int weight = Holdable.GetWeight(NewType);
		if (weight > 16)
		{
			SetWeightPenalty(2f);
		}
		else if (weight > 8)
		{
			SetWeightPenalty(1.5f);
		}
		else
		{
			SetWeightPenalty(1f);
		}
	}

	public override ActionType GetActionFromObject(AFO Info)
	{
		if (Info.m_ActionType == AFO.AT.Primary && Info.m_Actioner.GetComponent<Crane>() != null)
		{
			return ActionType.Pickup;
		}
		if (Info.m_ActionType == AFO.AT.AltPrimary || Info.m_ActionType == AFO.AT.AltSecondary)
		{
			return ActionType.Total;
		}
		return base.GetActionFromObject(Info);
	}

	public void UpdateWheels(float RotationDelta)
	{
		Quaternion quaternion = Quaternion.Euler(0f, RotationDelta, 0f);
		if ((bool)m_Wheels1)
		{
			m_Wheels1.transform.localRotation = m_Wheels1.transform.localRotation * quaternion;
			m_Wheels2.transform.localRotation = m_Wheels2.transform.localRotation * quaternion;
		}
	}

	public override bool CanAcceptObject(BaseClass NewObject, ObjectType NewType)
	{
		if (NewType == ObjectType.Folk)
		{
			return false;
		}
		return base.CanAcceptObject(NewObject, NewType);
	}
}
