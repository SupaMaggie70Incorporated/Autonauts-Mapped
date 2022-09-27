public class RoadGood : Floor
{
	public override void RegisterClass()
	{
		base.RegisterClass();
		ModelManager.Instance.AddModel("Models/Buildings/Floors/RoadGood2", ObjectType.RoadGood, RandomVariants: true);
		ModelManager.Instance.AddModel("Models/Buildings/Floors/RoadGood3", ObjectType.RoadGood, RandomVariants: true);
	}

	public override void Restart()
	{
		base.Restart();
		SetDimensions(new TileCoord(0, 0), new TileCoord(0, 0), new TileCoord(0, 0));
	}
}
