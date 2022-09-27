public class IndustryLevelEvent
{
	public QuestEvent.Type m_Type;

	public bool m_BotOnly;

	public object m_ExtraData;

	public int m_Count;

	private QuestEvent m_Event;

	public IndustryLevelEvent(QuestEvent.Type Type, bool BotOnly, object ExtraData, int Count)
	{
		m_Type = Type;
		m_BotOnly = BotOnly;
		m_ExtraData = ExtraData;
		m_Count = Count;
		m_Event = new QuestEvent(m_Type, BotOnly, m_ExtraData, m_Count);
	}

	public string GetDisplayString()
	{
		return m_Event.GetDisplayString();
	}
}
