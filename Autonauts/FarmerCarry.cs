using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;

public class FarmerCarry : MonoBehaviour
{
	[HideInInspector]
	public List<Holdable> m_CarryObject;

	[HideInInspector]
	public int m_CarrySlotsUsed;

	[HideInInspector]
	public int m_SlotsRemaining;

	[HideInInspector]
	public ObjectType m_LastObjectType;

	[HideInInspector]
	public Holdable m_LastObject;

	[HideInInspector]
	public int m_MaxCarryCount;

	[HideInInspector]
	public int m_ExtraCapacity;

	[HideInInspector]
	public int m_TotalCapacity;

	private GameObject m_CarryRoot;

	private GameObject m_ToolCarryRoot;

	private GameObject m_WorkerCarryRoot;

	[HideInInspector]
	public float m_ToolCarryDistance;

	private Farmer m_Farmer;

	[HideInInspector]
	public GameObject m_ToolModel;

	private Holdable m_LastTempCarry;

	private void Awake()
	{
		m_MaxCarryCount = 6;
		m_ExtraCapacity = 0;
		m_CarryObject = new List<Holdable>();
		m_CarrySlotsUsed = 0;
		m_SlotsRemaining = m_MaxCarryCount;
		m_Farmer = GetComponent<Farmer>();
		m_ToolModel = null;
		m_LastObjectType = ObjectTypeList.m_Total;
		m_LastObject = null;
	}

	public void Restart()
	{
		UpdateCapacity();
	}

	public void GetNodes()
	{
		if (ObjectUtils.FindDeepChild(base.transform, "CarryPoint") == null)
		{
			m_CarryRoot = null;
		}
		m_CarryRoot = ObjectUtils.FindDeepChild(base.transform, "CarryPoint").gameObject;
		m_CarryRoot.transform.localRotation = Quaternion.identity;
		m_ToolCarryRoot = ObjectUtils.FindDeepChild(base.transform, "ToolCarryPoint").gameObject;
		m_ToolCarryRoot.transform.localRotation = Quaternion.identity;
		m_WorkerCarryRoot = ObjectUtils.FindDeepChild(base.transform, "WorkerCarryPoint").gameObject;
		m_WorkerCarryRoot.transform.localRotation = Quaternion.identity;
		m_ToolCarryDistance = 0f - (m_ToolCarryRoot.transform.position - m_Farmer.transform.position).z;
	}

	public void UpdateScales()
	{
		Vector3 localScale = m_ToolCarryRoot.transform.parent.localScale;
		localScale.x = 1f / localScale.x;
		localScale.y = 1f / localScale.y;
		localScale.z = 1f / localScale.z;
		if ((bool)m_CarryRoot)
		{
			m_CarryRoot.transform.localScale = localScale;
		}
		m_ToolCarryRoot.transform.localScale = localScale;
		m_WorkerCarryRoot.transform.localScale = localScale;
	}

	public void PreRefreshPlayerModel()
	{
		foreach (Holdable item in m_CarryObject)
		{
			if (GameStateManager.Instance.GetActualState() == GameStateManager.State.DragInventorySlot)
			{
				GameStateManager.Instance.GetCurrentState().GetComponent<GameStateDragInventorySlot>().CheckObjectDropped(item);
			}
			item.transform.SetParent(null);
		}
	}

	public void RefreshPlayerModel()
	{
		if (m_CarryObject.Count > 0 && MyTool.GetIsTypeSpecialTool(m_CarryObject[0].m_TypeIdentifier))
		{
			UpdateCarryStickRock();
			return;
		}
		foreach (Holdable item in m_CarryObject)
		{
			SetCarryObjectParent(item);
		}
	}

	public void StopUsing(bool AndDestroy = true)
	{
		foreach (Holdable item in m_CarryObject)
		{
			item.StopUsing();
		}
	}

	public void Save(JSONNode Node)
	{
		JSONArray jSONArray = (JSONArray)(Node["CarryObjects"] = new JSONArray());
		int num = 0;
		for (int i = 0; i < m_CarryObject.Count; i++)
		{
			if ((bool)m_CarryObject[i])
			{
				JSONNode jSONNode2 = new JSONObject();
				string saveNameFromIdentifier = ObjectTypeList.Instance.GetSaveNameFromIdentifier(m_CarryObject[i].GetComponent<BaseClass>().m_TypeIdentifier);
				JSONUtils.Set(jSONNode2, "ID", saveNameFromIdentifier);
				m_CarryObject[i].GetComponent<Savable>().Save(jSONNode2);
				jSONArray[num] = jSONNode2;
				num++;
			}
		}
		JSONUtils.Set(Node, "LastObjectType", ObjectTypeList.Instance.GetSaveNameFromIdentifier(m_LastObjectType));
	}

	public void Load(JSONNode Node)
	{
		JSONArray asArray = Node["CarryObjects"].AsArray;
		for (int i = 0; i < asArray.Count; i++)
		{
			JSONNode asObject = asArray[i].AsObject;
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
					AddCarry(baseClass.GetComponent<Holdable>());
				}
			}
		}
		string asString = JSONUtils.GetAsString(Node, "LastObjectType", "");
		m_LastObjectType = ObjectTypeList.Instance.GetIdentifierFromSaveName(asString, Check: false);
		m_LastObject = null;
	}

	public void SendAction(ActionInfo Info)
	{
		if (Info.m_Action == ActionType.Refresh)
		{
			for (int i = 0; i < m_CarryObject.Count; i++)
			{
				m_CarryObject[i].SendAction(Info);
			}
		}
	}

	private void CarryChanged()
	{
		NastyBugFix();
		if (m_CarryObject.Count > 0)
		{
			m_LastObject = m_CarryObject[0];
			m_LastObjectType = m_CarryObject[0].m_TypeIdentifier;
		}
		CheckGameStateInventory();
		if (m_CarryObject.Count > 0 && MyTool.GetIsTypeSpecialTool(m_CarryObject[0].m_TypeIdentifier))
		{
			UpdateCarryStickRock();
		}
		if ((bool)GameStateManager.Instance && (bool)GameStateManager.Instance.GetCurrentState().GetComponent<GameStateInventory>())
		{
			GameStateManager.Instance.GetCurrentState().GetComponent<GameStateInventory>().CheckTargetUpdated(m_Farmer);
		}
		if ((bool)m_Farmer.GetComponent<Worker>())
		{
			m_Farmer.GetComponent<Worker>().CarryChanged();
		}
		if (m_CarryObject.Count > 0)
		{
			if (!m_CarryObject[0].m_AllowMultiple)
			{
				m_CarrySlotsUsed = m_TotalCapacity;
				m_SlotsRemaining = 0;
			}
			else
			{
				m_CarrySlotsUsed = m_CarryObject.Count * m_CarryObject[0].m_Weight;
				m_SlotsRemaining = (m_TotalCapacity - m_CarrySlotsUsed) / m_CarryObject[0].m_Weight;
			}
		}
		else
		{
			m_CarrySlotsUsed = 0;
			m_SlotsRemaining = m_TotalCapacity;
		}
		if (m_Farmer.m_TypeIdentifier == ObjectType.FarmerPlayer)
		{
			m_Farmer.GetComponent<FarmerPlayer>().CheckCarryHeavy();
			if ((bool)HudManager.Instance.m_InventoryBar)
			{
				HudManager.Instance.m_InventoryBar.CheckSlots();
				HudManager.Instance.m_InventoryBar.UpdateObjects();
			}
		}
	}

	private void CheckGameStateInventory()
	{
		if ((bool)GetComponent<FarmerPlayer>() && (bool)GameStateManager.Instance.GetCurrentState().GetComponent<GameStateInventory>())
		{
			GameStateManager.Instance.GetCurrentState().GetComponent<GameStateInventory>().UpdateInventory();
		}
	}

	private void UpdateCarryStickRock()
	{
		if (m_CarryObject.Count == 1)
		{
			Holdable holdable = m_CarryObject[0];
			holdable.transform.SetParent(m_ToolCarryRoot.transform);
			holdable.transform.localPosition = new Vector3(0f, 1f, 0f);
			holdable.transform.localRotation = Quaternion.Euler(0f, 0f, 90f);
			return;
		}
		int num = 0;
		foreach (Holdable item in m_CarryObject)
		{
			float height = ObjectTypeList.Instance.GetHeight(item.m_TypeIdentifier);
			item.transform.SetParent(m_CarryRoot.transform);
			Vector3 localPosition = new Vector3(Random.Range(-0.2f, 0.2f), (float)num * height * 1.1f, Random.Range(-0.2f, 0.2f));
			item.transform.localPosition = localPosition;
			item.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
			num++;
		}
	}

	private void SetCarryObjectParent(Holdable CarryObject)
	{
		if (MyTool.GetIsTypeSpecialTool(CarryObject.m_TypeIdentifier))
		{
			return;
		}
		Vector3 localPosition;
		if ((bool)CarryObject.GetComponent<MyTool>() || (bool)CarryObject.GetComponent<Instrument>())
		{
			if ((bool)CarryObject.GetComponent<ToolBasket>())
			{
				CarryObject.transform.parent = m_CarryRoot.transform;
			}
			else
			{
				CarryObject.transform.parent = m_ToolCarryRoot.transform;
			}
			localPosition = default(Vector3);
		}
		else if ((bool)CarryObject.GetComponent<Animal>() || (bool)CarryObject.GetComponent<Farmer>() || (bool)CarryObject.GetComponent<Folk>() || (bool)CarryObject.GetComponent<Vehicle>() || CarryObject.m_Weight >= m_TotalCapacity)
		{
			CarryObject.transform.parent = m_WorkerCarryRoot.transform;
			localPosition = default(Vector3);
		}
		else
		{
			float height = ObjectTypeList.Instance.GetHeight(CarryObject.m_TypeIdentifier);
			CarryObject.transform.parent = m_CarryRoot.transform;
			int num = m_CarryObject.IndexOf(CarryObject);
			localPosition = new Vector3(Random.Range(-0.2f, 0.2f), (float)num * height * 1.1f, Random.Range(-0.2f, 0.2f));
		}
		CarryObject.transform.localPosition = localPosition;
		CarryObject.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
	}

	private void CheckSpecialAddCarry(Holdable CarryObject)
	{
		if ((bool)CarryObject.GetComponent<MyTool>() || MyTool.GetIsTypeSpecialTool(CarryObject.m_TypeIdentifier))
		{
			m_ToolModel = CarryObject.gameObject;
		}
		if (Sign.GetIsTypeSign(CarryObject.m_TypeIdentifier) && m_Farmer.m_TypeIdentifier == ObjectType.FarmerPlayer)
		{
			GetTopObject().GetComponent<Sign>().SetBeingHeldByPlayer(Held: true);
		}
	}

	private void CheckSpecialRemoveCarry(Holdable CarryObject)
	{
		if ((bool)CarryObject.GetComponent<MyTool>() || MyTool.GetIsTypeSpecialTool(CarryObject.m_TypeIdentifier))
		{
			m_ToolModel = null;
		}
		if (Sign.GetIsTypeSign(CarryObject.m_TypeIdentifier) && m_Farmer.m_TypeIdentifier == ObjectType.FarmerPlayer)
		{
			GetTopObject().GetComponent<Sign>().SetBeingHeldByPlayer(Held: false);
		}
	}

	public void AddCarry(Holdable CarryObject)
	{
		if (m_Farmer.m_UniqueID == 325468 || m_Farmer.m_UniqueID == 22018)
		{
			BaseClass.TestObject(CarryObject, null, m_Farmer);
		}
		m_CarryObject.Add(CarryObject);
		SetCarryObjectParent(CarryObject);
		CarryObject.SendAction(new ActionInfo(ActionType.BeingHeld, default(TileCoord), m_Farmer));
		CheckSpecialAddCarry(CarryObject);
		if ((bool)m_Farmer.GetComponent<Worker>())
		{
			m_Farmer.GetComponent<Worker>().AddCarry();
		}
		ModManager.Instance.CheckCustomCallback(ModManager.CallbackTypes.HoldablePickedUp, CarryObject.m_TypeIdentifier, CarryObject.m_TileCoord, CarryObject.m_UniqueID, m_Farmer.m_UniqueID);
		CarryChanged();
	}

	public void AddCarry(Holdable CarryObject, bool Top)
	{
	}

	public void Disable(bool Disabled)
	{
		m_ToolCarryRoot.SetActive(!Disabled);
		m_WorkerCarryRoot.SetActive(!Disabled);
		m_CarryRoot.SetActive(!Disabled);
	}

	public bool AreHandsFull()
	{
		if (m_CarryObject.Count == 0 || m_CarryObject[0] == null)
		{
			return false;
		}
		if (!m_CarryObject[0].GetComponent<Holdable>().m_AllowMultiple)
		{
			return true;
		}
		if (m_CarryObject[0].m_Weight * (m_CarryObject.Count + 1) > m_TotalCapacity)
		{
			return true;
		}
		return false;
	}

	private void NastyBugFix()
	{
		if (m_CarryObject.Count > 0 && m_CarryObject[0] == null)
		{
			m_CarryObject.RemoveAt(0);
		}
	}

	public bool CanAddCarry(Holdable NewHoldable)
	{
		if (NewHoldable == null)
		{
			return false;
		}
		NastyBugFix();
		if (m_CarryObject.Count > 0)
		{
			if (ToolFillable.GetIsTypeFillable(m_CarryObject[0].m_TypeIdentifier) && m_CarryObject[0].GetComponent<ToolFillable>().CanAcceptObjectType(NewHoldable.m_TypeIdentifier))
			{
				return true;
			}
			if (!m_CarryObject[0].GetComponent<Holdable>().m_AllowMultiple)
			{
				return false;
			}
			if (m_CarryObject[0].m_TypeIdentifier != NewHoldable.m_TypeIdentifier)
			{
				return false;
			}
			if (m_CarryObject[0].m_Weight * (m_CarryObject.Count + 1) > m_TotalCapacity)
			{
				return false;
			}
		}
		if ((bool)m_Farmer.GetComponent<Worker>() && (bool)NewHoldable.GetComponent<FarmerPlayer>())
		{
			return false;
		}
		return true;
	}

	public bool CanAddCarry(ObjectType NewType)
	{
		NastyBugFix();
		if (m_CarryObject.Count > 0)
		{
			if (ToolFillable.GetIsTypeFillable(m_CarryObject[0].m_TypeIdentifier) && m_CarryObject[0].GetComponent<ToolFillable>().CanAcceptObjectType(NewType) && m_CarryObject[0].m_TypeIdentifier != ObjectType.ToolPitchfork)
			{
				return true;
			}
			if (!m_CarryObject[0].GetComponent<Holdable>().m_AllowMultiple)
			{
				return false;
			}
			if (m_CarryObject[0].m_TypeIdentifier != NewType)
			{
				return false;
			}
			if (m_CarryObject[0].m_Weight * (m_CarryObject.Count + 1) > m_TotalCapacity)
			{
				return false;
			}
		}
		if (ToolFillable.GetIsTypeLiquid(NewType))
		{
			return false;
		}
		return true;
	}

	public bool CanNormallyCarryMore(ObjectType NewType)
	{
		if (m_CarryObject.Count == 0)
		{
			return false;
		}
		if (!m_CarryObject[0].GetComponent<Holdable>().m_AllowMultiple)
		{
			return false;
		}
		if (m_CarryObject[0].m_TypeIdentifier != NewType)
		{
			return false;
		}
		return true;
	}

	public bool TryAddCarry(Holdable NewHoldable)
	{
		if (!CanAddCarry(NewHoldable))
		{
			return false;
		}
		NewHoldable.SetIsSavable(IsSavable: false);
		AddCarry(NewHoldable);
		return true;
	}

	public bool TryAddCarry(ObjectType IdentifierType, int Used = 0)
	{
		Holdable component = ObjectTypeList.Instance.CreateObjectFromIdentifier(IdentifierType, new Vector3(0f, 0f, 0f), Quaternion.identity).GetComponent<Holdable>();
		bool num = TryAddCarry(component);
		if (!num)
		{
			component.StopUsing();
		}
		component.m_UsageCount = Used;
		return num;
	}

	public void AddTempCarry(ObjectType IdentifierType, int Used = 0)
	{
		TryAddCarry(IdentifierType, Used);
		m_LastTempCarry = GetTopObject();
	}

	public void AddTempCarry(Holdable NewObject)
	{
		TryAddCarry(NewObject);
		m_LastTempCarry = NewObject;
	}

	public void RemoveTempCarry()
	{
		if ((bool)m_LastTempCarry && m_LastTempCarry.m_TypeIdentifier != ObjectType.Folk)
		{
			if (m_CarryObject.Contains(m_LastTempCarry))
			{
				int index = m_CarryObject.IndexOf(m_LastTempCarry);
				DestroyObject(index);
			}
			else
			{
				m_LastTempCarry.StopUsing();
			}
			m_LastTempCarry = null;
		}
	}

	public Holdable GetTempObject()
	{
		return m_LastTempCarry;
	}

	public bool GetIsCarryingSomething()
	{
		if (m_CarryObject.Count > 0)
		{
			return true;
		}
		return false;
	}

	private void ObjectRemoved(Holdable NewObject)
	{
		if ((bool)GameStateManager.Instance.GetCurrentState().GetComponent<GameStateDragInventorySlot>())
		{
			GameStateManager.Instance.GetCurrentState().GetComponent<GameStateDragInventorySlot>().CheckObjectDropped(NewObject);
		}
		CheckSpecialRemoveCarry(NewObject);
	}

	private void DestroyObject(int Index)
	{
		if (Index >= 0 && Index < m_CarryObject.Count)
		{
			Holdable holdable = m_CarryObject[Index];
			ObjectRemoved(m_CarryObject[Index]);
			m_CarryObject.RemoveAt(Index);
			holdable.SendAction(new ActionInfo(ActionType.Dropped, default(TileCoord), m_Farmer));
			holdable.StopUsing();
			CarryChanged();
		}
	}

	public void DestroyTopObject()
	{
		DestroyObject(m_CarryObject.Count - 1);
	}

	public void DestroyAllObjects()
	{
		while (m_CarryObject.Count > 0)
		{
			DestroyObject(m_CarryObject.Count - 1);
		}
	}

	public Holdable RemoveObject(int Index)
	{
		Holdable result = m_CarryObject[Index];
		m_CarryObject[Index].SendAction(new ActionInfo(ActionType.Dropped, m_Farmer.m_TileCoord, m_Farmer));
		ObjectRemoved(m_CarryObject[Index]);
		m_CarryObject.RemoveAt(Index);
		CarryChanged();
		return result;
	}

	public Holdable RemoveTopObject()
	{
		if (m_CarryObject.Count == 0)
		{
			return null;
		}
		Holdable result = m_CarryObject[m_CarryObject.Count - 1];
		RemoveObject(m_CarryObject.Count - 1);
		return result;
	}

	public Holdable RemoveBottomObject()
	{
		Holdable result = m_CarryObject[0];
		RemoveObject(0);
		return result;
	}

	public Holdable GetTopObject()
	{
		if (m_CarryObject.Count == 0)
		{
			return null;
		}
		return m_CarryObject[m_CarryObject.Count - 1];
	}

	public ObjectType GetTopObjectType()
	{
		if (m_CarryObject.Count == 0)
		{
			return ObjectTypeList.m_Total;
		}
		return m_CarryObject[m_CarryObject.Count - 1].m_TypeIdentifier;
	}

	public ObjectType GetLastObjectType()
	{
		return m_LastObjectType;
	}

	public Holdable GetLastObject()
	{
		return m_LastObject;
	}

	public void DropAllObjects()
	{
		for (int i = 0; i < m_CarryObject.Count; i++)
		{
			m_CarryObject[i].SendAction(new ActionInfo(ActionType.Dropped, m_Farmer.m_TileCoord, m_Farmer));
			ObjectRemoved(m_CarryObject[i]);
			ModManager.Instance.CheckItemTypeSpawnedCallback(m_CarryObject[i].m_UniqueID, m_CarryObject[i].m_TypeIdentifier, m_CarryObject[i].m_TileCoord);
		}
		m_CarryObject.Clear();
		CarryChanged();
	}

	public int GetCarryCount()
	{
		return m_CarryObject.Count;
	}

	public int GetHeldWeight()
	{
		Holdable topObject = GetTopObject();
		if (topObject == null)
		{
			return 0;
		}
		return Holdable.GetWeight(topObject.m_TypeIdentifier) * m_CarryObject.Count;
	}

	public MyTool GetTool()
	{
		if (m_CarryObject.Count == 0)
		{
			return null;
		}
		if (m_CarryObject[0] == null)
		{
			return null;
		}
		return m_CarryObject[0].GetComponent<MyTool>();
	}

	public int StowObjects()
	{
		int result = 0;
		for (int num = m_CarryObject.Count - 1; num >= 0; num--)
		{
			Holdable holdable = m_CarryObject[num];
			if (Upgrade.GetIsTypeUpgrade(holdable.m_TypeIdentifier) && m_Farmer.m_FarmerUpgrades.WillAdd(holdable))
			{
				ObjectRemoved(holdable);
				m_CarryObject.RemoveAt(num);
				m_Farmer.m_FarmerUpgrades.AttemptAdd(holdable);
				result = 2;
			}
			else if (m_Farmer.m_FarmerInventory.CanAdd(holdable))
			{
				ObjectRemoved(holdable);
				m_CarryObject.RemoveAt(num);
				m_Farmer.m_FarmerInventory.AttemptAdd(holdable);
				result = 1;
			}
		}
		CarryChanged();
		return result;
	}

	public int SwapObjects()
	{
		if (m_CarryObject.Count == 0 && m_Farmer.m_FarmerInventory.GetFullSlots() == 0)
		{
			return 0;
		}
		if (m_CarryObject.Count != 0 && !m_Farmer.m_FarmerInventory.IsObjectTypeAcceptable(m_CarryObject[0]))
		{
			return 0;
		}
		int num = 0;
		for (int num2 = m_CarryObject.Count - 1; num2 >= 0; num2--)
		{
			num += m_CarryObject[num2].m_Weight;
		}
		int num3 = 0;
		Holdable lastObject = m_Farmer.m_FarmerInventory.GetLastObject();
		if ((bool)lastObject)
		{
			num3 = lastObject.m_Weight;
		}
		int num4 = m_Farmer.m_FarmerInventory.m_TotalCapacity - (m_Farmer.m_FarmerInventory.m_TotalWeight - num3);
		if (num > num4)
		{
			return 0;
		}
		lastObject = m_Farmer.m_FarmerInventory.PopObject(Top: true);
		for (int num5 = m_CarryObject.Count - 1; num5 >= 0; num5--)
		{
			Holdable holdable = m_CarryObject[num5];
			if (m_Farmer.m_FarmerInventory.CanAdd(holdable))
			{
				ObjectRemoved(holdable);
				m_CarryObject.RemoveAt(num5);
				m_Farmer.m_FarmerInventory.AttemptAdd(holdable);
			}
		}
		if ((bool)lastObject)
		{
			TryAddCarry(lastObject);
		}
		CarryChanged();
		return 1;
	}

	public void RecallObject(ObjectType Identifier)
	{
		if (!GetIsCarryingSomething())
		{
			Holdable holdable = ((Identifier == ObjectTypeList.m_Total) ? m_Farmer.m_FarmerInventory.PopObject(Top: true) : m_Farmer.m_FarmerInventory.ReleaseObject(Identifier));
			if ((bool)holdable)
			{
				AddCarry(holdable);
			}
		}
	}

	public ObjectType GetObjectType()
	{
		if (m_CarryObject.Count == 0)
		{
			return ObjectTypeList.m_Total;
		}
		return m_CarryObject[m_CarryObject.Count - 1].m_TypeIdentifier;
	}

	private void UpdateCapacity()
	{
		m_TotalCapacity = m_MaxCarryCount + m_ExtraCapacity;
		int num = m_CarryObject.Count - m_TotalCapacity;
		for (int i = 0; i < num; i++)
		{
			RemoveObject(m_CarryObject.Count - 1);
		}
		CarryChanged();
		if (m_Farmer.m_TypeIdentifier == ObjectType.Worker)
		{
			m_Farmer.GetComponent<Worker>().SetCarryCapacity();
		}
	}

	public void SetCapacity(int Capacity)
	{
		m_MaxCarryCount = Capacity;
		UpdateCapacity();
	}

	public void SetExtraCarry(int ExtraCapacity)
	{
		m_ExtraCapacity = ExtraCapacity;
		UpdateCapacity();
	}

	public void FarmerMoved()
	{
		if (Sign.GetIsTypeSign(GetObjectType()))
		{
			GetTopObject().GetComponent<Sign>().CheckCoordChanged(m_Farmer.m_TileCoord);
		}
	}
}
