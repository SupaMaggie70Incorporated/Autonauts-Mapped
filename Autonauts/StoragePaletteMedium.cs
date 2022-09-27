public class StoragePaletteMedium : StoragePalette
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

	public override void SetObjectType(ObjectType NewType)
	{
		if (m_ObjectType != NewType)
		{
			base.SetObjectType(NewType);
			m_Capacity *= 20;
		}
	}
}
