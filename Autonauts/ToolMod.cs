using UnityEngine;

public class ToolMod : MyTool
{
	public override void RegisterClass()
	{
		base.RegisterClass();
		ClassManager.Instance.RegisterClass("Tool", m_TypeIdentifier);
	}

	protected override void ActionBeingHeld(Actionable Holder)
	{
		base.ActionBeingHeld(Holder);
		base.transform.localPosition = new Vector3(0f, 0f, 0f);
		base.transform.localRotation = Quaternion.Euler(0f, 90f, 0f);
	}

	public override void Restart()
	{
		base.Restart();
		Vector3 Scale = new Vector3(-1f, 1f, 1f);
		ModManager.Instance.ModToolClass.GetModelScale(m_TypeIdentifier, out Scale);
		m_ModelRoot.transform.localScale = new Vector3(Scale.x, Scale.y, Scale.z);
		if (ModManager.Instance.ModToolClass.GetModelRotation(m_TypeIdentifier, out var Rot))
		{
			m_ModelRoot.transform.localRotation = Quaternion.Euler(Rot.x, Rot.y, Rot.z);
		}
		if (ModManager.Instance.ModToolClass.GetModelTranslation(m_TypeIdentifier, out var Trans))
		{
			m_ModelRoot.transform.localPosition = new Vector3(Trans.x, Trans.y, Trans.z);
		}
	}
}
