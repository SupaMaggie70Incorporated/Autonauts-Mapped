public class CastleWall : Wall
{
	public override void RegisterClass()
	{
		base.RegisterClass();
		ModelManager.Instance.AddModel("Models/Buildings/CastleWallCross", ObjectType.CastleWall);
		ModelManager.Instance.AddModel("Models/Buildings/CastleWallT", ObjectType.CastleWall);
		ModelManager.Instance.AddModel("Models/Buildings/CastleWallL", ObjectType.CastleWall);
		ModelManager.Instance.AddModel("Models/Buildings/CastleWall", ObjectType.CastleWall);
	}

	public override void Restart()
	{
		base.Restart();
		m_CrossModelName = "CastleWallCross";
		m_StraightModelName = "CastleWall";
		m_TModelName = "CastleWallT";
		m_LModelName = "CastleWallL";
	}
}
