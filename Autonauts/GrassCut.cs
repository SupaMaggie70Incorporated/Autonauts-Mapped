public class GrassCut : Holdable
{
	private void EndPitchfork(AFO Info)
	{
	}

	private ActionType GetActionFromPitchfork(AFO Info)
	{
		Info.m_EndAction = EndPitchfork;
		Info.m_FarmerState = Farmer.State.PickingUp;
		if (!Info.m_Object.GetComponent<ToolPitchfork>().CanAcceptObjectType(m_TypeIdentifier))
		{
			return ActionType.Fail;
		}
		return ActionType.Pickup;
	}

	public override ActionType GetActionFromObject(AFO Info)
	{
		ObjectType objectType = Info.m_ObjectType;
		if (Info.m_ActionType == AFO.AT.Primary && objectType == ObjectType.ToolPitchfork)
		{
			return GetActionFromPitchfork(Info);
		}
		return base.GetActionFromObject(Info);
	}
}
