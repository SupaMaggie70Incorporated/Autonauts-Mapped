public class Castle : Housing
{
	public override void Restart()
	{
		base.Restart();
		SetDimensions(new TileCoord(-1, -4), new TileCoord(2, 0), new TileCoord(0, 1));
	}
}
