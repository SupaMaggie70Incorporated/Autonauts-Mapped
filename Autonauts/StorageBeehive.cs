using System.Collections.Generic;

public class StorageBeehive : Storage
{
	public List<AnimalBee> m_Bees;

	protected int m_MaxBees;

	public static bool GetIsTypeBeehive(ObjectType NewType)
	{
		if (NewType == ObjectType.StorageBeehive || NewType == ObjectType.StorageBeehiveCrude)
		{
			return true;
		}
		return false;
	}

	public override void Restart()
	{
		base.Restart();
		SetDimensions(new TileCoord(0, 0), new TileCoord(0, 0), new TileCoord(0, 1));
		m_MaxBees = 4;
		foreach (AnimalBee bee in m_Bees)
		{
			bee.gameObject.SetActive(value: true);
		}
	}

	protected new void Awake()
	{
		base.Awake();
		m_Bees = new List<AnimalBee>();
		m_ObjectType = ObjectType.Honey;
		m_Capacity = 5;
	}

	protected new void OnDestroy()
	{
		base.OnDestroy();
		foreach (AnimalBee bee in m_Bees)
		{
			if ((bool)bee)
			{
				bee.StopUsing();
			}
		}
		m_Bees.Clear();
	}

	public override void StopUsing(bool AndDestroy = true)
	{
		base.StopUsing(AndDestroy);
		foreach (AnimalBee bee in m_Bees)
		{
			if (AndDestroy)
			{
				bee.StopUsing();
				continue;
			}
			bee.gameObject.SetActive(value: false);
			bee.m_Indicator.SetNeedFood(NeedsFood: false);
		}
		if (AndDestroy)
		{
			m_Bees.Clear();
		}
	}

	public override object GetActionInfo(GetActionInfo Info)
	{
		GetAction action = Info.m_Action;
		if (action == GetAction.IsDeletable)
		{
			return true;
		}
		return base.GetActionInfo(Info);
	}

	public void AddBee(AnimalBee NewBee)
	{
		m_Bees.Add(NewBee);
	}

	private void AddBees(int Count)
	{
		for (int i = 0; i < Count; i++)
		{
			AnimalBee component = ObjectTypeList.Instance.CreateObjectFromIdentifier(ObjectType.AnimalBee, base.transform.position, base.transform.localRotation).GetComponent<AnimalBee>();
			component.SetState(AnimalBee.State.InHive);
			component.SetBeeHive(this);
			m_Bees.Add(component);
		}
	}

	protected override void TileCoordChanged(TileCoord Position)
	{
		base.TileCoordChanged(Position);
		foreach (AnimalBee bee in m_Bees)
		{
			if (bee.m_State == AnimalBee.State.InHive)
			{
				bee.UpdatePositionToTilePosition(Position);
			}
		}
	}

	private void StartAddBeesNest(AFO Info)
	{
		AddBees(m_MaxBees);
		Info.m_Object.transform.position = base.transform.position;
	}

	private void EndAddBeesNest(AFO Info)
	{
		Info.m_Object.StopUsing();
	}

	private ActionType GetActionFromBeesNest(AFO Info)
	{
		Info.m_StartAction = StartAddBeesNest;
		Info.m_EndAction = EndAddBeesNest;
		Info.m_FarmerState = Farmer.State.Adding;
		if (m_Bees.Count != 0)
		{
			return ActionType.Fail;
		}
		return ActionType.AddResource;
	}

	private void StartRemoveBucket(AFO Info)
	{
		_ = Info.m_Object;
		m_Stored = 0;
		AudioManager.Instance.StartEvent("BuildingStorageTake", this);
		QuestManager.Instance.AddEvent(QuestEvent.Type.FillBucketHoney, Info.m_Actioner.m_TypeIdentifier == ObjectType.Worker, 0, this);
	}

	private void EndRemoveBucket(AFO Info)
	{
		Info.m_Object.GetComponent<ToolBucket>().Fill(m_ObjectType, 1);
	}

	private ActionType GetPrimaryActionFromBucket(AFO Info)
	{
		Info.m_StartAction = StartRemoveBucket;
		Info.m_EndAction = EndRemoveBucket;
		Info.m_FarmerState = Farmer.State.Taking;
		if (m_Stored != GetCapacity())
		{
			return ActionType.Fail;
		}
		if (Info.m_Object == null)
		{
			return ActionType.Fail;
		}
		ToolBucket component = Info.m_Object.GetComponent<ToolBucket>();
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

	private void StartAddBucket(AFO Info)
	{
		Actionable @object = Info.m_Object;
		AddToStored(m_ObjectType, GetCapacity(), Info.m_Actioner);
		@object.GetComponent<ToolBucket>().Empty(1);
		AudioManager.Instance.StartEvent("BuildingStorageAdd", this);
	}

	private ActionType GetSecondaryActionFromBucket(AFO Info)
	{
		Info.m_StartAction = StartAddBucket;
		Info.m_FarmerState = Farmer.State.Adding;
		if (m_Stored != 0)
		{
			return ActionType.Fail;
		}
		ToolBucket component = Info.m_Object.GetComponent<ToolBucket>();
		if (component == null)
		{
			return ActionType.Fail;
		}
		if (!component.CanAcceptObjectType(m_ObjectType))
		{
			return ActionType.Fail;
		}
		return ActionType.AddResource;
	}

	public override ActionType GetActionFromObject(AFO Info)
	{
		if (Info.m_ActionType == AFO.AT.Primary)
		{
			if (ToolBucket.GetIsTypeBucket(Info.m_ObjectType))
			{
				return GetPrimaryActionFromBucket(Info);
			}
		}
		else if (Info.m_ActionType == AFO.AT.Secondary && Info.m_ObjectType == ObjectType.BeesNest)
		{
			return GetActionFromBeesNest(Info);
		}
		return ActionType.Total;
	}

	public void AddHoney()
	{
		bool isFull = GetIsFull();
		AddToStored(m_ObjectType, 1, null);
		if (!isFull && GetIsFull())
		{
			QuestManager.Instance.AddEvent(QuestEvent.Type.BeeMakesHoney, Bot: false, 0, this);
			BadgeManager.Instance.AddEvent(BadgeEvent.Type.Honey);
		}
	}

	protected override void Update()
	{
		base.Update();
	}
}
