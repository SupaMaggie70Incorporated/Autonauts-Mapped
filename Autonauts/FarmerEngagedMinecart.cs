using UnityEngine;

public class FarmerEngagedMinecart : FarmerEngagedBase
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
		Vector3 engagerPosition = m_Farmer.m_EngagedObject.GetComponent<Minecart>().GetEngagerPosition();
		m_Farmer.transform.position = engagerPosition;
		m_Farmer.transform.rotation = m_Farmer.m_EngagedObject.transform.rotation;
		m_Farmer.SetTilePosition(m_Farmer.m_EngagedObject.GetComponent<TileCoordObject>().m_TileCoord);
	}

	public override void Update()
	{
		UpdatePosition();
	}
}
