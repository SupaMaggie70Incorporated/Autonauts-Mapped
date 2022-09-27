using UnityEngine;

public class QuestPanel : BaseGadget
{
	public Quest m_Quest;

	private StandardProgressBar m_ProgressBar;

	private BaseText m_Name;

	private BaseText m_Description;

	private bool m_MissingIngredient;

	public BasePanel m_Panel;

	public bool m_Used;

	protected new void Awake()
	{
		base.Awake();
		m_Panel = base.transform.Find("BasePanel").GetComponent<BasePanel>();
		m_Panel.SetBorderVisible(Visible: false);
	}

	public void SetQuest(Quest NewQuest, bool New)
	{
		m_Quest = NewQuest;
		BaseImage component = base.transform.Find("Book").GetComponent<BaseImage>();
		component.m_OnEnterEvent = PointerEnter;
		component.m_OnExitEvent = PointerExit;
		m_ProgressBar = base.transform.Find("StandardProgressBar").GetComponent<StandardProgressBar>();
		m_Name = base.transform.Find("Name").GetComponent<BaseText>();
		m_Name.SetTextFromID(NewQuest.m_Title);
		base.transform.Find("BaseImage").GetComponent<BaseImage>().SetSprite(m_Quest.GetIconName());
		UpdateStatus();
		UpdateLesson();
		UpdatePanelColour();
		UpdateScale();
		UpdateBookColour();
	}

	public void UpdateLesson()
	{
		LessonButton component = base.transform.Find("LessonButton").GetComponent<LessonButton>();
		component.SetAction(OnLessonClicked, component);
		component.SetQuest(m_Quest);
		UpdateLessonNew();
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

	public void UpdateLessonNew()
	{
		if (m_Quest.m_ID == Quest.ID.AcademyLumber2)
		{
			base.transform.Find("LessonButton").GetComponent<LessonButton>().UpdateNew();
		}
	}

	public void OnLessonClicked(BaseGadget NewGadget)
	{
		CertificateInfo infoFromQuestID = QuestData.Instance.m_AcademyData.GetInfoFromQuestID(m_Quest.m_ID);
		TutorialPanelController.Instance.SetupFirstTutorial(infoFromQuestID.m_LessonID);
		TutorialPanelController.Instance.StartTutorial();
		TutorialPanelController.Instance.SetActive(Active: true);
	}

	public bool AnyActive()
	{
		if (m_Quest != null && !m_Quest.GetIsComplete())
		{
			return true;
		}
		return false;
	}

	public void SetMissing(bool Missing)
	{
		m_MissingIngredient = Missing;
	}

	public void PointerEnter(BaseGadget NewGadget)
	{
		HudManager.Instance.ActivateQuestRollover(Activate: true, m_Quest);
		m_Panel.SetBorderVisible(Visible: true);
	}

	public void PointerExit(BaseGadget NewGadget)
	{
		HudManager.Instance.ActivateQuestRollover(Activate: false, null);
		m_Panel.SetBorderVisible(Visible: false);
	}

	private void OnDisable()
	{
		float num = 1f;
		base.transform.localScale = new Vector3(num, num, num);
	}

	private void UpdateStatus()
	{
		float completePercent = m_Quest.GetCompletePercent();
		m_ProgressBar.SetValue(completePercent);
	}

	private void UpdateBookColour()
	{
		base.transform.Find("Book").GetComponent<BaseImage>().SetColour(Certificate.GetColour(m_Quest));
	}

	private void UpdatePanelColour()
	{
		Color backingColour = new Color(1f, 1f, 1f, 1f);
		if (m_MissingIngredient && HudManager.Instance.m_ConverterRollover.m_MissingFlashOn)
		{
			backingColour = new Color(1f, 0f, 0f, 1f);
		}
		if (m_Quest.GetState() == Quest.State.Unavailable && !m_Quest.m_MajorNode)
		{
			float num = 0.35f;
			backingColour.r *= num;
			backingColour.g *= num;
			backingColour.b *= num;
		}
		m_Panel.SetBackingColour(backingColour);
	}

	private float GetScale()
	{
		float result = 1f;
		if (m_Quest.GetState() == Quest.State.Unavailable)
		{
			result = 0.75f;
		}
		return result;
	}

	private void UpdateScale()
	{
		float scale = GetScale();
		base.transform.localScale = new Vector3(scale, scale, scale);
	}

	public new float GetHeight()
	{
		float scale = GetScale();
		return GetComponent<RectTransform>().sizeDelta.y * scale;
	}

	public void UpdateAll()
	{
		UpdateStatus();
		UpdateLesson();
	}

	private void Update()
	{
		UpdatePanelColour();
		UpdateScale();
	}
}
