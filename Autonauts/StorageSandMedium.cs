public class StorageSandMedium : StorageSand
{
	public override void Restart()
	{
		base.Restart();
		SetDimensions(new TileCoord(-1, -1), new TileCoord(1, 1), new TileCoord(0, 2));
		SetObjectType(m_ObjectType);
	}

	protected new void Awake()
	{
		base.Awake();
		m_Capacity = 1000;
		m_MaxLevels = 1;
	}
}
