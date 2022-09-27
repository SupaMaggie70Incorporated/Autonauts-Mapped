using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;

public class WaterManager : MonoBehaviour
{
	public static WaterManager Instance;

	private static float m_UpdateDelay = 0.05f;

	private Dictionary<TileCoord, DredgedTile> m_Tiles;

	private float m_UpdateTimer;

	private void Awake()
	{
		Instance = this;
		m_Tiles = new Dictionary<TileCoord, DredgedTile>();
		m_UpdateTimer = 0f;
	}

	public void Save(JSONNode Node)
	{
		JSONArray jSONArray = (JSONArray)(Node["Tiles"] = new JSONArray());
		int num = 0;
		foreach (KeyValuePair<TileCoord, DredgedTile> tile in m_Tiles)
		{
			JSONNode jSONNode2 = new JSONObject();
			tile.Value.Save(jSONNode2);
			jSONArray[num] = jSONNode2;
			num++;
		}
	}

	public void Load(JSONNode Node)
	{
		JSONArray asArray = Node["Tiles"].AsArray;
		for (int i = 0; i < asArray.Count; i++)
		{
			JSONNode asObject = asArray[i].AsObject;
			DredgedTile dredgedTile = new DredgedTile();
			dredgedTile.Load(asObject);
			m_Tiles.Add(dredgedTile.m_Coord, dredgedTile);
		}
	}

	public void AddDredgedTile(TileCoord NewTile)
	{
		if (!m_Tiles.ContainsKey(NewTile))
		{
			DredgedTile value = new DredgedTile(NewTile);
			m_Tiles.Add(NewTile, value);
		}
	}

	private bool CheckAdjacentWaterTile(TileCoord OldCoord, TileCoord Delta, DredgedTile NewTile)
	{
		if (m_Tiles.ContainsKey(OldCoord))
		{
			return false;
		}
		Tile.TileType tileType = TileManager.Instance.GetTileType(OldCoord + Delta);
		if (TileHelpers.GetTileWater(tileType))
		{
			NewTile.StartFilling(tileType, -Delta);
			m_Tiles.Add(OldCoord, NewTile);
			return true;
		}
		return false;
	}

	private void CheckAdjacentDredgedTile(TileCoord NewCoord)
	{
		if (TileManager.Instance.GetTileType(NewCoord) == Tile.TileType.Dredged)
		{
			AddDredgedTile(NewCoord);
		}
	}

	private void Update()
	{
		if (SaveLoadManager.Instance.m_Loading)
		{
			return;
		}
		m_UpdateTimer += TimeManager.Instance.m_NormalDelta;
		if (!(m_UpdateTimer > m_UpdateDelay))
		{
			return;
		}
		m_UpdateTimer -= m_UpdateDelay;
		Dictionary<TileCoord, DredgedTile> tiles = m_Tiles;
		m_Tiles = new Dictionary<TileCoord, DredgedTile>();
		foreach (KeyValuePair<TileCoord, DredgedTile> item in tiles)
		{
			if (!item.Value.m_Filling)
			{
				if (!CheckAdjacentWaterTile(item.Key, new TileCoord(0, -1), item.Value) && !CheckAdjacentWaterTile(item.Key, new TileCoord(0, 1), item.Value) && !CheckAdjacentWaterTile(item.Key, new TileCoord(-1, 0), item.Value))
				{
					CheckAdjacentWaterTile(item.Key, new TileCoord(1, 0), item.Value);
				}
			}
			else
			{
				TileManager.Instance.SetTileType(item.Key, item.Value.m_FillingType);
				CheckAdjacentDredgedTile(item.Key + new TileCoord(0, -1));
				CheckAdjacentDredgedTile(item.Key + new TileCoord(0, 1));
				CheckAdjacentDredgedTile(item.Key + new TileCoord(-1, 0));
				CheckAdjacentDredgedTile(item.Key + new TileCoord(1, 0));
			}
		}
	}
}
