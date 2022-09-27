using UnityEngine;
using UnityEngine.EventSystems;

public class EventComplete : BaseImage
{
	public QuestEvent m_Event;

	private BaseText m_EventText;

	private BaseText m_CompleteText;

	private float m_BotTimer;

	private bool m_CanBeCompleted;

	public void SetEvent(QuestEvent NewEvent, Quest NewQuest)
	{
		m_Event = NewEvent;
		m_CanBeCompleted = m_Event.CanBeCompleted();
		string text = NewEvent.GetDisplayString();
		if (NewEvent.m_Type == QuestEvent.Type.Research)
		{
			text = text + " " + ObjectTypeList.Instance.GetHumanReadableNameFromIdentifier(NewQuest.m_ObjectTypeRequired);
		}
		m_EventText = base.transform.Find("EventText").GetComponent<BaseText>();
		m_CompleteText = base.transform.Find("CompleteText").GetComponent<BaseText>();
		m_EventText.SetText(text);
		ShowTextBlack();
		UpdateProgress();
		UpdateBackground();
	}

	private void SetTextColour(Color NewColour)
	{
		if (m_CanBeCompleted)
		{
			m_EventText.SetColour(NewColour);
			m_CompleteText.SetColour(NewColour);
		}
		else
		{
			m_EventText.SetColour(new Color(1f, 0f, 0f, 1f));
			m_CompleteText.SetColour(new Color(1f, 0f, 0f, 1f));
		}
	}

	public void UpdateProgress()
	{
		string text = m_Event.m_Progress + "/" + m_Event.m_Required;
		m_CompleteText.SetText(text);
		if (m_Event.m_Progress == m_Event.m_Required)
		{
			SetTextColour(new Color(0f, 0f, 0f, 0.5f));
		}
		else if (m_Event.m_Locked)
		{
			SetTextColour(new Color(0f, 0f, 0f, 0.5f));
		}
		else
		{
			SetTextColour(new Color(0f, 0f, 0f, 1f));
		}
		UpdateBackground();
	}

	public void ShowTextWhite()
	{
		SetTextColour(new Color(1f, 1f, 1f, 1f));
	}

	public void ShowTextBlack()
	{
		SetTextColour(new Color(0f, 0f, 0f, 1f));
	}

	public bool CheckProgress(QuestEvent.Type TestEvent, bool BotOnly, object ExtraData)
	{
		if (m_Event.DoesTypeMatch(TestEvent, BotOnly, ExtraData))
		{
			UpdateProgress();
			return true;
		}
		return false;
	}

	private void UpdateBackground()
	{
		if (m_Event.m_Locked || m_Event.m_Complete)
		{
			SetColour(new Color(1f, 1f, 1f, 0f));
		}
		else
		{
			SetColour(new Color(1f, 1f, 1f, 0.5f));
		}
	}

	public override void OnPointerEnter(PointerEventData eventData)
	{
		base.OnPointerEnter(eventData);
		UpdateBackground();
	}

	public override void OnPointerExit(PointerEventData eventData)
	{
		base.OnPointerExit(eventData);
		UpdateBackground();
	}

	public int GetVisibleLines()
	{
		m_EventText.m_Text.ForceMeshUpdate(ignoreActiveState: true);
		return m_EventText.m_Text.textInfo.lineCount;
	}
}
