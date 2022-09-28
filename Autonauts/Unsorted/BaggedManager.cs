using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;

public class BaggedManager : MonoBehaviour
{
	public static BaggedManager Instance;

	[HideInInspector]
	public Dictionary<BaseClass, BaseClass> m_BaggedObjects;

	[HideInInspector]
	public Dictionary<int, BaseClass> m_BaggedTiles;

	private void Awake()
	{
		Instance = this;
		m_BaggedObjects = new Dictionary<BaseClass, BaseClass>();
		m_BaggedTiles = new Dictionary<int, BaseClass>();
	}

	public void Save(JSONNode Node)
	{
		JSONArray jSONArray = (JSONArray)(Node["Objects"] = new JSONArray());
		int num = 0;
		foreach (KeyValuePair<BaseClass, BaseClass> baggedObject in m_BaggedObjects)
		{
			jSONArray[num] = baggedObject.Key.m_UniqueID;
			num++;
			jSONArray[num] = baggedObject.Value.m_UniqueID;
			num++;
		}
		JSONArray jSONArray2 = (JSONArray)(Node["Tiles"] = new JSONArray());
		num = 0;
		foreach (KeyValuePair<int, BaseClass> baggedTile in m_BaggedTiles)
		{
			jSONArray2[num] = baggedTile.Key;
			num++;
			jSONArray2[num] = baggedTile.Value.m_UniqueID;
			num++;
		}
	}

	public void Load(JSONNode Node)
	{
		if (LoadJSON.m_LoadingString == "V131.7")
		{
			return;
		}
		m_BaggedObjects = new Dictionary<BaseClass, BaseClass>();
		m_BaggedTiles = new Dictionary<int, BaseClass>();
		JSONArray asArray = Node["Objects"].AsArray;
		int num = 0;
		while (num < asArray.Count)
		{
			int iD = asArray[num];
			BaseClass objectFromUniqueID = ObjectTypeList.Instance.GetObjectFromUniqueID(iD, ErrorCheck: false);
			num++;
			int iD2 = asArray[num];
			BaseClass objectFromUniqueID2 = ObjectTypeList.Instance.GetObjectFromUniqueID(iD2, ErrorCheck: false);
			num++;
			if ((bool)objectFromUniqueID && (bool)objectFromUniqueID2)
			{
				if ((bool)objectFromUniqueID2.GetComponent<Farmer>())
				{
					objectFromUniqueID2.GetComponent<Farmer>().SetBaggedObject(objectFromUniqueID.GetComponent<TileCoordObject>());
				}
				else if ((bool)objectFromUniqueID2.GetComponent<Animal>())
				{
					objectFromUniqueID2.GetComponent<Animal>().SetBaggedObject(objectFromUniqueID.GetComponent<TileCoordObject>());
				}
			}
		}
		JSONArray asArray2 = Node["Tiles"].AsArray;
		num = 0;
		while (num < asArray2.Count)
		{
			int index = asArray2[num];
			num++;
			int iD3 = asArray2[num];
			BaseClass objectFromUniqueID3 = ObjectTypeList.Instance.GetObjectFromUniqueID(iD3, ErrorCheck: false);
			num++;
			if ((bool)objectFromUniqueID3)
			{
				if ((bool)objectFromUniqueID3.GetComponent<Farmer>())
				{
					objectFromUniqueID3.GetComponent<Farmer>().SetBaggedTile(new TileCoord(index));
				}
				else if ((bool)objectFromUniqueID3.GetComponent<Animal>())
				{
					objectFromUniqueID3.GetComponent<Animal>().SetBaggedTile(new TileCoord(index));
				}
			}
		}
	}

	public void Add(BaseClass BaggedObject, BaseClass Bagger)
	{
		if (!m_BaggedObjects.ContainsKey(BaggedObject))
		{
			m_BaggedObjects.Add(BaggedObject, Bagger);
		}
		if ((bool)DespawnManager.Instance && (bool)BaggedObject.GetComponent<Holdable>())
		{
			DespawnManager.Instance.Remove(BaggedObject.GetComponent<Holdable>());
		}
	}

	public void Remove(BaseClass BaggedObject)
	{
		if (m_BaggedObjects.ContainsKey(BaggedObject))
		{
			m_BaggedObjects.Remove(BaggedObject);
		}
	}

	public bool IsObjectBagged(BaseClass NewObject)
	{
		return m_BaggedObjects.ContainsKey(NewObject);
	}

	public BaseClass GetObjectBagger(BaseClass NewObject)
	{
		if (!IsObjectBagged(NewObject))
		{
			return null;
		}
		return m_BaggedObjects[NewObject];
	}

	public void Add(TileCoord NewTile, BaseClass Bagger)
	{
		int index = NewTile.GetIndex();
		if (!m_BaggedTiles.ContainsKey(index))
		{
			m_BaggedTiles.Add(index, Bagger);
		}
	}

	public void Remove(TileCoord NewTile)
	{
		int index = NewTile.GetIndex();
		if (m_BaggedTiles.ContainsKey(index))
		{
			m_BaggedTiles.Remove(index);
		}
	}

	public bool IsTileBagged(TileCoord NewTile)
	{
		int index = NewTile.GetIndex();
		return m_BaggedTiles.ContainsKey(index);
	}

	public BaseClass GetTileBagger(TileCoord NewTile)
	{
		if (!IsTileBagged(NewTile))
		{
			return null;
		}
		int index = NewTile.GetIndex();
		return m_BaggedTiles[index];
	}
}
