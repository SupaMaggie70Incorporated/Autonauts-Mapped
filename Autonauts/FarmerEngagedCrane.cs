using UnityEngine;

public class FarmerEngagedCrane : FarmerEngagedBase
{
	public override void Start()
	{
		UpdatePosition();
	}

	public override void End()
	{
	}

	private void UpdatePosition()
	{
		Vector3 position = m_Farmer.m_EngagedObject.GetComponent<Crane>().m_DrivePoint.transform.position;
		if (m_Farmer.m_EngagedObject.GetComponent<Vehicle>().m_State == Vehicle.State.Moving)
		{
			float num = 0f;
			if ((int)(m_Farmer.m_StateTimer * 60f) % 12 < 7)
			{
				num = 0.5f;
			}
			position.y += num;
			Quaternion newRotation = Quaternion.Euler(600f * TimeManager.Instance.m_NormalDelta, 0f, 0f);
			m_Farmer.m_EngagedObject.GetComponent<Crane>().AddWheelRotation(newRotation);
		}
		m_Farmer.transform.position = position;
		m_Farmer.transform.rotation = m_Farmer.m_EngagedObject.transform.rotation;
		m_Farmer.SetTilePosition(m_Farmer.m_EngagedObject.GetComponent<TileCoordObject>().m_TileCoord);
	}

	public override void Update()
	{
		UpdatePosition();
	}
}
