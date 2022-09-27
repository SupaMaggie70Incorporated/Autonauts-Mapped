using UnityEngine;

public class FarmerStateScythe : FarmerStateTool
{
	public FarmerStateScythe()
	{
		m_ActionSoundName = "ToolScytheChop";
		m_NoToolIconName = "GenIcons/GenIconToolScytheMetal";
		m_AdjacentTile = true;
	}

	public static bool GetIsToolAcceptable(ObjectType NewType)
	{
		if (!ToolScythe.GetIsTypeScythe(NewType))
		{
			return NewType == ObjectType.ToolBlade;
		}
		return true;
	}

	public override bool IsToolAcceptable(Holdable NewObject)
	{
		if (NewObject == null)
		{
			return false;
		}
		return GetIsToolAcceptable(NewObject.m_TypeIdentifier);
	}

	public override void StartState()
	{
		base.StartState();
		GetFinalRotationTile();
		StandInTargetTile();
		m_Farmer.StartAnimation("FarmerScythe");
	}

	public override void EndState()
	{
		base.EndState();
		StandInTargetTile();
	}

	protected override void SendEvents(Actionable TargetObject)
	{
		bool bot = m_Farmer.m_TypeIdentifier == ObjectType.Worker;
		if (m_Tool.m_TypeIdentifier == ObjectType.RockSharp)
		{
			QuestManager.Instance.AddEvent(QuestEvent.Type.UseRockSharpOnCrops, bot, 0, m_Farmer);
		}
		else if (m_Tool.m_TypeIdentifier == ObjectType.ToolScytheStone)
		{
			QuestManager.Instance.AddEvent(QuestEvent.Type.ScytheWheatWithScytheCrude, bot, 0, m_Farmer);
		}
		else if (m_Tool.m_TypeIdentifier == ObjectType.ToolScythe)
		{
			QuestManager.Instance.AddEvent(QuestEvent.Type.ScytheWheatWithScythe, bot, 0, m_Farmer);
		}
		if ((bool)TargetObject && TargetObject.m_TypeIdentifier == ObjectType.CropWheat)
		{
			QuestManager.Instance.AddEvent(QuestEvent.Type.ScytheWheat, bot, 0, m_Farmer);
		}
		if ((bool)TargetObject && TargetObject.m_TypeIdentifier == ObjectType.CropCotton)
		{
			QuestManager.Instance.AddEvent(QuestEvent.Type.ScytheCotton, bot, 0, m_Farmer);
		}
		if ((bool)TargetObject && TargetObject.m_TypeIdentifier == ObjectType.Bullrushes)
		{
			QuestManager.Instance.AddEvent(QuestEvent.Type.ScytheBullrushes, bot, 0, m_Farmer);
		}
		if ((bool)TargetObject && TargetObject.m_TypeIdentifier == ObjectType.CropWheat)
		{
			BadgeManager.Instance.AddEvent(BadgeEvent.Type.CropsCut);
		}
		if ((bool)TargetObject && TargetObject.m_TypeIdentifier == ObjectType.Grass)
		{
			QuestManager.Instance.AddEvent(QuestEvent.Type.ScytheGrass, bot, 0, m_Farmer);
		}
		if ((bool)TargetObject && TargetObject.m_TypeIdentifier == ObjectType.CropPumpkin)
		{
			QuestManager.Instance.AddEvent(QuestEvent.Type.ScythePumpkin, bot, 0, m_Farmer);
		}
		if ((bool)TargetObject && TargetObject.m_TypeIdentifier == ObjectType.FlowerWild)
		{
			QuestManager.Instance.AddEvent(QuestEvent.Type.ScytheFlower, bot, 0, m_Farmer);
		}
	}

	public override void DoAnimationAction()
	{
		base.DoAnimationAction();
		Vector3 finalPosition = m_Farmer.m_FinalPosition;
		finalPosition += m_ActionDelta * Tile.m_Size * 0.75f;
		m_Farmer.CreateParticles(finalPosition, "CutGrass");
	}
}
