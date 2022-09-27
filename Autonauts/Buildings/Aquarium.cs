using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;

public class Aquarium : Building
{
	private List<Fish> m_Fish;

	private int m_Capacity;

	public static bool GetIsTypeAquiarium(ObjectType NewType)
	{
		if (NewType == ObjectType.Aquarium || NewType == ObjectType.AquariumGood)
		{
			return true;
		}
		return false;
	}

	public override void Restart()
	{
		base.Restart();
		SetDimensions(new TileCoord(-1, -2), new TileCoord(1, 0), new TileCoord(0, 1));
		m_Fish = new List<Fish>();
		m_Capacity = VariableManager.Instance.GetVariableAsInt(m_TypeIdentifier, "Capacity");
	}

	public override void PostCreate()
	{
		base.PostCreate();
	}

	public override void Save(JSONNode Node)
	{
		base.Save(Node);
		JSONArray jSONArray = (JSONArray)(Node["Fish"] = new JSONArray());
		int num = 0;
		for (int i = 0; i < m_Fish.Count; i++)
		{
			JSONNode jSONNode2 = new JSONObject();
			string saveNameFromIdentifier = ObjectTypeList.Instance.GetSaveNameFromIdentifier(m_Fish[i].GetComponent<BaseClass>().m_TypeIdentifier);
			JSONUtils.Set(jSONNode2, "ID", saveNameFromIdentifier);
			m_Fish[i].GetComponent<Savable>().Save(jSONNode2);
			jSONArray[num] = jSONNode2;
			num++;
		}
	}

	public override void Load(JSONNode Node)
	{
		base.Load(Node);
		m_Fish.Clear();
		JSONArray asArray = Node["Fish"].AsArray;
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
					AddFish(baseClass.GetComponent<Fish>());
				}
			}
		}
	}

	public override void SendAction(ActionInfo Info)
	{
		base.SendAction(Info);
		if ((bool)m_Engager && Info.m_Action != ActionType.Disengaged)
		{
			return;
		}
		switch (Info.m_Action)
		{
		case ActionType.Engaged:
			if (GameStateManager.Instance.GetActualState() == GameStateManager.State.Normal && (bool)Info.m_Object.GetComponent<FarmerPlayer>())
			{
				m_Engager = Info.m_Object;
				GameStateManager.Instance.StartAquarium(this);
			}
			break;
		case ActionType.Disengaged:
			m_Engager = null;
			break;
		}
	}

	public override bool CanDoAction(ActionInfo Info, bool RightNow)
	{
		if (RightNow)
		{
			return false;
		}
		if ((bool)m_Engager && Info.m_Action != ActionType.Disengaged)
		{
			return false;
		}
		switch (Info.m_Action)
		{
		case ActionType.Engaged:
			if (GameStateManager.Instance.GetActualState() == GameStateManager.State.Normal && m_Engager == null)
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
			return false;
		}
	}

	public override object GetActionInfo(GetActionInfo Info)
	{
		switch (Info.m_Action)
		{
		case GetAction.IsDeletable:
			if (m_Engager != null)
			{
				return false;
			}
			return m_Fish.Count == 0;
		case GetAction.IsMovable:
			return !m_DoingAction;
		case GetAction.IsBusy:
			return m_DoingAction;
		default:
			return base.GetActionInfo(Info);
		}
	}

	public int GetStored()
	{
		return m_Fish.Count;
	}

	public int GetCapacity()
	{
		return m_Capacity;
	}

	public bool CanAdd(Holdable NewObject)
	{
		if (!Fish.GetIsTypeFish(NewObject.m_TypeIdentifier))
		{
			return false;
		}
		return m_Fish.Count < m_Capacity;
	}

	public Holdable ReleaseObject(int Index)
	{
		return ReleaseFish(Index);
	}

	public void AttemptAdd(Holdable NewObject)
	{
		AddFish(NewObject.GetComponent<Fish>());
	}

	public Holdable GetObject(int Index)
	{
		if (m_Fish.Count <= Index)
		{
			return null;
		}
		return m_Fish[Index];
	}

	private void AddFish(Fish NewFish)
	{
		NewFish.SendAction(new ActionInfo(ActionType.BeingHeld, m_TileCoord, this));
		m_Fish.Add(NewFish);
	}

	private Fish ReleaseFish(int Index)
	{
		Fish fish = m_Fish[Index];
		m_Fish.RemoveAt(Index);
		fish.SendAction(new ActionInfo(ActionType.Dropped, m_TileCoord, this));
		return fish;
	}

	private void StartActionFromFish(AFO Info)
	{
		if (!(Info.m_Object == null))
		{
			AddAnimationManager.Instance.Add(this, Adding: true);
			AudioManager.Instance.StartEvent("BuildingStorageAdd", this);
		}
	}

	private void EndActionFromFish(AFO Info)
	{
		Fish component = Info.m_Object.GetComponent<Fish>();
		if ((bool)component)
		{
			AddFish(component);
		}
	}

	private void AbortActionFromFish(AFO Info)
	{
	}

	private ActionType GetActionFromFish(AFO Info)
	{
		Info.m_StartAction = StartActionFromFish;
		Info.m_EndAction = EndActionFromFish;
		Info.m_AbortAction = AbortActionFromFish;
		Info.m_FarmerState = Farmer.State.Adding;
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
			Info.m_FarmerState = Farmer.State.Engaged;
			if (GameStateManager.Instance.GetActualState() == GameStateManager.State.Normal && m_Engager == null)
			{
				return ActionType.EngageObject;
			}
			return ActionType.Fail;
		}
		if (Info.m_ActionType == AFO.AT.Secondary && Fish.GetIsTypeFish(Info.m_ObjectType))
		{
			return GetActionFromFish(Info);
		}
		return ActionType.Total;
	}
}
