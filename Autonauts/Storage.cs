using System;
using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;

public class Storage : Building
{
	public static float m_PercentFull = 0.95f;

	public static List<ObjectType> m_Types;

	[HideInInspector]
	public ObjectType m_ObjectType;

	public int m_Capacity;

	public static bool m_SharedStorage = false;

	public int m_Stored;

	protected int m_Reserved;

	protected int m_TempStored;

	private float m_WobbleTimer;

	private float m_WobbleDelay;

	protected MeshRenderer m_Sign;

	public MeshRenderer m_LidRenderer;

	public List<int> m_Used;

	private int m_FullCheck;

	private bool m_FullAnimate;

	private float m_FullTimer;

	private bool m_FullOn;

	private float m_OpenTimer;

	private int m_AddedAmount;

	private static float[] m_HeightMultiplyer;

	private bool m_ChangeTypeLocked;

	public static bool GetIsTypeStorage(ObjectType NewType)
	{
		if (NewType == ObjectType.StorageFertiliser || StorageLiquid.GetIsTypeStorageLiquid(NewType) || StorageSand.GetIsTypeStorageSand(NewType) || StorageGeneric.GetIsTypeStorageGeneric(NewType) || StoragePalette.GetIsTypeStoragePalette(NewType) || NewType == ObjectType.StorageSeedlings || NewType == ObjectType.StorageWorker || NewType == ObjectType.StorageBeehiveCrude || NewType == ObjectType.StorageBeehive)
		{
			return true;
		}
		return false;
	}

	public static bool GetIsTypeMediumStorage(ObjectType NewType)
	{
		if (NewType == ObjectType.StorageGenericMedium || NewType == ObjectType.StorageLiquidMedium || NewType == ObjectType.StoragePaletteMedium || NewType == ObjectType.StorageSandMedium)
		{
			return true;
		}
		return false;
	}

	public new static void Init()
	{
		m_Types = new List<ObjectType>();
	}

	public override void RegisterClass()
	{
		base.RegisterClass();
		ClassManager.Instance.RegisterClass("Storage", m_TypeIdentifier);
		m_Types.Add(m_TypeIdentifier);
		m_HeightMultiplyer = new float[3];
		for (int i = 1; i < 3; i++)
		{
			m_HeightMultiplyer[i] = VariableManager.Instance.GetVariableAsFloat("StorageCapacityHeight" + (i + 1));
		}
	}

	public override void CheckAddCollectable(bool FromLoad)
	{
		base.CheckAddCollectable(FromLoad);
		if (!ObjectTypeList.m_Loading || FromLoad)
		{
			CollectionManager.Instance.AddCollectable("Storage", this);
		}
	}

	public override void Restart()
	{
		base.Restart();
		SetDimensions(new TileCoord(-1, -1), new TileCoord(1, 1), new TileCoord(0, 2));
		m_WobbleTimer = 0f;
		m_WobbleDelay = 0.5f;
		m_Stored = 0;
		m_Reserved = 0;
		m_TempStored = 0;
		m_Used.Clear();
		SetObjectType(m_ObjectType);
		UpdateStored();
		if (m_ObjectType != ObjectTypeList.m_Total)
		{
			ResourceManager.Instance.RegisterStorage(this);
		}
		m_ChangeTypeLocked = true;
	}

	protected new void Awake()
	{
		base.Awake();
		m_Used = new List<int>();
		m_ObjectType = ObjectTypeList.m_Total;
		m_Capacity = 0;
	}

	public override void PostCreate()
	{
		base.PostCreate();
		Transform transform = ObjectUtils.FindDeepChild(m_ModelRoot.transform, "Plane");
		if ((bool)transform)
		{
			m_Sign = transform.GetComponent<MeshRenderer>();
		}
	}

	public override void StopUsing(bool AndDestroy = true)
	{
		base.StopUsing(AndDestroy);
		if (m_ObjectType != ObjectTypeList.m_Total)
		{
			ResourceManager.Instance.UnRegisterStorage(this);
		}
	}

	public override void SetHighlight(bool Highlighted)
	{
		base.SetHighlight(Highlighted);
		GameStateBase currentState = GameStateManager.Instance.GetCurrentState();
		if ((bool)currentState && currentState.m_BaseState == GameStateManager.State.Edit)
		{
			return;
		}
		if ((bool)m_ParentBuilding)
		{
			if (m_ParentBuilding.m_Highlighted != Highlighted)
			{
				m_ParentBuilding.SetHighlight(Highlighted);
			}
			return;
		}
		if (Highlighted)
		{
			HudManager.Instance.ActivateStorageRollover(Activate: true, this);
		}
		else
		{
			HudManager.Instance.ActivateStorageRollover(Activate: false, this);
		}
		if (m_Levels == null)
		{
			return;
		}
		foreach (Building level in m_Levels)
		{
			if (level.m_Highlighted != Highlighted)
			{
				level.SetHighlight(Highlighted);
			}
		}
	}

	public override string GetHumanReadableName()
	{
		if (m_Name != "")
		{
			return m_Name;
		}
		string text = base.GetHumanReadableName();
		if (m_ObjectType != ObjectTypeList.m_Total)
		{
			if (m_ObjectType >= ObjectType.Total)
			{
				string value = "";
				foreach (ModCustom modCustomClass in ModManager.Instance.ModCustomClasses)
				{
					if (modCustomClass.ModIDOriginals.TryGetValue(m_ObjectType, out value))
					{
						text = TextManager.Instance.Get("StorageAny", value);
						break;
					}
				}
			}
			else
			{
				string humanReadableNameFromIdentifier = ObjectTypeList.Instance.GetHumanReadableNameFromIdentifier(m_ObjectType);
				text = TextManager.Instance.Get("StorageAny", humanReadableNameFromIdentifier);
			}
		}
		if (m_CountIndex != 0)
		{
			text = text + " " + m_CountIndex;
		}
		return text;
	}

	public override void Save(JSONNode Node)
	{
		base.Save(Node);
		if (m_ObjectType == ObjectTypeList.m_Total)
		{
			JSONUtils.Set(Node, "ObjectType", "Unknown");
		}
		else
		{
			JSONUtils.Set(Node, "ObjectType", ObjectTypeList.Instance.GetSaveNameFromIdentifier(m_ObjectType));
		}
		JSONUtils.Set(Node, "Stored", m_Stored);
		JSONUtils.Set(Node, "Locked", m_ChangeTypeLocked);
		JSONArray jSONArray = (JSONArray)(Node["Used"] = new JSONArray());
		for (int i = 0; i < m_Used.Count; i++)
		{
			jSONArray[i] = m_Used[i];
		}
	}

	public override void Load(JSONNode Node)
	{
		string asString = JSONUtils.GetAsString(Node, "ObjectType", "Unknown");
		ObjectType objectType = ((!(asString == "Unknown")) ? ObjectTypeList.Instance.GetIdentifierFromSaveName(asString, Check: false) : ObjectTypeList.m_Total);
		SetObjectType(objectType);
		m_TempStored = JSONUtils.GetAsInt(Node, "Stored", 0);
		m_ChangeTypeLocked = JSONUtils.GetAsBool(Node, "Locked", DefaultValue: false);
		m_Used.Clear();
		JSONArray asArray = Node["Used"].AsArray;
		for (int i = 0; i < asArray.Count; i++)
		{
			m_Used.Add(asArray[i].AsInt);
		}
		UpdateStored();
		base.Load(Node);
	}

	public void Transfer(Building OriginalBuilding)
	{
		ObjectType objectType = OriginalBuilding.GetComponent<Storage>().m_ObjectType;
		int stored = OriginalBuilding.GetComponent<Storage>().GetStored();
		SetObjectType(objectType);
		AddToStored(objectType, stored, null);
		UpdateStored();
	}

	protected void SetSign(ObjectType NewType)
	{
		if (NewType == ObjectTypeList.m_Total)
		{
			NewType = ObjectType.Empty;
		}
		else
		{
			ResourceManager.Instance.RegisterStorage(this);
		}
		Sprite icon = IconManager.Instance.GetIcon(NewType);
		if ((bool)icon && (bool)m_Sign)
		{
			m_Sign.material.SetTexture("_MainTex", icon.texture);
		}
	}

	public float GetStoredPercent()
	{
		return (float)GetStored() / (float)GetCapacity();
	}

	public bool GetStoredPercentFull()
	{
		if (GetStoredPercent() >= m_PercentFull)
		{
			return true;
		}
		return false;
	}

	public int GetStored(bool CheckStack = true, bool CheckReserved = true)
	{
		if (m_SharedStorage)
		{
			return ResourceManager.Instance.GetResource(m_ObjectType);
		}
		if ((bool)m_ParentBuilding && CheckStack)
		{
			return m_ParentBuilding.GetComponent<Storage>().GetStored();
		}
		if (CheckReserved)
		{
			return m_Stored - m_Reserved;
		}
		return m_Stored;
	}

	public virtual int GetStoredForDisplay()
	{
		if (m_SharedStorage)
		{
			return ResourceManager.Instance.GetResourceForDisplay(m_ObjectType);
		}
		if ((bool)m_ParentBuilding)
		{
			return m_ParentBuilding.GetComponent<Storage>().GetStoredForDisplay();
		}
		return m_Stored;
	}

	public int GetCapacity()
	{
		if (m_SharedStorage)
		{
			ResourceManager.Instance.GetStorage(m_ObjectType);
		}
		if (m_Levels != null && m_Levels.Count > 0)
		{
			int num = 1;
			foreach (Building level in m_Levels)
			{
				if (level.m_TypeIdentifier != ObjectType.ConverterFoundation)
				{
					num++;
				}
			}
			if (num > 1 && num <= 3)
			{
				float num2 = m_HeightMultiplyer[num - 1];
				return (int)((float)m_Capacity * num2);
			}
		}
		else if ((bool)m_ParentBuilding && GetIsTypeStorage(m_ParentBuilding.m_TypeIdentifier))
		{
			return m_ParentBuilding.GetComponent<Storage>().GetCapacity();
		}
		return m_Capacity;
	}

	private void UpdateStoredEvents(ObjectType NewType)
	{
		if (NewType == ObjectType.Stick)
		{
			QuestManager.Instance.AddEvent(QuestEvent.Type.UpdateStoredSticks, Bot: false, 0, this);
		}
	}

	public void SetStored(int Target)
	{
		if (m_ObjectType != ObjectTypeList.m_Total)
		{
			int capacity = GetCapacity();
			if (Target > capacity)
			{
				Target = capacity;
			}
			if (Target < 0)
			{
				Target = 0;
			}
			m_Stored = Target;
		}
	}

	public void AddToStored(ObjectType NewType, int Amount, Actionable Actioner, bool CheckCapacity = true)
	{
		if (m_SharedStorage)
		{
			ResourceManager.Instance.AddResource(NewType, Amount);
		}
		else
		{
			int capacity = GetCapacity();
			if (m_Stored == capacity)
			{
				return;
			}
			m_Stored += Amount;
			if (CheckCapacity && m_Stored > capacity)
			{
				m_Stored = capacity;
			}
			if (CheatManager.Instance.m_FillStorage && (bool)Actioner && Actioner.m_TypeIdentifier == ObjectType.FarmerPlayer)
			{
				m_Stored = capacity;
			}
		}
		UpdateStoredEvents(NewType);
		if ((bool)Actioner)
		{
			QuestManager.Instance.AddEvent(QuestEvent.Type.Store, Actioner.m_TypeIdentifier == ObjectType.Worker, NewType, this, Amount);
		}
	}

	public void AddToStored(BaseClass NewObject, Actionable Actioner)
	{
		if ((bool)m_ParentBuilding)
		{
			m_ParentBuilding.GetComponent<Storage>().AddToStored(NewObject, Actioner);
			return;
		}
		Holdable component = NewObject.GetComponent<Holdable>();
		if ((bool)component)
		{
			int usageCount = component.m_UsageCount;
			if (usageCount != 0)
			{
				m_Used.Add(usageCount);
			}
		}
		AddToStored(NewObject.m_TypeIdentifier, 1, Actioner);
		ObjectType typeIdentifier = NewObject.m_TypeIdentifier;
		UpdateStoredEvents(typeIdentifier);
		ModManager.Instance.CheckStorageAddedCallback(m_UniqueID);
	}

	public int ReleaseStored(ObjectType NewType, Actionable Actioner, int Amount = 1)
	{
		if ((bool)m_ParentBuilding)
		{
			return m_ParentBuilding.GetComponent<Storage>().ReleaseStored(NewType, Actioner, Amount);
		}
		int result = 0;
		if (m_SharedStorage)
		{
			ResourceManager.Instance.ReleaseResource(NewType);
		}
		else
		{
			if (m_Stored == 0)
			{
				return 0;
			}
			m_Stored -= Amount;
			if (m_Stored < 0)
			{
				m_Stored = 0;
			}
			int count = m_Used.Count;
			if (count > 0)
			{
				result = m_Used[count - 1];
				m_Used.RemoveAt(count - 1);
			}
			if (CheatManager.Instance.m_FillStorage && (bool)Actioner && Actioner.m_TypeIdentifier == ObjectType.FarmerPlayer)
			{
				m_Stored = 0;
				m_Used.Clear();
			}
		}
		ModManager.Instance.CheckStorageRemovedCallback(m_UniqueID);
		return result;
	}

	public bool GetIsFull()
	{
		return GetStored() == GetCapacity();
	}

	public override void SendAction(ActionInfo Info)
	{
		switch (Info.m_Action)
		{
		case ActionType.Engaged:
			m_Engager = Info.m_Object;
			if ((bool)m_Engager.GetComponent<FarmerPlayer>())
			{
				GameStateManager.Instance.StartSelectBuilding(this);
			}
			break;
		case ActionType.Disengaged:
			m_Engager = null;
			break;
		case ActionType.ReserveResource:
			if (m_SharedStorage)
			{
				ResourceManager.Instance.ReserveResource(m_ObjectType);
			}
			else
			{
				m_Reserved++;
			}
			break;
		case ActionType.UnreserveResource:
			if (m_SharedStorage)
			{
				ResourceManager.Instance.UnreserveResource(m_ObjectType);
			}
			else
			{
				m_Reserved--;
			}
			break;
		case ActionType.Refresh:
			if (m_TempStored == 0 || (bool)m_ParentBuilding)
			{
				break;
			}
			m_Stored = m_TempStored;
			m_TempStored = 0;
			if (m_Levels != null)
			{
				foreach (Building level in m_Levels)
				{
					if ((bool)level.GetComponent<Storage>())
					{
						level.GetComponent<Storage>().m_TempStored = 0;
					}
				}
			}
			UpdateStored();
			break;
		}
		base.SendAction(Info);
	}

	public override object GetActionInfo(GetActionInfo Info)
	{
		switch (Info.m_Action)
		{
		case GetAction.GetObjectType:
			return m_ObjectType;
		case GetAction.IsDeletable:
			if (m_SharedStorage || m_TypeIdentifier == ObjectType.FolkSeedPod || m_TypeIdentifier == ObjectType.Transmitter)
			{
				break;
			}
			if ((bool)m_ParentBuilding)
			{
				int storedForDisplay = m_ParentBuilding.GetComponent<Storage>().GetStoredForDisplay();
				int buildingLevelIndex = GetBuildingLevelIndex();
				if (storedForDisplay - buildingLevelIndex * m_Capacity > 0)
				{
					return false;
				}
			}
			else if (GetStoredForDisplay() > 0)
			{
				return false;
			}
			break;
		case GetAction.IsMovable:
			if (!m_SharedStorage && m_TypeIdentifier != ObjectType.FolkSeedPod && m_TypeIdentifier != ObjectType.Transmitter && (bool)m_ParentBuilding)
			{
				m_ParentBuilding.GetComponent<Storage>().GetStoredForDisplay();
				int buildingLevelIndex2 = GetBuildingLevelIndex();
				_ = m_Capacity;
			}
			break;
		case GetAction.IsPickable:
			if ((bool)m_ParentBuilding && m_ParentBuilding.GetIsSavable())
			{
				return true;
			}
			break;
		}
		return base.GetActionInfo(Info);
	}

	public override bool CanDoAction(ActionInfo Info, bool RightNow)
	{
		switch (Info.m_Action)
		{
		case ActionType.Engaged:
			if (GameStateManager.Instance.GetCurrentState().GetComponent<GameStateTeachWorker2>() == null && m_Engager == null)
			{
				return true;
			}
			return false;
		case ActionType.Disengaged:
			if ((bool)m_Engager)
			{
				return true;
			}
			return false;
		default:
			return base.CanDoAction(Info, RightNow);
		}
	}

	private void UpdateWobble()
	{
		if (m_WobbleTimer > 0f)
		{
			m_WobbleTimer -= TimeManager.Instance.m_NormalDelta;
			if (m_WobbleTimer < 0f)
			{
				m_WobbleTimer = 0f;
				base.transform.localScale = new Vector3(1f, 1f, 1f);
			}
			else
			{
				float num = m_WobbleTimer / m_WobbleDelay * 0.25f;
				float y = Mathf.Cos(m_WobbleTimer / m_WobbleDelay * 5f * 360f * ((float)Math.PI / 180f)) * num + 1f;
				base.transform.localScale = new Vector3(1f, y, 1f);
			}
		}
	}

	private void UpdateStateBuilding()
	{
	}

	public virtual void SetObjectType(ObjectType NewType)
	{
		ObjectType objectType = m_ObjectType;
		m_ObjectType = NewType;
		if (m_Levels != null)
		{
			foreach (Building level in m_Levels)
			{
				if ((bool)level.GetComponent<Storage>())
				{
					level.GetComponent<Storage>().SetObjectType(NewType);
				}
			}
		}
		if (!ObjectTypeList.m_Loading && (bool)ObjectCountManager.Instance)
		{
			if (m_ObjectType != ObjectTypeList.m_Total && !m_Blueprint)
			{
				if (objectType != m_ObjectType)
				{
					m_CountIndex = ObjectCountManager.Instance.RegisterNewObject(this);
				}
			}
			else
			{
				m_CountIndex = 0;
			}
		}
		if (!ObjectTypeList.m_Loading && (bool)ObjectCountManager.Instance && objectType != m_ObjectType)
		{
			string iDName = ObjectTypeList.GetIDName(m_ObjectType);
			ModManager.Instance.CheckStorageItemChangedCallback(m_UniqueID, iDName);
		}
	}

	public override Vector3 AddBuilding(Building NewBuilding)
	{
		Vector3 result = base.AddBuilding(NewBuilding);
		if ((bool)NewBuilding.GetComponent<Storage>())
		{
			int stored = NewBuilding.GetComponent<Storage>().GetStored(CheckStack: false);
			NewBuilding.GetComponent<Storage>().m_Stored = 0;
			if (m_ObjectType != ObjectTypeList.m_Total)
			{
				NewBuilding.GetComponent<Storage>().SetObjectType(m_ObjectType);
			}
			else
			{
				SetObjectType(NewBuilding.GetComponent<Storage>().m_ObjectType);
			}
			AddToStored(m_ObjectType, stored, null, CheckCapacity: false);
		}
		UpdateStored();
		return result;
	}

	protected virtual bool IsObjectTypeAcceptible(ObjectType NewType)
	{
		return true;
	}

	protected virtual bool CanAcceptObject(BaseClass NewObject, ObjectType NewType)
	{
		int stored = GetStored(CheckStack: true, CheckReserved: false);
		int capacity = GetCapacity();
		if (stored >= capacity)
		{
			return false;
		}
		return true;
	}

	public Vector3 GetAddPoint()
	{
		if (m_Levels != null && m_Levels.Count > 0 && m_Levels[m_Levels.Count - 1].m_TypeIdentifier != ObjectType.ConverterFoundation)
		{
			return m_Levels[m_Levels.Count - 1].m_ModelRoot.transform.position;
		}
		return m_ModelRoot.transform.position;
	}

	private void StartAddAnything(AFO Info)
	{
		Actionable @object = Info.m_Object;
		Info.m_Object.transform.position = GetAddPoint();
		if (m_ObjectType != @object.m_TypeIdentifier)
		{
			SetObjectType(@object.m_TypeIdentifier);
		}
		PlaySound playSound = AudioManager.Instance.StartEvent("BuildingStorageAdd", this, Remember: true);
		if ((bool)Info.m_Actioner && (bool)Info.m_Actioner.GetComponent<Farmer>())
		{
			float pitch = 1f + (float)Info.m_Actioner.GetComponent<Farmer>().m_FarmerCarry.GetCarryCount() * 0.25f;
			if (playSound != null && playSound.m_Result != null)
			{
				playSound.m_Result.ActingVariation.VarAudio.pitch = pitch;
			}
		}
		AddToStored(@object, Info.m_Actioner);
		m_Reserved++;
		GetTopBuilding().GetComponent<Storage>().StartOpenLid();
	}

	private void EndAddAnything(AFO Info)
	{
		Info.m_Object.StopUsing();
		m_Reserved--;
		UpdateStored();
	}

	private void AbortAddAnything(AFO Info)
	{
		Actionable @object = Info.m_Object;
		Info.m_Object.transform.position = m_ModelRoot.transform.position;
		m_Reserved--;
		ReleaseStored(@object.m_TypeIdentifier, Info.m_Actioner);
		if (ToolFillable.GetIsTypeFillable(@object.m_TypeIdentifier) && (bool)Info.m_Actioner)
		{
			Info.m_Actioner.GetComponent<Farmer>().m_FarmerAction.UndoAdd(@object.GetComponent<Actionable>());
		}
	}

	private void StartRelease(AFO Info)
	{
		int used = ReleaseStored(m_ObjectType, Info.m_Actioner);
		AddAnimationManager.Instance.Add(this, Adding: false);
		Info.m_Actioner.GetComponent<Farmer>().m_FarmerCarry.AddTempCarry(m_ObjectType, used);
		PlaySound playSound = AudioManager.Instance.StartEvent("BuildingStorageTake", this, Remember: true);
		float pitch = 1f + (float)Info.m_Actioner.GetComponent<Farmer>().m_FarmerCarry.GetCarryCount() * 0.25f;
		if (playSound != null && playSound.m_Result != null)
		{
			playSound.m_Result.ActingVariation.VarAudio.pitch = pitch;
		}
		UpdateStored();
		GetTopBuilding().GetComponent<Storage>().StartOpenLid();
	}

	private void AbortRelease(AFO Info)
	{
		Info.m_Actioner.GetComponent<Farmer>().m_FarmerCarry.RemoveTempCarry();
		AddToStored(m_ObjectType, 1, Info.m_Actioner);
	}

	private bool CanReleaseObject()
	{
		if (m_ObjectType == ObjectTypeList.m_Total)
		{
			return false;
		}
		return GetStored() > 0;
	}

	private bool GetIsStoredTypeVariable()
	{
		if (StorageGeneric.GetIsTypeStorageGeneric(m_TypeIdentifier) || StorageLiquid.GetIsTypeStorageLiquid(m_TypeIdentifier) || StoragePalette.GetIsTypeStoragePalette(m_TypeIdentifier) || StorageSand.GetIsTypeStorageSand(m_TypeIdentifier))
		{
			return true;
		}
		return false;
	}

	protected ActionType CheckReset(AFO Info)
	{
		if (Info.m_ActionType == AFO.AT.Primary && GetIsStoredTypeVariable() && GameStateManager.Instance.GetActualState() == GameStateManager.State.Normal && m_ObjectType != ObjectTypeList.m_Total && m_Stored == 0 && Info.m_Actioner.m_TypeIdentifier == ObjectType.FarmerPlayer && (Info.m_ActionType == AFO.AT.Primary || !CanAcceptObject(Info.m_Object, Info.m_ObjectType)))
		{
			Info.m_StartAction = null;
			Info.m_AbortAction = null;
			Info.m_FarmerState = Farmer.State.Total;
			return ActionType.EngageObject;
		}
		return ActionType.Total;
	}

	public bool CanBeUpgraded()
	{
		if (m_BuildingToUpgradeTo == ObjectTypeList.m_Total)
		{
			return false;
		}
		if (m_TotalLevels != m_MaxLevels)
		{
			return false;
		}
		if (QuestManager.Instance.GetIsBuildingLocked(m_BuildingToUpgradeTo))
		{
			return false;
		}
		return true;
	}

	protected ActionType GetActionFromNothing(AFO Info)
	{
		if (GameStateManager.Instance.GetActualState() != GameStateManager.State.TeachWorker && m_Engager == null)
		{
			Info.m_FarmerState = Farmer.State.Engaged;
			return ActionType.EngageObject;
		}
		return ActionType.Total;
	}

	private void StartRemoveFillable(AFO Info)
	{
		ToolFillable component = Info.m_Object.GetComponent<ToolFillable>();
		m_AddedAmount = component.GetSpace();
		if (m_AddedAmount > m_Stored)
		{
			m_AddedAmount = m_Stored;
		}
		ReleaseStored(m_ObjectType, Info.m_Actioner, m_AddedAmount);
		AddAnimationManager.Instance.Add(this, Adding: false);
		AudioManager.Instance.StartEvent("BuildingStorageTake", this);
		GetTopBuilding().GetComponent<Storage>().StartOpenLid();
		UpdateStored();
	}

	private void EndRemoveFillable(AFO Info)
	{
		Info.m_Object.GetComponent<ToolFillable>().Fill(m_ObjectType, m_AddedAmount);
	}

	private void AbortRemoveFillable(AFO Info)
	{
		AddToStored(m_ObjectType, m_AddedAmount, Info.m_Actioner);
		Info.m_Object.GetComponent<ToolFillable>().Empty(m_AddedAmount);
	}

	private ActionType GetPrimaryActionFromFillable(AFO Info)
	{
		Info.m_StartAction = StartRemoveFillable;
		Info.m_EndAction = EndRemoveFillable;
		Info.m_AbortAction = AbortRemoveFillable;
		Info.m_FarmerState = Farmer.State.Taking;
		if (GetStored() == 0)
		{
			return ActionType.Fail;
		}
		ToolFillable component = Info.m_Object.GetComponent<ToolFillable>();
		if (component == null)
		{
			return ActionType.Fail;
		}
		if (!component.CanAcceptObjectType(m_ObjectType))
		{
			return ActionType.Fail;
		}
		return ActionType.TakeResource;
	}

	private void StartAddFillable(AFO Info)
	{
		if (!(Info.m_Object == null))
		{
			ToolFillable component = Info.m_Object.GetComponent<ToolFillable>();
			int num = component.m_Stored;
			if (m_Stored + num > GetCapacity())
			{
				num = GetCapacity() - m_Stored;
			}
			ObjectType heldType = component.m_HeldType;
			component.Empty(num);
			if (m_ObjectType != heldType)
			{
				SetObjectType(heldType);
			}
			AddToStored(heldType, num, Info.m_Actioner);
			m_AddedAmount = num;
			AddAnimationManager.Instance.Add(this, Adding: true);
			AudioManager.Instance.StartEvent("BuildingStorageAdd", this);
			GetTopBuilding().GetComponent<Storage>().StartOpenLid();
		}
	}

	private void EndAddFillable(AFO Info)
	{
		UpdateStored();
	}

	private void AbortAddFillable(AFO Info)
	{
		ReleaseStored(m_ObjectType, Info.m_Actioner, m_AddedAmount);
	}

	private ActionType GetSecondaryActionFromFillable(AFO Info)
	{
		Info.m_StartAction = StartAddFillable;
		Info.m_EndAction = EndAddFillable;
		Info.m_AbortAction = AbortAddFillable;
		Info.m_FarmerState = Farmer.State.Adding;
		ToolFillable component = Info.m_Object.GetComponent<ToolFillable>();
		if (component == null)
		{
			return ActionType.Fail;
		}
		if (component.m_HeldType == ObjectTypeList.m_Total)
		{
			return ActionType.Fail;
		}
		if (!IsObjectTypeAcceptible(component.m_HeldType))
		{
			return ActionType.Total;
		}
		if (!CanAcceptObject(null, component.m_HeldType))
		{
			return ActionType.Fail;
		}
		if (m_ObjectType != component.m_HeldType && GetStored() > 0)
		{
			return ActionType.Fail;
		}
		int stored = GetStored();
		int capacity = GetCapacity();
		if (stored >= capacity)
		{
			return ActionType.Fail;
		}
		return ActionType.AddResource;
	}

	public override ActionType GetActionFromObject(AFO Info)
	{
		if (Info.m_ActionType == AFO.AT.Primary)
		{
			if (ToolFillable.GetIsTypeFillable(Info.m_ObjectType))
			{
				if (GetTopBuilding().m_TypeIdentifier == ObjectType.ConverterFoundation)
				{
					return ActionType.Fail;
				}
				return GetPrimaryActionFromFillable(Info);
			}
			Info.m_StartAction = StartRelease;
			Info.m_AbortAction = AbortRelease;
			Info.m_FarmerState = Farmer.State.Taking;
			if (GetTopBuilding().m_TypeIdentifier == ObjectType.ConverterFoundation)
			{
				return ActionType.Fail;
			}
			if (CanReleaseObject())
			{
				return ActionType.TakeResource;
			}
			ActionType actionType = CheckReset(Info);
			if (actionType != ActionType.Total)
			{
				return actionType;
			}
			return ActionType.Fail;
		}
		if (Info.m_ActionType == AFO.AT.Secondary)
		{
			if (ToolFillable.GetIsTypeFillable(Info.m_ObjectType) && (bool)Info.m_Object && !Info.m_Object.GetComponent<ToolFillable>().GetIsEmpty() && ToolFillable.GetIsTypeEmptyable(Info.m_Object.m_TypeIdentifier))
			{
				if (GetTopBuilding().m_TypeIdentifier == ObjectType.ConverterFoundation)
				{
					return ActionType.Fail;
				}
				return GetSecondaryActionFromFillable(Info);
			}
			Info.m_StartAction = StartAddAnything;
			Info.m_EndAction = EndAddAnything;
			Info.m_AbortAction = AbortAddAnything;
			Info.m_FarmerState = Farmer.State.Adding;
			if (GetTopBuilding().m_TypeIdentifier == ObjectType.ConverterFoundation)
			{
				return ActionType.Fail;
			}
			if (m_ChangeTypeLocked && Info.m_ObjectType != m_ObjectType && (bool)Info.m_Actioner && Info.m_Actioner.m_TypeIdentifier == ObjectType.Worker && m_ObjectType != ObjectTypeList.m_Total)
			{
				return ActionType.Fail;
			}
			if (CanAcceptObject(Info.m_Object, Info.m_ObjectType))
			{
				return ActionType.AddResource;
			}
			ActionType actionType2 = CheckReset(Info);
			if (actionType2 != ActionType.Total)
			{
				return actionType2;
			}
			return ActionType.Fail;
		}
		if (Info.m_ActionType == AFO.AT.AltPrimary && Info.m_Actioner.m_TypeIdentifier == ObjectType.FarmerPlayer)
		{
			ActionType actionFromNothing = GetActionFromNothing(Info);
			if (actionFromNothing != ActionType.Total)
			{
				return actionFromNothing;
			}
		}
		return ActionType.Total;
	}

	public override void SetBlueprint(bool Blueprint, bool Register = false)
	{
		base.SetBlueprint(Blueprint, Register);
		if (Blueprint)
		{
			if (m_ObjectType != ObjectTypeList.m_Total)
			{
				ResourceManager.Instance.UnRegisterStorage(this);
			}
		}
		else if (m_ObjectType != ObjectTypeList.m_Total)
		{
			ResourceManager.Instance.RegisterStorage(this);
		}
	}

	public void MakeSignTransparent(bool Transparent)
	{
		if (!(m_Sign == null))
		{
			if (Transparent)
			{
				m_Sign.material.color = new Color(0f, 0f, 0f, 1f);
			}
			else
			{
				m_Sign.material.color = new Color(1f, 1f, 1f, 1f);
			}
			m_FullAnimate = false;
		}
	}

	private void UpdateLidColour(bool Empty, bool Full)
	{
		if (!(m_LidRenderer == null) && !m_Blueprint)
		{
			if (Empty)
			{
				m_LidRenderer.sharedMaterial = MaterialManager.Instance.m_MaterialBlack;
			}
			else if (Full)
			{
				m_LidRenderer.sharedMaterial = MaterialManager.Instance.m_MaterialRed;
			}
			else
			{
				m_LidRenderer.sharedMaterial = MaterialManager.Instance.m_MaterialRed;
			}
		}
	}

	public virtual void UpdateStored()
	{
		if (!m_Sign)
		{
			return;
		}
		int stored = GetStored();
		int capacity = GetCapacity();
		bool flag = false;
		if (stored == 0 && capacity != 0)
		{
			flag = true;
		}
		bool full = false;
		if (stored >= capacity && capacity != 0)
		{
			full = true;
		}
		MakeSignTransparent(flag);
		UpdateLidColour(flag, full);
		if (m_Levels == null)
		{
			return;
		}
		foreach (Building level in m_Levels)
		{
			if (GetIsTypeStorage(level.m_TypeIdentifier))
			{
				Storage component = level.GetComponent<Storage>();
				component.MakeSignTransparent(flag);
				component.UpdateLidColour(flag, full);
			}
		}
	}

	public override void MaterialsChanged()
	{
		base.MaterialsChanged();
		UpdateStored();
	}

	protected bool GetNewLevelAllowedStacked(Building NewBuilding)
	{
		if (NewBuilding.m_TileCoord != m_TileCoord || NewBuilding.m_Rotation != m_Rotation)
		{
			return false;
		}
		if (m_TotalLevels + NewBuilding.m_TotalLevels > m_MaxLevels)
		{
			return false;
		}
		ObjectType typeIdentifier = NewBuilding.m_TypeIdentifier;
		if (typeIdentifier == ObjectType.ConverterFoundation)
		{
			typeIdentifier = NewBuilding.GetComponent<ConverterFoundation>().m_NewBuilding.m_TypeIdentifier;
		}
		if (typeIdentifier != m_TypeIdentifier)
		{
			return false;
		}
		if ((bool)NewBuilding.GetComponent<Storage>())
		{
			ObjectType objectType = NewBuilding.GetComponent<Storage>().m_ObjectType;
			if (m_ObjectType != ObjectTypeList.m_Total && objectType != ObjectTypeList.m_Total && m_ObjectType != objectType)
			{
				return false;
			}
		}
		return true;
	}

	private void UpdateFull()
	{
		m_FullCheck++;
		if (m_FullCheck >= 10)
		{
			m_FullCheck = 0;
			m_FullAnimate = false;
			if (GetStoredPercent() >= m_PercentFull && GetCapacity() != 0)
			{
				m_FullAnimate = true;
			}
		}
		if (!m_FullAnimate)
		{
			return;
		}
		m_FullTimer += TimeManager.Instance.m_NormalDelta;
		bool flag = (int)(m_FullTimer * 60f) % 20 < 10;
		if (m_FullOn == flag)
		{
			return;
		}
		m_FullOn = flag;
		Color color = new Color(1f, 1f, 1f, 1f);
		if (m_FullOn)
		{
			color = new Color(1f, 0f, 0f, 1f);
		}
		if (m_Sign != null)
		{
			m_Sign.material.color = color;
		}
		if (m_Levels == null)
		{
			return;
		}
		foreach (Building level in m_Levels)
		{
			if (level.m_TypeIdentifier != ObjectType.ConverterFoundation)
			{
				level.GetComponent<Storage>().m_Sign.material.color = color;
			}
		}
	}

	public virtual void StartOpenLid()
	{
		m_OpenTimer = 0.2f;
	}

	public virtual void CloseLid()
	{
		m_OpenTimer = 0f;
	}

	public void ToggleLock()
	{
		m_ChangeTypeLocked = !m_ChangeTypeLocked;
	}

	public bool GetIsLocked()
	{
		return m_ChangeTypeLocked;
	}

	private void UpdateLid()
	{
		if (m_OpenTimer > 0f)
		{
			m_OpenTimer -= TimeManager.Instance.m_NormalDelta;
			if (m_OpenTimer <= 0f)
			{
				m_OpenTimer = 0f;
				CloseLid();
			}
		}
	}

	protected virtual void Update()
	{
		UpdateWobble();
		UpdateFull();
		UpdateLid();
	}
}
