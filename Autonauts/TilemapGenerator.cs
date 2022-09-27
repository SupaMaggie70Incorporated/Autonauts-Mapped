using UnityEngine;

public class TilemapGenerator : MonoBehaviour
{
	public static TilemapGenerator Instance;

	public bool m_Generating;

	private int m_GenerateLine;

	private int m_Width;

	private int m_Height;

	private int m_Seed;

	public Tile.TileType[] m_Map;

	private Noise m_SeaNoise;

	private Noise m_GrassNoise;

	private Noise m_TreeNoise;

	private Noise m_WaterNoise;

	private Noise m_BoulderNoise;

	private Noise m_SeaBoulderNoise;

	private void Awake()
	{
		Instance = this;
	}

	public void StartNewMap(int Seed, int Width, int Height)
	{
		m_Seed = Seed;
		m_Width = Width;
		m_Height = Height;
		m_Map = new Tile.TileType[m_Width * m_Height];
		float num = 240f;
		m_SeaNoise = new Noise(m_Seed, 4, 1f / num, 0.4f);
		m_TreeNoise = new Noise(m_Seed + 1, 2, 4f / num, 0.3f);
		m_GrassNoise = new Noise(m_Seed + 2, 2, 5f / num, 0.2f);
		m_WaterNoise = new Noise(m_Seed + 3, 2, 2f / num, 0.15f);
		m_BoulderNoise = new Noise(m_Seed + 4, 2, 5f / num, 0.2f);
		m_SeaBoulderNoise = new Noise(m_Seed + 5, 4, 7.5f / num, 0.21f);
		m_GenerateLine = 0;
		m_Generating = true;
	}

	private Tile.TileType GetTile(int x, int y)
	{
		float noise = m_SeaNoise.GetNoise(x, y);
		if (noise < m_SeaNoise.m_Threshold)
		{
			noise = m_SeaBoulderNoise.GetNoise(x, y);
			if (noise < m_SeaBoulderNoise.m_Threshold)
			{
				return Tile.TileType.Raised;
			}
			return Tile.TileType.SeaWaterDeep;
		}
		if (noise < m_SeaNoise.m_Threshold + 0.025f)
		{
			return Tile.TileType.SeaWaterShallow;
		}
		if (noise < m_SeaNoise.m_Threshold + 0.05f)
		{
			return Tile.TileType.Sand;
		}
		float num = m_WaterNoise.GetNoise(x, y) * 2f * (1f - noise);
		if (num < m_WaterNoise.m_Threshold)
		{
			return Tile.TileType.WaterDeep;
		}
		if (num < m_WaterNoise.m_Threshold + 0.015f)
		{
			return Tile.TileType.WaterShallow;
		}
		noise = m_BoulderNoise.GetNoise(x, y);
		if (noise < m_BoulderNoise.m_Threshold)
		{
			return Tile.TileType.Raised;
		}
		if (noise < m_BoulderNoise.m_Threshold + 0.015f)
		{
			return Tile.TileType.Grass;
		}
		noise = m_TreeNoise.GetNoise(x, y);
		if (noise < m_TreeNoise.m_Threshold)
		{
			return Tile.TileType.Trees;
		}
		noise = m_GrassNoise.GetNoise(x, y);
		if (noise < m_GrassNoise.m_Threshold)
		{
			return Tile.TileType.Grass;
		}
		return Tile.TileType.Empty;
	}

	private void CreateMapLine(int Line)
	{
		int num = 0;
		int num2 = 0;
		if (SaveLoadManager.m_MiniMap)
		{
			num = (int)(SaveLoadManager.m_MiniMapCameraX / Tile.m_Size) + 2 - m_Width;
			num2 = (int)((0f - SaveLoadManager.m_MiniMapCameraZ) / Tile.m_Size) + 2 - m_Height;
		}
		for (int i = 0; i < m_Width; i++)
		{
			m_Map[Line * m_Width + i] = GetTile(i + num, Line + num2);
		}
	}

	private void Update()
	{
		if (!m_Generating)
		{
			return;
		}
		for (int i = 0; i < 10; i++)
		{
			CreateMapLine(m_GenerateLine);
			m_GenerateLine++;
			if (m_GenerateLine == m_Height)
			{
				m_Generating = false;
				break;
			}
		}
	}
}
