public class Nothing : Actionable
{
	private void StartInstrument(AFO Info)
	{
		Info.m_Object.GetComponent<Instrument>().Play();
	}

	private ActionType GetActionForInstrument(AFO Info)
	{
		Info.m_FarmerState = Farmer.State.Play;
		Info.m_StartAction = StartInstrument;
		return ActionType.UseInHands;
	}

	private void EndFillable(AFO Info)
	{
		ToolFillable component = Info.m_Object.GetComponent<ToolFillable>();
		component.Empty(component.m_Stored);
	}

	private ActionType GetActionForFillable(AFO Info)
	{
		Info.m_FarmerState = Farmer.State.Emptying;
		Info.m_EndAction = EndFillable;
		if ((bool)Info.m_Object && (!ToolFillable.GetIsTypeFillable(Info.m_Object.m_TypeIdentifier) || Info.m_Object.GetComponent<ToolFillable>().GetIsEmpty()))
		{
			return ActionType.Fail;
		}
		return ActionType.UseInHands;
	}

	public override ActionType GetActionFromObject(AFO Info)
	{
		if (ToolFillable.GetIsTypeFillable(Info.m_ObjectType))
		{
			return GetActionForFillable(Info);
		}
		if (Instrument.GetIsTypeInstrument(Info.m_ObjectType))
		{
			return GetActionForInstrument(Info);
		}
		return ActionType.Total;
	}
}
