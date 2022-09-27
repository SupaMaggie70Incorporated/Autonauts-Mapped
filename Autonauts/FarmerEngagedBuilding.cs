using UnityEngine;

public class FarmerEngagedBuilding : FarmerEngagedBase
{
	private Converter m_Building;

	private bool m_JumpUp;

	private float m_JumpHeight;

	public override void Start()
	{
		m_Farmer.m_FinalPosition = m_Farmer.transform.position;
		m_Building = m_Farmer.m_EngagedObject.GetComponent<Converter>();
		m_Farmer.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
		m_Building.ResumeConversion(m_Farmer);
		m_JumpHeight = m_Building.m_IngredientsHeight;
		m_JumpUp = false;
	}

	public override void End()
	{
		m_Farmer.transform.position = m_Farmer.m_FinalPosition;
	}

	public override void Update()
	{
		if ((int)(m_Farmer.m_StateTimer * 60f) % 12 < 5)
		{
			m_Farmer.transform.position = m_Building.transform.position + new Vector3(0f, m_JumpHeight, 0f);
			if (m_JumpUp)
			{
				m_JumpUp = false;
				if (m_Building.m_TypeIdentifier == ObjectType.ConverterFoundation)
				{
					m_Building.GetComponent<ConverterFoundation>().FarmerJump();
				}
				ParticlesManager.Instance.CreateParticles("BuildDust", m_Farmer.transform.position, Quaternion.Euler(90f, 0f, 0f), AutoDestroy: true);
				AudioManager.Instance.StartEvent("BuildingBlueprintJump", m_Farmer);
			}
		}
		else
		{
			m_Farmer.transform.position = m_Building.transform.position + new Vector3(0f, m_JumpHeight + 2.5f, 0f);
			m_JumpUp = true;
		}
	}
}
