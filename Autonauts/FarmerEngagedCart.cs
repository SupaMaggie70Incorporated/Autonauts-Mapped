using UnityEngine;

public class FarmerEngagedCart : FarmerEngagedBase
{
	public override void Start()
	{
	}

	public override void End()
	{
	}

	public override void Update()
	{
		Vector3 position = m_Farmer.m_EngagedObject.transform.TransformPoint(new Vector3(0f, 0f, Tile.m_Size));
		if (m_Farmer.m_EngagedObject.GetComponent<Vehicle>().m_State == Vehicle.State.Moving)
		{
			float num = 0f;
			float x = 0f;
			if ((int)(m_Farmer.m_StateTimer * 60f) % 12 < 7)
			{
				num = 0.5f;
				x = -5f;
			}
			position.y += num;
			if (m_Farmer.m_EngagedObject.m_TypeIdentifier != ObjectType.TrojanRabbit)
			{
				m_Farmer.m_EngagedObject.m_ModelRoot.transform.localRotation = Quaternion.Euler(x, 0f, 0f);
			}
		}
		m_Farmer.transform.position = position;
		m_Farmer.transform.rotation = m_Farmer.m_EngagedObject.transform.rotation;
		m_Farmer.SetTilePosition(m_Farmer.m_EngagedObject.GetComponent<TileCoordObject>().m_TileCoord);
	}
}
