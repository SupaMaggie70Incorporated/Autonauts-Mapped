using System.Collections.Generic;
using UnityEngine;

public class TutorialEventList : BaseScrollView
{
	private Quest m_Quest;

	private EventComplete m_DefaultEventComplete;

	private List<EventComplete> m_Events;

	private BaseText m_Title;

	private bool m_ReadyCameraMoveEvent;

	private bool m_ResetScroll;

	protected new void Awake()
	{
		base.Awake();
		m_Events = new List<EventComplete>();
	}

	private void CheckGadgets()
	{
		if (!m_DefaultEventComplete)
		{
			m_DefaultEventComplete = GetContent().transform.Find("EventComplete").GetComponent<EventComplete>();
			m_DefaultEventComplete.gameObject.SetActive(value: false);
			m_Title = base.transform.Find("EventTitle").GetComponent<BaseText>();
		}
	}

	private void DestroyEvents()
	{
		foreach (EventComplete @event in m_Events)
		{
			Object.Destroy(@event.gameObject);
		}
		m_Events.Clear();
	}

	private void CreateEvents()
	{
		float num = -5f;
		float y = m_DefaultEventComplete.GetComponent<RectTransform>().sizeDelta.y;
		float num2 = 10f;
		float num3 = 0f;
		foreach (QuestEvent item in m_Quest.m_EventsRequired)
		{
			EventComplete eventComplete = Object.Instantiate(m_DefaultEventComplete, GetContent().transform);
			eventComplete.gameObject.SetActive(value: true);
			eventComplete.SetEvent(item, m_Quest);
			m_Events.Add(eventComplete);
			num3 += y;
			if (eventComplete.GetVisibleLines() > 1)
			{
				num3 += num2;
				Vector2 sizeDelta = eventComplete.GetComponent<RectTransform>().sizeDelta;
				sizeDelta.y += num2;
				eventComplete.GetComponent<RectTransform>().sizeDelta = sizeDelta;
			}
		}
		float scrollSize = num3 - 5f;
		SetScrollSize(scrollSize);
		foreach (EventComplete @event in m_Events)
		{
			@event.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, num);
			num -= y;
			if (@event.GetVisibleLines() > 1)
			{
				num -= num2;
			}
		}
	}

	public void UnlockEvent(int Index)
	{
		m_Events[Index].m_Event.m_Locked = false;
		m_Events[Index].UpdateProgress();
		switch (m_Events[Index].m_Event.m_Type)
		{
		case QuestEvent.Type.MoveCamera:
			m_ReadyCameraMoveEvent = true;
			break;
		case QuestEvent.Type.ZoomCamera:
			CameraManager.Instance.ReadyZoomEvent();
			break;
		}
		float num = 0f - m_Events[Index].transform.localPosition.y + GetComponent<RectTransform>().sizeDelta.y / 2f;
		num += base.verticalScrollbar.GetComponent<RectTransform>().sizeDelta.y - 2f;
		float num2 = GetScrollSize() - GetHeight();
		float scrollValue = 1f - num / num2;
		SetScrollValue(scrollValue);
	}

	public int UpdateEvent(QuestEvent.Type TestEvent, bool BotOnly, object ExtraData)
	{
		for (int i = 0; i < m_Events.Count; i++)
		{
			EventComplete eventComplete = m_Events[i];
			if (i != m_Events.Count - 1 && m_Events[i + 1].m_Event.m_Locked && eventComplete.CheckProgress(TestEvent, BotOnly, ExtraData))
			{
				if (eventComplete.m_Event.m_Complete)
				{
					_ = m_Events.Count - 1;
				}
				return i;
			}
			if (i == m_Events.Count - 1 && eventComplete.CheckProgress(TestEvent, BotOnly, ExtraData))
			{
				return i;
			}
		}
		return -1;
	}

	public void UpdateAllEvents()
	{
		foreach (EventComplete @event in m_Events)
		{
			@event.m_Event.m_Locked = true;
			@event.UpdateProgress();
		}
		UnlockEvent(0);
	}

	public void OnEventClicked(BaseGadget NewGadget)
	{
		int eventIndex = m_Events.IndexOf(NewGadget.GetComponent<EventComplete>());
		TutorialPanelController.Instance.EventClicked(eventIndex);
	}

	private void UpdateTitle()
	{
		m_Title.SetTextFromID(m_Quest.m_Title);
	}

	public int SetQuest(Quest NewQuest)
	{
		CheckGadgets();
		DestroyEvents();
		m_Quest = NewQuest;
		CreateEvents();
		m_ResetScroll = true;
		UpdateTitle();
		int num = 0;
		foreach (EventComplete @event in m_Events)
		{
			if (!@event.m_Event.m_Complete)
			{
				UnlockEvent(num);
				return num;
			}
			num++;
		}
		return num;
	}

	public int SkipStep()
	{
		for (int i = 0; i < m_Events.Count; i++)
		{
			_ = m_Events[i];
			if (!m_Events[i].m_Event.m_Locked)
			{
				m_Events[i].m_Event.SetProgress(m_Events[i].m_Event.m_Required);
				m_Events[i].m_Event.m_Locked = true;
				m_Events[i].UpdateProgress();
				i++;
				if (i != m_Events.Count)
				{
					UnlockEvent(i);
				}
				return i;
			}
		}
		return -1;
	}

	public int GoBackAStep()
	{
		for (int i = 0; i < m_Events.Count; i++)
		{
			_ = m_Events[i];
			if (!m_Events[i].m_Event.m_Locked && !m_Events[i].m_Event.m_Complete)
			{
				m_Events[i].m_Event.m_Locked = true;
				m_Events[i].UpdateProgress();
				i--;
				m_Events[i].m_Event.Reset();
				UnlockEvent(i);
				return i;
			}
		}
		return -1;
	}

	private void CheckValidEvent()
	{
		if (TeachWorkerScriptEdit.Instance == null || GameStateManager.Instance == null)
		{
			return;
		}
		for (int i = 0; i < m_Events.Count; i++)
		{
			_ = m_Events[i];
			if (m_Events[i].m_Event.m_Locked)
			{
				continue;
			}
			if (m_Events[i].m_Event.m_Type == QuestEvent.Type.ClickStop)
			{
				if (TeachWorkerScriptEdit.Instance.gameObject.activeSelf && TeachWorkerScriptEdit.Instance.m_State == TeachWorkerScriptEdit.State.Idle)
				{
					QuestManager.Instance.AddEvent(QuestEvent.Type.ClickStop, Bot: false, null, null);
				}
			}
			else if (m_Events[i].m_Event.m_Type == QuestEvent.Type.SelectBot)
			{
				if (GameStateManager.Instance.GetActualState() == GameStateManager.State.Normal && GameStateManager.Instance.GetCurrentState().GetComponent<GameStateNormal>().GetSelectedWorker() != null)
				{
					QuestManager.Instance.AddEvent(QuestEvent.Type.SelectBot, Bot: false, null, null);
				}
			}
			else if (m_Events[i].m_Event.m_Type == QuestEvent.Type.Pickup)
			{
				List<BaseClass> players = CollectionManager.Instance.GetPlayers();
				string extraDataAsString = m_Events[i].m_Event.GetExtraDataAsString();
				ObjectType identifierFromSaveName = ObjectTypeList.Instance.GetIdentifierFromSaveName(extraDataAsString);
				if (players[0].GetComponent<Farmer>().m_FarmerCarry.GetTopObjectType() == identifierFromSaveName && TeachWorkerScriptEdit.Instance.m_State != TeachWorkerScriptEdit.State.Recording)
				{
					QuestManager.Instance.AddEvent(QuestEvent.Type.Pickup, Bot: false, identifierFromSaveName, null);
				}
			}
			else if (m_Events[i].m_Event.m_Type == QuestEvent.Type.GiveBotAnything)
			{
				Worker worker = null;
				if (GameStateManager.Instance.GetActualState() == GameStateManager.State.Normal)
				{
					worker = GameStateManager.Instance.GetCurrentState().GetComponent<GameStateNormal>().GetSelectedWorker();
				}
				if (GameStateManager.Instance.GetActualState() == GameStateManager.State.TeachWorker)
				{
					worker = GameStateManager.Instance.GetCurrentState().GetComponent<GameStateTeachWorker2>().m_CurrentTarget;
				}
				if (worker != null)
				{
					string extraDataAsString2 = m_Events[i].m_Event.GetExtraDataAsString();
					ObjectType identifierFromSaveName2 = ObjectTypeList.Instance.GetIdentifierFromSaveName(extraDataAsString2);
					if (worker.m_FarmerCarry.GetTopObjectType() == identifierFromSaveName2)
					{
						QuestManager.Instance.AddEvent(QuestEvent.Type.GiveBotAnything, Bot: false, identifierFromSaveName2, null);
					}
				}
			}
			else if (m_Events[i].m_Event.m_Type == QuestEvent.Type.Build && GameStateManager.Instance.GetActualState() == GameStateManager.State.Normal && GameOptionsManager.Instance.m_Options.m_GameMode == GameOptions.GameMode.ModeCreative)
			{
				string extraDataAsString3 = m_Events[i].m_Event.GetExtraDataAsString();
				ObjectType identifierFromSaveName3 = ObjectTypeList.Instance.GetIdentifierFromSaveName(extraDataAsString3);
				QuestManager.Instance.AddEvent(QuestEvent.Type.Build, Bot: false, identifierFromSaveName3, null);
			}
		}
	}

	private void Update()
	{
		if (m_ReadyCameraMoveEvent && GameStateManager.Instance.GetActualState() == GameStateManager.State.Normal)
		{
			m_ReadyCameraMoveEvent = false;
			CameraManager.Instance.ReadyMoveEvent();
		}
		if (m_ResetScroll)
		{
			m_ResetScroll = false;
			SetScrollValue(1f);
		}
		CheckValidEvent();
	}
}
