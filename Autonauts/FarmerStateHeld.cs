public class FarmerStateHeld : FarmerStateBase
{
	public override void StartState()
	{
		base.StartState();
		m_SafetyDelay = 0f;
		if (m_Farmer.m_HolderObject != null)
		{
			m_Farmer.SetRotation(m_Farmer.m_HolderObject.m_Rotation);
		}
	}

	public override void EndState()
	{
		base.EndState();
		if (m_Farmer.m_HolderObject != null)
		{
			m_Farmer.SetRotation(m_Farmer.m_HolderObject.m_Rotation);
		}
	}
}
