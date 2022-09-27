using UnityEngine;

public class BestStartPosition : MonoBehaviour
{
	private static int m_Padding = 3;

	private static bool IsTileEmpty(Tile.TileType[] TileData, int Width, int Height, TileCoord Position)
	{
		if (Position.x < m_Padding || Position.y < m_Padding || Position.x >= Width - m_Padding || Position.y >= Height - m_Padding)
		{
			return false;
		}
		if (Position.x % Plot.m_PlotTilesWide == 0 || Position.x % Plot.m_PlotTilesWide >= Plot.m_PlotTilesWide - 2 || Position.y % Plot.m_PlotTilesHigh == 0 || Position.y % Plot.m_PlotTilesHigh >= Plot.m_PlotTilesHigh - 3)
		{
			return false;
		}
		if (TileData[Position.y * Width + Position.x] == Tile.TileType.Empty)
		{
			return true;
		}
		return false;
	}

	private static bool IsAreaEmpty(Tile.TileType[] TileData, int Width, int Height, TileCoord Position, int Radius)
	{
		for (int i = Position.y - Radius; i <= Position.y + Radius; i++)
		{
			for (int j = Position.x - Radius; j <= Position.x + Radius; j++)
			{
				if (j >= m_Padding && i >= m_Padding && j < Width - m_Padding && i < Height - m_Padding && TileData[i * Width + j] != 0)
				{
					return false;
				}
			}
		}
		return true;
	}

	public static TileCoord FindNearestEmptyTile(Tile.TileType[] TileData, int Width, int Height, TileCoord OldPosition)
	{
		for (int i = 1; i < 200; i++)
		{
			for (int j = -i; j <= i; j++)
			{
				TileCoord tileCoord = new TileCoord(OldPosition.x + j, OldPosition.y - i);
				if (IsTileEmpty(TileData, Width, Height, tileCoord) && IsAreaEmpty(TileData, Width, Height, tileCoord, 3))
				{
					return tileCoord;
				}
				tileCoord = new TileCoord(OldPosition.x + j, OldPosition.y + i);
				if (IsTileEmpty(TileData, Width, Height, tileCoord) && IsAreaEmpty(TileData, Width, Height, tileCoord, 3))
				{
					return tileCoord;
				}
			}
			for (int k = -i + 1; k <= i - 1; k++)
			{
				TileCoord tileCoord2 = new TileCoord(OldPosition.x - i, OldPosition.y + k);
				if (IsTileEmpty(TileData, Width, Height, tileCoord2) && IsAreaEmpty(TileData, Width, Height, tileCoord2, 3))
				{
					return tileCoord2;
				}
				tileCoord2 = new TileCoord(OldPosition.x + i, OldPosition.y + k);
				if (IsTileEmpty(TileData, Width, Height, tileCoord2) && IsAreaEmpty(TileData, Width, Height, tileCoord2, 3))
				{
					return tileCoord2;
				}
			}
		}
		Debug.Log("Couldn't find an empty tile");
		return OldPosition;
	}
}
