public class FencePost : Wall
{
	public override void RegisterClass()
	{
		base.RegisterClass();
		ModelManager.Instance.AddModel("Models/Buildings/FencePostCross", ObjectType.FencePost);
		ModelManager.Instance.AddModel("Models/Buildings/FencePostT", ObjectType.FencePost);
		ModelManager.Instance.AddModel("Models/Buildings/FencePostL", ObjectType.FencePost);
		ModelManager.Instance.AddModel("Models/Buildings/FencePost", ObjectType.FencePost);
	}

	public override void Restart()
	{
		base.Restart();
		m_CrossModelName = "FencePostCross";
		m_StraightModelName = "FencePost";
		m_TModelName = "FencePostT";
		m_LModelName = "FencePostL";
	}
}
