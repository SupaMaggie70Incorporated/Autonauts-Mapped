public class FencePicket : Wall
{
	public override void RegisterClass()
	{
		base.RegisterClass();
		ModelManager.Instance.AddModel("Models/Buildings/FencePicketCross", ObjectType.FencePicket);
		ModelManager.Instance.AddModel("Models/Buildings/FencePicketT", ObjectType.FencePicket);
		ModelManager.Instance.AddModel("Models/Buildings/FencePicketL", ObjectType.FencePicket);
		ModelManager.Instance.AddModel("Models/Buildings/FencePicket", ObjectType.FencePicket);
	}

	public override void Restart()
	{
		base.Restart();
		m_CrossModelName = "FencePicketCross";
		m_StraightModelName = "FencePicket";
		m_TModelName = "FencePicketT";
		m_LModelName = "FencePicketL";
	}
}
