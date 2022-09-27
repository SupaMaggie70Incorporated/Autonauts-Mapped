public class FarmerStateRocketRide : FarmerStateBase
{
	public override void StartState()
	{
		base.StartState();
		m_Farmer.m_ModelRoot.SetActive(value: false);
	}

	public override void EndState()
	{
		base.EndState();
		m_Farmer.m_ModelRoot.SetActive(value: true);
	}

	public override void UpdateState()
	{
		base.UpdateState();
	}
}
