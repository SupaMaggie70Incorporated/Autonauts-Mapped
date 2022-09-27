using UnityEngine;

public class ToolPick : MyTool
{
	public static bool GetIsTypePick(ObjectType NewType)
	{
		if (NewType == ObjectType.ToolPickStone || NewType == ObjectType.ToolPick)
		{
			return true;
		}
		return false;
	}

	public override void RegisterClass()
	{
		base.RegisterClass();
		ClassManager.Instance.RegisterClass("ToolPick", m_TypeIdentifier);
	}

	protected override void ActionBeingHeld(Actionable Holder)
	{
		base.ActionBeingHeld(Holder);
		base.transform.localPosition = new Vector3(0f, 0f, 0f);
		base.transform.localRotation = Quaternion.Euler(0f, 90f, 0f);
	}
}
