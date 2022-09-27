using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;

public class TileUseManager : MonoBehaviour
{
	public static TileUseManager Instance;

	private Dictionary<TileCoord, int> m_Tiles;

	private Dictionary<TileCoord, TileUsed> m_UsedTiles;

	private void Awake()
	{
		Instance = this;
		m_Tiles = new Dictionary<TileCoord, int>();
		m_UsedTiles = new Dictionary<TileCoord, TileUsed>();
	}

	public void Save(JSONNode Node)
	{
		JSONArray jSONArray = (JSONArray)(Node["PartUsed"] = new JSONArray());
		int num = 0;
		foreach (KeyValuePair<TileCoord, int> tile in m_Tiles)
		{
			jSONArray[num] = tile.Key.x;
			num++;
			jSONArray[num] = tile.Key.y;
			num++;
			jSONArray[num] = tile.Value;
			num++;
		}
		JSONArray jSONArray2 = (JSONArray)(Node["Used"] = new JSONArray());
		num = 0;
		foreach (KeyValuePair<TileCoord, TileUsed> usedTile in m_UsedTiles)
		{
			jSONArray2[num] = usedTile.Key.x;
			num++;
			jSONArray2[num] = usedTile.Key.y;
			num++;
			jSONArray2[num] = Tile.GetNameFromType(usedTile.Value.m_Type);
			num++;
			jSONArray2[num] = (int)(usedTile.Value.m_Time * 100f);
			num++;
		}
	}

	public void Load(JSONNode Node)
	{
		m_Tiles = new Dictionary<TileCoord, int>();
		m_UsedTiles = new Dictionary<TileCoord, TileUsed>();
		JSONArray asArray = Node["PartUsed"].AsArray;
		int num = 0;
		while (num < asArray.Count)
		{
			int asInt = asArray[num].AsInt;
			num++;
			int asInt2 = asArray[num].AsInt;
			num++;
			int asInt3 = asArray[num].AsInt;
			num++;
			m_Tiles.Add(new TileCoord(asInt, asInt2), asInt3);
		}
		JSONArray asArray2 = Node["Used"].AsArray;
		num = 0;
		while (num < asArray2.Count)
		{
			int asInt4 = asArray2[num].AsInt;
			num++;
			int asInt5 = asArray2[num].AsInt;
			num++;
			Tile.TileType typeFromName = Tile.GetTypeFromName(asArray2[num]);
			num++;
			float time = (float)asArray2[num].AsInt / 100f;
			num++;
			TileCoord tileCoord = new TileCoord(asInt4, asInt5);
			TileUsed tileUsed = new TileUsed(typeFromName);
			tileUsed.m_Time = time;
			m_UsedTiles.Add(tileCoord, tileUsed);
			TileChanged(tileCoord, typeFromName);
		}
	}

	private void TileChanged(TileCoord Position, Tile.TileType NewType)
	{
		if (NewType == Tile.TileType.Iron)
		{
			TileManager.Instance.SetTileType(Position, Tile.TileType.IronUsed);
		}
		if (NewType == Tile.TileType.Clay)
		{
			TileManager.Instance.SetTileType(Position, Tile.TileType.ClayUsed);
		}
		if (NewType == Tile.TileType.Coal)
		{
			TileManager.Instance.SetTileType(Position, Tile.TileType.CoalUsed);
		}
		if (NewType == Tile.TileType.Stone)
		{
			TileManager.Instance.SetTileType(Position, Tile.TileType.StoneUsed);
		}
	}

	public void UseTile(TileCoord Position, Tile.TileType NewType, int Amount = 1)
	{
	}

	public void RemoveTile(TileCoord Position)
	{
		if (m_Tiles.ContainsKey(Position))
		{
			m_Tiles.Remove(Position);
		}
		if (m_UsedTiles.ContainsKey(Position))
		{
			m_UsedTiles.Remove(Position);
		}
	}

	private void Update()
	{
		List<TileCoord> list = new List<TileCoord>();
		foreach (KeyValuePair<TileCoord, TileUsed> usedTile in m_UsedTiles)
		{
			usedTile.Value.m_Time += TimeManager.Instance.m_NormalDelta;
			float recoverDelay = Tile.m_TileInfo[(int)usedTile.Value.m_Type].m_RecoverDelay;
			if (usedTile.Value.m_Time > recoverDelay)
			{
				TileManager.Instance.SetTileType(usedTile.Key, usedTile.Value.m_Type);
				list.Add(usedTile.Key);
			}
		}
		foreach (TileCoord item in list)
		{
			m_UsedTiles.Remove(item);
		}
	}
}
