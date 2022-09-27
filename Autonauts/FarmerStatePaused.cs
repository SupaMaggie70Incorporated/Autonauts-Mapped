public class FarmerStatePaused : FarmerStateBase
{
	public override void StartState()
	{
		base.StartState();
		m_SafetyDelay = 0f;
	}

	public override void EndState()
	{
	}

	public void SetListening(bool Listening)
	{
		m_Farmer.GetComponent<Worker>().SetOccluded(Listening);
		if (Listening)
		{
			m_Farmer.GetComponent<Worker>().CreateColliders();
		}
		else
		{
			m_Farmer.GetComponent<Worker>().DestroyColliders();
		}
	}
}
