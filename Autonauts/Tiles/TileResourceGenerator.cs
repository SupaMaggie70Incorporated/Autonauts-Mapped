using System;
using System.Collections.Generic;
using UnityEngine;

public class TileResourceGenerator : MonoBehaviour
{
	private static Tile.TileType[] m_Map;

	private static int m_Width;

	private static int m_Height;

	private static Noise m_Noise;

	private static float m_Threshold;

	private static float m_CropThreshold;

	public static TileCoord m_NearbyStonesPosition;

	private static float GetResourceAllowedPercent(TileCoord Position, int Radius)
	{
		int x = Position.x;
		int y = Position.y;
		int num = 0;
		int num2 = 0;
		for (int i = y - Radius; i <= y + Radius; i++)
		{
			for (int j = x - Radius; j <= x + Radius; j++)
			{
				if (j > 0 && j < m_Width && i > 0 && i < m_Height && (new TileCoord(j, i) - Position).Magnitude() < (float)Radius)
				{
					Tile.TileType tileType = m_Map[i * m_Width + j];
					if (tileType != 0 && tileType != Tile.TileType.Trees && tileType != Tile.TileType.Grass)
					{
						num++;
					}
					num2++;
				}
			}
		}
		return (float)num / (float)num2;
	}

	private static void CreateResource(TileCoord Position, int Radius, Tile.TileType NewType)
	{
		int x = Position.x;
		int y = Position.y;
		for (int i = y - Radius; i <= y + Radius; i++)
		{
			for (int j = x - Radius; j <= x + Radius; j++)
			{
				if (j <= 0 || j >= m_Width || i <= 0 || i >= m_Height)
				{
					continue;
				}
				float num = (new TileCoord(j, i) - Position).Magnitude();
				if (!(num <= (float)Radius))
				{
					continue;
				}
				Tile.TileType tileType = m_Map[i * m_Width + j];
				if (tileType != 0 && tileType != Tile.TileType.Trees && tileType != Tile.TileType.Grass)
				{
					continue;
				}
				float num2 = 1f - num / (float)Radius;
				float noise = m_Noise.GetNoise(j, i);
				noise *= num2;
				if (Radius <= 4)
				{
					noise = 1000f;
				}
				if (noise > m_Threshold)
				{
					if ((NewType == Tile.TileType.CropWheat || NewType == Tile.TileType.CropCotton) && noise < m_CropThreshold)
					{
						m_Map[i * m_Width + j] = Tile.TileType.Weeds;
					}
					else
					{
						m_Map[i * m_Width + j] = NewType;
					}
				}
			}
		}
	}

	private static TileCoord GetPositionFromRange(TileCoord OldPosition, int MinRange, int MaxRange)
	{
		int num = UnityEngine.Random.Range(MinRange, MaxRange);
		float num2 = UnityEngine.Random.Range(0, 360);
		float num3 = Mathf.Cos(num2 * ((float)Math.PI / 180f)) * (float)num;
		float num4 = Mathf.Sin(num2 * ((float)Math.PI / 180f)) * (float)num;
		return OldPosition + new TileCoord((int)num3, (int)num4);
	}

	private static void AddResource(TileCoord OldPosition, int Radius, int Range, Tile.TileType NewType)
	{
		int num = 0;
		do
		{
			TileCoord positionFromRange = GetPositionFromRange(OldPosition, Range - 3, Range + 3);
			if (GetResourceAllowedPercent(positionFromRange, Radius) < 0.1f)
			{
				if (NewType == Tile.TileType.StoneHidden)
				{
					m_NearbyStonesPosition = positionFromRange;
				}
				CreateResource(positionFromRange, Radius, NewType);
				return;
			}
			num++;
		}
		while (num < 200);
		Debug.Log("*** Warning : Near resource " + NewType.ToString() + " couldn't be spawned");
	}

	private static TileCoord GetPositionMinDist(TileCoord OldPosition, int MinRange)
	{
		int num = 0;
		TileCoord tileCoord = default(TileCoord);
		do
		{
			int nx = UnityEngine.Random.Range(0, m_Width);
			int ny = UnityEngine.Random.Range(0, m_Height);
			tileCoord = new TileCoord(nx, ny);
			if ((new TileCoord(nx, ny) - OldPosition).Magnitude() > (float)MinRange)
			{
				return tileCoord;
			}
			num++;
		}
		while (num < 1000);
		return tileCoord;
	}

	private static void AddMinDistResource(TileCoord OldPosition, int Radius, int MinRange, Tile.TileType NewType)
	{
		int num = 0;
		do
		{
			TileCoord positionMinDist = GetPositionMinDist(OldPosition, MinRange);
			if (GetResourceAllowedPercent(positionMinDist, Radius) < 0.2f)
			{
				CreateResource(positionMinDist, Radius, NewType);
				return;
			}
			num++;
		}
		while (num < 200);
		Debug.Log("*** Warning : Far resource " + NewType.ToString() + " couldn't be spawned");
	}

	private static void AddSwamp(List<TileCoord> Tiles, int Radius)
	{
		if (Tiles.Count > 0)
		{
			int num = 0;
			do
			{
				int index = UnityEngine.Random.Range(0, Tiles.Count);
				TileCoord position = Tiles[index];
				if (GetResourceAllowedPercent(position, Radius) < 0.6f)
				{
					CreateResource(position, Radius, Tile.TileType.Swamp);
					return;
				}
				num++;
			}
			while (num < 200);
		}
		Debug.Log("*** Warning : Swamp couldn't be spawned");
	}

	private static void AddSwamps()
	{
		List<TileCoord> list = new List<TileCoord>();
		for (int i = 0; i < m_Height; i++)
		{
			for (int j = 0; j < m_Width; j++)
			{
				if (m_Map[i * m_Width + j] == Tile.TileType.WaterShallow)
				{
					list.Add(new TileCoord(j, i));
				}
			}
		}
		for (int k = 0; k < 4; k++)
		{
			AddSwamp(list, 10);
		}
	}

	public static void AddResources(int Seed, Tile.TileType[] Map, int Width, int Height, TileCoord PlayerPosition)
	{
		if (!SaveLoadManager.m_MiniMap)
		{
			m_Map = Map;
			m_Width = Width;
			m_Height = Height;
			m_Noise = new Noise(Seed, 2, 1f / 32f, 0.2f);
			m_Threshold = 0.2f;
			AddResource(PlayerPosition, 12, 15, Tile.TileType.Trees);
			m_NearbyStonesPosition = default(TileCoord);
			AddResource(PlayerPosition, 2, 20, Tile.TileType.StoneSoil);
			AddResource(PlayerPosition, 2, 30, Tile.TileType.ClaySoil);
			m_CropThreshold = 0.175f;
			m_Threshold = 0.125f;
			AddResource(PlayerPosition, 2, 35, Tile.TileType.CropWheat);
			AddResource(PlayerPosition, 2, 35, Tile.TileType.CropCotton);
			m_Threshold = 0.2f;
			m_CropThreshold = 0.3f;
			AddResource(PlayerPosition, 10, 35, Tile.TileType.WaterShallow);
			for (int i = 0; i < 4; i++)
			{
				AddMinDistResource(PlayerPosition, 6, 20, Tile.TileType.StoneSoil);
			}
			for (int j = 0; j < 4; j++)
			{
				AddMinDistResource(PlayerPosition, 6, 30, Tile.TileType.ClaySoil);
			}
			for (int k = 0; k < 4; k++)
			{
				AddMinDistResource(PlayerPosition, 10, 35, Tile.TileType.CropWheat);
			}
			for (int l = 0; l < 4; l++)
			{
				AddMinDistResource(PlayerPosition, 10, 35, Tile.TileType.CropCotton);
			}
			for (int m = 0; m < 4; m++)
			{
				AddMinDistResource(PlayerPosition, 6, 60, Tile.TileType.IronSoil);
			}
			for (int n = 0; n < 4; n++)
			{
				AddMinDistResource(PlayerPosition, 6, 100, Tile.TileType.CoalSoil);
			}
			AddSwamps();
		}
	}
}
