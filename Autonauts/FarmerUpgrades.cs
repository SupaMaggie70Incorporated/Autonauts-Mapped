using SimpleJSON;
using UnityEngine;

public class FarmerUpgrades : MonoBehaviour
{
	[HideInInspector]
	public Holdable[] m_Objects;

	[HideInInspector]
	public int m_Capacity;

	private Farmer m_Farmer;

	private GameObject m_UpgradeInventoryRoot;

	private void Awake()
	{
		SetCapacity(4);
		m_Farmer = GetComponent<Farmer>();
	}

	public void Restart()
	{
		ModelUpdated();
	}

	public void ModelUpdated()
	{
		Transform transform = ObjectUtils.FindDeepChild(base.transform, "BackpackPoint");
		if (transform != null)
		{
			m_UpgradeInventoryRoot = transform.gameObject;
			m_UpgradeInventoryRoot.transform.localRotation = Quaternion.identity;
		}
		else
		{
			m_UpgradeInventoryRoot = null;
		}
		UpdateScales();
		RefreshPlayerModel();
	}

	public void UpdateScales()
	{
		if ((bool)m_UpgradeInventoryRoot)
		{
			Vector3 localScale = m_UpgradeInventoryRoot.transform.parent.localScale;
			localScale.x = 1f / localScale.x;
			localScale.y = 1f / localScale.y;
			localScale.z = 1f / localScale.z;
			m_UpgradeInventoryRoot.transform.localScale = localScale;
		}
	}

	public void StopUsing(bool AndDestroy = true)
	{
		for (int i = 0; i < m_Capacity; i++)
		{
			if ((bool)m_Objects[i])
			{
				m_Objects[i].StopUsing();
			}
		}
	}

	public void SetCapacity(int Capacity)
	{
		m_Capacity = Capacity;
		Holdable[] objects = m_Objects;
		m_Objects = new Holdable[m_Capacity];
		for (int i = 0; i < m_Capacity; i++)
		{
			m_Objects[i] = null;
		}
		if (objects == null)
		{
			return;
		}
		for (int j = 0; j < objects.Length; j++)
		{
			if ((bool)objects[j])
			{
				if (j < m_Capacity)
				{
					m_Objects[j] = objects[j];
					continue;
				}
				objects[j].SendAction(new ActionInfo(ActionType.Dropped, default(TileCoord), m_Farmer));
				objects[j].gameObject.SetActive(value: true);
				objects[j].UpdatePositionToTilePosition(m_Farmer.m_TileCoord);
			}
		}
	}

	public void Save(JSONNode Node)
	{
		JSONArray jSONArray = (JSONArray)(Node["UpgradeObjects"] = new JSONArray());
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
		JSONArray asArray = Node["UpgradeObjects"].AsArray;
		int num = asArray.Count;
		if (num > m_Objects.Length)
		{
			num = m_Objects.Length;
		}
		for (int i = 0; i < num; i++)
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
			if (ObjectTypeList.Instance.GetObjectFromUniqueID(asInt, ErrorCheck: false) == null)
			{
				BaseClass baseClass = ObjectTypeList.Instance.CreateObjectFromSaveName(asObject, new Vector3(0f, 0f, 0f), Quaternion.identity);
				if ((bool)baseClass)
				{
					baseClass.GetComponent<Savable>().Load(asObject);
					Add(baseClass.GetComponent<Holdable>(), i);
				}
			}
		}
	}

	public void PreRefreshPlayerModel()
	{
		for (int i = 0; i < m_Capacity; i++)
		{
			if ((bool)m_Objects[i])
			{
				if (GameStateManager.Instance.GetActualState() == GameStateManager.State.DragInventorySlot)
				{
					GameStateManager.Instance.GetCurrentState().GetComponent<GameStateDragInventorySlot>().CheckObjectDropped(m_Objects[i]);
				}
				m_Objects[i].transform.SetParent(null);
			}
		}
	}

	public void RefreshPlayerModel()
	{
		for (int i = 0; i < m_Capacity; i++)
		{
			if ((bool)m_Objects[i])
			{
				m_Objects[i].transform.SetParent(m_UpgradeInventoryRoot.transform);
			}
		}
	}

	public int GetEmptySlot()
	{
		for (int i = 0; i < m_Capacity; i++)
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
		bool flag2 = false;
		bool flag3 = false;
		bool flag4 = false;
		bool flag5 = false;
		bool flag6 = false;
		for (int i = 0; i < m_Capacity; i++)
		{
			if ((bool)m_Objects[i])
			{
				switch (m_Objects[i].GetComponent<Upgrade>().m_Type)
				{
				case Upgrade.Type.PlayerInventory:
				case Upgrade.Type.WorkerInventory:
				{
					int capacity = m_Objects[i].GetComponent<UpgradeInventory>().m_Capacity;
					m_Farmer.m_FarmerInventory.SetExtraCapacity(capacity);
					flag = true;
					break;
				}
				case Upgrade.Type.WorkerMemory:
				{
					int size = m_Objects[i].GetComponent<UpgradeWorkerMemory>().m_Size;
					m_Farmer.GetComponent<Worker>().SetExtraMemory(size);
					flag3 = true;
					break;
				}
				case Upgrade.Type.WorkerSearch:
				{
					int range = m_Objects[i].GetComponent<UpgradeWorkerSearch>().m_Range;
					int initialDelay = m_Objects[i].GetComponent<UpgradeWorkerSearch>().m_InitialDelay;
					m_Farmer.GetComponent<Worker>().SetExtraSearch(range, initialDelay);
					flag4 = true;
					break;
				}
				case Upgrade.Type.WorkerCarry:
				{
					int capacity = m_Objects[i].GetComponent<UpgradeWorkerCarry>().m_Capacity;
					m_Farmer.m_FarmerCarry.SetExtraCarry(capacity);
					flag2 = true;
					break;
				}
				case Upgrade.Type.WorkerMovement:
				{
					int initialDelay = m_Objects[i].GetComponent<UpgradeWorkerMovement>().m_InitialDelay;
					float moveScale = m_Objects[i].GetComponent<UpgradeWorkerMovement>().m_MoveScale;
					m_Farmer.GetComponent<Worker>().SetExtraMovement(initialDelay, moveScale);
					flag5 = true;
					break;
				}
				case Upgrade.Type.WorkerEnergy:
				{
					float energy = m_Objects[i].GetComponent<UpgradeWorkerEnergy>().m_Energy;
					m_Farmer.GetComponent<Worker>().SetExtraEnergy(energy);
					flag6 = true;
					break;
				}
				}
			}
		}
		if (!flag)
		{
			m_Farmer.m_FarmerInventory.SetExtraCapacity(0);
		}
		if (!flag2)
		{
			m_Farmer.m_FarmerCarry.SetExtraCarry(0);
		}
		Worker component = m_Farmer.GetComponent<Worker>();
		if ((bool)component)
		{
			if (!flag3)
			{
				component.SetExtraMemory(0);
			}
			if (!flag4)
			{
				component.SetExtraSearch(0, 0);
			}
			if (!flag5)
			{
				component.SetExtraMovement(0, 0f);
			}
			if (!flag6)
			{
				component.SetExtraEnergy(0f);
			}
		}
		if ((bool)GameStateManager.Instance.GetCurrentState().GetComponent<GameStateInventory>())
		{
			GameStateManager.Instance.GetCurrentState().GetComponent<GameStateInventory>().CheckTargetUpdated(m_Farmer);
		}
	}

	private void Add(Holdable CarryObject, int Index)
	{
		BaseClass.TestObject(CarryObject);
		m_Objects[Index] = CarryObject;
		if ((bool)CarryObject.GetComponent<UpgradeInventory>())
		{
			CarryObject.transform.SetParent(m_UpgradeInventoryRoot.transform);
			CarryObject.transform.localPosition = default(Vector3);
			CarryObject.SendAction(new ActionInfo(ActionType.BeingHeld, default(TileCoord), m_Farmer));
		}
		else
		{
			CarryObject.SendAction(new ActionInfo(ActionType.BeingHeld, default(TileCoord), m_Farmer));
			CarryObject.SendAction(new ActionInfo(ActionType.Stowed, default(TileCoord), m_Farmer));
			CarryObject.gameObject.SetActive(value: false);
		}
		UpdateAppliedObjects();
	}

	private bool IsObjectTypeAcceptable(Holdable CarryObject)
	{
		Upgrade component = CarryObject.GetComponent<Upgrade>();
		if (!component)
		{
			return false;
		}
		if (m_Farmer.m_TypeIdentifier == ObjectType.FarmerPlayer && component.m_Target == Upgrade.Target.Bot)
		{
			return false;
		}
		if (m_Farmer.m_TypeIdentifier == ObjectType.Worker && component.m_Target == Upgrade.Target.Player)
		{
			return false;
		}
		Upgrade.Type type = component.m_Type;
		for (int i = 0; i < m_Capacity; i++)
		{
			if ((bool)m_Objects[i] && m_Objects[i].GetComponent<Upgrade>().m_Type == type)
			{
				if (m_Farmer.m_TypeIdentifier == ObjectType.FarmerPlayer)
				{
					return false;
				}
				int level = m_Objects[i].GetComponent<Upgrade>().m_Level;
				if (component.m_Level <= level)
				{
					return false;
				}
				return true;
			}
		}
		if (GetEmptySlot() == -1)
		{
			return false;
		}
		return true;
	}

	public void RemoveSameType(Holdable CarryObject)
	{
		Upgrade.Type type = CarryObject.GetComponent<Upgrade>().m_Type;
		for (int i = 0; i < m_Capacity; i++)
		{
			if ((bool)m_Objects[i] && m_Objects[i].GetComponent<Upgrade>().m_Type == type)
			{
				Holdable holdable = m_Objects[i];
				holdable.SendAction(new ActionInfo(ActionType.Dropped, m_Farmer.m_TileCoord, m_Farmer));
				holdable.gameObject.SetActive(value: true);
				m_Objects[i] = null;
				TileCoord tileCoord = m_Farmer.m_TileCoord;
				TileCoord randomEmptyTile = TileHelpers.GetRandomEmptyTile(tileCoord);
				float y = randomEmptyTile.ToWorldPositionTileCentered().y;
				SpawnAnimationManager.Instance.AddJump(holdable, tileCoord, randomEmptyTile, m_Farmer.transform.position.y, y, 4f);
			}
		}
	}

	public bool CanAdd(Holdable CarryObject)
	{
		if (!IsObjectTypeAcceptable(CarryObject))
		{
			return false;
		}
		return true;
	}

	public bool WillAdd(Holdable CarryObject)
	{
		if (CanAdd(CarryObject) && GetEmptySlot() != -1)
		{
			return true;
		}
		return false;
	}

	public bool AttemptAdd(Holdable CarryObject)
	{
		RemoveSameType(CarryObject);
		if (CanAdd(CarryObject))
		{
			int emptySlot = GetEmptySlot();
			if (emptySlot != -1)
			{
				Add(CarryObject, emptySlot);
				if (Upgrade.GetIsTypeWorkerUpgrade(CarryObject.m_TypeIdentifier))
				{
					QuestManager.Instance.AddEvent(QuestEvent.Type.UpgradeBot, m_Farmer.m_TypeIdentifier == ObjectType.Worker, 0, m_Farmer);
				}
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

	public Holdable ReleaseObject(ObjectType Identifier)
	{
		for (int i = 0; i < m_Capacity; i++)
		{
			if ((bool)m_Objects[i] && m_Objects[i].m_TypeIdentifier == Identifier)
			{
				Holdable obj = m_Objects[i];
				obj.SendAction(new ActionInfo(ActionType.Recalled, default(TileCoord), m_Farmer));
				obj.gameObject.SetActive(value: true);
				m_Objects[i] = null;
				UpdateAppliedObjects();
				return obj;
			}
		}
		return null;
	}

	public Holdable ReleaseObject(int Index)
	{
		Holdable obj = m_Objects[Index];
		m_Objects[Index] = null;
		UpdateAppliedObjects();
		obj.SendAction(new ActionInfo(ActionType.Recalled, default(TileCoord), m_Farmer));
		obj.gameObject.SetActive(value: true);
		return obj;
	}

	public Holdable PopObject(bool Top)
	{
		Holdable holdable = null;
		if (Top)
		{
			for (int num = m_Capacity - 1; num >= 0; num--)
			{
				if (m_Objects[num] != null)
				{
					holdable = m_Objects[num];
					m_Objects[num] = null;
					break;
				}
			}
		}
		else
		{
			for (int i = 0; i < m_Capacity; i++)
			{
				if (m_Objects[i] != null)
				{
					holdable = m_Objects[i];
					m_Objects[i] = null;
					break;
				}
			}
		}
		UpdateAppliedObjects();
		holdable.SendAction(new ActionInfo(ActionType.Recalled, default(TileCoord), m_Farmer));
		holdable.gameObject.SetActive(value: true);
		return holdable;
	}

	public void PushObject(Holdable NewObject, bool Top)
	{
		NewObject.SendAction(new ActionInfo(ActionType.BeingHeld, default(TileCoord), m_Farmer));
		NewObject.SendAction(new ActionInfo(ActionType.Stowed, default(TileCoord), m_Farmer));
		NewObject.gameObject.SetActive(value: false);
		if (Top)
		{
			int num = m_Capacity - 1;
			while (num >= 0 && !(m_Objects[num] == null))
			{
				num--;
			}
			for (int i = num; i < m_Capacity - 1; i++)
			{
				m_Objects[i] = m_Objects[i + 1];
			}
			m_Objects[m_Capacity - 1] = NewObject;
		}
		else
		{
			int j;
			for (j = 0; j < m_Capacity - 1 && !(m_Objects[j] == null); j++)
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
		int num = 0;
		for (int i = 0; i < m_Objects.Length; i++)
		{
			if (m_Objects[i] == null)
			{
				num++;
			}
		}
		return num;
	}

	public bool GetAnyInventory()
	{
		if (GetFreeSlots() == m_Capacity)
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
				m_Objects[i] = null;
			}
		}
		UpdateAppliedObjects();
	}

	public Holdable GetWhistle()
	{
		for (int i = 0; i < m_Objects.Length; i++)
		{
			if (m_Objects[i] != null && (bool)m_Objects[i].GetComponent<UpgradePlayerWhistle>())
			{
				return m_Objects[i];
			}
		}
		return null;
	}

	public bool GetContainsUpgradePlayerMovement()
	{
		if (ContainsObjectType(ObjectType.UpgradePlayerMovementCrude) || ContainsObjectType(ObjectType.UpgradePlayerMovementGood) || ContainsObjectType(ObjectType.UpgradePlayerMovementSuper))
		{
			return true;
		}
		return false;
	}

	public ObjectType GetMovementUpgrade()
	{
		GetComponent<FarmerPlayer>();
		for (int i = 0; i < m_Objects.Length; i++)
		{
			if ((bool)m_Objects[i] && UpgradePlayerMovement.GetIsTypeUpgradePlayerMovement(m_Objects[i].m_TypeIdentifier))
			{
				return m_Objects[i].m_TypeIdentifier;
			}
		}
		return ObjectTypeList.m_Total;
	}
}
