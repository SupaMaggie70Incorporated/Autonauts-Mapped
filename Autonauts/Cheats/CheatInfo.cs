using System;

public class CheatInfo
{
	public enum Type
	{
		Button,
		Slider,
		Toggle,
		Total
	}

	public Type m_Type;

	private Action<BaseGadget> m_Action;

	public CheatInfo(Type NewType, Action<BaseGadget> NewAction)
	{
		m_Type = NewType;
		m_Action = NewAction;
	}
}
