using UnityEngine;

public class ToolFishingStick : ToolFillable
{
	public override void RegisterClass()
	{
		base.RegisterClass();
		ClassManager.Instance.RegisterClass("ToolFishingStick", m_TypeIdentifier);
	}

	public override void Restart()
	{
		base.Restart();
	}

	public override bool CanAcceptObjectType(ObjectType NewType)
	{
		if (NewType != ObjectType.FishBait)
		{
			return false;
		}
		return base.CanAcceptObjectType(NewType);
	}

	protected override void ActionBeingHeld(Actionable Holder)
	{
		base.ActionBeingHeld(Holder);
		base.transform.localPosition = new Vector3(0f, 0f, 0f);
		base.transform.localRotation = Quaternion.Euler(0f, 90f, 0f);
	}
}
