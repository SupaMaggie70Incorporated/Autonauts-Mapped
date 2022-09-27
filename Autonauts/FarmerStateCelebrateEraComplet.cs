using UnityEngine;

public class FarmerStateCelebrateEraComplete : FarmerStateBase
{
	public override void StartState()
	{
		base.StartState();
		m_SafetyDelay = 0f;
		m_Farmer.m_FinalPosition = m_Farmer.transform.position;
		m_Farmer.m_FinalPosition.y = 0f;
		m_Farmer.m_FinalRotation = Quaternion.identity;
	}

	public override void EndState()
	{
		base.EndState();
		m_Farmer.transform.position = m_Farmer.m_FinalPosition;
		m_Farmer.transform.rotation = m_Farmer.m_FinalRotation;
	}

	public override void UpdateState()
	{
		base.UpdateState();
		switch ((int)(m_Farmer.m_InterruptStateTimer * 60f) / 10 % 4)
		{
		case 0:
		case 2:
			m_Farmer.transform.position = m_Farmer.m_FinalPosition;
			m_Farmer.transform.rotation = m_Farmer.m_FinalRotation;
			break;
		case 1:
			m_Farmer.transform.position = m_Farmer.m_FinalPosition + new Vector3(0f, 2f, 0f);
			m_Farmer.transform.rotation = m_Farmer.m_FinalRotation * Quaternion.Euler(0f, 0f, 30f);
			break;
		case 3:
			m_Farmer.transform.position = m_Farmer.m_FinalPosition + new Vector3(0f, 2f, 0f);
			m_Farmer.transform.rotation = m_Farmer.m_FinalRotation * Quaternion.Euler(0f, 0f, -30f);
			break;
		}
	}
}
