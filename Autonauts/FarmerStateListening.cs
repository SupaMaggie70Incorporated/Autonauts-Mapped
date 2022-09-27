public class FarmerStateListening : FarmerStateBase
{
	public override void StartState()
	{
		base.StartState();
		m_SafetyDelay = 0f;
		m_Farmer.GetComponent<Worker>().SetOccluded(Occluded: true);
		m_Farmer.StartAnimation("FarmerListening");
		m_Farmer.GetComponent<Worker>().CreateColliders();
	}

	public void SetHighlighted(bool Highlighted)
	{
		if (Highlighted)
		{
			m_Farmer.StartAnimation("FarmerIdle");
		}
		else
		{
			m_Farmer.StartAnimation("FarmerListening");
		}
	}

	public override void EndState()
	{
		base.EndState();
		m_Farmer.GetComponent<Worker>().DestroyColliders();
		m_Farmer.GetComponent<Worker>().SetOccluded(Occluded: false);
	}
}
