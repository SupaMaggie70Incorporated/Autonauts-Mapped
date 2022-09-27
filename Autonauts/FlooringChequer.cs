public class FlooringChequer : Floor2D
{
	public override void RegisterClass()
	{
		base.RegisterClass();
		ModelManager.Instance.AddModel("Models/Buildings/Floors/FlooringChequerWhole", ObjectType.FlooringChequer);
		ModelManager.Instance.AddModel("Models/Buildings/Floors/FlooringChequerThreeQuarters", ObjectType.FlooringChequer);
		ModelManager.Instance.AddModel("Models/Buildings/Floors/FlooringChequerHalf", ObjectType.FlooringChequer);
		ModelManager.Instance.AddModel("Models/Buildings/Floors/FlooringChequerQuarter", ObjectType.FlooringChequer);
		ModelManager.Instance.AddModel("Models/Buildings/Floors/FlooringChequerWhole2", ObjectType.FlooringChequer, RandomVariants: true);
		ModelManager.Instance.AddModel("Models/Buildings/Floors/FlooringChequerWhole3", ObjectType.FlooringChequer, RandomVariants: true);
	}

	public override void Restart()
	{
		base.Restart();
		m_WholeModelName = "FlooringChequerWhole";
		m_ThreeQuatersModelName = "FlooringChequerThreeQuarters";
		m_HalfModelName = "FlooringChequerHalf";
		m_QuaterModelName = "FlooringChequerQuarter";
	}
}
