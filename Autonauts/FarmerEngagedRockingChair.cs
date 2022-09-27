using UnityEngine;

public class FarmerEngagedRockingChair : FarmerEngagedBase
{
	private RockingChair m_RockingChair;

	public override void Start()
	{
		m_RockingChair = m_Farmer.m_EngagedObject.GetComponent<RockingChair>();
		m_Farmer.m_FinalPosition = m_Farmer.transform.position;
		m_Farmer.m_FinalRotation = m_Farmer.transform.rotation;
	}

	public override void End()
	{
		m_Farmer.transform.position = m_Farmer.m_FinalPosition;
		m_Farmer.transform.rotation = m_Farmer.m_FinalRotation;
	}

	public void UpdateToChair()
	{
		m_Farmer.transform.position = m_RockingChair.m_ModelRoot.transform.TransformPoint(new Vector3(0f, 1f, 0f));
		m_Farmer.transform.rotation = m_RockingChair.m_ModelRoot.transform.rotation;
	}
}
