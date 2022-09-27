using System.Collections.Generic;
using UnityEngine;

public class BadgeRollover : Rollover
{
	private Badge m_Target;

	private BaseText m_Text;

	private List<GameObject> m_Events;

	private BadgeEventRollover m_DefaultEvent;

	protected new void Awake()
	{
		base.Awake();
		m_Target = null;
		m_Text = m_Panel.transform.Find("Title").GetComponent<BaseText>();
		m_DefaultEvent = m_Panel.transform.Find("Event").GetComponent<BadgeEventRollover>();
		m_DefaultEvent.SetActive(Active: false);
		m_Wobbler.m_WobbleWhilePaused = true;
		m_Events = new List<GameObject>();
		Hide();
	}

	public void SetTarget(Badge Target)
	{
		if (Target == m_Target)
		{
			return;
		}
		foreach (GameObject event2 in m_Events)
		{
			Object.Destroy(event2.gameObject);
		}
		m_Events.Clear();
		m_Target = Target;
		if (m_Target == null)
		{
			return;
		}
		string newText = "Badge" + Target.m_ID;
		m_Text.SetTextFromID(newText);
		Transform parent = m_Panel.transform;
		float num = 0f;
		foreach (BadgeEventRequired item in m_Target.m_EventsRequired)
		{
			BadgeEvent @event = BadgeManager.Instance.GetEvent(item.m_Event);
			BadgeEventRollover component = Object.Instantiate(m_DefaultEvent, new Vector3(0f, 0f, 0f), Quaternion.identity, parent).GetComponent<BadgeEventRollover>();
			component.SetActive(Active: true);
			component.SetEvent(@event, m_Target);
			component.transform.localPosition = new Vector3(0f, num, 0f);
			m_Events.Add(component.gameObject);
			num -= 25f;
		}
	}

	protected override bool GetTargettingSomething()
	{
		if (m_Target != null)
		{
			return true;
		}
		return false;
	}
}
