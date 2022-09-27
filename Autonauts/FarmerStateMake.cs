using UnityEngine;

public class FarmerStateMake : FarmerStateBase
{
	private PlaySound m_PlaySound;

	public override void StartState()
	{
		base.StartState();
		m_PlaySound = AudioManager.Instance.StartEvent("FarmerMakingInHand", m_Farmer, Remember: true);
	}

	public override void EndState()
	{
		base.EndState();
		AudioManager.Instance.StopEvent(m_PlaySound);
	}

	public override void UpdateState()
	{
		base.UpdateState();
		if (m_Farmer.m_StateTimer > 2f)
		{
			m_Farmer.transform.localScale = new Vector3(1f, 1f, 1f);
			m_Farmer.m_FarmerAction.MakeInHand();
			if ((bool)m_Farmer.GetComponent<FarmerPlayer>())
			{
				m_Farmer.SetState(Farmer.State.CelebrateMakeInHand);
			}
			else
			{
				m_Farmer.SetState(Farmer.State.None);
			}
			if (m_Farmer.GetComponent<Worker>() == null)
			{
				int.Parse(m_Farmer.m_FarmerAction.m_CurrentInfo.m_Value);
			}
		}
		else if ((int)(m_Farmer.m_StateTimer * 60f) % 16 < 8)
		{
			m_Farmer.transform.localScale = new Vector3(1.1f, 0.9f, 1.1f);
		}
		else
		{
			m_Farmer.transform.localScale = new Vector3(0.9f, 1.1f, 0.9f);
		}
	}
}
