public class LockedByTickets : BasePanel
{
	private ObjectType m_Type;

	protected new void Awake()
	{
		base.Awake();
		if (QuestManager.Instance.GetQuest(Quest.ID.ResearchLevel1) != null && !QuestManager.Instance.GetQuest(Quest.ID.ResearchLevel1).GetIsComplete())
		{
			SetInteractable(Interactable: false);
		}
	}

	public void Init(ObjectType NewType)
	{
		m_Type = NewType;
		int variableAsInt = VariableManager.Instance.GetVariableAsInt(m_Type, "PrizeCost");
		base.transform.Find("Title").GetComponent<BaseText>().SetText(variableAsInt.ToString());
		bool interactable = OffworldMissionsManager.Instance.m_Tickets >= variableAsInt;
		StandardButtonText component = base.transform.Find("BuyButton").GetComponent<StandardButtonText>();
		component.SetInteractable(interactable);
		component.SetAction(OnClicked, this);
	}

	public void OnClicked(BaseGadget NewGadget)
	{
		SetActive(Active: false);
		OffworldMissionsManager.Instance.BuyPrize(m_Type);
		Objects.Instance.UpdateTickets();
	}
}
