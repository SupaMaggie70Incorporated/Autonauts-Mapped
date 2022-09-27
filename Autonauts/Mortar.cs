public class Mortar : Holdable
{
	private ActionType GetActionFromFillable(AFO Info)
	{
		Info.m_FarmerState = Farmer.State.PickingUp;
		Holdable topObject = Info.m_Actioner.GetComponent<Farmer>().m_FarmerCarry.GetTopObject();
		if (topObject == null)
		{
			return ActionType.Fail;
		}
		ToolBucket component = topObject.GetComponent<ToolBucket>();
		if (component == null)
		{
			return ActionType.Fail;
		}
		if (!component.CanAcceptObjectType(m_TypeIdentifier))
		{
			return ActionType.Fail;
		}
		return ActionType.Pickup;
	}

	public override ActionType GetActionFromObject(AFO Info)
	{
		if (Info.m_ActionType == AFO.AT.Primary)
		{
			if ((bool)Info.m_Actioner && (bool)Info.m_Actioner.GetComponent<Farmer>() && ToolBucket.GetIsTypeBucket(Info.m_Actioner.GetComponent<Farmer>().m_FarmerCarry.GetTopObjectType()))
			{
				return GetActionFromFillable(Info);
			}
			return ActionType.Fail;
		}
		return base.GetActionFromObject(Info);
	}
}
