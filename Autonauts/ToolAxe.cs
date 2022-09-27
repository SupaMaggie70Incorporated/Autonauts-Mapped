using UnityEngine;

public class ToolAxe : MyTool
{
	public static bool GetIsTypeAxe(ObjectType NewType)
	{
		if (NewType == ObjectType.ToolAxeStone || NewType == ObjectType.ToolAxe)
		{
			return true;
		}
		return false;
	}

	public override void RegisterClass()
	{
		base.RegisterClass();
		ClassManager.Instance.RegisterClass("ToolAxe", m_TypeIdentifier);
	}

	public override void Restart()
	{
		base.Restart();
	}

	protected override void ActionBeingHeld(Actionable Holder)
	{
		base.ActionBeingHeld(Holder);
		base.transform.localPosition = new Vector3(0f, 0f, 0f);
		base.transform.localRotation = Quaternion.Euler(0f, 90f, 0f);
	}
}
