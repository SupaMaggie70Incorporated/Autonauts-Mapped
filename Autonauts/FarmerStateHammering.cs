public class FarmerStateHammering : FarmerStateTool
{
	public FarmerStateHammering()
	{
		m_ActionSoundName = "ToolMalletHammer";
		m_NoToolIconName = "GenIcons/GenIconToolMallet";
		m_AdjacentTile = true;
	}

	public static bool GetIsToolAcceptable(ObjectType NewType)
	{
		return NewType == ObjectType.ToolMallet;
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
		FaceTowardsTargetTile();
		Actionable currentObject = m_Farmer.m_FarmerAction.m_CurrentObject;
		if ((bool)currentObject && currentObject.m_TypeIdentifier == ObjectType.Pumpkin)
		{
			m_Farmer.StartAnimation("FarmerHammeringVertical");
		}
		else
		{
			m_Farmer.StartAnimation("FarmerHammeringHorizontal");
		}
	}

	public override void EndState()
	{
		base.EndState();
		StandInOldTile();
	}

	protected override void SendEvents(Actionable TargetObject)
	{
		if ((bool)TargetObject && TargetObject.m_TypeIdentifier == ObjectType.Pumpkin)
		{
			QuestManager.Instance.AddEvent(QuestEvent.Type.BashPumpkin, m_Farmer.m_TypeIdentifier == ObjectType.Worker, 0, m_Farmer);
		}
		if ((bool)TargetObject && TargetObject.m_TypeIdentifier == ObjectType.TreeApple)
		{
			QuestManager.Instance.AddEvent(QuestEvent.Type.BashAppleTree, m_Farmer.m_TypeIdentifier == ObjectType.Worker, 0, m_Farmer);
		}
		if ((bool)TargetObject && TargetObject.m_TypeIdentifier == ObjectType.TreeCoconut)
		{
			QuestManager.Instance.AddEvent(QuestEvent.Type.BashCoconutTree, m_Farmer.m_TypeIdentifier == ObjectType.Worker, 0, m_Farmer);
		}
	}
}
