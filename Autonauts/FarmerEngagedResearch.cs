public class FarmerEngagedResearch : FarmerEngagedBase
{
	public override void Start()
	{
		base.Start();
		m_Farmer.m_EngagedObject.GetComponent<ResearchStation>().ResumeResearch(m_Farmer);
	}
}
