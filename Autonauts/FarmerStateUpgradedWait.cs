public class FarmerStateUpgradedWait : FarmerStateBase
{
	public override void StartState()
	{
		base.StartState();
		m_SafetyDelay = 0f;
	}
}
