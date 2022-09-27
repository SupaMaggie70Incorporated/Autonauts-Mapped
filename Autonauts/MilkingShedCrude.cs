using UnityEngine;

public class MilkingShedCrude : AnimalStation
{
	public override void Restart()
	{
		base.Restart();
		SetDimensions(new TileCoord(-1, -1), new TileCoord(1, 0), new TileCoord(0, 1));
		m_Slots = 3;
		m_Animals = new AnimalGrazer[m_Slots];
		m_RequiredAnimalType = ObjectType.AnimalCow;
	}

	protected override void UpdateAnimalPosition(int Slot)
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

	protected override Vector3 GetActionPosition(int Slot)
	{
		float num = 0f - (float)m_Slots * Tile.m_Size / 2f;
		num += Tile.m_Size * 0.5f;
		num += (float)Slot * Tile.m_Size;
		return base.transform.TransformPoint(new Vector3(num, 0f, 0f));
	}

	protected override void StartAnimalAction(AFO Info)
	{
		base.StartAnimalAction(Info);
		AnimalGrazer component = Info.m_Actioner.GetComponent<Farmer>().m_BaggedObject.GetComponent<AnimalGrazer>();
		((FarmerStateMilking)Info.m_Actioner.GetComponent<Farmer>().m_States[9]).m_Cow = component.GetComponent<AnimalCow>();
	}

	protected override void EndAnimalAction(AFO Info)
	{
		base.EndAnimalAction(Info);
		QuestManager.Instance.AddEvent(QuestEvent.Type.MilkCowInMilkingShed, Info.m_Actioner.m_TypeIdentifier == ObjectType.Worker, 0, this);
		QuestManager.Instance.AddEvent(QuestEvent.Type.MilkCow, Info.m_Actioner.m_TypeIdentifier == ObjectType.Worker, 0, this);
		BadgeManager.Instance.AddEvent(BadgeEvent.Type.Milk);
		AnimalGrazer cow = ((FarmerStateMilking)Info.m_Actioner.GetComponent<Farmer>().m_States[9]).m_Cow;
		cow.GetComponent<AnimalCow>().Milk();
		int animalSlot = GetAnimalSlot(cow);
		ReleaseAnimal(animalSlot);
		StopAnimalAction(cow);
	}

	protected override void AbortAnimalAction(AFO Info)
	{
		base.AbortAnimalAction(Info);
		AnimalGrazer cow = ((FarmerStateMilking)Info.m_Actioner.GetComponent<Farmer>().m_States[9]).m_Cow;
		StopAnimalAction(cow);
	}

	private ActionType GetActionFromBucket(AFO Info)
	{
		Info.m_StartAction = StartAnimalAction;
		Info.m_EndAction = EndAnimalAction;
		Info.m_AbortAction = AbortAnimalAction;
		Info.m_FarmerState = Farmer.State.Milking;
		if (GetFullAnimal() == -1)
		{
			return ActionType.Fail;
		}
		if (Info.m_Object == null || Info.m_Object.GetComponent<ToolBucket>() == null)
		{
			return ActionType.Fail;
		}
		if (Info.m_Object.GetComponent<ToolBucket>().GetIsFull())
		{
			return ActionType.Fail;
		}
		return ActionType.UseInHands;
	}

	public override ActionType GetActionFromObject(AFO Info)
	{
		if (Info.m_ActionType == AFO.AT.Primary && ToolBucket.GetIsTypeBucket(Info.m_ObjectType))
		{
			return GetActionFromBucket(Info);
		}
		return base.GetActionFromObject(Info);
	}
}
