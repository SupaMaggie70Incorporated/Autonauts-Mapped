public class BlockWall : Wall
{
	public override void RegisterClass()
	{
		base.RegisterClass();
		ModelManager.Instance.AddModel("Models/Buildings/BlockWallCross", ObjectType.BlockWall);
		ModelManager.Instance.AddModel("Models/Buildings/BlockWallT", ObjectType.BlockWall);
		ModelManager.Instance.AddModel("Models/Buildings/BlockWallL", ObjectType.BlockWall);
		ModelManager.Instance.AddModel("Models/Buildings/BlockWall", ObjectType.BlockWall);
	}

	public override void Restart()
	{
		base.Restart();
		m_CrossModelName = "BlockWallCross";
		m_TModelName = "BlockWallT";
		m_LModelName = "BlockWallL";
		m_StraightModelName = "BlockWall";
	}

	public new void Awake()
	{
		base.Awake();
		m_MaxLevels = 5;
		m_NumLevels = 1;
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
		if (typeIdentifier != ObjectType.BlockWall && !Window.GetIsTypeWindow(typeIdentifier))
		{
			return false;
		}
		return true;
	}
}
