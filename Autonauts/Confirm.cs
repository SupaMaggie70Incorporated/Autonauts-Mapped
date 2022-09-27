public class Confirm : BaseMenu
{
	private ConfirmCallback m_Callback;

	private ConfirmCallback m_Cancel;

	protected new void Start()
	{
		base.Start();
		AddAction(base.transform.Find("BasePanelOptions").Find("NoButton").GetComponent<BaseGadget>(), OnNo);
		AddAction(base.transform.Find("BasePanelOptions").Find("YesButton").GetComponent<BaseGadget>(), OnYes);
	}

	public void OnNo(BaseGadget NewGadget)
	{
		GameStateManager.Instance.PopState();
		if (m_Cancel != null)
		{
			m_Cancel();
		}
	}

	public void OnYes(BaseGadget NewGadget)
	{
		GameStateManager.Instance.PopState();
		m_Callback();
	}

	public void SetTitle(string Title)
	{
		base.transform.Find("BasePanelOptions").Find("Panel/TitleStrip/Title").GetComponent<BaseText>()
			.SetTextFromID(Title);
	}

	public void SetDescription(string Description)
	{
		base.transform.Find("BasePanelOptions").Find("Description").GetComponent<BaseText>()
			.SetTextFromID(Description);
	}

	public void SetConfirm(ConfirmCallback Callback, ConfirmCallback Cancel = null)
	{
		m_Callback = Callback;
		m_Cancel = Cancel;
	}
}
