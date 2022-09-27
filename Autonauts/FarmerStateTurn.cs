public class FarmerStateTurn : FarmerStateBase
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
		m_Farmer.UpdateTurning();
	}
}
