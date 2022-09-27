public class LogWall : Wall
{
	public override void RegisterClass()
	{
		base.RegisterClass();
		ModelManager.Instance.AddModel("Models/Buildings/LogWallCross", ObjectType.LogWall);
		ModelManager.Instance.AddModel("Models/Buildings/LogWallT", ObjectType.LogWall);
		ModelManager.Instance.AddModel("Models/Buildings/LogWallL", ObjectType.LogWall);
		ModelManager.Instance.AddModel("Models/Buildings/LogWall", ObjectType.LogWall);
	}

	public override void Restart()
	{
		base.Restart();
		m_CrossModelName = "LogWallCross";
		m_TModelName = "LogWallT";
		m_LModelName = "LogWallL";
		m_StraightModelName = "LogWall";
	}

	public new void Awake()
	{
		base.Awake();
		m_MaxLevels = 3;
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
		if (typeIdentifier != ObjectType.LogWall && !Window.GetIsTypeWindow(typeIdentifier))
		{
			return false;
		}
		return true;
	}
}
