using UnityEngine;

public class LinkedSystemMechanicalRollover : GeneralRollover
{
	[HideInInspector]
	public BeltLinkage m_Target;

	private PowerValue m_PowerValue;

	private BaseProgressBar m_CurrentEnergy;

	private PowerValue m_PowerValue2;

	protected new void Awake()
	{
		base.Awake();
		m_Target = null;
		m_PowerValue = m_Panel.transform.Find("PowerValue").GetComponent<PowerValue>();
		m_CurrentEnergy = m_Panel.transform.Find("CurrentEnergy").GetComponent<BaseProgressBar>();
		m_PowerValue2 = m_Panel.transform.Find("PowerValue2").GetComponent<PowerValue>();
		Hide();
	}

	protected override void UpdateTarget()
	{
		if (m_Target.m_LinkedSystem != null)
		{
			LinkedSystemMechanical linkedSystemMechanical = (LinkedSystemMechanical)m_Target.m_LinkedSystem;
			int maxUsedEnergy = linkedSystemMechanical.m_MaxUsedEnergy;
			m_PowerValue.SetValue(maxUsedEnergy);
			float value = 0f;
			if (linkedSystemMechanical.m_TotalPotentialEnergy != 0)
			{
				value = linkedSystemMechanical.m_TotalEnergyAvailable / (float)linkedSystemMechanical.m_TotalPotentialEnergy;
			}
			m_CurrentEnergy.SetValue(value);
			maxUsedEnergy = linkedSystemMechanical.m_TotalPotentialEnergy;
			m_PowerValue2.SetValue(maxUsedEnergy);
		}
	}

	public void SetTarget(BeltLinkage Target)
	{
		if (Target != m_Target)
		{
			m_Target = Target;
			if (m_Target != null)
			{
				UpdateTarget();
			}
		}
	}

	protected override bool GetTargettingSomething()
	{
		return m_Target != null;
	}
}
