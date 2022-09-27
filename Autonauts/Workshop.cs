public class Workshop : Floor2D
{
	public override void RegisterClass()
	{
		base.RegisterClass();
		ModelManager.Instance.AddModel("Models/Buildings/Floors/WorkshopFloorWhole", ObjectType.Workshop);
		ModelManager.Instance.AddModel("Models/Buildings/Floors/WorkshopFloorThreeQuarters", ObjectType.Workshop);
		ModelManager.Instance.AddModel("Models/Buildings/Floors/WorkshopFloorHalf", ObjectType.Workshop);
		ModelManager.Instance.AddModel("Models/Buildings/Floors/WorkshopFloorQuarter", ObjectType.Workshop);
		ModelManager.Instance.AddModel("Models/Buildings/Floors/WorkshopFloorWhole2", ObjectType.Workshop, RandomVariants: true);
		ModelManager.Instance.AddModel("Models/Buildings/Floors/WorkshopFloorWhole3", ObjectType.Workshop, RandomVariants: true);
	}

	public override void Restart()
	{
		base.Restart();
		m_WholeModelName = "WorkshopFloorWhole";
		m_ThreeQuatersModelName = "WorkshopFloorThreeQuarters";
		m_HalfModelName = "WorkshopFloorHalf";
		m_QuaterModelName = "WorkshopFloorQuarter";
	}
}
