using SimpleJSON;
using UnityEngine;

public class AnimalStation : Building
{
	protected int m_Slots;

	protected AnimalGrazer[] m_Animals;

	protected ObjectType m_RequiredAnimalType;

	public override void Restart()
	{
		base.Restart();
		SetDimensions(new TileCoord(-1, -2), new TileCoord(1, 0), new TileCoord(0, 1));
		m_Slots = 3;
		m_Animals = new AnimalGrazer[m_Slots];
		m_RequiredAnimalType = ObjectType.AnimalCow;
	}

	public override void StopUsing(bool AndDestroy = true)
	{
		for (int i = 0; i < m_Animals.Length; i++)
		{
			if ((bool)m_Animals[i])
			{
				ReleaseAnimal(i);
			}
		}
		base.StopUsing(AndDestroy);
	}

	public override void Save(JSONNode Node)
	{
		base.Save(Node);
		JSONArray asArray = Node["Animals"].AsArray;
		int num = 0;
		for (int i = 0; i < m_Slots; i++)
		{
			AnimalGrazer animalGrazer = m_Animals[i];
			if ((bool)animalGrazer)
			{
				JSONNode asObject = asArray[num].AsObject;
				JSONUtils.Set(asObject, "Slot", i);
				string saveNameFromIdentifier = ObjectTypeList.Instance.GetSaveNameFromIdentifier(animalGrazer.m_TypeIdentifier);
				JSONUtils.Set(asObject, "ID", saveNameFromIdentifier);
				animalGrazer.GetComponent<Savable>().Save(asObject);
				num++;
			}
		}
	}

	public override void Load(JSONNode Node)
	{
		base.Load(Node);
		JSONArray asArray = Node["Animals"].AsArray;
		for (int i = 0; i < asArray.Count; i++)
		{
			JSONNode asObject = asArray[i].AsObject;
			int asInt = JSONUtils.GetAsInt(asObject, "Slot", 0);
			BaseClass baseClass = ObjectTypeList.Instance.CreateObjectFromSaveName(asObject, new Vector3(0f, 0f, 0f), Quaternion.identity);
			baseClass.GetComponent<Savable>().Load(asObject);
			AddAnimal(baseClass.GetComponent<AnimalGrazer>(), asInt);
		}
	}

	public override void SendAction(ActionInfo Info)
	{
		base.SendAction(Info);
		if ((bool)m_Engager && Info.m_Action != ActionType.Disengaged && Info.m_Action != ActionType.SetValue)
		{
			return;
		}
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
		case ActionType.Refresh:
		case ActionType.RefreshFirst:
		{
			for (int i = 0; i < m_Slots; i++)
			{
				AnimalGrazer animalGrazer = m_Animals[i];
				if ((bool)animalGrazer)
				{
					animalGrazer.SendAction(Info);
				}
			}
			break;
		}
		}
	}

	private bool GetIsBeingAddedTo()
	{
		for (int i = 0; i < m_Slots; i++)
		{
			AnimalGrazer animalGrazer = m_Animals[i];
			if ((bool)animalGrazer && SpawnAnimationManager.Instance.GetIsObjectSpawning(animalGrazer))
			{
				return true;
			}
		}
		return false;
	}

	public override object GetActionInfo(GetActionInfo Info)
	{
		switch (Info.m_Action)
		{
		case GetAction.GetObjectType:
			return m_RequiredAnimalType;
		case GetAction.IsDeletable:
		case GetAction.IsMovable:
			return !GetIsBeingAddedTo();
		default:
			return base.GetActionInfo(Info);
		}
	}

	private int GetFreeSlot()
	{
		for (int i = 0; i < m_Slots; i++)
		{
			if (!m_Animals[i])
			{
				return i;
			}
		}
		return -1;
	}

	public bool GetIsFull()
	{
		if (GetFreeSlot() == -1)
		{
			return true;
		}
		return false;
	}

	protected int GetAnimalSlot(AnimalGrazer NewAnimal)
	{
		for (int i = 0; i < m_Slots; i++)
		{
			if (NewAnimal == m_Animals[i])
			{
				return i;
			}
		}
		return -1;
	}

	protected int GetFullAnimal()
	{
		for (int i = 0; i < m_Slots; i++)
		{
			AnimalGrazer animalGrazer = m_Animals[i];
			if ((bool)animalGrazer && !BaggedManager.Instance.IsObjectBagged(animalGrazer) && !SpawnAnimationManager.Instance.GetIsObjectSpawning(animalGrazer))
			{
				return i;
			}
		}
		return -1;
	}

	private int GetNumAnimals()
	{
		int num = 0;
		for (int i = 0; i < m_Slots; i++)
		{
			if ((bool)m_Animals[i])
			{
				num++;
			}
		}
		return num;
	}

	protected virtual void UpdateAnimalPosition(int Slot)
	{
		AnimalGrazer obj = m_Animals[Slot];
		float num = 0f - (float)m_Slots * Tile.m_Size / 2f;
		num += Tile.m_Size * 0.5f;
		num += (float)Slot * Tile.m_Size;
		obj.transform.rotation = Quaternion.identity;
		float z = ObjectUtils.ObjectBounds(obj.gameObject).size.z;
		float num2 = Tile.m_Size * 1.5f + 0.5f;
		num2 -= z * 0.5f;
		obj.transform.position = base.transform.TransformPoint(new Vector3(num, 0f, num2));
		obj.transform.rotation = base.transform.rotation * Quaternion.Euler(0f, 180f, 0f);
	}

	protected virtual Vector3 GetActionPosition(int Slot)
	{
		float num = 0f - (float)m_Slots * Tile.m_Size / 2f;
		num += Tile.m_Size * 0.5f;
		num += (float)Slot * Tile.m_Size;
		return base.transform.TransformPoint(new Vector3(num, 0f, 0f));
	}

	private void AddAnimal(AnimalGrazer NewAnimal, int Slot)
	{
		NewAnimal.transform.SetParent(base.transform);
		m_Animals[Slot] = NewAnimal;
		NewAnimal.SetIsSavable(IsSavable: false);
		UpdateAnimalPosition(Slot);
	}

	protected void ReleaseAnimal(int Index)
	{
		AnimalGrazer animalGrazer = m_Animals[Index];
		if ((bool)animalGrazer)
		{
			Vector3 position = animalGrazer.transform.position;
			animalGrazer.SetIsSavable(IsSavable: true);
			animalGrazer.ForceHighlight(Highlighted: false);
			animalGrazer.transform.SetParent(ObjectTypeList.Instance.GetParentFromIdentifier(animalGrazer.m_TypeIdentifier));
			animalGrazer.transform.position = position;
			Vector3 endPosition = GetAccessPosition().ToWorldPositionTileCentered();
			SpawnAnimationManager.Instance.AddJump(animalGrazer, position, endPosition, 4f);
			m_Animals[Index] = null;
		}
	}

	protected void StopAnimalAction(AnimalGrazer NewAnimal)
	{
		if ((bool)NewAnimal.m_BaggedBy)
		{
			Farmer component = NewAnimal.m_BaggedBy.GetComponent<Farmer>();
			component.SetState(Farmer.State.None);
			component.UpdatePositionToTilePosition(GetAccessPosition());
			component.SetBaggedObject(null);
			NewAnimal.m_BaggedBy = null;
		}
	}

	private bool GetAnyAnimalsBeingActioned()
	{
		for (int i = 0; i < m_Slots; i++)
		{
			if ((bool)m_Animals[i] && (bool)m_Animals[i].m_BaggedBy)
			{
				return true;
			}
		}
		return false;
	}

	private void StartAddAnimal(AFO Info)
	{
		AnimalGrazer component = Info.m_Object.GetComponent<AnimalGrazer>();
		component.SetState(AnimalGrazer.State.EnterBuilding);
		AddAnimal(component, GetFreeSlot());
		m_DoingAction = false;
	}

	private void EndAddAnimal(AFO Info)
	{
		AnimalGrazer component = Info.m_Object.GetComponent<AnimalGrazer>();
		component.enabled = true;
		int animalSlot = GetAnimalSlot(component);
		UpdateAnimalPosition(animalSlot);
	}

	private void AbortAddAnimal(AFO Info)
	{
		AnimalGrazer component = Info.m_Object.GetComponent<AnimalGrazer>();
		int animalSlot = GetAnimalSlot(component);
		m_Animals[animalSlot] = null;
		component.transform.SetParent(ObjectTypeList.Instance.GetParentFromIdentifier(component.m_TypeIdentifier));
		component.SetIsSavable(IsSavable: true);
	}

	private ActionType GetActionFromAnimal(AFO Info)
	{
		Info.m_StartAction = StartAddAnimal;
		Info.m_EndAction = EndAddAnimal;
		Info.m_AbortAction = AbortAddAnimal;
		Info.m_FarmerState = Farmer.State.Adding;
		if (GetFreeSlot() == -1)
		{
			return ActionType.Fail;
		}
		if ((bool)Info.m_Object && !Info.m_Object.GetComponent<AnimalGrazer>().GetIsFull())
		{
			return ActionType.Fail;
		}
		return ActionType.AddResource;
	}

	protected virtual void StartAnimalAction(AFO Info)
	{
		int fullAnimal = GetFullAnimal();
		AnimalGrazer animalGrazer = m_Animals[fullAnimal];
		TileCoord tileCoord = new TileCoord(0, 1);
		tileCoord.Rotate(m_Rotation);
		Info.m_Actioner.transform.position = animalGrazer.transform.position + tileCoord.ToWorldPosition();
		Info.m_Actioner.transform.rotation = base.transform.rotation * Quaternion.Euler(0f, 180f, 0f);
		Info.m_Actioner.GetComponent<Farmer>().SetBaggedObject(animalGrazer);
		animalGrazer.m_BaggedBy = Info.m_Actioner;
		m_DoingAction = false;
	}

	protected virtual void EndAnimalAction(AFO Info)
	{
	}

	protected virtual void AbortAnimalAction(AFO Info)
	{
	}

	private ActionType GetActionFromBucket(AFO Info)
	{
		if (m_RequiredAnimalType != ObjectType.AnimalCow)
		{
			return ActionType.Total;
		}
		Info.m_StartAction = StartAnimalAction;
		Info.m_EndAction = EndAnimalAction;
		Info.m_AbortAction = AbortAnimalAction;
		Info.m_FarmerState = Farmer.State.Milking;
		if (GetFullAnimal() == -1)
		{
			return ActionType.Fail;
		}
		if (Info.m_Object.GetComponent<ToolBucket>().GetIsFull())
		{
			return ActionType.Fail;
		}
		return ActionType.UseInHands;
	}

	private ActionType GetActionFromNothing(AFO Info)
	{
		Info.m_FarmerState = Farmer.State.Engaged;
		if (GameStateManager.Instance.GetActualState() != 0)
		{
			return ActionType.Fail;
		}
		if (m_Engager != null)
		{
			return ActionType.Fail;
		}
		if (GetNumAnimals() == 0)
		{
			return ActionType.Fail;
		}
		if (GetAnyAnimalsBeingActioned())
		{
			return ActionType.Fail;
		}
		return ActionType.EngageObject;
	}

	public override ActionType GetActionFromObject(AFO Info)
	{
		if (Info.m_ActionType == AFO.AT.Primary)
		{
			return GetActionFromNothing(Info);
		}
		if (Info.m_ActionType == AFO.AT.Secondary)
		{
			if ((m_RequiredAnimalType == ObjectType.AnimalCow && AnimalCow.GetIsTypeCow(Info.m_ObjectType)) || (m_RequiredAnimalType == ObjectType.AnimalSheep && AnimalSheep.GetIsTypeSheep(Info.m_ObjectType)))
			{
				return GetActionFromAnimal(Info);
			}
			return GetActionFromNothing(Info);
		}
		return base.GetActionFromObject(Info);
	}

	public void StopAll()
	{
		if (GetAnyAnimalsBeingActioned())
		{
			return;
		}
		for (int i = 0; i < m_Slots; i++)
		{
			if ((bool)m_Animals[i])
			{
				StopAnimalAction(m_Animals[i]);
				m_Animals[i].SetState(AnimalGrazer.State.None);
				ReleaseAnimal(i);
			}
		}
	}
}
