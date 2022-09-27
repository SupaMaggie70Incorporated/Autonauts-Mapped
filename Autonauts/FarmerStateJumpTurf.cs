using UnityEngine;

public class FarmerStateJumpTurf : FarmerStateBase
{
	private int m_Count;

	public override void StartState()
	{
		base.StartState();
		m_Farmer.StartAnimation("FarmerJumpTurf");
		m_Count = 0;
	}

	public override void DoAnimationAction()
	{
		base.DoAnimationAction();
		ParticlesManager.Instance.CreateParticles("BuildDust", m_Farmer.transform.position + new Vector3(0f, 1f, 0f), Quaternion.Euler(90f, 0f, 0f), AutoDestroy: true);
		AudioManager.Instance.StartEvent("FarmerJumpSeed", m_Farmer);
		m_Count++;
		if (m_Count == 5)
		{
			DoEndAction();
		}
	}
}
