using UnityEngine;

public class ToolNet : MyTool
{
	public static bool GetIsTypeNet(ObjectType NewType)
	{
		if (NewType == ObjectType.ToolNet || NewType == ObjectType.ToolNetCrude)
		{
			return true;
		}
		return false;
	}

	public override void RegisterClass()
	{
		base.RegisterClass();
		ClassManager.Instance.RegisterClass("ToolNet", m_TypeIdentifier);
	}

	protected override void ActionBeingHeld(Actionable Holder)
	{
		base.ActionBeingHeld(Holder);
		base.transform.localPosition = new Vector3(0f, 0f, 0f);
		base.transform.localRotation = Quaternion.identity;
	}
}
