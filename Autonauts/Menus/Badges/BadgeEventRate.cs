using UnityEngine;
using UnityEngine.UI;

public class BadgeEventRate : MonoBehaviour
{
	private BadgeEvent m_Event;

	private Badge m_Badge;

	public void SetEvent(BadgeEvent NewEvent, Badge NewBadge)
	{
		m_Event = NewEvent;
		m_Badge = NewBadge;
		string text = "BadgeEvent" + m_Event.m_Type;
		string text2 = TextManager.Instance.Get(text);
		base.transform.Find("EventText").GetComponent<Text>().text = text2;
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
		string text = num + "/" + eventRequiredCount + " " + TextManager.Instance.Get("BadgeRolloverPerMin");
		base.transform.Find("CompleteText").GetComponent<Text>().text = text;
	}
}
