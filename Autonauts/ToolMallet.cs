using UnityEngine;

public class ToolMallet : MyTool
{
	public override void RegisterClass()
	{
		base.RegisterClass();
		ClassManager.Instance.RegisterClass("ToolMallet", m_TypeIdentifier);
	}

	protected override void ActionBeingHeld(Actionable Holder)
	{
		base.ActionBeingHeld(Holder);
		base.transform.localPosition = new Vector3(0f, 0f, 0f);
		base.transform.localRotation = Quaternion.Euler(0f, 90f, 0f);
	}
}
