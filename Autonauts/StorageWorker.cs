using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;

public class StorageWorker : Storage
{
	private List<Worker> m_Workers;

	private Transform m_Hinge;

	private Worker m_TempBot;

	public override void Restart()
	{
		base.Restart();
		SetDimensions(new TileCoord(0, -1), new TileCoord(0, 0), new TileCoord(0, 1));
		SetObjectType(ObjectType.Worker);
	}

	protected new void Awake()
	{
		base.Awake();
		m_Capacity = 10;
		m_Workers = new List<Worker>();
	}

	public override void PostCreate()
	{
		base.PostCreate();
		m_Hinge = m_ModelRoot.transform.Find("DoorHinges");
		m_Sign = m_ModelRoot.transform.Find("Plane").GetComponent<MeshRenderer>();
	}

	public override void Save(JSONNode Node)
	{
		base.Save(Node);
		JSONArray jSONArray = (JSONArray)(Node["Bots"] = new JSONArray());
		int num = 0;
		foreach (Worker worker in m_Workers)
		{
			JSONNode jSONNode2 = new JSONObject();
			string saveNameFromIdentifier = ObjectTypeList.Instance.GetSaveNameFromIdentifier(worker.m_TypeIdentifier);
			JSONUtils.Set(jSONNode2, "ID", saveNameFromIdentifier);
			worker.Save(jSONNode2);
			jSONArray[num] = jSONNode2;
			num++;
		}
	}

	public override void Load(JSONNode Node)
	{
		base.Load(Node);
		JSONArray asArray = Node["Bots"].AsArray;
		for (int i = 0; i < asArray.Count; i++)
		{
			JSONNode asObject = asArray[i].AsObject;
			BaseClass baseClass = ObjectTypeList.Instance.CreateObjectFromSaveName(asObject, new Vector3(0f, 0f, 0f), Quaternion.identity);
			if ((bool)baseClass)
			{
				Worker component = baseClass.GetComponent<Worker>();
				component.GetComponent<Savable>().Load(asObject);
				component.GetComponent<Holdable>().UpdatePositionToTilePosition(m_TileCoord);
				component.SendAction(new ActionInfo(ActionType.BeingHeld, default(TileCoord), this));
				component.SendAction(new ActionInfo(ActionType.Stowed, default(TileCoord), this));
				component.gameObject.SetActive(value: false);
				component.RemoveFromWorld();
				m_Workers.Add(component);
			}
		}
		m_Stored = m_Workers.Count;
		m_TempStored = m_Stored;
		UpdateStored();
	}

	public override void StopUsing(bool AndDestroy = true)
	{
		base.StopUsing(AndDestroy);
	}

	public override void SetObjectType(ObjectType NewType)
	{
		base.SetObjectType(NewType);
		if (m_ObjectType != ObjectTypeList.m_Total)
		{
			ResourceManager.Instance.UnRegisterStorage(this);
		}
		m_ObjectType = NewType;
		SetSign(NewType);
	}

	private void StartAddAnything(AFO Info)
	{
		Worker component = Info.m_Object.GetComponent<Worker>();
		component.transform.position = m_ModelRoot.transform.position;
		if (m_ObjectType == ObjectTypeList.m_Total)
		{
			SetObjectType(component.m_TypeIdentifier);
		}
		component.m_FarmerCarry.DropAllObjects();
		component.m_FarmerInventory.DropAllObjects();
		component.RemoveFromWorld();
		m_Workers.Add(component);
		m_Stored = m_Workers.Count;
		StartOpenLid();
	}

	private void EndAddAnything(AFO Info)
	{
		Actionable @object = Info.m_Object;
		AudioManager.Instance.StartEvent("BuildingStorageAdd", this);
		@object.SendAction(new ActionInfo(ActionType.BeingHeld, default(TileCoord), this));
		@object.SendAction(new ActionInfo(ActionType.Stowed, default(TileCoord), this));
		@object.gameObject.SetActive(value: false);
		UpdateStored();
	}

	private void AbortAddAnything(AFO Info)
	{
		_ = m_Workers[m_Workers.Count - 1];
		m_Workers.RemoveAt(m_Workers.Count - 1);
		m_Stored = m_Workers.Count;
	}

	private void StartRelease(AFO Info)
	{
		AudioManager.Instance.StartEvent("BuildingStorageTake", this);
		Worker worker = m_Workers[m_Workers.Count - 1];
		m_Workers.Remove(worker);
		m_Stored = m_Workers.Count;
		worker.SendAction(new ActionInfo(ActionType.Recalled, default(TileCoord), this));
		worker.gameObject.SetActive(value: true);
		worker.AddToWorld();
		m_TempBot = worker;
		Info.m_Actioner.GetComponent<Farmer>().m_FarmerCarry.AddTempCarry(worker);
		StartOpenLid();
		UpdateStored();
	}

	private void AbortRelease(AFO Info)
	{
		if ((bool)m_TempBot)
		{
			m_Workers.Add(m_TempBot);
			m_Stored = m_Workers.Count;
			m_TempBot.SendAction(new ActionInfo(ActionType.BeingHeld, default(TileCoord), this));
			m_TempBot.SendAction(new ActionInfo(ActionType.Stowed, default(TileCoord), this));
			m_TempBot.gameObject.SetActive(value: false);
		}
	}

	private bool CanReleaseObject()
	{
		if (m_ObjectType == ObjectTypeList.m_Total)
		{
			return false;
		}
		return GetStored() > 0;
	}

	public override ActionType GetActionFromObject(AFO Info)
	{
		if (Info.m_ActionType == AFO.AT.Primary)
		{
			Info.m_StartAction = StartRelease;
			Info.m_AbortAction = AbortRelease;
			Info.m_FarmerState = Farmer.State.Taking;
			if (CanReleaseObject())
			{
				return ActionType.TakeResource;
			}
			return ActionType.Fail;
		}
		if (Info.m_ActionType == AFO.AT.Secondary)
		{
			Info.m_StartAction = StartAddAnything;
			Info.m_EndAction = EndAddAnything;
			Info.m_AbortAction = AbortAddAnything;
			Info.m_FarmerState = Farmer.State.Adding;
			if (Info.m_ObjectType == ObjectType.Worker && CanAcceptObject(Info.m_Object, Info.m_ObjectType))
			{
				return ActionType.AddResource;
			}
			return ActionType.Fail;
		}
		return ActionType.Total;
	}

	protected override void Update()
	{
		base.Update();
	}

	public override void StartOpenLid()
	{
		base.StartOpenLid();
		m_Hinge.localRotation = Quaternion.Euler(-90f, 0f, 270f);
	}

	public override void CloseLid()
	{
		base.CloseLid();
		m_Hinge.localRotation = Quaternion.Euler(-90f, 0f, 180f);
	}

	public void ModAddBot(Worker NewObject)
	{
		NewObject.transform.position = m_ModelRoot.transform.position;
		if (m_ObjectType == ObjectTypeList.m_Total)
		{
			SetObjectType(NewObject.m_TypeIdentifier);
		}
		NewObject.m_FarmerCarry.DropAllObjects();
		NewObject.m_FarmerInventory.DropAllObjects();
		NewObject.RemoveFromWorld();
		m_Workers.Add(NewObject);
		m_Stored = m_Workers.Count;
		AudioManager.Instance.StartEvent("BuildingStorageAdd", this);
		NewObject.SendAction(new ActionInfo(ActionType.BeingHeld, default(TileCoord), this));
		NewObject.SendAction(new ActionInfo(ActionType.Stowed, default(TileCoord), this));
		NewObject.gameObject.SetActive(value: false);
		UpdateStored();
		ModManager.Instance.CheckStorageAddedCallback(m_UniqueID);
	}

	public Worker ModReleaseBot(TileCoord Pos)
	{
		AudioManager.Instance.StartEvent("BuildingStorageTake", this);
		Worker worker = m_Workers[m_Workers.Count - 1];
		m_Workers.Remove(worker);
		m_Stored = m_Workers.Count;
		worker.SendAction(new ActionInfo(ActionType.Recalled, default(TileCoord), this));
		worker.gameObject.SetActive(value: true);
		worker.AddToWorld();
		worker.SendAction(new ActionInfo(ActionType.Dropped, default(TileCoord), this));
		SpawnAnimationManager.Instance.AddJump(worker, m_TileCoord, Pos, 0f, worker.transform.position.y, 4f, 0.2f, null, DustLand: false, this);
		UpdateStored();
		ModManager.Instance.CheckStorageRemovedCallback(m_UniqueID);
		return worker;
	}
}
