public class FarmerStateSpawnJump : FarmerStateBase
{
	public override void StartState()
	{
		base.StartState();
	}

	public override void EndState()
	{
		base.EndState();
	}

	public override void SpawnEnd(BaseClass NewObject)
	{
		base.SpawnEnd(NewObject);
		if (m_Farmer.m_SpawnEnd)
		{
			if (m_Farmer.m_EngagedObject != null)
			{
				m_Farmer.SetState(Farmer.State.Engaged);
			}
			else
			{
				m_Farmer.SetState(Farmer.State.None);
			}
		}
	}

	public override void UpdateState()
	{
		base.UpdateState();
	}
}
