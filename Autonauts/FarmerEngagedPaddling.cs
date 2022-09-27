using UnityEngine;

public class FarmerEngagedPaddling : FarmerEngagedBase
{
	private GameObject m_RightHand;

	private GameObject m_RightHandPoint;

	private Vector3 m_HandPosition;

	private int m_OldFrame;

	private bool m_Player;

	public override void Start()
	{
		m_Player = false;
		if ((bool)m_Farmer.GetComponent<FarmerPlayer>())
		{
			m_Player = true;
		}
		if (m_Player)
		{
			m_RightHand = m_Farmer.m_ModelRoot.transform.Find("HandRight").gameObject;
			m_HandPosition = m_RightHand.transform.localPosition;
			m_RightHandPoint = m_Farmer.m_ModelRoot.transform.Find("ToolCarryPoint").gameObject;
			m_OldFrame = 0;
		}
	}

	public override void End()
	{
		if (m_Player)
		{
			m_RightHand.transform.localPosition = m_HandPosition;
			m_RightHandPoint.transform.localPosition = m_HandPosition;
			m_RightHandPoint.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
		}
	}

	public override void Update()
	{
		m_Farmer.transform.position = m_Farmer.m_EngagedObject.transform.TransformPoint(new Vector3(0f, 0f, 0f));
		m_Farmer.transform.rotation = m_Farmer.m_EngagedObject.transform.rotation;
		m_Farmer.SetTilePosition(m_Farmer.m_EngagedObject.GetComponent<Canoe>().m_TileCoord);
		if (!m_Player)
		{
			return;
		}
		if (m_Farmer.m_EngagedObject.GetComponent<Canoe>().m_State == Vehicle.State.Moving)
		{
			int num = (int)(m_Farmer.m_StateTimer * 60f) % 18 / 6;
			Vector3 localPosition = num switch
			{
				0 => m_HandPosition + new Vector3(0f, 1f, -1f), 
				1 => m_HandPosition + new Vector3(0f, 0.25f, -0.25f), 
				_ => m_HandPosition + new Vector3(0f, 0.25f, 0.5f), 
			};
			m_RightHandPoint.transform.localPosition = localPosition;
			m_RightHand.transform.localPosition = localPosition;
			if (num == 1 && m_OldFrame == 0)
			{
				AudioManager.Instance.StartEvent("ToolPaddleSplash", m_Farmer);
				Vector3 position = m_RightHand.transform.TransformPoint(default(Vector3));
				position.y = 0f;
				m_Farmer.CreateParticles(position, "Splash");
			}
			m_OldFrame = num;
		}
		else
		{
			m_RightHand.transform.localPosition = m_HandPosition;
			m_RightHandPoint.transform.localPosition = m_HandPosition;
		}
	}
}
