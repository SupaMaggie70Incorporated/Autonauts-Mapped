public class AquariumGood : Aquarium
{
	public override void Restart()
	{
		base.Restart();
		SetDimensions(new TileCoord(-2, -2), new TileCoord(3, 0), new TileCoord(0, 1));
	}
}
