using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;

public class DespawnManager : MonoBehaviour
{
	public static DespawnManager Instance;

	public float m_LifeTime = 180f;

	private Dictionary<Holdable, float> m_Objects;

	private int[] m_ObjectCounts;

	private void Awake()
	{
		Instance = this;
		m_Objects = new Dictionary<Holdable, float>();
		m_ObjectCounts = new int[(int)ObjectTypeList.m_Total];
	}

	public void Save(JSONNode Node)
	{
		JSONArray jSONArray = (JSONArray)(Node["Objects"] = new JSONArray());
		int num = 0;
		foreach (KeyValuePair<Holdable, float> @object in m_Objects)
		{
			jSONArray[num] = @object.Key.m_UniqueID;
			num++;
		}
	}

	public void Load(JSONNode Node)
	{
		m_Objects.Clear();
		JSONArray asArray = Node["Objects"].AsArray;
		for (int i = 0; i < asArray.Count; i++)
		{
			int num = asArray[i];
			if (num != -1)
			{
				BaseClass objectFromUniqueID = ObjectTypeList.Instance.GetObjectFromUniqueID(num, ErrorCheck: false);
				if (objectFromUniqueID == null)
				{
					Debug.Log("DespawnManager.Load : Couldn't find object with UID " + num);
				}
				else if ((bool)objectFromUniqueID.GetComponent<Holdable>())
				{
					Add(objectFromUniqueID.GetComponent<Holdable>());
				}
			}
		}
	}

	public void Add(Holdable NewObject)
	{
		if (NewObject.m_UniqueID != -1 && NewObject.m_TypeIdentifier != ObjectType.Folk && NewObject.m_TypeIdentifier != ObjectType.Rock && NewObject.m_TypeIdentifier != ObjectType.Clay && NewObject.m_TypeIdentifier != ObjectType.IronOre && NewObject.m_TypeIdentifier != ObjectType.Coal && !Animal.GetIsTypeAnimalDespawn(NewObject.m_TypeIdentifier) && !m_Objects.ContainsKey(NewObject))
		{
			m_ObjectCounts[(int)NewObject.m_TypeIdentifier]++;
			m_Objects.Add(NewObject, 0f);
		}
	}

	public void Remove(Holdable NewObject)
	{
		if (NewObject.m_TypeIdentifier != ObjectType.Folk && m_Objects.ContainsKey(NewObject))
		{
			m_Objects.Remove(NewObject);
			m_ObjectCounts[(int)NewObject.m_TypeIdentifier]--;
		}
	}

	private void Update()
	{
		List<Holdable> list = new List<Holdable>();
		int[] array = new int[(int)ObjectTypeList.m_Total];
		foreach (KeyValuePair<Holdable, float> @object in m_Objects)
		{
			Holdable key = @object.Key;
			if (m_ObjectCounts[(int)key.m_TypeIdentifier] - array[(int)key.m_TypeIdentifier] >= 20)
			{
				key.m_LifeTimer += TimeManager.Instance.m_NormalDelta;
				float num = m_LifeTime;
				if (key.m_TypeIdentifier == ObjectType.Manure)
				{
					num = 60f;
				}
				if (key.m_LifeTimer > num)
				{
					array[(int)key.m_TypeIdentifier]++;
					list.Add(key);
				}
			}
		}
		foreach (Holdable item in list)
		{
			item.StopUsing();
		}
	}
}
