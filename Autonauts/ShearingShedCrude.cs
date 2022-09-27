using UnityEngine;

public class ShearingShedCrude : AnimalStation
{
	public override void Restart()
	{
		base.Restart();
		SetDimensions(new TileCoord(-1, -1), new TileCoord(1, 0), new TileCoord(0, 1));
		m_Slots = 3;
		m_Animals = new AnimalGrazer[m_Slots];
		m_RequiredAnimalType = ObjectType.AnimalSheep;
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
		num2 = Tile.m_Size * 0.5f;
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
		((FarmerStateShearing)Info.m_Actioner.GetComponent<Farmer>().m_States[29]).m_Sheep = component.GetComponent<AnimalSheep>();
	}

	protected override void EndAnimalAction(AFO Info)
	{
		base.EndAnimalAction(Info);
		BadgeManager.Instance.AddEvent(BadgeEvent.Type.Wool);
		AnimalGrazer sheep = ((FarmerStateShearing)Info.m_Actioner.GetComponent<Farmer>().m_States[29]).m_Sheep;
		sheep.GetComponent<AnimalSheep>().Shear();
		for (int i = 0; i < 2; i++)
		{
			BaseClass newObject = ObjectTypeList.Instance.CreateObjectFromIdentifier(ObjectType.Fleece, GetAccessPosition().ToWorldPositionTileCentered(), Quaternion.identity);
			SpawnAnimationManager.Instance.AddJump(newObject, sheep.transform.position, GetAccessPosition().ToWorldPositionTileCentered(), 4f);
		}
		int animalSlot = GetAnimalSlot(sheep);
		ReleaseAnimal(animalSlot);
		StopAnimalAction(sheep);
	}

	protected override void AbortAnimalAction(AFO Info)
	{
		base.AbortAnimalAction(Info);
		AnimalGrazer sheep = ((FarmerStateShearing)Info.m_Actioner.GetComponent<Farmer>().m_States[29]).m_Sheep;
		StopAnimalAction(sheep);
	}

	private ActionType GetActionFromShears(AFO Info)
	{
		Info.m_StartAction = StartAnimalAction;
		Info.m_EndAction = EndAnimalAction;
		Info.m_AbortAction = AbortAnimalAction;
		Info.m_FarmerState = Farmer.State.Shearing;
		if (GetFullAnimal() == -1)
		{
			return ActionType.Fail;
		}
		return ActionType.UseInHands;
	}

	public override ActionType GetActionFromObject(AFO Info)
	{
		if (Info.m_ActionType == AFO.AT.Primary && Info.m_ObjectType == ObjectType.ToolShears)
		{
			return GetActionFromShears(Info);
		}
		return base.GetActionFromObject(Info);
	}
}
