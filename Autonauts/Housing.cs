using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;

public class Housing : Building
{
	public List<Folk> m_Folks;

	public int m_MaxFolks;

	private string m_FamilyName;

	private Transform m_HeadPoint;

	private Transform m_DoorPoint;

	private FolkStatusIndicator m_StatusIndicator;

	private Folk m_BestFolk;

	[HideInInspector]
	public int m_UsageCount;

	[HideInInspector]
	public int m_MaxUsageCount;

	[HideInInspector]
	public int m_RepairCountAdded;

	public static List<ObjectType> m_Types;

	protected GameObject m_Dead;

	private List<MeshRenderer> m_NormalMeshes;

	private GameObject m_Scaffolding;

	public override void RegisterClass()
	{
		base.RegisterClass();
		m_Types = new List<ObjectType>();
		m_Types.Add(ObjectType.Hut);
		m_Types.Add(ObjectType.LogCabin);
		m_Types.Add(ObjectType.StoneCottage);
		m_Types.Add(ObjectType.BrickHut);
		m_Types.Add(ObjectType.Mansion);
		m_Types.Add(ObjectType.Castle);
	}

	public static bool GetIsTypeHouse(ObjectType NewType)
	{
		if (NewType == ObjectType.Hut || NewType == ObjectType.LogCabin || NewType == ObjectType.StoneCottage || NewType == ObjectType.BrickHut || NewType == ObjectType.Mansion || NewType == ObjectType.Castle)
		{
			return true;
		}
		return false;
	}

	public override void Restart()
	{
		base.Restart();
		SetDimensions(new TileCoord(0, 0), new TileCoord(0, 0), new TileCoord(0, 1));
		m_Folks = new List<Folk>();
		m_MaxFolks = 1;
		m_StatusIndicator.gameObject.SetActive(value: false);
		m_FamilyName = "Nobby";
		m_MaxUsageCount = VariableManager.Instance.GetVariableAsInt(m_TypeIdentifier, "MaxUsage", CheckValid: false);
		m_UsageCount = 0;
		UpdateUsage();
	}

	public override void PostCreate()
	{
		base.PostCreate();
		m_Dead = m_ModelRoot.transform.Find("Dead").gameObject;
		m_NormalMeshes = new List<MeshRenderer>();
		MeshRenderer[] componentsInChildren = m_ModelRoot.GetComponentsInChildren<MeshRenderer>();
		foreach (MeshRenderer item in componentsInChildren)
		{
			m_NormalMeshes.Add(item);
		}
		componentsInChildren = m_Dead.GetComponentsInChildren<MeshRenderer>();
		foreach (MeshRenderer item2 in componentsInChildren)
		{
			m_NormalMeshes.Remove(item2);
		}
		m_HeadPoint = m_ModelRoot.transform.Find("HeadPoint");
		m_DoorPoint = m_ModelRoot.transform.Find("DoorPoint");
		if ((bool)ObjectUtils.FindDeepChild(m_ModelRoot.transform, "Scaffolding"))
		{
			m_Scaffolding = ObjectUtils.FindDeepChild(m_ModelRoot.transform, "Scaffolding").gameObject;
		}
		GameObject original = (GameObject)Resources.Load("Prefabs/WorldObjects/Other/FolkStatusIndicator", typeof(GameObject));
		Transform parent = null;
		if ((bool)HudManager.Instance)
		{
			parent = HudManager.Instance.m_IndicatorsRootTransform;
		}
		m_StatusIndicator = Object.Instantiate(original, new Vector3(0f, 0f, 0f), Quaternion.identity, parent).GetComponent<FolkStatusIndicator>();
		m_StatusIndicator.SetFolk(this);
		m_StatusIndicator.gameObject.SetActive(value: false);
		m_StatusIndicator.SetState(FolkStatusIndicator.State.Unhappy);
	}

	public override void StopUsing(bool AndDestroy = true)
	{
		foreach (Folk folk in m_Folks)
		{
			folk.Homeless();
		}
		m_Folks.Clear();
		base.StopUsing(AndDestroy);
	}

	protected new void OnDestroy()
	{
		foreach (Folk folk in m_Folks)
		{
			Object.DestroyImmediate(folk.gameObject);
		}
		if ((bool)m_StatusIndicator)
		{
			Object.DestroyImmediate(m_StatusIndicator.gameObject);
		}
		base.OnDestroy();
	}

	public override string GetHumanReadableName()
	{
		string text = "";
		if (m_Folks.Count > 0)
		{
			text = text + m_FamilyName + "'s ";
		}
		return text + base.GetHumanReadableName();
	}

	public override void Save(JSONNode Node)
	{
		base.Save(Node);
		JSONUtils.Set(Node, "Used", m_UsageCount);
		JSONUtils.Set(Node, "Repaired", m_RepairCountAdded);
		JSONArray jSONArray = (JSONArray)(Node["Folks"] = new JSONArray());
		int num = 0;
		foreach (Folk folk in m_Folks)
		{
			JSONNode jSONNode2 = new JSONObject();
			string saveNameFromIdentifier = ObjectTypeList.Instance.GetSaveNameFromIdentifier(folk.GetComponent<BaseClass>().m_TypeIdentifier);
			JSONUtils.Set(jSONNode2, "ID", saveNameFromIdentifier);
			folk.GetComponent<Savable>().Save(jSONNode2);
			jSONArray[num] = jSONNode2;
			num++;
		}
	}

	public override void Load(JSONNode Node)
	{
		base.Load(Node);
		m_UsageCount = JSONUtils.GetAsInt(Node, "Used", 0);
		m_RepairCountAdded = JSONUtils.GetAsInt(Node, "Repaired", 0);
		JSONArray asArray = Node["Folks"].AsArray;
		if (asArray != null && !asArray.IsNull)
		{
			for (int i = 0; i < asArray.Count; i++)
			{
				JSONNode asObject = asArray[i].AsObject;
				BaseClass baseClass = ObjectTypeList.Instance.CreateObjectFromSaveName(asObject, new Vector3(0f, 0f, 0f), Quaternion.identity);
				if ((bool)baseClass)
				{
					baseClass.GetComponent<Savable>().Load(asObject);
					AddFolk(baseClass.GetComponent<Folk>());
				}
			}
		}
		UpdateUsage();
	}

	public override void SendAction(ActionInfo Info)
	{
		base.SendAction(Info);
		ActionType action = Info.m_Action;
		if ((uint)(action - 41) <= 1u && !m_Blueprint && (bool)m_Scaffolding)
		{
			m_Scaffolding.SetActive(value: false);
		}
	}

	public override object GetActionInfo(GetActionInfo Info)
	{
		switch (Info.m_Action)
		{
		case GetAction.GetObjectType:
			return ObjectType.Folk;
		case GetAction.IsDeletable:
			if (m_Folks.Count != 0)
			{
				return false;
			}
			break;
		}
		return base.GetActionInfo(Info);
	}

	public override void Moved()
	{
		base.Moved();
		for (int i = 0; i < m_Folks.Count; i++)
		{
			Folk folk = m_Folks[i];
			if (i == m_Folks.Count - 1)
			{
				folk.transform.position = m_HeadPoint.position;
			}
			else
			{
				folk.transform.position = m_DoorPoint.position;
			}
			folk.transform.rotation = base.transform.rotation;
		}
	}

	protected virtual void UpdateUsage()
	{
		bool flag = m_UsageCount >= m_MaxUsageCount;
		m_Dead.SetActive(flag);
		MeshRenderer[] componentsInChildren = m_Dead.GetComponentsInChildren<MeshRenderer>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].enabled = true;
		}
		foreach (MeshRenderer normalMesh in m_NormalMeshes)
		{
			normalMesh.enabled = !flag;
		}
	}

	public void Use(int Amount)
	{
		m_UsageCount += Amount;
		if (m_UsageCount >= m_MaxUsageCount)
		{
			m_UsageCount = m_MaxUsageCount;
			UpdateUsage();
			int num = m_BottomRight.x - m_TopLeft.x;
			int num2 = m_BottomRight.y - m_TopLeft.y;
			Vector3 localPosition = base.transform.position + new Vector3((float)num * Tile.m_Size / 2f, 1f, (float)num2 * Tile.m_Size / 2f);
			float num3 = num + 1;
			MyParticles myParticles = ParticlesManager.Instance.CreateParticles("HouseDestroyed", localPosition, Quaternion.Euler(90f, 0f, 0f));
			ParticlesManager.Instance.DestroyParticles(myParticles, WaitUntilNoParticles: true);
			myParticles.transform.localScale = new Vector3(num3, num3, num3);
			AudioManager.Instance.StartEvent("HouseDestroyed", this);
			ModManager.Instance.CheckBuildingStateChangedCallback(m_UniqueID, "Destroyed");
		}
	}

	public float GetUsed()
	{
		return (float)m_UsageCount / (float)m_MaxUsageCount;
	}

	public virtual void ReleaseFolk(Folk NewFolk)
	{
		NewFolk.GetComponent<Actionable>().SendAction(new ActionInfo(ActionType.Dropped, default(TileCoord), this));
		NewFolk.Homeless();
		m_Folks.Remove(NewFolk);
		if (m_Folks.Count > 0)
		{
			m_Folks[m_Folks.Count - 1].Show(Show: true);
		}
		if (m_Folks.Count == 0)
		{
			m_StatusIndicator.gameObject.SetActive(value: false);
		}
	}

	public virtual void AddFolk(Folk NewFolk)
	{
		if (m_Folks.Count > 0)
		{
			m_Folks[m_Folks.Count - 1].transform.position = m_DoorPoint.position;
			m_Folks[m_Folks.Count - 1].Show(Show: false);
		}
		m_Folks.Add(NewFolk);
		NewFolk.GetComponent<Actionable>().SendAction(new ActionInfo(ActionType.BeingHeld, default(TileCoord), this));
		NewFolk.Housed(this);
		NewFolk.transform.position = m_HeadPoint.position;
		NewFolk.transform.rotation = base.transform.rotation;
		m_StatusIndicator.gameObject.SetActive(value: true);
	}

	private void StartRemoveFolk(AFO Info)
	{
		Folk folk = m_Folks[m_Folks.Count - 1];
		ReleaseFolk(folk);
		AddAnimationManager.Instance.Add(this, Adding: false);
		Info.m_Actioner.GetComponent<Farmer>().m_FarmerCarry.AddTempCarry(folk);
		if (m_Folks.Count > 0)
		{
			Folk folk2 = m_Folks[m_Folks.Count - 1];
			folk2.Show(Show: true);
			folk2.transform.position = m_DoorPoint.position;
			SpawnAnimationManager.Instance.AddJump(folk2, m_DoorPoint.position, m_HeadPoint.position, 0f);
		}
	}

	private void EndRemoveFolk(AFO Info)
	{
		Info.m_Actioner.GetComponent<Farmer>().m_FarmerCarry.GetTempObject();
	}

	private void AbortRemoveFolk(AFO Info)
	{
		if (m_Folks.Count > 0)
		{
			m_Folks[m_Folks.Count - 1].transform.position = m_DoorPoint.position;
		}
		Holdable tempObject = Info.m_Actioner.GetComponent<Farmer>().m_FarmerCarry.GetTempObject();
		if ((bool)tempObject && (bool)tempObject.GetComponent<Folk>())
		{
			AddFolk(tempObject.GetComponent<Folk>());
		}
	}

	private ActionType GetActionFromNothing(AFO Info)
	{
		Info.m_StartAction = StartRemoveFolk;
		Info.m_EndAction = EndRemoveFolk;
		Info.m_AbortAction = AbortRemoveFolk;
		Info.m_FarmerState = Farmer.State.Taking;
		if (m_Folks.Count == 0)
		{
			return ActionType.Fail;
		}
		return ActionType.TakeResource;
	}

	private void StartAddFolk(AFO Info)
	{
		Folk component = Info.m_Object.GetComponent<Folk>();
		AddFolk(component);
		if (m_Folks.Count > 1)
		{
			Folk folk = m_Folks[m_Folks.Count - 2];
			folk.transform.position = m_HeadPoint.position;
			folk.Show(Show: true);
			SpawnAnimationManager.Instance.AddJump(folk, m_HeadPoint.position, m_DoorPoint.position, 0f);
		}
	}

	private void EndAddFolk(AFO Info)
	{
		if (m_Folks.Count > 1)
		{
			m_Folks[m_Folks.Count - 2].Show(Show: false);
		}
		FolkManager.Instance.StartUpdateHoused();
	}

	private void AbortAddFolk(AFO Info)
	{
		if (m_Folks.Count > 1)
		{
			m_Folks[m_Folks.Count - 2].transform.position = m_HeadPoint.position;
		}
		Actionable @object = Info.m_Object;
		ReleaseFolk(@object.GetComponent<Folk>());
	}

	private ActionType GetActionFromFolk(AFO Info)
	{
		Info.m_StartAction = StartAddFolk;
		Info.m_EndAction = EndAddFolk;
		Info.m_AbortAction = AbortAddFolk;
		Info.m_FarmerState = Farmer.State.Adding;
		if (m_Folks.Count == m_MaxFolks)
		{
			return ActionType.Fail;
		}
		return ActionType.AddResource;
	}

	private Folk FindBestFolk(AFO Info)
	{
		if (m_Folks.Count == 0)
		{
			return null;
		}
		AFO info = new AFO(Info);
		ActionType actionFromObjectSafe = m_Folks[0].GetActionFromObjectSafe(info);
		if (actionFromObjectSafe == ActionType.AddResource || actionFromObjectSafe == ActionType.TakeResource)
		{
			return m_Folks[0];
		}
		return null;
	}

	private void StartAddFolkObject(AFO Info)
	{
		m_BestFolk = FindBestFolk(Info);
		if ((bool)m_BestFolk)
		{
			m_BestFolk.StartAction(Info.m_Object, Info.m_Actioner, Info.m_ActionType, Info.m_Position);
		}
	}

	private void EndAddFolkObject(AFO Info)
	{
		if ((bool)m_BestFolk)
		{
			m_BestFolk.EndAction(Info.m_Object, Info.m_Actioner, Info.m_ActionType, Info.m_Position);
		}
	}

	private void AbortAddFolkObject(AFO Info)
	{
		if ((bool)m_BestFolk)
		{
			m_BestFolk.AbortAction(Info.m_Object, Info.m_Actioner, Info.m_ActionType, Info.m_Position);
		}
	}

	private ActionType GetFolkActionFromObject(AFO Info)
	{
		Info.m_StartAction = StartAddFolkObject;
		Info.m_EndAction = EndAddFolkObject;
		Info.m_AbortAction = AbortAddFolkObject;
		Info.m_FarmerState = Farmer.State.Adding;
		if (m_Folks.Count == 0)
		{
			return ActionType.Fail;
		}
		string requirementsOut = "";
		if (Food.GetIsTypeFood(Info) || Food.GetIsTypeFood(Info.m_ObjectType))
		{
			requirementsOut = "FNOHouseFood";
		}
		if (Top.GetIsTypeTop(Info.m_ObjectType))
		{
			requirementsOut = "FNOHouseClothes";
		}
		if (Toy.GetIsTypeToy(Info.m_ObjectType))
		{
			requirementsOut = "FNOHouseToy";
		}
		if (Medicine.GetIsTypeMedicine(Info.m_ObjectType))
		{
			requirementsOut = "FNOHouseMedicine";
		}
		if (Education.GetIsTypeEducation(Info.m_ObjectType))
		{
			requirementsOut = "FNOHouseEducation";
		}
		if (Art.GetIsTypeArt(Info.m_ObjectType))
		{
			requirementsOut = "FNOHouseArt";
		}
		Info.m_RequirementsOut = requirementsOut;
		if ((bool)FindBestFolk(Info) && (Info.m_RequirementsIn == "" || Info.m_RequirementsIn == Info.m_RequirementsOut))
		{
			return ActionType.AddResource;
		}
		Info.m_RequirementsOut = "";
		return ActionType.Fail;
	}

	private void StartTakeFolkObject(AFO Info)
	{
		m_BestFolk = FindBestFolk(Info);
		if ((bool)m_BestFolk)
		{
			m_BestFolk.StartAction(Info.m_Object, Info.m_Actioner, Info.m_ActionType, Info.m_Position);
		}
	}

	private void EndTakeFolkObject(AFO Info)
	{
		if ((bool)m_BestFolk)
		{
			m_BestFolk.EndAction(Info.m_Object, Info.m_Actioner, Info.m_ActionType, Info.m_Position);
		}
	}

	private void AbortTakeFolkObject(AFO Info)
	{
		if ((bool)m_BestFolk)
		{
			m_BestFolk.AbortAction(Info.m_Object, Info.m_Actioner, Info.m_ActionType, Info.m_Position);
		}
	}

	private ActionType GetFolkActionFromTakeObject(AFO Info)
	{
		Info.m_StartAction = StartTakeFolkObject;
		Info.m_EndAction = EndTakeFolkObject;
		Info.m_AbortAction = AbortTakeFolkObject;
		Info.m_FarmerState = Farmer.State.Taking;
		if (m_Folks.Count == 0)
		{
			return ActionType.Fail;
		}
		if ((bool)FindBestFolk(Info))
		{
			return ActionType.TakeResource;
		}
		Info.m_RequirementsOut = "";
		return ActionType.Fail;
	}

	private void StartAddGetActionFromRepairObject(AFO Info)
	{
		m_RepairCountAdded++;
		if (m_RepairCountAdded == GetRepairAmountRequired())
		{
			m_UsageCount = 0;
			m_RepairCountAdded = 0;
			UpdateUsage();
		}
	}

	private void EndAddGetActionFromRepairObject(AFO Info)
	{
		Info.m_Object.StopUsing();
		if (m_UsageCount == 0)
		{
			int num = m_BottomRight.x - m_TopLeft.x;
			int num2 = m_BottomRight.y - m_TopLeft.y;
			Vector3 localPosition = base.transform.position + new Vector3((float)num * Tile.m_Size / 2f, 1f, (float)num2 * Tile.m_Size / 2f);
			float num3 = num + 1;
			MyParticles myParticles = ParticlesManager.Instance.CreateParticles("HouseRepaired", localPosition, Quaternion.Euler(90f, 0f, 0f));
			ParticlesManager.Instance.DestroyParticles(myParticles, WaitUntilNoParticles: true);
			myParticles.transform.localScale = new Vector3(num3, num3, num3);
			AudioManager.Instance.StartEvent("HouseRepaired", this);
			ModManager.Instance.CheckBuildingStateChangedCallback(m_UniqueID, "Repaired");
		}
	}

	private void AbortAddGetActionFromRepairObject(AFO Info)
	{
		m_RepairCountAdded--;
		m_UsageCount = m_MaxUsageCount;
		UpdateUsage();
	}

	private ActionType GetActionFromRepairObject(AFO Info)
	{
		Info.m_StartAction = StartAddGetActionFromRepairObject;
		Info.m_EndAction = EndAddGetActionFromRepairObject;
		Info.m_AbortAction = AbortAddGetActionFromRepairObject;
		Info.m_FarmerState = Farmer.State.Adding;
		if (m_UsageCount != m_MaxUsageCount)
		{
			return ActionType.Fail;
		}
		if (Info.m_ObjectType != GetRepairTypeRequired())
		{
			return ActionType.Fail;
		}
		return ActionType.AddResource;
	}

	public override ActionType GetActionFromObject(AFO Info)
	{
		ObjectType objectType = Info.m_ObjectType;
		if (Info.m_ActionType == AFO.AT.Primary)
		{
			return GetActionFromNothing(Info);
		}
		if (Info.m_ActionType == AFO.AT.Secondary)
		{
			if (objectType == ObjectType.Folk)
			{
				return GetActionFromFolk(Info);
			}
			if (Folk.GetWillFolkAcceptObjectType(objectType))
			{
				return GetFolkActionFromObject(Info);
			}
			return GetActionFromRepairObject(Info);
		}
		if (Info.m_ActionType == AFO.AT.AltSecondary && Folk.GetWillFolkAcceptObjectType(objectType))
		{
			return GetFolkActionFromObject(Info);
		}
		if (Info.m_ActionType == AFO.AT.AltPrimary)
		{
			return GetFolkActionFromTakeObject(Info);
		}
		return ActionType.Total;
	}

	public bool GetIsUnhappyState(Folk.UnhappyState State)
	{
		foreach (Folk folk in m_Folks)
		{
			if (folk.GetIsUnhappyState(State))
			{
				return true;
			}
		}
		return false;
	}

	public int GetRepairAmountDone()
	{
		return m_RepairCountAdded;
	}

	public int GetRepairAmountRequired()
	{
		return VariableManager.Instance.GetVariableAsInt(m_TypeIdentifier, "RepairAmount");
	}

	public ObjectType GetRepairTypeRequired()
	{
		return (ObjectType)VariableManager.Instance.GetVariableAsInt(m_TypeIdentifier, "RepairObject");
	}

	private void Update()
	{
		if (m_StatusIndicator.gameObject.activeSelf)
		{
			m_StatusIndicator.UpdateIndicator();
		}
	}

	public bool ModAddObject(Actionable ActionObject)
	{
		if (Folk.GetWillFolkAcceptObjectType(ActionObject.m_TypeIdentifier) && m_Folks.Count > 0)
		{
			Folk folk = m_Folks[0];
			if ((bool)folk)
			{
				return folk.ModAddObject(ActionObject);
			}
		}
		return false;
	}

	public bool ModRepairHousing(Actionable ActionObject)
	{
		if (m_UsageCount == 0)
		{
			return false;
		}
		if (ActionObject.m_TypeIdentifier != GetRepairTypeRequired())
		{
			return false;
		}
		m_RepairCountAdded++;
		if (m_RepairCountAdded == GetRepairAmountRequired())
		{
			m_UsageCount = 0;
			m_RepairCountAdded = 0;
			UpdateUsage();
		}
		ActionObject.StopUsing();
		if (m_UsageCount == 0)
		{
			int num = m_BottomRight.x - m_TopLeft.x;
			int num2 = m_BottomRight.y - m_TopLeft.y;
			Vector3 localPosition = base.transform.position + new Vector3((float)num * Tile.m_Size / 2f, 1f, (float)num2 * Tile.m_Size / 2f);
			float num3 = num + 1;
			MyParticles myParticles = ParticlesManager.Instance.CreateParticles("HouseRepaired", localPosition, Quaternion.Euler(90f, 0f, 0f));
			ParticlesManager.Instance.DestroyParticles(myParticles, WaitUntilNoParticles: true);
			myParticles.transform.localScale = new Vector3(num3, num3, num3);
			AudioManager.Instance.StartEvent("HouseRepaired", this);
			ModManager.Instance.CheckBuildingStateChangedCallback(m_UniqueID, "Repaired");
		}
		return true;
	}
}
