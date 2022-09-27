public class FarmerStateBeingEngaged : FarmerStateBase
{
	public override void StartState()
	{
		base.StartState();
	}

	public override void EndState()
	{
		base.EndState();
	}

	public override void DoAnimationAction()
	{
		DoEndAction();
	}

	public override void UpdateState()
	{
		base.UpdateState();
	}
}
