using UnityEngine;

public class ToolHoe : MyTool
{
	public static bool GetIsTypeHoe(ObjectType NewType)
	{
		if (NewType == ObjectType.ToolHoeStone || NewType == ObjectType.ToolHoe)
		{
			return true;
		}
		return false;
	}

	public override void RegisterClass()
	{
		base.RegisterClass();
		ClassManager.Instance.RegisterClass("ToolHoe", m_TypeIdentifier);
	}

	protected override void ActionBeingHeld(Actionable Holder)
	{
		base.ActionBeingHeld(Holder);
		base.transform.localPosition = new Vector3(0f, 0f, 0f);
		base.transform.localRotation = Quaternion.identity;
	}
}
