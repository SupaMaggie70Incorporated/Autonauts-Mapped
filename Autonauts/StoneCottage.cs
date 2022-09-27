public class StoneCottage : Housing
{
	public override void Restart()
	{
		base.Restart();
		SetDimensions(new TileCoord(0, -2), new TileCoord(1, 0), new TileCoord(0, 1));
	}
}
