using UnityEngine;

public class FlowerBunch : Holdable
{
	[HideInInspector]
	public FlowerWild.Type m_Type;

	public static bool GetIsTypeFlowerBunch(ObjectType NewType)
	{
		if (NewType == ObjectType.FlowerBunch01 || NewType == ObjectType.FlowerBunch02 || NewType == ObjectType.FlowerBunch03 || NewType == ObjectType.FlowerBunch04 || NewType == ObjectType.FlowerBunch05 || NewType == ObjectType.FlowerBunch06 || NewType == ObjectType.FlowerBunch07)
		{
			return true;
		}
		return false;
	}

	public override void RegisterClass()
	{
		base.RegisterClass();
		ClassManager.Instance.RegisterClass("FlowerBunch", m_TypeIdentifier);
	}

	protected override void ActionBeingHeld(Actionable Holder)
	{
		base.ActionBeingHeld(Holder);
		base.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
	}
}
