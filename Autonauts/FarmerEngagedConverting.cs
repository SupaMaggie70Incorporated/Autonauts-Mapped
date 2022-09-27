using UnityEngine;

public class FarmerEngagedConverting : FarmerEngagedBase
{
	public override void Start()
	{
		Converter component = m_Farmer.m_EngagedObject.GetComponent<Converter>();
		float accessRotationInDegrees = component.GetAccessRotationInDegrees();
		m_Farmer.transform.rotation = Quaternion.Euler(0f, accessRotationInDegrees - 90f, 0f);
		m_Farmer.m_FinalPosition = m_Farmer.transform.position;
		component.ResumeConversion(m_Farmer);
		if (m_Farmer.m_EngagedObject.GetComponent<Converter>().AreRequrementsMet())
		{
			m_Farmer.StartAnimation("FarmerEngagedConverterUsing");
		}
		else
		{
			m_Farmer.StartAnimation("FarmerEngagedConverterSetting");
		}
	}

	public override void End()
	{
		m_Farmer.transform.position = m_Farmer.m_FinalPosition;
	}
}
