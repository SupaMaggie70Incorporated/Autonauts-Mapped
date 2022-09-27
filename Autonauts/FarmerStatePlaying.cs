public class FarmerStatePlaying : FarmerStateBase
{
	public override void StartState()
	{
		base.StartState();
		Holdable topObject = m_Farmer.m_FarmerCarry.GetTopObject();
		if ((bool)topObject)
		{
			Instrument component = topObject.GetComponent<Instrument>();
			if ((bool)component)
			{
				component.Play();
			}
		}
	}

	public override void EndState()
	{
		base.EndState();
	}

	public override void UpdateState()
	{
		base.UpdateState();
		DoEndAction();
	}
}
