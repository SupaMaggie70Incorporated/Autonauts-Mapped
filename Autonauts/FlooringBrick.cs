public class FlooringBrick : Floor2D
{
	public override void RegisterClass()
	{
		base.RegisterClass();
		ModelManager.Instance.AddModel("Models/Buildings/Floors/FlooringBrickWhole", ObjectType.FlooringBrick);
		ModelManager.Instance.AddModel("Models/Buildings/Floors/FlooringBrickThreeQuarters", ObjectType.FlooringBrick);
		ModelManager.Instance.AddModel("Models/Buildings/Floors/FlooringBrickHalf", ObjectType.FlooringBrick);
		ModelManager.Instance.AddModel("Models/Buildings/Floors/FlooringBrickQuarter", ObjectType.FlooringBrick);
		ModelManager.Instance.AddModel("Models/Buildings/Floors/FlooringBrickWhole2", ObjectType.FlooringBrick, RandomVariants: true);
		ModelManager.Instance.AddModel("Models/Buildings/Floors/FlooringBrickWhole3", ObjectType.FlooringBrick, RandomVariants: true);
	}

	public override void Restart()
	{
		base.Restart();
		m_WholeModelName = "FlooringBrickWhole";
		m_ThreeQuatersModelName = "FlooringBrickThreeQuarters";
		m_HalfModelName = "FlooringBrickHalf";
		m_QuaterModelName = "FlooringBrickQuarter";
	}
}
