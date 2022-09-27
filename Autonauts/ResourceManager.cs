using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
	public static ResourceManager Instance;

	private Dictionary<ObjectType, int> m_Resources;

	private Dictionary<ObjectType, int> m_StorageCapacity;

	private List<Storage> m_Storages;

	private Dictionary<ObjectType, int> m_Reserved;

	private void Awake()
	{
		Instance = this;
		m_Resources = new Dictionary<ObjectType, int>();
		m_StorageCapacity = new Dictionary<ObjectType, int>();
		m_Storages = new List<Storage>();
		m_Reserved = new Dictionary<ObjectType, int>();
	}

	public void Save(JSONNode Node)
	{
		JSONArray jSONArray = (JSONArray)(Node["ResourceTypes"] = new JSONArray());
		JSONArray jSONArray2 = (JSONArray)(Node["ResourceCount"] = new JSONArray());
		int num = 0;
		foreach (KeyValuePair<ObjectType, int> resource in m_Resources)
		{
			jSONArray[num] = ObjectTypeList.Instance.GetSaveNameFromIdentifier(resource.Key);
			jSONArray2[num] = resource.Value;
			num++;
		}
		jSONArray = (JSONArray)(Node["ReservedTypes"] = new JSONArray());
		jSONArray2 = (JSONArray)(Node["ReservedCount"] = new JSONArray());
		num = 0;
		foreach (KeyValuePair<ObjectType, int> item in m_Reserved)
		{
			jSONArray[num] = ObjectTypeList.Instance.GetSaveNameFromIdentifier(item.Key);
			jSONArray2[num] = item.Value;
			num++;
		}
	}

	public void Load(JSONNode Node)
	{
		m_Resources.Clear();
		m_StorageCapacity.Clear();
		m_Storages.Clear();
		m_Reserved.Clear();
		JSONArray asArray = Node["ResourceTypes"].AsArray;
		JSONArray asArray2 = Node["ResourceCount"].AsArray;
		for (int i = 0; i < asArray.Count; i++)
		{
			ObjectType identifierFromSaveName = ObjectTypeList.Instance.GetIdentifierFromSaveName(asArray[i].Value, Check: false);
			if (identifierFromSaveName == ObjectTypeList.m_Total)
			{
				Debug.Log("ResourceManager.Load : Ignorning unknown type " + asArray[i].Value);
				continue;
			}
			int result = 0;
			int.TryParse(asArray2[i].Value, out result);
			AddResource(identifierFromSaveName, result);
		}
		asArray = Node["ReservedTypes"].AsArray;
		if (!(asArray != null) || asArray.IsNull)
		{
			return;
		}
		asArray = Node["ReservedTypes"].AsArray;
		asArray2 = Node["ReservedCount"].AsArray;
		for (int j = 0; j < asArray.Count; j++)
		{
			ObjectType identifierFromSaveName2 = ObjectTypeList.Instance.GetIdentifierFromSaveName(asArray[j].Value, Check: false);
			if (identifierFromSaveName2 == ObjectTypeList.m_Total)
			{
				continue;
			}
			int result2 = 0;
			if (int.TryParse(asArray2[j].Value, out result2))
			{
				for (int k = 0; k < result2; k++)
				{
					ReserveResource(identifierFromSaveName2);
				}
			}
		}
	}

	public void AddResource(ObjectType NewType, int Count)
	{
		if (m_Resources.TryGetValue(NewType, out var value))
		{
			m_Resources[NewType] = value + Count;
		}
		else
		{
			m_Resources.Add(NewType, Count);
		}
	}

	public ObjectType FindBuildingResource(ObjectType OldType, bool IncludeUpgrades = true)
	{
		ObjectType result = ObjectTypeList.m_Total;
		if (m_Resources.TryGetValue(OldType, out var value) && value > 0)
		{
			result = OldType;
		}
		return result;
	}

	public ObjectType ReleaseResource(ObjectType OldType, bool IncludeUpgrades = true)
	{
		ObjectType objectType = FindBuildingResource(OldType, IncludeUpgrades);
		if (objectType != ObjectTypeList.m_Total && m_Resources.TryGetValue(objectType, out var value))
		{
			m_Resources[objectType] = value - 1;
			return objectType;
		}
		ErrorMessage.LogError("ReleaseResource : Couldn't find Resource type " + objectType);
		return ObjectTypeList.m_Total;
	}

	public bool ReserveResource(ObjectType NewType)
	{
		if (m_Resources.TryGetValue(NewType, out var value))
		{
			m_Reserved.TryGetValue(NewType, out value);
			m_Reserved[NewType] = value + 1;
			return true;
		}
		ErrorMessage.LogError("ReserveResource : Couldn't find Resource type " + NewType);
		return false;
	}

	public bool UnreserveResource(ObjectType NewType)
	{
		if (m_Reserved.TryGetValue(NewType, out var value))
		{
			m_Reserved[NewType] = value - 1;
			return true;
		}
		ErrorMessage.LogError("UnReserveResource : Couldn't find Resource type " + NewType);
		return false;
	}

	public int TallyBuildingResource(ObjectType NewType, int Total, bool IncludeUpgrades = true)
	{
		int num = Total;
		if (m_Resources.TryGetValue(NewType, out var value))
		{
			num += value;
			if (m_Reserved.TryGetValue(NewType, out var value2))
			{
				num -= value2;
			}
		}
		return num;
	}

	public int GetResource(ObjectType NewType, bool IncludeUpgrades = true)
	{
		return TallyBuildingResource(NewType, 0, IncludeUpgrades);
	}

	public int GetResourceForDisplay(ObjectType NewType)
	{
		if (m_Resources.TryGetValue(NewType, out var value))
		{
			return value;
		}
		return 0;
	}

	public void RegisterStorage(Storage NewStorage)
	{
	}

	public void UnRegisterStorage(Storage NewStorage)
	{
	}

	public int GetStorage(ObjectType NewType)
	{
		if (m_StorageCapacity.TryGetValue(NewType, out var value))
		{
			return value;
		}
		return 0;
	}

	public bool FoundationRequired(ObjectType NewBuildingType)
	{
		if (NewBuildingType == ObjectType.FolkSeedPod || NewBuildingType == ObjectType.Transmitter || !CheatManager.Instance.m_InstantBuild)
		{
			if (GetResource(NewBuildingType) == 0)
			{
				return true;
			}
			return false;
		}
		return false;
	}
}
