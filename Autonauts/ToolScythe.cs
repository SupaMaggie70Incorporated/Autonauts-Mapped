using UnityEngine;

public class ToolScythe : MyTool
{
	public static bool GetIsTypeScythe(ObjectType NewType)
	{
		if (NewType == ObjectType.ToolScytheStone || NewType == ObjectType.ToolScythe)
		{
			return true;
		}
		return false;
	}

	public override void RegisterClass()
	{
		base.RegisterClass();
		ClassManager.Instance.RegisterClass("ToolScythe", m_TypeIdentifier);
	}

	protected override void ActionBeingHeld(Actionable Holder)
	{
		base.ActionBeingHeld(Holder);
		base.transform.localPosition = new Vector3(0f, 0f, 0f);
		base.transform.localRotation = Quaternion.Euler(0f, 90f, 0f);
	}
}
