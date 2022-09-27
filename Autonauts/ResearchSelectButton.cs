public class ResearchSelectButton : StandardButtonBlueprint
{
	public Quest m_Quest;

	private BaseText m_CostText;

	protected new void Awake()
	{
		base.Awake();
		m_CostText = base.transform.Find("Cost").GetComponent<BaseText>();
		m_CostText.SetActive(Active: false);
	}

	public void SetQuest(Quest.ID NewID)
	{
		m_Quest = QuestManager.Instance.GetQuest(NewID);
		int required = m_Quest.m_EventsRequired[0].m_Required;
		m_CostText.SetText(GeneralUtils.FormatBigInt(required));
		m_CostText.SetActive(Active: true);
	}

	public override void SetIndicated(bool Indicated)
	{
		base.SetIndicated(Indicated);
		CheckGadgets();
		m_Border.gameObject.SetActive(Indicated);
		HudManager.Instance.ActivateConverterRollover(Activate: false, m_Type);
		HudManager.Instance.ActivateQuestRollover(Indicated, m_Quest);
	}
}
