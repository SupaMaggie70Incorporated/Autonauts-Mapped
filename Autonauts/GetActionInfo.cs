public class GetActionInfo
{
	public GetAction m_Action;

	public string m_Value;

	public BaseClass m_Object;

	public GetActionInfo(GetAction Action, BaseClass NewObject = null, string Value = "")
	{
		m_Action = Action;
		m_Object = NewObject;
		m_Value = Value;
	}
}
