using UnityEngine;

public class TutorialPanel : MonoBehaviour
{
	public BasePanel m_Panel;

	public TutorialTutor m_Tutor;

	public TutorialDialog m_Dialog;

	public TutorialEventList m_EventList;

	private BaseButtonImage m_SkipStepButton;

	private bool m_Talking;

	public Quest m_Quest;

	private TutorialInfo m_TutorialInfo;

	private float m_ErrorTimer;

	private int m_CurrentEvent;

	private void Awake()
	{
		CheckGadgets();
		BaseButtonImage component = base.transform.Find("SkipLessonButton").GetComponent<BaseButtonImage>();
		component.SetAction(OnSkipLessonClicked, component);
		m_SkipStepButton = base.transform.Find("SkipStepButton").GetComponent<BaseButtonImage>();
		m_SkipStepButton.SetAction(OnSkipStepClicked, m_SkipStepButton);
	}

	public void CheckGadgets()
	{
		if (!m_Tutor)
		{
			m_Panel = base.transform.Find("Tutor").GetComponent<BasePanel>();
			m_Tutor = base.transform.Find("Tutor").GetComponent<TutorialTutor>();
			m_Dialog = base.transform.Find("SpeechBubble/Dialog").GetComponent<TutorialDialog>();
			m_EventList = base.transform.Find("EventList").GetComponent<TutorialEventList>();
		}
	}

	public void ConfirmSkip()
	{
		TutorialPanelController.Instance.EndTutorial(Complete: false);
	}

	public void OnSkipLessonClicked(BaseGadget NewGadget)
	{
		GameStateManager.Instance.PushState(GameStateManager.State.Confirm);
		GameStateManager.Instance.GetCurrentState().GetComponent<GameStateConfirm>().SetConfirm(ConfirmSkip, "ConfirmSkipTutorial");
		GameStateManager.Instance.GetCurrentState().GetComponent<GameStateConfirm>().HideHud();
	}

	public void OnSkipStepClicked(BaseGadget NewGadget)
	{
		int num = m_EventList.SkipStep();
		if (num == m_Quest.m_EventsRequired.Count)
		{
			QuestManager.Instance.CheatCompleteQuest(m_Quest, DoCeremony: true);
		}
		else
		{
			EventClicked(num);
		}
	}

	public void SetActive(bool Active)
	{
		if (!Active)
		{
			m_Dialog.StopTalking();
		}
		base.gameObject.SetActive(Active);
	}

	public void SetSkipStepActive(bool Active)
	{
		m_SkipStepButton.SetActive(Active);
	}

	public void SetQuest(Quest NewQuest)
	{
		m_Dialog.StopTalking();
		CheckGadgets();
		m_Quest = NewQuest;
		m_TutorialInfo = QuestData.Instance.m_TutorialData.GetInfoFromQuestID(NewQuest.m_ID);
		int eventIndex = m_EventList.SetQuest(NewQuest);
		EventClicked(eventIndex);
	}

	public void DoSpeechFromID(string NewSpeech)
	{
		StartSpeech(NewSpeech);
	}

	public void QuestCompleted()
	{
		m_Dialog.StopTalking();
	}

	public void UpdateEvent(QuestEvent.Type TestEvent, bool BotOnly, object ExtraData)
	{
		int i = m_EventList.UpdateEvent(TestEvent, BotOnly, ExtraData);
		if (i != -1 && !m_Quest.m_EventsRequired[i].m_Locked)
		{
			for (; i != m_Quest.m_EventsRequired.Count && m_Quest.m_EventsRequired[i].m_Complete; i++)
			{
			}
			if (i != m_Quest.m_EventsRequired.Count)
			{
				EventClicked(i);
				m_EventList.UnlockEvent(i);
			}
			else
			{
				TutorialPointerManager.Instance.SetType(TutorialPointerManager.Type.Total);
			}
		}
	}

	public void UpdateEvents()
	{
		m_EventList.UpdateAllEvents();
	}

	private void StartSpeech(string DescriptionID)
	{
		m_Dialog.StartSpeechFromID(DescriptionID, Ceremony: false);
		m_Tutor.StartTalking();
		m_Talking = true;
	}

	private void UpdateSpeech()
	{
		if (m_Talking && !m_Dialog.GetIsTalking())
		{
			m_Tutor.StopTalking();
			m_Talking = false;
		}
	}

	public void EventClicked(int EventIndex)
	{
		if (EventIndex < m_TutorialInfo.m_EventPointers.Count && EventIndex >= 0)
		{
			TutorialPointerManager.Type type = m_TutorialInfo.m_EventPointers[EventIndex];
			TutorialPointerManager.Instance.SetType(type);
			string description = m_Quest.m_EventsRequired[EventIndex].m_Description;
			if (description != "" && TextManager.Instance.DoesExist(description))
			{
				StartSpeech(description);
				m_CurrentEvent = EventIndex;
			}
			if (type == TutorialPointerManager.Type.EditSearchArea)
			{
				TeachWorkerScriptEdit.Instance.FlashEditAreas(Flash: true);
			}
			else
			{
				TeachWorkerScriptEdit.Instance.FlashEditAreas(Flash: false);
			}
		}
	}

	public void DoError()
	{
		DoSpeechFromID("TutorialBadInstruction");
		m_ErrorTimer = 1f;
	}

	public void GoBackAStep()
	{
		int num = m_EventList.GoBackAStep();
		if (num != -1)
		{
			EventClicked(num);
		}
	}

	private void UpdateError()
	{
		if (!(m_ErrorTimer <= 0f))
		{
			if (TimeManager.Instance.m_PauseTimeEnabled)
			{
				m_ErrorTimer -= TimeManager.Instance.m_PauseDelta;
			}
			else
			{
				m_ErrorTimer -= TimeManager.Instance.m_NormalDelta;
			}
			if (m_ErrorTimer <= 0f)
			{
				EventClicked(m_CurrentEvent);
			}
		}
	}

	private void Update()
	{
		if (!(TimeManager.Instance == null))
		{
			UpdateSpeech();
			UpdateError();
		}
	}
}
