using UnityEngine;

public class Certificate : BasePanel
{
	private CertificateInfo m_Info;

	public Quest m_Quest;

	private Pin m_Pin;

	private StandardProgressBar m_ProgressBar;

	private GameObject m_LockedPanel;

	private LockedByResearch m_LockedByResearch;

	private GameObject m_Book;

	private GameObject m_Certificate;

	private float m_FlashTimer;

	public static Color GetColour(Quest NewQuest)
	{
		return (new Color(1f, 1f, 1f, 1f) - NewQuest.m_Colour) * 0.5f + NewQuest.m_Colour;
	}

	private void UpdateBookColour()
	{
		Color color = new Color(1f, 1f, 1f, 1f);
		if (GetLocked())
		{
			color = new Color(0.25f, 0.25f, 0.25f, 1f);
		}
		Color colour = GetColour(m_Quest) * color;
		m_Book.transform.Find("Middle").GetComponent<BaseImage>().SetColour(colour);
		m_Book.transform.Find("Front").GetComponent<BaseImage>().SetColour(color);
		m_Book.transform.Find("Image").GetComponent<BaseImage>().SetColour(color);
		m_Book.GetComponent<BaseImage>().SetColour(color);
	}

	private void SetupBook()
	{
		m_Book = base.transform.Find("Book").gameObject;
		m_Book.transform.Find("Image").GetComponent<BaseImage>().SetSprite(m_Quest.GetIconName());
		m_Book.transform.Find("Title").GetComponent<BaseText>().SetTextFromID(m_Quest.m_Title);
		m_Book.transform.Find("Description").GetComponent<BaseText>().SetTextFromID(m_Quest.m_Description);
		m_ProgressBar = m_Book.transform.Find("StandardProgressBar").GetComponent<StandardProgressBar>();
		m_Pin = m_Book.transform.Find("Pin").GetComponent<Pin>();
		m_Pin.SetAction(OnPinClicked, m_Pin);
		m_Pin.SetQuest(m_Quest, m_Info);
		m_LockedPanel = m_Book.transform.Find("LockedPanel").gameObject;
		m_LockedByResearch = m_LockedPanel.transform.Find("LockedByResearch").GetComponent<LockedByResearch>();
		if (m_Info.m_ResearchIDs.Count > 0)
		{
			m_LockedByResearch.Init(m_Info.m_ResearchIDs[0]);
		}
		BaseImage component = m_Book.transform.Find("Middle").GetComponent<BaseImage>();
		component.m_OnEnterEvent = OnEnter;
		component.m_OnExitEvent = OnExit;
		component.SetAction(OnClicked, component);
		UpdateBookColour();
	}

	private void SetupCertificate()
	{
		m_Certificate = base.transform.Find("Certificate").gameObject;
		m_Certificate.transform.Find("Image").GetComponent<BaseImage>().SetSprite(m_Quest.GetIconName());
		m_Certificate.transform.Find("Title").GetComponent<BaseText>().SetTextFromID(m_Quest.m_Title);
		m_Certificate.transform.Find("Description").GetComponent<BaseText>().SetTextFromID(m_Quest.m_Description);
		BaseImage component = m_Certificate.GetComponent<BaseImage>();
		component.m_OnEnterEvent = OnEnter;
		component.m_OnExitEvent = OnExit;
		component.SetAction(OnClicked, component);
		Color colour = GetColour(m_Quest);
		component = m_Certificate.GetComponent<BaseImage>();
		component.SetColour(colour);
	}

	public void SetQuest(Quest.ID NewID)
	{
		SetAction(OnClicked, this);
		Quest quest = (m_Quest = QuestManager.Instance.GetQuest(NewID));
		m_Info = QuestData.Instance.m_AcademyData.GetInfoFromQuestID(NewID);
		SetupBook();
		SetupCertificate();
		UpdateLocked();
		UpdateProgressBar();
		UpdatePin();
		UpdateComplete();
		UpdateImportant();
		UpdateLesson();
	}

	public void UpdateLesson()
	{
		LessonButton component = m_Book.transform.Find("LessonButton").GetComponent<LessonButton>();
		component.SetAction(OnLessonClicked, component);
		component.SetQuest(m_Quest);
		component.UpdateNew(UpdatePaused: true);
		CertificateInfo infoFromQuestID = QuestData.Instance.m_AcademyData.GetInfoFromQuestID(m_Quest.m_ID);
		if (infoFromQuestID.m_LessonID == Quest.ID.Total || QuestData.Instance.GetQuest(infoFromQuestID.m_LessonID).m_Complete)
		{
			component.SetActive(Active: false);
		}
		else
		{
			component.SetActive(Active: true);
		}
	}

	public void OnLessonClicked(BaseGadget NewGadget)
	{
		CertificateInfo infoFromQuestID = QuestData.Instance.m_AcademyData.GetInfoFromQuestID(m_Quest.m_ID);
		TutorialPanelController.Instance.SetupFirstTutorial(infoFromQuestID.m_LessonID);
		TutorialPanelController.Instance.StartTutorial();
		TutorialPanelController.Instance.SetActive(Active: true);
		QuestManager.Instance.AddEvent(QuestEvent.Type.SelectAutopedia, Bot: false, null, null);
	}

	public void UpdateLessonButton()
	{
		if (m_Quest.m_ID == Quest.ID.AcademyLumber2)
		{
			m_Book.transform.Find("LessonButton").GetComponent<LessonButton>().UpdateNew(UpdatePaused: true);
		}
	}

	private bool GetLocked()
	{
		if (m_Info.m_ResearchIDs.Count == 0)
		{
			return false;
		}
		return !QuestManager.Instance.GetQuest(m_Info.m_ResearchIDs[0]).GetIsComplete();
	}

	private void UpdateLocked()
	{
		if (!GetLocked())
		{
			m_LockedPanel.SetActive(value: false);
		}
		else
		{
			m_LockedPanel.SetActive(value: true);
		}
	}

	private void UpdateProgressBar()
	{
		if (!GetLocked() && !m_Quest.GetIsComplete())
		{
			if (m_Quest.GetCompletePercent() != 0f)
			{
				m_ProgressBar.SetActive(Active: true);
				m_ProgressBar.SetValue(m_Quest.GetCompletePercent());
			}
			else
			{
				m_ProgressBar.SetActive(Active: false);
			}
		}
		else
		{
			m_ProgressBar.SetActive(Active: false);
		}
	}

	public void OnLockedPanelClicked(BaseGadget NewGadget)
	{
		Autopedia.Instance.SetPage(Autopedia.Page.Research);
		Quest.ID research = m_Info.m_ResearchIDs[0];
		Research.Instance.SetResearch(research);
	}

	public void OnPinClicked(BaseGadget NewGadget)
	{
		m_Quest.m_Pinned = !m_Quest.m_Pinned;
		if (m_Quest.m_Pinned)
		{
			Autopedia.Instance.StartPinned(m_Quest);
		}
		UpdatePin();
		TabQuests.Instance.UpdateLists();
	}

	private void UpdatePin()
	{
		m_Pin.UpdateStatus();
	}

	private void UpdateComplete()
	{
		m_Book.gameObject.SetActive(!m_Quest.GetIsComplete());
		m_Certificate.gameObject.SetActive(m_Quest.GetIsComplete());
	}

	public void ForceComplete(bool Complete)
	{
		m_Book.gameObject.SetActive(!Complete);
		m_Certificate.gameObject.SetActive(Complete);
	}

	private void UpdateImportant()
	{
		GameObject gameObject = m_Book.transform.Find("Important").gameObject;
		bool active = false;
		foreach (ObjectType item in m_Quest.m_BuildingsUnlocked)
		{
			if (Wonder.GetIsTypeWonder(item))
			{
				active = true;
			}
		}
		gameObject.SetActive(active);
	}

	public void OnEnter(BaseGadget NewGadget)
	{
		HudManager.Instance.ActivateQuestRollover(Activate: true, m_Quest);
	}

	public void OnExit(BaseGadget NewGadget)
	{
		HudManager.Instance.ActivateQuestRollover(Activate: false, null);
	}

	public void OnClicked(BaseGadget NewGadget)
	{
		if (CheatManager.Instance.m_CheatMissions && Input.GetKey(KeyCode.LeftShift))
		{
			QuestManager.Instance.CheatCompleteQuest(m_Quest);
		}
	}

	public void FlashSelected()
	{
		m_FlashTimer = 2f;
	}

	private void Update()
	{
		if (!(m_FlashTimer > 0f))
		{
			return;
		}
		m_FlashTimer -= TimeManager.Instance.m_PauseDelta;
		if (m_FlashTimer <= 0f)
		{
			m_Book.SetActive(value: true);
			return;
		}
		bool active = true;
		if ((int)(m_FlashTimer * 60f) % 12 < 6)
		{
			active = false;
		}
		m_Book.SetActive(active);
	}
}
