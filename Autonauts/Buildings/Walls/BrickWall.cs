public class BrickWall : Wall
{
	public override void RegisterClass()
	{
		base.RegisterClass();
		ModelManager.Instance.AddModel("Models/Buildings/BrickWallCross", ObjectType.BrickWall);
		ModelManager.Instance.AddModel("Models/Buildings/BrickWallT", ObjectType.BrickWall);
		ModelManager.Instance.AddModel("Models/Buildings/BrickWallL", ObjectType.BrickWall);
		ModelManager.Instance.AddModel("Models/Buildings/BrickWall", ObjectType.BrickWall);
	}

	public override void Restart()
	{
		base.Restart();
		m_CrossModelName = "BrickWallCross";
		m_TModelName = "BrickWallT";
		m_LModelName = "BrickWallL";
		m_StraightModelName = "BrickWall";
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
		if (typeIdentifier != ObjectType.BrickWall && !Window.GetIsTypeWindow(typeIdentifier))
		{
			return false;
		}
		return true;
	}
}
