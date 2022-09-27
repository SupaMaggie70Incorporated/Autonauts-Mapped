using UnityEngine;

public class ToolFlail : MyTool
{
	[HideInInspector]
	public GameObject m_Hinge;

	public static bool GetIsTypeFlail(ObjectType NewType)
	{
		if (NewType == ObjectType.ToolFlailCrude || NewType == ObjectType.ToolFlail)
		{
			return true;
		}
		return false;
	}

	public override void RegisterClass()
	{
		base.RegisterClass();
		ClassManager.Instance.RegisterClass("ToolFlail", m_TypeIdentifier);
	}

	public override void Restart()
	{
		base.Restart();
		m_Hinge = m_ModelRoot.transform.Find("Hinge").gameObject;
	}

	protected override void ActionBeingHeld(Actionable Holder)
	{
		base.ActionBeingHeld(Holder);
		base.transform.localPosition = new Vector3(0f, 0f, 0f);
		base.transform.localRotation = Quaternion.identity;
	}
}
