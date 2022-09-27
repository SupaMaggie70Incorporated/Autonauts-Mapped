using SimpleJSON;
using UnityEngine;

public class TutorialPanelController : MonoBehaviour
{
	public enum Orientation
	{
		TopRight,
		BottomLeft,
		TopLeft,
		BottomRight,
		Total
	}

	public static TutorialPanelController Instance;

	private Orientation m_Orientation;

	public Quest m_Quest;

	private TutorialPanel[] m_Panels;

	private TutorialPanel m_CurrentPanel;

	private bool m_Active;

	private bool m_CeremonyActive;

	private Quest.ID m_StartQuest;

	private void Awake()
	{
		Instance = this;
		CheckGadgets();
		m_Orientation = Orientation.BottomLeft;
		m_StartQuest = Quest.ID.Total;
		m_Active = true;
		base.gameObject.SetActive(value: false);
	}

	public void Save(JSONNode Node)
	{
		if (m_Quest != null)
		{
			JSONUtils.Set(Node, "ActiveTutorial", QuestManager.Instance.m_Data.GetQuestNameFromID(m_Quest.m_ID));
		}
	}

	public void Load(JSONNode Node)
	{
		string asString = JSONUtils.GetAsString(Node, "ActiveTutorial", "Total");
		Quest.ID questIDFromName = QuestManager.Instance.m_Data.GetQuestIDFromName(asString);
		if (QuestData.Instance.m_TutorialData.GetFirstQuestFromQuest(questIDFromName) != Quest.ID.TutorialStart)
		{
			QuestManager.Instance.TutorialFinished();
			ModeButton.Get(ModeButton.Type.Autopedia).Show();
		}
		Quest quest = QuestManager.Instance.m_Data.GetQuest(questIDFromName);
		if (quest != null && !quest.m_Complete)
		{
			SetupFirstTutorial(questIDFromName);
		}
		else
		{
			m_StartQuest = Quest.ID.Total;
		}
	}

	private void CheckGadgets()
	{
		if (m_Panels == null)
		{
			m_Panels = new TutorialPanel[4];
			m_Panels[0] = base.transform.Find("TutorialPanelTopRight").GetComponent<TutorialPanel>();
			m_Panels[1] = base.transform.Find("TutorialPanelBottomLeft").GetComponent<TutorialPanel>();
			m_Panels[2] = base.transform.Find("TutorialPanelTopLeft").GetComponent<TutorialPanel>();
			m_Panels[3] = base.transform.Find("TutorialPanelBottomRight").GetComponent<TutorialPanel>();
			for (int i = 0; i < m_Panels.Length; i++)
			{
				m_Panels[i].CheckGadgets();
				m_Panels[i].SetActive(Active: false);
			}
			m_CurrentPanel = Object.Instantiate(m_Panels[1], base.transform);
			m_CurrentPanel.SetActive(Active: true);
		}
	}

	public void SetActive(bool Active)
	{
		m_Active = Active;
		UpdateActive();
	}

	public bool GetActive()
	{
		return base.gameObject.activeSelf;
	}

	public void DoSpeechFromID(string NewSpeech)
	{
		m_CurrentPanel.DoSpeechFromID(NewSpeech);
	}

	public void DoError()
	{
		m_CurrentPanel.DoError();
	}

	private void UpdateActive()
	{
		if (m_CeremonyActive || m_Quest == null)
		{
			base.gameObject.SetActive(value: false);
		}
		else
		{
			base.gameObject.SetActive(m_Active);
		}
		UpdateOrientation();
	}

	public void CeremonyActive(bool Active)
	{
		m_CeremonyActive = Active;
		UpdateActive();
		TutorialPointerManager.Instance.CeremonyActive(Active);
	}

	public void SetQuest(Quest NewQuest)
	{
		m_Quest = NewQuest;
		UpdateActive();
		CheckGadgets();
		m_CurrentPanel.SetQuest(NewQuest);
		TutorialScriptManager.Instance.StartQuest();
	}

	public void ResetQuest()
	{
		m_Quest.Reset();
		m_Quest.m_Active = true;
		m_Quest.m_Started = true;
		m_CurrentPanel.UpdateEvents();
		m_CurrentPanel.EventClicked(0);
		QuestManager.Instance.CreateActiveEventsArray();
		TutorialScriptManager.Instance.StartQuest();
		m_CurrentPanel.DoError();
	}

	public void UpdateEvent(QuestEvent.Type TestEvent, bool BotOnly, object ExtraData)
	{
		if (m_Active)
		{
			m_CurrentPanel.UpdateEvent(TestEvent, BotOnly, ExtraData);
		}
	}

	private void SetOrientation(Orientation NewOrientation)
	{
		if (m_Orientation != NewOrientation)
		{
			CheckGadgets();
			m_Orientation = NewOrientation;
			ObjectUtils.TransferRectTransform(m_CurrentPanel.gameObject, m_Panels[(int)m_Orientation].gameObject);
			ObjectUtils.TransferRectTransform(m_CurrentPanel.m_Tutor.gameObject, m_Panels[(int)m_Orientation].m_Tutor.gameObject);
			ObjectUtils.TransferRectTransform(m_CurrentPanel.m_Dialog.gameObject, m_Panels[(int)m_Orientation].m_Dialog.gameObject);
			ObjectUtils.TransferRectTransform(m_CurrentPanel.m_EventList.gameObject, m_Panels[(int)m_Orientation].m_EventList.gameObject);
		}
	}

	public void EventClicked(int EventIndex)
	{
		m_CurrentPanel.EventClicked(EventIndex);
	}

	private void AllCompleted()
	{
		m_Quest = null;
		QuestManager.Instance.TutorialFinished();
		SetActive(Active: false);
		TutorialPointerManager.Instance.SetType(TutorialPointerManager.Type.Total);
	}

	public void NextQuest()
	{
		if (m_Quest == null)
		{
			return;
		}
		if (m_Quest.m_QuestsUnlocked.Count > 0)
		{
			Quest quest = QuestManager.Instance.m_Data.GetQuest(m_Quest.m_QuestsUnlocked[0]);
			SetQuest(quest);
			return;
		}
		Quest.ID iD = m_Quest.m_ID;
		AllCompleted();
		if (QuestData.Instance.m_TutorialData.GetInfoFromQuestID(iD).m_LessonNumber == TutorialData.Lesson.First)
		{
			QuestManager.Instance.AddEvent(QuestEvent.Type.CompleteTutorial, Bot: false, null, null);
			if (GameOptionsManager.Instance.m_Options.m_GameMode == GameOptions.GameMode.ModeCampaign)
			{
				QuestManager.Instance.AddQuest(Quest.ID.AcademyBasics);
			}
			CeremonyManager.Instance.AddCeremony(CeremonyManager.CeremonyType.TutorialFinished);
			BadgeManager.Instance.AddEvent(BadgeEvent.Type.Tutorial);
		}
		TabQuests.Instance.UpdateAll();
		m_StartQuest = Quest.ID.Total;
	}

	public void QuestCompleted(Quest CompletedQuest)
	{
		if (CompletedQuest == m_Quest)
		{
			m_CurrentPanel.QuestCompleted();
		}
	}

	public void SkipTutorial()
	{
		if (m_Quest != null)
		{
			m_CurrentPanel.QuestCompleted();
			QuestManager.Instance.CheatCompleteQuest(m_Quest);
			NextQuest();
		}
	}

	public void EndTutorial(bool Complete)
	{
		if (!Complete)
		{
			if ((bool)TutorBot.Instance)
			{
				TutorBot.Instance.StopUsing();
			}
			m_CurrentPanel.QuestCompleted();
			StopOldTutorial();
			m_Quest = null;
			m_StartQuest = Quest.ID.Total;
			AllCompleted();
			return;
		}
		if ((bool)TutorBot.Instance)
		{
			TutorBot.Instance.StopUsing();
		}
		m_CurrentPanel.QuestCompleted();
		if (Complete)
		{
			while (m_Quest != null)
			{
				QuestManager.Instance.AddQuest(m_Quest.m_ID);
				QuestManager.Instance.CheatCompleteQuest(m_Quest);
				if (m_Quest.m_QuestsUnlocked.Count > 0)
				{
					Quest.ID newID = m_Quest.m_QuestsUnlocked[0];
					m_Quest = QuestManager.Instance.m_Data.GetQuest(newID);
				}
				else
				{
					m_Quest = null;
				}
			}
		}
		m_StartQuest = Quest.ID.Total;
		AllCompleted();
		if (Complete)
		{
			BadgeManager.Instance.AddEvent(BadgeEvent.Type.Tutorial);
			if (GameOptionsManager.Instance.m_Options.m_GameMode == GameOptions.GameMode.ModeCampaign)
			{
				QuestManager.Instance.AddQuest(Quest.ID.AcademyBasics);
				QuestManager.Instance.CheatCompleteQuest(QuestManager.Instance.GetQuest(Quest.ID.AcademyBasics));
			}
		}
	}

	public void StartTutorial()
	{
		if (m_StartQuest != Quest.ID.Total)
		{
			SetQuest(QuestManager.Instance.m_Data.GetQuest(m_StartQuest));
		}
	}

	public void PostLoad()
	{
		StartTutorial();
		if (m_StartQuest != Quest.ID.Total)
		{
			SetActive(Active: true);
		}
		else
		{
			AllCompleted();
		}
	}

	private void StopOldTutorial()
	{
		if (m_StartQuest == Quest.ID.Total)
		{
			return;
		}
		Quest.ID firstQuestFromQuest = QuestData.Instance.m_TutorialData.GetFirstQuestFromQuest(m_StartQuest);
		Quest quest = QuestData.Instance.GetQuest(firstQuestFromQuest);
		do
		{
			foreach (QuestEvent item in quest.m_EventsRequired)
			{
				item.m_Locked = true;
			}
			QuestManager.Instance.ResetQuest(quest);
			quest = ((quest.m_QuestsUnlocked.Count <= 0) ? null : QuestData.Instance.GetQuest(quest.m_QuestsUnlocked[0]));
		}
		while (quest != null);
	}

	public void SetupFirstTutorial(Quest.ID StartID)
	{
		StopOldTutorial();
		m_StartQuest = StartID;
		if (m_StartQuest != Quest.ID.Total)
		{
			QuestManager.Instance.AddQuest(StartID);
		}
		if (QuestData.Instance.m_TutorialData.GetFirstQuestFromQuest(m_StartQuest) == Quest.ID.TutorialStart)
		{
			m_CurrentPanel.SetSkipStepActive(Active: false);
		}
		else
		{
			m_CurrentPanel.SetSkipStepActive(Active: true);
		}
	}

	public void GoBackAStep()
	{
		m_CurrentPanel.GoBackAStep();
	}

	public bool GetIsFirstDigging()
	{
		if (m_Active && m_CurrentPanel.m_Quest != null && m_CurrentPanel.m_Quest.m_ID == Quest.ID.TutorialTeaching2)
		{
			return true;
		}
		return false;
	}

	private void UpdateOrientation()
	{
		Orientation orientation = Orientation.TopLeft;
		GameStateManager.State actualState = GameStateManager.Instance.GetActualState();
		switch (actualState)
		{
		case GameStateManager.State.Confirm:
			return;
		case GameStateManager.State.EditArea:
			orientation = Orientation.TopRight;
			break;
		case GameStateManager.State.SelectWorker:
		case GameStateManager.State.SelectObject:
			orientation = Orientation.BottomRight;
			break;
		default:
			if ((bool)TeachWorkerScriptEdit.Instance.m_CurrentTarget || actualState == GameStateManager.State.SelectWorker)
			{
				orientation = Orientation.TopRight;
			}
			else if (actualState == GameStateManager.State.Autopedia)
			{
				orientation = Orientation.BottomLeft;
			}
			break;
		}
		SetOrientation(orientation);
	}

	private void UpdateVisibility()
	{
		bool active = true;
		GameStateManager.State actualState = GameStateManager.Instance.GetActualState();
		if (actualState == GameStateManager.State.Inventory || actualState == GameStateManager.State.DragInventorySlot)
		{
			active = false;
		}
		m_CurrentPanel.SetActive(active);
	}

	private void Update()
	{
		UpdateOrientation();
		UpdateVisibility();
	}
}
