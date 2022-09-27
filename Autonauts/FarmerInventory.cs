using SimpleJSON;
using UnityEngine;

public class FarmerInventory : MonoBehaviour
{
	[HideInInspector]
	public Holdable[] m_Objects;

	[HideInInspector]
	public int m_Capacity;

	[HideInInspector]
	public int m_ExtraCapacity;

	[HideInInspector]
	public int m_TotalCapacity;

	private Farmer m_Farmer;

	[HideInInspector]
	public int m_SlotsHidden;

	[HideInInspector]
	public int m_TotalWeight;

	private int m_SlotsLeft;

	private void Awake()
	{
		SetCapacity(10);
		SetExtraCapacity(0);
		m_Farmer = GetComponent<Farmer>();
	}

	public void StopUsing(bool AndDestroy = true)
	{
		for (int i = 0; i < m_Objects.Length; i++)
		{
			if ((bool)m_Objects[i])
			{
				m_Objects[i].StopUsing();
			}
		}
	}

	private void UpdateCapacity()
	{
		int num = m_Capacity + m_ExtraCapacity;
		if (m_Objects == null)
		{
			m_Objects = new Holdable[num];
			for (int i = 0; i < num; i++)
			{
				m_Objects[i] = null;
			}
		}
		else
		{
			Holdable[] objects = m_Objects;
			m_Objects = new Holdable[num];
			for (int j = 0; j < num; j++)
			{
				m_Objects[j] = null;
				if (objects.Length > j)
				{
					m_Objects[j] = objects[j];
				}
			}
			for (int k = m_Objects.Length; k < objects.Length; k++)
			{
				if ((bool)objects[k])
				{
					objects[k].SendAction(new ActionInfo(ActionType.Dropped, default(TileCoord), m_Farmer));
					objects[k].gameObject.SetActive(value: true);
					objects[k].UpdatePositionToTilePosition(m_Farmer.m_TileCoord);
				}
			}
		}
		m_TotalCapacity = num;
		UpdateAppliedObjects();
	}

	public void SetCapacity(int Capacity)
	{
		m_Capacity = Capacity;
		UpdateCapacity();
	}

	public void SetExtraCapacity(int ExtraCapacity)
	{
		m_ExtraCapacity = ExtraCapacity;
		UpdateCapacity();
	}

	public void PreRefreshPlayerModel()
	{
		for (int i = 0; i < m_Objects.Length; i++)
		{
			if ((bool)m_Objects[i] && GameStateManager.Instance.GetActualState() == GameStateManager.State.DragInventorySlot)
			{
				GameStateManager.Instance.GetCurrentState().GetComponent<GameStateDragInventorySlot>().CheckObjectDropped(m_Objects[i]);
			}
		}
	}

	public void Save(JSONNode Node)
	{
		JSONArray jSONArray = (JSONArray)(Node["InvObjects"] = new JSONArray());
		for (int i = 0; i < m_Objects.Length; i++)
		{
			JSONNode jSONNode2 = new JSONObject();
			if ((bool)m_Objects[i])
			{
				string saveNameFromIdentifier = ObjectTypeList.Instance.GetSaveNameFromIdentifier(m_Objects[i].GetComponent<BaseClass>().m_TypeIdentifier);
				JSONUtils.Set(jSONNode2, "ID", saveNameFromIdentifier);
				m_Objects[i].GetComponent<Savable>().Save(jSONNode2);
			}
			else
			{
				JSONUtils.Set(jSONNode2, "ID", "");
			}
			jSONArray[i] = jSONNode2;
		}
	}

	public void Load(JSONNode Node)
	{
		JSONArray asArray = Node["InvObjects"].AsArray;
		int count = asArray.Count;
		int num = 0;
		for (int i = 0; i < count; i++)
		{
			JSONNode asObject = asArray[i].AsObject;
			if (!(JSONUtils.GetAsString(asObject, "ID", "") != ""))
			{
				continue;
			}
			int asInt = JSONUtils.GetAsInt(asObject, "UID", -1);
			BaseClass objectFromUniqueID = ObjectTypeList.Instance.GetObjectFromUniqueID(asInt, ErrorCheck: false);
			if (objectFromUniqueID != null && objectFromUniqueID.GetComponent<TileCoordObject>().m_Plot != null)
			{
				objectFromUniqueID.StopUsing();
			}
			if (!(ObjectTypeList.Instance.GetObjectFromUniqueID(asInt, ErrorCheck: false) == null))
			{
				continue;
			}
			BaseClass baseClass = ObjectTypeList.Instance.CreateObjectFromSaveName(asObject, new Vector3(0f, 0f, 0f), Quaternion.identity);
			if ((bool)baseClass)
			{
				baseClass.GetComponent<Savable>().Load(asObject);
				baseClass.GetComponent<Holdable>().UpdatePositionToTilePosition(m_Farmer.m_TileCoord);
				if (GetEmptySlot(baseClass.GetComponent<Holdable>()) != -1)
				{
					Add(baseClass.GetComponent<Holdable>(), num);
					num++;
				}
			}
		}
	}

	public int GetEmptySlot(Holdable NewObject)
	{
		if (m_SlotsLeft < NewObject.m_Weight)
		{
			return -1;
		}
		for (int i = 0; i < m_Objects.Length; i++)
		{
			if (!m_Objects[i])
			{
				return i;
			}
		}
		return -1;
	}

	private void UpdateAppliedObjects()
	{
		bool flag = false;
		do
		{
			flag = false;
			for (int i = 0; i < m_Objects.Length - 1; i++)
			{
				if (m_Objects[i] == null && m_Objects[i + 1] != null)
				{
					for (int j = i; j < m_Objects.Length - 1; j++)
					{
						m_Objects[j] = m_Objects[j + 1];
					}
					m_Objects[m_Objects.Length - 1] = null;
					flag = true;
					break;
				}
			}
		}
		while (flag);
		m_TotalWeight = 0;
		int num = 0;
		for (int k = 0; k < m_Objects.Length; k++)
		{
			if ((bool)m_Objects[k])
			{
				num++;
				m_TotalWeight += m_Objects[k].m_Weight;
			}
		}
		m_SlotsLeft = m_Objects.Length - m_TotalWeight;
		m_SlotsHidden = m_TotalWeight - num;
		if ((bool)GameStateManager.Instance && (bool)GameStateManager.Instance.GetCurrentState().GetComponent<GameStateInventory>())
		{
			GameStateManager.Instance.GetCurrentState().GetComponent<GameStateInventory>().CheckTargetUpdated(m_Farmer);
		}
	}

	private void Add(Holdable CarryObject, int Index)
	{
		BaseClass.TestObject(CarryObject);
		m_Objects[Index] = CarryObject;
		CarryObject.SendAction(new ActionInfo(ActionType.BeingHeld, default(TileCoord), m_Farmer));
		CarryObject.SendAction(new ActionInfo(ActionType.Stowed, default(TileCoord), m_Farmer));
		CarryObject.transform.SetParent(m_Farmer.transform);
		CarryObject.gameObject.SetActive(value: false);
		UpdateAppliedObjects();
	}

	public bool IsObjectTypeAcceptable(Holdable CarryObject)
	{
		if ((bool)CarryObject.GetComponent<Worker>())
		{
			return false;
		}
		if ((bool)CarryObject.GetComponent<Folk>())
		{
			return false;
		}
		if ((bool)CarryObject.GetComponent<Animal>())
		{
			return false;
		}
		return true;
	}

	public bool CanAdd(Holdable CarryObject)
	{
		if (GetEmptySlot(CarryObject) == -1)
		{
			return false;
		}
		if (!IsObjectTypeAcceptable(CarryObject))
		{
			return false;
		}
		return true;
	}

	public bool AttemptAdd(Holdable CarryObject)
	{
		if (CanAdd(CarryObject))
		{
			QuestManager.Instance.AddEvent(QuestEvent.Type.Stow, m_Farmer.m_TypeIdentifier == ObjectType.Worker, 0, m_Farmer);
			int emptySlot = GetEmptySlot(CarryObject);
			if (emptySlot != -1)
			{
				Add(CarryObject, emptySlot);
				return true;
			}
		}
		return false;
	}

	private void Add(ObjectType IdentifierType)
	{
		BaseClass baseClass = ObjectTypeList.Instance.CreateObjectFromIdentifier(IdentifierType, new Vector3(0f, 0f, 0f), Quaternion.identity);
		baseClass.GetComponent<Savable>().SetIsSavable(IsSavable: false);
		AttemptAdd(baseClass.GetComponent<Holdable>());
	}

	private void ObjectRemoved(Holdable NewObject)
	{
		if ((bool)GameStateManager.Instance.GetCurrentState().GetComponent<GameStateDragInventorySlot>())
		{
			GameStateManager.Instance.GetCurrentState().GetComponent<GameStateDragInventorySlot>().CheckObjectDropped(NewObject);
		}
	}

	public Holdable GetLastObject()
	{
		for (int num = m_Objects.Length - 1; num >= 0; num--)
		{
			if ((bool)m_Objects[num])
			{
				return m_Objects[num];
			}
		}
		return null;
	}

	public Holdable ReleaseObject(ObjectType Identifier)
	{
		for (int i = 0; i < m_Objects.Length; i++)
		{
			if ((bool)m_Objects[i] && m_Objects[i].m_TypeIdentifier == Identifier)
			{
				Holdable obj = m_Objects[i];
				obj.SendAction(new ActionInfo(ActionType.Recalled, default(TileCoord), m_Farmer));
				obj.gameObject.SetActive(value: true);
				ObjectRemoved(m_Objects[i]);
				m_Objects[i] = null;
				UpdateAppliedObjects();
				QuestManager.Instance.AddEvent(QuestEvent.Type.Recall, m_Farmer.m_TypeIdentifier == ObjectType.Worker, 0, m_Farmer);
				return obj;
			}
		}
		return null;
	}

	public Holdable ReleaseObject(int Index)
	{
		Holdable obj = m_Objects[Index];
		ObjectRemoved(m_Objects[Index]);
		m_Objects[Index] = null;
		UpdateAppliedObjects();
		obj.SendAction(new ActionInfo(ActionType.Recalled, default(TileCoord), m_Farmer));
		obj.gameObject.SetActive(value: true);
		QuestManager.Instance.AddEvent(QuestEvent.Type.Recall, m_Farmer.m_TypeIdentifier == ObjectType.Worker, 0, m_Farmer);
		return obj;
	}

	public Holdable GetObject(bool Top)
	{
		if (Top)
		{
			for (int num = m_Objects.Length - 1; num >= 0; num--)
			{
				if (m_Objects[num] != null)
				{
					return m_Objects[num];
				}
			}
		}
		else
		{
			for (int i = 0; i < m_Objects.Length; i++)
			{
				if (m_Objects[i] != null)
				{
					return m_Objects[i];
				}
			}
		}
		return null;
	}

	public Holdable PopObject(bool Top)
	{
		Holdable holdable = null;
		if (Top)
		{
			for (int num = m_Objects.Length - 1; num >= 0; num--)
			{
				if (m_Objects[num] != null)
				{
					holdable = m_Objects[num];
					ObjectRemoved(m_Objects[num]);
					m_Objects[num] = null;
					break;
				}
			}
		}
		else
		{
			for (int i = 0; i < m_Objects.Length; i++)
			{
				if (m_Objects[i] != null)
				{
					holdable = m_Objects[i];
					ObjectRemoved(m_Objects[i]);
					m_Objects[i] = null;
					break;
				}
			}
		}
		if ((bool)holdable)
		{
			UpdateAppliedObjects();
			holdable.SendAction(new ActionInfo(ActionType.Recalled, default(TileCoord), m_Farmer));
			holdable.gameObject.SetActive(value: true);
			QuestManager.Instance.AddEvent(QuestEvent.Type.Recall, m_Farmer.m_TypeIdentifier == ObjectType.Worker, 0, m_Farmer);
		}
		return holdable;
	}

	public void PushObject(Holdable NewObject, bool Top)
	{
		NewObject.SendAction(new ActionInfo(ActionType.BeingHeld, default(TileCoord), m_Farmer));
		NewObject.SendAction(new ActionInfo(ActionType.Stowed, default(TileCoord), m_Farmer));
		NewObject.gameObject.SetActive(value: false);
		if (Top)
		{
			int num = m_Objects.Length - 1;
			while (num >= 0 && !(m_Objects[num] == null))
			{
				num--;
			}
			for (int i = num; i < m_Objects.Length - 1; i++)
			{
				m_Objects[i] = m_Objects[i + 1];
			}
			m_Objects[m_Objects.Length - 1] = NewObject;
		}
		else
		{
			int j;
			for (j = 0; j < m_Objects.Length - 1 && !(m_Objects[j] == null); j++)
			{
			}
			for (int num2 = j; num2 >= 1; num2--)
			{
				m_Objects[num2] = m_Objects[num2 - 1];
			}
			m_Objects[0] = NewObject;
		}
		UpdateAppliedObjects();
	}

	public int GetFreeSlots()
	{
		return m_TotalCapacity - GetFullSlots();
	}

	public int GetFullSlots()
	{
		int num = 0;
		for (int i = 0; i < m_Objects.Length; i++)
		{
			if (m_Objects[i] != null)
			{
				num += m_Objects[i].m_Weight;
			}
		}
		return num;
	}

	public bool GetAnyInventory()
	{
		if (GetFreeSlots() == m_Objects.Length)
		{
			return false;
		}
		return true;
	}

	public bool ContainsObjectType(ObjectType NewType)
	{
		for (int i = 0; i < m_Objects.Length; i++)
		{
			if (m_Objects[i] != null && m_Objects[i].m_TypeIdentifier == NewType)
			{
				return true;
			}
		}
		return false;
	}

	public Holdable GetObjectType(ObjectType NewType)
	{
		for (int i = 0; i < m_Objects.Length; i++)
		{
			if (m_Objects[i] != null && m_Objects[i].m_TypeIdentifier == NewType)
			{
				return m_Objects[i];
			}
		}
		return null;
	}

	public void DropAllObjects()
	{
		for (int i = 0; i < m_Objects.Length; i++)
		{
			if (m_Objects[i] != null)
			{
				m_Objects[i].SendAction(new ActionInfo(ActionType.Dropped, m_Farmer.m_TileCoord, m_Farmer));
				m_Objects[i].gameObject.SetActive(value: true);
				ObjectRemoved(m_Objects[i]);
				m_Objects[i] = null;
			}
		}
		UpdateAppliedObjects();
	}

	public bool GetContainsHeavyForPlayer()
	{
		GetComponent<FarmerPlayer>();
		for (int i = 0; i < m_Objects.Length; i++)
		{
			if (m_Objects[i] != null && m_Objects[i].GetIsHeavyForPlayer())
			{
				return true;
			}
		}
		return false;
	}

	public bool GetContainsVeryHeavyForPlayer()
	{
		GetComponent<FarmerPlayer>();
		for (int i = 0; i < m_Objects.Length; i++)
		{
			if (m_Objects[i] != null && m_Objects[i].GetIsVeryHeavyForPlayer())
			{
				return true;
			}
		}
		return false;
	}
}
