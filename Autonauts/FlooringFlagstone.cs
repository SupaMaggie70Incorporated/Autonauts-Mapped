public class FlooringFlagstone : Floor2D
{
	public override void RegisterClass()
	{
		base.RegisterClass();
		ModelManager.Instance.AddModel("Models/Buildings/Floors/FlooringFlagstoneWhole", ObjectType.FlooringFlagstone);
		ModelManager.Instance.AddModel("Models/Buildings/Floors/FlooringFlagstoneThreeQuarters", ObjectType.FlooringFlagstone);
		ModelManager.Instance.AddModel("Models/Buildings/Floors/FlooringFlagstoneHalf", ObjectType.FlooringFlagstone);
		ModelManager.Instance.AddModel("Models/Buildings/Floors/FlooringFlagstoneQuarter", ObjectType.FlooringFlagstone);
		ModelManager.Instance.AddModel("Models/Buildings/Floors/FlooringFlagstoneWhole2", ObjectType.FlooringFlagstone, RandomVariants: true);
		ModelManager.Instance.AddModel("Models/Buildings/Floors/FlooringFlagstoneWhole3", ObjectType.FlooringFlagstone, RandomVariants: true);
	}

	public override void Restart()
	{
		base.Restart();
		m_WholeModelName = "FlooringFlagstoneWhole";
		m_ThreeQuatersModelName = "FlooringFlagstoneThreeQuarters";
		m_HalfModelName = "FlooringFlagstoneHalf";
		m_QuaterModelName = "FlooringFlagstoneQuarter";
	}
}
