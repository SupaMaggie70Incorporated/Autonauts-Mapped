public class StoneWall : Wall
{
	public override void RegisterClass()
	{
		base.RegisterClass();
		ModelManager.Instance.AddModel("Models/Buildings/Wall_Dry_Stone_Cross", ObjectType.StoneWall);
		ModelManager.Instance.AddModel("Models/Buildings/Wall_Dry_StoneT", ObjectType.StoneWall);
		ModelManager.Instance.AddModel("Models/Buildings/Wall_Dry_StoneL", ObjectType.StoneWall);
		ModelManager.Instance.AddModel("Models/Buildings/Wall_Dry_Stone", ObjectType.StoneWall);
		ModelManager.Instance.AddModel("Models/Buildings/Wall_Dry_Stone2", ObjectType.StoneWall, RandomVariants: true);
		ModelManager.Instance.AddModel("Models/Buildings/Wall_Dry_Stone3", ObjectType.StoneWall, RandomVariants: true);
	}

	public override void Restart()
	{
		base.Restart();
		m_CrossModelName = "Wall_Dry_Stone_Cross";
		m_StraightModelName = "Wall_Dry_Stone";
		m_TModelName = "Wall_Dry_StoneT";
		m_LModelName = "Wall_Dry_StoneL";
	}
}
