using UnityEngine;

public class StationarySteamEngineRollover : GeneralRollover
{
	[HideInInspector]
	public LinkedSystemEngine m_Target;

	private BaseProgressBar m_Fuel;

	private BaseImage m_FuelImage;

	private BaseProgressBar m_Water;

	private BaseImage m_WaterImage;

	private BaseProgressBar m_RechargeTimer;

	private BaseImage m_RechargeImage;

	private PowerValue m_PowerValue;

	protected new void Awake()
	{
		base.Awake();
		m_Target = null;
		m_Fuel = m_Panel.transform.Find("FuelProgressBar").GetComponent<BaseProgressBar>();
		m_FuelImage = m_Panel.transform.Find("FuelImage").GetComponent<BaseImage>();
		m_Water = m_Panel.transform.Find("WaterProgressBar").GetComponent<BaseProgressBar>();
		m_WaterImage = m_Panel.transform.Find("WaterImage").GetComponent<BaseImage>();
		m_RechargeTimer = m_Panel.transform.Find("RechargeProgressBar").GetComponent<BaseProgressBar>();
		m_RechargeImage = m_Panel.transform.Find("RechargeImage").GetComponent<BaseImage>();
		m_PowerValue = m_Panel.transform.Find("PowerValue").GetComponent<PowerValue>();
		Hide();
	}

	protected override void UpdateTarget()
	{
		float num = m_Target.m_Energy;
		float num2 = m_Target.m_EnergyCapacity;
		float value = num / num2;
		m_Fuel.SetValue(value);
		if (m_Target.m_TypeIdentifier == ObjectType.StationaryEngine)
		{
			float water = m_Target.GetComponent<StationaryEngine>().m_Water;
			num2 = m_Target.GetComponent<StationaryEngine>().m_WaterCapacity;
			value = water / num2;
			m_Water.SetValue(value);
		}
		else
		{
			float num3 = 0f;
			if (m_Target.m_TypeIdentifier == ObjectType.Windmill)
			{
				num3 = m_Target.GetComponent<Windmill>().m_EnergyTimer;
			}
			if (m_Target.m_TypeIdentifier == ObjectType.GiantWaterWheel)
			{
				num3 = m_Target.GetComponent<GiantWaterWheel>().m_EnergyTimer;
			}
			float variableAsFloat = VariableManager.Instance.GetVariableAsFloat(m_Target.m_TypeIdentifier, "RechargeTime");
			value = num3 / variableAsFloat;
			m_RechargeTimer.SetValue(value);
		}
		int value2 = (int)m_Target.GetEnergy();
		m_PowerValue.SetValue(value2);
	}

	public void SetTarget(LinkedSystemEngine Target)
	{
		if (!(Target != m_Target))
		{
			return;
		}
		m_Target = Target;
		if ((bool)m_Target)
		{
			bool flag = true;
			if (m_Target.m_TypeIdentifier == ObjectType.StationaryEngine)
			{
				flag = false;
			}
			m_Title.SetText(ObjectTypeList.Instance.GetHumanReadableNameFromIdentifier(m_Target.m_TypeIdentifier));
			m_Fuel.SetActive(!flag);
			m_FuelImage.SetActive(!flag);
			m_Water.SetActive(!flag);
			m_WaterImage.SetActive(!flag);
			m_RechargeTimer.SetActive(flag);
			m_RechargeImage.SetActive(flag);
			Sprite icon = IconManager.Instance.GetIcon(Target.m_TypeIdentifier);
			m_RechargeImage.SetSprite(icon);
			UpdateTarget();
		}
	}

	protected override bool GetTargettingSomething()
	{
		return m_Target != null;
	}
}
