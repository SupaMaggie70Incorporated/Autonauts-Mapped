public class LockedByCertificate : BasePanel
{
	private Quest.ID m_ID;

	protected new void Awake()
	{
		base.Awake();
		SetAction(OnClicked, this);
	}

	public void Init(Quest.ID NewID)
	{
		m_ID = NewID;
		CertificateInfo infoFromQuestID = QuestData.Instance.m_AcademyData.GetInfoFromQuestID(NewID);
		Quest quest = QuestManager.Instance.GetQuest(infoFromQuestID.m_ID);
		base.transform.Find("Title").GetComponent<BaseText>().SetTextFromID(quest.m_Title);
		if (!quest.m_Started)
		{
			SetInteractable(Interactable: false);
		}
	}

	public void OnClicked(BaseGadget NewGadget)
	{
		Autopedia.Instance.SetPage(Autopedia.Page.Academy);
		Academy.Instance.SetCertficate(m_ID);
	}
}
