using System.Collections.Generic;
using UnityEngine;

public class QuestRollover : Rollover
{
	private Quest m_Quest;

	private List<EventComplete> m_Events;

	private float m_Height;

	protected new void Awake()
	{
		base.Awake();
		m_Events = new List<EventComplete>();
		Hide();
	}

	private void SetupQuest()
	{
		foreach (EventComplete @event in m_Events)
		{
			Object.Destroy(@event.gameObject);
		}
		m_Events.Clear();
		Transform transform = m_Panel.transform;
		BaseText component = transform.Find("Description").GetComponent<BaseText>();
		component.SetTextFromID(m_Quest.m_Description);
		float preferredHeight = component.GetPreferredHeight();
		m_Height = 120f + preferredHeight - 16f;
		Transform transform2 = transform.Find("EventsPanel");
		GameObject gameObject = transform2.transform.Find("EventComplete").gameObject;
		Vector2 anchoredPosition = gameObject.GetComponent<RectTransform>().anchoredPosition;
		gameObject.SetActive(value: false);
		float num = 0f;
		foreach (QuestEvent item in m_Quest.m_EventsRequired)
		{
			if (item.m_Type != QuestEvent.Type.Total && (!m_Quest.GetIsComplete() || item.m_Complete) && (!m_Quest.m_Or || !m_Quest.m_ProgressStarted || item.m_Progress > 0))
			{
				EventComplete component2 = Object.Instantiate(gameObject, HudManager.Instance.m_CeremoniesRootTransform).GetComponent<EventComplete>();
				component2.gameObject.SetActive(value: true);
				component2.SetEvent(item, m_Quest);
				m_Events.Add(component2);
				float num2 = 17f;
				if (component2.GetVisibleLines() > 1)
				{
					num2 += 10f;
				}
				component2.transform.SetParent(transform2);
				component2.GetComponent<RectTransform>().offsetMin = new Vector2(10f, anchoredPosition.y - num);
				component2.GetComponent<RectTransform>().offsetMax = new Vector2(-10f, anchoredPosition.y - num);
				component2.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f);
				num += num2;
			}
		}
		m_Height += num;
		m_Panel.SetHeight(m_Height);
		if (m_Quest.m_Type == Quest.Type.Academy)
		{
			transform.Find("IndustryImage").GetComponent<BaseImage>().SetSprite(m_Quest.m_IconName);
			transform.Find("SubIndustryImage").GetComponent<BaseImage>().SetSprite(m_Quest.m_IconName);
		}
		transform.Find("Level").GetComponent<BaseText>().SetTextFromID(m_Quest.m_Title);
		string newText = "QuestRolloverTitle";
		if (m_Quest.m_Type == Quest.Type.Tutorial)
		{
			newText = "QuestRolloverTutorialTitle";
		}
		transform.Find("Title").GetComponent<BaseText>().SetTextFromID(newText);
		transform.Find("ApprovedTick").GetComponent<BaseImage>().SetActive(m_Quest.GetIsComplete());
		transform.Find("EventsPanel").GetComponent<BaseImage>().SetHeight(num + 15f);
	}

	public void SetTarget(Quest Target)
	{
		if (Target != m_Quest)
		{
			m_Quest = Target;
			if (m_Quest != null)
			{
				SetupQuest();
			}
		}
	}

	public void CheckProgress(QuestEvent.Type TestEvent, bool BotOnly, object ExtraData)
	{
		foreach (EventComplete @event in m_Events)
		{
			@event.CheckProgress(TestEvent, BotOnly, ExtraData);
		}
	}

	protected override bool GetTargettingSomething()
	{
		return m_Quest != null;
	}

	public void SetComplete()
	{
		foreach (EventComplete @event in m_Events)
		{
			@event.ShowTextBlack();
		}
	}

	protected new void Update()
	{
		base.Update();
		m_Panel.SetHeight(m_Height);
	}
}
