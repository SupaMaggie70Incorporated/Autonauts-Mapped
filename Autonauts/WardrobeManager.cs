using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;

public class WardrobeManager : MonoBehaviour
{
	public static WardrobeManager Instance;

	public int m_Capacity;

	private List<Holdable> m_Objects;

	private void Awake()
	{
		Instance = this;
		SetupCapacity();
		m_Objects = new List<Holdable>();
	}

	private void SetupCapacity()
	{
		m_Capacity = 0;
		for (int i = 0; i < (int)ObjectTypeList.m_Total; i++)
		{
			if (Clothing.GetIsTypeClothing((ObjectType)i))
			{
				m_Capacity++;
			}
		}
	}

	public void Save(JSONNode Node)
	{
		JSONArray jSONArray = (JSONArray)(Node["Wardrobe"] = new JSONArray());
		for (int i = 0; i < m_Objects.Count; i++)
		{
			JSONNode jSONNode2 = new JSONObject();
			string saveNameFromIdentifier = ObjectTypeList.Instance.GetSaveNameFromIdentifier(m_Objects[i].GetComponent<BaseClass>().m_TypeIdentifier);
			JSONUtils.Set(jSONNode2, "ID", saveNameFromIdentifier);
			m_Objects[i].GetComponent<Savable>().Save(jSONNode2);
			jSONArray[i] = jSONNode2;
		}
	}

	public void Load(JSONNode Node)
	{
		JSONArray asArray = Node["Wardrobe"].AsArray;
		for (int i = 0; i < asArray.Count; i++)
		{
			JSONNode asObject = asArray[i].AsObject;
			BaseClass baseClass = ObjectTypeList.Instance.CreateObjectFromSaveName(asObject, new Vector3(0f, 0f, 0f), Quaternion.identity);
			if ((bool)baseClass)
			{
				baseClass.GetComponent<Savable>().Load(asObject);
				AttemptAdd(baseClass.GetComponent<Holdable>());
			}
		}
	}

	public bool CanAdd(ObjectType NewType)
	{
		if (!Clothing.GetIsTypeClothing(NewType))
		{
			return false;
		}
		if (m_Objects.Count >= m_Capacity)
		{
			return false;
		}
		foreach (Holdable @object in m_Objects)
		{
			if (@object.m_TypeIdentifier == NewType)
			{
				return false;
			}
		}
		return true;
	}

	public bool CanAdd(Holdable NewObject)
	{
		return CanAdd(NewObject.m_TypeIdentifier);
	}

	public Holdable ReleaseObject(int Index)
	{
		if (Index >= m_Objects.Count)
		{
			return null;
		}
		Holdable holdable = m_Objects[Index];
		m_Objects.RemoveAt(Index);
		holdable.SendAction(new ActionInfo(ActionType.Recalled, default(TileCoord)));
		holdable.gameObject.SetActive(value: true);
		return holdable;
	}

	public void AttemptAdd(Holdable NewObject)
	{
		NewObject.SendAction(new ActionInfo(ActionType.BeingHeld, default(TileCoord)));
		NewObject.SendAction(new ActionInfo(ActionType.Stowed, default(TileCoord)));
		NewObject.gameObject.SetActive(value: false);
		m_Objects.Add(NewObject);
	}

	public Holdable GetObject(int Index)
	{
		if (Index >= m_Objects.Count)
		{
			return null;
		}
		return m_Objects[Index];
	}

	public void AddNewObject(ObjectType NewType)
	{
		if (CanAdd(NewType))
		{
			BaseClass baseClass = ObjectTypeList.Instance.CreateObjectFromIdentifier(NewType, base.transform.position, base.transform.localRotation);
			AttemptAdd(baseClass.GetComponent<Holdable>());
		}
	}
}
