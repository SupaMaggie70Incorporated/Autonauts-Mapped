using UnityEngine;

public class FarmerEngagedBot : FarmerEngagedBase
{
	private float m_Timer;

	public override void Start()
	{
		m_Timer = 0f;
		m_Farmer.m_FinalPosition = m_Farmer.transform.position;
	}

	public override void End()
	{
		m_Farmer.transform.position = m_Farmer.m_FinalPosition;
	}

	public override void DoAnimationAction()
	{
		m_Farmer.m_States[(int)m_Farmer.m_State].DoEndAction();
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
		m_Timer += TimeManager.Instance.m_NormalDelta;
		if (m_Timer > 1f)
		{
			m_Farmer.m_States[(int)m_Farmer.m_State].DoEndAction();
		}
	}
}
