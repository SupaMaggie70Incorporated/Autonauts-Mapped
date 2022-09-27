using SimpleJSON;
using UnityEngine;

public class GiantWaterWheel : LinkedSystemEngine
{
	private GameObject m_Wheel1;

	private GameObject m_Wheel2;

	private PlaySound m_PlaySound;

	private bool m_Fast;

	[HideInInspector]
	public float m_EnergyTimer;

	private float m_Speed;

	public override void Restart()
	{
		base.Restart();
		SetDimensions(new TileCoord(-2, -1), new TileCoord(2, 1), new TileCoord(0, 2));
		HideAccessModel();
		m_EnergyCapacity = VariableManager.Instance.GetVariableAsInt(ObjectType.GiantWaterWheel, "EnergyCapacity");
		m_Wheel1 = m_ModelRoot.transform.Find("Stage1/Wheel").gameObject;
		m_Wheel2 = m_ModelRoot.transform.Find("Stage2/BladeSet").gameObject;
		m_PlaySound = null;
		m_Fast = true;
		m_EnergyTimer = 0f;
		m_PulleySide = 3;
	}

	public override void StopUsing(bool AndDestroy = true)
	{
		base.StopUsing(AndDestroy);
		StopSound();
	}

	public override void Save(JSONNode Node)
	{
		base.Save(Node);
		JSONUtils.Set(Node, "EnergyTimer", m_EnergyTimer);
	}

	public override void Load(JSONNode Node)
	{
		base.Load(Node);
		m_EnergyTimer = JSONUtils.GetAsFloat(Node, "EnergyTimer", 0f);
	}

	private void StopSound()
	{
		if (m_PlaySound != null)
		{
			AudioManager.Instance.StopEvent(m_PlaySound);
			m_PlaySound = null;
		}
	}

	private void UpdateSpeed()
	{
		float targetSpeed = GetTargetSpeed();
		float num = 1f * TimeManager.Instance.m_NormalDelta;
		if (m_Speed < targetSpeed)
		{
			m_Speed += num;
			if (m_Speed >= targetSpeed)
			{
				m_Speed = targetSpeed;
			}
		}
		else if (m_Speed > targetSpeed)
		{
			m_Speed -= num;
			if (m_Speed <= targetSpeed)
			{
				m_Speed = targetSpeed;
			}
		}
	}

	private float GetTargetSpeed()
	{
		if (m_EnergyUsedTimer > 0f)
		{
			return 1f;
		}
		if (m_Energy == 0)
		{
			return 0.025f;
		}
		return 0.05f;
	}

	private float GetSpeed()
	{
		return m_Speed;
	}

	private void UpdateBlades()
	{
		float speed = GetSpeed();
		speed = 720f * TimeManager.Instance.m_NormalDelta * speed;
		m_Wheel1.transform.localRotation = Quaternion.Euler(0f, speed, 0f) * m_Wheel1.transform.localRotation;
		m_Wheel2.transform.localRotation = m_Wheel1.transform.localRotation;
		bool flag = false;
		if (m_EnergyUsedTimer > 0f)
		{
			flag = true;
		}
		if (flag != m_Fast)
		{
			m_Fast = flag;
			if (m_PlaySound != null)
			{
				AudioManager.Instance.StopEvent(m_PlaySound);
			}
			if (flag)
			{
				m_PlaySound = AudioManager.Instance.StartEvent("BuildingWindmillMaking", this, Remember: true);
			}
			else
			{
				m_PlaySound = AudioManager.Instance.StartEvent("BuildingWindmillIdle", this, Remember: true);
			}
		}
	}

	private void UpdateRecharge()
	{
		if (m_Energy != m_EnergyCapacity)
		{
			m_EnergyTimer += TimeManager.Instance.m_NormalDelta;
			if (m_EnergyTimer >= VariableManager.Instance.GetVariableAsFloat(m_TypeIdentifier, "RechargeTime"))
			{
				AddEnergy(m_EnergyCapacity - m_Energy);
			}
		}
	}

	public override void UseEnergy(int Amount)
	{
		base.UseEnergy(Amount);
		m_EnergyTimer = 0f;
	}

	protected new void Update()
	{
		base.Update();
		UpdateSpeed();
		if (TimeManager.Instance.m_NormalTimeEnabled)
		{
			UpdateBlades();
		}
		UpdateRecharge();
	}
}
