using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;

public class TileManager : MonoBehaviour
{
	public static TileManager Instance;

	private static bool m_RLESave = true;

	[HideInInspector]
	public int m_TilesWide;

	[HideInInspector]
	public int m_TilesHigh;

	[HideInInspector]
	public Tile[] m_Tiles;

	private int[] m_TileTextures;

	[HideInInspector]
	public Texture2D m_TileMapTexture;

	private bool m_TileMapChanges;

	[HideInInspector]
	public Texture2D m_TileMapTexture2;

	private List<TileCoord> m_ChangedTiles;

	private List<int> m_ActiveSoilTileIndexes;

	private List<float> m_ActiveSoilTileTimes;

	public List<int> m_SoilTileIndexes;

	public Dictionary<int, int> m_SoilTileIndexesDictionary;

	public List<int> m_EmptyTileIndexes;

	public Dictionary<int, int> m_EmptyTileIndexesDictionary;

	private int[] m_TilesChecked;

	public int m_SearchIndex;

	private int m_MaxSearchSize;

	public bool m_SearchDone;

	private void Awake()
	{
		m_TileMapChanges = false;
		m_ChangedTiles = new List<TileCoord>();
		m_SoilTileIndexes = new List<int>();
		m_SoilTileIndexesDictionary = new Dictionary<int, int>();
		m_EmptyTileIndexes = new List<int>();
		m_EmptyTileIndexesDictionary = new Dictionary<int, int>();
		m_ActiveSoilTileIndexes = new List<int>();
		m_ActiveSoilTileTimes = new List<float>();
		m_TilesChecked = new int[10000];
		m_SearchIndex = 0;
	}

	private void OnDestroy()
	{
		RouteFinding.ShutDown();
	}

	private JSONNode SaveWeededTile(TileCoord Coord)
	{
		int num = Coord.y * m_TilesWide + Coord.x;
		Tile tile = m_Tiles[num];
		JSONObject jSONObject = new JSONObject();
		JSONUtils.Set(jSONObject, "x", Coord.x);
		JSONUtils.Set(jSONObject, "y", Coord.y);
		JSONUtils.Set(jSONObject, "T", tile.m_Timer * 10f);
		return jSONObject;
	}

	private void UncompressedSave(JSONNode Node)
	{
		JSONArray jSONArray = new JSONArray();
		int tilesWide = m_TilesWide;
		int tilesHigh = m_TilesHigh;
		JSONUtils.Set(Node, "TilesWide", tilesWide);
		JSONUtils.Set(Node, "TilesHigh", tilesHigh);
		Node["TileTypes"] = jSONArray;
		Tile[] tiles = m_Tiles;
		for (int i = 0; i < tilesHigh; i++)
		{
			for (int j = 0; j < tilesWide; j++)
			{
				int num = i * tilesWide + j;
				jSONArray[num] = (int)tiles[num].m_TileType;
			}
		}
	}

	private void RLESave(JSONNode Node)
	{
		JSONArray jSONArray = new JSONArray();
		int tilesWide = m_TilesWide;
		int tilesHigh = m_TilesHigh;
		JSONUtils.Set(Node, "RLE", 1);
		JSONUtils.Set(Node, "TilesWide", tilesWide);
		JSONUtils.Set(Node, "TilesHigh", tilesHigh);
		Node["TileTypes"] = jSONArray;
		Tile[] tiles = m_Tiles;
		int num = 0;
		Tile.TileType tileType = tiles[0].m_TileType;
		int num2 = 0;
		for (int i = 0; i < tilesHigh * tilesWide; i++)
		{
			Tile.TileType tileType2 = tiles[i].m_TileType;
			if (tileType2 != tileType)
			{
				jSONArray[num2++] = (int)tileType;
				jSONArray[num2++] = num;
				tileType = tileType2;
				num = 0;
			}
			num++;
		}
		jSONArray[num2++] = (int)tileType;
		jSONArray[num2++] = num;
	}

	public void Save(JSONNode Node)
	{
		if (m_RLESave)
		{
			RLESave(Node);
		}
		else
		{
			UncompressedSave(Node);
		}
		JSONArray jSONArray = (JSONArray)(Node["Soil"] = new JSONArray());
		int num = 0;
		foreach (int activeSoilTileIndex in m_ActiveSoilTileIndexes)
		{
			JSONNode jSONNode2 = new JSONObject();
			JSONUtils.Set(jSONNode2, "I", activeSoilTileIndex);
			JSONUtils.Set(jSONNode2, "T", m_ActiveSoilTileTimes[num] * 10f);
			jSONArray[jSONArray.Count] = jSONNode2;
			num++;
		}
		TileUseManager.Instance.Save(Node);
	}

	private void UncompressedLoad(JSONNode Node)
	{
		int asInt = JSONUtils.GetAsInt(Node, "TilesWide", 0);
		int asInt2 = JSONUtils.GetAsInt(Node, "TilesHigh", 0);
		JSONArray asArray = Node["TileTypes"].AsArray;
		Tile[] array = new Tile[asInt * asInt2];
		for (int i = 0; i < asInt2; i++)
		{
			for (int j = 0; j < asInt; j++)
			{
				int num = i * asInt + j;
				array[num] = new Tile();
				array[num].m_TileType = (Tile.TileType)asArray[num].AsInt;
			}
		}
		MapManager.Instance.Load(array, asInt, asInt2);
		TileUseManager.Instance.Load(Node);
	}

	private void RLELoad(JSONNode Node)
	{
		int asInt = JSONUtils.GetAsInt(Node, "TilesWide", 0);
		int asInt2 = JSONUtils.GetAsInt(Node, "TilesHigh", 0);
		JSONArray asArray = Node["TileTypes"].AsArray;
		Tile[] array = new Tile[asInt * asInt2];
		int num = 0;
		for (int i = 0; i < asArray.Count; i += 2)
		{
			Tile.TileType asInt3 = (Tile.TileType)asArray[i].AsInt;
			int asInt4 = asArray[i + 1].AsInt;
			for (int j = 0; j < asInt4; j++)
			{
				array[num] = new Tile();
				array[num].m_TileType = asInt3;
				num++;
			}
		}
		MapManager.Instance.Load(array, asInt, asInt2);
		TileUseManager.Instance.Load(Node);
	}

	public void Load(JSONNode Node)
	{
		if (JSONUtils.GetAsBool(Node, "RLE", DefaultValue: false))
		{
			RLELoad(Node);
		}
		else
		{
			UncompressedLoad(Node);
		}
		JSONArray asArray = Node["Soil"].AsArray;
		for (int i = 0; i < asArray.Count; i++)
		{
			JSONNode node = asArray[i];
			int asInt = JSONUtils.GetAsInt(node, "I", 0);
			float item = JSONUtils.GetAsFloat(node, "T", 0f) / 10f;
			m_ActiveSoilTileIndexes.Add(asInt);
			m_ActiveSoilTileTimes.Add(item);
		}
	}

	private bool CheckAdjacentTilesMatching(int TempTextureNum, TileCoord Position)
	{
		if (Position.y > 0 && m_TileTextures[(Position.y - 1) * m_TilesWide + Position.x] == TempTextureNum)
		{
			return true;
		}
		if (Position.y < m_TilesHigh - 1 && m_TileTextures[(Position.y + 1) * m_TilesWide + Position.x] == TempTextureNum)
		{
			return true;
		}
		if (Position.x > 0 && m_TileTextures[Position.y * m_TilesWide + Position.x - 1] == TempTextureNum)
		{
			return true;
		}
		if (Position.x < m_TilesWide - 1 && m_TileTextures[Position.y * m_TilesWide + Position.x + 1] == TempTextureNum)
		{
			return true;
		}
		return false;
	}

	public int GetTileTexture(Tile NewTile, TileCoord Position)
	{
		int num = Tile.m_TileInfo[(int)NewTile.m_TileType].m_TextureNum;
		int variantsIndex = Tile.m_TileInfo[(int)NewTile.m_TileType].m_VariantsIndex;
		if (variantsIndex != 0)
		{
			int num2 = Random.Range(0, 100);
			if (num2 >= 80)
			{
				int num3 = variantsIndex;
				num3 = ((num2 < 88) ? num3 : ((num2 < 96) ? (num3 + 1) : ((num2 >= 98) ? (num3 + 3) : (num3 + 2))));
				if (!CheckAdjacentTilesMatching(num3, Position))
				{
					num = num3;
				}
			}
		}
		if (GetTileBuilding(NewTile) && !TileHelpers.GetTileWater(NewTile.m_TileType))
		{
			num = Tile.m_TileInfo[33].m_TextureNum;
		}
		m_TileTextures[Position.y * m_TilesWide + Position.x] = num;
		return num;
	}

	public int GetTileTexture(TileCoord Position)
	{
		return m_TileTextures[Position.y * m_TilesWide + Position.x];
	}

	public void CreateTiles(Tile[] Tiles, int Wide, int High)
	{
		m_TilesWide = Wide;
		m_TilesHigh = High;
		RouteFinding.InitTiles();
		m_Tiles = new Tile[Wide * High];
		m_TileTextures = new int[Wide * High];
		int num = 1024;
		m_TileMapTexture = new Texture2D(num, num, TextureFormat.RGBA32, mipChain: false, linear: true);
		if (m_TileMapTexture == null)
		{
			m_TileMapTexture = new Texture2D(num, num, TextureFormat.ARGB32, mipChain: false, linear: true);
		}
		m_TileMapTexture.filterMode = FilterMode.Point;
		m_TileMapTexture2 = new Texture2D(num, num, TextureFormat.RGBA32, mipChain: false, linear: true);
		if (m_TileMapTexture2 == null)
		{
			m_TileMapTexture2 = new Texture2D(num, num, TextureFormat.ARGB32, mipChain: false, linear: true);
		}
		m_TileMapTexture2.filterMode = FilterMode.Point;
		m_TileMapChanges = true;
		MaterialManager.Instance.GetMisc(MaterialManager.MiscType.TileMapMaterial, Dark: false).SetTexture("_TilemapTex", m_TileMapTexture);
		MaterialManager.Instance.GetMisc(MaterialManager.MiscType.TileMapMaterial, Dark: false).SetTexture("_TilemapTex2", m_TileMapTexture2);
		MaterialManager.Instance.GetMisc(MaterialManager.MiscType.TileMapMaterial, Dark: true).SetTexture("_TilemapTex", m_TileMapTexture);
		MaterialManager.Instance.GetMisc(MaterialManager.MiscType.TileMapMaterial, Dark: true).SetTexture("_TilemapTex2", m_TileMapTexture2);
		MaterialManager.Instance.GetMisc(MaterialManager.MiscType.TileMapWaterMaterial, Dark: false).SetTexture("_TilemapTex", m_TileMapTexture);
		MaterialManager.Instance.GetMisc(MaterialManager.MiscType.TileMapWaterMaterial, Dark: false).SetTexture("_TilemapTex2", m_TileMapTexture2);
		MaterialManager.Instance.GetMisc(MaterialManager.MiscType.TileMapWaterMaterial, Dark: true).SetTexture("_TilemapTex", m_TileMapTexture);
		MaterialManager.Instance.GetMisc(MaterialManager.MiscType.TileMapWaterMaterial, Dark: true).SetTexture("_TilemapTex2", m_TileMapTexture2);
		m_SoilTileIndexes.Clear();
		m_SoilTileIndexesDictionary.Clear();
		m_EmptyTileIndexes.Clear();
		m_EmptyTileIndexesDictionary.Clear();
		for (int i = 0; i < High; i++)
		{
			for (int j = 0; j < Wide; j++)
			{
				int num2 = i * m_TilesWide + j;
				m_Tiles[num2] = new Tile();
				m_Tiles[num2].m_Building = null;
				m_Tiles[num2].m_Timer = 0f;
				m_Tiles[num2].m_TileType = Tile.TileType.Total;
				SetTileType(new TileCoord(j, i), Tiles[num2].m_TileType);
			}
		}
	}

	public void UpdateShading()
	{
		for (int i = 0; i < m_TilesHigh; i++)
		{
			for (int j = 0; j < m_TilesWide; j++)
			{
				UpdateTileShading(j, i);
			}
		}
		m_TileMapTexture2.Apply();
	}

	private bool TestTileType(Tile.TileType NewType, Tile.TileType TestType, Tile.TileType AvoidType, Tile.TileType AvoidType2, int i, int j, int Index, out bool Up, out bool Down, out bool Left, out bool Right, out Tile.TileType OutType, out int BlendNum)
	{
		Up = true;
		Down = true;
		Left = true;
		Right = true;
		OutType = NewType;
		BlendNum = 0;
		if (NewType != TestType)
		{
			if (j != 0 && m_Tiles[Index - m_TilesWide].m_TileType == TestType)
			{
				Up = false;
			}
			if (j != m_TilesHigh - 1 && m_Tiles[Index + m_TilesWide].m_TileType == TestType)
			{
				Down = false;
			}
			if (i != 0 && m_Tiles[Index - 1].m_TileType == TestType)
			{
				Left = false;
			}
			if (i != m_TilesWide - 1 && m_Tiles[Index + 1].m_TileType == TestType)
			{
				Right = false;
			}
			if (!Up)
			{
				BlendNum++;
			}
			if (!Down)
			{
				BlendNum += 2;
			}
			if (!Left)
			{
				BlendNum += 4;
			}
			if (!Right)
			{
				BlendNum += 8;
			}
			Up = true;
			Down = true;
			Left = true;
			Right = true;
			if (BlendNum != 0)
			{
				OutType = TestType;
				return true;
			}
			return false;
		}
		if (j != 0)
		{
			Tile.TileType tileType = m_Tiles[Index - m_TilesWide].m_TileType;
			if (tileType != TestType && tileType != AvoidType && tileType != AvoidType2)
			{
				OutType = tileType;
				Up = false;
			}
		}
		if (j != m_TilesHigh - 1)
		{
			Tile.TileType tileType2 = m_Tiles[Index + m_TilesWide].m_TileType;
			if (tileType2 != TestType && tileType2 != AvoidType && tileType2 != AvoidType2)
			{
				OutType = tileType2;
				Down = false;
			}
		}
		if (i != 0)
		{
			Tile.TileType tileType3 = m_Tiles[Index - 1].m_TileType;
			if (tileType3 != TestType && tileType3 != AvoidType && tileType3 != AvoidType2)
			{
				OutType = tileType3;
				Left = false;
			}
		}
		if (i != m_TilesWide - 1)
		{
			Tile.TileType tileType4 = m_Tiles[Index + 1].m_TileType;
			if (tileType4 != TestType && tileType4 != AvoidType && tileType4 != AvoidType2)
			{
				OutType = tileType4;
				Right = false;
			}
		}
		if (Up | Down | Left | Right)
		{
			return true;
		}
		if (!Up && !Down && !Left && !Right)
		{
			Up = true;
			Down = true;
			Left = true;
			Right = true;
		}
		Tile.TileType tileType5 = NewType;
		Tile.TileType tileType6 = NewType;
		Tile.TileType tileType7 = NewType;
		Tile.TileType tileType8 = NewType;
		if (j != 0 && i != 0)
		{
			tileType5 = m_Tiles[Index - m_TilesWide - 1].m_TileType;
		}
		if (j != 0 && i != m_TilesWide - 1)
		{
			tileType6 = m_Tiles[Index - m_TilesWide + 1].m_TileType;
		}
		if (j != m_TilesHigh - 1 && i != 0)
		{
			tileType7 = m_Tiles[Index + m_TilesWide - 1].m_TileType;
		}
		if (j != m_TilesHigh - 1 && i != m_TilesWide - 1)
		{
			tileType8 = m_Tiles[Index + m_TilesWide + 1].m_TileType;
		}
		if (tileType5 != NewType && tileType5 != AvoidType && tileType5 != AvoidType2)
		{
			OutType = tileType5;
			BlendNum++;
		}
		if (tileType6 != NewType && tileType6 != AvoidType && tileType6 != AvoidType2)
		{
			OutType = tileType6;
			BlendNum += 2;
		}
		if (tileType7 != NewType && tileType7 != AvoidType && tileType7 != AvoidType2)
		{
			OutType = tileType7;
			BlendNum += 4;
		}
		if (tileType8 != NewType && tileType8 != AvoidType && tileType8 != AvoidType2)
		{
			OutType = tileType8;
			BlendNum += 8;
		}
		if (BlendNum != 0)
		{
			return true;
		}
		return false;
	}

	private bool GetTileBuilding(Tile NewTile)
	{
		if ((bool)NewTile.m_BuildingFootprint && NewTile.m_BuildingFootprint.m_TypeIdentifier != ObjectType.ConverterFoundation)
		{
			return true;
		}
		return false;
	}

	private bool TestBuilding(Tile.TileType NewType, int i, int j, int Index, out bool Up, out bool Down, out bool Left, out bool Right, out Tile.TileType OutType, out int BlendNum)
	{
		Up = true;
		Down = true;
		Left = true;
		Right = true;
		OutType = NewType;
		BlendNum = 0;
		if (TileHelpers.GetTileWater(NewType))
		{
			return false;
		}
		bool tileBuilding = GetTileBuilding(m_Tiles[Index]);
		if (j != 0 && GetTileBuilding(m_Tiles[Index - m_TilesWide]) != tileBuilding)
		{
			Up = false;
		}
		if (j != m_TilesHigh - 1 && GetTileBuilding(m_Tiles[Index + m_TilesWide]) != tileBuilding)
		{
			Down = false;
		}
		if (i != 0 && GetTileBuilding(m_Tiles[Index - 1]) != tileBuilding)
		{
			Left = false;
		}
		if (i != m_TilesWide - 1 && GetTileBuilding(m_Tiles[Index + 1]) != tileBuilding)
		{
			Right = false;
		}
		if (!Up || !Down || !Left || !Right)
		{
			if (!tileBuilding)
			{
				OutType = Tile.TileType.Building;
			}
			return true;
		}
		return false;
	}

	private void UpdateTileShading(int i, int j)
	{
		int num = 16;
		int num2 = j * m_TilesWide + i;
		Tile.TileType tileType = m_Tiles[num2].m_TileType;
		int num3 = 0;
		int BlendNum = 0;
		int num4 = 0;
		bool Up = true;
		bool Down = true;
		bool Left = true;
		bool Right = true;
		Tile.TileType OutType;
		if (tileType == Tile.TileType.SeaWaterShallow || tileType == Tile.TileType.SeaWaterDeep)
		{
			BlendNum = 48;
			Tile.TileType tileType2 = Tile.TileType.Sand;
			if (tileType == Tile.TileType.SeaWaterDeep)
			{
				tileType2 = Tile.TileType.SeaWaterShallow;
			}
			num3 = Tile.m_TileInfo[(int)tileType2].m_TextureNum;
			Tile.TileType tileType3 = tileType;
			Tile.TileType tileType4 = tileType;
			Tile.TileType tileType5 = tileType;
			Tile.TileType tileType6 = tileType;
			if (j != 0)
			{
				tileType3 = m_Tiles[num2 - m_TilesWide].m_TileType;
			}
			if (j != m_TilesHigh - 1)
			{
				tileType4 = m_Tiles[num2 + m_TilesWide].m_TileType;
			}
			if (i != 0)
			{
				tileType5 = m_Tiles[num2 - 1].m_TileType;
			}
			if (i != m_TilesWide - 1)
			{
				tileType6 = m_Tiles[num2 + 1].m_TileType;
			}
			if (tileType3 != tileType && tileType3 == tileType2)
			{
				Up = false;
			}
			if (tileType4 != tileType && tileType4 == tileType2)
			{
				Down = false;
			}
			if (tileType5 != tileType && tileType5 == tileType2)
			{
				Left = false;
			}
			if (tileType6 != tileType && tileType6 == tileType2)
			{
				Right = false;
			}
			if (!(Up && Down && Left && Right) && tileType2 == Tile.TileType.Sand)
			{
				ShorelineManager.Instance.CheckAddShorelineTile(i, j, Up, Down, Left, Right);
			}
		}
		else if (TestBuilding(tileType, i, j, num2, out Up, out Down, out Left, out Right, out OutType, out BlendNum))
		{
			BlendNum += 32;
			num3 = Tile.m_TileInfo[(int)OutType].m_TextureNum;
		}
		else if (TestTileType(tileType, Tile.TileType.Sand, Tile.TileType.SeaWaterShallow, Tile.TileType.SeaWaterShallow, i, j, num2, out Up, out Down, out Left, out Right, out OutType, out BlendNum))
		{
			BlendNum += 32;
			num3 = Tile.m_TileInfo[(int)OutType].m_TextureNum;
		}
		else if (TestTileType(tileType, Tile.TileType.Soil, Tile.TileType.SoilHole, Tile.TileType.SoilHole, i, j, num2, out Up, out Down, out Left, out Right, out OutType, out BlendNum))
		{
			BlendNum += 32;
			num3 = Tile.m_TileInfo[(int)OutType].m_TextureNum;
		}
		else if (TestTileType(tileType, Tile.TileType.SoilUsed, Tile.TileType.SoilUsed, Tile.TileType.SoilUsed, i, j, num2, out Up, out Down, out Left, out Right, out OutType, out BlendNum))
		{
			BlendNum += 32;
			num3 = Tile.m_TileInfo[(int)OutType].m_TextureNum;
		}
		else if (TestTileType(tileType, Tile.TileType.SoilTilled, Tile.TileType.SoilTilled, Tile.TileType.SoilTilled, i, j, num2, out Up, out Down, out Left, out Right, out OutType, out BlendNum))
		{
			BlendNum += 32;
			num3 = Tile.m_TileInfo[(int)OutType].m_TextureNum;
		}
		else if (TestTileType(tileType, Tile.TileType.Clay, Tile.TileType.Clay, Tile.TileType.Clay, i, j, num2, out Up, out Down, out Left, out Right, out OutType, out BlendNum))
		{
			BlendNum += 32;
			num3 = Tile.m_TileInfo[(int)OutType].m_TextureNum;
		}
		else if (TestTileType(tileType, Tile.TileType.Iron, Tile.TileType.Iron, Tile.TileType.Iron, i, j, num2, out Up, out Down, out Left, out Right, out OutType, out BlendNum))
		{
			BlendNum += 32;
			num3 = Tile.m_TileInfo[(int)OutType].m_TextureNum;
		}
		else if (TestTileType(tileType, Tile.TileType.Stone, Tile.TileType.Stone, Tile.TileType.Stone, i, j, num2, out Up, out Down, out Left, out Right, out OutType, out BlendNum))
		{
			BlendNum += 32;
			num3 = Tile.m_TileInfo[(int)OutType].m_TextureNum;
		}
		else if (TestTileType(tileType, Tile.TileType.IronSoil, Tile.TileType.IronSoil, Tile.TileType.IronSoil, i, j, num2, out Up, out Down, out Left, out Right, out OutType, out BlendNum))
		{
			BlendNum += 16;
			num3 = Tile.m_TileInfo[(int)OutType].m_TextureNum;
		}
		else if (TestTileType(tileType, Tile.TileType.ClaySoil, Tile.TileType.ClaySoil, Tile.TileType.ClaySoil, i, j, num2, out Up, out Down, out Left, out Right, out OutType, out BlendNum))
		{
			BlendNum += 16;
			num3 = Tile.m_TileInfo[(int)OutType].m_TextureNum;
		}
		else if (TestTileType(tileType, Tile.TileType.StoneSoil, Tile.TileType.StoneSoil, Tile.TileType.StoneSoil, i, j, num2, out Up, out Down, out Left, out Right, out OutType, out BlendNum))
		{
			BlendNum += 16;
			num3 = Tile.m_TileInfo[(int)OutType].m_TextureNum;
		}
		else if (TestTileType(tileType, Tile.TileType.CoalSoil, Tile.TileType.CoalSoil, Tile.TileType.CoalSoil, i, j, num2, out Up, out Down, out Left, out Right, out OutType, out BlendNum))
		{
			BlendNum += 16;
			num3 = Tile.m_TileInfo[(int)OutType].m_TextureNum;
		}
		else if (TestTileType(tileType, Tile.TileType.IronHidden, Tile.TileType.IronHidden, Tile.TileType.IronHidden, i, j, num2, out Up, out Down, out Left, out Right, out OutType, out BlendNum))
		{
			BlendNum += 16;
			num3 = Tile.m_TileInfo[(int)OutType].m_TextureNum;
		}
		else if (TestTileType(tileType, Tile.TileType.StoneHidden, Tile.TileType.StoneHidden, Tile.TileType.StoneHidden, i, j, num2, out Up, out Down, out Left, out Right, out OutType, out BlendNum))
		{
			BlendNum += 16;
			num3 = Tile.m_TileInfo[(int)OutType].m_TextureNum;
		}
		else if (TestTileType(tileType, Tile.TileType.ClayHidden, Tile.TileType.ClayHidden, Tile.TileType.ClayHidden, i, j, num2, out Up, out Down, out Left, out Right, out OutType, out BlendNum))
		{
			BlendNum += 16;
			num3 = Tile.m_TileInfo[(int)OutType].m_TextureNum;
		}
		else if (TestTileType(tileType, Tile.TileType.CoalHidden, Tile.TileType.CoalHidden, Tile.TileType.CoalHidden, i, j, num2, out Up, out Down, out Left, out Right, out OutType, out BlendNum))
		{
			BlendNum += 16;
			num3 = Tile.m_TileInfo[(int)OutType].m_TextureNum;
		}
		else if (TestTileType(tileType, Tile.TileType.WaterShallow, Tile.TileType.WaterDeep, Tile.TileType.WaterDeep, i, j, num2, out Up, out Down, out Left, out Right, out OutType, out BlendNum))
		{
			BlendNum = BlendNum;
			num3 = Tile.m_TileInfo[(int)OutType].m_TextureNum;
		}
		else if (TestTileType(tileType, Tile.TileType.WaterDeep, Tile.TileType.WaterDeep, Tile.TileType.WaterDeep, i, j, num2, out Up, out Down, out Left, out Right, out OutType, out BlendNum))
		{
			BlendNum = BlendNum;
			num3 = Tile.m_TileInfo[(int)OutType].m_TextureNum;
		}
		if (!Up)
		{
			num4++;
		}
		if (!Down)
		{
			num4 += 2;
		}
		if (!Left)
		{
			num4 += 4;
		}
		if (!Right)
		{
			num4 += 8;
		}
		if (num4 == 1 || num4 == 2 || num4 == 4 || num4 == 8)
		{
			int num5 = Random.Range(0, 3);
			BlendNum += num5 * 16 * 4;
		}
		BlendNum += num4;
		int num6 = BlendNum % num * 255 / num + num / 2;
		int num7 = BlendNum / num * 255 / num + num / 2;
		int num8 = num3 % num * 255 / num + num / 2;
		int num9 = num3 / num * 255 / num + num / 2;
		Color32[] colors = new Color32[1]
		{
			new Color32((byte)num8, (byte)num9, (byte)num6, (byte)num7)
		};
		m_TileMapTexture2.SetPixels32(i, j, 1, 1, colors);
	}

	public bool DoesObjectAffectRouteFinding(ObjectType NewType)
	{
		if (Boulder.GetIsTypeBoulder(NewType) || NewType == ObjectType.Hedge)
		{
			return true;
		}
		return false;
	}

	public void SetAssociatedObject(TileCoord Position, BaseClass NewObject)
	{
		int num = Position.y * m_TilesWide + Position.x;
		Tile tile = m_Tiles[num];
		if ((bool)tile.m_AssociatedObject)
		{
			if (tile.m_TileType == Tile.TileType.Soil || tile.m_TileType == Tile.TileType.SoilHole || tile.m_TileType == Tile.TileType.SoilTilled || tile.m_TileType == Tile.TileType.SoilDung)
			{
				SetTileType(Position, Tile.TileType.Soil);
			}
		}
		else if (tile.m_TileType == Tile.TileType.SoilUsed)
		{
			SetTileType(Position, Tile.TileType.Soil);
		}
		bool flag = false;
		if ((bool)tile.m_AssociatedObject && DoesObjectAffectRouteFinding(tile.m_AssociatedObject.m_TypeIdentifier))
		{
			flag = true;
		}
		tile.m_AssociatedObject = NewObject;
		if (((bool)tile.m_AssociatedObject && DoesObjectAffectRouteFinding(tile.m_AssociatedObject.m_TypeIdentifier)) || flag)
		{
			RouteFinding.UpdateTileWalk(Position.x, Position.y);
		}
	}

	public void ClearAssociatedObject(TileCoord Position, BaseClass NewObject)
	{
		int num = Position.y * m_TilesWide + Position.x;
		Tile tile = m_Tiles[num];
		if (!(tile.m_AssociatedObject != NewObject))
		{
			bool flag = false;
			if ((bool)tile.m_AssociatedObject && DoesObjectAffectRouteFinding(tile.m_AssociatedObject.m_TypeIdentifier))
			{
				flag = true;
			}
			tile.m_AssociatedObject = null;
			if (flag)
			{
				RouteFinding.UpdateTileWalk(Position.x, Position.y);
			}
			if (tile.m_TileType == Tile.TileType.SoilUsed || tile.m_TileType == Tile.TileType.SoilTilled)
			{
				SetTileType(Position, Tile.TileType.Soil);
			}
		}
	}

	public BaseClass GetAssociatedObject(TileCoord Position)
	{
		int num = Position.y * m_TilesWide + Position.x;
		return m_Tiles[num].m_AssociatedObject;
	}

	public void UpdateTile(TileCoord Position)
	{
		Color32[] array = new Color32[1];
		int num = Position.y * m_TilesWide + Position.x;
		Tile tile = m_Tiles[num];
		int num2 = 16;
		int tileTexture = GetTileTexture(tile, Position);
		int num3 = tileTexture % num2 * 256 / num2 + num2 / 2;
		int num4 = tileTexture / num2 * 256 / num2 + num2 / 2;
		int num5 = 0;
		if (tile.m_WalledArea != null)
		{
			num5 = 1;
		}
		array[0] = new Color32((byte)num3, (byte)num4, (byte)num5, byte.MaxValue);
		m_TileMapTexture.SetPixels32(Position.x, Position.y, 1, 1, array);
		m_TileMapChanges = true;
		m_ChangedTiles.Add(Position);
	}

	public void SetTileType(TileCoord Position, Tile.TileType NewTileType, Farmer NewFarmer = null)
	{
		int num = Position.y * m_TilesWide + Position.x;
		Tile tile = m_Tiles[num];
		if (tile.m_TileType == NewTileType)
		{
			return;
		}
		float tileHeight = GetTileHeight(Position);
		Tile.TileType tileType = tile.m_TileType;
		if (tileType == Tile.TileType.SeaWaterShallow)
		{
			ShorelineManager.Instance.CheckRemoveShorelineTile(Position.x, Position.y);
		}
		if (tileType == Tile.TileType.SoilHole)
		{
			tile.m_AssociatedObject.StopUsing();
			tile.m_AssociatedObject = null;
		}
		if (tileType == Tile.TileType.Swamp && (bool)tile.m_AssociatedObject)
		{
			tile.m_AssociatedObject.StopUsing();
			tile.m_AssociatedObject = null;
		}
		if (m_SoilTileIndexesDictionary.ContainsKey(num))
		{
			m_SoilTileIndexesDictionary.Remove(num);
			m_SoilTileIndexes.Remove(num);
		}
		if (m_EmptyTileIndexesDictionary.ContainsKey(num))
		{
			m_EmptyTileIndexesDictionary.Remove(num);
			m_EmptyTileIndexes.Remove(num);
		}
		tile.m_TileType = NewTileType;
		tile.m_Timer = 0f;
		UpdateTile(Position);
		RouteFinding.UpdateTileWalk(Position.x, Position.y);
		if (NewTileType == Tile.TileType.Soil && !tile.GetContainsObject())
		{
			m_SoilTileIndexesDictionary.Add(num, 0);
			m_SoilTileIndexes.Add(num);
		}
		if (NewTileType == Tile.TileType.Empty)
		{
			m_EmptyTileIndexesDictionary.Add(num, 0);
			m_EmptyTileIndexes.Add(num);
		}
		if (NewTileType == Tile.TileType.SoilHole)
		{
			BaseClass baseClass = ObjectTypeList.Instance.CreateObjectFromIdentifier(ObjectType.SoilHolePile, Position.ToWorldPositionTileCentered(), Quaternion.identity);
			baseClass.m_UniqueID = -2;
			tile.m_AssociatedObject = baseClass;
		}
		if (GetTileHeight(Position) != tileHeight)
		{
			PlotManager.Instance.TileHeightChanged(Position);
			PlotManager.Instance.GetPlotAtTile(Position).SetMeshDirty();
			int num2 = Position.x % Plot.m_PlotTilesWide;
			int num3 = Position.y % Plot.m_PlotTilesHigh;
			if (Position.x != 0 && num2 == 0)
			{
				PlotManager.Instance.GetPlotAtTile(Position + new TileCoord(-1, 0)).SetMeshDirty();
				if (Position.y != 0 && num3 == 0)
				{
					PlotManager.Instance.GetPlotAtTile(Position + new TileCoord(-1, -1)).SetMeshDirty();
				}
				if (Position.y != m_TilesHigh - 1 && num3 == Plot.m_PlotTilesHigh - 1)
				{
					PlotManager.Instance.GetPlotAtTile(Position + new TileCoord(-1, 1)).SetMeshDirty();
				}
			}
			if (Position.x != m_TilesWide - 1 && num2 == Plot.m_PlotTilesWide - 1)
			{
				PlotManager.Instance.GetPlotAtTile(Position + new TileCoord(1, 0)).SetMeshDirty();
				if (Position.y != 0 && num3 == 0)
				{
					PlotManager.Instance.GetPlotAtTile(Position + new TileCoord(1, -1)).SetMeshDirty();
				}
				if (Position.y != m_TilesHigh - 1 && num3 == Plot.m_PlotTilesHigh - 1)
				{
					PlotManager.Instance.GetPlotAtTile(Position + new TileCoord(1, 1)).SetMeshDirty();
				}
			}
			if (Position.y != 0 && num3 == 0)
			{
				PlotManager.Instance.GetPlotAtTile(Position + new TileCoord(0, -1)).SetMeshDirty();
			}
			if (Position.y != m_TilesHigh - 1 && num3 == Plot.m_PlotTilesHigh - 1)
			{
				PlotManager.Instance.GetPlotAtTile(Position + new TileCoord(0, 1)).SetMeshDirty();
			}
		}
		if (!SaveLoadManager.Instance.m_Loading && (bool)QuestManagerTiles.Instance)
		{
			QuestManagerTiles.Instance.TileChanged(Position, NewTileType, NewFarmer);
		}
		RecordingManager.Instance.ChangeTile(Position.x, Position.y, NewTileType);
	}

	public TileCoord GetCappedTileCoord(TileCoord OldPosition)
	{
		TileCoord result = OldPosition;
		if (result.x < 0)
		{
			result.x = 0;
		}
		if (result.y < 0)
		{
			result.y = 0;
		}
		if (result.x >= m_TilesWide)
		{
			result.x = m_TilesWide - 1;
		}
		if (result.y >= m_TilesHigh)
		{
			result.y = m_TilesHigh - 1;
		}
		return result;
	}

	public Tile GetTile(TileCoord Position)
	{
		if (Position.y < 0 || Position.x < 0 || Position.y >= m_TilesHigh || Position.x >= m_TilesWide)
		{
			return null;
		}
		return m_Tiles[Position.y * m_TilesWide + Position.x];
	}

	public Tile.TileType GetTileType(TileCoord Position)
	{
		if (Position.y < 0 || Position.x < 0 || Position.y >= m_TilesHigh || Position.x >= m_TilesWide)
		{
			return Tile.TileType.Total;
		}
		return m_Tiles[Position.y * m_TilesWide + Position.x].m_TileType;
	}

	public float GetTileDelay(TileCoord NewCoord, bool Worker)
	{
		float num = 0f;
		if (SaveLoadManager.m_Video)
		{
			return num;
		}
		Tile tile = Instance.GetTile(NewCoord);
		if ((bool)tile.m_Floor)
		{
			if (tile.m_Floor.m_TypeIdentifier == ObjectType.SandPath)
			{
				num -= 0.025f;
			}
			if (tile.m_Floor.m_TypeIdentifier == ObjectType.StonePath)
			{
				num -= 0.05f;
			}
			else if (tile.m_Floor.m_TypeIdentifier == ObjectType.RoadCrude)
			{
				num -= 0.075f;
			}
			else if (tile.m_Floor.m_TypeIdentifier == ObjectType.RoadGood)
			{
				num -= 0.1f;
			}
			else if (Bridge.GetIsTypeBridge(tile.m_Floor.m_TypeIdentifier) || tile.m_Floor.m_TypeIdentifier == ObjectType.TrainTrackBridge)
			{
				num -= 0.025f;
			}
			else if (tile.m_Floor.m_TypeIdentifier == ObjectType.FlooringCrude || tile.m_Floor.m_TypeIdentifier == ObjectType.Workshop || tile.m_Floor.m_TypeIdentifier == ObjectType.FlooringBrick || tile.m_Floor.m_TypeIdentifier == ObjectType.FlooringFlagstone || tile.m_Floor.m_TypeIdentifier == ObjectType.FlooringParquet)
			{
				num -= 0.05f;
			}
		}
		else
		{
			num += Tile.m_TileInfo[(int)tile.m_TileType].m_Speed;
			if ((bool)tile.m_AssociatedObject && tile.m_AssociatedObject.m_TypeIdentifier == ObjectType.Bush)
			{
				num += 0.2f;
			}
		}
		return num;
	}

	public void GetTileWalkable(TileCoord NewCoord, out bool WorkerWalk, out bool AnimalWalk, out bool BoatWalk, out bool PlayerWalk, bool TestFarmer, out float WalkCost)
	{
		if (NewCoord.y < 0 || NewCoord.x < 0 || NewCoord.y >= m_TilesHigh || NewCoord.x >= m_TilesWide)
		{
			WorkerWalk = false;
			AnimalWalk = false;
			PlayerWalk = false;
			BoatWalk = false;
			WalkCost = 0f;
			return;
		}
		Tile tile = m_Tiles[NewCoord.y * m_TilesWide + NewCoord.x];
		WorkerWalk = true;
		AnimalWalk = true;
		PlayerWalk = true;
		BoatWalk = false;
		if (TileHelpers.GetTileWater(tile.m_TileType) && tile.m_Floor == null && tile.m_Building == null && tile.m_BuildingFootprint == null)
		{
			BoatWalk = true;
		}
		if (Tile.m_TileInfo[(int)tile.m_TileType].m_Solid && (tile.m_Floor == null || tile.m_Floor.m_TypeIdentifier == ObjectType.ConverterFoundation))
		{
			WorkerWalk = false;
			AnimalWalk = false;
			PlayerWalk = false;
		}
		if ((bool)tile.m_Building)
		{
			if ((bool)tile.m_Building && tile.m_Building.m_TypeIdentifier == ObjectType.ConverterFoundation)
			{
				if (tile.m_Building.GetComponent<ConverterFoundation>().m_State == Converter.State.Converting)
				{
					WorkerWalk = false;
					AnimalWalk = false;
					PlayerWalk = false;
				}
			}
			else if ((bool)tile.m_Building && Door.GetIsTypeDoor(tile.m_Building.m_TypeIdentifier))
			{
				WorkerWalk = true;
				AnimalWalk = false;
				PlayerWalk = true;
			}
			else if ((bool)tile.m_Building && Building.GetIsTypeWalkable(tile.m_Building.m_TypeIdentifier))
			{
				WorkerWalk = true;
				AnimalWalk = true;
				PlayerWalk = true;
			}
			else
			{
				WorkerWalk = false;
				AnimalWalk = false;
				PlayerWalk = false;
			}
		}
		else if ((bool)tile.m_AssociatedObject)
		{
			ObjectType typeIdentifier = tile.m_AssociatedObject.m_TypeIdentifier;
			if (DoesObjectAffectRouteFinding(typeIdentifier))
			{
				WorkerWalk = false;
				AnimalWalk = false;
				PlayerWalk = false;
			}
		}
		WalkCost = 0.2f + GetTileDelay(NewCoord, Worker: true);
		if (!PlotManager.Instance.GetPlotAtTile(NewCoord).m_Visible)
		{
			WorkerWalk = false;
			AnimalWalk = false;
		}
	}

	public bool GetTileSolidToPlayer(TileCoord NewCoord)
	{
		GetTileWalkable(NewCoord, out var _, out var _, out var _, out var PlayerWalk, TestFarmer: true, out var _);
		return !PlayerWalk;
	}

	private void UpdateActive()
	{
		List<int> list = new List<int>();
		int num = 0;
		foreach (int activeSoilTileIndex in m_ActiveSoilTileIndexes)
		{
			if (m_Tiles[activeSoilTileIndex].m_Building == null && m_Tiles[activeSoilTileIndex].m_BuildingFootprint == null && m_Tiles[activeSoilTileIndex].m_Floor == null && m_Tiles[activeSoilTileIndex].m_MiscObject == null && m_Tiles[activeSoilTileIndex].m_Farmer == null)
			{
				float num2 = m_ActiveSoilTileTimes[num] + TimeManager.Instance.m_NormalDelta;
				m_ActiveSoilTileTimes[num] = num2;
				if (num2 > Tile.m_WeededDelay)
				{
					list.Add(activeSoilTileIndex);
				}
			}
			num++;
		}
		foreach (int item in list)
		{
			int nx = item % m_TilesWide;
			int ny = item / m_TilesWide;
			TileCoord position = new TileCoord(nx, ny);
			ObjectTypeList.Instance.CreateObjectFromIdentifier(ObjectType.Weed, position.ToWorldPositionTileCentered(), Quaternion.identity);
			AudioManager.Instance.StartEvent("CropWeedAppear", GetTile(position).m_AssociatedObject.GetComponent<TileCoordObject>());
		}
	}

	private void UpdateChangedTiles()
	{
		foreach (TileCoord changedTile in m_ChangedTiles)
		{
			UpdateTileShading(changedTile.x, changedTile.y);
			if (changedTile.x != 0)
			{
				UpdateTileShading(changedTile.x - 1, changedTile.y);
			}
			if (changedTile.x != m_TilesWide - 1)
			{
				UpdateTileShading(changedTile.x + 1, changedTile.y);
			}
			if (changedTile.y != 0)
			{
				UpdateTileShading(changedTile.x, changedTile.y - 1);
			}
			if (changedTile.y != m_TilesHigh - 1)
			{
				UpdateTileShading(changedTile.x, changedTile.y + 1);
			}
		}
		m_ChangedTiles.Clear();
		m_TileMapTexture2.Apply();
	}

	public Tile.TileType GetTileTypeCapped(TileCoord Position)
	{
		if (m_Tiles == null)
		{
			return Tile.TileType.Total;
		}
		if (Position.x < 0)
		{
			Position.x = 0;
		}
		if (Position.x >= m_TilesWide)
		{
			Position.x = m_TilesWide - 1;
		}
		if (Position.y < 0)
		{
			Position.y = 0;
		}
		if (Position.y >= m_TilesHigh)
		{
			Position.y = m_TilesHigh - 1;
		}
		return m_Tiles[Position.y * m_TilesWide + Position.x].m_TileType;
	}

	private int TileHeightIndex(Tile.TileType NewType, bool IgnoreWater)
	{
		switch (NewType)
		{
		case Tile.TileType.Coal:
		case Tile.TileType.CoalUsed:
			return -4;
		case Tile.TileType.Iron:
		case Tile.TileType.IronUsed:
		case Tile.TileType.CoalSoil3:
			return -3;
		case Tile.TileType.IronSoil2:
		case Tile.TileType.CoalSoil2:
		case Tile.TileType.Stone:
		case Tile.TileType.StoneUsed:
			return -2;
		case Tile.TileType.Clay:
		case Tile.TileType.ClayUsed:
			return -1;
		case Tile.TileType.Dredged:
			return -3;
		case Tile.TileType.WaterDeep:
		case Tile.TileType.SeaWaterDeep:
			if (IgnoreWater)
			{
				return 0;
			}
			return -4;
		case Tile.TileType.WaterShallow:
		case Tile.TileType.SeaWaterShallow:
		case Tile.TileType.Swamp:
			if (IgnoreWater)
			{
				return 0;
			}
			return -2;
		default:
			return 0;
		}
	}

	public int GetTileHeightIndex(TileCoord Position, bool IgnoreWater = false)
	{
		Tile.TileType tileTypeCapped = GetTileTypeCapped(Position);
		return TileHeightIndex(tileTypeCapped, IgnoreWater);
	}

	public float GetTileHeight(TileCoord Position, bool IgnoreWater = false)
	{
		Tile.TileType tileTypeCapped = GetTileTypeCapped(Position);
		if (IgnoreWater && TileHelpers.GetTileWater(tileTypeCapped))
		{
			return PlotMeshBuilderWater.m_WaterLevel;
		}
		return (float)TileHeightIndex(tileTypeCapped, IgnoreWater) * 0.333333f;
	}

	private void CheckConnectedTile(TileCoord NewCoord)
	{
		if (m_SearchDone)
		{
			return;
		}
		int num = NewCoord.y * m_TilesWide + NewCoord.x;
		Tile tile = m_Tiles[num];
		if (tile.m_Checked)
		{
			return;
		}
		AddTileChecked(num);
		if (!tile.m_BuildingFootprint && !tile.m_Floor)
		{
			return;
		}
		if ((bool)tile.m_BuildingFootprint)
		{
			tile.m_BuildingFootprint.SetConnectedToTransmitter(Connected: true);
			if (tile.m_BuildingFootprint.m_TypeIdentifier == ObjectType.BeltLinkage)
			{
				BeltLinkage component = tile.m_BuildingFootprint.GetComponent<BeltLinkage>();
				if ((bool)component.m_BeltConnectTo)
				{
					CheckConnectedTile(component.m_BeltConnectTo.m_TileCoord);
				}
				if ((bool)component.m_Belt)
				{
					CheckConnectedTile(component.m_Belt.m_ConnectedTo.m_TileCoord);
				}
				if ((bool)component.m_RodConnectTo)
				{
					CheckConnectedTile(component.m_RodConnectTo.m_TileCoord);
				}
				if ((bool)component.m_Rod)
				{
					CheckConnectedTile(component.m_Rod.m_ConnectedTo.m_TileCoord);
				}
			}
		}
		if ((bool)tile.m_Floor)
		{
			tile.m_Floor.SetConnectedToTransmitter(Connected: true);
		}
		if (NewCoord.x > 0)
		{
			CheckConnectedTile(NewCoord + new TileCoord(-1, 0));
		}
		if (NewCoord.x < m_TilesWide - 1)
		{
			CheckConnectedTile(NewCoord + new TileCoord(1, 0));
		}
		if (NewCoord.y > 0)
		{
			CheckConnectedTile(NewCoord + new TileCoord(0, -1));
		}
		if (NewCoord.y < m_TilesHigh - 1)
		{
			CheckConnectedTile(NewCoord + new TileCoord(0, 1));
		}
	}

	private void UpdateTransmitterConnections()
	{
		Tile[] tiles = m_Tiles;
		foreach (Tile tile in tiles)
		{
			if ((bool)tile.m_BuildingFootprint)
			{
				tile.m_BuildingFootprint.SetConnectedToTransmitter(Connected: false);
			}
			if ((bool)tile.m_Floor)
			{
				tile.m_Floor.SetConnectedToTransmitter(Connected: false);
			}
		}
		StartTileSearch(10000);
		Dictionary<BaseClass, int> collection = CollectionManager.Instance.GetCollection("Transmitter");
		if (collection != null)
		{
			Transmitter transmitter = null;
			using (Dictionary<BaseClass, int>.Enumerator enumerator = collection.GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					transmitter = enumerator.Current.Key.GetComponent<Transmitter>();
				}
			}
			CheckConnectedTile(transmitter.m_TileCoord);
			foreach (KeyValuePair<BaseClass, int> item in CollectionManager.Instance.GetCollection("Building"))
			{
				Building component = item.Key.GetComponent<Building>();
				if (component.m_ParentBuilding == null)
				{
					component.TestConnectedToTransmitter();
				}
			}
			Shader.SetGlobalFloat("_ConnectedSaturation", 0.5f);
		}
		ClearCheckedTiles();
	}

	public void StartTileSearch(int MaxSearchSize)
	{
		m_MaxSearchSize = MaxSearchSize;
		m_SearchIndex = 0;
		m_SearchDone = false;
	}

	public bool AddTileChecked(int Index)
	{
		if (m_SearchDone)
		{
			Debug.Log("Maximum tiles checked");
			return false;
		}
		m_Tiles[Index].m_Checked = true;
		m_TilesChecked[m_SearchIndex] = Index;
		m_SearchIndex++;
		if (m_SearchIndex == m_MaxSearchSize)
		{
			m_SearchDone = true;
		}
		return true;
	}

	public void ClearCheckedTiles()
	{
		for (int i = 0; i < m_SearchIndex; i++)
		{
			m_Tiles[m_TilesChecked[i]].m_Checked = false;
		}
	}

	private void Update()
	{
		if (m_TileMapChanges)
		{
			m_TileMapChanges = false;
			m_TileMapTexture.Apply();
		}
		if (m_ChangedTiles.Count > 0)
		{
			UpdateChangedTiles();
		}
	}
}
