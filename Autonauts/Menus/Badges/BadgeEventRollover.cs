public class BadgeEventRollover : BaseGadget
{
	private BadgeEvent m_Event;

	private Badge m_Badge;

	public void SetEvent(BadgeEvent NewEvent, Badge NewBadge)
	{
		m_Event = NewEvent;
		m_Badge = NewBadge;
		string newText = "BadgeEvent" + m_Event.m_Type;
		base.transform.Find("Description").GetComponent<BaseText>().SetTextFromID(newText);
		UpdateProgress();
	}

	private void UpdateProgress()
	{
		int num = m_Event.m_Count;
		int eventRequiredCount = m_Badge.GetEventRequiredCount(m_Event.m_Type);
		if (num > eventRequiredCount)
		{
			num = eventRequiredCount;
		}
		string text = num + "/" + eventRequiredCount;
		base.transform.Find("Count").GetComponent<BaseText>().SetText(text);
	}
}
