using UnityEngine;

public class FlowerSeeds : Holdable
{
	[HideInInspector]
	public FlowerWild.Type m_Type;

	public static bool GetIsTypeFlowerSeeds(ObjectType NewType)
	{
		if (NewType == ObjectType.FlowerSeeds01 || NewType == ObjectType.FlowerSeeds02 || NewType == ObjectType.FlowerSeeds03 || NewType == ObjectType.FlowerSeeds04 || NewType == ObjectType.FlowerSeeds05 || NewType == ObjectType.FlowerSeeds06 || NewType == ObjectType.FlowerSeeds07)
		{
			return true;
		}
		return false;
	}

	public override void RegisterClass()
	{
		base.RegisterClass();
		ClassManager.Instance.RegisterClass("FlowerSeeds", m_TypeIdentifier);
	}

	protected override void ActionBeingHeld(Actionable Holder)
	{
		base.ActionBeingHeld(Holder);
		base.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
	}
}
