using UnityEngine;

public class Cart : MobileStorageGeneral
{
	[HideInInspector]
	public GameObject m_Wheels;

	[HideInInspector]
	public GameObject m_Wheels2;

	protected bool m_Tilt;

	public static bool GetIsTypeCart(ObjectType NewType)
	{
		if (NewType == ObjectType.Cart || NewType == ObjectType.WheelBarrow || NewType == ObjectType.CartLiquid || NewType == ObjectType.TrojanRabbit)
		{
			return true;
		}
		return false;
	}

	public override void Restart()
	{
		base.Restart();
		if ((bool)m_ModelRoot.transform.Find("Wheels"))
		{
			m_Wheels = m_ModelRoot.transform.Find("Wheels").gameObject;
		}
		else if ((bool)m_ModelRoot.transform.Find("Wheel"))
		{
			m_Wheels = m_ModelRoot.transform.Find("Wheel").gameObject;
		}
		else if ((bool)m_ModelRoot.transform.Find("Wheels.001"))
		{
			m_Wheels = m_ModelRoot.transform.Find("Wheels.001").gameObject;
		}
		if ((bool)m_ModelRoot.transform.Find("Wheels.002"))
		{
			m_Wheels2 = m_ModelRoot.transform.Find("Wheels.002").gameObject;
		}
		SetBaseDelay(0.2f);
		m_ModelRoot.transform.localRotation = Quaternion.Euler(15f, 0f, 0f);
		m_MoveSoundName = "CartMotion";
	}

	public override void SendAction(ActionInfo Info)
	{
		base.SendAction(Info);
		switch (Info.m_Action)
		{
		case ActionType.Engaged:
			m_ModelRoot.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
			break;
		case ActionType.Disengaged:
			m_ModelRoot.transform.localRotation = Quaternion.Euler(15f, 0f, 0f);
			break;
		}
	}

	protected override void SetObjectType(ObjectType NewType)
	{
		base.SetObjectType(NewType);
		int weight = Holdable.GetWeight(NewType);
		if (weight > 8)
		{
			SetWeightPenalty(2f);
		}
		else if (weight > 6)
		{
			SetWeightPenalty(1.5f);
		}
		else
		{
			SetWeightPenalty(1f);
		}
	}

	private void UpdateWheels()
	{
		if ((bool)m_Wheels)
		{
			Quaternion localRotation = m_Wheels.transform.localRotation;
			localRotation *= Quaternion.Euler(600f * TimeManager.Instance.m_NormalDelta, 0f, 0f);
			m_Wheels.transform.localRotation = localRotation;
		}
		if ((bool)m_Wheels2)
		{
			Quaternion localRotation2 = m_Wheels2.transform.localRotation;
			localRotation2 *= Quaternion.Euler(600f * TimeManager.Instance.m_NormalDelta, 0f, 0f);
			m_Wheels2.transform.localRotation = localRotation2;
		}
	}

	protected new void Update()
	{
		base.Update();
		if (m_State == State.Moving && (bool)m_Engager && (m_Engager.m_TypeIdentifier == ObjectType.FarmerPlayer || (m_Engager.m_TypeIdentifier == ObjectType.Worker && m_Engager.GetComponent<Worker>().m_Energy > 0f)))
		{
			UpdateWheels();
		}
	}
}
