public class FarmerStateStowing : FarmerStateBase
{
	public override void StartState()
	{
		base.StartState();
	}

	public override void EndState()
	{
		base.EndState();
	}

	public override void UpdateState()
	{
		base.UpdateState();
		if (m_Farmer.m_StateTimer > 0.125f * m_GeneralStateScale)
		{
			m_Farmer.m_FarmerAction.DoStow();
			DoEndAction();
		}
	}
}
