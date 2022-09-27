using UnityEngine;

public class StorageLiquidMedium : StorageLiquid
{
	public override void Restart()
	{
		base.Restart();
		SetDimensions(new TileCoord(-1, -2), new TileCoord(1, 0), new TileCoord(0, 1));
	}

	protected new void Awake()
	{
		base.Awake();
		m_Capacity = 1000;
		m_MaxLevels = 1;
	}

	public override void PostCreate()
	{
		base.PostCreate();
		m_Sign = m_ModelRoot.transform.Find("Plane").GetComponent<MeshRenderer>();
	}
}
