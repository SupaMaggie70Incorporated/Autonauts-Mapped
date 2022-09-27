using UnityEngine;

public class TrainRefuellingStationRollover : GeneralRollover
{
	[HideInInspector]
	public TrainRefuellingStation m_Target;

	private BaseProgressBar m_Fuel;

	private BaseProgressBar m_Water;

	protected new void Awake()
	{
		base.Awake();
		m_Target = null;
		m_Fuel = m_Panel.transform.Find("FuelProgressBar").GetComponent<BaseProgressBar>();
		m_Water = m_Panel.transform.Find("WaterProgressBar").GetComponent<BaseProgressBar>();
		Hide();
	}

	protected override void UpdateTarget()
	{
		m_Fuel.SetValue(m_Target.GetFuelPercent());
		m_Water.SetValue(m_Target.GetWaterPercent());
	}

	public void SetTarget(TrainRefuellingStation Target)
	{
		if (Target != m_Target)
		{
			m_Target = Target;
			if ((bool)m_Target)
			{
				m_Title.SetText(m_Target.GetHumanReadableName());
				UpdateTarget();
			}
		}
	}

	protected override bool GetTargettingSomething()
	{
		return m_Target != null;
	}
}
