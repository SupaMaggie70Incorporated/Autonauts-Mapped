public class FarmerStateRocketKicked : FarmerStateBase
{
	private TrailMaker2 m_Trail;

	public override void StartState()
	{
		base.StartState();
		m_Farmer.StartAnimation("FarmerRocketKicked");
		m_Trail = TrailManager.Instance.StartTrail(m_Farmer.m_ModelRoot.transform);
		m_Farmer.m_ModelRoot.SetActive(value: false);
	}

	public override void DoAnimationAction()
	{
		base.DoAnimationAction();
		TrailManager.Instance.StopTrail(m_Trail);
		DoEndAction();
	}

	public override void UpdateState()
	{
		base.UpdateState();
		m_Farmer.m_ModelRoot.SetActive(value: true);
	}
}
