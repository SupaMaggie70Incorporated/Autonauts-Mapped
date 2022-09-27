using UnityEngine;

public class FarmerStateSeed : FarmerStateBase
{
	private bool m_Done;

	public override bool GetIsAdjacentTile(TileCoord TargetTile, BaseClass Object)
	{
		return true;
	}

	public override void StartState()
	{
		base.StartState();
		m_Farmer.SetTilePosition(m_Farmer.m_GoToTilePosition);
		m_FinalPosition = m_Farmer.m_TileCoord.ToWorldPositionTileCentered();
		Vector3 vector = m_FinalPosition - m_Farmer.transform.position;
		float num = 0f - Mathf.Atan2(vector.z, vector.x);
		m_FinalRotation = Quaternion.Euler(0f, num * 57.29578f - 90f, 0f);
		m_Farmer.transform.position = m_FinalPosition;
		m_Farmer.transform.rotation = m_FinalRotation;
		m_Farmer.StartAnimation("FarmerSeed");
		m_Done = false;
	}

	public override void EndState()
	{
		base.EndState();
		m_Farmer.transform.position = m_FinalPosition;
		m_Farmer.transform.rotation = m_FinalRotation;
	}

	public override void DoAnimationAction()
	{
		base.DoAnimationAction();
		m_Done = true;
	}

	public override void UpdateState()
	{
		base.UpdateState();
		if (m_Done)
		{
			bool bot = m_Farmer.m_TypeIdentifier == ObjectType.Worker;
			if (m_Farmer.m_FarmerCarry.m_LastObjectType == ObjectType.WheatSeed)
			{
				QuestManager.Instance.AddEvent(QuestEvent.Type.PlantCropSeed, bot, 0, m_Farmer);
			}
			if (m_Farmer.m_FarmerCarry.m_LastObjectType == ObjectType.WheatSeed)
			{
				QuestManager.Instance.AddEvent(QuestEvent.Type.PlantWheat, bot, 0, m_Farmer);
			}
			if (m_Farmer.m_FarmerCarry.m_LastObjectType == ObjectType.CottonSeeds)
			{
				QuestManager.Instance.AddEvent(QuestEvent.Type.PlantCotton, bot, 0, m_Farmer);
			}
			if (m_Farmer.m_FarmerCarry.m_LastObjectType == ObjectType.BullrushesSeeds)
			{
				QuestManager.Instance.AddEvent(QuestEvent.Type.PlantBullrushes, bot, 0, m_Farmer);
			}
			if (m_Farmer.m_FarmerCarry.m_LastObjectType == ObjectType.PumpkinSeeds)
			{
				QuestManager.Instance.AddEvent(QuestEvent.Type.PlantPumpkinSeeds, bot, 0, m_Farmer);
			}
			if (m_Farmer.m_FarmerCarry.m_LastObjectType == ObjectType.Fertiliser)
			{
				QuestManager.Instance.AddEvent(QuestEvent.Type.PlantFertiliser, bot, 0, m_Farmer);
			}
			if (m_Farmer.m_FarmerCarry.m_LastObjectType == ObjectType.TreeSeed)
			{
				QuestManager.Instance.AddEvent(QuestEvent.Type.PlantTreeSeed, bot, 0, m_Farmer);
			}
			if (m_Farmer.m_FarmerCarry.m_LastObjectType == ObjectType.TreeMulberry)
			{
				QuestManager.Instance.AddEvent(QuestEvent.Type.PlantMulberrySeed, bot, 0, m_Farmer);
			}
			if (m_Farmer.m_FarmerCarry.m_LastObjectType == ObjectType.Seedling)
			{
				QuestManager.Instance.AddEvent(QuestEvent.Type.PlantSeedling, bot, 0, m_Farmer);
			}
			if (m_Farmer.m_FarmerCarry.m_LastObjectType == ObjectType.SeedlingMulberry)
			{
				QuestManager.Instance.AddEvent(QuestEvent.Type.PlantSeedlingMulberry, bot, 0, m_Farmer);
			}
			if (m_Farmer.m_FarmerCarry.m_LastObjectType == ObjectType.Apple)
			{
				QuestManager.Instance.AddEvent(QuestEvent.Type.PlantApple, bot, 0, m_Farmer);
			}
			if (m_Farmer.m_FarmerCarry.m_LastObjectType == ObjectType.Berries)
			{
				QuestManager.Instance.AddEvent(QuestEvent.Type.PlantBerries, bot, 0, m_Farmer);
			}
			if (m_Farmer.m_FarmerCarry.m_LastObjectType == ObjectType.Manure)
			{
				QuestManager.Instance.AddEvent(QuestEvent.Type.PlantManure, bot, 0, m_Farmer);
			}
			if (m_Farmer.m_FarmerCarry.m_LastObjectType == ObjectType.MushroomDug)
			{
				QuestManager.Instance.AddEvent(QuestEvent.Type.PlantMushroom, bot, 0, m_Farmer);
			}
			if (m_Farmer.m_FarmerCarry.m_LastObjectType == ObjectType.CarrotSeed)
			{
				QuestManager.Instance.AddEvent(QuestEvent.Type.PlantCarrotSeed, bot, 0, m_Farmer);
			}
			if (m_Farmer.m_FarmerCarry.m_LastObjectType == ObjectType.Coconut)
			{
				QuestManager.Instance.AddEvent(QuestEvent.Type.PlantCoconut, bot, 0, m_Farmer);
			}
			Holdable holdable = m_Farmer.m_FarmerCarry.RemoveTopObject();
			DoEndAction();
			if ((bool)holdable)
			{
				holdable.StopUsing();
			}
			m_Farmer.SetBaggedTile(new TileCoord(0, 0));
			if (m_Farmer.m_FarmerCarry.GetLastObjectType() == ObjectType.Fertiliser)
			{
				AudioManager.Instance.StartEvent("FertiliserUsed", m_Farmer);
			}
			else
			{
				AudioManager.Instance.StartEvent("FarmerSeed", m_Farmer);
			}
			if (m_Farmer.m_FarmerCarry.GetLastObjectType() == ObjectType.Turf || m_Farmer.m_FarmerCarry.GetLastObjectType() == ObjectType.TreeSeed || m_Farmer.m_FarmerCarry.GetLastObjectType() == ObjectType.MulberrySeed || m_Farmer.m_FarmerCarry.GetLastObjectType() == ObjectType.Seedling || m_Farmer.m_FarmerCarry.GetLastObjectType() == ObjectType.SeedlingMulberry || m_Farmer.m_FarmerCarry.GetLastObjectType() == ObjectType.Apple || m_Farmer.m_FarmerCarry.GetLastObjectType() == ObjectType.Berries || m_Farmer.m_FarmerCarry.GetLastObjectType() == ObjectType.Manure || m_Farmer.m_FarmerCarry.GetLastObjectType() == ObjectType.Coconut || m_Farmer.m_FarmerCarry.GetLastObjectType() == ObjectType.MushroomDug)
			{
				m_Farmer.SetState(Farmer.State.JumpTurf);
			}
		}
	}
}
