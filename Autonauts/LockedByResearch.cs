public class LockedByResearch : BasePanel
{
	private Quest.ID m_ID;

	protected new void Awake()
	{
		base.Awake();
		SetAction(OnClicked, this);
		if (QuestManager.Instance.GetQuest(Quest.ID.ResearchLevel1) != null && !QuestManager.Instance.GetQuest(Quest.ID.ResearchLevel1).GetIsComplete())
		{
			SetInteractable(Interactable: false);
		}
	}

	public void Init(Quest.ID NewID)
	{
		m_ID = NewID;
		ResearchInfo infoFromQuestID = QuestData.Instance.m_ResearchData.GetInfoFromQuestID(NewID);
		Quest quest = QuestManager.Instance.GetQuest(infoFromQuestID.m_ID);
		BaseText component = base.transform.Find("Level").GetComponent<BaseText>();
		string text = TextManager.Instance.Get("AcademyResearchLevel", (infoFromQuestID.m_Level + 1).ToString());
		component.SetText(text);
		base.transform.Find("Title").GetComponent<BaseText>().SetTextFromID(quest.m_Title);
	}

	public void OnClicked(BaseGadget NewGadget)
	{
		Autopedia.Instance.SetPage(Autopedia.Page.Research);
		Research.Instance.SetResearch(m_ID);
	}
}
