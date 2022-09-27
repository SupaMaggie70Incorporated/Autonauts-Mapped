using UnityEngine;

public class FarmerEngagedGeneric : FarmerEngagedBase
{
	public override void Start()
	{
		m_Farmer.m_FinalPosition = m_Farmer.transform.position;
	}

	public override void End()
	{
		m_Farmer.transform.position = m_Farmer.m_FinalPosition;
	}

	public override void Update()
	{
		if ((int)(m_Farmer.m_StateTimer * 60f) % 10 < 5)
		{
			m_Farmer.transform.position = m_Farmer.m_FinalPosition;
		}
		else
		{
			m_Farmer.transform.position = m_Farmer.m_FinalPosition + new Vector3(0f, 1f, 0f);
		}
	}
}
