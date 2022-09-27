public class Window : Building
{
	public static bool GetIsTypeWindow(ObjectType NewType)
	{
		if (NewType == ObjectType.Window || NewType == ObjectType.WindowStone)
		{
			return true;
		}
		return false;
	}

	public override void Restart()
	{
		base.Restart();
		SetDimensions(new TileCoord(0, 0), new TileCoord(0, 0), new TileCoord(0, 1));
		HideAccessModel();
	}

	public new void Awake()
	{
		base.Awake();
		m_MaxLevels = 5;
		m_NumLevels = 2;
	}

	public override bool GetNewLevelAllowed(Building NewBuilding)
	{
		if (m_TotalLevels + NewBuilding.m_TotalLevels > m_MaxLevels)
		{
			return false;
		}
		ObjectType typeIdentifier = NewBuilding.m_TypeIdentifier;
		if (typeIdentifier == ObjectType.ConverterFoundation)
		{
			typeIdentifier = NewBuilding.GetComponent<ConverterFoundation>().m_NewBuilding.m_TypeIdentifier;
		}
		if (typeIdentifier != ObjectType.BrickWall && typeIdentifier != ObjectType.LogWall && typeIdentifier != ObjectType.BlockWall && !GetIsTypeWindow(typeIdentifier))
		{
			return false;
		}
		return true;
	}
}
