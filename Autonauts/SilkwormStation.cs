using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;

public class SilkwormStation : Building
{
	private AnimalSilkworm[] m_Silkworms;

	private int m_Capacity;

	private List<AnimalSilkmoth> m_Silkmoths;

	private int m_Fuel;

	private int m_FuelCapacity;

	private AnimalSilkworm m_TakingWorm;

	public override void Restart()
	{
		base.Restart();
		if (!ObjectTypeList.m_Loading)
		{
			CollectionManager.Instance.AddCollectable("SilkwormStation", this);
		}
		SetDimensions(new TileCoord(0, -1), new TileCoord(1, 0), new TileCoord(0, 1));
		m_Silkworms = new AnimalSilkworm[m_Capacity];
		m_Silkmoths = new List<AnimalSilkmoth>();
		m_Fuel = 0;
	}

	public override void PostCreate()
	{
		base.PostCreate();
		m_Capacity = VariableManager.Instance.GetVariableAsInt(ObjectType.SilkwormStation, "Capacity");
		m_FuelCapacity = VariableManager.Instance.GetVariableAsInt(ObjectType.SilkwormStation, "FuelCapacity");
	}

	public override void StopUsing(bool AndDestroy = true)
	{
		foreach (AnimalSilkmoth silkmoth in m_Silkmoths)
		{
			silkmoth.ClearTarget(Deregister: false);
		}
		m_Silkmoths.Clear();
		base.StopUsing(AndDestroy);
	}

	public override void Save(JSONNode Node)
	{
		base.Save(Node);
		JSONUtils.Set(Node, "Fuel", m_Fuel);
		JSONArray jSONArray = (JSONArray)(Node["Silkworms"] = new JSONArray());
		int num = 0;
		for (int i = 0; i < m_Silkworms.Length; i++)
		{
			if (m_Silkworms[i] != null)
			{
				JSONNode jSONNode2 = new JSONObject();
				string saveNameFromIdentifier = ObjectTypeList.Instance.GetSaveNameFromIdentifier(m_Silkworms[i].GetComponent<BaseClass>().m_TypeIdentifier);
				JSONUtils.Set(jSONNode2, "ID", saveNameFromIdentifier);
				m_Silkworms[i].GetComponent<Savable>().Save(jSONNode2);
				jSONArray[num] = jSONNode2;
				num++;
			}
		}
	}

	public override void Load(JSONNode Node)
	{
		base.Load(Node);
		CollectionManager.Instance.AddCollectable("SilkwormStation", this);
		m_Fuel = JSONUtils.GetAsInt(Node, "Fuel", 0);
		m_Silkworms = new AnimalSilkworm[m_Capacity];
		JSONArray asArray = Node["Silkworms"].AsArray;
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
					AnimalSilkworm component = baseClass.GetComponent<AnimalSilkworm>();
					AddSilkworm(component);
				}
			}
		}
	}

	public int GetStored()
	{
		int num = 0;
		for (int i = 0; i < m_Silkworms.Length; i++)
		{
			if (m_Silkworms[i] != null)
			{
				num++;
			}
		}
		return num;
	}

	public int GetCapacity()
	{
		return m_Capacity;
	}

	public int GetSilkCount()
	{
		int num = 0;
		for (int i = 0; i < m_Silkworms.Length; i++)
		{
			AnimalSilkworm animalSilkworm = m_Silkworms[i];
			if (animalSilkworm != null && animalSilkworm.m_State == AnimalSilkworm.State.StationConverted)
			{
				num++;
			}
		}
		return num;
	}

	private Vector3 GetSilkwormPosition(int Index)
	{
		float num = 1.1f;
		float num2 = 2f;
		int num3 = 5;
		float x = (float)(Index % num3) * num - 0.6f;
		float z = (float)(Index / num3) * num2;
		return new Vector3(x, 1f, z);
	}

	public int GetFuel()
	{
		return m_Fuel;
	}

	public float GetFuelPercent()
	{
		return (float)m_Fuel / (float)m_FuelCapacity;
	}

	private int GetFreeWormIndex()
	{
		for (int i = 0; i < m_Silkworms.Length; i++)
		{
			if (m_Silkworms[i] == null)
			{
				return i;
			}
		}
		return -1;
	}

	private void AddSilkworm(AnimalSilkworm NewWorm)
	{
		int freeWormIndex = GetFreeWormIndex();
		Vector3 silkwormPosition = GetSilkwormPosition(freeWormIndex);
		m_Silkworms[freeWormIndex] = NewWorm;
		NewWorm.SetIsSavable(IsSavable: false);
		NewWorm.SendAction(new ActionInfo(ActionType.BeingHeld, m_TileCoord));
		NewWorm.transform.parent = base.transform;
		NewWorm.transform.localPosition = silkwormPosition;
		NewWorm.transform.localRotation = Quaternion.Euler(0f, -90f, 0f) * Quaternion.Euler(0f, 90f, 0f);
	}

	private void RemoveSilkworm(AnimalSilkworm NewWorm)
	{
		for (int i = 0; i < m_Silkworms.Length; i++)
		{
			if (m_Silkworms[i] == NewWorm)
			{
				m_Silkworms[i] = null;
				break;
			}
		}
		NewWorm.SendAction(new ActionInfo(ActionType.Dropped, m_TileCoord));
		NewWorm.SetIsSavable(IsSavable: true);
	}

	private void CheckSilkworms()
	{
		for (int i = 0; i < m_Silkworms.Length; i++)
		{
			AnimalSilkworm animalSilkworm = m_Silkworms[i];
			if (animalSilkworm != null && m_Fuel > 0 && animalSilkworm.m_State == AnimalSilkworm.State.StationIdle)
			{
				m_Fuel--;
				animalSilkworm.SetState(AnimalSilkworm.State.StationConverting);
			}
		}
	}

	private AnimalSilkworm GetConvertedSilk()
	{
		for (int i = 0; i < m_Silkworms.Length; i++)
		{
			AnimalSilkworm animalSilkworm = m_Silkworms[i];
			if (animalSilkworm != null && animalSilkworm.m_State == AnimalSilkworm.State.StationConverted)
			{
				return animalSilkworm;
			}
		}
		return null;
	}

	public bool CanAddSilkmoth()
	{
		return m_Silkmoths.Count < 6;
	}

	public void AddSilkmoth(AnimalSilkmoth NewSilkmoth)
	{
		m_Silkmoths.Add(NewSilkmoth);
	}

	public void RemoveSilkmoth(AnimalSilkmoth NewSilkmoth)
	{
		m_Silkmoths.Remove(NewSilkmoth);
	}

	public override object GetActionInfo(GetActionInfo Info)
	{
		if (Info.m_Action == GetAction.GetObjectType)
		{
			return ObjectType.SilkRaw;
		}
		return base.GetActionInfo(Info);
	}

	private void StartActionFromSilkworm(AFO Info)
	{
		if (!(Info.m_Object == null))
		{
			AnimalSilkworm component = Info.m_Object.GetComponent<AnimalSilkworm>();
			if ((bool)component)
			{
				AddSilkworm(component);
			}
			AddAnimationManager.Instance.Add(this, Adding: true);
			AudioManager.Instance.StartEvent("BuildingStorageAdd", this);
		}
	}

	private void EndActionFromSilkworm(AFO Info)
	{
		AnimalSilkworm component = Info.m_Object.GetComponent<AnimalSilkworm>();
		if ((bool)component)
		{
			component.SetState(AnimalSilkworm.State.StationIdle);
		}
		CheckSilkworms();
	}

	private void AbortActionFromSilkworm(AFO Info)
	{
		AnimalSilkworm component = Info.m_Object.GetComponent<AnimalSilkworm>();
		if ((bool)component)
		{
			RemoveSilkworm(component);
		}
	}

	private ActionType GetActionFromSilkworm(AFO Info)
	{
		Info.m_StartAction = StartActionFromSilkworm;
		Info.m_EndAction = EndActionFromSilkworm;
		Info.m_AbortAction = AbortActionFromSilkworm;
		Info.m_FarmerState = Farmer.State.Adding;
		if (!GetAreRequirementsMet())
		{
			return ActionType.Fail;
		}
		if (GetStored() >= GetCapacity())
		{
			return ActionType.Fail;
		}
		return ActionType.AddResource;
	}

	private void StartActionFromFuel(AFO Info)
	{
		if (!(Info.m_Object == null))
		{
			m_Fuel++;
			AddAnimationManager.Instance.Add(this, Adding: true);
			AudioManager.Instance.StartEvent("BuildingStorageAdd", this);
		}
	}

	private void EndActionFromFuel(AFO Info)
	{
		if ((bool)Info.m_Object)
		{
			Info.m_Object.StopUsing();
		}
		CheckSilkworms();
	}

	private void AbortActionFromFuel(AFO Info)
	{
		m_Fuel--;
	}

	private ActionType GetActionFromFuel(AFO Info)
	{
		Info.m_StartAction = StartActionFromFuel;
		Info.m_EndAction = EndActionFromFuel;
		Info.m_AbortAction = AbortActionFromFuel;
		Info.m_FarmerState = Farmer.State.Adding;
		if (!GetAreRequirementsMet())
		{
			return ActionType.Fail;
		}
		if (m_Fuel >= m_FuelCapacity)
		{
			return ActionType.Fail;
		}
		return ActionType.AddResource;
	}

	private void StartActionFromNothing(AFO Info)
	{
		AddAnimationManager.Instance.Add(this, Adding: false);
		Info.m_Actioner.GetComponent<Farmer>().m_FarmerCarry.AddTempCarry(ObjectType.SilkRaw);
		m_TakingWorm = GetConvertedSilk();
		m_TakingWorm.SetState(AnimalSilkworm.State.StationTaking);
	}

	private void EndActionFromNothing(AFO Info)
	{
		RemoveSilkworm(m_TakingWorm);
		ObjectTypeList.Instance.CreateObjectFromIdentifier(ObjectType.AnimalSilkmoth, m_TakingWorm.transform.position, Quaternion.identity);
		m_TakingWorm.StopUsing();
		m_TakingWorm = null;
		QuestManager.Instance.AddEvent(QuestEvent.Type.Make, Bot: false, ObjectType.SilkRaw, this);
	}

	private void AbortActionFromNothing(AFO Info)
	{
		Info.m_Actioner.GetComponent<Farmer>().m_FarmerCarry.RemoveTempCarry();
		m_TakingWorm.SetState(AnimalSilkworm.State.StationConverted);
		m_TakingWorm = null;
	}

	private ActionType GetActionFromNothing(AFO Info)
	{
		Info.m_StartAction = StartActionFromNothing;
		Info.m_EndAction = EndActionFromNothing;
		Info.m_AbortAction = AbortActionFromNothing;
		Info.m_FarmerState = Farmer.State.Taking;
		if (!GetAreRequirementsMet())
		{
			return ActionType.Fail;
		}
		if (GetConvertedSilk() == null)
		{
			return ActionType.Fail;
		}
		return ActionType.TakeResource;
	}

	public override ActionType GetActionFromObject(AFO Info)
	{
		if (Info.m_ActionType == AFO.AT.Primary)
		{
			return GetActionFromNothing(Info);
		}
		if (Info.m_ActionType == AFO.AT.Secondary)
		{
			if (Info.m_ObjectType == ObjectType.AnimalSilkworm)
			{
				return GetActionFromSilkworm(Info);
			}
			if (Info.m_ObjectType == ObjectType.MulberrySeed)
			{
				return GetActionFromFuel(Info);
			}
		}
		return ActionType.Total;
	}
}
