public class FlooringParquet : Floor2D
{
	public override void RegisterClass()
	{
		base.RegisterClass();
		ModelManager.Instance.AddModel("Models/Buildings/Floors/FlooringParquetWhole", ObjectType.FlooringParquet);
		ModelManager.Instance.AddModel("Models/Buildings/Floors/FlooringParquetThreeQuarters", ObjectType.FlooringParquet);
		ModelManager.Instance.AddModel("Models/Buildings/Floors/FlooringParquetHalf", ObjectType.FlooringParquet);
		ModelManager.Instance.AddModel("Models/Buildings/Floors/FlooringParquetQuarter", ObjectType.FlooringParquet);
		ModelManager.Instance.AddModel("Models/Buildings/Floors/FlooringParquetWhole2", ObjectType.FlooringParquet, RandomVariants: true);
		ModelManager.Instance.AddModel("Models/Buildings/Floors/FlooringParquetWhole3", ObjectType.FlooringParquet, RandomVariants: true);
	}

	public override void Restart()
	{
		base.Restart();
		m_WholeModelName = "FlooringParquetWhole";
		m_ThreeQuatersModelName = "FlooringParquetThreeQuarters";
		m_HalfModelName = "FlooringParquetHalf";
		m_QuaterModelName = "FlooringParquetQuarter";
	}
}
