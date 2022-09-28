using System.Collections.Generic;
using UnityEngine;

public class CollectionManager : MonoBehaviour
{
	public static CollectionManager Instance;

	private static Dictionary<string, Collection> m_Objects;

	private static List<BaseClass> m_Players;

	private void Awake()
	{
		Instance = this;
		m_Objects = new Dictionary<string, Collection>();
		m_Players = new List<BaseClass>();
	}

	private void OnDestroy()
	{
		Instance = null;
	}

	public void AddCollectable(string ClassName, BaseClass NewCollectable)
	{
		if (NewCollectable.m_TypeIdentifier == ObjectType.FarmerPlayer)
		{
			m_Players.Add(NewCollectable);
		}
		if (!m_Objects.ContainsKey(ClassName))
		{
			m_Objects.Add(ClassName, new Collection());
		}
		if (m_Objects.TryGetValue(ClassName, out var value) && !value.m_ObjectsDictionary.ContainsKey(NewCollectable))
		{
			value.m_ObjectsDictionary.Add(NewCollectable, 0);
		}
	}

	public void RemoveCollectable(string ClassName, BaseClass NewCollectable)
	{
		if (NewCollectable.m_TypeIdentifier == ObjectType.FarmerPlayer)
		{
			m_Players.Remove(NewCollectable);
		}
		if (m_Objects.ContainsKey(ClassName))
		{
			if (m_Objects.TryGetValue(ClassName, out var value))
			{
				value.m_ObjectsDictionary.Remove(NewCollectable);
			}
			else
			{
				ErrorMessage.LogError("RemoveCollectable : object doesn't exist in class list " + ClassName);
			}
		}
		else
		{
			ErrorMessage.LogError("RemoveCollectable : couldn't find class name " + ClassName);
		}
	}

	public void RemoveCollectable(BaseClass NewCollectable)
	{
		if (NewCollectable.m_TypeIdentifier == ObjectType.FarmerPlayer)
		{
			m_Players.Remove(NewCollectable);
		}
		foreach (KeyValuePair<string, Collection> @object in m_Objects)
		{
			@object.Value.m_ObjectsDictionary.Remove(NewCollectable);
		}
	}

	public Dictionary<BaseClass, int> GetCollection(string ClassName)
	{
		if (m_Objects.TryGetValue(ClassName, out var value))
		{
			return value.m_ObjectsDictionary;
		}
		return null;
	}

	public List<BaseClass> GetPlayers()
	{
		return m_Players;
	}

	public bool GetAnyInBuildingCollectionActive(string ClassName)
	{
		Dictionary<BaseClass, int> collection = Instance.GetCollection(ClassName);
		if (collection != null && collection.Count != 0)
		{
			foreach (KeyValuePair<BaseClass, int> item in collection)
			{
				if (!item.Key.GetComponent<Building>().m_Blueprint)
				{
					return true;
				}
			}
		}
		return false;
	}

	public BaseClass GetFirstActiveBuilding(string ClassName)
	{
		Dictionary<BaseClass, int> collection = Instance.GetCollection(ClassName);
		if (collection != null && collection.Count != 0)
		{
			foreach (KeyValuePair<BaseClass, int> item in collection)
			{
				if (!item.Key.GetComponent<Building>().m_Blueprint)
				{
					return item.Key;
				}
			}
		}
		return null;
	}
}
