public class FarmerStateHoe : FarmerStateTool
{
	public FarmerStateHoe()
	{
		m_ActionSoundName = "ToolHoeUse";
		m_NoToolIconName = "GenIcons/GenIconToolHoeMetal";
		m_AdjacentTile = true;
	}

	public static bool GetIsToolAcceptable(ObjectType NewType)
	{
		return ToolHoe.GetIsTypeHoe(NewType);
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
		m_Farmer.StartAnimation("FarmerHoe");
	}

	public override void EndState()
	{
		base.EndState();
		StandInTargetTile();
	}

	protected override void SendEvents(Actionable TargetObject)
	{
		BaseClass topObject = m_Farmer.m_FarmerCarry.GetTopObject();
		if ((bool)topObject && topObject.m_TypeIdentifier == ObjectType.ToolHoeStone)
		{
			QuestManager.Instance.AddEvent(QuestEvent.Type.HoeWithHoeCrude, m_Farmer.m_TypeIdentifier == ObjectType.Worker, 0, m_Farmer);
		}
		else
		{
			QuestManager.Instance.AddEvent(QuestEvent.Type.HoeWithHoe, m_Farmer.m_TypeIdentifier == ObjectType.Worker, 0, m_Farmer);
		}
		QuestManager.Instance.AddEvent(QuestEvent.Type.Hoe, m_Farmer.m_TypeIdentifier == ObjectType.Worker, 0, m_Farmer);
	}

	public override void DoAnimationAction()
	{
		base.DoAnimationAction();
		m_Farmer.CreateParticles(m_Farmer.m_FinalPosition, "Hoe");
	}
}
