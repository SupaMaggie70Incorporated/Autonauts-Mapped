using UnityEngine;

public class ToolLight : MyTool
{
	protected MyLight m_Light;

	public override void RegisterClass()
	{
		base.RegisterClass();
		ClassManager.Instance.RegisterClass("ToolLight", m_TypeIdentifier);
	}

	protected new void Awake()
	{
		base.Awake();
		m_Light = LightManager.Instance.LoadLight("TorchCrudeLight", base.transform, new Vector3(0f, 1f, 0f));
	}

	protected new void OnDestroy()
	{
		LightManager.Instance.DestroyLight(m_Light);
		base.OnDestroy();
	}

	protected override void ActionBeingHeld(Actionable Holder)
	{
		base.ActionBeingHeld(Holder);
		base.transform.localPosition = new Vector3(0f, 0f, 0f);
	}
}
