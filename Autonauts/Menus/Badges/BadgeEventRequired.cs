public class BadgeEventRequired
{
	public BadgeEvent.Type m_Event;

	public int m_Count;

	public BadgeEventRequired(BadgeEvent.Type NewEvent, int EventCount)
	{
		m_Event = NewEvent;
		m_Count = EventCount;
	}
}
