public class FarmerStateWorkerSelect : FarmerStateBase
{
	public override void StartState()
	{
		base.StartState();
		m_SafetyDelay = 0f;
		m_Farmer.GetComponent<FarmerPlayer>().m_FarmerPlayerWorkerSelect.SetActive(Active: true);
	}

	public override void EndState()
	{
		base.EndState();
		m_Farmer.GetComponent<FarmerPlayer>().m_FarmerPlayerWorkerSelect.SetActive(Active: false);
	}
}
