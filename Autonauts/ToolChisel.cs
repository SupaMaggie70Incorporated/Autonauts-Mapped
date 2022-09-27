using UnityEngine;

public class ToolChisel : MyTool
{
	public static bool GetIsTypeChisel(ObjectType NewType)
	{
		if (NewType == ObjectType.ToolChiselCrude || NewType == ObjectType.ToolChisel)
		{
			return true;
		}
		return false;
	}

	public override void RegisterClass()
	{
		base.RegisterClass();
		ClassManager.Instance.RegisterClass("ToolChisel", m_TypeIdentifier);
	}

	protected override void ActionBeingHeld(Actionable Holder)
	{
		base.ActionBeingHeld(Holder);
		base.transform.localPosition = new Vector3(0f, 0f, 0f);
		base.transform.localRotation = Quaternion.identity;
	}
}
