using UnityEngine;

public class ToolBroom : MyTool
{
	public override void RegisterClass()
	{
		base.RegisterClass();
		ClassManager.Instance.RegisterClass("ToolBroom", m_TypeIdentifier);
	}

	public override void Restart()
	{
		base.Restart();
	}

	protected override void ActionBeingHeld(Actionable Holder)
	{
		base.ActionBeingHeld(Holder);
		base.transform.localPosition = new Vector3(0f, 0f, 0f);
		base.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
	}

	protected override void ActionDropped(Actionable PreviousHolder, TileCoord DropLocation)
	{
		base.ActionDropped(PreviousHolder, DropLocation);
		base.transform.localPosition = base.transform.localPosition + new Vector3(0f, 0.6f, 0f);
	}

	public static bool GetIsObjectTypeAcceptable(ObjectType NewType)
	{
		if (NewType == ObjectType.Folk || NewType == ObjectType.FolkSeed)
		{
			return false;
		}
		if (Animal.GetIsTypeAnimal(NewType))
		{
			return false;
		}
		if (Vehicle.GetIsTypeVehicle(NewType))
		{
			return false;
		}
		if (NewType == ObjectType.Worker)
		{
			return false;
		}
		if (ObjectTypeList.Instance.GetWeight(NewType) > 2)
		{
			return false;
		}
		return true;
	}

	public static bool GetIsObjectAcceptable(Holdable NewObject)
	{
		return GetIsObjectTypeAcceptable(NewObject.m_TypeIdentifier);
	}
}
