using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;

public class SoilManager : MonoBehaviour
{
	public static SoilManager Instance;

	private Dictionary<TileCoord, int> m_Tiles;

	private void Awake()
	{
		Instance = this;
		m_Tiles = new Dictionary<TileCoord, int>();
	}

	public void Save(JSONNode Node)
	{
		JSONArray jSONArray = (JSONArray)(Node["Tiles"] = new JSONArray());
		int num = 0;
		foreach (KeyValuePair<TileCoord, int> tile in m_Tiles)
		{
			JSONNode jSONNode2 = new JSONObject();
			tile.Key.Save(jSONNode2, "T");
			JSONUtils.Set(jSONNode2, "C", tile.Value);
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
			TileCoord key = default(TileCoord);
			key.Load(asObject, "T");
			int asInt = JSONUtils.GetAsInt(asObject, "C", 0);
			m_Tiles.Add(key, asInt);
		}
	}

	public void AddSoil(TileCoord NewCoord, int Amount, ObjectType SoilType, Farmer NewFarmer)
	{
		int num = 0;
		if (m_Tiles.ContainsKey(NewCoord))
		{
			num = m_Tiles[NewCoord];
		}
		else
		{
			m_Tiles.Add(NewCoord, Amount);
		}
		num += Amount;
		m_Tiles[NewCoord] = num;
		Tile.TileType tileType = TileManager.Instance.GetTileType(NewCoord);
		int num2 = 6;
		if (tileType == Tile.TileType.Swamp || tileType == Tile.TileType.WaterShallow || tileType == Tile.TileType.Dredged)
		{
			num2 = 3;
		}
		if (num < num2)
		{
			return;
		}
		Tile.TileType newTileType = Tile.TileType.Soil;
		if (SoilType == ObjectType.Sand)
		{
			newTileType = Tile.TileType.Sand;
		}
		switch (tileType)
		{
		case Tile.TileType.WaterShallow:
			newTileType = Tile.TileType.Swamp;
			break;
		case Tile.TileType.WaterDeep:
			newTileType = Tile.TileType.WaterShallow;
			break;
		case Tile.TileType.SeaWaterDeep:
			newTileType = Tile.TileType.SeaWaterShallow;
			break;
		}
		TileManager.Instance.SetTileType(NewCoord, newTileType, NewFarmer);
		if (tileType == Tile.TileType.SeaWaterShallow || tileType == Tile.TileType.WaterShallow)
		{
			for (int i = -1; i <= 1; i++)
			{
				for (int j = -1; j <= 1; j++)
				{
					TileCoord position = NewCoord + new TileCoord(j, i);
					switch (TileManager.Instance.GetTileType(position))
					{
					case Tile.TileType.SeaWaterDeep:
						TileManager.Instance.SetTileType(position, Tile.TileType.SeaWaterShallow, NewFarmer);
						break;
					case Tile.TileType.WaterDeep:
						TileManager.Instance.SetTileType(position, Tile.TileType.WaterShallow, NewFarmer);
						break;
					}
				}
			}
		}
		m_Tiles.Remove(NewCoord);
	}
}
