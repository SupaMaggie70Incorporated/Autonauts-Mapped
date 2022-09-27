using UnityEngine;

public class ToolWateringCan : ToolFillable
{
	private GameObject m_Water;

	public static bool GetIsTypeWateringCan(ObjectType NewType)
	{
		if (NewType == ObjectType.ToolWateringCan)
		{
			return true;
		}
		return false;
	}

	public override void RegisterClass()
	{
		base.RegisterClass();
		ClassManager.Instance.RegisterClass("ToolWateringCan", m_TypeIdentifier);
	}

	public override void Restart()
	{
		base.Restart();
	}

	public override void PostCreate()
	{
		base.PostCreate();
		m_Water = m_ModelRoot.transform.Find("Water").gameObject;
	}

	protected override void ActionBeingHeld(Actionable Holder)
	{
		base.ActionBeingHeld(Holder);
		base.transform.localPosition = new Vector3(0f, 0f, -1f);
		base.transform.localRotation = Quaternion.Euler(0f, -90f, 0f);
	}

	protected override void UpdateContentsModel()
	{
		base.UpdateContentsModel();
		m_Water.SetActive(m_HeldType == ObjectType.Water);
		m_Water.GetComponent<MeshRenderer>().enabled = m_HeldType == ObjectType.Water;
	}

	public override void Fill(ObjectType NewType, int Amount)
	{
		base.Fill(NewType, Amount);
		AudioManager.Instance.StartEvent("ToolWateringCanFill", this);
	}

	public override void Empty(int Amount)
	{
		base.Empty(Amount);
		AudioManager.Instance.StartEvent("ToolWateringCanEmpty", this);
	}

	public override bool CanAcceptObjectType(ObjectType NewType)
	{
		if (NewType != ObjectType.Water && NewType != ObjectType.SeaWater)
		{
			return false;
		}
		return base.CanAcceptObjectType(NewType);
	}
}
