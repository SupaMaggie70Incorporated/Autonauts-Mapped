public class Dirt : Selectable
{
	public override void Restart()
	{
		base.Restart();
	}

	private void EndBroom(AFO Info)
	{
		StopUsing();
	}

	private ActionType GetActionFromBroom(AFO Info)
	{
		Info.m_EndAction = EndBroom;
		Info.m_FarmerState = Farmer.State.Sweeping;
		if (Info.m_ActionType == AFO.AT.Primary)
		{
			return ActionType.UseInHands;
		}
		return ActionType.Total;
	}

	public override ActionType GetActionFromObject(AFO Info)
	{
		if (FarmerStateSweeping.GetIsToolAcceptable(Info.m_ObjectType))
		{
			return GetActionFromBroom(Info);
		}
		return base.GetActionFromObject(Info);
	}
}
