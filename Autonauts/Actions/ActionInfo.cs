public class ActionInfo
{
	public ActionType m_Action;

	public TileCoord m_Position;

	public Actionable m_Object;

	public string m_Value;

	public string m_Value2;

	public int m_ObjectUID;

	public ObjectType m_ObjectType;

	public AFO.AT m_ActionType;

	public string m_ActionRequirement;

	public ObjectType m_ActionObjectType;

	public ActionInfo(ActionType Action, TileCoord Position, Actionable Object = null, string Value = "", string Value2 = "", AFO.AT NewActionType = AFO.AT.Total, string ActionRequirement = "", ObjectType ActionObjectType = (ObjectType)100000)
	{
		m_Action = Action;
		m_Position = Position;
		m_Object = Object;
		m_Value = Value;
		m_Value2 = Value2;
		m_ActionType = NewActionType;
		m_ActionRequirement = ActionRequirement;
		if (ActionObjectType == (ObjectType)100000)
		{
			m_ActionObjectType = ObjectTypeList.m_Total;
		}
		else
		{
			m_ActionObjectType = ActionObjectType;
		}
		if ((bool)Object)
		{
			m_ObjectUID = Object.m_UniqueID;
			m_ObjectType = Object.m_TypeIdentifier;
		}
		else
		{
			m_ObjectUID = 0;
			m_ObjectType = ObjectTypeList.m_Total;
		}
	}

	public ActionInfo(ActionInfo OldInfo)
	{
		m_Action = OldInfo.m_Action;
		m_Position = OldInfo.m_Position;
		m_Object = OldInfo.m_Object;
		m_Value = OldInfo.m_Value;
		m_Value2 = OldInfo.m_Value2;
		m_ObjectUID = OldInfo.m_ObjectUID;
		m_ObjectType = OldInfo.m_ObjectType;
		m_ActionType = OldInfo.m_ActionType;
		m_ActionRequirement = OldInfo.m_ActionRequirement;
		m_ActionObjectType = OldInfo.m_ActionObjectType;
	}
}
