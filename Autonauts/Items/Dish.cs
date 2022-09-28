using UnityEngine;

public class Dish : Holdable
{
	public static bool GetIsTypeDish(ObjectType NewType)
	{
		if (NewType == ObjectType.PotClay || NewType == ObjectType.LargeBowlClay || NewType == ObjectType.JarClay)
		{
			return true;
		}
		return false;
	}

	protected override void ActionBeingHeld(Actionable Holder)
	{
		base.ActionBeingHeld(Holder);
		base.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
	}
}
