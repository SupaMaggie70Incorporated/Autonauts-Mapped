public class FlooringCrude : Floor2D
{
	public override void RegisterClass()
	{
		base.RegisterClass();
		ModelManager.Instance.AddModel("Models/Buildings/Floors/FlooringCrudeWhole", ObjectType.FlooringCrude);
		ModelManager.Instance.AddModel("Models/Buildings/Floors/FlooringCrudeThreeQuarters", ObjectType.FlooringCrude);
		ModelManager.Instance.AddModel("Models/Buildings/Floors/FlooringCrudeHalf", ObjectType.FlooringCrude);
		ModelManager.Instance.AddModel("Models/Buildings/Floors/FlooringCrudeQuarter", ObjectType.FlooringCrude);
		ModelManager.Instance.AddModel("Models/Buildings/Floors/FlooringCrudeWhole2", ObjectType.FlooringCrude, RandomVariants: true);
		ModelManager.Instance.AddModel("Models/Buildings/Floors/FlooringCrudeWhole3", ObjectType.FlooringCrude, RandomVariants: true);
	}

	public override void Restart()
	{
		base.Restart();
		m_WholeModelName = "FlooringCrudeWhole";
		m_ThreeQuatersModelName = "FlooringCrudeThreeQuarters";
		m_HalfModelName = "FlooringCrudeHalf";
		m_QuaterModelName = "FlooringCrudeQuarter";
	}
}
