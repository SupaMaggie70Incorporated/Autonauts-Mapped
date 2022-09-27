using System.Collections.Generic;
using UnityEngine;

public class ObjectCountManager : MonoBehaviour
{
	public static ObjectCountManager Instance;

	private Dictionary<ObjectType, int> m_ObjectTypeCounts;

	private void Awake()
	{
		Instance = this;
		m_ObjectTypeCounts = new Dictionary<ObjectType, int>();
	}

	private ObjectType GetType(BaseClass NewObject)
	{
		if ((bool)NewObject.GetComponent<Storage>())
		{
			return NewObject.GetComponent<Storage>().m_ObjectType;
		}
		return NewObject.m_TypeIdentifier;
	}

	private void RegisterType(ObjectType NewType)
	{
		if (!m_ObjectTypeCounts.ContainsKey(NewType))
		{
			m_ObjectTypeCounts.Add(NewType, 0);
		}
	}

	public int RegisterNewObject(BaseClass NewObject)
	{
		ObjectType type = GetType(NewObject);
		RegisterType(type);
		m_ObjectTypeCounts[type]++;
		return m_ObjectTypeCounts[type];
	}

	public void RegisterOldObject(BaseClass NewObject, int Index)
	{
		ObjectType type = GetType(NewObject);
		RegisterType(type);
		if (m_ObjectTypeCounts[type] < Index)
		{
			m_ObjectTypeCounts[type] = Index;
		}
	}

	public void DeregisterObject(BaseClass NewObject, int Index)
	{
		ObjectType type = GetType(NewObject);
		if (m_ObjectTypeCounts[type] == Index)
		{
			m_ObjectTypeCounts[type]--;
		}
	}
}
