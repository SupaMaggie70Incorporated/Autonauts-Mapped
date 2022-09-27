public class StorageGenericMedium : StorageGeneric
{
	public override void Restart()
	{
		base.Restart();
		SetDimensions(new TileCoord(-1, -2), new TileCoord(1, 0), new TileCoord(0, 1));
		m_ObjectType = ObjectTypeList.m_Total;
	}

	protected new void Awake()
	{
		base.Awake();
		m_MaxLevels = 1;
	}

	public override void SetObjectType(ObjectType NewType)
	{
		base.SetObjectType(NewType);
		int tier = ObjectTypeList.Instance.GetTier(NewType);
		if (tier < 4)
		{
			m_Capacity *= 40;
		}
		else if (tier == 4)
		{
			m_Capacity *= 20;
		}
		else
		{
			m_Capacity *= 4;
		}
	}
}
