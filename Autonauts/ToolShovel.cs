using UnityEngine;

public class ToolShovel : MyTool
{
	public static bool GetIsTypeShovel(ObjectType NewType)
	{
		if (NewType == ObjectType.ToolShovelStone || NewType == ObjectType.ToolShovel)
		{
			return true;
		}
		return false;
	}

	public override void RegisterClass()
	{
		base.RegisterClass();
		ClassManager.Instance.RegisterClass("ToolShovel", m_TypeIdentifier);
	}

	protected override void ActionBeingHeld(Actionable Holder)
	{
		base.ActionBeingHeld(Holder);
		base.transform.localPosition = new Vector3(0f, 0f, 0f);
		base.transform.localRotation = Quaternion.identity;
	}
}
